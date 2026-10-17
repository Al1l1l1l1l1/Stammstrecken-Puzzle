# 06 – Motion und Sound

> **Entwurfsstatus / Freigabevorbehalt:** Dieses Kapitel konkretisiert ausschließlich eine Gestaltungs- und Produktions**empfehlung** für WP-035. Jede hier vorgeschlagene Dauer, Geometrie, Pegelbeziehung, Cue-ID und Importangabe braucht vor Produktion eine ausdrückliche Freigabe. Es liefert weder Animationen noch Sounddateien, ändert keine Produktregel und ist keine Rechts-, Qualitäts- oder Productionfreigabe.

## 1. Zweck und verbindlicher Rahmen

Die Inszenierung lässt das Rätsel bis zum gültigen Abschluss eine ruhige technische Arbeitsfläche bleiben. Erst dann wird sichtbar und hörbar, dass die selbst gebaute Verbindung tatsächlich befahrbar ist.

| Quelle / Grenze | Übernommene Vorgabe für diesen Entwurf |
|---|---|
| [Level-Erlebnis](../../../Stammstrecken_Puzzle_Konzept_00-15/05_Level_Erlebnis_und_Qualitaetsdifferenzierung.md) | Denken bleibt ungestört; die Zugfahrt beweist die eigene A–B-Verbindung. |
| [Visuelle Identität](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md) | Sachliche Fläche wird erst nach der Lösung zur hochwertigen Miniaturwelt. |
| [UI- und Bedienung](../../../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md) | Ergebnisreihenfolge ist Hilfen zurücknehmen → Strecke hervorheben → Zug A–B → Ergebnisfläche. |
| [ADR-011](../../../DECISIONS/ADR-011-ui-assets-lokalisierung-und-audio.md) | Bestehende Unity-Audio-Gruppen und semantische Cues; kein zweiter Audio-Stack. |
| [Content Pipeline](../../../ARCHITECTURE/CONTENT_PIPELINE.md) | Assetadresse, Lizenzmetadaten, Rohformat und Importpreset sind später inventarisierbar. |

**Nicht verändert:** Braun bleibt konkrete Schiene, Grau offene Belegungsannahme, Rot X optionale Leerannahme und Gelb reine Auswahl. Kein Einrastmoment bewertet eine Annahme als richtig oder falsch. Die sechs Formen bleiben `NS`, `EW`, `NE`, `ES`, `SW`, `WN`; weder Motion noch Klang führen eine neue Gleisform, Schleife, Weiche oder Pfadautorität ein.

## 2. Gemeinsame visuelle Parameter

Die [Owner-Stilkorrektur](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md) legt die Cartoon-/Comic-Spielwelt und die glasklare Deutsche-Bahn-Anspielung fest. Fahrzeuge und Weltobjekte bleiben zeichnerisch konturiert und stämmig; keine fotorealistische Kamerafahrt oder PBR-Materialinszenierung. Die sachliche Bedienoberfläche ist der bewusste Kontrast zur Cartoonwelt. Klang bleibt selbst erzeugter, grafisch prägnanter Bahn-/Amtsmaterial-Charakter, keine realistische Fahrzeug-/Ansagensimulation.

Für alle semantischen UI-/Puzzleanteile gelten die gemeinsamen Tokens aus dem [Owner-Brief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md): `brand-red #C83D45`, `surface #F4F2EE`, `paper #FFFFFF`, `ink #20272C`, `track-brown #805535`, `assumption-gray #727B82`, `empty-red #B42E39`, `selection-yellow #F1C84B`, `selection-edge #6C5717` und `grid-line #CCD0D1`.

- **Schrift:** IBM Plex Sans für Zeiten, Werte und Bedienelemente; IBM Plex Sans Condensed nur für straffe Statuszeilen. Diese Verwendung folgt der gemeinsamen Empfehlung; Lizenz, Embedding und Weitergabe bleiben separat zu bestätigen.
- **Referenzraum:** Eine Rasterzelle hat die variable Kantenlänge `c`. Alle Maße darunter skalieren mit `c`; sie sind keine Geräte-, Touch-, Accessibility- oder Performancevorgabe.
- **Easing-Notation:** CSS-ähnliche kubische Bézierkurven beschreiben die gewünschte Bewegungskurve, nicht ein vorgeschriebenes Runtimeformat.
- **Vorschlagsmarkierung:** Jede Zahl in den Abschnitten 3–8 ist ein Freigabevorschlag. Aus ihnen folgt kein Zeitlimit, keine Wertung und keine neue Nutzeroption.

