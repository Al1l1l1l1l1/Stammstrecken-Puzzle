# ADR-038 – Mobile Provider Runtime Contracts

## Status

**Angenommen** – 21.10.2026 nach getrennter fachlicher Review durch Bahn B gegen ADR-020/024 und prospektiver technischer R-Annahme im WP-046-Mandat. Keine blockierenden Vertragsbefunde; Implementierungs-, Sandbox-, Geräte- und Rechtsnachweise bleiben gesondert offen.

## Datum

2026-10-21

## Kontext

WP-046 setzt P03/P04/P07 des vollständigen Releaseauftrags um. Die drei Application-Ports für Ads, Consent und Käufe sind bislang leere Interfaces; die Adapter sind SDK-freie Skelette. Die bestehenden ADR-020/024-Grenzen bleiben verbindlich. Eine Implementierung benötigt einen einzigen providerneutralen Callback-, Abbruch- und Finalisierungsvertrag, ohne neue Assembly oder neues Backend.

## Entscheidung

1. `STP.Application` besitzt unveränderliche mobile Requests/Resultate und die Mitglieder von `IConsentPort`, `IAdsPort`, `IPurchasePort`. Jeder Request enthält eine lokale eindeutige Operation-ID, eine positive begrenzte Timeoutdauer und Cancellation. Status sind `Succeeded`, `Unavailable`, `NotAllowed`, `Cancelled`, `TimedOut`, `Pending`, `Failed`; Diagnosecodes sind feste lokale Codes ohne SDKtexte/Secrets. SDKtypen, Rohreceipts, Kauf-/Adtokens und Exceptions verlassen den Adapter nicht. Der Konstruktor erhält einen expliziten Hauptthread-Dispatcher; kein Service Locator.
2. Consent startet effective false. UMP wird je Prozessstart aktualisiert; nur ein erfolgreich abgeschlossener aktueller Update-/Formfluss und `CanRequestAds` können Ads freigeben. Fehler/Abbruch/Timeout bleiben aus. Privacyoptionen sind als sichtbarer UIeinstieg verfügbar, wenn UMP sie verlangt. Beim Öffnen der Optionen werden Ads sofort gesperrt und geladene Anzeigen verworfen. Ein Generationstoken verhindert die Reaktivierung durch ältere Callbacks. Analytics/Crash bleiben false und paketmäßig ausgeschlossen. UMP ist keine IAPfreigabe. Persistente Privacyrecords werden ausschließlich über Application/Save geführt; kein UMPstring oder Identifier wird gespeichert.
3. Ads besitzen getrennte Load-/Showoperationen und eindeutige geladene Handles. Alle Requests sind nicht personalisiert (`npa=1`); optionale Publisher-First-Party-ID wird deaktiviert. Kein automatischer Load/Retry im Bootstrap oder nach Fehler. Application entscheidet Zeitpunkt, Modus, Taktung, modal-/Werbefrei-/Reservationberechtigung. Ein Rewarded-Show setzt eine bereits persistierte fachliche Reservation voraus; ein Rewardcallback wird nur als unveränderlicher Nachweis mit Operation-/Reservation-ID gemeldet. Die Punkte-/Hint-/Dailyvergabe bleibt ein atomarer Applicationcommit.
4. Jede Operation hat genau ein Aufruferresultat. Timeout/Cancel eines bereits begonnenen Shows/Kaufs behauptet keinen Abbruch beim Anbieter und keinen Rewardverlust. Spätere geld-/rewardrelevante Nachweise werden zusätzlich als deduplizierbare Recoveryevents zugestellt, auch nach dem Aufrufresultat; sie enthalten die ursprüngliche Operation-/Reservationbindung. Keine zweite Vergabe aus dem Event selbst. Ein neues Show wird während unklarem altem Showzustand abgelehnt; Application hält offene Reservations gemäß bestehendem Vertrag in Reconciliation. Widerruf verwirft Loads und verhindert neue Shows, verwirft aber keinen bereits tatsächlich verdienten Nachweis.
5. IAP startet ausschließlich nach sichtbarer Kauf-/Restoreaktion oder expliziter persistenter Recovery **und** revisionsgebundener `IapPrivacyReadiness`. Ohne Readiness erfolgt kein `Connect`, kein Produkt-/Kaufabruf. Genau ein logischer Non-Consumable `remove_ads`, profilgetrennte Store-ID und lokalisierte Storemetadata; keine Remote-Catalog-/D2C-/Authenticationinitialisierung durch Appcode. Die tatsächlichen unvermeidbaren SDKdatenflüsse bleiben Freigabegate.
6. IAP meldet nur lokal verifizierte, konfigurationsgebundene Evidence (Store, Environment, App-ID, Produkt, nicht reversibler Transaktionsschlüssel, opaque Prozesshandle). Googlebelege werden lokal mit dem konfigurierten Play-Lizenzschlüssel kryptografisch sowie auf Produkt/App/Kaufstatus geprüft. Apple StoreKit2 liefert nur nativ verifizierte Transaktionen; kein ungeprüfter JSONreceipt gilt als Erfolg. Fehlende/unverifizierbare Evidence bleibt Reconciliation/Failed und wird nicht finalisiert. Rohbelege verbleiben nur flüchtig im Adapter.
7. `FinalizeAsync` ist ein eigener Schritt nach dem atomaren dauerhaften Application-Grant. Es erhält eine Bestätigung mit genau demselben Transaktionsschlüssel und der erfolgreich committed Savegeneration; ohne diese Bindung kein `ConfirmPurchase`. Ein `FailedOrder` ist kein Finalisierungserfolg. Restore/Replays gehen durch dieselbe Verification-/Grantpipeline; bereits bestätigte Orders brauchen keinen erneuten Grant. Leere/fehlerhafte Abfragen widerrufen keinen lokalen Anspruch. Eindeutige verifizierte Revocation wird separat gemeldet; bloße Abwesenheit ist kein Widerruf. Persistierte Operation-/Entitlementzustände bleiben bei B; C implementiert Adapter und keine eigene Savedatei.
8. Profile/Providerkonfiguration werden explizit injiziert. Nichtproduction verwendet offizielle Google-Test-IDs und Sandboxstores; Production mit Test-IDs, fehlenden finalen IDs, fehlender Readiness oder ungeprüften SDK-/Legal-/Gerätebelegen ist nicht promotable. Installation eines Pakets ersetzt weder Rechtsprüfung noch Netz-/Sandbox-/Geräteprüfung.

