using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Bootstrap;

namespace STP.Tests.RuntimeComposition.EditMode
{
    public sealed class PublisherContactConfigurationTests
    {
        [Test]
        public async Task ConfigurationIsInert_AndOnlyExplicitRequestDispatchesTheSelectedPublicPage()
        {
            var calls=new List<string>();
            var configuration=new PublisherContactConfiguration("https://publisher.example/legal#privacy","https://publisher.example/support");
            Assert.That(calls,Is.Empty);
            await configuration.RequestOpenAsync(false,calls.Add);
            Assert.That(calls,Is.EqualTo(new[]{"https://publisher.example/legal#privacy"}));
            await configuration.RequestOpenAsync(true,calls.Add);
            Assert.That(calls,Is.EqualTo(new[]{"https://publisher.example/legal#privacy","https://publisher.example/support"}));
        }
        [TestCase("http://publisher.example/legal")]
        [TestCase("file:///private/profile/save.json")]
        [TestCase("javascript:alert(1)")]
        [TestCase("mailto:person@publisher.example")]
        [TestCase("https://account:secret@publisher.example/legal")]
        [TestCase("https://publisher.example:8443/legal")]
        [TestCase("https://127.0.0.1/legal")]
        [TestCase("https://[::1]/legal")]
        [TestCase("https://localhost/legal")]
        [TestCase("https://publisher.example/legal?receipt=private")]
        [TestCase(" https://publisher.example/legal")]
        [TestCase("https://publisher.example/legal\n")]
        [TestCase("")]
        [TestCase(null)]
        public void MissingOrUnapprovedDestinationShapeIsRejectedBeforeAnyLaunch(string invalid)
        {
            Assert.Throws<ArgumentException>(()=>new PublisherContactConfiguration(invalid,"https://publisher.example/support"));
            Assert.Throws<ArgumentException>(()=>new PublisherContactConfiguration("https://publisher.example/legal",invalid));
        }
        [Test]
        public void OversizedDestinationAndAbsentLauncherAreRejected()
        {
            Assert.Throws<ArgumentException>(()=>new PublisherContactConfiguration("https://publisher.example/"+new string('a',2048),"https://publisher.example/support"));
            var configuration=new PublisherContactConfiguration("https://publisher.example/legal","https://publisher.example/support");
            Assert.Throws<ArgumentNullException>(()=>configuration.RequestOpenAsync(false,null!));
        }
        [Test]
        public void ProductionSceneContactGuardRejectsDuplicateOrChangedDestinations()
        {
            var expected=new PublisherContactConfiguration("https://publisher.example/legal","https://publisher.example/support");
            string valid="  publisherLegalUrl: "+expected.LegalUrl+"\n  publisherSupportUrl: "+expected.SupportUrl+"\n";
            CheckSceneBlock(valid,expected);
            foreach(string invalid in new[]{valid+"  publisherLegalUrl: "+expected.LegalUrl+"\n",valid.Replace(expected.SupportUrl,"https://other.example/support"),valid.Replace("publisherSupportUrl:","missingSupportUrl:")})
            {
                var failure=Assert.Throws<TargetInvocationException>(()=>CheckSceneBlock(invalid,expected));
                Assert.That(failure!.InnerException,Is.TypeOf<InvalidOperationException>());
                Assert.That(failure.InnerException!.Message,Is.EqualTo("PRODUCTION_PUBLISHER_CONTACTS_CHANGED"));
            }
        }
        [Test]
        public void ActualUnityAssetSerializationRetainsExactConfiguredPublicDestinations()
        {
            var scene=UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
            string path="Assets/StammstreckenPuzzle/Tests/EditMode/Content/RuntimeComposition/publisher-serialization-"+Guid.NewGuid().ToString("N")+".prefab";
            Assert.That(File.Exists(path),Is.False);
            try
            {
                var holder=new UnityEngine.GameObject("Publisher serialization fixture");holder.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(holder,scene);
                var installer=holder.AddComponent<MobilePreviewInstaller>();
                var contacts=new PublisherContactConfiguration("https://publisher.example/legal#privacy","https://publisher.example/support");
                installer.ConfigurePublisherContacts(contacts);Assert.That(installer.HasPublisherContacts(contacts),Is.True);
                Assert.That(UnityEditor.PrefabUtility.SaveAsPrefabAsset(holder,path),Is.Not.Null);
                CheckSceneBlock(File.ReadAllText(path),contacts);
            }
            finally
            {
                UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(scene);
                if(File.Exists(path))UnityEditor.AssetDatabase.DeleteAsset(path);
            }
        }
        private static void CheckSceneBlock(string block,PublisherContactConfiguration expected)
            =>Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.ProductionGameBuild",true)!
                .GetMethod("ValidatePublisherSceneBlock",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{block,expected});
    }
}
