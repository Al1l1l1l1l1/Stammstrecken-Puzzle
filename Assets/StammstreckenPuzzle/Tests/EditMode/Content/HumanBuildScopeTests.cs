using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

namespace STP.Tests.Content.EditMode
{
    public sealed class HumanBuildScopeTests
    {
        [Test]
        public void NativePanelAndImportedTextReferencesKeepTheirActualPPtrTypes()
        {
            string script=new string('a',32),panel=new string('b',32),text=new string('c',32);
            string source="%YAML 1.1\n--- !u!114 &1\nMonoBehaviour:\n  m_Script: {fileID: 11500000, guid: "+script+", type: 3}\n"+
                "  panel: {fileID: 11400000, guid: "+panel+", type: 2}\n  assignmentData: {fileID: 4900000, guid: "+text+", type: 3}\n";
            var method=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobilePreviewBuild",true)!.GetMethod("ValidateTypedSceneReferences",BindingFlags.Static|BindingFlags.NonPublic)!;
            void Check(string value){try{method.Invoke(null,new object[]{value,script,new[]{"panel","assignmentData"},new[]{panel,text},new long[]{11400000,4900000},new[]{2,3}});}catch(TargetInvocationException e){throw e.InnerException!;}}
            Assert.DoesNotThrow(()=>Check(source));
            Assert.Throws<InvalidOperationException>(()=>Check(source.Replace(panel+", type: 2",panel+", type: 3")));
            Assert.Throws<InvalidOperationException>(()=>Check(source.Replace(text+", type: 3",text+", type: 2")));
            Assert.Throws<InvalidOperationException>(()=>Check(source+"  panel: {fileID: 11400000, guid: "+panel+", type: 2}\n"));
        }
        private static void Validate(BuildTarget target)
        {
            var type=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.HumanCalibrationBuild",true)!;
            try{type.GetMethod("ValidateBuildScope",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{target});}
            catch(TargetInvocationException error){throw error.InnerException!;}
        }
        [TestCase(BuildTarget.Android)]
        [TestCase(BuildTarget.iOS)]
        public void ActualHumanApplicationIdWithoutExplicitRootBuildScopeIsRejected(BuildTarget target)
        {
            var named=target==BuildTarget.Android?NamedBuildTarget.Android:NamedBuildTarget.iOS;
            string original=PlayerSettings.GetApplicationIdentifier(named);
            try
            {
                PlayerSettings.SetApplicationIdentifier(named,"com.STP.StammstreckenPuzzle.qa");Assert.DoesNotThrow(()=>Validate(target));
                foreach(string id in new[]{"com.STP.StammstreckenPuzzle.human","com.STP.StammstreckenPuzzle.HUMAN","foreign.application.human"})
                {PlayerSettings.SetApplicationIdentifier(named,id);Assert.Throws<BuildFailedException>(()=>Validate(target),id);}
            }
            finally{PlayerSettings.SetApplicationIdentifier(named,original);}
        }
    }
}
