# Prospektiver I-Plan – gespeicherter Public240-Rahmen, Szene und Tests

19.10.2026. Nur lesende Vorplanung durch den bisherigen A-/Atomic-Testimplementierer. Kein I-WP, keine reservierte ID, kein Anker und kein Fachcode. Dieser Entwurf liegt ausschließlich im ignorierten Root-Scratch. Erst tatsächliches P und der danach prospektiv gemeinsam verankerte I-WP-/Manifeststand erlauben Implementierung. R koordiniert; entry_audit bleibt unabhängig. Maßgeblich sind angenommene ADR034/035 und die tatsächlichen A-/S-Verträge; keine neue Produkt-/Import-/Releasefreigabe.

## Abgestimmtes Ergebnis

Ein neuer SavedPublic240Review-Einstieg erhält einen eigenen Statusrahmen, eigene Szene und eigenen Builder. A-Public240Review und DevelopmentPuzzle bleiben bytegleich und getrennt bedienbar. Der gespeicherte Rahmen verwendet den bestehenden A-Presenter, das bestehende Grid und DevelopmentTrainRide; es gibt keinen zweiten Rasterrenderer und keinen zweiten Domainzustand. SavedPublic240Review ist der erste normale interne QA-Entry. Welche alten QA-Szenen danach in der regulären Buildliste verbleiben, führt R ausdrücklich; der gemeinsame Entwurf erhält Public240Review, DevelopmentPuzzle und BootstrapComposition als getrennte QA-Entries. Kein unity.yml-Delta ist nötig.

## Prospektive Dateiliste und klare Eigentümer

Jeder folgende neue Pfad erhält eine eigene neue `.meta`, beide Pfade einzeln im späteren I-Manifest. GUIDs legt R für den tatsächlichen I-Stand vor Code eindeutig fest; keine bestehenden A-GUIDs umdeuten.

| Neue Datei | Eigentümer / Aufgabe |
|---|---|
| `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/SavedPublic240ReviewPresenter.cs` | UI-Owner: Savedrahmen, beobachtete Taskaktionen, Corepresenter-Composition, Status-/Progress-/Recoveryansicht; kein Dateisystem. |
| `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uxml` | UI-Owner: immer vorhandener Status-/Picker-/Recoveryrahmen und zunächst leerer Gamehost. |
| `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uss` | UI-Owner: Rahmenlayout, kompatible A-Stile, ausgeblendete Core-Navigation und Fehler-/Bestätigungsbereich. |
| `Assets/StammstreckenPuzzle/UI/SavedPublic240ReviewLocale.json` | UI-Owner: vollständige erforderliche A-Corekeys mit gespeicherten QA-Texten plus `qa.public240.saved.*`. |
| `Assets/StammstreckenPuzzle/Scripts/Editor/Build/SavedPublic240ReviewBuild.cs` | UI-Owner: eigene deterministische Scene-Erzeugung/Verify/Open-Menü; kein zusätzlicher Sceneprocessguard. |
| `Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity` | UI-Owner: neue Szene mit exakt einem neuen Savedinstaller und expliziten serialisierten Quellen. |
| `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/SavedPublic240ReviewPresenterTests.cs` | UI-Owner: tatsächliche Rahmen-/Core-Controls, Lokalisierung und Parentgates; bestehende Assemblygrenzen. |
| `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240ReviewSmokeTests.cs` | UI-Owner: echte Savedscene, reale temporäre Savebäume, persistierter UI-Roundtrip und native NormalScreen-/Ride-/Resultfälle. |
| `Assets/StammstreckenPuzzle/Scripts/Application/SavedPublic240ReviewSession.cs` | S-/App-Owner: Bridge, zusätzliche immutable Anzeige-/Statusrecords in derselben Datei. |
| `Assets/StammstreckenPuzzle/Scripts/Platform/LocalSaveRuntime.cs` | S-/App-Owner: instanzgebundene Stopwatch-/UTC-Uhr und Hauptthreaddispatcher. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Application/SavedPublic240ReviewSessionTests.cs` | S-/App-Owner: Bridge-/ACK-/Task-/Dispatch-/Lifecycleverträge, transitive Typen nur test-only Introspektion. |
| `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240Composition.cs` | R: tatsächliche workerfähige A-/S-Composition, keine Domainkante. |
| `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs` | R: Scene-/Task-/Lifecycle-/Dispatcher-/Worldowner, konkrete Validierung und guarded Testroot-API. |
| `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240BuildEntryTests.cs` | R: `#if UNITY_EDITOR`-Build-/Preflight-/Firstentry-/actualSceneProcessedtests im vorhandenen Bootstrap-PlayModeassembly, kein EditModeclaim. |

