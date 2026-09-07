# Test Strategy v0.1

## 1. Qualitätsprinzip

Automatisierte Tests sind Teil des Architekturvertrags. Der Product Owner muss keinen Code lesen, um einen belastbaren Qualitätsnachweis zu erhalten. Jeder Test belegt eine fachliche Invariante, Modulgrenze, Migration oder reale Integrationsfähigkeit. Eine grüne Coveragezahl ohne relevante Behauptung genügt nicht.

## 2. Testpyramide

| Ebene | Anteil/Tempo | Hauptzweck |
|---|---|---|
| Pure EditMode | sehr breit, Sekunden | Domain, Solver, Applicationpolicies, Migration und Content ohne Szene. |
| Contract/Integration EditMode | breit, Sekunden bis Minuten | Serializer, Dateisystemadapter, Providerfakes und Importer. |
| PlayMode | gezielt, Minuten | Composition Root, UI Toolkit, Szenen, Addressables, Audio. |
| Device | klein, Minuten | Lifecycle, IL2CPP, Store-/Ads-/Consent-Sandbox und Performance. |
| Store Preflight | je Kandidat | Signatur, Manifest, Datenschutz, Store-Upload und Rollout. |

Manuelle explorative QA ergänzt automatisierte Beweise für UX, Satireton, Lesbarkeit, Audio und gerätespezifisches Gefühl. Sie ersetzt keine Gültigkeits-, Save- oder Storetests.

## 3. Domain- und Commandtests

Pflichtbehauptungen:

- alle sechs Trackformen besitzen exakt die dokumentierten zwei Ports;
- Rotation/Spiegelung bildet Formen, Endpoints und Counts korrekt ab;
- Gelb/Selection existiert nicht im Domainzelleninhalt;
- Hilfsmarkierungen beeinflussen Completion nicht und verhindern keine Sterne;
- Zeilen-/Spaltenanzeige meldet nur offen/exakt/überschritten aus sichtbarer Belegung;
- Completion verlangt exakt einen vollständigen A-B-Pfad mit allen konkreten Tracks;
- Schleifen, getrennte Komponenten, falsche Außenkanten und Anschlussmismatch scheitern;
- Batchcommands sind ganz oder gar nicht und bilden einen Undo-Schritt;
- No-op startet keinen Timer und erzeugt keinen Undo-Diff;
- Undo setzt sticky Meisterschaftsflags und gestarteten Timer nicht zurück;
- identische Commandfolgen ergeben identische kanonische Snapshots und Events;
- veraltete Revisionen und doppelte Command-IDs mutieren nicht erneut.

Tests verwenden Builder mit sinnvollen Defaults und nennen die fachliche Behauptung im Namen.

## 4. Solver- und Generatortests

| Testart | Verpflichtender Nachweis |
|---|---|
| Known cases | 0, 1 und 2+ Lösungen korrekt bis Limit zwei. |
| Cross-check | kleiner unabhängiger Enumerator stimmt auf kleinen Rastern mit Productionsolver überein. |
| Metamorphic | Rotation, Spiegelung und A/B-Tausch bewahren Lösungsklasse. |
| Determinism | Proof, Lösung und Metriken sind über Runs/Plattformen identisch. |
| Mutation | Entfernte Constraintregeln lassen mindestens einen Test scheitern. |
| Hint | jeder ausgegebene Hint ist aus dem öffentlichen Puzzle ohne Spielerannahmen wahr, kein Suchguess und enttarnt keine nicht objektiv fehlerhafte Annahme; andernfalls `NO_NON_REVEALING_HINT_AVAILABLE`. |
| Budget | kuratierte 10×10-Fälle bleiben innerhalb dokumentierter Grenzen. |
| Generator | fester Seed reproduziert Kandidat; Production akzeptiert nur Profilstatus `PRODUCT_APPROVED` mit exakt passendem Profil-/Korpus-/Solverhash. |

Der vollständige kuratierte Katalog wird bei jedem Release neu bis Lösungslimit zwei geprüft. Ein gespeicherter Proof allein genügt nicht.

## 5. Datenvertrags- und Contenttests

- jedes Level gegen JSON Schema Draft 2020-12;
- semantische Codes für jede Querschnittsregel;
- Hashprojektionen in C# und unabhängigem CI-Werkzeug identisch;
- unbekannte Eigenschaften wegen `additionalProperties: false` abgewiesen;
- bösartige Größe, Tiefe, Strings und polymorphe Typmetadaten abgewiesen;
- IDs und Contenthierarchie eindeutig/vollständig;
- veröffentlichter Puzzlehash unter stabiler Level-ID unveränderlich; Mutation blockiert Build und übernimmt keinen Fortschritt;
- Lokalisierungs- und Assetreferenzen auflösbar;
- zwei saubere Importe erzeugen denselben Kataloghash;
- Runtimeartefakt enthält keine unbeabsichtigte Authoringlösung;
- Pseudolocale und fehlende deutsche Keys geprüft;
- Golden-Änderungen werden als sichtbarer Diff reviewt.

