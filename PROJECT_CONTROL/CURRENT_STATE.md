# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10. **WP-021 (Unity-Scaffold-Recovery) ist der aktive Produktionsauftrag** und läuft auf `feat/wp-021-unity-scaffold-recovery`. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 ebenfalls. WP-021 aktiv auf `feat/wp-021-unity-scaffold-recovery`, Basis `e4f8cc1` und Trust Anchor `fc10c61` unverändert. Eng begrenzte Coverage-Erzeugungskorrektur am 2026-10-06 auf selbst verifiziertem PR-HEAD `4be3937cb4e329f0738be6c223c3b1d434a3ec27`; Astra-QC-01/QC-02 und QC-CI-01/QC-CI-02 bleiben erhalten. Implementierungsrolle; unabhängige Abschluss-QC und Integration folgen separat. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | Echte lokale Unity-6000.3.23f1-Messung: EditMode **1/1 PASS**, PlayMode **27/27 PASS** inkl. QA-Szenen-Smoke; vollständige Rohpunktdaten für alle acht Pflichtassemblies, tatsächliche Nullwerte im EditMode-Abschlussbericht. Unveränderte Coverage-/PlayMode-Gates PASS. **5 Collector- und 51 Gateregressionen PASS**; Modulgraph 14+9, QA-Identitäten/12 Negativproben, positiver Scope/Trust/Secret/Diff und 31 Shellsyntaxchecks PASS. Beide Selbsttests FAIL ausschließlich bekannter Windows-Befund `V03-005-ABSOLUTE`. Details im WP-021. |
| Aktuell zu prüfender Schritt | Ausgangs-HEAD `4be3937` tatsächlich remote geprüft: Architecture Validation Run `37347568464` SUCCESS; Unity CI Run `37347568546`, Job `111976793747`, scheitert am Coverage-Gate wegen vom Collector ausgelassener unbesuchter Assemblies. Billing blockiert diesen Run nicht mehr. Nur `unity.yml` schaltet den SHA-/versionsgeprüften Code-Coverage-1.3.0-Collector in der ignorierten Paketkopie auf vorhandenen Full-Export um. Keine Datenauffüllung, Gateabsenkung oder Produktions-/Paket-/Validatoränderung. Neue commitgebundene Actions-Ergebnisse nach Push noch zu prüfen. |
| Nächster vorgesehener Schritt | Korrektur committen/pushen, Remote/PR-HEAD verifizieren und neue GitHub-Actions-Läufe vollständig auswerten; tatsächliche Coverage-/Android-/iOS-Nachweise persistieren. Anschließend unabhängige Abschluss-QC auf eingefrorenem HEAD und Integrationsentscheidung. Kein Merge/Close; **WP-022 gesperrt**. |
| Produktionscode | WP-021-Unity-Scaffold auf `feat/wp-021-unity-scaffold-recovery` (Projektbasis `6000.3.23f1`, 14 Produktions- und 9 Testassemblies, 15 Ports, fail-closed `ApplicationComposition`, `BootstrapComposition`, QA-Szene, `BootstrapCompositionSmoke`, `StpBuildEntrypoints`; **keine** Puzzlefachlogik). **Nicht freigegeben** bis zur unabhängigen QC und Integration. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-021 und `tools/architecture-validation/scopes/WP-021.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `fc10c61` eingeführt (Elterncommit = `baseCommit` = `e4f8cc1d0f0d0590fd7508992af464c0230ef314`); danach ist das Manifest byteunveränderlich. WP-014/WP-018 bleiben unverändert. Für WP-022 und alle Folgepakete gilt ohne Ausnahme: Work Package und eigenes Production-Scope-Manifest werden gemeinsam erstmals auf der jeweiligen frischen Implementierungsbranch hinzugefügt. |
| CI-Follow-up | `validate.yml` läuft mit `STP_SCOPE: production` und `STP_SCOPE_MANIFEST: tools/architecture-validation/scopes/WP-021.production.scope.json` (WP-011-PR-Head-Mechanik unverändert). `unity.yml` ist mit WP-021 neu eingerichtet: Preflight mit statischem Modulgraph-Check, fail-closed `unity-config`/`unity-evidence-guard`, drei Umgebungsjobs ohne Secrets sowie die serialisierten Lizenzjobs Compile, EditMode inkl. Coverage, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile; Aktivierung der Unity-Personal-Lizenz per Unity Licensing Client mit `secrets.UNITY_EMAIL`/`secrets.UNITY_PASSWORD`; `vars.UNITY_RUNNERS_READY=true` ist gesetzt. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 (läuft):** Auf der Basis des aktuellen `main` verankert (`fc10c61`), aus dem Vertrag implementiert, mit eigenen CI-Nachweisen belegt; PR gegen `main`; unabhängige Astra-/Sol-QC; Integration erst nach deren PASS und Geschäftsführungsentscheidung.
3. **Puzzle-Kern:** WP-022 wird erst nach integriertem WP-021 auf einer neuen Branch gemeinsam mit eigenem Manifest verankert und umgesetzt.
4. **Content-/Proofkette:** Erst danach folgen WP-015 → WP-016 → WP-017 in harter Reihenfolge.
5. **Contentproduktion:** Erst nach WP-017 dürfen konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke beginnen.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert und werden durch WP-021 nicht berührt.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `../WORK_PACKAGES/WP-021_Unity-Scaffold-Recovery.md` | Aktiver Produktionsauftrag: Unity-Scaffold-Recovery mit Trust Anchor, Vergleichsmatrix, Scope, Akzeptanzkriterien und allen Abschlussnachweisen. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Auftrag mit nachträglicher Trust-Anchor-Korrektur. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext, historische Branches und vorab angelegte Planungsunterlagen ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
