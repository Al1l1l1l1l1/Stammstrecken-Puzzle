# WP-047 – Die Bahnwelt in echtem 3D und alle Bildschirme neu

`WP-047`

**Bearbeitungsstatus:** Abgeschlossen · 27.11.2026 · Merge `35a0d9d7ff40179569ab3fc23cce3863cc3c5c61`

**Scope-Manifest des gesamten Reworks (gilt für `WP-047`, `WP-048` und `WP-049`):** [WP-047.production.scope.json](../tools/architecture-validation/scopes/WP-047.production.scope.json)

**Pflichtlektüre vor der ersten Codezeile, in dieser Reihenfolge:**

1. [Satire- und Lore-Bibel](../PRODUKT/SATIRE_UND_LORE_BIBEL.md) – vollständig. Besonders Kapitel 1–5 (warum das Spiel existiert, Humorformel, Pendler-Test, harte Grenzen, die Tabelle „wörtlich falsch / gemeint richtig“), Kapitel 12 (Optik als Satire, „bubbly“, Regeln gegen den PDF-Look), Kapitel 13 (die großen Momente) und Kapitel 14 (Welten).
2. [Konzept 2.1 – Rundum-Rework](../PRODUKT/KONZEPT_2.1_RUNDUM_REWORK.md) – vollständig, besonders Abschnitt 3 (Stil), 5 (Welt-Gags), 6 (Satire in den Systemen) und 9 (alle Bildschirme).
3. Dieses Arbeitspaket bis zum Ende.

Wenn etwas in den alten Konzeptdateien unter `Stammstrecken_Puzzle_Konzept_00-15/` anders steht (zum Beispiel „Die Oberfläche wirkt dienstlich“, „helle, klare Informationsoberfläche“), **gilt das hier und in `PRODUKT/`**. Die alten Dateien sind die Ursache des PDF-Looks.

---

## ID

`WP-047` – Bahnwelt in echtem 3D und alle Bildschirme. Lieferbahn „Welt“ des Rundum-Reworks.

## Ziel

### Was am Ende anders ist (in einem Satz)

Wer die APK öffnet, sieht kein Formular mehr, sondern **eine knubbelige, lebendige Comic-Bahnwelt im Ausnahmezustand**, in der er selbst Gleise baut – und jeder Bahnfahrer erkennt in drei Sekunden, worauf das anspielt.

### Warum dieses Paket existiert (bitte wirklich verstehen)

Der Owner (Alex) hat den letzten APK-Build getestet. Sein Urteil, wörtlich: „Schaut viel zu sehr nach PDF / flach aus“, „viel zu 2D (Spiel fühlt sich wie eine interaktive PDF an, vor allem die Menüs)“, „Schienen sehen nicht wie Schienen aus“, „Betriebshof-Bild KI-Slop (Dinge überlappen sich, falsch skaliert, ganz übel)“. Das betrifft **das ganze Spiel, sogar das Hauptmenü**: Es ist aufgebaut wie ein Dokument mit Überschrift, Unterüberschrift, Fließtext und Knopfliste.

Die Ursache ist nicht ein einzelner Fehler, sondern das Leitbild. Frühere Agents haben „dienstliche Informationsoberfläche“ wörtlich umgesetzt. Dieses Paket ersetzt das Leitbild:

> **Bisher:** Eine dienstliche Informationsoberfläche, auf der ein Puzzle liegt.
> **Neu:** Eine knubbelige Comic-Bahnwelt, in der das Dienstliche in **Gegenständen** steckt – Abfahrtstafel, Durchsage, Dienst-Mail, Stellwerkspult, Werkhalle, Stempel. Genau dort ist es komisch.

### Warum die Optik selbst Satire ist (nicht nur Deko)

Das Spiel soll aussehen, **als hätte der Designer der Bahn ein Spiel gebaut** – nicht kopiert, sondern „derselbe Designer“. Die Wiedererkennung entsteht über Bausteine, die jeder Pendler kennt: die tiefblaue Abfahrtstafel mit weißer Schrift und orangefarbener Verspätung, das weiße Störungsbanner mit orangem Balken, Verkehrsrot für die eigenen Züge, der Durchsage-Gong, das Fallblatt. Der Witz entsteht, wenn diese ernsten, offiziellen Gegenstände in einer knuffigen Spielzeugwelt stehen und **mit vollem Ernst Unsinn melden**. Die Tafel zeigt „+0:08“ neben Ihrer Bauzeit – genau wie am Bahnsteig. Das Baustellenschild hat fünf durchgestrichene Fertigstellungsjahre. Die Pendler applaudieren **ungläubig**, wenn tatsächlich ein Zug kommt.

Merksatz: **Die Welt meint es ernst. Deshalb ist sie komisch.** Kein Gegenstand zwinkert, keine Figur macht Faxen, kein Zug hat ein Gesicht.

### Was „bubbly“ konkret heißt (Maßstab: Clash of Clans, nicht Fotorealismus)

- **Formen:** übertrieben rund, weich, gestaucht-knuffig. Dicke abgerundete Zugfronten, große Fenster, gerundete Bahnsteigkanten, Signale mit großem Kopf. Nie kindisch: **keine Gesichter auf Zügen.**
- **Oberflächen:** satte, glatte Farbflächen mit weichem Verlauf, fast keine Textur. Schotter = knubbelige Kieselfläche, Holz = zwei, drei gemalte Maserungsstriche.
- **Licht:** warmes Hauptlicht von oben links, weiche Schatten, leichte Randaufhellung an Silhouetten. Alles wirkt plastisch und greifbar.
- **Bewegung:** alles federt. Knöpfe quetschen sich beim Drücken (Skalierung 0,92 → 1,04 → 1,0 in 0,18 s), Gleise ploppen mit Staubwölkchen ein, Züge wippen beim Anfahren. Reaktionen 0,1–0,3 s, damit das Denken nicht gebremst wird.
- **Figuren:** kleine rundliche Pendler ohne Detailgesicht (zwei Punktaugen), in Gruppen. Sie schauen aufs Handy, seufzen, gucken zur Tafel, jubeln.
- **Verboten:** Fotorealismus, KI-generierte Bilder, Texturen aus Bildgeneratoren, gestreckte Illustrationen, sich überlappende Objekte, falsche Maßstäbe.

## Voraussetzungen

- Repository `Al1l1l1l1l1/Stammstrecken-Puzzle`, Ausgangsbranch `rework/manus-rundum-rework`. Dort liegt bereits der Rework-Stand: überarbeitete 720 Kampagnentexte, blaue Kopftafel mit Feldgröße, Rundenreset mit Rückfrage, Saisonende-Logik, Endlos-„Weiter“, Sternzeit-Schätzung, Einstiegsmail, Generator path-search-v2, 29 Kosmetik-Items inklusive Waggons und Münchner U-Bahn.
- Unity **6000.3.23f1**, C#, URP 17.3, UI Toolkit (UXML/USS), Linear Color Space. Assembly `STP.Presentation.UI` kompiliert mit `-nullable:enable -warnaserror+` (jede Warnung ist ein Fehler).
- Kein lokales Unity nötig. Gebaut und getestet wird über GitHub Actions (siehe „Tests“).
- Parallel läuft [`WP-048`](WP-048_Satire-Inhalte-Stimmen-Klang-und-Einstieg.md) (Texte, Mails, Gags, Klang). Die Dateigrenze zwischen beiden Paketen ist unten unter „Ausdrücklich nicht erlaubte Änderungen“ festgelegt. Danach kommt [`WP-049`](WP-049_Abschluss-Sternzeiten-Geraet-und-Aufraeumen.md).

