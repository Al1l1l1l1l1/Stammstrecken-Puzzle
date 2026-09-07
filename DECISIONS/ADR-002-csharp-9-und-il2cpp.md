# ADR-002 – C# 9 und IL2CPP als Sprach- und Release-Runtime

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Die Codebasis muss von wechselnden KI-Agenten sicher bearbeitbar sein. Sie benötigt statische Typisierung, kleine Schnittstellen, gute Testbarkeit und denselben Quellcode auf Android und iOS. Unity 6 unterstützt offiziell C# 9; nicht unterstützte Sprachmerkmale führen zu Compilerfehlern.[1]

## Entscheidung

Produktionscode wird ausschließlich in **C# 9** geschrieben. Nicht offiziell unterstützte neuere Sprachmerkmale, Compiler-Austausch, Source Weaver und Laufzeit-Codegenerierung sind verboten. Nullable Reference Types werden projektweit aktiviert. Warnungen der eigenen Assemblies werden im Continuous-Integration-System als Fehler behandelt; begründete Unterdrückungen erfolgen lokal mit Kommentar.

Release-Builds für Android und iOS verwenden **IL2CPP**. Der Mono-Backend bleibt auf Editor-Iteration und ausdrücklich freigegebene lokale Entwicklungsbuilds beschränkt. Reflexion wird in Domain und Application vermieden; notwendige SDK-Reflexion wird in Adapter-Assemblies isoliert und über Linker-Konfiguration getestet.

Unity-serialisierte Klassen sind auf Präsentation und Infrastruktur begrenzt. Domainmodelle sind normale, unveränderliche C#-Typen. C#-Records werden nicht für Unity-serialisierte Typen verwendet.

## Begründung

C# 9 ist die offiziell unterstützte, reproduzierbare Sprachbasis der gewählten Engine. IL2CPP entspricht dem mobilen Releasepfad, deckt AOT- und Stripping-Probleme vor Veröffentlichung auf und ermöglicht native Symbolik für Fehlerdiagnose. Die Einschränkung verhindert agentenabhängige Compilertricks.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Neuere C#-Version erzwingen | Nicht offiziell unterstützt und erhöht Build- sowie Agentenrisiko. |
| Visual Scripting | Erschwert Diffprüfung, automatisierte Analyse und präzise Modulgrenzen. |
| Native Swift/Kotlin-Erweiterungen für Spiellogik | Erzeugt doppelte Domainimplementierungen; nur dünne Plattform-Plugins sind zulässig. |
| Mono in Release | Einfachere Iteration, deckt aber AOT- und Strippingfehler nicht zuverlässig vor dem Store-Build auf. |

## Konsequenzen

Alle Bibliotheken müssen AOT- und IL2CPP-tauglich sein. Generische und reflektionsbasierte Serialisierung benötigt Linker-Tests. Entwickler und Agenten dürfen keine Syntax oberhalb C# 9 einführen.

## Betroffene Artefakte

`ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/TEST_STRATEGY.md` und `ARCHITECTURE/BUILD_AND_RELEASE.md`.

## Validierung

CI kompiliert alle Assemblies mit dem Unity-Compiler, behandelt eigene Warnungen als Fehler und erstellt regelmäßig IL2CPP-Development-Builds sowie Release-Kandidaten für beide Plattformen.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://docs.unity3d.com/6000.3/Documentation/Manual/csharp-compiler.html "C# compiler and language version reference"
[2]: ./ADR-001-unity-6-3-lts.md "ADR-001 – Unity 6.3 LTS als Game Engine"
