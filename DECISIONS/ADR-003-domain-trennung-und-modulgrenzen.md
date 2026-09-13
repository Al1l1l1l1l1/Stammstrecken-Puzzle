# ADR-003 – Reine Puzzle-Domain und gerichtete Modulgrenzen

## Status

**Ersetzt**

## Datum

2026-09-07

## Kontext

Die Kernlogik muss deterministisch, testbar und unabhängig von Unity-Szenen, SDKs sowie Chatwissen bleiben. Wechselnde KI-Agenten benötigen maschinell erzwingbare Grenzen statt nur informeller Schichten.

## Entscheidung

Die Produktionsstruktur verwendet explizite Unity-Assembly-Definitionen. `STP.Puzzle.Domain` und `STP.Puzzle.Solver` sind reine C#-Assemblies ohne `UnityEngine`-Referenz. `STP.Application` orchestriert Use Cases und definiert Ports. Adapter für Persistenz, Mobile-Dienste und Unity-Präsentation implementieren diese Ports. Nur `STP.Bootstrap` darf konkrete Adapter zusammenbauen.

Die erlaubte Richtung lautet:

`Domain <- Solver`, `Domain <- Application`, `Solver <- Application`, `Application <- Presentation`, `Application <- Infrastructure`, `Application <- MobileServices` und alle konkreten Module `-> Bootstrap` nur im Sinne der Composition-Root-Referenz.

Genauer gilt: `STP.Bootstrap` darf alle Module referenzieren; kein anderes Modul darf `STP.Bootstrap` referenzieren. `Presentation`, `Infrastructure` und `MobileServices` dürfen Domain-Datentypen nur über ausdrücklich freigegebene Contracts verwenden. Direkte SDK-Aufrufe außerhalb des zuständigen Adapters sind verboten.

Es wird kein Dependency-Injection-Framework eingeführt. Konstruktorinjektion und eine kleine manuelle Composition Root sind verbindlich. Globale Service-Locator, veränderliche Singletons und `FindObjectOfType` als Abhängigkeitsmechanismus sind verboten.

## Begründung

Assembly-Grenzen erzeugen Compilerfehler bei unerlaubten Abhängigkeiten. Eine manuelle Composition Root ist transparent, debuggbar und für Agenten leichter als ein reflektionsbasiertes Container-Framework. Die reine Domain kann ohne Szene, Gerät oder Netzwerk getestet werden.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Ein einziges Game-Assembly | Einfach am Anfang, aber keine erzwingbaren Grenzen und hohe Driftgefahr. |
| Umfangreiches Clean-Architecture-Framework | Zu viel Abstraktion für den Projektumfang. |
| Dependency-Injection-Container | Zusätzliche Laufzeitmagie, AOT-Risiko und versteckte Bindungen. |
| Unity-Komponenten als Domainmodell | Koppelt Regeln an Szenen, Lebenszyklen und Serialisierung. |

## Konsequenzen

Mehrere kleine Assemblies und Contract-Tests sind erforderlich. Gemeinsame Datentypen müssen bewusst platziert werden. Neue Drittanbieter-SDKs erhalten immer einen eigenen Adapter und dürfen keine Domainreferenz erzwingen.

## Betroffene Artefakte

`ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/PUZZLE_ENGINE.md`, `ARCHITECTURE/MOBILE_SERVICES.md` sowie die späteren `.asmdef`-Dateien.

## Validierung

CI prüft die Assembly-Definitionen gegen eine Allowlist und scannt reine Assemblies auf `UnityEngine`, SDK-Namensräume, statische Service-Locator und unerlaubte Referenzen.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ersetzt durch [ADR-013](./ADR-013-zyklusfreie-ports-und-modulgrenzen.md).

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/03_Raetselkern_und_Interaktionsmodell.md "Train Track Spiel – Rätselkern und Interaktionsmodell"
