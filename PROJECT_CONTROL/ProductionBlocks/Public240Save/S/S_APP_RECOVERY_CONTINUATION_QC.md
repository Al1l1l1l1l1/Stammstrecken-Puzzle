# S-APP-04: no continuation after selected recovery winner

High, exact e5f99270… app source retained in S_INITIAL_CORRECTIONS_QC.json.

Own FirstRunX solvesA; commitConflict reloads winner where otherAttemptY alreadyclearedA. After explicitRecover+successful selectedpayloadPromotion, oldX remainsSolvedPendingCommit and completionCommittedfalse. Start/Restore guards reject it, retryX isModeInvalid because current mode isPractice/Revision, and Load also keeps oldsession. The confirmedwinner has no valid continuation through these entrypoints.

Request actual controlled regression through conflict/differentattemptwinner/recovery/promotion/newStart and explicit resolution of the old unacknowledged session after confirmed user recovery selection. No falseACK forX. Runtime repro/correction pending; no reviewer source/runtime/Unity/CI action.
