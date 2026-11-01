# Prospective non-executing JIT diagnosis

No objection to a tightly bounded scratch A/B using ordinary MoveNext.MethodHandle.GetFunctionPointer and discarding the pointer. [Unity Mono source](https://raw.githubusercontent.com/Unity-Technologies/mono/unity-main/mono/metadata/icall.c) lines7828–7835 compiles the method without calling it. RuntimeHelpers.PrepareMethod is discarded. Public source is not an exact pinned-binary guarantee.

Keep the helper editor/test-only in its permitted existing file, use the fixed STP assembly list and nongeneric IAsyncStateMachine MoveNext methods, fail visibly on incomplete preparation, and never invoke a method or create a state-machine instance. Only actual trace can bind it before FullEmpty. A/B must compare every method offset set and prove zero pretest hits, then stable sets with actual test hits. No counter/reset/XML/collector/filter/gate changes.

This is a prospective diagnosis review, not acceptance of a fix or S. Actual pinned Unity A/B originals and replacement regular CI remain required.
