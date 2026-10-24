using System;
using System.Collections.Generic;
using STP.Application;
using STP.Application.Ports;
using UnityEngine;
using UnityEngine.Audio;

namespace STP.Audio
{
    /// <summary>Bounded Unity Audio voice pool and central five-group mix. Cues express shown actions, never correctness.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionGameAudio:MonoBehaviour,IAudioPort
    {
        private static readonly string[] CueNames={"ToolFocus","EmptyMark","OccupancyNote","TrackSeat","Clear","Undo","RouteReveal","TrainStart","TrainRoll","TrainArrive","ResultsOpen","CompletionClearance","SceneAmbience","WorkshopMusic","TrainCurve"};
        private static readonly string[] Groups={"UI","SFX","SFX","SFX","UI","UI","SFX","SFX","Ambience","SFX","UI","SFX","Ambience","Music","SFX"};
        private static readonly string[] GroupNames={"Master","Music","SFX","Ambience","UI"};
        private readonly Dictionary<string,float> gain=new Dictionary<string,float>(StringComparer.Ordinal);
        private readonly List<AudioSource> owned=new List<AudioSource>();
        private AudioClip[] clips=Array.Empty<AudioClip>();
        private AudioSource[] ui=Array.Empty<AudioSource>(),effects=Array.Empty<AudioSource>();
        private AudioSource? roll,ambience,music;
        private int uiNext,sfxNext;
        private bool configured,suspended,puzzleVisible,riding,hadFocus=true,appPaused;
        /// <summary>Exact imported bank size, with stable semantic ordering.</summary>
        public const int ClipCount=15;
        /// <summary>Whether all required clips and five central mixer groups were explicitly configured.</summary>
        public bool IsConfigured=>configured;
        /// <summary>True while lifecycle or an explicit modal suspends owned playback.</summary>
        public bool IsSuspended=>suspended||!hadFocus||appPaused;
        /// <summary>Number of allocated sources, bounded independently of click count.</summary>
        public int VoiceCount=>owned.Count;
        /// <summary>Configures imported clips and mixer once. No file or network access occurs.</summary>
        public void Configure(AudioMixer mixer,AudioClip[] bank)
        {
            if(configured)throw new InvalidOperationException("AUDIO_ALREADY_CONFIGURED");if(mixer==null||bank==null||bank.Length!=ClipCount)throw new ArgumentException("AUDIO_BANK_INVALID");
            foreach(var clip in bank)if(clip==null||clip.channels!=1||clip.frequency!=48000)throw new ArgumentException("AUDIO_CLIP_FORMAT_INVALID");
            var buses=new Dictionary<string,AudioMixerGroup>(StringComparer.Ordinal);
            foreach(var id in GroupNames)
            {
                var matches=mixer.FindMatchingGroups(id);AudioMixerGroup? exact=null;foreach(var match in matches)if(match.name==id){if(exact!=null)throw new ArgumentException("AUDIO_GROUP_DUPLICATE");exact=match;}
                if(exact==null)throw new ArgumentException("AUDIO_GROUP_MISSING:"+id);buses.Add(id,exact);gain.Add(id,1);
            }
            clips=(AudioClip[])bank.Clone();ui=new[]{Source(buses["UI"]),Source(buses["UI"])};effects=new[]{Source(buses["SFX"]),Source(buses["SFX"]),Source(buses["SFX"])};
            roll=Source(buses["Ambience"]);roll.clip=clips[8];roll.loop=true;
            ambience=Source(buses["Ambience"]);ambience.clip=clips[12];ambience.loop=true;
            music=Source(buses["Music"]);music.clip=clips[13];music.loop=true;configured=true;ApplyGains();SyncLoops();
        }
        /// <summary>Projects persisted rational volume groups. Unknown future groups are ignored rather than rewritten.</summary>
        public void ApplySettings(LocalSettings settings)
        {
            if(settings==null)throw new ArgumentNullException(nameof(settings));if(!configured)return;
            foreach(var id in GroupNames)gain[id]=1;
            foreach(var stored in settings.AudioGroups)if(gain.ContainsKey(stored.GroupId))gain[stored.GroupId]=Mathf.Clamp01((float)((double)stored.Numerator/stored.Denominator));
            ApplyGains();
        }
        /// <summary>Receives closed semantic action names. Unknown optional cues safely do nothing.</summary>
        public void Play(string semanticCue)
        {
            if(!configured||IsSuspended)return;var index=Array.IndexOf(CueNames,semanticCue);if(index<0||index==8||index==12||index==13)return;
            var pool=Groups[index]=="UI"?ui:effects;var next=Groups[index]=="UI"?uiNext++:sfxNext++;var voice=pool[(next&int.MaxValue)%pool.Length];
            voice.Stop();voice.clip=clips[index];voice.Play();
        }
        /// <summary>Root supplies only actual presentation state, never a guessed valid route.</summary>
        public void SetPresentationState(bool puzzleIsVisible,bool rideIsActive)
        {if(puzzleVisible==puzzleIsVisible&&riding==rideIsActive)return;puzzleVisible=puzzleIsVisible;riding=rideIsActive;SyncLoops();ApplyGains();}
        /// <summary>Silences owned sounds while focus, native modals or help pause gameplay.</summary>
        public void SetSuspended(bool value){if(suspended==value)return;suspended=value;SyncPause();}
        /// <summary>Stops every owned source. No global AudioListener state is changed.</summary>
        public void StopAll(){foreach(var source in owned)if(source!=null)source.Stop();}
        private AudioSource Source(AudioMixerGroup group)
        {
            var child=new GameObject("Audio "+group.name+" "+owned.Count);child.transform.SetParent(transform,false);var source=child.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.loop=false;source.outputAudioMixerGroup=group;source.ignoreListenerPause=false;source.priority=group.name=="UI"?100:group.name=="SFX"?90:180;owned.Add(source);return source;
        }
        private void ApplyGains()
        {
            if(!configured)return;foreach(var source in owned)source.volume=gain["Master"]*gain[source.outputAudioMixerGroup.name];
            if(music!=null)music.volume*=riding?.32f:1;
        }
        private void SyncLoops()
        {
            if(!configured)return;Loop(music,puzzleVisible);Loop(ambience,puzzleVisible&&riding);Loop(roll,riding);
        }
        private void Loop(AudioSource? source,bool wanted)
        {if(source==null)return;if(!wanted){source.Stop();return;}if(IsSuspended){source.Pause();return;}if(!source.isPlaying)source.Play();}
        private void SyncPause()
        {
            if(!configured)return;if(IsSuspended){foreach(var source in owned)source.Pause();}
            else{foreach(var source in owned)source.UnPause();SyncLoops();}
        }
        private void OnApplicationFocus(bool value){hadFocus=value;SyncPause();}
        private void OnApplicationPause(bool value){appPaused=value;SyncPause();}
        private void OnEnable()=>SyncLoops();
        private void OnDisable()=>StopAll();
        private void OnDestroy(){StopAll();owned.Clear();}
    }
}
