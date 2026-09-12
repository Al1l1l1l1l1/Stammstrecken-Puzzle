# Architecture Validation Tool

## Zweck

Dieses Verzeichnis enthält den **einzigen autoritativen, eingecheckten und repositoryrelativen Validator** für Architecture v0.4. Das Werkzeug prüft Dokumentstruktur, Datenverträge, ausführbare Architekturmodelle und den Git-Scope. Es ist Governance-/Contract-Tooling und kein Spielproduktionscode.

## Voraussetzungen

| Voraussetzung | Vertrag |
|---|---|
| Betriebssystem | Linux oder macOS; die spätere CI-Umgebung verwendet Ubuntu 24.04. |
| Python | CPython 3.11.x mit `venv` und `pip`. |
| Node.js | Node.js 22.x für den unabhängigen JCS-Crosscheck. |
| Git | Vollständiger Checkout; Basis- und Manifestankercommit müssen lokal erreichbar sein. |
| Netzwerk | Nur zur Installation der exakt gepinnten Pythonpakete; der Validatorlauf selbst ist offline. |

Die Pythonabhängigkeiten stehen versionsgepinnt in [`requirements.lock.txt`](./requirements.lock.txt). Node verwendet ausschließlich Standardmodule.

## Sauberes Setup

```bash
python3 -m venv ../.venv-stp-architecture
../.venv-stp-architecture/bin/python -m pip install --disable-pip-version-check \
  -r tools/architecture-validation/requirements.lock.txt
```

## Kanonischer WP-004-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-004.documentation.scope.json \
  --self-test
