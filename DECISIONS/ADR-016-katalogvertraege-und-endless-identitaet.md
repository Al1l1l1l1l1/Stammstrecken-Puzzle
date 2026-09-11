# ADR-016 – Katalogverträge und Endless-Identität

## Status

**Angenommen**

## Datum

2026-09-08

## Kontext

Der damalige ADR-004 definierte Level-JSON, Schema, Semantik und veröffentlichte Puzzleidentität; diese Grundsätze sind heute vollständig in ADR-021 restatiert. Kampagnenstruktur, Completion-/Fortschrittsdaten, Kosmetika und Preise waren dagegen nur als Dateinamen skizziert. Für kosmetische Käufe fehlte ein atomarer Economyvertrag. Generierte Dauerbaustellenlevel hatten keine reproduzierbare Identität. Außerdem schlossen Domain und Levelschema technisch einen A-B-Pfad durch genau eine Rasterzelle aus, obwohl keine Produktregel dies fordert.

## Entscheidung

Für kuratierte Level gilt heute ADR-021. ADR-016 führte zusätzlich drei getrennte, lokale und versionierte JSON-Kataloge ein:

- `campaign-v1` ist die autoritative Quelle für Season-, Netzabschnitt-, Routen- und Levelreihenfolge sowie Unlockbeziehungen.
- `completion-v1` ist die autoritative Quelle für freigegebene Completion-/Fortschritts-Reason-Codes, bestätigte Geduldspunktebeträge und typisierte Ergebnis-/Kartenreferenzen.
- `cosmetics-v1` ist die autoritative Quelle für kosmetische Item-IDs, Typ, Asset-/Lokalisierungsreferenzen, Veröffentlichungsstatus und `pricePatience`.

Jeder Katalog besitzt `schemaVersion`, stabile `catalogId`, streng steigende `catalogRevision` und einen RFC-8785/JCS-SHA-256-Hash im Release-Lock. Production akzeptiert nur `PRODUCT_APPROVED`; Vertragsfixtures sind ausdrücklich `FIXTURE_ONLY`. Die JSON-Schemata und Validierungsreihenfolge stehen in `CONTENT_CATALOGS.md` und `ARCHITECTURE/schemas/`.

`STP.Application` besitzt `ICampaignCatalog`, `ICompletionCatalog` und `ICosmeticsCatalog`. Adapter liefern immutable DTOs plus Katalog-ID, Revision und Hash. Kataloge mutieren weder Save noch Economy.

Bei einem kosmetischen Kauf ist der aktuell geladene, release-gelockte `cosmetics-v1`-Eintrag die autoritative Preisquelle. Application serialisiert Kaufcommands. Sie prüft Itemstatus, bestehendes Ownership und den aus Checkpoint plus Journal abgeleiteten Saldo. Abbuchung und Inventory-Grant erfolgen in demselben Savecommit unter `cosmetic-purchase:<itemId>`. Bereits besessenes Item und identischer doppelter Command sind erfolgreiche No-ops ohne zweite Abbuchung. Dieselbe Transaktions-ID mit anderem Item, Betrag oder Kataloghash ist ein harter Konflikt. Ein pending Kauf bindet Preis und Kataloghash; eine Katalogrevision ändert ihn nicht rückwirkend. Entfernte Items bleiben als Tombstone auflösbar und bereits besessen.

Ein Dauerbaustellenlevel verwendet den getrennten Vertrag `endless-v1`. Der vollständige Generationsdeskriptor enthält `endlessContractVersion`, `rulesetVersion`, `generatorVersion`, einen vorzeichenlosen 64-Bit-Seed als kanonischen Dezimalstring, `generationOrdinal`, `parameterHashSha256` und die kanonischen relevanten Generatorparameter. Nach Erzeugung kommen profilierte Puzzle-, Lösungs- und Proofhashes hinzu. Seine stabile ID lautet `E1-` plus dem vollständigen kleingeschriebenen SHA-256-Hexwert der JCS-Projektion `{endlessContractVersion,rulesetVersion,generatorVersion,seed,generationOrdinal,parameterHashSha256}`.

