> Persistenznachtrag der Geschäftsführung: Die nachstehende gesonderte Teilprüfung war ausschließlich lesend. Ihre ursprüngliche temporäre Ablage und Aussagen beziehen sich auf die erstellende Prüfrolle; Manus hat diesen Bericht anschließend als Entscheidungsbeleg im erlaubten Reviewpfad gesichert. Nur Zeilenendleerzeichen sind für den Git-Diffcheck normalisiert; kein Urteil oder Sachbefund geändert. Rohe Downloads bleiben im ignorierten Bereich; Run-/Artefakt-IDs, Digests und Gitobjektbindungen sind hier dauerhaft nachvollziehbar.

# WP-036 – Read-only Teilprüfung: technischer Abschluss-/Integrationsweg

**Berichts-ID:** `WP-036-READONLY-INTEGRATION-PATH-PARTIAL-AUDIT-01`
**Rolle:** Read-only Teilprüfung zur Geschäftsführerentscheidung; keine Freigabeentscheidung und keine neue dauerhafte Prüfrolle.
**Abrufzeit (UTC):** `2026-10-18T18:01:28Z`
**Methode:** GitHub API nur lesend, Git-Blobvergleich sowie gezielte Original-Log-/ZIP-/NUnit-XML-Auswertung. Keine CI gestartet/wiederholt/abgebrochen; kein Unity-Build und kein Solver-Neutest.

## Bindung und aktueller PR-Zustand

| Gegenstand | Tatsächlich festgestellter Wert |
|---|---|
| PR | #31, `codex/wp-036-vollstaendige-textbibliothek`, `open`, Draft |
| Head / Basis | `ff1836e293a2a5cff69773525c347b55deb67dfb` / `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |
| Kreativ angenommener Text-Freeze | `89c8d2b90e08ad7b1ba01d5b122763d2ae1fdd8b` |
| Kanonische Quelle | `Content/Localization/source/de-DE.json`, SHA-256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c` |
| GitHub-Merge-Metadaten beim Abruf | `mergeable=true`, `mergeable_state=unstable`; keine Freigabeaussage |

Der PR-Head ist `ff1836e…`. Die Unity-PR-Ausführung `39079578360`, im Auftrag zuvor noch **IN_PROGRESS**, war beim Abruf jetzt **COMPLETED / SUCCESS**.

## Tatsächliche CI-Lage am Head `ff1836e`

| Workflow/Ereignis | Run | Terminalstatus | Kernaussage |
|---|---:|---|---|
| Architecture / Push | `39079576265` | COMPLETED / SUCCESS | Job `117533601135` SUCCESS |
| Architecture / Pull Request | `39079578380` | COMPLETED / SUCCESS | Job `117533607493` SUCCESS |
| Unity / Push | `39079576211` | **COMPLETED / FAILURE** | EditMode-Job fehlgeschlagen; Folgejobs kaskadierend übersprungen |
| Unity / Pull Request | `39079578360` | **COMPLETED / SUCCESS** | Alle zehn Jobs SUCCESS; relevante NUnit-XMLs konkret geprüft |

Aus den aktuellen Check-Runs am Head ergeben sich **22** Zustände: **18 SUCCESS, 1 FAILURE, 3 SKIPPED**. Der spätere vollständige Unity-PR-PASS ist ein eigener positiver Nachweis; er **löscht** den zuvor tatsächlich eingetretenen Unity-Push-FAIL nicht.

## Originalbefund: fehlgeschlagener Unity-Push

