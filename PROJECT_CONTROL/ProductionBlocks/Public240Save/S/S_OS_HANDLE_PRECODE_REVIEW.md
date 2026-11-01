# Independent narrow OS-handle pre-code review

Permitted within immutable S filesystem scope and accepted ADR035/T-details OS-handle execution. Windows named mutex plus native directory-handle flock on supported Unix targets is a concrete implementation choice; root-wide serialization is conservative and changes no product policy. No platform/runtime PASS is given.

Prefer a stable already-existing app-private local root, with a named error if absent. Do not implicitly create it during Load/Recover or an arbitrary-thread constructor. Explicit worker provisioning belongs separately to later composition; read-only operations must show zero filesystem delta.

Use independent readonly directory open per acquisition, native nonblocking exclusive lock, monotonic30s timeout/cancellation, and guaranteed close after all paths. Bind platform ABI/constants/libraries explicitly. Unsupported errno or PInvoke linkage fails closed with a stable error; no fallback to unproven Mono named mutex. Keep every observe/CAS/write/ACK inside the handle. Stable root inode must not be replaced during use.

[Linux flock documentation](https://man7.org/linux/man-pages/man2/flock.2.html) establishes open-description locking, contention between separate opens, final-close release, and readonly mode. [Official Android Bionic header](https://android.googlesource.com/platform/bionic/+/8bc9f80f42cc5c9bc20dc546d808d57f0c3a4dfe/libc/include/sys/file.h) exports the API.

[Apple iOS manual](https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/flock.2.html) documents native flock and unsupported-type errors; [Apple XNU sys_flock](https://raw.githubusercontent.com/apple-oss-distributions/xnu/main/bsd/kern/kern_descrip.c) delegates vnode locks to the filesystem. Neither is a target APFS directory-FD execution proof.

Require actual separate-process exclusion, normal/crash release, same-process distinct opens, cancellation/timeout, no read-only delta and native UnityLinuxFFI evidence. GCC/CPython is supplemental; mobile runtime/durability remains separately bounded. Reviewer performed no source/Git/Unity/CI action.
