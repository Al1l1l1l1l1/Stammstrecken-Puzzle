# WP-002 – Architecture-v0.2-Korrekturen

## ID

`WP-002`

**Bearbeitungsstatus:** Abgeschlossen am 2026-09-08 auf Branch `arch/architecture-v0.1`.

## Ziel

Die zwölf Findings des externen Architecture-v0.1-Reviews werden vollständig, widerspruchsfrei und maschinenprüfbar behoben. Das Ergebnis ist **Architecture v0.2** als allein maßgebliche Architekturversion. Die Korrekturrunde erzeugt keinen Produktionscode, verändert keine bestätigte Produktregel und öffnet keinen Pull Request.

Zusätzlich wird das bereits bestehende Work-Package-ID-Schema bereinigt. Die abgeschlossene technische Produktionsspezifikation erhält die regelkonforme stabile ID `WP-001`; dieses Korrekturpaket verwendet `WP-002`. Alle internen Referenzen werden atomar auf die eindeutigen IDs und Dateinamen umgestellt.

## Voraussetzungen

Vor der Bearbeitung sind `AGENTS.md` und die dort vorgeschriebene Lesereihenfolge vollständig abzuarbeiten. Zusätzlich sind alle relevanten Dateien unter `ARCHITECTURE/`, `DECISIONS/`, `PROJECT_CONTROL/` und `WORK_PACKAGES/` vollständig zu lesen.

Ausgangspunkt ist der am 2026-09-08 verifizierte aktuelle Remote-Stand `origin/main` (`3236334a65d0517c543af0366ba7bc4081bdc263`) zusammen mit dem direkt darauf aufbauenden, unveränderten Architecture-v0.1-Commit `3114ac8238138288e779c6727ac0e1018a89153e`. Die Korrekturen erfolgen gemäß Auftrag ausschließlich auf `arch/architecture-v0.1`.

Die bestätigten Produktregeln und die drei offenen Produktblocker in `ARCHITECTURE/OPEN_BLOCKERS.md` bleiben verbindlich. Änderungen an wesentlichen angenommenen Architekturentscheidungen erfolgen nur über neue ersetzende Architecture Decision Records (ADRs); bestehende ADRs werden nicht still inhaltlich umgedeutet.

## Scope

Erlaubt und erforderlich sind ausschließlich folgende Tätigkeiten:

1. Das Work-Package-ID-Schema auf `WP-###` konsolidieren, die zuvor regelwidrig benannte technische Produktionsspezifikation auf `WP-001` umbenennen und sämtliche Referenzen korrigieren.
2. Die Port- und Adapterabhängigkeiten zyklusfrei und compilerwirksam festlegen. Der unzulässige Vertrag `STP.MobileServices.Contracts -> STP.Application` wird entfernt; Ownership und Richtung zwischen Application, Ports und Adaptern werden eindeutig dokumentiert.
3. Den Save-Hashvertrag auf einen konkret benannten Kanonisierungsstandard, exakte Hashprojektion, UTF-8-Bytefolge und Golden-/Cross-Tool-Tests festlegen.
4. Den einmaligen Rewarded-Ad-Claim `POST_CLEAR_PATIENCE` fachlich über Meldung und Placement identifizieren sowie Prüfung, Reservierung und Ledgercommit atomar und crash-/restart-sicher modellieren.
5. IAP-/Restore-/Revoke-Vorgänge als persistente Zustandsmaschine mit zulässigen Übergängen, Retry-/Reconciliation-Policy und widersprüchlichen Storeantworten spezifizieren.
6. Einen versionierten Campaign-, Completion- und Cosmetics-Katalogvertrag mit JSON-Schema, Identitätsregeln, Cross-References und Validierungsreihenfolge ergänzen.
7. Die technische Zulässigkeit eines exakt ein Feld langen A-B-Pfads definieren und den Levelvertrag einschließlich Schema, Semantikvalidator und Tests konsistent anpassen, ohne eine Season-1-Contententscheidung zu treffen.
8. Eine deterministische Endless-Level-Identität aus `generatorVersion`, Seed und Parameterhash festlegen und von Kampagnen-ID-Regeln trennen.
9. Die Privacy-Policy strikt default-off definieren: Analytics und Crashreports bleiben bis zu einer explizit bestätigten Capability deaktiviert; jeder unklare Zustand ist `false`.
10. Die Definition einer Gerätesmoke-„Prüfung“ so präzisieren, dass Emulator/Simulator nicht als bestanden gilt und physische Geräte oder eine ausdrücklich freigegebene physische Device-Farm erforderlich sind.
11. Die Work-Package-Modulregel auf eine fachlich kohärente, einzeln testbare Änderung mit explizit aufgelisteten betroffenen Modulen korrigieren.
12. Ein versioniertes, reproduzierbares Architektur-Validatorpaket mit Setup, Einstiegspunkt, Abhängigkeitslock, Fixture-Selbsttests und eindeutiger README einchecken.
13. Alle Überschriften, Statusangaben, Work-Package-Verweise, Inventarzahlen und Governanceaussagen auf Architecture v0.2 aktualisieren.
14. Alle Korrekturen gegen Schema, Semantik, Links, Governance, Diff-Scope und dokumentübergreifende Widersprüche validieren.

