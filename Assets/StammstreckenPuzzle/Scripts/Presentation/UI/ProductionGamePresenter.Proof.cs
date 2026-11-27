using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    public sealed partial class ProductionGamePresenter
    {
        private void Celebrate(int stars,long reward)
        {
            var sheet=Require<VisualElement>("result-panel");
            var strip=Require<VisualElement>("result-stamps");strip.Clear();
            var proof=resultAttempt;
            if(!reducedMotion){sheet.style.translate=new Translate(0,40);sheet.schedule.Execute(()=>sheet.style.translate=new Translate(0,0)).StartingIn(20);}
            for(int i=1;i<=Mathf.Clamp(stars,0,3);i++){
                var stamp=new Label(new string('★',i)+"\n"+W("stamp-"+i));
                stamp.AddToClassList("rubber-stamp");strip.Add(stamp);
                if(reducedMotion)continue;
                stamp.style.opacity=0;
                stamp.schedule.Execute(()=>{
                    if(disposed||proof!=resultAttempt)return;
                    stamp.style.opacity=1;stamp.style.scale=new Scale(new Vector3(1.6f,1.6f,1));sound?.Invoke("Stamp");
                    stamp.schedule.Execute(()=>stamp.style.scale=new Scale(new Vector3(.95f,.95f,1))).StartingIn(32);
                    stamp.schedule.Execute(()=>stamp.style.scale=new Scale(Vector3.one)).StartingIn(150);
                }).StartingIn(120+(i-1)*160);
            }
            if(reward<=0||reducedMotion)return;
            var balance=Require<Label>("result-balance");
            var coin=new Label("+"+reward+" GP");coin.pickingMode=PickingMode.Ignore;coin.AddToClassList("coin");sheet.Add(coin);
            float start=Time.unscaledTime;
            var tick=coin.schedule.Execute(()=>{
                float t=Mathf.Clamp01((Time.unscaledTime-start)/.65f);
                var target=sheet.WorldToLocal(balance.worldBound.center);
                coin.style.left=Mathf.Lerp(sheet.contentRect.width*.22f,target.x,t);
                coin.style.top=Mathf.Lerp(sheet.contentRect.height*.7f,target.y,t)-28*Mathf.Sin(t*Mathf.PI);
                balance.text=L("result.balance",balanceAtStart+(long)(t*reward));
                if(t>=1||resultAttempt!=proof)coin.RemoveFromHierarchy();
            });tick.Every(16);
        }
        private void ShareProof()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(UnityEngine.Application.platform!=RuntimePlatform.Android||worldCamera==null||view==null||
                view.Phase!=STP.Application.DevelopmentPreviewPhase.Results||programme==null)return;
            using var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity=player.GetStatic<AndroidJavaObject>("currentActivity");
            using var sharing=new AndroidJavaClass("com.stp.railway.PunctualityProof");
            var stars=Mathf.Clamp(model?.State.Progress?.SelectedStars??0,0,3);
            sharing.CallStatic("share",activity,W("certificate"),Require<Label>("puzzle-context").text,
                view.Width+"×"+view.Height,Require<Label>("result-time").text,new string('★',stars)+"  "+W("stamp-"+stars),
                programme.Voice.proofOfPunctuality.statement,W("share-title"));
#endif
        }
    }
}
