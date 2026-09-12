# Test Strategy v0.4

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
- A und B dürfen verschiedene Außenanschlüsse derselben Zelle sein; das Ein-Zellen-Fixture löst exakt mit einer Trackzelle;
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
| Determinism | Gebundenes `proof-v1`, Lösung und Metriken sind über Runs/Plattformen identisch. |
| Mutation | Entfernte Constraintregeln lassen mindestens einen Test scheitern. |
| Hint | jeder ausgegebene Hint ist aus dem öffentlichen Puzzle ohne Spielerannahmen wahr, kein Suchguess und enttarnt keine nicht objektiv fehlerhafte Annahme; andernfalls `NO_NON_REVEALING_HINT_AVAILABLE`. |
| Budget | kuratierte 10×10-Fälle bleiben innerhalb dokumentierter Grenzen. |
| Generator | fester Seed reproduziert Kandidat; Production akzeptiert nur Profilstatus `PRODUCT_APPROVED` mit exakt passendem Profil-/Korpus-/Solverhash. |
| Endless identity/lifecycle | Generatorversion, Seed, Ordinal und Parameterhash reproduzieren dieselbe `E1-`-ID; Watermark plus höchstens 20 offene Reservation-/Draft-/Claimrecords bleiben nach 10.000 gemischten Transitions konstant begrenzt; Crash vor/nach Generation, Claimreconciliation, Resume-Lücken und terminale Duplicates werden korrekt klassifiziert. |

Der vollständige kuratierte Katalog wird bei jedem Release neu bis Lösungslimit zwei geprüft. Ein gespeicherter Proof allein genügt nicht.

## 5. Datenvertrags- und Contenttests

- jedes neue Level gegen `level-v2`, Legacyfixtures zusätzlich gegen `level-v1`; jeder v1-Positivfixture muss durch den neutralen Migrator auch v2-Schema und -Semantik bestehen;
- semantische Codes für jede Querschnittsregel;
- Hashprojektionen in C# und unabhängigem CI-Werkzeug identisch;
- unbekannte Eigenschaften wegen `additionalProperties: false` abgewiesen;
- bösartige Größe, Tiefe, Strings und polymorphe Typmetadaten abgewiesen;
- IDs und Contenthierarchie eindeutig/vollständig;
- Campaign-, Completion- und Cosmetics-Kataloge gegen eigene Schemata, Freigabestatus, Revision, Hash und Cross-Reference-Reihenfolge;
- Cosmetics-Preis stammt ausschließlich aus dem release-gelockten Katalog; Duplicate Purchase und Already Owned buchen nie doppelt ab;
- veröffentlichter profilierter semantischer Puzzlehash unter stabiler `puzzleId` unveränderlich; Dokumentmigration und Proofregeneration bleiben bei identischer Semantik zulässig;
- `proof-v1` bindet Puzzle-ID, profilierten Puzzlehash, Lösungshash, Solverversion und Metriken; Cross-Puzzle-Copy und stale Proof scheitern;
- Lokalisierungs- und Assetreferenzen auflösbar;
- zwei saubere Importe erzeugen denselben Kataloghash;
- Runtimeartefakt enthält keine unbeabsichtigte Authoringlösung;
- Pseudolocale und fehlende deutsche Keys geprüft;
- Levelmigrations-Goldens binden vollständige JCS-Dokumenthashes der tatsächlichen v1-Quelle, v2-Zieldatei und Release-Lock-Datei;
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
| Save hash | RFC-8785-Bytes, UTF-8 ohne BOM/Newline, Unicode/Integergrenzen, Duplicate-Key-Ablehnung in beiden Parsern, unabhängiger Cross-Tool-Golden und Hashprofildispatch. |
| Economy | doppelte Reward-/Stern-/Route-/Abschnitts-Callbacks sind No-op; Checkpoint/Journalsaldo und Hashkette stimmen. |
| Compaction | Schwellen, harte Grenzen, terminale Fachrecords, Crash vor/nach atomarem Commit und Migration. |
| Endless Save v2 | Watermarkinvarianten, offene Höchstzahl 20, Reserve vor Generierung, deterministische Regeneration, Promotion, Resume, Complete zu offenem Claim, Claim-/Puzzle-/Providerbindung, katalogautoritatives Reward, Claimcommit/Close, Abandon, kein Terminaldetail als Wahrheit, 10.000 Transitions und v1→v2-Claimgolden. |
| IAP | Belegprüfung, atomarer Grant vor Acknowledge/Finish, Phasencrash, Retry, Restore, widersprüchliche Antwort und bestätigte Revocation. |
| Clock | Uhr zurück, Zeitzone wechselt, Tagesgrenze, Apppause. |
| Ressourcen | niedriger Speicherplatz, I/O-Fehler, lange Pfade, Appkill. |