## Betroffene Dateien/Module

| Pfad | Zulässige Änderung |
|---|---|
| Ehemalige nicht regelkonforme Datei der technischen Produktionsspezifikation | Mit Git-Rename in `WP-001_Technische_Produktionsspezifikation.md` überführen; ID und Referenzen korrigieren, historischen Abschlussinhalt sonst bewahren. |
| `WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md` | Regelkonforme Zieldatei der Umbenennung. |
| `WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md` | Dieses Paket; Scope, Abnahme, Tests und Abschlussnachweise dokumentieren. |
| `PROJECT_CONTROL/*.md` | Governance, Status, Warteschlange und Übergabe ausschließlich soweit für v0.2, ID-Konsistenz und Abschluss erforderlich. |
| `DECISIONS/README.md` | ADR-Governance und tatsächlichen ADR-Bestand korrigieren. |
| `DECISIONS/ADR-001-*.md` bis `DECISIONS/ADR-012-*.md` | Nur Status-/Ersetzungsverweise, WP-Referenzen und eindeutige v0.2-Klarstellungen; keine stille wesentliche Entscheidungsänderung. |
| `DECISIONS/ADR-013-*.md` und weitere technisch notwendige ADRs | Neu anlegen, wenn eine wesentliche angenommene Entscheidung ersetzt oder ein neuer langfristiger Vertrag eingeführt wird. |
| `ARCHITECTURE/*.md` | Die zwölf Findings, Versionen, Referenzen, Inventare und gegenseitige Verträge korrigieren. |
| `ARCHITECTURE/schemas/*.json` | Levelschema korrigieren und neue Katalogschemata anlegen. |
| `ARCHITECTURE/examples/*.json` | Bestehendes Fixture anpassen und minimale positive/negative Vertragsfixtures ergänzen. |
| `tools/architecture-validation/**` | Neu anlegen; reproduzierbarer Validator, Lock, README und Selbsttests. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Projektdateien, ausführbare Spielimplementierung, 240 konkrete Season-1-Rätsel, finale Zeitwerte, UI-/Audio-/Markenassets, Store- oder Providerkonfiguration, neue Produktentscheidungen sowie Änderungen an den drei bestätigten offenen Produktblockern. `main` darf nicht verändert oder gemergt werden. Es wird kein Pull Request erstellt.

