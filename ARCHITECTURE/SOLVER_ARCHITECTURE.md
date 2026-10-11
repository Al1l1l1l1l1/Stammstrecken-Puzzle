# Solver Architecture v0.4

## 1. Verantwortungsgrenze

`STP.Puzzle.Solver` beantwortet vier Fragen: Ist ein Puzzle lösbar? Wie viele Lösungen besitzt es bis zum Limit zwei? Welche eindeutige Lösung existiert? Welche sicheren, erklärbaren Deduktionsschritte und Qualitätsmetriken entstehen? Es entscheidet keine Sterne, Werbung, Kampagnenfreigabe oder UI-Darstellung.

Der Solver konsumiert nur `PuzzleDefinition` und optional einen sichtbaren partiellen Zustand. Er kennt weder die gespeicherte Authoringlösung noch Unity.

## 2. Constraint-Modell

Jede Zelle besitzt eine Domäne aus sieben Werten: `EMPTY` und sechs konkrete Trackformen. Hilfsmarkierungen sind keine Lösungswerte. Ein partieller Spielerzustand darf ausschließlich in einem internen Diagnose-/Authoringmodus als zusätzliche Annahme geprüft werden; Ergebnisse dieses Modus erreichen die normale Spieleroberfläche nicht.

Der normale Hintmodus löst immer den **unveränderten öffentlichen Puzzleinput ohne X-, Grau- oder konkrete Spielerannahmen**. Der sichtbare Sessionstate dient danach nur als Auswahlfilter: Bevorzugt wird eine sichere Schlussfolgerung auf einer `UNSET`-Zelle, die keiner nicht objektiv fehlerhaften Annahme direkt widerspricht. Ist kein solcher Schritt verfügbar, liefert der Solver `NO_NON_REVEALING_HINT_AVAILABLE` statt einen Widerspruch oder die Falschheit einer plausiblen Annahme offenzulegen. Objektiv sichtbare Verstöße bleiben ausschließlich Sache der Domain-Diagnostik.

| Constraint | Wirkung |
|---|---|
| Rastergrenze | entfernt Formen mit Port an ungültiger Außenkante. |
| Endpoint | angrenzende Zelle muss passenden Außenport besitzen; andere Außenports bleiben verboten. |
| Zellnachbar | Port zu Nachbar genau dann, wenn Nachbar Gegenport besitzt. |
| Zeilensumme | exakt `rowCounts[y]` Nicht-EMPTY-Werte. |
| Spaltensumme | exakt `columnCounts[x]` Nicht-EMPTY-Werte. |
| Zellgrad | konkrete Form hat genau zwei Ports; keine Kreuzung/T-Knoten. |
| Subtour | keine geschlossene Komponente ohne beide Endpoints. |
| Globale Erreichbarkeit | verbleibende Domänen müssen noch eine mögliche A-B-Verbindung zulassen. |
| Finale Gesamtheit | alle Nicht-EMPTY-Zellen liegen auf genau einem einfachen A-B-Pfad. |

Constraints liefern Kandidatenreduktionen mit stabilen Reason Codes und referenzierten Zellen/Linien.

## 3. Propagation

Eine deterministisch sortierte Work Queue verarbeitet betroffene Constraints bis zum Fixpunkt. Reihenfolge:

1. Raster- und Endpointports;
2. konkrete Nachbarkonsistenz;
3. Zeilen- und Spaltenunter-/obergrenzen;
4. lokale Graderreichbarkeit;
5. Komponenten- und Zyklusgrenzen;
6. globale mögliche A-B-Erreichbarkeit.

Jede Reduktion protokolliert `(ruleCode, premises, conclusion)`. Identische Reduktionen werden dedupliziert. Ein leerer Wertebereich ist ein Widerspruch. Ein Singleton wird als Zuweisung behandelt und löst neue Constraints aus.

## 4. Suche und Lösungslimit

Bleibt nach Propagation Mehrdeutigkeit, wählt Minimum Remaining Values die Zelle mit kleinster Domäne größer eins. Gleichstände werden nach `y`, dann `x` aufgelöst. Wertreihenfolge ist `EMPTY`, `TRACK_NS`, `TRACK_EW`, `TRACK_NE`, `TRACK_ES`, `TRACK_SW`, `TRACK_WN`.

Depth-First Search kopiert einen kompakten Solverstate, nimmt einen Wert an und propagiert. Die Suche stoppt sofort nach zwei vollständigen Lösungen. Ergebnis:

| Anzahl | Klassifikation |
|---:|---|
| 0 | `UNSATISFIABLE` |
| 1 | `UNIQUE` |
| 2 | `MULTIPLE_OR_MORE` |

Ein Timeout oder Ressourcenlimit ist `INDETERMINATE`, niemals `UNIQUE`.

## 5. Eindeutigkeitsnachweis

Der Content-Validator führt den Solver aus dem unveränderten öffentlichen Puzzleinput aus. Er vergleicht die einzige gefundene Lösung mit der Authoringlösung nach kanonischem Lösungshash. Der Nachweis liegt als separates `proof-v1`-Artefakt vor und enthält:

- `solverVersion`;
- `puzzleId` sowie den profilierten `publicPuzzleHash`;
- den profilierten `solutionHash`;
- `proofFormatVersion`;
- `solutionCount` bis zwei;
- Zahl der Suchknoten;
- Zahl erklärbarer Deduktionsschritte;
- maximale Deduktionskettentiefe;
- benötigte Annahmetiefe;
- den profilierten `proofHash` über alle vorgenannten Felder außer sich selbst.

Gespeicherte Werte sind ein Cache. CI berechnet sie mit dem Produktionssolver neu. Ein anderer Proof bei identischer Solver-Version und identischem Puzzlehash ist `PRF-NONDETERMINISTIC`. Eine neue Solverversion darf ein neues Proofartefakt liefern, ohne `puzzleId` oder den semantischen Puzzlehash zu ändern. Der lokale Architekturvalidator prüft Bindung und kleine Fixtures, behauptet aber keine Regeneration durch noch nicht vorhandenen Produktionscode.

## 6. Deduction Tracer und Hinweise

Der Tracer unterscheidet beweisbare Regeln, zum Beispiel:

| Reason Code | Beispiel |
|---|---|
| `LINE_ZERO` | Zeile/Spalte mit Ziel 0 ist leer. |
| `LINE_ALL_REMAINING_OCCUPIED` | Restkapazität entspricht Zahl verbleibender Kandidaten. |
| `LINE_TARGET_REACHED` | übrige Kandidaten sind leer. |
| `ENDPOINT_ENTRY_REQUIRED` | Zelle an A/B muss belegt sein und passenden Port haben. |
| `BORDER_PORT_FORBIDDEN` | Form darf nicht aus falscher Kante führen. |
| `NEIGHBOR_PORT_REQUIRED` | gesetzter Port erzwingt Gegenport. |
| `LOOP_PREVENTION` | Kandidat würde eine unzulässige Teilrunde schließen. |
| `CONNECTIVITY_PRESERVATION` | Kandidat würde A/B oder Pflichtbereiche trennen. |

Ein Hint besteht aus genau einer Schlussfolgerung und den minimalen Prämissen aus dem öffentlichen Puzzle. Application filtert nach Sessionanzeige und der noch festzulegenden Produktdarstellung. Suchannahmen, Unsatisfiability wegen Spielerinhalt und Widerspruchsauflösung gegen X/Grau/plausible konkrete Gleise sind keine normalen Hinweise. Ein Level, das ohne Suchannahme nicht lösbar ist, kann nicht als normale deduktive Kampagnenaufgabe freigegeben werden.

Der Verbrauch und die Persistenz von Hintcredits unterliegen `BLOCKER-PROD-001` in [`OPEN_BLOCKERS.md`](./OPEN_BLOCKERS.md). Solver und Testfakes können vorher implementiert werden; eine finale Hint-Economy darf nicht veröffentlicht werden.

## 7. Qualitäts- und Schwierigkeitsmetriken

Der Solver liefert Rohmetriken, keine endgültige menschliche Schwierigkeit.

