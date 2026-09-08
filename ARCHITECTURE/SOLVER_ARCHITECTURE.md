# Solver Architecture v0.2

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

Der Content-Validator führt den Solver aus dem unveränderten öffentlichen Puzzleinput aus. Er vergleicht die einzige gefundene Lösung mit der Authoringlösung nach kanonischem Lösungshash. Der Nachweis enthält:

- `solverVersion`;
- `puzzleHashSha256`;
- `solutionHashSha256`;
- `solutionCount` bis zwei;
- Zahl der Suchknoten;
- Zahl erklärbarer Deduktionsschritte;
- maximale Deduktionskettentiefe;
- benötigte Annahmetiefe;
- `proofHashSha256`.

Gespeicherte Werte sind ein Cache. CI berechnet sie neu. Ein anderer Proof bei identischer Solver-Version und identischem Puzzlehash ist ein deterministischer Fehler.

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

Die stabile ID lautet `E1-<hex>`, wobei `<hex>` der vollständige kleingeschriebene SHA-256-Hexwert der JCS-Projektion `{endlessContractVersion,rulesetVersion,generatorVersion,seed,generationOrdinal,parameterHashSha256}` ist. Nach Generierung ergänzt der Runtime-/Diagnosedatensatz `puzzleHashSha256`, `solutionHashSha256`, `proofHashSha256`, Solverversion und Status.

Der Save reserviert `generationOrdinal` atomar vor Ausführung und erhöht `nextGenerationOrdinal`, sodass Crash/Retry keine zweite Identität erzeugt. Gleiche ID plus gleicher Deskriptor ist dieselbe Instanz. Gleiche ID mit anderem Deskriptor ist `GEN-ENDLESS-ID-COLLISION` und fatal. Vor Annahme wird gegen aktive IDs und terminale Ordinalintervalle geprüft; ein Duplikat ist weder eine neue Meldung noch rewardberechtigt.

Ein aktiver Entwurf speichert Deskriptor **und** öffentlichen Puzzleinput. Dadurch ist Wiederaufnahme möglich, auch wenn die Generatorbinary später nicht mehr enthalten ist. Seed, Generatorversion, Parameter und Proof erlauben Diagnose/Reproduktion mit archiviertem Tooling.

Die Lebensdauer ist explizit begrenzt:

- höchstens 20 aktive Endless-Deskriptoren, exakt gekoppelt an die globale Jüngsten-20-Draftgrenze;
- ein verdrängter Entwurf wird im selben Savecommit terminal `ABANDONED`, nie still wiederverwendet und ist nicht rewardberechtigt;
- jeder reservierte Ordinal ist entweder aktiv oder terminal; unbekannte Lücken sind Savekorruption;
- terminale Status-/Claimwahrheit liegt in maximal zusammengeführten, sortierten disjunkten Intervallen, insgesamt höchstens 64;
- nur die jüngsten 64 terminalen Instanzen behalten den vollständigen Reproduktionsdeskriptor; ältere Details gehen mit Ordinal, Status und Hash in die Endless-Checkpoint-Hashkette ein;
- bei einer nicht kompaktierbaren 20-/64-Grenze wird neue Generierung mit `ENDLESS_RETENTION_LIMIT` fail-closed verweigert, bis ein alter aktiver Entwurf bewusst abgeschlossen oder aufgegeben wurde.

Damit bleiben Resume und volle Diagnose für alle aktiven sowie die jüngsten 64 terminalen Instanzen erhalten. Für ältere Instanzen bleiben stabile ID-/Ordinal-/Status-/Claimwahrheit und manipulationsanzeigende Hashkette; eine vollständige lokale Reproduktion ist nach Ablauf dieses ausdrücklich definierten Diagnosefensters nicht garantiert. Der Save wächst bei Endlosnutzung nicht unbegrenzt.

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

Pflichtfixture sind mindestens: unlösbar, exakt eindeutig, technisch gültiger Ein-Zellen-Pfad, zwei Lösungen, Endpointfehler, Randzahlfehler, isolierte Schleife, getrennte Komponente, lange eindeutige Kette und gültige 10×10-Grenze. Metamorphic Tests rotieren/spiegeln Puzzle samt Endpoints und Counts; Lösungsklasse muss invariant bleiben. Solverproofs sind Golden Files pro Solverversion.

Ein kleiner unabhängiger Exhaustive Enumerator prüft alle sehr kleinen Rastersubräume in Tests gegen den Produktionssolver. Er wird nicht in Production ausgeliefert und reduziert das Risiko gemeinsamer Regelbugs.

## 12. Versionierung

`solver-v1` umfasst Constraintsemantik, Tiebreaker, Wertreihenfolge und Metrikdefinitionen. Performanceoptimierungen, die Proofreihenfolge oder Metriken ändern, erhöhen die Solverversion. Alle kuratierten Proofs werden dann bewusst regeneriert, diffgeprüft und erneut freigegeben.

## Referenzen

[1]: ../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md "ADR-007 – Deterministischer Constraint-Solver"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Train Track Spiel – Schwierigkeit, Feldgrößen und Levelgenerierung"
[3]: ./LEVEL_DATA_FORMAT.md "Level Data Format v1"
[4]: ./PUZZLE_ENGINE.md "Puzzle Engine v0.2"
[5]: ../DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md "ADR-016 – Katalogverträge und Endless-Identität"
