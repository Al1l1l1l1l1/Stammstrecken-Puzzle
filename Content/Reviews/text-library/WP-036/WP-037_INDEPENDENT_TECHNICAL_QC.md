# WP-037 – unabhängige technische Abschlussprüfung AK07

**Ergebnis: PASS auf dem unveränderten Liefer-HEAD `8dd68e22b58f126cb499fee408e98c173202a98e`. Keine offenen Blocker-, High- oder sonstigen technischen Befunde im Prüfauftrag.** AK01–AK06 sind unabhängig bestätigt; diese tatsächlich getrennte Rollenprüfung erfüllt AK07. Ein gesonderter zuständiger Unterintegrationsentscheid und die danach neu gebundene integrierte CI bleiben erforderlich. Dieser Bericht erteilt keine Merge-, Main-, Produkt-, Import- oder Releasefreigabe.

Prüfrolle: `/root/wp037_independent_qc`, getrennt von der bisherigen Implementierung und von der koordinierenden Integrationsrolle. Keine Implementierungsänderungen, keine Commits, Pushes, PR-Schreibaktionen, Builds oder CI-Retries durch diese Rolle. Eigene Ausführung ausschließlich im ignorierten `.utmp/wp037/independent-qc/`; als einziges neues Repositoryartefakt dieser eigene Bericht. Datum: 18.10.2026 (Europe/Berlin).

## Prüfgegenstand und Quellen

| Bindung | Geprüfter Wert |
|---|---|
| Liefer-HEAD / Tree | `8dd68e22b58f126cb499fee408e98c173202a98e` / `25b7907fee4e0741574c3f32867fbef03bfb1e96` |
| Codefreeze / Tree | `3638dd8ba39b1f4f6867fdad03ab70afb0de79af` / `411696a214427f91d4ec4b96dd7eeebf098b8449` |
| WP037-Anker | `4bfd4ee98153f64a948d2358c6ce30a49a441673` |
| Einziger Ankerparent / Manifestbasis | `ff1836e293a2a5cff69773525c347b55deb67dfb` |
| Manifest-SHA256 (Git-LF-Blob) | `022590d076f1e6a972b445b8696c56f83acccfebbaf7e1f30475a388e18e44a4` |
| Autorisierte WP034-Quelle | `f34d6f4ca437effb0ce422015b02618b2a31a9c2` |
| Frisch selbst gelesener PR32 | offen/Draft, Head exakt `8dd68e2`, Ziel `codex/wp-036-vollstaendige-textbibliothek`, Zielhead exakt `ff1836e` |

Die vollständige AGENTS-Lesefolge wurde selbst eingehalten: AGENTS, Projektübergabe 15, Konzeptindex 00, Master 09, CURRENT_STATE, WP037. Danach vollständig Verfahrensregeln/Übergaberegeln/DoD/Queue, ADR026/030/031, Solverarchitektur, Teststrategie, Build-and-Release, beide tatsächlichen Workflows sowie die im WP verankerten technischen Entscheidungen, WP036-Abschlussübergabe, WP034-Wiederverwendungsanalyse und WP037-Ausführungs-/CI-/Scopebelege gelesen. WP034/WP034-HANDOVER/QC/INTEGRATION wurden aus dem fest gebundenen Gitobjekt `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d` selbst gelesen. Historische WP034-Differential-/Mutationsbelege werden als solche wiederverwendet; keine neue allgemeine Solverprüfung, Korpusregeneration oder Textgesamtlektüre.

## Eigene Git-, Source- und Scopeprüfung

Der Anker besitzt exakt einen Parent `ff1836e` und führt ausschließlich WP037 und sein Manifest gemeinsam mit Status A ein. Es gibt genau einen von der Lieferung erreichbaren Manifest-Erstadd. Der historische WP-Blob bindet das kanonische Manifest per lokalem Link; IDs und Manifestbasis passen. Aktueller und historischer Manifestblob sind byteidentisch. Eigene `git diff --check` für Basis→Codefreeze und Basis→Lieferhead bestanden.

