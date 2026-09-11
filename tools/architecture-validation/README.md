# Architecture Validation Tool

## Zweck

Dieses Verzeichnis enthält den **einzigen autoritativen, eingecheckten und repositoryrelativen Validator** für Architecture v0.3. Das Werkzeug ist Governance-/Contract-Tooling und kein Spielproduktionscode. Es setzt weder Manus noch einen absoluten Sandboxpfad voraus.

## Voraussetzungen

| Voraussetzung | Vertrag |
|---|---|
| Betriebssystem | Linux oder macOS; CI verwendet Ubuntu 24.04. |
| Python | CPython 3.11.x mit `venv` und `pip`. |
| Node.js | Node.js 22.x für den unabhängigen JCS-Crosscheck. |
| Git | Vollständiger Checkout; der im Scope-Manifest benannte Basiscommit muss lokal erreichbar sein. |
| Netzwerk | Nur zur Installation der exakt gepinnten Pythonpakete; der Validatorlauf selbst ist offline. |

Die Pythonabhängigkeiten stehen versionsgepinnt in [`requirements.lock.txt`](./requirements.lock.txt). Node verwendet ausschließlich Standardmodule.

## Sauberes Setup

```bash
python3 -m venv ../.venv-stp-architecture
../.venv-stp-architecture/bin/python -m pip install --disable-pip-version-check \
  -r tools/architecture-validation/requirements.lock.txt
```

## Kanonischer WP-003-Befehl

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-003.documentation.scope.json \
  --self-test
```

`--scope` und `--scope-manifest` sind gemeinsam für jede Work-Package-Abnahme Pflicht. `validate.py --self-test` ohne beide Argumente ist als Architecture-only-Diagnoselauf zulässig, belegt aber keinen Git-Scope. Ein gültiger Abschluss endet mit Exitcode `0` und `RESULT PASS`. Ohne `--self-test` fehlt der Mutationsnachweis und der Lauf genügt nicht für Work-Package-Abnahme.

## Scope-Modell

Das Manifest folgt [`scope-manifest-v1.schema.json`](./scope-manifest-v1.schema.json) und bindet Work-Package-ID, unveränderlichen Basiscommit, Modus und erlaubte repositoryrelative POSIX-Globmuster. Der Validator bildet die reale Menge aus `git diff --name-status <baseCommit> --` einschließlich beider Rename-/Copy-Endpunkte und untracked Dateien. Absolute Pfade, `..`, ein globales `**` oder nicht erlaubte Dateien scheitern. Documentation-Scope lehnt Produktcode und Unityartefakte zusätzlich kategorisch ab.

Lokale Abnahme und eine spätere autorisierte CI-Integration müssen dasselbe eingecheckte Manifest und denselben kanonischen Befehl verwenden. Im Abschluss von WP-003 ist kein GitHub-Actions-Workflow enthalten, weil die aktive GitHub-App Workflowdateien ohne `workflows`-Berechtigung nicht pushen durfte; dieser Remote-CI-Nachweis wird nicht als PASS ausgewiesen.

## Prüfumfang

| Gruppe | Tatsächlicher Nachweis |
|---|---|
| Inventar/Governance | v0.3-Dokumente, elf Schemaverträge, Fixtures, 23 ADRs, drei Work Packages und Tooldateien. |
| Scope | Reale Git-Diffmenge gegen versioniertes Manifest; unverändertes lokales/remote `main`. |
| ADR/WP/Links | IDs, Pflichtabschnitte, 15 aktuelle/8 ersetzte ADRs, bidirektionales Superseding und relative Ziele. |
| Datenverträge | Draft-2020-12 für Legacy-v1 und aktuelle Level-/Campaign-/Cosmetics-v2-, Proof-, Lock-, Manifest- und Scope-Schemata. |
| Puzzle/Proof | Pfad, Counts, Endpoint, kleine erschöpfende Eindeutigkeit, profilierte JCS-Hashes und vollständige Proofbindung. |
| Kataloge | Hierarchie, Unlockgraph, Completionwerte und diskriminierte Cosmetics-Eligibility. |
| Assemblygraph | Exakte Bootstrapreferenzen, Adapterrichtung, Azyklizität und Mermaid-/Tabellenparität. |
| Persistenz | Save-JCS Cross-Tool, v1→v2-Endless-Golden, Watermarkinvarianten und 10.000 Transitionen. |
| Mobile | Ausführbare IAP-Fixture-Zustandsmaschine und strukturierter Privacy-Lifecycle. |
| Release | Production-RC, nicht promotable Stagingfixture, artefaktgleicher Promotion-Receipt und Release-Lock-Bindung. |
| Selbsttest | Gezielte Mutationen FINAL-001 bis FINAL-008 plus Regressionen müssen erkannt werden. |

## Ehrliche Belegkategorien

Der Lauf gibt sechs Kategorien aus:

- **LOCAL_DOCUMENT_STRUCTURE PASS:** lokale Inventar-, Governance-, Link- und Statusprüfung;
- **LOCAL_ARCHITECTURE_SEMANTICS PASS:** lokale Schema-, Fixture-, Modell- und Mutationsprüfung;
- **LOCAL_SCOPE PASS:** reale Git-Diffmenge gegen das Work-Package-Manifest;
- **CONTRACT_ONLY:** Vertrag/Fixture vorhanden, aber kein Produktionscodebeleg;
- **REQUIRED_LATER/NOT_EXECUTED:** verbindliches späteres Unity-, Geräte-, SDK- oder Storegate;
- **BLOCKED:** die drei bekannten Produktblocker.

Ein lokales PASS des Architekturvalidators ist ausdrücklich **kein** Unity-Compile, `.asmdef`-Beleg, C#-Solverproof, IL2CPP-Build, physischer Netzwerkmitschnitt, SDK-Sandboxlauf oder Storeupload.

## Zentrale Fixtures

`save-payload-v1.golden.json` plus `.expected.json` bilden den sprachübergreifenden Save-JCS-Golden. Duplicate Keys und Floattokens werden von Python und Node abgewiesen. `review-contracts-v0.3.json` ist nur eine kleine Projektion; normative Wahrheit bleibt in Architektur und ADRs. `level-progress-v1-to-v2.golden.json`, `endless-save-v2.example.json`, `endless-save-v1-to-v2.golden.json`, `iap-state-machine-v1.json` und `privacy-lifecycle-v1.json` treiben ausführbare Migrations- und Vertragsmodelle.

Die Dateien unter `ARCHITECTURE/examples/` sind ausschließlich Contractfixtures. `FIXTURE_ONLY` darf nie in Production importiert werden.

## Fehlerbehandlung

Der Validator akkumuliert stabile Fehlercodes. Ein Fehler wird in Vertrag oder Tool behoben; Regeln, Goldens oder Tests werden nicht bloß zur Erzeugung eines grünen Ergebnisses abgeschwächt. Jede neue Architekturregel erhält im selben Work Package eine positive Prüfung und eine negative Mutation.

## Referenzen

[1]: ../../DECISIONS/ADR-022-validator-scope-und-belegkategorien.md "ADR-022 – Validator-Scope und Belegkategorien"
[2]: ../../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.3"
[3]: ../../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003 – Architecture-v0.3-Finalkorrekturen"
