# WP-036 – Ausführungsbrief für die gezielte kreative Nacharbeit

**Auftragsgrundlage:** [Geschäftsführerentscheidung WP-036-CREATIVE-01](CREATIVE_DECISION.md), 17.10.2026. **Zugehörigkeit:** bestehendes `WP-036`, kein neues Work Package. **Status:** vor Ausführung verbindlich verankert; in diesem Entscheidungsschritt noch nicht begonnen.

## 1. Ziel

Die begrenzten Verwaltungsabstraktionsserien wieder als konkrete deutsche Bahn-Alltagssatire lesbar machen. Bestehende Störungs-/Routenprämissen bleiben, ebenso der trockene dienstliche Duktus. Die selbst gebaute befahrbare A–B-Verbindung soll in den betroffenen Ergebnissen als praktischer Gegenbeweis und eigene Leistung erfahrbar werden. Keine zweite Textbibliothek erstellen.

## 2. Voraussetzungen und Standprüfung

Zuerst `AGENTS.md` und die dort verlangte Lesereihenfolge einhalten. Das vollständige bestehende WP-036, Projektsteuerungsregeln, `CREATIVE_DECISION.md` und [CREATIVE_SCOPE.json](CREATIVE_SCOPE.json) lesen. Relevante Konzepte sind insbesondere 05/11/12/14; Produkt-/Werkzeug-/Rewardverträge aus 03/06/07/08/13 bleiben unverändert. Die in der Entscheidung persistent erklärte Chatabsicht ist mit anzuwenden: sofortige Bahnfahrer-Wiedererkennung, selbstironisch statt überernst, sorgfältig statt botartig, eigener Erfolg statt bloßer Textverwaltung.

Ausgangspunkt ist der Textbestand des Liefer-HEAD `f217ccbd615f33ca323fec2ecd29f4681fdbc721`, Quelle `Content/Localization/source/de-DE.json`, SHA-256 `b45db9d6d8216de7a185b26da1edf77f5ad71b1c921ebcfbc49f9726834557be`. Vor Ausführung aktuellen Remote-HEAD und Quellhash prüfen. Ein reiner Entscheidungs-/Dokumentationscommit nach f217 ist zulässig; ein abweichender Textstand erfordert zunächst den Abgleich mit neueren Entscheidungen. Keine parallele Lieferung überschreiben.

Der bestehende gemeinsame WP-/Manifestanker `213065f7f33be634073e5bcef85db4f848d5b99f` auf Basis `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` bleibt bestehen. Das Manifest wird nicht geändert oder neu verankert. Dieser Brief ist die vor Ausführung dokumentierte redaktionelle Korrektur innerhalb des vorhandenen Textblocks.

## 3. Abschließender Scope

**110 bereits vorhandene Einträge, ausschließlich 109 `text`- und 11 vorhandene `shortText`-Felder.** Die exakte Liste pro Schlüssel/Eigenschaft samt Originalfassung und gewünschter Wirkung steht unter `correction_scope` in `CREATIVE_SCOPE.json`. Sie ist die abschließende Änderungsgrenze. Die Bereichsschreibweise unten ist nur die lesbare Gruppierung; sie erlaubt keine zusätzlichen Titel-, Nachbar- oder Metadatenänderungen.

| Korrekturfenster | Erlaubte Copy-Felder | Gegenstand / gewünschte Wirkung |
|---|---|---|
| Stammstrecke R04 | `level.s1.01.04.02/04/06/08.status.text`; `level.s1.01.04.03.result.text` | Stand-/Plan-/Teilplanfolgen an vorhandene Fahrplan-, Aushang-, Anzeige- oder Abfahrtslage binden. „Redaktioneller Spielraum“ nicht als Produktionsvokabel im Resultat belassen; in-world würdigen. |
| Kurzname R03 | `campaign.s1.route.01.03.name.shortText` | Das Paradox „planmäßig ohne Sie“ in der kurzen Fassung erhalten; Langname unverändert. |
| Bauabschnitt R06/06–08 | `level.s1.02.02.06/07/08.{status,result}.text` | Den eingleisigen Richtungsengpass und seine nachvollziehbare Fahrtfolge konkretisieren. |
| Bauabschnitt R07/01–12 | `level.s1.02.03.01–12.{status,result}.text` | Das schweigende Stellwerk als konkreten Signal-/Fahrt-/Wartekonflikt entwickeln, nicht als zwölf Varianten von Anfrage/Antwort/Statusverwaltung. Titel und Kampagnenklammer erhalten. |
| Außenast R09 | `level.s1.03.01.01/03/05.result.text` | Nutzen der gelösten Verbindung für Halt, Bahnsteig oder Weg konkret machen, ohne Anschlussgarantie. |
| Außenast R11 | `level.s1.03.03.03/05/08.{status,result}.text`; `.06.status.text`; `.11.status.text` | Zugteile, Depot und Zusammenführung statt zusammenhängender Aussagen/Bearbeitungsstände zum Gegenstand machen. |
| Umleitung R14/02–09 | `level.s1.04.02.02–09.{status,result}.text`; `.07.result.shortText` | Zeit-/Umwegidee behalten; Anzeige, Wegweisung, Umstieg oder tatsächlich befahrbarer längerer Weg müssen die Serie tragen. Keine Dauer erfinden. |
| Umleitung R15 | `level.s1.04.03.03/04/06/07/08/10/11.{status,result}.text`; `.07/.11.status.shortText` | Selbstaktualisierende Auskunft als konkrete Anzeige-/Bahnsteig-/Wegeführungslage zeigen. Die starke Schlussmeldung 12 und alle Titel bleiben. |
| Umleitung R16/01–08 | `level.s1.04.04.01–08.{status,result}.text`; `.01/.04/.06/.07/.08.status.shortText` | Alternative zur Alternative an Umstieg, Beschilderung, Bahnsteig oder Zugweg erden. Keine zusätzliche Umstiegsregel. |
| SEV R18 | `level.s1.05.02.07/09/10.{status,result}.text`; `.07.status.shortText` | Bus-/Auskunfts-/Standortproblem am Ersatzhalt erfahrbar machen. Meldung 08 bleibt vollständig erhalten. |
| SEV R20/01–05 | `level.s1.05.04.01–05.{status,result}.text`; `.02.status.shortText` | Früh behauptete Stabilität an konkreter Fahrt/Verbindung überprüfen; referenzloses „darunter“ im Kurztext beseitigen. Meldungen 06–12 und Seasonfinale bewahren. |
| Zugfahrtvariante | `variant.ride-ended.04.text` | Tatsächlich befahrene eigene Strecke statt abstrakter „Erfahrung“ als Abschluss würdigen. |

