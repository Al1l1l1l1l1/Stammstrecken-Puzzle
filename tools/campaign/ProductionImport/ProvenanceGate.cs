using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;
using STP.Infrastructure.Content;

namespace STP.Tools.ProductionImport;

internal static partial class ProvenanceGate
{
    internal static void Validate(Sources sources,JsonValue request,IReadOnlyList<LevelValidationResult> levels,IReadOnlyList<JsonValue> profiles,StarCalibrationCatalog calibration,RuntimePublicPuzzleCatalog corpus,ReleaseLock release,IReadOnlyList<JsonValue> evidence,bool fixture,string? approvalPath,string? trustedApprovalHash,string targetTextSourceRawHash)
    {
        byte[] uiBytes=sources.Read(J.S(request,"uiSourceLock"));string uiHash=J.Raw(uiBytes);var ui=J.Parse(uiBytes);J.Fields(ui,"schemaVersion","kind","files");J.Need(J.N(ui,"schemaVersion")==1&&J.S(ui,"kind")=="HUMAN_UI_SOURCE_LOCK","IMPORT_UI_SOURCE_LOCK");
        var uiPaths=new List<string>();foreach(var file in J.A(ui,"files")){J.Fields(file,"path","sha256");string path=J.S(file,"path");uiPaths.Add(path);J.Need(J.Raw(sources.Read(path))==J.H(file,"sha256"),"IMPORT_UI_SOURCE_BYTES:"+path);}J.Need(uiPaths.SequenceEqual(uiPaths.OrderBy(p=>p,StringComparer.Ordinal))&&uiPaths.Distinct().Count()==uiPaths.Count,"IMPORT_UI_LOCK_FILESET_ORDER");
        if(fixture)
        {
            J.Need(approvalPath==null&&trustedApprovalHash==null&&evidence.Count>0,"IMPORT_FIXTURE_CANNOT_USE_PRODUCT_APPROVAL");
            var found=new HashSet<string>(StringComparer.Ordinal);foreach(var path in evidence){byte[] bytes=sources.Read(J.Text(path));var e=J.Parse(bytes);J.Fields(e,"fixtureEvidenceVersion","kind","calibrationVersion","uiSourceSha256");J.Need(J.N(e,"fixtureEvidenceVersion")==1&&J.S(e,"kind")=="SYNTHETIC_CONTRACT_TEST_NO_HUMAN_EVIDENCE"&&J.H(e,"uiSourceSha256")==uiHash&&found.Add(J.S(e,"calibrationVersion")),"IMPORT_FIXTURE_EVIDENCE_INVALID");var p=calibration.Provenance.SingleOrDefault(p=>p.CalibrationVersion==J.S(e,"calibrationVersion"));J.Need(p!=null&&p.EvidenceSha256==J.Raw(bytes)&&p.UiSourceSha256==uiHash,"IMPORT_FIXTURE_EVIDENCE_BINDING");}
            J.Need(found.Count==calibration.Provenance.Count,"IMPORT_PROVENANCE_MEMBERSHIP");return;
        }
        UiTransfer.CheckTargetFileSet(sources,ui);
        ValidateProduction(sources,request,levels,profiles,calibration,corpus,release,evidence,uiHash,approvalPath!,trustedApprovalHash!,targetTextSourceRawHash);
    }
    static partial void ValidateProduction(Sources sources,JsonValue request,IReadOnlyList<LevelValidationResult> levels,IReadOnlyList<JsonValue> profiles,StarCalibrationCatalog calibration,RuntimePublicPuzzleCatalog corpus,ReleaseLock release,IReadOnlyList<JsonValue> evidence,string uiHash,string approvalPath,string trustedApprovalHash,string targetTextSourceRawHash);
}
