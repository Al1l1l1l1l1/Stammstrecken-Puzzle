# T-Schlussstand D – unabhängiger tatsächlicher CI-Nachweis

**Reguläre D-CI tatsächlich PASS.** Getrennte lesende Rolle `/root/entry_audit`, 19.10.2026. Beide eigenen D-Workflows sind terminal SUCCESS, alle zehn Unityjobs erfolgreich, jeweils Attempt 1. Vier echte neue D-Lizenzdeaktivierungen und eigene Original-XML-/Build-/Artefaktnachweise liegen vor.

Dies ist ausschließlich ein technisches CI-Evidenzurteil. R-Freigabe und frischer Refabgleich vor A/S-Verankerung bleiben bei Root. Kein A/S-/I- oder Produkt-/Main-/Release-PASS durch diese Rolle.

## Stand und Ereignisse

D `925ca608a961ac26c84385c8da3b949f609df883`, einziger Parent `54d2496ac2c74b681d24fe7a69d7f044392c46dd`, Tree `6b2693ad596ba3b8fb84ef1044a9b367270d22b2`. Lokales Git und GitHub-Git-Commit-API stimmen überein. Workflowblob `.github/workflows/unity.yml`: `027abe195289bd4b30baaae404bef81c12df7c83`.

Die eigene reguläre CI wurde über `/actions/runs` entdeckt und bei unveränderten Zwischenständen mit 50–60 Sekunden Abstand lesend begleitet. Erste Beobachtung 08:44:18Z; terminale Originale um 09:25:24Z (11:25:24 Europe/Berlin) gelesen. Keine Reruns, Dispatches, Abbrüche, Pushes oder lokale Unityausführung durch diese Rolle.

| Eigener Workflow / Ereignis | Run | Terminal |
|---|---:|---|
| Architecture Validation / Push | [39224770669](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39224770669) | completed / success, Attempt 1, aktualisiert 08:43:44Z |
| Unity CI / Push | [39224770689](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39224770689) | completed / success, Attempt 1, aktualisiert 09:24:27Z |

API-head_sha und tatsächliche Checkouts in den Originallogs binden D; kein synthetischer PR-Merge. Der Commit-API-Tree ist exakt `6b2693ad…`. Roots vorgelagerter Rohblobnachweis wurde als separate Provenienz gelesen: `T_D_RAW_BLOB_BINDINGS.json`, SHA256 `95c22d3ac0a80ac36c38ef0c2f5f06b842af0c3b2c3ceb7788f391a7029ec855`. Keine pauschale erneute Source-Gesamt-QC daraus abgeleitet.

## Tatsächliche neue Jobs und Originaltests

Architecturejob `117968269537` protokolliert Architecture-only **17** und WP040-Production-Scope **18** lokale Prüfgruppen PASS einschließlich Self-/Negativprüfungen.

| Unityjob | Job-ID | Ergebnis |
|---|---:|---|
| unity-environment-android | 117968269697 | SUCCESS |
| unity-environment-linux | 117968269996 | SUCCESS |
| unity-environment-ios | 117968270045 | SUCCESS |
| unity-config | 117968270078 | SUCCESS |
| project-preflight | 117968270079 | SUCCESS |
| unity-compile-editmode | 117968400200 | SUCCESS |
| unity-evidence-guard | 117968400226 | SUCCESS |
| unity-playmode-bootstrap-smoke | 117970765569 | SUCCESS |
| unity-android-development-il2cpp | 117972117245 | SUCCESS |
| unity-ios-export | 117978722428 | SUCCESS |

Eigene D-Testartefakte wurden über GitHub bezogen, ausschließlich im Speicher heruntergeladen, bytegenau gehasht und die tatsächlichen Original-XML-Wurzeln und sämtliche test-case-Ergebnisse selbst gezählt:

| Originalartefakt | Tatsächliche Testzahlen | ZIP-SHA256 |
|---|---|---|
| EditMode `12172924814` | **496/496 Passed**, 0 Failed/Skipped/Inconclusive, 162.4044856 s | `16cef35d051e8a2c9f957e6139f6133578ee91a558d8ac9a85853d9606a1570c` |
| PlayMode `12173936200` | **49/49 Passed**, 0 Failed/Skipped/Inconclusive, 10.1997304 s | `9ad16f5fc1a102dd21c5df8469069bd6cf8102a524958e3ce4596e688777424c` |

