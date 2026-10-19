# WP-041 – Public240-interne-Prueffassung (A)

## ID
`WP-041`

## Ziel
Alle240 echten oeffentlichen Puzzles in einem isolierten internen Unityreviewpfad selbst bedienbar machen, ohne Loesungswissen auszuliefern.

## Voraussetzungen
AGENTS vollstaendig und dortige Lesereihenfolge Konzept15/00/09, CURRENT_STATE und dieses WP; vollstaendiges MANUS-2026-10-19-01, WORK_QUEUE/WORK_PACKAGE_RULES/AI_HANDOVER_RULES/DEFINITION_OF_DONE und alle im T-Plan benannten Fachvertraege/ADR034/035 lesen. Abgeschlossene gemeinsame Basis exakt 925ca608a961ac26c84385c8da3b949f609df883. Eigener Anker dieses WP plus [unveraenderliches Productionmanifest](../tools/architecture-validation/scopes/WP-041.production.scope.json) gemeinsam vor Fachcode mit einzigem Parent exakt T. Kein Umbasieren auf das andere Kind.

T-Abschlussbindung vor Kindanker, fuer A/S identisch:
```json
{
  "schemaVersion": 1,
  "t": "925ca608a961ac26c84385c8da3b949f609df883",
  "tree": "6b2693ad596ba3b8fb84ef1044a9b367270d22b2",
  "parent": "54d2496ac2c74b681d24fe7a69d7f044392c46dd",
  "readyForChildAnchors": true,
  "releasedAtUtc": "2026-10-19T09:36:03.910Z",
  "authority": "R/Codex exclusively under MANUS-2026-10-19-01 section5; separate technical execution acceptance after actual independent PASS and required regular CI, no product/main/release approval",
  "independentReviews": [
    "T_CLOSEOUT_QC.md/.json PASS_CODE_AND_DOCUMENTATION_DELTA at D",
    "T_D_INDEPENDENT_RAW_QC.md/.json PASS_RAW_GIT_BLOB_AND_AUTHORITY_DELTA_ONLY",
    "entry_audit T_D_CI.md/.json D_REGULAR_CI_PASS; independent from T code"
  ],
  "workflowRuns": [
    {
      "id": 39224770669,
      "name": "Architecture Validation",
      "event": "push",
      "head_sha": "925ca608a961ac26c84385c8da3b949f609df883",
      "path": ".github/workflows/validate.yml",
      "run_attempt": 1,
      "status": "completed",
      "conclusion": "success",
      "created_at": "2026-10-19T08:43:13Z",
      "updated_at": "2026-10-19T08:43:44Z",
      "html_url": "https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39224770669"
    },
    {
      "id": 39224770689,
      "name": "Unity CI",
      "event": "push",
      "head_sha": "925ca608a961ac26c84385c8da3b949f609df883",
      "path": ".github/workflows/unity.yml",
      "run_attempt": 1,
      "status": "completed",
      "conclusion": "success",
      "created_at": "2026-10-19T08:43:13Z",
      "updated_at": "2026-10-19T09:24:27Z",
      "html_url": "https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39224770689"
    }
  ],
  "licensedJobReleaseUtc": {
    "117968400200": "2026-10-19T08:49:27.9731482Z",
    "117970765569": "2026-10-19T08:52:56.2101562Z",
    "117972117245": "2026-10-19T09:09:25.3839517Z",
    "117978722428": "2026-10-19T09:24:19.9148500Z"
  },
  "reports": [
    {
      "name": "T_D_CI.md",
      "bytes": 6620,
      "sha256": "0efc52fc4f4c386b49545e041323c507b09b8382cea2b42d7cd06379169d1205"
    },
    {
      "name": "T_D_CI_EVIDENCE.json",
      "bytes": 64960,
      "sha256": "a48558ac84ca0ffb1412cab9fc58d5ad4c5c90f9fedfb48a860514418e047f5e"
    },
    {
      "name": "T_D_RAW_BLOB_BINDINGS.json",
      "bytes": 10314,
      "sha256": "95c22d3ac0a80ac36c38ef0c2f5f06b842af0c3b2c3ceb7788f391a7029ec855"
    },
    {
      "name": "T_D_INDEPENDENT_RAW_QC.md",
      "bytes": 2736,
      "sha256": "1441ed9eb3643b781dc4d7a0f0afa20cfdf773b338263c8950fbdf3220bd74bc"
    },
    {
      "name": "T_D_INDEPENDENT_RAW_QC.json",
      "bytes": 16780,
      "sha256": "28f2b4b048f70088d86453a907293c1c8bf67077536cb923fc85178381cc578c"
    },
    {
      "name": "T_D_ROOT_TERMINAL_RUN_CHECK.json",
      "bytes": 987,
      "sha256": "848ad801b6bcf2c97343e139d9cc100b56b80febed5f52fb58d86926ec534a00"
    },
    {
      "name": "T_D_DURABLE_PACK.json",
      "bytes": 1969628,
      "sha256": "1113f76cd319a22d6789beda56f75ac3419221ed090ce3d60289ca0024985e23"
    }
  ],
  "remoteRefsAfterFreshFetch": {
    "main": "d90e798dd9b26d3f152f3b0c2d1d52cae641eb16",
    "development": "3d1ac59c3f70d50e05434a54d3ddec5d282bb0c3",
    "t": "925ca608a961ac26c84385c8da3b949f609df883"
  },
  "procedure": "Both child WP+immutable manifest first additions have sole parent/base exactly this completed T. Identical full binding embedded in both WPs. Original reports copied byte-identically into each own child evidence scope after anchors. No T single merge; LA/LS then isolated P then own prospective I then separate expected-HEAD non-main integration.",
  "formalCompletion": "T is technically fully completed and released as common base now. Formal shared CURRENT_STATE/WP/queue completion updates are owned by later prospective I, as previously accepted. Full user mandate remains active."
}
```

