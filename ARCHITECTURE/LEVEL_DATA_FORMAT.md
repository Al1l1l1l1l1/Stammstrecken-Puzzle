# Level Data Format v1

## 1. Vertrag und Geltung

Die kanonische Authoringquelle eines Levels ist UTF-8-JSON nach [`level-v1.schema.json`](./schemas/level-v1.schema.json). [`level-v1.example.json`](./examples/level-v1.example.json) und [`level-v1.single-cell.example.json`](./examples/level-v1.single-cell.example.json) sind reine Vertragsfixtures und ausdrücklich **keine freigegebenen Season-1-Produktionslevel**.

Das JSON Schema prüft Syntax, Typen, geschlossene Enums und lokale Wertebereiche. Der `LevelSemanticValidator` prüft alle Beziehungen, die JSON Schema nicht zuverlässig ausdrücken kann. Ein Level darf erst nach beiden Prüfungen, Eindeutigkeitsnachweis und Hashvergleich in einen Laufzeitkatalog gelangen.

## 2. Koordinaten- und Richtungsvertrag

| Begriff | Festlegung |
|---|---|
| Ursprung | Rasterzelle links oben ist `(x=0, y=0)`. |
| Achsen | `x` wächst nach rechts, `y` nach unten. |
| Zeilen | `rowCounts[y]`, Länge exakt `height`. |
| Spalten | `columnCounts[x]`, Länge exakt `width`. |
| Richtungen | `N`, `E`, `S`, `W`. |
| Endpoint N/S | `index` ist eine Spaltennummer und muss `< width` sein. |
| Endpoint E/W | `index` ist eine Zeilennummer und muss `< height` sein. |
| A-Anschluss | erster Pfadknoten; seine Gleisform muss an die Endpointseite anschließen. |
| B-Anschluss | letzter Pfadknoten; seine Gleisform muss an die Endpointseite anschließen. |
| Pfadreihenfolge | von A nach B, ohne Wiederholung einer Koordinate. |

Endpoints liegen außerhalb des Rasters, werden aber durch Seite und Index adressiert. Sie belegen keine Rasterzelle und zählen nicht in Randzahlen.

A und B müssen verschiedene Außenanschlüsse sein, dürfen aber dieselbe angrenzende Rasterzelle besitzen. In diesem Fall besteht ein technisch gültiger Pfad aus genau einer Trackzelle, wenn deren Form beide Endpointseiten verbindet und alle Zeilen-/Spaltenwerte exakt dazu passen. Das Schema erlaubt deshalb `solution.path` ab einem Eintrag. Season-1-Rastergrößen und Contentauswahl bleiben unverändert; die technische Zulässigkeit ist keine Pflicht, ein solches Produktionslevel zu verwenden.

## 3. Identität und Hierarchie

Kampagnen-IDs folgen `S{season}-{networkSection:00}-{route:00}-{position:00}`, beispielsweise `S1-03-11-08`. Für Season 1 gelten zusätzlich die semantischen Grenzen: Abschnitte 1–5, Routen innerhalb des Abschnitts 1–4 und Positionen 1–12. Die `content`-Felder müssen exakt zur ID passen.

Die ID bleibt über Text-, Asset- und Balancingrevisionen stabil. `contentRevision` steigt bei jeder zulässigen Änderung von Darstellungsschlüsseln, Produktionsmetadaten, Assets oder noch nicht final bestätigten Zeitwerten. Vor der ersten Veröffentlichung kann sie auch Änderungen am Authoringpuzzle begleiten.

**Korrektur des Veröffentlichungsvertrags:** Ab der ersten Veröffentlichung eines Kampagnenlevels sind dessen öffentliche Puzzleprojektion (`rulesetVersion`, `grid`, `endpoints`, `rowCounts`, `columnCounts`) und damit `puzzleHashSha256` unter derselben Level-ID unveränderlich. Eine logische Korrektur erhält eine neue Level-ID und eine ausdrücklich bestätigte Progress-/Grandfathering-Migration. Unter der bestehenden ID dürfen nur Texte, Assets, Produktionsnotizen und noch nicht bestätigte Zeitkalibrierung durch höhere `contentRevision` fortgeschrieben werden; Puzzle-, Lösungs- und Proofhash bleiben identisch. Vor der ersten Veröffentlichung dürfen Authoringrevisionen den Puzzleinput ändern, weil noch kein Spielerfortschritt existiert.

Katalogimport und Save-Laden vergleichen den veröffentlichten `puzzleHashSha256` mit dem Release-Lock. Ein abweichender Hash unter derselben veröffentlichten ID ist `LVL-PUBLISHED-PUZZLE-MUTATED` und blockiert Build sowie Laufzeitkatalog. Er wird niemals durch Übernahme alter Sterne, Rewards oder Bestzeiten auf den neuen Inhalt „migriert“.

Endloslevel verwenden den getrennten Vertrag `endless-v1` aus [`SOLVER_ARCHITECTURE.md`](./SOLVER_ARCHITECTURE.md). Generatorversion, Seed, Generationordinal und Parameterhash erzeugen eine deterministische ID. Sie werden nicht durch erfundene Kampagnen-IDs in dieses Schema gepresst.

