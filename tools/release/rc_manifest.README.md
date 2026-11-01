# RC metadata and manifest tools — ADR047

No command signs, uploads, tags, invokes Unity or grants public release permission. Install the existing pinned `tools/architecture-validation/requirements.lock.txt`. V1 schema/examples remain unchanged; `read_v1` never converts a record. V2 has 21 top-level fields and eight closed gates.

## Protected caller boundary

`AuthorizationContext(subject_immutable_jcs_sha256, original_hashes)` must come from the actual qualified protected review source. The subject is SHA256(JCS(all manifest fields except `evidenceGates`, `blockerState`, `promotionAllowed`)). Hashes are a purpose-specific mapping of the eight gate IDs and three blocker IDs to the authorized original bytes. The tool cannot establish the identity or competence of a human authority. Reading `acceptedBy`, a JSON role, a PASS string or an assessed provenance record never supplies this context. A foreign subject fails even when an acceptance Markdown body has identical bytes.

The CLI requires a **separate protected** `--trust-bundle` and its independently injected `--trust-bundle-sha256`. Its closed fields are:

- `receiptHashes`: exactly `runtimeBindings`, `signingVerification`, `storeReceipt`, each an independently established actual original SHA256. These are technical facts, not gate approvals.
- `historyReceiptHashes`: named, separately protected maps with those same three fields for each retained historical record. A renewed verification can have a new receipt hash for identical artifact bytes; current trust must not replace historical trust.
- `authorizationContexts`: named contexts, each null or `{subjectImmutableJcsSha256,authorizedOriginalHashes}`. `current` is required. History records select their separately retained contexts by name. Never generate this bundle or its expected digest automatically from the assessed request, reports or provenance. Protect the digest and qualify the original authorization source in the workflow environment.

The reader revalidates original hashes, scopes, actual artifact bytes, upload/delivery roles, and predecessor authority. It does not replace the existing ProductionImport/Human/Proof/SourceApproval pipeline. `runtimeBindings` is the actual independently pinned output of that pipeline. No DRAFT/ContractTest runtime enters this RC path.

## CLI

`python tools/release/rc_manifest_cli.py` provides:

- `identity-from-ci --expected-app APP --platform ANDROID|IOS --out NEWFILE`: actual `GITHUB_ACTIONS=true`, tag ref, exact rc tag/commit/workflow SHA, positive run/attempt required. Recipe is `release.yml`; number is `runNumber*1000+attempt`, attempt 1–999. Both platforms share one run/attempt number. It creates neither a tag nor a release.
- `identity --identity FILE --identity-sha256 SHA --expected-app APP --ci-build-number N`: read only, exact nine fields.
- `build-info --identity FILE --identity-sha256 SHA --expected-app APP --ci-build-number N --built-at-utc UTC --unity-version V --package-lock FILE --content-lock FILE --save-schema-version N --level-schema-version N --solver-version V --workflow-run-id ID --out NEWFILE`.
- `build --request FILE --request-sha256 SHA --source-root ORIGINALS_ROOT --artifact ACTUAL_AAB_OR_IPA --expected-app APP --ci-build-number N --trust-bundle FILE --trust-bundle-sha256 SHA --schema-root REPO --out NEW_DIRECTORY`.

Expected app and CI values must come from the protected actual profile/job, not from the assessed identity. Unknown/invalid input returns 2. No original bodies, environment or passwords are printed. Outputs never overwrite files. No `build` output exists before a genuine signing verification and store receipt; unsigned AAB and iOS source exports stay separate C packaging receipts.

## Closed internal build request and provenance

Request fields: `buildRequestVersion:1`, `identity`, `inputs`, `evidence`, `blockers`, `history`. Identity is exactly `releaseVersion,candidateTag,candidateOrdinal,gitCommit,workflow,platform,buildNumber,buildProfile,applicationIdentifier`. Production only. Tag/version/ordinal agree, workflow has a recipe SHA, Android number ≤2100000000. This request is not an approval.

