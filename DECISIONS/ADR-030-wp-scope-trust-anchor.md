# ADR-030 – Gemeinsamer historischer WP-/Scope-Trust-Anchor

## Status

**Angenommen**

## Datum

2026-09-13

## Kontext

ADR-026 verlangt Work Package und Scope-Manifest im selben Ankercommit. Der Validator bewies bislang nur den Manifest-Add-Commit, dessen Elterncommit und die Bytegleichheit des Manifests. Ein später hinzugefügtes Work Package oder ein erst später ergänzter Manifestverweis konnte die Prüfung dennoch bestehen.

## Entscheidung

Die Scope-Abnahme wird für den konkret aufgerufenen kanonischen Manifestpfad historisch gebunden:

1. Der Validator ermittelt genau einen von `HEAD` erreichbaren Commit, der diesen Manifestpfad mit Status `A` einführt.
2. Der historische Manifestblob wird aus genau diesem Commit geladen. Dessen `baseCommit` muss exakt der Elterncommit des Ankers sein. Der aktuelle Manifestblob muss bytegleich bleiben.
3. Aus dem Baum desselben Ankers wird exakt ein Work Package `WORK_PACKAGES/<workPackageId>_*.md` bestimmt. Dieses Work Package darf im Elternbaum nicht existieren. Der Ankerdiff muss Work Package und Manifest beide als `A` ausweisen.
4. Dateiname, H1-ID und `## ID` des historischen Work Packages müssen exakt `workPackageId` entsprechen.
5. Der historische Work-Package-Blob muss einen lokalen Markdownlink enthalten, dessen sicher normalisiertes Ziel exakt der kanonische Manifestpfad ist. HTTP(S), absolute Ziele, Backslashes, Pfadescape und andere Manifestpfade sind keine Bindung.
6. Aktuelle Work-Package-Dateien oder später ergänzte Verweise sind keine Trust-Evidenz.

Die Prüfung ist pro aufgerufenem Manifest parametrisiert. Sie hardcodiert weder `WP-005` noch Documentation-Scope. Ein späteres Production-Work-Package darf sein eigenes Manifest in einem eigenen gemeinsamen Ankercommit einführen.

## Begründung

Nur ein gemeinsamer historischer Add-Commit beweist, dass Scope und Auftrag vor fachlichen Änderungen gemeinsam feststanden. Die exakte Linkauflösung bindet den Auftrag an genau die geprüfte Allowlist. Die Parametrisierung erhält spätere legitime Work Packages.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Aktuelles Work Package lesen | Ermöglicht nachträgliche Reparatur oder Umdeutung. |
| Nur Dateinamen vergleichen | Beweist keinen historischen expliziten Verweis. |
| Text-Substring suchen | Ist durch andere Links, Pfadescape oder fremde Manifeste umgehbar. |
| Global nur ein Manifest zulassen | Blockiert legitime spätere Production-Scopes. |

## Konsequenzen

WP-005 und `WP-005.documentation.scope.json` werden vor fachlichen Änderungen gemeinsam in Commit `ebf522a9ef3c28035341cc04dbd6d8251b107603` eingeführt. Der Manifest-`baseCommit` ist dessen Elterncommit `cdf153c7ee1d96f03cfd5ea7445dc4f186c9ff5e`. Spätere Änderungen am Work Package sind zulässige Abschlussdokumentation, aber keine Trust-Evidenz.

## Betroffene Artefakte

`tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md`, `tools/architecture-validation/scopes/WP-005.documentation.scope.json`, `WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` und `ARCHITECTURE/TEST_STRATEGY.md`.

## Validierung

Der positive Test lädt Work Package und Manifest aus demselben Ankercommit. Negativtests decken später hinzugefügtes WP/Manifest, getrennte Add-Commits, später ergänzten Verweis, falsche WP-ID, anderes Manifest, externen/absoluten/Pfad-Escape-Link und nachträgliche Manifestmutation ab. Ein synthetisches späteres Production-WP mit eigenem korrektem Anker bleibt zulässig.

## Ersetzt / ersetzt durch

Präzisiert und ersetzt ausschließlich die Work-Package-/Manifest-Bindung aus [ADR-026](./ADR-026-validator-evidenz-und-scope-vertrauensanker.md). Scope-Modi, Segmentglobvertrag und Evidenzkategorien aus ADR-026 bleiben gültig. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../tools/architecture-validation/README.md "Architecture Validation Tool"
[2]: ../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.5"
[3]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005"
