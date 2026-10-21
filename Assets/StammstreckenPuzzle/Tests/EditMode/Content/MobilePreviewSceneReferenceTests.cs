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
    }
}
