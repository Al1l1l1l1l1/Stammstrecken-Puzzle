using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;
using STP.Tools.HumanReadback;

namespace STP.Tools.ProductionImport;

internal static partial class ProvenanceGate
{
    private sealed class OriginalSet
    {
        internal string Id="";internal string Prefix="";internal string Packs="";internal SourceBundle Bundle=null!;internal Readback Readback=null!;
        internal JsonValue SourceBindings=null!;internal readonly HashSet<string> Included=new(StringComparer.Ordinal);
    }
    static partial void ValidateProduction(Sources sources,JsonValue request,IReadOnlyList<LevelValidationResult> levels,IReadOnlyList<JsonValue> profiles,StarCalibrationCatalog calibration,RuntimePublicPuzzleCatalog corpus,ReleaseLock release,IReadOnlyList<JsonValue> evidence,string uiHash,string approvalPath,string trustedApprovalHash,string targetTextSourceRawHash)
    {
        var approved=ReadTrustedApproval(sources,approvalPath,trustedApprovalHash);
        J.Fields(approved,"approvalLockVersion","kind","use","releaseLock","evidence","sources","acceptedBy","decisionRef","acceptance");
        J.Need(J.N(approved,"approvalLockVersion")==1&&J.S(approved,"kind")=="PRODUCTION_SOURCE_APPROVAL_LOCK"&&J.S(approved,"use")=="Production","IMPORT_APPROVAL_SOURCE_KIND");
        J.Need(J.S(J.Get(approved,"releaseLock"),"path")==J.S(request,"releaseLock")&&J.Raw(sources.Reference(J.Get(approved,"releaseLock")))==release.RawHash,"IMPORT_APPROVAL_RELEASE_BINDING");
        string acceptedBy=J.S(approved,"acceptedBy"),decision=J.S(approved,"decisionRef");sources.Reference(J.Get(approved,"acceptance"));
        var evidenceRefs=J.A(approved,"evidence");J.Need(evidence.Count>0&&evidenceRefs.Count==evidence.Count&&evidence.Select(J.Text).OrderBy(x=>x,StringComparer.Ordinal).SequenceEqual(evidenceRefs.Select(e=>J.S(e,"path")).OrderBy(x=>x,StringComparer.Ordinal)),"IMPORT_APPROVAL_EVIDENCE_MEMBERSHIP");
        var versions=new HashSet<string>(StringComparer.Ordinal);foreach(var evidenceRef in evidenceRefs)
        {
            byte[] bytes=sources.Reference(evidenceRef);var e=J.Parse(bytes);J.Fields(e,"evidenceVersion","kind","use","calibrationVersion","uiSourceSha256","method","observations","exclusions","logicClasses","campaignTransfers","profileTransfers","acceptance");
            string version=J.S(e,"calibrationVersion"),measuredUiHash=J.H(e,"uiSourceSha256");J.Need(J.N(e,"evidenceVersion")==1&&J.S(e,"kind")=="STAR_CALIBRATION_EVIDENCE"&&J.S(e,"use")=="Production"&&versions.Add(version),"IMPORT_PRODUCTION_EVIDENCE_KIND_VERSION_UI");
            var provenance=calibration.Provenance.SingleOrDefault(p=>p.CalibrationVersion==version);J.Need(provenance!=null&&provenance.EvidenceSha256==J.Hash(e)&&provenance.UiSourceSha256==measuredUiHash,"IMPORT_EVIDENCE_JCS_BINDING");
            J.Need(calibration.Campaign.All(x=>x.Times.CalibrationVersion==version)&&calibration.EndlessEntries.All(x=>x.Times.CalibrationVersion==version),"IMPORT_EVIDENCE_TARGET_VERSION_MISMATCH");
            var method=J.Get(e,"method");J.Fields(method,"source","version");J.S(method,"version");sources.Reference(J.Get(method,"source"));
            var acceptance=J.Get(e,"acceptance");J.Fields(acceptance,"source","acceptedBy","decisionRef");J.Need(J.S(acceptance,"acceptedBy")==acceptedBy&&J.S(acceptance,"decisionRef")==decision,"IMPORT_EVIDENCE_ACCEPTANCE_CONTEXT");sources.Reference(J.Get(acceptance,"source"));
            var classes=new HashSet<string>(StringComparer.Ordinal);foreach(var c in J.A(e,"logicClasses")){J.Fields(c,"id","criteria","transferMethod");J.Need(classes.Add(J.S(c,"id")),"IMPORT_LOGIC_CLASS_DUPLICATE");sources.Reference(J.Get(c,"criteria"));sources.Reference(J.Get(c,"transferMethod"));}J.Need(classes.Count>0,"IMPORT_LOGIC_CLASSES_MISSING");
            var sets=ReadOriginalSets(sources,e,measuredUiHash);foreach(var set in sets)UiTransfer.Validate(sources,J.S(request,"uiSourceLock"),set.Prefix+"/"+set.Packs+"/ui-source-lock.json",set.Prefix,measuredUiHash,targetTextSourceRawHash);var observations=sets.SelectMany(s=>s.Readback.Rows.Select(row=>(Set:s,Row:row))).ToArray();
            foreach(var group in observations.GroupBy(x=>x.Row.ExposureId))J.Need(group.Count()==1,"IMPORT_OBSERVATION_EXPOSURE_REUSED");foreach(var group in observations.GroupBy(x=>x.Row.AttemptId))J.Need(group.Count()==1,"IMPORT_OBSERVATION_ATTEMPT_REUSED");foreach(var group in sets.SelectMany(s=>s.Readback.Inputs).GroupBy(x=>x.Sha256))J.Need(group.Count()==1,"IMPORT_EXPORT_DUPLICATE_ACROSS_SETS");
            foreach(var group in sets.SelectMany(s=>s.Readback.Inputs).GroupBy(x=>x.SessionToken))J.Need(group.Key!=null&&group.Count()==1,"IMPORT_SESSION_MULTIPLE_EXPORTS_ACROSS_SETS");
            var included=observations.Where(x=>x.Set.Included.Contains(x.Row.ExposureId.ToString("D"))).ToArray();J.Need(included.Length>0&&included.All(x=>x.Row.Reasons.Count==0&&x.Row.UiSourceSha256==measuredUiHash),"IMPORT_NONHUMAN_OR_UNCERTAIN_INCLUDED_OBSERVATION");
            // Readback stays unattested. Only the independently trusted acceptance original above can adopt the
            // exact measurements. A UI-origin string, candidate classification or caller DTO never grants approval.
            var exclusions=new HashSet<string>(StringComparer.Ordinal);foreach(var exclusion in J.A(e,"exclusions")){J.Fields(exclusion,"observationId","exposureId","reason");J.S(exclusion,"reason");J.Need(exclusions.Add(J.S(exclusion,"observationId")+":"+J.S(exclusion,"exposureId")),"IMPORT_EXCLUSION_DUPLICATE");}
            foreach(var x in observations){string key=x.Set.Id+":"+x.Row.ExposureId.ToString("D");bool a=x.Set.Included.Contains(x.Row.ExposureId.ToString("D")),b=exclusions.Remove(key);J.Need(a!=b,"IMPORT_OBSERVATION_NOT_EXACTLY_ACCOUNTED");}J.Need(exclusions.Count==0,"IMPORT_EXCLUSION_UNKNOWN_TRIAL");
            var usedClasses=new HashSet<string>(StringComparer.Ordinal);
            (OriginalSet Set,Observation Row)[] References(JsonValue transfer)
            {
                string cls=J.S(transfer,"logicClassId");J.Need(classes.Contains(cls),"IMPORT_LOGIC_CLASS_UNRESOLVED");usedClasses.Add(cls);var found=new List<(OriginalSet Set,Observation Row)>();var keys=new HashSet<string>(StringComparer.Ordinal);
                foreach(var r in J.A(transfer,"observationIds")){J.Fields(r,"observationId","exposureId");string sid=J.S(r,"observationId"),eid=J.S(r,"exposureId");var exact=included.Where(x=>x.Set.Id==sid&&x.Row.ExposureId.ToString("D")==eid).ToArray();J.Need(exact.Length==1&&keys.Add(sid+":"+eid),"IMPORT_TRANSFER_OBSERVATION_UNRESOLVED");found.Add(exact[0]);}J.Need(found.Count>0,"IMPORT_TRANSFER_UNSUPPORTED_BY_OBSERVATION");return found.ToArray();
            }
            var transfers=J.A(e,"campaignTransfers");J.Need(transfers.Count==240,"IMPORT_TRANSFER_FULL_240_REQUIRED");var campaignIds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var t in transfers)
            {
                J.Fields(t,"puzzleId","sourceDocument","targetDocumentSha256","publicPuzzleHash","logicClassId","observationIds");string id=J.S(t,"puzzleId");var target=levels.SingleOrDefault(l=>l.Level.Document!.PuzzleId==id);J.Need(target!=null&&campaignIds.Add(id),"IMPORT_CAMPAIGN_TRANSFER_MEMBERSHIP");
                var old=LevelV2Loader.Load(sources.Reference(J.Get(t,"sourceDocument")));J.Need(old.IsValid&&old.Document!.PuzzleId==id&&old.PublicPuzzleHash!.Equals(target!.Level.PublicPuzzleHash)&&J.Hash(J.Get(t,"publicPuzzleHash"))==J.Hash(target.Level.PublicPuzzleHash!.ToJson())&&J.H(t,"targetDocumentSha256")==target.Level.ContentHash&&target.Level.Document!.Production.StarThresholdsSeconds!.CalibrationVersion==version,"IMPORT_SOURCE_TARGET_DOCUMENT_GEOMETRY_BINDING");
                var references=References(t);J.Need(references.All(x=>x.Row.ProfileId==null),"IMPORT_CAMPAIGN_TRANSFER_USES_ENDLESS_SAMPLE");
                string sourceRaw=J.H(J.Get(t,"sourceDocument"),"rawSha256");bool direct=references.Any(x=>x.Row.PuzzleId==id&&x.Row.SemanticSha256==target!.Level.PublicPuzzleHash!.Sha256&&x.Row.SourceSha256==sourceRaw);
                sources.HumanSourceBindings.Add(J.Obj(("kind",J.Str("CAMPAIGN_SOURCE_TO_TARGET")),("puzzleId",J.Str(id)),("calibrationVersion",J.Str(version)),("evidenceJcsSha256",J.Str(J.Hash(e))),("evidenceRawSha256",J.Str(J.Raw(bytes))),("measuredUiSourceLockRawSha256",J.Str(measuredUiHash)),("targetUiSourceLockRawSha256",J.Str(uiHash)),("sourceDocument",J.Get(t,"sourceDocument")),("sourceDocumentJcsSha256",J.Str(old.ContentHash!)),("targetDocumentJcsSha256",J.Str(target!.Level.ContentHash!)),("directlyObserved",JsonValue.FromBoolean(direct)),("originalObservations",ObservationRefs(references))));
            }
            CheckRequiredCampaignSample(sources,sets,included,corpus);
            var profileTransfers=J.A(e,"profileTransfers");J.Need(profileTransfers.Count==4,"IMPORT_FOUR_PROFILE_TRANSFERS_REQUIRED");var profileIds=new HashSet<string>(StringComparer.Ordinal);
            byte[] completionBytes=sources.Read(J.S(request,"endlessCompletion"));var wrapper=J.Parse(completionBytes);var completionSource=J.Get(wrapper,"completionCatalog");var completion=new RuntimeCompletionCatalog(completionBytes,corpus,new CatalogIdentity(J.S(completionSource,"catalogId"),checked((int)J.N(completionSource,"catalogRevision")),J.Hash(wrapper),CatalogUse.Production,CatalogApprovalStatus.ProductApproved),new CanonicalJsonAdapter());
            foreach(var t in profileTransfers)
            {
                J.Fields(t,"profileId","sourceProfiles","sourcePublicCatalog","sourcePublicBindings","targetConditionsSha256","logicClassId","observationIds");string id=J.S(t,"profileId");J.Need(profileIds.Add(id),"IMPORT_PROFILE_TRANSFER_DUPLICATE");var target=profiles.Single(v=>J.S(v,"profileId")==id);
                var oldBindings=J.Parse(sources.Reference(J.Get(t,"sourcePublicBindings")));var oldCorpus=new MobileCandidateData(sources.Reference(J.Get(t,"sourcePublicCatalog")),J.H(oldBindings,"publicCatalogHashSha256"));J.Need(oldCorpus.Puzzles.Select(p=>p.Identity).SequenceEqual(corpus.Puzzles.Select(p=>p.Identity)),"IMPORT_PROFILE_CORPUS_GEOMETRY_CHANGED");
                byte[] oldProfileBytes=sources.Reference(J.Get(t,"sourceProfiles"));var oldProfile=new LockedEndlessProfiles(oldProfileBytes,oldCorpus).Get(id);var oldRaw=J.A(J.Parse(oldProfileBytes),"profiles").Single(p=>J.S(p,"profileId")==id);
                J.Need(J.Hash(ProfileConditions(target))==J.H(t,"targetConditionsSha256")&&J.S(target,"rulesetVersion")=="train-track-v1"&&J.S(oldRaw,"generatorVersion")==J.S(target,"generatorVersion")&&J.S(oldRaw,"solverVersion")==J.S(target,"solverVersion")&&J.S(oldRaw,"noveltyVersion")==J.S(J.Get(target,"novelty"),"algorithmVersion"),"IMPORT_PROFILE_CONDITION_VERSION_BINDING");
                var h=J.Get(target,"humanReview");J.Need(J.S(h,"calibrationVersion")==version&&J.H(h,"uiSourceSha256")==measuredUiHash&&J.H(h,"evidenceSha256")==J.Hash(e)&&J.S(target,"approvedBy")==acceptedBy&&J.S(target,"approvedDecisionRef")==decision,"IMPORT_PROFILE_HUMAN_ACCEPTANCE_BINDING");
                var locked=new LockedCalibratedEndlessProfile(J.Bytes(target),corpus,completion,J.Hash(target)).Profile;J.Need(EndlessProjection.SearchFields(oldProfile.Search)==EndlessProjection.SearchFields(locked.Search),"IMPORT_ORIGINAL_SEARCH_PARAMETERS_CHANGED");
                var references=References(t);J.Need(references.Length>=12&&references.Select(x=>x.Row.PuzzleId).Distinct().Count()>=12&&J.N(h,"sampleCount")==references.Length&&references.All(x=>x.Row.ProfileId==id&&x.Row.ProfileHashSha256==oldProfile.HashSha256),"IMPORT_PROFILE_REQUIRED_ACTUAL_CASES");
                foreach(var x in references)
                {
                    var a=x.Set.Bundle.Catalogs[x.Row.ManifestSha256].Assignments.Single(a=>a.Id==x.Row.AssignmentId);var pz=a.Puzzle;var proof=PuzzleSolver.Solve(pz.ToDomainDefinition(),new SolverLimits(1000000,60000));int trivial=pz.RowCounts.Count(n=>n<=1||n==pz.Width)+pz.ColumnCounts.Count(n=>n<=1||n==pz.Height);long occupied=pz.RowCounts.Sum();var g=J.Get(target,"gridRanges");
                    J.Need(proof.Classification==SolverClassification.UNIQUE&&proof.Metrics!=null&&locked.Quality.Accepts(proof.Metrics,trivial)&&pz.Width==pz.Height&&pz.Width>=J.N(g,"minSize")&&pz.Width<=J.N(g,"maxSize")&&occupied*100>=J.N(g,"minDensityPercent")*pz.Width*pz.Height&&occupied*100<=J.N(g,"maxDensityPercent")*pz.Width*pz.Height,"IMPORT_OBSERVED_PROFILE_OUTSIDE_TARGET_QUALITY");
                }
                sources.HumanSourceBindings.Add(J.Obj(("kind",J.Str("PROFILE_SOURCE_TO_TARGET")),("profileId",J.Str(id)),("calibrationVersion",J.Str(version)),("evidenceJcsSha256",J.Str(J.Hash(e))),("evidenceRawSha256",J.Str(J.Raw(bytes))),("measuredUiSourceLockRawSha256",J.Str(measuredUiHash)),("targetUiSourceLockRawSha256",J.Str(uiHash)),("sourceProfileHashSha256",J.Str(oldProfile.HashSha256)),("targetProfileJcsSha256",J.Str(J.Hash(target))),("targetConditionsJcsSha256",J.Str(J.Hash(ProfileConditions(target)))),("originalObservations",ObservationRefs(references))));
            }
            J.Need(usedClasses.SetEquals(classes),"IMPORT_UNUSED_LOGIC_CLASS");
        }
        J.Need(versions.SetEquals(calibration.Provenance.Select(p=>p.CalibrationVersion)),"IMPORT_EVIDENCE_VERSION_MEMBERSHIP");
        var sourceRows=J.A(approved,"sources");var actual=sources.Originals.Where(x=>x.Key!=approvalPath).ToArray();J.Need(sourceRows.Count==actual.Length&&sourceRows.Select(x=>J.S(x,"path")).SequenceEqual(actual.Select(x=>x.Key)),"IMPORT_APPROVAL_EXACT_SOURCE_FILESET");for(int i=0;i<sourceRows.Count;i++){J.Fields(sourceRows[i],"path","rawSha256");J.Need(J.H(sourceRows[i],"rawSha256")==J.Raw(actual[i].Value),"IMPORT_APPROVAL_SOURCE_RAW_MISMATCH");}
    }
    internal static JsonValue ProfileConditions(JsonValue p)=>J.Obj(new[]{"rulesetVersion","difficultyBand","generatorVersion","solverVersion","gridRanges","metricRanges","runtimeBudgets","novelty"}.Select(k=>(k,J.Get(p,k))).ToArray());
    private static JsonValue ObservationRefs(IEnumerable<(OriginalSet Set,Observation Row)> rows)=>J.Arr(rows.Select(x=>J.Obj(("observationId",J.Str(x.Set.Id)),("exposureId",J.Str(x.Row.ExposureId.ToString("D"))),("exportRawSha256",J.Str(x.Row.ExportSha256)),("manifestRawSha256",J.Str(x.Row.ManifestSha256)),("assignmentId",J.Str(x.Row.AssignmentId)),("sourceOriginalRawSha256",J.Str(x.Row.SourceSha256)),("publicSemanticSha256",J.Str(x.Row.SemanticSha256)))));
    internal static JsonValue ReadTrustedApproval(Sources sources,string path,string expectedRawHash)
    {
        byte[] bytes=sources.Read(path);J.Need(J.Raw(bytes)==expectedRawHash,"IMPORT_TRUSTED_APPROVAL_RAW_MISMATCH");var v=J.Parse(bytes);
        J.Fields(v,"approvalLockVersion","kind","use","releaseLock","evidence","sources","acceptedBy","decisionRef","acceptance");J.Need(J.N(v,"approvalLockVersion")==1&&J.S(v,"kind")=="PRODUCTION_SOURCE_APPROVAL_LOCK"&&J.S(v,"use")=="Production","IMPORT_APPROVAL_SOURCE_KIND");return v;
    }
}