### Arbeitsweise (gilt verbindlich)

- **Ein Agent, ein Goal-Mode-Lauf, das ganze Paket.** Nicht in Etappen abgeben, nicht nach Teil A stoppen und fragen. Nicht „ein Level als Prototyp“ – ein einzelnes Level kann niemals das Spiel repräsentieren. Es zählt nur der vollständige Zusammenhang.
- **Keine Review-Schleifen.** Kein „QC-Bericht“, kein Gutachten, keine neuen Prüf-, Governance- oder Statusdokumente, keine Evidenzordner, keine Teilbriefe. Der Owner ist Solo-Unternehmer, kein Konzern. Die einzige Prüfung ist deine eigene Durchsicht am Ende (siehe Definition of Done) plus CI.
- **Keine Rückfragen** zu Dingen, die dieses Paket, die Bibel oder das Konzept beantworten. Wenn etwas wirklich offen ist: die Lösung wählen, die die Bibel am besten trifft, und in der Abschlussmeldung in einem Satz nennen.
- **Branch:** Lege von `rework/manus-rundum-rework` den Branch `rework/wp-047-welt` an. Arbeite dort. Am Ende mergst du **selbst** zurück nach `rework/manus-rundum-rework` (Ablauf unter „Definition of Done“). Der Owner merged nicht.
- **Lock-Kette nur einmal am Ende:** `python3 tools/rework/rebind_all.py` (siehe „Tests“). Nicht nach jeder Änderung.

## Scope

Dieses Paket baut die **gesamte sichtbare Spielwelt und alle Bildschirme** neu. Spielregeln, Solver, Speicherstände, Fortschritt, Ökonomie, Werbung, Käufe und alle Ablauf-Entscheidungen bleiben exakt erhalten – nur die Darstellung und Inszenierung darüber wird ausgetauscht.

### A. Technische Grundentscheidung (verbindlich)

**A1. Echte Unity-3D-Szene statt Software-Mesh.**

Heute erzeugt `Scripts/Presentation/UI/World3D/` (`StpWorld3D.cs`, `StpWorldKit.cs`, `StpBoardWorld.cs`, zusammen ca. 1 385 Zeilen) 3D-Geometrie in Software und malt sie als flaches UI-Mesh in UI Toolkit. Das ist eine Übergangslösung: kein echtes Licht, keine Kamerafahrt, keine Animation, keine Tiefe. **Das Endziel ist eine echte Unity-3D-Szene.**

- Lege einen echten 3D-Renderer an: In `Assets/StammstreckenPuzzle/Settings/StpUniversalRP.asset` gibt es heute nur `StpRenderer2D.asset` (`m_RendererDataList` mit einem Eintrag). Füge einen **Universal (Forward) Renderer** `StpRendererForward.asset` hinzu und mache ihn zum Standard, oder ersetze den 2D-Renderer, wenn nichts mehr davon abhängt. Was stabil läuft, entscheidest du.
- Eine perspektivische Kamera `StpWorldCamera` rendert die 3D-Welt. UI Toolkit liegt als Overlay darüber (`MobilePreviewPanelSettings.asset` hat bereits `m_ClearColor: 0`; der UI-Hintergrund des Wurzelelements `production-root` muss **transparent** werden, sonst sieht man die Welt nicht).
- Shading: **ein** gemeinsames Toon-/Gradient-Material mit Vertexfarben (URP Lit mit Vertex-Color oder ein eigener kleiner Shader Graph / HLSL-Shader). Ein Hauptlicht (warm, von oben links), weiche Schatten, Randlicht (Fresnel). Optional ein dezenter Outline-Pass. Keine Texturen außer einem kleinen Verlaufs-Lookup.
- Für die kleinen Vorschauen (Drehscheibe im Betriebshof, Fahrzeugkarten) darf eine RenderTexture genutzt werden.

**A2. Ein Baukasten, ein Raster, eine Palette.**

- Die Geometrie-Bausteine in `World3D/` (`StpBuilder`, `StpKit.Loco`, `StpKit.Wagon`, `StpKit.TrackBallast`, `StpKit.TrackRails`, `StpKit.Module`, `StpKit.House` usw.) sind gut. Wandle ihre Dreiecke in echte `UnityEngine.Mesh`-Objekte um (Vertex, Normal, Vertexfarbe) und cache sie. Alternativ dürfen Modelle in Blender gebaut und als FBX eingecheckt werden. **Keine KI-Bilder, keine KI-Texturen.**
- **1 Einheit = 1 Rätselfeld.** Alle Teile (Gleissegmente, Bahnsteigmodule, Gebäudeblöcke, Fahrzeugteile, Requisiten, Figuren) folgen diesem Raster, derselben Lichtsetzung und derselben Palette. Dadurch können Überlappungen und falsche Maßstäbe wie beim alten Betriebshof-Bild gar nicht mehr entstehen.
- Palette (aus der Bibel Kapitel 12, nicht erfinden): Tafelblau `#0F2A57` / `#0F4BA0`, Störungsorange `#FF6A13`, Warnorange `#F39200`, Verkehrsrot `#E0243C`, dazu Schotter-, Holz- und Stahltöne aus `StpKit`.
- Code-Ort: Neue 3D-Szenenlogik gehört nach `Assets/StammstreckenPuzzle/Scripts/Presentation/World/` (Assembly `STP.Presentation.World`, darf `UnityEngine` und URP referenzieren). Die Baukasten-Bausteine dürfen dorthin umziehen. Die Verdrahtung mit dem Presenter (`ProductionGamePresenter`) passiert im Composition Root `Scripts/Bootstrap/MobilePreviewInstaller.cs` (Assembly `STP.Bootstrap` referenziert alles). Wenn du `STP.Presentation.UI` → `STP.Presentation.World` referenzieren musst: erlaubt; passe dann `ARCHITECTURE/MODULE_BOUNDARIES.md` (Zeilen 80–84) und, falls der Validator meckert, die Prüfung in `tools/architecture-validation/validate.py` an.

**A3. Grafikloses CI darf nicht brechen.**

CI startet Unity mit `-batchmode -nographics`. Dort gibt es kein Grafikgerät. Muster, das im Repo schon benutzt wird (z. B. `MobilePreviewInstaller.cs` Zeile 314):

```csharp
output.enabled = SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null;
```

Ohne Grafikgerät wird die 3D-Szene **nicht aufgebaut** (keine Meshes, keine RenderTextures, keine Kamera aktiv), UI und Spiellogik laufen weiter, alle Tests bleiben grün. Keine Exception, kein `Debug.LogError` (manche Tests werten Log-Fehler als Fehlschlag).

**A4. Leistung.**

Ziel 60 fps auf einem Mittelklasse-Android. Einfache Geometrie (Low-Poly mit runden Kanten), statisches Batching oder `Graphics.RenderMeshInstanced` für Schotter, Schwellen, Pendler. Ein Echtzeitlicht. Bei 9×9 und 10×10 Gleisdetails reduzieren (weniger Schwellen, keine Kiesel). Schatten nur vom Hauptlicht.

**A5. Die Build-Szene.**

Die Android-Szene `Scenes/QA/MobilePreview.unity` wird **nicht von Hand gepflegt**, sondern von `Scripts/Editor/Build/MobilePreviewBuild.cs` erzeugt (lädt TextAssets aus `UI/` und verdrahtet den Installer). Neue Objekte (Kamera, Licht, Welt-Wurzel) erzeugst du dort oder zur Laufzeit im Installer – nicht durch Handbearbeitung der `.unity`-Datei.

