# WP-035 – Konsistenz und Eigenprüfung

## Rollen- und Liefergrenze

Dies ist die Eigenprüfung des ausführenden Gestaltungsagenten, keine unabhängige QC, keine GF-Abnahme und keine Produkt-, Rechts-, Runtime- oder Releasefreigabe. Die Parallelbausteine wurden nur als Fachausarbeitung verwendet; Architektur, Produktionsplanung und Abnahmerollen wurden nicht neu delegiert oder eingeführt. Der Hauptagent hat die Kapitel und Referenzen selbst zusammengeführt und gesichtet.

## Gemeinsam konsolidierte Werte

Die ursprünglichen Teilentwürfe enthielten unterschiedliche Detailwerte. Vor Lieferung wurden sie in den Kapiteln, HTML-Quellen, Token-JSON und Inventar abgeglichen:

| Gegenstand | Einheitlicher Vorschlag |
|---|---|
| Wortmarke | IBM Plex Sans Condensed SemiBold, +0,026em; Primärsatz 52 px, Kompaktsatz 28/30 px; einzeilig. |
| Gleise | Sechs Formen; normierte Halbkantenzentrum-Ports; 18/100 Strich; Viertelkreisradius 50/100; bündige Enden. |
| Graue Belegung | Kantenfreies kleines Standardgleis aus zwei kurzen Strichen und drei Schwellen, innerhalb 0,36C×0,20C; keine graue Anschlussform. |
| Zellauswahl | 38 % Gelb auf Papier; unveränderte Inhaltsfarbe; innenliegende Kontur 0,028C. Werkzeugauswahl: volle gelbe Fläche. |
| Große Raster | Derselbe 18/100-Gleisstrich, dieselben zehn direkten Feldwerkzeuge; keine neue Bedienpolitik. |
| Zählstatus | Offen/erfüllt/überschritten ausschließlich aus der gezeigten Belegung; Grau zählt als Belegung, X nicht. Keine versteckte Lösung. |
| Ergebnis/Tagesmeldung | Gelb ist keine Belohnungs- oder Ergebnisfarbe; Weiß/Anthrazit/Markenrot tragen die sachliche Darstellung. |
| Fahrzeugmaß | Wagenkastenhöhe H; L=3,18H, W=0,72H. Weltmodule benutzen H, nicht einen konkurrierenden fiktiven Metermaßstab. |
| Puzzlefahrt | Körperlänge 0,78C und Breite 0,24C; die Portkoordinaten bleiben Halbkantenzentren. |
| Abschluss | Hilfen zurücknehmen → tatsächlicher Pfad → Zug A–B → Ergebnis. |
| Audiogrenze | Semantische Cues und Mixbrief geliefert; keine Audiodatei oder Hörprüfung behauptet. |

## Sichtprüfung der tatsächlich gerenderten Ansichten

Der erste Renderingdurchgang zeigte unter anderem einen CSS-Namenskonflikt zwischen den Zeitachsen-Schritten „train“ und einer Fahrzeugklasse, unverbundene Schienen im Ergebnisfixture, eine nicht passend kleine Gleisstrichstärke im 10×10-Fixture sowie uneinheitliche Referenzfontpfade. Diese eigenen Fehler wurden innerhalb des Dokumentationsscopes korrigiert und die betroffenen Ansichten erneut gerendert. Die finalen zehn PNGs werden vollständig gesichtet, nicht nur ihre Dateinamen geprüft.

Die Referenzen 01/02/03/04/07/08 und die beiden zusätzlichen Konstruktionsblätter werden aus ihren lokalen HTML-Quellen mit Chromium erzeugt. `build_references.py` protokolliert Seitengröße, geladene Schriftfamilien und JavaScript-Seitenfehler; die Seiten enthalten keine App- oder Spiellaufzeit. Die Original-TTFs liegen ausschließlich im Referenzordner. Das ist keine Freigabe zum Unity-Import.

## Generierte Materialtafeln – ausdrücklich keine Maßautorität