## 4. Feldgruppen

| Feldgruppe | Zweck | Laufzeitbedarf |
|---|---|---:|
| `schemaVersion`, `rulesetVersion` | Parser- und Regelvertragsauswahl | Ja |
| `id`, `contentRevision` | stabile Identität und Revision | Ja |
| `grid`, `endpoints`, `rowCounts`, `columnCounts` | öffentliches Puzzle | Ja |
| `content` | Kampagnenhierarchie und Lokalisierung | Ja |
| `production` | Didaktik, Qualität und spätere Zeitkalibrierung | Teilweise |
| `completion` | Karten-, Zug- und Ergebnisreferenzen | Ja |
| `solution` | Authoringlösung und Validierungsbezug | Nicht an Puzzle-UI ausliefern |
| `validation` | Hashes, Solver- und Qualitätsnachweis | Manifest/QA; nur notwendige Teile in Runtime |

Der Runtimeimport erzeugt zwei getrennte Datensätze: `RuntimePuzzleDefinition` ohne kanonische Lösung und `LevelQualityManifest` für QA/Diagnose. In Development-/QA-Builds darf die Lösung für Debugwerkzeuge enthalten sein. In Production wird sie entfernt, soweit der Hinweissolver sie nicht benötigt; Hinweise werden aus Regeln berechnet.

## 5. Geschlossene Enums

### 5.1 Gleisformen

| Wert | Anschlüsse |
|---|---|
| `TRACK_NS` | N–S |
| `TRACK_EW` | E–W |
| `TRACK_NE` | N–E |
| `TRACK_ES` | E–S |
| `TRACK_SW` | S–W |
| `TRACK_WN` | W–N |

Leere und Hilfsmarkierungen sind Spielerzustände und erscheinen nicht in der Authoringlösung.

### 5.2 Produktionsschwerpunkte

`OCCUPANCY`, `EXCLUSION`, `ENDPOINT_GEOMETRY`, `CHAIN`, `DENSITY` und `COMBINATION` bilden die bestätigten Lern- und Qualitätsachsen ab. Sie sind keine zusätzlichen Rätselregeln.

### 5.3 Zeitdaten

`timeClass` ist eine vorläufige redaktionelle Klasse. `starThresholdsSeconds` bleibt `null`, bis konkrete Zeitwerte produktseitig kalibriert und bestätigt sind. Wenn Schwellen vorliegen, muss semantisch `threeStars < twoStars` gelten. Fehlende Schwellen ergeben technisch nur den ersten Stern; sie werden nie geraten.

## 6. Semantische Validierung

Der Validator liefert eine sortierte Liste stabiler Diagnosecodes und ist fehlerakkumulierend. Ein Fehler verhindert den Import; Warnungen benötigen im kuratierten Katalog eine explizite, versionierte Ausnahme.

| Codefamilie | Prüfung |
|---|---|
| `LVL-ID-*` | ID eindeutig, Hierarchie konsistent, Season-1-Bereiche korrekt. |
| `LVL-GRID-*` | Arraylängen, Werte höchstens Gegenachse, Season-1-Größe 4×4 bis 10×10 gemäß Contentposition. |
| `LVL-ENDPOINT-*` | Index im Bereich, A und B verschieden, angrenzende Pfadzellen korrekt. |
| `LVL-PATH-*` | mindestens eine Koordinate im Raster, keine Duplikate, bei mehreren Zellen orthogonal benachbart, passende Gleisanschlüsse; Ein-Zellen-Pfad verbindet beide Endpoints über dieselbe Zelle. |
| `LVL-RULE-*` | ein einfacher Pfad A–B, keine Kreuzung, keine Schleife, keine offenen inneren Anschlüsse. |
| `LVL-COUNT-*` | aus Lösung abgeleitete Zeilen-/Spaltenzahlen sind exakt identisch. |
| `LVL-HASH-*` | Puzzle-, Lösungs- und Proofhash entsprechen kanonischen Projektionen. |
| `LVL-SOLVER-*` | aktueller Solver findet genau eine Lösung; gespeicherte Lösung ist diese Lösung. |
| `LVL-CONTENT-*` | Lokalisierungsschlüssel, Kartensegment und Zugmoment existieren. |
| `LVL-TIME-*` | Schwellen vollständig, positiv und streng geordnet oder vollständig `null`. |
| `LVL-QUALITY-*` | Einstiegsschluss, Schwerpunkt, Kettentiefe und Qualitätsnotiz vorhanden; keine bloße Platzhalterphrase. |

Der Validator vertraut `validation.solutionCount` nie. Er berechnet die Lösung erneut und vergleicht den Nachweis.

## 7. Hashvertrag

Hashes verwenden SHA-256 über die exakten UTF-8-Ausgabebytes des **JSON Canonicalization Scheme (JCS) nach RFC 8785**.[5] JCS emittiert kein Whitespace, sortiert Objektschlüssel rekursiv nach UTF-16-Codeeinheiten, erhält Arrayreihenfolgen, serialisiert Primitive normativ und führt ausdrücklich keine Unicode-Normalisierung aus. Die Hashbytes enthalten weder Byte Order Mark noch abschließendes Zeilenende. Nicht-I-JSON-konforme Werte werden abgewiesen. Levelverträge verwenden weiterhin keine Fließkommazahlen.

