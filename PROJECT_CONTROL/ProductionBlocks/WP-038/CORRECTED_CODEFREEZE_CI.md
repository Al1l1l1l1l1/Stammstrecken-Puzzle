# WP038 – tatsächliche korrigierte Codefreeze-CI-Eigenbelege

Implementierungs-/CI-Evidenzsupport, keine unabhängige Abschlussrolle. Korrigierter Sourcefreeze C2 `524b45024438b93c592351d7683a7c62aec2ab8d`, Tree `f1adfa4e006169c32bd930cc6c76f1d62b98e780`. Reguläres CI-Ergebnis **PASS**; dokumentierter unabhängiger Abschlussstand **PENDING_C2_INDEPENDENT_REVIEW**. Keine Integrationsfreigabe.

- C1 e77c5fa und dessen reguläre CI-/SCREEN-01-FAIL-Nachweise bleiben unveränderte Historie. Dieser Sammler liest ausschließlich neue Originale für C2; kein C1-PASS und kein C1-Testgesamtwert wird übernommen. Technische Abschluss-/Integrationsentscheidungen liegen bei der gesonderten unabhängigen Prüf- und Integrationsrolle.

| Run | Workflow / Ereignis | Tatsächlicher Zustand | Jobs |
|---|---|---|---|
| 39175992736 | Architecture Validation / push | completed / success | 1 |
| 39175992749 | Unity CI / push | completed / success | 10 |

Die vollständige frische Workflowabfrage für diesen Commit findet genau diese zwei ursprünglichen Push-Ereignisse; kein PR-Ereignis wird als ausgeführt behauptet.

| Run | Original-XML | Ausgeführte Fälle | Tatsächliche Ergebnisse |
|---|---|---|---|
| 39175992749 | playmode.xml | 49 | {"Passed": 49} |
| 39175992749 | editmode.xml | 474 | {"Passed": 474} |

| Original-Artefakt | STP-Assemblies | Coverageprüfung |
|---|---|---|
| 12155020175 / playmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |
| 12153869990 / editmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |

Die erforderlichen neuen Application-, Presentation- und Bootstrapmethoden einschließlich der Screenkorrektur werden ausschließlich aus den eingefrorenen C2-Testsourcebytes abgeleitet und einzeln an tatsächliche Original-XML-Fälle gebunden. Die vorhandene Composition-Smoke bleibt Pflicht. Grafikproben im regulären Null-Renderer-Lauf liefern keinen PNG-Nachweis; ihre semantischen Testergebnisse ersetzen den separat dokumentierten nativen Grafiklauf nicht.

| Plattformjob | Tatsächlich verarbeitete QA-Szenen |
|---|---|
| 117820720540 / unity-android-development-il2cpp | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |
| 117824102206 / unity-ios-export | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |

| Original-ZIP | Bytes | SHA256 (= GitHub-Artefaktdigest) |
|---|---|---|
| 12155020175 / playmode-results | 432480 | `d2042e3eedf384816c7dcefc6cdea4f8796bc86ae47ed6d50856628605e935e7` |
| 12154684308 / android-development-il2cpp | 45585018 | `08f2d2cabdc53a44a15476d5ce85220dc04d26d8fcaf4a341b5e6b9a076485f5` |
| 12153869990 / editmode-results | 498158 | `14dfaa51f8eb4c16cea695b7361ea9f8ae04f539a340bdae56c0712424339762` |

Originale Status-/Job-/Artefaktmetadaten, tatsächliche Checkoutfolgen, Original-XMLbytes, Coverageberichte und alle ZIP-/Memberdigests stehen in [CORRECTED_CODEFREEZE_CI_EVIDENCE.json](CORRECTED_CODEFREEZE_CI_EVIDENCE.json). Beide tatsächlichen BuildReport-Szenenbelege werden getrennt für Android und iOS geprüft. Lokale unveränderte ZIPs und decodierte Originallogs liegen ignoriert unter `.utmp/wp038/ci-corrected`; persistierte vollständige Logprofile sind ausdrücklich account-/tokenbereinigt und separat gehasht.

- Author/implementation CI evidence support, not an independent final review or integration decision.
- Regular headless CI graphical test cases do not replace the separately executed D3D11 graphics evidence.
- Original ZIPs and exact decoded job logs remain ignored locally; persistent log profiles are explicitly redacted.
- Unsigned QA/development builds are no device, signing, store, human campaign test, product import, save, reward, main, or release approval.
- No retry, cancellation, push, workflow dispatch, Editor run, or new general solver/text review by this evidence role.
