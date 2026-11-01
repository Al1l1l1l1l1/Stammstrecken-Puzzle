# I-ROOT-002: Unix protected-real-root alias

HIGH confirmed by exact sourcecounterexample on originalInstallercb4b6dd9. OnUnix ComparisonDirectory returns the lexicalpath; realRoot is not physically resolved/reparsechecked. Candidate/tmp/.../physical-profile/test can be ordinary and underOS-temp while protectedrealRoot has a different symlinkname targeting/tmp/.../physical-profile. Lexicaldisjointness passes. S.CheckReparse only traverses candidateancestors and cannot see the differentlynamed realRootalias.

No actualrealprofileIO/symlink/Unity/Monoexecution performed; the pathpredicate computation is source-equivalent only. FulloriginalInstaller and readonlySaveFS bytes captured beforefix are losslessly included inJSON.

The proposed test-onlyUnix reject-all-existing-Reparse/NonDirectory ancestor guard on allfourpaths is an appropriate narrow fail-closed correction, provided actualMono metadata proof covers danglinglinks andMissingSuffix behavior beforeclosure. WindowsNTbranch unchanged; symlinked boundsprefixes intentionally rejected, no fallbackroot.

Native08Windows45/45OS0 and originalimages remainvalid/unchanged. This separatefinding blocksUnixphysicaltestroot assurance/I03andfinalfreeze readiness until actualfix/proofs; it doesnotrewritehistoricalnativePASS.
