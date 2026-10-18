# WP038 – zusammenhängender Ausführungs- und Korrekturstand

19.10.2026. Aktuell: erster Freeze e77c5fa regulär vollständig SUCCESS mit474 Edit-/48 Playfällen und tatsächlichen Android-/iOS-/Xcodebelegen, wegen unabhängigem SCREEN-01 dennoch historisch FAIL. [Unveränderte historische CI](CODEFREEZE_CI.md), [getrennte Originalprüfung](INITIAL_CODEFREEZE_CI_QC.md), [konkreter FAIL](SCREEN_OUTPUT_QC.md).

Die eng begrenzte Installer-/Szenentestkorrektur ist tatsächlich nativ geprüft:9/9 Passed, drei normale korrekt orientierte Screens und drei Panelproben selbst betrachtet. [Aktueller Lauf-/Bild-/Sourcebeleg](SCREEN_CORRECTION_RUNTIME.md), [beide verworfenen Captureversuche](SCREEN_CORRECTION_ATTEMPTS.md). Eigene Screenkamera mit null-Target und offizielle AsTexture-Aufnahme nach Frameende, keine Panel-RT-Umleitung. Geschützte automatische Settingsdeltas gesichert und zurückgesetzt. Gezielt unabhängige Korrekturprüfung, neuer Freeze mit eigener vollständiger CI und unabhängiger Gesamtannahme sowie gesonderte Unterintegration folgen intern. **Kein Gesamt-/Integrations-PASS.** Die folgenden Abschnitte enthalten den erhaltenen ersten Implementierungsstand; der aktuelle Abschlussstand steht in diesem Vorspann, CURRENT_STATE und der AK-Matrix.

## Konkreter Fortschritt

Die neue interne Szene lässt ein eigenes öffentliches 4×4-Rätsel tatsächlich über UI Toolkit bearbeiten. Alle sechs Trackformen, X/Grau/Leeren, getrennte gelbe Auswahl, expliziter atomarer Batch, Undo, Domain-Timer und neue Versuche sind implementiert. Eine echte gültige Spielerstrecke erzeugt den Domain-PuzzleSolved-Pfad; nur dessen Kopie steuert die sichtbare World-Zugfahrt. Danach erscheint ein ausdrücklich ungespeichertes Ergebnis. Der Domainzustand bleibt SolvedPendingCommit; kein Save, Fortschritt, Reward oder Produktionscontent wird vorgetäuscht. Die zweite eigene 10×10-Definition prüft das große Raster.

UI, World, Clock und Content werden ausschließlich vom expliziten Bootstrapinstaller über die bestehenden Grenzen komponiert. Die neue externe Unity.Localization-Referenz ist exakt benannt; die eigene neutrale de-DE-Tabelle bleibt vom Produkttextbestand getrennt. Szene, Panel und Material haben feste Identitäten und werden durch den Editorbuilder reproduziert. Der bestehende reguläre Android-/iOS-Buildpfad führt die neue Szene sowie die alte Composition-Smoke-Szene mit; das tatsächliche Mitbauen muss noch die reguläre CI belegen.

## Bereits tatsächlich belegt

- Gemeinsamer Anker `ee14a1bf1ad88768fecca6937072cfe4fc14ba67` auf Basis `3772c0370b39ec95979fb9efdd47f4344c41a539`, unveränderliches eigenes Manifest. Gesonderte ADR033-Annahme `4667413ae0baa9971b4b7660a19988ce36248469` nach getrennter Boundaryprüfung, danach unabhängiges kanonisches Vorcode-PASS.
- [Ergänzende Eigenprüfung](SUPPLEMENTAL_VALIDATION.md): eigene relevante Assemblies kompiliert, reine Application-NUnitfälle 19/19 Passed; tatsächlicher unity.yml-Modulgraphblock 14 Produktions-/9 Testassemblies PASS. Diese lizenzfreien Teilbelege ersetzen keine Unity-CI.
- [Tatsächlicher lokaler Unity-/Grafiklauf](GRAPHICS_QC.md): gepinnter Editor, echter D3D11-Renderer, finale tatsächliche Assetneuanlage und anschließende 8/8 bestandene Szenen-/Grafikfälle ohne Skip. Doppelte Erzeugung und synchroner sauberer Reimport bytegleich. Original-XML, deklarierte vollständige bereinigte Logprofile, Originallogdigests und Source-/Bildbindungen gesichert. Root hat die echten Bilder selbst betrachtet.
- Konkreter unabhängiger GUID-FAIL nach dem ersten Lauf erkannt und innerhalb desselben Auftrags korrigiert; ursprüngliche Belege bleiben erhalten. Keine Weitergabe eines technischen Korrekturauftrags an den Gesellschafter. Unveränderte Solver-/Textbeweise werden wiederverwendet.
- [AK01–AK11-Matrix](ACCEPTANCE_MAP.md), [Bedienung](QA_PLAY_GUIDE.md), [nächste ausführbare Blockgrenzen](NEXT_BLOCK_BOUNDARIES.md). [WP037-Schlussnachlauf](PARENT_CLOSEOUT_CI.md) gehört ausschließlich zum Parent: beide Architecture und beide Unityevents SUCCESS, Unity jeweils zehn Jobs; keine Übertragung als WP038-CI.

