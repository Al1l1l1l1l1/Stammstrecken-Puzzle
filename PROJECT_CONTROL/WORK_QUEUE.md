# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, einen gemeinsamen Trust-Anchor-Commit, ein eigenes unveränderliches Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | Trust-Anchor-Korrektur (WP-018-Amendment) | **Abgeschlossen** | PR #8 ist integriert; WP-019/WP-020 sind endgültig archivierte, nicht ausführbare Planungen. |
| 1 | WP-021 – Unity-Scaffold-Recovery | **Abgeschlossen und integriert** | Unity-Scaffold freigegeben; formaler Closeout über PR #12. |
| 2 | WP-022 – Puzzle-Kern-Recovery | **Abgeschlossen und integriert** | Finaler PR-HEAD `b257efd98bd14e7f66ffc40916a0b584c3f51fb7` unabhängig PASS; Geschäftsführungsfreigabe; Integration über PR #13, formaler Closeout über PR #14. |
| 3 | WP-023 – Level-v2-Foundation und Hashverträge | **Abgeschlossen und integriert** | Erste unabhängige Abschluss-QC auf `182db13…` = FAIL; vier Befunde innerhalb WP-023 reproduziert und behoben. Erneute unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68` = **PASS**, AK-01 bis AK-09 erfüllt, kein integrationsblockierender Befund. Geschäftsführungsfreigabe erteilt; PR #15 als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072` in `main` integriert. Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`; Manifest bis zur Integration byteunverändert. |
| 4 | Solver-v2-Metriken und Deduktionsspur | **Nächster Produktionsblock; noch nicht ausführbar** | Fachlich auf Basis der vorhandenen WP-016-Planung und ADR-031. Da WP-016 bereits vorab auf `main` liegt, ist sie kein ausführbarer Trust Anchor. Vor Implementierung muss ein neuer ausführbarer Solver-v2-Auftrag gemeinsam mit eigenem unveränderlichem Production-Scope-Manifest auf einer frischen Branch vom aktuellen `main` nach ADR-030 verankert und Phase A erfolgreich geprüft werden. |
| 5 | Proof-v1-Regeneration und Strict-Validation | **Gesperrt bis Solver-v2 integriert** | Fachlich auf Basis der vorhandenen WP-017-Planung; eigener ausführbarer Startanker erst nach Vorgängerintegration. |
| 6 | 240 konkrete Rätselinstanzen | Blockiert durch Proof-v1 | Erst nach vollständiger Level-/Solver-/Proofpipeline. |
| 7 | Finale Zeitwerte | Nicht begonnen | Nach gebauten, getesteten Rätselinstanzen. |
| 8 | Finale Asset-Bible | Nicht begonnen | Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Objekte, Motion und Sound. |
| 9 | Vollständige Textbibliothek | Nicht begonnen | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen. |
| 10 | Finale Zug- und Objektkataloge | Nicht begonnen | Züge, Betriebsobjekte, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 11 | Werbefrei-Produkt | Nicht begonnen | Plattformpreis, Kaufwiederherstellung, Consent- und Datenschutzflows. |
| 12 | Rechtliche Endprüfung | Nicht begonnen | Marken-, Urheber- und Herkunftseindruck durch qualifizierte Rechtsberatung. |
| 13 | Launchpaket | Nicht begonnen | Store-Metadaten, Screenshots, Trailer, Influencer-/Community- und Messplan. |

## Historische Basis

Die Vorgeschichte `WP-001` bis `WP-005` ist abgeschlossen; Architecture v1.0 bleibt die freigegebene Grundlage. Die vorab auf `main` angelegten WP-015/WP-016/WP-017-Dateien sind fachliche Planungsgrundlagen, können aber wegen ADR-030 nicht selbst nachträglich als erstmaliger ausführbarer Work-Package-/Manifest-Anker dienen.

| Historischer Block | Status | Aussage |
|---|---|---|
| CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** | Das zwingende CI-Gate vor Produktionscoding bleibt verbindlich. |

## Integrationsregel für historische Branches

`origin/feat/wp-008-unity-scaffold`, `origin/feat/wp-009-puzzle-kern` und `origin/feat/wp-013-level-v2-pipeline` sind keine direkt integrierbaren Branches. Ebenso sind vorab angelegte Planungs-WPs keine Ersatzanker. Kein Direktmerge, Cherry-Pick, Rebase, Copy, Konfliktauflösung oder Übernahme historischer PASS-Aussagen.

```text
main
  → WP-021 integriert und geschlossen
  → WP-022 integriert und geschlossen
  → WP-023 integriert; finale unabhängige Abschluss-QC PASS, GF-Freigabe erfolgt
  → neuer ausführbarer Solver-v2-Block
    → eigenes WP + eigenes unveränderliches Manifest gemeinsam auf frischer Main-Branch verankern
    → Phase-A-Governance-/Scope-/Trust-Prüfung
    → erst danach separater Implementierungsauftrag
  → eigener ausführbarer Proof-v1-Block
  → erst dann Contentproduktion
```

Jeder Pfeil ist eine harte Voraussetzung. Ein Agent darf keinen späteren Schritt beginnen, um Scope-, CI-, Lizenz-/Runner- oder Vertragsnachweise zu umgehen.

## QC- und Rollenregel

Die Schutzfunktion ist eine **unabhängige Abschlussprüfung auf einem eingefrorenen Stand vor Integration**. Das konkrete ausführende KI-System ist kein Bestandteil der Projektarchitektur und darf zwischen Arbeitsschritten wechseln. Zusätzliche parallele Prüfungen sind nur erforderlich, wenn das konkrete Work Package oder eine ausdrückliche Geschäftsführungsentscheidung sie wegen des Risikos verlangt. Ein Merge ist ausgeschlossen, solange die erforderliche unabhängige Prüfung einen offenen Blocker oder High-Befund meldet.

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
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Abgeschlossener und integrierter Level-v2-/Hash-Block als technische Basis. |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Historische Planungsgrundlage des nächsten Solver-v2-Blocks; nicht selbst ausführbarer Trust Anchor. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche gemeinsame historische WP-/Manifest-Erstverankerung für den neu anzulegenden Solver-v2-Auftrag. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Verbindlicher Zielvertrag und Grenze für Solver-v2 und den späteren Proof-v1-Block. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Integrierter Puzzle-Kern als technische Basis. |
| `DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |
