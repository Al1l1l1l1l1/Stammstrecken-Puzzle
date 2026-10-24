using System;
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
    }
}