| Metrik | Bedeutung |
|---|---|
| `initialForcedMoves` | Zugänglichkeit des Einstiegs. |
| `deductionSteps` | Zahl einzelner sicherer Folgerungen. |
| `maxDeductionDepth` | längste Prämissenabhängigkeit. |
| `simultaneousFronts` | Zahl unabhängiger aktiver Schlussbereiche. |
| `maxDomainAmbiguity` | verbleibende Formenvielfalt. |
| `searchNodes` | technische Suchlast. |
| `requiredGuessDepth` | 0 für rein deduktive Standardlevel. |
| `pathLength`, `density` | belegte Zellen und Anteil am Raster. |
| `symmetryScore` | Diagnose für mögliche Redundanz, kein Qualitätsurteil allein. |

Contentkuratoren kombinieren diese Werte mit Einstiegsschluss, Qualitätsnotiz, Nichtredundanz und Spieltests. Zeitgrenzen werden daraus nicht automatisch erzeugt.

## 8. Generatorarchitektur

Der spätere Dauerbaustellen-Generator ist eine getrennte Assembly beziehungsweise ein getrenntes Tool. Seine Pipeline lautet:

1. Zielparameter und expliziten Seed festlegen.
2. A/B und einen einfachen Pfad ohne Wiederholung erzeugen.
3. Randzahlen ableiten.
4. Authoringlösung aus dem öffentlichen Puzzleinput entfernen.
5. Solver bis Limit zwei ausführen.
6. Nur `UNIQUE` akzeptieren.
7. Deduktions- und Qualitätsmetriken berechnen.
8. Technische Filter anwenden.
9. Menschlich kalibrierte Qualitätsprofile anwenden.
10. JSON plus Proof erzeugen und erneut unabhängig validieren.

Generator und Solver teilen Domainregeln, aber der Generator darf keinen internen „bekannten Pfad“-Shortcut im Eindeutigkeitsnachweis verwenden. Seeds, Generatorversion und Zielparameter werden für Reproduktion protokolliert. Der Generator gehört nicht zum Scope der Architecture-Implementierung und wird hier nur als Vertrag festgelegt.

## 9. Generatorvalidierung

| Gate | Abweisung bei |
|---|---|
| Schema | formal ungültigem Datensatz. |
| Domain | ungültigem Pfad, Counts, Endpoint oder Hash. |
| Unique | 0, 2+ oder unbestimmten Lösungen. |
| Deduction | unerlaubter Annahmetiefe für normalen Modus. |
| Difficulty band | Metriken außerhalb gewählter Klasse. |
| Readability | fehlendem Einstieg, extremem Chaos oder unlesbarer Dichte. |
| Novelty | zu großer struktureller Ähnlichkeit mit jüngsten/kuratierten Leveln. |
| Runtime budget | Solver- oder Hintlimit überschritten. |

Die Akzeptanzquote ist eine Qualitätsmetrik; sie darf nicht durch Absenken der Gates künstlich erhöht werden.

### 9.1 Versionierter Qualitätsprofilvertrag

Ein `GeneratorQualityProfile` ist eine kanonische, gehashte Datei und enthält mindestens:

| Feld | Vertrag |
|---|---|
| `profileId`, `profileVersion`, `rulesetVersion` | stabile Identität und Kompatibilität. |
| `approvalStatus` | `DRAFT`, `LEVEL_DESIGN_REVIEWED` oder `PRODUCT_APPROVED`. |
| `difficultyBand` | benannte Zielklasse ohne automatische Gleichsetzung mit Rastergröße. |
| `gridRanges` | erlaubte Breite/Höhe und Dichteintervalle. |
| `metricRanges` | Min/Max je verwendeter Solvermetrik einschließlich Guessdepth. |
| `runtimeBudgets` | harte Solver-/Hintgrenzen und Verhalten bei Timeout. |
| `novelty` | Algorithmus-/Versions-ID, Referenzkorpus-Hash, Vergleichsfenster, Distanzmetrik und Schwelle. |
| `humanReview` | Stichprobengröße, Prüfkriterien, Eigentümerrollen und akzeptierter Status. |
| `approvedBy`, `approvedDecisionRef` | persistente Produktfreigabe, nicht bloß Agentenname. |

Profile dürfen keine fehlenden Grenzwerte aus Defaults ableiten. Unbekanntes Feld, fehlende Metrik, anderer Solver-/Korpus-Hash oder Status unter `PRODUCT_APPROVED` ergibt `GEN-PROFILE-NOT-PUBLISHABLE`. Der Release-Lock nennt exakt Profilhash und Referenzkorpus. Bis `BLOCKER-PROD-003` geschlossen ist, existiert kein freigegebenes Profil; Generatorausgaben bleiben interne `DRAFT`-Artefakte und dürfen Production nicht erreichen.

