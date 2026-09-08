# Architecture Validation Tool

## Zweck

Dieses Verzeichnis enthält den **einzigen autoritativen, eingecheckten und repositoryrelativen Validator** für Architecture v0.2. Das Werkzeug ist Entwicklungs-/Governance-Tooling und kein Spielproduktionscode. Es setzt weder Manus noch einen absoluten Sandboxpfad voraus.

## Voraussetzungen

| Voraussetzung | Vertrag |
|---|---|
| Betriebssystem | Linux oder macOS für den kanonischen Shellablauf; CI verwendet Ubuntu. |
| Python | CPython 3.12.x mit `venv` und `pip`. |
| Node.js | Node.js 22.x für die unabhängige JCS-Crosscheck-Implementierung. |
| Git | Repository muss ein Checkout mit den Refs `main` und `origin/main` sein. |
| Netzwerk | Nur beim erstmaligen Installieren der exakt gepinnten Pythonpakete erforderlich. Der Validatorlauf selbst ist offline. |

Die Pythonabhängigkeiten stehen vollständig versionsgepinnt in [`requirements.lock.txt`](./requirements.lock.txt). Node verwendet ausschließlich Standardmodule und benötigt kein `npm install`.

## Sauberes Setup aus der Repositorywurzel

```bash
python3 -m venv ../.venv-stp-architecture
../.venv-stp-architecture/bin/python -m pip install --disable-pip-version-check -r tools/architecture-validation/requirements.lock.txt
```

Die virtuelle Umgebung liegt bewusst neben dem Checkout und wird nicht eingecheckt. Ein Dependency-Update erfolgt ausschließlich in einem eigenen Work Package mit sauberem Neuinstallations- und vollständigem Selbsttestnachweis.

## Kanonischer Ausführungsbefehl

Aus der Repositorywurzel:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py --self-test
```

Ein gültiger Abschluss endet mit Exitcode `0` und einer Zeile der Form:

```text
RESULT PASS (<Anzahl> Prüfgruppen)
```

Fehlende Abhängigkeiten, fehlendes Node.js, ein nicht erkannter Mutationsfehler oder ein übersprungenes Gate sind **kein PASS**. Ohne `--self-test` läuft nur die positive Repositoryprüfung; dieser verkürzte Lauf genügt nicht für Work-Package-Abnahme.

## Prüfumfang

| Gruppe | Nachweis |
|---|---|
| Inventar | Alle Architecture-v0.2-Dokumente, vier Schemata, zehn JSON-Fixtures, 17 ADRs, zwei Work Packages und Tooldateien existieren. |
| Work Packages | Dateiname, Titel und ID folgen eindeutig `WP-###`; Pflichtabschnitte sind geordnet; keine alte Sonder-ID bleibt. |
| ADR-Governance | Index, Status, 13 aktuelle/4 ersetzte ADRs und bidirektionale Superseding-Verweise stimmen. |
| Links | Relative Markdownziele existieren. |
| JSON/Schema | Duplicate Keys werden abgewiesen; Draft-2020-12-Schemata und Beispiele sind gültig. |
| Level | 4×4- und Ein-Zellen-Fixture bestehen Pfad-, Count-, Endpoint-, Hash- und erschöpfende Eindeutigkeitsprüfung. |
| Non-Level-Kataloge | Campaign-, Completion- und Cosmetics-IDs, bestätigte Rewardwerte und Cross-References stimmen. |
| Assemblygraph | Nur reale normative Assemblies, erlaubte Adapterrichtung und kein Zyklus. |
| Reviewverträge | Saveprofil, Rewardclaim, IAP-Reihenfolge, Privacy Default-Off, Ledgergrenzen, Endless-ID, Device Evidence und Modulregel. |
| JCS-Crosscheck | Python und unabhängiges Node-Skript erzeugen identische UTF-8-Bytes und SHA-256-Werte. |
| Status/Blocker | Architecture v0.2 ist aktuell; genau die drei Produktblocker bleiben offen und fail-closed. |
| Git-Scope | `main == origin/main`; nur zulässige Dokument-/Schema-/Governance-/Toolpfade; kein Produktionscode. |
| Selbsttest | Zwölf gezielte Negativmutationen `ARCH-REV-001` bis `ARCH-REV-012` müssen erkannt werden. |

## Fixtures

`fixtures/save-payload-v1.golden.json` ist der sprachübergreifende Save-JCS-Goldeninput; `fixtures/save-payload-v1.expected.json` fixiert Profil, Byteanzahl und SHA-256. `fixtures/duplicate-key.invalid.json` und `fixtures/float-token.invalid.json` müssen von Python und dem unabhängigen Node-Parser als Nicht-I-JSON beziehungsweise als Verletzung des integer-only Saveprofils abgewiesen werden. `fixtures/review-contracts-v0.2.json` spiegelt kleine, maschinenprüfbare Eckwerte der zwölf Reviewverträge. Die fachlich ausführliche Wahrheit bleibt in den Architektur- und ADR-Dokumenten; das Fixture darf sie nicht eigenständig erweitern.

Die Beispiele unter `ARCHITECTURE/examples/` sind ausschließlich Contractfixtures. `FIXTURE_ONLY` darf niemals in einen Productionkatalog importiert werden.

## Fehlerbehandlung

Der Validator akkumuliert Fehler und gibt stabile Präfixe aus. Ein Fehler wird in Vertrag oder Tool behoben; Tests, erwartete Hashes oder Regeln dürfen nicht nur zur Erzeugung eines grünen Ergebnisses abgeschwächt werden. Eine neue Architekturregel erhält im selben Work Package eine positive Prüfung und eine negative Mutation.

## Referenzen

[1]: ../../DECISIONS/ADR-017-versionierter-architekturvalidator.md "ADR-017 – Versionierter Architekturvalidator"
[2]: ../../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.2"
[3]: ../../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md "WP-002 – Architecture-v0.2-Korrekturen"
