# WP-013 – Level-v2-Parser, JCS-Hashverträge, proof-v1 und LevelValidationPipeline

## ID

`WP-013`

**Bearbeitungsstatus:** Abgeschlossen

**Trust Anchor:** Dieses Work Package und sein Production-Scope-Manifest
[`tools/architecture-validation/scopes/WP-013.production.scope.json`](../tools/architecture-validation/scopes/WP-013.production.scope.json)
werden gemeinsam in einem Trust-Anchor-Commit eingeführt (ADR-030); der Anker wird auf
`feat/wp-013-level-v2-pipeline` verankert und remote verifiziert. Der `baseCommit` des
Manifests ist der Elterncommit des Ankers
(`767c01efb3720b3dae080280f58c568fbb36686f`, WP-009-HEAD).

**Nummernkontext:** Die IDs `WP-010` und `WP-011` sind durch abgeschlossene
Governance-Arbeiten auf `main` vergeben (`docs/wp-010-tests-standort`,
`chore/wp-011-pr-scope-checkout`). Die ID `WP-012` ist im dokumentierten Projektstand
nicht als Work Package verankert, gilt als vergeben und wird nicht wiederverwendet;
kein Inhalt dieses verworfenen Ansatzes ist Quelle, Vorlage oder Inspiration für
WP-013. Die nächste zulässige ID ist daher `WP-013`.

## Ziel

Implementierung der Leveldaten-Validierungskette für das bestätigte Level-v2-Format
als Produktionscode auf dem bestehenden WP-008-/WP-009-Modulgraphen gemäß den
verbindlichen Verträgen
[`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md),
[`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md) (Abschnitt 5),
[`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md)
(Abschnitte 5 und 12),
[`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md)
(Abschnitte 3 und 4),
[`ARCHITECTURE/PUZZLE_ENGINE.md`](../ARCHITECTURE/PUZZLE_ENGINE.md) (Abschnitt 10),
[`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md) (Abschnitte 4 und 5)
sowie [`ADR-021`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md) und
[`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md).

Dies ist der im Projektstand nach WP-009 ausdrücklich dokumentierte nächste Schritt
(`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`). Generator und
Levelauthoring folgen ausdrücklich erst danach und sind nicht Teil dieses Work Packages.

Konkret entstehen:

- Der **Level-v2-Parser** in `STP.Infrastructure.Content`: robuster UTF-8-JSON-Parser
  für lokale Dateien als untrusted Input (LEVEL_DATA_FORMAT.md Abschnitt 11) mit
  Ressourcengrenzen für Dateigröße, Verschachtelungstiefe, Stringlänge und technische
  Rastergröße, Duplicate-Key-Ablehnung sowie deaktivierter polymorpher Typnamen- und
  automatischer Typkonstruktion; strukturelle Prüfung gegen den `level-v2`-Vertrag
  (das kanonische Schemadokument bleibt
  `ARCHITECTURE/schemas/level-v2.schema.json`, Draft 2020-12); DTO-Schicht und
  Domain-Mapping auf `PuzzleDefinition` einschließlich des Ein-Zellen-Sonderfalls.
  Unbekannte `documentSchemaVersion` oder `rulesetVersion` schlagen fail-closed fehl.
- Die **semantischen Leveldiagnosen** der dokumentierten Codefamilien `LVL-ID-*`,
  `LVL-GRID-*`, `LVL-ENDPOINT-*`, `LVL-PATH-*`, `LVL-RULE-*`, `LVL-COUNT-*` und
  `LVL-TIME-ORDER` (LEVEL_DATA_FORMAT.md Abschnitt 8) für den einzelnen
  Leveldatensatz einschließlich seiner Authoringlösung.
- Die **JCS-Hashverträge** (LEVEL_DATA_FORMAT.md Abschnitt 6): RFC-8785-Kanonisierung
  als UTF-8 ohne BOM und ohne Abschlussnewline unter I-JSON-Regeln (keine
  Fließkommazahlen, ungültige Surrogate abgelehnt), SHA-256, profilierte Hashes
  `{profile, sha256}` und eine Profilregistry mit `STP-PUZZLE-SEMANTIC-JCS-1`,
  `STP-SOLUTION-JCS-1`, `STP-PROOF-JCS-1` sowie dem historischen Leseeintrag
  `STP-LEVEL-V1-PUZZLE-JCS-1`. Ein unbekanntes Profil ist ein harter Fehler
  (`LVL-HASH-*`); kein Hash wird anhand seines Wertes heuristisch gedeutet.
