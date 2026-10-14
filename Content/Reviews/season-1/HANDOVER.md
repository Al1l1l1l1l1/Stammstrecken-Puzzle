# Historische Lieferung vor unabhängiger Nachbesserung

Dieser Bericht beschreibt den Stand bis `3c9b7d0…`. Seine schematische Eigenbewertung wurde durch die unabhängige Prüfung beanstandet und ist keine aktuelle Abnahme. Aktueller Stand: [REPAIR_HANDOVER.md](REPAIR_HANDOVER.md), Befundregister [REVIEW_RESOLUTION.md](REVIEW_RESOLUTION.md). Alter Teil-PASS wird nicht übertragen.

# WP-026: logischer Season-1-Authoringbestand zur externen Abnahme

Liefer-/Übergabestand 14.10.2026 (Europe/Berlin); Auftrag und Korrektur begannen
am 13.10.2026. Tatsächliche Prüfzeitpunkte sind in den Originalausgaben erhalten.

**Geliefert:** 240 konkrete Level-v2-Quellen, ihre vollständigen A→B-Lösungen,
240 echte `solver-v2`-/`proof-v1`-Artefakte, 240 konkrete Einstiegsschlüsse und
eigene fachliche Einzelbewertungen, 20 Routenberichte, vollständige Rootspuren
und tatsächliche technische Nachweise. **Offen:** unabhängige technische/
Contentabnahme, menschlicher Spieltest, fehlende Referenzziele und weitere
Pflichtgates. WP-026 ist nicht insgesamt abgeschlossen; keine Seasonfreigabe.

## Fortgesetzter Auftrag und Quellenkorrektur

Bestehende Branch `codex/wp-026-season1-logical-content`, bestehender Draft-PR
[21](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/21), unveränderte
Basis `c9516bdbd5858fd9003d5207ec37d180ccc6ae23`, gemeinsamer historischer Anker
`f9231c73e45ade33450c541e77dbeff5d2eae162`. Manifest-SHA256 weiterhin
`981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c`.
Der final eingefrorene HEAD und die tatsächliche CI-/Remotebindung werden am
Draft-PR geführt; eine folgende Prüfperson verifiziert genau diesen SHA und
darf keine Vorläufernachweise auf neue Artefakte übertragen.

Die [Auftragskorrektur](ORDER_CORRECTION.md) nimmt die falsche Nutzerzuschreibung
des pauschalen Koordinatorverbots zurück und benennt die maßgeblichen Quellen.
Die neue ausgefüllte Referenzprobe widerlegt den allgemeinen Startblocker.
Es gibt keine Schema-, Architektur-, Manifest-, Paketgrenzen- oder Regeländerung.
Der historische unabhängige Startbericht bleibt gekennzeichnete Historie und
wird nicht zur unabhängigen Abnahme dieses neuen Bestands erklärt.
Keine neue Parallelbearbeitung oder erneute Paketverankerung.

## Dateien und tatsächlicher Umfang

| Lieferteil | Persistente Quelle |
|---|---|
| 240 kanonische Levelquellen | `Content/Levels/s1/01–05/01–04/S1-*.level-v2.json` |
| 240 generierte Proofs | `Content/Proofs/S1-*/solver-v2.proof-v1.json` |
| Öffentlicher Input, konkrete Einstiege, tatsächliche Prämissenketten/Metriken | [level-inventory.json](level-inventory.json) |
| Vollständige Rootspuren, an Puzzle-/Proofhash gebunden | `root-trace-S1-<section>-<route>.json`, 20 Dateien |
| Eigene fachliche Einzel- und Routenbewertungen, Counts/A/B, sachliche Textgitter | `leveldesign-S1-<section>-<route>.md`, 20 Dateien |
| Ähnlichkeitsbefund einschließlich D4/A-B-Tausch | [similarity-report.json](similarity-report.json) |
| Generator/Strict/Metamorphose/Mutation/Metrik-/Hashbelege | [technical-evidence.json](technical-evidence.json), [echte Konsolenausgabe](authoring-validation.txt) |
| Draft-2020-12-/Node-Belege über tatsächliche neue Inputs | [schema-node-evidence.json](schema-node-evidence.json), [echte Ausgabe](schema-node-validation.txt) |
| Vorhandene unveränderte Regressionen, Original-NUnit3-XML als Base64 plus Bytehash | [regression-evidence.json](regression-evidence.json), [XML-Textansicht](regression-results.txt), [Konsolenausgabe](regression-run.txt) |
| Sechs offene technische Referenzziele je Puzzle | [unresolved-references.json](unresolved-references.json) |
| Reproduzierbare Aufrufe unveränderter APIs | `tools/season1-authoring/run_authoring.py`, `Season1Authoring.cs`, `check_schema_hashes.py`, `review_inventory.py` |

