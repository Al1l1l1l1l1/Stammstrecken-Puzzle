using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.Platform;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Bootstrap
{
    /// <summary>Separate assigned Human observation composition over the actual production presentation.</summary>
    [DisallowMultipleComponent]
    public sealed class HumanCalibrationInstaller : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset? tree;
        [SerializeField] private StyleSheet? styles;
        [SerializeField] private TextAsset? locale;
        [SerializeField] private PanelSettings? panel;
        [SerializeField] private TextAsset? publicData;
        [SerializeField] private string publicCatalogHashSha256 = string.Empty;
        [SerializeField] private TextAsset[] assignmentPacks = Array.Empty<TextAsset>();
        [SerializeField] private TextAsset? rawAssignmentLock;
        [SerializeField] private TextAsset? uiSourceLock;
        [SerializeField] private TextAsset? railwayAssets;
        [SerializeField] private TextAsset? campaignText;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private LocalSaveClock? clock;
        private MainThreadSaveDispatcher? dispatcher;
        private HumanManifestRegistry? manifests;
        private MobileCandidateData? currentPublic;
        private IHumanRoutingRepository? router;
        private IHumanCalibrationRepository? repository;
        private HumanRoutingInspection? inspection;
        private string authorityKey = string.Empty;
        private ProductionHumanOperatorPanel? operatorPanel;
        private AssignedHumanSession? session;
        private ProductionCampaignText? texts;
        private ProductionGamePresenter? presenter;
        private HumanLocalExportAdapter? exporter;
        private ProductionTrainRide? ride;
        private Guid rideAttempt;
        private Task tick = Task.CompletedTask;
        private Task routingTask = Task.CompletedTask;
        private bool started, disposed, paused, operatorVisible = true, routingBusy, routingUnknown, focused = true;
        private Camera? output;
        private Rect lastSafeArea;
        private int lastWidth, lastHeight;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        private string? testRoot;
#endif
        public AssignedHumanSession? Session => session;
        public ProductionHumanOperatorPanel? OperatorPanel => operatorPanel;
        public HumanRoutingInspection? RoutingInspection => inspection;
        public bool IsOperatorVisible => operatorVisible;
        public ProductionGamePresenter Presenter => presenter ?? throw new InvalidOperationException("HUMAN_UI_NOT_READY");
        public UIDocument Document { get; private set; } = null!;
        public Task InitializeTask { get; private set; } = Task.CompletedTask;
        public Task PendingOperation => Task.WhenAll(InitializeTask, tick, routingTask,
            operatorPanel?.PendingOperation ?? Task.CompletedTask, presenter?.PendingRequest ?? Task.CompletedTask);
        public Task ShutdownTask { get; private set; } = Task.CompletedTask;
        public string StartupDiagnostic { get; private set; } = "NONE";

        /// <summary>Scene construction only; no runtime assignment or product-profile selector.</summary>
        public void Configure(VisualTreeAsset view, StyleSheet sheet, TextAsset translations, PanelSettings settings,
            TextAsset puzzles, string publicHash, TextAsset[] packs, TextAsset rawLock, TextAsset sourceLock,
            TextAsset assets, TextAsset playerTexts)
        {
            if (started || disposed) throw new InvalidOperationException("HUMAN_CONFIG_WINDOW_CLOSED");
            tree=view; styles=sheet; locale=translations; panel=settings; publicData=puzzles; publicCatalogHashSha256=publicHash;
            assignmentPacks=packs==null?throw new ArgumentNullException(nameof(packs)):(TextAsset[])packs.Clone();
            rawAssignmentLock=rawLock; uiSourceLock=sourceLock; railwayAssets=assets; campaignText=playerTexts;
            ValidateReferences();
        }

        public void ValidateReferences()
        {
            if (tree==null || styles==null || locale==null || panel==null || publicData==null || rawAssignmentLock==null ||
                uiSourceLock==null || railwayAssets==null || campaignText==null) throw new ArgumentException("HUMAN_SCENE_REFERENCE_MISSING");
            var json=new CanonicalJsonAdapter();
            var current=new MobileCandidateData(publicData.bytes,publicCatalogHashSha256);
            _=new HumanManifestRegistry(assignmentPacks,rawAssignmentLock,uiSourceLock,current,json);
        }
        /// <summary>Build preflight stays behind the Bootstrap boundary and cannot replace or approve a source.</summary>
        public static void ValidateRoutingAssets(TextAsset[] packs,TextAsset rawLock,TextAsset sourceLock,TextAsset puzzles,string publicHash)
        {_=new HumanManifestRegistry(packs,rawLock,sourceLock,puzzles,publicHash);}
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        /// <summary>Actual integration probes use a separate physically checked temporary root.</summary>
        public void ConfigureTestRoot(string root)
        {
            if(started)throw new InvalidOperationException("HUMAN_CONFIG_WINDOW_CLOSED");
            var real=UnityEngine.Application.persistentDataPath;
            testRoot=Task.Run(()=>SavedPublic240ReviewInstaller.ValidateTestRootBoundary(root,real,Path.GetTempPath(),
                Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT"))).GetAwaiter().GetResult();
        }
#endif
        private void Awake()
        {
            started=true;
            clock=new LocalSaveClock(); dispatcher=new MainThreadSaveDispatcher();
            Document=gameObject.AddComponent<UIDocument>(); Document.panelSettings=panel;
            try
            {
                ValidateReferences();
                tree!.CloneTree(Document.rootVisualElement); Document.rootVisualElement.styleSheets.Add(styles!);
                var screen=new GameObject("Assigned puzzle screen"); screen.transform.SetParent(transform,false);
                output=screen.AddComponent<Camera>(); output.cullingMask=0; output.clearFlags=CameraClearFlags.SolidColor;
                output.backgroundColor=new Color(.96f,.92f,.81f); output.depth=100;
                output.enabled=SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null;
                CreateOperatorPanel();
                InitializeTask=InitializeAsync();
            }
            catch(Exception error){Fail(error);}
        }
        private async Task InitializeAsync()
        {
            try
            {
                if(!UnityEngine.Application.isEditor&&UnityEngine.Application.identifier!=HumanAssignmentCatalog.ApplicationId)
                    throw new ArgumentException("HUMAN_APPLICATION_ID_REQUIRED");
                var json=new CanonicalJsonAdapter();
                currentPublic=new MobileCandidateData(publicData!.bytes,publicCatalogHashSha256);
                manifests=new HumanManifestRegistry(assignmentPacks,rawAssignmentLock!,uiSourceLock!,currentPublic,json);
                string root=UnityEngine.Application.persistentDataPath;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                if(testRoot!=null)root=testRoot;
#endif
                router=new HumanCalibrationStore(root,manifests.Resolve(manifests.ManifestBytesSha256[0]).Catalog,manifests,json);
                texts=new ProductionCampaignText(campaignText!,true);
                exporter=new HumanLocalExportAdapter(HumanAssignmentCatalog.ApplicationId);
                SetInspection(await router.InspectAsync(lifetime.Token));
                if(!disposed)operatorPanel!.Refresh();
            }
            catch(OperationCanceledException){if(!disposed)Fail(new IOException("HUMAN_START_CANCELLED"));}
            catch(Exception error){if(!disposed)Fail(error);}
        }
        private HumanRunOrigin Origin => UnityEngine.Application.isEditor?HumanRunOrigin.AUTOMATED_TEST:HumanRunOrigin.PARTICIPANT_UI;
        private void SetInspection(HumanRoutingInspection value)
        {if(disposed)return;inspection=value;authorityKey=Guid.NewGuid().ToString("N");routingUnknown=false;}
        private HumanOperatorView ReadOperator()
        {
            if(inspection==null)return new HumanOperatorView(routingUnknown?HumanOperatorPhase.Recovery:HumanOperatorPhase.Loading,string.Empty,busy:routingBusy);
            if(inspection.Status==HumanStoreStatus.Missing)return new HumanOperatorView(HumanOperatorPhase.Missing,authorityKey,busy:routingBusy);
            if(inspection.IsConfirmed)
            {
                bool complete=inspection.Snapshot!.Run.NextAssignmentIndex==inspection.Catalog!.Assignments.Count;
                return new HumanOperatorView(complete?HumanOperatorPhase.Complete:HumanOperatorPhase.Active,authorityKey,
                    manifests!.PathForHash(inspection.Catalog.ManifestBytesSha256),routingBusy,true);
            }
            return new HumanOperatorView(inspection.Status==HumanStoreStatus.Unsupported?HumanOperatorPhase.Unsupported:HumanOperatorPhase.Recovery,
                string.Empty,busy:routingBusy);
        }
        private void CreateOperatorPanel()
        {
            operatorVisible=true;
            var options=HumanManifestRegistry.ExpectedPackPaths().Select(p=>new HumanOperatorPack(p,6)).ToArray();
            operatorPanel=new ProductionHumanOperatorPanel(Document.rootVisualElement,locale!,options,ReadOperator,
                (pack,key)=>TrackRouting(()=>StartParticipantAsync(pack,key)),()=>TrackRouting(ResumeParticipantAsync),
                ()=>TrackRouting(RetryRoutingAsync),()=>TrackRouting(ExportOperatorAsync));
            ApplySuspension();
        }
        private Task TrackRouting(Func<Task> operation)
        {
            if(disposed||routingBusy||paused||!focused||!operatorVisible)throw new InvalidOperationException("HUMAN_ROUTING_INTENT_BLOCKED");
            routingTask=RunRoutingAsync(operation);return routingTask;
        }
        private async Task RunRoutingAsync(Func<Task> operation)
        {
            routingBusy=true;
            try{await operation();}
            catch
            {
                if(!disposed)
                {
                    inspection=null;authorityKey=string.Empty;routingUnknown=true;
                    session?.SetSuspension(HumanPauseReason.Presentation,true);presenter?.SetAccessibilitySuspended(true);
                    if(operatorPanel==null)CreateOperatorPanel();
                }
                throw;
            }
            finally{routingBusy=false;if(!disposed)operatorPanel?.Refresh();}
        }
        private async Task StartParticipantAsync(string pack,string key)
        {
            if(inspection?.Authority==null||key!=authorityKey||!inspection.CanStartParticipant)
                throw new InvalidOperationException("HUMAN_ROUTING_AUTHORITY_STALE");
            var result=await router!.StartNextParticipantAsync(inspection.Authority,manifests!.HashForPath(pack),Origin,clock!.UtcNow,lifetime.Token);
            SetInspection(result);
            if(!result.IsConfirmed)throw new IOException(result.ErrorCode);
            await MountConfirmedAsync(result);
        }
        private async Task ResumeParticipantAsync()
        {
            var expected=inspection;
            var fresh=await router!.InspectAsync(lifetime.Token);SetInspection(fresh);
            if(expected?.Authority==null||fresh.Authority==null||!fresh.IsConfirmed||!expected.Authority.Matches(fresh.Authority))
                throw new IOException("HUMAN_ROUTING_RESUME_CHANGED");
            await MountConfirmedAsync(fresh);
        }
        private async Task RetryRoutingAsync()
        {
            var prior=inspection;
            // Explicit known-source preservation only. Unknown outcomes first require a fresh read-only inspection.
            var result=prior!=null&&prior.Status==HumanStoreStatus.RecoveryRequired
                ?await router!.RecoverRoutingAsync(prior.GlobalObservationToken,lifetime.Token)
                :await router!.InspectAsync(lifetime.Token);
            SetInspection(result);
        }
        private async Task MountConfirmedAsync(HumanRoutingInspection confirmed)
        {
            if(disposed)return;
            if(!confirmed.IsConfirmed||confirmed.Authority==null||confirmed.Catalog==null)throw new InvalidOperationException("HUMAN_ROUTING_ACK_REQUIRED");
            var fixedRepository=router!.OpenConfirmedRepository(confirmed.Authority);
            var next=new AssignedHumanSession(confirmed.Catalog,fixedRepository,clock!,dispatcher!,Origin);
            next.SetSuspension(HumanPauseReason.Presentation,true);
            try
            {
                await next.InitializeAsync(lifetime.Token);
                if(disposed){next.Dispose();return;}
                var fresh=await router.InspectAsync(lifetime.Token);
                if(disposed){next.Dispose();return;}
                SetInspection(fresh);
                if(!fresh.IsConfirmed||fresh.Authority?.SessionToken!=confirmed.Authority.SessionToken||
                    fresh.Catalog!.ManifestBytesSha256!=confirmed.Catalog.ManifestBytesSha256||
                    next.State.ObservationStatus==HumanObservationStatus.Unsupported||next.State.ObservationStatus==HumanObservationStatus.RecoveryRequired)
                    throw new IOException("HUMAN_ROUTING_LOADED_BINDING_UNCONFIRMED");
                operatorPanel?.Dispose();operatorPanel=null;
                DisposeRide();presenter?.Dispose();presenter=null;session?.Dispose();
                session=next;repository=fixedRepository;
                // A fresh tree prevents retained callbacks/glyphs from the prior participant crossing into the new binder.
                Document.rootVisualElement.Clear();tree!.CloneTree(Document.rootVisualElement);
                presenter=new ProductionGamePresenter(Document.rootVisualElement,locale!,()=>clock!.MonotonicMilliseconds);
                presenter.BindAssetCatalog(railwayAssets!,true);
                presenter.SetAccessibilitySuspended(true);
                presenter.BindAssignedReview(next,assignment=>ResolveDisplay(assignment,currentPublic!),ExportAsync);
                presenter.BindAssignedOperator(OpenOperator);
                operatorVisible=false;next.SetSuspension(HumanPauseReason.Presentation,false);ApplySuspension();presenter.Refresh();
            }
            catch{if(!ReferenceEquals(session,next))next.Dispose();throw;}
        }
        private void OpenOperator()
        {
            if(disposed||operatorVisible||routingBusy||session?.State.IsFinished!=true)return;
            session.SetSuspension(HumanPauseReason.Presentation,true);presenter!.SetAccessibilitySuspended(true);
            inspection=null;authorityKey=string.Empty;CreateOperatorPanel();
            routingTask=RunRoutingAsync(async()=>{await tick;SetInspection(await router!.InspectAsync(lifetime.Token));});
        }
        private async Task ExportOperatorAsync()
        {
            var expected=inspection;
            var fresh=await router!.InspectAsync(lifetime.Token);SetInspection(fresh);
            if(disposed)return;
            if(expected?.Authority==null||fresh.Authority==null||!fresh.IsConfirmed||!expected.Authority.Matches(fresh.Authority))
                throw new IOException("HUMAN_ROUTING_EXPORT_CHANGED");
            await ExportRepositoryAsync(router.OpenConfirmedRepository(fresh.Authority));
        }
        private ProductionPuzzleDisplay ResolveDisplay(HumanAssignment assignment,MobileCandidateData current)
        {
            if(assignment.Purpose==HumanAssignmentPurpose.CAMPAIGN_TIME)
            {
                int index=Enumerable.Range(0,current.Puzzles.Count).Single(i=>current.Puzzles[i].Identity.Equals(assignment.Puzzle.Identity));
                return texts!.Resolve(index,string.Empty,string.Empty);
            }
            return new ProductionPuzzleDisplay(string.Empty,string.Empty,"Neue Gleise, neuer Fahrplan",
                "Verbinde die beiden Anschlüsse. Die Zahlen zeigen die belegten Felder.","Dein Zug ist angekommen.");
        }
        private async Task ExportAsync()
        {
            if(session==null||repository==null||exporter==null||!Presenter.IsAssignedInstructionOpen)
                throw new InvalidOperationException("HUMAN_EXPORT_REQUIRES_PAUSED_MENU");
            await ExportRepositoryAsync(repository);
        }
        private async Task ExportRepositoryAsync(IHumanCalibrationRepository source)
        {
            if(exporter==null)throw new InvalidOperationException("HUMAN_EXPORT_NOT_READY");
            byte[] bytes=await source.ExportAsync(lifetime.Token);
            string name="stp-human-"+LocalSaveHash.Bytes(bytes)+".json";
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            if(testRoot!=null)
            {
                string directory=Path.Combine(testRoot,"human-exports");Directory.CreateDirectory(directory);
                await Task.Run(()=>File.WriteAllBytes(Path.Combine(directory,name),bytes),lifetime.Token);
                return;
            }
#endif
            string file=exporter.PrepareFilePath(name);
            await Task.Run(()=>File.WriteAllBytes(file,bytes),lifetime.Token);
            await exporter.ShareAsync(file,"Puzzlebeobachtungen exportieren",lifetime.Token);
        }
        private void Update()
        {
            if(disposed)return;
            dispatcher?.Drain(); ApplySafeArea(); ApplySuspension();
            if(operatorVisible){operatorPanel?.Refresh();return;}
            presenter?.Refresh();
            if(session==null||!InitializeTask.IsCompleted||StartupDiagnostic!="NONE")return;
            if(tick.IsCompleted)tick=ObserveTickAsync();
            UpdateRide(); presenter?.Refresh();
        }
        private async Task ObserveTickAsync()
        {
            try{await session!.TickAsync(lifetime.Token);}
            catch(OperationCanceledException){if(!disposed)presenter?.ShowRequestFailure(true);}
            catch(Exception){if(!disposed)presenter?.ShowRequestFailure(false);}
        }
        private void UpdateRide()
        {
            var view=session!.PuzzleView;
            if(ride==null&&session.State.CanStartRide&&view!=null&&!paused&&focused&&!operatorVisible)
            {
                var attempt=view.AttemptId;
                if(session.BeginTrainRide(attempt)==DevelopmentCommandStatus.Applied)
                {ride=new ProductionTrainRide(view.BuiltPath,view.EndpointA,view.EndpointB,view.Width,view.Height);rideAttempt=attempt;}
            }
            if(ride==null)return;
            if(view==null||view.AttemptId!=rideAttempt||view.Phase!=DevelopmentPreviewPhase.TrainRide){DisposeRide();return;}
            if(!paused&&focused&&!operatorVisible)ride.Tick(Time.unscaledDeltaTime);
            presenter!.SetTrainPose(ride.X,ride.Y,ride.AngleDegrees,true);
            if(ride.IsFinished&&session.CompleteTrainRide(rideAttempt)==DevelopmentCommandStatus.Applied)DisposeRide();
        }
        private void DisposeRide(){ride?.Dispose();ride=null;rideAttempt=Guid.Empty;presenter?.SetTrainPose(0,0,0,false);}
        public void SetPaused(bool value){paused=value;ApplySuspension();}
        public void SetFocused(bool value){focused=value;ApplySuspension();}
        private void OnApplicationPause(bool value)=>SetPaused(value);
        private void OnApplicationFocus(bool value)=>SetFocused(value);
        private void OnApplicationQuit()=>SetPaused(true);
        private void ApplySuspension()
        {
            presenter?.SetAccessibilitySuspended(paused||!focused||operatorVisible);
            operatorPanel?.SetExternalSuspended(paused||!focused);
        }
        private void ApplySafeArea()
        {
            if(Document==null||Screen.width<=0||Screen.height<=0)return;
            var safe=Screen.safeArea;if(safe==lastSafeArea&&lastWidth==Screen.width&&lastHeight==Screen.height)return;
            var root=Document.rootVisualElement;float scale=root.resolvedStyle.width/Screen.width;
            if(!float.IsFinite(scale)||scale<=0)return;
            lastSafeArea=safe;lastWidth=Screen.width;lastHeight=Screen.height;
            root.style.paddingLeft=safe.xMin*scale;root.style.paddingRight=(Screen.width-safe.xMax)*scale;
            root.style.paddingTop=(Screen.height-safe.yMax)*scale;root.style.paddingBottom=safe.yMin*scale;
        }
        private void Fail(Exception error)
        {
            StartupDiagnostic=error.GetType().Name+":"+error.Message;
            if(presenter!=null)presenter.ShowStartupFailure(error is IOException?SavedStartupFailure.LocalStorageUnavailable:SavedStartupFailure.InvalidAssets);
            else if(Document!=null)Document.rootVisualElement.Add(new Label("Die zugewiesene Prüffassung konnte nicht geladen werden. Bitte den Testleiter informieren."));
            Debug.LogError("Human calibration startup failed: "+StartupDiagnostic);
        }
        private void OnDestroy()
        {
            if(disposed)return;disposed=true;lifetime.Cancel();DisposeRide();operatorPanel?.Dispose();presenter?.Dispose();session?.Dispose();texts?.Dispose();dispatcher?.Dispose();
            if(output!=null)Destroy(output.gameObject);
            ShutdownTask=FinishShutdownAsync();
        }
        private async Task FinishShutdownAsync(){try{await Task.WhenAll(InitializeTask,tick,routingTask);}catch(Exception){}finally{lifetime.Dispose();}}
    }
}
