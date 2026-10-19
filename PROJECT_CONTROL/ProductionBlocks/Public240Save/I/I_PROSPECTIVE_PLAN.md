# Prospektiver I-Verdrahtungsentwurf – gespeicherte Public240-QA

Stand: 19.10.2026, reine lesende API-/Kanteninventur durch den S-Implementierer nach eigenem Lieferfreeze. **Kein I-WP, keine neue ID, kein Anker, kein Codeauftrag und keine Sourceänderung.** Dieser Entwurf liegt ausschließlich im ignorierten Rootverzeichnis. R entscheidet die exakten technischen Details und verankert nach tatsächlichem P erst das eigene I-WP/Manifest. Gemeinsame aktuelle Quellen werden erst auf P maßgeblich; die gelesenen A-/S-Arbeitskopien sind im begleitenden JSON mit Rohchecksummen gebunden.

Maßgeblich: MANUS-2026-10-19-01 §§4–6, angenommene ADR034/035, DELIVERY_CONTRACT, T_TECHNICAL_DETAILS, S-Liefervertrag und tatsächliche APIs. Keine neue Produktregel, gespeicherte UI-Selection, Generator-/Provideroperation oder Aufweichung der Import-/Releasegates ist erforderlich.

## 1. Empfohlener schmaler Aufbau

R hat während dieser rein prospektiven Abstimmung konkretisiert: **neue SavedPublic240Review-Szene, neuer Installer/Presenterrahmen/Builder und eigene Saved-UI-Assets**. Die vorhandene A-Public240Review-Szene einschließlich Installer/Presenter/Builder bleibt als tatsächlich angenommene ungespeicherte Lieferung bedienbar; ebenso DevelopmentPuzzle. Der neue normale QA-Playerentry ist SavedPublic240Review. Bestehende Panel-/Grid-/Train-/Publicdataassets werden wiederverwendet.

Ein neuer **SavedPublic240ReviewSession** im Applicationmodul implementiert die vorhandene A-Grenze **IReviewPuzzleSession** durch die tatsächliche S-Usecaseinstanz. Er erzeugt keine Public240ReviewSession und besitzt keinen zweiten Domainzustand. Alle Zell-/Batch-/Undo-/Lifecyclecommands laufen über PersistedPuzzleSession.Handle; alle Save-/Completion-/Recoverywirkungen über die bestehenden S-Methoden und genau einen AutosaveCoordinator. Zusätzliche immutable Anzeige-/Statusrecords in derselben neuen Applicationdatei haben keine eigene Fortschrittsautorität.

Der neue SavedPublic240ReviewPresenter bildet den gespeicherten Rahmen. Er erzeugt den unveränderten A-Public240ReviewPresenter erst bei einer echten nichtnull PuzzleView innerhalb eines eigenen Gamehosts. Eigene gespeicherte Picker-/Restartcontrols führen ausschließlich beobachtete Taskcommands aus; **Corepicker und Corerestart sind dauerhaft ausgeblendet**. Der Gamehost wird bei Sperre auf Elternebene deaktiviert, sodass ein Core-Refresh seine Kinder nicht versehentlich aktivieren kann. Die eigene Savedlocale enthält alle erforderlichen Corekeys mit wahrheitsgemäßen gespeicherten QA-Texten sowie neue **qa.public240.saved.***-Keys. Wichtig: der vorhandene Localizationadapter akzeptiert ausschließlich den Prefix qa.public240.; qa.saved.* würde selbst als unbenutzter Eintrag abgewiesen. Frame und Core besitzen getrennt entsorgte lokale Tabellen. DevelopmentPreviewPhase bleibt unverändert vierwertig; Speicherung hat eine eigene Statusprojektion.

Zwei kleine konkrete Adapter in STP.Platform liefern Uhr und Hauptthreadqueue. Die bestehende Bootstrapkante zu Platform reicht aus. Eine neue reine Bootstrap-Compositionfactory macht den tatsächlichen Public-/Campaign-/Completion-/Profile-/Repositoryaufbau separat prüfbar. Das Scene-MonoBehaviour hält nur Lifecycle, UI, Taskbeobachtung, Dispatcherdrain und Ride.

## 2. Exakte technische Pfadliste für den späteren I-Anker

### Neue Runtimequellen, jeweils mit neuer `.meta` – insgesamt sechs C#-Dateien

