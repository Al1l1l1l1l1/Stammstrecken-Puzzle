# ADR-040 – Interner Endlosgenerator und begrenzte Noveltydaten

## Status

**Angenommen** am 21.10.2026 durch Releaselead R innerhalb WP-046 nach getrennter Fachreview durch C. Der konkrete Regenerationsbefund (kein Überspringen eines zeitbedingt unbestimmten Kandidaten) wurde vor Annahme in Entscheidung6 nachgeschärft; Root-Suchkern entsprechend korrigiert, tatsächliche Regression noch auszuführen. Annahme erfolgt vor abhängiger neuer Parameter-/Novelty-Saveumsetzung. Keine Human-/Profil-/Geräte-/Produktfreigabe damit.

## Datum

21.10.2026

## Kontext

ADR-019/025/027 legen Reservation, Watermark, höchstens 20 offene Endlosvorgänge und lokale beziehungsweise providergebundene Claimabschlüsse fest. Die vorhandene Saveimplementierung kennt bisher nur die explizite Testparametergrammatik `contract-preservation-parameters-v1`. Die neue echte Suche `path-search-v1` benötigt vollständige Parameter und eine beim Reservieren festgeschriebene Noveltybasis, damit ein Crash keine neue Instanz oder veränderten Referenzbestand erzeugt. P02 verlangt alle vier Größenklassen, Duplicate-/D4-Schutz gegen die Kampagne und die jüngsten 50 tatsächlich ausgegebenen Endlosrätsel. Ein dauerhaft wachsendes Terminalarchiv bleibt verboten.

## Entscheidung

1. Ausschließlich der getrennte ADR-039-Verbraucher `InternalPreview`/`qa-mobile-candidate` erhält die neue registrierte Grammatik `path-search-v1` und `public-d4-l1-v1`. Die alte Testgrammatik und die Public240-v2-Linie bleiben unverändert. `Production` darf DRAFT-Profile nicht lesen oder erzeugen. Die vier technischen Profile decken 4–5, 5–6, 6–8 und 8–10 ab; tatsächliche Human- und Geräteprüfung bleiben offen.
2. Neue `SearchGeneratorParameters` enthalten geschlossen und ohne Defaults: `contractVersion:1`, `profileId`, `profileHashSha256`, `noveltyVersion`, `campaignCatalog` als bestehende dreifeldrige Katalogprojektion, `recentPublicInputs` mit höchstens 50 vollständigen öffentlichen Inputs sowie alle zwölf Suchfelder `minSize`, `maxSize`, `minDensityPercent`, `maxDensityPercent`, `minDeductionSteps`, `minDeductionDepth`, `maxAttempts`, `pathStateBudget`, `totalBudgetMs`, `solverBudgetMs`, `maxTrivialLines`, `recentSimilarityPercent`. Der vollständige kanonische interne Profilquelltext wird durch den Profilhash gebunden und muss die identischen zwölf Felder, `path-search-v1`, `solver-v2`, den echten Publickorpuslock, Noveltyversion und DRAFT-Provenienz belegen. Fehlende/veränderte Profile erlauben keine neue Reservation. Bereits reservierte Instanzen verwenden ausschließlich ihre gespeicherten Parameter und exakt gebundenen Kampagneninputs.
3. `parameterHashSha256` bleibt SHA-256 über genau die RFC-8785/JCS-Parameterbytes. Die bestehende `endless-v1`-IDprojektion `{endlessContractVersion,rulesetVersion,generatorVersion,seed,generationOrdinal,parameterHashSha256}` und sämtliche Puzzle-/Lösungs-/Proofhashprofile bleiben unverändert. Keine alternative Hashfunktion und kein stilles Rebinding.
4. `LocalEndlessState` erhält für den neuen Save-v3-Pfad eine geschlossene `recordVersion:2` mit `highestReservedOrdinal`, `openEndless`, `recentPublicInputs`. Das zusätzliche Fenster enthält die letzten höchstens 50 in erfolgreichem Promotioncommit tatsächlich ausgegebenen öffentlichen Inputs in Ausgabereihenfolge. Es dient allein dem Noveltyvergleich. Es berechtigt weder Resume, Reward, neue Credits noch eine terminale Wiederbelebung. Terminalität bleibt ausschließlich Watermark plus offenes Komplement. Doppelte oder semantisch ungebundene Fensterinputs werden abgewiesen; ein neuer Eintrag verdrängt ausschließlich den ältesten. Es werden keine Lösungen gespeichert.
5. Alte `recordVersion:1`-Zustände bleiben lesbar und werden unverändert geschrieben, solange der neue Pfad keine Noveltydaten benötigt. Ein neuer Zustand mit Noveltydaten darf nicht durch einen v2-Writer verloren gehen. Save-v3 kann Version 1 und 2 lesen; unbekannte Versionen bleiben unverändert als unsupported geschützt. Die neutrale v2→v3-Goldenmigration wird dadurch nicht verändert.
6. Reserve commitet den neuen Ordinal und vollständigen Descriptor vor jeder Generierung. Fehler, Timeout oder Abbruch behalten die Reservation für expliziten Retry beziehungsweise Abandon; sie erzeugen keine Ersatzreservation. Promotion verlangt echten Unique-/NoGuess-Solvernachweis, gebundene öffentliche Projektion sowie tatsächliche Lösungs- und Proofhashes. Fehlende Generator-/Referenzversion ergibt `ENDLESS_GENERATOR_RECOVERY_REQUIRED`. Zeitbudgetüberschreitung, Cancel oder ein nicht eindeutig abgeschlossenes Solverergebnis brechen den gesamten Generationsversuch ohne Promotion ab. Sie dürfen niemals einen Kandidaten als ungeeignet überspringen und anschließend einen späteren Kandidaten auswählen: Hardware-/Wallclocklaufzeiten dürfen nicht die gewählte Instanz beeinflussen. Ausschließlich deterministisch abgeschlossene Ablehnungen mit exakt gebundenen PRNG-/Generator-/Solverversionen setzen die Kandidatenfolge fort. Retry startet dieselbe Folge mit denselben Grenzen erneut. Gewonnene, aber noch nicht promotete Arbeit darf verworfen werden; nur der persistierte Descriptor kann sie erneut anfordern.
7. Aktive Endlosattempts werden im bestehenden einen Sessionwriter gespeichert. Abschluss ersetzt den aktiven Payload atomar durch die bereits definierte offene Claimfortsetzung. Der lokale ADR-027-Skip bleibt providerfrei. Rewardbeträge stammen aus dem exakt geladenen Completionkatalog; Reservation/Callback/Commit verwenden exakte Claim-/Operation-/Generationbindungen. Terminale Instanzen rekonstruieren keinen Claim. Für terminale Endlosinstanzen dürfen Hintdetailrecords entfernt werden, weil fehlende Aktivbindung weiterhin jeden neuen Creditanspruch verhindert. Kampagnenhintrecords bleiben identitätsgebunden erhalten.

