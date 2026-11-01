# WP038 – tatsächliche lokale Unity-/Grafikeigenprüfung

**Eigene Laufzeit-/Bildprüfung PASS für den unten gebundenen Source-Snapshot; keine unabhängige Abschlussfreigabe und kein regulärer Codefreeze-CI-PASS.**

Unity6000.3.23f1, ProductVersion6000.3.23f1_09d2ecc7fb28, WindowsFileVersion6000.3.23.643820. Nach vollständigem Parent-CI-Ende und acht tatsächlich bestätigten Seatdeaktivierungen lief genau eine lokale Editorinstanz zurzeit. Generate und anschließend Graphics beide terminaler nativer Exit0; ausdrücklich ohne -nographics. Tatsächlicher D3D11-Renderer AMD Radeon RX9070XT, kein Nullrenderer. Die eng erlaubte zusätzliche CI-Grafikroute wurde deshalb nicht benötigt und nicht in den Workflow eingefügt.

## Tatsächlich gelesene Original-XML

8/8 Passed,0 Failed,0 Skipped,0 Inconclusive; Dauer6.8248068s. Die Original-XML ist im [Evidenzobjekt](GRAPHICS_EVIDENCE.json) als unveränderte gzip/base64-Payload gesichert. Enthalten sind UI-AllTools/Selektion/atomarerBatch/Undo/Noop, echte SolvedPath→eineFahrt→ungespeichertesResult→Neustart, Lifecycle/Suspend-/Callbackcleanup, negative Profil-/Referenz-/Keygrenzen und doppelte Builderreproduktion einschließlich Force-Synchronous-Reimport mit unveränderten Bytes/GUIDs. Drei eigene Grafikfälle sind tatsächlich Passed und verlangten STP_REQUIRE_GRAPHICS=1/Nicht-Nullrenderer, tatsächlich gerenderte Panelpixel und sichtbare Trainpixel im echten Worldtarget. Die vollständigen Testnamen und Assertionausgaben bleiben im Original-XML gebunden.

## Selbst betrachtete tatsächliche Bilder

Root hat alle drei Original-PNGs mit view_image betrachtet. 4×4 und10×10: lesbare A/B und öffentliche sichtbare/ Ziel-Counts, getrennte Randbereiche, alle sechs Trackformen plus X/Grau/Leeren und Aktionsknöpfe im960×840-Bild sichtbar; rote X-Notiz, graue Belegungsnotiz, gelbe Auswahl und aktives Werkzeug erkennbar. Fahrtbild: neutraler sichtbarer Train auf der tatsächlich eingegebenen L-Strecke, passende objektiv exakte Counts, gesperrte Eingaben und Fahrtphase; noch kein Result. Das separate echte UI-Szenentestassert belegt die Resultanzeige erst nach Fahrtende und danach den Neustart mit neuer Versuchidentität.

- [wp038-4x4-editing.png](NATIVE_INITIAL_wp038-4x4-editing.png): 960×840, 78574Bytes, SHA256`b73666e0c53a886d8faadd32b8015db6ae68444f69b39944d66e804cfaf139b3`.
- [wp038-10x10-editing.png](NATIVE_INITIAL_wp038-10x10-editing.png): 960×840, 83416Bytes, SHA256`78ece007e81f0e3348eaa982ff06ae0a89abf828096cdf01ce2d5df5e6ae3eff`.
- [wp038-4x4-ride.png](NATIVE_INITIAL_wp038-4x4-ride.png): 960×840, 75839Bytes, SHA256`6b31dd73a5a633fd4059e550870ea044d6336e22b7ba1a1a38eb123923fa40df`.

## Byteprofile und Grenzen

GRAPHICS_EVIDENCE.json: 285314Bytes, SHA256`f624c5781ef6bb5d8af0ffd9b290d3c5b790c32eb1c3711cd02b73b9fb517c2c`. Eigene Sourcebindungen erfassen rohe Dateibytes und kanonisches LF-Profil getrennt. Vollständige dekodierte Generate-/Graphicslogprofile sind nach expliziter Entfernung sensitiver Konto-/Lizenzmetadaten gzip/base64 gesichert; diese Profile werden nicht als unveränderte Rohlogs bezeichnet. Originale Rohlogbytes bleiben lokal im ignorierten Laufverzeichnis mit exaktem Digest und Bytezahl für die getrennte Prüfung zugänglich. Alle persistierten Payloads wurden dekomprimiert und byte-/hashgeprüft.

Der Editor schrieb automatisch drei zuvor unveränderte geschützte Projektsettings um; Originaldiff wurde gesichert und gelesen, anschließend wurden exakt diese Dateien auf ihren vorigen HEAD zurückgesetzt. Kein Profil-/Coverage-/Renderpipeline-/Gatewechsel wird übernommen.

Dies ist ein zusätzlicher lokaler QA-Lauf. Reguläre vollständige CI mit Coverage, Android-IL2CPP und iOS/Xcode sowie unabhängige Abschluss-QC auf Codefreeze stehen noch aus. Kein menschlicher Kampagnenrätseltest, physischer Geräte-/Touch-/Accessibility-/Performance-/Privacy-/Store-/Releasebeleg; kein Produktionsimport, Save, Reward oder Fortschrittsclaim.

## Historischer unabhängiger konkreter FAIL vor Freeze
Die acht Laufzeitfälle bestanden, aber unabhängige GUID-Prüfung fand abweichende zufällige Panel-/Materialidentitäten. Deshalb AK08/Artefakt-Gesamtprüfung FAIL; kein Abschluss-PASS. Originale Source-/Meta-/XML-/Logbindungen bleiben in GRAPHICS_INITIAL_EVIDENCE.json erhalten. Der folgende Korrektur-/Nachprüfschritt wird separat gebunden.