Gerätetests erzwingen Hintergrundwechsel und Prozesskill während Autosave und Abschlusscommit.

## 7. Application-Policy-Tests

Fortschrittstests prüfen Erstabschluss, Übungsfahrt, freigeschaltete Betriebsrevision, bessere Sterne, einmalige Nachzahlung und Dauerbaustellenfreigabe exakt gegen Produktregeln.

Adpolicy-Tests verwenden tabellarische Daten für Abschlussordinal 0–9+, Modus, Abstand, Sessioncount, `remove_ads`, Ergebnisphase, Consent, SDK-Bereitschaft und Fehler. Jede harte Ausschlusszone besitzt mindestens einen Negativtest. Kein Retry/No Fill erhöht Sessioncount.

Rewardtests prüfen `POST_CLEAR_PATIENCE` über die fachliche Claim-ID exakt einmal je Meldung: wiederholte und parallele Anfragen, Crash nach Reservation, Rewardcallback vor/nach Close, später Callback nach Freigabe, Reconciliation und atomarer Claim-plus-Ledgercommit. Negativmutationen decken falschen Betrag, fehlende/falsche Claim-ID, unzulässigen Claimstatus, Commit ohne bestätigtes Rewardergebnis und späten Callback ab. Provider-IDs werden variiert und dürfen keinen zweiten Grant erlauben; der Betrag wird ausschließlich aus dem gelockten Completionkatalog gelesen. Zusätzlicher Hintcredit und Tageslauf bleiben bis zur jeweiligen Produktfreigabe deaktiviert.

Kosmetikkauftests prüfen ausreichenden/ungenügenden Saldo, atomaren Debit-plus-Ownership-Commit, Duplicate Purchase, Already Owned, parallele Commands, Kataloghashwechsel, DRAFT-/Hidden-/Tombstone-Ablehnung und Save-Migration. Meilensteintests prüfen Campaign-Subject-Expansion, First-Clear-Eligibility, atomaren Claim-plus-Ownership-Commit ohne Ledgerdelta, Reserved-Crash-Resume, Replay, Claimkollision, erlaubte/verbotene Statustransitionen, Revision/Tombstone und Crash vor/nach Commit.

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
- effektive Ads-/Analytics-/Crashcapabilities starten immer `false`; `UNKNOWN`, `ENABLE_PENDING`, `REVOKE_PENDING`, fehlende oder inkompatible Entscheidung und Consentfehler aktivieren kein SDK;
- der prä-SDK-Deny-Fence läuft vor jeder optionalen SDK-Erfassung; direkte Legacy-Upgrades und ein installierter, nie gestarteter Zwischenbuild bleiben ohne Fence fail-closed;
- Widerruf persistiert `REVOKE_PENDING` vor Native Disable und `REVOKED_CONFIRMED` erst nach Bestätigung; Crash nach jedem Schritt ist recoverbar;
- Crashlytics fehlt im Productionpaket; IAP initialisiert weder im Bootstrap noch ohne Readiness und Nutzeraktion/Recovery;
- Sandboxkonfiguration kann Production-IDs nicht verwenden.

Echte SDK-Smokes laufen nur mit Testanzeigen, Sandboxprodukten und Testgeräten.

## 9. Präsentations- und Accessibility-Tests

PlayMode-Tests prüfen Screenhierarchie, nur drei Hauptziele außerhalb des Puzzles, ausgeblendete Navigation/Währung/Ads im Rätsel, direkte Erreichbarkeit aller Werkzeuge, Auswahlsemantik, große Raster, Safe Areas und Abschlussreihenfolge.

UI Toolkit Querytests arbeiten über stabile semantische IDs, nicht Pixelkoordinaten. Wenige Visual-Regression-Snapshots decken 4×4, 10×10, klein/groß, Pseudolocale und Ergebnis ab; sie sind plattform-/rendererfest gepinnt und werden nicht blind neu aufgenommen.

