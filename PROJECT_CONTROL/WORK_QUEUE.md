# Produktionswarteschlange

Diese Warteschlange enthält nur verbindliche, persistierte Projektaufträge. Kein Produktionscoding beginnt ohne passendes Work Package, einen gemeinsamen Trust-Anchor-Commit, ein eigenes unveränderliches Scope-Manifest und die dort verlangte CI- und QC-Evidenz.

## Aktuelle Prioritätsreihenfolge

| Priorität | Block | Status | Verbindlicher Inhalt |
|---:|---|---|---|
| 0 | Trust-Anchor-Korrektur (WP-018-Amendment) | **Abgeschlossen** | PR #8 ist integriert; WP-019/WP-020 sind endgültig archivierte, nicht ausführbare Planungen. |
| 1 | WP-021 – Unity-Scaffold-Recovery | **Abgeschlossen und integriert** | Unity-Scaffold freigegeben; formaler Closeout über PR #12. |
| 2 | WP-022 – Puzzle-Kern-Recovery | **Abgeschlossen und integriert** | Finaler PR-HEAD `b257efd98bd14e7f66ffc40916a0b584c3f51fb7` unabhängig PASS; Geschäftsführungsfreigabe; Integration über PR #13, formaler Closeout über PR #14. |
| 3 | WP-023 – Level-v2-Foundation und Hashverträge | **Abgeschlossen und integriert** | Erste unabhängige Abschluss-QC auf `182db13…` = FAIL; vier Befunde innerhalb WP-023 reproduziert und behoben. Erneute unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68` = **PASS**, AK-01 bis AK-09 erfüllt, kein integrationsblockierender Befund. Geschäftsführungsfreigabe erteilt; PR #15 als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072` in `main` integriert. Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`; Manifest bis zur Integration byteunverändert. |
| 4 | WP-024 – Solver-v2-Metriken und Deduktionsspur | **Abgeschlossen und integriert** | Nach zwei korrigierten QC-FAIL-Runden vollständige unabhängige Abschluss-QC **PASS** auf `64d8801ee28d68dc0c8ec9800898b7862977c32c` gegen `ab117f459689d2b2937d436bb1a763296a65d52a`; AK-01 bis AK-12 erfüllt, keine integrationsblockierenden Befunde. Übermittelter Bericht und ausdrückliche Geschäftsführungsfreigabe vom 13.10.2026 („übernehmen“) in [PR #17, Review 5502357045](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/17#pullrequestreview-5502357045). Integration des unveränderten geprüften HEAD mit Expected-HEAD-Absicherung als Merge-Commit `22ff74558259c7ba1f9a157280e4d70b909bac26`. Trust Anchor `3e7f1ab2e56853294a4f09650d8ec361ab4eca1a`, Manifest unverändert. Finale PR-CI: Architecture Validation `38324116629` und Unity CI `38324116596` SUCCESS. Frühere FAIL-Berichte und Korrekturen bleiben im WP als Historie erhalten; keine weitere Reparatur oder automatische zusätzliche Abschluss-QC offen. |
| 5 | WP-025 – Proof-v1-Regeneration und Strict-Validation | **Abgeschlossen und integriert** | Beide unabhängigen Abschlussrollen PASS auf `92842bc653bf364bd7ade5bbff7c37f71bf9fd1f` gegen `5f5127362b101dd4e066b68b21d3e35a5af314df`; AK-B01 bis AK-B07 erfüllt, QC-STRICT-01 geschlossen. [Proof-QC](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6150287241), [Strict-/Tests-/Scope-/CI-QC](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6150288578), [Auftraggeberfreigabe vom 13.10.2026](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/19#issuecomment-6154248884). PR #19 mit Expected-HEAD-Absicherung als `f496ab9001d2043a17c3ffd2ff2a8a39d175495b` integriert; keine Dateiänderung gegenüber dem abgenommenen HEAD. Finale Architecture PR 38421061552 und Unity PR 38421061511 SUCCESS; 454/454 EditMode, 27/27 PlayMode, Coverage, Android-IL2CPP/iOS-Xcode. Anker und Manifest unverändert, frühere Befunde/Nachweise im WP erhalten. |
| 6 | WP-026 – 240 konkrete logische Rätselinstanzen | **Nachbesserung geliefert; erneute Abnahme/Paketabschluss offen** | Bestehender Draft-PR #21 ab unabhängiger Prüfung 3c9b7d0: 240 individuelle Level- und 20 Routenbewertungen, 15 gezielte Logikkorrekturen und aktuelle echte Nachweise. V-01 bis V-03 bearbeitet, Q-01 bis Q-06 einzeln entschieden; Originalbericht unverändert. Auftragskorrektur/Anker/Manifest erhalten. Neue unabhängige Prüfung/Spieltest, Auslegung „erste A/B-Kanten“, CI-Referenz-Leistung, historische Auswahlprovenienz und 1.440 fehlende Ziele/Folgegates offen. Übergabe: Content/Reviews/season-1/REPAIR_HANDOVER.md. |
| 7 | Finale Zeitwerte | Nicht begonnen | Nach gebauten, getesteten Rätselinstanzen. |
| 8 | Finale Asset-Bible | Nicht begonnen | Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Objekte, Motion und Sound. |
| 9 | Vollständige Textbibliothek | Nicht begonnen | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen. |
| 10 | Finale Zug- und Objektkataloge | Nicht begonnen | Züge, Betriebsobjekte, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 11 | Werbefrei-Produkt | Nicht begonnen | Plattformpreis, Kaufwiederherstellung, Consent- und Datenschutzflows. |
| 12 | Rechtliche Endprüfung | Nicht begonnen | Marken-, Urheber- und Herkunftseindruck durch qualifizierte Rechtsberatung. |
| 13 | Launchpaket | Nicht begonnen | Store-Metadaten, Screenshots, Trailer, Influencer-/Community- und Messplan. |

## Historische Basis

Die Vorgeschichte `WP-001` bis `WP-005` ist abgeschlossen; Architecture v1.0 bleibt die freigegebene Grundlage. Die vorab auf `main` angelegten WP-015/WP-016/WP-017-Dateien sind fachliche Planungsgrundlagen, können aber wegen ADR-030 nicht selbst nachträglich als erstmaliger ausführbarer Work-Package-/Manifest-Anker dienen.

| Historischer Block | Status | Aussage |
|---|---|---|
| CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** | Das zwingende CI-Gate vor Produktionscoding bleibt verbindlich. |

## Integrationsregel für historische Branches

`origin/feat/wp-008-unity-scaffold`, `origin/feat/wp-009-puzzle-kern` und `origin/feat/wp-013-level-v2-pipeline` sind keine direkt integrierbaren Branches. Ebenso sind vorab angelegte Planungs-WPs keine Ersatzanker. Kein Direktmerge, Cherry-Pick, Rebase, Copy, Konfliktauflösung oder Übernahme historischer PASS-Aussagen.

```text
main
  → WP-021 integriert und geschlossen
  → WP-022 integriert und geschlossen
  → WP-023 integriert und geschlossen
  → WP-024 integriert und geschlossen
  → WP-025 integriert und geschlossen
    → gemeinsamer Anker de314db auf 5f51273 unverändert
    → Phase A und separat beauftragte Phase B abgeschlossen
    → beide unabhängigen Abschlussrollen PASS auf 92842bc
    → ausdrückliche Auftraggeberfreigabe und Integration f496ab9
  → WP-026: vollständiger logischer Contentauftrag erteilt und gemeinsam verankert
  → Auftragskorrektur: BLOCKER-WP026-001 als allgemeiner Startblocker widerlegt
  → 240er-logischer Authoringbestand mit echten Proofs und eigenen Bewertungen geliefert
  → unabhängige Abnahme/Spieltest, Referenzauflösung und alle weiteren betroffenen Pflichtgates offen
