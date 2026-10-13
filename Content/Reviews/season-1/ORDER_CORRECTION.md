# WP-026: Auftragskorrektur und erneute Quellenprüfung

Auftraggeberkorrektur vom 13.10.2026, fortgesetzt auf derselben Branch und im
bestehenden Draft-PR #21 ab `43f737e5a2581e01eb79583d08a9551c4bec95ca`.
Kein neuer Auftrag, Anker, Scope, Manifest, Architekturentscheid oder Parallelagent.

## Korrigierte Zuschreibung

Das pauschale Verbot von Platzhaltern/Dummy-Keys/vorläufigen technischen
Referenzen in WP-026 Voraussetzungen 5 und den bisherigen Berichten war ein
Koordinatorzusatz. Der Auftraggeber hat ausdrücklich klargestellt, dass dies
keine Nutzervorgabe ist. Seine Übernahme ins WP und die Übernahme derselben
Prämisse durch den damaligen Gegenprüfer begründen keine unabhängige Autorität.
Die historischen Texte und der Anker bleiben als Historie nachvollziehbar;
ihre gegenteilige Nutzerzuschreibung ist zurückgenommen. Die Korrektur erteilt
weder ein allgemeines Platzhalterverbot noch eine allgemeine Platzhalterfreigabe.

## Quellenhierarchie und enger zulässiger Weg

| Maßgebliche Quelle | Konkrete Aussage und Anwendung |
|---|---|
| AGENTS.md, Quellenhierarchie | Nutzerkorrektur und angenommene ADRs gehen einer Koordinatorinterpretation im WP vor. Scope-/Manifest-/Architekturgrenzen bleiben bindend. |
| ADR-021, Entscheidung | Neue Quellen bleiben Level-v2-JSON mit Lösung und Produktionsmetadaten. Texte und Completionmetadaten sind nicht Teil des öffentlichen Puzzlehashes. Keine zweite kanonische Logikquelle nötig. |
| ADR-016, Entscheidung | Completionkatalog ist Autorität über typisierte Ergebnis-/Kartenreferenzen. Eine technische Entwurfskennung behauptet keine bestätigte Zieldefinition. |
| Content-Bible 14, §§2,4–9,12 | 240 konkrete Instanzen nach den 20 bestehenden Lernbögen; finale Textbibliothek/Kartenillustration sind eigene Produktionsaufgaben. Kreative Arbeit wird hier nicht vorgezogen. |
| CONTENT_CATALOGS, §§3,5 | DRAFT ist für Authoring/Review zulässig, nicht für Productionimport. Keine generische Production-Fallback-ID darf eine fehlende Referenz verdecken. Deshalb individuelle, explizit offene Entwurfsreferenzen mit fehlenden Zielen im Inventar; kein Fallback oder Fixtureziel. |
| CONTENT_PIPELINE, §§3.2,5 | Vor Commit sind die Prüfungen auszuführen. Das technische Proofgate belegt ausdrücklich nur Regeneration/Bindung; Cross-References, Produktfreigabe und Release-Lock bleiben spätere Voraussetzungen. Fehlende Referenzprüfung bleibt offen/fehlgeschlagen, wird nicht aus einem Proof-PASS abgeleitet. |
| CONTENT_PIPELINE, §8 | Level speichern Schlüssel, keine fertigen Nutzertexte; stabiler Namensraum wird beispielhaft als `level.s1.01.01.01.title` festgelegt. Interne sachliche Qualitätsnotizen dürfen Freitext sein. |
| LEVEL_DATA_FORMAT, §§4,8,11 und Level-v2-Schema | Referenzfelder müssen belegt sein; vollständige Auflösung bleibt Pflicht. Ein syntaktisch gültiger Schlüssel allein belegt sie nicht. Der gelieferte technische Pipelineweg bleibt ohne spätere Gates nicht importierbar. |

Der ausführbare Weg ist daher ein **prüfbarer Authoringbestand** im vorhandenen
Level-v2-Format: neue individuelle technische Keys nach dem vorhandenen
Namensraum; ausdrücklich als `draft-…` gekennzeichnete individuelle Map-/Train-
Referenzen; keine Zielkataloge, Lokalisierungstexte, Assets oder Freigaben.
Das Referenzinventar benennt für jede ID die sechs fehlenden Ziele und die
zuständigen Folgearbeiten. Es ist Reviewdokumentation, kein Ersatzkatalog.
Die Kennungen sind keine kreative Ausarbeitung und keine freigegebene endgültige
Zuordnung. Bei späterer Zielzuordnung bleiben öffentlicher Puzzlehash und Proof
unverändert; der vollständige Dokumenthash ändert sich gemäß ADR-021.

Alle vorgeschriebenen Prüfungen bleiben erhalten. Referenzauflösung,
Präsentations-/Import-/Releaseprüfungen können mangels Zielquellen oder wegen
des beauftragten Import-/Kreativausschlusses nicht bestanden werden; dadurch
bleiben Contentabschluss/DoD und Freigabe blockiert. Der logische Bestand kann
aber durch den bestehenden Produktionsgenerator tatsächlich nachgewiesen werden.
`timeClass = UNCALIBRATED` kennzeichnet den noch unbestimmten Authoringwert;
es setzt keine Sollsekunden oder bestätigte Zeitklasse. Finale Zeiten bleiben null.

## Tatsächliche widerlegende Gegenprobe

Der bestehende isolierte Zusatzharness wurde um ausgefüllte, bewusst nicht
auflösbare Scratchreferenzen erweitert. Produktionscode, Schema und Fixtures
bleiben unverändert. Ergebnis: Generator erfolgreich, Solver aufgerufen,
kanonische Proofbytes identisch zur Kontrollfixture. Strict-`ValidateOne` besteht
das technische Proofgate; Runtimeimport und Release-Lock bleiben false.
Die 18 bisherigen fehlend/null/leer-Varianten werden weiterhin abgewiesen.

Damit widerlegt die neue Probe den bisherigen Schluss, fehlende Referenzziele
würden bereits den Generatorzugang verhindern. Die 18 alten Negativtests
belegten ausschließlich Strukturpflichten. Sie beantworteten die Zulässigkeit
ausgefüllter technischer Entwurfsreferenzen nicht. SDK 10.0.401; separater
net10.0-Harness, kein Unityersatz. `python3.11 tools/season1-authoring/reference_gate_probe.py`.
Tatsächliche neue Ausgabe in `reference-gate-probe.txt`.

## Blockerstatus und Übergabegrenze

BLOCKER-WP026-001 ist als pauschaler Erstellungs-/Proofstartblocker **widerlegt**.
Die fehlenden sechs Referenzziele je Level bleiben konkret offen und sperren
die Cross-Reference-/Contentabschluss-/Importfreigabe. Kein früherer
Gegenbericht wird in eine neue unabhängige Contentabnahme umgedeutet.
240er-Bestand, fachliche Einzel-/Routenreviews und tatsächliche technische
Nachweise werden im bestehenden WP fortgeführt. Unabhängige Abschlussabnahme
und menschlicher Spieltest bleiben echte externe Übergabepunkte.
