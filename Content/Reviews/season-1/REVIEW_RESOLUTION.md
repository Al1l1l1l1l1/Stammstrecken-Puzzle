## Aktueller Abschlussstand der technischen Nacharbeiten – WP-033, 15.10.2026

F-01 ist sachlich korrigiert, Revision2→3 und kanonische Metadaten-/Nachweisbindungen konsistent; beide unabhängigen Rollen bestätigen die Korrektur. Alle240 öffentlichen Inputs, Lösungen und Proofbytes erhalten. Drei später gefundene P2-Status-/Byteprofil-/Verfahrensbefunde sind auf 9c1859f gezielt unabhängig geschlossen. O-03: Manus' WP-027-Auslegungen entschieden, WP-028-CI-Leistungsnachweis erbracht und Gesamtabnahme b1457ba tatsächlich durchgeführt, weiterhin kein Gesamt-PASS; Originalberichte unverändert.

O-04 ist im zulässigen gemeinsamen Nicht-main-Arbeitsstand unter eigenem WP-032 technisch erbracht: Freeze B83, eigene unveränderte prospektive Verankerung auf C060, getrennte vollständige Herkunfts-/Einzelanker-/Manifest-/Konfliktabnahme, neue tatsächliche gemeinsame Produktion/Schema/Node/Root, Architecture17+Scope18 sowie alle zehn regulären Unity-Jobs bestanden (455 EditMode,27 PlayMode, Android-Development-IL2CPP, iOS/Xcode). Beide unabhängigen Rollen und gesondertes technisches CI-Schlussaddendum bestätigen ihren tatsächlichen Umfang; keine Selbstfreigabe. Der alte WP-031-Importscope bleibt als historischer FAIL9 unverändert. Keine Erweiterung alter Manifeste, keine neue Architektur-/Validatorregel, keine Testlockerung.

O-02 wurde anhand Originalbefund, vollständiger Originaljoblogs und gezielter Gegenprüfungen untersucht; Ursache unbewiesen. Eng vorgeschlagene reguläre Phasen-/CPU-/GC-/Allokationsdiagnostik bleibt zur Entscheidung, keine Reparatur ausgeführt. Menschlicher Spieltest,1440 Referenzziele/Cross-Reference-FAIL,UNCALIBRATED/null Sternsekunden sowie kreative/Import-/Produkt-/Release-/Geräte-/Storegates bleiben offen. Kein Gesamt-WP-026-PASS, keine Main-Integration, kein Runtimeimport oder Release.

Diese WP-033-Nachführung verändert ausschließlich Status-/Evidenz-/eigene CI-Zuordnungsdokumentation. Sämtliche technischen Quellen und Inputs bleiben exakt zu B83 erhalten; neue Scope-/Diff-QC-/nachgelagerte CI-Ergebnisse werden separat am tatsächlichen Dokumentationscommit/PR gebunden. Die folgenden älteren Kopfblöcke und Tabellen sind standgebundene Historie des technischen B83-Arbeitsstands und seiner Vorgänger; dort früher ausstehende Prüfungen sind durch die hier verlinkten tatsächlichen Schlussnachweise fortgeschrieben.

[Aktuelle vollständige Übergabe](../../../PROJECT_CONTROL/Integration/WP-033_HANDOVER.md).

## Aktueller gemeinsamer Prüfstand – WP-032, 15.10.2026

F-01 ist sachlich korrigiert und durch beide unabhängigen Rollen bestätigt; auf dem Korrekturfreeze 9c1859f sind QC-CONTENT-01 und QC-TECH-01/02 gezielt nachgeprüft und geschlossen. Kanonische Gitblobbindungen sind explizit, Originalberichte unverändert. Manus' WP-027-Auslegungen sind entschieden, WP-028-CI-PERF und die durchgeführte Gesamtabnahme auf b1457ba bleiben standgebundene Teilnachweise ohne Gesamt-PASS.