```

Jeder Pfeil ist eine harte Voraussetzung. Ein Agent darf keinen späteren Schritt beginnen, um Scope-, CI-, Lizenz-/Runner- oder Vertragsnachweise zu umgehen.

## QC- und Rollenregel

Die Schutzfunktion ist eine **unabhängige Abschlussprüfung auf einem eingefrorenen Stand vor Integration**. Das konkrete ausführende KI-System ist kein Bestandteil der Projektarchitektur und darf zwischen Arbeitsschritten wechseln. Zusätzliche parallele Prüfungen sind nur erforderlich, wenn das konkrete Work Package oder eine ausdrückliche Geschäftsführungsentscheidung sie wegen des Risikos verlangt. Ein Merge ist ausgeschlossen, solange die erforderliche unabhängige Prüfung einen offenen Blocker oder High-Befund meldet.

## Offene Produktfolgeblocker

| Thema | Status | Wirkung |
|---|---|---|
| Hint-Entitlement / Hint-Economy (`BLOCKER-PROD-001`) | Offen, fail-closed | Blockiert Hintcredit-/Rewarded-Hint-Featurepakete. |
| Kalendertag / Zeitzone / Offline-Policy (`BLOCKER-PROD-002`) | Offen, fail-closed | Blockiert Tagesanspruch-/Daily-Featurepakete. |
| Generator-Qualitätsprofil (`BLOCKER-PROD-003`) | Offen, fail-closed | Blockiert Veröffentlichung generierter Dauerbaustellenlevel. |

Diese Blocker bleiben unverändert und werden nicht durch Recovery-, Solver- oder Proofarbeit umgangen.

## Pflichtquellen

| Datei | Relevanz |
|---|---|
| `CURRENT_STATE.md` | Autoritativer Phasen- und Integrationsstand. |
| `../WORK_PACKAGES/WP-025_Proof-v1-Regeneration-und-Strict-Validation.md` | Abgeschlossener Proof-/Strict-Auftrag: unveränderte Definition, historische Nachweise, beide finale QC-Berichte, Auftraggeberfreigabe und Integration. |
| `../tools/architecture-validation/scopes/WP-025.production.scope.json` | Eigenes unveränderliches Manifest ab gemeinsamem ADR-030-Anker de314db, Basis 5f51273. |
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Abgeschlossener und integrierter Level-v2-/Hash-Block als technische Basis. |
| `../WORK_PACKAGES/WP-024_Solver-v2-Metriken-und-Deduktionsspur.md` | Abgeschlossener und integrierter Solver-v2-Block mit beiden Korrekturrunden, finaler unabhängiger Abnahme, Freigabe und Integration. |
| `../WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` | Historische Planungsgrundlage von WP-025; kein ausführbarer Trust Anchor. |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Historische Planungsgrundlage für WP-024; nicht selbst ausführbarer Trust Anchor. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche gemeinsame historische WP-/Manifest-Erstverankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Verbindlicher Solver-v2-/Proofvertrag. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Integrierter Puzzle-Kern als technische Basis. |
| `DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |

## WP-025 Phase-B-Autorisierung – 13.10.2026 (historischer Beginn)

Separater ausdrücklicher Implementierungsauftrag eingegangen; Phase B in Bearbeitung auf bestehender Branch und PR #19 ab verifiziertem Vorbereitungs-HEAD 5e4b986bf29a3223e96b5993f37a99882581dab2. Die bisherige Phase-A-Beschränkung ist historisch. Priorität 5, sämtliche Anforderungen, Manifest, unabhängige Abschlussrollen und Integrationsgates bleiben unverändert. Erst nach tatsächlicher Implementierungs-CI wird derselbe finale HEAD für beide unabhängigen Prüfungen eingefroren; Integration benötigt weiterhin die ausdrückliche Auftraggeberentscheidung.

## WP-025 Abschluss – 13.10.2026

Die oben historisch verlangte Integration wurde nach erfolgreicher finaler CI, beiden unabhängigen PASS-Berichten und ausdrücklicher Auftraggeberentscheidung tatsächlich ausgeführt. Abschlussnachführung nur in WP-025, CURRENT_STATE und WORK_QUEUE; keine Änderung der Prioritäten oder Anforderungen. Nachgelagerte Dokumentations-CI bleibt separat auszuweisen. Es wurde kein Folgepaket erstellt, keine Folgeimplementierung gestartet und keine Veröffentlichung freigegeben.
