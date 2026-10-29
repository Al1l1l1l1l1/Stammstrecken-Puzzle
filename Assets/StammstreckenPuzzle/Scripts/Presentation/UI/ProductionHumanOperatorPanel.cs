using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Display states only. Root retains the actual ADR045 authority receipt and all persistence.</summary>
    public enum HumanOperatorPhase { Loading, Missing, Active, Complete, Recovery, Unsupported }

    /// <summary>A neutral six-input pack label, without a public puzzle or preview field.</summary>
    public sealed class HumanOperatorPack
    {
        public HumanOperatorPack(string id,int assignmentCount)
        {
            if(!TryNumber(id,out var campaign,out var number)||assignmentCount!=6)throw new ArgumentException("HUMAN_OPERATOR_PACK_INVALID");
            Id=id;IsCampaign=campaign;Number=number;AssignmentCount=assignmentCount;
        }
        public string Id{get;}
        public bool IsCampaign{get;}
        public int Number{get;}
        public int AssignmentCount{get;}
        internal static bool Known(string? id)=>TryNumber(id,out _,out _);
        private static bool TryNumber(string? id,out bool campaign,out int number)
        {
            campaign=false;number=0;if(id==null)return false;
            for(var group=0;group<2;group++)for(var i=1;i<=(group==0?9:8);i++)
                if(id==(group==0?"campaign":"endless")+"-pack-"+i.ToString("00",CultureInfo.InvariantCulture)+".json")
                {campaign=group==0;number=i;return true;}
            return false;
        }
    }

    /// <summary>A transient Root projection. AuthorityKey is never displayed, persisted or treated as participant identity.</summary>
    public sealed class HumanOperatorView
    {
        public HumanOperatorView(HumanOperatorPhase phase,string authorityKey,string? activePackId=null,bool busy=false,bool canExport=false)
        {
            if(!Enum.IsDefined(typeof(HumanOperatorPhase),phase)||authorityKey==null||authorityKey.Length>512||
                ((phase==HumanOperatorPhase.Missing||phase==HumanOperatorPhase.Active||phase==HumanOperatorPhase.Complete)&&string.IsNullOrWhiteSpace(authorityKey))||
                ((phase==HumanOperatorPhase.Active||phase==HumanOperatorPhase.Complete)&&!HumanOperatorPack.Known(activePackId))||
                ((phase==HumanOperatorPhase.Loading||phase==HumanOperatorPhase.Missing)&&activePackId!=null)||
                (activePackId!=null&&!HumanOperatorPack.Known(activePackId))||
                (canExport&&phase!=HumanOperatorPhase.Active&&phase!=HumanOperatorPhase.Complete))throw new ArgumentException("HUMAN_OPERATOR_VIEW_INVALID");
            Phase=phase;AuthorityKey=authorityKey;ActivePackId=activePackId;Busy=busy;CanExport=canExport;
        }
        public HumanOperatorPhase Phase{get;}
        public string AuthorityKey{get;}
        public string? ActivePackId{get;}
        public bool Busy{get;}
        public bool CanExport{get;}
    }

    /// <summary>Neutral operator composition before any gameplay presenter, or above a suspended completed binder.</summary>
    public sealed class ProductionHumanOperatorPanel:IDisposable
    {
        private enum Operation { Start, Resume, Retry, Export }
        private readonly int owner=Thread.CurrentThread.ManagedThreadId;
        private readonly VisualElement root,view,confirmation;
        private readonly ScrollView packsScroll;
        private readonly ProductionGameLocalization locale;
        private readonly Func<HumanOperatorView> read;
        private readonly Func<string,string,Task> start;
        private readonly Func<Task> resume,retry;
        private readonly Func<Task>? export;
        private readonly Dictionary<string,HumanOperatorPack> packs=new Dictionary<string,HumanOperatorPack>(StringComparer.Ordinal);
        private readonly Dictionary<string,Button> choices=new Dictionary<string,Button>(StringComparer.Ordinal);
        private readonly Dictionary<Button,Action> handlers=new Dictionary<Button,Action>();
        private readonly IVisualElementScheduledItem scheduled;
        private Task? pending;
        private Operation operation;
        private bool invoking,disposed,external,confirmOpen,faulted,changed,exportFailed;
        private string? selected,confirmedKey;
        private HumanOperatorPhase confirmedPhase;

        /// <summary>Callbacks are explicit user intents. Only Root may resolve AuthorityKey to the actual inspected receipt.</summary>
        public ProductionHumanOperatorPanel(VisualElement root,TextAsset translations,IReadOnlyList<HumanOperatorPack> options,
            Func<HumanOperatorView> read,Func<string,string,Task> start,Func<Task> resume,Func<Task> retry,Func<Task>? export=null,bool publishNative=true)
        {
            this.root=root??throw new ArgumentNullException(nameof(root));this.read=read??throw new ArgumentNullException(nameof(read));
            this.start=start??throw new ArgumentNullException(nameof(start));this.resume=resume??throw new ArgumentNullException(nameof(resume));this.retry=retry??throw new ArgumentNullException(nameof(retry));this.export=export;
            view=Require<VisualElement>("human-operator-overlay");confirmation=Require<VisualElement>("human-operator-confirm");packsScroll=Require<ScrollView>("human-operator-packs");
            if(options==null||options.Count!=17)throw new ArgumentException("HUMAN_OPERATOR_PACKSET_INVALID");
            foreach(var item in options)if(item==null||packs.ContainsKey(item.Id))throw new ArgumentException("HUMAN_OPERATOR_PACKSET_INVALID");else packs.Add(item.Id,item);
            for(var group=0;group<2;group++)for(var i=1;i<=(group==0?9:8);i++)
                if(!packs.ContainsKey((group==0?"campaign":"endless")+"-pack-"+i.ToString("00",CultureInfo.InvariantCulture)+".json"))throw new ArgumentException("HUMAN_OPERATOR_PACKSET_INVALID");
            HideOtherViews();locale=new ProductionGameLocalization(translations);
            try
            {
            foreach(var label in view.Query<Label>().ToList())if(!string.IsNullOrEmpty(label.viewDataKey))label.text=L(label.viewDataKey);
            packsScroll.contentContainer.Clear();
            for(var group=0;group<2;group++)for(var i=1;i<=(group==0?9:8);i++)
            {
                var id=(group==0?"campaign":"endless")+"-pack-"+i.ToString("00",CultureInfo.InvariantCulture)+".json";
                var option=packs[id];var button=new Button{name="human-operator-pack-"+id,text=PackName(option)};button.AddToClassList("human-operator-pack");
                Wire(button,()=>Select(id));packsScroll.Add(button);choices.Add(id,button);
            }
            Wire(Require<Button>("human-operator-choose"),OpenConfirmation);Wire(Require<Button>("human-operator-reset"),ResetSelection);
            Wire(Require<Button>("human-operator-confirm-start"),Confirm);Wire(Require<Button>("human-operator-confirm-cancel"),CancelConfirmation);
            Wire(Require<Button>("human-operator-resume"),Resume);Wire(Require<Button>("human-operator-retry"),Retry);Wire(Require<Button>("human-operator-export"),Export);
            view.RegisterCallback<NavigationCancelEvent>(OnCancel);
            Accessibility=new ProductionGameAccessibility(view,locale.Resolve,publishNative);
            scheduled=view.schedule.Execute(Refresh).Every(50);Refresh();
            }
            catch
            {
                scheduled?.Pause();view.UnregisterCallback<NavigationCancelEvent>(OnCancel);foreach(var pair in handlers)pair.Key.clicked-=pair.Value;
                Accessibility?.Dispose();locale.Dispose();throw;
            }
        }
        public ProductionGameAccessibility Accessibility{get;}
        public Task PendingOperation=>pending??Task.CompletedTask;
        public bool HasPendingOperation=>invoking||pending!=null;
        public bool IsConfirmationOpen=>confirmOpen;
        public string? SelectedPackId=>selected;
        public bool HasRequestFault=>faulted;
        public void SetExternalSuspended(bool value)
        {CheckThread();if(disposed)return;external=value;Accessibility.SetSuspended(value);Refresh();}
        /// <summary>Read-only presentation polling. It never starts, resumes, retries or exports automatically.</summary>
        public void Refresh()
        {
            CheckThread();if(disposed)return;HideOtherViews();
            if(pending!=null&&pending.IsCompleted)
            {
                var finished=pending;pending=null;confirmOpen=false;selected=null;confirmedKey=null;
                try{finished.GetAwaiter().GetResult();if(operation==Operation.Retry)faulted=false;}
                catch{if(operation==Operation.Export)exportFailed=true;else faulted=true;}
            }
            HumanOperatorView state;
            try{state=read()??throw new InvalidOperationException("HUMAN_OPERATOR_VIEW_MISSING");}
            catch{faulted=true;state=new HumanOperatorView(HumanOperatorPhase.Recovery,string.Empty);}
            if(confirmOpen&&(state.AuthorityKey!=confirmedKey||state.Phase!=confirmedPhase)){confirmOpen=false;selected=null;confirmedKey=null;changed=true;}
            if(!Selectable(state)){selected=null;confirmOpen=false;confirmedKey=null;}
            bool idle=Idle(state),canChoose=idle&&!faulted&&Selectable(state);
            foreach(var pair in choices)
            {pair.Value.SetEnabled(canChoose&&!confirmOpen);pair.Value.EnableInClassList("settings-choice--active",pair.Key==selected);}
            Visible(packsScroll,Selectable(state));packsScroll.SetEnabled(!confirmOpen);
            Require<Label>("human-operator-status").text=L(HasPendingOperation||state.Busy?"human.operator.pending":faulted?"human.operator.failed":PhaseKey(state.Phase));
            Require<Label>("human-operator-selection").text=selected==null?L("human.operator.no-selection"):L("human.operator.selection",PackName(packs[selected]));
            var active=Require<Label>("human-operator-active");active.text=state.ActivePackId==null?string.Empty:L("human.operator.active-pack",PackName(packs[state.ActivePackId]));Visible(active,state.ActivePackId!=null);
            var notice=Require<Label>("human-operator-notice");notice.text=changed?L("human.operator.changed"):exportFailed?L("human.operator.export-failed"):string.Empty;Visible(notice,changed||exportFailed);
            var choose=Require<Button>("human-operator-choose");choose.text=L("human.operator.choose");Visible(choose,Selectable(state));choose.SetEnabled(canChoose&&selected!=null&&!confirmOpen);
            var reset=Require<Button>("human-operator-reset");reset.text=L("human.operator.reset");Visible(reset,selected!=null&&!confirmOpen);reset.SetEnabled(idle);
            var continuation=Require<Button>("human-operator-resume");continuation.text=L(state.Phase==HumanOperatorPhase.Complete?"human.operator.inspect-complete":"human.operator.resume");Visible(continuation,state.Phase==HumanOperatorPhase.Active||state.Phase==HumanOperatorPhase.Complete);continuation.SetEnabled(idle&&!faulted&&!confirmOpen);
            var recheck=Require<Button>("human-operator-retry");recheck.text=L("human.operator.retry");Visible(recheck,faulted||state.Phase==HumanOperatorPhase.Recovery||state.Phase==HumanOperatorPhase.Unsupported);recheck.SetEnabled(idle&&!confirmOpen);
            var exportButton=Require<Button>("human-operator-export");exportButton.text=L("human.export");Visible(exportButton,state.CanExport&&export!=null);exportButton.SetEnabled(idle&&!faulted&&!confirmOpen);
            Visible(confirmation,confirmOpen);
            if(confirmOpen&&selected!=null)Require<Label>("human-operator-confirm-body").text=L(confirmedPhase==HumanOperatorPhase.Missing?"human.operator.confirm-first":"human.operator.confirm-next",PackName(packs[selected]));
            var confirm=Require<Button>("human-operator-confirm-start");confirm.text=L(HasPendingOperation?"human.operator.pending":"human.operator.start");confirm.SetEnabled(canChoose&&confirmOpen);
            var cancel=Require<Button>("human-operator-confirm-cancel");cancel.text=L("cancel");cancel.SetEnabled(idle&&confirmOpen);
        }
        public void Dispose()
        {
            CheckThread();if(disposed)return;disposed=true;scheduled.Pause();ObserveLate(pending);pending=null;
            view.UnregisterCallback<NavigationCancelEvent>(OnCancel);foreach(var pair in handlers)pair.Key.clicked-=pair.Value;handlers.Clear();
            Accessibility.Dispose();locale.Dispose();Visible(view,false);
        }
        private bool Idle(HumanOperatorView state)=>!external&&!invoking&&pending==null&&!state.Busy;
        private static bool Selectable(HumanOperatorView state)=>state.Phase==HumanOperatorPhase.Missing||state.Phase==HumanOperatorPhase.Complete;
        private void Select(string id)
        {CheckThread();if(disposed)return;var state=Current();if(!Idle(state)||faulted||confirmOpen||!Selectable(state))return;selected=id;changed=false;exportFailed=false;Refresh();}
        private void ResetSelection(){CheckThread();if(disposed)return;var state=Current();if(!Idle(state)||confirmOpen)return;selected=null;Refresh();}
        private void OpenConfirmation()
        {
            CheckThread();if(disposed)return;var state=Current();if(!Idle(state)||faulted||!Selectable(state)||selected==null||confirmOpen)return;
            confirmedKey=state.AuthorityKey;confirmedPhase=state.Phase;confirmOpen=true;Refresh();Require<Button>("human-operator-confirm-start").Focus();
        }
        private void CancelConfirmation()
        {CheckThread();if(disposed||!confirmOpen||!Idle(Current()))return;confirmOpen=false;confirmedKey=null;Refresh();if(selected!=null)choices[selected].Focus();}
        private void Confirm()
        {
            CheckThread();if(disposed||!confirmOpen||selected==null)return;var state=Current();
            if(!Idle(state)||faulted)return;
            if(!Selectable(state)||state.AuthorityKey!=confirmedKey||state.Phase!=confirmedPhase){confirmOpen=false;selected=null;confirmedKey=null;changed=true;Refresh();return;}
            var id=selected;var key=confirmedKey!;Run(Operation.Start,()=>start(id,key));
        }
        private void Resume(){CheckThread();if(disposed)return;var state=Current();if(Idle(state)&&!faulted&&!confirmOpen&&(state.Phase==HumanOperatorPhase.Active||state.Phase==HumanOperatorPhase.Complete))Run(Operation.Resume,resume);}
        private void Retry(){CheckThread();if(disposed)return;var state=Current();if(Idle(state)&&!confirmOpen&&(faulted||state.Phase==HumanOperatorPhase.Recovery||state.Phase==HumanOperatorPhase.Unsupported))Run(Operation.Retry,retry);}
        private void Export(){CheckThread();if(disposed)return;var state=Current();if(Idle(state)&&!faulted&&!confirmOpen&&state.CanExport&&export!=null)Run(Operation.Export,export);}
        private void Run(Operation kind,Func<Task> action)
        {
            invoking=true;operation=kind;exportFailed=false;
            try{var task=action()??throw new InvalidOperationException("HUMAN_OPERATOR_TASK_MISSING");if(disposed)ObserveLate(task);else pending=task;}
            catch{if(kind==Operation.Export)exportFailed=true;else faulted=true;confirmOpen=false;selected=null;confirmedKey=null;}
            finally{invoking=false;}
            if(!disposed)Refresh();
        }
        private HumanOperatorView Current()
        {try{return read()??throw new InvalidOperationException("HUMAN_OPERATOR_VIEW_MISSING");}catch{faulted=true;return new HumanOperatorView(HumanOperatorPhase.Recovery,string.Empty);}}
        private void OnCancel(NavigationCancelEvent e){if(confirmOpen){CancelConfirmation();e.StopPropagation();}}
        private void HideOtherViews()
        {var ownerRoot=root.name=="production-root"?root:root.Q<VisualElement>("production-root")??throw new ArgumentException("HUMAN_OPERATOR_ROOT_MISSING");foreach(var child in ownerRoot.Children())if(child!=view)Visible(child,false);Visible(view,true);}
        private T Require<T>(string name)where T:VisualElement=>root.Q<T>(name)??throw new ArgumentException("HUMAN_OPERATOR_ELEMENT_MISSING:"+name);
        private void Wire(Button button,Action action){button.clicked+=action;handlers.Add(button,action);}
        private string PackName(HumanOperatorPack item)=>L(item.IsCampaign?"human.operator.campaign":"human.operator.endless",item.Number);
        private string L(string key,params object[] values)=>locale.Resolve(key,values);
        private static void Visible(VisualElement element,bool value){element.EnableInClassList("hidden",!value);element.style.display=value?DisplayStyle.Flex:DisplayStyle.None;}
        private static string PhaseKey(HumanOperatorPhase phase)=>phase switch
        {HumanOperatorPhase.Loading=>"human.operator.loading",HumanOperatorPhase.Missing=>"human.operator.missing",HumanOperatorPhase.Active=>"human.operator.active",HumanOperatorPhase.Complete=>"human.operator.complete",HumanOperatorPhase.Recovery=>"human.operator.recovery",_=>"human.operator.unsupported"};
        private void CheckThread(){if(Thread.CurrentThread.ManagedThreadId!=owner)throw new InvalidOperationException("HUMAN_OPERATOR_WRONG_THREAD");}
        private static void ObserveLate(Task? task){if(task!=null)task.ContinueWith(t=>{var ignored=t.Exception;},CancellationToken.None,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);}
    }
}
