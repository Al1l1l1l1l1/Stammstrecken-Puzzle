# ADR-024 – Privacy-Bootstrap-Fence und crashsicherer Widerruf

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

ADR-020 trennt lokale Privacy-Entscheidung, gewünschte Capability und wirksamen Providerzustand. Der dort beschriebene Reset-only-Releasepfad setzt jedoch voraus, dass ein bestimmtes Zwischenbinary mindestens einmal gestartet wurde. Ein Gerät kann von einem älteren Build mit persistiertem Firebase-Analytics-Override `true` direkt auf einen späteren re-enable-fähigen Build springen. Ein Reset-only-Build kann außerdem installiert, aber nie gestartet worden sein.

Beim Widerruf entsteht ein zweites Crashfenster: Wird zuerst `REVOKED` gespeichert und danach der native Override deaktiviert, können lokale Entscheidwahrheit und persistenter Providerzustand auseinanderfallen. Application-`effective=false` nach Start ist kein Beleg dafür, dass ein SDK nicht bereits vorher erfasst hat.

## Entscheidung

Firebase Analytics besitzt im Productionprofil keinen allgemeinen Re-enable-Pfad, solange für die gepinnte Android- und iOS-Integration kein reproduzierbarer **prä-SDK-Deny-Fence** belegt ist. Dieser Fence muss vor jeder möglichen Analytics-Initialisierung oder -Erfassung einen beliebigen persistierten Runtime-Override neutralisieren. Installation oder Ausführung eines bestimmten Zwischenbuilds darf keine Voraussetzung sein.

Der versionierte Privacyzustand enthält zusätzlich `nativeSyncState` mit genau einem Wert:

- `DENIED_CONFIRMED`: Native Sammlung ist für die aktuelle `sdkContractRevision` vor SDK-Erfassung nachweislich deaktiviert.
- `ENABLE_PENDING`: Ein neuer gültiger Entscheid liegt vor, aber die native Aktivierung ist noch nicht bestätigt.
- `ENABLED_CONFIRMED`: Aktivierung wurde nach aktuellem Entscheid und erfolgreichem Fence bestätigt.
- `REVOKE_PENDING`: Widerruf ist lokal dauerhaft angefordert; Native Disable ist noch nicht bestätigt.
- `REVOKED_CONFIRMED`: Native Deaktivierung wurde bestätigt.
- `UNKNOWN`: Zustand fehlt, ist beschädigt oder passt nicht zur aktuellen Revision.

Bei Prozessstart gilt zuerst ein bootstrap-nativer Deny-Fence. Erst nach dessen Erfolg wird der Record gelesen. `UNKNOWN`, `REVOKE_PENDING`, revisionsinkompatible Records und jeder Synchronisationsfehler halten Analytics aus. Ein persistierter Provideroverride ist ausschließlich untrusted input.

Ein Widerruf wird in folgender Reihenfolge ausgeführt:

1. Ads und Analyticsports werden sofort als No-op geschaltet; geladene Anzeigen werden verworfen.
2. `REVOKE_PENDING` wird atomar persistiert.
3. Native Analyticscollection wird deaktiviert und ihr Erfolg überprüft.
4. Erst danach wird `REVOKED_CONFIRMED` atomar persistiert.

Ein Crash nach jedem Schritt führt beim nächsten Start erneut durch den prä-SDK-Deny-Fence und danach durch die ausstehende Native-Disable-Reconciliation. Ein verspäteter Providercallback kann keinen Widerruf rückgängig machen.

Re-enable benötigt einen neuen revisionsaktuellen Entscheid, aktuelle Privacy-/Personalisierungssignale, `ENABLE_PENDING`, erfolgreichen Native Apply und erst danach `ENABLED_CONFIRMED`. Bis zur Bestätigung bleibt die Application-Capability `false`. Offline kann nur ein bereits für dieselbe Policy- und SDK-Revision bestätigtes `ENABLED_CONFIRMED` weitergelten; Ads benötigen weiterhin den aktuellen UMP-Startfluss.

