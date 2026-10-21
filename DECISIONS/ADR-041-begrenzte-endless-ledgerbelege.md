# ADR-041 – Begrenzte Endlos-Ledgerbelege

## Status

**Angenommen** am 21.10.2026 durch Releaselead R innerhalb WP-046 nach getrennter C-Fachreview, vor abhängiger Receiptimplementierung. Die konkreten Ledger-/Retentions-/Mutationsnachweise stehen gesondert aus; keine Produkt-/Human-/Geräteannahme daraus.

## Datum

21.10.2026

## Kontext

P05 gewährt jeder neuen korrekt gelösten Endlosinstanz 15 Punkte und ihre einmaligen Zeitsternboni 5/10; optionale Videoeffekte kommen getrennt hinzu. ADR-040 ermöglicht deren echten Runtimepfad. Das vorhandene Ledger kompaktiert nur terminalgedeckte Einträge und verlangt auch für seine letzten 64 Diagnostikeinträge Terminalreferenzen. Der bestehende `COMPLETION_CLAIM_OPEN`-Payload speichert keine Basisterminals. Eine unbegrenzte terminale Endloshistorie bleibt durch ADR-019/025 verboten.

## Entscheidung

Der neue ADR-040-Endlosrecord `recordVersion:2` erhält zusätzlich `economyReceipts`, eine geschlossene Liste aus vorhandenen vollständigen `EconomyTerminal`-Objekten. Sie ist auf höchstens 576 Einträge begrenzt: maximal 512 Journalreferenzen plus maximal 64 Referenzen des vorhandenen Ledgerdiagnostiktails. Jede Receipt-Transaktions-ID ist eindeutig und muss tatsächlich in Journal oder Tail referenziert sein; doppelte, fremde, wertmäßig oder katalogseitig widersprüchliche Belege werden abgewiesen. Zulässig sind ausschließlich echte Endlosbasis-/Stern-/Postclearterminals mit tatsächlichen Completionkatalogwerten und CommittedGeneration innerhalb des bestätigten Saves.

Ein Grant schreibt Terminalbeleg und Ledgerdelta atomar. Die bestehende Ledgerhashkette, JCS-Hashprofile und bestehende Entry-/Terminalform bleiben unverändert. Kompaktierung berücksichtigt diese Belege als zusätzliche Terminalquelle; danach entfernt dieselbe Snapshotprojektion ausschließlich nicht mehr durch Journal oder Tail referenzierte Receipts. Das feste 576er-Maximum erzeugt keine Nutzungsgrenze: vor Aufnahme neuer Grants erfolgt vorhandene Kompaktierung und anschließendes Pruning. Bei integritätswidriger Belegung wird fail-closed gestoppt.

`economyReceipts` ist ausschließlich terminaler Buchungsbeleg für die begrenzten Ledgerreferenzen. Es entscheidet niemals Endlosmembership, Resume, Abbruch-/Abschlussstatus, Claimberechtigung, neue Credits oder Deduplikation. Das reservierte Watermark und sein höchstens 20 Einträge umfassendes offenes Komplement bleiben alleinige Endlos-Lifecycleautorität. Fehlende alte Receipts rekonstruieren keinen Claim. Alte Endlosrecords Version 1, historische QA und neutrale v2→v3-Goldens bleiben unverändert.

## Begründung

Die reale Punktevergabe benötigt überprüfbare Buchungsreferenzen. Deren Lebenszeit an die bereits begrenzten Ledgerreferenzen zu binden erhält konstante Savegröße, exakte Kompaktierung und die bestehende unbeschränkte Endlosnutzung.

## Betrachtete Alternativen

- Alle Endloslevel in Kampagnenprogress aufheben: unbegrenzte Terminalhistorie und falsche Kampagnenwerte.
- Ledger ohne Terminalbindung kompaktierten: schwächt die bestehende Integritätsprüfung.
- Nur den offenen Claim als Beleg verwenden: sein legitimer Skip/Terminalabschluss würde Ledgerreferenzen zerstören.
- Zweites Punktekonto: widerspricht der einen atomaren Ledgerwahrheit.

## Konsequenzen

LocalEndlessState/Save-v3-Recordreader und SaveLedgerCodec erhalten die eng begrenzte zusätzliche Terminalquelle. Die Endlospolicy schreibt und bereinigt Belege gemeinsam mit den echten Effekten. Keine Human-, Geräte-, Profil- oder Produktfreigabe wird daraus abgeleitet.

## Betroffene Artefakte

`Application/LocalProfileState.cs`, neue Endlos-/Economypolicies, `Infrastructure/Persistence/LocalSaveCodec.cs`, `SaveLedgerCodec.cs`, `SaveMigration.cs` und gezielte EditModeprüfungen.

## Validierung

Getrennte C-Fachreview vor abhängiger Umsetzung. Tests prüfen atomare Basis-/Stern-/Videogrants, Katalogbetrag-/Transaktions-/Generationmutationen, Kompaktierung und Taildeckung, fehlende/doppelte/fremde Belege, höchstens 576 Belege sowie mindestens 10.000 Lifecyclewechsel ohne wachsende Terminalhistorie. Ein terminaler Ordinal ohne offenen Claim bleibt auch mit vorhandener Receipt nicht reward- oder hintberechtigt. Alte QA-/Migrationsgoldens müssen unverändert bleiben.

## Ersetzt / ersetzt durch

Ergänzt ausschließlich die begrenzte Ledgerbelegführung des neuen ADR-040-Endlosrecords. Die angenommenen übrigen Verträge ADR-019/025/027/037/040 bleiben unverändert. Wird derzeit durch keinen ADR ersetzt.
