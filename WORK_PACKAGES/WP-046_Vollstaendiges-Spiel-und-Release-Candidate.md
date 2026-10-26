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

Stand26.10.2026: vollständiger Spielumfang bleibt integriert. Neuer Spielquellcheckpoint 9812c956b7bbb5c36025643488ecb88a8610ea0e ergänzt die vorherige iOS-POSIX-Korrektur um genau eine unabhängig geprüfte Ledgerdatei. Begrenzte instanzgebundene Entry-/Journal-Projektionsbytes mit vollem Feld-/Reihenfolgevergleich, privaten Snapshots, Rückgabekopien und geschützter Verwaltung vermeiden wiederholte Arbeit; sämtliche Daten-, Chain-, Terminal-, Balance-, Limit- und physischen Speicherprüfungen bleiben erhalten.401 Alt-/Neu-Ergebnisse exakt gleich,319 gezielteTests und zusätzliche unabhängige Byte-Eviction-/ParallelprobenPASS. Kontrollierte240-Completionpaare42–45% schneller, echte volle Speicherpaare10–17% schneller bei identischem Ablauf/Resultat. TatsächlicheUnity844Edit/2vollständigeEndlosfälle71,606s/72,640sPASS, Fälle/Fristen/0Retry/Coverage24 unverändert, Originale unabhängig angenommen. NeueLinux-/Android-/Xcodeprüfung und frischeHaupt-APK stehen aus; keine Geräte-/M3annahme. Der tatsächliche iOS-Export5 bleibt Nachweis der vorherigen ABI-Korrektur, kein nativer Build dieser neuen Fassung.

Vollständige lokale Edit35 **1.706/1.706 PASS**, Play42 **251/251 PASS**, Unity-/CompilerExit0, gleiche Fälle und Fristen,0Skip/Retry/Repeat,Coverage24PASS. Je1114Vorlaufquellen und exakt2Deltas unabhängig angenommen. Beide Endlosabläufe vollständig einschließlich Hinweis/Credits2,Neustart,Abschluss,zweiterReservation/Identität; Runtime zusätzlich physischerReread/finalFirstClear240.88,82/95,72s beiunveränderten180s.

[Haupt-APK18](../Release/ANDROID_BUILD18_INSPECTION.json) tatsächlich ausbcb gebaut und unabhängig als QA-/Debugpaket angenommen:58.639.777Bytes. Reguläre frische Paketierung entfernt belegte ungenutzte Containerbereiche, Original17 bleibt erhalten. [Human-APK8](../Release/HUMAN_ANDROID_BUILD8_INSPECTION.json)/Android06/Revision6/UI75 bleibt unverändert mit102Aufgaben; kein neuer Human-Build oder neue Studienkennung erklärt. Alte Messungen behalten ihre wirkliche Fassung.

Aktueller echter D3D11-Hinweisablauf und spätes10×10S1-05-04-06 mit richtigem Titel/REVISION unabhängig geprüft. Werkzeuge setzen/entfernen direkt,122Positionenortsfest; nachX-EntfernungBoardpixelgleichAusgang. Oberfläche unverändert durch Speicherfix. Keine Härte-/Telefon-/Humanannahme ausEditorbildern.

Voriger voller [PR-Lauf40143424066](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/40143424066) aufbcb5225 besteht unabhängig Linux1706/251mitunverändertenFällen/Fristen/0Retry/Coverage24 und [NativeAndroid](../Release/CI_ANDROID_40143424066_INSPECTION.json). IOSscheitert tatsächlichunterXcode26.3/SDK26.2anderrename-Deklaration; Fehloriginale bleiben erhalten. [Export5](../Release/IOS_EXPORT5_INSPECTION.json) prüftlokaldieKorrektur; neuevolle [CI40334134453](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/40334134453) aufd9da070 ist nach1706Editor-PASS imPlayMode249/251 fehlgeschlagen; Push ebenfalls249/251. Unterschiedliche Endlos-Fehlerbilder und Originale sind unabhängig gesichert. Gezielte Speicher-/Journal-Kostenmessung und Produktnachbesserung laufen, ohne Fristen/Assertions abzuschwächen; beide Nativeketten wurden übersprungen. Main18/Human8bleibenbestehendeAndroidpakete, kein automatischerAustauschdesanderssigniertenCI-APKs. AlleOriginalbindungeninder [Hauptübergabe](../PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/HANDOVER.md).

Offen bisM3: Owner-Spielannahme, echte Erstkontakte und Difficulty-/Sternzeit-/Profil-/Quellenfreigaben, physische Geräte-/Privacy-/Lifecycle-/Accessibility-/Performancebelege, reale Werbung/Käufe/Restore/Storekanäle, Signing und qualifizierte Rechtsprüfung. [Kurze Begleitliste](../Release/OWNER_INPUTS.md) nennt Beitrag und Bedarfszeitpunkt. Kein M1/M2/M3-, Main- oder Storeabschluss; vollständiger Auftrag bleibt aktiv.
