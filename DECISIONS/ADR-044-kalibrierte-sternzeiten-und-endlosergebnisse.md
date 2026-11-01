# ADR-044 – Kalibrierte Sternzeiten und begrenzte Endlosergebnisse

## Status

**Angenommen** – Releaselead R am 21.10.2026 innerhalb WP-046, nach getrennter C-Fachreview des finalen Entwurfs SHA256 `52cc4e937a4bd9bf837fcc83599cc78f834bb974a2dd541d7324d01e5600b8d8` und vor abhängiger Implementierung. Die technische Annahme bestätigt keine reale Kalibrierung, Quellenpromotion oder Produkt-/Geräteannahme.

## Datum

2026-10-21

## Kontext

P02/P05 verlangen echte 2-/3-Sternzeiten und die einmaligen Endlosboni +5/+10. Die Kampagne besitzt bereits `production.starThresholdsSeconds={twoStars,threeStars,calibrationVersion}`, `CompletionValues` und die atomare Stern-/Revisionspolicy. Der Candidateproducer verweigert derzeit nonnull-Werte bewusst. Endlos vergibt bisher nur +15 und zeigt nach Commit einen Stern; sein offener Completionrecord enthält weder tatsächliche Abschlusszeit noch Sterne. ADR040/041 begrenzen Reservation, offene Vorgänge und Ledgerbelege; keine davon darf zu einer unbegrenzten Ergebnishistorie werden. Die Technik wird jetzt vollständig ausführbar vorbereitet, während echte Werte und deren fachliche Annahme offen bleiben.

## Entscheidung

### 1. Vorhandene Verträge bleiben Autorität

Level-v2, `campaign-v2`, `completion-v1`, `cosmetics-v2`, `CompletionValues`, `CatalogUse`/`CatalogApprovalStatus`, `GeneratorQualityProfile`, Release-Lock-v1, die bestehenden Hashprofile und EconomyTerminal-/Ledgerformen werden weiterverwendet. Neue Reader dürfen nach tatsächlicher Gesamtprüfung Production konsumieren; bestehende Candidate-/Public240-Einstiege bleiben eng. `DRAFT` und `FIXTURE_ONLY` bleiben in Production verboten. Eine programmatisch konstruierte Zulässigkeitsantwort oder ein neues Boolean ersetzt niemals Sourceapproval, Proof-/Crossreference-/Qualitätsprüfung und den tatsächlichen Release-Lock. Kontrollierte positive Tests verwenden ausschließlich `ContractTest`/`FIXTURE_ONLY` und eigene Profile.

### 2. Eine gebundene Kalibrierquelle, keine zweite Preis-/Rewardquelle

Ein neuer geschlossener `star-calibration-v1`-Katalog hat genau `schemaVersion:1`, `catalogId`, `catalogRevision`, `approvalStatus`, `publicCatalog`, `provenance`, `campaign`, `endlessProfiles`. Katalogidentität und Hash verwenden die bestehenden vollständigen JCS-/Katalogregeln; kein neues Hashprofil. `publicCatalog` ist die vorhandene dreifeldrige Katalogreferenz. Alle Einträge und Versionen sind eindeutig, unbekannte/fehlende Felder werden abgewiesen.

- `provenance`: Einträge `{calibrationVersion,evidenceSha256,uiSourceSha256}`. Sie referenzieren den intern erhaltenen kanonischen Methoden-/Quellen-/Übertragungsnachweis und die tatsächlich gemessene UIquelle; keine Personen-/Originalexportdaten kommen in den Player. Der interne Nachweis bindet erhaltene Originalexporthashes, Ausschlüsse, Logikklassen, die nachvollziehbare Zuordnung auf ungemessene Level und die tatsächliche unabhängige fachliche Annahme. Er ordnet die tatsächlichen ursprünglichen Manifest-/Dokument-/Profil-/Publickorpus-/UIhashes ausdrücklich den Zielquellen zu: allein das Ergänzen von Kalibriermetadaten/Approval ändert bereits volle Dokument-/Profil-/Kataloghashes, deshalb sind unveränderte öffentliche Geometrie und konkrete Such-/Qualitätsbedingungen einzeln nachzuweisen; Label-/Rastergleichheit genügt nicht. Eine geänderte Spieloberfläche oder Suchbedingung wird nicht still als gemessene Fassung behandelt. Ein lokaler Teilnehmer-Selbstbericht oder Readback-PASS ist diese Annahme nicht. Der Releaseproducer prüft den tatsächlichen internen Nachweis; Runtime liest nur dessen gehashte Referenz. Neue Herkunft erfordert neue Version/Revision und einen neuen Lock.
- `campaign`: genau ein Eintrag je tatsächlicher Campaignidentity `{identity,sourceDocumentSha256,calibrationVersion,twoStarsSeconds,threeStarsSeconds}`. `identity` verwendet die vorhandene Form `{puzzleId,profile,sha256}`. Dokumenthash ist der bestehende volle Level-JCS-Hash; Version und Sekunden müssen exakt dem gelesenen Level-v2-Datensatz entsprechen. Fehlende, doppelte oder fremde Membership scheitert. Produktionsumfang ist vollständig 5/20/240, nicht nur die gemessene Teilmenge.
- `endlessProfiles`: je benanntem Profil ein Eintrag `{profileId,profileHashSha256,calibrationVersion,twoStarsSeconds,threeStarsSeconds}`. Er bindet den vollständigen konkreten GeneratorQualityProfile-Quellhash einschließlich Such-/Metrik-/Novelty-/Versions-/Humanreviewvertrag. Das aktuelle DRAFT-Suchprofil wird nicht durch Umbenennen freigegeben. Qualitätsprofil und interner Kalibriernachweis referenzieren nicht den daraus erzeugten Kalibrierkataloghash; die Hashabhängigkeiten bleiben azyklisch.

