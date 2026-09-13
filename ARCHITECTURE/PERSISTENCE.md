# Persistence v0.5

## 1. Ziel und Wahrheiten

Der lokale, versionierte Save-Snapshot ist die autoritative Quelle für Gameplay-Fortschritt, Entwürfe, Geduldspunkte, kosmetische Freischaltungen, Einstellungen und persistente Mobile-Operationen. Das Spiel ist vollständig offline spielbar. Analytics, Crashlytics und Werbung speichern oder rekonstruieren keine Gameplay-Wahrheit.

Der Apple App Store beziehungsweise Google Play Store ist die externe Wahrheit für den nicht konsumierbaren Anspruch `remove_ads`. Der lokale Save enthält einen ausfallsicheren Cache, den persistenten Operationszustand und verarbeitete Transaktionsreferenzen. Es gibt keine Cloud-Synchronisation und keinen `ISaveSyncPort`.

## 2. Speicherorte und Dateien

Alle Dateien liegen in Unitys app-privatem `persistentDataPath`. Nur `STP.Infrastructure.Persistence` besitzt den Pfad.

| Datei | Zweck |
|---|---|
| `save.json` | letzter erfolgreich atomar bestätigter Snapshot; der Dateiname ist keine DTO-Version. |
| `save.backup.json` | unmittelbar vorheriger gültiger Snapshot. |
| `save.pending.json` | vollständig zu schreibender Kandidat; nur mit gültigem Envelope wiederherstellbar. |
| `save.migration-source.json` | unveränderte Sicherung vor Schema- oder Hashprofilmigration. |
| `diagnostics-ring.jsonl` | begrenztes lokales, redigiertes Diagnosejournal. |

PlayerPrefs wird nicht für Spielstände, Währung, Entitlements, Claimreservierungen oder Entwürfe verwendet.

## 3. Save-Envelope und Hashprofil

| Feld | Vertrag |
|---|---|
| `saveSchemaVersion` | positive Ganzzahl; aktueller Schreibstand `2`; versioniert die Payloadform. |
| `payloadHashProfile` | geschlossenes Profil; initial `STP-SAVE-JCS-1`. |
| `generation` | bei jedem erfolgreichen Commit streng erhöhtes 64-Bit-Integer im Save-Ganzzahlbereich. |
| `createdAtUtc` | RFC-3339-UTC-Diagnosezeit; keine Konfliktwahrheit. |
| `appVersion`, `buildId` | Diagnose und Migrationsnachweis. |
| `contentCatalogHash` | Katalogstand, gegen den IDs validiert wurden. |
| `payload` | vollständiger `GameProfileState` plus aktive Entwürfe und Operationen. |
| `payloadSha256` | kleingeschriebener SHA-256-Hexwert über die vom Profil definierten Bytes. |

Die Prüfsumme erkennt Korruption und unvollständige Writes, ist aber kein Manipulationsschutz.

### 3.1 `STP-SAVE-JCS-1`

Die Hashprojektion ist exakt das JSON-Objekt unter dem Envelopefeld `payload`; Envelopefelder werden nicht mitgehasht. Die Bytes sind die UTF-8-Ausgabe des **JSON Canonicalization Scheme nach RFC 8785** über diesen Payload.[3]

Der Vertrag ist vollständig:

- kein Byte Order Mark und kein abschließendes Zeilenende;
- Objektschlüssel rekursiv gemäß RFC 8785 nach UTF-16-Codeeinheiten sortiert;
- Arrayreihenfolgen unverändert;
- Strings gemäß JCS/ECMAScript escaped, ohne Unicode-Normalisierung;
- ungültige Surrogate und Nicht-I-JSON-Daten werden abgewiesen;
- keine Fließkommazahlen, NaN oder Infinity im Savevertrag;
- Ganzzahlen ausschließlich in `[-9007199254740991, 9007199254740991]`; größere technische Werte als validierte Dezimalstrings;
- Hashfunktion SHA-256; Ausgabe 64 kleingeschriebene Hexzeichen.

`STP-SAVE-JCS-1` ist eine Serializer-**Vertragsversion**, keine Bibliotheksversion. Ein Bibliothekswechsel ist nur zulässig, wenn alle Bytegoldens unverändert bleiben.

### 3.2 Alte Saves und Profilwechsel

Der Leser wählt den Verifier zuerst aus `payloadHashProfile`. Erst nach erfolgreicher Hashprüfung wird die DTO-/Fachvalidierung ausgeführt. Danach folgen sequenzielle `saveSchemaVersion`- und gegebenenfalls Hashprofilmigrationen. Das Ergebnis wird als neue Generation mit dem Zielprofil geschrieben.

Ein unbekanntes Profil liefert `SAVE_HASH_PROFILE_UNSUPPORTED`, nicht `SAVE_CORRUPT`. Der Kandidat bleibt unverändert erhalten und löst keinen automatischen leeren Save aus. Ein neues Profil benötigt einen Reader für jedes noch unterstützte Profil, Golden Input/Bytes/Hash, Migration und explizite Supportgrenze. Ein fehlendes Profil in einer künftig tatsächlich veröffentlichten Legacyversion darf nur über eine versionsgebundene, dokumentierte Legacyregel interpretiert werden; es gibt keinen heuristischen Serializer-Fallback. Vor v0.2 wurde kein Production-Save veröffentlicht, daher beginnt der erste Scaffold direkt mit `STP-SAVE-JCS-1`.

## 4. Payloadstruktur

| Bereich | Inhalt |
|---|---|
| `profile` | lokale Profil-ID, erste Nutzung, höchste freigegebene Inhalte, Meisterschaft. |
| `levels` | Records pro `puzzleId` plus profiliertem `publicPuzzleHashAtFirstCompletion`, Abschluss, Stern-High-Water, Zeiten und endlichen Claims. |
| `endless` | `highestReservedOrdinal` als UInt64-Dezimalstring plus höchstens 20 `openEndless`-Einträge in `RESERVED_NOT_GENERATED`, `ACTIVE_DRAFT` oder `COMPLETION_CLAIM_OPEN`. |
| `ledgerCheckpoint` | kompaktierter Saldo-/Hashkettenstand. |
| `economyJournal` | begrenzte idempotente Gutschriften/Belastungen nach dem Checkpoint. |
| `inventory` | gezielt freigeschaltete und ausgewählte kosmetische IDs samt terminaler Kauf- oder Meilensteinclaim-Provenienz. |
| `cosmeticClaimReservations` | höchstens eine offene, eligibility-validierte Reservation je fachlicher Cosmetic-Claim-ID mit Operation, Generation, Item-, Katalog- und Eligibility-Provenienz. |
| `seasonProgressCache` | abgeleitete Route-/Abschnitt-/Seasonstände plus Quellhash. |
| `drafts` | höchstens ein aktiver Entwurf je Level, begrenzt auf die jüngsten 20, inklusive Undo-Diffs. |
| `rewardClaims` | `POST_CLEAR_PATIENCE`-Reservationen/-Terminals und künftig freigegebene Claimarten. |
| `dailyStatus` | reservierter `DailyEntitlementRecord`; keine Tagespolicy vor `BLOCKER-PROD-002`. |
| `entitlements` | `remove_ads`, Store, logischer Produktkey, letzte bestätigte Transaktionsreferenz und Reconciliationstatus. |
| `pendingOperations` | persistente Reward-/Kauf-/Restore-/Finalisierungsoperationen ohne Receipt-/Tokenrohwerte. |
| `settings` | Locale, Lautstärkegruppen, Haptik, Accessibility- und Privacypräferenzen. |
| `dataLifecycle` | versionierter `PrivacyDecisionRecord`, Policy-/SDK-Revision, Gültigkeit, `nativeSyncState` und letzte Reconciliation; keine SDK-Caches als fachliche Wahrheit. |

