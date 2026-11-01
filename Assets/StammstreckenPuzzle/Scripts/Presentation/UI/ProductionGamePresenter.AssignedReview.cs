using System;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    public sealed partial class ProductionGamePresenter
    {
        private AssignedHumanSession? assignedReview;
        private Func<HumanAssignment,ProductionPuzzleDisplay>? assignedDisplay;
        private Func<Task>? exportHuman;
        private Action? showAssignedOperator;
        private ProductionExposureGate? exposureGate;
        private Guid exposureArmed,exposureAcknowledged;
        private HumanExposureReceipt? exposureReceipt;
        private HumanFirstContact? selectedContact;
        private bool priorVersion,humanMenuOpen,humanAbortConfirmation,productFeatureBound,externalUiSuspended;
        private bool? humanInstructionPaused;
        private bool humanNextParticipantOpen,humanNextParticipantRequested;
        private Task? humanExportTask;

        /// <summary>The exclusive actual observation adapter, never a projected product save.</summary>
        public bool HasAssignedReview=>assignedReview!=null;
        /// <summary>Actual instruction/menu attention, independent of external focus and hint pauses.</summary>
        public bool IsAssignedInstructionOpen=>assignedReview!=null&&(!assignedReview.State.ExposureReady||!assignedReview.State.FirstContactRecorded||humanMenuOpen||humanNextParticipantOpen);

        /// <summary>Overrides only the explicit completed-run next-participant intent; opening the operator starts no run.</summary>
        public void BindAssignedOperator(Action showOperator)
        {
            CheckThread();if(disposed||assignedReview==null||showAssignedOperator!=null||pending!=null)throw new InvalidOperationException("HUMAN_OPERATOR_BINDING_NOT_ALLOWED");
            showAssignedOperator=showOperator??throw new ArgumentNullException(nameof(showOperator));
        }

        /// <summary>Binds one assigned observation run. Display metadata must resolve this exact assignment.</summary>
        public void BindAssignedReview(AssignedHumanSession session,Func<HumanAssignment,ProductionPuzzleDisplay> display,Func<Task>? export=null)
        {
            CheckThread();if(disposed||model!=null||assignedReview!=null||productFeatureBound||hintHelpOpen||pending!=null)throw new InvalidOperationException("PRODUCT_UI_ALREADY_BOUND");
            assignedReview=session??throw new ArgumentNullException(nameof(session));assignedDisplay=display??throw new ArgumentNullException(nameof(display));exportHuman=export;
            prepareHint=()=>session.PrepareHintAsync(CancellationToken.None);commitHint=receipt=>session.CommitHintDisplayAsync(receipt,CancellationToken.None);hintCreditCount=()=>session.State.HintCredits;
            canRequestHintVideo=null;requestHintVideo=null;requestFault=false;requestCancelled=false;screen=Screen.Puzzle;
            session.SetSuspension(HumanPauseReason.External,externalUiSuspended);
            AddButton("human-begin","human.begin",()=>RunHuman(()=>session.BeginRunAsync(CancellationToken.None)));
            AddButton("human-show","human.show",()=>RunHuman(()=>session.ReserveExposureAsync(CancellationToken.None)));
            AddButton("human-advance","human.advance",AdvanceAssignedReview);
            AddButton("human-retry","human.retry",()=>RunHuman(RetryHumanObservation,true));
            AddButton("human-export","human.export",ExportHuman);
            AddButton("human-next-participant","human.next-participant",OpenNextParticipant);
            AddButton("human-next-participant-confirm","human.next-participant.confirm",ConfirmNextParticipant);
            AddButton("human-next-participant-cancel","cancel",CancelNextParticipant);
            AddButton("human-never","human.contact.never",()=>SelectContact(HumanFirstContact.NEVER_SEEN));
            AddButton("human-seen","human.contact.seen",()=>SelectContact(HumanFirstContact.SEEN_OR_PLAYED));
            AddButton("human-unsure","human.contact.unsure",()=>SelectContact(HumanFirstContact.UNSURE));
            AddButton("human-prior-version","human.contact.prior-version.off",()=>{if(CanAnswerContact){priorVersion=!priorVersion;Refresh();}});
            AddButton("human-contact-confirm","human.contact.confirm",ConfirmContact);
            AddButton("human-resume","human.menu.resume",CloseHumanMenu);
            AddButton("human-restart","human.menu.restart",RestartHuman);
            AddButton("human-abort","human.menu.abort",()=>{if(CanOpenHumanMenu){humanAbortConfirmation=true;Refresh();}});
            AddButton("human-abort-confirm","human.menu.abort.confirm",AbortHuman);
            AddButton("human-abort-cancel","human.menu.cancel",()=>{if(pending==null){humanAbortConfirmation=false;Refresh();}});
            Refresh();
        }

        private void CheckProductBinding()
        {if(assignedReview!=null)throw new InvalidOperationException("ASSIGNED_REVIEW_PRODUCT_FEATURE_FORBIDDEN");productFeatureBound=true;}

        // The same stable grid and its one ActivateCell callback serve both compositions.
        private void BindPuzzleView(IReviewPuzzleSession? next)
        {
            if(ReferenceEquals(view,next))return;
            exposureGate?.Dispose();exposureGate=null;exposureArmed=Guid.Empty;
            Grid?.Dispose();Grid?.RemoveFromHierarchy();view=next;renderedRevision=-1;renderedSecond=-1;renderedState=-1;zoom=false;
            if(view!=null){Grid=new ProductionPuzzleGrid(view,locale.Resolve,ActivateCell);viewport.Add(Grid);Grid.SetTrainAppearance(selectedCosmetics);LayoutGridViewport();viewport.scrollOffset=UnityEngine.Vector2.zero;}
            else Grid=null;
        }

        private bool HumanBlocked=>assignedReview==null||requestFault||assignedReview.State.ObservationStatus==HumanObservationStatus.RecoveryRequired||assignedReview.State.ObservationStatus==HumanObservationStatus.Unsupported;
        private bool AssignedCanEdit=>!HumanBlocked&&!humanMenuOpen&&!externalUiSuspended&&assignedReview!.State.CanInput;
        private bool CanAnswerContact=>!HumanBlocked&&pending==null&&assignedReview!.State.Trial?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED&&!assignedReview.State.FirstContactRecorded;
        private bool CanOpenHumanMenu=>!HumanBlocked&&pending==null&&!hintHelpOpen&&view?.Phase==DevelopmentPreviewPhase.Editing&&assignedReview!.State.FirstContactRecorded&&assignedReview.State.CanAbort;

        private void RefreshAssignedReview()
        {
            var session=assignedReview!;var state=session.State;
            if(humanNextParticipantRequested&&pending==null)
            {
                if(HumanBlocked){humanNextParticipantRequested=false;humanNextParticipantOpen=false;}
                else if(!state.IsFinished&&state.ObservationStatus==HumanObservationStatus.NeedsExposure)
                {humanNextParticipantRequested=false;humanNextParticipantOpen=false;ResetHumanPresentationForNewRun();}
            }
            if(humanExportTask!=null&&humanExportTask.IsCompleted)
            {
                var task=humanExportTask;humanExportTask=null;
                try{task.GetAwaiter().GetResult();Require<Label>("human-export-state").text=L("human.export.done");}
                catch{Require<Label>("human-export-state").text=L("human.export.failed");}
            }
            // Provider/product panels never read or run in this composition.
            foreach(var id in new[]{"home-screen","map-screen","depot-screen","profile-screen","main-navigation","startup-overlay","error-overlay","notice-overlay","daily-overlay","settings-overlay","endless-overlay"})SetVisible(Require<VisualElement>(id),false);
            var blocked=HumanBlocked;var actual=session.PuzzleView;
            var reveal=!blocked&&state.ExposureReady&&actual!=null&&state.Trial?.Outcome!=HumanTrialOutcome.ABORTED;
            BindPuzzleView(reveal?actual:null);screen=reveal?Screen.Puzzle:Screen.Home;
            SetVisible(Require<VisualElement>("human-screen"),!reveal);SetVisible(Require<VisualElement>("puzzle-screen"),reveal);
            var instruction=!reveal||!state.FirstContactRecorded||humanMenuOpen;
            if(humanInstructionPaused!=instruction){humanInstructionPaused=instruction;session.SetSuspension(HumanPauseReason.Instruction,instruction);}
            var editing=CanEdit;
            if(view!=null)
            {
                if(renderedRevision!=view.Revision||renderedState!=state.Version||renderedInteractive!=editing)
                {Grid?.Refresh(editing);renderedRevision=view.Revision;renderedInteractive=editing;}
                var seconds=view.ElapsedAt(clock())/1000;
                Require<Label>("puzzle-time").text=L("time",seconds/60,seconds%60);
                Require<Label>("result-time").text=L("human.result.active",seconds/60,seconds%60);
                var assignment=state.Assignment??throw new InvalidOperationException("HUMAN_VISIBLE_ASSIGNMENT_MISSING");
                var display=assignedDisplay!(assignment)??throw new InvalidOperationException("HUMAN_ASSIGNMENT_DISPLAY_MISSING");
                Require<Label>("puzzle-context").text=L("human.assigned");Require<Label>("puzzle-title").text=display.Title;Require<Label>("puzzle-title").tooltip=display.Title;
                Require<Label>("puzzle-phase").text=view.Phase==DevelopmentPreviewPhase.TrainRide?L("ride.running"):state.CanStartRide?L("ride.preparing"):state.FirstContactRecorded?display.Status:L("human.contact.body");
                Require<Label>("puzzle-phase").tooltip=Require<Label>("puzzle-phase").text;
                Require<Label>("result-mode").text=L("human.result.mode");Require<Label>("result-copy").text=display.Result;
                if(assetCatalog!=null)
                {
                    var moment=assignment.Purpose==HumanAssignmentPurpose.CAMPAIGN_TIME?assetCatalog.ResolvePuzzleMoment(assignment.Puzzle.Identity.PuzzleId,true):assetCatalog.ResolveReusableMoment("draft-endless-train-default");
                    resultArt.SetComposition(assetCatalog.ResolveComposition(moment.compositionId),moment.stage);
                }
                var preview=state.PreviewActiveMs;var previewSeconds=preview.GetValueOrDefault()/1000;
                Require<Label>("human-result-observation").text=L("human.result.saved")+"\n"+(state.TimingComplete&&preview.HasValue?L("human.result.preview",previewSeconds/60,previewSeconds%60):L("human.result.preview.missing"))+"\n"+L(state.HintCredits<3?"human.result.supported":"human.result.unassisted");
                SetVisible(Require<Label>("human-result-observation"),true);
                SetVisible(Require<VisualElement>("result-panel"),view.Phase==DevelopmentPreviewPhase.Results&&!blocked);
                foreach(var id in new[]{"result-stars","result-progress","result-balance","result-bonus-state","result-video","result-skip-bonus","result-home"})SetVisible(Require<VisualElement>(id),false);
                buttons["result-next"].text=L("human.advance");buttons["result-next"].SetEnabled(pending==null&&state.CanAdvance&&!externalUiSuspended);
                buttons["puzzle-back"].SetEnabled(CanOpenHumanMenu);buttons["puzzle-undo"].SetEnabled(editing&&view.CanUndo);buttons["puzzle-hint"].SetEnabled(editing&&hintPreparation==null&&!hintHelpOpen);
                buttons["puzzle-zoom"].SetEnabled(view.Phase==DevelopmentPreviewPhase.Editing&&!externalUiSuspended);
                for(var i=0;i<tools.Length;i++){tools[i].SetEnabled(editing);tools[i].EnableInClassList("tool--active",i==(int)activeTool);}
                RenderExposure(state);
            }
            SetVisible(Require<VisualElement>("tool-dock"),reveal&&state.FirstContactRecorded);
            SetVisible(Require<VisualElement>("human-contact"),reveal&&!state.FirstContactRecorded);
            RenderFirstContact(state);RenderHumanMenu();
            RenderNextParticipant();
            SetVisible(buttons["human-begin"],!blocked&&state.ObservationStatus==HumanObservationStatus.NeedsRun);
            SetVisible(buttons["human-show"],!blocked&&state.ObservationStatus==HumanObservationStatus.NeedsExposure);
            SetVisible(buttons["human-advance"],!reveal&&!blocked&&state.CanAdvance);
            SetVisible(buttons["human-retry"],blocked&&state.ObservationStatus!=HumanObservationStatus.Unsupported);
            SetVisible(buttons["human-export"],exportHuman!=null&&state.HasRun&&!reveal);
            SetVisible(buttons["human-next-participant"],!blocked&&state.IsFinished);
            foreach(var id in new[]{"human-begin","human-show","human-advance","human-retry"})buttons[id].SetEnabled(pending==null&&humanExportTask==null&&!externalUiSuspended);
            buttons["human-export"].SetEnabled(pending==null&&humanExportTask==null&&!externalUiSuspended);
            buttons["human-next-participant"].SetEnabled(CanBeginNextParticipant);
            Require<Label>("human-status").text=L(HumanStatusKey(state));
            if(blocked)SetVisible(Require<VisualElement>("hint-overlay"),false);
            else
            {
                RenderHint();
                if(hintHelpOpen&&hintReceipt==null&&!hintCommitFailed)
                    Require<Label>("hint-save-state").text=L(hintOffer!=null?"human.hint.pending":"human.hint.unprepared");
            }
            renderedState=state.Version;resultSheet.Refresh();
        }

        private string HumanStatusKey(AssignedHumanState state)
        {
            if(requestFault)return "human.status.failed";
            if(state.Trial?.Outcome==HumanTrialOutcome.ABORTED)return "human.status.aborted";
            switch(state.ObservationStatus)
            {
                case HumanObservationStatus.NeedsRun:return "human.status.needs-run";
                case HumanObservationStatus.NeedsExposure:return "human.status.needs-exposure";
                case HumanObservationStatus.RecoveryRequired:return "human.status.recovery";
                case HumanObservationStatus.Unsupported:return "human.status.unsupported";
                case HumanObservationStatus.Complete:return "human.status.complete";
                case HumanObservationStatus.Saving:return "human.status.saving";
                default:return "human.status.loading";
            }
        }
        private void RenderExposure(AssignedHumanState state)
        {
            var presentation=state.Exposure;
            if(Grid==null||presentation==null||state.Trial?.Exposure!=HumanExposureState.EXPOSURE_PENDING||exposureAcknowledged==presentation.ExposureId||exposureArmed==presentation.ExposureId)return;
            selectedContact=null;priorVersion=false;
            exposureGate?.Dispose();var attempt=view!.AttemptId;exposureArmed=presentation.ExposureId;
            exposureGate=new ProductionExposureGate(Require<VisualElement>("puzzle-screen"),Grid.Board,()=>!disposed&&!externalUiSuspended&&!HumanBlocked&&pending==null&&assignedReview?.State.Exposure?.ExposureId==presentation.ExposureId&&view?.AttemptId==attempt);
            exposureGate.Arm(()=>
            {
                if(disposed||HumanBlocked||view?.AttemptId!=attempt)return;
                var receipt=presentation.AcknowledgeVisible(clock());exposureReceipt=receipt;exposureAcknowledged=presentation.ExposureId;
                RunHuman(()=>assignedReview!.ConfirmExposureVisibleAsync(receipt,CancellationToken.None));
            });
        }
        private void SelectContact(HumanFirstContact contact){if(!CanAnswerContact)return;selectedContact=contact;Refresh();}
        private async Task RetryHumanObservation()
        {
            var session=assignedReview!;await session.RetryAsync(CancellationToken.None);
            var receipt=exposureReceipt;var state=session.State;
            if(receipt!=null&&state.ObservationStatus!=HumanObservationStatus.RecoveryRequired&&state.Exposure?.ExposureId==receipt.Presentation.ExposureId&&session.AttemptId==receipt.Presentation.AttemptId&&state.Trial?.Exposure==HumanExposureState.EXPOSURE_PENDING)
                await session.ConfirmExposureVisibleAsync(receipt,CancellationToken.None);
        }
        private void RenderFirstContact(AssignedHumanState state)
        {
            Require<Label>("human-prior-exposure").text=L(state.PriorExposure?"human.contact.prior":"human.contact.body");
            var ids=new[]{"human-never","human-seen","human-unsure"};var values=new[]{HumanFirstContact.NEVER_SEEN,HumanFirstContact.SEEN_OR_PLAYED,HumanFirstContact.UNSURE};
            for(var i=0;i<ids.Length;i++){buttons[ids[i]].SetEnabled(CanAnswerContact);buttons[ids[i]].EnableInClassList("human-contact-option--selected",selectedContact==values[i]);}
            buttons["human-prior-version"].text=L(priorVersion?"human.contact.prior-version.on":"human.contact.prior-version.off");buttons["human-prior-version"].SetEnabled(CanAnswerContact);
            buttons["human-contact-confirm"].SetEnabled(CanAnswerContact&&selectedContact.HasValue);
            buttons["human-contact-confirm"].text=L(pending!=null?"human.contact.saving":"human.contact.confirm");
        }
        private void ConfirmContact()
        {if(!CanAnswerContact||!selectedContact.HasValue)return;var contact=selectedContact.Value;var previous=priorVersion;RunHuman(()=>assignedReview!.RecordFirstContactAsync(contact,previous,CancellationToken.None));}
        private void BackFromPuzzle(){if(assignedReview==null){Navigate(Screen.Home);return;}if(CanOpenHumanMenu){humanMenuOpen=true;Refresh();}}
        private void BackFromResult(){if(assignedReview==null)Navigate(Screen.Home);}
        private void CloseHumanMenu(){if(pending!=null)return;humanMenuOpen=false;humanAbortConfirmation=false;Refresh();}
        private void RenderHumanMenu()
        {
            SetVisible(Require<VisualElement>("human-menu-overlay"),humanMenuOpen&&!HumanBlocked);
            Require<Label>("human-menu-body").text=L(humanAbortConfirmation?"human.menu.abort.body":"human.menu.body");
            foreach(var id in new[]{"human-resume","human-restart","human-abort"})SetVisible(buttons[id],!humanAbortConfirmation);
            SetVisible(buttons["human-abort-confirm"],humanAbortConfirmation);SetVisible(buttons["human-abort-cancel"],humanAbortConfirmation);
            buttons["human-resume"].SetEnabled(pending==null);buttons["human-restart"].SetEnabled(pending==null&&assignedReview!.State.CanRestart);
            buttons["human-abort"].SetEnabled(pending==null&&assignedReview!.State.CanAbort);buttons["human-abort-confirm"].SetEnabled(pending==null&&assignedReview!.State.CanAbort);buttons["human-abort-cancel"].SetEnabled(pending==null);
        }
        private void RestartHuman()
        {if(!humanMenuOpen||pending!=null||assignedReview?.State.CanRestart!=true)return;RunHuman(()=>assignedReview.RestartAsync(CancellationToken.None));humanMenuOpen=false;humanAbortConfirmation=false;Refresh();}
        private void AbortHuman()
        {if(!humanMenuOpen||!humanAbortConfirmation||pending!=null||assignedReview?.State.CanAbort!=true)return;RunHuman(()=>assignedReview.AbortAsync(CancellationToken.None));humanMenuOpen=false;humanAbortConfirmation=false;Refresh();}
        private void AdvanceAssignedReview()
        {
            if(assignedReview==null||HumanBlocked||pending!=null||externalUiSuspended||!assignedReview.State.CanAdvance)return;
            selectedContact=null;priorVersion=false;humanMenuOpen=false;humanAbortConfirmation=false;
            RunHuman(()=>assignedReview.NextAsync(CancellationToken.None));
        }
        private void RunHuman(Func<Task> action,bool recovery=false)
        {
            if(disposed||assignedReview==null||pending!=null||humanExportTask!=null||externalUiSuspended||!recovery&&HumanBlocked)return;
            if(recovery){requestFault=false;requestCancelled=false;}
            try{pending=action()??throw new InvalidOperationException("HUMAN_ACTION_NULL");}catch{requestFault=true;pending=null;}Refresh();
        }
        private void ExportHuman()
        {
            if(disposed||exportHuman==null||pending!=null||humanExportTask!=null||externalUiSuspended)return;
            try{humanExportTask=exportHuman()??throw new InvalidOperationException("HUMAN_EXPORT_NULL");Require<Label>("human-export-state").text=L("human.export.pending");}
            catch{Require<Label>("human-export-state").text=L("human.export.failed");}Refresh();
        }
        private bool CanBeginNextParticipant=>!disposed&&!HumanBlocked&&pending==null&&humanExportTask==null&&!externalUiSuspended&&assignedReview!.State.CanBeginNextParticipant;
        private void OpenNextParticipant(){if(!CanBeginNextParticipant)return;if(showAssignedOperator!=null){showAssignedOperator();return;}humanNextParticipantOpen=true;humanNextParticipantRequested=false;Refresh();}
        private void CancelNextParticipant(){if(!humanNextParticipantOpen||pending!=null||humanNextParticipantRequested)return;humanNextParticipantOpen=false;Refresh();}
        private void ConfirmNextParticipant()
        {
            if(!humanNextParticipantOpen||humanNextParticipantRequested||!CanBeginNextParticipant)return;
            humanNextParticipantRequested=true;RunHuman(()=>assignedReview!.BeginNextParticipantAsync(CancellationToken.None));
        }
        private void RenderNextParticipant()
        {
            SetVisible(Require<VisualElement>("human-participant-overlay"),humanNextParticipantOpen&&!HumanBlocked);
            Require<Label>("human-next-participant-body").text=L(humanNextParticipantRequested?"human.next-participant.pending":"human.next-participant.body");
            buttons["human-next-participant-confirm"].SetEnabled(!humanNextParticipantRequested&&CanBeginNextParticipant);
            buttons["human-next-participant-cancel"].SetEnabled(!humanNextParticipantRequested&&pending==null);
        }
        private void ResetHumanPresentationForNewRun()
        {
            exposureGate?.Cancel();exposureArmed=Guid.Empty;exposureAcknowledged=Guid.Empty;exposureReceipt=null;
            selectedContact=null;priorVersion=false;humanMenuOpen=false;humanAbortConfirmation=false;
            hintDisplayGate.Cancel();hintOffer=null;hintReceipt=null;hintView=null;hintAttempt=Guid.Empty;hintRevision=0;
            hintHelpOpen=false;hintAcknowledged=false;hintCommitFailed=false;Require<Label>("human-export-state").text=string.Empty;
        }
        private void DisposeAssignedReview(){exposureGate?.Dispose();ObserveLateFault(humanExportTask);}
    }
}
