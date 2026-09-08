# Content Catalogs v0.2

## 1. Ziel und Geltung

Neben Leveldaten sind Kampagnenhierarchie, Completion-/Fortschrittsdefinitionen und Kosmetika eigenständige, versionierte Quellverträge. Kein Screen, Save und keine Application-Policy darf Hierarchie, Preise oder Rewardbeträge als Literal duplizieren.

Die drei Beispiele unter `ARCHITECTURE/examples/` sind ausschließlich `FIXTURE_ONLY`. Sie erzeugen keinen Season-1-Produktcontent und legen keine neuen Preise fest. Productionkataloge benötigen redaktionelle beziehungsweise produktseitige Freigabe und Status `PRODUCT_APPROVED`.

## 2. Kanonische Kataloge

| Katalog | Schema | Autoritative Verantwortung |
|---|---|---|
| `campaign-v1` | [`campaign-v1.schema.json`](./schemas/campaign-v1.schema.json) | Season-, Netzabschnitts-, Routen- und Levelreihenfolge sowie Unlockbeziehungen. |
| `completion-v1` | [`completion-v1.schema.json`](./schemas/completion-v1.schema.json) | Completionmomente, Fortschritts-Reason-Codes und bestätigte Geduldspunktebeträge. |
| `cosmetics-v1` | [`cosmetics-v1.schema.json`](./schemas/cosmetics-v1.schema.json) | Kosmetische Item-IDs, Typen, Status, Asset-/Textreferenzen und Geduldspunktepreise. |

Jeder Katalog besitzt `schemaVersion`, `catalogId`, `catalogRevision` und `approvalStatus`. Die Quelldatei wird nach RFC 8785 kanonisiert; der SHA-256 des vollständigen Katalogobjekts steht im Release-Lock. Eine Revision ist innerhalb derselben `catalogId` streng monoton. Dieselbe ID/Revision mit anderem Hash ist `CAT-REVISION-HASH-CONFLICT`.

## 3. Identität und Status

| Regel | Vertrag |
|---|---|
| Katalog-ID | Stabil für dieselbe logische Kataloglinie; Schemawechsel verwendet eine neue Schema-/Dateilinie. |
| Eintrags-ID | Innerhalb und über Revisionen derselben Linie eindeutig; nie für eine andere Bedeutung wiederverwenden. |
| `FIXTURE_ONLY` | Nur Architektur-/Contracttests; Productionimport ist verboten. |
| `DRAFT` | Authoring und Review; Productionimport ist verboten. |
| `PRODUCT_APPROVED` | Darf nach allen Cross-Reference- und Release-Lock-Prüfungen importiert werden. |
| Tombstone | Entfernt Darstellung/Neukauf, bewahrt aber ID-Auflösung, Save-Ownership und Migrationswissen. |

Season-1-Struktur wird zusätzlich gegen die bestätigten fünf Netzabschnitte, je vier Routen und je zwölf Meldungen validiert, sobald der Content-Work-Package-Abschluss den vollständigen Katalog verlangt. Ein Fixture darf bewusst kleiner sein und wird niemals als Productionkatalog akzeptiert.

## 4. Application-Ports

Alle Ports werden von `STP.Application` besessen. Adapter liefern unveränderliche provider- und Unity-freie Read Types.

| Port | Operationen | Darf nicht |
|---|---|---|
| `ICampaignCatalog` | Katalogidentität, Hierarchie, Reihenfolge und Unlockdefinitionen lesen. | Fortschritt schreiben oder Sternepflicht erfinden. |
| `ICompletionCatalog` | Completionmoment und Rewarddefinition nach stabilem Key liefern. | Ledger mutieren oder Rewardberechtigung entscheiden. |
| `ICosmeticsCatalog` | Item, Status, Preis und Referenzen nach stabiler ID liefern. | Saldo prüfen, Kauf committen oder Ownership vergeben. |

Jede Antwort trägt `catalogId`, `catalogRevision` und `catalogHashSha256`. Application verarbeitet nur Kataloge, deren Hash im lokalen Build-/Content-Manifest gelockt ist.

## 5. Cross-Reference-Vertrag

Die Validierung erfolgt deterministisch in dieser Reihenfolge:

1. UTF-8-JSON ohne Duplicate Keys parsen und Größenlimits prüfen.
2. Jede Datei gegen ihr Draft-2020-12-Schema validieren.
3. Kataloginterne ID- und Reihenfolgenregeln prüfen; der Unlockgraph einschließlich impliziter Elternabhängigkeiten muss selbstreferenzfrei, azyklisch und vollständig von mindestens einem `ALWAYS`-Einstieg erreichbar sein.
4. `campaign-v1`-Levelreferenzen gegen den validierten Levelkatalog auflösen.
5. Level-`completion`-Referenzen gegen `completion-v1` auflösen.
6. Completion-Map-/Trainreferenzen gegen typisierte Asset-/World-Manifeste auflösen.
7. Cosmetics-Assetadressen und Lokalisierungsschlüssel auflösen.
8. Reward-Reason-Codes und Beträge gegen bestätigte Produktwerte prüfen; fehlende oder abweichende Werte sind Fehler, keine Defaults.
9. Kataloge nach RFC 8785 hashen und gegen Release-Lock vergleichen.
10. Status `PRODUCT_APPROVED` als letztes Productiongate prüfen.

Diagnosen sind nach Katalog-ID, JSON-Pointer und Code sortiert. Keine generische Production-Fallback-ID verdeckt eine fehlende Referenz.

## 6. Kampagnen- und Fortschrittsvertrag