Alle 240 IDs des Bereichs `S1-01-01-01` bis `S1-05-04-12` sind genau einmal
vorhanden, in der vorgesehenen 5×4×12-Hierarchie. Die Routen 01–20 aus
Content-Bible 14 sind unverändert auf Abschnitt/lokale Route abgebildet.
Die Raster folgen den konkreten Routen: 4×4; 4×5; 5×5/6×5; 6×6/7×6;
7×7/8×8; 8×8/9×9/10×10. Keine Sonderfelder, Spielerannahmen, neuen Regeln
oder Dauerbaustellenlevel. Lösungen enthalten konkrete Formen und bleiben
Authoringdaten; es wurde kein Runtimekatalog erzeugt.

## Eigene fachliche Bewertung und behobene Auswahlfehler

Die Kandidatenhilfen konstruierten geometrische Pfade; ausschließlich die
unveränderte Domain, `ProofGenerator`, `CampaignGate` und Strict-Pipeline
entschieden technische Gültigkeit. Eine Zufallsauswahl ist keine automatische
Contentfreigabe. Der Levelautor wählte anhand der bestehenden fünf Gruppen pro
Route, Counts, Endpunkten, Dichte, Geometrie und tatsächlicher Root-Abhängigkeit.

Eigene Bewertung/Korrektur: Die erste große Kandidatenauswahl war überwiegend
zu dünn für die geplanten Dichtevariationen. Ergänzt wurden strukturierte
Kandidaten, wieder durch denselben Solver geprüft. In einer Zwischenwahl waren
zwei späte 6×6-Aufgaben weitgehend ein schmaler Zweispaltenkorridor; sie wurden
wegen fehlender räumlicher Rolle ersetzt. Zwei Paare mit fast gleichem
Count-Vektor/Motiv wurden ebenfalls ersetzt, statt neue IDs als Noveltybeleg
auszugeben. Betroffene Inputs/Proofs und alle zugehörigen Nachweise wurden
danach erneuert. Diese Auswahlpräferenzen sind keine neuen Produktgrenzwerte.

Endstand: keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder
A/B-Tausch. Der Bericht nennt pro Level den nächsten gleich orientierten
Count-Vektor und dessen L1-Abstand; auch dies ist keine automatische menschliche
Nichtredundanzfreigabe. Bekannte Regelmotive werden didaktisch wiederverwendet.

Eigene Beurteilung: Die 240 Aufgaben sind technisch fair und durch konkrete
öffentliche Einstiegsschlüsse zugänglich. Jede individuelle Notiz verweist auf
ihre Counts, konkrete Anschluss-/Restzahlfolgen und tatsächliche Prämissenkette.
Die ersten beiden Positionen jeder Route reaktivieren direkte Anker; spätere
Gruppen verändern Anordnung/Verzahnung; räumliche Variationen dürfen Erholung
bringen; zwei deduktive Abschlüsse bilden den Höhepunkt. Die späten Finale
verwenden auch geringe Dichte ohne Null-/Voll-Linien, über das ganze Raster
gekoppelte Counts und tatsächliche Root-Tiefen 42/40. Rastergröße oder Tiefe
allein wurden nicht als menschliche Schwierigkeitsfreigabe behandelt.

**Gezielte externe Qualitätsfragen:** Können Spieler die teilweise großen
Tiefensprünge an Routenabschlüssen nachvollziehen? Sind wiederkehrende
Kurven-/Streifenmotive trotz Counts-/Endpunktvariation ausreichend verschieden?
Bleiben große, dünnere Aufgaben gut lesbar? Diese Fragen bleiben für unabhängige
Contentprüfung und menschlichen Spieltest offen. `production.chainDepth` ist
hier der tatsächliche maximale Rootwert als vorläufiger Authoringbeleg; eine
menschlich erwartete Schwierigkeit wird damit nicht als kalibriert behauptet.

