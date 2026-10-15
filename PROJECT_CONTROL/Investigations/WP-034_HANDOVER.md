# WP-034 – O-02 Zuverlässigkeitskorrektur

Status: Implementierung und lokale Nachweise geliefert; finale reguläre CI und unabhängige technische Abnahme stehen noch aus. Keine Selbstfreigabe, kein Gesamt-WP026-PASS, keine Main-Integration.

## Auftrag und Quellenstände

Ausdrücklicher O-02-Auftrag vom 15.10.2026; eigenes prospektives WP034 mit unveränderlichem Manifest. Anker f59d28bfb58331d3e9161708d99eb1834c5ae7e2, einziger Parent/Base a2e26a658a2bcdd47002698c150cb5736d9760da. Branch codex/wp-034-hardbudget-reliability, [Draft-Unter-PR29](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/29) ausschließlich gegen WP026. Main d90e798 und PR21-Lieferzweig bleiben unverändert. Keine andere aktive Projektbearbeitung bei Übernahme sichtbar; einziger lokaler Checkout sauber auf a2e.

## Originalbefund und kausale Reichweite

[Originalartefakte](WP-034_ORIGINALS.json) enthalten verlustfreie Original-XMLbytes, ZIP-/XMLhashes und alle Testzeitpaare. Run38668535139:455/455 PASS, betroffener Test100,400123s; Run38668540223:454/455 PASS, Test213,369626s, ausschließlich180000ms-Gesamtzeitfehler. Beide Ausgaben enthalten alle sechs Fälle, identische Klassen/Größen und maxStates2 für den unterbrochenen Langpfad. Alle Nachlaufwerte erfüllen die40%-Assertion. Zwei andere große32er-Budgetfälle ebenfalls rund doppelt langsam; Median des gemeinsamen >100ms-Korpus nur1,05015. Die historische konkrete CPU-/Scheduling-/GC-Ursache des zeitweisen Faktors ist mangels Telemetrie nicht rekonstruierbar.

Tatsächlicher Pushcheckout a2e; tatsächlicher PRcheckout76c5bab5ca0a8dd12b21368d3a7a9dfd131d0760 mit Eltern d90e798/a2e. Beide haben **identischen vollständigen Git-Tree05042b508afacc9b03e9ab1baf771d82ab33fd9d**. Das ist nach Fetch des tatsächlichen PR-Mergerefs nachgerechnet, nicht aus dem API-head_sha geraten. WP030s alter eigener Originaltimeout bleibt separat historischer FAIL.

Der nachgewiesene vermeidbare Aufwand liegt in RootDeductionTracer.Record: jede versuchsweise Prämissenlöschung und die abschließende Faktenminimierung erzeugten die gesamte Prämissensicht erneut, initialisierten jede Rasterzelle und scannten die gesamte bisherige Spur. Jede Löschung ändert jedoch ausschließlich die Zelle des gelöschten Eintrags. Das kontrollierte Scratchprofil gleicher Quellen/Fälle/Parameter zeigt4.283.337 View-Aufrufe und rund4,49GB inklusive View-Allokationen; der Tracer dominiert die Gesamtarbeit. Keine Behauptung, die historischen Hostumstände damit bewiesen zu haben.

## Korrektur und unveränderte Verträge

Record baut die Sicht einmal auf. Beim Löschversuch rekonstruiert es nur die betroffene Zelle aus genau den noch eingeschlossenen Kandidaten; bei nicht entbehrlicher Prämisse stellt es den vorherigen Zellwert wieder her. Faktenminimierung konsumiert dieselbe fertige Sicht. Kandidatenmenge, aufsteigende Prämissenliste, jüngster-zuerst-Löschung, Faktenlöschung und alle Holds-/Sufficient-Prädikate bleiben unverändert. Eine Zellrekonstruktion berücksichtigt auch mehrere/überlappende Einträge korrekt; einfaches OR-Wiedereinsetzen wird nicht verwendet.

Keine Änderung in PuzzleSolver.cs, Ergebnisbau/Budgetprüfung, Solverversion, Suchfolge, Traceform, Metriken, Domain, Proof-/Strict-/Hash-/Schemacode, Leveln/Proofs oder CI-Ausführung. Kein ADR031-Versionswechsel erforderlich, sofern die unabhängige Prüfung diese vollständige Invarianz bestätigt. Keine Assertion, Grenze, Warm-up-/Sample-/Fallzahl, Zustandsgrenzsuche oder Timeout verändert.

## Tatsächliche Diagnose und Tests

Baseline840b929, unveränderte Produktion: regulärer Unity-Run38724396166, EditMode455/455 bestanden, Test83,194s. Neue Phasenmessung außerhalb aller Nachlauf-/Kopierintervalle: große Fälle13,37–18,40s; CPU annähernd Wallzeit, jeweils13–16Gen0/1/2-Zählungen. Zustandsgrenzsuche1,239s bis maxStates2. Das lokalisiert Rechen-/GC-Arbeit; live_heap_delta ist ausdrücklich kein Allokationszähler, allocated_bytes=UNAVAILABLE. Baseline ist Diagnose, keine Reparatur oder übertragener Gesamt-PASS. Vollständige übrige Baselinejobs werden nach ihrem tatsächlichen Stand geführt.

