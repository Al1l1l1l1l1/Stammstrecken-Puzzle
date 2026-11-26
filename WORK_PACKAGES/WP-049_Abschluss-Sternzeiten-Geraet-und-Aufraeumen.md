# WP-049 – Abschluss: Sternzeiten, Gerät, Feinschliff, Aufräumen und Merge nach main

`WP-049`

**Bearbeitungsstatus:** Offen · startet erst, wenn `WP-047` und `WP-048` auf `rework/manus-rundum-rework` gemergt sind · ein Agent, ein Goal-Mode-Lauf

**Scope-Manifest des gesamten Reworks:** [WP-047.production.scope.json](../tools/architecture-validation/scopes/WP-047.production.scope.json) (gilt auch für dieses Paket; nie verändern)

**Pflichtlektüre:**

1. [Satire- und Lore-Bibel](../PRODUKT/SATIRE_UND_LORE_BIBEL.md) – Kapitel 1–5 und 12 (für den Feinschliff musst du wissen, was „richtig“ aussieht und klingt).
2. [Konzept 2.1 – Rundum-Rework](../PRODUKT/KONZEPT_2.1_RUNDUM_REWORK.md) – Abschnitt 6 (Systeme), 9 (Bildschirme), 11 (Rätselqualität, Sternzeiten).
3. [`WP-047`](WP-047_3D-Bahnwelt-und-alle-Bildschirme.md) und [`WP-048`](WP-048_Satire-Inhalte-Stimmen-Klang-und-Einstieg.md) – deren Akzeptanzkriterien sind dein Abnahmemaßstab.
4. Dieses Arbeitspaket bis zum Ende.

---

## ID

`WP-049` – Abschluss des Rundum-Reworks: Sternzeiten, Gerät und Leistung, Alex' Fehlerliste, CI vollständig grün, Prozess-Ballast entfernen, Merge nach `main`.

## Ziel

Am Ende dieses Pakets ist das Rework **fertig**: Die APK läuft flüssig auf einem Mittelklasse-Android, die Sterne funktionieren mit sinnvollen Zeiten, jeder Fehler aus Alex' Test ist behoben, die CI ist grün oder ehrlich abgeschaltet, das Repo ist nicht mehr mit Konzern-Prozessen zugemüllt, und `main` enthält den neuen Stand.

### Warum dieses Paket existiert

Alex ist Solo-Unternehmer. Bisher wurde jede Kleinigkeit durch QC-, Rework- und erneute QC-Schleifen gezogen; ein „Validator“ prüft Dutzende Governance-Regeln, die für ein Handyspiel eines Einzelnen keinen Wert haben. Alex wörtlich: „Ich bin Solo-Unternehmer, kein Konzern, und wir bauen hier auch nicht das neue SAP.“ Dieses Paket räumt das auf – **ohne die echten Sicherheitsnetze zu entfernen** (Compile, Tests, Build, Schemas, Spielstand-Kompatibilität bleiben).

## Voraussetzungen

- `WP-047` und `WP-048` sind auf `rework/manus-rundum-rework` gemergt (in deren Datei steht „Abgeschlossen“). Wenn nicht: nicht anfangen, sondern in der Abschlussmeldung sagen, was fehlt.
- Branch: von `rework/manus-rundum-rework` den Branch `rework/wp-049-abschluss` anlegen.
- **Alex' Fehlerliste:** Wenn Alex dir beim Start eine Liste aus seinem APK-Test mitgibt, ist sie Teil dieses Pakets (Abschnitt C). Wenn er keine mitgibt: Abschnitt C mit deiner eigenen Durchsicht gegen die Akzeptanzkriterien von `WP-047` und `WP-048` erledigen.

### Arbeitsweise (gilt verbindlich)

- **Ein Agent, ein Goal-Mode-Lauf, das ganze Paket.**
- **Keine Review-Schleifen,** keine QC-Berichte, keine neuen Prüfdokumente. Eigene Durchsicht und CI reichen.
- **Keine Rückfragen** zu Dingen, die dieses Paket beantwortet. Einzige erlaubte Frage an Alex: Gerätedaten für die Sternzeiten (Abschnitt A), und auch die nur, wenn er sie nicht schon mitgegeben hat – sonst Schätzung behalten.
- Am Ende mergst du **selbst** nach `rework/manus-rundum-rework` und danach nach `main` (Abschnitt „Definition of Done“). Alex merged nicht.

