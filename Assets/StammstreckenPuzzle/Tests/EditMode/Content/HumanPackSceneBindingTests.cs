using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    public sealed class HumanPackSceneBindingTests
    {
        private static readonly string Script = new string('a', 32);
        private static readonly string[] Guids = Enumerable.Range(1, 17).Select(i => i.ToString("x32")).ToArray();
        private static string Line(int i) => "  - {fileID: 4900000, guid: " + Guids[i] + ", type: 3}\n";
        private static string Scene => "%YAML 1.1\n--- !u!114 &1\nMonoBehaviour:\n  m_Script: {fileID: 11500000, guid: " + Script + ", type: 3}\n" +
            "  assignmentPacks:\n" + string.Concat(Enumerable.Range(0, 17).Select(Line)) + "  rawAssignmentLock: {fileID: 4900000, guid: " + new string('b', 32) + ", type: 3}\n";
        private static void Check(string scene)
        {
            var method=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.HumanCalibrationBuild",true)!
                .GetMethod("ValidatePackSceneReferences",BindingFlags.Static|BindingFlags.NonPublic)!;
            try{method.Invoke(null,new object[]{scene,Script,Guids,Enumerable.Repeat(4900000L,17).ToArray()});}
            catch(TargetInvocationException error){throw error.InnerException!;}
        }
        [Test]
        public void ActualOrderedSeventeenTextPPtrsAcceptLfAndCrLf()
        {Assert.DoesNotThrow(()=>Check(Scene));Assert.DoesNotThrow(()=>Check(Scene.Replace("\n","\r\n")));}
        [TestCase("missing")]
        [TestCase("extra")]
        [TestCase("duplicate")]
        [TestCase("reorder")]
        [TestCase("nativeType")]
        [TestCase("wrongFileId")]
        [TestCase("wrongGuid")]
        [TestCase("secondArray")]
        [TestCase("legacyFallback")]
        [TestCase("secondInstaller")]
        public void PackBindingCannotBeSatisfiedByPartialOrIncidentalSceneText(string mutation)
        {
            string scene=Scene;
            switch(mutation)
            {
                case "missing":scene=scene.Replace(Line(0),string.Empty);break;
                case "extra":scene=scene.Replace(Line(0),Line(0)+Line(0));break;
                case "duplicate":scene=scene.Replace(Line(1),Line(0));break;
                case "reorder":scene=scene.Replace(Line(0)+Line(1),Line(1)+Line(0));break;
                case "nativeType":scene=scene.Replace(Line(0),Line(0).Replace("type: 3","type: 2"));break;
                case "wrongFileId":scene=scene.Replace(Line(0),Line(0).Replace("4900000","11400000"));break;
                case "wrongGuid":scene=scene.Replace(Line(0),Line(0).Replace(Guids[0],new string('c',32)));break;
                case "secondArray":scene+="  assignmentPacks:\n"+Line(0);break;
                case "legacyFallback":scene+="  assignmentData: {fileID: 4900000, guid: "+Guids[0]+", type: 3}\n";break;
                case "secondInstaller":scene+=Scene.Substring(Scene.IndexOf("---",StringComparison.Ordinal));break;
                default:throw new ArgumentException(nameof(mutation));
            }
            // All expected GUID strings may still occur in an unrelated component; they grant no binding authority.
            scene+="--- !u!114 &9\nMonoBehaviour:\n  m_Name: "+string.Join(",",Guids)+"\n";
            Assert.Throws<InvalidOperationException>(()=>Check(scene));
        }
        private const string CurrentPublicHash="0ad7ae94f5a457e0b78d2dec5901256de15069fdd21e3c13dd724d549e7a60e8";
        private const string StalePublicHash="886dab4515817e7f1bdf3142e4097c5bd87b4431e82262189d5fe4a5908c23f0";
        private static string HashLine(string hash)=>"  publicCatalogHashSha256: "+hash+"\n";
        private static string HashScene=>Scene+HashLine(CurrentPublicHash);
        private static void CheckPublicHash(string scene,string script,string expected)
        {
            var method=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.HumanCalibrationBuild",true)!
                .GetMethod("ValidatePublicCatalogHashValue",BindingFlags.Static|BindingFlags.NonPublic)!;
            try{method.Invoke(null,new object[]{scene,script,expected});}
            catch(TargetInvocationException error){throw error.InnerException!;}
        }
        [Test]
        public void ExactInstallerPublicHashAcceptsLfCrLfAndIgnoresForeignComponentValue()
        {
            Assert.DoesNotThrow(()=>CheckPublicHash(HashScene,Script,CurrentPublicHash));
            Assert.DoesNotThrow(()=>CheckPublicHash(HashScene.Replace("\n","\r\n"),Script,CurrentPublicHash));
            string foreign="--- !u!114 &9\nMonoBehaviour:\n  m_Script: {fileID: 11500000, guid: "+new string('f',32)+", type: 3}\n"+HashLine(StalePublicHash);
            Assert.DoesNotThrow(()=>CheckPublicHash(HashScene+foreign,Script,CurrentPublicHash));
        }
        [TestCase("stale")]
        [TestCase("missing")]
        [TestCase("duplicateSame")]
        [TestCase("duplicateDifferent")]
        [TestCase("uppercase")]
        [TestCase("quoted")]
        [TestCase("trailingText")]
        [TestCase("wrongScript")]
        [TestCase("secondInstaller")]
        [TestCase("duplicateScript")]
        [TestCase("foreignOnly")]
        [TestCase("foreignWithStaleInstaller")]
        public void PublicHashCannotBeSatisfiedByStaleMissingDuplicateOrForeignText(string mutation)
        {
            string text=HashScene;
            string foreign="--- !u!114 &9\nMonoBehaviour:\n  m_Script: {fileID: 11500000, guid: "+new string('f',32)+", type: 3}\n"+HashLine(CurrentPublicHash);
            switch(mutation)
            {
                case "stale":text=text.Replace(CurrentPublicHash,StalePublicHash);break;
                case "missing":text=text.Replace(HashLine(CurrentPublicHash),string.Empty);break;
                case "duplicateSame":text+=HashLine(CurrentPublicHash);break;
                case "duplicateDifferent":text+=HashLine(StalePublicHash);break;
                case "uppercase":text=text.Replace(CurrentPublicHash,CurrentPublicHash.ToUpperInvariant());break;
                case "quoted":text=text.Replace(CurrentPublicHash,"\""+CurrentPublicHash+"\"");break;
                case "trailingText":text=text.Replace(CurrentPublicHash,CurrentPublicHash+" extra");break;
                case "wrongScript":text=text.Replace(Script,new string('e',32));break;
                case "secondInstaller":text+=HashScene.Substring(HashScene.IndexOf("---",StringComparison.Ordinal));break;
                case "duplicateScript":text+="  m_Script: {fileID: 11500000, guid: "+Script+", type: 3}\n";break;
                case "foreignOnly":text=text.Replace(HashLine(CurrentPublicHash),string.Empty)+foreign;break;
                case "foreignWithStaleInstaller":text=text.Replace(CurrentPublicHash,StalePublicHash)+foreign;break;
                default:throw new ArgumentException(nameof(mutation));
            }
            Assert.Throws<InvalidOperationException>(()=>CheckPublicHash(text,Script,CurrentPublicHash));
        }
        [Serializable] private sealed class CurrentBinding {public string publicCatalogHashSha256=string.Empty;}
        [Test]
        public void ActualSavedHumanScenePassesPreflightAndStaleInMemoryHashIsRejectedWithoutRepair()
        {
            const string scenePath="Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity";
            const string installerPath="Assets/StammstreckenPuzzle/Scripts/Bootstrap/HumanCalibrationInstaller.cs";
            var before=File.ReadAllBytes(scenePath);string text=File.ReadAllText(scenePath);
            string expected=JsonUtility.FromJson<CurrentBinding>(AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json").text).publicCatalogHashSha256;
            string guid=AssetDatabase.AssetPathToGUID(installerPath);
            Assert.DoesNotThrow(()=>CheckPublicHash(text,guid,expected));
            Assert.That(text.Contains("  publicCatalogHashSha256: "+expected),Is.True);
            string stale=text.Replace("  publicCatalogHashSha256: "+expected,"  publicCatalogHashSha256: "+StalePublicHash);
            Assert.That(stale,Is.Not.EqualTo(text));
            Assert.Throws<InvalidOperationException>(()=>CheckPublicHash(stale,guid,expected));
            var type=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.HumanCalibrationBuild",true)!;
            void Verify(){try{type.GetMethod("VerifyArtifacts",BindingFlags.Public|BindingFlags.Static)!.Invoke(null,null);}catch(TargetInvocationException error){throw error.InnerException!;}}
            Assert.DoesNotThrow(Verify);
            CollectionAssert.AreEqual(before,File.ReadAllBytes(scenePath),"Verification must not repair or rewrite the scene.");
        }

    }
}
