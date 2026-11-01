> Persistenznachtrag der Geschäftsführung: Die nachstehende gesonderte Teilprüfung war ausschließlich lesend. Ihre ursprüngliche temporäre Ablage und Aussagen beziehen sich auf die erstellende Prüfrolle; Manus hat diesen Bericht anschließend als Entscheidungsbeleg im erlaubten Reviewpfad gesichert. Nur Zeilenendleerzeichen sind für den Git-Diffcheck normalisiert; kein Urteil oder Sachbefund geändert. Rohe Downloads bleiben im ignorierten Bereich; Run-/Artefakt-IDs, Digests und Gitobjektbindungen sind hier dauerhaft nachvollziehbar.

# WP034-Reparatur – Fachanalyse zur technischen Integration in den WP036-Pfad

**Status:** Read-only-Entscheidungshilfe, keine Freigabe und keine Implementierung
**Untersuchungsziel:** Ob und wie die belegte WP034-Aufwandskorrektur für `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` eng, prospektiv und ohne Übernahme des WP026-Korpus in den auf `ff1836e293a2a5cff69773525c347b55deb67dfb` eingefrorenen WP036-Pfad wiederverwendet werden kann.

> Diese Analyse hat **keinen Code importiert, keinen Branch/Commit/PR verändert und keine Tests, Builds, CI-Läufe oder Retries gestartet**. Die einzigen neuen Dateien liegen in diesem ignorierten Verzeichnis.

## 1. Entscheidung in einem Satz

**Empfohlen ist kein Whole-WP026-/PR21-Import und keine ungeprüfte „Garantiereparatur“, sondern ein neues, prospektiv verankertes, enges Produktions-WP auf einem Kindbranch von PR31-HEAD `ff1836…`: nach eigenem ADR030-Trust-Anchor nur den RootDeductionTracer-Hunk aus WP034 plus die 1639er-Trace-/Metrik-Regression übernehmen, den bereits passenden HardBudget-Test unverändert lassen, danach am neuen Freeze reguläre CI und unabhängige Scope-/Diff-Integration abwarten.**

Der künftige WP-Identifier ist hier absichtlich **nicht** festgelegt. Vor Verankerung muss die verantwortliche Stelle die ID-Vergabe prüfen (der gelesene Bestand reicht derzeit bis `WP-036`).

## 2. Verbindliche Untersuchungsbasis und Objektbindungen

| Rolle | Commit / Tree | Feststellung |
|---|---|---|
| Main-Baseline | `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` | Von PR31 abstammende Main-Basis; drei Kandidatdateien noch vor WP034. |
| WP036-/PR31-Quellfreeze | `ff1836e293a2a5cff69773525c347b55deb67dfb`, Tree `dd08dd7892ce6186ddb29598d4abae04852d658c` | PR31-HEAD, Parent `89c8d2b…`; `d90e798…` ist Vorfahr. Arbeitsbaum bei Prüfung sauber. |
| WP034-Reparaturfreeze (Quellnachweis) | `f34d6f4ca437effb0ce422015b02618b2a31a9c2`, Tree `cb4dec722580f3024dfba8a5d6555d3b36ebe960` | Commit `fix: reuse root premise views without changing solver-v2 deductions`; einziger Produktionsquellendiff in `RootDeductionTracer.cs`. |
| WP034-Unterintegration | `085357d59a62dae73189ea76051c73f0ffb78ae3`, Tree `58ee055410afd3c5e6a3acba9c47ff4f42050cf8` | Merge von WP034 in den WP026-Weg; `f34d6f4…` ist Vorfahr. |
| PR21-/WP026-Stand | `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d`, Tree `4b84b56d0c7446d5182018b02f8c223bbe36c096` | Dokumentationsfolge auf `085357…`; `f34d6f4…` ist Vorfahr. PR21 ist offen/Draft gegen `main`. |

### Gelesene Primärquellen aus dem verifizierten PR21-Objekt `dc1652…`