```

`--scope` und `--scope-manifest` sind gemeinsam für jede Work-Package-Abnahme Pflicht. Der Manifestpfad muss repositoryrelativ sein. `validate.py --self-test` ohne beide Argumente ist als Architecture-only-Diagnoselauf zulässig, belegt aber keinen Git-Scope. Ein gültiger Abschluss endet mit Exitcode `0` und `RESULT PASS`. Ohne `--self-test` fehlt der Mutationsnachweis.

## Scope-Vertrauensanker

Das Manifest folgt [`scope-manifest-v1.schema.json`](./scope-manifest-v1.schema.json) und liegt exakt unter `tools/architecture-validation/scopes/<WP-ID>.<scope>.scope.json`. Es wird zusammen mit dem Work Package **vor** den fachlichen Änderungen in einem eigenen Ankercommit eingeführt. Sein `baseCommit` muss der Elterncommit dieses Ankers sein. Der Validator liest den Manifestblob aus dem Ankercommit und verlangt Bytegleichheit mit der aktuellen Datei; eine nachträgliche Erweiterung scheitert.

Die reale Änderungsmenge stammt aus `git diff --name-status <baseCommit> --` einschließlich beider Rename-/Copy-Endpunkte und untracked Dateien. Muster verwenden segmentierte POSIX-Semantik: `*` matcht kein `/`, `**` ist nur als eigenes Segment erlaubt. Globale beziehungsweise global-äquivalente Muster (`*`, `**`, `**/*`, `*/**`), absolute oder externe Manifestpfade, unversionierte Manifeste, falsche WP-/Scope-Dateinamen, `..`, Backslashes, Symlink-Escape und nicht erlaubte Dateien scheitern. Das Manifest muss sich selbst ausdrücklich nennen und darf keine anderen Scope-Manifeste erlauben. Documentation-Scope lehnt Produktcode, Unityartefakte und Produktquellen zusätzlich kategorisch ab.

Eine spätere autorisierte CI-Integration muss denselben kanonischen Befehl und denselben Manifestanker verwenden. Der fehlende GitHub-Actions-Workflow ist für WP-004 **non-blocking with follow-up**; ein eigenes CI-Setup-Work-Package ist jedoch zwingend vor dem ersten produktiven Coding-Work-Package abzuschließen.

## Tatsächlicher Prüfumfang

| Gruppe | Tatsächlicher Nachweis |
|---|---|
| Inventar/Governance | v0.4-Dokumente, Schemata, Fixtures, 26 ADRs, vier Work Packages und Tooldateien. |
| Scope | Reale Git-Diffmenge gegen vorab verankertes Manifest; unverändertes lokales/remote `main`. |
| ADR/WP/Links | IDs, Pflichtabschnitte, 17 aktuelle/9 ersetzte ADRs, bidirektionales Superseding, ADR-016-Entscheidungskörper gegen den v0.2-Historiencommit und relative Ziele. |
| Datenverträge | Draft 2020-12 für Legacy-v1 und aktuelle Level-/Campaign-/Cosmetics-v2-, Proof-, Lock-, Release- und Scope-Schemata; DRAFT-Cosmetics ist schemafähig. |
| Puzzle/Proof/Migration | Pfad, Counts, Endpoint, kleine erschöpfende Eindeutigkeit, profilierte JCS-Hashes, vollständige Proofbindung, alle historischen Fokuswerte sowie vollständige Quell-/Ziel-/Lock-Goldenbindung. |
| Kataloge/Cosmetics | Hierarchie, Unlockgraph, Completionwerte, DRAFT-Ausschluss zur Runtime und ausführbare Purchase-/Milestone-/Claim-/Ownership-/Statusübergänge. |
| Persistenz | Save-JCS-Cross-Tool, offener Endless-v2-Lifecycle, Watermarkinvarianten, Claimmigration und 10.000 begrenzte Transitionen. |
| Mobile | Strukturierte IAP-Fixture-Zustandsmaschine und ausführbarer Privacy-v2-Reducer für Fresh Install, Direktupgrade, nie gestarteten Zwischenbuild, Widerruf, Crash, Restart, Re-enable und Offline. |
| Release | Production-RC, nicht promotable Stagingfixture, artefaktgleicher Promotion-Receipt, vollständige Release-Lock-Cross-References und fail-closed Store-Crashrate-Entscheidung. |
| Selbsttest | Gezielte Negativmutationen für V03-001 bis V03-007 plus bewahrte Regressionen müssen erkannt werden. |

## Ehrliche Belegkategorien

Der Lauf gibt sieben strikt getrennte Kategorien aus:

- **LOCAL_DOCUMENT_STRUCTURE PASS:** lokale Inventar-, Governance-, Link- und Statusprüfung;
- **LOCAL_ARCHITECTURE_SEMANTICS PASS:** tatsächlich ausgeführte Schema-, Fixture-, Reducer-, Bindungs- und Mutationsprüfung;
- **MANUAL_ARCHITECTURE_REVIEW:** verbindliche textuelle Prüfung, insbesondere die vollständige normative IAP-Dokumentreihenfolge; kein automatischer PASS;
- **LOCAL_SCOPE PASS:** reale Git-Diffmenge gegen den verankerten Work-Package-Scope;
- **CONTRACT_ONLY:** Vertrag oder Fixture vorhanden, aber kein Produktionscodebeleg;
- **REQUIRED_LATER/NOT_EXECUTED:** getrennte Unterzeilen für `production-code-validation`, `device-validation` und `store-validation`; verbindlich, aber hier nicht ausgeführt;
- **BLOCKED:** die drei bekannten Produktfolgeblocker.

Ein lokales PASS ist ausdrücklich **kein** Unity-Compile, `.asmdef`-Beleg, C#-Solverproof, IL2CPP-Build, physischer Netzwerkmitschnitt, SDK-Sandboxlauf oder Storeupload. Die IAP-Fixture beweist ausschließlich die strukturierte Modellreihenfolge; die Vollständigkeit der normativen Textregeln bleibt `MANUAL_ARCHITECTURE_REVIEW`.

## Zentrale Fixtures

`save-payload-v1.golden.json` plus `.expected.json` bilden den sprachübergreifenden Save-JCS-Golden. Duplicate Keys und Floattokens werden von Python und Node abgewiesen. `review-contracts-v0.4.json` ist nur eine kleine Projektion; normative Wahrheit bleibt in Architektur und ADRs.

`level-progress-v1-to-v2.golden.json`, `endless-save-v2.example.json`, `endless-save-v1-to-v2.golden.json`, `iap-state-machine-v1.json`, `privacy-lifecycle-v2.json`, `cosmetics-lifecycle-v1.json` und `rollout-metric-v1.json` treiben die ausführbaren Migrations-, Lifecycle- und Entscheidungsmodelle. Die Dateien unter `ARCHITECTURE/examples/` sind ausschließlich Contractfixtures. `FIXTURE_ONLY` darf nie in Production importiert werden.

## Fehlerbehandlung

Der Validator akkumuliert stabile Fehlercodes. Ein Fehler wird in Vertrag oder Tool behoben; Regeln, Goldens oder Tests werden nicht bloß zur Erzeugung eines grünen Ergebnisses abgeschwächt. Jede neue Architekturregel erhält im selben Work Package eine positive Prüfung und eine negative Mutation.

## Referenzen

[1]: ../../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[2]: ../../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.4"
[3]: ../../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md "WP-004 – Architecture-v0.4-Abschlusskorrekturen"
