# Hauptübergabe – vollständiges Spiel und Release Candidate

Stand 22.10.2026. Releaselead R führt [WP-046](../../../WORK_PACKAGES/WP-046_Vollstaendiges-Spiel-und-Release-Candidate.md) unter [MANUS-2026-10-20-02](../../PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md) und dem direkten Ownerauftrag bis M3 weiter. M1/M2 sind Zwischenziele. Drei ausdrücklich beauftragte Agenten arbeiten im gemeinsamen Checkout; R besitzt das einzige Unityfenster.

## Verbindliche Ausgangsbasis

Development `codex/wp-036-vollstaendige-textbibliothek`, [Draft-PR31](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/31). Gemeinsamer unveränderter Manifest-/WP-Anker `39aaac5bdccfc612d71d012e556db80f96f2dcf9`, Parent `bd05da191cd2bf8da89830f7db6da4941cc62343`. Architecture v1.0 und historische Quellen/Manifeste bleiben erhalten. [Issue38](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/issues/38) bleibt offen; Texte gelesen, fehlende Originalmedien nicht als angesehen ausgegeben.

ADR037–047 sind angenommen. Der reine ADR023→047-Rückverweis besitzt eine gesonderte prospektive Autorisierung in Commit `213186388648f3478103346c8f604fa7858d4a50`, exakt nach `ab5c3e1` und vor der Zieländerung. Der [Authorityrecord](ADR023_047_METADATA_AUTHORIZATION.json) und der read-only Gitguard prüfen unveränderte Originalbytes und ausschließlich das feste EOFappend. Das historische WP046-Manifest wurde nicht erweitert. Der kanonische Scopeprüfer weist diesen einen Pfad getrennt nach.

Dateieigentum: A Presentation/UI/World/Art/Audio; B Application/Persistenz/Content/Generator/Editor.Content; C MobileServices/Platform/native SDKs/Releasevorlagen. R besitzt Bootstrap, Build, Szenen, Workflows, Projektsteuerung und zentrale Verträge. Echte neue Verträge brauchen vor abhängiger Implementierung einen dokumentierten ADR und getrennte Fachreview. Routinefragen entscheidet R. Keine Mikro-WP-Kaskade.

## Integrierter Spielumfang und Spielqualität

240 Rätsel in fünf gestalteten Welten/20 Routen, 720 Playertexte, 240 Karten-/240 Zugreferenzen und 14 Cosmetics. Eigene Cartoonumgebung, IBM-Plex/OFL, 15 eigene PCM-Clips, echter Mixer und gespeicherte Audio-/Haptik-/Accessibilityoptionen. R sichtete tatsächliche 4×4-/6×5-/10×10-Ansichten bei 390×844.

Acht direkte Werkzeuge: sechs Formen/X/Grau setzen, ersetzen und bei erneutem Tippen leeren; Undo, kein Mehrfachauswahl-/Anwendenmodus. Raster, Zellen, Randzahlen und Anschlüsse bleiben ortsfest. Timer beginnt mit Mutation. Erst bestätigter Save, dann tatsächliche Zugfahrt auf dem Spielerpfad, Ergebnis und bewusste Fortsetzung. Hinweisabbuchung erst nach bestätigtem Displayreceipt, Restart ohne Auffüllen. Tagesbetrag vorab dauerhaft festgelegt und dedupliziert; ältere Videoreservation verdeckt heutigen FREE nicht mehr.

Gemeinsamer serialisierter Savev3 für Fortschritt, Punkte, Bestwerte, Sterne, Übung/Revision, Sammlung/Kosmetikkauf/Auswahl und Hint-/Dailyreceipts. Vier technische DRAFT-Dauerbaustellenprofile: Reserve→begrenzte Worker-Suche→erneute Noveltyprüfung→persistierter ActiveDraft→Hint/Restart/Resume→Abschluss+15→optional Video+10/Skip→nächste Partie. Maximal20 offene Datensätze, monotone Ordinale, Recent50 und höchstens576 Economyreceipts. Strikter E1-Authoringreader, UNIQUE/Guess0 pro tatsächlich ausgegebenem Puzzle und vollständige Retainedbindung.

UMP/AdMob/Unity-IAP sind tatsächlich integriert. Consent und bestätigtes Profil bewachen den Dienststart, Grant liegt vor Nativefinalisierung. Expliziter Preisabruf und nachfolgender Kaufklick mit adaptereigenem unverbrauchtem Angebot; kein versteckter Refetch. Unbekannte Review-/Profil-/Produktdaten bleiben gesperrt; Offline-Spiel speichert weiter. Accessibilityhierarchie verwendet dieselben bewachten Aktionen, native Fenster pausieren Input über exakte ephemere Handles. Physische SDK-/TalkBack-/VoiceOverbelege fehlen.

