# Nächster großer Ausführungsauftrag – Prüffassung und lokaler Fortschritt

Gezielte getrennte Bestandsprüfung der bestehenden Verträge durch die Application-Implementierungsrolle. Kein Code, keine Produktentscheidung und kein nachträglich erfundener Folgeanker. Der erste tatsächliche Spielblock WP038 wird dadurch nicht erweitert. Nach dessen abgeschlossener Schnittstelle und bestätigter Basis kann ein Auftrag zwei getrennt verankerte WPs parallel koordinieren, jeweils einschließlich Eigenkorrektur, unabhängiger QC und zulässiger Integration.

## Strang A – echte 240er-Prüffassung

Den bestehenden WP026-Freeze und seine Quellhashes binden; ausschließlich öffentliche ID/Ruleset/Raster/A/B/Counts/Puzzleidentität in einen ausdrücklich nichtproduktiven QA-Prüfbestand projizieren. Autorenlösung, Proof und interne Einstiegshinweise erreichen die Spieleransicht nicht. Vorhandene logische Nachweise unverändert wiederverwenden. Die vorhandene Oberfläche kann diese Definitionen ohne Save, finale Assets oder Rewards bedienbar machen. Notwendig sind ein eigener prospektiver WP-/Manifestanker und ein vor Code angenommener technischer Reviewzugriffs-ADR; ADR033 erlaubt nur die beiden eigenen WP038-QA-Puzzles.

DRAFT ist nach CONTENT_CATALOGS im Authoring/Review zulässig; CONTENT_PIPELINE ordnet den menschlichen Spieltest vor dem Productionimport ein. `IsRuntimeImportable`, DRAFT, Release-Lock und produktive Import-/Freigabegates bleiben unverändert. Dieser Strang ist keine heimliche WP026-Mainintegration oder Kampagnenfreigabe. Nach tatsächlicher Fertigstellung dürfen echte Menschen prüfen, während Strang B und Gestaltung weiterlaufen.

## Strang B – Save v2 und Grundfortschritt

Zusammenhängend typisierte Save-/Katalogports, validierte Checkpoints/Restoregrenze, Save-v2-Schema/Codec, atomaren Writer/Recovery, idempotenten Erstabschluss und UI-Anschluss produzieren. Dateisystemzugriff bleibt ausschließlich beim Persistenceadapter; Applicationports bleiben providerfrei. Die technische Transaktion wird gegen eigene gelockte Testkataloge geprüft und behauptet dadurch keine Produktionskampagne.

Die bestehenden Verträge entscheiden bereits:

- PERSISTENCE §3: `STP-SAVE-JCS-1`, kanonischer Payload, UTF-8 ohne BOM/Zeilenende, UTF-16-Schlüsselsortierung, keine Floats, sichere Ganzzahlgrenze und SHA256. Unbekannte Profile erhalten die Daten und ergeben `SAVE_HASH_PROFILE_UNSUPPORTED`.
- §5: ein Writer; Pending schreiben, flushen und wiederlesen; gültiges Main nach Backup rotieren, atomar ersetzen, Main erneut prüfen. Erst danach `SaveCommitted(generation)`.
- §6: Main/Backup/Pending unabhängig prüfen, höchste gültige Generation wählen; Split-Brain bewahrt beide Quellen und bevorzugt konservativ Main. Kein automatischer leerer Save bei Recoveryfehlern.
- §7: Autosave nach 500 ms, spätestens zwei Sekunden; Lifecycle sofort; Abschlusscommit vor produktiver Fahrt und Reward.
- §8 / ADR019/025: Migration auf Kopie mit expliziter Versionsfolge und Quellsicherung; keine erfundenen Identitäten/Claims.
- CONTENT_CATALOGS §§6–8 / GAME_STATE_MODEL §§6,10: erwartete Savegeneration serialisiert konkurrierende Mutationen; Fortschritt und Ledger atomar und idempotent.

Heute sind `ISaveRepository`, `ICampaignCatalog` und `ICompletionCatalog` leere Marker, Persistence ist ein Skelett. Es fehlen Save-/Profil-Datenmodell, vollständiger Codec/Writer/Recovery, Progress-/Ledgerproduktion sowie validierter Domainrestore und die bestätigten Save-/Recoveryübergänge. Ein Restore nur aus Zellgettern verlöre Undo-, Korrektur- und Deduplikationszustand. Konkrete Checkpoint-/Restoregrenze, Commitbeleg und typisierte Load-/CAS-Ergebnisse deshalb vor Code technisch entscheiden.

Der bestehende JCS-Code liegt in Infrastructure.Content. Eine direkte neue Persistence→Content-Kante wäre verboten. Seine Wiederverwendung beziehungsweise die gemeinsame reine Kanonisierungsgrenze muss vor Umsetzung innerhalb der bestehenden Modulgrenzen oder durch einen ausdrücklich angenommenen technischen ADR geklärt werden; keine beiläufige neue Referenz oder zweite ungebundene Hashsemantik.

Ein eigenständiges vorhandenes Save-Implementierungs-WP wurde in den gefetchten Remotebäumen nicht gefunden; WP001 enthält Architekturplanung. Daher keinen alten Planungsauftrag als ausführbaren Erstanker behaupten.

## Produktentscheidungen mit engem Effekt

Hintanspruch, Tagesgrenze und Generatorprofil bleiben jeweils für ihre eigenen Features blockiert. Fehlende Sternzeiten sind nullable und erlauben weiterhin einen Stern und Grundfortschritt. Keiner dieser Punkte blockiert normalen Save oder die ausdrücklich nichtproduktive 240er-Prüffassung. Neue Preis-/Asset-/Releaseentscheidungen werden nicht in diese technischen WPs hineingeraten. Quellen: OPEN_BLOCKERS, GAME_STATE_MODEL §9 und die genannten angenommenen Verträge.
