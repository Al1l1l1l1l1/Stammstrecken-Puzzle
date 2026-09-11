# Produktionswarteschlange

Diese Warteschlange enthält bestätigte große Produktionsblöcke und den ausdrücklich vorgeschriebenen nächsten Architecture-v1.0-Freigabereview. Sie ist **keine** detaillierte Implementierungsplanung; jeder neue Block benötigt ein eigenes freigegebenes `WP-###`.

| Priorität | Produktionsblock | Status | Dokumentierter Inhalt |
|---:|---|---|---|
| 1 | Technische Produktionsspezifikation und Finalkorrekturen | **Abgeschlossen (`WP-001`, `WP-002`, `WP-003`)** | **Architecture v0.3** mit kompilierbarer Composition Root, constant-space Endless-Watermark, Privacy-Lifecycle, stabiler Puzzle-/Proofidentität, ehrlichem Validator-Scope, production-identischem RC und atomaren Cosmetics-Claims. Drei Folgeblocker bleiben fail-closed. |
| 2 | Unabhängiger Architecture-v1.0-Freigabereview | **Nicht begonnen** | Architecture v0.3 erneut unabhängig gegen Produktquellen, aktuelle ADRs, geschlossene Sol-/Astra-Findings und lokale Beleggrenzen prüfen. v1.0 benötigt ein eigenes freigegebenes Work Package und wird in WP-003 nicht ausgerufen. |
| 3 | Puzzle-Solver und Levelauthoring | Nicht begonnen | Implementierbarer Solver, Eindeutigkeitsprüfung, Generatorvalidierung, Level-/Katalogdaten, Editor-Workflow und automatisierte Tests. |
| 4 | 240 konkrete Rätselinstanzen | Nicht begonnen | Pro Meldung Lösung, Randzahlen, A/B-Positionen, Solver-Nachweis, Einstiegsschluss, Qualitätsnotiz, Zeitklasse und Abschlussinszenierung. |
| 5 | Finale Zeitwerte | Nicht begonnen | Konkrete Zwei- und Drei-Sterne-Grenzen für gebaute und geprüfte Rätsel. |
| 6 | Finale Asset-Bible | Nicht begonnen | Exakte Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Lackierungen, Betriebsobjekte sowie Motion- und Soundregeln. |
| 7 | Vollständige Textbibliothek | Nicht begonnen | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen mit Längenregeln und Tonprüfung. |
| 8 | Finaler Zug- und Objektkatalog | Nicht begonnen | Züge, Betriebsobjekte, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 9 | Werbefrei-Produkt | Nicht begonnen | Plattformkonformer Preis, Produktbeschreibung, clientseitiger Trustvertrag, Kaufwiederherstellung sowie Consent- und Datenschutz-Flows. |
| 10 | Rechtliche Endprüfung | Nicht begonnen | Wortmarke, Icon, Screenshots, Werbemittel, Zugdesigns und Herkunftseindruck durch qualifizierte Rechtsberatung prüfen. |
| 11 | Launchpaket | Nicht begonnen | Store-Metadaten, Screenshots, Trailer, Meme-Clips, Influencer-Material, Community- und Messplan. |

## Offene Produktfolgeblocker

| Thema | Status | Wirkung |
|---|---|---|
| Hint-Entitlement / Hint-Economy (`BLOCKER-PROD-001`) | **Offen, fail-closed** | Blockiert nur Hintcredit-/Rewarded-Hint-Featurepakete. |
| Kalendertag / Zeitzone / Offline-Policy (`BLOCKER-PROD-002`) | **Offen, fail-closed** | Blockiert nur Tagesanspruch-/Daily-Featurepakete. |
| Generator-Qualitätsprofil (`BLOCKER-PROD-003`) | **Offen, fail-closed** | Blockiert Veröffentlichung generierter Dauerbaustellenlevel. |

Diese drei Punkte blockieren weder den Architecture-v1.0-Freigabereview noch eine spätere Freigabe für sich allein, sofern sie offen bleiben und die betroffenen Funktionen deaktiviert sind.

## Bekannte Reihenfolgeabhängigkeit

Architecture v0.3 ist mit WP-003 abgeschlossen. Der nächste zulässige Architekturschritt ist der unabhängige v1.0-Freigabereview in einem neuen Work Package. Kein Produktionsblock beginnt ohne eigenes freigegebenes `WP-###`. Das technische Puzzle-Fundament muss vor konkreten Rätselinstanzen, Zeitkalibrierung oder Veröffentlichung der Dauerbaustelle implementiert und geprüft werden.

## Pflege der Warteschlange

Ein Eintrag wechselt erst dann von „Nicht begonnen“ zu einem anderen Status, wenn ein eindeutig abgegrenztes Work Package dafür angelegt wurde. Neue Produktionsblöcke oder Prioritätsänderungen benötigen eine dokumentierte, ausdrücklich bestätigte Grundlage.

## Projektquellen

| Datei | Relevanz für diese Warteschlange |
|---|---|
| `CURRENT_STATE.md` | Aktuell gültiger Architektur- und Übergabestand. |
| `../ARCHITECTURE/ARCHITECTURE.md` | Architecture v0.3 und nächster Review-Schritt. |
| `../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md` | Acht Astra-Findings und Abschlussnachweise. |
| `../Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md` | Bestätigte große Produktionsblöcke. |