| Gegenstand | Eigenständig verifizierter Wert |
|---|---|
| Fehlrun / Job | `39079576211` / `117533648377` (`unity-compile-editmode`) |
| Originalartefakt | `12118003255`, `editmode-results`, nicht abgelaufen |
| GitHub-Digest = selbst geladener ZIP-SHA-256 | `b4b6647c85ce1f371762949e10c3182907d2735785a5aac8fb84c2b54ec98968` (**gleich**) |
| ZIP-Größe gemäß API | `421701` Bytes |
| NUnit-XML | `editmode.xml`, SHA-256 `b7d5487967ac89e78c971603d350fd05fb042576982e130c4ff64bec13957d32` |
| Testergebnis aus XML | **454 total, 453 Passed, 1 Failed**, 0 inconclusive, 0 skipped |
| Fehltest | `STP.Tests.Solver.EditMode.PuzzleSolverTests.HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` |
| Dauer / Timeout | `186.063155 s`; `Timeout value of 180000 ms was exceeded. If this is intended, increase the timeout value using a TimeoutAttribute.` |
| Stacktrace in Original-NUnit-XML | leer |
| Originallog | SHA-256 `6288f84e6be6373787a83f454684f2475c8e1a1665cf55b61f37f5f262bc2ae0` |

Die Artefaktmetadaten binden das ZIP an Run `39079576211`, Branch `codex/wp-036-vollstaendige-textbibliothek` und Head `ff1836e293a2a5cff69773525c347b55deb67dfb`. PlayMode (`117535377359`), Android (`117535377216`) und iOS (`117535377809`) stehen danach auf **SKIPPED**; dies sind keine drei eigenständigen Fehler.

## Späterer terminaler Unity-PR-Nachweis – konkret, nicht als Überdeckung des FAIL

| XML/Artefakt | Selbst geprüfter Befund |
|---|---|
| PR EditMode, Artefakt `12118727323` | ZIP-SHA-256/Digest `4e442d573f6665076b0e91dd285da2f497a7aef7903dc144f31bc28babc4b8b9`; XML `7efd6adbc810a5e9a1f951a79b8b5de5b9af050be96200b0f00022846f718d85`; **454/454 Passed**, 0 Failed. Der HardBudget-Test ist Passed, Dauer `83.671989 s`. |
| PR PlayMode, Artefakt `12118881942` | ZIP-SHA-256/Digest `83eee91feb28d2a7368e8cca751cfff4fc915ed61af5ad1db685f79e54f54eb7`; XML `fdb70a8708c8b0cd107976c38d86e223301f2350f63fb025456bf38cb24fb279`; **27/27 Passed**, 0 Failed. |

Dies ist gezielte Prüfung relevanter Originalartefakte; **kein** zweites großes Build- oder Vollmodulreview.

## Strikte Trennung vom alten Freeze `89c8`

`COPY_CI_QC.md` und `COPY_CI_RUNS.json` belegen am kreativ angenommenen Freeze `89c8…`: beide Architecture-Läufe SUCCESS und Unity-Push 10/10 SUCCESS; der getrennte Unity-PR war dort **CANCELLED** und kein vollständiger PR-PASS. Das ist **nicht** der neue Befund: Der neue Lauf am `ff1836e`-Head ist ein **FAILURE** im EditMode-Test. Beide Tatsachen bleiben getrennt erhalten.

## Gitblobvergleich: keine WP-036-Änderung im Solver-/Unity-Pfad

Verglichen wurden **118** Pfade in `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/**`, `Assets/StammstreckenPuzzle/Tests/**` und `.github/workflows/unity.yml`.

| Vergleich | Ergebnis |
|---|---|
| `ff1836e` ↔ `89c8` | gleicher Pfadsatz; **0** Blobabweichungen |
| `ff1836e` ↔ `main d90e798` | gleicher Pfadsatz; **0** Blobabweichungen |
| `89c8` ↔ `main d90e798` | gleicher Pfadsatz; **0** Blobabweichungen |

Solver, Unity-Tests und Unity-Workflow sind folglich in diesen drei Ständen bytegleich. Der Quellhash von `de-DE.json` wurde unmittelbar aus den Gitblobs von `ff1836e` und `89c8` neu berechnet; beide ergeben `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c` (718456 Bytes).