- Das **proof-v1-Artefakt** (LEVEL_DATA_FORMAT.md Abschnitt 7, SOLVER_ARCHITECTURE.md
  Abschnitt 5) nach `ARCHITECTURE/schemas/proof-v1.schema.json`: `proofFormatVersion`,
  `puzzleId`, profilierter `publicPuzzleHash`, profilierter `solutionHash`,
  `solverVersion` (`solver-v1`), `solutionCount` und die Metriken `searchNodes`,
  `deductionSteps`, `maxDeductionDepth`, `requiredGuessDepth`; Proofhash
  `STP-PROOF-JCS-1` über alle vorgenannten Felder außer sich selbst; Generierung aus
  dem Solverlauf über den unveränderten öffentlichen Puzzleinput; Vergleich der
  einzigen gefundenen Lösung mit der Authoringlösung über den kanonischen
  Lösungshash; Bindungsprüfungen der Familie `PRF-*` (Artefaktformat,
  Puzzle-/Lösungsbindung, Lösung genau eins, Proofhash) inklusive Erkennung von
  Copy-Paste auf ein anderes Puzzle, stale Lösungshash und Metrikänderung sowie die
  `proofRef`-Bindung des Leveldokuments an Artefakt-ID, Format und Proofhash.
- Die **LevelValidationPipeline** in `STP.Editor.Content` als gemeinsamer
  Batchvalidator (CONTENT_PIPELINE.md Abschnitt 5) mit den Stufen Parse, Schema,
  Domain map, Semantic und Solver/Proof, sortierten Fehlern/Warnungen mit stabilen
  Diagnosecodes, einem `--strict`-Modus, der Warnungen als Fehler behandelt, und der
  Regel, dass ein späterer Schritt einen früheren Fehler nicht durch Fallback
  verdeckt. Die dort ebenfalls genannten Stufen Cross-reference, Quality und Import
  sind ausdrücklich nicht Teil dieses Work Packages.
- Aussagekräftige EditMode-Tests in `STP.Tests.Content.EditMode` sowie ergänzend in
  `STP.Tests.Solver.EditMode`, soweit Proofeingaben im Solver angesiedelt werden.

## Voraussetzungen

- Vollständige Lesereihenfolge aus `AGENTS.md`; zusätzlich die oben genannten
  Vertragsdokumente, `PROJECT_CONTROL/WORK_PACKAGE_RULES.md`,
  `PROJECT_CONTROL/AI_HANDOVER_RULES.md`, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`,
  `tools/architecture-validation/README.md` sowie ADR-026 und ADR-030.
- WP-009 ist abgeschlossen auf Commit `767c01efb3720b3dae080280f58c568fbb36686f`
  (Branch `feat/wp-009-puzzle-kern`); WP-013 arbeitet auf dem davon abgeleiteten
  Branch `feat/wp-013-level-v2-pipeline`. WP-008 und WP-009 erhalten keine
  nachträglichen Änderungen; ihre Trust-Anchors und Manifeste bleiben unverändert.
- Lokaler Unity-Nachweis des Scaffolds und des WP-009-Kerns (Unity 6000.3.23f1,
  EditMode 65/65 PASS) liegt vor; die offenen WP-008-Buildnachweise
  (GitHub-CI-Runner, Android, iOS) blockieren WP-013 ausdrücklich nicht.
- `BLOCKER-PROD-001/002/003` bleiben unverändert offen und fail-closed: Hintcredits,
  Tagesanspruch und das Generator-Qualitätsprofil sind nicht Teil dieses Work
  Packages.
- Keine neuen Pakete: Der gepinnte Stack (`TECH_STACK.md`) enthält mit
  `com.unity.nuget.newtonsoft-json` 3.2.2 die einzige JSON-Bibliothek. Reicht der
  vorhandene Stack für eine vertragstreue Fähigkeit nicht aus (insbesondere eine
  generische Draft-2020-12-Auswertung in C#), ist das Work Package zu stoppen und
  der Befund zu berichten statt eine Dependency eigenmächtig einzuführen.

## Scope

Erlaubt ist ausschließlich:

1. Parser-, DTO-, Struktur- und Semantikcode für `level-v2` in
   `STP.Infrastructure.Content` (Parse-, Schema-, Domain-map- und Semantic-Stufe)
   gemäß LEVEL_DATA_FORMAT.md Abschnitte 1–8 und 11.
2. JCS-Kanonisierung, Profilregistry und die drei aktuellen Hashprojektionen
   (`STP-PUZZLE-SEMANTIC-JCS-1`, `STP-SOLUTION-JCS-1`, `STP-PROOF-JCS-1`) in
   `STP.Infrastructure.Content`. Der historische Profilname
   `STP-LEVEL-V1-PUZZLE-JCS-1` wird ausschließlich als bekannter Registriereintrag
   geführt; die v1-Projektionsberechnung und die v1→v2-Migration sind nicht Teil
   dieses Work Packages.
3. Ein typisiertes Proof-Ergebnismodell in `STP.Puzzle.Solver` entsprechend der
   dokumentierten Modulverantwortung „Constraintmodell, Propagation, Suche, Proof
   und Deduktionsspur": Bündelung von Lösungsklassifikation, gefundenem Lösungspfad,
   den dokumentierten Metriken und der `solver-v1`-Versionskonstante als Eingabe für
   die Proofgenerierung. Constraintsemantik, Tiebreaker, Wertreihenfolge und
   Metrikdefinitionen des Solvers bleiben unverändert; jede Änderung daran wäre eine
   neue Solverversion und ist nicht Teil dieses Work Packages.
4. proof-v1-Artefaktmodell, Serialisierung/Deserialisierung, Hashbindung und die
   `PRF-*`-Prüfungen in `STP.Infrastructure.Content` sowie die Proofgenerierung und
   -bindung als Solverstufe der Pipeline.
5. Die `LevelValidationPipeline` in `STP.Editor.Content` (dokumentierte
   Batchvalidator-Verantwortung) mit den fünf oben genannten Stufen, sortierten
   Diagnosen und `--strict`-Semantik, ohne GUI und ohne eigenes abweichendes
   Validierungsverhalten.
6. EditMode-Tests in `STP.Tests.Content.EditMode` und `STP.Tests.Solver.EditMode`.
7. Mechanische Governance-Nachführungen: dieses Work Package, sein Scope-Manifest,
   die CI-Scope-Umschaltung in `.github/workflows/validate.yml`, die mechanische
   Spiegelung ergänzter Testassembly-Referenzen im statischen Modulgraph-Check von
   `.github/workflows/unity.yml` (Unity-Assemblyreferenzen sind nicht transitiv)
   sowie `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`.

## Betroffene Dateien/Module

| Pfad/Modul | Änderung |
|---|---|
| `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/` | Neu: Level-v2-Parser, DTOs, strukturelle und semantische Diagnosen, JCS-Kanonisierung, Profilregistry, Hashprojektionen, proof-v1-Artefakt-IO und `PRF-*`-Bindungen. |
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/` | Neu: typisiertes Proof-Ergebnismodell (Klassifikation, Lösungspfad, Metriken, `solver-v1`-Konstante); bestehende Solversemantik unverändert. |
| `Assets/StammstreckenPuzzle/Scripts/Editor/Content/` | Neu: `LevelValidationPipeline` als Batchvalidator (Parse, Schema, Domain map, Semantic, Solver/Proof, `--strict`). |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Content/` | Neu: Content-EditMode-Tests (Parser, Semantik, JCS, proof-v1, Pipeline). |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/` | Ergänzend: Tests des Proof-Ergebnismodells, soweit dort angesiedelt. |
| `.github/workflows/validate.yml` | Mechanische Umschaltung von `STP_SCOPE_MANIFEST` auf das WP-013-Manifest. |
| `.github/workflows/unity.yml` | Mechanische Spiegelung, falls Testassembly-Referenzen ergänzt werden (Unity-Assemblyreferenzen sind nicht transitiv). |
| `WORK_PACKAGES/WP-013_Level-v2-Parser-JCS-Hashvertraege-proof-v1-und-LevelValidationPipeline.md` | Neu: dieses Work Package. |
| `tools/architecture-validation/scopes/WP-013.production.scope.json` | Neu: verankertes Production-Scope-Manifest. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Statusfortschreibung zu WP-013. |

## Ausdrücklich nicht erlaubte Änderungen

- Kein Generator, keine Generatorvalidierung, kein Levelauthoring und kein
  Editorfenster; keine 240 konkreten Rätselinstanzen und keine Zeitwertkalibrierung
  (`starThresholdsSeconds` bleibt reines Datenfeld ohne produktseitige Kalibrierung);
  allesamt spätere Work Packages.
- Keine Cross-reference-, Quality- oder Import-Stufe der LevelValidationPipeline
  (Kataloge, Lokalisierung, Assets und deterministischer Unity-Import folgen
  später); kein `Content/`-Authoringbaum und keine echten Season-1-Level; die
  Fixtures unter `ARCHITECTURE/examples/` bleiben unveränderte `FIXTURE_ONLY`-Verträge.
- Keine Kataloge (`campaign-v2`, `completion-v1`, `cosmetics-v2`), keine
  Lokalisierungstabellen, kein `release-lock-v1`, keine v1→v2-Migration, keine
  Save-/Fortschrittsbindung und keine Persistenz.
- Keine Implementierung der Runtime-Katalogports (`ILevelCatalog`,
  `ICampaignCatalog`, `ICompletionCatalog`, `ICosmeticsCatalog` bleiben Skelette);
  keine Application-, Persistenz-, UI-, World-, Audio-, Plattform-, MobileServices-
  oder Bootstrap-Fachlogik; keine Änderung an Produktionsassemblies außerhalb von
  `STP.Infrastructure.Content`, `STP.Editor.Content` und der genannten
  Proof-Erweiterung in `STP.Puzzle.Solver`.
- Keine neuen Assemblies, keine neuen direkten Produktions-Assemblyreferenzen, keine
  neuen Pakete oder Dependency-Änderungen (`Packages/`, `Toolchain.lock.md` bleiben
  unverändert). Ergänzte Referenzen der Testassemblies auf bereits bestehende
  Produktionsassemblies sind zulässig und werden mechanisch in `unity.yml` gespiegelt.
- Keine Änderung an `ARCHITECTURE/`, `DECISIONS/` oder den Konzeptdateien unter
  `Stammstrecken_Puzzle_Konzept_00-15/`; kein neuer ADR; keine neue oder geänderte
  Produktentscheidung; keine Änderung an `BLOCKER-PROD-001/002/003`; keine neue
  Solverversion.
- Keine Auslieferung der Authoringlösung an die Puzzle-UI und kein Vergleich eines
  Spielerzwischenstands mit der Authoringlösung; die Lösung bleibt ein
  authoringseitiges Artefakt.
- Keine Verwendung von Inhalten des verworfenen WP-012-Ansatzes (kein Checkout,
  keine Übernahme, keine Vorlage).
- Keine Änderung an bestehenden Scope-Manifesten oder Trust-Anchors; kein Merge
  nach `main` oder einem anderen Branch; kein Pull Request; kein Force Push.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust Anchor: WP-013 und sein Manifest wurden gemeinsam im selben Add-Commit eingeführt, der Manifest-`baseCommit` ist der Elterncommit des Ankers (`767c01e…`), der Anker ist remote auf `feat/wp-013-level-v2-pipeline` verifiziert, das Manifest ist seitdem byteunveränderlich. |
| `AK-02` | Der Parser akzeptiert die Vertragsfixtures `level-v2.example.json` und `level-v2.single-cell.example.json` und mappt sie auf gültige `PuzzleDefinition`-Objekte; malformed JSON, Duplicate Keys, Überschreitung der dokumentierten Ressourcengrenzen, unbekannte Eigenschaften (`additionalProperties: false`), polymorphe Typmetadaten sowie unbekannte `documentSchemaVersion`/`rulesetVersion` werden fail-closed mit stabilen Diagnosecodes abgelehnt. |
| `AK-03` | Die Semantikstufe prüft den Datensatz einschließlich Authoringlösung gegen die dokumentierten Familien `LVL-ID-*` (Segmente exakt gegen `content.season`, `networkSection`, `route`, `position`), `LVL-GRID-*`, `LVL-ENDPOINT-*`, `LVL-PATH-*`, `LVL-RULE-*`, `LVL-COUNT-*` (aus der Lösung abgeleitete Randzahlen) und `LVL-TIME-ORDER` und meldet sortierte, stabile Diagnosecodes; katalogübergreifende ID-/Hierarchieprüfungen bleiben ausdrücklich der späteren Cross-reference-Stufe vorbehalten. |
| `AK-04` | JCS-Kanonisierung erzeugt RFC-8785-Bytes (UTF-8 ohne BOM, ohne Abschlussnewline, I-JSON-Regeln, Fließkommazahlen und ungültige Surrogate abgelehnt); die drei Projektionen reproduzieren exakt die in den Vertragsfixtures (`level-v2.example.json`, `level-v2.single-cell.example.json`, `proof-v1.example.json`, `proof-v1.single-cell.example.json`) dokumentierten Hashes; die C#-Ergebnisse stimmen mit dem unabhängigen CI-Crosscheck (`tools/architecture-validation/jcs_crosscheck.mjs`) überein; ein unbekanntes Profil ist ein harter Fehler; semantikneutrale Änderungen (Formatierung, Schlüsselreihenfolge, `documentSchemaVersion`, `contentRevision`, Texte, Proofregeneration) ändern den semantischen Puzzlehash nicht, Änderungen an Puzzle-ID, Ruleset, Raster, Endpoints oder Counts ändern ihn. |
| `AK-05` | proof-v1 wird aus dem unveränderten öffentlichen Puzzleinput mit `solver-v1` generiert; gleicher Input und gleiche Solverversion erzeugen bytegleichen Proof; die einzige gefundene Lösung wird mit der Authoringlösung über den kanonischen Lösungshash verglichen; `solutionCount` ist verbindlich `1`; Copy-Paste auf ein anderes Puzzle, stale Lösungshashes und Metrikänderungen werden erkannt; das `proofRef` des Leveldokuments bindet Artefakt-ID, Proofformat und Proofhash. |
| `AK-06` | Die LevelValidationPipeline führt die fünf Stufen in der dokumentierten Reihenfolge aus, meldet sortierte Diagnosen, behandelt Warnungen im `--strict`-Modus als Fehler und verdeckt frühere Fehler nicht durch spätere Stufen; jede Fixturemutation scheitert an der zuständigen Stufe mit dem zuständigen Code. |
| `AK-07` | Die EditMode-Tests decken die in den Verträgen geforderten Pflichtfälle ab und laufen lokal unter Unity 6000.3.23f1 grün; der Ausführungsstand wird wahrheitsgemäß berichtet (nicht ausführbare Nachweise NOT_EXECUTED, niemals simuliert). |
| `AK-08` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich Manifest-erlaubte Pfade; `git diff --check` und der kanonische Scope-Lauf mit dem WP-013-Manifest bestehen. |

