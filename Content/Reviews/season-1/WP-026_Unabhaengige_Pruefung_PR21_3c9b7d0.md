# WP-026: Unabhängige technische und fachliche Prüfung des logischen Season-1-Rätselbestands

| Feld | Wert |
|---|---|
| Repository / PR | Al1l1l1l1l1/Stammstrecken-Puzzle, PR #21 (Draft), Branch `codex/wp-026-season1-logical-content` |
| **Geprüfter HEAD** | `3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e` |
| **Basis** | `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` (zugleich Spitze von `main`, drei Commits Abstand) |
| Gemeinsamer Anker / Manifest | `f9231c73e45ade33450c541e77dbeff5d2eae162` / SHA-256 `981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c` |
| Prüfdatum | 14.10.2026 (Europe/Berlin) |
| Rolle | Unabhängiger Prüfer (KI). Nur prüfend: keine Repositoryänderung, keine Reparatur, kein Commit, Push oder Merge. Keine Änderung an Planung, Anforderungen oder Projektregeln. Keine kreative Ausarbeitung, keine Zeitkalibrierung, kein simulierter Spieltest. |
| Prüfstand | Ein einziger Stand: HEAD `3c9b7d0…` gegen Basis `c9516bd…`. HEAD und Basis wurden zu Beginn und unmittelbar vor Abgabe geprüft (Abschnitt 2). Es wurde keine zweite unabhängige Prüfung parallel gestartet; am PR gab es zum Prüfzeitpunkt keine Reviews oder Kommentare Dritter. |

---

## 1. Urteil

| Gegenstand | Urteil |
|---|---|
| **A. Korrektheit des gelieferten Rätselbestands** (Lösbarkeit, Eindeutigkeit, Pfade, Counts, Proofs, Hashes, Determinismus, Rootspuren) | **PASS**, ohne Befund. Alle 240 Level und 240 Proofs wurden mit eigenem, vom Produktsolver unabhängigem Code und zusätzlich durch Reproduktion mit dem Produktcode bestätigt. |
| **B. Qualität und Nachweislage des Authoringbestands** (AK-02, AK-06, Leveldesign) | **FAIL, nachbesserbar.** AK-06 ist nicht erfüllt (V-01, V-02, V-03). AK-02 ist mit Befunden erfüllt. Messbare strukturelle Wiederholung (Q-01) ist ein fachlicher Befund, den die Contentabnahme bewerten muss. |
| **Urteil zum geprüften Lieferumfang** | **FAIL (nachbesserbar).** Kein Befund betrifft Lösbarkeit, Eindeutigkeit, Proofs oder Hashes. Die Befunde betreffen den Nachweis von Variation und Nichtredundanz sowie die Entstehung der Qualitätsnotizen, Einstiegstexte und Routenbewertungen. Die Proofs und Level-Dateien müssen für die Behebung voraussichtlich nicht neu erzeugt werden, es sei denn, einzelne Level werden ersetzt. |
| **Gesamt-DoD WP-026** | **NICHT ERFÜLLT.** Gründe: AK-06 nicht erfüllt; AK-08 offen; CI-Referenz-Leistungsnachweis (WP-Test 5) nicht erbracht; Cross-Reference-Gate FAIL (1.440 Ziele fehlen); Zeitklasse unbestimmt; Gates aus WP-Test 7 REQUIRED_LATER; unabhängige Contentabnahme und menschlicher Spieltest ausstehend. |
| **Integrationsbereitschaft** | **NICHT GEGEBEN.** Fehlende Voraussetzungen werden durch das Teil-PASS in A nicht aufgehoben. Integration ist laut WP-026 ausdrücklich nicht beauftragt. |
| **Spätere kreative, menschliche und Produktionsfreigaben** | Nicht Gegenstand dieser Prüfung, nicht erteilt, nicht ersetzt. `runtimeImportable` und `releaseLockEligible` bleiben `false`. |

Das Urteil B stützt sich auf überprüfbare Vertragsbezüge (WP-026 AK-06 und Test 6, CONTENT_PIPELINE §3.2), nicht auf Geschmack. Die Befunde sind in Abschnitt 5 einzeln belegt.

### 1.1 Akzeptanzkriterien

| AK | Bewertung | Kernbegründung |
|---|---|---|
| AK-01 | **ERFÜLLT** | Anker `f9231c7` hat als einzigen Elter die Basis `c9516bd`, die weiterhin Spitze von `main` ist. Manifest-SHA-256 stimmt und ist seit dem Anker unverändert. Validator-Scope-Lauf lokal PASS (`LOCAL_SCOPE PASS`, `worktreeDirty=false`), `git diff --check` sauber. `validate.yml` ändert genau eine Zeile (`STP_SCOPE_MANIFEST` auf WP-026). Architecture-Validation-CI auf dem Head: Success (Push- und PR-Lauf). Übergabe über WP, HANDOVER, CURRENT_STATE und WORK_QUEUE nachvollziehbar. Evidenzgrenze: Logs nicht abrufbar (E-02). |
| AK-02 | **ERFÜLLT MIT BEFUNDEN** | 240 eindeutige IDs, 5×4×12 vollständig, Pfad und Hierarchie stimmen. Raster aller 20 Routen entsprechen Content-Bible 14 (Anhang A). Keine exakten Kopien: kein Paar identisch unter D4 und A/B-Tausch (eigene Prüfung). Der Lernbogen-Nachweis selbst ist nur über die generischen Routenberichte geführt (V-02); drei Wortlautabweichungen bei messbaren Klauseln (Q-02). |
| AK-03 | **ERFÜLLT** | Schema (Draft 2020-12), Pfad A→B, sechs Formen, Port-Konsistenz, Counts neu abgeleitet: 240/240, 0 Befunde, mit Negativkontrolle. |
| AK-04 | **ERFÜLLT** | 240 echte `solver-v2`-Proofs, UNIQUE/1, `searchNodes` 0, `requiredGuessDepth` 0. Alle Hashes und die Referenzbindung stimmen. Rootspuren (23.108 Schritte) und Metrikrekonstruktion unabhängig bestätigt. |
| AK-05 | **ERFÜLLT** | Regeneration auf frischer Linux-Umgebung mit .NET SDK 10.0.401: alle 240 Proofs bytegleich. Strict einzeln, Batch und umgekehrter Batch bestanden. 3.840 Metamorphosen, 1.440 verworfene Mutationen. 1.200 Node-JCS-Abgleiche eigenständig wiederholt, 0 Abweichungen. |
| AK-06 | **NICHT ERFÜLLT** | Ähnlichkeitsbericht erfasst symmetrische Varianten bei der Nähe nicht (V-01). Die 20 Routenbewertungen enthalten keine routenspezifische Beurteilung (V-02). Qualitätsnotizen, Einstiegstexte und Fokus sind skriptgeneriert (V-03). Variation und Nichtredundanz sind damit nicht belegt, und die Messung zeigt strukturelle Wiederholung (Q-01). Spieltest und Contentabnahme sind korrekt als offen ausgewiesen. |
| AK-07 | **ERFÜLLT** | 1.440 offene Referenzen mit Level-ID, JSON-Pointer, Owner und Quelle inventarisiert und mit den Level-Dateien deckungsgleich (0 Abweichungen). `fallbackUsed=false` bei allen. Gates mit Zuständigkeit im HANDOVER. Keine falsche Freigabe, `runtimeImportable` und `releaseLockEligible` bleiben `false`. |
| AK-08 | **OFFEN** | Gelieferter Teil erfüllt: eigene Nachweise sind persistiert und passen zum Head (meine Reproduktion ergab bytegleiche Traces und identische Nicht-Zeitfelder). Die unabhängige Abschlussbewertung ist dieser Bericht (Urteil FAIL). Nicht erbracht: CI-Referenz-Leistungsnachweis und Persistierung dieses Berichts. Der Gesamtabschluss wird vom Autor korrekt nicht behauptet. |

---

## 2. Geprüfte Stände

