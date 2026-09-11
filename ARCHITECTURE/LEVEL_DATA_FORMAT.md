# Level Data Format v0.3

## 1. Vertrag und Geltung

Neue Authoringdaten verwenden UTF-8-JSON nach [`level-v2.schema.json`](./schemas/level-v2.schema.json). [`level-v2.example.json`](./examples/level-v2.example.json) und [`level-v2.single-cell.example.json`](./examples/level-v2.single-cell.example.json) sind reine **FIXTURE_ONLY**-Verträge und keine freigegebenen Season-1-Level. `level-v1` bleibt unverändert als Legacyreader- und Migrationsquelle erhalten.

JSON Schema prüft Struktur, Typen, Enums und lokale Grenzen. Der semantische Validator prüft Cross-Field-Regeln, Kampagnenhierarchie, Zeitordnung, Pfadgeometrie, Hashprofile, Proofbindung, Eindeutigkeit und Releasehistorie. Erst danach darf ein Level in einen Laufzeitkatalog gelangen.

## 2. Koordinaten und Puzzleinvarianten

| Begriff | Festlegung |
|---|---|
| Ursprung | Rasterzelle links oben ist `(x=0, y=0)`. |
| Achsen | `x` wächst nach rechts, `y` nach unten. |
| Zeilen/Spalten | `rowCounts[y]` hat Länge `height`; `columnCounts[x]` hat Länge `width`. |
| Richtungen | `N`, `E`, `S`, `W`. |
| Endpoint N/S | `index` ist eine Spalte und `< width`. |
| Endpoint E/W | `index` ist eine Zeile und `< height`. |
| Pfad | Von A nach B, ohne Koordinatenwiederholung; ab zwei Zellen orthogonal benachbart. |

Endpoints liegen außerhalb des Rasters und zählen nicht in Randzahlen. A und B müssen verschiedene Außenanschlüsse sein, dürfen aber dieselbe angrenzende Zelle besitzen. Dann ist genau eine Trackzelle zulässig, wenn ihre Form beide Außenports und alle Zeilen-/Spaltenwerte erfüllt. Dies ändert keine Season-1-Contententscheidung.

Die zulässigen Gleisformen bleiben `TRACK_NS`, `TRACK_EW`, `TRACK_NE`, `TRACK_ES`, `TRACK_SW`, `TRACK_WN`. Leere und Hilfsmarkierungen gehören nur zum Spielerzustand.

## 3. Drei getrennte Identitätsebenen

| Ebene | Feld/Artefakt | Lebenszyklus |
|---|---|---|
| Fachliche Puzzleidentität | `puzzleId` | Dauerhaft; für Kampagnenlevel `S{season}-{section:00}-{route:00}-{position:00}`. |
| Dokumentformat | `documentSchemaVersion` | Wählt Parser/Migrator; darf sich bei semantisch neutraler Formatänderung ändern. |
| Solvernachweis | `proof-v1` | Generiertes, regenerierbares und solverversioniertes Artefakt. |

`puzzleId` wird nie aus Dateipfad, `contentRevision`, Text, Asset, Dokumentversion oder Solverversion abgeleitet. Für Season 1 gelten zusätzlich Abschnitt 1–5, Route 1–4 und Position 1–12. Die vier ID-Segmente müssen exakt `content.season`, `networkSection`, `route`, `position` und der Platzierung in `campaign-v2` entsprechen.

Ab der ersten Veröffentlichung ist die öffentliche Fachprojektion unter derselben Puzzle-ID unveränderlich. Eine logische Korrektur erhält eine neue Puzzle-ID. Ein Fortschrittstransfer ist ohne ausdrücklich freigegebenes, release-gelocktes Grandfathering-Manifest verboten.

Endless-Level verwenden weiter `endless-v1` und `E1-…`; sie werden nicht in Kampagnen-IDs gepresst.

## 4. Level-v2-Feldgruppen

| Feldgruppe | Zweck | Laufzeitbedarf |
|---|---|---:|
| `documentSchemaVersion`, `rulesetVersion` | Dokumentparser und Rätselregelvertrag | Ja |
| `puzzleId`, `contentRevision` | Fachidentität und redaktionelle Revision | Ja |
| `grid`, `endpoints`, `rowCounts`, `columnCounts` | öffentlicher Puzzleinput | Ja |
| `content` | hierarchiegebundene Metadaten und Lokalisierung | Ja |
| `production` | Didaktik, Qualität und Zeitkalibrierung | Teilweise |
| `completion` | Karten-, Zug- und Ergebnisreferenzen | Ja |
| `solution` | kanonische Authoringlösung | Nicht an die Puzzle-UI ausliefern |
| `proofRef` | Artefakt-ID, Format und Proofhash | Manifest/QA |

Der Runtimeimport trennt `RuntimePuzzleDefinition`, `LevelPresentationData` und `LevelQualityManifest`. Production liefert die Authoringlösung nicht an UI-Code aus; Hinweise werden regelbasiert berechnet.

## 5. Zeitdaten

`starThresholdsSeconds` ist entweder vollständig `null` oder ein Objekt mit positiven Integern und Kalibrierungsversion. Bei einem Objekt gilt strikt `threeStars < twoStars`. `null` ergibt technisch nur den ersten Stern und bleibt zulässig, bis Produktkalibrierung vorliegt. Der Validator erfindet keine Sekundenwerte.

## 6. Profilierte Hashverträge

Alle Projektionen werden nach RFC 8785/JCS als UTF-8 ohne BOM und Abschlussnewline kanonisiert und mit SHA-256 gehasht. Nicht-I-JSON, ungültige Surrogate und Fließkommazahlen sind verboten.

| Profil | Projektion |
|---|---|
| `STP-PUZZLE-SEMANTIC-JCS-1` | `{puzzleId,rulesetVersion,grid,endpoints,rowCounts,columnCounts}` |
| `STP-SOLUTION-JCS-1` | `{puzzleId,publicPuzzleHash,path}` |
| `STP-PROOF-JCS-1` | das vollständige `proof-v1`-Objekt ohne `proofHash` |
| `STP-LEVEL-V1-PUZZLE-JCS-1` | unveränderte historische v1-Projektion aus dem Legacyvertrag |

Jeder Hash wird als `{profile, sha256}` gespeichert. Ein unbekanntes Profil ist ein harter Fehler; kein Hash wird anhand seines Wertes heuristisch gedeutet. Formatierung, Schlüsselreihenfolge, `documentSchemaVersion`, `contentRevision`, Texte, Assets und Proofregeneration ändern den semantischen Puzzlehash nicht. Eine Änderung von Puzzle-ID, Ruleset, Raster, Endpoints oder Counts ändert ihn.

## 7. Proof-v1

[`proof-v1.schema.json`](./schemas/proof-v1.schema.json) definiert das geschlossene Proofartefakt. Es bindet `puzzleId`, profilierten Puzzlehash, profilierten Lösungshash, `solverVersion`, `solutionCount` und Metriken. Der Proofhash schließt alle diese Werte ein. Copy-Paste eines Proofs auf ein anderes Puzzle, ein stale Lösungshash oder eine Metrikänderung wird deshalb erkannt.

Gleicher öffentlicher Input und gleiche Solverversion müssen bytegleichen Proof erzeugen. Ein neuer Solververtrag benötigt eine neue `solverVersion`; er darf einen neuen Proof erzeugen, ohne `puzzleId` oder semantischen Puzzlehash zu ändern. Ein neues Proofformat benötigt `proofFormatVersion + 1` und einen expliziten Migrator/Reader.

Der Architecture-v0.3-Validator rehasht und bindet Fixtures und zählt ihre kleinen Lösungsmengen unabhängig nach. Die echte Proofregeneration durch den späteren C#-Solver ist **REQUIRED_LATER/NOT_EXECUTED**.

## 8. Semantische Validierung

| Codefamilie | Prüfung |
|---|---|
| `LVL-ID-*` | ID eindeutig; Segmente stimmen mit Content und Campaignhierarchie; Season-1-Bereiche 5×4×12. |
| `LVL-GRID-*` | Arraylängen und Werte gegen Rastergrenzen. |
| `LVL-ENDPOINT-*` | Indizes, verschiedene Außenanschlüsse und angrenzende Pfadzellen. |
| `LVL-PATH-*` | mindestens eine Zelle, keine Duplikate, Nachbarschaft und passende Anschlüsse. |
| `LVL-RULE-*` | einfacher Pfad A–B, keine Kreuzung, Schleife oder offene innere Verbindung. |
| `LVL-COUNT-*` | aus der Lösung abgeleitete Zeilen-/Spaltenzahlen. |
| `LVL-TIME-ORDER` | `threeStars < twoStars` oder vollständig `null`. |
| `LVL-HASH-*` | bekannte Profile und exakte JCS-Projektionen. |
| `PRF-*` | Artefaktformat, Puzzle-/Lösungsbindung, Lösung genau eins und Proofhash. |
| `LOCK-*` | vollständiger aktueller Lock und keine publizierte semantische Mutation. |

Ein `PRODUCT_APPROVED`-Season-1-Katalog muss exakt 5 Abschnitte × 4 Routen × 12 Level enthalten. Kleine `FIXTURE_ONLY`-Kataloge dürfen unvollständig sein, müssen aber ID-/Content-/Parentkonsistenz erfüllen.

## 9. Migration und Fortschritt

`level-v1 -> level-v2` läuft auf Kopie, ist deterministisch und idempotent. `id` wird zu `puzzleId`; alle öffentlichen Eingaben, Lösung, Texte, Completion- und Produktionsfelder bleiben erhalten. Die neue semantische Projektion und der Proof werden erzeugt, ohne `contentRevision` allein wegen des Formats zu erhöhen. Fehlende nicht neutral ableitbare Felder führen zu `LVL_MIGRATION_NEEDS_EDITORIAL_DECISION`.

Progress und Drafts binden an `{puzzleId, publicPuzzleHash.profile, publicPuzzleHash.sha256}`. Eine Save-Migration verwendet die historische Lockbindung des v1-Legacyhashes. Bei eindeutiger neutraler Zuordnung bleiben Erstabschluss, höchste Sterne, terminale Rewards, zulässige Bestzeit, direkte Lösung und Resume erhalten. Ohne eindeutige Bindung bleibt die Quelle unangetastet und `SAVE_LEVEL_IDENTITY_UNRESOLVED` wird gemeldet.

## 10. Append-only Release-Lock

[`release-lock-v1.schema.json`](./schemas/release-lock-v1.schema.json) hält pro Release Git-/Toolchain-/Katalogbezug und pro Puzzle Dokumenthash/-version, semantischen Puzzlehash, Lösungshash, Proofformat/-version/-hash und Legacybindungen fest. Die Registry ist append-only. Gleiche Puzzle-ID mit anderem semantischem Hash in zwei veröffentlichten Locks blockiert mit `LOCK_PUBLISHED_PUZZLE_MUTATED`; neue Dokument- oder Proofversion bei identischer Semantik ist zulässig.

## 11. Robustheit

Parser behandeln auch lokale Dateien als untrusted Input. Dateigröße, Verschachtelung, Stringlänge und technische Rastergröße werden begrenzt. Polymorphe JSON-Typnamen und automatische Typkonstruktion sind deaktiviert. Lokalisierungsschlüssel, Completionreferenzen und Assets müssen vollständig auflösbar sein.

## 12. Referenzen

[1]: ./schemas/level-v2.schema.json "JSON Schema für Leveldaten v2"
[2]: ./schemas/proof-v1.schema.json "JSON Schema für Proof v1"
[3]: ./schemas/release-lock-v1.schema.json "JSON Schema für Release-Lock v1"
[4]: ./examples/level-v2.example.json "Level-v2-Vertragsfixture"
[5]: ./examples/level-v2.single-cell.example.json "Ein-Zellen-Level-v2-Vertragsfixture"
[6]: ./examples/proof-v1.example.json "Proof-v1-Vertragsfixture"
[7]: ../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md "ADR-021 – Puzzleidentität und versionierte Proofartefakte"
[8]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