Alle Sekunden sind positive SafeInteger; zusätzlich gilt `seconds <= floor(9007199254740991/1000)` und strikt `threeStarsSeconds < twoStarsSeconds`. Die einzige Abbildung ist ganzzahlig `checked(seconds * 1000)`, zusätzlich innerhalb SafeInteger. Keine Rundung, Fließkommazahl, Sättigung oder Ersatzzeit. Gültiger Abschluss bis einschließlich Dreisterngrenze gibt drei Sterne, bis einschließlich Zweisterngrenze zwei, sonst einen. Hinweise/Marker ändern diese Zeitsterne nicht; die vorhandene gesonderte Direkt-gelöst-Regel bleibt erhalten. Aktive Domainzeit ist die Spielautorität. Welche Human-/Previewbeobachtungen fachlich welche Zeitklassen tragen, entscheidet der interne tatsächliche Kalibriernachweis, nicht dieser Rechenweg.

### 3. Exakt gehashte Runtimeprojektion und Productionimport

Ein getrennter `completion-runtime-v1`-Wrapper hat genau `{runtimeCompletionContractVersion:1,completionCatalog,calibrationCatalog}`. Seine beiden inneren Dokumente folgen unverändert `completion-v1` beziehungsweise Entscheidung2. Seine **vollständigen** JCS-Bytes definieren die Completion-`CatalogIdentity`; Katalog-ID/Revision/Approval werden aus `completionCatalog` übernommen. Die neue Runtimekataloglinie verwendet eigene IDs beziehungsweise eine ausdrücklich neue Revision; keine bereits gebundene ID/Revision erhält einen anderen Hash. Das innere Completiondokument ist hier Bestandteil dieser einen Runtimequelle, keine zweite gleichnamige Runtimeidentität mit anderem Hash. Damit bindet die vorhandene Save-/Terminal-/CatalogSet-Referenz auch die tatsächlich verwendeten Schwellen und Herkunft. Es gibt getrennte Campaign- und ENDLESS_INSTANCE-Completionquellen; kein LEVEL→ENDLESS-Umdeuten. Rewardbeträge kommen ausschließlich aus der jeweiligen Completionquelle (Campaign 15/5/10/60/150, Endlos 15/5/10, getrennt optional +10). Der Kalibrierkatalog enthält keine Rewardbeträge.

Der Release-Lock-v1 nimmt die vollständigen Public-/Campaign-/Completionwrapper-/Cosmetics-/Kalibrier-/Qualitätsprofilhashes unter seinen vorhandenen `catalogHashes` auf; jede tatsächliche Level-/Proofbindung bleibt im bestehenden `puzzles`-Teil. Der Producer liest die echten typisierten Level-/Proof-/Quellverträge, prüft Herkunft, alle Asset-/Text-/Graphreferenzen, nonnull-Schwellen und Status vor Ausgabe. Der Player erhält öffentliche Inputs, Runtimewerte und gelockte Referenzen, keine Authoringsolutions oder Humanoriginale. Ein neuer Produktionsreader liefert die bestehenden Applicationports. Historische Candidatewrapper und ihre Hashbytes ändern sich nicht. Ein gelungener technischer Import ist kein Store-/Geräte-/Human-PASS.

### 4. Endlos pinnt Zeit- und Rewardquelle vor der Reservation

Für kalibrierte Endlosreservierungen wird die geschlossene `SearchGeneratorParameters.contractVersion:2` eingeführt. Sie enthält alle unveränderten ADR040-Felder plus genau `starCalibration:{completionCatalog,calibrationCatalog,calibrationVersion,twoStarMs,threeStarMs}`. Beide Katalogreferenzen verwenden die vorhandene dreifeldrige Form. Profil-ID/-hash stehen bereits in den Parametern; der Reader prüft die eindeutige zugehörige Kalibrierzeile und deren exakte Ms-Abbildung sowie den vollständigen Completionwrapper. Keine fremde aktuelle Quelle darf fehlende alte Quellen ersetzen.

`parameterHashSha256` bleibt SHA256 über die vollständigen tatsächlichen JCS-Parameterbytes. Die bestehende E1-Descriptorprojektion, Generator-/PRNG-/Solverregeln und Puzzle-/Solution-/Proofhashprofile bleiben unverändert. Die zusätzliche ausdrücklich versionierte Parameterbindung erzeugt folgerichtig eine andere neue E1-ID. Reserve persistiert alle Bindungen vor Generierung. Completion und Postclear verwenden ausschließlich die so gepinnte exakte Completionquelle. Retainedreader benötigen deren tatsächliche unveränderte Quellen; fehlende/abweichende Quellen führen zu Unsupported/Recovery mit Quellenerhalt, nie zu anderen Zeiten oder Beträgen.

Jede v2-Promotion prüft das tatsächlich verifizierte Kandidatenergebnis gegen **alle** gebundenen GeneratorQualityProfile-Intervalle einschließlich Maximalmetriken, Novelty sowie Algorithmus-/Solver-/Rulesetversionen; die zwölf bisherigen Suchfelder oder ein approved-Import allein genügen nicht. Eine unbekannte oder nicht umsetzbare Profilanforderung stoppt fail-closed ohne Ausgabe oder Ersatzprofil; ein bekannter Kandidaten-/Budgetfehlschlag erhält die bestätigte Reservation nach ADR040 ohne erfundenen Save-Recoveryausgang.

`contractVersion:1` wird unverändert gelesen/geschrieben. Bereits ausgegebene interne v1-Instanzen bleiben ohne nachträgliches Rebinding oder nachgezahlte Zeitsterne bei ihrer bisherigen Semantik. Production kann ausschließlich vollständig freigegebene v2-Reservierungen erzeugen. Die alte Contracttestgrammatik bleibt unverändert; technische v2-Fixtures erhalten eigene ausdrücklich gebundene Testquellen.

### 5. Ein begrenztes dauerhaftes Abschlussergebnis

Der kalibrierte `OpenEndlessCompletion` erhält `recordVersion:2` mit allen bisherigen Feldern plus genau `result:{identity,attemptId,activeElapsedMs,stars,firstClearTerminal,secondStarPaid,thirdStarPaid}`. `identity` ist die tatsächliche verifizierte öffentliche E1-Identity, `attemptId` der wirkliche ENDLESS-Domainattempt; `completionCommitId` bleibt exakt `puzzle-solved:<attemptId>`. Die tatsächliche endgültige aktive Domainzeit ist nichtnegativer SafeInteger. Sterne werden aus dieser Zeit und den gepinnten Grenzen erneut berechnet. Kein Command oder Callback liefert eine Sternzahl.