### 9.2 Endless-Levelvertrag `endless-v1`

Die Identität einer generierten Meldung ist vollständig von Kampagnen-IDs getrennt. Vor jeder Generierung wird ein kanonischer Deskriptor erstellt:

| Feld | Vertrag |
|---|---|
| `endlessContractVersion` | `1`; wählt Form und ID-Projektion. |
| `rulesetVersion` | registrierter Puzzle-Regelvertrag. |
| `generatorVersion` | geschlossene Version von Algorithmus, Zufallsfolge und Tiebreakern. |
| `seed` | vorzeichenloser 64-Bit-Wert als kanonischer Dezimalstring ohne führende Nullen, außer `0`. |
| `generationOrdinal` | lokal monotoner positiver 64-Bit-Ordinal als Dezimalstring; wird vor Generierung reserviert. |
| `parameters` | vollständig aufgelistete relevante technische Zielparameter; keine versteckten Defaults. |
| `parameterHashSha256` | RFC-8785/JCS-SHA-256 über exakt `parameters`. |

Die stabile ID lautet `E1-<hex>`, wobei `<hex>` der vollständige kleingeschriebene SHA-256-Hexwert der JCS-Projektion `{endlessContractVersion,rulesetVersion,generatorVersion,seed,generationOrdinal,parameterHashSha256}` ist. Nach Generierung ergänzt der Runtime-/Diagnosedatensatz `publicPuzzleHash{profile,sha256}`, `solutionHash{profile,sha256}`, `proofHash{profile,sha256}`, Solverversion und Status.

Der Save reserviert `generationOrdinal` atomar **vor** Generierung, indem er ausschließlich `highestReservedOrdinal + 1` zusammen mit `RESERVED_NOT_GENERATED` und dem vollständigen Deskriptor persistiert. Dieser Zustand fordert noch keinen Puzzleinput. Crash/Retry verwendet denselben Descriptor und erzeugt keine zweite Identität. Gleiche ID plus gleicher Deskriptor ist dieselbe Instanz. Gleiche ID mit anderem Deskriptor ist `GEN-ENDLESS-ID-COLLISION` und fatal.

Nach Generierung werden Schema, Domain, Unique, Lösungshash und Proof vollständig geprüft. Erst dann ersetzt ein atomarer Commit `RESERVED_NOT_GENERATED` durch `ACTIVE_DRAFT` mit Deskriptor, öffentlichem Puzzleinput, profilierten Hashes und Sessionstate. Dadurch ist Wiederaufnahme möglich, auch wenn die Generatorbinary später nicht mehr enthalten ist. Ist bei einer offenen Reservation die gebundene Generatorversion nicht verfügbar, bleibt `ENDLESS_GENERATOR_RECOVERY_REQUIRED`; die Instanz wird weder neu reserviert noch still terminalisiert.

Die Lebensdauer ist durch einen konstanten autoritativen Zustand begrenzt, nicht durch die Anzahl terminaler Instanzen:

- höchstens 20 offene Endless-Records über `RESERVED_NOT_GENERATED`, `ACTIVE_DRAFT` und `COMPLETION_CLAIM_OPEN`; der 21. offene Vorgang wird mit `ENDLESS_OPEN_CAPACITY` abgewiesen;
- `open(o)` gilt exakt bei vorhandenem Open-Record; `terminal(o)` gilt für jeden reservierten Ordinal bis zum Watermark ohne Open-Record;
- Complete schreibt alle zulässigen Completioneffekte und ersetzt den aktiven Draft atomar durch `COMPLETION_CLAIM_OPEN`; erst Claimcommit oder `CLOSED_NO_REWARD` entfernt den letzten Open-Record; Abandon entfernt Reservation/Draft ohne Reward;
- ein terminaler Ordinal ist dauerhaft nicht erneut generierbar oder rewardberechtigt und liefert bei Wiederholung `ENDLESS_TERMINAL_DUPLICATE` beziehungsweise No-op;
- terminale Statusintervalle und Deskriptorlisten sind keine fachliche Savewahrheit; höchstens 64 optionale Diagnoseeinträge dürfen best-effort im lokalen Ring liegen;
- nur `ENDLESS_ORDINAL_SPACE_EXHAUSTED` beendet nach vollständiger UInt64-Ausschöpfung neue Reservationen.

