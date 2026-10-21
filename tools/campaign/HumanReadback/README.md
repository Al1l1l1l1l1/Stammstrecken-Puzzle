# Interner Humanexport-Readback

Dieses reine Authoringwerkzeug liest vorhandene ADR042-Beobachtungsexporte. Es öffnet keinen Spielstand, ruft keinen Provider auf und ändert weder Spielquellen noch Assignments. .NET 10 wird benötigt; externe NuGet-Pakete werden nicht verwendet.

Der Build verwendet direkt die vorhandenen Domain-, Application-, Content- und Persistencequellen, insbesondere `HumanCalibrationCodec`, `HumanAssignmentCatalog`, `StrictJsonParser` und JCS. Deshalb muss `StpRepositoryRoot` ausdrücklich den gewünschten Quellstand nennen. Keine zweite Save-/Hashprojektion entsteht.

```powershell
dotnet build tools/campaign/HumanReadback/HumanReadback.csproj `
  -p:StpRepositoryRoot=C:/STP/Stammstrecken-Puzzle-WP008

dotnet tools/campaign/HumanReadback/bin/Debug/net10.0/HumanReadback.dll `
  --source-root C:/STP/Stammstrecken-Puzzle-WP008 `
  --packs Assets/StammstreckenPuzzle/UI/HumanAssignments `
  --public-catalog Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json `
  --public-bindings Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json `
  --source-bindings Content/Quality/human/assignment-source-bindings.json `
  --exports C:/HumanOriginale `
  --out C:/HumanAuswertung/neuer-eindeutiger-Ordner
```

Für ältere Studienpakete muss `--source-root` deren erhaltenes tatsächliches Quellenpaket enthalten. Ein neuer UI-/Level-/Profilstand wird nicht still als alte Messquelle akzeptiert. Der Quellroot ist eine ausdrückliche Angabe des Aufrufers; Toolassemblyhash, Rohdateihashes und geprüfte Quellen werden im Ergebnis genannt.

Die Beobachtungs-App ist ebenfalls versionsgebunden. Den vollständigen Beobachtungsexport des aktiven Laufs vor einem Appupdate ausdrücklich noch aus der passenden Version erzeugen; abgeschlossene Läufe bereits vor dem Wechsel zur nächsten Person exportieren. Auch unterbrochene Beobachtungen erhalten. Die unveränderten Exportbytes sichern und mit deren erhaltenen Quellen erfolgreich zurücklesen. Der lokale Export umfasst jeweils den aktuell bestätigt gebundenen Lauf, kein automatisches Gesamtarchiv. Zugehörige APK, Quellenpaket und lokale Originale aufbewahren. Ein neuer Build kennt frühere Manifesthashes möglicherweise nicht: `Unsupported` erhält die Daten, erlaubt aber weder Fortsetzung noch Operator-Export. Ein Update führt keine Migration oder automatische Rücksetzung aus.

Eine neue Messreihe setzt eine kontrolliert frische Studienumgebung für genau ihre APK-/Manifest-/UIversion voraus. Vorherige Originale und verifizierte Exporte müssen getrennt erhalten bleiben; Installation, Datenlöschen oder ein neuer Teilnehmerlauf ersetzen diese Sicherung nicht. Bei bereits unbekannter Version keine Daten löschen oder mit neuen Quellen fortsetzen. Der Readback verwendet für jeden Export weiterhin dessen ursprüngliches Quellenpaket.

Eingänge sind der vollständige Teilnehmerordner mit Rawlock/UIlock/zugehörigen Manifesten, der dazugehörige aktuelle öffentliche Korpus samt Binding und die internen portablen Producer-Quellenbindungen. Das Tool prüft tatsächliche UIquellen, alle zugewiesenen Level-/Kandidatenbytes, Authoringrevisionen, öffentliche Identitäten und P05-Profil-/Descriptor-/Poolbindungen. Teilnehmerassets erhalten dadurch keine internen Lösungen oder Notizen.

`--exports` liest höchstens 512 direkte JSON-Dateien. Der Ausgabeordner muss neu sein. Originalexportbytes werden unter ihrem SHA256 unverändert erhalten, auch bei Codec-Ablehnung; exakte Dateiduplikate werden nur einmal gezählt. Quellenlocks werden separat kopiert. Die interne Datei `readback.internal.json` nennt für jede Beobachtung die tatsächliche gespeicherte aktive Domainzeit und nullable Previewzeit, Quellen, Selbstbericht, Hint-/Unterbrechungsfakten und konkrete Ausschlussgründe.

Automatisierte, nicht gelöste, unterbrochene, zeitlich unvollständige, bekannte/unsichere/vorversionsbezogene, wiederholte und möglicherweise oder bestätigt unterstützte Beobachtungen werden separat ausgewiesen. Mehrere unterschiedliche Exporte derselben Session sowie wiederverwendete Attempt-/Exposure-IDs gelangen nicht automatisch in die unassistierte Erstkontaktteilmenge. Das Tool bevorzugt keine höhere Generation; eine bewusste Auswahl muss mit erhaltenen Originalen gesondert nachvollziehbar erfolgen.

Auch `PARTICIPANT_UI` und `NEVER_SEEN` sind nur unbeglaubigte lokale Angaben. Die übrige Teilmenge heißt ausdrücklich `UNATTESTED_UNASSISTED_FIRST_CONTACT_OBSERVATION`. Das Tool attestiert keine menschliche Identität, Geräteannahme oder vollständige Lösezeit, erzeugt keine Sternzeiten/Profilefreigaben und erlaubt keine Promotion. Reviewer-/Geräteannahme und die fachliche Kalibrierung bleiben extern.

Exitcodes: `0` = technisch ausgewertet, auch wenn alle Beobachtungen ausgeschlossen sind; `2` = mindestens ein Originalexport abgelehnt und im Bericht erhalten; `1` = Quellen-/Argument-/I/Ofehler, keine belastbare Auswertung. Ein technischer Exitcode ist kein Human-PASS.

Die kontrollierten adversarialen Tests laufen mit denselben Quellenargumenten und `--self-test <neuerScratchordner>` anstelle von `--exports`/`--out`. Sie erzeugen ausdrücklich automatisierte/synthetische Fixtures einschließlich eines gefälschten `PARTICIPANT_UI`-Labels zur Prüfung der fehlenden Personenattestierung. Ihre Zeiten stammen aus kontrollierten Domaincommands und sind keine menschlichen Messwerte.
