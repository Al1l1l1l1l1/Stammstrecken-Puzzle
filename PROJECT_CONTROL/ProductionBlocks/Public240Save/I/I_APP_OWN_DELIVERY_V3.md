# I App-/Platform-Lieferanteil V3

Implementiererbericht, 19.10.2026. WP043/Manifest wurden gemeinsam in `1c002e14e07a7cf48366e786f2ec598234f9d314` direkt auf P verankert. ADR036 wurde nach getrennter Review vor Fachcode durch R angenommen und in `3ca62a9b109f828fbb32965eace4fbf8fe3eae7e` gebunden. Dieser Bericht ist kein unabhängiger I-PASS, kein Unity-/Dateisystem-/CI-/Gesamtabschluss.

## Eigener stabiler Sourceumfang

Genau die drei neuen Quellen und ihre zugewiesenen Metas; vollständige native Bytebindungen im [begleitenden JSON](I_APP_OWN_DELIVERY_V3.json):

| Quelle unter Assets/StammstreckenPuzzle | SHA256 |
|---|---|
| Scripts/Application/SavedPublic240ReviewSession.cs | `9d1023cef6a45e5bc7a5c24c366f9be576264d9315e7242c126ba400541e284a` |
| Scripts/Platform/LocalSaveRuntime.cs | `568472f0fbeac372d537639157ab4730e3375217be2d52c3c7fa48c10dec09c8` |
| Tests/EditMode/Application/SavedPublic240ReviewSessionTests.cs | `c42048a5e03e4f6dc9db985e68c67931a4e310de17f90891ab6aa3f1c336ae5a` |

Keine bestehende A-/S-Fachdatei, kein Assemblyrand, Manifest oder Workflow durch diesen Implementierer geändert. Quellen bleiben bis zu einem konkreten neuen Befund unverändert. Status-/WP-/Gesamtabschluss führt Root.

## Tatsächliche Anschluss-API

`STP.Application.SavedPublic240ReviewSession(PersistedPuzzleSession, AutosaveCoordinator, IPublicPuzzleCatalog, ILocalSessionClock, ILocalSaveDispatcher) : IDisposable` wird auf Unitymain konstruiert. Sie bietet `PuzzleView : IReviewPuzzleSession?`, `Puzzles`, den immutable `State` und `HasDraftForPuzzle(int)`. State.SelectedIndex ist vor ausdrücklich gestarteter Session -1. Progress.SelectedIdentity ist dann null. Die read-only Draftprojektion prüft das ganze Identitytripel im bestätigten Snapshot; Dropdownauswahl hat keine Appwirkung.

Beobachtete Taskmethoden mit CancellationToken: InitializeAsync, CreateProfileAsync, SelectPuzzleAsync(index, restoreExistingDraft), RestartAsync, RetrySaveAsync, RetryCompletionAsync, ReloadAsync, RecoverAndPromoteAsync(kind), ConfirmedResetAsync, TickAutosaveAsync, SetSuspendedAsync(paused). Der hostseitige OR-Suspensionswert bleibt Roots Verantwortung. SetSuspendedAsync erfasst den tatsächlichen Pause-/Resumeinstant synchron auf Main, auch bei noch laufendem Write oder Load, sofern S den bekannten Versuch tatsächlich mutieren lässt. Nur der nachfolgende Flush wartet beobachtet.

Synchrone Viewcommands erzeugen keine unbeobachtete Completiontask. Ein tatsächliches Solved markiert CompletionPending; der beobachtete Tick übernimmt einmal. Fehler stoppen automatische Retries. `BeginTrainRide(Guid expectedAttempt)` verlangt genau den aktuellen bestätigten Attempt. Nur dessen gebundene View darf CompleteTrainRide ausführen. Nach Navigation/Restart werden neue Views veröffentlicht; alte Views behalten ihre eigene Definition und können keine neue Attempt erfassen. Der synchrone View-Restart lehnt mit SAVE_REVIEW_ASYNC_RESTART_REQUIRED ab.

SavedReviewState besitzt Version, Status, LoadStatus, CommitStatus, ErrorCode, alle vereinbarten Can-Gates, IsDirty, SelectedIndex, CurrentIdentity, HasDraftForSelection, nullable Progress und defensive RecoveryChoices. SavedReviewProgress bindet Profile/Catalog/Generation, QA-Balance, FirstClearCount/MemberCount sowie nullable ausgewählte Identität/Sterne exakt an den bestätigten Snapshot. Bei unklarer Autorität bleibt Progress null. Root/UI dürfen Taskcompletion nicht als Saveerfolg verwenden.

`STP.Platform.LocalSaveClock : ILocalSessionClock` bietet Prozess-Stopwatch und invariante UTC-Diagnose. `MainThreadSaveDispatcher : ILocalSaveDispatcher, IDisposable` erfasst den Konstruktionsthread, publiziert dort inline, queued Workerpublikationen als beobachtete Tasks und bietet `Drain()` ausschließlich auf diesem Owner. Dispose beantwortet wartende/future Publikationen kontrolliert ohne deren Actions auszuführen. Root muss den Dispatcher auch während null Modell/Startup pumpen und die Bridge nach der Workercomposition auf Main konstruieren.