Deterministische Generationswiederholung ist für `RESERVED_NOT_GENERATED`, volle Puzzlewiederaufnahme für `ACTIVE_DRAFT` und begrenzte Rewardreconciliation für `COMPLETION_CLAIM_OPEN` garantiert. Für terminale Instanzen werden bewusst keine dauerhaft fachlich relevanten Detail-/Statusdaten versprochen; eine spätere solche Produktanforderung benötigt eine neue Entscheidung. Der Save wächst bei Endlosnutzung nicht mit der Terminalhistorie.

Dieser Identitätsvertrag entscheidet keine Qualitätsgrenze. Ohne `PRODUCT_APPROVED`-GeneratorQualityProfile bleibt jede Instanz unveröffentlichbar.

## 10. Laufzeitnutzung

Completion verwendet den schnelleren Domainvalidator, nicht den vollständigen Solver. Der Solver läuft zur Laufzeit nur für ausdrücklich angeforderte Hinweise und spätere generierte Level. Er arbeitet außerhalb des Renderframes auf einer kontrollierten Task, erhält Cancellation und ein Plattformbudget. Ergebnisse werden nur übernommen, wenn `levelHash` und `sessionRevision` noch passen.

Startbudgets für Season 1 auf einem CI-Referenzrechner:

| Operation | Budget |
|---|---:|
| Lösung zählen bis zwei, 10×10 kuratiert | p95 < 250 ms, Maximum 2 s |
| sicherer nächster Hinweis, 10×10 | p95 < 100 ms, Runtime Maximum 1 s |
| gesamter Season-1-Katalog in CI | < 60 s parallelisiert, Ergebnisse deterministisch sortiert |

Überschreitungen ergeben kein stilles Timeout-„Unique“. Sie blockieren Content oder liefern zur Laufzeit `HINT_TEMPORARILY_UNAVAILABLE` ohne Werbekonsum/Belohnungsverlust.

## 11. Teststrategie

Pflichtfixture sind mindestens: unlösbar, exakt eindeutig, technisch gültiger Ein-Zellen-Pfad, zwei Lösungen, Endpointfehler, Randzahlfehler, isolierte Schleife, getrennte Komponente, lange eindeutige Kette und gültige 10×10-Grenze. Metamorphic Tests rotieren/spiegeln Puzzle samt Endpoints und Counts; Lösungsklasse muss invariant bleiben. `proof-v1`-Artefakte sind Golden Files pro Solverversion. Zusätzlich laufen 10.000 alternierende Endless-Complete-/Abandon-Transitionen, Resume-Lücken und Duplicate-after-Compaction.

Ein kleiner unabhängiger Exhaustive Enumerator prüft alle sehr kleinen Rastersubräume in Tests gegen den Produktionssolver. Er wird nicht in Production ausgeliefert und reduziert das Risiko gemeinsamer Regelbugs. Der Endless-Referenzreducer prüft zusätzlich Reserve → Generate → Promote → Resume/Complete/Abandon → Claimreconciliation → Terminal sowie Crashpunkte und 10.000 gemischte Zyklen.

## 12. Versionierung

`solver-v1` umfasst Constraintsemantik, Tiebreaker, Wertreihenfolge und Metrikdefinitionen. Performanceoptimierungen, die Proofreihenfolge oder Metriken ändern, erhöhen die Solverversion. Alle kuratierten Proofs werden dann bewusst regeneriert, diffgeprüft und erneut freigegeben.

`solver-v1` bleibt als Wert in historischen Daten lesbar. Neue Solverergebnisse tragen `solver-v2` (ADR-031, WP-024); der gelieferte Stand ist in Abschnitt 13 beschrieben. `solver-v2` ändert weder Constraintsemantik noch Tiebreaker oder Wertreihenfolge von `solver-v1`, sondern ergänzt Root-Spur, Metriken und Kampagnengate.

## 13. Implementierungsstand `solver-v2` (WP-024)

