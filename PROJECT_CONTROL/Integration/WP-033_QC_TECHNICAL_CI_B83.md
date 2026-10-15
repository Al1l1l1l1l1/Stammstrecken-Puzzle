# WP-032 – getrenntes technisches CI-Addendum zu B83

**Abgeschlossen: technische Quellen-/Scopeprüfung und neue vollständige B83-CI bestanden, keine offenen technischen Befunde im beschriebenen WP-032-Prüfumfang.** Separate unabhängige Codex-Technikrolle `/root/independent_technical_qc`; keine Autorenrolle und keine behauptete Astra-/Sol-Modellidentität. Abschlussabfrage15.10.2026,01:14:55UTC. Der Quellenbericht bleibt unverändert: SHA256 `6a2bb25a6834e0644bade07134ec73a7c3124a0c8d05f117dbbd7eb7acaa39fa`.

Gegenstand ausschließlich `b83c69ccecdaf4455aa4d5b0882519e4530291d0`, Tree `ff0d4cdeaf4a86612530ce90bf6dd7a274e05b4a`, Architecture38661531726 und Unity38661531680. Kein PASS eines anderen Commits wird übertragen. Keine Retries, Ref-/Dateiänderungen am Lieferstand oder Workflowaktionen ausgelöst.

## Bereits selbst nachgeprüfte neue CI

Architecture Job116183682294 SUCCESS; Originaljoblog selbst gelesen: tatsächlicher CheckoutB83,17Architecture-only- und18WP032-ScopegruppenPASS, `base=C060 head=B83 worktreeDirty=false`. Anker-/Eingangs-/Konflikt-/Bindungsprüfung bleibt die eigene zusätzliche Prüfung im unveränderten Quellenbericht, kein Ersatz durch diesen kleinen Delta.

Unity-EditMode Job116183762944 SUCCESS. Artefakt11939038443 und dessen tatsächliche API-Metadaten selbst gelesen: WorkflowRun38661531680, HeadB83. Original-ZIP selbst gelesen/gehasht:432074Bytes/SHA256 `41d46db90ee938f604b234aa44ebfd10dd53302acfc1a884e2fce23a4af5693e`, exakt API-Digest. Original-XML ausZIP:401768Bytes/SHA256 `5b995d828a662b46062fce1784545117afe99c4ab73947d2a4c162b1716ec88f`;455/455Passed,0Failed/Skipped/Inconclusive. Vollständige XML selbst geparst, jeder TestcasePassed. Unveränderte Originalbytes, keine Newlinenormalisierung. Echtes EditMode-Joblog ebenfalls vollständig gelesen; Coverage-Evidenz15STP-AssembliesPASS, keine erfundene Mindestcoverage.

Aus genau einer tatsächlichen `WP028_CORPUS_PERFORMANCE`-XML-Zeile eigenständig nachgerechnet:

* CommitB83, ProtokollADR-032-v1, Unity6000.3.23f1/Mono6.13, sieben exakte IDs S1-05-04-06 bis-12.
* Je fünfWarmups und50Messungen,350Samples. Alle Messinputs searchNodes/requiredGuessDepth0. Sortierter Index `ceil(0.95×350)-1=332`; p95 **55,9932ms**, Maximum **105,7528ms**, beide unter250/2000ms.
*240frische bestandene Proofgates mit vierArbeitern in **2243,5763ms**, unter60000ms, keine Fehlerliste.
*Fingerprint selbst aus den480ordinal sortierten tatsächlichen Gitpfaden und Rohblobbytes des eingefrorenen B83 mit UInt64-BE-Längenframing berechnet: `360f3309ff958ab6871f963354ea5ec691a096b0aec68bf19165a1ea43cea6bf`, exakt XML-Wert.

Der historische HardBudget-Test besteht aufB83 in151,039259s. Dieser neue PASS ist kein Ursachenbeweis oder Schließung vonO02.

Eigene Auswertung: `.utmp/wp032-independent-technical/ci_b83.py`, `ci-b83.json`, `ci-b83.txt`; selbst gelesene API-Metadaten/decodedEditlog in demselben eigenen Scratch. Der gespeicherte dekodierte Editlogtext hat363683UTF8-Bytes/SHA256 `3a143b6048ac4c44bb7ed8520f151256c15c53a87a510842168a85711c30e10c`; das ist ein Texttransporthash, kein ZIP-Originalhash.

## Zusätzlich selbst geprüfter PlayMode-Abschluss

PlayMode Job116186380899 inzwischen SUCCESS. Originalartefakt11939696279 selbst über Connector heruntergeladen und gehasht:353984Bytes/SHA256 `39a3dc887367e102f5aed0c58d496976f43d2922f481bc6626dcb38f3e143b78`, exakt API-Digest/WorkflowRun38661531680/HeadB83. Enthaltenes Original-XML26988Bytes/SHA256 `f0d71c21ba0bd6a27aab440e3fbba6f57a8d41be99a51807476edde04c798206` vollständig geparst:27/27Passed,0Failed/Skipped/Inconclusive, jeder TestcasePassed. Neue PlayMode-/QA-Ausführung und Coverage-Jobverifikation anhand des tatsächlichen abgeschlossenen Joblogs separat gelesen.

