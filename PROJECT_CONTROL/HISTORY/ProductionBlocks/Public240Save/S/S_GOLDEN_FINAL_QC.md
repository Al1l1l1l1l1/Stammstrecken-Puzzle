# Unabhängige S-Goldenprüfung vor Codeccode

**Urteil: PASS_PRECODEC_GOLDEN_CONTRACT_ONLY.** Keine offenen Blocker oder Highs gegen den abschließenden festen V7-Goldensatz. R entscheidet über dessen Annahme vor Codecbeginn. Dieses Urteil ist weder Codec-/Migrator-/Writer-/CAS-/Autosave-PASS noch S-Abschluss.

Geprüfte Endfassung: `S/GOLDENS_PRECODEC_V7.json`, **9.988.353 Bytes**, SHA256 **`ee0ef35bd3dbf03396fc5fd30389b9e75e486f8b08982b2efde3ffec1cba4ed0`**. Enthalten sind **43 vollständige positive Fälle und 91 feste negative Verträge**: 23 Corrupt, 68 Unsupported. Die Negativen bestimmen künftige Expectedantworten einschließlich keinem Ziel und unverändertem Original; ein noch nicht implementierter Codec wurde dafür nicht ausgeführt.

S gehört zu WP042, Anker `8c3481b3602567406ab4ef6f9365249ccb5d3afc`, einziger Parent D `925ca608a961ac26c84385c8da3b949f609df883`. Die bereits separat angenommene vollständige T-Bindung hat SHA256 `25bfd7224f88b4f0256611ae0b91b8d25edea5bd01077a3a51132552ee87c1ad`.

## Tatsächliche unabhängige Prüfungen

| Nachweis | Ergebnis |
|---|---|
| Eigener rein lesender Prüfer, tatsächliches WSL Node **v22.20.0** | **23.178 Checks, 0 Fehler**. Vollständige Envelope-/Payloadstrings und Hashes, geschlossene Records, Safeinteger, GUIDs/UTC, Zellen/Reverse-Undo/History, Quellenbindungen und Fachbeziehungen. |
| Kanonischer vorhandener Venv, **CPython 3.11.13** | **1.223 Byte-/Hashchecks, 0 Fehler**. Alle zehn eindeutigen Quellobjekte gegen die vorhandenen Draft-2020-12-Schemata erfolgreich. |
| CPython 3.14.4 | Ergänzender Crosscheck, dieselben 1.223 Checks und Schemata erfolgreich. Kein Ersatz für 3.11. |
| Node22-Referenzfolge in isolierten ignorierten Kopien | V3_BASE→V3→V4→V5→V6 und abschließend V7 vollständig reproduziert. Jede erzeugte Bundledatei bytegleich mit dem eingereichten jeweiligen Original; V7 exakt obiger SHA. Keine Ausführung gegen Originalausgabeordner. |
| Tatsächlicher Domainexport des Owners | Vollständige exportierte Bytes unabhängig gegen die festen erwarteten Checkpointwerte verglichen; exakt gleich. Harness und Rawexport separat gehasht, siehe unten. |

Primärreceipts liegen neben diesem Bericht: `S_GOLDEN_V7_NODE22.json`, `S_GOLDEN_V7_PYTHON311.json`, `S_GOLDEN_V7_REPRODUCTION.json`. Der maschinenlesbare vollständige Bericht `S_GOLDEN_FINAL_QC.json` bindet Eingänge, eigene Prüfer, Receipts, alle 91 negativen Expectedverträge und Grenzen. Eigene Prüfer sind `S_GOLDEN_REVIEW.js`, `S_GOLDEN_REVIEW_PY.py`, `S_GOLDEN_REPRODUCE.cjs`.

## Vollständigkeit und fachliche Beziehungen

Alle sieben Pflichtfamilien sind vorhanden: V1_FULL_FIRST_CLEAR, V1_FULL_CHECKPOINT, V1_FULL_RETAINED_LEGACY, V1_FULL_ENDLESS, V2_FULL_RESERVED_ROUNDTRIP, V2_LEGACY_ACTIVE_DRAFTS, LEDGER_CONTRACT1_AND_CATALOGSET. Die zusätzlichen vollständigen Fälle decken sämtliche angegebenen Ownership-/Reward-/IAP-/Pendingreward-/OpenEndless-Varianten sowie sechs Privacy-Synczustände jeweils VALID/INVALID ab.

Die geschlossene V1-Linie bindet alle Wurzelbereiche, ursprüngliche vollständige Envelope-/Payloadbytes, Generation7→8, alte Profil-/Katalogidentität, trusted HistoricalProfileBinding, tatsächlich passende Legacybindungen und neutrale Zwischenobjekte. Das zweite unvollständige Puzzle ist als vollständiger gespeicherter Record vorhanden; eigene vollständige V1-/V2-Dokument-, Proof- und Lockbindungen lösen dessen Identität ohne erfundene Fortschrittswerte auf. Nicht aufgelöste Fakten werden nicht durch Defaults ergänzt.

