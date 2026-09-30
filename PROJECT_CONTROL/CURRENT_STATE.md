# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und Architecture v1.0 sind verbindlich. `main` enthält weiterhin keinen freigegebenen Produktionscode. Diese Governance-Branch dokumentiert das Governance-Package WP-014 sowie die Folgeaufträge WP-015 bis WP-017; deren Implementierung hat **nicht** begonnen. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen. `WP-011` (PR-Scope-Checkout) und `WP-010` (Unity-Testassembly-Standort) sind nach `main` integriert. Die nicht integrierten Branches `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben getrennte Vorgänger-/Prototypstände. |
| Letzter abgeschlossener Schritt | Quellenautorität von WP-013 geklärt: ein späterer separater Manus-Chat ist keine Projektquelle. ADR-031 entscheidet `solver-v2`-Metriken, Proofregeneration und Strict-Semantik ausschließlich aus persistenten Projektquellen und der aktuellen Architekturentscheidung. |
| Aktuell zu prüfender Schritt | Diese Governance-Änderung wartet auf unabhängige Astra- und Sol-QC. Bis zu beiden Berichten gibt es keinen Merge und keinen Implementierungsauftrag. |
| Nächster vorgesehener Schritt | Nach fehlerfreier Governance-QC: Branch mit WP-014 mit ADR-031, Quellenklarstellung sowie WP-015 bis WP-017 nach `main` integrieren. Danach WP-008 vollständig abschließen und integrieren, danach WP-009 vollständig abschließen und integrieren, danach WP-015 → WP-016 → WP-017 in dieser Reihenfolge ausführen. |
| Produktionscode | **Nicht freigegeben.** Der historische WP-013-Branch ist ein nicht integrierter Prototyp; er darf nicht direkt gemergt werden. |
| Aktuell gültige Architekturversion | **Architecture v1.0**; ADR-031 ergänzt ADR-007 und ADR-021, ohne deren übrige Geltung aufzuheben. |
| Architekturentscheidungen | 31 ADRs: 22 angenommen und aktuell wirksam, 9 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |
| WP-013-Quellenautorität | Verbindlich: `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md`. Der separate spätere Chat ist kein Entscheidungsnachweis. |
| Scope-Vertrauensanker | Jeder kommende Implementierungsauftrag erhält ein eigenes Scope-Manifest im gemeinsamen Trust-Anchor-Commit. Die bereits historischen Anker von WP-006 bis WP-011 bleiben unverändert. |
| CI-Follow-up | Das Architecture-Validation-Gate ist eingerichtet. Für künftige Produktions-WPs sind Unity Compile/EditMode, der gültige Scope-Lauf und die vorgesehenen unabhängigen QC-Berichte Pflicht. |

## Verbindliche Grundlage

Die Projektquellen und ihre Hierarchie stehen in [`AGENTS.md`](../AGENTS.md). ADR-031 ist die alleinige ergänzende Architekturentscheidung für Solvermetriken und Proofregeneration. Die ursprünglichen Produktentscheidungen bleiben unverändert: Schwierigkeit entsteht aus erklärbaren Schlussketten; normale Kampagnenlevel verlangen kein blindes Raten.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance-PR:** ADR-031, Quellenklarstellung, aktualisierte Steuerungsdokumente und WP-014 bis WP-016 nur nach unabhängiger Astra-/Sol-QC nach `main` integrieren.
2. **Bestehende Vorgänger:** WP-008 auf seinem bestehenden Scope abschließen und integrieren; danach WP-009 auf seinem bestehenden Scope abschließen und integrieren.
3. **Kein Direktmerge von WP-013:** Der Branch ist wegen fehlender Main-Basis und Vertragsbrüchen archivierter Vergleichskorpus.
4. **Neue Folgearbeit:** WP-015, danach WP-016, danach WP-017 ausführen. Jeder Schritt benötigt eigene Branch, Trust Anchor, CI-Evidenz und unabhängige QC.
5. **Erst danach:** Konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke planen oder beginnen.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Blocker bleiben unverändert. Sie werden durch ADR-031, WP-014 sowie WP-015, WP-016 und WP-017 nicht gelöst oder umgangen.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur, ausgeschlossener separater Chat und Wiederanlauf. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindende Solver-/Proofentscheidung. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | WP-014 ist der Governance-Vorläufer; dieser erste Implementierungsauftrag folgt nach WP-008 und WP-009. |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Zweiter neuer Implementierungsauftrag. |
| `../WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` | Dritter neuer Implementierungsauftrag. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Priorisierte Reihenfolge. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext oder ein nicht persistierter Fremdchat ersetzt keinen Repositoryzustand.
