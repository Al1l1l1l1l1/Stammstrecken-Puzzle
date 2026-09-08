# ADR-006 – Lokaler atomarer Save als Offline-First-Wahrheit

## Status

**Ersetzt**

## Datum

2026-09-07

## Kontext

Rätsel, Kampagnenfortschritt und kosmetische Ökonomie müssen ohne Netzwerk funktionieren. Der Datenumfang ist klein. Es gibt zum Launch keine bestätigte Konto-, Login- oder geräteübergreifende Synchronisationsentscheidung. Die Werbefrei-Option muss über Store-Wiederherstellung erneut herstellbar sein.

## Entscheidung

Der Launch verwendet einen **lokalen, versionierten JSON-Snapshot** als autoritative Spielstandquelle. Der Snapshot umfasst Profil, Level-Fortschritt, Währungsledger, kosmetische Freischaltungen, Einstellungen, aktive Puzzle-Entwürfe, begrenzte Undo-Diffs und verarbeitete externe Transaktions-IDs.

Jede Änderung wird in dieser Reihenfolge geschrieben: kanonisches JSON in temporäre Datei, flush, Prüfsumme, vorhandenen Hauptstand als Backup rotieren und temporäre Datei atomar zum Hauptstand umbenennen. Beim Start werden Hauptstand, Backup und nur vollständig geschriebene temporäre Kandidaten nach Version und Prüfsumme bewertet. Migrationen sind sequenziell, idempotent getestet und sichern den unveränderten Vorgänger.

Es gibt in Architecture v0.1 **keine Cloudpflicht und kein Backend**. `ISaveSyncPort` bleibt als nicht aktivierter Erweiterungspunkt bestehen. Gameplay-Fortschritt wird nicht über Analytics rekonstruiert. Der nicht konsumierbare IAP-Anspruch `remove_ads` wird zusätzlich aus dem Store wiederhergestellt; lokaler Cache beschleunigt den Offline-Start, ersetzt aber nicht den Storebeleg.

Konflikte werden nie still durch Zeitstempel entschieden. Eine spätere Cloudfunktion benötigt ein eigenes ADR und eine bestätigte Produktentscheidung zu Konto-UX, Datenschutz, Konfliktregeln und Gerätewechsel.

## Begründung

Ein einzelner kleiner Snapshot ist transparenter und robuster als eine eingebettete Datenbank. Offline-First schützt den Rätselkern vor Dienststörungen. Die bewusste Cloud-Abgrenzung verhindert, dass Architektur ungeklärte Konto- und Datenschutzentscheidungen vorwegnimmt.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| SQLite | Für den kleinen, zusammenhängenden Zustand unnötige Abhängigkeit und Migrationskomplexität. |
| PlayerPrefs | Keine Transaktionen, schwache Struktur und ungeeignet für versionierte Spielstände. |
| Unity Cloud Save als Pflicht | Benötigt Konto-, Datenschutz- und Konfliktentscheidungen, die nicht bestätigt sind. |
| Plattformclouds direkt | Unterschiedliche Semantik auf iOS und Android und kein klarer Cross-Platform-Vertrag. |

## Konsequenzen

Deinstallation oder Geräteverlust kann Gameplay-Fortschritt ohne spätere Cloudfunktion entfernen; dies ist als Produktlücke dokumentiert. Save-Schreibfehler dürfen die aktive Session nicht crashen, müssen aber sichtbar diagnostiziert und erneut versucht werden. Manipulationsschutz ist nicht zugesichert; es gibt keine kaufbare Währung oder Gameplay-Vorteile.

## Betroffene Artefakte

`ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/MOBILE_SERVICES.md` und spätere Save-Migratoren.

## Validierung

Golden-Migrations-, Stromausfall-, Korruptions-, idempotente Transaktions- und Restore-Tests prüfen Hauptstand, Backup und alle Fehlerpfade auf Android und iOS.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ersetzt durch [ADR-014](./ADR-014-save-kanonisierung-und-ledgerkompaktierung.md).

## Referenzen

[1]: ../Stammstrecken_Puzzle_Konzept_00-15/06_Fortschritt_Belohnungen_und_Meisterschaft.md "Stammstrecken-Puzzle – Fortschritt, Belohnungen und Meisterschaft"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/08_Faire_Monetarisierung_und_Werbeangebote.md "Stammstrecken-Puzzle – Faire Monetarisierung und Werbeangebote"