## Tests

- `STP.Tests.Content.EditMode`: Parser positiv/negativ (die Fixtures aus
  `ARCHITECTURE/examples/` dienen gemäß TEST_STRATEGY.md Abschnitt 5 als Golden
  Fixtures), Robustheitsgrenzen, Duplicate-Key-Ablehnung, strukturelle Verletzungen
  je Regel, Domain-Mapping inklusive Ein-Zellen-Fall, je ein Vertreter jeder
  `LVL-*`-Codefamilie, JCS-Known-Answer-Vektoren gegen die dokumentierten
  Fixture-Hashes, Crosscheck gegen `jcs_crosscheck.mjs`, unbekanntes Profil,
  Fließkomma-/Surrogate-Ablehnung, Semantikinvarianz und -änderung des Puzzlehashs,
  proof-v1-Bindungen (Cross-Puzzle-Copy, stale Lösungshash, Metrikänderung,
  `proofRef`), Determinismus der Proofgenerierung sowie Pipeline-Stufenordnung,
  `--strict`-Semantik und Diagnosesortierung.
- `STP.Tests.Solver.EditMode` (ergänzend): Proof-Ergebnismodell deterministisch für
  bekannte 0-/1-/2+-Fälle, `solver-v1`-Versionskonstante unverändert.
- Ausführung: lokaler Unity-EditMode-Lauf mit Unity 6000.3.23f1; das Ergebnis wird
  mit Protokoll benannt. Nicht ausführbare Nachweise werden als NOT_EXECUTED
  gemeldet, niemals simuliert.

## Risikoklasse

Mittel. Mehrere betroffene Komponenten (Parser, Hashverträge, Proof, Pipeline) mit
relevanter Auswirkung auf Datenqualität und Tests, jedoch innerhalb der bestehenden
Modulgrenzen und ohne neue Architekturentscheidung. Keine Persistenz-, Datenschutz-,
Monetarisierungs- oder Releaseberührung. Die Robustheit gegen untrusted Input ist
vertraglich begrenzt und durch vorgeschriebene Negativtests abgesichert.

## Definition of Done

- Alle Akzeptanzkriterien sind nachgewiesen oder mit konkretem Grund als offen
  dokumentiert; keine simulierten Nachweise.
- Der kanonische Validatorlauf mit dem WP-013-Manifest und `git diff --check`
  gegen den `baseCommit` bestehen.
- Die lokal ausführbaren EditMode-Tests sind tatsächlich gelaufen; ihr Ergebnis ist
  im Work Package dokumentiert.
- `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` sind
  fortgeschrieben; Ergebnis, Validierung und offene Punkte sind so dokumentiert,
  dass ein neuer Agent ohne Chat-Verlauf übernehmen kann.

## Abschlussdokumentation

### Ergebnis

WP-013 ist abgeschlossen. Die Leveldaten-Validierungskette ist als Produktionscode
auf dem WP-008-/WP-009-Modulgraphen implementiert:

- **`STP.Infrastructure.Content`** (17 neue Dateien): strikter UTF-8-JSON-Parser
  für untrusted Input (`StrictJsonParser` mit `JsonParseLimits`: Dateigröße,
  Verschachtelungstiefe, Stringlänge, Arraylänge; Duplicate-Key-Ablehnung;
  Ablehnung von Fließkomma-/Exponententokens, ungültigen Surrogaten,
  Steuerzeichen, BOM und polymorphen Typmetadaten `$type`; automatische
  Typkonstruktion existiert strukturell nicht) auf dem Dokumentmodell
  `JsonValue`; RFC-8785/JCS-Kanonisierung (`JcsCanonicalizer`, UTF-8 ohne BOM,
  ohne Abschlussnewline, I-JSON); Profilregistry (`HashProfiles`) mit den drei
  aktuellen Profilen und dem historischen Leseeintrag; die drei
  Hashprojektionen (`PuzzleHashContracts`); Diagnosemodell
  (`LevelDiagnostic`, sortiert, Fehler/Warnung); level-v2-DTO-Schicht
  (`LevelV2Dtos`); zweckgebaute strukturelle Prüfung gegen den feststehenden
  level-v2-Vertrag (`LevelV2SchemaValidator`, Codes `LVL-SCHEMA-*`; das
  kanonische Schemadokument `ARCHITECTURE/schemas/level-v2.schema.json`
  bleibt unverändert maßgeblich, eine generische Draft-2020-12-Maschine ist
  nicht Bestandteil); Domain-Mapping (`LevelV2DomainMapper`, Codes
  `LVL-GRID-*`/`LVL-ENDPOINT-*`, Ein-Zellen-Sonderfall); semantische
  Leveldiagnosen (`LevelV2Semantics`: `LVL-ID-*`, `LVL-PATH-*`, `LVL-RULE-*`,
  `LVL-COUNT-*`, `LVL-TIME-ORDER`, `LVL-HASH-*`); proof-v1-Artefaktmodell mit
  Formatstufe (`ProofV1SchemaValidator`, Codes `PRF-FORMAT-*`),
  Serialisierung/Deserialisierung (`ProofV1Serializer`) und den
  `PRF-*`-Bindungsprüfungen (`ProofV1Binding`).