## Tatsächlich ausgeführte technische Nachweise

- Separater net10.0-Harness, .NET SDK 10.0.401, unveränderte reine Module in
  getrennten Assemblies; Build erfolgreich, 0 Warnungen/Fehler. Er ersetzt
  weder Unity-Compile noch `.asmdef`-/IL2CPP-/Gerätebelege.
- 240/240 UNIQUE, solutionCount 1, searchNodes 0, requiredGuessDepth 0.
  Counts und vollständige Formen-/Pfadsemantik werden vom Produktionsloader
  und der alleinigen Domainautorität geprüft.
- Drei frische Produktionsregenerationen je Quelle ergeben dieselben
  kanonischen gespeicherten Proofbytes. Strict-Einzelprüfung, vollständiger
  Strict-Command-Batch und umgekehrter Inputbatch erfolgreich.
- Alle 3.840 D4-/A-B-Varianten erneut durch Produktionsgenerator/CampaignGate
  qualifiziert. 1.440 Mutationen verworfen: Counts, Endpoints, Form/Pfad,
  Proofreferenzhash, neu selbstgehashte falsche Proofmetrik und falsche Artefakt-ID.
- Alle 240 vollständigen Rootspuren: Schritte/Tiefe aus den Prämissen neu
  berechnet, frühere Indizes, Deduplizierung und Sicherheit jeder Entfernung
  gegen die nachgewiesene Lösung geprüft. Keine Suchzweige als Rootspur.
- Gepinntes Linuxsetup CPython 3.11.13, Node 22.20.0, exakt sechs Pakete aus
  bestehendem requirements.lock.txt: 240 Levelschemas, 240 Proofschemas,
  1.200 tatsächliche unabhängige Node-JCS-Prüfungen (öffentlicher Input,
  Lösung, Proofprojektion, volle Proofbytes, vollständiger Dokumenthash),
  alle Bindungen und 240er-Inventar erfolgreich.
- 453/453 vorhandene Domain-/Solver-/Contenttests im getrennten Zusatzharness
  erfolgreich, 0 failed/skipped/warnings. Original-XMLbytehash und tatsächliche
  Ausgabe persistiert. Keine neue Abnahme von WP-025 und keine Goldenänderung.

Lokaler Laufzeitbeleg, keine Spielerzeiten: Gesamte technische Prüfung inklusive
3.840 Metamorphosen/1.440 Mutationen 7.379 s; Strict-Gesamtbatch 146.602 ms.
Sieben echte 10×10-Levels: p95/max 2.028 ms; Einzelmesswerte stehen im JSON.
Das sind Messungen auf dem lokalen Windows-/net10.0-Rechner, **kein Beleg auf
dem vorgeschriebenen CI-Referenzrechner**. Der endgültige Plattform-/CI-Beleg
bleibt ein externer Pflichtnachweis; vorhandene CI-Goldens ersetzen den neuen
240er-Corpuslauf nicht. Es wurden keine finalen Zeitwerte kalibriert.

Eigenen Helper-Compilefehler (`ComputeContentHash` liefert einen String, kein
Hashresultat) korrigiert und danach erfolgreich gebaut. Ein unpassender
`python`-Alias wurde durch die vorhandene CPython-3.11.13-Ausführung ersetzt.
Diese Vorläufer sind kein PASS; ausschließlich der finale erfolgreiche Lauf
ist oben und in den tatsächlichen Ausgaben belegt.

Der erste abschließende Governanceversuch bestand Architecture-only (17),
scheiterte im Scope aber an einer selbst erzeugten untracked Importcachedatei
außerhalb der Allowlist. Diese eigene Datei wurde entfernt und weitere
Bytecodeerzeugung in der Reviewhilfe unterbunden. Keine Manifest-/Gateänderung;
der endgültige saubere Scope-PASS wird am finalen Draft-PR nachgeführt.

## Echte offene Gates und Zuständigkeiten