## Noch auszuführender interner Abschluss

1. Tatsächlichen Codefreeze-SHA/Tree und unveränderten Manifest-/Text-/Altdateischutz binden; kanonische finale Architecture-/Scope-/Self-/Negativchecks ausführen.
2. Draft-Kind-PR ausschließlich gegen den bestätigten WP036-Developmentzweig; reguläre komplette eigene CI am Freeze abwarten und sämtliche Originalstatus-/Checkout-/XML-/Coverage-/Build-/Artefaktbelege selbst lesen und sichern.
3. Vom Implementieren getrennte Abschluss-QC desselben Freeze. Konkrete Befunde intern korrigieren und betroffen nachprüfen; reine Dokumentationsnachträge benötigen nur die vertragliche schmale Delta-/Byteprüfung.
4. Bei PASS eigener zuständiger Unterintegrationsentscheid, frischer Expected-HEAD-/Zielcheck, tatsächlicher Untermerge und eigene integrierte Tree-/Diff-/Scopeketten-/reguläre-CI-Prüfung. Keine Mainintegration.

## Nächster großer Produktionsauftrag

Nach fertig geprüfter Spielschleife können eine nichtproduktive Public-Input-Prüffassung für die vorhandenen 240 WP026-Rätsel und Save/Grundfortschritt in einem koordinierten Auftrag mit getrennten technischen WPs/Ankern parallel entstehen. Der QA-Zugriff benötigt vor Code seine eigene technische Boundaryentscheidung; ADR033 erlaubt nur die beiden eigenen QA-Puzzles. Menschliche Tests starten, sobald ihre bedienbare Prüffassung da ist, unabhängig von finalen Assets oder vollständiger Saveproduktion. Der bestehende Manusplan und alle Produkt-/Import-/Rechts-/Geräte-/Releasegates bleiben verbindlich.

## Getrennte native Nachprüfung

Aktueller zusätzlicher Teilabschluss: [SCREEN_OUTPUT_CORRECTION_QC](SCREEN_OUTPUT_CORRECTION_QC.md) tatsächlich getrennt PASS auf den exakten korrigierten Sourcebytes. Alle41 Bindungen (zwei geändert/39 unverändert),9erOriginal-XML, vier Payloads, sechs PNGs und fünf restaurierte Settings geprüft. Die ursprünglichen SCREEN-01-/Capture-FAILs bleiben erhalten. Neues Source-CI-/Gesamt-/Integrationsgate bleibt offen.

[Historischer konkreter GUID-FAIL](NATIVE_INITIAL_QC.md) und [gezielte unabhängige Korrekturprüfung](NATIVE_CORRECTION_QC.md) sind getrennt persistiert. NATIVE-01 ist nach neuer tatsächlicher Assetneuanlage, Original-XML-/Meta-/Source-/Bildprüfung geschlossen. 41 aktuelle Quellbindungen bestätigt: sechs betroffene Deltas, 35 unveränderte Pfade; kein offener nativer Artefaktbefund. Dieser Teil-PASS ersetzt ausdrücklich keine unabhängige Abschluss-QC auf Codefreeze mit dessen regulärer CI.