Die Kopie des ursprünglichen Routenhumors wird nicht durch bloßes Synonymtauschen verbessert. Eine Begriffsrekursion soll an einer nachvollziehbaren Bahn-/Fahrgastlage brechen. Das muss innerhalb des jeweiligen Korrekturfensters im Zusammenhang lesbar sein; es ist keine Pflicht zu einem bestimmten Gegenstand oder derselben Satzstruktur in jedem einzelnen Text.

## 4. Erlaubte Artefakte und ausdrücklich ausgeschlossene Änderungen

Erlaubt sind die oben genannten Copy-Felder in der kanonischen Quelle und die durch die neue Textrevision tatsächlich erforderliche Aktualisierung der **eigenen** abgeleiteten Lesefassung, Text-/Pseudolocale-/Wiederholungs-/Prüfberichte und redaktionellen Übergabe unter `Content/Reviews/text-library/WP-036/`. Neue Korrektur-/Abnahmenachweise erhalten eine eindeutige Standbindung; ältere unabhängige `QC.md`-/`CI_QC.md`-Nachweise dürfen nicht umgeschrieben oder auf den neuen Quellhash übertragen werden. WP-036, CURRENT_STATE und WORK_QUEUE erhalten ausschließlich die eigene Statusnachführung. Das vorhandene unveränderliche Manifest bestimmt weiterhin die erlaubten Pfade; dieser Brief erweitert es nicht.

**Nicht erlaubt:** Änderung eines anderen `de-DE.json`-Feldes, eines nicht gelisteten Textes oder des Schemas; Änderung von Keys, Reihenfolge, Beschreibungen, Zeichenlimits, Platzhalterverträgen, Status, Sichtbarkeit, `availability`, `blockedBy` oder Kurztextlimits. Alle 1.298 voll angenommenen Einträge und alle sonstigen Eigenschaften der 110 Korrektureinträge sind zu bewahren. Alle 240 Titel, 240 internen Einstiege, 70 produktabhängig gesperrten Entwürfe, Abschnitts-/Routen-/Season-Langtexte und 54 übrigen Ereignisvarianten bleiben unverändert.

Keine fremden WPs integrieren, keine Level/Lösungen/Proofs/Karten-/Zugreferenzen/Assets/Kataloge ändern. Keine neuen Produktregeln, Zeitwerte, Sterne, Belohnungen, Figuren, Routen, Ereignisse, Preise oder Freischaltungen. Kein Wortverbot, keine Humorquote und keine neue Längenregel. Keine Produktionswerkzeuge oder Workflows ändern, um Textprüfung zu umgehen. Kein Merge nach main, kein Runtimeimport und keine automatische PRODUCT_APPROVED-Promotion.

## 5. Akzeptanzkriterien der Nacharbeit