Unbekannte Level- oder Asset-IDs werden als `orphaned` erhalten, diagnostiziert und bei erneut verfügbarem Content reaktiviert.

## 5. Atomarer Schreibalgorithmus

Jeder Commit erhält eine fortlaufende `generation`:

1. neuen unveränderlichen Snapshot erzeugen und fachlich validieren;
2. Payload mit dem benannten `payloadHashProfile` kanonisieren und SHA-256 berechnen;
3. vollständigen Envelope nach `pending` schreiben;
4. Stream flushen und bestmöglichen persistenten Plattformflush anfordern;
5. `pending` erneut öffnen, Profil dispatchen, parsen und Hash prüfen;
6. gültige Hauptdatei nach `backup` rotieren;
7. `pending` über eine plattformgetestete Replace/Rename-Operation zur Hauptdatei machen;
8. Hauptdatei erneut auf Profil, Generation und Hash prüfen;
9. erst danach `SaveCommitted(generation)` melden.

Kann eine einzelne Dateisystemoperation nicht atomar garantiert werden, verwendet der Adapter Copy-on-Write über die drei Kandidaten und lädt ausschließlich vollständig validierte Generationen. Es gibt höchstens einen Writer; weitere Mutationen werden seriell zusammengeführt.

## 6. Lade- und Recoveryalgorithmus

`main`, `backup` und `pending` werden unabhängig gelesen. Die Pipeline je Kandidat lautet: Envelopegrenzen → Profilverifier → Payloadhash → DTO/Schema → Fachinvarianten → Migration auf Kopie. Unter allen gültigen unterstützten Kandidaten gewinnt die höchste Generation. Gleichstand mit unterschiedlichem Hash ist `SAVE_SPLIT_BRAIN`; `main` wird konservativ bevorzugt und beide Dateien bleiben erhalten.

Falls kein Kandidat gültig ist, darf ein leerer Save nur nach sichtbarer Recoveryinformation und bewusster Nutzerentscheidung entstehen. „Erneut versuchen“, „lokales Backup verwenden“ und erst zuletzt „neuen Spielstand beginnen“ bleiben getrennt. Destruktives Zurücksetzen verlangt Bestätigung.

## 7. Autosave-Policy

| Auslöser | Verhalten |
|---|---|
| Zell-/Batchcommand oder Undo | 500-ms-Debounce, spätestens 2 Sekunden nach erster ungeschriebener Mutation. |
| App geht in Hintergrund / verliert Fokus | sofortiger Snapshot ohne Netzwerk. |
| Level gelöst | logischer Commit vor Zugfahrt und Ergebnisreward. |
| Economy-/Inventaraktion | atomarer Commit vor Erfolgsmeldung. |
| Reward-/Kaufcallback | idempotent verarbeiten und sofort committen. |
| Änderung einer persistenten Operation | sofortiger Commit vor externem Folgeschritt. |
| Einstellung | 500-ms-Debounce; lokale Wirkung sofort. |

Datei-I/O geschieht außerhalb des Renderframes. Quit ist nur Best Effort; Korrektheit hängt nicht davon ab.

## 8. Saveversionen und Migrationen

Jede Version besitzt einen expliziten DTO-Typ und genau einen Migrator `vN -> vN+1`. Migrationen sind rein, deterministisch und greifen nicht auf Netzwerk, Locale oder aktuelle Wanduhr für fachliche Defaults zu. Der aktuelle Writer erzeugt Save v2; Save v1 bleibt ausschließlich Reader-/Migrationsinput.

Vor einer Migration wird die unveränderte Quelle gesichert. Jede Zwischenversion wird einzeln migriert, validiert und roundtrip-serialisiert. Sterne, Ownership, Entitlements, Claimterminals und Ledgerbalance dürfen nicht sinken. Kann ein Pflichtwert nicht neutral hergeleitet werden, stoppt die Migration mit `SAVE_MIGRATION_NEEDS_DECISION`. Downgrade-Schreiben ist verboten.