## 3. Eingabe, Auswahl und Rücknahme

Die folgenden Reaktionen beginnen **erst nach** einer bereits durch die vorhandene Bedienlogik angenommenen Aktion. Sie legen bewusst keine neue Geste, Drag-Schwelle, Orientierung oder Fehlertoleranz fest.

| Handlung | Vorgeschlagene Bildphase | Vorschlagsdauer / Easing | Kontakt und Rotation | Semantischer Cue |
|---|---|---|---|---|
| Aktives Werkzeug wechseln | Gelbe Werkzeugfläche legt sich als 0,028-`c`-äquivalenter Rand mit gelber Vollton-Werkzeugfläche um das aktive Symbol; vorheriger Rand wird ausgeblendet. | 96 ms, `cubic-bezier(.16,1,.3,1)` ein; 72 ms, `linear` aus. | Keine Drehung, kein Puls über 1.02×. | `stp/audio/ui-tool-focus` |
| Einzelfeld auswählen | Dünne Kontur in `selection-edge`; ein `selection-yellow`-Schleier mit 38 % Deckkraft über Papier, identisch zu Teil C bleibt innerhalb der Zelle. | 84 ms, `cubic-bezier(.2,.8,.2,1)`. | Kontur folgt immer der Zellkante, nie dem Gleisverlauf. | `stp/audio/ui-cell-select` |
| Mehrfachauswahl zeigen | Jede bereits von der bestehenden Auswahl erfasste Zelle erhält dieselbe Kontur; die zuletzt ergänzte Kontur darf 40 ms später erscheinen. | 84 ms plus höchstens 40 ms Versatz je sichtbarer Ergänzung. | Keine Kettenbewegung und kein Farbwechsel der Feldinhalte. | `stp/audio/ui-selection-add` |
| Rotes X setzen | Zwei gleich breite Diagonalen werden von Mitte zu Endpunkt aufgebaut; danach ruhen sie bei 86 % der Gleisstrichstärke. | 132 ms, `cubic-bezier(.16,1,.3,1)`. | Kein Aufprall, keine Rotation. | `stp/audio/grid-empty-mark` |
| Graue Belegungsnotiz setzen | Das kleine Standardgleis wird von 92 % auf 100 % skaliert und von 0 % auf 100 % sichtbar. | 144 ms, `cubic-bezier(.2,.8,.2,1)`. | Zentral, mit 0° Drehung; niemals als graue Kurve. | `stp/audio/grid-occupancy-note` |
| Konkrete Form setzen | Schiene erscheint entlang ihrer tatsächlichen Gerade oder ihres Viertelbogens; ein feiner dunkler Rand folgt 36 ms nach dem Körper. | 168 ms, `cubic-bezier(.16,1,.3,1)`. | Gerade hat 0°/90°; Kurve zeichnet ihre `90°`-Drehung aus der Formgeometrie, nicht aus einem freien Spin. | `stp/audio/grid-track-seat` |
| Leeren | Inhalt verblasst in seinen eigenen Ursprung zurück; der Zellgrund bleibt unverändert `paper`. | 120 ms, `cubic-bezier(.4,0,.2,1)`. | Keine Schüttelbewegung und kein roter Fehlerton. | `stp/audio/grid-clear` |
| Undo | Genau die zuletzt sichtbare Feldänderung wird in umgekehrter Reihenfolge zurückgeführt; die Werkzeugauswahl bleibt, wie sie vor Undo bereits sichtbar war. | 156 ms, `cubic-bezier(.4,0,.2,1)`. | Rückwärtsrichtung ist zeitlich, nicht eine physische Rückwärtsrotation. | `stp/audio/ui-undo` |

