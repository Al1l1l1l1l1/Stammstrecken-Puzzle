# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, einen gemeinsamen Trust-Anchor-Commit, ein eigenes unveränderliches Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | Trust-Anchor-Korrektur (WP-018-Amendment) | **Abgeschlossen** | PR #8 ist integriert; WP-019/WP-020 sind endgültig archivierte, nicht ausführbare Planungen. |
| 1 | WP-021 – Unity-Scaffold-Recovery | **Abgeschlossen und integriert** | Unabhängige Abschluss-QC **PASS**, Geschäftsführungsfreigabe; vor Integration kein technischer Restblocker. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) am 2026-10-06 als `cd1a048fe7c77f971c00613de5687a0cac1ec395` nach `main` integriert; formaler Closeout über PR #12. |
| 2 | WP-022 – Puzzle-Kern-Recovery | **Abgeschlossen und integriert** | Finaler geprüfter PR-HEAD `b257efd98bd14e7f66ffc40916a0b584c3f51fb7`; unabhängige Abschluss-QC **PASS**, Geschäftsführungsfreigabe und Integration über [PR #13](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/13) als Merge-Commit `a85ea224d109ad1713a226481a91f6e24208177d`. AK-01–AK-10 und vollständige Unity-/Governance-/Coverage-/Mutations-/Budget-/Android-/iOS-Nachweise dokumentiert; Anker/Manifest unverändert. |
| 3 | Level-v2-Foundation und Hashverträge | **Nächster Produktionsblock; Startvorbereitung erforderlich** | Fachlich entspricht der nächste Block der vorhandenen WP-015-Planung: Parser, Levelstruktur, Semantik und Hashbasis ohne Proofruntime. Die vorhandene Datei `WP-015_Level-v2-Foundation-und-Hashvertraege.md` liegt jedoch bereits auf `main` und kann deshalb die ADR-030-Forderung einer gemeinsamen erstmaligen WP-/Manifest-Verankerung nicht selbst erfüllen. Vor Implementierung ist ein sauberer ausführbarer Main-basierter Work-Package-/Manifest-Startzustand herzustellen; bis dahin kein Produktionscoding. |
| 4 | WP-016 – Solver-v2-Metriken und Deduktionsspur | Definiert, nicht begonnen | Nach integrierter Level-v2-Foundation sowie WP-014/WP-018/WP-021/WP-022 auf `main`: Root-Logikmetriken, auditable Spur und deterministische Suchpfadtiefe gemäß ADR-031. |
| 5 | WP-017 – Proof-v1-Regeneration und Strict-Validation | Definiert, nicht begonnen | Nach integrierter Solver-v2-Stufe: `minimum: 0`-Schema, schema-valider `solver-v2`-Proof, bytegenaue Regeneration und Strict-Gate. |
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

`origin/feat/wp-008-unity-scaffold`, `origin/feat/wp-009-puzzle-kern` und `origin/feat/wp-013-level-v2-pipeline` sind **keine** direkt integrierbaren Branches. Ihre Basen sind nicht Vorfahren des aktuellen Governance-/Main-Stands; ein read-only `merge-tree` belegt Konflikte in Workflow- und Steuerungsdateien. Die historischen Scope-Manifeste bleiben unverändert und können keine Konfliktauflösung gegen den aktuellen `main` legitimieren. Zusätzlich ist `origin/chore/ci-readiness-unity` **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness (Lizenzweg, Runner, Toolchains); auch er ist keine Lieferquelle.

Sie bleiben nur lesbare Vergleichskorpora. Insbesondere sind Direktmerge, Cherry-Pick, Rebase, Konfliktauflösung auf historischen Branches und Übernahme historischer CI-PASS-Aussagen verboten.

```text
main
  → Trust-Anchor-Korrektur integriert (PR #8)
  → Hold durch Geschäftsführung aufgehoben; CI-Readiness real nachgewiesen
  → WP-021 + eigenes Manifest gemeinsam auf frischer Main-Branch verankert (fc10c61)
    → implementiert → unabhängige QC PASS → integriert → formal abgeschlossen
  → WP-022 + eigenes Manifest gemeinsam auf frischer Main-Branch verankert (ed89889)
    → implementiert → AK-07 korrigiert → unabhängige QC PASS → integriert über PR #13 (a85ea22)
  → Level-v2-/Hash-Foundation: vor Implementierung erst ADR-030-konformen ausführbaren WP-/Manifest-Startzustand herstellen
  → Solver-v2
  → Proof-v1
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
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur und Abgrenzung der historischen Branches. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche gemeinsame historische WP-/Manifest-Erstverankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und Zeitpunkt der Schemaumsetzung. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Abgeschlossener und integrierter Puzzle-Kern mit eigenem Trust Anchor und Abschlussnachweisen. |
| `../tools/architecture-validation/scopes/WP-022.production.scope.json` | Unverändertes WP-022-Production-Scope-Manifest. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Bestehende Planungsfassung des nächsten fachlichen Blocks; wegen Vorabexistenz auf `main` nicht unmittelbar als ADR-030-Anker-WP ausführbar. |
| `../PROJECT_CONTROL/DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |
