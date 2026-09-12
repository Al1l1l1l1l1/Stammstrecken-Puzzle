# WP-005 – Architecture-v0.5-letzte-High-Korrekturen

## ID

`WP-005`

**Bearbeitungsstatus:** Abgeschlossen auf Branch `arch/architecture-v0.1`.

## Ziel

Die genau vier verbliebenen HIGH-Probleme `HIGH-1` bis `HIGH-4` werden ohne neue allgemeine Architekturreview-Runde vollständig geschlossen. Das Ergebnis heißt **Architecture v0.5**. Architecture v1.0 wird ausdrücklich nicht freigegeben.

WP-005 verändert keine Produktentscheidung, erzeugt keinen Produktionscode, löst keinen Produktfolgeblocker und refaktoriert keine bereits geschlossenen Architekturbereiche.

## Voraussetzungen

Vor Beginn gelten die vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v0.4, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md`, die für die vier HIGH-Probleme relevanten Architektur- und ADR-Dateien sowie der kanonische Architekturvalidator.

Ausgangspunkt ist der verifizierte Commit `cdf153c7ee1d96f03cfd5ea7445dc4f186c9ff5e` auf lokalem und entferntem Branch `arch/architecture-v0.1`. Lokaler `main` und `origin/main` sind zu Beginn identisch bei `3236334a65d0517c543af0366ba7bc4081bdc263`.

Der Documentation-Scope ist vor allen fachlichen Korrekturen im kanonischen Manifest [`tools/architecture-validation/scopes/WP-005.documentation.scope.json`](../tools/architecture-validation/scopes/WP-005.documentation.scope.json) verankert. Dieses Work Package und das Manifest müssen im selben Trust-Anchor-Commit eingeführt werden. Der `baseCommit` des Manifests ist dessen Elterncommit. Der Validator muss Work Package und Manifest aus genau diesem historischen Commit laden und ihre kanonische Bindung beweisen.

Der dokumentierte CI-Follow-up bleibt unverändert und ist vor dem ersten produktiven Coding-Work-Package zwingend.

## Scope

Erlaubt sind ausschließlich diese vier Korrekturen und zwingend notwendige konsistente Folgeänderungen:

1. **HIGH-1 Endless No-Reward-Terminalpfad:** Einen lokalen, providerfreien, crashsicheren und idempotenten Skip-/Nicht-beansprucht-Pfad für abgeschlossene Endless-Puzzles definieren und ausführbar prüfen. Er darf weder Reward noch spätere Doppelbelohnung ermöglichen und nicht in eine bereits gestartete oder reservierte Provideroperation eingreifen.
2. **HIGH-2 Cosmetics Claim Reservation Binding:** `COMMIT_CLAIM` zwingend an eine zuvor persistierte, eligibility-validierte und exakt passende Reservation binden. Claim-ID, Subject/Cosmetic-ID, Erwerbsart, Katalogrevision, Eligibility-/Progressnachweis, Status und eindeutige Operation-/Claimgeneration müssen übereinstimmen.
3. **HIGH-3 Rollout Reducer Semantik:** Eine automatische `ADVANCE`-Entscheidung nur bei vollständig vorhandenem, frischem, vollständigem, versionsgebundenem und populationsausreichendem Plattformnachweis zulassen. Jeder fehlende oder unzureichende Nachweis führt zu `PAUSE_NO_ADVANCE`.
4. **HIGH-4 WP-/Scope-Manifest-Ankerbindung:** Beim Scope-Check beweisen, dass Work Package und Manifest im selben Trust-Anchor-Commit existierten, das Work Package dort exakt dieses Manifest referenzierte und spätere Änderungen die historische Bindung nicht ersetzen können.
5. Architecture-, ADR-, Validator-, Work-Package- und Project-Control-Status konsistent auf **Architecture v0.5** aktualisieren.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` | Auftrag, Trust-Anchor-Verweis, Status, Acceptance Checks und Abschlussnachweise. |
| `tools/architecture-validation/scopes/WP-005.documentation.scope.json` | Vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunveränderlich. |
| `ARCHITECTURE/ARCHITECTURE.md`, `ARCHITECTURE/OPEN_BLOCKERS.md` | Architecture-v0.5-Navigation und unveränderte Produktblocker. |
| `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md` | Ausschließlich Endless-No-Reward-Pfad und gebundene Cosmetics-Reservation. |
| `ARCHITECTURE/CONTENT_CATALOGS.md` | Ausschließlich Cosmetics-Reservation-/Commitvertrag. |
| `ARCHITECTURE/BUILD_AND_RELEASE.md` | Ausschließlich vollständige Rollout-Reducer-Semantik. |
| `ARCHITECTURE/TEST_STRATEGY.md` | Ausschließlich Tests und Acceptance Checks der vier HIGH-Probleme. |
| `DECISIONS/ADR-023-releasekandidat-und-kosmetikclaims.md`, `DECISIONS/ADR-025-endless-open-lifecycle-und-claims.md`, `DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md` | Nur zulässige Nachfolgerverweise; keine rückwirkende Änderung angenommener Entscheidungskörper. |
| `DECISIONS/ADR-027-endless-no-reward-terminalpfad.md` | Neu: providerfreier lokaler Skip-/Terminalvertrag. |
| `DECISIONS/ADR-028-cosmetics-reservation-binding.md` | Neu: gebundene Cosmetics-Reservation und Ownership-Commit. |
| `DECISIONS/ADR-029-rollout-reducer-semantik.md` | Neu: vollständige Nachweisbasis für `ADVANCE`. |
| `DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Neu: gemeinsamer historischer WP-/Manifest-Trust-Anchor. |
| `DECISIONS/README.md` | ADR-Index, Status und Superseding-Verweise. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Architecture-v0.5-Abschluss und unveränderter CI-Follow-up. |
| `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md` | Ausschließlich ausführbare Semantik, Scope-Ankerprüfung, Bericht und Negativmutationen für HIGH-1 bis HIGH-4. |
| `tools/architecture-validation/fixtures/endless-save-v2.example.json` | Lokaler Skip-, Restart-, Duplicate- und Konfliktszenarien. |
| `tools/architecture-validation/fixtures/cosmetics-lifecycle-v1.json` | Persistierte Reservation und gebundene Commit-/Negativszenarien. |
| `tools/architecture-validation/fixtures/rollout-metric-v1.json` | Beobachtungsfenster, Population, Freshness, Completeness und Buildbindung. |
| `tools/architecture-validation/fixtures/review-contracts-v0.5.json` | Neu: nicht normative strukturierte Projektion der vier HIGH-Verträge. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind eine neue allgemeine Architekturreview-Runde, Nice-to-have-Themen, Refactorings geschlossener Bereiche, neue Produktentscheidungen, Produktionscode, Unity-Projektdateien, Konzeptdateiänderungen, konkrete Items oder Meilensteine, konkrete Season-1-Puzzles, Änderungen an `main`, Merge oder automatische Pull-Request-Erstellung.

`BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed. Der CI-Follow-up bleibt bestehen. Architecture v1.0 darf nicht ausgerufen werden.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Ein abgeschlossenes Endless-Puzzle kann ohne Ad-Anfrage und ohne Provider-ID lokal als nicht beansprucht terminalisiert werden; es entsteht keine Belohnung und kein offener Claimrecord bleibt zurück. |
| `AK-02` | Lokaler Skip ist crashsicher und idempotent; Duplicate Skip ist No-op. Skip nach bestätigtem Reward oder während gestarteter/reservierter Provideroperation wird abgewiesen und kann keine spätere Doppelbelohnung erzeugen. |
| `AK-03` | Mindestens 100 aufeinanderfolgende Endless-Abschlüsse mit lokalem Skip erzeugen keine Capacity-Sperre und keine offenen Claimrecords. |
| `AK-04` | Cosmetics `COMMIT_CLAIM` ist nur aus einer tatsächlich zuvor persistierten, eligibility-validierten, nicht verbuchten und exakt passenden Reservation zulässig. |
| `AK-05` | Commit ohne Reservation, gefälschte Reservation ohne Eligibility, andere Cosmetic-ID, andere Katalogrevision und nachträglich unpassende Eligibility scheitern; bereits verbuchter Claim bleibt idempotent ohne doppelte Ownership. |
| `AK-06` | Rollout `ADVANCE` verlangt erfülltes Beobachtungsfenster, korrekte Plattform, Sessions, erforderliche Active Devices, vorhandene Crashmetrik, ausreichende Freshness, vollständiges Reporting, Mindestpopulation und exakte Version-/Buildbindung. |
| `AK-07` | Jeder fehlende, veraltete, unvollständige oder unter Mindestbasis liegende Rolloutnachweis ergibt `PAUSE_NO_ADVANCE`; vollständige gute Daten erlauben die bereits dokumentierte positive Entscheidung. |
| `AK-08` | Scope-Prüfung lädt WP und Manifest aus demselben Trust-Anchor-Commit und beweist die dortige exakte kanonische Verknüpfung. |
| `AK-09` | Später hinzugefügtes WP/Manifest, falscher oder später ergänzter Verweis, falsche WP-ID, anderes Manifest, externer Pfad und nachträglich mutiertes Manifest werden abgewiesen; spätere legitime Production-Manifeste bleiben möglich. |
| `AK-10` | Validatorausgaben behaupten nur tatsächlich ausgeführte Belege. Architecture-, ADR-, Project-Control- und Work-Package-Status sind konsistent Architecture v0.5. |
| `AK-11` | Die drei Produktfolgeblocker und der CI-Follow-up bleiben unverändert. Keine Produktdatei, kein Produktionscode und kein `main` werden verändert. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator, Documentation-Scope-Validator und vollständiger Self-Test aus der gepinnten Lock-Umgebung;
2. Acceptance A: 100 aufeinanderfolgende Endless-Completions ohne Rewarded-Ad-Interaktion mit lokalem Skip, Restart/Resume, Duplicate Skip, Skip nach Reward und Skip während reservierter Provideroperation;
3. Acceptance B: Cosmetics-Commit ohne valide vorherige Eligibility-Reservation sowie alle geforderten Bindungsmutationen;
4. Acceptance C: Rolloutnegativtests für zu wenig iOS Active Devices, unvollständiges Beobachtungsfenster, stale Daten, unvollständiges Reporting, fehlende Metrik, falsche Buildbindung und unzureichende Android-Freshness sowie ein positiver Vollnachweis;
5. Acceptance D: historische gemeinsame WP-/Manifest-Ankerbindung und Manipulationsgegenproben für alle geforderten Fehlformen;
6. Schema-, Golden-, Hash- und Migrationstests des bestehenden Validators ohne Absenkung;
7. `git diff --check`, vollständiger Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck;
8. vollständige Delta-Prüfung gegen `cdf153c7ee1d96f03cfd5ea7445dc4f186c9ff5e`;
9. erneuter vollständiger Lauf auf dem konkreten Abschlusscommit vor Push und Remote-Commit-Verifikation nach Push.