**Einrastregel:** Einrastfeedback bestätigt ausschließlich „Aktion wurde dargestellt“. Gleiche Länge, Pegelklasse und Bewegungsenergie gelten für X, Note und konkrete Schiene. Objektive Randzahl-Überbelegung darf weiterhin kurz `empty-red` anzeigen, bekommt aber keinen Strafklang, keinen Ausschlag und keinen Hinweis auf Lösbarkeit.

## 4. Gültiger Abschluss: feste Präsentationsfolge

Die folgenden Phasen beginnen ausschließlich nach dem vorhandenen Nachweis einer **gültigen** A–B-Verbindung. Der Entwurf führt keine zweite Completion-Prüfung ein und macht aus keiner Teilaktion eine Wahrheitserklärung.

| Phase | Vorschlagsfenster ab Valid-Event | Bild und Kamera | Audio / Sperre |
|---|---:|---|---|
| P0 · Beruhigen | 0–120 ms | Die letzte Eingabe kann zu Ende auslaufen; keine neue Kamera- oder Farbdramaturgie. | Keine Fanfare; neue Feldaktionen werden nur während dieser Präsentation ruhend gehalten, nicht bewertet. |
| P1 · Hilfen zurücknehmen | 120–300 ms | Alle roten X und grauen Standardgleise gehen gleichzeitig von 100 % auf 0 % Deckkraft. Werkzeuggelb, Mehrfachauswahlkonturen, Randzahl-Überlaufblitze, Kopfzeilenhilfen und Timer treten auf 0 % bzw. in den Hintergrund. | `stp/audio/completion-clearance` als kurze Entlastung; keine Sprache. |
| P2 · Tatsächlichen Pfad zeigen | 300–600 ms | Nur die bereits gültige braune A–B-Linie erhält einen 0.055-`c`-hellen Rand und eine warme innere Aufhellung von maximal 12 %. Nicht zum Pfad gehörende Flächen bleiben `paper`. | `stp/audio/completion-route-reveal`; kein Stern- oder Belohnungston. |
| P3 · Miniaturraum öffnen | 600–900 ms | Kamerablick hebt sich von der senkrechten Arbeitsansicht um vorgeschlagene 14° in eine flache 2.5D-Perspektive; Zoom von 1.00 auf 0.94, Rasterzentrum bleibt am selben Bildschirmanker. Bahnsteig-, Tunnel- oder Signalkanten werden nur außerhalb der Logikzellen eingeblendet. | `stp/audio/ambience-scene-open` blendet ein; Musik bleibt untergeordnet. |
| P4 · Zugfahrt A–B | ab 900 ms, skalierbar | Der Zug fährt die hervorgehobene Strecke exakt von A nach B; die Kamera folgt mit ruhigem Vorhalt. | Fahr-, Roll- und Umgebungs-Cues nach Abschnitt 5. |
| P5 · Ankunft halten | nach `T_fahrt` bis `T_fahrt + 360 ms` | Zug stoppt an B ohne Federung; Pfad bleibt lesbar, Kamera hält den Zug und die Zielmarke. | `stp/audio/train-arrive` und ein sehr kurzer Raum-Nachhall. |
| P6 · Ergebnis einführen | anschließend 360–720 ms | Ergebnisfläche fährt von unten auf `surface` ein, ohne den Weg während des ersten Frames vollständig zu verdecken. | `stp/audio/results-panel-open`; erst jetzt Sterne, Zeit, Geduldspunkte und Kartenfortschritt. |

Die Anzeige folgt damit exakt: **Hilfen zurücknehmen → tatsächlichen Pfad hervorheben → Zug A–B → Ergebnis.** Erst nach vollständiger Ergebniswürdigung dürfen getrennte, vorhandene freiwillige Angebote oder die bestehende Werbelogik überhaupt in Betracht kommen; sie gehören nicht zu P0–P6.

## 5. A–B-Fahrt: Geometrie, Kurven und Kamera

### 5.1 Ableitung statt neuer Pfadautorität

- Der Präsentationspfad wird ausschließlich aus dem bereits gültigen logischen A–B-Pfad abgeleitet; weder eine Spline-Abkürzung noch eine freie Animationskurve darf Zellen überspringen oder einen anderen Weg zeigen.
- Gerade werden als Zellmittelachsen dargestellt. Jede `NE`/`ES`/`SW`/`WN`-Form wird als exakter Viertelkreis mit Radius `0.50 c` um die passende Zellinnenecke abgebildet.
- Der Pfad wird nach Bogenlänge parametrisiert. Dadurch bleibt die Geschwindigkeit auf Geraden und Viertelbögen gleichmäßig, statt in Kurven sichtbar zu beschleunigen.
- Kein Rigidbody, keine Kollisionsantwort, kein Pathfinding und keine Kameraentscheidung erzeugen eine alternative Strecke. Die Logikfläche wird nicht gebogen, gedehnt oder in eine andere Topologie überführt.

### 5.2 Vorschlagsparameter des Zugmodells

| Parameter | Freigabevorschlag | Zweck |
|---|---:|---|
| Fahrzeugkörperlänge | `0.78 c` | Eigenständiges, kompakt lesbares Miniaturfahrzeug innerhalb enger Bögen. |
| Drehgestellabstand `d` | `0.34 c` | Vorder- und Hinterkontakt bleiben bei `0.50 c`-Kurven sichtbar auf derselben Strecke. |
| Fahrzeugbreite quer zur Fahrt | `0.27 c` | Identisch zur Puzzlefahrt-Reduktion in Teil D; Pfadports bleiben unverändert auf Halbkantenzentren. |
| Schienenkörperhöhe in P3–P5 | `0.035 c` | Verleiht Materie, ohne ein neues Spielbrett zu bauen. |
| Anfahrzeit | 280 ms | Zug löst sich ohne Spielzeug-Sprung von A. |
| Fahrtzeit | `T_fahrt = 0.28 s + L × 0.22 s + 0.36 s` | `L` ist die Bogenlänge in Zellkanten; längere gültige Wege erhalten proportional mehr Fahrzeit. |
| Bremsweg | letzte `0.38 c` | Gleichmäßig in B übergehen, ohne Gummiband-Überschwingen. |

Für jeden Fortschrittswert `s` liegen die Drehgestelle bei `p(s + d/2)` und `p(s - d/2)`. Der Körpermittelpunkt ist ihr Mittel; seine Orientierung ist `atan2(p_vorne.y-p_hinten.y, p_vorne.x-p_hinten.x)` der Verbindung beider Drehgestelle; auf einem reinen Kreisstück entspricht das dem Tangentenwinkel. Auf einem Viertelbogen läuft die Rotation stetig über genau `90°`; sie springt nicht an der Zellgrenze und kippt nicht seitlich. Das ist ein Konstruktionshinweis für die spätere Präsentationsschicht, kein Ersatz für die Puzzlelogik.

### 5.3 Kameraverhalten

| Moment | Freigabevorschlag |
|---|---|
| P3-Aufklappen | Kamera bewegt sich mit `cubic-bezier(.33,0,.2,1)`; der Rastermittelpunkt bleibt fix, sodass kein Zellinhalt aus dem Bild „wandert“. |
| Gerade Fahrt | Kamerarahmen hält den Zug bei ungefähr 44 % Breite und 56 % Höhe; Vorhalt liegt in Fahrtrichtung und beträgt maximal `0.70 c`. |
| 90°-Kurve | Vorhalt gleitet entlang desselben Viertelbogens; weder harte Pan-Sprünge noch ein Kameraspin begleiten die Fahrzeugrotation. |
| Kurze Strecke | Kamera verbleibt in der P3-Totalen, damit A, B und die ganze Leistung lesbar bleiben. |
| Lange Strecke | Eine sanfte Follow-Kamera darf folgen, zeigt aber am letzten `1.20 c` wieder B und die Zielumgebung. |
| Ergebnis | Kein zusätzlicher Kamerazoom; die Ergebnisfläche kommt zur bestehenden Abschlussansicht. |

## 6. Ergebnis und Kosmetikpräsentation

### 6.1 Ergebnis nach der Ankunft

Die Ergebnisfläche erhält ausschließlich die schon vorhandenen Daten: Abschluss, verliehene Sterne, Zeit/Bestzeit, Geduldspunkte und Kartenfortschritt. Die Darstellung berechnet keine Sternlogik, setzt keine Zeitgrenze und erzeugt keine neue Meisterschaftskategorie.

| Schritt | Freigabevorschlag für die Präsentation | Cue |
|---|---|---|
| Überschrift | `STRECKE FREIGEGEBEN` erscheint mit 160 ms Opazitätswechsel und 8 px vertikalem Weg, nicht als Alarm. | `stp/audio/results-heading` |
| Sterne | Bereits vom Ergebnisdatensatz gelieferte Sterne erscheinen links nach rechts mit 140 ms Abstand; fehlende Sterne animieren nicht abwertend. | `stp/audio/results-star` je vorhandener Stern |
| Zeit / Bestzeit | Werte werden nach 220 ms eingeblendet; eine bereits gelieferte Bestzeitmarkierung bekommt nur einen `brand-red`-Unterstrich. | `stp/audio/results-stat` |
| Geduldspunkte | Gelieferter Betrag zählt über vorgeschlagene 480 ms sichtbar hoch; er wird nicht verdoppelt und nicht durch Sound als Kaufreiz inszeniert. | `stp/audio/results-points` |
| Kartenfortschritt | Kleine Karte zeichnet ausschließlich den neu freigegebenen Abschnitt in `ink`; ein Zugpunkt fährt höchstens 0.80 `c`-äquivalent darüber. | `stp/audio/results-map-release` |

### 6.2 Rein kosmetische Präsentation

Der gewählte Zug und seine Lackierung bestimmen nur Fahrzeugmesh, Material, ggf. leichtes Fahrgeräuschprofil und die Betriebswerk-Ansicht. Sie ändern weder `T_fahrt`, Pfadgeometrie, Hinweise, Sterne, Zeitwerte noch die Lautheitspriorität der Completion.

| Ort | Freigabevorschlag | Kameraregel / Material |
|---|---|---|
| Abschlussfahrt | Gewählte Garnitur fährt P4 als einheitliches Modell; optionale Lackstreifen bleiben flache, eigene Farbbänder auf der bestehenden Fahrzeugform. | Die Kameraregeln aus Abschnitt 5 gelten identisch für jede Kosmetik. |
| Betriebswerk | Der Zug steht auf einem geraden 1.8-`c`-Sockel vor flach gezeichneter Werkhallenfläche; eine langsame Vorführung dreht das Modell maximal 12° nach links und zurück. | 1.6 s je Richtung, `cubic-bezier(.33,0,.2,1)`; keine Lootbox-, Zufalls- oder Kaufanimation. |
| Betriebsobjekt | Signal, Bahnsteig, Wegweiser oder Tunnelportal erscheint als stilles Cartoonobjekt mit 80 ms Schatteneinblendung. | Keine Vorführung darf eine Rasterzelle, Spielregel oder Kartenfreigabe vortäuschen. |

## 7. Semantische Audio-Cues und Mixvorschlag

### 7.1 Cues sind Handlungen, keine Wahrheitsurteile

`IAudioPort` erhält nur semantische Cue-IDs; spätere Clips bleiben hinter dem Adapter. Die vorgeschlagenen Adressen erfüllen das vorhandene Muster `stp/<domain>/<stable-id>` und sind weder Dateipfade noch zugesicherte Assets.