Der vollständige Zwei-Dateien-Patch ist bytegleich zum Patch des autorisierten WP034-Codecommits; SHA256 `a7e0ba53caf6ab5b73cf31078d042884c0f02001a85f2da8ba3761349ff31ced`. RootDeductionTracer +12/−3, Zielblob `a41a802fd3e3f2f54074bd0d1e0bf540d0ae5a6c`; SolverV2MetricsTests +17/−0, Zielblob `6be782565505376c960fc3f040dd1de95f3202bc`. Keine weitere fachliche Änderung. PuzzleSolver und PuzzleSolverTests bleiben bytegleich; keine zwölf Diagnosezeilen, kein WP028-Korpustest und kein fremder Commitstapel übernommen. Unityworkflow, Packages, ProjectSettings, übrige Produktions-/Testquellen, Architektur, ADRs, Schemata, Texttools, Lesefassung, kreative Annahme und alte Manifeste unverändert. Validate.yml wechselt ausschließlich den eigenen Manifestselektor.

Die View-Invariante wurde am echten Code nachvollzogen: `view[c] = 127 & ~Removed[j1] & ...` für genau alle eingeschlossenen Einträge der Zelle c. Bei Ausschluss von j kann nur dessen Zelle abweichen. Ihre Neuberechnung berücksichtigt alle noch eingeschlossenen Kandidaten und damit auch überlappende Masken. Bei misslungenem Holds werden Auswahlbit und gespeicherter Zellwert gemeinsam restauriert. Holds/MinimalFacts und ihre Hilfsprädikate lesen die View; sie verändern sie nicht. Reihenfolge von Kandidaten, jüngster-zuerst-Löschung, Faktenminimierung, publizierten Prämissen und Tiefen bleibt erhalten; Faulted-Pfad und finale Ergebnis-/Budgetbaufolge ebenfalls. Ein solver-v3-Wechsel ist für diesen exakt äquivalenten Port nicht begründet.

Basis→Lieferung umfasst 25 erlaubte Pfade; Codefreeze→Lieferung genau elf eigene Dokumentations-/Nachweispfade. Letzterer Diff enthält keine Assets, Packages, ProjectSettings, Workflows, Tools, Konzepte, Architektur, ADRs oder produktive Contentquellen. Die beiden Scopeketten bleiben getrennt: Main `d90e798`→`ff1836e` ist der unveränderte historische WP036-Textvertrag; `ff1836e`→`8dd68e2` ist ausschließlich WP037. Kein kumulativer solverhaltiger Diff wird durch ein verbreitertes WP036-Manifest legitimiert.

