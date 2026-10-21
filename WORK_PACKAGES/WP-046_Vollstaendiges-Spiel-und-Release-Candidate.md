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

21.10.2026: Vollständige Candidate-Spielmontage einschließlich Kampagne/Metagame, vier Endlosprofilen, Save/Resume/Hint/Daily/Sammlung/Revision, echter gespeicherter Pfadfahrt, Audio/Accessibility und tatsächlichen Mobiladaptern integriert. EditMode9 70/70, ganzer Endlos-UIweg bis zweiter Partie Play16 1/1, Kauf-/Offlineintegration Play17 32/32 bestanden. Sieben DRAFT-Geometrien verbessert, zu ähnliche Kandidaten ersetzt, bestehende C#-Pipeline240/240PASS. ADR037–043 angenommen. Finaler Endlosstichprobenpool577Dateien/96Kandidaten/48Auswahl unabhängig mit96 frischen UNIQUE/NoGuess-Solverläufen geprüft.

Separater Android-Humanprüfzugang mit demselben tatsächlichen Spiel integriert:17Sechserpacks/102Aufgaben, eigener serialisierter Store, dauerhafte Exposition/Anzeigereceipts, Hint-/Restartgeschichte, aktive Zeiten, expliziter neuer Teilnehmer und enger privater JSONexport. Reale Recovery-/Ack-/Geometrie-/Testwartebefunde geschlossen; Edit12 44/44, final Edit13 24/24, Play20 2/2 und HumanScene13 Exit0. Portable Quellenbindungen und Exportreadback integriert,29 kontrollierte Fälle/144Assertions PASS; keine echten Humandaten. Operatorpackverteilung und technische Production-/nonnull-Sternzeiten-/Endloszeitsterntechnik werden weitergeführt, notwendige echte Verträge vor Abhängigkeit getrennt geprüft. DRAFT-Nullschwellen bleiben kein Launchzustand.

Eigene Appicons tatsächlich zugewiesen; echte Icon-/inkrementelle Übergangsbefunde ausAPK5/Human2/Main6 gezielt geschlossen, Originale erhalten. Korrigierte Haupt-APK7 und Human-APK3 tatsächlich gebaut und unabhängig native-/iconbezogen PASS; DEX-/Provider-/Exportpfad-/Mergegates geprüft. iOSexport3 mit3282Dateien/ZIP362.753.079Bytes strukturell und19Icons geprüft, keine native Xcode-/Signierungsannahme. Ältere APK4/iOSexport2 bleiben ehrlich preicons/sourceb22a3ff gebunden. CI auf tatsächliche Spielszene, explizite Ziele und macOS-Pods-/Workspacecompile umgestellt; lokale19Classifier-/36Bash-/9Python-/4Negative-/18Architecturegruppen PASS, tatsächlicher Remote-CIlauf nach Push folgt. ADR044 samt exakten Profil-/Importformen und ADR045 mit geschlossener flüchtiger Routingauthority vor abhängiger Umsetzung angenommen; Production-/Zeitsterntechnik und Multipackmontage entstehen imStaging. Kein M1-/M2-/M3-PASS; Geräte/Humans/Ownerannahme, qualifizierte Rechts-/Storeinputs, Signing/Sandbox/Storegates offen. Eine Hauptübergabe bindet Originale und Fortsetzung; derselbe Auftrag bleibt bis M3 aktiv.