Golden Tests umfassen immer ursprünglichen Envelope, Hashprofil, kanonische Payloadbytes, ursprünglichen Hash, migriertes Objekt, Zielbytes und Zielhash. Dadurch kann ein fachlich gültiger alter Save nicht allein wegen eines Serializerwechsels als korrupt gelten.

Save v1→v2 führt drei getrennte Migrationen aus. Erstens wird jedes Levelrecord nur über einen historischen Release-Lock auf `{puzzleId, publicPuzzleHash.profile, publicPuzzleHash.sha256}` überführt; das Golden bindet die vollständigen JCS-Hashes der tatsächlichen v1-Quelle, des v2-Ziels und des Release-Locks. Fehlt eine eindeutige Bindung oder stimmen die Dokumentbytes nicht, stoppt `SAVE_LEVEL_IDENTITY_UNRESOLVED`. Zweitens wird `highestReservedOrdinal = nextGenerationOrdinal - 1` gesetzt und die v1-Präfixabdeckung aus aktiven Drafts, Terminalintervallen und ausdrücklich offenen Rewardclaims validiert. Aktive Drafts werden `ACTIVE_DRAFT`; eindeutig gebundene offene Completed-Claims werden `COMPLETION_CLAIM_OPEN`. Ein Completed-Intervall ohne beweisbare Claimauflösung stoppt mit `SAVE_MIGRATION_NEEDS_DECISION`; erst danach dürfen historische Intervalle entfernt werden. Drittens bleiben Legacy-Cosmetics ohne neutrale Provenienzzuordnung unverändert lesbar; es entstehen keine synthetischen Meilensteinclaims. Jeder Stopp erhält die unveränderte Quelle.

## 9. Economycheckpoint, Journal und Kompaktierung

`LedgerCheckpoint` enthält `ledgerContractVersion`, `throughSequence`, `balancePatience`, `entryChainHashSha256` und `sourceSaveGeneration`. `economyJournal` enthält streng sequenzielle Einträge mit `transactionId`, `reasonCode`, `amount`, `subjectId`, Katalog-/Claimbezug und UTC-Diagnosezeit.

Der aktuelle Saldo ist `checkpoint.balancePatience + sum(journal.amount)`. Jeder Command prüft zusätzlich, dass die Hashkette und Sequenzen lückenlos sind.

| Grenze | Vertrag |
|---|---|
| Harte Journalgrenze | höchstens 512 Einträge oder 256 KiB kanonische JSON-Bytes. |
| Kompaktierungsschwelle | ab 384 Einträgen oder 192 KiB beim nächsten erfolgreichen Savecommit. |
| Diagnosefenster | jüngste 64 terminale Einträge bleiben nach Kompaktierung erhalten. |
| Transaktionskollision | gleiche ID/gleicher Inhalt ist No-op; gleiche ID/anderer Inhalt blockiert. |

Kompaktierung ist deterministisch nach `sequence`. Ein entfernter Eintrag wird in Saldo, `throughSequence` und der Hashkette des Checkpoints gebunden. Er darf nur entfernt werden, wenn seine Deduplikationswahrheit in einem terminalen Fachrecord verbleibt:

- Level-, Stern-, Routen- und Rewardclaims in Level-/Progress-/Claimrecords;
- kosmetische Käufe im Inventory;
- IAP im Entitlement-/Operationsrecord;
- Endless-Claims durch Watermark plus begrenztes `openEndless`-Komplement; ein offener Claim darf vor `COMMITTED` oder `CLOSED_NO_REWARD` nicht kompakt terminalisiert werden.
- kosmetische Meilensteinclaims im terminalen Inventory-Ownership-Record.

Ein Eintrag ohne terminale Fachwahrheit blockiert Kompaktierung mit `ECO-COMPACTION-NONTERMINAL`. Checkpointbildung und Journalkürzung erfolgen im selben atomaren Savecommit. Endlosnutzung lässt dadurch weder Journal noch Claim-ID-Liste unbegrenzt wachsen.

