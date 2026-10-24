# RC packaging integration (staged; no actual game signing or Unity build)

The accepted ADR047 is the authority for the new packaging stages. Historical release-manifest-v1 is unchanged. This group creates technical native/packaging receipts only; it never creates a StoreBuildReference, upload, tag or promotion.

## Unity entries and required inputs

- `STP.Editor.Build.ProductionNativeBuild.BuildAndroidReleaseBundle`
- `STP.Editor.Build.ProductionNativeBuild.ExportIosReleaseSources`

Call Unity with the explicit `-buildTarget Android` or `-buildTarget iOS` before assembly load. All existing ProductionImportPreflight / source approval / protected service preset / IAP review / publisher original inputs remain mandatory. Added env:

- `STP_RC_IDENTITY`: absolute path to Bs exact nine-field identity; `STP_RC_IDENTITY_RAW_SHA256`.
- `STP_RC_BUILD_INFO`: absolute path to Bs exact fourteen-field original; `STP_RC_BUILD_INFO_RAW_SHA256`.
- `STP_RC_NATIVE_OUTPUT_ROOT`: absolute nonexistent child of `RUNNER_TEMP`; use a separate fresh root per platform/attempt.
- Real GitHub tag-job fields: `GITHUB_ACTIONS=true`, `GITHUB_REF_TYPE=tag`, `GITHUB_REF_NAME`, `GITHUB_SHA`, `GITHUB_WORKFLOW_SHA`, `GITHUB_RUN_NUMBER`, `GITHUB_RUN_ATTEMPT`, `GITHUB_RUN_ID`.

Identity numbering is `run_number * 1000 + run_attempt`, attempt 1–999, result <=2100000000. This binds CI identity; it does not prove monotonicity against an unknown store. Bs separate store-state reader owns the initial/known actual original comparison. Both platform jobs in the same run/attempt receive the same integer. The Store version is X.Y.Z; only the tag contains -rc.N.

C# validates closed identity/buildinfo before mutation. A small `PrepareForNativeBuildWithFacts` seam compares the actual importer Release-JCS before AssetDatabase/preset writes. Native scene/profile/SDK gates remain the original ones. Temporary version/number/build flags/symbol/GMA settings are restored in finally even on failure. Release scopes are distinct from Development scopes.

Android clears the generated release signingConfig explicitly, checks it before package/bundle tasks and rejects any final AAB signature marker. `useCustomKeystore=false` alone is debug signing and is never the unsigned claim. Existing actual two-merge native/privacy checks also run on bundle tasks. The generated build-info asset has a fixed owned marker, exact bound original, fail-closed unknown/modified handling and nonrelease incremental cleanup. No repo StreamingAssets are introduced. iOS inserts exactly one generated PBX app resource after export.

Known native outputs below the fresh root:

- Android: `stp-production-unsigned.aab`, actual Unity `*.symbols.zip` (Full+Zip), `native-rc-receipt.json`, `native-inputs.json`, `build-info.original.json`, and `android-native-originals/` with exact original merge reports/manifests.
- iOS: `Xcode/` plus those three JSON originals. This remains unsigned source, no IPA claim.

Buildinfo is actually packaged at AAB `base/assets/build-info.json` / IPA `Payload/<app>.app/build-info.json` and must match every original byte. Native input index hashes every exported input before downstream packaging. The optional externally pinned Podfile.lock is the only permitted pre-Pods additional source file. Pods may change the generated PBX; all other original source bytes are rechecked. The manual signing delta changes only generated application/UnityFramework settings; the application provisioning profile is not passed as a global override to frameworks or Pods.

## Executable packaging CLI

`python tools/release/rc_package.py android-sign` common required arguments:

`--identity PATH --identity-sha256 SHA --native-receipt PATH --native-receipt-sha256 SHA --output FRESH --certificate-sha256 UPLOAD_CERT_SHA`

Android adds `--jdk UNITY_OPENJDK --bundletool LOCAL_JAR --bundletool-sha256 SHA --keystore TEMP_UPLOAD_KEY --key-alias ALIAS`. Optional `--signature-algorithm SHA256withRSA|SHA256withECDSA` (RSA default). Passwords ONLY through `STP_RC_KEYSTORE_PASSWORD` / `STP_RC_KEY_PASSWORD` and jarsigner env-source switches. No keystore/certificate/private-key generation, external login or installation occurs. Other subprocesses receive a minimal environment with no GitHub token/signing passwords/JAVA_TOOL_OPTIONS. Signed output: `stp-production-signed.aab`.

Use the installed Unity bundletool rather than downloading one: `Tools/bundletool-all-1.17.2.jar`, expected raw SHA256 `2d4ad908faea64047c1cc9cb747e6aa667c6ab192e09607bd16b67246a8cd6ae` (Root independently located this binary). Pin the actual JDK/toolchain originals in Bs manifest request.

`ios-package` uses the common arguments and adds `--xcodebuild PATH --pod PATH --podfile-sha256 SHA --pod-lock-sha256 SHA --team TEAM --profile-uuid UUID --keychain TEMP_KEYCHAIN --certificate PUBLIC_DER`. macOS only, Xcode/iOS SDK >=26, existing installed pinned CocoaPods. It runs `pod install --deployment --no-repo-update`, workspace archive, then export with `destination=export`; no allowProvisioningUpdates, automatic login, upload or device registration. A real approved Podfile.lock is required; current QA exports are not a substitute. A locked resolution newly obtained by ordinary CI still needs its actual Production lock review before this path.

