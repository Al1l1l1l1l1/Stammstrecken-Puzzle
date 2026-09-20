# Architecture Validation Tool

## Zweck

Dieses Verzeichnis enthält den **einzigen autoritativen, eingecheckten und repositoryrelativen Validator** für Architecture v1.0. Das Werkzeug prüft Dokumentstruktur, Datenverträge, ausführbare Architekturmodelle und den Git-Scope. Es ist Governance-/Contract-Tooling und kein Spielproduktionscode.

## Voraussetzungen

| Voraussetzung | Vertrag |
|---|---|
| Betriebssystem | Linux oder macOS; die spätere CI-Umgebung verwendet Ubuntu 24.04. |
| Python | CPython 3.11.x mit `venv` und `pip`. |
| Node.js | Node.js 22.x für den unabhängigen JCS-Crosscheck. |
| Git | Vollständiger Checkout; Basis- und gemeinsamer WP-/Manifest-Ankercommit müssen lokal erreichbar sein. |
| Netzwerk | Nur zur Installation der exakt gepinnten Pythonpakete; der Validatorlauf selbst ist offline. |

Die Pythonabhängigkeiten stehen versionsgepinnt in [`requirements.lock.txt`](./requirements.lock.txt). Node verwendet ausschließlich Standardmodule.

## Sauberes Setup

```bash
python3 -m venv ../.venv-stp-architecture
../.venv-stp-architecture/bin/python -m pip install --disable-pip-version-check \
  -r tools/architecture-validation/requirements.lock.txt
```

## Kanonischer WP-005-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-005.documentation.scope.json \
  --self-test
```

`--scope` und `--scope-manifest` sind gemeinsam für jede Work-Package-Abnahme Pflicht. Der Manifestpfad muss repositoryrelativ sein. `validate.py --self-test` ohne beide Argumente ist als Architecture-only-Diagnoselauf zulässig, belegt aber keinen Git-Scope. Ein gültiger Abschluss endet mit Exitcode `0` und `RESULT PASS`. Ohne `--self-test` fehlt der Mutationsnachweis.

## Kanonischer WP-006-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-006.documentation.scope.json \
  --self-test
```

## Kanonischer WP-007-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-007.documentation.scope.json \
  --self-test
```

## Kanonischer WP-008-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope production \
  --scope-manifest tools/architecture-validation/scopes/WP-008.production.scope.json \
  --self-test
```

WP-008 ist das erste Production-Scope-Work-Package: Es verankerte `WP-008.production.scope.json` und sein Work Package gemeinsam im Trust-Anchor `8d3e245fcab8bce42b5e8efdded480f47dacfbb8` (Elterncommit = `baseCommit` = `3c1a6988c1aab2084763edf772b6adc268874865`) und stellte den CI-Scope-Lauf in `.github/workflows/validate.yml` auf `STP_SCOPE: production` mit diesem Manifest um.

## Autorisierte CI-Integration (WP-007)

Der Workflow [`../../.github/workflows/validate.yml`](../../.github/workflows/validate.yml) (`Architecture Validation`, Check `Architecture Validation / validate`) ist die autorisierte CI-Integration dieses Validators. Er läuft bei Pull Requests gegen `main`, bei Pushes auf Branches außer `main` und manuell (`workflow_dispatch`) auf `ubuntu-24.04` mit CPython 3.11.13 und Node.js 22.20.0. Alle Actions sind per vollständigem Commit-SHA gepinnt, die Python-Abhängigkeiten kommen exakt aus `requirements.lock.txt`, und das `GITHUB_TOKEN` ist auf `contents: read` begrenzt.

Der Workflow führt den Architecture-only-Lauf mit `--self-test` auf jedem Ereignis aus. Den kanonischen Scope-Lauf mit `--self-test` führt er bei Pull Requests und bei Pushes außerhalb von `main` gegen das in `STP_SCOPE_MANIFEST` benannte Manifest aus; auf `main`-Pushes entfällt der Scope-Schritt, weil der Integrationsstand keinem einzelnen Work-Package-Diff mehr entspricht. Jedes künftige Work Package setzt `STP_SCOPE_MANIFEST` auf sein eigenes verankertes Manifest und nimmt die Workflowdatei in seine Allowlist auf. Jeder Validatorfehler lässt den Check fail-closed fehlschlagen.

## Scope-Vertrauensanker

Das Manifest folgt [`scope-manifest-v1.schema.json`](./scope-manifest-v1.schema.json) und liegt exakt unter `tools/architecture-validation/scopes/<WP-ID>.<scope>.scope.json`. **Manifest und zugehöriges Work Package müssen im selben historischen Add-Commit erstmals eingeführt werden.** Sein `baseCommit` ist der Elterncommit dieses gemeinsamen Ankers.

Der Validator lädt Manifest und Work Package aus genau diesem Commit. Er verlangt für beide Pfade Add-Status, prüft, dass sie im Elterncommit noch nicht existierten, und verifiziert im historischen WP-Blob die übereinstimmende H1-/Body-ID sowie einen exakt auf dieses kanonische Manifest auflösenden repositorylokalen Markdownlink. Der aktuelle Work-Package-Inhalt ist dafür keine Evidenz; ein später ergänzter Link oder ein später hinzugefügtes Work Package scheitert. Der historische Manifestblob muss außerdem bytegleich mit der aktuellen Datei sein.

Die reale Änderungsmenge stammt aus `git diff --name-status <baseCommit> --` einschließlich beider Rename-/Copy-Endpunkte und untracked Dateien. Muster verwenden segmentierte POSIX-Semantik: `*` matcht kein `/`, `**` ist nur als eigenes Segment erlaubt. Globale beziehungsweise global-äquivalente Muster (`*`, `**`, `**/*`, `*/**`), absolute oder externe Manifestpfade, unversionierte Manifeste, falsche WP-/Scope-Dateinamen, `..`, Backslashes, Symlink-Escape und nicht erlaubte Dateien scheitern. Das Manifest muss sich selbst ausdrücklich nennen und darf keine anderen Scope-Manifeste erlauben. Documentation-Scope lehnt Produktcode, Unityartefakte und Produktquellen zusätzlich kategorisch ab.

Ein späteres Production-Manifest ist zulässig, wenn es mit seinem eigenen Work Package denselben gemeinsamen-Anker-Vertrag erfüllt und einen engen Production-Scope besitzt. Eine spätere autorisierte CI-Integration muss denselben kanonischen Befehl und denselben Manifestanker verwenden. Der fehlende GitHub-Actions-Workflow ist für WP-005 **non-blocking with follow-up**; ein eigenes CI-Setup-Work-Package ist jedoch zwingend vor dem ersten produktiven Coding-Work-Package abzuschließen. Dieses zwingende CI-Setup-Work-Package wurde mit `WP-007` abgeschlossen; die autorisierte CI-Integration ist im eigenen Abschnitt oben beschrieben.

## Tatsächlicher Prüfumfang

| Gruppe | Tatsächlicher Nachweis |
|---|---|
| Inventar/Governance | v1.0-Dokumente, Schemata, Fixtures, 30 ADRs, acht Work Packages, Tooldateien sowie das WP-008-Produktionsscaffold-Inventar (ProjectVersion, Paketlocks, Toolchain-Lock, Unity-CI-Workflow und die vierzehn normativen `.asmdef`-Dateien). |
| Scope | Reale Git-Diffmenge gegen gemeinsam mit dem historischen Work Package verankertes Manifest; unverändertes lokales/remote `main`. |
| ADR/WP/Links | IDs, Pflichtabschnitte, 21 aktuelle/9 ersetzte ADRs, bidirektionales vollständiges Superseding und explizite Teilnachfolger, ADR-016-Entscheidungskörper gegen den v0.2-Historiencommit sowie relative Ziele. |
| Datenverträge | Draft 2020-12 für Legacy-v1 und aktuelle Level-/Campaign-/Cosmetics-v2-, Proof-, Lock-, Release- und Scope-Schemata; DRAFT-Cosmetics ist schemafähig. |
| Puzzle/Proof/Migration | Pfad, Counts, Endpoint, kleine erschöpfende Eindeutigkeit, profilierte JCS-Hashes, vollständige Proofbindung, alle historischen Fokuswerte sowie vollständige Quell-/Ziel-/Lock-Goldenbindung. |
| Kataloge/Cosmetics | Hierarchie, Unlockgraph, Completionwerte, DRAFT-Ausschluss zur Runtime und ausführbare Purchase-/Milestone-/Reservation-/Ownership-/Statusübergänge; Commit ohne exakt passende persistierte Reservation scheitert. |
| Persistenz | Save-JCS-Cross-Tool, offener Endless-v2-Lifecycle, providerfreier lokaler Skip, Providerreservation vor SDK-Aufruf, Savegeneration-Race, Watermarkinvarianten, Claimmigration, 100 Skip-Zyklen und 10.000 gemischte Transitionen. |
| Mobile | Strukturierte IAP-Fixture-Zustandsmaschine und ausführbarer Privacy-v2-Reducer für Fresh Install, Direktupgrade, nie gestarteten Zwischenbuild, Widerruf, Crash, Restart, Re-enable und Offline. |
| Release | Production-RC, nicht promotable Stagingfixture, artefaktgleicher Promotion-Receipt, vollständige Release-Lock-Cross-References und vollständiger Android-/iOS-Store-Evidenzreducer. |
| Selbsttest | Gezielte Negativmutationen für HIGH-001 bis HIGH-004 plus bewahrte V03- und Guardrail-Regressionen müssen erkannt werden. |