| Zeitpunkt | HEAD | Basis (`main`) | PR |
|---|---|---|---|
| Prüfbeginn | `3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e` | `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` | #21, Draft, drei Commits (`f9231c7`, `43f737e`, `3c9b7d0`) |
| Unmittelbar vor Abgabe (14.10.2026, 01:00 Uhr) | `3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e` | `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` | #21, Draft, unverändert (Details in Abschnitt 9) |

Der Diff gegen die Basis umfasst 544 hinzugefügte und 3 geänderte Dateien (`validate.yml`, `CURRENT_STATE.md`, `WORK_QUEUE.md`). Unter `Assets/` und `ARCHITECTURE/` gibt es keine Änderung. Alle Dateien liegen innerhalb der Allowlist des Manifests (Validator bestätigt).

Arbeitsumgebung der Prüfung: Linux, isolierte Kopie (`git archive` des Heads) plus unveränderter Klon. Der Klon blieb unverändert (`git status` leer, auch ignorierte Dateien). Werkzeuge: CPython 3.11.17 mit den sechs gepinnten Paketen aus `requirements.lock.txt`, Node v22.22.0, .NET SDK 10.0.401 (gleiche Version wie beim Autor), NUnitLite 3.14.0, g++. CI nutzt 3.11.13 und 22.20.0. Die Patch-Abweichung ist vom WP gedeckt (CPython 3.11.x, Node 22.x).

---

## 3. Prüfmaßstab (aus dem Repository abgeleitet)

Gelesen wurden AGENTS.md in der vorgegebenen Reihenfolge, Konzept 15, 00, 09, 14, 04, 03, PROJECT_CONTROL (CURRENT_STATE, WORK_QUEUE, DEFINITION_OF_DONE, AI_HANDOVER_RULES, WORK_PACKAGE_RULES), ADR-007, 021, 030, 031, SOLVER_ARCHITECTURE, CONTENT_PIPELINE, LEVEL_DATA_FORMAT, CONTENT_CATALOGS, TEST_STRATEGY, Validator-README, WP-026 vollständig mit Auftragskorrektur (`ORDER_CORRECTION.md`) sowie `HANDOVER.md`. Berichte des Autors wurden als zu prüfende Aussagen behandelt. Zurückgenommene Koordinatorzusätze (pauschales Verbot technischer Entwurfsreferenzen) wurden nicht angewandt; `draft-…`-Referenzen wurden nicht beanstandet. Zusätzliche Anforderungen wurden nicht erfunden. Wo das Repo keinen Schwellwert enthält (Nichtredundanz), ist der Befund als fachlich gekennzeichnet.

Maßgebliche Quellen je Prüfpunkt:

| Prüfpunkt | Quelle |
|---|---|
| AK-01 bis AK-08, Tests 1 bis 7 | WP-026 |
| Rätselregeln (ein Pfad A→B, sechs Formen, keine Kreuzung, keine Schleife) | Konzept 03 §1 und §3, LEVEL_DATA_FORMAT §2 |
| 20 Lernbögen, Raster, Gruppenrollen, Routenrollen, Nichtredundanz | Content-Bible 14 §1 bis §9 (§2: „ohne die einzelnen Rätsel redundant zu machen“) |
| Hashprofile, Proofformat | LEVEL_DATA_FORMAT §6 und §7, `proof-v1.schema.json` |
| Solverbudgets | SOLVER_ARCHITECTURE §10 |
| Qualitätsnotiz, Einstiegserkenntnis | CONTENT_PIPELINE §3.2 und §3.3 |

---

## 4. Tatsächlich ausgeführte Prüfungen

Alle Skripte der Prüfung liegen als Anhang-Archiv bei (`wp026-review-verifiers.tgz`). Sie liegen außerhalb des Repositorys.

| Nr. | Prüfung | Methode und Unabhängigkeit | Ergebnis |
|---|---|---|---|
| P-01 | HEAD, Basis, Anker, Manifest | `git rev-parse`, `merge-base`, Elter des Ankers, `sha256sum` Manifest, Manifestdiff seit Anker | PASS. Basis = `origin/main`, Anker-Elter = Basis, Manifest-SHA stimmt, Manifest unverändert |
| P-02 | Validator Architecture-only | `validate.py --self-test`, venv mit gepinnten Paketen | PASS (17 lokale Prüfgruppen) |
| P-03 | Validator WP-026-Scope | `validate.py --scope production --scope-manifest …WP-026.production.scope.json --self-test` | PASS (18 Gruppen, `LOCAL_SCOPE PASS`, `worktreeDirty=false`) |
| P-04 | Whitespace | `git diff --check c9516bd… --` | PASS (Exitcode 0) |
| P-05 | CI-Schritt „JCS-Goldenvektoren gegen Node“ | Inline-Schritt aus `validate.yml` lokal ausgeführt | PASS (12 Fälle, 32 Rejects, 2+2 Dokumentvektoren) |
| P-06 | Struktur, Schema, Pfad, Counts, Hashes | Eigener Python-Verifier, ohne Produktcode: Draft-2020-12 für 240 Level und 240 Proofs, ID/Pfad/Hierarchie, Pfad A→B mit Portabgleich, Counts neu abgeleitet, Public-, Solution- und Proofhash, kanonische Proofbytes, `proofRef` | PASS, 0 Befunde. Negativkontrolle mit 3 manipulierten Leveln: 8 Befunde erkannt |
| P-07 | Eindeutigkeit | Eigener C++-Zähler (DFS über einfache A→B-Pfade mit Zähl-Pruning), unabhängig vom Produktsolver | 240/240 genau eine Lösung (Limit 3), Lösung identisch mit dem gelieferten Pfad. Differentialtest gegen Brute-Force an 400 Zufallsrätseln (299 eindeutig, 101 mehrdeutig): 0 Abweichungen |
| P-08 | Requiredguessdepth 0 | Eigener Constraint-Propagator (Ports, Nachbarkonsistenz, Zeilen- und Spaltenzahlen, Schleifenverbot, Erreichbarkeit) | 240/240 ohne Suche zur gelieferten Lösung. Auch mit ausschließlich lokalen Regeln (Ports, Nachbarn, Linienzahlen) 240/240. Kontrolle an 300 Zufallsrätseln: 0 unzulässige Entfernungen, 14 eindeutige Rätsel ohne Suche nicht lösbar (Propagator ist also nicht trivial stark), 0 mehrdeutige fälschlich „gelöst“ |
| P-09 | Metamorphosen | 8 D4-Transformationen × A/B-Tausch × 240 Level = 3.840 Varianten, eigene Transformation | alle 3.840 genau eine Lösung (C++) und ohne Suche lösbar (Propagator) |
| P-10 | Rootspuren | Eigener Audit der 20 `root-trace-*.json`: 240 Spuren, 23.108 Schritte. Indexfolge, Prämissen nur früher, Tiefe neu berechnet, Entfernungen sicher gegen die Lösung, Domänenfolge konsistent, jede Zelle am Ende auf die Lösung reduziert, Metriken gleich Proof, Bindung an Puzzle- und Proofhash | 0 Befunde |
| P-11 | Node-JCS | `jcs_crosscheck.mjs` mit Node v22.22.0: öffentlicher Input, Lösung, Proofprojektion, Proofbytes, vollständiges Dokument je Level = 1.200 Abgleiche; zusätzlich `technical-evidence.json` gegen Dateien (Content-Hash, Public-, Solution-, Proofhash, Metriken) | 0 Abweichungen, 240/240 konsistent |
| P-12 | Reproduktion der Autor-Verifikation | `python tools/season1-authoring/run_authoring.py verify` aus sauberem `git archive` des Heads, .NET SDK 10.0.401, Produktassemblies unverändert | PASS. 3× Regeneration je Quelle bytegleich, Strict einzeln, Batch, umgekehrter Batch, 3.840 Metamorphosen, 1.440 verworfene Mutationen. Alle 20 `root-trace`-Dateien bytegleich zur Lieferung. `technical-evidence.json` außer Zeitfeldern identisch |
| P-13 | Bestandsregression | NUnitLite 3.14.0, net10.0, Linux; Domain-, Solver- und Content-EditMode-Tests des Repos | 453/453 bestanden (34,5 s) |
| P-14 | Lernbogen-Analyse | Raster je Route, messbare Themenklauseln, A/B-Relationen, Null- und Vollinien, Dichte, Ecken, Tiefen | Abschnitt 5 (Q-02), Anhang A und B |
| P-15 | Ähnlichkeit und Redundanz | Exakte Duplikate unter D4 und A/B-Tausch; Belegungsüberlappung (Jaccard) unter D4; Count-Vektor-L1 unter D4; Schlangenmotiv-Detektor | exakte Duplikate 0; Befunde V-01 und Q-01 |
| P-16 | Textanalyse | Masken- und Vergleichsanalyse von `qualityNote`, `focus`, Einstiegstexten, Routenberichten; Lektüre von `select_candidates.py` und `review_inventory.py` | Befunde V-02, V-03 |
| P-17 | CI | Öffentliche GitHub-Seiten der Läufe und des PR (Abschnitt 7) | Architecture Success; Unity PR Success; Unity Push Cancelled (Concurrency) |
| P-18 | Schlussverifikation HEAD und Basis | `git fetch` von `main` und `refs/pull/21/head`, Abgleich | siehe Abschnitt 9 |