1. `Assets/StammstreckenPuzzle/Scripts/Application/SavedPublic240ReviewSession.cs` – IReviewPuzzleSession-Bridge; zusätzliche gespeicherte View-/Status-/Actiongrenze und immutable QA-Fortschritts-/Recoveryanzeige in derselben Datei.
2. `Assets/StammstreckenPuzzle/Scripts/Platform/LocalSaveRuntime.cs` – LocalSaveClock und MainThreadSaveDispatcher, normale instanzgebundene Klassen ohne Singleton/Locator.
3. `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240Composition.cs` – workerfähiger konkreter Aufbau aus kopierten UTF8-Quellen/Profil-/Rootdiagnosen; typed Rückgabe ausschließlich mit Application-/Platform-/Content-/Persistence-Typen, ohne Domainkante.
4. `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs` – neues Scene-MonoBehaviour mit beobachteten Tasks, Initialisierung/Lifecycle/Dispatcherdrain, UI und tatsächlichem Commit-vor-Ride.
5. `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/SavedPublic240ReviewPresenter.cs` – Savedstatus-/Recoveryrahmen und unveränderter A-Presentercore wie oben; keine neue Raster-/Regelautorität.
6. `Assets/StammstreckenPuzzle/Scripts/Editor/Build/SavedPublic240ReviewBuild.cs` – eigener deterministischer Builder/Verify; konsumiert Bootstrapvalidierung ohne neue Assemblykante. Der tatsächliche Scene-BuildReportcheck liegt ausschließlich bei Roots engem bestehenden DevelopmentPuzzleSceneBuildCheck-Anschluss, kein zweiter Guard.

### Neue Assets, jeweils mit neuer `.meta`

1. `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uxml` – gespeicherter Rahmen mit eigenem Picker/Restart-/Fortsetzen-/Retry-/Recovery-/Bestätigungsbereich und eingebettetem ungespeicherten Coretree als bloßer unveränderter Tool-/Gridstruktur.
2. `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uss` – Savedstatus-/Recoverylayout, Parentenablegrenzen und dauerhaft ausgeblendete Corepicker-/restartcontrols; vorhandene A-Stile als explizite Abhängigkeit wiederverwenden.
3. `Assets/StammstreckenPuzzle/UI/SavedPublic240ReviewLocale.json` – alle erforderlichen Corekeys plus qa.public240.saved.*; klare QA-/Bestätigt-/Ungewiss-/Recoverytexte, keine Änderung globaler Tabellen.
4. `Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity` – neuer eigener Scene-GUID, ein neuer Savedinstaller, eigene UI/Locale und echte A-Public-/S-Campaign-/S-Completionassets; Panel/Material aus A/Development unverändert referenzieren. R legt die GUIDs vor Code fest.

### Genau zu ändernde vorhandene technische Dateien

1. `Assets/StammstreckenPuzzle/Scripts/Editor/Build/StpBuildEntrypoints.cs` – regulärer QA-Preflight prüft Savedartefakte; normale Szenenliste beginnt SavedPublic240Review und erhält die getrennten Public240Review/DevelopmentPuzzle/BootstrapCompositionentries.
2. `Assets/StammstreckenPuzzle/Scripts/Editor/Build/DevelopmentPuzzleBuild.cs` – ausschließlich enger DevelopmentPuzzleSceneBuildCheck-Guardanschluss an die neue normale Szenenliste, keine Änderungen alter Developmentartefakte oder deren Reproduktion. Exakter Hunk durch R vor I-Code zu prüfen.
3. `ProjectSettings/EditorBuildSettings.asset` – SavedPublic240Review als erster aktivierter normaler Editorentry; die bisherigen separaten QAentries bleiben ausdrücklich erreichbar.
4. `.github/workflows/validate.yml` – Selektor auf das dann tatsächlich verankerte I-Manifest, keine Gate-/Runner-/Versionsabsenkung.

Alle bestehenden A-Dateien einschließlich Scene/Meta/Installer/Presenter/Builder/UXML/USS/Locale sowie sämtliche S-Fachquellen bleiben unverändert. Diese Aufteilung ist die inzwischen ausdrücklich vorliegende R-Scopedesignentscheidung und ersetzt den zuerst erwogenen direkten A-Anschlussentwurf.

### Neue Tests, jeweils mit `.meta`

1. `Assets/StammstreckenPuzzle/Tests/EditMode/Application/SavedPublic240ReviewSessionTests.cs` – reine Bridge-, Savedstatus-, Command-/ACK-/Selection-/Restart-/Recoveryraces; bei fehlender direkter Kante vorhandene test-only Introspektion. Enthält auch die Platformqueue-/Uhrfixture über Introspektion, keine neue Testassembly.
2. `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/SavedPublic240ReviewPresenterTests.cs` – echte Savedrahmen-/Core-UI-Aktionen, Status-/Kandidaten-/Resetbestätigung, Parentgates und Localeverträge; echte 4×4-/10×10-Layouts. Bestehende A-Regressionen bleiben unverändert.
3. `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240ReviewSmokeTests.cs` – tatsächliche Scene/Composition, echter isolierter temporärer Dateibaum, Restart/Undo/Timer/Lifecycle, Savefehler und Commit→Ride→Results, Builder-/normaler QA-Entry und Profilrejektion. Editor-Buildtests ausschließlich über test-only Introspektion, weil keine direkte Editor.Build-Kante vorhanden ist.
4. `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240BuildEntryTests.cs` – ausdrücklich Rootowned, unter UNITY_EDITOR in der vorhandenen Bootstrap-PlayModeassembly; separate tatsächliche Buildentry-/SceneProcessed-/Preflight-/Profilgateprüfungen über test-only Introspektion. Kein EditModelabel und keine neue Assemblykante. Die Flow-/Presentationdateien davor gehören UIowner.

