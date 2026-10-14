# WP-030: Untersuchung des historischen HardBudget-Timeouts

**Ergebnis: Untersuchung ausgeführt, Originalursache nicht bewiesen; O-02 bleibt offen. Keine Reparatur, Quarantäne oder Budgetänderung.** Eigene Untersuchung, keine unabhängige Freigabe. Quellenstand main d90e798; Originalbefund 96697ab; Probe auf dem WP-030-Anker 438b45e mit unveränderten Test-/Domain-/Solverquellen.

## Originalbefund und gezielte Vergleiche

Original-XML aus der bereits archivierten GitHub-Evidenz dekomprimiert, bytegenau gehasht und geparst. Pushrun 38490243240: XML-SHA256 `8ef8f9bf3baae10555a238d132a79b0289f2173149a5a9939d94c7390fd82bf1`, Testdauer 197,533248 s, NUnit-Fehler 180000-ms-Timeout, 453/454 PASS. Vergleich desselben Heads: PR-Run 38490247420, XML-SHA256 `8c8350bcca006d31f56dfae51ed03f09453c3040b39b2af771ec244a4a06130c`, 100,490521 s, PASS.

Beide Originalausgaben enthalten **alle sechs Ergebnisfälle**, gleiche Klassen, Größen und Budgets. Die unterbrochene Langpfadsuche findet in beiden bei maxStates=2 die erste Lösung; keine bis 10000 ausufernde Grenzsuche. Der fehlgeschlagene Lauf protokolliert auch den letzten UNIQUE-Fall. Seine angegebenen Nachlaufüberschüsse/Resultatkopierzeiten (µs) sind 0,8/5,0; 0,9/4,7; 0,9/5,5; 1,0/5,7; 0,6/5,4. Alle liegen unter der unveränderten 40%-Grenze. Damit ist ein im Output belegter Nachlaufassertionsfehler ausgeschlossen; die abschließende NUnit-Gesamtzeitverletzung bleibt real. Der XML-Output beweist keine zeitliche Telemetrie innerhalb jedes Falls.

Alle 37 gemeinsamen Tests mit PR-Dauer >100 ms verglichen: medianer Push/PR-Faktor 1,0346. Drei große Budgetfälle sind dagegen ähnlich verlangsamt: dieser Test 1,9657; `HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn` 40,101345/20,655385 s (1,9414); `HardBudget_UniqueNeedsAProbeAfterTheWholeTraceBuild_AtEveryInterruptionKind(32,14)` 10,029191/5,046811 s (1,9872). Kein Beleg für eine konstante globale Verdopplung aller Tests. GC-/Speicher-/CPU-/Schedulingbelastung gerade während dieser großen Fälle ist eine plausible, **unbewiesene** Hypothese.

Der reproduzierbare Prüfer vergleicht sämtliche Domain-/Solver-CS-Gitblobs und Originaltest/Oracle zwischen 96697ab und aktuellem HEAD: identisch. Testseedunterschiede erklären keine veränderten Fixtures: dieser Test benutzt feste Konstruktionen, keine seedgesteuerte Zufallsauswahl. Frühere bestandene Runs sind Vergleichsevidenz, keine Ursachenheilung.

## Tatsächliche Diagnoseprobe

`tools/wp030-investigation/run_probe.py` liest den Originaltest und ergänzt ausschließlich eine Scratchkopie um Phasenmessungen außerhalb der Nachlauf-/Kopierintervalle. Alle sechs Fixtures, 2 Warm-ups, 25 Messungen je Fall, Grenzsuche, Limits und Assertionen bleiben erhalten. Unveränderte getrennte Produktionsassemblies, vorhandene NUnit-3.14-DLL; keine neue Projektabhängigkeit. .NET SDK 10.0.401 / Runtime 10.0.12 auf Windows, **kein Unity-/CI-Ersatz**. Build ohne Fehler/Warnungen. Originalassertionen vollständig ausgeführt; Gesamtdauer 6.377,2318 ms.

