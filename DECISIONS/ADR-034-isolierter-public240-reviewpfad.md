# ADR-034 – Isolierter Public240-Reviewpfad

## Status
**Angenommen** – technische Rolle R/Codex gemäß MANUS-2026-10-19-01 §5 nach getrennter Vorcode-Review PASS durch /root/t_boundary_review. [Annahme und Prüfbindung](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_ADR_DECISION.md). Keine allgemeine Produkt-/Geschäftsführerbefugnis.

## Datum
2026-10-19

## Kontext
MANUS-2026-10-19-01 §§4–6 beauftragt einen nichtproduktiven Zugriff auf genau 240 bestehende Rätsel. ADR033 bleibt auf zwei eigene transiente QA-Puzzles begrenzt. WP026/PR21 ist keine angenommene Produktionskampagne. WP040 verankert diese Fachentscheidung vor Code.

## Entscheidung
1. Public240 projiziert ausschließlich puzzleId, rulesetVersion, grid, endpoints, rowCounts, columnCounts und publicPuzzleHash. Quelle ist unveränderlich Commit dc1652befd7545b4c2a2dd0580f35a4e8e05d72d, genau 240 level-v2-Blobs. Ein Offline-Gitobjektprojektor liest jeden Quellblob, prüft dessen gebundene Quellenidentität, bildet eine geschlossene Allowlist und berechnet die öffentliche semantische Identität mit dem vorhandenen JCS-Profil. Kein Merge/Copy ganzer Leveldokumente. Ein eigener Reviewlock bindet Quellcommit, Pfade/Blob-IDs/Bytehashes und exakt die projizierten Payloadbytes; Quellblobs, Lösungen, Proofs und Notizen werden ausschließlich im Repositoryaudit referenziert, nicht als Playerassets kopiert. Die Spielerdatei trägt nur öffentliche Projektionen; auch keine Lösungshashes, Qualitätsmetriken, Text-/Completion-/Assetreferenzen oder interne Notizen als Payload.
2. Der Loader prüft begrenztes striktes JSON, geschlossene Properties, exakt 240 eindeutige Puzzleidentitäten und alle öffentlichen Hashes/Domainmappings. Er führt keinen Solver aus. Vorhandene WP026-Nachweise bleiben an ihrem Stand gültig; sie werden nicht pauschal wiederholt. Zwei deterministische Projektorläufe müssen identische Bytes liefern. Negative Tests injizieren zusätzliche Properties und veränderte Identitäten.
3. Eigene neue QA-Szene Public240Review mit explizitem Inspectorentry, neuem technischen Picker, UI-Keys und serialisiertem TextAsset. Sie nutzt die vorhandenen Domaincommands/Grid-/Trainfähigkeiten über Application-Readtypes. Nur für diese neue Szene wird die eigene serialisierte neutrale de-DE-QA-JSONquelle in eine isolierte Unity-Localization-In-Memory-StringTable überführt; alle sichtbaren Texte einschließlich Formatwerte werden über eigene stabile Keys aufgelöst, fehlende deutsche Keys sind Fehler. Dies ist eine neue eng benannte Ausnahme ausschließlich für Public240Review; ADR033 wird nicht als Erlaubnis dieses neuen Entry ausgelegt. Kennzeichnung: interner Review, keine Produktkampagne; alle 240 sind unabhängig vom Produktunlock auswählbar. Die bestehenden transienten Zwei-Puzzle-Tests behalten ihren getrennten Entry. Keine Loader-Fallbacks aus Fixtures, Testassemblies oder DRAFT-Produktkatalogen.
4. Für den späteren integrierten Saveablauf ist das Profil fest isoliert: Verwendung IsolatedReview; eigener logischer Profilnamensraum qa-public240-v1; Katalogidentität aus dem gebundenen Reviewlock; eigenes Unterverzeichnis qa-public240-v1/<review-catalog-hash>/ unter persistentDataPath, dessen Pfad ausschließlich Persistence besitzt. Ein Produktionsrepository lehnt diesen Profil-/Katalograum ab und umgekehrt. Es existiert kein Export-/Import-/Mergepfad zu Produktprofilen. Keine Produktionsclaims, Rewards oder Entitlements durch Review; angezeigter Grundfortschritt/Saldo ist ausdrücklich QA und verbleibt ausschließlich im Reviewprofil.
5. Ein QA-Kampagnenadapter liefert die aus stabilen IDs gebundene Reviewstruktur und einen technisch gelockten Review-Completion-Snapshot mit ausschließlich bestätigten Grundwerten 15/+5/+10/60/150. Das sind keine PRODUCT_APPROVED-Kataloge und kein Zugriff durch den produktiven Loader. Produktionsfähige Policy wird separat gegen eigene gelockte Contracttestkataloge geprüft; Testdaten leben nur in Tests. Die QA-Adapter sind eigene Runtime-QA-Quellen mit ausdrücklicher ADR-Freigabe, keine Laufzeitbindung an Fixtures. Null-Sternschwellen bleiben null. Frei wählbare Reviewinputs ändern keine produktiven Unlockbeziehungen.
6. Productionimport/IsRuntimeImportable/IsReleaseLockEligible/Release-Lock/PRODUCT_APPROVED bleiben unverändert fail-closed. Buildentry nur dev/qa; staging/production lehnen diesen Entry ab. Bestehende Buildgates/Composition-Smoketests bleiben vollständig erhalten.