`05-vehicles-props.png` und `06-worlds.png` wurden am 17.10.2026 als originale AI-Referenztafeln erzeugt, jeweils 2560×1440 PNG. Ihre Herkunft ist die Manus-Bildgenerierung aus den eigenen, dokumentierten Fahrzeug- und Weltbildbriefen, nicht ein übernommenes Suchbild oder ein Screenshot eines bestehenden Spiels. Kein generiertes Modell, keine Texturdatei und keine Audiodatei wurde damit geliefert.

Die Tafeln zeigen die gewünschte materielle Bahnfahrerwelt, die getrennten roten Lackfragmente, die fünf Räume und den Schienenersatzverkehr. **Nicht maßhaltig** sind einzelne erzeugte Frontfenster-, Signal-, Portal- und Propdetails. Die generierte Vegetation und Schattierung begründen keine zusätzlichen UI-Farbtoken. Die erzeugten Weltbeschriftungen sind keine nachgewiesene IBM-Plex-Textsetzung. Für diese Punkte sind die exakten Markdown-/SVG-/HTML-Konstruktionen und die tatsächlich geladenen Originalschriften maßgeblich; die AI-Tafeln sind Material-/Raumreferenzen, keine Produktionsmaster. Die zusätzlichen Blätter `05-vehicles-construction.html` und `06-world-construction.html` samt PNG machen diese Trennung praktisch nutzbar.

Diese Einschränkung ist Teil der dokumentierten Lieferung und darf nicht als stillschweigende Annahme aller AI-Details durch einen späteren Produktionsagenten verloren gehen.

## Nachweise und Reproduktion

- `font-provenance.md`, `font-license-original.zip` und die lesbare Textkopie: gepinnte IBM-Originalquelle, bytegleicher ZIP-Eintrag, SHA-256 und OFL-Bedingungen; keine kostenpflichtige Beschaffung.
- `asset-requirements.csv`: 40 eindeutige Anforderungszeilen; tatsächliche lokale Referenzdateien von künftig zu produzierenden Runtimeassets getrennt.
- `delivery-validation.json`: Datei-, Link-, PNG-, SVG-, JSON-, Originalfont-Glyphen- und Fremdquellenprüfung.
- `render-results.json`: tatsächlich erzeugte Ansichten und Schriftladezustand.
- `file-manifest.json`: SHA-256 und Bytegröße aller gelieferten Dateien; enthält sich selbst nicht und wird nach den letzten Dateianpassungen erneuert.
- Der bestehende Architekturvalidator wird mit dem gemeinsam verankerten, unveränderten `WP-035.documentation.scope.json` und `--self-test` ausgeführt; Ergebnis und genaue Umgebung stehen in den zugehörigen Logs.
- Reguläre GitHub-CI wird am eigenen Draft-PR ausgeführt. Der vorhandene Workflowselektor verweist auf WP-027. Diese nicht beauftragte Workflowkonfiguration wird weder geändert noch umgangen: Ein daraus entstehender CI-Fehler wird als tatsächlicher fehlender Abschlussnachweis dokumentiert, nicht als neue Produktregel oder pauschaler Projektstopp.

## Was die Eigenprüfung nicht belegt

Kein menschlicher Spieltest, kein Hörtest, kein Zielgeräte-/Accessibility-Nachweis, keine rechtliche Markenprüfung, kein Unity-Import und keine funktionsfähige Zugfahrt wurden durchgeführt. Das verhindert die vollständige Entwurfsausarbeitung nicht, verhindert aber ihre falsche Kennzeichnung als Produktions- oder unabhängige Abschlussfreigabe.

## Anker- und Formatkorrektur vor Erstveröffentlichung

Der bestehende Validator verlangte die WP-ID zusätzlich als eigenständige Codezeile. Der erste eigene, noch unveröffentlichte WP-035-Anker hatte sie unformatiert; dieser Formfehler wurde nach vollständiger lokaler Sicherung vor jeder Veröffentlichung durch korrektes erneutes Erst-Add auf derselben Mainbasis bereinigt. Keine historische fremde WP-Definition und kein fremdes Manifest wurde geändert. Der eigene Manifestinhalt blieb identisch. Zeilenendleerzeichen der eigenen Texte wurden entfernt; die Originallizenz ist im ZIP bytegenau geschützt. Die tatsächlichen Ergebnisse beider Eigenprüfungsdurchgänge bleiben protokolliert.
