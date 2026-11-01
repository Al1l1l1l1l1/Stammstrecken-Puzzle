# Unabhängige Abschlussprüfung – WP-036

**Ergebnis: PASS für die lokale fachliche und technische Prüfung der eingefrorenen DRAFT-Lieferung.** Keine offenen Blocker oder High-Befunde in diesem Prüfumfang. Dies ist keine kreative Ownerannahme, Produkt-, Import-, Rechts- oder Releasefreigabe. Die vollständige reguläre Unity-CI ist bei Erstellung dieses Berichts noch nicht abgeschlossen; damit wird kein vollständiger DoD-/AK09-Abschluss behauptet.

## Rolle und geprüfter Stand

Prüfdatum: 17.10.2026. Ausführende Rolle: gesonderter Agent `/root/independent_qc`, unabhängig von beiden Leveltext-Autoren und vom Autor der UI-/Kampagnentexte, des Schemas und der Produktionsprüfhilfen. Diese Rolle hat keine Produktionsquelle geändert und keinen Commit, Push, PR-Kommentar oder Merge ausgeführt. Vorläufige Befunde wurden an die Koordination gemeldet; die Koordination hat die Korrekturen selbst vorgenommen. Dieser Bericht und seine Prüfbelege wurden erst nach dem ausdrücklich mitgeteilten Freeze geschrieben. Es wird eine unabhängige kombinierte QC-Rolle nachgewiesen, kein menschliches, rechtliches oder gesondertes UI-/Gerätereview simuliert.

Die AGENTS-Lesereihenfolge, der vollständige Auftrag, WP-036, Definition of Done, Übergaberegeln und die relevanten Konzepte 03/05/06/07/08/11/12/13/14 sowie Lokalisierungs-/Katalogverträge und ADR-011/026/030 wurden gelesen.