Begrenzter lokaler Diagnosering ohne externe Telemetrie; ausdrücklich bestätigte profilgenaue Löschung mit Fence/Intent/Retry/Pendingboot und geschlossenem Invalidstartup. Keine tatsächlichen Nutzerprofile wurden durch Entwicklungswerkzeuge gelöscht. Öffentliche Rechts-/Supportziele werden getrennt serialisiert und erst nach bewusstem Klick geöffnet; echte Publisherziele sind im finalen Produktionsbuild erforderlich.

Sieben späte DRAFT-Geometrien gezielt verbessert, Ähnlichkeitskandidaten ersetzt und tatsächliche C#-Pipeline 240/240 bestanden. P05-Replay96/96 mit576 bytegleichen Payloads/96Identitäten/48Auswahlen; neuer Originalpool unter `Content/Quality/endless/p05-current-replay-2026-10-21/`. Menschliche Schwierigkeit und Sternzeiten bleiben offen. Keine erfundenen Zeiten, keine DRAFT-/Nullschwellenpromotion.

## Tatsächlich ausgeführte Prüfung

| Nachweis | Ergebnis und Grenze |
|---|---|
| Vollständiges Edit26, gezieltes Edit27 | 1622/1639;17 reine Reflection-/CRLF-Fixturefehler minimal korrigiert, exakt diese17 anschließend17/17 PASS. Kein einzelner Fullgreenlauf behauptet. |
| Gemeinsames Play27 | 50/50 PASS: sechs echte Root-Lösch-/Recoverywege, Kontakte, bisheriges Spiel und Humanintegration. |
| Kritische Editregressionen im Fullrun | ProfileDelete54, Ring35, Atomic301, Migration91, Publisher18, Daily7 bestanden; [Originalbindungen](../../../Release/WP046_CONTROLS_VALIDATION.json). |
| Gemeinsames Release-Edit28 | 224/224 PASS, Exit0: Packaging, native Entryguards, Sourcepreflight, Kalibrierung und Runtimecomposition; [gebundene Quellen/XML/Logs](../../../Release/WP046_RELEASE_TOOLING_VALIDATION.json). |
| Kontrollierte Releasewerkzeuge | 33 Manifest-,29 Inputbridge-,48 Packaging-,7 CI-Metadaten-,13 Credentialtests PASS. Zusätzlich24 Metadataauthoritytests. Keine echte Signierung oder Storeannahme daraus. |
| Tatsächliche Spielaufnahme | 17,29Sekunden WindowsEditor/D3D12/390×844;478Originalframes/479Videoframes,59Rideframes/34Pixelvarianten/32Positionen. R sah sieben decodierte Zustände. Kein Ton/APK-/Humanbeleg; [Originalbindungen](../../../Release/WP046_ADDITIONAL_VALIDATION.json). |
| Statische Android16-KiB-Prüfung | APK9 und Human6:14ARM64ELFs/52LOADsegmente, beide echten zipalign-Prüfungen bestanden. Kein AAB-/Play-Split-/physischer16-KiB-Lauf. |

Frühere Fehleroriginale bleiben erhalten. Insbesondere der echte Endlos-Reserve2-Rereadfehler ist mit eigenem striktem siebenfeldrigem Descriptorreader geschlossen; Save-/Hashformat unverändert. Negativtest-/Fixturekorrekturen ändern keine Produktassertionen oder Goldens. Einzelne gezielte Nachläufe ergänzen frühere Läufe, ersetzen deren tatsächliche Ergebnisse nicht.

## Aktuelle native Originale

**Aktuelle Haupt-APK10:** tatsächlich erfolgreich gebaut und unabhängig als QA-/Debugartefakt angenommen; [Inspektion](../../../Release/ANDROID_BUILD10_INSPECTION.json).58.628.605Bytes,SHA `85c3d1726cd29f8a67583cc01f89f5a5d12b7a37073cf60372f41034bb598364`. Beide Mergegates, DEX/24Icons,495CPP/12Player-Symbole und statische16-KiB-Prüfung bestanden.1849Native-/815Managed-IL2CPP-Originale erhalten. Vorbuild1114 Dateien, InventurSHA `d149a418103d40687c7c0278017c0098444d459a1eaf9cb853183f891e0bd986`, ArchivSHA `715545b54f7e741da5703fa63d6778606aa2e1d32f5b8144e60b461e254cfb2f`. Lokales APK `.utmp/wp046-native/android-build10-verified/stp-mobile-preview.apk`. Der Build prüft den normalen QA-Spielweg mit dem neuen globalen Nativeguard; keine Productionannahme.

