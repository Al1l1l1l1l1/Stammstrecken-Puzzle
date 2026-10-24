# Stammstrecken-Puzzle – Kurskorrektur und vollständiges Release-Gesamtmandat

**Entscheidung:** MANUS-2026-10-20-02 · **Datum:** 20.10.2026 · **Entscheider:** Manus in der vom Owner beauftragten Produkt-/Produktionsführungsrolle.

**Autorität:** Direkter vollständiger Ownerauftrag „Produktionsweise ändern und das Spiel zügig zur Marktreife führen“ vom 20.10.2026; ausdrückliches Feedback in [Issue38](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/issues/38) sowie das während dieser Entscheidung tatsächlich im Chat betrachtete aktuelle Handfoto. Dies ist eine neue ausdrückliche Entscheidung, keine rückwirkende Befugnisbehauptung.

**Verankerung:** [WP-045](../WORK_PACKAGES/WP-045_Produktionskurskorrektur-und-Release-Gesamtmandat.md) mit eigenem gemeinsamem Erstanker und [unveränderlichem Governance-Manifest](../tools/architecture-validation/scopes/WP-045.governance.scope.json), Basis `8d2ff1950fa72777014eda0555b02b6e9fd9a995`. Development: `codex/wp-036-vollstaendige-textbibliothek`, [PR31](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/31). Mainbasis `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Korrigierte Stilquelle bleibt `9e3f209cd92d491ab3a1ab51cce63677d3b5d53e`; Level-/Proofgrundlage `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d`. Finale tatsächliche Veröffentlichungssha wird in der ausgelieferten einzigen Koordinatorfassung ergänzt.

## 1. Was falsch gelaufen ist und was erhalten bleibt

Die bisherige Produktionsführung hat technische Nachvollziehbarkeit zu stark zum eigentlichen Lieferergebnis gemacht. Der Public240-/Saveweg wurde in T/A/S/P/I, Montage, Fix und Closeout zerlegt. Die tatsächliche Historie enthält unter anderem einen P-Merge ohne Dateidiff und separate Integrations-/Nachweis-/Schlussstände. Viele Übergaben und eng begrenzte Befugnisse verhinderten, dass ein Verantwortlicher ein großes Stück Spiel wirklich fertig liefern durfte.

Zugleich starteten Unity-/Architecture-Workflows auf Branch-Push und PR. Unity lizenzpflichtige Schritte liefen am Personal Seat seriell. Eingesehene Einzelfälle lagen bei rund 45–68 Minuten Workflowdauer; ein Push brauchte mit Queue rund 88 Minuten. Das sind reale Einzelbeobachtungen, keine allgemeine Kosten-/Leistungsrechnung. Unveränderte Produktquellen nach reinen Doku- oder Schlussupdates erneut zu bauen ist vermeidbare Wartezeit.

Der schwerwiegendere Fehler war ein fehlender echter Produktmaßstab: Technische CI und Scroll-Erreichbarkeit legitimierten eine tatsächlich schlechte mobile Oberfläche. Das neue Foto zeigt Status-/Generations-/QAtexte, Dropdown-/Verwaltungsaktionen, technische Textbuttons und Leerraum statt eines überzeugenden Spiels. Die angenommene Cartoon-Asset-Bible war noch keine umgesetzte Spielgrafik. Der Owner hat die tatsächliche Fassung kategorisch abgelehnt; ein korrigierter Saveindikator allein erledigt diese Beanstandung nicht.

**Erhalten und nutzen:** der funktionierende Rätselkern, sechs Formen/Notizsemantik, vorhandener Solver-/Hash-/Proof-/Validierungspfad, alle verwertbaren 240 DRAFT/Reviewinstanzen, Save-/Ledger-/CAS-/Commit-vor-Fahrt-Grundlage, kreativ angenommene Textbibliothek und korrigierte Cartoon-Gestaltung. Kein Enginewechsel, kein Neubau des Fundaments, keine neue globale Recherche oder Konzeptphase. Die 240er-Prüffassung und Skelette sind dennoch keine fertige Produktionskampagne, Monetarisierung oder Storefassung.

## 2. Jetzt geltende Produktionsführung

Die Vorgaben in AGENTS, WORK_PACKAGE_RULES, AI_HANDOVER_RULES und DEFINITION_OF_DONE werden tatsächlich ersetzt. Build-/Test-/Steuerungsfachdateien werden passend fortgeschrieben.

1. **Ein vollständiger Gesamtauftrag an R als Releaselead.** R verantwortet drei große parallele Lieferbahnen bis zum integrierten, einreichungsfähigen Release Candidate. Der Auftrag endet nicht nach einer weiteren internen Prüffassung. R darf selbst planen, Spezialisten beauftragen, innerhalb seines Scopes entscheiden, korrigieren, prüfen, nachbessern und integrieren. Der Owner ist kein Bote zwischen diesen Schritten.
2. **Ein großer prospektiver Programmanker ist ausreichend.** R verankert ein vollständiges Release-WP mit realen benannten Modulgruppen/Tests/Integrationsgrenzen und eigenem Manifest gemeinsam vor Fachcode. Die drei Bahnen erhalten kurze interne Teilbriefe und eindeutige Dateieigentümerschaft; kein neues WP/Manifest für jeden Subagenten, DTO, Routinefix, Montage, Teilmerge oder Closeout. Braucht die gewählte echte parallele Branchführung unabhängig verankerte Lieferbahnen, sind höchstens deren tatsächlich nötige eigene Anker erlaubt; danach keine Mikrokette. Alte Manifeste bleiben unverändert.
3. **Normale technische Entscheidungen sind übertragen.** Responsive UIdetails, Klassen/Views, Presets, lokale Katalog-/Assetcomposition, Tests, SDKadapter im beschlossenen Stack, Bugfixes und interne Integration entscheidet R selbst. Echte neue Hash-/Save-/Sicherheits-/Provider-/Modul-/Datenverträge benötigen weiterhin eine kurze vorab dokumentierte Fachentscheidung und gezielte getrennte Review. R darf diese innerhalb des Gesamtmandats technisch abschließen; kein neuer Manusprompt für jede Vertragskonkretisierung. Neue Produktversprechen/Scopekürzungen oder externe rechts-/finanzrelevante Handlungen bleiben zuständig vorbehalten.
4. **Eine risikogerechte unabhängige Annahme je integriertem Produktmeilenstein.** Kein weiterer globaler Review allein für einen unveränderten Dokuhead, eine Berichtformatkorrektur oder Montage ohne neue Risiken. Konkrete Befunde werden im selben Auftrag behoben und gezielt nachgeprüft. Kritische Puzzle-/Save-/Kauf-/Consentfälle bleiben fachlich unabhängig abgesichert.
5. **Kurze aktuelle Steuerung, eine externe Hauptübergabe.** CURRENT_STATE und WORK_QUEUE zeigen die aktuelle Arbeit, nicht sämtliche alten Phasen als vermeintlich aktuelle Blocker. Historie bleibt in Git und eindeutig historischer Ablage. Originalbelege direkt im Repo/CI verlinken; der Owner erhält je Meilenstein nur eine vollständige Koordinatordatei und eine kurze Chatmeldung.
6. **Prüfen, wo sich wirklich etwas ändert.** Docs-/Status-/Scopeänderungen prüfen Syntax/Links/Scope, nicht Unity/IL2CPP oder den ganzen Altlevelkorpus. Neue UI braucht echte Touch-/Sichtprüfung; neue Instanzen brauchen ihre neuen Eindeutigkeits-/Proofnachweise; neuer SDK-/Savecode braucht passende Negativ-/Recovery-/Geräteprüfungen. Am finalen RC einmal vollständige aktuelle Releaseintegration, keine pauschale Wiederholung unberührter historischen Gutachten.

### Tatsächlich jetzt geänderte CIsteuerung

Ein stdlib-basierter Klassifikator und deterministische Positiv-/Negativtests werden in `tools/ci/` eingecheckt. `unity.yml` erhält einen günstigen Vorprüfpfad; seine zehn vorhandenen Unityjobs, Lizenz-, Test-, Evidence- und Artefaktschrittinhalte bleiben erhalten. Reine Dokumentation/Governance und reine Routingänderung bei identischen Unityrezepten melden ausdrücklich `NOT_REQUIRED_SOURCE_UNCHANGED` und starten keine teuren Unityjobs. Architecture-/Scopeprüfung bleibt fail-closed; JCS-/Textprüfungen laufen nach betroffener Änderungsklasse oder ForceFull.

**Doppelläufe jetzt konkret vermieden:** Ein bekannter Produkt-Arbeitsbranchpush liefert nur den günstigen Vorcheck und meldet `DEFERRED_TO_PR_CANDIDATE`, niemals Unity-PASS oder Quellen unverändert. Der bestehende vollständige Unitypfad läuft am tatsächlichen Produkt-PR-Kandidaten; PR-Trigger umfassen alle Zielbranches. Beide Anteile `base→head` und `head→synthetic merge` werden berücksichtigt, sodass veränderte Basis-/Mergeunionen nicht als Docs-only verschwinden. Manuell/ForceFull, neuer Branch ohne echte Basis, unbekannter Pfad oder echte Rezeptänderung bleiben `REQUIRED`. Dies ist Kandidatensteuerung, keine behauptete Wiederverwendung ungeprüfter früherer Buildartefakte. `cancel-in-progress: false` bleibt; keine aktive Lizenzarbeit wird blind storniert. Neunzehn neue deterministische Tests und der Normalvergleich der unveränderten zehn Unityrezepte sind lokal bestanden; der tatsächliche GitHub-Vorcheck nach Veröffentlichung ist separat zu bestätigen. Die konkrete Beschleunigung ist damit implementiert und nicht als neue kleine Nacharbeit an R zurückgegeben.

## 3. Beschlossener fertiger Launchzustand – keine stille Kürzung

| Bereich | Vollständiges Ergebnis |
|---|---|
| Rätsel | 240 handkuratierte eindeutige Season1-Rätsel, fünf Abschnitte, 20 Routen à zwölf Meldungen, raffiniert steigende Logik ohne Sonderfelder. |
| Bedienung | Direkte Werkzeugwahl → Tap; gleiches Werkzeug auf gleichen Inhalt → leer; Werkzeug bleibt aktiv. Sechs Formen, X, Grau, Leeren, Undo, aktive Hinweise. Keine Mehrfachauswahl/Anwendenverwaltung. |
| Grafik/Spielgefühl | Echte sorgfältige Cartoon-/Comic-Bahn-Alltagsinszenierung nach korrigierter WP035-Basis; passende Schrift-/Farben-/Icon-/Zug-/Weltquellen. Kein QAformular im Produkt. Raster/Targets bleiben bei Tap/Undo/Autosave ortsfest. |
| Ablauf | Erster Einstieg → Start/Betriebslage/Karte → Puzzle → verifizierter Abschlusscommit → Zug fährt tatsächlichen Spielerpfad → Sterne/Punkte/Fortschritt → nächstes Puzzle. |
| Metagame | Erspielbare kosmetische Sammlung/Betriebswerk, gespeicherte Bestwerte, erster Clear/Übungsfahrt, bewertete Betriebsrevision erst nach Seasonabschluss; keine Leben, Wartezeiten oder Fehlerstrafen. |
| Postgame/Tagesangebot | Dauerbaustelle mit neuen geprüften Rätseln und vier wählbaren Profilen nach 240 Clears; freiwillige tägliche Meldetafel. Nicht als „später vielleicht“ versteckt. |
| Geld/Ads | Beschlossene faire Interstitialtaktung, freiwillige +10-Punkte-/Hint-/Dailyvideos, dauerhaftes Werbefrei-Non-Consumable mit Restore; keine Werbung während Denken/Lösung/Fahrt. |
| Veröffentlichung | Offlinefähige lokale Inhalte/Save, getestete Android-/iOS-Produktion, Privacy/Consent/Käufe/Fallbacks, echte signierte Storeartefakte, Rechte-/Lizenz-/Datenschutzhinweise, Storelisting/echte Screens und kurzes eigenständiges Launchmaterial. |

Die 480 Map-/Trainreferenzziele bleiben vollständig aufzulösen. Das sind 240 Karten- und 240 Zugmomentreferenzen, **keine Forderung nach 480 Unikatillustrationen**. Fünf Weltgrundlagen/20 Routenkompositionen und wiederverwendbare Zug-/Objekt-/UIbausteine dürfen hochwertig zusammengesetzt werden. Saubere Wiederverwendung ist gewollt, billige austauschbare QAoptik nicht.

## 4. Jetzt getroffene Produktentscheidungen

### P01 – Direkte mobile Bedienung und reale Qualitätsgrenze

O1/O2 werden wortsinngemäß umgesetzt: kein Mehrfachauswahl-/Anwendenmodus; aktives Werkzeug bearbeitet eine Zelle sofort. Anderer Inhalt wird ersetzt, identischer Inhalt geleert; das Werkzeug bleibt aktiv. Leeren und Undo bleiben. Gelb ist aktive Werkzeug-/neutrale Bedienhervorhebung, kein Gleisinhalt. Der Timer startet erst mit einer tatsächlichen Feldänderung. Routine-Save läuft unsichtbar oder in einer festen layoutneutralen Fläche; echte Fehler bleiben sichtbar und recoverbar.

Launch ist ein **Hochformat-Telefonspiel**, ohne dafür Tablet-/Landscape-Optimierung als zusätzliche Launchpflicht einzuführen. Kleine/große Telefonviewportgrößen und Safe Areas müssen funktionieren. R entscheidet Symbolabstände/Responsive-Details, Kontrast, semantische Screenreaderlabels, Haptik-/Ton-/Reduced-Motion-Optionen und bei Bedarf eine freiwillige stabile Detailvergrößerung für große Raster. Kein Zwangszoom, kein bei jedem Tap wanderndes Feld. Die eigentliche Farb-/Stilbasis bleibt angenommen; die aktuelle Umsetzung ist nicht angenommen.

### P02 – Späte Difficulty, Stars und Umgang mit internen Saves

Einstieg und gelungene Lernbögen bleiben. Der Puzzlelead prüft gezielt den gesamten letzten 48er-Abschnitt, alle 10×10 und die späten Routenabschlüsse; nicht lediglich zwei formal niedrige Metrikfälle. `S1-05-04-12` bleibt als ausdrücklich beanstandeter Ownerkontext zwingender menschlicher Prüffall, auch wenn seine Solverproxies hoch sind. Die vorhandenen Metriken zeigen bereits formal steigende Tiefe; daraus folgt **nicht**, dass sich diese Aufgaben für Menschen tatsächlich schwer genug anfühlen.

Neue Kandidaten bleiben eindeutig und ohne blindes Raten lösbar. Für anspruchsvolle 10×10 ist als vorläufiger Authoring-Vorscreen `deductionSteps >= 210`, `maxDeductionDepth >= 23`, `requiredGuessDepth = 0` beschlossen; das ist eine relative technische Baseline aus dem vorhandenen Late-Gamebestand, **kein Humanqualitätsbeweis und keine allgemeine Starzeit**. Entscheidend sind tatsächlich weniger sofortige Voll-/Null-Linienautopilot, verknüpfte entfernte Schlussketten und nicht früh offenliegende Gesamtgeometrie. Bewusste Erholungslevels dürfen nur mit nachvollziehbarer didaktischer Rolle abweichen. Der Lead erweitert/justiert diese technischen Proxies nach der echten früh geprüften Stichprobe innerhalb dieses Mandats, ohne jeden Zahlenwert wieder als Geschäftsführerfrage vorzulegen.

Nur tatsächlich veränderte DRAFT/Reviewrätsel erhalten neue vollständige Hash-/Proof-/Quality-/Projektionen. Kampagnenplätze können vor der **ersten Veröffentlichung** ihre unveröffentlichte Instanz verbessern; veröffentlichte semantische IDs werden nie umgedeutet. Alte QAprofile bleiben isoliert, werden nicht zu Productionprofilen und nicht mit Fakefortschritt migriert. Bei unpassendem internen Draft eine klar benannte Archiv-/Neustartoption, keinen stillen Verlust. Alle 240 Quellen/Referenzen werden abschließend vollständig validiert; nicht nach jedem UI-/Dokucommit neu erzeugt.

Sternzeiten entstehen nach stabiler neuer Bedienung aus echten Erstkontaktzeiten nach Logikklasse; jeder konkrete Leveldatensatz bekommt validierte eigene oder nachvollziehbar zugeordnete 2-/3-Sternwerte. Keine 240-fache Pflichtspielrunde je Person, keine erfundenen Messwerte. Startstichprobe: je Route ein Einstieg und ein Abschluss plus sämtliche 10×10 und alle geänderten Auffälligkeiten; Mehrfachmessungen unbekannter repräsentativer Aufgaben mit unterschiedlichen Spielstilen. Ausreißer und neue Kandidaten gezielt ergänzen. Der Contentlead dokumentiert Methode/Übertragung und prüft die Klassen mit echten Spielergebnissen. Reicht die Evidenz nicht, bleibt die betreffende Zeitklasse intern unfreigegeben – eine bloß ein-Stern-fähige Nullschwellenkampagne darf nicht still als vollständiger Launch durchgehen.

### P03 – Hinweisanspruch beschlossen, Rewarded-Hint erhalten

Die ältere ausdrücklich bestätigte Möglichkeit eines aktiv erbetenen Hinweisvideos bleibt erhalten. Sie wird **nicht** aufgrund einer abstrakten Masterformulierungs-Kollision gestrichen. „Keine Rätselvorteile durch Kauf/ Werbung“ bedeutet hier: kein Geldkauf von Kompetenz, exklusiven Hinweisen, Lösungen oder Sternen; dieselbe begrenzte Hilfe ist regulär kostenlos erreichbar. Das Video ist der bereits bestätigte freiwillige Zusatzweg, kein Zwang für Rätsel/Progress.

`hint-policy-v1`: pro `{puzzleId, publicPuzzleHash}` drei regulär kostenlose sichere Hinweise insgesamt – ein Startanspruch plus zwei reguläre Credits. Die Identität, nicht jeder Neustart, bestimmt „neu“; Abbruch/Neustart/Übung/Revision füllen nicht erneut auf. Jedes neue Endlosrätsel hat seinen eigenen gleichen Grundvorrat. Zusätzlich höchstens ein freiwilliges Hintvideo je Puzzleidentität erzeugt einen einzelnen persistenten Zusatzcredit; keine Geld-/Geduldspunktekäufe. Credits werden erst bei tatsächlich dargestelltem objektiv sicherem, nicht enttarnendem Schluss verbraucht. Fehlt ein solcher Schluss oder scheitert die Anzeige, bleibt der Credit/Anspruch. Ein vorhandener Credit wird vor einem weiteren Video verwendet. Keine automatische Hint-/Werbeaufforderung.

Sterne/Fortschritt bleiben uneingeschränkt; nur das Profilmerkmal „direkt gelöst“ entfällt. Hinweiswerbung darf ausschließlich nach ausdrücklicher Anforderung in einer pausierten Hilfeoberfläche erscheinen, nicht überraschend während normaler Eingaben. Ohne Ads/Consent bleiben die regulären kostenlosen Credits nutzbar. Die begrenzte persistente Identity-/Credit-/Claimsemantik wird innerhalb der B/C-Lieferbahn technisch mit Save-/Callbacktests abgeschlossen.

### P04 – Tagespolicy und ehrliche Offlinegrenze

Maßgeblich ist das lokale Gerätedatum beim Claim. Pro Datum ein kostenloser und höchstens ein zusätzlicher freiwilliger Videodurchlauf. Kein Serverzeitkonto, keine künstliche Zukunftssperre, kein Fortschrittsverlust bei Reise/Uhränderung. Deduplikation ist best-effort mit begrenzten persistenten Claimrecords; alle tatsächlichen Callback-/Saveoperationen bleiben idempotent. Die rein kosmetische kleine Uhrmanipulationsmöglichkeit wird bewusst toleriert, nicht als angeblich servergesicherte Wahrheit verkauft.

Die Meldetafel bleibt das schon konzipierte kleine Zufallsextra, kein Casino oder Lootboxshop. Für jeden regulären/zusätzlichen Durchlauf: +10 Punkte mit 60 %, +15 mit 30 %, +25 mit 10 %. Werte/Chancen sind vor Teilnahme auffindbar; Ergebnis wird atomar festgelegt/gespeichert, Neustart würfelt denselben offenen Claim nicht neu. Auslassen verliert nichts; keine Streakstrafen, Kaufteilnahmen oder exklusiven Rätselvorteile. Es gibt zum Launch keine zufällig gewährten exklusiven Kosmetikitems; Punkte finanzieren die gezielte Sammlung. Diese Ausprägung konkretisiert den vorhandenen „Punkte oder kosmetischer Fortschritt“-Rahmen, sie streicht kein zugesagtes Einzelitem.

### P05 – Dauerbaustelle vollständig beauftragt

Nach 240 Erstabschlüssen öffnen Dienstbeginn (4×4–5×5), Regelbetrieb (5×5–6×6), Störungslage (6×6–8×8) und Großlage (8×8–10×10). Gleiche Regeln, exakt eine Lösung, kein blindes Raten, vollständig gebundener Descriptor/Hash/Proof und ehrlicher Budgetabbruch; keine Währung/Strafe beim Verwerfen ungeeigneter Kandidaten. Generierung läuft außerhalb der Eingabe-/Renderarbeit, passende geprüfte Aufgaben dürfen vorgehalten werden.

Der Puzzlelead erhält die Produktbefugnis, **einmal** das versionierte Qualitätsprofil aus tatsächlichen kuratierten Referenzen, neuen Kandidaten und einer menschlichen Stichprobe zu kalibrieren/locken. Start: zwölf gezielt gewählte Kandidaten je Profil, unten/mittig/oben, einschließlich Grenz-/Variationsfällen; mindestens ein vollständiger Erstkontakt je Kandidat, unabhängige Nachsicht der Auffälligkeiten, zusätzliche Fälle falls die Klassen sich nicht plausibel unterscheiden. Dieses risikogerechte Startsample ist keine statistische Populationsgarantie und wird bei widersprüchlichen Befunden erweitert. Keine fixe 120-Humanlevel-Vorprüfung als isolierte monatelange Vorstufe.

Exakter Duplicate-Schutz gegen den Kampagnenbestand und die jüngsten 50 ausgegebenen Endloshashes ist Pflicht; normalisierte Rotations-/Spiegelungsklone und sehr ähnliche Endpunkt-/Zahlmuster im jüngsten Fenster werden gefiltert. Der technische Noveltyvergleich und Metrikintervalle werden vor Produktaktivierung konkret versioniert, mit der echten Humanprüfung und getrenntem QC geprüft. `PRODUCT_APPROVED` entsteht erst nach diesen echten Nachweisen; kein heute erfundenes Profil-PASS. Produktdefinition ist jetzt entschieden, Umsetzung/Kalibrierung bleiben Lieferarbeit statt neuer Product-Ownerblocker.

Dauerbaustellenrunden schenken aus eigener korrekter Lösung 15 Punkte und ihre entsprechenden einmaligen Zeitsternboni; kein Routen-/Abschnittsbonus. Jedes neue Puzzle hat eigene deduplizierte Identität. Ein freiwilliges +10-Abschlussvideo bleibt begrenzt einmal je neuer Aufgabe. Die vorhandene providerfreie Skip-/Open-Claim-/Watermarkintegrität bleibt erhalten.

### P06 – Konkreter kosmetischer Launchkatalog und Werbefrei

Die vorhandenen Punktgrundwerte bleiben 15/+5/+10/+60/+150. Beschlossen sind die folgenden 14 rein kosmetischen Launchitems; tatsächliche Asset-/Locale-/Lizenzherkunft und unabhängige Frontgestaltung werden vor Runtime geprüft. Stilpassende vorhandene Familien dürfen wiederverwendet werden.

| Logischer Key | Anzeige | Erwerb |
|---|---|---:|
| train.stammrot.default | Stammrot 01 | sofort |
| obj.wartebank.ohne-anschluss | Wartebank ohne Anschluss | 120 Punkte |
| obj.wegweiser.vielleicht | Wegweiser: Vielleicht | 120 |
| obj.absperrung.laufzeit-offen | Absperrung mit offenem Ablaufdatum | 120 |
| obj.anzeiger.in-pruefung | Anzeiger: In Prüfung | 120 |
| livery.betriebsrot | Betriebsrot | 300 |
| livery.baustellengelb | Baustellengelb | 300 |
| obj.bahnsteig.provisorisch | Bahnsteig provisorisch | 300 |
| obj.tunnelportal.wartung | Tunnelportal mit Wartungsfenster | 650 |
| obj.sev-bucht.geplant | SEV-Bucht planmäßig ungeplant | 650 |
| train.ersatzgarnitur | Ersatzgarnitur | 1.200 |
| train.anschlusswunder | Anschlusswunder-Express | 2.500 |
| obj.signal.freigegeben | Signal: theoretisch frei | Stammstrecke erstabschließen |
| obj.tafel.trotzdem-weiter | Meldetafel: Trotzdem weiter | Season1 erstabschließen |

Die logischen Keys werden in die bestehenden zulässigen kanonischen Katalog-ID-/Assetadressformen gemappt; keine Schemaumgehung wegen Punktnotation. Ein Erwerbsmodus je Item, atomarer Ownership-/Ledgercommit, keine doppelte Abbuchung. Aktive unveröffentlichte Namen dürfen redaktionell rechtssicher konkretisiert werden, ohne kosmetische Wirkung/Preis/Freischaltprinzip still zu ändern.

`remove_ads` ist ein Non-Consumable mit **3,99 € deutschem Basispreis**, später entsprechend tatsächlichem Storepreispunkt/lokalen Steuern. Entfernt dauerhaft Interstitials in allen Modi, nicht die freiwilligen Videooptionen. Kein Abo, Echtgeldpunktepaket, weiterer Kaufprovider oder Echtgeld-Kosmetikshop. Dies ist eine Produktentscheidung, keine jetzt ausgeführte Storekonfiguration oder Zahlung.

### P07 – Datensparsame Provider-/Releasegrundlage

Google Mobile Ads/UMP nach bestehender gepinnter Linie, zunächst AdMob allein; Unity IAP nach bestehender Linie für Apple/Google und genau `remove_ads`. Keine zweite Mediation, Firebase Analytics/Crashlytics, D2C oder neues Backend zum Launch. Lokaler redigierter Diagnosering und Store-/Adaggregate reichen für den Start. Optionales Tracking wird nicht beauftragt: nicht-personalisierte/nicht-trackende Konfiguration und aktuelles UMP; falls ein tatsächlicher SDKdatenfluss dennoch Tracking wäre, vor diesem Flow korrekt entscheiden/ATT erfüllen oder ihn deaktivieren, nicht eine Rechtsfreiheit behaupten. Fehlender Consent/Netz/No Fill stoppt nur den Dienst, niemals das Spiel.

R darf rechtlich geprüfte vorhandene Pakete installieren, nötige native Konfiguration/Ports implementieren und Paket-/Configlocks fortschreiben. Ein unvermeidlicher sicherheits-/Storekompatibilitätsupgrade im selben Providervertrag ist nach sichtbarem SDK-/Privacydiff und gezielter Fachprüfung übertragene technische Entscheidung, kein pauschaler Anbieterwechsel. Identitäten dev/qa/staging/production und Kauf-/Grant-/Finalize-/Restoreintegrität bleiben streng getrennt.

Rechte-/Datenschutz-/Storeangaben beziehen sich auf die **tatsächlich eingebauten** Assets/SDKs/Datenflüsse. Physische Geräteprüfungen und die externe qualifizierte Rechtsendprüfung bleiben echte Gates. Nicht veröffentlichte frühere QAstände werden nicht als beliebig viele zu unterstützende Production-Legacy-Versionen behandelt. Erhaltene tatsächliche Migrations-/Risiko-/Providerpfade werden gezielt getestet; ein vollständig abwesender Analyticsprovider braucht einen Abwesenheits-/Netzbeleg, nicht zusätzlich die vollständige Aktivierungs-/Widerrufsmatrix eines gar nicht ausgelieferten Systems.

## 5. Ein vollständiger Auftrag an R, drei parallele Lieferbahnen

**R ist hiermit als durchgehender Releaselead für das folgende Programm beauftragt.** Die Koordination schreibt aus dieser Datei einen einzigen vollständigen Auftrag, nicht zunächst erneut eine Planungs-/Freigabeinterviewphase. R kann Arbeit sinnvoll teilen und intern fortsetzen. Die folgenden Ergebnisse/Schreibgruppen bilden den geplanten großen Scope; ihr eigener Erstanker folgt vor der neuen Produktion auf dem tatsächlichen veröffentlichten Developmenthead dieser Entscheidung.

| Bahn | Vollständiges verantwortetes Ergebnis | Primäre Eigentümerschaft |
|---|---|---|
| A – Spielerlebnis und Kampagneninszenierung | Produktionsentry statt QAformular; stabile direkte Bedienung, neue vollständige Screens, Karte/Routenwelt, reale Pfadfahrt/Audio/Haptik, 14 passende Kosmetikassets und reale 480 Referenzauflösungen; spielbarer vollständiger Weg. | Presentation/UI, Presentation/World, Audio, reale eigene UI-/Art-/World-/Scene-/Localeimportquellen. |
| B – Puzzlequalität und vollständiges Metagame | Gezielte späte Difficultyverbesserung, alle 240 vollständige validierte Quellen, echte Starzeitklassen, neue Endlosengine/Profil, produktive Campaign-/Completion-/Cosmeticskataloge, Progress/Bestwerte/Revision/Hint/Daily/Ownership-/Economypolicies und nötige Saveerweiterungen. | Content/Levels/Proofs/Catalogs/Quality/Generator, Editor.Content, Application-Policies, entsprechende Infrastructure.Content/Persistence-Dateien und ihre gezielten Tests. |
| C – Mobilservices und Releasefertigstellung | Echte UMP/Ads/IAP/Restore-/Privacyintegration, Profile/Signing/Releaseautomation, bestehende sichere CI-Steuerung für das große Produktionsprogramm pflegen, Geräte-/Sandbox-/Storekanalprüfung, Release-Lock/-Manifest, fertige Store-/Lizenz-/Privacy-/Launchvorlage. | MobileServices.Google/Store, Platform, entsprechende Coordinator-/Build-/Providerdateien, Packages/ProjectSettings/Workflows, Release-/SDK-/Legal-/Storeartefakte. |

R besitzt zentrale Bootstrapcomposition, Workflow-/Projektsteuerung und Integrationsdateien. Spezialisten bearbeiten nur ihre eindeutig vereinbarten Dateien, nicht gleichzeitig CURRENT_STATE oder dieselben Coördinatoren. Innerhalb B müssen Puzzle/Generator und Metagame gleichzeitig bearbeitbar sein, innerhalb A Art/World und tatsächliche UX. Diese Spezialisierung erweitert Ergebnisse, nicht die Zahl äußerer Auftragsschleifen.

**Gemeinsame Vorbereitung ist kurz:** aktuelle Quellen/Dateieigentümer, Produktionsentry-/Profilgrenze, ID-/Katalog-/Asset-/Portübergaben, Saveversion und Test-/Buildwege abstimmen. Nur echte Vertragsänderungen gezielt vorab entscheiden. Keine neue globale Architekturspezifikation, kein erneutes Train-Tracks-Verstehen, keine 480 Assetbrief-Einzelinterviews.

**Befugnisse bis zum Endergebnis:** R darf die Bahnen nach relevanter unabhängiger Annahme normal nach Development integrieren, Mergekonflikte beheben und denselben Scope bis zur kompletten RC-Vorlage fortsetzen. Nach bestandener integrierter RCprüfung und tatsächlicher Owner-Spielqualitätsannahme darf R den reviewten geprüften Stand über den normalen geschützten PRprozess nach Main integrieren; keine Force-Pushes, Branchschutzumgehung oder fremden ungeprüften Features. Ein solcher Mainmerge ist **heute nicht ausgeführt** und bedeutet keine öffentliche Storefreigabe.

R bereitet Production-Signing und interne Store-/TestFlightverteilung nur mit tatsächlich eingeräumten Rollen/Zugängen und deren passender externer Handlungsautorisierung vor. Er fordert nötige Credentials sicher und gebündelt an, keine Secrets in Chats/Repo. Kontoverträge, Steuern, Auszahlungsangaben, neue Zahlungen und öffentliche Storeeinreichung/-promotion bleiben die konkret autorisierte Ownerhandlung; die fertige genaue Einreich-/Listing-/Buildvorlage wird dazu einmal abschließend präsentiert. Keine nachträgliche Selbstfreigabe einer Rechtsprüfung.

## 6. Wenige echte Meilensteine und Kontrollen

**M1 – früh sichtbares gutes Spiel, Arbeitsziel binnen drei Arbeitstagen nach Ausführungsstart.** Eine tatsächlich installierbare neue mobile Fassung liefert Start/Kartenkontext → 6×5-Puzzle → gespeicherter Abschluss → echte Zugfahrt → Ergebnis → nächstes Puzzle, plus echte 10×10-Bedienprobe. Schon hier hochwertige Cartoon-/Symbol-/Typographieausführung statt neutrales Gerüst. Direkte Togglebedienung, keine Multi/Anwenden-/Debuglast, Raster/Tools bei Save/Undo stabil. Positionsinvarianz nach abgeschlossenem Layout: gleiche Zell-/Toolrects, nur unvermeidbares Rundungstoleranzmaß, keine sichtbare Verschiebung. Neue Laufzeitregressionen und tatsächlicher Handtest. Owner erhält **eine** installierbare Fassung samt kurzem Bild/Video, nicht zehn Referenzdateien. Ein visueller Fehlbefund wird unmittelbar in A korrigiert; unabhängig mögliche B/Carbeit läuft weiter.

**M2 – vollständiges integriertes Spiel.** Alle 240 und fünf Welten/20 Routen laufen; Karte, Sterne/Punkte, Sammlung, Revision, Hint/Daily/Dauerbaustelle und echte neue Difficulty sind fertig. R prüft vollständigen Erstlauf-/Postgameflow, Save/Restore/Offline/Abbruch, Katalog-/Hash-/Proof-/Reward-/Ownershipintegrität und entsprechende neue UI-/Audio-/Performancefälle. Eine unabhängige ergebnisbezogene Prüfung kontrolliert die integrierte Anwendung und geänderten Risiken. Owner beurteilt tatsächlich neue Oberfläche und repräsentative späte Aufgaben. Daten-/Proxy-PASSs ersetzen diese Spielannahme nicht.

**M3 – einreichbarer vollständiger RC.** Android/iOS signiert/produktionseindeutig, echte Sandboxkauf-/Restore-/Ad-/Consentflows, installierter Storekanal-Smoke, gezielte physische Privacy-/Save-/Lifecycle-/Accessibilitytests, vollständiger Content-/Paket-/Lizenz-/Rights-/Privacy-/Storecheck. Einmal abschließende unabhängige Regression auf exakt gebundenen RC/Artefakten und eine vollständige Hauptübergabe. Schwere UIfehler, Save-/Geld-/Consentfehler, Contentmehrdeutigkeit oder fehlende externe Gatebelege verhindern M3.

Danach eine einzige gebündelte Ownerentscheidung zur konkreten Storeeinreichung/öffentlichen Veröffentlichung; keine erneute Anschlussplanung. Fremdbestimmte Storeprüfung ist kein Agenten-PASS. Erstveröffentlichung und spätere gestufte Updatepromotion sind in der technischen Releasevorlage plattformrichtig zu unterscheiden; historische Update-Rollout-Proxies dürfen den erstmaligen Start weder fälschlich garantieren noch ohne reale Plattformgrundlage unmöglich machen.

## 7. Zeitbild und wirklich terminbestimmende Abhängigkeiten

**Arbeitsziel: circa vier Wochen bis zu einem vollständigen einreichbaren RC; konditionaler Planungskorridor vier bis sechs Wochen ab Ausführungsstart.** Dies ist bewusst **kein belastbar nachgewiesener Fertigstellungstermin, keine Garantie und keine Storestartzusage**. Der Bestand enthält noch keine gemessene Produktionsrate für fertige Runtimeassets und die derzeitigen Provider sind Skelette. Die konservativen Fachschätzungen bei nur zwei serialisierten Umsetzungsrollen lagen bereits bei fünf bis acht Wochen Metagame plus späterem Releasepfad. Das würde das vom Owner abgelehnte Modell praktisch fortführen. Deshalb wird Releasearbeit nicht erst nach dem fertigen Metagame gestartet, sondern von Tag1 parallel; Puzzle/Generator, Metagame, UI/Art und Mobilservices bekommen getrennt arbeitsfähige Spezialisten unter R.

| Fenster als Arbeitsplanung | Sichtbares Sollresultat |
|---|---|
| Start bis Tag3 | Neue echte UI-/Pfadfahrtslice und 10×10probe; Konten/Recht/Geräte gleichzeitig klären, gemeinsame Schnittstellen kurz fixieren. |
| Woche1–2 | A: komplette UI-/Weltbausteine; B: Late-Game-Kandidaten/Starstichproben plus Metagamekataloge, parallel Generator; C: reale SDK-/Production-/Signing-/Storevorbereitung. |
| Woche2–3 | Vollständiger M2flow zusammenführen, gezielte Defekte schließen; Humanzeiten/Difficulty/Profile innerhalb desselben Programms finalisieren. |
| Woche3–4 als Ziel, Reserve bis Woche6 | M3-Geräte-/Privacy-/Kauf-/Releaseprüfung, tatsächliche Lizenz-/Rechts-/Storevorlage und finale RC-Korrekturen. |

Voraussetzungen für dieses aggressive Arbeitsziel: mehrere parallel tatsächlich arbeitsfähige Fachrollen und passende Toolingmöglichkeiten, keine neue Engine/Provider-/Backendlinie, M1 wird zügig akzeptabel, Zugang/Signing/Geräte und qualifizierte Rechtsprüfung werden früh verfügbar; Humanbeurteilung startet mit stabiler UI und nicht erst nach allen Assets. Bei fehlendem Applezugang, Testgeräten, Testergruppe oder problematischem SDKdatenfluss kann auch sechs Wochen nicht seriös zugesichert werden.

**Replanung spätestens nach M1/Tag3:** R legt anhand tatsächlicher Slice-/Asset-/Kandidaten-/SDKfortschritte und externer Zugangslage einen konkreten Restplan vor. Er nennt früh Abweichungen und deren Ursachen. Keine erneute lange Konzeptrunde, keine pauschale weitere Achtwochenfortsetzung, keine heimliche Scopekürzung. Wenn die reale Durchsatz-/Zugangslage das Vier-bis-sechs-Wochen-Ziel nicht trägt, wird dies sofort als Risiko gesagt und ein präziser Gegenplan innerhalb des Programms umgesetzt; eine mögliche echte Launchumfangänderung benötigt ausdrücklich Ownerentscheidung.

**Extern und überlappend:** Für nach dem 13.11.2023 angelegte persönliche Google-Play-Konten können zwölf durchgehend optierte Tester über mindestens 14 Tage und danach Production-Access-Prüfung erforderlich sein. Kontotyp/-alter sind unbekannt; sofort klären und mit genügend stabiler Testfassung überlappend starten, nicht am Ende entdecken. Organisation/ältere bereits berechtigte Konten haben diesen speziellen Wartepfad nicht pauschal. Apple-/Google-Review, Rückfragen, Konto-/Rechtsprüfung und öffentliche Freigabe liegen außerhalb einer Agentengarantie. Aktuelle Toolbaseline: Google API36, Apple Xcode26/iOS26-SDK; vor Einreichung erneut den aktuellen zuständigen Mindeststand bestätigen.

## 8. Gebündelte notwendige Ownermitwirkung

**Jetzt eine gebündelte externe Bestandsklärung:** Wer veröffentlicht rechtlich; bestehen Apple Developer/App Store Connect, Google Play Console und AdMob, welche Rollen/IDs/Signing sind vorhanden, persönliches oder Organisations-Play-Konto/Erstellungsdatum, verfügbare physische Android-/iOSgeräte und reale Tester, zuständige qualifizierte Rechts-/Datenschutzprüfung und Support-/Impressumskontakt. R erhebt diese Angaben in einer einzigen geeigneten sicheren Übergabe; keine Blockade der parallel implementierbaren Produktarbeit.

**An M1/M2:** tatsächliche mobile Qualitäts-/Difficultybeurteilung mit kurzer installierbarer Fassung und klar genannten Prüfaufgaben. Keine Abnahme einzelner Presets, Toolklassen oder Dokumente. Star-/Difficulty-Testpersonen müssen unbekannte Aufgaben tatsächlich spielen; Lösungen/Screenshots nicht als Erstkontaktmessung ausgeben.

**An M3:** gebündelte Freigabe der tatsächlich geprüften rechtlichen/Datenschutz-/Storeangaben und der genauen öffentlichen Einreich-/Veröffentlichungshandlung. Die Produktdefaults P01–P07 sind mit dieser Manusentscheidung bereits entschieden; nicht dieselben Hint-/Daily-/Preisfragen als neue unbeantwortete Ownerblocker zurückreichen. Eine echte spätere Änderung wird explizit mit Folgen vorgelegt.

## 9. Grenzen und heute tatsächlich abgeschlossener Gegenstand

MANUS-2026-10-20-01 bleibt die konkrete kreative Stilannahme mit ihren minimalen Asset-Bible-Korrekturen; deren Ausführung ist in A enthalten. Seine Mehrfachauswahlforderungen, das Ende nach interner Montage und seine engeren Folgeproduktionsvorbehalte werden durch diese neue Entscheidung ausdrücklich abgelöst. Alte Herkunftsdaten und Nachweise bleiben unverändert nachvollziehbar.

Heute werden Produktionsführung, aktuelle Produktdefaults, CI-Docs-only-Steuerung und der vollständige Folgeauftrag beschlossen/verankert. **Nicht heute fertig:** neue Spieloberfläche, Difficultyaustausch, echte Sternezeiten, produktiver Katalog/Generator/Metagame, SDK-/Signing-/Geräte-/Human-/Rechts-/Storegates. Issue38 ist damit verbindlich in Bearbeitung, nicht durch Dokumentation als geschlossen ausgegeben. Keine aktuelle Mainintegration, Zahlung, Storekonfiguration/-einreichung oder Veröffentlichung.

Diese eine Datei ist der vollständige fachliche Koordinatorbrief. Der Koordinator nutzt direkte Repositoryquellen, formuliert daraus einen großen Auftrag an R und benötigt keine zehn separaten Nutzerattachments. Detailoriginale bleiben intern/Repositorybelege; endgültige reale Dokumentations-/CIprüfung und Veröffentlichungssha werden in WP045 und der einzigen ausgelieferten Kopie wahrheitsgemäß gebunden.

## Quellen für die externen Releasevoraussetzungen

- [Google: aktuelle Target-API-Anforderungen](https://developer.android.com/google/play/requirements/target-sdk).
- [Apple: aktuelle Uploadanforderungen](https://developer.apple.com/news/upcoming-requirements/).
- [Google: Testanforderungen neuer persönlicher Entwicklerkonten](https://support.google.com/googleplay/android-developer/answer/14151465?hl=en).
- [Google: UMP für Unity](https://developers.google.com/admob/unity/privacy).
- [Apple: Datenschutz und tatsächliches Tracking/ATT](https://developer.apple.com/app-store/user-privacy-and-data-use/).
- [Apple: App Review](https://developer.apple.com/app-store/review/) – typische Zeiten sind keine verbindliche Zusage.

## Abschluss der heute beauftragten Kurskorrektur

Produktionsregeln und betroffene Fachfassungen sind tatsächlich nachgeführt. Aktuelle Steuerung: 21/23 Zeilen statt wachsender Phasenchronik; unveränderte Originale im historischen Pfad. Lokaler kanonischer Governance-/Scope-/Contract-/Negativlauf 18 Gruppen PASS, CIclassifier/Syntax/19 Unitfälle/Originalrezepte PASS, wirklicher heutiger Gesamt-Diff Unity-frei; relevante Produktbäume, Schemas/Fixtures und Trustanker unverändert. Eine unabhängige ergebnisbezogene Prüfung bestätigt Ausführbarkeit/Scope-/Sicherheits-/Produktgrenzen; ausschließlich zwei dokumentarische Aktualitäts-/Scopewortlautbefunde wurden im selben Auftrag gezielt geschlossen. Kein Produkt-, Unity-, Human-, Geräte-, Rechts- oder Storeabschluss dadurch vorgetäuscht.

Einmaliger normaler Finalcommit/Push nach Development; echte Head-/reguläre CIbindung und eine vollständige Koordinatorfassung werden danach über Git/PR und die ausgelieferte Datei nachgewiesen, nicht durch eine weitere selbstreferente Schlusscommitserie. Der nächste Auftrag ist jetzt Rausführung des gesamten Programms, keine weitere reine Anschlussplanung.
