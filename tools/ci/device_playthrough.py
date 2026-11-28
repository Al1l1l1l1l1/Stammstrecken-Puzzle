"""Drive the installed Android development build by touching its visible controls."""
import json
import threading
import re
import shlex
import subprocess
import sys
import time
from pathlib import Path

PACKAGE, COMPONENT = sys.argv[1:3]
OUT = Path("DeviceResults")
OUT.mkdir(exist_ok=True)
metrics = {}
last = {}
def adb(*args, raw=False):
    data = subprocess.check_output(["adb", *args], timeout=90)
    return data if raw else data.decode().strip()


def command(text):
    return adb("shell", "run-as " + shlex.quote(PACKAGE) + " sh -c " + shlex.quote(text))


def page(sample=False):
    global last
    command(("echo sample" if sample else "echo state") + " > files/screen.request")
    for _ in range(140):
        if command("if test -f files/screen.request; then echo pending; else echo done; fi") == "done":
            last = json.loads(command("cat files/screen.json"))
            return last
        time.sleep(.2)
    raise RuntimeError("Android reagiert nicht auf Bildschirmabfrage")


def targets(p):
    return {t["name"]: t for t in p["targets"]}


def wait(check, title, timeout=100):
    end = time.monotonic() + timeout
    while time.monotonic() < end:
        p = page()
        if check(p):
            return p
        time.sleep(.25)
    raise RuntimeError(f"{title}: {last['status']}, {last['puzzle']}, {list(targets(last))}")


def ready(name, p):
    target = targets(p).get(name)
    return target and target["enabled"]


def tap(name):
    p = wait(lambda x: ready(name, x), name)
    t = targets(p)[name]
    adb("shell", "input", "tap", str(t["x"]), str(t["y"]))


def capture(name):
    print(f"Android: {name} ({last.get('status')}, {last.get('clears')} Verbindungen)", flush=True)
    (OUT / (name + ".png")).write_bytes(adb("exec-out","screencap","-p",raw=True))
    (OUT / (name + ".json")).write_text(json.dumps(last, ensure_ascii=False),encoding="utf8")


def cold(name, control):
    adb("shell","am","force-stop",PACKAGE)
    start = time.monotonic()
    adb("shell","am","start","-n",COMPONENT,"--ez","stpDeviceCheck","true")
    wait(lambda p: ready(control,p), name)
    metrics[name + "Seconds"] = round(time.monotonic() - start, 2)
    capture(name)


def measure(name, active=None):
    def touch():
        for _ in range(7):
            adb("shell","input","tap",str(active["x"]),str(active["y"]))
            time.sleep(.5)
    worker = threading.Thread(target=touch) if active else None
    if worker: worker.start()
    page(sample=True)
    if worker: worker.join()
    metrics[name + "Fps"] = round(last["measuredFps"], 1)
    mem = adb("shell","dumpsys","meminfo",PACKAGE)
    (OUT / (name + "-memory.txt")).write_text(mem)
    match = re.search(r"TOTAL PSS:\s*([\d,]+)",mem) or re.search(r"(?m)^\s*TOTAL\s+([\d,]+)",mem)
    if not match: raise ValueError("Speichernutzung fehlt")
    metrics[name + "MiB"] = round(int(match[1].replace(",",""))/1024,1)


def dismiss_post():
    for _ in range(4):
        p = page()
        if ready("mail-close",p): tap("mail-close")
        elif ready("result-skip-bonus",p): tap("result-skip-bonus")
        else: break


def solve_path(p):
    # Public clues suffice even for newly issued endless puzzles.
    width, height = p["width"], p["height"]
    start, finish = (p["ax"],p["ay"]), (p["bx"],p["by"])
    moves = [(0,-1),(1,0),(0,1),(-1,0)]
    rows, cols = p["rows"][:],p["columns"][:]
    total = sum(rows)
    def walk(cell, path, seen):
        x,y = cell
        if rows[y] <= 0 or cols[x] <= 0: return None
        rows[y] -= 1; cols[x] -= 1; path.append(cell); seen.add(cell)
        answer = None
        if cell == finish:
            if len(path) == total: answer = list(path)
        elif len(path) + abs(x-finish[0]) + abs(y-finish[1]) <= total:
            for dx,dy in moves:
                nxt = (x+dx,y+dy)
                if 0 <= nxt[0] < width and 0 <= nxt[1] < height and nxt not in seen:
                    answer = walk(nxt,path,seen)
                    if answer: break
        rows[y] += 1; cols[x] += 1; path.pop(); seen.remove(cell)
        return answer
    # The packaged fixed boards already have a bound solution; reserve search for small endless boards.
    fixed = list(Path("Content/Levels/s1").glob("*/*/" + p["puzzle"] + ".level-v2.json"))
    if fixed: path=[(c["x"],c["y"]) for c in json.loads(fixed[0].read_bytes())["solution"]["path"]]
    else: path=walk(start,[],set())
    if not path: raise ValueError("Keine Route für "+p["puzzle"])
    steps=[]
    shapes={5:3,10:4,3:5,6:6,12:7,9:8}
    for i,(x,y) in enumerate(path):
        def direction(c): return moves.index((c[0]-x,c[1]-y))
        a=direction(path[i-1]) if i else p["ae"]
        b=direction(path[i+1]) if i+1<len(path) else p["be"]
        steps.append((shapes[(1<<a)|(1<<b)],x,y))
    return steps


