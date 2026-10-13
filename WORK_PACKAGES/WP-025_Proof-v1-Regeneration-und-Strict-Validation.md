# WP-025 – Proof-v1-Regeneration und Strict-Validation

## ID

`WP-025`

**Bearbeitungsstatus:** **Phase B am 13.10.2026 separat ausdrücklich beauftragt und vollständig implementiert einschließlich Produktionsgoldens, Regressionstests und eigener Scopekorrekturen.** Tatsächliche Nachweise sind unten nach Lieferstand getrennt; die finale HEAD-/CI-/Abnahmebindung und die beiden vollständigen unabhängigen Berichte werden in [PR #19](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19) persistiert. Phase A ist abgeschlossen; ihre unten dokumentierten Autorisierungsgrenzen beschreiben den vorherigen Auftrag. Anforderungen, Paketdefinition, Manifest und Freigabegrenzen bleiben unverändert. Das Gesamtpaket bleibt bis den vorgeschriebenen Nachweisen/Abnahmen und separater Integration offen. Integration und Release sind nicht beauftragt.

[Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-025.production.scope.json)

## Ziel

Der bereits in [WP-017](./WP-017_Proof-v1-Regeneration-und-Strict-Validation.md) geplante fachliche Block wird auf der integrierten Level-v2-/Hash-Foundation (WP-023) und dem integrierten Solver-v2 (WP-024) ausführbar verankert. WP-025 ist dessen neuer historischer Auftrag nach ADR-030, keine neue fachliche Planung. WP-017 bleibt unveränderte Planungsquelle.

Der vollständige Sollzustand der späteren Implementierung bleibt: `proof-v1` ist ein vollständig regenerierbarer, byteidentisch gebundener `solver-v2`-Nachweis. Die gemeinsame `LevelValidationPipeline` prüft gespeicherte und frisch erzeugte Proofs, lehnt jede Divergenz mit `PRF-NONDETERMINISTIC` ab und behandelt Warnungen im Strict-Modus in Einzel- und Batchresultaten konsistent als Fehler.

**Dieser Auftrag führt ausschließlich Phase A bis zur geprüften Remote-Übergabe aus.** Ein verankertes Production-Manifest erlaubt noch keine Implementierung. Erst nach Governance-/Scope-/Trust-Nachweis folgt laut `WORK_QUEUE.md` ein ausdrücklicher separater Implementierungsauftrag.

## Voraussetzungen

1. Pflichtlektüre in der Reihenfolge aus [AGENTS.md](../AGENTS.md), danach [CURRENT_STATE.md](../PROJECT_CONTROL/CURRENT_STATE.md), [WORK_QUEUE.md](../PROJECT_CONTROL/WORK_QUEUE.md), [WORK_PACKAGE_RULES.md](../PROJECT_CONTROL/WORK_PACKAGE_RULES.md), [AI_HANDOVER_RULES.md](../PROJECT_CONTROL/AI_HANDOVER_RULES.md), [DEFINITION_OF_DONE.md](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), WP-017 und dieses Work Package.
2. Verbindliche Fachquellen: [ADR-007](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [ADR-021](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [ADR-013](../DECISIONS/ADR-013-zyklusfreie-ports-und-modulgrenzen.md), [ADR-026](../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md), [ADR-030](../DECISIONS/ADR-030-wp-scope-trust-anchor.md), [ADR-031](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md), [LEVEL_DATA_FORMAT.md](../ARCHITECTURE/LEVEL_DATA_FORMAT.md), [SOLVER_ARCHITECTURE.md](../ARCHITECTURE/SOLVER_ARCHITECTURE.md), [CONTENT_PIPELINE.md](../ARCHITECTURE/CONTENT_PIPELINE.md), [MODULE_BOUNDARIES.md](../ARCHITECTURE/MODULE_BOUNDARIES.md), [TEST_STRATEGY.md](../ARCHITECTURE/TEST_STRATEGY.md) und [Validator-README](../tools/architecture-validation/README.md).
3. WP-014/WP-018 und WP-021 bis WP-024 sind abgeschlossen und integriert. Ausgangsbasis ist der am 13.10.2026 tatsächlich verifizierte Remote-`main` bei `5f5127362b101dd4e066b68b21d3e35a5af314df`, einschließlich WP-024-Closeout PR #18. Die Zuordnung WP-015 → WP-023 und WP-016 → WP-024 ist unten mit Lieferverträgen belegt.
4. Frische Branch `codex/wp-025-proof-v1-strict-preparation` von exakt dieser Basis. WP-025 und sein eigenes Manifest werden erstmals gemeinsam im ersten Branchcommit als `A` eingeführt; dessen einziger Elterncommit und Manifest-`baseCommit` sind exakt diese Basis. Historischer Dateiname, H1, `## ID` und lokaler Manifestlink müssen dieselbe Kennung und denselben kanonischen Pfad binden. Das Manifest ist danach byteunveränderlich.
5. Vor Änderungen tatsächlichen `main`, Branches, Historie, offene PRs und relevante CI prüfen. Bei bereits laufender oder abgeschlossener gleicher Arbeit keinen zweiten Anchor erzeugen. WP-025 war in der vollständigen abgerufenen Historie unvergeben; kein entsprechender Remote-Branch oder offener PR lag vor.
6. Bestehende Workflowgates und Zugänge bleiben erhalten. Unity-Lizenzjobs teilen die globale Concurrency-Gruppe `unity-personal-seat` mit `cancel-in-progress: false`; GitHub kann zusätzliche wartende Jobs ersetzen. Eigene Push-/PR-Ereignisse deshalb zeitlich trennen und laufende fremde CI weder abbrechen noch neu starten. Laufende/abgebrochene Runs sind kein PASS.
7. Historische WP-008-/WP-009-/WP-013-Branches sind nur lesbare Vergleichs-/Negativkorpora, keine Lieferquellen. [Quellenkorrektur](../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md) gilt unverändert. `BLOCKER-PROD-001` bis `-003` bleiben offen und fail-closed.

## Scope

### Phase A – ausschließlich jetzt beauftragte Vorbereitung

1. Bestehenden WP-017-Auftrag vollständig gegen aktuelle Quellen und integrierte Lieferungen abgleichen; Voraussetzungen, Verbote, Akzeptanzkriterien, Tests und Übergabepunkte in diesem ausführbaren WP festhalten.
2. Gesamten Phase-A-/Phase-B-Dateiscope vor Verankerung bestimmen. Genau dieses neue WP und sein eigenes Production-Manifest gemeinsam im ersten Branchcommit verankern; noch keine fachlichen Änderungen im Anker.
3. Erst danach ausschließlich `CURRENT_STATE.md` und `WORK_QUEUE.md` mechanisch auf den erreichten Vorbereitungsstand nachführen und in `validate.yml` ausschließlich `STP_SCOPE_MANIFEST` auf WP-025 umstellen. Prioritäten, Workflowmechanik, Gates und Freigabegrenzen unverändert lassen.
4. Beide kanonischen Validatorläufe mit Self-/Negativtests, historische Bindung, Basis/Ancestry, unveränderte Manifestbytes, vollständigen Diff, Scope und Secret-/Whitespaceprüfung ausführen. Eigene Fehler innerhalb dieses Scopes beheben und betroffene Prüfungen wiederholen.
5. Eigene Branch normal committen/pushen und Draft-PR gegen `main` anlegen. Zugehörige CI selbst abrufen, konkrete Commit-/Prüfgegenstandbindung ausweisen, Remote-HEAD und vollständigen PR-Diff verifizieren. Ergebnisse, Dateifunktionen, Einschränkungen, Blocker und nächsten Schritt nach den Übergaberegeln persistieren.
6. Ende: definiert, gemeinsam verankert, Governance-/Scope-/Trust-geprüft und bereit für separaten Phase-B-Auftrag. Kein Produktions-, Test-, Schema-, Golden-, Architektur- oder Proofartefakt wird in Phase A geändert/erzeugt. Keine selbst simulierte unabhängige Prüfung und keine erneute fachliche WP-024-Abschluss-QC.

### Phase B – vollständiger bestehender Block, erst nach separatem Auftrag

1. `proof-v1.schema.json`, Proof-DTOs, strikten Parser, kanonischen Serializer, profilierten Hash und Bindung gemäß ADR-021/ADR-031 implementieren oder vervollständigen. `proofFormatVersion = 1`; Objektform und Projektion `STP-PROOF-JCS-1` bleiben erhalten. Alle vier Proofmetriken sind nichtnegative Integer mit `minimum: 0`.
2. Produktions-Proofgenerator in der bestehenden `STP.Editor.Content` implementieren. Aus dem unveränderten öffentlichen Puzzleinput mit dem integrierten `solver-v2` bis Lösungslimit zwei neu lösen; einzige gefundene Lösung über `STP-SOLUTION-JCS-1` mit Authoringlösung vergleichen. Nur ein gültiges `UNIQUE`-Ergebnis mit vorhandenen Metriken liefert den Nachweis. `Reductions` und interner Zustandsbudgetzähler sind keine Root-Spur/Metrikquelle. Solverversion, Traceform, vier Metriken und Tiebreaker werden nur konsumiert, nicht verändert.
3. Frischen Proof vor jeder weiteren Verwendung selbst gegen Schema, bekannte Profile, exakte Projektion und Hash prüfen. DTO/Parser/Serializer/Binding/Diagnosen liegen in `STP.Infrastructure.Content`, Solveraufruf und gemeinsame Pipeline in `STP.Editor.Content`. Keine direkte Solverreferenz aus Infrastructure; vorhandene Assemblykanten bleiben unverändert.
4. Gemeinsame `LevelValidationPipeline` Parse → Schema → Domain-Abbildung → Semantik → Solver/Proofregeneration auf Basis des vorhandenen `LevelV2Loader` vervollständigen. Keine zweite Puzzle-Regelautorität und kein toleranter Fallback. Bestehende unveränderliche Ergebnislisten, Parser-/JCS-Fehlerpfade und Stufenreihenfolge erhalten.
5. Gespeicherte und frisch erzeugte kanonische Proofbytes bytegenau vergleichen. Bei identischem Puzzlehash und `solver-v2` ergibt jede Abweichung `PRF-NONDETERMINISTIC`, auch wenn eine verfälschte Metrik mit neu berechnetem Selbsthash intern konsistent ist. Vollständige Artefakt-ID-, Format-, Puzzle-, Lösungs-, Profil-, Solverversions- und Hashbindung fail-closed prüfen; fehlende/unbekannte/stale/cross-puzzle Proofs mit stabilen `PRF-*`-Codes abweisen.
6. Strict-Semantik in jedem `LevelValidationResult`, `ValidateOne`, Batchreport, `Succeeded`, `FailedStage` und Fehler-/Warnzählern einheitlich umsetzen. `--strict` eskaliert jede Warnung mit unverändertem Diagnosecode zur Fehlerdiagnose; keine bloße Reportkorrektur. Non-strict-Verhalten und Reihenfolge deterministisch prüfen.
7. Kampagnengate aus WP-024 unverändert einbinden: normale Kampagne verlangt `requiredGuessDepth = 0`; 0/2+/`INDETERMINATE`, fehlende Metriken, Budget-/Cancellationabbrüche sind nie freigabefähig. Ohne fehlerfreie Proofregeneration kann kein Level Runtimekatalog oder Release-Lock erreichen. Dieser Block implementiert das Proof-/Strict-Gate, keine Katalog-, Release-Lock-, Generator- oder Contentproduktion; übrige bestehende Freigabe-/Cross-Reference-Gates werden nicht umgangen.
8. Neue `solver-v2`-Proofgoldens ausschließlich aus dem Produktionsgenerator erzeugen, dessen Ausgabe selbst schema-/hashvalidieren und im Test erneut generieren/bytegenau vergleichen. `solver-v1`-Prototypfixtures bleiben historische Lesefälle und dürfen nicht als aktuelle Produktionsgoldens referenziert werden. Keine frei erfundenen Metrikgoldens. Die unveränderten WP-023-JCS-/Legacy-Vertragsvektoren sind kein solver-v2-Regenerationsbeleg.
9. `LEVEL_DATA_FORMAT.md`, `CONTENT_PIPELINE.md`, `SOLVER_ARCHITECTURE.md` nur zur Einbindung der gelieferten Solver-v2-Regression sowie `TEST_STRATEGY.md` nur für die zugehörigen Proof-/Strict-Tests auf den tatsächlich gelieferten Stand bringen. Keine neue Architektur-/Produktentscheidung. Alle sieben ursprünglichen WP-017-Akzeptanzkriterien bleiben verbindlich.

## Betroffene Dateien/Module

Abschließende technische Außengrenze ist das [eigene unveränderliche Manifest](../tools/architecture-validation/scopes/WP-025.production.scope.json). Seine Production-Allowlist beschreibt den gesamten späteren Block; die engere Phase-A-Autorisierung gilt zusätzlich.

| Pfad / Modul | Zulässige Funktion und Phase |
|---|---|
| `WORK_PACKAGES/WP-025_Proof-v1-Regeneration-und-Strict-Validation.md` **neu** | Gemeinsame Definition im Anker; danach ausschließlich Nachweis-/Status-/Übergabefortschreibung. |
| `tools/architecture-validation/scopes/WP-025.production.scope.json` **neu** | Gemeinsam im Anker; danach keinerlei Byteänderung. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Mechanische Status-/Übergabenachführung nach dem Anker. |
| `.github/workflows/validate.yml` | Ausschließlich vorhandene Manifestzuordnung auf WP-025; keine Workflowmechanik oder Gateänderung. |
| `ARCHITECTURE/schemas/proof-v1.schema.json` | Nur Phase B: bestehender Proofvertrag, vier nichtnegative Metriken nach ADR-031. |
| `ARCHITECTURE/examples/proof-v1*.json` (neue solver-v2-Beispiele bei Bedarf) | Nur Phase B: aus tatsächlicher Produktionsgenerierung; bestehende historische Contractfixtures nicht als Production ausgeben. |
| `ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/CONTENT_PIPELINE.md` | Nur Phase B: tatsächlich gelieferter Proof-/Strict-/Pipelinestand. |
| `ARCHITECTURE/SOLVER_ARCHITECTURE.md` | Nur Phase B: Einbindung der bereits gelieferten solver-v2-Regression. |
| `ARCHITECTURE/TEST_STRATEGY.md` | Nur Phase B: Proof-/Strict-Tests entsprechend WP-017 Scope 9. |
| `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/*.cs` und `*.cs.meta` | Nur Phase B: bestehende Assembly, Proof-DTO/Parser/Serializer/Binding/Diagnose-/Resultatmodell; nötige neue Dateien explizit erlaubt. |
| `Assets/StammstreckenPuzzle/Scripts/Editor/Content/*.cs` und `*.cs.meta` | Nur Phase B: bestehende Assembly, Produktions-Proofgenerator und gemeinsame Pipeline; nötige neue Dateien explizit erlaubt. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Content/*.cs`, `*.cs.meta`, `*.json`, `*.json.meta` | Nur Phase B: Proof-/Strict-/Pipeline-Contracttests und generierte Golden-/Negativdaten; nötige neue Dateien explizit erlaubt. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/*.cs`, `*.cs.meta`, `*.json`, `*.json.meta` | Nur Phase B und ausschließlich soweit notwendige generierte Proofgoldens dies verlangen; keine Neuprüfung/-änderung der WP-024-Solverimplementierung. |

Die bestehenden Content-/Editor-/Contenttest-Assemblies und ihre Kanten sind vorhanden; die Content-Testassembly referenziert bereits `STP.Editor.Content` und den Solver. Content und Solver sind schon in Unity-EditMode-/Coverage-Gates erfasst. Keine Änderung von `.asmdef`, `csc.rsp`, `.github/workflows/unity.yml` oder Abhängigkeiten erforderlich/freigegeben. Die Dateimuster sind flach und erlauben keine beliebigen Unterverzeichnisse. Ein später entdeckter Bedarf außerhalb des Manifests ist ein Blocker, keine nachträgliche Erweiterungsbefugnis.

Nur lesend: Pflichtquellen, WP-023-/WP-024-Lieferverträge und Abschlussnachweise, bestehende Module/Tests, Schemas/Contractfixtures, Workflows und Validator. Bestehende Work Packages, ADRs, Projektregeln und historische Manifeste werden nicht geändert.

## Ausdrücklich nicht erlaubte Änderungen

- Kein Phase-B-Code, Schema-/Goldenwechsel, Proofgenerierung oder Contentproduktion im jetzigen Auftrag. Keine Integration, kein Merge, Rebase, Force-Push oder Release.
- Keine nachträgliche Manifeständerung; keine Umdeutung aktueller WP-Fortschreibung zu historischer Trust-Evidenz.
- Keine Änderung von AGENTS.md, Projektregeln, bestehenden Paketdefinitionen, ADRs, Architektur, Abhängigkeiten, Prüfpflichten, Prioritäten oder Freigabegrenzen.
- Keine Änderung der ADR-031-Metriksemantik, Solver-/Traceversion, Prämissenwahl, Constraintsemantik, MRV-/Tiebreaker-/Wertreihenfolge, Lösungslimit oder Budget-/Cancellationsemantik; kein Solver-Produktionscode im Scope.
- Keine Änderung von Puzzle-ID oder semantischem Puzzlehashvertrag, Kampagnenstruktur, Save-Migration, Release-Lock-Vertrag/Produktion, Generator, UI, Assets, Privacy oder Monetarisierung.
- Keine Production-Season-1-Level, Zeitkalibrierung oder automatische Qualitätsfreigabe. Produktfolgeblocker bleiben offen.
- Kein Akzeptieren nicht regenerierbarer oder nur selbstgehashter Proofs; keine Herabstufung von `PRF-NONDETERMINISTIC` oder Strict-Fehlern.
- Keine fremden Manifeste, historischen Codeübernahmen, Tests/Gates bloß für Grün abschwächen oder nicht ausgeführte Prüfungen als PASS darstellen.
- Keine zusätzliche fachliche WP-024-Abschlussprüfung; keine eigene Simulation der unabhängigen Abschluss-QC.

## Akzeptanzkriterien

`AK-A01` bis `AK-A05` definieren den Vorbereitungsübergabepunkt. `AK-B01` bis `AK-B07` sind unverändert die sieben fachlichen Kriterien der WP-017-Planung und werden erst nach separatem Phase-B-Auftrag geprüft.

| ID | Prüfkriterium |
|---|---|
| `AK-A01` | Vollständiger WP-017-Scope, Voraussetzungenzuordnung, Verbote, sieben fachliche Kriterien, Prüfpflichten und zwei unabhängige Abschlussrollen sind quellengebunden dokumentiert; WP-017 und Vorgängerdefinitionen unverändert. |
| `AK-A02` | Neuer gemeinsamer Erst-Add von exakt WP-025 und eigenem Manifest im ersten Branchcommit; einziger Elterncommit = tatsächliche Main-Basis = Manifest-`baseCommit`; historische IDs/Link korrekt; Manifestbytes unverändert und remote nachweisbar. |
| `AK-A03` | Vollständiger Basis→HEAD-Diff einschließlich untracked/Rename-/Copy-Endpunkten innerhalb des unveränderlichen Manifests und ausschließlich Phase A; Statusnachführung und alleinige CI-Manifestumschaltung konsistent. |
| `AK-A04` | Architecture-only- und WP-025-Production-Scope-Lauf samt Self-/Negativtests nach bestehendem Verfahren bestanden; Trust-/Ancestry-/Secret-/Diffnachweise geprüft. Echte Ubuntu-PR-CI selbst abgerufen und konkret zugeordnet; keine übernommene PASS-Aussage. |
| `AK-A05` | Normale Commits/Pushes, Draft-PR gegen main, tatsächlicher finaler Remote-HEAD und vollständiger PR-Diff verifiziert; Ergebnis/Dateien/Tests/Evidenzgrenzen/Blocker/nächster separater Auftrag persistent. Phase B bleibt unbeauftragt, WP-025 offen. |
| `AK-B01` | Rein deduktiver `solver-v2`-Proof mit `searchNodes = 0` schema- und bindungsgültig; alle vier Schema-Minima 0, Proofformat weiterhin 1. |
| `AK-B02` | Alle solver-v2-Prooffixtures stammen aus dem Produktionsgenerator; Deserialisierung, Selbsthash und erneute Generierung ergeben byteidentische Ergebnisse. |
| `AK-B03` | Geänderte Metrik plus neu berechneter Selbsthash schlägt dennoch mit `PRF-NONDETERMINISTIC` fehl, wenn gespeicherter und frischer Proof abweichen. |
| `AK-B04` | Cross-Puzzle-Copy, stale Lösungshash, falsche Artefakt-ID, falsches Hashprofil, unbekannte Solverversion, falscher Solverversionseintrag und fehlender Proof schlagen fail-closed mit stabilen `PRF-*`-Codes fehl. |
| `AK-B05` | `--strict` eskaliert jede betroffene Warnung innerhalb jedes `LevelValidationResult` mit unverändertem Code; Einzel-/Batchreport, `Succeeded`, `FailedStage` und Zähler melden denselben Status. |
| `AK-B06` | Ohne fehlerfreie Proofregeneration kein Zugang zu Runtimekatalog oder Release-Lock; unverändertes Kampagnengate und übrige Freigabegrenzen bleiben wirksam. |
| `AK-B07` | Pipeline deterministisch bezüglich Input-, Diagnose- und Batchreihenfolge sowie Proofbytes. |

## Tests

### Phase A

1. Vollständiger lokaler Architecture-only-Lauf und eigener Production-Scope-Lauf mit Self-/Negativtests; maßgebliche gleichartige Ubuntu-CI mit CPython 3.11.13/Node 22.20.0 gemäß bestehendem Workflow. Die README verlangt lokal Linux/macOS, CPython 3.11.x und Node 22.x. Windows-/abweichende Runtime-Läufe sind nur Zusatzbefunde, kein Ersatz oder Grund zur Tooländerung.
2. Historischen Erst-Add, einen Elterncommit, Base-/Anchor-Ancestry, Manifest-Schema/Hash/Bytegleichheit (Anker, Index/HEAD, Working Tree und Remote) sowie historische WP-ID-/Linkbindung prüfen. Bestehende Selftests für separate Adds, spätere Links, falsche IDs/Pfade, Mutation, Scope-Escape, Segmentglobs und künftigen legitimen Production-Anker erhalten.
3. Vollständigen Diff gegen `5f5127362b101dd4e066b68b21d3e35a5af314df` mit `git diff --check` und Allowlist/Phasengrenze prüfen; Secret-Mustersuche. Workflowdiff darf nur die vorhandene Manifestzeile ändern. Vorgänger-WPs, Produktions-/Test-/Architektur-/Schema-/Golden-/Unity-CI-Dateien müssen bytegleich zur Basis bleiben.
4. Eigene Architecture-PR-CI einschließlich beider Selftestläufe und des bereits bestehenden Node-JCS-Crosschecks selbst abrufen. Zusätzliche Unityläufe nach tatsächlichem Zustand separat berichten: Phase A ändert keinen Unity-relevanten Inhalt und verlangt keinen neuen fachlichen Unity-/Proofnachweis (bestehender Phase-A-Weg WP-023/WP-024). Laufende, abgebrochene oder übersprungene Jobs sind kein vollständiger PASS.
5. Draft-PR/Base/Remote-HEAD und gesamten Remote-PR-Diff abschließend gegen lokalen HEAD/Basisdiff prüfen; unabhängige Phase-B-QC wird hier nicht ausgeführt.

Kanonische Befehle (aus Repositoryroot; Pythonpfad gemäß lokaler Prüfumgebung):

```text
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-025.production.scope.json --self-test
git diff --check 5f5127362b101dd4e066b68b21d3e35a5af314df --
```

### Phase B nach separatem Auftrag

- Vollständiger Architekturvalidator mit WP-025-Production-Scope und Self-/Negativtests sowie gesamter Basisdiff/Trust-/Secretnachweis.
- Unity-Compile und sämtliche Content-/Solver-EditMode-Tests im zugelassenen Runner; bestehende Domain-/Bootstrap-, Coverage-, PlayMode-/QA-, Android-IL2CPP-/iOS-Gates gemäß unveränderten Projektverträgen erhalten. Lokaler Harness ist nur zusätzliche Evidenz; WP-023-NUnit-/Nullable-Unterschiede beachten.
- Positiv-/Negativtests für jedes `AK-B01` bis `AK-B07`: Nullfall, generierte Goldens, erneute bytegenaue Regeneration, Selbsthash-aber-falsch, alle Bindungs-/Versionsfehler, Strict-/Non-strict-Einzelresultat und Batchreport, fehlender/ungültiger Proof und deterministische Reihenfolgen.
- Unabhängiger Node-JCS-Crosscheck für die **tatsächliche neue Proofprojektion**; bestehender WP-023-Legacy-Hashcheck allein genügt nicht. Kein Workflowumbau durch dieses WP; Zusatzprüfung mit konkretem Befehl, Input/Output/Commit persistent nachweisen.
- Zwei unabhängige Abschlussprüfungen nach erfolgreicher CI auf demselben eingefrorenen finalen PR-HEAD: Proofbindung/Versionierung/Cache-/Regeneration/Fehlercodes und Strict-/Tests-/Scope-/CI-/Integrationsreife. Ergebnisse nicht vor Abgabe gegenseitig austauschen.

## Risikoklasse

**Hoch.** Proofidentität, Kampagnengates und CI-Freigabe werden verbunden. Fehlerhafte Bindung kann falsche Eindeutigkeit, Schwierigkeit oder nicht reproduzierbare Release-Locks zulassen. Phase A führt ausschließlich die zugehörige Governance-Vorbereitung aus.

## Definition of Done

Die [projektweite DoD](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) gilt vollständig. **Vorbereitung abgeschlossen ist weder Implementierung freigegeben noch Work Package abgeschlossen.** Phase A endet nur nach Erfüllung von `AK-A01` bis `AK-A05` mit persistierten tatsächlichen lokalen/CI-/Remote-Nachweisen und offen ausgewiesenen späteren Tests.

Das gesamte WP-025 ist erst nach vollständig beauftragter/gelieferter Phase B abgeschlossen: alle `AK-B*` mit konkretem Test und CI-Evidenz belegt; aktuelle Goldens aus Produktionsregeneration; solver-v1-Prototypfixtures nicht als aktuelle Goldens verwendet; beide unabhängigen Abschlussberichte ohne offene Blocker/High verlinkt; Fachdokumentation, WP, `CURRENT_STATE.md` und `WORK_QUEUE.md` nachgeführt. Integration setzt diese Nachweise und die ausdrückliche Geschäftsführungsentscheidung voraus und gehört nicht zum aktuellen Auftrag. Contentproduktion bleibt bis zur vollständig integrierten Pipeline gesperrt.

## Rollen und Übergabe

- Geeigneter Implementierungsagent erst nach separatem Auftrag; keinerlei Architektur-, Metrik- oder Scopeentscheidung.
- WP-017 und ADR-031 verlangen ausdrücklich zwei unabhängige Abschlussrollen. Die aktuelle Queue macht die Systeme austauschbar, hebt diese konkrete Prüfpflicht aber nicht auf: getrennte Proof-/Vertrags-QC (historisch „Astra“) und Strict-/Test-/Scope-/CI-QC (historisch „Sol“) auf demselben eingefrorenen HEAD. Eigene Implementierungstests ersetzen keine davon.
- Phase A führt keine dieser Abschlussprüfungen selbst durch. Jeder Schritt dokumentiert Status, Ergebnis, Dateien, Tests, Einschränkungen/Blocker und nächsten Auftrag gemäß `AI_HANDOVER_RULES.md`.

## Herkunft / Abgleich der WP-017-Planung

| Planungs-/Vertragsaussage | Aktuell belegte Zuordnung / Geltung |
|---|---|
| WP-015 als Level-/Hash-Voraussetzung | Tatsächlich ausgeführt durch [WP-023](./WP-023_Level-v2-Foundation-und-Hashvertraege.md): unabhängiger PASS auf `f56eab34dca0346d0e339a1c5cde650e15955f68`, integriert über PR #15 als `10405370967496f51be9e5c00c381fd8052ec072`, anschließend formal geschlossen. Parser/immutable DTO, Domainabbildung, JCS, Content-/Public-/Lösungs-/Proofprojektionshashes, diagnostischer fail-closed Hashpfad und neutrale v1→v2-Migration sind geliefert. Kein Proofartefakt; `AwaitingProofGate`, `IsRuntimeImportable = false`. |
| WP-016 als Solver-v2-Voraussetzung | Tatsächlich ausgeführt durch [WP-024](./WP-024_Solver-v2-Metriken-und-Deduktionsspur.md): finaler unabhängiger PASS auf `64d8801ee28d68dc0c8ec9800898b7862977c32c`, Bericht/Freigabe in [PR #17, Review 5502357045](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/17#pullrequestreview-5502357045), integriert als `22ff74558259c7ba1f9a157280e4d70b909bac26`, Closeout PR #18 auf Ausgangsbasis. Rootspur/Metriken nur bei UNIQUE, unveränderliche hinreichende/irredundante Prämissen, letzter Budgetcheck nach gesamter Ergebniskonstruktion und fail-closed CampaignGate geliefert. Kein Proofschema/-artefakt/Strict. |
| Historische Voraussetzungen WP-014/WP-018/WP-021/WP-022 | Laut aktueller Steuerung abgeschlossen/integrationswirksam; integrierte Basis bleibt maßgeblich, keine Wiederholung ihrer Abschluss-QC. |
| WP-017 + Manifest als neuer Erstanker | WP-017 existiert vorab auf main und erfüllt den Erst-Add-Vertrag nicht. Neue unvergebene ID WP-025 und eigenes Manifest binden denselben vollständigen Block auf tatsächlichem main. WP-017 unverändert. |
| WP-017 Scope 2–8 und sieben AK | Vollständig in Phase B / `AK-B01` bis `AK-B07` erhalten; keine Verkleinerung, Erweiterung, Umpriorisierung oder Verschmelzung. |
| WP-017 Scope 9: Datenformat-/Pipeline-/Architektur-/Testdokumentation | Zugehörige Fachdateien plus `TEST_STRATEGY.md` explizit im vollständigen Manifest; diese Testdatei ist durch Scope 9 und Testvertrag bereits verlangt, keine neue fachliche Aufgabe. |
| Rollen Kimi/Astra/Sol | Systeme austauschbar; Implementierung separat, **zwei** unabhängige Abschlussrollen wegen konkreter WP-017-/ADR-031-Pflicht beibehalten. WP-024s abweichende Zahl eigener Reviews wird nicht auf diesen Block übertragen. |
| Ältere Schema-Minima/solver-v1-Beispiele | Von ADR-031 ausdrücklich erwartete Übergangsform; erst Phase B ändert Proofschema/aktuelle Goldens. Kein ungelöster Widerspruch und kein Root-only-Proof-PASS in Phase A. |
| Pipelinevertrag enthält noch WP-023-Zwischenstand | Tatsächlicher später integrierter Solverstand steht in `SOLVER_ARCHITECTURE.md` Abschnitt 13 und WP-024; gemeinsame Proofpipeline weiterhin unimplementiert. Phase B führt allein den zugehörigen Lieferstand nach. |

WP-023-/WP-024-Abschlüsse sind Voraussetzungenbelege, keine neu ausgeführte fachliche QC. Physische Geräte-/SDK-/Storeevidenz und Produktfolgeblocker behalten ihre dokumentierten Grenzen. Ein tatsächlich ungelöster Quellen- oder Scopewiderspruch stoppt den betroffenen Teil und wird persistent dokumentiert.

## Phase-A-Nachweis – 2026-10-13

### Ausgangsbasis, Lieferverträge und gemeinsame Verankerung

- Tatsächlicher Remote-main nach vollständigem Fetch: `5f5127362b101dd4e066b68b21d3e35a5af314df`. Lokaler alter Checkout war WP-022; dessen Branch/Prüfstand wurde nicht verändert. Neue Branch direkt von origin/main; der lokale main-Verweis wurde anschließend ausschließlich per normalem Fast-forward-Fetch synchronisiert (keine Integration oder Historienumschreibung).
- Remote-Branches und offene PRs vor Änderungen abgefragt: kein paralleler Proofauftrag/offener PR; vollständige abgerufene Historie ohne vergebenes WP-025 oder WP-025-Manifest. Keine neue fachliche WP-024-Abschluss-QC.
- Liefermetadaten selbst gelesen: PR #15 mit geprüftem HEAD `f56eab34…` als `10405370…` integriert; PR #17 mit geprüftem HEAD `64d8801…` als `22ff7455…` integriert. WP-023-/WP-024-Abschlussdokumentation und WP-024-Review 5502357045 gelesen; dortige Prüferexperimente nicht erneut ausgeführt und nicht als eigene Evidenz ausgegeben.
- Gemeinsamer historischer Erstanker **`de314dbd6e928d4ffb8893fe4f1ee7e6c7873132`**, exakt zwei `A`-Dateien: dieses neue WP und eigenes Manifest. Einziger Elterncommit = Manifestbasis = `5f5127362b101dd4e066b68b21d3e35a5af314df`. Historischer Markdownlink war bereits im Anker vorhanden. Kein alter Planungs-WP wurde geändert.
- Manifest-SHA-256 **`1a30063cf00771e053b39b6ee37405ba25d7310600ad8bb158ac8055194ed44d`**. Historischer Gitblob und Working-Tree-Bytes direkt verglichen: identisch. Nach dem Anker keine Manifeständerung.
- Mechanischer Folgediff: `CURRENT_STATE.md` und `WORK_QUEUE.md` nennen WP-025-Vorbereitungsphase und separate Phase B; `validate.yml` ändert allein `STP_SCOPE_MANIFEST` von WP-024 auf WP-025. Kein Produkt-, Test-, Schema-, Golden-, Architektur-, Unity-CI- oder Validatorcode geändert.

### Lokale Prüfungen und noch ausstehende Remote-Evidenz

| Prüfgegenstand | Tatsächlicher Stand |
|---|---|
| Sauberer Anker `de314db…` – Architecture-only | Linux/WSL, CPython 3.11.13, Node 22.20.0, exakt die sechs unveränderten Lockpakete; kanonischer `--self-test`-Lauf Exit 0, `RESULT PASS`, **17 Prüfgruppen**. |
| Sauberer Anker `de314db…` – eigener Production-Scope | Gleiche Umgebung; kanonischer WP-025-Lauf inklusive Self-/Negativtests Exit 0, `RESULT PASS`, **18 Prüfgruppen**, `worktreeDirty=false`, konkrete Base/HEAD ausgegeben. Historische gemeinsame ID-/Link-/Add-Bindung, Segmentglobs, Manifestmutation/-Escape und späterer legitimer Production-Anker in bestehender Selftest-Suite enthalten. |
| Vollständiger Folgediff / bestehender Node-Goldencheck | Beide kanonischen Linuxläufe auf dem vollständigen Folgediff bei HEAD `de314db…`, `worktreeDirty=true`: Exit 0, 17/18 Gruppen PASS. Unveränderter Workflow-Crosscheck lokal mit Node 22.20.0: 12 Kanonisierungsfälle, 32 Ablehnungen, 2 v2-/2 v1-Dokumentvektoren PASS. Scratch-Extraktion zunächst mit falschem Pythonpfad gescheitert; Hilfsskript korrigiert und dieser Crosscheck erfolgreich erneut ausgeführt. Repositorytool/Tests unverändert. |
| Baseline-main Architecture CI | Run [38387795432](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38387795432), `push`, HEAD `5f5127362b101dd4e066b68b21d3e35a5af314df`, selbst abgefragter Status **completed/success**. Baselinebeleg, keine eigene WP-025-PR-CI. |
| Zusätzliche Baseline-main Unity CI | Run [38387795108](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38387795108), derselbe main-HEAD. Anfangs `in_progress`, iOS pending; vor eigener Remoteaktion frisch als **completed/success, alle zehn Jobs success** verifiziert, einschließlich iOS-Job `115278887464`. Der Run wurde weder abgebrochen noch neu gestartet. Diese Zusatz-CI ist auf main gebunden, kein eigener Proofnachweis. |
| Eigene Remote-/Draft-PR-CI | Vorbereitungscommit `fb9ddb5880b0619e37c7464234ad8207eb0ea309`: Architecture Push **38395357162**, Job **115289054494**, und PR **38395554524**, Job **115289734763**, beide completed/success. Beide vollständigen Logs selbst gelesen: 17/18 Gruppen mit Self-/Negativtests und bestehender Node-Crosscheck PASS. PR-Architecture-only/Node auf synthetischem Merge `854e845136a83f57b97d2095b26bba1a779c6ecb`; Scope nach echtem Head-Checkout auf `fb9ddb…`, `worktreeDirty=false`, Basis 5f51273. |

Die Prüfumgebung wurde isoliert außerhalb versionierter Projektdateien eingerichtet; keine Abhängigkeit oder Toollogik des Repositorys geändert. Eigene lokale Linuxausgaben liegen zusätzlich in ignorierten Scratchlogs; die maßgeblichen Befehle, Versionen und Ergebnisse sind hier persistent, eigene CI wird mit Run-/Job-/Commitbezug ergänzt.

### Belegkategorien und Übergabegrenze

| Kategorie | Stand / Grenze |
|---|---|
| `LOCAL_DOCUMENT_STRUCTURE` | Auf Anker und vollständigem Vorbereitungsfolgediff ausgeführte Inventar-/WP-/Link-/ADR-/Statusprüfungen PASS; eigene Push-/PR-Architecture-CI zusätzlich PASS auf fb9ddb. |
| `LOCAL_ARCHITECTURE_SEMANTICS` | Auf Anker und Folgediff ausgeführte Schema-/Hash-/Fixture-/Reducer-/Self-/Negativprüfungen PASS; bestehender Node-Goldencheck PASS; kein neuer C#-Proofbeleg. |
| `MANUAL_ARCHITECTURE_REVIEW` | Quellenhierarchie, Lieferzuordnung, vollständiger geplanter Scope, Modulgrenzen und zwei Abschlussrollen abgeglichen; kein automatisches PASS für reine Textsemantik. |
| `LOCAL_SCOPE` | Historisch gebundener vollständiger Basisdiff inklusive Folgestand PASS; genau fünf erlaubte Phase-A-Dateien, alleinige Manifestzeile im Workflow, `git diff --check` sauber, keine untracked Scopepfade; vollständiger Remote-PR-Diff exakt gleich lokalem Basisdiff (50.877 Zeichen auf fb9ddb). |
| `CONTRACT_ONLY` | Proof-v1-/Strict-/Generator-/Import-/Releaseverträge weiterhin ohne neue Implementierung. |
| `REQUIRED_LATER/NOT_EXECUTED` | AK-B01–B07, Produktionsproof, zugehörige Unity-/Node-Proofprüfungen und beide unabhängigen Abschluss-QCs erst Phase B; Geräte-/SDK-/Storetests bleiben später. |
| `BLOCKED` | Produktfolgeblocker 001–003 unverändert. Kein offener Phase-A-Definitions-/Scope-/Trust-/CI-Blocker. Spätere Implementierung und ihre Nachweise ausdrücklich unbeauftragt. |

Letzter abgeschlossener Schritt: vollständige Phase-A-Vorbereitung einschließlich eigener PR-CI und Remote-Diffprüfung. Nächster zulässiger Schritt: **separater ausdrücklicher Implementierungsauftrag für Phase B von WP-025 auf derselben verankerten Branch**. Keine Implementierungsfreigabe durch diese Eintragung.


### Phase-A-Übergabe und finale Nachweisbindung

**Ergebnis: Vorbereitung abgeschlossen; WP-025 als Gesamtpaket offen.** Branch `codex/wp-025-proof-v1-strict-preparation`, [Draft-PR #19](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19) gegen main. Der geprüfte Vorbereitungs-HEAD ist `fb9ddb5880b0619e37c7464234ad8207eb0ea309`; die anschließende Übergabefortschreibung ändert ausschließlich dieses WP, `CURRENT_STATE.md` und `WORK_QUEUE.md`. Anchor, Manifest, Manifestzuordnung und alle fachlichen Dateien bleiben identisch. Der finale Übergabe-HEAD ist der letzte normale Dokumentationscommit der Branch; seine vollständige SHA und eigene nachgelagerte CI werden nach Push in der **Beschreibung von PR #19** konkret persistiert. Diese CI wird selbst abgerufen und nicht aus dem vorherigen PASS abgeleitet. Die finale Remote-/Diff-/Manifestprüfung folgt ebenfalls auf diesem tatsächlichen HEAD, keine Prüfung nur der zuletzt nachgeführten Dateien.

| Phase-A-Kriterium | Ergebnis / Nachweis |
|---|---|
| AK-A01 | Erfüllt: vollständige unveränderte WP-017-Absicht, aktuelle Lieferzuordnung, Grenzen und beide unabhängigen späteren Abschlussrollen dokumentiert; bestehende Paketdefinitionen/ADRs unverändert. |
| AK-A02 | Erfüllt: gemeinsamer Erst-Add de314db, genau ein Elterncommit 5f51273; historische IDs/Link, Schema, Base-/Anchor-Ancestry und unveränderte Manifestbytes mit direkter Gitblobprüfung lokal/HEAD/Remote belegt. |
| AK-A03 | Erfüllt: gesamter Basisdiff genau fünf Phase-A-Dateien; Workflowblob exakt Baseline mit einzigem Ersatz WP-024→WP-025-Manifest; keine untracked Scopepfade, keine fremden Produktions-/Schema-/Test-/Golden-/Architekturänderungen. |
| AK-A04 | Erfüllt: lokale Linux-Selftestläufe 17/18 PASS, voller Scope-/Secret-/Whitespace-/Trustnachweis; eigene Ubuntu-Push-/PR-CI auf fb9ddb mit gelesenen Logs PASS. Finaler Dokumentationsnachfolger erhält separate, am PR persistierte Nachweise. |
| AK-A05 | Erfüllt: normale eigene Branchcommits/Push, Draft-PR #19, vollständiger PR-Diff auf fb9ddb exakt lokal, HEAD/Base frisch gelesen. Finale Dokumentations-HEAD-/CI-/Diffbindung wird in PR #19 festgehalten; Übergabe und separater nächster Auftrag hier persistent. |

Geänderte Dateien und Funktionen: neues WP (vollständiger Auftrag/Quellenabgleich/Evidenz/Übergabe), neues Manifest (immutable vollständige Außengrenze), `CURRENT_STATE.md` (Phasen-/Ergebnis-/Blocker-/Nachweisstand), `WORK_QUEUE.md` (mechanischer Stand bei unveränderter Priorität 5 und Startgates), `validate.yml` (ausschließlich vorhandene Manifestzuordnung).

Die zusätzliche eigene Unity-CI auf fb9ddb ist **kein Phase-A-Abnahmekriterium**: Push-Run `38395357226` und PR-Run `38395554553` waren bei dieser Eintragung noch nicht vollständig abgeschlossen. project-preflight/config/evidence-guard erfolgreich; Push-EditMode aktiv, PR-EditMode im gemeinsamen Lizenzslot wartend; übrige Umgebungs-/Folgejobs nach tatsächlichem Zustand. Keine vollständige PASS-Aussage. Eigene Push-/PR-Ereignisse wurden zeitlich getrennt und erst nach SUCCESS des bestehenden main-Unity-Runs ausgelöst; keine laufende fremde CI abgebrochen oder neu gestartet. Ein weiterer normaler Dokumentationspush löst nach unverändertem Workflow eigene Runs aus; deren tatsächliche Zustände stehen separat am PR. Natürliches Scheduling/Concurrency-CANCELLED wäre kein bestandener Test und wird nicht umetikettiert. Es erfolgt kein vorsorglicher CI-Abbruch oder Neustart.

Keine offene Phase-A-Korrektur und kein neuer Produkt-/Architekturentscheid nötig. Die kurze anfänglich fehlgeschlagene Node-Scratch-Extraktion ist behoben; kein Repositoryvalidator oder Gate wurde geändert. Die bewusst späteren Prüfungen bleiben AK-B01–B07, tatsächliche Produktions-Proofregeneration/Node-Proofprojektion, Unity-Content-/Solvernachweise und zwei unabhängige Abschluss-QCs. Integration und Contentproduktion bleiben hinter ihren separaten Gates; die drei Produktfolgeblocker und Geräte-/SDK-/Storeeinschränkungen bleiben unverändert.

## Phase B – separat beauftragte Implementierung, 13.10.2026

Der neue ausdrückliche Auftrag erlaubt den vollständigen oben definierten Phase-B-Scope einschließlich eigener Korrekturen, vorgeschriebener Prüfungen, normaler Commits/Pushes und Dokumentation auf der bestehenden Branch und PR #19. Die Phase-A-Beschränkungen oben sind die Historie des früheren Auftrags; keine Anforderung oder Paketdefinition wurde geändert. Integration, Rebase, Force-Push, Release, Folgepakete und Contentproduktion bleiben ausgeschlossen.

### Einstieg und tatsächlicher Lieferumfang

Vor Änderungen: vollständige Pflichtlektüre/Quellenhierarchie, sauberer Working Tree, lokaler/Remote-/PR-HEAD `5e4b986bf29a3223e96b5993f37a99882581dab2`, main/PR-/Manifestbasis `5f5127362b101dd4e066b68b21d3e35a5af314df`, kein zweiter offener Proof-PR. Der bestehende Unity-PR-Lauf `38396165644` wurde weiterlaufen gelassen. Beginn in diesem WP, CURRENT_STATE und WORK_QUEUE vor fachlichen Änderungen persistiert. Anker und Manifest-SHA-256 `1a30063cf00771e053b39b6ee37405ba25d7310600ad8bb158ac8055194ed44d` unverändert.

| Dateien | Tatsächliche Funktion |
|---|---|
| `Scripts/Infrastructure/Content/ProofV1.cs` (+ Meta) | Immutable Proof-/Metrik-DTOs, strikter Parser/geschlossene Schemalesung, exakte Projektion, kanonische Bytes, Selbsthash, bekannte Profile/Versionen und vollständige Level-/Referenz-/Artefaktbindung mit stabilen PRF-Codes. |
| `Scripts/Infrastructure/Content/LevelValidationResult.cs` (+ Meta), `LevelDiagnostic.cs` | Immutable Einzel-/Batchmodelle, Error/Warning-Severity, Strict-Promotion vor Status/Stufe/Zählern, spätere Import-/Releasegates geschlossen. |
| `Scripts/Infrastructure/Content/ContentEditorAccess.cs` (+ Meta) | Nur die bereits referenzierende Editorassembly darf intern Pipelineergebnisse konstruieren; keine neue Assemblyreferenz. |
| `Scripts/Editor/Content/ProofGenerator.cs` (+ Meta) | Echte integrierte solver-v2-Regeneration aus öffentlichem Input, einziges Ergebnis und kanonischen Lösungshash vergleichen, unveränderte vier Metrics konsumieren, vollständige frische Ausgabe vor Verwendung selbst prüfen. |
| `Scripts/Editor/Content/LevelValidationPipeline.cs`, `LevelValidationCommand.cs` (+ Metas) | Gemeinsamer stufenweiser Einzel-/Batchweg; normalisierte/sortierte Quellen, doppelte Quellen abweisen, CampaignGate unverändert, gespeicherte/frische Proofbytes vergleichen; headless `--strict`-Adapter ohne Import-/Dateisystem-Nebenwirkung. |
| `ARCHITECTURE/schemas/proof-v1.schema.json` | Ausschließlich die drei bisher positiven Metrikminima auf 0; alle vier nichtnegative Integer, Proofformat und Objektform unverändert. |
| `LevelV1ToV2Migrator.cs`, `LevelMigrationTests.cs` | An das ausdrücklich gelieferte Schema angepasste nichtnegative Metrikgrenze; aufgezeichnete Zero-v1-Fakten neutral erhalten, kein aktuelles Proofartefakt durch Migration. |
| `Tests/EditMode/Content/ProofV1Tests.cs` (+ Meta), Ergänzung `SchemaParityTests.cs` | Produktionsgoldens, echte Byte-Regeneration, selbstgehashte Divergenz, vollständige Bindung, Parser/Schema-/Versions-/Budget-/Kampagnenfehler, Strict/Non-strict jeder Stufe, Einzel-/Batchzähler, Ordnung und Unveränderlichkeit. |
| `Tests/EditMode/Content/proof-v1-solver-v2-{root,single,search}.json` und jeweils `.level.json` (+ Metas) | Drei ausschließlich vom Produktionsgenerator erzeugte FIXTURE_ONLY-Goldens mit gebundener Levelquelle. Keine Season-1-Produktion und keine erfundenen Metriken. |
| `LEVEL_DATA_FORMAT.md`, `CONTENT_PIPELINE.md`, `SOLVER_ARCHITECTURE.md`, `TEST_STRATEGY.md` | Tatsächlich gelieferten Proof-/Strict-/Regressionsstand und übrige unveränderte Freigabegrenzen dokumentiert. |
| Dieses WP, `CURRENT_STATE.md`, `WORK_QUEUE.md` | Separaten Auftrag, Ergebnis, tatsächliche Nachweise, Korrekturen und nächste Freigabepunkte dokumentiert. |

Alle `Scripts/...`-/`Tests/...`-Pfade in der Tabelle sind unter `Assets/StammstreckenPuzzle/`. Solver-Produktionscode, asmdefs, csc.rsp, Unityworkflow, Validator/Abhängigkeiten, historische Beispiele/Hashvektoren, ADRs, Projektregeln, andere WPs und sämtliche Manifeste unverändert. Der vollständige Basisdiff enthält weiterhin den historischen Erstanker und die alleinige Manifestzuordnung in validate.yml aus Phase A.

### Tatsächliche lokale Nachweise

Die Produktionsgoldens wurden per `STP.Tests.Content.EditMode.ProofV1Tests.RegenerateGoldens()` aus dem neuen `ProofGenerator` erzeugt (expliziter Aufruf im ignorierten C#-Harness, derselbe statische Entry ist im Unity-Editor aufrufbar). Vor jedem Schreiben prüft der Produktionscodec die Ausgabe; die eingecheckten Tests regenerieren jeden Fall dreimal.

| Golden | Tatsächliche vier Metriken `(searchNodes,deductionSteps,maxDeductionDepth,requiredGuessDepth)` | Proofhash `STP-PROOF-JCS-1` |
|---|---|---|
| root | `(0,35,5,0)` | `dfdc03486b04a333b70e46d76a45e33bb5d3ae84580aa494558512352a0baa70` |
| single | `(0,7,3,0)` | `40acb1f2a6f680430334d214f62039ac66a8d983e4ad73ae3b355d336d653386` |
| search | `(2,12,2,1)` | `296f2020a176c72320254e95015a5586bf895fd40559875038d473744f9d1389` |

Der search-Proof ist schema-/hash-/regenerationsgültig, wird jedoch durch das unveränderte CampaignGate abgewiesen. Die beiden deduktiven Proofs passieren das technische Proofgate. Alle Ergebnisse bleiben ohne die späteren Cross-Reference-/Produkt-/Release-Lock-Gates nicht runtimeimportierbar und nicht release-lock-fähig.

| Ausführung | Tatsächliches Ergebnis / Grenze |
|---|---|
| Getrennte Domain-/Solver-/Infrastructure.Content-/Editor.Content-/Contenttest-Assemblies, .NET SDK 10.0.401, CLR 10.0.12, NUnit/NUnitLite 3.14.0 | Compile Exit 0, keine Compilerwarnungen/-fehler. Bestehendes unberührtes Content-Skeleton ist nur im Zusatzharness wegen seiner Applicationports ausgeschlossen; echte Unityassembly-/Projektkompilierung bleibt eigenes CI-Gate. |
| Vollständiger lokaler Contentlauf nach Assemblytrennung | 366/366 PASS; nach zehn zusätzlichen Every-stage-/Mixed-batch-Fällen **376/376 PASS**, keine Skips/Inconclusive/Warnings. Scratch-XML/Logs unter `.utmp/`. |
| Breiter lokaler Domain-/Solver-/Contentlauf | 440/440 PASS im ersten breiten Lauf; final mit den zehn zusätzlichen Contentfällen **450/450 PASS**, darunter 376 Content-, 19 Domain- und 55 unveränderte Solvertests (40,989 s). Keine Skips/Inconclusive/Warnings. Lokaler Zusatzbeleg, kein Unityersatz. |
| Kanonische Linuxvalidatoren auf vollständigem Phase-B-Working-Tree | WSL/Linux, CPython 3.11.13, Node 22.20.0, exakt requirements.lock: Architecture-only **17 Gruppen PASS**, eigener WP-025-Production-Scope **18 Gruppen PASS**, beide mit Self-/Negativtests. Scopekontext Basis 5f51273/HEAD 5e4b986/worktreeDirty=true; diese Bindung wird nicht als sauberer späterer Commit ausgegeben. |
| Bestehender unabhängiger Nodecheck | 12 Kanonisierungen, 32 Ablehnungen, zwei v2-/zwei v1-Vertragsvektoren PASS. Historischer Hash-/Legacybeleg, kein alleiniger neuer Proofnachweis. |
| **Tatsächliche neue Proofprojektion gegen Node** | Alle drei neuen Goldens: Draft-2020-12-Proof-/Levelschema, komplette kanonische Artefaktbytes, SHA-256 der vollständigen Projektion ohne proofHash, öffentlicher Puzzle-/Lösungshash und Artefaktreferenz **PASS**. Gepinnt Node 22.20.0, unverändert `jcs_crosscheck.mjs`; Outputs entsprechen exakt der Tabelle oben. |
| Whitespace/Manifest | `git diff --check 5f5127362b101dd4e066b68b21d3e35a5af314df --` Exit 0; Working-Tree-Manifestsha unverändert. Vollständiger Scope-/Trustlauf umfasst auch neue Dateien; finale HEAD/Remote-Bindung folgt nach Commit/Push. |

Kanonische Validatorbefehle bleiben unverändert wie im Testabschnitt. Der neue Node-Scratchbefehl lädt die drei `proof-v1-solver-v2-*.json` plus `.level.json`, validiert die echten Schemas und ruft für jedes vollständige Artefakt sowie `{proof ohne proofHash}`, `{puzzleId,rulesetVersion,grid,endpoints,rowCounts,columnCounts}` und `{puzzleId,publicPuzzleHash,path}` auf:

```text
node tools/architecture-validation/jcs_crosscheck.mjs <temporäre UTF-8-Projektionsdatei>
```

Jeder Rückgabewert `canonicalBase64`/`sha256` wird gegen die tatsächlichen C#-Bytes bzw. gespeicherten profilierten Hashes verglichen. Der ausgeführte Scratchwrapper liegt in `.utmp/wp025_proof_node.py`; Gesamtaufruf im gepinnten Linuxsetup `.utmp/wp025_linux_validation.py`, Output zusätzlich `.utmp/wp025-linux-production-proof-node.log`. Keine Node-/Workflow-/Repositoryabhängigkeit geändert. Scratchartefakte sind Zusatzlogs, die oben genannten Befehle, Inputs und tatsächlichen Outputs sind hier persistent.

Eigene korrigierte Zwischenbefunde: anfänglich unvollständige Applicationreferenz des Zusatzharness korrigiert; dessen erste Einzelassemblytests verdeckten einen internen Serializeraufruf im neuen Test, durch echte Assemblytrennung entdeckt und auf die öffentliche API korrigiert. Ein neuer Bindungstest erwartete PRF statt des vorhandenen früheren LVL-Semantikcodes; Stufengrenze und bestehender Code erhalten und Test entsprechend präzisiert. Linuxruntime unter Windowsmount extrahierte zu langsam; eigener unvollständiger Setupversuch beendet, danach vollständig erfolgreicher isolierter temporärer Lauf inklusive neuer Node-Projektion. Kein abgebrochener Versuch als PASS gewertet, keine fremde CI abgebrochen, keine Gateabsenkung.

### AK-/CI-/Abnahmegrenze vor dem ersten Implementierungspush

| Kriterium | Lokaler Implementierungsnachweis; spätere CI/QC bleibt separat |
|---|---|
| AK-B01 | GeneratedGoldens (root/single), EveryMetric und echte Schemaparität: Format 1, alle Minima 0, deduktiver searchNodes-Nullfall gültig. |
| AK-B02 | RegenerateGoldens + drei GeneratedGoldens-Testfälle: Codec/Selbsthash und dreifache echte bytegenaue Regeneration. Neue Nodeprüfung auf denselben Bytes PASS. |
| AK-B03 | SelfConsistentChangedMetric, vier Fälle: neuer Selbsthash intern gültig, Pipeline dennoch PRF-NONDETERMINISTIC. |
| AK-B04 | RehashedBindingMutations, WrongSlotAndUnknownProfiles, CrossPuzzleCopy, MalformedMissingUnknown: vollständige fail-closed Bindungs-/Versions-/Fehlercodeprüfung. |
| AK-B05 | StrictCommand, neun StrictPromotesWarningsFromEveryStage-Fälle, MixedBatch: Promotion jedes Einzelresultats vor Status/Stufe/Zählern; --strict und Report konsistent. |
| AK-B06 | CrossPuzzleCopy/ReferenceFailures, EarlierLevelFailures, FreshRegenerationAlwaysRuns, MultiplePublicSolutions und Suchgolden: Proof-/Kampagnengate geschlossen; spätere Import-/Lock-/Produktgates werden nie freigegeben. |
| AK-B07 | CanonicalProof, DiagnosticsAndBatchOrdering, MixedBatch: Objekt-/Quell-/Diagnosereihenfolge, Snapshotunveränderlichkeit und reproduzierbare kanonische Bytes. |

**Neue eigene Implementierungs-CI, tatsächliche Logs/XML/Coverage/Android/iOS-Artefakte und die beiden unabhängigen Abschlussprüfungen stehen vor Push noch aus. Kein Gesamt-PASS.** Phase-A-CI ersetzt keine davon. Verfügbare getrennte Subagent-Prüfer können nach erfolgreicher Implementierungs-CI beide Rollen ausführen; sie werden erst dann auf demselben eingefrorenen finalen HEAD separat beauftragt und dürfen keine Ergebnisse vor Berichtsabgabe austauschen. Ihre tatsächlichen Berichte und die finale SHA-/Remote-/CI-Bindung werden in PR #19 persistent verlinkt. Änderungen während laufender Abschlussprüfungen sind ausgeschlossen; bestätigte Befunde erfordern Scopekorrektur und erneute unabhängige Prüfung des geänderten Stands.

Letzter abgeschlossener Schritt: vollständige lokale Implementierung, Produktionsgoldens, Content-/Domain-/Solver-, Linux- und unabhängige Node-Nachweise sowie Fachdokumentation. Nächster Schritt: normaler Commit/Push auf bestehender Branch, neue eigene CI selbst prüfen, finalen Stand dokumentieren und für beide unabhängigen Abschlussrollen einfrieren. Gesamtpaket und Integrationsgate bleiben offen; Produktfolgeblocker 001–003, physische Geräte-/SDK-/Storegrenzen unverändert.

Der erste saubere Implementierungscommit ist `db2805bc35a32ab388ac81b0b01f65ea01e79293` (38 Phase-B-Dateien; vollständiger Basisdiff 40 Dateien einschließlich Phase A). Sein nachgelagerter Linuxlauf fand einen eigenen mechanischen Dokumentationsfehler: Die Statusfortschreibung hatte die vom vorhandenen Validator ausdrücklich verlangte Fettschrift `Architecture v1.0` entfernt (`version:current-state`). Die Fettschrift wurde wiederhergestellt, der Validator unverändert gelassen und beide betroffenen Läufe werden vor Push wiederholt. Kein Code-/Test-/Manifestfehler und kein übertragenes PASS auf diesen Zwischencommit.

### Nachweis-/Korrekturfortschreibung auf 6d4b5c1

Korrigierter sauberer HEAD `6d4b5c10c5be2f001de27da7d68c3139cda752b1`: beide kanonischen Linuxläufe erneut Exit 0, 17/18 Gruppen inklusive Self-/Negativtests PASS, Scopekontext exakt diese SHA/Basis 5f51273/worktreeDirty=false. Bestehender und neuer Drei-Proof-Nodecheck ebenfalls erneut PASS. Normale Commits, keine Historienumschreibung.

Der erste Push wurde von der automatischen Freigabeprüfung wegen fehlender direkter Pushautorisierung im vertrauenswürdigen Chat blockiert. Der Auftraggeber bestätigte danach ausdrücklich den konkreten normalen Push und die Fortsetzung von CI/Abnahmen; Push erfolgreich. Die separat abgelehnte PR-Titel-/Beschreibungsschreibung wurde nach direkter zusätzlicher Autorisierung der PR-Dokumentation und tatsächlichen Prüfberichte erfolgreich durchgeführt. Keine Umgehung der Ablehnungen. Der tatsächliche PR-HEAD ist 6d4b5c1, Basis unverändert. Vollständiger Remote-PR-Diff (40 Dateien, 171.493 Zeichen) stimmt mit lokalem Basisdiff (171.361 Zeichen) exakt überein, wenn ausschließlich CRLF und die von Git unterschiedlich hergeleiteten Hunküberschriften ausgeglichen werden; alle Pfade, Blob-/Hunkdaten und Änderungszeilen gleich. 175 eindeutige Unity-GUIDs, keine fehlenden Metas.

Eigene Architecture Push [38403792373](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38403792373), Job 115317951865, und PR [38403799616](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38403799616), Job 115317977245: completed/success. Beide vollständigen Logs selbst abgerufen; 17/18 Gruppen und bestehender Nodecheck PASS. Pushscope direkt auf 6d4b5c1; PR Architecture-only/Node zunächst synthetisch `f69da37`, danach expliziter echter Head-Checkout 6d4b5c1 für Scope, worktreeDirty=false. Die separat ausgeführte neue Node-Proofprojektion ist oben konkret belegt und kein Legacy-PASS-Ersatz.

Eigene Unity Push [38403792397](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38403792397) und PR [38403799772](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38403799772) auf 6d4b5c1 sind noch nicht vollständig beendet; kein Gesamt-PASS. Vor Push war der vorher bestehende Unitylauf 38396165644 vollständig success; keine fremde CI abgebrochen. Beide automatischen Push-/Synchronizeereignisse des bestehenden PR teilen unverändert den Lizenzslot; natürlich cancelled/pending/skipped wäre kein PASS.

Während diese CI den unveränderten Commit 6d4b5c1 prüft, wurde im eigenen Selbstreview ein Strict-Randfall reproduziert: Ein beliebiger numerischer Severity-Enumwert war im neuen Diagnosemodell konstruierbar, obwohl nur Error/Warning gültig sind, und konnte daher die Promotion umgehen. Zuerst neuer Regressionstest `InvalidDiagnosticSeverity_CannotBypassStrictPromotion`: **FAIL** gegen den vorherigen Code (keine erwartete Abweisung). Danach Scopekorrektur: Konstruktor weist unbekannte Severity mit `ArgumentOutOfRangeException` ab. Getrennte Assemblies erneut ohne Compilerwarnungen/-fehler, kompletter betroffener Contentlauf **377/377 PASS**, danach breiter lokaler Lauf **451/451 PASS** (377 Content, 19 Domain, 55 Solver; 38,521 s). Der geänderte Lieferstand erhält neue CI und beide späteren unabhängigen Prüfungen; kein 6d4b5c1-PASS wird auf ihn übertragen. Keine finale Prüfung bereits gestartet, keine Änderung an einem eingefrorenen Abschlussprüfstand.

### Erste vollständige Implementierungs-CI und finaler Lieferstand

Unity Push **38403792397 vollständig completed/success, alle zehn Jobs success** auf 6d4b5c10. Die echten Compile-/EditMode-, PlayMode-/Coverage-, Android- und iOS-Joblogs wurden selbst abgerufen; die Test-/Coverageartefakte heruntergeladen, Digest geprüft und XML selbst gelesen. Paralleler PR-Run 38403799772 bleibt cancelled und ist ausdrücklich kein PASS. Kein manueller CI-Abbruch oder fremder Neustart.

| Gate auf 6d4b5c10 | Tatsächlicher Beleg |
|---|---|
| Compile/EditMode | Job 115318072107; NUnit-XML **451/451 insgesamt**, davon **450 projektspezifisch** (376 Content, 19 Domain, 55 Solver) plus ein unberührter Addressables-Pakettest. Alle 37 ProofV1Tests-Fälle Passed, keine Failed/Skipped. Artifact 11818985710, ZIP-SHA-256 `253a13536b09429baa536b0172bb44c26feb8636033e5430f40d6c1e7c4353fe`, direkt nachgerechnet. |
| Coverage | Echte Gesamt-/Mess-XML: Editor.Content 171/181 Sequenzpunkte (94,5 %), 36/37 Methoden; Infrastructure.Content 2162/2214 (97,7 %), 380/386; Solver 1022/1031 (99,1 %), 123/123. Coveragegate für 15 STP-Assemblies PASS, keine erfundene Mindestschwelle. |
| PlayMode/QA | Job 115321863003, XML **27/27 Passed**, einschließlich tatsächlichem `QaScene_ComposesCompleteGraph_ExactlyOneBindingPerPort_NoProviderStart`. Artifact 11817677206, ZIP-SHA-256 `9fb79f409d27e30eab752e81ce0d047ee6ef0b706cec345e712b98a83848c07a`, direkt geprüft; Coveragegate ebenfalls PASS. |
| Android Development IL2CPP | Job 115323326159 success; tatsächliche Logs `Build Finished, Result: Success` und `STP Build PASS: Android -> Builds/Android/stp-qa-development.apk`. Artifact 11818398168, 38.949.007 ZIP-Bytes, GitHub-Digest `083dc502ec31c31f8bc2ac92d4f8bbf17c702f33ba78354c59c624b2a92d7178`, explizit an 6d4b5c10 gebunden. APK nicht auf physischem Gerät ausgeführt. |
| iOS Export/Xcode | Job 115327939138 success; tatsächliche Logs `STP Build PASS: iOS -> Builds/iOS/Xcode` und **`BUILD SUCCEEDED`** (Xcode 26.3/iPhoneOS SDK 26.2). Kein Signing-/Store-/physischer Gerätebeleg behauptet. |
| CI-Commit-/Treebindung | Tatsächlicher PR-HEAD 6d4b5c10, Basis 5f51273; synthetischer PR-CI-Commit `f69da3709a4a7a7e5c253be32f0f521e093d5fee` nur lesend gefetcht. Beide Bäume exakt `482bfc8f065439faead0c21909d8954f3b9d7dc1`. Kein tatsächlicher Merge. |

Der nachfolgende normale Liefercommit enthält ausschließlich die lokal **451/451** geprüfte Severitykorrektur samt Regressionstest, zugehörige Fachdokumentation und diese Status-/Nachweisfortschreibung. Vor seinem Push ist der bestehende vollständige Unitylauf beendet; keine weitere wartende fremde Lizenzprüfung wird durch einen vorsorglichen Push verdrängt. Beide kanonischen Linuxläufe, der vollständige Basisdiff und der neue Drei-Proof-Nodecheck werden auf seinem tatsächlichen sauberen HEAD nochmals geprüft. Die neue eigene CI wird gesondert mit ihren tatsächlichen Runs/Jobs/Artefakten am PR gebunden; die erfolgreiche erste CI ist lediglich der Vorläuferbeleg.

Nach erfolgreicher finaler Implementierungs-CI prüfen zwei **tatsächlich getrennte unabhängige Subagentinstanzen** den identischen eingefrorenen finalen HEAD: [Proof-/Vertrags-QC sowie Strict-/Test-/Scope-/CI-QC und vollständige Berichte in PR #19](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19). Sie erhalten getrennte Rollen ohne gegenseitigen Ergebnisaustausch, dürfen keine versionierte Datei/HEAD verändern und liefern ihre Berichte zuerst getrennt ab. Erst nach beiden Berichten werden Ergebnisse am PR persistiert. Die konkrete finale SHA, vollständige CI-Evidenz und direkten Berichtlinks werden in der PR-Beschreibung festgehalten, damit diese abschließende Evidenzfortschreibung den eingefrorenen Lieferstand nicht erneut verändert. Bestätigte korrekturbedürftige Befunde führen zu Scopekorrektur und erneuter unabhängiger Abnahme des geänderten Stands.

**Implementierung geliefert; Gesamtpaket nicht integriert.** Die lokale Akzeptanzprüfung AK-B01–B07 ist in den oben benannten Tests erfüllt, einschließlich zusätzlichem Invalid-Severity-Regressionsfall zu AK-B05. Die finalen CI-/QC-Ergebnisse sind nach Ausführung über den verlinkten PR an exakt dieselbe finale SHA zu binden; vor deren Ausführung gibt es keinen selbst erteilten Gesamt-PASS. Nächster Freigabepunkt bleibt nach beiden unabhängigen Abnahmen die ausdrückliche Auftraggeberentscheidung zur Integration. Produktfolgeblocker 001–003 und alle übrigen Release-/Geräte-/SDK-/Storegrenzen bleiben unverändert.
