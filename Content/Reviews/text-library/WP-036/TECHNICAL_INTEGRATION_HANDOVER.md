# WP-036 / WP-037 – Koordinatorübergabe zum technischen Integrationsnachlauf

**Stand:** 18.10.2026. **WP036 ist noch nicht technisch integrationsbereit.** Die Textbibliothek/Copy-Nacharbeit ist vollständig und kreativ angenommen; keine weitere Textproduktion oder pauschale Solverdiagnose beauftragen.

## Tatsächlicher Befund

Am unveränderten PR31-Head `ff1836e293a2a5cff69773525c347b55deb67dfb` bestehen beide Architecture-Läufe und inzwischen der vollständige Unity-PR-Lauf `39079578360`. Der Unity-Push `39079576211` bleibt dagegen FAILURE: ein HardBudget-Test überschreitet 180000 ms, Dauer 186.063155 s, 453/454 EditMode bestanden. Folgejobs lediglich skipped. Original-ZIP/Digest/XML eigenständig geprüft. Der spätere grüne PR-Lauf ist kein Nachweis einer Fehlerbehebung und löscht den FAIL nicht. Die ältere 89c8-PR-Stornierung ist ein anderer historischer Stand.

## Entscheidung und jetzt auszuführender Auftrag

[Geschäftsführerentscheidung WP-036-TECHNICAL-01](TECHNICAL_INTEGRATION_DECISION.md): bereits geprüfte WP034-Reparatur isoliert wiederverwenden, statt sie neu zu entwickeln oder den offenen WP026-Zweig zu importieren.

**Der konkrete Folgeauftrag ist bereits vor Code verankert:** [WP-037 – HardBudget-Reparaturübernahme für WP036](../../../../WORK_PACKAGES/WP-037_HardBudget-Reparaturuebernahme-fuer-WP036.md), [eigenes unveränderliches Production-Scope-Manifest](../../../../tools/architecture-validation/scopes/WP-037.production.scope.json).

| Übernahmeparameter | Verbindlicher Wert |
|---|---|
| Eigener Ausführungsbranch | `codex/wp-037-hardbudget-wp036` |
| Gemeinsamer Erst-Add von WP/Manifest | `4bfd4ee98153f64a948d2358c6ce30a49a441673` |
| Einziger Ankerparent / unveränderliche Manifestbasis | `ff1836e293a2a5cff69773525c347b55deb67dfb` |
| Bestehender Zielzweig / Ziel-PR | `codex/wp-036-vollstaendige-textbibliothek` / offener Draft-PR #31 |
| Main / PR31-Basis | unverändert `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |
| Vorhandene Reparaturquelle | `f34d6f4ca437effb0ce422015b02618b2a31a9c2` aus geprüftem WP034 |
| Ausschließlich lesbarer WP026-Vergleichsstand | `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d` |
| Kreativ angenommene Quelle | `de-DE.json`, SHA256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c` |

Die neue Instanz liest erneut AGENTS in seiner Pflichtfolge und das vollständige WP037. Sie setzt auf dem **tatsächlich veröffentlichten neuesten eigenen WP037-Head** fort und prüft dessen gemeinsamen historischen Anker sowie den erlaubten Vorbereitungsdiff; sie legt weder WP noch Manifest erneut an. Den tatsächlichen Vorbereitungshead und die Draft-Unter-PR-Nummer aus Remote/Git/Unter-PR feststellen; der [maschinenlesbare Bindungsnachweis](TECHNICAL_INTEGRATION_EVIDENCE.json) hält den unveränderlichen Anker, seine Basis und die geschützten Quellen fest. Neue CI am Vorbereitungsstand bleibt strikt getrennt von einer späteren Reparatur-CI; bei Übergabe noch laufende reguläre Prüfungen vor dem nächsten Push auslaufen lassen.

## Exakt begrenzte Fachänderung

Nur zwei vorhandene Hunks aus f34d6f4: `RootDeductionTracer.cs` (+12/−3) und `SolverV2MetricsTests.cs` (+17), einschließlich 1639er-Trace-/Metrikbindung. Ihre Vor-Kontexte sind am Ausgangsstand identisch mit WP034s Basis. `PuzzleSolver.cs`, `PuzzleSolverTests.cs` und sämtliche Budgetprüfungen bleiben unverändert; insbesondere ursprüngliche sechs Fälle, zwei Warm-ups, 25 Samples, Zustandsgrenzsuche, 40%-Assertion und 180000-ms-Testgrenze.

Der bereits vorhandene Manifestselektor wird ausschließlich auf WP037 gestellt; sonst kein Workflowwechsel. Keine WP034-Diagnosezeilen, WP028-Leistungsprüfung, WP026-Level-/Proof-/Korpusdateien oder fremde Commitstapel übernehmen. Kein neuer Golden-/Semantik-/Versionswechsel. Historische konkrete Host-/Scheduling-/GC-Ursache bleibt unbewiesen.

## Abnahme- und Integrationsgrenze

Nach Port: finale reguläre Architecture-/WP037-Scope-/Text-/JCS-/Unity-CI mit vorhandenen Zielprojektregressionen und neuer Tracebindung sowie gesonderte unabhängige technische Prüfung am selben Freeze. Eigenprüfung ist kein Rollen-PASS; alte WP034-CI ist keine neue Abnahme. Bei High-/Blockerbefund keine Integration.

Anschließend **prüffertige Unterintegrationsübergabe an die zuständige Entscheidung**. Kein automatischer Merge. Nach später ausdrücklich genehmigtem Untermerge neu gebundene SHA-/Tree-/Diff-/CI-Nachweise am tatsächlichen PR31-Integrationshead. Beide Scopeketten getrennt erhalten: Main→ff1836e (WP036), ff1836e→Reparatur-/Integrationshead (WP037). Das alte WP036-Manifest bleibt byteunverändert. Eine spätere Mainentscheidung wird nicht vorweggenommen.

**In dieser Vorbereitung nicht umgesetzt:** C#-Port, Reparatur-CI/-QC, Untermerge, Mainintegration. **Weiter unverändert offen:** Owner-/Produkt-/Runtime-/Import-/Rechts-/Release- und sämtliche Fremdgates; alle Texte DRAFT, WP026 weiterhin offen. Keine neue allgemeine Planung nötig: ausschließlich das vollständige verankerte WP037 ausführen.
