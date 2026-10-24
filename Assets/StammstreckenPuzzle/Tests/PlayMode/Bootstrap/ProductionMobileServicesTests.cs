using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.MobileServices.Google;
using STP.MobileServices.Store;
using UnityEngine;

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>Controlled ContractTest/FIXTURE_ONLY pipeline probes. No production source, native SDK, legal or store attestation.</summary>
    public sealed class ProductionMobileServicesTests
    {
        private sealed class Owner : ILocalSaveDispatcher
        {
            private readonly int thread = Thread.CurrentThread.ManagedThreadId;
            private readonly ConcurrentQueue<(Action, TaskCompletionSource<bool>)> queue = new ConcurrentQueue<(Action, TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action)
            {
                if (Thread.CurrentThread.ManagedThreadId == thread) { action(); return Task.CompletedTask; }
                var ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously); queue.Enqueue((action, ready)); return ready.Task;
            }
            internal void Drain() { while (queue.TryDequeue(out var item)) { try { item.Item1(); item.Item2.TrySetResult(true); } catch (Exception error) { item.Item2.TrySetException(error); } } }
            internal void Until(Func<bool> done) { var elapsed = Stopwatch.StartNew(); while (!done()) { Drain(); if (elapsed.ElapsedMilliseconds > 10000) Assert.Fail("Controlled owner queue did not settle"); Thread.Sleep(1); } Drain(); }
            internal T Pump<T>(Task<T> task) { Until(() => task.IsCompleted); return task.GetAwaiter().GetResult(); }
            internal void Pump(Task task) { Until(() => task.IsCompleted); task.GetAwaiter().GetResult(); }
        }
        private sealed class Clock : ILocalSessionClock { public string UtcNow => "2026-10-21T00:00:00Z"; public long MonotonicMilliseconds => 0; }
        private sealed class Repository : ILocalProfileRepository
        {
            internal SaveSnapshot<LocalProfileState> Current;
            internal int Commits;
            internal Repository(SaveSnapshot<LocalProfileState> initial) { Current = initial; }
            private SaveRepositoryObservation Observation() => new SaveRepositoryObservation(Current.Profile, Current.Catalog, new[] {
                new SaveCandidateProvenance(SaveCandidateKind.Main,SaveCandidateStatus.Valid,Current.Generation,new string('a',64),new string('b',64)),
                new SaveCandidateProvenance(SaveCandidateKind.Backup,SaveCandidateStatus.Missing,null,null,null),new SaveCandidateProvenance(SaveCandidateKind.Pending,SaveCandidateStatus.Missing,null,null,null) });
            public Task<SaveLoadResult<LocalProfileState>> LoadAsync(CancellationToken ct) => Task.FromResult(SaveLoadResult<LocalProfileState>.Success(Current, Observation()));
            public Task<SaveCommitResult<LocalProfileState>> CommitAsync(SaveSnapshot<LocalProfileState> offered, long expected, CancellationToken ct)
            {
                if (Current.Generation != expected || offered.Generation != expected + 1) return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "FIXTURE_CAS"));
                Current = offered; Commits++; return Task.FromResult(SaveCommitResult<LocalProfileState>.Success(offered));
            }
            public Task<SaveLoadResult<LocalProfileState>> RecoverAsync(SaveRecoveryRequest request, CancellationToken ct) => LoadAsync(ct);
            public Task<SaveCommitResult<LocalProfileState>> CreateProfileAsync(LocalProfileState state, CancellationToken ct) => throw new NotSupportedException();
            public Task<SaveCommitResult<LocalProfileState>> ConfirmedResetCorruptProfileAsync(SaveRepositoryObservation observation, LocalProfileState state, CancellationToken ct) => throw new NotSupportedException();
        }
        private sealed class Google : IGoogleMobileRuntime
        {
            internal int Updates, Loads, Initializations;
            public void RefreshConsent(Action<bool,bool,bool> done) { Updates++; done(true,true,true); }
            public void ShowPrivacyOptions(Action<bool,bool,bool> done) => done(true,false,true);
            public void InitializeAds(Action<bool> done) { Initializations++; done(true); }
            public void Load(MobileAdKind kind, Action<IGoogleLoadedAd?> done) { Loads++; done(null); }
        }
        private sealed class Store : IUnityStoreRuntime
        {
            private readonly Repository repo;
            internal int Connects, Purchases, Confirms;
            internal bool Deferred;
            internal Store(Repository repository) { repo = repository; }
            public bool Supported => true;
            public event Action<StoreTransaction>? TransactionReceived;
            public event Action<MobileServiceStatus>? PurchaseStopped { add { } remove { } }
            public void ConnectAndFetch(Action<StoreProduct?> done) { Connects++; done(new StoreProduct("fixture-remove-ads", "fixture-localized-price", "fixture product")); }
            public void Purchase() { Purchases++; if (!Deferred) Emit(); }
            internal void Emit() => TransactionReceived?.Invoke(new StoreTransaction("fixture-native-transaction", "fixture-remove-ads", false, this));
            public void Restore(Action<bool> done) => done(true);
            public void Verify(StoreTransaction transaction, Action<VerifiedStoreTransaction?> done) => done(new VerifiedStoreTransaction(transaction.NativeId,"Sandbox",false));
            public void Confirm(StoreTransaction transaction, Action<bool> done)
            {
                Assert.That(repo.Current.Payload.Entitlements.Records.Single().Owned, Is.True);
                Assert.That(repo.Current.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State, Is.EqualTo("GRANTED_NOT_FINALIZED"));
                Confirms++; done(true);
            }
            public void Dispose() { }
        }
        private sealed class Fixture : IDisposable
        {
            internal readonly Owner Owner = new Owner(); internal readonly Clock Clock = new Clock();
            internal readonly Repository Repo; internal readonly Google Google = new Google(); internal readonly Store Store;
            internal readonly MobileCandidateServices Services; internal int GoogleFactories, StoreFactories, WriterEntries;
            private readonly SemaphoreSlim writer = new SemaphoreSlim(1,1);
            internal Fixture()
            {
                var type = Assembly.Load("STP.Tests.Application.EditMode").GetType("STP.Tests.Application.EditMode.LocalSaveContractFixtures",true)!;
                var initial = (SaveSnapshot<LocalProfileState>)type.GetMethod("CreateContractSnapshot",BindingFlags.Static|BindingFlags.Public)!.Invoke(null,new object[]{1L,false})!;
                Repo = new Repository(initial); Store = new Store(Repo);
                Func<Func<CancellationToken,Task<SaveCommitResult<LocalProfileState>>>,CancellationToken,Task<SaveCommitResult<LocalProfileState>>> shared = async (action,ct) =>
                { await writer.WaitAsync(ct).ConfigureAwait(false); try { WriterEntries++; return await action(ct).ConfigureAwait(false); } finally { writer.Release(); } };
                var method = typeof(MobileCandidateServices).GetMethod("CreateForContractTests",BindingFlags.Static|BindingFlags.NonPublic)!;
                Services = (MobileCandidateServices)method.Invoke(null,new object[]{Repo,initial,Owner,Clock,shared,
                    new Func<AdRewardEvidence,CancellationToken,Task<SaveCommitResult<LocalProfileState>>>((e,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Success(Repo.Current))),
                    new Func<GoogleAdConfiguration,IGoogleMobileRuntime>(c=>{GoogleFactories++;Assert.That(c.Production,Is.False);return Google;}),
                    new Func<StoreConfiguration,IUnityStoreRuntime>(c=>{StoreFactories++;Assert.That(c.ApplicationId,Is.EqualTo("contract.stp.qa"));Assert.That(c.Production,Is.False);return Store;})})!;
            }
            internal void Configure()
            {
                var method=typeof(MobileCandidateServices).GetMethod("ConfigureContractTestStore",BindingFlags.Instance|BindingFlags.NonPublic)!;
                method.Invoke(Services,new object[]{new StoreConfiguration("contract.stp.qa","fixture-remove-ads","GooglePlay",false,"fixture-policy"),new IapPrivacyReadiness("fixture-policy","unity-iap-5.4.3",new string('a',64))});
            }
            public void Dispose() { Services.Dispose(); Owner.Pump(Services.PendingCallbacks); writer.Dispose(); }
        }
        [Test]
        public void MissingProductionInputsHaveConcretePureDiagnosticsAndNeverSelectQaDefaults()
        {
            var problems=MobileCandidateServices.ProductionConfigurationIssues(string.Empty,RuntimePlatform.WindowsEditor,null,null,null);
            Assert.That(problems,Does.Contain("MOBILE_PRODUCTION_APPLICATION_ID_REQUIRED"));Assert.That(problems,Does.Contain("MOBILE_PRODUCTION_PLATFORM_REQUIRED"));
            Assert.That(problems,Does.Contain("MOBILE_PRODUCTION_AD_UNITS_REQUIRED"));Assert.That(problems,Does.Contain("IAP_PRODUCTION_CONFIGURATION_REQUIRED"));Assert.That(problems,Does.Contain("IAP_PRIVACY_READINESS_REQUIRED"));
            var qa=GoogleAdConfiguration.TestAds(MobileApplicationProfile.CandidateApplicationId,false);Assert.That(qa.Production,Is.False);
            Assert.That(MobileCandidateServices.ProductionConfigurationIssues(qa.ApplicationId,RuntimePlatform.Android,qa,null,null),Does.Contain("MOBILE_PRODUCTION_AD_PROFILE_MISMATCH"));
        }
        [TestCase("\n")][TestCase(" ")][TestCase("\t")]
        public void PureBuildCheckRejectsTrailingAdUnitCharactersUsingOnlyOfficialTestIds(string suffix)
        {
            var qa=GoogleAdConfiguration.TestAds(MobileApplicationProfile.CandidateApplicationId,false);
            var malformed=new GoogleAdConfiguration(qa.ApplicationId,qa.InterstitialId+suffix,qa.RewardedId+suffix,false);
            Assert.That(MobileCandidateServices.ProductionConfigurationIssues(qa.ApplicationId,RuntimePlatform.Android,malformed,null,null),Does.Contain("MOBILE_PRODUCTION_AD_UNITS_INVALID"));
        }
        [Test]
        [TestCase(RuntimePlatform.Android,RuntimePlatform.Android,"contract.stp.qa",true)]
        [TestCase(RuntimePlatform.Android,RuntimePlatform.IPhonePlayer,"contract.stp.qa",false)]
        [TestCase(RuntimePlatform.IPhonePlayer,RuntimePlatform.IPhonePlayer,"other.stp.qa",false)]
        [TestCase(RuntimePlatform.IPhonePlayer,RuntimePlatform.WindowsEditor,"contract.stp.qa",false)]
        [TestCase(RuntimePlatform.WindowsEditor,RuntimePlatform.WindowsEditor,"contract.stp.qa",false)]
        public void ExactMobileRuntimeBoundaryUsesFixtureIdentitiesWithoutInvokingUnity(RuntimePlatform expected,RuntimePlatform actual,string identifier,bool allowed)
        {
            var method=typeof(MobileCandidateServices).GetMethod("RuntimeProfileMatches",BindingFlags.Static|BindingFlags.NonPublic)!;
            Assert.That((bool)method.Invoke(null,new object[]{"contract.stp.qa",expected,identifier,actual})!,Is.EqualTo(allowed));
        }
        [TestCase(false)][TestCase(true)]
        public void ControlledFactoryRejectsMismatchedPayloadProfileOrAggregateBeforeAnyFactory(bool aggregate)
        {
            using var f=new Fixture();var current=f.Repo.Current;
            var offered=new SaveSnapshot<LocalProfileState>(aggregate?current.Profile:new SaveProfileIdentity("contract-save-v1","other",CatalogUse.ContractTest),
                aggregate?new CatalogIdentity("fixture-other",1,new string('b',64),CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly):current.Catalog,current.Generation,current.Payload);
            var method=typeof(MobileCandidateServices).GetMethod("CreateForContractTests",BindingFlags.Static|BindingFlags.NonPublic)!;
            var failure=Assert.Throws<TargetInvocationException>(()=>method.Invoke(null,new object[]{f.Repo,offered,f.Owner,f.Clock,
                new Func<Func<CancellationToken,Task<SaveCommitResult<LocalProfileState>>>,CancellationToken,Task<SaveCommitResult<LocalProfileState>>>((a,ct)=>a(ct)),
                new Func<AdRewardEvidence,CancellationToken,Task<SaveCommitResult<LocalProfileState>>>((e,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Success(current))),
                new Func<GoogleAdConfiguration,IGoogleMobileRuntime>(c=>throw new AssertionException("Unbound factory invoked")),new Func<StoreConfiguration,IUnityStoreRuntime>(c=>throw new AssertionException("Unbound factory invoked"))}));
            Assert.That(failure!.InnerException,Is.TypeOf<ArgumentException>());Assert.That(f.Repo.Commits,Is.Zero);Assert.That(f.StoreFactories,Is.Zero);
        }
        [Test]
        public void QueuedWorkerCannotConstructAProviderOnTheWrongOwner()
        {
            using var f=new Fixture();var method=typeof(MobileCandidateServices).GetMethod("RequireInlineOwner",BindingFlags.Static|BindingFlags.NonPublic)!;
            var rejected=Task.Run(()=>Assert.Throws<TargetInvocationException>(()=>method.Invoke(null,new object[]{f.Owner,Thread.CurrentThread.ManagedThreadId})));
            Assert.That(rejected.Wait(2000),Is.True);Assert.That(rejected.Result!.InnerException!.Message,Is.EqualTo("MOBILE_DISPATCH_OWNER_REQUIRED"));
            f.Owner.Drain();Assert.That(f.GoogleFactories,Is.EqualTo(1));Assert.That(f.Google.Updates,Is.Zero);Assert.That(f.StoreFactories,Is.Zero);Assert.That(f.Repo.Commits,Is.Zero);
        }
        [Test]
        public void IncompatibleReadinessCannotConstructEvenTheControlledStoreRuntime()
        {
            using var f=new Fixture();var method=typeof(MobileCandidateServices).GetMethod("ConfigureContractTestStore",BindingFlags.Instance|BindingFlags.NonPublic)!;
            var error=Assert.Throws<TargetInvocationException>(()=>method.Invoke(f.Services,new object[]{new StoreConfiguration("contract.stp.qa","fixture-remove-ads","GooglePlay",false,"fixture-policy"),new IapPrivacyReadiness("other-policy","unity-iap-5.4.3",new string('a',64))}));
            Assert.That(error!.InnerException!.Message,Is.EqualTo("IAP_REVIEWED_INPUTS_MISMATCH"));Assert.That(f.StoreFactories,Is.Zero);Assert.That(f.Services.Purchases,Is.Null);
        }
        [Test]
        public void ProductionEntryRejectsTheActualContractSnapshotBeforeTouchingRuntimeOrNativeFactories()
        {
            using var f=new Fixture();int prior=f.GoogleFactories;
            Assert.Throws<ArgumentException>(()=>MobileCandidateServices.CreateProduction(f.Repo,f.Repo.Current,f.Owner,f.Clock,(a,ct)=>a(ct),
                (e,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Success(f.Repo.Current)),GoogleAdConfiguration.TestAds("contract.stp.qa",false),RuntimePlatform.Android));
            Assert.That(f.GoogleFactories,Is.EqualTo(prior));Assert.That(f.StoreFactories,Is.Zero);Assert.That(f.Google.Updates,Is.Zero);Assert.That(f.Repo.Commits,Is.Zero);
            Assert.Throws<ArgumentException>(()=>new ProductionStoreConfiguration(new StoreConfiguration("contract.stp.qa","fixture-remove-ads","GooglePlay",false,"fixture-policy")));
        }
        [Test]
        public void ControlledCompositionAndStoreMetadataStartNoSdkAndConsentUsesTheSharedDurableProtocol()
        {
            using var f=new Fixture();Assert.That(f.GoogleFactories,Is.EqualTo(1));Assert.That(f.Google.Updates,Is.Zero);Assert.That(f.Google.Initializations,Is.Zero);Assert.That(f.StoreFactories,Is.Zero);Assert.That(f.Services.Capabilities.CanRequestAds,Is.False);
            f.Configure();Assert.That(f.StoreFactories,Is.Zero);Assert.That(f.Services.Product,Is.Null);
            var consent=f.Owner.Pump(f.Services.RefreshConsentAsync(CancellationToken.None));Assert.That(consent.Status,Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(f.Repo.Commits,Is.EqualTo(2));Assert.That(f.WriterEntries,Is.EqualTo(1));Assert.That(f.Services.Capabilities.CanRequestAds,Is.True);Assert.That(f.Google.Initializations,Is.Zero);
            var revoked=f.Owner.Pump(f.Services.RevokeConsentAsync(CancellationToken.None));Assert.That(revoked.Succeeded,Is.True);Assert.That(f.Services.Capabilities.CanRequestAds,Is.False);
        }
        [Test]
        public void ControlledPriceLoadIsSeparateFromPurchaseAndNativeFinishRequiresDurableOwnedState()
        {
            using var f=new Fixture();f.Configure();var loaded=f.Owner.Pump(f.Services.LoadStorePriceAsync(CancellationToken.None));Assert.That(loaded.Status,Is.EqualTo(MobileServiceStatus.Succeeded));Assert.That(f.Store.Purchases,Is.Zero);Assert.That(f.StoreFactories,Is.EqualTo(1));
            Assert.That(f.Services.Product,Is.SameAs(loaded.Value));var bought=f.Owner.Pump(f.Services.PurchaseRemoveAdsAsync(loaded.Value!,CancellationToken.None));
            Assert.That(bought.Status,Is.EqualTo(MobileServiceStatus.Succeeded));Assert.That(f.Store.Confirms,Is.EqualTo(1));Assert.That(f.WriterEntries,Is.EqualTo(1));Assert.That(f.Repo.Current.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State,Is.EqualTo("FINALIZED"));
            var duplicate=f.Owner.Pump(f.Services.PurchaseRemoveAdsAsync(loaded.Value!,CancellationToken.None));Assert.That(duplicate.Status,Is.EqualTo(MobileServiceStatus.NotAllowed));Assert.That(f.Store.Purchases,Is.EqualTo(1));
            Assert.That(f.Owner.Pump(f.Services.RestoreAsync(CancellationToken.None)).Status,Is.EqualTo(MobileServiceStatus.Succeeded));Assert.That(f.Repo.Current.Payload.Entitlements.Records.Single().Owned,Is.True);
        }
        [Test]
        public void CancelledCallerStillRoutesLateVerifiedContractEvidenceThroughTheSameWriter()
        {
            using var f=new Fixture();f.Configure();var loaded=f.Owner.Pump(f.Services.LoadStorePriceAsync(CancellationToken.None));f.Store.Deferred=true;using var cancelled=new CancellationTokenSource();
            var purchase=f.Services.PurchaseRemoveAdsAsync(loaded.Value!,cancelled.Token);f.Owner.Until(()=>f.Store.Purchases==1);cancelled.Cancel();
            Assert.That(f.Owner.Pump(purchase).Status,Is.EqualTo(MobileServiceStatus.Cancelled));Assert.That(f.Store.Confirms,Is.Zero);
            f.Store.Emit();f.Owner.Pump(f.Services.PendingCallbacks);Assert.That(f.Store.Confirms,Is.EqualTo(1));Assert.That(f.WriterEntries,Is.EqualTo(1));Assert.That(f.Repo.Current.Payload.Entitlements.Records.Single().Owned,Is.True);
        }
    }
}