### 9.1 Begrenzter Endless State ohne Terminalnutzungsgrenze

`highestReservedOrdinal` ist ein kanonischer UInt64-Dezimalstring ohne führende Nullen, initial `0`. `openEndless` ist die einzige detaillierte fachliche Sammlung und enthält höchstens 20 eindeutig nach Ordinal sortierte Records. Zulässige Zustände und Pflichtdaten sind:

| Zustand | Pflichtdaten | Zulässiger nächster Fachschritt |
|---|---|---|
| `RESERVED_NOT_GENERATED` | Ordinal, E1-ID und vollständiger kanonischer `endless-v1`-Deskriptor | deterministisch generieren und nach vollständiger Prüfung atomar promoten oder abbrechen. |
| `ACTIVE_DRAFT` | Reservation plus öffentlicher Puzzleinput, profilierte Puzzle-/Lösungs-/Proofhashes und Sessionstate | fortsetzen, abschließen oder abbrechen. |
| `COMPLETION_CLAIM_OPEN` | Reservation, Puzzle-ID, Completion-Commit-ID, Reason-Code, fachliche Claim-ID, lokale Operation, `LOCAL_DECISION_PENDING`/`PROVIDER_RESERVED`/`RECONCILIATION_REQUIRED`/`REWARD_CONFIRMED`/`NO_REWARD_CONFIRMED` und gegebenenfalls Provideroperation | lokal ohne Reward terminalisieren, Provideroperation vor SDK-Aufruf reservieren, bestätigten Claim committen, bestätigt ohne Reward schließen oder reconciliieren. |

Für `o` gilt exakt: `open(o)`, wenn ein `openEndless`-Record mit `o` existiert; `terminal(o)`, wenn `1 <= o <= highestReservedOrdinal` und kein offener Record existiert; andernfalls ist `o` unreserviert. Ordinal 0, Duplikate, offene Ordinale über dem Watermark, unzulässige Pflichtfelder oder Descriptor-/E1-Mismatch machen den Save ungültig. Terminalität unterscheidet absichtlich nicht mehr zwischen Complete und Abandon.

`ReserveEndless` commitet ausschließlich `highestReservedOrdinal + 1` und `RESERVED_NOT_GENERATED` gemeinsam. Die Generierung liest nur diesen persistierten Descriptor. Erst schema-, solver- und proofvalidierter Output wird atomar `ACTIVE_DRAFT`; ein Crash wiederholt dieselbe E1-Instanz. Fehlt die Generatorversion, bleibt `ENDLESS_GENERATOR_RECOVERY_REQUIRED` offen. Bei 20 offenen Records liefert jede weitere Reservation `ENDLESS_OPEN_CAPACITY`.

`CompleteEndless` schreibt Completion-/Fortschrittseffekte, persistiert die lokale Claimoperation und ersetzt `ACTIVE_DRAFT` atomar durch `COMPLETION_CLAIM_OPEN` mit `claimStatus: LOCAL_DECISION_PENDING`; der Resume-Payload wird entfernt. `AbandonEndless` entfernt `RESERVED_NOT_GENERATED` oder `ACTIVE_DRAFT` ohne Reward.

Aus `LOCAL_DECISION_PENDING` konkurrieren `SkipEndlessReward` und `ReserveEndlessRewardProvider` über dieselbe `expectedSaveGeneration`. Der lokale Skip entfernt den offenen Record in einem Copy-on-Write-Savecommit, schreibt weder Ledger noch Provider-ID und startet keinen Provideraufruf. Ein Crash vor Commit lässt denselben Record wiederaufnehmbar; ein Crash nach Commit lädt ihn kompakt terminal. Duplicate Skip ist ein No-op. `ReserveEndlessRewardProvider` bindet dagegen eine eindeutige Provideroperation und `PROVIDER_RESERVED` **vor** jedem SDK-Aufruf. Gewinnt diese Reservation, wird Skip ohne Mutation abgewiesen. Gewinnt Skip, muss der Providerpfad nach erneutem Lesen vor jedem externen Aufruf abbrechen. Skip ist außerdem bei `RECONCILIATION_REQUIRED`, `REWARD_CONFIRMED` und `NO_REWARD_CONFIRMED` verboten.

