# WP-032 – Gemeinsamer Integrationsprüfstand WP-026 / main

## ID

`WP-032`

Status: Abgegrenzte technische Integrationsvorbereitung auf B83 erbracht: AK01–06 nach tatsächlichen Quellen-/Anker-/Herkunfts-/Konflikt-/Scope-/Produktions-/Schema-/Node-/Rootprüfungen, vollständiger regulärer CI und beiden getrennten unabhängigen Rollen im jeweiligen Umfang bestätigt. Technischer Schlussstand und Originalberichte unter WP-033 gesichert; alter FAIL9 erhalten. Keine Gesamt-WP-026-/Human-/Main-/Import-/Releasefreigabe; Definition/Manifest unverändert.

[Eigenes Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-032.production.scope.json)

## Ziel

Einen eng abgegrenzten, dauerhaft reviewbaren gemeinsamen Nicht-main-Arbeitsstand für WP-026 und main d90e798 herstellen. Bereits unter WP-031 autorisierte Zusammenstellung C060 als tatsächlichen Eingang vollständig nach Herkunft und Konfliktentscheidungen abnehmen; eigene nachfolgende Dokumentations-/CI-Zuordnungsänderungen separat scopeprüfen. Weder der Anker noch ein kleiner eigener Diff ersetzen die Prüfung der Eingänge.

## Voraussetzungen

Ausdrückliche Nutzerbeauftragung für O-04 einschließlich eigener regelkonformer enger Integrationsvorbereitungsauftrag-Verankerung, keine Integration nach main. AGENTS mit vollständiger Lesereihenfolge, DoD/AI_HANDOVER_RULES/WORK_PACKAGE_RULES/WORK_QUEUE, ADR-026/030/031/032, Scopevalidator und README. WP-026/028/029/030/031 sowie WP-027 auf main und vollständiger Originalgesamtbericht. Getrennte Original-QC auf93/C060: QC-TECH-01 kanonische Bindungen, QC-TECH-02 zu weit gefasste externe Verfahrenssperre, QC-CONTENT-01 aktuelle Statusinkonsistenz. Kein genereller neuer Architekturentscheid für dieses konkrete prospektive Paket erforderlich; unveränderte Einzelanker und Eingangsnachweise sind zwingend.

Eingänge: main d90e798dd9b26d3f152f3b0c2d1d52cae641eb16; technischer Lieferfreeze d7f69f66e6d58191fa44d64a20523f78ad9a5432; gemeinsame autorisierte Scratchzusammenstellung 060c1c4b01458a80835094d5bd004be8c36eec54, Tree652661ddd524cbfaea1bdeb2f5de2e8d4ea9c101, genau diese zwei Eltern. Bestehende dokumentarische Lieferung93b49a9 sowie ihr enger nachfolgender P2-Korrekturcommit werden nur mit exakt gebundener Herkunft übernommen. Gesamtabnahme b1457ba und Manifestbasen bleiben davon getrennt.

## Scope

