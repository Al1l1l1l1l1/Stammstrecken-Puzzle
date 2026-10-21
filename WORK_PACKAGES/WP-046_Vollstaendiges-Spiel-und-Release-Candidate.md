# WP-046 – Vollständiges Spiel und Release Candidate

## ID

`WP-046`

## Ziel

Stammstrecken-Puzzle als überzeugendes vollständiges mobiles Spiel bis zum einreichungsfähigen Android-/iOS-Release Candidate M3 liefern. Releaselead R verantwortet denselben Auftrag einschließlich Vorbereitung, drei paralleler Bahnen, Umsetzung, Korrektur, Integration und unabhängiger Meilensteinprüfung. M1/M2 sind Zwischenziele, kein Auftragsende.

## Voraussetzungen

Direkter Gesamtauftrag des Owners vom 21.10.2026 und [MANUS-2026-10-20-02](../PROJECT_CONTROL/PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md) einschließlich P01–P07. Tatsächlich gefetchte Developmentbasis `bd05da191cd2bf8da89830f7db6da4941cc62343` auf `codex/wp-036-vollstaendige-textbibliothek`; Main bleibt geschützt. AGENTS, CURRENT_STATE, aktuelles Mandat und einschlägige Fachverträge sind Eingänge. Issue #38 wurde am 21.10.2026 vollständig per GitHub-Connector gelesen; keine dort fehlenden Originalmedien als erneut betrachtet ausgeben. Angenommene Stilquelle `9e3f209cd92d491ab3a1ab51cce63677d3b5d53e`, Rätsel-/Proofquelle `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d` gezielt verwenden.

## Scope

Ein vollständiges Produktionsprogramm mit [unveränderlichem gemeinsamem Production-Manifest](../tools/architecture-validation/scopes/WP-046.production.scope.json), vor Fachänderungen gemeinsam verankert. Keine unabhängigen Arbeitsbranches erforderlich: getrennte Dateiverantwortung im gemeinsamen Checkout. A liefert neue tatsächliche Spieloberflächen, Cartoon-Welten, Züge/Kosmetik/Audio und den vollständigen Spielweg. B liefert 240 validierte Rätsel, gezielte späte Schwierigkeit, echte Zeitkalibrierung, Kampagne/Metagame/Hint/Daily/Sammlung/Revision und vier vollständige Dauerbaustellenprofile. C liefert echte beschlossene Mobilservices, Profile, Releaseautomation und vollständige Release-/Store-/Rechte-/Privacyvorlage. R besitzt zentrale Composition, Builds, Szenen, Workflows, Projektsteuerung und gemeinsame Verträge.

Echte neue öffentliche Daten-/Save-/Hash-/Security-/Provider-/Modulverträge vor abhängiger Implementierung kurz per ADR und getrennter Fachreview entscheiden. Routineentscheidungen im bestehenden Stack trifft R. Konten-/Geräte-/Human-/Rechtsfragen früh gebündelt klären; unabhängige Arbeit fortsetzen. Die bereits vorhandenen lokalen Unity-Einstellungsänderungen bleiben erhalten und werden getrennt von eigenen Änderungen dokumentiert.

## Betroffene Dateien/Module

Die segmentierten Modulgruppen im Manifest sind abschließend: Application und Puzzle/Generator, Infrastructure.Content/Persistence, Presentation.UI/World, Audio, MobileServices.Google/Store, Platform, Bootstrap, Editor.Content/Build; dazugehörige Unitytests, Art/UI/Audio/Localization/Generated/Runtimecontent/Settings/Scenes; authoringseitige Levels/Proofs/Catalogs/Quality/Generator/Assets/Localization; Packages/ProjectSettings und native SDKintegration; gezielte CI-/Content-/Releasewerkzeuge und Buildworkflows; geänderte Architektur-/Schema-/ADR-/Fachverträge. Keine Änderungen historischer Manifeste oder alter Evidenz. Hauptübergabe und interne Teilbriefe unter `PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/`.

Dateieigentum: A ausschließlich Presentation/UI, Presentation/World, UI, Art, Audio und eigene Presentationtests. B Application, Infrastructure.Content/Persistence, Contentkataloge/Quality/Generator, Editor.Content und eigene EditMode-Tests. C MobileServices.Google/Store, neue spezifische Platformadapter, Packages/native Providerquellen, Release-/Store-/Legalvorlage und eigene Mobiletests. Zentrale Bootstrap-/Build-/Szenen-/Workflow-/Projekt-/Steuerungsdateien sowie ADRregistry nur R. Änderungen über Eigentumsgrenzen zuerst direkt intern abstimmen.

## Ausdrücklich nicht erlaubte Änderungen