Dieser Abschnitt beschreibt den tatsächlich gelieferten Stand von `STP.Puzzle.Solver`. Er ändert den Vertrag von ADR-031 nicht. Proofschema, Proofdateien, Strict-Validation und Contentpipeline gehören zum späteren Proof-v1-Block und sind nicht Teil dieses Stands.

### 13.1 Ergebnisobjekt

`PuzzleSolver.SolverVersion` und jedes `SolverResult` melden `solver-v2`. Ein `SolverResult` enthält `Classification`, `SolutionCount` (bis zwei), `Path`, das Recovery-Protokoll `Reductions` sowie `DeductionTrace` und `Metrics`.

- `Reductions` bleibt das bisherige Recovery-Protokoll über Root und Suchzweige. Es ist weder Beweis noch Spur und geht in keine Metrik ein.
- `DeductionTrace` ist die auditierbare Root-Spur. `Metrics` enthält die vier ADR-031-Metriken.
- Beides gibt es nur bei `Classification = UNIQUE`. Für `UNSATISFIABLE`, `MULTIPLE_OR_MORE` und `INDETERMINATE` ist `Metrics` leer (`null`) und `DeductionTrace` leer. Keine Quelle legt Metriken für diese Fälle fest; es wird daher kein Pfadmetrikergebnis als gültig ausgegeben (konservative Lesart von ADR-031 Abschnitt 4).

### 13.2 Root und Suche

Die Root-Propagation ist die erste Suchebene vor der ersten DFS-Annahme. Nur sie wird protokolliert. Jede begonnene Wertannahme (Kindaufruf der Suche) zählt als genau ein `searchNodes`, gleichgültig ob der Zweig zur Lösung führt oder verworfen wird; die Wurzel zählt nicht. Das interne Zustandsbudget (Wurzel plus alle Annahmen) und seine Prüfreihenfolge bleiben unverändert und sind getrennt von den Metriken.

Hartes Budget: Zeitbudget, Cancellation und monotone Uhr werden beim Eintritt in jeden Suchknoten, vor jeder Propagationsstufe und zuletzt genau einmal nach der gesamten budgetierten Arbeit geprüft. Budgetierte Arbeit ist alles, was zum Ergebnis führt: Suche, Vollständigkeitsprüfung der Blätter, Deduplizierung des Recovery-Protokolls, Aufbau der Root-Spur, Metrikberechnung und Konstruktion des `UNIQUE`-Ergebnisses. `PuzzleSolver.Solve` baut das `UNIQUE`-Ergebnis deshalb vollständig, bevor die letzte Prüfung läuft. Ist das Budget dann erschöpft, die Cancellation gesetzt oder die Uhr rückläufig, ist das Ergebnis `INDETERMINATE` ohne Spur und ohne Metriken; nach der letzten Prüfung folgt nur noch die Rückgabe des fertigen Objekts. Das Zustandslimit (`maxStates`) wird vor Beginn der Arbeit eines Knotens geprüft und hat diese Lücke nicht. Das Budget ist kooperativ: Ein Aufruf kann es um die Dauer des Abschnitts zwischen zwei Prüfungen überschreiten, liefert dann aber nie `UNIQUE`.

### 13.3 Spureintrag

Jeder `DeductionStep` ist eine unveränderliche, atomare Konklusion „Zelle *c* kann die Werte *R* nicht mehr annehmen". Er enthält `Index`, `RuleCode` (einer der acht Reason Codes aus Abschnitt 6 plus `LOCAL_DEGREE_REQUIRED`), die Zelle, die entfernten und die verbleibenden Kandidatenwerte, die Prämissen und die Tiefe. Eine Reduktion, die mehrere Werte derselben Zelle gleichzeitig entfernt, ist ein Schritt. Identische Reduktionen werden nicht doppelt aufgenommen.

Prämissen sind ausschließlich

- öffentliche Puzzlefakten (`GRID_SIZE`, `ENDPOINT_A`, `ENDPOINT_B`, `ROW_COUNT`/`COLUMN_COUNT` einer Linie) oder
- Indizes früherer Spureinträge.

