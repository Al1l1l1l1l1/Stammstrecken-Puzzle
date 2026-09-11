# ADR-018 – Kompilierbare Bootstrap-Composition-Root

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

ADR-013 ordnet alle providerneutralen Ports `STP.Application` zu und lässt konkrete Adapter von dieser Assembly abhängen. Die v0.2-Allowlist erlaubte `STP.Bootstrap` jedoch alle konkreten Adapter, nicht aber `STP.Application`. Eine manuelle Composition Root konnte damit die Use Cases und ihre Ports nicht als C#-Typen sehen.

## Entscheidung

`STP.Bootstrap` erhält genau eine zusätzliche direkte interne Referenz: `STP.Application`. Seine vollständige direkte interne Referenzmenge lautet damit:

- `STP.Application`
- `STP.Infrastructure.Content`
- `STP.Infrastructure.Persistence`
- `STP.MobileServices.Google`
- `STP.MobileServices.Store`
- `STP.Platform`
- `STP.Audio`
- `STP.Presentation.UI`
- `STP.Presentation.World`

Direkte Bootstrap-Referenzen auf `STP.Puzzle.Domain`, `STP.Puzzle.Solver`, Testassemblies oder eine neue Contracts-/DI-Assembly sind nicht erlaubt. Die Root verwendet manuelle Konstruktorinjektion; Service Locator, veränderliche globale Registry, Reflexions-Wiring und Szenensuche sind ausgeschlossen.

Die Compile-Referenz auf einen Adapter gestattet keine eager Providerinitialisierung. Ads, IAP, Analytics und Crashdiagnose bleiben hinter den jeweils geltenden Capability-, Privacy- und Recovery-Gates.

## Begründung

Die zusätzliche Application-Kante ist das kleinste kompilierbare Wiring-Modell. Domain und Solver bleiben transitive innere Abhängigkeiten. Dadurch bleibt der Graph azyklisch und die in ADR-013 festgelegte Port-Eigentümerschaft unverändert.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Direkte Bootstrap-Referenzen auf Domain und Solver | Nicht minimal und ohne konkrete Konstruktoranforderung unbelegt. |
| Ports in eigene Contracts-Assembly auslagern | Zusätzliche Abstraktionsschicht ohne Bedarf; widerspricht ADR-013. |
| Service Locator oder Reflexion | Verbirgt Compilefehler und verletzt die gerichteten Modulgrenzen. |

## Konsequenzen

Die normative Assemblytabelle und ihre Mermaid-Projektion enthalten dieselbe neunte Bootstrapkante. Der spätere Produktions-Scaffold muss `.asmdef`-Referenzen gegen diese exakte Menge prüfen und einen Composition-Smoke ausführen. Da noch kein Unity-Projekt existiert, bleibt der echte Compile-/PlayMode-Nachweis ein sichtbares späteres Gate.

## Betroffene Artefakte

`ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/TEST_STRATEGY.md`, Architekturvalidator und späterer Unity-Scaffold.

## Validierung

Der v0.3-Validator vergleicht die vollständige Bootstrapmenge mit der normativen Tabelle, prüft Tabelle und Diagramm gegeneinander und mutiert fehlende sowie unzulässige Kanten. Später kompiliert ein PlayMode-Smoke die reale Root und belegt vollständige eindeutige Bindings ohne Providerstart.

## Ersetzt / ersetzt durch

Ergänzt [ADR-013](./ADR-013-zyklusfreie-ports-und-modulgrenzen.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/MODULE_BOUNDARIES.md "Module Boundaries v0.3"
[2]: ../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.3"
[3]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
