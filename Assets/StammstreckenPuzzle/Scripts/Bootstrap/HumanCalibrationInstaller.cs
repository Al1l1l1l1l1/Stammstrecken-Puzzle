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
        [SerializeField] private TextAsset? assignmentData;
        [SerializeField] private string assignmentBytesSha256 = string.Empty;
        [SerializeField] private TextAsset? uiSourceLock;
        [SerializeField] private TextAsset? railwayAssets;
        [SerializeField] private TextAsset? campaignText;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private LocalSaveClock? clock;
        private MainThreadSaveDispatcher? dispatcher;
        private HumanAssignmentCatalog? catalog;
        private HumanCalibrationStore? repository;
        private AssignedHumanSession? session;
        private ProductionCampaignText? texts;
        private ProductionGamePresenter? presenter;
        private HumanLocalExportAdapter? exporter;
        private ProductionTrainRide? ride;
        private Guid rideAttempt;
        private Task tick = Task.CompletedTask;
        private bool started, disposed, paused, focused = true;
        private Camera? output;
        private Rect lastSafeArea;
        private int lastWidth, lastHeight;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        private string? testRoot;
#endif
        public AssignedHumanSession? Session => session;
        public ProductionGamePresenter Presenter => presenter ?? throw new InvalidOperationException("HUMAN_UI_NOT_READY");
        public UIDocument Document { get; private set; } = null!;
        public Task InitializeTask { get; private set; } = Task.CompletedTask;
        public Task PendingOperation => Task.WhenAll(InitializeTask, tick, presenter?.PendingRequest ?? Task.CompletedTask);
        public Task ShutdownTask { get; private set; } = Task.CompletedTask;
        public string StartupDiagnostic { get; private set; } = "NONE";

        /// <summary>Scene construction only; no runtime assignment or product-profile selector.</summary>
        public void Configure(VisualTreeAsset view, StyleSheet sheet, TextAsset translations, PanelSettings settings,
            TextAsset puzzles, string publicHash, TextAsset assignments, string assignmentHash, TextAsset sourceLock,
            TextAsset assets, TextAsset playerTexts)
        {
            if (started || disposed) throw new InvalidOperationException("HUMAN_CONFIG_WINDOW_CLOSED");
            tree=view; styles=sheet; locale=translations; panel=settings; publicData=puzzles; publicCatalogHashSha256=publicHash;
            assignmentData=assignments; assignmentBytesSha256=assignmentHash; uiSourceLock=sourceLock; railwayAssets=assets; campaignText=playerTexts;
            ValidateReferences();
        }

        public void ValidateReferences()
        {
            if (tree==null || styles==null || locale==null || panel==null || publicData==null || assignmentData==null ||
                uiSourceLock==null || railwayAssets==null || campaignText==null) throw new ArgumentException("HUMAN_SCENE_REFERENCE_MISSING");
            var json=new CanonicalJsonAdapter();
            var current=new MobileCandidateData(publicData.bytes,publicCatalogHashSha256);
            var assigned=new HumanAssignmentCatalog(assignmentData.bytes,assignmentBytesSha256,current,json);
            if (assigned.UiSourceSha256!=LocalSaveHash.Bytes(uiSourceLock.bytes)) throw new ArgumentException("HUMAN_UI_LOCK_MISMATCH");
        }
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
                presenter=new ProductionGamePresenter(Document.rootVisualElement,locale!,()=>clock.MonotonicMilliseconds);
                presenter.BindAssetCatalog(railwayAssets!,true);
                var screen=new GameObject("Assigned puzzle screen"); screen.transform.SetParent(transform,false);
                output=screen.AddComponent<Camera>(); output.cullingMask=0; output.clearFlags=CameraClearFlags.SolidColor;
                output.backgroundColor=new Color(.96f,.92f,.81f); output.depth=100;
                output.enabled=SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null;
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
                var current=new MobileCandidateData(publicData!.bytes,publicCatalogHashSha256);
                catalog=new HumanAssignmentCatalog(assignmentData!.bytes,assignmentBytesSha256,current,json);
                string root=UnityEngine.Application.persistentDataPath;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                if(testRoot!=null)root=testRoot;
#endif
                repository=new HumanCalibrationStore(root,catalog,json);
                session=new AssignedHumanSession(catalog,repository,clock!,dispatcher!,
                    UnityEngine.Application.isEditor?HumanRunOrigin.AUTOMATED_TEST:HumanRunOrigin.PARTICIPANT_UI);
                texts=new ProductionCampaignText(campaignText!,true);
                exporter=new HumanLocalExportAdapter(HumanAssignmentCatalog.ApplicationId);
                presenter!.BindAssignedReview(session,assignment=>ResolveDisplay(assignment,current),ExportAsync);
                ApplySuspension();
                await session.InitializeAsync(lifetime.Token);
                if(!disposed)presenter.Refresh();
            }
            catch(OperationCanceledException){if(!disposed)Fail(new IOException("HUMAN_START_CANCELLED"));}
            catch(Exception error){if(!disposed)Fail(error);}
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
            byte[] bytes=await repository.ExportAsync(lifetime.Token);
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
            dispatcher?.Drain(); ApplySafeArea(); ApplySuspension(); presenter?.Refresh();
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
            if(ride==null&&session.State.CanStartRide&&view!=null&&!paused&&focused)
            {
                var attempt=view.AttemptId;
                if(session.BeginTrainRide(attempt)==DevelopmentCommandStatus.Applied)
                {ride=new ProductionTrainRide(view.BuiltPath,view.EndpointA,view.EndpointB,view.Width,view.Height);rideAttempt=attempt;}
            }
            if(ride==null)return;
            if(view==null||view.AttemptId!=rideAttempt||view.Phase!=DevelopmentPreviewPhase.TrainRide){DisposeRide();return;}
            if(!paused&&focused)ride.Tick(Time.unscaledDeltaTime);
            presenter!.SetTrainPose(ride.X,ride.Y,ride.AngleDegrees,true);
            if(ride.IsFinished&&session.CompleteTrainRide(rideAttempt)==DevelopmentCommandStatus.Applied)DisposeRide();
        }
        private void DisposeRide(){ride?.Dispose();ride=null;rideAttempt=Guid.Empty;presenter?.SetTrainPose(0,0,0,false);}
        public void SetPaused(bool value){paused=value;ApplySuspension();}
        public void SetFocused(bool value){focused=value;ApplySuspension();}
        private void OnApplicationPause(bool value)=>SetPaused(value);
        private void OnApplicationFocus(bool value)=>SetFocused(value);
        private void OnApplicationQuit()=>SetPaused(true);
        private void ApplySuspension()=>presenter?.SetAccessibilitySuspended(paused||!focused);
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
            if(disposed)return;disposed=true;lifetime.Cancel();DisposeRide();presenter?.Dispose();session?.Dispose();texts?.Dispose();dispatcher?.Dispose();
            if(output!=null)Destroy(output.gameObject);
            ShutdownTask=FinishShutdownAsync();
        }
        private async Task FinishShutdownAsync(){try{await Task.WhenAll(InitializeTask,tick);}catch(Exception){}finally{lifetime.Dispose();}}
    }
}
