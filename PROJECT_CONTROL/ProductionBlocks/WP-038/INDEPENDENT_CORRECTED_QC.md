# WP038 – unabhängige korrigierte technische Abschlussprüfung

**PASS: AK01–AK10 am exakten korrigierten Freeze C2. Keine offenen Blocker oder Highs innerhalb dieses Prüfgegenstands.** Getrennte Rolle `/root/wp037_independent_qc`; keine Implementierung, Source-, Shared-Git-, Remote-, CI- oder Editormutation. Dies ist die technische Annahme. AK11, zuständiger Unterintegrationsentscheid und tatsächlicher integrierter Nachweis bleiben gesondert; Main ist nicht freigegeben.

[Vollständige eigene maschinenlesbare Bindungen](INDEPENDENT_CORRECTED_QC.json), SHA256 `b487c3be2a1d2e57ddca70841229ad35a849fff87b369a391d69971f56fe88e7`.

## Geprüfter Gegenstand und Schutzketten

Codefreeze `524b45024438b93c592351d7683a7c62aec2ab8d`, Tree `f1adfa4e006169c32bd930cc6c76f1d62b98e780`, einziger Parent C1 `e77c5fa168b915979c3c9e39d45fbcfcaefa3aea`. Gemeinsamer prospektiver WP-/Manifestanker ee14a1b auf H `3772c0370b39ec95979fb9efdd47f4344c41a539`, gesonderte angenommene ADR033-Entscheidung 4667413 vor Implementierung. Manifest seit Erstadd bytegleich, SHA256 cf9087d50941eb64dea3dc1ea894dd29a3a5a2a7d611668e3228cc6b875b979a. Frisch PR33 offen/Draft exakt C2 gegen H, PR31 weiter Draft gegen unverändertes Main d90e798.

Eigener sauberer LF-Prüfscratch, gepinnte CPython 3.11.13 und Node 22.20.0: **Architecture 17 und Production-Scope 18 einschließlich Self-/Negativtests tatsächlich PASS**, H→C2, worktreeDirty=false, 113 kumulative erlaubte Pfade. Vollständige Outputs/argv/Head im eigenen JSON. Architecture-Output SHA256 820a3f7829f82c7390c108717f0a8bb395d2152f3dbf8f1ab70d7fc871b65ba9; Scope-Output 0a02d0114aebe705da3c2f95f913cd462c60a89947351a045842eb32ac75dbd7.

Alle alten 17 Manifeste, ganze Content-/Packages-/ProjectSettingsbäume, Domain-/Solverquellen/-tests und geschützte Textquelle SHA256 4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c erhalten. ADR-/Registry-/Architekturannahme seit 466 unverändert; geschlossene 33/24/9-Registry samt Negativproben wirksam. Alle bisherigen Assemblies außer exakt vorgesehener externer UI-Unity.Localizationreferenz bytegleich, keine neue Assembly oder interne Kante. C1→C2 verändert an Fach-/Workflow-/Werkzeugpfaden genau Installer und bestehenden Bootstraptest.

## Tatsächliche neue CI und native Bedienbarkeit

Eigener erster regulärer Push auf C2: **Architecture 39175992736 SUCCESS, Unity 39175992749 mit exakt allen zehn Jobs SUCCESS**. Tatsächliche Checkout-/Tree-/Event-/Attemptbindungen selbst frisch gelesen. Child-PR33 gegen WP036 erzeugt nach unverändertem main-Filter keine PR-Läufe; kein zusätzlicher PR-PASS erfunden und kein C1-/Parent-PASS übertragen.

- Neues EditMode-Original: **474/474 Passed**, darunter alle 19 Applicationfälle; 0 Failed/Skipped/Inconclusive. XML SHA256 515d78c5b785f14c4ce6090dd29e71265b314a55aa40c57bb31ebb66ddb9a614.
- Neues PlayMode-Original: **49/49 Passed**, darunter 13 Presentation-, 9 Bootstrap- und sämtliche bisherigen 27 Compositionfälle einschließlich verbindlichem QaScene-Smoke; 0 Failed/Skipped/Inconclusive. XML SHA256 7e4846512db4c8b16f0bedcbb4d57c040d255fae20ed87a495eef2c5a8a50177.
- Beide neuen OpenCover-Gesamt-/Messsitzungen tatsächlich gelesen: je 18 STP-Module mit expliziten Werten und sämtlichen workflowverlangten Assemblies; keine erfundene Mindest-Coverage.
- Neuer realer Android- und iOS-BuildReport verarbeitet jeweils DevelopmentPuzzle und BootstrapComposition, Installer jeweils 1; beide Plattformbuilds erfolgreich, iOS zusätzlich unsigned **BUILD SUCCEEDED** um 23:59:34 UTC. Android-ZIP 12154684308 SHA256 08f2d2cabdc53a44a15476d5ce85220dc04d26d8fcaf4a341b5e6b9a076485f5; tatsächliches APK 46844249 Bytes SHA256 a2f261081707d7ddf9da460676303d6e0b7cf2d865b88ed03156a3bb024db16e, Manifest/ARM64-IL2CPP vorhanden.

