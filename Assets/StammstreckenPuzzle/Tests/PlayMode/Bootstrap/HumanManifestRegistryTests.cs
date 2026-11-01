#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using STP.Bootstrap;
using UnityEditor;
using UnityEngine;

namespace STP.Tests.Bootstrap.PlayMode
{
    public sealed class HumanManifestRegistryTests
    {
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private static TextAsset Asset(string path)=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+path);
        private static string PublicHash=>JsonUtility.FromJson<Binding>(Asset("MobileCandidate.bindings.json").text).publicCatalogHashSha256;
        [Test]
        public void CompleteRegistryResolvesAllSeventeenExactOriginalsWithoutCreatingRuns()
        {
            var packs=HumanManifestRegistry.ExpectedPackPaths().Select(p=>Asset("HumanAssignments/"+p)).ToArray();
            var registry=new HumanManifestRegistry(packs,Asset("HumanAssignments/raw-sha256-lock.json"),Asset("HumanAssignments/ui-source-lock.json"),Asset("MobileCandidate.public.json"),PublicHash);
            Assert.That(registry.Options.Count,Is.EqualTo(17));Assert.That(registry.ManifestBytesSha256.Distinct().Count(),Is.EqualTo(17));
            Assert.That(registry.Options.Sum(p=>registry.Resolve(registry.HashForPath(p.Id)).Catalog.Assignments.Count),Is.EqualTo(102));
            foreach(var source in packs)
            {
                string path=source.name+".json",hash=registry.HashForPath(path);
                Assert.That(registry.PathForHash(hash),Is.EqualTo(path));CollectionAssert.AreEqual(source.bytes,registry.Resolve(hash).GetOriginalBytes());
                byte[] copy=registry.Resolve(hash).GetOriginalBytes();copy[0]^=1;
                CollectionAssert.AreEqual(source.bytes,registry.Resolve(hash).GetOriginalBytes(),"Resolver bytes must remain immutable.");
            }
            Assert.Throws<ArgumentException>(()=>registry.Resolve(new string('0',64)));
        }
        [TestCase("missingPack")]
        [TestCase("duplicateAsset")]
        [TestCase("unknownField")]
        [TestCase("duplicateKey")]
        [TestCase("wrongUi")]
        [TestCase("wrongHash")]
        public void RegistryCannotSilentlySelectDefaultOrAcceptPartialAndRewrittenBindings(string mutation)
        {
            var packs=HumanManifestRegistry.ExpectedPackPaths().Select(p=>Asset("HumanAssignments/"+p)).ToArray();
            string raw=Asset("HumanAssignments/raw-sha256-lock.json").text;
            TextAsset? replacement=null;
            try
            {
                switch(mutation)
                {
                    case "missingPack":packs=packs.Take(16).ToArray();break;
                    case "duplicateAsset":packs[16]=packs[0];break;
                    case "unknownField":raw=raw.Substring(0,raw.Length-1)+",\"extra\":1}";break;
                    case "duplicateKey":raw=raw.Substring(0,raw.Length-1)+",\"schemaVersion\":1}";break;
                    case "wrongUi":raw=Regex.Replace(raw,"\"uiSourceSha256\":\"[a-f0-9]{64}\"","\"uiSourceSha256\":\""+new string('0',64)+"\"");break;
                    case "wrongHash":raw=Regex.Replace(raw,"\"sha256\":\"[a-f0-9]{64}\"","\"sha256\":\""+new string('0',64)+"\"",RegexOptions.None,TimeSpan.FromSeconds(1));break;
                    default:throw new ArgumentException(nameof(mutation));
                }
                replacement=new TextAsset(raw);
                Assert.Throws<ArgumentException>(()=>new HumanManifestRegistry(packs,replacement,Asset("HumanAssignments/ui-source-lock.json"),Asset("MobileCandidate.public.json"),PublicHash));
            }
            finally{if(replacement!=null)UnityEngine.Object.DestroyImmediate(replacement);}
        }
        [Serializable] private sealed class Binding{public string publicCatalogHashSha256=string.Empty;}
    }
}
#endif
