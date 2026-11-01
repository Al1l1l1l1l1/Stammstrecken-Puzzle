# T-Freeze – tatsächlicher CI-Nachweis und offener iOS-Blocker

**Ergebnis: T-CI nicht vollständig bestanden.** Getrennte lesende Rolle `/root/entry_audit`, 19.10.2026. Architecture SUCCESS; Unity terminal FAILURE wegen eines konkret beobachteten Unity-Editor-Downloadfehlers im iOS-Job. Neun von zehn Unityjobs sind SUCCESS. Kein T06-/T-Gesamt-PASS, keine A/S-/I- oder Produktfreigabe.

## Exakter Gegenstand

Veröffentlichter Freeze `54d2496ac2c74b681d24fe7a69d7f044392c46dd`, Tree `85d1cdb94beaa440cca600e16a1bf6ac06cf128e`, einziger Parent `4929ad8be779c2def5d7b2b504add9346ee5a389`. GitHub-Git-Commit-API und lokales Git stimmen überein. Branch `codex/wp-040-public240-save-technikbasis`, [Draft-PR34](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/34) gegen WP036/B `3d1ac59c3f70d50e05434a54d3ddec5d282bb0c3`.

Beide tatsächlich vorhandenen Workflows sind Pushereignisse, Attempt 1. Die CI checkt den Source-SHA selbst aus; kein synthetischer PR-Merge. Architecture und alle vier fachlichen Unity-Joblogs binden den tatsächlichen Checkout `54d2496…`.

| Workflow | Run | Terminaler Originalstand |
|---|---:|---|
| Architecture Validation / Push | [39217159710](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39217159710) | completed / success, aktualisiert 07:37:49Z |
| Unity CI / Push | [39217159752](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39217159752) | completed / failure, aktualisiert 08:13:20Z |

Beobachtung begann um 07:38:10Z; der terminale Nachweis wurde um 08:14:57Z (10:14:57 Europe/Berlin) gelesen. Unveränderte Zwischenstände wurden mit 50–60 Sekunden Abstand lesend beobachtet. Keine Reruns, Dispatches, Abbrüche, Pushes oder lokale Unityausführung durch diese Rolle.

## Tatsächliche Jobs und Originale

| Unityjob | Job-ID | Ergebnis |
|---|---:|---|
| unity-environment-linux | 117943594212 | SUCCESS |
| unity-config | 117943594433 | SUCCESS |
| project-preflight | 117943594619 | SUCCESS |
| unity-environment-ios | 117943594627 | SUCCESS |
| unity-environment-android | 117943594630 | SUCCESS |
| unity-evidence-guard | 117943718447 | SUCCESS |
| unity-compile-editmode | 117943718790 | SUCCESS |
| unity-playmode-bootstrap-smoke | 117946186468 | SUCCESS |
| unity-android-development-il2cpp | 117947450077 | SUCCESS |
| unity-ios-export | 117953319870 | FAILURE |

Architecturejob `117943594137` protokolliert tatsächlich Architecture-only 17 und WP040-Production-Scope 18 lokale Prüfgruppen PASS, jeweils einschließlich Self-/Negativprüfungen. Diese originalen Ergebnisse gehören diesem Freeze.

Originale EditMode-/PlayMode-ZIPs wurden über den GitHub-Connector bezogen, im Speicher heruntergeladen, bytegenau gehasht und ihre tatsächlichen XML-Testfälle selbst gezählt:

| Originalartefakt | Ergebnis | ZIP-SHA256 |
|---|---|---|
| editmode-results, 12170022607 | 496/496 Passed; 0 Failed/Skipped/Inconclusive; 229.17389 s | `5bbc1e0bd1ff964a173fc88741424068c18648d4b374bd3ddbab8bb9394132d3` |
| playmode-results, 12170930865 | 49/49 Passed; 0 Failed/Skipped/Inconclusive; 10.419509 s | `e6ac998cb1352278cf4ea8214f95a8416f965cc4afacaf686f65463d8665cfd4` |

ZIP-Digests entsprechen jeweils der API. Die Originallogs bestätigen Domain 19, Solver 56 und Content 401 erfolgreiche Fälle sowie PlayMode 49/27 Smoke; beide Coveragebelege erfassen 18 STP-Assemblies ohne behauptete Mindestcoverage. XML-Text-Hashes in der JSON-Datei bezeichnen ausdrücklich dekodiertes UTF-8, keinen roh gehashten ZIP-Member.