Bestehende Änderungen ausschließlich R: `StpBuildEntrypoints.cs` für Saved-Verify und tatsächliche erste Savedscene; `DevelopmentPuzzleBuild.cs` ausschließlich im vorhandenen `DevelopmentPuzzleSceneBuildCheck` für dessen guarded Savedscene-/Installer-/BuildReportanschluss; `ProjectSettings/EditorBuildSettings.asset` für tatsächlichen ersten Editorentry; `validate.yml` ausschließlich eigener neuer I-Selektor. Alte Builderbodies bleiben unverändert. Gemeinsame WP-/CURRENT_STATE-/Queue-/Handover-/I-Evidenzpfade benennt R vor Anker einzeln. Keine asmdef-/rsp-/World-/alten A-/S-/JCS-/Solver-/Textquellenänderung ist vorgesehen.

## Schlanker Savedrahmen statt A-Konvertierung

`SavedPublic240ReviewPresenter` bekommt das bereits geklonte neue Rahmentree, das explizite bestehende A-UXML, bestehende A-Styles plus eigene Savedstyles, eigene Savedlocale und die neue Applicationbridge. Der Installer serialisiert diese Abhängigkeiten; kein Resources-/Scene-/Service-Locator. Erst wenn `PuzzleView` echt vorhanden ist, wird A-UXML in den Gamehost geklont und der vorhandene `Public240ReviewPresenter` gebaut. Loading/NeedsInitialization funktionieren ohne Fakeattempt.

Der Rahmen besitzt den sichtbaren Picker einschließlich Öffnen/Fortsetzen sowie den asynchronen Neustart. `qa-picker-bar` und `qa-restart` des Core bleiben dauerhaft ausgeblendet; eigene Controls verwenden ausschließlich Bridge-Tasks und `State.Can*`. Das Core-Actiondelegate bleibt ebenfalls an dieselben geprüften Taskaktionen gebunden. Das direkte synchrone `IReviewPuzzleSession.Restart` im gespeicherten Modell lehnt benannt ab. So kann kein versteckter alter Callback die asynchrone Sicherungsgrenze umgehen.

Vor gesperrten Zuständen wird der ganze Gamehost per Parent deaktiviert. Ein Core-Refresh darf deshalb seine Kinder lokal aktivieren, ohne effektive Eingabefreigabe zu erzeugen. Sichtbarkeit von bestätigtem Result und aktuellem Pending-/Recoveryzustand wird zusätzlich am Rahmen gebunden. Der Rahmen liest je Refresh genau einen immutable State; State.Version/Savedstatus müssen auch ohne Änderung der Domainrevision sichtbar werden. BindSession erfolgt bei geändertem Puzzle-/Attempt-/Dimensionsschlüssel, auch wenn ein Bridgeproxy dieselbe Objektreferenz behält. Dadurch werden Selection und Rendercache nach Navigation/Restart sicher zurückgesetzt.

Die bestehende Localizationklasse akzeptiert ausschließlich `qa.public240.`. Die neue Quelle enthält alle vom Core/Grid benötigten Keys und neue `qa.public240.saved.*`, keine `qa.saved.*`-Einträge. Boundary-/Resulttexte behaupten nur die tatsächlich gespeicherte QA-Wahrheit; die alten unsaved-Texte werden über die neue injizierte Quelle ersetzt. Keine globalen Tabellen oder Produkttexte ändern. Rahmen und Core besitzen je eigene lokale Locale/SharedTableData/StringTable; beide Lifetimes werden auch bei Constructorfehler, deaktiviertem UIDocument und Installer-only-Destroy geprüft.

## Abgestimmte Applicationgrenze

Gemeinsamer ausführlicher Entwurf: [I_PROSPECTIVE_PLAN.md](I_PROSPECTIVE_PLAN.md). Neue Klasse `SavedPublic240ReviewSession(PersistedPuzzleSession, AutosaveCoordinator, IPublicPuzzleCatalog, ILocalSessionClock, ILocalSaveDispatcher)`. Keine Public240ReviewSession und kein zweiter Domainstate.

