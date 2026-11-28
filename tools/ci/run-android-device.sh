#!/usr/bin/env bash
set -euo pipefail
mkdir -p DeviceResults
if [[ "$(uname -m)" == aarch64 ]]; then
  # Run the ARM64 player directly on a disposable ARM64 host, with a software GPU.
  sudo apt-get update -qq
  sudo apt-get install -y --no-install-recommends adb aapt
  if ! sudo modprobe binder_linux devices=binder,hwbinder,vndbinder; then
    sudo apt-get install -y --no-install-recommends "linux-modules-extra-$(uname -r)"
    sudo modprobe binder_linux devices=binder,hwbinder,vndbinder
  fi
  image=redroid/redroid@sha256:46478a567194aed24cd0877d4434a9e58b534d4aad30931eb21999a52f2ce131
  docker pull "$image"
  docker run --detach --name stp-android --privileged --publish 127.0.0.1:5555:5555 "$image" \
    androidboot.use_memfd=true androidboot.redroid_gpu_mode=guest \
    androidboot.redroid_width=1080 androidboot.redroid_height=2400 androidboot.redroid_dpi=420
  export ANDROID_SERIAL=127.0.0.1:5555
  trap 'docker logs --tail 60 stp-android || true' ERR
  for _ in $(seq 1 60); do
    adb connect 127.0.0.1:5555 || true
    adb devices | grep -Eq '^127.0.0.1:5555[[:space:]]+device' && break
    sleep 2
  done
else
  export PATH="$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator:$ANDROID_HOME/cmdline-tools/latest/bin:$PATH"
  image='system-images;android-35;google_apis;x86_64'
  df -h "$ANDROID_HOME" "$HOME"
  if [[ "${RUNNER_ENVIRONMENT:-}" == github-hosted ]]; then
    sudo rm -rf /usr/share/dotnet /opt/ghc /opt/hostedtoolcache/CodeQL
  fi
  timeout 10m sdkmanager --install "$image" emulator
  export ANDROID_USER_HOME="$RUNNER_TEMP/stp-android"
  export ANDROID_AVD_HOME="$ANDROID_USER_HOME/avd"
  mkdir -p "$ANDROID_AVD_HOME"
  avd="$ANDROID_AVD_HOME/stp-device.avd"
  echo no | avdmanager create avd --force --name stp-device --package "$image" --device pixel_4 --path "$avd"
  test -f "$avd/config.ini"
  printf 'avd.ini.encoding=UTF-8\npath=%s\ntarget=android-35\n' "$avd" > "$ANDROID_AVD_HOME/stp-device.ini"
  emulator -list-avds | grep -Fx stp-device
  # The runner's SDK image includes the ARM64 translation needed by the shipped APK.
  sudo chmod 666 /dev/kvm
  emulator -avd stp-device -no-window -no-audio -no-snapshot -no-boot-anim -gpu software -memory 4096 -partition-size 2048 > DeviceResults/emulator.log 2>&1 &
  emulator_pid=$!
  trap 'tail -100 DeviceResults/emulator.log || true' ERR
  for _ in $(seq 1 60); do
    kill -0 "$emulator_pid" || { tail -120 DeviceResults/emulator.log; exit 1; }
    if adb devices | grep -Eq '^emulator-[0-9]+[[:space:]]+device'; then break; fi
    sleep 2
  done
fi
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
adb shell settings put secure immersive_mode_confirmations confirmed
adb shell svc power stayon true
adb shell settings put system screen_off_timeout 1800000
adb shell wm dismiss-keyguard
adb shell input keyevent 82
# Wait for the virtual machine's own home screen to settle before timing a cold game launch.
timeout 60s adb shell am start -W -a android.intent.action.MAIN -c android.intent.category.HOME || true
sleep 25
adb shell am force-stop com.google.android.apps.nexuslauncher
apk="$(find apk -name '*.apk' -type f -print -quit)"
if command -v aapt >/dev/null; then aapt="$(command -v aapt)"; else
  aapt="$(find "$ANDROID_HOME/build-tools" -name aapt -type f | sort -V | tail -1)"
fi
"$aapt" dump badging "$apk" > DeviceResults/package.txt
package="$(sed -n "s/^package: name='\([^']*\)'.*/\1/p" DeviceResults/package.txt)"
test -n "$package"
adb install "$apk"
component="$(adb shell cmd package resolve-activity --brief "$package" | tr -d '\r' | tail -1)"
trap 'timeout 15s adb logcat -d -s Unity ActivityManager AndroidRuntime > DeviceResults/player.log || true; timeout 15s adb logcat -d -b crash > DeviceResults/native-crash.log || true' EXIT
python3 tools/ci/device_playthrough.py "$package" "$component"
