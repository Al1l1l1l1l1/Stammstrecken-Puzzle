using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    public sealed partial class ProductionGamePresenter
    {
        private StpWorldCamera? worldCamera;
        private StpPresentationMemory? memory;
        private StationProgramme? programme;
        private Action<string>? sound;
        private Action? skipRide;
        private Button? focusedCell;
        private Screen puzzleOrigin=Screen.Home;
        private string lastScene="", gagLine="";
        private bool mailOpen, rareVisit, depotObjects, closedMemory, reducedMotion;
        private int appearanceVersion, depotIndex, ticketIndex=-1, totalsStars, threeStars, revisions;
        private int frameAppearance=-1;
        private string frameScene="";
        private int lastLoading=-1;
        private long lastGeneration=-2;
        private Guid resultAttempt, finaleShown;
        private bool finaleLetters;
        private string? secondLetter;
        private float sceneOpened;
        private string chosenGag="";
        private int lastFlap=-1;
        private bool revisionDesk;
        private readonly List<string> stationProps=new List<string>();
        private string[] previewEquipment=Array.Empty<string>();
        private string[] previewProps=Array.Empty<string>();
        private readonly List<string> depotIds=new List<string>();
        private readonly Dictionary<string,string> panelWords=new Dictionary<string,string>();
        private float swipeStart;
        private bool swipeActive;
        private int? swipePointer;
        private StpVehicle drawnStock;
        private Guid observedAttempt;
        private long? previousBest;
        private long balanceAtStart;
        private int lastStep;
        private int lastMailCount=-1;
        private string? openLetter;

        public void BindStation(string directory,StationBulletins bulletins,StpWorldCamera? camera,Action skip,Action<string> cue)
        {
            worldCamera=camera;skipRide=skip;sound=cue;
            memory=new StpPresentationMemory(directory);
            programme=new StationProgramme(bulletins);
            var labels=Newtonsoft.Json.Linq.JObject.Parse(bulletins.labels.text);
            foreach(var label in labels.Properties())panelWords[label.Name]=label.Value.ToString();
            Root.AddToClassList("native-station");
            foreach(var id in new[]{"mail-open","home-map","home-daily","home-endless","home-revision","nav-depot","nav-profile","nav-home","puzzle-hint","puzzle-undo","puzzle-reset"}){
                var control=Require<Button>(id);control.Add(new StationControlIcon(id));control.AddToClassList("object-control");}
            Require<Button>("home-map").text=W("interlocking");
            Require<Button>("home-daily").text=W("daily");
            Require<Button>("home-endless").text=W("endless");
            Require<Button>("home-revision").text=W("revision");
            Require<Button>("nav-home").text=W("platform");
            Require<Button>("nav-profile").text=W("id-card");
            Require<Button>("puzzle-reset").text=W("replan");
            Require<Label>("proof-heading").text=W("certificate");
            RegisterPanelGags();
            void Wire(string id,Action callback){var button=Require<Button>(id);actions.Add(id,callback);buttons.Add(id,button);button.clicked+=callback;}
            Wire("mail-open",OpenMail);Wire("mail-close",CloseMail);
            Wire("map-dispatch",()=>{if(ticketIndex>=0&&CanOpen(ticketIndex))OpenPuzzle(ticketIndex);CloseTicket();});
            Wire("depot-prev",()=>SelectDepot(-1));Wire("depot-next",()=>SelectDepot(1));
            Wire("depot-group",()=>{depotObjects=!depotObjects;depotIndex=0;BuildDepotSelection();});
            Wire("result-share",ShareProof);
            buttons["result-share"].text=W("share-title");buttons["map-dispatch"].text=W("dispatch");
            buttons["mail-close"].text=L("notice.close");
            var art=Require<VisualElement>("depot-art");
            art.RegisterCallback<PointerDownEvent>(StartSwipe);
            art.RegisterCallback<PointerUpEvent>(FinishSwipe);
            art.RegisterCallback<PointerCancelEvent>(CancelSwipe);
            viewport.RegisterCallback<PointerDownEvent>(TapRide,TrickleDown.TrickleDown);
            SetVisible(Require<Button>("result-share"),UnityEngine.Application.platform==RuntimePlatform.Android && camera!=null);
            homeArt.DrawPreview=false;mapArt.DrawPreview=false;depotArt.DrawPreview=false;resultArt.DrawPreview=false;
            var endlessArt=Root.Q("endless-art")?.Q<RailwayIllustrationElement>();if(endlessArt!=null)endlessArt.DrawPreview=false;
            BuildDepotSelection();
        }
        private string W(string key)=>panelWords.TryGetValue(key,out var label)?label:"";
        public void DeletePresentationMemory(){closedMemory=true;memory?.Delete();}
        private void DisposeStation()
        {
            if(memory==null)return;
            var art=Require<VisualElement>("depot-art");
            art.UnregisterCallback<PointerDownEvent>(StartSwipe);art.UnregisterCallback<PointerUpEvent>(FinishSwipe);
            art.UnregisterCallback<PointerCancelEvent>(CancelSwipe);viewport.UnregisterCallback<PointerDownEvent>(TapRide,TrickleDown.TrickleDown);
        }
        private void TapRide(PointerDownEvent evt){if(view?.Phase==DevelopmentPreviewPhase.TrainRide&&!externalUiSuspended&&!IsProductModalOpen)skipRide?.Invoke();}
        private void StartSwipe(PointerDownEvent e){swipeStart=e.position.x;swipeActive=true;swipePointer=e.pointerId;((VisualElement)e.currentTarget).CapturePointer(e.pointerId);}
        private void CancelSwipe(PointerCancelEvent e){swipeActive=false;swipePointer=null;}
        private void FinishSwipe(PointerUpEvent e)
        {
            if(!swipeActive||swipePointer!=e.pointerId)return;
            swipeActive=false;swipePointer=null;((VisualElement)e.currentTarget).ReleasePointer(e.pointerId);
            float delta=e.position.x-swipeStart;if(Mathf.Abs(delta)>28)SelectDepot(delta<0?1:-1);
        }
        private void SelectDepot(int delta)
        {
            if(depotIds.Count==0)return;
            depotIndex=(depotIndex+delta+depotIds.Count)%depotIds.Count;appearanceVersion++;UpdateDepot();
        }
        private void BuildDepotSelection()
        {
            depotIds.Clear();
            if(assetCatalog!=null)foreach(var item in assetCatalog.CosmeticAssets)
                if(item.id.StartsWith("obj-",StringComparison.Ordinal)==depotObjects)depotIds.Add(item.id);
            depotIndex=0;appearanceVersion++;UpdateDepot();
        }
        private void UpdateDepot()
        {
            if(memory==null||depotIds.Count==0)return;
            var id=depotIds[depotIndex];
            foreach(var entry in cosmeticButtons)SetVisible(Require<VisualElement>("cosmetic-"+entry.Key),entry.Key==id);
            buttons["depot-group"].text=W(depotObjects?"train-yard":"improvements");
            Require<Label>("depot-current-name").text=L("cosmetic."+id+".title");
            Require<Label>("depot-current-body").text=programme!.Depot.TryGetValue(id,out var line)?line:L("depot.body");
        }
        private void CloseTicket(){ticketIndex=-1;SetVisible(Require<VisualElement>("map-ticket"),false);}
        private void SelectTicket(int index)
        {
            if(model==null)return;
            ticketIndex=index;var item=model.Puzzles[index];
            var text=campaignText?.Resolve(index,L("section."+index/48),L("route."+index/12));
            int stars=readCampaignRecord?.Invoke(index).Stars??0;
            Require<Label>("map-selection").text=$"{text?.Title}  ·  {item.Width}×{item.Height}  {new string('★',stars)}";
            SetVisible(Require<VisualElement>("map-ticket"),true);
        }

        private float mapPulseStart;
        private bool wasDaily;
        private void RegisterPanelGags()
        {
            var board=Require<Label>("daily-flaps");
            board.schedule.Execute(()=>{
                if(disposed||programme==null)return;
                bool open=Require<VisualElement>("daily-overlay").style.display.value==DisplayStyle.Flex;
                if(open&&!wasDaily){
                    board.text=W("daily");sound?.Invoke("Flap");
                    if(!reducedMotion)for(int i=0;i<4;i++){
                        int flip=i;board.schedule.Execute(()=>{
                            if(disposed||!wasDaily)return;
                            var lines=programme.Voice.boardMessagesRare;
                            board.text=flip<3&&lines.Length>0?lines[flip%lines.Length]:W("daily");
                            sound?.Invoke("Flap");
                        }).StartingIn((i+1)*140);
                    }
                }
                wasDaily=open;
            }).Every(170);
        }
        private void UpdateLoading()
        {
            if(StartupPhase!=SavedStartupPhase.Loading||programme==null||programme.Voice.loading.Length==0)return;
            int slot=Mathf.FloorToInt(Time.unscaledTime/2.5f)%programme.Voice.loading.Length;
            if(slot!=lastLoading){lastLoading=slot;Require<Label>("startup-message").text=programme.Voice.loading[slot];}
        }
        private void OpenMail()
        {
            if(memory==null||productPanels.IsModalOpen||postgamePanels.IsModalOpen||pending!=null)return;
            mailOpen=true;openLetter=null;DrawMail();SetVisible(Require<VisualElement>("mail-overlay"),true);
            Require<Button>("mail-close").Focus();
        }
        private void CloseMail(){
            if(secondLetter!=null){var next=programme!.Mails.FirstOrDefault(m=>m.id==secondLetter);secondLetter=null;if(next!=null){ReadMail(next);return;}}
            mailOpen=false;SetVisible(Require<VisualElement>("mail-overlay"),false);buttons["mail-open"].Focus();}
        private bool Received(StationProgramme.Mail m)=>m.trigger=="RANDOM_CIRCULAR"?memory!.State.circularIds.Contains(m.id):memory!.State.firedTriggers.Contains(m.trigger);
        private void DrawMail()
        {
            var tray=Require<ScrollView>("mail-envelopes");tray.Clear();
            foreach(var mail in programme!.Mails.Where(Received))
            {
                var button=new Button(()=>ReadMail(mail)){text=mail.subject};button.AddToClassList("envelope");
                button.EnableInClassList("sealed",!memory!.State.readMailIds.Contains(mail.id));tray.Add(button);
            }
            var selected=programme.Mails.FirstOrDefault(m=>m.id==openLetter);
            Require<Label>("mail-letter").text=selected==null?"":selected.from+"\n"+selected.subject+"\n\n"+string.Join(" ",selected.body)+(string.IsNullOrEmpty(selected.ps)?"":"\n\nP.S. "+selected.ps);
            UpdateBadge();
        }
        private void ReadMail(StationProgramme.Mail mail){memory!.Read(mail.id);openLetter=mail.id;DrawMail();}
        private void UpdateBadge()
        {
            int count=programme!.Mails.Count(m=>Received(m)&&!memory!.State.readMailIds.Contains(m.id));
            if(lastMailCount==count)return;
            lastMailCount=count;buttons["mail-open"].text=W("inbox")+(count>0?"  ● "+count:"");
        }

        private void ObserveProgress(long generation,int clears)
        {
            if(lastGeneration==generation)return;lastGeneration=generation;
            var mem=memory!;mem.Fire("FIRST_START");
            if(clears>=3)mem.Fire("AFTER_PUZZLE_3");
            foreach(int n in new[]{10,50,100})if(clears>=n)mem.Fire("CLEARS_"+n);
            var profile=model?.ConfirmedSnapshot?.Payload;
            if(profile!=null){
                totalsStars=profile.Levels.Sum(v=>v.StarsHighWater);threeStars=profile.Levels.Count(v=>v.StarsHighWater==3);
                revisions=profile.Levels.Count(v=>v.RevisionBestElapsedMs.HasValue);
                if(threeStars>0)mem.Fire("FIRST_THREE_STARS");
                var knownProps=new List<string>();
                foreach(var owned in profile.Inventory.Ownership)
                {
                    if(owned is PurchasedOwnership)mem.Fire("FIRST_PURCHASE");
                    if(owned.ItemType=="WAGON")mem.Fire("FIRST_WAGON");
                    var prop=RailwayCosmeticElement.ModuleForItem(owned.ItemId);if(prop!=null)knownProps.Add(prop);
                }
                reducedMotion=profile.Settings.Accessibility?.ReducedMotion==true;worldCamera?.SetReducedMotion(reducedMotion);
                if(!stationProps.SequenceEqual(knownProps)){stationProps.Clear();stationProps.AddRange(knownProps);appearanceVersion++;}
            }
            if(SeasonComplete){mem.Fire("SEASON_END_PRESS");mem.Fire("SEASON_END_LEAD");mem.Fire("ENDLESS_UNLOCKED");mem.Fire("REVISION_UNLOCKED");}
            var voice=programme!.Voice;
            Require<Label>("profile-rank").text=voice.ranks.Where(r=>r.minStars<=totalsStars).OrderByDescending(r=>r.minStars).FirstOrDefault()?.title??"";
            Require<Label>("profile-jubilees").text=string.Join("\n",voice.jubilees.Where(j=>JubileeEarned(j.condition,clears)).Select(j=>j.text));
            Require<Label>("profile-deadline").text=string.Format(W("years-shifted"),mem.State.deadlineShiftYears);
            var home=model!.Puzzles[NextIndex];
            var display=campaignText?.Resolve(NextIndex,L("section."+NextIndex/48),L("route."+NextIndex/12));
            platformTitle=$"{display?.Title}  ·  {home.Width}×{home.Height}";
            UpdateBadge();
        }
        private bool JubileeEarned(string condition,int clears)
        {
            if(condition=="SEASON_COMPLETE")return SeasonComplete;
            int split=condition.LastIndexOf('_');if(split<0||!int.TryParse(condition.Substring(split+1),out int goal))return false;
            var profile=model?.ConfirmedSnapshot?.Payload;
            int count=condition.Substring(0,split) switch{
                "CLEARS"=>clears,"STARS"=>totalsStars,"THREE_STARS"=>threeStars,"RESETS"=>memory!.State.deadlineShiftYears/2,
                "DEPOT_ITEMS"=>profile?.Inventory.Ownership.Count??0,"WAGONS"=>profile?.Inventory.Ownership.Count(o=>o.ItemType=="WAGON")??0,
                "ENDLESS"=>profile?.Endless.EconomyReceipts.Count??0,"REVISION"=>revisions,_=>0};
            return count>=goal;
        }
        private string platformTitle="";
        private void RefreshStation()
        {
            if(memory==null||programme==null||model==null||closedMemory)return;
            var progress=model.State.Progress;
            ObserveProgress(progress?.Generation??-1,progress?.FirstClearCount??0);
            string scene=screen switch{Screen.Map=>"Map",Screen.Depot=>"Depot",Screen.Profile=>"Profile",Screen.Puzzle=>"Puzzle",_=>model.State.CanCreateProfile?"Office":"Platform"};
            if(postgamePanels.IsModalOpen&&Require<VisualElement>("endless-overlay").style.display.value==DisplayStyle.Flex)scene="Endless";
            int zone=(screen==Screen.Map?Math.Max(0,mapStart):screen==Screen.Puzzle&&CurrentCampaignIndex>=0?CurrentCampaignIndex:NextIndex)/48;
            bool finale=scene=="Puzzle"&&CurrentCampaignIndex==239&&SeasonComplete&&view?.Phase==DevelopmentPreviewPhase.Results;
            if(finale)scene="Finale";
            if(scene=="Map"&&revisionDesk)scene="Revision";
            if(scene!=lastScene)
            {
                lastScene=scene;sceneOpened=Time.unscaledTime;lastFlap=-1;if(scene!="Map"&&scene!="Revision")CloseTicket();
                if(scene=="Finale"&&view!=null&&finaleShown!=view.AttemptId){
                    finaleShown=view.AttemptId;finaleLetters=true;
                    SetVisible(Require<VisualElement>("result-panel"),false);
                }
                if(scene!="Puzzle"){rareVisit=memory.Visit();ChooseGag(scene,zone);}
                if(scene=="Depot"){memory.Fire("FIRST_DEPOT");UpdateDepot();}
                if(scene=="Office"){
                    var welcome=programme.Mails.FirstOrDefault(m=>m.trigger=="FIRST_START");
                    if(welcome!=null){memory.Read(welcome.id);var card=Require<VisualElement>("home-mail");
                        card.Q<Label>(className:"mail-from").text=welcome.from;card.Q<Label>(className:"mail-subject").text=welcome.subject;
                        card.Q<Label>(className:"mail-body").text=string.Join(" ",welcome.body);card.Q<Label>(className:"mail-ps").text=welcome.ps;}
                }
                if(scene=="Platform"){CheckCircular();UpdateBadge();}
                if(scene=="Map"||scene=="Revision")mapPulseStart=Time.unscaledTime;
            }
            if(finale){
                bool showing=Time.unscaledTime-sceneOpened<(reducedMotion?0:3.8f);
                SetVisible(Require<VisualElement>("result-panel"),!showing);
                if(!showing&&finaleLetters){finaleLetters=false;ShowSeasonPost();}
            }
            if(scene=="Puzzle"&&CurrentCampaignIndex>=0)memory.Fire("WORLD_ENTER_"+(zone+1));
            var sign=programme.Voice.deadlineSign;int year=2028+memory.State.deadlineShiftYears;
            string deadline=sign.title+"  ·  "+sign.line.Replace("{year}",year.ToString())+" "+sign.note;
            Require<Label>("station-gag").text=scene=="Office"||string.IsNullOrEmpty(gagLine)?deadline:gagLine;
            if((scene=="Map"||scene=="Revision")&&!reducedMotion&&NextIndex>=mapStart&&NextIndex<mapStart+nodes.Length){
                var current=nodes[NextIndex-mapStart];current.style.backgroundColor=Color.Lerp(new Color(.70f,.12f,.20f),new Color(.99f,.28f,.24f),.5f+.5f*Mathf.Sin((Time.unscaledTime-mapPulseStart)*3));
            }
            Require<Label>("map-gag").text=gagLine;Require<VisualElement>("map-nodes").EnableInClassList("filing-cabinet",scene=="Revision");
            Require<Label>("endless-deadline").text=sign.endless;
            Require<Label>("home-route").text=platformTitle;
            buttons["home-play"].text=W(scene=="Office"?"accept":"dispatch");
            SetVisible(buttons["mail-open"],scene!="Office");
            if(scene=="Depot")UpdateDepotText();
            if((scene=="Puzzle"||finale)&&view!=null)RefreshBoardText();
            if(worldCamera!=null)
            {
                var area=scene switch{"Finale"=>viewport,"Puzzle"=>Grid?.Board,"Map"=>mapArt.parent,"Revision"=>mapArt.parent,"Depot"=>depotArt.parent,"Endless"=>Root.Q("endless-art"),"Profile"=>Root.Q("profile-screen"),_=>homeArt.parent};
                if(area==null||area.panel==null)return;
                if(frameAppearance!=appearanceVersion||frameScene!=scene)
                {
                    frameAppearance=appearanceVersion;frameScene=scene;
                    var items=new List<string>(selectedCosmetics);previewProps=stationProps.ToArray();
                    if(scene=="Depot"&&depotIds.Count>0){
                        var id=depotIds[depotIndex];string family=id.Split('-')[0]+"-";
                        if(family=="train-"||family=="livery-")items.RemoveAll(v=>v.StartsWith(family,StringComparison.Ordinal));
                        if(family=="wagon-"&&!items.Contains(id)&&items.Count(v=>v.StartsWith(family,StringComparison.Ordinal))>=3)items.Remove(items.Last(v=>v.StartsWith(family,StringComparison.Ordinal)));
                        if(!items.Contains(id))items.Add(id);
                        var prop=RailwayCosmeticElement.ModuleForItem(id);if(prop!=null)previewProps=new[]{prop};
                    }
                    previewEquipment=items.ToArray();drawnStock=StpKit.Resolve(selectedCosmetics);
                }
                var board=scene=="Puzzle"?view:null;
                bool covered=scene=="Depot"&&depotIds.Count>0&&depotIds[depotIndex].StartsWith("train-",StringComparison.Ordinal)&&ownsCosmetic?.Invoke(depotIds[depotIndex])==false;
                worldCamera.SetStage(scene,zone,board,frameAppearance,previewEquipment,previewProps,covered,memory.State.menuVisits,rareVisit&&(chosenGag.Contains("through")||chosenGag.Contains("ohne halt")||chosenGag.Contains("durchfahrt")));
                if(board!=null&&(!ReferenceEquals(drawnBoard,board)||drawnRevision!=board.Revision||drawnPhase!=board.Phase))
                {worldCamera.SyncBoard();drawnBoard=board;drawnRevision=board.Revision;drawnPhase=board.Phase;}
                var panel=area.panel.visualTree;
                worldCamera.Frame(area.worldBound,panel.layout.size);
                string timetable=GagTimetable(scene);
                worldCamera.UpdateSign(sign.title,scene=="Endless"?sign.endless:sign.line.Replace("{year}",year.ToString()),year,finale?W("on-time"):timetable);
            }
        }
        private IReviewPuzzleSession? drawnBoard;private long drawnRevision=-1;private DevelopmentPreviewPhase drawnPhase;
        private void UpdateDepotText(){if(depotIds.Count>0)Require<Label>("depot-current-name").text=L("cosmetic."+depotIds[depotIndex]+".title");}
        private void ChooseGag(string scene,int zone)
        {
            var possible=programme!.Gags.Where(g=>(g.scene=="ALL"||g.scene==scene.ToUpperInvariant()||g.scene=="WORLD_"+(zone+1))&&(g.kind=="RARE")==rareVisit).ToArray();
            if(possible.Length==0){chosenGag="";gagLine="";return;}
            var gag=possible[(memory!.State.menuVisits/2)%possible.Length];gagLine=gag.line;chosenGag=(gag.id+" "+string.Join(" ",gag.objects)+" "+gag.line).ToLowerInvariant();
            if(!string.IsNullOrEmpty(gag.sound))sound?.Invoke(gag.sound);
        }
        private string GagTimetable(string scene)
        {
            if(scene!="Platform")return gagLine;
            if(!rareVisit)return platformTitle;
            if(chosenGag.Contains("delay")||chosenGag.Contains("fällt")||chosenGag.Contains("cancel")){
                int flap=Mathf.Clamp((int)((Time.unscaledTime-sceneOpened)/1.2f),0,3);
                if(lastFlap!=flap){lastFlap=flap;sound?.Invoke(flap==3?"Sigh":"Flap");}
                return flap==3?gagLine:new[]{"+5","+10","+20"}[flap];
            }
            return gagLine;
        }
        private void ShowSeasonPost()
        {
            var first=programme!.Mails.FirstOrDefault(m=>m.trigger=="SEASON_END_PRESS"&&!memory!.State.readMailIds.Contains(m.id));
            var second=programme.Mails.FirstOrDefault(m=>m.trigger=="SEASON_END_LEAD"&&!memory!.State.readMailIds.Contains(m.id));
            if(first==null&&second==null)return;
            OpenMail();if(!mailOpen)return;
            secondLetter=first!=null?second?.id:null;ReadMail((first??second)!);
        }
        private void CheckCircular()
        {
            string date=DateTime.Now.ToString("yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
            if(memory!.State.lastCircularDate==date||!rareVisit)return;
            var pool=programme!.Mails.Where(m=>m.trigger=="RANDOM_CIRCULAR"&&!Received(m)).ToArray();
            if(pool.Length>0)memory.Circular(date,pool[UnityEngine.Random.Range(0,pool.Length)].id);
        }
        private static string Minutes(long seconds)=>$"{seconds/60}:{seconds%60:00}";
        private void RefreshBoardText()
        {
            var v=view!;int index=CurrentCampaignIndex;
            var voice=programme!.Voice;
            var targets=index>=0?starTargets?.Invoke(index):null;
            long elapsed=v.ElapsedAt(clock());long secs=elapsed/1000;
            var record=index>=0?readCampaignRecord?.Invoke(index):null;
            if(observedAttempt!=v.AttemptId){observedAttempt=v.AttemptId;previousBest=record?.RevisionBestElapsedMs??record?.FirstRunBestElapsedMs;balanceAtStart=model!.State.Progress?.BalancePatience??0;}
            Require<Label>("puzzle-phase").text=$"{v.Width}×{v.Height}";
            Require<Label>("puzzle-time").text=Minutes(secs);
            var ticker=Require<Label>("puzzle-title");ticker.text=Require<Label>("puzzle-phase").tooltip;
            if(!reducedMotion&&ticker.layout.width>0){
                float natural=ticker.MeasureTextSize(ticker.text,0,VisualElement.MeasureMode.Undefined,0,VisualElement.MeasureMode.Undefined).x;
                float overflow=Mathf.Max(0,natural-ticker.parent.contentRect.width);
                // Keep the single-line board message inside its fixed aperture.
                float travel=Mathf.Clamp(Mathf.PingPong((Time.unscaledTime-sceneOpened)*23,overflow+38)-19,0,overflow);
                ticker.style.width=Mathf.Max(ticker.parent.contentRect.width,natural+1);
                ticker.style.translate=new Translate(-travel,0);
            }
            Require<Label>("puzzle-targets").text=targets.HasValue?$"★★★ {Minutes(targets.Value.three/1000)}  ·  ★★ {Minutes(targets.Value.two/1000)}  ·  ★ {W("arrival")}":"";
            Require<Label>("puzzle-delay").text=targets.HasValue&&elapsed>targets.Value.three?"+"+Minutes((elapsed-targets.Value.three)/1000):"";
            buttons["puzzle-hint"].text=L("hint")+" "+(hintCreditCount?.Invoke()??0);
            int step=index+1;var tutorial=voice.tutorial.FirstOrDefault(item=>item.step==step);
            var banner=Require<Label>("tutorial-banner");
            if(v.Phase==DevelopmentPreviewPhase.TrainRide){banner.text=worldCamera?.ReversedOrder==true&&drawnStock.Wagons?.Length>0?W("reversed"):voice.arrivalAnnouncements.FirstOrDefault()??"";SetVisible(banner,true);}
            else {bool show=v.Phase==DevelopmentPreviewPhase.Editing&&tutorial!=null&&!memory!.State.tutorialStepsDone.Contains(step);
                banner.text=show?tutorial!.text:"";SetVisible(banner,show);}
            if(v.Phase!=DevelopmentPreviewPhase.Results)return;
            int stars=model!.State.Progress?.SelectedStars??0;
            buttons["result-next"].text=index==239&&SeasonComplete?voice.seasonEnd.greyedNext:W(index<0?"again":"next");
            buttons["result-home"].text=W("signal-box");
            if(index<0){SetVisible(buttons["result-next"],false);buttons["result-endless"].text=W("again");}
            else SetVisible(buttons["result-next"],true);
            if(resultAttempt==v.AttemptId)return;
            resultAttempt=v.AttemptId;
            if(step==5)memory!.CompleteTutorial(5);
            long delta=targets.HasValue&&stars<3?Math.Max(0,(elapsed-(stars==2?targets.Value.three:targets.Value.two)+999)/1000):0;
            Require<Label>("result-star-targets").text=delta>0?string.Format(W("shortfall"),Minutes(delta),stars==2?"★★★":"★★"):"";
            Require<Label>("result-record").text=(!previousBest.HasValue||elapsed<previousBest.Value)&&index>=0?W("record"):"";
            Celebrate(stars,Math.Max(0,(model.State.Progress?.BalancePatience??balanceAtStart)-balanceAtStart));
        }
        private void OnTutorialAction(DevelopmentTool tool,int x,int y)
        {
            if(memory==null||view==null)return;
            lastStep=CurrentCampaignIndex+1;
            bool track=(int)tool>=(int)DevelopmentTool.TrackNS;
            if((lastStep==1&&track)||(lastStep==2&&track&&(view.RowVisible(y)==view.RowTarget(y)||view.ColumnVisible(x)==view.ColumnTarget(x)))||
                (lastStep==3&&tool==DevelopmentTool.EmptyMarker)||(lastStep==4&&tool==DevelopmentTool.OccupiedMarker))
                memory.CompleteTutorial(lastStep);
        }
        public void ShowRide(ProductionTrainRide ride)
        {
            if(worldCamera==null){SetTrainPose(ride.X,ride.Y,ride.AngleDegrees,true);return;}
            var stock=drawnStock.Wagons==null?StpKit.Resolve(selectedCosmetics):drawnStock;
            float d=0;
            for(int i=0;i<=stock.Wagons.Length;i++){
                var position=ride.Behind(d);var dir=ride.Behind(d-.09f)-ride.Behind(d+.09f);
                int car=worldCamera.ReversedOrder?stock.Wagons.Length-i:i;
                worldCamera.TrainPose(car,position,Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg);
                int nextCar=worldCamera.ReversedOrder?car-1:car+1;
                d+=((car==0?StpKit.Length(stock.Type):StpKit.WagonLength)+(nextCar==0?StpKit.Length(stock.Type):StpKit.WagonLength))*.5f+.055f;
            }
        }
    }
}
