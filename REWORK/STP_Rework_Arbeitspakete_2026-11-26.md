# Stammstrecken-Puzzle – Rework-Arbeitspakete für Umsetzungs-Agents

Stand 26.11.2026 · Verfasser: Manus · Auftraggeber: Alex (Solo-Unternehmer)
Repo `Al1l1l1l1l1/Stammstrecken-Puzzle`, Ausgangsbranch `rework/manus-rundum-rework`, Commit `9c4fb29`.
Maßgebliches Konzept: `REWORK/Konzept_2.1_Rundum_Rework.md` (liegt neben dieser Datei). **Diese Datei ist die einzige Arbeitsanweisung. Sie ersetzt für das Rework alle WP-, Mandats-, QC- und Governance-Dokumente in `PROJECT_CONTROL/` und `WORK_PACKAGES/`.**

Lies zuerst Teil 0 bis 2 vollständig. Danach nur dein eigenes Paket.

---

## Teil 0 – Arbeitsweise (gilt für jedes Paket)

1. **Du baust ein Spiel, kein Prüfsystem.** Keine neuen Gates, Locks, Hashketten, Evidenzdateien, Review-Berichte, QC-Runden, Abnahmeprotokolle oder Mandatsdokumente. Kein „Entwurf zur Prüfung“, kein „wartet auf Freigabe“. Du entscheidest innerhalb dieser Vorgaben selbst und setzt um.
2. **Ein Paket = ein durchgehender Goal-Mode-Lauf bis zur Definition of Done.** Nicht nach Teilschritten anhalten, nicht nachfragen, wie etwas gemeint ist, wenn diese Datei es beantwortet. Rückfrage an Alex nur, wenn etwas Geld kostet oder unumkehrbar ist.
3. **Prüfung ist schlank:** CI grün (Compile, EditMode, PlayMode, Android-APK) + deine eigene Durchsicht mit den Checklisten in deinem Paket. Kein zweiter Agent prüft dich. Alex testet danach das APK und meldet Fehler, die dann direkt behoben werden.
4. **Bestehende Tests:** Wenn ein Test nur ein altes Layout, alte Elementnamen, alte Texte oder alte Hashes festschreibt, passe ihn an oder lösche ihn. Tests für Rätselregeln, Solver, Speicherstand, Währung, Käufe, Werbung und Fortschritt bleiben und müssen grün bleiben.
5. **Locks und Hashketten** (siehe Teil 3) nur einmal am Ende deines Pakets neu binden, nicht nach jeder Änderung.
6. **Abschluss:** Commit und Push auf deinen Paketbranch, ein CI-Lauf, eine kurze Abschlussmeldung auf Deutsch (was gebaut wurde, was nicht, Link zum APK-Artefakt). Keine weiteren Berichtsdateien.

## Teil 1 – Was bleibt unverändert (nicht anfassen)

- **Rätselregeln:** Raster 4×4 bis 10×10, Randzahlen, Ein-/Ausgang A und B, sechs Gleisformen (gerade senkrecht/waagrecht, vier Kurven), rotes X und graue Gleis-Notiz als **freiwillige** Hilfen, Leeren durch erneutes Antippen, Mehrfachauswahl, Raster verschiebt sich nie.
- **Rätselinhalte:** die 240 Kampagnenrätsel und der Endlosgenerator `path-search-v2` sind fertig.
- **Spielsysteme:** Solver, Speicherstand, Geduldspunkte, Käufe, freiwillige Videos, Werbefrei-Kauf, Tagesbonus, Fortschritt, Sterne-Logik, Endlosmodus-Profile, Betriebsrevision.
- **Fairness:** keine Leben, keine Wartezeiten, keine Strafen, keine Sonderfelder. Kein Witz verändert eine Regel oder kostet den Spieler etwas.
- **Fahrzeugkatalog:** 7 Züge, 10 Lackierungen, 9 Betriebsobjekte, 3 Waggons (`Content/Catalogs/s1/cosmetics-v2.json`). Waggons nur gegen Geduldspunkte.
- **Ansprache:** durchgehend „Sie“.

---

## Teil 2 – Der Humor-Kern (Pflichtlektüre, nicht wörtlich nehmen!)

### 2.1 Warum es dieses Spiel gibt

Das Spiel existiert, damit jeder, der in Deutschland regelmäßig Bahn fährt, es sieht und **sofort** weiß, worauf es anspielt – und nicht weiß, ob er lachen oder heulen soll. Das Rätsel ist das Handwerk, **die Satire ist das Produkt**. Ohne Satire ist es ein beliebiger Gleis-Puzzle-Klon. Mit ihr ist es ein Meme, das Pendler sich gegenseitig schicken.

Der Name spielt auf die Münchner S-Bahn-Stammstrecke an: der eine Tunnel, durch den fast alle S-Bahnen müssen und der deshalb ständig gestört ist. Die zweite Stammstrecke soll Abhilfe schaffen, war ursprünglich für 2026 angekündigt und ist inzwischen irgendwo bei 2035 bis 2037. Das ist der Ur-Witz des Spiels: **ein Projekt, das immer fertig wird – nur nie jetzt.**

Die Grundpointe in einem Satz: **Der Spieler schafft im Rätsel genau das, was das echte System nie schafft – eine funktionierende Verbindung von A nach B.**

### 2.2 Die Humorformel

Jeder Gag besteht aus drei Teilen:

1. **Eine echte, wiedererkennbare Störung**, die jeder Pendler selbst erlebt hat.
2. **Ruhige, freundliche, beschwichtigende Betriebssprache**, wie aus der Lautsprecherdurchsage.
3. **Die trockene Wahrheit dahinter**, die der Pendler sofort mitdenkt.

Beispiel: „Signalstörung. Das Signal zeigt Grün. Niemand traut sich.“ – (1) Signalstörung kennt jeder, (2) nüchterne Meldung, (3) die Wahrheit: Das System ist so kaputt, dass selbst ein funktionierendes Signal niemand glaubt.

**Der Ton ist trocken (deadpan), nie albern.** Das System nimmt sich selbst todernst und merkt nicht, wie absurd es ist. Das Spiel selbst zwinkert nur. Keine Ausrufezeichen-Comedy, keine Smileys, keine „Haha“-Wörter, kein Slapstick in Texten. Optisch darf die Welt knuffig und verspielt sein; die Sprache bleibt dienstlich-ernst. **Gerade dieser Kontrast ist der Witz.**