Lokale uninstrumentierte Gegenprobe (.NET10.0.12/SDK10.0.401, Windows, kein Unityersatz): alle ursprünglichen HardBudget-Assertionen, Fälle,2Warm-ups/25Samples erhalten. Alt5598,093ms/7.619.849.440allozierteBytes; Neu3282,641ms/3.708.884.872Bytes. Die exakten Rohwerte sind laufgebunden, keine CI-Referenzwerte. Mit Scratchprofil: View-Aufrufe4.283.337→231.264, inklusiveViewbytes4.488.253.520→242.172.728; beide Profile verwenden denselben Messhelper, inklusives Timing nicht additiv.

Zwölf vollständige Alt-/Neu-Ergebnissnapshots (vier32er-Fixtures × Zustandslimits1/2/5M) stimmen byteweise in ihrer vollständigen Serialisierung überein: Klasse, Zählung, Pfad, Recoveryprotokoll, vier Metriken und sämtliche Tracefelder/Prämissen. Neuer Unity-fähiger Regressionstest bindet vor Reparatur erfasste vollständige1639er-Spur samt0/1639/30/0-Metriken; VorreparaturtestPASS. Vollständige reine Suite454/454PASS,0Warnungen/Fehler/Skipped. Drei Scratchmutationen erkannt: Ergebnisbau nach letzter Prüfung verletzt die ursprüngliche Wanduhrassertion; fehlende Sichtwiederherstellung und umgekehrte Prämissenlöschung verändern die gebundenen Ausgaben. Kein Buildfehler als Mutationserfolg gezählt.

Isolierte aktuelle Produktion:240×3Regenerationen, Strict-One/Batch/Reverse,3840Metamorphosen,1440Mutationen bestanden. Alle240Level-/Proofbytes exakt kanonisch erhalten; alle20frisch regenerierten Rootdateien objektgleich und im Git-LF-Profil bytegleich. Windows-C#-Prettyprint enthält CRLF; dessen separate Rohhashes werden ausdrücklich erhalten, kein Rohbytegleichheitsfehler verdeckt. [Produktionsnachweis](WP-034_PRODUCTION.json). Schema240/240,Node1200/Root240 wird nach tatsächlichem Ende separat gesichert.

Eigener kanonischer Linux-Scope mit --self-test bestanden(18Gruppen) in noch schmutzigem Arbeitsbaum auf Baseline840b929; kein finaler Freeze-PASS. Python3.11.13/Node22.20.0 und bestehende Lockumgebung; endgültige Architecture-/Scopeprüfungen separat ausführen. [Originale eigene Ausgaben und XMLs](WP-034_LOCAL.json) erhalten explizite Byteprofile. Erster nicht eskalierter .NET-Build startete nicht erfolgreich; alter WP025-Harness scheiterte an Unity-only-Korpustest, danach eigener reiner Harness mit expliziter Ausgrenzung dieses unverändert in echter CI geprüften Tests. Erste Root-Rohbyteassertion scheiterte am Prettyprint-Zeilenprofil; vollständige JSON-/Git-LF-Gegenprüfung und separate Rohhashes korrigieren nur die Nachweisbehauptung.

## AK / DoD / nächste Schritte

AK01 eigener Anker und enger Diff vorhanden, finaler Scope offen. AK02 Originale vollständig untersucht; historische Hostumstände offen. AK03 vermeidbare vollständige Rekonstruktion kontrolliert kausal belegt und beseitigt, echter optimierter CI-Vergleich offen. AK04 ursprüngliche Aussagen erhalten, zusätzliche Bindungs-/Differential-/Mutations-/240er-Nachweise erbracht. AK05 finale echte CI/Artefaktauswertung offen. AK06 unabhängige technische Abschlussrolle auf finalem HEAD erforderlich. AK07 diese Übergabe und aktuelle Steuerung werden auf den tatsächlichen Freeze fortgeschrieben.

Letzter Schritt: belegte mechanische Aufwandskorrektur und lokale vollständige Regression. Nächster Schritt: echten unveränderten regulären CI-Weg am eingefrorenen Korrekturstand vollständig beurteilen, unabhängige technische Prüfung durchführen und eigene Befunde nötigenfalls im Scope korrigieren. Keine neue Nutzerentscheidung für diese bereits beauftragten Arbeiten erforderlich. Integration in WP026 und nach main nicht ausgeführt. WP026-Gesamtfreigabe/Humanreview/1440Referenzziele/UNCALIBRATED und sämtliche kreativen/Import-/Produkt-/Release-/Geräte-/Storegates bleiben getrennt offen.
