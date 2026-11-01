# ADR-033 – Interne transiente Spielschleife ohne Produktionscontentimport

## Status

**Angenommen** – tatsächliche zuständige Manusannahme vom 19.10.2026 für den unveränderten engen internen QA-Gegenstand. [MANUS-2026-10-19-01](../PROJECT_CONTROL/ProductionBlocks/WP-038/MANUS_DECISION_AND_EXECUTION_MANDATE_2026-10-19.md), §3, ist die heutige Annahmequelle. Keine frühere zuständige Vorcode-Annahme oder vollständige historische WP-038-AK02-/AK11-Erfüllung wird dadurch belegt; die damalige Codex-Autoritätsbehauptung bleibt nach der Herkunftsberichtigung unzutreffend. Die neue Annahme gilt ab heute für die interne Weiterentwicklung, nicht für Main, Produktcontent, Save/Rewards oder Release. Fachkörper und damaliges Dokumentationsdatum bleiben unverändert.

### Historischer Statuswortlaut vom 18.10.2026 – Autorisierung durch Herkunftsberichtigung berichtigt
**Angenommen** – zuständiger technischer Entscheid der ausführenden Koordination im ausdrücklichen Geschäftsführerauftrag vom 18.10.2026, nach tatsächlicher unabhängiger Boundaryprüfung PASS durch /root/wp037_independent_qc. Entwurf und getrennte Prüfchronik sind im eigenen Evidenzpfad erhalten. Annahme erfolgt vor Implementierung; keine Produkt-/Main-/Releasefreigabe.

## Datum
2026-10-18

## Kontext
Domaincommands, Completion und Solver-/Proofgrundlagen sind implementiert; Application, Persistenz und Darstellung sind bisher überwiegend WP021-Skelette. Der bestätigte Nutzerauftrag verlangt echte Fortschritte zu einer bedienbaren Spielversion und untersagt, menschliche Rätseltests vor deren Existenz zu verlangen. Gleichzeitig verbieten CONTENT_CATALOGS/CONTENT_PIPELINE den Productionimport von DRAFT und FIXTURE_ONLY; WP026 bleibt offen, Textquellen bleiben DRAFT und kein Save-/Rewardpfad ist fertig. Ein interner bedienbarer Test darf diese Grenzen nicht umgehen oder als Produktionskampagne auftreten.

## Entscheidung
Entschieden wird eine explizit isolierte QA-Spielschleife im bestehenden Unityprojekt unter dem bestehenden nichtöffentlichen QA-Buildprofil. Sie verwendet den echten Domainkern über eine reine Application-Sessionsteuerung, UI Toolkit und eine aus dem tatsächlichen PuzzleSolved-Ereignis abgeleitete neutrale World-Zugdarstellung. Zwei eigene technisch benannte QA-Definitionen enthalten ausschließlich öffentliche Puzzlewerte und gehören keiner Kampagne an. Kein WP026-/WP036-Import, keine gespeicherte Lösung und keine Runtimekatalog-/Release-Lock-Promotion.

Der gesamte Versuch bleibt im Speicher. Domainphase SolvedPendingCommit wird nicht in einen falschen Saveerfolg verwandelt. Eine getrennte QA-Präsentationsphase erlaubt die interne Fahrt-/Resultprobe, sichtbar als ungespeichert und ohne Reward. Production hält weiterhin strikt Savecommit vor Fahrt/Reward ein. Keine Datei-, Ledger-, Stern-, Kampagnen-, Claim-, Hint-, Daily-, Endless-, SDK-/Storewirkung.

Die QA-Szene besitzt genau einen DevelopmentPuzzleInstaller. Ihre UXML-/USS-/Panel-/Locale-/Materialreferenzen werden explizit serialisiert und validiert. Nur für diese isolierte Szene ist direkte Inspectorbindung zulässig; das ist kein neuer Production-Ladepfad. Productionassets bleiben über lokale Addressables und freigegebene Kataloge geladen. Keine Resources-, Reflection-, Szenensuch- oder Service-Locator-Ersatzverdrahtung.

Eine eigene neutrale QA-JSON-Quelle liefert stabile technische de-DE-Schlüssel, Werte und Formatverträge. Der UI-Adapter erzeugt daraus ausschließlich im Speicher eine Unity-Localization-Locale/StringTable und löst sämtliche sichtbaren Texte darüber auf. Keine sichtbaren C#/UXML-Literale, kein generischer Missing-Key-Fallback und keine Kopie/Änderung der angenommenen Bibliothek. Unity.Localization wird als bereits durch ADR011 vorgesehene externe Referenz ausschließlich der UI-asmdef hinzugefügt; der CI-Preflight prüft die bisherigen internen Kanten weiter exakt und erlaubt separat nur diesen Paketverweis.

Ein reproduzierbarer DevelopmentPuzzleBuild erstellt die eng benannte QA-Szene, Panelsettings und das neutrale technische URP-Trainmaterial. Er referenziert intern weiterhin ausschließlich Bootstrap. Bootstrap konsumiert Application-DTOs und seine bereits erlaubten Adapter-/Presenterreferenzen, keine direkte Domainkante. Bestehende reguläre Buildentrypoints bauen den neuen QA-Stand mit, behalten aber alle Versions-, Profil-, URP-, Stripping- und Composition-Smoke-Gates. Kein Produktions- oder Storebuild wird dadurch freigegeben.

