# ADR-015 – Mobile Transaktionen und Privacy Default-Off

## Status

**Angenommen**

## Datum

2026-09-08

## Kontext

ADR-008 hat providerneutrale Mobile-Ports, sichere Fallbacks und die ausgewählten Adapter korrekt eingeführt. Drei Verträge waren nicht ausreichend präzise: Der einmalige `POST_CLEAR_PATIENCE`-Reward verwendete eine Provider-ID als fachlichen Deduplikationsschlüssel; der Trust- und Finalisierungsablauf für `remove_ads` war nicht vollständig; und Application-No-ops allein verhinderten keine native Datenerfassung vor Consent.

## Entscheidung

Die bestehenden Provider, ihre gepinnten Versionen und die Adaptertrennung bleiben erhalten. Providerneutrale Ports und Ergebnisverträge liegen gemäß ADR-013 in `STP.Application`.

### Rewarded Claim

Der fachliche Schlüssel lautet `reward-claim:POST_CLEAR_PATIENCE:<levelId>`. Er ist unabhängig von Provider-, Ad- und Operation-ID. Vor dem Anzeigen reserviert Application den Claim in einem atomaren Savecommit als `RESERVED` mit lokaler `operationId`. Nur eine Reservation darf existieren. Erst ein bestätigter Rewardcallback derselben Operation führt in **einem** Savecommit sowohl zu `COMMITTED` als auch zum Ledger-Eintrag mit derselben fachlichen Claim-ID. Provider-Reward-ID und Callbackzeit sind reine Auditfelder.

Parallele Anfragen werden nach erfolgreicher erster Reservation als `CLAIM_ALREADY_RESERVED` abgewiesen. Ein Terminalzustand ohne Reward gibt die Reservation atomar frei. Ein Crash oder unbekannter Ausgang erzeugt `RECONCILIATION_REQUIRED`; bis zu einer eindeutigen Replay-/Adapteraussage bleibt der Claim gesperrt und eine zweite Anzeige verboten. Ein später Callback darf den reservierten Claim genau einmal committen. Ein Callback nach bereits terminal freigegebener, neuerer Operation wird quarantänisiert und niemals automatisch zusätzlich verbucht.

### Clientseitiger IAP-Trust

Es wird kein Backend eingeführt. Der Trust-Vertrag ist bewusst client-only und auf das einzelne Non-Consumable `remove_ads` begrenzt. Ein Kaufbeleg ist nur akzeptabel, wenn der Storeadapter die plattformspezifische signierte Transaktion beziehungsweise das Receipt/Token lokal mit den offiziellen Store-/Unity-IAP-Mechanismen validiert und Produkt-ID, Bundle-/Package-ID, Environment, Transaktions-ID sowie Kaufstatus zur Buildkonfiguration passen. Rohreceipts und Tokens werden weder geloggt noch in Analytics geschrieben. Für Crash-Recovery speichert der app-private Operationsdatensatz nur die minimal notwendige Storetransaktionsreferenz; falls ein erneuter Beleg benötigt wird, wird er vom Store erneut abgefragt.

Die Reihenfolge ist verbindlich:

1. Callback/Restoreantwort empfangen und als `EVIDENCE_RECEIVED` persistent referenzieren.
2. Signatur/Receipt/Token und Produktbindung lokal prüfen; Fehler wird `REJECTED`, Unklarheit `RECONCILIATION_REQUIRED`.
3. Entitlement `remove_ads` und Operation `GRANTED_NOT_FINALIZED` atomar persistieren; ab diesem Commit sind Interstitials aus.
4. Erst danach Google-Purchase acknowledge beziehungsweise Apple-Transaction finish über Unity IAP auslösen.
5. Storeerfolg als `FINALIZED` persistieren. Timeout bleibt `GRANTED_NOT_FINALIZED` und wird mit exponentiellem, gedeckeltem Retry erneut finalisiert.

Ein Crash vor Schritt 3 führt durch Store-Replay/Restore zurück zu Validierung. Ein Crash zwischen 3 und 5 darf den Grant nicht duplizieren; die Transaktionsreferenz und das bereits aktive Entitlement machen Recovery idempotent. Ein Crash nach Storefinalisierung, aber vor lokalem `FINALIZED`, wird durch Ownership-/Transaktionsabfrage abgeglichen.

Restore nutzt dieselbe Validierungs- und Grantpipeline. Eine leere oder fehlgeschlagene Abfrage widerruft keinen zuvor bestätigten lokalen Anspruch. Nur eine eindeutige, plattformseitig verifizierbare Revocation/Refund-Aussage darf `REVOCATION_CONFIRMED` setzen. Widersprüchliche Storeantworten führen zu `RECONCILIATION_REQUIRED`; bis zur Klärung bleibt ein zuvor bestätigtes `remove_ads` konservativ aktiv.

### Privacy Default-Off

Alle optionalen externen Erfassungen sind **buildseitig und nativ default-off**. `canSendAnalytics` und `canSendCrashReports` sind bei Erstinstallation oder ohne gültigen persistierten Entscheid `false` und werden nur nach explizit bestätigter Capability sowie erfolgreicher Runtime-Aktivierung `true`. `UNKNOWN`, fehlende Konfiguration, Consentfehler und Abbruch bleiben `false`; eine bereits gültig persistierte Entscheidung darf offline weitergelten.