Die drei Terminalfelder verwenden ausschließlich den bestehenden EconomyTerminal-Vertrag: +15 immer, +5 genau bei mindestens zwei Sternen, +10 genau bei drei Sternen, sonst jeweilig null. Die vorhandenen exakten Transaktions-/Claim-/Reason-/E1-IDs bleiben unverändert; Quelle, Betrag, UTC und CommittedGeneration sind innerhalb desselben Abschlusses konsistent. Ein gelöster echter verifizierter Domainzustand wird im **einen** gemeinsamen Writer atomar ersetzt: Ergebnis plus offene Bonusclaimfortsetzung, alle zulässigen Terminals, Ledgerdeltas, ADR041-Receipts und Hintretirement. Erst bestätigtes Repositoryack erlaubt Fahrt/Ergebnis. Replay desselben Abschlusses verändert nichts; anderes Attempt-/Source-/Generationbinding scheitert. Es gibt keinen nachträglichen Stern-Upgrader und keine Endlosrevision.

Die Ergebnis-Terminals bleiben nur solange der höchstens20fach begrenzte offene Completionrecord besteht. Sie sind konsistente Ergebnisbelege, keine neue Eligibility- oder Ledgerkompaktierungsautorität. ADR041-Receipts bleiben allein an tatsächliche Journal-/Tailreferenzen gebunden (höchstens576), werden vor neuen Grants gemeinsam kompaktiert/geprunt und mit den Ergebnis-Terminals exakt gegengeprüft, solange referenziert. Nach Compaction bleiben die höchstens drei Ergebnis-Terminals je offenem Abschluss für präzisen Reload erhalten; nach Skip beziehungsweise bestätigtem Bonus werden sie mit dem offenen Record entfernt. Erhaltene Ledgerreferenzen bleiben weiterhin durch ADR041-Receipts gedeckt. Kein terminales Archiv entsteht.

`CurrentEndlessStars`/Resultreadmodel stammen nach Ack und nach Reload aus dem tatsächlich gebundenen Ergebnis, nicht aus einer Konstanten, Receiptanwesenheit oder Watermark. Ein offener Abschluss ist kein aktives Puzzle. Nach Claimretirement darf eine bereits angezeigte unveränderliche flüchtige Ergebnisansicht bleiben; nach Neustart wird daraus keine terminale Historie rekonstruiert. Fehlendes v1-Ergebnis wird nicht als tatsächliche Zeit oder höhere Sterne erfunden; seine bisherige Legacyanzeige bleibt explizit getrennt und löst keinen Grant aus.

### 6. Versions- und Consumergrenzen

SaveSchema3 und `STP-SAVE-JCS-1` bleiben erhalten; nur der neue Completiondetailrecord und die Searchparameter haben explizit neue Versionen. Der umgebende ADR040/041-Endlosrecord2, 20er-Komplement, UInt64-Watermark, 50er-Noveltyfenster und 576er-Receiptgrenze bleiben unverändert. v1-Detailrecords und alte Goldens bleiben bytegleich les-/schreibbar. Unbekannte Versionen werden erhalten und unsupported gestoppt. Kein stiller Downgrade, kein v2-Writerverlust und keine QA-/Human-/ContractTest→Productionmigration. Production benötigt eigene tatsächliche CatalogUse-/Namespace-/Application-/Providerkonfiguration; eine neue technische Factory ist keine Freigabe dieser Konfiguration.

## Begründung

Der vollständige Completionhash bindet Kampagnenzeiten ohne neue Kampagnenprogressfelder. Endlos benötigt dagegen vor der Ausgabe gepinnte Schwellen und nach dem Abschluss ein dauerhaftes, begrenztes Ergebnis, weil der aktive Resumezustand absichtlich entfernt wird. Bestehende Terminal- und Retentionsformen tragen die echten Boni, ohne ein zweites Punktekonto oder wachsende Terminalhistorie.

## Betrachtete Alternativen

- Nur den bisherigen nonnull-Throw entfernen: Herkunft, Umrechnungsgrenzen und Produktionsimport bleiben ungesichert.
- Jeweils aktuelle Profilzeiten beim Abschluss/Reload lesen: ändert bereits ausgegebene Instanzen nachträglich.
- Sterne allein aus +5/+10-Receipts ableiten: deren zulässiges Pruning verliert das Ergebnis; sie sind keine Lifecycleautorität.
- Jeden Endlosabschluss in `Levels` speichern: erzeugt unbegrenzte Historie und falsche Kampagnenmembership.
- DRAFTwerte als Productionetikett oder automatisierte Zeiten verwenden: verletzt das ausdrückliche Mandat.

## Konsequenzen

Die vollständige Technik kann mit klaren synthetischen ContractTestquellen implementiert und negativ geprüft werden. Reale Quellen bleiben bis zur tatsächlichen Kalibrierung DRAFT/null; kein Dummywert wird in Spielerassets, Profilquellen oder Freigaben eingetragen. Kein Providervertrag und keine Humanbeobachtungsdatei ändert sich.

## Betroffene Artefakte

| Eigentum nach Annahme | Konkrete Umsetzung |
|---|---|
| R | Registry/Fachquellverträge, neue Schemas und Productionproducer unter `tools/campaign`, Release-Lock-/Sourceapprovalprüfung, echte spätere Quellen/Build-/Bootstrap-/Providercomposition. |
| B | Neue reine Calibration-/Runtimecompletionreader und Applicationwerte/Policies; `SearchGeneratorParameters`, `LocalEndlessPolicy`, `LocalProfileState`, gezielte `LocalSaveEndlessCodec`-/Retainedreadererweiterung, `PersistedEndlessSession`-/Saved-Resultprojektion, atomare Stern-/Replay-/Reloadtests. Bestehende Candidatepfade getrennt erhalten. |
| A | Falls erforderlich ausschließlich Darstellung der tatsächlich gelieferten Resultwerte; keine eigene Sternberechnung/Quellenfreigabe. |
| C | Getrennte Fachreview des Vertrags und der tatsächlichen Atomic-/Retained-/Compaction-/Consumergrenzen; Provider APIs bleiben unverändert. |

R/B vereinbaren die kleinen neuen Reader-/Producerformen vor parallelem Schreiben; Root besitzt weiter Produktionserzeugung und Composition. Diese Aufteilung erzeugt kein neues Work Package.

## Validierung

