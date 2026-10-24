# WP-036 – unabhängige kreative Restprüfung der Copy-Korrektur

**Berichtssprache:** Deutsch
**Prüfrolle:** Unabhängige kreative Restprüfung; keine Autorrolle, keine Beteiligung an den sieben Korrekturvorschlägen und keine Ownerrolle.
**Berichts-ID:** `WP-036-INDEPENDENT-CREATIVE-QC-01`
**Verdikt für den bestehenden Korrekturbestand:** **FAIL** – ein konkreter `HIGH` verletzt K05 (Kurzform trägt nicht dieselbe Statusaussage) und verhindert deshalb die kreative Restannahme in diesem Stand.

> Dieser Bericht beurteilt ausschließlich die tatsächlich ausgeführten 120 Copy-Änderungen und ihre erhaltenen Routenkontexte. Er ist **keine** ausdrückliche Owner-, Produkt-, Import-, Rechts- oder Releaseannahme. Die delegierte eigene kreative Restentscheidung liegt beim Parent/Koordinator.

## 1. Standbindung und Unabhängigkeit

| Bindung | Verifizierter Wert |
|---|---|
| unabhängiger Worktree / geprüfter Freeze-HEAD | `c7683ea445c65becd78cfd1e34ccb8ba587fd024` |
| Ausgangs-/Entscheidungsanker | `6103d53f9a01b199db7eaf9b3e5fd0c76f4a63a1` |
| alte Quelle, SHA-256 | `b45db9d6d8216de7a185b26da1edf77f5ad71b1c921ebcfbc49f9726834557be` |
| einzige geprüfte neue Textquelle | `Content/Localization/source/de-DE.json` |
| neue Quelle, SHA-256 | `2cd576dc52a7e3379dfe8a925a7e8c4201c1e24e988913b247d39f1f81b605fb` |
| Scope-Entscheidung | `WP-036-CREATIVE-01`; `CREATIVE_SCOPE.json` unverändert |
| tatsächlicher struktureller Abgleich | 1.408 Keys in gleicher Reihenfolge; exakt 110 Einträge / 120 freigegebene Felder geändert: 109 `text`, 11 `shortText`; kein zusätzliches oder fehlendes freigegebenes Feld |
| geschützte Grundlage | 1.298 angenommene Einträge sowie alle nicht freigegebenen Properties blieben außerhalb dieser kreativen Restprüfung unverändert |

Der angegebene neue Sourcehash wurde direkt aus der kanonischen Quelle am geprüften Freeze ermittelt und stimmt mit `COPY_CHANGE_AUDIT.json` überein. Der Vergleich gegen `6103…` bestätigt die exakte **Menge** der 120 Scope-Felder; er behauptet keine neue technische, Build-, CI- oder Solverprüfung. Es wurden keine Tests, Builds oder CI-Läufe doppelt ausgeführt und keine Textquelle korrigiert.

## 2. Pflichtgrundlagen und tatsächlicher Lektüreumfang

Vor der Prüfung wurden `AGENTS.md`, Konzept 15, 00, 09, `CURRENT_STATE.md` und WP-036 in der verlangten Reihenfolge gelesen. Danach wurden die maßgebliche `CREATIVE_DECISION.md`, `CORRECTION_BRIEF.md`, `CREATIVE_SCOPE.json`, `COPY_CHANGE_AUDIT.json`, `COPY_CHANGE_REVIEW.md`, die Lesefassung sowie Konzepte 11/12/14 und die erforderlichen Konzepte 03/05/06/07/08/13 herangezogen.

**Vollständige Lektüre `ALL120`:** Alle 120 tatsächlichen Alt-/Neufassungen aus Audit und Quelle wurden einzeln gelesen, nicht aus einer Wortliste extrapoliert. Für jeden Level-Schlüssel wurde außerdem die gesamte jeweilige 12-Meldungen-Folge mit Titel, aktuellem Status, Ergebnis und vorhandener Kurzform direkt aus der kanonischen Quelle gelesen. Das betrifft die zehn Levelrouten S1-01-04, S1-02-02, S1-02-03, S1-03-01, S1-03-03, S1-04-02, S1-04-03, S1-04-04, S1-05-02 und S1-05-04; zusätzlich wurden die S1-01-03-Kartenbenennung und die komplette `ride-ended`-Variantenfamilie gelesen. `READING_EDITION.md` wurde als abgeleitete Querlesefassung gegengeprüft.

Die unveränderten 1.298 Einträge wurden **nicht** neu zur Vollannahme aufgerollt. Ihre durch `WP-036-CREATIVE-01` angenommene Grundlage wird bewahrt; nur erhaltene Nachbarn dienten als Kontext für die Korrekturfenster.

## 3. Maßstab

Geprüft wurde gegen die konkrete Nacharbeitsursache, nicht gegen isolierten Synonymgeschmack:

1. **Bahnfahreralltag:** Status nennt eine anschauliche Lage und/oder spürbare Fahrgastfolge – etwa Signal, Bahnsteig, Wegweiser, Zugteil, Ersatzhalt oder Bus – statt nur die Verwaltung ihrer eigenen Aussage.
2. **Eigene Leistung:** Ergebnisse würdigen die tatsächlich selbst gebaute, nutzbare A–B-Strecke und die Zugfahrt; sie versprechen weder reale Reise, Anschluss, Zeit, Sterne noch Bonus.
3. **Routenbogen:** Die bestehende Prämisse muss sich von Meldung 1 bis 12 entwickeln, ohne eine neue Puzzle-, Produkt- oder Verkehrsregel zu erfinden.
4. **Kurzform:** Sie muss denselben Status oder dasselbe Ergebnis in komprimierter Form tragen; sie darf den Abschluss nicht in einen ungelösten Status hineinziehen.
5. **Ton:** glasklare Bahn-Alltagssatire zwischen Lachen und Heulen; trocken, präzise, plump-staubig-behördenhaft und selbstironisch, nicht austauschbar oder überernst. Die hochwertige, konkrete Ausführung bleibt vor Cartoon-/Comic-Rahmung maßgeblich.
6. **Schutzgrenzen:** Keine INTERNAL-Lösungshinweise, keine neue Rätselregel, keine Echtzeit-/Reise-/Anschlussgarantie und kein neues Zeit-/Stern-/Bonusversprechen. Das unveränderte trockene Seasonfinale bleibt erhalten.

## 4. Prüfung der zwölf freigegebenen Fenster

| Freigegebenes Fenster | Felder | Einzelursache aus dem Brief | unabhängiges Urteil |
|---|---:|---|---|
| S1-01 R03 Kartenkurzname | 1 | Das Paradox „planmäßig ohne Sie“ war in der Karte zu schwach. | **PASS.** „Anschluss: planmäßig ohne Sie“ trägt Anschluss, Fahrgastverlust und den trockenen Widerspruch kompakt; der Langname blieb erhalten. |
| S1-01 R04 Fahrplan-/Aushangserie | 5 | Stand-, Plan- und Teilplanrekursion sollte wieder Abfahrt, Aushang, Bahnsteig und Wegweisung betreffen. | **PASS.** Die Situationen sind nun unterschiedlich konkret und erhalten den Verwaltungsduktus. |
| S1-02 R06 eingleisiger Engpass 06–08 | 6 | Gegenrichtung/Gesamtschau war abstrakt statt als Engpass und Fahrtfolge erfahrbar. | **PASS.** Freies Gleis, rotes Signal, wartende Gegenrichtung und befahrbarer Hinweg bilden eine klare, begrenzte Betriebslage. |
| S1-02 R07 Stellwerk-/Signalsituation 01–12 | 24 | Anfrage-, Antwort- und Statusverwaltung verdrängte die wartende Fahrt. | **PASS, mit schwachem Serienhinweis.** Signal, Bahnsteig, Durchsage, Warteschlange und Durchfahrt tragen einen geschlossenen Stellwerksbogen; die Korrektur ist nicht bloß Nomentausch. |
| S1-03 R09 Ergebnisnutzen | 3 | Halt/Bahnsteig/Weg waren als Ergebnis zu abstrakt. | **PASS.** Außenbahnsteig, verlegter Halt und Haltebereich werden durch die Strecke praktisch nutzbar, ohne Anschlussversprechen. |
| S1-03 R11 Zugteil-/Depotbogen | 8 | „Zusammenhang“ und Bearbeitungsstand ersetzten Zugteile, Depot und gemeinsame Weiterfahrt. | **PASS.** Vorderer/hinterer Zugteil, Depot-Abzweig und gemeinsame Strecke entwickeln die Lage anschaulich. |
| S1-04 R14 Zeit-/Umweg-Mittelstrecke | 17 | Mehr Reisezeit/Prognose brauchte Bahnsteig, Anzeige, Umweg und Zugweg statt Textverwaltung. | **PASS.** Die Statusfolge bleibt unbestimmt, aber nie versprechend; die Ergebnisse machen allein den gebauten Weg befahrbar. |
| S1-04 R15 Prüfstands-/Auskunftsschleife | 16 | Selbstaktualisierung sollte an Anzeigen, Wegweisung und Bahnsteige rückgebunden werden. | **FAIL wegen eines HIGH.** Die 15 Langfelder sind konkret und tragfähig; die eine Kurzform bei Meldung 07 nimmt jedoch ein Ergebnis vorweg und widerspricht ihrem Langstatus. |
| S1-04 R16 Alternativenrekursion 01–08 | 21 | Alternative zur Alternative sollte an Umstieg, Schild, Bahnsteig und Zugweg lesbar werden. | **PASS.** Die Rekursion bleibt Pointe, wird aber durch Pfeile, Ersatzschild, gesperrten Bahnsteig und tatsächliche Fahrbarkeit verankert. |
| S1-05 R18 Bus-/Anzeigefenster | 7 | Der Ersatzhalt brauchte einen erfahrbaren Bus-, Anzeige- und Standortkonflikt. | **PASS.** „unterwegs“ ohne beschilderten Halt ist konkrete SEV-Friktion; die Gleisresultate ersetzen keine Echtzeit- oder Anschlusszusage. |
| S1-05 R20 Stabilitätsserie 01–05 | 11 | Stabilitätsbewertung/Bearbeitung brauchte getrennte Bahnsteige, offene Gleise und einen befahrbaren Gegenbeweis. | **PASS.** Die sarkastische Verwaltung bleibt, aber sie kollidiert sichtbar mit der Strecke und wird durch die Spielerleistung überholt. |
| Zugfahrtvariante `ride-ended.04` | 1 | „Erfahrung“ ersetzte den zentralen Leistungsbeweis. | **PASS.** „Fahrt abgeschlossen. Ihre selbst gebaute Strecke hat den Zug bis B gebracht.“ benennt genau die geleistete A–B-Fahrt. |

## 5. Konkrete Kontextproben und zusammenhängende Dramaturgie

### Anfang: Stammstrecke – Fahrplan, der am Bahnsteig scheitert

In S1-01 R04 ersetzt Meldung 02 das selbstreferenzielle „Der aktuelle Stand …“ durch:

> **„Der Aushang ändert die Abfahrt. Der Bahnsteig wartet noch auf die passende Zeile.“**

Meldung 03 löst nicht die Uhrzeit, sondern gibt dem Spieler die richtige Art von Erfolg zurück:

> **„Die selbst gebaute Strecke führt von A nach B. Die Anzeigetafel sucht weiter nach einer Uhrzeit.“**

Das trägt sowohl die konkrete Fahrgastfolge als auch das satirische Gefälle: Anzeige und Strecke sind nicht mehr beide abstrakte Aktenobjekte. Die späteren Wegweiser und die unveränderten starken Routenabschlüsse halten den Einstiegston zusammen.

### Mitte I: Bauabschnitt – aus dem Schweigen des Stellwerks wird eine Fahrtlage

Die Engpassroute verbindet in R06 Meldungen 06–08 freie Gleise, Gegenrichtung, Signal und Hinweg. R07 führt anschließend dieselbe Grundidee durch zwölf unterschiedliche Situationen: von

> **„Das Stellwerk schweigt. Vor dem Signal wartet bereits die erste Fahrt.“**

über

> **„Die Durchsage läuft weiter, weil am Signal weiterhin niemand weiterläuft.“**