Firebase Analytics und Crashlytics erhalten die exakten Android-Manifest-/iOS-`Info.plist`-Schalter für deaktivierte automatische Sammlung. Analytics-ID-/Personalisierungssignale bleiben aus, bis die Capability explizit aktiviert wird. Crashlytics hält bei deaktivierter Collection laut Hersteller Berichte lokal; deshalb löscht eine dünne native Android-/iOS-Bridge nicht freigegebene Berichte vor jedem späteren Enable. Unity Analytics/Diagnostics und nicht benötigte Unity-Developer-Data-Pakete werden nicht eingebunden; Unity Services werden nicht automatisch initialisiert.

Unity IAP 5.4 wird lazy erst für sichtbare Kauf-/Restoreaktion oder persistente Recovery initialisiert. Laut Hersteller erhebt IAP 5.4 für seine Funktion immer Player ID, Unity Installation ID, Geräte-/Sessiondaten und Land und aktiviert Unity Authentication.[4] Diese notwendige Verarbeitung ist kein Analytics-Opt-in, muss aber vor Production in Privacy Policy, SDK-Inventar, Storedeklarationen und Rechtsgrundlage freigegeben sein. IAP hat keinen eigenen Consentdienst. Vor dieser Freigabe bleibt der Adapter nicht initialisiert. Google Mobile Ads wird vor `CanRequestAds() == true` weder initialisiert noch zum Laden verwendet; nur der notwendige UMP-Consentstatus-/Formfluss darf vorher kommunizieren.

Providerdashboards müssen automatische Datenerfassung, Signals/Personalization und Verknüpfungen soweit technisch verfügbar deaktivieren. Abweichungen sind Releaseblocker und werden im SDK-Inventar dokumentiert. Application puffert ohne Capability keine optionalen Analyticsereignisse. Ein späteres Opt-in versendet keine selbst erzeugten Vor-Opt-in-Events. Die SDK-eigene lokale Crashlytics-Erfassung wird nicht verschwiegen: Vor jedem späteren Enable werden alle während einer deaktivierten Phase erfassten Berichte gelöscht.

## Begründung

Fachliche Claim-IDs sichern die Produktregel unabhängig von Providerverhalten. Grant-before-ack/finish verhindert bezahlten Anspruchsverlust bei Crash und lässt die Storetransaktion bis zum erfolgreichen lokalen Commit wiederholbar. Clientseitige lokale Belegprüfung ist für das einzelne Non-Consumable angemessen, ohne den nicht beschlossenen Backendumfang einzuführen. Native Default-Off-Schalter schließen das Zeitfenster vor Application-Initialisierung.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Provider-Reward-ID als Ledger-ID | Dedupliziert Callbackreplays, aber nicht mehrere gültige Anzeigen für dasselbe Level. |
| Ack/Finish vor lokalem Grant | Kann bei Crash zu bezahltem Anspruch ohne lokalen Entitlementcommit führen. |
| Eigenes Receipt-Backend | Höherer Trust, aber für den bestätigten client-only Launchumfang nicht erforderlich und nicht beschlossen. |
| SDK erst nach Consent importieren | Buildseitig unmöglich; native Default-Off-Konfiguration und verzögerte Aktivierung sind erforderlich. |
| Nur No-op-Application-Port | Verhindert keine automatische native SDK-Erfassung. |

## Konsequenzen

Pending Reward- und IAP-Operationen sind Savebestandteil. `remove_ads` bleibt bei transienten Storefehlern aktiv. Productionbuilds benötigen Manifest-/`Info.plist`-, Dashboard- und physische Gerätetests. Kann ein SDK seine optionale Erfassung nicht nachweisbar deaktivieren, darf es nicht in Production aktiviert werden. Nicht abschaltbare, für eine ausdrücklich gestartete Kernfunktion notwendige IAP-Verarbeitung benötigt dagegen eine dokumentierte Freigabe und bleibt bis zur Nutzeraktion vollständig uninitialisiert.

## Betroffene Artefakte

`ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TEST_STRATEGY.md` und spätere Adapter.

## Validierung

Contracttests decken Reservation, Parallelität, Callbackreplay, Crash an jeder Phase, Quarantäne, Restore, Acknowledge/Finish, widersprüchliche Storeantworten und Revocation ab. Auf physischen Geräten oder einer ausdrücklich freigegebenen physischen Device-Farm wird ein Fresh-Install-Netzwerkmitschnitt vor Consent ausgeführt; Emulator/Simulator allein erfüllt dieses Gate nicht. Der Nachweis zeigt keine Analytics-, Crash-, Ad-Request- oder IAP-/Unity-Developer-Data-Übertragung vor Kauf, Restore oder Recovery. Ein Vor-Consent-Testcrash wird vor Aktivierung gelöscht und darf nie im Dashboard erscheinen.

## Ersetzt / ersetzt durch

Ersetzt [ADR-008](./ADR-008-mobile-service-ports-und-provider.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/MOBILE_SERVICES.md "Mobile Services v0.2"
[2]: ../ARCHITECTURE/OBSERVABILITY.md "Observability v0.2"
[3]: ../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md "WP-002 – Architecture-v0.2-Korrekturen"
[4]: https://docs.unity.com/en-us/iap/privacy-and-consent/overview "Unity IAP 5.4 Privacy overview"