## Begründung

Die neue geschlossene Grammatik macht den bereits beschlossenen echten Generator ausführbar und crashfest, ohne historische Testdaten als Produktparameter umzudeuten. Ein konstantes öffentliches 50er-Fenster erfüllt den Noveltyvertrag bei konstant begrenztem Save. Gepinnte Referenzen verhindern, dass die gleiche Reservation nach einem Crash unter veränderten Qualitätsbedingungen läuft. Der nichtproduktive Verbraucher hält die offene Kalibrierung sichtbar.

## Betrachtete Alternativen

- Neue Zufallsinstanz nach Timeout: verletzt persistierte Reservation und Identität.
- Nur 50 Hashes speichern: reicht für D4-/Endpunkt-/Zahlmustervergleich und gepinnte Regeneration nicht aus.
- Alle terminalen Puzzleinputs speichern: verletzt die begrenzte Endloshistorie.
- DRAFT-Profile als produktfreigegeben markieren: widerspricht P02 und ADR-039.
- Neue Hashprofile oder ein paralleler Endlossave: unnötige neue Integritätswahrheit.

## Konsequenzen

Die Application erhält eine geschlossene Parameterklasse und Reservation-/Promotion-/Resume-/Completion-/Abandon-/Claimpolicies. Infrastructure erhält strikte Profil-/Parameter-/Recordreader. Größere Saves bleiben durch 20 offene Vorgänge, höchstens 50 Fensterinputs je gepinnter Reservation und das globale 50er-Fenster begrenzt. Die Arbeit behauptet keine Geräteperformance, menschliche Schwierigkeit oder Produktannahme. Konkrete Suchdefizite werden durch bessere Suche/Quellen bearbeitet; Schwellen werden nicht für einen grünen Nachweis gelockert.

## Betroffene Artefakte

`Application/LocalProfileState.cs`, neue Endlosparameter/-policies/-usecases, `Infrastructure/Persistence/LocalSaveCodec.cs`, `SaveMigration.cs`, neue strikte Profilreader, `Content/Generator/path-search-v1.profiles.json` und zugehörige Qualitylocks, gezielte EditMode-/PlayModeprüfungen. R besitzt Generator/Qualityquellen und Komposition, B die Save-/Metagameimplementierung, C prüft Provider-/Rewardbindungen unabhängig.

## Validierung

Unabhängige fachliche Review vor Abhängigmachen. Expliziter Langsam-/Timeoutfall muss Gesamtversuchabbruch statt Auswahl eines späteren Kandidaten belegen. Anschließend echte vier Profilklassen mit Unique/NoGuess, vollständige Descriptor-/Hashmutationen, Reservation-vor-Generation, Crash-/Retrybindung, erfolgreiche Promotion, kompletter Resume, Abschluss/Skip/Abandon, verspätete Callback-Noops, widersprüchliche Operation-/Generationbindung, CAS, 20er-Kapazität, UInt64-Grenze und mindestens 10.000 reine Lifecyclewechsel ohne wachsende Terminalhistorie. Noveltyfälle prüfen Kampagnenduplikat, Rotation/Spiegelung, ähnliches Zahlen-/Endpunktmuster, Fenster 50→50, Ablehnung ungebundener Inputs sowie unveränderte historische QA-/neutrale Save-Goldens. Runtime-/Human-/Storegates bleiben gesonderte reale Nachweise.

## Ersetzt / ersetzt durch

Ergänzt ausschließlich neue interne Generatorparameter und begrenzte Noveltydaten zu ADR-019/025/027/037/039. Deren übrige Integritäts-, Hash-, Claim-, Retentions- und Nichtproduktionsgrenzen bleiben bestehen. Wird derzeit durch keinen ADR ersetzt.

