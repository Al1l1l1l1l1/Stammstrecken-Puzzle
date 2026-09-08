# WP-001 – Technische Produktionsspezifikation

## ID

`WP-001`

**Bearbeitungsstatus:** **Abgeschlossen am 2026-09-07.** Dieses Work Package wurde gemäß ausdrücklichem Auftrag in derselben Sitzung angelegt, bearbeitet, unabhängig überprüft und auf dem vorgesehenen Branch abgeschlossen.

## Ziel

Eine vollständige, umsetzungsreife und dauerhaft chatunabhängig verständliche **Architecture v0.1** für das Mobile-Spiel **Stammstrecken-Puzzle** liegt im Repository vor. Sie legt die technischen Systemgrenzen und Verträge für Android und iOS fest, ohne Produktionscode zu erzeugen oder bestätigte Produktentscheidungen zu verändern.

## Voraussetzungen

Vor der Bearbeitung sind `AGENTS.md` und die dort vorgeschriebene Lesereihenfolge vollständig abzuarbeiten. Verbindliche Grundlagen sind insbesondere die Projektübergabe, der Konzeptindex, die Master-Spezifikation, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_PACKAGE_RULES.md`, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md` und `DECISIONS/README.md`.

Für die Architekturarbeit sind anschließend die laut Projektübergabe für die Rolle „Technische Architektur-KI“ erforderlichen Fachdateien `03_Raetselkern_und_Interaktionsmodell.md`, `04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md`, `12_UI_und_Bedienungsspezifikation.md`, `13_Oekonomie_und_Monetarisierungs_Balancing.md` und `14_Season_1_Content_Bible.md` vollständig zu lesen. Ergänzend sind die für Fortschritt, Level-Erlebnis und monetarisierte Mobile-Dienste unmittelbar relevanten Fachdateien `05_Level_Erlebnis_und_Qualitaetsdifferenzierung.md`, `06_Fortschritt_Belohnungen_und_Meisterschaft.md` und `08_Faire_Monetarisierung_und_Werbeangebote.md` vollständig zu lesen.

Benötigt werden Lese- und Schreibzugriff auf das Repository sowie Zugriff auf verlässliche Primärquellen für aktuelle Technologie- und Plattformstände. Ausgangspunkt ist der aktuelle Stand von `main`; sämtliche Änderungen erfolgen ausschließlich auf `arch/architecture-v0.1`.

## Scope

Erlaubt sind die folgenden Tätigkeiten:

1. Die technische Architektur v0.1 für Android und iOS vollständig dokumentieren.
2. Game Engine einschließlich konkreter Release-Linie, Programmiersprache, Projekt- und Source-Struktur festlegen.
3. Puzzle-Domain, Solver, Engine-Adapter, Benutzeroberfläche und Mobile-Dienste durch klare Modulgrenzen und erlaubte Abhängigkeiten trennen.
4. Deterministische Zustands-, Befehls-, Validierungs- und Undo-Verträge für Raster, Felder, Werkzeuge, Gleise, Session und Abschluss definieren.
5. Ein maschinenprüfbares, versioniertes Leveldatenformat einschließlich Migration, Validierung, Solver-Nachweis und Authoring-Workflow festlegen.
6. Puzzlevalidierung, Solver, Eindeutigkeitsprüfung und Generatorvalidierung spezifizieren.
7. Savegames, Offline-First-Verhalten, Datenmigration und Wiederherstellung definieren.
8. Plattformabstraktionen für Android und iOS einschließlich Werbung, In-App-Käufe, Kaufwiederherstellung, Analytics, Consent/Datenschutz, Audio und Lifecycle definieren.
9. Assetverwaltung, Lokalisierbarkeit, Logging, Fehlerdiagnose, automatisierte Tests, Build-Konfigurationen, Continuous Integration und Releaseprozess festlegen.
10. Jede wesentliche technische Entscheidung als eigenes angenommenes Architecture Decision Record gemäß `DECISIONS/README.md` dokumentieren.
11. Projektsteuerungsdateien ausschließlich soweit aktualisieren, wie es für Status, Abschluss und Übergabe dieses Work Packages erforderlich ist.
12. Dokumentationsprüfungen und statische Vertragsprüfungen erstellen und ausführen; diese dürfen als reine Prüfskripte außerhalb des künftigen Produktionsprojekts temporär ausgeführt werden, werden jedoch nicht als Produktionscode eingecheckt.

