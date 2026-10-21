using System;
using System.Text.RegularExpressions;
using STP.Infrastructure.Content;

namespace STP.Bootstrap
{
    /// <summary>Strict private build-job metadata, never source approval or release promotion authority.</summary>
    public sealed class ReleaseBuildIdentity
    {
        private ReleaseBuildIdentity(string version,string tag,int ordinal,string commit,string workflow,string platform,int number,string id)
        {ReleaseVersion=version;CandidateTag=tag;CandidateOrdinal=ordinal;GitCommit=commit;Workflow=workflow;Platform=platform;BuildNumber=number;ApplicationIdentifier=id;}
        public string ReleaseVersion{get;}public string CandidateTag{get;}public int CandidateOrdinal{get;}public string GitCommit{get;}
        public string Workflow{get;}public string Platform{get;}public int BuildNumber{get;}public string ApplicationIdentifier{get;}
        public static ReleaseBuildIdentity Read(byte[] bytes,string expectedRawSha256,string platform,long ciRunNumber,int ciAttempt)
        {
            Need(bytes!=null&&bytes.Length>0&&bytes.Length<=8192,"RC_IDENTITY_SIZE_INVALID");
            ProductionImportOutputVerifier.RequireHash(expectedRawSha256);
            Need(ProductionImportOutputVerifier.RawHash(bytes!)==expectedRawSha256,"RC_IDENTITY_RAW_MISMATCH");
            var parsed=StrictJsonParser.Parse(bytes!,new JsonParserLimits(8192,4,1024,32,16));
            Need(parsed.Succeeded,"RC_IDENTITY_JSON_INVALID");var root=parsed.Value!;
            string[] keys={"releaseVersion","candidateTag","candidateOrdinal","gitCommit","workflow","platform","buildNumber","buildProfile","applicationIdentifier"};
            Need(root.Kind==JsonKind.Object&&root.Members.Count==keys.Length,"RC_IDENTITY_FIELDS_INVALID");
            foreach(var field in root.Members)Need(Array.IndexOf(keys,field.Name)>=0,"RC_IDENTITY_FIELDS_INVALID");
            string Text(string key){var value=root.Get(key);Need(value!=null&&value.Kind==JsonKind.String,"RC_IDENTITY_STRING_INVALID");return value!.StringValue;}
            string version=Text("releaseVersion"),tag=Text("candidateTag"),commit=Text("gitCommit"),workflow=Text("workflow"),number=Text("buildNumber"),id=Text("applicationIdentifier");
            var ordinal=root.Get("candidateOrdinal");Need(ordinal!=null&&ordinal.Kind==JsonKind.Integer&&ordinal.IntegerValue>0&&ordinal.IntegerValue<=int.MaxValue,"RC_ORDINAL_INVALID");
            Need(Regex.IsMatch(version,@"\A(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\z")&&tag=="v"+version+"-rc."+ordinal!.IntegerValue.ToString(System.Globalization.CultureInfo.InvariantCulture),"RC_TAG_VERSION_MISMATCH");
            Need(Regex.IsMatch(commit,@"\A[0-9a-f]{40}\z")&&Regex.IsMatch(workflow,@"\A[A-Za-z0-9_-]+\.ya?ml@[0-9a-f]{40}\z"),"RC_SOURCE_IDENTITY_INVALID");
            Need(Text("buildProfile")=="production"&&Text("platform")==platform&&(platform=="ANDROID"||platform=="IOS"),"RC_PROFILE_PLATFORM_INVALID");
            Need(Regex.IsMatch(id,@"\A[A-Za-z][A-Za-z0-9]*(?:\.[A-Za-z][A-Za-z0-9]*)+\z")&&!Regex.IsMatch(id,@"\.(?:qa|dev|human|staging|test)\z",RegexOptions.IgnoreCase),"RC_APPLICATION_ID_INVALID");
            Need(Regex.IsMatch(number,@"\A[1-9][0-9]*\z")&&long.TryParse(number,out long build)&&build<=2100000000,"RC_BUILD_NUMBER_INVALID");
            Need(ciRunNumber>0&&ciRunNumber<=2100000&&ciAttempt>=1&&ciAttempt<=999,"RC_CI_NUMBER_INVALID");
            long derived=checked(ciRunNumber*1000+ciAttempt);Need(derived<=2100000000&&derived.ToString(System.Globalization.CultureInfo.InvariantCulture)==number,"RC_CI_NUMBER_MISMATCH");
            return new ReleaseBuildIdentity(version,tag,(int)ordinal!.IntegerValue,commit,workflow,platform,(int)derived,id);
        }
        public static void ValidateRetainedBuildInfo(byte[] bytes)=>BuildInfoShape(bytes);
        public static void ValidateBuildInfo(byte[] bytes,string expectedRawSha256,ReleaseBuildIdentity identity,string unityVersion,string packageRawSha256,string? contentJcsSha256,string workflowRunId)
        {
            ProductionImportOutputVerifier.RequireHash(expectedRawSha256);Need(ProductionImportOutputVerifier.RawHash(bytes)==expectedRawSha256,"RC_BUILD_INFO_RAW_MISMATCH");var root=BuildInfoShape(bytes);
            string Text(string key)=>root.Get(key)!.StringValue;
            Need(Text("releaseVersion")==identity.ReleaseVersion&&Text("buildNumber")==identity.BuildNumber.ToString(System.Globalization.CultureInfo.InvariantCulture)&&Text("gitCommit")==identity.GitCommit&&Text("candidateTag")==identity.CandidateTag&&Text("workflow")==identity.Workflow&&Text("workflowRunId")==workflowRunId,"RC_BUILD_INFO_IDENTITY_MISMATCH");
            Need(Text("unityVersion")==unityVersion&&Text("packageLockSha256")==packageRawSha256&&(contentJcsSha256==null||Text("contentLockSha256")==contentJcsSha256),"RC_BUILD_INFO_LOCK_MISMATCH");
            // The production composition uses ForMetagame (write3), existing level-v2 and Content ProofV1 production solver-v2 contract.
            Need(root.Get("saveSchemaVersion")!.IntegerValue==3&&root.Get("levelSchemaVersion")!.IntegerValue==LevelV2Contract.DocumentSchemaVersion&&Text("solverVersion")=="solver-v2","RC_BUILD_INFO_CONTRACT_VERSION_MISMATCH");
        }
        private static JsonValue BuildInfoShape(byte[] bytes)
        {
            var parsed=StrictJsonParser.Parse(bytes,new JsonParserLimits(8192,4,1024,32,16));Need(parsed.Succeeded,"RC_BUILD_INFO_JSON_INVALID");var root=parsed.Value!;
            string[] keys={"buildInfoVersion","releaseVersion","buildNumber","gitCommit","candidateTag","builtAtUtc","unityVersion","packageLockSha256","contentLockSha256","saveSchemaVersion","levelSchemaVersion","solverVersion","workflow","workflowRunId"};
            Need(root.Kind==JsonKind.Object&&root.Members.Count==keys.Length,"RC_BUILD_INFO_FIELDS_INVALID");foreach(var member in root.Members)Need(Array.IndexOf(keys,member.Name)>=0,"RC_BUILD_INFO_FIELDS_INVALID");
            foreach(string key in new[]{"buildInfoVersion","saveSchemaVersion","levelSchemaVersion"})Need(root.Get(key)!.Kind==JsonKind.Integer&&root.Get(key)!.IntegerValue>0,"RC_BUILD_INFO_INTEGER_INVALID");Need(root.Get("buildInfoVersion")!.IntegerValue==1,"RC_BUILD_INFO_VERSION_INVALID");
            foreach(string key in keys)if(key!="buildInfoVersion"&&key!="saveSchemaVersion"&&key!="levelSchemaVersion")Need(root.Get(key)!.Kind==JsonKind.String,"RC_BUILD_INFO_STRING_INVALID");
            string T(string key)=>root.Get(key)!.StringValue;
            Need(Regex.IsMatch(T("releaseVersion"),@"\A(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\z")&&Regex.IsMatch(T("candidateTag"),@"\Av"+Regex.Escape(T("releaseVersion"))+@"-rc\.[1-9][0-9]*\z"),"RC_BUILD_INFO_TAG_INVALID");
            Need(Regex.IsMatch(T("buildNumber"),@"\A[1-9][0-9]*\z")&&Regex.IsMatch(T("gitCommit"),@"\A[0-9a-f]{40}\z")&&Regex.IsMatch(T("workflow"),@"\A[A-Za-z0-9_-]+\.ya?ml@[0-9a-f]{40}\z")&&Regex.IsMatch(T("workflowRunId"),@"\A[1-9][0-9]*\z"),"RC_BUILD_INFO_SOURCE_INVALID");
            Need(DateTime.TryParseExact(T("builtAtUtc"),"yyyy-MM-dd'T'HH:mm:ss'Z'",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.AssumeUniversal|System.Globalization.DateTimeStyles.AdjustToUniversal,out _),"RC_BUILD_INFO_UTC_INVALID");
            Need(Regex.IsMatch(T("unityVersion"),@"\A[0-9]+\.[0-9]+\.[0-9]+[abfp][0-9]+\z")&&Regex.IsMatch(T("solverVersion"),@"\Asolver-v[1-9][0-9]*\z"),"RC_BUILD_INFO_TOOL_VERSION_INVALID");
            ProductionImportOutputVerifier.RequireHash(T("packageLockSha256"));ProductionImportOutputVerifier.RequireHash(T("contentLockSha256"));return root;
        }
        private static void Need(bool ok,string code){if(!ok)throw new ArgumentException(code);}
    }
}