## Tatsächlicher Plattformabschluss und sämtliche zehn Jobs

Android-Development-IL2CPP Job116187282941 inzwischen SUCCESS. Tatsächliches vollständiges Joblog selbst gelesen, CheckoutB83 belegt; um01:02:44,1649736UTC `STP Build PASS: Android -> Builds/Android/stp-qa-development.apk (683444711 Bytes).` Die im Marker genannten683444711Bytes sind der Unity-BuildReport-Wert, **nicht die tatsächliche APK-Dateigröße**. Die APK im ZIP selbst vollständig gelesen/gehasht:40066242Bytes/SHA256 `ecf01670c207cbfeb9f4d3fc300d39ca8c9df20568b5f6c927661ee805af2712`. Original-ZIPartefakt11939389115 selbst heruntergeladen/gehasht:38948976Bytes/SHA256 `47fe10e9883bff974828cf8bafb816ed67c597cef879e6da8cb0f2a98140e5b3`, exakt API-Digest/Run38661531680/HeadB83. Dekodierter eigener Logtext873138UTF8-Bytes/SHA256 `45bd2a77ad7b16d30482447d4d28f1f16f4b16616ec74ca1e4bcd3b6d1deab15` (Transporthash, kein Artefakthash). Kein physischer Android-Gerätetest behauptet.

iOS-Export/-Xcode Job116189910438 inzwischen ebenfalls SUCCESS. Vollständigen dekodierten Originaljoblogtext selbst perAPI abgerufen und die entscheidenden Checkout-/Ausführungsmarker selbst gelesen: CheckoutB83, Xcode26.3 um01:06:02,5801440UTC; `STP Build PASS: iOS -> Builds/iOS/Xcode (900432206 Bytes)` um01:07:37,7319680UTC; anschließender tatsächlicher xcodebuild-Aufruf für das exportierte Unity-iPhone-Projekt, Release/generic-platform-iOS/`CODE_SIGNING_ALLOWED=NO`; **BUILD SUCCEEDED** um01:14:03,5741070UTC. Die900432206Bytes sind ebenfalls Unity-BuildReport.totalSize, keine von mir behauptete Dateisystemgröße eines IPA oder exportierten Projektarchivs. Gespeicherter eigener Texttransport2529899UTF8-Bytes/SHA256 `d5996d198d3ee48043bf337885583799035fbc01878b1812eb6d9b4b214d1d51`; kein ZIP-/IPA-/Store-/Signingnachweis behauptet.

Die abschließend selbst erneut abgefragte Jobs-API bestätigt alle zehn Jobs vonUnity38661531680 als **completed/success**, keinen ausstehenden oder fehlgeschlagenen Job:

| Job | Tatsächliche Job-ID | Ergebnis |
|---|---:|---|
| unity-environment-android |116183681750|SUCCESS|
| unity-environment-linux |116183682056|SUCCESS|
| project-preflight |116183682106|SUCCESS|
| unity-config |116183682147|SUCCESS|
| unity-environment-ios |116183682167|SUCCESS|
| unity-evidence-guard |116183762702|SUCCESS|
| unity-compile-editmode |116183762944|SUCCESS|
| unity-playmode-bootstrap-smoke |116186380899|SUCCESS|
| unity-android-development-il2cpp |116187282941|SUCCESS|
| unity-ios-export |116189910438|SUCCESS|

Finale eigene API-Ausgabe in `.utmp/wp032-independent-technical/b83-finaljobs.json`; vollständige Tooltext-Logtransporte und originale Test-ZIPs daneben bzw. am genannten ursprünglichen Artefaktpfad. Log-Dateihashes bezeichnen die gespeicherten dekodierten Texttransporte, ZIP-/XML-/APK-Hashes jeweils die tatsächlich oben bezeichneten Originaldatei-/Memberbytes. Keine dieser Größenbasen wird gleichgesetzt.

## Abschließende technische Reichweite

Zusammen mit dem unveränderten eigenen B83-Quellenbericht sind jetzt die technischen Quellen-/Anker-/1109-/967-Provenienz-/Konflikt-/9+21+7-Bindungs-/prospektiven Scope- und echten neuen CI-Prüfungen dieses WP-032-Standes bestanden. Kein offener technischer Befund und kein ausstehendes B83-CI-Schlussgate dieser Prüfung. Die separat abgegebene fachliche Rolle wird hier weder neu beurteilt noch simuliert. Dieses Addendum gilt ausschließlich für exaktB83/Treeff0d4cde und erteilt selbst keine Refmutation oder Mainintegration. Ein späterer reiner Dokumentationsfreeze erhält seine eigene eng begrenzte Nachprüfung; keine automatische PASS-Übertragung auf geänderte Inhalte.

O02 bleibt kausal offen; Humanreview/Spieltest,1440Referenzziele/Cross-Reference-FAIL,UNCALIBRATED/null-Zeitklasse sowie kreative/Import-/Produkt-/Release-/Geräte-/Storegates bleiben unverändert. Keine Main-/Runtime-/Produkt-/Releasefreigabe.
