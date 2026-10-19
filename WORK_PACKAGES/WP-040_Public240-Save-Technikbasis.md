# WP-040 – Gemeinsame Public240-/Save-Technikbasis T

## ID
`WP-040`

## Ziel
Prospektive, unabhängig geprüfte gemeinsame technische Basis T für den zusammenhängenden Auftrag MANUS-2026-10-19-01 §§4–7. A/S beginnen erst auf dem vollständig abgeschlossenen T. Kein Ende des Gesamtauftrags nach diesem Teil.

## Voraussetzungen
AGENTS und Konzept15/00/09, CURRENT_STATE/WP038, vollständiges Manusmandat, WORK_QUEUE/WORK_PACKAGE_RULES/AI_HANDOVER_RULES/DEFINITION_OF_DONE, CONTENT_PIPELINE/CONTENT_CATALOGS/PERSISTENCE/GAME_STATE_MODEL/MODULE_BOUNDARIES/OPEN_BLOCKERS/TEST_STRATEGY/BUILD_AND_RELEASE und ADR-Verfahren/ADR030 vollständig lesen. B ist 3d1ac59c3f70d50e05434a54d3ddec5d282bb0c3, Erstadd des Mandats, Parent fb99477. Frische Remote-/CI-/Ancestorprüfung und keine Doppelarbeit. Eigener Branch codex/wp-040-public240-save-technikbasis. Dieses WP und [unveränderliches Productionmanifest](../tools/architecture-validation/scopes/WP-040.production.scope.json) gemeinsam vor Facharbeit mit einzigem Parent exakt B verankern. IDs gegen alle frisch verfügbaren Remote-Bäume prüfen.

## Scope
1. Eingangsbefund mit Git-/CI-/Quellbindungen sichern, bestehende Nachweise gezielt wiederverwenden.
2. ADR034: allowlist-basierte public-only Projektion des exakten WP026-Freeze dc1652befd7545b4c2a2dd0580f35a4e8e05d72d; separate QA-Auswahl, Entry und QA-Profil-/Katalog-/Savegrenze, keine Productionpromotion. Vor Code Entwurf, getrennte technische Review, R-Annahme.
3. ADR035: typisierte providerneutrale Save-/Katalogports, vollständiger validierter Domaincheckpoint mit Undo/Timer/sticky Flags/PreviouslySet/Revision/Dedup, Generation/CAS und Commit-vor-Fahrt; JCS ausschließlich wiederverwenden über injizierten Application-Byteport und Contentadapter ohne neue Assemblykante. Exakte Save-v2-/Migrations-/Recovery-/Ledgerverträge bleiben erhalten; ausgeschlossene Features nur erhalten, nicht aktivieren. Entwurf → getrennte technische Review → R-Annahme vor Umsetzung.
4. LocalGameContracts.cs implementiert gemeinsame immutable PublicPuzzle-/Identity-/Catalogtypen, geschlossene Operationsergebnisse, generischen typisierten ISaveRepository<TSnapshot>-Zusatzvertrag mit Cancellation/Generation/CAS und ICanonicalJson für begrenzte strikte JSON-Kanonisierung. Bestehende leere Scaffoldports bleiben für den Composition-Smoke kompatibel; neue produktive Adapter benutzen die typisierten Ergänzungen. Keine Dateisystem-/Unitytypen im Applicationvertrag.
5. CanonicalJsonAdapter verwendet unverändert StrictJsonParser/JcsSerializer und liefert kopierte Ergebnisse/stabile Fehler. Keine zweite Hashsemantik, Persistence→Content-Kante oder Änderung bestehender JCS-/Level-Quellen.
6. ADR033 ausschließlich formale Statusauszeichnung korrigieren: abgeschlossener fett gesetzter Statusbegriff, Erläuterung außerhalb. Datum, Manusannahme/Provenienz und Fachkörper bleiben unverändert. Validator nicht abschwächen.
7. Geschlossene ADR-/Inventar-/Indexregistry exakt für tatsächlich angenommene 034/035 nachführen; bestehende Assertions/Unknown-ADR-Ablehnung erhalten, gezielte neue Index-/Status-Negativmutationen. Keine Scope-Summensemantik.
8. Vor A/S konkrete Patheigentümer und prospektive AK-/Testmatrix dokumentieren. A besitzt ausschließlich neue Public240-Daten/Loader/Projektion/UI/Builder/Szene und deren Tests. S besitzt neue Save-/Profile-/Ledger-/Migrations-/Sessiondateien und Domaincheckpoint plus Tests. Gemeinsame bestehende UI-/Installer-/Buildverdrahtung ausschließlich I. A/S ändern keine gemeinsamen Status-/Registry-/Dokumentdateien; ihre WP-/Evidenzpfade sind getrennt. Beide erhalten exakt T als einzigen Ankerparent. Tatsächliche freie Kind-IDs erst nach T prüfen.
9. Vorbereitung als eigenen Draft-Unter-PR gegen WP036 veröffentlichen, nötige reguläre CI, unabhängige T-Abschlussprüfung. T nicht einzeln in Development unterintegrieren; T ist gemeinsame Kindbasis. Gesamtübergabe bleibt in PROJECT_CONTROL/ProductionBlocks/Public240Save/HANDOVER.md erschließbar.

