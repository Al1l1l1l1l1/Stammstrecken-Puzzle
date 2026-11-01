# WP-037 – tatsächliche reguläre Codefreeze-CI

**Implementierungsnachweis / Eigenprüfung, keine unabhängige AK07-Abnahme.**

Codefreeze **3638dd8ba39b1f4f6867fdad03ab70afb0de79af**, Tree **411696a214427f91d4ec4b96dd7eeebf098b8449**. Reguläre Pushläufe [Architecture 39155858104](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39155858104) und [Unity 39155858157](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39155858157) sind beide **COMPLETED / SUCCESS**. Alle zehn Unityjobs erfolgreich; kein übersprungener Pflichtjob. Die unveränderten Workflows lösen für den Unter-PR gegen WP036 Push-CI aus; pull_request ist auf Ziel main gefiltert. Keine PR-Ausführung erfunden, keine Filter-/Runner-/Teständerung, kein Retry oder Abbruch.

## Tatsächliche Jobzustände und Checkoutbindung

| Job | ID | Status | Ergebnis |
|---|---:|---|---|
| Architecture / validate | 117758820102 | completed | success |
| unity-environment-ios | 117758820436 | completed | success |
| unity-environment-linux | 117758820622 | completed | success |
| project-preflight | 117758820646 | completed | success |
| unity-config | 117758820671 | completed | success |
| unity-environment-android | 117758820705 | completed | success |
| unity-evidence-guard | 117758893320 | completed | success |
| unity-compile-editmode | 117758893452 | completed | success |
| unity-playmode-bootstrap-smoke | 117760651180 | completed | success |
| unity-android-development-il2cpp | 117761480355 | completed | success |
| unity-ios-export | 117763242667 | completed | success |

Die vollständigen elf Joblogs sind erhalten. Alle relevanten Git-Checkoutausgaben lauten tatsächlich 3638dd8ba39b1f4f6867fdad03ab70afb0de79af; die Bindung ist aus den Logs ausgewertet, nicht allein vom API-head_sha abgeleitet. Der Scopekontext lautet exakt: workPackage=WP-037, base=ff1836e293a2a5cff69773525c347b55deb67dfb, head=3638dd8ba39b1f4f6867fdad03ab70afb0de79af, worktreeDirty=false.

## Architecture / Scope / JCS / Text

Tatsächlicher Originallog: 17 Architecture-Prüfgruppen und 18 WP037-Scopegruppen einschließlich Self-/Negativtests bestanden. CPython 3.11.13 / Node 22.20.0 / bestehender Lock unverändert. JCS: 12 Kanonisierungen, 32 Ablehnungsfälle, 2 v2- und 2 v1-Dokumentvektoren. Text: 1.408 Einträge, 960/960 Referenzen, 240 INTERNAL, zwölf erkannte Negativmutationen; exakte Lesefassung bestanden. 480 fremde Referenzziele bleiben ausdrücklich offen.

## Original-Test-XMLs

| Nachweis | Tatsächlich gelesen |
|---|---|
| EditMode | 455 total / 455 Passed / 0 Failed / 0 Skipped |
| Aufteilung | 19 Domain + 56 Solver + 379 Content + 1 Unity-Addressables-Beispielfall |
| HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind | Passed, 72.442419 s |
| LargeRootTrace_PreservesSolverV2PremiseSelectionAndMetrics | Passed, 0.513261 s |
| PlayMode | 27 total / 27 Passed / 0 Failed / 0 Skipped |
| QaScene_ComposesCompleteGraph_ExactlyOneBindingPerPort_NoProviderStart | Passed, 0.408011 s |

Die neue tatsächliche Zahl ist 455, nicht die 456 des WP034-Korpusstands. Alle vorhandenen Domain-/Solver-/Contentregressionen des Zielprojekts wurden im regulären EditMode ausgeführt. Beide Coverage-Sitzungen enthalten 15 STP-Assemblies; EditMode-Solver 1039/1045 Sequenzpunkte und 123/123 Methoden. Nullmessungen werden nicht mit der tatsächlichen Besuchsmessung vermischt; keine neue Mindestquote eingeführt.

Die originale HardBudget-XML enthält alle sechs Fälle. Für die fünf großen Fälle sind excess/build-Verhältnisse aus den Originalausgaben 0.800/5.100, 0.700/5.400, 0.800/6.200, 0.600/6.700 und 0.900/6.400; maximal rund 0.156863 < 0.4. Der unterbrochene Langpfad verwendet weiterhin maxStates=2. Zwei Warm-ups, 25 Samples, Zustandsgrenzsuche, 40%-Assertion und der beobachtete 180000-ms-Runnerdefault bleiben unverändert; das interne Solverzeitbudget bleibt 600000 ms. Keine universelle Host-Latenzgarantie aus diesem Lauf ableiten.