| ID | Erforderlicher Nachweis |
|---|---|
| K01 – Scope | Ein eindeutiger Before/After-Diff weist ausschließlich die 120 freigegebenen Copy-Felder nach. Nicht freigegebene Inhalte/Metadaten bleiben identisch; alle Keys/Reihenfolge/Sperren sind erhalten. |
| K02 – Konkrete Wirkung | Die korrigierten Routenfenster sind im Zusammenhang wieder als Bahn-/Fahrgastlage verständlich. Verwaltungsduktus und vorhandener Witz bleiben, aber die Serie erzählt nicht mehr hauptsächlich ihre eigene Textverwaltung. Die bestehenden guten Nachbartexte dienen als Vergleich, nicht als Kopiervorlage. |
| K03 – Entwicklung | Die jeweilige Route behält ihre Prämisse und entwickelt die Situation weiter; keine Reihe aus nur vertauschten Behördennomen. R07 bleibt Stellwerk, R14 Zeit-/Umwegfrust, R15 Auskunftsänderung, R16 Ersatzroutenrekursion, R18 Bus-/Ersatzhaltlage, R20 endgültige Stabilisierung. |
| K04 – Eigene Leistung | Korrigierte Ergebnisse würdigen die tatsächlich befahrbare Spielerstrecke. Keine garantierte reale Zeit, Pünktlichkeit, externe Anschlusslage, Sterne, Boni oder Funktion behaupten. |
| K05 – Kurzformen | Alle elf gelisteten Kurzfassungen erhalten den Sinn und gegebenenfalls die Pointe ihrer neuen Langfassung. Kein referenzloser Bezug; keine überbestimmte Zusage durch Kürzung. Der Langname von R03 bleibt unverändert. |
| K06 – Schutz | Interne Einstiege bleiben intern; keine Lösung, Koordinaten oder autorenseitigen Prämissen in die sichtbare Copy übernehmen. Hilfsmarkierungen bleiben vollwertig; produktabhängige Entwürfe bleiben gesperrt. |
| K07 – Neue Evidenz | Frischer Quellhash, neue abgeleitete Lesefassung und die nach bestehendem WP-036 betroffenen technischen/CI-/unabhängigen Nachweise sind an den tatsächlich geänderten Freeze gebunden. Alte PASS-Ergebnisse werden nicht als neue ausgegeben. |
| K08 – Kreative Restannahme | Eine kreative Nachlektüre der Korrekturfenster samt erhaltenen Nachbarn und Kurzfassungen entscheidet über den verbleibenden Korrekturbestand. Der bereits angenommene übrige Bestand wird nicht erneut pauschal zur Disposition gestellt. |

## 6. Prüfungen und Definition of Done

Dieser Brief verlangt für eine **spätere tatsächliche Textkorrektur** die im bestehenden WP-036 vorgesehenen neuen Nachweise: vollständiger eigener Textchecker mit seinen Negativproben, exakte Reproduktion der Lesefassung/abgeleiteten Berichte, Quell-/Scope-/Diffbindung und die erforderlichen regulären Architecture-/Unity-CI-Ergebnisse am neuen Freeze. Die technische unabhängige Prüfung erfolgt gemäß bestehendem Projektverfahren; sie ersetzt die kreative Restannahme nicht. Es werden keine Gates abgesenkt und keine neue breit angelegte Solver-/Level-QC eröffnet.

Der jetzige Entscheidungsschritt startet diese Läufe für den unveränderten Textbestand **nicht erneut**. Neue Nachweise entstehen erst aus der tatsächlichen Korrekturlieferung. Der erforderliche Scope-Diff kann strukturiert alle nicht freigegebenen Eigenschaften gegenüber f217 vergleichen; er ist keine neue Puzzlelogik.

**Risikoklasse:** mittel, wie der bestehende Textblock: sichtbare Sprache mit Spoiler-/Leistungs-/Produktzusagerisiko, aber keine Architektur- oder Runtimeänderung. Die Nacharbeit gilt erst als abgeschlossen, wenn K01–K08 nachweisbar erfüllt sind, die bestehende WP-036-DoD erhalten ist und Status/Übergabe den neuen Freeze, offene Gates und nächsten Schritt korrekt nennen. Eine vorbehaltene ausdrückliche Ownerannahme wird weiterhin nicht erfunden.

## 7. Ausführungsfolge

1. Veröffentlichung des jetzigen Entscheidungs-/Auftragscommits am bestehenden Draft-PR #31 sicherstellen, Remote und Quellhash abgleichen.
2. Ausschließlich die gelisteten Copy-Felder redaktionell nacharbeiten, im jeweiligen Routenfluss statt isoliert aus einer Wortliste.
3. Betroffene eigene abgeleitete Dokumentation erzeugen; den Before/After-Diff und neuen Quellhash sichern.
4. Die vom bestehenden WP-036 verlangten neuen Prüfungen am geänderten Freeze durchführen und dokumentieren; anschließend die unabhängige Prüfung nach Projektverfahren und die kreative Restannahme erhalten.
5. CURRENT_STATE, WORK_QUEUE und WP-036 nach tatsächlich erreichtem Stand nachführen. Kein automatischer Main-Merge und keine Produkt-/Import-/Releasefreigabe.

Die Arbeit ist damit ohne Chat ausführbar. Dieser Brief beauftragt keine andere Warteschlangenposition und entscheidet keine offenen Hint-, Daily-, Generator-, Katalog-, Rechts- oder Assetfragen mit.
