using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using STP.Puzzle.Domain;

namespace STP.Application
{
    internal static class LocalModel
    {
        internal const long MaxSafe=9007199254740991L;
        internal static void Require(bool value,string code) { if(!value) throw new ArgumentException(code); }
        internal static string Text(string value) { Require(!string.IsNullOrWhiteSpace(value)&&value.Length<=65536,"SAVE_TEXT_INVALID");return value!; }
        internal static string Hash(string value) { Require(value!=null&&value.Length==64&&value.All(c=>c>='0'&&c<='9'||c>='a'&&c<='f'),"SAVE_HASH_INVALID");return value!; }
        internal static long Safe(long value) { Require(value>=0&&value<=MaxSafe,"SAVE_INTEGER_INVALID");return value!; }
        internal static long Positive(long value) { Require(value>0,"SAVE_INTEGER_INVALID");return Safe(value); }
        internal static long Signed(long value) { Require(value>=-MaxSafe&&value<=MaxSafe,"SAVE_INTEGER_INVALID");return value!; }
        internal static string Utc(string value) { DateTimeOffset instant;Require(value!=null&&System.Text.RegularExpressions.Regex.IsMatch(value,@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,7})?Z$")&&DateTimeOffset.TryParse(value,CultureInfo.InvariantCulture,DateTimeStyles.None,out instant)&&instant.Offset==TimeSpan.Zero,"SAVE_UTC_INVALID");return value!; }
        internal static string UInt64(string value) { ulong n;Require(value!=null&&(value=="0"||value.Length>0&&value[0]!='0')&&value.All(c=>c>='0'&&c<='9')&&ulong.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out n),"SAVE_ORDINAL_INVALID");return value!; }
        internal static string One(string value,string allowed) { Require(allowed.Split('|').Contains(value),"SAVE_ENUM_INVALID");return value!; }
        internal static IReadOnlyList<T> Copy<T>(IReadOnlyList<T> values) { if(values==null)throw new ArgumentNullException(nameof(values));Require(values.All(v=>v!=null),"SAVE_NULL_ITEM");return Array.AsReadOnly(values.ToArray()); }
        internal static void Unique<T>(IEnumerable<T> values) { var list=values.ToArray();Require(list.Distinct().Count()==list.Length,"SAVE_DUPLICATE_ID"); }
    }

    /// <summary>The single shared SHA256 utility over bytes emitted by the injected canonical JSON authority.</summary>
    public static class LocalSaveHash
    {
        /// <summary>Returns lowercase SHA256 of the exact supplied bytes.</summary>
        public static string Bytes(byte[] bytes) { if(bytes==null)throw new ArgumentNullException(nameof(bytes));using(var sha=SHA256.Create())return string.Concat(sha.ComputeHash(bytes).Select(b=>b.ToString("x2",CultureInfo.InvariantCulture))); }
        /// <summary>Canonicalizes a projection through the shared port; throws its stable rejection code on failure.</summary>
        public static byte[] Canonical(ICanonicalJson json,string projection) { if(json==null)throw new ArgumentNullException(nameof(json));var r=json.Canonicalize(Encoding.UTF8.GetBytes(projection),Limits);if(!r.Succeeded)throw new ArgumentException(r.ErrorCode);return r.GetUtf8()!; }
        /// <summary>Review resource bounds, with no truncation of authoritative deduplication truth.</summary>
        public static JsonResourceLimits Limits { get; }=new JsonResourceLimits(67108864,64,65536,1048576,256);
        /// <summary>Quotes one JSON string without selecting canonical key order or parsing JSON.</summary>
        public static string Quote(string value) { if(value==null)throw new ArgumentNullException(nameof(value));var b=new StringBuilder("\"");foreach(char c in value){switch(c){case '"':b.Append("\\\"");break;case '\\':b.Append("\\\\");break;case '\b':b.Append("\\b");break;case '\f':b.Append("\\f");break;case '\n':b.Append("\\n");break;case '\r':b.Append("\\r");break;case '\t':b.Append("\\t");break;default:if(c<32)b.Append("\\u").Append(((int)c).ToString("x4",CultureInfo.InvariantCulture));else b.Append(c);break;}}return b.Append('"').ToString(); }
        /// <summary>Exact three-field catalog projection used in the aggregate identity.</summary>
        public static string CatalogProjection(CatalogIdentity c)=>"{\"catalogId\":"+Quote(c.CatalogId)+",\"revision\":"+c.Revision.ToString(CultureInfo.InvariantCulture)+",\"hashSha256\":"+Quote(c.HashSha256)+"}";
    }