Getrennte C-Review und prospektive R-Annahme/Registry erfolgt. Folgende Implementierungsnachweise stehen noch aus: kontrollierte Sekunden→Ms-Grenzen einschließlich exakter Gleichheit, Null/Teilpaar/Ordnung/Overflow/Fließkomma, falsche Version/UI-/Dokument-/Identity-/Profil-/Kalibrier-/Wrapper-/Lockhashes, fehlende Herkunft und strikte DRAFT-/FIXTURE-/Productiontrennung. Campaign: vollständige240er-Zuordnung, Erstlauf/PRACTICE/REVISION und einmalige Sternboni bleiben korrekt. Endlos: echte Domainlösungen für1/2/3Sterne, genau15/20/30Basisdelta, Hint-/Markerneutralität, Commit vor Fahrt/Result, Crash/CAS/unknownOutcome, Replay vor/nach Reload, Quellenwechsel zwischen Reserve/Generate/Complete/Reload, Bonus/Skip und späte Callbacks, 512/64-Compaction mit Ergebnis-/Receiptdeckung,20/50/576-Grenzen und mindestens10.000 Wechsel ohne Historienwachstum. Historische QA-/v1-/v2→v3-Goldens bleiben unverändert. Tatsächliche Humanwerte, Klassenübertragung, Quellenannahme und Geräteprüfung sind weiterhin reale gesonderte Befunde.

## Ersetzt / ersetzt durch

Ersetzt keinen bestehenden ADR. Ergänzt ADR016/019/021/025/037/040/041 um die explizite Kalibrierquelle, versionierte Parameterbindung und begrenzte Ergebnisdetails; erlaubt deren tatsächlich freigegebenen Productionverbrauch unter den bestehenden übrigen Import-/Providergrenzen. ADR039-DRAFTisolation, ADR042-Beobachtungstrennung und ADR043-Proof-/Identitätsverträge bleiben erhalten. Wird derzeit durch keinen ADR ersetzt.

## Geschlossene Profil-/Importdetails

Nach getrennter C-Fachreview des Nachtrags SHA256 `33eab5ee8c7b7350308948ddc2de5cc80bb6a8cbdcfb97e376a0c5d9c22e0b03` von R am 21.10.2026 vor abhängiger Implementierung angenommen. Die folgenden Formen konkretisieren denselben Vertrag; noch keine tatsächliche Kalibrierung oder Productionpromotion.

## Geschlossenes GeneratorQualityProfile v1

Jedes Profil ist eine eigene vollständige JCS-Datei **ohne Selfhashfeld**; ihr vollständiger SHA256 steht im Release-Lock und in der Kalibrierzeile. Exakt diese 15 Topfelder:

| Feld | Geschlossene Form |
|---|---|
| `schemaVersion` | Integer1 |
| `profileId` | einer der vier bestehenden IDs `dienstbeginn`, `regelbetrieb`, `stoerungslage`, `grosslage` |
| `profileVersion` | positiver Integer, revisionsgebunden |
| `rulesetVersion` | `train-track-v1` |
| `approvalStatus` | bestehende Quellenstatuswerte; Production ausschließlich `PRODUCT_APPROVED`, isolierter ContractTest ausschließlich `FIXTURE_ONLY`, keine Umdeutung von DRAFT |
| `difficultyBand` | exakt dieselbe benannte Profil-ID, keine Ableitung aus Rastergröße |
| `generatorVersion` | `path-search-v1` |
| `solverVersion` | `solver-v2` |
| `gridRanges` | genau `{minSize,maxSize,minDensityPercent,maxDensityPercent}`; bekannte Suchgrenzen, benannte vier Größenbereiche wie bisher, tatsächliche Kandidatendichte wird geprüft |
| `metricRanges` | genau `{searchNodes,deductionSteps,maxDeductionDepth,requiredGuessDepth,trivialLines}`; jeder Wert genau `{min,max}`, nichtnegative SafeInteger, min<=max; requiredGuessDepth zwingend0/0 |
| `runtimeBudgets` | genau `{maxAttempts,pathStateBudget,totalBudgetMs,solverBudgetMs,solverStateBudget,hintBudgetMs,hintStateBudget,timeoutPolicy}`. Bekannte Suchgrenzen; aktuelle Engine unterstützt explizit solverStateBudget=1000000, hintBudgetMs=100, hintStateBudget=100000. Andere Werte bleiben unsupported statt ignoriert. timeoutPolicy exakt `ABORT_ATTEMPT_KEEP_RESERVATION`; für Hinttimeout kein Hint/Verbrauch. |
| `novelty` | genau `{algorithmVersion,referenceCorpus,recentWindow,recentSimilarityPercent}`; algorithmVersion=`public-d4-l1-v1`, referenceCorpus bestehende dreifeldrige exakte Publickatalogreferenz, recentWindow=50, Prozent0..100. Algorithmus bedeutet D4/A-B-Exactschutz gegen Corpus+Recent und den bestehenden inklusiven Similarityreject gegen Recent; keine andere Distanzinterpretation. |
| `humanReview` | genau `{calibrationVersion,evidenceSha256,uiSourceSha256,sampleCount,criteriaVersion}`. Quelle/Version stimmen exakt zur Kalibrierprovenienz; Production mindestens12 tatsächlich belegte Fälle je Profil, keine Selbstattestierung aus Zahl/Status. |
| `approvedBy` | nichtleerer Referenzstring auf die tatsächliche verantwortliche Rolle; keine automatische Agentenfreigabe |
| `approvedDecisionRef` | nichtleere tatsächliche Annahmereferenz; Producer prüft Originalnachweis, Runtime nur gelockte Quelle |

Extra-/Missing-/Duplicatefelder scheitern auf jeder Ebene. Die vorhandenen zwölf Suchfelder werden eindeutig abgeleitet: vier Gridfelder, min von deductionSteps/maxDeductionDepth, maxAttempts/pathStateBudget/totalBudgetMs/solverBudgetMs, max von trivialLines, recentSimilarityPercent. Die verbleibenden Min/Max werden vollständig vor Promotion am echten Solverresult/öffentlichen Input geprüft. Damit bleiben keine ungenutzten unterstützten Profilfelder.