O-04: eigener prospektiver WP-032-Anker fdee9569eb52ee66b23028433c86d5d9f56dacdb, einziger Parent/Manifestbasis C060. Beide Lieferungen und die drei Konfliktlösungen bleiben erhalten; 1109 Nichtkonfliktpfade und unveränderte Produktions-/Mainquellen sind zusätzlich zur eigenen künftigen Scopeprüfung nachgewiesen. Die geprüfte dokumentarische Nachführung 9c1859f ist im gemeinsamen Folgecommit 37aff06 erhalten. Darin erneut tatsächliche Produktion (240×3, Strict/3840/1440), Schema240/240, Node1200 und Root240 bestanden. Neuer finaler Scope, reguläre CI und zwei gezielte unabhängige Rollen werden separat an den tatsächlichen WP-032-Freeze gebunden; deren verbindlicher Schlussstand steht in der WP-032-Übergabe und den PR-Nachweisen. Der alte WP-031-Importscope bleibt FAIL9, kein rückwirkender PASS und keine neue externe Architekturentscheidung behauptet.

O-02 ist untersucht, historische Ursache unbewiesen; konkrete Testdiagnostik bleibt zur Entscheidung. Menschlicher Spieltest,1440 Referenzziele, Zeitklasse und kreative/Import-/Produkt-/Release-/Geräte-/Storegates bleiben offen. Kein Gesamt-PASS und kein Merge nach main.

Die nachfolgende Nachführung ist unveränderte standgebundene Historie des Korrekturfreezes 9c1859f; dort genannte künftige WP-032-Schritte sind durch diesen neuen eigenen Prüfstand fortgeschrieben.

## Aktueller Originaltransport / WP-031 – 15.10.2026

Die Gesamtabnahme ist am verlinkten TXT-Pfad als **STP_ORIGINAL_REPORT_BASE64_V1** verlustfrei gespeichert. Original-SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433 bezeichnet die vollständig dekodierten Originalbytes, einschließlich der zusätzlichen Schlussleerzeile. Der Transportcontainer hat einen eigenen Dateihash in F01_BINDINGS.json. `tools/wp031-integration/prepare.py decode --output <Scratchdatei>` stellt die unveränderte Originaldatei wieder her und prüft ihren Hash. Kein Originaltext wurde getrimmt oder redigiert. Der unveränderte Klartext und das isolierte Archiv liegen weiter in der ursprünglichen externen Auditablage.

Eigener erster Nacharbeitsstand f8e1dfc: Architecture Run38651701033 / Job116151786197 scheiterte tatsächlich an `scope:diff-check`: zusätzliche Schlussleerzeile des unveränderten Originalberichts. Lokaler vorheriger Scope lief vor diesem Commit mit untracked Originaldatei; dessen Whitespaceprüfung deckte die erst eingecheckte Schlussleerzeile nicht ab. Der neue verlustfreie Transport behebt die eigene Persistierungsgrenze, ohne Originalbytes oder Qualitätsanforderung zu ändern. Der tatsächliche Lieferstand 93b49a9 hat Architecture und sämtliche zehn Unity-Jobs einschließlich beider Plattformkompilationen bestanden. Nachgelagerte Dokumentations-/Bindungskorrekturen werden separat geprüft; keine Übertragung eines alten PASS auf geänderte Inhalte. O-02 bleibt ursächlich offen, keine Gesamt-DoD-Freigabe.

## Aktuelle Nachführung zur Gesamtabnahme – 15.10.2026

Stand der Originalgesamtabnahme: `b1457ba6a61826fe3b64a83323c27a13e9d23080`; Quellenstand main: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Historische Manifestbasen bleiben WP-026=c9516bd und WP-028=d97c39d. Keine Umdeutung des PR-Metadatenfelds base_sha zu aktuellem main.

**WP-027 entscheidet R01-FIRST und R18-MIDDLE.** Bible §§4.1/8.1: erster gezielter Variationsschwerpunkt, frühere Portseitenvielfalt zulässig; vorbereitender lokaler Einser-Portanker zulässig, weitere Lösung kombiniert mittlere Resthinweise. Keine neue Port-/Einserquote. Quellen: [WP-027 auf main](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/WORK_PACKAGES/WP-027_Season-1-Quellenklarstellung-und-CI-Leistungsweg.md), dort bestätigte Bible und ADR-032.

**CI-PERF-WP026 auf b1457ba erbracht.** WP-028: Freeze b449ab7, PR #25 ausschließlich nach WP-026 integriert als b1457ba, gleicher Tree 2bc50e6. Historische Unity-CI auf b1457ba [38637012837](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012837): 455/455 EditMode, 27/27 PlayMode, Android IL2CPP und iOS/Xcode erfolgreich; Architecture [38637012841](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012841) erfolgreich. 350 Samples, p95-Index 332, p95 38,2026 ms, Maximum 72,0952 ms, 240 frische Proofgates in 2.586,8097 ms mit vier Arbeitern. Git/Linux-Fingerprint 6fc307231c43d3f09c60fa3116c8517588a40855c4266e8ec4dd120ed0b6372a; Windows-CRLF-Fingerprint 29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679. Dies sind Nachweise dieses historischen Standes, kein vorweggenommener PASS neuer Metadaten.

**Neue unabhängige Gesamtabnahme durchgeführt, KEIN GESAMT-PASS / NICHT INTEGRATIONSBEREIT.** [Originalbericht](WP-026_Gesamtabnahme_b1457ba.txt), byteunverändert, SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433. Evidenzarchiv SHA256 e1a974ebfdf1f87ee9af6bb08981563e8ce3a5a3166c0c21aada92d81cfb4258, ausschließlich isoliert. Alle 240 logischen Rätsel/20 Routen gegen WP-027 geprüft; F-01 begrenzter Sachtextbefund, keine neue Puzzlelogikabweichung. Originale frühere Prüfberichte bleiben unveränderte standgebundene Historie.

F-01 ist unter dem bestehenden WP-026-Metadatenscope und separat verankerter WP-029-Nachweisführung korrigiert: x0=0, x1=2, x3=6, x4=5. Ein Level Revision 2→3, kein neuer öffentlicher Input, keine neue Lösung oder Proofbytes. Reale neue Bindungs-/Regenerationsnachweise und Grenzen stehen in [ACCEPTANCE_FOLLOWUP.md](ACCEPTANCE_FOLLOWUP.md). Separate unabhängige Fachprüfung auf 93b49a9 bestätigt F-01 einschließlich eigener 240er-Metadaten-/Bytegegenprüfungen. Ihr Statusbefund QC-CONTENT-01 und der technische Byteprofilbefund werden hier eng korrigiert; gezielte Nachprüfung des Korrekturdiffs bleibt erforderlich.

O-04: Die drei Konflikte sind im gemeinsamen Preview 060c1c4 tatsächlich gelöst; der kombinierte Baum wurde mit Produktion, Schema/Node/Root, Architektur und vollständiger Unity-CI (455 EditMode, 27 PlayMode, Android, iOS/Xcode) geprüft. Das dort aufgerufene unveränderte WP-031-Scopegate meldet neun Fehler beim Main-Import; dieser FAIL bleibt erhalten. Die unabhängige technische Gegenprüfung weist einen prospektiven neuen engen Auftrag auf Parent 060c1c4 als grundsätzlich zulässigen ADR-030-Weg nach; dafür ist WP-032 separat zu verankern und die Herkunft beider Lieferungen plus Konfliktdelta unabhängig abzunehmen. Kein pauschal behaupteter neuer Architekturentscheid erforderlich, kein nachträgliches Grünfärben von WP-031. Weiter offen: O-01 menschlicher Spieltest, O-02 historische Timeoutursache, O-04 dieser getrennte Integrationsvorbereitungs-/Prüfschritt, O-05 1.440 Referenzziele, O-06 Zeitklasse sowie O-07 kreative/Import-/Produkt-/Release-/Geräte-/Storegates. WP-030 hat die Untersuchung geliefert, keine kausale Reparatur. Keine Erweiterung alter Paketdefinitionen oder Manifeste, kein Gesamt-PASS, Main-Merge, Runtimeimport oder Release. Aktuelle gezielte QC-Ergebnisse und Korrekturen bleiben an ihren tatsächlichen Prüfstand gebunden.

Die nachfolgenden älteren Abschnitte beschreiben ihren damaligen Stand. Ihre überholten Entscheidungs-/CI-PERF-Statusangaben sind keine aktuelle Blockerliste.

# WP-026: Bearbeitung der unabhängigen Prüfung zu PR #21

Ausgangsstand `3c9b7d0b3e74bd79c0ba4e1c0a20d9379b697d5e`. Bestehender Auftrag,
Branch, PR und Manifest bleiben erhalten. Der Originalbericht ist unverändert
als `WP-026_Unabhaengige_Pruefung_PR21_3c9b7d0.md` persistiert, SHA256
`7c5782eb33517f244eb9f6e102ecc2acb0ebb8acb48622feff83916d49918130`.
Das Prüferarchiv hat SHA256
`e3d197b48bb9b91dcdbe50a6f6021bb71f90571784b4cc678501de6bfd23c078`
und bleibt isoliert in ignoriertem Scratch. Es ist Prüfevidenz, keine Spezifikation.

