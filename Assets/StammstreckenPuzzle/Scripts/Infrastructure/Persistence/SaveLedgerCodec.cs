using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Contract1 ledger codec: raw32 previous hash followed by the canonical exact ten-field entry projection.</summary>
    public sealed class SaveLedgerCodec : ILocalLedger
    {
        private const long MaxSafe=9007199254740991L;private readonly ICanonicalJson json;
        // Pure projection bytes only: no state, chain verdict, balance or save acknowledgement is cached.
        // Both caps are fixed, and every hit compares every immutable stored-entry field.
        private const int EntryCacheCountLimit=512+64,EntryCacheByteLimit=262144;
        private readonly object entryProjectionGate=new object();
        private readonly Dictionary<LocalStoredEntry,byte[]> entryProjections=new Dictionary<LocalStoredEntry,byte[]>(new StoredEntryEquality());
        private readonly Queue<LocalStoredEntry> entryProjectionOrder=new Queue<LocalStoredEntry>();
        private int entryProjectionBytes;
        private sealed class JournalProjection
        {
            internal JournalProjection(LocalStoredEntry[] entries,byte[] bytes){Entries=entries;Bytes=bytes;}
            internal readonly LocalStoredEntry[] Entries;internal readonly byte[] Bytes;
        }
        private readonly Queue<JournalProjection> journalProjections=new Queue<JournalProjection>();
        private bool SameJournal(LocalStoredEntry[] a,LocalStoredEntry[] b)
        {
            if(a.Length!=b.Length)return false;
            for(int i=0;i<a.Length;i++)if(!entryProjections.Comparer.Equals(a[i],b[i]))return false;
            return true;
        }
        private byte[] CanonicalJournalBytes(IReadOnlyList<LocalStoredEntry> entries)
        {
            // Snapshot the same ordered immutable entries once. A caller-owned mutable list
            // cannot mutate a stored key, and every later call compares all entry fields.
            if(entries==null)return LocalSaveHash.Canonical(json,"["+string.Join(",",entries!.Select(Stored))+"]");
            var captured=new List<LocalStoredEntry>();
            foreach(var entry in entries){if(entry==null)Stored(entry!);captured.Add(entry!);}
            var key=captured.ToArray();
            lock(entryProjectionGate)foreach(var hit in journalProjections)if(SameJournal(hit.Entries,key))return hit.Bytes;
            var bytes=LocalSaveHash.Canonical(json,"["+string.Join(",",key.Select(Stored))+"]");
            if(key.Length>512||bytes.Length>EntryCacheByteLimit)return bytes;
            lock(entryProjectionGate)
            {
                foreach(var hit in journalProjections)if(SameJournal(hit.Entries,key))return hit.Bytes;
                while(journalProjections.Count>=2)journalProjections.Dequeue();
                journalProjections.Enqueue(new JournalProjection(key,bytes));return bytes;
            }
        }

        private sealed class StoredEntryEquality:IEqualityComparer<LocalStoredEntry>
        {
            public bool Equals(LocalStoredEntry? a,LocalStoredEntry? b)
            {
                if(ReferenceEquals(a,b))return true;
                return a!=null&&b!=null&&a.Sequence==b.Sequence&&a.TransactionId==b.TransactionId&&a.ReasonCode==b.ReasonCode&&a.Amount==b.Amount&&a.SubjectId==b.SubjectId&&a.CatalogId==b.CatalogId&&a.CatalogRevision==b.CatalogRevision&&a.CatalogHashSha256==b.CatalogHashSha256&&a.ClaimId==b.ClaimId&&a.CreatedAtUtc==b.CreatedAtUtc&&a.EntryChainHashSha256==b.EntryChainHashSha256;
            }
            private static int H(string? value)=>value==null?0:StringComparer.Ordinal.GetHashCode(value);
            public int GetHashCode(LocalStoredEntry e)
            {
                unchecked
                {
                    int h=e.Sequence.GetHashCode();h=h*31+H(e.TransactionId);h=h*31+H(e.ReasonCode);h=h*31+e.Amount.GetHashCode();h=h*31+H(e.SubjectId);h=h*31+H(e.CatalogId);h=h*31+e.CatalogRevision.GetHashCode();h=h*31+H(e.CatalogHashSha256);h=h*31+H(e.ClaimId);h=h*31+H(e.CreatedAtUtc);return h*31+H(e.EntryChainHashSha256);
                }
            }
        }
        private byte[] CanonicalEntryBytes(LocalStoredEntry e)
        {
            if(e==null)return LocalSaveHash.Canonical(json,Projection(e!));
            lock(entryProjectionGate)if(entryProjections.TryGetValue(e,out var hit))return hit;
            // The canonical authority runs outside the lock; failures never enter the cache.
            var bytes=LocalSaveHash.Canonical(json,Projection(e));
            if(bytes.Length>EntryCacheByteLimit)return bytes;
            lock(entryProjectionGate)
            {
                if(entryProjections.TryGetValue(e,out var hit))return hit;
                while(entryProjections.Count>=EntryCacheCountLimit||entryProjectionBytes+bytes.Length>EntryCacheByteLimit)
                {
                    var oldest=entryProjectionOrder.Dequeue();entryProjectionBytes-=entryProjections[oldest].Length;entryProjections.Remove(oldest);
                }
                entryProjections.Add(e,bytes);entryProjectionOrder.Enqueue(e);entryProjectionBytes+=bytes.Length;
                return bytes;
            }
        }

        /// <summary>Binds the sole runtime canonical JSON authority.</summary>
        public SaveLedgerCodec(ICanonicalJson json){this.json=json??throw new ArgumentNullException(nameof(json));}
        /// <summary>Canonical exact Ei bytes, excluding its stored chain hash.</summary>
        public byte[] EntryBytes(LocalStoredEntry e)=>(byte[])CanonicalEntryBytes(e).Clone();
        /// <summary>Canonical stored entry bytes, including its verified chain hash.</summary>
        public byte[] StoredEntryBytes(LocalStoredEntry e)=>LocalSaveHash.Canonical(json,Stored(e));
        /// <summary>Canonical journal array bytes used for both exact byte limits.</summary>
        public byte[] JournalBytes(IReadOnlyList<LocalStoredEntry> entries)=>(byte[])CanonicalJournalBytes(entries).Clone();
        private static string Q(string? s)=>s==null?"null":LocalSaveHash.Quote(s);
        private static string N(long n)=>n.ToString(CultureInfo.InvariantCulture);
        private static string Projection(LocalStoredEntry e)=>"{\"sequence\":"+N(e.Sequence)+",\"transactionId\":"+Q(e.TransactionId)+",\"reasonCode\":"+Q(e.ReasonCode)+",\"amount\":"+N(e.Amount)+",\"subjectId\":"+Q(e.SubjectId)+",\"catalogId\":"+Q(e.CatalogId)+",\"catalogRevision\":"+(e.CatalogRevision.HasValue?N(e.CatalogRevision.Value):"null")+",\"catalogHashSha256\":"+Q(e.CatalogHashSha256)+",\"claimId\":"+Q(e.ClaimId)+",\"createdAtUtc\":"+Q(e.CreatedAtUtc)+"}";
        private static string Stored(LocalStoredEntry e)=>Projection(e).TrimEnd('}')+",\"entryChainHashSha256\":"+Q(e.EntryChainHashSha256)+"}";
        private string Chain(string previous,LocalStoredEntry e)=>ChainBytes(previous,CanonicalEntryBytes(e));
        private string NewChain(string previous,LocalStoredEntry e)=>ChainBytes(previous,LocalSaveHash.Canonical(json,Projection(e)));
        private static string ChainBytes(string previous,byte[] bytes){var all=new byte[32+bytes.Length];for(int i=0;i<32;i++)all[i]=byte.Parse(previous.Substring(i*2,2),NumberStyles.HexNumber,CultureInfo.InvariantCulture);Buffer.BlockCopy(bytes,0,all,32,bytes.Length);return LocalSaveHash.Bytes(all);}
        private static void Require(bool value,string code){if(!value)throw new ArgumentException(code);}
        private static bool Matches(LocalStoredEntry e,EconomyTerminal t)=>e.TransactionId==t.TransactionId&&e.ReasonCode==t.ReasonCode&&e.SubjectId==t.SubjectId&&e.Amount==t.Amount&&e.CatalogId==t.Catalog.CatalogId&&e.CatalogRevision==t.Catalog.Revision&&e.CatalogHashSha256==t.Catalog.HashSha256&&e.ClaimId==t.ClaimId&&e.CreatedAtUtc==t.CreatedAtUtc;
        private static EconomyTerminal[] Terminals(LocalProfileState state)=>state.Levels.SelectMany(l=>new[]{l.FirstClear?.Terminal,l.SecondStarPaid,l.ThirdStarPaid}).Concat(state.RewardClaims.Select(c=>c is CampaignProgressTerminal p?p.Terminal:c is PostClearRewardClaim r?r.Terminal:null)).Concat(state.Metagame?.Daily.Select(c=>c.Terminal)??Enumerable.Empty<EconomyTerminal?>()).Concat(state.Endless.EconomyReceipts).Where(t=>t!=null).Cast<EconomyTerminal>().ToArray();
        private static bool MatchesPurchase(LocalStoredEntry e,PurchasedOwnership p)=>e.TransactionId==p.TransactionId&&e.SubjectId==p.ItemId&&e.Amount==-p.PricePatience&&e.CatalogId==p.Catalog.CatalogId&&e.CatalogRevision==p.Catalog.Revision&&e.CatalogHashSha256==p.Catalog.HashSha256&&e.ClaimId==null&&e.ReasonCode=="COSMETIC_PURCHASE"&&e.CreatedAtUtc==p.AcquiredAtUtc;
        private static bool TerminalBacked(LocalStoredEntry e,LocalProfileState state)=>Terminals(state).Any(t=>Matches(e,t))||state.Inventory.Ownership.OfType<PurchasedOwnership>().Any(p=>MatchesPurchase(e,p));
        /// <summary>Verifies contiguous sequence, exact chain, nonnegative safe balance, checkpoint/tail consistency and all terminal references.</summary>
        public long Validate(LocalProfileState state)=>ValidateCore(state,out _);
        private long ValidateCore(LocalProfileState state,out int journalByteLength)
        {
            journalByteLength=0;
            if(state==null)throw new ArgumentNullException(nameof(state));var checkpoint=state.LedgerCheckpoint;var journal=state.EconomyJournal;Require(journal.Count<=512&&(journalByteLength=CanonicalJournalBytes(journal).Length)<=262144,"SAVE_LEDGER_LIMIT");
            if(checkpoint.ThroughSequence==0)Require(checkpoint.BalancePatience==0&&checkpoint.EntryChainHashSha256==LocalSaveHash.Bytes(Array.Empty<byte>())&&checkpoint.JournalDiagnosticTail.Count==0,"SAVE_LEDGER_GENESIS_INVALID");
            // Only this invocation owns these lookups. Building them does not validate, suppress or reorder a guard.
            EconomyTerminal[]? currentTerminals=null;
            ILookup<string,EconomyTerminal>? terminalLookup=null;
            EconomyTerminal[] CurrentTerminals()=>currentTerminals??(currentTerminals=Terminals(state));
            ILookup<string,EconomyTerminal> TerminalLookup()=>terminalLookup??(terminalLookup=CurrentTerminals().ToLookup(t=>t.TransactionId,StringComparer.Ordinal));
            bool TailTerminalBacked(LocalStoredEntry entry)=>TerminalLookup()[entry.TransactionId].Any(t=>Matches(entry,t))||state.Inventory.Ownership.OfType<PurchasedOwnership>().Any(p=>MatchesPurchase(entry,p));
            var tail=checkpoint.JournalDiagnosticTail;for(int i=0;i<tail.Count;i++){Require(TailTerminalBacked(tail[i]),"SAVE_LEDGER_TAIL_TERMINAL_INVALID");if(i>0)Require(tail[i].Sequence==tail[i-1].Sequence+1&&Chain(tail[i-1].EntryChainHashSha256,tail[i])==tail[i].EntryChainHashSha256,"SAVE_LEDGER_CHAIN_INVALID");else if(tail[i].Sequence==1)Require(Chain(LocalSaveHash.Bytes(Array.Empty<byte>()),tail[i])==tail[i].EntryChainHashSha256,"SAVE_LEDGER_CHAIN_INVALID");}if(tail.Count>0)Require(tail[tail.Count-1].Sequence==checkpoint.ThroughSequence&&tail[tail.Count-1].EntryChainHashSha256==checkpoint.EntryChainHashSha256,"SAVE_LEDGER_TAIL_INVALID");
            long sequence=checkpoint.ThroughSequence;long balance=checkpoint.BalancePatience;string hash=checkpoint.EntryChainHashSha256;var ids=new HashSet<string>(StringComparer.Ordinal);
            foreach(var e in journal){Require(sequence<MaxSafe&&e.Sequence==sequence+1,"SAVE_LEDGER_SEQUENCE_INVALID");Require(ids.Add(e.TransactionId),"ECO-TRANSACTION-ID-COLLISION");Require(Chain(hash,e)==e.EntryChainHashSha256,"SAVE_LEDGER_CHAIN_INVALID");sequence=e.Sequence;hash=e.EntryChainHashSha256;try{balance=checked(balance+e.Amount);}catch(OverflowException){throw new ArgumentException("SAVE_LEDGER_BALANCE_INVALID");}Require(balance>=0&&balance<=MaxSafe,"SAVE_LEDGER_BALANCE_INVALID");}
            var terminals=CurrentTerminals();Require(terminals.Select(t=>t.TransactionId).Distinct(StringComparer.Ordinal).Count()==terminals.Length,"ECO-TRANSACTION-ID-COLLISION");
            // Journal duplicate IDs and chain guards above have already established unique entries.
            var journalLookup=journal.ToDictionary(e=>e.TransactionId,StringComparer.Ordinal);
            foreach(var terminal in terminals){if(journalLookup.TryGetValue(terminal.TransactionId,out var entry))Require(Matches(entry,terminal),"ECO-TRANSACTION-ID-COLLISION");else Require(checkpoint.ThroughSequence>0&&terminal.CommittedGeneration<=checkpoint.SourceSaveGeneration,"SAVE_TERMINAL_LEDGER_MISSING");}
            foreach(var purchase in state.Inventory.Ownership.OfType<PurchasedOwnership>()){Require(purchase.TransactionId=="cosmetic-purchase:"+purchase.ItemId&&purchase.PricePatience>0,"SAVE_OWNERSHIP_LEDGER_MISMATCH");Require(journalLookup.TryGetValue(purchase.TransactionId,out var entry)?MatchesPurchase(entry,purchase):checkpoint.ThroughSequence>0,"SAVE_OWNERSHIP_LEDGER_MISMATCH");}
            foreach(var entry in journal){var known=TerminalLookup()[entry.TransactionId].FirstOrDefault();if(known!=null)Require(Matches(entry,known),"ECO-TRANSACTION-ID-COLLISION");}
            return balance;
        }
        /// <summary>Creates a chained entry without mutating an existing checkpoint, journal or terminal.</summary>
        public LocalStoredEntry Next(LocalLedgerCheckpoint checkpoint,IReadOnlyList<LocalStoredEntry> journal,EconomyTerminal terminal)
        {
            if(checkpoint==null||journal==null||terminal==null)throw new ArgumentNullException();Require(!journal.Any(e=>e.TransactionId==terminal.TransactionId),"ECO-TRANSACTION-ID-COLLISION");long previous=journal.Count==0?checkpoint.ThroughSequence:journal[journal.Count-1].Sequence;Require(previous<MaxSafe,"SAVE_LEDGER_SEQUENCE_EXHAUSTED");var entry=LocalStoredEntry.Create(previous+1,terminal.TransactionId,terminal.ReasonCode,terminal.Amount,terminal.SubjectId,terminal.Catalog.CatalogId,terminal.Catalog.Revision,terminal.Catalog.HashSha256,terminal.ClaimId,terminal.CreatedAtUtc,new string('0',64));string priorHash=journal.Count==0?checkpoint.EntryChainHashSha256:journal[journal.Count-1].EntryChainHashSha256;return LocalStoredEntry.Create(entry.Sequence,entry.TransactionId,entry.ReasonCode,entry.Amount,entry.SubjectId,entry.CatalogId,entry.CatalogRevision,entry.CatalogHashSha256,entry.ClaimId,entry.CreatedAtUtc,NewChain(priorHash,entry));
        }
        /// <summary>Creates the schema-preserving purchase debit; the caller must commit the ownership in the same snapshot.</summary>
        public LocalStoredEntry NextPurchase(LocalLedgerCheckpoint checkpoint,IReadOnlyList<LocalStoredEntry> journal,PurchasedOwnership ownership)
        {
            if(checkpoint==null||journal==null||ownership==null)throw new ArgumentNullException();
            Require(!journal.Any(e=>e.TransactionId==ownership.TransactionId),"ECO-TRANSACTION-ID-COLLISION");
            long previous=journal.Count==0?checkpoint.ThroughSequence:journal[journal.Count-1].Sequence;
            Require(previous<MaxSafe,"SAVE_LEDGER_SEQUENCE_EXHAUSTED");
            var entry=LocalStoredEntry.Create(previous+1,ownership.TransactionId,"COSMETIC_PURCHASE",-ownership.PricePatience,ownership.ItemId,ownership.Catalog.CatalogId,ownership.Catalog.Revision,ownership.Catalog.HashSha256,null,ownership.AcquiredAtUtc,new string('0',64));
            string priorHash=journal.Count==0?checkpoint.EntryChainHashSha256:journal[journal.Count-1].EntryChainHashSha256;
            return LocalStoredEntry.Create(entry.Sequence,entry.TransactionId,entry.ReasonCode,entry.Amount,entry.SubjectId,entry.CatalogId,entry.CatalogRevision,entry.CatalogHashSha256,null,entry.CreatedAtUtc,NewChain(priorHash,entry));
        }
        /// <summary>At either soft threshold, folds the entire verified terminal suffix into one checkpoint and retains only the last64 removed entries as diagnosis.</summary>
        public LocalProfileState Compact(LocalProfileState state,long sourceSaveGeneration)
        {
            long balance=ValidateCore(state,out int journalByteLength);Require(sourceSaveGeneration>=0&&sourceSaveGeneration<=MaxSafe,"SAVE_GENERATION_INVALID");if(state.EconomyJournal.Count<384&&journalByteLength<196608)return state;Require(state.EconomyJournal.All(e=>TerminalBacked(e,state)),"ECO-COMPACTION-NONTERMINAL");var last=state.EconomyJournal.Last();var tail=state.LedgerCheckpoint.JournalDiagnosticTail.Concat(state.EconomyJournal).Reverse().Take(64).Reverse().ToArray();var checkpoint=LocalLedgerCheckpoint.Create(last.Sequence,balance,last.EntryChainHashSha256,sourceSaveGeneration,tail);var compacted=state.WithProgress(state.Levels,checkpoint,Array.Empty<LocalStoredEntry>(),state.SeasonProgressCache,state.Drafts,state.RewardClaims);return LocalEndlessEconomy.Prune(compacted);
        }
    }
}
