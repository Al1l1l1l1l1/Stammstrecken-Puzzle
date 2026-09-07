# ADR-001 – Unity 6.3 LTS als Game Engine

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Stammstrecken-Puzzle benötigt eine gemeinsame Produktionsbasis für Android und iOS, eine leistungsfähige 2D-/2.5D-Darstellung, mobile SDK-Integrationen, deterministische Tests im Continuous-Integration-System und eine für wechselnde KI-Coding-Agenten gut dokumentierte Projektstruktur. Das Produkt enthält eine konzentrierte Rasteroberfläche sowie eine inszenierte Miniaturbahnwelt nach der Lösung.

Unity 6.3 ist die aktuelle Long-Term-Support-Linie. Unity empfiehlt die LTS-Linie für Produktionen, die sich auf eine bestimmte Version festlegen; 6.3 LTS erhält regulären Support bis Dezember 2027.[1] Diese Entscheidung betrifft nur die technische Produktionsbasis und verändert keine Produktregel.

## Entscheidung

Das Spiel wird mit **Unity 6.3 LTS**, Release-Linie **6000.3**, und der initialen exakten Patchversion **`6000.3.23f1`** entwickelt. `ProjectSettings/ProjectVersion.txt`, CI-Runnerimage und Buildmetadaten müssen denselben Wert tragen. Builds dürfen niemals implizit auf einen anderen Patch ausweichen.

Ein Patchwechsel innerhalb `6000.3` ist nur in einem eigenen Pull Request zulässig. Dieser Pull Request ändert den einen autoritativen Toolchain-Lock, aktualisiert alle daraus generierten Stellen und muss Domain-, EditMode-, PlayMode-, Android- und iOS-Smoke-Tests erfolgreich ausführen. Ein Wechsel auf eine andere Unity-Release-Linie erfordert ein neues ADR.

Die Universal Render Pipeline wird für die 2D-/2.5D-Präsentation verwendet. Unity-Physik ist kein Bestandteil der Puzzle-Regeln; die Puzzle-Domain bleibt eine reine C#-Bibliothek.

## Begründung

Unity liefert einen bewährten gemeinsamen Editor, mobile Exportpfade, Asset-Pipelines und ein großes SDK-Ökosystem. Die LTS-Festlegung verringert Upgrade-Rauschen und macht Builds für autonome Agenten reproduzierbar. Die Trennung der Domain verhindert, dass die Engine die testbare Rätsellogik bestimmt.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Godot | Technisch geeignet und offen, aber das mobile Monetarisierungs- und Diagnoseökosystem erfordert mehr eigene Integrationsarbeit. |
| Native Swift und Kotlin | Maximale Plattformnähe, aber doppelte Präsentations- und Integrationsarbeit sowie erhöhte Driftgefahr zwischen iOS und Android. |
| Flutter mit Spielebibliothek | Für App-Oberflächen geeignet, aber die Miniaturbahn-Inszenierung, Assetpipeline und Game-Tooling wären weniger direkt. |
| Unity Supported/Update Release 6.6 | Produktionsreif, aber eine beweglichere Release-Linie bietet für diesen stabilitätsorientierten Produktionsstart keinen ausreichenden Vorteil. |

## Konsequenzen

Unity-Lizenzen und passende Build-Agenten sind für CI erforderlich. Plattform- und Storeanforderungen müssen trotz LTS regelmäßig geprüft werden. Unity-Typen dürfen die Puzzle-Domain nicht erreichen. Engine-Updates werden bewusst als kontrollierte Änderungen behandelt.

## Betroffene Artefakte

`ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md` und alle späteren Unity-Projektdateien.

## Validierung

Der erste Produktions-Scaffold muss die exakte Editorversion ausgeben, einen leeren Android- und iOS-Build erzeugen und die headless Test-Suite ausführen. CI vergleicht die verwendete Version mit `ProjectVersion.txt`.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://unity.com/releases/unity-6/support "Unity 6 release support"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md "Stammstrecken-Puzzle – Projektübergabe und Gesamtstatus"
[3]: ../WORK_PACKAGES/WP-ARCH-001_Technische_Produktionsspezifikation.md "WP-ARCH-001 – Technische Produktionsspezifikation"