Nach vollständig bestätigtem serialisiertem Load/Create/Reset/Recoverypromotion wird genau die aktive Autosavekoordination über dieselbe S-Session neu gebunden. Das folgt [Rs enger Vorkorrekturentscheidung](I_AUTOSAVE_REBIND_CORRECTION_DECISION.md); keine alte offene Schreibarbeit oder zweite Persistenzautorität wird weitergeführt.

## Tatsächlicher eigener Supplementnachweis

Root führte den frischen lizenzfreien Compile05 aus: Domain, Solver, Application, Content, Persistence, Platform und TestApp jeweils Exit0. Tatsächliches Mono-NUnit 13:32:38–45 UTC: **28/28 PASS, 0 FAIL/SKIP**, 6.994639 Sekunden. Roh-XML, sämtliche sieben DLL/PDB/RSP/Outputs und 73 Quellbindungen sind verlustfrei in [I_APP_V3_ORIGINAL_COMPILER_TEST_PACK.json](I_APP_V3_ORIGINAL_COMPILER_TEST_PACK.json), SHA256 `bb8316ba52735e3bae12f00e686385d8cd59d28c5949b82e72b7a80e13d9d980`, gebunden. JSON dieses Berichts enthält jeden tatsächlichen Testnamen und Status.

| WP-AK-Anteil | Eigener konkret ausgeführter Nachweis / verbleibende Grenze |
|---|---|
| I05 | Ownerthread-Bridge, echte Platformqueue/Drain-/Dispose-/Clockprobe per Testintrospektion, Workerentry abgewiesen. Native Startup-/Unitygetter-/Renderprobe gehört Root/UI. |
| I06 | Missing ohne Create/Fakeattempt; ausdrückliche Generation1. Physische drei Missing und realer Root noch Integration. |
| I07/I08 | Expliziter Neu-/Resumeweg, kein fehlender-Draft-Fallback, Flush-vor-Wechsel, stale Views über verschiedene Dimensionen, tatsächliche S-/Domain-Checkpoint-/Undo-/Sticky-/Zeitwerte. Eigener Testkatalog ist 2x2/3x2/10x10; echte 240er-UI und nativer 4x4/10x10-Roundtrip bleiben gemeinsame Nachweise. |
| I09 | No-op ohne Dirty, 499/500ms, neue Mutation während gehaltenem Write, Pauseinstant während Write/Load, beobachtete Lifecycle-Cancellation und phasentreue Pause/Resume der Fahrt/Results. S-eigene 2s-Verträge bleiben unverändert. |
| I10/I13 | Reale Domain-Spielercommands lösen; kontrolliert gehaltenes Repository-ACK sperrt Fahrt, bestätigter eigener Commit öffnet genau gebundene Fahrt/Result. Nullzeiten 1 Stern/15 QA-Punkte, keine Doppelzahlung bei neuer Übungsfahrt; expliziter neuer Readycheckpoint und gescheiterter neuer Readywrite. Reale OSlock-/File-/World-/Screenprobe noch Integration. |
| I11/I12 | Safe failure ohne Frame-Retry, Unknown nur nach fresh Load, Conflict mit frischer Observation und expliziter Promotion, Corrupt-Reset nur ausdrücklicher Request, Unsupported/Failed geschützt, Fault mit erreichbarem explizitem Reload; Dispose/late ACK wirkungslos. Diese Fixture nutzt kontrollierte Repositoryports; keine tatsächliche Dateiwirkung/CAS-/Quarantänebehauptung daraus. |
| I14/I15 | Immutable Anzeige und XMLdocs, keine Test-/File-/Unityabhängigkeit in App, unveränderte Kanten. UI/Locale/Playerdependencygraph, unabhängige Review, native Unity und reguläre CI bleiben offen. |

## Erhaltene rote Schritte und nächster Schritt

V1 19/21 und V2 27/28 bleiben unverändert in ihren eigenen Original-/Compilerpacks erhalten. V1 zeigte die alte Dirtykoordination nach bestätigter Sessionablösung sowie die zu frühe Failureinjektion des Tests. V2 zeigte ausschließlich NUnit-exakten Throws versus gültiger TaskCanceledException-Unterklasse; V3 änderte nur diesen Assert zu Catch. Keine erwarteten Runtimeergebnisse wurden abgesenkt.

Nächster Schritt: Root/UI schließen die gemeinsame native Verdrahtung, echten Dateiflow, Grafik, Build-/Scope-/CI- und unabhängigen I-Nachweise ab. Dieser Lieferanteil nimmt keinen solchen PASS und keine Produkt-/Main-/Release-/Human-/Geräteannahme vorweg.
