# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, einen gemeinsamen Trust-Anchor-Commit, ein eigenes unveränderliches Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | Trust-Anchor-Korrektur (WP-018-Amendment) | **Abgeschlossen** | PR #8 ist integriert; WP-019/WP-020 sind endgültig archivierte, nicht ausführbare Planungen. |
| 1 | WP-021 – Unity-Scaffold-Recovery | **Abgeschlossen und integriert** | Unabhängige Abschluss-QC **PASS**, Geschäftsführungsfreigabe; vor Integration kein technischer Restblocker. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) am 2026-10-06 als `cd1a048fe7c77f971c00613de5687a0cac1ec395` nach `main` integriert; PR-Head `c93d4c192e55b26ae91db02d7e0c77b9ffd9f54b`. Vier finale Architecture-/Unity-Runs SUCCESS, AK-01–AK-08 und echte Mess-/Buildnachweise im WP-021 dokumentiert; Trust Anchor und Manifest unverändert. |
| 2 | WP-022 – Puzzle-Kern-Recovery | **Vollständig definiert/lokal verankert; Remote-/CI-Gate extern blockiert, Implementierung nicht begonnen** | WP-021 und formaler Closeout PR #12 integriert. Frische Branch `codex/wp-022-puzzle-kern-recovery` vom tatsächlichen Main-HEAD `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`; [WP-022](../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md) und [eigenes Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) gemeinsam erstmals im ersten Add-Commit `ed898894573f2bb3456d21f22a8522c6b8825621` lokal verankert (ADR-030), Manifest immutable. Kein Definitionsblocker. BLOCKER-WP022-REMOTE-001: Git-/Blobupload meldet GitHub-Serverfehler; Remote-Branch steht noch auf Basis 8c0d73a. Lokaler Scope/Trust PASS, vollständige Ubuntu-Diagnose PASS mit abweichenden Pins; gepinnte CI/Definitions-PR ausstehend. Erst Phase-A-Remote-/CI-Abschluss, danach separater Phase-B-Auftrag; keine Folgefreigabe vor Implementierung, unabhängiger QC und Integration. |
| 3 | WP-015 – Level-v2-Foundation und Hashverträge | Definiert, nicht begonnen | Nach WP-022: Parser, Levelstruktur, Semantik und Hashbasis ohne Proofruntime. |
| 4 | WP-016 – Solver-v2-Metriken und Deduktionsspur | Definiert, nicht begonnen | Nach WP-015 sowie WP-014/WP-018/WP-021/WP-022 auf `main`: Root-Logikmetriken, auditable Spur und deterministische Suchpfadtiefe gemäß ADR-031. |
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

`origin/feat/wp-008-unity-scaffold`, `origin/feat/wp-009-puzzle-kern` und `origin/feat/wp-013-level-v2-pipeline` sind **keine** direkt integrierbaren Branches. Ihre Basen sind nicht Vorfahren des aktuellen Governance-/Main-Stands; ein read-only `merge-tree` belegt Konflikte in Workflow- und Steuerungsdateien. Die historischen Scope-Manifeste bleiben unverändert und können keine Konfliktauflösung gegen den aktuellen `main` legitimieren. Zusätzlich ist `origin/chore/ci-readiness-unity` **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness (Lizenzweg, Runner, Toolchains); auch er ist keine Lieferquelle.

Sie bleiben nur lesbare Vergleichskorpora. Insbesondere sind Direktmerge, Cherry-Pick, Rebase, Konfliktauflösung auf historischen Branches und Übernahme historischer CI-PASS-Aussagen verboten.

```text
main
  → Trust-Anchor-Korrektur integriert (PR #8)
  → Hold durch Geschäftsführung aufgehoben; CI-Readiness real nachgewiesen
  → WP-021 + eigenes Manifest gemeinsam auf frischer Main-Branch verankert (fc10c61);
    eigene CI-Nachweise grün; unabhängige Abschluss-QC PASS; Geschäftsführungsfreigabe;
    integriert über PR #11 (cd1a048), formal abgeschlossen
  → WP-021-Closeout PR #12 integriert (8c0d73a)
  → WP-022 + eigenes Manifest gemeinsam auf frischer Main-Branch verankert (ed89889);
    separate Implementierung ausstehend → eigene Evidenz/QC → Integration
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
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte Planungsunterlage, nicht ausführbar. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte Planungsunterlage, nicht ausführbar. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Aktuell vollständig definierter Auftrag; Phase-A-Verankerung und spätere Implementierung getrennt, keine Vorwegnahme WP-015/016/017. |
| `../tools/architecture-validation/scopes/WP-022.production.scope.json` | Eigenes immutable Manifest, gemeinsame historische Erstverankerung ed89889 auf Basis 8c0d73a. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Level-/Hashfolgeauftrag. |
| `../PROJECT_CONTROL/DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |
