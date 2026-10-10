# Level Data Format v0.4

## 1. Vertrag und Geltung

Neue Authoringdaten verwenden UTF-8-JSON nach [`level-v2.schema.json`](./schemas/level-v2.schema.json). [`level-v2.example.json`](./examples/level-v2.example.json) und [`level-v2.single-cell.example.json`](./examples/level-v2.single-cell.example.json) sind reine **FIXTURE_ONLY**-Verträge und keine freigegebenen Season-1-Level. `level-v1` bleibt unverändert als Legacyreader- und Migrationsquelle erhalten. Level v2 bewahrt die in v1 gültigen Fokuswerte `OCCUPANCY`, `EXCLUSION`, `ENDPOINT_GEOMETRY`, `CHAIN`, `DENSITY` und `COMBINATION`; die neueren Werte sind additive Authoringbegriffe und erzwingen keine redaktionelle Umdeutung alter Level.

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

### 6.1 Content-Hash und Public-Puzzle-Hash

Die beiden Hashes sind getrennte Verträge und werden nie vermischt:

| Hash | Definition | Form | Bindet |
|---|---|---|---|
| **Public-Puzzle-Hash** | `STP-PUZZLE-SEMANTIC-JCS-1` über die Projektion der Tabelle oben | `{profile, sha256}` | ausschließlich den öffentlichen Rätselinput unter derselben `puzzleId`; Progress, Drafts und Release-Lock-Semantik hängen daran. |
| **Content-Hash** | SHA-256 über die JCS-Kanonisierung des **vollständigen** Level-v2-Dokuments | 64-stellige Kleinbuchstaben-Hexfolge, im Release-Lock und im Migrationsgolden als `documentSha256` | jedes Feld des Dokuments einschließlich Texte, Produktionsdaten, Authoringlösung und `proofRef`; Dokumentidentität, nicht Rätselidentität. |

Der Content-Hash trägt kein Profil, ersetzt keines der vier Profile und darf nie als Public-Puzzle-Hash, Lösungshash oder Proofhash verwendet werden. Er ändert sich bei jeder Dokumentänderung, der Public-Puzzle-Hash nur bei einer Änderung seiner Projektion.

### 6.2 JCS-Profil und Grenzen

JCS-Eingaben sind ausschließlich Integer-JSON: Zahlen im sicheren Bereich ±(2^53−1), kein Float, kein Exponent, kein `-0`. Objektschlüssel werden nach UTF-16-Codeeinheiten aufsteigend sortiert; Strings werden nach RFC 8785 maskiert (übrige Steuerzeichen als `\u00xx` in Kleinbuchstaben); Ausgabe ist UTF-8 ohne BOM und ohne Abschlussnewline. Alleinstehende Surrogate und Integer außerhalb des sicheren Bereichs sind harte Fehler (`LVL-JCS-*`). Die Implementierung wird durch unabhängig erzeugte Testvektoren (`Assets/StammstreckenPuzzle/Tests/EditMode/Content/level-v2-jcs-golden.json`, zusätzlich per Node-Referenzimplementierung nachgerechnet) abgesichert.

Ein programmgesteuert aufgebautes `JsonValue`-DOM kann Strings mit alleinstehenden Surrogaten enthalten (`JsonValue.CreateString` nimmt jeden .NET-String an); der Parser erzeugt sie nie. Gelangt ein solcher Freitext durch Schema, Domain-Abbildung und Semantik, melden `LevelV2Loader.Load(JsonValue)` und `LevelV1ToV2Migrator` ihn als reguläre Diagnose `LVL-JCS-SURROGATE` (Stufe Hash, Pfad des Strings): das Ergebnis ist `Rejected`, es werden keine Hashes gesetzt, und es wird keine Exception geworfen. Die öffentlichen werfenden Hash-Helfer (`LevelHashing.HashProjection`, `ComputeContentHash` u. a.) sind für bekannt gültige Projektionen bestimmt und werfen `InvalidOperationException`; für Projektionen aus nicht vertrauenswürdiger Quelle dienen `LevelHashing.Compute` und `JcsSerializer.Serialize`, die den Fehler als Diagnose zurückgeben.

