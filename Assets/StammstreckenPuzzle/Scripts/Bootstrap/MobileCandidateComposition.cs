using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;

namespace STP.Bootstrap
{
    /// <summary>Explicit ADR039 internal candidate, isolated from historical review and future production saves.</summary>
    public sealed class MobileCandidateComposition
    {
        private MobileCandidateComposition(MobileCandidateData catalog, MobileCandidateProgressCatalog progress,
            LockedCosmeticsCatalog cosmetics, SaveProfileIdentity profile, AtomicSaveRepository repository,
            PersistedPuzzleSession persisted, AutosaveCoordinator autosave, LocalMetagamePolicy metagame,
            LocalCosmeticsPolicy cosmeticPolicy, LocalPostClearPolicy postClear,LockedEndlessProfiles? endlessProfiles,
            LockedEndlessCompletion? endlessCompletion,LocalEndlessPolicy? endlessPolicy)
        { Catalog=catalog; Progress=progress; Cosmetics=cosmetics; Profile=profile; Repository=repository;
          Persisted=persisted; Autosave=autosave; Metagame=metagame; CosmeticPolicy=cosmeticPolicy; PostClear=postClear;
          EndlessProfiles=endlessProfiles;EndlessCompletion=endlessCompletion;EndlessPolicy=endlessPolicy; }

        public MobileCandidateData Catalog { get; }
        public MobileCandidateProgressCatalog Progress { get; }
        public LockedCosmeticsCatalog Cosmetics { get; }
        public SaveProfileIdentity Profile { get; }
        public AtomicSaveRepository Repository { get; }
        public PersistedPuzzleSession Persisted { get; }
        public AutosaveCoordinator Autosave { get; }
        public LocalMetagamePolicy Metagame { get; }
        public LocalCosmeticsPolicy CosmeticPolicy { get; }
        public LocalPostClearPolicy PostClear { get; }
        public LockedEndlessProfiles? EndlessProfiles { get; }
        public LockedEndlessCompletion? EndlessCompletion { get; }
        public LocalEndlessPolicy? EndlessPolicy { get; }