Das Beispiel unter `ARCHITECTURE/examples/` wird bereits in der Architekturphase gegen Schema und Eindeutigkeit geprüft. Spätere Schemaimplementierungen übernehmen es als Golden Fixture.

## 6. Persistenz- und Migrationstests

| Risiko | Pflichtfälle |
|---|---|
| Teilwrite | Abbruch nach jedem Schritt des atomaren Algorithmus; höchste gültige Generation wird geladen. |
| Korruption | truncation, ungültiges JSON, falscher Hash, ungültiger Payload. |
| Backup | Hauptstand defekt, Backup gültig; Recovery ohne Fortschrittsverlust. |
| Split brain | gleiche Generation/anderer Hash; konservative Auswahl und Diagnose. |
| Migration | Golden vN→vN+1, Kette über alle Versionen, Idempotenz, Rollbacksource. |
| Downgrade | ältere App überschreibt neueren Save nicht. |
| Economy | doppelte Reward-/Stern-/Route-/Abschnitts-Callbacks sind No-op. |
| IAP | Owned-Cache bleibt bei Timeout; bestätigter Restore/Revocationpfad. |
| Clock | Uhr zurück, Zeitzone wechselt, Tagesgrenze, Apppause. |
| Ressourcen | niedriger Speicherplatz, I/O-Fehler, lange Pfade, Appkill. |

Gerätetests erzwingen Hintergrundwechsel und Prozesskill während Autosave und Abschlusscommit.

## 7. Application-Policy-Tests

Fortschrittstests prüfen Erstabschluss, Übungsfahrt, freigeschaltete Betriebsrevision, bessere Sterne, einmalige Nachzahlung und Dauerbaustellenfreigabe exakt gegen Produktregeln.

Adpolicy-Tests verwenden tabellarische Daten für Abschlussordinal 0–9+, Modus, Abstand, Sessioncount, `remove_ads`, Ergebnisphase, Consent, SDK-Bereitschaft und Fehler. Jede harte Ausschlusszone besitzt mindestens einen Negativtest. Kein Retry/No Fill erhöht Sessioncount.

Rewardtests prüfen Post-Clear +10 genau einmal, zusätzlichen Hintcredit und zusätzlichen Tageslauf. Gegenwert wird erst nach Rewardcallback und persistentem Commit sichtbar.

Solange `BLOCKER-PROD-001` beziehungsweise `BLOCKER-PROD-002` offen sind, müssen Productionprofile `EXTRA_HINT` und Betriebslage-des-Tages-Claims deaktivieren. Solange `BLOCKER-PROD-003` offen ist, muss jeder Generator-Productionimport fehlschlagen. Diese Fail-closed-Prüfungen sind selbst Merge- und Release-Gates.

## 8. Provider-Contracttests

Jede Implementierung von Ads, IAP, Consent, Analytics, Crash, Audio und Lifecycle muss dieselbe Suite gegen einen abstrakten Contract bestehen.

- jeder Aufruf endet in genau einem terminalen Ergebnis oder `PENDING`;
- Cancellation und Timeout liefern stabile Codes;
- späte/doppelte Callbacks sind idempotent;
- SDK-Exceptions verlassen den Adapter nicht;
- Unity-Objekte werden nur am Hauptthread berührt;
- deaktivierter/no-op Adapter erfüllt denselben Vertrag;
- keine nicht erlaubten Felder erreichen Analytics/Crash;
- Sandboxkonfiguration kann Production-IDs nicht verwenden.

Echte SDK-Smokes laufen nur mit Testanzeigen, Sandboxprodukten und Testgeräten.

## 9. Präsentations- und Accessibility-Tests

PlayMode-Tests prüfen Screenhierarchie, nur drei Hauptziele außerhalb des Puzzles, ausgeblendete Navigation/Währung/Ads im Rätsel, direkte Erreichbarkeit aller Werkzeuge, Auswahlsemantik, große Raster, Safe Areas und Abschlussreihenfolge.

UI Toolkit Querytests arbeiten über stabile semantische IDs, nicht Pixelkoordinaten. Wenige Visual-Regression-Snapshots decken 4×4, 10×10, klein/groß, Pseudolocale und Ergebnis ab; sie sind plattform-/rendererfest gepinnt und werden nicht blind neu aufgenommen.

Accessibility-Gates prüfen Fokusreihenfolge, Screenreaderlabels, Kontrast, dynamische Schrift, Information nicht nur durch Farbe, Touchziele und Möglichkeit, Zugfahrt gemäß späterer bestätigter UX-Option zu verkürzen. Offene Detailwerte werden nicht erfunden; Tests werden mit der späteren Produktentscheidung konkretisiert.

## 10. Performance- und Stabilitätstests

ADR-012 legt die Mindest-Geräteklassen fest. Das erste Implementierungs-Work-Package benennt und inventarisiert konkrete physische oder remote erreichbare Geräte, die diesen Vertrag erfüllen. Verbindliche technische Ziele:

| Metrik | Ziel |
|---|---|
| Puzzlecommand 10×10 | p95 < 4 ms auf niedrigstem Referenzgerät. |
| Completion 10×10 | p95 < 2 ms. |
| Solver Unique 10×10 CI | p95 < 250 ms, hart < 2 s. |
| Laufzeithint 10×10 | p95 < 100 ms, hart < 1 s. |
| Wiederholte Zelleingabe | keine regelmäßigen Frame-Spikes; Hotpath nach Warm-up ohne Presentation-Allokation. |
| Saveabschluss | kein sichtbarer Belohnungsverlust; I/O außerhalb Renderframe. |
| Memory | Low-Memory-Event gibt nicht aktive Addressables frei; Save bleibt intakt. |

Long-run-Smokes wiederholen Level öffnen/lösen/zur Karte 100-mal mit Fakes und prüfen Speichertrend, Eventsubscriptions und Handlefreigabe.

## 11. Build- und Release-Tests

| Gate | Pull Request | Main/Integrationsstand | Release Candidate |
|---|---:|---:|---:|
| Markdown-/Schema-/Architekturcontracts | Ja | Ja | Ja |
| Compile + EditMode | Ja | Ja | Ja |
| Content-/Solverkatalog | betroffener Umfang | vollständig | vollständig |
| PlayMode | Ja | Ja | Ja |
| Android Development IL2CPP | Ja | Ja | Ja |
| iOS Export/Compile | optional bei docs-only, sonst nightly | Ja | Ja |
| Gerätesmoke Android/iOS | bei Adapteränderung | nightly | Ja |
| Store-/Privacy-/Signing-Preflight | Nein | Stagingdryrun | Ja |
| TestFlight/Internal Track Smoke | Nein | Nein | Ja |
| Symbolisierter Testcrash | bei Crash-SDK-Änderung | Staging | Ja |

Documentation-only Pull Requests wie WP-ARCH-001 benötigen keinen Spielbuild, solange noch kein Produktionsprojekt existiert. Ihre eigenen Dokument-, Link- und Schema-Gates bleiben verbindlich.

## 12. Coverage, Mutation und Qualitätsmetriken

Line-/Branchcoverage wird berichtet, aber keine willkürliche Zahl ersetzt fachliche Tests. Mit dem ersten Code-Work-Package werden Baselines nach Assembly festgelegt; Domain, Solver, Policies und Migrationen erhalten besonders hohe Branch- und Mutationserwartungen. Ein Coverageabfall benötigt eine begründete Ausnahme.

Wichtiger als Gesamtprozent sind:

- jede Domaininvariante besitzt Positiv- und Negativtest;
- jede Migration besitzt Golden und Fehlerpfad;
- jeder externe Resultstatus ist getestet;
- jede Product-Guardrail besitzt mindestens einen automatisierten Gate-Test;
- Mutation Score für Domain/Solver bleibt oberhalb der im Scaffold akzeptierten Baseline.

## 13. Flaky-Test-Regel

Ein Test wird nicht durch automatisches Mehrfachretry „grün“. Flaky bedeutet Fehler. Er blockiert, bis Ursache behoben oder eine dokumentierte Quarantäne mit Issue, Eigentümer, Umfang und Ablaufdatum eingerichtet ist. Quarantänetests zählen nicht als erfülltes Release-Gate für das betroffene Risiko.

Zufallstests protokollieren Seed, Solverversion, Testfall und minimierten Gegenbeweis. CI verwendet feste Seeds plus rotierenden, gespeicherten Tagesseed nur als Zusatzsignal.

## 14. Testdaten und Goldens

Fixtures enthalten keine Produktionssecrets oder personenbezogene Daten. Goldens sind klein, lesbar und mit Generator-/Schema-/Solverversion markiert. Ihre Aktualisierung geschieht durch expliziten Befehl und wird getrennt vom Verhaltenscode reviewt. Ein Agent darf erwartete Outputs nicht ändern, nur damit ein fehlschlagender Test besteht.

## 15. Fehlerbehebungsvertrag

Jeder Productionbug erhält zuerst einen minimalen reproduzierenden Test auf der niedrigsten sinnvollen Ebene. Die Korrektur darf den Test nicht durch Sonderpfade umgehen. Postmortems für Saveverlust, falsche Economy, Storefehler, Contentmehrdeutigkeit und öffentliche Crashregression aktualisieren Tests und gegebenenfalls Architektur.

## Referenzen

[1]: ../DECISIONS/ADR-009-automatisierte-teststrategie.md "ADR-009 – Automatisierte Tests als Architekturgrenze"
[2]: ./PUZZLE_ENGINE.md "Puzzle Engine v0.1"
[3]: ./SOLVER_ARCHITECTURE.md "Solver Architecture v0.1"
[4]: ./PERSISTENCE.md "Persistence v0.1"
