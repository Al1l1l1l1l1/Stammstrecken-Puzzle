# Verbindliche Agentenleitlinie

**Geltung ab MANUS-2026-10-20-02:** Die ausdrückliche Kurskorrektur des Owners ersetzt die bisherige kleinschrittige Produktionsführung. Maßstab ist ein gutes, tatsächlich laufendes und veröffentlichungsfähiges Spiel, nicht die Zahl von Dokumenten, Work Packages oder grünen technischen Teilprüfungen. Die bisherige Version bleibt in Git am Stand `8d2ff1950fa72777014eda0555b02b6e9fd9a995` nachvollziehbar.

## Kurzer verbindlicher Einstieg

1. Diese Datei und `PROJECT_CONTROL/CURRENT_STATE.md` lesen; tatsächlichen Branch/Head und aktive Aufgabe feststellen.
2. Das dort verlinkte aktuelle Gesamtmandat und das eigene große Work Package beziehungsweise den delegierten Teilbrief lesen.
3. Nur die für die eigene Arbeit einschlägigen Fachverträge und aktuellen Änderungen lesen. Konzeptindex/Master stehen zur Navigation bereit; eine komplette Wiederlektüre aller Konzept-, Historien- oder ADR-Dateien bei jedem Routineabschnitt ist nicht erforderlich.

Bei Fortsetzung desselben Auftrags den aktuellen Arbeitsstand und seitdem geänderte Anforderungen laden, nicht die ganze Einstiegskette wiederholen. Fehlende technische Kleinentscheidungen innerhalb des übertragenen Mandats trifft die verantwortliche technische Rolle; ein echter Konflikt mit Produktversprechen, Datenintegrität oder externer Befugnis wird nur im betroffenen Teil eskaliert.

## Verantwortung und Auftragsgröße

Ein Gesamtauftrag liefert einen großen, zusammenhängenden Teil des Produkts einschließlich Vorbereitung, Umsetzung, eigener Korrekturen, nötiger unabhängiger Prüfung, Nachbesserung und Integration. Die Koordination übergibt diesen Auftrag einmal; Routinefortsetzungen, Defektbehebung, Subagentenbeauftragung und zulässige Integration erfordern keine neuen Ownerprompts.

Der benannte Releaselead ist für die Zusammenführung, spielbare Meilensteine und das vollständige Endergebnis verantwortlich. Parallel arbeitende Rollen besitzen eindeutige Dateieigentümerschaft und kurze interne Teilbriefe. Ein neuer Subagent, ein Fix, eine Union oder ein Dokumentationsschluss erzeugt allein kein neues Work Package, kein neues Scope-Manifest und keine zusätzliche Geschäftsführerfreigabe.

Ein eigener unveränderlicher Scopeanker ist je tatsächlich unabhängiger großer Lieferbahn vor Fachänderungen ausreichend. Sein Scope benennt die zusammengehörigen Module und realen Ergebnis-/Risiko-/Integrationsgrenzen. Der historische ADR-030-Vertrag bleibt erhalten; er verlangt nicht, jeden internen Kleinschritt als neues Programm zu verankern. Erweiterungen außerhalb des Programms bleiben ausdrückliche Entscheidungen.

## Produkt- und technische Entscheidungsgrenzen

Neue direkte Ownerentscheidungen und ausdrücklich zuständige Produktentscheidungen haben Vorrang. Aktuelle Fachfassungen müssen ihnen entsprechen. Keine stille Featurekürzung, erfundene menschliche Abnahme, falsche Befugnisherkunft oder nachträgliche Umdeutung alter Nachweise.

Implementierungsdetails im bestehenden Stack, normale Daten-/UIcomposition, Importpresets, Tests, Fehlerkorrektur und geplante interne Integration darf der Lead innerhalb seines Mandats selbst entscheiden. Ein Fach-ADR ist nur bei einer echten Änderung eines öffentlichen Daten-/Persistenz-/Hash-/Sicherheits-/Providervertrags oder einer tragenden Modul-/Stackgrenze erforderlich, nicht für jede Klasse, Ansicht, Presetdatei oder Montage. Solche echten Vertragsänderungen werden vor Abhängigmachen in einer kurzen Entscheidung mit nötiger getrennten Fachreview festgehalten.

Main-/Produkt-/Releasebefugnisse gelten nur im ausdrücklichen aktuellen Gesamtmandat. Eine technische Integration ist nicht gleich Storeveröffentlichung. Externe Konten, rechtliche Erklärungen, Zahlungen und Veröffentlichung brauchen ihre jeweils tatsächlich autorisierte Handlung; keine Annahme aus einem allgemeinen Agentenauftrag.

## Qualitätsmaßstab

Die laufende App muss gut aussehen, stabil reagieren und ihren vollständigen Spielablauf liefern. Referenzbilder, Build-PASS, Scope-/Proof-PASS und Dokumentation ersetzen die tatsächliche visuelle, bedienungsbezogene und gerätebezogene Prüfung nicht. Automatisierte Kontrollen sichern konkrete Risiken; unveränderte Quellen werden nicht wegen neuer Schlussdokumentation neu kompiliert oder vollständig erneut beurteilt.

Die risikogerechte Definition of Done und CIsteuerung stehen in `PROJECT_CONTROL/DEFINITION_OF_DONE.md` und `ARCHITECTURE/TEST_STRATEGY.md`. Eine unabhängige ergebnisbezogene Annahme je integriertem Produktmeilenstein reicht; ein konkreter Nachbefund wird gezielt geschlossen, nicht über eine neue globale Abnahmekette.

## Kurze aktuelle Dokumentation

`CURRENT_STATE.md` ist ein kurzer aktueller Steuerungsstand, kein wachsendes Historienbuch. Der Lead pflegt eine vollständige Hauptübergabe des Gesamtauftrags und verlinkt erforderliche Originalbelege. Änderungen an Produktregeln stehen in den maßgeblichen Fachdateien. Git und datierte Quellbindungen bewahren Historie; identische Ergebnisse nicht in mehreren Zusatzberichten erneut erzählen.

Der Owner erhält pro Liefermeilenstein höchstens eine vollständige übertragbare Koordinatordatei und eine kurze konkrete Chatmeldung. Keine Pflicht, einzelne technische Belege manuell zwischen Agenten zu transportieren. Die Koordination und Ausführung lesen sie direkt aus dem Repository.
