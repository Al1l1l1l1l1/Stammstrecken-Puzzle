# WP-036 – Entscheidung zum technischen Abschluss- und Integrationsweg

**Entscheidungs-ID:** `WP-036-TECHNICAL-01`
**Datum:** 18.10.2026
**Entscheidungsrolle:** Manus in der ausdrücklich bestehenden Geschäftsführerrolle; keine behauptete Ownerannahme der Texte und keine Main-/Produktfreigabe.

## Verbindliche Entscheidung

**WP-036 ist aktuell noch nicht technisch integrationsbereit.** Der inzwischen vollständige Unity-PR-PASS ist positiv, aber am selben Lieferhead bleibt ein realer Unity-Push-FAIL im wiederkehrenden HardBudget-Test. Der bekannte Zuverlässigkeitsbefund blockiert nach TEST_STRATEGY §13; er wird nicht durch einen späteren grünen Lauf, technische GitHub-Mergefähigkeit, Retry, Quarantäne oder eine größere Zeitgrenze beseitigt.

**Autorisiert ist die isolierte Wiederverwendung der vorhandenen WP-034-Reparatur unter dem neuen WP-037, nicht eine neue allgemeine Solverdiagnose.** Nur zwei bereits quellengeprüfte Fachhunks: RootDeductionTracer-Aufwandskorrektur und die 1639er-Trace-/Metrikregression. Der vorhandene HardBudget-Test bleibt unverändert. Vor jeder Codeübernahme wurde ein eigenes vollständiges WP und enges Manifest gemeinsam verankert; das alte WP036-Manifest wird nicht verbreitert.

[Konkreter ausführbarer Auftrag WP-037](../../../../WORK_PACKAGES/WP-037_HardBudget-Reparaturuebernahme-fuer-WP036.md) und [eigenes unveränderliches Manifest](../../../../tools/architecture-validation/scopes/WP-037.production.scope.json). Gemeinsamer Erst-Add: **`4bfd4ee98153f64a948d2358c6ce30a49a441673`**, einziger Parent/Manifestbasis **`ff1836e293a2a5cff69773525c347b55deb67dfb`**. Eigener Kindbranch **`codex/wp-037-hardbudget-wp036`**. Der Erst-Add enthält ausschließlich diese zwei neuen Dateien; sein tatsächlicher kanonischer Production-Scope mit Self-/Negativtests besteht mit 18 Prüfgruppen. Diese Aussage ist ein Anker-/Scopebeleg, **kein Reparatur- oder CI-PASS**.

## Tatsächlich verifizierter Ausgangsstand