Bestehende A-/S-/T-Tests und deren Hilfen bleiben unverändert. Test-only feste Commands dürfen bestehende tatsächliche Rätselbelege nutzen; keine Authoringlösung oder Testfixture wird von Runtime referenziert. Falls beim I-Anker zusätzliche Testhilfsdateien nötig werden, muss R sie vor Code exakt benennen; der Entwurf benötigt sie derzeit nicht.

### R-geführte Status-/Evidenzpfade vor Anker exakt ergänzen

Vorhandene `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` und `PROJECT_CONTROL/ProductionBlocks/Public240Save/HANDOVER.md` sind die gemeinsamen Abschluss-/Übergabepfade. Die tatsächlich vorhandenen A-/S-WPs `WORK_PACKAGES/WP-041_Public240-interne-Prueffassung.md` beziehungsweise `WORK_PACKAGES/WP-042_Lokaler-Save-Restore-Grundfortschritt.md` dürfen nur durch R nach tatsächlicher Annahme formal fortgeschrieben werden. R legt freie I-ID, konkreten I-WP-Pfad, Scope-JSONpfad und einzelne eigene I-Evidenzpfade fest. Hier werden keine solchen IDs/Pfade vorreserviert. Keine ADR-/Registryänderung ist aus dieser Verdrahtung bereits begründet.

## 3. Tatsächliche APIs und prospektive Bridgeoberfläche

Bestehend, unverändert nutzbar:

- `IReviewPuzzleSession`: Puzzle/Width/Height/EndpointA/B, Phase, LastCommand/Error, AttemptId/Revision/TimerStarted/IsPaused/CanUndo, aktuelle Counts/Zellen, ElapsedAt, CaptureCommand, Apply/ApplyBatch/Undo/Pause/Resume/Restart/CompleteTrainRide. DevelopmentEndpoint besitzt einen internal Domainendpointkonstruktor; die neue Bridge im selben Applicationmodul kann ihn direkt verwenden.
- `PersistedPuzzleSession`: LoadAsync, CreateProfileAsync, ConfirmedResetCorruptProfileAsync, Start, RestoreDraft, Handle, SaveDraftAsync, CommitCompletionAsync, RecoverAsync, PromoteRecoveryAsync; Snapshot, Session, RecoveryObservation, Status, CanMutate und CanPresentResults.
- `AutosaveCoordinator`: NotifyMutation, TickAsync, FlushAsync, CommitCompletionAsync und IsDirty.
- `ReviewProgressCatalog(publicCatalog,campaignUtf8,completionUtf8,canonicalJson)`: Campaign, Completion und CatalogSet. A-Publickatalog wird ausschließlich als IPublicPuzzleCatalog injiziert.
- `SaveFileSystem.ProvisionPrivateRoot(root)` und `SaveFileSystem(root,profile,aggregateCatalog)`; `AtomicSaveRepository(files,codec,clock,appVersion,buildId)`.
- `Public240ReviewPresenter(root,model,locale,clock,puzzles,openPuzzle)`, BindSession, Refresh und SetTrainTexture; `DevelopmentTrainRide(path,endpointA,endpointB,width,height,material,duration)`.

Schlanke **zwischen App- und UI-Planung abgestimmte prospective** neue Application-API, durch R vor I-Code endgültig festzulegen:

