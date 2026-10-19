# T – ein gezielter iOS-Infrastrukturnachlauf

**Ergebnis: Der eine zulässige iOS-Nachlauf ist tatsächlich SUCCESS.** Die ursprünglichen neun erfolgreichen Unityjob-Ergebnisse werden mit ihren ursprünglichen Ausführungsbindungen wiederverwendet; nur iOS wurde neu ausgeführt. Der erste terminale Unity-FAIL bleibt unverändert dokumentiert. Dieser Bericht ist ein getrenntes CI-Evidenzurteil, keine Annahme oder Umsetzung von A/S/I.

Getrennte lesende Rolle `/root/entry_audit`, 19.10.2026; terminaler API-/Originallogstand gelesen um 08:29:44Z / 10:29:44 Europe/Berlin. Root hat nach seiner gesonderten Prüfung genau einen Job-Retry ausgelöst, Toolantwortzeitpunkt 08:17:24Z; [Dispatchrecord](T_IOS_RETRY_DISPATCH.json). Diese Rolle hat weder Dispatch noch weitere Retrymaßnahme, Push, Workflowänderung oder lokale Unityausführung vorgenommen.

## Stand und tatsächliche neue Ausführung

Unveränderter Sourcefreeze `54d2496ac2c74b681d24fe7a69d7f044392c46dd`, Tree `85d1cdb94beaa440cca600e16a1bf6ac06cf128e`. Workflow `.github/workflows/unity.yml`, Blob `027abe195289bd4b30baaae404bef81c12df7c83`; eingefrorener und aktueller Workflowblob sind gleich. Neuer Originalcheckout des iOS-Jobs ist ausdrücklich derselbe Source-SHA.

[Unityrun39217159752, Attempt 2](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39217159752/attempts/2) ist terminal **completed / success**, aktualisiert 08:29:30Z. Neuer iOSjob `117958208352`, Runner `GitHub Actions 1000002151`, Start 08:17:19Z, Ende 08:29:29Z, tatsächlich SUCCESS. Der ursprüngliche iOSjob `117953319870` und dessen Download-FAIL werden dadurch nicht umgeschrieben.

Der unveränderte Workflow restaurierte im neuen Job den Editorcache `unity-editor-macos15-6000.3.23f1-ios-v1` erfolgreich. Der tatsächlich gestartete Editor protokolliert `6000.3.23f1 (09d2ecc7fb28)`; Xcode meldet 26.3. Beide QA-Szenen DevelopmentPuzzle und BootstrapComposition wurden ausdrücklich im echten iOS-BuildReport verarbeitet.

| Tatsächlicher Originalbeleg | UTC |
|---|---|
| `Activation processed successfully.` | 08:19:57.9848390Z |
| `STP Build PASS: iOS -> Builds/iOS/Xcode` | 08:22:24.1384010Z |
| `** BUILD SUCCEEDED **` | 08:29:21.1487500Z |
| `Deactivation processed successfully.` | 08:29:23.4628170Z |

Aktivierung und Freigabe nennen denselben Seat `2476057761288-UnityPersonal`. Dies ist der vierte tatsächliche Freigabebeleg über die erfolgreichen ursprünglichen EditMode-/PlayMode-/Androidausführungen plus den neuen iOS-Job. Der ursprüngliche fehlgeschlagene iOS-Versuch aktivierte keinen Seat; sein best-effort-Cleanup bleibt ausdrücklich ohne Receipt.

## Wiederverwendete Ergebnisse und Artefaktgrenze

GitHub zeigt in Attempt 2 neun weitere SUCCESS-Statusobjekte mit neuen Job-IDs. Für **alle neun** sind Startzeit, Endzeit und Runner-ID tatsächlich identisch zum Original aus Attempt 1 und liegen vor dem Retrydispatch. Sie sind übernommene Ergebnisse, keine neun neu ausgeführten Jobs. [T_IOS_RETRY_EVIDENCE.json](T_IOS_RETRY_EVIDENCE.json) bindet die ursprünglichen und neuen Status-IDs samt Zeit-/Runnervergleich.

Die ursprünglichen **496/496 EditMode**, **49/49 PlayMode**, Coverage- und Android-/APKbelege bleiben an ihren ersten Ausführungen gebunden. Artefaktinventar nach dem Retry enthält weiterhin ausschließlich dieselben drei Originalartefakte:

- EditMode `12170022607`, ZIP-SHA256 `5bbc1e0bd1ff964a173fc88741424068c18648d4b374bd3ddbab8bb9394132d3`.
- PlayMode `12170930865`, ZIP-SHA256 `e6ac998cb1352278cf4ea8214f95a8416f965cc4afacaf686f65463d8665cfd4`.
- Android `12171995495`, ZIP-SHA256 `746fede902f194f4b01d864f372cfdffcd317509699e3053ad5fd02a558056ec`.

Der bestehende Workflow lädt keinen separaten iOS-Export als herunterladbares Artefakt hoch. Der neue iOS-Originaljoblog liefert den tatsächlichen Export-/Xcode-/Lizenznachweis; ein nicht vorhandenes Export-ZIP oder dessen Hash wird nicht erfunden. SHA256 des vollständig gelesenen dekodierten UTF-8-Originallogs: `1f96c61c161b393f40740dd7a0063ee59f1343707e4a186c748dcd4c25a7fa7f`; ausgewählte Originalzeilen und vollständige terminale Job-/Runmetadaten stehen im JSON.

## Erhaltene Historie und Reichweite

[T_CI.md](T_CI.md) und [T_CI_EVIDENCE.json](T_CI_EVIDENCE.json) bleiben durch diese Rolle bytegleich zum vor dem Dispatch persistierten Attempt-1-Stand:

| Datei | SHA256 |
|---|---|
| T_CI.md | `45ef4f19646f99a9ad1b1608068256c5c591a017fe6ed150ef20fd555b2932d0` |
| T_CI_EVIDENCE.json | `820760dac3d29ccb5c53808dcae0690e4a2abc105c222756bb526400a8de3a06` |

Die tatsächliche kombinierte CI-Aussage ist: Architecture Attempt 1 SUCCESS; neun ursprüngliche Unityjobs SUCCESS; ursprünglicher iOS-Download-FAIL erhalten; genau eine separat autorisierte neue iOS-Ausführung SUCCESS. Kein „zehn neue Jobs“ oder Löschen des ersten FAILs. T-Abschluss und anschließende Kindbasisentscheidung liegen bei Root nach den übrigen T-Kriterien. Keine Aussage über implementierte A/S/I-Funktionen, Humanreview, physische Geräte, Grafik oder Produkt-/Main-/Releasefreigabe. Ausschließlich die beiden neuen Retry-Evidenzdateien geschrieben; nicht committed oder gepusht.
