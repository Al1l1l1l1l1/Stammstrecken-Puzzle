# WP-026 – Season-1-Logischer-Raetselbestand

## ID

`WP-026`

**Status:** Beauftragt am 13.10.2026; Voraussetzungenprüfung. Noch keine Rätsel geliefert und keine fachliche Abnahme. Dieses Package verankert den vom Auftraggeber verlangten vollständigen logischen Bestand, keine neue Seasonplanung. Fehlende Voraussetzungen bleiben echte Übergabepunkte; es gibt keine künstliche Beispiel- oder Teilmengenbegrenzung.

[Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-026.production.scope.json)

## Ziel

Die bereits in Konzeptbaustein 14 geplanten 240 Rätsel der Season 1 liegen mit konkretem öffentlichem Puzzleinput, vollständiger A→B-Lösung, aus ihr abgeleiteten Randzahlen, regenerierbaren solver-v2-Proofs, nachvollziehbaren Einstiegsschlüssen und fachlicher Leveldesignbewertung vor. Die 20 Routen und ihre 12er-Lernbögen bleiben unverändert. Ein logischer Bestand ist keine fertig ausgestaltete, importierbare oder freigegebene Season.

## Voraussetzungen

1. Vollständige Lesereihenfolge aus [AGENTS.md](../AGENTS.md): Projektübergabe 15, Index 00, Master 09, CURRENT_STATE und dieses WP; anschließend Konzeptbausteine 03, 04 und 14, WORK_QUEUE, WORK_PACKAGE_RULES, AI_HANDOVER_RULES und DEFINITION_OF_DONE.
2. Maßgeblich sind [ADR-021](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [ADR-007](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [ADR-030](../DECISIONS/ADR-030-wp-scope-trust-anchor.md), [ADR-031](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md), LEVEL_DATA_FORMAT, SOLVER_ARCHITECTURE, CONTENT_PIPELINE, CONTENT_CATALOGS, TEST_STRATEGY, die unveränderten Level-/Proofschemata und die Validator-README.
3. Tatsächlicher Main-Stand bei Beginn: `c9516bdbd5858fd9003d5207ec37d180ccc6ae23`. WP-021 bis WP-025 sind integriert; WP-025 über PR #19 (`f496ab9001d2043a17c3ffd2ff2a8a39d175495b`), Closeout PR #20. Der zuvor lokale WP-025-Stand war veraltet. Seine Arbeit/Abnahme wird nicht erneut gestartet. Frische Remote-Abfrage: kein offener PR; keine WP-026-Datei oder Manifest in der vollständigen abgerufenen Historie.
4. Neue eigene Branch `codex/wp-026-season1-logical-content` von genau dieser Basis. WP und Manifest erstmals gemeinsam im ersten Branchcommit; einziger Elterncommit = Manifestbasis. Manifest danach byteunveränderlich. Erst nach diesem Anker fachliche Änderungen. Vor Commit/Push laufende fremde Arbeit und relevante CI berücksichtigen.
5. Neue Authoringdaten verwenden den vorhandenen vollständigen `level-v2`-Vertrag. Pflichtreferenzen werden ausschließlich aus tatsächlichen maßgeblichen Quellen übernommen. Keine erfundenen Texte, Referenzziele, Dummy-Keys, Fixture-Übernahmen oder alternativen kanonischen Levelverträge. Ein fehlender zulässiger Weg für unvollständige kreative Metadaten ist als Blocker nachzuweisen, nicht durch Schema-/Loader-/Proofgeneratoränderung zu umgehen.
6. Der Nutzerauftrag autorisiert die gesamte zulässige Erstellung, eigene Korrekturen, Prüfungen, normale Commits/Pushes, Draft-PR und Nachweispersistierung. Nach internen Teilschritten ist kein erneuter Fortsetzungsauftrag erforderlich. Ausdrücklich erforderliche externe Entscheidungen und unerfüllte Voraussetzungen bleiben davon unberührt.

## Scope

1. Quellengetreue Verankerung dieses vollständigen Folgeauftrags, tatsächlichen Stand und laufende Arbeit abgleichen, fehlende Voraussetzungen konkret nachweisen und Übergabe persistieren.
2. Alle 240 Meldungen gemäß unverändertem Konzeptbaustein 14 kuratieren: fünf Abschnitte, je vier lokale Routen, je zwölf Positionen; IDs `S1-01-01-01` bis `S1-05-04-12`. Globale Routen 01–20 aus der Content-Bible werden auf Abschnitt und lokale Route 01–04 abgebildet; die im Konzepttext vorkommende Beispiel-ID mit Route 11 ersetzt nicht den angenommenen ADR-/Level-v2-Vertrag.
3. Raster, A/B, konkrete Formen, Pfad und Counts erzeugen; genau eine Lösung und `requiredGuessDepth = 0` mit dem integrierten unveränderten Produktionssolver nachweisen. Rein technische Authoringhilfen dürfen dessen öffentliche APIs konsumieren, aber keine zweite Regelautorität, neue Pipeline, Solverversion oder Dauerbaustellenproduktion bilden.
4. Proofs ausschließlich durch den vorhandenen Produktions-Proofgenerator aus öffentlichen Daten erzeugen und mit der gemeinsamen Strict-Pipeline frisch regenerieren, binden und bytegenau vergleichen. Alle vier Metriken und vollständige Root-Deduktionsspuren als tatsächliche Evidenz, keine geschätzten Proofwerte.
5. Pro Level sachlicher interner Einstiegsschluss, Fokus, Schlusskettentiefe und Qualitätsbegründung; pro Route Prüfung der Gruppen 1–2/3–5/6–8/9–10/11–12 und der Abschnittsrolle. Dichte, Zahlenverteilung, A/B-Lage, Geometrie, faire Deduktion, Lesbarkeit und Nichtredundanz bewerten; Metriken sind keine automatische menschliche Qualitätsfreigabe. Ähnlichkeit einschließlich Rotation, Spiegelung und A/B-Tausch berichten und fachlich beurteilen.
6. Vorgeschriebene technische Prüfungen, tatsächliche Berichte und offene kreative/Import-/Releasegates getrennt ausweisen. Sachliche Tabellen/Rasterdarstellungen im Prüfbericht sind Prüfdaten, keine Spielgrafik oder Gestaltung. Geforderte kreative Miniatur-/Präsentationsnachweise bleiben offen, wenn sie ausgeschlossene Arbeit voraussetzen.
7. Normale Commits/Pushes auf eigener Branch und Draft-PR, aktuelle CI selbst prüfen; unabhängige Abschlussprüfung auf eingefrorenem HEAD vor späterer Integration. Nur tatsächlich unabhängige Ausführung oder externe Übergabe, keine simulierte Freigabe. Eigene Fehler im Scope beheben und betroffene Nachweise erneuern.

## Betroffene Dateien/Module

Abschließende Außengrenze ist das oben verlinkte Manifest. Zulässig sind:

| Pfad | Funktion |
|---|---|
| `WORK_PACKAGES/WP-026_Season-1-Logischer-Raetselbestand.md` **neu** | Auftrag im Anker; anschließend ausschließlich Status, Befunde, Nachweise und Übergabe. |
| `tools/architecture-validation/scopes/WP-026.production.scope.json` **neu** | Gemeinsamer unveränderlicher Scopeanker. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Mechanische Fortschreibung; keine Prioritäts-/Gateänderung. |
| `.github/workflows/validate.yml` | Ausschließlich vorhandene Manifestzuordnung auf WP-026. |
| `Content/Levels/s1/<section>/<route>/<puzzleId>.level-v2.json` **neu** | Kanonische Leveldaten; Abschnitt 01–05, lokale Route 01–04. Nur nach erfüllten betroffenen Voraussetzungen. |
| `Content/Proofs/<puzzleId>/solver-v2.proof-v1.json` **neu** | Tatsächliche Produktionsproofs, keine frei konstruierten Artefakte. |
| `Content/Reviews/season-1/*.md`, `*.json`, `*.txt` **neu** | Logische Einzel-/Routenbewertung, Inventar, Rootspur, Ähnlichkeits-/Prüfberichte, offene Referenzen und externe Abnahme. |
| `tools/season1-authoring/*.cs`, `*.csproj`, `*.py`, `*.md` **neu, nur bei Bedarf** | Reproduzierbare Authoring-/Prüfaufrufe vorhandener APIs, keine Produktionsmoduländerung oder neue Abhängigkeit. |

Vorhandene Konzepte, ADRs, Architektur, Produktionscode, Tests, Schemata, Fixtures, Manifeste und kreative Inhalte sind ausschließlich lesende Quellen. Kataloge, Lokalisierung, Assets und Release-Locks gehören nicht zum Schreibscope. Ein nicht vorhersehbarer Bedarf außerhalb dieser Außengrenze ist ein Übergabeblocker.

## Ausdrücklich nicht erlaubte Änderungen

- Kein Merge, Rebase, Force-Push, Runtimeimport, Release, Release-Lock oder PRODUCT_APPROVED-Status.
- Keine Änderung der Planung von Manus, Projektregeln, bestehenden Paketdefinitionen/-grenzen, Architektur, Solver-/Proof-/Hash-/Schema-/Strict-Semantik, Abhängigkeiten oder Freigabegrenzen.
- Keine Humor-, Pointen-, Meme-, Spieltext-, Lokalisierungs-, Grafik-, Gestaltungs-, Zugmodell-, Kosmetik-, Animations- oder Audioarbeit; vorhandene kreative Inhalte bleiben unverändert.
- Keine finalen Zeitwerte oder Zeitkalibrierung. `starThresholdsSeconds` bleibt nach bestehendem Vertrag `null`. Eine im vollständigen Schema verlangte vorläufige Zeitklasse darf keine erfundene Sekundenkalibrierung oder nicht belegte Produktentscheidung enthalten; ungeklärte Werte bleiben als Lücke sichtbar.
- Keine erfundenen kreativen Referenzen oder Freigaben, keine generischen Fallback-IDs und keine Ersetzung fehlender Produktionsquellen durch FIXTURE_ONLY-Beispiele.
- Keine automatische Qualitätsfreigabe, kein Metriketikett als Ersatz für Leveldesignbewertung, keine Übertragung alter CI-/QC-PASS-Aussagen auf neue Artefakte.
- Keine Wiederaufnahme bereits abgeschlossener WP-025-Arbeit oder paralleler Auftrag. Dauerbaustelle und BLOCKER-PROD-001 bis -003 bleiben unverändert außerhalb dieses Auftrags.

## Akzeptanzkriterien

| ID | Kriterium |
|---|---|
| AK-01 | Gemeinsamer historischer WP-/Manifestanker auf tatsächlichem Main; kompletter Diff innerhalb der unveränderlichen Allowlist, zugehörige CI-Manifestzuordnung und nachvollziehbare Übergabe. |
| AK-02 | Exakt 240 konkrete, eindeutig identifizierte Levelquellen; 5×4×12 vollständig, Reihenfolge und Raster-/Lernbögen aus Content-Bible 14 pro Route nachgewiesen. Keine bloßen Beispiele oder Kopien unter neuen IDs. |
| AK-03 | Jede Lösung ist ein gültiger einfacher A→B-Pfad aus sechs Formen; Counts neu abgeleitet; unverändertes Schema und Domainsemantik bestanden. |
| AK-04 | Für jedes Level echter solver-v2-Produktionsproof: UNIQUE/1, vier tatsächliche Metriken, Guessdepth 0; öffentliche Puzzle-/Lösungs-/Proofhashes und Referenzbindung stimmen. Vollständige Rootspur und Metrikrekonstruktion nachvollziehbar. |
| AK-05 | Frische Regeneration aller 240 Proofs ergibt gleiche kanonische Bytes; gemeinsame Strict-Einzel-/Batchprüfung sowie unabhängiger Node-JCS-Abgleich der tatsächlichen neuen Projektionen bestanden. Kein Ersatz durch Legacy-Goldens. |
| AK-06 | Individuelle Einstiegsschlüsse, sachliche Qualitätsnotizen und 20 Routenbewertungen belegen Dramaturgie, Fairness, Variation und Nichtredundanz. Ähnlichkeitsbericht und fachliche Bewertung liegen vor; Spieltest und unabhängige Contentabnahme sind getrennt ausgewiesen. |
| AK-07 | Fehlende kreative Quellen, Cross-References und spätere Produktions-/Import-/Releaseprüfungen sind konkret mit betroffenen IDs/Feldern und Zuständigkeit als offen/blockiert ausgewiesen; keine falsche Schemagültigkeit oder Seasonfreigabe. |
| AK-08 | Eigene aktuelle technische Prüfungen und unabhängige Abschlussbewertung sind an denselben finalen Lieferstand gebunden, tatsächliche Berichte persistiert. Offene Voraussetzungen verhindern einen Gesamtabschluss; fertige zulässige Teilergebnisse bleiben ausdrücklich als solche übergeben. |

## Tests

1. Kanonische Architecture-only- und WP-026-Production-Scope-Läufe mit `--self-test` auf Linux/macOS, CPython 3.11.x und Node 22.x gemäß Validator-README; CI verwendet 3.11.13/22.20.0. Ankerhistorie, Parent/Base/Ancestry, Manifestbytegleichheit, vollständiger Diff einschließlich untracked, Whitespace und Secrets prüfen.
2. Alle neuen Level gegen echtes Draft-2020-12-Schema und vorhandenen C#-Loader; Counts, Formen, Endpoint-/Pfadsemantik, eindeutige IDs und 240er-Vollständigkeit. Keine Lockerung für ausgeschlossene kreative Felder.
3. Produktionsgenerator und gemeinsame Strict-Pipeline für den gesamten Bestand, mehrfacher Bytevergleich, Node-JCS der vollständigen neuen Proofprojektion und öffentlichen Puzzle-/Lösungsprojektionen. Metrikrekonstruktion aus tatsächlicher Rootspur. Budgets/Abbruch bleiben fail-closed; keine INDETERMINATE-Ausgabe als UNIQUE.
4. Metamorphosen Rotation/Spiegelung/A-B-Tausch, Golden-/Mutationsnachweise gegen Counts-/Endpoint-/Pfad-/Hash-/Proofverfälschung; einschlägige vorhandene Domain-/Solver-/Contentregressionen. Keine erneute fachliche Abnahme der unveränderten WP-025-Implementierung.
5. Solverbudget für kuratierte 10×10 auf CI-Referenzrechner: p95 < 250 ms, Maximum 2 s; Gesamtbestand < 60 s parallelisiert laut SOLVER_ARCHITECTURE. Kein Raten aus Rastergröße und keine Spielerzeitkalibrierung.
6. Pro-Level-/Routenreview und Ähnlichkeitsbericht einschließlich symmetrischer Varianten. Unabhängige Abschlussprüfung vor Integration muss technische Nachweise und Contentqualität tatsächlich beurteilen; bei fehlendem regelkonformem Weg externe prüffertige Übergabe. Keine erfundenen menschlichen Spieltests.
7. Cross-Reference-/Pseudolocale-/Doppelimport-/Addressables-/Lizenz-/Größengates aus CONTENT_PIPELINE werden nicht gestrichen. Soweit fehlende fremde Inhalte oder der ausdrückliche Import-/Kreativausschluss ihre Ausführung verhindern: REQUIRED_LATER/NOT_EXECUTED mit konkreter Ursache; keine Inhaltsfreigabe. Bestehende Unity-CI-Gates erhalten und echte Ergebnisse separat berichten.

Kanonische Befehle (vorhandene Umgebung gemäß README):

```text
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-026.production.scope.json --self-test
git diff --check c9516bdbd5858fd9003d5207ec37d180ccc6ae23 --
```

## Risikoklasse

**Hoch.** Der erste vollständige Kampagnenbestand bindet dauerhafte IDs, Eindeutigkeits-/Deduktionsnachweise und das didaktische Qualitätsversprechen. Ein technischer Proof darf keine fehlende kreative, menschliche oder produktseitige Freigabe ersetzen.

## Definition of Done

Die [projektweite DoD](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) gilt vollständig. AK-01 bis AK-08 einzeln prüfen; tatsächliche Tests, Dateien, Einschränkungen, externe Prüfrollen und Folgegates in diesem WP und CURRENT_STATE dokumentieren. Kein Gesamtabschluss bei fehlenden Rätseln, nicht regenerierten Proofs, unerfüllten Voraussetzungen oder ausstehender vorgeschriebener Abnahme. Die Verankerung dieses Auftrags ist keine Lieferung der 240 Rätsel. Normale Remoteübergabe mit konkretem HEAD, Draft-PR und tatsächlichen CI-Status sichern; spätere Integration bleibt ausdrücklich ausgeschlossen.

## Ausführung und blockierte Übergabe – 13.10.2026

**Status: offen/blockiert durch BLOCKER-WP026-001; 0/240 konkrete Rätsel und 0/240 neue Proofs.** Keine formale Gesamtabnahme. Die Paketdefinition oben bleibt unverändert; dieser Abschnitt hält den tatsächlichen Ausführungsstand fest.

### Anker und tatsächliche Dateien

Gemeinsamer Erst-Add von genau WP und eigenem Manifest in `f9231c73e45ade33450c541e77dbeff5d2eae162`; einziger Elterncommit = Manifestbasis = `c9516bdbd5858fd9003d5207ec37d180ccc6ae23`. Manifest-SHA-256 `981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c`, nach dem Anker unverändert. Kein anderer WP-/Manifest-/Architektur-/Produktionsbestand verändert.

Nach dem Anker erstellt: [Startgate-/Übergabebericht](../Content/Reviews/season-1/START_GATE_REVIEW.md), [vollständiger unabhängiger Gegenbericht](../Content/Reviews/season-1/INDEPENDENT_START_GATE_REVIEW.md), [tatsächliche API-Ausgabe](../Content/Reviews/season-1/reference-gate-probe.txt) und reproduzierbarer isolierter Diagnosewrapper `tools/season1-authoring/reference_gate_probe.py` / `ReferenceGateProbe.cs`. CURRENT_STATE und WORK_QUEUE führen ausschließlich den erteilten Folgeauftrag und Blocker nach; validate.yml ändert ausschließlich die vorhandene Manifestzuordnung. Keine Level-/Proof-, Katalog-, Lokalisierungs-, Asset- oder Release-Lock-Datei angelegt.

### Blocker und unabhängige Gegenprüfung

Für sämtliche 240 IDs fehlen belegte Werte von `/content/titleKey`, `/content/statusKey`, `/production/entryDeductionKey`, `/completion/mapSegmentId`, `/completion/trainMomentId`, `/completion/resultTextKey`. Level-v2-Schema und Produktionsreader verlangen sie vor dem Solver. Die tatsächliche Main-Basis enthält keine kanonischen Contentquellen; vorhandene Beispiele sind FIXTURE_ONLY. Der Nutzer schließt das Erfinden solcher Referenzen aus. Ein bestehender regelkonformer Logik-only-Quell-/Proofweg wurde nicht gefunden. Die getrennte reine Quellen-/Codegegenprüfung durch `/root/content_startgate_review` bestätigt den engen Befund; sie ist keine WP-025-Abnahme oder finale WP-026-Contentprüfung.

Fehlende finale Assets, Produktfreigaben und Release-Locks sind ausdrücklich **nicht** zum generellen Authoringstartgate erklärt; DRAFT-Authoring bleibt laut Vertrag zulässig. Hier scheitert bereits die Belegung der Pflichtfelder ohne erfundene Referenzen. Pure Solverexperimente wären nur Zwischenevidenz und kein Ersatz für den kanonischen Produktionsproof. Bewusst schemaungültige Entwürfe oder ein neues Datenformat wurden mangels dokumentierter Ausnahme nicht als Produktion persistiert.

Auflösung: tatsächliche maßgebliche Zuordnungen durch Text-/Lokalisierungs- und Completion-/World-Verantwortliche liefern, oder die Vertragslücke separat nach bestehendem Entscheidungsverfahren bestätigen lassen. Kein bloßer neuer Fortsetzungsauftrag erforderlich. Dieser Agent verändert dafür keine Architektur oder Schemaprüfung. Details und weitere offene Nachweise im verlinkten Bericht.

### Tatsächliche Prüfungen und Evidenzgrenzen

| Prüfung/Kategorie | Tatsächliches Ergebnis |
|---|---|
| API-Gegenprobe, eigener Zusatzharness | SDK 10.0.401, getrennte pure Assemblies aus unveränderten Quellen; Build Exit 0, 0 Warnungen/Fehler. Eine gültige bestehende Fixture als positive Kontrolle, 18 fehlend/null/leer-Mutationen der sechs Referenzen erwartungsgemäß vor Solver abgewiesen. Produktionsgenerator liefert keinen Proof. Eingecheckter Reproduktionswrapper ebenfalls Exit 0. Kein Season-1-Solvernachweis, kein Unityersatz. |
| LOCAL_DOCUMENT_STRUCTURE / LOCAL_ARCHITECTURE_SEMANTICS | Erster kanonischer Linuxlauf mit CPython 3.11.13/Node 22.20.0 und exakt sechs Lockpaketen: Architecture-only 17 Gruppen PASS einschließlich Self-/Negativtests. Bestehender Nodecheck 12 Kanonisierungen/32 Ablehnungen/2 v2-/2 v1-Vektoren PASS. Nur Governance-/Contractnachweis. |
| LOCAL_SCOPE, erster Lauf | FAIL `git:main-changed`: unbenutzter lokaler main-Verweis war noch 5f51273, origin/main bereits c9516bd. Daraufhin ausschließlich normaler Fast-forward-Fetch `git fetch origin main:main`; keine Integration, kein Merge oder Rebase. Betroffener Scope-Lauf vor Remoteübergabe erneut auszuführen, keine PASS-Übertragung. |
| MANUAL_ARCHITECTURE_REVIEW | Konkrete Pflichtfeld-/Quellen-/Produktionspfadgrenze selbst gelesen und unabhängig gegengeprüft; kein automatischer PASS für fachliche Textsemantik. |
| CONTRACT_ONLY | Vollständiger 240er-Auftrag vorhanden; tatsächliche Rätsel, Rootspuren und Produktionsproofs fehlen. |
| REQUIRED_LATER/NOT_EXECUTED | 240er-Schema/Domain/Solver-/Proof-/Metrik-/Ähnlichkeits-/Leveldesign-/Spieltest-/Abnahmenachweise noch nicht ausgeführt; kreative Cross-References, Pseudolocale, Doppelimport, Addressables, Lizenz-/Größen-/Release-/Geräte-/Storeprüfungen nicht als erfüllt dargestellt. |
| BLOCKED | BLOCKER-WP026-001 wie oben. Produktfolgeblocker 001–003 bleiben unverändert und sind keine zusätzliche allgemeine Contentstartsperre. |

Erste .NET-Scratchausführung scheiterte am fehlenden Restore (NETSDK1004); nach regulärem Restore und mit dem reproduzierbaren Wrapper bestanden. Die kurzlebige Linuxumgebung unter `/tmp` war in einer späteren Sitzung nicht mehr verfügbar; ein solcher Startversuch ist kein Testlauf/PASS. Der folgende kanonische Wiederholungslauf richtet dieselbe isolierte gepinnte Umgebung neu ein, ohne Repositoryabhängigkeiten oder Validatoren zu ändern. Finale saubere HEAD-/CI-/Remote-/Scopebindung wird nach tatsächlicher Ausführung im Draft-PR persistiert, damit der Bericht keinen rekursiven Prüfcommit erzeugt.

### Akzeptanzkriterien und nächster Schritt

| Kriterien | Status |
|---|---|
| AK-01 | Gemeinsamer Anker vorhanden; vollständige finale Scope-/CI-/Remoteevidenz vor Übergabe nachzuführen. |
| AK-02 bis AK-06 | Nicht erfüllt; keine 240-Level-Lieferung und keine entsprechenden Nachweise behauptet. Blocker verhindert den vorhandenen vollständigen Produktionsweg. |
| AK-07 | Fehlende Felder, alle betroffenen IDs, Zuständigkeiten und nachgelagerte Gates konkret dokumentiert, keine falsche Freigabe. |
| AK-08 | Keine Abschlussabnahme des Bestands; nur tatsächliche Startgate-Gegenprüfung. Vollständiger Auftrag bleibt offen/blockiert. |

Letzter abgeschlossener fachlicher Schritt: Main-/PR-Rekonstruktion, Auftragsverankerung und konkrete Pflichtreferenz-Gegenprobe. Nächster fachlicher Schritt: belegte Referenzzuordnung oder separat bestätigte Vertragslösung. Erreichter Stand wird normal auf eigener Branch und als Draft-PR gesichert; keine Merge-/Runtime-/Releaseaktion. Nach Auflösung Blocker und aktuellen Parallelstand prüfen und denselben vollständigen Auftrag fortsetzen.