Alle elf vollständigen neuen Originallogs, drei Original-ZIPs, 22 persistierten Payloads und 292 Einzelbindungen selbst geprüft. Architecture und alle vier lizenzierten Joblogs zusätzlich selbst frisch geladen und exakt mit gesammelten Rohbytes verglichen. Drei gefrorene Testsourcepayloads zusätzlich gegen echte C2-Gitblobs geprüft. Vollständige Original-/Gzip-/Redaktionsprofile und GitHub-ZIP-Digests verifiziert. [Dauerhaftes neues C2-Evidenzpaket](CORRECTED_CODEFREEZE_CI_EVIDENCE.json): 3815732 Bytes, SHA256 51b336ce70aaeffd66e1b058e7ada69c78b999d1900b956cf3009cb0aaae53f9. Tatsächliche Seatfreigaben aller vier lizenzierten Jobs, zuletzt iOS um 23:59:37 UTC. ProcessScene-GUIDs sind tatsächlich all-zero und werden nicht als eindeutige Build-IDs behandelt; Job/Checkout/Plattform/Szenen/Buildabschlüsse tragen die Bindung. BuildReport-Gesamtgröße wird nicht mit APK-Dateigröße verwechselt.

Die sechs headless Bildoutputs melden ausdrücklich NOT_EXECUTED und beweisen nur UI-Semantik. Echte grafische Annahme trägt der bereits selbst geprüfte **native D3D11-Nichtbatchlauf 9/9**, sechs bytegebundene PNGs und korrekte normale Screenframes bei Panelziel=null. [Unabhängige native Screenkorrektur](SCREEN_OUTPUT_CORRECTION_QC.md), JSON SHA256 7bc6643d31b2aba0fca8c4c9e16091b227ac00a4f70544af1ad8c132ef8283e8: jetzt sämtliche 41 Source-/Meta-/Assetbindungen gegen C2-Gitblobs bestätigt, genau zwei geändert und 39 unverändert. Eigene Bildbetrachtung, Orientierung, Werkzeuge, objektive Counts, selektiertes Gelb, tatsächlicher Train auf selbst gebauter Strecke, Ergebnis danach und Neustart gebunden. Eigene Camera-Lebenszeit und Renderhook-/Coroutine-/Texture-Cleanup bestätigt. Alle fünf automatischen Editor-Settingsdeltas restauriert; kein solcher Quellendiff geliefert.

## Einzelannahme und folgende Entscheidung

| AK | Unabhängiges Ergebnis und konkrete Grundlage |
|---|---|
| AK01 | PASS – Erstadd/Parent/Manifest/Allowlist, 113 Pfade, geschützte Quellen und alte Scopeketten erhalten. |
| AK02 | PASS – getrennte angenommene ADR033-Grenze vor Code, geschlossene Registry und exakter Assembly-/Localizationgraph. |
| AK03 | PASS – zwei eigene öffentliche QA-Definitionen, echte Commands und neues 4×4-/10×10-Layout, keine Produkt-/Lösungsübernahme. |
| AK04 | PASS – tatsächliche Werkzeuge, Auswahl/Batch/Undo/No-op/Counts/A/B; neue reale korrekte Screenprobe und eigene C2-Tests. |
| AK05 | PASS – Timer, Lebenszyklus, frische Versuche, Stale-/Duplikatabwehr; transiente Session ohne dauerhafte Providerwirkung. |
| AK06 | PASS – tatsächlicher PuzzleSolved-Pfad, genau eine gesperrte Fahrt, normal sichtbarer Train, Result danach und Neustart; kein Domain-Savecommit. |
| AK07 | PASS – tatsächliche isolierte de-DE-Unity-StringTable, typisierte Formate/Negativfälle; WP036-Quelle unverändert. |
| AK08 | PASS – feste GUIDs, doppelte Reproduktion/Reimport und neue wirkliche Android-/iOS-Szenenbuilds; Composition erhalten. |
| AK09 | PASS – eigene vollständige neue C2-CI-Originalprüfung einschließlich Logs/XML/Coverage/Artefakten/Checkout/Tree. |
| AK10 | PASS – diese tatsächlich getrennte technische Abschlussrolle bestätigt AK01–09 desselben Freeze ohne offenen Scope-Blocker/High. |
| AK11 | Noch gesondert – zuständige Entscheidung, Unterintegration mit Expected-HEAD und tatsächliche integrierte Tree-/Diff-/Scope-/CI-Belege sowie Schlussstatus. |

Der frühere C1-FAIL, SCREEN-01 und beide verworfenen Screenversuche bleiben bytegleich historisch erhalten. C1s grüner CI-Lauf hat den High nicht geheilt; erst konkrete Korrektur, eigenständige native Prüfung, C2-Bytebindung und neue vollständige C2-CI schließen ihn. Unveränderte Source-/Solver-/Textbelege werden wiederverwendet; kein zusätzlicher allgemeiner Solver-/Textlauf oder eigener Editorstart durch diese Rolle.

Zulässiger nächster Schritt ist der **gesonderte zuständige Unterintegrationsentscheid** für genau C2 gegen exakt H; dieser Bericht führt keine Integration aus. Danach tatsächlicher integrierter Stand separat prüfen und persistieren. Main-/Produktionskampagnen-/Save-/Reward-/Fortschritts-/Geräte-/Store-/Releasegates bleiben unverändert separat. Reine spätere unveränderte Hinzufügung dieser Prüfdokumente benötigt die vertragliche schmale Delta-/Byte-/CI-Zuordnung, keine neue vollständige technische Sourceprüfung.