Ein Schritt deklariert genau die öffentlichen Fakten, die seine Ableitung liest. Die Ableitung ist eine deterministische Funktion dieser Fakten und der Konklusionen der gewählten früheren Einträge: Ein Fakt, den sie nicht liest, kann die Konklusion nicht beeinflussen, und ein gelesener Fakt wird immer deklariert. Daraus folgt die Garantie der Spur: Jeder veröffentlichte Schritt gilt für **jedes** gültige Puzzle, das in den deklarierten Fakten mit dem Ausgangspuzzle übereinstimmt (andere Rastergröße, andere Endpoints, andere Summen sind dann zulässig, soweit sie nicht deklariert sind). Eine feste Faktentabelle je Regelcode, wie sie der erste Stand dieses Abschnitts festlegte, erfüllt das nicht (siehe `WP-024`, QC-Befund 1) und ist ersetzt.

Gelesene Fakten je Regel:

| Regel | Deklarierte öffentliche Fakten |
|---|---|
| `BORDER_PORT_FORBIDDEN`, `ENDPOINT_ENTRY_REQUIRED` | `GRID_SIZE` (Rand und Zellenlage) und die Endpoints, die die Randprüfung liest. Ob ein Außenport ein Endpointport ist, hängt von **beiden** Endpoints ab. Wird er als Endpointport erkannt, genügt der passende Endpoint; wird er als Nicht-Endpointport erkannt, was bei `BORDER_PORT_FORBIDDEN` der Regelfall ist, sind `ENDPOINT_A` **und** `ENDPOINT_B` Prämissen, weil jeder von beiden auf diesen Port gelegt werden könnte. `EMPTY` an einer Endpoint-Zelle liest `GRID_SIZE` und den Endpoint dieser Zelle. Verletzen mehrere Bedingungen denselben Wert, wird die mit den wenigsten Fakten gewählt (bei Gleichstand die erste in der Reihenfolge N, E, S, W); über mehrere entfernte Werte wird vereinigt. |
| `NEIGHBOR_PORT_REQUIRED`, `LOOP_PREVENTION` | `GRID_SIZE`. |
| `LOCAL_DEGREE_REQUIRED` | `GRID_SIZE`; zählt die Ports des Werts und liest deshalb für jeden Außenport zusätzlich die Endpointfakten wie oben. |
| `CONNECTIVITY_PRESERVATION` | `GRID_SIZE`, `ENDPOINT_A` (Start der Erreichbarkeit) und `ENDPOINT_B` (Ziel). |
| `LINE_ZERO`, `LINE_TARGET_REACHED` | Zeilen- bzw. Spaltensumme ihrer Linie (es zählen nur bereits belegte Zellen, die Linienlänge ist ohne Einfluss). |
| `LINE_ALL_REMAINING_OCCUPIED` | Summe ihrer Linie **und** `GRID_SIZE`: Die Zahl der noch belegbaren Zellen hängt von der Linienlänge ab. |

Reihenfolge der Fakten: `GRID_SIZE`, `ENDPOINT_A`, `ENDPOINT_B`, Zeilen-/Spaltensumme. `GRID_SIZE` wird überall deklariert, wo die Regel die Rastergeometrie liest. Das ist konservativ: Für Randseiten N und W sowie für Nachbarn, die eine Prämisse selbst benennt, wäre es nicht zwingend nötig. Eine zu weit gefasste Deklaration ist sicher, eine zu enge nicht; die Tests belegen die Notwendigkeit der Endpoint-, Summen- und `LINE_ALL`-Raster-Fakten und die Hinlänglichkeit aller Deklarationen. Der Auditor leitet die gelesenen Fakten aus den deklarierten Prämissen selbst ab und weist jede Abweichung (fehlender oder überzähliger Fakt) ab.

Die Spurprämissen sind minimal im Sinn von irredundant: Zuerst wählt der Tracer die früheren Einträge, dann bestimmt er die Fakten, die die Ableitung auf genau dieser Menge liest. Der Tracer prüft mit einer vom Regelcode abhängigen Ableitungsprüfung, ob die Regel die Entfernung allein aus den Konklusionen der gewählten früheren Einträge begründet, und löscht dann deterministisch vom jüngsten zum ältesten früheren Eintrag jeden Kandidaten, der entbehrlich ist. Kein verbleibender Spureintrag kann ohne Verlust der Ableitung entfernt werden. Gibt es mehrere irredundante Prämissenmengen, wählt diese feste Löschreihenfolge (jüngster zuerst, also ältere Einträge bevorzugt) genau eine und macht die Wahl reproduzierbar. Die Tiefe des Schritts folgt allein aus der gewählten Menge.

