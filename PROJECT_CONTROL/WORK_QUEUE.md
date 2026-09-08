# Produktionswarteschlange

Diese Warteschlange enthält die bereits bestätigten großen Produktionsblöcke sowie den ausdrücklich vorgeschriebenen Astra-Finalreview. Sie ist **keine** technische Implementierungsplanung und enthält noch keine detaillierten Folge-Work-Packages.

| Priorität | Produktionsblock | Status | Dokumentierter Inhalt |
|---:|---|---|---|
| 1 | Technische Produktionsspezifikation | **Abgeschlossen (`WP-001`, korrigiert durch `WP-002`)** | Architecture v0.2 mit Zielplattformen, Engine, zyklusfreien Modulen, Daten-/Katalogverträgen, JCS-Persistenz, Mobile-Transaktionen, Privacy Default-Off, Qualitätssicherung und Releasepipeline. Drei Folgeblocker sind fail-closed dokumentiert. |
| 2 | Unabhängiger Astra-Finalreview / Architecture-v1.0-Entscheidung | **Nicht begonnen** | Architecture v0.2 unabhängig gegen Produktquellen, ADRs und zwölf geschlossene Sol-Findings prüfen. v1.0 benötigt ein eigenes freigegebenes Work Package. |
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

Diese drei Punkte blockieren weder den Astra-Finalreview noch eine Architecture-v1.0-Freigabe, sofern sie weiterhin ausdrücklich offen und in den betroffenen Bereichen deaktiviert bleiben.

## Bekannte Reihenfolgeabhängigkeit

Die **Technische Produktionsspezifikation** ist mit Architecture v0.2 abgeschlossen. Der nächste zulässige Schritt ist der unabhängige Astra-Finalreview. Kein Produktionsblock beginnt ohne eigenes freigegebenes `WP-###`. Das technische Puzzle-Fundament muss weiterhin vor konkreten Rätselinstanzen, Zeitkalibrierung oder Veröffentlichung der Dauerbaustelle implementiert und geprüft werden.

## Pflege der Warteschlange

Ein Eintrag wechselt erst dann von „Nicht begonnen“ zu einem anderen Status, wenn ein eindeutig abgegrenztes Work Package dafür angelegt wurde. Neue Produktionsblöcke oder Prioritätsänderungen benötigen eine dokumentierte, ausdrücklich bestätigte Grundlage.

## Projektquellen

| Datei | Relevanz für diese Warteschlange |
|---|---|
| `CURRENT_STATE.md` | Aktuell gültiger Architektur- und Übergabestand. |
| `../ARCHITECTURE/ARCHITECTURE.md` | Architecture v0.2 und nächster Review-Schritt. |
| `../Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md` | Bestätigte große Produktionsblöcke. |