### 2.3 Der Pendler-Test (für jeden Text, jedes Objekt, jede Animation)

> Erkennt ein regelmäßiger Bahnfahrer **innerhalb von drei Sekunden**, worum es geht – und denkt an seinen eigenen Morgen auf dem Bahnsteig?

Ja → bleibt. Muss man überlegen, welcher Fachbegriff gemeint ist → raus. **Höchstens ein Fachbegriff pro Satz.** Die Pointe sitzt beim ersten Lesen.

### 2.4 Typische Fehler von Umsetzungs-Agents (genau so NICHT)

| Falsch (wörtlich genommen) | Richtig (gemeint) |
|---|---|
| Amtsbegriffe aneinanderreihen: „Betriebslage gemäß Störungsmanagement im Rahmen der Infrastrukturmaßnahme“ | Eine Situation, die man vor sich sieht: „Der Aufzug zu Gleis 2 ist außer Betrieb. Gleis 2 ebenfalls.“ |
| „Zurücksetzen verschiebt die Fertigstellung um zwei Jahre“ als echte Strafe (Zeitstrafe, Punktabzug, Sperre) | Reiner Gag: Ein Baustellenschild in der Welt zählt sichtbar zwei Jahre hoch. Spielerisch passiert nichts. Die Bauzeit läuft normal weiter. |
| „Betriebshof hat Personalmangel“ → neue Mechanik „Personal einstellen“ | Erzählerischer Grund, warum **Sie** auch noch den Fahrzeugladen (Betriebshof) machen. Gerd ist der einzige Mitarbeiter. Das ist die ganze Pointe. |
| „Verspätung“ → der Zug fährt im Spiel tatsächlich verspätet los, der Spieler muss warten | Verspätung ist nur Anzeige-Satire: Über dem Sternziel steht in Orange „+0:08“ wie auf der Abfahrtstafel. Der Spieler wartet nie. |
| „Wagenreihung geändert“ → Waggons kommen in falscher Reihenfolge an und das ist ein Fehler | Seltener optischer Gag, Tafel meldet „Heute in umgekehrter Wagenreihung“. Nichts ist kaputt. |
| „Schienenersatzverkehr“ in jedem zweiten Satz | Das Wort ist ein Trumpf. Sparsam an starken Stellen einsetzen (Welt 5, seltene Meldungen, Finale). Abnutzung tötet den Witz. |
| Bahnmitarbeiter als Idioten darstellen | **Der Witz zielt auf das System, nie auf Menschen.** Zugbegleiter, Gerd, Pendler sind Leidtragende – sympathisch, erschöpft, tapfer. |
| Pendler als dumme Opfer | Pendler sind abgebrühte Profis: Sie seufzen synchron, laufen routiniert zum anderen Bahnsteigende und glauben nichts mehr. |
| Ergebnistext spricht vom Bus, obwohl gerade ein Zug fährt | Text muss zur Situation passen. Ein Bus taucht nur beim SEV-Gag auf. |
| Politische Wut, echte Manager-Namen, Konzernlogo, echte Unglücke | Niemals. Keine echten Personen, kein DB-Logo, keine kopierten Original-Ansagen oder App-Screens, keine Unfälle, kein „Personen im Gleis“, keine Notarzteinsätze, keine Suizidbezüge. |
| Witz während des Lösens mitten aufs Brett legen | **Die Satire verdeckt nie das Rätsel.** Gags passieren am Rand, in Menüs und in Pausen. Während des Denkens bleibt das Baufeld ruhig. |

### 2.5 Die Lore – was Pendler wiedererkennen

Diese Begriffe sind frei verwendbar (sie gehören niemandem). Die rechte Spalte erklärt, **warum** sie wehtun – darauf beruht der Witz:

| Begriff / Situation | Warum jeder Pendler zuckt |
|---|---|
| Schienenersatzverkehr (SEV) | Bus statt Zug, dauert doppelt so lange, Haltestelle nicht auffindbar. Das meistgehasste Wort im Pendleralltag. |
| Signalstörung, Weichenstörung, Stellwerksstörung | Die drei Standardausreden, die nichts erklären. |
| „Verzögerungen im Betriebsablauf“ | Die Meldung, die alles bedeutet und nichts sagt. |
| „Wir bitten um Ihr Verständnis“ | Der Abschluss jeder Durchsage. Niemand hat Verständnis. |
| „Reparatur am Zug“, „Verspätung aus vorheriger Fahrt“ | Die Verspätung hat eine Verspätung als Ursache. |
| Geänderte / umgekehrte Wagenreihung | Alle stehen am falschen Bahnsteigende und rennen los. |
| Gleiswechsel in letzter Minute | Ganze Menschenmenge sprintet durch die Unterführung. |
| Zugausfall, „Zug endet heute hier“, „Halt entfällt“ | Man steht irgendwo, wo man nie hinwollte. |
| Anschluss verpasst / „Anschluss wird nicht abgewartet“ | Man sieht den Anschlusszug abfahren. |
| Aufzug / Rolltreppe außer Betrieb | Dauerzustand, mit Schild „Instandsetzung geplant“. |
| Bauarbeiten, Baustellenschild mit Jahreszahl | Fertigstellungstermine werden weitergeschoben. Zweite Stammstrecke: 2026 → 2028 → 2035 → 2037 … |
| Ersatzzug, Ersatzgarnitur, „heute mit kürzerem Zug“ | Zu wenig Platz, alle stehen. |
| Klimaanlage defekt, WC defekt, Bordrestaurant geschlossen | Komfortversprechen, die nie gelten. („Speisewagen. Heute ohne Speisen.“) |
| Pünktlichkeitsstatistik, „besonders ungünstiger Vergleichszeitraum“ | Schönrechnen. Paradesatz für die Pressestelle. |
| Fallblattanzeiger, blaue Abfahrtstafel, „+5 / +10 / fällt aus“ | Das ikonische Bild des Wartens: Die Zahl springt immer weiter. |
| Durchsage-Gong mit Knistern | Man versteht nur die Hälfte, die wichtige Hälfte nie. |

### 2.6 Die fünf Satire-Schichten (wo der Witz überall stecken muss)

Der alte Build hatte Satire fast nur als Textzeile über dem Raster. **Das ist der Hauptfehler.** Der Witz muss in alle fünf Schichten:

