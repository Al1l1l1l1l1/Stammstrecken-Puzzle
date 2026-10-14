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
