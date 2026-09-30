# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, Trust-Anchor, Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | Governance-QC und Integration ADR-031 | **In Prüfung** | Quellenkorrektur zu WP-013, WP-014, ADR-031 und WP-015 bis WP-017 und Steuerungsdokumente. Erst nach unabhängiger Astra-/Sol-QC nach `main` mergen. |
| 1 | WP-008 – Unity-Scaffold | Nicht integriert | Bestehenden Scope auf `feat/wp-008-unity-scaffold` abschließen, CI-/QC-Nachweise herstellen und separat nach `main` integrieren. |
| 2 | WP-009 – Puzzle-Kern | Nicht integriert | Erst nach WP-008; bestehenden Scope auf `feat/wp-009-puzzle-kern` abschließen, CI-/QC-Nachweise herstellen und separat nach `main` integrieren. |
| 3 | WP-015 – Level-v2-Foundation und Hashverträge | Definiert, nicht begonnen | Nach WP-008 und WP-009: Parser, Levelstruktur, Semantik und Hashbasis ohne Proofruntime. |
| 4 | WP-016 – Solver-v2-Metriken und Deduktionsspur | Definiert, nicht begonnen | Nach WP-014: Root-Logikmetriken, auditable Spur und deterministische Suchpfadtiefe gemäß ADR-031. |
| 5 | WP-017 – Proof-v1-Regeneration und Strict-Validation | Definiert, nicht begonnen | Nach WP-015: schema-valider solver-v2-Proof, bytegenaue Regeneration und einheitliches Strict-Gate. |
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
| CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** | Das zwingende CI-Gate vor Produktionscoding bleibt verbindlich und wird durch WP-014 nicht eingeschränkt. |

## Integrationsregel für historische Branches

`origin/feat/wp-013-level-v2-pipeline` ist **kein** direkt integrierbarer Branch. Er basiert auf `767c01e…`, das kein Vorfahr des aktuellen `main` ist, und enthält nicht gelöste Proofvertragsbrüche. Er darf nur als lesbarer Vergleichskorpus für die explizit freigegebenen Folge-Work-Packages dienen.

Die bestehende Kette bleibt strikt:

```text
main (WP-010 und WP-011 integriert)
  → WP-008 abschließen und integrieren
  → WP-009 abschließen und integrieren
  → WP-015
  → WP-016
  → WP-017
  → erst dann Contentproduktion
```

Jeder Pfeil ist eine harte Voraussetzung. Ein Agent darf keine spätere Branchbasis vorziehen, um sich einen Scope-, CI- oder Vertragsnachweis zu sparen.

## QC- und Rollenregel

| Rolle | Zulässige Verantwortung |
|---|---|
| Geschäftsführung / Projektarchitekt | ADRs, Work-Package-Grenzen, Reihenfolge, Freigabe der nächsten Phase. |
| Kimi / Implementierung | Ausschließlich Umsetzung eines vollständig definierten und freigegebenen Work Packages; keine Architektur- oder Scopeentscheidung. |
| Astra | Unabhängige Architektur-/Vertrags-QC auf eingefrorenem PR-Head. |
| Sol | Unabhängige Test-/Scope-/CI-/Integrations-QC auf eingefrorenem PR-Head. |

Astra und Sol erhalten keine gegenseitigen Ergebnisse vor Abgabe des eigenen Reviews. Ein Merge ist ausgeschlossen, solange einer der beiden einen offenen Blocker oder High-Befund meldet.

## Offene Produktfolgeblocker

| Thema | Status | Wirkung |
|---|---|---|
| Hint-Entitlement / Hint-Economy (`BLOCKER-PROD-001`) | Offen, fail-closed | Blockiert Hintcredit-/Rewarded-Hint-Featurepakete. |
| Kalendertag / Zeitzone / Offline-Policy (`BLOCKER-PROD-002`) | Offen, fail-closed | Blockiert Tagesanspruch-/Daily-Featurepakete. |
| Generator-Qualitätsprofil (`BLOCKER-PROD-003`) | Offen, fail-closed | Blockiert Veröffentlichung generierter Dauerbaustellenlevel. |

Diese Blocker bleiben unverändert und werden nicht durch Proof- oder Solverarbeit umgangen.

## Pflichtquellen

| Datei | Relevanz |
|---|---|
| `CURRENT_STATE.md` | Autoritativer Phasen- und Integrationsstand. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur und Status des historischen Prototyps. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindende Metrik-/Proofentscheidung. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Erster neuer Implementierungsscope. |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Zweiter neuer Implementierungsscope. |
| `../WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` | Dritter neuer Implementierungsscope. |
| `../PROJECT_CONTROL/DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |
