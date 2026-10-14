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
| Q-02 | **Konkrete Mängel behoben, ein Auslegungspunkt offen.** Erste Abschlüsse 01-01-11/12 enthalten nun jeweils Null- und Vollmuster. 05-03-01 hat alle neun Achsen aktiv und keine öffentliche Nullzahl statt sechs leerer Spalten. Zusätzlich 02-04-05 für entfernte Gebiete und 05-02-03/04/05 für alleinigen mittleren Hinweiszugriff korrigiert. **DECISION-WP026-R01-FIRST:** Bible §4 nennt 9/10 „erste unterschiedliche A/B-Kanten“, frühe Felder haben bereits angrenzende oder gegenüberliegende Seiten. Unklar ist, ob „erste“ deren früheres Auftreten ausschließt oder den expliziten Unterrichtsschwerpunkt meint. Master/ADRs bestätigen keine engere Start-Portfolge. Manus-/Produkteigentümerentscheidung erforderlich; kein eigenmächtiges Verbot früherer Kanten oder Änderung der Planung. Die übrige Nachbesserung ist davon unabhängig. |
| Q-03 | **Technischer Befund bestätigt; daraus abgeleiteter zwingender Mangel begründet widerlegt.** Alle aktuellen Aufgaben lösen ohne globale NO_LOOP/CONNECTIVITY-Schritte; die Archivpropagation bestätigt das. Keine Quelle verlangt eine Regelquote. Das Abschnitt-4-Lernziel betrifft das Erkennen global nicht passender Verbindungen; solche Abkürzungen scheitern in den individuell benannten Feldern bereits an entfernten Counts und Gegenports. Globaler Zusammenhang und menschliche Lernwirkung bleiben offen zur fachlichen Abnahme, ohne eine neue Regelpflicht einzuführen. |
| Q-04 | **Risiko konkret offen zum Spieltest; keine automatische Austauschentscheidung.** Alle 20 Abschlussübergänge auf tatsächlicher Geometrie bewertet, technische 10→11-Tiefen separat in `repair-summary-facts.json`. Dünne serielle Abschlüsse und dichte Formabschlüsse haben unterschiedliche Last; insbesondere die Finales 05-04-11/12 (42/40) und der neue Wechsel Route 18, Gruppe 3→6, sind explizite Risiken. Es gibt weder eine bestätigte menschliche Schwelle noch Spieltestdaten, deshalb keine behauptete finale Schwierigkeit oder willkürliche Tiefenkappung. |
| Q-05 | **Briefbezogene Einzelmängel behoben; pauschaler Flächenmangel widerlegt.** Kleine Wiedereinstiege und räumliche Dichtevarianten sind in Bible §2/§§4–8 ausdrücklich möglich. 05-03-01 wird wegen „ohne Vereinfachung“ geändert; mehrere frühe Verdichtungs-/Gebietsfelder aus Q-01/Q-02 ebenfalls. Verbleibende reduzierte Achsen in 1/2 oder 9/10 sind je ID begründet. Die Prüfersicht 70 % wird ausschließlich beschrieben, nicht zur neuen Annahmegrenze gemacht. |
| Q-06 | **Aktuelle Kuratierung behoben; ursprüngliche Provenienzlücke konkret offen.** Der frühere Score-/Perzentilprozess belegt keine damalige vollständige Handkuratierung. Neue Einzelurteile und die 15 ausdrücklichen Auswahlentscheidungen sind persistiert. `candidate-provenance.json` enthält echte Poolbytes-/Hash-/Kandidatenbezüge und exakte aktuelle Matches aller 240 Felder. Historische Originalseedaufrufe und zwischenzeitliche Generatorsnapshots sind nicht vollständig persistiert, werden nicht erfunden und sind nicht rückwirkend als reproduzierbare Auswahl ausgegeben. Der aktuelle kanonische Korpus/Proof ist vollständig reproduzierbar. Ob weitergehende historische Auswahlreproduktion für die Abnahme nötig ist, bleibt der unabhängigen Prüfstelle vorbehalten. |
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
