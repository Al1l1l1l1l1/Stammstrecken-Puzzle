using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace STP.Editor.Build
{
    /// <summary>Reproducible local audio import and five-bus Unity mixer for the internal mobile candidate.</summary>
    public static class ProductionAudioAssets
    {
        /// <summary>Versioned central mixer location, consumed by scene composition.</summary>
        public const string MixerPath="Assets/StammstreckenPuzzle/Audio/ProductionAudio.mixer";
        private const string ClipRoot="Assets/StammstreckenPuzzle/Audio/Cues/";
        private static readonly string[] Files={"ui-tool-focus","grid-empty-mark","grid-occupancy-note","grid-track-seat","grid-clear","ui-undo","completion-route-reveal","train-start","train-roll","train-arrive","results-panel-open","completion-clearance","ambience-scene-open","music-workshop","train-curve"};
        /// <summary>Applies the explicit PCM preset and creates missing mixer buses through Unity's pinned editor implementation.</summary>
        public static void EnsureAssets()
        {
            foreach(var file in Files)
            {
                var path=ClipRoot+file+".wav";if(!File.Exists(path))throw new InvalidOperationException("AUDIO_SOURCE_MISSING:"+path);
                AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=AssetImporter.GetAtPath(path) as AudioImporter??throw new InvalidOperationException("AUDIO_IMPORTER_MISSING");
                var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;settings.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;settings.preloadAudioData=true;settings.quality=1;
                importer.defaultSampleSettings=settings;importer.forceToMono=true;importer.loadInBackground=false;importer.ambisonic=false;
                var serialized=new SerializedObject(importer);var normalize=serialized.FindProperty("m_Normalize")??throw new InvalidOperationException("AUDIO_IMPORT_NORMALIZE_FIELD_MISSING");normalize.boolValue=false;serialized.ApplyModifiedPropertiesWithoutUndo();importer.SaveAndReimport();
            }
            var mixer=AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if(mixer==null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(MixerPath)!);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                // Unity 6000.3 exposes mixer authoring through its internal editor controller. Reflection stays editor-only.
                Type? controller=null;foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){controller=assembly.GetType("UnityEditor.Audio.AudioMixerController",false);if(controller!=null)break;}
                if(controller==null)throw new InvalidOperationException("AUDIO_MIXER_EDITOR_CONTROLLER_MISSING");
                var create=controller.GetMethod("CreateMixerControllerAtPath",BindingFlags.Static|BindingFlags.Public)??throw new InvalidOperationException("AUDIO_MIXER_EDITOR_API_MISSING");
                mixer=(AudioMixer)create.Invoke(null,new object[]{MixerPath})!;
                AudioMixerGroup? master=null;foreach(var group in mixer.FindMatchingGroups("Master"))if(group.name=="Master")master=group;
                if(master==null)throw new InvalidOperationException("AUDIO_MASTER_GROUP_MISSING");
                foreach(var id in new[]{"Music","SFX","Ambience","UI"})
                {
                    var group=controller.GetMethod("CreateNewGroup",BindingFlags.Public|BindingFlags.Instance)!.Invoke(mixer,new object[]{id,false})!;
                    controller.GetMethod("AddChildToParent",BindingFlags.Public|BindingFlags.Instance)!.Invoke(mixer,new[]{group,master});
                }
                EditorUtility.SetDirty(mixer);AssetDatabase.SaveAssets();
            }
            foreach(var id in new[]{"Master","Music","SFX","Ambience","UI"})
            {var count=0;foreach(var group in mixer.FindMatchingGroups(id))if(group.name==id)count++;if(count!=1)throw new InvalidOperationException("AUDIO_MIXER_GROUP_INVALID:"+id);}
            var clips=LoadClips();foreach(var clip in clips)if(clip.channels!=1||clip.frequency!=48000||clip.loadType!=AudioClipLoadType.DecompressOnLoad)throw new InvalidOperationException("AUDIO_IMPORTED_FORMAT_INVALID");
        }
        /// <summary>Loads the required central mixer after EnsureAssets.</summary>
        public static AudioMixer LoadMixer()=>AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath)??throw new InvalidOperationException("AUDIO_MIXER_MISSING");
        /// <summary>Loads exactly fifteen imported cues in the documented runtime semantic order.</summary>
        public static AudioClip[] LoadClips()
        {var result=new AudioClip[Files.Length];for(var i=0;i<Files.Length;i++)result[i]=AssetDatabase.LoadAssetAtPath<AudioClip>(ClipRoot+Files[i]+".wav")??throw new InvalidOperationException("AUDIO_CLIP_MISSING:"+Files[i]);return result;}
    }
}
