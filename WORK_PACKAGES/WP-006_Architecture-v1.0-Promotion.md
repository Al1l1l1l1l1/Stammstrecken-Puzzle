# WP-006 – Architecture-v1.0-Promotion

## ID

`WP-006`

**Bearbeitungsstatus:** Abgeschlossen auf Branch `arch/architecture-v0.1`.

## Ziel

Die bereits unabhängig geprüfte und freigegebene **Architecture v0.5** wird rein formal und administrativ zu **Architecture v1.0** promoviert. Der unabhängige Abschlussreview (durchgeführt mit K3 Max) ist bereits bestanden: `HIGH-1` bis `HIGH-4` CLOSED, 0 neue BLOCKER, 0 neue HIGH, relevante Acceptance-/Validator-Tests PASS, Freigabeempfehlung JA.

WP-006 ist **keine** Architekturarbeit und **keine** technische Überarbeitung. Es dokumentiert ausschließlich den Freigabestand konsistent in den Architektur- und Projektsteuerungsdokumenten und führt den versionierten Validator mechanisch auf die promovierte Versionsnummer nach. WP-006 verändert keine Architektursemantik, keine Produktentscheidung, keinen ADR-Entscheidungskörper, keinen Produktionscode und keinen Produktfolgeblocker.

## Voraussetzungen

Vor Beginn gelten die vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v0.5, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md` und der kanonische Architekturvalidator.

Ausgangspunkt ist der unabhängig freigegebene Commit `79f64d7191672175ede1153c9458be2207ce3c62` auf lokalem und entferntem Branch `arch/architecture-v0.1`. Der freigegebene Stand muss Bestandteil des Branches sein und der Working Tree muss zu Beginn sauber sein; andernfalls ist WP-006 ohne jede Änderung zu stoppen.

Der Documentation-Scope ist vor allen Promotionsänderungen im kanonischen Manifest [`tools/architecture-validation/scopes/WP-006.documentation.scope.json`](../tools/architecture-validation/scopes/WP-006.documentation.scope.json) verankert. Dieses Work Package und das Manifest werden im selben Trust-Anchor-Commit eingeführt. Der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers (`79f64d7191672175ede1153c9458be2207ce3c62`). Der Validator lädt Work Package und Manifest aus genau diesem historischen Commit und beweist ihre kanonische Bindung.

Der dokumentierte CI-Follow-up bleibt unverändert und ist vor dem ersten produktiven Coding-Work-Package zwingend.

## Scope

Erlaubt sind ausschließlich diese formalen Promotionsänderungen:

1. `ARCHITECTURE/ARCHITECTURE.md`: Versionsidentität von v0.5 auf v1.0 heben; dokumentieren, dass der unabhängige Abschlussreview bestanden ist, alle vier HIGH-Findings CLOSED sind und 0 neue BLOCKER sowie 0 neue HIGH existieren. Keine inhaltliche Architekturänderung.
2. `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`: Architecture v1.0 als freigegebenen Stand, WP-006-Abschluss und den unveränderten CI-Follow-up dokumentieren.
3. `DECISIONS/README.md`: Aktuellen Architekturstand auf v1.0 fortgeschrieben; ADR-Inventar (30 ADRs, 21 angenommen, 9 ersetzt) bleibt unverändert. Es wird **kein** neuer ADR angelegt.
4. `tools/architecture-validation/validate.py` und `tools/architecture-validation/README.md`: Der versionierte Validator (ADR-017, ADR-026) wird mechanisch auf die Versionserwartung v1.0 nachgeführt: erwartete Versionszeichenketten, geprüftes Abschluss-Work-Package, WP-006-Inventar und WP-006-Manifest-Schemabeispiel. Keine Regeländerung, keine Abschwächung, keine neue oder entfernte Prüfung.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` | Neu: Auftrag, Trust-Anchor-Verweis, Status, Acceptance Checks und Abschlussnachweise. |
| `tools/architecture-validation/scopes/WP-006.documentation.scope.json` | Neu: vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunveränderlich. |
| `ARCHITECTURE/ARCHITECTURE.md` | Ausschließlich Versionsidentität v1.0 und formale Review-Feststellung. |
| `DECISIONS/README.md` | Ausschließlich aktueller Architekturstand v1.0 und zugehörige Verweise. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich v1.0-Abschluss, WP-006-Status und unveränderter CI-Follow-up. |
| `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md` | Ausschließlich mechanische Versionserwartung v1.0, WP-006-Inventar und kanonischer WP-006-Befehl. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind technische Architekturänderungen, neue ADRs oder Änderungen an ADR-Entscheidungskörpern, Produktentscheidungen, Produktionscode, Unity-Implementierung, Lösung der drei Produktfolgeblocker, CI-Setup, GitHub-Actions-Workflows, opportunistische Verbesserungen, Refactorings, Konzeptdateiänderungen, Änderungen an `ARCHITECTURE/OPEN_BLOCKERS.md`, Merge nach `main` und Pull Requests.

`BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed. Der CI-Follow-up bleibt zwingende Voraussetzung vor dem ersten produktiven Coding-Work-Package. Erscheint für die Promotion irgendeine technische Änderung über die oben genannte mechanische Versionsnachführung hinaus notwendig, ist WP-006 zu stoppen und der Befund zu berichten.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | `ARCHITECTURE/ARCHITECTURE.md` trägt die Versionsidentität Architecture v1.0 mit Status Angenommen und dokumentiert den bestandenen unabhängigen Abschlussreview. |
| `AK-02` | `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` weisen Architecture v1.0 als freigegebenen Stand und WP-006 als abgeschlossen aus. |
| `AK-03` | Alle vier HIGH-Findings sind dokumentiert CLOSED; es existieren 0 neue BLOCKER und 0 neue HIGH. |
| `AK-04` | `BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed; `ARCHITECTURE/OPEN_BLOCKERS.md` ist unverändert. |
| `AK-05` | Das CI-Setup-Work-Package bleibt dokumentiert zwingende Voraussetzung vor dem ersten produktiven Coding-Work-Package. |
| `AK-06` | Es wurde kein neuer ADR angelegt und kein bestehender ADR-Entscheidungskörper verändert; der ADR-Index zeigt unverändert 30 ADRs (21 angenommen, 9 ersetzt). |
| `AK-07` | Architecture-only- und Documentation-Scope-Validator einschließlich Self-/Negativtests bestehen mit dem WP-006-Manifest. |
| `AK-08` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich die vorab erlaubten Dateien; `git diff --check`, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck bestehen. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Documentation-Scope-Validator mit WP-006-Manifest und `--self-test`;
3. Trust-Anchor-Nachweis: WP-006 und Manifest wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf dieses Manifest, der Manifestblob ist bytegleich;
4. `git diff --check`, Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck;
5. vollständige Delta-Prüfung gegen `79f64d7191672175ede1153c9458be2207ce3c62`;
6. erneuter vollständiger Lauf auf dem konkreten Abschlusscommit vor Push und Remote-Commit-Verifikation nach Push.

Unity Compile/EditMode/PlayMode, IL2CPP, physische Geräte-, SDK- und Storetests bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**. Der GitHub-Actions-Nachweis bleibt ein separates zwingendes CI-Follow-up vor Produktionscoding.

## Risikoklasse

**Mittel.** Die Promotion verändert keine Architektursemantik und keine bestätigte Entscheidung, berührt aber die projektweit sichtbare Versionsdeklaration und die Versionserwartung des Governance-Validators.

## Definition of Done

WP-006 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi und alle Self-/Negativtests grün sind, der vollständige Diff ausschließlich die vorab erlaubten Dateien enthält und der Abschlusscommit auf `arch/architecture-v0.1` gepusht und remote verifiziert wurde.