Das Profil ist weder der alte DRAFTcontainer `path-search-v1.profiles.json` noch eine neue Interpretation seiner Selfhashprojektion. Alte v1-Parameter und Profile bleiben unverändert. Positive Technikfixtures sind ausdrücklich synthetische FIXTURE_ONLY-Quellen in eigenen ContractTestprofilen; keine Produktdatei wird damit erzeugt.

### Geschlossener neuer Publicruntimepayload

Exakt `{schemaVersion:1,catalogId,revision,use,status,puzzles,catalogHashSha256}`; puzzle-Einträge exakt die bestehende Siebenfeldform `{puzzleId,rulesetVersion,grid,endpoints,rowCounts,columnCounts,publicPuzzleHash}`. Neue Reader nur `use:Production/status:PRODUCT_APPROVED` oder isoliert `ContractTest/FIXTURE_ONLY`; vorhandene Candidate-/Reviewreader unverändert. Eigene zulässige Kataloglinie, kein qa-prefix in Production. Vollständige aktuelle 240 S1-Identitäten in 5/20/12-Reihenfolge. Selfhash wie bestehender Publicpayload: JCS des vollständigen Topobjekts **ohne nur** `catalogHashSha256`, zusätzlich exakter injizierter Release-Lockhash. Semantische Puzzlehashprojektion bleibt unverändert. Der Reader prüft Datengrammatik/Hashes und ist keine alleinige Proof-/Sourceapproval-/Releaseautorität; R-Import muss deren tatsächliche Prüfung vor Composition belegen.

### Vereinbarte Reader-/Producergrenzen nach Annahme

Neue Klassen unter `Infrastructure.Content`, keine Content→Persistencekante:

```csharp
RuntimePublicPuzzleCatalog(byte[] source, CatalogIdentity expected);
// : IPublicPuzzleCatalog; expected kommt aus tatsächlichem Release-/Fixturelock.

StarCalibrationCatalog(byte[] source, IPublicPuzzleCatalog corpus,
    CatalogIdentity expected, ICanonicalJson json);
// Identity; Campaign/EndlessEntries; GetCampaign(identity), GetEndless(profileId, profileHash).
// Strikte 240er-Membership, Version/Herkunft, exakte integer Seconds→Ms.

RuntimeCompletionCatalog(byte[] wrapper, IPublicPuzzleCatalog corpus,
    CatalogIdentity expected, ICanonicalJson json);
// : ILockedCompletionCatalog für Campaign; Identity, Calibration, SubjectKind, RewardAmounts.
// Campaign- und ENDLESS_INSTANCE-Wrapper bleiben getrennte Katalogidentitäten.

LockedCalibratedEndlessProfile(byte[] source, IPublicPuzzleCatalog corpus,
    RuntimeCompletionCatalog completion, string expectedProfileHashSha256);
// Profile : CalibratedEndlessProfile (Application DTO): Id/Hash/Version/Search/Quality/StarCalibration.
// Die vier Einzeldateien werden durch R gegen vollständigen Lock/Namedset zusammengesetzt.

RuntimeProgressCatalog(IPublicPuzzleCatalog corpus, byte[] campaign,
    CatalogIdentity expectedCampaign, RuntimeCompletionCatalog completion, ICanonicalJson json);
// Campaign : ILockedCampaignCatalog; Completion; CatalogSet über vorhandene Create-Projektion.
```

`CatalogIdentity` trägt wie bisher Use/Approval im CLR-Typ; persistierte dreifeldrige Referenzen werden nur im explizit gebundenen Consumer rekonstruiert. Die Rohformen `star-calibration-v1`/`completion-runtime-v1` sind exakt ADR044, keine impliziten Zusatzfelder. Campaignwerte werden aus Kalibrierzeilen in bestehende `CompletionValues` projiziert, Belohnungswerte aus der Completionquelle. Der Rootproducer bindet jede Zeile zusätzlich an den tatsächlichen typisierten vollständigen Level-v2-Datensatz und den tatsächlichen Provenienznachweis; Runtime benötigt keine Authoringsolution.

Für Rootsuche optional am Ende der bisherigen Signatur eine zusätzliche unveränderliche `EndlessQualityBounds? quality = null`. Deren `Accepts(SolverMetrics, trivialLines)` prüft alle fünf geschlossenen Bereiche; Root prüft sie deterministisch vor Kandidatenannahme. Bestehende Aufrufe ohne quality bleiben unverändert. Productionpolicy muss dieses exakte Boundobjekt aus dem gepinnten Profil verwenden und vor Promote erneut prüfen; kein frei konfigurierbarer UI-Predicate und kein Zeitbudget-bedingtes Überspringen. B liefert den DTO, R besitzt Änderung an PuzzleCandidateSearch.

Persistence erhält eine neue geschlossene `RetainedCalibratedEndlessBinding.FromDocuments(...)` über tatsächliche gelockte Wrapper-/Profilbytes und den gebundenen Publickorpus; keine Registrierung allein durch frei konstruierte Policy-DTOnamen. Ihr Factorypfad ergänzt bestehende Retainedbindings, ersetzt keine historischen Quellen. Die konkreten DTO-/Methodennamen sind Implementierungsdetails; die obigen Daten-/Hash-/Consumerformen bleiben verbindlich.

## Geschlossene interne Importnachweise

Nach getrennter C-Fachreview der geschlossenen Fassung SHA256 `d08f37ba4efd87ce630c373e6178ff8e94bab4f87541e0974a22c5b5c8a9f582` von R am21.10.2026 vor abhängiger Freigabeimplementierung angenommen. Präzisiert denselben ADR044-Sourceapproval-/Herkunftsvertrag, ohne neue Runtime-/Save-/Hashform oder tatsächliche Quellenfreigabe. Die folgenden technischen Regeln sind verbindlich; die Formannahme erzeugt keine Human-/Produktannahme.

### ADR044 interner Importnachweis – prospektiver geschlossener Vorschlag

Nach getrennter C-Fachreview prospektiv von R angenommen; keine tatsächliche Kalibrierung oder Freigabe. Bestehende Player-/Hash-/Saveformen ändern sich nicht.

