# WP-042 – Lokaler-Save-Restore-Grundfortschritt (S)

## ID
`WP-042`

## Ziel
Vollstaendiger providerfreier lokaler Save-v2-/Checkpoint-/Restore-/Recovery-/Grundfortschrittsablauf, echter atomarer Writer und Commit-vor-Erfolgsfreigabe, ohne ausgeschlossene Features zu aktivieren.

## Voraussetzungen
AGENTS vollstaendig und dortige Lesereihenfolge Konzept15/00/09, CURRENT_STATE und dieses WP; vollstaendiges MANUS-2026-10-19-01, WORK_QUEUE/WORK_PACKAGE_RULES/AI_HANDOVER_RULES/DEFINITION_OF_DONE und alle im T-Plan benannten Fachvertraege/ADR034/035 lesen. Abgeschlossene gemeinsame Basis exakt 925ca608a961ac26c84385c8da3b949f609df883. Eigener Anker dieses WP plus [unveraenderliches Productionmanifest](../tools/architecture-validation/scopes/WP-042.production.scope.json) gemeinsam vor Fachcode mit einzigem Parent exakt T. Kein Umbasieren auf das andere Kind.

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
S_IMPLEMENTATION_PLAN.md einschliesslich S01-S12/Recordgrammatik/Goldens, angenommene T_TECHNICAL_DETAILS.md und T_WRITE_SAFETY_ACCEPTANCE.md vollstaendig umsetzen. Vor Codecimplementierung vollstaendige feste unabhängige Goldenbytes und geschlossene V1-/V2-Reader-/Policybindungen im eigenen Evidenzpfad festhalten. Domaincheckpoint als neue Fabrik ohne Aenderung alter Sessiondatei. Typisierte inaktive Bereiche erhalten, keine Operationen aktivieren. QA-Katalogadapter konsumiert nur T-IPublicPuzzleCatalog, keine A-Referenz. Echte9Writerkernschritte plus Sicherungsfenster, CAS/Recoverylease/Autosave und Nullsternzeiten nachweisen. Der validate.yml-Selektor wird ausschliesslich auf das eigene vorab verankerte Manifest gesetzt, keine weiteren Workflowaenderungen. Eigene fachliche Lieferfreezeannahme und Status im WP/Evidenzpfad dokumentieren; formaler WPabschluss mit gemeinsamer CURRENT_STATE-Nachfuehrung spaeter durch R im eigenen prospektiven I-Scope. Kein Kindmerge oder kumulativer Scopeclaim.

## Betroffene Dateien/Module
Abschliessend nur die folgenden neu anzulegenden Pfade; .github/workflows/validate.yml ist bestehend und nur der Selektor darf geaendert werden.

