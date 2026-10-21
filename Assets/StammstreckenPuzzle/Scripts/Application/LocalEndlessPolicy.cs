using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Application
{
    /// <summary>Actual verified generation result, bound to the one already committed reservation.</summary>
    public sealed class LocalEndlessCandidate
    {
        internal LocalEndlessCandidate(ReservedEndless reservation,PublicPuzzleInput puzzle,LocalHashReference solution,LocalHashReference proof){Reservation=reservation;Puzzle=puzzle;SolutionHash=solution;ProofHash=proof;}
        public ReservedEndless Reservation{get;}
        public PublicPuzzleInput Puzzle{get;}
        public LocalHashReference SolutionHash{get;}
        public LocalHashReference ProofHash{get;}
    }
    /// <summary>Actual internal endless lifecycle, with bounded novelty and exact existing ledger/descriptor projections.</summary>
    public sealed class LocalEndlessPolicy
    {
        private readonly IPublicPuzzleCatalog corpus;private readonly CatalogIdentity completion;private readonly ICanonicalJson json;private readonly ILocalLedger ledger;
        public LocalEndlessPolicy(IPublicPuzzleCatalog corpus,CatalogIdentity completion,ICanonicalJson json,ILocalLedger ledger)
        {this.corpus=corpus??throw new ArgumentNullException(nameof(corpus));this.completion=completion??throw new ArgumentNullException(nameof(completion));this.json=json??throw new ArgumentNullException(nameof(json));this.ledger=ledger??throw new ArgumentNullException(nameof(ledger));LocalModel.Require(corpus.Identity.Use==CatalogUse.InternalPreview&&completion.Use==CatalogUse.InternalPreview&&completion.CatalogId=="completion-qa-mobile-candidate-endless","ENDLESS_CANDIDATE_REQUIRED");}
        private void Binding(LocalProfileState state){LocalModel.Require(state.Profile.Identity.Use==CatalogUse.InternalPreview&&state.Profile.CatalogSet.PublicCatalog.Equals(corpus.Identity),"ENDLESS_PROFILE_BINDING");ledger.Validate(state);}
        public bool IsUnlocked(LocalProfileState state){Binding(state);return corpus.Puzzles.Count==240&&corpus.Puzzles.All(p=>state.Levels.Any(l=>l.Identity.Equals(p.Identity)&&l.FirstClear!=null));}
        public static string CompletionSourceHash(LocalEndlessClaim claim)
        {var f=claim.LocalOperationId.Split(':');LocalModel.Require(f.Length==3&&f[0]=="endles-claim-v1"&&System.Text.RegularExpressions.Regex.IsMatch(f[1],"^[a-f0-9]{64}$")&&Guid.TryParseExact(f[2],"D",out _),"ENDLESS_COMPLETION_SOURCE_REQUIRED");return f[1];}
        private void ClaimBinding(OpenEndlessCompletion done)=>LocalModel.Require(CompletionSourceHash(done.Claim)==completion.HashSha256,"ENDLESS_COMPLETION_SOURCE_CHANGED");
        private static LocalProfileState Change(LocalProfileState state,System.Collections.Generic.IReadOnlyList<LocalOpenEndless> open,System.Collections.Generic.IReadOnlyList<PublicPuzzleInput>? recent=null,System.Collections.Generic.IReadOnlyList<EconomyTerminal>? receipts=null,string? highest=null)
            =>LocalEndlessEconomy.WithEndless(state,LocalEndlessState.Runtime(highest??state.Endless.HighestReservedOrdinal,open,recent??state.Endless.RecentPublicInputs,receipts??state.Endless.EconomyReceipts));
        public LocalProfileState Reserve(LocalProfileState state,EndlessSearchProfile profile,string seed)
        {
            Binding(state);LocalModel.Require(IsUnlocked(state),"ENDLESS_SEASON_FIRST_CLEARS_REQUIRED");LocalModel.UInt64(seed);LocalModel.Require(profile.PublicCatalogHashSha256==corpus.Identity.HashSha256,"ENDLESS_PROFILE_CORPUS_CHANGED");LocalModel.Require(state.Endless.OpenEndless.Count<20,"ENDLESS_OPEN_CAPACITY");
            ulong highest=ulong.Parse(state.Endless.HighestReservedOrdinal,CultureInfo.InvariantCulture);LocalModel.Require(highest<ulong.MaxValue,"ENDLESS_ORDINAL_SPACE_EXHAUSTED");string ordinal=(highest+1).ToString(CultureInfo.InvariantCulture);
            var parameters=new SearchGeneratorParameters(profile.Id,profile.HashSha256,PuzzleCandidateSearch.NoveltyVersion,corpus.Identity,state.Endless.RecentPublicInputs,profile.Search);
            string hash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,parameters.Projection()));var descriptor=LocalEndlessDescriptor.Create("train-track-v1",PuzzleCandidateSearch.GeneratorVersion,seed,ordinal,parameters,hash);
            var reserved=ReservedEndless.Create(ordinal,EndlessProjection.DescriptorId(descriptor,json),descriptor);
            return Change(state,state.Endless.OpenEndless.Concat(new[]{reserved}).ToArray(),highest:ordinal);
        }
        /// <summary>Only the caller's already acknowledged snapshot may supply this reservation. A timeout returns null, retaining it for the same descriptor's retry.</summary>
        public async Task<LocalEndlessCandidate?> GenerateAsync(SaveSnapshot<LocalProfileState> committed,string id,CancellationToken cancellation)
        {
            Binding(committed.Payload);LocalModel.Require(committed.Generation>=1,"ENDLESS_RESERVATION_NOT_COMMITTED");
            var reserved=committed.Payload.Endless.OpenEndless.OfType<ReservedEndless>().SingleOrDefault(r=>r.Id==id)??throw new ArgumentException("ENDLESS_RESERVATION_REQUIRED");
            var p=reserved.Descriptor.Parameters as SearchGeneratorParameters??throw new ArgumentException("ENDLESS_GENERATOR_RECOVERY_REQUIRED");
            LocalModel.Require(reserved.Descriptor.GeneratorVersion==PuzzleCandidateSearch.GeneratorVersion&&p.CampaignCatalog.Equals(corpus.Identity)&&EndlessProjection.DescriptorId(reserved.Descriptor,json)==id,"ENDLESS_GENERATOR_RECOVERY_REQUIRED");
            var generated=await PuzzleCandidateSearch.GenerateAsync(ulong.Parse(reserved.Descriptor.Seed,CultureInfo.InvariantCulture),p.Search,corpus.Puzzles.Select(x=>x.ToDomainDefinition()).ToArray(),p.RecentPublicInputs.Select(x=>x.ToDomainDefinition()).ToArray(),cancellation).ConfigureAwait(false);
            if(!generated.Succeeded)return null;var definition=generated.Puzzle!;var result=generated.Verification!;
            LocalModel.Require(result.Classification==SolverClassification.UNIQUE&&result.Metrics!=null&&result.Metrics.RequiredGuessDepth==0,"ENDLESS_VERIFICATION_REQUIRED");
            var temporary=new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,new string('0',64)),definition.RulesetVersion,definition.Grid.Width,definition.Grid.Height,(PublicPuzzleSide)definition.A.Side,definition.A.Index,(PublicPuzzleSide)definition.B.Side,definition.B.Index,definition.RowCounts,definition.ColumnCounts);
            string publicHash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,EndlessProjection.PublicInput(temporary,false)));
            var puzzle=new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,publicHash),temporary.RulesetVersion,temporary.Width,temporary.Height,temporary.A.Side,temporary.A.Index,temporary.B.Side,temporary.B.Index,temporary.RowCounts,temporary.ColumnCounts);
            string HashRef(string profile,string hash)=>"{\"profile\":"+LocalSaveHash.Quote(profile)+",\"sha256\":"+LocalSaveHash.Quote(hash)+"}";
            Direction Toward(CellCoordinate from,CellCoordinate to)=>to.Y<from.Y?Direction.N:to.X>from.X?Direction.E:to.Y>from.Y?Direction.S:Direction.W;
            string PathCell(int index)
            {
                var cell=result.Path[index];var incoming=index==0?definition.A.Side:Toward(cell,result.Path[index-1]);var outgoing=index==result.Path.Count-1?definition.B.Side:Toward(cell,result.Path[index+1]);int ports=(1<<(int)incoming)|(1<<(int)outgoing);int shape=-1;for(int s=0;s<6;s++)if(TrackGeometry.Ports((TrackShape)s)==ports)shape=s;LocalModel.Require(shape>=0,"ENDLESS_SOLUTION_TRACK_INVALID");
                return "{\"x\":"+cell.X.ToString(CultureInfo.InvariantCulture)+",\"y\":"+cell.Y.ToString(CultureInfo.InvariantCulture)+",\"track\":"+LocalSaveHash.Quote("TRACK_"+((TrackShape)shape).ToString())+"}";
            }
            string solution="{\"puzzleId\":"+LocalSaveHash.Quote(id)+",\"publicPuzzleHash\":"+HashRef(PublicPuzzleIdentity.SemanticHashProfile,publicHash)+",\"path\":["+string.Join(",",Enumerable.Range(0,result.Path.Count).Select(PathCell))+"]}";
            string solutionHash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,solution));var m=result.Metrics!;
            string proof="{\"proofFormatVersion\":1,\"puzzleId\":"+LocalSaveHash.Quote(id)+",\"publicPuzzleHash\":"+HashRef(PublicPuzzleIdentity.SemanticHashProfile,publicHash)+",\"solutionHash\":"+HashRef("STP-SOLUTION-JCS-1",solutionHash)+",\"solverVersion\":\"solver-v2\",\"solutionCount\":1,\"metrics\":{\"searchNodes\":"+m.SearchNodes+",\"deductionSteps\":"+m.DeductionSteps+",\"maxDeductionDepth\":"+m.MaxDeductionDepth+",\"requiredGuessDepth\":"+m.RequiredGuessDepth+"}}";
            return new LocalEndlessCandidate(reserved,puzzle,LocalHashReference.Create("STP-SOLUTION-JCS-1",solutionHash),LocalHashReference.Create("STP-PROOF-JCS-1",LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,proof))));
        }
        public LocalProfileState Promote(LocalProfileState state,LocalEndlessCandidate candidate,Guid attempt)
        {
            Binding(state);var prior=state.Endless.OpenEndless.SingleOrDefault(r=>r.Id==candidate.Reservation.Id);LocalModel.Require(prior is ReservedEndless&&prior.Descriptor.ParameterHashSha256==candidate.Reservation.Descriptor.ParameterHashSha256&&prior.GenerationOrdinal==candidate.Reservation.GenerationOrdinal,"ENDLESS_PROMOTION_RESERVATION_CHANGED");
            var parameters=prior!.Descriptor.Parameters as SearchGeneratorParameters??throw new ArgumentException("ENDLESS_GENERATOR_RECOVERY_REQUIRED");var definition=candidate.Puzzle.ToDomainDefinition();string signature=PuzzleCandidateSearch.CanonicalSignature(definition);
            LocalModel.Require(!state.Endless.RecentPublicInputs.Any(p=>PuzzleCandidateSearch.CanonicalSignature(p.ToDomainDefinition())==signature||PuzzleCandidateSearch.SimilarityDistancePercent(definition,p.ToDomainDefinition())<=parameters.Search.RecentSimilarityPercent),"ENDLESS_PROMOTION_NOVELTY_CONFLICT");
            EndlessProjection.ValidatePublic(candidate.Puzzle,json);var s=PuzzleSessionState.Create(candidate.Puzzle.ToDomainDefinition(),candidate.Puzzle.Identity.PuzzleId,candidate.Puzzle.Identity.HashProfile,candidate.Puzzle.Identity.HashSha256,attempt,SessionMode.ENDLESS,0,null,null).Value??throw new ArgumentException("ENDLESS_SESSION_INVALID");
            var active=ActiveEndlessDraft.Create(prior!.GenerationOrdinal,prior.Id,prior.Descriptor,candidate.Puzzle,LocalHashReference.Create(candidate.Puzzle.Identity.HashProfile,candidate.Puzzle.Identity.HashSha256),candidate.SolutionHash,candidate.ProofHash,PuzzleSessionCheckpoint.Export(s,0));
            var recent=state.Endless.RecentPublicInputs.Concat(new[]{candidate.Puzzle}).ToArray();if(recent.Length>50)recent=recent.Skip(recent.Length-50).ToArray();
            return Change(state,state.Endless.OpenEndless.Select(r=>r.Id==prior.Id?active:r).ToArray(),recent);
        }
        private static bool SameDefinition(PuzzleDefinition first,PuzzleDefinition second)=>first.RulesetVersion==second.RulesetVersion&&first.Grid.Width==second.Grid.Width&&first.Grid.Height==second.Grid.Height&&first.A.Side==second.A.Side&&first.A.Index==second.A.Index&&first.B.Side==second.B.Side&&first.B.Index==second.B.Index&&first.RowCounts.SequenceEqual(second.RowCounts)&&first.ColumnCounts.SequenceEqual(second.ColumnCounts);
        public LocalProfileState SaveDraft(LocalProfileState state,PuzzleSessionState session,long monotonic)
        {
            Binding(state);var active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.Id==session.PuzzleId)??throw new ArgumentException("ENDLESS_DRAFT_REQUIRED");
            LocalModel.Require(SameDefinition(session.Definition,active.PublicPuzzleInput.ToDomainDefinition())&&session.Mode==SessionMode.ENDLESS&&session.PublicPuzzleHashSha256==active.PublicPuzzleInput.Identity.HashSha256,"ENDLESS_DRAFT_BINDING");var updated=ActiveEndlessDraft.Create(active.GenerationOrdinal,active.Id,active.Descriptor,active.PublicPuzzleInput,active.PublicPuzzleHash,active.SolutionHash,active.ProofHash,PuzzleSessionCheckpoint.Export(session,monotonic));
            return Change(state,state.Endless.OpenEndless.Select(r=>r.Id==active.Id?updated:r).ToArray());
        }
        public LocalProfileState RestartDraft(LocalProfileState state,string id,Guid attempt)
        {
            Binding(state);var active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.Id==id)??throw new ArgumentException("ENDLESS_DRAFT_REQUIRED");var p=active.PublicPuzzleInput;
            var ready=PuzzleSessionState.Create(p.ToDomainDefinition(),id,p.Identity.HashProfile,p.Identity.HashSha256,attempt,SessionMode.ENDLESS,0,null,null).Value??throw new ArgumentException("ENDLESS_SESSION_INVALID");var restarted=ActiveEndlessDraft.Create(active.GenerationOrdinal,id,active.Descriptor,p,active.PublicPuzzleHash,active.SolutionHash,active.ProofHash,PuzzleSessionCheckpoint.Export(ready,0));return Change(state,state.Endless.OpenEndless.Select(r=>r.Id==id?restarted:r).ToArray());
        }
        public LocalProfileState Complete(LocalProfileState state,PuzzleSessionState session,long generation,string utc)
        {
            Binding(state);var active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.Id==session.PuzzleId)??throw new ArgumentException("ENDLESS_DRAFT_REQUIRED");
            LocalModel.Require(SameDefinition(session.Definition,active.PublicPuzzleInput.ToDomainDefinition())&&session.Mode==SessionMode.ENDLESS&&session.Phase==SessionPhase.SolvedPendingCommit&&session.PublicPuzzleHashSha256==active.PublicPuzzleInput.Identity.HashSha256&&session.SolvedEventId=="puzzle-solved:"+session.AttemptId.ToString("D")&&PuzzleEvaluator.Evaluate(session.Definition,session.Cells).Completed,"ENDLESS_COMPLETION_NOT_SOLVED");
            state=ledger.Compact(state,generation);var claim=LocalEndlessClaim.Create("reward-claim:POST_CLEAR_PATIENCE:"+active.Id,"endles-claim-v1:"+completion.HashSha256+":"+Guid.NewGuid().ToString("D"),"LOCAL_DECISION_PENDING",null);
            var done=OpenEndlessCompletion.Create(active.GenerationOrdinal,active.Id,active.Descriptor,active.Id,session.SolvedEventId!,claim);
            var terminal=EconomyTerminal.Create("level:"+active.Id+":first-clear","LEVEL_FIRST_CLEAR",active.Id,completion,"reward:LEVEL_FIRST_CLEAR:"+active.Id,15,generation,utc);
            var entry=ledger.Next(state.LedgerCheckpoint,state.EconomyJournal,terminal);
            var next=Change(state,state.Endless.OpenEndless.Select(r=>r.Id==active.Id?done:r).ToArray(),receipts:state.Endless.EconomyReceipts.Concat(new[]{terminal}).ToArray());
            if(next.Metagame!=null)next=next.WithMetagame(new LocalMetagameState(next.Metagame.Hints.Where(h=>h.Identity.PuzzleId!=active.Id).ToArray(),next.Metagame.Daily));
            next=next.WithProgress(next.Levels,next.LedgerCheckpoint,next.EconomyJournal.Concat(new[]{entry}).ToArray(),next.SeasonProgressCache,next.Drafts,next.RewardClaims);
            return ledger.Compact(next,generation);
        }
        public LocalProfileState Abandon(LocalProfileState state,string id)
        {Binding(state);var found=state.Endless.OpenEndless.SingleOrDefault(r=>r.Id==id);if(found==null)return state;LocalModel.Require(found is ReservedEndless||found is ActiveEndlessDraft,"ENDLESS_OPEN_CLAIM_CANNOT_ABANDON");return Remove(state,id);}
        public LocalProfileState SkipReward(LocalProfileState state,string id)
        {Binding(state);var found=state.Endless.OpenEndless.SingleOrDefault(r=>r.Id==id);if(found==null)return state;LocalModel.Require(found is OpenEndlessCompletion c&&c.Claim.ClaimStatus=="LOCAL_DECISION_PENDING"&&c.Claim.ProviderOperationId==null,"ENDLESS_SKIP_NOT_ELIGIBLE");ClaimBinding((OpenEndlessCompletion)found);return Remove(state,id);}
        private static OpenEndlessCompletion WithClaim(OpenEndlessCompletion c,LocalEndlessClaim claim)=>OpenEndlessCompletion.Create(c.GenerationOrdinal,c.Id,c.Descriptor,c.PuzzleId,c.CompletionCommitId,claim);
        private static long ProviderGeneration(string operation)
        {var f=operation.Split(':');LocalModel.Require(f.Length==3&&f[0]=="endless-provider-v1"&&long.TryParse(f[1],NumberStyles.None,CultureInfo.InvariantCulture,out _)&&Guid.TryParseExact(f[2],"D",out _),"ENDLESS_PROVIDER_OPERATION_INVALID");return LocalModel.Positive(long.Parse(f[1],CultureInfo.InvariantCulture));}
        public LocalProfileState ReserveReward(LocalProfileState state,string id,long generation)
        {
            Binding(state);var done=state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Id==id)??throw new ArgumentException("ENDLESS_COMPLETION_REQUIRED");ClaimBinding(done);
            if(done.Claim.ClaimStatus!="LOCAL_DECISION_PENDING")return state;
            string provider="endless-provider-v1:"+LocalModel.Positive(generation).ToString(CultureInfo.InvariantCulture)+":"+Guid.NewGuid().ToString("D");
            var next=WithClaim(done,LocalEndlessClaim.Create(done.Claim.ClaimId,done.Claim.LocalOperationId,"PROVIDER_RESERVED",provider));return Change(state,state.Endless.OpenEndless.Select(c=>c.Id==id?next:c).ToArray());
        }
        public static AdRewardReservation RewardReservation(SaveCommitResult<LocalProfileState> committed,string id)
        {
            LocalModel.Require(committed.Succeeded&&committed.Snapshot!=null,"ENDLESS_REWARD_NOT_COMMITTED");var done=committed.Snapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Id==id);
            LocalModel.Require(done!=null&&(done.Claim.ClaimStatus=="PROVIDER_RESERVED"||done.Claim.ClaimStatus=="RECONCILIATION_REQUIRED")&&done.Claim.ProviderOperationId!=null,"ENDLESS_REWARD_RESERVATION_REQUIRED");CompletionSourceHash(done!.Claim);long generation=ProviderGeneration(done.Claim.ProviderOperationId!);LocalModel.Require(generation<=committed.Snapshot.Generation,"ENDLESS_REWARD_GENERATION_INVALID");return new AdRewardReservation(done.Claim.ClaimId,done.Claim.ProviderOperationId!,generation);
        }
        private static bool Matches(OpenEndlessCompletion c,AdRewardReservation r)=>c.Claim.ClaimId==r.ClaimId&&c.Claim.ProviderOperationId==r.OperationId&&ProviderGeneration(r.OperationId)==r.SaveGeneration;
        public LocalProfileState ConfirmReward(LocalProfileState state,AdRewardEvidence evidence,long generation,string utc)
        {
            Binding(state);var done=state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Claim.ClaimId==evidence.Reservation.ClaimId);
            if(done==null||done.Claim.ClaimStatus=="LOCAL_DECISION_PENDING"||evidence.ProviderOperationId!=evidence.Reservation.OperationId||!Matches(done,evidence.Reservation))return state;ClaimBinding(done);state=ledger.Compact(state,generation);
            var terminal=EconomyTerminal.Create(done.Claim.ClaimId,"POST_CLEAR_PATIENCE",done.Id,completion,done.Claim.ClaimId,10,generation,utc);var entry=ledger.Next(state.LedgerCheckpoint,state.EconomyJournal,terminal);
            var next=Change(state,state.Endless.OpenEndless.Where(c=>c.Id!=done.Id).ToArray(),receipts:state.Endless.EconomyReceipts.Concat(new[]{terminal}).ToArray());next=next.WithProgress(next.Levels,next.LedgerCheckpoint,next.EconomyJournal.Concat(new[]{entry}).ToArray(),next.SeasonProgressCache,next.Drafts,next.RewardClaims);return ledger.Compact(Remove(next,done.Id),generation);
        }
        public LocalProfileState ResolveReward(LocalProfileState state,AdRewardReservation reservation,bool definitelyNoReward)
        {
            Binding(state);var done=state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Claim.ClaimId==reservation.ClaimId);if(done==null||done.Claim.ClaimStatus=="LOCAL_DECISION_PENDING"||!Matches(done,reservation))return state;ClaimBinding(done);if(definitelyNoReward)return Remove(state,done.Id);
            var next=WithClaim(done,LocalEndlessClaim.Create(done.Claim.ClaimId,done.Claim.LocalOperationId,"RECONCILIATION_REQUIRED",done.Claim.ProviderOperationId));return Change(state,state.Endless.OpenEndless.Select(c=>c.Id==done.Id?next:c).ToArray());
        }
        private static LocalProfileState Remove(LocalProfileState state,string id)
        {
            var result=Change(state,state.Endless.OpenEndless.Where(r=>r.Id!=id).ToArray());
            if(result.Metagame!=null)result=result.WithMetagame(new LocalMetagameState(result.Metagame.Hints.Where(h=>h.Identity.PuzzleId!=id).ToArray(),result.Metagame.Daily));
            return LocalEndlessEconomy.Prune(result);
        }
    }
}