## Betroffene Dateien/Module
Abschließend nur die Pfade im verlinkten Manifest. Neue ADR034/035, LocalGameContracts.cs, CanonicalJsonAdapter.cs samt Metas, CanonicalJsonAdapterTests.cs samt Meta und eigene Public240Save-Evidenzdateien. Bestehend nur ADR033-Statusformat, ADR-Index/geschlossene Validatorregistry und gezielte Negativtests, validate.yml-Manifestselektor sowie eng additive Vertrags-/Statusnachträge. Bestehende Domain-/Solver-/JCS-/Text-/Levelquellen und alte Manifeste unverändert.

## Ausdrücklich nicht erlaubte Änderungen
Keine Mainintegration, Produkt-/Release-/WP026-Gesamtannahme, komplette WP026-Übernahme, Lösung/Proof/Notizen im Spielerpayload; keine neuen Zeiten/Preise/480Assets/finalen UXwerte; keine Hint/Daily/Generator/Kauf/Ads/IAP/Analytics/Cloudaktivierung. Keine Gate-/Budget-/Runner-/Lizenz-/Coverageabsenkung, keine historischen Manifeständerungen. A/S-Fachimplementierung erst in eigenen prospektiven Kindaufträgen auf abgeschlossenem T.

## Akzeptanzkriterien
- T01: Erstadd/Parent/Base/Manifestbytes und frische Referenzen belegt; kein fremder Fortschritt überschrieben.
- T02: ADR034/035 tatsächlich getrennt geprüft und R-angenommen vor Implementierung; Produktgrenzen und historische Herkunft erhalten.
- T03: Typisierte, immutable und providerfreie gemeinsame Verträge mit Cancellation/CAS/closed result states; Patheigentümer vollständig.
- T04: JCS-Port nutzt bestehende Serializerbytes; Positiv-/Negativfälle einschließlich Duplicate Keys, Float, Unicode, Surrogat, Integergrenze und Größenlimit bestanden; keine neue Assemblykante.
- T05: ADR033-Formatfehler konkret reproduziert und rein formal behoben; Registry exakt 35/26 angenommen/9 ersetzt nach Annahme, neue negative Index-/Statusmutationen wirksam, übrige Checks erhalten.
- T06: Kanonische Architecture-only/Scope-Self-/Negativprüfungen und passende C#-/Unity-/reguläre CI tatsächlich bestanden; Originale und tatsächliche Checkoutbindungen dokumentiert.
- T07: Unabhängige technische Freezeprüfung ohne offenen Blocker/High, Arbeit/Ergebnisse/Grenzen und nächste A/S-Schritte in WP/CURRENT_STATE/QUEUE/HANDOVER dokumentiert. Der Gesamtblock bleibt bis I/Unterintegration offen.

## Tests
Vor Code: Gitanker/Ancestor/Byteinventar, bestehende CI lesend prüfen, ADR033-Befund reproduzieren. Vor ADRannahme getrennte technische Boundaryreview. Code: positiver JCS-Golden plus Node-Crosscheck und negative Fälle/immutable defensive copies, tatsächlicher vorhandener asmdef-Preflight; kanonische CPython3.11.13/Node22.20.0-Läufe mit --self-test und eigenem Scope. Vollständige reguläre bestehende CI am T-Freeze mit originalen XML/Logs/Checkout/Jobstatus; keine Reruns zum Erzwingen von Grün. Lizenzfreier C#-Compile ergänzt, ersetzt nicht Unity. Native Grafik hier nicht anwendbar, da keine Präsentation geändert wird; neuer A/I-Nachweis bleibt verpflichtend. Bestehende unveränderte Solver-/Text-/Grafiknachweise nicht pauschal wiederholen. Getrennte unabhängige T-Abschlussprüfung, nach Befunden betroffene gezielte Nachprüfung.

## Risikoklasse
Hoch: neue technische Review-/Persistenzgrenze und gemeinsame Schnittstellen. Begrenzung durch enge ADRs, geschlossene Ports, prospektive Scopes und getrennte Prüfung.

## Definition of Done
Projekt-DoD und T01–T07 tatsächlich erfüllt. Neue Dokumentationsnachläufe erhalten eigene CIbindung. T erst danach als unveränderlicher gemeinsamer Kindfreeze benennen. Keine Behauptung fertiger A/S-/I-Funktionen oder menschlicher Tests.

## Bearbeitungsstatus
Codefreeze54d fachlich unabhängig und durch tatsächliche reguläre CI nachgewiesen. Gemeinsame T-Basisfreigabe bleibt bis zur erfolgreichen eigenen CI des reinen Schlussdokumentationsstands D offen. Historische Zwischenstände sind nachfolgend erhalten; maßgeblich ist der letzte Schlussnachtrag. Gesamtauftrag läuft über A/S/P/I weiter.