        /// <summary>Copies all inputs before worker validation and IO; creates no profile and initializes no SDK.</summary>
        public static Task<MobileCandidateComposition> CreateAsync(byte[] publicBytes, byte[] campaignBytes,
            byte[] completionBytes, byte[] cosmeticsBytes, byte[] bindingBytes, string privateRoot,
            ILocalSessionClock clock, ILocalSaveDispatcher dispatcher, string appVersion, string buildId, CancellationToken cancellation,
            byte[]? endlessProfiles=null,byte[]? endlessCompletion=null,byte[]? endlessBindings=null)
        {
            var p=(byte[])publicBytes.Clone(); var c=(byte[])campaignBytes.Clone(); var v=(byte[])completionBytes.Clone();
            var a=(byte[])cosmeticsBytes.Clone(); var b=(byte[])bindingBytes.Clone();
            var ep=endlessProfiles==null?null:(byte[])endlessProfiles.Clone();var ec=endlessCompletion==null?null:(byte[])endlessCompletion.Clone();
            var eb=endlessBindings==null?null:(byte[])endlessBindings.Clone();
            if((ep!=null||ec!=null||eb!=null)&&(ep==null||ec==null||eb==null))throw new ArgumentException("ENDLESS_ASSETS_INCOMPLETE");
            return Task.Run(() => {
                cancellation.ThrowIfCancellationRequested();
                var bindings=ReadBindings(b); ICanonicalJson json=new CanonicalJsonAdapter();
                var catalog=new MobileCandidateData(p,Text(bindings,"publicCatalogHashSha256"));
                var progress=new MobileCandidateProgressCatalog(catalog,c,v,json,Text(bindings,"campaignHashSha256"),Text(bindings,"completionHashSha256"));
                var cosmetics=new LockedCosmeticsCatalog(a,Text(bindings,"cosmeticsHashSha256"),CatalogUse.InternalPreview,json);
                var profile=new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"candidate",CatalogUse.InternalPreview);
                LockedEndlessProfiles? profiles=null;LockedEndlessCompletion? endless=null;
                var rewardBindings=new List<RetainedCompletionRewardBinding>{RetainedCompletionRewardBinding.FromWrapper(LocalPostClearPolicy.PolicyVersion,v,json,progress.Completion.Identity)};
                if(ep!=null)
                {
                    var extra=ReadEndlessBindings(eb!,catalog.Identity.HashSha256,ep,ec!);
                    profiles=new LockedEndlessProfiles(ep,catalog);
                    endless=new LockedEndlessCompletion(ec!,Text(extra,"completionHashSha256"),json);
                    rewardBindings.Add(RetainedCompletionRewardBinding.FromDocument(LocalPostClearPolicy.PolicyVersion,ec!,json,CatalogUse.InternalPreview));
                }
                var retained=RetainedContractBindings.ForRuntimeWithRewards(CatalogUse.InternalPreview,
                    RetainedContractBindings.ForCosmetics(cosmetics.Identity,progress.Campaign.Identity,a,c,catalog,json),
                    rewardBindings);
                var codec=LocalSaveCodec.ForMetagame(json,profile,progress.CatalogSet,catalog,progress.Campaign,progress.Completion,retained);
                var ledger=new SaveLedgerCodec(json);
                var policy=new LocalProgressPolicy(catalog,progress.Campaign,progress.Completion,json,ledger);
                SaveFileSystem.ProvisionPrivateRoot(privateRoot);
                var files=new SaveFileSystem(privateRoot,profile,progress.CatalogSet.AggregateCatalog);
                SavedPublic240Composition.ValidateStoragePathLengths(files);
                var repository=new AtomicSaveRepository(files,codec,clock,appVersion,buildId);
                var persisted=new PersistedPuzzleSession(repository,catalog,policy,profile,progress.CatalogSet,clock,dispatcher);
                LocalEndlessPolicy? endlessPolicy=null;
                if(endless!=null){endlessPolicy=new LocalEndlessPolicy(catalog,endless.Identity,json,ledger);persisted.ConfigureEndless(endlessPolicy);}
                return new MobileCandidateComposition(catalog,progress,cosmetics,profile,repository,persisted,new AutosaveCoordinator(persisted),
                    new LocalMetagamePolicy(catalog,progress.Completion.Identity,ledger),new LocalCosmeticsPolicy(cosmetics,progress.Campaign,json,ledger),
                    new LocalPostClearPolicy(catalog,progress.Completion.Identity,ledger),profiles,endless,endlessPolicy);
            }, cancellation);
        }
        public static void ValidateCatalogBytes(byte[] p, byte[] c, byte[] v, byte[] a, byte[] b)
        {
            var bindings=ReadBindings(b); ICanonicalJson json=new CanonicalJsonAdapter();
            var catalog=new MobileCandidateData(p,Text(bindings,"publicCatalogHashSha256"));
            _=new MobileCandidateProgressCatalog(catalog,c,v,json,Text(bindings,"campaignHashSha256"),Text(bindings,"completionHashSha256"));
            _=new LockedCosmeticsCatalog(a,Text(bindings,"cosmeticsHashSha256"),CatalogUse.InternalPreview,json);
        }
        private static JsonValue ReadBindings(byte[] bytes)
        {
            var parsed=StrictJsonParser.Parse(bytes,new JsonParserLimits(4096,4,256,32,16));
            if(!parsed.Succeeded)throw new ArgumentException("CANDIDATE_BINDINGS_JSON");
            var value=parsed.Value!;
            if(value.Kind!=JsonKind.Object||value.Members.Count!=9||value.Get("schemaVersion")?.IntegerValue!=1||
                Text(value,"catalogNamespace")!=SaveProfileIdentity.MobileCandidateNamespace||Text(value,"use")!="InternalPreview"||
                Text(value,"approvalStatus")!="DRAFT"||value.Get("releaseEligible")?.Kind!=JsonKind.Boolean||value.Get("releaseEligible")!.BooleanValue)
                throw new ArgumentException("CANDIDATE_BINDINGS_HEADER");
            foreach(var key in new[]{"publicCatalogHashSha256","campaignHashSha256","completionHashSha256","cosmeticsHashSha256"})
                if(!System.Text.RegularExpressions.Regex.IsMatch(Text(value,key),"^[0-9a-f]{64}$"))throw new ArgumentException("CANDIDATE_BINDINGS_HASH");
            return value;
        }
        private static JsonValue ReadEndlessBindings(byte[] bytes,string publicHash,byte[] profiles,byte[] completion)
        {
            var parsed=StrictJsonParser.Parse(bytes,new JsonParserLimits(4096,4,256,32,16));
            if(!parsed.Succeeded)throw new ArgumentException("ENDLESS_BINDINGS_JSON");
            var value=parsed.Value!;string[] fields={"schemaVersion","use","approvalStatus","releaseEligible","publicCatalogHashSha256","profilesHashSha256","completionHashSha256"};
            if(value.Kind!=JsonKind.Object||value.Members.Count!=fields.Length||value.Members.Any(m=>!fields.Contains(m.Name))||
                value.Get("schemaVersion")?.Kind!=JsonKind.Integer||value.Get("schemaVersion")!.IntegerValue!=1||
                Text(value,"use")!="InternalPreview"||Text(value,"approvalStatus")!="DRAFT"||value.Get("releaseEligible")?.Kind!=JsonKind.Boolean||
                value.Get("releaseEligible")!.BooleanValue||Text(value,"publicCatalogHashSha256")!=publicHash)throw new ArgumentException("ENDLESS_BINDINGS_HEADER");
            foreach(var item in new[]{(key:"profilesHashSha256",source:profiles),(key:"completionHashSha256",source:completion)})
            {
                var source=StrictJsonParser.Parse(item.source,new JsonParserLimits(65536,12,1024,1024,32));
                if(!source.Succeeded||LevelHashing.HashProjection(source.Value!)!=Text(value,item.key))throw new ArgumentException("ENDLESS_BINDINGS_LOCK_MISMATCH");
            }
            return value;
        }
        private static string Text(JsonValue value,string key)
            => value.Get(key)?.Kind==JsonKind.String?value.Get(key)!.StringValue:throw new ArgumentException("CANDIDATE_BINDINGS_FIELD:"+key);
    }
}