Der neue unverändert übernommene Test bindet UNIQUE, 1.639 Schritte, Metriken (0,1639,30,0) und vollständige Trace-SHA256 6E59FFFAFB16B61C9FDDC301ED9229078FEEA11BEF1E2F44616CE7F9D8460B78. Kein neuer Golden-/Erwartungswert und keine Solversemantikänderung.

## Artefaktdigests und Plattformen

| Artefakt | ID | Original-ZIP-SHA256 = GitHub-Digest |
|---|---:|---|
| editmode-results | 12147470438 | 779e6b842d863cd746b12a70dcfb5d459a936266e7c8ff8b99d0505896c7fa6c |
| playmode-results | 12147630430 | e8a6d5271f33875f1932efdbc0849c43021b9f296ecf7be0fff996d9ecb31584 |
| android-development-il2cpp | 12147785393 | 145e6f6ab76991bad9917e572868d56b6e7779cdf5a3eb292b1dd55edc14ab45 |

EditMode-XML-SHA256: 3e36665baf1944e70809a11226ce9591af96390906276e869787701e4b993bd9. PlayMode-XML-SHA256: ba7de22e30027a0e54c1c2b4cae14da244438e175e5dcc6304764a58a29d8051. Die ZIPs wurden tatsächlich heruntergeladen, dekomprimiert und gegen die serverseitigen Digests geprüft.

Android: ursprüngliches APK 40066246 Bytes, SHA256 8a98d7b6519f59f8ea650eada2d1df306f6a282deced9eb3fb5a7f763342120a, AndroidManifest und native ARM64-libIL2CPP vorhanden. Vollständiger Buildlog bestätigt den Codefreeze und STP Build PASS: Android. iOS: vollständiger 2526568-Byte-Log im ausdrücklich ausgewiesenen gespeicherten Textprofil, SHA256 bb21cc71759817914c0868bcaf0cc79d4ebab4fc08b9230627fa2814c093d860; Xcode 26.3, erfolgreicher Unityexport und BUILD SUCCEEDED am 18.10.2026 um 19:15:43.145228 UTC, CODE_SIGNING_ALLOWED=NO. Keine Geräte-/Signing-/Store-/Releasefreigabe.

## Dauerhafte Originale und Byteprofile

[Maschinenlesbare Originalevidenz](WP-037_CI_EVIDENCE.json), SHA256 **b897fa2fc65b3c33bd11778af8625f282fb42522bcc777cabbec7037d735261b**, 2724335 Bytes. Sie enthält 19 gzip/base64-Payloads mit allen elf vollständigen Joblogs, sechs Original-XMLs und zwei Original-Test-ZIPs; für jede Payload sind Roh-/Gziphash und Bytezahl gespeichert. Bei der Persistenz wurden alle 19 Payloads erneut dekomprimiert und sämtliche Bytezahlen/SHA256-Werte geprüft. Das Support-Evidenzobjekt wurde bytegleich übernommen.

ZIP-/XML-Hashes gelten für unveränderte Originalbytes. Loghashes gelten für den vollständigen dekodierten Connectorinhalt als UTF-8 mit gegebenenfalls vorhandenem BOM, LF-Trennern und einer angefügten terminalen Newline; sie werden ausdrücklich nicht als Roh-HTTP-Archivhash bezeichnet. Metadaten sind geparste abgeleitete JSON-Antworten. Original-ZIPs und Downloads bleiben lokal im ignorierten .utmp/wp037/ci-evidence/codefreeze/; das Android-APK wird nicht redundant ins Repository kopiert. Dauerhaft erhalten sind dessen Digest, Memberinventar, nativer IL2CPP-Nachweis und kompletter Buildlog.

## Historie, Liefernachlauf und Abnahmegrenze

Der bekannte ff1836e-Push 39079576211 bleibt FAILURE: 453/454, HardBudget 186.063155 s bei 180000 ms; Folgejobs skipped. Beide ff1836e-Architecture-Läufe und der spätere Unity-PR-PASS bleiben getrennte positive Nachweise; sie löschen den FAIL nicht. Originale TECHNICAL_CI_AUDIT.md/.json bleiben unverändert. Die ältere 89c8-Stornierung bleibt ein anderer Stand. Vorbereitung bdf5cd8: Architecture 39153458038 und Unity 39153457924 terminal SUCCESS; alleiniger Vorbereitungsnachweis, kein Reparatur-PASS. Konkrete historische Host-/Scheduling-/GC-Ursachen bleiben unbewiesen.

Dieser Bericht bindet den Codefreeze, nicht einen späteren reinen Nachweis-/Statuscommit. Dessen tatsächlicher veröffentlichter SHA, eigener CI-Stand und Nachlauf werden im bestehenden PR32 separat ausgewiesen. Laufende Prüfungen sind kein PASS. Die vorgeschriebene unabhängige technische AK07-Abnahme bleibt offen; diese Implementierungs-/Support-Eigenprüfung vergibt keinen Gesamt-PASS und autorisiert keinen Untermerge/Mainmerge.
