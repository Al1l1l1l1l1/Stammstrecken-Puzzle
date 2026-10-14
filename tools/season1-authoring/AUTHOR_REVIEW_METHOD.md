# WP-026: individuelle Autorbewertung und rein technischer Transport

Der unabhängige Bericht zu `3c9b7d0…` bleibt unverändert im Reviewscope.
Die Nachbesserung bewertet tatsächlich alle 240 öffentlichen Inputs,
Lösungsgitter, Counts, Ports und Rootfolgen sowie die 20 Gruppen-/Routenbögen.
`author-decisions.txt` enthält die wörtlich einzeln verfassten Entscheidungen,
Einstiege, Fokuswerte, Routenurteile und vorläufigen gedanklichen Kettenschätzungen.
Sie entstanden durch Einzelbetrachtung, nicht durch Textvorlagen oder
algorithmische Qualitätszuweisung. Kein Human-Spieltest oder unabhängiger PASS.

`author-review.json` bindet jedes Urteil an Public-Puzzle- und Lösungshash.
`apply_author_review.py` übernimmt ausschließlich diese expliziten Werte in die
kanonischen Quellen; bei abweichendem gebundenem Proofinput stoppt es.
`review_inventory.py` prüft zusätzlich die konkrete öffentliche Null-/Volllinie
bzw. Endpointwurzel anhand der tatsächlichen Rootspur und rendert Fakten plus
wörtliche Urteile. Der frühere `--annotate`-Vorlagenmodus ist entfernt.
`select_candidates.py` erzeugt nur Vorschläge mit ausdrücklich unbewerteter
Scratchnotiz, kein fachliches Urteil. Ein Tool bleibt für Kandidaten, Counts,
Hash-/Proofregeneration und getreuen Transport zulässig. CONTENT_PIPELINE §3.2
wird hier eng auf das Erfinden von Urteilen angewandt, ohne allgemeines Skriptverbot
oder Änderung einer Produkt-/Architekturregel.

`production.chainDepth` enthält jetzt die explizite individuelle Autorenschätzung
zusammengefasster Linien-, Restzahl- und Portschlüsse. Sie ist vorläufig und nicht
im Spieltest gemessen; `proof.metrics.maxDeductionDepth` bleibt die unveränderte
ADR-031-Tiefe einzelner Rootreduktionen. Das ist keine Kalibrierung von Spielzeiten.
Fokus ist eine individuelle fachliche Zuordnung, keine aus der Existenz einer
Nullzahl automatisch abgeleitete Eigenschaft.

15 genau benannte Logikänderungen stehen mit altem/neuem Input, echtem Pool-/
Kandidatenbezug und Hash in `targeted-changes.json`; 225 Logikinputs bleiben erhalten.
Nur unveröffentlichte Authoringentwürfe werden geändert (LEVEL_DATA_FORMAT §3;
ADR-021, Veröffentlichungs-/Release-Lock-Kontext). Keine Runtimequelle, Migration,
Release-ID oder veröffentlichte Identität wird verändert.

Die früheren Pools sind bewusst keine zweite kanonische Quelle. Aktuelle
Quellen und Produktionsproofs sind vollständig reproduzierbar, der ursprüngliche
gesamte Auswahlprozess ist anhand der verbliebenen Scratchpools nicht rückwirkend
vollständig reproduzierbar. Fehlende damalige Toolversionen werden nicht erfunden.
Neue Auswahlentscheidungen sind explizit nachgewiesen; ihre Poolhashes und
Kandidatenwerte allein ersetzen keine fachliche Kuratierung.

```text
python3.11 -B tools/season1-authoring/apply_author_review.py
python3.11 -B tools/season1-authoring/run_authoring.py verify
python3.11 -B tools/season1-authoring/review_inventory.py
python3.11 -B tools/season1-authoring/check_schema_hashes.py
```

Der letzte Befehl benötigt die bestehende gepinnte Schemaumgebung und Node 22.x;
die tatsächliche Version wird berichtet. Linux/macOS-Governance wird separat mit
dem unveränderten WP-026-Scopebefehl ausgeführt. Die neu hinzugefügten Toolhilfen
ändern keine Produktionsmodule, Schemas, Validatoren oder Projektabhängigkeiten.
Das Archiv `wp026-review-verifiers.tgz` bleibt ausschließlich in ignoriertem Scratch.
Eine vom Autor ausgeführte Gegenprüfung mit Prüfercode ist keine unabhängige Abnahme.

Die tatsächlichen unabhängigen Nachprüfungen auf 96697ab sind unverändert im
Reviewscope persistiert. Drei dort bestätigte Sachtextfehler wurden individuell
korrigiert, das Route-18-Urteil hält den Wortlautvorbehalt offen. Der Transport
erhöht die Inhaltsrevision nur bei einer tatsächlichen Änderung der expliziten
Qualitäts-/Fokus-/Kettenschätzung und behält sie bei identischer erneuter Ausführung.
Diese Idempotenz verhindert eine Rücksetzung späterer Korrekturen auf Revision 2;
Public-Puzzle-/Lösungsbindung und vollständige Vorprüfung bleiben unverändert.
Neue unabhängige Nachprüfung nach Freeze bleibt erforderlich.
