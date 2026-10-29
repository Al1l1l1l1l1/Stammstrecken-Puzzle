using System;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    public sealed partial class LocalSaveCodec
    {
        private SearchGeneratorParameters ReadSearchParameters(CanonicalDtoCursor v)
        {
            int version=v.Find("contractVersion").Int32();Require(WriteSchemaVersion==3&&(version==1&&profile.Use==CatalogUse.InternalPreview||version==2&&(profile.Use==CatalogUse.Production||profile.Use==CatalogUse.ContractTest)),"SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);
            string[] names={"contractVersion","profileId","profileHashSha256","noveltyVersion","campaignCatalog","recentPublicInputs","minSize","maxSize","minDensityPercent","maxDensityPercent","minDeductionSteps","minDeductionDepth","maxAttempts","pathStateBudget","totalBudgetMs","solverBudgetMs","maxTrivialLines","recentSimilarityPercent"};var f=v.Fields(version==2?names.Concat(new[]{"starCalibration"}).ToArray():names);
            CatalogIdentity Reference(CanonicalDtoCursor x){var r=x.Fields("catalogId","revision","hashSha256");return new CatalogIdentity(r[0].String(),r[1].Int32(),r[2].String(),profile.Use,puzzles.Identity.ApprovalStatus);}
            var catalog=Reference(f[4]);EndlessStarCalibration? calibration=null;if(version==2){var a=f[18].Fields("completionCatalog","calibrationCatalog","calibrationVersion","twoStarMs","threeStarMs");calibration=new EndlessStarCalibration(Reference(a[0]),Reference(a[1]),a[2].String(),a[3].Integer(),a[4].Integer());}
            var search=new PuzzleCandidateParameters(f[6].Int32(),f[7].Int32(),f[8].Int32(),f[9].Int32(),f[10].Int32(),f[11].Int32(),f[12].Int32(),f[13].Int32(),f[14].Int32(),f[15].Int32(),f[16].Int32(),f[17].Int32());
            return new SearchGeneratorParameters(f[1].String(),f[2].String(),f[3].String(),catalog,f[5].Array().Select(ReadSearchRecentPublicInput).ToArray(),search,calibration);
        }
        private PublicPuzzleInput ReadSearchRecentPublicInput(CanonicalDtoCursor v)
        {
            var f=v.Fields("puzzleId","rulesetVersion","grid","endpoints","rowCounts","columnCounts","publicPuzzleHash");
            var claimed=ReadLocalHashReference(f[6]);
            Require(claimed.Profile==PublicPuzzleIdentity.SemanticHashProfile,"SAVE_ENDLESS_REFERENCE_HASH_PROFILE_UNSUPPORTED",true);
            string semantic=Object(("puzzleId",f[0].Raw),("rulesetVersion",f[1].Raw),("grid",f[2].Raw),("endpoints",f[3].Raw),("rowCounts",f[4].Raw),("columnCounts",f[5].Raw));
            var input=ReadPublicPuzzleInput(CanonicalDtoCursor.FromCanonical(LocalSaveHash.Canonical(json,semantic)));
            Require(claimed.Sha256==input.Identity.HashSha256,"SAVE_ENDLESS_REFERENCE_HASH_MISMATCH");
            return input;
        }
        private LocalEndlessState ReadRuntimeEndless(CanonicalDtoCursor v)
        {
            Require(profile.Use==CatalogUse.InternalPreview||retained.CalibratedEndless.Count>0,"SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);var f=v.Fields("highestReservedOrdinal","openEndless","recentPublicInputs","economyReceipts","recordVersion");f[4].RequireInteger(2);
            return LocalEndlessState.Runtime(f[0].String(),f[1].Array().Select(ReadLocalOpenEndless).ToArray(),f[2].Array().Select(ReadPublicPuzzleInput).ToArray(),f[3].Array().Select(ReadEconomyTerminal).ToArray());
        }
        private string WriteRuntimeEndless(LocalEndlessState state)
        {
            Require(WriteSchemaVersion==3&&(profile.Use==CatalogUse.InternalPreview||retained.CalibratedEndless.Count>0),"SAVE_SCHEMA_UNSUPPORTED",true);
            return Object(("highestReservedOrdinal",Write(state.HighestReservedOrdinal)),("openEndless","["+string.Join(",",state.OpenEndless.Select(Write))+"]"),("recentPublicInputs","["+string.Join(",",state.RecentPublicInputs.Select(Write))+"]"),("economyReceipts","["+string.Join(",",state.EconomyReceipts.Select(Write))+"]"),("recordVersion","2"));
        }
        private void ValidateRuntimeEndless(LocalProfileState state,long? generation)
        {
            if(state.Endless.RecordVersion==1)return;
            Require(profile.Use==CatalogUse.InternalPreview||retained.CalibratedEndless.Count>0,"SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);
            foreach(var done in state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Where(c=>c.Result!=null))
            {
                var result=done.Result!;Require(done.Descriptor.Parameters is SearchGeneratorParameters search&&search.StarCalibration!=null&&search.StarCalibration.Stars(result.ActiveElapsedMs)==result.Stars&&search.StarCalibration.CompletionCatalog.Equals(result.FirstClearTerminal.Catalog),"SAVE_ENDLESS_RESULT_BINDING");
                foreach(var terminal in result.Terminals){Require(!generation.HasValue||terminal.CommittedGeneration<=generation.Value,"SAVE_TERMINAL_GENERATION_INVALID");var references=state.EconomyJournal.Concat(state.LedgerCheckpoint.JournalDiagnosticTail).Where(e=>e.TransactionId==terminal.TransactionId).ToArray();if(references.Length>0)Require(state.Endless.EconomyReceipts.Any(r=>Write(r)==Write(terminal)),"SAVE_ENDLESS_RESULT_RECEIPT_MISMATCH");else Require(state.LedgerCheckpoint.ThroughSequence>0&&terminal.CommittedGeneration<=state.LedgerCheckpoint.SourceSaveGeneration,"SAVE_ENDLESS_RESULT_LEDGER_MISSING");}
            }
            foreach(var done in state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Where(c=>c.Claim.ProviderOperationId!=null))
            {var f=done.Claim.ProviderOperationId!.Split(':');Require(f.Length==3&&f[0]=="endless-provider-v1"&&long.TryParse(f[1],System.Globalization.NumberStyles.None,System.Globalization.CultureInfo.InvariantCulture,out long g)&&g>0&&g<=9007199254740991L&&(!generation.HasValue||g<=generation.Value)&&Guid.TryParseExact(f[2],"D",out _),"SAVE_ENDLESS_PROVIDER_GENERATION_INVALID");}
            foreach(var p in state.Endless.RecentPublicInputs){Require(p.Identity.PuzzleId.StartsWith("E1-",StringComparison.Ordinal),"SAVE_ENDLESS_REFERENCE_INVALID");EndlessProjection.ValidatePublic(p,json);}
            var entries=state.EconomyJournal.Concat(state.LedgerCheckpoint.JournalDiagnosticTail).ToArray();
            foreach(var entry in entries.Where(e=>e.SubjectId!=null&&e.SubjectId.StartsWith("E1-",StringComparison.Ordinal)))
                Require(state.Endless.EconomyReceipts.Any(r=>r.TransactionId==entry.TransactionId),"SAVE_ENDLESS_RECEIPT_REQUIRED");
            foreach(var receipt in state.Endless.EconomyReceipts)
            {
                var sources=retained.CompletionRewards.Where(d=>d.SubjectKind=="ENDLESS_INSTANCE"&&d.Catalog.Equals(receipt.Catalog)).ToArray();Require(sources.Length==1,"SAVE_ENDLESS_RECEIPT_SOURCE_UNSUPPORTED",true);Require(receipt.SubjectId.StartsWith("E1-",StringComparison.Ordinal),"SAVE_ENDLESS_RECEIPT_BINDING");
                Require(sources[0].RewardAmounts.TryGetValue(receipt.ReasonCode,out var amount)&&receipt.Amount==amount,"SAVE_ENDLESS_RECEIPT_AMOUNT");
                string suffix=receipt.ReasonCode=="LEVEL_FIRST_CLEAR"?"first-clear":receipt.ReasonCode=="LEVEL_SECOND_STAR"?"second-star":"third-star";string claim=receipt.ReasonCode=="POST_CLEAR_PATIENCE"?"reward-claim:POST_CLEAR_PATIENCE:"+receipt.SubjectId:"reward:"+receipt.ReasonCode+":"+receipt.SubjectId;
                Require(receipt.ClaimId==claim&&receipt.TransactionId==(receipt.ReasonCode=="POST_CLEAR_PATIENCE"?claim:"level:"+receipt.SubjectId+":"+suffix),"SAVE_ENDLESS_RECEIPT_ID_INVALID");
                Require(receipt.CommittedGeneration>0&&(!generation.HasValue||receipt.CommittedGeneration<=generation.Value),"SAVE_TERMINAL_GENERATION_INVALID");
                Require(entries.Any(e=>e.TransactionId==receipt.TransactionId&&e.ReasonCode==receipt.ReasonCode&&e.SubjectId==receipt.SubjectId&&e.Amount==receipt.Amount&&e.ClaimId==receipt.ClaimId&&e.CatalogId==receipt.Catalog.CatalogId&&e.CatalogRevision==receipt.Catalog.Revision&&e.CatalogHashSha256==receipt.Catalog.HashSha256&&e.CreatedAtUtc==receipt.CreatedAtUtc),"SAVE_ENDLESS_RECEIPT_REFERENCE_MISSING");
            }
        }
    }
}