Für Firebase Analytics 13.16.0 ist im vorliegenden Repository kein plattformbezogener prä-SDK-Fence für direkte Legacy-Upgrades belegt. Daher bleibt Analytics in Production **deaktiviert**, einschließlich eines späteren Re-enable-Builds, bis ein separates Implementierungs-/Release-Work-Package diesen Nachweis auf beiden Plattformen erbringt. Das ist die Anwendung des bestehenden Fail-closed-Prinzips und keine neue Produktentscheidung.

Ads bleiben an UMP gebunden. Crashlytics bleibt in Production ausgeschlossen. Unity IAP bleibt lazy und readiness-gesteuert. Diese Teile von ADR-020 werden nicht geändert.

## Begründung

Nur ein vor möglicher SDK-Erfassung wirksamer nativer Mechanismus kann einen alten persistierten Override sicher neutralisieren. Die zusätzliche durable Synchronisationsphase macht Crashfenster explizit und recoveryfähig. Der Ausschluss des unbewiesenen Re-enable-Pfads verhindert, dass eine Dokumentannahme als Datenschutzgarantie ausgegeben wird.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Reset-only-Zwischenbuild voraussetzen | Nicht sicher, weil Installationen übersprungen oder nie gestartet werden können. |
| Application-Capability beim Start auf `false` setzen | Zu spät, wenn das SDK vor Application-Synchronisierung erfasst. |
| Persistierten Provideroverride als Entscheid lesen | Providerzustand ist keine Nutzer- oder Policywahrheit. |
| Widerruf ohne `REVOKE_PENDING` | Lässt das Crashfenster zwischen lokalem Record und Native Disable unmodelliert. |
| Analytics trotz fehlendem Fence aktivieren | Widerspricht dem bestehenden Fail-closed-Grundsatz. |

## Konsequenzen

Der Production-Analyticspfad bleibt bis zum realen prä-SDK-Nachweis deaktiviert. Die lokale Architektur kann Zustandsübergänge, Reihenfolge und Fail-closed-Regeln prüfen. Native Konfiguration, direkte Upgradepfade, Crashinjektion und Netzwerkverkehr bleiben physische Release-Gates und dürfen nicht als lokaler PASS erscheinen.

`PrivacyDecisionRecord` und Save bleiben bounded. Es entsteht kein Providercache als fachliche Wahrheit und keine Vorfreigabe-Eventqueue.

## Betroffene Artefakte

`ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/PRIVACY_PROVIDER_EVIDENCE.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TEST_STRATEGY.md`, Privacy-Fixture und Architekturvalidator.

## Validierung

Ein ausführbarer Referenzreducer prüft Fresh Install, kompatiblen Restart, direkten Legacy-Sprung, installierten aber nie gestarteten Reset-only-Build, Widerruf mit Crash nach jedem persistenten Schritt, Recovery, Re-enable-Versuch und Offlinezustand. Mutationen von Inputs, Reihenfolge, Fence, Native-Bestätigung und erlaubten Effekten müssen scheitern. Physische Android- und iOS-Captures bleiben `REQUIRED_LATER/NOT_EXECUTED`.

## Ersetzt / ersetzt durch

Ersetzt die Analytics-Upgrade-, Re-enable- und Widerrufsanteile aus [ADR-020](./ADR-020-privacy-lifecycle-und-sdk-grenzen.md). Dessen Ads-, Crashlytics- und IAP-Grenzen bleiben angenommen. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://firebase.google.com/docs/analytics/android/configure-data-collection "Firebase Analytics Android collection controls"
[2]: https://firebase.google.com/docs/analytics/ios/configure-data-collection "Firebase Analytics Apple collection controls"
[3]: ../ARCHITECTURE/PRIVACY_PROVIDER_EVIDENCE.md "Privacy Provider Evidence für Architecture v0.4"
[4]: ../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md "WP-004"
