# WP-036 – Vollständige Textbibliothek

## ID
`WP-036`

## Ziel
Den bestehenden Manus-Produktionsblock „Vollständige Textbibliothek“ tatsächlich als zusammenhängenden deutschen Originaltextbestand liefern: 240 Meldungen mit 960 vorhandenen Textreferenzen, fünf Abschnitts- und zwanzig Routenbögen, entschiedene Oberflächen und wiederkehrende Ereignisse. Prüffähige Entwürfe, keine eigene Produkt-/Releasefreigabe.

## Voraussetzungen
Ausdrücklicher Owner-Ausführungsauftrag vom 17.10.2026, vollständig in dieser Definition operationalisiert. AGENTS-Lesefolge, PROJECT_CONTROL-Regeln, Konzepte 03/05/06/07/08/11/12/13/14, CONTENT_PIPELINE §8, CONTENT_CATALOGS, ADR-011/026/030. Frischer Fetch: main d90e798dd9b26d3f152f3b0c2d1d52cae641eb16. Alle Remote-Branches und historischen WP-Erstadds geprüft: höchste vergebene Nummer 035, kein Text-WP vorhanden. WP-026/PR21 dc1652befd7545b4c2a2dd0580f35a4e8e05d72d und WP-035/PR30 9e3f209cd92d491ab3a1ab51cce63677d3b5d53e ausschließlich lesbare Fachquellen. Eigene Branch von main, keine Integration fremder WPs. Gemeinsamer Erst-Add dieses WP und des [unveränderlichen Scope-Manifests](../tools/architecture-validation/scopes/WP-036.production.scope.json) vor Textproduktion.

## Scope
Neue kanonische de-DE-Textquelle samt eigenem Authoringschema; stabile Originalschlüssel aus tatsächlichen WP-026-Leveln und ungelöstem Referenzinventar lesen. Jede interne Einstiegserkenntnis an Authoring-/Leveldesignquelle und tatsächlichen öffentlichen Input binden, ausdrücklich nicht automatisch sichtbar. Playertexte ohne Lösungsverrat. Kampagnentexte nur zu bestehenden Momenten; vollständige Texte zu entschiedenen UI-Funktionen und neutralen Fehler-/Ladezuständen. Offene Hint-/Daily-/Endless-/Store-/Kosmetikregeln bleiben gesperrte redaktionelle Entwürfe oder dokumentierte Entscheidungslücken. Varianten liefern keine neue Auswahlmechanik. Eigene lokale Prüfhilfe, Schema-, Key-, Zeichen-, Platzhalter-, Duplikat-, Pseudolocale- und Referenzprüfungen samt Negativproben; kritische redaktionelle Gesamtprüfung und begründete Stichprobe. Herkunft, Quellenhashes, Befund-/Freigabegrenzen, Übergabe dauerhaft sichern. Bestehenden CI-Workflow ausschließlich auf eigenes production-Manifest umstellen und eigenen Textcheck additiv einhängen; alle bestehenden Schritte/Gates erhalten. Draft-PR gegen main, kein Merge. Temporäre Arbeitsdateien ausschließlich im ignorierten .utmp/.

## Betroffene Dateien/Module
Neu: Content/Localization/source/de-DE.json; Content/Localization/source-v1.schema.json. Neu: Content/Reviews/text-library/WP-036/ mit README.md, EDITORIAL_RULES.md, EDITORIAL_REVIEW.md, COVERAGE.md, HANDOVER.md, QC.md sowie *.json/*.txt als konkrete Prüf-/Quellen-/CI-/Pseudolocale-Nachweise. Neu: tools/text-library/*.py und README.md als eigene Produktions-/Prüfhilfen. Eigene WP-Datei und Manifest. PROJECT_CONTROL/CURRENT_STATE.md und WORK_QUEUE.md ausschließlich additive eigene Statusnachführung bzw. eigene Tabellenzeile. .github/workflows/validate.yml ausschließlich genannte Auswahl und additive Prüfung. Lesend alle auftragsrelevanten Quellen einschließlich gepinnter Gitobjekte aus WP-026/WP-035. Keine fremden Quellen in den Lieferbaum importieren.

## Ausdrücklich nicht erlaubte Änderungen
Keine fremden WPs/Manifeste, Konzepte, Architektur-/ADR-Verträge, Solver, Level, Lösungen, Proofs, Assets, Kataloge, Runtime/Importer, Zeitwerte oder Werbe-/Hilferegeln ändern. Keine neuen Routen, Figuren, Ereignisse, Preise, Objekte oder Freischaltregeln. Keine bestehenden Qualitätsgates/Validatoren lockern. Keine rechtliche, kreative, Produkt-, Import- oder Releasefreigabe behaupten. Keine Main-Integration. Die übrigen 480 Completion-/World-Ziele bleiben fremder Scope.

## Akzeptanzkriterien
- AK01: Genau 240 vorhandene IDs in 5×4×12 Reihenfolge und alle vier tatsächlichen Keys (960/960) mit vollständigen Texten; 240 interne Einstiege explizit von 720 Spielertexten getrennt.
- AK02: 240 nachvollziehbare Quellenbindungen einschließlich Levelbytes, Authoring/Review und mathematisch prüfbarer Einstiegsaussage; keinerlei erfundene Rätsellogik.
- AK03: Fünf Abschnitts- und zwanzig Routenbögen mit bestehenden Einleitungs-/Freigabe-/Abschlussmomenten und trockenem Seasonabschluss, Eskalation bis zum SEV.
- AK04: Flächen-/Zustandsinventar für alle im Auftrag benannten UI-Bereiche; vollständige entschieden formulierbare Texte, explizite Abhängigkeiten statt erfundener Regeln. Feste Begriffe unverändert.
- AK05: Varianten für alle neun benannten wiederkehrenden Anlässe, semantisch gleicher Anlass ohne Auswahlalgorithmus oder variable Belohnungsversprechen.
- AK06: Jede Quelle enthält Text, Beschreibung, vorgeschlagenes Zeichenlimit, erlaubte typisierte Platzhalter, Status, Sichtbarkeits-/Freigabegrenze; Kurzfassungen bei langen Spielertexten. Neue Limits sind redaktionelle Vorschläge, keine UI-/Accessibilityentscheidung.
- AK07: Gesamte Syntax-/Schema-/Key-/Referenz-/Platzhalter-/Zeichen-/NFC-/Duplikatprüfung und Pseudolocale erfolgreich; absichtliche Negativmutationen werden erkannt. Kein unbemerkter Laufzeitimport.
- AK08: Kritische redaktionelle Gesamtprüfung gegen 11/12/14 und Stichprobe aus Anfang, mehreren mittleren Routen und Finale mit begründeten Urteilen; eigene Kontrolle klar von unabhängiger QC getrennt.
- AK09: Eigener ADR030-Scope, Architektur-Selftests, reguläre unveränderte CI und Draft-PR mit tatsächlichen standgebundenen Ergebnissen; fehlende Nachweise als offen. Gesonderte unabhängige Abschlussprüfung am eingefrorenen Stand vor etwaiger Integration.
- AK10: Vollständige Repoübergabe mit Umfang, Fundorten, 960-Abdeckung, Prüfungen, offenen Produkt-/Fremdgates, Branch/PR/Commit; Statusdateien aktuell.

## Tests
Eigener headless Textchecker auf gesamter Quelle mit jsonschema aus bestehendem Lock: Eindeutigkeit, exakte Saisonhierarchie/Keys, gepinnte Fremdquellen via Gitobjekte (kein Import), 240 Einstiegsbelege, Limits/Kurzformen, erlaubte Platzhaltertypen, feste Begriffe, UTF-8/NFC, keine fertigen Sonderregeln, Wiederholungsbericht, Pseudolocale als Layoutprüfmaterial. Negativproben für fehlenden Key, Duplikat, unerlaubten Platzhalter, Überlänge, falschen Levelbezug und sichtbaren internen Einstieg. Unveränderter Architekturvalidator --self-test sowie --scope production --scope-manifest tools/architecture-validation/scopes/WP-036.production.scope.json --self-test; git diff --check. Reguläre Architecture-/Unity-CI über bestehenden Workflow, nicht vorab als PASS. Lokale Unity-Neubuilds, UI-Geräte-/Screenreader-/Doppelimport-/Addressables- und Releaseprüfungen mangels Runtime-/Importänderung nicht als durch diesen Textauftrag erbracht behandeln; entsprechende Produkt-/Importgates bleiben offen.

## Risikoklasse
Mittel. Umfangreiche redaktionelle Produktionsquelle mit Spoiler-, Ton-, Bindungs- und UI-/Monetarisierungszusagerisiken; keine Runtime-/Architekturänderung. Sichtbarkeits- und Freigabemetadaten halten offene Funktionen gesperrt.

## Definition of Done
Alle AK einzeln mit tatsächlichen Nachweisen prüfen; vollständige Texte und reproduzierbarer Checker ausführbar, vorgeschriebene CI tatsächlich ausführen und Ergebnis binden. Eigenprüfung ersetzt keine unabhängige Abschluss-QC und keine Ownerannahme. Bei fehlender CI/QC/Produktannahme bleibt der betreffende Status offen. CURRENT_STATE/WORK_QUEUE und eigene Übergabe nennen letzten und nächsten Schritt, alle Grenzen. Manifest bleibt byteunverändert. Kein automatischer Merge.

## Bearbeitungsstatus
Prospektiv verankert; Textproduktion beginnt erst nach gemeinsamem Ankercommit. Noch keine Abschlussprüfung oder Produktannahme.
