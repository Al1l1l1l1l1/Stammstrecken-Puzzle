# Produktionskandidaten bauen und erhalten

Die Pipeline `.github/workflows/release.yml` paketiert einen tatsächlich freigegebenen Produktionsstand für Android und iOS. Sie erstellt keine Tags, lädt nichts zu einem Store hoch und veröffentlicht nichts. Ein technischer Signingbericht schließt keine menschliche, rechtliche oder Gerätefreigabe. Maßgeblich ist [ADR047](../DECISIONS/ADR-047-produktionspaketierung-und-release-manifest-v2.md).

## Vor dem tatsächlichen ersten Lauf

R stellt die folgenden technischen Eingaben zusammen. Der zuständige Publisher beziehungsweise Signingverantwortliche stellt Rollen, geschützte Schlüssel und die konkrete Befugnis zum Signieren bereit; keine Secrets im Chat oder Repository.

1. Der geprüfte Commit enthält den vollständig angenommenen ProductionImport-Auftrag, die unveränderten tatsächlichen Quellen-/Kalibrierungs-/Profilfreigaben und die passenden plattformspezifischen Productionpresets. QA-/Human-/DRAFT-Quellen reichen nicht. Der vollständige bestehende Import-/IAP-/Publisherpreflight wird vor nativer Produktion ausgeführt.
2. Ein tatsächlich eingerichtetes GitHub-Environment `production` besitzt mindestens einen zuständigen Required Reviewer. Beide Plattformjobs verwenden dieses Environment; die Bridge prüft dessen reale Regel zusätzlich lesend. Eine vorhandene Regel behauptet keine fachliche Human-/Rechtsannahme. Der konkrete Signierlauf wird im Environment genehmigt.
3. Ein geschütztes Inputartefakt je Plattform enthält ausschließlich `rc-inputs.json` und das erforderliche echte IAPreview-Original. Die zuständige Vorbereitung bindet Repository, erfolgreichen Quellrun, annotierten RCtag, Commit und den Raw-SHA256 des ZIPs. Die [Bridgebeschreibung](../tools/release/RC_CI_INPUTS.md) enthält das genaue Format. Ein Offline-ZIP ist kein Online-CIfallback.
4. Ein annotierter Tag `vX.Y.Z-rc.N` benennt den tatsächlich geprüften Kandidatencommit. Er muss für diesen ersten Kandidaten noch nicht auf Main liegen; das Gesamtmandat verlangt die integrierte RCprüfung und Owner-Spielannahme vor Mainintegration. Ein neuer Tag oder Signierlauf ist eine konkrete externe Handlung.
5. Für iOS ist der tatsächliche freigegebene `Release/IosPods/Podfile.lock` bereits im Tag enthalten. Sein Rawhash und der erwartete exportierte Podfile-Rawhash sind geschützt vorgegeben. Eine neue ungebundene Podsauflösung ersetzt diesen Lock nicht. Der feste Macrunner benötigt Xcode26.3/iOS26-SDK und CocoaPods1.16.2; fehlende Voraussetzungen scheitern geschlossen.

`Toolchain.lock.md` bleibt die historische WP021-Baseline; sein früherer Hinweis auf noch nicht eingebaute Mobile-SDKs ist keine aktuelle Produktionsinventur. Ein neuer tatsächlicher RC-Toolchainlock wird aus den realen Paket-/Editor-/JDK-/Gradle-/Xcode-/Podbelegen unter `Release/` erhalten und im ProductionImport-Auftrag sowie ReleaseLock gebunden. Vorhandene [SDKinventur](SDK_INVENTORY.md) und Originalbelege sind dabei zu verwenden; unbekannte Werte werden nicht geraten.

## Geschützte CI-Eingaben

| Zweck | Namen im Environment `production` |
|---|---|
| Androididentität und Inputquelle | `STP_RC_ANDROID_APPLICATION_ID`, `STP_RC_ANDROID_INPUT_RUN_ID`, `STP_RC_ANDROID_INPUT_ARTIFACT_ID`, `STP_RC_ANDROID_INPUT_ZIP_SHA256` |
| Android-Uploadsigner | Vars `STP_RC_ANDROID_KEY_ALIAS`, `STP_RC_ANDROID_UPLOAD_CERTIFICATE_SHA256`; Secrets `STP_RC_ANDROID_KEYSTORE_BASE64`, `STP_RC_KEYSTORE_PASSWORD`, `STP_RC_KEY_PASSWORD` |
| iOSidentität und Inputquelle | `STP_RC_IOS_APPLICATION_ID`, `STP_RC_IOS_INPUT_RUN_ID`, `STP_RC_IOS_INPUT_ARTIFACT_ID`, `STP_RC_IOS_INPUT_ZIP_SHA256` |
| iOSsigner und Locks | Vars `STP_RC_IOS_TEAM`, `STP_RC_IOS_PROFILE_UUID`, `STP_RC_IOS_CERTIFICATE_SHA256`, `STP_RC_IOS_PODFILE_SHA256`, `STP_RC_IOS_POD_LOCK_SHA256`; Secrets `STP_RC_IOS_P12_BASE64`, `STP_RC_IOS_P12_PASSWORD`, `STP_RC_IOS_CERTIFICATE_BASE64`, `STP_RC_IOS_PROFILE_BASE64` |
| Bereits bestehende Unitylizenz | Secrets `UNITY_EMAIL`, `UNITY_PASSWORD`; beide Jobs teilen mit der normalen CI dieselbe exklusive Seatgruppe. |

