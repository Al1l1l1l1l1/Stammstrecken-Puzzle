# WP-043 – Public240 gespeicherte Integration (I)

## Aktueller Abschlussstatus

Technisch ABGESCHLOSSEN und unabhängig angenommen; tatsächlich in WP036 unterintegriert. Maßgeblich sind die untenstehende Abschlussbindung und die Gesamtübergabe. Die folgenden Zwischenstände bleiben historisch erhalten.

## ID
`WP-043`

## Ziel
Die tatsächlich angenommenen getrennten A-/S-Lieferungen auf ihrer exakt nachgewiesenen isolierten Montage P zu einem bedienbaren gespeicherten internen Public240-Prüfablauf verbinden, vollständig prüfen und nach gesonderter zuständiger R-Entscheidung ausschließlich in den gebundenen WP036-Developmentzweig unterintegrieren. Eine maßgebliche persistente Übergabe erschließt Ergebnis, Startweg, Quellen, Entscheidungen, Originalnachweise und verbleibende Grenzen.

## Voraussetzungen
AGENTS vollständig mit Konzept15/00/09, CURRENT_STATE und diesem WP lesen. Vollständiges MANUS-2026-10-19-01 insbesondere §§4–7, WORK_QUEUE, WORK_PACKAGE_RULES, AI_HANDOVER_RULES, DEFINITION_OF_DONE, CONTENT_PIPELINE, CONTENT_CATALOGS, PERSISTENCE, GAME_STATE_MODEL, MODULE_BOUNDARIES, BUILD_AND_RELEASE, TEST_STRATEGY, OPEN_BLOCKERS sowie ADR-Verfahren und angenommene ADR034/035 lesen. Keine neue allgemeine Solver-/Text-/Bestands-QC.

Veröffentlichte Entscheidungsbasis B3d1ac59c3f70d50e05434a54d3ddec5d282bb0c3 und abgeschlossene gemeinsame Basis D925ca608a961ac26c84385c8da3b949f609df883. Exakt angenommene LA=`7d2e99837cd3910d11ed5f01ea67e8d06acba2c4`, LS=`3746184ca75eb439de918896e9b88aa238cff893`. Tatsächliche isolierte Montage P=`95508482696cc1cc4e355021ce47d64863d9651a`, Tree=`f0d7306e8b907dfa60661bb8703ce970b3049200`, geordnete Eltern exakt LA/LS. Provenienz SHA256=`0b134f82424c9cf1d20d5fa3f829f09a21f20edfd3a7cb94796e496dfc2352bf`; der einzige tatsächliche Konflikt betrifft validate.yml und ist exakt mit dem LA-Blob aufgelöst. Alle übrigen P-Modi/Blobs entsprechen der angenommenen A-/S-/D-Union. Der tatsächliche kumulative Kindscope-FAIL bleibt erhalten, kein Einmanifest-Gesamt-PASS.

Dieser WP-/[Manifest-Erstadd](../tools/architecture-validation/scopes/WP-043.production.scope.json) hat gemeinsam und ausschließlich den Parent/Base P. Danach bleiben alle Manifestbytes unverändert. Frische globale WP-/ADR-IDprüfung SHA256=`62c4629dfda667d4940edb533f8f1074d695a5fcaa57a521bbf1f16873ecedc2`; frühere IDs/Anker werden nicht wiederverwendet. Das eigentliche nummerierte ADR-036 ist im Folgenden ausdrücklich noch Vorcodevoraussetzung, keine bereits angenommene Entscheidung dieses Ankers.

Die vollständigen LA-/LS-Annahmebindungen und Rohprovenienzen werden nach diesem gemeinsamen Erstadd bytegebunden ausschließlich im eigenen I-Evidenzpfad übernommen. Die technischen A-/S-Fertigstellungen bleiben an ihren eigenen Freezes und CI-Läufen gebunden; gemeinsame formale Statusnachführung gehört diesem I-Scope.

