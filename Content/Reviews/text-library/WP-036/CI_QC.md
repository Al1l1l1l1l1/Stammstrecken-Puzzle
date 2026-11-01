# Unabhängiger CI-Nachtrag – WP-036

**Endergebnis: vollständige Source-Freeze-PR-CI PASS; Unity-Push-CI CANCELLED/partiell.** Diese gesonderte unabhängige Rolle hat alle vier tatsächlichen Remote-Läufe des unveränderten Produktions-Freeze `af6eb73eebc73bb3662569b2f5f610734e68b6f5` bis zu ihren terminalen Ergebnissen geprüft. Beide Architecture-Läufe und sämtliche zehn Unity-PR-Jobs bestehen. Der abgebrochene Unity-Push wird nicht als bestanden behandelt. Keine pauschale Aussage „22 Checks PASS“ und kein daraus abgeleiteter vollständiger DoD-/Produktabschluss.

Prüfdatum: 17.10.2026; abschließender tatsächlicher Abfragezeitpunkt in `ci-qc.json` (UTC). Quelle SHA-256 unverändert `b45db9d6d8216de7a185b26da1edf77f5ad71b1c921ebcfbc49f9726834557be`. Die ursprüngliche lokale [QC.md](QC.md) bleibt byteunverändert, SHA-256 `b10101b0d86a95682de4f5c1dabccab64b45654d93d4a1ea465f59c545ca5343`. Der Nachtrag stammt ebenfalls von `/root/independent_qc`; es wurden keine Produktionsdateien oder Workflows geändert.

Die Run-IDs werden einzeln über den GitHub-Connector gelesen, nicht aus einem auf PRs oder neueste Workflowläufe eingeschränkten Commitwrapper abgeleitet. Tatsächliche Metadaten, Job-/Schrittzustände, selbst neu gelesene Logergebnisse und der eigene Artefaktabgleich stehen in [ci-qc.json](ci-qc.json). Keine Läufe wurden geändert, abgebrochen oder neu gestartet, keine Zugangsdaten gelesen und kein Merge ausgeführt.

| Workflow | Ereignis | Run | Eigener beobachteter Stand |
|---|---|---:|---|
| Architecture Validation | Push | 39058795864 | SUCCESS; echte Logs: Architecture17, Scope18, JCS12/32/2/2, Text1408/960/12 und Lesefassung PASS |
| Architecture Validation | Pull Request | 39058816091 | SUCCESS; dieselben tatsächlich ausgeführten Prüfgruppen und korrekter Scope-HEAD |
| Unity CI | Push | 39058795866 | Terminal CANCELLED; EditMode erfolgreich, PlayMode cancelled, Android/iOS skipped |
| Unity CI | Pull Request | 39058816058 | Terminal SUCCESS; alle zehn erwarteten Jobs erfolgreich einschließlich EditMode, PlayMode, Android-IL2CPP und iOS/Xcode |

Das Push-EditMode-Artefakt `12111155483` wurde selbst über den Connector bezogen und im ignorierten Prüfbereich gelesen. ZIP: 421.347 Bytes, SHA-256 `fad692fc72c876454d818b7f9e9a6d0a0655a62532eaf3d3094add186a5a31d9`, identisch mit dem GitHub-Digest. `editmode.xml` enthält tatsächlich **454/454 Passed**, keine fehlgeschlagenen, übersprungenen oder unentschiedenen Fälle: 379 Content, 19 Domain, 55 Solver und 1 Addressables-Pakettest. Die echte Coverage-Ausgabe bestätigt 15 STP-Assemblies; sie legt keine Mindestcoverage oder Abdeckung neuer Texte durch Unity fest.

Auch das PR-EditMode-Artefakt `12111106614` wurde selbst geladen und gelesen: 421.365 Bytes, SHA-256 `5138e1497ad76bf904644110010f3641d0e28c55b8d32eebd6d7df17d8898de6`, identisch mit seinem GitHub-Digest; ebenfalls tatsächlich 454/454 Passed und dieselben vier Assemblyzahlen.

Der PR-PlayMode-Job `117475779375` ist ebenfalls tatsächlich erfolgreich. Sein Artefakt `12110764083` wurde selbst geladen und geprüft: 346.909 Bytes, SHA-256 `cfed41ca7e9cc89f1ba98252c47bfebd3e8839125620696777401c363e6c9693`, identisch mit dem GitHub-Digest. `playmode.xml` enthält tatsächlich **27/27 Passed**, keine fehlgeschlagenen oder übersprungenen Fälle, sämtlich BootstrapCompositionSmoke einschließlich der QA-Szenenprüfung. Die Einzelfallnamen sind im strukturierten Nachweis enthalten. Das prüft vorhandene Bootstrap-/Vertragsfunktionen, keinen Runtimeimport der neuen Textbibliothek.

