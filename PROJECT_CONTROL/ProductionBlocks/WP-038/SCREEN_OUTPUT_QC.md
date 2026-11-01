# WP038 – unabhängiger gezielter Screenoutputbefund

**FAIL – SCREEN-01, High; AK04/AK06 am Freeze C nicht vollständig erfüllt, AK10 nicht erteilt.** Gegenstand `e77c5fa168b915979c3c9e39d45fbcfcaefa3aea`, Tree `e5f9235716d15b1ab0a74b46fac1e1cdd1aeddd0`. Vom Implementieren getrennte Rolle `/root/wp037_independent_qc`; keine Implementierungsänderung oder Editor-/CI-Mutation. Eigene gebundene Quellen-/Zeilenbelege in [SCREEN_OUTPUT_QC.json](SCREEN_OUTPUT_QC.json), SHA256 `296d0c986ce43665dd69f995571cc7562af437d8120e4a6a2696ab401b9bedb5`.

## Konkreter technischer Befund

Die eingefrorene QA-Szene und ihr Installer erzeugen vor Fahrtbeginn keine Screenkamera. Während der Fahrt existiert ausschließlich die explizite Worldkamera mit nichtleerer targetTexture. Der echte gepinnte URP17.3.0-Code setzt bei mindestens einer Kamera die Overlayverantwortung auf SRP. UniversalCameraData.resolveToScreen verlangt dagegen targetTexture==null; damit ist rendersOverlayUI für die einzige vorhandene Kamera false. Renderer2D koppelt seinen normalen Overlaypass an genau diese Eigenschaft und isLastBaseCamera; DrawScreenSpaceUIPass umfasst ausdrücklich UIToolkit/uGUI. Bei der isolierten QA-Szene fehlt dadurch während der Worldfahrt der normale Screen-UI-Ausgabepfad.

Das ist eine aus den tatsächlichen eingefrorenen Quellen und den gebundenen Paketquellen hergeleitete Pfadunterbrechung. Die konkrete Pixelwirkung eines normalen Standalonescreens wurde **noch nicht tatsächlich ausgeführt**; es wird kein beobachteter schwarzer Bildschirm behauptet. Die bestehende D3D11-Grafikprobe setzt am Test-Panel targetTexture und umgeht deshalb diesen ungeprüften Screenpfad. Ihre echten8/8-/PNGbelege bleiben für den dortigen Offscreen-Gegenstand gültig, schließen SCREEN-01 aber nicht.

## Erforderliche Korrektur und betroffene Nachprüfung

Eine explizite ausschließlich szenenlokale QA-Screenkamera im Installer, targetTexture=null, cullingMask0 und passende Reihenfolge nach der Worldkamera, liegt innerhalb des bestehenden WP038-Scopes. Sie benötigt eine klare eigene Lebenszeit; globale Pipelineflags, andere Settings oder neue Assemblykanten sind nicht erforderlich. Implementierung/Korrektur bleibt der getrennten Autorenrolle vorbehalten.

Gezielt tatsächlich nachzuweisen: normaler Screenpfad mit unverändert null bleibender PanelSettings.targetTexture, reale UI-Aktionen, Editing/Fahrt/Result/Neustart und aktuelle sichtbare Framebilder mit Nicht-Nullrenderer. Bestehende Builder-/GUID- und UI-Grafikfälle bleiben wirksam; geänderte Quellen und betroffene Laufzeitgrenze werden neu gebunden. Danach neuer konkreter Freeze, vollständige reguläre CI dieses neuen Stands und unabhängiger Abschluss; unveränderte Solver-/Textnachweise werden wiederverwendet. Der laufende ursprüngliche C-Lauf wird unverändert regulär beendet und historisch gesichert, keine Cancellation/Retry und kein konkurrierender Editor. Selbst ein später vollständiger CI-PASS am C heilt diesen konkreten Befund nicht.
