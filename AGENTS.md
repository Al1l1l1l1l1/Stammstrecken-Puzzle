# Agenten-Einstieg – Stammstrecken-Puzzle

Stammstrecken-Puzzle ist ein deutschsprachiges Handy-Logikspiel (Gleise legen von A nach B, Randzahlen wie beim Sudoku) mit **Bahn-Satire als Existenzgrund**. Owner ist Alex, Solo-Unternehmer. Kein Konzern, keine Prüfketten.

## So fängst du an (immer, in dieser Reihenfolge)

1. [`PROJECT_CONTROL/CURRENT_STATE.md`](PROJECT_CONTROL/CURRENT_STATE.md) lesen: aktueller Stand, Branch, welches Paket dran ist.
2. **Dein Arbeitspaket** vollständig lesen. Welches das ist, steht in deinem Auftrag oder in [`PROJECT_CONTROL/WORK_QUEUE.md`](PROJECT_CONTROL/WORK_QUEUE.md):
   - [`WP-047` – 3D-Bahnwelt und alle Bildschirme](WORK_PACKAGES/WP-047_3D-Bahnwelt-und-alle-Bildschirme.md)
   - [`WP-048` – Satire-Inhalte, Stimmen, Klang, Einstieg](WORK_PACKAGES/WP-048_Satire-Inhalte-Stimmen-Klang-und-Einstieg.md)
   - [`WP-049` – Abschluss, Sternzeiten, Gerät, Aufräumen, Merge nach main](WORK_PACKAGES/WP-049_Abschluss-Sternzeiten-Geraet-und-Aufraeumen.md)
3. [`PRODUKT/SATIRE_UND_LORE_BIBEL.md`](PRODUKT/SATIRE_UND_LORE_BIBEL.md) lesen. **Pflicht für jedes Paket.** Dort steht, warum das Spiel existiert, wie der Humor funktioniert und was du auf keinen Fall wörtlich nehmen darfst.
4. [`PRODUKT/KONZEPT_2.1_RUNDUM_REWORK.md`](PRODUKT/KONZEPT_2.1_RUNDUM_REWORK.md) lesen (Stil, Bildschirme, Systeme).

Mehr musst du nicht lesen. Alte Work-Packages (`WP-001` bis `WP-046`), ADRs und Historie sind Nachschlagewerk, keine Arbeitsanweisung.

## Was Vorrang hat

1. Neue ausdrückliche Aussagen von Alex.
2. `PRODUKT/` (Bibel und Konzept 2.1) und das aktuelle Arbeitspaket.
3. Alles andere. Die alten Konzeptdateien unter `Stammstrecken_Puzzle_Konzept_00-15/` sind teilweise überholt (z. B. „Die Oberfläche wirkt dienstlich“, „helle, klare Informationsoberfläche“ – genau das hat den PDF-Look verursacht). Bei Widerspruch gilt `PRODUKT/`.

## Wie gearbeitet wird

- **Ein Agent, ein Paket, ein Goal-Mode-Lauf.** Das ganze Paket, nicht in Etappen. Kein „Prototyp mit einem Level“.
- **Keine QC-, Review- oder Rework-Schleifen.** Keine Prüfberichte, keine Evidenzordner, keine neuen Governance-Dokumente, keine neuen ADRs, keine neuen Scope-Manifeste. Deine eigene Durchsicht (Pendler-Test aus der Bibel) plus CI ist die Prüfung.
- **Keine Rückfragen** zu Dingen, die Paket, Bibel oder Konzept beantworten. Ist etwas wirklich offen: die Lösung wählen, die der Bibel am besten entspricht, und in der Abschlussmeldung in einem Satz nennen.
- **Eigener Branch** von `rework/manus-rundum-rework` (Name steht im Paket). **Du mergst selbst** zurück, wie im Paket beschrieben. Alex merged nicht.
- **Lock-Kette nur einmal am Ende:** `python3 tools/rework/rebind_all.py`, danach `--check`.
- **CI genau einmal pro Paket voll starten:** `gh workflow run unity.yml --ref <dein-branch>`. Nie mehrfach, nie in Schleife (Concurrency-Gruppe bricht Läufe ab).
- **Abschlussmeldung an Alex:** Deutsch, kurz (höchstens zehn bis zwölf Zeilen), ohne Fachjargon, mit Link zum APK des grünen CI-Laufs und ehrlich, was offen ist. Keine Dateien anhängen.

## Harte Grenzen

- Spielregeln, Solver, Spielstand-Format und Ökonomie bleiben unverändert, außer ein Paket sagt ausdrücklich etwas anderes.
- Keine KI-generierten Bilder oder Texturen im Spiel. Kein Fotorealismus.
- Kein echtes DB-Logo, keine Original-Ansagen, kein Original-Gong, keine Kopie realer App-Screens. Bahnbegriffe sind frei verwendbar.
- Kein Anwalts-, Rechts- oder Logo-Nebenschauplatz.
- Keine Unity-Zugangsdaten in Dateien, Logs oder Commits.