Campaign-v1/v2, Completion-v1 und Cosmetics-v2 sind echte schemaförmige Contractquellen mit getrennten Readprojektionen. Public-/Campaign-/Completionteilhashes und exakte Dreieraggregation stimmen mit CatalogSet und Envelope überein. Use/ApprovalStatus bleiben ContractTest/FixtureOnly; keine Fixturepromotion. Vollständige Cachemembership und Quellprojektionshash werden aus dem gebundenen Campaigngraph und echten First-Clear-Records hergeleitet. Der Kauf stimmt jetzt in Katalogpreis1, Ownershippreis1, stabiler Transaktions-ID und Ledgerdelta−1 überein. Cosmetic-Milestoneprovenienz bindet die tatsächlich vollständig abgeschlossene Route und die exakte bestehende Eligibilityprojektion, ohne zusätzliches Ledgerdelta.

E1 wird ausschließlich aus der unveränderten Sechsfeldprojektion berechnet; vollständige technische Testparameter werden separat gehasht und bleiben an die ausdrücklich testinterne geschlossene Grammatik gebunden. Aktive Public-/Solution-/Proofreferenzen stimmen überein; vollständige Checkpoints und offene Completedclaims bleiben getrennte Varianten. Bestätigte Pendingrewardzustände referenzieren nun passende offene Endlessclaims mit denselben lokalen und Provideroperations-IDs.

Ledger Contract1 besitzt feste Ei-/Hi-/Checkpoint-/Diagnosetail-/autoritative Suffixbytes und einen eigenständigen expliziten Nullfeldvektor. Hi bindet rohe 32 Vorgängerbytes und JCS(Ei); Diagnoseanteile werden nicht erneut zum Saldo addiert. Die vollständige Nullfeldprojektion ist ausdrücklich ein Hashvektor und keine registrierte neue Economyoperation.

Retained Legacyownership, Entitlement, IAP/Reward und Hint/Daily/Privacy/Accessibility haben explizite passive ContractTest-Lesebindungen. Unbekannte Versionen, Policies, Parametergrammatiken, unaufgelöste Claims und unbewiesene Ledgerherkunft bleiben Stopfälle. Daraus folgt keine reale Produktionspolicy oder Behauptung eines veröffentlichten vollständigen V1-Bestands.

Die negative Matrix enthält die konkret geforderten fehlenden/fremden Recordversionen, Pflichtwerte und Policybindungen; fehlende/doppelte Lockbindungen; falsche Dokumentbytes; unveränderte ID mit geändertem Hash; unvollständige Command-/Undo-/PreviouslySet-History; ungeklärte Completedclaims; fremde Ledgeralgorithmen/unbewiesene Voranker; zusätzliche Felder; sämtliche genannten Parameter-/Hash-/E1-Abweichungen. Bei Missing/Duplicate-Lock sind nun auch die tatsächlich festen mutierten Lockbytes und zugehörigen Hashes konsistent. Der absichtlich falsche Dokumentbytefall bleibt als solcher getrennt.

## Domainexport und unveränderte historische Originale

Der Owner führte den vorhandenen Domainreducer mit festen Commands bei 0/100/200/300/400ms und anschließendem Checkpointexport aus. Die unabhängig gelesenen Harnessbytes haben SHA256 `9594a68d1ce7638114973d690f9dbdf9204c19cb0adeaf6f6ae1ddf21d4e9b1b`, der tatsächliche Rawexport `544f0ef5e72a79433d53567161e7d2b7756e64df0c2dc36c32c95e822bfc157e`. Neu berechnete vollständige kanonische Checkpointbytes ergeben **`b0f48d733d8f8ce63cdbf4a00423004de74a7acffc6c0107cf0b93baba904f3b`** und stimmen exakt mit dem festen Expected überein. Der Ownerreceipt bindet V6; der komplette positive Fallbestand ist in V7 nachweislich unverändert. Der Reviewer führte keine eigene native Domaininstanz aus.

Die beiden historischen Endlessfixtures stimmen als volle Originalrawbytes mit den eingebetteten festen Bytes überein: `986307232e6a20952434206e8e0afb408c2b724113d9e9c3491fdd919556c15f` und `f41e5f0a474d219c2814e96c8ec5d918bde630f17fddd07bbddd25a9e57c58ed`. Der unveränderte ganze Fixturecontainer ist als direkter Envelopeinput Corrupt/SAVE_ENVELOPE_FIELDS_INVALID. Die ausdrücklich separate Auflösung der vorhandenen partiellen Sourceform stoppt Unsupported/SAVE_MIGRATION_CHECKPOINT_INCOMPLETE; kein Parserfallback oder erfundener Resume.

Der verworfene Originalsatz `77ef839de5142c55bd2faed0e9a7ebd190523e8e2ac99b0431704081ed2b57bf` einschließlich ursprünglicher Referenz/Binding bleibt byteidentisch erhalten; V3–V6 ebenfalls. Die ursprünglichen konkreten Befunde sind in `S_GOLDEN_INITIAL_QC.md/.json` erhalten und durch V7 fachlich behoben.

Der Reviewer veränderte ausschließlich ignorierte Prüfberichte/-werkzeuge und isolierte Referenzkopien. Keine Quelldatei-, Git-, Unity-, CI-, Retry-, Dispatch- oder Pushaktion. `LocalSaveCodec.cs` war zum Abschluss der Eingangsprüfung noch nicht vorhanden. Tatsächliche Codec-, Migrations-, Writer-, Crash-, Recovery-, Autosave-, Native- und CI-Nachweise sind spätere eigene Implementierungsgates.
