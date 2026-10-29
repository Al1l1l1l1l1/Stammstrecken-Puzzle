using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>Real common Root/UI/repository in an owned ContractTest root. No actual participant, production or native claim.</summary>
    public sealed class LocalProfileDeletionIntegrationTests
    {
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private const string Fixtures="Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/RuntimeFixtures/";
        private static readonly string[] Paths={"Calibration.json","Campaign.json","CampaignCompletion.json","CampaignText.json","Cosmetics.json","EndlessCompletion.json","Profiles/dienstbeginn.json","Profiles/grosslage.json","Profiles/regelbetrieb.json","Profiles/stoerungslage.json","Public.json","RailwayAssets.json"};
        private GameObject? host;
        private MobilePreviewInstaller installer=null!;
        private PanelSettings panel=null!;
        private RenderTexture? texture;
        private string ownRoot=string.Empty;
        private bool ownsRoot;
        private TaskCompletionSource<bool>? drainBarrier;
        private ProductionGameComposition Runtime=>installer.RuntimeComposition??throw new InvalidOperationException("FIXTURE_RUNTIME_REQUIRED");
        private SavedPublic240ReviewSession Model=>installer.Model??throw new InvalidOperationException("REAL_SAVED_MODEL_REQUIRED");
        private ProductionLocalDataState State=>Read<ProductionLocalDataState>(installer,"localDataState");
        private Task DeletionTask=>Read<Task>(installer,"localDataOperation");
        private string ProfilePath=>Runtime.Repository.ProfileDirectory;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            ownsRoot=false;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            ownRoot=Path.Combine(Path.GetTempPath(),"d"+Guid.NewGuid().ToString("N").Substring(0,6));Assert.That(Directory.Exists(ownRoot),Is.False);ownsRoot=true;
            panel=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>(Ui+"Public240ReviewPanelSettings.asset"));panel.referenceResolution=new Vector2Int(390,844);panel.match=0;
            if(SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null){texture=new RenderTexture(390,844,24,RenderTextureFormat.ARGB32);Assert.That(texture.Create(),Is.True);panel.targetTexture=texture;}
            yield return null;
#else
            Assert.Ignore("Explicit ContractTest selection and owned filesystem injection are Editor tests only.");yield break;