### Implementierter T-Kandidat
ADR034/035 vor Code getrennt PASS und in e521398 durch R angenommen. Neue LocalGameContracts/CanonicalJsonAdapter plus gezielte Content-Contracttests implementiert; reine Serialisierungs-/Modulgrenze ohne neue Assemblykante. Lizenzfreies Roslyn-Supplement kompiliert die betroffenen Assemblies;22/22 gezielte NUnitfälle bestanden einschließlich tatsächlichem Interfacepfad, immutabler Public-/Catalog-/Save-/Recoverytypen, vollständiger Beobachtungsbindung und Node-JCS-Byte-/Hashgolden. Erste konkrete QA-Promotionslücke gezielt erkannt, korrigiert und historische rote XML erhalten. [Lokale Source-/Testbindung](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_LOCAL_EVIDENCE.json). Dies ist noch kein Unity-/CI-/unabhängiger T-Abschluss. Kanonische Vorbereitung17/18 mit Selftests PASS; neuer sauberer Freezecheck folgt. Gesamtauftrag wird nach T über A/S/P/I weitergeführt.

### Unabhängige konkrete Vorfreeze-Korrektur
Ursprünglicher lokaler Kandidat4929ad8 bestand seine sauberen kanonischen17/18, wurde jedoch durch getrennte statische Prüfung wegen zweier T03-Highs nicht angenommen. Strikte Sternzeitordnung und vollständiger Recoverybeobachtungstransport gezielt korrigiert;20/2Rotnachweis gegen4929 und22/22korrigiertes Supplement tatsächlich ausgeführt, Tabellenkleinbefund behoben. [Unveränderter ursprünglicher Prüfstand und Korrekturen](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_INITIAL_FREEZE_QC.md). Neuer Codefreeze/CI/gezielte unabhängige Bestätigung stehen noch aus; kein gemeinsamer T-Abschluss oder A/S-Startclaim.

### Tatsächlicher T-Codeabschluss und gesondertes Dokumentationsgate

Codefreeze54d2496ac2c74b681d24fe7a69d7f044392c46dd/Tree85d1cdb ist nach gezielter unabhängiger Korrekturprüfung fachlich belegt. [AK-Matrix](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_ACCEPTANCE_MATRIX.md): T01–T05 erfüllt; T06 am Codefreeze durch originale kanonische17/18, UnityEdit496/496, Play49/49 und tatsächlicheAndroid-/iOS-Builds belegt. Reguläre Architecture39217159710SUCCESS. Unity39217159752Attempt1 hat9SUCCESS/iOSDNSFAIL; der ausschließlich infrastrukturell begründete einmalige Attempt2/iOS117958208352 ist vollständigSUCCESS. Original9PASS und neueiOS-Originale werden getrennt erhalten, keine zehn neu ausgeführten Jobs desselben Attempts behauptet. Tatsächliche vier Freigaben:07:44:38.5092525Z/07:48:10.1395829Z/08:04:05.1284825Z/08:29:23.462817Z.

[T_CI Originalattempt](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_CI.md), [separater Infrastrukturentscheid](../PROJECT_CONTROL/ProductionBlocks/Public240Save/T_IOS_INFRA_DECISION.md), T_IOS_RETRY.md/JSON als eigener späterer Originalnachweis. Der ursprüngliche Formatkleinbefund wurde erst im dokumentierten Nachlauf tatsächlich behoben; die frühere Aussage ist in T_DOCUMENTATION_DELTA präzise berichtigt. Vorherige Berichte bleiben unverändert. A-/S-Planung, geschlossene Recordgrammatik, vollständige Endlessparameter, Writer-Sicherungsinvariante und eng payloadgebundenes Recoverylease vor S-Code getrennt geprüft/angenommen; kein neuer Produktentscheid.

T07: unabhängige abschließende enge Code-CI-/Dokumentdelta-Prüfung vor Schlusscommit, WP/CURRENT_STATE/QUEUE/HANDOVER nachgeführt. Dieser reine Schlussdokumentationsstand D enthält keine Source-/Meta-/Test-/Workflow-/Manifeständerung gegenüber54d. **D darf erst nach erfolgreichem Abschluss seiner eigenen regulären CI als gemeinsame A/S-Basis verwendet werden.** Deren tatsächliche Run-/Checkout-/Tree-/Originalbindungen und anschließende R-Freigabe werden identisch in beiden erst danach prospektiv erstellten Kindankern dokumentiert; kein vorab eingetretener D-CI-Erfolg und keine unendliche rekursive Dokumentationscommitkette. Bis dahin bleibt der gemeinsame T-Freigabeschritt offen. Gesamtauftrag wird anschließend innerhalb desselben Mandats vollständig weitergeführt.
