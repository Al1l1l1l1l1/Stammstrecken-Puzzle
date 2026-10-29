using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    public sealed class HumanLocalExportPathTests
    {
        private string? fixtureRoot;
        private string? fixtureBoundary;

        [SetUp]
        public void CreatePrivateTemporaryFixture()
        {
            var configured = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");
            var temporary = string.IsNullOrWhiteSpace(configured) ? Path.GetTempPath() : configured!;
            if (!Path.IsPathRooted(temporary)) throw new InvalidOperationException("HUMAN_TEST_TEMP_ROOT_MUST_BE_ABSOLUTE");
            fixtureBoundary = Path.GetFullPath(Path.Combine(temporary, "STP.HumanLocalExport.Tests"));
            RejectReparseAncestors(fixtureBoundary);
            Directory.CreateDirectory(fixtureBoundary);
            fixtureRoot = Path.GetFullPath(Path.Combine(fixtureBoundary, Guid.NewGuid().ToString("N")));
            RequireFixtureBoundary(fixtureRoot);
            if (Directory.Exists(fixtureRoot) || File.Exists(fixtureRoot)) throw new IOException("HUMAN_TEST_FIXTURE_ALREADY_EXISTS");
            Directory.CreateDirectory(fixtureRoot);
        }

        [TearDown]
        public void RemoveOnlyTheOwnedTemporaryFixture()
        {
            if (fixtureRoot == null || !Directory.Exists(fixtureRoot)) return;
            RequireFixtureBoundary(fixtureRoot);
            RemoveOwnedTree(fixtureRoot);
        }

        private void RequireFixtureBoundary(string path)
        {
            if (fixtureRoot == null || fixtureBoundary == null ||
                !string.Equals(Path.GetFullPath(path), fixtureRoot, StringComparison.Ordinal) ||
                !string.Equals(Path.GetDirectoryName(fixtureRoot), fixtureBoundary, StringComparison.Ordinal) ||
                !Guid.TryParseExact(Path.GetFileName(fixtureRoot), "N", out _))
                throw new IOException("HUMAN_TEST_CLEANUP_BOUNDARY_INVALID");
            RejectReparseAncestors(path);
        }

        private static void RejectReparseAncestors(string path)
        {
            for (string? node = Path.GetFullPath(path); node != null; node = Path.GetDirectoryName(node))
                if ((File.Exists(node) || Directory.Exists(node)) && (File.GetAttributes(node) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("HUMAN_TEST_REPARSE_REJECTED");
        }

        private void RemoveOwnedTree(string directory)
        {
            if (fixtureRoot == null || (directory != fixtureRoot &&
                !directory.StartsWith(fixtureRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal)))
                throw new IOException("HUMAN_TEST_CLEANUP_OUTSIDE_FIXTURE");
            RejectReparseAncestors(directory);
            foreach (var entry in Directory.GetFileSystemEntries(directory))
            {
                if ((File.GetAttributes(entry) & FileAttributes.ReparsePoint) != 0) throw new IOException("HUMAN_TEST_REPARSE_REJECTED");
                if (Directory.Exists(entry)) RemoveOwnedTree(entry); else File.Delete(entry);
            }
            Directory.Delete(directory, false);
        }

        private Type Adapter=>Assembly.Load(Environment.GetEnvironmentVariable("STP_HUMAN_EXPORT_ASSEMBLY")??"STP.Platform").GetType("STP.Platform.HumanLocalExportAdapter",true)!;
        private object? Call(string name,params object[] args)=>Adapter.GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,args);
        [TestCase("com.STP.StammstreckenPuzzle.qa")][TestCase("com.STP.StammstreckenPuzzle")][TestCase("com.STP.StammstreckenPuzzle.HUMAN")]
        public void OnlyTheExactHumanApplicationMayExport(string id)=>Assert.Catch(()=>Call("RequireApplicationId",id));
        [Test]
        public void ExactHumanAndDirectExistingJsonChildPassWithoutInvokingUnityOrNativeUi()
        {
            Assert.DoesNotThrow(()=>Call("RequireApplicationId","com.STP.StammstreckenPuzzle.human"));
            var root=fixtureRoot!;var file=Path.Combine(root,"human-2026_test.json");File.WriteAllText(file,"{\"kind\":\"controlled-fixture\"}");
            Assert.That(Call("ValidateChildPath",root,file,true),Is.EqualTo(Path.GetFullPath(file)));
            Assert.Catch(()=>Call("ValidateChildPath",root,Path.Combine(root,"missing.json"),true));
            File.WriteAllBytes(file,Array.Empty<byte>());Assert.Catch(()=>Call("ValidateChildPath",root,file,true));
        }
        [TestCase("../record.json")][TestCase("sub/record.json")][TestCase("record.txt")][TestCase(".json")][TestCase(".hidden.json")][TestCase("record.json.exe")][TestCase("a:b.json")][TestCase("record..json")][TestCase("record.JSON")]
        public void FileNameCannotEscapeOrSmuggleOtherExtensions(string name)=>Assert.Catch(()=>Call("ValidateFileName",name));
        [Test]
        public void NormalizedOtherDirectoryAndNestedChildrenCannotBeShared()
        {
            var root=fixtureRoot!;
            Assert.Catch(()=>Call("ValidateChildPath",root,Path.Combine(root,"..","foreign.json"),false));
            Assert.Catch(()=>Call("ValidateChildPath",root,Path.Combine(root,"sub","foreign.json"),false));
            Assert.Catch(()=>Call("ValidateChildPath",root,"relative.json",false));
        }
    }
}
