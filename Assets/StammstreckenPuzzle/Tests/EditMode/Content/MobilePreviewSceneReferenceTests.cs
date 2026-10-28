using System;
using System.Reflection;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    public sealed class MobilePreviewSceneReferenceTests
    {
        private const string Script="11111111111111111111111111111111";
        private const string Asset="22222222222222222222222222222222";
        private const string Header="%YAML 1.1\n--- !u!114 &1\nMonoBehaviour:\n  m_Script: {fileID: 11500000, guid: "+Script+", type: 3}\n";
        private const string Reference="  endlessProfilesData: {fileID: 4900000, guid: "+Asset+", type: 3}\n";
        private static void Validate(string text)
        {
            var method=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobilePreviewBuild",true)!
                .GetMethod("ValidateSceneReferences",BindingFlags.NonPublic|BindingFlags.Static)!;
            try{method.Invoke(null,new object[]{text,Script,new[]{"endlessProfilesData"},new[]{Asset},new long[]{4900000}});}
            catch(TargetInvocationException error){throw error.InnerException!;}
        }
        [Test]
        public void SavedInstallerRequiresItsOwnExactAssetReference()
        {
            Assert.DoesNotThrow(()=>Validate(Header+Reference));
            Assert.DoesNotThrow(()=>Validate((Header+Reference).Replace("\n","\r\n")));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+Reference+Reference));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+Reference+"  endlessProfilesData: {fileID: 0}\n"));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+Reference.Replace("4900000","4900001")));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+Reference.Replace(Asset,Script)));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+"--- !u!114 &2\nMonoBehaviour:\n"+Reference));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+Reference+Header+Reference));
            Assert.Throws<InvalidOperationException>(()=>Validate(Header+"  m_Script: {fileID: 11500000, guid: "+Asset+", type: 3}\n"+Reference));
        }
        [Test]
        public void ActualSavedMobileSceneBindsFullOfflineNoticesWithoutPreflightRepair()
        {
            const string scenePath="Assets/StammstreckenPuzzle/Scenes/QA/MobilePreview.unity";
            const string assetPath="Assets/StammstreckenPuzzle/UI/ThirdPartyNotices.txt";
            byte[] before=System.IO.File.ReadAllBytes(scenePath),original=System.IO.File.ReadAllBytes(assetPath);
            var asset=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(assetPath);
            Assert.That(asset,Is.Not.Null);CollectionAssert.AreEqual(original,asset.bytes);
            Assert.That(asset.text.Length,Is.GreaterThan(0));
            var build=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobilePreviewBuild",true)!;
            var verify=build.GetMethod("VerifyArtifacts",BindingFlags.Public|BindingFlags.Static)!;
            Assert.DoesNotThrow(()=>{try{verify.Invoke(null,null);}catch(TargetInvocationException error){throw error.InnerException!;}});
            CollectionAssert.AreEqual(before,System.IO.File.ReadAllBytes(scenePath));
            Assert.That(UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset,out string guid,out long id),Is.True);
            string script=UnityEditor.AssetDatabase.AssetPathToGUID("Assets/StammstreckenPuzzle/Scripts/Bootstrap/MobilePreviewInstaller.cs");
            var validate=build.GetMethod("ValidateSceneReferences",BindingFlags.NonPublic|BindingFlags.Static)!;
            void Check(string text)
            {try{validate.Invoke(null,new object[]{text,script,new[]{"offlineNotices"},new[]{guid},new[]{id}});}catch(TargetInvocationException error){throw error.InnerException!;}}
            string scene=System.Text.Encoding.UTF8.GetString(before);
            Assert.DoesNotThrow(()=>Check(scene));
            string missing=System.Text.RegularExpressions.Regex.Replace(scene,@"(?m)^  offlineNotices:.*\r?\n","");
            Assert.That(missing,Is.Not.EqualTo(scene));Assert.Throws<InvalidOperationException>(()=>Check(missing));
            CollectionAssert.AreEqual(before,System.IO.File.ReadAllBytes(scenePath));
            CollectionAssert.AreEqual(original,System.IO.File.ReadAllBytes(assetPath));
        }
    }
}
