# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. **WP-021 bis WP-025 sind abgeschlossen und auf `main` integriert.** Puzzle-Kern, Level-v2-/Hash-Foundation, Solver-v2 und Produktions-Proof-v1-/Strict-Validation sind geliefert. Keine Produktionscontent- oder Releasefreigabe. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 und WP-021 bis WP-025 ebenfalls abgeschlossen und integriert. WP-025 bestand nach Korrektur des Befunds QC-STRICT-01 beide unabhängigen Abschlussrollen auf `92842bc653bf364bd7ade5bbff7c37f71bf9fd1f`: PASS, AK-B01 bis AK-B07 erfüllt, keine offenen Befunde. Der Auftraggeber erteilte am 13.10.2026 mit „übernehmen“ die ausdrückliche Integration samt Abschlussdokumentation. PR #19 wurde mit Expected-HEAD-Absicherung als `f496ab9001d2043a17c3ffd2ff2a8a39d175495b` integriert. Vorab angelegte WP-015/WP-016/WP-017 bleiben historische Planungsquellen, keine ausführbaren Trust Anchors. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. Die integrierten WP-023-/WP-024-/WP-025-Arbeitsbranches sind Historie; maßgeblich ist `main`. |
| Letzter verifizierter Nachweis | **WP-025:** [Proof-/Vertrags-QC PASS](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6150287241) und [Strict-/Tests-/Scope-/CI-QC PASS](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6150288578), beide auf `92842bc653bf364bd7ade5bbff7c37f71bf9fd1f` gegen `5f5127362b101dd4e066b68b21d3e35a5af314df`. [Ausdrückliche Freigabe](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6154248884). Architecture PR 38421061552 und Unity PR 38421061511 SUCCESS. Der tatsächliche Integrationscommit enthält gegenüber dem abgenommenen HEAD keine Dateiänderung; Git-Baum `3159dfe2349d6273fa923eda87dba1164559d024` erhalten. |
| Aktuell zu prüfender Schritt | WP-029: F-01 und aktuelle Abnahme-/Statusnachweise; WP-026 bleibt offen. Gesamtabnahme b1457ba durchgeführt, kein Gesamt-PASS. |
| Nächster vorgesehener Schritt | Separate O-02-Ursachenuntersuchung und O-04-Integrationsvorbereitung, danach gezielte unabhängige Prüfung des neuen Freeze; kein Merge nach main. |
| Produktionscode | Auf `main`: WP-021-Scaffold; WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer; WP-023-Level-v2-/Hash-Foundation mit Parser, Domainabbildung, JCS, profilierten Hashes und neutraler Migration; WP-024-`solver-v2` mit auditierbarer Rootspur, vier ADR-031-Metriken und fail-closed CampaignGate; **WP-025** mit immutable Proof-/Ergebnismodellen, geschlossenem Proofcodec, Produktionsgenerator, vollständiger Bindung, frischer bytegenauer Regeneration, gemeinsamer LevelValidationPipeline und Strict-Einzel-/Batch-/Commandsemantik. Zusatzdiagnosen bei Duplikaten bleiben erhalten und deterministisch geordnet. Die neutrale Migrationsgrenze entspricht den vier nichtnegativen Proofmetriken. |
| Fortbestehende Produktionsgrenze | Technisch gültige Proofs passieren ausschließlich das Proofgate. Katalog-, Cross-Reference-, Produkt- und Release-Lock-Gates werden nicht umgangen. `IsRuntimeImportable` und `IsReleaseLockEligible` bleiben `false`. Drei generierte Goldens sind FIXTURE_ONLY, keine Season-1-Produktion. Keine UI-/Asset-/Generator-/Monetarisierungsarbeit durch WP-025. |
| Architekturentscheidungen | Architecture v1.0 unverändert; ADR-030 regelt historische Scope-Verankerung, ADR-031 den integrierten Solver-v2-/Proofregenerationsvertrag. Keine neue Architektur- oder Produktentscheidung durch Integration oder Closeout. |
| Scope-Vertrauensanker WP-023 | Gemeinsamer Erst-Add `a1d284caec7746a4aa48fbe564d37d179d60fed8`; einziger Elterncommit/Manifestbasis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`; Manifest unverändert. Historischer abgeschlossener Lieferstand. |
| Scope-Vertrauensanker WP-024 | Gemeinsamer Erst-Add `3e7f1ab2e56853294a4f09650d8ec361ab4eca1a`; einziger Elterncommit/Manifestbasis `ab117f459689d2b2937d436bb1a763296a65d52a`; Manifest-SHA-256 `2055ed1a9e8985fb6898ad6f914e7775ff1ae75148118a6e1b8086d20b69e631`, unverändert. |
| Scope-Vertrauensanker WP-025 | Gemeinsamer Erst-Add von WP-025 und eigenem Manifest in `de314dbd6e928d4ffb8893fe4f1ee7e6c7873132`; einziger Elterncommit/Manifestbasis `5f5127362b101dd4e066b68b21d3e35a5af314df`; Manifest-SHA-256 `1a30063cf00771e053b39b6ee37405ba25d7310600ad8bb158ac8055194ed44d`, unverändert. Der historische Anker wird durch Abschlussnachführung nicht verändert. |
| CI WP-025 | Finale fachliche Nachweise auf `92842bc…`: Architecture PR 38421061552 und Unity PR 38421061511 SUCCESS; Unity zehn erfolgreiche Jobs mit 454/454 EditMode, 27/27 PlayMode/QA, Coverage, Android-IL2CPP und iOS/Xcode. Getrennte tatsächliche Push-/Vorläufernachweise und lokale Node-/Zusatztests stehen in WP-025 und PR #19. Abgebrochene historische Runs sind kein PASS. |
| Evidenzherkunft | Beide vollständigen unabhängigen Rollenberichte wurden gelesen und dem finalen HEAD zugeordnet. Die Koordination hat aktuelle Remote-/CI-Metadaten und Inhaltsgleichheit der Integration geprüft sowie die heruntergeladenen EditMode-/PlayMode-ZIPs selbst gehasht und deren XML ausgewertet. Die lokalen Prüferexperimente und die fachliche Gesamt-QC wurden nicht nochmals ausgeführt; keine dritte Abnahme. |
| CI-Follow-up | Der Closeout ändert ausschließlich WP-025, CURRENT_STATE.md und WORK_QUEUE.md. Produktionscode, Tests, Schema, Goldens, Architektur, Workflows und Manifeste bleiben unverändert. Seine nachgelagerten CI-Läufe werden am Closeout-PR separat mit tatsächlichem Commit/Status belegt, nicht vorab als PASS ausgegeben. |

## WP-026: aktuelle Nachführung zur Gesamtabnahme – 15.10.2026

Maßgeblicher geprüfter Lieferstand: `b1457ba6a61826fe3b64a83323c27a13e9d23080`; Quellenstand main: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Historische Manifestbasen bleiben WP-026=c9516bd und WP-028=d97c39d. Keine Umdeutung des PR-Metadatenfelds base_sha zu aktuellem main.

**WP-027 entscheidet R01-FIRST und R18-MIDDLE.** Bible §§4.1/8.1: erster gezielter Variationsschwerpunkt, frühere Portseitenvielfalt zulässig; vorbereitender lokaler Einser-Portanker zulässig, weitere Lösung kombiniert mittlere Resthinweise. Keine neue Port-/Einserquote. Quellen: [WP-027 auf main](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/WORK_PACKAGES/WP-027_Season-1-Quellenklarstellung-und-CI-Leistungsweg.md), dort bestätigte Bible und ADR-032.

**CI-PERF-WP026 auf b1457ba erbracht.** WP-028: Freeze b449ab7, PR #25 ausschließlich nach WP-026 integriert als b1457ba, gleicher Tree 2bc50e6. Aktuelle Unity-CI [38637012837](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012837): 455/455 EditMode, 27/27 PlayMode, Android IL2CPP und iOS/Xcode erfolgreich; Architecture [38637012841](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012841) erfolgreich. 350 Samples, p95-Index 332, p95 38,2026 ms, Maximum 72,0952 ms, 240 frische Proofgates in 2.586,8097 ms mit vier Arbeitern. Git/Linux-Fingerprint 6fc307231c43d3f09c60fa3116c8517588a40855c4266e8ec4dd120ed0b6372a; Windows-CRLF-Fingerprint 29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679. Dies sind Nachweise dieses historischen Standes, kein vorweggenommener PASS neuer Metadaten.

**Neue unabhängige Gesamtabnahme durchgeführt, KEIN GESAMT-PASS / NICHT INTEGRATIONSBEREIT.** [Originalbericht](../Content/Reviews/season-1/WP-026_Gesamtabnahme_b1457ba.txt), byteunverändert, SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433. Evidenzarchiv SHA256 e1a974ebfdf1f87ee9af6bb08981563e8ce3a5a3166c0c21aada92d81cfb4258, ausschließlich isoliert. Alle 240 logischen Rätsel/20 Routen gegen WP-027 geprüft; F-01 begrenzter Sachtextbefund, keine neue Puzzlelogikabweichung. Originale frühere Prüfberichte bleiben unveränderte standgebundene Historie.

F-01 wird unter dem bestehenden WP-026-Metadatenscope und separat verankerter WP-029-Nachweisführung korrigiert: x0=0, x1=2, x3=6, x4=5. Ein Level Revision 2→3, kein neuer öffentlicher Input, keine neue Lösung oder Proofbytes. Reale neue Bindungs-/Regenerationsnachweise und Grenzen stehen in [ACCEPTANCE_FOLLOWUP.md](../Content/Reviews/season-1/ACCEPTANCE_FOLLOWUP.md). Gezielte unabhängige Nachprüfung auf neuem Freeze bleibt erforderlich.

Weiter offen: O-01 menschlicher Spieltest, O-02 historische HardBudget-Timeoutursache, O-04 drei tatsächliche Main-Konflikte/noch ungeprüfter gemeinsamer Stand, O-05 1.440 fehlende Referenzziele, O-06 unbestimmte Zeitklasse sowie O-07 kreative/Import-/Produkt-/Release-/Geräte-/Storegates. Kein Gesamt-DoD-PASS, Merge nach main, Runtimeimport oder Release. O-02/O-04 erhalten getrennte Untersuchungs-/Vorbereitungsaufträge; kein bestehendes Paket oder Manifest wird erweitert.

Die nachfolgenden älteren Abschnitte beschreiben ihren damaligen Stand. Ihre überholten Entscheidungs-/CI-PERF-Statusangaben sind keine aktuelle Blockerliste.

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten ersetzen keine Projektquelle. Die vorhandenen WP-015/WP-016/WP-017 sind historische Planungsgrundlagen und keine ausführbaren Trust Anchors. Die vollständigen unabhängigen WP-025-Abschlussberichte und die Auftraggeberfreigabe sind am PR persistiert und im WP direkt verlinkt. Prüferexperimente bleiben Evidenz der jeweiligen Prüfer.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 abgeschlossen und integriert:** Level-v2-/Hash-Foundation unabhängig PASS auf `f56eab34…`, Geschäftsführungsfreigabe; PR #15 / `10405370967496f51be9e5c00c381fd8052ec072`.
4. **WP-024 abgeschlossen und integriert:** Solver-v2 nach zwei korrigierten QC-FAIL-Runden unabhängig PASS auf `64d8801…`, Geschäftsführungsfreigabe; PR #17 / `22ff74558259c7ba1f9a157280e4d70b909bac26`, Closeout PR #18.
5. **WP-025 abgeschlossen und integriert:** Proof-v1/Strict-Validation nach Korrektur QC-STRICT-01 mit beiden unabhängigen PASS-Berichten auf `92842bc…`, Geschäftsführungsfreigabe; PR #19 / `f496ab9001d2043a17c3ffd2ff2a8a39d175495b`.
6. **Contentproduktion:** bleibt hinter der vollständig integrierten erforderlichen Pipeline und einem eigenen passenden Auftrag. Die WP-025-Integration erfüllt die Proofblock-Voraussetzung, nicht automatisch alle weiteren Produktionsgates.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert. Physische Geräte-, SDK-, Privacy-, Signing- und Storeprüfungen bleiben spätere Nachweise. Die technische WP-025-Abnahme ist keine Spielfreigabe oder Releaseabnahme.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `WORK_QUEUE.md` | Unveränderte Priorisierung; WP-025 abgeschlossen, WP-026 in externer Übergabe. |
| `../WORK_PACKAGES/WP-026_Season-1-Logischer-Raetselbestand.md` | Fortgeführter Auftrag, Auftragskorrektur, 240er-Lieferung und tatsächliche AK-/DoD-Grenzen. |
| `../Content/Reviews/season-1/HANDOVER.md` | Prüffertiger logischer Bestand, echte technische Nachweise und offene externe Abnahme-/Referenz-/Folgegates. |
| `../WORK_PACKAGES/WP-025_Proof-v1-Regeneration-und-Strict-Validation.md` | Vollständiger abgeschlossener Auftrag mit historischer Vorbereitung, Implementierung, Korrektur, beiden finalen Abnahmen, Freigabe und Integration. |
| `../tools/architecture-validation/scopes/WP-025.production.scope.json` | Unveränderlicher Scope ab gemeinsamem Anker de314db auf Basis 5f51273. |
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Integrierter Level-v2-/Hash-Block. |
| `../WORK_PACKAGES/WP-024_Solver-v2-Metriken-und-Deduktionsspur.md` | Integrierter Solver-v2-Block und seine historische QC. |
| `../WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` | Unveränderte historische Planungsquelle von WP-025. |
| `../ARCHITECTURE/CONTENT_PIPELINE.md` | Gelieferter Proof-/Strict-Pfad und weiterhin offene nachgelagerte Produktionsgates. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Verbindlicher Solver-v2-/Proofvertrag. |
| `DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.

## WP-030: Timeoutuntersuchung – 15.10.2026

Originaltimeout analysiert, sechs abgeschlossene Fälle und gleiche Zustandsgrenze 2 im alten FAIL/PASS belegt. Große 32×32-Budgetfälle im Push rund doppelt so langsam; nicht alle Tests pauschal verlangsamt. Neue instrumentierte Originaltestprobe, Quellgleichheit und hoher Allokationsaufwand dokumentiert. Ursache nicht bewiesen, O-02 nicht geschlossen, keine Reparatur. Konkreter eng begrenzter Testdiagnostikbedarf zur Entscheidung: [WP-030-Bericht](Investigations/WP-030_TIMEOUT.md). Nächster beauftragter Schritt: WP-031 isolierte konfliktaufgelöste Kombination tatsächlich prüfen und verlustfreie Originalberichtpersistierung korrigieren; unabhängige Nachprüfung bleibt erforderlich.