## Quellen und korrigierte Auftragsermittlung

AGENTS-Lesereihenfolge, CURRENT_STATE, WP-026, Konzept 03/04/14, angenommene
ADRs und Architekturverträge wurden zugrunde gelegt. `ORDER_CORRECTION.md`
bleibt maßgeblich: Der Koordinatorzusatz ist keine Nutzervorgabe; individuelle
offene Entwurfsreferenzen sind zulässiges Authoring, keine behaupteten Zielquellen.
BLOCKER-WP026-001 wird nicht wieder als Erstellungsstartblocker eingeführt.

CONTENT_PIPELINE §3.2 verbietet erfundene Qualitätsnotizen/Einstiege, kein
allgemeines Skript. Konzept 04 §§2/4/6 und Bible 14 §§1/2/4–9 verlangen echte
Kuratierung und fachliche Beurteilung des Lernzwecks. Diese Auslegung ist durch
die tatsächliche individuelle Bearbeitung umgesetzt, ohne eine Projektregel zu
ändern: Tools liefern Fakten oder übernehmen explizite Urteile getreu. Die neue
Bewertung ist eigene Autorarbeit; Spieltest und unabhängige Rollen bleiben getrennt.

## Befundregister

| ID | Status und konkrete Bearbeitung |
|---|---|
| V-01 | **Behoben.** Vollständige Count-L1-Sicht unter D4 und A/B-unabhängigen Ports, ergänzende Belegungsüberlappung, 240 nächste Nachbarn und sämtliche 3.183 passende Paare. Exakte öffentliche Kopien weiterhin 0; vollständige Fakten statt gleichorientiertem Mindestabstand. Verbleibende Nähe individuell in `PAIR_ADJUDICATION.md` entschieden. Keine neuen Qualitätsgrenzwerte. |
| V-02 | **Behoben als eigene Authorleistung.** Alle 240 Raster/Ports/Counts/Lösungen und konkreten Folgezusammenhänge gelesen; 20 unterschiedliche Routenurteile und 240 literal verfasste Entscheidungen in `author-decisions.txt` / `author-review.json`. 15 gezielte Logikänderungen, 225 Inputs erhalten. Die alten vier Routenabsätze und pauschalen Notizen ersetzen diese Arbeit nicht und sind entfernt. Unabhängige Bestätigung dieser Leistung noch erforderlich. |
| V-03 | **Behoben im beauftragten Umfang, Auslegungsvorbehalt eng behandelt.** Kein automatisch formulierter Bewertungs-/Einstiegstext; Fokus und vorläufige gedankliche Kettenschätzung individuell verfasst, von den technischen Rootmetriken getrennt. Hashgebundener Transport prüft aktuellen öffentlichen Input und Lösung vor jedem Schreibzugriff; reale stale-Public-/Lösungsgegenproben verworfen. `--annotate` entfernt, Kandidatenskript ohne Autorurteile. Kein pauschales Skriptverbot und keine neu erfundene Regel. |
| Q-01 | **Bestätigte lokale Redundanz behoben; restliche Nähe begründet behalten.** Der wiederholte Zweispaltenkorridor 01-04-08, Vollstreifen 02-02-06/02-04-07, direkte Serpentinenzwillinge 03-02-02/04-03-08 und späte Wiedereinstiege 04-04-02/05-02-02/05-04-02 ersetzt. Die 15 verbleibenden Paare der deskriptiven ≥0,85-Sicht einzeln beurteilt; wiederkehrende Familien bleiben für externen Contentreview sichtbar. Keine automatische Neuheitsfreigabe aus abweichenden Hashes. |
| Q-02 | **Konkrete Teilmängel behoben, zwei Auslegungen offen.** Null-/Vollmuster in 01-01-11/12, entfernte Gebiete in 02-04-05/07 und volle 9×9-Achsenausdehnung in 05-03-01 sind belegt. Route 18/3–5 besitzt keine 0-/Vollzahlen und notwendige mittlere Restkombination, aber direkte Einser-Portanker. DECISION-WP026-R01-FIRST und DECISION-WP026-R18-MIDDLE bleiben konkrete Eigentümerentscheidungen; Details und IDs im Nachprüfungsabschnitt. Keine eigene engere Portfolge oder Einsergrenze. |
| Q-03 | **Technischer Befund bestätigt; daraus abgeleiteter zwingender Mangel begründet widerlegt.** Alle aktuellen Aufgaben lösen ohne globale NO_LOOP/CONNECTIVITY-Schritte; die Archivpropagation bestätigt das. Keine Quelle verlangt eine Regelquote. Das Abschnitt-4-Lernziel betrifft das Erkennen global nicht passender Verbindungen; solche Abkürzungen scheitern in den individuell benannten Feldern bereits an entfernten Counts und Gegenports. Globaler Zusammenhang und menschliche Lernwirkung bleiben offen zur fachlichen Abnahme, ohne eine neue Regelpflicht einzuführen. |
| Q-04 | **Risiko konkret offen zum Spieltest; keine automatische Austauschentscheidung.** Alle 20 Abschlussübergänge auf tatsächlicher Geometrie bewertet, technische 10→11-Tiefen separat in `repair-summary-facts.json`. Dünne serielle Abschlüsse und dichte Formabschlüsse haben unterschiedliche Last; insbesondere die Finales 05-04-11/12 (42/40) und der neue Wechsel Route 18, Gruppe 3→6, sind explizite Risiken. Es gibt weder eine bestätigte menschliche Schwelle noch Spieltestdaten, deshalb keine behauptete finale Schwierigkeit oder willkürliche Tiefenkappung. |
| Q-05 | **Briefbezogene Einzelmängel behoben; pauschaler Flächenmangel widerlegt.** Kleine Wiedereinstiege und räumliche Dichtevarianten sind in Bible §2/§§4–8 ausdrücklich möglich. 05-03-01 wird wegen „ohne Vereinfachung“ geändert; mehrere frühe Verdichtungs-/Gebietsfelder aus Q-01/Q-02 ebenfalls. Verbleibende reduzierte Achsen in 1/2 oder 9/10 sind je ID begründet. Die Prüfersicht 70 % wird ausschließlich beschrieben, nicht zur neuen Annahmegrenze gemacht. |
| Q-06 | **Aktuelle Kuratierung nachgebessert und auf 96697ab unabhängig fachlich bestätigt; ursprüngliche Provenienzlücke bleibt dokumentiert.** Score-/Perzentilauswahl belegt keine damalige Handkuratierung. Aktuelle Einzelurteile, 15 Auswahlentscheidungen, echte Poolhashes und 240 vollständige Matches sind nachvollziehbar. Nicht persistierte Originalseedaufrufe/Generatorsnapshots bleiben unbekannt; der heutige kanonische Korpus/Proof ist reproduzierbar. Keine zusätzliche rückwirkende Kampagnen-Seedpflicht aus Endless-Regeln ableiten. Die drei neuen Textkorrekturen benötigen ihre tatsächliche Nachprüfung auf neuem Freeze. |
| O-01 | **Behoben.** Nullbasierte Koordinaten/Linien-/Portindizes ausdrücklich in allen 20 Berichten und im Autorenmaterial. |
| O-02 | **Behoben.** Node 22.x wie WP Tests 1 akzeptiert; tatsächliche Patchversion berichtet, die ausgeführte CI-/lokale Referenz bleibt 22.20.0. Kein Lock-/Workflowwechsel. |
| O-03 | **Behoben.** Neue Konsolenendwerte verwenden dieselben ganzzahligen Mikrosekunden wie JSON; keine localeabhängige ms-Rundungsdarstellung mehr. Lokale Werte bleiben ausdrücklich lokale Prüflaufzeiten. |
| O-04 | **Teilweise recovered, konkrete Historienlücke offen.** Tatsächliches Poolinventar und ausgewählte Index-/Hashbezüge vorhanden; unbekannte Originalaufrufe/Toolstände siehe Q-06. |

## Eigene Korrekturen und Nachweise

Die 15 Logikänderungen stehen mit alten/neuen Inputs und Public-/Lösungs-/
Proofhashes in `targeted-changes.json`. Unveröffentlichter Draftbestand nach
LEVEL_DATA_FORMAT §3; keine veröffentlichte Identitätsänderung oder Migration.
225 öffentliche Inputs und 225 Proofbytes sind gegenüber `3c9b7d0…` identisch.
Alle 240 redaktionellen Metadaten sind bewusst neu bewertet (Revision 2).

Eigenfehler wurden vor dem Einfrieren korrigiert: falsche erste Abschätzung
der Endpointentfernung von 01-02-10 (tatsächlich 3); UTF-8-Ausgabe im lokalen
Vorschauharness; unzulässiger zusammengefasster Patch; kurzlebige WSL-Umgebung
und fehlender C++-Compiler durch isoliertes Setup in derselben Sitzung;
unnötiges Umschreiben unveränderter Proofs bei vorübergehender Windows-Dateisperre
durch Bytevergleich vor Materialisierung; sachliche Formulierung eines Umwegs;
eine erste Ersatzmaske von 01-01-12 wegen zu naher Wiederholung vor Freeze verworfen.
Keiner dieser fehlgeschlagenen Versuche ist als bestandener Lauf behauptet.
Nach jeder Input-/Dokumentkorrektur wurden die betroffenen realen Nachweise erneuert.

