using System;
using System.IO;
using NUnit.Framework;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.Rendering;

namespace STP.Tests.Presentation.PlayMode
{
    public sealed class StationMemoryTests
    {
        [Test]
        public void ResetAndMailSurviveReloadAndConfirmedDeletionRemovesBothFiles()
        {
            string directory=Path.Combine(UnityEngine.Application.temporaryCachePath,Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string path=Path.Combine(directory,"presentation-v1.json");
            try
            {
                File.WriteAllText(path,"broken JSON");
                var first=new StpPresentationMemory(directory);
                Assert.AreEqual(0,first.State.deadlineShiftYears);
                Assert.IsTrue(first.Fire("FIRST_START"));first.Read("mail-welcome");
                first.ConfirmReset();first.ConfirmReset();
                var reloaded=new StpPresentationMemory(directory);
                Assert.AreEqual(4,reloaded.State.deadlineShiftYears);
                Assert.IsFalse(reloaded.Fire("FIRST_START"));Assert.IsFalse(reloaded.Fire("FIRST_RESET"));
                CollectionAssert.AreEqual(new[]{"mail-welcome"},reloaded.State.readMailIds);
                File.WriteAllText(path+".tmp","interrupted write");reloaded.Delete();
                Assert.IsFalse(File.Exists(path));Assert.IsFalse(File.Exists(path+".tmp"));
                Assert.IsFalse(reloaded.Fire("FIRST_START"));
            }
            finally{if(File.Exists(path))File.Delete(path);if(File.Exists(path+".tmp"))File.Delete(path+".tmp");Directory.Delete(directory);}
        }

        [Test]
        public void RailwayCameraOnlyCreatesSceneryWithGraphics()
        {
            var root=new GameObject("Station test");var child=new GameObject("View");
            child.transform.SetParent(root.transform,false);
            try
            {
                var camera=child.AddComponent<Camera>();camera.enabled=false;
                var world=StpWorldCamera.Attach(camera);
                if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null)
                {
                    Assert.IsNull(world);Assert.IsFalse(camera.enabled);
                    Assert.IsEmpty(root.GetComponentsInChildren<MeshFilter>(true));
                    Assert.IsEmpty(root.GetComponentsInChildren<Light>(true));
                }
                else{Assert.IsNotNull(world);Assert.IsFalse(camera.orthographic);}
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
    }
}