- Konstruktor `SavedPublic240ReviewSession(PersistedPuzzleSession persisted, AutosaveCoordinator autosave, IPublicPuzzleCatalog catalog, ILocalSessionClock clock, ILocalSaveDispatcher dispatcher)`.
- Properties `IReviewPuzzleSession? PuzzleView`, `IReadOnlyList<PublicPuzzleInput> Puzzles` und immutable `SavedReviewState State`. PuzzleView bleibt vor echtem Start/Restore null; Coldload benötigt keinen Fakeattempt.
- Geschlossener `SavedReviewStatus`: Loading, NeedsInitialization, Ready, Saving, CompletionPending, ProgressCommitted, TrainRide, Results, RecoveryRequired, Unsupported, Failed, Disposed. Savedstatus und DevelopmentPreviewPhase bleiben getrennt.
- `SavedReviewState`: `Version`, `Status`, `SaveLoadStatus? LoadStatus`, `SaveCommitStatus? CommitStatus`, `ErrorCode`; `CanMutate/CanNavigate/CanRestart/CanRetryLoad/CanRetrySave/CanCreateProfile/CanRecover/CanReset/CanStartRide`; `IsDirty`, `SelectedIndex`, `PublicPuzzleIdentity? CurrentIdentity`, `HasDraftForSelection`, nullable `SavedReviewProgress Progress` sowie defensive `IReadOnlyList<SaveCandidateProvenance> RecoveryChoices`. UI bestätigt keinen Erfolg aus einem rohen Status allein.
- `SavedReviewProgress`: exakt `SaveProfileIdentity Profile`, `CatalogIdentity Catalog`, `long Generation` der bestätigten und validierten Snapshotbasis; `BalancePatience`, `FirstClearCount`, `MemberCount`, `PublicPuzzleIdentity SelectedIdentity`, nullable `SelectedStars`. Keine Completionprojektion vor ACK veröffentlichen. Bei blockierter unklarer Recovery ist Progress null; ein sicher fehlgeschlagener Write darf weiterhin nur den klar als zuletzt bestätigt markierten alten Snapshot anzeigen.
- Alle folgenden Methoden liefern beobachtete `Task` mit CancellationToken: `InitializeAsync(ct)`, `CreateProfileAsync(ct)`, `SelectPuzzleAsync(int index,bool restoreExistingDraft,ct)`, `RestartAsync(ct)`, `RetrySaveAsync(ct)`, `RetryCompletionAsync(ct)`, `ReloadAsync(ct)`, `RecoverAndPromoteAsync(SaveCandidateKind selectedKind,ct)`, `ConfirmedResetAsync(ct)`, `TickAutosaveAsync(ct)`, `SetSuspendedAsync(bool paused,ct)`.
- Zusätzlich `DevelopmentCommandStatus BeginTrainRide(Guid expectedAttempt)`: nur aktueller, exakt bestätigter Attempt mit CanStartRide/CanPresentResults darf von ProgressCommitted zur Präsentationsphase TrainRide wechseln. Root erzeugt den Worldride und verwirft ihn bei zwischenzeitlich ungültiger Bindung. Abschluss weiterhin über die vorhandene gebundene PuzzleView.CompleteTrainRide-Methode. Keine persistierte Animationsphase.
- Die zwingend synchrone IReviewPuzzleSession.Restart-Methode darf keine unüberwachte asynchrone Aktion vortäuschen. Der gespeicherte Presenter bindet stattdessen den expliziten Taskcommand über einen beobachteten UI-Requesthandler. Die synchrone Methode kann im gespeicherten Pfad benannt ablehnen; bisheriger transienter Sechsargumentpfad bleibt unverändert.

Alle asynchronen UI-Aktionen besitzen einen einzelnen beobachteten Taskowner/Busyguard; keine unobserved async-void-Exception, kein `.Wait()`/`.Result` im Frame und kein Weiterarbeiten auf einem disponierten Presenter. Die S-Writerserialisierung bleibt die einzige Persistenz-/CASautorität. Ein Bridgeguard serialisiert nur konkurrierende Nutzer-Navigation/Recovery/Restart/Completionrequests.

Jede nichtnull PuzzleView ist an **genau eine immutable AttemptId und ein Publicidentitytripel** gebunden. Nach Start/Restore/Restart wird ein neuer Viewadapter veröffentlicht; ein verspäteter Callback auf dem alten Objekt darf niemals aus einer inzwischen neuen Session ein frisches Commandenvelope gewinnen. Alte Viewcommands und Ridecallbacks lehnen STALE_ATTEMPT ab, statt auf den jeweils aktuellen Versuch umzubiegen. Readprojektionen eines alten Views werden nicht mit neuer Definition/Rastergröße vermischt. Das ist keine zusätzliche Sessionsammlung, sondern eine begrenzte Callback-/Viewbindung über die ohnehin immutable Domainwerte.

## 4. Worker, Hauptthread und physische Isolation

Im Unity-Hauptthread werden ausschließlich TextAsset.bytes in private Arrays kopiert sowie `Application.persistentDataPath`, Version/Builddiagnose und die serialisierten UI-/Materialreferenzen erfasst. Auf Worker: Public240ReviewData.VerifyAssetBytes und tatsächlicher A-Katalog, ReviewProgressCatalog, Profil-/Aggregatebindung, LocalProgressPolicy/SaveLedgerCodec, LocalSaveCodec, private Rootprovisionierung und SaveFileSystem/AtomicSaveRepository. Keine Unityobjekt-/TextAssetgetter im Worker. Kein Profile-/Savefile beim normalen Start erzeugen.

Von R in dieser prospektiven Abstimmung konkret vorgegeben: `SaveProfileIdentity("qa-public240-v1","r",CatalogUse.IsolatedReview)`. Root wird als unveränderter absoluter `Application.persistentDataPath` an Persistence gegeben: **nicht** in Bootstrap erneut `qa-public240-v1` oder Hash anhängen. SaveFileSystem erzeugt bereits `<persistentDataPath>/qa-public240-v1/<aggregateCatalogHash>/r/`. Kein Root-/Pfadzugriff in Application/UI; keine Produkt-/ContractTestpromotion oder Export-/Importfunktion. R verlangt vor realem Savedentry zusätzlich ein tatsächliches natives Archive-/Quarantäne-/Migration-Source-MAX_PATH-Startgate am echten Root. Ein kurzer Test-Temproot ersetzt diesen Nachweis nicht. Bootstrap erfindet dafür keine zweite Pfadgrammatik; bei unzureichendem echten Rootpfad muss R die technische Grenze vor I-Code klären.