## Begründung
Die geschlossene öffentliche Projektion erhält Rätselidentität und macht vorhandene Arbeit bedienbar, ohne internes Lösungswissen oder Produktionsannahme zu transportieren. Die physische und fachliche Saveisolation verhindert eine spätere Progress-/Claimpromotion.

## Betrachtete Alternativen
Kompletter WP026-Untermerge/Level-v2-Payload würde internes Wissen ausliefern und ist verboten. Produktionsloader für DRAFT würde Gates lockern. Verbleib bei zwei transienten Puzzles erfüllt den Auftrag nicht. Ungebundene Handkopien verlieren Reproduzierbarkeit.

## Konsequenzen
Neue QA-Daten/Loader/Picker/Szene gehören A; Saveadapter/-profile S; bestehende Build-/Lifecycle-/UI-Transaktionsverdrahtung I. Eigene öffentliche Daten benötigen keine neue allgemeine Solverprüfung. Menschliche Rätseltests bleiben echte spätere Arbeit. Der isolierte QA-Fortschritt ist kein Produktclaim.

## Betroffene Artefakte
WP040 und spätere eigene A/S/I-WPs; Public240-Projektor, geschlossenes Playerdatenasset/Reviewlock, PublicPuzzleInput/CatalogIdentity, QA-Loader/-Katalogadapter/-Szene, Persistenceprofil, PlayMode-/Leak-/Boundarytests. Konkrete Dateien vor Fachcode in eigenen Manifesten.

## Validierung
Vor Annahme getrennte technische Review dieses Entwurfs. T: providerfreie immutable Schnittstellen, unveränderte Assemblykanten und geschlossene Registry. A: 240er-Projektions-/Hash-/Leak-/Doppelprojektions-/UI-/Layouttests, echte Grafik und reguläre CI. S: Profil-/Katalogmismatch und fehlgeschlagene Recovery fail-closed. I: native Auswahl und Lösung→Commit→Fahrt→Result sowie Restart/Restore; unabhängige eingefrorene Reviews und tatsächliche integrierte CI.

## Ersetzt / ersetzt durch
Ergänzt [ADR-033](./ADR-033-interne-transiente-spielschleife.md) ausschließlich um einen getrennten neuen Reviewentry. Dessen Fachkörper und transiente Grenze bleiben erhalten. Ergänzt die Produktionsverträge aus ADR011/016/021 ohne deren Gates zu ersetzen. Angenommen am 19.10.2026; kein Nachfolger.
