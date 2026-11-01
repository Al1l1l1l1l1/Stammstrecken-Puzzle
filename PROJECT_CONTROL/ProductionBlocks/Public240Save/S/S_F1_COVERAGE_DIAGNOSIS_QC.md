# F1 independent coverage diagnosis

Regular Unity run39245200214 failed after 1038/1038 passed NUnit tests. The unchanged gate correctly detects full sequence-point growth: Application2194→2254 (+60, nine generated async MoveNexts); TestApplication1252→1380 (+128, nineteen generated async MoveNexts). Method counts stay634/201. All188 additions are actual new ILoffset nodes; no old offsets were removed. Exact method/offset details and raw XML hashes are in the companion JSON; original827685-byte ZIP matches API digest a0ab388b9fb2d3901c63a35eee72ca31cd06df7214372086b2b962b3e282d469.

The JIT-prewarm proposal is a permitted subject for a separately coordinated scratch diagnosis, not an accepted implementation. Early initialization plausibly precedes test RunStarted/FullEmpty, but actual ordering must be traced. RuntimeHelpers.PrepareMethod is not a proven JIT operation on this exact Mono build: Unity public Mono source has a stub in an ENABLE_NETCORE branch. Require actual before/after sequence-point and hit-count evidence.

The helper must be editor-only, remain in its existing permitted test file, use a closed assembly/method selection, and never execute MoveNext or generate hits. No filters, counters, XML, collector, validator, assembly graph or gate edits. A/B must prove full stable point sets before accepting any fix; corrected replacement regular CI is still required.

Actual successful seat deactivation:11:39:55.7737897Z. Later Play/Android/iOS jobs skipped. Original full raw logs stay ignored; companion embeds a complete declared redacted log profile.

Sources: [Unity Mono source](https://raw.githubusercontent.com/Unity-Technologies/mono/unity-main/mono/metadata/icall.c), [Microsoft PrepareMethod API](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.runtimehelpers.preparemethod).
