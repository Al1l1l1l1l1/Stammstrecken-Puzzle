using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Application;
using STP.MobileServices.Google;
using STP.MobileServices.Store;
using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>Inert serialized preset of existing provider inputs. The protected build checks its actual asset and reviewed original; this asset grants no approval.</summary>
    public sealed class ProductionMobileServiceProfile : ScriptableObject
    {
        [SerializeField] private string applicationId=string.Empty;
        [SerializeField] private RuntimePlatform platform;
        [SerializeField] private string nativeAdMobAppId=string.Empty;
        [SerializeField] private string interstitialId=string.Empty;
        [SerializeField] private string rewardedId=string.Empty;
        [SerializeField] private string storeProductId=string.Empty;
        [SerializeField] private string policyRevision=string.Empty;
        [SerializeField] private string reviewedSdkRevision=string.Empty;
        [SerializeField] private string reviewedEvidenceRawSha256=string.Empty;
        [SerializeField] private string googlePlayPublicKey=string.Empty;

        public string ApplicationId=>applicationId;
        public bool IsIos=>platform==RuntimePlatform.IPhonePlayer;
        public string NativeAdMobAppId=>nativeAdMobAppId;
        public string PolicyRevision=>policyRevision;
        public string ReviewedSdkRevision=>reviewedSdkRevision;
        public string ReviewedEvidenceRawSha256=>reviewedEvidenceRawSha256;

        /// <summary>Metadata checks only, with the independently retained review hash supplied by the protected build.</summary>
        public IReadOnlyList<string> ConfigurationIssues(string expectedApplicationId,bool expectedIos,string expectedReviewRawSha256)
        {
            var issues=new List<string>();
            var expectedPlatform=expectedIos?RuntimePlatform.IPhonePlayer:RuntimePlatform.Android;
            if(applicationId!=expectedApplicationId||!Regex.IsMatch(applicationId??string.Empty,@"\A[A-Za-z][A-Za-z0-9_]*(?:\.[A-Za-z][A-Za-z0-9_]*)+\z"))issues.Add("PRODUCTION_SERVICE_APPLICATION_ID_INVALID");
            if(platform!=expectedPlatform)issues.Add("PRODUCTION_SERVICE_PLATFORM_MISMATCH");
            if(!Regex.IsMatch(reviewedEvidenceRawSha256??string.Empty,@"\A[0-9a-f]{64}\z")||reviewedEvidenceRawSha256!=expectedReviewRawSha256)issues.Add("PRODUCTION_SERVICE_REVIEW_ORIGINAL_MISMATCH");
            var app=Regex.Match(nativeAdMobAppId??string.Empty,@"\Aca-app-pub-([0-9]{16})~[0-9]{10}\z");
            var interstitial=Regex.Match(interstitialId??string.Empty,@"\Aca-app-pub-([0-9]{16})/[0-9]{10}\z");
            var rewarded=Regex.Match(rewardedId??string.Empty,@"\Aca-app-pub-([0-9]{16})/[0-9]{10}\z");
            if(!app.Success||!interstitial.Success||!rewarded.Success||app.Groups[1].Value=="3940256099942544"||
                app.Groups[1].Value!=interstitial.Groups[1].Value||app.Groups[1].Value!=rewarded.Groups[1].Value||interstitialId==rewardedId)
                issues.Add("PRODUCTION_SERVICE_ADMOB_PUBLISHER_BINDING_INVALID");
            try
            {
                var ads=new GoogleAdConfiguration(applicationId??string.Empty,interstitialId??string.Empty,rewardedId??string.Empty,true);
                var store=new ProductionStoreConfiguration(new StoreConfiguration(applicationId??string.Empty,storeProductId,expectedIos?"AppleAppStore":"GooglePlay",true,policyRevision,googlePlayPublicKey));
                var readiness=new IapPrivacyReadiness(policyRevision,reviewedSdkRevision,reviewedEvidenceRawSha256??string.Empty);
                issues.AddRange(MobileCandidateServices.ProductionConfigurationIssues(expectedApplicationId,expectedPlatform,ads,store,readiness));
            }
            catch(ArgumentException){issues.Add("PRODUCTION_SERVICE_EXISTING_CONFIGURATION_INVALID");}
            return issues.Distinct(StringComparer.Ordinal).ToArray();
        }

        /// <summary>Revalidates serialized metadata after scene load. Construction remains entirely local and starts no SDK.</summary>
        internal bool TryRead(string installedApplicationId,RuntimePlatform installedPlatform,out GoogleAdConfiguration? ads,
            out ProductionStoreConfiguration? store,out IapPrivacyReadiness? readiness)
        {
            ads=null;store=null;readiness=null;
            if(installedPlatform!=RuntimePlatform.Android&&installedPlatform!=RuntimePlatform.IPhonePlayer)return false;
            if(ConfigurationIssues(installedApplicationId,installedPlatform==RuntimePlatform.IPhonePlayer,reviewedEvidenceRawSha256).Count!=0)return false;
            ads=new GoogleAdConfiguration(applicationId,interstitialId,rewardedId,true);
            store=new ProductionStoreConfiguration(new StoreConfiguration(applicationId,storeProductId,IsIos?"AppleAppStore":"GooglePlay",true,policyRevision,googlePlayPublicKey));
            readiness=new IapPrivacyReadiness(policyRevision,reviewedSdkRevision,reviewedEvidenceRawSha256);
            return true;
        }
    }
}