## Scope
Vor jeglichem I-Fachcode den bereits getrennt vorgeprüften, eng auf SavedPublic240Review begrenzten Architekturentwurf als neue Datei `DECISIONS/ADR-036-gespeicherter-public240-qa-entry.md` tatsächlich anlegen, getrennt an seinem nummerierten Wortlaut prüfen lassen und durch die mandatierte technische Rolle R dokumentiert annehmen. Die bisher auf Public240Review begrenzte Inspector-/In-Memory-Localeausnahme wird nicht still ausgeweitet. Nur nach dieser Annahme entstehen neue Saved-Bridge, Platformclock/-dispatcher, Bootstrapcomposition/-installer, UI-Rahmen, deterministischer Builder, neue Szene/Locale und ihre konkreten Tests. Bestehende A-/S-Fachdateien bleiben unverändert; bestehende Scene-/Domain-/JCS-/Providerautorität wird wiederverwendet.

Genau die14 nachfolgend neu benannten Dateien mit ihren14Metas und die ausdrücklich gelisteten bestehenden Anschluss-/Governance-/Statusdateien bearbeiten. Neuer normaler QA-Entry ist SavedPublic240Review, danach getrennte Public240Review/DevelopmentPuzzle/BootstrapComposition. Der bestehende DevelopmentPuzzleSceneBuildCheck wird nur um Saved ergänzt; A behält seinen unveränderten Public240ReviewSceneBuildCheck. Kein zweiter neuer Saved-Prozessguard. validate.yml ändert nur den Manifestselektor; unity.yml bleibt unverändert.

Die Validatoränderung ist ausschließlich die neue geschlossene ADR-036-Status-/Range-/Summen-/Followupregistrierung und passende fehlend/falsch-Status-Negativtests. Bestehende Registrywerte, Scope-/Hash-/Gate-/Runner-/Lizenz-/Timeout-/Budgetregeln bleiben unverändert. Fehlender Nachfolger-Rücklink wird zusätzlich durch tatsächliche isolierte Mutation gegen dieselbe adr_check()-Implementierung geprüft; kein allgemeines dynamisches Registry- oder Summenscopefeature.

Runtime: Clock, Dispatcher und Saved-Bridge auf Unitymain; dazwischen reine S-/Katalog-/Persistencecomposition auf Worker aus zuvor kopierten Unitybytes/Diagnosen. Sichtbares lokalisiertes Startup bereits ohne Bridge, Hauptthreadpump auch bei null Modell, einmaliges BindModel, Epoch-/Disposebindung und beobachtete Tasks. Dropdown ist flüchtige Auswahl; ausschließlich ausdrücklicher Neuanfang oder Fortsetzen startet eine Session. HasDraftForPuzzle prüft das vollständige öffentliche Identitytripel im bestätigten Snapshot. Jeder PuzzleView und Fahrtcallback ist an eine immutable Attempt/Identity gebunden. Keine automatische Retryschleife nach Fehler.

Echter RuntimeRoot ist Application.persistentDataPath, QA-ProfilId r; keine doppelten Namespace-/Hashsegmente oder Ersatzwurzel. EditorbuildId editor-public240-review; Player tatsächlicher Build-GUID. Konkrete Windows-Pfadlängen aller tatsächlichen Save-/Recovery-/Migrations-/Stagingnamen vor Dateiwirkungen prüfen; keine pauschale Windowsgrenze für Unix erfinden. Testrootscope nur UNITY_EDITOR && UNITY_INCLUDE_TESTS, vor dem tatsächlichen Awake, einzeln/nicht verschachtelt/pro Instanz erfasst, unter OS-Temp oder STP_TEST_TEMP_ROOT und immer vollständig disjunkt vom echten persistentDataPath. Echte Kataloge/Codec/Repository bleiben bestehen. Keine Tests am echten AppData-Profilbaum.

Nach tatsächlichem unabhängigem I-PASS und vollständiger regulärer CI am exakten finalen Freeze trifft R eine gesonderte dokumentierte Nicht-main-Unterintegrationsentscheidung, sichert Expected-HEAD frisch und führt nur den zulässigen WP036-Untermerge aus. Anschließend tatsächlichen integrierten Tree/Diff, historische Scopeketten, reguläre integrierte CI und getrennte Integrationskontrolle nachweisen. Abschlussnachweise und Dokumentationsnachläufe erhalten ihre eigenen tatsächlichen Standbindungen; keine laufende CI als PASS.