## Betroffene Dateien/Module

| Pfad | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md` | Neu anlegen; Auftrag, Status, Ergebnis und Nachweise dokumentieren. |
| `ARCHITECTURE/ARCHITECTURE.md` | Neu anlegen; Gesamtarchitektur und Navigationsdokument. |
| `ARCHITECTURE/TECH_STACK.md` | Neu anlegen; Technologie-, Versions- und Paketstrategie. |
| `ARCHITECTURE/MODULE_BOUNDARIES.md` | Neu anlegen; Module, Ports, Abhängigkeiten und Source-Struktur. |
| `ARCHITECTURE/GAME_STATE_MODEL.md` | Neu anlegen; persistenter und flüchtiger Spielzustand sowie Befehle. |
| `ARCHITECTURE/LEVEL_DATA_FORMAT.md` | Neu anlegen; kanonisches Leveldatenformat, Schema und Migration. |
| `ARCHITECTURE/schemas/level-v1.schema.json` | Bei technischer Notwendigkeit neu anlegen; maschinenprüfbarer Levelvertrag. |
| `ARCHITECTURE/examples/level-v1.example.json` | Bei technischer Notwendigkeit neu anlegen; gültiges minimales Vertragsbeispiel. |
| `ARCHITECTURE/PUZZLE_ENGINE.md` | Neu anlegen; deterministische Domainlogik und Validierung. |
| `ARCHITECTURE/SOLVER_ARCHITECTURE.md` | Neu anlegen; Solver, Eindeutigkeit und Generatorprüfung. |
| `ARCHITECTURE/PERSISTENCE.md` | Neu anlegen; Savegames, Offline-First und Migration. |
| `ARCHITECTURE/MOBILE_SERVICES.md` | Neu anlegen; Plattform- und Dienstadapter. |
| `ARCHITECTURE/TEST_STRATEGY.md` | Neu anlegen; Testpyramide, Verträge und Qualitätsgates. |
| `ARCHITECTURE/BUILD_AND_RELEASE.md` | Neu anlegen; Buildvarianten, CI, Signing und Store-Release. |
| `ARCHITECTURE/OBSERVABILITY.md` | Nur bei technischer Notwendigkeit neu anlegen; Logging, Analytics und Fehlerdiagnose. |
| `ARCHITECTURE/CONTENT_PIPELINE.md` | Nur bei technischer Notwendigkeit neu anlegen; Levelauthoring und Asset-/Lokalisierungspipeline. |
| `ARCHITECTURE/OPEN_BLOCKERS.md` | Bei technischer Notwendigkeit neu anlegen; fehlende Produktentscheidungen und fail-closed Unblock-Bedingungen. |
| `DECISIONS/ADR-001-*.md` bis `DECISIONS/ADR-012-*.md` | Neu anlegen; einzelne wesentliche Entscheidungen einschließlich Plattformbaselines. Die endgültige Zahl darf nur steigen, wenn eine weitere wesentliche Entscheidung technisch notwendig ist. |
| `PROJECT_CONTROL/CURRENT_STATE.md` | Ist-Stand, nächster Schritt und Blocker aktualisieren. |
| `PROJECT_CONTROL/WORK_QUEUE.md` | Status des bereits bestätigten Produktionsblocks an das angelegte und abgeschlossene Work Package anpassen. |

Die bestehenden Produktdateien unter `Stammstrecken_Puzzle_Konzept_00-15/`, `AGENTS.md`, die Regeln unter `PROJECT_CONTROL/` und `DECISIONS/README.md` werden ausschließlich gelesen und nicht verändert.

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Projektdateien, ausführbare Spielimplementierungen, vollständige Implementierungs-Roadmaps, 240 konkrete Rätselinstanzen, finale Zeitwerte, UI-Assets, Audio-Assets oder Änderungen an bestehenden Produktentscheidungen. `main` darf weder verändert noch gemergt werden. Das Work Package erlaubt keinen automatischen Merge eines Pull Requests.

Offene Produktentscheidungen, insbesondere Werbefrei-Preis, finale Wort-/Bildmarke, konkrete Zeitziele, finaler Zugkatalog sowie noch unbestimmte UI-Restdetails, dürfen nicht technisch vorentschieden werden. Eine technische Kollision oder fehlende notwendige Produktentscheidung wird als **BLOCKER** dokumentiert.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| AK-01 | Alle im Auftrag mindestens geforderten Architekturdokumente existieren, sind gegenseitig konsistent und aus `ARCHITECTURE/ARCHITECTURE.md` auffindbar. |
| AK-02 | Engine, konkrete Release-Linie, Sprache, Source-Struktur, Modulgrenzen und erlaubte Abhängigkeiten sind eindeutig festgelegt. |
| AK-03 | Puzzle-Domain und Solver sind deterministisch, engine-unabhängig und durch kleine, dokumentierte Schnittstellen von UI und Mobile-Diensten getrennt. |
| AK-04 | Feld-, Werkzeug-, Auswahl-, Gleis-, Session-, Undo- und Abschlusszustände entsprechen exakt den bestätigten Produktregeln. |
| AK-05 | Das Leveldatenformat ist versioniert, maschinenprüfbar, migrierbar und enthält die für A/B, Randzahlen, Lösung, Solver-Nachweis, Content-Metadaten und spätere Zeitkalibrierung erforderlichen Verträge, ohne konkrete Season-1-Level zu produzieren. |
| AK-06 | Puzzlevalidierung, Solver, Eindeutigkeitsprüfung, Generatorvalidierung und Levelauthoring sind implementierungsreif beschrieben. |
| AK-07 | Savegames, Offline-First, atomare Speicherung, Migration, IAP-Entitlements und Kaufwiederherstellung sind einschließlich Fehlerfällen beschrieben. |
| AK-08 | Android-/iOS-Abstraktionen für Werbung, IAP, Analytics, Consent/Datenschutz, Audio und Lifecycle sind mit sicheren Ausfallzuständen festgelegt. |
| AK-09 | Assetverwaltung, Lokalisierung, Buildvarianten, CI/Release, Logging und Fehlerdiagnose sind implementierungsreif beschrieben. |
| AK-10 | Eine automatisierbare Teststrategie deckt Domain, Solver, Datenverträge, Persistenzmigrationen, Plattformadapter, Builds und Releases ab. |
| AK-11 | Mindestens die Entscheidungen zu Engine, Sprache, Domain-/Engine-Trennung, Leveldatenformat, Persistenz, Solver, Mobile-Service-Abstraktion und Teststrategie liegen als einzelne angenommene ADRs vor; weitere wesentliche Entscheidungen sind ebenfalls per ADR dokumentiert. |
| AK-12 | Alle bestätigten Produktentscheidungen bleiben unverändert; echte Konflikte oder fehlende Produktentscheidungen sind als BLOCKER ausgewiesen. |
| AK-13 | `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` und dieses Work Package dokumentieren den tatsächlichen Abschluss- und Übergabestand. |
| AK-14 | Der vollständige Git-Diff enthält ausschließlich die in diesem Work Package erlaubten Dokumentations- und Vertragsartefakte; es wurde kein Produktionscode erzeugt. |

## Tests

Vor Abschluss sind mindestens folgende Prüfungen auszuführen und mit Ergebnis zu dokumentieren:

1. **Dateiinventar:** Existenz aller Pflichtdokumente und ADRs prüfen.
2. **Strukturprüfung:** Pflichtabschnitte dieses Work Packages und aller ADRs prüfen.
3. **Linkprüfung:** Alle relativen Markdown-Verweise in neu angelegten oder geänderten Dateien auf existierende Repository-Pfade prüfen.
4. **Vertragsprüfung:** Das Levelbeispiel gegen das Level-JSON-Schema validieren.
5. **Konsistenzprüfung:** Begriffe, Modulnamen, Versionsangaben, Abhängigkeitsregeln, Datenversionen und Statusangaben dokumentübergreifend prüfen.
6. **Anforderungsmatrix:** Jeden ausdrücklich geforderten Architekturpunkt mindestens einem maßgeblichen Abschnitt und gegebenenfalls einem ADR zuordnen.
7. **Produktregelprüfung:** Die Architektur gegen die Pflichtfachdateien sowie die Guardrails der Projektübergabe und Master-Spezifikation prüfen.
8. **Diffprüfung:** Sicherstellen, dass keine bestehende Produktdatei und kein Produktionscode verändert oder erzeugt wurde.
9. **Buildnachweis:** Nicht anwendbar, da dieses Work Package ausdrücklich keinen Produktionscode und kein Buildprojekt erzeugen darf. Diese Nichtanwendbarkeit wird nicht als Berechtigung zur Erzeugung eines Buildsystems verstanden.

## Risikoklasse

**Hoch.** Das Work Package trifft grundlegende Entscheidungen zu Architektur, Datenformat, Persistenz, Datenschutzgrenzen, Monetarisierungsintegrationen, Qualitätssicherung und Releasefähigkeit. Fehler würden zahlreiche spätere Produktionsblöcke betreffen. Deshalb sind angenommene ADRs, maschinenprüfbare Verträge, statische Konsistenzprüfungen und eine chatunabhängige Abschlussübergabe zwingend.

## Definition of Done

Das Work Package ist nur abgeschlossen, wenn der gesamte Scope dokumentiert ist, alle Akzeptanzkriterien einzeln geprüft und erfüllt sind, alle vorgeschriebenen Prüfungen erfolgreich waren und ihre Ergebnisse in diesem Dokument festgehalten wurden. Sämtliche wesentlichen Architekturentscheidungen müssen als angenommene ADRs vorliegen. `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` müssen den tatsächlichen Stand wiedergeben.

Da kein Produktionscode zulässig ist, sind Spiel-Build und Laufzeittests für dieses Work Package ausdrücklich nicht anwendbar. Maschinenprüfbare Dokument- und Datenverträge sowie ihre Validierung ersetzen diesen Nachweis nicht, sondern bilden den für diese Projektphase angemessenen Qualitätsnachweis.

Bekannte Einschränkungen oder nicht erfüllte Kriterien verhindern den Abschluss, sofern sie nicht als echter externer **BLOCKER** ausgewiesen sind und die Vollständigkeit der Architecture v0.1 nicht betreffen. Die Übergabe muss allein aus Repository und Dokumentation verständlich sein.

## Ergebnis

Architecture v0.1 liegt als vollständiger Dokument- und Vertragsstand vor. Der Einstieg ist `../ARCHITECTURE/ARCHITECTURE.md`. Die Lieferung umfasst:

| Artefaktgruppe | Ergebnis |
|---|---|
| Architektur | 14 thematische Markdown-Dokumente einschließlich zentralem Blockerregister. |
| Maschinenverträge | JSON Schema Draft 2020-12 und ein schema-, hash- und eindeutigkeitsgeprüftes 4×4-Vertragsfixture. |
| Entscheidungen | Zwölf einzelne angenommene ADRs für Engine, Sprache/Runtime, Modulgrenzen, Leveldaten, State/Commands, Persistenz, Solver, Mobile-Dienste, Tests, Build/Observability, UI/Assets/Localization/Audio und Plattformbaselines. |
| Projektsteuerung | Dieses Work Package, `CURRENT_STATE.md` und `WORK_QUEUE.md` auf den tatsächlichen Abschlussstand fortgeschrieben. |
| Produktionscode | **Keiner.** Kein Unity-Projekt, Spielcode, konkretes Season-1-Level oder Buildsystem wurde erzeugt. |

Die Architektur legt Unity 6.3 LTS `6000.3.23f1`, C# 9, IL2CPP, Android API 26 als Deploymentminimum mit API 36+ als Storeziel sowie iOS 15 mit Xcode 26/iOS-26-SDK+ fest. Alle externen Dienste liegen hinter typisierten Ports. Veröffentlichtes Puzzleverhalten ist pro Level-ID über den unveränderlichen Puzzlehash geschützt.

## Abnahme der Akzeptanzkriterien

| ID | Status | Nachweis |
|---|---|---|
| AK-01 | Erfüllt | Alle Pflichtdokumente und Zusatzverträge existieren und sind im Hauptdokument verlinkt. |
| AK-02 | Erfüllt | `TECH_STACK.md`, ADR-001/002/012 und die normative Allowlist in `MODULE_BOUNDARIES.md`. |
| AK-03 | Erfüllt | Reine Assemblies `STP.Puzzle.Domain`, `STP.Puzzle.Solver` und `STP.Application`; Ports/Adapter compilerwirksam getrennt. |
| AK-04 | Erfüllt | `GAME_STATE_MODEL.md` und `PUZZLE_ENGINE.md` bilden Zell-, Werkzeug-, Auswahl-, Undo-, Timer- und Completionregeln ab. |
| AK-05 | Erfüllt | `LEVEL_DATA_FORMAT.md`, Schema und Fixture definieren Version, Migration, JCS-Hashes und Solverproof. |
| AK-06 | Erfüllt | Solver, Lösungslimit zwei, Eindeutigkeitsproof, Authoring und Generatorprofilvertrag sind implementierungsreif; Veröffentlichung bleibt wegen `BLOCKER-PROD-003` korrekt fail-closed. |
| AK-07 | Erfüllt | Atomarer lokaler Save, Recovery, Migration, Ledger, IAP-Entitlement und Restore sind einschließlich Fehlerfällen spezifiziert. |
| AK-08 | Erfüllt | Android-/iOS-, Ads-, IAP-, Consent-, Analytics-, Crash-, Audio- und Lifecycle-Ports besitzen sichere Fallbacks. |
| AK-09 | Erfüllt | Addressables, Localization, Contentimport, Buildprofile, CI/Release, Logs und Diagnosecodes sind festgelegt. |
| AK-10 | Erfüllt | `TEST_STRATEGY.md` deckt Domain bis Storepreflight und die drei fail-closed Blockergates ab. |
| AK-11 | Erfüllt | Zwölf angenommene ADRs enthalten Status, Kontext, Entscheidung, Alternativen, Konsequenzen und Validierung. |
| AK-12 | Erfüllt | Produktdateien blieben unverändert; drei fehlende Produktentscheidungen sind in `OPEN_BLOCKERS.md` präzise ausgewiesen. |
| AK-13 | Erfüllt | Dieses Work Package, `CURRENT_STATE.md` und `WORK_QUEUE.md` dokumentieren denselben Abschlussstand. |
| AK-14 | Erfüllt | Enddiff enthält ausschließlich zugelassene Dokument-/Vertrags-/Steuerungsdateien und keinen Produktionscode. |

## Ausgeführte Prüfungen

Architecture v0.1 wurde historisch mit einem temporären, nicht eingecheckten Prüfer abgenommen: **PASS, 13 Prüfgruppen**. Dieser nicht reproduzierbare Nachweis ist ausschließlich Historie und für den aktuellen Stand ungültig; `WP-002` ersetzt ihn durch `tools/architecture-validation/validate.py` samt Lock, README, Fixtures und Selbsttests.

| Prüfgruppe | Ergebnis |
|---|---|
| Datei-/ADR-Inventar | PASS – 16 Architektur-/Vertragsartefakte und 12 ADRs. |
| WP-/ADR-Struktur | PASS – Pflichtabschnitte vollständig und geordnet; alle ADRs angenommen. |
| Relative Markdownlinks | PASS – 29 geprüfte Markdown-Dateien ohne gebrochene lokale Ziele. |
| JSON-Syntax und Duplicate Keys | PASS. |
| JSON Schema Draft 2020-12 | PASS – Fixture erfüllt das Schema. |
| RFC-8785-kompatible Hashprojektionen | PASS – Puzzle-, Lösungs- und Proofhash reproduziert. |
| Levelsemantik und Eindeutigkeit | PASS – gültiger Pfad und erschöpfend exakt eine einfache A–B-Lösung. |
| Anforderungsmatrix/Navigation | PASS – alle ausdrücklich geforderten Bereiche abgedeckt und auffindbar. |
| Produktguardrails | PASS – zentrale bestätigte Regeln technisch abgebildet. |
| Diffscope | PASS – nur Dokumentation/Verträge/erforderliche Projektsteuerung, kein Produktionscode. |

Der erste unabhängige Principal-Architecture-Review fand sechs inhaltliche HIGH-Befunde. Sie wurden korrigiert: einheitlicher Unity-Pin, eine normative Modul-Allowlist, sichere und blockierte Hint-Economy, unveränderlicher Published-Puzzle-Hash, versioniertes GeneratorQualityProfile, Tagesgrenzenblocker, eigenes Plattform-ADR und RFC 8785. Der abschließende unabhängige Re-Review bestätigte **keine verbleibenden inhaltlichen CRITICAL-, HIGH-, MEDIUM- oder LOW-Befunde**. Seine einzigen HIGH-Hinweise betrafen die zu diesem Zeitpunkt planmäßig noch ausstehenden Status- und Commit-Schritte; diese sind Bestandteil dieses Abschlusses.

`git diff --cached --check`, vollständige Staging-Diffprüfung und sauberer Branchstatus werden unmittelbar vor beziehungsweise nach dem Abschlusscommit ausgeführt. Ein Spielbuild ist gemäß Scope **nicht anwendbar**, weil weder Produktionscode noch Unity-Projektdateien erzeugt werden durften.

## Verbleibende Blocker

Die folgenden Folgeblocker sind autoritativ in `../ARCHITECTURE/OPEN_BLOCKERS.md` beschrieben und verhindern nicht den Abschluss der technischen Grundlage:

| ID | Blockierter späterer Bereich |
|---|---|
| `BLOCKER-PROD-001` | Finale Hint-Entitlement-/Economy-Implementierung bis zur Präzisierung von „neuem Level“ und regulärem Hilfekontingent. |
| `BLOCKER-PROD-002` | Betriebslage-des-Tages-Anspruch bis zur Produktentscheidung über Kalendertag, Zeitzone, Offline und Uhränderung. |
| `BLOCKER-PROD-003` | Veröffentlichung generierter Dauerbaustellenlevel bis zu einem kalibrierten, produktfreigegebenen GeneratorQualityProfile. |

Alle drei Bereiche sind in Production fail-closed. Sie blockieren das lokale Puzzle-/Solverfundament und nicht betroffene Fortschritts-, Save-, UI- oder Buildarbeiten nicht.

## Übergabe

Der nächste Agent beginnt bei `PROJECT_CONTROL/CURRENT_STATE.md`, liest danach `ARCHITECTURE/ARCHITECTURE.md`, die für sein Modul zuständigen Detaildokumente und alle referenzierten ADRs. Produktionscode darf erst in einem neuen, freigegebenen Work Package entstehen. Änderungen an einer angenommenen Entscheidung erfordern vor Implementierung ein ersetzendes ADR. `main` wurde nicht verändert oder gemergt.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../DECISIONS/README.md "Architekturentscheidungen (ADR-System)"
[6]: ../Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md "Stammstrecken-Puzzle – Projektübergabe und Gesamtstatus"
