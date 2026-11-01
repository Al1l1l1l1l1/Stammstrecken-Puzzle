using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

namespace STP.Tests.Content.EditMode
{
    public sealed class MobileBuildTargetGuardTests
    {
        private static void Validate(BuildTarget requested, BuildTarget active, BuildTarget compiled)
        {
            var method = Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobilePrivacyBuildGuard", true)!
                .GetMethod("ValidateBuildTarget", BindingFlags.Static | BindingFlags.NonPublic)!;
            try { method.Invoke(null, new object[] { requested, active, compiled }); }
            catch (TargetInvocationException error) { throw error.InnerException!; }
        }

        [TestCase(BuildTarget.Android)]
        [TestCase(BuildTarget.iOS)]
        public void MobileTargetMustMatchActiveAndCompiledEditorHooks(BuildTarget target)
        {
            Assert.DoesNotThrow(() => Validate(target, target, target));
            Assert.Throws<BuildFailedException>(() => Validate(target, BuildTarget.StandaloneWindows64, target));
            Assert.Throws<BuildFailedException>(() => Validate(target, target, BuildTarget.NoTarget));
            var other = target == BuildTarget.Android ? BuildTarget.iOS : BuildTarget.Android;
            Assert.Throws<BuildFailedException>(() => Validate(target, target, other));
        }
    }
}
