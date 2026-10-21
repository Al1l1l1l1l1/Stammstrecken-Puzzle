# ProductionImport

Headless authoring import over the actual STP strict JSON/JCS, Level-v2 loader, fresh `LevelValidationPipeline`/solver-v2/proof regeneration and accepted ADR044 readers. It does not alter `project_candidate.cjs`, whose DRAFT/null-threshold isolation remains intact. No Unity process or Application→Content edge is introduced.

Build after the B44/Root search union:

```powershell
dotnet build tools/campaign/ProductionImport/ProductionImport.csproj -p:StpRepositoryRoot=C:/path/to/exact-checkout
dotnet .utmp/wp046-production-import/bin/net10.0/ProductionImport.dll --source-root C:/path/to/exact-checkout --request Content/Release/actual-import-request.json --out C:/path/to/NEW-output --trusted-approval-lock Content/Release/actual-source-approval.json --trusted-approval-sha256 EXPECTED_ORIGINAL_RAW_SHA_FROM_PROTECTED_RELEASE_CONFIGURATION
```

The trusted approval path/hash are separate release-configuration inputs, never fields in the request, a UI DTO or a caller approval boolean. Production checks the original approval source, all exact source bytes, accepted evidence and the actual release lock itself. The hash identifies an independently accepted original; possession of a CLI argument does not attest a person or produce a source acceptance. CI/build separately owns the actual Gitcommit/toolchain/clean checkout, platform configuration and downstream release gates. The importer validates their exact source bindings, not a store account or device.

All request paths are repository-relative forward-slash paths, no `..`, root, drive, backslash, reparse point or duplicate JSON key. Individual originals ≤16MiB, read-once snapshot ≤8192 files/512MiB. Outputs refuse any existing path. Build artifacts stay under `.utmp/wp046-production-import/{obj,bin}`; no `obj/bin` in tools. `ProductionImport.csproj` may need explicit Git inclusion under the repository ignore rule.

## Exact request

See `import-request.schema.json`. Root fields, all required and closed:

```
{importRequestVersion:1,use,publicCatalog:{catalogId,revision},
 campaign,campaignCompletion,endlessCompletion,calibration,cosmetics,
 profiles:[four source paths],levels:[240 {level,proof}],localizations:[source paths],
 railwayAssets,releaseLock,toolchainLock,uiSourceLock,evidence:[source paths],
 publishedLocks:[{lock:{path,rawSha256},catalogs:[{path,rawSha256}]}]}
```

`use` is `Production` or isolated `ContractTest`. Request catalog identity is projected from actual typed levels; the computed public selfhash must match the release lock and every source reference. The release lock is existing strict v1, containing exactly ten actual catalog/profile hashes (six catalogs, including two full wrappers, and four profiles), all240 full level/proof bindings, actual Gitcommit and actual raw toolchain-lock hash. Previous published locks plus all their actual catalog originals enforce append-only public identities and monotone revisions: same catalog ID/revision cannot receive another hash. A lock without its referenced historical originals fails closed.

The 240 levels must contain real nonnull thresholds, exactly matching the full target-Level-JCS hash/version/seconds in calibration. Each proof is freshly regenerated from only public inputs and byte-compared; rehashing a false cache cannot pass. Full named5/20/240 campaign graph and both completion scopes/rewards are validated by the existing readers. All level completion, world/map/train, cosmetic/milestone, actual P06 prices and player/internal localization references are resolved; missing refs never fall back. Original RailwayAssets fractional values use its bounded typed float JSON reader and exact rawSHA; integer-JCS is unchanged.

The internal Evidence/ApprovalLock forms and narrow measured-UI metadata transfer are normative ADR044. Sources include actual retained old snapshots, full nine-pack54-case campaign sample, at least12 distinct included original profile cases per profile, original exports with exclusions, logic criteria, transfer methods and independent source acceptance. `HumanReadback.SourceBundle`, the actual ADR042 codecs, cross-export duplicate controls and fresh solver/GQP checks are reused. Automated-origin, assisted/uncertain/incomplete observations never become Production measurements. Root calibration versions/UI refs bind every target row; old source raw/JCS hashes are distinct from new target-document hashes. Evidence uses full JCS hash; original file references additionally use raw hashes. No evidence→full target profile/calibration/wrapper hash cycle is introduced.

Measured UI source hash always stays the actual old UI lock. Target UI lock is separately bound. Only the accepted exact75-file metadata-only DRAFT→approved case is supported: all other source bytes identical; RailwayAssets only top approvalStatus; CampaignText only top approvalStatus/sourceSha256 with all720 strings unchanged. Float number tokens compare exactly without rounding. Source originals and explicit independent acceptance remain required. No code/layout/timing/art/text transfer is inferred.

## Outputs

`player/`: exact twelve payloads plus `Runtime.bindings.json`, following accepted ADR044 Runtimebindings-v1: `Public.json`, `Campaign.json`, `CampaignCompletion.json`, `EndlessCompletion.json`, `Calibration.json`, `Cosmetics.json`, `Profiles/{dienstbeginn,regelbetrieb,stoerungslage,grosslage}.json`, `RailwayAssets.json`, `CampaignText.json`. Every integer JSON output is existing JCS; RailwayAssets is byte-identical to its validated original. Runtimebindings has no selfhash; Root verifies its actual rawSHA through the protected build/scene input and all twelve payload raw hashes.

CampaignText is the existing `{schemaVersion,locale,approvalStatus,sourceSha256,entries:[240 {puzzleId,title,status,result}]}` projection. All720 referenced values must come from one actual original localization source, whose rawSHA is sourceSha256. No internal deduction text, solution, proof, proxy, participant export or evidence is shipped. The current UIlocale remains a separate UI source protected by the full UI lock. Technical FIXTURE_ONLY texts require an isolated ContractTest reader; they must not broaden the existing production text reader.

`authoring/source-bindings.json`: all exact original raw hashes, full target-level JCS/public/proof identities, and separate actual old-source→target transfers with measured/target UI refs and direct-vs-transferred classification based on original identity/source, not a caller flag. `authoring/player-raw-lock.json`: exact deterministic output bytes. Authoring files never become player assets.

## Technical tests and real remaining gates

```powershell
dotnet .utmp/wp046-production-import/bin/net10.0/ProductionImport.dll --source-root C:/path/to/exact-checkout --self-test .utmp/NEW-unique-self-test-directory
```

The test generator writes only explicitly synthetic `FIXTURE_ONLY` catalogs/levels and `SYNTHETIC_CONTRACT_TEST_NO_HUMAN_EVIDENCE`, with synthetic2/1-second **test** boundaries; it writes no PRODUCT_APPROVED document, participant run or approval. Metadata-promotion comparisons use in-memory strings only. Every retained attempt directory is immutable; successful cases include full240 actual typed/fresh-proof import and byte-identical repeat, while mutations cover nonnull/time/source/identity/reward/crossrefs/float/duplicate/lock/history/trust/path cases. The CLI reports only the scope actually exercised; these are headless self-tests, no Unity/render/device claim.

Current repository inputs remain DRAFT/null. There is no real calibration catalog, no accepted target GQP files, no actual human export/evidence/independent acceptance, no actual Production release/approval lock, and no protected Production release configuration. Current cosmetic catalog `cosmetic.*.title` refs additionally need an explicit accepted source resolution against UI keys currently named `product.cosmetic.*.title`; the importer does not invent aliases. The synthetic test generator makes its own clearly test-only aliases. These are real source/data gates, not missing import code. Production compilation/mount/build still belongs to R/B; native devices, signing, store/legal and Owner acceptance remain separate.