## Scope

### A. Sternzeiten

Heute: `Content/Quality/s1/star-time-estimates.json` (`approvalStatus: DRAFT`, `calibration: UNCALIBRATED_ESTIMATE`, Methode: ★★★ = clamp(round5(Solver-Deduktionsschritte × 0,8 s), 30 s, 240 s), ★★ = 2 × ★★★, ★ immer). Eingebaut in `Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json`.

**Regeln, die nie verletzt werden dürfen:**

- ★ gibt es **immer** beim Lösen.
- ★★ und ★★★ über Zeitgrenzen.
- **★★★ muss auch mit Hilfsfeldern (X und graue Schwellen) erreichbar sein.** Hilfsfelder sind freiwillig; wer sie nutzt wie ein Sudoku-Spieler mit Bleistift, darf nicht bestraft werden. Die Zeitgrenzen sind also für „gutes Spielen mit Notizen“ gedacht, nicht für Speedrunner ohne Notizen.
- Wiederholen (Übungsfahrt) bringt keine besseren Sterne in der Kampagne; „bewertet“ verbessern geht erst in der Betriebsrevision nach Saisonende (Logik existiert, nicht ändern).

**Vorgehen:**

1. Baue eine kleine **Debug-Aufzeichnung**: In Development-Builds schreibt das Spiel bei jedem gelösten Rätsel eine Zeile `{puzzleId, size, seconds, notesUsed, hintsUsed}` in `Application.persistentDataPath/star-times-debug.jsonl` und bietet in den Einstellungen (nur Development-Build) „Bauzeiten exportieren“ über das System-Teilen-Menü an. Kein Studiensystem, kein Server, kein Tracking in Release-Builds.
2. **Wenn Alex Daten liefert** (mindestens drei Rätsel je Feldgröße): Faktor je Feldgröße ableiten – ★★★ ≈ 1,3 × Median von Alex, ★★ ≈ 2,5 × Median, auf 5 s runden, je Rätsel mit dessen Deduktionstiefe skalieren. Werte in `star-time-estimates.json` einsetzen, `calibration: "CALIBRATED_OWNER"`, `approvalStatus: "APPROVED"`, Methode in einem Satz beschreiben. Danach `MobileCandidate.completion.json` über die vorhandene Projektion neu erzeugen (`python3 tools/rework/rebind_all.py`).
3. **Wenn nicht:** Schätzung lassen, aber sichtbar prüfen: Für fünf Rätsel je Feldgröße selbst die Lösung mit Notizen nachspielen (im Editor oder per Solver-Spur geschätzt) und sicherstellen, dass ★★★ mit Notizen realistisch ist. Offensichtliche Ausreißer korrigieren. Status bleibt `DRAFT`.
4. Die Kopftafel zeigt die Sternziele, der Pünktlichkeitsnachweis zeigt den Abstand („Noch 0:08 bis ★★★“) – prüfe, dass beides mit den neuen Werten stimmt.

### B. Gerät und Leistung

Ziel: **60 fps** auf einem Mittelklasse-Android (Referenz: Snapdragon-7-Klasse, 6 GB RAM, 1080×2400). Startzeit bis Bahnsteig ≤ 5 s kalt. Speicher ≤ 600 MB.

