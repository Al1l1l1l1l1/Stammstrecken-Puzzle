# Regeln für große Work Packages

**Ab MANUS-2026-10-20-02 verbindlich:** Ein Work Package ist ein vollständiger großer Lieferauftrag mit einem sichtbaren Ergebnis im Spiel. Vorbereitung, Implementierung, eigene Fehlerkorrektur, notwendige Nachbesserung, angemessene unabhängige Prüfung und zulässige Integration gehören in denselben Auftrag. Die alte Forderung nach genau einer kleinen einzeln testbaren Änderung pro WP ist aufgehoben.

## Mindestinhalt vor Beginn

Das WP enthält die bestehenden verständlichen Abschnitte ID, Ziel, Voraussetzungen, Scope, Betroffene Dateien/Module, Ausdrücklich nicht erlaubte Änderungen, Akzeptanzkriterien, Tests, Risikoklasse und Definition of Done. Ein kurzer Bearbeitungsstand nennt Verantwortung, reale Basis/Head, nächstes Ergebnis und Blocker. Es soll direkt ausführbar sein, nicht alle internen Schritte als neue Erlaubnisanträge ausformulieren.

Das Ziel benennt einen tatsächlichen Produktzustand: beispielsweise vollständige mobil bedienbare Kampagne oder getestete Store-/Monetarisierungsintegration. Der Scope umfasst alle dafür sachlich notwendigen benannten Module, Daten, Assets, Tests, Korrekturen und Integrationsarbeiten. Segmentierte passende Verzeichnisglobs sind für große Lieferbahnen zulässig; globale Sammel-Allowlists und sachfremde Refactorings nicht.

## Ein Programmanker statt Ankerketten für Routinearbeit

Je unabhängiger großer Lieferbahn entstehen WP und eigenes Manifest einmal vor Fachänderungen im gemeinsamen Erst-Add-Commit nach ADR-030. Historische IDs und Manifeste bleiben unverändert. Unteraufgaben innerhalb dieses Scopes brauchen nur einen kurzen internen Brief: Eigentümer, Ziel, Schreibbereich, Eingänge, Prüf- und Übergabeergebnis. Kein neues WP/Manifest für jeden Subagenten, Routinefix, Testbericht, Closeout oder vorher erfassten Integrationsschritt.

Der Lead darf denselben gültigen Programmscope über alle Korrekturen, Fortsetzungen und sachlich dazugehörigen Unterintegrationen verwenden. Unabhängige parallele Lieferbahnen bekommen je einen passenden Scope; ihre Montage und der RC-Abschluss sind bereits im Gesamtmandat und einem passenden Integrationsscope enthalten. Die Ausführung darf technische Reihenfolge, Teilung und interne Fortsetzung selbst wählen, ohne neue Ownermeldung.

Echte Scopeerweiterungen außerhalb benannter Liefermodule, Änderungen am Produktversprechen oder neue sicherheits-/finanz-/datenschutzrelevante externe Aktionen bleiben separate ausdrückliche Entscheidungen. Keine nachträgliche Scope-/AKabsenkung zum Bestehen eines fehlenden Ergebnisses.

## Entscheidungen innerhalb des Auftrags

Die verantwortliche Rolle trifft normale technische Implementierungs-, UIresponsive-, Import-, Test- und Integrationsentscheidungen im bestehenden Stack selbst. Nur echte Vertrags-/Sicherheits-/Modulgrenzenänderungen benötigen einen Fach-ADR samt erforderlicher getrennter Review. Ein vorher schon entschiedener Grundsatz oder eine Routinekorrektur erzeugt keine neue Entscheiderkette.

Unsicherheit wird nach Wirkung behandelt: Ein implementierbares Detail wird mit nachvollziehbarer innerhalb des Mandats zulässiger Wahl umgesetzt; ein echter ungelöster Produkt-/Rechts-/Befugniskonflikt blockiert nur den betroffenen Teil. Unabhängige Arbeit geht weiter. Der Owner ist nicht technischer Bote.

## Risikogerechte Abschlussbedingungen

Niedrig: reine Dokumentation, Navigation oder technisch identische Schlussdateien – Struktur, Links, Scope und tatsächliche Quellgleichheit; kein Unity-/Geräte-/Korpus-Neulauf.

Mittel: neue UI/Assets/Features – relevante automatisierte Tests, reale Ausführung und Sichtung der betroffenen Spielabläufe, passende technische Integration; vorhandene unveränderte Risiko-Nachweise wiederverwenden.

Hoch: Puzzle-/Hash-/Proof-/Persistenz-/Zahlungs-/Consent-/Releaseänderung – betroffene positiven/negativen Integritätsprüfungen und echte neue Quellen-/Build-/Geräte-/Providerbelege. Eine unabhängige gezielte Fachprüfung der veränderten Risiken, keine pauschale Wiederprüfung aller unberührten Bereiche.

Der nächste Meilenstein ist erst erfüllt, wenn sein Ergebnis im tatsächlich laufenden Spiel überzeugt und die passenden Gates bestanden sind. Bei konkreten Reviewbefunden korrigiert dieselbe verantwortliche Rolle innerhalb desselben Auftrags und weist nur die betroffenen Nachbesserungen neu nach. Ein methodisches Problem kann zusätzliche Prüfung rechtfertigen, aber nicht jede Formatkorrektur eine zweite vollständige Abnahme.

## Dokumentation und Übergabe

Der Lead aktualisiert den WP-Status, den kurzen CURRENT_STATE und die geänderten Fachverträge. Eine Hauptübergabe mit echtem Head/Tree, Ergebnis, vorhandenen Prüfungen, Installationsweg, offenen Risiken und nächster Phase ist ausreichend. Originale als direkte Repository-/CIverweise beifügen, nicht denselben Befund mehrfach in separaten Annahme-/Scope-/Closeout-/Koordinatorberichten wiederholen.

Vorlagen und historische WPs bleiben lesbare Herkunftsquellen. Ihre alten Schrittgrenzen oder Vorbehalte sind keine neuen Blocker gegen einen späteren ausdrücklich breiteren Gesamtauftrag. Das jeweilige aktuelle Mandat und diese Regeln entscheiden.
