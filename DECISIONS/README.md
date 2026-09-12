# Architekturentscheidungen – verbindlicher ADR-Index

**Aktueller Architekturstand:** **Architecture v0.4**, angenommen am 2026-09-12 im Rahmen von [`WP-004`](../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md).
**Architektureinstieg:** [`ARCHITECTURE/ARCHITECTURE.md`](../ARCHITECTURE/ARCHITECTURE.md)

Der Ordner `DECISIONS/` enthält die dauerhaft nachvollziehbaren Architecture Decision Records (ADRs) des Projekts. Ein ADR dokumentiert Entscheidung, Begründung, Alternativen, Folgen und Ablösung. Produktentscheidungen bleiben den bestätigten Produktquellen vorbehalten.

## Aktueller Index

| ADR | Titel | Status | Geltung in Architecture v0.4 |
|---|---|---|---|
| [ADR-001](./ADR-001-unity-6-3-lts.md) | Unity 6.3 LTS als Game Engine | **Angenommen** | Gültig. |
| [ADR-002](./ADR-002-csharp-9-und-il2cpp.md) | C# 9 und IL2CPP | **Angenommen** | Gültig. |
| [ADR-003](./ADR-003-domain-trennung-und-modulgrenzen.md) | Reine Puzzle-Domain und gerichtete Modulgrenzen | **Ersetzt** | Historisch; durch ADR-013 ersetzt. |
| [ADR-004](./ADR-004-json-leveldaten-und-content-pipeline.md) | Versionierte JSON-Leveldaten | **Ersetzt** | Historisch; vollständig durch ADR-021 ersetzt, der alle fortgeltenden Grundsätze restatiert. |
| [ADR-005](./ADR-005-deterministisches-command-state-modell.md) | Deterministisches Command/State-Modell | **Angenommen** | Gültig. |
| [ADR-006](./ADR-006-lokale-persistenz-und-offline-first.md) | Lokaler atomarer Save als Offline-First-Wahrheit | **Ersetzt** | Historisch; durch ADR-014 und danach ADR-019 ersetzt. |
| [ADR-007](./ADR-007-solver-und-eindeutigkeitspruefung.md) | Deterministischer Constraint-Solver | **Angenommen** | Gültig; ADR-021 ergänzt das Proofartefakt. |
| [ADR-008](./ADR-008-mobile-service-ports-und-provider.md) | Mobile-Service-Ports mit Google- und Unity-Adaptern | **Ersetzt** | Historisch; durch ADR-015 und danach ADR-020 ersetzt. |
| [ADR-009](./ADR-009-automatisierte-teststrategie.md) | Automatisierte Tests als Architekturgrenze | **Ersetzt** | Historisch; durch ADR-017 und danach ADR-022 ersetzt. |
| [ADR-010](./ADR-010-build-release-und-observability.md) | Build, Release und Observability | **Angenommen** | Build-/Releasegrundsätze gültig; Production-Crashlytics durch ADR-020 ersetzt, ADR-023 ergänzt Kandidatenidentität. |
| [ADR-011](./ADR-011-ui-assets-lokalisierung-und-audio.md) | UI, Assets, Lokalisierung und Audio | **Angenommen** | Gültig. |
| [ADR-012](./ADR-012-mobile-plattformbaselines.md) | Mobile Plattformbaselines | **Angenommen** | Gültig. |
| [ADR-013](./ADR-013-zyklusfreie-ports-und-modulgrenzen.md) | Zyklusfreie Ports und Modulgrenzen | **Angenommen** | Gültig; ADR-018 ergänzt die minimale Bootstrapkante. |
| [ADR-014](./ADR-014-save-kanonisierung-und-ledgerkompaktierung.md) | Save-Kanonisierung und Ledgerkompaktierung | **Ersetzt** | Historisch; durch ADR-019 ersetzt. |
| [ADR-015](./ADR-015-mobile-transaktionen-und-privacy-default-off.md) | Mobile Transaktionen und Privacy Default-Off | **Ersetzt** | Historisch; durch ADR-020 ersetzt. |
| [ADR-016](./ADR-016-katalogvertraege-und-endless-identitaet.md) | Katalogverträge und Endless-Identität | **Angenommen** | Historischer Entscheidungskörper unverändert; Katalog- und E1-ID-Grundvertrag gültig, Retention durch ADR-019 und Open-Lifecycle durch ADR-025 ersetzt, Cosmetics durch ADR-023 ergänzt. |
| [ADR-017](./ADR-017-versionierter-architekturvalidator.md) | Versionierter Architekturvalidator | **Ersetzt** | Historisch; durch ADR-022 ersetzt. |
| [ADR-018](./ADR-018-bootstrap-composition-root.md) | Kompilierbare Bootstrap-Composition-Root | **Angenommen** | Gültig; ergänzt ADR-013. |
| [ADR-019](./ADR-019-endless-watermark-und-save-v2.md) | Endless-Watermark und Save v2 | **Angenommen** | Watermark-/Save-v2-Grundsatz gültig; Open-Lifecycle und Claims durch ADR-025 präzisiert. |
| [ADR-020](./ADR-020-privacy-lifecycle-und-sdk-grenzen.md) | Privacy-Lifecycle und SDK-Grenzen | **Angenommen** | Provider-/Fail-closed-Grundsatz gültig; Bootstrapfence und Widerruf durch ADR-024 präzisiert. |
| [ADR-021](./ADR-021-puzzleidentitaet-und-proofartefakte.md) | Puzzleidentität und versionierte Proofartefakte | **Angenommen** | Gültig; ersetzt ADR-004 vollständig mit Restatement und ergänzt ADR-007. |
| [ADR-022](./ADR-022-validator-scope-und-belegkategorien.md) | Validator-Scope und Belegkategorien | **Ersetzt** | Historisch; vollständig durch ADR-026 ersetzt. |
| [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) | Releasekandidat-Identität und kosmetische Meilensteinclaims | **Angenommen** | Gültig; ergänzt ADR-010 und ADR-016. |
| [ADR-024](./ADR-024-privacy-bootstrap-fence-und-widerruf.md) | Privacy-Bootstrap-Fence und crashsicherer Widerruf | **Angenommen** | Gültig; präzisiert ADR-020 für direkte Upgrades, Native Reconciliation und Widerruf. |
| [ADR-025](./ADR-025-endless-open-lifecycle-und-claims.md) | Endless-Open-Lifecycle und nachgelagerte Claims | **Angenommen** | Gültig; präzisiert ADR-019 und ersetzt die offenen Retentions-/Claimanteile aus ADR-016. |
| [ADR-026](./ADR-026-validator-evidenz-und-scope-vertrauensanker.md) | Validator-Evidenz und Scope-Vertrauensanker | **Angenommen** | Gültig; ersetzt ADR-022 vollständig. |