`LocalSaveClock` benutzt eine instanzgebundene Stopwatch für MonotonicMilliseconds und striktes UTC für Diagnose. `MainThreadSaveDispatcher` erfasst den Hauptthread bei Konstruktion, führt dortige InvokeAsync inline aus, stellt Worker-Actions mit TaskCompletionSource/RunContinuationsAsynchronously in eine instanzgebundene ConcurrentQueue und leert sie im Installer-Update ausschließlich am erfassten Thread. Dispose beendet ausstehende Publikationstasks kontrolliert; keine statische Queue und kein angenommener SynchronizationContextwechsel.

Rootprovisionierung und Dateisystemkonstruktion liegen ebenfalls auf Worker; die FS-Konstruktion prüft echte Verzeichnis-/Reparsegrenzen. Die aktuelle nach R-Prüfung angenommene FS-/Repositoryversion auf P ist maßgeblich; keine Übertragung alter V3-Supplement-PDBs auf Rootpatches.

Runtime erhält tatsächliche Application.version/buildGUID; Editor-buildId ist von R konkret `editor-public240-review` vorgegeben, statt einen nicht existierenden Playerbuild zu behaupten. Die Appversion bleibt tatsächlich erfasst und nichtleer. Diese Strings wirken nicht auf Timer, Eligibility, Rewards oder Savegeneration.

## 5. Bedienbarer Zustandsablauf

**Loading/ConfigurationFailure:** Spielcontrols gesperrt; sichtbarer lokalisierter Status. Die Bridge kann vor Sessionstart Publiccounts/Identität zeigen, aber keine spielbare Fakeattempt oder Fortschrittswahrheit erzeugen. Fehlende/falsche Assets/Profile verhindern Composition vor Dateiwirkung.

**NeedsInitialization:** sichtbarer Button „QA-Profil beginnen“, ausschließlich expliziter CreateProfileAsync. Kein automatische Initialsave im Awake/Update, kein Resetfehlerfallback. Bestätigte Initialisierung erzeugt allein Generation1; danach eigene Puzzleauswahl/Start.

**Loaded/Ready:** alle240 technisch zugänglich, keine neuen Stern-/Unlockregeln. Picker bietet gespeicherte genaue Draftidentität zum bewussten Fortsetzen; es gibt keinen Save-v2-Slot für „zuletzt ausgewähltes Puzzle“. Keine Persistenz dieses UI-Details erfinden. Beim Fortsetzen tatsächliches RestoreDraft und Resume mit frischem monotonem Anker, sobald die Anwendung sichtbar/fokussiert ist.

**Mutation/Autosave:** jeder tatsächlich geänderte State/Revision wird genau einmal notifiziert; No-op/Toolauswahl/Selection nicht. Commands binden AttemptId/CommandId/Revision und dürfen bei Busy/Recovery/Completion nicht ausgeführt werden. ElapsedAt ist nur eine Anzeigeprojektion aus öffentlichem ActiveElapsedMs plus lokalem letzten tatsächlich akzeptierten Command-/Resumeinstant; das interne Domain-LastMonotonicMs bleibt unangetastet. Export/Savezeit bleibt ausschließlich S/Domainautorität.

**Puzzlewechsel:** zuerst aktuellen tatsächlichen Versuch bei Bedarf Pausecommand/notifizieren und FlushAsync abwarten; nur bestätigter Flush erlaubt Start/Restore des nächsten Puzzles. Währenddessen Navigation gesperrt. Fehler behält alte Selection und aktuelle Session/Draftdirtywahrheit. Keine240-Sessionsammlung neben dem Save. Bei bestätigtem Wechsel werden UI-Selection und eventuelle alte Ridecallbacks zurückgesetzt; die eigene BoundSession wird explizit neu gebunden.

**Neuer Versuch:** gespeicherter Restart ist ein eigener Taskcommand. Vorherige ungesicherte Wahrheit wird zuerst serialisiert; dann ausdrücklicher Start derselben Identität mit neuem Attempt, Dirtymeldung für diesen neuen Versuch und unmittelbare Speicherung des neuen Readycheckpoints. Fehlgeschlagene Speicherung zeigt den neuen Versuch als ungesichert und behält die alte durable Payload; kein scheinbar erfolgreicher Reset auf einen später doch alten Draft. Während unbestätigter Completion oder Fahrt bleibt Restart gesperrt.

