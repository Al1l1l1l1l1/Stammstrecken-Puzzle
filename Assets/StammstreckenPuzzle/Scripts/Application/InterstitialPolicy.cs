using System;
using System.Collections.Generic;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Three independently paced completion modes sharing a single process-session advertisement cap.</summary>
    public enum InterstitialCompletionMode { CampaignFirstClear, Revision, Endless }

    /// <summary>Captured only from the exact acknowledged currently solved attempt; UI presentation remains a separate live gate.</summary>
    public sealed class InterstitialResultReceipt
    {
        private InterstitialResultReceipt(SaveProfileIdentity profile, CatalogIdentity catalog, long generation, PublicPuzzleIdentity identity,
            Guid attemptId, InterstitialCompletionMode mode, int campaignFirstClearCount)
        { Profile = profile; Catalog = catalog; Generation = generation; Identity = identity; AttemptId = attemptId; Mode = mode; CampaignFirstClearCount = campaignFirstClearCount; }
        public SaveProfileIdentity Profile { get; }
        public CatalogIdentity Catalog { get; }
        public long Generation { get; }
        public PublicPuzzleIdentity Identity { get; }
        public Guid AttemptId { get; }
        public InterstitialCompletionMode Mode { get; }
        public int CampaignFirstClearCount { get; }

        /// <summary>Neither a proposed save, a terminal watermark nor a public DTO can manufacture completion authority.</summary>
        public static InterstitialResultReceipt FromCommittedResult(PersistedPuzzleSession use)
        {
            if (use == null || !use.CanPresentResults || use.Snapshot == null || use.Session == null) throw new ArgumentException("INTERSTITIAL_CONFIRMED_RESULT_REQUIRED");
            var saved = use.Snapshot; var session = use.Session;
            var identity = new PublicPuzzleIdentity(session.PuzzleId, session.PublicPuzzleHashProfile, session.PublicPuzzleHashSha256);
            InterstitialCompletionMode mode;
            var level = saved.Payload.Levels.SingleOrDefault(item => item.Identity.Equals(identity));
            if (session.Mode == SessionMode.REVISION && level?.FirstClear != null) mode = InterstitialCompletionMode.Revision;
            else if (session.Mode == SessionMode.FIRST_RUN && level?.FirstClear?.AttemptId == session.AttemptId) mode = InterstitialCompletionMode.CampaignFirstClear;
            else if (session.Mode == SessionMode.ENDLESS && saved.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Any(item => item.Id == identity.PuzzleId && item.CompletionCommitId == session.SolvedEventId))
                mode = InterstitialCompletionMode.Endless;
            else throw new ArgumentException("INTERSTITIAL_PERSISTED_CLEAR_BINDING_REQUIRED");
            return new InterstitialResultReceipt(saved.Profile, saved.Catalog, saved.Generation, identity, session.AttemptId, mode,
                saved.Payload.SeasonProgressCache.Subjects.Where(item => item.Kind == "SEASON").SelectMany(item => item.FirstClearPuzzleIds).Distinct(StringComparer.Ordinal).Count());
        }
    }

    /// <summary>Once-only ephemeral load/show opportunity; it grants neither reward nor progress.</summary>
    public sealed class InterstitialPermit
    {
        internal InterstitialPermit(Guid attemptId) { AttemptId = attemptId; }
        public Guid AttemptId { get; }
        internal bool ShowBudgetConsumed;
    }

    /// <summary>Concept08 §2 pacing. One instance lives for the complete app session, across menus, focus changes and modes.</summary>
    public sealed class InterstitialPolicy
    {
        private readonly SaveProfileIdentity profile;
        private readonly CatalogIdentity catalog;
        private readonly HashSet<Guid> consumedAttempts = new HashSet<Guid>();
        private readonly HashSet<string> endlessIdentities = new HashSet<string>(StringComparer.Ordinal);
        private readonly object gate = new object();
        private InterstitialPermit? pending;
        private int showBudgetConsumed, revisionCompletions, endlessCompletions;
        private long latestGeneration;

        public InterstitialPolicy(SaveProfileIdentity profile, CatalogIdentity catalog)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile)); this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (profile.Use != catalog.Use) throw new ArgumentException("INTERSTITIAL_PROFILE_CATALOG_MISMATCH");
        }
        /// <summary>Conservative count of native show starts, including an uncertain timeout; no-fill loads do not consume this cap.</summary>
        public int SessionShowBudgetConsumed { get { lock (gate) return showBudgetConsumed; } }

        /// <summary>Called only for an explicit Continue after fully presented train ride/results. Missed opportunities are consumed, never queued.</summary>
        public bool TryTake(InterstitialResultReceipt receipt, bool completeResultVisible, bool explicitContinue, bool canRequestAds, bool removeAds,
            out InterstitialPermit? permit, out string code)
        {
            permit = null;
            if (receipt == null || !receipt.Profile.Equals(profile) || !receipt.Catalog.Equals(catalog) || receipt.AttemptId == Guid.Empty || receipt.Generation < 1)
            { code = "INTERSTITIAL_RESULT_BINDING_INVALID"; return false; }
            lock (gate)
            {
                if (!completeResultVisible || !explicitContinue) { code = "INTERSTITIAL_RESULT_BOUNDARY_REQUIRED"; return false; }
                if (receipt.Generation < latestGeneration || !consumedAttempts.Add(receipt.AttemptId)) { code = "INTERSTITIAL_RESULT_ALREADY_CONSUMED"; return false; }
                latestGeneration = receipt.Generation;
                bool due;
                switch (receipt.Mode)
                {
                    case InterstitialCompletionMode.CampaignFirstClear:
                        int count = receipt.CampaignFirstClearCount;
                        due = count == 2 || count >= 3 && count <= 8 && count % 2 == 0 || count >= 9 && (count - 8) % 3 == 0;
                        break;
                    case InterstitialCompletionMode.Revision:
                        revisionCompletions++; due = revisionCompletions % 5 == 0; break;
                    case InterstitialCompletionMode.Endless:
                        if (!endlessIdentities.Add(receipt.Identity.PuzzleId + ":" + receipt.Identity.HashSha256)) { code = "INTERSTITIAL_ENDLESS_ALREADY_KNOWN"; return false; }
                        endlessCompletions++; due = endlessCompletions % 3 == 0; break;
                    default: code = "INTERSTITIAL_MODE_UNSUPPORTED"; return false;
                }
                if (receipt.CampaignFirstClearCount + endlessCompletions < 2) { code = "INTERSTITIAL_TWO_CLEARS_REQUIRED"; return false; }
                if (!due) { code = "INTERSTITIAL_NOT_DUE"; return false; }
                if (removeAds) { code = "INTERSTITIAL_REMOVE_ADS"; return false; }
                if (!canRequestAds) { code = "INTERSTITIAL_CAPABILITY_REQUIRED"; return false; }
                if (showBudgetConsumed >= 3) { code = "INTERSTITIAL_SESSION_CAP"; return false; }
                if (pending != null) { code = "INTERSTITIAL_OPERATION_PENDING"; return false; }
                permit = new InterstitialPermit(receipt.AttemptId); pending = permit; code = "INTERSTITIAL_ELIGIBLE"; return true;
            }
        }
        /// <summary>Recheck current consent, remove_ads and the live result boundary directly before invoking the SDK show.</summary>
        public bool BeginShow(InterstitialPermit permit, bool completeResultVisible, bool canRequestAds, bool removeAds)
        {
            lock (gate)
            {
                if (!ReferenceEquals(pending, permit) || permit.ShowBudgetConsumed || !completeResultVisible || !canRequestAds || removeAds || showBudgetConsumed >= 3) return false;
                permit.ShowBudgetConsumed = true; showBudgetConsumed++; return true;
            }
        }
        /// <summary>Once this explicit opportunity finishes or no-fills, no following completion inherits it.</summary>
        public void Release(InterstitialPermit permit) { lock (gate) { if (ReferenceEquals(pending, permit)) pending = null; } }
    }
}