`inputs` has exactly nine `{path,rawSha256}` original references: `toolchainLock,packageLock,releaseLock,approvalLock,productionConfig,blockersSource,runtimeBindings,signingVerification,storeReceipt`. Paths are forward-slash relative paths under ORIGINALS_ROOT; traversal, links/reparse ancestors and key containers are rejected. Original bytes are rechecked. Toolchain/package/config manifest hashes are raw; content hash is the existing release-lock JCS hash. Its original source commit stays distinct from the candidate commit. Actual release membership is 240 campaign identities, six catalogs and four named quality profiles.

`evidence` has exactly the eight ADR047 gate IDs; `blockers` exactly the three known IDs. Each entry is null (open) or `{original,bindings,resources}`. `original` is an exact reference, `resources` is a nonempty unique reference list, and `bindings` is the exact candidate-specific scope returned by `rc_manifest._expected_scope`. Source scope includes actual release/approval/runtime/UI/calibration/all four profile hashes. Device/store/provider/owner/independent scopes preserve the upload identity and actual delivered binary/signer list. Rights scope binds actual content/package/config and preserved resources. Hint/daily blocker scopes bind the real integrated candidate/provider configuration; endless also binds the complete calibrated source scope. SIGNING_PACKAGE's original must be the actual C signing verifier report. A caller boolean cannot close any gate or blocker.

Store receipt fields are exactly `storeReceiptVersion:1,identity,storeBuildReference,uploadArtifactSha256,uploadSigningFingerprintSha256,deliveredArtifacts,mappingOriginal,previousStoreBuildNumber,previousStoreState,previousStoreVersionOriginal`. Each delivered artifact is `{path,rawSha256,signingFingerprintSha256}`. Mapping/previous-version originals are exact references. Previous state is `KNOWN_VERSION` with a lower positive prior number, or `INITIAL_RELEASE_CONFIRMED` with null; both require a real independently pinned store receipt and original query. Missing account/version is never treated as initial release. Upload AAB/IPA and delivered APK/splits/processed app may have different bytes and signer roles. The actual qualified store mapping is retained; no store delivery is inferred by the tool.

History is oldest first, each `{manifest,provenance,authorizationContextKey,receiptTrustKey}`. Both references are exact originals; genesis has no predecessor. Every retained predecessor is revalidated, bounded to 256 records, including its separate authorization context and separately pinned technical receipt map. The Python API represents that immutable caller-supplied bundle as `RetainedManifest(manifest_raw,provenance_raw,authorizations,trusted_receipts,predecessor=None)`. These capabilities are never obtained from the assessed provenance. Only the three ADR047 fields may change. Provenance contains exactly `provenanceVersion:1,manifestRawSha256,manifestJcsSha256,predecessor,inputs,evidence,blockers`; predecessor is null or `{rawSha256,jcsSha256}`. A successor never overwrites the original manifest/artifact. The current public action still needs the actual qualified manifest hash; this tool performs no promotion.

The output directory contains `release-manifest.json`, `release-provenance.internal.json` and byte-preserved `originals/`. `INCOMPLETE` remains on failed output assembly; it grants nothing. Both manifest hashes and every original are verified. Promotion is computed from all eight VERIFIED gates plus all three evidence-closed blockers, production/debug/signing/store/lock conditions. A false candidate may later acquire an append-only evidence successor. No closed state is silently inherited without its actual authority.

## Build information

The exact 14 fields are `buildInfoVersion:1,releaseVersion,buildNumber,gitCommit,candidateTag,builtAtUtc,unityVersion,packageLockSha256,contentLockSha256,saveSchemaVersion,levelSchemaVersion,solverVersion,workflow,workflowRunId`. Schema versions are positive integers (not booleans); build number and run ID are positive decimal strings. Time is actual UTC `YYYY-MM-DDTHH:mm:ssZ`. `read_build_info` compares all fields with independently supplied actual identity/toolchain/version facts. C/Root inject these bytes only in generated native outputs and check the final AAB/IPA; no repository StreamingAssets or app data is created here.

## Controlled checks

`STP_REPO_ROOT=... python tools/release/test_rc_manifest.py` executes controlled contract fixtures, including a fake external oracle explicitly labeled as such. All fixture store references/certificates/acceptances are synthetic, not actual signing, human, legal, store or production evidence. Tests include the executable CLI and self-contained copied-original readback. Actual keys, qualified approvals, signed builds, store channels and public actions remain external work.