**CompletionPending/Saving:** PuzzleSolvedpath wird ausschließlich aus dem wirklichen Handle-Ergebnis übernommen. Bei restauriertem tatsächlich gelöstem Checkpoint kommt der Pfad aus erneuter Domain-Evaluation desselben vollständigen Zustands; kein Autoren-/Solverlookup. Eingaben, Picker, Restart und Ride sind gesperrt. Genau ein beobachteter AutosaveCoordinator.CommitCompletionAsync wartet ältere Writes ab. Nur `CanPresentResults` für die exakte aktuelle Attempt-/Puzzleidentität erlaubt Übergang **ProgressCommitted→TrainRide→Results**. Die Fahrt erhält ausschließlich diesen Spielerpfad; Animation löst keinen Grant aus.

**Failure/Unknown/Conflict:** benannter sichtbarer Speicherfehler, keine Fahrt/Result-/Rewarderfolg. Sicherer fehlgeschlagener Versuch darf explizit retryen; ungewisse Wirkung verlangt frischen Load. Conflict führt bereits in S zwingend zu neuem Load; die UI benutzt ausschließlich die neue RecoveryObservation, niemals Commitzeitpunkt oder gecachten Kandidaten. Fehlgeschlagener/cancelled/unvollständiger Load bleibt gesperrt.

**Recovery:** sichtbare drei Kandidaten mit Kind, Status, Generation und gekürzter Sourcehashkennung, keine Payloads/Geheimnisse. Vollständige gebundene Observation bleibt hinter der Applicationgrenze. Auswahl nur Valid, keine Unknown/Unsupported/Failedkandidaten übergehen. SaveRecoveryRequest→read-only RecoverAsync→exakte PromoteRecoveryAsync; stale/changed Kandidat führt erneut zu Conflict/Reload. Ein SplitBrain-ConservativeSnapshot macht keinen Spielcontrol aktiv. Bei Unsupported/Failed ist der Resetbutton gesperrt. Bei nachgewiesen korruptem bekannten Profil führt erst ein gesonderter sichtbarer Bestätigungsschritt zu ConfirmedResetCorruptProfileAsync; keine Ersatzdaten für Unknown.

**Nach bestätigtem Load/Promotion/Neuprofil:** S löst alte unbestätigte Session bewusst ab. Die Bridge verwirft alte Attempt-/Path-/Resultcallbacks, zeigt die tatsächlich geladene Fortschrittswahrheit und verlangt neue Start-/Fortsetzenentscheidung. Fremder Gewinner wird nicht als eigener Abschluss quittiert. Bereits bestätigte FirstClears erzeugen beim Laden keinen zweiten Reward und keine automatische Fahrt; die Animation ist keine persistierte Wahrheit.

**Ergebnis:** nur QA-Gespeichertkennzeichnung, tatsächliche gebundene Sterne und validierter QA-Saldo; keine produktiven Claims. Projektion aus dem bestätigten Snapshot wird einmal nach Änderung berechnet/veröffentlicht, nicht mit Ledgerhashing im Renderframe. Nullschwellen bleiben ausdrücklich null/Ein-Stern-Logik in S. Ein optionaler Rewarddelta wird nur aus dem exakt bestätigten eigenen Commit abgeleitet, nicht aus fremdem Load; die schlanke Variante zeigt nur QA-Saldo/Sterne/Fortschritt.

## 6. Lifecycle, Fahrt und Dispose

OnApplicationPause und OnApplicationFocus werden zu einem einzigen OR-Suspensionszustand zusammengeführt; doppelte Notifications sind wirkungslose No-ops. Vor Suspension echte Domainpause am aktuellen Monotoninstant, nur tatsächliche Änderung notifizieren, dann unmittelbar FlushAsync anfordern. Kein Warten auf0.5/2s. OnApplicationQuit ist best effort und kein behaupteter mobiler Dauerhaftigkeitspass. Zurückkehren nur bei tatsächlichem Fokus und nicht pausiert, keine versteckte Hintergrundzeit.

Eine bestätigte Fahrt stoppt ihren World-Tick während Suspension. Fahrtende ist mit der erfassten AttemptId gebunden; disposed/alte/neu gestartete Session darf kein spätes CompleteTrainRide erhalten. Savefehler hat nie eine DevelopmentTrainRide-Instanz. Destroy cancelt eigene Tasks/Dispatcherpublikationen und entsorgt vorhandene Presenter/Ride/ScreenCamera/UIDocument ausschließlich auf dem Unitythread. Ausstehender Workerwrite wird nicht als sicher rückgängig behauptet; nächster Start lädt vollständig neu.

## 7. Vorhandene Assemblykanten – keine Änderung erforderlich