## Scope
A_IMPLEMENTATION_PLAN.md, DELIVERY_CONTRACT.md und angenommene T_TECHNICAL_DETAILS.md konkret umsetzen. Ausschliesslich neue A-Quellen/Assets/Tests/Projektor. Bestehende DevelopmentPuzzlequellen bleiben unveraendert. Eigene IReviewPuzzleSession-Read-/Commandgrenze auf echter Domain, eigene Darstellung/Locale und expliziter Editorentry. StpBuildEntrypoints/gespeicherte Verdrahtung erstI. Der validate.yml-Selektor wird ausschliesslich auf das eigene vorab verankerte Manifest gesetzt, keine weiteren Workflowaenderungen. Eigene fachliche Lieferfreezeannahme und Status im WP/Evidenzpfad dokumentieren; formaler WPabschluss mit gemeinsamer CURRENT_STATE-Nachfuehrung spaeter durch R im eigenen prospektiven I-Scope. Kein Kindmerge oder kumulativer Scopeclaim.

## Betroffene Dateien/Module
Abschliessend nur die folgenden neu anzulegenden Pfade; .github/workflows/validate.yml ist bestehend und nur der Selektor darf geaendert werden.

- `WORK_PACKAGES/WP-041_Public240-interne-Prueffassung.md`
- `tools/architecture-validation/scopes/WP-041.production.scope.json`
- `.github/workflows/validate.yml`
- `Assets/StammstreckenPuzzle/Scripts/Application/Public240ReviewSession.cs`
- `Assets/StammstreckenPuzzle/Scripts/Application/Public240ReviewSession.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/Public240ReviewData.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/Public240ReviewData.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/Public240ReviewPresenter.cs`
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/Public240ReviewPresenter.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/Public240ReviewGrid.cs`
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/Public240ReviewGrid.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/Public240ReviewInstaller.cs`
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/Public240ReviewInstaller.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/Public240ReviewBuild.cs`
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/Public240ReviewBuild.cs.meta`
- `Assets/StammstreckenPuzzle/UI/Public240Review.uxml`
- `Assets/StammstreckenPuzzle/UI/Public240Review.uxml.meta`
- `Assets/StammstreckenPuzzle/UI/Public240Review.uss`
- `Assets/StammstreckenPuzzle/UI/Public240Review.uss.meta`
- `Assets/StammstreckenPuzzle/UI/Public240ReviewLocale.json`
- `Assets/StammstreckenPuzzle/UI/Public240ReviewLocale.json.meta`
- `Assets/StammstreckenPuzzle/UI/Public240ReviewPanelSettings.asset`
- `Assets/StammstreckenPuzzle/UI/Public240ReviewPanelSettings.asset.meta`
- `Assets/StammstreckenPuzzle/UI/Public240.public.json`
- `Assets/StammstreckenPuzzle/UI/Public240.public.json.meta`
- `Assets/StammstreckenPuzzle/Scenes/QA/Public240Review.unity`
- `Assets/StammstreckenPuzzle/Scenes/QA/Public240Review.unity.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/Public240ReviewSessionTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/Public240ReviewSessionTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/Public240ReviewDataTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/Public240ReviewDataTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/Public240ReviewPresenterTests.cs`
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/Public240ReviewPresenterTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/Public240ReviewSmokeTests.cs`
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/Public240ReviewSmokeTests.cs.meta`
- `tools/public240/project.cjs`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/A_SOURCE_LOCK.json`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/A/*.md`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/A/*.json`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/A/*.txt`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/A/*.png`

## Ausdrücklich nicht erlaubte Änderungen
Keine alten Manifeste, T-/anderesKind-/gemeinsamen Registry-/Status-/Domain-/Solver-/JCS-/Textquellen ausser den oben ausdruecklich NEU benannten eigenen Dateien. Kein Main-/Produkt-/Release-/WP026-Gesamt-/WP035-/finalerGestaltungspass, keine Authoringloesungen/Proofs/Notizen im Player, keine neuen Sternzeiten/Preise/480Assets oder Hint/Daily/Generator/Kauf/Ads/IAP/Analytics/Cloudaktivierung. Keine Assemblykanten-/Budget-/Gate-/Runner-/Lizenzabsenkung. Keine erneute allgemeine Solver-/TextQC.

## Akzeptanzkriterien
| AK | Vor Code verbindlicher Nachweis |
|---|---|
| A01 | Eigener gemeinsamer WP-/Manifest-Erstadd mit einzigem Parent/Base exakt abgeschlossenes T; unveraenderte Manifestbytes und allein eigene Pfade. |
| A02 | Exakt240 public-only Projektionen aus Gitfreeze dc1652befd7545b4c2a2dd0580f35a4e8e05d72d; doppelte bytegleiche Projektion; repositoryseitiger Source-/Blob-/Hashlock, keine Authoringpayloads im Player. |
| A03 | Strikter geschlossener Loader, echte semantische Puzzleidentitaeten, alle240 auswaehlbar; Unknown/Duplicate/solution/proof/notes/Hashmutation scheitern. |
| A04 | Echte Domaincommands fuer alleTools/Counts/Batch/Undo/No-op/Timer/Pause/Resume/Revision/Dedup; keine zweite Regelautoritaet oder Loesungsabfrage. |
| A05 | Bedienbarer eigener UI-Toolkit-Entry und eigene neutrale deutsche UnityLocalization-Tabelle; reale Picker-/Button-/Pointeraktionen; echte4x4/10x10-Raetsel und selbstgebauter PuzzleSolved->Fahrt->Resultpfad. |
| A06 | Reproduzierbarer Builder mit festen Metas/GUIDs, zwei bytegleiche Erzeugungen und sauberer Reimport; vorhandene neutrale Theme-/Trainquellen explizit unveraendert. |
| A07 | Native Nichtnullgrafik mit echten normalen Screenframes4x4/10x10/Fahrt/Result, originale PNGs durch R betrachtet; kein Panel-only-/Batchersatz. Nachvollziehbarer interner Startweg, keine simulierten menschlichen240erTests. |
| A08 | Eigene kanonische Architecture-/Scope-Self-/Negativchecks, regulare komplette CI am echten LA und getrennte technische Freezepruefung ohne Blocker/High; eigene Lieferuebergabe mit Grenzen. |

## Tests
Vor Code die oben gebundenen konkreten AK-/Testmatrix und vollstaendigen eigenen Testpfade abgleichen. Alle vorgesehenen Positiv-/Negativ-/Regressions-/Grenzfaelle tatsaechlich ausfuehren; urspruengliche Fehler erhalten. Lizenzfreies Roslyn/Mono-Supplement nur als solches kennzeichnen. Kanonische CPython3.11.13/Node22.20.0 Architecture-/eigenerScope mit Selftests, tatsaechliche vollstaendige regulaere CI mit Original-XML/Logs/Checkout/Tree und unabhängige Freezepruefung. Native Unitylaeufe/Pushes nur durch R koordiniert nach belegter freier Lizenz; nie zwei gleichzeitige Instanzen/Ketten. Physische Geraete-/Store-/Human-/Produktgates bleiben separat.

## Risikoklasse
Hoch: neuer echter Contentreviewzugang und Datenleckgrenze. Begrenzung durch angenommenes T, eigenen prospektiven Scope und getrennte Freezepruefung.

## Definition of Done
Alle AK tatsaechlich erfuellt, eigene Source-/Originaltest-/CI-/Prueferbindung und bedienbare Uebergabe im eigenen Evidenzpfad. LA/LS darf als fachlicher Lieferfreeze angenommen werden; formaler WPabschluss einschliesslich CURRENT_STATE wird erst durch R inI dokumentiert. Gesamtauftrag bleibt bis P/I/unterintegrierter CI/Kontrolle offen.

## Bearbeitungsstatus
Prospektiv vor Facharbeit verankert; keine Implementierung oder AK-PASS behauptet.

### Tatsächlicher Implementierungsnachtrag – 19.10.2026
Eigener initialer A-Code-/Asset-/Teststand vollständig erstellt und lizenzfrei kompiliert; App20/20 und Content20/20 Contracts tatsächlich Passed. Die frühere Statuszeile ist der erhaltene Ankerstand. Maßgeblicher aktueller eigener AK-/Restgatestand: [A/STATUS.md](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/STATUS.md), [interner Startweg](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/README.md), [Originalsupplement](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/A_LOCAL_SUPPLEMENT.json). Tatsächliche Builder-/UI-/Grafik-/kanonische End-/CI-/unabhängige Freezeprüfung stehen noch aus; kein A-Lieferfreeze- oder Gesamt-PASS. R koordiniert ab gesichertem Snapshot09:53:54.559Z die erste echte Builderphase. Sourceänderungen pausieren während dieser Ausführung. Formale gemeinsame CURRENT_STATE-/Abschlussführung erst R in I, keine Kindscopeänderung.

Letzter Korrekturstand: erste Builderphase tatsächlich erfolgreich; Native02 mit20/13/7 wegen eigenem UIDocument-TearDownfehler ausdrücklich FAIL und Originale erhalten. Dispose eng korrigiert, zwei Ownershipregressionen ergänzt. Direkte feste Cataloghashbindung mittels echter selbstkonsistenter A/B-NeuHash-Regression21/20/1 rot→21/21grün abgesichert; App20/20 unverändert. Stage03 nur überholter Snapshot ohne Lauf; Stage04 mit22geplantenPlayModefällen plus Content-Unitynachweis durch R koordiniert. [Entwicklungshistorie](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/DEVELOPMENT_HISTORY.md), [maßgeblicher aktueller AK-/Gate-/Quellstand](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/STATUS.md). Noch kein eigener Lieferfreeze-/CI-/unabhängiger A-PASS.

**Letzter tatsächlicher Lieferstand:** Native04 terminalExit0/22von22Passed, keine Fail/Skip/Inconclusive; echter Nonbatch-D3D11-Screenpfad und Builderdoppelerzeugung/Reimport einschließlich Ressourcenregressionen. R betrachtete die vier verschiedenen aktuellen Originalscreens selbst; Pickerbild bytegleich10x10, kein Popup-/Humanpass. [A-Lieferübergabe mit allen Daten-/Source-/Original-/Bildbindungen und Grenzen](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/HANDOVER.md). Implementierung lokal technisch prüfbar, eigenständige reguläre Freeze-CI/Unity-EditMode und getrennte fachliche LA-Annahme noch offen. Keine Selbstannahme oder formaler WP-/Gesamtabschluss. R führt jetzt kanonische Schlusschecks, exakten Freeze/Veröffentlichung/CI sowie separate Abnahme fort; bei konkreten Befunden enge eigene Korrektur im unveränderten Manifest.

Aktueller gezielter Nachlauf nach separater Vorfreezeprüfung: sofortigen Restart-/Doppellösungs-Ride-Lifecycle und Creator-Panelnamen eng korrigiert,23.PlayMode-Regression kompiliert. Tatsächliche Nachprüfung und sauberer isolierter Panel-/Scene-/Meta-Neuaufbau noch offen; Native04Originale unverändert. [Maßgeblicher letzter eigener AKstand](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/STATUS.md) und [konkreter sichere Reproduktionsweg](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/ARTIFACT_REBUILD_INSTRUCTIONS.md). Kein Lieferfreeze- oder unabhängiger PASS vorweggenommen.

**Maßgebliches technisches Freezeangebot nach tatsächlicher Korrekturprüfung:** Native05 durch R Exit0/23von23Passed ohne Fail/Skip/Inconclusive, einschließlich echtem UI-No-yield-Doppellösungsfall; Source32bytes unverändert, vier unterschiedliche aktuelle NormalScreenoriginale von R selbst betrachtet. Native06 im gesonderten Scratch: saubere Creation der vier fehlenden eigenen Panel-/Scene-/Metadateien und zweiter tatsächlicher Lauf beide Exit0/jeweils4von4rawbytegleich den vorab gebundenen Originalen. [Aktuelle AK-Tabelle und ehrliche offene Gates](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/STATUS.md), [Native05Originalnachweis](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/A_NATIVE_SCREEN_05.json), [Native06Originalnachweis](../PROJECT_CONTROL/ProductionBlocks/Public240Save/A/A_NATIVE_REBUILD_06.json). Alle alten Fehler-/Zwischenoriginale bleiben unverändert. Quellen-/Dokumentenschreibpause nach diesem Nachtrag, R erhält den technisch prüfbaren Lieferstand; eigene kanonische Endchecks, reguläre vollständige Freeze-CI/Unity-EditMode und unabhängige fachliche LA-Annahme bleiben ausdrücklich offen. Kein Selbstannahme-/formaler WP-/CURRENT_STATE-/Gesamtabschluss.
