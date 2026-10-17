# Übergabe WP-036 – Vollständige Textbibliothek

Stand: 17.10.2026. **Textproduktion vollständig als DRAFT geliefert; konkrete kreative Ownerannahme offen.** Eigene Branch: codex/wp-036-vollstaendige-textbibliothek. Basis main d90e798dd9b26d3f152f3b0c2d1d52cae641eb16, gemeinsamer eigener Anker 213065f7f33be634073e5bcef85db4f848d5b99f, Manifest danach byteunverändert. Kein fremder Zweig integriert und kein Main-Merge.

## Was tatsächlich vorliegt

1.408 deutsche Originaleinträge: 960 Leveltexte für sämtliche 240 wirklichen WP-026-IDs, 85 Kampagnenbegleitungen für fünf Abschnitte und zwanzig Routen einschließlich Seasonfinale, 308 UI-/Zustandstexte und 55 Varianten in elf semantisch getrennten Familien. 82 Kurzfassungen. 240 Einstiege sind INTERNAL; 1.168 PLAYER-Einträge sind vorgesehene Anzeigetexte. Alle 1.408 bleiben DRAFT. 70 zusätzliche Produkt-/Katalog-/Darstellungsabhängigkeiten sind durch BLOCKED_PRODUCT ausdrücklich gesperrt, 1.338 Einträge sind lediglich konzeptionell DECIDED.

Die einzige kanonische Quelle ist [de-DE.json](../../../Localization/source/de-DE.json), ihr eigenes geschlossenes [Authoringschema](../../../Localization/source-v1.schema.json) konkretisiert die vorhandenen Mindestfelder aus CONTENT_PIPELINE §8. Keine Architekturänderung und kein Runtimeformat. [Vollständige Lesefassung](READING_EDITION.md), [Abdeckung](COVERAGE.md), [konkrete Sprachregeln](EDITORIAL_RULES.md), [kritische Eigenprüfung/Stichprobe](EDITORIAL_REVIEW.md), [gesonderte unabhängige QC](QC.md), [Quellenbindungen](source-bindings.json) und tatsächliche JSON-/TXT-Prüfnachweise liegen gemeinsam hier.

## Reproduktion und technische Aussage

Der Textchecker aus tools/text-library/validate.py liest ausschließlich gepinnte Gitobjekte aus dc1652befd7545b4c2a2dd0580f35a4e8e05d72d: 240 Level, zwanzig Leveldesignreviews, individuellen Autorensatz und das ursprüngliche 1.440-Referenzen-Inventar. Er prüft Level-/Review-/Authorbytehashes, alle ursprünglichen Keys und deren Reihenfolge, 240 Autor-/Revieweinstiege, eigene interne Texthashes, öffentliche Fakten und einen elementaren mathematischen Eingangsanker pro Level. Er beweist keine gesamte sprachliche Paraphrase und führt keinen neuen Solver-/Human-Test aus. Die Autoren prüften ihre einzelnen Formulierungen am realen Input; die unabhängige Rolle prüft dies gesondert.

```text
git fetch --no-tags origin dc1652befd7545b4c2a2dd0580f35a4e8e05d72d
python tools/text-library/validate.py --self-test --write-reports
python tools/text-library/render.py --check
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-036.production.scope.json --self-test
git diff --check d90e798dd9b26d3f152f3b0c2d1d52cae641eb16
```

Bestehende gepinnte Architekturabhängigkeiten verwenden, keine neuen Pakete. Kanonisch Linux/macOS, CPython3.11 und Node22; CI verwendet 3.11.13/22.20.0. Lokal bestand der eigentliche Textcheck auf Windows CPython3.11.15. Ein erster Windows-Architekturlauf erkannte die damals noch unformatierte WP-ID und eine plattformabhängige absolute-Pfad-Selbstprobe; der unveröffentlichte Anker wurde ausschließlich im ID-Format korrigiert. Keine Regel oder Manifest-Allowlist wurde für einen PASS verändert. Der erste temporäre Assemblyversuch scheiterte vor Erzeugung der kanonischen Quelle an einem Indexfehler in der nur temporären Zusammenführung; nach Korrektur wurde die gesamte Quelle neu erstellt und vollständig geprüft. Fehlversuche sind keine PASS-Evidenz.

Der neue echte Textlauf besteht sämtliche Schema-/Key-/960-Referenz-/Platzhalter-/NFC-/Zeichen-/Quellenbindungsprüfungen und zwölf absichtliche Mutationen. Die vollständigen tatsächlich ausgeführten Ergebnisse samt Quellhash stehen in technical-validation.json. Pseudolocale und Wiederholungsbericht sind reproduzierbares Prüfmaterial; keine ausgeführten Gerätelayouts. Bestehende echte Produktionskeys sind bedient, fehlende Fixture-/Testkeys bleiben ausdrücklich isoliert.

