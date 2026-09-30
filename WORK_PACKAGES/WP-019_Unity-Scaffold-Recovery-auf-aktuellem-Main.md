# WP-019 – Unity-Scaffold-Recovery auf aktuellem Main

## ID

`WP-019`

**Bearbeitungsstatus:** **Zurückgezogen vor Implementierungsbeginn am 2026-09-30.** Dieses Dokument wurde auf `main` vor einem eigenen gemeinsamen Work-Package-/Scope-Manifest-Add-Commit persistiert. Es kann deshalb die historische Trust-Anchor-Bindung aus ADR-030 nicht erfüllen und ist **kein ausführbarer Implementierungsauftrag**. Die technische Absicht dient nur als Planungsarchiv; ein neues, erst gemeinsam mit eigenem Manifest verankertes WP-021 ersetzt diese Planung. Es wurden keine Produktionsdateien, Branches, CI-Nachweise oder Freigaben aus diesem Dokument erzeugt.

## Ziel

Auf einer **neu vom nach WP-014 und WP-018 aktualisierten `main` abgezweigten Branch** wird das Unity-6.3-Produktionsscaffold sauber wiederhergestellt und unabhängig nachgewiesen. Der historische WP-008-Branch ist dabei nur ein lesbarer Vergleichskorpus; weder sein Commitgraph noch sein Scope-Manifest, seine CI-Ergebnisse oder seine Dateien werden gemergt, gerebased oder gecherry-picked.

Der Lieferumfang umfasst Unity 6000.3.23f1, gepinnte Projekt-/Paketkonfiguration, den normativen Assemblygraphen, Testassemblies an den in `ARCHITECTURE/MODULE_BOUNDARIES.md` festgelegten Pfaden unter `Assets/StammstreckenPuzzle/Tests/`, QA-Szene, Bootstrap-Composition-Smoke und echte CI-Nachweiswege. Er enthält keine Puzzlefachlogik.

## Voraussetzungen