bis zu

> **„Ihre Strecke macht den Stellwerksabschnitt befahrbar. Das genügt für heute als Rückmeldung.“**

Damit ist die konkrete Fahrgastfolge zuerst Ursache der Komik und erst danach die dienstliche Rückmeldung. Das behebt die ursprüngliche Anfrage-/Antwortrekursion inhaltlich.

### Mitte II: Außenast und Umleitung – Orte und Objekte bleiben der Gegenstand

R11 zeigt statt „Bearbeitungsständen“ zwei Zugteile: „Der vordere Zugteil fährt weiter. Der hintere wartet noch am Abzweig zum Depot.“ Das Ergebnis „Die Strecke verbindet beide Zugteile …“ löst genau diese Lage ein.

R14 und R16 steigern die räumliche Friktion, ohne Dauer oder Regel zu erfinden. Beispiel R14:

> **„Zusätzliche Reisezeit ist eingeplant. Die Wegweiser verteilen sie großzügig über mehrere Bahnsteige.“**
>
> **„Ihre Verbindung führt durch die Umwege. Der Zug fährt den längeren Weg, statt ihn nur anzukündigen.“**

R16 macht aus der Alternativenrekursion eine Bildfolge – erstes Schild, Ersatzschild, gesperrter Bahnsteig, ehemaliger Regelweg – und lässt den Zug dann praktisch weiterkommen. Das ist konkret genug für Bahnfahreralltag und bleibt bewusst eigenständig statt einer Reiseauskunft.

### SEV und Finalkontext: die Strecke gewinnt, das Finale bleibt trocken

R18 fasst die Ersatzhalt-Friktion präzise:

> **„Am Ersatzhalt wartet die Anzeige auf den Bus. Sie meldet weiterhin nur: unterwegs.“**
>
> **„Ihre Strecke führt bis zum Ersatzhalt. Der Zug erreicht ihn auf Schienen, nicht nach Busanzeige.“**

Die weiterführenden R20-Felder geben der behaupteten Stabilisierung eine sichtbare Gegenprobe, etwa „Zwischen den letzten Bahnsteigen fehlt noch ein Gleisweg.“ und „Die Verbindung steht. Der Zug fährt sie ab; die Bewertung darf danach gern hinterherkommen.“ Die unveränderten Meldungen 06–12 und insbesondere der Abschluss bleiben erhalten:

> **„Der Zug ist angekommen. Die Verbindung funktioniert. Für diese Strecke wird kein Ersatz mehr benötigt.“**

Auch die unveränderte Season-Schlussformel „Betrieb bis auf Weiteres stabil. Bitte bewahren Sie diese Information gut auf.“ bleibt außerhalb des Correction-Scope unverändert. Damit wird der satirische Abschluss nicht durch die präziseren Korrekturen ernster oder zu einer garantierten realen Reiseaussage.

## 6. Befunde

### HIGH-01 – Kurzstatus nimmt die eigene Lösung vorweg

| Feld | Befund |
|---|---|
| Schwere | **HIGH** |
| Key / Feld | `level.s1.04.03.07.status` · `shortText` |
| langer, korrekter neuer Status | „Drei Anzeigen nennen drei Wege. Gemeinsam ist ihnen nur, dass keiner zum gleichen Bahnsteig zeigt.“ |
| neuer Kurztext | „Drei Anzeigen, drei Wege. **Ihre Strecke verbindet die Bahnsteige.**“ |
| verletzter Maßstab | K05 aus `CORRECTION_BRIEF.md`: Kurzform muss Sinn/gegebenenfalls Pointe der neuen Langfassung tragen und darf keine überbestimmte Zusage durch Kürzung erzeugen. |
| konkrete Wirkung | Der Langstatus beschreibt eine ungelöste, widersprüchliche Wegweisungs-/Bahnsteiglage. Der Kurzstatus behauptet dagegen bereits die individuelle gelöste Strecke. Er ist daher weder semantisch äquivalent noch ein zulässiger kompakter Status; in einem Status-/Kartenkontext kann er die nach der Lösung verdiente Spielerleistung vorwegnehmen. |
| erforderliche Nacharbeit | Ausschließlich dieses bereits freigegebene `shortText`-Feld so nacharbeiten, dass es die drei widersprüchlichen Anzeigen bzw. den fehlenden gemeinsamen Bahnsteigbezug komprimiert. Kein Ergebnis, keine eigene Strecke und keine neue Zusage im Status vorwegnehmen. |