Damit existieren **26 ADRs**: 17 sind angenommen und aktuell wirksam; 9 bleiben als ersetzte historische Entscheidungen erhalten. Bei Widerspruch gilt das jüngere ausdrücklich ersetzende ADR. Teilersetzung oder Ergänzung ist nur zulässig, wenn Umfang und fortgeltender Teil im ADR und in diesem Index ausdrücklich benannt sind.

## Wann ein ADR erforderlich ist

Ein ADR ist erforderlich, bevor eine technische Entscheidung umgesetzt wird, wenn sie Architektur, Datenhaltung, Persistenz, Sicherheits- oder Datenschutzgrenzen, Plattformstrategie, externe Dienste, Integrationen, Build- oder Releasefähigkeit oder eine andere langfristige Systemgrenze beeinflusst.

Ein ADR ist kein Ersatz für eine Produktentscheidung. Fehlt eine notwendige Produktentscheidung, wird sie als Blocker dokumentiert statt erfunden.

## Ablage und Dateinamen

Jeder ADR liegt unmittelbar in diesem Ordner und folgt `ADR-###-kurzer-titel.md`. Die Nummer ist fortlaufend, eindeutig und wird nie wiederverwendet. Ein angenommener ADR wird nicht still inhaltlich umgeschrieben. Erlaubt sind nachträgliche Status-, Link- und Ersetzungsverweise, die seine historische Entscheidung nicht verändern.

## Verbindliche ADR-Struktur

| Abschnitt | Inhalt |
|---|---|
| Titel und ID | Eindeutige ADR-Nummer und kurze Bezeichnung. |
| Status | Entwurf, angenommen, ersetzt oder verworfen. |
| Datum | Datum der Dokumentation. |
| Kontext | Ausgangslage, Anforderungen, Einschränkungen und relevante Quellen. |
| Entscheidung | Präzise und überprüfbare technische Entscheidung. |
| Begründung | Warum die Entscheidung den Anforderungen entspricht. |
| Betrachtete Alternativen | Relevante Alternativen und Gründe gegen ihre Wahl. |
| Konsequenzen | Folgen, Risiken, Migrationen und Prüfpflichten. |
| Betroffene Artefakte | Work Packages, Dateien und Module. |
| Validierung | Nachweis der Entscheidung. |
| Ersetzt / ersetzt durch | Beidseitige Referenz auf frühere oder spätere ADRs. |

## Lebenszyklus und Superseding

Ein ADR beginnt als **Entwurf** und ist nicht verbindlich. Erst **Angenommen** macht die Entscheidung wirksam. Eine angenommene Entscheidung wird nur durch einen neuen, fortlaufend nummerierten ADR vollständig oder ausdrücklich teilweise abgelöst. Der neue ADR benennt den Vorgänger; der alte ADR erhält ausschließlich Status-/Link-/Ersetzungsverweise. Bei vollständiger Ablösung wird sein Status **Ersetzt**. Bei klar abgegrenzter Teilersetzung kann er **Angenommen** bleiben, wenn der fortgeltende Umfang in beiden ADRs und im Index eindeutig ist.

Ein ersetzter ADR bleibt als historische Begründung erhalten. Der Index wird im selben Commit aktualisiert. Ein verworfener ADR erzeugt keine Vorgabe. Work Package, Architekturnavigation und Validator müssen denselben Status ausweisen.

## Governance-Prüfung

Der eingecheckte Validator prüft fortlaufende eindeutige Nummern, zulässige Statuswerte, Pflichtabschnitte, bidirektionale Ersetzungsverweise, Indexvollständigkeit und den in `PROJECT_CONTROL/CURRENT_STATE.md` benannten Architekturstand. Ein fehlender oder widersprüchlicher Indexeintrag blockiert den Abschluss.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture v0.4"
[3]: ../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md "WP-004 – Architecture-v0.4-Abschlusskorrekturen"