The keychain/profile/private material are preinstalled by the protected CI step. The keychain must be under RUNNER_TEMP, with the pinned certificate available. This CLI never imports or deletes shared credentials. Root workflow owns isolated setup, authorization, and unconditional cleanup. Keep keychain/profile/P12/keystore outside artifact upload roots. Never serialize them into Unity settings or logs.

Both commands return 0 only after actual signed artifact verification, required symbols and receipts; input/gate/tool errors return 2 with a stable code. Partial failed output is preserved; never retry into it. Timeouts attempt termination of the owned process group/tree and bound pipe drain; the protected runner must also clean up any remaining descendant; uncertain completion never emits success. Full redacted diagnostics/exit codes stay local; private signer stdout is never treated as approval.

Read-only validators: `android-verify` adds `--signed-artifact PATH` instead of keystore/alias; `ios-verify` adds `--signed-artifact PATH --team TEAM --profile-uuid UUID --archive PATH --archive-index PATH --archive-index-sha256 SHA`. The last three arguments identify the retained actual .xcarchive and its pre-export `archive-inputs.json` with a protected expected raw SHA. They never read signing secrets. Both reuse original bound native metadata. Android readonly adds optional `--native-output-root DOWNLOADED_NATIVE_ROOT`; the receipt must reside immediately inside that root, with its unchanged native-inputs.json/build-info sidecars, original AAB, symbol ZIPs and android-native-originals subtree. Every used file must be indexed and match its original raw SHA. Old Gradle paths are interpreted lexically as their recorded Windows/POSIX grammar and remapped only into those indexed copies; the old Runner/Gradle root is never required and no receipt/report bytes are rewritten. Android signing uses the original native output without an override. Archive the entire native output for transfer. Read-only identity validation is independent of the current CI run; all actual package/certificate/metadata must still match the original identity.

`VerifyBundleSignatures.java` verifies each actual JAR payload, including nonsignature META-INF entries, exact single signer and certificate pin/validity. No debug subject, unsigned/additional signer, missing entry or payload mutation passes. IPA verification inspects the final exported IPA, not only its archive: codesign, actual certificate, closed entitlements, exact profile/app/version, ARM64, native privacy fields and original build-info bytes. The final IPA is matched against its source-bound archive: exact app file set/resources, every thin ARM64 Mach-O UUID and all code bytes except the structurally validated re-signing signature area/LINKEDIT size. Main and UnityFramework dSYMs must match the final IPA UUIDs; this is not a completed crash-symbolization/device test.

## Portable receipt seam for B

`packaging-attestation.json`: exact version1 fields agreed with B; stage SIGNED_CANDIDATE only after signing/actual verification; evidence/symbol/tool references use safe relative forward-slash paths under its output root.

`signing-verification.json`: `{verificationVersion:1,kind:ACTUAL_SIGNED_ANDROID_AAB|ACTUAL_SIGNED_IOS_IPA,identity:<9>,artifactSha256,signingFingerprintSha256,checks:[ACTUAL_ARTIFACT_SIGNATURE,ACTUAL_PACKAGE_VERSION,ACTUAL_NATIVE_PRIVACY,IDENTITY_BOUND],evidence:[{path,rawSha256}]}`. Evidence original hashes must be independently authorized by the actual protected source for B's SIGNING_PACKAGE gate; file existence/header claims never confer approval. AAB fingerprint is upload signer, distinct from Play's delivered APK signer. No AAB or IPA here has been signed with a real key or accepted for distribution.

Root release.yml owns protected-environment gating, immutable tag/commit/workflow/actual prior-store-state binding, .NET10 importer runtime, complete source and toolchain provenance, original SDK/licenses/SBOM, credential cleanup, source/owner/legal/device/provider/store/RC approvals, and later independently authorized store actions. The Python tool intentionally supplies no promotion/store manifest and no store upload implementation. No Unity build, actual game signing or Xcode execution is claimed by controlled tests.

## Symbol and obfuscation boundary

Android native-inputs pins both the actual unsigned AAB and every emitted symbols ZIP. Sign and readonly verification require the core libmain/libunity/libil2cpp GNU build IDs to match actual AAB ELF bytes and nonempty ELF symbol tables, then preserve the original ZIPs. R8 is affirmatively rejected in the RC-only Unity and generated Gradle paths while an approved obfuscation smoke/mapping pipeline is absent; it is never silently disabled. The private native receipt records androidMinifyRelease=false (null on iOS), and readonly validators require that fact.

iOS archives are inventoried immediately after successful archive, before export. The closed archive index binds identity, nativeReceiptRawSha256 and every archive file; readonly verification rechecks every original and compares the actual final IPA against that archive. The exact pre-signing PBX bytes are restored in finally after archive/export success or failure. No actual Mac archive/IPA or Unity AAB has been executed by these controlled tests.

Official source references: [Unity Android symbols](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/developing/testing-and-debugging/symbols), [Apple UUID/dSYM binding](https://developer.apple.com/documentation/technotes/tn3178-checking-for-and-resolving-build-uuid-problems). The actual installed Unity6000.3 APIs are separately compiled in all three editor branches; a newer manual is contextual evidence and does not replace that source/API check.

Readonly iOS reads source metadata only from the supplied receipt directory; it performs no I/O against the recorded old Xcode/source-export path. Its separately supplied actual .xcarchive and protected archive index remain mandatory. Preserve the complete native sidecars and packaging archive for transfer.
