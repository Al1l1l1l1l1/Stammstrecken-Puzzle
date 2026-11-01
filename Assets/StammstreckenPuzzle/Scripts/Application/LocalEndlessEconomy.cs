using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Application
{
    /// <summary>Exact bounded ledger receipts; this helper never decides endless eligibility or terminal membership.</summary>
    public static class LocalEndlessEconomy
    {
        /// <summary>Copies one complete profile while retaining all unrelated immutable facts.</summary>
        public static LocalProfileState WithEndless(LocalProfileState state,LocalEndlessState endless)
            =>LocalProfileState.Create(state.Profile,state.Levels,endless,state.LedgerCheckpoint,state.EconomyJournal,state.Inventory,state.CosmeticClaimReservations,state.SeasonProgressCache,state.Drafts,state.RewardClaims,state.DailyStatus,state.Entitlements,state.PendingOperations,state.Settings,state.DataLifecycle,state.HintEntitlement).WithMetagame(state.Metagame);
        /// <summary>Removes only receipts no longer referenced by the actual current ledger or bounded tail.</summary>
        public static LocalProfileState Prune(LocalProfileState state)
        {
            var data=state.Endless;if(data.RecordVersion==1)return state;
            var references=new HashSet<string>(state.EconomyJournal.Select(e=>e.TransactionId).Concat(state.LedgerCheckpoint.JournalDiagnosticTail.Select(e=>e.TransactionId)),StringComparer.Ordinal);
            var kept=data.EconomyReceipts.Where(r=>references.Contains(r.TransactionId)).ToArray();
            return kept.Length==data.EconomyReceipts.Count?state:WithEndless(state,LocalEndlessState.Runtime(data.HighestReservedOrdinal,data.OpenEndless,data.RecentPublicInputs,kept));
        }
    }
}