Ein Providerergebnis gilt nur bei exakt passender Claim-, Puzzle-, lokaler Operations- und Provideroperations-ID; eine bereits gebundene Provideroperation ist unveränderlich. `COMMIT_CLAIM` ist nur nach `REWARD_CONFIRMED`, `CLOSED_NO_REWARD` nur nach `NO_REWARD_CONFIRMED` und jeweils mit denselben Bindungen zulässig. `CLOSED_NO_REWARD` bleibt der providerbestätigte Pfad und darf keinen lokalen Skip vortäuschen. Claimcommit, providerbestätigtes Close oder lokaler Skip entfernen den letzten offenen Record. Ein erneuter terminaler Command oder verspäteter Callback liefert `ENDLESS_TERMINAL_DUPLICATE` beziehungsweise redigierten No-op, rekonstruiert keinen Record und schreibt keinen Reward. Terminalintervalle, Terminalcheckpoint, Terminaldetailtail und Skip-ID-Listen sind in Save v2 verboten. Der Diagnose-Ring darf höchstens 64 redigierte Hinweise halten, ist aber nie fachliche Wahrheit. Nach Erschöpfung des UInt64-Raums gilt `ENDLESS_ORDINAL_SPACE_EXHAUSTED`.

## 10. Einmaliger `POST_CLEAR_PATIENCE`-Claim

Die fachliche Claim-ID lautet `reward-claim:POST_CLEAR_PATIENCE:<puzzleId>`. Provider-Reward-ID und lokale Operation-ID sind ausschließlich Audit-/Callbackfelder.

| Zustand | Bedeutung |
|---|---|
| `AVAILABLE` | kein Record; Claim kann reserviert werden. |
| `RESERVED` | genau eine persistierte Operation darf eine Anzeige ausführen. |
| `RECONCILIATION_REQUIRED` | Ausgang nach Crash/Timeout unbekannt; keine zweite Anzeige zulässig. |
| `COMMITTED` | +10 und terminaler Claim wurden atomar persistiert. |

Eligibilityprüfung und `RESERVED` werden in einem Savecommit durchgeführt. Parallele Operationen verlieren auf `expectedSaveGeneration`. Rewardcallback, Claimterminal und Ledgergutschrift werden in einem gemeinsamen Savecommit geschrieben. Der gutzuschreibende Betrag stammt ausschließlich aus der exakt geladenen, release-gelockten `POST_CLEAR_PATIENCE`-Definition des Completionkatalogs; Callback und Command führen keinen Betrag. Ein bestätigtes `CLOSED_NO_REWARD` beziehungsweise `FAILED` gibt die Reservation atomar frei. Ein unbekannter Ausgang bleibt gesperrt, bis Adapterreplay oder eine eindeutige terminale Antwort vorliegt. Späte Callbacks derselben Reservation sind idempotent; Callbacks einer freigegebenen älteren Operation werden quarantänisiert und erzeugen keinen zweiten Grant.

Für Kampagnenlevel kann das Fehlen eines Claimrecords `AVAILABLE` bedeuten. Für Endless gilt enger: Nur ein passender `COMPLETION_CLAIM_OPEN`-Record ist berechtigt. Reserved-, aktive, kompakt terminale oder abgebrochene Ordinale erzeugen aus Recordabwesenheit niemals einen neuen Claim.

## 11. IAP-Verifikation, Grant und Storefinalisierung

Persistente Operationszustände sind:

```text
STARTED -> EVIDENCE_RECEIVED -> VERIFIED -> GRANTED_NOT_FINALIZED -> FINALIZED
                               \-> REJECTED
any non-terminal ambiguity -> RECONCILIATION_REQUIRED
```

