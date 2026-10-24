# WP038 – unabhängige native Teilprüfung des ersten Grafik-/Builderstands

**Historischer Teilprüfbefund: FAIL NATIVE-01 – feste Panel-/Material-GUIDs nicht umgesetzt.** Noch keine unabhängige Abschlussfreigabe. Die Laufzeit-/Bildbelege des ersten unveränderten QA-Darstellungspfads sind tatsächlich vorhanden; ein bestandener Reproduktionstest ersetzt die gesonderte feste Identitätsbindung nicht. Implementierung und Korrektur bleiben getrennt von Prüfrolle `/root/wp037_independent_qc`.

## Tatsächlich eigenständig bestätigte Originalbelege

Die ursprüngliche [GRAPHICS_INITIAL_EVIDENCE.json](GRAPHICS_INITIAL_EVIDENCE.json), SHA256 `f624c5781ef6bb5d8af0ffd9b290d3c5b790c32eb1c3711cd02b73b9fb517c2c`, vollständig gelesen. Alle drei gzip/base64-Payloads selbst dekomprimiert, gepackte/ungepackte Bytezahlen und SHA256 verifiziert. XML-Payload ist exakt gleich dem nativen Original graphics.xml. Beide Rohloghashes/Bytezahlen stimmen mit den nativen archivierten Logs überein; vollständige deklarierte LF-/Account-/Lizenzredaktionsprofile zeilenweise gegen die Originale geprüft. Alle nicht redigierten Zeilen stimmen, 422 beziehungsweise 29 ersetzte Zeilen entsprechen dem erklärten Muster. Kein Rohlog wird als redigiertes Profil ausgegeben.

Original-XML SHA256 `1f9ec453b5f6ff90bb8902c8b991b18c9f4abf039972f647a5890a47b57b14fc`, 12871Bytes: exakt acht tatsächlich Passed ausgeführte DevelopmentPuzzleSmokeTests, keine Failed/Skipped/Inconclusive, einschließlich aller drei Grafikfälle und der realen Builder-/Reimportprobe. Der übergeordnete Testinventarcount ist kein zusätzlich ausgeführter Testumfang. Der tatsächliche lokale D3D11-/RX9070XT-Renderer und der grafische Lauf ohne -nographics sind in den nativen Logs gebunden. Dies ersetzt keine reguläre gesamte Codefreeze-CI.

Alle drei ursprünglichen PNGs selbst über view_image betrachtet: 4×4 und10×10 mit lesbaren öffentlichen Counts/A/B, sechs Gleisformen und X/Grau/Leeren, explizitem Batchanwenden und gelber Selektion; Fahrtbild mit sichtbarem technischem Zug auf der selbst gebauten braunen L-Strecke, gesperrten Eingaben und noch fehlendem Result. Die echte spätere Result-/Neustartfolge ist durch den Passed-Szenentest gebunden. PNG-Signatur, alle Chunk-CRCs, vollständige IDAT-Dekompression, 960×840/RGBA8 und Originalhashes selbst geprüft. Historische unverwechselbare Bytekopien:

- [4×4](NATIVE_INITIAL_wp038-4x4-editing.png), SHA256 `b73666e0c53a886d8faadd32b8015db6ae68444f69b39944d66e804cfaf139b3`.
- [10×10](NATIVE_INITIAL_wp038-10x10-editing.png), SHA256 `78ece007e81f0e3348eaa982ff06ae0a89abf828096cdf01ce2d5df5e6ae3eff`.
- [Fahrt](NATIVE_INITIAL_wp038-4x4-ride.png), SHA256 `6b31dd73a5a633fd4059e550870ea044d6336e22b7ba1a1a38eb123923fa40df`.

## NATIVE-01 – konkreter Scope7-/AK08-Befund

Der gelesene ursprüngliche DevelopmentPuzzleBuild.cs deklariert feste Panel-GUID b281a1bc613c4544b3dc4c9e6a038001 und Material-GUID b281a1bc613c4544b3dc4c9e6a038002. Die tatsächlich erzeugten Meta-/Szenenreferenzen enthalten stattdessen Panel `1c770fd0a7c6816418bd086575289c19` und Material `efea9527556a4684dbdc75a90184d5bd`. Die Szenen-GUID …8003 passt. Die ursprüngliche Prüfung ruft WriteMeta für Panel/Material nur vor CreateAsset im fehlenden-Asset-Zweig auf und prüft diese Identitäten bei vorhandenen Assets nicht. Zwei erneute EnsureArtifacts-Läufe mit bereits vorhandenen abweichenden Assets können deshalb Passed sein, ohne den ausdrücklich festen GUID-Vertrag zu bestätigen.

Erforderlich ist die konsistente feste Identität nach tatsächlicher Unityerzeugung und auch bei vorhandenen Assets, tatsächliche Neuherstellung/Reimport-/Reproduktionsprüfung sowie betroffene native Belege. Die Koordination hat den Befund bestätigt und Korrektur innerhalb desselben verankerten Scopes veranlasst. Kein neuer Solver-/Textgesamtprüfauftrag; kein Absenken des Gates. Dieser Bericht hält den ursprünglichen FAIL unverändert als Historie fest. Eine Korrektur darf ihn nur durch nachfolgenden konkreten Nachweis schließen, nicht rückwirkend entfernen.

## Schutz- und Abschlussgrenzen

Der initiale Editor-Autosettingspatch ist separat bytegebunden und enthält zwei Inhaltsdiffheader; die Koordination dokumentiert drei berührte geschützte Settingspfade. Ein eigener git diff über ProjectSettings war nach der initialen Wiederherstellung leer. Daraus wird keine Ursache für unterschiedliche Pfadzählungen behauptet; temporäre spätere Editorumschreibungen sind am späteren Freeze gesondert auf vollständige Wiederherstellung zu prüfen. Angenommene Textquelle, Domain/Solver und alte Manifestverträge bleiben außerhalb dieses Teilreviews.

[Eigene maschinenlesbare Belege](NATIVE_INITIAL_EVIDENCE.json), SHA256 `e4ad4202971a4f680f54dd9a90144b52d6500c5676aa61671eefc51c1fd89c04`. Kein Editorstart, keine Implementierung und keine CI-Mutation durch die Prüfrolle. Nach konkreter Korrektur folgt nur die betroffene Nachprüfung; unabhängige Gesamt-QC erst auf tatsächlichem Freeze mit seinen Original-CI-/Buildnachweisen. Die drei ursprünglichen Bilder bleiben auch bei neuen Bildern dauerhaft eigenständig gebunden.