| Cue-ID | Auslöser | Klangcharakter ohne kopierte Ansage/Jingle | Bestehende Mixergruppe |
|---|---|---|---|
| `stp/audio/ui-tool-focus` | Werkzeug aktiv sichtbar | trockener, kurzer Kunststoff-/Papierkontakt mit wenig Oberton. | UI |
| `stp/audio/ui-cell-select` | Zellauswahl sichtbar | heller Randimpuls, nicht melodisch und nicht bestätigend. | UI |
| `stp/audio/ui-selection-add` | Auswahl erweitert | gedämpfter Doppelimpuls, gleiche Tonhöhe unabhängig vom Feldinhalt. | UI |
| `stp/audio/grid-empty-mark` | X dargestellt | weicher Graphitstrich mit kurzer Luft. | SFX |
| `stp/audio/grid-occupancy-note` | Graue Note dargestellt | matter Bleistift-/Klemmbrettkontakt; klar leiser als Schiene. | SFX |
| `stp/audio/grid-track-seat` | Braune Form dargestellt | warmer Holz-Metall-Sitzimpuls mit kurzer Tiefe, ohne Triumphintervall. | SFX |
| `stp/audio/grid-clear` / `stp/audio/ui-undo` | Leeren / Undo | zurückgenommener Papierhub beziehungsweise sanfter Rückzug; nie negativer Buzzer. | UI |
| `stp/audio/completion-clearance` | P1 | flacher Ausatem aus Papier und fernem Schienenklang, ohne Sprachfragment. | SFX |
| `stp/audio/completion-route-reveal` | P2 | kurzer, aufsteigender mechanischer Ton mit zurückhaltender Wärme. | SFX |
| `stp/audio/ambience-scene-open` | P3 | leise fiktive Tunnel-/Bahnsteigluft, frei von realen Stationsansagen. | Ambience |
| `stp/audio/train-start` / `stp/audio/train-roll` / `stp/audio/train-curve` / `stp/audio/train-arrive` | P4–P5 | eigene Miniatur-Radkontakte, leichter Elektromotor, in Kurven eine sehr subtile Spurreibung und ein stumpfer Zielhalt. | SFX / Ambience |
| `stp/audio/results-heading` bis `stp/audio/results-map-release` | P6 | sachliche Wertigkeitsimpulse, nie ein fremdes Bahn-Jingle oder Gewinnspiel-Fanfare. | UI / SFX |

Die Formen unterscheiden Handlungen: **X = Graphit**, **offene Belegung = mattes Papier**, **konkrete Schiene = warmes Material**, **Auswahl = heller UI-Rand**, **Undo = Rücknahme**. Keiner dieser Klänge steigt wegen einer später richtigen oder falschen Vermutung in Tonhöhe, Länge, Lautheit oder Harmonie.

### 7.2 Vorschlag für Pegelbeziehungen

| Schicht | Vorgeschlagene Beziehung am Master | Frequenzcharakter und Ducking |
|---|---|---|
| Master | Ceiling bei -1.0 dBTP. | Transparent, keine nachträgliche Überkompression als Ersatz für Materialität. |
| Music | etwa -24 LUFS integriert unter ruhiger Rätselphase. | Weiche Mitten, unter 160 Hz zurückhaltend; bei P1–P6 um vorgeschlagene 4 dB absenken. |
| Ambience | etwa -30 LUFS integriert im Puzzle, an P3 auf etwa -26 LUFS anhebbar. | Diffuse Luft zwischen 250 Hz und 2.5 kHz; keinen Sprachformanten bauen. |
| Grid-SFX | kurze Ereignisse mit etwa -18 dBFS Sample-Peak, anfangs 3 dB über UI. | `grid-track-seat` hat Schwerpunkt bei 700 Hz–1.8 kHz; X und Note teilen keine identische Signatur. |
| UI | etwa 3 dB unter Grid-SFX. | Präzision vor Schärfe: Hochtonanteil oberhalb 5 kHz sparsam, damit Serienaktionen nicht ermüden. |
| Zugfahrt | Motor/Rollen liegen über Ambience, aber etwa 4 dB unter Completion-Route-Reveal. | 120–500 Hz für Gewicht, 1–3 kHz für Raddetail; in P5 über 220 ms in Ambience zurückführen. |

Diese Relationen sind ein Mixbrief, keine Lautheitsnorm für eine Plattform. Die vorhandenen Nutzerregler über `Master`, `Music`, `SFX`, `Ambience` und `UI` bleiben maßgeblich; dieses Kapitel fügt keinen neuen Schalter, keine Haptikregel und keine separate Audiooption hinzu.

## 8. Rohformat, Loop, Lizenz und Importbedarf