- **`STP.Puzzle.Solver`** (1 neue Datei, 2 Dateien ergänzt): typisiertes
  Proof-Ergebnismodell (`SolverProofResult`: Klassifikation, gefundener
  Lösungspfad A→B inklusive Ein-Zellen-Fall, `SolverMetrics`,
  `MaxDeductionDepth`, `solver-v1`-Konstante) über
  `PuzzleSolver.SolveForProof`. Die in SOLVER_ARCHITECTURE.md Abschnitte 5
  und 7 dokumentierte, bislang nicht berechnete Metrik
  `maxDeductionDepth` wird rein additiv als Prämissentiefen-Buchführung in
  `SolverCore` ergänzt (statische Reduktionen Tiefe 1, abhängige Reduktionen
  +1; Suchannahmen sind keine Deduktionen). Constraintsemantik, Tiebreaker,
  Wertreihenfolge und die bestehenden solver-v1-Metrikdefinitionen sind
  unverändert; die WP-009-Tests laufen unverändert grün.
- **`STP.Editor.Content`** (2 neue Dateien): `ProofGenerator` (proof-v1 aus
  dem Solverlauf über den unveränderten öffentlichen Puzzleinput,
  `solver-v1`, `solutionCount` 1, dokumentierte Metriken, Proofhash
  `STP-PROOF-JCS-1`, kanonische Bytes) und `LevelValidationPipeline` mit den
  Stufen Parse, Schema, Domain map, Semantic und Solver/Proof, sortierten
  Diagnosen, `--strict`-Semantik (Warnungen als Fehler) und der Regel, dass
  spätere Stufen frühere Fehler nicht verdecken (Stufenabbruch bei Fehler,
  frühere Diagnosen bleiben vollständig erhalten). Cross-reference-, Quality-
  und Import-Stufe sind nicht Teil.
- **Tests** (8 neue Dateien): 66 Testmethoden in `STP.Tests.Content.EditMode`
  (Parser, Robustheitsgrenzen, Duplicate Keys, strukturelle Verletzungen je
  Regel, Domain-Mapping inklusive Ein-Zellen-Fall, je Vertreter aller
  `LVL-*`-Familien, JCS-Known-Answer-Vektoren, Live-Crosscheck gegen
  `jcs_crosscheck.mjs`, unbekanntes Profil, Fließkomma-/Surrogate-Ablehnung,
  Semantikinvarianz/-änderung, proof-v1-Bindungen inklusive
  Cross-Puzzle-Copy, stale Lösungshash, Metrikänderung und `proofRef`,
  Determinismus der Proofgenerierung, Pipeline-Stufenordnung,
  `--strict`-Semantik, Diagnosesortierung) sowie 7 Testmethoden in
  `STP.Tests.Solver.EditMode` (Proof-Ergebnismodell deterministisch für
  0-/1-/2+-Fälle, `solver-v1`-Konstante unverändert).
- **Mechanische Governance:** Testassembly `STP.Tests.Content.EditMode`
  erhielt direkte Referenzen auf die bestehenden Produktionsassemblies
  `STP.Puzzle.Domain`, `STP.Puzzle.Solver` und `STP.Editor.Content`
  (Unity-Assemblyreferenzen sind nicht transitiv); der statische
  Modulgraph-Check in `.github/workflows/unity.yml` wurde mechanisch
  gespiegelt. `.github/workflows/validate.yml` war bereits auf das
  WP-013-Manifest umgeschaltet und blieb unverändert.

### Akzeptanzkriterien

