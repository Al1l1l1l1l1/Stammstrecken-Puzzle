# ADR-004 – Versionierte JSON-Leveldaten als einzige Content-Quelle

## Status

**Ersetzt**

## Datum

2026-09-07

## Kontext

240 kuratierte Season-1-Level und später generierte Rätsel benötigen lesbare Diffs, maschinenprüfbare Verträge, stabile IDs, Solver-Nachweise und eine Pipeline, die ohne implizites Editorwissen funktioniert. Die Content-Bible verlangt pro Level unter anderem Raster, A/B, Lösung, Randzahlen, Eindeutigkeit, Einstiegsschluss, Schwerpunkt, Schlusskettentiefe, Qualitätsnotiz, Zeitklasse und Abschlussinszenierung.

## Entscheidung

Die einzige kanonische Quelle eines Levels ist **UTF-8-JSON** ohne Kommentare. Der Vertrag verwendet JSON Schema Draft 2020-12 und beginnt mit `schemaVersion: 1` sowie `rulesetVersion: "train-track-v1"`. Koordinaten sind nullbasiert mit Ursprung links oben; Richtungen und Gleisformen verwenden geschlossene String-Enums.

Das Schema prüft Struktur und Typen. Ein separater semantischer Validator prüft Querschnittsregeln wie Arraylängen, Endpunktlage, Randzahlen, Pfadgeometrie, Lösungs-Hash, Eindeutigkeit und Contenthierarchie. Nur semantisch validierte Level dürfen in einen Build gelangen.

Ab der ersten Veröffentlichung ist die öffentliche Puzzleprojektion unter einer stabilen Kampagnen-Level-ID unveränderlich. Logische Korrekturen erhalten eine neue ID und benötigen eine ausdrücklich bestätigte Progress-/Grandfathering-Migration. Texte, Assets, Produktionsnotizen und später bestätigte Zeitwerte dürfen über eine getrennte `contentRevision` fortgeschrieben werden, ohne den Puzzlehash zu ändern.

Authoring-JSON enthält die kanonische Lösung und Produktionsmetadaten. Eine deterministische Importpipeline erzeugt daraus einen Laufzeitkatalog und lokale Addressables. Generierte Unity-Artefakte sind niemals Authoring-Quelle und werden nicht manuell editiert.

Migrationen sind reine, aufeinanderfolgende Funktionen `vN -> vN+1`. Downgrade wird nicht unterstützt. Unbekannte Versionen schlagen mit einem klaren Diagnosecode fehl. Puzzle-, Lösungs- und Proofprojektionen werden nach **RFC 8785 / JSON Canonicalization Scheme** kanonisiert und über deren exakte UTF-8-Ausgabebytes mit SHA-256 gebildet.

## Begründung

JSON ist diffbar, editorunabhängig und in jeder Agentenumgebung prüfbar. Schema plus semantischer Validator trennt formale Struktur von Puzzlelogik. Die Buildtransformation liefert Unity-Laufzeitleistung, ohne den verständlichen Ursprung zu verlieren.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| ScriptableObjects als Quelle | Schlechter außerhalb des Editors prüfbar und anfällig für GUID-/YAML-Fehler. |
| Tabellenkalkulation als Quelle | Praktisch für Übersicht, aber ungeeignet für verschachtelte Lösungspfade und strikte Typverträge. Import darf optional sein, nie kanonisch. |
| YAML | Lesbar, aber mehrdeutige Typisierung und weniger einheitliche mobile Toolunterstützung. |
| Binärformat als Quelle | Kompakt, aber nicht review- oder agentenfreundlich. |

## Konsequenzen

Schema, Beispiel, Migratoren und semantischer Validator sind versionsgleich zu pflegen. Jede Schemaänderung benötigt eine Migration oder eine neue Ruleset-Version. Der Build muss generierte Kataloge reproduzierbar erzeugen.

## Betroffene Artefakte

`ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/CONTENT_PIPELINE.md`, `ARCHITECTURE/schemas/level-v1.schema.json` und `ARCHITECTURE/examples/level-v1.example.json`.

## Validierung

CI validiert jede Quelldatei gegen das Schema, führt die semantische Prüfung und Eindeutigkeitszählung aus und vergleicht den reproduzierten Katalog-Hash mit dem erwarteten Ergebnis.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Vollständig ersetzt durch [ADR-021](./ADR-021-puzzleidentitaet-und-proofartefakte.md), der alle fortgeltenden JSON-Source-, Schema-, Semantik-, Unveränderlichkeits-, Pipeline- und Migrationsgrundsätze explizit restatiert und Dokumentformat, fachliche Puzzleidentität sowie Proofartefakt trennt.

## Referenzen

[1]: ../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md "Stammstrecken-Puzzle – Season-1-Content-Bible"
[2]: ../WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md "WP-001 – Technische Produktionsspezifikation"