    /// <summary>Immutable aggregate binding for exactly one public, campaign and completion catalog snapshot.</summary>
    public sealed class CatalogSetIdentity : IEquatable<CatalogSetIdentity>
    {
        private CatalogSetIdentity(CatalogIdentity p,CatalogIdentity c,CatalogIdentity v,CatalogIdentity a){PublicCatalog=p;CampaignCatalog=c;CompletionCatalog=v;AggregateCatalog=a;}
        /// <summary>Checks common use and recomputes SHA256(JCS(exact three catalog projections)).</summary>
        public static CatalogSetIdentity Create(CatalogIdentity publicCatalog,CatalogIdentity campaignCatalog,CatalogIdentity completionCatalog,CatalogIdentity aggregateCatalog,ICanonicalJson json)
        {
            if(publicCatalog==null||campaignCatalog==null||completionCatalog==null||aggregateCatalog==null)throw new ArgumentNullException();
            LocalModel.Require(!publicCatalog.Equals(campaignCatalog)&&!publicCatalog.Equals(completionCatalog)&&!campaignCatalog.Equals(completionCatalog),"SAVE_CATALOG_ROLES_MUST_DIFFER");
            LocalModel.Require(publicCatalog.Use==campaignCatalog.Use&&publicCatalog.Use==completionCatalog.Use&&publicCatalog.Use==aggregateCatalog.Use,"SAVE_CATALOG_USE_MISMATCH");
            string projection="{\"publicCatalog\":"+LocalSaveHash.CatalogProjection(publicCatalog)+",\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(campaignCatalog)+",\"completionCatalog\":"+LocalSaveHash.CatalogProjection(completionCatalog)+"}";
            LocalModel.Require(LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,projection))==aggregateCatalog.HashSha256,"SAVE_CATALOG_SET_HASH_MISMATCH");
            return new CatalogSetIdentity(publicCatalog,campaignCatalog,completionCatalog,aggregateCatalog);
        }
        /// <summary>Exact public catalog binding.</summary>
        public CatalogIdentity PublicCatalog { get; }
        /// <summary>Exact campaign graph binding.</summary>
        public CatalogIdentity CampaignCatalog { get; }
        /// <summary>Exact completion values binding.</summary>
        public CatalogIdentity CompletionCatalog { get; }
        /// <summary>Combined identity used by the save envelope and repository.</summary>
        public CatalogIdentity AggregateCatalog { get; }
        /// <summary>Ordinal equality of all four exact identities.</summary>
        public bool Equals(CatalogSetIdentity? other)=>other!=null&&PublicCatalog.Equals(other.PublicCatalog)&&CampaignCatalog.Equals(other.CampaignCatalog)&&CompletionCatalog.Equals(other.CompletionCatalog)&&AggregateCatalog.Equals(other.AggregateCatalog);
        /// <inheritdoc/>
        public override bool Equals(object? other)=>Equals(other as CatalogSetIdentity);
        /// <inheritdoc/>
        public override int GetHashCode()=>AggregateCatalog.GetHashCode();
    }

    /// <summary>Closed passive ownership union; none of these records executes a grant.</summary>
    public abstract class LocalOwnership { private protected LocalOwnership(string itemId,string itemType){ItemId=LocalModel.Text(itemId);ItemType=LocalModel.One(itemType,"OPERATING_OBJECT|LIVERY|TRAIN_VARIANT");} /// <summary>Stable owned item.</summary>
 public string ItemId {get;} /// <summary>Known cosmetics type.</summary>
 public string ItemType {get;} }

    /// <summary>Closed passive saved claim union.</summary>
 public abstract class LocalRewardClaim { private protected LocalRewardClaim(){} }

    /// <summary>Closed passive operation union, without provider commands or secret receipt data.</summary>
 public abstract class LocalPendingOperation { private protected LocalPendingOperation(string id){OperationId=LocalModel.Text(id);} /// <summary>Stable local operation.</summary>
 public string OperationId {get;} }

    /// <summary>Closed generator parameter family. Production grammars need an explicit documented implementation.</summary>
 public abstract class RetainedGeneratorParameters { private protected RetainedGeneratorParameters(){} }

    /// <summary>Closed retained open endless variants; creating one never generates or reserves gameplay.</summary>
 public abstract class LocalOpenEndless { private protected LocalOpenEndless(string ordinal,string id,LocalEndlessDescriptor descriptor){GenerationOrdinal=LocalModel.UInt64(ordinal);Id=LocalModel.Text(id);Descriptor=descriptor??throw new ArgumentNullException(nameof(descriptor));LocalModel.Require(ordinal!="0"&&ordinal==descriptor.GenerationOrdinal,"SAVE_ENDLESS_BINDING_MISMATCH");} /// <summary>Reserved ordinal.</summary>
 public string GenerationOrdinal{get;} /// <summary>Canonical E1 identity.</summary>
 public string Id{get;} /// <summary>Complete retained technical inputs.</summary>
 public LocalEndlessDescriptor Descriptor{get;} }

    /// <summary>Immutable typed LocalProfileHeader; construction validates the closed record's local invariants.</summary>
    public sealed class LocalProfileHeader
    {
        private LocalProfileHeader(string localProfileId, string firstUseUtc, SaveProfileIdentity identity, CatalogSetIdentity catalogSet)
        {
            LocalProfileId=LocalModel.Text(localProfileId);
            FirstUseUtc=LocalModel.Utc(firstUseUtc);
            Identity=identity??throw new ArgumentNullException(nameof(identity));
            CatalogSet=catalogSet??throw new ArgumentNullException(nameof(catalogSet));
            LocalModel.Require(LocalProfileId==Identity.ProfileId&&Identity.Use==CatalogSet.AggregateCatalog.Use,"SAVE_PROFILE_BINDING_MISMATCH");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalProfileHeader Create(string localProfileId, string firstUseUtc, SaveProfileIdentity identity, CatalogSetIdentity catalogSet)=>new LocalProfileHeader(localProfileId,firstUseUtc,identity,catalogSet);
        /// <summary>Validated immutable LocalProfileId.</summary>
        public string LocalProfileId { get; }
        /// <summary>Validated immutable FirstUseUtc.</summary>
        public string FirstUseUtc { get; }
        /// <summary>Validated immutable Identity.</summary>
        public SaveProfileIdentity Identity { get; }
        /// <summary>Validated immutable CatalogSet.</summary>
        public CatalogSetIdentity CatalogSet { get; }
    }

    /// <summary>Immutable typed EconomyTerminal; construction validates the closed record's local invariants.</summary>
    public sealed class EconomyTerminal
    {
        private EconomyTerminal(string transactionId, string reasonCode, string subjectId, CatalogIdentity catalog, string claimId, long amount, long committedGeneration, string createdAtUtc)
        {
            TransactionId=LocalModel.Text(transactionId);
            ReasonCode=LocalModel.Text(reasonCode);
            SubjectId=LocalModel.Text(subjectId);
            Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            ClaimId=LocalModel.Text(claimId);
            Amount=LocalModel.Signed(amount);
            CommittedGeneration=LocalModel.Safe(committedGeneration);
            CreatedAtUtc=LocalModel.Utc(createdAtUtc);
            LocalModel.Signed(Amount);
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static EconomyTerminal Create(string transactionId, string reasonCode, string subjectId, CatalogIdentity catalog, string claimId, long amount, long committedGeneration, string createdAtUtc)=>new EconomyTerminal(transactionId,reasonCode,subjectId,catalog,claimId,amount,committedGeneration,createdAtUtc);
        /// <summary>Validated immutable TransactionId.</summary>
        public string TransactionId { get; }
        /// <summary>Validated immutable ReasonCode.</summary>
        public string ReasonCode { get; }
        /// <summary>Validated immutable SubjectId.</summary>
        public string SubjectId { get; }
        /// <summary>Validated immutable Catalog.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable Amount.</summary>
        public long Amount { get; }
        /// <summary>Validated immutable CommittedGeneration.</summary>
        public long CommittedGeneration { get; }
        /// <summary>Validated immutable CreatedAtUtc.</summary>
        public string CreatedAtUtc { get; }
    }

    /// <summary>Immutable typed LocalFirstClear; construction validates the closed record's local invariants.</summary>
    public sealed class LocalFirstClear
    {
        private LocalFirstClear(Guid attemptId, string solvedEventId, string completedAtUtc, long activeElapsedMs, EconomyTerminal terminal)
        {
            AttemptId=attemptId;
            SolvedEventId=LocalModel.Text(solvedEventId);
            CompletedAtUtc=LocalModel.Utc(completedAtUtc);
            ActiveElapsedMs=LocalModel.Safe(activeElapsedMs);
            Terminal=terminal??throw new ArgumentNullException(nameof(terminal));
            LocalModel.Require(AttemptId!=Guid.Empty&&SolvedEventId=="puzzle-solved:"+AttemptId.ToString("D"),"SAVE_FIRST_CLEAR_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalFirstClear Create(Guid attemptId, string solvedEventId, string completedAtUtc, long activeElapsedMs, EconomyTerminal terminal)=>new LocalFirstClear(attemptId,solvedEventId,completedAtUtc,activeElapsedMs,terminal);
        /// <summary>Validated immutable AttemptId.</summary>
        public Guid AttemptId { get; }
        /// <summary>Validated immutable SolvedEventId.</summary>
        public string SolvedEventId { get; }
        /// <summary>Validated immutable CompletedAtUtc.</summary>
        public string CompletedAtUtc { get; }
        /// <summary>Validated immutable ActiveElapsedMs.</summary>
        public long ActiveElapsedMs { get; }
        /// <summary>Validated immutable Terminal.</summary>
        public EconomyTerminal Terminal { get; }
    }

    /// <summary>Immutable typed LocalLevelProgress; construction validates the closed record's local invariants.</summary>
    public sealed class LocalLevelProgress
    {
        private LocalLevelProgress(PublicPuzzleIdentity identity, LocalFirstClear? firstClear, int starsHighWater, long? firstRunBestElapsedMs, long? revisionBestElapsedMs, bool directSolved, EconomyTerminal? secondStarPaid, EconomyTerminal? thirdStarPaid)
        {
            Identity=identity??throw new ArgumentNullException(nameof(identity));
            FirstClear=firstClear;
            StarsHighWater=starsHighWater;
            FirstRunBestElapsedMs=firstRunBestElapsedMs.HasValue?LocalModel.Safe(firstRunBestElapsedMs.Value):(long?)null;
            RevisionBestElapsedMs=revisionBestElapsedMs.HasValue?LocalModel.Safe(revisionBestElapsedMs.Value):(long?)null;
            DirectSolved=directSolved;
            SecondStarPaid=secondStarPaid;
            ThirdStarPaid=thirdStarPaid;
            LocalModel.Require(StarsHighWater>=0&&StarsHighWater<=3,"SAVE_STARS_INVALID");LocalModel.Require(FirstClear!=null?StarsHighWater>=1:StarsHighWater==0&&!DirectSolved&&FirstRunBestElapsedMs==null&&RevisionBestElapsedMs==null&&SecondStarPaid==null&&ThirdStarPaid==null,"SAVE_LEVEL_PROGRESS_INVALID");LocalModel.Require(SecondStarPaid==null||StarsHighWater>=2,"SAVE_BONUS_INVALID");LocalModel.Require(ThirdStarPaid==null||StarsHighWater>=3,"SAVE_BONUS_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalLevelProgress Create(PublicPuzzleIdentity identity, LocalFirstClear? firstClear, int starsHighWater, long? firstRunBestElapsedMs, long? revisionBestElapsedMs, bool directSolved, EconomyTerminal? secondStarPaid, EconomyTerminal? thirdStarPaid)=>new LocalLevelProgress(identity,firstClear,starsHighWater,firstRunBestElapsedMs,revisionBestElapsedMs,directSolved,secondStarPaid,thirdStarPaid);
        /// <summary>Validated immutable Identity.</summary>
        public PublicPuzzleIdentity Identity { get; }
        /// <summary>Validated immutable FirstClear.</summary>
        public LocalFirstClear? FirstClear { get; }
        /// <summary>Validated immutable StarsHighWater.</summary>
        public int StarsHighWater { get; }
        /// <summary>Validated immutable FirstRunBestElapsedMs.</summary>
        public long? FirstRunBestElapsedMs { get; }
        /// <summary>Validated immutable RevisionBestElapsedMs.</summary>
        public long? RevisionBestElapsedMs { get; }
        /// <summary>Validated immutable DirectSolved.</summary>
        public bool DirectSolved { get; }
        /// <summary>Validated immutable SecondStarPaid.</summary>
        public EconomyTerminal? SecondStarPaid { get; }
        /// <summary>Validated immutable ThirdStarPaid.</summary>
        public EconomyTerminal? ThirdStarPaid { get; }
    }

    /// <summary>Immutable typed LocalDraft; construction validates the closed record's local invariants.</summary>
    public sealed class LocalDraft
    {
        private LocalDraft(PublicPuzzleIdentity identity, string savedAtUtc, PuzzleSessionCheckpoint checkpoint)
        {
            Identity=identity??throw new ArgumentNullException(nameof(identity));
            SavedAtUtc=LocalModel.Utc(savedAtUtc);
            Checkpoint=checkpoint??throw new ArgumentNullException(nameof(checkpoint));
            LocalModel.Require(Identity.PuzzleId==Checkpoint.PuzzleId&&Identity.HashProfile==Checkpoint.PublicPuzzleHashProfile&&Identity.HashSha256==Checkpoint.PublicPuzzleHashSha256,"SAVE_DRAFT_IDENTITY_MISMATCH");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalDraft Create(PublicPuzzleIdentity identity, string savedAtUtc, PuzzleSessionCheckpoint checkpoint)=>new LocalDraft(identity,savedAtUtc,checkpoint);
        /// <summary>Validated immutable Identity.</summary>
        public PublicPuzzleIdentity Identity { get; }
        /// <summary>Validated immutable SavedAtUtc.</summary>
        public string SavedAtUtc { get; }
        /// <summary>Validated immutable Checkpoint.</summary>
        public PuzzleSessionCheckpoint Checkpoint { get; }
    }

    /// <summary>Immutable typed LocalStoredEntry; construction validates the closed record's local invariants.</summary>
    public sealed class LocalStoredEntry
    {
        private LocalStoredEntry(long sequence, string transactionId, string reasonCode, long amount, string subjectId, string? catalogId, int? catalogRevision, string? catalogHashSha256, string? claimId, string createdAtUtc, string entryChainHashSha256)
        {
            Sequence=LocalModel.Safe(sequence);
            TransactionId=LocalModel.Text(transactionId);
            ReasonCode=LocalModel.Text(reasonCode);
            Amount=LocalModel.Signed(amount);
            SubjectId=LocalModel.Text(subjectId);
            CatalogId=catalogId==null?null:LocalModel.Text(catalogId);
            CatalogRevision=catalogRevision;
            CatalogHashSha256=catalogHashSha256==null?null:LocalModel.Hash(catalogHashSha256);
            ClaimId=claimId==null?null:LocalModel.Text(claimId);
            CreatedAtUtc=LocalModel.Utc(createdAtUtc);
            EntryChainHashSha256=LocalModel.Hash(entryChainHashSha256);
            LocalModel.Positive(Sequence);LocalModel.Signed(Amount);LocalModel.Require(CatalogId==null?CatalogRevision==null&&CatalogHashSha256==null:CatalogRevision.HasValue&&CatalogRevision.Value>0&&CatalogHashSha256!=null,"SAVE_LEDGER_CATALOG_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalStoredEntry Create(long sequence, string transactionId, string reasonCode, long amount, string subjectId, string? catalogId, int? catalogRevision, string? catalogHashSha256, string? claimId, string createdAtUtc, string entryChainHashSha256)=>new LocalStoredEntry(sequence,transactionId,reasonCode,amount,subjectId,catalogId,catalogRevision,catalogHashSha256,claimId,createdAtUtc,entryChainHashSha256);
        /// <summary>Validated immutable Sequence.</summary>
        public long Sequence { get; }
        /// <summary>Validated immutable TransactionId.</summary>
        public string TransactionId { get; }
        /// <summary>Validated immutable ReasonCode.</summary>
        public string ReasonCode { get; }
        /// <summary>Validated immutable Amount.</summary>
        public long Amount { get; }
        /// <summary>Validated immutable SubjectId.</summary>
        public string SubjectId { get; }
        /// <summary>Validated immutable CatalogId.</summary>
        public string? CatalogId { get; }
        /// <summary>Validated immutable CatalogRevision.</summary>
        public int? CatalogRevision { get; }
        /// <summary>Validated immutable CatalogHashSha256.</summary>
        public string? CatalogHashSha256 { get; }
        /// <summary>Validated immutable ClaimId.</summary>
        public string? ClaimId { get; }
        /// <summary>Validated immutable CreatedAtUtc.</summary>
        public string CreatedAtUtc { get; }
        /// <summary>Validated immutable EntryChainHashSha256.</summary>
        public string EntryChainHashSha256 { get; }
    }

    /// <summary>Immutable typed LocalLedgerCheckpoint; construction validates the closed record's local invariants.</summary>
    public sealed class LocalLedgerCheckpoint
    {
        private LocalLedgerCheckpoint(long throughSequence, long balancePatience, string entryChainHashSha256, long sourceSaveGeneration, IReadOnlyList<LocalStoredEntry> journalDiagnosticTail)
        {
            ThroughSequence=LocalModel.Safe(throughSequence);
            BalancePatience=LocalModel.Safe(balancePatience);
            EntryChainHashSha256=LocalModel.Hash(entryChainHashSha256);
            SourceSaveGeneration=LocalModel.Safe(sourceSaveGeneration);
            JournalDiagnosticTail=LocalModel.Copy(journalDiagnosticTail);
            LocalModel.Require(JournalDiagnosticTail.Count<=64&&JournalDiagnosticTail.All(e=>e.Sequence<=ThroughSequence),"SAVE_LEDGER_TAIL_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalLedgerCheckpoint Create(long throughSequence, long balancePatience, string entryChainHashSha256, long sourceSaveGeneration, IReadOnlyList<LocalStoredEntry> journalDiagnosticTail)=>new LocalLedgerCheckpoint(throughSequence,balancePatience,entryChainHashSha256,sourceSaveGeneration,journalDiagnosticTail);
        /// <summary>Validated immutable ThroughSequence.</summary>
        public long ThroughSequence { get; }
        /// <summary>Validated immutable BalancePatience.</summary>
        public long BalancePatience { get; }
        /// <summary>Validated immutable EntryChainHashSha256.</summary>
        public string EntryChainHashSha256 { get; }
        /// <summary>Validated immutable SourceSaveGeneration.</summary>
        public long SourceSaveGeneration { get; }
        /// <summary>Validated immutable JournalDiagnosticTail.</summary>
        public IReadOnlyList<LocalStoredEntry> JournalDiagnosticTail { get; }
    }

    /// <summary>Immutable typed LocalSubjectProgress; construction validates the closed record's local invariants.</summary>
    public sealed class LocalSubjectProgress
    {
        private LocalSubjectProgress(string subjectId, string kind, IReadOnlyList<string> firstClearPuzzleIds, int firstClearCount, int memberCount, bool completed)
        {
            SubjectId=LocalModel.Text(subjectId);
            Kind=LocalModel.Text(kind);
            FirstClearPuzzleIds=LocalModel.Copy(firstClearPuzzleIds);
            FirstClearCount=firstClearCount;
            MemberCount=memberCount;
            Completed=completed;
            LocalModel.One(Kind,"SEASON|SECTION|ROUTE");LocalModel.Unique(FirstClearPuzzleIds);LocalModel.Require(MemberCount>0&&FirstClearCount==FirstClearPuzzleIds.Count&&FirstClearCount<=MemberCount&&Completed==(FirstClearCount==MemberCount),"SAVE_CACHE_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalSubjectProgress Create(string subjectId, string kind, IReadOnlyList<string> firstClearPuzzleIds, int firstClearCount, int memberCount, bool completed)=>new LocalSubjectProgress(subjectId,kind,firstClearPuzzleIds,firstClearCount,memberCount,completed);
        /// <summary>Validated immutable SubjectId.</summary>
        public string SubjectId { get; }
        /// <summary>Validated immutable Kind.</summary>
        public string Kind { get; }
        /// <summary>Validated immutable FirstClearPuzzleIds.</summary>
        public IReadOnlyList<string> FirstClearPuzzleIds { get; }
        /// <summary>Validated immutable FirstClearCount.</summary>
        public int FirstClearCount { get; }
        /// <summary>Validated immutable MemberCount.</summary>
        public int MemberCount { get; }
        /// <summary>Validated immutable Completed.</summary>
        public bool Completed { get; }
    }

    /// <summary>Immutable typed LocalSeasonProgressCache; construction validates the closed record's local invariants.</summary>
    public sealed class LocalSeasonProgressCache
    {
        private LocalSeasonProgressCache(CatalogIdentity campaignCatalog, string sourceProjectionHashSha256, IReadOnlyList<LocalSubjectProgress> subjects)
        {
            CampaignCatalog=campaignCatalog??throw new ArgumentNullException(nameof(campaignCatalog));
            SourceProjectionHashSha256=LocalModel.Hash(sourceProjectionHashSha256);
            Subjects=LocalModel.Copy(subjects);
            LocalModel.Unique(Subjects.Select(x=>x.Kind+":"+x.SubjectId));
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalSeasonProgressCache Create(CatalogIdentity campaignCatalog, string sourceProjectionHashSha256, IReadOnlyList<LocalSubjectProgress> subjects)=>new LocalSeasonProgressCache(campaignCatalog,sourceProjectionHashSha256,subjects);
        /// <summary>Validated immutable CampaignCatalog.</summary>
        public CatalogIdentity CampaignCatalog { get; }
        /// <summary>Validated immutable SourceProjectionHashSha256.</summary>
        public string SourceProjectionHashSha256 { get; }
        /// <summary>Validated immutable Subjects.</summary>
        public IReadOnlyList<LocalSubjectProgress> Subjects { get; }
    }

    /// <summary>Immutable typed DefaultOwnership; construction validates the closed record's local invariants.</summary>
    public sealed class DefaultOwnership: LocalOwnership
    {
        private DefaultOwnership(string itemId, string itemType, CatalogIdentity catalog, string? acquiredAtUtc) : base(itemId,itemType)
        {
            Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            AcquiredAtUtc=acquiredAtUtc==null?null:LocalModel.Utc(acquiredAtUtc);
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static DefaultOwnership Create(string itemId, string itemType, CatalogIdentity catalog, string? acquiredAtUtc)=>new DefaultOwnership(itemId,itemType,catalog,acquiredAtUtc);
        /// <summary>Validated immutable Catalog.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Validated immutable AcquiredAtUtc.</summary>
        public string? AcquiredAtUtc { get; }
    }

    /// <summary>Immutable typed PurchasedOwnership; construction validates the closed record's local invariants.</summary>
    public sealed class PurchasedOwnership: LocalOwnership
    {
        private PurchasedOwnership(string itemId, string itemType, CatalogIdentity catalog, string transactionId, long pricePatience, string acquiredAtUtc) : base(itemId,itemType)
        {
            Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            TransactionId=LocalModel.Text(transactionId);
            PricePatience=LocalModel.Positive(pricePatience);
            AcquiredAtUtc=LocalModel.Utc(acquiredAtUtc);
            LocalModel.Require(TransactionId=="cosmetic-purchase:"+ItemId,"SAVE_OWNERSHIP_TRANSACTION_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static PurchasedOwnership Create(string itemId, string itemType, CatalogIdentity catalog, string transactionId, long pricePatience, string acquiredAtUtc)=>new PurchasedOwnership(itemId,itemType,catalog,transactionId,pricePatience,acquiredAtUtc);
        /// <summary>Validated immutable Catalog.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Validated immutable TransactionId.</summary>
        public string TransactionId { get; }
        /// <summary>Validated immutable PricePatience.</summary>
        public long PricePatience { get; }
        /// <summary>Validated immutable AcquiredAtUtc.</summary>
        public string AcquiredAtUtc { get; }
    }

    /// <summary>Immutable typed MilestoneOwnership; construction validates the closed record's local invariants.</summary>
    public sealed class MilestoneOwnership: LocalOwnership
    {
        private MilestoneOwnership(string itemId, string itemType, CatalogIdentity catalog, string claimId, string operationId, long claimGeneration, string eligibilityProjectionHashSha256, string acquiredAtUtc) : base(itemId,itemType)
        {
            Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            ClaimId=LocalModel.Text(claimId);
            OperationId=LocalModel.Text(operationId);
            ClaimGeneration=LocalModel.Safe(claimGeneration);
            EligibilityProjectionHashSha256=LocalModel.Hash(eligibilityProjectionHashSha256);
            AcquiredAtUtc=LocalModel.Utc(acquiredAtUtc);
            LocalModel.Positive(ClaimGeneration);LocalModel.Require(ClaimId=="cosmetic-milestone-claim:v1:"+Catalog.CatalogId+":"+ItemId,"SAVE_COSMETIC_CLAIM_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static MilestoneOwnership Create(string itemId, string itemType, CatalogIdentity catalog, string claimId, string operationId, long claimGeneration, string eligibilityProjectionHashSha256, string acquiredAtUtc)=>new MilestoneOwnership(itemId,itemType,catalog,claimId,operationId,claimGeneration,eligibilityProjectionHashSha256,acquiredAtUtc);
        /// <summary>Validated immutable Catalog.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable OperationId.</summary>
        public string OperationId { get; }
        /// <summary>Validated immutable ClaimGeneration.</summary>
        public long ClaimGeneration { get; }
        /// <summary>Validated immutable EligibilityProjectionHashSha256.</summary>
        public string EligibilityProjectionHashSha256 { get; }
        /// <summary>Validated immutable AcquiredAtUtc.</summary>
        public string AcquiredAtUtc { get; }
    }

    /// <summary>Immutable typed LegacyOwnership; construction validates the closed record's local invariants.</summary>
    public sealed class LegacyOwnership: LocalOwnership
    {
        private LegacyOwnership(string itemId, string itemType, string? legacyPurchaseTransactionId, string? legacyAcquiredAtUtc, string? legacyGrantCode) : base(itemId,itemType)
        {
            LegacyPurchaseTransactionId=legacyPurchaseTransactionId==null?null:LocalModel.Text(legacyPurchaseTransactionId);
            LegacyAcquiredAtUtc=legacyAcquiredAtUtc==null?null:LocalModel.Utc(legacyAcquiredAtUtc);
            LegacyGrantCode=legacyGrantCode==null?null:LocalModel.Text(legacyGrantCode);
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LegacyOwnership Create(string itemId, string itemType, string? legacyPurchaseTransactionId, string? legacyAcquiredAtUtc, string? legacyGrantCode)=>new LegacyOwnership(itemId,itemType,legacyPurchaseTransactionId,legacyAcquiredAtUtc,legacyGrantCode);
        /// <summary>Validated immutable LegacyPurchaseTransactionId.</summary>
        public string? LegacyPurchaseTransactionId { get; }
        /// <summary>Validated immutable LegacyAcquiredAtUtc.</summary>
        public string? LegacyAcquiredAtUtc { get; }
        /// <summary>Validated immutable LegacyGrantCode.</summary>
        public string? LegacyGrantCode { get; }
    }

    /// <summary>Immutable typed LocalInventory; construction validates the closed record's local invariants.</summary>
    public sealed class LocalInventory
    {
        private LocalInventory(IReadOnlyList<LocalOwnership> ownership, IReadOnlyList<string> selectedItemIds)
        {
            Ownership=LocalModel.Copy(ownership);
            SelectedItemIds=LocalModel.Copy(selectedItemIds);
            LocalModel.Unique(Ownership.Select(x=>x.ItemId));LocalModel.Unique(SelectedItemIds);LocalModel.Require(SelectedItemIds.All(id=>Ownership.Any(x=>x.ItemId==id)),"SAVE_SELECTION_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalInventory Create(IReadOnlyList<LocalOwnership> ownership, IReadOnlyList<string> selectedItemIds)=>new LocalInventory(ownership,selectedItemIds);
        /// <summary>Validated immutable Ownership.</summary>
        public IReadOnlyList<LocalOwnership> Ownership { get; }
        /// <summary>Validated immutable SelectedItemIds.</summary>
        public IReadOnlyList<string> SelectedItemIds { get; }
    }

    /// <summary>Immutable typed LocalCosmeticReservation; construction validates the closed record's local invariants.</summary>
    public sealed class LocalCosmeticReservation
    {
        private LocalCosmeticReservation(string claimId, string operationId, long claimGeneration, string itemId, CatalogIdentity catalog, string eligibilityProjectionHashSha256)
        {
            ClaimId=LocalModel.Text(claimId);
            OperationId=LocalModel.Text(operationId);
            ClaimGeneration=LocalModel.Safe(claimGeneration);
            ItemId=LocalModel.Text(itemId);
            Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            EligibilityProjectionHashSha256=LocalModel.Hash(eligibilityProjectionHashSha256);
            LocalModel.Positive(ClaimGeneration);LocalModel.Require(ClaimId=="cosmetic-milestone-claim:v1:"+Catalog.CatalogId+":"+ItemId,"SAVE_COSMETIC_CLAIM_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalCosmeticReservation Create(string claimId, string operationId, long claimGeneration, string itemId, CatalogIdentity catalog, string eligibilityProjectionHashSha256)=>new LocalCosmeticReservation(claimId,operationId,claimGeneration,itemId,catalog,eligibilityProjectionHashSha256);
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable OperationId.</summary>
        public string OperationId { get; }
        /// <summary>Validated immutable ClaimGeneration.</summary>
        public long ClaimGeneration { get; }
        /// <summary>Validated immutable ItemId.</summary>
        public string ItemId { get; }
        /// <summary>Validated immutable Catalog.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Validated immutable EligibilityProjectionHashSha256.</summary>
        public string EligibilityProjectionHashSha256 { get; }
    }

    /// <summary>Immutable typed CampaignProgressTerminal; construction validates the closed record's local invariants.</summary>
    public sealed class CampaignProgressTerminal: LocalRewardClaim
    {
        private CampaignProgressTerminal(string subjectKind, string subjectId, EconomyTerminal terminal)
        {
            SubjectKind=LocalModel.Text(subjectKind);
            SubjectId=LocalModel.Text(subjectId);
            Terminal=terminal??throw new ArgumentNullException(nameof(terminal));
            LocalModel.One(SubjectKind,"ROUTE|SECTION");LocalModel.Require(Terminal.SubjectId==SubjectId,"SAVE_TERMINAL_SUBJECT_MISMATCH");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static CampaignProgressTerminal Create(string subjectKind, string subjectId, EconomyTerminal terminal)=>new CampaignProgressTerminal(subjectKind,subjectId,terminal);
        /// <summary>Validated immutable SubjectKind.</summary>
        public string SubjectKind { get; }
        /// <summary>Validated immutable SubjectId.</summary>
        public string SubjectId { get; }
        /// <summary>Validated immutable Terminal.</summary>
        public EconomyTerminal Terminal { get; }
    }

    /// <summary>Immutable typed PostClearRewardClaim; construction validates the closed record's local invariants.</summary>
    public sealed class PostClearRewardClaim: LocalRewardClaim
    {
        private PostClearRewardClaim(PublicPuzzleIdentity puzzleIdentity, string claimId, string localOperationId, string? providerOperationId, string claimStatus, CatalogIdentity completionCatalog, EconomyTerminal? terminal)
        {
            PuzzleIdentity=puzzleIdentity??throw new ArgumentNullException(nameof(puzzleIdentity));
            ClaimId=LocalModel.Text(claimId);
            LocalOperationId=LocalModel.Text(localOperationId);
            ProviderOperationId=providerOperationId==null?null:LocalModel.Text(providerOperationId);
            ClaimStatus=LocalModel.Text(claimStatus);
            CompletionCatalog=completionCatalog??throw new ArgumentNullException(nameof(completionCatalog));
            Terminal=terminal;
            LocalModel.One(ClaimStatus,"RESERVED|RECONCILIATION_REQUIRED|COMMITTED|CLOSED_NO_REWARD|FAILED");LocalModel.Require(ClaimId=="reward-claim:POST_CLEAR_PATIENCE:"+PuzzleIdentity.PuzzleId&&(ClaimStatus=="COMMITTED")==(Terminal!=null),"SAVE_REWARD_CLAIM_INVALID");if(Terminal!=null)LocalModel.Require(Terminal.ClaimId==ClaimId&&Terminal.Catalog.Equals(CompletionCatalog)&&Terminal.SubjectId==PuzzleIdentity.PuzzleId,"SAVE_REWARD_TERMINAL_MISMATCH");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static PostClearRewardClaim Create(PublicPuzzleIdentity puzzleIdentity, string claimId, string localOperationId, string? providerOperationId, string claimStatus, CatalogIdentity completionCatalog, EconomyTerminal? terminal)=>new PostClearRewardClaim(puzzleIdentity,claimId,localOperationId,providerOperationId,claimStatus,completionCatalog,terminal);
        /// <summary>Validated immutable PuzzleIdentity.</summary>
        public PublicPuzzleIdentity PuzzleIdentity { get; }
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable LocalOperationId.</summary>
        public string LocalOperationId { get; }
        /// <summary>Validated immutable ProviderOperationId.</summary>
        public string? ProviderOperationId { get; }
        /// <summary>Validated immutable ClaimStatus.</summary>
        public string ClaimStatus { get; }
        /// <summary>Validated immutable CompletionCatalog.</summary>
        public CatalogIdentity CompletionCatalog { get; }
        /// <summary>Validated immutable Terminal.</summary>
        public EconomyTerminal? Terminal { get; }
    }

    /// <summary>Immutable typed LocalIapOperation; construction validates the closed record's local invariants.</summary>
    public sealed class LocalIapOperation: LocalPendingOperation
    {
        private LocalIapOperation(string operationId, string logicalProductKey, string store, string? transactionReference, string state) : base(operationId)
        {
            LogicalProductKey=LocalModel.Text(logicalProductKey);
            Store=LocalModel.Text(store);
            TransactionReference=transactionReference==null?null:LocalModel.Text(transactionReference);
            State=LocalModel.Text(state);
            LocalModel.One(LogicalProductKey,"remove_ads");LocalModel.One(State,"STARTED|EVIDENCE_RECEIVED|VERIFIED|GRANTED_NOT_FINALIZED|FINALIZED|REJECTED|RECONCILIATION_REQUIRED");LocalModel.Require(State=="STARTED"||TransactionReference!=null,"SAVE_IAP_REFERENCE_MISSING");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalIapOperation Create(string operationId, string logicalProductKey, string store, string? transactionReference, string state)=>new LocalIapOperation(operationId,logicalProductKey,store,transactionReference,state);
        /// <summary>Validated immutable LogicalProductKey.</summary>
        public string LogicalProductKey { get; }
        /// <summary>Validated immutable Store.</summary>
        public string Store { get; }
        /// <summary>Validated immutable TransactionReference.</summary>
        public string? TransactionReference { get; }
        /// <summary>Validated immutable State.</summary>
        public string State { get; }
    }

    /// <summary>Immutable typed LocalRewardOperation; construction validates the closed record's local invariants.</summary>
    public sealed class LocalRewardOperation: LocalPendingOperation
    {
        private LocalRewardOperation(string operationId, string claimId, PublicPuzzleIdentity puzzleIdentity, string? providerOperationId, string state) : base(operationId)
        {
            ClaimId=LocalModel.Text(claimId);
            PuzzleIdentity=puzzleIdentity??throw new ArgumentNullException(nameof(puzzleIdentity));
            ProviderOperationId=providerOperationId==null?null:LocalModel.Text(providerOperationId);
            State=LocalModel.Text(state);
            LocalModel.One(State,"RESERVED|RECONCILIATION_REQUIRED|REWARD_CONFIRMED|NO_REWARD_CONFIRMED|COMMITTED|CLOSED_NO_REWARD|FAILED");LocalModel.Require(ClaimId=="reward-claim:POST_CLEAR_PATIENCE:"+PuzzleIdentity.PuzzleId,"SAVE_REWARD_CLAIM_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalRewardOperation Create(string operationId, string claimId, PublicPuzzleIdentity puzzleIdentity, string? providerOperationId, string state)=>new LocalRewardOperation(operationId,claimId,puzzleIdentity,providerOperationId,state);
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable PuzzleIdentity.</summary>
        public PublicPuzzleIdentity PuzzleIdentity { get; }
        /// <summary>Validated immutable ProviderOperationId.</summary>
        public string? ProviderOperationId { get; }
        /// <summary>Validated immutable State.</summary>
        public string State { get; }
    }

    /// <summary>Immutable typed LocalEntitlement; construction validates the closed record's local invariants.</summary>
    public sealed class LocalEntitlement
    {
        private LocalEntitlement(string productKey, string store, string transactionReference, bool owned, bool reconciliationRequired, bool revocationConfirmed, string? lastConfirmedAtUtc, string? grantOperationId, string? finalizationOperationId)
        {
            ProductKey=LocalModel.Text(productKey);
            Store=LocalModel.Text(store);
            TransactionReference=LocalModel.Text(transactionReference);
            Owned=owned;
            ReconciliationRequired=reconciliationRequired;
            RevocationConfirmed=revocationConfirmed;
            LastConfirmedAtUtc=lastConfirmedAtUtc==null?null:LocalModel.Utc(lastConfirmedAtUtc);
            GrantOperationId=grantOperationId==null?null:LocalModel.Text(grantOperationId);
            FinalizationOperationId=finalizationOperationId==null?null:LocalModel.Text(finalizationOperationId);
            LocalModel.One(ProductKey,"remove_ads");LocalModel.Require(!(Owned&&RevocationConfirmed),"SAVE_ENTITLEMENT_CONTRADICTION");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalEntitlement Create(string productKey, string store, string transactionReference, bool owned, bool reconciliationRequired, bool revocationConfirmed, string? lastConfirmedAtUtc, string? grantOperationId, string? finalizationOperationId)=>new LocalEntitlement(productKey,store,transactionReference,owned,reconciliationRequired,revocationConfirmed,lastConfirmedAtUtc,grantOperationId,finalizationOperationId);
        /// <summary>Validated immutable ProductKey.</summary>
        public string ProductKey { get; }
        /// <summary>Validated immutable Store.</summary>
        public string Store { get; }
        /// <summary>Validated immutable TransactionReference.</summary>
        public string TransactionReference { get; }
        /// <summary>Validated immutable Owned.</summary>
        public bool Owned { get; }
        /// <summary>Validated immutable ReconciliationRequired.</summary>
        public bool ReconciliationRequired { get; }
        /// <summary>Validated immutable RevocationConfirmed.</summary>
        public bool RevocationConfirmed { get; }
        /// <summary>Validated immutable LastConfirmedAtUtc.</summary>
        public string? LastConfirmedAtUtc { get; }
        /// <summary>Validated immutable GrantOperationId.</summary>
        public string? GrantOperationId { get; }
        /// <summary>Validated immutable FinalizationOperationId.</summary>
        public string? FinalizationOperationId { get; }
    }

    /// <summary>Immutable typed LocalEntitlements; construction validates the closed record's local invariants.</summary>
    public sealed class LocalEntitlements
    {
        private LocalEntitlements(IReadOnlyList<LocalEntitlement> records)
        {
            Records=LocalModel.Copy(records);
            LocalModel.Unique(Records.Select(e=>e.ProductKey+":"+e.Store+":"+e.TransactionReference));
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalEntitlements Create(IReadOnlyList<LocalEntitlement> records)=>new LocalEntitlements(records);
        /// <summary>Validated immutable Records.</summary>
        public IReadOnlyList<LocalEntitlement> Records { get; }
    }

    /// <summary>Immutable typed LocalAudioPreference; construction validates the closed record's local invariants.</summary>
    public sealed class LocalAudioPreference
    {
        private LocalAudioPreference(string groupId, long numerator, long denominator)
        {
            GroupId=LocalModel.Text(groupId);
            Numerator=LocalModel.Safe(numerator);
            Denominator=LocalModel.Safe(denominator);
            LocalModel.Positive(Denominator);LocalModel.Require(Numerator<=Denominator,"SAVE_AUDIO_SCALE_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalAudioPreference Create(string groupId, long numerator, long denominator)=>new LocalAudioPreference(groupId,numerator,denominator);
        /// <summary>Validated immutable GroupId.</summary>
        public string GroupId { get; }
        /// <summary>Validated immutable Numerator.</summary>
        public long Numerator { get; }
        /// <summary>Validated immutable Denominator.</summary>
        public long Denominator { get; }
    }

    /// <summary>Immutable typed LocalAccessibilityPreference; construction validates the closed record's local invariants.</summary>
    public sealed class LocalAccessibilityPreference
    {
        private LocalAccessibilityPreference(string contractRevision, bool? reducedMotion, bool? trainRideSkip)
        {
            ContractRevision=LocalModel.Text(contractRevision);
            ReducedMotion=reducedMotion;
            TrainRideSkip=trainRideSkip;
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalAccessibilityPreference Create(string contractRevision, bool? reducedMotion, bool? trainRideSkip)=>new LocalAccessibilityPreference(contractRevision,reducedMotion,trainRideSkip);
        /// <summary>Validated immutable ContractRevision.</summary>
        public string ContractRevision { get; }
        /// <summary>Validated immutable ReducedMotion.</summary>
        public bool? ReducedMotion { get; }
        /// <summary>Validated immutable TrainRideSkip.</summary>
        public bool? TrainRideSkip { get; }
    }

    /// <summary>Immutable typed LocalSettings; construction validates the closed record's local invariants.</summary>
    public sealed class LocalSettings
    {
        private LocalSettings(string? locale, IReadOnlyList<LocalAudioPreference> audioGroups, bool? hapticsEnabled, LocalAccessibilityPreference? accessibility)
        {
            Locale=locale==null?null:LocalModel.Text(locale);
            AudioGroups=LocalModel.Copy(audioGroups);
            HapticsEnabled=hapticsEnabled;
            Accessibility=accessibility;
            LocalModel.Unique(AudioGroups.Select(x=>x.GroupId));
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalSettings Create(string? locale, IReadOnlyList<LocalAudioPreference> audioGroups, bool? hapticsEnabled, LocalAccessibilityPreference? accessibility)=>new LocalSettings(locale,audioGroups,hapticsEnabled,accessibility);
        /// <summary>Validated immutable Locale.</summary>
        public string? Locale { get; }
        /// <summary>Validated immutable AudioGroups.</summary>
        public IReadOnlyList<LocalAudioPreference> AudioGroups { get; }
        /// <summary>Validated immutable HapticsEnabled.</summary>
        public bool? HapticsEnabled { get; }
        /// <summary>Validated immutable Accessibility.</summary>
        public LocalAccessibilityPreference? Accessibility { get; }
    }

    /// <summary>Immutable typed LocalPrivacyDecision; construction validates the closed record's local invariants.</summary>
    public sealed class LocalPrivacyDecision
    {
        private LocalPrivacyDecision(string policyRevision, string sdkContractRevision, long decisionRevision, string capturedAtUtc, bool ads, bool analytics, bool crash, string validity, string source, string nativeSyncState)
        {
            PolicyRevision=LocalModel.Text(policyRevision);
            SdkContractRevision=LocalModel.Text(sdkContractRevision);
            DecisionRevision=LocalModel.Safe(decisionRevision);
            CapturedAtUtc=LocalModel.Utc(capturedAtUtc);
            Ads=ads;
            Analytics=analytics;
            Crash=crash;
            Validity=LocalModel.Text(validity);
            Source=LocalModel.Text(source);
            NativeSyncState=LocalModel.Text(nativeSyncState);
            LocalModel.Positive(DecisionRevision);LocalModel.One(Validity,"VALID|INVALID");LocalModel.One(NativeSyncState,"UNKNOWN|DENIED_CONFIRMED|ENABLE_PENDING|ENABLED_CONFIRMED|REVOKE_PENDING|REVOKED_CONFIRMED");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalPrivacyDecision Create(string policyRevision, string sdkContractRevision, long decisionRevision, string capturedAtUtc, bool ads, bool analytics, bool crash, string validity, string source, string nativeSyncState)=>new LocalPrivacyDecision(policyRevision,sdkContractRevision,decisionRevision,capturedAtUtc,ads,analytics,crash,validity,source,nativeSyncState);
        /// <summary>Validated immutable PolicyRevision.</summary>
        public string PolicyRevision { get; }
        /// <summary>Validated immutable SdkContractRevision.</summary>
        public string SdkContractRevision { get; }
        /// <summary>Validated immutable DecisionRevision.</summary>
        public long DecisionRevision { get; }
        /// <summary>Validated immutable CapturedAtUtc.</summary>
        public string CapturedAtUtc { get; }
        /// <summary>Validated immutable Ads.</summary>
        public bool Ads { get; }
        /// <summary>Validated immutable Analytics.</summary>
        public bool Analytics { get; }
        /// <summary>Validated immutable Crash.</summary>
        public bool Crash { get; }
        /// <summary>Validated immutable Validity.</summary>
        public string Validity { get; }
        /// <summary>Validated immutable Source.</summary>
        public string Source { get; }
        /// <summary>Validated immutable NativeSyncState.</summary>
        public string NativeSyncState { get; }
    }

    /// <summary>Immutable typed LocalDataLifecycle; construction validates the closed record's local invariants.</summary>
    public sealed class LocalDataLifecycle
    {
        private LocalDataLifecycle(LocalPrivacyDecision? privacyDecision, string? lastReconciledAtUtc)
        {
            PrivacyDecision=privacyDecision;
            LastReconciledAtUtc=lastReconciledAtUtc==null?null:LocalModel.Utc(lastReconciledAtUtc);
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalDataLifecycle Create(LocalPrivacyDecision? privacyDecision, string? lastReconciledAtUtc)=>new LocalDataLifecycle(privacyDecision,lastReconciledAtUtc);
        /// <summary>Validated immutable PrivacyDecision.</summary>
        public LocalPrivacyDecision? PrivacyDecision { get; }
        /// <summary>Validated immutable LastReconciledAtUtc.</summary>
        public string? LastReconciledAtUtc { get; }
    }

    /// <summary>Immutable typed LocalDailyStatus; construction validates the closed record's local invariants.</summary>
    public sealed class LocalDailyStatus
    {
        private LocalDailyStatus(string policyVersion, IReadOnlyList<string> confirmedClaimIds)
        {
            PolicyVersion=LocalModel.Text(policyVersion);
            ConfirmedClaimIds=LocalModel.Copy(confirmedClaimIds);
            LocalModel.Unique(ConfirmedClaimIds);
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalDailyStatus Create(string policyVersion, IReadOnlyList<string> confirmedClaimIds)=>new LocalDailyStatus(policyVersion,confirmedClaimIds);
        /// <summary>Validated immutable PolicyVersion.</summary>
        public string PolicyVersion { get; }
        /// <summary>Validated immutable ConfirmedClaimIds.</summary>
        public IReadOnlyList<string> ConfirmedClaimIds { get; }
    }

    /// <summary>Immutable typed LocalHintEntitlement; construction validates the closed record's local invariants.</summary>
    public sealed class LocalHintEntitlement
    {
        private LocalHintEntitlement(string policyVersion, PublicPuzzleIdentity puzzleIdentity, SessionMode mode, long availableCredits, IReadOnlyList<string> claimIds)
        {
            PolicyVersion=LocalModel.Text(policyVersion);
            PuzzleIdentity=puzzleIdentity??throw new ArgumentNullException(nameof(puzzleIdentity));
            Mode=mode;
            AvailableCredits=LocalModel.Safe(availableCredits);
            ClaimIds=LocalModel.Copy(claimIds);
            LocalModel.Require(Enum.IsDefined(typeof(SessionMode),Mode),"SAVE_MODE_INVALID");LocalModel.Unique(ClaimIds);
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalHintEntitlement Create(string policyVersion, PublicPuzzleIdentity puzzleIdentity, SessionMode mode, long availableCredits, IReadOnlyList<string> claimIds)=>new LocalHintEntitlement(policyVersion,puzzleIdentity,mode,availableCredits,claimIds);
        /// <summary>Validated immutable PolicyVersion.</summary>
        public string PolicyVersion { get; }
        /// <summary>Validated immutable PuzzleIdentity.</summary>
        public PublicPuzzleIdentity PuzzleIdentity { get; }
        /// <summary>Validated immutable Mode.</summary>
        public SessionMode Mode { get; }
        /// <summary>Validated immutable AvailableCredits.</summary>
        public long AvailableCredits { get; }
        /// <summary>Validated immutable ClaimIds.</summary>
        public IReadOnlyList<string> ClaimIds { get; }
    }

    /// <summary>Immutable typed ContractGeneratorParameters; construction validates the closed record's local invariants.</summary>
    public sealed class ContractGeneratorParameters: RetainedGeneratorParameters
    {
        private ContractGeneratorParameters(int gridWidth, int gridHeight)
        {
            GridWidth=gridWidth;
            GridHeight=gridHeight;
            LocalModel.Require(GridWidth>=2&&GridWidth<=32&&GridHeight>=2&&GridHeight<=32,"SAVE_GENERATOR_PARAMETER_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static ContractGeneratorParameters Create(int gridWidth, int gridHeight)=>new ContractGeneratorParameters(gridWidth,gridHeight);
        /// <summary>Validated immutable GridWidth.</summary>
        public int GridWidth { get; }
        /// <summary>Validated immutable GridHeight.</summary>
        public int GridHeight { get; }
    }

    /// <summary>Immutable typed LocalEndlessDescriptor; construction validates the closed record's local invariants.</summary>
    public sealed class LocalEndlessDescriptor
    {
        private LocalEndlessDescriptor(string rulesetVersion, string generatorVersion, string seed, string generationOrdinal, RetainedGeneratorParameters parameters, string parameterHashSha256)
        {
            RulesetVersion=LocalModel.Text(rulesetVersion);
            GeneratorVersion=LocalModel.Text(generatorVersion);
            Seed=LocalModel.Text(seed);
            GenerationOrdinal=LocalModel.Text(generationOrdinal);
            Parameters=parameters??throw new ArgumentNullException(nameof(parameters));
            ParameterHashSha256=LocalModel.Hash(parameterHashSha256);
            LocalModel.One(RulesetVersion,"train-track-v1");LocalModel.UInt64(Seed);LocalModel.UInt64(GenerationOrdinal);LocalModel.Require(GenerationOrdinal!="0","SAVE_ORDINAL_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalEndlessDescriptor Create(string rulesetVersion, string generatorVersion, string seed, string generationOrdinal, RetainedGeneratorParameters parameters, string parameterHashSha256)=>new LocalEndlessDescriptor(rulesetVersion,generatorVersion,seed,generationOrdinal,parameters,parameterHashSha256);
        /// <summary>Validated immutable RulesetVersion.</summary>
        public string RulesetVersion { get; }
        /// <summary>Validated immutable GeneratorVersion.</summary>
        public string GeneratorVersion { get; }
        /// <summary>Validated immutable Seed.</summary>
        public string Seed { get; }
        /// <summary>Validated immutable GenerationOrdinal.</summary>
        public string GenerationOrdinal { get; }
        /// <summary>Validated immutable Parameters.</summary>
        public RetainedGeneratorParameters Parameters { get; }
        /// <summary>Validated immutable ParameterHashSha256.</summary>
        public string ParameterHashSha256 { get; }
    }

    /// <summary>Immutable typed LocalHashReference; construction validates the closed record's local invariants.</summary>
    public sealed class LocalHashReference
    {
        private LocalHashReference(string profile, string sha256)
        {
            Profile=LocalModel.Text(profile);
            Sha256=LocalModel.Hash(sha256);
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalHashReference Create(string profile, string sha256)=>new LocalHashReference(profile,sha256);
        /// <summary>Validated immutable Profile.</summary>
        public string Profile { get; }
        /// <summary>Validated immutable Sha256.</summary>
        public string Sha256 { get; }
    }

    /// <summary>Immutable typed ReservedEndless; construction validates the closed record's local invariants.</summary>
    public sealed class ReservedEndless: LocalOpenEndless
    {
        private ReservedEndless(string generationOrdinal, string id, LocalEndlessDescriptor descriptor) : base(generationOrdinal,id,descriptor)
        {
            
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static ReservedEndless Create(string generationOrdinal, string id, LocalEndlessDescriptor descriptor)=>new ReservedEndless(generationOrdinal,id,descriptor);
    }

    /// <summary>Immutable typed ActiveEndlessDraft; construction validates the closed record's local invariants.</summary>
    public sealed class ActiveEndlessDraft: LocalOpenEndless
    {
        private ActiveEndlessDraft(string generationOrdinal, string id, LocalEndlessDescriptor descriptor, PublicPuzzleInput publicPuzzleInput, LocalHashReference publicPuzzleHash, LocalHashReference solutionHash, LocalHashReference proofHash, PuzzleSessionCheckpoint checkpoint) : base(generationOrdinal,id,descriptor)
        {
            PublicPuzzleInput=publicPuzzleInput??throw new ArgumentNullException(nameof(publicPuzzleInput));
            PublicPuzzleHash=publicPuzzleHash??throw new ArgumentNullException(nameof(publicPuzzleHash));
            SolutionHash=solutionHash??throw new ArgumentNullException(nameof(solutionHash));
            ProofHash=proofHash??throw new ArgumentNullException(nameof(proofHash));
            Checkpoint=checkpoint??throw new ArgumentNullException(nameof(checkpoint));
            LocalModel.Require(Id==PublicPuzzleInput.Identity.PuzzleId&&PublicPuzzleHash.Profile==PublicPuzzleInput.Identity.HashProfile&&PublicPuzzleHash.Sha256==PublicPuzzleInput.Identity.HashSha256&&Checkpoint.PuzzleId==Id&&Checkpoint.Mode==SessionMode.ENDLESS,"SAVE_ENDLESS_PUBLIC_BINDING_MISMATCH");LocalModel.Require(SolutionHash.Profile=="STP-SOLUTION-JCS-1"&&ProofHash.Profile=="STP-PROOF-JCS-1","SAVE_ENDLESS_HASH_PROFILE_INVALID");LocalModel.Require(Checkpoint.Restore(PublicPuzzleInput.ToDomainDefinition(),Id,PublicPuzzleHash.Profile,PublicPuzzleHash.Sha256).Succeeded,"SAVE_ENDLESS_CHECKPOINT_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static ActiveEndlessDraft Create(string generationOrdinal, string id, LocalEndlessDescriptor descriptor, PublicPuzzleInput publicPuzzleInput, LocalHashReference publicPuzzleHash, LocalHashReference solutionHash, LocalHashReference proofHash, PuzzleSessionCheckpoint checkpoint)=>new ActiveEndlessDraft(generationOrdinal,id,descriptor,publicPuzzleInput,publicPuzzleHash,solutionHash,proofHash,checkpoint);
        /// <summary>Validated immutable PublicPuzzleInput.</summary>
        public PublicPuzzleInput PublicPuzzleInput { get; }
        /// <summary>Validated immutable PublicPuzzleHash.</summary>
        public LocalHashReference PublicPuzzleHash { get; }
        /// <summary>Validated immutable SolutionHash.</summary>
        public LocalHashReference SolutionHash { get; }
        /// <summary>Validated immutable ProofHash.</summary>
        public LocalHashReference ProofHash { get; }
        /// <summary>Validated immutable Checkpoint.</summary>
        public PuzzleSessionCheckpoint Checkpoint { get; }
    }

    /// <summary>Immutable typed LocalEndlessClaim; construction validates the closed record's local invariants.</summary>
    public sealed class LocalEndlessClaim
    {
        private LocalEndlessClaim(string claimId, string localOperationId, string claimStatus, string? providerOperationId)
        {
            ClaimId=LocalModel.Text(claimId);
            LocalOperationId=LocalModel.Text(localOperationId);
            ClaimStatus=LocalModel.Text(claimStatus);
            ProviderOperationId=providerOperationId==null?null:LocalModel.Text(providerOperationId);
            LocalModel.One(ClaimStatus,"LOCAL_DECISION_PENDING|PROVIDER_RESERVED|RECONCILIATION_REQUIRED|REWARD_CONFIRMED|NO_REWARD_CONFIRMED");LocalModel.Require(ClaimStatus=="LOCAL_DECISION_PENDING"?ProviderOperationId==null:ProviderOperationId!=null,"SAVE_ENDLESS_PROVIDER_BINDING_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalEndlessClaim Create(string claimId, string localOperationId, string claimStatus, string? providerOperationId)=>new LocalEndlessClaim(claimId,localOperationId,claimStatus,providerOperationId);
        /// <summary>Validated immutable ClaimId.</summary>
        public string ClaimId { get; }
        /// <summary>Validated immutable LocalOperationId.</summary>
        public string LocalOperationId { get; }
        /// <summary>Validated immutable ClaimStatus.</summary>
        public string ClaimStatus { get; }
        /// <summary>Validated immutable ProviderOperationId.</summary>
        public string? ProviderOperationId { get; }
    }

    /// <summary>Immutable typed OpenEndlessCompletion; construction validates the closed record's local invariants.</summary>
    public sealed class OpenEndlessCompletion: LocalOpenEndless
    {
        private OpenEndlessCompletion(string generationOrdinal, string id, LocalEndlessDescriptor descriptor, string puzzleId, string completionCommitId, LocalEndlessClaim claim) : base(generationOrdinal,id,descriptor)
        {
            PuzzleId=LocalModel.Text(puzzleId);
            CompletionCommitId=LocalModel.Text(completionCommitId);
            Claim=claim??throw new ArgumentNullException(nameof(claim));
            LocalModel.Require(PuzzleId==Id&&Claim.ClaimId=="reward-claim:POST_CLEAR_PATIENCE:"+Id,"SAVE_ENDLESS_CLAIM_INVALID");
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static OpenEndlessCompletion Create(string generationOrdinal, string id, LocalEndlessDescriptor descriptor, string puzzleId, string completionCommitId, LocalEndlessClaim claim)=>new OpenEndlessCompletion(generationOrdinal,id,descriptor,puzzleId,completionCommitId,claim);
        /// <summary>Validated immutable PuzzleId.</summary>
        public string PuzzleId { get; }
        /// <summary>Validated immutable CompletionCommitId.</summary>
        public string CompletionCommitId { get; }
        /// <summary>Validated immutable Claim.</summary>
        public LocalEndlessClaim Claim { get; }
    }

    /// <summary>Immutable typed LocalEndlessState; construction validates the closed record's local invariants.</summary>
    public sealed class LocalEndlessState
    {
        private LocalEndlessState(string highestReservedOrdinal, IReadOnlyList<LocalOpenEndless> openEndless)
        {
            HighestReservedOrdinal=LocalModel.Text(highestReservedOrdinal);
            OpenEndless=LocalModel.Copy(openEndless);
            LocalModel.UInt64(HighestReservedOrdinal);LocalModel.Require(OpenEndless.Count<=20,"SAVE_ENDLESS_LIMIT");ulong previous=0;foreach(var e in OpenEndless){ulong n=ulong.Parse(e.GenerationOrdinal,CultureInfo.InvariantCulture);LocalModel.Require(n>previous&&n<=ulong.Parse(HighestReservedOrdinal,CultureInfo.InvariantCulture),"SAVE_ENDLESS_ORDER_INVALID");previous=n;}LocalModel.Unique(OpenEndless.Select(x=>x.Id));
        }
        /// <summary>Copies all collection inputs and rejects inconsistent retained facts.</summary>
        public static LocalEndlessState Create(string highestReservedOrdinal, IReadOnlyList<LocalOpenEndless> openEndless)=>new LocalEndlessState(highestReservedOrdinal,openEndless);
        /// <summary>Validated immutable HighestReservedOrdinal.</summary>
        public string HighestReservedOrdinal { get; }
        /// <summary>Validated immutable OpenEndless.</summary>
        public IReadOnlyList<LocalOpenEndless> OpenEndless { get; }
    }

    /// <summary>Complete deeply immutable v2 profile payload. Passive records are retained without executing excluded capabilities.</summary>
    public sealed class LocalProfileState : IImmutableSavePayload
    {
        private LocalProfileState(LocalProfileHeader profile,IReadOnlyList<LocalLevelProgress> levels,LocalEndlessState endless,LocalLedgerCheckpoint ledgerCheckpoint,IReadOnlyList<LocalStoredEntry> economyJournal,LocalInventory inventory,IReadOnlyList<LocalCosmeticReservation> cosmeticClaimReservations,LocalSeasonProgressCache seasonProgressCache,IReadOnlyList<LocalDraft> drafts,IReadOnlyList<LocalRewardClaim> rewardClaims,LocalDailyStatus? dailyStatus,LocalEntitlements entitlements,IReadOnlyList<LocalPendingOperation> pendingOperations,LocalSettings settings,LocalDataLifecycle dataLifecycle,LocalHintEntitlement? hintEntitlement)
        {
            Profile=profile??throw new ArgumentNullException(nameof(profile));Levels=LocalModel.Copy(levels);Endless=endless??throw new ArgumentNullException(nameof(endless));LedgerCheckpoint=ledgerCheckpoint??throw new ArgumentNullException(nameof(ledgerCheckpoint));EconomyJournal=LocalModel.Copy(economyJournal);Inventory=inventory??throw new ArgumentNullException(nameof(inventory));CosmeticClaimReservations=LocalModel.Copy(cosmeticClaimReservations);SeasonProgressCache=seasonProgressCache??throw new ArgumentNullException(nameof(seasonProgressCache));Drafts=LocalModel.Copy(drafts);RewardClaims=LocalModel.Copy(rewardClaims);DailyStatus=dailyStatus;Entitlements=entitlements??throw new ArgumentNullException(nameof(entitlements));PendingOperations=LocalModel.Copy(pendingOperations);Settings=settings??throw new ArgumentNullException(nameof(settings));DataLifecycle=dataLifecycle??throw new ArgumentNullException(nameof(dataLifecycle));HintEntitlement=hintEntitlement;
            LocalModel.Require(Drafts.Count<=20&&EconomyJournal.Count<=512,"SAVE_COLLECTION_LIMIT");LocalModel.Unique(Levels.Select(x=>x.Identity.PuzzleId));LocalModel.Unique(Drafts.Select(x=>x.Identity.PuzzleId));LocalModel.Unique(CosmeticClaimReservations.Select(x=>x.ClaimId));LocalModel.Unique(CosmeticClaimReservations.Select(x=>x.ItemId));LocalModel.Unique(PendingOperations.Select(x=>x.OperationId));LocalModel.Unique(RewardClaims.OfType<PostClearRewardClaim>().Select(c=>c.ClaimId));LocalModel.Unique(RewardClaims.OfType<PostClearRewardClaim>().Select(c=>c.LocalOperationId));LocalModel.Unique(RewardClaims.OfType<CampaignProgressTerminal>().Select(c=>c.SubjectKind+":"+c.SubjectId));LocalModel.Unique(Endless.OpenEndless.OfType<OpenEndlessCompletion>().Select(c=>c.Claim.LocalOperationId));LocalModel.Unique(RewardClaims.OfType<PostClearRewardClaim>().Select(c=>c.LocalOperationId).Concat(Endless.OpenEndless.OfType<OpenEndlessCompletion>().Select(c=>c.Claim.LocalOperationId)));LocalModel.Unique(RewardClaims.OfType<PostClearRewardClaim>().Select(c=>c.ClaimId).Concat(Endless.OpenEndless.OfType<OpenEndlessCompletion>().Select(c=>c.Claim.ClaimId)));
            LocalModel.Require(CosmeticClaimReservations.All(r=>!Inventory.Ownership.OfType<MilestoneOwnership>().Any(o=>o.ClaimId==r.ClaimId||o.ItemId==r.ItemId)),"SAVE_COSMETIC_CLAIM_COLLISION");
            foreach(var e in Entitlements.Records)foreach(string? id in new[]{e.GrantOperationId,e.FinalizationOperationId})if(id!=null)LocalModel.Require(PendingOperations.OfType<LocalIapOperation>().Any(o=>o.OperationId==id&&o.LogicalProductKey==e.ProductKey&&o.Store==e.Store&&o.TransactionReference==e.TransactionReference),"SAVE_ENTITLEMENT_OPERATION_MISMATCH");
            foreach(var o in PendingOperations.OfType<LocalRewardOperation>())LocalModel.Require(RewardClaims.OfType<PostClearRewardClaim>().Any(c=>c.LocalOperationId==o.OperationId&&c.ClaimId==o.ClaimId&&c.PuzzleIdentity.Equals(o.PuzzleIdentity)&&c.ProviderOperationId==o.ProviderOperationId)||Endless.OpenEndless.OfType<OpenEndlessCompletion>().Any(c=>c.Claim.LocalOperationId==o.OperationId&&c.Claim.ClaimId==o.ClaimId&&c.PuzzleId==o.PuzzleIdentity.PuzzleId&&c.Claim.ProviderOperationId==o.ProviderOperationId),"SAVE_REWARD_OPERATION_MISMATCH");
        }
        /// <summary>Builds a complete payload and copies every authoritative collection.</summary>
        public static LocalProfileState Create(LocalProfileHeader profile,IReadOnlyList<LocalLevelProgress> levels,LocalEndlessState endless,LocalLedgerCheckpoint ledgerCheckpoint,IReadOnlyList<LocalStoredEntry> economyJournal,LocalInventory inventory,IReadOnlyList<LocalCosmeticReservation> cosmeticClaimReservations,LocalSeasonProgressCache seasonProgressCache,IReadOnlyList<LocalDraft> drafts,IReadOnlyList<LocalRewardClaim> rewardClaims,LocalDailyStatus? dailyStatus,LocalEntitlements entitlements,IReadOnlyList<LocalPendingOperation> pendingOperations,LocalSettings settings,LocalDataLifecycle dataLifecycle,LocalHintEntitlement? hintEntitlement)=>new LocalProfileState(profile,levels,endless,ledgerCheckpoint,economyJournal,inventory,cosmeticClaimReservations,seasonProgressCache,drafts,rewardClaims,dailyStatus,entitlements,pendingOperations,settings,dataLifecycle,hintEntitlement);
        /// <summary>Complete immutable Profile.</summary>
        public LocalProfileHeader Profile {get;}
        /// <summary>Complete immutable Levels.</summary>
        public IReadOnlyList<LocalLevelProgress> Levels {get;}
        /// <summary>Complete immutable Endless.</summary>
        public LocalEndlessState Endless {get;}
        /// <summary>Complete immutable LedgerCheckpoint.</summary>
        public LocalLedgerCheckpoint LedgerCheckpoint {get;}
        /// <summary>Complete immutable EconomyJournal.</summary>
        public IReadOnlyList<LocalStoredEntry> EconomyJournal {get;}
        /// <summary>Complete immutable Inventory.</summary>
        public LocalInventory Inventory {get;}
        /// <summary>Complete immutable CosmeticClaimReservations.</summary>
        public IReadOnlyList<LocalCosmeticReservation> CosmeticClaimReservations {get;}
        /// <summary>Complete immutable SeasonProgressCache.</summary>
        public LocalSeasonProgressCache SeasonProgressCache {get;}
        /// <summary>Complete immutable Drafts.</summary>
        public IReadOnlyList<LocalDraft> Drafts {get;}
        /// <summary>Complete immutable RewardClaims.</summary>
        public IReadOnlyList<LocalRewardClaim> RewardClaims {get;}
        /// <summary>Complete immutable DailyStatus.</summary>
        public LocalDailyStatus? DailyStatus {get;}
        /// <summary>Complete immutable Entitlements.</summary>
        public LocalEntitlements Entitlements {get;}
        /// <summary>Complete immutable PendingOperations.</summary>
        public IReadOnlyList<LocalPendingOperation> PendingOperations {get;}
        /// <summary>Complete immutable Settings.</summary>
        public LocalSettings Settings {get;}
        /// <summary>Complete immutable DataLifecycle.</summary>
        public LocalDataLifecycle DataLifecycle {get;}
        /// <summary>Complete immutable HintEntitlement.</summary>
        public LocalHintEntitlement? HintEntitlement {get;}
        /// <summary>Copies only S-owned gameplay projections while preserving every passive area and profile binding.</summary>
        public LocalProfileState WithProgress(IReadOnlyList<LocalLevelProgress> levels,LocalLedgerCheckpoint checkpoint,IReadOnlyList<LocalStoredEntry> journal,LocalSeasonProgressCache cache,IReadOnlyList<LocalDraft> drafts,IReadOnlyList<LocalRewardClaim> claims)=>Create(Profile,levels,Endless,checkpoint,journal,Inventory,CosmeticClaimReservations,cache,drafts,claims,DailyStatus,Entitlements,PendingOperations,Settings,DataLifecycle,HintEntitlement);
    }
}