def solve(name):
    p=wait(lambda x: ready("puzzle-cell-0-0",x) and x["phase"]=="Editing",name)
    controls=targets(p)
    capture(name+"-board")
    if p["width"]==10: metrics["cellPixels10x10"]=controls["puzzle-cell-0-0"]["width"]
    for tool,x,y in solve_path(p):
        # Tool selection persists between cells.
        button=controls["tool-"+str(tool)]
        cell=controls[f"puzzle-cell-{x}-{y}"]
        # The grid reapplies the selected tool across cells; buttons set a tool explicitly.
        if tool != getattr(solve,"last_tool",None):
            adb("shell","input","tap",str(button["x"]),str(button["y"]))
            solve.last_tool=tool
        adb("shell","input","tap",str(cell["x"]),str(cell["y"]))
    solve.last_tool=None
    wait(lambda x: x["phase"]=="Results",name+" Ergebnis",150)
    time.sleep(4.2)
    capture(name+"-result")
    dismiss_post()

def home():
    for _ in range(7):
        p=page()
        if ready("home-play",p): return
        for name in ["mail-close","result-skip-bonus","result-home","map-back","nav-home","puzzle-back"]:
            if ready(name,p):
                tap(name)
                time.sleep(1)
                break
        else: time.sleep(1)
    wait(lambda p:ready("home-play",p),"Bahnsteig")


def select_cosmetic(item):
    print("Betriebshof: " + item, flush=True)
    action="cosmetic-action-"+item
    for _ in range(40):
        p=page()
        if ready(action,p): break
        if "depot-next" in targets(p):
            tap("depot-next")
        else:
            adb("shell","input","swipe","540","1780","540","680","480")
    tap(action)
    wait(lambda p:item in p["owned"],"Kauf "+item)
    if item not in last["equipped"]:
        tap(action)
        wait(lambda p:item in p["equipped"],"Ausrüsten "+item)


def main():
    cold("entry","home-play")
    tap("home-play")
    wait(lambda p:ready("puzzle-cell-0-0",p),"Erstes Rätsel")
    measure("puzzleActive",targets(last)["tool-1"])
    time.sleep(2.5)
    measure("puzzleIdle")
    solve("auftrag")
    home()
    cold("platform","home-play")
    measure("platform")
    storage=last["storage"]
    assert storage.endswith("/Android/data/"+PACKAGE+"/files")
    initial=command("cat "+shlex.quote(storage+"/star-times-debug.jsonl"))
    src=next(Path("seed").rglob("save.json"))
    relative=src.relative_to(Path("seed")/"DeviceSeed")
    destination=storage+"/"+relative.as_posix()
    adb("shell","am","force-stop",PACKAGE)
    adb("shell","pm","clear",PACKAGE)
    command("mkdir -p "+shlex.quote(destination.rsplit("/",1)[0]))
    remote="run-as "+shlex.quote(PACKAGE)+" sh -c "+shlex.quote("cat > "+shlex.quote(destination))
    subprocess.run(["adb","shell","-T",remote],input=src.read_bytes(),check=True)
    cold("season239","home-play")
    assert last["clears"]==239
    tap("home-play")
    solve("saisonfinale")
    assert last["clears"]==240
    home()
    tap("nav-depot")
    for item in ["train-u-bahn-muenchen","wagon-grossraum-sitzplatz-optional","livery-baustellengelb"]:
        select_cosmetic(item)
    capture("betriebshof")
    home()
    tap("home-endless")
    tap("endless-profile-dienstbeginn")
    played=set()
    for index in range(3):
        wait(lambda p:p["mode"]=="ENDLESS" and p["phase"]=="Editing" and p["puzzle"] not in played,"Nächste Dauerbaustelle")
        played.add(last["puzzle"])
        solve("dauerbaustelle-"+str(index+1))
        if index<2: tap("result-endless")
    home()
    tap("home-revision")
    tap("map-node-0")
    tap("map-dispatch")
    solve("betriebsrevision")
    timings=command("cat "+shlex.quote(storage+"/star-times-debug.jsonl"))
    records=[json.loads(line) for line in (initial+"\n"+timings).splitlines() if line.strip()]
    assert len(records)>=6 and all(set(row)=={"puzzleId","size","seconds","notesUsed","hintsUsed"} for row in records)
    metrics["completedEndless"]=len(played)
    metrics["revisionCompleted"]=True
    metrics["timings"]=len(records)


try:
    main()
finally:
    (OUT/"measurements.json").write_text(json.dumps(metrics,indent=2),encoding="utf8")
    print(json.dumps(metrics),flush=True)
    if last: capture("last-screen")