- Unity-Profiler-Messung im Development-Build auf dem Gerät (falls Alex' Gerät nicht verfügbar: Messwerte aus einem Emulator-/Cloud-Gerät, ehrlich als solche benennen).
- Was ruckelt, vereinfachen: Polygonzahl, Schatten-Distanz, Pendler-Anzahl, Instancing, statisches Batching, Shader-Varianten.
- **Lesbarkeit 10×10** auf 6,1 Zoll: Zellen ≥ 28 px, Randzahlen lesbar, Gleisformen unterscheidbar.
- **Touch-Ziele ≥ 44 pt**, Bedienung mit einer Hand (Gleiskiste unten im Daumenbereich).
- Akku/Wärme: Im Rätsel (statische Szene) Bildrate auf 30 fps drosseln, sobald 2 s keine Eingabe und keine Animation läuft; bei Eingabe sofort zurück auf 60.
- Einstellung „Bewegung reduzieren“ (`settings-motion`) muss Kamerafahrten, Federn und seltene Gags abschalten bzw. verkürzen.

### C. Feinschliff aus Alex' APK-Test

Alex' Kritikpunkte aus dem letzten Test (vor dem Rework) – **jeder muss nach `WP-047`/`WP-048` erledigt sein**. Prüfe jeden einzelnen auf dem APK und behebe, was noch nicht stimmt:

| Nr. | Alex' Punkt (sinngemäß) | Erwartung jetzt |
|---|---|---|
| 1 | Banner oben muss blau oder weiß-orange sein | Kopftafel tiefblau; Störungsbanner weiß-orange |
| 2 | Im Haupt-Textfeld muss die Feldgröße stehen | „6×6“ groß in der Kopftafel |
| 3 | Texte wirr, Amtsbegriff-Salat | Pendler-Test bestanden (Stichprobe 30 Texte) |
| 4 | Reset-Knopf mit Rückfrage „Bau verzögert sich um zwei Jahre“ | vorhanden, Bauzeit läuft weiter, Baustellenschild +2 Jahre dauerhaft |
| 5 | Mehr Zugfarben; rote kaum unterscheidbar | zehn klar unterscheidbare Lackierungen |
| 6 | Neue Münchner U-Bahn | kaufbar (nur Geduldspunkte), hellblau-weiß, runde Front |
| 7 | Sieht aus wie PDF / flach | echte 3D-Welt, sieben Regeln gegen den PDF-Look |
| 8 | Nicht selbsterklärend → Einstieg als Mail | Einstiegsmail + geführter Einstieg Rätsel 1–5 |
| 9 | Betriebshof-Bild KI-Slop, Überlappungen, falsche Maßstäbe | Werkhalle aus Baukasten, nichts überlappt |
| 10 | Strecken zu logisch/regelmäßig | Generator path-search-v2 ist drin; Stichprobe aus jeder Welt: keine Serpentinen-Monotonie |
| 11 | Sternesystem funktioniert nicht, Zeiten fehlen | Sterne und Zeiten sichtbar und korrekt (Abschnitt A) |
| 12 | Letztes Level startet bei „Weiter“ immer wieder neu | „Weiter“ ausgegraut mit Hinweis auf freien Modus, dann Saisonende |
| 13 | Freier Modus: nach jeder Runde zurück zum Levelbildschirm | „Weiter“ startet direkt die nächste Runde |
| 14 | Schienen sehen nicht wie Schienen aus | Schotter, braune Schwellen, zwei Stahlschienen |
| 15 | Waggons einführen, kaufbar wie Loks | drei Waggons kaufbar, max. 3 angehängt, fahren mit |
| 16 | Ständig ganze Reihen rausstreichbar | Null-/Vollreihen nur gelegentlich (Generator), Stichprobe prüfen |

Plus: **jede neue Fehlermeldung, die Alex beim Start dieses Pakets mitgibt**, wird hier als weitere Zeile behandelt und behoben.

Rätsellogik, Generator und Kampagnendaten änderst du nur, wenn Punkt 10 oder 16 in der Stichprobe klar verfehlt ist – und dann nur über die vorhandenen Generator-Parameter und mit der vorhandenen Validierung (`tools/campaign`, `tools/public240`), nicht durch Handbearbeitung von Rätseln.

### D. iOS-CI

Die Jobs `unity-environment-ios` und `unity-ios-export` in `.github/workflows/unity.yml` scheitern am Unity-Hub-Installationsbefehl (`"/Applications/Unity Hub.app/Contents/MacOS/Unity Hub" -- --headless install …`, Zeilen ~477–487 und ~845–851). Das ist Infrastruktur, nicht Spielcode.

- **Erst versuchen, es zu reparieren** (höchstens ein Versuch mit einem einzigen CI-Lauf): aktuelle Hub-CLI-Syntax (`install --version 6000.3.23f1 --changeset 09d2ecc7fb28 --module ios`), oder Installation über den direkten Download des Editors + iOS-Support-Pakets.
- **Wenn das nicht grün wird:** iOS-Jobs bis zum iOS-Start abschalten (`if: false` mit Kommentar „iOS bis zum iOS-Start deaktiviert, siehe WP-049“) und in der Abschlussmeldung ehrlich sagen. Kein zweiter, dritter, vierter Reparaturversuch.

### E. Prozess-Ballast entfernen

Was **bleibt** (echte Sicherheitsnetze): Compile, EditMode-, PlayMode-Tests, Android-Build, Schema-Validierung von JSON-Inhalten, Spielstand-Kompatibilität, `tools/text-library/validate.py`, `tools/rework/check_satire.py`, `tools/rework/rebind_all.py --check`.

Was **weg** bzw. schlanker wird:

1. **`tools/architecture-validation/validate.py`:** Die Governance-Prüfungen, die für das Spiel keinen Wert haben, entfernen oder abschalten: Pflicht-Statussätze in `CURRENT_STATE.md`/`WORK_QUEUE.md`, ADR-Nummern-Sequenz, `OPEN_BLOCKERS`-Statuszählung, Scope-Manifest-Anker-Vertrauenskette, historische WP-Blob-Bindung, Verbot bestimmter WP-ID-Muster. Behalten: JSON-Schemas, Gleisport-/Rätselprüfungen, Assembly-Grenzen, Link-Prüfung, Selbsttest (`--self-test`) für das, was bleibt. Danach muss `python3 tools/architecture-validation/validate.py` ohne Manifest-Argument laufen; `.github/workflows/validate.yml` und `tools/ci/test_change_scope.py` entsprechend anpassen.
2. **`PROJECT_CONTROL/`:** `PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md`, `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md`, `WP-028_EXECUTION_BRIEF.md` und `ProductionBlocks/` nach `PROJECT_CONTROL/HISTORY/` verschieben (nicht löschen). `WORK_PACKAGE_RULES.md`, `DEFINITION_OF_DONE.md`, `AI_HANDOVER_RULES.md` auf je höchstens 20 Zeilen in der Sprache dieses Reworks bringen (ein Agent, ein Paket, eigene Durchsicht, selbst mergen).
3. **`ARCHITECTURE/OPEN_BLOCKERS.md`:** Blocker, die durch das Rework gegenstandslos sind, schließen; Rest in einem Satz je Blocker.
4. **`PROJECT_CONTROL/CURRENT_STATE.md`:** auf höchstens 25 Zeilen: aktueller Stand, Branch, wie man baut, wo die Produktquellen liegen.
5. **`AGENTS.md`:** bleibt kurz (Einstieg → aktuelles Arbeitspaket → Bibel).
6. Alte Work-Packages bleiben unverändert liegen (Historie). Alte Evidenz- und Review-Ordner unter `Content/Reviews/` nach `Content/Reviews/HISTORY/` verschieben, sofern kein Test sie liest (vorher mit `grep -r` prüfen).

### F. Merge nach main

`main` ist nicht geschützt. Nach grünem CI auf `rework/manus-rundum-rework`:

```bash
git checkout main && git pull
git merge --no-ff rework/manus-rundum-rework -m "Rundum-Rework (WP-047 bis WP-049)"
git push origin main
```

Dann **ein** CI-Lauf auf `main` (`gh workflow run unity.yml --ref main`), grün abwarten. Branches `rework/wp-047-welt`, `rework/wp-048-satire`, `rework/wp-049-abschluss` dürfen danach gelöscht werden; `rework/manus-rundum-rework` bleibt als Referenz.

## Betroffene Dateien/Module

- `Content/Quality/s1/star-time-estimates.json`, `Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json` (über Projektion)
- `Assets/StammstreckenPuzzle/Scripts/**` für Debug-Aufzeichnung, Leistung, Fehlerbehebung aus Abschnitt C
- `Assets/StammstreckenPuzzle/UI/**`, `Art/**`, `Settings/**`, `Scenes/**` (über Generator), `Audio/**` für Feinschliff
- `Assets/StammstreckenPuzzle/Tests/**`
- `.github/workflows/unity.yml`, `.github/workflows/validate.yml`
- `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md`, `tools/ci/**`, `tools/rework/**`
- `PROJECT_CONTROL/**`, `AGENTS.md`, `ARCHITECTURE/OPEN_BLOCKERS.md`, `ARCHITECTURE/*.md` (nur Verweise)
- `Content/Reviews/**` (nur Verschieben in `HISTORY/`)
- Generator-Parameter in `Content/Generator/**` nur bei klarem Verfehlen von Punkt 10/16
- Am Ende: alles, was `tools/rework/rebind_all.py` neu schreibt

## Ausdrücklich nicht erlaubte Änderungen

- **Keine neuen Features.** Nichts, was nicht in `WP-047`, `WP-048` oder hier steht. Keine Sonderfelder (Alex: „keine Sonderfelder fürs Erste“), keine neuen Modi, keine neue Währung, kein Echtgeld für Fahrzeuge oder Waggons.
- **Spielstand-Format nicht brechen.** Bestehende Saves müssen geladen werden.
- **Ökonomie nicht ändern** (Preise, Belohnungen, Gewinnchancen).
- **Echte Sicherheitsnetze nicht entfernen** (Compile, Tests, Build, Schemas, Spielstand-Kompatibilität).
- **Kein Löschen von Historie** (alte WPs, ADRs, Konzepte bleiben; Ballast wird verschoben, nicht gelöscht).
- **Keine neuen Governance-, QC- oder Evidenzdokumente.** Keine neuen ADRs, keine neuen Scope-Manifeste; das Manifest oben nie verändern (es wird mit Abschnitt E1 ohnehin überflüssig).
- **Keine Unity-Zugangsdaten** in Dateien, Logs oder Commits.

## Akzeptanzkriterien

1. Sternzeiten kalibriert (`CALIBRATED_OWNER`) **oder** bewusst als Schätzung geprüft; ★★★ mit Notizen erreichbar; Tafel und Nachweis zeigen korrekte Werte.
2. Development-Build hat „Bauzeiten exportieren“; Release-Build hat es nicht.
3. 60 fps im Rätsel und auf dem Bahnsteig auf einem Mittelklasse-Gerät (oder ehrlich benannte Ersatzmessung); Kaltstart ≤ 5 s.
4. Alle 16 Punkte aus Abschnitt C plus Alex' neue Fehlerliste sind erledigt.
5. iOS-CI grün **oder** bewusst abgeschaltet mit Kommentar.
6. `validate.py` ohne Governance-Ballast, läuft ohne Manifest-Argument; Validate-Workflow grün.
7. `PROJECT_CONTROL/` schlank, Altlasten in `HISTORY/`.
8. CI auf `main` vollständig grün (Compile, EditMode, PlayMode, Android, Validation; iOS grün oder aus).

## Tests

- `python3 tools/architecture-validation/validate.py` (nach Umbau ohne Manifest) und `--self-test`
- `python3 tools/ci/test_change_scope.py`
- `python3 tools/text-library/validate.py`, `python3 tools/rework/check_satire.py`
- `python3 tools/rework/rebind_all.py --check` → PASS (vorher einmal ohne `--check`, falls nötig)
- **CI:** genau ein voller Lauf `gh workflow run unity.yml --ref rework/wp-049-abschluss`; nach dem Merge genau einer auf `main`. Nie mehrfach parallel.
- **Eigene Durchsicht auf dem APK:** einmal komplett von „Auftrag annehmen“ bis Saisonende (Debug-Freischaltung erlaubt), Dauerbaustelle drei Runden, Betriebsrevision ein Rätsel, Betriebshof: U-Bahn kaufen, Waggon ankuppeln, Lackierung wechseln, dann eine Fahrt ansehen.

## Risikoklasse

**Mittel.** Größtes Risiko: beim Entschlacken des Validators echte Prüfungen mit zu entfernen. Gegenmittel: Liste „was bleibt“ in Abschnitt E ist verbindlich.

## Definition of Done

1. Alle Akzeptanzkriterien erfüllt.
2. `rework/wp-049-abschluss` → selbst gemergt nach `rework/manus-rundum-rework` (`git merge --no-ff`), CI grün.
3. `rework/manus-rundum-rework` → selbst gemergt nach `main`, CI auf `main` grün.
4. `Bearbeitungsstatus` oben auf „Abgeschlossen“ (Datum, Merge-Commit auf `main`), `PROJECT_CONTROL/WORK_QUEUE.md` und `CURRENT_STATE.md` auf den neuen Stand.
5. **Abschlussmeldung an Alex auf Deutsch, höchstens zwölf Zeilen, ohne Fachjargon:** Spiel ist auf `main`, Link zum APK des grünen `main`-Laufs, Sternzeiten kalibriert oder geschätzt, Leistung in einem Satz, was bewusst offen bleibt (z. B. iOS).