- `IReviewPuzzleSession? PuzzleView`, `IReadOnlyList<PublicPuzzleInput> Puzzles`, immutable `SavedReviewState State`.
- Status geschlossen: Loading, NeedsInitialization, Ready, Saving, CompletionPending, ProgressCommitted, TrainRide, Results, RecoveryRequired, Unsupported, Failed, Disposed.
- State enthält Version, Load-/Commitstatus, ErrorCode, alle `CanMutate/CanNavigate/CanRestart/CanRetryLoad/CanRetrySave/CanCreateProfile/CanRecover/CanReset/CanStartRide`-Gates, Dirty, SelectedIndex, CurrentIdentity, HasDraftForSelection, defensive Kandidaten und nullable bestätigte Progressprojektion.
- Progress besitzt exakte Profile-/Catalog-/Generationbasis; QA-Saldo, FirstClearCount/MemberCount und ausgewählte Sterne stammen ausschließlich aus einem bestätigten validierten Snapshot. Keine sichtbare Completionprojektion vor ACK; bei ungewisser Recovery null. Kein Ledgerhashing im UI-Frame.
- Beobachtete Taskaktionen mit CancellationToken: Initialize, CreateProfile, SelectPuzzle(index,restoreExistingDraft), Restart, RetrySave, RetryCompletion, Reload, RecoverAndPromote(kind), ConfirmedReset, TickAutosave, SetSuspended(bool).
- `BeginTrainRide(expectedAttempt)` prüft CanStartRide und S.CanPresentResults für exakt denselben aktuellen Attempt. IReview.CompleteTrainRide bleibt attemptgebunden. Domaincommands/-Evaluator/-Checkpointzugriffe vollständig in App; Bootstrap hat keine direkte Domainkante.

Eine Navigation pausiert den bisherigen echten Versuch am aktuellen Monotoninstant, notifiziert nur wirkliche Änderungen und wartet erforderlichen Flush ab. Erst dann wird der neue Versuch gestartet/restauriert. Fehler hält alte Auswahl/Session. Es gibt keine zweite Sammlung von240 In-Memory-Versuchen. Ein neuer leerer Restartcheckpoint wird ausdrücklich gesichert; sein Fehler bleibt sichtbar ungesichert und behauptet keine aktualisierte durable Wahrheit. UI-Auswahl selbst wird nicht im Save erfunden.

Zusätzlich gemeinsam festgelegt: Jede PuzzleViewinstanz bindet unveränderlich ihren ursprünglichen Attempt und die vollständige PublicIdentity. Navigation/Restart erzeugen einen neuen View. Ein alter View darf bei CaptureCommand/Apply/Undo/Pause/Resume/CompleteTrainRide niemals heimlich den inzwischen aktuellen Attempt neu erfassen; jede veraltete Wirkung endet benannt als STALE_ATTEMPT. Gespeicherte UI-Callbacks und sämtliche Worldabschlüsse werden gegen diese Bindung geprüft. Der Rahmen bindet den neuen Coreview ausdrücklich und übernimmt keinen verspäteten Erfolg des alten Views.

## Sichtbare Zustände und Eingriffe

| Zustand | Sichtbares Verhalten / zulässige Aktion |
|---|---|
| Loading | Statusrahmen sichtbar; kein Spielmodel, keine Tool-/Ergebnisfreigabe. |
| NeedsInitialization | „Lokales QA-Profil beginnen“ löst ausschließlich explizites CreateProfile aus; kein Awake-Autosave. |
| Ready ohne PuzzleView | Eigener240erPicker und Öffnen/Fortsetzen; keine erfundene zuletzt gespeicherte Auswahl. |
| Ready mit PuzzleView | Tatsächliche Zellen/Counts/Tools; Status trennt Dirty/Speichern/zuletzt bestätigt. |
| Saving/CompletionPending | Exakte Bridgegates anwenden; Completion sperrt Eingaben, Navigation, Neustart, Fahrt und Ergebnis. |
| ProgressCommitted | Nur bestätigte QA-Fortschrittswerte; danach exakt gebundener Worldstart. |
| TrainRide/Results | Tatsächliche Spielerstrecke; Ergebnis erst nach bestätigtem Commit und abgeschlossener eigener Fahrt. |
| Sicherer Speicherfehler | Eigener Retrybutton; keine vorweggenommenen Rewards/Results. |
| RecoveryRequired/Conflict | Frischer Load; aktuelle vollständige Observation, keine Commitzeitpunkt-/Cachebehauptung. |
| SplitBrain | Drei Kandidaten mit Kind/Status/Generation/gekürzter Sourcekennung; konservativer Main ist keine Spielberechtigung. |
| Unsupported/Failed | Klare Lesesperre, erneutes Laden möglich; keine Auswahl um diesen Kandidaten herum und kein Resetbutton. |
| Bekannte Corruptlage | Getrennter sichtbarer Reset-Bestätigungsschritt mit Abbrechen; erst ausdrückliches Bestätigen ruft ConfirmedReset. |