`STP-PROOF-JCS-1` wird für ein Level-v2-Dokument nur als **Projektionshash** berechnet (vollständiges `proof-v1`-Objekt ohne `proofHash`). Das Proofartefakt selbst entsteht erst im späteren Solver-/Proofblock.

## 7. Proof-v1

[`proof-v1.schema.json`](./schemas/proof-v1.schema.json) definiert das geschlossene Proofartefakt. Es bindet `puzzleId`, profilierten Puzzlehash, profilierten Lösungshash, `solverVersion`, `solutionCount` und Metriken. Der Proofhash schließt alle diese Werte ein. Copy-Paste eines Proofs auf ein anderes Puzzle, ein stale Lösungshash oder eine Metrikänderung wird deshalb erkannt.

Gleicher öffentlicher Input und gleiche Solverversion müssen bytegleichen Proof erzeugen. Ein neuer Solververtrag benötigt eine neue `solverVersion`; er darf einen neuen Proof erzeugen, ohne `puzzleId` oder semantischen Puzzlehash zu ändern. Ein neues Proofformat benötigt `proofFormatVersion + 1` und einen expliziten Migrator/Reader.

Der Architecture-v0.4-Validator rehasht und bindet Fixtures und zählt ihre kleinen Lösungsmengen unabhängig nach. Die echte Proofregeneration durch den späteren C#-Solver ist **REQUIRED_LATER/NOT_EXECUTED**.

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
| `LVL-PARSE-*` | Parserstufe: leere/zu große Eingabe, BOM, ungültiges UTF-8, Syntax, Kommentare, doppelte Schlüssel, Float/Exponent, Zahlenbereich, `-0`, Surrogate, Escapes, Steuerzeichen, nachlaufender Inhalt, Tiefen-/Längengrenzen. |
| `LVL-VERSION-*` | unbekannte `documentSchemaVersion`, unbekanntes Ruleset oder Proofformat; bei unbekannter Dokumentversion wird ausschließlich dieser Code gemeldet. |
| `LVL-SCHEMA-*` | Strukturstufe gegen das geschlossene Schema: Typ, Pflichtfeld, unbekannte Eigenschaft, Konstante, Enum, Pattern, Wertebereich, Länge, Eindeutigkeit. |
| `LVL-DOMAIN-*` | Abbildung auf die Domain: Definition oder Ruleset von der Domain abgelehnt, Authoringlösung von der Domain nicht als abgeschlossen bewertet. |
| `LVL-JCS-*` | Kanonisierung: Integer außerhalb des sicheren Bereichs, alleinstehendes Surrogat, ungültige Eingabe. |
| `LVL-IMPORT-*` | Importgate; `LVL-IMPORT-PROOF-GATE-MISSING`: kein Proofartefakt gebunden, kein Release-Lock, Level nicht in einen Laufzeitkatalog importierbar. |
| `LVL_MIGRATION_NEEDS_EDITORIAL_DECISION` | Migration: ein v1-Wert lässt sich nicht neutral und schemagültig übernehmen. |
| `PRF-*` | Artefaktformat, Puzzle-/Lösungsbindung, Lösung genau eins und Proofhash. |
| `LOCK-*` | vollständiger aktueller Lock und keine publizierte semantische Mutation. |

