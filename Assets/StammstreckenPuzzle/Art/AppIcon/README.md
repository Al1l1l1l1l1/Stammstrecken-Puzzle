# Originales STP-Appsymbol

`stp-appicon-master.svg` ist der code-native 1024er-Vektormaster. Der rote Zug, das cremefarbene Band, die dunklen Konturen und die braune Gleiskurve greifen die vorhandene originale RailwayDrawing-Bildsprache auf. Kein Text, Logo, externes Bild oder fremdes Fontasset befindet sich im Icon.

`stp-appicon-1024.png` und die weiteren `stp-appicon-<Größe>.png` sind quadratische, deckende sRGB-Renderings ohne eingebrannte Eckmaske. Die kleinen Größen bis 16 px wurden zusätzlich in `preview-sizes-and-masks.png` gemeinsam angesehen.

Für Android liegen separate Vordergrund-, Hintergrund- und Monochromebenen als SVG sowie 1024-/432-PNG vor. Der tatsächliche Vordergrund-Alphabereich liegt innerhalb der zentralen 66/108-Sicherheitszone; die ermittelte maximale Radialausdehnung steht in `provenance.json`. Kreis- und Rundrechteckbilder auf der Kontaktvorschau sind ausschließlich Maskenvorschauen, keine auszuliefernden Eckmasken.

`build-appicon.cjs` rendert die eigenen SVGs direkt über sharp/libvips/librsvg. Node muss `sharp` über seine normale Modulauflösung erreichen können, etwa über das lokal eingerichtete `NODE_PATH`. Der Generator schreibt in diesen Ordner. `provenance.json` bindet Stilquelle, aktuelle Originalzeichenquelle, Renderer, Dateien und Generator-SHA-256. Eine erneute Generierung setzt die Sichtungsmarkierung wieder zurück.

R hat die Originale in diesen Assetordner übernommen und die unveränderten Renderings mit A verglichen. `MobileIconAssets.ConfigureForActiveTarget` setzt die tatsächlichen 2D-Textureimporte und Plattformslots für das vorgewählte mobile Ziel ausdrücklich. iOS- und Android-Zuweisung wurden im Editor erfolgreich geprüft. Mobile-Builds prüfen die gespeicherte Zuweisung, ohne sie still zu reparieren; native Iconressourcen werden am nächsten jeweiligen Build geprüft. Keine Geräte-, Owner- oder Storeannahme wird daraus abgeleitet.

Formatgrundlagen, geprüft am 21.10.2026: [Android Adaptive Icons](https://developer.android.com/develop/ui/compose/system/icon_design_adaptive), [Apple App Icons](https://developer.apple.com/design/human-interface-guidelines/app-icons).
