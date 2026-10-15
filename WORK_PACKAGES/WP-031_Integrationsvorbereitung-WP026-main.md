# WP-031 – Integrationsvorbereitung WP-026 / main

## ID

`WP-031`

Status: Historische Integrationsprobe C060 tatsächlich erstellt/geprüft und als solche abgegeben; alter Importscope FAIL9 unverändert, kein rückwirkender Production-PASS. Drei P2-Nacharbeiten auf 9c1859f gezielt unabhängig geschlossen. Eigener prospektiver WP-032-StandB83 anschließend mit getrennter Herkunfts-/Konflikt-/vollständiger CI-/Rollenabnahme technisch erbracht; reine Schlussnachführung WP-033. Keine Definition-/Manifestaufweitung, neue externe Verfahrenssperre oder Gesamt-/Mainfreigabe.

[Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-031.production.scope.json)

## Ziel

Eine konkrete konfliktaufgelöste Kombination des korrigierten WP-026-Lieferstands mit main d90e798 in einem isolierten Git-Prüfcheckout erstellen und tatsächlich prüfen; beide Lieferungen, Manus-Entscheidungen und historische Anker erhalten. Zulässige Lieferzweignachführung und noch nicht freigabefähige Kombination getrennt übergeben.

## Voraussetzungen

AGENTS-Lesereihenfolge, vollständige WP-026/028/029/030, Gesamtabnahme b1457ba, WP-027/Bible/ADR-032 auf d90e798, ADR-030, Scopevalidator/README, Paket-/Übergaberegeln/DoD. Neuer gemeinsamer Erst-Add WP/Manifest auf 21f8ce5d5f9ed39b4f63c4372b5c4e92b1b1fb2a; danach Manifest unverändert. Originalbericht-SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433. Tatsächlicher erster Nacharbeits-CI-Lauf f8e1dfc scheitert allein am unveränderten Originalbericht mit leerer Schlusszeile; keine inhaltliche Berichtkorrektur erlaubt.

## Scope

1. Originalbericht verlustfrei in einem klar gekennzeichneten Base64-TXT-Transportcontainer am bestehenden Reviewpfad persistieren; vollständige Dekodierung muss identische Originalbytes/Hash liefern. Keine Originalzeile trimmen. Betroffene Nachweis-/Statusverweise und eigene Bindungen konsistent nachführen; historischen CI-FAIL transparent erhalten.
2. Eigenen Lieferzweig-Dokumentationsscope und isolierte Integrationsprobe getrennt behandeln. Im eigenen Scope ausschließlich die unten aufgeführten Nachweise und Statusdateien ändern; validate.yml nur eigene Manifestzuordnung.
3. In ignoriertem separatem vollständigem Git-Klon einen benannten Previewbranch vom neuen eingefrorenen Lieferstand anlegen, main exakt d90e798 hineinmergen und drei Konflikte in validate.yml/CURRENT_STATE/WORK_QUEUE sachlich auflösen. Automatische übrige Main-Änderungen unverändert erhalten; keine Solver-/Content-/Test-/ADRentscheidung treffen.
4. Tatsächlichen gemeinsamen Baum mit aktuellem Produktions-/Schema-/Node-/Root-/Governanceweg und regulärer Unity-CI prüfen. Scopefehler nicht umgehen: der bestehende Einzelmanifestvalidator kann fremde Manifest-/Produktquellenimporte nicht als eigenes Productiondelta freigeben. Kombinierte Provenienz/Anker/Dateibäume zusätzlich getrennt prüfen; ein solcher Beleg ersetzt kein fehlgeschlagenes Pflichtgate.
5. Previewcommit/Eltern/Tree, Konfliktpatch und tatsächliche Prüfstände reviewbar sichern; normale Pushes und erforderliche Draft-Unter-PR gegen den offenen WP-026-Lieferzweig erlaubt, niemals Merge nach main oder automatischer Previewtransfer. Konkrete Verfahrens-/QC-Grenze zur Entscheidung übergeben, falls ein dauerhaft zulässiger Integrationsscope fehlt.

## Betroffene Dateien/Module

Eigener abschließender Manifestbereich: dieses WP/Manifest, tools/wp031-integration/prepare.py, PROJECT_CONTROL/Integration/WP-031_* als eigene MD-/JSON-/TXT-/Patch-Nachweise, CURRENT_STATE/WORK_QUEUE, WP-029/WP-030 nur aktueller Status, validate.yml nur eigene Manifestzuordnung, Originalbericht-Transportcontainer, F01_BINDINGS und die vier aktuellen Review-/Übergabe-MDs. Der separate kombinierte Scratchcheckout ist eine **Untersuchungsprobe** und verändert die Projektdateien dieses Lieferzweigs nicht; dortiger vollständiger Main-Import bleibt als Scopegegenprobe sichtbar und wird nicht unter die eigene Allowlist geschoben.

## Ausdrücklich nicht erlaubte Änderungen

Keine Änderung bestehender Manifeste, Paketdefinitionen/-grenzen, Projektregeln, Produktentscheidungen, ADRs, Validatoren, Solver-/Pipeline-/Hash-/Schemacode, Tests, Budgets oder Runner. Keine Erteilung einer Scopeausnahme, kein fiktiver Gesamt-PASS. Keine menschlichen Spieltests, kreative Arbeit oder Zeitkalibrierung. Kein Merge der Preview-Unter-PR, nach main, Runtimeimport oder Release.

## Akzeptanzkriterien