## Begründung
Die echte Application-/Darstellungsproduktion wird bedienbar und testbar, während jede noch nicht implementierte dauerhafte Wahrheit offen bleibt. Die begrenzte Szene vermeidet erfundene Produktannahmen und eine parallele Puzzleimplementierung. Ein Harness allein ist keine vollständige spielbare Season; er schafft den ausführbaren Anfang des vorgesehenen lokalen Ablaufs und belastbare Anschlussstellen für Save-/Contentproduktion.

## Betrachtete Alternativen
| Alternative | Bewertung |
|---|---|
| WP026-DRAFT still in Runtime importieren | Verletzt ausdrückliche Import-/Freigabeverträge und wird verworfen. |
| Alle Arbeiten bis Humanreview, finalen Assets und Sternzeiten stoppen | Widerspricht OPEN_BLOCKERS und dem neuen Nutzerauftrag; kein technischer Zwang. |
| Separates Webspiel, Regelkopie oder neues UI-System | Verdoppelt Autorität und verfehlt die angenommene Unityarchitektur. |
| Schon jetzt vollständige Save-/Economy-/Katalogproduktion behaupten | Nicht implementiert und nicht geprüft; eigener großer Folgeblock. |

## Konsequenzen
Der QA-Stand ist intern bedienbar und kann manuell exploriert werden; er ist kein bestandener menschlicher WP026-Spieltest, kein Productionimport, keine Fortschrittspersistenz und kein Release. Der spätere Save-/Kampagnenblock muss diese technische QA-Präsentationsgrenze bewusst durch den echten persistierten Abschluss ersetzen. Die QA-Szene darf nie als Productionentry ausgeliefert werden; negative Build-/Compositiontests prüfen das. Finale Grafik-, Gesten-, Motion-, Accessibility- und Zeitentscheidungen werden nicht vorweggenommen.

## Betroffene Artefakte
WP038 und eigenes Manifest; DevelopmentPuzzleSession, DevelopmentPuzzleData, DevelopmentClock, DevelopmentPuzzlePresenter/PuzzleGridElement, DevelopmentTrainRide, DevelopmentPuzzleInstaller, DevelopmentPuzzleBuild, eigene QA-UI-/Szenen-/Material-/Localequellen; vorhandene UI-asmdef, unity.yml-Preflight und StpBuildEntrypoints; MODULE_BOUNDARIES/CONTENT_PIPELINE/BUILD_AND_RELEASE ausschließlich erklärend für diese Grenze; ADR-Index und eigene Produktionssteuerung. ADR011 erhält ausschließlich einen additiven rückwärtigen Link-/Geltungsnachtrag auf diese eng begrenzte QA-Ergänzung. Sein angenommener historischer Entscheidungskörper bleibt unverändert; der Index benennt den unveränderten produktiven Addressables-/Localizationvertrag und die Ausnahme nur für die bestimmte interne QA-Szene. tools/architecture-validation/validate.py führt ausschließlich die geschlossene Registry/Inventar-/Indexerwartung für diese tatsächlich neu angenommene ADR033-Grenze auf exakt 33 ADRs/24 angenommen/9 ersetzt nach und ergänzt die gezielten fehlender-ADR033-Indexeintrag-/falscher-ADR033-Indexstatus-Negativregressionen; keine dynamische Annahme unbekannter ADRs und keine andere Gate-/Scope-/Hash-/Fachsemantikänderung.

## Validierung
Unveränderte interne Assemblykanten und exakter externer Localizationverweis; public-only QA-Daten und keine Content-/Solutionreferenzen; Domaincommands statt eigener Regeln; Timer/Batch/Undo/Counts; echte PuzzleSolved-Pfadbindung, Fahrt vor Result, Neustart und Lifecycle; fehlende Referenzen/Keys scheitern; keine Save-/Provider-/Rewardwirkung; reproduzierbare Szene und reguläre Android-/iOS-CI; getrennte unabhängige Abschlussprüfung. Die grafische Bedienprobe verwendet die tatsächlich verfügbare Route aus WP038 Tests7: vorhandener lokaler Unity6000.3.23f1-Editor mit `-batchmode -force-d3d11` ohne `-nographics` nach tatsächlicher Lizenzprüfung ohne konkurrierende Personalinstanz; alternativ genau ein zusätzlicher gefilterter Grafiklauf im bestehenden serialisierten unity-playmode-bootstrap-smoke-Job nach dem vollständigen unveränderten Headlesspflichtlauf und vor Seatfreigabe, `xvfb-run`/Software-GL/`-force-glcore` ohne `-nographics`. Identische Runner/Toolchain/Limits/Seatserialisierung bleiben erhalten. Tatsächliche UI-Aktionen, Nicht-Nullrenderer, eigene bestandene Grafik-XML-Fälle, PanelSettings.targetTexture/RenderTexture-/ReadPixels-Bilder und selbst betrachtete 4×4-/10×10-Screenshots sind erforderlich. CI-Grafikbilder werden unter TestResults/DevelopmentGraphics vom vorhandenen Upload archiviert, ohne den ursprünglichen Pflichtlauf/Coveragebeleg zu ersetzen. Ein fehlender Legacy-ULF-File ist allein kein Lizenzbeweis; keine Ausführung/Rendererfähigkeit wird vorab PASS genannt. Production-Start/-Build mit QA-Installer muss scheitern. Keine vorhandene Content-/Import-/Save-/Releaseprüfung wird als bestanden umgedeutet.

## Ersetzt / ersetzt durch
Ersetzt keine angenommene Entscheidung. Ergänzt ADR011 ausschließlich um eine ausdrücklich begrenzte nichtproduktive Inspector-/In-Memory-Locale-Harnessbindung. ADR005/013/018 sowie alle Content-/Save-/Releaseverträge bleiben wirksam. Noch durch keinen ADR ersetzt.
