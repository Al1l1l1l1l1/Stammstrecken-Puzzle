#!/usr/bin/env bash
set -euo pipefail
version="${STP_UNITY_VERSION:?}"
base="https://download.unity3d.com/download_unity/${STP_UNITY_CHANGESET:?}"
work="$(mktemp -d "${RUNNER_TEMP}/unity-install.XXXXXX")"
for package in "MacEditorInstallerArm64/Unity-${version}.pkg" "MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-${version}.pkg"; do
  file="${work}/${package##*/}"
  curl --fail --location --retry 3 --output "${file}" "${base}/${package}"
  pkgutil --check-signature "${file}" | tee "${work}/signature"
  grep -F 'Developer ID Installer: Unity Technologies' "${work}/signature"
  sudo installer -pkg "${file}" -target /
  rm -f "${file}"
done
bundle="/Applications/Unity/Unity.app"
destination="/Applications/Unity/Hub/Editor/${version}"
if [[ -d "${bundle}/Contents/PlaybackEngines/iOSSupport" ]]; then
  sudo mkdir -p "${destination}"
  sudo mv "${bundle}" "${destination}/Unity.app"
fi
test -x "${destination}/Unity.app/Contents/MacOS/Unity"
test -d "${destination}/Unity.app/Contents/PlaybackEngines/iOSSupport"
