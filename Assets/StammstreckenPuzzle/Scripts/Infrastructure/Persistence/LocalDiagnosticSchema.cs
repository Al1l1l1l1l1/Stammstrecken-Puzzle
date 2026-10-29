using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Local-only typed platform metadata. No device identifier or OS free text.</summary>
    public enum LocalDiagnosticPlatform { Editor, Android, IOS, Other }
    /// <summary>Closed diagnostic outcomes; never business/transaction authority.</summary>
    public enum LocalDiagnosticOutcome { Succeeded, Unavailable, NotAllowed, Cancelled, TimedOut, Pending, Failed, NoFill }
    /// <summary>Calls whose coarse local outcome can be counted without provider payloads.</summary>
    public enum LocalDiagnosticOperation { Consent, PrivacyOptions, Rewarded, Interstitial, StorePrice, Purchase, Restore }
    /// <summary>Redacted exception category. Exception messages, names and stacks never enter the ring.</summary>
    public enum LocalDiagnosticCategory { None, IO, Access, Timeout, Cancelled, InvalidOperation, NullReference, Other }
    /// <summary>Closed source events; numeric unknown enum values are rejected.</summary>
    public enum LocalDiagnosticEvent { AppStarted, AppStopped, ProfileSynced, RewardSaveFailed, GrantSaveFailed, RevocationSaveFailed, SaveRecoveryRequired, UnexpectedException, FatalException, CorruptRingReset, DebugBreadcrumb }
    /// <summary>Best-effort logging outcome; callers must never turn these into a gameplay gate.</summary>
    public enum LocalDiagnosticWriteStatus { Written, Dropped, Closed, Unavailable }
    /// <summary>Only these deterministic injection points exist; never active unless explicitly injected in tests.</summary>
    public enum LocalDiagnosticFaultPoint { BeforeRead, BeforeWrite, AfterFlush, BeforeReplace, AfterReplace, BeforeClear }

    /// <summary>Explicit trusted build metadata, independently checked before a diagnostic file exists.</summary>
    public sealed class LocalDiagnosticEnvironment
    {
        /// <summary>Build labels/short commit hashes and numeric app versions only. Not an arbitrary data/ID field.</summary>
        public LocalDiagnosticEnvironment(string buildId, string appVersion, LocalDiagnosticPlatform platform)
        {
            if(buildId==null||buildId.Length>80||!Regex.IsMatch(buildId,@"\A(?:wp[0-9]{3}-[a-z0-9]+(?:-[a-z0-9]+){0,9}|[a-f0-9]{7,12})\z"))throw new ArgumentException("DIAGNOSTIC_BUILD_METADATA_INVALID");
            if(appVersion==null||!Regex.IsMatch(appVersion,@"\A[0-9]{1,4}(?:\.[0-9]{1,4}){1,3}\z")||!Enum.IsDefined(typeof(LocalDiagnosticPlatform),platform))throw new ArgumentException("DIAGNOSTIC_APP_METADATA_INVALID");
            BuildId=buildId;AppVersion=appVersion;Platform=platform;
        }
        public string BuildId { get; }
        public string AppVersion { get; }
        public LocalDiagnosticPlatform Platform { get; }
    }
    /// <summary>Only tighter limits may be selected; public maximums cannot be overridden.</summary>
    public sealed class LocalDiagnosticLimits
    {
        public const int MaximumEntries=1000;
        public const int MaximumBytes=1048576;
        public LocalDiagnosticLimits(int entries=MaximumEntries,int bytes=MaximumBytes)
        {if(entries<1||entries>MaximumEntries||bytes<512||bytes>MaximumBytes)throw new ArgumentException("DIAGNOSTIC_LIMIT_INVALID");Entries=entries;Bytes=bytes;}
        public int Entries { get; }
        public int Bytes { get; }
    }
    /// <summary>Immutable structured local row. No caller-supplied operation/transaction/correlation identifier exists.</summary>
    public sealed class LocalDiagnosticEntry
    {
        internal LocalDiagnosticEntry(string timestamp,long sequence,string severity,string code,string module,string build,string version,LocalDiagnosticPlatform platform,LocalDiagnosticOutcome outcome,LocalDiagnosticCategory category)
        {TimestampUtc=timestamp;Sequence=sequence;Severity=severity;EventCode=code;Module=module;BuildId=build;AppVersion=version;Platform=platform;Outcome=outcome;Category=category;}
        public string TimestampUtc { get; }
        public long Sequence { get; }
        public string Severity { get; }
        public string EventCode { get; }
        public string Module { get; }
        public string BuildId { get; }
        public string AppVersion { get; }
        public LocalDiagnosticPlatform Platform { get; }
        public LocalDiagnosticOutcome Outcome { get; }
        public LocalDiagnosticCategory Category { get; }
    }
    /// <summary>Only the currently retained window, never population metrics or an authoritative history.</summary>
    public sealed class LocalDiagnosticSnapshot
    {
        internal LocalDiagnosticSnapshot(IEnumerable<LocalDiagnosticEntry> entries)
        {
            Entries=Array.AsReadOnly(entries.ToArray());
            Breadcrumbs=Array.AsReadOnly(Entries.Skip(Math.Max(0,Entries.Count-32)).Select(e=>e.EventCode).ToArray());
            Aggregates=new System.Collections.ObjectModel.ReadOnlyDictionary<string,int>(Entries.GroupBy(e=>e.EventCode+":"+e.Outcome).ToDictionary(g=>g.Key,g=>g.Count(),StringComparer.Ordinal));
        }
        public IReadOnlyList<LocalDiagnosticEntry> Entries { get; }
        public IReadOnlyList<string> Breadcrumbs { get; }
        public IReadOnlyDictionary<string,int> Aggregates { get; }
    }
    internal readonly struct LocalDiagnosticSignal
    {
        internal LocalDiagnosticSignal(string code,string module,string severity,LocalDiagnosticOutcome outcome,LocalDiagnosticCategory category=LocalDiagnosticCategory.None)
        {Code=code;Module=module;Severity=severity;Outcome=outcome;Category=category;}
        internal readonly string Code,Module,Severity;
        internal readonly LocalDiagnosticOutcome Outcome;
        internal readonly LocalDiagnosticCategory Category;
    }
    internal static class LocalDiagnosticSchema
    {
        internal static LocalDiagnosticSignal? Event(LocalDiagnosticEvent value,LocalDiagnosticCategory category=LocalDiagnosticCategory.None)
        {
            switch(value)
            {
                case LocalDiagnosticEvent.AppStarted:return new LocalDiagnosticSignal("BLD-APP-STARTED","BLD","INFO",LocalDiagnosticOutcome.Succeeded);
                case LocalDiagnosticEvent.AppStopped:return new LocalDiagnosticSignal("BLD-APP-STOPPED","BLD","INFO",LocalDiagnosticOutcome.Succeeded);
                case LocalDiagnosticEvent.ProfileSynced:return new LocalDiagnosticSignal("SAV-PROFILE-SYNCED","SAV","INFO",LocalDiagnosticOutcome.Succeeded);
                case LocalDiagnosticEvent.RewardSaveFailed:return new LocalDiagnosticSignal("ADS-REWARD-SAVE-FAILED","ADS","ERROR",LocalDiagnosticOutcome.Failed);
                case LocalDiagnosticEvent.GrantSaveFailed:return new LocalDiagnosticSignal("IAP-GRANT-SAVE-FAILED","IAP","ERROR",LocalDiagnosticOutcome.Failed);
                case LocalDiagnosticEvent.RevocationSaveFailed:return new LocalDiagnosticSignal("IAP-REVOCATION-SAVE-FAILED","IAP","ERROR",LocalDiagnosticOutcome.Failed);
                case LocalDiagnosticEvent.SaveRecoveryRequired:return new LocalDiagnosticSignal("SAV-RECOVERY-REQUIRED","SAV","ERROR",LocalDiagnosticOutcome.Failed);
                case LocalDiagnosticEvent.UnexpectedException:return new LocalDiagnosticSignal("DOM-UNEXPECTED-EXCEPTION","DOM","ERROR",LocalDiagnosticOutcome.Failed,category==LocalDiagnosticCategory.None?LocalDiagnosticCategory.Other:category);
                case LocalDiagnosticEvent.FatalException:return new LocalDiagnosticSignal("DOM-FATAL-EXCEPTION","DOM","FATAL",LocalDiagnosticOutcome.Failed,category==LocalDiagnosticCategory.None?LocalDiagnosticCategory.Other:category);
                case LocalDiagnosticEvent.CorruptRingReset:return new LocalDiagnosticSignal("SAV-DIAGNOSTIC-RING-RESET","SAV","WARN",LocalDiagnosticOutcome.Failed);
                case LocalDiagnosticEvent.DebugBreadcrumb:return new LocalDiagnosticSignal("DOM-DEBUG-BREADCRUMB","DOM","DEBUG",LocalDiagnosticOutcome.Succeeded);
                default:return null;
            }
        }
        internal static LocalDiagnosticSignal? Service(LocalDiagnosticOperation operation,MobileServiceStatus status,string? code)
        {
            if(!Enum.IsDefined(typeof(LocalDiagnosticOperation),operation)||!Enum.IsDefined(typeof(MobileServiceStatus),status))return null;
            LocalDiagnosticOutcome outcome;
            switch(status){case MobileServiceStatus.Succeeded:outcome=LocalDiagnosticOutcome.Succeeded;break;case MobileServiceStatus.Unavailable:outcome=LocalDiagnosticOutcome.Unavailable;break;case MobileServiceStatus.NotAllowed:outcome=LocalDiagnosticOutcome.NotAllowed;break;case MobileServiceStatus.Cancelled:outcome=LocalDiagnosticOutcome.Cancelled;break;case MobileServiceStatus.TimedOut:outcome=LocalDiagnosticOutcome.TimedOut;break;case MobileServiceStatus.Pending:outcome=LocalDiagnosticOutcome.Pending;break;case MobileServiceStatus.Failed:outcome=LocalDiagnosticOutcome.Failed;break;default:return null;}
            if(code=="ADS_NO_FILL"&&(status==MobileServiceStatus.Unavailable||status==MobileServiceStatus.Failed)&&(operation==LocalDiagnosticOperation.Rewarded||operation==LocalDiagnosticOperation.Interstitial))outcome=LocalDiagnosticOutcome.NoFill;
            string module=operation==LocalDiagnosticOperation.Consent||operation==LocalDiagnosticOperation.PrivacyOptions?"CON":operation==LocalDiagnosticOperation.Rewarded||operation==LocalDiagnosticOperation.Interstitial?"ADS":"IAP";
            string name=operation==LocalDiagnosticOperation.PrivacyOptions?"PRIVACY-OPTIONS":operation==LocalDiagnosticOperation.StorePrice?"STORE-PRICE":operation.ToString().ToUpperInvariant();
            return new LocalDiagnosticSignal(module+"-"+name+"-RESULT",module,outcome==LocalDiagnosticOutcome.Failed||outcome==LocalDiagnosticOutcome.TimedOut?"WARN":"INFO",outcome);
        }
        internal static LocalDiagnosticSignal? Notice(string? code)
        {
            switch(code)
            {
                case "MOBILE_PROFILE_SYNCED":return Event(LocalDiagnosticEvent.ProfileSynced);
                case "ADS_REWARD_SAVE_RECOVERY_REQUIRED":return Event(LocalDiagnosticEvent.RewardSaveFailed);
                case "IAP_GRANT_SAVE_RECOVERY_REQUIRED":return Event(LocalDiagnosticEvent.GrantSaveFailed);
                case "IAP_REVOCATION_SAVE_RECOVERY_REQUIRED":return Event(LocalDiagnosticEvent.RevocationSaveFailed);
                default:return null;
            }
        }
        internal static LocalDiagnosticCategory Category(Exception value)=>value is UnauthorizedAccessException?LocalDiagnosticCategory.Access:value is System.IO.IOException?LocalDiagnosticCategory.IO:value is TimeoutException?LocalDiagnosticCategory.Timeout:value is OperationCanceledException?LocalDiagnosticCategory.Cancelled:value is NullReferenceException?LocalDiagnosticCategory.NullReference:value is InvalidOperationException?LocalDiagnosticCategory.InvalidOperation:LocalDiagnosticCategory.Other;
        internal static bool Known(string code,string module,string severity,LocalDiagnosticOutcome outcome,LocalDiagnosticCategory category)
        {
            if(!Enum.IsDefined(typeof(LocalDiagnosticOutcome),outcome)||!Enum.IsDefined(typeof(LocalDiagnosticCategory),category))return false;
            foreach(LocalDiagnosticEvent e in Enum.GetValues(typeof(LocalDiagnosticEvent))){var a=Event(e,category)!.Value;if(a.Code==code&&a.Module==module&&a.Severity==severity&&a.Outcome==outcome&&a.Category==category)return true;}
            if(category!=LocalDiagnosticCategory.None)return false;
            foreach(LocalDiagnosticOperation op in Enum.GetValues(typeof(LocalDiagnosticOperation)))foreach(MobileServiceStatus status in Enum.GetValues(typeof(MobileServiceStatus)))
            {var a=Service(op,status,outcome==LocalDiagnosticOutcome.NoFill?"ADS_NO_FILL":null)!.Value;if(a.Code==code&&a.Module==module&&a.Severity==severity&&a.Outcome==outcome)return true;}
            return false;
        }
    }
}