1. Vor weiteren Änderungen dieses WP und sein eigenes unveränderliches Manifest in einem einzigen Erst-Add-Anker, dessen einziger Parent/BaseCommit exakt C060 ist. Keine Erweiterung alter WPs/Manifeste.
2. Unveränderte historische Einzelanker/Manifestbytes und getrennte bisherige Liefernachweise erfassen. C060 anhand aller drei Eingabebäume vollständig nachrechnen: 1109 Nichtkonfliktpfade mit Modus/Blob dem zuständigen Elternstand zuordnen; genau drei Konfliktentscheidungen manuell abnehmen. Manus' Main-Prioritätsfolge, WP-027-Bible/ADR-032 und alle Produktions-/Test-/Contentquellen erhalten.
3. Ausschließlich den dokumentarischen93-Nachweisstand und eng korrigierte P2-Status-/Hashbindungen aus dem offenen Lieferzweig übernehmen. Aktuelle Steuerungs-/Reviewköpfe konsistent auf den neuen gemeinsamen Prüfstand beziehen; historische Abschnitte/Originalberichte unverändert erhalten. WP-029/030/031 ausschließlich Bearbeitungsstatus und tatsächliche Ergebnisnachweise, keine Definitions-/AK-/Scopeänderung.
4. validate.yml ausschließlich aktive eigene Manifestzuordnung auf WP-032 ändern, alle übrige Main-Ausführung erhalten. Neuer unveränderter Einzelmanifest-Scope prüft ausschließlich den prospektiven Delta abC060; der alte WP-031-Kombinationslauf mit neun Fehlern bleibt separat als historische tatsächliche Gegenprobe erhalten. Keinen rückwirkenden PASS oder allein aus neuer Basis eine Importfreigabe behaupten.
5. Gemeinsamen resultierenden Baum tatsächlich erneut mit Produktion/Regeneration/Strict/Mutation/Metamorphose, Schema/Node/Root und unverändertem kanonischem Architecture-/eigenem Scope-/Selftestweg sowie echter regulärer Unity-CI prüfen. Vollständige Eingangsprovenienz und alte Fehlversuche separat neben neuem Scope-Ergebnis nachweisen. Produktions-/Test-/Level-/Proof-/Architektur-/Regelgleichheit zum C060 ausdrücklich prüfen.
6. Zwei separate unabhängige gezielte Prüfrollen für P2-Korrekturen sowie neuen Eingangs-/Konflikt-/Status-/Scope-/CI-Stand organisieren, ohne Selbstfreigabe oder simulierte Modellidentität. Ergebnisse an tatsächliche Freezes binden. Eigenen Branch und erforderliche Draft-PR mit Nachweisen übergeben, kein Main-Merge. Reine spätere Ergebnis-/Statusbelege getrennt binden, keine rekursive Übertragung eines alten technischen PASS auf geänderten Code.

## Betroffene Dateien/Module

Neu: diese Datei, ausschließlich eigenes Manifest, tools/wp032-integration/verify.py und PROJECT_CONTROL/Integration/WP-032_*.md/json/txt. Zulässige Übernahme unveränderter eigener WP-031-Nachweise unter PROJECT_CONTROL/Integration/WP-031_*.md/json/txt/patch sowie tools/wp031-integration/prepare.py exakt aus dem gebundenen dokumentarischen Lieferfreeze. Statusdateien CURRENT_STATE/WORK_QUEUE, vier Review-MDs HANDOVER/REPAIR_HANDOVER/REVIEW_RESOLUTION/ACCEPTANCE_FOLLOWUP und F01_BINDINGS.json. WP-029/WP-030/WP-031 nur Status/Ergebnisnachweise. validate.yml nur aktive Manifestzuordnung. Übrige vollständige Quellen-/Manifest-/Content-/Testbäume ausschließlich lesen und unveränderte Herkunft nachweisen. Eigene erzeugende Proben in ignoriertem Scratch; erzeugte technische Rohoutputs als eigene neue WP-032-Nachweise sichern und keine fremden Produktionsartefakte umschreiben.

## Ausdrücklich nicht erlaubte Änderungen

Keine Main-Integration, kein Runtimeimport oder Release. Keine Änderung alter Manifestbytes, Paketdefinitionen/-AK/-Scopes, Projektregeln, Produkt-/Architekturentscheidungen, Bible/ADR/Validator/Schema/Teststrategie, Produktionscode/Tests/Budgets/Timeout/Runner, Level-/Lösungs-/Proofbytes oder Authoringlogik. Keine fremden Manifeste in der Allowlist, kein Governance-Umbuchen, keine Testlockerung, keine Quarantäne und kein Umdeuten des alten FAIL9. Keine menschlichen Spieltests, kreative Ausarbeitung oder Zeitkalibrierung. O-02-Ursache nicht aus neuem PASS schließen; diagnostischer Teständerungsbedarf bleibt gesondert zur Entscheidung.

## Akzeptanzkriterien

