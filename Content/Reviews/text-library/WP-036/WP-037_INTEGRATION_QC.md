# WP-037 – unabhängige Integrationsprüfung

**Finales Ergebnis: PASS der unabhängigen Integrationsprüfung auf `b9b59647cf73e2dcfa05a413259ca27c18a5afdf`.** Tree/Diff und beide getrennten Scopeketten sind bestätigt; beide neuen Architecture-Läufe sowie der vollständige neue Unity-PR-Lauf mit allen zehn Jobs sind erfolgreich und an Originalbelege gebunden. Der zusätzliche Unity-Push ist CANCELLED und ausdrücklich kein PASS. Keine offenen technischen Integrationsbefunde. Dieser eigene nachgelagerte Nachweis ändert das unabhängige AK07-Urteil auf der unveränderten Lieferung nicht und erteilt keine Main-/Produkt-/Releasefreigabe. Frühere unten eindeutig als Teilstände beschriebene Pending-/In-Progress-Angaben sind Historie; der finale Schlussnachtrag ist maßgeblich.

Prüfrolle `/root/wp037_independent_qc`, weiterhin unabhängig von Implementierung und koordinierender Integrationsentscheidung. Neue gezielte Prüfung nach gelesenem gesondertem Entscheid [WP-037-SUBINTEGRATION-01](WP-037_SUBINTEGRATION_DECISION.md); keine zusätzliche Solver-/Textgesamtprüfung, keine Quelländerung, keine CI-Mutation und keine Mainfreigabe.

## Tatsächliche Unterintegration