Kann der Tracer eine Root-Reduktion nicht aus ihrer Regel ableiten, ist die Spur nicht belegbar. Ein sonst eindeutiges Ergebnis wird dann `INDETERMINATE`, nie `UNIQUE` ohne geprüfte Spur (fail-closed). Dieser Fall ist ein interner Konsistenzfehler und trat in keinem Test auf.

### 13.4 Metriken

| Metrik | Berechnung im Code |
|---|---|
| `searchNodes` | Zahl der begonnenen DFS-Wertannahmen bis zum Lösungslimit zwei; Wurzel ausgeschlossen. |
| `deductionSteps` | Zahl der Spureinträge. |
| `maxDeductionDepth` | Maximum der Schritttiefen; 0 ohne Root-Schritt. Tiefe = 1 + Maximum der Tiefen der referenzierten früheren Einträge, 1 ohne solche; öffentliche Fakten zählen 0. |
| `requiredGuessDepth` | Zahl gleichzeitig aktiver Annahmen am ersten vollständigen Blatt (Rekursionstiefe der einzigen Lösung); 0, wenn die Wurzel die Lösung liefert. Verworfene Zweige, auch tiefere, zählen nicht. |

`deductionSteps` und `maxDeductionDepth` lassen sich allein aus `DeductionTrace` nachrechnen; die Tests tun das mit unabhängigem Code. Wo mehrere Suchzweige existieren, bleibt die Reihenfolge durch MRV (`y`, dann `x`) und die feste Wertreihenfolge deterministisch.

### 13.5 Kampagnengate

`CampaignGate.Evaluate(SolverResult?)` liefert `ELIGIBLE` nur für `UNIQUE` mit genau einer Lösung, vorhandenen Metriken und `requiredGuessDepth = 0`. `requiredGuessDepth > 0` ergibt `GUESS_DEPTH_REQUIRED`; `UNSATISFIABLE` und `MULTIPLE_OR_MORE` ergeben `NOT_UNIQUE`; `INDETERMINATE`, fehlende Metriken und `null` ergeben `INDETERMINATE`. Budget-, Zeit- und Cancellationabbrüche werden nie zu `UNIQUE`. Das Gate ist eine reine Solverprüfung: Es importiert nichts, erzeugt keinen Proof und trifft keine Freigabe.

### 13.6 Grenzen dieses Stands

- Es gibt weder `proof-v1`-Ausgabe noch Schemaänderung noch Strict-Validation. Ein Root-only-`solver-v2`-Ergebnis ist nicht als schema-valide, importierbar oder releasefähig zu behaupten (ADR-031 Abschnitt 2; `BLOCKER-PROD-001` bis `-003` bleiben unberührt).
- Die Wahl unter mehreren irredundanten Prämissenmengen (Abschnitt 13.3) ist eine Festlegung der Implementierung innerhalb von ADR-031 und wird von `solver-v2` nicht mehr geändert; ein späterer Eingriff erfordert `solver-v3`.
- Ein Leistungsbeleg liegt nur für die 10×10-Referenzfälle vor. Große Raster ohne Kuratierung können in der Suche weiterhin das Zeitbudget erreichen; das Ergebnis ist dann `INDETERMINATE`.

## Referenzen

[1]: ../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md "ADR-007 – Deterministischer Constraint-Solver"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Train Track Spiel – Schwierigkeit, Feldgrößen und Levelgenerierung"
[3]: ./LEVEL_DATA_FORMAT.md "Level Data Format v0.3"
[4]: ./PUZZLE_ENGINE.md "Puzzle Engine v0.3"
[5]: ../DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md "ADR-016 – Katalogverträge und Endless-Identität"
[6]: ../DECISIONS/ADR-019-endless-watermark-und-save-v2.md "ADR-019 – Endless-Watermark und Save v2"
[7]: ../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md "ADR-021 – Puzzleidentität und versionierte Proofartefakte"
[8]: ../DECISIONS/ADR-025-endless-open-lifecycle-und-claims.md "ADR-025 – Endless-Open-Lifecycle und Claims"
