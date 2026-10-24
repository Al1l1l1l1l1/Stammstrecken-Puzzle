using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Editor.Build
{
    /// <summary>Source-approved full-game scene composition. Does not sign, submit, distribute or grant release/legal approval.</summary>
    public static class ProductionGameBuild
    {
        public const string ScenePath="Assets/StammstreckenPuzzle/Scenes/ProductionGame.unity";
        private const string Root="Assets/StammstreckenPuzzle/";
        private static readonly string[] PresetInputs={"STP_PRODUCTION_SERVICE_PROFILE","STP_PRODUCTION_SERVICE_PROFILE_RAW_SHA256",
            "STP_PRODUCTION_IAP_REVIEW_ORIGINAL","STP_PRODUCTION_IAP_REVIEW_RAW_SHA256","STP_PRODUCTION_IAP_POLICY_REVISION","STP_PRODUCTION_IAP_SDK_REVISION"};
        private static ProductionImportPreflightResult? preparedSource;
        private static ProductionMobileServiceProfile? preparedProfile;
        private static string? preparedDestination;
        private static string? preparedPresetJson;

        [MenuItem("Stammstrecken-Puzzle/Production/Rebuild source-approved scene")]
        public static void EnsureArtifacts()=>EnsureArtifactsCore(null);
        private static void EnsureArtifactsCore(Action<ProductionImportOutputFacts>? validateReleaseFacts)
        {
            string repository=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,".."));
            var inputs=ProductionImportPreflightInputs.FromEnvironment(repository);
            if(!string.IsNullOrWhiteSpace(inputs.SourceRoot)&&!SameRoot(inputs.SourceRoot,repository))
                throw new InvalidOperationException("PRODUCTION_SCENE_SOURCE_ROOT_MUST_MATCH_REPOSITORY");
            // Actual importer approval gate always precedes AssetDatabase, scene, settings or native provider mutation.
            var source=ProductionImportPreflight.RunAsync(inputs,CancellationToken.None).GetAwaiter().GetResult();
            source.VerifyStillCurrent();
            VerifySourceOriginals(source,repository);
            validateReleaseFacts?.Invoke(source.Facts);
            var profile=ReadProtectedPreset(repository);
            var publisher=ReadPublisherContacts();
            string presetJson=EditorJsonUtility.ToJson(profile);
            if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.Android&&EditorUserBuildSettings.activeBuildTarget!=BuildTarget.iOS)
                throw new InvalidOperationException("PRODUCTION_MOBILE_TARGET_REQUIRED");
            bool ios=EditorUserBuildSettings.activeBuildTarget==BuildTarget.iOS;
            var issues=profile.ConfigurationIssues(profile.ApplicationId,ios,RequiredEnvironment(PresetInputs[3]));
            if(issues.Count!=0)throw new InvalidOperationException(issues[0]);
            source.VerifyStillCurrent();
            string destination=Root+"Generated/ProductionRuntime/"+source.Facts.BindingsRawSha256+"/";
            foreach(string name in source.PlayerPaths)
            {
                string path=destination+name;byte[] bytes=source.GetPlayerBytes(name);
                if(File.Exists(path))
                {if(!File.ReadAllBytes(path).SequenceEqual(bytes))throw new InvalidOperationException("PRODUCTION_ASSET_IDENTITY_REUSED");}
            }
            foreach(string name in source.PlayerPaths)
            {
                string path=destination+name;if(File.Exists(path))continue;
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None);byte[] bytes=source.GetPlayerBytes(name);stream.Write(bytes,0,bytes.Length);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            var files=source.PlayerPaths.Where(p=>p!="Runtime.bindings.json").ToDictionary(p=>p,p=>Required<TextAsset>(destination+p),StringComparer.Ordinal);
            foreach(string name in source.PlayerPaths)
                if(!Required<TextAsset>(destination+name).bytes.SequenceEqual(source.GetPlayerBytes(name)))throw new InvalidOperationException("PRODUCTION_IMPORTED_ASSET_BYTES_CHANGED");
            ProductionAudioAssets.EnsureAssets();
            var prior=EditorSceneManager.GetActiveScene();
            if(prior.IsValid()&&prior.isDirty)throw new InvalidOperationException("PRODUCTION_UNSAVED_SCENE_MUST_BE_SAVED_FIRST");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var panel=Required<PanelSettings>(Root+"Settings/MobilePreviewPanelSettings.asset");
            var holder=new GameObject("Stammstrecken-Puzzle");holder.SetActive(false);
            var installer=holder.AddComponent<MobilePreviewInstaller>();
            installer.ConfigureProductionRuntime(Required<VisualTreeAsset>(Root+"UI/ProductionGame.uxml"),Required<StyleSheet>(Root+"UI/ProductionGame.uss"),
                Required<TextAsset>(Root+"UI/ProductionGameLocale.json"),panel,files,Required<TextAsset>(destination+"Runtime.bindings.json"),
                source.Facts.BindingsRawSha256,source.Facts.ReleaseLockRawSha256,source.Facts.ReleaseLockJcsSha256,source.Facts.ApprovalLockRawSha256!,source.Facts.UiSourceLockRawSha256);
            installer.ConfigureProductionServiceProfile(profile);
            installer.ConfigurePublisherContacts(publisher);
            installer.ConfigureAudio(ProductionAudioAssets.LoadMixer(),ProductionAudioAssets.LoadClips());holder.SetActive(true);
            PreservePreviousScene(source.AuditDirectory);
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("PRODUCTION_SCENE_SAVE_FAILED");
            AssetDatabase.SaveAssets();
            if(EditorJsonUtility.ToJson(profile)!=presetJson)throw new InvalidOperationException("PRODUCTION_PRESET_IN_MEMORY_CHANGED");
            VerifySavedScene(source,profile,destination);
            preparedSource=source;preparedProfile=profile;preparedDestination=destination;preparedPresetJson=presetJson;
            Debug.Log("STP source-approved full-game scene composed and saved-byte bindings verified. Native build, signing, devices, legal and store approval remain separate actual gates.");
        }

        internal static ProductionMobileServiceProfile PrepareForNativeBuild()=>PrepareForNativeBuildWithFacts(null);
        internal static ProductionMobileServiceProfile PrepareForNativeBuildWithFacts(Action<ProductionImportOutputFacts>? validateReleaseFacts)
        {preparedSource=null;preparedProfile=null;preparedDestination=null;preparedPresetJson=null;EnsureArtifactsCore(validateReleaseFacts);VerifyForNativeBuild(preparedProfile!);return preparedProfile!;}
        internal static void VerifyForNativeBuild(ProductionMobileServiceProfile expected)
        {
            if(preparedSource==null||preparedProfile==null||preparedDestination==null||expected!=preparedProfile)throw new InvalidOperationException("PRODUCTION_PREPARED_SCENE_REQUIRED");
            if(preparedPresetJson==null||EditorJsonUtility.ToJson(expected)!=preparedPresetJson)throw new InvalidOperationException("PRODUCTION_PRESET_IN_MEMORY_CHANGED");
            string repository=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,".."));
            if(!SameRoot(ProductionImportPreflightInputs.FromEnvironment(repository).SourceRoot,repository))throw new InvalidOperationException("PRODUCTION_SCENE_SOURCE_ROOT_MUST_MATCH_REPOSITORY");
            VerifySavedScene(preparedSource,preparedProfile,preparedDestination);
        }

        private static ProductionMobileServiceProfile ReadProtectedPreset(string repository,bool reload=true)
        {
            string[] missing=PresetInputs.Where(n=>string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(n))).ToArray();
            if(missing.Length!=0)throw new InvalidOperationException("PRODUCTION_MISSING_INPUTS:"+string.Join(",",missing));
            string asset=RequiredEnvironment(PresetInputs[0]);ProductionImportOutputVerifier.RequireRelativePath(asset);
            if(!asset.StartsWith(Root+"Settings/",StringComparison.Ordinal)||!asset.EndsWith(".asset",StringComparison.Ordinal))throw new InvalidOperationException("PRODUCTION_SERVICE_PRESET_PATH_INVALID");
            string expected=RequiredEnvironment(PresetInputs[1]);ProductionImportOutputVerifier.RequireHash(expected);
            string bound=Inside(repository,asset);
            if(ProductionImportOutputVerifier.RawHash(File.ReadAllBytes(bound))!=expected)throw new InvalidOperationException("PRODUCTION_SERVICE_PRESET_RAW_MISMATCH");
            string review=RequiredEnvironment(PresetInputs[2]);if(!Path.IsPathRooted(review))throw new InvalidOperationException("PRODUCTION_REVIEW_ABSOLUTE_PATH_REQUIRED");
            review=Path.GetFullPath(review);NoReparse(review);
            var info=new FileInfo(review);if(!info.Exists||info.Length<=0||info.Length>16777216)throw new InvalidOperationException("PRODUCTION_REVIEW_ORIGINAL_SIZE_INVALID");
            string reviewHash=RequiredEnvironment(PresetInputs[3]);ProductionImportOutputVerifier.RequireHash(reviewHash);
            if(ProductionImportOutputVerifier.RawHash(File.ReadAllBytes(review))!=reviewHash)throw new InvalidOperationException("PRODUCTION_REVIEW_ORIGINAL_RAW_MISMATCH");
            if(reload)AssetDatabase.ImportAsset(asset,ImportAssetOptions.ForceSynchronousImport|ImportAssetOptions.ForceUpdate);
            var profile=Required<ProductionMobileServiceProfile>(asset);
            if(ProductionImportOutputVerifier.RawHash(File.ReadAllBytes(bound))!=expected||profile.ReviewedEvidenceRawSha256!=reviewHash||
                profile.PolicyRevision!=RequiredEnvironment(PresetInputs[4])||profile.ReviewedSdkRevision!=RequiredEnvironment(PresetInputs[5]))
                throw new InvalidOperationException("PRODUCTION_REVIEW_PRESET_BINDING_MISMATCH");
            // The protected reviewer supplies these original bindings; this reader does not infer qualified legal approval from file existence/content.
            return profile;
        }

        private static void VerifySavedScene(ProductionImportPreflightResult source,ProductionMobileServiceProfile profile,string destination)
        {
            string text=File.ReadAllText(ScenePath),script=AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/MobilePreviewInstaller.cs");
            var fields=new[]{"tree","styles","locale","panel","publicData","campaignData","completionData","railwayAssets","cosmeticsData","campaignText","runtimeBindingsData","productionServiceProfile"};
            UnityEngine.Object[] assets={Required<VisualTreeAsset>(Root+"UI/ProductionGame.uxml"),Required<StyleSheet>(Root+"UI/ProductionGame.uss"),Required<TextAsset>(Root+"UI/ProductionGameLocale.json"),Required<PanelSettings>(Root+"Settings/MobilePreviewPanelSettings.asset"),
                Required<TextAsset>(destination+"Public.json"),Required<TextAsset>(destination+"Campaign.json"),Required<TextAsset>(destination+"CampaignCompletion.json"),Required<TextAsset>(destination+"RailwayAssets.json"),Required<TextAsset>(destination+"Cosmetics.json"),Required<TextAsset>(destination+"CampaignText.json"),Required<TextAsset>(destination+"Runtime.bindings.json"),profile};
            var guids=new string[assets.Length];var ids=new long[assets.Length];var types=Enumerable.Repeat(3,assets.Length).ToArray();types[3]=2;types[11]=2;
            for(int i=0;i<assets.Length;i++)if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assets[i],out guids[i],out ids[i]))throw new InvalidOperationException("PRODUCTION_ASSET_GUID_MISSING");
            MobilePreviewBuild.ValidateTypedSceneReferences(text,script,fields,guids,ids,types);
            var blocks=Regex.Split(text,@"(?m)^--- !u!").Where(b=>Regex.IsMatch(b,@"\A114 &-?\d+\r?\nMonoBehaviour:\r?\n")&&b.Contains("guid: "+script+", type: 3}")).ToArray();
            if(blocks.Length!=1)throw new InvalidOperationException("PRODUCTION_SCENE_INSTALLER_COUNT");
            string block=blocks[0];
            ValidatePublisherSceneBlock(block,ReadPublisherContacts());
            string[] names=source.PlayerPaths.Where(p=>p!="Runtime.bindings.json").OrderBy(p=>p,StringComparer.Ordinal).ToArray();
            string expectedPaths="  runtimePayloadPaths:\n"+string.Join("",names.Select(n=>"  - "+n+"\n"));
            string expectedAssets="  runtimePayloads:\n";
            foreach(string name in names)
            {if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Required<TextAsset>(destination+name),out string guid,out long id))throw new InvalidOperationException("PRODUCTION_ASSET_GUID_MISSING");expectedAssets+="  - {fileID: "+id.ToString(System.Globalization.CultureInfo.InvariantCulture)+", guid: "+guid+", type: 3}\n";}
            block=block.Replace("\r\n","\n");
            ValidatePayloadArrayBlocks(block,expectedPaths,expectedAssets);
            var hashes=new Dictionary<string,string>{{"runtimeBindingRawSha256",source.Facts.BindingsRawSha256},{"runtimeReleaseRawSha256",source.Facts.ReleaseLockRawSha256},{"runtimeReleaseJcsSha256",source.Facts.ReleaseLockJcsSha256},{"runtimeApprovalRawSha256",source.Facts.ApprovalLockRawSha256!},{"runtimeUiRawSha256",source.Facts.UiSourceLockRawSha256}};
            foreach(var pair in hashes)if(Regex.Matches(block,@"(?m)^  "+Regex.Escape(pair.Key)+": "+Regex.Escape(pair.Value)+@"$").Count!=1)throw new InvalidOperationException("PRODUCTION_SCENE_LOCK_MISMATCH");
            if(new[]{"candidateBindings","endlessProfilesData","endlessCompletionData","endlessBindingsData"}.Any(f=>!Regex.IsMatch(block,@"(?m)^  "+f+@": \{fileID: 0\}$")))throw new InvalidOperationException("PRODUCTION_SCENE_CANDIDATE_MIX_FORBIDDEN");
            foreach(string name in source.PlayerPaths)
                if(!Required<TextAsset>(destination+name).bytes.SequenceEqual(source.GetPlayerBytes(name))||
                    !File.ReadAllBytes(destination+name).SequenceEqual(source.GetPlayerBytes(name)))
                    throw new InvalidOperationException("PRODUCTION_SAVED_ASSET_BYTES_CHANGED");
            string repository=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,".."));
            if(ReadProtectedPreset(repository,false)!=profile)throw new InvalidOperationException("PRODUCTION_SAVED_PRESET_CHANGED");
            VerifySourceOriginals(source,repository);
            source.VerifyStillCurrent();
        }

        private static bool SameRoot(string first,string second)=>string.Equals(Path.GetFullPath(first).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar),Path.GetFullPath(second).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar),Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal);
        private static PublisherContactConfiguration ReadPublisherContacts()
            =>new PublisherContactConfiguration(RequiredEnvironment("STP_PUBLISHER_LEGAL_URL"),RequiredEnvironment("STP_PUBLISHER_SUPPORT_URL"));
        internal static void ValidatePublisherSceneBlock(string block,PublisherContactConfiguration expected)
        {
            block=block.Replace("\r\n","\n");
            foreach(var pair in new[]{("publisherLegalUrl",expected.LegalUrl),("publisherSupportUrl",expected.SupportUrl)})
                if(Regex.Matches(block,@"(?m)^  "+pair.Item1+@":").Count!=1||
                    Regex.Matches(block,@"(?m)^  "+pair.Item1+": "+Regex.Escape(pair.Item2)+@"$").Count!=1)
                    throw new InvalidOperationException("PRODUCTION_PUBLISHER_CONTACTS_CHANGED");
        }
        private static void VerifySourceOriginals(ProductionImportPreflightResult source,string repository)
        {
            foreach(var original in source.Facts.OriginalRawHashes)
                if(ProductionImportOutputVerifier.RawHash(File.ReadAllBytes(Inside(repository,original.Key)))!=original.Value)
                    throw new InvalidOperationException("PRODUCTION_ACTUAL_REPOSITORY_SOURCE_CHANGED");
        }

        internal static void ValidatePayloadArrayBlocks(string block,string expectedPaths,string expectedAssets)
        {
            block=block.Replace("\r\n","\n");
            foreach(var pair in new[]{("runtimePayloadPaths",expectedPaths),("runtimePayloads",expectedAssets)})
            {
                var matches=Regex.Matches(block,@"(?m)^  "+pair.Item1+@":\n(?:  - [^\n]*\n)*");
                if(Regex.Matches(block,@"(?m)^  "+pair.Item1+@":").Count!=1||matches.Count!=1||matches[0].Value!=pair.Item2)
                    throw new InvalidOperationException("PRODUCTION_SCENE_PAYLOAD_ARRAY_MISMATCH");
            }
        }

        private static string RequiredEnvironment(string name)=>Environment.GetEnvironmentVariable(name)??throw new InvalidOperationException("PRODUCTION_MISSING_INPUT:"+name);
        private static string Inside(string root,string relative)
        {string path=Path.GetFullPath(Path.Combine(root,relative.Replace('/',Path.DirectorySeparatorChar)));if(!path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal))throw new InvalidOperationException("PRODUCTION_PATH_ESCAPE");NoReparse(path);return path;}
        private static void NoReparse(string path)
        {for(string? part=path;part!=null;part=Path.GetDirectoryName(part))if(File.Exists(part)||Directory.Exists(part))if((File.GetAttributes(part)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("PRODUCTION_REPARSE_FORBIDDEN");}
        private static void PreservePreviousScene(string audit)
        {if(!File.Exists(ScenePath))return;string destination=Path.Combine(audit,"previous-production-scene.unity");File.Copy(ScenePath,destination,false);if(File.Exists(ScenePath+".meta"))File.Copy(ScenePath+".meta",destination+".meta",false);}
        private static T Required<T>(string path)where T:UnityEngine.Object=>AssetDatabase.LoadAssetAtPath<T>(path)??throw new InvalidOperationException("PRODUCTION_ASSET_MISSING:"+path);
    }
}