| Weiterhin erhaltenes angenommenes Artefakt | Original und Bindung |
|---|---|
| Haupt-APK9 | `.utmp/wp046-native/android-build9-verified/stp-mobile-preview.apk`;58.628.601Bytes;SHA `8a6e9d0884803c8f8feaf5530225ff7a9c24184aae365775a34da99731ace467`; [Inspektion](../../../Release/ANDROID_BUILD9_INSPECTION.json). QA/debug/ARM64/min26/target36;1104Vorbuildquellen, beide Mergegates/DEX/Icons/12Player-Symbole unabhängig geprüft. |
| Human-APK6 | `.utmp/wp046-native/human-android-build6-verified/stp-human-calibration.apk`;56.809.131Bytes;SHA `07321ad6989cbdbad44b12c5ad59e33b388af7968c4efc9dcfecf6ea8b987265`; [Inspektion](../../../Release/HUMAN_ANDROID_BUILD6_INSPECTION.json).17Gruppen/102Aufgaben,Android04/Revision4/UI75 `43d3…`;17Scene-PPtrs/19TextAssets/native Exportprovider unabhängig geprüft. |
| iOS-Quellprojektexport4 | `.utmp/wp046-native/ios-export4-verified/ios-exported-project-source.zip`;364.613.735Bytes;SHA `99d1e6847af5c489078dc4f6b0b36438421a835afc5e933ccd6f3bd24805011d`; [Inspektion](../../../Release/IOS_EXPORT4_INSPECTION.json).3284Dateien/1104Quellen/PBX/Swift/StoreKit/Plist/19Icons unabhängig geprüft; kein Pods-/Xcode-/IPA-/Signingbeleg. |

Installierbare Dateien sind lokale ignorierte Originale, keine GitHub-Dateilinks. Historische APKs/Exporte und Inspektionen bleiben mit ihren ursprünglichen Bindungen erhalten; keine Umbenennung zum aktuellen RC. Vor Humanupdate/nächster Person bestätigten Lauf exportieren und alte APK/Quellen/lokale Daten bewahren. Unbekannte Versionen werden nicht gelöscht oder still migriert. Human-iOS bleibt gesperrt.

## Produktionspaketierung und Releasevertrag

[ADR047](../../../DECISIONS/ADR-047-produktionspaketierung-und-release-manifest-v2.md), [Pipeline-Runbook](../../../Release/RC_PIPELINE.md) und [Packagingwerkzeuge](../../../tools/release/RC_PACKAGING.md) sind integriert und getrennt geprüft. Android erhält einen tatsächlich unsignierten AAB mit erhaltenen Merge-/Native-/Symboloriginalen; separate Signierung prüft Signer, Manifest, Paketpayload und16-KiB-Bindungen. iOS bindet Podfile/Podlock, echtes Archive/Index, manuelles Profil und finales IPA einschließlich Signatur, Ressourcen/MachO-/dSYM-UUIDs. Temporäre Credentials kommen erst nach dem Nativebuild und werden auch bei Fehlern entfernt; keine vorhandenen Profile überschreiben.

Der Workflow verwendet echte getaggte CI-Identität und geschützte Inputartefakte sowie Required Reviewer im Environment `production`. Source-/IAP-/Publisher-/Profilgates bleiben geschlossen. Echte Buildnummer, Recipeblobbytes und build-info sind gebunden. Native Originale und getrennte Source-Evidence-Artefakte bewahren die13Player-/Runtime-/Rawlock-/Szenenbelege für spätere Prüfung;14Tage CIaufbewahrung erfordert rechtzeitige zusätzliche geschützte Sicherung.

Der [V2-Manifesterzeuger](../../../tools/release/rc_manifest.README.md) bindet qualifizierte Originalfreigaben und unabhängig geschützte technische Receipthashes. Vor einer tatsächlich autorisierten Storelieferung gibt es ausschließlich Packagingreceipts. Spätere reine Evidenznachfolger bewahren Vorgänger raw/JCS und denselben unveränderten Kandidaten; alle8Gates/3Blocker müssen tatsächlich geschlossen sein. V1-Schema/Beispiele/historische Reducer bleiben unverändert. Bisher kein tatsächlicher Production-AAB-/IPA-/Signing-/Storelauf, kein `promotionAllowed=true`-Claim.

## Git und tatsächliche Remote-CI

