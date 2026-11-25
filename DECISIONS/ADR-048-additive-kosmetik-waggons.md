# ADR-048 – Additive kosmetische Waggons

## Status

**Angenommen** – durch den delegierten Ownerauftrag vom 25.11.2026; technische Umsetzung und Integration sind gezielt zu prüfen. Keine behauptete Unity-, Produkt- oder Releaseabnahme.

## Datum

2026-11-25

## Kontext

Der ausdrücklich erweiterte Fahrzeugkatalog braucht neben TRAIN_VARIANT, LIVERY und OPERATING_OBJECT den Typ WAGON. Bestehende IDs, Preise, Eigentumsbelege und Erwerbsarten bleiben erhalten. Die konkret vorgegebenen IDs ergeben 29 Items (7/10/9/3), nicht die im Auftrag ebenfalls genannte Summe 30; kein zusätzlicher Gegenstand wird erfunden.

## Entscheidung

1. cosmetics-v2 und der stringbasierte C#-Vertrag kennen zusätzlich `WAGON`. Waggons sind ausschließlich `PATIENCE_PURCHASE` mit positivem `pricePatience`; kein Echtgeld-, Default- oder Meilensteinerwerb.
2. Auswahl bleibt eine Liste eigener stabiler IDs: höchstens ein TRAIN_VARIANT und eine LIVERY, beliebig viele OPERATING_OBJECT und höchstens drei WAGON. Waggons sind additiv und einzeln abwählbar.
3. Speichern nutzt unverändert LocalInventory.SelectedItemIds und PurchasedOwnership.ItemType mit dem Wert `WAGON`. Keine neue Saveversion oder zusätzliche Pflichtfelder. Der bestehende Schema-1-Legacyreader bleibt auf die tatsächlich damaligen drei Typen beschränkt; er erfindet keinen historischen Waggonbesitz.
4. Katalogrevision steigt von 1 auf 2. Historische Ownership-Katalogbindungen und Kaufpreise werden nicht auf aktuelle Werte umgeschrieben. Der passive RetainedCosmeticsHistory-Reader erhält genau die originalen Revision-1-Bytes und bestehenden Hashes des internen Kandidaten und der Runtimefixture; kein bisher nicht veröffentlichter Produktionskatalog wird erfunden. Bestehende Hashprüfung bleibt strikt; Abschluss-Neubindung der Quellen/Locks gehört der Integration.
5. Kein Eingriff in Rätsel, Laufzeituhr, Währungserzeugung, Provider oder Präsentationsimplementation. Kauf erfolgt über denselben atomaren Geduld-/Ownership-Commit wie bisher.
## Begründung
Der Eigentümerauftrag verlangt kaufbare Waggons neben den Zugmaschinen. Ein eigener Typ hält Auswahlregeln (höchstens drei, additiv) und Erwerbsart (nur Geduld) prüfbar und ändert weder Saveversion noch Rätselregeln.
## Betrachtete Alternativen
Waggons als OPERATING_OBJECT zu führen wurde verworfen, weil sich Mehrfachauswahl und Obergrenze dann nicht von Objekten trennen lassen. Eine neue Saveversion wurde verworfen, weil der bestehende Ownership-Typ als String genügt.
## Konsequenzen

Importer, aktuelle passive Katalogreader, Lokalisierung, Assetreferenzen und Tests unterstützen den neuen Typ. Presentation erhält ihn über CosmeticDefinition.Type, ILockedCosmeticsCatalog.Items/TryGet sowie LocalInventory.SelectedItemIds/Ownership. Altstände ohne Waggons bleiben unverändert lesbar; neue Waggonstände benötigen den erweiterten Reader.
## Betroffene Artefakte
Content/Catalogs/s1/cosmetics-v2.json, ARCHITECTURE/schemas/cosmetics-v2.schema.json, Kosmetik-Policy, SaveMigration, Importer, Locale, RailwayAssets.json, Tests.
## Validierung

Gezielt prüfen: drei gleichzeitige Waggons neben Zug/Lackierung/Objekten; Zug-/Lackierungs-Konflikte und vierter Waggon abgewiesen; ausschließlich Geduldskauf mit exaktem Debit und idempotentem Replay; Save-Roundtrip; Migration ohne erfundenen Besitz. Unity-/Geräteprüfung und Quellenlock-Neubindung erfolgen getrennt.

## Ersetzt / ersetzt durch

Ergänzt die Kosmetiktypen und Auswahlregel; bestehende ADR-028-Claim-, Provenienz- und Transaktionsverträge bleiben unverändert.
## Ersetzt / ersetzt durch
Ergänzt [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) um den Typ WAGON; ersetzt keine Entscheidung.