| Bindung | Tatsächlicher Wert |
|---|---|
| Produktions-Freeze | `af6eb73eebc73bb3662569b2f5f610734e68b6f5` |
| Branch | `codex/wp-036-vollstaendige-textbibliothek` |
| Basis | `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |
| Gemeinsamer WP-/Manifestanker | `213065f7f33be634073e5bcef85db4f848d5b99f` |
| Gepinnte WP-026-Fachquelle | `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d` |
| de-DE.json SHA-256 | `b45db9d6d8216de7a185b26da1edf77f5ad71b1c921ebcfbc49f9726834557be` |
| Authoringschema SHA-256 | `3553f1e5cadf1aef134886b4f766c440bf95b9b46d7a8de73fc08d23ad811514` |
| source-bindings.json SHA-256 | `2fc3d3d9a2336941261456f88f9d5d98460d9cd0092efbbefadfc7e411ea75ed` |
| validate.py SHA-256 | `38c60d1e8afdf3c0a3f72d3ba0f7e113d396d6101e2f76224d653160943e2b91` |
| render.py SHA-256 | `ae7fc4065a9557d0cf4d7c46d7422aa4e34866f28104168607d190b05ad9d1ea` |
| Unverändertes Manifest SHA-256 | `f04c0ca2b57319aacb2ea40632b862853fce5284b795114612a85dc181a1ce2e` |

Die genannten Produktionsdateien stimmen bytegenau mit dem Gitbaum dieses Freeze überein. Der Anker hat genau die genannte Basis als Elterncommit und fügt WP und Manifest gemeinsam hinzu. Der Produktionscommit hat ausschließlich den eigenen Anker als Elterncommit; kein fremder Arbeitszweig wurde integriert. Der Freeze-Diff enthält 21 ausschließlich erlaubte Dateien. Solver, Level, Proofs, Assets, Kataloge, Architekturverträge, fremde WPs und bestehende Validatoren bleiben unverändert. Der Workflowdiff ändert nur die eigene Scopeauswahl und ergänzt gepinnten Quellfetch, Textcheck und Lesefassungscheck; bestehende Schritte bleiben erhalten.

Der unabhängige kanonische Scope-Lauf erfolgte mit `worktreeDirty=false`. Nach diesem Lauf ergänzte die Koordination ausschließlich eigene Provenienz-/CI-Belege. Eine zusätzliche spätere Clean-Tree-Assertion bemerkte zunächst die neue untracked `repository-provenance.json`; sie war kein Produkt- oder Validatorfehler. Die anschließende Prüfung bestätigte unveränderte getrackte Produktionsdateien und Manifestbytes. Die hier ergänzten QC-Belege gehören ebenfalls zum erlaubten Reviewpfad und sind kein neuer Produktions-Freeze.

## Tatsächlich ausgeführte technische Prüfungen

Die vier kanonischen Befehle liefen unabhängig in WSL Ubuntu mit vorhandener CPython **3.11.13**, Node **22.20.0** und den gelockten Architekturabhängigkeiten, darunter jsonschema 4.26.0 und referencing 0.37.0. Es wurde nichts installiert und kein bestehendes Gate geändert. Aus dem Repositoryroot, mit Node22 im PATH und TMPDIR im ignorierten `.utmp/wp036`:

```sh
.utmp/wp029-venv/bin/python -B tools/architecture-validation/validate.py --self-test
.utmp/wp029-venv/bin/python -B tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-036.production.scope.json --self-test
.utmp/wp029-venv/bin/python -B tools/text-library/validate.py --self-test
.utmp/wp029-venv/bin/python -B tools/text-library/render.py --check
git diff --check d90e798dd9b26d3f152f3b0c2d1d52cae641eb16 af6eb73eebc73bb3662569b2f5f610734e68b6f5
```

| Prüfung | Beobachtetes Ergebnis | Originalausgabe |
|---|---|---|
| Architecture-only mit unveränderten Self-/Negativtests | Exit 0, RESULT PASS, 17 lokale Gruppen | [qc-architecture.txt](qc-architecture.txt) |
| Eigener historisch verankerter Production-Scope mit Selftests | Exit 0, RESULT PASS, 18 lokale Gruppen, korrekte Basis/HEAD, sauberer Baum | [qc-scope.txt](qc-scope.txt) |
| Gesamter Textchecker mit zwölf Mutationen | Exit 0, PASS: 1.408 Einträge, 960/960, 240 INTERNAL, 480 fremde Ziele weiterhin offen | [qc-text.txt](qc-text.txt) |
| Abgeleitete Lesefassung | Exit 0, exakte kanonische Textprojektion | [qc-render.txt](qc-render.txt) |
| Git-Diff-/Anker-/Dateibindung | Exit 0; gemeinsame Erstadds, unveränderte Manifestbytes, erlaubte 21 Dateien und bytegleiche Produktionsquellen bestätigt | Bindungstabelle und Scopeausgabe oben |

Die TXT-Dateien bewahren den tatsächlich zurückgegebenen stdout ohne nachträglich hinzugefügte Erfolgsaussagen. Text- und Renderausgabe kamen aus demselben Shellaufruf und sind hier nur an ihrer Befehlsgrenze getrennt. Der bereits dokumentierte frühere Windows-V03-Pfadbefund wird nicht als kanonischer Linuxfehler übernommen; die unabhängigen tatsächlichen Linuxläufe oben bestehen ihn unverändert.

Zusätzlich wurden zehn eigene gezielte Negativproben ausschließlich in Memory ausgeführt: doppelte JSON-Eigenschaft, unbekanntes Wurzelfeld, unbekanntes Eintragsfeld, versuchte PRODUCT_APPROVED-Promotion, falsche Locale, gesperrter Eintrag ohne Abhängigkeit, falscher Reviewhash, veränderte öffentliche Counts, vertauschte Level-Key-Reihenfolge und unerlaubter Platzhalter in shortText. **Alle zehn wurden abgewiesen.** Die gespeicherten technischen Reportfelder und drei Eingabehashes wurden neu abgeglichen. Pseudolocale, Wiederholungsbericht und Lesefassung wurden in Memory vollständig reproduziert. Alle 1.168 Pseudolocale-Einträge gehören zu PLAYER; Platzhalter bleiben in identischer Reihenfolge erhalten, INTERNAL fehlt. Die interne Zusatzangabe „19 Kurven“ in S1-02-04-12 wurde am tatsächlichen gebundenen Lösungspfad nachgezählt.

Die tatsächlich ausgeführte Python-Here-String-Nutzlast ist zur Reproduktion in [qc-probes-command.txt](qc-probes-command.txt) erhalten; sie lief mit `C:\STP\.venv-stp-architecture\Scripts\python.exe -B -` (CPython 3.11.15, Windows, zusätzlich zu den kanonischen Linuxläufen). Dieselbe Nutzlast kann aus dem Repositoryroot mit `python -B Content/Reviews/text-library/WP-036/qc-probes-command.txt` gelesen ausgeführt werden. Ihr tatsächliches strukturiertes Ergebnis steht in [qc-probes.json](qc-probes.json). Sie ersetzt keine Spieltests oder Solverabnahme.

## Redaktionelle Gesamtprüfung und Quellenabgleich

Gelesen wurden sämtliche 720 Level-Spielertexte, alle 240 internen Einstiegstexte, die 448 zusätzlichen UI-/Kampagnen-/Variantentexte und alle 82 Kurzfassungen. Die finalen Status-/Ergebniszeilen und Kurzfassungen wurden nach dem Freeze erneut gelesen. Alle 240 internen Einstiege wurden einzeln den tatsächlichen gepinnten Rastermaßen, Endpunkten und Zeilen-/Spaltenzahlen gegenübergestellt. Die vollständige technische Autor-/Routenreview-/Levelbytebindung wurde zusätzlich ausgeführt. Die manuellen Vergleiche fanden keinen falschen Zahlenwert oder unzulässigen einfachen Schluss. Aussagen über spätere Restkoordination wurden anhand der zugehörigen Quellen geprüft; eine neue vollständige Solveranalyse aller Paraphrasen wurde nicht durchgeführt oder behauptet.

Die Bibliothek folgt der bestätigten Welt: anfängliche Planbehauptung, dauerhafte Baumaßnahme, entkoppelte Halte und Wagen, verschachtelte Umleitungen, dann Ersatzhalt/Busprognose und die Rückkehr auf Schienen. Die fünf Abschnitte und zwanzig Routennamen bleiben erkennbar. Signal, Stellwerk, Bahnsteigwechsel, Anschluss, Wagenreihung und SEV tragen die deutsche Bahn-Anspielung; es werden keine neuen Figuren oder Rasterregeln eingeführt. Im gelesenen Bestand fand sich keine Herkunftsbehauptung als offizieller Verkehrsservice und keine Pointe gegen reale Betroffene oder Mitarbeitende. Das ist eine redaktionelle Inhaltsprüfung, keine externe Plagiatsrecherche oder Rechtsprüfung.

Die neun verlangten Ereignisanlässe sind vollständig abgedeckt. Elf Familien sind durch die getrennten Sachverhalte Punktgutschrift/kosmetischer Fortschritt und Videoangebot/Rückkehr nachvollziehbar. Die Varianten bewahren jeweils denselben Zustand. Erfolg, Reward, Sterne und Besitz werden nur unter den beschriebenen bestätigten Vorbedingungen formuliert. Gesperrte Hint-/Daily-/Endless-/Store-/Kosmetikentwürfe entscheiden keine offenen Regeln. Feste Beschriftungen und Sternnamen bleiben erhalten. Der Finale-Titel „Tatsächlich pünktlich“ ist im Kontext ein bestehender Titel; er vergibt keine Sterne.

### Begründete unabhängige Stichprobe

| Beispiel | Urteil und tatsächlicher Abgleich |
|---|---|
| S1-01-01-01 | Die bereits berücksichtigte, aber ungeklärte Abweichung eröffnet verständlich den Plan-/Wirklichkeitskontrast. Vollzeile 0 und B an N/0 sind reale Eingangsanker; sie bleiben ausschließlich intern. |
| S1-01-02-08 | Gegenseitiger Verweis zwischen Durchsage und Tafel ist konkrete Pendlerfriktion. Der Ergebnistext löst dieses Motiv durch die tatsächliche Verbindung auf, ohne neue Informationsmechanik. |
| S1-02-02-05 | Die fehlende Kapazitätsreserve wird verwaltungssprachlich erklärt. Intern wird korrekt nur der Nordport aus A an N/4 festgelegt; eine ganze Gleisform wird nicht daraus erfunden. |
| S1-02-04-12 | Der Kartenring schließt den Bauabschnitt ab. Die unteren Vollzeilen 3/4 stimmen mit dem 6×5-Input überein. Die 19 Kurven sind echte interne Authoringinformation und gelangen nicht in den Playertext. |
| S1-03-04-08 | Der Verweis vom anderen Bahnsteig zur ersten Anzeige macht die Auskunftsschleife räumlich verständlich. Ergebnis und Kurzformen schließen denselben Vorgang; keine neue Umstiegspflicht entsteht. |
| S1-04-01-12 | Die Brücke gehört zum bestätigten Karten-Ziel. Im Puzzle wird keine Brückenfunktion behauptet; intern verbraucht A an E/0 den Einsercount der rechten Spalte. |
| S1-04-02-11 | Die finale Kurzform „vorerst unverändert“ erhält die zeitliche Einschränkung der Langfassung. Der geprüfte interne Nordport-/Einserzeilen-Schluss ist nachvollziehbar. |
| S1-04-03-12 | Anzeige und fehlende Verbindung zwischen Bahnsteigen liefern eine konkrete Lage. A an E/0 erschöpft Spalte 7 im 8×8-Input. Kein Spielertext verrät diese Position. |
| S1-05-02-03 bis 05 | Die Einstiegstexte trennen lokalen Einser-Portanker und weitere Restkoordination entsprechend Bible §8.1. Die Busabfahrt bleibt Satire, ohne Wartezeit oder neue Mechanik für den Spieler. |
| S1-05-03-11 | Fahrplanzeile und fehlende Beschilderung zwischen Ersatzhalt und Bahnsteig tragen den Anschlusswitz. Das Ergebnis hat einen konkreten räumlichen Bezug; keine reale Anschlussgarantie. |
| S1-05-04-12 und Seasonfinale | Die letzte Schienenverbindung ersetzt den Ersatzweg; der Zug kommt auf der eigenen Strecke an. A an E/0 und Count 1 der oberen Zeile erzwingen intern (9,0) und die Fortsetzung nach (9,1). Der Abschluss würdigt Verbindung und 240 Meldungen, ohne unkalibrierte Zeitwertung. |

## Befunde und verbleibende Grenzen

**Vorläufiger Provenienzbefund geschlossen:** Im frühen Checker fehlte die Prüfung der zusätzlichen authorReview-Wurzelbindung. Die Koordination ergänzte vor dem Freeze Pfad-/Bytehashprüfung, geordnete 240 Autor-IDs, vollständige Autor-/Routenreview-Zitatübereinstimmung und zwei gezielte Negativproben mit erwarteten Diagnostiken. Der finale unabhängige Gesamtlauf besteht diese Ergänzung. Die unklare Provisoriums-Kreisformulierung und die bestimmtere Reisezeit-Kurzform wurden ebenfalls vor dem Freeze korrigiert und nachgelesen.

**Nichtblockierender Redaktionshinweis:** Vor allem die späten Routen verwenden weiterhin viele abstrakte Wörter wie Stand, Zusammenhang und Bewertung sowie ähnlichen Zweitsatzrhythmus. Das ist zum Teil die beabsichtigte Behördenstimme, kann aber über längere Spielstrecken ermüden. Die konkrete Nacharbeit an Anzeigen, Wegweisern, Busankunft und Bahnsteigen verbessert dies sichtbar. Im finalen Bericht wiederholt sich „Die Verbindung steht.“ noch in drei Levelergebnissen und einer Fahrteinleitung; die verschiedenen Fortsetzungen rechtfertigen diese knappe funktionale Wiederholung. Keine vollständigen Level-Status- oder Ergebniszeilen sind identisch. Die verbleibende Humorintensität und Rhythmusfrage gehört ausdrücklich zur kreativen Ownerbewertung; diese QC erklärt sie nicht automatisch für geschmacklich angenommen.

`render.py` ist eine übersichtliche, deterministische Lesefassungsprojektion. Es setzt die separat validierte feste Reihenfolge der ersten 960 Einträge voraus und ist kein zusätzlicher Runtimeimporter oder Schemaersatz. Die zwölf Checker-Mutationen und zehn unabhängigen Proben beweisen jeweils ihre konkreten Ablehnungen; sie beweisen keine allgemeine Fehlerfreiheit. Zeichenlimits und Pseudolocale sind kein Nachweis für reale Umbrüche, expandierte Variablen, dynamische Schrift oder Screenreader.

### AK- und CI-Abgrenzung

AK01–AK08 sind im Umfang einer vollständigen, prüfbaren DRAFT-Textproduktion nachvollziehbar erfüllt. AK09 ist für eigenen Anker/Scope und lokale unabhängige Abschlussprüfung erfüllt; der vollständige reguläre CI-/PR-Abschluss bleibt nach seinem tatsächlichen Ergebnis nachzuführen. AK10 besitzt die vollständigen Quellen und Übergabedokumente; endgültiger Liefer-/PR-/CI-Nachtrag ist Sache des folgenden dokumentierten Evidenzcommits.

Die von der Koordination bereitgestellte [Architecture-CI-Originalausgabe](ci-architecture-af6eb73.json) wurde gelesen und ihre tatsächlichen Ergebniszeilen kontrolliert: Run `39058816091`, Job `117472311700`, HEAD `af6eb73…`, conclusion `success`; Architecture 17, eigener Scope 18, JCS 12/32/2/2, Text 1408/960/12 und Lesefassung bestehen. Dies ist die Auswertung eines erhaltenen Connector-Logs, keine von dieser Rolle separat abgefragte Live-GitHub-Sitzung. Ein darin enthaltener Befehlstext allein wurde nicht als Ergebnis gewertet. Die vollständige Unity-CI war beim Schreiben dieses Berichts noch ausstehend und wird hier ausdrücklich nicht als PASS ausgegeben. [Draft-PR #31](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/31) wurde von der Koordination erstellt; diese Rolle hat ihn nicht veröffentlicht oder integriert.

Die 480 Karten-/Zugreferenzen, der gesamte WP-026-Abschluss, menschliche Spieltests, finale Zeitwerte, Hint-/Daily-/Endlessblocker, Kosmetik-/Storeentscheidungen, Runtime-/Doppelimport, Addressables, Geräte-/SDK-/Privacy-/Signing-/Store-/Rechts- und Releasegates bleiben offen. Die aktuelle lokale DRAFT-QC gibt weder diese Fremdgates noch die kreative/produktseitige Annahme frei. Nächster Schritt: tatsächliche vollständige reguläre CI nachtragen, anschließend die konkreten Texte und Nachweise durch den Owner bewerten lassen. Kein automatischer Merge.
