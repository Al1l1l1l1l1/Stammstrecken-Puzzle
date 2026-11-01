using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using STP.Application;
using STP.Infrastructure.Persistence;
using STP.Puzzle.Domain;

namespace STP.Tools.HumanReadback;

internal sealed class Observation
{
    public string ExportSha256{get;set;}="";public string SessionToken{get;set;}="";public long Generation{get;set;}public string ManifestSha256{get;set;}="";public string AssignmentId{get;set;}="";public string PuzzleId{get;set;}="";public string SemanticSha256{get;set;}="";public string SourceSha256{get;set;}="";public int? ContentRevision{get;set;}public string? ProfileId{get;set;}public string? ProfileHashSha256{get;set;}
    public string BuildId{get;set;}="";public string UiSourceSha256{get;set;}="";public Guid AttemptId{get;set;}public Guid ExposureId{get;set;}public Guid ClockEpoch{get;set;}public string Origin{get;set;}="";public string Outcome{get;set;}="";public string? SelfReportedContact{get;set;}public bool PriorVersion{get;set;}public bool PriorExposure{get;set;}public bool Interrupted{get;set;}public bool RunComplete{get;set;}public string ExposureState{get;set;}="";public bool TimingComplete{get;set;}
    public long ObservedActiveElapsedMs{get;set;}public long? ObservedPreviewActiveMs{get;set;}public bool TimerStarted{get;set;}public int ConfirmedHintCount{get;set;}public int PossibleOrConfirmedHintSlots{get;set;}public Guid[] HintDisplayIds{get;set;}=Array.Empty<Guid>();public int HintSlotsAcrossRunPuzzleHistory{get;set;}public long CorrectionCount{get;set;}public bool UsedEmptyMarker{get;set;}public bool UsedOccupiedMarker{get;set;}
    public SortedSet<string> Reasons{get;}=new(StringComparer.Ordinal);
    public string MeasurementClass=>Reasons.Count==0?"UNATTESTED_UNASSISTED_FIRST_CONTACT_OBSERVATION":"EXCLUDED_OR_SEPARATE_OBSERVATION";
    public bool HumanIdentityAttested=>false;public bool DeviceAccepted=>false;public bool CalibrationApproved=>false;public bool PromotionAllowed=>false;public bool ExternalReviewerRequired=>true;
}
internal sealed class ExportInput
{
    public string Path{get;set;}="";public string Sha256{get;set;}="";public string? DuplicateOf{get;set;}public string? Rejection{get;set;}public string? SessionToken{get;set;}public long? Generation{get;set;}
    [System.Text.Json.Serialization.JsonIgnore]public byte[] Bytes{get;set;}=Array.Empty<byte>();
}
internal sealed class Readback
{
    internal readonly List<Observation> Rows=new();internal readonly List<ExportInput> Inputs=new();
    private readonly SourceBundle sources;
    internal Readback(SourceBundle sources){this.sources=sources;}
    internal void Add(string path,byte[] bytes)
    {
        var input=new ExportInput{Path=System.IO.Path.GetFullPath(path),Sha256=SourceBundle.Hash(bytes),Bytes=bytes};Inputs.Add(input);var duplicate=Inputs.FirstOrDefault(x=>!ReferenceEquals(x,input)&&x.Sha256==input.Sha256);if(duplicate!=null){input.DuplicateOf=duplicate.Path;return;}
        try
        {
            var header=SourceBundle.Parse(bytes);string manifest=SourceBundle.S(header,"manifestBytesSha256");SourceBundle.Need(sources.Catalogs.TryGetValue(manifest,out var catalog),"READBACK_UNKNOWN_MANIFEST");var snapshot=new HumanCalibrationCodec(sources.Json,catalog!).Decode(bytes);input.SessionToken=snapshot.Run.SessionToken;input.Generation=snapshot.Generation;
            foreach(var trial in snapshot.Run.Trials)
            {
                var assignment=catalog!.Assignments.Single(a=>a.Id==trial.AssignmentId);var row=new Observation{ExportSha256=input.Sha256,SessionToken=snapshot.Run.SessionToken,Generation=snapshot.Generation,ManifestSha256=manifest,AssignmentId=assignment.Id,PuzzleId=trial.Identity.PuzzleId,SemanticSha256=trial.Identity.HashSha256,SourceSha256=trial.SourceSha256,ContentRevision=assignment.ContentRevision,ProfileId=assignment.ProfileId,ProfileHashSha256=assignment.ProfileHashSha256,BuildId=catalog.BuildId,UiSourceSha256=catalog.UiSourceSha256,AttemptId=trial.Checkpoint.AttemptId,ExposureId=trial.ExposureId,ClockEpoch=trial.ClockEpoch,Origin=snapshot.Run.Origin.ToString(),Outcome=trial.Outcome.ToString(),SelfReportedContact=trial.Contact?.ToString(),PriorVersion=trial.PriorVersion,PriorExposure=trial.PriorExposure,Interrupted=trial.Interrupted,RunComplete=snapshot.Run.NextAssignmentIndex==catalog.Assignments.Count,ExposureState=trial.Exposure.ToString(),TimingComplete=trial.TimingComplete,ObservedActiveElapsedMs=trial.Checkpoint.ActiveElapsedMs,ObservedPreviewActiveMs=trial.PreviewActiveMs,TimerStarted=trial.Checkpoint.TimerStarted,ConfirmedHintCount=trial.Checkpoint.HintCount,PossibleOrConfirmedHintSlots=trial.Hints.Count,CorrectionCount=trial.Checkpoint.CorrectionCount,UsedEmptyMarker=trial.Checkpoint.UsedEmptyMarker,UsedOccupiedMarker=trial.Checkpoint.UsedOccupiedMarker};
                row.HintDisplayIds=trial.Hints.Select(h=>h.DisplayId).ToArray();void Reject(bool condition,string code){if(condition)row.Reasons.Add(code);}
                Reject(snapshot.Run.Origin!=HumanRunOrigin.PARTICIPANT_UI,"AUTOMATED_TEST_ORIGIN");Reject(trial.Outcome!=HumanTrialOutcome.SOLVED_UNREVIEWED,"NOT_SOLVED");Reject(!PuzzleEvaluator.Evaluate(assignment.Puzzle.ToDomainDefinition(),trial.Checkpoint.Cells).Completed,"DOMAIN_NOT_COMPLETED");Reject(trial.Interrupted,"INTERRUPTED");Reject(trial.Exposure!=HumanExposureState.EXPOSURE_CONFIRMED||!trial.VisibleAtMs.HasValue,"EXPOSURE_UNCONFIRMED_OR_UNCERTAIN");Reject(!trial.TimingComplete||!trial.PreviewActiveMs.HasValue,"TIMING_INCOMPLETE_OR_PREVIEW_MISSING");Reject(!trial.Checkpoint.TimerStarted,"DOMAIN_TIMER_NOT_STARTED");Reject(trial.Contact==null,"FIRST_CONTACT_NOT_RECORDED");Reject(trial.Contact==HumanFirstContact.SEEN_OR_PLAYED,"SELF_REPORTED_SEEN");Reject(trial.Contact==HumanFirstContact.UNSURE,"SELF_REPORTED_UNSURE");Reject(trial.PriorVersion,"SELF_REPORTED_PRIOR_VERSION");Reject(trial.PriorExposure,"PRIOR_OR_POSSIBLE_EXPOSURE");Reject(trial.Hints.Count>0||trial.Checkpoint.HintCount>0,"CONFIRMED_OR_POSSIBLE_HINT_ASSISTANCE");Reject(trial.Hints.Any(h=>h.State!=HumanHintState.HINT_DISPLAY_CONFIRMED),"HINT_DISPLAY_PENDING_OR_UNCERTAIN");Rows.Add(row);
            }
        }
        catch(Exception e)when(e is ArgumentException||e is InvalidOperationException||e is SaveCodecFailure){input.Rejection=e.Message;}
    }
    internal void ClassifyDuplicates()
    {
        foreach(var group in Inputs.Where(i=>i.SessionToken!=null&&i.DuplicateOf==null).GroupBy(i=>i.SessionToken))if(group.Count()>1)foreach(var row in Rows.Where(r=>r.SessionToken==group.Key))row.Reasons.Add("SESSION_HAS_MULTIPLE_EXPORTS_OR_CONFLICT");
        foreach(var group in Rows.GroupBy(r=>r.AttemptId))if(group.Count()>1)foreach(var row in group)row.Reasons.Add("TRIAL_ID_REUSED_ACROSS_RECORDS");
        foreach(var group in Rows.GroupBy(r=>r.ExposureId))if(group.Count()>1)foreach(var row in group)row.Reasons.Add("EXPOSURE_ID_REUSED_ACROSS_TRIALS_OR_RECORDS");
        foreach(var group in Rows.GroupBy(r=>r.SessionToken+":"+r.SemanticSha256))
        {
            int historySlots=group.SelectMany(r=>r.HintDisplayIds).Distinct().Count();foreach(var row in group){row.HintSlotsAcrossRunPuzzleHistory=historySlots;if(group.Count()>1)row.Reasons.Add("REPEATED_PUZZLE_WITHIN_RUN");if(historySlots>0)row.Reasons.Add("HINT_ASSISTANCE_IN_RUN_PUZZLE_HISTORY");}
        }
    }
    internal object Report()=>new{reportKind="INTERNAL_UNATTESTED_HUMAN_OBSERVATION_READBACK",toolReportVersion=1,readerAssemblySha256=SourceBundle.Hash(File.ReadAllBytes(typeof(Readback).Assembly.Location)),sourceRoot=sources.Root,sourceRootIsCallerDeclaration=true,humanIdentityAttested=false,deviceAccepted=false,humanAcceptance=false,starTimesProduced=false,profileApproved=false,promotionAllowed=false,externalReviewerRequired=true,duplicatePolicy="Identical original bytes count once. Multiple differing exports of one session and reused trial/exposure identities are excluded from the unassisted first-contact subset; no generation is automatically preferred.",inputs=Inputs,observations=Rows,byPuzzle=Rows.GroupBy(r=>r.PuzzleId).OrderBy(g=>g.Key,StringComparer.Ordinal).Select(g=>new{puzzleId=g.Key,observations=g.Count(),unattestedCandidates=g.Count(r=>r.Reasons.Count==0),actualTimes=g.Where(r=>r.Reasons.Count==0).Select(r=>new{r.ExportSha256,r.SessionToken,r.AttemptId,r.ObservedActiveElapsedMs,r.ObservedPreviewActiveMs})}),byProfile=Rows.Where(r=>r.ProfileId!=null).GroupBy(r=>r.ProfileId).OrderBy(g=>g.Key,StringComparer.Ordinal).Select(g=>new{profileId=g.Key,observations=g.Count(),unattestedCandidates=g.Count(r=>r.Reasons.Count==0),actualTimes=g.Where(r=>r.Reasons.Count==0).Select(r=>new{r.PuzzleId,r.ExportSha256,r.SessionToken,r.ObservedActiveElapsedMs,r.ObservedPreviewActiveMs})}),provenance=sources.Provenance.Select(p=>new{name=p.Key,sha256=SourceBundle.Hash(p.Value),sizeBytes=p.Value.Length})};
    internal void Write(string output)
    {
        output=Path.GetFullPath(output);SourceBundle.CheckPath(output);SourceBundle.Need(!Directory.Exists(output)&&!File.Exists(output),"READBACK_OUTPUT_EXISTS");Directory.CreateDirectory(output);Directory.CreateDirectory(Path.Combine(output,"original-exports"));Directory.CreateDirectory(Path.Combine(output,"provenance"));
        foreach(var group in Inputs.GroupBy(i=>i.Sha256))File.WriteAllBytes(Path.Combine(output,"original-exports",group.Key+".json"),group.First().Bytes);
        foreach(var p in sources.Provenance)File.WriteAllBytes(Path.Combine(output,"provenance",p.Key+"-"+SourceBundle.Hash(p.Value)+".json"),p.Value);
        File.WriteAllText(Path.Combine(output,"readback.internal.json"),JsonSerializer.Serialize(Report(),new JsonSerializerOptions{WriteIndented=true,PropertyNamingPolicy=JsonNamingPolicy.CamelCase})+"\n");
    }
}
