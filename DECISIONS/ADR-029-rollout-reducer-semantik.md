# ADR-029 – Vollständige Rollout-Reducer-Semantik

## Status

**Angenommen**

## Datum

2026-09-13

## Kontext

Architecture v0.4 dokumentiert plattformspezifische Crashraten, Mindestpopulationen, Freshness, Stufen und Beobachtungsfenster. Der ausführbare Reducer wertete jedoch nur Crashrate, eine generische Population, Datenalter und ein ungebundenes Boolean für exakte Releasefilterung aus. Ein automatisches `ADVANCE` war damit ohne vollständigen Nachweis möglich.

## Entscheidung

Der Reducer akzeptiert ein vollständiges Evidenzobjekt. Er liefert `ADVANCE_OR_HOLD_AT_100` nur, wenn alle folgenden Bedingungen gleichzeitig erfüllt sind:

1. Plattform, Quelle und Metrik entsprechen exakt dem gewählten Plattformvertrag.
2. `releaseIdentity` und `storeBuildReference` entsprechen exakt der erwarteten Release-/Buildidentität.
3. `stagePercentage` ist eine definierte Stufe und `windowEndUtc - windowStartUtc` erfüllt deren `minimumObservationHours`.
4. `observedAtUtc` liegt nicht vor dem Fensterende. Die aus `freshness.observedThroughUtc` bis `observedAtUtc` berechnete Datenalterung ist vorhanden, nicht negativ und höchstens plattformspezifisch erlaubt.
5. Android weist mindestens 100 `distinctUsers` aus. iOS weist mindestens 100 `sessions` und mindestens fünf `activeDevices` aus.
6. `numerator`, `denominator` und `crashRate` sind vorhanden, nicht negativ, plattformkonsistent und rechnerisch konsistent.
7. `reportingComplete` ist wahr. Alle verpflichtenden Evidenzfelder einschließlich `reviewer` sind vorhanden.

Fehlt eine Bedingung, ist die Plattform unbekannt oder ist die Datengrundlage stale, unvollständig, versionsfremd oder zu klein, lautet das Ergebnis ausschließlich `PAUSE_NO_ADVANCE`.

Erst nach dieser Evidenzprüfung gelten die bestehenden Schwellen unverändert: unter `0,005` folgt `ADVANCE_OR_HOLD_AT_100`, ab `0,005` und unter `0,01` `PAUSE_AND_INVESTIGATE`, ab `0,01` `HALT_AND_ROLL_BACK_IF_AVAILABLE`.

## Begründung

Ein Releasegate darf die Vollständigkeit seines Belegs nicht außerhalb des Reducers voraussetzen. Die konkrete Evidenzbindung macht `ADVANCE` zu einer nachprüfbaren, fail-closed Entscheidung und bewahrt zugleich alle bereits bestätigten Schwellen und Plattformquellen.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Weiterhin lose Parameter plus `exactRelease`-Boolean | Beweist weder Buildbindung noch vollständiges Reporting. |
| Fehlende Daten als neutral behandeln | Kann Rollout ohne belastbare Datengrundlage fortsetzen. |
| Neue Schwellen festlegen | Nicht erforderlich und außerhalb des WP-005-Scopes. |

## Konsequenzen

`rollout-metric-v1.json` erhält positive Evidenzfixtures für Android und iOS. Der Validator mutiert jede Pflichtdimension einzeln und verlangt `PAUSE_NO_ADVANCE`. Store-/Providerabruf bleibt spätere Productionintegration; der lokale Reducer belegt nur die dokumentierte Entscheidungssemantik.

## Betroffene Artefakte

`ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `tools/architecture-validation/fixtures/rollout-metric-v1.json`, `tools/architecture-validation/validate.py` und `WP-005`.

## Validierung

Positive Android- und iOS-Evidenz erlaubt die dokumentierte Entscheidung. Negativtests decken zu wenige iOS-Geräte, unvollständiges Fenster, stale Daten, fehlende Freshness, unvollständiges Reporting, fehlende Metrik, falsche Plattform/Quelle, falsche Buildbindung und zu kleine Population ab.

## Ersetzt / ersetzt durch

Präzisiert den operativen Rolloutanteil aus [ADR-010](./ADR-010-build-release-und-observability.md) und den releaseidentischen Kandidaten aus [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md). Schwellen, Storequellen und Artefaktpromotion bleiben unverändert. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/BUILD_AND_RELEASE.md "Build and Release v0.5"
[2]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005"