### 4.1 Nicht ausgeführt, mit Ursache

| Prüfung | Ursache | Einordnung |
|---|---|---|
| Unity-Compile, EditMode, PlayMode, IL2CPP, Gerät lokal | In der Prüfumgebung steht kein Unity zur Verfügung. Die Unity-CI läuft, führt die neuen Inhalte aber nicht aus (keine `Assets/`-Änderung) | REQUIRED_LATER laut WP-Test 7 |
| CI-Logs und Artefakt-Downloads | GitHub verlangt Anmeldung. Die API verweigert für dieses Repo die Sitzungsrechte. Der einzige angebotene Weg wäre ein Push-fähiger Zugriff gewesen. Den habe ich bewusst nicht angefordert, weil der Auftrag „keine Pushes“ lautet | Evidenzgrenze E-02. Nachprüfung mit lesendem Token möglich |
| CI-Referenz-Leistungsnachweis (WP-Test 5) | Kein Zugriff auf den Referenzrechner, keine CI-Stufe für den 240er-Bestand | offen, E-01 |
| Cross-Reference, Pseudolocale, Doppelimport, Addressables, Lizenz, Größe | Zielinhalte fehlen, Runtimeimport ausgeschlossen | REQUIRED_LATER/NOT_EXECUTED, vom Autor korrekt ausgewiesen |
| `check_schema_hashes.py` des Autors | Das Skript prüft hart auf Node v22.20.0 und CPython 3.11.13 | ersetzt durch P-06 und P-11 (Anmerkung O-02) |
| Menschlicher Spieltest, Zeitkalibrierung | Nicht beauftragt, nicht simuliert | offen |

---

## 5. Befunde

Einstufung: **V** = Vertragsverletzung (Verstoß gegen AK, WP-Test oder verbindliche Regel), **Q** = fachlicher Qualitätsbefund (kein Repo-Schwellwert, Bewertung der Contentabnahme), **E** = fehlender oder nicht ausführbarer Nachweis, **O** = optionaler Verbesserungsvorschlag.

### 5.1 Vertragsverletzungen

#### V-01 Ähnlichkeitsbericht erfasst symmetrische Varianten bei der Nähe nicht (mittel)

- **Quelle:** WP-026 Test 6 („Ähnlichkeitsbericht einschließlich symmetrischer Varianten“), AK-06, CONTENT_PIPELINE §3.2 Punkt 9.
- **Betroffen:** `Content/Reviews/season-1/similarity-report.json` (Feld `nearestMetric`: „L1 counts, same grid orientation only“), Erzeugung in `tools/season1-authoring/review_inventory.py`. Betroffene Rätsel: S1-01-03-03 und S1-01-04-08 (L1 = 0 unter Spiegelung); je L1 = 2 bei S1-01-01-02 / S1-01-01-04, S1-01-02-07 / S1-01-04-03, S1-02-01-03 / S1-02-02-02, S1-02-02-01 / S1-02-03-01, S1-03-02-01 / S1-03-02-02, S1-04-03-07 / S1-05-02-02.
- **Reproduktion:** Für alle Paare gleicher Rastergröße den minimalen L1-Abstand der Count-Vektoren über alle rastererhaltenden D4-Transformationen bilden (Skript im Archiv, Snippet im Abschnitt 10). Ergebnis: 7 Paare mit L1 ≤ 2, davon 1 mit L1 = 0. Der Bericht kennt dagegen nur gleich orientierte Vektoren: kleinster Abstand 4, alle 240 Einträge lauten `DIFFERENT_COUNT_VECTOR`. Der Autor wendet in `select_candidates.py` selbst die Schwelle „L1 > 2“ an und beschreibt in HANDOVER, dass „zwei Paare mit fast gleichem Count-Vektor/Motiv“ ersetzt wurden. Die symmetrische Blindstelle lässt sieben weitere unter derselben Schwelle stehen.
- **Einordnung:** S1-01-03-03 und S1-01-04-08 sind keine Duplikate, weil A und B verschieden liegen (N0/W3 gegen N2/E4). Beide sind aber 4×5-Zweispaltenkorridore mit identischem gespiegeltem Count-Muster (Zeilen alle 2, Spalten 5/5/0/0 bzw. 0/0/5/5).
- **Auswirkung:** Die Aussage „Nichtredundanz“ ist im Bericht nicht belegt. Keine Auswirkung auf Eindeutigkeit oder Proofs.

#### V-02 Die 20 Routenbewertungen enthalten keine routenspezifische Beurteilung (mittel)

- **Quelle:** AK-06 („20 Routenbewertungen belegen Dramaturgie, Fairness, Variation und Nichtredundanz“), AK-02 („Lernbögen … pro Route nachgewiesen“), Content-Bible 14 §4 bis §8 (je Route eigener Bogen).
- **Betroffen:** `leveldesign-S1-01-01.md` bis `leveldesign-S1-05-04.md`.
- **Reproduktion:** Der Fließtext vor den Level-Abschnitten besteht in allen 20 Dateien aus denselben vier Absätzen, wortgleich. Routenindividuell sind nur die Tabellenzeilen (Gruppenlabel aus der Konstante `BRIEFS` plus Root-Tiefen). Die Zeile „Grenze:“ ist in allen 240 Level-Abschnitten wortgleich. Die Dateien entstehen vollständig aus `review_inventory.py`.
- **Auswirkung:** Es gibt keinen routenbezogenen Nachweis, dass Dramaturgie, Variation oder Nichtredundanz einer Route geprüft wurden. Die Dateien dokumentieren nur Metriken.

#### V-03 Qualitätsnotizen, Einstiegserkenntnisse und Fokus sind werkzeuggeneriert (mittel; Auslegung durch den Auftraggeber zu bestätigen)

