# ADR-043 – Getrennter strikter Endlos-Proofreader

## Status

**Angenommen** – Releaselead R am 21.10.2026 innerhalb WP046 nach getrennter C-Fachreview. Die technische Annahme erfolgt vor abhängiger Implementierung und bestätigt keine bereits erfolgte Human-/Produkt-/Endlosausgabe.

## Datum

2026-10-21

## Kontext

Die tatsächliche P05-Stichprobenvorbereitung erzeugte einen eindeutigen NoGuess-Kandidaten mit korrekter E1-Descriptoridentität. Der bestehende `ProofV1Codec.Create` lehnt dessen Puzzle-ID ab, weil sein Reader und sein Kampagnenschema ausschließlich die S-Kampagnen-IDgrammatik akzeptieren. `LocalEndlessPolicy.GenerateAsync` erzeugt bereits den vollständigen Proof-JCS-Payload und Hash aus echten Solverwerten unter E1. Der Kampagnenreader darf deshalb nicht still erweitert und ein Sample nicht auf eine erfundene S999-Kampagne umetikettiert werden.

## Entscheidung

1. Ein getrennter `EndlessProofV1Codec` und ein eigenständiger strikter Schemaentry akzeptieren ausschließlich `^E1-[0-9a-f]{64}$`. Der bestehende Kampagnenreader einschließlich Schema und S-IDprüfung bleibt unverändert geschlossen und lehnt E1 weiter ab. Der Endlosreader lehnt S-IDs, Großbuchstaben, falsche Länge und jedes Extra-/Missingfeld ab.
2. Beide Einstiege verwenden denselben geschlossenen Feld-, Profil-, Solver-, Metrik- und JCS-Prüfkern. Die acht vorhandenen Felder, `proofFormatVersion:1`, `solutionCount:1`, unterstützte Solverversionen und SafeInteger-Metriken bleiben identisch. `STP-PROOF-JCS-1`, `STP-SOLUTION-JCS-1`, `STP-PUZZLE-SEMANTIC-JCS-1`, Descriptor-/Parameterprojektionen und sämtliche Payload-/Hashbytes bleiben unverändert. Der neue Einstieg ist keine alternative Hashprojektion.
3. Ein explizites `ValidateAgainst` bindet den gelesenen Proof an die aus dem echten vollständigen Descriptor berechnete E1-ID, den tatsächlichen öffentlichen Puzzlehash und den tatsächlichen Lösungshash. Produkt-/Stichprobenverwendung verlangt zusätzlich `solver-v2`, Unique/NoGuess und frische Solververifikation des öffentlichen Inputs. Ein syntaktischer oder selbstgehashter Proof allein ist keine Eindeutigkeitsautorität.
4. `ProofV1Document` kann als bestehender unveränderlicher Datenträger geteilt werden. Ein enger interner Read-/Createkern erhält seine IDprüfung ausdrücklich vom jeweiligen Einstieg; kein öffentlicher beliebiger Predicate-/Bypassparameter. Die Kampagnen-Bindung bleibt kampagnenspezifisch. Application erhält keine Kante zu Infrastructure.Content.
5. Der Root-Offlinesampler verwendet den getrennten Endlosreader und die bestehenden tatsächlichen Generator-/Solver-/Kanonisierungsimplementierungen. Vollständige Descriptor-/Parameter-/Public-/Solution-/Prooforiginale bleiben im internen Audit; Teilnehmerassets enthalten nur die öffentlichen Inputs und exakte Quellenreferenzen gemäß ADR042. Die E1-ID ist eine unissued Quellenidentität, keine persistierte Reservation, Ausgabe, Freischaltung oder Economyhandlung.
6. Kein Save-/Claim-/Ledger-/Provider-/Humanstatus wird aus diesem Reader erzeugt. Produktionsprofile, Schwierigkeitsklassen und Sternzeiten werden dadurch nicht freigegeben. Keine Migration und keine Änderung historischer Level-/Proofdateien.

## Begründung

Der getrennte Einstieg schließt die reale E1-Validierungslücke und hält die etablierte Kampagnengrammatik eng. Gemeinsamer JCS-/Feldkern verhindert zwei abweichende Proofdefinitionen. Die erwartete Descriptor-/Public-/Lösungsbindung verhindert einen gültig selbstgehashten fremden Proof als Nachweis für einen anderen Kandidaten.

## Betrachtete Alternativen

- S999-IDs als Workaround verfälschen Quellenidentität und verdecken die Lücke.
- Den Kampagnenreader global auf beliebige IDs öffnen würde bestehende Negativgarantien schwächen.
- Eigener Proof-/Hashalgorithmus oder neue Profile würden unnötig sämtliche bestehenden Bindungen verändern.
- Die Stichprobe ohne strikten Proofreader weitergeben ließe den konkreten Befund offen.

## Konsequenzen

Ein enger neuer Reader-/Schemaentry und gezielte Tests entstehen. Tatsächliche Runtime-Hashgoldens bleiben bytegleich. Die neuen Belege bestätigen technische Kandidaten, keine menschliche Schwierigkeit oder tatsächliche Endlosausgabe.

## Betroffene Artefakte

`Infrastructure/Content/ProofV1.cs`, neuer `EndlessProofV1Codec`, eigenes Endlos-Proofschema, gezielte Contenttests und Root-Offlinesampler. Keine neue Assemblykante, kein Persistenzformatwechsel.

## Validierung

Getrennte C-Fachreview und prospektive R-Annahme erfolgt; beide Reader und der Sampler sind integriert. Privater C#-Lauf: 57/57 Fälle mit 977 Assertions einschließlich 40 unveränderter Kampagnenfälle, 16 E1-Fällen und tatsächlichem Runtimegolden. Die entsprechenden 56 Kampagnen-/E1-Readerfälle bestanden auch im tatsächlichen Unity-EditMode11. Geprüft sind getrennte IDräume, malformed/extra/missing/duplicate/profile/solver/metric/hashnegative sowie Descriptor-/Public-/Solutionmismatch.

C las den erhaltenen finalen P05-Pool unabhängig zurück: alle 577 Dateien bytegleich zu den eingefrorenen Originalen, 96/96 strikte E1-Proofbindungen und 96 frische UNIQUE/NoGuess-Solverläufe mit exakt passenden Metriken und geordneten Lösungsgeometrien. Die Auswahl enthält zwölf von 24 Kandidaten je Profil. Äußeres `sample-audit.internal.json`: SHA256 `48f58aeff9d45463f5c3bf98cfc35c7bc5d5cf57ce52111c1a2e82c3dbebd46a`; Originalreadback `.utmp/wp046-c-p05-readback/FINAL_READBACK.json`. Keine Generator-Replayattestation, Humanannahme oder issued/progress-/Produktpromotion daraus.

## Ersetzt / ersetzt durch

Ersetzt keinen ADR. Ergänzt ADR021/040 ausschließlich um den getrennten E1-Readerentry mit unveränderter Hashprojektion. ADR042 benutzt nur tatsächliche öffentliche Stichprobenquellen; seine Beobachtungs-/Produkttrennung bleibt unverändert.
