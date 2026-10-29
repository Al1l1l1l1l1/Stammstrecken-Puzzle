# Independent initial S code review

Open concrete findings; no S implementation PASS. Exact six early source copies and hashes are retained in S_EARLY_CODE_SOURCE_BINDING.json. No source/Git/Unity/CI action by reviewer.

- S-APP-01: Start/Restore could replace solved A before completion ACK and let old ACK release new B. Owner first correction is statically present in bound0b0acca source; actual controlled regression remains pending. Original faulty full text was read but no pre-correction rawcopy captured.
- S-MODEL-01 High: CatalogSetIdentity.Create accepts repeated identical parts with correct aggregatehash, contrary to accepted plan§11 three different identities. Owner informed; pairwise equality check plus validhash negative requested.
- S-APP-02 High: AlreadyCommitted branch retains delayed mutable-session closure. Repeat completion on committed A, delay dispatcher, Start+SolveB, run old callback can publish B as committed without repositoryCommitB. Exact immutable capture/stale callback guard and delayed-dispatch regression requested.

Full checkpoint, immutable record model, policy, app/autosave and review catalog were statically read. Codec/migration/tests remain under construction and are not prematurely assessed as missing. Full runtime/originalgolden/testing/freeze gates remain open.
