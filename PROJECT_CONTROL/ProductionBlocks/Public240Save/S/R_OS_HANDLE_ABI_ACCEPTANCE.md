# R – enge FFI-ABI-Korrektur vor Apple-/Android-Compile

R/Codex nimmt die getrennte technische ABI-Prüfung von entry_audit an. Die eigene neue SaveFileSystem.cs bleibt innerhalb der bereits angenommenen Betriebssystemhandle-Ausführung von ADR035; keine Architektur-/Produkt-/Manifest-/Assemblyänderung.

Der bisherige feste dreiparametrige PInvoke für die variadische fcntl-Schnittstelle wird entfernt. Auf Apple ARM64 liegen variadische Argumente auf dem Stack; der feste dritte Managedparameter ist deshalb kein tragfähiger plattformübergreifender ABI-Vertrag. Der read-only Rootdeskriptor wird stattdessen unmittelbar mit O_RDONLY|O_CLOEXEC geöffnet. open erhält nur seine zwei festen Argumente; O_CREAT/O_TMPFILE und ein Mode-Vararg werden nicht verwendet. Darwin/iOS verwenden0x01000000, Linux/Android1<<19. iOS/Android-Player wählen explizite Compilesymbole, Desktopeditoren die tatsächliche Runtime-OS-Identität; andere Plattformen scheitern benannt statt eine ungeprüfte Konstante oder einen fcntl-Fallback zu verwenden. Windows-Mutexcode bleibt unverändert.

Primärquellen:
- Apple ARM64-ABI: https://developer.apple.com/documentation/apple-silicon/addressing-architectural-differences-in-your-macos-code
- Apple-XNU-Fcntlheader: https://github.com/apple/darwin-xnu/blob/main/bsd/sys/fcntl.h
- Linux-UAPI: https://github.com/torvalds/linux/blob/master/include/uapi/asm-generic/fcntl.h

Alle a6bb386-/85872e4-Nachweise bleiben unverändert historische gültige Windows-/Linux-Bindungen. Vor Freeze folgen exakte neue Compile-/Linux-FFI-/Timeoutbindungen sowie definierte Apple-/Android-Compileprüfungen; die reguläre eigene Unity-CI bleibt erforderlich. Ein physischer iOS/APFS- oder Android-Hardware-PASS wird nicht behauptet.
