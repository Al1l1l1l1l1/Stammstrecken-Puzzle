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
        private readonly Button dailyFree,dailyVideo,dailyClose,settingsClose,haptics,reducedMotion,skipRide,privacy,restore,removeAds,legal,support;
        private Func<DailyView>? readDaily;
        private Func<bool,Task>? claimDaily;
        private Func<LocalSettings>? readSettings;
        private Func<LocalSettings,Task>? saveSettings;
        private Func<ProductionServiceState>? readServices;
        private Func<ProductionServiceAction,Task>? serviceAction;
        private Task? dailyTask,settingsTask,serviceTask,localDataTask;
        private Func<ProductionLocalDataView>? readLocalData;
        private Func<ProductionLocalDataAction,Task>? localDataAction;
        private ProductionLocalDataState localDataState=ProductionLocalDataState.Unavailable;
        private ProductionLocalDataAction? localDataPendingAction;
        private bool localDataFailed,localDataReadFailed,localDataInvoking,localDataIntentStarted,localDataWasBusy,externalSuspended;
        private readonly Button localDataOpen,localDataStartup,localDataError,localDataConfirm,localDataCancel,localDataRetry,localDataRestart;
        private ProductionServiceAction? renderedStoreAction;
        private string? renderedStorePrice;
        private bool dailyOpen,settingsOpen,dailyFailed,settingsFailed,serviceFailed,disposed;

        /// <summary>Creates normal product panels. It never reserves, rolls, grants or saves a setting itself.</summary>
        public ProductionProductPanels(VisualElement root,ProductionGameLocalization locale)
        {
            this.root=root??throw new ArgumentNullException(nameof(root));this.locale=locale??throw new ArgumentNullException(nameof(locale));
            Add("home-daily","daily.open",()=>{if(!CanUseOrdinaryPanelActions())return;dailyOpen=true;Refresh();});dailyFree=Add("daily-free","daily.free",()=>ClaimDaily(false));
            dailyVideo=Add("daily-video","daily.video",()=>ClaimDaily(true));dailyClose=Add("daily-close","notice.close",()=>{if(CanUseOrdinaryPanelActions()&&dailyTask==null){dailyOpen=false;Refresh();}});
            Add("profile-settings","settings.open",()=>{if(!CanUseOrdinaryPanelActions())return;settingsOpen=true;Refresh();});settingsClose=Add("settings-close","notice.close",()=>{if(CanUseOrdinaryPanelActions()&&settingsTask==null&&serviceTask==null){settingsOpen=false;Refresh();}});
            haptics=Add("settings-haptics","settings.haptics",()=>ChangeSettings(s=>Copy(s,hapticsEnabled:!(s.HapticsEnabled??false))));
            reducedMotion=Add("settings-motion","settings.motion",()=>ChangeSettings(s=>Copy(s,reduced:!(s.Accessibility?.ReducedMotion??false))));
            skipRide=Add("settings-skip","settings.skip",()=>ChangeSettings(s=>Copy(s,skip:!(s.Accessibility?.TrainRideSkip??false))));
            privacy=Add("settings-privacy","settings.privacy",()=>RunService(ProductionServiceAction.ReviewPrivacy));
            restore=Add("settings-restore","settings.restore",()=>RunService(ProductionServiceAction.RestorePurchases));
            removeAds=Add("settings-remove-ads","settings.remove-ads",RequestStoreAction);
            legal=Add("settings-legal","settings.legal",()=>RequestContactAction(ProductionServiceAction.ShowLegal));support=Add("settings-support","settings.support",()=>RequestContactAction(ProductionServiceAction.ContactSupport));
            localDataOpen=Add("settings-delete-local","local-data.open",()=>RequestLocalData(ProductionLocalDataAction.Open));
            localDataStartup=Add("startup-delete-local","local-data.open",()=>RequestLocalData(ProductionLocalDataAction.Open));
            localDataError=Add("error-delete-local","local-data.open",()=>RequestLocalData(ProductionLocalDataAction.Open));
            localDataConfirm=Add("local-data-confirm","local-data.confirm",()=>RequestLocalData(ProductionLocalDataAction.Confirm));
            localDataCancel=Add("local-data-cancel","local-data.cancel",()=>RequestLocalData(ProductionLocalDataAction.Cancel));
            localDataRetry=Add("local-data-retry","local-data.retry",()=>RequestLocalData(ProductionLocalDataAction.Retry));
            localDataRestart=Add("local-data-restart","local-data.restart",()=>RequestLocalData(ProductionLocalDataAction.Restart));
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
            root.RegisterCallback<AttachToPanelEvent>(OnLocalDataAttached);Refresh();
        }
        /// <summary>Whether a product modal owns the player's attention and must keep gameplay paused.</summary>
        public bool IsModalOpen=>dailyOpen||settingsOpen||IsLocalDataBlocking;
        /// <summary>A destructive lifecycle operation owns attention until Root confirms its outcome and restarts.</summary>
        public bool IsLocalDataBlocking=>localDataInvoking||localDataTask!=null||localDataIntentStarted||localDataState==ProductionLocalDataState.Confirmation||localDataState==ProductionLocalDataState.Deleting||localDataState==ProductionLocalDataState.RetryRequired||localDataState==ProductionLocalDataState.Deleted;
        /// <summary>Shares Root's existing external/native-focus fence with the same UI handlers.</summary>
        public void SetExternalSuspended(bool value)
        {if(disposed||externalSuspended==value)return;externalSuspended=value;if(value){localDataConfirm.Blur();localDataCancel.Blur();localDataRetry.Blur();localDataRestart.Blur();}Refresh();if(!value)FocusLocalDataAction();}
        /// <summary>Binds exact Root-held profile lifecycle status; does not create an intent or access storage.</summary>
        public void BindLocalDataDeletion(Func<ProductionLocalDataView> read,Func<ProductionLocalDataAction,Task> action)
        {if(disposed)throw new ObjectDisposedException(nameof(ProductionProductPanels));if(readLocalData!=null)throw new InvalidOperationException("LOCAL_DATA_ALREADY_BOUND");readLocalData=read??throw new ArgumentNullException(nameof(read));localDataAction=action??throw new ArgumentNullException(nameof(action));Refresh();}
        /// <summary>Binds actual persistent daily eligibility and a Root-controlled free/video claim flow.</summary>
        public void BindDaily(Func<DailyView> read,Func<bool,Task> action)
        {readDaily=read??throw new ArgumentNullException(nameof(read));claimDaily=action??throw new ArgumentNullException(nameof(action));Refresh();}
        /// <summary>Binds actual immutable local settings and an atomic acknowledged settings-save operation.</summary>
        public void BindSettings(Func<LocalSettings> read,Func<LocalSettings,Task> save)
        {readSettings=read??throw new ArgumentNullException(nameof(read));saveSettings=save??throw new ArgumentNullException(nameof(save));Refresh();}
        /// <summary>Binds actual Store/privacy/contact readiness; service failure never grants an entitlement or blocks the game.</summary>
        public void BindServices(Func<ProductionServiceState> read,Func<ProductionServiceAction,Task> action)
        {readServices=read??throw new ArgumentNullException(nameof(read));serviceAction=action??throw new ArgumentNullException(nameof(action));Refresh();}
        /// <summary>Projects read models after observing operation outcomes; tasks are not taken as grant truth.</summary>
        public void Refresh()
        {
            if(disposed)return;Poll(ref dailyTask,ref dailyFailed);Poll(ref settingsTask,ref settingsFailed);Poll(ref serviceTask,ref serviceFailed);
            if(!localDataInvoking)Poll(ref localDataTask,ref localDataFailed);if(localDataTask==null&&!localDataInvoking)localDataPendingAction=null;
            var previousLocalState=localDataState;ReadLocalData();var ordinary=!externalSuspended&&!IsLocalDataBlocking;
            Visible("daily-overlay",dailyOpen&&!IsLocalDataBlocking);Visible("settings-overlay",settingsOpen&&!IsLocalDataBlocking);
            dailyClose.SetEnabled(ordinary&&dailyTask==null);settingsClose.SetEnabled(ordinary&&settingsTask==null&&serviceTask==null);
            DailyView? daily=null;try{daily=readDaily?.Invoke();}catch{dailyFailed=true;}
            Require<Label>("daily-date").text=daily?.DateLabel??L("daily.unavailable");
            Require<Label>("daily-result").text=daily?.ConfirmedAmount is int points?L("daily.result",points):L("daily.ready");
            Require<Label>("daily-state").text=L(dailyTask!=null?"daily.pending":dailyFailed?"daily.failed":daily?.PendingClaim==true?"daily.resume":daily?.ConfirmedAmount!=null?"daily.confirmed.status":"daily.optional");
            dailyFree.SetEnabled(ordinary&&dailyTask==null&&claimDaily!=null&&daily?.FreeAvailable==true);dailyVideo.SetEnabled(ordinary&&dailyTask==null&&claimDaily!=null&&daily?.VideoAvailable==true);
            LocalSettings? settings=null;try{settings=readSettings?.Invoke();}catch{settingsFailed=true;}
            var editable=ordinary&&settingsTask==null&&settings!=null&&saveSettings!=null;
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
            var serviceReady=ordinary&&serviceTask==null&&serviceAction!=null;
            privacy.SetEnabled(serviceReady&&services?.CanReviewPrivacy==true);restore.SetEnabled(serviceReady&&services?.CanRestore==true);
            var canPurchase=services?.CanPurchaseRemoveAds==true&&!string.IsNullOrWhiteSpace(services.LocalizedRemoveAdsPrice)&&!services.RemoveAdsOwned;
            var canLoad=services?.CanLoadStorePrice==true&&!services.RemoveAdsOwned;
            renderedStoreAction=!serviceReady||services?.RemoveAdsOwned==true?(ProductionServiceAction?)null:canPurchase?ProductionServiceAction.PurchaseRemoveAds:canLoad?ProductionServiceAction.LoadStorePrice:null;
            renderedStorePrice=canPurchase?services!.LocalizedRemoveAdsPrice:null;removeAds.SetEnabled(renderedStoreAction.HasValue);
            removeAds.text=services?.RemoveAdsOwned==true?L("settings.remove-ads.owned"):canPurchase?L("settings.remove-ads.price",renderedStorePrice!):canLoad?L("settings.remove-ads.load-price"):L("settings.remove-ads.unavailable");
            Require<Label>("settings-service-state").text=L(serviceTask!=null?"settings.service.pending":serviceFailed?"settings.service.failed":"settings.service.quiet");
            legal.SetEnabled(serviceReady&&services?.CanShowLegal==true);support.SetEnabled(serviceReady&&services?.CanContactSupport==true);
            var contactState=Require<Label>("settings-contact-state");
            var legalReady=services?.CanShowLegal==true;var supportReady=services?.CanContactSupport==true;
            contactState.text=legalReady&&supportReady?string.Empty:L(!legalReady&&!supportReady?"settings.contacts.unavailable":!legalReady?"settings.legal.unavailable":"settings.support.unavailable");
            contactState.style.display=legalReady&&supportReady?DisplayStyle.None:DisplayStyle.Flex;
            RenderLocalData();var localBusy=localDataInvoking||localDataTask!=null;
            if(previousLocalState!=localDataState||localDataWasBusy&&!localBusy)FocusLocalDataAction();localDataWasBusy=localBusy;
        }
        /// <summary>Releases own callbacks and observes late failures without UI access.</summary>
        public void Dispose()
        {if(disposed)return;disposed=true;root.UnregisterCallback<AttachToPanelEvent>(OnLocalDataAttached);foreach(var pair in actions)pair.Key.clicked-=pair.Value;Observe(dailyTask);Observe(settingsTask);Observe(serviceTask);Observe(localDataTask);}
        private void ClaimDaily(bool rewarded)
        {
            if(!CanUseOrdinaryPanelActions()||dailyTask!=null||readDaily==null||claimDaily==null)return;dailyFailed=false;
            try{var state=readDaily();if(rewarded?!state.VideoAvailable:!state.FreeAvailable)return;dailyTask=claimDaily(rewarded)??throw new InvalidOperationException("DAILY_TASK_NULL");}catch{dailyFailed=true;}Refresh();
        }
        private void ChangeSettings(Func<LocalSettings,LocalSettings> change)
        {
            if(!CanUseOrdinaryPanelActions()||settingsTask!=null||readSettings==null||saveSettings==null)return;settingsFailed=false;
            try{settingsTask=saveSettings(change(readSettings()))??throw new InvalidOperationException("SETTINGS_TASK_NULL");}catch{settingsFailed=true;}Refresh();
        }
        private void RequestStoreAction()
        {
            if(!CanUseOrdinaryPanelActions()||serviceTask!=null||readServices==null||!renderedStoreAction.HasValue)return;
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
        private void RequestContactAction(ProductionServiceAction action)
        {
            if(!CanUseOrdinaryPanelActions()||!settingsOpen||serviceTask!=null||readServices==null||serviceAction==null)return;
            try
            {
                var state=readServices();
                if(state==null||(action==ProductionServiceAction.ShowLegal?!state.CanShowLegal:!state.CanContactSupport)){Refresh();return;}
                RunService(action);
            }
            catch{serviceFailed=true;Refresh();}
        }
        private void RunService(ProductionServiceAction action)
        {
            if(!CanUseOrdinaryPanelActions()||serviceTask!=null||serviceAction==null)return;serviceFailed=false;
            try{serviceTask=serviceAction(action)??throw new InvalidOperationException("SERVICE_TASK_NULL");}catch{serviceFailed=true;}Refresh();
        }
        private bool ReadLocalData()
        {
            if(readLocalData==null){localDataState=ProductionLocalDataState.Unavailable;localDataReadFailed=false;return true;}
            try
            {
                localDataState=(readLocalData()??throw new InvalidOperationException("LOCAL_DATA_VIEW_NULL")).State;
                if(localDataState==ProductionLocalDataState.Deleting||localDataState==ProductionLocalDataState.RetryRequired||localDataState==ProductionLocalDataState.Deleted)localDataIntentStarted=true;
                localDataReadFailed=false;return true;
            }
            catch{localDataReadFailed=true;return false;}
        }
        private bool CanUseOrdinaryPanelActions()=>!disposed&&!externalSuspended&&ReadLocalData()&&!IsLocalDataBlocking;
        private void RequestLocalData(ProductionLocalDataAction action)
        {
            if(disposed||externalSuspended||localDataInvoking||localDataTask!=null||readLocalData==null||localDataAction==null)return;
            if(!ReadLocalData()){Refresh();return;}
            var allowed=AllowsLocalDataAction(localDataState,localDataIntentStarted,action)&&
                (action!=ProductionLocalDataAction.Open||dailyTask==null&&settingsTask==null&&serviceTask==null);
            if(!allowed){Refresh();return;}
            localDataFailed=false;localDataInvoking=true;localDataPendingAction=action;
            if(action==ProductionLocalDataAction.Confirm||action==ProductionLocalDataAction.Retry)localDataIntentStarted=true;
            try
            {
                var task=localDataAction(action)??throw new InvalidOperationException("LOCAL_DATA_TASK_NULL");
                if(disposed){Observe(task);return;}localDataTask=task;
            }
            catch{if(!disposed)localDataFailed=true;}
            finally{localDataInvoking=false;}
            if(!disposed)Refresh();
        }
        private static bool AllowsLocalDataAction(ProductionLocalDataState state,bool intentStarted,ProductionLocalDataAction action)
        {
            switch(action)
            {
                case ProductionLocalDataAction.Open:return state==ProductionLocalDataState.Ready&&!intentStarted;
                case ProductionLocalDataAction.Cancel:case ProductionLocalDataAction.Confirm:return state==ProductionLocalDataState.Confirmation&&!intentStarted;
                case ProductionLocalDataAction.Retry:return state==ProductionLocalDataState.RetryRequired;
                case ProductionLocalDataAction.Restart:return state==ProductionLocalDataState.RetryRequired||state==ProductionLocalDataState.Deleted;
                default:return false;
            }
        }
        private void RenderLocalData()
        {
            var blocking=IsLocalDataBlocking;var busy=localDataInvoking||localDataTask!=null;
            var ready=readLocalData!=null&&localDataAction!=null&&!localDataReadFailed&&!externalSuspended;
            var canOpen=ready&&!blocking&&localDataState==ProductionLocalDataState.Ready&&dailyTask==null&&settingsTask==null&&serviceTask==null;
            foreach(var button in new[]{localDataOpen,localDataStartup,localDataError})button.SetEnabled(canOpen);
            localDataStartup.style.display=canOpen?DisplayStyle.Flex:DisplayStyle.None;localDataError.style.display=canOpen?DisplayStyle.Flex:DisplayStyle.None;
            Require<Label>("settings-delete-state").text=L(localDataFailed?"local-data.failed":localDataState==ProductionLocalDataState.Ready?"local-data.ready":"local-data.unavailable");
            Visible("local-data-overlay",blocking);
            var confirmation=localDataState==ProductionLocalDataState.Confirmation&&!localDataIntentStarted;
            var retry=localDataState==ProductionLocalDataState.RetryRequired;var deleted=localDataState==ProductionLocalDataState.Deleted;
            Require<Label>("local-data-title").text=L(deleted?"local-data.deleted.title":confirmation?"local-data.confirm.title":"local-data.title");
            Require<Label>("local-data-body").text=L(deleted?"local-data.deleted.body":confirmation?"local-data.confirm.body":"local-data.progress.body");
            Require<Label>("local-data-scope").text=L("local-data.scope");
            var pendingDelete=localDataPendingAction==ProductionLocalDataAction.Confirm||localDataPendingAction==ProductionLocalDataAction.Retry||localDataState==ProductionLocalDataState.Deleting;
            Require<Label>("local-data-state").text=L(localDataReadFailed||localDataFailed?"local-data.failed":busy?pendingDelete?"local-data.deleting":"local-data.pending":retry?"local-data.retry.body":deleted?"local-data.deleted.state":confirmation?"local-data.confirm.state":localDataState==ProductionLocalDataState.Deleting?"local-data.deleting":"local-data.failed");
            localDataConfirm.style.display=confirmation?DisplayStyle.Flex:DisplayStyle.None;localDataCancel.style.display=confirmation?DisplayStyle.Flex:DisplayStyle.None;
            localDataRetry.style.display=retry?DisplayStyle.Flex:DisplayStyle.None;localDataRestart.style.display=retry||deleted?DisplayStyle.Flex:DisplayStyle.None;
            localDataRestart.text=L(retry?"local-data.reload":"local-data.restart");
            localDataConfirm.SetEnabled(ready&&!busy&&confirmation);localDataCancel.SetEnabled(ready&&!busy&&confirmation);
            localDataRetry.SetEnabled(ready&&!busy&&retry);localDataRestart.SetEnabled(ready&&!busy&&(retry||deleted));
        }
        private void OnLocalDataAttached(AttachToPanelEvent change)=>FocusLocalDataAction();
        private void FocusLocalDataAction()
        {
            if(disposed||externalSuspended||localDataReadFailed||localDataInvoking||localDataTask!=null)return;
            if(localDataState==ProductionLocalDataState.Confirmation&&!localDataIntentStarted)localDataCancel.Focus();
            else if(localDataState==ProductionLocalDataState.RetryRequired)localDataRetry.Focus();
            else if(localDataState==ProductionLocalDataState.Deleted)localDataRestart.Focus();
            else if(localDataState==ProductionLocalDataState.Ready&&settingsOpen)localDataOpen.Focus();
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
