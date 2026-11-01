# Lokale Unity-Vorbereitung

Vor Unitytests unter Windows mit der verwendeten Pythonlaufzeit ausführen:

```powershell
python tools/ci/restore_golden_checkout.py
```

Git kann die historische `GOLDENS_PRECODEC_V7.json` beim Checkout nach CRLF konvertieren. Der unveränderte Bytevergleich verlangt ihre ursprünglichen LF-Bytes. Das Werkzeug prüft den festen SHA256 des aktuellen Gitblobs, akzeptiert ausschließlich dessen reine CRLF-Checkoutkonvertierung und bewahrt die vorhandenen Bytes unverändert unter `.utmp/checkout-originals/`, bevor es die Gitbytes wiederherstellt. Ein zweiter Aufruf prüft nur. Andere Inhaltsänderungen oder Reparsepfade werden abgewiesen; Gitkonfiguration, Historie und Testaussagen bleiben unverändert.

Für CI-Coverage prüft `test_verify_coverage.py` den strikten Reader; `verify_coverage.py TestResults/Coverage --report TestResults/CoverageVerification.json` liest die ursprünglichen XMLs und schreibt einen neuen abgeleiteten Bericht. Die finale Full-Messung ist der Nenner. Fehlende oder widersprüchliche Daten scheitern, auch ohne Mindest-Coverage.