Die Korrektur des Ein-Zellen-Pfads definiert ausschließlich technische Regelkonsistenz. Sie erklärt ein solches Level weder zum Season-1-Content noch ändert sie bestätigte Raster- oder Kampagnenvorgaben.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| AK-01 | Alle Work Packages verwenden eindeutige IDs im Format `WP-###`; `WP-001` und `WP-002` sind datei- und referenzweit konsistent. |
| AK-02 | Der normative Assemblygraph ist azyklisch; Ports besitzen einen eindeutigen Eigentümer; Application und Adapter referenzieren nur die zulässige providerneutrale Vertragsgrenze. |
| AK-03 | Save-Envelope und Payload-Hash verwenden einen vollständig benannten Kanonisierungs-, Projektions- und Bytevertrag mit unabhängigen Golden-/Cross-Tool-Tests. |
| AK-04 | `POST_CLEAR_PATIENCE` ist pro Meldung und Placement exakt einmal beanspruchbar; Reservierung, Rewardcallback, Ledgercommit, Parallelität, Restart und späte Callbacks sind idempotent definiert. |
| AK-05 | IAP besitzt eine persistente Zustandsmaschine mit zulässigen Übergängen, Retry/Reconciliation, widersprüchlichen Storeantworten und idempotenter Entitlementbehandlung. |
| AK-06 | Campaign-, Completion- und Cosmetics-Kataloge besitzen versionierte JSON-Schemata, Identitätsregeln, Referenzverträge, Beispiele und eine eindeutige Validierungsreihenfolge. |
| AK-07 | Ein-Zellen-A-B-Pfade sind in Domain, Levelschema, Semantikvalidator und Tests konsistent zulässig; das veröffentlichte 4×4-Fixture bleibt hash- und solvervalide. |
| AK-08 | Endless-Level besitzen eine deterministische, reproduzierbare und von Kampagnen-IDs getrennte Identität aus Generatorversion, Seed und Parameterhash. |
| AK-09 | Analytics und Crashreports sind bis zu explizit bestätigten Capabilities strikt deaktiviert; unklare oder fehlende Zustimmung ergibt `false`. |
| AK-10 | Gerätetestnachweise unterscheiden physische Geräte/physische Device-Farm strikt von Emulator/Simulator; Emulator/Simulator allein kann kein Gerätesmoke-Gate erfüllen. |
| AK-11 | Work Packages dürfen vollständige fachlich kohärente, einzeln testbare Änderungen über explizit genannte Module enthalten; beiläufige Refactorings bleiben verboten. |
| AK-12 | Ein eingecheckter Validator ist aus sauberem Checkout reproduzierbar, prüft die Architekturverträge und besteht eigene positive sowie negative Fixture-Selbsttests. |
| AK-13 | Architecture-v0.2-Überschriften, ADR-/WP-Inventare, Statusdateien und Referenzen sind dokumentübergreifend konsistent. |
| AK-14 | Alle drei Produktblocker bleiben unverändert offen und fail-closed; keine Produktentscheidung wurde erfunden. |
| AK-15 | Finaler Diff gegen `origin/main` enthält ausschließlich erlaubte Dokumentations-, Schema-, Fixture-, Governance- und Validatorartefakte; kein Produktionscode und kein PR. |

## Tests

Vor Abschluss sind mindestens folgende Prüfungen auszuführen und im Ergebnisabschnitt dieses Work Packages zu dokumentieren:

1. vollständiges Datei-, ADR-, Work-Package- und Versionsinventar;
2. Prüfung aller Work-Package-IDs, Dateinamen und internen Referenzen;
3. Prüfung des normativen Assemblygraphen auf unbekannte Knoten, verbotene Kanten und Zyklen;
4. Markdown-Linkprüfung aller geänderten und maßgeblichen Dokumente;
5. JSON-Syntax, Duplicate-Key- und JSON-Schema-Draft-2020-12-Prüfung aller Schemata und Beispiele;
6. Levelsemantik einschließlich Ein-Zellen-Positivfall, Endpunkt-/Count-Grenzen, veröffentlichtem 4×4-Fixture und exakt einer Lösung;
7. RFC-8785/JCS-Hashgoldens des Levelvertrags sowie des Save-Payload-Vertrags mit mindestens zwei unabhängigen Implementierungen;
8. positive und negative Contractfixtures für Rewarded-Ad-, IAP-, Katalog-, Endless-ID-, Privacy- und Device-Evidence-Regeln;
9. Selbsttest, dass der Validator jedes gezielt mutierte Finding erkennt;
10. `git diff --check`, Scopeprüfung gegen `origin/main`, Secret-/Produktionscode-Prüfung und Suche nach veralteten aktuellen Architecture-v0.1- beziehungsweise nicht regelkonformen Work-Package-Referenzen;
11. Buildnachweis ist nicht anwendbar, weil weiterhin kein Unity-Produktionsprojekt existiert; der reproduzierbare Validatorlauf ist der angemessene ausführbare Nachweis.

