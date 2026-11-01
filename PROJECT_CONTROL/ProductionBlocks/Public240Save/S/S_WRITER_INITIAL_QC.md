# Independent initial S filesystem/repository review

Two concrete findings accepted for correction; no writer PASS. Full reviewed sources are retained in S_WRITER_REVIEW_SOURCE_BINDING.json (FS c6327d…, repository8fe43cb…).

S-FS-01 High: direct CreateNew write to immutable final archive filename can leave partial bytes after crash. Later legal same-source recovery/migration retries permanently fail existing-byte mismatch. Root accepted same-directory unique staging, persistent flush, complete rereads and immutable final Move, with retry/finalization regression windows. Original candidate data was protected; this is recovery liveness failure.

S-REPO-01 Medium: initial actual OS diskfull HRESULT falls into generic IO code, contrary to promised distinct Lowdisk code. Root confirms later source already maps Windows112/39 and will add ENOSPC28 plus numeric-exception tests. Reviewed old source is kept distinct from current patches.

Full Load/Recover/CAS/one-shot lease, highest regular-copy safety, sole destructive Pending site, backup rotation, exact verified main acknowledgement, uncertain-effect cancellation, source preservation and private path boundaries were read. No further concrete High identified in this bound snapshot.

Actual crash/cancellation/platform/resource tests and exact correction review remain required. Reviewer did not alter source, Git, Unity or CI.