| Feld | Freigabevorschlag für spätere Beschaffung / Produktion |
|---|---|
| Rohformat | Quellen in 48 kHz, 24-bit PCM WAV; kurze UI-/SFX-Cues mono, räumliche Ambience und Musik stereo, sofern die Aufnahme dies trägt. |
| Schnitt | UI-Cues mit 5–12 ms Anfangs-Fade und sauberem Ausklang; kein abgeschnittener Transient und kein künstliches Klickende. |
| Loops | `train-roll`, Tunnel-/Bahnsteigluft und Musik nur mit dokumentiertem Sample-Start/-Ende, Crossfadebereich und Vorhörprobe; Übergänge dürfen keine reale Durchsage enthalten. |
| Lizenznachweis | Pro Quelldatei später Urheber:in, Quelle, Lizenztyp, Nutzungsgebiet, Bearbeitungsrecht, Namensnennung, Ablauf/Restriktion und Freigabestatus inventarisieren. |
| Verbotene Quellen | Keine übernommenen Bahnansagen, Stationsmelodien, Jingles, Durchsagen, Fahrzeugaufnahmen mit identifizierbarer Markenansage oder fremde Soundpacks ohne nachgewiesene Rechte. |
| Unity-Anschluss | Späteres Runtimeasset unter `Assets/StammstreckenPuzzle/...` mit `.meta`, versioniertem Audio-Importpreset, `audio`-Label und stabiler Addressables-Adresse; kein `Resources`-Pfad. |
| Importmetadaten | Cue-ID, Mixergruppe, Mono/Stereo, Loop-Punkte, Normalisierungsnotiz, Lizenzreferenz, Importpreset-Version und ggf. Kontextlabel dokumentieren. |

Keine in diesem Kapitel benannte Datei, kein Cue und keine Animation ist geliefert oder importiert. Ein fehlender optionaler Cue bleibt gemäß Pipeline ein sicherer No-op; die spätere Pflicht-/Optional-Einstufung ist nicht Bestandteil dieses Entwurfs.

## 9. Statische Referenz und spätere Prüfungen

Der statische [Phasen- und Cue-Spezifikationsbogen](references/08-motion-audio.html) zeigt P0–P6, die Skalierung der Fahrt und die Buszuordnung als reproduzierbare Dokumentationsansicht. Er ist weder App-Screenshot noch Runtimecode; die Hauptausarbeitung rendert daraus bei Bedarf eine PNG-Ansicht.

| Spätere externe Prüfung | Fragestellung | Einordnung jetzt |
|---|---|---|
| Hörtest mit lizenzierten Rohclips | Ermüdet die unterscheidbare Eingabepalette bei wiederholten Zügen nicht und bleibt sie ohne Wahrheitsaussage verständlich? | Spätere Produktionsprüfung, **kein zusätzlicher Startblocker**. |
| Laufzeit-/Geräteprüfung | Bleibt die A–B-Kamera auf kleinen und großen gültigen Rastern lesbar, ohne die Logikfläche zu verzerren? | Spätere Produktionsprüfung, **kein zusätzlicher Startblocker**. |
| Kurvenfahrt-Sichtung | Bleiben beide Drehgestelle an 90°-Bögen sichtbar geführt und der Zug ohne Physics stabil? | Spätere Produktionsprüfung, **kein zusätzlicher Startblocker**. |
| Lizenz-/Rechtsprüfung | Sind alle Aufnahmen, Bearbeitungen, Namen und eventuellen Sprecherbestandteile nachweisbar eigenständig nutzbar? | Spätere externe Freigabe, **kein zusätzlicher Startblocker**. |
| Mix auf Zielgeräten | Halten die vorgeschlagenen Pegelbeziehungen unter bestehenden Reglern und Interruptionsituationen? | Spätere Produktionsprüfung, **kein zusätzlicher Startblocker**. |

**Grenze der Ausarbeitung:** Diese Empfehlung beschreibt nur die Vorbereitung von Motion und Sound. Sie behauptet keine implementierte Zugfahrt, keine vorhandene Audiodatei, keine absolvierten Spiel- oder Hörtests und keine Freigabe von Produkt, Rechtslage oder Production.
