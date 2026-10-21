using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Closed outcome of strict save decoding. Unsupported source bytes are never replaced by a default payload.</summary>
    public enum LocalSaveDecodeStatus { Valid,Corrupt,Unsupported }
    /// <summary>Immutable decoded envelope metadata and an optional fully validated materialized snapshot.</summary>
    public sealed class LocalSaveDecodeResult
    {
        internal LocalSaveDecodeResult(LocalSaveDecodeStatus status,string? error,SaveSnapshot<LocalProfileState>? snapshot,int? sourceSchema,long? generation,string? payloadHash,string sourceHash,string? created,string? app,string? build){Status=status;ErrorCode=error;Snapshot=snapshot;SourceSchemaVersion=sourceSchema;Generation=generation;SourcePayloadSha256=payloadHash;SourceBytesSha256=sourceHash;CreatedAtUtc=created;AppVersion=app;BuildId=build;}
        /// <summary>Exact closed decoding outcome.</summary>
        public LocalSaveDecodeStatus Status{get;}
        /// <summary>Stable error code, absent only for Valid.</summary>
        public string? ErrorCode{get;}
        /// <summary>Validated materialized snapshot; null on every failure.</summary>
        public SaveSnapshot<LocalProfileState>? Snapshot{get;}
        /// <summary>Original supported or unsupported schema when unambiguously read.</summary>
        public int? SourceSchemaVersion{get;}
        /// <summary>Original safe generation when unambiguously read.</summary>
        public long? Generation{get;}
        /// <summary>Original source payload hash, before any neutral migration.</summary>
        public string? SourcePayloadSha256{get;}
        /// <summary>SHA256 of every original input byte, including whitespace.</summary>
        public string SourceBytesSha256{get;}
        /// <summary>Original validated UTC diagnostic.</summary>
        public string? CreatedAtUtc{get;}
        /// <summary>Original app diagnostic.</summary>
        public string? AppVersion{get;}
        /// <summary>Original build diagnostic.</summary>
        public string? BuildId{get;}
    }
    internal sealed class SaveCodecFailure : Exception
    {
        internal SaveCodecFailure(string code,bool unsupported=false):base(code){Unsupported=unsupported;}
        internal bool Unsupported{get;}
    }
    // A cursor over bytes ALREADY accepted and canonicalized by ICanonicalJson. It only maps fixed DTO fields.
    // No public parse entry, generic DOM, numeric grammar, Unicode validator or independent canonical serializer exists here.
    internal readonly struct CanonicalDtoCursor
    {
        private readonly string? source;private readonly int start;private readonly int length;
        internal CanonicalDtoCursor(string source,int start,int length){this.source=source;this.start=start;this.length=length;}
        internal static CanonicalDtoCursor FromCanonical(byte[] bytes){var s=Encoding.UTF8.GetString(bytes);return new CanonicalDtoCursor(s,0,s.Length);}
        internal bool Missing=>source==null;
        internal bool IsNull=>!Missing&&length==4&&source!.Substring(start,length)=="null";
        internal string Raw=>source?.Substring(start,length)??throw new SaveCodecFailure("SAVE_FIELDS_INVALID");
        internal byte[] Bytes=>Encoding.UTF8.GetBytes(Raw);
        private void Type(char prefix){if(Missing||length<1||source![start]!=prefix)throw new SaveCodecFailure("SAVE_TYPE_INVALID");}
        internal string String(){Type('"');return Decode(source!,start,length);}
        private static string Decode(string s,int p,int n){var b=new StringBuilder();for(int i=p+1;i<p+n-1;i++){char c=s[i];if(c!='\\'){b.Append(c);continue;}c=s[++i];switch(c){case '"':case '\\':case '/':b.Append(c);break;case 'b':b.Append('\b');break;case 'f':b.Append('\f');break;case 'n':b.Append('\n');break;case 'r':b.Append('\r');break;case 't':b.Append('\t');break;case 'u':b.Append((char)int.Parse(s.Substring(i+1,4),NumberStyles.HexNumber,CultureInfo.InvariantCulture));i+=4;break;default:throw new SaveCodecFailure("SAVE_CANONICAL_CURSOR_INVALID");}}return b.ToString();}
        private static int End(string s,int p){char c=s[p];if(c=='"'){for(int i=p+1;i<s.Length;i++){if(s[i]=='\\'){i++;continue;}if(s[i]=='"')return i+1;}throw new SaveCodecFailure("SAVE_CANONICAL_CURSOR_INVALID");}if(c=='{'||c=='['){int depth=0;for(int i=p;i<s.Length;i++){if(s[i]=='"'){i=End(s,i)-1;continue;}if(s[i]=='{'||s[i]=='[')depth++;else if(s[i]=='}'||s[i]==']'){depth--;if(depth==0)return i+1;}}throw new SaveCodecFailure("SAVE_CANONICAL_CURSOR_INVALID");}int end=p;while(end<s.Length&&s[end]!=','&&s[end]!=']'&&s[end]!='}')end++;return end;}
        internal IReadOnlyList<CanonicalDtoCursor> Array(){Type('[');var list=new List<CanonicalDtoCursor>();int p=start+1;while(p<start+length-1){int end=End(source!,p);list.Add(new CanonicalDtoCursor(source!,p,end-p));p=end+1;}return list;}
        internal IReadOnlyList<KeyValuePair<string,CanonicalDtoCursor>> Members(){Type('{');var list=new List<KeyValuePair<string,CanonicalDtoCursor>>();int p=start+1;while(p<start+length-1){int end=End(source!,p);string name=Decode(source!,p,end-p);p=end+1;end=End(source!,p);list.Add(new KeyValuePair<string,CanonicalDtoCursor>(name,new CanonicalDtoCursor(source!,p,end-p)));p=end+1;}return list;}
        internal CanonicalDtoCursor Find(string name){foreach(var pair in Members())if(pair.Key==name)return pair.Value;return default;}
        internal bool Has(string name)=>!Find(name).Missing;
        internal CanonicalDtoCursor[] Fields(params string[] names)=>FieldsOptional(names,System.Array.Empty<string>());
        internal CanonicalDtoCursor[] FieldsOptional(string[] required,string[] optional){var names=required.Concat(optional).ToArray();var slots=new CanonicalDtoCursor[names.Length];foreach(var pair in Members()){int i=System.Array.IndexOf(names,pair.Key);if(i<0||!slots[i].Missing)throw new SaveCodecFailure("SAVE_FIELDS_INVALID");slots[i]=pair.Value;}for(int i=0;i<required.Length;i++)if(slots[i].Missing)throw new SaveCodecFailure("SAVE_FIELDS_INVALID");return slots;}
        internal long Integer(){if(Missing||!long.TryParse(Raw,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out var n)||n < -9007199254740991L||n>9007199254740991L)throw new SaveCodecFailure("SAVE_TYPE_INVALID");return n;}
        internal int Int32(){long n=Integer();if(n<int.MinValue||n>int.MaxValue)throw new SaveCodecFailure("SAVE_INTEGER_INVALID");return(int)n;}
        internal bool Boolean(){if(Raw=="true")return true;if(Raw=="false")return false;throw new SaveCodecFailure("SAVE_TYPE_INVALID");}
        internal Guid Guid(){if(!System.Guid.TryParseExact(String(),"D",out var id)||id==System.Guid.Empty)throw new SaveCodecFailure("SAVE_GUID_INVALID");return id;}
        internal T EnumValue<T>()where T:struct{string text=String();if(!System.Enum.GetNames(typeof(T)).Contains(text)||!System.Enum.TryParse<T>(text,false,out var v)||!System.Enum.IsDefined(typeof(T),v))throw new SaveCodecFailure("SAVE_ENUM_INVALID");return v;}
        internal void Version(string key,long expected,string code){var n=Find(key);if(n.Missing||n.Integer()!=expected)throw new SaveCodecFailure(code,true);}
        internal void RequireString(string expected){if(String()!=expected)throw new SaveCodecFailure("SAVE_VALUE_INVALID");}
        internal void RequireInteger(long expected){if(Integer()!=expected)throw new SaveCodecFailure("SAVE_VALUE_INVALID");}
    }

    /// <summary>Closed typed save-v2 codec and explicit migration dispatch behind the sole injected strict JCS authority.</summary>
    public sealed partial class LocalSaveCodec
    {
        private readonly ICanonicalJson json;private readonly SaveProfileIdentity profile;private readonly CatalogSetIdentity catalogs;private readonly IPublicPuzzleCatalog puzzles;private readonly LocalProgressPolicy policy;private readonly RetainedContractBindings retained;private readonly SaveMigration migration;private readonly ILockedCompletionCatalog completion;private readonly ILockedCampaignCatalog campaign;
        /// <summary>Binds one profile, exact catalogs and explicitly injected passive/historical reader contracts. No test binding is registered implicitly.</summary>
        public LocalSaveCodec(ICanonicalJson json,SaveProfileIdentity profile,CatalogSetIdentity catalogs,IPublicPuzzleCatalog puzzles,ILockedCampaignCatalog campaign,ILockedCompletionCatalog completion,RetainedContractBindings? retained=null,IReadOnlyList<HistoricalProfileBinding>? historicalProfiles=null,IReadOnlyList<LegacyV2ReaderBinding>? legacyV2Readers=null)
        {this.json=json??throw new ArgumentNullException(nameof(json));this.profile=profile??throw new ArgumentNullException(nameof(profile));this.catalogs=catalogs??throw new ArgumentNullException(nameof(catalogs));this.puzzles=puzzles??throw new ArgumentNullException(nameof(puzzles));this.completion=completion;this.campaign=campaign;this.retained=retained??RetainedContractBindings.Empty(profile.Use);if(this.retained.Use!=profile.Use)throw new ArgumentException("SAVE_PROFILE_BINDING_MISMATCH");policy=new LocalProgressPolicy(puzzles,campaign,completion,json,new SaveLedgerCodec(json));migration=new SaveMigration(this,json,profile,catalogs,historicalProfiles??System.Array.Empty<HistoricalProfileBinding>(),legacyV2Readers??System.Array.Empty<LegacyV2ReaderBinding>());}
        /// <summary>Fully reads bounded untrusted bytes, verifies the original payload hash, dispatches a known reader and returns only validated immutable state.</summary>
        public LocalSaveDecodeResult Decode(byte[] source)
        {
            if(source==null)throw new ArgumentNullException(nameof(source));string sourceHash=LocalSaveHash.Bytes(source);int? schema=null;long? generation=null;string? payloadHash=null;string? created=null;string? app=null;string? build=null;
            try
            {
                var canonical=json.Canonicalize(source,LocalSaveHash.Limits);if(!canonical.Succeeded)throw new SaveCodecFailure(canonical.ErrorCode!);var root=CanonicalDtoCursor.FromCanonical(canonical.GetUtf8()!);CanonicalDtoCursor[] f;try{f=root.Fields("saveSchemaVersion","payloadHashProfile","generation","createdAtUtc","appVersion","buildId","contentCatalogHash","payload","payloadSha256");}catch(SaveCodecFailure){throw new SaveCodecFailure("SAVE_ENVELOPE_FIELDS_INVALID");}
                schema=f[0].Int32();if(schema!=1&&schema!=2&&(schema!=3||WriteSchemaVersion!=3))throw new SaveCodecFailure("SAVE_SCHEMA_UNSUPPORTED",true);if(f[1].String()!="STP-SAVE-JCS-1")throw new SaveCodecFailure("SAVE_HASH_PROFILE_UNSUPPORTED",true);generation=f[2].Integer();Require(generation>=0,"SAVE_GENERATION_INVALID");created=Utc(f[3].String());app=Text(f[4].String());build=Text(f[5].String());string contentHash=Hash(f[6].String());payloadHash=Hash(f[8].String());Require(LocalSaveHash.Bytes(f[7].Bytes)==payloadHash,"SAVE_PAYLOAD_HASH_MISMATCH");
                LocalProfileState state;
                if(schema==1)state=migration.ReadAndMigrateV1(f[7],contentHash,generation.Value);
                else {Require(contentHash==catalogs.AggregateCatalog.HashSha256,"SAVE_CATALOG_BINDING_MISMATCH",true);if(f[7].Find("endless").Has("activeDrafts"))migration.RequireLegacyV2(sourceHash,payloadHash,contentHash);state=schema==3?ReadStateV3(f[7]):ReadState(f[7]);}
                if(WriteSchemaVersion==3)state=NormalizeV3(state);
                Validate(state,generation.Value);var snapshot=new SaveSnapshot<LocalProfileState>(profile,catalogs.AggregateCatalog,generation.Value,state);return new LocalSaveDecodeResult(LocalSaveDecodeStatus.Valid,null,snapshot,schema,generation,payloadHash,sourceHash,created,app,build);
            }
            catch(SaveCodecFailure e){return new LocalSaveDecodeResult(e.Unsupported?LocalSaveDecodeStatus.Unsupported:LocalSaveDecodeStatus.Corrupt,e.Message,null,schema,generation,payloadHash,sourceHash,created,app,build);}
            catch(ArgumentException e){return new LocalSaveDecodeResult(LocalSaveDecodeStatus.Corrupt,Stable(e.Message),null,schema,generation,payloadHash,sourceHash,created,app,build);}
            catch(InvalidOperationException){return new LocalSaveDecodeResult(LocalSaveDecodeStatus.Corrupt,"SAVE_RECORD_INVARIANT_INVALID",null,schema,generation,payloadHash,sourceHash,created,app,build);}
            catch(OverflowException){return new LocalSaveDecodeResult(LocalSaveDecodeStatus.Corrupt,"SAVE_INTEGER_INVALID",null,schema,generation,payloadHash,sourceHash,created,app,build);}
        }
        private static string Stable(string value){var first=value.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()??"SAVE_RECORD_INVALID";return first.All(c=>c>='A'&&c<='Z'||c>='0'&&c<='9'||c=='_'||c=='-')?first:"SAVE_RECORD_INVALID";}
        /// <summary>Validates an immutable offered snapshot and emits a canonical v2 envelope. No file operation or hidden compaction occurs.</summary>
        public byte[] Encode(SaveSnapshot<LocalProfileState> snapshot,string createdAtUtc,string appVersion,string buildId)
        {if(snapshot==null)throw new ArgumentNullException(nameof(snapshot));Require(snapshot.Profile.Equals(profile)&&snapshot.Catalog.Equals(catalogs.AggregateCatalog),"SAVE_PROFILE_BINDING_MISMATCH");var state=WriteSchemaVersion==3?NormalizeV3(snapshot.Payload):snapshot.Payload;Validate(state,snapshot.Generation);var payload=PayloadBytes(state);return LocalSaveHash.Canonical(json,Object(("saveSchemaVersion",Write(WriteSchemaVersion)),("payloadHashProfile",Q("STP-SAVE-JCS-1")),("generation",Write(snapshot.Generation)),("createdAtUtc",Q(Utc(createdAtUtc))),("appVersion",Q(Text(appVersion))),("buildId",Q(Text(buildId))),("contentCatalogHash",Q(catalogs.AggregateCatalog.HashSha256)),("payload",Encoding.UTF8.GetString(payload)),("payloadSha256",Q(LocalSaveHash.Bytes(payload)))));}
        /// <summary>Canonical payload-only bytes; the envelope, BOM and line ending never enter its hash.</summary>
        public byte[] PayloadBytes(LocalProfileState state){if(WriteSchemaVersion==3)state=NormalizeV3(state);Validate(state,null);return LocalSaveHash.Canonical(json,WriteSchemaVersion==3?WriteStateV3(state):Write(state));}
        /// <summary>Exact bound profile identity.</summary>
        public SaveProfileIdentity Profile=>profile;
        /// <summary>Exact bound aggregate catalog identity.</summary>
        public CatalogIdentity Catalog=>catalogs.AggregateCatalog;
        internal ILockedCampaignCatalog TargetCampaign=>campaign;
        internal LocalSeasonProgressCache CacheFor(ILockedCampaignCatalog graph,IReadOnlyList<LocalLevelProgress> levels)=>new LocalProgressPolicy(puzzles,graph,completion,json,new SaveLedgerCodec(json)).Cache(levels);
        internal LocalSeasonProgressCache CacheCurrent(IReadOnlyList<LocalLevelProgress> levels)=>policy.Cache(levels);
        internal void Validate(LocalProfileState state,long? generation)
        {
            Require(profile.Use!=CatalogUse.InternalPreview||WriteSchemaVersion==3,"SAVE_CANDIDATE_V3_REQUIRED",true);
            Require(WriteSchemaVersion==3||state.Metagame==null,"SAVE_SCHEMA_UNSUPPORTED",true);if(WriteSchemaVersion==3){ValidateMetagameV3(state,generation);ValidateRuntimeEndless(state,generation);}
            Require(state.Profile.Identity.Equals(profile)&&state.Profile.CatalogSet.Equals(catalogs),"SAVE_PROFILE_BINDING_MISMATCH",true);retained.Validate(state,json);ValidateProgressAuthority(state);
            foreach(var d in state.Drafts)if(puzzles.TryGet(d.Identity,out var p)&&p!=null)Require(d.Checkpoint.Restore(p.ToDomainDefinition(),d.Identity.PuzzleId,d.Identity.HashProfile,d.Identity.HashSha256).Succeeded,"CHECKPOINT_UNDO_REPLAY_INVALID");
            if(generation.HasValue){Require(state.LedgerCheckpoint.SourceSaveGeneration<=generation.Value,"SAVE_LEDGER_GENERATION_INVALID");foreach(var l in state.Levels)foreach(var t in new[]{l.FirstClear?.Terminal,l.SecondStarPaid,l.ThirdStarPaid})if(t!=null)Require(t.CommittedGeneration<=generation.Value,"SAVE_TERMINAL_GENERATION_INVALID");foreach(var c in state.RewardClaims){var t=c is CampaignProgressTerminal a?a.Terminal:c is PostClearRewardClaim r?r.Terminal:null;if(t!=null)Require(t.CommittedGeneration<=generation.Value,"SAVE_TERMINAL_GENERATION_INVALID");}}
            policy.Validate(state);
        }
        private CompletionValues BoundValues(CatalogIdentity source,PublicPuzzleIdentity identity)
        {
            if(source.Equals(completion.Identity)&&completion.TryGet(identity,out var current)&&current!=null)return current;
            var histories=retained.HistoricalProgress.Where(h=>h.Completion.Identity.Equals(source)&&h.Completion.TryGet(identity,out _)).ToArray();Require(histories.Length==1,"SAVE_COMPLETION_BINDING_UNSUPPORTED",true);histories[0].Completion.TryGet(identity,out var values);Require(values!=null,"SAVE_COMPLETION_BINDING_UNSUPPORTED",true);return values!;
        }
        private void ValidateProgressAuthority(LocalProfileState state)
        {
            foreach(var level in state.Levels)
            {
                Require(!puzzles.Puzzles.Any(p=>p.Identity.PuzzleId==level.Identity.PuzzleId&&!p.Identity.Equals(level.Identity)),"LVL-PUBLISHED-PUZZLE-MUTATED");
                void Terminal(EconomyTerminal t,string reason,Func<CompletionValues,int> amount){var v=BoundValues(t.Catalog,level.Identity);Require(t.ReasonCode==reason&&t.SubjectId==level.Identity.PuzzleId&&t.Amount==amount(v),"SAVE_TERMINAL_VALUE_MISMATCH");}
                if(level.FirstClear!=null)Terminal(level.FirstClear.Terminal,"LEVEL_FIRST_CLEAR",v=>v.FirstClear);if(level.SecondStarPaid!=null)Terminal(level.SecondStarPaid,"LEVEL_SECOND_STAR",v=>v.SecondStar);if(level.ThirdStarPaid!=null)Terminal(level.ThirdStarPaid,"LEVEL_THIRD_STAR",v=>v.ThirdStar);
            }
            foreach(var claim in state.RewardClaims.OfType<CampaignProgressTerminal>())
            {
                var source=claim.Terminal.Catalog;ILockedCampaignCatalog graph;
                bool InGraph(ILockedCampaignCatalog c)=>c.Entries.Any(e=>(claim.SubjectKind=="ROUTE"?e.RouteId:e.SectionId)==claim.SubjectId);
                if(source.Equals(completion.Identity)&&InGraph(campaign))graph=campaign;
                else {var known=retained.HistoricalProgress.Where(h=>h.Completion.Identity.Equals(source)&&InGraph(h.Campaign)).ToArray();Require(known.Length==1,"SAVE_COMPLETION_BINDING_UNSUPPORTED",true);graph=known[0].Campaign;}
                var members=graph.Entries.Where(e=>(claim.SubjectKind=="ROUTE"?e.RouteId:e.SectionId)==claim.SubjectId).ToArray();Require(members.Length>0&&members.All(e=>state.Levels.Any(l=>l.Identity.Equals(e.Puzzle)&&l.FirstClear!=null)),"SAVE_SUBJECT_TERMINAL_INVALID");var values=members.Select(e=>BoundValues(source,e.Puzzle)).ToArray();int amount=claim.SubjectKind=="ROUTE"?values[0].RouteClear:values[0].SectionClear;Require(values.All(v=>(claim.SubjectKind=="ROUTE"?v.RouteClear:v.SectionClear)==amount)&&claim.Terminal.Amount==amount&&claim.Terminal.ReasonCode==(claim.SubjectKind=="ROUTE"?"ROUTE_CLEAR":"SECTION_CLEAR"),"SAVE_SUBJECT_TERMINAL_INVALID");
            }
        }
        internal static void Require(bool condition,string code,bool unsupported=false){if(!condition)throw new SaveCodecFailure(code,unsupported);}
        internal static string Text(string s){Require(!string.IsNullOrWhiteSpace(s),"SAVE_TEXT_INVALID");return s;}
        internal static string Hash(string s){Require(s.Length==64&&s.All(c=>c>='0'&&c<='9'||c>='a'&&c<='f'),"SAVE_HASH_INVALID");return s;}
        internal static string Utc(string s){Require(System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,7})?Z$")&&DateTimeOffset.TryParse(s,CultureInfo.InvariantCulture,DateTimeStyles.None,out var v)&&v.Offset==TimeSpan.Zero,"SAVE_UTC_INVALID");return s;}
        internal static string Object(params(string Key,string Value)[] fields)=>"{"+string.Join(",",fields.Select(f=>Q(f.Key)+":"+f.Value))+"}";
        internal static string Q(string? text)=>text==null?"null":LocalSaveHash.Quote(text);
        internal static string Write(string? text)=>Q(text);
        internal static string Write(long v)=>v.ToString(CultureInfo.InvariantCulture);
        internal static string Write(int v)=>v.ToString(CultureInfo.InvariantCulture);
        internal static string Write(long? v)=>v.HasValue?Write(v.Value):"null";
        internal static string Write(int? v)=>v.HasValue?Write(v.Value):"null";
        internal static string Write(bool v)=>v?"true":"false";
        internal static string Write(bool? v)=>v.HasValue?Write(v.Value):"null";
        internal static string Write(Guid v)=>Q(v.ToString("D"));
        internal static string Write(SessionMode v)=>Q(v.ToString());
        internal static string Write(SessionPhase v)=>Q(v.ToString());
        internal static string Write(CellContent v)=>Q(v.ToString());
        internal PublicPuzzleIdentity ReadPublicPuzzleIdentity(CanonicalDtoCursor v){var f=v.Fields("puzzleId","profile","sha256");Require(f[1].String()=="STP-PUZZLE-SEMANTIC-JCS-1","SAVE_PUBLIC_HASH_PROFILE_UNSUPPORTED",true);return new PublicPuzzleIdentity(f[0].String(),f[1].String(),f[2].String());}
        internal static string Write(PublicPuzzleIdentity v)=>Object(("puzzleId",Q(v.PuzzleId)),("profile",Q(v.HashProfile)),("sha256",Q(v.HashSha256)));
        internal SaveProfileIdentity ReadSaveProfileIdentity(CanonicalDtoCursor v){var f=v.Fields("namespace","profileId","use");return new SaveProfileIdentity(f[0].String(),f[1].String(),f[2].EnumValue<CatalogUse>());}
        internal static string Write(SaveProfileIdentity v)=>Object(("namespace",Q(v.Namespace)),("profileId",Q(v.ProfileId)),("use",Q(v.Use.ToString())));
        internal CatalogIdentity ReadCatalogIdentity(CanonicalDtoCursor v){var f=v.Fields("catalogId","revision","hashSha256","use","approvalStatus");return new CatalogIdentity(f[0].String(),f[1].Int32(),f[2].String(),f[3].EnumValue<CatalogUse>(),f[4].EnumValue<CatalogApprovalStatus>());}
        internal static string Write(CatalogIdentity v)=>Object(("catalogId",Q(v.CatalogId)),("revision",Write(v.Revision)),("hashSha256",Q(v.HashSha256)),("use",Q(v.Use.ToString())),("approvalStatus",Q(v.ApprovalStatus.ToString())));
        internal CatalogSetIdentity ReadCatalogSetIdentity(CanonicalDtoCursor v){v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");var f=v.Fields("recordVersion","publicCatalog","campaignCatalog","completionCatalog","aggregateCatalog");return CatalogSetIdentity.Create(ReadCatalogIdentity(f[1]),ReadCatalogIdentity(f[2]),ReadCatalogIdentity(f[3]),ReadCatalogIdentity(f[4]),json);}
        internal static string Write(CatalogSetIdentity v)=>Object(("recordVersion","1"),("publicCatalog",Write(v.PublicCatalog)),("campaignCatalog",Write(v.CampaignCatalog)),("completionCatalog",Write(v.CompletionCatalog)),("aggregateCatalog",Write(v.AggregateCatalog)));
        internal PublicPuzzleInput ReadPublicPuzzleInput(CanonicalDtoCursor v){var f=v.Fields("puzzleId","rulesetVersion","grid","endpoints","rowCounts","columnCounts");var g=f[2].Fields("width","height");var ep=f[3].Fields("a","b");var a=ep[0].Fields("side","index");var b=ep[1].Fields("side","index");return new PublicPuzzleInput(new PublicPuzzleIdentity(f[0].String(),"STP-PUZZLE-SEMANTIC-JCS-1",LocalSaveHash.Bytes(v.Bytes)),f[1].String(),g[0].Int32(),g[1].Int32(),a[0].EnumValue<PublicPuzzleSide>(),a[1].Int32(),b[0].EnumValue<PublicPuzzleSide>(),b[1].Int32(),f[4].Array().Select(x=>x.Int32()).ToArray(),f[5].Array().Select(x=>x.Int32()).ToArray());}
        internal static string Write(PublicPuzzleInput v)=>Object(("puzzleId",Q(v.Identity.PuzzleId)),("rulesetVersion",Q(v.RulesetVersion)),("grid",Object(("width",Write(v.Width)),("height",Write(v.Height)))),("endpoints",Object(("a",Object(("side",Q(v.A.Side.ToString())),("index",Write(v.A.Index)))),("b",Object(("side",Q(v.B.Side.ToString())),("index",Write(v.B.Index)))))),("rowCounts","["+string.Join(",",v.RowCounts.Select(Write))+"]"),("columnCounts","["+string.Join(",",v.ColumnCounts.Select(Write))+"]"));
        internal PuzzleSessionCheckpoint ReadCheckpoint(CanonicalDtoCursor v,PublicPuzzleIdentity? legacyIdentity=null)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");var f=v.Fields("recordVersion","identity","rulesetVersion","attemptId","mode","phase","cells","previouslySet","revision","timerStarted","activeElapsedMs","usedEmptyMarker","usedOccupiedMarker","hintCount","correctionCount","appliedCommandIds","undo","solvedEventId","startedAtUtc","lastSavedAtUtc");var id=legacyIdentity??ReadPublicPuzzleIdentity(f[1]);var diffs=f[16].Array().Select(d=>{var a=d.Fields("changes");return new CheckpointCellDiff(a[0].Array().Select(c=>{var z=c.Fields("x","y","before","after");return new CheckpointCellChange(z[0].Int32(),z[1].Int32(),z[2].EnumValue<CellContent>(),z[3].EnumValue<CellContent>());}).ToArray());}).ToArray();string? started=f[18].IsNull?null:Utc(f[18].String());string? saved=f[19].IsNull?null:Utc(f[19].String());return new PuzzleSessionCheckpoint(id.PuzzleId,id.HashProfile,id.HashSha256,f[2].String(),f[3].Guid(),f[4].EnumValue<SessionMode>(),f[5].EnumValue<SessionPhase>(),f[6].Array().Select(x=>x.EnumValue<CellContent>()).ToArray(),f[7].Array().Select(x=>x.Boolean()).ToArray(),f[8].Integer(),f[9].Boolean(),f[10].Integer(),f[11].Boolean(),f[12].Boolean(),f[13].Int32(),f[14].Integer(),f[15].Array().Select(x=>x.Guid()).ToArray(),diffs,f[17].IsNull?null:f[17].String(),started==null?(DateTimeOffset?)null:DateTimeOffset.Parse(started,CultureInfo.InvariantCulture),saved==null?(DateTimeOffset?)null:DateTimeOffset.Parse(saved,CultureInfo.InvariantCulture)).WithDiagnosticText(started,saved);
        }
        internal static string Write(PuzzleSessionCheckpoint v)=>Object(("recordVersion","1"),("identity",Write(new PublicPuzzleIdentity(v.PuzzleId,v.PublicPuzzleHashProfile,v.PublicPuzzleHashSha256))),("rulesetVersion",Q(v.RulesetVersion)),("attemptId",Write(v.AttemptId)),("mode",Write(v.Mode)),("phase",Write(v.Phase)),("cells","["+string.Join(",",v.Cells.Select(Write))+"]"),("previouslySet","["+string.Join(",",v.PreviouslySet.Select(Write))+"]"),("revision",Write(v.Revision)),("timerStarted",Write(v.TimerStarted)),("activeElapsedMs",Write(v.ActiveElapsedMs)),("usedEmptyMarker",Write(v.UsedEmptyMarker)),("usedOccupiedMarker",Write(v.UsedOccupiedMarker)),("hintCount",Write(v.HintCount)),("correctionCount",Write(v.CorrectionCount)),("appliedCommandIds","["+string.Join(",",v.AppliedCommandIds.Select(Write))+"]"),("undo","["+string.Join(",",v.UndoStack.Select(d=>Object(("changes","["+string.Join(",",d.Changes.Select(c=>Object(("x",Write(c.X)),("y",Write(c.Y)),("before",Write(c.Before)),("after",Write(c.After)))))+"]"))))+"]"),("solvedEventId",Q(v.SolvedEventId)),("startedAtUtc",Q(v.StartedAtUtcText)),("lastSavedAtUtc",Q(v.LastSavedAtUtcText)));
        internal LocalProfileState ReadState(CanonicalDtoCursor v)
        {var f=v.FieldsOptional(new[]{"profile","levels","endless","ledgerCheckpoint","economyJournal","inventory","cosmeticClaimReservations","seasonProgressCache","drafts","rewardClaims","dailyStatus","entitlements","pendingOperations","settings","dataLifecycle"},new[]{"hintEntitlement"});return LocalProfileState.Create(ReadLocalProfileHeader(f[0]),f[1].Array().Select(ReadLocalLevelProgress).ToArray(),ReadEndless(f[2]),ReadLocalLedgerCheckpoint(f[3]),f[4].Array().Select(ReadLocalStoredEntry).ToArray(),ReadLocalInventory(f[5]),f[6].Array().Select(ReadLocalCosmeticReservation).ToArray(),ReadLocalSeasonProgressCache(f[7]),f[8].Array().Select(ReadLocalDraft).ToArray(),f[9].Array().Select(ReadLocalRewardClaim).ToArray(),f[10].IsNull?null:ReadLocalDailyStatus(f[10]),ReadLocalEntitlements(f[11]),f[12].Array().Select(ReadLocalPendingOperation).ToArray(),ReadLocalSettings(f[13]),ReadLocalDataLifecycle(f[14]),f[15].Missing||f[15].IsNull?null:ReadLocalHintEntitlement(f[15]));}
        internal string Write(LocalProfileState v)=>Object(("profile",Write(v.Profile)),("levels","["+string.Join(",",v.Levels.Select(Write))+"]"),("endless",Write(v.Endless)),("ledgerCheckpoint",Write(v.LedgerCheckpoint)),("economyJournal","["+string.Join(",",v.EconomyJournal.Select(Write))+"]"),("inventory",Write(v.Inventory)),("cosmeticClaimReservations","["+string.Join(",",v.CosmeticClaimReservations.Select(Write))+"]"),("seasonProgressCache",Write(v.SeasonProgressCache)),("drafts","["+string.Join(",",v.Drafts.Select(Write))+"]"),("rewardClaims","["+string.Join(",",v.RewardClaims.Select(Write))+"]"),("dailyStatus",v.DailyStatus==null?"null":Write(v.DailyStatus)),("entitlements",Write(v.Entitlements)),("pendingOperations","["+string.Join(",",v.PendingOperations.Select(Write))+"]"),("settings",Write(v.Settings)),("dataLifecycle",Write(v.DataLifecycle)),("hintEntitlement",v.HintEntitlement==null?"null":Write(v.HintEntitlement)));
        internal LocalOwnership ReadLocalOwnership(CanonicalDtoCursor v){v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");switch(v.Find("grantKind").String()){case"DEFAULT":return ReadDefaultOwnership(v);case"PATIENCE_PURCHASE":return ReadPurchasedOwnership(v);case"MILESTONE_CLAIM":return ReadMilestoneOwnership(v);case"LEGACY":return ReadLegacyOwnership(v);default:throw new SaveCodecFailure("SAVE_OWNERSHIP_VARIANT_UNSUPPORTED",true);}}
        internal string Write(LocalOwnership v)=>v is DefaultOwnership d?Write(d):v is PurchasedOwnership p?Write(p):v is MilestoneOwnership m?Write(m):v is LegacyOwnership l?Write(l):throw new SaveCodecFailure("SAVE_OWNERSHIP_VARIANT_UNSUPPORTED",true);
        internal LocalRewardClaim ReadLocalRewardClaim(CanonicalDtoCursor v){v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");switch(v.Find("kind").String()){case"CAMPAIGN_PROGRESS_TERMINAL":return ReadCampaignProgressTerminal(v);case"POST_CLEAR_PATIENCE":return ReadPostClearRewardClaim(v);default:throw new SaveCodecFailure("SAVE_REWARD_VARIANT_UNSUPPORTED",true);}}
        internal string Write(LocalRewardClaim v)=>v is CampaignProgressTerminal c?Write(c):v is PostClearRewardClaim p?Write(p):throw new SaveCodecFailure("SAVE_REWARD_VARIANT_UNSUPPORTED",true);
        internal LocalPendingOperation ReadLocalPendingOperation(CanonicalDtoCursor v){v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");switch(v.Find("kind").String()){case"IAP":return ReadLocalIapOperation(v);case"REWARD":return ReadLocalRewardOperation(v);default:throw new SaveCodecFailure("SAVE_OPERATION_VARIANT_UNSUPPORTED",true);}}
        internal string Write(LocalPendingOperation v)=>v is LocalIapOperation i?Write(i):v is LocalRewardOperation r?Write(r):throw new SaveCodecFailure("SAVE_OPERATION_VARIANT_UNSUPPORTED",true);
        internal LocalOpenEndless ReadLocalOpenEndless(CanonicalDtoCursor v){if(v.Find("state").String()!="COMPLETION_CLAIM_OPEN")v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");switch(v.Find("state").String()){case"RESERVED_NOT_GENERATED":return ReadReservedEndless(v);case"ACTIVE_DRAFT":return ReadActiveEndlessDraft(v);case"COMPLETION_CLAIM_OPEN":return ReadOpenEndlessCompletion(v);default:throw new SaveCodecFailure("SAVE_ENDLESS_VARIANT_UNSUPPORTED",true);}}
        internal string Write(LocalOpenEndless v)=>v is ReservedEndless r?Write(r):v is ActiveEndlessDraft a?Write(a):v is OpenEndlessCompletion c?Write(c):throw new SaveCodecFailure("SAVE_ENDLESS_VARIANT_UNSUPPORTED",true);
        internal RetainedGeneratorParameters ReadParameters(CanonicalDtoCursor v){if(v.Has("contractVersion"))return ReadSearchParameters(v);if(v.Missing||!v.Has("gridWidth")||!v.Has("gridHeight"))throw new SaveCodecFailure("SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);return ReadContractGeneratorParameters(v);}
        internal string Write(RetainedGeneratorParameters v)=>v is SearchGeneratorParameters s?s.Projection():v is ContractGeneratorParameters p?Write(p):throw new SaveCodecFailure("SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);
        internal LocalEndlessState ReadEndless(CanonicalDtoCursor v){if(!v.Has("activeDrafts"))return ReadLocalEndlessState(v);v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");var f=v.Fields("recordVersion","highestReservedOrdinal","activeDrafts");return LocalEndlessState.Create(f[1].String(),f[2].Array().Select(ReadActiveEndlessDraft).Cast<LocalOpenEndless>().ToArray());}

        internal LocalProfileHeader ReadLocalProfileHeader(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("localProfileId","firstUseUtc","identity","catalogSet","recordVersion");
            return LocalProfileHeader.Create(f[0].String(),f[1].String(),ReadSaveProfileIdentity(f[2]),ReadCatalogSetIdentity(f[3]));
        }
        internal string Write(LocalProfileHeader v)=>Object(("localProfileId",Write(v.LocalProfileId)),("firstUseUtc",Write(v.FirstUseUtc)),("identity",Write(v.Identity)),("catalogSet",Write(v.CatalogSet)),("recordVersion","1"));
        internal EconomyTerminal ReadEconomyTerminal(CanonicalDtoCursor v)
        {
            var f=v.Fields("transactionId","reasonCode","subjectId","catalog","claimId","amount","committedGeneration","createdAtUtc");
            return EconomyTerminal.Create(f[0].String(),f[1].String(),f[2].String(),ReadCatalogIdentity(f[3]),f[4].String(),f[5].Integer(),f[6].Integer(),f[7].String());
        }
        internal string Write(EconomyTerminal v)=>Object(("transactionId",Write(v.TransactionId)),("reasonCode",Write(v.ReasonCode)),("subjectId",Write(v.SubjectId)),("catalog",Write(v.Catalog)),("claimId",Write(v.ClaimId)),("amount",Write(v.Amount)),("committedGeneration",Write(v.CommittedGeneration)),("createdAtUtc",Write(v.CreatedAtUtc)));
        internal LocalFirstClear ReadLocalFirstClear(CanonicalDtoCursor v)
        {
            var f=v.Fields("attemptId","solvedEventId","completedAtUtc","activeElapsedMs","terminal");
            return LocalFirstClear.Create(f[0].Guid(),f[1].String(),f[2].String(),f[3].Integer(),ReadEconomyTerminal(f[4]));
        }
        internal string Write(LocalFirstClear v)=>Object(("attemptId",Write(v.AttemptId)),("solvedEventId",Write(v.SolvedEventId)),("completedAtUtc",Write(v.CompletedAtUtc)),("activeElapsedMs",Write(v.ActiveElapsedMs)),("terminal",v.Terminal==null?"null":Write(v.Terminal)));
        internal LocalLevelProgress ReadLocalLevelProgress(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("identity","firstClear","starsHighWater","firstRunBestElapsedMs","revisionBestElapsedMs","directSolved","secondStarPaid","thirdStarPaid","recordVersion");
            return LocalLevelProgress.Create(ReadPublicPuzzleIdentity(f[0]),f[1].IsNull?(LocalFirstClear?)null:ReadLocalFirstClear(f[1]),f[2].Int32(),f[3].IsNull?(long?)null:f[3].Integer(),f[4].IsNull?(long?)null:f[4].Integer(),f[5].Boolean(),f[6].IsNull?(EconomyTerminal?)null:ReadEconomyTerminal(f[6]),f[7].IsNull?(EconomyTerminal?)null:ReadEconomyTerminal(f[7]));
        }
        internal string Write(LocalLevelProgress v)=>Object(("identity",Write(v.Identity)),("firstClear",v.FirstClear==null?"null":Write(v.FirstClear)),("starsHighWater",Write(v.StarsHighWater)),("firstRunBestElapsedMs",Write(v.FirstRunBestElapsedMs)),("revisionBestElapsedMs",Write(v.RevisionBestElapsedMs)),("directSolved",Write(v.DirectSolved)),("secondStarPaid",v.SecondStarPaid==null?"null":Write(v.SecondStarPaid)),("thirdStarPaid",v.ThirdStarPaid==null?"null":Write(v.ThirdStarPaid)),("recordVersion","1"));
        internal LocalDraft ReadLocalDraft(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("identity","savedAtUtc","checkpoint","recordVersion");
            return LocalDraft.Create(ReadPublicPuzzleIdentity(f[0]),f[1].String(),ReadCheckpoint(f[2]));
        }
        internal string Write(LocalDraft v)=>Object(("identity",Write(v.Identity)),("savedAtUtc",Write(v.SavedAtUtc)),("checkpoint",Write(v.Checkpoint)),("recordVersion","1"));
        internal LocalStoredEntry ReadLocalStoredEntry(CanonicalDtoCursor v)
        {
            var f=v.Fields("sequence","transactionId","reasonCode","amount","subjectId","catalogId","catalogRevision","catalogHashSha256","claimId","createdAtUtc","entryChainHashSha256");
            return LocalStoredEntry.Create(f[0].Integer(),f[1].String(),f[2].String(),f[3].Integer(),f[4].String(),f[5].IsNull?(string?)null:f[5].String(),f[6].IsNull?(int?)null:f[6].Int32(),f[7].IsNull?(string?)null:f[7].String(),f[8].IsNull?(string?)null:f[8].String(),f[9].String(),f[10].String());
        }
        internal string Write(LocalStoredEntry v)=>Object(("sequence",Write(v.Sequence)),("transactionId",Write(v.TransactionId)),("reasonCode",Write(v.ReasonCode)),("amount",Write(v.Amount)),("subjectId",Write(v.SubjectId)),("catalogId",Write(v.CatalogId)),("catalogRevision",Write(v.CatalogRevision)),("catalogHashSha256",Write(v.CatalogHashSha256)),("claimId",Write(v.ClaimId)),("createdAtUtc",Write(v.CreatedAtUtc)),("entryChainHashSha256",Write(v.EntryChainHashSha256)));
        internal LocalLedgerCheckpoint ReadLocalLedgerCheckpoint(CanonicalDtoCursor v)
        {
            v.Version("ledgerContractVersion",1,"SAVE_LEDGER_ALGORITHM_UNSUPPORTED");
            var f=v.Fields("throughSequence","balancePatience","entryChainHashSha256","sourceSaveGeneration","journalDiagnosticTail","ledgerContractVersion");
            return LocalLedgerCheckpoint.Create(f[0].Integer(),f[1].Integer(),f[2].String(),f[3].Integer(),f[4].Array().Select(x=>ReadLocalStoredEntry(x)).ToArray());
        }
        internal string Write(LocalLedgerCheckpoint v)=>Object(("throughSequence",Write(v.ThroughSequence)),("balancePatience",Write(v.BalancePatience)),("entryChainHashSha256",Write(v.EntryChainHashSha256)),("sourceSaveGeneration",Write(v.SourceSaveGeneration)),("journalDiagnosticTail","["+string.Join(",",v.JournalDiagnosticTail.Select(x=>Write(x)))+"]"),("ledgerContractVersion","1"));
        internal LocalSubjectProgress ReadLocalSubjectProgress(CanonicalDtoCursor v)
        {
            var f=v.Fields("subjectId","kind","firstClearPuzzleIds","firstClearCount","memberCount","completed");
            return LocalSubjectProgress.Create(f[0].String(),f[1].String(),f[2].Array().Select(x=>x.String()).ToArray(),f[3].Int32(),f[4].Int32(),f[5].Boolean());
        }
        internal string Write(LocalSubjectProgress v)=>Object(("subjectId",Write(v.SubjectId)),("kind",Write(v.Kind)),("firstClearPuzzleIds","["+string.Join(",",v.FirstClearPuzzleIds.Select(x=>Write(x)))+"]"),("firstClearCount",Write(v.FirstClearCount)),("memberCount",Write(v.MemberCount)),("completed",Write(v.Completed)));
        internal LocalSeasonProgressCache ReadLocalSeasonProgressCache(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("campaignCatalog","sourceProjectionHashSha256","subjects","recordVersion");
            return LocalSeasonProgressCache.Create(ReadCatalogIdentity(f[0]),f[1].String(),f[2].Array().Select(x=>ReadLocalSubjectProgress(x)).ToArray());
        }
        internal string Write(LocalSeasonProgressCache v)=>Object(("campaignCatalog",Write(v.CampaignCatalog)),("sourceProjectionHashSha256",Write(v.SourceProjectionHashSha256)),("subjects","["+string.Join(",",v.Subjects.Select(x=>Write(x)))+"]"),("recordVersion","1"));
        internal DefaultOwnership ReadDefaultOwnership(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("itemId","itemType","catalog","acquiredAtUtc","recordVersion","grantKind");
            f[5].RequireString("DEFAULT");
            return DefaultOwnership.Create(f[0].String(),f[1].String(),ReadCatalogIdentity(f[2]),f[3].IsNull?(string?)null:f[3].String());
        }
        internal string Write(DefaultOwnership v)=>Object(("itemId",Write(v.ItemId)),("itemType",Write(v.ItemType)),("catalog",Write(v.Catalog)),("acquiredAtUtc",Write(v.AcquiredAtUtc)),("recordVersion","1"),("grantKind",Q("DEFAULT")));
        internal PurchasedOwnership ReadPurchasedOwnership(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("itemId","itemType","catalog","transactionId","pricePatience","acquiredAtUtc","recordVersion","grantKind");
            f[7].RequireString("PATIENCE_PURCHASE");
            return PurchasedOwnership.Create(f[0].String(),f[1].String(),ReadCatalogIdentity(f[2]),f[3].String(),f[4].Integer(),f[5].String());
        }
        internal string Write(PurchasedOwnership v)=>Object(("itemId",Write(v.ItemId)),("itemType",Write(v.ItemType)),("catalog",Write(v.Catalog)),("transactionId",Write(v.TransactionId)),("pricePatience",Write(v.PricePatience)),("acquiredAtUtc",Write(v.AcquiredAtUtc)),("recordVersion","1"),("grantKind",Q("PATIENCE_PURCHASE")));
        internal MilestoneOwnership ReadMilestoneOwnership(CanonicalDtoCursor v)
        {
            v.Version("eligibilityContractVersion",1,"SAVE_ELIGIBILITY_CONTRACT_UNSUPPORTED");
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("itemId","itemType","catalog","claimId","operationId","claimGeneration","eligibilityProjectionHashSha256","acquiredAtUtc","recordVersion","grantKind","acquisition","eligibilityContractVersion","claimState");
            f[9].RequireString("MILESTONE_CLAIM");
            f[10].RequireString("MILESTONE_GRANT");
            f[11].RequireInteger(1);
            f[12].RequireString("COMMITTED");
            return MilestoneOwnership.Create(f[0].String(),f[1].String(),ReadCatalogIdentity(f[2]),f[3].String(),f[4].String(),f[5].Integer(),f[6].String(),f[7].String());
        }
        internal string Write(MilestoneOwnership v)=>Object(("itemId",Write(v.ItemId)),("itemType",Write(v.ItemType)),("catalog",Write(v.Catalog)),("claimId",Write(v.ClaimId)),("operationId",Write(v.OperationId)),("claimGeneration",Write(v.ClaimGeneration)),("eligibilityProjectionHashSha256",Write(v.EligibilityProjectionHashSha256)),("acquiredAtUtc",Write(v.AcquiredAtUtc)),("recordVersion","1"),("grantKind",Q("MILESTONE_CLAIM")),("acquisition",Q("MILESTONE_GRANT")),("eligibilityContractVersion","1"),("claimState",Q("COMMITTED")));
        internal LegacyOwnership ReadLegacyOwnership(CanonicalDtoCursor v)
        {
            v.Version("sourceSaveSchemaVersion",1,"SAVE_LEGACY_READER_UNSUPPORTED");
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("itemId","itemType","legacyPurchaseTransactionId","legacyAcquiredAtUtc","legacyGrantCode","recordVersion","grantKind","sourceSaveSchemaVersion");
            f[6].RequireString("LEGACY");
            f[7].RequireInteger(1);
            return LegacyOwnership.Create(f[0].String(),f[1].String(),f[2].IsNull?(string?)null:f[2].String(),f[3].IsNull?(string?)null:f[3].String(),f[4].IsNull?(string?)null:f[4].String());
        }
        internal string Write(LegacyOwnership v)=>Object(("itemId",Write(v.ItemId)),("itemType",Write(v.ItemType)),("legacyPurchaseTransactionId",Write(v.LegacyPurchaseTransactionId)),("legacyAcquiredAtUtc",Write(v.LegacyAcquiredAtUtc)),("legacyGrantCode",Write(v.LegacyGrantCode)),("recordVersion","1"),("grantKind",Q("LEGACY")),("sourceSaveSchemaVersion","1"));
        internal LocalInventory ReadLocalInventory(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("ownership","selectedItemIds","recordVersion");
            return LocalInventory.Create(f[0].Array().Select(x=>ReadLocalOwnership(x)).ToArray(),f[1].Array().Select(x=>x.String()).ToArray());
        }
        internal string Write(LocalInventory v)=>Object(("ownership","["+string.Join(",",v.Ownership.Select(x=>Write(x)))+"]"),("selectedItemIds","["+string.Join(",",v.SelectedItemIds.Select(x=>Write(x)))+"]"),("recordVersion","1"));
        internal LocalCosmeticReservation ReadLocalCosmeticReservation(CanonicalDtoCursor v)
        {
            v.Version("eligibilityContractVersion",1,"SAVE_ELIGIBILITY_CONTRACT_UNSUPPORTED");
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("claimId","operationId","claimGeneration","itemId","catalog","eligibilityProjectionHashSha256","recordVersion","acquisition","eligibilityContractVersion","claimState");
            f[7].RequireString("MILESTONE_GRANT");
            f[8].RequireInteger(1);
            f[9].RequireString("RESERVED");
            return LocalCosmeticReservation.Create(f[0].String(),f[1].String(),f[2].Integer(),f[3].String(),ReadCatalogIdentity(f[4]),f[5].String());
        }
        internal string Write(LocalCosmeticReservation v)=>Object(("claimId",Write(v.ClaimId)),("operationId",Write(v.OperationId)),("claimGeneration",Write(v.ClaimGeneration)),("itemId",Write(v.ItemId)),("catalog",Write(v.Catalog)),("eligibilityProjectionHashSha256",Write(v.EligibilityProjectionHashSha256)),("recordVersion","1"),("acquisition",Q("MILESTONE_GRANT")),("eligibilityContractVersion","1"),("claimState",Q("RESERVED")));
        internal CampaignProgressTerminal ReadCampaignProgressTerminal(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("subjectKind","subjectId","terminal","recordVersion","kind");
            f[4].RequireString("CAMPAIGN_PROGRESS_TERMINAL");
            return CampaignProgressTerminal.Create(f[0].String(),f[1].String(),ReadEconomyTerminal(f[2]));
        }
        internal string Write(CampaignProgressTerminal v)=>Object(("subjectKind",Write(v.SubjectKind)),("subjectId",Write(v.SubjectId)),("terminal",v.Terminal==null?"null":Write(v.Terminal)),("recordVersion","1"),("kind",Q("CAMPAIGN_PROGRESS_TERMINAL")));
        internal PostClearRewardClaim ReadPostClearRewardClaim(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("puzzleIdentity","claimId","localOperationId","providerOperationId","claimStatus","completionCatalog","terminal","recordVersion","kind");
            f[8].RequireString("POST_CLEAR_PATIENCE");
            return PostClearRewardClaim.Create(ReadPublicPuzzleIdentity(f[0]),f[1].String(),f[2].String(),f[3].IsNull?(string?)null:f[3].String(),f[4].String(),ReadCatalogIdentity(f[5]),f[6].IsNull?(EconomyTerminal?)null:ReadEconomyTerminal(f[6]));
        }
        internal string Write(PostClearRewardClaim v)=>Object(("puzzleIdentity",Write(v.PuzzleIdentity)),("claimId",Write(v.ClaimId)),("localOperationId",Write(v.LocalOperationId)),("providerOperationId",Write(v.ProviderOperationId)),("claimStatus",Write(v.ClaimStatus)),("completionCatalog",Write(v.CompletionCatalog)),("terminal",v.Terminal==null?"null":Write(v.Terminal)),("recordVersion","1"),("kind",Q("POST_CLEAR_PATIENCE")));
        internal LocalIapOperation ReadLocalIapOperation(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("operationId","logicalProductKey","store","transactionReference","state","recordVersion","kind");
            f[6].RequireString("IAP");
            return LocalIapOperation.Create(f[0].String(),f[1].String(),f[2].String(),f[3].IsNull?(string?)null:f[3].String(),f[4].String());
        }
        internal string Write(LocalIapOperation v)=>Object(("operationId",Write(v.OperationId)),("logicalProductKey",Write(v.LogicalProductKey)),("store",Write(v.Store)),("transactionReference",Write(v.TransactionReference)),("state",Write(v.State)),("recordVersion","1"),("kind",Q("IAP")));
        internal LocalRewardOperation ReadLocalRewardOperation(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("operationId","claimId","puzzleIdentity","providerOperationId","state","recordVersion","kind");
            f[6].RequireString("REWARD");
            return LocalRewardOperation.Create(f[0].String(),f[1].String(),ReadPublicPuzzleIdentity(f[2]),f[3].IsNull?(string?)null:f[3].String(),f[4].String());
        }
        internal string Write(LocalRewardOperation v)=>Object(("operationId",Write(v.OperationId)),("claimId",Write(v.ClaimId)),("puzzleIdentity",Write(v.PuzzleIdentity)),("providerOperationId",Write(v.ProviderOperationId)),("state",Write(v.State)),("recordVersion","1"),("kind",Q("REWARD")));
        internal LocalEntitlement ReadLocalEntitlement(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("productKey","store","transactionReference","owned","reconciliationRequired","revocationConfirmed","lastConfirmedAtUtc","grantOperationId","finalizationOperationId","recordVersion");
            return LocalEntitlement.Create(f[0].String(),f[1].String(),f[2].String(),f[3].Boolean(),f[4].Boolean(),f[5].Boolean(),f[6].IsNull?(string?)null:f[6].String(),f[7].IsNull?(string?)null:f[7].String(),f[8].IsNull?(string?)null:f[8].String());
        }
        internal string Write(LocalEntitlement v)=>Object(("productKey",Write(v.ProductKey)),("store",Write(v.Store)),("transactionReference",Write(v.TransactionReference)),("owned",Write(v.Owned)),("reconciliationRequired",Write(v.ReconciliationRequired)),("revocationConfirmed",Write(v.RevocationConfirmed)),("lastConfirmedAtUtc",Write(v.LastConfirmedAtUtc)),("grantOperationId",Write(v.GrantOperationId)),("finalizationOperationId",Write(v.FinalizationOperationId)),("recordVersion","1"));
        internal LocalEntitlements ReadLocalEntitlements(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("records","recordVersion");
            return LocalEntitlements.Create(f[0].Array().Select(x=>ReadLocalEntitlement(x)).ToArray());
        }
        internal string Write(LocalEntitlements v)=>Object(("records","["+string.Join(",",v.Records.Select(x=>Write(x)))+"]"),("recordVersion","1"));
        internal LocalAudioPreference ReadLocalAudioPreference(CanonicalDtoCursor v)
        {
            var f=v.Fields("groupId","numerator","denominator");
            return LocalAudioPreference.Create(f[0].String(),f[1].Integer(),f[2].Integer());
        }
        internal string Write(LocalAudioPreference v)=>Object(("groupId",Write(v.GroupId)),("numerator",Write(v.Numerator)),("denominator",Write(v.Denominator)));
        internal LocalAccessibilityPreference ReadLocalAccessibilityPreference(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("contractRevision","reducedMotion","trainRideSkip","recordVersion");
            return LocalAccessibilityPreference.Create(f[0].String(),f[1].IsNull?(bool?)null:f[1].Boolean(),f[2].IsNull?(bool?)null:f[2].Boolean());
        }
        internal string Write(LocalAccessibilityPreference v)=>Object(("contractRevision",Write(v.ContractRevision)),("reducedMotion",Write(v.ReducedMotion)),("trainRideSkip",Write(v.TrainRideSkip)),("recordVersion","1"));
        internal LocalSettings ReadLocalSettings(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("locale","audioGroups","hapticsEnabled","accessibility","recordVersion");
            return LocalSettings.Create(f[0].IsNull?(string?)null:f[0].String(),f[1].Array().Select(x=>ReadLocalAudioPreference(x)).ToArray(),f[2].IsNull?(bool?)null:f[2].Boolean(),f[3].IsNull?(LocalAccessibilityPreference?)null:ReadLocalAccessibilityPreference(f[3]));
        }
        internal string Write(LocalSettings v)=>Object(("locale",Write(v.Locale)),("audioGroups","["+string.Join(",",v.AudioGroups.Select(x=>Write(x)))+"]"),("hapticsEnabled",Write(v.HapticsEnabled)),("accessibility",v.Accessibility==null?"null":Write(v.Accessibility)),("recordVersion","1"));
        internal LocalPrivacyDecision ReadLocalPrivacyDecision(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("policyRevision","sdkContractRevision","decisionRevision","capturedAtUtc","ads","analytics","crash","validity","source","nativeSyncState","recordVersion");
            return LocalPrivacyDecision.Create(f[0].String(),f[1].String(),f[2].Integer(),f[3].String(),f[4].Boolean(),f[5].Boolean(),f[6].Boolean(),f[7].String(),f[8].String(),f[9].String());
        }
        internal string Write(LocalPrivacyDecision v)=>Object(("policyRevision",Write(v.PolicyRevision)),("sdkContractRevision",Write(v.SdkContractRevision)),("decisionRevision",Write(v.DecisionRevision)),("capturedAtUtc",Write(v.CapturedAtUtc)),("ads",Write(v.Ads)),("analytics",Write(v.Analytics)),("crash",Write(v.Crash)),("validity",Write(v.Validity)),("source",Write(v.Source)),("nativeSyncState",Write(v.NativeSyncState)),("recordVersion","1"));
        internal LocalDataLifecycle ReadLocalDataLifecycle(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("privacyDecision","lastReconciledAtUtc","recordVersion");
            return LocalDataLifecycle.Create(f[0].IsNull?(LocalPrivacyDecision?)null:ReadLocalPrivacyDecision(f[0]),f[1].IsNull?(string?)null:f[1].String());
        }
        internal string Write(LocalDataLifecycle v)=>Object(("privacyDecision",v.PrivacyDecision==null?"null":Write(v.PrivacyDecision)),("lastReconciledAtUtc",Write(v.LastReconciledAtUtc)),("recordVersion","1"));
        internal LocalDailyStatus ReadLocalDailyStatus(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("policyVersion","confirmedClaimIds","recordVersion");
            return LocalDailyStatus.Create(f[0].String(),f[1].Array().Select(x=>x.String()).ToArray());
        }
        internal string Write(LocalDailyStatus v)=>Object(("policyVersion",Write(v.PolicyVersion)),("confirmedClaimIds","["+string.Join(",",v.ConfirmedClaimIds.Select(x=>Write(x)))+"]"),("recordVersion","1"));
        internal LocalHintEntitlement ReadLocalHintEntitlement(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("policyVersion","puzzleIdentity","mode","availableCredits","claimIds","recordVersion");
            return LocalHintEntitlement.Create(f[0].String(),ReadPublicPuzzleIdentity(f[1]),f[2].EnumValue<SessionMode>(),f[3].Integer(),f[4].Array().Select(x=>x.String()).ToArray());
        }
        internal string Write(LocalHintEntitlement v)=>Object(("policyVersion",Write(v.PolicyVersion)),("puzzleIdentity",Write(v.PuzzleIdentity)),("mode",Write(v.Mode)),("availableCredits",Write(v.AvailableCredits)),("claimIds","["+string.Join(",",v.ClaimIds.Select(x=>Write(x)))+"]"),("recordVersion","1"));
        internal ContractGeneratorParameters ReadContractGeneratorParameters(CanonicalDtoCursor v)
        {
            var f=v.Fields("gridWidth","gridHeight");
            return ContractGeneratorParameters.Create(f[0].Int32(),f[1].Int32());
        }
        internal string Write(ContractGeneratorParameters v)=>Object(("gridWidth",Write(v.GridWidth)),("gridHeight",Write(v.GridHeight)));
        internal LocalEndlessDescriptor ReadLocalEndlessDescriptor(CanonicalDtoCursor v)
        {
            v.Version("endlessContractVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            if(!v.Has("parameters"))throw new SaveCodecFailure("SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);
            retained.RequireGenerator(v.Find("generatorVersion").String());
            var f=v.Fields("rulesetVersion","generatorVersion","seed","generationOrdinal","parameters","parameterHashSha256","endlessContractVersion");
            return LocalEndlessDescriptor.Create(f[0].String(),f[1].String(),f[2].String(),f[3].String(),ReadParameters(f[4]),f[5].String());
        }
        internal string Write(LocalEndlessDescriptor v)=>Object(("rulesetVersion",Write(v.RulesetVersion)),("generatorVersion",Write(v.GeneratorVersion)),("seed",Write(v.Seed)),("generationOrdinal",Write(v.GenerationOrdinal)),("parameters",Write(v.Parameters)),("parameterHashSha256",Write(v.ParameterHashSha256)),("endlessContractVersion","1"));
        internal LocalHashReference ReadLocalHashReference(CanonicalDtoCursor v)
        {
            var f=v.Fields("profile","sha256");
            Require(new[]{"STP-PUZZLE-SEMANTIC-JCS-1","STP-SOLUTION-JCS-1","STP-PROOF-JCS-1"}.Contains(f[0].String()),"SAVE_HASH_PROFILE_UNSUPPORTED",true);
            return LocalHashReference.Create(f[0].String(),f[1].String());
        }
        internal string Write(LocalHashReference v)=>Object(("profile",Write(v.Profile)),("sha256",Write(v.Sha256)));
        internal ReservedEndless ReadReservedEndless(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("generationOrdinal","id","descriptor","recordVersion","state");
            f[4].RequireString("RESERVED_NOT_GENERATED");
            return ReservedEndless.Create(f[0].String(),f[1].String(),ReadLocalEndlessDescriptor(f[2]));
        }
        internal string Write(ReservedEndless v)=>Object(("generationOrdinal",Write(v.GenerationOrdinal)),("id",Write(v.Id)),("descriptor",Write(v.Descriptor)),("recordVersion","1"),("state",Q("RESERVED_NOT_GENERATED")));
        internal ActiveEndlessDraft ReadActiveEndlessDraft(CanonicalDtoCursor v)
        {
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("generationOrdinal","id","descriptor","publicPuzzleInput","publicPuzzleHash","solutionHash","proofHash","checkpoint","recordVersion","state");
            f[9].RequireString("ACTIVE_DRAFT");
            return ActiveEndlessDraft.Create(f[0].String(),f[1].String(),ReadLocalEndlessDescriptor(f[2]),ReadPublicPuzzleInput(f[3]),ReadLocalHashReference(f[4]),ReadLocalHashReference(f[5]),ReadLocalHashReference(f[6]),ReadCheckpoint(f[7]));
        }
        internal string Write(ActiveEndlessDraft v)=>Object(("generationOrdinal",Write(v.GenerationOrdinal)),("id",Write(v.Id)),("descriptor",Write(v.Descriptor)),("publicPuzzleInput",Write(v.PublicPuzzleInput)),("publicPuzzleHash",Write(v.PublicPuzzleHash)),("solutionHash",Write(v.SolutionHash)),("proofHash",Write(v.ProofHash)),("checkpoint",Write(v.Checkpoint)),("recordVersion","1"),("state",Q("ACTIVE_DRAFT")));
        internal LocalEndlessClaim ReadLocalEndlessClaim(CanonicalDtoCursor v)
        {
            var f=v.Fields("claimId","localOperationId","claimStatus","providerOperationId","reasonCode");
            f[4].RequireString("POST_CLEAR_PATIENCE");
            return LocalEndlessClaim.Create(f[0].String(),f[1].String(),f[2].String(),f[3].IsNull?(string?)null:f[3].String());
        }
        internal string Write(LocalEndlessClaim v)=>Object(("claimId",Write(v.ClaimId)),("localOperationId",Write(v.LocalOperationId)),("claimStatus",Write(v.ClaimStatus)),("providerOperationId",Write(v.ProviderOperationId)),("reasonCode",Q("POST_CLEAR_PATIENCE")));
        internal OpenEndlessCompletion ReadOpenEndlessCompletion(CanonicalDtoCursor v)
        {
            int version=v.Find("recordVersion").Int32();Require(version==1||version==2&&WriteSchemaVersion==3,"SAVE_RECORD_VERSION_UNSUPPORTED",true);
            string[] fields={"generationOrdinal","id","descriptor","puzzleId","completionCommitId","claim","recordVersion","state"};var f=v.Fields(version==2?fields.Concat(new[]{"result"}).ToArray():fields);
            f[7].RequireString("COMPLETION_CLAIM_OPEN");
            var descriptor=ReadLocalEndlessDescriptor(f[2]);var claim=ReadLocalEndlessClaim(f[5]);if(version==1)return OpenEndlessCompletion.Create(f[0].String(),f[1].String(),descriptor,f[3].String(),f[4].String(),claim);
            var r=f[8].Fields("identity","attemptId","activeElapsedMs","stars","firstClearTerminal","secondStarPaid","thirdStarPaid");var result=new LocalEndlessResult(ReadPublicPuzzleIdentity(r[0]),r[1].Guid(),r[2].Integer(),r[3].Int32(),ReadEconomyTerminal(r[4]),r[5].IsNull?null:ReadEconomyTerminal(r[5]),r[6].IsNull?null:ReadEconomyTerminal(r[6]));return OpenEndlessCompletion.CreateCalibrated(f[0].String(),f[1].String(),descriptor,f[3].String(),f[4].String(),claim,result);
        }
        internal string Write(OpenEndlessCompletion v){var fields=new System.Collections.Generic.List<(string,string)>{("generationOrdinal",Write(v.GenerationOrdinal)),("id",Write(v.Id)),("descriptor",Write(v.Descriptor)),("puzzleId",Write(v.PuzzleId)),("completionCommitId",Write(v.CompletionCommitId)),("claim",Write(v.Claim)),("recordVersion",Write(v.RecordVersion)),("state",Q("COMPLETION_CLAIM_OPEN"))};if(v.Result!=null){Require(WriteSchemaVersion==3,"SAVE_SCHEMA_UNSUPPORTED",true);var r=v.Result;fields.Add(("result",Object(("identity",Write(r.Identity)),("attemptId",Write(r.AttemptId)),("activeElapsedMs",Write(r.ActiveElapsedMs)),("stars",Write(r.Stars)),("firstClearTerminal",Write(r.FirstClearTerminal)),("secondStarPaid",r.SecondStarPaid==null?"null":Write(r.SecondStarPaid)),("thirdStarPaid",r.ThirdStarPaid==null?"null":Write(r.ThirdStarPaid)))));}return Object(fields.ToArray());}
        internal LocalEndlessState ReadLocalEndlessState(CanonicalDtoCursor v)
        {
            if(v.Find("recordVersion").Integer()==2){Require(WriteSchemaVersion==3,"SAVE_SCHEMA_UNSUPPORTED",true);return ReadRuntimeEndless(v);}
            v.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");
            var f=v.Fields("highestReservedOrdinal","openEndless","recordVersion");
            return LocalEndlessState.Create(f[0].String(),f[1].Array().Select(x=>ReadLocalOpenEndless(x)).ToArray());
        }
        internal string Write(LocalEndlessState v)=>v.RecordVersion==2?WriteRuntimeEndless(v):Object(("highestReservedOrdinal",Write(v.HighestReservedOrdinal)),("openEndless","["+string.Join(",",v.OpenEndless.Select(x=>Write(x)))+"]"),("recordVersion","1"));
    }
}