| Gegenstand | Selbst bestätigter Wert |
|---|---|
| PR32 | tatsächlich merged/closed, ursprünglicher geprüfter Head unverändert `8dd68e22b58f126cb499fee408e98c173202a98e` |
| Mergecommit | `b9b59647cf73e2dcfa05a413259ca27c18a5afdf` |
| Erster / zweiter Parent | `ff1836e293a2a5cff69773525c347b55deb67dfb` / `8dd68e22b58f126cb499fee408e98c173202a98e` |
| Merge-Tree | `25b7907fee4e0741574c3f32867fbef03bfb1e96`, exakt identisch zum unabhängig geprüften Lieferbaum |
| Lieferung→Integration | vollständiger `git diff` leer; 0 geänderte Dateien |
| Basis→Lieferung / Basis→Integration | vollständige Diffs bytegleich |
| PR31 | frisch selbst gelesen: weiterhin open/Draft, Head `b9b5964`, Basis main `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |

Main, ff1836e, ursprünglicher WP037-Anker und Lieferung sind Vorfahren des Integrationscommits. Beide historischen Manifeste sind unverändert erhalten; kein Squash/Rebase oder fremder Contentimport. Der separate Koordinationsentscheid bindet Expected-HEAD und Ziel-HEAD vor Merge; der tatsächliche Git-/PR-Zustand entspricht genau dieser Lieferung und dieser Basis.

## Eigene getrennte Scopeketten

Beide nachstehenden kanonischen Production-Scopeprüfungen mit `--self-test` wurden neu im eigenen sauberen LF-Scratchcheckout ausgeführt, mit der vorhandenen gepinnten Python-/Node-/Lockumgebung. Der Rootarbeitsbaum wurde nicht ausgecheckt oder verändert.

| Kette / tatsächlicher Scratchhead | Verwendetes unverändertes Manifest | Ergebnis |
|---|---|---|
| main `d90e798`→`ff1836e` / Checkout exakt `ff1836e` | `tools/architecture-validation/scopes/WP-036.production.scope.json` | Exit 0, 18 Gruppen PASS, worktreeDirty=false |
| `ff1836e`→`b9b5964` / Checkout exakt `b9b5964` | `tools/architecture-validation/scopes/WP-037.production.scope.json` | Exit 0, 18 Gruppen PASS, worktreeDirty=false |

Das alte WP036-Manifest wurde ausschließlich am alten reinen Textlieferhead geprüft. Kein WP036-Scope auf dem kumulativen solverhaltigen Integrationsdiff. Beide Läufe prüfen ihren jeweiligen gemeinsamen historischen WP-/Manifestanker samt Parent, ID/Link und unveränderten Manifestbytes. WP037-Manifest-SHA256 bleibt `022590d076f1e6a972b445b8696c56f83acccfebbaf7e1f30475a388e18e44a4`.

Eigene vollständige Ausgaben: `.utmp/wp037/independent-qc/integration-WP036-scope.txt`, SHA256 `516837477667a6a975420e78a7a11f2d4ec10fe0285cebe8e3f3ae6b47f759dc`; `integration-WP037-scope.txt`, SHA256 `92b6f5ac6322197924aa1162a62e390d59adfbee5e24fae0c7f69372fa9ca039`. Reproduktionsskript `integration.py` und kompakter eigener Git-/Scopenachweis `integration-checks.json` im selben ignorierten Prüfscratch.

## Neue integrierte CI – ausdrücklich separat

Frisch selbst abgefragte tatsächliche neue Runs auf `b9b59647cf73e2dcfa05a413259ca27c18a5afdf`, alle Attempt 1:

| Workflow/Ereignis | Run | Stand dieser Teilprüfung |
|---|---:|---|
| Architecture Push | 39162496562 | completed/success |
| Architecture Pull Request | 39162499593 | completed/success |
| Unity Push | 39162496566 | completed/cancelled; 7SUCCESS/1CANCELLED/2SKIPPED, kein vollständiger PASS |
| Unity Pull Request | 39162499589 | completed/success; alle zehn Jobs erfolgreich |

Beide vollständigen Architecture-Originallogs neu selbst gelesen: Jobs 117778544323 und 117778553507. Je 17 Architecture-/18 WP037-Scopegruppen, JCS 12/32/2/2, Text 1.408/960/240 mit zwölf Mutationen und exakte Lesefassung bestanden; Scopekontext jeweils Head `b9b5964`, Basis `ff1836e`, worktreeDirty=false. Der PR-Lauf prüft zunächst seinen synthetischen Mergebaum und checkt vor dem Scope regulär den tatsächlichen PR-Head aus. Gespeicherte vollständige Connectorlogs liegen im eigenen Prüfscratch; daraus wird kein Roh-HTTP-Archivhash behauptet.

Die echten Checkoutausgaben wurden selbst nachgelesen: Push `b9b59647cf73e2dcfa05a413259ca27c18a5afdf`; PR zuerst `1e95ebaefb445f05e8f18ae0a291ffab01b06966`, vor Scope dann `b9b59647cf73e2dcfa05a413259ca27c18a5afdf`. Das synthetische PR-Gitobjekt wurde zusätzlich frisch über GitHub gelesen: Parents `d90e798` und `b9b5964`, vollständiger Tree ebenfalls `25b7907fee4e0741574c3f32867fbef03bfb1e96`. Auch die PR-Architektur-/späteren PR-Unityprüfungen sind damit an denselben echten Inhalt gebunden; API-head_sha allein wurde nicht als Checkoutnachweis verwendet.

Noch ausstehend: beide vollständigen tatsächlichen Unity-Jobinventare und neue Original-XMLs/ZIPdigests/Checkoutbindungen/Plattformnachweise. Frühere Codefreeze-/Liefer-CI bleibt quellengebundener Nachweis und ersetzt diese neuen Runs nicht. Bis zu ihrem wirklichen erfolgreichen Ende bleibt der vollständige Integrationsabschluss offen. Keine Retries, Quarantäne, Abbrüche oder Testsuiteverkürzung durch diese Prüfrolle.

### Tatsächlich abgeschlossener neuer Push-EditMode-Teilnachweis

Job117778615652 inzwischen tatsächlich SUCCESS, selbst geladenes Originalartefakt12149357430: ZIP422667Bytes, SHA256 `73627afa3326d6cac996fd7a258bc3f7d238ef5debd28d47ed13dd4764ad76a7`, gleich frisch selbst abgefragtem GitHub-Digest. Original-XML SHA256 `35d4fff57d32def6b86af4c0dbbc1fcbf91bdee7849eb22bff5f039e5c139c46`, 455/455 Passed, kein Failed/Skipped. Original-HardBudget48.342306s, neue1639er-Regression0.382111s. Alle sechs Originalfälle erhalten, maxStates2 beim unterbrochenen Langpfad, maximale selbst nachgerechnete excess/build-Quote0.1132075472<0.4. Coverage15STP-Assemblies, Solver1039/1045besuchteSequenzpunkte und123/123Methoden; Nullmessung separat. Vollständiger tatsächlicher Joblog bestätigt Checkout exaktb9b5964. Dieser abgeschlossene Teilnachweis ersetzt nicht die noch laufenden anderen Pflichtjobs/Ereignisse und ist noch kein Gesamt-Integrations-PASS.

### Neuer tatsächlicher Pushabbruch und unabhängiger PR-Teilnachweis

Der Pushlauf39162496566 ist inzwischen tatsächlich COMPLETED/CANCELLED (updated_at20:30:11UTC), Attempt1. Jobinventar: siebenSUCCESS einschließlich EditMode, PlayMode117780248567CANCELLED, Android117781810910/iOS117781811159SKIPPED. Das ist kein vollständiger Push-PASS. Für den nicht ausgeführten PlayModejob liefert der selbst versuchte Originallogabruf GitHub404BlobNotFound; kein Loginhalt oder Abbruchgrund wird erfunden. Diese Rolle hat keine CI-Mutation ausgeführt. Der unabhängige PR-Lauf darf regulär auslaufen, ohne Retry oder erneuten Push.

PR-EditMode117778622614 inzwischen tatsächlich SUCCESS: Originalartefakt12148819422, ZIP422634Bytes/SHA256 `ce36b3b5636e6fdae25da8f7fd2fccc7925630b7640083264c79bec6eb4c8d01`, selbst geladen/gehasht und GitHub-digestgleich. Original-XML `a9f84515de48a3753473a8298eb12de852b2b4f86d3f2bbcdba23f654fd6a032`,455/455Passed. HardBudget47.345466s,1639er-Test0.361034s, größte selbst nachgerechneteQuote0.1090909091<0.4. Tatsächlicher Logcheckout1e95eba mit dem zuvor selbst geprüften identischen Tree25b7907. Alle ursprünglichen sechs Fälle, Coverage15Assemblies/Solver1039/1045+123/123 erhalten.

Vertragsauslegung direkt aus WP037 Scope8/Tests5: verlangt wird neue vollständige reguläre CI mit allen zehn Unityjobs am Integrationsstand; ein zusätzlicher PASS beider Ereignisse wird nicht gefordert. Ein später tatsächlich vollständiger PR-PASS kann diesen Buildnachweis erfüllen, während der Push ausdrücklich CANCELLED bleibt. Der bislang noch laufende PR wird hier noch nicht als PASS bezeichnet; kein „22ChecksPASS“ und keine erfundene Abbruchursache.

## Tatsächlich terminaler Integrationsabschluss

Die vier neuen Runmetadaten sowie beide vollständigen Unity-Job-/Artefaktinventare wurden nach dem Ende erneut frisch selbst gelesen. Alle vier Runs sind abgeschlossen, jeweils Attempt1. UnityPR39162499589 ist tatsächlich SUCCESS (updated_at20:52:37UTC). Alle zehn PR-Jobs sind completed/success; kein übersprungener Pflichtjob. Der Push bleibt der oben erhaltene CANCELLED-Stand. Zusammen sind die tatsächlichen 22 Ereignischecks 19SUCCESS/1CANCELLED/2SKIPPED, ausdrücklich kein „22ChecksPASS“.

| Tatsächlicher erfolgreicher PR-Job | ID |
|---|---:|
| unity-config | 117778553484 |
| unity-environment-linux | 117778553587 |
| project-preflight | 117778553616 |
| unity-environment-android | 117778553647 |
| unity-environment-ios | 117778553755 |
| unity-evidence-guard | 117778622165 |
| unity-compile-editmode | 117778622614 |
| unity-playmode-bootstrap-smoke | 117781807652 |
| unity-android-development-il2cpp | 117782567976 |
| unity-ios-export | 117784358564 |

PR-PlayMode: selbst geladenes Originalartefakt12149693059, ZIP347903Bytes/SHA256 `5e846c1ff5ce350ff153d7a247450342fb0939e1a3d643ed5b3f34aaf46bd9f1`, gleich GitHub-Digest. Original-XML `284b6ca56a3a396c309d7549b592cf933e545aa31ba842748f1af9071f7c7621`:27/27Passed, keinFailed/Skipped; vorgeschriebener QA-KompositionsfallPassed0.223152s. Beide Coverage-Sitzungen enthalten15STP-Assemblies. Vollständiger Joblog selbst gelesen, tatsächlicher Checkout1e95eba/Tree25b7907.

PR-Android: Originalartefakt12150356105, ZIP38948933Bytes/SHA256 `89c7a38667fa71e225e2f1ff868cb563c7b7e04d2fbbdf07df96d5589a84cb0b`, gleich GitHub-Digest. APK40066234Bytes/SHA256 `d80e099d2c880fbe87d2d7ec491a3441e2df864553cd6f72ea1a7a1f8ca74446`; AndroidManifest und native `lib/arm64-v8a/libil2cpp.so` selbst im APK vorgefunden. Vollständiger875786Byte-Joblog bestätigt tatsächlichen Checkout1e95eba und STPBuildPASS:Android; gespeicherter Log-SHA256 `712eb6056ace56aa074d05eb09af373f4431acb12521baaa2ad9a3099cdbe9cc`.

PR-iOS: vollständiger2528021Byte-Joblog selbst abgerufen und ausgewertet, tatsächlicher Checkout1e95eba, Xcode26.3, echter Unityexport mit STPBuildPASS:iOS und anschließendes `** BUILD SUCCEEDED **` bei `CODE_SIGNING_ALLOWED=NO`. Gespeicherter Log-SHA256 `7ccaa69082da799bc1874c0a9518e93ee2a9bf6101b5354697613f0175b4f258`. Es wird keine Signierung, Installation, Geräteleistung oder Storefreigabe behauptet.

Damit besteht der neue vollständige reguläre Integrations-Buildnachweis am tatsächlich identischen Tree unabhängig von der erhaltenen Pushstornierung. Neue Testausgaben, Checkout-/Treebindungen und Artefaktdigests ersetzen keine historischen FAILs und werden nicht aus der Liefer-CI übertragen. Keine eigene CI-Ausführung, kein Retry, Abbruch oder Push durch diese Prüfrolle; keine konkrete Ursache der Stornierung behauptet.

## Dauerhaft gebundene eigene Originalevidenz und Übergabe

[Eigene maschinenlesbare Integrationsoriginale](WP-037_INTEGRATION_EVIDENCE.json),2492595Bytes, SHA256 `bf7b8e954fd1e6f7a09277866f857324a11b843e61603eb8366a5136a62cd8fd`. Sie enthält frische eventbezogene Run-/Job-/Artefaktmetadaten, die tatsächlich erhaltene Pushstornierung samt Log404 und insgesamt27verlustfreiegzip/base64-Payloads:19vollständige dekodierte erfolgreiche Joblogs, drei unveränderte Original-NUnit-XMLs, drei Original-Test-ZIPs und die zwei eigenen vollständigen getrennten Scopeausgaben. Alle27Payloads wurden nach dem Schreiben erneut vollständig dekomprimiert; Roh-/gzip-SHA256 und Bytezahlen stimmen. Coverage-Originale bleiben zusätzlich in den erhaltenen Original-Test-ZIPs. AndroidZIP/APK wird nicht redundant ins Repository kopiert; vollständig erhalten sind dessen Digests/Memberinventar/nativerARM64-Nachweis und der vollständige Buildlog.

ZIP-/XMLhashes gelten für unveränderte Originalbytes. Die Loghashes gelten ausschließlich für vollständige gespeicherte dekodierte Connectorinhalte im ausdrücklich dokumentierten UTF8-/BOM-/LF-/terminalenNewline-Profil, nicht für rohe HTTP-Logarchive. Geparste API-Metadaten sind als solche ausgewiesen.

**Unabhängiger Abschluss:** exakter unveränderter Lieferbaum integriert, historische Anker und beide Scopeketten erhalten, neuer vollständiger regulärer CI-/Buildnachweis samt Originalen erfolgreich, keine offenen technischen Integrationsbefunde. AK08/AK09-Nachführung kann diesen realen Integrationsstand als abgeschlossen aufnehmen. Der von der Koordination nachfolgend angelegte reine Closeout-Dokumentationsdiff benötigt noch seine schmale Delta-/Byte-/Scopeprüfung und gesonderte tatsächliche automatische CI-Zuordnung; er ändert die abgenommene technische Quelle nicht.

Letzter abgeschlossener Prüfschritt: neue vollständige integrierte PR-CI einschließlich Original-XMLs, Android und iOS sowie eigener Scopeketten- und Treeprüfung unabhängig PASS aufb9b5964. Nächster Schritt: Koordination persistiert den aktuellen WP-/Status-/Übergabeabschluss und lässt ausschließlich dessen reinen Dokumentationsdiff gezielt nachprüfen. Main-, Produkt-, Runtime-, Import-, Rechts-, Geräte-, Signing-, Store-, Release- und Fremdgates bleiben offen.