Alle folgenden internen JSON-Dateien sind streng geschlossene integer-JCS-Dokumente mit eindeutigen Keys. Dateiverweise haben exakt `{path,rawSha256}` (repositoryrelativ, Forwardslash, keine Links/Traversal). Die äußere geschützte Releasekonfiguration injiziert ApprovalLock-Pfad und erwarteten **Raw**-SHA separat vom Importrequest. Ein Request kann sich keine vertrauenswürdige Approvalquelle geben.

`Calibration.provenance.evidenceSha256` und `GQP.humanReview.evidenceSha256` binden ausdrücklich SHA256 der **vollständigen bestehenden JCS-Kanonisierung** des passenden Evidence-v1-Objekts. ApprovalLock-Dateiverweise prüfen zusätzlich die unveränderten Originalbytes mit `rawSha256`. Beide werden unabhängig geprüft; Prettybytes dürfen verschieden sein, Raw und JCS werden niemals implizit gleichgesetzt. Dasselbe gilt für die bereits bestehenden JCS-Level-/Profil-/Kataloghashes gegenüber Import-Rawquellbindungen.

#### Evidence v1

Exakt `{evidenceVersion:1,kind:"STAR_CALIBRATION_EVIDENCE",use,calibrationVersion,uiSourceSha256,method,observations,exclusions,logicClasses,campaignTransfers,profileTransfers,acceptance}`.

- `use` nur `Production` oder `ContractTest`; Fixtures sind zusätzlich durch synthetische Quellen/Annahme gekennzeichnet und niemals Production.
- `method` genau `{source,version}`; source ist ein Dateiverweis auf den erhaltenen tatsächlichen Methoden-/Auswertungsnachweis.
- `observations` Liste `{id,sourceRoot,packs,publicCatalog,publicBindings,sourceBindings,exports,includedExposureIds}`. sourceRoot ist der repositoryrelative erhaltene alte Quellsnapshot; packs/publicCatalog/publicBindings/sourceBindings sind relative Pfade darin. exports ist eine Liste gehashter Dateiverweise relativ zum Importroot. Der bestehende `HumanReadback.SourceBundle` und ADR042-Assignment-/Observationcodec validieren die Originale gegen diesen tatsächlichen Snapshot einschließlich UI-Dateiset/Raw-SHAs, alter Level-/Profil-/Parameter-/Manifestbindung. Alle so gelesenen Originale werden zusätzlich in die Import-Rawquellliste aufgenommen. includedExposureIds benennt tatsächliche Exposure-IDs, nicht Personen. Production inkludiert nur vorhandene `PARTICIPANT_UI`-Originale ohne Ausschlussgrund des bestehenden Readbacks. Readback ist keine fachliche Annahme.
- `exclusions` Liste `{observationId,exposureId,reason}`. Jede vorhandene Trial wird genau einmal inkludiert oder mit konkretem Grund ausgeschlossen. Keine stillen Auslassungen; doppelte Exportoriginale/Exposure-IDs dürfen keine Stichprobe aufblasen.
- `logicClasses` Liste `{id,criteria,transferMethod}`; criteria und transferMethod sind gehashte Dateiverweise. Inhalt trägt tatsächliche Logikklasse und nachvollziehbare Übertragungsmethode, nicht bloße Raster-/Labelklassen.
- `campaignTransfers` genau240 Zeilen `{puzzleId,sourceDocument,targetDocumentSha256,publicPuzzleHash,logicClassId,observationIds}`. sourceDocument bindet das **alte gemessene/übertragene** volle Leveldokument, targetDocumentSha256 das neue Ziel-Level-JCS. Beide typisierten Quellen müssen exakt dieselbe öffentliche Puzzleprojektion besitzen. Die öffentliche Identity wird vollständig gebunden. Die Zielschwellen entsprechen exakt dem Kalibrierkatalog. Tatsächlich gemessene Quellen und ungemessene Übertragungsziele bleiben unterscheidbar.
- `profileTransfers` genau4 Zeilen `{profileId,sourceProfiles,sourcePublicCatalog,sourcePublicBindings,targetConditionsSha256,logicClassId,observationIds}`. Die drei Sourcefelder sind gehashte Dateiverweise auf die tatsächlichen alten DRAFT-Profil-/Public-/Bindingsoriginale. Der bestehende LockedEndlessProfiles-Reader prüft den ursprünglichen Selfhash/Korpus; alle zwölf daraus gelesenen Suchparameter müssen exakt den zwölf abgeleiteten Ziel-GQP-Suchparametern entsprechen, ebenso Generator-/Solver-/Noveltyversion. Alter und Zielpublickorpus müssen alle240 identischen semantischen Identitäten enthalten. targetConditionsSha256 bindet ausschließlich die geschlossene Projektion aus `rulesetVersion,difficultyBand,generatorVersion,solverVersion,gridRanges,metricRanges,runtimeBudgets,novelty`; weder Humanreview/Approval noch Kalibrierkataloghash. Die neuen vollständigen Min-/Max-Grenzen werden außerdem auf jedem einbezogenen tatsächlichen öffentlichen Fall durch einen frischen Solverlauf geprüft; sie werden nicht als ursprünglich vorhanden ausgegeben. Die gehashte fachliche Transfermethode nimmt diese ausdrückliche Erweiterung an. Mindestens12 unterschiedliche tatsächlich inkludierte Profilfälle je Profil und exakte ursprüngliche Profilhashbindung pro Originalfall; volle ursprüngliche und neue Zielprofilhashes werden nie gleichgesetzt.
- `acceptance` genau `{source,acceptedBy,decisionRef}`. Gehashte tatsächliche unabhängige fachliche Annahme des gesamten Methoden-/Quellen-/Transfernachweises, Rolle und Referenz stimmen mit geschütztem ApprovalLock überein. Keine Agenten-/Readback-/Selbstberichtableitung.

#### ApprovalLock v1

Exakt `{approvalLockVersion:1,kind:"PRODUCTION_SOURCE_APPROVAL_LOCK",use,releaseLock,evidence,sources,acceptedBy,decisionRef,acceptance}`.

`releaseLock` und `acceptance` sind Dateiverweise; `evidence` ist Liste der exakten Evidence-Dateiverweise. `sources` ist die ordinal nach Pfad sortierte exakte Liste aller geprüften Importquellen `{path,rawSha256}` einschließlich Level-/Proof-/Katalog-/RailwayAssets-/Lokalisation-/UIlock-/Toolchainoriginalen, ohne ApprovalLock selbst und ohne generierte Ausgaben. Keine Zusatz-/fehlende Quelle. `acceptance` ist das erhaltene unabhängige Annahmeoriginal; `acceptedBy/decisionRef` müssen dessen tatsächlichem Freigabekontext entsprechen. Der separat injizierte erwartete Rawhash bindet diese Originalquelle an die geschützte Releasekonfiguration; er ist allein keine menschliche Attestation.