1. WP-014 und WP-018 sind nach `main` integriert; die neue Branch startet von diesem konkreten, unveränderten `main`-Commit.
2. [`AGENTS.md`](../AGENTS.md), [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md), [`ARCHITECTURE/TECH_STACK.md`](../ARCHITECTURE/TECH_STACK.md), [`ARCHITECTURE/BUILD_AND_RELEASE.md`](../ARCHITECTURE/BUILD_AND_RELEASE.md), [`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md), ADR-001, ADR-002, ADR-012, ADR-013, ADR-018, ADR-026 und ADR-030 sind gelesen.
3. Der historische WP-008-Auftrag und sein Manifest wurden ausschließlich als Vergleich auf `origin/feat/wp-008-unity-scaffold` gelesen. Vor dem ersten Produktionsartefakt wird eine Vergleichsmatrix erstellt, die jeden übernommenen technischen Zweck gegen den aktuellen Architekturvertrag und keine historische Branchautorität bindet.
4. Dieses Work Package und sein **neues** Production-Scope-Manifest werden im selben Trust-Anchor-Commit eingeführt. Dessen `baseCommit` ist der Elterncommit dieses gemeinsamen Ankers auf der neuen Main-basierten Branch. Der Manifestblob bleibt danach byteunverändert.
5. `secrets.UNITY_LICENSE` sowie die dokumentierten Linux/Android- und macOS/iOS-Runner müssen für einen Abschlussnachweis verfügbar sein. Fehlen sie, ist WP-019 blockiert und darf weder abgeschlossen noch als Basis für WP-020 integriert werden.

## Scope

1. Das Unity-Projekt mit exakt Unity `6000.3.23f1`, C# 9, Nullable, Force Text, Visible Meta Files und gepinnten Paket-/Projektlocks neu anlegen.
2. Den vollständigen normativen `.asmdef`-Graphen inklusive Testassemblies an der aktuellen, architektonisch festgelegten `Assets/StammstreckenPuzzle/Tests/`-Struktur herstellen. Domain/Solver/Application bleiben compilerfähige Skelette ohne Fachlogik.
3. Den Bootstrap-Composition-Root, die benötigten neutralen Port-/Adapter-/Präsentationsskelette, QA-Szene und den ausführbaren `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` implementieren, ausschließlich soweit ARCHITECTURE/ADR-018 dies fordert.
4. Die Unity-CI auf dem aktuellen Workflowstand ergänzen: Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile; SHA-gepinnte Actions und Least Privilege bleiben erhalten.
5. Trust Anchor, aktuellen Scope-Lauf, Toolchain-Lock, tatsächliche Geräte-/Runnerverfügbarkeit sowie Coverage-/Mutation-Baseline dokumentieren; alle Nachweise neu ausführen.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `ProjectSettings/**`, `Packages/*`, `Assets.meta`, `Assets/StammstreckenPuzzle/**` | Neu: Unity-Projekt, Meta-Dateien, Assembly-/Skelettstruktur, QA-Szene und Testassemblies an den normierten Assets-Pfaden. |
| `Toolchain.lock.md` | Neu: vollständiger Toolchain-Lock. |
| `.github/workflows/validate.yml`, `.github/workflows/unity.yml` | Scope-Umschaltung und echte Unity-CI-Nachweise auf Grundlage des aktuellen Main-Workflows. |
| `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md` | Nur mechanische WP-019-Inventar-/Befehlsnachführung, ohne Regelabschwächung. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Mechanische Recovery-Status- und Basisnachführung. |
| `WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md`, eigenes WP-019-Manifest | Auftrag, Trust Anchor, Vergleichsmatrix, Evidenz und Abschlussnachweis. |

## Ausdrücklich nicht erlaubte Änderungen

- Kein Merge, Cherry-Pick, Rebase oder Konfliktauflösung aus `feat/wp-008-unity-scaffold`; keine Änderung dessen historischer Dateien oder Manifeste.
- Keine Puzzle-Domain-, Solver-, Command-, Completion-, Level-v2-, JCS-, Proof-, Pipeline-, Generator- oder Editorfachlogik. Diese gehören frühestens zu WP-020/WP-015/WP-016/WP-017.
- Keine Produktentscheidung, keine ADR-Änderung, keine Änderung offener Produktblocker oder von Architekturverträgen.
- Keine SDK-Aktivierung, Consent-, IAP-, Ads-, Analytics-, Persistenz- oder fertige UI-Funktion.
- Kein Abschluss ohne alle realen Unity-/CI-Nachweise; keine simulierten oder aus dem historischen Branch übernommenen PASS-Aussagen.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Neuer Main-basierter Trust Anchor und neues WP-019-Manifest sind gemeinsam eingeführt, historisch gebunden und byteunverändert; kein historisches WP-008-Manifest wird verwendet. |
| `AK-02` | `ProjectVersion.txt` ist exakt `6000.3.23f1`; C# 9, Nullable, Force Text, Visible Meta Files, Paketlocks und Toolchain-Lock entsprechen den aktuellen Verträgen. |
| `AK-03` | Alle in `MODULE_BOUNDARIES.md` normierten Produktions- und Testassemblies existieren mit exakt zulässigen Referenzen, azyklischem Graph, `noEngineReferences` für Domain/Solver/Application und Testpfaden unter `Assets/StammstreckenPuzzle/Tests/`. |
| `AK-04` | QA-Szene und Bootstrap-PlayMode-Smoke prüfen den vollständigen Composition Graph, genau ein Portbinding, fehlende/falsche Bindungen fail-closed und keinen Providerstart. |
| `AK-05` | Unity Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile laufen commitgebunden erfolgreich in der vorgesehenen CI; Runner-/Lizenznachweis ist reproduzierbar dokumentiert. |
| `AK-06` | Coverage-/Mutation-Baseline und Geräte-/Device-Farm-Zustand sind aus tatsächlich ausgeführten Läufen wahrheitsgemäß dokumentiert. |
| `AK-07` | Vergleichsmatrix, Scope-Diff, Secret-/Produktquellenprüfung und `git diff --check` zeigen ausschließlich den aktuellen WP-019-Scope; kein historischer WP-008-Code ist durch Git-Historienoperation übernommen. |

## Tests

1. Architecture- und WP-019-Production-Scope-Validator jeweils mit `--self-test`.
2. Trust-Anchor-/Manifestblob-/historischer-Link-Nachweis gemäß ADR-030.
3. Unity Compile, alle EditMode-Tests, PlayMode-Ausführung der QA-Szene, Android-Development-IL2CPP und iOS-Export/Compile in CI.
4. Statischer Assemblygraph- und Referenzcheck, `git diff --check`, vollständige Scope-/Secret-/Produktquellenprüfung und Vergleichsmatrix-Review.
5. Astra- und Sol-QC unabhängig auf demselben eingefrorenen PR-Head.

## Risikoklasse

**Hoch.** Erstes integrierbares Produktionsscaffold berührt Unity-Toolchain, Build-/CI-Nachweise und die vollständige Assemblygrenze.

## Definition of Done

Alle Kriterien und Tests einschließlich aller realen Unity-/CI-Nachweise müssen PASS sein; fehlende Lizenz oder Runner ist ein Blocker, nicht eine Ausnahme. Astra und Sol dürfen keine offenen BLOCKER/HIGH melden. Erst dann wird ein separater PR nach `main` integriert und WP-020 freigegeben.

## Rollen und Übergabe

- **Implementierung:** Kimi oder gleichwertiger Agent implementiert nur diesen Scope auf neuer Main-basierter Branch.
- **Astra-QC:** unabhängiger Architektur-/Assemblygraph-/Composition-Review.
- **Sol-QC:** unabhängiger Scope-/CI-/Evidenz-/Integrationsreview.
- Historische WP-008-Artefakte sind Vergleichsmaterial, niemals Autorität oder direkte Lieferquelle.