Dies ist kein Synonymurteil: Der Widerspruch liegt zwischen dem konkreten Langstatus und derselben vorhandenen Kurzform/Funktion. Er ist ein begrenzter Feldbefund; die zugehörige Langfassung und das Ergebnis von R15/07 bleiben kreativ tragfähig.

### Schwächerer Hinweis – R07-Ergebnisrhythmus beobachten, aber keine verpflichtende Nacharbeit

`level.s1.02.03.05–12.result` nutzt gehäuft das Muster „Ihre Strecke/Ihr Zugweg/Ihre Verbindung …“. Das ist als Leistungswürde sachlich begründet und die jeweiligen zweiten Sätze beziehen sich konkret auf Durchsage, Hintergrundarbeit, Aufenthalt, Fragen, Signal oder Rückmeldung. Deshalb entsteht **keine** neue bloße Dauerwitz- oder Behördennomen-Serie und kein weiterer Pflichtbefund.

Für eine spätere freiwillige Feinglättung kann die Vorlesewirkung dieses gemeinsamen Satzanfangs beobachtet werden. Daraus folgt ausdrücklich **kein** Zwang zu Synonymrotation und keine Erweiterung des freigegebenen Scopes.

### Nicht festgestellt

- **Keine weiteren Blocker** und keine weiteren High-Befunde in den übrigen 119 tatsächlich geprüften Copy-Feldern.
- Keine neu erfundene Zeit, kein Stern-/Bonusversprechen, keine garantierte Anschluss- oder reale Reiseaussage und keine Echtzeitbehauptung.
- Keine neue Rätselregel, kein sichtbarer INTERNAL-Einstieg und keine Produktidee in einem Korrekturfeld.
- Keine systematische Formel-/Dauerwitzserie: Die wiederkehrende Spielerleistung ist funktionaler Abschlussanker; die konkreten Ursachen rotieren deutlich zwischen Aushang, Signal, Zugteil, Wegweisung, Bus und Bahnsteig.
- Die bewahrte Grundlage – insbesondere Titel, angenommene Nachbarn, UI, INTERNAL, BLOCKED_PRODUCT und trockenes Seasonfinale – wird durch diesen Bericht nicht neu bewertet oder in Frage gestellt.

## 7. Fazit und klare Grenzen

Die Nacharbeit behebt die zentrale Ursache **in 119 von 120 Copy-Feldern** überzeugend: Aus Selbstverwaltung über Selbstverwaltung wird wieder konkrete, selbstironische Bahnfahrer-Satire. Die Season entwickelt sich weiterhin plausibel von Fahrplan-/Aushangchaos über Engpass, Stellwerk, Außenast und räumliche Umleitung bis zur Bus-/Ersatzhaltlage. Besonders wichtig: Die Ergebnisse machen die selbst gebaute Strecke wieder zur verdienten, sichtbaren Gegenbewegung – nicht zum weiteren Aktenstand.

Die eine Kurzstatusabweichung `HIGH-01` ist jedoch innerhalb der erlaubten elf Kurzformen eine reale K05-Verletzung. Daher lautet das **Verdikt für den bestehenden Korrekturbestand FAIL**. Eine sehr begrenzte Korrektur dieses einen bereits freigegebenen Feldes und eine erneute unabhängige Prüfung genau dieses Lang-/Kurzstatuskontexts sind erforderlich, bevor eine kreative Restannahme erwogen werden kann.

**Ausdrückliche Grenzen dieses QC:** keine Quellkorrektur durch diese Rolle; keine Ownerannahme; keine Produkt-, Import-, Rechts- oder Releaseannahme; keine Aussage zu Runtime, technischem Scope, Validatoren, CI, Build, Solver, Karten-/Zugreferenzen, Freigabestatus oder offenen Fremdgates. Alte und neue technische Evidenz wird weder ersetzt noch übertragen.