## Akzeptanzkriterien und Abschlussgrenze

| AK | Tatsächlicher Stand vor gesondertem Abschlussnachtrag |
|---|---|
| AK01 | Vollständige 240er-Reihenfolge, 960/960 Texte, 240 INTERNAL/720 Level-PLAYER erfolgreich geprüft. |
| AK02 | Alle 240 Level-/Author-/Review-/Textbindungen plus öffentliche Fakten/elementarer Anker geprüft; individuelle redaktionelle Abgleiche geliefert, unabhängiges Ergebnis separat. |
| AK03 | Fünf Abschnitts- und zwanzig Routenbögen samt bestehendem Finale vollständig. |
| AK04 | Gesamtes Oberflächeninventar und entschiedene Beschriftungen geliefert; konkrete offene Regeln in den jeweiligen Entwürfen gesperrt. |
| AK05 | Alle neun geforderten Anlässe geliefert, wegen klarer Sachverhaltstrennung elf Familien mit je fünf Varianten. |
| AK06 | Pflichtfelder an jedem Eintrag, vorgeschlagene Limits, erlaubte Typen, Sichtbarkeit und 82 Kurzformen. |
| AK07 | Vollständiger eigener Checker und zwölf Negativproben PASS. Pseudolocale/Inventar erzeugt; kein Runtimeimport. |
| AK08 | Individuelle Autorrollen, Gesamtnacharbeit, Wiederholungsbeurteilung, 16 begründete Stichproben und Abgleich11/12/14 geliefert. Keine unabhängige Freigabe durch Eigenprüfung. |
| AK09 | Eigener Anker/Manifest und korrekte Workflowauswahl vorhanden. Unabhängige Freeze-QC und tatsächliche reguläre CI erst nach ihrem Lauf einzutragen, bis dahin offen. |
| AK10 | Vollständige Repoquellen/Lesefassung/Nachweise, Status und Übergabe vorhanden. Draft-PR/finaler Liefercommit folgen nach tatsächlicher Erstellung. |

## Was weiterhin offen bleibt

- Konkrete kreative Ownerannahme der Texte und Kurzformen. Die bestätigte deutliche Bahn-Anspielung wird nicht abgeschwächt; die Ausführung kann trotzdem redaktionell nachgearbeitet werden.
- Finale UI-/Accessibility-/Layoutprüfung einschließlich expandierter Variablen, dynamischer Schrift und Screenreader. Zeichenlimits sind ausschließlich redaktionelle Vorschläge.
- BLOCKER-PROD-001 Hint-Entitlement/-Economy und geprüfte Hinweisdarstellung; BLOCKER-PROD-002 Kalendertag/Zeitzone/Offline-/Manipulationspolicy; BLOCKER-PROD-003 Generatorqualitätsprofil.
- Finaler Zug-/Objektkatalog mit konkreten Namen, Beschreibungen, Preisen und Freischaltungen. Dieses WP liefert bestehende Kategorien und parametrisierte Zustandsbeschriftungen, keine erfundenen finalen Items.
- Meisterschaftsdetaildarstellung, kosmetischer Tagesfortschritt, Store-/Werbefrei-Produkt/Preis, Consent-/SDK-/Rechtstexte nach zuständiger Freigabe. Keine zusätzlichen Rechte oder Monetarisierungsregeln gesetzt.
- WP-026 bleibt insgesamt offen: menschlicher Spieltest, finale Zeitkalibrierung, kreative-/Produkt-/Import-/Releasegates. Dessen 480 Karten-/Zugziele werden durch keine Textquelle erledigt. Die 960 vorhandenen DRAFT-Texte sind weder ein ganzer Cross-Reference-PASS noch ein Release-Lock.
- Die konkrete WP-035-Ausführung ist weiterhin offen. Keine Assetproduktion oder fremde Freigabe. Runtimeimport, deterministischer Doppelimport, Addressables, Lizenz-/Größenbudgets, Geräte-/SDK-/Privacy-/Signing-/Store- und Rechtsendprüfung bleiben ihre zuständigen Folgeschritte.

Nächste Tätigkeit nach vollständiger Nachweisübergabe ist die konkrete Bewertung der Texte und ihrer tatsächlichen Nachweise. Die nächste Instanz beginnt mit AGENTS und dem eigenen Work Package; sie muss die Textproduktion nicht aus Chatfragmenten rekonstruieren.