Unity Compile/EditMode/PlayMode, IL2CPP, physische Geräte-, SDK- und Storetests bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**. Der GitHub-Actions-Nachweis bleibt ein separates zwingendes CI-Follow-up vor Produktionscoding.

## Risikoklasse

**Hoch.** Die Korrekturen betreffen crashsichere Persistenz, monetarisierungsnahe Reward- und Ownershiptransaktionen, automatische Releasefreigabe sowie die Vertrauenswürdigkeit des Scope-Gates.

## Definition of Done

WP-005 ist nur abgeschlossen, wenn alle vier HIGH-Probleme einzeln `CLOSED` sind, Acceptance Checks A bis D bestanden haben, beide Validator-Modi und alle Self-/Negativtests grün sind, der vollständige Diff ausschließlich die vorab erlaubten Dateien enthält und der Abschlusscommit auf `arch/architecture-v0.1` gepusht und remote verifiziert wurde.

Jede materielle Korrektur einer angenommenen Architekturentscheidung steht in einem nachfolgenden ADR. `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `DECISIONS/README.md`, dieses Work Package und die Architekturnavigation geben denselben Architecture-v0.5-Stand wieder. Es erfolgt kein Merge und keine Änderung an `main`.

## Ergebnis

Architecture v0.5 schließt exakt die vier beauftragten HIGH-Probleme. Es wurde kein Produktionscode erzeugt, keine Produktdatei geändert und kein Produktfolgeblocker gelöst. Architecture v1.0 wird nicht ausgerufen.

| Finding | Status | Abschlussnachweis |
|---|---|---|
| `HIGH-1` Endless No-Reward-Terminalpfad | **CLOSED** | `LOCAL_DECISION_PENDING` kann ohne Provider-ID oder Ledgerdelta per lokalem Savecommit terminalisiert werden. Savegeneration-CAS, Providerreservation vor SDK-Aufruf, Crash/Restart, Duplicate, später Callback, Skip nach Reward und 100 aufeinanderfolgende lokale Skips sind ausführbar geprüft. |
| `HIGH-2` Cosmetics Claim Reservation Binding | **CLOSED** | `COMMIT_CLAIM` verlangt eine zuvor persistierte, eligibility-/progress-validierte Reservation und bindet Claim, Operation, Generation, Item, Erwerbsart, Katalogsnapshot und Eligibility-Projektion. Fehlende, gefälschte oder abweichende Reservationen scheitern; Replay bleibt einmalige Ownership. |
| `HIGH-3` Rollout Reducer Semantik | **CLOSED** | Android- und iOS-Evidenzobjekte binden Plattform, Quelle, Metrik, Release, Build, Stufe, Fenster, Freshness, Reporting, Reviewer und plattformspezifische Population. Jede fehlende oder unzureichende Dimension liefert `PAUSE_NO_ADVANCE`. |
| `HIGH-4` WP-/Scope-Manifest-Ankerbindung | **CLOSED** | WP-005 und Manifest wurden gemeinsam in Commit `ebf522a9ef3c28035341cc04dbd6d8251b107603` eingeführt. Der Validator lädt beide historischen Blobs, verlangt Add-Status im selben Commit, prüft Elternabwesenheit, IDs, exakten historischen Link und aktuelle Manifestbytegleichheit. |

## Acceptance Checks A–D

| Check | Ergebnis |
|---|---|
| A – Endless ohne Rewarded Ad | **PASS.** 100 lokale Skipzyklen enden ohne Capacityfehler, offenen Record oder Economyänderung; Restart, Duplicate, Provider-Race und Skip nach Reward sind abgedeckt. |
| B – Cosmetics ohne valide Reservation | **PASS.** Direkter Commit, gefälschte Reservation, falsche Claim-/Item-/Erwerbsart-/Katalog-/Operations-/Generations-/Eligibility-Bindung und nachträglich fehlende Eligibility werden abgewiesen. |
| C – unvollständige Rollout-Evidenz | **PASS.** Fehlendes Fenster, Active Devices, Freshness, Reporting, Metrik, Reviewer, Population oder Release-/Buildbindung pausiert; vollständige Android-/iOS-Daten erlauben die dokumentierte positive Entscheidung. |
| D – historischer Trust-Anchor | **PASS.** Späteres WP/Manifest, späterer oder falscher Link, falsche ID, anderes Manifest, absoluter/externer Pfad und mutierter Manifestblob werden abgewiesen; eigenes späteres Production-WP/-Manifest bleibt möglich. |

## Review- und Beleggrenzen

Ein unabhängiger Principal-Architecture-Recheck bestätigte nach der letzten HIGH-2-Korrektur die Erwerbsartbindung und den vollständigen Self-Test ohne Abbruch. Automatisch belegt sind ausschließlich lokale Dokumentstruktur, Architektursemantik, Fixtures, Reducer, Mutationen und Git-Scope. IAP-Dokumentreihenfolge bleibt `MANUAL_ARCHITECTURE_REVIEW`; Unity-/Produktionscode-, Geräte- und Storetests bleiben `REQUIRED_LATER/NOT_EXECUTED`. Die drei Produktfolgeblocker bleiben `BLOCKED` und fail-closed.

| Ausgeführter Nachweis | Ergebnis |
|---|---|
| Architecture-only mit `--self-test` | **PASS**, 17 lokale Prüfgruppen. |
| Documentation-Scope mit WP-005-Manifest und `--self-test` | **PASS**, 18 lokale Prüfgruppen einschließlich `LOCAL_SCOPE`. |
| Unabhängiger Recheck der HIGH-2-Erwerbsartmutation | **PASS**; abweichendes `PATIENCE_PURCHASE` gegen persistiertes `MILESTONE_GRANT` ergibt `cosmetics:commit-binding`. |
| Delta-/Whitespace-/JSON-/Secret-/Produktcodewachen | **PASS** vor dem Abschlusscommit; nach jedem nachfolgenden Dokumentationsschritt erneut auszuführen. |

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture"
[6]: ../DECISIONS/README.md "Architekturentscheidungen – verbindlicher ADR-Index"
