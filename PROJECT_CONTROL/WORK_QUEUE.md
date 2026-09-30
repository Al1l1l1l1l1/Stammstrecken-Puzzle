# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, einen gemeinsamen Trust-Anchor-Commit, ein eigenes unveränderliches Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | WP-018 – QC-Korrektur und Produktionsbasis-Reintegration | **Abgeschlossen** | Beide unabhängigen QC-Berichte PASS ohne BLOCKER/HIGH; PR #6 ist als Merge-Commit `7b0d65a…` nach `main` integriert. |
| 1 | WP-019 – Unity-Scaffold-Recovery | Freigegeben, nicht begonnen | Nächster Auftrag auf frischer Main-Branch, eigener Production-Anchor und reale Unity-/CI-Evidenz. Start/Abschluss nur bei verfügbarer Unity-Lizenz und Linux/Android-/macOS/iOS-Runnern. Historisches WP-008 bleibt nur Vergleich. |
| 2 | WP-020 – Puzzle-Kern-Recovery | Definiert, nicht begonnen | Nach integriertem WP-019: Domain und `solver-v1` auf frischer Main-Branch, eigener Production-Anchor. Historisches WP-009 bleibt nur Vergleich. |
| 3 | WP-015 – Level-v2-Foundation und Hashverträge | Definiert, nicht begonnen | Nach WP-020: Parser, Levelstruktur, Semantik und Hashbasis ohne Proofruntime. |
| 4 | WP-016 – Solver-v2-Metriken und Deduktionsspur | Definiert, nicht begonnen | Nach WP-015 sowie WP-014/WP-018/WP-019/WP-020 auf `main`: Root-Logikmetriken, auditable Spur und deterministische Suchpfadtiefe gemäß ADR-031. |
| 5 | WP-017 – Proof-v1-Regeneration und Strict-Validation | Definiert, nicht begonnen | Nach WP-016 sowie allen Vorgängern auf `main`: `minimum: 0`-Schema, schema-valider `solver-v2`-Proof, bytegenaue Regeneration und Strict-Gate. |
| 6 | 240 konkrete Rätselinstanzen | Blockiert durch WP-017 | Erst nach vollständiger Level-/Solver-/Proofpipeline. |
| 7 | Finale Zeitwerte | Nicht begonnen | Nach gebauten, getesteten Rätselinstanzen. |
| 8 | Finale Asset-Bible | Nicht begonnen | Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Objekte, Motion und Sound. |
| 9 | Vollständige Textbibliothek | Nicht begonnen | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen. |
| 10 | Finale Zug- und Objektkataloge | Nicht begonnen | Züge, Betriebsobjekte, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 11 | Werbefrei-Produkt | Nicht begonnen | Plattformpreis, Kaufwiederherstellung, Consent- und Datenschutzflows. |
| 12 | Rechtliche Endprüfung | Nicht begonnen | Marken-, Urheber- und Herkunftseindruck durch qualifizierte Rechtsberatung. |
| 13 | Launchpaket | Nicht begonnen | Store-Metadaten, Screenshots, Trailer, Influencer-/Community- und Messplan. |

## Historische Basis

Die Vorgeschichte `WP-001` bis `WP-005` ist abgeschlossen; Architecture v1.0 bleibt die freigegebene Grundlage.

| Historischer Block | Status | Aussage |
|---|---|---|
| CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** | Das zwingende CI-Gate vor Produktionscoding bleibt verbindlich und wird durch WP-014/WP-018 nicht eingeschränkt. |

## Integrationsregel für historische Branches

`origin/feat/wp-008-unity-scaffold`, `origin/feat/wp-009-puzzle-kern` und `origin/feat/wp-013-level-v2-pipeline` sind **keine** direkt integrierbaren Branches. Ihre Basen sind nicht Vorfahren des aktuellen Governance-/Main-Stands; ein read-only `merge-tree` belegt Konflikte in Workflow- und Steuerungsdateien. Die historischen Scope-Manifeste bleiben unverändert und können keine Konfliktauflösung gegen den aktuellen `main` legitimieren.

Sie bleiben nur lesbare Vergleichskorpora. Insbesondere sind Direktmerge, Cherry-Pick, Rebase, Konfliktauflösung auf historischen Branches und Übernahme historischer CI-PASS-Aussagen verboten.

```text
main
  → WP-014 + WP-018 Governance integriert (PR #6)
  → WP-019 auf frischer Main-Branch abschließen und integrieren
  → WP-020 auf frischer Main-Branch abschließen und integrieren
  → WP-015
  → WP-016
  → WP-017
  → erst dann Contentproduktion
```

Jeder Pfeil ist eine harte Voraussetzung. Ein Agent darf keinen späteren Schritt beginnen, um Scope-, CI-, Lizenz-/Runner- oder Vertragsnachweise zu umgehen.

## QC- und Rollenregel

| Rolle | Zulässige Verantwortung |
|---|---|
| Geschäftsführung / Projektarchitekt | ADRs, Work-Package-Grenzen, Reihenfolge und Freigabe der nächsten Phase. |
| Kimi / Implementierung | Ausschließlich Umsetzung eines vollständig definierten, freigegebenen Work Packages; keine Architektur- oder Scopeentscheidung. |
| Astra | Unabhängige Architektur-/Vertrags-QC auf eingefrorenem PR-Head. |
| Sol | Unabhängige Test-/Scope-/CI-/Integrations-QC auf eingefrorenem PR-Head. |

Astra und Sol erhalten keine gegenseitigen Ergebnisse vor Abgabe des eigenen Reviews. Ein Merge ist ausgeschlossen, solange einer der beiden einen offenen Blocker oder High-Befund meldet.

## Offene Produktfolgeblocker

| Thema | Status | Wirkung |
|---|---|---|
| Hint-Entitlement / Hint-Economy (`BLOCKER-PROD-001`) | Offen, fail-closed | Blockiert Hintcredit-/Rewarded-Hint-Featurepakete. |
| Kalendertag / Zeitzone / Offline-Policy (`BLOCKER-PROD-002`) | Offen, fail-closed | Blockiert Tagesanspruch-/Daily-Featurepakete. |
| Generator-Qualitätsprofil (`BLOCKER-PROD-003`) | Offen, fail-closed | Blockiert Veröffentlichung generierter Dauerbaustellenlevel. |

Diese Blocker bleiben unverändert und werden nicht durch Recovery-, Solver- oder Proofarbeit umgangen.

## Pflichtquellen

| Datei | Relevanz |
|---|---|
| `CURRENT_STATE.md` | Autoritativer Phasen- und Integrationsstand. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur und Abgrenzung der historischen Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und Zeitpunkt der Schemaumsetzung. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossene QC-Korrektur und Recovery-Freigabe. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Erste Produktionsrecovery. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Zweite Produktionsrecovery. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Level-/Hashfolgeauftrag. |
| `../PROJECT_CONTROL/DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |
