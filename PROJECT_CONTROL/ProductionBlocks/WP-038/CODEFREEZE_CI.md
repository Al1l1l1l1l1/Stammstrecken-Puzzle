# WP038 – tatsächliche Codefreeze-CI-Eigenbelege

Implementierungs-/CI-Evidenzsupport, keine unabhängige Abschlussrolle. Historischer Codefreeze `e77c5fa168b915979c3c9e39d45fbcfcaefa3aea`, Tree `e5f9235716d15b1ab0a74b46fac1e1cdd1aeddd0`. Reguläres CI-Ergebnis **PASS**; dokumentierter unabhängiger Abschlussstand **FAIL_SCREEN_01**. Keine Integrationsfreigabe.

- Historischer ursprünglicher Codefreeze C: unabhängige QC meldete SCREEN-01 (normaler Screen-UI-Renderpfad gegenüber World-Offscreenkamera fehlt; Panel-targetTexture-PNGprobe deckt diesen Pfad nicht ab). Ein regulärer CI-PASS entkräftet diesen konkreten AK04/06-Befund nicht. Korrektur und unabhängige Nachprüfung sind gesondert erforderlich.

| Run | Workflow / Ereignis | Tatsächlicher Zustand | Jobs |
|---|---|---|---|
| 39171716266 | Architecture Validation / push | completed / success | 1 |
| 39171716314 | Unity CI / push | completed / success | 10 |

Die vollständige frische Workflowabfrage für diesen Commit findet genau diese zwei ursprünglichen Push-Ereignisse; kein PR-Ereignis wird als ausgeführt behauptet.

| Run | Original-XML | Ausgeführte Fälle | Tatsächliche Ergebnisse |
|---|---|---|---|
| 39171716314 | playmode.xml | 48 | {"Passed": 48} |
| 39171716314 | editmode.xml | 474 | {"Passed": 474} |

| Original-Artefakt | STP-Assemblies | Coverageprüfung |
|---|---|---|
| 12153410658 / playmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |
| 12151809989 / editmode-results | 18 | Gesamtpunkte und explizite Messdaten für jede Assembly; kein Mindestprozentsatz |

Alle 19 neuen Application-, 13 Presentation- und acht Bootstrapfälle werden anhand der eingefrorenen Testmethoden und tatsächlichen Original-XML-Fälle einzeln gebunden. Die vorhandene Composition-Smoke bleibt Pflicht. Die drei Grafikproben melden im regulären Null-Renderer-Lauf ausdrücklich `GRAPHICS NOT_EXECUTED` für PNGs; ihre semantischen Testergebnisse ersetzen den separat dokumentierten D3D11-Grafiklauf nicht.

| Plattformjob | Tatsächlich verarbeitete QA-Szenen |
|---|---|
| 117808452127 / unity-android-development-il2cpp | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |
| 117812219539 / unity-ios-export | `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity`: 1 tatsächliche BuildReport-Zeile(n), `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity`: 1 tatsächliche BuildReport-Zeile(n) |

| Original-ZIP | Bytes | SHA256 (= GitHub-Artefaktdigest) |
|---|---|---|
| 12153681049 / android-development-il2cpp | 45592029 | `3a9f771a3346e5ab5480a67d26bb1a302927b5c84bdda5e19bfc767e94656f68` |
| 12153410658 / playmode-results | 423818 | `d8efa0491b28fea15dc77098512b7071d2e2727918063e6be27478a66d3f6958` |
| 12151809989 / editmode-results | 489853 | `7c5f7e3f070c5d66a3ddf1c7d15abc11d306bcd4985b8b65c7adfc2cd55f5ded` |

Originale Status-/Job-/Artefaktmetadaten, tatsächliche Checkoutfolgen, Original-XMLbytes, Coverageberichte und alle ZIP-/Memberdigests stehen in [CODEFREEZE_CI_EVIDENCE.json](CODEFREEZE_CI_EVIDENCE.json). Beide tatsächlichen BuildReport-Szenenbelege werden getrennt für Android und iOS geprüft. Lokale unveränderte ZIPs und decodierte Originallogs liegen ignoriert unter `.utmp/wp038/ci-codefreeze`; persistierte vollständige Logprofile sind ausdrücklich account-/tokenbereinigt und separat gehasht.

- Author/implementation CI evidence support, not an independent final review or integration decision.
- Regular headless CI graphical test cases do not replace the separately executed D3D11 graphics evidence.
- Original ZIPs and exact decoded job logs remain ignored locally; persistent log profiles are explicitly redacted.
- Unsigned QA/development builds are no device, signing, store, human campaign test, product import, save, reward, main, or release approval.
- No retry, cancellation, push, workflow dispatch, Editor run, or new general solver/text review by this evidence role.