AK-01: eigener Anker/Parent/Manifest und erlaubter Lieferzweigdiff korrekt; Originalbericht vollständig verlustfrei dekodierbar, historische CI-Grenze erhalten. AK-02: drei konkrete Konflikte isoliert gelöst, Preview mit beiden Eltern, sämtliche automatischen Main-/Lieferinhalte und alle historischen Manifestbytes erhalten. AK-03: tatsächlicher gemeinsamer Baum geprüft; jede Probe an echten Commit/Tree und Toolchain gebunden; kein PASS der getrennten Vorgänger übertragen. AK-04: aktueller Status/PR-Übergabe korrekt; kombinierter Scopefehler und nötiger Verfahrensentscheid exakt dokumentiert, keine Regeländerung. AK-05: unabhängige gezielte Prüfung tatsächlich organisieren, wenn ein regelkonformer Ausführungsdienst verfügbar ist; andernfalls prüffertige externe Übergabe, keine Selbstfreigabe.

## Tests

1. Originaltransport dekodieren, SHA256 und vollständige Bytes gegen Original prüfen; git diff --check, eigener kanonischer Architecture-/Production-Scope mit --self-test.
2. Main-/PR-/Merge-Base-/Elternabgleich; vollständige Dateien- und Manifest-/Productsource-/Code-/Test-/Contentgleichheit beider Eltern mit der Kombination, drei Konfliktlösungen manuell lesen.
3. Produktionsverify (240, Regeneration/Strict/Metamorphose/Mutation), aktuelle Schema-/Node-/Rootbindungen und Architecture-only tatsächlich im Previewcheckout. Einzelmanifest-Scope dort tatsächlich aufrufen und etwaige Fehler ungefiltert sichern.
4. Echte reguläre CI am eingefrorenen Liefer-/Previewstand, entscheidende Logs/Test-XML/Benchmarkzeile prüfen. Keine erfundenen Builds; nicht ausführbare/fehlgeschlagene Gates und verbleibende unabhängige Prüfung ausdrücklich offen lassen.

## Risikoklasse

Hoch: kombinierte Quellen-/Integrations- und Scopegrenzen. Technische Konfliktfreiheit ist keine Integrationsfreigabe.

## Definition of Done

Projektweite DoD unverändert. Nur tatsächlich erfüllte Kriterien als erfüllt kennzeichnen. Der isolierte Preview kann technisch konfliktfrei und inhaltlich geprüft sein, während Scope-/QC-/Human-/Fremdgates weiterhin eine Übernahme verhindern. Eigene Ergebnisse/CURRENT_STATE und konkrete Branch-/PR-/Commit-/Treeübergabe sichern; keine Gesamtfreigabe.

## Tatsächliche Ausführung und Übergabe – 15.10.2026

Anker f1f0c65 mit Parent/Manifestbasis 21f8ce5; Manifest unverändert. Originalbericht vollständig verlustfrei dekodiert/hashgeprüft, eigener f8-Whitespace-FAIL separat erhalten, d7-CI Architecture SUCCESS und 455/455 EditMode-XML nachgerechnet. Gemeinsamer Preview 060c1c4 (Tree652661d, Eltern d7f69f6/d90e798) isoliert erstellt, drei Konflikte gelöst, 1109 Nichtkonfliktpfade gegen Eltern geprüft. Produktionsverify, 1200 Node-/Schema-/Rootbindungen und 17 Architecture-Gruppen wirklich im gemeinsamen Baum bestanden. Lokale und echte CI-Scopeprüfung melden neun konkrete Mainimport-Fehler, nicht umgangen. Tatsächliche weitere CI wird an diesem Preview und dem reinen Übergabecommit getrennt am PR nachgeführt. [Vollständige AK-/Grenzen-/Nachweisübergabe](../PROJECT_CONTROL/Integration/WP-031_HANDOVER.md). AK-01/02 eigene Umsetzung erfüllt; AK-03 gemeinsame Prüfungen geliefert, Scopepflichtgate FAIL; AK-04 konkrete Grenze dokumentiert; AK-05 prüffertige externe gezielte Nachprüfung, keine Selbstfreigabe. Kein Gesamt-DoD-PASS, keine Main-Integration/Import/Release.

## Gezielte QC-Nacharbeit – nach Freeze93

AK05 unabhängige Rollen tatsächlich ausgeführt; Originalberichte und drei P2-Korrekturen [hier](../PROJECT_CONTROL/Integration/WP-031_QC_CORRECTIONS.md). Quellenhierarchie korrigiert die frühere pauschale externe Verfahrenssperre: neuer ausdrücklich autorisierter prospektiver WP032-Anker aufC060 mit separater zwingender Provenienz-/Konflikt-/CI-Abnahme ist zulässig; der alte kombinierte WP031-Scope bleibt FAIL9. Keine Änderung dieser historischen Paketdefinition/Manifestgrenze, kein rückwirkender PASS. Gezielte unabhängige Korrekturdiffprüfung separat belegen.

## Aktuelle standgebundene Schlussnachführung – WP-033

Diese Nachführung ändert ausschließlich Bearbeitungs-/Ergebnisstatus, keine Definition, AK, Scopegrenze oder historische Manifestbasis. [Vollständige aktuelle Nachweise und Grenzen](../PROJECT_CONTROL/Integration/WP-033_HANDOVER.md). Frühere Ausführungsabschnitte bleiben ihre standgebundene Historie; keine Umdeutung alter FAILs und keine Gesamtfreigabe.