`campaign-v1` speichert ausschließlich Struktur und Unlockvoraussetzungen. Die Application-Policy berechnet Fortschritt aus `LevelProgressRecord`; der Katalog speichert keinen veränderlichen Spielerstatus. Jede Section hängt implizit von ihrer Season, jede Route von ihrer Section und jedes Level von seiner Route ab. Eine explizite Referenz auf das eigene Subject, ein Unlockzyklus oder ein von keinem `ALWAYS`-Einstieg erreichbares Subject ist ungültig. Die zulässigen Unlocktypen der ersten Version sind bewusst geschlossen:

- `ALWAYS` für Einstiegspunkte;
- `ALL_FIRST_CLEARS` für eine explizite Menge stabiler Subjects.

Sterne dürfen nicht als Kampagnenzugangsvoraussetzung modelliert werden. Betriebsrevision und Dauerbaustelle werden nach vollständigem Season-Erstabschluss aus Fortschrittsrecords freigegeben, nicht durch ein manuell gesetztes Saveboolean.

`completion-v1` trennt drei Dinge: den fachlichen `reasonCode`, den bestätigten Geduldspunktebetrag und die Präsentationsreferenz. Ein Rewarddefinitionseintrag ist keine Berechtigung. Application prüft Modus, Fortschritt und Claimstatus, bevor sie einen Ledgercommit anfordert.

## 7. Kosmetikkauf als atomare Transaktion

Der release-gelockte `ICosmeticsCatalog` ist die autoritative Preisquelle. Save, UI und Analytics sind keine Preisquelle.

`PurchaseCosmetic(commandId, itemId, expectedSaveGeneration, expectedCatalogHash)` folgt exakt:

1. Command-ID, Savegeneration und Kataloghash prüfen.
2. Item muss `ACTIVE` sein; `HIDDEN` und `TOMBSTONE` sind nicht neu kaufbar.
3. Bereits bestehendes Ownership liefert `ALREADY_OWNED` als erfolgreichen No-op ohne Abbuchung.
4. Preis aus exakt diesem Katalogsnapshot lesen.
5. Saldo aus `LedgerCheckpoint + Journal` ableiten und ausreichende Geduldspunkte prüfen.
6. In einem neuen unveränderlichen Save-Snapshot gleichzeitig negativen Ledger-Eintrag und Inventory-Ownership mit `transactionId = cosmetic-purchase:<itemId>` erzeugen.
7. Snapshot atomar committen; erst danach Erfolg und Präsentationscue emittieren.

Ein Crash vor dem Savecommit verändert weder Saldo noch Ownership. Ein Crash danach lädt beide Änderungen. Dieselbe Transaktions-ID mit identischem Item, Preis und Kataloghash ist ein No-op. Dieselbe ID mit abweichendem Inhalt ist `ECO-TRANSACTION-ID-COLLISION` und blockiert den Commit. Parallele Commands werden über erwartete Savegeneration serialisiert; höchstens einer kann committen.

## 8. Katalogrevision, Ownership und Migration

Ein pending Kosmetikkauf bindet `itemId`, `pricePatience` und `catalogHashSha256`. Eine spätere Katalogrevision verändert eine bereits reservierte Transaktion nicht rückwirkend. Da der lokale Kaufcommit synchron und ohne externen Dienst erfolgt, darf ein pending Zustand nur für Save-Recovery bestehen und wird beim nächsten Start idempotent abgeschlossen oder vollständig verworfen.

Bereits besessene Items bleiben unabhängig von `ACTIVE`, `HIDDEN` oder `TOMBSTONE` nutzbar, solange das erforderliche Asset im unterstützten Build vorhanden ist. Eine ID wird nie einer anderen Kosmetik zugeordnet. Ein Assetentfall benötigt Tombstone, Fallbackdarstellung, Save-Migration und ausdrückliche Inhaltsfreigabe; Ownership wird nicht gelöscht.

Katalogschemamigrationen sind reine Funktionen `vN -> vN+1`. Kann Preis, Identität oder Referenz nicht neutral hergeleitet werden, stoppt die Migration. Save-Migrationen erhalten die zuletzt gelockte Katalogidentität, Inventory-IDs und abgeschlossene Kauftransaktionen.

## 9. Größen- und Robustheitsgrenzen

Parser begrenzen Dateigröße, Verschachtelung, Arraylängen und Stringgrößen vor Objektkonstruktion. Duplicate Keys, unbekannte Eigenschaften und unbekannte Enumwerte sind Fehler. Kataloge sind lokale Buildinputs; dennoch werden sie wie untrusted Input behandelt. Runtime lädt nur das bereits validierte, deterministisch erzeugte Artefakt.

## 10. Tests und Release-Gates

Pflichtfälle umfassen Schemafehler, Duplicate IDs, falsche Reihenfolge, Unlock-Selbstreferenz, Unlockzyklus, unerreichbares Subject, gebrochene Level-/Completion-/Asset-/Localization-Referenzen, nicht freigegebenen Status, Hashkonflikt, Preisabweichung, unzureichenden Saldo, Duplicate Purchase, Already Owned, parallele Commands, Crash vor/nach Commit, Tombstone, Katalogrevision und Save-Migration.

Der eingecheckte Architekturvalidator prüft die drei Fixtures gegen ihre Schemata und zentrale Cross-References. Der Produktionsvalidator im späteren Content-Work-Package verwendet dieselbe Reihenfolge und Diagnosecodes.

## Referenzen

[1]: ../DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md "ADR-016 – Katalogverträge und Endless-Identität"
[2]: ./CONTENT_PIPELINE.md "Content Pipeline v0.2"
[3]: ./PERSISTENCE.md "Persistence v0.2"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/13_Oekonomie_und_Monetarisierungs_Balancing.md "Stammstrecken-Puzzle – Ökonomie- und Monetarisierungs-Balancing"