Recoveryauswahl verwendet nur Valid-Kandidaten aus dem aktuellen Gate; zuerst rein lesendes Recover, danach explizite exakte Promotion. Jede stale Änderung inklusive nicht ausgewählter Envelopebytes verlangt neue Beobachtung/Entscheidung. Nach Load/Promotion/Create wird bewusst neu geöffnet/fortgesetzt. Geladene bestätigte FirstClears starten weder Fahrt noch Grant automatisch. Bei restauriertem SolvedPendingCommit muss die bestehende S-ACK-/Replaygrenze durchlaufen werden; ein FirstClearboolean allein öffnet keine Results.

## Bootstrap, World und Tasklifetime

Unity-Hauptthread kopiert TextAsset.bytes und erfasst persistentDataPath/version/buildGUID/serialisierte UI-Referenzen. Worker validiert Public240/Reviewkataloge, Policy/Codec, provisioniert ausdrücklich nur den privaten Root und konstruiert SaveFileSystem/Repository. Keine Unitygetter im Worker und keine Datei-/Hash-/Katalogarbeit im Render-Update. Profil exakt `SaveProfileIdentity("qa-public240-v1","r",IsolatedReview)`; FS hängt selbst Namespace/Aggregatehash/r an. Kein zusätzlicher Bootstrap-Unterpfad. EditorbuildId `editor-public240-review`, im Player tatsächliche Builddiagnose.

`LocalSaveRuntime` liefert Stopwatch-Monotonie und striktes UTC für Diagnose; instanzgebundene Dispatcherqueue mit kontrolliertem Dispose. Installer.Update pumpt die Queue **vor jeder Nullmodel-/Statusfrühprüfung**, damit Coldload-/Capturetasks nicht deadlocken. UI-Aktionen und Autosave haben beobachtete Taskowner, keine async-void-, Wait-/Result- oder disposed-Callbackpfade. Bei Dispose werden wartende Publikationen kontrolliert beendet; ein möglicherweise wirkender Write gilt nicht als rückgängig.

Worldcode bleibt unverändert. R erfasst bestätigten Attempt/Pfad, konstruiert DevelopmentTrainRide und ruft BeginTrainRide für exakt diese Bindung; bei Ablehnung wird der gerade erzeugte Ride entsorgt. Eine Allokationsstörung darf keine gespeicherte Rewardtransaktion erneut auslösen. Suspension stoppt Worldticks. Fahrtende beendet den eigenen Ride unmittelbar mit Dispose/null und attemptgebundenem CompleteTrainRide; kein alter fertiger Ride für sofortigen Restart/erneutes Lösen. Keine persistierte Animationsphase.

## Konkreter privater Root und isolierte echte Szenentests

R bindet vor I-Code ein natives Startgate am tatsächlichen `Application.persistentDataPath`: Längen und reale FS-Proben für Main/Backup/Pending, beide Archivbereiche, alle drei Kind-/Vollhashdateien und kurze Stagingnamen. Profil r verkürzt die tatsächliche QA-Pfadform; es ersetzt keinen konkreten Beleg. Bei unzureichendem Root sichtbarer benannter Fehler, kein stiller Speicherortwechsel und kein allgemeiner Longpathclaim. Vorhandene Profil-Rohbytes werden bei Proben nicht angetastet; Probeaufbau/-abbau ist ein gesonderter explizit kontrollierter Test, kein Bestandteil des geprüften Read-only-Loads.