Beide ZIP-Digests entsprechen ihrer API. Originallogs bestätigen Domain 19 / Solver 56 / Content 401 erfolgreiche Fälle sowie PlayMode 49 / 27 Smoke und beide Coveragebelege für 18 STP-Assemblies ohne behauptete Mindestcoverage. XML-Text-Hashes im JSON sind ausdrücklich dekodierte UTF-8-Hashes und keine roh gehashten ZIP-Member.

## Plattformbelege und vier tatsächliche D-Freigaben

Android- und iOS-Originallogs bestätigen jeweils die tatsächliche BuildReport-Verarbeitung beider QA-Szenen `DevelopmentPuzzle.unity` und `BootstrapComposition.unity`.

- Android: `STP Build PASS` um 09:09:18.8874177Z. Originalartefakt `12174542073`, ZIP 45,585,316 Bytes, SHA256 `87da1c9a128d1d7adc00533affce9898b26082040c4d4397320e6b144bd6c2f0`, selbst berechnet und API-identisch. Tatsächlicher APK-Member 46,844,477 Bytes, SHA256 `62be328c4ff19733c8ef690407c04b41f2fa2700b7df38ce4f49cdba77e0fda4`.
- iOS: `STP Build PASS` um 09:15:12.6518140Z und unsigned Xcode `** BUILD SUCCEEDED **` um 09:24:16.9979460Z. Tatsächlich gestarteter Editor `6000.3.23f1 (09d2ecc7fb28)`, Xcode 26.3. Der unveränderte Workflow lädt kein separates iOS-Export-ZIP hoch; Export, Xcodeabschluss und Freigabe werden durch den selbst gelesenen Originaljoblog gebunden. Ein nicht vorhandenes Artefakt oder dessen Hash wird nicht erfunden.

Jeder der vier neuen D-Fachjobs hat genau eine echte Aktivierungs- und Deaktivierungsquittung. Sämtliche Quittungen nennen Seat `2476057761288-UnityPersonal`. Tatsächliche `Deactivation processed successfully.`-Receipts:

| D-Job | Receipt UTC |
|---|---|
| EditMode `117968400200` | 2026-10-19T08:49:27.9731482Z |
| PlayMode `117970765569` | 2026-10-19T08:52:56.2101562Z |
| Android `117972117245` | 2026-10-19T09:09:25.3839517Z |
| iOS `117978722428` | 2026-10-19T09:24:19.9148500Z |

Es sind vier neue D-Receipts; keine Übertragung der B-/54d-/Retryfreigaben und kein bloßer Cleanup-Stepstatus als Ersatz.

## Evidenz und Reichweite

[T_D_CI_EVIDENCE.json](T_D_CI_EVIDENCE.json) enthält beide terminalen Runmetadaten, vollständige terminale Jobs mit Runnern/Start-/Endzeiten/Steps, die neuen Artefaktmetadaten und selbst ermittelten ZIP-/APK-/XML-Bindungen. Die fünf Originaljoblogs sind durch SHA256 der vollständig gelesenen dekodierten UTF-8-Texte, Original-URLs und exakte ausgewählte Zeilen mit Originalzeilennummern gesichert.

Die beiden ignorierten Dateien sind die einzigen Schreibziele dieser Aufgabe; D, CURRENT_STATE, Work Packages, bestehende T-Evidenz und Source bleiben unverändert. Der Arbeitsbaum war vor den ignorierten Schreibzugriffen sauber. Kein Commit oder Push. Die Dateien sind für die spätere identische Aufnahme in beiden eigenen Kind-Evidenzpfaden vorgesehen; hier wurde noch kein Kindanker erstellt.

Dieser eigene D-Nachweis ist keine aus Elternständen übernommene CI-Aussage. Die ursprüngliche 54d-iOS-Downloadfehlerserie und der eine separat erlaubte erfolgreiche Infrastrukturnachlauf bleiben unverändert historische Belege. Keine neue native Grafik-, Geräte-, Humanreview-, A/S-/I- oder Releaseabnahme. Root entscheidet nach diesem tatsächlichen Nachweis und seinem frischen Refabgleich den zulässigen nächsten Schritt.