| Hash | Projektion |
|---|---|
| `puzzleHashSha256` | `schemaVersion`, `rulesetVersion`, `id`, `grid`, `endpoints`, `rowCounts`, `columnCounts`. |
| `solutionHashSha256` | Objekt `{id, path}`. |
| `proofHashSha256` | `solverVersion`, `solutionCount`, `searchNodes`, `deductionSteps`, `maxDeductionDepth`, `requiredGuessDepth`. |

Der genaue Serializer erhält Golden Tests in C# und einem unabhängigen RFC-8785-kompatiblen CI-Werkzeug. Goldens decken Schlüsselreihenfolge, rekursive Objekte, Escapezeichen, Steuerzeichen, Nicht-ASCII, unterschiedliche Unicode-Normalformen und ungültige Surrogate ab. Unterschiedliche Formatierung der Quelldatei darf keinen Hash ändern; unterschiedliche Unicode-Codepunktfolgen bleiben nach RFC 8785 bewusst unterschiedlich.

## 8. Schema- und Ruleset-Versionierung

`schemaVersion` beschreibt die Form des Dokuments. `rulesetVersion` beschreibt die fachlichen Rätselregeln. Eine additive redaktionelle Eigenschaft erhöht wegen `additionalProperties: false` die Schema-Version. Eine neue Gleisform, ein Sonderfeld oder eine neue Gültigkeitsregel benötigt eine neue, ausdrücklich produktseitig bestätigte Ruleset-Version und ein neues ADR.

Parser akzeptieren nur explizit registrierte Versionen. „Best effort“ für unbekannte Daten ist verboten. Ein unbekanntes Schema erzeugt `LVL-SCHEMA-UNSUPPORTED`; der Build bricht ab. Production zeigt nie ein unvalidiertes Level.

## 9. Migration

Authoringmigrationen sind reine Kommandozeilen-/Editorfunktionen `LevelVn -> LevelVn+1`. Sie laufen auf einer Kopie, validieren das Ergebnis vollständig und schreiben erst nach erfolgreichem Roundtrip. Jede Migration besitzt:

- Golden Input/Output;
- Idempotenztest auf bereits migriertem Ziel;
- Negativtests für unvollständige Quelldaten;
- dokumentierte Standardwerte nur dann, wenn sie fachlich neutral sind;
- einen Migrationsbericht mit alten/neuen Hashes.

Kann eine neue Pflichtangabe nicht neutral hergeleitet werden, stoppt die Migration und verlangt redaktionelle Eingabe. Sie rät keinen Content.

## 10. Lokalisierung und Assets

JSON speichert stabile Lokalisierungsschlüssel, keine übersetzten Strings. Jeder Schlüssel muss in der deutschen Startlocale existieren. `mapSegmentId` und `trainMomentId` referenzieren typisierte Kataloge. Fehlende Referenzen sind Buildfehler und werden nicht durch generische Production-Fallbacks verdeckt.

## 11. Sicherheits- und Robustheitsgrenzen

Auch lokale Dateien werden als untrusted Input validiert. Parser begrenzen Dateigröße, Verschachtelung, Stringlängen und Raster auf 32×32 als technische Schutzgrenze. Season 1 bleibt fachlich auf die bestätigten Größen beschränkt. Externe JSON-Typmetadaten, polymorphe Typnamen und automatische Objektkonstruktion sind deaktiviert.

## 12. Beispielnachweise

Das Beispiel enthält genau einen einfachen Pfad: `(0,0) -> (0,1) -> (0,2) -> (1,2) -> (2,2) -> (2,1) -> (3,1) -> (3,2) -> (3,3)`. Daraus entstehen Zeilenwerte `[1,3,4,1]` und Spaltenwerte `[3,1,2,3]`. Die Eindeutigkeitsprüfung wurde als Vertragsprüfung mit Lösungslimit zwei ausgeführt und ergab exakt eine Lösung.

Das Ein-Zellen-Fixture verwendet ein 2×2-Raster, A an `N/0`, B an `W/0`, Zeilenwerte `[1,0]`, Spaltenwerte `[1,0]` und `TRACK_WN` in `(0,0)`. Die Endpoints sind verschieden, grenzen aber an dieselbe Zelle. Schema, Semantik, Hashes und erschöpfende Eindeutigkeitsprüfung müssen genau diese eine Lösung bestätigen.

## Referenzen

[1]: ./schemas/level-v1.schema.json "JSON Schema für Leveldaten v1"
[2]: ./examples/level-v1.example.json "Leveldaten-Beispiel v1"
[3]: ../DECISIONS/ADR-004-json-leveldaten-und-content-pipeline.md "ADR-004 – Versionierte JSON-Leveldaten"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md "Stammstrecken-Puzzle – Season-1-Content-Bible"
[5]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
[6]: ./examples/level-v1.single-cell.example.json "Leveldaten-Ein-Zellen-Beispiel v1"
