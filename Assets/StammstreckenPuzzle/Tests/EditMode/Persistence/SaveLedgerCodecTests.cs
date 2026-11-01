using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Persistence;

namespace STP.Tests.Persistence.EditMode
{
    public sealed class SaveLedgerCodecTests
    {
        private static object? P(object o,string key)=>LocalSaveContractFixtures.Property(o,key);
        private static object? Call(object o,string name,params object?[] args)=>LocalSaveContractFixtures.Invoke(o,name,args);
        private static object[] Boundary(int count,bool bytes=false)=>(object[])Assembly.Load("STP.Tests.Application.EditMode").GetType("STP.Tests.Application.EditMode.LocalSaveContractFixtures",true)!.GetMethod("CreateLedgerBoundary")!.Invoke(null,new object[]{count,bytes})!;
        private static object[] ByteBoundary(int count,int bytes)=>(object[])Assembly.Load("STP.Tests.Application.EditMode").GetType("STP.Tests.Application.EditMode.LocalSaveContractFixtures",true)!.GetMethod("CreateLedgerByteBoundary")!.Invoke(null,new object[]{count,bytes})!;
        private static object Create(string type,params object?[] args)=>Assembly.Load("STP.Application").GetType("STP.Application."+type,true)!.GetMethod("Create")!.Invoke(null,args)!;
        private static object[] Items(object list)=>((IEnumerable)list).Cast<object>().ToArray();
        private static Array Entries(IEnumerable<object> list){var values=list.ToArray();var result=Array.CreateInstance(Assembly.Load("STP.Application").GetType("STP.Application.LocalStoredEntry",true)!,values.Length);for(int i=0;i<values.Length;i++)result.SetValue(values[i],i);return result;}
        private static object With(object state,object checkpoint,object journal)=>Call(state,"WithProgress",P(state,"Levels"),checkpoint,journal,P(state,"SeasonProgressCache"),P(state,"Drafts"),P(state,"RewardClaims"))!;
        [Test]
        public void ExactIndependentNodeProjectionTailSuffixAndNullFieldsArePreservedWithoutDoubleCounting()
        {
            var c=LocalSaveContractFixtures.Case("LEDGER_CONTRACT1_AND_CATALOGSET");var decoded=LocalSaveContractFixtures.CodecFor(c).Decode(Encoding.UTF8.GetBytes(c.Get("sourceEnvelopeUtf8").Text));Assert.That(decoded.Status,Is.EqualTo(LocalSaveDecodeStatus.Valid),decoded.ErrorCode);var state=P(P(decoded,"Snapshot")!,"Payload")!;var f=Boundary(1);var ledger=f[3];var vectors=LocalSaveContractFixtures.Bundle.Get("ledgerCompleteVectors");var checkpoint=P(state,"LedgerCheckpoint")!;var tail=Items(P(checkpoint,"JournalDiagnosticTail")!);var suffix=Items(P(state,"EconomyJournal")!);Assert.That((byte[])Call(ledger,"EntryBytes",tail[0])!,Is.EqualTo(Encoding.UTF8.GetBytes(vectors.Get("removedEntryProjectionUtf8").Text)));Assert.That((byte[])Call(ledger,"StoredEntryBytes",tail[0])!,Is.EqualTo(Encoding.UTF8.GetBytes(vectors.Get("removedStoredEntryUtf8").Text)));Assert.That((byte[])Call(ledger,"JournalBytes",P(checkpoint,"JournalDiagnosticTail"))!,Is.EqualTo(Encoding.UTF8.GetBytes(vectors.Get("diagnosticTailUtf8").Text)));Assert.That((byte[])Call(ledger,"EntryBytes",suffix[0])!,Is.EqualTo(Encoding.UTF8.GetBytes(vectors.Get("suffixEntryProjectionUtf8").Text)));long balance=(long)Call(ledger,"Validate",state)!;Assert.That(balance,Is.EqualTo(14));Assert.That(Call(ledger,"Compact",state,8L),Is.SameAs(state));
            var nullEntry=Create("LocalStoredEntry",1L,"contract-ledger-null-fields","CONTRACT_NO_CATALOG",0L,"contract-null-subject",null,null,null,null,"2026-10-19T00:00:00Z",vectors.Get("nullEntryChainHashSha256").Text);Assert.That((byte[])Call(ledger,"EntryBytes",nullEntry)!,Is.EqualTo(Encoding.UTF8.GetBytes(vectors.Get("nullEntryProjectionUtf8").Text)));
        }
        [TestCase(383,false)][TestCase(384,true)][TestCase(511,true)][TestCase(512,true)]
        public void CountThresholdCompactsOnlyAt384AndPreservesAllTerminals(int count,bool compact)
        {
            var f=Boundary(count);var source=f[1];var ledger=f[3];var result=Call(ledger,"Compact",source,2L)!;Assert.That((long)Call(ledger,"Validate",result)!,Is.EqualTo(count*15));Assert.That(Items(P(result,"EconomyJournal")!).Length,Is.EqualTo(compact?0:count));Assert.That(P(result,"LedgerCheckpoint")==P(source,"LedgerCheckpoint"),Is.EqualTo(!compact));Assert.That(Items(P(source,"EconomyJournal")!).Length,Is.EqualTo(count));Assert.That(Items(P(result,"Levels")!).Length,Is.EqualTo(count));if(compact){var cp=P(result,"LedgerCheckpoint")!;var tail=Items(P(cp,"JournalDiagnosticTail")!);Assert.That(tail.Length,Is.EqualTo(64));Assert.That(P(tail[0],"Sequence"),Is.EqualTo(count-63));Assert.That(P(tail[63],"Sequence"),Is.EqualTo(count));Assert.That(P(cp,"SourceSaveGeneration"),Is.EqualTo(2));Assert.That(Call(ledger,"Compact",result,3L),Is.SameAs(result));}
        }
        [TestCase(196607,false)][TestCase(196608,true)][TestCase(262144,true)]
        public void CanonicalByteThresholdIsIndependentOfEntryCount(int bytes,bool compact)
        {
            var f=ByteBoundary(383,bytes);var source=f[1];var ledger=f[3];Assert.That(((byte[])Call(ledger,"JournalBytes",P(source,"EconomyJournal"))!).Length,Is.EqualTo(bytes));var result=Call(ledger,"Compact",source,2L)!;Assert.That(Items(P(result,"EconomyJournal")!).Length,Is.EqualTo(compact?0:383));Assert.That((long)Call(ledger,"Validate",result)!,Is.EqualTo(5745));
        }
        [Test]
        public void HardByteLimitRejectsRatherThanTruncatesAnyAuthoritativeEntry()
        {var error=Assert.Throws<TargetInvocationException>(()=>ByteBoundary(383,262145));Assert.That(error!.InnerException!.Message,Is.EqualTo("SAVE_LEDGER_LIMIT"));}
        [Test]
        public void HardCountLimitRejectsRatherThanTruncatesAnyAuthoritativeEntry()
        {var error=Assert.Throws<TargetInvocationException>(()=>Boundary(513));Assert.That(error!.InnerException!.Message,Is.EqualTo("SAVE_COLLECTION_LIMIT"));}
        [Test]
        public void UnknownTerminalSuffixCannotBeCompactedAndIncorrectHashCannotBeAccepted()
        {
            var f=Boundary(384);var source=f[1];var ledger=f[3];var journal=Items(P(source,"EconomyJournal")!);var e=journal[0];var bad=Create("LocalStoredEntry",P(e,"Sequence"),P(e,"TransactionId"),P(e,"ReasonCode"),P(e,"Amount"),P(e,"SubjectId"),P(e,"CatalogId"),P(e,"CatalogRevision"),P(e,"CatalogHashSha256"),P(e,"ClaimId"),P(e,"CreatedAtUtc"),new string('0',64));var state=With(source,P(source,"LedgerCheckpoint")!,Entries(new[]{bad}.Concat(journal.Skip(1))));var error=Assert.Throws<TargetInvocationException>(()=>Call(ledger,"Validate",state));Assert.That(error!.InnerException!.Message,Is.EqualTo("SAVE_LEDGER_CHAIN_INVALID"));
            // Valid hashes cannot replace missing terminal authority: retain the journal but remove the saved level facts.
            var empty=Array.CreateInstance(Assembly.Load("STP.Application").GetType("STP.Application.LocalLevelProgress",true)!,0);var noTerminals=Call(source,"WithProgress",empty,P(source,"LedgerCheckpoint"),P(source,"EconomyJournal"),P(source,"SeasonProgressCache"),P(source,"Drafts"),P(source,"RewardClaims"))!;Assert.That((long)Call(ledger,"Validate",noTerminals)!,Is.EqualTo(5760));error=Assert.Throws<TargetInvocationException>(()=>Call(ledger,"Compact",noTerminals,2L));Assert.That(error!.InnerException!.Message,Is.EqualTo("ECO-COMPACTION-NONTERMINAL"));
        }
        [TestCase("sequence","SAVE_LEDGER_SEQUENCE_INVALID")][TestCase("balance","SAVE_LEDGER_BALANCE_INVALID")][TestCase("collision","ECO-TRANSACTION-ID-COLLISION")]
        public void SemanticSequenceBalanceAndTransactionCollisionsStopDespiteOtherwiseRealBoundSuffix(string kind,string expected)
        {
            var f=Boundary(2);var source=f[1];var ledger=f[3];var checkpoint=P(source,"LedgerCheckpoint")!;var journal=Items(P(source,"EconomyJournal")!);int index=kind=="collision"?1:0;var entry=journal[index];object Clone(string hash)=>Create("LocalStoredEntry",kind=="sequence"?2L:P(entry,"Sequence"),kind=="collision"?P(journal[0],"TransactionId"):P(entry,"TransactionId"),P(entry,"ReasonCode"),kind=="balance"?-1L:P(entry,"Amount"),P(entry,"SubjectId"),P(entry,"CatalogId"),P(entry,"CatalogRevision"),P(entry,"CatalogHashSha256"),P(entry,"ClaimId"),P(entry,"CreatedAtUtc"),hash);var replacement=Clone((string)P(entry,"EntryChainHashSha256")!);if(kind=="balance"){var projection=(byte[])Call(ledger,"EntryBytes",replacement)!;var bytes=new byte[32+projection.Length];string previous=(string)P(checkpoint,"EntryChainHashSha256")!;for(int i=0;i<32;i++)bytes[i]=byte.Parse(previous.Substring(i*2,2),System.Globalization.NumberStyles.HexNumber);Buffer.BlockCopy(projection,0,bytes,32,projection.Length);replacement=Clone(LocalSaveContractFixtures.Sha(bytes));}journal[index]=replacement;var corrupted=With(source,checkpoint,Entries(journal));var error=Assert.Throws<TargetInvocationException>(()=>Call(ledger,"Validate",corrupted));Assert.That(error!.InnerException!.Message,Is.EqualTo(expected));Assert.That((long)Call(ledger,"Validate",source)!,Is.EqualTo(30));
        }
    }
}