#endif
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // Never strand an actual Root-owned drain if any assertion fails.
            drainBarrier?.TrySetResult(true);yield return StopInstaller();
            if(texture!=null){texture.Release();UnityEngine.Object.Destroy(texture);}if(panel!=null)UnityEngine.Object.Destroy(panel);
            if(ownsRoot&&!string.IsNullOrEmpty(ownRoot)&&Directory.Exists(ownRoot))
            {
                var absolute=Path.GetFullPath(ownRoot);var prefix=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
                Assert.That(absolute.StartsWith(prefix,StringComparison.OrdinalIgnoreCase),Is.True);Assert.That(Path.GetFileName(absolute),Does.StartWith("d"));Assert.That(Path.GetFileName(absolute).Length,Is.EqualTo(7));
                Directory.Delete(absolute,true);
            }
            ownRoot=string.Empty;ownsRoot=false;drainBarrier=null;
        }

        [UnityTest]
        public IEnumerator SettingsOpenCancel_ActualProfileAndArchivesRemain_AndNoIntentOrMarkerIsCreated()
        {
            yield return Mount();yield return CreateProfile();var profile=ProfilePath;var codec=Codec(Runtime.Repository);
            string save=Path.Combine(profile,"save.json"),archive=Path.Combine(profile,"recovery","kept-original.fixture");Directory.CreateDirectory(Path.GetDirectoryName(archive)!);File.WriteAllBytes(archive,new byte[]{4,6,2});
            byte[] payload=codec.PayloadBytes(codec.Decode(File.ReadAllBytes(save)).Snapshot!.Payload);
            Submit("nav-profile");Submit("profile-settings");yield return null;Assert.That(State,Is.EqualTo(ProductionLocalDataState.Ready));Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null);
            Submit("settings-delete-local");yield return LocalState(ProductionLocalDataState.Confirmation);
            Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null,"Dialog observation is not deletion consent or a minted intent.");
            Assert.That(File.Exists(Path.Combine(profile,"profile-lifecycle-v1.json")),Is.False);Assert.That(File.ReadAllBytes(archive),Is.EqualTo(new byte[]{4,6,2}));
            yield return CaptureConfirmation();
            Submit("local-data-cancel");yield return LocalState(ProductionLocalDataState.Ready);
            Assert.That(Find("settings-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));Assert.That(File.Exists(save),Is.True);
            Assert.That(codec.PayloadBytes(codec.Decode(File.ReadAllBytes(save)).Snapshot!.Payload),Is.EqualTo(payload));Assert.That(File.Exists(Path.Combine(profile,"profile-lifecycle-v1.json")),Is.False);
            Submit("settings-close");yield return Idle();Assert.That(installer.Presenter.IsProductModalOpen,Is.False);Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null);
        }

        [UnityTest]
        public IEnumerator Confirm_ActualUiDeletesOnlyItsBoundProfile_ClosesGameplayAndOldWriter_BeforeAnyFreshBinder()
        {
            yield return Mount();yield return CreateProfile();Submit("home-play");yield return Idle();
            var heldView=Model.PuzzleView!;Submit("tool-1");Submit(ProductionPuzzleGrid.CellName(1,1));yield return Idle();Submit("puzzle-back");yield return Idle();
            string profile=ProfilePath;var oldRepository=Runtime.Repository;string archive=Path.Combine(profile,"recovery","owned.fixture");Directory.CreateDirectory(Path.GetDirectoryName(archive)!);File.WriteAllText(archive,"owned original");
            string sibling=Path.Combine(Path.GetDirectoryName(profile)!,"other-profile-control","original.fixture");Directory.CreateDirectory(Path.GetDirectoryName(sibling)!);File.WriteAllText(sibling,"outside profile original");
            string human=Path.Combine(ownRoot,"human-calibration-control","original.fixture");Directory.CreateDirectory(Path.GetDirectoryName(human)!);File.WriteAllText(human,"separate observation original");
            Submit("nav-profile");Submit("profile-settings");Submit("settings-delete-local");yield return LocalState(ProductionLocalDataState.Confirmation);
            var heldCell=heldView.CellAt(1,1);Submit("local-data-confirm");
            SendEvenIfDisabled("local-data-confirm");SendEvenIfDisabled("home-play");SendEvenIfDisabled(ProductionPuzzleGrid.CellName(1,1));SendEvenIfDisabled("nav-home");
            yield return LocalState(ProductionLocalDataState.Deleted);
            Assert.That(installer.Presenter.IsProductModalOpen,Is.True);Assert.That(heldView.CellAt(1,1),Is.EqualTo(heldCell));
            var intent=Read<ProfileDeletionIntent>(installer,"deletionIntent");Assert.That(intent,Is.Not.Null);Assert.That(intent.Observation.Profile,Is.EqualTo(Runtime.Profile));
            string[] remaining=Directory.GetFiles(profile,"*",SearchOption.AllDirectories);Assert.That(remaining.Select(Path.GetFileName),Is.EquivalentTo(new[]{"profile-lifecycle-v1.json"}),"No save, archive, diagnostic or stage can survive the confirmed scope.");
            var marker=new ProfileLifecycleCodec(new CanonicalJsonAdapter()).Decode(File.ReadAllBytes(remaining.Single()));Assert.That(marker.DeletePending,Is.False);Assert.That(marker.WriterEpoch,Is.EqualTo(intent.TargetEpoch));
            Assert.That(File.ReadAllText(sibling),Is.EqualTo("outside profile original"));Assert.That(File.ReadAllText(human),Is.EqualTo("separate observation original"));
            var staleLoad=oldRepository.LoadAsync(CancellationToken.None);yield return Await(staleLoad);Assert.That(staleLoad.Result.ErrorCode,Is.EqualTo("SAVE_PROFILE_REPOSITORY_RETIRED"));
            Assert.That(Button("local-data-restart").enabledInHierarchy,Is.True);Assert.That(Find("local-data-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));
            yield return null;yield return null;Assert.That(Directory.GetFiles(profile,"save*.json",SearchOption.AllDirectories),Is.Empty,"No automatic new profile/binder after deletion.");
        }

        [UnityTest]
        public IEnumerator Boot_ResumesOnlyActuallyConfirmedPendingMarker_BeforeLoadingGameplayOrStartingAnotherWriter()
        {
            yield return Mount();yield return CreateProfile();var prior=Runtime;string profile=ProfilePath;var codec=Codec(prior.Repository);yield return StopInstaller();
            var physical=new SaveFileSystem(ownRoot,prior.Profile,prior.Progress.CatalogSet.AggregateCatalog,p=>{if(p=="delete.pending-verified")throw new ControlledProcessStop();});
            var staged=new AtomicSaveRepository(physical,codec,new FixtureClock(),"fixture","root-boot-delete-fixture");
            var inspect=staged.InspectDeletionAsync(CancellationToken.None);yield return Await(inspect);var intent=staged.CreateDeletionIntent(inspect.Result);
            var confirmed=staged.ConfirmDeleteAsync(intent,CancellationToken.None);
            yield return Until(()=>confirmed.IsCompleted,"controlled interruption after real confirmed pending Flush/Reread");
            Assert.That(confirmed.IsFaulted,Is.True);Assert.That(confirmed.Exception!.GetBaseException(),Is.TypeOf<ControlledProcessStop>());
            var pending=new ProfileLifecycleCodec(new CanonicalJsonAdapter()).Decode(File.ReadAllBytes(Path.Combine(profile,"profile-lifecycle-v1.json")));Assert.That(pending.DeletePending,Is.True);Assert.That(pending.WriterEpoch,Is.EqualTo(intent.TargetEpoch));Assert.That(File.Exists(Path.Combine(profile,"save.json")),Is.True);
            yield return Mount();yield return LocalState(ProductionLocalDataState.Deleted);
            Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null,"Boot continuation never creates a new explicit confirmation/token.");
            var result=new ProfileLifecycleCodec(new CanonicalJsonAdapter()).Decode(File.ReadAllBytes(Path.Combine(profile,"profile-lifecycle-v1.json")));Assert.That(result.WriterEpoch,Is.EqualTo(intent.TargetEpoch));Assert.That(result.DeletePending,Is.False);
            Assert.That(Directory.GetFiles(profile,"*",SearchOption.AllDirectories).Select(Path.GetFileName),Is.EquivalentTo(new[]{"profile-lifecycle-v1.json"}));
            Assert.That(installer.Presenter.IsProductModalOpen,Is.True);Assert.That(Find("local-data-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));Assert.That(Model.PuzzleView,Is.Null);
        }

        [UnityTest]
        public IEnumerator RetryAfterRealDrainTimeout_PreservesExactIntentAndEpoch_AndCannotDeleteBesidePendingWork()
        {
            drainBarrier=new TaskCompletionSource<bool>();yield return Mount(drainBarrier.Task);yield return CreateProfile();var profile=ProfilePath;
            Submit("nav-profile");Submit("profile-settings");Submit("settings-delete-local");yield return LocalState(ProductionLocalDataState.Confirmation);Submit("local-data-confirm");
            yield return Until(()=>Read<ProfileDeletionIntent?>(installer,"deletionIntent")!=null,"one actual Root confirmation intent");var held=Read<ProfileDeletionIntent>(installer,"deletionIntent");
            SendEvenIfDisabled("local-data-confirm");yield return LocalState(ProductionLocalDataState.RetryRequired,12);
            Assert.That(drainBarrier.Task.IsCompleted,Is.False);Assert.That(File.Exists(Path.Combine(profile,"save.json")),Is.True);Assert.That(File.Exists(Path.Combine(profile,"profile-lifecycle-v1.json")),Is.False,"A five-second UI timeout cannot authorize deletion beside unfinished owned work.");
            Assert.That(Read<ProfileDeletionIntent>(installer,"deletionIntent"),Is.SameAs(held));Assert.That(installer.Presenter.IsProductModalOpen,Is.True);Assert.That(Button("local-data-cancel").enabledInHierarchy,Is.False);
            drainBarrier.TrySetResult(true);Submit("local-data-retry");SendEvenIfDisabled("local-data-retry");SendEvenIfDisabled("local-data-confirm");yield return LocalState(ProductionLocalDataState.Deleted);
            Assert.That(Read<ProfileDeletionIntent>(installer,"deletionIntent"),Is.SameAs(held));var marker=new ProfileLifecycleCodec(new CanonicalJsonAdapter()).Decode(File.ReadAllBytes(Path.Combine(profile,"profile-lifecycle-v1.json")));
            Assert.That(marker.WriterEpoch,Is.EqualTo(held.TargetEpoch));Assert.That(marker.DeletePending,Is.False);Assert.That(File.Exists(Path.Combine(profile,"save.json")),Is.False);
        }

        [UnityTest]
        public IEnumerator InvalidBootMetadata_StaysClosedUntilARealStartupOpenCancelOrFreshConfirmation()
        {
            yield return Mount();yield return CreateProfile();string profile=ProfilePath;yield return StopInstaller();File.WriteAllText(Path.Combine(profile,"profile-lifecycle-v1.json"),"{invalid fixture");
            byte[] original=File.ReadAllBytes(Path.Combine(profile,"save.json"));yield return Mount();
            Assert.That(State,Is.EqualTo(ProductionLocalDataState.Ready));Assert.That(Model.PuzzleView,Is.Null);Assert.That(Model.State.CanMutate,Is.False);
            Assert.That(installer.Presenter.HasBoundModel,Is.False,"Invalid lifecycle cannot be projected as a loaded Saved profile.");Assert.That(Runtime.Persisted.Snapshot,Is.Null);
            Assert.That(Find("startup-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));Assert.That(installer.Presenter.StartupPhase,Is.EqualTo(SavedStartupPhase.Failed));
            Submit("startup-retry");yield return Await(installer.InitializeTask);yield return null;
            Assert.That(installer.Presenter.HasBoundModel,Is.False,"Explicit startup retry must inspect the still-invalid lifecycle before any profile load.");Assert.That(Runtime.Persisted.Snapshot,Is.Null);
            string trigger=Find("error-overlay").resolvedStyle.display==DisplayStyle.Flex?"error-delete-local":"startup-delete-local";
            Submit(trigger);yield return LocalState(ProductionLocalDataState.Confirmation);Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null);
            Submit("local-data-cancel");yield return LocalState(ProductionLocalDataState.Ready);Assert.That(File.ReadAllBytes(Path.Combine(profile,"save.json")),Is.EqualTo(original));Assert.That(File.ReadAllText(Path.Combine(profile,"profile-lifecycle-v1.json")),Is.EqualTo("{invalid fixture"));
            Assert.That(Read<bool>(installer,"localDataStartupBlocked"),Is.True);Assert.That(Read<bool>(installer,"localDataInputFence"),Is.True,"Cancel must preserve the actual invalid-startup gameplay fence.");
            using(var exclusive=new FileStream(Path.Combine(profile,"profile-lifecycle-v1.json"),FileMode.Open,FileAccess.Read,FileShare.None))
            {
                Submit(trigger);yield return LocalState(ProductionLocalDataState.Ready);Assert.That(DeletionTask.IsFaulted,Is.True);_ = DeletionTask.Exception;
                Assert.That(Read<bool>(installer,"localDataInputFence"),Is.True,"A real Inspect IO failure cannot reopen gameplay.");Assert.That(Runtime.Persisted.Snapshot,Is.Null);Assert.That(installer.Presenter.HasBoundModel,Is.False);
            }
            Submit(trigger);yield return LocalState(ProductionLocalDataState.Confirmation);Submit("local-data-confirm");yield return LocalState(ProductionLocalDataState.Deleted);
            Assert.That(File.Exists(Path.Combine(profile,"save.json")),Is.False);Assert.That(new ProfileLifecycleCodec(new CanonicalJsonAdapter()).Decode(File.ReadAllBytes(Path.Combine(profile,"profile-lifecycle-v1.json"))).DeletePending,Is.False);
        }

        private IEnumerator Mount(Task? barrier=null)
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            Assert.That(host,Is.Null);host=new GameObject("WP046 actual Root local profile deletion");host.SetActive(false);installer=host.AddComponent<MobilePreviewInstaller>();installer.ConfigureTestRoot(ownRoot);
            if(barrier!=null)
            {
                var hook=typeof(MobilePreviewInstaller).GetMethod("ConfigureTestLocalDataDrain",BindingFlags.Public|BindingFlags.Instance);Assert.That(hook,Is.Not.Null,"The sole approved Editor-only pre-Awake Root drain hook is required.");hook!.Invoke(installer,new object[]{barrier});
            }
            var files=Paths.ToDictionary(p=>p,p=>AssetDatabase.LoadAssetAtPath<TextAsset>(Fixtures+p),StringComparer.Ordinal);Assert.That(files.Values.All(v=>v!=null),Is.True);
            installer.ConfigureContractTestRuntime(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml"),AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui+"ProductionGame.uss"),AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"),panel,
                files,AssetDatabase.LoadAssetAtPath<TextAsset>(Fixtures+"Runtime.bindings.json"),"3baf99433918a2aa35e9edd7bdab988f9daaaf7c1e57ede5268c624e3935d9a1","3174e9d24f8ac6be040032601ba156ab51463df596f094adf04ac0c4b8dccf06","3174e9d24f8ac6be040032601ba156ab51463df596f094adf04ac0c4b8dccf06","fc0c2a3f02fcd00445a5c5a1ab1d1e8017657329c84447b619ff76c77b15a8ee");
            host.SetActive(true);installer.Document.rootVisualElement.style.width=390;installer.Document.rootVisualElement.style.height=844;yield return Await(installer.InitializeTask);yield return null;yield return null;
            Assert.That(installer.RuntimeComposition,Is.Not.Null,installer.StartupDiagnostic);Assert.That(Runtime.Profile.Use,Is.EqualTo(CatalogUse.ContractTest));Assert.That(ProfilePath.StartsWith(Path.GetFullPath(ownRoot)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),Is.True);
#else
            Assert.Ignore("Editor-only explicit fixture/runtime/owned path composition.");yield break;
#endif
        }
        [UnityTest]
        public IEnumerator StartupRetry_LoadsTheRealOriginalOnlyAfterFixtureBoundaryIsActuallyReady()
        {
            yield return Mount();yield return CreateProfile();string profile=ProfilePath;var codec=Codec(Runtime.Repository);
            byte[] expected=codec.PayloadBytes(Model.ConfirmedSnapshot!.Payload);yield return StopInstaller();
            string marker=Path.Combine(profile,"profile-lifecycle-v1.json");File.WriteAllText(marker,"{invalid fixture");yield return Mount();
            Assert.That(Runtime.Persisted.Snapshot,Is.Null);Assert.That(installer.Presenter.HasBoundModel,Is.False);Assert.That(Read<bool>(installer,"localDataInputFence"),Is.True);
            // Controlled fixture restoration of the historically absent boundary, after no UI confirmation.
            // Product code is never asked to infer or repair these invalid bytes.
            Assert.That(Path.GetFullPath(marker).StartsWith(Path.GetFullPath(ownRoot)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),Is.True);File.Delete(marker);
            Submit("startup-retry");yield return Await(installer.InitializeTask);yield return Idle();
            Assert.That(installer.Presenter.HasBoundModel,Is.True);Assert.That(Runtime.Persisted.Snapshot,Is.Not.Null);
            Assert.That(codec.PayloadBytes(Model.ConfirmedSnapshot!.Payload),Is.EqualTo(expected));Assert.That(Read<bool>(installer,"localDataInputFence"),Is.False);
            Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null);Assert.That(File.Exists(marker),Is.False);Assert.That(Model.State.CanMutate,Is.False,"A real profile load does not invent a selected active puzzle.");
        }
        private IEnumerator StopInstaller()
        {
            if(host==null)yield break;var old=installer;UnityEngine.Object.Destroy(host);host=null;yield return null;yield return Await(old.ShutdownTask);
        }
        private IEnumerator CreateProfile()
        {Assert.That(State,Is.EqualTo(ProductionLocalDataState.Ready));yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.CreateProfile,Model.State.Version)));yield return Idle();Assert.That(Model.ConfirmedSnapshot,Is.Not.Null);}
        private IEnumerator Idle()
        {yield return Until(()=>installer.PendingOperation.IsCompleted&&installer.Presenter.PendingRequest==null&&!installer.Presenter.IsResultContinuationPending&&Model.State.IsDirty==false,"normal Root work settled");Assert.That(installer.PendingOperation.IsFaulted,Is.False,installer.PendingOperation.Exception?.GetBaseException().ToString());}
        private IEnumerator LocalState(ProductionLocalDataState expected,float seconds=25)
        {yield return Until(()=>State==expected&&DeletionTask.IsCompleted,"Root deletion="+expected,seconds);yield return null;installer.Presenter.Refresh();if(expected!=ProductionLocalDataState.Ready)Assert.That(Find("local-data-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));}
        private static IEnumerator Until(Func<bool> condition,string boundary,float seconds=25)
        {float deadline=Time.realtimeSinceStartup+seconds;while(!condition()&&Time.realtimeSinceStartup<deadline)yield return null;Assert.That(condition(),Is.True,boundary);}
        private static IEnumerator Await(Task work)
        {yield return Until(()=>work.IsCompleted,"actual async task completion");if(work.IsFaulted)throw work.Exception!;Assert.That(work.IsCanceled,Is.False);}
        private VisualElement Find(string name)=>installer.Document.rootVisualElement.Q(name)??throw new InvalidOperationException(name);
        private Button Button(string name)=>Find(name) as Button??throw new InvalidOperationException("BUTTON_REQUIRED:"+name);
        private void Submit(string name){Assert.That(Button(name).enabledInHierarchy,Is.True,name);SendEvenIfDisabled(name);}
        private void SendEvenIfDisabled(string name){var button=Button(name);using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}
        private static T Read<T>(object owner,string name)=>(T)(owner.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)??throw new InvalidOperationException("ROOT_TEST_FIELD_MISSING:"+name)).GetValue(owner)!;
        private static LocalSaveCodec Codec(AtomicSaveRepository repository)=>Read<LocalSaveCodec>(repository,"codec");
        private IEnumerator CaptureConfirmation()
        {
            if(texture==null)yield break;yield return null;yield return null;
            Assert.That(State,Is.EqualTo(ProductionLocalDataState.Confirmation));Assert.That(Read<ProfileDeletionIntent?>(installer,"deletionIntent"),Is.Null);
            var active=RenderTexture.active;var image=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
            try
            {
                RenderTexture.active=texture;image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);image.Apply();
                string directory=Path.Combine(Directory.GetCurrentDirectory(),".utmp","wp046-runtime");Directory.CreateDirectory(directory);
                File.WriteAllBytes(Path.Combine(directory,"profile-delete-confirmation.png"),image.EncodeToPNG());
            }
            finally{RenderTexture.active=active;UnityEngine.Object.Destroy(image);}
        }
        private sealed class ControlledProcessStop:Exception{}
        private sealed class FixtureClock:ILocalSessionClock{public long MonotonicMilliseconds=>0;public string UtcNow=>"2026-10-21T00:00:00Z";}
    }
}
