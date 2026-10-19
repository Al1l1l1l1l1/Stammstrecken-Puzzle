# S-APP-03: canonical/hash work on application loop

High, reported to Root and owner. Bound application source e5f99270… is fully retained in S_INITIAL_CORRECTIONS_QC.json.

ApplyLoad invokes policy.Validate inside dispatcher publication; this performs cache JCS/SHA and ledger JCS/chains. Complete/PrepareForSave/NewProfile can also execute inline on the original caller when semaphore wait completes immediately. ConfigureAwait(false) does not itself offload. This conflicts with accepted T-details§35 worker requirement for canonical/hash work.

Request actual worker projection over captured immutable inputs, bounded application-loop publication and preserved exact ACK binding, with controlled canonical/ledger thread-ID evidence. No own runtime, source or Unity action; corrected source and regression remain pending.