Kein Engine-/Providerwechsel, kein zusätzlicher Analytics-/Crashlytics-/Mediations-/Backendlaunch. Keine neue Spielregel/Sonderfelder, keine stille Scopekürzung oder erfundene Humanzeiten/Owner-/Geräte-/Store-/Rechtsannahme. Keine QA-/DRAFT-/Fixturepromotion ohne tatsächliche erforderliche Gates. Keine stillen Saveverluste, doppelte Vergaben/Abbuchen oder vorzeitiges Storefinish. Keine Secrets in Chat/Repository. Öffentliche Storeeinreichung, Veröffentlichung, neue Zahlungen und Kontoverträge bleiben konkret zu autorisieren. Mainintegration erst nach integrierter RC-Prüfung und tatsächlicher Owner-Spielqualitätsannahme im geschützten PR-Verfahren. Keine Force-Pushes/Schutzumgehung.

## Akzeptanzkriterien

1. M1: installierbare neue Hochformatfassung mit Start/Kartenkontext, 6×5-Puzzle, gespeichertem Abschluss, tatsächlicher Spielerpfadfahrt, Ergebnis/nächster Aufgabe und echter 10×10-Bedienprobe. Überzeugende integrierte Cartoon-/Symbol-/Schriftgestaltung.
2. Sechs Formen/X/Grau direkt setzen/ersetzen/bei Identität leeren; Werkzeug bleibt aktiv, kein Mehrfachauswahl-/Anwendenmodus. Timer erst bei Mutation. Raster/Tools bei Tap/Undo/Autosave stabil; Savefehler sichtbar und behebbar. Automatisierte Positionsprüfung plus tatsächlicher Handtest.
3. M2: alle 240 Rätsel/fünf Welten/20 Routen und sämtliche Launchfunktionen einschließlich 14 Kosmetikitems/480 Referenzen vollständig integriert. P02–P06 und vorhandene Punkte-/Progressregeln korrekt umgesetzt.
4. Späte 48/alle 10×10/späte Abschlüsse und S1-05-04-12 gezielt verbessert/geprüft; neue Hashes/Proofs bei Änderungen, vollständige Kampagnenvalidierung. Echte Erstkontakt-/Sternzeit-/Generatorstichproben gemäß Mandat, keine Proxyannahme als Human-PASS.
5. Save-/Reward-/Hint-/Daily-/Ownership-/Kauf-/Restoreintegrität, Offline-/Lifecyclefälle, Profiltrennung und Privacygrenzen positiv/negativ abgesichert. Tatsächliche AdMob-/UMP-/Unity-IAPintegration mit nicht blockierenden Dienstausfällen.
6. M3: exakt gebundene signierte Android-/iOSartefakte, reale Geräte-/Sandbox-/Storekanalprüfungen, unabhängige RC-Regression und tatsächliche Owner-Spielannahme; vollständige Lizenz-/Rechte-/Datenschutz-/Store-/Launchvorlage. Erforderliche fehlende externe Nachweise verhindern M3.
7. Eine unabhängige Ergebnis-/Risikoprüfung je integriertem Meilenstein; konkrete Befunde im selben Auftrag schließen. Eine vollständige Hauptübergabe mit Quellen/Artefakten, Ausführung, Prüfungen, Risiken und abschließender Ownerhandlung; Issue #38 nur nach tatsächlicher Behebung/Prüfung fortschreiben.

## Tests

Scope-/Syntax-/Link-/CIclassifierprüfung vor Fachcode. Geänderte UI: Unitycompile, gezielte Edit-/PlayMode-Integration, echte Layout-/Input-/Save-/Fahrtregressionen, Sichtprüfung und physische Handtests. Geänderte Puzzle: Eindeutigkeit, Logik, Hash-/Proofregeneration, Qualität/Novelty/Generierungsbudget; finale vollständige Kampagnenvalidierung. Save/Geld/Consent: gezielte Race-/Crash-/Replay-/Recovery-/Profile-/Restore-/Grant-/Finalize-Negativfälle und unabhängige Fachprüfung. Reale Provider-/Geräte-/Store-/Privacy-/Accessibilityprüfungen auf tatsächlichen Artefakten. Finale RC-Integration einmal vollständig; keine teuren Unityläufe allein für unveränderte Doku. Nicht ausgeführte Prüfungen ausdrücklich offen, vorhandene Nachweise nur an ihrer wirklichen Quelle weiterverwenden.

## Risikoklasse

Hoch: vollständiges Produkt einschließlich Persistenz, Geld/Consent, Puzzlequalität, nativer SDKs und Releasefähigkeit. Externe Gates werden nicht simuliert.

## Definition of Done