### B. Das Rätsel: die Baustelle

Der wichtigste Bildschirm. **Die Bedienung bleibt exakt gleich**, nur das Aussehen ändert sich.

**B1. Kamera:** fast senkrecht von oben, um 10–15° geneigt. Das Raster ist vollständig sichtbar und auf einem 6,1-Zoll-Handy bis 10×10 gut lesbar: Zellen ≥ 28 px bei 10×10, ≥ 45 px bei kleineren Feldern. Das Raster verschiebt sich nie, auch nicht beim Antippen. Der bestehende Zoom (`puzzle-zoom`) bleibt.

**B2. Baufeld:** eine knubbelige Schotterplatte mit erhöhtem, gerundetem Rand und feinen Rasterfugen. A und B sind kleine **Tunnelportale bzw. Bahnsteigenden** am Rand, mit „A“ und „B“ auf kleinen Schildern. Die Randzahlen stehen auf **kleinen Schildern** am Rand; ist eine Reihe genau erfüllt → Schild wird grau; überschritten → Schild blinkt kurz rot (0,3 s) und bleibt rot umrandet.

**B3. Gleise müssen wie Gleise aussehen** (Alex' Kritik wörtlich: „Schienen sehen nicht wie Schienen aus“):

- Jede der sechs Formen (`TRACK_NS`, `TRACK_EW`, `TRACK_NE`, `TRACK_ES`, `TRACK_SW`, `TRACK_WN`) als echtes 3D-Gleis: Schotterbett, **braune Holzschwellen**, **zwei glänzende Stahlschienen**, saubere Kurven mit gleichmäßigem Radius, Schwellen radial in der Kurve. Anschlüsse an Nachbarfeldern stoßen nahtlos aneinander.
- **Grau (Notiz „hier kommt ein Gleis hin“)** = nur graue Schwellen, **ohne Schienen**, leicht transparent.
- **X (Notiz „hier kommt sicher nichts hin“)** = ein **gesprühtes rotes Kreuz** auf dem Schotter, leicht unregelmäßig wie mit der Dose.
- **Gelb** = ausschließlich die Auswahl (gelber Rand um die gewählte Zelle), unabhängig davon, was in der Zelle liegt. Gelb ist nie eine Gleisform.
- Setzen eines Gleises: Es „ploppt“ von oben ein (0,15 s, Überschwingen 1,08), kleines Staubwölkchen. Entfernen: kurz zusammenschrumpfen.

**B4. Umgebung außerhalb des Rasters:** dezente Baustelle (Bauzaun, Baubude, Bagger, Absperrbaken) und das **Baustellenschild der zweiten Stammstrecke** mit durchgestrichenen Fertigstellungsjahren und dem aktuellen Jahr (siehe E3). **Während des Lösens bewegt sich auf und neben dem Brett nichts**, was vom Denken ablenkt. Gags passieren nur in Menüs und nach dem Lösen.

**B5. Kopftafel (oben, blau).** Inhaltlich existiert sie bereits (`puzzle-operating-mark`, `puzzle-context`, `puzzle-title`, `puzzle-time`, `puzzle-phase`). Sie wird zu einer **echten Abfahrtstafel** mit Volumen (Gehäuse, leichter Glanz, LED-/Fallblatt-Anmutung):

- **links:** Linie und Meldung, z. B. „S1 · 07“
- **Mitte:** die Tafelmeldung in **einer** Zeile (Lauftext, wenn zu lang) und darunter groß die **Feldgröße „6×6“** – das hat Alex ausdrücklich verlangt.
- **rechts:** die Bauzeit und die drei Sternziele als Abfahrtszeiten (z. B. „0:45 · 1:30 · 3:00“); liegt man über dem nächsten Ziel, erscheint daneben die Verspätung in Orange: „+0:08“.
- Farbe: Tafelblau, weiße Schrift, Verspätung Orange. **Nie grau, nie weiß, nie ein anderer Ton.**

**B6. Gleiskiste (unten).** Heute ist das `tool-dock` mit `tool-0` … `tool-8`. Neu: eine **Holzkiste** mit den sechs Gleisstücken als kleine 3D-Stücke, daneben die **Sprühdose** (X), ein **Bündel grauer Schwellen** (Notiz) und der **Radiergummi/Schaufel** („leer“, entfernt ein belegtes Feld). Das aktive Werkzeug ist gelb umrandet und leicht angehoben (+6 px, Schatten größer). Die Element-Namen `tool-0` … `tool-8` und ihre Bedeutung bleiben, damit Tests und Bedienung gleich bleiben.

Seitlich drei Objektknöpfe mit Volumen:

| Bestehendes Element | Neues Aussehen | Verhalten |
|---|---|---|
| `puzzle-undo` | kleiner Hebel / Rückspul-Knopf | unverändert |
| `puzzle-hint` | **Bahnsteig-Lautsprecher** mit kleiner Zahl der Gratis-Hinweise | öffnet `hint-overlay` als **Durchsage** (Gong, Knistern, Text im Störungsbanner-Stil); Logik unverändert |
| `puzzle-reset` | Bauplan-Rolle „Neu planen“ | öffnet die bestehende Rückfrage `reset-round-overlay` („Bau verzögert sich um zwei Jahre“), Bauzeit läuft weiter |

**B7. Bedienung bleibt exakt:** Antippen setzt das aktive Werkzeug, erneutes Antippen derselben Belegung leert das Feld, Mehrfachauswahl, Zoom, Rückgängig. Nichts davon wird geändert oder „verbessert“. Die bestehenden PlayMode-Tests in `Tests/PlayMode/Presentation/ProductionGamePresentationTests.cs` (60 Elementabfragen) müssen weiter grün sein. Wenn ein Element nur noch unsichtbar existiert, ist das zulässig; gelöschte Elemente erfordern angepasste Tests.

### C. Lösungsmoment, Fahrt und Ergebnis (der Dopamin-Moment)

Das ist der Moment, für den man spielt. Er muss sich anfühlen wie **das erste Mal seit Jahren, dass ein Zug pünktlich kommt.**

1. **Einrasten:** Die Lösung rastet ein, Notizen (grau, X) verschwinden mit kurzem Puff, die Gleise **blitzen nacheinander von A nach B auf** (je Feld 30–50 ms, ein heller Glanz läuft die Strecke entlang). Klang `RouteReveal`.
2. **Kamera kippt** in 0,6 s von 10–15° auf ca. 40° in die Szene. An A und B öffnen sich Tunnelportal bzw. Bahnsteig, dort stehen **wartende Pendler** (die schon auf die Uhr schauen).
3. **Gong + Durchsage** (Text aus `UI/Satire/voices.json` → `arrivalAnnouncements`, z. B. „Ihr Zug fährt ein. Ja, wirklich.“) erscheint als weiß-oranges Störungsbanner. Dann fährt **der eigene Zug mit den gekauften Waggons und der gewählten Lackierung** die gebaute Strecke ab. Der Pfad kommt aus `Scripts/Presentation/World/ProductionTrainRide.cs` (`X`, `Y`, `AngleDegrees`, `Tick`, `IsFinished`). **Waggons folgen exakt dem Gleis** (eigene Bogenlängen-Verfolgung hinter der Lok, Abstand = Wagenlänge + Kupplung), auch in Kurven. Die Tauben auf der Oberleitung fliegen erschrocken auf. Die Pendler an B **applaudieren ungläubig** (zögern, schauen sich an, dann dünner Applaus). Dauer 3–5 s, **per Tap überspringbar**.
4. **Seltener Gag (ca. 1 von 15 Fahrten):** Der Zug fährt in **umgekehrter Wagenreihung** (Waggons vorn, Lok hinten schiebend); die Tafel meldet „Heute in umgekehrter Wagenreihung.“ Rein optisch.
5. **Tafel klappt** im Fallblatt-Effekt auf „Strecke freigegeben“.
6. **Ergebnis = Pünktlichkeitsnachweis.** Heute `result-panel` mit `result-art`, `result-mode`, `result-copy`, `result-stars`, `result-star-targets`, `result-time`, `result-progress`, `result-balance`, `result-bonus-state`, `result-video`, `result-skip-bonus`, `result-season-end`, `result-next`, `result-endless`, `result-home`. Neu: ein **Amtsformular aus Papier**, das von unten hereinfährt (leicht schräg, Papierschatten, Lochrand, Aktenzeichen oben rechts):
   - groß die Bauzeit, darunter „Neue Bestzeit“, wenn zutreffend;
   - die Sterne als **Stempel**, die nacheinander aufs Papier knallen (Skalierung 1,6 → 0,95 → 1,0, kleiner Wackler, Klang Stempel), jeder Stempel mit seinem Wort: ★ „Angekommen“, ★★ „Anschluss erwischt“, ★★★ „Tatsächlich pünktlich“;
   - bei verfehlten Sternen der Abstand: „Noch 0:08 bis ★★★“;
   - die Ergebnismeldung des Rätsels (Kampagnentext `result`) als Bemerkungszeile im Formular;
   - die verdienten Geduldspunkte **fliegen als Münzen ins Messing-Zählwerk** oben rechts (das Zählwerk dreht hoch wie ein mechanischer Zähler);
   - Hauptknopf **„Nächste Störung“** (`result-next`): öffnet sofort das nächste Rätsel per Kamerafahrt (Kamera fährt über die Welt zur nächsten Baustelle);
   - Zweitknopf **„Zum Stellwerk“** (`result-home`): führt **tatsächlich zur Karte** (Stellwerkspult), nicht zum Hauptmenü;
   - kleiner Knopf **„Nachweis teilen“** (neu): rendert den Nachweis als PNG (inklusive Stempeln, Bauzeit, Feldgröße, Linie und dem Satz „Hiermit wird bestätigt, dass die Verbindung von A nach B pünktlich war. Der Vorfall wird untersucht.“) und öffnet das System-Teilen-Menü. Auf Android über einen `Intent.ACTION_SEND` per `AndroidJavaObject` mit einem `FileProvider`-Eintrag; ohne Grafikgerät oder ohne Plattform-Unterstützung wird der Knopf ausgeblendet. Keine Drittanbieter-Plugins, keine neue Netzwerk- oder Tracking-Funktion;
   - ganz unten, **deutlich getrennt** (eigene Leiste, kleiner): „Zusätzliche Geduld beantragen“ (`result-video`, freiwilliges Video, bestehende Logik) und `result-skip-bonus`.
7. **Bestehende Ablauflogik bleibt unverändert:**
   - Letztes Kampagnenrätsel → statt „Nächste Störung“ ist der Knopf **ausgegraut** mit Hinweis auf den freien Modus (`result-season-end`), danach das Saisonende (siehe D). **Nie** startet das letzte Level erneut.
   - Freier Modus (Dauerbaustelle): „Weiter“ (`result-endless`) startet **direkt die nächste Runde** im gleichen Profil, nicht den Levelbildschirm.

### D. Alle Bildschirme (Konzept 2.1, Abschnitt 9)

Der Presenter kennt heute `enum Screen { Home, Map, Puzzle, Depot, Profile }` plus Overlays (`endless-overlay`, `daily-overlay`, `settings-overlay`, `notices-overlay`, `startup-overlay`, `reset-round-overlay`, `hint-overlay`, `notice-overlay`, `error-overlay`, `local-data-overlay`). **Jeder** davon wird eine Szene mit Gegenständen. Die Tabelle ist verbindlich; „bestehende Elemente“ heißt: diese Namen bleiben erhalten (Tests und Logik hängen daran), ihr Aussehen ändert sich.

| Bildschirm | Szene (was man sieht) | Hauptaktion (auffälligstes Objekt) | Weitere Ziele als Gegenstände | Bestehende Elemente |
|---|---|---|---|---|
| **Erster Start** (Konzept 9.1) | Baubüro: Schreibtisch, Diensttablet, Kaffeetasse, Fenster auf die Baustelle mit dem Baustellenschild. | Großer Knopf **„Auftrag annehmen“** auf dem Tablet → Kamerafahrt durchs Fenster hinunter auf die erste Baustelle. | Die Einstiegsmail ist auf dem Tablet geöffnet (Text aus `UI/Satire/mails.json`, Trigger `FIRST_START`). | `home-mail`, `home-play` (beim ersten Start), `startup-overlay` |
| **Geführter Einstieg** (9.2, Rätsel 1–5) | normale Baustelle | – | Pro Rätsel **ein** neues Element mit **einem** Satz im weiß-orangen Banner (Texte aus `UI/Satire/voices.json` → `tutorial`): 1 A/B + Gleise setzen, 2 Randzahlen, 3 X, 4 graue Schwellen („Notizen kosten keine Sterne“), 5 Uhr und Sterne. Das Banner verschwindet, **sobald die Handlung einmal ausgeführt wurde** (nicht per Wegklicken). Danach greift das Spiel nie mehr ein. | `puzzle-phase` oder neues `tutorial-banner` |
| **Hauptmenü = Bahnsteig** (9.3) | Leicht schräg über dem lebendigen Bahnsteig der aktuellen Welt: Pendler, Tauben, Bahnsteiguhr, Baustellenschild, Welt-Gags. | Großer **roter Stellwerksknopf „Strecke freigeben“** (Pilzknopf mit Schutzring). | Große **blaue Abfahrtstafel** oben zeigt den nächsten Auftrag wie eine Abfahrt: „S1 · Meldung 07 · Weichenstörung Ostkreuz · 6×6“ plus Sternstand. **Tor** zum Betriebshof, **Tür** zum Stellwerk (Karte), **Briefkasten** (Dienstpostfach, rote Zahl ungelesener Mails), **Fallblatt-Tafel „Betriebslage des Tages“** (Tagesbonus), **Bauschild-Kiosk** (Dauerbaustelle und Betriebsrevision, erst nach Saisonende aktiv). Oben rechts **Messing-Zählwerk** mit Geduldspunkten. Unten schmale Symbolleiste: Bahnsteig, Betriebshof, Dienstausweis. | `home-play`, `home-balance`, `home-route`, `home-section`, `home-progress`, `home-map`, `home-daily`, `home-endless`, `home-revision`, `home-mail`, `nav-home`, `nav-depot`, `nav-profile`, `main-navigation` |
| **Karte = Stellwerkspult** (9.4) | Beleuchtetes Gleisbildpult in knubbeliger Comic-Form, fünf Pultbereiche = fünf Welten. | Die **blinkende rote** aktuelle Störung. | Gelöste Abschnitte leuchten (gelb/weiß), kommende sind dunkel. Antippen klappt eine kleine **Meldekarte** auf (Name, Feldgröße, Sterne, Knopf „Strecke freigeben“). Am Pultrand ein Klebezettel: „Bitte nichts anfassen, was funktioniert.“ Mit jedem gelösten Rätsel leuchtet ein Stück mehr – **das Netz wird sichtbar heil**. | `map-back`, `map-previous`, `map-next`, `map-title`, `map-nodes`, `map-art` |
| **Rätsel = Baustelle** (9.5) | siehe B | Gleiskiste | siehe B | `puzzle-*`, `tool-0..8` |
| **Lösung, Fahrt, Ergebnis** (9.6, 9.7) | siehe C | „Nächste Störung“ | siehe C | `result-*` |
| **Saisonende** (9.8) | Der eigene Zug fährt über das **ganze heile Netz**, alle Pendler aller Welten jubeln an den Bahnsteigen, Konfetti, alle Tafeln zeigen „pünktlich“. | – | Danach erscheinen nacheinander zwei Mails (Trigger `SEASON_END_PRESS`, dann `SEASON_END_LEAD` mit „P.S.: Ab morgen beginnen die Bauarbeiten für Season 2.“). „Nächste Störung“ ausgegraut: „Alle Störungen behoben. Vorerst.“ Zwei Wege öffnen sich sichtbar am Bahnsteig: Bauschild „Dauerbaustelle“ und Aktenschrank „Betriebsrevision“. | `result-season-end`, `home-endless`, `home-revision` |
| **Dauerbaustelle** (9.9, freier Modus) | Vier **Bauschilder** vor einer endlosen Baustelle = die vier Schwierigkeitsprofile. Das große Baustellenschild hier zeigt „Fertigstellung: nie“. | Profil-Schild antippen = Runde starten. | Profilwechsel jederzeit über kleinen Knopf. Nach jeder Runde startet „Weiter“ sofort die nächste Runde im gleichen Profil. | `endless-overlay`, `endless-profiles`, `endless-unlock`, `endless-status`, `endless-open-list`, `endless-abandon-*`, `endless-close` |
| **Betriebsrevision** | **Aktenschrank** im Stellwerk; jede Schublade ein Netzabschnitt, darin die gelösten Rätsel als Aktenkarten mit Sternstempel. | Aktenkarte wählen → Rätsel spielen, Sterne verbessern. | Nur nach Saisonende verfügbar (Logik existiert in `ProductionPostgamePanels.cs`). | `home-revision` und vorhandene Revisionselemente |
| **Betriebshof = Werkhalle** (9.10) | Knubbelige Werkhalle mit **Drehscheibe**; darauf groß der **aktuelle Zugverband** (Lok + bis zu 3 Waggons), schön ausgeleuchtet. **Gerd** steht daneben (rundliche Figur, Schraubenschlüssel, Kaffeebecher, keine Detailmimik). | Kaufen bzw. Ausrüsten des gezeigten Fahrzeugs (großer Knopf mit Preis in Geduldspunkten). | **Wischen dreht die Drehscheibe** zum nächsten Fahrzeug. Gesperrte Fahrzeuge stehen **unter einer Plane**, ein Preisschild hängt dran. Zwei Bereiche: „**Zugpark**“ (Loks, Triebzüge, Waggons, Lackierungen) und „**Betriebsverbesserungen**“ (Objekte, die danach **wirklich in der Welt stehen**). Waggons per Antippen ankuppeln, mit sichtbarer **Kupplungsanimation** (max. 3). Zu jedem Objekt eine Sprechblase von Gerd (Text aus `UI/Satire/voices.json` → `depotLines[<katalog-id>]`). | `depot-screen`, `depot-art`, `depot-current-name`, `depot-current-livery`, `depot-current-body`, `depot-catalog` |
| **Profil = Dienstausweis** (9.11) | Ein **Dienstausweis** (Plastikkarte mit Lanyard) auf einem Schreibtisch. | – | Platzhalterfoto (Silhouette), **Dienstgrad** (steigt mit Sternen; Liste in `voices.json` → `ranks`), Statistik, **Dienstjubiläen** (`voices.json` → `jubilees`), Zähler „Fertigstellung durch Sie verschoben: 14 Jahre“ (aus E3). Kleines Zahnrad → Einstellungen. | `profile-clears`, `profile-balance`, `profile-settings`, `profile-direct`, `profile-records` |
| **Einstellungen** | dürfen schlicht bleiben, aber im Bahnlook: Schalter als **Kippschalter am Stellwerk**, Hintergrund = Schaltschrank. Rechtstexte (Datenschutz, Lizenzen, Löschen) bleiben **sachlich lesbar**. | – | „Werbung entfernen“ heißt künftig „**Ruhebereich buchen**“ (Text liefert `WP-048`). | `settings-*`, `notices-*`, `local-data-*` |
| **Tagesbonus** | Fallblatt-Tafel „Betriebslage des Tages“, die beim Öffnen durch Meldungen klappert und auf der Belohnung stehen bleibt. | Freies Los. | Zusatzlos per freiwilligem Video, klar getrennt. | `daily-overlay`, `daily-free`, `daily-video`, `daily-close`, `daily-*` |
| **Laden / Fehler / Rückfragen** | Bahnhofsszene mit Durchsage: „Verbindung wird gesucht …“ mit wechselnden Ladesätzen (`voices.json` → `loading`, alle 2,5 s ein neuer Satz). | – | Alle Rückfragen (Reset, Löschen, Abbrechen) im **weiß-orangen Störungsbanner-Stil**, nie als graues System-Popup. | `startup-overlay`, `error-overlay`, `reset-round-overlay`, `reset-confirmation`, `notice-overlay` |

**Die sieben Regeln gegen den PDF-Look gelten auf jedem Bildschirm** (Bibel Kapitel 12.3, Konzept Abschnitt 3):

1. Keine Überschriften-Hierarchie aus Titel, Untertitel und Fließtext.
2. Außerhalb von Mails höchstens **eine Textzeile pro Gegenstand**.
3. Keine Listen aus Textknöpfen. Auswahl geschieht über Gegenstände.
4. Kein Bildschirm ohne Szene. **Weißer Leerraum als Hintergrund ist verboten.**
5. Jeder Knopf ist ein Objekt mit Volumen, Schatten und Drückzustand.
6. Jeder Bildschirm hat genau **eine** Hauptaktion, und sie ist das auffälligste Objekt.
7. Keine gestreckten Bilder. Nichts überlappt, was nicht überlappen soll. Alles hat denselben Maßstab.

**USS-Hinweise:** `UI/ProductionGame.uss` (278 Zeilen) enthält schon einen „bubbly“-Ansatz. Bubbly in UI Toolkit heißt: große `border-radius` (≥ 18 px), dicke untere Kante als Volumen (`border-bottom-width: 6px` in dunklerer Farbe), beim Drücken `translate: 0 4px` und untere Kante 2 px, weicher Schatten über ein zweites, versetztes, halbtransparentes Element dahinter (UI Toolkit hat keinen box-shadow), Verlauf über ein kleines Hintergrundbild aus Code (`Texture2D` 1×64, zur Laufzeit erzeugt). Schrift bleibt IBM Plex (liegt bereits unter `Art/`).

### E. Welt-Gags, Mails und Gedächtnis darstellen

**E1. Welt-Gags abspielen.** `WP-048` liefert `Assets/StammstreckenPuzzle/UI/Satire/world-gags.json` (Vertrag in `WP-048`, Abschnitt „Vertrag zwischen WP-047 und WP-048“). Dieses Paket baut die dort genannten Objekte und Animationen und spielt sie ab:

- `kind: LOOP` läuft dauerhaft in Schleife in der genannten Szene.
- `kind: RARE` passiert etwa **bei jedem zehnten Besuch** eines Menübildschirms (Zufall mit fester Mindestpause von 5 Besuchen).
- **Nie im Rätsel**, solange gelöst wird.
- Mindestens alle Gags mit `required: true` müssen sichtbar umgesetzt sein. Das sind mindestens: Baustellenschild mit durchgestrichenen Jahren und „?“-Zettel; Aufzug zu Gleis 2 „Außer Betrieb“ mit verstaubender Plakette; Bahnsteiguhr geht drei Minuten nach; Pendlergruppe läuft wegen geänderter Wagenreihung ans andere Bahnsteigende und zurück; SEV-Bus mit Schild „Ersatzhaltestelle“ und Pfeil ins Nichts; Tauben auf der Oberleitung; seltener Durchfahrer „Fährt heute ohne Halt“; Tafel springt +5 → +10 → +20 → „Zug fällt aus“ mit synchronem Seufzen; Durchsage nur Knistern, dann „Wir bitten um Verständnis.“
- Solange `WP-048` nicht gemergt ist: Lege die drei Dateien als **Platzhalter derselben Form** mit je zwei, drei Beispieleinträgen selbst an (siehe Vertrag). Bei der Zusammenführung gewinnt die Fassung von `WP-048`.

**E2. Dienstpostfach (Briefkasten).** Neuer Overlay `mail-overlay`: Liste der empfangenen Mails als **Briefumschläge** (ungelesen = Siegel rot), Antippen öffnet die Mail als **Dienstmail auf dem Tablet** (Absender, Betreff, bis zu vier Sätze, P.S.). Mails kommen aus `UI/Satire/mails.json`. Ein Trigger feuert genau einmal (Ausnahme `RANDOM_CIRCULAR`: höchstens eine pro Tag, Zufall aus den vorhandenen). Die Trigger-Ereignisse stammen aus vorhandenen Read Models (Zahl gelöster Rätsel, Sterne, Welt, Käufe, Reset-Bestätigung, Saisonende, Freischaltung Dauerbaustelle/Revision). **Kein Trigger verändert Spielstand, Währung oder Belohnungen.** Einzige Ausnahme: die Lackierung, die Gerd nach Rätsel 3 schenkt – dafür nutzt du den vorhandenen Belohnungs-/Kosmetik-Weg; gibt es keinen sicheren vorhandenen Weg, wird die Mail ohne Geschenk angezeigt und die Lücke in der Abschlussmeldung genannt.

**E3. Präsentationsgedächtnis (kein Spielstand).** Für reine Präsentationswerte legst du einen kleinen eigenen Speicher an: `StpPresentationMemory` (in `STP.Presentation.UI` oder `STP.Presentation.World`), gespeichert als JSON-Datei `Application.persistentDataPath/presentation-v1.json`. Inhalt:

```json
{ "version": 1, "deadlineShiftYears": 14, "readMailIds": ["mail-welcome"], "firedTriggers": ["FIRST_START"],
  "menuVisits": 37, "lastRareGagVisit": 31, "lastCircularDate": "2026-11-26", "tutorialStepsDone": [1,2,3] }
```

- `deadlineShiftYears` zählt **bei jedem bestätigten Rundenreset +2** (dauerhaft, über das ganze Spiel). Das Baustellenschild zeigt `2028 + deadlineShiftYears` als aktuelles Fertigstellungsjahr, darüber die vorherigen Jahre durchgestrichen. Der Dienstausweis zeigt „Fertigstellung durch Sie verschoben: {n} Jahre“.
- **Das ist ausdrücklich nicht Teil des Save-Vertrags** (`LocalSaveCodec`, `LocalProfileState` bleiben unberührt). Verlust dieser Datei ist harmlos.
- Die bestehende Funktion „Lokale Daten löschen“ (`settings-delete-local`, `startup-delete-local`, `error-delete-local`) muss diese Datei **mitlöschen**.
- Schreiben mit Atomarität (in `.tmp` schreiben, dann ersetzen). Fehler beim Lesen → mit Standardwerten weiter, nie abstürzen.

### F. Fahrzeuge und Sammlung sichtbar machen

- Alle **29 Katalog-Items** aus `Content/Catalogs/s1/cosmetics-v2.json` (7 `TRAIN_VARIANT`, 10 `LIVERY`, 9 `OPERATING_OBJECT`, 3 `WAGON`) bekommen ein 3D-Modell aus dem Baukasten. IDs (nicht umbenennen): `train-stammrot-default`, `train-ersatzgarnitur`, `train-anschlusswunder`, `train-u-bahn-muenchen`, `train-doppelstock-pendler`, `train-dampflok-sonderfahrt`, `train-rangierlok-notbetrieb`; `livery-betriebsrot`, `livery-baustellengelb`, `livery-pendlerblau`, `livery-ersatzgruen`, `livery-umleitungsorange`, `livery-sonderviolett`, `livery-bummlertuerkis`, `livery-nachtanthrazit`, `livery-kulanzrosa`, `livery-frischweiss`; `obj-wartebank-ohne-anschluss`, `obj-wegweiser-vielleicht`, `obj-absperrung-laufzeit-offen`, `obj-anzeiger-in-pruefung`, `obj-bahnsteig-provisorisch`, `obj-tunnelportal-wartung`, `obj-sev-bucht-geplant`, `obj-signal-freigegeben`, `obj-tafel-trotzdem-weiter`; `wagon-grossraum-sitzplatz-optional`, `wagon-bordbistro-kaffeemaschine-defekt`, `wagon-fahrradabteil-ueberfuellt`.
- **Fahrzeuge unterscheiden sich durch Form, nicht nur Farbe** (Alex: „die roten kann man kaum unterscheiden“). Mindestens: S-Bahn rund-rot-weiß, Doppelstock (zwei Fensterreihen, hoch), Münchner U-Bahn neue Generation (**hellblau-weiß, runde moderne Front, ohne Betreiberlogo**), Dampflok (Kessel, Schornstein, Dampfwölkchen), Rangierlok (kurz, Mittelführerstand, orange), Ersatzgarnitur (kantig, ältere Bauart, leicht verbeult), Anschlusswunder (glänzend, Sonderzug-Anmutung).
- **Lackierungen:** zehn klar unterscheidbare Töne, auf jedem Fahrzeug sauber an Front und Seite. Kein Ton darf mit einem anderen verwechselbar sein (Sichtprüfung nebeneinander auf der Drehscheibe).
- **Waggons:** max. 3, hängen bei jeder Fahrt und auf der Drehscheibe hinter der Lok. Kauf nur mit Geduldspunkten (bestehende Logik).
- **Betriebsverbesserungen** (`obj-*`) stehen nach dem Kauf **sichtbar in der Bahnsteig-Szene** an festen Plätzen (ein Platz je Objekt, keine Überlappung).

### G. Ablaufpfade, die stimmen müssen

Jeder Knopf führt dorthin, wohin sein Text es verspricht. Alex' Fehlerliste enthielt genau solche Fehler. Prüfe am Ende jeden Pfad:

| Von | Aktion | Muss führen zu |
|---|---|---|
| Erster Start | „Auftrag annehmen“ | Rätsel 1 (Kamerafahrt) |
| Bahnsteig | Stellwerksknopf | nächstes offenes Kampagnenrätsel |
| Bahnsteig | Tür Stellwerk / `home-map` | Karte |
| Bahnsteig | Tor / `nav-depot` | Betriebshof |
| Bahnsteig | Briefkasten | Dienstpostfach |
| Bahnsteig | Fallblatt-Tafel | Tagesbonus |
| Ergebnis | „Nächste Störung“ | nächstes Rätsel direkt |
| Ergebnis | „Zum Stellwerk“ | Karte |
| Ergebnis letztes Level | „Nächste Störung“ | **ausgegraut**, Hinweis freier Modus; danach Saisonende-Inszenierung; nie Neustart des Levels |
| Ergebnis Dauerbaustelle | „Weiter“ | nächste Runde gleiches Profil, direkt |
| Rätsel | „Neu planen“ → bestätigen | Raster leer, Bauzeit läuft weiter, Schild +2 Jahre |
| Rätsel | „Neu planen“ → abbrechen | nichts verändert |
| Rätsel | Zurück (`puzzle-back`) | dorthin, wo man herkam (Bahnsteig oder Karte) |
| Karte | Meldekarte → „Strecke freigeben“ | genau dieses Rätsel |

### H. Fallstricke (kennen die Vorgänger-Agents schon, bitte nicht wiederholen)

- **Nicht wörtlich nehmen.** „Dienstausweis“ heißt nicht: ein Formular mit der Überschrift „DIENSTAUSWEIS“. Es heißt: eine Karte, die aussieht wie eine Plastikkarte. „Stellwerkspult“ heißt nicht: eine Liste mit dem Titel „Stellwerk“. Siehe Bibel Kapitel 5.
- **Nicht Text auf Gegenstände kleben.** Eine Tafel, auf der „Abfahrtstafel“ steht, ist falsch. Eine Tafel, die wie eine Abfahrtstafel **aussieht** und eine Abfahrt **zeigt**, ist richtig.
- **Keine Witze erfinden, die nicht in der Bibel oder in `UI/Satire/*.json` stehen.** Du bist Regisseur, nicht Autor. Texte schreibt `WP-048`.
- **Nicht verniedlichen.** Keine Gesichter auf Zügen, kein Comic-„BOING“, keine Ausrufezeichen-Freude. Die Welt bleibt ernst, auch wenn sie knuffig aussieht.
- **Nicht die Spiellogik anfassen**, um die Optik einfacher zu machen. Wenn die Darstellung etwas braucht, das kein Read Model hat, lies es aus vorhandenen Daten oder speichere es im Präsentationsgedächtnis.
- **`-warnaserror+`:** ungenutzte Variablen, fehlende Nullable-Annotationen und veraltete APIs brechen den Build. Lieber einmal mehr lokal mit `dotnet build` gegen Stubs prüfen (siehe Tests), bevor du CI startest.
- **Locks:** Jede neue `.cs`-Datei unter `Scripts/Presentation/UI` oder `Scripts/Presentation/World` und jede neue Datei unter `Art/` (png/svg/ttf/otf) verändert den UI-Lock. Das korrigiert `tools/rework/rebind_all.py` am Ende automatisch (Dateianzahl, Hashes). Nicht von Hand Hashes jagen.
- **CI nie mehrfach starten.** Die Concurrency-Gruppe `unity-personal-seat` bricht Läufe gegenseitig ab.

## Betroffene Dateien/Module

Dieses Paket darf ändern:

- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/**` (Presenter, Grid, Ergebnis, Panels, World3D)
- `Assets/StammstreckenPuzzle/Scripts/Presentation/World/**` (neue 3D-Szene, Kamera, Baukasten, Fahrt)
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/MobilePreviewInstaller.cs` und weitere Bootstrap-Dateien, soweit für die Verdrahtung nötig
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/MobilePreviewBuild.cs` (Szenenaufbau, neue TextAssets `UI/Satire/*.json`)
- `Assets/StammstreckenPuzzle/UI/ProductionGame.uxml`, `UI/ProductionGame.uss`
- `Assets/StammstreckenPuzzle/UI/Satire/*.json` nur als **Platzhalter** (siehe E1)
- `Assets/StammstreckenPuzzle/Settings/**` (Forward-Renderer, URP-Asset), `Assets/StammstreckenPuzzle/Art/**` (Materialien, Shader, ggf. FBX), `Assets/StammstreckenPuzzle/Scenes/**` (nur über den Build-Generator)
- `Assets/StammstreckenPuzzle/Tests/PlayMode/**` und `Tests/EditMode/**`, soweit Tests an neue Darstellung angepasst werden müssen
- `ProjectSettings/*.asset` (Grafik-Einstellungen, falls für den Forward-Renderer nötig), `Assets/Plugins/Android/**` (FileProvider-Eintrag für „Nachweis teilen“)
- `ARCHITECTURE/MODULE_BOUNDARIES.md` und `tools/architecture-validation/validate.py`, nur falls eine neue Assembly-Referenz nötig ist
- Am Ende: alles, was `tools/rework/rebind_all.py` neu schreibt (Locks, Human-Packs, Hashes)

## Ausdrücklich nicht erlaubte Änderungen

- **Spielregeln, Solver, Generator, Rätseldaten:** `Scripts/Puzzle/**`, `Content/Levels/**`, `Content/Proofs/**`, `Content/Generator/**`, `UI/MobileCandidate.public.json` – nicht anfassen.
- **Spielstand und Ökonomie:** `Scripts/Infrastructure/Persistence/**`, `LocalSaveCodec`, `LocalProfileState`, Preise, Belohnungsmengen, Werbe- und Kauflogik (`Scripts/MobileServices/**`) – nicht anfassen.
- **Texte gehören `WP-048`:** `UI/ProductionGameLocale.json`, `UI/ProductionCampaignText.json`, `Content/Localization/**` nicht ändern. Neue sichtbare Texte holst du aus `UI/Satire/*.json` (siehe Vertrag in `WP-048`). Wenn ein Text fehlt: Platzhalter im selben JSON-Format anlegen, nicht in den C#-Code schreiben.
- **Klänge gehören `WP-048`:** `Content/Assets/build-product-audio.cjs`, `Assets/StammstreckenPuzzle/Audio/**` nicht ändern. Neue Klang-Aufrufe über die vorhandenen Cue-Namen in `Scripts/Audio/ProductionGameAudio.cs` (`ToolFocus`, `EmptyMark`, `OccupancyNote`, `TrackSeat`, `Clear`, `Undo`, `RouteReveal`, `TrainStart`, `TrainRoll`, `TrainArrive`, `ResultsOpen`, `CompletionClearance`, `SceneAmbience`, `WorkshopMusic`, `TrainCurve`) und die neuen Cue-Namen, die `WP-048` ergänzt (`Gong`, `Flap`, `Stamp`, `Applause`, `Sigh`, `Pigeons`, `DoorBeep`, `Brake`). Fehlt ein Cue noch, ruf ihn trotzdem über den Namen auf; `ProductionGameAudio` ignoriert unbekannte Namen.
- **Keine KI-generierten Bilder oder Texturen.** Keine Drittanbieter-Assets aus Asset Stores ohne CC0-Lizenz.
- **Kein Logo-Nebenschauplatz, kein Anwalts-Hinweis, keine Rechtsprüfungs-Dokumente.** Kein DB-Logo, keine Kopie realer App-Screens oder Baureihen-Fronten, keine Originalansagen.
- **Keine neuen Governance-, QC-, Evidenz- oder Statusdokumente**, keine neuen ADRs, keine neuen Scope-Manifeste. Das Manifest oben gilt für das gesamte Rework und darf **nie verändert** werden.
- Alte Konzeptdateien `Stammstrecken_Puzzle_Konzept_00-15/**` nicht ändern.

## Akzeptanzkriterien

1. **Kein Bildschirm sieht aus wie ein Dokument.** Jeder Bildschirm aus Tabelle D ist eine Szene mit Gegenständen und erfüllt die sieben Regeln gegen den PDF-Look.
2. **Echte 3D-Welt** mit perspektivischer Kamera, Licht, Schatten und Bewegung (kein UI-Mesh als Endzustand). Im CI ohne Grafikgerät läuft alles ohne Exception.
3. **Gleise sehen aus wie Gleise:** Schotter, braune Schwellen, zwei Stahlschienen, saubere Kurven. Grau = nur Schwellen, X = gesprühtes Kreuz, Gelb = nur Auswahl.
4. **Rätsel auf 4×4 und 10×10 gut lesbar** auf 1080×2400 (6,1 Zoll), Bedienung exakt wie vorher.
5. **Kopftafel blau** mit Linie, einer Zeile Meldung, großer **Feldgröße**, Bauzeit, Sternzielen und orangefarbener Verspätung.
6. **Lösungsmoment** mit Aufblitzen A→B, Kamerakippen, Durchsage, eigener Zugverband mit Waggons exakt auf dem Gleis, Tauben, ungläubigem Applaus, Überspringen per Tap.
7. **Pünktlichkeitsnachweis** mit Stempel-Sternen, Abstand zum nächsten Stern, Münzflug ins Zählwerk, funktionierendem „Nachweis teilen“ auf Android.
8. **Alle Pfade aus Tabelle G** führen dorthin, wohin ihr Text es verspricht.
9. **Welt-Gags** aus `UI/Satire/world-gags.json` mit `required: true` sind sichtbar; seltene Gags erscheinen selten; nie während des Lösens.
10. **Dienstpostfach** zeigt Mails nach Trigger; **Baustellenschild** zählt bei jedem bestätigten Reset dauerhaft +2 Jahre; „Lokale Daten löschen“ entfernt auch das Präsentationsgedächtnis.
11. **Alle 29 Katalog-Items** haben ein eigenes 3D-Modell; Fahrzeuge unterscheiden sich in der Form; die zehn Lackierungen sind nebeneinander klar unterscheidbar; die Münchner U-Bahn ist hellblau-weiß mit runder Front.
12. **Keine Überlappungen, keine gestreckten Bilder, keine KI-Bilder** im gesamten Spiel.
13. CI grün: Compile, EditMode, PlayMode, Android-APK, Architecture Validation. (iOS ist bekannt rot wegen Infrastruktur und wird in `WP-049` erledigt.)

## Tests

- **Lokal vor dem Push (Sandbox ohne Unity):**
  - `python3 tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-047.production.scope.json`
  - `python3 tools/ci/test_change_scope.py`
  - `python3 tools/rework/rebind_all.py --check` (meldet, ob Locks neu gebunden werden müssen; ca. 3–4 Minuten)
  - Optional C#-Vorprüfung ohne Unity: ein kleines `dotnet`-Projekt mit Stubs für die genutzten `UnityEngine`-Typen kompiliert die geänderten Dateien mit `-nullable:enable -warnaserror+`. Das fängt Tippfehler, bevor CI 45 Minuten läuft.
- **Lock-Kette einmal am Ende:** `python3 tools/rework/rebind_all.py` (schreibt Kampagnentext-Projektion, Text-Bibliothek, Mobile-Candidate-Projektion, Sample-Audit-Hashes, Human-Packs und Locks, Dateianzahl im UI-Lock in den vier Testdateien und den Public-Hash). Danach `python3 tools/rework/rebind_all.py --check` muss PASS melden. Nur falls du Dateien unter `Tests/PlayMode/Bootstrap/RuntimeFixtures/` änderst: dort `Runtime.bindings.json` von Hand nachziehen (Roh-SHA256 der Dateien; Kosmetik-Katalog-Hash ist der JCS-Hash = SHA256 über JSON mit sortierten Schlüsseln ohne Leerzeichen) und den neuen Roh-SHA256 von `Runtime.bindings.json` in `RuntimeGameIntegrationTests.cs` (Zeile ~30) und `LocalProfileDeletionIntegrationTests.cs` (Zeile ~176) eintragen.
- **CI:** Push startet CI, überspringt Unity aber oft („source unchanged“). Für einen vollen Lauf **genau einmal** `gh workflow run unity.yml --ref rework/wp-047-welt`, dann warten (ca. 45–60 min). Nicht in Schleife, nicht mehrfach. Das APK-Artefakt heißt `android-development-il2cpp`.
- **PlayMode-Tests** für: Präsentationsgedächtnis (Reset +2, Löschen entfernt Datei, kaputte Datei → Standardwerte), Mail-Trigger feuern genau einmal, letztes Level → ausgegraut, Dauerbaustelle „Weiter“ → nächste Runde, „Zum Stellwerk“ → Karte, 3D-Welt wird ohne Grafikgerät nicht aufgebaut und wirft nicht.
- **Eigene Sichtprüfung** (kein Review durch andere): Im Editor bzw. auf dem APK einmal komplett durchklicken von „Auftrag annehmen“ bis Saisonende (Kampagne darf per vorhandener Debug-Freischaltung übersprungen werden), danach Dauerbaustelle und Betriebsrevision. Für jeden Bildschirm den Pendler-Test aus der Bibel Kapitel 3 im Kopf stellen.

## Risikoklasse

**Hoch** (größter Umbau der Darstellung, Renderer-Wechsel, viele Dateien). Gegenmittel: Spiellogik bleibt unberührt, grafikloser Fallback, bestehende Element-Namen bleiben, Lock-Kette per Skript. Kein zusätzliches Review nötig.

## Definition of Done

1. Alle Akzeptanzkriterien erfüllt.
2. `python3 tools/rework/rebind_all.py --check` → PASS; lokale Validatoren grün.
3. Genau ein voller CI-Lauf auf `rework/wp-047-welt` mit grünem Compile, EditMode, PlayMode, Android-APK und Architecture Validation.
4. **Selbst mergen:**
   1. `git fetch origin` und `git merge origin/rework/manus-rundum-rework` in deinen Branch (falls `WP-048` schon gemergt ist).
   2. Konflikte lösen: Bei `Assets/StammstreckenPuzzle/UI/Satire/*.json` gewinnt **immer die Fassung von `WP-048`** (`git checkout --theirs`). Bei Lock-Dateien und Human-Packs nicht von Hand mischen, sondern danach `python3 tools/rework/rebind_all.py` laufen lassen.
   3. Falls nach dem Merge etwas neu gebunden wurde: Push, **ein** CI-Lauf, grün abwarten.
   4. `git checkout rework/manus-rundum-rework && git merge --no-ff rework/wp-047-welt && git push origin rework/manus-rundum-rework`.
5. `Bearbeitungsstatus` oben in dieser Datei auf „Abgeschlossen“ setzen (mit Datum und Merge-Commit) und in `PROJECT_CONTROL/WORK_QUEUE.md` die Zeile von `WP-047` auf „Abgeschlossen“ stellen. Sonst keine Dokumente.
6. **Abschlussmeldung an Alex auf Deutsch, höchstens zehn Zeilen, ohne Fachjargon:** was jetzt anders aussieht, Link zum APK-Artefakt des grünen CI-Laufs, was bewusst offen ist (in einem Satz je Punkt). Keine Datei anhängen.
