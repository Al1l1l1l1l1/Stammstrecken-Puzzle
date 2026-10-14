# WP-029: F-01 / O-03 – neue eigene Nachweise

## Aktuelle Nachführung zur Gesamtabnahme – 15.10.2026

Maßgeblicher geprüfter Lieferstand: `b1457ba6a61826fe3b64a83323c27a13e9d23080`; Quellenstand main: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Historische Manifestbasen bleiben WP-026=c9516bd und WP-028=d97c39d. Keine Umdeutung des PR-Metadatenfelds base_sha zu aktuellem main.

**WP-027 entscheidet R01-FIRST und R18-MIDDLE.** Bible §§4.1/8.1: erster gezielter Variationsschwerpunkt, frühere Portseitenvielfalt zulässig; vorbereitender lokaler Einser-Portanker zulässig, weitere Lösung kombiniert mittlere Resthinweise. Keine neue Port-/Einserquote. Quellen: [WP-027 auf main](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/WORK_PACKAGES/WP-027_Season-1-Quellenklarstellung-und-CI-Leistungsweg.md), dort bestätigte Bible und ADR-032.

**CI-PERF-WP026 auf b1457ba erbracht.** WP-028: Freeze b449ab7, PR #25 ausschließlich nach WP-026 integriert als b1457ba, gleicher Tree 2bc50e6. Aktuelle Unity-CI [38637012837](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012837): 455/455 EditMode, 27/27 PlayMode, Android IL2CPP und iOS/Xcode erfolgreich; Architecture [38637012841](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012841) erfolgreich. 350 Samples, p95-Index 332, p95 38,2026 ms, Maximum 72,0952 ms, 240 frische Proofgates in 2.586,8097 ms mit vier Arbeitern. Git/Linux-Fingerprint 6fc307231c43d3f09c60fa3116c8517588a40855c4266e8ec4dd120ed0b6372a; Windows-CRLF-Fingerprint 29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679. Dies sind Nachweise dieses historischen Standes, kein vorweggenommener PASS neuer Metadaten.

**Neue unabhängige Gesamtabnahme durchgeführt, KEIN GESAMT-PASS / NICHT INTEGRATIONSBEREIT.** [Originalbericht](WP-026_Gesamtabnahme_b1457ba.txt), byteunverändert, SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433. Evidenzarchiv SHA256 e1a974ebfdf1f87ee9af6bb08981563e8ce3a5a3166c0c21aada92d81cfb4258, ausschließlich isoliert. Alle 240 logischen Rätsel/20 Routen gegen WP-027 geprüft; F-01 begrenzter Sachtextbefund, keine neue Puzzlelogikabweichung. Originale frühere Prüfberichte bleiben unveränderte standgebundene Historie.

F-01 wird unter dem bestehenden WP-026-Metadatenscope und separat verankerter WP-029-Nachweisführung korrigiert: x0=0, x1=2, x3=6, x4=5. Ein Level Revision 2→3, kein neuer öffentlicher Input, keine neue Lösung oder Proofbytes. Reale neue Bindungs-/Regenerationsnachweise und Grenzen stehen in [ACCEPTANCE_FOLLOWUP.md](ACCEPTANCE_FOLLOWUP.md). Gezielte unabhängige Nachprüfung auf neuem Freeze bleibt erforderlich.

Weiter offen: O-01 menschlicher Spieltest, O-02 historische HardBudget-Timeoutursache, O-04 drei tatsächliche Main-Konflikte/noch ungeprüfter gemeinsamer Stand, O-05 1.440 fehlende Referenzziele, O-06 unbestimmte Zeitklasse sowie O-07 kreative/Import-/Produkt-/Release-/Geräte-/Storegates. Kein Gesamt-DoD-PASS, Merge nach main, Runtimeimport oder Release. O-02/O-04 erhalten getrennte Untersuchungs-/Vorbereitungsaufträge; kein bestehendes Paket oder Manifest wird erweitert.

Die nachfolgenden älteren Abschnitte beschreiben ihren damaligen Stand. Ihre überholten Entscheidungs-/CI-PERF-Statusangaben sind keine aktuelle Blockerliste.


Originalablage der Abnahme und des isolierten Evidenzarchivs: `C:/Users/Anwender/.codex/visualizations/2026/10/14/01a13c6b-0929-7472-99ff-8d249ffd3cd5/wp026-audit/`. Beide tatsächlichen Bytehashes vor Verwendung geprüft. Kein Archivquellcode ins Repository übernommen.

Neue eigene Produktionsprüfung: 240 UNIQUE/1, drei Regenerationen je Quelle bytegleich; Strict-One/Batch/Reverse PASS, 3840 Metamorphosen und 1440 verworfene Mutationen. Lokaler net10.0-Zusatzharness, keine unabhängige Freigabe oder CI-Referenzmessung. Rohoutput F01_VALIDATION.txt, neue Bindungen F01_BINDINGS.json.

Eigene Vorläufe: isolierter Skriptversuch scheiterte vor Metadatenänderung an Windows-Konsolenencoding; mit UTF-8 korrigiert. Erster .NET-Restore war nicht erfolgreich; anschließend mit taskbezogenem DOTNET_CLI_HOME regulär bestanden. Keine Produkt-/Teständerung und kein fehlgeschlagener Lauf als PASS.
