# Produktionswarteschlange

Diese Warteschlange enthält bestätigte große Produktionsblöcke, den abgeschlossenen Architecture-v1.0-Freigabereview und das mit `WP-007` abgeschlossene CI-Gate vor Produktionscoding. Sie ist **keine** detaillierte Implementierungsplanung; jeder neue Block benötigt ein eigenes freigegebenes `WP-###`.

| Priorität | Produktionsblock | Status | Dokumentierter Inhalt |
|---:|---|---|---|
| 1 | Technische Produktionsspezifikation und Abschlusskorrekturen | **Abgeschlossen (`WP-001` bis `WP-005`)** | **Architecture v0.5** mit providerfreiem Endless-Skip, bindender Cosmetics-Reservation, vollständigem Rollout-Evidenzreducer und gemeinsamem historischem WP-/Manifest-Trust-Anchor. Drei Produktfolgeblocker bleiben fail-closed. |
| 2 | Unabhängiger Architecture-v1.0-Freigabereview und Promotion | **Abgeschlossen (`WP-006`)** | Unabhängiger Abschlussreview von Architecture v0.5 bestanden: alle vier HIGH-Findings CLOSED, 0 neue BLOCKER, 0 neue HIGH, relevante Acceptance-/Validator-Tests PASS. **Architecture v1.0** ist der freigegebene Architekturstand; die Promotion war rein formal ohne technische Architekturänderung. |
| 3 | CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** | GitHub-Actions-Workflow `.github/workflows/validate.yml` mit gepinnter Laufzeitumgebung (Ubuntu 24.04, CPython 3.11.13, Node.js 22.20.0, SHA-gepinnte Actions, `contents: read`), kanonischem Architekturvalidator einschließlich Self-/Negativtests, fail-closed Verhalten, positivem und negativem CI-Nachweis und dem eindeutigen Check `Architecture Validation / validate` für Pull Requests gegen `main` und Branch-Pushes. Kein Merge nach `main`; der Branch `chore/ci-setup` bleibt für eine separat zu entscheidende Übernahme bestehen. |
| 4 | Puzzle-Solver und Levelauthoring | Nicht begonnen; CI-Gate durch `WP-007` erfüllt | Implementierbarer Solver, Eindeutigkeitsprüfung, Generatorvalidierung, Level-/Katalogdaten, Editor-Workflow und automatisierte Tests. |
| 5 | 240 konkrete Rätselinstanzen | Nicht begonnen | Pro Meldung Lösung, Randzahlen, A/B-Positionen, Solver-Nachweis, Einstiegsschluss, Qualitätsnotiz, Zeitklasse und Abschlussinszenierung. |
| 6 | Finale Zeitwerte | Nicht begonnen | Konkrete Zwei- und Drei-Sterne-Grenzen für gebaute und geprüfte Rätsel. |
| 7 | Finale Asset-Bible | Nicht begonnen | Exakte Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Lackierungen, Betriebsobjekte sowie Motion- und Soundregeln. |
| 8 | Vollständige Textbibliothek | Nicht begonnen | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen mit Längenregeln und Tonprüfung. |
| 9 | Finaler Zug- und Objektkatalog | Nicht begonnen | Züge, Betriebsobjekte, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 10 | Werbefrei-Produkt | Nicht begonnen | Plattformkonformer Preis, Produktbeschreibung, clientseitiger Trustvertrag, Kaufwiederherstellung sowie Consent- und Datenschutz-Flows. |
| 11 | Rechtliche Endprüfung | Nicht begonnen | Wortmarke, Icon, Screenshots, Werbemittel, Zugdesigns und Herkunftseindruck durch qualifizierte Rechtsberatung prüfen. |
| 12 | Launchpaket | Nicht begonnen | Store-Metadaten, Screenshots, Trailer, Meme-Clips, Influencer-Material, Community- und Messplan. |

## Offene Produktfolgeblocker

| Thema | Status | Wirkung |
|---|---|---|
| Hint-Entitlement / Hint-Economy (`BLOCKER-PROD-001`) | **Offen, fail-closed** | Blockiert nur Hintcredit-/Rewarded-Hint-Featurepakete. |
| Kalendertag / Zeitzone / Offline-Policy (`BLOCKER-PROD-002`) | **Offen, fail-closed** | Blockiert nur Tagesanspruch-/Daily-Featurepakete. |
| Generator-Qualitätsprofil (`BLOCKER-PROD-003`) | **Offen, fail-closed** | Blockiert Veröffentlichung generierter Dauerbaustellenlevel. |

Diese drei Punkte blockierten weder den Architecture-v1.0-Freigabereview noch die v1.0-Freigabe, weil sie offen bleiben und die betroffenen Funktionen deaktiviert sind.

## Bekannte Reihenfolgeabhängigkeit

Architecture v1.0 ist mit `WP-001` bis `WP-006` freigegeben. **Das separate CI-Setup-Work-Package ist mit `WP-007` vollständig abgeschlossen; das zwingende CI-Gate vor Produktionscoding ist damit erfüllt.** Kein Produktionsblock beginnt ohne eigenes freigegebenes `WP-###`. Jedes künftige Work Package verankert sein eigenes Scope-Manifest und setzt `STP_SCOPE_MANIFEST` in `.github/workflows/validate.yml` auf dieses Manifest. Das technische Puzzle-Fundament muss vor konkreten Rätselinstanzen, Zeitkalibrierung oder Veröffentlichung der Dauerbaustelle implementiert und geprüft werden.

## Pflege der Warteschlange

Ein Eintrag wechselt erst dann von „Nicht begonnen" zu einem anderen Status, wenn ein eindeutig abgegrenztes Work Package dafür angelegt wurde. Neue Produktionsblöcke oder Prioritätsänderungen benötigen eine dokumentierte, ausdrücklich bestätigte Grundlage.

## Projektquellen

| Datei | Relevanz für diese Warteschlange |
|---|---|
| `CURRENT_STATE.md` | Aktuell gültiger Architektur-, CI-Gate- und Übergabestand. |
| `../ARCHITECTURE/ARCHITECTURE.md` | Architecture v1.0 als freigegebener Architekturstand. |
| `../WORK_PACKAGES/WP-007_CI-Setup.md` | CI-Setup, Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` | Formale v1.0-Promotion, Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` | Vier HIGH-Korrekturen, WP-005-Trust-Anchor und Abschlussnachweise. |
| `../Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md` | Bestätigte große Produktionsblöcke. |
