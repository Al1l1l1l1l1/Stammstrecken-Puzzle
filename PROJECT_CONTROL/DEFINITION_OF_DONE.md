# Definition of Done

Fertig heißt: **Das Ergebnis läuft im Spiel, sieht gut aus und tut, was der Text verspricht.** Dokumente oder grüne Teilprüfungen allein reichen nicht.

Für jedes Arbeitspaket gilt:

1. Alle Akzeptanzkriterien des Pakets sind erfüllt.
2. Die betroffenen Abläufe wurden selbst durchgespielt (Editor oder APK). Satire und Optik bestehen den Pendler-Test aus der [Bibel](../PRODUKT/SATIRE_UND_LORE_BIBEL.md).
3. `python3 tools/rework/rebind_all.py --check` meldet PASS; lokale Validatoren grün.
4. Genau ein voller CI-Lauf ist grün: Compile, EditMode, PlayMode, Android-APK, Architecture Validation (iOS bis `WP-049` ausgenommen).
5. Der Agent hat selbst gemergt, wie im Paket beschrieben.
6. Status im Paket und in `WORK_QUEUE.md` steht auf „Abgeschlossen“.
7. Kurze deutsche Abschlussmeldung an Alex mit APK-Link und dem, was offen ist.

Unveränderliche Grundsätze: Das Raster bleibt ortsfest; Werkzeug wählen, Feld antippen setzt, erneutes Antippen mit derselben Belegung leert; Hilfsfelder (X, grau) sind freiwillig und ★★★ bleibt mit ihnen erreichbar; bestehende Spielstände laden weiter.

Keine zweite Abnahme, keine unabhängige Review-Runde, keine Nachweisdokumente.