Alle AK und aktuelle [Definition of Done](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) für M3 tatsächlich erfüllt. M1/M2 und Teilimplementierungen bleiben Zwischenstände. Hauptübergabe/WP/CURRENT_STATE nennen echte Quelle/Artefakte, ausgeführte Prüfungen, offene Befunde, externen Bedarf und nächste autorisierte Handlung. Arbeitsziel vier Wochen, bedingter Korridor vier–sechs ab 21.10.2026; konkreter Restplan spätestens nach M1/Tag3 anhand tatsächlichen Durchsatzes, keine Termingarantie.

## Bearbeitungsstatus

21.10.2026: Vollständiges Kandidatenspiel einschließlich 240 Kampagnenrätseln, fünf Welten/20 Routen, Metagame, vier Endlosprofilen, Save/Resume/Hint/Daily/Sammlung/Revision, echter gespeicherter Pfadfahrt, Audio/Accessibility und tatsächlichen Mobiladaptern integriert. Sieben DRAFT-Geometrien gezielt verbessert, zu ähnliche Kandidaten ersetzt, tatsächliche C#-Pipeline240/240PASS. Aktueller P05-Suchkernreplay96/96 mit576bytegleichenPayloads/96IDs/48Auswahlen unabhängig geprüft und als neuer577Datei-Originalpool erhalten. Reale Difficulty-/Zeitannahme bleibt offen.

ADR037–045 angenommen. Operatorrouting für17Sechsergruppen/102Aufgaben, eigener Humanstore/Export, Sourcebindungen/Readback sowie strikte kalibrierte Productionprojektion/Runtimecomposition/Retained-/Endloszeitsterntechnik und träge serialisierte Dienste integriert. Tatsächliche Prüfungen: Edit14 148/148; Play22 1/1; Edit15+voller10000-WechselstressEdit16 gemeinsam82/82; LegacyEdit17 31/31; RuntimeEdit19 24/24; SourcepreflightEdit20 25/25; NativeentryEdit21 28/28. Play23 23/24, reine zu frühe Daily-Testbeobachtung korrigiert, Play24 betroffen+7bestehendeSpielwege8/8PASS. Nullschwellen/DRAFT/fehlende echte Freigaben werden weiterhin abgelehnt; keine positive Productionbuildbehauptung.

Letzte Spielgestaltung mit gemalter Cartoonumgebung im freien Rätselrand, Arbeitsplattenkante und klaren Werkzeugen unabhängig angenommen. Raster/Ports/Zahlen/Hitziele unverändert; tatsächlicher Play25 8/9, ausschließlich neue synthetische Telefon-Testfixture korrekt an echtes Dokument angepasst, Play26 2/2PASS. R sah aktuelle4×4/6×5/10×10-Ansichten bei390×844 tatsächlich. Fehlende14bestehendeKosmetik-Autorennamen quellengetreu ergänzt und unabhängig geprüft; kein Katalog-/UI-/Savewechsel.

**Aktuelle installierbare Haupt-APK8 und Human-APK5 tatsächlich gebaut und unabhängig geprüft:** [Spielinspektion](../Release/ANDROID_BUILD8_INSPECTION.json), [Humaninspektion](../Release/HUMAN_ANDROID_BUILD5_INSPECTION.json). Human5 Android03/Revision3/UI75`63e1…`, alle17Scene-PPtrbindungen/19TextAssets/102Aufgaben und tatsächliche native Manifest-/DEX-/Provider-/Iconbytes geprüft. Aktuellen bestätigten Studienlauf vor Appupdate/nächsterPerson exportieren, alte APK/Quellen/lokaleDaten erhalten; unbekannteVersionen werden nicht gelöscht oder migriert. iOSexport3 bleibt ursprünglicher tatsächlicher Quellprojektexport ohne Xcode-/IPA-/Signingannahme.

**Git/CI:** Gemeinsamer unveränderter Anker39aaac5. Owner bestätigte konkret vier Commits bis4102f64; normaler Push auf bestehendenDevelopmentzweig erfolgreich. Tatsächliche Architecture-CI39665718891 PASS, Unity-CI39665719215 läuft; kein lokaler Unityparallelbetrieb. Lokaler Folgecheckpoint a046656 und danach integrierte geprüfte Änderungen sind nicht Teil des bestätigten4102-Pushs. Kein Mainmerge oder Storeakt.

**Noch offen:** repräsentative Owner-Spiel-/Gestaltungsannahme, echte Erstkontakte/Difficulty-/Sternzeiten-/Profil-/Quellenfreigaben, physische Android-/iOS-/Sandbox-/Storekanal-/Privacy-/Accessibility-/Performancebelege, Signing und qualifizierte Rechtsendprüfung. Kein M1/M2/M3-PASS. Eine [Hauptübergabe](../PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/HANDOVER.md) bindet Originale und Fortsetzung; [kurze Begleitliste](../Release/OWNER_INPUTS.md) nennt tatsächlich benötigte Ownerinputs. Derselbe Gesamtauftrag bleibt bis M3 aktiv.
