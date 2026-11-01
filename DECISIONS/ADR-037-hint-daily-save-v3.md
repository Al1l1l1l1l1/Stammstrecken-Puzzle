# ADR-037 – Identitätsgebundene Hinweise und lokale Tagesclaims in Save v3

## Status

**Angenommen** – 21.10.2026 nach getrennter fachlicher Review durch Bahn C und prospektiver technischer Annahme durch Releaselead R im WP046-Mandat. Keine Human-/Geräte-/Implementierungsannahme.

## Datum

21.10.2026
**Auftrag:** WP-046, MANUS-2026-10-20-02 P03/P04; technische Annahme durch R erst nach unabhängiger Fachreview.

## Kontext

Save v2 besitzt nur einen modegebundenen Hint-Platzhalter und Tages-Claimstrings ohne Ergebnis-/Operationsbindung. Das genügt nicht für drei kostenlose Hinweise insgesamt je Puzzleidentität, einen zusätzlichen Videoanspruch und atomar ausgewürfelte Tagesbelohnungen. Die bestehenden Envelope-, CAS-, Ledger-, Provider- und Profilgrenzen bleiben erhalten.

## Entscheidung

Der Writer erhöht `saveSchemaVersion` auf 3. `STP-SAVE-JCS-1`, Ganzzahlgrenzen, Puzzle-/Katalog-/Ledgerhashprojektionen und Generation-/Recoveryalgorithmus bleiben unverändert. Alte v1/v2-Reader und ihre originalen Hashverifier bleiben lesend erhalten; vor Migration wird die unveränderte Quelle gesichert. Production und QA bleiben strikt getrennte Profile.

`hintState` ersetzt den Einzelplatzhalter. Es enthält `policyVersion: hint-policy-v1` und sortierte Records mit vollständiger `{puzzleId, profile, sha256}`-Identität, `regularConsumed` 0–3, `rewardedConsumed` 0–1 und einem optionalen Rewardrecord. Der Rewardrecord bindet stabile Claim-ID `hint-reward:v1:<puzzleId>:<publicHash>`, lokale Operation, unveränderliche Provideroperation und Zustand `RESERVED|RECONCILIATION_REQUIRED|CONFIRMED|CLOSED_NO_REWARD`. Genau ein `CONFIRMED` erzeugt genau einen zusätzlichen Credit. Kein Sessionmodus/Neustart füllt Credits auf. Ein verfügbares reguläres oder bestätigtes Zusatzcredit verhindert eine neue Videoreservation. Bestätigt ohne Reward darf derselbe Anspruch erneut mit neuer Operation versucht werden; callbacks der abgeschlossenen Operation bleiben No-op und erzeugen keinen Credit.

Ein sicherer Hinweis wird zuerst berechnet, dann in der pausierten Hilfeansicht tatsächlich dargestellt. Erst die quittierte Anzeige verbraucht ein Credit in einem atomaren Save zusammen mit dem Session-Hintzähler; ein Fehler oder fehlender Schluss verbraucht nichts. Die Anzeigequittung bindet Identität, Attempt, Sessionrevision, eindeutige Display-ID und sicheren Schluss. Ein zweiter Callback derselben Display-ID ist No-op. Ein Crash vor Commit darf kostenlose Hilfe wiederholen; er darf keinen Credit ohne nachweisbare Darstellung verbrauchen. Eine Hintanzeige schreibt nie eine Lösung oder Sternsperre.

Campaign-Hintrecords bleiben für alle unterstützten Kampagnenidentitäten erhalten. Endlos-Hintrecords werden nur für offene, wiederaufnehmbare Ordinale gehalten; ein terminales Endlosordinal kann nie erneut Hintberechtigung aus Recordabwesenheit erhalten. Ein zukünftiger Katalogwechsel erhält ältere unterstützte Records; unbekannte gleiche Puzzle-ID mit anderem Hash wird nicht still umgedeutet.

`dailyState` enthält `policyVersion: daily-local-v1` und begrenzte sortierte Claimrecords. Fachliche Claim-ID ist `daily:v1:<YYYY-MM-DD>:<FREE|REWARDED>`, nach lokalem Gerätedatum bei Reservation; spätere Zeitzonen-/Uhrwechsel ändern diese Bindung nicht. Pro Datum ein FREE und ein REWARDED. FREE reserviert Ergebnis und Punkte atomar; REWARDED reserviert zusätzlich die Provideroperation vor SDK-Aufruf und grantet erst nach exakt passendem bestätigtem Callback. Slot und Datum sind danach unveränderlich. Kein Serverzeit-/Zukunftslock und kein Streakverlust.

