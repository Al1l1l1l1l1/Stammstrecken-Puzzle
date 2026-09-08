# ADR-008 – Mobile-Service-Ports mit Google- und Unity-Adaptern

## Status

**Ersetzt**

## Datum

2026-09-07

## Kontext

Werbung, In-App-Kauf, Consent, Analytics und Crashdiagnose sind externe, asynchrone und datenschutzrelevante Dienste. Sie dürfen weder den Offline-Rätselkern noch die Produktregeln steuern. Beide Stores benötigen Kauf- und Wiederherstellungsabläufe. Der Anbieter muss austauschbar und in Tests vollständig simulierbar sein.

## Entscheidung

`STP.Application` definiert kleine Ports: `IAdsPort`, `IPurchasePort`, `IConsentPort`, `IAnalyticsPort`, `ICrashReportingPort`, `IAudioPort`, `IAppLifecyclePort` und `INetworkStatusPort`. SDK-Typen dürfen diese Grenze nicht überschreiten. Jeder Aufruf liefert ein typisiertes Ergebnis mit Timeout, Abbruch und Fehlercode; kein `bool` trägt mehrere Bedeutungen.

Die initialen Adapter sind:

| Fähigkeit | Adapter |
|---|---|
| Unterbrechende und freiwillige Werbung | Google Mobile Ads Unity Plugin mit AdMob Mediation, initial auf Version 11.5.0 festzuschreiben.[1] |
| Werbe-/Privacy-Messaging | Google User Messaging Platform über denselben Plugin-Stack; Statusupdate bei jedem Start und `CanRequestAds` als hartes Gate.[2] |
| Nicht konsumierbarer Kauf `remove_ads` | Unity IAP **5.4.3**, exakt gepinnt.[3] |
| Produktanalytics | Firebase Analytics, standardmäßig deaktiviert bis zur Freigabe durch `IConsentPort`. |
| Crashdiagnose | Firebase Crashlytics, Sammlung gemäß freigegebener Consent-/Rechtsbasis-Konfiguration; IL2CPP-Symbole im Releaseprozess.[4] |

Nur der Application-Policy-Service entscheidet, ob eine Anzeige produktkonform zulässig ist. Der Ads-Adapter kennt weder Levelnummern noch Sterne. Bei fehlendem Consent, SDK-Fehler, Offlinezustand oder Timeout bleibt das Spiel vollständig spielbar. Belohnungen werden nur nach bestätigtem Reward-Callback idempotent vergeben.

## Begründung

Google Mobile Ads unterstützt Android und iOS über eine C#-Schnittstelle und bietet die benötigten Formate sowie Mediation.[1] Unity IAP abstrahiert Apple App Store und Google Play über eine gemeinsame API.[3] Die Ports verhindern Anbieter-Lock-in in Domain und UI und ermöglichen sichere Fakes.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Direkte SDK-Aufrufe aus Screens | Verstreute Policies, kaum testbar und hohes Datenschutzrisiko. |
| Unity LevelPlay als initiale Mediation | Geeignet, aber ein zweiter Anbieterstack neben Firebase/Google erhöht Integrationsbreite; per Adapter später austauschbar. |
| StoreKit und Google Billing direkt | Mehr Plattformcode ohne Produktvorteil für einen einzelnen nicht konsumierbaren Kauf. |
| Keine Analytics/Crashdiagnose | Datensparsam, aber unzureichend für Releasequalität und die bestätigten Messziele; deaktivierte Fallbacks bleiben verpflichtend. |

## Konsequenzen

Konten, App-IDs, Storeprodukte und Datenschutztexte sind vor Gerätetests erforderlich. SDK-Versionen werden exakt gepinnt und quartalsweise auf Storekonformität geprüft. Rechtsgrundlage und endgültige Consent-Texte bleiben eine rechtliche/produktbezogene Freigabe, keine Architekturentscheidung.

## Betroffene Artefakte

`ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/PERSISTENCE.md` und `ARCHITECTURE/BUILD_AND_RELEASE.md`.

## Validierung

Contract-Tests laufen gegen Fakes und echte Sandboxadapter. Gerätetests decken Consent akzeptiert/abgelehnt/nicht verfügbar, keine Füllung, Reward-Abbruch, Kauf, Pending-Kauf, Duplikat, Restore und Offline-Start ab.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ersetzt durch [ADR-015](./ADR-015-mobile-transaktionen-und-privacy-default-off.md).

## Referenzen

[1]: https://developers.google.com/admob/unity/quick-start "Set up Google Mobile Ads Unity Plugin"
[2]: https://developers.google.com/admob/unity/privacy "Set up UMP SDK for Unity"
[3]: https://docs.unity3d.com/Packages/com.unity.purchasing@5.4/changelog/CHANGELOG.html "Unity IAP 5.4 changelog"
[4]: https://firebase.google.com/docs/crashlytics/unity/get-started "Get started with Crashlytics for Unity"