| ID | Stand | Nachweis |
|---|---|---|
| `AK-01` | Erfüllt | Trust-Anchor `e09658ba964b76d2a36b4baf07e69a2ccc26d952` (bestand bereits); Validator bestätigt `LOCAL_SCOPE PASS` gegen das byteunveränderte Manifest. |
| `AK-02` | Erfüllt | `LevelV2ParserTests`: beide Vertragsfixtures werden akzeptiert und auf gültige `PuzzleDefinition`-Objekte gemappt (inklusive Ein-Zellen-Fall); malformed JSON, Duplicate Keys, Ressourcenüberschreitungen, `additionalProperties: false`, polymorphe Typmetadaten und unbekannte `documentSchemaVersion`/`rulesetVersion` werden fail-closed mit stabilen Codes abgelehnt. |
| `AK-03` | Erfüllt | `LevelV2SemanticsTests`: Vertreter aller geforderten Familien; ID-Segmente exakt gegen `content.season`/`networkSection`/`route`/`position`, Season-1-Bereiche 5×4×12, aus der Lösung abgeleitete Randzahlen, Zeitordnung; sortierte stabile Codes; Katalogprüfungen bleiben der Cross-reference-Stufe vorbehalten. |
| `AK-04` | Erfüllt | `HashContractTests`: alle sechs dokumentierten Fixture-Hashes werden exakt reproduziert; Live-Crosscheck gegen `jcs_crosscheck.mjs` (Node.js 22) bestätigt byte- und hashgleiche Ergebnisse; unbekanntes Profil ist harter Fehler; Fließkomma-/Surrogate-Ablehnung; Semantikinvarianz (Formatierung, Schlüsselreihenfolge, `documentSchemaVersion`, `contentRevision`, Texte, Proofregeneration) und -änderung (Puzzle-ID, Ruleset, Raster, Endpoints, Counts) nachgewiesen. |
| `AK-05` | Erfüllt | `ProofBindingTests` und `PipelineTests`: Generierung aus dem unveränderten öffentlichen Puzzleinput mit `solver-v1`; bytegleicher Proof bei gleichem Input und gleicher Solverversion; Vergleich der einzigen gefundenen Lösung mit der Authoringlösung über den kanonischen Lösungshash; `solutionCount` 1; Cross-Puzzle-Copy, stale Lösungshash und Metrikänderung werden erkannt; `proofRef` bindet Artefakt-ID, Proofformat und Proofhash. |
| `AK-06` | Erfüllt | `PipelineTests`: fünf Stufen in Reihenfolge, sortierte Diagnosen, `--strict`-Semantik, keine Verdeckung früherer Fehler; jede Fixturemutation scheitert an der zuständigen Stufe mit dem zuständigen Code (Parse/Schema/Domain map/Semantic/Solver/Proof). |
| `AK-07` | Erfüllt | Lokaler Unity-EditMode-Lauf 6000.3.23f1: **141/141 PASSED** (Protokoll unten). |
| `AK-08` | Erfüllt mit dokumentiertem Umgebungsbefund | `git diff --check` besteht; `LOCAL_SCOPE PASS` mit dem WP-013-Manifest; der vollständige kanonische Selbsttest scheitert auf diesem Windows-Host an einer vorab existierenden, OS-abhängigen Negativmutation (siehe Befund B-4); auf der kanonischen POSIX-Umgebung (CI, Ubuntu 24.04) ist er nicht betroffen. |

### Validierung

- **Lokaler Unity-Nachweis (durch den ausführenden Agenten):** Headless-EditMode-Lauf
  auf Unity 6000.3.23f1 (`-batchmode -nographics -runTests -testPlatform EditMode`,
  NUnit, Lauf 2026-09-30 17:49:10Z): **141 Tests, 141 PASSED, 0 FAILED, 0 SKIPPED,
  0 INCONCLUSIVE**. Aufschlüsselung: 69 `STP.Tests.Content.EditMode` (66 WP-013-neu
  zuzüglich Anteilen der Assembly), 17 `STP.Tests.Solver.EditMode` (10 WP-009 plus
  7 WP-013-neu), 54 `STP.Tests.Domain.EditMode` (WP-009, unverändert) und 1 vom
  Addressables-Paket mitgelieferter Doc-Example-Test. Ein erster Lauf endete mit 4
  Testfehlern in den neuen Tests (Tiefenlimit-Erwartung um eins daneben, zwei
  Bindungstests mit zusätzlich erwarteter `proofRef`-Kopplung, ein überzogener
  Formatanspruch an generierte Proofs bei rein deduktiven Läufen);
  Produktionscode war davon nicht betroffen; nach Testkorrekturen ist der Lauf
  vollständig grün.
- **Unabhängiger JCS-Crosscheck:** Der EditMode-Test
  `Crosscheck_NodeToolProducesIdenticalResults` lief live gegen Node.js 22 und
  `tools/architecture-validation/jcs_crosscheck.mjs` (kanonische Bytes und
  SHA-256 identisch für alle Projektionen beider Fixtures); ohne erreichbaren
  Node-Interpreter meldet der Test ehrlich Inconclusive statt zu simulieren.
- **Kanonischer Scope-Lauf:** `validate.py --scope production --scope-manifest
  tools/architecture-validation/scopes/WP-013.production.scope.json --self-test`:
  alle Architektur- und Dokumentgruppen PASS einschließlich **`LOCAL_SCOPE PASS`**
  (reale Diffmenge ausschließlich innerhalb der Manifest-Allowlist; Manifest
  byteunverändert; `SCOPE_CONTEXT workPackage=WP-013 base=767c01e… head=7c0d7b5…`).
  `git diff --check` gegen den `baseCommit` besteht.
