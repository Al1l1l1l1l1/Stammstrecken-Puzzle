#!/usr/bin/env bash
# Host wrapper. Only literal environment names enter the pinned container.
set -euo pipefail
image='unityci/editor:ubuntu-6000.3.23f1-android-3.2.2@sha256:4f19f96e06eb8bdd36051c8b9f92d8bf6ba3953e1e7286a1eecabaac21929a28'
mode="${1:?build or package required}"
test "${GITHUB_ACTIONS:-}" = true
test "${GITHUB_REF_TYPE:-}" = tag
args=(--rm -v "${GITHUB_WORKSPACE}:${GITHUB_WORKSPACE}" -v "${RUNNER_TEMP}:${RUNNER_TEMP}" -w "${GITHUB_WORKSPACE}")
for name in GITHUB_ACTIONS GITHUB_WORKSPACE GITHUB_REF_TYPE GITHUB_REF_NAME GITHUB_SHA GITHUB_WORKFLOW_SHA GITHUB_RUN_ID GITHUB_RUN_NUMBER GITHUB_RUN_ATTEMPT RUNNER_TEMP STP_RC_IDENTITY STP_RC_IDENTITY_RAW_SHA256 STP_RC_BUILD_INFO STP_RC_BUILD_INFO_RAW_SHA256 STP_RC_NATIVE_OUTPUT_ROOT; do
  test -n "${!name:-}"
  args+=(-e "$name")
done
if [ "$mode" = build ]; then
  test -x "${DOTNET_ROOT:?}/dotnet"
  args+=(-v "${DOTNET_ROOT}:${DOTNET_ROOT}:ro" -e "STP_DOTNET10=${DOTNET_ROOT}/dotnet")
  for name in UNITY_EMAIL UNITY_PASSWORD STP_PRODUCTION_SOURCE_ROOT STP_PRODUCTION_IMPORT_REQUEST STP_PRODUCTION_APPROVAL_LOCK STP_PRODUCTION_APPROVAL_RAW_SHA256 STP_PRODUCTION_SERVICE_PROFILE STP_PRODUCTION_SERVICE_PROFILE_RAW_SHA256 STP_PRODUCTION_IAP_REVIEW_ORIGINAL STP_PRODUCTION_IAP_REVIEW_RAW_SHA256 STP_PRODUCTION_IAP_POLICY_REVISION STP_PRODUCTION_IAP_SDK_REVISION STP_PUBLISHER_LEGAL_URL STP_PUBLISHER_SUPPORT_URL; do
    test -n "${!name:-}"
    args+=(-e "$name")
  done
  docker run "${args[@]}" "$image" bash -euo pipefail -c '
    "${STP_DOTNET10}" --version | grep -Fx "10.0.401"
    lic=/opt/unity/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client
    release_seat() { "$lic" --username "$UNITY_EMAIL" --password "$UNITY_PASSWORD" --deactivate-all >/dev/null 2>&1 || true; }
    trap release_seat EXIT
    "$lic" --username "$UNITY_EMAIL" --password "$UNITY_PASSWORD" --activate-all --include-personal >/dev/null 2>&1
    mkdir -p "$RUNNER_TEMP/stp-rc-evidence"
    unity-editor -batchmode -nographics -projectPath "$GITHUB_WORKSPACE" -buildTarget Android \
      -executeMethod STP.Editor.Build.ProductionNativeBuild.BuildAndroidReleaseBundle -quit \
      -logFile "$RUNNER_TEMP/stp-rc-evidence/unity-android.log"
  '
elif [ "$mode" = package ]; then
  # Signing and verification are offline. No Unity or license credentials enter this process.
  args+=(--network none -v "${RUNNER_TEMP}/stp-rc-credentials:${RUNNER_TEMP}/stp-rc-credentials:ro")
  for name in STP_RC_KEYSTORE_PASSWORD STP_RC_KEY_PASSWORD STP_RC_ANDROID_KEY_ALIAS STP_RC_CERTIFICATE_SHA256; do
    test -n "${!name:-}"
    args+=(-e "$name")
  done
  docker run "${args[@]}" "$image" bash -euo pipefail -c '
    receipt="$STP_RC_NATIVE_OUTPUT_ROOT/native-rc-receipt.json"
    receipt_sha=$(sha256sum "$receipt" | cut -d" " -f1)
    android=/opt/unity/Editor/Data/PlaybackEngines/AndroidPlayer
    python3 -B tools/release/rc_package.py android-sign \
      --identity "$STP_RC_IDENTITY" --identity-sha256 "$STP_RC_IDENTITY_RAW_SHA256" \
      --native-receipt "$receipt" --native-receipt-sha256 "$receipt_sha" \
      --jdk "$android/OpenJDK" --bundletool "$android/Tools/bundletool-all-1.17.2.jar" \
      --bundletool-sha256 2d4ad908faea64047c1cc9cb747e6aa667c6ab192e09607bd16b67246a8cd6ae \
      --keystore "$RUNNER_TEMP/stp-rc-credentials/android-upload.jks" --key-alias "$STP_RC_ANDROID_KEY_ALIAS" \
      --certificate-sha256 "$STP_RC_CERTIFICATE_SHA256" --output "$RUNNER_TEMP/stp-rc-package-android"
  '
else
  echo 'RC_ANDROID_MODE_INVALID' >&2
  exit 2
fi