1. **Aufmachung:** Das Spiel sieht aus, als hätte der Bahn-Designer ein Handyspiel gebaut: tiefblaue Abfahrtstafel (#0F2A57 / #0F4BA0) mit weißer Schrift, weiß-oranges Störungsbanner, Verkehrsrot (#E0243C) für Hauptaktion und eigene Züge, Durchsage-Gong (eigener, nicht der Original-Dreiklang), Fallblatt-Klappern. Nah dran, als wäre es derselbe Designer, aber nichts kopiert.
2. **Welt:** Jede Szene ist ein Wimmelbild voller Alltagsgags (Liste in Paket 2, Abschnitt B).
3. **Systeme:** Spielmechaniken tragen Bahnnamen und Bahnlogik (Bauzeit statt Timer, Verspätung „+0:08“, Hinweis als Durchsage, Werbefrei als „Ruhebereich buchen“, Zurücksetzen verschiebt das Baustellenschild).
4. **Stimmen:** ein festes Ensemble aus vier Absendern mit eigenem Ton (Abschnitt 2.7).
5. **Momente:** große inszenierte Pointen: der erste pünktliche Zug, bei dem die Pendler ungläubig applaudieren; die erste Reset-Rückfrage; das Saisonfinale.

### 2.7 Das Ensemble (fiktiv, ohne Gesichter, nur Signatur und Ton)

| Absender | Ton | Running Gag | Beispielsatz |
|---|---|---|---|
| **Abteilungsleitung Stammstrecke** (Ihre Chefin) | überkorrekt, herzlich, leicht überfordert | Jede Mail endet mit einem „P.S.“ mit neuer Zusatzaufgabe. | „P.S.: Der Betriebshof hat aktuell Personalmangel. Könnten Sie den nebenbei übernehmen?“ |
| **Gerd vom Betriebshof** | erschöpft, trocken, herzlich | „Ich bin der Betriebshof. Ganz wörtlich.“ Redet über Lackierungen, Waggons, Kaffee, Rücken. | „Ersatzgarnitur. Ersetzt seit 2009 eine andere Ersatzgarnitur.“ |
| **Pressestelle** | strahlend positiv, egal was passiert | Formuliert jede Katastrophe als Erfolg. | „Die Pünktlichkeit hat sich gegenüber einem besonders ungünstigen Vergleichszeitraum deutlich verbessert.“ |
| **Rundschreiben Konzernleitung** (selten) | visionär, realitätsfern | Großprojekte, die nie fertig werden. | „Pünktlichkeitsziel 2041 bestätigt.“ |

Die Einstiegsmail kommt von **Gerd Hollerbach (Abteilungsleitung)** – bereits so im Spiel. Name nicht ändern. Gerd vom Betriebshof ist eine andere Person mit Vornamen Gerd; wenn das verwirrt, heißt der Betriebshof-Kollege **„Gerd (Betriebshof)“** und die Abteilungsleitung unterschreibt künftig mit „G. Hollerbach“. Mails: höchstens vier kurze Sätze plus P.S.

### 2.8 Tonproben (Maßstab für alles Neue)

- Tafel: „Weichenstörung. Ursache: Weiche.“ · „Ihr Anschluss wartet. Behauptet er.“ · „Wegen Verspätung aus vorheriger Fahrt verzögert sich auch diese Fahrt. Die vorherige Fahrt war 2019.“
- Ergebnis: ★ „Angekommen. Mehr war nie versprochen.“ · ★★ „Anschluss erwischt. Bitte erzählen Sie es niemandem, sonst will das jeder.“ · ★★★ „Tatsächlich pünktlich. Der Vorfall wird untersucht.“
- Laden: „Verbindung wird gesucht … zuletzt 2014 gesehen.“ · „Bitte haben Sie Geduld. Sie haben ja Geduldspunkte.“
- Durchsage-Hinweis: „Aufgrund von … *krrrz* … gehört in dieses Feld eine Kurve. Wir bitten um Verständnis.“ (Der Hinweis selbst ist immer eindeutig und vollständig.)

### 2.9 Der Stil („bubbly“)

Knubbelige Comic-Miniaturwelt in echtem 3D, Referenz: die Anmutung von Clash of Clans. **Kein Fotorealismus, keine KI-generierten Bilder im Spiel.** Formen rund, übertrieben, gestaucht; satte Farbflächen mit weichem Verlauf; warmes Licht von oben links, weiche Schatten, leichte Randaufhellung; alles federt kurz (0,1–0,3 s). Pendler sind rundliche Figuren mit Punktaugen, keine Gesichter auf Zügen. Alle Teile kommen aus **einem** maßstabsgleichen Baukasten mit gemeinsamem Raster – dadurch kann nichts überlappen oder falsch skaliert sein (der alte Betriebshof war genau das: KI-Slop).

**Harte Regeln gegen den „interaktives PDF“-Look:**
1. Kein Bildschirm hat Titel/Untertitel/Fließtext-Hierarchie.
2. Außerhalb von Mails höchstens eine Textzeile pro Gegenstand.
3. Keine Listen aus Textknöpfen – Auswahl geschieht über Gegenstände in der Szene.
4. Kein Bildschirm ohne Szene. Weißer Hintergrund ist verboten.
5. Jeder Knopf ist ein Objekt mit Volumen, Schatten und Drückzustand.
6. Jeder Bildschirm hat genau eine Hauptaktion, und sie ist das auffälligste Objekt.

---

## Teil 3 – Technik-Spickzettel (Ist-Zustand)

- Unity **6000.3.23f1**, C#, URP 17.3 (derzeit mit Renderer2D), UI Toolkit (UXML/USS), Linear Color Space. Assembly der Präsentation: `STP.Presentation.UI` (`-nullable:enable -warnaserror+`).
- Hauptdateien: `Assets/StammstreckenPuzzle/UI/ProductionGame.uxml` / `.uss`, `ProductionGameLocale.json` (Systemtexte), `ProductionCampaignText.json` (720 Rätseltexte, Quelle `Content/Localization/source/de-DE.json`), `Scripts/Presentation/UI/ProductionGamePresenter.cs` (Bildschirmlogik), `ProductionPuzzleGrid.cs` (Brett, Zugfahrt), `ProductionPostgamePanels.cs` (Endlos/Revision).
- **Übergangslösung 3D:** `Scripts/Presentation/UI/World3D/` (`StpWorld3D.cs`, `StpWorldKit.cs`, `StpBoardWorld.cs`) erzeugt echte 3D-Geometrie (Gleise, 7 Züge, 3 Waggons, Kulissen, 29 Katalogobjekte) in Software und malt sie als UI-Mesh. Das ist **nicht** das Endziel (keine Kamerafahrten, kein Licht, keine Animation). Die Geometrie-Bausteine (`StpBuilder`, `StpKit.Loco/Wagon/TrackBallast/TrackRails/Module/House/...`) sind aber gut und dürfen als Quelle für echte Unity-Meshes weiterverwendet werden.
- CI: `.github/workflows/unity.yml`. Push auf Branch startet CI, überspringt Unity aber oft („source unchanged“). Für einen vollen Lauf **einmal** `gh workflow run unity.yml --ref <branch>`, dann warten. **Nie mehrfach dispatchen** (Concurrency-Gruppe bricht Läufe gegenseitig ab). Dauer gesamt ca. 45–60 min. APK-Artefakt: `android-development-il2cpp`. CI läuft mit `-nographics` (keine GPU): Code, der Kamera/RenderTexture braucht, muss ohne Grafikgerät sauber zurückfallen (z. B. `SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null` → nichts rendern, keine Exception).
- iOS-Jobs in CI schlagen wegen Unity-Hub-Installationsbefehl fehl (Infrastruktur, Paket 3).
- **Lock-Kette (nur einmal am Paketende):** Nach Änderungen an Präsentationscode, UI- oder Art-Dateien schlagen Hash-Tests fehl. Reihenfolge:
  1. `node tools/campaign/project_candidate.cjs` und danach `node tools/campaign/project_candidate.cjs --check`.
  2. Falls `MobileCandidate.*.json` sich geändert haben: in `Content/Quality/endless/p05-structure-rework-final-v2/sample-audit.internal.json` die `sourceFiles[].sha256` auf die neuen Roh-SHA256 setzen.
  3. `node tools/campaign/project_human_assignments.cjs --repo-root . --out /tmp/hproj --build-id <neue-id> --study-id wp046-human --endless-sample Content/Quality/endless/p05-structure-rework-final-v2/sample-audit.internal.json`; dann `/tmp/hproj/participant/*.json` nach `Assets/StammstreckenPuzzle/UI/HumanAssignments/` und `/tmp/hproj/internal/source-bindings.json` nach `Content/Quality/human/assignment-source-bindings.json` kopieren.
  4. Zahl der Dateien im UI-Lock (aktuell 79) steht fest in `Tests/EditMode/Content/HumanPackSceneBindingTests.cs`, `tools/campaign/ProductionImport/UiTransfer.cs`, `tools/campaign/HumanReadback/SelfTests.cs`, `tools/campaign/tests/project_human_assignments.test.cjs` – bei neuen `.cs`-Dateien unter `Scripts/Presentation/` oder neuen Art-Dateien anpassen.
  5. Bei geänderten Fixture-Dateien unter `Tests/PlayMode/Bootstrap/RuntimeFixtures/`: `Runtime.bindings.json` (Roh-SHA256 der Dateien; Kosmetik-Katalog-Hash ist der **JCS**-Hash = SHA256 von JSON mit sortierten Schlüsseln, ohne Leerzeichen) neu setzen und den neuen Roh-SHA256 von `Runtime.bindings.json` in `RuntimeGameIntegrationTests.cs` und `LocalProfileDeletionIntegrationTests.cs` ersetzen.
  6. Paket 3 baut daraus ein einziges Skript `tools/rebind_all.sh`. Wer früher fertig ist, darf es schon bauen.

---

## Reihenfolge und Branches

```
Paket 1 (3D-Welt + alle Bildschirme)  ─┐
                                       ├─► Paket 3 (Abschluss & Feinschliff)
Paket 2 (Satire-Inhalte, Klang, Gags) ─┘
```

- Paket 1 und Paket 2 laufen **gleichzeitig** auf eigenen Branches (`rework/p1-welt`, `rework/p2-satire`), beide von `rework/manus-rundum-rework`.
- Paket 2 schreibt **nur** Inhaltsdateien (JSON, Texte, Audio) nach dem Vertrag in Paket 2 Abschnitt E. Paket 1 liest diese Dateien. Dadurch gibt es keine Konflikte im Code.
- Paket 2 wird zuerst gemergt (kleiner), Paket 1 rebased darauf, dann Merge. Danach Paket 3.

---

# PAKET 1 – Die Bahnwelt in echtem 3D und alle Bildschirme neu

**Ziel:** Jeder Bildschirm des Spiels ist eine lebendige, knubbelige 3D-Bahnszene mit Gegenständen statt Dokument. Rätsel, Lösungsmoment, Fahrt, Ergebnis, Karte, Betriebshof, Profil, Hauptmenü, Einstieg, Saisonende und Dauerbaustelle sind nach Konzept 2.1 Abschnitt 9 umgesetzt.

### A. Technische Grundentscheidung (verbindlich)

1. **Echte Unity-3D-Szene statt Software-Mesh.** Eine persistente Szene `StpWorld` mit URP-Forward-Renderer (zusätzlich zum bestehenden 2D-Renderer oder als Ersatz – wähle, was stabil läuft), einer perspektivischen Kamera und einfachem Toon-/Gradient-Shading (Vertexfarben, ein Hauptlicht, weiche Schatten, Randlicht). Die UI Toolkit-Oberfläche liegt als Overlay darüber (HUD, Tafel, Knöpfe). Für Kartenvorschauen im Betriebshof darf eine RenderTexture genutzt werden.
2. **Baukasten:** Meshes werden prozedural aus den vorhandenen Bausteinen in `World3D/` erzeugt (StpBuilder-Dreiecke → `UnityEngine.Mesh`), oder in Blender gebaut und als FBX eingecheckt. Ein gemeinsames Raster (1 Einheit = 1 Rätselfeld), eine Palette, ein Material. **Keine KI-Bilder.**
3. **Grafikloses CI:** Ohne Grafikgerät wird die 3D-Szene nicht aufgebaut; UI und Spiellogik laufen weiter, Tests bleiben grün.
4. Ziel 60 fps auf Mittelklasse-Android: einfache Geometrie, statisches Batching, Bake oder ein Echtzeitlicht, Gleisdetails bei 9×9/10×10 reduzieren.

### B. Rätsel – die Baustelle

- **Kamera:** fast senkrecht, 10–15° geneigt; Raster vollständig sichtbar und gut lesbar auf 6,1-Zoll-Handy bis 10×10 (Zellen ≥ 28 px bei 10×10, ≥ 45 px sonst).
- **Baufeld:** knubbelige Schotterplatte mit erhöhtem Rand und feinen Fugen. A und B sind kleine Tunnelportale bzw. Bahnsteigenden am Rand. Randzahlen auf kleinen Schildern; erfüllt → grau, überschritten → kurz rot.
- **Gleise:** echte 3D-Gleise je Form: Schotterbett, braune Holzschwellen, zwei Stahlschienen, saubere Kurven. Graue Notiz = nur graue Schwellen ohne Schienen. X = gesprühtes rotes Kreuz auf dem Schotter. Auswahl = gelber Rand, unabhängig vom Inhalt. Gleis „ploppt“ mit Staubwölkchen ein (0,15 s).
- **Umgebung außerhalb des Rasters:** dezente Baustelle (Bauzaun, Baubude, Bagger, **Baustellenschild mit Fertigstellungsjahr**). Während des Lösens keine Bewegung auf dem Brett.
- **Kopftafel (oben, blau, existiert inhaltlich bereits):** links Linie und Nummer („S1 · 07“), Mitte Tafelmeldung in einer Zeile plus groß die Feldgröße („6×6“), rechts Bauzeit und die drei Sternziele als Abfahrtszeiten; bei Überschreitung orange „+0:08“.
- **Gleiskiste (unten):** Holzkiste mit den sechs Gleisstücken als kleine 3D-Stücke, Sprühdose (X), Bündel grauer Schwellen. Aktives Werkzeug gelb umrandet und angehoben. Seitlich drei Objektknöpfe: Rückgängig, Durchsage (Hinweis, mit Zahl der Gratis-Hinweise), Neu planen (Reset mit bestehender Rückfrage).
- Bestehende Bedienung (Antippen, erneut Antippen leert, Mehrfachauswahl, Zoom) bleibt exakt erhalten.

### C. Lösungsmoment, Fahrt, Ergebnis

1. Lösung rastet ein, Notizen verschwinden, Gleise blitzen nacheinander von A nach B auf.
2. **Kamera kippt auf ca. 40°** in die Szene; an A und B öffnen sich Tunnel/Bahnsteig mit wartenden Pendlern.
3. Gong + Durchsage „Ihr Zug fährt ein. Ja, wirklich.“; der **eigene Zug mit gekauften Waggons und Lackierung** fährt die gebaute Strecke (Pfad aus `ProductionTrainRide`), Waggons folgen exakt dem Gleis. Tauben fliegen auf, Pendler an B **applaudieren ungläubig**. 3–5 s, per Tap überspringbar.
4. Tafel klappt im Fallblatt auf „Strecke freigegeben“.
5. **Ergebnis = Pünktlichkeitsnachweis** (Amtsformular-Optik, fährt von unten herein): groß die Bauzeit, „Neue Bestzeit“ falls zutreffend, Sterne als **Stempel** nacheinander mit Ergebnistext, bei verfehlten Sternen „Noch 0:08 bis ★★★“, Geduldspunkte fliegen ins Messing-Zählwerk. Hauptknopf „Nächste Störung“ (öffnet sofort das nächste Rätsel per Kamerafahrt), Zweitknopf „Zum Stellwerk“ (Karte), kleiner Knopf „Nachweis teilen“ (erzeugt Bild des Nachweises mit Stempel und Text „Hiermit wird bestätigt, dass die Verbindung von A nach B pünktlich war. Der Vorfall wird untersucht.“ und öffnet das System-Teilen-Menü), ganz unten getrennt „Zusätzliche Geduld beantragen“ (freiwilliges Video, bestehende Logik).
6. Bestehende Ablauflogik bleibt: Letztes Kampagnenrätsel → Saisonende statt „Weiter“; im freien Modus startet „Weiter“ direkt die nächste Runde im gleichen Profil.

### D. Die Bildschirme (Konzept 2.1, Abschnitt 9)

| Bildschirm | Szene | Hauptaktion | Weitere Ziele als Gegenstände |
|---|---|---|---|
| **Erster Start** | Baubüro: Schreibtisch, Diensttablet, Fenster auf die Baustelle mit Baustellenschild | Großer Knopf „Auftrag annehmen“ auf dem Tablet → Kamerafahrt durchs Fenster auf die erste Baustelle | Einstiegsmail auf dem Tablet |
| **Geführter Einstieg** (Rätsel 1–5) | normale Baustelle | – | Pro Rätsel ein neues Element mit einem Satz im orangen Banner: 1 A/B + Gleise, 2 Randzahlen, 3 X („Hier kommt sicher nichts hin“), 4 graue Schwellen („Notizen kosten keine Sterne“), 5 Uhr und Sterne. Banner verschwindet, sobald die Handlung ausgeführt ist. Nach Rätsel 3 Mail von Gerd (Betriebshof) mit geschenkter Lackierung. |
| **Hauptmenü = Bahnsteig** | Leicht schräg über dem lebendigen Bahnsteig der aktuellen Welt, Pendler, Tauben, Welt-Gags | Großer roter Stellwerksknopf „Strecke freigeben“ | Große blaue Abfahrtstafel oben zeigt den nächsten Auftrag wie eine Abfahrt („S1 · Meldung 07 · Weichenstörung Ostkreuz · 6×6“ + Sterne). Tor zum Betriebshof, Tür zum Stellwerk (Karte), Briefkasten (Dienstpostfach mit Zähler ungelesener Mails), Fallblatt-Tafel „Betriebslage des Tages“ (Tagesbonus), Bauschild-Kiosk (Dauerbaustelle/Revision nach Saisonende). Oben rechts Messing-Zählwerk (Geduldspunkte). Unten schmale Symbolleiste: Bahnsteig, Betriebshof, Dienstausweis. |
| **Karte = Stellwerkspult** | Beleuchtetes Gleisbildpult, fünf Bereiche = fünf Welten | Blinkende rote aktuelle Störung | Gelöste Abschnitte leuchten, kommende dunkel; Antippen klappt eine Meldekarte (Name, Feldgröße, Sterne, Knopf). Zettel am Rand: „Bitte nichts anfassen, was funktioniert.“ |
| **Betriebshof = Werkhalle** | Werkhalle mit Drehscheibe, aktueller Zugverband ausgeleuchtet, Gerd mit Schraubenschlüssel und Kaffee | Kaufen/Ausrüsten des gezeigten Fahrzeugs | Wischen dreht die Drehscheibe; gesperrte Fahrzeuge unter Plane mit Preisschild; zwei Bereiche „Zugpark“ (Loks, Triebzüge, Waggons, Lackierungen) und „Betriebsverbesserungen“ (Objekte, die dann in der Welt stehen); Waggons per Antippen ankuppeln mit Kupplungsanimation (max. 3); je Objekt ein Satz von Gerd (aus Paket 2). |
| **Profil = Dienstausweis** | Dienstausweis auf Schreibtisch | – | Platzhalterfoto, Dienstgrad (steigt mit Sternen, vom Praktikanten bis „Oberbaurat für Ausnahmezustände“), Statistik, Dienstjubiläen, Zähler „Fertigstellung durch Sie verschoben: 14 Jahre“. Zahnrad → Einstellungen (dürfen schlicht bleiben, aber im Bahnlook). |
| **Saisonende** | Zug fährt über das ganze heile Netz, alle Pendler aller Welten jubeln, Konfetti, Tafeln zeigen „pünktlich“ | – | Mail Pressestelle, danach Mail Abteilungsleitung („P.S.: Ab morgen beginnen die Bauarbeiten für Season 2.“). „Nächste Störung“ ausgegraut: „Alle Störungen behoben. Vorerst.“ Zwei Wege öffnen sich: „Dauerbaustelle“ und „Betriebsrevision“. |
| **Dauerbaustelle (freier Modus)** | Vier Bauschilder vor endloser Baustelle = vier Profile, Schild „Fertigstellung: nie“ | Profil antippen = Runde starten | Profilwechsel jederzeit; nach jeder Runde „Weiter“ = nächste Runde gleiches Profil. |
| **Betriebsrevision** | Aktenschrank im Stellwerk mit gelösten Rätseln | Rätsel wählen und Sterne verbessern | nur nach Saisonende verfügbar (Logik existiert). |
| **Laden / Fehler / Dialoge** | Bahnhofsszene mit Durchsage | – | „Verbindung wird gesucht …“ mit wechselnden Ladesätzen; Rückfragen im weiß-orangen Störungsbanner-Stil. |

### E. Welt-Gags darstellen

Paket 2 liefert `Content/Satire/s1/world-gags.json` (Vertrag siehe Paket 2 E). Paket 1 baut die dort genannten Objekte und Animationen und spielt sie ab: Dauer-Gags laufen in Schleife, seltene Gags ca. bei jedem zehnten Menübesuch, **nie im Rätsel**. Mindestens die in Paket 2 B als **Pflicht** markierten Gags müssen sichtbar umgesetzt sein. Solange Paket 2 nicht gemergt ist, mit Platzhalter-JSON derselben Form arbeiten.

### F. Definition of Done Paket 1

- Alle Bildschirme der Tabelle D existieren als Szene mit Gegenständen; die sechs Regeln gegen den PDF-Look (Teil 2.9) sind auf jedem Bildschirm erfüllt.
- Rätsel ist auf 4×4 und 10×10 gut lesbar und exakt so bedienbar wie vorher.
- Lösungsmoment mit Kamerakippen, Fahrt mit eigenem Zugverband, applaudierenden Pendlern und Pünktlichkeitsnachweis mit Stempeln.
- Alle Weiter-/Zurück-/Ergebnis-/Freimodus-Pfade führen dorthin, wohin ihr Text es verspricht (Kampagne, letztes Level, Dauerbaustelle, Revision).
- Selbstdurchsicht: einmal komplett von „Auftrag annehmen“ bis Saisonende (Kampagne darf per Debug-Freischaltung übersprungen werden) und Dauerbaustelle durchklicken; Screenshots jedes Bildschirms (Editor) in den PR-Text, keine Extradatei.
- CI grün (Compile, EditMode, PlayMode, Android-APK).

---

# PAKET 2 – Satire-Inhalte, Stimmen, Klang und Welt-Gags

**Ziel:** Alle Texte, Mails, Gag-Beschreibungen und Klänge, die das Spiel braucht, um den Pendler-Test zu bestehen – als Inhaltsdateien, die Paket 1 nur noch abspielt.

### A. Texte prüfen und ergänzen

1. **720 Rätseltexte** (`Content/Localization/source/de-DE.json`, Schlüssel `level.s1.*.{title,status,result}`): Alle einmal gegen Teil 2 lesen. Raus damit oder umschreiben, wenn: Amtsbegriff-Salat, mehr als ein Fachbegriff pro Satz, Pointe braucht Nachdenken, Bus/SEV im Ergebnis eines normalen Zuges, Wiederholung derselben Pointe, „Schienenersatzverkehr“ zu oft. Zeichenlimits: Titel 58, Tafelmeldung 110, Ergebnis 130. Danach `ProductionCampaignText.json` per bestehendem Werkzeug neu erzeugen (`tools/campaign/ProductionImport`) und `python3 tools/text-library/render.py` ausführen.
2. **Systemtexte** (`Assets/StammstreckenPuzzle/UI/ProductionGameLocale.json`): Alle Knöpfe, Dialoge, Ladesätze, Einstellungen, Kauftexte, Werbetexte, Tagesbonus, Endlos, Revision in Bahnsprache und „Sie“. Werbefrei = „Ruhebereich buchen“ („Garantiert ohne Durchsagen. Ausnahmsweise.“), Video = „Zusätzliche Geduld beantragen“, Hinweis = Durchsage mit *krrrz*, Uhr = „Bauzeit“, Sterne = „Angekommen / Anschluss erwischt / Tatsächlich pünktlich“. Keine Texte für Rechtliches, Datenschutz, Kauf-Pflichtangaben verändern, die rechtlich klar sein müssen – dort sachlich bleiben.
3. Neue Systemtexte für Paket-1-Bildschirme: Stellwerks-Zettel, Abfahrtstafel-Formate, Dienstgrade (mindestens 8 Stufen), Dienstjubiläen (mindestens 15, z. B. „100 Strecken gebaut. Im gleichen Zeitraum wurde ein Planfeststellungsverfahren eröffnet.“), Ladesätze (mindestens 20), Pünktlichkeitsnachweis-Text, Durchsagen beim Lösen (mindestens 10 Varianten von „Ihr Zug fährt ein. Ja, wirklich.“), Baustellenschild-Texte.

### B. Welt-Gags (Inhalt und Regie)

Für jede der fünf Welten und den Bahnsteig im Hauptmenü. **Pflicht** (müssen in Paket 1 sichtbar werden):
- Baustellenschild 2. Stammstrecke mit durchgestrichenen Jahren und Zettel mit „?“; zählt bei jedem bestätigten Reset dauerhaft +2 Jahre (Wert speichert Paket 1 lokal, z. B. im Präsentations-Speicher, nicht im Spielstand-Vertrag).
- Aufzug zu Gleis 2 „Außer Betrieb“ mit Plakette „Instandsetzung geplant“, die langsam verstaubt.
- Bahnsteiguhr geht drei Minuten nach.
- Pendlergruppe läuft wegen geänderter Wagenreihung ans andere Bahnsteigende und zurück.
- SEV-Bus auf Parkplatz, Schild „Ersatzhaltestelle“ mit Pfeil ins Nichts.
- Tauben auf der Oberleitung, fliegen erschrocken auf, wenn tatsächlich ein Zug kommt.
- Seltene Gags (Menü, ca. jeder 10. Besuch): Zug fährt ohne Halt durch („Fährt heute ohne Halt“); Tafel springt +5/+10/+20/„Zug fällt aus“, Pendler seufzen synchron; Durchsage nur Knistern, dann „Wir bitten um Verständnis.“; seltene umgekehrte Wagenreihung bei der eigenen Fahrt.
**Optional:** Rolltreppe fährt nur nach unten; Kaffeebecher-Stapel auf der Bank wächst mit gespielten Rätseln; Zugbegleiter rennt mit Kaffee dem Zug hinterher.

Welt-Grundwitze (Konzept 2.1 Abschnitt 5): 1 Stammstrecke (überfüllter Tunnelbahnhof), 2 Bauabschnitt Mitte (Baugrube, wandernde Bauzäune, Bagger ohne Fahrer), 3 Außenast ohne Halt (einsamer Bahnsteig, Spinnweben-Bank, „Bedarfshalt“), 4 Umleitung über Irgendwo (Wegweiser in alle Richtungen, Brücke „vorläufig tragfähig“), 5 Schienenersatzverkehr (Buskolonne, gestrichelte Linien statt Gleise, Pendler mit Rollkoffern auf Feldwegen, am Ende echte Gleise).

### C. Mails (Dienstpostfach)

Mindestens 40 Mails des Ensembles (Teil 2.7), je höchstens vier Sätze plus ggf. P.S. Anlässe: Einstieg (existiert), nach Rätsel 3 (Gerd + Geschenk-Lackierung), erster Betriebshof-Besuch, erster Kauf, erster Waggon, jede neue Welt (5×), erste 3 Sterne, erster Reset, 10/50/100/240 Rätsel, Saisonende (Pressestelle + Abteilungsleitung), Freischaltung Dauerbaustelle und Revision, plus 10–15 seltene Rundschreiben ohne Anlass (Konzernleitung, Pressestelle). Kein Anlass darf eine Belohnung versprechen, die das Spiel nicht vergibt – nur die bestehende Geschenk-Lackierung nach Rätsel 3 ist eine echte Belohnung.

### D. Gerds Sätze

Ein trockener Satz von Gerd (Betriebshof) für jedes der 29 Katalogobjekte. Beispiele: „Ersatzgarnitur. Ersetzt seit 2009 eine andere Ersatzgarnitur.“ · „Speisewagen. Heute ohne Speisen.“ · Münchner U-Bahn: „Neue Generation. Fährt pünktlich, sagt man. Ich hab's nicht gesehen.“

### E. Klang

Eigene oder lizenzfreie (CC0) Sounds, in `Assets/StammstreckenPuzzle/Audio/` mit Quellenangabe in `ThirdPartyNotices.txt`: eigener Durchsage-Gong (zwei/drei Töne, **nicht** der Original-Dreiklang nachspielen), Fallblatt-Klappern, Gleis-Plopp, Stempel, Türpiepen, Bremsquietschen, Pendler-Applaus (ungläubig, dünn), Pendler-Seufzen, Tauben-Flattern, Bahnhofs-Hintergrund. Kurz, leise, abschaltbar über bestehende Audio-Einstellungen.

### F. Vertrag zu Paket 1 (Dateien, die Paket 2 anlegt)

Ordner `Content/Satire/s1/`, UTF-8, einfache JSON-Dateien ohne Hashes:

```json
// mails.json
{ "mails": [ { "id": "mail-welcome", "trigger": "FIRST_START", "from": "ABTEILUNGSLEITUNG", "subject": "…", "body": ["…","…"], "ps": "…" } ] }
// trigger ∈ FIRST_START, AFTER_PUZZLE_3, FIRST_DEPOT, FIRST_PURCHASE, FIRST_WAGON, WORLD_ENTER_1..WORLD_ENTER_5,
//   FIRST_THREE_STARS, FIRST_RESET, CLEARS_10, CLEARS_50, CLEARS_100, SEASON_END_PRESS, SEASON_END_LEAD,
//   ENDLESS_UNLOCKED, REVISION_UNLOCKED, RANDOM_CIRCULAR
// from ∈ ABTEILUNGSLEITUNG, BETRIEBSHOF, PRESSESTELLE, KONZERNLEITUNG

// world-gags.json
{ "gags": [ { "id": "gag-elevator", "scene": "PLATFORM|WORLD_1..WORLD_5|DEPOT|MAP|ALL", "kind": "LOOP|RARE",
             "required": true, "objects": ["elevator","sign-out-of-order"], "line": "Außer Betrieb", "direction": "kurze Regieanweisung für die Animation" } ] }

// voices.json  – Gerds Sätze je Katalog-ID, Dienstgrade, Dienstjubiläen, Ladesätze, Durchsagen
{ "depotLines": { "train-stammrot-default": "…" }, "ranks": [ {"minStars": 0, "title": "Praktikant"} ],
  "jubilees": [ {"id": "j-100", "condition": "CLEARS_100", "text": "…"} ], "loading": ["…"], "arrivalAnnouncements": ["…"],
  "boardMessagesRare": ["…"] }
```

Paket 1 bindet diese Dateien ein (TextAsset, nicht über die Hash-Lock-Kette).

### G. Definition of Done Paket 2

- Alle Texte aus A bis D vorhanden und gegen Teil 2 gelesen; keine Ausrufezeichen-Comedy, keine Amtsbegriff-Ketten, „Sie“ durchgehend.
- `Content/Satire/s1/*.json` vollständig nach Vertrag F; Sounds aus E eingecheckt.
- Text-Werkzeuge laufen (`tools/text-library/validate.py`, `render.py`), CI grün.

---

# PAKET 3 – Abschluss: Sternzeiten, Gerät, Feinschliff, Aufräumen

**Start erst, wenn Paket 1 und 2 gemergt sind.**

### A. Sternzeiten

Aktuell sind die Sternzeiten Schätzungen (`Content/Quality/s1/star-time-estimates.json`, Status DRAFT), eingebaut in `UI/MobileCandidate.completion.json`. Regel: ★ immer, ★★ und ★★★ über Zeitgrenzen; **★★★ muss auch mit Hilfsfeldern (X, grau) erreichbar sein** – also nicht nur für Speedrunner ohne Notizen. Vorgehen: Alex spielt einige Rätsel je Feldgröße auf dem Gerät; das Spiel schreibt dabei die Bauzeiten lokal mit (einfaches Debug-Export, kein Studiensystem). Daraus Faktor je Feldgröße ableiten: ★★★ ≈ 1,3 × Median von Alex, ★★ ≈ 2,5 × Median. Werte einsetzen, Status auf „CALIBRATED_OWNER“. Ohne Alex' Daten: Schätzung lassen.

### B. Gerät und Leistung

60 fps auf einem Mittelklasse-Android (Alex' Gerät), Speicher, Ladezeit; Lesbarkeit 10×10; Touch-Ziele ≥ 44 pt; Bedienung mit einer Hand. Was ruckelt, vereinfachen.

### C. Feinschliff aus Alex' APK-Test

Alle Fehler, die Alex nach Paket 1+2 meldet, direkt beheben.

### D. Aufräumen der alten Bürokratie

- iOS-CI reparieren (Unity-Hub-Installationsbefehl auf aktuelle Hub-CLI umstellen) oder iOS-Jobs bis zum iOS-Start deaktivieren.
- Architecture Validation: WP-046-Scope-Gate und Governance-Prüfungen (ADR-Sequenz, Scope-Manifeste, Statusdokumente) für den Rework-Branch abschalten oder auf eine schlichte Prüfung „Schemas gültig, Dateien parsebar“ reduzieren.
- Lock-Kette aus Teil 3 in ein Skript `tools/rebind_all.sh` gießen, das alles in einem Schritt neu bindet.
- `PROJECT_CONTROL/CURRENT_STATE.md` auf zehn Zeilen kürzen: aktueller Stand, Branch, wie man baut.

### E. Definition of Done Paket 3

Sternzeiten eingesetzt (oder bewusst Schätzung), APK läuft flüssig auf Alex' Gerät, Alex' Fehlerliste abgearbeitet, CI vollständig grün inklusive Validation (iOS grün oder bewusst aus), ein Befehl zum Neubinden.

---

## Anhang – Startprompts zum Kopieren

**Paket 1 (Goal Mode):**
> Du arbeitest im Repo `Al1l1l1l1l1/Stammstrecken-Puzzle`. Lege von `rework/manus-rundum-rework` den Branch `rework/p1-welt` an. Lies vollständig `REWORK/STP_Rework_Arbeitspakete_2026-11-26.md` Teil 0 bis 3 und PAKET 1 sowie `REWORK/Konzept_2.1_Rundum_Rework.md`. Setze PAKET 1 vollständig um, bis die Definition of Done erfüllt ist. Halte dich an Teil 0 (keine QC-Schleifen, keine neuen Prüf- oder Governance-Dokumente, keine Rückfragen zu Dingen, die die Datei beantwortet). Nimm die Satire nicht wörtlich, sondern nach Teil 2. Am Ende: Push, genau ein CI-Lauf per `gh workflow run unity.yml --ref rework/p1-welt`, kurze deutsche Abschlussmeldung mit APK-Link.

**Paket 2 (Goal Mode, gleichzeitig):**
> Du arbeitest im Repo `Al1l1l1l1l1/Stammstrecken-Puzzle`. Lege von `rework/manus-rundum-rework` den Branch `rework/p2-satire` an. Lies vollständig `REWORK/STP_Rework_Arbeitspakete_2026-11-26.md` Teil 0 bis 3 und PAKET 2 sowie `REWORK/Konzept_2.1_Rundum_Rework.md`. Du bist Autor und Regisseur der Satire: Verstehe Teil 2 (Humorformel, Pendler-Test, Fehlertabelle, Lore) und schreibe danach. Setze PAKET 2 vollständig um, ändere keinen C#-Code außer Text-Werkzeugen. Halte dich an Teil 0. Am Ende: Push, ein CI-Lauf, kurze deutsche Abschlussmeldung.

**Paket 3 (Goal Mode, nach Merge von 1 und 2):**
> Du arbeitest im Repo `Al1l1l1l1l1/Stammstrecken-Puzzle` auf `rework/manus-rundum-rework` (Pakete 1 und 2 sind gemergt). Lies `REWORK/STP_Rework_Arbeitspakete_2026-11-26.md` Teil 0 bis 3 und PAKET 3. Setze PAKET 3 vollständig um. Hier ist Alex' Fehlerliste aus dem APK-Test: <LISTE EINFÜGEN>. Halte dich an Teil 0. Am Ende: Push, ein CI-Lauf, kurze deutsche Abschlussmeldung.

**Merge (kann Alex selbst oder ein beliebiger Agent):**
> Merge `rework/p2-satire` in `rework/manus-rundum-rework`. Rebase danach `rework/p1-welt` darauf, löse Konflikte (Paket 1 bindet `Content/Satire/s1/*.json` ein), binde die Lock-Kette nach Teil 3 neu, ein CI-Lauf, dann Merge.