Die kanonische Textquelle hat selbst aus dem Liefer-Gitblob neu berechnet SHA256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c`; kreative Annahme WP-036-CREATIVE-02 bleibt erhalten.

Eigene neue kanonische Prüfungen im sauberen LF-Scratchcheckout exakt auf `8dd68e2`, unter Verwendung der vorhandenen gepinnten Python-/Node-/Lockumgebung:

```text
.utmp/wp029-venv/bin/python tools/architecture-validation/validate.py --self-test
.utmp/wp029-venv/bin/python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-037.production.scope.json --self-test
```

Beide Exitcode 0, 17 bzw. 18 Prüfgruppen PASS einschließlich Self-/Negativtests. Scopekontext exakt `workPackage=WP-037 base=ff1836e293a2a5cff69773525c347b55deb67dfb head=8dd68e22b58f126cb499fee408e98c173202a98e worktreeDirty=false`. Der erste nicht eskalierte WSL-Start wurde von der Umgebung abgewiesen und zählt als kein Test; die freigegebene lokale Ausführung wurde anschließend tatsächlich vollständig erfolgreich beendet. Keine Quelländerung oder Prüfabsenkung.

## Tatsächliche CI und Originalevidenz

Die vier Runmetadaten und beide vollständigen Unity-Job-/Artefaktinventare wurden frisch selbst über den lesenden GitHub-Connector abgefragt. Sämtliche folgenden Läufe sind COMPLETED/SUCCESS, Ereignis push, jeweils Attempt 1, mit exakt passendem SHA und Tree:

| Stand | Architecture | Unity | Jobs |
|---|---:|---:|---|
| Codefreeze `3638dd8` | 39155858104 | 39155858157 | Architecture + sämtliche zehn Unityjobs erfolgreich |
| Lieferhead `8dd68e2` | 39158530698 | 39158530663 | Architecture + sämtliche zehn Unityjobs erfolgreich |

Die regulären Workflowfilter gelten unverändert: pull_request nur gegen main; der Unter-PR gegen WP036 erhält vollständige Push-CI. Kein fehlender PR-Lauf wird erfunden. Cachehit-bedingt übersprungene Installationsschritte sind von übersprungenen Pflichtjobs unterschieden; sämtliche zehn Jobs und erforderlichen Test-/Buildschritte bestanden.

Die vorhandenen Originalpakete wurden unabhängig erneut vollständig dekomprimiert: jeweils 19/19 Payloads mit korrektem gzip-SHA256, Roh-SHA256 und Bytezahl. Darin alle elf vollständigen gespeicherten Joblogs, sechs Original-XMLs und zwei Original-Test-ZIPs. Codefreeze-Evidenz SHA256 `b897fa2fc65b3c33bd11778af8625f282fb42522bcc777cabbec7037d735261b`; Liefer-Evidenz SHA256 `9296c14ce2f9549d6807a3c4e2ce7b4a6ad761f44b1f39e15dda8d22f77682b3`. Lieferoriginale und alle Digests sind zusätzlich im frisch gelesenen PR32-Schlussnachtrag gebunden. Gespeicherte vollständige Logs tragen echte Checkoutausgaben, nicht nur API-head_sha. Architecture-, Preflight-, EditMode-, PlayMode-, Android- und iOS-Checkouts passen jeweils exakt zum jeweiligen Freeze.

Beide Architecture-Originallogs bestätigen 17/18 Gruppen, korrekten Scopekontext mit sauberem Baum, CPython 3.11.13/Node 22.20.0, JCS 12 positiv/32 negativ/2 v2/2 v1, Text 1.408/960/240 mit zwölf erkannten Mutationen und exakte Lesefassung. 480 fremde Referenzziele bleiben ausdrücklich offen.

| Selbst geparstes Original | Codefreeze | Lieferhead |
|---|---|---|
| EditMode | 455/455 Passed, kein Failed/Skipped | 455/455 Passed, kein Failed/Skipped |
| Original HardBudget | Passed, 72.442419 s | Passed, 46.632770 s |
| 1639er-Tracebindung | Passed, 0.513261 s | Passed, 0.343423 s |
| PlayMode/QA | 27/27 Passed | 27/27 Passed |
| Coverage | beide Sitzungen je 15 STP-Assemblies | beide Sitzungen je 15 STP-Assemblies |

Alle 56 Solverfälle einschließlich Budget/Cancellation/rückläufiger Uhr, Klassifikation, Trace-Sufficiency/Irredundanz/Auditor und Kampagnengate sind als tatsächlich Passed vorhanden; ebenso 19 Domain- und 379 Contentfälle samt Hash-/Proof-/Strict-Regressionen, plus ein Unity-Beispielfall. Keine WP034-Testzahl ungeprüft übertragen.

Beide Original-HardBudget-Ausgaben enthalten alle sechs vorgeschriebenen Fälle; unterbrochener Langpfad weiterhin maxStates=2. Alle fünf großen excess/build-Verhältnisse selbst neu berechnet: maximal 0.1568627451 bzw. 0.1627906977, beide <0.4. Originale Fälle, 2 Warm-ups, 25 Samples, Zustandsgrenzsuche und Assertion bytegleich. Präzisierung des historischen WP-Wortlauts: Der Originaltest besitzt kein explizites TimeoutAttribute; unverändert bleibt der beobachtete 180000-ms-Runnerdefault. Der interne Solverrahmen bleibt 600000 ms. Es wurde kein Attribut hinzugefügt und keine Zeitgrenze angehoben. Dies korrigiert eine bereits in Übergabe/PR erklärte Benennung, keine Akzeptanzbedingung.

Der neue Test ist exakt der vorab gebundene WP034-Test: UNIQUE, Metriken `(0,1639,30,0)`, vollständige 1639er-Spur mit SHA256 `6E59FFFAFB16B61C9FDDC301ED9229078FEEA11BEF1E2F44616CE7F9D8460B78`. Alle Schlussfolgerungen, Prämissen/Fakten samt Reihenfolge und Tiefe gehen in die Hashprojektion ein. Keine angepassten Goldenwerte.

| Original-ZIP | Codefreeze: ID / selbst berechneter SHA256 = frischer GitHub-Digest | Lieferhead: ID / selbst berechneter SHA256 = frischer GitHub-Digest |
|---|---|---|
| EditMode | 12147470438 / `779e6b842d863cd746b12a70dcfb5d459a936266e7c8ff8b99d0505896c7fa6c` | 12148226111 / `a9d210b03baa1913531189afc022efc92d6ec8c47f7f74777182d0ee2e5ba236` |
| PlayMode | 12147630430 / `e8a6d5271f33875f1932efdbc0849c43021b9f296ecf7be0fff996d9ecb31584` | 12147757814 / `da4f508403d00f76643ca12f636416996e7bceddd5fcfc44303a8b45f4f2013d` |
| Android | 12147785393 / `145e6f6ab76991bad9917e572868d56b6e7779cdf5a3eb292b1dd55edc14ab45` | 12147364466 / `601fcad8061ef6603f9bd9ea0864930283ad36e171b15bc27803596ad5ad99af` |

Original-EditMode-XMLhashes `3e36665baf1944e70809a11226ce9591af96390906276e869787701e4b993bd9` / `9bc66254fc582838817e637c65de969882995c010f36f7a8a159bbefdb87eba1`; PlayMode `ba7de22e30027a0e54c1c2b4cae14da244438e175e5dcc6304764a58a29d8051` / `d25e473c8b3f63ce8e816514f4a41f8afa65d752616e67aa22a17953ba05a2c0`.

Beide Android-ZIPs selbst geöffnet, APKs gehasht und AndroidManifest/native ARM64-libIL2CPP tatsächlich vorgefunden. APKs 40066246/40066238 Bytes; SHA256 `8a98d7b6519f59f8ea650eada2d1df306f6a282deced9eb3fb5a7f763342120a` / `ea3c6a611e06529b232a4951b146c23a99a4c4d6b19135b67447cc9cc30d004f`. Beide vollständigen iOS-Logs bestätigen passenden Checkout, Xcode 26.3, Unityexport und BUILD SUCCEEDED bei deaktivierter Signierung. Gespeicherte iOS-Loghashes `bb21cc71759817914c0868bcaf0cc79d4ebab4fc08b9230627fa2814c093d860` / `c23d4e08e6c0d3c241fd96f8fe45510deb4c38630ead37161ac5a3e0d454fa40`. Loghashes gelten ausdrücklich für das dokumentierte dekodierte UTF-8/LF-Connectorprofil; ZIP-/XMLbytes bleiben unverändert, keine behaupteten Roh-HTTP-Loghashes.

## Historie, Akzeptanz und nächste Zuständigkeit

Die vier tatsächlichen ff1836e-Runs wurden frisch selbst bestätigt: Architecture 39079576265/39079578380 SUCCESS; Unity-Push 39079576211 FAILURE; Unity-PR 39079578360 SUCCESS, jeweils Attempt 1. Unveränderte persistente Original-ZIP-/XML-/Logbindungen in TECHNICAL_CI_AUDIT.md/.json gelesen: 453/454, 186.063155 s bei 180000 ms, Folgejobs skipped. Historischer FAIL bleibt FAIL; späterer PASS löscht ihn nicht. Keine konkrete Host-/Scheduling-/GC-Ursache behauptet. Frühere WP034-Wirkungs-/Differential-/Mutationsbelege gelten als quellengebundene Wiederverwendungsevidenz, ausdrücklich nicht als neuer eigener Lauf.

| AK | Unabhängiges Ergebnis |
|---|---|
| AK01 | PASS: eigener gemeinsamer historischer Anker, Parent, IDs/Link, unveränderte Manifestbytes und neuer eigener sauberer Scope-PASS. |
| AK02 | PASS: vier tatsächlich terminale Ausgangsruns und unveränderte Originalbindungen; FAIL/PASS/Skipped korrekt getrennt. |
| AK03 | PASS: exakter Zwei-Hunk-Port, Quell-/Zielblobs, View-/Restore-Invariante und sämtliche geschützten Pfade bestätigt. |
| AK04 | PASS: ungeschwächter Original-HardBudget und alle vorhandenen Zielprojektregressionen in beiden tatsächlichen regulären XMLs Passed. |
| AK05 | PASS: unveränderte neue Trace-/Metrik-/Hashregression in beiden tatsächlichen XMLs Passed. |
| AK06 | PASS: vollständige neue Codefreeze- und Lieferhead-CI, Originalpayloads/Checkouts/Artefakte/Plattformen selbst geprüft. |
| AK07 | PASS: vorliegende tatsächlich getrennte technische Abschlussrolle; keine offenen Befunde. |
| AK08 | Bestätigt für die Lieferung: Schutzbindungen, beide Scopeketten und richtiges Draft-Ziel erhalten; Unterintegration benötigt weiterhin gesonderten Entscheid. |
| AK09 | Technische Lieferung und prüffertige Übergabe nachvollziehbar; die Koordination muss dieses neue Rollenurteil, ihren separaten Entscheid und gegebenenfalls die tatsächliche Unterintegration samt neuer CI additiv nachführen. |

DoD im technischen Prüfgegenstand erfüllt: Port vollständig, vorgeschriebene Regressionen/Builds nachgewiesen, keine undokumentierte Architekturänderung oder offene Implementierungslücke. Reiner eigener nachfolgender Dokumentationsdiff benötigt gezielte Delta-/Scopeprüfung und seine tatsächliche reguläre CI-Bindung; unveränderte Text-/Solvergesamtprüfungen sind nicht zusätzlich erforderlich.

Der geprüfte Liefer-HEAD kann nach gesonderter zuständiger Entscheidung mit Expected-HEAD=`8dd68e2` und erneuter Prüfung des Zielheads `ff1836e` unterintegriert werden. Dieser Bericht selbst trifft diese Entscheidung nicht. Nachher sind Integrations-SHA/Tree, Lieferbaumgleichheit, getrennte Scopeketten und neue reguläre CI tatsächlich zu prüfen und zu persistieren. Mainintegration und sämtliche DRAFT-/Owner-/Produkt-/Runtime-/Import-/Rechts-/Zeit-/Asset-/Geräte-/SDK-/Store-/Release-/Fremdgates bleiben offen.

Eigener nachvollziehbarer Prüfscratch: `.utmp/wp037/independent-qc/check.py`, `checks.json`, `architecture.txt`, `scope.txt`; der Prüfer hat keine vorhandenen Autorberichte überschrieben. Letzter abgeschlossener Schritt: unabhängige gezielte AK07-Abnahme auf `8dd68e2` PASS. Nächster Schritt: separate zuständige Unterintegrationsentscheidung, danach tatsächliche Integration und integrierte CI-/Closeoutnachführung.