- `WORK_PACKAGES/WP-042_Lokaler-Save-Restore-Grundfortschritt.md`
- `tools/architecture-validation/scopes/WP-042.production.scope.json`
- `.github/workflows/validate.yml`
- `Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain/PuzzleSessionCheckpoint.cs`
- `Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain/PuzzleSessionCheckpoint.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Application/LocalProfileState.cs`
- `Assets/StammstreckenPuzzle/Scripts/Application/LocalProfileState.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Application/LocalProgressPolicy.cs`
- `Assets/StammstreckenPuzzle/Scripts/Application/LocalProgressPolicy.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Application/PersistedPuzzleSession.cs`
- `Assets/StammstreckenPuzzle/Scripts/Application/PersistedPuzzleSession.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Application/AutosaveCoordinator.cs`
- `Assets/StammstreckenPuzzle/Scripts/Application/AutosaveCoordinator.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/LocalSaveCodec.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/LocalSaveCodec.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/AtomicSaveRepository.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/AtomicSaveRepository.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveMigration.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveMigration.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveLedgerCodec.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveLedgerCodec.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveFileSystem.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveFileSystem.cs.meta`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/ReviewProgressCatalog.cs`
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/ReviewProgressCatalog.cs.meta`
- `Assets/StammstreckenPuzzle/UI/ReviewCampaign.catalog.json`
- `Assets/StammstreckenPuzzle/UI/ReviewCampaign.catalog.json.meta`
- `Assets/StammstreckenPuzzle/UI/ReviewCompletion.catalog.json`
- `Assets/StammstreckenPuzzle/UI/ReviewCompletion.catalog.json.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Domain/PuzzleSessionCheckpointTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Domain/PuzzleSessionCheckpointTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalProfileStateTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalProfileStateTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalProgressPolicyTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalProgressPolicyTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/PersistedPuzzleSessionTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/PersistedPuzzleSessionTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/AutosaveCoordinatorTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/AutosaveCoordinatorTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/LocalSaveCodecTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/LocalSaveCodecTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/AtomicSaveRepositoryTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/AtomicSaveRepositoryTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/SaveMigrationTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/SaveMigrationTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/SaveLedgerCodecTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/SaveLedgerCodecTests.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalSaveContractFixtures.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/LocalSaveContractFixtures.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/LocalSaveContractFixtures.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/LocalSaveContractFixtures.cs.meta`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/ReviewProgressCatalogTests.cs`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/ReviewProgressCatalogTests.cs.meta`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/S/*.md`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/S/*.json`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/S/*.txt`
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/S/*.png`

## Ausdrücklich nicht erlaubte Änderungen
Keine alten Manifeste, T-/anderesKind-/gemeinsamen Registry-/Status-/Domain-/Solver-/JCS-/Textquellen ausser den oben ausdruecklich NEU benannten eigenen Dateien. Kein Main-/Produkt-/Release-/WP026-Gesamt-/WP035-/finalerGestaltungspass, keine Authoringloesungen/Proofs/Notizen im Player, keine neuen Sternzeiten/Preise/480Assets oder Hint/Daily/Generator/Kauf/Ads/IAP/Analytics/Cloudaktivierung. Keine Assemblykanten-/Budget-/Gate-/Runner-/Lizenzabsenkung. Keine erneute allgemeine Solver-/TextQC.

## Akzeptanzkriterien
| AK | Erforderlicher konkreter Nachweis |
|---|---|
| S01 Quelle/Scope | Eigener Erstadd/Parent/Base exakt abgeschlossenes T, unveränderte Manifestbytes, keine A-/T-/Fremdpfade; reale Git-/Blobbindungen. |
| S02 Checkpoint | Alle Felder roundtrip, Update nach Restore mit PreviouslySet/Correction und Dedup; Undo256/Rückwärtsreplay; 2×2/4×4/10×10; falsche Identität/Ruleset/Guid/Enum/Länge/Counter/Safeinteger/Phase/Timer/solved-ID und inkompatible Diffs scheitern. |
| S03 Codec | Vollständiger v2-Roundtrip und defensive Copies; payload-only JCS/Nodebytegleichheit, Unicode/Surrogate/Float/duplicate/unknown/envelope/payload/range/limit/truncation/badhash; unbekannte Profile/neueres Schema bleiben als Unsupported erhalten. |
| S04 Migration | Echte eigene vollständige v1→v2-Goldens plus vorhandene Teilvektoren getrennt; historische Lock-/Dokumenthash-/Claim-/Prefix-/Cosmeticsbindungen, fehlende Bindung/Resume-/Pflichtwertstopps, unveränderte Source-/migration-source-Dateibytes. |
| S05 Writer | Temporärer echter Dateibaum und injizierte Abbrüche nach sämtlichen9Kernschritten **plus allen vorbereitenden Copy-/Open-/Flush-/Reread-/Backuprotationsfenstern**. Vor jeder destruktiven Pendingöffnung muss höchste gültige Wahrheit zusätzlich regulär in Main/Backup vollständig verifiziert sein; Pending-only-Vorlage und jeder Sicherungsfehler werden mit neuem normalem Load geprüft, Quarantäne zählt nicht als einzige Rückfallkopie. Gilt normale Writes/Migration/Recoverypromotion ohne automatische SplitBrain-/Unsupportedreparatur. Außerdem gleiche-hash-Ties, Pending/Main/Backupabweichungen, Lowdisk/Permission, Flush-/Replacefallback und Crash vor Callback. |
| S06 CAS/Cancel | Zwei Repositoryinstanzen, parallele Commands, nur ein CASgewinner, tatsächlicher frischer Writerstand; Cancellation vor/nach erster Wirkung korrekt, keine falsche sichere Rücknahme; erwartete/angebotene Generationen und Safeintegerende prüfen. |
| S07 Recovery | Drei Missing ohne Dateierzeugung; Corrupt/Unsupported/Failed getrennt; read-only Recover mit vollständigem Sourcehashvergleich. Änderung auch eines nicht ausgewählten Kandidaten, gleicher Payloadhash bei anderem Envelope, neuerer Main, verschwundener/entstandener Pending, Catalog/Profilewechsel → Conflict. Dateibytes vor/nach Recover identisch. |
| S08 Fortschritt | Gelockte ausschließlich testinterne Contractkataloge; Erstclear/Nullzeiten, beide Marker, Practice-vor-Season, Revision-nach-Season, Sterne nur verbessern, Sternnachzahlung/Route/Section genau einmal, unveränderte terminale Bindungen nach Restart/Compaction; Identitäts-/Katalogabweichungen und gleich-ID-anderer-Inhalt sperren. |
| S09 Ledger | Exakte Entry-/Ketten-/Checkpointbytes, Sequenz/Summe/Collisionnegativfälle, 383/384/511/512/513 und 192/256-KiB-Grenzen einschließlich Bytegrenzenmutationen, 64-Diagnosetail ohne Doppelzählung, offene/nonterminale Deduplizierungswahrheit blockiert statt gelöscht. |
| S10 Erhalt/Isolation | Jede deaktivierte gespeicherte Variante vor/nach Save/Migration unverändert semantisch erhalten und invalides Variantpayload abweisen; keine aktivierten Operationscommands. QA/Product/ContractTest sowie Cataloghash-/Namespace-/Roottraversalgrenzen nachweisen. |
| S11 Use Case/Autosave | Kontrollierte Tasks zeigen keinen Ride-/Result-/Rewarderfolg vor Commit; Fake-Commitfehler/Recovery/Retry/Crashcallback; 499/500/1999/2000-ms-Grenzen, dauernde Mutationen, ältere Response/neue Mutation, Autosave-versus-Completion, Lifecycleflush und Ausschluss von Hintergrundzeit/Render-I/O. |
| S12 Abschluss | Passende lizenzfreie Compile-/kanonische Architecture-/Scope-Self-/Negativchecks, vollständige reguläre eigene CI am echten Freeze mit originalen XML/Logs/Checkoutbindungen und unabhängige technische Freezeprüfung ohne offene Blocker/Highs; eigene Übergabe mit tatsächlichen Einschränkungen. |

## Tests
Vor Code die oben gebundenen konkreten AK-/Testmatrix und vollstaendigen eigenen Testpfade abgleichen. Alle vorgesehenen Positiv-/Negativ-/Regressions-/Grenzfaelle tatsaechlich ausfuehren; urspruengliche Fehler erhalten. Lizenzfreies Roslyn/Mono-Supplement nur als solches kennzeichnen. Kanonische CPython3.11.13/Node22.20.0 Architecture-/eigenerScope mit Selftests, tatsaechliche vollstaendige regulaere CI mit Original-XML/Logs/Checkout/Tree und unabhängige Freezepruefung. Native Unitylaeufe/Pushes nur durch R koordiniert nach belegter freier Lizenz; nie zwei gleichzeitige Instanzen/Ketten. Physische Geraete-/Store-/Human-/Produktgates bleiben separat.

## Risikoklasse
Hoch: persistente Save-/Recovery-/Economy-/Migrationswahrheit. Begrenzung durch angenommenes T, eigenen prospektiven Scope und getrennte Freezepruefung.

## Definition of Done
Alle AK tatsaechlich erfuellt, eigene Source-/Originaltest-/CI-/Prueferbindung und bedienbare Uebergabe im eigenen Evidenzpfad. LA/LS darf als fachlicher Lieferfreeze angenommen werden; formaler WPabschluss einschliesslich CURRENT_STATE wird erst durch R inI dokumentiert. Gesamtauftrag bleibt bis P/I/unterintegrierter CI/Kontrolle offen.

## Bearbeitungsstatus
Prospektiv vor Facharbeit verankert; keine Implementierung oder AK-PASS behauptet.

## Vorcode-Anschlussentscheidung der technischen Rolle R
Die bestehende T-API bleibt unveraendert. Nach Commit-Conflict bleibt die Application gesperrt und muss zwingend einen frischen LoadAsync mit vollstaendiger Drei-Kandidaten-Observation ausfuehren. Diese Observation gilt zum Loadzeitpunkt, nicht atomar zum frueheren Commit. Fehler/Cancel/unvollstaendige Observation belassen die Sperre; kein altes Lease, Cachefallback oder blindes Recommit. Jeder spaetere Write prueft erneut CAS und alle Kandidatenbytes. Diese von S-Implementierung getrennt gepruefte technische Portausfuehrung konkretisiert den vorhandenen funktionalen Anschluss ohne T-/Produkt-/Planmutation. Die Race-/Fehlerfaelle sind zusaetzliche verbindliche S-Tests.
```json
{
  "role": "R technical execution acceptance within MANUS-2026-10-19-01 section 5; no product or T-source change",
  "date": "2026-10-19",
  "independentReviewer": "/root/t_boundary_review",
  "implementationOwner": "/root/t_contracts_implementation",
  "verdict": "ACCEPTED_TWO_STEP_EXISTING_PORT_EXECUTION_BEFORE_S_CODE",
  "review": "CommitResult does not carry Observation. Conflict invalidates the in-memory recovery lease and leaves the Application use case blocked. The use case must perform a fresh LoadAsync and obtain its full three-candidate Observation before exposing a new recovery basis. This observation is bound to Load time, never represented as atomically returned under the previous Commit lock. Subsequent effects independently recheck CAS and all candidate source bytes.",
  "mandatoryTests": [
    "Candidate changed between Commit Conflict and Load appears in the fresh Load observation",
    "Failed or cancelled Load, or absent complete observation, preserves the mutation, autosave, completion, ride and success block",
    "No old lease, cached observation or blind recommit is accepted; a fresh explicit recovery choice is required"
  ],
  "scope": "Persist in S own WP/test/evidence scope after its prospective anchor. Existing T API, accepted source plans and manifest bytes remain unchanged. This is not implementation PASS."
}
```