Accessibility-Gates prüfen Fokusreihenfolge, Screenreaderlabels, Kontrast, dynamische Schrift, Information nicht nur durch Farbe, Touchziele und Möglichkeit, Zugfahrt gemäß späterer bestätigter UX-Option zu verkürzen. Offene Detailwerte werden nicht erfunden; Tests werden mit der späteren Produktentscheidung konkretisiert.

## 10. Performance- und Stabilitätstests

ADR-012 legt die Mindest-Geräteklassen fest. Das erste Implementierungs-Work-Package benennt und inventarisiert konkrete physische Geräte oder eine ausdrücklich freigegebene Device-Farm mit physischen Geräten. Emulatoren/Simulatoren ergänzen schnelle Tests, gelten aber weder als physisches Gerät noch als bestandener Geräte-/Privacy-Smoke. Verbindliche technische Ziele:

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

Der Privacy-Smoke folgt exakt `MOBILE_SERVICES.md`: Fresh Install, direkter Sprung von jedem unterstützten Legacy-Build mit früher aktivem Analytics-Override, installierter aber nie gestarteter Reset-only-Zwischenbuild, Widerruf mit Crashinjektion nach jedem persistenten Schritt, Recovery, Re-enable-Versuch, Restart und Offline. Für Android und iOS werden physisches Gerätemodell, OS, Buildhash, native Konfigurationshashes, Capturetool/-regelversion und Capturehash archiviert. Unerklärter Traffic blockiert; Simulator-/Emulatorcapture allein kann das Gate nicht erfüllen.

## 11. Build- und Release-Tests

### 11.1 Maschinenprüfbarer Scope

Jeder **Work-Package-Abnahmelauf** benötigt `--scope documentation|production` und `--scope-manifest <json>`. Ein Architecture-only-Lauf ohne diese Argumente bleibt für allgemeine Diagnose zulässig, ist aber kein Scope- oder Abschlussbeleg. Das Manifest liegt exakt unter `tools/architecture-validation/scopes/<WP-ID>.<scope>.scope.json`, wird zusammen mit dem Work Package vor den fachlichen Änderungen in einem eigenen Ankercommit eingeführt und ist danach byteunveränderlich. Sein `baseCommit` ist der Elterncommit des Ankers. Der Validator liest den autoritativen Blob aus diesem Git-Commit, prüft die aktuelle Bytegleichheit und berechnet die reale Diffmenge inklusive Rename-/Copy-Endpunkten und untracked Dateien.

Muster verwenden segmentierte POSIX-Semantik: `*` matcht niemals `/`; `**` ist nur als eigenes Segment zulässig. Globale oder global-äquivalente Muster wie `*`, `**`, `**/*` und `*/**`, absolute/externe/unversionierte Manifestpfade, `..`, Symlink-Escape, falsche WP-/Scope-Namen und jeder Pfad außerhalb der Allowlist werden abgewiesen. `documentation` scheitert insbesondere bei `Assets/**`, `Packages/**`, `ProjectSettings/**` oder anderem Produktcode; **nur** ein vorab verankertes Production-Manifest kann solche Pfade erlauben. Eine Umgebungsvariable darf den Scope nicht erweitern.

Der kanonische WP-004-Befehl lautet:

```bash
python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-004.documentation.scope.json \
  --self-test
```

Ein später autorisierter GitHub-Workflow verwendet denselben kanonischen Manifestpfad und muss den verankerten Basecommit bestätigen. Ein fehlender oder nicht erreichbarer Basecommit, ein geänderter Manifestblob oder ein nicht passender Ankercommit ist ein harter Fehler. Das noch fehlende CI-Setup bleibt als non-blocking Follow-up vor dem ersten produktiven Coding-Work-Package dokumentiert.

### 11.2 Belegkategorien

| Kategorie | Bedeutung |
|---|---|
| **LOCAL_DOCUMENT_STRUCTURE PASS** | Im aktuellen Repository ausgeführte Inventar-, Governance-, Link- oder Statusprüfung. |
| **LOCAL_ARCHITECTURE_SEMANTICS PASS** | Im aktuellen Repository ausgeführtes Schema-, Fixture-, Modell- oder Mutationsgate. |
| **MANUAL_ARCHITECTURE_REVIEW** | Verbindliche, aber nur textuell/manuell geprüfte Reihenfolge oder Systembehauptung; ausdrücklich kein automatischer PASS. |
| **LOCAL_SCOPE PASS** | Reale Git-Diffmenge bestand das versionierte Work-Package-Manifest. |
| **CONTRACT_ONLY** | Schema, Fixture oder Dokumentvertrag vorhanden; noch kein Produktionscodebeleg. |
| **REQUIRED_LATER/NOT_EXECUTED** | Verbindliches Gate für Scaffold, Gerät oder Store, das mangels Artefakt/Zugang noch nicht laufen kann. |
| **BLOCKED** | Benötigt offene Produktentscheidung, Credential oder externe Freigabe. |