**Aktueller Nachlauf:** Beide Folgecommits bis `8215262799a840831a44a6721dfa5b7725c1c00a` normal gepusht, PR31 aktualisiert und weiter Draft. Architecture-PR39692999154/Push39692993669, Modulpreflight und alle drei Environmentjobs PASS. Unity-PR39692999153 ist terminal FAIL: zwölf verschiedene CS0234 wegen unbedingt referenzierter Android-Editor-Modultypen in ProductionNativeBuild/ProductionRcBuildContext; das Base-Linuximage besitzt dieses Modul nicht. Keine Edit-/Play-/Native-/Xcodeausführung in diesem Lauf. Vollständige dekodierte API-Joblogtextkopie `.utmp/wp046-ci-8215262/unity-compile-editmode.log`, SHA `483c6d751e8056ff451255297e92491e90d4919d5a49fd2ed9eea31282803dda`. B reproduzierte exakt den Modulbefund kontrolliert. Unity-Push39692993670 ist ebenfalls terminal FAIL mit denselben Stellen; seine dekodierte API-Logtextkopie `.utmp/wp046-ci-8215262/push-unity-compile-editmode.log`, SHA `72921239d9174d56e5c641f486c4b087911de5ff80b8a814d80bca64094ba8c0`, ist erhalten. Keine lokale Unityinstanz. Frühere akzeptierte APK-/Test-/Releasewerkzeugbelege bleiben unverändert an ihren ursprünglichen Quellen gebunden.

Der Owner bestätigte zunächst vier Commits bis4102f64 und später genau die drei Commits `a046656`, `0e93b54`, `ab5c3e1`. Normaler nicht erzwungener Push bis `ab5c3e172ed2ed6970499739dc6ee452a7d582ff` erfolgreich; PR31 ist als Draft mit konkretem Spielumfang und tatsächlichen CIfehlern aktualisiert. Kein Mainmerge, Tag oder Storeakt. Die nachfolgenden Releaseänderungen liegen lokal.

| Tatsächlicher Lauf an ab5c3e1 | Ergebnis |
|---|---|
| [Architecture39687236186](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39687236186) | FAIL: vier Markdownlinks auf ignorierte lokale APKs. Lokal durch wörtliche Pfade und eingecheckte Inspektionslinks korrigiert. |
| [Unity39687236366](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39687236366) | Konfigurations-/Environmentvorprüfungen PASS; ModulpreflightFAIL wegen zweier fehlender Testassembly-Einträge und csc.rsp. Unity/Edit/Play/Native/Xcode übersprungen. |
| Lokale Korrektur | 14Produktions-/11Testassemblies, exakte Allowlists und azyklischer Graph PASS; RuntimeGameCompositionTests samt Meta bytegleich in Calibration verschoben, keine Testassembly-zu-Testassembly-Kante. Beide Gruppen nullable/WarnAsError kompiliert, danach Unity Edit28 PASS. |

Original-Joblogs liegen unter `.utmp/wp046-ci-ab5c3e1/`; sie sind dekodierte GitHub-API-Logtextkopien, keine Original-ZIP-Downloads. Ältere CI39665718891/39665719215 bleiben mit ihren damaligen PASS-/FAIL-/Skipzuständen erhalten. Lokale Nachweise behaupten keinen neuen Remote-PASS.

## Enger Modulfix nach dem tatsächlichen CI-Befund

C-Paket43b24c864435320b024461207924b39b59e000dcc14b2a9d272da9043f9d00e7 nach unabhängiger B-Annahmecc723efe3a3818e11a42e2521832faaaf1a95d4b1e53dbae1a73d278adf8039a in genau drei bestehende Editor-/Testdateien integriert. Androidsymbole werden nur im Android-Kompilierungszweig gelesen/gesetzt; ein angeforderter Androidbuild ohne Modul wird vor dem Sourceimport geschlossen abgewiesen. iOS berührt keine Androidsymbolsettings. Android verlangt weiterhin tatsächliche Fullsymbole/ZIP, beide Restoreversuche und dieselben Receipt-/Original-/Buildinfomethoden. Kein Format-/Runtime-/Workflow-/Classifierwechsel.

Kontrollierte Matrix:16Compiles ohne Warnung/Fehler; Base ohne sämtliche PlaybackEngines-DLLs, iOS ausschließlich mit Xcode-DLL, Android ausschließlich mit Android-Modul-DLLs. Packagingfälle Base41/41, iOS41/41, Android39/39 und bestehende Nativefälle28/28 bestanden. B wiederholte unabhängig AltBase12CS0234 sowie korrigiertes Base/iOS/Android jeweils Compiler0. Kein tatsächlicher Linux-Unity-/SDK-Settinglauf nach dem Fix daraus behauptet. Private Integrationsbindung `.utmp/wp046-ci-8215262/modulefix-root-integration/integration.json`, SHA `aa0d08df945b74958d3b6f78630a72b03c169d276507d555272282871d3aa859`.

