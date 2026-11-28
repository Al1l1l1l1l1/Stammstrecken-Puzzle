# Technische Spielprüfungen
Vom Projektverzeichnis aus mit den Abhängigkeiten aus `requirements.lock.txt` und Node.js ausführen.

    python tools/architecture-validation/validate.py --self-test

Geprüft werden relative Links, JSON-Schemas, Rätsel und Lösungsnachweise, Modulgrenzen, Kataloge und Spielstand-Kompatibilität.
Die Tests decken auch Endlosspiel, Käufe, Datenschutz-Abläufe und Freigabebedingungen ab. Node.js berechnet die Vergleichshashes unabhängig.