„Verifiziert“ bedeutet im client-only Vertrag: Der Storeadapter hat die plattformspezifische signierte Transaktion beziehungsweise Receipt-/Tokenantwort mit den offiziellen Store-/Unity-IAP-Mechanismen lokal geprüft. Logischer Produktkey, Plattformprodukt-ID, Bundle-/Package-ID, Environment, Kaufstatus und Transaktionsreferenz müssen zur Buildkonfiguration passen. Rohreceipt und Token werden nicht geloggt, in Analytics geschrieben oder dauerhaft im Klartext in generischen Diagnosen gespeichert.

Die Reihenfolge ist verbindlich:

1. Belegreferenz als `EVIDENCE_RECEIVED` persistieren.
2. Lokal prüfen; gültig wird `VERIFIED`, ungültig `REJECTED`, unklar `RECONCILIATION_REQUIRED`.
3. `remove_ads` und `GRANTED_NOT_FINALIZED` **atomar** persistieren.
4. Erst danach Google Purchase acknowledge beziehungsweise Apple Transaction finish anfordern.
5. Erfolg als `FINALIZED` persistieren.

Retry für Schritt 4 verwendet denselben Operations-/Transaktionsschlüssel, exponentielle Abstände von 5 Sekunden, 30 Sekunden, 5 Minuten, 30 Minuten und danach höchstens alle 6 Stunden bei Appaktivität. Es gibt keinen aggressiven Hintergrundloop. Timeout oder Offlinezustand entziehen das bereits persistierte Entitlement nicht.

Crash vor Schritt 3 wird durch Store-Replay/Restore erneut validiert. Crash zwischen 3 und 5 dupliziert den Grant nicht und wiederholt nur Finalisierung. Crash nach Storeerfolg, aber vor lokalem `FINALIZED`, wird bei der nächsten Produkt-/Ownershipabfrage abgeglichen. Restore verwendet dieselbe Pipeline. Eine leere oder fehlerhafte Abfrage ist keine Revocation. Nur eine eindeutige verifizierte Refund-/Revocationaussage darf `REVOCATION_CONFIRMED` setzen. Widersprüchliche Antworten werden `RECONCILIATION_REQUIRED`; ein zuvor bestätigtes Werbefrei-Entitlement bleibt konservativ aktiv.

## 12. Tagesstatus und Uhr

`BLOCKER-PROD-002` verhindert weiterhin die Implementierung und Veröffentlichung der Betriebslage-des-Tages-Anspruchslogik. Persistenz reserviert nur einen versionierbaren Record mit bestätigten Claim-IDs und führt keine lokale-/UTC-Regel als Wahrheit ein.

## 13. Kosmetischer Meilensteinclaim

`ClaimMilestoneCosmetic` bindet an den gelockten `cosmetics-v2`-Katalog. Die fachliche ID lautet `cosmetic-milestone-claim:v1:<catalogId>:<itemId>`. Nach positiver Eligibility schreibt `RESERVE_CLAIM` zuerst einen eigenen Copy-on-Write-Snapshot. Die persistierte Reservation enthält Claim-ID, eindeutige `operationId`, fixe `claimGeneration`, Item-ID, `MILESTONE_GRANT`, Katalog-ID/-revision/-hash, Eligibility-Vertragsversion, den über Anforderungen und tatsächlich validierte First-Clear-Puzzle-IDs gebildeten Eligibility-Projektionshash sowie `claimState: RESERVED`.

`COMMIT_CLAIM` lädt genau diese Reservation. Claim-ID, Operation, Generation, Item, Erwerbsart, Katalogidentität und erneut hergeleitete Eligibility-Projektion müssen vollständig übereinstimmen. Fehlende, nur nachträglich eingefügte, nicht eligibility-validierte oder abweichende Reservationen werden abgewiesen. Ein zweiter atomarer Copy-on-Write-Commit entfernt die offene Reservation und schreibt den terminalen Inventoryeintrag mit `grantKind: MILESTONE_CLAIM`, `claimState: COMMITTED` und derselben Provenienz. Es gibt kein Ledgerdelta. Gleiche ID und Projektion ist `ALREADY_OWNED`; dieselbe ID mit anderem Item, Katalog oder Eligibility-Hash ist `COS_MILESTONE_CLAIM_COLLISION`.