## Risikoklasse

**Hoch.** Die Korrekturen betreffen Governance, Compilergrenzen, persistente Economy-/IAP-Vorgänge, Datenidentität, Privacy, Releaseprovenienz und maschinenprüfbare Architekturverträge. Fehler könnten spätere Implementierungen, Spielerfortschritt, Datenschutz oder Releasefähigkeit beschädigen.

## Definition of Done

Das Work Package ist nur abgeschlossen, wenn alle zwölf Findings umgesetzt, alle Akzeptanzkriterien einzeln belegt und sämtliche vorgeschriebenen Prüfungen erfolgreich ausgeführt sind. `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, dieses Work Package und die Architektur-Navigation müssen denselben tatsächlichen Architecture-v0.2-Stand wiedergeben.

Jede wesentliche Änderung an einer angenommenen Architekturentscheidung benötigt vor Abschluss ein neues angenommenes beziehungsweise ersetzendes ADR mit expliziter Vorgängerreferenz. Alle nicht ersetzten ADR-Aussagen bleiben gültig. Die Übergabe muss ohne Chatwissen vollständig aus dem Repository nachvollziehbar sein.

Der Abschlusscommit verwendet die aussagekräftige Nachricht `docs(architecture): address review findings for v0.2`. Danach wird ausschließlich der Branch `arch/architecture-v0.1` gepusht; weder Pull Request noch Merge sind Teil dieses Work Packages.

## Ergebnis

Architecture v0.2 schließt alle zwölf Findings des unabhängigen Sol-Reviews. Die regelwidrige historische Work-Package-ID wurde ohne Sonderregel auf `WP-001` migriert; dieses Paket ist `WP-002`. Fünf neue angenommene ADRs dokumentieren die wesentlichen Korrekturentscheidungen. Vier frühere ADRs bleiben als ausdrücklich ersetzte Historie erhalten. Produktionscode, Unity-Projektdateien und Produktquellen wurden nicht geändert.

Ein erster unabhängiger Principal-Re-Review fand zusätzliche materielle Lücken in der I-JSON-Gleichwertigkeit, nativen Privacy-Spezifikation, Katalogsemantik und Endless-Retention. Diese wurden vor Abschluss korrigiert. Der finale unabhängige Re-Review stuft den Vor-Commit-Stand als **abnahmefähig** ein und meldet keine neuen CRITICAL-, HIGH- oder MEDIUM-Widersprüche.

### Closing Status der zwölf Reviewfindings

| Finding | Status | Abschlussnachweis |
|---|---|---|
| `ARCH-REV-001` | **CLOSED** | Repositoryrelativer Validator, Lock, README, kanonischer Befehl sowie ausschließlich `WP-###`. |
| `ARCH-REV-002` | **CLOSED** | Autoritativer ADR-Index für v0.2 mit 13 wirksamen und 4 ersetzten ADRs sowie beidseitigem Superseding. |
| `ARCH-REV-003` | **CLOSED** | `STP-SAVE-JCS-1`, striktes I-JSON, Integer-only, Python-/Node-Golden-Crosscheck und negative Duplicate-/Floatfixtures. |
| `ARCH-REV-004` | **CLOSED** | Fachliche Claim-ID, atomare Reservation/Commit, Parallelität, Crash-/Restart-Recovery und Late-Callback-Quarantäne. |
| `ARCH-REV-005` | **CLOSED** | Persistente client-only IAP-Zustandsmaschine, Grant-before-Acknowledge/Finish, Retry, Restore und Revocation. |
| `ARCH-REV-006` | **CLOSED** | Exakte native Firebase-Schalter, Crashreport-Löschung vor Enable, verzögertes IAP samt Pflichtdatenoffenlegung und physische Gerätetests. |
| `ARCH-REV-007` | **CLOSED** | Campaign-/Completion-/Cosmetics-Verträge, Ports, atomarer Kauf, ID-/Order-/Unlockgraph- und Cross-Reference-Prüfungen. |
| `ARCH-REV-008` | **CLOSED** | Eine normative azyklische Assembly-Allowlist; Ports in `STP.Application`; obsolete/spekulative Knoten entfernt. |
| `ARCH-REV-009` | **CLOSED** | Journal-/Bytegrenzen, Checkpoint, Deduplikationswahrheit sowie 20/64/64-beschränkter Endless-Savezustand. |
| `ARCH-REV-010` | **CLOSED** | Deterministische `E1-`-Identität, Ordinalreservation, Resume, Retention, Hashkette und fail-closed Lebensdauer. |
| `ARCH-REV-011` | **CLOSED** | Fachlich kohärente, einzeln testbare Multi-Modul-Work-Packages mit expliziter Modulliste. |
| `ARCH-REV-012` | **CLOSED** | Ein-Zellen-A-B-Verbindung in Schema, Domain, Solversemantik, Fixture und Negativmutation konsistent zulässig. |

