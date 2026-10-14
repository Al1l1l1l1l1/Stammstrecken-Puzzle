# WP-029 – Abnahme-Nachführung WP-026

## ID

`WP-029`

Status: verankert, Bearbeitung noch nicht begonnen. Nutzerauftrag vom 15.10.2026 autorisiert die eng begrenzte Dokumentationsnacharbeit F-01/O-03. Kein Ersatz oder Erweiterung von WP-026/WP-028.

[Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-029.production.scope.json)

## Ziel

Sachlich richtige, aktuell gebundene Abnahme- und Übergabedokumentation für den weiterhin offenen WP-026-Lieferzweig; F-01 wird innerhalb des bestehenden WP-026-Metadatenscopes korrigiert.

## Voraussetzungen

AGENTS-Lesereihenfolge, vollständiges WP-026/WP-028, DoD, Übergabe-/Paketregeln, ADR-021/030/031/032, Content-Bible 14 und WP-027 auf main d90e798 sowie vollständige unabhängige Gesamtabnahme zu b1457ba. Start: sauberer PR-21-HEAD b1457ba6a61826fe3b64a83323c27a13e9d23080, main d90e798dd9b26d3f152f3b0c2d1d52cae641eb16; andere Projektchats idle. Gemeinsamer erster Add dieses WP und eigenen Manifests auf genau b1457ba; Manifest danach unverändert.

## Scope

1. Originalgesamtabnahme byteunverändert als TXT im bestehenden WP-026-Reviewscope persistieren; Archiv ausschließlich isoliert lesen, Herkunft und Hashes getrennt dokumentieren.
2. F-01: nur die falsche Ortsangabe in S1-03-03-03 präzisieren, explizite Autorenliterale/-bindung, eine Revision, Inventar, Review und aktuelle Dokument-/Produktions-/Nodebelege erneuern. Kein neuer Puzzleinput oder Proofvertrag.
3. O-03: aktuelle Statusquellen um entschiedene WP-027-Auslegungen, tatsächlich erbrachtes CI-PERF und durchgeführte Gesamtabnahme ergänzen. Aktuelle Routenurteile mechanisch auf die bestätigte Auslegung beziehen; Originalprüfberichte unverändert lassen.
4. Aktuelle Repository-/PR-Übergabe und getrennte Scope-/Standbelege sichern. Normale Commits/Pushes, bestehender Draft-PR #21; unabhängige Nachprüfung nach Freeze organisieren, sofern regelkonform verfügbar, sonst prüffertig übergeben.

## Betroffene Dateien/Module

Abschließend das verlinkte Manifest: eigenes WP/Manifest; validate.yml nur Manifestzuordnung; WP-026 nur Status/Nachweise; CURRENT_STATE/WORK_QUEUE; HANDOVER/REPAIR_HANDOVER/REVIEW_RESOLUTION nur aktuelle Einordnung; Originalbericht neu; Autorenliteral/-JSON, einziges Level S1-03-03-03; bestehendes Inventar, Routenreviews 01/11/18, technische/schema-node-Evidenz und drei neue F01-Nachweisdateien. Keine weiteren Module.

## Ausdrücklich nicht erlaubte Änderungen

Keine Definitionserweiterung bestehender Pakete/Manifeste. Keine Originalberichtänderung, Puzzle-/Lösungs-/Proofbyteänderung, Produktionscode-/Test-/Schema-/Architektur-/Produktänderung, menschlicher Test, kreative Arbeit oder Zeitkalibrierung. O-02 und O-04 sind getrennte Folgeaufträge; kein Merge nach main, Import, Release oder Gesamtfreigabe.

## Akzeptanzkriterien

AK-01: Gemeinsamer historischer Add/Parent/Manifestbytegleichheit; vollständiger eigener Diff erlaubt. AK-02: F-01 korrekt mit x0=0, x1=2, x3=6, x4=5 beschrieben; nur ein Level erhöht seine Revision einmal, Transport idempotent. AK-03: 240 öffentliche Inputs/Lösungen und Proofbytes erhalten, aktuelle Schema-/Node-/Produktionsbindungen bestanden. AK-04: Manus-Auslegungen entschieden, CI-PERF auf b1457ba belegt, Gesamtabnahme mit ihrem tatsächlichen Urteil eingeordnet; offene Gates getrennt. AK-05: Originalbericht byteunverändert, neue Evidenz getrennt und standgebunden; eingefrorener Prüfstand/PR übergeben, unabhängige Prüfung nicht simuliert.

## Tests

1. Architecture-only und eigener Production-Scope jeweils mit --self-test; git diff --check und geschichtete frühere WP-026-/WP-028-Pfad-/Manifestprüfung.
2. Tatsächliche Ortsprüfung am Countsarray und Pfad, 240er Vorher/Nachhervergleich, zweimaliger Transport und Revisionsvergleich.
3. Vorhandene Produktionsprüfung verify (240, dreifache Regeneration, Strict/Batch/Reverse, 3840 Metamorphosen, 1440 Mutationen), Inventar und 1200 Node-/Schema-/Hashprüfungen; vorhandene echte Unity-CI auf neuem Freeze separat prüfen.
4. Status-/Quellen-/Originalhashprüfung; kein historischer PASS-Transfer. Unabhängige gezielte F-01-/Statusnachprüfung auf neuem Freeze oder konkrete externe Übergabe.

## Risikoklasse

Mittel: begrenzte redaktionelle Korrektur und Nachweisführung; falsche Einordnung könnte eine Gesamtfreigabe vortäuschen.

## Definition of Done

Projektweite DoD bleibt unverändert. AK einzeln mit realen Ergebnissen dokumentieren; CURRENT_STATE aktualisieren. Gesamt-WP-026 bleibt offen. Fehlende unabhängige Nachprüfung/CI konkret ausweisen; keine Selbstfreigabe.