AK-01 eigener gemeinsamer Erst-Add-Anker, genau Parent/BaseC060, Manifest danach unverändert; vorhandene Einzelanker/Manifeste identisch. AK-02 vollständige1109-Pfadprovenienz und genau drei manuell begründete Konfliktentscheidungen, beide Lieferungen und Manus' Entscheidungen erhalten; keine Herkunft durch Basenwechsel verdeckt. AK-03 P2-Hashkorrektur kanonisch reproduzierbar, aktuelle Statusdarstellungen widerspruchsfrei, Originalberichte unverändert. AK-04 neue tatsächliche gemeinsame Produktion-/Schema-/Node-/Root-/Architecture-/eigenes Scope-/Selftestprüfungen erfolgreich, echte Unity-CI und Artefakte standgebunden beurteilt; alter FAIL9 separat erhalten. AK-05 getrennte gezielte unabhängige technische und fachliche Prüfung tatsächlich erbracht oder konkreter Ausführungsblocker mit prüffertiger Übergabe, keine Selbstabnahme. AK-06 eigener vollständiger Status/Branch/PR/Commit/Tree-/Nachweis-/Restgatehandover, kein Gesamt-WP026-/Human-/Import-/Release-PASS.

## Tests

1. Eigenen sowie historische gemeinsame Erst-Add-Anker/Parent/Manifestbytes mit Git prüfen; vollständige Elternbaum-/Konflikt-/Blobprovenienz nachrechnen. C060→neuerHEAD außerhalb der exakt erlaubten Dokumentations-/Zuordnungspfade unverändert. Tatsächlichen Main-Ref getrennt feststellen.
2. Beide Hashinventare gegen kanonische Gitblobs/Bytecounts prüfen; vollständige Originalreportdekodierung/Originalhash erhalten; unabhängige Original-QC-Bytes identisch. Git diff --check über tatsächliche gestagte/finale Dateien.
3. Bestehende vollständige Produktionsverify, aktueller Schema-/Node-/Rootweg tatsächlich im neuen gemeinsamen Checkout. Ausgaben außerhalb erzeugter Contentdateien sichern, diese anschließend auf unveränderte Gitbytes zurückführen. Eigener kanonischer Linux CPython3.11.13/Node22.20.0/Lock-Scope plus --self-test, keine Gateausnahme.
4. Echter regulärer CI-Run auf neuem gemeinsamen Freeze; originale Test-XMLs/350 Samples/Fingerprint und Plattformjoblogs tatsächlich prüfen. Frühere PASS/FAIL nur als ihre eigenen Ergebnisse führen. Gezielte unabhängige Originalberichte und neue Diff-/Integrationsprüfung getrennt angeben.

## Risikoklasse

Hoch: gemeinsame Integrationsvorbereitung und Scope-/Nachweisgrenzen. Technischer PASS ist keine Produkt-/Human-/Main-/Import-/Releasefreigabe.

## Definition of Done

Projektweite DoD unverändert. AK einzeln am tatsächlichen neuen Prüfstand nachweisen, eigenes Paket/CURRENT_STATE und PR aktualisieren. Originalgesamtabnahme und alle historischen Einzelmanifest-/Prüfstände bleiben unverändert. Keine Ableitung eines Gesamt-PASS aus erledigten Teilaufgaben; O-02-Zuverlässigkeit und sämtliche Human-/Fremdgates ausdrücklich getrennt offen.

## Tatsächliche Ausführung und Übergabe

Anker fdee9569eb52ee66b23028433c86d5d9f56dacdb, genau ein Parent C060, Manifest unverändert. Dokumentarischen Korrekturfreeze9c1859f im gemeinsamen37aff06 erhalten; keine neue Produkt-/Test-/Level-/Proof-/Regeländerung. Lokale tatsächliche Eingangs-/Produktions-/Schema-/Node-/Rootprüfung und getrennte Original-Nachprüfungen in [WP-032_HANDOVER](../PROJECT_CONTROL/Integration/WP-032_HANDOVER.md). Abschließende Scope-/CI-/unabhängige Ergebnisse stets am tatsächlichen gemeinsamen Freeze/PR nachweisen; keine vorweggenommene Gesamtfreigabe. O02 und Human-/Fremdgates bleiben offen.

## Aktuelle standgebundene Schlussnachführung – WP-033

Diese Nachführung ändert ausschließlich Bearbeitungs-/Ergebnisstatus, keine Definition, AK, Scopegrenze oder historische Manifestbasis. [Vollständige aktuelle Nachweise und Grenzen](../PROJECT_CONTROL/Integration/WP-033_HANDOVER.md). Frühere Ausführungsabschnitte bleiben ihre standgebundene Historie; keine Umdeutung alter FAILs und keine Gesamtfreigabe.