Abhängigkeiten sind azyklisch: alte Originale/Methoden/Kriterien → Evidence (Ziel-Levelhash und Targetconditions, **kein** voller Zielprofil-/Kalibrier-/Wrapperhash) → GQP.humanReview → Kalibrierkatalog → Completionwrapper → ReleaseLock → ApprovalLock. Evidence/Methoden/Annahme enthalten keine daraus resultierenden Kalibrier-/Wrapper-/ApprovalLockhashes. Assetfloatdaten werden ausschließlich über exakte Rawbytes gebunden, niemals durch Änderung des integer-JCS-Profils.

Alle Listen sind eindeutig; observationIds referenziert exakt `{observationId,exposureId}`. Pro Logicclass ist mindestens eine tatsächlich inkludierte Referenz nötig; Campaignübertragung auf ungemessene Fälle bleibt ausdrücklich als Methoden-/Annahmeentscheidung nachweisbar. Production benötigt identische tatsächlich gemessene UIquelle; keine automatische neue-UI-Übertragung. Der aktuelle UIlock ist bereits eigenständiger gehashter Importinput. Die interne Form ist technisch angenommen. Ohne die tatsächlichen darin verlangten Quellen, Humanbeobachtungen und unabhängig erhaltenen Annahmeoriginale bleibt Production geschlossen; FixtureOnly bleibt isoliert.

Zusätzliche zwingende Validierungsregeln: Aus dem tatsächlich erhaltenen gehashten ursprünglichen `Content/Quality/s1/difficulty-and-human-sample.json` und vollständigen neun ursprünglichen Campaignmanifesten wird die bindende Menge `humanSampleRequired` rekonstruiert (aktuell54 unterschiedliche tatsächliche Fälle). **Jeder** erforderliche semantisch exakte Fall muss durch mindestens eine tatsächlich inkludierte passende CAMPAIGN_TIME-Beobachtung mit exakter ursprünglicher Dokument-/Manifest-/Public-/UIbindung gedeckt sein. Reine Endlosbeobachtungen oder Klassenübertragungen erfüllen diese Pflicht niemals. Root-`calibrationVersion` und `uiSourceSha256` stimmen ausdrücklich mit jeder aus diesem Nachweis verwendeten Ziel-Kalibrierzeile, Ziel-Levelschwelle und GQP-humanReview-Version/UI überein. Ob ein Kampagnenziel direkt gemessen oder übertragen ist, ergibt sich aus tatsächlicher inkludierter Observation-Identity **und** Originalquelle, nicht aus einer frei gesetzten Kennzeichnung.

## Geschlossene Runtimeimportbindungen

Nach getrennter C-Fachreview der Fassung SHA256 `c08e6b2c1c201dcda6a267f7e1c9b825629503013f30c3f65fb315edc770d5f5` von R am21.10.2026 vor abhängiger Runtime-/Buildimplementierung angenommen. Die bestehenden Sourceapproval-, Proof-, Human-, Provider- und Releasegates bleiben erforderlich; diese technische Formannahme gewährt keine davon.

### ProductionImport Runtimeausgabe v1 – angenommene genaue Montageform

Bestehende ADR044-Quellen/Hashes bleiben unverändert. Alle zwölf Payloaddateien sind vor Ausgabe vollständig gegatet. `Runtime.bindings.json` ist das dreizehnte Playerartefakt; kein zusätzliches Selfhashfeld/-profil. Rootbuild prüft seine exakten Bytes und injiziert den tatsächlichen Raw-SHA in die Scenecomposition.

Dateiset exakt: `Public.json`, `Campaign.json`, `CampaignCompletion.json`, `EndlessCompletion.json`, `Calibration.json`, `Cosmetics.json`, `Profiles/dienstbeginn.json`, `Profiles/regelbetrieb.json`, `Profiles/stoerungslage.json`, `Profiles/grosslage.json`, `RailwayAssets.json`, `CampaignText.json`.

`Runtime.bindings.json` exakt diese18 Felder:

```
{runtimeImportVersion:1,use,status,
 releaseLockRawSha256,releaseLockJcsSha256,approvalLockRawSha256,
 publicCatalog,campaignCatalog,campaignCompletionCatalog,endlessCompletionCatalog,
 calibrationCatalog,cosmeticsCatalog,qualityProfiles,catalogSet,
 railwayAssetsRawSha256,campaignTextRawSha256,uiSourceLockRawSha256,playerFiles}
```

- use nur `Production` oder `ContractTest`; status exakt `PRODUCT_APPROVED` beziehungsweise `FIXTURE_ONLY`. Keine QA-/Draftconsumption.
- Sechs Catalogfelder und `catalogSet` sind jeweils exakt bestehende `{catalogId,revision,hashSha256}`. catalogSet ist Bs tatsächlich gelesene bestehende3ref-Aggregatprojektion/Identität (Production `production-s1-catalog-set` revision1, ContractTest separat), kein neuer Hashweg.
- publicCatalog.hash ist die bestehende JCS-Projektion des Publicpayloads ohne allein catalogHashSha256. Campaign/Cosmetics/Calibration/vierGQP sind volle bestehende JCS-Hashes; beide Completionhashes sind jeweils vollständiger Wrapper-JCS. Der ReleaseLock-v1 enthält exakt diese zehn Katalog-/Profilhashes und240 vollständige Puzzle-/Dokument-/Lösungs-/Proofbindungen.
- qualityProfiles genau vier Zeilen `{profileId,hashSha256}` in ordinaler Profil-ID-Reihenfolge, geschlossenes Namedset. Keine Selfhashfelder in GQP.
- releaseLockRawSha256 ist SHA256 der unveränderten tatsächlichen ReleaseLockdatei; releaseLockJcsSha256 deren voller bestehender integer-JCS-Hash. approvalLockRawSha256 Production ist die separat geschützte erwartete und tatsächlich geprüfte Originalquelle, in ContractTest genau null. Keines davon ist allein Humanattestation.
- railwayAssetsRawSha256 und campaignTextRawSha256 sind SHA256 der jeweiligen exakten Playerdateibytes. RailwayAssets enthält Floats und wird **bytegleich** aus der tatsächlichen Raw-gebundenen Quelle kopiert, niemals durch integer-JCS geleitet. CampaignText ist JCS der unten vorhandenen Projektionsform.
- uiSourceLockRawSha256 bindet den tatsächlichen vollständigen UI75-Lockinput; vollständiges exaktes Quellfileset und jede Rawdateibindung werden im Import/Build geprüft. Das bestehende UIlocale ist darin enthalten, kein zweiter UIstringrenderer entsteht.
- playerFiles genau zwölf Zeilen `{path,rawSha256}` ordinal nach obigem Dateipfad, eindeutige exakte feste Pfade, keine absoluten/Traversal-/Backslashpfade. Keine Bindings-Selbstzeile, Extrazeile oder fehlende Datei. Jeder Rawhash trifft die tatsächliche ausgegebene Datei. Die semantischen Kataloghashes werden zusätzlich durch die bestehenden Reader geprüft; Rawlisten ersetzen diese nicht.