Die unveränderliche Ergebniszahl wird bei Reservation aus einer injizierten Zufallsquelle mit gleichverteiltem Integer 0–99 festgelegt: 0–59 → 10, 60–89 → 15, 90–99 → 25. Ein offener Claim wird nach Neustart mit demselben Ergebnis fortgesetzt. Der Commit schreibt `DAILY_PATIENCE`, Claimterminal und Ledgerdelta gemeinsam. Callback/Command liefern nie einen Betrag. Claim-ID, Operation, Provideroperation, Zustand und Ergebnis müssen übereinstimmen; Doppelcallback ist No-op. Unbekannter Providerausgang bleibt `RECONCILIATION_REQUIRED`; kein zweiter SDK-Aufruf. Ein bestätigtes No-Reward darf denselben Slot erneut anbieten, behält dabei das bereits gebundene Ergebnis. Ein verspäteter Callback der freigegebenen Operation bleibt wirkungslos.

Dailyrecords sind auf 768 begrenzt. Davon dürfen höchstens 20 Claims offen sein. Terminalrecords bleiben erhalten, solange Journal oder dessen 64er Diagnosetail sie als Ledgerbeleg benötigen; zusätzlich bleiben die jüngsten 64 terminalen Claims erhalten. Vor Entfall wird erforderliche Ledgerkompaktierung atomar durchgeführt. Erst danach werden ältere nicht mehr referenzierte Terminaldetails entfernt. Recordabwesenheit außerhalb dieses Fensters kann einen alten Datumsslot wieder ermöglichen: bewusst best-effort gemäß P04. Offene Records werden nie verdrängt. Bei Kapazitätsgrenzen bleibt das Spiel weiter nutzbar; nur ein neuer Tagesclaim meldet einen recoverbaren Kapazitätsfehler.

v2→v3 ist rein und deterministisch: leere/null Hint-/Daily-Platzhalter werden zu leeren Zuständen; bestehende eindeutig gebundene Hintverbrauchs-/Claimdaten werden konservativ übernommen, sofern ihre Bedeutung aus dem gebundenen historischen Vertrag folgt. Fehlt diese Bindung, stoppt `SAVE_MIGRATION_NEEDS_DECISION` und erhält die Quelle. Keine Umwandlung alter QAprofile in Production, keine erfundenen Claims oder frischen Credits für belegte unbekannte Daten.

## Begründung

Hintverbrauch muss an unveränderliche Puzzleidentität statt Sessionmodus gebunden sein. Dailyergebnis und Reward müssen atomar mit ihrer eindeutigen Operation gespeichert sein. Eine explizite Saveversion erhält die strikte Lesbarkeit alter Quellen.

## Betrachtete Alternativen

Die Recordänderung innerhalb von Save v2 wird verworfen: alte strikte Reader würden legitime neue Payloads als korrupt behandeln. Ein zweites Metagame-Save wird verworfen, weil Credit/Session/Ledger dann nicht atomar wären. Serverzeit und Backend werden gemäß Produktmandat nicht eingeführt. Die Collections sind bewusst begrenzt, während Campaignfortschritt und verdientes Eigentum erhalten bleiben.

## Konsequenzen

Neue v3-DTOs und eine neutrale Migration sind erforderlich. Die Hashprofile, QAisolation und bestehenden atomaren Repositoryverfahren bleiben erhalten; begrenzte lokale Tagesdeduplikation erhält bewusst keine Serverzeitgarantie.

## Betroffene Artefakte

Application/LocalProfileState und neue Hint-/Dailypolicies; Infrastructure.Persistence/LocalSaveCodec, SaveMigration, SaveLedgerCodec; entsprechende EditMode-Tests und ARCHITECTURE/PERSISTENCE.md. Registry/Annahme besitzt R. Keine Änderung bestehender Puzzlehashes oder Providerverträge.

## Validierung

Unabhängige Review dieses Vertrags; v2→v3-Byte-/Hashgoldens und neutrales Migrationsstopp-Golden; Hint-Neustart/Übung/Revision/Hashwechsel/Anzeigeabbruch/Duplicate-Quittung; Daily-Datumswechsel/Reise/Uhrrückstellung/Ergebnisneuwürfelung/20-offen-Grenze; Crash vor/nach Reservation/Grant, CAS-Races, gefälschte/späte Providercallbacks, Ledgerkompaktierung mit 768 Records und langfristige begrenzte Retention. Keine Implementierungs-/Human-/Produktabnahme wird durch diesen Entwurf behauptet.

## Ersetzt / ersetzt durch

Ergänzt ADR-019/025/035 um Save-v3-Records für P03/P04 und ersetzt ausschließlich die noch nicht produktiven Hint-/Daily-Platzhalterformen. Unveränderte historische v1/v2-Verträge bleiben lesend erhalten. Noch durch keinen ADR ersetzt.