### Akzeptanz

| Kriterium | Status |
|---|---|
| `AK-01` bis `AK-14` | **PASS** – einzeln durch die 14 Validator-Prüfgruppen und den finalen unabhängigen Re-Review belegt. |
| `AK-15` | **PASS** – Diff enthält ausschließlich erlaubte Architektur-, ADR-, Governance-, Schema-, Fixture- und Validatortooling-Artefakte; kein Produktionscode, keine Produktquelldatei und kein Pull Request. |

### Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| Frische repositoryexterne Virtualenv aus `requirements.lock.txt` | **PASS** |
| `python tools/architecture-validation/validate.py --self-test` | **PASS – 14 Prüfgruppen** |
| Mutations-Selbsttest `ARCH-REV-001` bis `ARCH-REV-012` | **PASS** |
| Node-Negativfixture Duplicate Key | **PASS – Exit 1** |
| Node-Negativfixture Floattoken `1.0` | **PASS – Exit 1** |
| Save-JCS-Python-/Node-Golden | **PASS – 688 Bytes, SHA-256 `090cd7ae0f9ac180adda57f927c98306f358d0d359c7a56e04519f2ba9e52351`** |
| JSON Schema Draft 2020-12, Levelsemantik, 4×4-/Ein-Zellen-Eindeutigkeit | **PASS** |
| Katalog-ID, Reihenfolge, Parentpräfix, Unlockgraph, Cross-References | **PASS** |
| Normativer Assemblygraph | **PASS – azyklisch** |
| Relative Markdownlinks und Versions-/Inventarkonsistenz | **PASS** |
| `git diff --check`, Scope-, Secret-, Produktcode- und Produktquellenprüfung | **PASS** |
| Zwei unabhängige Korrektur-Re-Reviews | **PASS – final abnahmefähig; keine offenen Befunde ab MEDIUM** |
| Unity-/Storebuild | **Nicht anwendbar** – weiterhin kein Produktionscode oder Unity-Projekt im Scope. |

## Offene Punkte und Übergabe

`BLOCKER-PROD-001` bis `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed. Sie blockieren weder diesen Abschluss noch Architecture v1.0, sondern nur ihre später betroffenen Feature-/Releasepakete. Nächster Schritt ist der ausdrücklich vorgesehene unabhängige Astra-Finalreview. Eine Architecture-v1.0-Freigabe benötigt ein eigenes freigegebenes Work Package.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture"