- **Quelle:** CONTENT_PIPELINE §3.2 („Tools … dürfen keine Qualitätsnotiz, Einstiegserkenntnis, Zeitwerte oder Produkttexte erfinden“), AK-06 („Individuelle Einstiegsschlüsse, sachliche Qualitätsnotizen“), Content-Bible 14 §9 (Schwerpunkt je Meldung; Qualitätsnotiz: „Warum das Level fair, lesbar und nicht redundant ist“).
- **Betroffen:** `production.qualityNote` und `production.focus` in allen 240 Level-Dateien, Einstiegstexte in `level-inventory.json`.
- **Reproduktion:** `review_inventory.py --annotate` setzt die Notiz per f-String zusammen. Nach Maskierung von Zahlen und IDs bleiben 168 Varianten, nach zusätzlicher Maskierung des Gruppenbriefs der Route nur 6 Vorlagen. Evaluative Sätze („Lesbarer öffentlicher Einstieg“, „Fairness technisch belegt“) stehen in jeder Notiz. Der Einstieg ist in 127 Fällen die erste Nulllinie, in 77 Fällen die erste Vollinie und in 36 Fällen ein fester Endpunktsatz. `focus` hat nur vier Ausprägungen (102 / 98 / 30 / 10 Level) und ergibt sich aus „Nulllinie vorhanden“ und „Position > 2“ (`select_candidates.py`). `DENSITY` und `COMBINATION` kommen nie vor, obwohl das Schema sie erlaubt und der Lernbogen Dichte und Kombination als Schwerpunkt benennt.
- **Einordnung:** Die Zahlen stammen aus Lösung und Solver, nichts davon ist falsch. Die Bewertungsaussagen sind aber nicht aus einem Urteil über das einzelne Level entstanden. Nach dem Wortlaut von §3.2 ist das ein Verstoß. Die Auslegung („erfinden“ gegen „ableiten“) liegt beim Auftraggeber. Der Autor kennzeichnet seinen Bericht selbst als Authorbewertung ohne Spieltest, damit ist die Aussage ehrlich, aber im Informationsgehalt dünn.
- **Auswirkung:** Die Notiz taugt nicht als Reviewbeleg im Sinn von CONTENT_PIPELINE §3.3. Sie wiederholt Metriken.

### 5.2 Fachliche Qualitätsbefunde

#### Q-01 Messbare strukturelle Wiederholung und Schlangenmotiv

- **Quelle:** Content-Bible 14 §2 („ohne die einzelnen Rätsel redundant zu machen“), AK-06. Ein numerischer Schwellwert existiert im Repo nicht (Novelty-Profile hängen an BLOCKER-PROD-003). Die Bewertung gehört daher dem unabhängigen Contentreview.
- **Messung (Belegungsüberlappung der Gleiszellen, beste D4-Transformation, 3.183 vergleichbare Paare):** 4 Paare ≥ 0,90, 20 Paare ≥ 0,85, 48 Paare ≥ 0,80. Paare benachbarter Positionen derselben Route: S1-03-02-01 / -02 (0,947, transponierte Schlangenmuster), S1-04-03-07 / -08 (0,892), S1-05-02-09 / -10 (0,889), S1-05-01-02 / -03 (0,879, beide Endpunkte gleich), S1-02-03-09 / -10 (0,875). Routenübergreifend z. B. S1-02-02-01 / S1-02-03-01 (0,933), S1-05-02-07 / S1-05-04-02 (0,930), S1-05-01-02 / S1-05-02-02 (0,886, beide Endpunkte gleich), S1-04-02-02 / S1-04-03-02 (0,867, beide Endpunkte gleich). Liste: Anhang C.
- **Schlangenmotiv:** 22 Level (35 bei einem Kriterium ab zwei Umkehrungen) bestehen aus langen parallelen Läufen mit Umkehrstücken. Position 2 ist in allen sieben Routen 14 bis 20 ein solches Level (S1-04-02-02, S1-04-03-02, S1-04-04-02, S1-05-01-02, S1-05-02-02, S1-05-03-02, S1-05-04-02). Beispiel: S1-05-03-02 und S1-05-04-02 haben gleiche Lage (9×9, N1 nach S8), je 42 Gleiszellen, identische Metriken (177 Schritte, Tiefe 8), und 69 von 81 Zellen sind identisch belegt (gleiche Gleisform oder beide leer).
- **Einordnung:** Der Lernbogen verlangt teils „bekannte Startmuster“ und „frühere Logik als Wiedereinstieg“, wiederholte Motive sind also gewollt. Ob sieben Wiedereinstiege mit demselben Muster noch Variation sind, kann nur ein Contentreview mit Spieltest entscheiden. Der Autor nennt die Frage selbst als offene externe Qualitätsfrage („wiederkehrende Kurven-/Streifenmotive“).
- **Auswirkung:** Risiko für wahrgenommene Gleichförmigkeit. Keine technische Auswirkung.

#### Q-02 Lernbögen: Raster konform, messbare Klauseln überwiegend erfüllt, drei Wortlautabweichungen

- **Quelle:** Content-Bible 14 §3 bis §8.
- **Raster:** 240/240 konform. Alle 20 Routen entsprechen den Rastervorgaben (Breite × Höhe), einschließlich der gemischten Routen 7, 11, 15, 18 und 20 (Anhang A). Bei diesen fünf Routen nennt Content-Bible 14 beide Größen, aber keine Positionsgrenze (Route 20 nennt nur 9×9 für Meldungen 1 und 2 sowie 10×10 für 6 bis 8 und 11 und 12). Die Grenze zwischen Position 5 und 6 ist eine Entscheidung des Autors, die dem Text nicht widerspricht.
- **Erfüllt (maschinell prüfbar):** Route 1 Positionen 3 bis 5 enthalten eine Nulllinie, 6 bis 8 eine Vollinie. Route 3 Positionen 3 bis 5 haben angrenzende Kanten. Route 4 Positionen 9 und 10 haben gegenüberliegende Kanten. Route 5 Positionen 3 bis 5 enthalten Null- und Vollinien. Route 14 Positionen 3 bis 8 haben keine einzelne Extremlinie. Die Routen 9, 16 und 17 haben auf den Positionen 1 und 2 eine Extremlinie. Weitere Operationalisierungen des Autors (Abstände A/B in Routen 2, 8 und 11) halten.
- **Abweichungen vom Wortlaut:**
  1. Route 1 Positionen 11 und 12 (S1-01-01-11, S1-01-01-12): „alle Einstiegsmuster kombinieren“. Beide haben keine Nulllinie.
  2. Route 1 Positionen 9 und 10: „erste unterschiedliche A/B-Kanten“. Schon die Positionen 1 bis 8 haben durchgehend verschiedene Kanten, das „Erste“ bildet sich nicht ab. Lesart-abhängig.
  3. Route 19 Positionen 1 und 2: „bekannte Startmuster ohne Vereinfachung“. S1-05-03-01 ist ein 9×9-Level mit 17 Gleiszellen, 3 aktiven Spalten und 6 Nulllinien, also deutlich vereinfacht.
- **Nicht maschinell prüfbar** (Contentreview): z. B. „graue Belegungsmarkierung als optionale Notiz einführen“, Lesbarkeit, „faire Höhepunkte“.

#### Q-03 Der Bestand verlangt an keiner Stelle Schleifen- oder Konnektivitätsschlüsse

- **Messung:** In 23.108 Rootspur-Schritten kommen nur sechs Regelcodes vor (`BORDER_PORT_FORBIDDEN` 4.696, `NEIGHBOR_PORT_REQUIRED` 8.882, `LINE_TARGET_REACHED` 4.324, `LINE_ALL_REMAINING_OCCUPIED` 3.120, `LINE_ZERO` 1.606, `ENDPOINT_ENTRY_REQUIRED` 480). `LOOP_PREVENTION`, `CONNECTIVITY_PRESERVATION` und `LOCAL_DEGREE_REQUIRED` kommen nie vor. Mein Propagator löst alle 240 Level und alle 3.840 Varianten allein mit Ports, Nachbarkonsistenz und Zeilen- und Spaltenzahlen.
- **Einordnung:** Regelkonform (Content-Bible §1: keine neuen Regeln). Für die Aussage „raffinierter arrangiert“ und das Lernziel von Abschnitt 4 („erkennt, wann eine anscheinend naheliegende Verbindung global nicht funktionieren kann“) ist es ein Prüfpunkt für den Contentreview, kein Mangel.

#### Q-04 Steiler Tiefensprung vor den Routenabschlüssen (vom Autor offengelegt)

