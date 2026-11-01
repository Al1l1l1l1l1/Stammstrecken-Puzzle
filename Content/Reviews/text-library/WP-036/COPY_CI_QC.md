# WP-036 – unabhängige CI-Evidenzprüfung am Freeze `89c8`

**Berichts-ID:** `WP-036-INDEPENDENT-CI-EVIDENCE-01`
**Prüfsprache:** Deutsch
**Prüfrolle:** Gesonderte unabhängige CI-Evidenzprüfung; weder Autor-, Quellenreview- noch Ownerrolle.
**Prüfgegenstand:** Tatsächliche GitHub-Actions-Evidenz für Commit `89c8d2b90e08ad7b1ba01d5b122763d2ae1fdd8b` und die kanonische Quelle `Content/Localization/source/de-DE.json`, SHA-256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c`.

> **CI-Urteil:** **PASS für die vollständige Push-Ausführung und beide Architecture-Prüfungen.** Architecture-Push und Architecture-PR sind SUCCESS; Unity-Push ist COMPLETED/SUCCESS mit allen zehn Jobs SUCCESS. Der getrennte Unity-PR-Lauf ist dagegen COMPLETED/CANCELLED und **kein vollständiger PR-PASS**. Sein Integrationsnachlauf bleibt separat offen. Es wird ausdrücklich **kein** pauschales Urteil „22 Checks PASS“ und keine Freigabe des gesamten WP behauptet.

## 1. Methode, Bindung und Integrität

Die vorgegebene Lesefolge (`AGENTS.md`, 15, 00, 09, `CURRENT_STATE.md`, WP-036) sowie `CREATIVE_FINAL_DECISION.md`, `COPY_FINAL_QC.md`, `COPY_FORMAT_FINAL_QC.md`, `CI_QC.md` und `HANDOVER.md` wurde vor der Evidenzbewertung gelesen. Daraus wurde kein früheres CI-Ergebnis übertragen.

Die vier Runs, die Jobmetadaten, sechs vollständige Erfolg-Joblogs und die drei tatsächlich auf dem erfolgreichen Unity-Push vorhandenen Artefakt-ZIPs wurden erneut read-only über die GitHub-API bezogen. Binäre ZIP-Antworten wurden direkt in Dateien geschrieben, nicht über Terminalausgabe oder ANSI-Filter geleitet. GitHub-JSON und Logs enthielten CSI-ANSI-Sequenzen; diese wurden nur für das Parsen beziehungsweise die Textsuche entfernt. Die Originalbytes, ihre SHA-256-Werte und alle daraus abgeleiteten Werte bleiben im ignorierten Prüfbereich `.utmp/wp036-ci-final/`; signierte Download-URLs wurden nicht gespeichert.

Alle vier erneut gelesenen Runmetadaten binden an denselben `head_sha` `89c8d2b90e08ad7b1ba01d5b122763d2ae1fdd8b` und `run_attempt` 1. Die originäre Commitmetadatenabfrage bestätigt Tree `49dd2380effea732e8e1911013127633cc1d753c`, Parent `b181fecbf001397292ef0c90be1b4c2d4fb2711e` und Commitnachricht `fix(review): remove trailing whitespace without altering QC verdicts`. Der Quellenblob dieses Commits wurde lokal read-only erneut zu dem vorgegebenen SHA-256 gehasht.

## 2. Tatsächliche Runs und Jobzustände

| Workflow | Ereignis | Run | Terminalstatus | Tatsächlich belegte Joblage |
|---|---|---:|---|---|
| Architecture Validation | Push | `39076203467` | COMPLETED / SUCCESS | Job `117523759999` (`validate`) SUCCESS |
| Architecture Validation | Pull Request | `39076206173` | COMPLETED / SUCCESS | Job `117523768694` (`validate`) SUCCESS |
| Unity CI | Push | `39076203485` | COMPLETED / SUCCESS | **10/10** Jobs SUCCESS, einschließlich Compile/EditMode `117523834281`, PlayMode `117526009699`, Android-IL2CPP `117527281259` und iOS-Export `117530049555` |
| Unity CI | Pull Request | `39076206176` | COMPLETED / CANCELLED | sechs Vorprüfungen SUCCESS; Compile/EditMode `117523819618` CANCELLED; PlayMode `117523836835`, iOS `117523837081` und Android `117523837176` SKIPPED |

Der PR-Unitylauf ist daher geteilt: Seine sechs Vorprüfungen sind positive Teilbelege, aber ein cancellter Compile-Job plus drei übersprungene Folgejobs ersetzen weder EditMode noch PlayMode noch die beiden Plattformpfade. Es wurde keine Ursache der Stornierung behauptet. Ein späterer Abruf des spezifischen cancellten Joblog-Endpunkts lieferte HTTP 404; die vorhandene Origin-Jobmetadatenlage belegt den Zustand, nicht dessen Ursache. Weder diese Rolle noch der Parent hat CI gestartet, wiederholt, abgebrochen oder geändert.

## 3. Architecture: beide tatsächlichen Logs

Die separat bezogenen Originaljoblogs wurden gehasht und gegen ihre zugehörigen SUCCESS-Jobmetadaten gebunden.

| Ereignis | Job | Original-Log SHA-256 | Tatsächlicher Logbefund |
|---|---:|---|---|
| Push | `117523759999` | `6ef355e2e0f0149c02ab02cad1ffffd052fa0664fe7427109ef64782602f17f2` | Architecture-only: `RESULT PASS (17 lokale Prüfgruppen)`; Node-JCS: 12 Kanonisierungsfälle, 32 verworfene Inputs, 2 v2- und 2 v1-Vektoren; Text: `entries=1408`, `season1=960/960`, `internal=240`, `mutations=12`; exakte Lesefassung PASS; WP-036-Scope: `LOCAL_SCOPE PASS`, danach `RESULT PASS (18 lokale Prüfgruppen)`. |
| Pull Request | `117523768694` | `e791b1bfb0b68dad0851d67208f66ba36c36cb4c4a5302933ce5c746b1499c66` | Derselbe tatsächliche Befund: Architecture 17, JCS 12/32/2/2, Text 1408/960/240/12, exakte Lesefassung und Scope 18 PASS. Der Log nennt zusätzlich `head=89c8d2b90e08ad7b1ba01d5b122763d2ae1fdd8b`, `base=d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`, `worktreeDirty=false`. |

Damit sind beide Architecture-Checks nicht nur als Metadaten-SUCCESS, sondern durch ihre jeweiligen Originallogs mit den angeforderten Scope-, Text- und Lesefassungsresultaten belegt.

## 4. Vollständiger Unity-Push: Tests, Coverage und Artefakte

Der erfolgreiche Push-Run weist alle zehn tatsächlichen Jobs als SUCCESS aus. Die nachstehenden drei Artefakte sind zugleich die vollständige Artefaktliste dieses Runs; ein iOS-ZIP ist dort nicht enthalten.

| Artefakt | ID | API-Größe / beobachtete ZIP-Größe | GitHub-Digest = beobachteter SHA-256 | Inhalt und eigenständig geprüfter Befund |
|---|---:|---:|---|---|
| `editmode-results` | `12117352191` | 421.489 / 421.489 Bytes | `d4f423d369ffcbdfeb56b54867e9432123538928e305ed20a4e46e39c63870db` | `editmode.xml`: SHA-256 `cc848105df4491b8ab6566de1c4c7695039f32720e18a09f7c2f4326e2e14df9`; Root `test-run`, total/passed `454/454`, failed/inconclusive/skipped jeweils 0; alle 454 `test-case`-Elemente `Passed`. |
| `playmode-results` | `12117431560` | 346.909 / 346.909 Bytes | `83e45c8d7929e1721bca3a3f403fa0116d387fddab246be3054a6d65852c572e` | `playmode.xml`: SHA-256 `cd4bee9d6231768baf51b5bfdba6cb79152dcc877f22cfe947ff9aea6675b778`; Root `test-run`, total/passed `27/27`, failed/inconclusive/skipped jeweils 0; alle 27 `test-case`-Elemente `Passed`. |
| `android-development-il2cpp` | `12118250758` | 38.948.984 / 38.948.984 Bytes | `88d0b4ec9d550763d0e8acb545b57e465c6aaa513f590cb89d7c4f495517e8f7` | ZIP enthält `stp-qa-development.apk`, unkomprimiert 40.066.242 Bytes, SHA-256 `fe7239d67188aae201331a771253ff0a471cf07dbb4e1024bfc01752bd69d2e1`. |

Die Testzahlen werden ausschließlich wegen der tatsächlich gelesenen NUnit-XMLs bestätigt, nicht aus Jobnamen oder Metadaten abgeleitet.

Die EditMode-ZIP enthält außerdem zwei echte OpenCover-Sitzungen. `TestCoverageResults_0000.xml` (SHA-256 `c4f69aac0ea40fdc792d3424b0c7cbe3ff3ff03a54b4befef535efa23bfdd15c`) und `TestCoverageResults_0001.xml` (SHA-256 `662b985d4cf652d59a0e149c2f864dd7405980409b10a579cc8569b38bf131dd`) nennen jeweils 15 `STP.*`-Module. `_0000` trägt die Gesamtbasis von 10.410 Sequence Points und 1.491 Methoden; `_0001` trägt dazu 9.781 besuchte Sequence Points und 1.388 besuchte Methoden. Die gemessenen Module mit Besuchspunkten sind `STP.Editor.Content`, `STP.Infrastructure.Content`, `STP.Puzzle.Domain`, `STP.Puzzle.Solver`, `STP.Tests.Content.EditMode`, `STP.Tests.Domain.EditMode` und `STP.Tests.Solver.EditMode`; die übrigen Module sind in der 15er-Liste ebenfalls tatsächlich enthalten. Dies ist ein Evidenzbefund, **keine neu eingeführte Mindestcoverage**.

Der Android-Originaljoblog `117527281259` hat SHA-256 `e5c5a2763dac93141e427e4eff53c29bc3618aba18ad4f2e46a222dd5646b46a` und enthält `Build Finished, Result: Success.` sowie `STP Build PASS: Android -> Builds/Android/stp-qa-development.apk`. Das APK wurde nicht installiert oder auf einem Gerät geprüft. Der Logwert 683.444.710 Bytes wird nicht als APK-Dateigröße ausgegeben; maßgeblich für die tatsächlich gelieferte APK-Größe ist der oben geprüfte ZIP-Eintrag.

## 5. iOS: Export und Xcode-Build ohne Phantomartefakt

Der iOS-Job `117530049555` ist laut Origin-Metadaten SUCCESS; sein Originaljoblog hat SHA-256 `e6d8a9dfa07c9d4029a92197c1091ba825bfb41d466cb3acef3ed019a33d6f3f`. Der Log belegt einen erfolgreichen Unity-iOS-Export (`Build Finished, Result: Success.` und `STP Build PASS: iOS -> Builds/iOS/Xcode`) sowie den anschließenden Xcode-Build. Die aufgezeichnete `xcodebuild`-Invocation verwendet `/Applications/Xcode_26.3.app/...`, `-project Builds/iOS/Xcode/Unity-iPhone.xcodeproj`, Ziel `generic/platform=iOS` und `CODE_SIGNING_ALLOWED=NO`; die tatsächliche Endzeile lautet `** BUILD SUCCEEDED **` am 17.10.2026 um 23:25:52Z.

Es existiert kein iOS-ZIP in der tatsächlichen Artefaktliste des vollständigen Push-Runs, und dieser Bericht behauptet daher weder ein iOS-Binärartefakt noch IPA-Größe, Installation, Signing oder Store-Eignung. Der Logpfad und das Xcode-Bauergebnis sind der eigenständig gelesene iOS-Nachweis.

## 6. Schlussgrenze und offene Folge

**Positiv und standgebunden belegt:**

- vollständiger Unity-Push `39076203485`: zehn Jobs SUCCESS, echte EditMode- und PlayMode-NUnit-XMLs mit **454/454** beziehungsweise **27/27** Passed, Android-APK im Originalartefakt sowie Android- und iOS-Bauerfolg;
- Architecture-Push `39076203467` und Architecture-PR `39076206173`: jeweils SUCCESS und jeweils durch Originallogs mit Architecture 17, Scope 18, JCS 12/32/2/2, Text 1408/960/240/12 und exakter Lesefassung belegt.

**Nicht positiv behauptet:** Unity-PR `39076206176` ist CANCELLED, nicht vollständig bestanden. Ein möglicher Integrationsnachlauf muss als eigener tatsächlicher Lauf auf seinem tatsächlichen Commit und mit vollständigen Jobresultaten geprüft werden. Dieser Bericht sagt weder „22 Checks PASS“ noch „WP-036 insgesamt freigegeben“.

Die kanonische Quelle und alle 1.408 Einträge bleiben DRAFT. Die kreative Geschäftsführerrestannahme ist keine Ownerannahme. Keine Aussage dieses Berichts ersetzt oder erteilt eine Owner-, Produkt-, Runtime-/Import-, Cross-Reference-, Rechts-, Asset-, Geräte-, SDK-, Datenschutz-, Signing-, Store- oder Releasefreigabe. Offene Hint-/Daily-/Endless-, Item-/Store-/Layout-/Zeit-, WP-026- und sonstige Fremdgates bleiben unberührt; `main` bleibt unverändert. Es wurden keine Quellen, Reviewhistorien, Tools, Workflows, Manifeste, Statusdateien oder Gitobjekte geändert und keine Unity-/Solver-Builds doppelt ausgeführt.