| Pfad | Git-Blob | SHA-256 des via `git show` gelesenen LF-Inhalts | Aussage, die hier verwendet wurde |
|---|---:|---:|---|
| `PROJECT_CONTROL/Investigations/WP-034_INTEGRATION.md` | `c0779b2…` | `d1c05c7531b88e15d92bf18be3f4cc8f0f12ad71411bc4557fb0fc41027d9be6` | Tatsächliche Unterintegration, Trees, CI, harte Budgetgrenze und Grenzen. |
| `PROJECT_CONTROL/Investigations/WP-034_HANDOVER.md` | `2de9bd6…` | `15d3abb4c309690c77cfb81c4675068121874375a5a56c4676ad0a2f28949940` | Ursache, Korrekturalgorithmus, Freeze, CI- und Übergabegrenzen. |
| `PROJECT_CONTROL/Investigations/WP-034_QC.md` | `038bfef…` | `787ad773ee14f633317422dad998d1617800f327859cff389cddb1d38b997640` | Vorhandene getrennte technische Abschlussprüfung und deren Evidenzgrenzen. |
| `WORK_PACKAGES/WP-034_HardBudget-Zuverlaessigkeit.md` | `2d5bd01…` | `08f1aeaa443df8d058f30afbecb5965e90ff479e093be81d6ed90977386eeb3a` | Ursprünglicher enger Auftrag, AK und ausdrücklich ausgeschlossene Änderungen. |

Die WP034-QC-Quelle dokumentiert zusätzlich ihren historischen CRLF-Originalhash `f17c2cc31bed2b4ee957946100b7368c8ddd8e22c8092f48002a3837fc6e639c`; der oben ausgewiesene Hash ist korrekt als Git-LF-Blob getrennt ausgewiesen.

### Verankerung des vorhandenen WP034 – Nachweis, nicht Übernahmerecht

Der bestehende WP034-Trust-Anchor ist `f59d28bfb58331d3e9161708d99eb1834c5ae7e2`, einziger Parent `a2e26a658a2bcdd47002698c150cb5736d9760da`. Er führte WP und Manifest gemeinsam ein. Das historische Manifest `tools/architecture-validation/scopes/WP-034.production.scope.json` hat SHA-256 `417c621ddfeea9a60963e8f5591720be8cd56af93515b18a66dc50b9c8deaa11` und `baseCommit=a2e26a…`.

Das belegt die damalige WP034-Lieferung. Es **autorisiert nicht**, seine Allowlist, sein altes Manifest oder sein gesamtes WP034/WP026-Artefaktpaket in den WP036-Pfad zu übernehmen. ADR030 verlangt für eine neue produktive Änderung einen eigenen, vor der Fachänderung gemeinsam hinzugefügten WP-/Manifest-Anchor.

## 3. Was die bestehende HardBudget-Aussage genau prüft

`Assets/StammstreckenPuzzle/Tests/EditMode/Solver/PuzzleSolverTests.cs`, Methode `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind`, ist am Main- und WP036-Stand **unverändert**. Sie prüft sechs Fälle:

| Große Ergebnisart | Fixture / Steuerung |
|---|---|
| `UNSATISFIABLE` | 32×32 unsatisfiable serpentine |
| `MULTIPLE_OR_MORE` | 32×32 ambiguous corner |
| `MULTIPLE_OR_MORE` | 32×32 ambiguous long path |
| `INDETERMINATE` | 32×32 long path; kleinste State-Grenze, die eine Lösung hat, aber Suche nicht beendet |
| `UNIQUE` | 32×32 serpentine |
| Referenz | tiny unsatisfiable 2×2 |

Für jede große Art bleiben dabei erhalten: 2 Warm-ups, 25 Samples, dieselben Fallgrößen, derselbe `600000`-ms-Testzeitrahmen, dieselbe State-Grenzsuche und die 40%-Assertion. Gemessen wird die Zeit **vom letzten `monotonicNow`-Probe bis zur Rückgabe**, abzüglich der kleinen Referenz, gegen die im selben Lauf gemessene Kosten der Ergebnis-Kopie (`Path` und `Reductions`). Die Assertion scheitert, wenn diese zusätzliche Restzeit nicht kleiner als 40% der gemessenen Ergebnisbauzeit ist. Das ist eine zielgerichtete Regression gegen nach dem letzten Probe gebaute große Ergebnisse; sie ist keine deterministische Allokationsmessung und keine allgemeine Host-Latenzgarantie.

