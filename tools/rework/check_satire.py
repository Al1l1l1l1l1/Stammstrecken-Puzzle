#!/usr/bin/env python3
"""Validate the authored station programme against its runtime contract."""
from collections import Counter
import json
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[2]
UI = ROOT / "Assets/StammstreckenPuzzle/UI"
SATIRE = UI / "Satire"
EVENTS = set("""FIRST_START AFTER_PUZZLE_3 FIRST_DEPOT FIRST_PURCHASE FIRST_WAGON
WORLD_ENTER_1 WORLD_ENTER_2 WORLD_ENTER_3 WORLD_ENTER_4 WORLD_ENTER_5
FIRST_THREE_STARS FIRST_RESET CLEARS_10 CLEARS_50 CLEARS_100 SEASON_END_PRESS
SEASON_END_LEAD ENDLESS_UNLOCKED REVISION_UNLOCKED""".split())
SENDERS = {"ABTEILUNGSLEITUNG", "BETRIEBSHOF", "PRESSESTELLE", "KONZERNLEITUNG"}
CUES = {"Gong", "Flap", "Stamp", "Applause", "Sigh", "Pigeons", "DoorBeep", "Brake", "Crackle"}
WORLDS = {f"WORLD_{n}" for n in range(1, 6)}
REQUIRED = {"deadline", "elevator", "clock", "order", "sev", "birds", "through", "delay", "crackle", "reverse"}
INFORMAL = re.compile(r"\b(du|dich|dir|dein(?:e[rmns]?)?)\b", re.I)
EMOJI = re.compile("[\U0001F000-\U0001FAFF\u2600-\u27BF\uFE0F\u200D]")

def require(ok, message):
    if not ok:
        raise ValueError(message)

def distinct(values, label):
    values = list(values)
    require(len(set(values)) == len(values), f"duplicate {label}")

def prose(value, label, limit=None):
    require(isinstance(value, str) and value.strip(), f"{label}: empty")
    unquoted = re.sub(r'„[^“]*“|»[^«]*«|"[^"]*"', "", value)
    require(not INFORMAL.search(value) and not EMOJI.search(value) and "!" not in unquoted,
            f"{label}: check address, emoji or punctuation")
    require(limit is None or len(value) <= limit, f"{label}: too long")

def dataset(name):
    raw = (SATIRE / (name + ".json")).read_bytes()
    require(not raw.startswith(b"\xef\xbb\xbf") and b"\r" not in raw, f"{name}: expected UTF-8 with LF")
    data = json.loads(raw.decode("utf-8"))
    require(data["version"] == 1, name)
    return data

def check_mails():
    mails = dataset("mails")["mails"]
    distinct((m["id"] for m in mails), "mail ID")
    counts = Counter(m["trigger"] for m in mails)
    require(set(counts) == EVENTS | {"RANDOM_CIRCULAR"}, "unknown or missing mail trigger")
    require(all(counts[event] == 1 for event in EVENTS), "one letter per milestone")
    # Nineteen unique milestones leave at least 21 circulars for a 40-letter programme.
    require(len(mails) >= 40 and counts["RANDOM_CIRCULAR"] >= 10, "mail programme incomplete")
    for m in mails:
        key = m["id"]
        require(m["from"] in SENDERS and 1 <= len(m["body"]) <= 4, key)
        prose(m["subject"], key, 58)
        for sentence in m["body"]:
            prose(sentence, key)
        if m["from"] == "ABTEILUNGSLEITUNG" or m["ps"] is not None:
            prose(m["ps"], key)
        require(m["gift"] is None or (m["trigger"] == "AFTER_PUZZLE_3"
                and m["gift"] == "livery-baustellengelb"), key)
    return len(mails)

def check_gags():
    gags = dataset("world-gags")["gags"]
    distinct((g["id"] for g in gags), "gag ID")
    counts = Counter(g["scene"] for g in gags)
    require(all(counts[w] >= 2 for w in WORLDS) and counts["MAP"] and counts["DEPOT"], "missing scenery")
    required = {g["id"] for g in gags if g["required"]}
    require({"gag-" + g for g in REQUIRED} <= required, "missing required scene gag")
    for g in gags:
        key = g["id"]
        require(g["scene"] in WORLDS | {"PLATFORM", "RIDE", "DEPOT", "MAP", "ALL"}, key)
        require(g["kind"] in {"LOOP", "RARE"} and type(g["required"]) is bool, key)
        require(g["objects"] and all(re.fullmatch(r"[a-z0-9-]+", o) for o in g["objects"]), key)
        require(g["sound"] is None or g["sound"] in CUES, key)
        if g["line"] is not None:
            prose(g["line"], key, 110)
        prose(g["direction"], key)
    return len(gags)

def check_voices():
    v = dataset("voices")
    items = json.loads((ROOT / "Content/Catalogs/s1/cosmetics-v2.json").read_bytes())["items"]
    require(set(v["depotLines"]) == {i["id"] for i in items} and len(items) == 29, "catalogue dialogue")
    for key, line in v["depotLines"].items():
        prose(line, key, 110)
    ranks = [r["minStars"] for r in v["ranks"]]
    require(len(ranks) >= 8 and ranks == sorted(set(ranks)) and ranks[0] == 0 and ranks[-1] <= 720, "ranks")
    for r in v["ranks"]:
        prose(r["title"], "rank", 58)
    jubilees = v["jubilees"]
    require(len(jubilees) >= 15, "jubilees")
    distinct((j["id"] for j in jubilees), "jubilee")
    for j in jubilees:
        require(re.fullmatch(r"(CLEARS|STARS|THREE_STARS|RESETS|DEPOT_ITEMS|WAGONS|ENDLESS|REVISION)_[1-9][0-9]*|SEASON_COMPLETE", j["condition"]), j["id"])
        prose(j["text"], j["id"], 130)
    for key, count in (("loading", 20), ("arrivalAnnouncements", 10), ("boardMessagesRare", 10)):
        require(len(v[key]) >= count, key)
        distinct(v[key], key)
        for line in v[key]:
            prose(line, key, 110)
    require([r["step"] for r in v["tutorial"]] == list(range(1, 6)), "tutorial")
    for r in v["tutorial"]:
        prose(r["text"], "tutorial", 90)
    sign = v["deadlineSign"]
    require(re.findall(r"{[^{}]*}", sign["line"]) == ["{year}"], "deadline year")
    for key in ("title", "line", "note", "endless"):
        prose(sign[key], key, 110)
    proof = v["proofOfPunctuality"]
    prose(proof["statement"], "proof", 130)
    require(3 <= len(proof["remarks"]) <= 5, "remarks")
    for remark in proof["remarks"]:
        prose(remark, "remark", 130)
    for key in ("greyedNext", "endless", "revision"):
        prose(v["seasonEnd"][key], key, 90)

def main():
    letters = check_mails()
    gags = check_gags()
    check_voices()
    print(f"Satireprogramm geprüft: {letters} Mails, {gags} Szenen, 29 Betriebshoftexte.")

if __name__ == "__main__":
    try:
        main()
    except (ValueError, KeyError, TypeError, OSError) as error:
        sys.exit(str(error))