R stellt am neuen Installer ausschließlich unter `#if UNITY_EDITOR && UNITY_INCLUDE_TESTS` die genaue API `public static IDisposable UseTestPrivateRoot(string absoluteRoot)` bereit: QA-only, bestehender privater Root, echte Reparse-/FS-Prüfung, unter OS-Temp oder explizitem STP_TEST_TEMP_ROOT, single Scope/keine Nestedscopes. Override wird pro Instanz bei Awake vor Workerstart erfasst. In Player existiert kein Testpfad. Test setzt Scope vor tatsächlichem Savedscene-Load und entsorgt ihn erst nach Destroy/Task-/Dispatchercleanup. Kataloge, Profile r und sämtliche S-Adapter bleiben real. Alternative inaktive explizite ConfigureForQa-Composition ist für lokale Kontrollfälle möglich.

## Gezielte I-Tests des UI-/Scene-Owners

1. Echte Scene coldload: Scope/tempRoot vor Awake, NeedsInitialization ohne Profil-/Savefile; reale Createbuttonaktion erzeugt erst Generation1. Alle Referenzen und240erPublic-/Campaign-/Completionbindung konkret prüfen.
2. Echte Buttons/Pointer: beide Rastergrößen, Marker/Felder/Batch/Undo/No-op/Selection; eigener Picker wartet erforderlichen Flush, bewahrt alte Auswahl bei Failure/Cancel. Fortsetzen nach Hostdestroy/Neuaufbau bewahrt Checkpoint/Undo/Flags/Revision und schließt Downtime aus.
3. Save-before-ride ohne zusätzlichen Runtimefake: zweiter realer SaveFileSystem auf demselben TempRoot/Profile r/Aggregatekatalog hält tatsächliche Writersperre vor UI-Solve. Während blockierter Completion sichtbares Pending, keine Fahrt/Results/Rewardprojektion; nach Release genau bestätigter Commit und eigene Strecke. Bestehende Bootstrap-Testassembly hat Persistence/App-Kanten; keine neue Kante nötig.
4. Reale Kandidaten im TempTree für Corrupt/Unsupported/Failed/SplitBrain; sichtbare Auswahl/Resetbestätigung, keine Dateiwirkung beim Anzeigen/Abbrechen, Quarantäne nach bestätigtem Reset. Stale ausgewählter/nicht ausgewählter Envelope verhindert Promotion. Fehler/Conflict/Reentry kann keine alte Session freigeben.
5. Native NormalScreen4x4/10x10/Fahrt/Result mit ScreenCapture erst nach tatsächlich gerendertem Screenframe/WaitForEndOfFrame, targetTexture=null für Panel/Screenkamera. Headless bleibt NOT_EXECUTED für Grafik. R betrachtet Originale; kein Popup-/Human240claim aus geschlossenem Pickerbild.
6. Dispose: zerstörter Host, nur Installerkomponente, vorher deaktiviertes Document, Constructor-/Initialisierungsfehler und verspätete Task-/Ridecallbacks. Beide lokalen Tabellen und eigene Kameras/Trainressourcen freigeben. No-yield Results→Restart→erneutes Lösen mit echter neuer Fahrt.
7. Builder: eigene neue Scene/meta exakt reproduzierbar, zweimal und sauberer Reimport; zusätzlich tatsächlicher sauberer Creationzweig im isolierten Scratch gegen vorgebundene Rohbytes. Bestehende A-Panel-/Theme-/Train-/Scene-/Presenterbytes unverändert. Keine Repositorylocks/Authoring-/Proof-/Notiz-/Fixturepayloads in Playerdependencies.

Root prüft separat tatsächliche erste Savedbuildscene, QA-Identifier, Savedinstallerzahl und echte BuildReport-Verarbeitung über seinen bestehenden guarded Processor. App-Owner prüft ACK-/CAS-/Recovery-/Dispatcher-/Autosavegrenzen mit kontrollierten Tasks. Allgemeine Solver-/Text-QC wird nicht wiederholt. Später erforderlich: unabhängige I-Review, kanonische Checks, vollständige reguläre I-CI, zuständige Nicht-main-Unterintegration mit integrierter Kontrolle. Diese Planung erfüllt kein solches Gate.

## Gelesener Stand

Aktuelle A-Quellen und S-APIs wurden vollständig beziehungsweise für die im Plan benannten unveränderten Sourceabschnitte gelesen. Die nachfolgende Roh-/Git-LF-Hashliste bindet diesen konkreten Inventurstand; gemeinsame Quellen nach tatsächlichem P werden vor I-Code erneut gegen den Anker geprüft. I_PROSPECTIVE_PLAN ist ein gemeinsam weiter abgestimmter Scratchentwurf, keine angenommene ADR.

