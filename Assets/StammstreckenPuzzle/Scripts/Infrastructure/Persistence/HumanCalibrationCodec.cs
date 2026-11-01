using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Separate ADR042 observation format. It is intentionally not a game-save envelope or migration input.</summary>
    public sealed class HumanCalibrationCodec
    {
        private readonly ICanonicalJson json;private readonly IHumanAssignmentCatalog assignments;
        public HumanCalibrationCodec(ICanonicalJson json,IHumanAssignmentCatalog assignments){this.json=json??throw new ArgumentNullException(nameof(json));this.assignments=assignments??throw new ArgumentNullException(nameof(assignments));}
        private static string Q(string? value)=>value==null?"null":LocalSaveHash.Quote(value);
        private static string N(long value)=>value.ToString(CultureInfo.InvariantCulture);
        private static string B(bool value)=>value?"true":"false";
        private static string O(params(string Name,string Value)[] fields)=>LocalSaveCodec.Object(fields);
        private static string NullN(long? value)=>value.HasValue?N(value.Value):"null";
        private static void Need(bool value,string error){if(!value)throw new ArgumentException(error);}
        public byte[] Encode(HumanObservationSnapshot snapshot)
        {
            Validate(snapshot);string payload=WriteRun(snapshot.Run);var bytes=LocalSaveHash.Canonical(json,payload);
            return LocalSaveHash.Canonical(json,O(("humanRecordVersion","1"),("kind",Q("HUMAN_CALIBRATION_RECORD")),("manifestBytesSha256",Q(assignments.ManifestBytesSha256)),("buildId",Q(assignments.BuildId)),("uiSourceSha256",Q(assignments.UiSourceSha256)),("generation",N(snapshot.Generation)),("payload",Encoding.UTF8.GetString(bytes)),("payloadSha256",Q(LocalSaveHash.Bytes(bytes)))));
        }
        public HumanObservationSnapshot Decode(byte[] bytes)
        {
            Need(bytes!=null&&bytes.Length<=8388608,"HUMAN_RECORD_SIZE_INVALID");var parsed=json.Canonicalize(bytes!,new JsonResourceLimits(8388608,64,65536,1048576,256));Need(parsed.Succeeded,"HUMAN_RECORD_JSON_INVALID");var f=CanonicalDtoCursor.FromCanonical(parsed.GetUtf8()!).Fields("humanRecordVersion","kind","manifestBytesSha256","buildId","uiSourceSha256","generation","payload","payloadSha256");
            f[0].RequireInteger(1);f[1].RequireString("HUMAN_CALIBRATION_RECORD");Need(f[2].String()==assignments.ManifestBytesSha256&&f[3].String()==assignments.BuildId&&f[4].String()==assignments.UiSourceSha256,"HUMAN_RECORD_SOURCE_MISMATCH");Need(LocalSaveHash.Bytes(f[6].Bytes)==f[7].String(),"HUMAN_RECORD_HASH_MISMATCH");var result=new HumanObservationSnapshot(f[5].Integer(),ReadRun(f[6]));Validate(result);return result;
        }
        private void Validate(HumanObservationSnapshot snapshot)
        {
            var run=snapshot.Run;Need(run.ManifestBytesSha256==assignments.ManifestBytesSha256&&run.NextAssignmentIndex<=assignments.Assignments.Count,"HUMAN_RECORD_SOURCE_MISMATCH");
            int lastIndex=-1;HumanTrialRecord? previous=null;var lastByIndex=new Dictionary<int,HumanTrialRecord>();
            foreach(var trial in run.Trials)
            {
                int index=-1;for(int i=0;i<assignments.Assignments.Count;i++)if(assignments.Assignments[i].Id==trial.AssignmentId){index=i;break;}
                Need(index>=0&&index<=run.NextAssignmentIndex&&index>=lastIndex&&index<=lastIndex+1,"HUMAN_ASSIGNMENT_ORDER_INVALID");
                if(previous!=null)Need(index==lastIndex?previous.Outcome==HumanTrialOutcome.ABORTED:Terminal(previous),"HUMAN_ASSIGNMENT_PREFIX_UNCONFIRMED");
                lastIndex=index;previous=trial;lastByIndex[index]=trial;
            }
            for(int i=0;i<run.NextAssignmentIndex;i++)Need(lastByIndex.TryGetValue(i,out var past)&&Terminal(past),"HUMAN_ASSIGNMENT_PREFIX_UNCONFIRMED");
            var seen=new HashSet<PublicPuzzleIdentity>();var used=new Dictionary<PublicPuzzleIdentity,int>();
            foreach(var trial in run.Trials)
            {
                var entry=assignments.Assignments.SingleOrDefault(a=>a.Id==trial.AssignmentId);Need(entry!=null&&entry.Puzzle.Identity.Equals(trial.Identity)&&entry.SourceSha256==trial.SourceSha256,"HUMAN_TRIAL_SOURCE_MISMATCH");
                Need(trial.Checkpoint.Restore(entry!.Puzzle.ToDomainDefinition(),trial.Identity.PuzzleId,trial.Identity.HashProfile,trial.Identity.HashSha256).Succeeded,"HUMAN_CHECKPOINT_INVALID");
                Need(!seen.Contains(trial.Identity)||trial.PriorExposure,"HUMAN_EXPOSURE_HISTORY_INVALID");seen.Add(trial.Identity);
                int count=used.TryGetValue(trial.Identity,out int old)?old:0;count+=trial.Hints.Count;Need(count<=3,"HUMAN_HINT_HISTORY_INVALID");used[trial.Identity]=count;
                foreach(var hint in trial.Hints)
                {Need(hint.X<entry.Puzzle.Width&&hint.Y<entry.Puzzle.Height&&hint.ExpectedRevision<=trial.Checkpoint.Revision,"HUMAN_HINT_BINDING_INVALID");Need(hint.State!=HumanHintState.HINT_DISPLAY_CONFIRMED||trial.Checkpoint.AppliedCommandIds.Contains(hint.DisplayId),"HUMAN_HINT_RECEIPT_MISSING");}
                Need(trial.Exposure!=HumanExposureState.EXPOSURE_CONFIRMED||trial.VisibleAtMs.HasValue||trial.Interrupted,"HUMAN_VISIBLE_TIME_MISSING");
            }
        }
        private static bool Terminal(HumanTrialRecord trial)=>trial.Outcome==HumanTrialOutcome.ABORTED||trial.Outcome==HumanTrialOutcome.SOLVED_UNREVIEWED;
        private static string WriteRun(HumanCalibrationRun run)=>O(("manifestBytesSha256",Q(run.ManifestBytesSha256)),("sessionToken",Q(run.SessionToken)),("origin",Q(run.Origin.ToString())),("createdAtUtc",Q(run.CreatedAtUtc)),("nextAssignmentIndex",N(run.NextAssignmentIndex)),("trials","["+string.Join(",",run.Trials.Select(WriteTrial))+"]"));
        private static string WriteTrial(HumanTrialRecord t)=>O(("assignmentId",Q(t.AssignmentId)),("identity",WriteIdentity(t.Identity)),("sourceSha256",Q(t.SourceSha256)),("exposureId",Q(t.ExposureId.ToString("D"))),("exposure",Q(t.Exposure.ToString())),("contact",t.Contact.HasValue?Q(t.Contact.Value.ToString()):"null"),("priorVersion",B(t.PriorVersion)),("priorExposure",B(t.PriorExposure)),("interrupted",B(t.Interrupted)),("clockEpoch",Q(t.ClockEpoch.ToString("D"))),("visibleAtMs",NullN(t.VisibleAtMs)),("previewActiveMs",NullN(t.PreviewActiveMs)),("timingComplete",B(t.TimingComplete)),("outcome",Q(t.Outcome.ToString())),("checkpoint",WriteCheckpoint(t.Checkpoint)),("hints","["+string.Join(",",t.Hints.Select(h=>O(("displayId",Q(h.DisplayId.ToString("D"))),("expectedRevision",N(h.ExpectedRevision)),("x",N(h.X)),("y",N(h.Y)),("ruleCode",Q(h.RuleCode)),("content",Q(h.Content.ToString())),("state",Q(h.State.ToString())))))+"]"));
        private static string WriteIdentity(PublicPuzzleIdentity identity)=>O(("puzzleId",Q(identity.PuzzleId)),("profile",Q(identity.HashProfile)),("sha256",Q(identity.HashSha256)));
        private static PublicPuzzleIdentity ReadIdentity(CanonicalDtoCursor c){var f=c.Fields("puzzleId","profile","sha256");return new PublicPuzzleIdentity(f[0].String(),f[1].String(),f[2].String());}
        private static HumanCalibrationRun ReadRun(CanonicalDtoCursor c){var f=c.Fields("manifestBytesSha256","sessionToken","origin","createdAtUtc","nextAssignmentIndex","trials");return new HumanCalibrationRun(f[0].String(),f[1].String(),f[2].EnumValue<HumanRunOrigin>(),f[3].String(),f[4].Int32(),f[5].Array().Select(ReadTrial).ToArray());}
        private static HumanTrialRecord ReadTrial(CanonicalDtoCursor c)
        {var f=c.Fields("assignmentId","identity","sourceSha256","exposureId","exposure","contact","priorVersion","priorExposure","interrupted","clockEpoch","visibleAtMs","previewActiveMs","timingComplete","outcome","checkpoint","hints");return new HumanTrialRecord(f[0].String(),ReadIdentity(f[1]),f[2].String(),f[3].Guid(),f[4].EnumValue<HumanExposureState>(),f[5].IsNull?(HumanFirstContact?)null:f[5].EnumValue<HumanFirstContact>(),f[6].Boolean(),f[7].Boolean(),f[8].Boolean(),f[9].Guid(),f[10].IsNull?(long?)null:f[10].Integer(),f[11].IsNull?(long?)null:f[11].Integer(),f[12].Boolean(),f[13].EnumValue<HumanTrialOutcome>(),ReadCheckpoint(f[14]),f[15].Array().Select(ReadHint).ToArray());}
        private static HumanHintObservation ReadHint(CanonicalDtoCursor c){var f=c.Fields("displayId","expectedRevision","x","y","ruleCode","content","state");return new HumanHintObservation(f[0].Guid(),f[1].Integer(),f[2].Int32(),f[3].Int32(),f[4].String(),f[5].EnumValue<CellContent>(),f[6].EnumValue<HumanHintState>());}
        // The unchanged Domain checkpoint fields, interpreted only against this exact assignment's public definition.
        private static string WriteCheckpoint(PuzzleSessionCheckpoint c)=>O(("identity",WriteIdentity(new PublicPuzzleIdentity(c.PuzzleId,c.PublicPuzzleHashProfile,c.PublicPuzzleHashSha256))),("rulesetVersion",Q(c.RulesetVersion)),("attemptId",Q(c.AttemptId.ToString("D"))),("mode",Q(c.Mode.ToString())),("phase",Q(c.Phase.ToString())),("cells","["+string.Join(",",c.Cells.Select(x=>Q(x.ToString())))+"]"),("previouslySet","["+string.Join(",",c.PreviouslySet.Select(B))+"]"),("revision",N(c.Revision)),("timerStarted",B(c.TimerStarted)),("activeElapsedMs",N(c.ActiveElapsedMs)),("usedEmptyMarker",B(c.UsedEmptyMarker)),("usedOccupiedMarker",B(c.UsedOccupiedMarker)),("hintCount",N(c.HintCount)),("correctionCount",N(c.CorrectionCount)),("appliedCommandIds","["+string.Join(",",c.AppliedCommandIds.Select(x=>Q(x.ToString("D"))))+"]"),("undo","["+string.Join(",",c.UndoStack.Select(d=>"["+string.Join(",",d.Changes.Select(x=>O(("x",N(x.X)),("y",N(x.Y)),("before",Q(x.Before.ToString())),("after",Q(x.After.ToString())))))+"]"))+"]"),("solvedEventId",Q(c.SolvedEventId)),("startedAtUtc",Q(c.StartedAtUtcText)),("lastSavedAtUtc",Q(c.LastSavedAtUtcText)));
        private static PuzzleSessionCheckpoint ReadCheckpoint(CanonicalDtoCursor c)
        {
            var f=c.Fields("identity","rulesetVersion","attemptId","mode","phase","cells","previouslySet","revision","timerStarted","activeElapsedMs","usedEmptyMarker","usedOccupiedMarker","hintCount","correctionCount","appliedCommandIds","undo","solvedEventId","startedAtUtc","lastSavedAtUtc");var id=ReadIdentity(f[0]);
            var undo=f[15].Array().Select(d=>new CheckpointCellDiff(d.Array().Select(x=>{var a=x.Fields("x","y","before","after");return new CheckpointCellChange(a[0].Int32(),a[1].Int32(),a[2].EnumValue<CellContent>(),a[3].EnumValue<CellContent>());}).ToArray())).ToArray();
            string? started=f[17].IsNull?null:f[17].String();string? saved=f[18].IsNull?null:f[18].String();
            return new PuzzleSessionCheckpoint(id.PuzzleId,id.HashProfile,id.HashSha256,f[1].String(),f[2].Guid(),f[3].EnumValue<SessionMode>(),f[4].EnumValue<SessionPhase>(),f[5].Array().Select(x=>x.EnumValue<CellContent>()).ToArray(),f[6].Array().Select(x=>x.Boolean()).ToArray(),f[7].Integer(),f[8].Boolean(),f[9].Integer(),f[10].Boolean(),f[11].Boolean(),f[12].Int32(),f[13].Integer(),f[14].Array().Select(x=>x.Guid()).ToArray(),undo,f[16].IsNull?null:f[16].String(),started==null?(DateTimeOffset?)null:DateTimeOffset.Parse(started,CultureInfo.InvariantCulture),saved==null?(DateTimeOffset?)null:DateTimeOffset.Parse(saved,CultureInfo.InvariantCulture)).WithDiagnosticText(started,saved);
        }
    }
}