- **Messung:** Maximale Tiefe Position 10 → 11: Mittelwert Faktor 1,79 über 20 Routen (Min 1,17, Max 2,47). Abschnitte 1 und 2 ca. 1,3, Abschnitte 3 bis 5 zwischen 1,9 und 2,3. Spitzen: S1-05-04-11 Tiefe 42 und S1-05-04-12 Tiefe 40 bei 22 bzw. 24 Gleiszellen auf 10×10. Positionen 11 und 12 wurden in `select_candidates.py` bei Routen ≥ 9 als Maximaltiefe-Kandidaten gewählt (Quantil 1). Dichte der Positionen 11 und 12 in den Abschnitten 4 und 5: Median 0,27. Keine Null- und Vollinien bei den Finalen der Abschnitte 3 bis 5.
- **Einordnung:** Content-Bible verlangt „zwei faire Höhepunkte“. Fairness im Sinn von Lesbarkeit kann nicht aus Tiefe abgeleitet werden. Der Autor nennt die Frage selbst offen (HANDOVER, „Gezielte externe Qualitätsfragen“). Prüfpunkt für den Spieltest.

#### Q-05 46 Level nutzen weniger als 70 % der Zeilen oder Spalten

- **Messung:** Liste in Anhang F. Der Autor setzt diese Schwelle in `select_candidates.py` als Auswahlregel an, aber nur für die Routen 9 bis 20 und dort nur für die Positionen 3 bis 8 sowie 11 und 12 (Gruppenindex 1, 2 und 4 in `fits()`). Die Regel wirkt wie dokumentiert: Alle 24 Treffer in den Routen 9 bis 20 liegen auf den Positionen 1, 2, 9 oder 10, also außerhalb ihres Geltungsbereichs. Die übrigen 22 Treffer liegen in den Routen 1 bis 8, für die die Regel nicht gilt. Beispiele: S1-05-03-01 (9×9, 3 aktive Spalten), S1-03-04-02 (7×6, 2 aktive Spalten), S1-04-03-01 (7×7, 2 aktive Spalten), S1-01-04-04 (4×5, 2 aktive Spalten).
- **Einordnung:** Auf den Positionen 1, 2, 9 und 10 und in den Routen < 9 ist das nach Lernbogen teils gewollt (Wiedereinstieg, Variation). Das HANDOVER nennt „schmale Zweispaltenkorridore“ als Ersetzungsgrund, wendet diese Maßgabe aber nicht auf alle Positionen und Routen an. Das ist eine dokumentierte Auswahlgrenze, kein Verstoß; ob sie fachlich ausreicht, entscheidet die Contentabnahme.

#### Q-06 Herkunft und Reproduzierbarkeit der Auswahl

- **Beobachtung:** Die Level entstammen Zufallspfaden (`RandomPath`) in Pools und werden per Tiefenperzentil und Regelfilter gewählt (`select_candidates.py`). Pools und Auswahldatei liegen in `.utmp` und sind ignoriert, Seeds und Poolaufrufe sind nicht dokumentiert. Die Auswahl lässt sich daher nicht erneut ausführen.
- **Einordnung:** Content-Bible §1 spricht von „handkuratierten“ Meldungen. Der Nachweis der Kuration steht aus (Contentabnahme). Für die Proof-Regeneration ist das ohne Belang (HANDOVER sagt das ausdrücklich).

### 5.3 Fehlende oder nicht ausführbare Nachweise

#### E-01 CI-Referenz-Leistungsnachweis (WP-Test 5) offen

Siehe Abschnitt 6.

#### E-02 CI-Evidenzgrenzen

- Logs und Artefakt-Downloads waren nicht abrufbar (Anmeldung, API-Zugriff nicht freigegeben). Die Läufe wurden über die öffentlichen Seiten gelesen, und die Seiteninhalte kamen über ein zusammenfassendes Abrufwerkzeug zu mir, nicht als Rohdaten. Die Pro-Job-Ergebnisse stehen nicht auf der Seite, nur Gesamtstatus und Laufzeiten.
- Die Unity-CI führt die neuen Level nicht aus (keine `Assets/`-Änderung). Ein CI-PASS sagt nichts über die 240 Rätsel.
- Der Push-Lauf der Unity-CI wurde durch die Concurrency-Regel abgebrochen (siehe Abschnitt 7). Der PR-Lauf ist grün.

#### E-03 Nachweise des Autors stammen von einem lokalen Windows-Rechner

Pfade in `regression-run.txt` zeigen auf `C:/STP/…`, Messzeiten auf `DESKTOP-QG2J9IR`. Mit Abschnitt 4 (P-12, P-13) ist die Aussage auf einer zweiten, unabhängigen Umgebung bestätigt.

### 5.4 Optionale Verbesserungsvorschläge (keine Vertragsverletzung)

| Nr. | Vorschlag | Beleg |
|---|---|---|
| O-01 | Zeilen- und Spaltenindizes in `qualityNote` und Einstiegstexten sind nullbasiert („Zeile 3“ ist die vierte Zeile), das steht nur in den Leveldesign-Dateien. In der Level-Datei selbst fehlt der Hinweis. Beispiel: S1-01-01-01, `rowCounts[3] = 0`, Notiz „Zeile 3 hat Count 0“. | `qualityNote` |
| O-02 | `check_schema_hashes.py` bricht bei jeder anderen Patchversion von Node oder CPython ab (feste Assertion auf v22.20.0). Das WP lässt CPython 3.11.x und Node 22.x zu. | Skript Zeile `assert … =='v22.20.0'` |
| O-03 | Zeitangaben sind uneinheitlich gerundet: Konsole „2,029ms“ (Dezimalkomma, 2,029 ms), JSON 2028 µs, HANDOVER „2.028 ms“. Gemeint ist überall etwa 2 ms. | `authoring-validation.txt`, `technical-evidence.json`, HANDOVER |
| O-04 | Seeds und Aufrufe der Poolerzeugung dokumentieren, falls „handkuratiert“ später belegt werden soll (Q-06). | `Season1Authoring.cs` Modus `pool` |

---

## 6. Dokumentierte Einschränkungen und der offene CI-Referenz-Leistungsnachweis

**Maßstab:** SOLVER_ARCHITECTURE §10, WP-026 Test 5: Lösung zählen bis zwei, 10×10 kuratiert, **p95 < 250 ms und Maximum 2 s**; gesamter Season-1-Katalog in CI **< 60 s** parallelisiert, auf einem CI-Referenzrechner. Überschreitung blockiert Content.

| Messung | p95 / Max (10×10) | Katalog | Herkunft |
|---|---|---|---|
| Autor, `technical-evidence.json` | 2,028 ms / 2,028 ms (7 Level, p95 = Maximum bei 7 Werten) | Summe der 240 Erstgenerierungen 331 ms; Strict-Batch 147 ms; gesamte Verifikation inklusive 3.840 Metamorphosen 7,4 s | Windows, net10.0, ein warmer Prozess |
| Prüfer, gleicher Lauf | 5,264 ms / 5,264 ms | Strict-Batch 326 ms; gesamte Verifikation 12,3 s | Linux, 2 vCPU, net10.0 |
| Bestandstest `TenByTen_FixedCorpus…` (altes Korpus, nicht der neue Bestand) | 2,008 ms / 2,514 ms | | Linux, 2 vCPU |

**Bewertung:**
1. Der Pflichtnachweis (CI-Referenzrechner, neuer 240er-Bestand) ist **nicht erbracht**. Weder lokale Zahlen noch Bestandsgoldens ersetzen ihn (Autor und WP sagen das übereinstimmend). Er bleibt offener Pflichtnachweis und verhindert den Gesamtabschluss.
2. Das Risiko einer Budgetverfehlung ist nach den vorhandenen Zahlen gering. Die gemessenen Werte liegen rund drei Größenordnungen unter dem Maximum und etwa zwei unter dem p95-Budget, auch auf einem schwachen Prüfrechner. Das ist eine Plausibilität, keine Freigabe.
3. Die Messung ist nur bedingt vergleichbar: gemessen wurde `ProofGenerator.Generate` (Zählen bis zwei, Pfadrekonstruktion, Hashen) in einem warmen Prozess, mit nur sieben 10×10-Werten (p95 entspricht dort dem Maximum). Das Budget meint das Zählen bis zwei auf dem Referenzrechner.
4. Das HANDOVER weist alle drei Punkte korrekt aus. Eine Fehldarstellung liegt nicht vor.

Übrige dokumentierte Einschränkungen:

| Einschränkung laut HANDOVER | Bewertung |
|---|---|
| net10.0-Harness statt Unity | Korrekt als Zusatzbeleg gekennzeichnet. Ich habe ihn auf Linux reproduziert (P-12, P-13). Kein Ersatz für Unity-Compile oder IL2CPP. |
| Cross-Reference-Gate FAIL, 1.440 Ziele | Korrekt und vollständig. Inventar deckt sich mit den Dateien. Keine Fallbacks. |
| `UNCALIBRATED`, Sternsekunden `null` | Konform mit LEVEL_DATA_FORMAT §5 (`null` zulässig). `UNCALIBRATED` ist kein Zeitklassenwert, der Autor sagt das selbst. Content-Bible §9 verlangt eine vorläufige Sollzeitklasse, die das WP ausdrücklich ausschließt. Offen für das Folge-WP. |
| Miniaturvorschau nur als Textgitter | CONTENT_PIPELINE §3.3 verlangt eine Miniaturvorschau. Kreative Ausarbeitung ist ausgeschlossen. Gate REQUIRED_LATER, akzeptiert. |
| Unabhängige Abnahme, Spieltest offen | Korrekt getrennt von der Authorbewertung. |
| Auftragskorrektur (BLOCKER-WP026-001 widerlegt) | Quellen stützen die Korrektur. Ich habe sie so angewandt. |

---

## 7. CI-Befund (Stand der Abrufe)

Quelle: öffentliche GitHub-Seiten der Läufe, Commit `3c9b7d0`, ausgelöst am 13.10.2026 um 22:02 (Seitenzeit). Siehe E-02 zu den Grenzen.

| Lauf | Ereignis | Ergebnis | Dauer |
|---|---|---|---|
| Architecture Validation #200 (Run 38475029306) | push | Success | 34 s |
| Architecture Validation #201 (Run 38475032138) | pull_request | Success, Job `validate` | 37 s / 34 s |
| Unity CI #167 (Run 38475029356) | push | **Cancelled**. Annotation: „Canceling since a higher priority waiting request for unity-personal-seat exists“. Concurrency-Abbruch, kein Testfehler | 3 min 34 s |
| Unity CI #168 (Run 38475032171) | pull_request | **Success** (zuletzt abgerufen als beendet) | 41 min 40 s |

Unity PR-Lauf #168: Jobs `project-preflight` 18 s, `unity-config` 3 s, `unity-environment-linux` 1 min 37 s, `-android` 3 min 39 s, `-ios` 3 min 5 s, `unity-compile-editmode` 7 min 37 s, `unity-evidence-guard` 2 s, `unity-playmode-bootstrap-smoke` 3 min 18 s, `unity-android-development-il2cpp` 8 min 44 s, `unity-ios-export` 11 min 27 s. Artefakte: `editmode-results` 411 KB (`sha256:d4f35c9d…ba5d8a`), `playmode-results` 339 KB (`sha256:8d9dcde6…c5641`), `android-development-il2cpp` 37,1 MB (`sha256:2299233b…ccb8a`). Annotationen: vier Warnungen (veralteter Eingabe `save-always`, iOS-Jobs) und zwei Hinweise (macOS-Runner-Kapazität). Ich habe weder die Testzahlen aus den XML-Artefakten noch die Logs einsehen können.

**Einordnung:** Die Architecture-Validation führt dieselben Befehle aus, die ich lokal mit PASS wiederholt habe (P-02, P-03, P-05). Der grüne Unity-PR-Lauf belegt, dass die bestehenden Unity-Gates auf diesem Head weiterlaufen (WP-Test 7: „erhalten und separat berichten“), mehr nicht. Der Push-Abbruch ist eine Konsequenz der Runner-Concurrency, nicht des Inhalts. Die PR-Beschreibung nennt den PR-Unity-Lauf noch als „pending“; das ist der Stand zum Zeitpunkt der Beschreibung und überholt. Eine Bindung der Abnahme an einen konkreten CI-Lauf sollte der Koordinator mit Lauf-IDs im PR festhalten.

---

## 8. Abgrenzung der vier Ebenen

| Ebene | Stand |
|---|---|
| **Korrektheit und Qualität des gelieferten Authoringbestands** | Technische Korrektheit: PASS ohne Befund. Qualität und Nachweis: V-01, V-02, V-03 (Verträge), Q-01 bis Q-06 (fachlich). |
| **Fehlende oder nicht ausführbare Nachweise** | CI-Referenz-Leistung (E-01), CI-Logs und Artefakte (E-02), Unity-seitige Ausführung des 240er-Bestands, Cross-Reference, Pseudolocale, Doppelimport, Addressables, Lizenz, Größe (REQUIRED_LATER), Miniaturvorschau. |
| **Gesamtabschluss und Integrationsbereitschaft von WP-026** | Gesamt-DoD nicht erfüllt, Integration nicht bereit (Abschnitt 1). Ein Teil-PASS in A hebt das nicht auf. |
| **Spätere kreative, menschliche und Produktionsfreigaben** | Titel, Status- und Ergebnistexte, Karten- und Zugmomente, Zeitklassen und Sternsekunden, menschlicher Spieltest, unabhängige Contentabnahme, Produktstatus, Release-Lock, Gerät, Store. Nicht erteilt, nicht ersetzt. |

---

## 9. Offene Punkte und Übergabe

**Offen für den Autor (Befunde, keine Planänderung):** V-01, V-02, V-03 zur Behebung oder zur ausdrücklichen Auslegungsentscheidung des Auftraggebers; Q-01 bis Q-06 zur Bewertung durch die Contentabnahme.

**Offen außerhalb des Autors:** CI-Referenz-Leistungsnachweis auf dem vorgesehenen Runner mit den kanonischen Befehlen; unabhängige Contentabnahme und menschlicher Spieltest; Referenzziele (1.440) und Zeitklassen; Cross-Reference-, Pseudolocale-, Import-, Addressables-, Lizenz-, Größen- und Release-Lock-Gates; Persistierung dieses Berichts nach AI_HANDOVER_RULES (WP-026, CURRENT_STATE).

**Hinweis zur Persistierung:** Nach AI_HANDOVER_RULES §3 gehören Ergebnis, Tests, Einschränkungen und Blocker in die WP-Datei, Letzter und Nächster Schritt in CURRENT_STATE. Dieser Bericht ist dafür so aufgebaut, dass er ohne Chatkontext verständlich ist. Eine Ablage nach `Content/Reviews/season-1/` liegt in der Allowlist des Manifests nur für `*.md`, `*.json`, `*.txt`. Das Ablegen selbst erfolgt durch den Koordinator.

| Schlussverifikation | Wert |
|---|---|
| Zeitpunkt | 14.10.2026, 01:00 Uhr (Europe/Berlin) |
| HEAD von `refs/pull/21/head` | `3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e` (`git fetch origin refs/pull/21/head` und `git ls-remote`, auch Branch `codex/wp-026-season1-logical-content` identisch) |
| `origin/main` | `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` (unverändert, gleich der gemeldeten Basis) |
| Ergebnis | Kein vermischter Prüfstand. HEAD und Basis sind seit Prüfbeginn unverändert. Der PR ist weiterhin Draft mit drei Commits, ohne Reviews und ohne Kommentare Dritter. Unity-PR-Lauf #168 steht weiterhin auf Success. Der Klon blieb unverändert (`git status` leer) |

---

## 10. Reproduktion für Dritte

```text
# Umgebung: CPython 3.11.x mit tools/architecture-validation/requirements.lock.txt, Node 22.x, .NET SDK 10.0.401, g++
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-026.production.scope.json --self-test
git diff --check c9516bdbd5858fd9003d5207ec37d180ccc6ae23 --

# Autor-Verifikation auf frischem Export (schreibt nur nach Content/Reviews/season-1 der Kopie):
git archive 3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e | tar -x -C <leeres-verzeichnis>
cd <leeres-verzeichnis> && python3 tools/season1-authoring/run_authoring.py verify

# Eigene Verifier (Archiv wp026-review-verifiers.tgz):
python -I v1_structure.py <repo> dims.json     # P-06
python -I make_in.py <repo> > in.txt
g++ -O2 -o count count.cpp; ./count 3 < in.txt     # P-07
python v3_propagate.py <repo>                  # P-08
python v4_traces.py <repo>                     # P-10
python -I v2_nodejcs.py <repo> <tmp>           # P-11
python v5_meta.py <repo> <verifier-dir>        # P-09
python similar.py out.json                     # Q-01 (Jaccard)
```

