# Stammstrecken-Puzzle – aktueller Projektstand

Stand: 26.11.2026. **Architecture v1.0** bleibt technische Grundlage; `WP-001` bis `WP-007` sind abgeschlossen. Seit dem Owner-APK-Test läuft das **Rundum-Rework**. Das frühere Gesamtpaket [`WP-046`](../WORK_PACKAGES/WP-046_Vollstaendiges-Spiel-und-Release-Candidate.md) ist dadurch abgelöst.

**Branch:** `rework/manus-rundum-rework` (Basis aller Rework-Pakete). `main` wird erst am Ende von `WP-049` aktualisiert.

**Produktquellen (haben Vorrang):** [Satire- und Lore-Bibel](../PRODUKT/SATIRE_UND_LORE_BIBEL.md) · [Konzept 2.1](../PRODUKT/KONZEPT_2.1_RUNDUM_REWORK.md).

**Schon erledigt im Rework:** 720 Kampagnentexte überarbeitet; blaue Kopftafel mit Feldgröße; Rundenreset mit Rückfrage („Bau verzögert sich um zwei Jahre“, Bauzeit läuft weiter); letztes Level mit ausgegrautem „Weiter“ und Saisonende; Dauerbaustelle „Weiter“ startet direkt die nächste Runde; Sternzeit-Schätzung (DRAFT); Einstiegsmail; Generator path-search-v2 (unregelmäßigere Strecken); 29 Sammelobjekte inkl. Münchner U-Bahn und drei Waggons; Übergangs-3D als Software-Mesh; Lock-Kette in einem Befehl (`tools/rework/rebind_all.py`).

**CI-Stand:** Compile, EditMode, PlayMode und Android grün. iOS rot (Unity-Hub-Installation, Infrastruktur, wird in `WP-049` erledigt).

**Jetzt dran:** [`WP-047`](../WORK_PACKAGES/WP-047_3D-Bahnwelt-und-alle-Bildschirme.md) (3D-Welt, alle Bildschirme) und [`WP-048`](../WORK_PACKAGES/WP-048_Satire-Inhalte-Stimmen-Klang-und-Einstieg.md) (Texte, Mails, Gags, Klang) **parallel**. Danach [`WP-049`](../WORK_PACKAGES/WP-049_Abschluss-Sternzeiten-Geraet-und-Aufraeumen.md) (Sternzeiten, Gerät, Fehlerliste, Aufräumen, Merge nach `main`).

**Bauen und prüfen:** Unity läuft nur in GitHub Actions (`unity.yml`, ein voller Lauf per `gh workflow run unity.yml --ref <branch>`). Lokal: `python3 tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-047.production.scope.json`, `python3 tools/rework/rebind_all.py --check`, `python3 tools/text-library/validate.py`.
