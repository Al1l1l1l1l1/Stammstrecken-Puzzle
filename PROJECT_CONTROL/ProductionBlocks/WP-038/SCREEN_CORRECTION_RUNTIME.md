# WP038 – tatsächliche Screenkorrektur

**Eigene gezielte Laufzeit- und Bildprüfung PASS; unabhängige korrigierte Abschlussprüfung und neue reguläre Codefreeze-CI bleiben gesonderte Gates.**

Der konkrete unabhängige HIGH-Befund SCREEN-01 des ursprünglichen Freeze bleibt in SCREEN_OUTPUT_QC und INDEPENDENT_FINAL_QC unverändert erhalten. Eine eigene Camera mit targetTexture=null, CullingMask0 und höherer Depth stellt die normale Bildschirmausgabe auch während der getrennten World-Offscreenfahrt her. Kein globaler Pipelinewechsel und keine Umleitung des UI-Panels.

Unity6000.3.23f1 lief nach Ende des historischen CI-Laufs und vier tatsächlichen CI-Seatdeaktivierungen als einzige lokale Editorinstanz mit D3D11/AMD Radeon RX9070XT, ausdrücklich ohne -nographics. Tatsächlicher Exit0; Original-XML: **9/9 Passed,0 Failed,0 Skipped,0 Inconclusive**, Dauer8,9057482s. Die bisherigen acht Fälle sind erhalten; der neunte prüft normale Screenframes bei PanelSettings.targetTexture=null für Bearbeitung, Fahrt und Ergebnis plus Neustart und Ablehnung eines alten Fahrtcallbacks. Er prüft sichtbare Textpixel, Board-/Bediengrenzen und tatsächliche Zugpixel innerhalb des Bildschirmboards. Die Kopie des fertigen Screenbuffers ist nur ein Captureziel, niemals Panel- oder Cameratarget.

Root betrachtete alle sechs aktuellen PNGs selbst: drei getrennte Panel-/Worldproben und drei normale Screenbilder.

- [SCREEN_CORRECTION_wp038-4x4-editing.png](SCREEN_CORRECTION_wp038-4x4-editing.png): 960×840, 78574Bytes, SHA256`b73666e0c53a886d8faadd32b8015db6ae68444f69b39944d66e804cfaf139b3`.
- [SCREEN_CORRECTION_wp038-10x10-editing.png](SCREEN_CORRECTION_wp038-10x10-editing.png): 960×840, 83416Bytes, SHA256`78ece007e81f0e3348eaa982ff06ae0a89abf828096cdf01ce2d5df5e6ae3eff`.
- [SCREEN_CORRECTION_wp038-4x4-ride.png](SCREEN_CORRECTION_wp038-4x4-ride.png): 960×840, 75865Bytes, SHA256`6a538c949d0d7473faf5dfe5c28d041709cdf5aa9104f0c7b8cbf8d505c06aea`.
- [SCREEN_CORRECTION_wp038-screen-editing.png](SCREEN_CORRECTION_wp038-screen-editing.png): 586×543, 40068Bytes, SHA256`850143f55fc8a1fd1a8df53e0f9e2c68d3d047ab8c8a229314ebc27533b6d444`.
- [SCREEN_CORRECTION_wp038-screen-ride.png](SCREEN_CORRECTION_wp038-screen-ride.png): 586×543, 39149Bytes, SHA256`b39ff1dec5f98b2a9b4fa6af64dc24c37df7f97c092971d3861bb401978e8dd1`.
- [SCREEN_CORRECTION_wp038-screen-results.png](SCREEN_CORRECTION_wp038-screen-results.png): 586×543, 44882Bytes, SHA256`bf4cb72ac19f5d0392715640b2af5ef4ce8acfac74d78e4b05ba1fbdd335a3e5`.

[Originale und Sourcebindungen](SCREEN_CORRECTION_RUNTIME_EVIDENCE.json): 58758Bytes, SHA256`e0931c54dbc4c6495ede6bed4a1bb1f7096e1e531b1feafd529cca308ac7ebbb`. Original-XML und deklarierte vollständige Logprofile sind gzip/base64 gebunden und dekomprimiert byte-/hashgeprüft. Sensible Konto-/Lizenzzeilen sind nur im persistierten Logprofil explizit ersetzt; dessen Profil wird nicht als Rohlog bezeichnet. Die lokalen ignorierten Originalbytes bleiben über Digest/Bytezahl für die unabhängige Rolle verfügbar. Alle41 erlaubten Assetpfade sind mit Roh- und kanonischem LF-Digest gebunden. Geschützte automatische Editor-Settingsänderungen sind gelesen, gesichert und exakt zurückgesetzt.

Dies belegt die eigene technische QA-Schleife, keine Produktionskampagne, menschlichen Rätseltests, Save-/Reward-/Fortschrittsfunktion, physische Geräte- oder Releasefreigabe. Frühere native Evidenz und der konkrete ursprüngliche FAIL werden nicht überschrieben.

## Tatsächlich gewählte Screenroute und erhaltene Fehlversuche

Die endgültige Screenprobe lief ausdrücklich **ohne -batchmode**, mit offizieller `ScreenCapture.CaptureScreenshotAsTexture(1)` nach `WaitForEndOfFrame`. Die gleiche fertig gerenderte Framezahl musste zuvor die eigene Screenkamera enthalten. Es gibt kein eigenes RT-/ReadPixels-Y-Mapping, kein manuelles Spiegeln und keinen Panel-/Camera.Render-Ersatz. Die drei normalen Bilder haben die tatsächlich erfasste Editor-GameViewgröße586×543; die getrennten Panelbilder960×840. CLI-screen-width/-height erzwingen im Editor keine andere GameViewgröße.

Die verstärkten asymmetrischen Pixelgates verlangen rotes X genau in(1,1), nicht im grauen(1,2), gelbe Auswahl in(2,2) und den QA-Grenzhintergrund oben. Root betrachtete alle sechs aktuellen Original-PNGs: korrekte Orientierung, vollständige Bedienbereiche, sichtbarer Train während Fahrt und Result erst danach. Die beiden vorherigen tatsächlichen Versuche bleiben in [SCREEN_CORRECTION_ATTEMPTS.md](SCREEN_CORRECTION_ATTEMPTS.md)/[Originalbindungen](SCREEN_CORRECTION_ATTEMPTS.json) unverändert erhalten: Batch-Timeout8/9 sowie verworfene vertikal gespiegelte Capture-PNGs trotz automatischem9/9. Daraus wird keine beobachtete Produktions-Screeninversion abgeleitet.

Der normale Editor schrieb zusätzlich GraphicsSettings-Lightflags und QualitySettings-MSAA automatisch um. Der vollständig gelesene Diff ist exakt zum zuvor gesicherten automatischen Diff bytegleich; alle fünf betroffenen geschützten Dateien wurden wiederhergestellt. Die ebenfalls automatisch neue PackageManagerSettings-Datei wurde gesichert und entfernt. Der Diff und diese neuen Originalbytes sind im Evidenzobjekt gebunden; kein solcher Settingsdelta wird geliefert.