Crash vor dem Reservationcommit hinterlässt nichts. Crash danach lädt die Reservation unverändert. Crash vor dem Ownershipcommit lässt sie offen; Crash danach lädt ausschließlich den terminalen Ownershiprecord. Eine Katalogrevision darf ein veröffentlichtes Item unter stabiler ID nicht auf einen anderen Erwerbsmodus oder eine andere Eligibility umdeuten. Tombstones erhalten vorhandenes Ownership.

## 14. Datenschutz und Datenlöschung

Save und lokale Logs liegen im App-Sandboxspeicher und werden nicht automatisch übertragen. „Lokale Daten löschen“ entfernt Save, Backup, Pending, Logs und SDK-lokale optionale Telemetriedaten soweit APIs dies erlauben. Storekäufe bleiben beim Store und können wiederhergestellt werden. Die UI erklärt vor bestätigter Löschung, dass Gameplay-Fortschritt ohne Cloudsave nicht wiederherstellbar ist.

## 15. Tests

Pflicht sind Roundtrip-, Hashprofil-, JCS-Cross-Tool-, unbekanntes-Profil-, Schema-/Profilmigrations-, Truncation-, bad-hash-, pending-write-, backup-, split-brain-, low-disk-, permission-, idempotency-, Claimparallelitäts-, Spätcallback-, Rewardcrash-, IAP-Phasencrash-, Acknowledge-/Finish-Retry-, Restore-, Revocation-, Ledgergrenzen- und Kompaktierungstests. Hinzu kommen Reservation vor Generierung, Crash/Retry mit identischer E1-ID, Promotion zu `ACTIVE_DRAFT`, Completion zu `COMPLETION_CLAIM_OPEN`, lokaler Skip ohne Provider-ID, Skip-Crash/Restart/Duplicate, Skip-versus-Provider-CAS, Claimreconciliation, `CLOSED_NO_REWARD`, Abandon, mindestens 100 aufeinanderfolgende Skip-Terminalisierungen ohne Capacity-Sperre, 10.000 gemischte Terminaltransitionen ohne wachsende Historie, offene Kapazität, v1→v2-Claimmigration sowie atomare, zuvor reservierte Kosmetikclaim-/Kollisionsfälle. Gerätetests unterbrechen die App während Writes und Storeoperationen auf Android und iOS.

## Referenzen

[1]: ../DECISIONS/ADR-019-endless-watermark-und-save-v2.md "ADR-019 – Endless-Watermark und Save v2"
[2]: ../DECISIONS/ADR-020-privacy-lifecycle-und-sdk-grenzen.md "ADR-020 – Privacy-Lifecycle und SDK-Grenzen"
[3]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
[4]: ./GAME_STATE_MODEL.md "Game State Model v0.5"
[5]: ../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md "ADR-021 – Puzzleidentität und versionierte Proofartefakte"
[6]: ../DECISIONS/ADR-024-privacy-bootstrap-fence-und-widerruf.md "ADR-024 – Privacy-Bootstrap-Fence und Widerruf"
[7]: ../DECISIONS/ADR-025-endless-open-lifecycle-und-claims.md "ADR-025 – Endless-Open-Lifecycle und Claims"
[8]: ../DECISIONS/ADR-023-releasekandidat-und-kosmetikclaims.md "ADR-023 – Releasekandidat-Identität und kosmetische Meilensteinclaims"
[9]: ../DECISIONS/ADR-027-endless-no-reward-terminalpfad.md "ADR-027 – Providerfreier Endless-No-Reward-Terminalpfad"
[10]: ../DECISIONS/ADR-028-cosmetics-reservation-binding.md "ADR-028 – Bindende Cosmetics-Claim-Reservation"
