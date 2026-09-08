# ADR-013 – Zyklusfreie Ports und Modulgrenzen

## Status

**Angenommen**

## Datum

2026-09-08

## Kontext

ADR-003 hat die reine Puzzle-Domain, den separaten Solver, `STP.Application`, Adapter und eine manuelle Composition Root korrekt eingeführt. Die zusätzliche Assembly `STP.MobileServices.Contracts` war jedoch mit einer direkten Referenz auf `STP.Application` beschrieben, obwohl die Application-Schicht selbst die Ports besitzen sollte. Diese Darstellung machte Ownership und zulässige Abhängigkeitsrichtung mehrdeutig. Daneben waren `IHapticsPort`, der nicht beschlossene `ISaveSyncPort` und eine nicht existente Bezeichnung „Bootstrap-Verträge“ inkonsistent dokumentiert.

## Entscheidung

Die reine Kernstruktur und die manuelle Composition Root bleiben erhalten. **Alle providerneutralen Application-Ports, Ergebnisunionen und Adapter-DTOs gehören `STP.Application`.** Es gibt keine Produktionsassembly `STP.MobileServices.Contracts`. Konkrete Mobile-Service-Adapter referenzieren `STP.Application` und das jeweils notwendige SDK; `STP.Application` referenziert keinen Adapter und kein SDK.

`IHapticsPort` ist ein regulärer Application-Port und wird durch `STP.Platform` implementiert. `ISaveSyncPort` existiert nicht, solange weder Cloud-Synchronisation noch Konto- und Konfliktvertrag beschlossen sind. `STP.Editor.Build` referenziert die tatsächlich existente Assembly `STP.Bootstrap`; eine separate Assembly oder ein informeller Knoten „Bootstrap-Verträge“ existiert nicht.

Die einzige normative Allowlist aller internen Produktionsassembly-Referenzen steht in `ARCHITECTURE/MODULE_BOUNDARIES.md`. Sie enthält ausschließlich konkrete Assemblynamen und ist azyklisch. Externe Pakete werden getrennt als Paketreferenzen geprüft und bilden keine internen Assemblyknoten.

## Begründung

Ein Port wird vom inneren Verbraucher besessen. Dadurch hängt der Adapter vom Application-Vertrag ab, nicht umgekehrt. Die Entfernung spekulativer oder nicht existierender Erweiterungspunkte macht den Compilergraphen eindeutig und verhindert Zyklen. Cloud-Synchronisation kann später nur über eine bestätigte Produktentscheidung und ein ersetzendes ADR eingeführt werden.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Separate `STP.MobileServices.Contracts` ohne Application-Referenz | Möglich, aber eine zusätzliche Assembly ohne unabhängigen fachlichen Eigentümer erhöht die Fläche und verschiebt Ports aus ihrem Verbraucher. |
| `STP.Application -> STP.MobileServices.Contracts` | Zyklusfrei, widerspricht aber dem Grundsatz, dass Application die benötigten Ports selbst besitzt. |
| `ISaveSyncPort` als leere Zukunftsschnittstelle | Spekulativ und ohne bestätigten Cloud-, Konto- oder Konfliktvertrag nicht testbar. |
| Direkte SDK-Referenzen in Application | Verletzt Anbieterunabhängigkeit und reine Testbarkeit. |

## Konsequenzen

Mobile-, Store-, Plattform-, Persistenz- und Contentadapter implementieren direkt Ports aus `STP.Application`. Neue Ports oder Assemblies benötigen eine ADR-Prüfung und eine Änderung der maschinenprüfbaren Allowlist. Die bisherigen positiven Grenzen für Domain, Solver, Commands und Composition Root bleiben unverändert.

## Betroffene Artefakte

`ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/ARCHITECTURE.md`, `ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/TEST_STRATEGY.md` und spätere `.asmdef`-Dateien.

## Validierung

Der eingecheckte Architekturvalidator liest die normative Assembly-Allowlist, prüft alle Knoten und Kanten, weist unbekannte Namen ab und führt eine Zyklenerkennung aus. Contracttests kompilieren Application mit Fakes und jeden Adapter ausschließlich gegen `STP.Application` und sein SDK.

## Ersetzt / ersetzt durch

Ersetzt [ADR-003](./ADR-003-domain-trennung-und-modulgrenzen.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/MODULE_BOUNDARIES.md "Module Boundaries v0.2"
[2]: ../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md "WP-002 – Architecture-v0.2-Korrekturen"