Zertifikatshashes sind echte DER-Raw-SHA256-Werte. Android benennt ausdrücklich den Uploadsigner; der Play-App-Signing-Signer einer später ausgelieferten APK ist eine eigene Rolle. Apple verwendet ein tatsächliches Distributionprofil. Die Pipeline legt Schlüssel erst nach erfolgreichem Productionbuild in einer frischen temporären Wurzel an. Sie überschreibt keine bestehenden Profile, stellt die vorherige Keychain-Suchliste wieder her und bereinigt auch Fehlerpfade. Schlüssel-/Inputwurzeln gehören nicht zu den Artifactpfaden.

## Ergebnis und nachfolgende Prüfung

Beide Plattformen desselben Workflowruns verwenden die Buildnummer `run_number*1000+run_attempt` mit Attempt1–999. Die Storeversion bleibt X.Y.Z. Das beweist keine Monotonie gegenüber einem unbekannten Storekonto; dessen tatsächlicher Anfangs-/Vorversionszustand wird vor einer späteren Storelieferung separat gebunden.

Die Pipeline erhält je Plattform Metadaten, native Originale, das tatsächlich signierte AAB beziehungsweise IPA, Symbole, Signatur-/Paketberichte und Toolchainbelege. Die exakten neun Identitäts- und vierzehn Buildinfofelder sind vor dem Import gebunden; Recipepfad und echte Workflowblobbytes werden geprüft. `build-info.json` wird ausschließlich im generierten nativen Output ergänzt und im endgültigen Paket bytegeprüft. Ein Quellprojektexport bleibt ein Quellprojektexport.

Ein separates Source-Evidence-Artefakt bewahrt ausschließlich `.utmp/production-preflight/`, die generierte `ProductionRuntime/`-Kopie und `ProductionGame.unity` samt Meta. Die versteckte Auditwurzel wird ausdrücklich eingeschlossen; weitere `.utmp`-Verzeichnisse, geschützte Inputpakete und Schlüssel sind keine Uploadpfade. Der tatsächliche Importversuch enthält `import/player/Runtime.bindings.json`, alle 13 Playeroriginale, den Rawlock, das Quelleninventar und Prozessbelege. Ein späterer Prüfer bindet den tatsächlichen Runtime-Rawhash unabhängig an diese Originale. Das Quelleninventar ersetzt keine Human-, Rechts- oder Authoringoriginale; diese bleiben über relative Quellenpfade und Rawhashes im exakten Tag gebunden.

Die [Packagingwerkzeuge](../tools/release/RC_PACKAGING.md) prüfen die tatsächliche finale Signatur, App-/Versionsidentität, Native-/Privacybindungen und Symbole. Android bewahrt gebundene Mergeoriginale, AAB und Symbole für nachgelagerte Prüfung. iOS bewahrt das tatsächliche Archive mit seinem vor Export gebundenen Index; finale IPA-Code-/Ressourcen-/UUID- und dSYMzuordnung werden daran geprüft. Ein neues Ergebnisverzeichnis pro Versuch verhindert das Überschreiben alter Originale. Fehlversuche bleiben als solche erhalten.

Die GitHub-Artefakte werden 14 Tage aufbewahrt. R sichert die vollständigen benötigten Originale einschließlich Symbolen und Archive rechtzeitig zusätzlich in der zuständigen geschützten Releaseablage, bevor diese Frist abläuft. Ein nachgelagerter Prüfer verwendet die Originalhashes und die read-only Verifier; er baut oder signiert den angenommenen Kandidaten nicht erneut.

Ein V2-Storemanifest entsteht erst nach einer tatsächlich autorisierten internen Storelieferung mit unveränderlicher Storebuildreferenz und originaler Zuordnung vom Upload zur ausgelieferten App. Dafür gelten die [Manifestwerkzeuge](../tools/release/rc_manifest.README.md): geschützte unabhängige Receipthashes, qualifizierte Originalfreigaben und exakte Kandidatenbindung. Alle acht Evidenzgates und drei Produktblocker müssen tatsächlich geschlossen sein, bevor `promotionAllowed` berechnet true werden kann. Spätere Evidenz erzeugt nur einen erhaltenen, konkret vorgängergebundenen Manifestnachfolger bei unverändertem Kandidaten. Die konkrete öffentliche Handlung bleibt eine weitere ausdrückliche Ownerfreigabe.

## Tatsächlicher Ausführungsstand

Am22.10.2026 sind die Quellen und gezielten kontrollierten Prüfungen integriert. Die gemeinsame tatsächliche Unity-EditModeprüfung hat224/224 Fälle bestanden; die130kontrollierten Releasewerkzeugtests sind bestanden. [Originalbindungen](WP046_RELEASE_TOOLING_VALIDATION.json) trennen diese Nachweise von echter Produktionsausführung. Ein tatsächlicher Releaseworkflow mit realen Schlüsseln, Production-AAB, Xcode-Archive/IPA oder Storelieferung wurde noch nicht ausgeführt. Die aktuellen installierbaren QA-/Humanpakete und ihre Originalbindungen bleiben in der [Hauptübergabe](../PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/HANDOVER.md) ausgewiesen.