## Tatsächliche Plattformbauergebnisse

Android-Job `117476423121` ist SUCCESS. Der selbst neu gelesene Log enthält „Build Finished, Result: Success.“ und den tatsächlichen STP-Bauerfolg. Das neue Artefakt `12111800078` wurde selbst über den Connector geladen: ZIP **38.948.951 Bytes**, SHA-256 `0c9ef6ed74e111076884bc0c0d7560ada68d3cbfde939316cb39a803069f3f0d`, identisch mit GitHubs Digest. Sein ZIP-Verzeichnis enthält `stp-qa-development.apk` mit **40.066.238 Bytes** und Burst-Debuginformation. Das APK wurde nicht extrahiert, installiert oder auf einem Gerät getestet.

Eine zusätzliche anfängliche Größen-Gleichheitsprobe scheiterte an der Prüferannahme, der im Log genannte Wert 683.444.705 Bytes müsse die APK-Dateilänge sein. Die anschließende rein lesende Quellenkontrolle zeigt: `StpBuildEntrypoints.cs:133` protokolliert `report.summary.totalSize`. Das ist kein behaupteter FileInfo-Length-Wert. Die tatsächliche APK-Größe stammt deshalb ausdrücklich aus dem unabhängig geprüften ZIP-Verzeichnis; diese korrigierte Prüferannahme ist kein Produktionsfehler.

iOS-Job `117478099745` ist SUCCESS. Der selbst neu gelesene Log belegt **Xcode 26.3**, erfolgreichen Unity-iOS-Export und die tatsächliche Xcode-Zeile **BUILD SUCCEEDED** um `2026-10-17T19:12:19Z`. Der Build verwendet gemäß unverändertem Workflow `CODE_SIGNING_ALLOWED=NO`. Es gibt dort keinen iOS-Artefaktupload; entsprechend wird kein selbst heruntergeladenes iOS-Binärartefakt oder Store-/Signingnachweis behauptet. Der Unity-BuildReport-Wert 900.432.206 Bytes wird nicht als gemessene IPA-Dateigröße ausgegeben.

Die vier selbst geladenen und unabhängig geprüften ZIPs sind damit: Push-EditMode, PR-EditMode, PR-PlayMode und PR-Android. Ihre IDs, GitHub-Metadaten, Hashvergleiche, XML-Zahlen beziehungsweise ZIP-Einträge stehen im strukturierten Nachweis. Signierte Downloadlinks und Zugangsdaten werden dort nicht gespeichert.

## Schlussgrenze

Die terminalen Runmetadaten wurden abschließend erneut einzeln abgefragt: alle vier binden an `af6eb73…`, Versuch 1. Die erwartete vollständige Menge von zehn Unity-PR-Jobnamen wurde gegen die tatsächlichen zehn SUCCESS-Jobs verglichen. Zusammen mit Architecture-PR besteht damit die tatsächliche PR-CI. Architecture-Push besteht ebenfalls. Unity-Push enthält dagegen genau sieben erfolgreiche Jobs, einen abgebrochenen PlayMode-Job und zwei übersprungene Plattformjobs; sein Gesamtergebnis bleibt CANCELLED.

Der abgebrochene Push liefert ausschließlich partielle Evidenz; er wird weder nachträglich als bestanden umgedeutet noch in eine Behauptung „22 Checks PASS“ aufgenommen. Die vorhandene globale Single-Seat-Concurrency ist strukturell mit dem beobachteten Warteschlangenwechsel vereinbar; ein eigenständiger technischer Ursachennachweis liegt nicht vor. Keine Cancellation oder Wiederholung wurde von dieser Rolle ausgelöst.

Dieser Nachtrag prüft genau die Remote-CI des eingefrorenen Produktionsstands. Spätere reine Evidenz-/Dokumentationscommits und deren eigene CI werden dadurch nicht vorab als bestanden erklärt. Falls für den vollständigen DoD-/AK09-Abschluss auch ein erfolgreicher vollständiger Pushlauf verlangt wird, bleibt dieser Nachweis unerfüllt; eine solche Anforderung wird hier nicht eigenständig aufgehoben. Kreative Ownerannahme, menschliche Spieltests, UI-/Geräteprüfung und Produkt-/Import-/Rechts-/Releasegates bleiben unabhängig vom PR-CI-Erfolg offen. Kein Merge.
