# S F1: additive independent correction

The earlier claim that diff-check was optional was wrong. validate.py lines 1452–1453 enforces it; actual Architecture run 39245200207 failed scope:diff-check. The historical report remains unchanged.

F1 ba4abccebeb0e073986f37b92b7479f61dabc063 has 77 warnings across nine paths: 40 in the initial staged observation plus 37 in the later-added raw advisory file. Every original native gzip transport and Git-LF binding roundtrips exactly to the original F1 object. All nine mapping rows match current bytes. Runtime LocalProfileState changes only six blank lines (358, 428, 664, 747, 853, 868), removing twelve spaces each. Eight historical text files are readable whitespace-clean views; exact originals remain in the unchanged F1 commit and additive transport. The two XML views also gain a final LF. Current unchanged-validator diff-check against D exits 0.

PASS applies only to that exact narrow delta and original preservation. Fresh compiler/PDB and targeted tests for model SHA 6903df8bdd5c086d96882b278169e489f89d52b45caa765592c887e4c4356b62, a replacement freeze and new regular CI remain required.

F1 Unity 39245200214 also completed failure: 1038/1038 NUnit tests passed, then coverage consistency failed for STP.Application. Actual successful deactivation occurred at 11:39:55.7737897Z; subsequent three licensed jobs were skipped. This is no whole-S/LS acceptance.

Companion JSON SHA256 f1fcb73310443f2147253a7bc1e6319e84a559a02132777e6ac9de479a87008f embeds complete declared redacted decoded-UTF8 log profiles, independent original raw hashes, all nine byte/hash/transform results and gate provenance. Full unredacted logs stay ignored.