CampaignText ist die **vorhandene** Projektion `{schemaVersion:1,locale:"de-DE",approvalStatus,sourceSha256,entries:[240×{puzzleId,title,status,result}]}` in Publicreihenfolge. sourceSha256 bindet die tatsächliche einzelne Authoringlokalisierungsdatei, aus der genau alle720 referenzierten Texte gelesen wurden; mehrere uneindeutige Quellursprünge scheitern. Productionstatus PRODUCT_APPROVED, technische Fixture ausdrücklich FIXTURE_ONLY. Keine internen Entrydeductiontexte, Lösungen, Humanoriginale, Methoden oder Proxies im Player. Alle weiteren Text-/Asset-/Graphreferenzen werden trotzdem intern gegen die tatsächlichen Quellen geprüft.

Authoringausgaben separat außerhalb Player: `authoring/source-bindings.json` (alle gelesenen Original-Raw-SHAs, alte gemessene vs neue Targetlevelhashes getrennt), `authoring/player-raw-lock.json`. Kein Runtimefallback auf diese internen Dateien. Kein technischer PASS öffnet reale Human-/Geräte-/Storegates.

## Enger Metadatenabgleich gemessener UIquellen

Nach getrennter C-Fachreview der Klarstellung SHA256 `fb983a252d6344fdd6b820b82318515f83347b618336906beefa6bd8361f7fda` von R am21.10.2026 vor abhängiger Importprüfung angenommen. Keine Änderung an Evidence-/Runtime-/Save-/Hashformen; ausschließlich der konkret geschlossene, tatsächlich nachgewiesene Freigabemetadatenwechsel.

### ADR044 – enge gemessene/Target-UI-Klarstellung, prospektiv

Kein neues Evidence-/Runtimefeld oder Hashprofil. `Evidence.uiSourceSha256`, `Calibration.provenance.uiSourceSha256` und `GQP.humanReview.uiSourceSha256` bleiben **immer** der volle Raw-SHA des tatsächlich gemessenen alten UI-Quelllocks. Die Runtimebindung `uiSourceLockRawSha256` bindet getrennt den tatsächlichen Target-UIlock. Kein Targethash wird als gemessene Quelle ausgegeben.

Gleicher Lockhash benötigt keinen Transfer. Bei anderem Lockhash wird unter der bestehenden tatsächlichen unabhängigen Sourceannahme des Evidence-/ApprovalLocks nur dieser geschlossene Metadatenfall unterstützt:

1. Alter und Target-UIlock haben exakt dieselbe ordinal sortierte vollständige75er-Quelldateiliste; jede genannte Originaldatei ist vorhanden und trifft ihren jeweiligen Raw-SHA. Kein fehlender/zusätzlicher/umbenannter Pfad.
2. Alle Quellen außer exakt `Assets/StammstreckenPuzzle/UI/RailwayAssets.json` und `Assets/StammstreckenPuzzle/UI/ProductionCampaignText.json` sind Raw-bytegleich. Damit insbesondere sämtliche CS/UXML/USS, UIlocale, Art, AppIcon und Fonts. Keine Rendering-/Input-/Timing-/Struktur-/Textänderung wird übertragen.
3. RailwayAssets: beide bounded strikt ohne Duplicatekeys gelesen; genau das Topfeld `approvalStatus` darf von DRAFT nach PRODUCT_APPROVED wechseln. Alle übrigen vollständigen verschachtelten JSONwerte sind exakt gleich. Floatnummern werden ohne Rundung oder integer-JCS-Konvertierung verglichen (exakte ursprüngliche Zahlentokens; abweichende Schreibweise wird konservativ abgewiesen). Der vollständige jeweilige OriginalrawHash bleibt zusätzlich gebunden.
4. ProductionCampaignText: genau Topfelder `approvalStatus` (DRAFT→PRODUCT_APPROVED) und `sourceSha256` (alter vs tatsächlicher neuer gebundener Authoringtext-OriginalrawHash) dürfen wechseln. `schemaVersion`, locale und sämtliche240 geordneten `{puzzleId,title,status,result}` einschließlich aller720 Texte bleiben exakt gleich. Neuer sourceSha256 wird zusätzlich gegen die wirklich gelesene Targetauthoringquelle geprüft. Der alte Wert bleibt durch den alten Original-UIlock/Readback erhalten.
5. Die bestehende gehashte unabhängige Sourceannahme benennt diese beiden tatsächlichen Lockbindungen und den ausschließlichen Metadatenwechsel ausdrücklich. Der geschützte ApprovalLock bindet beide Originalfilesets und die Annahmequelle. Das ist kein automatischer neuer-UI-Messclaim und keine generische UItransfererlaubnis.

Jede andere Abweichung ist fail-closed und erfordert echte neue Mess-/Annahmequellen. Bereits durch die identischen Text-/Art-/CS-Originale gemessene Darstellung wird nicht wegen allein des Freigabestatus oder Authoringtext-Rawmetadatums unbrauchbar gemacht. Noch keine tatsächlichen Human-/Productionfreigaben vorhanden.