| Modul | Vorhandene relevante direkte Kanten | Konsequenz |
|---|---|---|
| Application | Domain, Solver; noEngineReferences=true | Neue Bridge darf Domaincommands/Evaluator und internal DevelopmentEndpoint direkt verwenden; kein Content/Persistence/Unity/Filebezug. |
| Platform | Application | Clock/Dispatcher implementieren S-Applicationports, keine Persistencekante. |
| Bootstrap | Application, Content, Persistence, Platform, UI, World u.a. | Konkrete Composition dort erlaubt. **Keine direkte Domainkante**: Installer/Composition dürfen S.Session/CommandResult/Checkpoint-Domaintypen nicht dereferenzieren; nur neue App-Read-/Actiongrenze verwenden. |
| Presentation.UI | Application, Domain, Unity.Localization | Gespeicherte View/Actions aus App und vorhandenes Grid konsumieren; keine Content/Persistence/Filekante. |
| Presentation.World | Application, Domain | Bestehende DevelopmentTrainRide unverändert konsumieren. |
| Editor.Build | ausschließlich Bootstrap | Builder ruft Bootstrap-Validierung mit TextAssets auf; keine neue App-/Contentkante durch direkten Constructorzugriff. |
| Tests.Application.EditMode | ausschließlich Application | Domain/Platform/Content/Persistencezugriffe nur test-only Introspektion, keine asmdefänderung. |
| Tests.Presentation.PlayMode | UI, World | Appgrenze/Testdoubles nur vorhandene test-only Introspektion, falls direkte Typen nicht sichtbar. |
| Tests.Bootstrap.PlayMode | Bootstrap, Application, Content, Persistence, Platform, UI, World u.a.; keine Domain/Editor.Build | Echte Composition/FS direkt; Domain-/Builddetails test-only Introspektion. |

Alle asmdefs, csc.rsp, Service-Scaffolds, Produktloader und JCS-/Domain-/S-Fachquellen bleiben unverändert. I macht keine Runtime-Reflectionverdrahtung und keine Scene-/Resources-Suche zur Auflösung von Diensten.

## 8. Vorab konkrete I-Akzeptanztests