Android protokolliert tatsächliche Verarbeitung der Szenen DevelopmentPuzzle und BootstrapComposition im BuildReport und `STP Build PASS` um 08:03:57.8231951Z. Das Originalartefakt `12171995495` wurde ebenfalls selbst im Speicher geprüft: ZIP 45,585,287 Bytes, SHA256 `746fede902f194f4b01d864f372cfdffcd317509699e3053ad5fd02a558056ec`, API-identisch. Der echte APK-Member hat 46,844,477 Bytes und SHA256 `a4fc2b2dfbb539e75be603339a1529606a941943ca2f533daf46aa162c1e7a2a`. Dies ist ein CI-Buildnachweis, kein physischer Gerätetest.

## Konkreter iOS-Fehler und Lizenzgrenze

iOSjob `117953319870` lief auf Runner `GitHub Actions 1000002150`, Runnerimage `macos-15-arm64 20260907.0337.1`, macOS 15.7.9 / 24G830. Der vorhandene Cachekey `unity-editor-macos15-6000.3.23f1-ios-v1` war ein ausdrücklicher Cachemiss. Der reguläre Workflow installierte Unity Hub und führte danach den bestehenden Editorinstallationsschritt aus:

```text
Unity Hub -- --headless install
  --version 6000.3.23f1 --changeset 09d2ecc7fb28
  --module ios --architecture arm64
```

Das ist eine lesbare Wiedergabe der im Originallog gebundenen Argumente und Environmentwerte. Eine konkrete Editor-Binary-DownloadURL wird im Original nicht ausgegeben. Beobachtet ist ausschließlich der folgende DNS-Hostfehler, keine abgeleitete genaue URL:

> 2026-10-19T08:13:12.7015060Z [Unity (6000.3.23f1)] failed to download. Error given: getaddrinfo ENOTFOUND download.unity3d.com

Der Editordownload stand zuletzt bei 85.98%; danach `Failed to install.` und Exitcode 1. Step 5 FAILURE. Editorversionsprüfung, Librarycache, Xcodeprüfung, Lizenzaktivierung und iOS-Export/Xcodebuild (Steps 6–10) sind tatsächlich SKIPPED. **iOS-Export und Xcodeabschluss sind NOT_EXECUTED; ein erfolgreicher Plattformbuild wird nicht behauptet.** Der Fehler ist als beobachteter Infrastrukturdownloadfehler eingegrenzt; eine Codeursache oder dauerhafte Ursache wurde daraus nicht unterstellt.

Es liegen **drei** tatsächliche `Deactivation processed successfully.`-Receipts vor, jeweils Seat `2476057761288-UnityPersonal`:

| Job | Receipt UTC |
|---|---|
| EditMode | 2026-10-19T07:44:38.5092525Z |
| PlayMode | 2026-10-19T07:48:10.1395829Z |
| Android | 2026-10-19T08:04:05.1284825Z |

Im iOS-Job wurde die Lizenzaktivierung nie erreicht; es gibt keine erfolgreiche Aktivierungs- oder Deaktivierungsquittung. Der best-effort-Cleanupstep ist durch `|| true` formal SUCCESS, meldet aber um 08:13:14.9852780Z den fehlenden Licensing-Client (`No such file or directory`). **Das ist kein viertes Receipt.** Dieser Job hat laut ausgeführter Schrittfolge keinen Seat aktiviert. Die drei zuvor aktivierten Jobs haben ihre eigenen echten Freigabebelege.

## Persistente Evidenz und nächste Zuständigkeit

[T_CI_EVIDENCE.json](T_CI_EVIDENCE.json) enthält die vollständigen terminalen API-Jobdaten mit Runnern/Zeiten/Steps, Runbindungen, Originalartefaktmetadaten und selbst ermittelten Hashes/Testzahlen. Originallogs sind durch SHA256 der dekodierten UTF-8-Texte, ursprüngliche Job-URLs und ausgewählte exakte Zeilen mit Zeilennummern gebunden; kurzlebige Download-URLs werden nicht gespeichert.

T bleibt wegen der fehlenden regulären iOS-Ausführung offen. Root behandelt den konkreten Infrastrukturblocker nach dem bestehenden Mandat und den geltenden Prüf-/Retryregeln. Diese Rolle hat keine Retrymaßnahme, Workflowänderung, Assertionabsenkung oder neue Unityinstanz ausgelöst. Die erfolgreiche Teil-CI wird nicht auf A/S/I, spätere Heads, Main, Humanreview, Produkt oder Release übertragen. Die aktuelle Sourcearbeitskopie wurde nicht verändert; ausschließlich diese beiden T-Evidenzdateien wurden geschrieben, ohne Commit oder Push.
