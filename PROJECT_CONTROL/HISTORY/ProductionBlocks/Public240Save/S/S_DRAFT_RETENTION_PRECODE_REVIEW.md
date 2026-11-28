# Narrow20draft retention pre-code review

Permitted within existing20draft slots, with one necessary detail: the current offered draft must be retained whenevercapacityexists, or a named stop must precede any write. PureUTC-desc ordering can otherwise evict the new current draft after clock rollback/future-dated oldrecords, yet wrongly acknowledge autosave success.

Retain every orphan unchanged and count it within20. Reserve current exact bound draft, choose remaining known older candidates by UTCinstant desc plus ordinal identitytriplet tie-break; serialize deterministically. With20orphans and a nonmatching new draft, stopcapacity. SamePuzzleId orphan/newhash collision also stops, rather than overwriting orphan or adding a new bag.

Only successful verifiedcommit makes retention durable. Failedwrite keeps old durable20 and current dirty/inmemory truth. Required targeted tests:21stcurrent, UTCrollback/equalinstant/tie,19/20orphans, identitycollision and failedcommit.

This concretizes PERSISTENCE§4/S-plan§2 within final scope; R records chosen detail beforecode. No new producttimepolicy, runtime, fullS or CI PASS.