Hinweis: In `appendix.py`, `appendix2.py`, `features.py` und `similar.py` steht der Repo-Pfad als Konstante `R` bzw. Argument am Dateianfang und ist anzupassen. Alle Skripte lesen nur und schreiben ausschließlich in die angegebene Ausgabedatei.

Kern von V-01 (Symmetrische Count-Vektor-Nähe):

```python
def variants(w,h,r,c):
    out=[(r,c),(r,c[::-1]),(r[::-1],c),(r[::-1],c[::-1])]
    if w==h: out+=[(c,r),(c,r[::-1]),(c[::-1],r),(c[::-1],r[::-1])]
    return out
# für alle Paare gleicher (w,h): min(sum(|x-y|)) über variants(a) gegen b.rowCounts+b.columnCounts
```

---

## Anhang

### A. Raster je Route (Content-Bible 14 gegen Lieferung, Breite×Höhe)

| Route (global) | Soll laut Content-Bible 14 | Geliefert (Positionen) | Ergebnis |
|---|---|---|---|
| 01 (S1-01-01) | 4×4 | 4×4 (1-12) | konform |
| 02 (S1-01-02) | 4×5 | 4×5 (1-12) | konform |
| 03 (S1-01-03) | 4×5 | 4×5 (1-12) | konform |
| 04 (S1-01-04) | 4×5 | 4×5 (1-12) | konform |
| 05 (S1-02-01) | 5×5 | 5×5 (1-12) | konform |
| 06 (S1-02-02) | 5×5 | 5×5 (1-12) | konform |
| 07 (S1-02-03) | 5×5 und 6×5 (Grenze nicht festgelegt) | 5×5 (1-5); 6×5 (6-12) | konform, Grenze vom Autor gewählt |
| 08 (S1-02-04) | 6×5 | 6×5 (1-12) | konform |
| 09 (S1-03-01) | 6×6 | 6×6 (1-12) | konform |
| 10 (S1-03-02) | 6×6 | 6×6 (1-12) | konform |
| 11 (S1-03-03) | 6×6 und 7×6 (Grenze nicht festgelegt) | 6×6 (1-5); 7×6 (6-12) | konform, Grenze vom Autor gewählt |
| 12 (S1-03-04) | 7×6 | 7×6 (1-12) | konform |
| 13 (S1-04-01) | 7×7 | 7×7 (1-12) | konform |
| 14 (S1-04-02) | 7×7 | 7×7 (1-12) | konform |
| 15 (S1-04-03) | 7×7 und 8×8 (Grenze nicht festgelegt) | 7×7 (1-5); 8×8 (6-12) | konform, Grenze vom Autor gewählt |
| 16 (S1-04-04) | 8×8 | 8×8 (1-12) | konform |
| 17 (S1-05-01) | 8×8 | 8×8 (1-12) | konform |
| 18 (S1-05-02) | 8×8 und 9×9 (Grenze nicht festgelegt) | 8×8 (1-5); 9×9 (6-12) | konform, Grenze vom Autor gewählt |
| 19 (S1-05-03) | 9×9 | 9×9 (1-12) | konform |
| 20 (S1-05-04) | 9×9 (Meldungen 1-2), 10×10 (Meldungen 6-8 und 11-12), übrige offen | 9×9 (1-5); 10×10 (6-12) | konform, Grenze vom Autor gewählt |

### B. Tiefen- und Zellenverlauf je Route (maxDeductionDepth / Gleiszellen, Positionen 1 bis 12)

| Route | Tiefe | Gleiszellen |
|---|---|---|
| 01 | 4 4 5 5 5 6 6 6 6 6 7 9 | 8 6 11 7 8 12 11 6 7 8 12 10 |
| 02 | 4 5 6 6 6 7 7 7 6 6 9 11 | 9 10 13 10 11 12 16 12 8 14 13 17 |
| 03 | 4 5 6 6 6 7 7 7 7 7 9 12 | 8 11 10 11 10 17 12 9 13 13 16 9 |
| 04 | 4 5 6 6 6 7 7 7 7 7 9 12 | 8 8 16 7 15 14 13 10 12 11 12 15 |
| 05 | 5 6 5 6 6 7 8 8 7 8 10 14 | 9 11 8 13 10 17 12 18 11 10 16 12 |
| 06 | 5 6 8 9 9 7 8 8 7 8 10 14 | 15 9 14 14 14 10 10 18 10 11 12 9 |
| 07 | 5 5 9 9 9 11 11 12 8 9 12 16 | 14 7 14 11 14 17 12 13 15 15 19 22 |
| 08 | 5 6 7 8 8 8 9 9 10 11 14 18 | 18 9 9 19 9 16 12 15 16 21 20 23 |
| 09 | 6 6 9 10 10 11 12 13 11 13 22 23 | 13 12 19 14 20 17 15 22 12 17 15 12 |
| 10 | 6 6 9 10 10 11 12 13 9 10 19 19 | 18 19 19 13 17 13 20 14 22 14 16 21 |
| 11 | 6 6 9 10 10 12 12 13 11 12 25 23 | 17 9 19 21 14 23 19 19 14 15 13 28 |
| 12 | 6 7 10 11 12 13 13 14 10 11 22 24 | 20 10 20 14 21 13 17 23 20 13 20 16 |
| 13 | 6 7 10 11 11 12 13 14 10 13 29 26 | 20 11 23 17 21 19 19 25 29 25 14 20 |
| 14 | 6 7 13 17 18 19 20 22 12 13 24 26 | 11 29 24 21 15 21 19 22 15 13 31 15 |
| 15 | 6 7 10 10 11 12 14 14 12 14 34 33 | 11 27 19 27 19 20 35 35 20 21 19 17 |
| 16 | 7 7 10 11 12 12 13 14 11 15 33 30 | 14 30 32 19 19 25 22 25 25 19 17 32 |
| 17 | 7 7 10 11 10 13 13 15 12 14 32 30 | 12 32 30 17 18 23 17 24 19 20 17 19 |
| 18 | 7 7 11 10 12 14 15 16 14 14 32 30 | 20 34 26 19 21 34 41 24 44 41 21 22 |
| 19 | 7 8 12 10 15 14 15 16 14 15 31 29 | 17 42 39 22 28 32 37 36 25 24 22 22 |
| 20 | 7 8 12 13 11 23 23 27 15 17 42 40 | 12 42 40 25 24 38 34 28 27 29 22 24 |

### C. Belegungsüberlappung der Gleiszellen (Jaccard, beste D4-Transformation), Paare ab 0,85

Verglichene Paare: 3183. Paare ab 0,90: 4, ab 0,85: 20, ab 0,80: 48. „Gleiche Ports“ zählt die Endpunkte (Seite und Index), die unter der besten Transformation übereinstimmen (0, 1 oder 2).