Die tatsächliche iOS-Environmentprüfung belegt Xcode26.3/Build17C529 und iOS26.2-SDK; `.utmp/wp046-ci-8215262/environment-ios.log`, SHA `7ca3cd734915d79a0b16b27ef30581ab40cec29563a808446f4b6e8ffd918bad`. Das belegt die vorhandene Macumgebung, noch keinen Xcodecompile/IPA. Alle bereits gepushten Release-/APK-/Test-JSONbelege bleiben unverändert mit ihren ursprünglichen Quellenbindungen. Die nächste tatsächliche PR-CI prüft den neuen Modulfix. Der unveränderte Classifier delegiert den bekannten Produktpush regulär an die PR; keine Rezept-/Unknown-Fallbacklockerung.

## Offene Gates und konkrete Fortsetzung

1. APK10 unabhängig angenommen und Originale erhalten; kanonische Architektur-/Scope-/Linkprüfung19/19, CIclassifier19/19 und Whitespaceprüfung PASS. Der Gitcommit dieser Übergabe sichert die geprüfte Union. Kein weiterer teurer Unitylauf nur wegen Dokumentation.
2. Passend autorisierten neuen Checkpoint normal zur bestehenden PR-CI übertragen. Tatsächliche Linux-Edit-/Play-/Coverage-/Android-/iOS-/macOS-Pods-/Xcodejobs beobachten und konkrete Befunde im selben Auftrag schließen. Während lizenzpflichtiger CI keine lokale Unityinstanz.
3. Echte Erstkontakte mit Human6 durchführen, lokale Originalexporte streng einlesen und Difficulty-/Sternzeiten-/Profil-/Quellenannahmen tatsächlich schließen. Erst dann echten Productionimport mit qualifizierten Originalen ausführen. Kein Nullschwellen-/DRAFTersatz.
4. Repräsentative Owner-Spielannahme, physische Android-/iOS-/Privacy-/Offline-/Lifecycle-/Accessibility-/Performanceprüfungen, reale Provider-/Kauf-/Restore-/Storekanaltests, Signing und qualifizierte Rechtsendprüfung schließen. Die [kurze Ownerliste](../../../Release/OWNER_INPUTS.md) nennt ausschließlich benötigte Eingaben und ihren Zeitpunkt. Unabhängige Entwicklung läuft weiter.

Mainintegration erst nach integrierter RCprüfung und tatsächlicher Owner-Spielannahme im geschützten PRverfahren; öffentliche Store-/Vertrags-/Zahlungshandlungen separat konkret autorisieren. Kein M1/M2/M3-PASS. Arbeitsziel vier Wochen, bedingter Korridor vier–sechs ab21.10.; anhand tatsächlicher Qualität/Abhängigkeiten spätestensTag3 neu planen, keine Garantie.

## Arbeitsumgebung und Bestandsschutz

Unity6000.3.23f1 unter `C:/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Unity.exe`, Android/iOSmodule/API36; .NET10.0.401; Architekturvenv `C:/STP/.venv-stp-architecture/`. Kein angeschlossenes ADB-Gerät oder physischer Test belegt. R startet lokale Unityinstanzen exklusiv und verborgen.

Vier vorhandene Nutzeränderungen byteerhalten und nicht stagen: GraphicsSettings, QualitySettings, PackageManagerSettings und URPProjectSettings. Bekanntes ShaderGraph-Whitespaceoriginal `.utmp/wp046-runtime/preexisting-ShaderGraphSettings.asset` erhalten. RootResources/ResolverXML sind SDKarbeitsdaten; nur bei geschlossenem Editor eng archivieren. Kanonischer IAPBillingMode bleibt GUIDgleich unter Settings/Resources. Historische Goldens/Manifeste unverändert; statisch falsches Git-M am PrecodecGolden ist kein Anlass zum Neuformatieren.

Vor der Verdichtung dieser Übergabe wurden ihre aktuellen Originalbytes zusätzlich unter `.utmp/wp046-r-production/docs-before-current-release-closeout/` erhalten; ältere eingecheckte Fassungen bleiben in Git nachvollziehbar. Der Gitcommit dieser Datei sichert den aktuellen dokumentierten Integrationsstand.