Die endgültigen Outputs heißen `repair-technical-validation.txt`,
`technical-evidence.json`, `schema-node-evidence.json`, `repair-countercheck-*.txt`,
`repair-regression-validation.txt` und `repair-regression-evidence.json` (Originalbytes; TXT als Lesefassung).
Die ursprüngliche Teil-PASS-Evidenz bleibt nur für ihren ursprünglichen HEAD
gültig. Autorbetriebene Prüfercode-Gegenprüfungen sind Zusatznachweise, keine
unabhängige Neuabnahme. Neue unabhängige Berichte müssen ihren eingefrorenen
HEAD selbst benennen; bis dahin gibt es keine Selbstfreigabe.

## Tatsächlich abgerufene CI-Evidenz und getrennte Leistungsgrenze

Unity-Lauf [38475032171](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38475032171)
ist an den **alten geprüften HEAD 3c9b7d0…** gebunden. Alle zehn Jobs SUCCESS;
vier echte Joblogs abgerufen. Die drei verfügbaren unexpired Artefakte wurden
heruntergeladen, SHA256 gegen GitHub verglichen, Inhalt gelesen und gehasht:
EditMode 454/454, PlayMode 27/27, Android Development IL2CPP APK; iOS Export/
Xcode-Build aus realem Log **BUILD SUCCEEDED**. Original-Test-XMLbytes,
Coveragezusammenfassungen, Logauszüge und sämtliche Artefakt-/Memberhashes in
`ci-baseline-*.json` / `ci-baseline-*.txt`. Kein bloßer Statusersatz, keine
Übertragung dieses Unity-Nachweises auf neue Contentinputs.

**CI-PERF-WP026 bleibt NOT_EXECUTED.** Die bestehenden Workflows führen die
Regressionen/Builds und Architekturcontracts aus, nicht den neuen 240er-Korpus
mit dessen 10×10-/Gesamtbudget. `unity.yml` ist außerhalb der Allowlist; bei
`validate.yml` ist ausschließlich die bestehende Manifestzuordnung erlaubt.
`workflow_dispatch` hat keinen zulässigen Korpus-/Leistungsparameter, kein
freigegebener Runner-Commandzugang ist vorhanden. Einen neuen CI-Aufrufschritt
hinzuzufügen würde die Paketgrenze ändern. Erforderlich ist ein separat
beauftragter CI-Ausführungsweg für WP Tests 5 / SOLVER_ARCHITECTURE §10 mit
Commit-/Toolchain-/Korpusbindung. Lokale Messungen ersetzen diesen Beleg nicht.

Referenzauflösung (1.440 fehlende Ziele), menschlicher Spieltest, vorläufige
Zeitklassenzuordnung, kreative Miniaturen/Präsentation, Pseudolocale, Doppelimport,
Addressables/Lizenz-/Größen-/Produkt-/Releasegates bleiben unverändert offen.
Kein Merge, Runtimeimport, Release oder Gesamt-DoD-PASS.

## Tatsächliche unabhängige Nachprüfung und eigene Sachkorrektur

Die getrennten vollständigen Rollenberichte auf `96697ab749fbea859fde8a4d4fcb42149cc2e89c`
sind unverändert als [technische Prüfung](INDEPENDENT_TECHNICAL_96697ab.md),
[zugehöriger CI-Nachtrag](INDEPENDENT_TECHNICAL_CI_ADDENDUM_96697ab.md) und
[fachliche 240er-/20-Routen-Prüfung](INDEPENDENT_CONTENT_96697ab.md) persistiert.
Der technische lokale Teil-PASS ist begrenzt; der CI-Nachtrag gehört zwingend
dazu. Die fachliche Prüfung bestätigt die individuelle Substanz und die
15 gezielten Eingriffe, entdeckt aber drei begrenzte sachliche Textfehler.
Keiner der alten oder dieser Teilbefunde wird auf Folgeänderungen übertragen.