| Jaccard | Gleiche Ports | Level A | Level B | Gleiszellen A / B | Lage |
|---|---|---|---|---|---|
| 1,000 | 0 | S1-01-03-03 | S1-01-04-08 | 10 / 10 | gleicher Abschnitt |
| 0,947 | 1 | S1-03-02-01 | S1-03-02-02 | 18 / 19 | gleiche Route |
| 0,933 | 1 | S1-02-02-01 | S1-02-03-01 | 15 / 14 | gleicher Abschnitt |
| 0,930 | 0 | S1-05-02-07 | S1-05-04-02 | 41 / 42 | gleicher Abschnitt |
| 0,892 | 0 | S1-04-03-07 | S1-04-03-08 | 35 / 35 | gleiche Route |
| 0,889 | 1 | S1-05-02-09 | S1-05-02-10 | 44 / 41 | gleiche Route |
| 0,889 | 1 | S1-02-01-03 | S1-02-02-02 | 8 / 9 | gleicher Abschnitt |
| 0,886 | 2 | S1-05-01-02 | S1-05-02-02 | 32 / 34 | gleicher Abschnitt |
| 0,882 | 1 | S1-01-02-07 | S1-01-04-03 | 16 / 16 | gleicher Abschnitt |
| 0,881 | 1 | S1-05-03-03 | S1-05-04-03 | 39 / 40 | gleicher Abschnitt |
| 0,879 | 2 | S1-05-01-02 | S1-05-01-03 | 32 / 30 | gleiche Route |
| 0,875 | 0 | S1-02-03-09 | S1-02-03-10 | 15 / 15 | gleiche Route |
| 0,867 | 2 | S1-04-02-02 | S1-04-03-02 | 29 / 27 | gleicher Abschnitt |
| 0,867 | 0 | S1-03-01-04 | S1-03-02-08 | 14 / 14 | gleicher Abschnitt |
| 0,865 | 1 | S1-04-03-08 | S1-05-02-02 | 35 / 34 | abschnittsübergreifend |
| 0,861 | 1 | S1-04-03-07 | S1-04-04-03 | 35 / 32 | gleicher Abschnitt |
| 0,857 | 2 | S1-02-02-04 | S1-02-02-11 | 14 / 12 | gleiche Route |
| 0,857 | 1 | S1-01-01-02 | S1-01-01-04 | 6 / 7 | gleiche Route |
| 0,857 | 0 | S1-01-02-03 | S1-01-03-09 | 13 / 13 | gleicher Abschnitt |
| 0,854 | 1 | S1-05-03-08 | S1-05-04-03 | 36 / 40 | gleicher Abschnitt |

### D. Count-Vektor-Nähe unter D4 (L1 ≤ 2, gleiche Rastergröße)

Der Ähnlichkeitsbericht des Autors vergleicht nur gleich orientierte Vektoren (kleinster Abstand 4).

| L1 | Level A | Level B | A (Endpunkte) | B (Endpunkte) |
|---|---|---|---|---|
| 0 | S1-01-03-03 | S1-01-04-08 | N0 / W3 | N2 / E4 |
| 2 | S1-01-01-02 | S1-01-01-04 | E2 / W2 | S2 / E0 |
| 2 | S1-01-02-07 | S1-01-04-03 | N2 / E2 | N0 / S1 |
| 2 | S1-02-01-03 | S1-02-02-02 | N2 / S3 | E1 / W3 |
| 2 | S1-02-02-01 | S1-02-03-01 | N0 / S4 | W0 / E3 |
| 2 | S1-03-02-01 | S1-03-02-02 | W1 / E5 | N0 / S5 |
| 2 | S1-04-03-07 | S1-05-02-02 | W0 / E1 | W0 / E0 |

### E. Schlangenmotiv (lange parallele Läufe mit Umkehrstücken)

Kriterium: mindestens drei Umkehrungen zwischen langen gegenläufigen Läufen (Lauflänge ≥ max(3, 0,6 × Rasterspanne)), höchstens drei Zellen Zwischenstück. Treffer: 22 Level.

S1-03-04-08, S1-04-01-03, S1-04-01-09, S1-04-02-02, S1-04-03-02, S1-04-03-04, S1-04-03-07, S1-04-03-08, S1-04-04-02, S1-04-04-03, S1-05-01-02, S1-05-01-03, S1-05-02-02, S1-05-02-07, S1-05-02-09, S1-05-02-10, S1-05-03-02, S1-05-03-03, S1-05-03-07, S1-05-04-02, S1-05-04-03, S1-05-04-06

### F. Level mit weniger als 70 % aktiven Spalten oder Zeilen

Aktiv = Count größer 0. Treffer: 46 Level, davon 24 in den Routen 9 bis 20 (alle auf den Positionen 1, 2, 9 oder 10, also außerhalb der Auswahlregel des Autors) und 22 in den Routen 1 bis 8.

| Level | Raster | Aktive Spalten | Aktive Zeilen | Gleiszellen |
|---|---|---|---|---|
| S1-01-01-02 | 4×4 | 4/4 | 2/4 | 6 |
| S1-01-01-04 | 4×4 | 2/4 | 4/4 | 7 |
| S1-01-02-09 | 4×5 | 4/4 | 2/5 | 8 |
| S1-01-03-02 | 4×5 | 4/4 | 3/5 | 11 |
| S1-01-03-03 | 4×5 | 2/4 | 5/5 | 10 |
| S1-01-04-01 | 4×5 | 4/4 | 3/5 | 8 |
| S1-01-04-02 | 4×5 | 3/4 | 3/5 | 8 |
| S1-01-04-04 | 4×5 | 2/4 | 5/5 | 7 |
| S1-01-04-08 | 4×5 | 2/4 | 5/5 | 10 |
| S1-02-01-01 | 5×5 | 3/5 | 5/5 | 9 |
| S1-02-01-03 | 5×5 | 3/5 | 5/5 | 8 |
| S1-02-01-05 | 5×5 | 5/5 | 3/5 | 10 |
| S1-02-02-06 | 5×5 | 5/5 | 2/5 | 10 |
| S1-02-02-07 | 5×5 | 3/5 | 5/5 | 10 |
| S1-02-02-09 | 5×5 | 5/5 | 3/5 | 10 |
| S1-02-02-10 | 5×5 | 5/5 | 3/5 | 11 |
| S1-02-03-02 | 5×5 | 5/5 | 2/5 | 7 |
| S1-02-04-02 | 6×5 | 3/6 | 5/5 | 9 |
| S1-02-04-03 | 6×5 | 6/6 | 2/5 | 9 |
| S1-02-04-05 | 6×5 | 3/6 | 5/5 | 9 |
| S1-02-04-07 | 6×5 | 6/6 | 2/5 | 12 |
| S1-02-04-08 | 6×5 | 4/6 | 5/5 | 15 |
| S1-03-01-01 | 6×6 | 6/6 | 4/6 | 13 |
| S1-03-01-02 | 6×6 | 6/6 | 3/6 | 12 |
| S1-03-01-09 | 6×6 | 4/6 | 6/6 | 12 |
| S1-03-02-09 | 6×6 | 6/6 | 4/6 | 22 |
| S1-03-03-02 | 6×6 | 3/6 | 6/6 | 9 |
| S1-03-03-09 | 7×6 | 6/7 | 3/6 | 14 |
| S1-03-04-02 | 7×6 | 2/7 | 5/6 | 10 |
| S1-03-04-09 | 7×6 | 7/7 | 4/6 | 20 |
| S1-03-04-10 | 7×6 | 7/7 | 4/6 | 13 |
| S1-04-01-02 | 7×7 | 7/7 | 2/7 | 11 |
| S1-04-02-01 | 7×7 | 4/7 | 5/7 | 11 |
| S1-04-02-09 | 7×7 | 7/7 | 3/7 | 15 |
| S1-04-02-10 | 7×7 | 3/7 | 7/7 | 13 |
| S1-04-03-01 | 7×7 | 2/7 | 7/7 | 11 |
| S1-04-03-09 | 8×8 | 8/8 | 5/8 | 20 |
| S1-04-04-01 | 8×8 | 8/8 | 2/8 | 14 |
| S1-05-01-01 | 8×8 | 8/8 | 2/8 | 12 |
| S1-05-01-09 | 8×8 | 5/8 | 8/8 | 19 |
| S1-05-01-10 | 8×8 | 5/8 | 6/8 | 20 |
| S1-05-02-01 | 8×8 | 8/8 | 5/8 | 20 |
| S1-05-03-01 | 9×9 | 3/9 | 9/9 | 17 |
| S1-05-03-09 | 9×9 | 9/9 | 6/9 | 25 |
| S1-05-03-10 | 9×9 | 6/9 | 6/9 | 24 |
| S1-05-04-01 | 9×9 | 4/9 | 6/9 | 12 |

Positionen der Treffer in Routen 9 bis 20: [1, 2, 9, 10].
