#!/usr/bin/env bash
set -euo pipefail
test "${GITHUB_ACTIONS:-}" = true
test "${GITHUB_REF_TYPE:-}" = tag
mode="${1:?build or package required}"
unity='/Applications/Unity/Hub/Editor/6000.3.23f1/Unity.app/Contents/MacOS/Unity'
if [ "$mode" = build ]; then
  test -x "$unity"
  test -x "${STP_DOTNET10:?}"
  "${STP_DOTNET10}" --version | grep -Fx '10.0.401'
  lic='/Applications/Unity/Hub/Editor/6000.3.23f1/Unity.app/Contents/Helpers/UnityLicensingClient.app/Contents/MacOS/Unity.Licensing.Client'
  release_seat() { "$lic" --username "$UNITY_EMAIL" --password "$UNITY_PASSWORD" --deactivate-all >/dev/null 2>&1 || true; }
  trap release_seat EXIT
  "$lic" --username "$UNITY_EMAIL" --password "$UNITY_PASSWORD" --activate-all --include-personal >/dev/null 2>&1
  mkdir -p "$RUNNER_TEMP/stp-rc-evidence"
  "$unity" -batchmode -nographics -projectPath "$GITHUB_WORKSPACE" -buildTarget iOS \
    -executeMethod STP.Editor.Build.ProductionNativeBuild.ExportIosReleaseSources -quit \
    -logFile "$RUNNER_TEMP/stp-rc-evidence/unity-ios.log"
elif [ "$mode" = package ]; then
  # A real approved Podfile.lock is already tracked in this exact tag.
  python3 -B - <<'PY'
import hashlib, os, re, sys
from pathlib import Path
sys.path.insert(0,'tools/release')
from rc_ci_metadata import git_blob
raw=git_blob(Path.cwd(),'Release/IosPods/Podfile.lock')
expected=os.environ['STP_RC_IOS_POD_LOCK_SHA256']
if not re.fullmatch('[0-9a-f]{64}',expected) or hashlib.sha256(raw).hexdigest()!=expected:
    raise SystemExit('RC_APPROVED_POD_LOCK_MISMATCH')
destination=Path(os.environ['STP_RC_NATIVE_OUTPUT_ROOT'])/'Xcode/Podfile.lock'
with destination.open('xb') as stream:stream.write(raw)
PY
  receipt="$STP_RC_NATIVE_OUTPUT_ROOT/native-rc-receipt.json"
  receipt_sha=$(shasum -a 256 "$receipt" | cut -d' ' -f1)
  test "$(pod --version)" = '1.16.2'
  python3 -B tools/release/rc_package.py ios-package \
    --identity "$STP_RC_IDENTITY" --identity-sha256 "$STP_RC_IDENTITY_RAW_SHA256" \
    --native-receipt "$receipt" --native-receipt-sha256 "$receipt_sha" \
    --xcodebuild /usr/bin/xcodebuild --pod "$(command -v pod)" \
    --podfile-sha256 "$STP_RC_IOS_PODFILE_SHA256" --pod-lock-sha256 "$STP_RC_IOS_POD_LOCK_SHA256" \
    --team "$STP_RC_IOS_TEAM" --profile-uuid "$STP_RC_IOS_PROFILE_UUID" \
    --keychain "$RUNNER_TEMP/stp-rc-credentials/signing.keychain-db" \
    --certificate "$RUNNER_TEMP/stp-rc-credentials/apple-distribution.cer" \
    --certificate-sha256 "$STP_RC_CERTIFICATE_SHA256" --output "$RUNNER_TEMP/stp-rc-package-ios"
else
  echo 'RC_IOS_MODE_INVALID' >&2
  exit 2
fi
