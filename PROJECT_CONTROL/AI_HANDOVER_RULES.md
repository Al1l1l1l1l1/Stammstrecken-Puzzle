# Regeln für die Übergabe zwischen Agents

**Alles steht im Repository.** Ein neuer Agent braucht nur den Repo-Namen, den Branch und die Nummer seines Arbeitspakets. Der Einstieg steht in [`AGENTS.md`](../AGENTS.md). Alex muss keine Dateien zwischen Agents hin- und herschicken und keinen Koordinator bedienen.

**Fortsetzen:** Wenn ein Lauf abbricht, setzt der nächste Agent auf demselben Branch fort. Er liest `AGENTS.md`, sein Paket und `git log` seines Branches und macht weiter. Kein Neuanfang, keine neue Planung.

**Parallel arbeiten:** `WP-047` und `WP-048` laufen gleichzeitig auf getrennten Branches. Die Dateigrenzen stehen in beiden Paketen unter „Ausdrücklich nicht erlaubte Änderungen“. Der Datei-Vertrag für `Assets/StammstreckenPuzzle/UI/Satire/*.json` steht in `WP-048`.

**Zusammenführen:** Jeder Agent mergt sein Paket selbst zurück nach `rework/manus-rundum-rework` (`WP-048` zuerst). `WP-049` mergt am Ende nach `main`.

**Melden:** Eine kurze deutsche Chatmeldung an Alex pro Paket. Keine Übergabedateien, keine Berichte.

**Eskalieren:** Nur bei echten Produktfragen (Regeln, Ökonomie, Spielstand-Format) oder fehlenden Zugängen. Alles andere entscheidet der Agent selbst.
