# WP-026: tatsächliches Startgate und Übergabe

Stand 13.10.2026. **0/240 konkrete Rätsel, 0/240 neue Proofs, keine Leveldesignabnahme.** Geliefert sind der vollständige verankerte Auftrag und die nachstehende konkrete Blockeranalyse. Kein Produktionscontent, keine kreative Änderung, keine Seasonfreigabe.

## Tatsächliche Basis

Frisch gefetchter `origin/main`: `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` (PR #20 Closeout), Proofintegration `f496ab9001d2043a17c3ffd2ff2a8a39d175495b` (PR #19). GitHub-Metadaten bestätigen PR #19 als merged. Offene PR-Suche lieferte keine Ergebnisse, Remote-Branchinventar und vollständige lokal gefetchte Historie keine WP-026-Verankerung. Vorheriger lokaler Checkout `92842bc...` war veraltet; dessen bereits abgeschlossene Implementierung/Prüfung wird nicht erneut begonnen. Der Diff dieses Checkouts zu main betrifft ausschließlich die drei Closeout-Dokumente.

## BLOCKER-WP026-001: fehlende belegte Pflichtreferenzen

Gilt für **alle 240 IDs** im geschlossenen Bereich Season 1 × Abschnitte 01–05 × lokale Routen 01–04 × Positionen 01–12. Die sechs betroffenen Felder:

| Feld | Benötigte vorhandene fachliche Zuordnung |
|---|---|
| `/content/titleKey` | Tatsächlicher Titel-Lokalisierungsschlüssel |
| `/content/statusKey` | Tatsächlicher Status-Lokalisierungsschlüssel |
| `/production/entryDeductionKey` | Tatsächlicher Schlüssel für den Einstiegsschluss; interne sachliche Erklärung ersetzt dieses Pflichtfeld nicht |
| `/completion/mapSegmentId` | Tatsächliche Mapsegment-Referenz |
| `/completion/trainMomentId` | Tatsächliche Zugmoment-Referenz |
| `/completion/resultTextKey` | Tatsächlicher Ergebnistext-Schlüssel |

`git ls-tree -r --name-only c9516bdbd5858fd9003d5207ec37d180ccc6ae23 Content` liefert keine Dateien. Produktionskataloge, Localization-/World-/Assetreferenzinventare unter den kanonischen Quellen fehlen. Die vorhandenen Beispiel-/Testzuordnungen sind ausdrücklich FIXTURE_ONLY; Konzepttexte nennen keine vollständige technische Zuordnung der sechs Pflichtfelder für alle 240 IDs. Weder neue Placeholder-Keys noch Fixture-IDs dürfen als tatsächliche Quellen erfunden werden.

Der Blocker ist **nicht** die fehlende finale Produktfreigabe, kein fehlender Release-Lock und kein pauschaler Anspruch auf fertige Assets: CONTENT_CATALOGS §3 erlaubt DRAFT für Authoring und Review; CONTENT_PIPELINE §5 trennt das technische Proofgate ausdrücklich von späteren Gates. Er betrifft die bereits beim Schema verlangten, nicht nullbaren Referenzwerte, die der Nutzer ausdrücklich nicht erfinden lässt.

Der vorhandene `ProofGenerator.Generate` lädt über `LevelV2Loader` und beendet einen ungültigen Input vor dem Solver. Der andere öffentliche Overload verlangt ebenfalls einen gültigen `LevelV2LoadResult`. `LevelV2Reader` prüft dieselben Pflichtfelder wie das Schema. Der gemeinsame Produktionspfad hat keinen dokumentierten Logik-only-Eingang. Direktes `PuzzleDefinition.Create`/`PuzzleSolver.Solve` ist technisch möglich, wäre aber nur Zwischenevidenz, kein vorgeschriebener Level-v2-/Produktionsproofnachweis. Direkte Codecaufrufe oder Ersatzformate beheben diese Vertragslücke nicht.

## Tatsächliche Reproduktion

Isolierter .NET-10-Zusatzharness gegen **unveränderte** Domain-, Solver-, Infrastructure.Content- und Editor.Content-Quellen in getrennten Assemblies. SDK 10.0.401, Target net10.0. Der unveränderte ContentModuleSkeleton wird nur im Zusatzharness wegen seiner Applicationports nicht kompiliert; dies ist kein Unity-/vollständiger Projektbuild.

Positive Kontrolle: vorhandenes `proof-v1-solver-v2-root.level.json`, unverändert und ausdrücklich FIXTURE_ONLY, `ProofGenerator.Generate` erfolgreich / UNIQUE. Danach ausschließlich Scratchmutationen je Pflichtfeld: entfernen, `null`, Leerstring. **Alle 18 Varianten abgewiesen, kein Solveraufruf und kein Proof.** Fehlend: `LVL-SCHEMA-REQUIRED`; null: `LVL-SCHEMA-TYPE`; leere Keys: `LVL-SCHEMA-PATTERN`; leere Map-/Train-ID: `LVL-SCHEMA-LENGTH`. Vollständige echte Ausgabe: [reference-gate-probe.txt](reference-gate-probe.txt).

Reproduzierbarer Aufruf vom Repositoryroot (Python 3 plus vorhandenes SDK):

```text
python tools/season1-authoring/reference_gate_probe.py
```

Der eingecheckte Wrapper erzeugt nur einen ignorierten `.utmp/wp026-reference-gate`-Harness und kompiliert die aktuellen Quellen, keine alten DLLs. Prüfprogramm: [ReferenceGateProbe.cs](../../../tools/season1-authoring/ReferenceGateProbe.cs). Er erzeugt keine neuen Level-/Proofdateien und verändert keine Fixtures. Das erste Scratchsetup ohne ausreichenden .NET-Restore scheiterte mit NETSDK1004; nach normalem Restore lief die beschriebene Gegenprobe erfolgreich. Dieser Setupfehler ist kein Produktionsfehler und kein PASS.

## Unabhängige Gegenprüfung

Separate rein lesende Subagentinstanz `/root/content_startgate_review`, ohne Änderungen, keine WP-025-Abnahme und keine Contentfreigabe. Fragestellung war ausdrücklich, den vermeintlichen Startblocker zu widerlegen, falls ein vorhandener regelkonformer Weg existiert. Ihr vollständiger Ergebnistext ist in [INDEPENDENT_START_GATE_REVIEW.md](INDEPENDENT_START_GATE_REVIEW.md) persistiert. Sie bestätigt die Pflichtfeldlücke, grenzt spätere Freigaben ausdrücklich davon ab und weist auf die technisch mögliche, aber nicht hinreichende pure Solver-Zwischenevidenz hin. Das ist keine Abschluss-QC über nicht vorhandene Rätsel.

## Was den Blocker auflöst

Belegte, maßgebliche Zuordnungen der sechs Pflichtreferenzen für alle betroffenen IDs bereitstellen; hierfür sind die zuständigen Text-/Lokalisierungs- und Completion-/World-Content-Verantwortlichen erforderlich. Fertige kreative Ausgestaltung wird dadurch hier nicht pauschal zum Authoringstartgate erklärt. Alternativ bedarf die Vertragslücke einer **separat bestätigten** technischen/architektonischen Lösung nach bestehendem Entscheidungsverfahren. Dieser Auftrag entscheidet oder implementiert keine solche Änderung.

Keine reine Fortsetzungsanweisung erforderlich: Der vollständige zulässige Auftrag ist erteilt. Es fehlen tatsächliche Eingaben beziehungsweise eine erforderliche separate Vertragsentscheidung. Nach ihrer dokumentierten Auflösung denselben WP-/Manifeststand wiederverwenden, Umfang und laufende Arbeit frisch prüfen; außerhalb der bestehenden Allowlist entstehender Änderungsbedarf bleibt ein eigener Übergabepunkt.

## Weitere offene Nachweise, keine vorgezogenen Startblocker

- Finale Sekundenwerte bewusst ausgeschlossen; `starThresholdsSeconds: null` ist zulässig und kein Blocker. Vorläufige Zeitklassen sind keine Zeitkalibrierung.
- Schema-/Domain-/240er-Vollständigkeit, echte Produktionsproofs, Rootspuren, Determinismus, Nodehashes, Ähnlichkeit und individuelle Leveldesignbewertung: noch nicht ausgeführt, weil keine zulässigen vollständigen Quellen erzeugt wurden.
- Spieltests und unabhängige technische/Contentabschlussabnahme: offen. Eine Reviewerinstanz für diese Prüfung ersetzt weder Nutzer-Spieltest noch Produktfreigabe.
- Cross-References, Pseudolocale, Miniatur-/Präsentationsnachweise, Doppelimport, Addressables, Lizenzen, Größenbudgets und Release-Lock: bestehende Pflichten unverändert; fehlende Kreativinhalte und ausdrücklicher Importausschluss bleiben konkrete Grenzen. Kein Gate als bestanden oder nicht mehr erforderlich dargestellt.
- BLOCKER-PROD-001 bis -003 betreffen ihre dokumentierten Folgefeatures und sind **nicht** als genereller Blocker kuratierter Season-1-Logik umgedeutet.

Letzter abgeschlossener Schritt: aktueller Main-/PR-Abgleich, vollständige Auftragsverankerung, Pflichtreferenzbefund und API-Gegenprobe. Nächster fachlicher Schritt: belegte Referenzzuordnung oder separat bestätigte Lösung der Vertragslücke; erst dann Erstellung des gesamten beauftragten Bestands. WP-026 bleibt offen/blockiert.