| Gegenstand | Bindung |
|---|---|
| WP-036 / PR31 | `codex/wp-036-vollstaendige-textbibliothek`, offen/Draft, unverändert `ff1836e293a2a5cff69773525c347b55deb67dfb` |
| Main / PRbasis | unverändert `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |
| Kreativ angenommener Textfreeze | `89c8d2b90e08ad7b1ba01d5b122763d2ae1fdd8b` |
| Einzige kanonische Textquelle | `Content/Localization/source/de-DE.json` |
| Source-SHA256 | `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c` |
| WP034-Quellfreeze | `f34d6f4ca437effb0ce422015b02618b2a31a9c2` |
| Vorhandene WP034-Unterintegration | `085357d59a62dae73189ea76051c73f0ffb78ae3`, ausschließlich im WP026-Zweig |
| Aktueller ausschließlich lesbarer WP026-Stand | PR21 `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d`, weiterhin offen/Draft |

Aktuelle PR-Metadaten und spätere Kommentare wurden gelesen. Der ältere PR31-Haupttext zur f217-Lieferung ersetzt weder die neue kreative Restannahme noch die folgende CI-Evidenz am ff1836e-Head. Auch der diff 89c8→ff1836e ist nur zwölf eigene Dokumentations-/Nachweis-/Statusdateien; keine Solver-/Test-/Unityworkflowänderung. Unabhängiger Vergleich von 118 relevanten Blobs bestätigt ff1836e = 89c8 = Main im Solver-/Unity-Pfad.

## Neuer CI-Befund – nicht die alte Stornierung

| Tatsächlicher ff1836e-Lauf | Ergebnis |
|---|---|
| Architecture Push `39079576265` | COMPLETED / SUCCESS |
| Architecture PR `39079578380` | COMPLETED / SUCCESS |
| Unity Push `39079576211` | COMPLETED / FAILURE; EditMode-Job `117533648377` fehlgeschlagen |
| Unity PR `39079578360` | inzwischen COMPLETED / SUCCESS, sämtliche zehn Jobs erfolgreich |

Der Push-FAIL wurde am Original-ZIP `12118003255` geprüft: Downloadhash = GitHub-Digest **`b4b6647c85ce1f371762949e10c3182907d2735785a5aac8fb84c2b54ec98968`**. NUnit-XML: 454 total, 453 Passed, ein Failed. Test `STP.Tests.Solver.EditMode.PuzzleSolverTests.HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind`, Timeout **180000 ms**, Dauer **186.063155 s**. PlayMode/Android/iOS danach SKIPPED, keine drei unabhängigen weiteren Fehler.

Der separate spätere PR-PASS wurde konkret an Original-NUnit-XMLs bestätigt: 454/454 EditMode und 27/27 PlayMode; HardBudget besteht dort in 83.671989 s. Am ff1836e-Head stehen insgesamt 22 Check-Runs: 18 SUCCESS, ein FAILURE, drei SKIPPED. Kein „22 Checks PASS“. Der spätere PR-PASS wird nicht verworfen, aber nicht als Reparatur oder Löschung des Push-FAIL ausgegeben.

Die ältere 89c8-Evidenz bleibt getrennt: dort vollständiger Unity-Push SUCCESS und Unity-PR CANCELLED. Jetzt liegt dagegen ein tatsächlicher Testtimeout am späteren ff1836e-Head vor. [Gesonderte lesende CI-Einordnung](TECHNICAL_CI_AUDIT.md), [maschinenlesbare Originalbelegbindung](TECHNICAL_CI_AUDIT.json).

## Warum gerade die vorhandene Arbeit wiederverwenden?

[Lesende Fachanalyse](WP034_REUSE_ASSESSMENT.md) belegt die isolierbare Patchmenge und identische Vor-Reparatur-Kontexte in WP034-Basis a2e26a6, Main und WP036. Der Produktionshunk verändert ausschließlich `RootDeductionTracer.Record`: statt wiederholter kompletter View-Rekonstruktionen wird die View einmal aufgebaut und bei Prämissenwegfall nur die betroffene Zelle mit exakter Restore-Semantik rekonstruiert. Die zugehörige neue Regression bindet UNIQUE, 1639 Spurschritte, Metriken `(0,1639,30,0)` und den unveränderten Tracehash.

WP034 dokumentiert eigene Differential-/Mutations-/Semantiknachweise und unabhängige QC am f34-Freeze sowie neue vollständige CI seiner Unterintegration. Das ist belastbare Wiederverwendungsevidenz. Es ist **keine Garantie**, dass jeder Host nie mehr ein Timeout zeigt, und keine neue Abnahme des WP037-Zielstands. Die historischen oder neuen konkreten Host-, Scheduling- und GC-Ursachen bleiben unbewiesen.

Das bestehende `PuzzleSolver.cs` und seine finale CheckBudget-/Ergebnisbaufolge werden nicht geändert. Der zwölfzeilige WP034-Diagnosehunk in `PuzzleSolverTests.cs` wird nicht portiert; dessen bestehender HardBudget-Test bleibt byte- und parametergleich. Kein WP026-Korpus, WP028-Leistungstest, ADR032-Benchmark, Proof, Level oder fremder Commitstapel gelangt in den Lieferbaum.

## Zulässiger Weg und noch fehlende Nachweise

1. **Erledigt:** konkrete Basis-/Patch-/Befundentscheidung, geprüfte unvergebene ID 037, eigener historischer WP-/Manifest-Erst-Add vor Code.
2. **Diese Vorbereitung:** eigene Befund-/Entscheidungs-/Statusdokumentation und ausschließlich der bestehende Architecture-Manifestselektor auf WP037. Keine C#-Übernahme. Ein normaler veröffentlichter Vorbereitungsstand erhält nur seine eigenen, separat auszuweisenden automatischen CI-Ergebnisse.
3. **Nächster ausführbarer Schritt:** auf dem veröffentlichten eigenen WP037-Head fortsetzen; Anker/Basis und unveränderte Quellen prüfen; ausschließlich die zwei verankerten Hunks übernehmen. Bestehende laufende Vorbereitungsprüfungen vor weiteren Pushes auslaufen lassen.
4. **Danach erforderlich:** neue tatsächliche reguläre Architecture-/WP037-Scope-/Text-/JCS-/Unity-CI am exakt benannten Codefreeze; unveränderte HardBudget-Regression, 1639er-Tracebindung und die bestehenden Zielprojektregressionen. Original-XMLs/-Logs/-Digests/-Checkoutbindungen prüfen. Kein Retry bis Grün und keine allgemeine Solver-Neuprüfung als Standardreaktion.
5. **Vor Unterintegration:** vom Portieren getrennte unabhängige gezielte technische Abschlussprüfung ohne offenen Blocker/High, dann gesonderte zuständige Unterintegrationsentscheidung. Draft-Unter-PR ausschließlich gegen den WP036-Zweig. Kein automatischer Merge und keine Mainintegration.
6. **Nach später freigegebener Unterintegration:** Expected-HEAD, Ziel-HEAD, integrierten SHA/Tree/Diff und neue tatsächliche CI am PR31-Integrationsstand dokumentieren. Main→ff1836e bleibt am ursprünglichen WP036-Vertrag gebunden; ff1836e→Reparatur-/Integrationshead am unveränderten neuen WP037-Vertrag. Kein rückwirkender Scopewechsel oder gemischtes Ersatzmanifest. Mainentscheidung bleibt separat.

## Unveränderte Annahme- und Produktgrenzen

Die gesamte Textbibliothek einschließlich 110 Keys/120 korrigierten Feldern bleibt kreativ angenommen durch WP-036-CREATIVE-02. Keine neue Textkorrektur oder kreative Gesamtlektüre. Quelle, Metadaten, Lesefassung, Schema, Texttools, Kreativscope und sämtliche historischen QC-/CI-Urteile bleiben unverändert.

Alle 1.408 Einträge bleiben DRAFT. Keine ausdrückliche Ownerzustimmung, Runtime-/Import-, Katalog-/Cross-Reference-, Hint-/Daily-/Endless-, Asset-, Rechts-, Geräte-, SDK-, Datenschutz-, Signing-, Store- oder Releasefreigabe. WP026 bleibt offen und wird nicht integriert; fremde Produktionsblöcke/Prioritäten werden nicht mitfreigegeben.

**Aktueller Abschluss dieser Entscheidung:** Autorisierungs-/Vorbereitungsauftrag erfüllt; technische Reparatur, neue finale Code-CI/QC und Unterintegration noch nicht ausgeführt. [Koordinatorübergabe](TECHNICAL_INTEGRATION_HANDOVER.md).
