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

Stand24.10.2026: vollständiger Spielumfang integriert. Übertragener Quellcheckpoint4eee9af498db45c32eb4bdd22f953a6e6ec2ea16 enthält genau zwei gezielte Endlos-Startverbesserungen auf f8-Basis: Encoder vermeidet doppelte Prüfung desselben unveränderlichen Zustands, Root zweite unveränderte Menüprojektion ohne PuzzleView. Vollständige Validierung/Generationsguards, öffentliche PayloadBytes, Polling und alle Puzzlepfade bleiben erhalten.389Old-/New-Ergebnisse exaktgleich; keine Fristen/Assertions/Fälle gekürzt. Unabhängige technische Annahme abgeschlossen.

VollEdit33 **1.706/1.706 PASS**, VollPlay40 **251/251 PASS**, jeweils UnityExit0, ohne Skip/Retry/Repeat, Coverage24Inventare und unabhängige Originalannahme; exakt alte Fälle/Fristen. Beide Endlosfälle erreichen Hinweis/Credit2/Abschluss/zweiteReservation und Runtime physischenSave-Reread. [Haupt-APK14](../Release/ANDROID_BUILD14_INSPECTION.json) tatsächlich aus4eee9af gebaut, Originale gesichert und unabhängig als QA-/Debugpaket angenommen. [Human-APK8](../Release/HUMAN_ANDROID_BUILD8_INSPECTION.json)/Android06/Revision6 bleibt unverändert mit102Aufgaben/UI75 gebunden; alte Beobachtungen und Pakete behalten ihre Fassung. Alle Quellen/Originalhashes stehen in der [einen Hauptübergabe](../PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/HANDOVER.md).

Neue vollePR-CI40123233064 läuft; noch kein neuer Linux-/Android-/Pods-/Xcode-PASS. Vorheriger f8-Lauf: Edit1706PASS, Play249/251 mit2Idlefehlern beim ersten Endlosstart, keine180sTimeouts; Native übersprungen. Diese Originale bleiben fehlgeschlagen. Die übertragene enge Pfaderkennung vermeidet doppelte Unityausführung für bekannte Änderungen, ohne unbekannte Pfade oder echte Rezepte zu lockern.

Offen bisM3: tatsächliche Owner-Spielannahme, echte Erstkontakte und Difficulty-/Sternzeit-/Profil-/Quellenfreigaben, physische Geräte-/Privacy-/Lifecycle-/Accessibility-/Performancebelege, reale Provider-/Kauf-/Restore-/Storekanaltests, Signing und qualifizierte Rechtsendprüfung. [Kurze Begleitliste](../Release/OWNER_INPUTS.md) mit Bedarfszeitpunkten. Kein M1/M2/M3-, Main- oder Storeabschluss; vollständiger Auftrag bleibt aktiv.