Die Pipeline läuft strikt Parse → Schema → Domain-Abbildung → Semantik; eine spätere Stufe verdeckt nie eine frühere. Die Domain (`GridSize`, `Endpoint`, `PuzzleDefinition.Create`, `PuzzleEvaluator`) bleibt die alleinige Autorität über Rätselgültigkeit; die `LVL-GRID-*`-/`LVL-ENDPOINT-*`-Codes ordnen eine Domain-Ablehnung nur einer autorenfreundlichen Ursache zu. Das Dokument verlangt zusätzlich die Listenreihenfolge der Authoringlösung von A nach B; die Domain bewertet dagegen eine ungeordnete Zellmenge. Ein Dokument ist daher nur gültig, wenn die Domain die Lösung als abgeschlossen bewertet **und** die Listenreihenfolge A→B stimmt.

Eine Level-v2-Quelle, die alle Stufen besteht, hat den Importzustand `AwaitingProofGate` und ist nie laufzeitimportierbar, solange kein gebundenes Proofartefakt und kein Release-Lock existieren (`LVL-IMPORT-PROOF-GATE-MISSING`). Eine abgelehnte Quelle meldet ihre Diagnosen als Importblocker.

Ein `PRODUCT_APPROVED`-Season-1-Katalog muss exakt 5 Abschnitte × 4 Routen × 12 Level enthalten. Kleine `FIXTURE_ONLY`-Kataloge dürfen unvollständig sein, müssen aber ID-/Content-/Parentkonsistenz erfüllen.

## 9. Migration und Fortschritt

`level-v1 -> level-v2` läuft auf Kopie, ist deterministisch und idempotent. `id` wird zu `puzzleId`; alle öffentlichen Eingaben, Lösung, Texte, Completion- und Produktionsfelder bleiben erhalten. Die neue semantische Projektion und der Proof werden erzeugt, ohne `contentRevision` allein wegen des Formats zu erhöhen. Jeder v1-Positivfixture wird programmatisch migriert; zusätzlich setzt der Validator nacheinander alle sechs historischen Fokuswerte ein und verlangt, dass sie im v2-Schema unverändert gültig bleiben. Fehlende nicht neutral ableitbare Felder führen zu `LVL_MIGRATION_NEEDS_EDITORIAL_DECISION`.

Umsetzung der Migration (`LevelV1ToV2Migrator`): Zuerst werden die drei in der v1-Quelle aufgezeichneten Hashes (Puzzle-, Lösungs-, Proofhash) neu berechnet und gegen die Quelle geprüft (`LVL-HASH-MISMATCH`). `proofRef` wird ausschließlich aus aufgezeichneten v1-Fakten abgeleitet: `artifactId = proofs/<puzzleId>/<solverVersion>.proof-v1.json`, `proofFormatVersion = 1`, `proofHash` = `STP-PROOF-JCS-1`-Projektionshash aus puzzleId, beiden Hashes, `solverVersion`, `solutionCount` und den vier Metriken. Es läuft kein Solver und es entsteht kein Proofartefakt. Eine Editorialentscheidung ist nötig, wenn der v1-Wert das strengere v2-Schema verletzt (zum Beispiel `chainDepth` 0, ein Raster über 10, ein Schlüssel ohne gültiges v2-Muster) oder eine aufgezeichnete Proofmetrik unter dem `proof-v1`-Minimum von 1 liegt; es wird nie ein Wert erfunden. `MigrateToCurrent` migriert `schemaVersion 1`, validiert und reicht `documentSchemaVersion 2` unverändert durch (Idempotenz) und meldet jede andere oder gleichzeitig vorhandene Version als `LVL-VERSION-UNKNOWN`.

Progress und Drafts binden an `{puzzleId, publicPuzzleHash.profile, publicPuzzleHash.sha256}`. Das Migrationsgolden nennt repositoryrelative Pfade und vollständige JCS-Dokumenthashes der tatsächlichen v1-Quelle, des v2-Ziels und des Release-Locks. Der Validator lädt diese drei Dateien, prüft die Hashes, die semantisch neutrale Feldabbildung und den exakten Legacy-/Zielhasheintrag im Lock. Bei eindeutiger neutraler Zuordnung bleiben Erstabschluss, höchste Sterne, terminale Rewards, zulässige Bestzeit, direkte Lösung und Resume erhalten. Ohne eindeutige Bindung bleibt die Quelle unangetastet und `SAVE_LEVEL_IDENTITY_UNRESOLVED` wird gemeldet.

