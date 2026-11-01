# WP038 – tatsächliche Integrations-CI-Eigenbelege

Implementierungs-/CI-Evidenzsupport, keine unabhängige Abschlussrolle. Tatsächlich integrierter WP038-Stand `07ba4d3130414ee82e843823c2f04aeac230f25b`, Tree `f1adfa4e006169c32bd930cc6c76f1d62b98e780`. Reguläres CI-Ergebnis **PASS**; dokumentierter unabhängiger Abschlussstand **INTEGRATION_NOT_EVALUATED_BY_AUTHOR_ROLE**. Keine Integrationsfreigabe.

- Eigene CI-Evidenzsammlung des tatsächlichen integrierten Stands; keine unabhängige Quellen-, Unterintegrations- oder Mainfreigabe. C1- und C2-Pakete bleiben unverändert, neue Ereignisse/Originale werden vollständig getrennt gebunden.

| Run | Workflow / Ereignis | Tatsächlicher Zustand | Jobs |
|---|---|---|---|
| 39179259658 | Architecture Validation / pull_request | completed / success | 1 |
| 39179259987 | Unity CI / pull_request | completed / success | 10 |
| 39179256058 | Architecture Validation / push | completed / success | 1 |
| 39179256064 | Unity CI / push | completed / success | 10 |

Die vollständige frische Workflowabfrage für diesen Commit wird mit jedem tatsächlich ausgeführten Ereignis separat abgeglichen; Push- und PR-Läufe ersetzen einander nicht. Synthetische PR-Checkoutobjekte benötigen eine eigene tatsächlich gelesene SHA-/Tree-Bindung.

| Run | Original-XML | Ausgeführte Fälle | Tatsächliche Ergebnisse |
|---|---|---|---|
| 39179259987 | editmode.xml | 474 | {"Passed": 474} |
| 39179259987 | playmode.xml | 49 | {"Passed": 49} |
| 39179256064 | playmode.xml | 49 | {"Passed": 49} |
| 39179256064 | editmode.xml | 474 | {"Passed": 474} |

| Original-Artefakt | STP-Assemblies | Coverageprüfung |
|---|---|---|
| 12156041597 / editmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |
| 12156017712 / playmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |
| 12155733748 / playmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |
| 12155214594 / editmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |

Die erforderlichen Application-, Presentation- und Bootstrapmethoden einschließlich der Screenkorrektur werden ausschließlich aus den eingefrorenen integrierten Testsourcebytes abgeleitet und je tatsächlichem Unityereignis einzeln an neue Original-XML-Fälle gebunden. Die vorhandene Composition-Smoke bleibt in jedem Lauf Pflicht. Grafikproben im regulären Null-Renderer-Lauf liefern keinen PNG-Nachweis; ihre semantischen Testergebnisse ersetzen den separat dokumentierten nativen Grafiklauf nicht.

| Plattformjob | Tatsächlich verarbeitete QA-Szenen |
|---|---|
| 117832803761 / unity-android-development-il2cpp | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |
| 117840270019 / unity-ios-export | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |
| 117831977660 / unity-android-development-il2cpp | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |
| 117836379989 / unity-ios-export | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |

| Original-ZIP | Bytes | SHA256 (= GitHub-Artefaktdigest) |
|---|---|---|
| 12156948095 / android-development-il2cpp | 45584989 | `7f0ca145cebda6fb0af55c2c35b761f63efdd0c5c5b3ef290812b23764254682` |
| 12156041597 / editmode-results | 498191 | `3ed8fffe6ad8c1d1d21e204046497cf1c9269501ab055cb326c40f58628e0e59` |
| 12156017712 / playmode-results | 432480 | `a93e0372901404dc31e6f0fc614c1964f29726a761d15e630d952d4b94f3f5e5` |
| 12156920934 / android-development-il2cpp | 45584967 | `01bee9ebc886b1ab39bf72229c56e087b3e71ce24b2c71cbafabed13f03485f1` |
| 12155733748 / playmode-results | 432512 | `78d8f934371798f2bc79a9e051d07f50738fd05ed8762241a102696ade87068c` |
| 12155214594 / editmode-results | 498135 | `a0e63820a28401a464cf10d24cb358afd785af81a50403ae7359b27207762bef` |

Originale Status-/Job-/Artefaktmetadaten, tatsächliche Checkoutfolgen, Original-XMLbytes, Coverageberichte und alle ZIP-/Memberdigests stehen in [INTEGRATION_CI_EVIDENCE.json](INTEGRATION_CI_EVIDENCE.json). Beide tatsächlichen BuildReport-Szenenbelege werden getrennt für Android und iOS sowie je Ereignis geprüft. Lokale unveränderte ZIPs und decodierte Originallogs liegen ignoriert unter `.utmp/wp038/ci-integration`; persistierte vollständige Logprofile sind ausdrücklich account-/tokenbereinigt und separat gehasht.

- Author/implementation CI evidence support, not an independent final review or integration decision.
- Regular headless CI graphical test cases do not replace the separately executed D3D11 graphics evidence.
- Original ZIPs and exact decoded job logs remain ignored locally; persistent log profiles are explicitly redacted.
- Unsigned QA/development builds are no device, signing, store, human campaign test, product import, save, reward, main, or release approval.
- No retry, cancellation, push, workflow dispatch, Editor run, or new general solver/text review by this evidence role.
