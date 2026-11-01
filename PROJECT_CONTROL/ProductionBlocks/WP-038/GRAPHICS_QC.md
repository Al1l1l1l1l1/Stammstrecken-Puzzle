# WP038 – tatsächliche lokale Unity-/Grafikeigenprüfung

**Eigene Laufzeit-/Bildprüfung PASS für den unten gebundenen Source-Snapshot; keine unabhängige Abschlussfreigabe und kein regulärer Codefreeze-CI-PASS.**

Unity6000.3.23f1, ProductVersion6000.3.23f1_09d2ecc7fb28, WindowsFileVersion6000.3.23.643820. Nach vollständigem Parent-CI-Ende und acht tatsächlich bestätigten Seatdeaktivierungen lief genau eine lokale Editorinstanz zurzeit. Generate und anschließend Graphics beide terminaler nativer Exit0; ausdrücklich ohne -nographics. Tatsächlicher D3D11-Renderer AMD Radeon RX9070XT, kein Nullrenderer. Die eng erlaubte zusätzliche CI-Grafikroute wurde deshalb nicht benötigt und nicht in den Workflow eingefügt.

## Tatsächlich gelesene Original-XML

8/8 Passed,0 Failed,0 Skipped,0 Inconclusive; Dauer 5,5200498 s. Die Original-XML ist im [Evidenzobjekt](GRAPHICS_EVIDENCE.json) als unveränderte gzip/base64-Payload gesichert. Enthalten sind UI-AllTools/Selektion/atomarerBatch/Undo/Noop, echte SolvedPath→eineFahrt→ungespeichertesResult→Neustart, Lifecycle/Suspend-/Callbackcleanup, negative Profil-/Referenz-/Keygrenzen und doppelte Builderreproduktion einschließlich Force-Synchronous-Reimport mit unveränderten Bytes/GUIDs. Drei eigene Grafikfälle sind tatsächlich Passed und verlangten STP_REQUIRE_GRAPHICS=1/Nicht-Nullrenderer, tatsächlich gerenderte Panelpixel und sichtbare Trainpixel im echten Worldtarget. Die vollständigen Testnamen und Assertionausgaben bleiben im Original-XML gebunden.

## Selbst betrachtete tatsächliche Bilder

Root hat alle drei Original-PNGs mit view_image betrachtet. 4×4 und10×10: lesbare A/B und öffentliche sichtbare/ Ziel-Counts, getrennte Randbereiche, alle sechs Trackformen plus X/Grau/Leeren und Aktionsknöpfe im960×840-Bild sichtbar; rote X-Notiz, graue Belegungsnotiz, gelbe Auswahl und aktives Werkzeug erkennbar. Fahrtbild: neutraler sichtbarer Train auf der tatsächlich eingegebenen L-Strecke, passende objektiv exakte Counts, gesperrte Eingaben und Fahrtphase; noch kein Result. Das separate echte UI-Szenentestassert belegt die Resultanzeige erst nach Fahrtende und danach den Neustart mit neuer Versuchidentität.

- [wp038-4x4-editing.png](wp038-4x4-editing.png): 960×840, 78574Bytes, SHA256`b73666e0c53a886d8faadd32b8015db6ae68444f69b39944d66e804cfaf139b3`.
- [wp038-10x10-editing.png](wp038-10x10-editing.png): 960×840, 83416Bytes, SHA256`78ece007e81f0e3348eaa982ff06ae0a89abf828096cdf01ce2d5df5e6ae3eff`.
- [wp038-4x4-ride.png](wp038-4x4-ride.png): 960×840, 75841Bytes, SHA256`70f1b7a13c242c073dd80650a650fbeb5610d4857a5aab5fa4f319cbb7234613`.

## Byteprofile und Grenzen

GRAPHICS_EVIDENCE.json: 55402Bytes, SHA256`4a162344772afa70e657a408ed8349359a0cbf806b34a4b571efa0db6e5fe386`. Eigene Sourcebindungen erfassen rohe Dateibytes und kanonisches LF-Profil getrennt. Vollständige dekodierte Generate-/Graphicslogprofile sind nach expliziter Entfernung sensitiver Konto-/Lizenzmetadaten gzip/base64 gesichert; diese Profile werden nicht als unveränderte Rohlogs bezeichnet. Originale Rohlogbytes bleiben lokal im ignorierten Laufverzeichnis mit exaktem Digest und Bytezahl für die getrennte Prüfung zugänglich. Alle persistierten Payloads wurden dekomprimiert und byte-/hashgeprüft.

Der Editor schrieb automatisch drei zuvor unveränderte geschützte Projektsettings um; Originaldiff wurde gesichert und gelesen, anschließend wurden exakt diese Dateien auf ihren vorigen HEAD zurückgesetzt. Kein Profil-/Coverage-/Renderpipeline-/Gatewechsel wird übernommen.

Dies ist ein zusätzlicher lokaler QA-Lauf. Reguläre vollständige CI mit Coverage, Android-IL2CPP und iOS/Xcode sowie unabhängige Abschluss-QC auf Codefreeze stehen noch aus. Kein menschlicher Kampagnenrätseltest, physischer Geräte-/Touch-/Accessibility-/Performance-/Privacy-/Store-/Releasebeleg; kein Produktionsimport, Save, Reward oder Fortschrittsclaim.

## Erhaltener Befund und gezielte Korrektur

Die initialen acht Laufzeitfälle bestanden, aber die unabhängige Meta-/GUIDprüfung ergab AK08-FAIL: zufällige Panel-/Materialidentitäten statt der prospektiven festen GUIDs. [Initiale Evidenz](GRAPHICS_INITIAL_EVIDENCE.json) und [historischer Laufbericht](GRAPHICS_INITIAL_QC.md) bleiben erhalten; die getrennte Prüfrolle sicherte außerdem die initialen PNGbytes. Die Korrektur bindet GUIDs erst nach CreateAsset, erzwingt synchronen Reimport und prüft Meta sowie AssetDatabase-Identität aller drei Artefakte auch im bestehenden Pfad. Der Buildertest verlangt die drei konkreten Vertrags-GUIDs. Ein weiterer tatsächlicher Generate-Lauf erzeugte die beiden zuvor gesicherten und entfernten eigenen Assets aus dem fehlenden Zustand neu, mit festen GUIDs und kanonischer YAML-Ausgabe. Danach bestand der betroffene vollständige Achtfall-Szenen-/Grafiklauf erneut; kein Solver-/Textgesamtauftrag wurde wiederholt.