## 10. Append-only Release-Lock

[`release-lock-v1.schema.json`](./schemas/release-lock-v1.schema.json) hält pro Release Git-/Toolchain-/Katalogbezug und pro Puzzle Dokumenthash/-version, semantischen Puzzlehash, Lösungshash, Proofformat/-version/-hash und Legacybindungen fest. Die Registry ist append-only. Gleiche Puzzle-ID mit anderem semantischem Hash in zwei veröffentlichten Locks blockiert mit `LOCK_PUBLISHED_PUZZLE_MUTATED`; neue Dokument- oder Proofversion bei identischer Semantik ist zulässig.

## 11. Robustheit

Parser behandeln auch lokale Dateien als untrusted Input. Dateigröße, Verschachtelung, Stringlänge und technische Rastergröße werden begrenzt. Polymorphe JSON-Typnamen und automatische Typkonstruktion sind deaktiviert. Lokalisierungsschlüssel, Completionreferenzen und Assets müssen vollständig auflösbar sein.

Der strenge C#-Parser (`StrictJsonParser`) setzt dies mit folgenden Implementierungsgrenzen um; es sind Parsergrenzen, keine Schemagrenzen:

| Grenze | Wert |
|---|---|
| Eingabegröße | 256 KiB (UTF-8-Bytes; `Parse(byte[])` und `ParseText(string)` messen identisch) |
| Verschachtelungstiefe | 16 |
| String- und Schlüssellänge | 8192 UTF-16-Codeeinheiten (ein Surrogatpaar zählt als zwei und darf die Grenze nicht überschreiten) |
| Arraylänge | 1024 Einträge |
| Objektmitglieder | 64 |

Abgelehnt werden ferner: UTF-8-BOM, ungültiges UTF-8, Kommentare, nachlaufende Kommas und nachlaufender Inhalt, doppelte Schlüssel (mit Pfad), Float-/Exponent-Token, Zahlen außerhalb von ±(2^53−1), `-0`, alleinstehende Surrogate (roh und als `\u`-Escape) und rohe Steuerzeichen in Strings. Der Parser wirft nie; jeder Fehler ist genau eine stabile Diagnose. Das Level-v2-Schema erlaubt höchstens ein Raster von 10×10; eine spätere Anhebung ist eine Schemaänderung und keine Parserfrage.

Der DOM (`JsonValue`) ist nach der Konstruktion unveränderlich: `Items` und `Members` sind schreibgeschützte Sichten und geben nie den internen Speicher heraus, sodass ein Objekt nach der Erzeugung keine doppelten Schlüssel erhalten kann. Aus demselben Grund sind alle öffentlichen Ergebnislisten der Pipeline (`Diagnostics`, `ImportBlockers`) schreibgeschützte Momentaufnahmen; ein Aufrufer kann die Gültigkeit eines abgelehnten Ergebnisses nicht durch Leeren der Liste ändern.

## 12. Referenzen

[1]: ./schemas/level-v2.schema.json "JSON Schema für Leveldaten v2"
[2]: ./schemas/proof-v1.schema.json "JSON Schema für Proof v1"
[3]: ./schemas/release-lock-v1.schema.json "JSON Schema für Release-Lock v1"
[4]: ./examples/level-v2.example.json "Level-v2-Vertragsfixture"
[5]: ./examples/level-v2.single-cell.example.json "Ein-Zellen-Level-v2-Vertragsfixture"
[6]: ./examples/proof-v1.example.json "Proof-v1-Vertragsfixture"
[7]: ../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md "ADR-021 – Puzzleidentität und versionierte Proofartefakte"
[8]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
