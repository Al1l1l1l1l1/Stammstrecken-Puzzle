using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Four-profile postgame selection, exact open-record actions and explicitly requested completion bonuses.</summary>
    public sealed class ProductionPostgamePanels:IDisposable
    {
        private readonly VisualElement root;
        private readonly ProductionGameLocalization locale;
        private readonly RailwayIllustrationElement art;
        private readonly Dictionary<Button,Action> fixedActions=new Dictionary<Button,Action>();
        private readonly Dictionary<string,Button> profileButtons=new Dictionary<string,Button>(StringComparer.Ordinal);
        private readonly Dictionary<string,(EndlessEntryView state,Button resume,Button abandon,Label phase)> cards=new Dictionary<string,(EndlessEntryView,Button,Button,Label)>(StringComparer.Ordinal);
        private readonly List<(Button button,Action action)> cardActions=new List<(Button,Action)>();
        private readonly Dictionary<string,(Button video,Button skip)> completionCards=new Dictionary<string,(Button,Button)>(StringComparer.Ordinal);
        private Func<EndlessView>? readEndless;
        private Func<string,Task>? startProfile,resumeId,abandonId;
        private Func<string,bool>? canRewardOpen,canSkipRewardOpen;
        private Func<string,bool,Task>? rewardOpen;
        private Func<CompletionBonusView>? readBonus;
        private Func<bool,Task>? requestBonus;
        private Task? endlessTask,bonusTask;
        private bool open,failed,bonusFailed,disposed;
        private string? pendingAbandon;
        private string renderedRecords=string.Empty;
        /// <summary>Creates panels, without querying a clock, generator or provider.</summary>
        public ProductionPostgamePanels(VisualElement root,ProductionGameLocalization locale,Action revision)
        {
            this.root=root;this.locale=locale;art=new RailwayIllustrationElement();Require<VisualElement>("endless-art").Add(art);Add("home-endless","endless.open",()=>{open=true;Refresh();});Add("home-revision","revision.open",revision);
            Add("endless-close","notice.close",()=>{if(endlessTask==null){open=false;pendingAbandon=null;Refresh();}});
            Add("endless-abandon-confirm","endless.abandon.confirm",()=>{var id=pendingAbandon;if(id!=null&&abandonId!=null){pendingAbandon=null;Run(()=>abandonId(id));}});
            Add("endless-abandon-cancel","cancel",()=>{pendingAbandon=null;Refresh();});
            Add("result-video","completion.video",()=>Bonus(true));Add("result-skip-bonus","completion.skip",()=>Bonus(false));
            var host=Require<VisualElement>("endless-profiles");
            foreach(var id in new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"})
            {
                var captured=id;var button=new Button{name="endless-profile-"+id,text=L("endless.profile."+id)};button.AddToClassList("endless-profile-button");
                Action action=()=>{try{if(readEndless!=null&&startProfile!=null){var state=readEndless();if(state.Unlocked&&!state.Generating&&state.AvailableProfileIds.Contains(captured))Run(()=>startProfile(captured));}}catch{failed=true;Refresh();}};
                button.clicked+=action;fixedActions.Add(button,action);profileButtons.Add(id,button);host.Add(button);
            }
            Refresh();
        }
        /// <summary>Whether postgame owns the attention pause.</summary>
        public bool IsModalOpen=>open;
        /// <summary>Any actual UI-requested postgame or optional bonus operation is unresolved.</summary>
        public bool IsBusy=>endlessTask!=null||bonusTask!=null;
        /// <summary>Opens the profile selection without starting generation.</summary>
        public void Open(){open=true;Refresh();}
        /// <summary>Resolves the actual reusable map reference for generated puzzles, separate from all campaign references.</summary>
        public void BindScene(RailwayAssetCatalog catalog){var moment=catalog.ResolveReusableMoment("draft-endless-map-default");art.SetComposition(catalog.ResolveComposition(moment.compositionId),0);}
        /// <summary>Shows the actual equipped train, livery and objects in the postgame map composition.</summary>
        public void SetActiveCosmetics(IReadOnlyList<string> ids)=>art.SetActiveCosmetics(ids);
        /// <summary>Binds only Root-owned persisted generation/open/abandon operations.</summary>
        public void BindEndless(Func<EndlessView> read,Func<string,Task> start,Func<string,Task> resume,Func<string,Task> abandon)
        {readEndless=read;startProfile=start;resumeId=resume;abandonId=abandon;Refresh();}
        /// <summary>Binds explicit video/skip for an actual completion record; abandon never substitutes for skipping a claim.</summary>
        public void BindEndlessReward(Func<string,bool> canRequest,Func<string,bool,Task> request,Func<string,bool>? canSkip=null)
        {canRewardOpen=canRequest??throw new ArgumentNullException(nameof(canRequest));rewardOpen=request??throw new ArgumentNullException(nameof(request));canSkipRewardOpen=canSkip;renderedRecords=string.Empty;Refresh();}
        /// <summary>Binds the currently shown puzzle's actual optional +10 claim.</summary>
        public void BindCompletionBonus(Func<CompletionBonusView> read,Func<bool,Task> request){readBonus=read;requestBonus=request;Refresh();}
        /// <summary>Called by Root only after an actual saved puzzle has bound; it grants no session itself.</summary>
        public void CloseForPuzzle(){open=false;pendingAbandon=null;Refresh();}
        /// <summary>Refreshes real snapshots; completing a task never creates a reward or generated puzzle.</summary>
        public void Refresh()
        {
            if(disposed)return;Poll(ref endlessTask,ref failed);Poll(ref bonusTask,ref bonusFailed);Visible("endless-overlay",open);
            EndlessView? state=null;try{state=readEndless?.Invoke();}catch{failed=true;}
            var busy=endlessTask!=null||state?.Generating==true;
            Require<Button>("home-revision").SetEnabled(state?.Unlocked==true);Require<Button>("endless-close").SetEnabled(!busy);
            Require<Label>("endless-unlock").text=L(state?.Unlocked==true?"endless.unlocked":"endless.locked");
            Require<Label>("endless-status").text=L("endless.status."+(failed?"failed":busy?"generating":state?.StatusKey??"unavailable"));
            foreach(var pair in profileButtons)pair.Value.SetEnabled(!busy&&state!=null&&state.Unlocked&&state.AvailableProfileIds.Contains(pair.Key)&&startProfile!=null);
            var records=state?.OpenEntries??Array.Empty<EndlessEntryView>();
            var signature=string.Join("|",records.Select(e=>e.RecordId+":"+e.ProfileId+":"+e.Phase));
            if(signature!=renderedRecords){BuildRecords(records);renderedRecords=signature;}
            foreach(var record in records)if(cards.TryGetValue(record.RecordId,out var card))
            {card.resume.SetEnabled(!busy&&record.CanResume&&resumeId!=null);card.abandon.SetEnabled(!busy&&record.CanAbandon&&record.Phase!=EndlessEntryPhase.CompletionPending&&abandonId!=null);}
            foreach(var pair in completionCards)
            {
                var canSkip=CanSkip(pair.Key);pair.Value.video.SetEnabled(!busy&&rewardOpen!=null&&canSkip&&CanVideo(pair.Key));pair.Value.skip.SetEnabled(!busy&&rewardOpen!=null&&canSkip);
                if(cards.TryGetValue(pair.Key,out var card))card.phase.text=L(!canSkip?"endless.reward.pending":"endless.phase.CompletionPending");
            }
            Visible("endless-abandon-confirmation",pendingAbandon!=null);Visible("endless-no-open",records.Count==0);
            CompletionBonusView? bonus=null;try{bonus=readBonus?.Invoke();}catch{bonusFailed=true;}
            var bonusBusy=bonusTask!=null||bonus?.IsPending==true;
            Require<Button>("result-video").SetEnabled(!bonusBusy&&bonus?.CanRequestVideo==true&&requestBonus!=null);Visible("result-video",bonus?.CanRequestVideo==true||bonusBusy);
            Require<Button>("result-skip-bonus").SetEnabled(!bonusBusy&&bonus?.CanSkip==true&&requestBonus!=null);Visible("result-skip-bonus",bonus?.CanSkip==true);
            var showBonusState=readBonus!=null&&(bonus?.CanRequestVideo==true||bonus?.CanSkip==true||bonusBusy||bonus?.ConfirmedAmount.HasValue==true);
            Require<Label>("result-bonus-state").text=!showBonusState?string.Empty:bonus?.ConfirmedAmount is int amount?L("completion.confirmed",amount):L(bonusBusy?"completion.pending":bonusFailed?"completion.failed":"completion.optional");
            Visible("result-bonus-state",showBonusState);
        }
        /// <summary>Removes only owned UI handlers and observes late failures.</summary>
        public void Dispose(){if(disposed)return;disposed=true;foreach(var pair in fixedActions)pair.Key.clicked-=pair.Value;ClearCards();Observe(endlessTask);Observe(bonusTask);}
        private void BuildRecords(IReadOnlyList<EndlessEntryView> entries)
        {
            ClearCards();var host=Require<VisualElement>("endless-open-list");host.Clear();
            foreach(var entry in entries)
            {
                var id=entry.RecordId;var card=new VisualElement{name="endless-record-"+id};card.AddToClassList("endless-record");
                card.Add(new Label(L("endless.profile."+entry.ProfileId)));var phase=new Label(L("endless.phase."+entry.Phase));phase.AddToClassList("small-copy");card.Add(phase);
                var resume=CardButton(card,"endless.resume",()=>{if(resumeId!=null)Run(()=>resumeId(id));});
                var abandon=CardButton(card,"endless.abandon",()=>{pendingAbandon=id;Refresh();});resume.name="endless-resume-"+id;abandon.name="endless-abandon-"+id;
                if(entry.Phase==EndlessEntryPhase.CompletionPending)
                {
                    resume.style.display=DisplayStyle.None;abandon.style.display=DisplayStyle.None;
                    var video=CardButton(card,"completion.video",()=>{if(rewardOpen!=null&&CanSkip(id)&&CanVideo(id))Run(()=>rewardOpen(id,true));});
                    var skip=CardButton(card,"completion.skip",()=>{if(rewardOpen!=null&&CanSkip(id))Run(()=>rewardOpen(id,false));});video.name="endless-video-"+id;skip.name="endless-skip-"+id;completionCards.Add(id,(video,skip));
                }
                cards.Add(id,(entry,resume,abandon,phase));host.Add(card);
            }
        }
        private void Bonus(bool video)
        {if(disposed||bonusTask!=null||requestBonus==null||readBonus==null)return;try{var state=readBonus();if(state.IsPending||(video?!state.CanRequestVideo:!state.CanSkip))return;bonusFailed=false;bonusTask=requestBonus(video)??throw new InvalidOperationException("COMPLETION_TASK_NULL");}catch{bonusFailed=true;}Refresh();}
        private void Run(Func<Task> action){if(disposed||endlessTask!=null)return;failed=false;try{endlessTask=action()??throw new InvalidOperationException("ENDLESS_TASK_NULL");}catch{failed=true;}Refresh();}
        private bool CanSkip(string id){try{return canSkipRewardOpen?.Invoke(id)??true;}catch{return false;}}
        private bool CanVideo(string id){try{return canRewardOpen?.Invoke(id)==true;}catch{return false;}}
        private void ClearCards(){foreach(var pair in cardActions)pair.button.clicked-=pair.action;cardActions.Clear();cards.Clear();completionCards.Clear();}
        private Button CardButton(VisualElement host,string key,Action action){var button=new Button{text=L(key)};button.AddToClassList("secondary-button");button.clicked+=action;cardActions.Add((button,action));host.Add(button);return button;}
        private static void Poll(ref Task? task,ref bool failed){if(task==null||!task.IsCompleted)return;var completed=task;task=null;try{completed.GetAwaiter().GetResult();}catch{failed=true;}}
        private static void Observe(Task? task){if(task!=null)_=task.ContinueWith(t=>{_ = t.Exception;},CancellationToken.None,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);}
        private string L(string key,params object[] values)=>locale.Resolve(key,values);
        private T Require<T>(string id)where T:VisualElement=>root.Q<T>(id)??throw new InvalidOperationException("POSTGAME_ELEMENT_MISSING:"+id);
        private void Visible(string id,bool value)=>Require<VisualElement>(id).style.display=value?DisplayStyle.Flex:DisplayStyle.None;
        private void Add(string id,string key,Action action){var button=Require<Button>(id);button.text=L(key);button.clicked+=action;fixedActions.Add(button,action);}
    }
}