- **Nicht ausführbare Nachweise (wahrheitsgemäß NOT_EXECUTED):** Der GitHub-Check
  `Architecture Validation / validate` für den Abschlusscommit kann vom
  ausführenden Agenten nicht ausgewertet werden (kein lesender GitHub-Zugriff in
  dieser Umgebung); die CI-Konfiguration ist unverändert und läuft auf Ubuntu
  24.04. GitHub-Unity-CI (Compile/EditMode/PlayMode/Android/iOS) bleibt wie in
  WP-008/WP-009 dokumentiert ohne Lizenz/Runner NOT_EXECUTED und ist kein
  Blocker für die weitere Spieleentwicklung.

### Technische Befunde (keine WP-013-Blocker)

- **B-1 Fixture-Proofmetriken vs. solver-v1-Metriken.** Die in der
  Architekturphase handgesetzten Metrikwerte der proof-v1-Fixtures
  (`searchNodes` 1, `deductionSteps` 9/1, `maxDeductionDepth` 4/1) stammen aus
  keinem Lauf des WP-009-Solvers; solver-v1 meldet für dieselben Fixtures
  `searchNodes` 0, `deductionSteps` 46/11, `maxDeductionDepth` 6/3. WP-013
  verlangt nach eigener Testliste Bindungs- und Determinismusnachweise, keinen
  Regenerationsgleichlauf; die Pipeline prüft deshalb Format, Bindungen und
  Proofhash des gespeicherten Artefakts sowie Eindeutigkeit und Lösungshash
  frisch, ohne die Fixture-Metriken mit solver-v1 gleichsetzen zu müssen
  (Solver- und Fixtureseite durften beide nicht geändert werden). Ein
  `PRF-NONDETERMINISTIC`-Regenerationsvergleich gehört zur späteren
  Katalog-/Quality-CI und muss diese Zählweitendifferenz dann adressieren.
- **B-2 Metrikminimum `searchNodes`.** Das proof-v1-Schema verlangt
  `searchNodes >= 1`; solver-v1 meldet für rein deduktiv (ohne Suchannahme)
  lösbare Puzzles wahrheitsgemäß `searchNodes = 0`. Generierte Proofs solcher
  Puzzles tragen den wahren solver-v1-Wert und erfüllen damit das
  Artefaktminimum nicht; bei Läufen mit Suchannahme ist der generierte Proof
  vollständig format- und bindungsgültig (beides testiert). Keine der beiden
  Seiten durfte in WP-013 geändert werden (keine neue Solverversion, keine
  Architekturänderung); die Auflösung ist einer späteren Entscheidung
  vorbehalten.
- **B-3 `maxDeductionDepth` additiv implementiert.** Die dokumentierte Metrik
  war bislang nicht berechnet; die gewählte Prämissentiefen-Buchführung ist im
  Code dokumentiert. Sie ändert weder Lösungen, Klassifikationen noch
  bestehende Metrikwerte (WP-009-Tests unverändert grün).
- **B-4 Windows-Selbsttest-Artefakt des Architekturvalidators.** Die
  Negativmutation `V03-005-ABSOLUTE` setzt POSIX-Pfadsemantik voraus
  (`Path("/tmp/scope.json")` ist nur unter POSIX absolut); sie schlägt auf
  diesem Windows-Host fehl und reproduziert identisch auch ohne den
  WP-013-Diff (verifiziert auf dem unveränderten Remote-HEAD `7c0d7b5…`).
  Eine zweite Umgebungsweiche (`adr:016-historical-decision-mutated` durch
  locale-abhängige Subprocess-Dekodierung) ist mit `PYTHONUTF8=1` behoben.
  Beide Punkte betreffen ausschließlich die lokale Windows-Ausführung; die
  kanonische Umgebung (README: Linux/macOS; CI: Ubuntu 24.04) ist nicht
  betroffen. Eine Korrektur des Validators ist selbst nicht Teil von WP-013
  (Governance-Tooling außerhalb der Manifest-Allowlist).

### Verbleibende Punkte (keine WP-013-Blocker)

- Generator, Generatorvalidierung, Levelauthoring, Editorfenster, die 240
  konkreten Rätselinstanzen, Zeitwertkalibrierung, Kataloge, Lokalisierung,
  deterministischer Import sowie die Pipeline-Stufen Cross-reference, Quality
  und Import folgen in späteren Work Packages.
- `BLOCKER-PROD-001/002/003` bleiben unverändert offen und fail-closed.
- GitHub-Unity-Buildnachweise (WP-008 B-02) bleiben offen und blockieren die
  weitere Spieleentwicklung nicht.
- Nächster vorgesehener Schritt gemäß `PROJECT_CONTROL/WORK_QUEUE.md`:
  Generator und Levelauthoring in eigenen, noch zu vergebenden Work Packages.
