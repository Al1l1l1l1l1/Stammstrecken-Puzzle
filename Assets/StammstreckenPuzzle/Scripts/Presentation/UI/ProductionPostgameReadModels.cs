using System;
using System.Collections.Generic;

namespace STP.Presentation.UI
{
    /// <summary>Actual persisted lifecycle of one open generated puzzle.</summary>
    public enum EndlessEntryPhase { Reserved, Active, CompletionPending }
    /// <summary>Open record projection with no seed, proof, solution or save authority.</summary>
    public sealed class EndlessEntryView
    {
        /// <summary>Copies the actual record identity and currently permitted actions.</summary>
        public EndlessEntryView(string recordId,string profileId,EndlessEntryPhase phase,bool canResume,bool canAbandon)
        {RecordId=recordId??throw new ArgumentNullException(nameof(recordId));ProfileId=profileId??throw new ArgumentNullException(nameof(profileId));Phase=phase;CanResume=canResume;CanAbandon=canAbandon;if(!Enum.IsDefined(typeof(EndlessEntryPhase),phase))throw new ArgumentOutOfRangeException(nameof(phase));}
        /// <summary>Actual stable persistent record ID; it is never displayed as player copy.</summary>
        public string RecordId{get;}
        /// <summary>One of the four versioned profile IDs.</summary>
        public string ProfileId{get;}
        /// <summary>Actual persisted phase.</summary>
        public EndlessEntryPhase Phase{get;}
        /// <summary>Root policy permits continuing this exact record.</summary>
        public bool CanResume{get;}
        /// <summary>Root policy permits explicit provider-free abandonment.</summary>
        public bool CanAbandon{get;}
    }
    /// <summary>Confirmed post-season gate, available profiles and bounded open records.</summary>
    public sealed class EndlessView
    {
        /// <summary>Copies a projection; statusKey is ready/generating/budget/unavailable/failed.</summary>
        public EndlessView(bool unlocked,bool generating,IReadOnlyList<string> availableProfileIds,IReadOnlyList<EndlessEntryView> openEntries,string statusKey)
        {
            Unlocked=unlocked;Generating=generating;StatusKey=statusKey??throw new ArgumentNullException(nameof(statusKey));
            if(statusKey!="ready"&&statusKey!="generating"&&statusKey!="budget"&&statusKey!="unavailable"&&statusKey!="failed")throw new ArgumentException("ENDLESS_DISPLAY_STATUS_INVALID");
            if(availableProfileIds==null||openEntries==null||openEntries.Count>20)throw new ArgumentException("ENDLESS_DISPLAY_RECORDS_INVALID");
            var profiles=new string[availableProfileIds.Count];var ids=new HashSet<string>(StringComparer.Ordinal);for(var i=0;i<profiles.Length;i++){var id=availableProfileIds[i];if(!KnownProfile(id)||!ids.Add(id))throw new ArgumentException("ENDLESS_DISPLAY_PROFILE_INVALID");profiles[i]=id;}AvailableProfileIds=Array.AsReadOnly(profiles);
            var records=new EndlessEntryView[openEntries.Count];ids.Clear();for(var i=0;i<records.Length;i++){var entry=openEntries[i];if(entry==null||!KnownProfile(entry.ProfileId)||!ids.Add(entry.RecordId))throw new ArgumentException("ENDLESS_DISPLAY_RECORD_INVALID");records[i]=entry;}OpenEntries=Array.AsReadOnly(records);
        }
        /// <summary>Actual completion of all 240 first clears.</summary>
        public bool Unlocked{get;}
        /// <summary>A bounded worker generation operation is actually running.</summary>
        public bool Generating{get;}
        /// <summary>Actually callable profiles, not an approval inferred by UI.</summary>
        public IReadOnlyList<string> AvailableProfileIds{get;}
        /// <summary>Actual open reservations, active drafts or completion claims.</summary>
        public IReadOnlyList<EndlessEntryView> OpenEntries{get;}
        /// <summary>Closed localized status suffix.</summary>
        public string StatusKey{get;}
        internal static bool KnownProfile(string id)=>id=="dienstbeginn"||id=="regelbetrieb"||id=="stoerungslage"||id=="grosslage";
    }
    /// <summary>Actual optional completion-claim projection, shared by campaign and generated puzzles.</summary>
    public sealed class CompletionBonusView
    {
        /// <summary>ConfirmedAmount is absent until the one actual +10 terminal has committed.</summary>
        public CompletionBonusView(bool canRequestVideo,bool canSkip,bool isPending,int? confirmedAmount)
        {CanRequestVideo=canRequestVideo;CanSkip=canSkip;IsPending=isPending;ConfirmedAmount=confirmedAmount;if(confirmedAmount.HasValue&&confirmedAmount!=10)throw new ArgumentOutOfRangeException(nameof(confirmedAmount));}
        /// <summary>Actual eligibility and provider readiness for the explicit optional video.</summary>
        public bool CanRequestVideo{get;}
        /// <summary>Actual unresolved local claim can be skipped without a provider.</summary>
        public bool CanSkip{get;}
        /// <summary>Provider/commit resolution is actually pending.</summary>
        public bool IsPending{get;}
        /// <summary>Actual durably granted bonus.</summary>
        public int? ConfirmedAmount{get;}
    }
    /// <summary>Confirmed best record for one canonical campaign identity.</summary>
    public sealed class CampaignPuzzleRecord
    {
        /// <summary>Copies actual saved bests and direct-solve classification.</summary>
        public CampaignPuzzleRecord(int stars,long? firstRunBestElapsedMs,long? revisionBestElapsedMs,bool directSolved)
        {if(stars<0||stars>3||firstRunBestElapsedMs.HasValue&&firstRunBestElapsedMs.Value<0||revisionBestElapsedMs.HasValue&&revisionBestElapsedMs.Value<0)throw new ArgumentOutOfRangeException(nameof(stars));Stars=stars;FirstRunBestElapsedMs=firstRunBestElapsedMs;RevisionBestElapsedMs=revisionBestElapsedMs;DirectSolved=directSolved;}
        /// <summary>Confirmed star high-water mark.</summary>
        public int Stars{get;}
        /// <summary>Confirmed first-run best active time.</summary>
        public long? FirstRunBestElapsedMs{get;}
        /// <summary>Confirmed revision best active time.</summary>
        public long? RevisionBestElapsedMs{get;}
        /// <summary>Actual retained direct-solve mark.</summary>
        public bool DirectSolved{get;}
    }
}