Die zugehörige bestehende `PuzzleSolver.Solve`-Semantik baut vor seinem finalen `CheckBudget()` die Deduplication, bei `UNIQUE` die Trace-/Metrikdaten und für **jede** Kandidatenklasse das `SolverResult` einschließlich Kopien von Path/Recovery-Protokoll. Erst danach liegt der finale Probe; bei Überschreitung wird fail-closed `INDETERMINATE` zurückgegeben. WP034 änderte diese Budget-/Ergebnisbaufolge **nicht**.

## 4. Befund zur tatsächlichen WP034-Reparatur

### 4.1 Belegte Ursache in Reichweite der Änderung

Vor WP034 erzeugte `RootDeductionTracer.Record` bei der anfänglichen Herleitung, bei jedem versuchten Prämissenwegfall und bei der anschließenden `MinimalFacts`-Berechnung erneut `View(included)`: vollständige Rasteransicht, Allokation und Durchlauf der vorhandenen Spur.

Die konkrete Korrektur im Freeze `f34d6f4…` baut die View einmal. Beim testweisen Wegfall eines Eintrags rekonstruiert sie ausschließlich dessen Zelle aus allen noch eingeschlossenen Kandidaten der gleichen Zelle; bei fehlgeschlagenem `Holds` restauriert sie sowohl `included[j]` als auch den gesicherten Zellwert. `MinimalFacts` verwendet danach dieselbe, fertige View.

Damit reduziert die Änderung nachweisbar Arbeit **vor** dem bereits existierenden letzten Budgetprobe. Sie verschiebt weder Ergebnisbau noch den Probe und ist deshalb keine semantische „Budgetfix“-Umschreibung, sondern eine äquivalente Aufwandskorrektur der Root-Trace-Berechnung. Die vorhandene unabhängige WP034-QC beschreibt als ergänzende kontrollierte Befunde `4.283.337 → 231.264` View-Aufrufe und `4.488.252.800 → 242.172.728` View-Allokationsbytes; diese Werte sind nicht als künftige CI-Garantie wiederzuverwenden.

### 4.2 Keine unzulässige Ursachenbehauptung

Die historische Timeout-Ursache ist **nicht** als Host-GC-, Scheduling- oder CPU-Wurzelursache bewiesen. Die Originale zeigten zwar unterschiedliche CPU-Modelle zwischen PASS/FAIL und eine 180-s-Gesamtzeitüberschreitung, aber keine Telemetrie, die eine konkrete Hostursache isoliert belegt. Das vorliegende Fachurteil übernimmt ausdrücklich nur den engen, quellen- und differentialbelegten Befund: unnötige vollständige `View`-Rekonstruktionen wurden vermieden.

### 4.3 Vorhandener Freeze- und Integrationsnachweis

| Stand | Bereits dokumentierter Nachweis | Zulässige Folgerung |
|---|---|---|
| `f34d6f4…` | Architecture Run `38725866779` SUCCESS; Unity Run `38725866769` SUCCESS mit zehn erfolgreichen Jobs; EditMode `456/456`, PlayMode `27/27`; HardBudget `48,655587 s`. | Der WP034-Quellfreeze wurde seinerzeit vollständig regulär geprüft. |
| `f34d6f4…` | Vorhandene unabhängige technische WP034-QC: PASS ohne offene technische Befunde auf genau diesem Freeze; vollständige Trace-/Premissen-/Metrikbindung und Mutationsbelege beschrieben. | Starkes Wiederverwendungsevidenz, aber **keine** Abnahme eines neuen WP036-Kindbranches. |
| `085357…` / PR21-Integration | Integration treegleich zum Lieferstand; dort neue Architektur- und Unity-Läufe dokumentiert. HardBudget bestand laut Bericht `45,958366 s` (Push) bzw. `46,918725 s` (PR). | Die Reparatur blieb im integrierten PR21-Baum erhalten. |
| aktuelles `dc1652…` | PR21 laut read-only API: Draft/open, mergeable clean; 22 Check-Runs SUCCESS. Die drei Kandidatblobs entsprechen `f34`. | PR21 ist eine nachweisbare Quelle, **kein** zu importierender Integrationscontainer. |