Ein Architekturvalidator darf eine Dokumentphrase, Fixtureprojektion oder Mutationsprüfung nie als ausgeführten Unity-, C#-, IL2CPP-, Geräte-, SDK-, Sandbox- oder Storetest darstellen. Die IAP-Fixtureprüfung darf als begrenzte `LOCAL_ARCHITECTURE_SEMANTICS` gemeldet werden; die Vollständigkeit der normativen IAP-Dokumentreihenfolge bleibt separat `MANUAL_ARCHITECTURE_REVIEW`. Abschlussberichte listen alle sieben Kategorien getrennt.

| Gate | Pull Request | Main/Integrationsstand | Release Candidate |
|---|---:|---:|---:|
| Markdown-/Schema-/Architekturcontracts | Ja | Ja | Ja |
| Compile + EditMode | Ja | Ja | Ja |
| Content-/Solverkatalog | betroffener Umfang | vollständig | vollständig |
| PlayMode | Ja | Ja | Ja |
| Android Development IL2CPP | Ja | Ja | Ja |
| iOS Export/Compile | optional bei docs-only, sonst nightly | Ja | Ja |
| Physischer Gerätesmoke Android/iOS | REQUIRED_LATER/NOT_EXECUTED ohne Scaffold/Hardware | geplant auf Device-Farm | Ja |
| Store-/Privacy-/Signing-Preflight | Nein | Stagingdryrun | Ja |
| TestFlight/Internal Track Smoke | Nein | Nein | Ja |
| Symbolisierter Testcrash | bei Crash-SDK-Änderung | Staging | Ja |

Documentation-only-Arbeiten wie `WP-001` bis `WP-004` benötigen keinen Spielbuild, solange noch kein Produktionsprojekt existiert. Ab WP-004 müssen sie mit ihrem vorab verankerten Documentation-Scope-Manifest laufen und dürfen Unity-/Geräte-/Storetests nur als REQUIRED_LATER/NOT_EXECUTED ausweisen. Historische Manifeste werden nicht rückwirkend als nach ADR-026 verankert ausgegeben.

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

Der Architekturvalidator liegt dauerhaft unter `tools/architecture-validation/`. Setup, gepinnte Abhängigkeiten und Einstiegspunkt stehen in seiner README. `--self-test` führt positive Repositoryprüfungen und gezielte Negativmutationen für Segmentglob/Scopeanker, Work-Package-ID, ADR-Historie/-Indexstatus, projektweite v0.4-Statuskonsistenz, Bootstrapkante, vollständige Levelmigration, Rewardclaim, strukturierte IAP-Fixture, Privacy-v2-Lifecycle, Kataloge, Season-1-Grenzen, Sternschwellen, Ledgerkompaktierung, Endless-Open-Lifecycle, Puzzle-/Proofbindung, Releaseidentität, Cosmetics-DRAFT/Eligibility/Transitionen, Rolloutmetrik und Ein-Zellen-Pfad aus. Eine nicht erkannte Mutation ist ein fehlgeschlagenes Gate.

## 15. Fehlerbehebungsvertrag

Jeder Productionbug erhält zuerst einen minimalen reproduzierenden Test auf der niedrigsten sinnvollen Ebene. Die Korrektur darf den Test nicht durch Sonderpfade umgehen. Postmortems für Saveverlust, falsche Economy, Storefehler, Contentmehrdeutigkeit und öffentliche Crashregression aktualisieren Tests und gegebenenfalls Architektur.

## Referenzen

[1]: ../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[2]: ./PUZZLE_ENGINE.md "Puzzle Engine v0.3"
[3]: ./SOLVER_ARCHITECTURE.md "Solver Architecture v0.4"
[4]: ./PERSISTENCE.md "Persistence v0.4"
[5]: ./MOBILE_SERVICES.md "Mobile Services v0.4"