## Begründung

Diese Konkretisierung macht die bisherigen Ports implementierbar und erhält die Zuständigkeit für Savedaten und Economy. Getrennte Recoveryevents lösen den unvermeidbaren Unterschied zwischen lokalem Timeout und tatsächlicher Anbietertransaktion ohne verlorene oder doppelte Vergabe. Storefinalisierung bleibt hinter dem dauerhaften Grant.

## Betrachtete Alternativen

- Callback direkt vergibt Punkte/Entitlement: verletzt Application-/Commitgrenze.
- Timeout verwirft jede spätere Transaktion: verliert bestätigte Käufe/verdiente Rewards.
- SDKinitialisierung beim Appstart: verletzt UMP-/IAPreadiness und Offlinefirst.
- Receipttext oder SDKobjekt im Applicationport: koppelt innere Module an Anbieter und erhöht Datenschutzrisiko.
- Backend/zweiter Provider: außerhalb P07.

## Konsequenzen

Die uninitialisierten alten Skelette bleiben für historische Compositiontests kompatibel und erhalten fail-closed Portmethoden. Neue echte Adapter werden von R explizit produktiv verdrahtet. GMA 11.5.0/UMP und IAP 5.4.3 werden exakt gelockt; native transitive Versionen, Lizenz und Privacydaten werden inventarisiert. Neue Contracttests prüfen Fakes und reale Adaptergrenzen; echte Sandbox-/physische Captures bleiben gesonderte M3gates. B/C stimmen persistente Callback-/Claimbindung ab; Saveänderungen werden nicht durch dieses ADR selbst umgesetzt.

## Betroffene Artefakte

WP-046; `Application/MobileServiceContracts.cs`, drei Applicationports; `MobileServices/Google`, `MobileServices/Store`; Mobiletests, Packages, konkrete Release-/SDK-/Legalvorlagen. Zentrale Bootstrap-/Build-/Workflowdateien bleiben R. Fachbeschreibung `ARCHITECTURE/MOBILE_SERVICES.md` wird nach Annahme fortgeschrieben.

## Validierung

Getrennte Review vor Implementierung. Tests: initial false; UMP stale callback/Fehler/Widerruf; kein SDKcall vor Freigabe; nicht personalisierte Requests; NoFill/Timeout/Cancel/Duplikate/Hauptthread; Reward spät/vor Close/nach Timeout; kein zweites Show bei unklarem Zustand; IAP ohne Readiness, ungültige Evidence, Pending/Replay/Restore, keine Finalisierung vor Grant oder bei anderer Transaktion, Bestätigungsfehler und leeres Restore ohne Revocation. Physische Geräte-/Netz-/Store-/Signinggates erst am wirklichen gebundenen Artefakt.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ergänzt ADR-020/024 sowie die bestehenden Grant-/Claim-/Modulgrenzen um typisierte Runtimeports. Kein Providerwechsel, keine Änderung öffentlicher Save-/Hashprojektionen. Wird derzeit durch keinen ADR ersetzt.

## Quellen

- [Google UMP Unity](https://developers.google.com/admob/unity/privacy), abgerufen 21.10.2026.
- [Google GMA 11.5.0](https://github.com/googleads/googleads-mobile-unity/releases/tag/v11.5.0), abgerufen 21.10.2026.
- [Unity IAP Verarbeitung](https://docs.unity.com/en-us/iap/purchases), [Verifikation](https://docs.unity.com/en-us/iap/receipt-validation), [Privacy](https://docs.unity.com/en-us/iap/privacy-and-consent/overview), abgerufen 21.10.2026.
