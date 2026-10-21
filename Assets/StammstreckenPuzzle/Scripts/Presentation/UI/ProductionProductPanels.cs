using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Daily and settings panels over confirmed Root read models and explicitly requested actions.</summary>
    public sealed class ProductionProductPanels:IDisposable
    {
        private static readonly string[] AudioGroups={"Master","Music","SFX","Ambience","UI"};
        private readonly VisualElement root;
        private readonly ProductionGameLocalization locale;
        private readonly Dictionary<Button,Action> actions=new Dictionary<Button,Action>();
        private readonly Dictionary<string,Button[]> audioButtons=new Dictionary<string,Button[]>(StringComparer.Ordinal);
        private readonly Dictionary<string,Label> audioLabels=new Dictionary<string,Label>(StringComparer.Ordinal);
        private readonly Button dailyFree,dailyVideo,dailyClose,settingsClose,haptics,reducedMotion,skipRide,privacy,restore,removeAds;
        private Func<DailyView>? readDaily;
        private Func<bool,Task>? claimDaily;
        private Func<LocalSettings>? readSettings;
        private Func<LocalSettings,Task>? saveSettings;
        private Func<ProductionServiceState>? readServices;
        private Func<ProductionServiceAction,Task>? serviceAction;
        private Task? dailyTask,settingsTask,serviceTask;
        private ProductionServiceAction? renderedStoreAction;
        private string? renderedStorePrice;
        private bool dailyOpen,settingsOpen,dailyFailed,settingsFailed,serviceFailed,disposed;

        /// <summary>Creates normal product panels. It never reserves, rolls, grants or saves a setting itself.</summary>
        public ProductionProductPanels(VisualElement root,ProductionGameLocalization locale)
        {
            this.root=root??throw new ArgumentNullException(nameof(root));this.locale=locale??throw new ArgumentNullException(nameof(locale));
            Add("home-daily","daily.open",()=>{dailyOpen=true;Refresh();});dailyFree=Add("daily-free","daily.free",()=>ClaimDaily(false));
            dailyVideo=Add("daily-video","daily.video",()=>ClaimDaily(true));dailyClose=Add("daily-close","notice.close",()=>{if(dailyTask==null){dailyOpen=false;Refresh();}});
            Add("profile-settings","settings.open",()=>{settingsOpen=true;Refresh();});settingsClose=Add("settings-close","notice.close",()=>{if(settingsTask==null&&serviceTask==null){settingsOpen=false;Refresh();}});
            haptics=Add("settings-haptics","settings.haptics",()=>ChangeSettings(s=>Copy(s,hapticsEnabled:!(s.HapticsEnabled??false))));
            reducedMotion=Add("settings-motion","settings.motion",()=>ChangeSettings(s=>Copy(s,reduced:!(s.Accessibility?.ReducedMotion??false))));
            skipRide=Add("settings-skip","settings.skip",()=>ChangeSettings(s=>Copy(s,skip:!(s.Accessibility?.TrainRideSkip??false))));
            privacy=Add("settings-privacy","settings.privacy",()=>RunService(ProductionServiceAction.ReviewPrivacy));
            restore=Add("settings-restore","settings.restore",()=>RunService(ProductionServiceAction.RestorePurchases));
            removeAds=Add("settings-remove-ads","settings.remove-ads",RequestStoreAction);
            Add("settings-legal","settings.legal",()=>RunService(ProductionServiceAction.ShowLegal));Add("settings-support","settings.support",()=>RunService(ProductionServiceAction.ContactSupport));
            var host=Require<VisualElement>("settings-audio");
            foreach(var group in AudioGroups)
            {
                var captured=group;var row=new VisualElement();row.AddToClassList("settings-audio-row");
                var label=new Label(L("settings.audio."+group));label.AddToClassList("settings-audio-label");row.Add(label);audioLabels.Add(group,label);
                var choices=new VisualElement();choices.AddToClassList("settings-audio-choices");var values=new[]{0,50,100};var buttons=new Button[3];
                for(var i=0;i<3;i++)
                {
                    var volume=values[i];var button=new Button{text=L("settings.percent",volume),name="settings-audio-"+group+"-"+volume};button.AddToClassList("settings-volume");
                    Action action=()=>ChangeSettings(s=>Copy(s,group:captured,volume:volume));button.clicked+=action;actions.Add(button,action);buttons[i]=button;choices.Add(button);
                }
                audioButtons.Add(group,buttons);row.Add(choices);host.Add(row);
            }
            Refresh();
        }
        /// <summary>Whether a product modal owns the player's attention and must keep gameplay paused.</summary>
        public bool IsModalOpen=>dailyOpen||settingsOpen;
        /// <summary>Binds actual persistent daily eligibility and a Root-controlled free/video claim flow.</summary>
        public void BindDaily(Func<DailyView> read,Func<bool,Task> action)
        {readDaily=read??throw new ArgumentNullException(nameof(read));claimDaily=action??throw new ArgumentNullException(nameof(action));Refresh();}
        /// <summary>Binds actual immutable local settings and an atomic acknowledged settings-save operation.</summary>
        public void BindSettings(Func<LocalSettings> read,Func<LocalSettings,Task> save)
        {readSettings=read??throw new ArgumentNullException(nameof(read));saveSettings=save??throw new ArgumentNullException(nameof(save));Refresh();}
        /// <summary>Binds actual Store/privacy readiness; service failure never grants an entitlement or blocks the game.</summary>
        public void BindServices(Func<ProductionServiceState> read,Func<ProductionServiceAction,Task> action)
        {readServices=read??throw new ArgumentNullException(nameof(read));serviceAction=action??throw new ArgumentNullException(nameof(action));Refresh();}
        /// <summary>Projects read models after observing operation outcomes; tasks are not taken as grant truth.</summary>
        public void Refresh()
        {
            if(disposed)return;Poll(ref dailyTask,ref dailyFailed);Poll(ref settingsTask,ref settingsFailed);Poll(ref serviceTask,ref serviceFailed);
            Visible("daily-overlay",dailyOpen);Visible("settings-overlay",settingsOpen);
            dailyClose.SetEnabled(dailyTask==null);settingsClose.SetEnabled(settingsTask==null&&serviceTask==null);
            DailyView? daily=null;try{daily=readDaily?.Invoke();}catch{dailyFailed=true;}
            Require<Label>("daily-date").text=daily?.DateLabel??L("daily.unavailable");
            Require<Label>("daily-result").text=daily?.ConfirmedAmount is int points?L("daily.result",points):L("daily.ready");
            Require<Label>("daily-state").text=L(dailyTask!=null?"daily.pending":dailyFailed?"daily.failed":daily?.PendingClaim==true?"daily.resume":daily?.ConfirmedAmount!=null?"daily.confirmed.status":"daily.optional");
            dailyFree.SetEnabled(dailyTask==null&&claimDaily!=null&&daily?.FreeAvailable==true);dailyVideo.SetEnabled(dailyTask==null&&claimDaily!=null&&daily?.VideoAvailable==true);
            LocalSettings? settings=null;try{settings=readSettings?.Invoke();}catch{settingsFailed=true;}
            var editable=settingsTask==null&&settings!=null&&saveSettings!=null;
            foreach(var group in AudioGroups)
            {
                var stored=settings?.AudioGroups.FirstOrDefault(g=>g.GroupId==group);var percent=stored==null?100:(int)Math.Round(stored.Numerator*100d/stored.Denominator);
                audioLabels[group].text=L("settings.audio.value",L("settings.audio."+group),percent);
                for(var i=0;i<3;i++){audioButtons[group][i].SetEnabled(editable);audioButtons[group][i].EnableInClassList("settings-choice--active",percent==i*50);}
            }
            haptics.text=L("settings.toggle",L("settings.haptics"),L(settings?.HapticsEnabled==true?"settings.on":"settings.off"));haptics.SetEnabled(editable);
            reducedMotion.text=L("settings.toggle",L("settings.motion"),L(settings?.Accessibility?.ReducedMotion==true?"settings.on":"settings.off"));reducedMotion.SetEnabled(editable);
            skipRide.text=L("settings.toggle",L("settings.skip"),L(settings?.Accessibility?.TrainRideSkip==true?"settings.on":"settings.off"));skipRide.SetEnabled(editable);
            Require<Label>("settings-save-state").text=L(settingsTask!=null?"settings.saving":settingsFailed?"settings.failed":settings!=null?"settings.saved":"settings.unavailable");
            ProductionServiceState? services=null;try{services=readServices?.Invoke();}catch{serviceFailed=true;}
            var serviceReady=serviceTask==null&&serviceAction!=null;
            privacy.SetEnabled(serviceReady&&services?.CanReviewPrivacy==true);restore.SetEnabled(serviceReady&&services?.CanRestore==true);
            var canPurchase=services?.CanPurchaseRemoveAds==true&&!string.IsNullOrWhiteSpace(services.LocalizedRemoveAdsPrice)&&!services.RemoveAdsOwned;
            var canLoad=services?.CanLoadStorePrice==true&&!services.RemoveAdsOwned;
            renderedStoreAction=!serviceReady||services?.RemoveAdsOwned==true?(ProductionServiceAction?)null:canPurchase?ProductionServiceAction.PurchaseRemoveAds:canLoad?ProductionServiceAction.LoadStorePrice:null;
            renderedStorePrice=canPurchase?services!.LocalizedRemoveAdsPrice:null;removeAds.SetEnabled(renderedStoreAction.HasValue);
            removeAds.text=services?.RemoveAdsOwned==true?L("settings.remove-ads.owned"):canPurchase?L("settings.remove-ads.price",renderedStorePrice!):canLoad?L("settings.remove-ads.load-price"):L("settings.remove-ads.unavailable");
            Require<Label>("settings-service-state").text=L(serviceTask!=null?"settings.service.pending":serviceFailed?"settings.service.failed":"settings.service.quiet");
            Require<Button>("settings-legal").SetEnabled(serviceReady);Require<Button>("settings-support").SetEnabled(serviceReady);
        }
        /// <summary>Releases own callbacks and observes late failures without UI access.</summary>
        public void Dispose()
        {if(disposed)return;disposed=true;foreach(var pair in actions)pair.Key.clicked-=pair.Value;Observe(dailyTask);Observe(settingsTask);Observe(serviceTask);}
        private void ClaimDaily(bool rewarded)
        {
            if(disposed||dailyTask!=null||readDaily==null||claimDaily==null)return;dailyFailed=false;
            try{var state=readDaily();if(rewarded?!state.VideoAvailable:!state.FreeAvailable)return;dailyTask=claimDaily(rewarded)??throw new InvalidOperationException("DAILY_TASK_NULL");}catch{dailyFailed=true;}Refresh();
        }
        private void ChangeSettings(Func<LocalSettings,LocalSettings> change)
        {
            if(disposed||settingsTask!=null||readSettings==null||saveSettings==null)return;settingsFailed=false;
            try{settingsTask=saveSettings(change(readSettings()))??throw new InvalidOperationException("SETTINGS_TASK_NULL");}catch{settingsFailed=true;}Refresh();
        }
        private void RequestStoreAction()
        {
            if(disposed||serviceTask!=null||readServices==null||!renderedStoreAction.HasValue)return;
            var action=renderedStoreAction.Value;var price=renderedStorePrice;
            try
            {
                var state=readServices();
                if(state.RemoveAdsOwned||(action==ProductionServiceAction.LoadStorePrice?!state.CanLoadStorePrice:
                    !state.CanPurchaseRemoveAds||string.IsNullOrWhiteSpace(price)||!string.Equals(price,state.LocalizedRemoveAdsPrice,StringComparison.Ordinal)))
                {Refresh();return;}
                RunService(action);
            }
            catch{serviceFailed=true;Refresh();}
        }
        private void RunService(ProductionServiceAction action)
        {
            if(disposed||serviceTask!=null||serviceAction==null)return;serviceFailed=false;
            try{serviceTask=serviceAction(action)??throw new InvalidOperationException("SERVICE_TASK_NULL");}catch{serviceFailed=true;}Refresh();
        }
        private static LocalSettings Copy(LocalSettings source,string? group=null,int volume=100,bool? hapticsEnabled=null,bool? reduced=null,bool? skip=null)
        {
            var groups=source.AudioGroups.ToList();
            if(group!=null){groups.RemoveAll(g=>g.GroupId==group);groups.Add(LocalAudioPreference.Create(group,volume,100));}groups.Sort((a,b)=>string.CompareOrdinal(a.GroupId,b.GroupId));
            var access=source.Accessibility;
            if(reduced.HasValue||skip.HasValue)access=LocalAccessibilityPreference.Create(access?.ContractRevision??"accessibility-launch-v1",reduced??access?.ReducedMotion,skip??access?.TrainRideSkip);
            return LocalSettings.Create(source.Locale,groups,hapticsEnabled??source.HapticsEnabled,access);
        }
        private static void Poll(ref Task? task,ref bool failed)
        {if(task==null||!task.IsCompleted)return;var completed=task;task=null;try{completed.GetAwaiter().GetResult();}catch{failed=true;}}
        private static void Observe(Task? task){if(task!=null)_=task.ContinueWith(t=>{_ = t.Exception;},CancellationToken.None,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);}
        private string L(string key,params object[] args)=>locale.Resolve(key,args);
        private T Require<T>(string id)where T:VisualElement=>root.Q<T>(id)??throw new InvalidOperationException("PRODUCT_PANEL_ELEMENT_MISSING:"+id);
        private void Visible(string id,bool show)=>Require<VisualElement>(id).style.display=show?DisplayStyle.Flex:DisplayStyle.None;
        private Button Add(string id,string key,Action action){var b=Require<Button>(id);b.text=L(key);b.clicked+=action;actions.Add(b,action);return b;}
    }
}