| Phase | Wall ms | Prozess-CPU ms | allozierte Bytes | Gen0-GCs |
|---|---:|---:|---:|---:|
| tiny baseline | 29,2002 | 31,25 | 988360 | 0 |
| unsatisfiable serpentine | 1387,5715 | 1578,125 | 1534484232 | 31 |
| ambiguous corner | 699,085 | 765,625 | 1252422488 | 25 |
| ambiguous long path | 1129,5302 | 1156,25 | 1616355352 | 33 |
| interrupted long path | 1538,4395 | 1562,5 | 1685978328 | 35 |
| unique serpentine | 1468,1473 | 1500 | 1531348088 | 32 |

Dies lokalisiert die Arbeit auf die großen Ergebnis-/Tracefälle, einschließlich reproduzierbar hoher Allokationslast. Prozess-CPU kann durch GC-Hilfsthreads höher als Wallzeit sein. Die Daten beweisen weder dieselbe Allokationsrate unter Unity Mono/Coverage noch konkrete historische GC-Pausen, Hostkonkurrenz oder einen Solverbug. Auch eine erneute erfolgreiche Unity-CI würde diese kausale Lücke allein nicht schließen.

## Konkreter Folge-/Entscheidungsbedarf

Es ist keine nachgewiesene Produktionsreparatur abzuleiten. O-02 bleibt ein bekannter Zuverlässigkeitsbefund gemäß TEST_STRATEGY §13. Für eine belastbare Ursachenprüfung fehlt die historische zeitaufgelöste Laufzeittelemetrie; sie ist nachträglich nicht rekonstruierbar. Enger vorgeschlagener Folgeauftrag: ausschließlich diagnostische Phasen-/CPU-/GC-/Allokationsausgaben im bestehenden HardBudget-Test bzw. dessen regulärem CI-Aufruf ergänzen, ohne Schleifen, Assertions, Budgets, Timeout oder Runnervertrag zu verändern; bei einem neuen Vorfall Original-Test-XML, Editorlog und tatsächlich verfügbare Container-/Hostlastwerte zeitlich korrelieren. Dies ist eine zur Entscheidung vorgelegte Testdiagnostikänderung, in diesem **lesenden** Untersuchungsauftrag nicht ausgeführt. Eine eventuell aus neuen Daten abgeleitete Solver-/Traceroptimierung erfordert ihren eigenen passenden Scope und ADR-031-Versionsprüfung; kein vorsorglicher solver-v3-Auftrag.

Kein automatischer Retry und keine nachträgliche Flaky-Ausnahme. Menschlicher Spieltest, CI-PERF des Kampagnenkorpus und Fremdcontent-Gates bleiben getrennte Sachverhalte. Die neue CI auf f8e1dfc hat einen separaten Dokument-Whitespacefehler gefunden; dieser ist kein Wiederauftreten oder Erklärung des historischen HardBudget-Timeouts und wird verlustfrei im folgenden Dokumentations-/Integrationsvorbereitungsschritt korrigiert.

## Nachweise und Reproduktion

Eigene vollständige Daten: [WP-030_EVIDENCE.json](WP-030_EVIDENCE.json); tatsächlicher Build-/Probenoutput: [WP-030_PROBE.txt](WP-030_PROBE.txt). Originalarchiv bleibt unverändert in `Content/Reviews/season-1/ci-repair-96697ab-artifact-evidence.json`. Ausführung:

```text
python -X utf8 -B tools/wp030-investigation/run_probe.py --nunit <vorhandene-nunit-3.14-netstandard2.0-DLL>
```

Alle eigenen erzeugten CS-/Builddateien bleiben in ignoriertem `.utmp/wp030-probe`. Kein Lauf des alten GitHub-Runs ausgelöst. AK-02/03/04: tatsächlich bearbeitet, kausale Grenze ausdrücklich offen; eigener Scope/Freeze und neue CI getrennt am PR belegen. Keine technische Gesamtfreigabe und keine Selbstabnahme.