## Einordnung der vorhandenen WP-034-Reparatur

Die aus dem dokumentierten WP-026-Stand gelesene WP-034-Integration belegt: WP-034 wurde als `085357d59a62dae73189ea76051c73f0ffb78ae3` in den weiterhin offenen WP-026-Zweig integriert. Der dokumentierte Reparaturumfang betrifft `RootDeductionTracer.Record`, ließ die 180-s-Grenze unverändert und wurde dort unabhängig geprüft.

Der `RootDeductionTracer.cs`-Blob ist in `dc1652…` und `085357d…` `a41a802fd3e3f2f54074bd0d1e0bf540d0ae5a6c`; in `main`/`89c8`/`ff1836e` ist er `bd719d0395aa6f5f3d6398d9ceec5d3c8fcf3eaa`. Die WP-034-Reparatur ist daher **nicht** Teil des WP-036- oder Main-Baums. Der direkte Vergleich `ff1836e…` gegen `dc1652…` umfasst in diesem eingeschränkten Reparaturbereich 15 geänderte Zeilen in `RootDeductionTracer.cs` und 12 hinzugefügte Zeilen in `PuzzleSolverTests.cs`.

> Die Gegenüberstellung belegt **keine** konkrete Host-, Scheduling- oder GC-Ursache. Sie beweist ebenso nicht, dass die WP-034-Lösung ohne einen neu autorisierten Scope am WP-036-Head passend wäre.

## Befund für die Geschäftsführungsentscheidung

**Die technische Integrationsbereitschaft ist nicht vollständig belegt.** Zwei Architecture-Pässe und ein inzwischen kompletter Unity-PR-Pass am aktuellen Head liegen vor. Daneben bleibt ein tatsächlicher Unity-Push-FAIL am selben Head mit dem konkret ausgewerteten HardBudget-Timeout. Nach `COPY_REWORK_HANDOVER.md` ist gerade dieser tatsächliche PR-/Integrationsnachlauf vor jeder Integration zu klären; technische GitHub-Mergefähigkeit allein ist keine Freigabe.

Falls die zuständige Geschäftsführungsentscheidung eine technische Korrektur zur Voraussetzung macht, ist **WP-034** eine bereits verifizierte Wiederverwendungsquelle, aber ausschließlich im offenen WP-026-Zweig. Ihre Übernahme darf weder still in WP-036 hineininterpretiert noch per pauschalem WP-026-Merge/Cherry-pick oder durch CI-Wiederholungen erfolgen. Vor Ausführung wäre ein eigenständiger, verankerter Auftrag mit Basis, Ziel, erlaubtem Scope, AK, Tests, Integrationsgrenze und unabhängiger Abschlussprüfung erforderlich. Dieser Bericht vergibt keine WP-ID und trifft diese Entscheidung nicht selbst.

## Evidenzgrenzen

- Keine kanonischen Repo-Pfade, Commits, Pushes, PR-Kommentare, WP-IDs oder Scopes geändert.
- Keine CI gestartet, wiederholt oder abgebrochen; kein Unity-Build/Solver-Neutest ausgeführt.
- GitHub-JSON enthielt CSI-ANSI: Originalbytes sind als `*.raw.json` erhalten; nur abgeleitete `*.pretty.json` wurden zum Parsen CSI-bereinigt. Originallogs sind als `*.log.raw` unverändert; `*.csi-stripped.txt` ist nur eine Suchableitung.
- Keine Owner-, Produkt-, Import-, Runtime-, Rechts-, Geräte-, Signing-, Store-, Release- oder Main-Mergefreigabe.

## Ablage

Sämtliche Rohantworten, unveränderten ZIPs/Originallogs, XML-Extrakte, Hashes, abgeleiteten Analysen sowie dieser Bericht liegen ausschließlich im ignorierten Evidenzpfad `.utmp/wp036-integration-path/ci-evidence/`.
