#!/usr/bin/env bash
set -euo pipefail
mkdir -p DeviceResults
export PATH="$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator:$ANDROID_HOME/cmdline-tools/latest/bin:$PATH"
image='system-images;android-30;google_apis;x86_64'
df -h "$ANDROID_HOME" "$HOME"
if [[ "${RUNNER_ENVIRONMENT:-}" == github-hosted ]]; then
  sudo rm -rf /usr/share/dotnet /opt/ghc /opt/hostedtoolcache/CodeQL
fi
timeout 10m sdkmanager --install "$image" emulator
echo no | avdmanager create avd --force --name stp-device --package "$image" --device pixel_4
# The runner's SDK image includes the ARM64 translation needed by the shipped APK.
sudo chmod 666 /dev/kvm
emulator -avd stp-device -no-window -no-audio -no-snapshot -no-boot-anim -gpu swiftshader_indirect -memory 4096 -partition-size 2048 > DeviceResults/emulator.log 2>&1 &
emulator_pid=$!
trap 'tail -100 DeviceResults/emulator.log || true' ERR
for _ in $(seq 1 60); do
  kill -0 "$emulator_pid" || { tail -120 DeviceResults/emulator.log; exit 1; }
  if adb devices | grep -Eq '^emulator-[0-9]+[[:space:]]+device'; then break; fi
  sleep 2
done
timeout 30s adb wait-for-device
for _ in $(seq 1 180); do
  [[ "$(timeout 10s adb shell getprop sys.boot_completed | tr -d '\r')" == 1 ]] && break
  sleep 2
done
test "$(adb shell getprop sys.boot_completed | tr -d '\r')" = 1
adb shell getprop ro.product.cpu.abilist | tee DeviceResults/abis.txt
grep -q arm64-v8a DeviceResults/abis.txt
adb shell wm size 1080x2400
adb shell wm density 420
adb shell input keyevent 82
apk="$(find apk -name '*.apk' -type f -print -quit)"
aapt="$(find "$ANDROID_HOME/build-tools" -name aapt -type f | sort -V | tail -1)"
"$aapt" dump badging "$apk" > DeviceResults/package.txt
package="$(sed -n "s/^package: name='\([^']*\)'.*/\1/p" DeviceResults/package.txt)"
test -n "$package"
adb install "$apk"
component="$(adb shell cmd package resolve-activity --brief "$package" | tr -d '\r' | tail -1)"
trap 'timeout 15s adb logcat -d -s Unity ActivityManager AndroidRuntime > DeviceResults/player.log || true' EXIT
python3 tools/ci/device_playthrough.py "$package" "$component"
