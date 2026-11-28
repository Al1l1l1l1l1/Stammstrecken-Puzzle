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
    for _ in range(450):
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


def tap(name, p=None):
    if p is None:
        p = wait(lambda x: ready(name, x), name)
        # Result sheets move after a bonus offer or a scrolling gesture.
        if name.startswith("result-"):
            previous = None
            for _ in range(15):
                target = targets(p).get(name)
                current = (target["x"], target["y"]) if target and target["enabled"] else None
                if current is not None and current == previous: break
                previous = current
                time.sleep(.4)
                p = page()
            else: raise RuntimeError("Schaltfläche bewegt sich: " + name)
    t = targets(p)[name]
    adb("shell", "input", "tap", str(t["x"]), str(t["y"]))


def capture(name):
    print(f"Android: {name} ({last.get('status')}, {last.get('clears')} Verbindungen)", flush=True)
    (OUT / (name + ".png")).write_bytes(adb("exec-out","screencap","-p",raw=True))
    (OUT / (name + ".json")).write_text(json.dumps(last, ensure_ascii=False),encoding="utf8")


def cold(name, control):
    adb("shell","am","force-stop",PACKAGE)
    command("mkdir -p files")
    start = time.monotonic()
    adb("shell","am","start","-n",COMPONENT,"--ez","stpDeviceCheck","true")
    wait(lambda p: ready(control,p), name)
    metrics[name + "Seconds"] = round(time.monotonic() - start, 2)
    capture(name)


def measure(name, active=None):
    def touch():
        for _ in range(5):
            x, y = active["x"], active["y"]
            adb("shell","input","swipe",str(x),str(y),str(x+30),str(y+4),"420")
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


def capture_unity_profile(storage):
    remote = storage + "/wp049-android-profiler.raw"
    options = "-profiler-enable -profiler-log-file " + remote + " -profiler-capture-frame-count 90"
    adb("shell","wm","size","1080x2400")
    adb("shell","wm","density","420")
    adb("shell","am","force-stop",PACKAGE)
    adb("shell",shlex.join(["am","start","-n",COMPONENT,"--es","unity",options,"--ez","stpDeviceCheck","true"]))
    previous = 0
    stable = 0
    for _ in range(60):
        time.sleep(2)
        count = adb("shell", "if test -s " + shlex.quote(remote) +
                    "; then wc -c < " + shlex.quote(remote) + "; else echo 0; fi")
        size = int(count)
        stable = stable + 1 if size > 4096 and size == previous else 0
        if stable >= 3:
            output = OUT / "android-profiler.raw"
            adb("pull",remote,str(output))
            if output.stat().st_size == size: return
        previous = size
    raise RuntimeError("Die Unity-Profilaufzeichnung wurde nicht abgeschlossen")


def dismiss_post():
    for _ in range(4):
        p = page()
        if ready("mail-close",p): tap("mail-close",p)
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
    if p["width"]==10: metrics["cellPixels10x10At540"]=controls["puzzle-cell-0-0"]["width"]
    for tool,x,y in solve_path(p):
        # Tool selection persists between cells.
        button=controls["tool-"+str(tool)]
        cell=controls[f"puzzle-cell-{x}-{y}"]
        # The grid reapplies the selected tool across cells; buttons set a tool explicitly.
        if tool != getattr(solve,"last_tool",None):
            adb("shell","input","tap",str(button["x"]),str(button["y"]))
            page()
            solve.last_tool=tool
        adb("shell","input","tap",str(cell["x"]),str(cell["y"]))
        page()
    solve.last_tool=None
    def finished(state):
        if name == "dauerbaustelle-1" and state["phase"] == "TrainRide" and not metrics.get("rideObserved"):
            expected = {"train-u-bahn-muenchen", "wagon-grossraum-sitzplatz-optional", "livery-baustellengelb"}
            if not expected.issubset(state["equipped"]): raise AssertionError("Fahrzeugausstattung fehlt")
            capture("fahrt-mit-u-bahn")
            metrics["rideObserved"] = True
        return state["phase"] == "Results"
    wait(finished,name+" Ergebnis",150)
    time.sleep(4.2)
    page()
    capture(name+"-result")
    dismiss_post()

def scroll_panel(down):
    # All navigation after the first measurement uses the 540 x 1200 viewport.
    y1, y2 = (930, 400) if down else (415, 920)
    adb("shell", "input", "swipe", "270", str(y1), "270", str(y2), "490")
    time.sleep(.5)


def home():
    for _ in range(16):
        p=page()
        if ready("home-play",p): return
        for name in ["mail-close","result-home","map-back","nav-home","puzzle-back"]:
            if ready(name,p):
                tap(name,p if name=="mail-close" else None)
                time.sleep(1)
                break
        else:
            if p["phase"] == "Results": scroll_panel(True)
            else: time.sleep(1)
    raise RuntimeError("Bahnsteig nicht erreicht: " + str(list(targets(page()))))


def select_cosmetic(item):
    print("Betriebshof: " + item, flush=True)
    action = "cosmetic-action-" + item
    for _ in range(40):
        p = page()
        for _ in range(4):
            if any(key.startswith("cosmetic-action-") for key in targets(p)): break
            scroll_panel(True)
            p = page()
        if action in targets(p): break
        for _ in range(4):
            if ready("depot-next", p): break
            scroll_panel(False)
            p = page()
        tap("depot-next")
    tap(action)
    wait(lambda p:item in p["owned"],"Kauf "+item)
    if item not in last["equipped"]:
        tap(action)
        wait(lambda p:item in p["equipped"],"Ausrüsten "+item)


def main():
    cold("entry","home-play")
    tap("home-play")
    wait(lambda p:ready("puzzle-cell-0-0",p),"Erstes Rätsel")
    measure("puzzleActive",{"x":420,"y":240})
    time.sleep(2.5)
    measure("puzzleIdle")
    # Keep the 1080x2400 measurements; reduce the raster only for synthetic touch replay on the CPU renderer.
    adb("shell","wm","size","540x1200")
    adb("shell","wm","density","210")
    time.sleep(3)
    solve("auftrag")
    home()
    adb("shell","wm","size","1080x2400")
    adb("shell","wm","density","420")
    cold("platform","home-play")
    measure("platform")
    adb("shell","wm","size","540x1200")
    adb("shell","wm","density","210")
    storage=last["storage"]
    assert storage.endswith("/Android/data/"+PACKAGE+"/files")
    initial=adb("shell", "cat", storage+"/star-times-debug.jsonl")
    src=next(Path("seed").rglob("save.json"))
    relative=src.relative_to(Path("seed")/"DeviceSeed")
    destination=storage+"/"+relative.as_posix()
    adb("shell","am","force-stop",PACKAGE)
    adb("shell","pm","clear",PACKAGE)
    # Let Android recreate the package's external files directory after clearing its test profile.
    cold("fresh-profile","home-play")
    adb("shell","am","force-stop",PACKAGE)
    adb("shell","mkdir","-p",destination.rsplit("/",1)[0])
    adb("push",str(src),destination)
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
    wait(lambda p:p["mode"]=="REVISION" and p["phase"]=="Editing","Bewertete Betriebsrevision")
    solve("betriebsrevision")
    timings=adb("shell", "cat", storage+"/star-times-debug.jsonl")
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
    if last:
        capture("last-screen")
        if last.get("storage"): capture_unity_profile(last["storage"])
    else: (OUT / "startup.png").write_bytes(adb("exec-out","screencap","-p",raw=True))
