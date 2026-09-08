# ADR-014 – Save-Kanonisierung und Ledgerkompaktierung

## Status

**Angenommen**

## Datum

2026-09-08

## Kontext

ADR-006 hat den lokalen atomaren Offline-First-Snapshot korrekt als Gameplay-Wahrheit festgelegt. Der Ausdruck „kanonischer Payload“ ließ jedoch Encoding, Schlüsselreihenfolge, Unicode, Zahlen und Serializerkompatibilität offen. Ein späterer Serializerwechsel könnte deshalb einen fachlich gültigen älteren Save fälschlich als korrupt bewerten. Außerdem war das Economy-Ledger als begrenzt bezeichnet, ohne Grenze oder deterministische Kompaktierung.

## Entscheidung

Der lokale atomare Snapshot, Backups, sequenzielle Migrationen und die Trennung vom Store-Entitlement bleiben erhalten. Jeder Save-Envelope trägt zusätzlich das geschlossene Feld `payloadHashProfile`. Der initiale Wert lautet **`STP-SAVE-JCS-1`**.

Für `STP-SAVE-JCS-1` sind die Hashbytes exakt die UTF-8-Ausgabe des JSON Canonicalization Scheme nach RFC 8785 über das JSON-Objekt `payload`. Es gibt keine Byte Order Mark und kein abschließendes Zeilenende. Objektschlüssel werden rekursiv nach RFC 8785 sortiert; Arrayreihenfolgen bleiben erhalten. Strings werden nicht Unicode-normalisiert. Nicht-I-JSON-konforme Strings, ungültige Surrogate, Fließkommazahlen, NaN und Infinity sind im Savevertrag verboten. Alle numerischen Savewerte sind vorzeichenbehaftete Ganzzahlen im sicheren interoperablen Bereich `[-9007199254740991, 9007199254740991]`; größere technische Werte werden als validierte Dezimalstrings gespeichert. `payloadSha256` ist der kleingeschriebene Hexwert von `SHA-256(JCS(payload))`.

Der Profilname ist die **Serializer-Vertragsversion**, nicht die Version einer Bibliothek. Eine Implementierung darf intern wechseln, solange Golden Tests byteidentisch bleiben. Ein neues Hashverfahren erhält einen neuen Profilnamen und einen expliziten Reader für jedes noch unterstützte alte Profil. Beim Laden wird zuerst anhand von `payloadHashProfile` der ursprüngliche Verifier gewählt. Erst nach erfolgreicher Prüfung, DTO-Validierung und fachlicher Migration wird der Payload mit dem neuen Profil als neue Savegeneration geschrieben. Ein unbekanntes oder fehlendes Profil ist `SAVE_HASH_PROFILE_UNSUPPORTED`, nicht `SAVE_CORRUPT`; der Kandidat bleibt unangetastet und darf keinen automatischen leeren Spielstand auslösen.

`saveSchemaVersion` versioniert die Payloadform unabhängig vom Hashprofil. Schema- und Hashprofilmigrationen sind reine, sequenzielle Schritte mit Golden Input, ursprünglichen Hashbytes, migriertem Payload und neuem Hash. Da vor Architecture v0.2 kein Produktions-Save veröffentlicht wurde, beginnt der erste Produktions-Scaffold mit `saveSchemaVersion: 1` und `STP-SAVE-JCS-1`; dennoch ist der Leser von Beginn an profildispatchend.

Das Economy-Ledger besteht aus einem `LedgerCheckpoint` und einem nachfolgenden Journal. Der Checkpoint enthält `ledgerContractVersion`, `throughSequence`, `balancePatience`, `entryChainHashSha256` und `sourceSaveGeneration`. Das Journal enthält höchstens 512 Einträge oder 256 KiB kanonische JSON-Bytes, je nachdem welche Grenze zuerst erreicht wird. Bei 384 Einträgen oder 192 KiB wird im nächsten erfolgreichen Savecommit deterministisch kompaktiert. Kompaktierung ist nur zulässig, wenn jede betroffene idempotente Claim-/Kaufwahrheit bereits in ihrem autoritativen Fachrecord terminal persistiert ist. Der neue Checkpoint übernimmt Saldo, letzte Sequenz und eine Hashkette über die entfernten Einträge; die jüngsten 64 terminalen Einträge bleiben als Diagnosefenster erhalten.

Dauerhaft notwendige Deduplikation wird nicht aus dem kompaktierten Journal rekonstruiert. Kampagnenclaims liegen in endlichen Level-/Fortschrittsrecords, kosmetische Käufe im Inventory, IAP im Entitlement-/Operationszustand und sequenzielle Dauerbaustellenclaims in terminalen Ordinal-Intervallen. Ein Eintrag ohne terminale Fachwahrheit blockiert die Kompaktierung. Dieselbe Transaktions-ID mit anderem Inhalt bleibt ein harter Konflikt.

## Begründung

RFC 8785 liefert einen extern normierten, sprachübergreifenden Bytevertrag. Die explizite Profildispatchregel verhindert, dass ein Serializerwechsel alte Daten scheinbar korrumpiert. Checkpoint plus begrenztes Journal hält den Snapshot endlich, ohne Deduplikationswahrheit zu verwerfen oder Analytics als Ersatz zu verwenden.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Bibliotheksspezifisches `JsonConvert.SerializeObject` | Reihenfolge und Escaping wären an Konfiguration und Bibliotheksversion gebunden. |
| Hash über die ursprünglichen Dateibytes | Jede harmlose Formatänderung würde den Hash ändern und Migration erschweren. |
| Unbegrenztes Eventledger | Einfach, aber Savegröße wächst bei Langzeitnutzung ohne Grenze. |
| Bloom-Filter für Claims | Begrenzt, aber False Positives könnten legitime Rewards verhindern. |
| Backendledger | Für den bestätigten Offline-First-Umfang unnötig und nicht beschlossen. |

## Konsequenzen

Save-DTOs dürfen nur die festgelegten JSON-Wertetypen enthalten. Jeder Hashprofilwechsel benötigt Reader, Migration und Cross-Tool-Goldens. Ledgerkompaktierung benötigt Fachrecords mit terminalem Claimstatus. Ein unbekanntes Profil führt zu Recoveryinformation statt Datenverlust.

## Betroffene Artefakte

`ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/TEST_STRATEGY.md`, der eingecheckte Architekturvalidator und spätere Save-Serializer/Migratoren.

## Validierung

Python- und Node-Implementierungen erzeugen aus denselben Save-Goldens exakt dieselben Bytes und SHA-256-Werte. Tests prüfen Schlüsselreihenfolge, verschachtelte Objekte, Nicht-ASCII, unterschiedliche Unicode-Normalformen, Escapes, Grenzinteger, ungültige Surrogate, Profildispatch, Schema-/Profilmigration, Kompaktierungsgrenzen, Deduplikation und Crash zwischen Checkpointbildung und atomarem Commit.

## Ersetzt / ersetzt durch

Ersetzt [ADR-006](./ADR-006-lokale-persistenz-und-offline-first.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
[2]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.2"
[3]: ../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md "WP-002 – Architecture-v0.2-Korrekturen"