| Inventur | Sourcepfad unter Assets/StammstreckenPuzzle | SHA256 native Rohbytes | SHA256 nur CRLF→LF |
|---|---|---|---|
| A | Scripts/Application/Public240ReviewSession.cs | f1498d4273686ddedb914fd621f060a68b62e7b34b8c4fe5e48ea57d6decac1d | f1498d4273686ddedb914fd621f060a68b62e7b34b8c4fe5e48ea57d6decac1d |
| A | Scripts/Bootstrap/Public240ReviewInstaller.cs | 5008fbc9baafd4e65666e786747bb659dd70211407574a21f5a904480096bb6f | 5008fbc9baafd4e65666e786747bb659dd70211407574a21f5a904480096bb6f |
| A | Scripts/Presentation/UI/Public240ReviewPresenter.cs | 944a63f484c3ea283271b48ccc2aadb48a489623482ca3474a0fa3059ea38b5a | 944a63f484c3ea283271b48ccc2aadb48a489623482ca3474a0fa3059ea38b5a |
| A | Scripts/Presentation/UI/Public240ReviewGrid.cs | a57e57e7956979bf83cb3e3411118fceb3694906bd5de7cd651285b01b90ec98 | a57e57e7956979bf83cb3e3411118fceb3694906bd5de7cd651285b01b90ec98 |
| A | Scripts/Presentation/World/DevelopmentTrainRide.cs | 326c5d43a00e32162662bd2c3d9310f695f21db23c748f2088f6e6b97054a369 | 647e4bb5ccfbb1c740c730dbe2ea2948f76215af739fe057b993c21faa2801b2 |
| A | Scripts/Editor/Build/Public240ReviewBuild.cs | 4cf45282a77464775f1fd26bda3d95d5030feef5b3f9fb40868fffa58b9625d0 | 4cf45282a77464775f1fd26bda3d95d5030feef5b3f9fb40868fffa58b9625d0 |
| A | Scripts/Editor/Build/StpBuildEntrypoints.cs | e82dc43f6051dfdce913ac40fc13bff3fecd011917f9a1405b0db69a0582564a | 55d0eacdccf64ec817dc6957ea4df9dbcbec4cba5b9408b0c9c4c5b930d1b40d |
| A | Tests/PlayMode/Bootstrap/Public240ReviewSmokeTests.cs | 11a77f1a4472f05c2fd38f5a071bf97f8cb4e894a090885ed8197c8633ba8c01 | 11a77f1a4472f05c2fd38f5a071bf97f8cb4e894a090885ed8197c8633ba8c01 |
| A | Tests/PlayMode/Presentation/Public240ReviewPresenterTests.cs | d5a8383bad4c5757512b2ae0f32da47784380e47fcb0867e14dd17a508a5cc08 | d5a8383bad4c5757512b2ae0f32da47784380e47fcb0867e14dd17a508a5cc08 |
| S | Scripts/Application/PersistedPuzzleSession.cs | 5c3b0463e41f44ebe84ac7d80c4d48057ddb215135a4953198830f7b51c8ed20 | 7da861fcfef7b723c4c01776003c90f0dc07e5940e7c836c797fc23857fdbbea |
| S | Scripts/Application/AutosaveCoordinator.cs | 11f6d688fb6c100cf4ed332640520b8ba61fe76e954788f0aaa2051b5e24765e | fc6d154f466e00780615762d34ecf42c3c944d06c31236c4f7beb916310ec084 |
| S | Scripts/Application/LocalProgressPolicy.cs | c993039cb9efe746018c31edc12dbd85b1cca5f185fd554566ab2e1d6387aa8e | a917356df50bf02e50255b3b00c83b6fa5ab93cb9dc1af4759ffe2983be1b6df |
| S | Scripts/Infrastructure/Content/ReviewProgressCatalog.cs | 5184b3c38c18ead0b990a02cc85229870fc59d0105ac7d27e20c35fd76183b53 | d6f5572bbfcf9c77ebb5e1c48403953e20e94827801dac0b487fa8c4076d7022 |
| S | Scripts/Infrastructure/Persistence/SaveFileSystem.cs | 537dd73cac4968f1fbe5fb72711a4bf16e3e1497ac20b691d4cbf5dc527ffed0 | 537dd73cac4968f1fbe5fb72711a4bf16e3e1497ac20b691d4cbf5dc527ffed0 |