## Betroffene Dateien/Module
Abschließende Pfadliste, ausschließlich die als neu bezeichneten Dateien dürfen neu entstehen. Einzige Ausführungsbasis ist das unveränderliche eigene Manifest, kein kumulativer Kindscope.

- `WORK_PACKAGES/WP-043_Public240-gespeicherte-Integration.md` — NEU anzulegen
- `tools/architecture-validation/scopes/WP-043.production.scope.json` — NEU anzulegen
- `DECISIONS/ADR-036-gespeicherter-public240-qa-entry.md` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Application/SavedPublic240ReviewSession.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Application/SavedPublic240ReviewSession.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Platform/LocalSaveRuntime.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Platform/LocalSaveRuntime.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240Composition.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240Composition.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/SavedPublic240ReviewPresenter.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/SavedPublic240ReviewPresenter.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/SavedPublic240ReviewBuild.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/SavedPublic240ReviewBuild.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uxml` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uxml.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uss` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uss.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240ReviewLocale.json` — NEU anzulegen
- `Assets/StammstreckenPuzzle/UI/SavedPublic240ReviewLocale.json.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/SavedPublic240ReviewSessionTests.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/EditMode/Application/SavedPublic240ReviewSessionTests.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/SavedPublic240ReviewPresenterTests.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Presentation/SavedPublic240ReviewPresenterTests.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240ReviewSmokeTests.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240ReviewSmokeTests.cs.meta` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240BuildEntryTests.cs` — NEU anzulegen
- `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/SavedPublic240BuildEntryTests.cs.meta` — NEU anzulegen
- `.github/workflows/validate.yml` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/StpBuildEntrypoints.cs` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `Assets/StammstreckenPuzzle/Scripts/Editor/Build/DevelopmentPuzzleBuild.cs` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `ProjectSettings/EditorBuildSettings.asset` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `DECISIONS/README.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `DECISIONS/ADR-034-isolierter-public240-reviewpfad.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `tools/architecture-validation/validate.py` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `ARCHITECTURE/MODULE_BOUNDARIES.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `ARCHITECTURE/BUILD_AND_RELEASE.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `PROJECT_CONTROL/CURRENT_STATE.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `PROJECT_CONTROL/WORK_QUEUE.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/HANDOVER.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `WORK_PACKAGES/WP-040_Public240-Save-Technikbasis.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `WORK_PACKAGES/WP-041_Public240-interne-Prueffassung.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `WORK_PACKAGES/WP-042_Lokaler-Save-Restore-Grundfortschritt.md` — bestehend; ausschließlich der oben konkret begrenzte Anschluss/Nachlauf
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/I/*.md` — NEUE eigene Evidenzdateien; nur diese Typen
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/I/*.json` — NEUE eigene Evidenzdateien; nur diese Typen
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/I/*.txt` — NEUE eigene Evidenzdateien; nur diese Typen
- `PROJECT_CONTROL/ProductionBlocks/Public240Save/I/*.png` — NEUE eigene Evidenzdateien; nur diese Typen

Dateieigentümer vor Fachcode: Root implementiert neue Composition/Installer/Buildentrytests samt Metas, bestehende technische Build-/Settingsanschlüsse, Architektur-/Scope-/Status-/Evidenzführung und zuständige R-Entscheidungen. `/root/t_contracts_implementation` implementiert ausschließlich neue SavedPublic240ReviewSession, LocalSaveRuntime und Applicationtests samt Metas. `/root/t_boundary_review` implementiert ausschließlich neuen SavedPresenter/Builder/UIassets/Szene und Presentation-/Smoke-Tests samt Metas. Root legt alle freien GUIDs vor den Quellen fest. `/root/entry_audit` ist von allen Implementierungen getrennt und prüft Entscheidungen, Sourcefreeze, Originale und Integration unabhängig. Kein Unteragent startet Unity, pusht oder disponiert den Seat.

## Ausdrücklich nicht erlaubte Änderungen
Keine alten Manifestbytes oder A-/S-Fachquellen/-tests/-assets, kein ADR034/035-Fachkörper (034 ausschließlich additiver Nachfolgerverweis), keine neue Assemblykante/asmdef/csc.rsp, kein unity.yml-/Gate-/Runner-/Lizenz-/Timeout-/Budgetdelta. Keine allgemeine Validator-/Summenscopefunktion. Keine Solver-/Authoring-/Produkttextproduktion oder versteckten Lösungen/Proofs/Notizen in Playerassets. Keine Produkt-/Main-/Release-/WP026-Gesamt-/WP035-/finale Gestaltungsannahme, keine480Assetproduktion, neuen Sternzeiten/Preise/UX-Endwerte oder Hint/Daily/Generator/Kauf/Ads/IAP/Analytics/Cloud-/Provideraktivierung. Menschliche240Tests/Zeitkalibrierung sowie physische Geräte-/Store-/Signingfreigaben werden nicht simuliert oder als bestanden ausgegeben.

## Akzeptanzkriterien
| AK | Vor Code festgelegter tatsächlicher Nachweis |
|---|---|
| I01 Herkunft/Scope | B/D/LA/LS/P exakt, angenommene eigene CI/QC und geordnete P-Eltern/Union/ursprüngliche Konfliktblobs/LA-Wahl; erwarteter kumulativer KindFAIL erhalten; gemeinsamer eigener Erstadd mit soleParent P und unveränderten Manifestbytes. |
| I02 Szenenentscheidung | Neuer nummerierter ADR-036-Entwurf → getrennte Review → zuständige dokumentierte R-Annahme vor Fachcode; ausschließlich geschlossene Index-/Registry-/Nachfolger-/Navigationsfortschreibung und passende positive/negative Prüfungen. |
| I03 echte Composition/Isolation | Exakte A-Publicbytes/all240/gebundener Publichash plus echte S-Campaign-/Completion-/Aggregate-/QA-Profilbindung; Mutationen, falsche Profile, Traversal/Reparse und ungültige Root-/Datenbindung stoppen sichtbar ohne Ersatzprofil. Tatsächlicher RuntimeRoot und isolierte Testrootgrenze nachgewiesen. |
| I04 normaler Entry | Saved als erster normaler Editor-/Android-/iOS-QA-Entry, weitere drei getrennte Szenen; richtige explizite Installer und Referenzen, QA-Identifier fail-closed, iOS BuildOptions.None kein Produktionspass. Tatsächliche BuildReports aller vier Szenen in beiden Plattformjobs. Deterministischer Builder erzeugt nur eigene neue Artefakte; Reproduktion verändert A-/Developmentassets nicht. |
| I05 Main/Worker/Startup | Clock/Dispatcher/Bridge auf Main, echte IO/JCS-/Hashcomposition auf Worker; verzögerte Antworten erst nach Drain; Workerdrain abgewiesen; kein Render-I/O. Lokalisierte Loading-/Fehleransicht ohne Modell, einmaliges BindModel, nullModelPump, alte Epoch-/Disposeantworten wirkungslos; kein Taskcompletion-als-Erfolg. |
| I06 Initialisierung | Drei echte Missing erzeugen sichtbar NeedsInitialization und keine Profildatei; erst ausdrückliche Createaktion bestätigt Generation1. Corrupt/Unsupported/Failed/SplitBrain werden weder automatisch initialisiert noch verdeckt. |
| I07 Auswahl/Views | Tatsächlich bedienbare240Auswahl mit4×4/10×10, korrekten Randzahlen/Endpoints/Tools/Batch/Undo. Dropdown allein wirkt nicht auf Session/Dateien; expliziter Neu-/Resumeweg und volle Draftidentity. Wechsel/Restart warten echten Flush ab; Fehler behält Wahrheit. Alte Viewcommands/Capture/Undo/Ridecallbacks können keine neue Attempt/Dimension erfassen. |
| I08 echter Restore | Reale UI-/Datei-/Coldloadprobe mit Marker→Leeren→Marker→Undo→Pause/Flush und neuem Installer. Zellen/PreviouslySet/stickyFlags/Correction/Revision/Dedup/Undo bleiben erhalten; neue Monotonie ohne Auszeitzuschlag. Keine zweite Checkpointautorität. |
| I09 Lifecycle/Autosave | Neue Bridge meldet tatsächliche Mutationen korrekt, keine bloße Tool-/Auswahländerung; vorhandene S-Debounce-/2s-Verträge bleiben nachgewiesen. Sofortiger Pause-/Fokusflush, OR-Suspension/keine Hintergrundzeit, alte Writeantwort/neue Mutation und Completion versus Draftwrite. Keine unobserved Tasks oder Frame-IO; Dispose beendet Publikationen kontrolliert. |
| I10 Commit→Ride→Results | Tatsächliche Spielercommands lösen; echter SaveFS-Ownerlock hält ACK bei responsivem Main, keine Fahrt/Results/Rewarderfolg oder Eingaben davor. Nach verifiziertem Mainreread ausschließlich eigene gebundene Spielerstrecke; genau eigene Attempt darf Fahrt/Result öffnen. Native4×4/10×10/Fahrt/Result selbst betrachten und Originalscreens binden. |
| I11 Savefehler/Retry | Tatsächliches Dateisystemhindernis stoppt den echten Writer, gelöste Session und alte durable Wahrheit bleiben; keine falsche Fahrt. Bewusster Retry nach Beseitigung zahlt nur einmal. Ungewisser/Conflict-Ausgang verlangt frischen Load statt altem Lease; S-Crash-/Lowdisk-/CASfensterbelege unverändert wiederverwenden. Unerwarteter gebundener Requestfault sperrt Spiel, erhält erreichbaren äußeren expliziten Reload ohne Schleife. |
| I12 Recovery/Reset | Drei echte Kandidaten einschließlich SplitBrain; Auswahl/Anzeigen/Abbrechen sind ohne Dateiwirkung. Bewusste bestätigte Auswahl führt read-only Recover→exakte Promotion, vollständige aktuelle Observation und Sourceänderungen werden berücksichtigt. Unsupported/Failed weder umgehen noch resetten. CorruptReset separat sichtbar bestätigen, Originale/Generation erhalten, alte Callbacks wirkungslos. |
| I13 Restart/Fortschritt | Bestätigter neuer Attempt/Readycheckpoint, Time0 bis erster Input; fehlgeschlagener Neustartwrite sichtbar ungesichert bei erhaltener alter durable Payload. Echter FirstClear bei nullZeiten1Stern/15QA-Geduld, Markerstraffrei, keine Doppelzahlung nach Restart/Restore. Bestätigte QA-Balance/Sterne/Claims ausschließlich aus S-Snapshot; weitergehende Policy-/Ledgerfälle sind S-Bestand, keine neue Produktregel. |
| I14 UI/Leakgrenze | Savedstatus sichtbar auch ohne Domainrevision, alle eigenen Keys/Argumenttypen und Startup-/Recovery-/Bestätigungscontrols, keine rohen Keys oder unbedienbaren abgeschnittenen Controls. Fehlende Workercomposition sichtbar, keine Fake-Session. Unveränderte A-Publicprojektion und tatsächlicher Scene-/Playerdependencygraph enthalten kein internes Lösungswissen/Testfixturepayload. |
| I15 Freeze/CI/QC | Passende lizenzfreie Targetcompiles ausdrücklich Supplements; echte kanonische17/18- und Self-/Negativchecks nach vollständigem Staging. Vollständige reguläre gemeinsame CI am exakten Freeze, OriginalXML/Logs/Coverage/Checkout/Tree/BuildReport/Seatfreigaben gebunden. Getrennte Source-/Original-/AKprüfung ohne offene Highs/Blocker; ursprüngliche FAILs und ihre Grenzen erhalten. |
| I16 Unterintegration/Handover | Gesonderte R-Entscheidung erst nach I-PASS+CI; frisch erwartungsgebundener Merge nur in WP036, actualTree/Diff/Scopeketten und integrierte CI/getrennte Kontrolle. Formale T/A/S/I-WP-, CURRENT_STATE-/Queue-Nachführung und eine maßgebliche Handoverfundstelle mit tatsächlichem Start-/Installationsweg, allen Commit-/PR-/CIbindungen, offenen Produkt-/Human-/Gerätegrenzen und nächstem gültigem Schritt. Dokumentationsnachläufe separat ehrlich gebunden. |

## Tests
Vor Code die obigen AK und die ursprünglichen App-/UI-Pläne samt konkreten R-Auflösungen und Startup-/Auswahladdenda vollständig abgleichen. Die vier neu genannten Testdateien tragen ausschließlich die jeweils neuen Bridge-/Platform-/Startup-/UI-/SavedScene-/Entry-/Recovery-/Failure-/ACK-/Lifecycle-/Callbacknachweise. Bestehende Domain/Solver/JCS/A/S-Nachweise wiederverwenden und unverändert in regulärer CI mitführen; keine neue allgemeine Bestandsprüfung. ACK-Halteprobe benutzt echten OSlock mit gleichthreadigem Acquire/Dispose auf einem dedizierten Testownerthread; Unitymain bleibt frei. Alle Testrootscopes vor tatsächlichem Awake/LoadScene, vollständiges Teardown/ausstehende Arbeit vor Cleanup.

Lizenzfreie frische Roslyn-/Mono-Compiles/gezielte NUnitfälle sind nur Supplements; portable PDBs/RSPs und vollständige Quellbindungen erhalten. Native gepinnte Unity6000.3.23f1-Läufe für Builder/Reproduktion, Saveddateiflow und tatsächlich selbst betrachtete Grafikbilder. Jede ursprüngliche rote Probe behalten, Korrektur gezielt nachprüfen. Kanonische CPython3.11.13/Node22.20.0 Architecture/Production-Scope samt Self-/Negativtests; vollständige reguläre Unity-CI mit Original-XML/Logs/Coverage, Android ARM64 IL2CPP und iOS/Xcode am echten finalen Stand. Danach neue tatsächliche integrierte CI und getrennte Integrationskontrolle. R koordiniert alle Push-/Unityaktionen seriell nach tatsächlichen Seatfreigaben; keine Doppelinstanzen oder willkürliche Grün-Retries.

## Risikoklasse
Hoch: reale Persistenz-/Recovery-/Fortschrittswahrheit und Commit-vor-Erfolgsfreigabe im bedienbaren Ablauf. Begrenzung durch unveränderte angenommene A/S/T-Verträge, prospektiven eigenen Scope, neue enge Szenenentscheidung vor Code, echte Dateisystem-/UI-/Buildtests und getrennte unabhängige Freeze-/Integrationsrollen.

## Definition of Done
Alle I01–I16 tatsächlich erfüllt und einzeln mit Originalen/Standbindungen nachvollziehbar. Keine offene mandatierte Restimplementierung, kein vorweggenommener CI-/Integrations-PASS. Gemeinsame CURRENT_STATE/QUEUE/WPs und HANDOVER entsprechen dem tatsächlich bedienbaren integrierten Nicht-main-Stand und erklären jeden separaten Dokumentationsnachlauf. Der neue Agent kann aus Repository und einer maßgeblichen Handoverfundstelle übernehmen; keine manuelle Teildokumentübergabe. Produkt-/Main-/Release-/Human-/Zeitkalibrierungs-/Geräte-/Storegates bleiben ausdrücklich getrennt.

## Bearbeitungsstatus
Prospektiv gemeinsam auf P verankert. ADR-036 wurde als eigener nummerierter Entwurf historisch angelegt, getrennt geprüft und durch R am 2026-10-19T13:13:31.376Z vor allen14 I-Fachdateien/Metas zuständig angenommen; Originalbindungen siehe eigener I-Evidenzpfad. Damit ist das enge Vorcodegate erfüllt. Implementierung, tatsächliche Tests, I-CI und Unterintegration sind noch offen.

## Tatsächlicher Implementierungs- und Nachprüfstand

Die vorstehende Statuszeile ist der erhaltene Vorcodestand. Alle14 I-Dateien/Metas und die vier eng begrenzten bestehenden technischen Anschlüsse sind inzwischen implementiert. App-V3: frischer Compile und28/28 gezielte Mono-Fälle; Rootaliasgrenze tatsächlich18/18 und getrennt geprüft. Vollständige unabhängige neue Sourceprüfung, konkrete Locale-/Stale-/Fixturekorrekturen und weitere Native04-Testdelta-QC liegen bytegebunden im eigenen I-Pfad; keine A-/S-Fachquelle oder alter Manifestbyte geändert. Kanonische17/18 samt Selftests wurden vollständig gestagt am noch nicht eingefrorenen WIP ausgeführt, nicht als finaler Freezeclaim.

T/A/S sind nach eigener tatsächlicher regulärer CI/QC technisch angenommen und in diesem I-Scope formal nachgeführt. Native03 geordnete eigene Szenenreproduktion OS0. Native04:29/44, originale drei Testfehler erhalten und eng korrigiert. Native05:40/44, vier Recoveryfixtures und selbst betrachteter echter10×10-Randzahlen-Layoutbefund werden gezielt behoben. Alle vier tatsächlichen Originalbilder, XML/Logs/Receipts und unabhängigen Kausalurteile bleiben unverändert erhalten. Noch kein I01–I16-Gesamt-PASS, finaler Freeze, reguläre I-CI oder autoritativer Untermerge. Maßgebliche zusammenhängende Ergebnis-/Start-/Restgateführung: [HANDOVER](../PROJECT_CONTROL/ProductionBlocks/Public240Save/HANDOVER.md).

## Native08 – tatsächlicher korrigierter Prüfstand

Die im vorstehenden Zwischenstand noch offenen Recoveryfixture-/Layoutbefunde sind durch gezielte Korrekturen und tatsächliche Native08-Nachprüfung erledigt:45/45 PASS0Fail/Skip, OSexit0. Vier originale586×543-Grafikbilder durch Root tatsächlich betrachtet;10×10-Randzahlen lesbar, keine seitliche Textbeschneidung, alle sichtbaren Contentcontrols im echten kleinen Viewport per ScrollTo/Bounds geprüft. Aktuelle Quellen unverändert zur08-Inputbindung497Gitblobs. [Vollständige Originale](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_NATIVE_08_PASSED_ORIGINALS.json), [Root-Ergebnis/Bildbindung](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_NATIVE_08_ROOT_RESULT_VISUAL.json), [AK-/Restgatematrix](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_LOCAL_ACCEPTANCE_MATRIX.md). Finale unabhängige Nativeannahme, sauberer Freeze/kanonische Prüfungen, reguläre I-CI sowie I16-Unterintegration bleiben offen; kein vollständiger I-Abschluss.

## Zusätzlicher enger Source-Nachlauf I-ROOT-002 / Native09

Vor dem Freeze wurde I-ROOT-002 zusätzlich konkret erkannt: Unix verglich einen anders benannten Realroot-Symlink nur lexikalisch. Der alte tatsächliche Guard akzeptierte den unsicheren eigenen synthetischen Alias; korrigiert wurden ausschließlich Installer-Testguard und Roottest. Unix weist jetzt Reparse-/Nichtverzeichnisgrenzen einschließlich aller Real-/Temp-/Env-/Kandidatvorfahren vor Provisionierung ab. Verlinkte Unix-Testumgebungen werden ausdrücklich fail-closed angehalten; keine Ersatzwurzel und keine darüber hinausgehende Aliasgarantie. Tatsächliche reine Mono-Metadatenprobe20/20 einschließlich dangling/ancestor/missing/file/double-slash; Windowsregression18/18 einschließlich8.3/SUBST. Erster fehlenderFacade-Lauf und Sandbox-SUBST-Nichtausführung bleiben erhalten. [Original/enger tatsächlicher Korrekturtransport](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_ROOT_002_ACTUAL_CORRECTION.json). Native09 läuft auf genau den zwei korrigierten Quellen vollständig45/45 PASS0Fail/Skip mit OS0; vier neue Originalbilder durch Root selbst betrachtet. [Aktuelle vollständige Nativeoriginale](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_NATIVE_09_PASSED_ORIGINALS.json), [Root-Ergebnis/Bildbindung](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_NATIVE_09_ROOT_RESULT_VISUAL.json). Native08 bleibt an seinem ursprünglichen Source unverändert45/45. Der tatsächliche unabhängige Abschluss dieses Nachlaufs folgt vor dem Freeze; eigene CI und I16 bleiben offen.

## Gezielter Nachlauf nach erster regulärer I-CI

F1 `89ee035ca0fa205ca1c0e845ad315047eae94fd8` hat kanonisch17/18 und Architecture39272387677 bestanden; Unity39272387608 ist tatsächlicher FAIL (Edit1107/1107, Play116/117, beide Mobilejobs SKIPPED, zwei reale Freigaben). Originale und unabhängiges FAIL-Urteil bleiben im eigenen I-Pfad erhalten. Native10/11 liefern den echten kausalen Headless-Testviewportbefund. Nach getrennter Planprüfung und dokumentierter R-Korrekturentscheidung ändert der Nachfolger ausschließlich die eigene Smoke-Fixture: tatsächliche positive endliche Panelmaße statt960×840, bounded Layout/Rebind und zusätzliche echte Panelcontainmentprüfungen. Alle ursprünglichen Assertions, Runtime-/A-/S-/Manifest-/Workflowbytes bleiben unverändert. Frischer gezielterCompileExit0; Native12 tatsächlich45/45 headless OS0, Native13 normaleGrafik1/1 OS0 mit vier von Root und Reviewer betrachteten Originalbildern. [Originale10–13](../PROJECT_CONTROL/ProductionBlocks/Public240Save/I/I_HEADLESS_001_NATIVE_10_13_ORIGINALS.json). Korrigierter eigener F2-Freeze, vollständige reguläre F2-CI und finales unabhängiges Urteil sowie I16-Unterintegration bleiben bis tatsächlichem Abschluss offen. Keine Grün-Wiederholung am gleichen fehlerhaften Stand.

## Tatsächlicher Abschluss und Unterintegration des Gesamtblocks

Technisch ABGESCHLOSSEN innerhalb MANUS-2026-10-19-01. Eigene angenommene T/A/S-Lieferungen bleiben exakt an ihren ursprünglichen Freezes/CI gebunden. Der korrigierte I-Source 242712d1b99202fd18273dc28ef2fd12aff71cab ist nach vollständiger eigener regulärer CI und unabhängiger Annahme durch den gesonderten R-Entscheid WP043-SUBINTEGRATION-01 tatsächlich als 1af988cd73a7267796fb3b6fd77204166241a07a ausschließlich in WP036 unterintegriert; geordnete Eltern B/F2, Tree exakt 8b4f4b2d9460979bd0a532c993933e7b5be4b93a, kein Source-Diff. Beide tatsächlich integrierten CI-Ereignisse und die unabhängige Integrationskontrolle sind PASS. [Eine vollständige Übergabe](../PROJECT_CONTROL/ProductionBlocks/Public240Save/HANDOVER.md) und die dortige AK-/Originalführung erschließen Ergebnis, Start-/Installationsweg und sämtliche Grenzen. Dieser separate Dokumentationsnachlauf erhält seine eigene tatsächliche Head-/CI-Bindung im PR31-/PR37-Nachtrag; noch nicht entstandene Doc-CI wird nicht vorab angenommen. Frühere offene Statusabsätze sind historische Zwischenstände. Keine Main-/Produkt-/Human-/Gerätefreigabe.