Gleiche ID und gleicher Deskriptor referenzieren dieselbe Instanz; gleiche ID mit anderem Deskriptor ist ein fataler Hashkonflikt. Die in der ursprünglichen ADR-Fassung entworfene Intervallretention ist vollständig verworfen und nicht normativ. Deduplikation, Reservation, aktive Drafts und terminale Wahrheit richten sich ausschließlich nach [ADR-019](./ADR-019-endless-watermark-und-save-v2.md); es gibt keine kleine terminale Nutzungsgrenze.

Konkrete Generator-Qualitätsgrenzen bleiben durch `BLOCKER-PROD-003` offen und fail-closed.

Der Level-v1-Vertrag erlaubt technisch einen Pfad aus **einer** Trackzelle, wenn A und B verschiedene Außenanschlüsse derselben Zelle sind, deren Trackform beide Außenports besitzt und alle Randzahlen exakt eins beziehungsweise null abbilden. Rastergröße und Season-1-Contentvorgaben bleiben unverändert. Dies ist eine technische Beseitigung eines unbegründeten Ausschlusses, keine Festlegung, dass Season 1 ein solches Level enthalten muss.

## Begründung

Getrennte Kataloge halten Verantwortungen, Revisionen und Diffs klein. Lokale, release-gelockte Preise unterstützen Offline-First und verhindern Preisrennen. Atomarer Debit-plus-Grant schützt Währung und Ownership. Die Endless-ID enthält alle reproduktionsrelevanten Identitätsparameter und ist klar von Kampagnen-IDs getrennt. Die Ein-Zellen-Regel folgt bereits den sechs zulässigen Gleisformen und erfordert keine neue Rätselregel.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Alle Daten im Level-JSON duplizieren | Erzeugt Drift bei Preisen, Hierarchie und wiederverwendeten Completionreferenzen. |
| Preise in Save oder UI hart codieren | Keine autoritative revisionierbare Quelle und schlechte Auditierbarkeit. |
| Zufällige UUID für Endless-Level | Eindeutig, aber nicht aus Seed/Generatorparametern reproduzierbar. |
| Kampagnen-ID für Endless verwenden | Vermischt endliche redaktionelle Hierarchie mit generierten Instanzen. |
| Ein-Zellen-Pfad weiter verbieten | Würde ohne bestätigte Produktgrundlage eine zusätzliche Regel erfinden. |

## Konsequenzen

Katalogschemata, Beispiele und Cross-Reference-Validator werden Teil der CI. Save-Migrationen bewahren Ownership und pending Preisbindungen. Generatorversionen werden für Diagnose archiviert; volle lokale Deskriptorreproduktion gilt für aktive Drafts. Terminale Deduplikationswahrheit folgt dem konstanten Watermarkvertrag aus ADR-019 und ist nicht von einem Diagnosefenster abhängig. Die Veröffentlichung generierter Level bleibt trotz Identitätsvertrag blockiert, bis das Produkt-Qualitätsprofil bestätigt ist.

## Betroffene Artefakte

`ARCHITECTURE/CONTENT_CATALOGS.md`, `ARCHITECTURE/CONTENT_PIPELINE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, neue JSON-Schemata/-Fixtures und spätere Katalogadapter.

## Validierung

Schema- und Cross-Reference-Tests prüfen Katalog-ID, Revision, Hash, eindeutige IDs, referenzierte Level/Assets/Keys und Productionstatus. Economytests decken Saldo, atomaren Debit-plus-Grant, Duplicate Purchase, Already Owned, Parallelität, Katalogrevision und Migration ab. Endless-Goldens prüfen Identität, Watermark, Resume, Duplicate-Erkennung, Hashkonflikte, 10.000 wechselnde Terminaltransitionen und v1→v2-Migration. Ein Ein-Zellen-Fixture muss Schema, Semantik und Eindeutigkeitsprüfung bestehen.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Die v0.2-Retentionsentscheidung wird durch [ADR-019](./ADR-019-endless-watermark-und-save-v2.md) ersetzt; die `endless-v1`-Identität und die Kataloggrundsätze bleiben angenommen. [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) ergänzt kosmetische Meilensteinclaims.

## Referenzen

[1]: ../ARCHITECTURE/CONTENT_CATALOGS.md "Content Catalogs v0.3"
[2]: ../ARCHITECTURE/LEVEL_DATA_FORMAT.md "Level Data Format v1"
[3]: ../ARCHITECTURE/OPEN_BLOCKERS.md "Offene Blocker nach Architecture v0.3"