`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `DECISIONS/README.md`, dieses Work Package und die Architekturnavigation geben denselben Architecture-v1.0-Stand wieder. Es erfolgt kein Merge und keine Änderung an `main`.

## Ergebnis

Architecture v0.5 (freigegebener Commit `79f64d7191672175ede1153c9458be2207ce3c62`) wurde rein formal auf **Architecture v1.0** promoviert. Der unabhängige Abschlussreview (K3 Max) ist bestanden: `HIGH-1` bis `HIGH-4` CLOSED, 0 neue BLOCKER, 0 neue HIGH, relevante Acceptance-/Validator-Tests PASS, Freigabe JA.

| Feststellung | Stand |
|---|---|
| Architecture v1.0 freigegeben | **JA** – `ARCHITECTURE/ARCHITECTURE.md` trägt die v1.0-Identität mit Status Angenommen. |
| Vier HIGH-Findings | CLOSED (unverändert aus v0.5 übernommen, durch den unabhängigen Review bestätigt). |
| Neue BLOCKER / neue HIGH | 0 / 0. |
| `BLOCKER-PROD-001/002/003` | **Unverändert offen und fail-closed**; `ARCHITECTURE/OPEN_BLOCKERS.md` wurde nicht verändert. |
| CI-Setup vor Produktionscoding | **Unverändert zwingend**; in `CURRENT_STATE.md` und `WORK_QUEUE.md` dokumentiert. |
| Technische Architekturänderung | **Keine.** Kein neuer ADR, kein geänderter ADR-Entscheidungskörper, keine Semantikänderung. |
| Produktionscode / Unity / CI-Workflow | **Keiner erzeugt oder verändert.** |
| Merge nach `main` / Pull Request | **Nicht erfolgt.** |

Geänderte Dateien: `ARCHITECTURE/ARCHITECTURE.md`, `DECISIONS/README.md`, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` (neu), `tools/architecture-validation/scopes/WP-006.documentation.scope.json` (neu, Trust-Anchor), `tools/architecture-validation/validate.py` und `tools/architecture-validation/README.md` (ausschließlich mechanische Versions-/WP-006-Nachführung: erwartete Versionszeichenketten v1.0, geprüftes Abschluss-WP, WP-006-Inventar, WP-006-Manifest-Schemabeispiel sowie WP-ID-Nachführung der HIGH-004-Selbsttest-Fixtures auf das jeweils geprüfte Manifest; keine Regeländerung, keine Abschwächung, keine neue oder entfernte Prüfung).

## Validierung

| Ausgeführter Nachweis | Ergebnis |
|---|---|
| Architecture-only mit `--self-test` | **PASS** für alle 17 lokalen Prüfgruppen; einzig verbleibende FAIL-Zeilen sind die zwei unten dokumentierten, plattformbedingten Windows-Artefakte. |
| Documentation-Scope mit WP-006-Manifest und `--self-test` | **PASS** für alle 18 lokalen Prüfgruppen einschließlich `LOCAL_SCOPE` (Trust-Anchor-Nachweis: WP-006 und Manifest gemeinsam in Commit `3e8441830552a2d99c4546fcebc2dc1b23de58cf` hinzugefügt, historischer WP-Blob mit exaktem Manifestlink, Manifestblob bytegleich, reale Diffmenge vollständig innerhalb der Allowlist); einzig verbleibende FAIL-Zeilen sind dieselben zwei Windows-Artefakte. |
| Plattformartefakt-Nachweis | Dieselben zwei FAIL-Zeilen (`adr:016-historical-decision-mutated`, `self-test:not-detected:V03-005-ABSOLUTE`) treten **identisch auf dem unveränderten freigegebenen Basiskommit `79f64d7`** auf (Separat-Worktree-Gegenprobe). Ursache: CRLF-Arbeitskopie gegen LF-Historienblob beziehungsweise Windows-Pfadsemantik von `Path("/tmp/...").is_absolute()`. Sie sind **bestehend und nicht regressiv** und wurden gemäß Auftrag nicht verändert. |
| `git diff --check`, Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheit | **PASS** (im `LOCAL_SCOPE`-Lauf enthalten und zusätzlich manuell geprüft). |
| Delta-Prüfung gegen `79f64d7191672175ede1153c9458be2207ce3c62` | **PASS**; der vollständige Diff enthält ausschließlich die im Manifest erlaubten Dateien. |
| Validator-Umgebung | Gepinnte Lock-Abhängigkeiten aus `requirements.lock.txt`; Ausführung unter Windows mit CPython 3.12 (Projektvertrag nennt Linux/macOS mit CPython 3.11 – plattformbedingte Abweichung, wie bereits im unabhängigen Review festgestellt). |
| GitHub-Actions-Nachweis | **NON-BLOCKING WITH FOLLOW-UP / NOT EXECUTED**; separates zwingendes CI-Setup-Work-Package vor Produktionscoding. |
| Unity-, Geräte-, SDK- und Storetests | **REQUIRED_LATER/NOT_EXECUTED**; WP-006 erzeugte keinen Produktionscode. |

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture"
[6]: ../DECISIONS/README.md "Architekturentscheidungen – verbindlicher ADR-Index"
[7]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005 – Architecture-v0.5-letzte-High-Korrekturen"
