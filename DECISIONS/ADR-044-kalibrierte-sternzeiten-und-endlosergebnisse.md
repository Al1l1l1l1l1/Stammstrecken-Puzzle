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
