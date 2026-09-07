# Architekturentscheidungen (ADR-System)

Der Ordner `DECISIONS/` enthält künftig **Architecture Decision Records (ADRs)**. Ein ADR ist ein dauerhaftes, nachvollziehbares Protokoll für eine technische Architekturentscheidung und ihre Begründung. Dieser Ordner enthält derzeit **keine Architekturentscheidung**.

## Zweck

ADRs verhindern, dass technische Entscheidungen nur in einzelnen Chats, Modellkontexten oder impliziten Implementierungsdetails existieren. Sie machen für jeden späteren Agenten sichtbar, welche technische Entscheidung gilt, warum sie getroffen wurde, welche Alternativen geprüft wurden und ob sie später ersetzt wurde.

## Wann ein ADR erforderlich ist

Ein ADR ist erforderlich, bevor eine technische Entscheidung umgesetzt wird, wenn sie die Architektur, Datenhaltung, Persistenz, Sicherheits- oder Datenschutzgrenzen, Plattformstrategie, externe Dienste, Integrationen, Build- oder Releasefähigkeit oder eine andere langfristige Systemgrenze beeinflusst.

Ein ADR ist kein Ersatz für eine Produktentscheidung. Er darf bestätigte Produktleitplanken nicht verändern. Fehlt eine notwendige Produktentscheidung, wird sie als Blocker dokumentiert statt durch einen ADR zu erfinden.

## Ablage und Dateinamen

Jeder ADR wird als eigene Markdown-Datei unmittelbar in diesem Ordner gespeichert. Das Format lautet:

```text
ADR-###-kurzer-titel.md
```

Die Nummer ist fortlaufend, eindeutig und wird nie wiederverwendet. Bereits angenommene ADRs werden nicht überschrieben oder still verändert.

## Verbindliche ADR-Struktur

| Abschnitt | Inhalt |
|---|---|
| Titel und ID | Eindeutige ADR-Nummer und kurze Bezeichnung. |
| Status | Entwurf, angenommen, ersetzt oder verworfen. |
| Datum | Datum der Dokumentation. |
| Kontext | Dokumentierte Ausgangslage, Anforderungen, Einschränkungen und relevante Projektquellen. |
| Entscheidung | Präzise, überprüfbare technische Entscheidung. |
| Begründung | Warum die Entscheidung den dokumentierten Anforderungen entspricht. |
| Betrachtete Alternativen | Relevante Alternativen und nachvollziehbare Gründe gegen ihre Wahl. |
| Konsequenzen | Erwartete technische Folgen, Risiken, Migrations- oder Prüfpflichten. |
| Betroffene Artefakte | Work Packages, Dateien, Module und Dokumentation, die von der Entscheidung berührt werden. |
| Validierung | Wie die Entscheidung geprüft oder nach ihrer Umsetzung nachgewiesen wird. |
| Ersetzt / ersetzt durch | Referenz auf frühere oder spätere ADRs, sofern vorhanden. |

## Lebenszyklus

Ein ADR beginnt als **Entwurf** und ist noch nicht verbindlich. Erst der Status **angenommen** macht eine technische Entscheidung verbindlich. Eine angenommene Entscheidung darf nur durch einen neuen ADR mit klarer Referenz ersetzt werden. Der frühere ADR bleibt erhalten und erhält den Status **ersetzt**. Ein verworfener ADR dokumentiert eine nicht angenommene Alternative und erzeugt keine Architekturvorgabe.

## Abgrenzung zum aktuellen Projektstand

Die Architekturversion des Projekts ist aktuell **nicht erstellt**. Dieses README definiert nur das Verfahren für künftige Entscheidungen und trifft weder eine Technologieauswahl noch eine Architektur-, Implementierungs- oder Datenmodellentscheidung.
