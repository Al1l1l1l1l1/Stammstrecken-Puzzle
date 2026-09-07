# Regeln für Work Packages

Ein Work Package ist der verbindliche, abgegrenzte Arbeitsauftrag für einen Agenten. Es wird als eigene Markdown-Datei im Ordner `WORK_PACKAGES/` angelegt. Ohne ein passendes Work Package darf keine technische, konzeptionelle oder inhaltliche Arbeit begonnen werden.

## Verbindliches Format

Jedes Work Package muss die folgenden Abschnitte in dieser Reihenfolge enthalten.

| Abschnitt | Verbindlicher Inhalt |
|---|---|
| ID | Eindeutige, stabile Kennung im Format `WP-###`. Eine vergebene ID wird nicht wiederverwendet. |
| Ziel | Ein klarer, überprüfbarer Sollzustand des Arbeitspakets. |
| Voraussetzungen | Pflichtlektüre, bestätigte Vorentscheidungen, benötigte Zugänge sowie vorher abzuschließende Work Packages. |
| Scope | Die ausdrücklich erlaubten Tätigkeiten und Ergebnisgrenzen. |
| Betroffene Dateien/Module | Alle Dateien, Verzeichnisse, Module oder Dokumente, die erstellt, geändert oder geprüft werden dürfen. Nicht vorhandene Zielartefakte sind ausdrücklich als neu anzulegen zu kennzeichnen. |
| Ausdrücklich nicht erlaubte Änderungen | Produktentscheidungen, Architekturteile, Dateien, Module oder Verhaltensweisen, die vom Scope ausgenommen sind. |
| Akzeptanzkriterien | Objektiv prüfbare Bedingungen, anhand derer das Ergebnis abgenommen wird. |
| Tests | Konkrete vorgeschriebene Tests, Prüfschritte oder begründete Nichtanwendbarkeit. |
| Risikoklasse | Niedrig, mittel oder hoch einschließlich einer kurzen Begründung. |
| Definition of Done | Die auf dieses Work Package konkret angewandten Abschlussbedingungen gemäß `PROJECT_CONTROL/DEFINITION_OF_DONE.md`. |

## Verbindliche Zusatzregeln

1. Ein Work Package beschreibt nur einen kohärenten und begrenzten Auftrag. Es darf keine unspezifische Sammelliste enthalten.
2. Der Scope ist abschließend. Eine nicht genannte Änderung ist nicht automatisch erlaubt.
3. Akzeptanzkriterien und Tests müssen vor Beginn der Bearbeitung feststehen. Sie werden nicht nachträglich abgesenkt, um einen unvollständigen Stand als fertig zu deklarieren.
4. Abhängigkeiten und betroffene Dateien müssen den tatsächlich dokumentierten Projektstand widerspiegeln. Unklare Informationen werden als Blocker markiert, nicht ergänzt oder geraten.
5. Ein Work Package darf bestätigte Produktentscheidungen nicht stillschweigend verändern. Architekturentscheidungen benötigen gegebenenfalls vor der Umsetzung einen akzeptierten ADR gemäß `DECISIONS/README.md`.
6. Nach Abschluss müssen das Work Package selbst und `PROJECT_CONTROL/CURRENT_STATE.md` den nachvollziehbaren Abschluss- und Übergabestand enthalten.

## Risikoklassen

| Klasse | Bedeutung |
|---|---|
| Niedrig | Isolierter, klar reversibler Auftrag ohne Auswirkung auf bestätigte Produktentscheidungen, Architektur, Persistenz, Datenschutz, Sicherheit oder Releasefähigkeit. |
| Mittel | Auftrag mit mehreren betroffenen Komponenten oder einer relevanten Auswirkung auf Qualität, Daten, Tests oder Dokumentation, jedoch ohne grundlegende Architekturentscheidung. |
| Hoch | Auftrag mit möglicher Auswirkung auf Architektur, Persistenz, Datenschutz, Sicherheit, Monetarisierung, Releasefähigkeit oder bestätigte Produktleitplanken. |

## Vorlage

```md
# WP-### – Kurztitel

## ID
WP-###

## Ziel

## Voraussetzungen

## Scope

## Betroffene Dateien/Module

## Ausdrücklich nicht erlaubte Änderungen

## Akzeptanzkriterien

## Tests

## Risikoklasse

## Definition of Done
```