## Ehrliche Belegkategorien

Der Lauf gibt sieben strikt getrennte Kategorien aus:

- **LOCAL_DOCUMENT_STRUCTURE PASS:** lokale Inventar-, Governance-, Link- und Statusprüfung;
- **LOCAL_ARCHITECTURE_SEMANTICS PASS:** tatsächlich ausgeführte Schema-, Fixture-, Reducer-, Bindungs- und Mutationsprüfung;
- **MANUAL_ARCHITECTURE_REVIEW:** verbindliche textuelle Prüfung, insbesondere die vollständige normative IAP-Dokumentreihenfolge; kein automatischer PASS;
- **LOCAL_SCOPE PASS:** reale Git-Diffmenge gegen den gemeinsamen historischen Work-Package-/Manifest-Anker;
- **CONTRACT_ONLY:** Vertrag oder Fixture vorhanden, aber kein Produktionscodebeleg;
- **REQUIRED_LATER/NOT_EXECUTED:** getrennte Unterzeilen für `production-code-validation`, `device-validation` und `store-validation`; verbindlich, aber hier nicht ausgeführt;
- **BLOCKED:** die drei bekannten Produktfolgeblocker.

Ein lokales PASS ist ausdrücklich **kein** Unity-Compile, `.asmdef`-Beleg, C#-Solverproof, IL2CPP-Build, physischer Netzwerkmitschnitt, SDK-Sandboxlauf oder Storeupload. Die IAP-Fixture beweist ausschließlich die strukturierte Modellreihenfolge; die Vollständigkeit der normativen Textregeln bleibt `MANUAL_ARCHITECTURE_REVIEW`.

## Zentrale Fixtures

`save-payload-v1.golden.json` plus `.expected.json` bilden den sprachübergreifenden Save-JCS-Golden. Duplicate Keys und Floattokens werden von Python und Node abgewiesen. `review-contracts-v0.5.json` ist nur eine kleine Projektion; normative Wahrheit bleibt in Architektur und ADRs.

`level-progress-v1-to-v2.golden.json`, `endless-save-v2.example.json`, `endless-save-v1-to-v2.golden.json`, `iap-state-machine-v1.json`, `privacy-lifecycle-v2.json`, `cosmetics-lifecycle-v1.json` und `rollout-metric-v1.json` treiben die ausführbaren Migrations-, Lifecycle-, Reservation-, Race- und Entscheidungsmodelle. Die Dateien unter `ARCHITECTURE/examples/` sind ausschließlich Contractfixtures. `FIXTURE_ONLY` darf nie in Production importiert werden.

## Fehlerbehandlung

Der Validator akkumuliert stabile Fehlercodes. Ein Fehler wird in Vertrag oder Tool behoben; Regeln, Goldens oder Tests werden nicht bloß zur Erzeugung eines grünen Ergebnisses abgeschwächt. Jede neue Architekturregel erhält im selben Work Package eine positive Prüfung und eine negative Mutation.

## Referenzen

[1]: ../../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[2]: ../../DECISIONS/ADR-030-wp-scope-trust-anchor.md "ADR-030 – Gemeinsamer historischer WP-/Scope-Trust-Anchor"
[3]: ../../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.5"
[4]: ../../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005 – Architecture-v0.5-letzte-High-Korrekturen"
