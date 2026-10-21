using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Phone-first game navigation, direct cell intents and confirmed-save presentation.</summary>
    public sealed partial class ProductionGamePresenter : IDisposable
    {
        private enum Screen { Home, Map, Puzzle, Depot, Profile }
        private readonly int ownerThread=Thread.CurrentThread.ManagedThreadId;
        private readonly ProductionGameLocalization locale;
        private readonly Func<long> clock;
        private readonly Func<SavedReviewUiRequest,Task>? runRequest;
        private readonly Dictionary<string,Button> buttons=new Dictionary<string,Button>(StringComparer.Ordinal);
        private readonly Dictionary<string,Action> actions=new Dictionary<string,Action>(StringComparer.Ordinal);
        private readonly Button[] tools=new Button[9];
        private readonly Button[] nodes=new Button[12];
        private readonly RailwayIllustrationElement homeArt;
        private readonly RailwayIllustrationElement depotArt;
        private readonly RailwayIllustrationElement mapArt;
        private readonly RailwayIllustrationElement resultArt;
        private readonly ProductionProductPanels productPanels;
        private readonly ProductionPostgamePanels postgamePanels;
        private Func<int,CampaignPuzzleRecord>? readCampaignRecord;
        private Func<string,ProductionPuzzleDisplay?>? readDynamicText;
        private Func<string?>? activePuzzleMode;
        private long renderedRecordGeneration=-1,renderedMapGeneration=-1;
        private ProductionCampaignText? campaignText;
        private Func<int,bool>? clearedPuzzle;
        private Func<int,bool>? canOpenPuzzle;
        private Func<int>? recommendedPuzzle;
        private string[] selectedCosmetics=Array.Empty<string>();
        private readonly ScrollView viewport;
        private readonly HintDisplayGate hintDisplayGate;
        private readonly ResultContinuationGate resultContinuation=new ResultContinuationGate();
        private readonly ProductionResultSheet resultSheet;
        private readonly ProductionGameAccessibility accessibility;
        private SavedPublic240ReviewSession? model;
        private IReviewPuzzleSession? view;
        private Task? pending;
        private Screen screen;
        private bool requestFault;
        private bool requestCancelled;
        private bool disposed;
        private bool openAfterCreate;
        private bool openAfterRequest;
        private bool zoom;
        private int mapStart;
        private int mapRoute=-1;
        private int mapClears=-1;
        private bool resetConfirmation;
        private SaveCandidateKind? recovery;
        private long recoveryVersion=-1;
        private long renderedRevision=-1;
        private long renderedSecond=-1;
        private long renderedState=-1;
        private bool renderedInteractive;
        private DevelopmentTool activeTool=DevelopmentTool.TrackEW;
        private RailwayAssetCatalog? assetCatalog;
        private ILockedCosmeticsCatalog? cosmeticsCatalog;
        private Func<string,bool>? ownsCosmetic;
        private Func<string,bool>? equippedCosmetic;
        private Func<string,bool>? canActCosmetic;
        private Func<string,Task>? cosmeticAction;
        private readonly Dictionary<string,Label> cosmeticStates=new Dictionary<string,Label>(StringComparer.Ordinal);
        private readonly Dictionary<string,Button> cosmeticButtons=new Dictionary<string,Button>(StringComparer.Ordinal);
        private Func<Task<SafePuzzleHint?>>? prepareHint;
        private Func<SafeHintDisplayReceipt,Task>? commitHint;
        private Func<int>? hintCreditCount;
        private Action<ProductionFeedbackCue>? feedback;
        private Task<SafePuzzleHint?>? hintPreparation;
        private Task? hintCommit;
        private SafePuzzleHint? hintOffer;
        private SafeHintDisplayReceipt? hintReceipt;
        private IReviewPuzzleSession? hintView;
        private Guid hintAttempt;
        private long hintRevision;
        private bool hintHelpOpen;
        private bool hintAcknowledged;
        private bool hintCommitFailed;
        private Func<bool>? canRequestHintVideo;
        private Func<Task>? requestHintVideo;
        private Task? hintVideoTask;
        private bool hintVideoFailed;

        /// <summary>Creates the product view on a cloned UXML tree; no repository or provider is opened.</summary>
        public ProductionGamePresenter(VisualElement root,TextAsset productLocale,Func<long> monotonicMilliseconds,
            Func<SavedReviewUiRequest,Task> runRequest)
            :this(root,productLocale,monotonicMilliseconds)
        {this.runRequest=runRequest??throw new ArgumentNullException(nameof(runRequest));}

        /// <summary>Creates the same view without a saved-profile callback, for the exclusive assigned-review composition.</summary>
        public ProductionGamePresenter(VisualElement root,TextAsset productLocale,Func<long> monotonicMilliseconds)
        {
            Root=root??throw new ArgumentNullException(nameof(root));
            clock=monotonicMilliseconds??throw new ArgumentNullException(nameof(monotonicMilliseconds));
            locale=new ProductionGameLocalization(productLocale);
            try
            {
                foreach(var element in Root.Query<Label>().ToList())
                    if(!string.IsNullOrEmpty(element.viewDataKey))element.text=L(element.viewDataKey);
                homeArt=new RailwayIllustrationElement();Require<VisualElement>("home-art").Add(homeArt);
                mapArt=new RailwayIllustrationElement();Require<VisualElement>("map-art").Add(mapArt);
                depotArt=new RailwayIllustrationElement();Require<VisualElement>("depot-art").Add(depotArt);
                resultArt=new RailwayIllustrationElement();Require<VisualElement>("result-art").Add(resultArt);
                resultSheet=new ProductionResultSheet(Require<ScrollView>("result-panel"),Require<VisualElement>("puzzle-screen"));
                productPanels=new ProductionProductPanels(Root,locale);
                postgamePanels=new ProductionPostgamePanels(Root,locale,()=>Navigate(Screen.Map));
                viewport=Require<ScrollView>("puzzle-viewport");
                viewport.horizontalScrollerVisibility=ScrollerVisibility.Hidden;viewport.verticalScrollerVisibility=ScrollerVisibility.Hidden;
                viewport.RegisterCallback<GeometryChangedEvent>(OnViewportGeometry);
                AddButton("home-play","play",OpenNext);
                AddButton("home-map","map.open",()=>Navigate(Screen.Map));
                AddButton("map-back","back",()=>Navigate(Screen.Home));
                AddButton("map-previous","map.previous",()=>ChangeMapRoute(-1));AddButton("map-next","map.next",()=>ChangeMapRoute(1));
                AddButton("puzzle-back","back",BackFromPuzzle);
                AddButton("puzzle-undo","undo",Undo);
                AddButton("puzzle-hint","hint",RequestHint);
                AddButton("hint-close","notice.close",CloseHint);
                AddButton("hint-retry","hint.retry",RetryHintCommit);
                AddButton("hint-video","hint.video",RequestHintVideo);AddButton("hint-request","hint.request",PrepareOpenHint);
                hintDisplayGate=new HintDisplayGate(Require<VisualElement>("hint-overlay"),Require<Label>("hint-fact"),()=>!externalUiSuspended);
                AddButton("puzzle-zoom","zoom",ToggleZoom);
                AddButton("result-next","result.next",ContinueFromResult);
                AddButton("result-home","result.home",BackFromResult);
                AddButton("nav-home","nav.home",()=>Navigate(Screen.Home));
                AddButton("nav-depot","nav.depot",()=>Navigate(Screen.Depot));
                AddButton("nav-profile","nav.profile",()=>Navigate(Screen.Profile));
                AddButton("error-retry","retry",Retry);
                AddButton("error-reload","reload",()=>SendBound(SavedReviewUiRequestKind.Reload));
                AddButton("error-recover","recover",ConfirmRecovery);
                AddButton("error-reset","reset",ShowResetConfirmation);
                AddButton("reset-confirm","reset.confirm",()=>SendBound(SavedReviewUiRequestKind.ConfirmReset));
                AddButton("reset-cancel","cancel",()=>{resetConfirmation=false;Refresh();});
                AddButton("notice-close","notice.close",()=>Require<VisualElement>("notice-overlay").style.display=DisplayStyle.None);
                for(var i=0;i<tools.Length;i++)
                {
                    var captured=(DevelopmentTool)i;var id="tool-"+i;
                    AddButton(id,ToolKey(captured),()=>ChooseTool(captured));tools[i]=buttons[id];
                    tools[i].text=string.Empty;tools[i].tooltip=L(ToolKey(captured));
                    var glyph=new ProductionTrackGlyph(true);glyph.SetContent(captured);tools[i].Add(glyph);
                }
                var nodeHost=Require<VisualElement>("map-nodes");
                for(var i=0;i<nodes.Length;i++)
                {
                    var number=i;nodes[i]=new Button(()=>OpenMapNode(number)){name="map-node-"+i};
                    nodes[i].AddToClassList("map-node");nodeHost.Add(nodes[i]);
                }
                accessibility=new ProductionGameAccessibility(Root,locale.Resolve);
                ShowStartupLoading();
            }
            catch {Unsubscribe();locale.Dispose();throw;}
        }

        /// <summary>Root supplied product tree.</summary>
        public VisualElement Root{get;}
        /// <summary>Current stable grid, absent before an actual puzzle view is supplied.</summary>
        public ProductionPuzzleGrid? Grid{get;private set;}
        /// <summary>Persistent active tool, independent of every cell value.</summary>
        public DevelopmentTool ActiveTool=>activeTool;
        /// <summary>Observed pending UI operation; completion alone never grants gameplay success.</summary>
        public Task? PendingRequest=>pending;
        /// <summary>Whether an actual Application bridge has been bound.</summary>
        public bool HasBoundModel=>model!=null;
        /// <summary>Startup lifetime, distinct from save authority.</summary>
        public SavedStartupPhase StartupPhase{get;private set;}
        /// <summary>Conservative local latch for failed asynchronous requests.</summary>
        public bool HasRequestFault=>requestFault;
        /// <summary>Whether help owns a modal gameplay pause; lifecycle/other modals must preserve it.</summary>
        public bool IsHelpOpen=>hintHelpOpen;
        /// <summary>Whether Daily/settings owns attention; lifecycle must keep gameplay paused.</summary>
        public bool IsProductModalOpen=>productPanels.IsModalOpen||postgamePanels.IsModalOpen;
        /// <summary>One explicit next-result service policy is pending; ordinary Home/Open/Refresh never starts it.</summary>
        public bool IsResultContinuationPending=>resultContinuation.IsPending;
        /// <summary>Only the actual visible puzzle screen is eligible for lifecycle resume.</summary>
        public bool IsPuzzleScreenActive=>screen==Screen.Puzzle;

        /// <summary>Binds the actual saved bridge exactly once.</summary>
        public void BindModel(SavedPublic240ReviewSession value)
        {
            CheckThread();if(disposed||model!=null||assignedReview!=null||runRequest==null)throw new InvalidOperationException("PRODUCT_UI_ALREADY_BOUND");
            model=value??throw new ArgumentNullException(nameof(value));StartupPhase=SavedStartupPhase.Bound;requestFault=false;requestCancelled=false;Refresh();
        }
        /// <summary>Shows model-free loading without a fabricated session.</summary>
        public void ShowStartupLoading(){CheckStartup();StartupPhase=SavedStartupPhase.Loading;Require<Label>("startup-message").text=L("startup.loading");Refresh();}
        /// <summary>Shows a closed safe composition failure.</summary>
        public void ShowStartupFailure(SavedStartupFailure failure)
        {
            CheckStartup();if(!Enum.IsDefined(typeof(SavedStartupFailure),failure))throw new ArgumentOutOfRangeException(nameof(failure));StartupPhase=SavedStartupPhase.Failed;
            Require<Label>("startup-message").text=L(failure==SavedStartupFailure.LocalStorageUnavailable?"startup.storage":"startup.failed");Refresh();
        }
        /// <summary>Reports an observed root operation failure without claiming an uncommitted result.</summary>
        public void ShowRequestFailure(bool cancelled){CheckThread();requestFault=true;requestCancelled=cancelled;Refresh();}
        /// <summary>Clears only the UI latch after the root's fresh explicit observation.</summary>
        public void ClearRequestFailure(){CheckThread();requestFault=false;requestCancelled=false;Refresh();}
        /// <summary>Projects an explicit root-owned ride pose into the board.</summary>
        public void SetTrainPose(float x,float y,float clockwiseDegrees,bool visible){CheckThread();Grid?.SetTrainPose(x,y,clockwiseDegrees,visible);}

        /// <summary>Binds the complete presentation manifest. Draft acceptance must be explicitly internal.</summary>
        public void BindAssetCatalog(TextAsset source,bool allowInternalDraft=false)=>BindAssetCatalogCore(()=>new RailwayAssetCatalog(source,allowInternalDraft));
        /// <summary>Binds only an explicit FIXTURE_ONLY technical catalog to the same existing render path.</summary>
        public void BindContractTestAssetCatalog(TextAsset source)=>BindAssetCatalogCore(()=>RailwayAssetCatalog.ForContractTest(source));
        private void BindAssetCatalogCore(Func<RailwayAssetCatalog> read)
        {
            CheckThread();if(disposed||assetCatalog!=null)throw new InvalidOperationException("PRODUCT_ASSETS_ALREADY_BOUND");
            assetCatalog=read();postgamePanels.BindScene(assetCatalog);
            var host=Require<VisualElement>("depot-catalog");
            foreach(var item in assetCatalog.CosmeticAssets)
            {
                var id=item.id;var card=new VisualElement{name="cosmetic-"+id};card.AddToClassList("cosmetic-card");
                card.Add(new RailwayCosmeticElement(id));var title=new Label(L("cosmetic."+id+".title"));title.AddToClassList("cosmetic-title");card.Add(title);
                var state=new Label(L("cosmetic.preview"));state.AddToClassList("cosmetic-state");card.Add(state);cosmeticStates.Add(id,state);
                var button=new Button(()=>RequestCosmetic(id)){name="cosmetic-action-"+id,text=L("cosmetic.preview")};button.AddToClassList("secondary-button");button.SetEnabled(false);card.Add(button);cosmeticButtons.Add(id,button);host.Add(card);
            }
            Refresh();
        }
        /// <summary>Binds confirmed ownership and root-managed acquisition/equipment intents, never grants locally.</summary>
        public void BindCosmetics(ILockedCosmeticsCatalog catalog,Func<string,bool> owns,Func<string,bool> equipped,
            Func<string,bool> canAct,Func<string,Task> requestAction)
        {
            CheckThread();CheckProductBinding();cosmeticsCatalog=catalog??throw new ArgumentNullException(nameof(catalog));ownsCosmetic=owns??throw new ArgumentNullException(nameof(owns));
            equippedCosmetic=equipped??throw new ArgumentNullException(nameof(equipped));canActCosmetic=canAct??throw new ArgumentNullException(nameof(canAct));cosmeticAction=requestAction??throw new ArgumentNullException(nameof(requestAction));Refresh();
        }
        /// <summary>Binds root-managed safe hint preparation, durable display acknowledgement and confirmed credits.</summary>
        public void BindHints(Func<Task<SafePuzzleHint?>> prepare,Func<SafeHintDisplayReceipt,Task> acknowledge,Func<int> credits)
        {
            CheckThread();if(disposed||hintHelpOpen||assignedReview!=null)throw new InvalidOperationException("HINT_BINDING_NOT_AVAILABLE");
            prepareHint=prepare??throw new ArgumentNullException(nameof(prepare));commitHint=acknowledge??throw new ArgumentNullException(nameof(acknowledge));hintCreditCount=credits??throw new ArgumentNullException(nameof(credits));
        }

        /// <summary>Binds an explicitly requested optional extra-credit video, only after regular credits are exhausted.</summary>
        public void BindHintVideo(Func<bool> canRequest,Func<Task> request)
        {CheckThread();CheckProductBinding();canRequestHintVideo=canRequest??throw new ArgumentNullException(nameof(canRequest));requestHintVideo=request??throw new ArgumentNullException(nameof(request));}
        /// <summary>Root suppresses the native semantic hierarchy during actual SDK, focus or lifecycle interruptions.</summary>
        public void SetAccessibilitySuspended(bool value){CheckThread();if(disposed)return;var changed=externalUiSuspended!=value;externalUiSuspended=value;if(changed)assignedReview?.SetSuspension(HumanPauseReason.External,value);accessibility.SetSuspended(value);if(!value)hintDisplayGate.Recheck();}
        /// <summary>Binds optional action feedback.</summary>
        public void BindFeedback(Action<ProductionFeedbackCue> receive){CheckThread();feedback=receive??throw new ArgumentNullException(nameof(receive));}
        private void EmitFeedback(ProductionFeedbackCue cue){try{feedback?.Invoke(cue);}catch{ /* Optional presentation feedback has no gameplay authority. */ }}
        /// <summary>Binds the Root policy invoked only by the explicit Next button of an actual completed ride.</summary>
        public void BindResultContinuation(Func<Task> beforeContinue){CheckThread();CheckProductBinding();resultContinuation.Bind(beforeContinue);}
        /// <summary>Binds actual four-profile postgame eligibility and exact saved record operations.</summary>
        public void BindEndless(Func<EndlessView> read,Func<string,Task> startProfile,Func<string,Task> resumeId,Func<string,Task> abandonId)
        {CheckThread();CheckProductBinding();postgamePanels.BindEndless(read,startProfile,resumeId,abandonId);}
        /// <summary>Binds explicit video or provider-free skip on an open generated completion record.</summary>
        public void BindEndlessReward(Func<string,bool> canRequest,Func<string,bool,Task> request,Func<string,bool>? canSkip=null){CheckThread();CheckProductBinding();postgamePanels.BindEndlessReward(canRequest,request,canSkip);}
        /// <summary>Resolves an actual generated profile through the product locale, without campaign identities or generator details.</summary>
        public ProductionPuzzleDisplay ResolveEndlessDisplay(string profileId)
        {
            CheckThread();if(disposed)throw new ObjectDisposedException(nameof(ProductionGamePresenter));
            if(!EndlessView.KnownProfile(profileId))throw new ArgumentException("ENDLESS_DISPLAY_PROFILE_INVALID",nameof(profileId));
            return new ProductionPuzzleDisplay(L("endless.title"),L("endless.profile."+profileId),L("endless.puzzle.title"),L("puzzle.instruction"),L("endless.puzzle.result"));
        }
        /// <summary>Binds the actual optional +10 completion claim; task completion alone never grants it.</summary>
        public void BindCompletionBonus(Func<CompletionBonusView> read,Func<bool,Task> request){CheckThread();CheckProductBinding();postgamePanels.BindCompletionBonus(read,request);}
        /// <summary>Binds actual saved campaign best records for card and profile projection.</summary>
        public void BindCampaignRecords(Func<int,CampaignPuzzleRecord> read){CheckThread();CheckProductBinding();readCampaignRecord=read??throw new ArgumentNullException(nameof(read));renderedRecordGeneration=-1;renderedMapGeneration=-1;Refresh();}
        /// <summary>Binds localized metadata for actual generated identities, without inserting them into the campaign.</summary>
        public void BindDynamicPuzzleText(Func<string,ProductionPuzzleDisplay?> read){CheckThread();readDynamicText=read??throw new ArgumentNullException(nameof(read));}
        /// <summary>Binds the actual saved FIRST_RUN/PRACTICE/REVISION/ENDLESS mode name, never inferring it from later progress.</summary>
        public void BindActivePuzzleMode(Func<string?> read){CheckThread();CheckProductBinding();activePuzzleMode=read??throw new ArgumentNullException(nameof(read));}
        /// <summary>Root calls this after the actual generic saved session has bound, never after a merely proposed generation.</summary>
        public void ShowCurrentPuzzle()
        {
            CheckThread();if(model?.PuzzleView==null||!model.CanRunProfileAction||requestFault)throw new InvalidOperationException("PRODUCT_SAVED_PUZZLE_NOT_READY");
            postgamePanels.CloseForPuzzle();screen=Screen.Puzzle;Refresh();
        }
        /// <summary>Binds actual persistent Daily eligibility and Root-owned free or optional video flow.</summary>
        public void BindDaily(Func<DailyView> read,Func<bool,Task> action){CheckThread();CheckProductBinding();productPanels.BindDaily(read,action);}
        /// <summary>Binds immutable settings and an acknowledged atomic save, retaining all untouched fields.</summary>
        public void BindSettings(Func<LocalSettings> read,Func<LocalSettings,Task> save){CheckThread();CheckProductBinding();productPanels.BindSettings(read,save);}
        /// <summary>Binds actual native privacy/Store readiness and explicitly requested actions.</summary>
        public void BindServices(Func<ProductionServiceState> read,Func<ProductionServiceAction,Task> action){CheckThread();CheckProductBinding();productPanels.BindServices(read,action);}
        /// <summary>Binds the exact 720 player texts; draft use requires an explicit internal opt-in.</summary>
        public void BindCampaignText(TextAsset source,bool allowInternalDraft=false)=>BindCampaignTextCore(()=>new ProductionCampaignText(source,allowInternalDraft));
        /// <summary>Binds only the dedicated technical FIXTURE_ONLY text source.</summary>
        public void BindContractTestCampaignText(TextAsset source)=>BindCampaignTextCore(()=>ProductionCampaignText.ForContractTest(source));
        private void BindCampaignTextCore(Func<ProductionCampaignText> read)
        {CheckThread();if(campaignText!=null)throw new InvalidOperationException("CAMPAIGN_TEXT_ALREADY_BOUND");campaignText=read();renderedRecordGeneration=-1;Refresh();}
        /// <summary>Binds actual identity-based clear and unlock policy, rather than inferring identities from a total.</summary>
        public void BindCampaignProgress(Func<int,bool> cleared,Func<int,bool> canOpen,Func<int> recommendedIndex)
        {CheckThread();CheckProductBinding();clearedPuzzle=cleared??throw new ArgumentNullException(nameof(cleared));canOpenPuzzle=canOpen??throw new ArgumentNullException(nameof(canOpen));recommendedPuzzle=recommendedIndex??throw new ArgumentNullException(nameof(recommendedIndex));mapClears=-1;Refresh();}
        /// <summary>Projects the actual selected canonical cosmetic IDs; this does not grant ownership or affect rules.</summary>
        public void SetActiveCosmetics(IReadOnlyList<string> ids)
        {
            CheckThread();if(ids==null)throw new ArgumentNullException(nameof(ids));var next=new string[ids.Count];
            for(var i=0;i<next.Length;i++){if(!RailwayCosmeticElement.SupportsItem(ids[i]))throw new ArgumentException("COSMETIC_RENDERER_MISSING");next[i]=ids[i];}
            Array.Sort(next,StringComparer.Ordinal);var same=selectedCosmetics.Length==next.Length;
            for(var i=0;same&&i<next.Length;i++)same=selectedCosmetics[i]==next[i];if(same)return;selectedCosmetics=next;
            homeArt.SetActiveCosmetics(next);mapArt.SetActiveCosmetics(next);depotArt.SetActiveCosmetics(next);resultArt.SetActiveCosmetics(next);postgamePanels.SetActiveCosmetics(next);Grid?.SetTrainAppearance(next);
            var train="train-stammrot-default";string? livery=null;foreach(var id in next){if(id.StartsWith("train-",StringComparison.Ordinal))train=id;if(id.StartsWith("livery-",StringComparison.Ordinal))livery=id;}
            Require<Label>("depot-current-name").text=L("cosmetic."+train+".title");Require<Label>("depot-current-body").text=L(train=="train-stammrot-default"?"depot.body":"depot.current.body");
            Require<Label>("depot-current-livery").text=livery==null?L("depot.livery.original"):L("cosmetic."+livery+".title");
        }

        /// <summary>Refreshes confirmed state without rebuilding cells or changing their layout.</summary>
        public void Refresh()
        {
            CheckThread();if(disposed)return;PollRequest();PollHint();PollHintVideo();if(assignedReview!=null){RefreshAssignedReview();return;}productPanels.Refresh();postgamePanels.Refresh();
            var currentResult=screen==Screen.Puzzle&&model?.PuzzleView?.Phase==DevelopmentPreviewPhase.Results;
            if(resultContinuation.PollWhenReady(currentResult,CanContinueResult,model?.PuzzleView?.AttemptId??Guid.Empty))OpenNext();
            var startup=Require<VisualElement>("startup-overlay");startup.style.display=model==null?DisplayStyle.Flex:DisplayStyle.None;
            if(model==null)
            {
                buttons["home-play"].SetEnabled(false);SetVisible(buttons["error-retry"],false);
                var retry=Require<Button>("startup-retry");retry.text=L("retry");retry.SetEnabled(pending==null);retry.style.display=StartupPhase==SavedStartupPhase.Failed?DisplayStyle.Flex:DisplayStyle.None;
                if(!actions.ContainsKey("startup-retry")){actions.Add("startup-retry",()=>Send(SavedReviewUiRequest.StartupRetry()));buttons.Add("startup-retry",retry);retry.clicked+=actions["startup-retry"];}
                return;
            }
            var state=model.State;
            BindPuzzleView(model.PuzzleView);
            var clear=pending==null&&!requestFault&&!postgamePanels.IsBusy&&!resultContinuation.IsPending;
            var editing=clear&&!IsProductModalOpen&&state.CanMutate&&view!=null&&view.Phase==DevelopmentPreviewPhase.Editing&&!view.IsPaused;
            if(view!=null&&view.Phase==DevelopmentPreviewPhase.TrainRide)screen=Screen.Puzzle;
            if(view!=null)
            {
                if(renderedRevision!=view.Revision||renderedState!=state.Version||renderedInteractive!=editing)
                {Grid?.Refresh(editing);renderedRevision=view.Revision;renderedInteractive=editing;}
                var second=view.ElapsedAt(clock())/1000;
                if(second!=renderedSecond){Require<Label>("puzzle-time").text=L("time",second/60,second%60);Require<Label>("result-time").text=L("result.time",second/60,second%60);renderedSecond=second;}
                var index=CurrentCampaignIndex;var section=Math.Max(0,index)/48;var route=(Math.Max(0,index)%48)/12;
                var display=index>=0?campaignText?.Resolve(index,L("section."+section),L("route."+(section*4+route))):readDynamicText?.Invoke(view.Puzzle.Identity.PuzzleId)??new ProductionPuzzleDisplay(L("endless.title"),L("endless.puzzle.title"),L("endless.puzzle.title"),L("puzzle.instruction"),L("endless.puzzle.result"));
                Require<Label>("puzzle-context").text=index>=0?L("puzzle.context",L("section."+section),index%12+1):display!=null?L("endless.puzzle.context",display.Section,display.Route):L("endless.context");
                Require<Label>("puzzle-title").text=display?.Title??L("route."+(section*4+route));
                Require<Label>("puzzle-title").tooltip=display?.Title??string.Empty;
                Require<Label>("result-copy").text=display?.Result??string.Empty;
                var mode=activePuzzleMode?.Invoke();var known=mode=="FIRST_RUN"||mode=="PRACTICE"||mode=="REVISION"||mode=="ENDLESS";
                Require<Label>("result-mode").text=known?L("puzzle.mode."+mode):string.Empty;
                if(index>=0&&(mode=="PRACTICE"||mode=="REVISION"))Require<Label>("puzzle-context").text=L("puzzle.context",L("puzzle.mode."+mode),index%12+1);
                Require<Label>("puzzle-phase").tooltip=display?.Status??string.Empty;
                Require<Label>("puzzle-phase").text=L(state.Status==SavedReviewStatus.TrainRide?"ride.running":state.Status==SavedReviewStatus.ProgressCommitted||state.Status==SavedReviewStatus.CompletionPending?"ride.preparing":"puzzle.instruction");
                if(display!=null&&view.Phase==DevelopmentPreviewPhase.Editing)Require<Label>("puzzle-phase").text=display.Status;
                SetVisible(Require<VisualElement>("result-panel"),view.Phase==DevelopmentPreviewPhase.Results&&!requestFault);
                var stars=state.Progress?.SelectedStars??0;
                Require<Label>("result-stars").text=L("result.stars."+Mathf.Clamp(stars,0,3));
                buttons["result-next"].text=L(index>=0?"result.next":"endless.next");
                Require<Label>("result-progress").text=L("progress",state.Progress?.FirstClearCount??0,state.Progress?.MemberCount??240);
                buttons["puzzle-back"].SetEnabled(clear&&view.Phase==DevelopmentPreviewPhase.Editing);
                buttons["puzzle-undo"].SetEnabled(editing&&view.CanUndo);buttons["puzzle-hint"].SetEnabled(editing&&hintPreparation==null&&!hintHelpOpen);
                buttons["puzzle-zoom"].SetEnabled(view.Phase==DevelopmentPreviewPhase.Editing);
                for(var i=0;i<tools.Length;i++){tools[i].SetEnabled(editing);tools[i].EnableInClassList("tool--active",i==(int)activeTool);}
            }
            var confirmed=state.Progress;var clears=confirmed?.FirstClearCount??0;var next=NextIndex;
            Require<Label>("home-balance").text=L("balance",confirmed?.BalancePatience??0L);
            Require<Label>("home-section").text=L("section."+(next/48));
            Require<Label>("home-route").text=L("route."+(next/12));
            Require<Label>("home-progress").text=L("progress",clears,confirmed?.MemberCount??240);
            Require<Label>("profile-clears").text=L("profile.clears",clears);
            Require<Label>("profile-balance").text=L("profile.balance",confirmed?.BalancePatience??0L);
            Require<Label>("result-balance").text=L("result.balance",confirmed?.BalancePatience??0L);
            homeArt.SetWorld(next/48);depotArt.SetWorld(next/48);mapArt.SetWorld(next/48);
            if(assetCatalog!=null)
            {
                var moment=assetCatalog.ResolvePuzzleMoment(model.Puzzles[next].Identity.PuzzleId,false);
                var composition=assetCatalog.ResolveComposition(moment.compositionId);
                homeArt.SetComposition(composition,moment.stage);depotArt.SetComposition(composition,moment.stage);
                var mapIndex=(mapRoute<0?next/12:mapRoute)*12;var mapMoment=assetCatalog.ResolvePuzzleMoment(model.Puzzles[mapIndex].Identity.PuzzleId,false);
                mapArt.SetComposition(assetCatalog.ResolveComposition(mapMoment.compositionId),next/12==mapIndex/12?next%12+1:0);
                if(view!=null&&CurrentCampaignIndex>=0)
                {
                    var trainMoment=assetCatalog.ResolvePuzzleMoment(view.Puzzle.Identity.PuzzleId,true);
                    resultArt.SetComposition(assetCatalog.ResolveComposition(trainMoment.compositionId),trainMoment.stage);
                }
                else if(view!=null){var generated=assetCatalog.ResolveReusableMoment("draft-endless-train-default");resultArt.SetComposition(assetCatalog.ResolveComposition(generated.compositionId),0);}
                RenderCosmetics(clear);
            }
            buttons["home-play"].text=L(view!=null&&view.Phase==DevelopmentPreviewPhase.Editing?"resume":"play");
            buttons["home-play"].SetEnabled(clear&&(state.CanCreateProfile||state.CanNavigate||view!=null));
            buttons["home-map"].SetEnabled(clear&&state.CanNavigate);
            buttons["result-next"].SetEnabled(clear&&state.CanNavigate);buttons["result-home"].SetEnabled(clear);
            if(mapClears!=clears||mapStart!=(mapRoute<0?next/12:mapRoute)*12||renderedMapGeneration!=(state.Progress?.Generation??0))
            {
                mapStart=(mapRoute<0?next/12:mapRoute)*12;mapClears=clears;renderedMapGeneration=state.Progress?.Generation??0;
                Require<Label>("map-title").text=L("route."+(mapStart/12));
                for(var i=0;i<nodes.Length;i++)
                {
                    var record=readCampaignRecord?.Invoke(mapStart+i);nodes[i].text=record!=null&&record.Stars>0?L("map.node.record",i+1,new string('★',record.Stars)):L("map.node",i+1);nodes[i].tooltip=L(IsCleared(mapStart+i)?"map.cleared":mapStart+i==next?"map.current":"map.locked");
                    nodes[i].EnableInClassList("map-node--cleared",IsCleared(mapStart+i));nodes[i].EnableInClassList("map-node--current",mapStart+i==next);
                }
            }
            for(var i=0;i<nodes.Length;i++)nodes[i].SetEnabled(clear&&state.CanNavigate&&CanOpen(mapStart+i));
            buttons["map-previous"].SetEnabled(clear&&mapStart>0);buttons["map-next"].SetEnabled(clear&&mapStart<228);
            RenderCampaignRecords(state.Progress?.Generation??0);RenderErrors(state,clear);RenderScreen();renderedState=state.Version;
            RenderHint();resultSheet.Refresh();
        }

        /// <summary>Releases all owned event handlers and localization objects.</summary>
        public void Dispose()
        {
            CheckThread();if(disposed)return;disposed=true;StartupPhase=SavedStartupPhase.Disposed;Unsubscribe();hintDisplayGate.Dispose();accessibility.Dispose();resultSheet.Dispose();productPanels.Dispose();postgamePanels.Dispose();resultContinuation.Dispose();campaignText?.Dispose();
            ObserveLateFault(pending);ObserveLateFault(hintPreparation);ObserveLateFault(hintCommit);ObserveLateFault(hintVideoTask);DisposeAssignedReview();
            viewport.UnregisterCallback<GeometryChangedEvent>(OnViewportGeometry);Grid?.Dispose();Grid?.RemoveFromHierarchy();Grid=null;feedback=null;locale.Dispose();
        }
        /// <summary>Closed own localization key for each existing field content.</summary>
        public static string ToolKey(DevelopmentTool value)=>value switch
        {
            DevelopmentTool.Clear=>"tool.clear",DevelopmentTool.EmptyMarker=>"tool.empty",DevelopmentTool.OccupiedMarker=>"tool.occupied",
            DevelopmentTool.TrackNS=>"tool.ns",DevelopmentTool.TrackEW=>"tool.ew",DevelopmentTool.TrackNE=>"tool.ne",DevelopmentTool.TrackES=>"tool.es",DevelopmentTool.TrackSW=>"tool.sw",DevelopmentTool.TrackWN=>"tool.wn",
            _=>throw new ArgumentOutOfRangeException(nameof(value))
        };
        private int CurrentCampaignIndex
        {
            get{if(model==null||view==null)return -1;for(var i=0;i<model.Puzzles.Count;i++)if(model.Puzzles[i].Identity.Equals(view.Puzzle.Identity))return i;return -1;}
        }
        private void RenderCampaignRecords(long generation)
        {
            if(readCampaignRecord==null||renderedRecordGeneration==generation)return;renderedRecordGeneration=generation;
            var host=Require<VisualElement>("profile-records");host.Clear();var direct=0;var completed=0;
            for(var i=0;i<240;i++)
            {
                var record=readCampaignRecord(i);if(record.Stars<=0)continue;completed++;if(record.DirectSolved)direct++;
                var display=campaignText?.Resolve(i,L("section."+(i/48)),L("route."+(i/12)));var row=new VisualElement();row.AddToClassList("profile-record");
                var title=new Label(display?.Title??L("record.number",i+1));title.AddToClassList("profile-record-title");row.Add(title);
                row.Add(new Label(L("record.stars",new string('★',record.Stars))));
                if(record.FirstRunBestElapsedMs.HasValue){var seconds=record.FirstRunBestElapsedMs.Value/1000;row.Add(new Label(L("record.first-time",seconds/60,seconds%60)));}
                if(record.RevisionBestElapsedMs.HasValue){var seconds=record.RevisionBestElapsedMs.Value/1000;row.Add(new Label(L("record.revision-time",seconds/60,seconds%60)));}
                if(record.DirectSolved)row.Add(new Label(L("record.direct")));host.Add(row);
            }
            Require<Label>("profile-direct").text=L("record.direct-total",direct,completed);
        }
        private int NextIndex=>Mathf.Clamp(recommendedPuzzle?.Invoke()??model?.State.Progress?.FirstClearCount??0,0,239);
        private bool IsCleared(int index)=>clearedPuzzle?.Invoke(index)??index<(model?.State.Progress?.FirstClearCount??0);
        private bool CanOpen(int index)=>index>=0&&index<240&&(canOpenPuzzle?.Invoke(index)??index<=NextIndex);
        private string L(string key,params object[] args)=>locale.Resolve(key,args);
        private T Require<T>(string id)where T:VisualElement=>Root.Q<T>(id)??throw new InvalidOperationException("PRODUCT_UI_ELEMENT_MISSING:"+id);
        private void CheckThread(){if(Thread.CurrentThread.ManagedThreadId!=ownerThread)throw new InvalidOperationException("PRODUCT_UI_WRONG_THREAD");}
        private void CheckStartup(){CheckThread();if(disposed)throw new ObjectDisposedException(nameof(ProductionGamePresenter));if(model!=null||assignedReview!=null)throw new InvalidOperationException("PRODUCT_UI_ALREADY_BOUND");}
        private void AddButton(string id,string key,Action action)
        {var button=Require<Button>(id);button.text=L(key);if(key=="back")button.tooltip=L("accessibility.back");buttons.Add(id,button);actions.Add(id,action);button.clicked+=action;}
        private void Unsubscribe(){foreach(var pair in actions)if(buttons.TryGetValue(pair.Key,out var button))button.clicked-=pair.Value;}
        private static void SetVisible(VisualElement target,bool value)=>target.style.display=value?DisplayStyle.Flex:DisplayStyle.None;
        private void RenderScreen()
        {
            SetVisible(Require<VisualElement>("home-screen"),screen==Screen.Home);SetVisible(Require<VisualElement>("map-screen"),screen==Screen.Map);
            SetVisible(Require<VisualElement>("puzzle-screen"),screen==Screen.Puzzle);SetVisible(Require<VisualElement>("depot-screen"),screen==Screen.Depot);
            SetVisible(Require<VisualElement>("profile-screen"),screen==Screen.Profile);SetVisible(Require<VisualElement>("main-navigation"),screen!=Screen.Puzzle&&screen!=Screen.Map);
            buttons["nav-home"].EnableInClassList("nav--active",screen==Screen.Home);buttons["nav-depot"].EnableInClassList("nav--active",screen==Screen.Depot);buttons["nav-profile"].EnableInClassList("nav--active",screen==Screen.Profile);
        }
        private void Navigate(Screen next)
        {
            if(assignedReview!=null)return;
            if(disposed||pending!=null||resultContinuation.IsPending||view?.Phase==DevelopmentPreviewPhase.TrainRide)return;
            if(screen==Screen.Puzzle&&next!=Screen.Puzzle&&view?.Phase==DevelopmentPreviewPhase.Editing)view.Pause(clock());
            screen=next;Refresh();
        }
        private bool CanContinueResult=>!disposed&&screen==Screen.Puzzle&&!hintHelpOpen&&!productPanels.IsModalOpen&&!postgamePanels.IsModalOpen&&pending==null&&!requestFault&&!postgamePanels.IsBusy&&model?.State.Status==SavedReviewStatus.Results&&model.State.CanNavigate&&model.PuzzleView?.Phase==DevelopmentPreviewPhase.Results;
        private void ContinueFromResult()
        {if(assignedReview!=null){AdvanceAssignedReview();return;}if(resultContinuation.TryBegin(CanContinueResult,model?.PuzzleView?.AttemptId??Guid.Empty))Refresh();}
        private void OpenNext()
        {
            if(model==null||pending!=null||resultContinuation.IsPending||requestFault)return;
            var state=model.State;
            if(state.CanCreateProfile){openAfterCreate=true;SendBound(SavedReviewUiRequestKind.CreateProfile);return;}
            if(view!=null&&view.Phase==DevelopmentPreviewPhase.Editing)
            {view.Resume(clock());screen=Screen.Puzzle;Refresh();return;}
            if(view!=null&&view.Phase==DevelopmentPreviewPhase.Results&&CurrentCampaignIndex<0){postgamePanels.Open();Refresh();return;}
            OpenPuzzle(NextIndex);
        }
        private void ChangeMapRoute(int delta){if(pending!=null)return;mapRoute=Mathf.Clamp((mapRoute<0?NextIndex/12:mapRoute)+delta,0,19);mapClears=-1;Refresh();}
        private void OpenMapNode(int node){if(CanOpen(mapStart+node))OpenPuzzle(mapStart+node);}
        private void OpenPuzzle(int index)
        {
            if(model==null||!model.State.CanNavigate||pending!=null)return;
            openAfterRequest=true;
            var restore=model.HasDraftForPuzzle(index);
            Send(SavedReviewUiRequest.Puzzle(restore?SavedReviewUiRequestKind.ResumeDraft:SavedReviewUiRequestKind.OpenPuzzle,model.State.Version,index));
        }
        private void ChooseTool(DevelopmentTool tool)
        {if(!CanEdit||activeTool==tool)return;activeTool=tool;Refresh();EmitFeedback(ProductionFeedbackCue.ToolFocus);}
        private void RenderCosmetics(bool clear)
        {
            if(assetCatalog==null||cosmeticsCatalog==null||ownsCosmetic==null||equippedCosmetic==null||canActCosmetic==null)return;
            var selected=new List<string>();
            foreach(var asset in assetCatalog.CosmeticAssets)
            {
                if(!cosmeticsCatalog.TryGet(asset.id,out var item)||item==null||item.AssetAddress!=asset.assetAddress)throw new InvalidOperationException("PRODUCT_COSMETICS_ASSET_MISMATCH");
                var owned=ownsCosmetic(item.Id);var active=equippedCosmetic(item.Id);if(active&&owned)selected.Add(item.Id);var canUnequip=active&&owned&&(item.Type=="LIVERY"||item.Type=="OPERATING_OBJECT");
                cosmeticStates[item.Id].text=L(active?"cosmetic.equipped":owned?"cosmetic.owned":item.Acquisition=="MILESTONE_GRANT"?"cosmetic.milestone":"cosmetic.available");
                var button=cosmeticButtons[item.Id];button.text=owned?L(active?canUnequip?"cosmetic.unequip":"cosmetic.equipped":"cosmetic.equip"):
                    item.PricePatience.HasValue?L("cosmetic.price",item.PricePatience.Value):L("cosmetic.claim");
                button.SetEnabled(clear&&(!active||canUnequip)&&canActCosmetic(item.Id));
            }
            SetActiveCosmetics(selected);
        }
        private void RequestCosmetic(string id)
        {
            if(disposed||pending!=null||cosmeticAction==null||canActCosmetic==null||!canActCosmetic(id))return;
            try{pending=cosmeticAction(id)??throw new InvalidOperationException("COSMETIC_REQUEST_NULL");}catch{requestFault=true;pending=null;}Refresh();
        }
        private void RequestHint()
        {
            if(!CanEdit||view==null||hintPreparation!=null)return;
            if(prepareHint==null||commitHint==null||hintCreditCount==null){ShowNotice("hint.title","hint.body");return;}
            hintView=view;assignedReview?.SetSuspension(HumanPauseReason.Hint,true);view.Pause(clock());hintAttempt=view.AttemptId;hintRevision=view.Revision;
            hintHelpOpen=true;hintOffer=null;hintReceipt=null;hintAcknowledged=false;hintCommitFailed=false;hintVideoFailed=false;hintDisplayGate.Cancel();
            Require<Label>("hint-fact").text=L("hint.preparing");Require<Label>("hint-explanation").text=string.Empty;
            try
            {
                if(hintCreditCount()<=0)Require<Label>("hint-fact").text=L("hint.empty");
                else hintPreparation=prepareHint()??throw new InvalidOperationException("HINT_PREPARE_NULL");
            }
            catch{Require<Label>("hint-fact").text=L("hint.failed");}
            Refresh();
        }
        private void PrepareOpenHint()
        {
            if(!hintHelpOpen||view==null||hintOffer!=null||hintPreparation!=null||hintVideoTask!=null||prepareHint==null||hintCreditCount==null)return;
            try
            {
                if(hintCreditCount()<=0)return;hintView=view;hintAttempt=view.AttemptId;hintRevision=view.Revision;
                hintVideoFailed=false;Require<Label>("hint-fact").text=L("hint.preparing");Require<Label>("hint-explanation").text=string.Empty;
                hintPreparation=prepareHint()??throw new InvalidOperationException("HINT_PREPARE_NULL");
            }
            catch{Require<Label>("hint-fact").text=L("hint.failed");}Refresh();
        }
        private void RequestHintVideo()
        {
            if(!hintHelpOpen||hintOffer!=null||hintVideoTask!=null||hintPreparation!=null||hintCommit!=null||hintCreditCount==null||canRequestHintVideo==null||requestHintVideo==null)return;
            try
            {
                if(hintCreditCount()>0||!canRequestHintVideo())return;hintVideoFailed=false;
                hintVideoTask=requestHintVideo()??throw new InvalidOperationException("HINT_VIDEO_TASK_NULL");
            }
            catch{hintVideoFailed=true;}Refresh();
        }
        private void PollHint()
        {
            if(hintPreparation!=null&&hintPreparation.IsCompleted)
            {
                var completed=hintPreparation;hintPreparation=null;
                try
                {
                    var offer=completed.GetAwaiter().GetResult();
                    if(hintHelpOpen&&CurrentHintAttempt)
                    {
                        hintOffer=offer;
                        if(offer==null)Require<Label>("hint-fact").text=L("hint.no-safe-conclusion");
                        else
                        {
                            Require<Label>("hint-fact").text=L("hint.fact",offer.X+1,offer.Y+1,L(ToolKey((DevelopmentTool)(int)offer.Content)));
                            Require<Label>("hint-explanation").text=L("hint.explanation");
                            hintDisplayGate.Arm(()=>
                            {
                                if(!disposed&&hintHelpOpen&&CurrentHintAttempt&&ReferenceEquals(hintOffer,offer))
                                {hintReceipt=offer.AcknowledgeDisplayed();RetryHintCommit();}
                            });
                        }
                    }
                    else if(hintHelpOpen)Require<Label>("hint-fact").text=L("hint.changed");
                }
                catch{if(hintHelpOpen)Require<Label>("hint-fact").text=L("hint.failed");}
            }
            if(hintCommit!=null&&hintCommit.IsCompleted)
            {
                var completed=hintCommit;hintCommit=null;
                try{completed.GetAwaiter().GetResult();hintAcknowledged=true;hintCommitFailed=false;}
                catch{hintCommitFailed=true;}
            }
        }
        private void PollHintVideo()
        {
            if(hintVideoTask==null||!hintVideoTask.IsCompleted)return;var task=hintVideoTask;hintVideoTask=null;
            try
            {
                task.GetAwaiter().GetResult();var confirmed=hintCreditCount!=null&&hintCreditCount()>0;
                Require<Label>("hint-fact").text=L(confirmed?"hint.video.confirmed":"hint.video.unconfirmed");
            }
            catch{hintVideoFailed=true;Require<Label>("hint-fact").text=L("hint.video.failed");}
        }
        private bool CurrentHintAttempt=>view!=null&&hintView!=null&&ReferenceEquals(view,hintView)&&view.AttemptId==hintAttempt&&view.Revision==hintRevision&&view.IsPaused;
        private void RenderHint()
        {
            SetVisible(Require<VisualElement>("hint-overlay"),hintHelpOpen);
            if(!hintHelpOpen)return;
            buttons["hint-close"].SetEnabled(hintVideoTask==null&&(hintOffer==null||hintAcknowledged));
            SetVisible(buttons["hint-retry"],hintCommitFailed&&hintCommit==null);
            Require<Label>("hint-save-state").text=L(hintCommitFailed?"hint.save.failed":hintReceipt!=null&&!hintAcknowledged?"hint.saving":hintAcknowledged?"hint.saved":"hint.unspent");
            try{Require<Label>("hint-credit-count").text=hintCreditCount==null?string.Empty:L("hint.credits",Math.Max(0,hintCreditCount()));}
            catch{Require<Label>("hint-credit-count").text=L("hint.credits.unavailable");}
            var credits=0;var canVideo=false;
            try{credits=hintCreditCount?.Invoke()??0;canVideo=credits==0&&canRequestHintVideo?.Invoke()==true;}catch{ /* Unavailable service never becomes an offer. */ }
            var quiet=hintPreparation==null&&hintCommit==null&&hintVideoTask==null&&hintOffer==null;
            SetVisible(buttons["hint-video"],canVideo&&hintOffer==null);buttons["hint-video"].SetEnabled(quiet&&canVideo);
            SetVisible(buttons["hint-request"],quiet&&credits>0);
            if(hintVideoTask!=null)Require<Label>("hint-save-state").text=L("hint.video.pending");
            else if(hintVideoFailed)Require<Label>("hint-save-state").text=L("hint.video.failed");
        }
        private void RetryHintCommit()
        {
            if(disposed||!hintHelpOpen||hintReceipt==null||commitHint==null||hintCommit!=null||hintAcknowledged)return;
            hintCommitFailed=false;
            try{hintCommit=commitHint(hintReceipt)??throw new InvalidOperationException("HINT_COMMIT_NULL");}
            catch{hintCommitFailed=true;}
            Refresh();
        }
        private void CloseHint()
        {
            if(!hintHelpOpen||hintVideoTask!=null||(hintOffer!=null&&!hintAcknowledged))return;
            hintHelpOpen=false;hintDisplayGate.Cancel();assignedReview?.SetSuspension(HumanPauseReason.Hint,false);
            if(view!=null&&hintView!=null&&ReferenceEquals(view,hintView)&&view.AttemptId==hintAttempt&&view.IsPaused)view.Resume(clock());
            Refresh();
        }
        private static void ObserveLateFault(Task? task)
        {if(task!=null)_=task.ContinueWith(completed=>{_ = completed.Exception;},CancellationToken.None,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);}
        private bool CanEdit=>!disposed&&!IsProductModalOpen&&!postgamePanels.IsBusy&&!hintHelpOpen&&pending==null&&!requestFault&&(assignedReview!=null?AssignedCanEdit:model?.State.CanMutate==true)&&view?.Phase==DevelopmentPreviewPhase.Editing&&!view.IsPaused;
        private void ActivateCell(int x,int y)
        {
            if(!CanEdit||view==null)return;
            var applied=activeTool!=DevelopmentTool.Clear&&view.CellAt(x,y)==activeTool?DevelopmentTool.Clear:activeTool;
            var status=view.Apply(x,y,applied,clock());Refresh();
            if(status==DevelopmentCommandStatus.Applied)EmitFeedback(applied==DevelopmentTool.Clear?ProductionFeedbackCue.Clear:applied==DevelopmentTool.EmptyMarker?ProductionFeedbackCue.EmptyMark:applied==DevelopmentTool.OccupiedMarker?ProductionFeedbackCue.OccupancyNote:ProductionFeedbackCue.TrackSeat);
        }
        private void Undo(){if(CanEdit){var status=view!.Undo(clock());Refresh();if(status==DevelopmentCommandStatus.Applied)EmitFeedback(ProductionFeedbackCue.Undo);}}
        private void ToggleZoom()
        {
            if(Grid==null)return;zoom=!zoom;LayoutGridViewport();
            buttons["puzzle-zoom"].text=L(zoom?"zoom.close":"zoom");viewport.scrollOffset=Vector2.zero;
        }
        private void OnViewportGeometry(GeometryChangedEvent change)=>LayoutGridViewport();
        private void LayoutGridViewport()
        {
            if(Grid==null)return;
            // Percent heights inside ScrollView are indefinite. Reserve the measured viewport once,
            // then change only contents during Tap/Undo/Save. Detail mode enlarges the pan container too.
            var rect=viewport.contentRect;
            if(rect.width<=0||rect.height<=0)return;
            var width=zoom?Mathf.Max(720,rect.width):rect.width;
            var height=zoom?Mathf.Max(720,rect.height):rect.height;
            viewport.contentContainer.style.width=width;viewport.contentContainer.style.height=height;
            Grid.style.width=width;Grid.style.height=height;
        }
        private void ShowNotice(string title,string body)
        {Require<Label>("notice-title").text=L(title);Require<Label>("notice-body").text=L(body);SetVisible(Require<VisualElement>("notice-overlay"),true);}
        private void RenderErrors(SavedReviewState state,bool clear)
        {
            var problem=requestFault||state.CanRetrySave||state.Status==SavedReviewStatus.Failed||state.Status==SavedReviewStatus.Unsupported||state.Status==SavedReviewStatus.RecoveryRequired;
            // Normal saving is deliberately invisible and never changes the play layout.
            SetVisible(Require<VisualElement>("error-overlay"),problem);
            Require<Label>("error-message").text=L(requestCancelled?"error.cancelled":state.Status==SavedReviewStatus.Unsupported?"error.unsupported":state.CanRecover?"error.recovery":"error.storage");
            SetVisible(buttons["error-retry"],clear&&state.CanRetrySave);SetVisible(buttons["error-reload"],pending==null&&(state.CanRetryLoad||requestFault));
            SetVisible(buttons["error-recover"],clear&&state.CanRecover);SetVisible(buttons["error-reset"],clear&&state.CanReset);
            if(recoveryVersion!=state.Version)
            {
                recoveryVersion=state.Version;recovery=null;var host=Require<VisualElement>("recovery-choices");host.Clear();
                foreach(var candidate in state.RecoveryChoices)
                {
                    var kind=candidate.Kind;var version=state.Version;
                    var option=new Button(()=>{if(model==null||model.State.Version!=version||!model.State.CanRecover||candidate.Status!=SaveCandidateStatus.Valid)return;recovery=kind;Refresh();}){text=L("recovery.candidate",L("recovery.kind."+kind))};
                    option.SetEnabled(candidate.Status==SaveCandidateStatus.Valid&&state.CanRecover);option.AddToClassList("secondary-button");host.Add(option);
                }
            }
            buttons["error-recover"].SetEnabled(recovery.HasValue);SetVisible(Require<VisualElement>("reset-confirmation"),resetConfirmation&&state.CanReset);
        }
        private void ConfirmRecovery(){if(model!=null&&recovery.HasValue&&recoveryVersion==model.State.Version)Send(SavedReviewUiRequest.Recovery(model.State.Version,recovery.Value));}
        private void ShowResetConfirmation(){resetConfirmation=true;Refresh();}
        private void Retry(){if(model!=null)SendBound(model.State.Status==SavedReviewStatus.CompletionPending?SavedReviewUiRequestKind.RetryCompletion:SavedReviewUiRequestKind.RetrySave);}
        private void SendBound(SavedReviewUiRequestKind kind){if(model!=null)Send(SavedReviewUiRequest.Bound(kind,model.State.Version));}
        private void Send(SavedReviewUiRequest request)
        {
            if(disposed||pending!=null)return;
            try{pending=(runRequest??throw new InvalidOperationException("PRODUCT_SAVED_REQUEST_UNBOUND"))(request)??throw new InvalidOperationException("PRODUCT_REQUEST_NULL");}
            catch{requestFault=true;pending=null;}
            Refresh();
        }
        private void PollRequest()
        {
            if(pending==null||!pending.IsCompleted)return;
            var completed=pending;pending=null;
            if(completed.IsFaulted){_ = completed.Exception;requestFault=true;openAfterRequest=false;openAfterCreate=false;}
            else if(completed.IsCanceled){requestFault=true;requestCancelled=true;openAfterRequest=false;openAfterCreate=false;}
            else if(openAfterRequest&&model?.PuzzleView!=null){openAfterRequest=false;screen=Screen.Puzzle;}
            else if(openAfterCreate&&model?.State.CanNavigate==true){openAfterCreate=false;OpenPuzzle(NextIndex);}
        }
    }
}