| Gate | Konkreter Zustand / nächste zuständige Arbeit |
|---|---|
| Cross-References | **FAIL_MISSING_TARGET_SOURCES**: alle sechs Ziele × 240 IDs = 1.440 offene Einträge; Zielquelle/Owner pro JSON-Pointer im Inventar. Deutsche Lokalisation sowie Completion-/World-/Assetmanifest fehlen. Keine generische Fallback-ID, kein Fixtureziel, keine fiktive Auflösung. |
| Zeitklasse | `UNCALIBRATED` ist nur ein unbestimmter technischer Entwurfsmarker; keine bestätigte vorläufige Sollzeitklasse. Fachliche Zuordnung nach Leveltest offen. Finale Sekunden/Calibration explizit ausgeschlossen, `starThresholdsSeconds = null`. |
| Unabhängige technische Abnahme | Separater Prüfer auf eingefrorenem finalem Draft-PR-HEAD: reale Quellen/Proofs, Regeneration/Strict, Metriken/Rootspuren, Hashes/Schemata, vollständiger Scope und echte CIbindung. Kein neuer Parallelprüfer durch diesen Agenten. |
| Unabhängiger Contentreview / menschlicher Spieltest | Separate fachliche Beurteilung der 20 Lernbögen/Nichtredundanz; tatsächliche menschliche Lösungs-/Lesbarkeitstests. Eigene Berichte sind prüffertige Authorbewertung, kein Ersatz. |
| CI-Referenz-Performance | Neuen tatsächlichen 240er-Corpus mit angegebenen Befehlen auf dem vorgesehenen Runner prüfen; p95 <250 ms/max <2 s für 10×10, Gesamtkatalog <60 s gemäß Solververtrag. Lokale Messung gesondert, kein CI-PASS dafür. |
| Präsentation / Pseudolocale / kreative Miniaturen | Finaler fremder Content und kreative Ausarbeitung ausgeschlossen/fehlend; nur sachliche interne Textgitter geliefert. REQUIRED_LATER/NOT_EXECUTED. |
| Import-Dry-Run / Doppelimport / Addressables / Lizenzen / Größen | Verbindlich erhalten. Kein Runtimeimport durch Nutzerauftrag; entsprechende Produktionsartefakte fehlen. REQUIRED_LATER/NOT_EXECUTED, keine implizite Nichtanwendbarkeitsfreigabe. |
| Produktstatus / Release-Lock / Gerät / Store | Keine Freigabe erteilt. Beide Runtime-/Releaseeligibility-Werte bleiben false. Keine Integration, Release oder final ausgearbeitete Season. |

Der fehlende Zielcontent wird nicht als allgemeines Authoringverbot behandelt.
Seine spätere Erstellung/Zuordnung und Freigabe ist eine echte Übergabe an die
jeweils zuständigen Contentverantwortlichen, keine Einladung an diesen Agenten,
ausgeschlossene Kreativinhalte zu erzeugen oder Gates abzuschwächen.

## Reproduktion und Abnahmeübergabe

Vom Repositoryroot, ohne Import- oder Schreibnebenwirkung auf Produktionsmodule:

```text
python3.11 tools/season1-authoring/run_authoring.py verify
python3.11 tools/season1-authoring/review_inventory.py
# im bestehenden gepinnten Linux-/Node-/requirements.lock-Setup:
python tools/season1-authoring/check_schema_hashes.py
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-026.production.scope.json --self-test
```

`verify` schreibt ausschließlich neue lokale Mess-/Reviewevidenz; die Quellen
und Proofs bleiben unverändert. Prüfer vergleichen vor/nach Quelle/Proofhashes
und behandeln maschinenabhängige Messberichte als ihre eigene neue Evidenz.
`review_inventory.py` ohne `--annotate` erzeugt Berichte aus den tatsächlichen
Quellen/Spuren. Authoring-Pools und Zwischenauswahlen in `.utmp` sind weder
kanonische Lieferquellen noch Voraussetzung zur Proofregeneration.

Nach eigener technischer Prüfung wird derselbe finale HEAD im vorhandenen
Draft-PR eingefroren. Keine neuen parallelen Agenten beauftragt. Nächster
fachlicher Schritt ist externe unabhängige Abnahme und Spieltest dieses
prüffertigen Bestands; Referenz-/Zeitklassen-/Folgegates bleiben konkret offen.
Kein bloßer Fortsetzungsauftrag nach internen Teilschritten erforderlich.