| Neuer Befund | Eigene Bearbeitung und heutige Grenze |
|---|---|
| CF-01 / S1-03-02-01 | **Sachlich korrigiert.** Der frühere fast gedrehte 19-Zellen-Nachbar wird als Historie bezeichnet; der tatsächliche Ersatz an Position 2 hat 16 Zellen und andere Kopf-/Boden-Vollanker. |
| CF-02 / S1-04-02-02 | **Sachlich korrigiert.** Beide rechten Arme x4/x6 kehren gemeinsam auf y5 um; beide linken x0/x2 auf y6. Unterschied zwischen Armpaaren statt falschem Unterschied innerhalb des rechten Paares. |
| CF-03 / S1-05-01-02 | **Sachlich korrigiert.** Rechte Sechserarme kehren auf y5 um, linkes Vollarmpaar auf y7: zwei Zeilen Unterschied. |
| CF-SOURCE-01 | **Konkret offen.** DECISION-WP026-R01-FIRST bleibt wie oben; keine eigene Erstkontakt-Portregel. |
| CF-SOURCE-02 | **Konkret offen: DECISION-WP026-R18-MIDDLE.** Bible 14 §8, Route 18/3–5, verlangt „mehrere mittlere Hinweise als einziges Orientierungsmittel“. S1-05-02-03 startet über E7/rowCounts[7]=1, 04 über N7/columnCounts[7]=1, 05 über W0/columnCounts[0]=1; danach sind mehrere mittlere Reste nötig. Fehlende Eigentümerentscheidung: sind solche elementaren vorbereitenden Einser-Portanker zulässig, wenn die weitere Lösung mittlere Kombination erfordert, oder schließt „einziges“ diese Anfangsorientierung aus? Weder Null-/Voll-Abwesenheit allein noch ein eigenes Einserverbot beantwortet das. |
| CI-GATE-WP026-UNITY-FAIL | **Tatsächlicher Push-FAIL dokumentiert, Ursache offen.** Run 38490243240 / Job 115613547399: 454 Tests, 453 PASS, 1 Timeout nach 180.000 ms im unveränderten HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind; Folgejobs skipped. Getrennter PR-Run 38490247420 besteht denselben Test in 100,490521 s und EditMode 454/454, PlayMode 27/27. Ein erfolgreicher anderer Lauf beweist keine Timeoutursache; kein Test-/Workflow-/Produktionscode außerhalb des Scopes geändert, kein Retry zur verdeckten Grün-Erzeugung. Tatsächliche Schlussstände/Logs/Artefakte getrennt am PR und im CI-Nachweis. |

Die drei Einzelkorrekturen und das eng offene Routenurteil sind literal verfasst
und erneut getreu gebunden/transportiert. Nur diese drei Levelmetadaten steigen
auf Revision 3; alle öffentlichen Inputs, Lösungen und Proofbytes bleiben
gegenüber `96697ab` gleich. Der Transport erhält bei unveränderten Urteilen die
vorhandene Revision, statt spätere Revisionen wieder auf 2 zu setzen. Aktuelle
Dokumenthash-/Node-/Inventarbindungen werden tatsächlich erneut geprüft.

**Aktualisierte Einordnung Q-02:** Die konkret fehlenden Null-/Vollmuster,
Gebietsdarstellungen und 9×9-Achsenausdehnung sind korrigiert. Route 1 „erste“
und Route 18 „einziges“ bleiben beide Eigentümerentscheidungen; die Aussage,
der Route-18-Brief sei durch Entfernung von 0/Voll uneingeschränkt erfüllt,
wird nicht aufrechterhalten. Das aktuelle Routenurteil benennt den Einserzugang
und den ganzen Restzahlenzugriff ausdrücklich. Keine zusätzliche Inputänderung
ohne die fehlende bestätigte Lesart.

Nächster Schritt nach eigener Fehlerbehebung: neuer eingefrorener HEAD und
tatsächliche unabhängige Nachprüfung der korrigierten gebundenen Evidenz;
Ergebnisse/Scope/Remote/CI am bestehenden Draft-PR persistieren. Menschlicher
Spieltest, beide Quellenentscheidungen, CI-PERF-WP026 und Fremdgates bleiben offen.