## 5. Exakte Pfade, Commits und minimale Patchmenge

### 5.1 Die drei maßgeblichen Quell-/Testpfade

| Pfad | WP034-Commit / Änderung gegenüber `a2e26a…` | Ziel bei Wiederverwendung |
|---|---|---|
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/RootDeductionTracer.cs` | `f34d6f4…` gegenüber Parent `840b929…`: **+12/-3**. Einziger Produktionshunk. | **Übernehmen**, aber nur diesen Hunk und nur nach neuem Anchor/erneuter Kontexteinstellung. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/PuzzleSolverTests.cs` | `840b929…`: **+12/-0** Diagnose-/Phasenlogging außerhalb der gemessenen Tail-/Copy-Intervalle. Die HardBudget-Assertion selbst, ihre Grenzen und Fälle wurden nicht geändert. | **Nicht übernehmen.** Der Zielstand besitzt die originale, bereits passende HardBudget-Regression bytegleich; nur unverändert ausführen/auswerten. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/SolverV2MetricsTests.cs` | `f34d6f4…`: **+17/-0** Test `LargeRootTrace_PreservesSolverV2PremiseSelectionAndMetrics`. | **Übernehmen.** Bindet 32×14/1639 Schritte, Metriken `(0,1639,30,0)` und Trace-SHA-256 `6E59FFFAFB16B61C9FDDC301ED9229078FEEA11BEF1E2F44616CE7F9D8460B78`. |

**Minimal zulässige neue fachliche Patchmenge:** Zwei Dateien, insgesamt **+29/-3**: der Produktionshunk in `RootDeductionTracer.cs` und der 17-Zeilen-Regressionstest in `SolverV2MetricsTests.cs`. Die dritte Datei ist zwingender bestehender Prüfkorridor, aber kein zu kopierender Hunk. Ein noch kleinerer Produktions-only-Port (`+12/-3`) wäre für eine hochriskante solver-v2-Optimierung nicht ausreichend abgesichert.

Explizit **nicht** Teil dieser Minimalmenge: `PuzzleSolver.cs`, `.github/workflows/validate.yml` aus WP034, WP034-Diagnose-/Vergleichsskripte, die WP026-240er-Korpusdateien/-Tests, ADR032-Protokoll-/Benchmarkmaterial, WP026/WP034-Statusdateien, Quellen, Proofs, Level, alte Manifeste oder frühere Work Packages.

### 5.2 Kompatibilität zum tatsächlichen WP036-Stand

Die Kandidatdateien haben in WP034-Ausgangsbasis `a2e26a…`, `main d90e…` und PR31-HEAD `ff183…` jeweils identische Vor-Reparatur-Blobs:

| Pfad | identischer Vor-Reparatur-Blob in `a2e…` / `d90…` / `ff183…` | Reparaturblob in `f34…` |
|---|---:|---:|
| `RootDeductionTracer.cs` | `bd719d0395aa6f5f3d6398d9ceec5d3c8fcf3eaa` | `a41a802fd3e3f2f54074bd0d1e0bf540d0ae5a6c` |
| `PuzzleSolverTests.cs` | `5931f0c165c084d67b344173171ea07abe2767b0` | `c2484d61e3a96b10ec00754ef646a3895c53eb7c` |
| `SolverV2MetricsTests.cs` | `9fb1f0482a23eecf888b7ccc6ca9be1c7f945ef1` | `6be782565505376c960fc3f040dd1de95f3202bc` |

Dies ist ein starker, lokaler Hunk-Kompatibilitätsnachweis: Der Quellkontext des Root-Hunks und der Testeinfügepunkt sind auf `ff183…` unverändert. `git merge-tree` gegen die gemeinsame Mainbasis zeigt für diesen Root-Hunk keine inhaltliche Kollision. Das ist **keine** abschließende Solverprüfung und ersetzt weder Review noch den neuen Scope-/CI-Nachweis.

Wichtig: PR31 enthält die Reparatur aktuell **noch nicht**; sein RootTracer-Blob ist weiter `bd719d0…`. Dagegen bleiben die drei in PR21 integrierten Blobs von `085357…` bis `dc1652…` gegenüber `f34…` unverändert.

## 6. Abhängigkeiten, lokale Semantikrisiken und Pflichtprüfpunkte

| Prüfpunkt | Warum er trotz Blobgleichheit beim neuen Freeze erneut geprüft werden muss | Erwartetes Ergebnis |
|---|---|---|
| Exakte Vor-Kontexte | Ein späterer PR31-HEAD könnte vor Implementierung abweichen. | Die drei Vor-Reparatur-Blobs und der `PuzzleSolver.cs`-Budgetpfad sind vor Patch erneut mit dem dokumentierten Stand zu vergleichen; sonst stoppen. |
| Mehrere Einträge/überlappende Masks einer Zelle | Ein rückwärtiges OR wäre falsch; der WP034-Hunk muss durch erneutes AND der Komplemente aller **eingeschlossenen** Kandidaten rekonstruieren. | Gleichheit der View-Invariante vor/nach jedem Versuch; keine verkürzte „Optimierung“. |
| Delete-/Prämissenordnung | Der Algorithmus muss Kandidatenreihenfolge, newest-first deletion und aufsteigend publizierte Premissen beibehalten. | Neuer 1639er Tracehash sowie Metriken unverändert. |
| Fehlgeschlagener `Holds` / `Faulted` | Der gespeicherte Wert und `included[j]` müssen gemeinsam restauriert werden; `Faulted` darf keinen anderen Facts-Pfad bekommen. | Bestehende Trace-Sufficiency-/Irredundanzregressionen und neuer Hash unverändert. |
| Budget-/Ergebnisbau | Die Korrektur darf `CheckBudget`, Klassifikation, `SolverResult`-Kopien und Fail-closed `INDETERMINATE` nicht berühren. | `PuzzleSolver.cs` bleibt unverändert; die bestehende Sechs-Fälle-HardBudget-Assertion bleibt byte- und parametergleich. |
| Workflow-/Manifestkonflikt | WP034 schaltete seinen Workflow auf sein eigenes Manifest; PR31 steht auf WP036. Das ist kein zu übernehmender Hunk. | Nur ein **neuer** eigener Workflow-Manifestwechsel im neuen Anchor; kein Edit am alten WP036-Manifest. |

## 7. Empfohlenes prospektives Vorgehen (nicht in dieser Analyse ausgeführt)

### A. Vor Code: eigener zulässiger Anchor

1. Einen Kindbranch **exakt von `ff1836e293a2a5cff69773525c347b55deb67dfb`** erstellen, nicht von PR21/WP026 und nicht von `f34`.
2. Nach verantwortlicher ID-Prüfung einen neuen Work-Package-Pfad und ein neues Production-Scope-Manifest im **selben ersten Add-Commit** hinzufügen. `baseCommit` muss exakt `ff183…` sein; das WP muss per sicherem lokalem Markdownlink auf genau dieses Manifest zeigen.
3. Dieses neue Manifest muss nur erlauben: eigenes WP, eigenes Manifest, die künftige aktive Manifestzuordnung in `.github/workflows/validate.yml`, die zwei Minimal-Patchpfade und eng begrenzte, eigene Nachweis-/Übergabedateien. Keine breiten Globs für `Content`, `Proofs`, `Assets/**`, ADRs oder WP026-Artefakte.
4. In diesem Anchor die aktive Workflow-Manifestzuordnung auf **das neue** Manifest umstellen. Das alte `WP-036.production.scope.json` bleibt unverändert.

Das erfüllt ADR030: genau ein erreichbarer Add-Commit, Manifestbasis gleich Anchor-Parent, neues WP und Manifest beide `A`, historischer lokaler Link. Ein nachträglich geschriebenes oder umgedeutetes WP wäre kein Trust-Nachweis.

### B. Danach nur der nachgewiesene Minimalport

5. In einem getrennten Codecommit ausschließlich den oben bestimmten `RootDeductionTracer.cs`-Hunk plus `SolverV2MetricsTests.cs`-Regression einbringen. Kein Cherry-pick/merge des WP034- oder WP026-Commitstapels und kein Import fremder Hilfen/Evidenzen.
6. Der bestehende `PuzzleSolverTests.cs`-HardBudget-Test bleibt unverändert. Er ist Teil der finalen Testauswertung, nicht Teil des Diff.
7. Einen exakt benannten Lieferfreeze festlegen; erst auf **diesem** Head Scope, reguläre CI und unabhängige technische Prüfung beurteilen. Keine historische f34-/PR21-CI auf den neuen Head übertragen.

### C. Erst dann Integration in den PR31-Weg

8. Eine spätere Unterintegration soll die dafür im Projekt vorgesehene Route nutzen (Kindbranch/Unter-PR gegen den PR31-Zweig, falls die zuständige Stelle dies eröffnet). Nach tatsächlicher Integration sind Tree/Diff, aktives Manifest, reguläre CI auf dem integrierten PR31-Head und eine unabhängige Scope-Integration erneut zu binden.
9. Das ist weder ein Main-Merge noch Produkt-/Word-/Runtimeimport-/Releasefreigabe. WP026 bleibt vollständig außerhalb dieser Liefermenge.

## 8. Vorgeschlagene AK und Testauswertung für den neuen Auftrag

Die folgenden Schritte sind **Plan-/Abnahmedaten**, nicht von dieser Analyse ausgeführt:

| AK | Abnahmebedingung |
|---|---|
| AK-Anchor | Neuer WP-/Manifest-Add-Commit gemäß ADR030; Parent exakt `ff183…`; historischer Link, IDs, Manifestbytes und enge Allowlist validiert. |
| AK-Scope | Diff `ff183… → finaler Lieferhead` enthält nur erlaubte eigene Governance-/Nachweisdateien, die neue aktive Manifestzuordnung und die zwei Minimal-Patchpfade. Kein WP026-Korpus, keine ADR032-Workflows/Benchmarks, Quellen, Proofs oder alte Manifeste. |
| AK-Source | Vor dem Port stimmen die drei Vor-Reparaturblobs mit oben genannten Werten; der Produktionsdiff ist exakt semantikgleich zum `f34`-Hunk. `PuzzleSolver.cs`/Budget-/Ergebnisbau bleiben unverändert. |
| AK-Budget | Die bestehende Methode `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` bleibt ungeschwächt (Fälle, 2/25, State-Suche, 40%, Zeitgrenze) und besteht in regulärer Unity-EditMode-Auswertung. Keine Timing-Garantie aus einem Einzellauf ableiten. |
| AK-Semantik | Neuer Test bindet Klassifikation, `(0,1639,30,0)` und den dokumentierten 1639er Tracehash; bestehende Solver-v2-/Proof-/Trace-/Sufficiency-/Irredundanztests bleiben grün. |
| AK-CI | Reguläre unveränderte Pipeline am finalen neuen Head: Architecture-only plus neuer Production-Scope mit Selftests, Unity EditMode, PlayMode/QA, Android Development IL2CPP und iOS/Xcode; Originalstatus/Artefakte passend zum tatsächlichen Checkout auswerten. |
| AK-Unabhängigkeit | Eine vom Implementieren getrennte technische Prüfung bewertet Sourcefreeze, Diff, Manifest/Anchor, Budget- und Traceergebnis sowie tatsächliche CI. Sie ist keine Selbstfreigabe. |
| AK-Integration | Nach erlaubter Unterintegration in den PR31-Weg: integrierten SHA/Tree/Diff und reguläre CI neu binden; keine frühere PR21-/f34-Evidenz substituieren. |

**Vorgeschlagene, erst später ausführbare Soll-Kommandos** (mit vom Verantwortlichen zu setzenden Platzhaltern; in dieser Analyse nicht ausgeführt):

```bash
# Anchor-/Scope-Prüfung am finalen Lieferhead
python3 tools/architecture-validation/validate.py --self-test \
  --scope production \
  --scope-manifest tools/architecture-validation/scopes/${ASSIGNED_WP_ID}.production.scope.json

# Übernahmekorridor vor/bei Review: nur zwei Produkt-/Testdateien dürfen fachlich abweichen
git diff --name-status ff1836e293a2a5cff69773525c347b55deb67dfb..${FINAL_HEAD}

# Die reguläre Unity-EditMode-Auswertung muss insbesondere beide Tests enthalten:
# HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind
# LargeRootTrace_PreservesSolverV2PremiseSelectionAndMetrics
```

Der erste Befehl ist nur nach Bereitstellung der im Workflow gepinnten Python-/Node-Abhängigkeiten im vorgesehenen Laufweg auszuführen; der zweite diff ersetzt keine CI. Keine Tests sollen durch erneutes bloßes Wiederholen, gelockerte Budgets, Quarantäne oder geänderte Goldenwerte „repariert“ werden.

## 9. Aktueller PR-/CI-Kontext als Integrationsvoraussetzung

Read-only API-Stand während dieser Analyse:

| Pull Request | Stand |
|---|---|
| PR31 | offen/Draft, Head `ff183…`, Basis `d90e…`, mergeable `true`, `mergeable_state=unstable`. 22 Check-Runs: 18 SUCCESS, 3 SKIPPED, 1 FAILURE. Der Push-Unity-Run `39079576211` fehlte im EditMode; der separate PR-Unity-Run `39079578360` und beide Architecture-Runs (`39079576265`, `39079578380`) waren SUCCESS. Es wurde weder Retry noch neue Diagnose-/Reparaturausführung ausgelöst. |
| PR21 | offen/Draft, Head `dc1652…`, Basis `d90e…`, mergeable clean; 22 Check-Runs SUCCESS. |

Aus dem gemischten PR31-Checkzustand folgt keine Aussage über den neuen Patch. Ein künftiger Integrationsentscheid braucht daher die tatsächlich abgeschlossene reguläre CI auf seinem eigenen, exakt gebundenen Liefer- und Integrationshead.

## 10. Endbewertung für die entscheidende Stelle

- **Technische Wiederverwendbarkeit:** Ja, auf Quellkontextebene hoch: alle drei relevanten Vor-Reparaturblobs sind zwischen WP034-Ausgangsbasis, Main und PR31-HEAD identisch; die zwei fachlich benötigten Hunks sind isolierbar.
- **Nachweis zur Wirkung:** Ja, aber eng: WP034 belegt die Reduktion unnötiger Root-Trace-Arbeit und den Erhalt der solver-v2-Ausgaben. Es belegt **nicht** eine bestimmte historische Host-GC-Ursache oder universelle Timeoutfreiheit.
- **Freigabe zur direkten Übernahme:** Nein. Der WP034-Anchor/CI/QC bindet `f34`, nicht einen neuen WP036-Kindbranch.
- **Empfohlene Entscheidung:** Einen neuen, kleinsten zulässigen, prospectiven Reparaturauftrag nach ADR030 schaffen und ausschließlich die beschriebene Zwei-Dateien-Patchmenge implementieren lassen; keine WP026-Gesamtintegration und keine Änderung an WP036s historischem Manifest. Erst nach neuer regulärer CI, unabhängiger Scope-/Diffprüfung und späterer tatsächlicher Unterintegration ist ein Folgeentscheid sachgerecht.

## 11. Read-only-Provenienz

Ausgewertet wurden Gitobjekte und lesende GitHub-API-Daten sowie die verpflichtenden Repositoryquellen (AGENTS, Übergabe 15, Index 00, Master 09, CURRENT_STATE, WP036, Arbeits-/Übergaberegeln, DoD, Queue und WP036-Manifest). Verwendete read-only Git-Operationen umfassten `show`, `ls-tree`, `diff`, `merge-base`, `merge-tree`, `log`, `grep` und `status`. Kein Patch wurde angewendet; der Arbeitsbaum war am Schluss sauber.