1. P/Anker/Sourcegate: exakt angenommene LA/LS und gemeinsame Basis, normale Montageprovenienz, neuer einziger Parent P; keine kumulative Kindscopebehauptung. Neuer eigener I-Selektor nach unverändertem Manifest.
2. Composition: echte A-Publicbytes (LF-normalisiert wie A-Vertrag), exakt240 und f647-Teilhash; S-Campaign/Wrapper vollständig validiert, Aggregate-/Profileinsatz IsolatedReview. Mutation jedes Teilhashes, Production/ContractTest/QA-Namespace/Roottraversal/Reparse und fehlende Quellen stoppen ohne Profilewrite.
3. Entry: tatsächlich verarbeiteter QA-BuildReport enthält SavedPublic240Review als erste normale Szene mit einem korrekt konfigurierten Savedinstaller; Public240Review/DevelopmentPuzzle/BootstrapComposition weiterhin als getrennte QAentries vorhanden. Android-/iOS-Identifier und qa/dev gegenüber staging/production fail-closed. iOS BuildOptions.None ist kein Produktprofil und bleibt als vorhandener Buildmodus getrennt. Enger DevelopmentPuzzleSceneBuildCheck-Guard ändert keine alten Artefakte.
4. Mainthread: kontrollierte Worker-Load/Commitantworten dürfen Unity/UI ausschließlich nach Queue-Drain verändern; Inline-Hauptthreadpublikation korrekt; Workerthread-Drain abweisen; Dispose beendet wartende Publikationen, keine alte Sceneaktion. Kanonische/FS-/Hasharbeit nachweislich außerhalb Renderthread.
5. Initialisierung: echte drei Missing zeigen NeedsInitialization und erzeugen kein Profil; erst reale Buttonaktion erzeugt bestätigte Generation1. Corrupt/Unsupported/Failed/SplitBrain werden sichtbar und niemals durch Initialisierung ersetzt.
6. 240-Picker: reale UI-Auswahl zweier Größen4×4/10×10; korrekte Randzahlen/Endpoints/alleTools/Batch/Undo/no-op/selection. Wechsel wartet echten Flush ab; Fail/Cancel hält ursprüngliche Auswahl und Session. Doppelklick/latecallback kann neuere Auswahl nicht überschreiben. Erhaltener alter PuzzleView mit seinem CaptureCommand/Apply/Undo/CompleteTrainRide erzeugt keine Commands/Anzeige für die neue Attempt oder neue Rastergröße.
7. Echter Savedroundtrip: UI-Marker→Leeren→Marker→Undo→Pause→Flush, Installer zerstören/neu aufbauen, exakt fortsetzen; Zellen, PreviouslySet, stickyFlags, Correction, Revision, Dedup und Undo erhalten; neue Monotonie ohne Downtimezuschlag. Keine neue Source-/Hash-/Undoautorität.
8. Autosave/Lifecycle:499/500/1999/2000ms, kontinuierliche Mutationen, frühere Saveantwort/neue Mutation, sofortiger Pause-/Fokusflush, OR-Suspension, Completion während Draftwrite und Navigation/Restart während Write; Render-Update führt selbst keine IO aus.
9. Commit→Ride→Results: tatsächliche Playercommands lösen; kontrolliertes RepositoryACK halten, keine Fahrt/Result-/Rewarderfolg und keine neuen Eingaben; nach verifiziertem Mainreread genau eigene Spielerstrecke; stale Attemptcallback und Appdispose stoppen. Native4×4-/10×10-/Ride-/Resultgrafik selbst betrachten und originale Screens binden.
10. Failure/Crash: Lowdisk/Flush/Replace-/Cancel-/Conflict aus echtem FS; gelöste Session bleibt, keine Fahrt; sicherer Retry zahlt einmal. Write dauerhaft/vor Callback verloren→neuer Load bindet Gewinner, kein Doppelgrant und keine alte Attemptfreigabe.
11. Recovery: drei reale Kandidaten einschließlich gleicherGeneration/andererHash; visibleSplitBrain, Candidatewahl read-only vorPromotion; Änderung auch nichtselektierten Kandidaten, Envelope-Byteänderung, neuererMain/entstandenerPending sperren. Nach expliziter Promotion nur exakt ausgewähltes Payload, keine versteckte Compaction/Rewards. Unknown/Failed kann weder recovernd umgangen noch resettiert werden.
12. Reset: sichtbare getrennte Bestätigung, keine Dateiwirkung beim bloßen Anzeigen/Abbrechen; confirmedCorruptReset bewahrt Originale in Hashquarantäne und nutzt höhereGeneration; danach neutral neue Session. Callback alter Session darf keine Results freigeben.
13. Restart: bestätigtes neues Attempt/Readycheckpoint, Time0 bis ersterInput, alte Commandcallbacks abweisen; FailedRestartWrite hält neue ungesicherte Session und alte durable Payload mit sichtbarem Status.
14. Grundfortschritt: ein echter FirstClear/Nullzeiten genau1Stern/15QA-Geduld; Markerstraffrei; Route-/Sectionterminals aus S, keine Sterne als Gates; vorhandene bestätigte Fortschrittsterminals nach Restart/Restore ohne Doppelzahlung. Produktionsfähige weitergehende Policyfälle bleiben nachgewiesener S-Bestand und werden nicht allgemein neu geprüft.
15. UI/Locale: Savedstatusänderung aktualisiert ohne Domainrevisionwechsel; alle neuen Keys/Argumenttypen korrekt, keine sichtbaren rohenKeys/abgeschnittenenRecoverycontrols, klare QA-Grenze, nur bestätigter QA-Saldo. Keine Autorenlösung/Proof/Notiz/Fixture im Playerdata-/Scenesdependencygraph.
16. Finale reale Nachweise: lizenzfreier Targetcompile als Supplement, kanonische17/18-Self-/Negativchecks, vollständige reguläre I-CI am echten Freeze inkl Build-/XML-/Log-/Source-/Receiptbindung, unabhängige Abschlussreview. Danach R-Entscheidung und frisch erwartungsgebundene zulässige Unterintegration mit integrierterCI/getrennterKontrolle; kein Main-/Produkt-/Human240-/physischerMobil-PASS.

## 9. Vor Code durch R eng zu bestätigen

R hat neue Savedscene statt A-Konvertierung, ProfileId r, EditorbuildId editor-public240-review sowie Dateieigentümer bereits als prospektive Ausführung festgelegt. Die konkrete I-Verankerung/GUIDs und endgültigen Bridge-/Statussignaturen folgen erst nach P. Root besitzt neuen Savedinstaller+Composition, Buildentry/engen Developmentcheck/EditorBuildSettings und SavedPublic240BuildEntryTests; App-Owner Bridge+LocalSaveRuntime+Applicationtests; A-/UI-Owner SavedPresenter/Builder/Assets/Scene+Presentationtests und SavedPublic240ReviewSmokeTests.

R hat für Tests konkretisiert: SavedInstaller.UseTestPrivateRoot gilt ausschließlich unter UNITY_EDITOR && UNITY_INCLUDE_TESTS, QA-only und in einem einzelnen Scope; absoluter Root unter OS-Temp oder STP_TEST_TEMP_ROOT, bestehend/nicht-Reparse durch echte FS-Prüfung. Vor Awake in einem inaktiven Testaufbau erfassen, Scope erst nach vollständigem InstancetearDown entsorgen. Runtime verwendet immer den tatsächlichen persistentDataPath/ProfileId r. Kein allgemeiner statischer Testlocator und kein Productionroot-Fallback. Alle Szenenreferenzen bleiben serialisiert/typisiert. Die vorhandenen Vertrags- und Assemblygrenzen erfordern keinen erkennbaren neuen ADR oder Edgewechsel. Auftretende echte Abweichung wird vor Fachcode gesondert dokumentiert. Dieser Plan setzt keinen bereits bestandenen P-/I-/Native-/Produktnachweis voraus.
