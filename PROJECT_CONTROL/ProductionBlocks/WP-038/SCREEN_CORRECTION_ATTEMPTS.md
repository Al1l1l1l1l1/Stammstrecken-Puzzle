# Erhaltene Screen-Nachweisversuche

**Keiner dieser beiden Versuche ist ein angenommener Screenkorrekturnachweis.** Die eigene Installerkorrektur war in beiden Versuchen identisch; die damaligen beiden Sourcebytes, tatsächlichen Original-XMLs und deklarierten vollständigen Logprofile sind im [Evidenzobjekt](SCREEN_CORRECTION_ATTEMPTS.json) gesichert: 77557Bytes, SHA256`8e4fd3266cb7ad4e6d8d4add2a7a2632264e89b49fb923a1e85d67e5b57856e5`. Keine alte Evidenzdatei wird überschrieben.

1. D3D11 mit -batchmode: tatsächlicher Exit2,8/9 Passed,ein FAIL. Die neue Probe beobachtete innerhalb8s keinen abgeschlossenen Kontext mit der Screenkamera. Daraus folgt keine behauptete beobachtete schwarze Bildwirkung und keine allgemeine Aussage, sämtliche Batch-GameViews würden nie rendern.
2. Derselbe unveränderte Test ohne -batchmode: tatsächlicher Exit0,9/9 Passed. Drei echte ScreenCapture-Aufnahmen bei unverändertem Paneltarget=null liegen in586×543 vor (tatsächliche Editor-GameViewgröße; CLI-screen-width/-height erzwingen diese Größe dort nicht). Root betrachtete alle sechs Bilder und verwarf die drei Screenbilder wegen vertikaler Spiegelung. Die automatischen damaligen Pixelgates deckten die Orientierung nicht ausreichend ab. Damit trotz9/9 ausdrücklich **grafischer Nachweis FAIL**, keine Produkt-Screeninversion behauptet.

- [SCREEN_ATTEMPT2_wp038-screen-editing.png](SCREEN_ATTEMPT2_wp038-screen-editing.png), 42007Bytes, SHA256`7cdd884e61387ce8c5f2fc459508ec6fb65bd672048ca3530f25fd350c412ecc`.
- [SCREEN_ATTEMPT2_wp038-screen-ride.png](SCREEN_ATTEMPT2_wp038-screen-ride.png), 40777Bytes, SHA256`8048ea1b79b61532e6daa77b55f0b3ad3243cd6635ab11ab14654aa235640507`.
- [SCREEN_ATTEMPT2_wp038-screen-results.png](SCREEN_ATTEMPT2_wp038-screen-results.png), 46998Bytes, SHA256`6fdd3d128b4e50964cc05038ffadf7884c6391a015d6de9f7b2f747885f59b45`.

Nächster gezielter Schritt: offizielles ScreenCaptureAsTexture nach abgeschlossenem normalen Frame statt eigener RT-/ReadPixels-Abbildung; vorhandene Pixelgates beibehalten und asymmetrische Editingmarker zur Orientierungsprüfung ergänzen. Keine manuelle Bildspiegelung, kein PanelRT- oder Camera.Render-Ersatz. Erst ein neuer tatsächlicher Lauf, Bildbetrachtung und getrennte technische Nachprüfung können den Befund schließen.
