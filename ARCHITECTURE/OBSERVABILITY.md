# Observability v0.3

## 1. Ziel

Observability macht Fehler und Produktflüsse nachvollziehbar, ohne Gameplay-Wahrheit oder unnötige Nutzerdaten in externe Systeme zu verschieben. Lokale strukturierte Logs funktionieren immer. Externe Analytics ist buildseitig und nativ standardmäßig deaktiviert und wird nur nach gültigem Entscheid plus erfolgreichem NativeApply aktiviert. Firebase Crashlytics ist im Productionprofil ausgeschlossen, bis ein späteres ADR einen sofort wirksamen Widerruf und sichere Pufferlöschung für die dann gepinnte SDK-Version belegt.

## 2. Ebenen

| Ebene | Zweck | Verfügbarkeit |
|---|---|---|
| Strukturierter lokaler Logger | Diagnosecodes, Zustandsübergänge und Fehlerursachen. | Immer; begrenzter Ringspeicher. |
| CI-/Buildlogs | Toolchain, Tests, Content, Build und Store-Preflight. | CI; secrets redigiert. |
| Firebase Analytics | aggregierte Produktmetriken aus Allowlist. | Automatische Sammlung nativ aus; nur bei `canSendAnalytics == true`. |
| Lokale Crashdiagnose | redigierte Fehlercodes und begrenzte Breadcrumbs ohne externe Übertragung. | Immer lokal; Firebase Crashlytics im Productionprofil ausgeschlossen. |
| Store-/Ad-Dashboards | Transaktion und Anzeigenbetrieb. | Anbieterbedingt; in SDK-Inventar dokumentiert. |

## 3. Logschema

Jeder lokale Datensatz ist JSON Lines mit:

- `timestampUtc`;
- `monotonicSequence`;
- `severity` (`DEBUG`, `INFO`, `WARN`, `ERROR`, `FATAL`);
- `eventCode`;
- `correlationId` oder lokale Operation-ID;
- `buildId`, `appVersion`, `platform`;
- `module`;
- schema-validierten Feldern aus einer Allowlist.

Freitext ist nur eine lokale Entwicklerinformation und wird in Production nicht extern übertragen. Exceptions werden an Adaptergrenzen in stabile Codes plus redigierte technische Kategorie übersetzt.

Der lokale Ring umfasst höchstens 1.000 Einträge oder 1 MiB, je nachdem was zuerst erreicht ist. Rotation ist atomar. Debuglogs sind in Production standardmäßig aus; Warn/Error/Fatal bleiben lokal.

## 4. Datenschutz-Allowlist

| Zulässig | Nicht zulässig |
|---|---|
| Level-ID, Rasterbreite/-höhe | Zellinhalte, Lösungspfad, Undo-Inhalt |
| Sessionphase und Modus | Savegame oder Economyledger vollständig |
| Build-, Content-, Schema- und Solverversion | Receipt, Transaktionsbeleg oder Storecredential |
| Command-/Fehler-/Reason-Code | Werbe-ID, IDFA/GAID im eigenen Log |
| grobe Dauer-Buckets | präzise Interaktionszeitreihe einzelner Zellen |
| Consent-Capabilities als Booleans | Consentformularantworten im Detail |
| Geräteklasse, OS-/Appversion | Nutzername, E-Mail, Kontakte, Standort |
| SDK- und Adapterstatus | vollständige Dateipfade oder Secretwerte |

Vor Übergabe an Anbieter läuft jeder Eventdatensatz durch einen `TelemetrySanitizer`. Unbekannte Felder werden verworfen, nicht durchgereicht.

### 4.1 Aktivierungsvertrag

Die vollständige SDK-Matrix, native Manifest-/`Info.plist`-Schalter, Dashboardregeln und Initialisierungsreihenfolge stehen in [`MOBILE_SERVICES.md`](./MOBILE_SERVICES.md). Dieser Vertrag ist auch für Observability normativ:

1. `desired` und `effective` für Ads, Analytics und Crash werden im Applicationmodell getrennt; alle effektiven Werte starten `false`. Ein nativer Override ist niemals Entscheidwahrheit.
2. Firebase-Autocollection ist in Android-Manifest und iOS-`Info.plist` deaktiviert; ein fehlender oder widersprüchlicher Schalter ist Buildfehler.
3. Unity Analytics, Cloud Diagnostics und nicht benötigte Unity-Gaming-Services-Pakete sind nicht Bestandteil des Production-Manifests.
4. Application erzeugt oder puffert keine Analyticsereignisse aus der Zeit vor Freigabe.
5. Ein Analytics-Widerruf persistiert zuerst `REVOKED`, setzt den Port sofort No-op und den Runtime-Override `false`. Ein inkompatibles Upgrade verwendet den Reset-only-Build aus `MOBILE_SERVICES.md`.
6. Firebase Crashlytics ist in Production nicht importiert, nicht gelinkt und nicht initialisiert. `canSendCrashReports` bleibt `false`; Breadcrumb-Kopplung existiert nicht.
7. Ein SDK, dessen optionale Vorabübertragung oder sofortiger Widerruf nicht reproduzierbar ausgeschlossen werden kann, bleibt in Production ausgeschlossen.

## 5. Analytics-Ereignisschema

Alle Events tragen `eventSchemaVersion = 1`, App-/Buildversion, Plattform und eine lokal zufällige, nicht kontoübergreifende Installationsreferenz nur soweit zulässig. Namen und Parameter sind geschlossen versioniert. Ein Eventobjekt wird erst nach bestätigter Capability konstruiert; vor Freigabe existiert keine persistente oder SDK-interne Warteschlange.

| Event | Erlaubte Kernparameter | Zweck |
|---|---|---|
| `app_session_started` | returning bucket, locale, consent capabilities | technische Reichweite. |
| `level_started` | level ID, mode, grid size, prior stars | Funnelstart. |
| `level_completed` | level ID, mode, duration bucket, stars, hint bucket, marker booleans | Abschlussrate und Schwierigkeit. |
| `results_acknowledged` | level ID, mode | natürliche Abschlussstelle. |
| `route_completed` | season/section/route IDs | Fortschrittsfunnel. |
| `season_completed` | season ID | Endgamefreigabe. |
| `hint_requested` | level ID, source type | Hilfebedarf. |
| `ad_opportunity` | placement, eligible bool, deny reason | Policyverständnis ohne SDKdetail. |
| `ad_result` | placement, format, status, provider code category | technische Adsqualität. |
| `reward_committed` | placement, reward type | idempotente Gegenwertbestätigung. |
| `purchase_flow` | product logical key, stage, result category | Kauf-/Restorequalität; kein Preis aus Client vertrauen. |
| `save_recovery` | selected source, error category, schema version | Datenintegrität. |
| `content_validation_failed` | level ID, code | QA; Production sollte dies nie senden. |

D1-/D7-Rückkehr wird im Analyticsbackend aus zulässigen Sessionevents aggregiert. Der Client schreibt keine „Retention“-Wahrheit. Exakte Zellkoordinaten, Lösungssequenzen und Freitext werden nie gemessen.

## 6. Eventversionierung

Eine semantische Änderung eines Events erhöht entweder dessen Namen (`*_v2`) oder die globale `eventSchemaVersion`; bestehende Dashboards werden nicht still umgedeutet. Ein maschinenprüfbares Telemetrieschema wird mit der Implementierung eingecheckt. CI testet Namen, Typen, erlaubte Werte, Feldanzahl und Größenlimit.

Remote Config darf weder Analytics-Consent umgehen noch Produktregeln, Adsfrequenz, Belohnungen oder Puzzlelogik verändern.

## 7. Crashdiagnose

Der lokale Diagnose-Ring erhält ausschließlich:

- Build-ID und Commitabkürzung;
- App-/OS-/Geräteklasse;
- Save-Schema und letzte erfolgreiche Savegeneration;
- Contentkataloghash und Level-ID;
- Sessionphase und letzter Command-/Adaptercode;
- maximal 32 redigierte Breadcrumbs.

Android-IL2CPP- und Apple-Symbole werden als geschützte Releaseartefakte nach Build-ID aufbewahrt. Ein interner Testcrash muss lokal und über Plattform-Crashlogs symbolisierbar sein. Ein später zugelassener externer Crashprovider benötigt einen neuen ADR, SDK-/Privacy-Diff und physische Capturebelege.

Erwartbare Anbieterfehler wie No Fill sind keine Non-Fatal-Crashes. Sie werden gezählt, aber nicht als Exceptionrauschen gesendet. Domaininvarianten, Savekorruption und unhandled exceptions sind Fehler hoher Priorität.

## 8. Diagnosecode-Namensraum

| Präfix | Modul |
|---|---|
| `DOM-*` | Puzzle-Domain und Commands. |
| `SOL-*` | Solver, Proof und Hint. |
| `LVL-*` | Levelschema, Semantik und Content. |
| `SAV-*` | Persistenz, Migration und Recovery. |
| `ECO-*` | Ledger, Reward und Inventar. |
| `ADS-*` | Werbeadapter und Adpolicy. |
| `IAP-*` | Kauf, Pending und Restore. |
| `CON-*` | Consent und Privacycapabilities. |
| `ANA-*` | Analyticsvalidierung und Versand. |
| `AUD-*` | Audio/Haptik. |
| `BLD-*` | Build, Signing und Store-Preflight. |

Codes werden nie für eine andere Bedeutung wiederverwendet. Ein neuer Code erhält Dokumentation, Schweregrad, erlaubte Felder, Nutzerwirkung und Test.

## 9. Nutzerkontrolle und Datenlebenszyklus

Einstellungen bieten Privacy-Optionen, Analyticspräferenz soweit rechtlich/technisch vorgesehen und lokales Datenlöschen. Änderungen werden zuerst atomar im `PrivacyDecisionRecord` festgehalten und anschließend an Adapter weitergegeben. Nicht erlaubte Analyticsereignisse werden nicht erzeugt. Da Crashlytics in Production fehlt, existiert dort keine externe Crashqueue; lokales Datenlöschen entfernt den redigierten Ring.

Da es kein Konto und keinen eigenen Backenddatensatz gibt, kann die App keinen geräteübergreifenden Profilabruf anbieten. Storetransaktionen unterliegen den Storeprozessen. Datenschutzerklärung, Anbieterauftragsverarbeitung, Retention und endgültige Consenttexte sind Release-Gates mit fachlicher/rechtlicher Freigabe.

## 10. Monitoring und Betrieb

Releaseverantwortliche prüfen nach interner, Staging- und öffentlicher Promotion mindestens:

| Signal | Reaktion |
|---|---|
| Plattform-Crashberichte und neue lokale Fatalcodes | Rollout stoppen, symbolisierten Stack prüfen. |
| Save-Recoveryrate | bei Anstieg Rollout stoppen; Datenmigration priorisieren. |
| Levelabschluss 1–2 | technische Regression von Contentschwierigkeit trennen. |
| Ads-Abbruch/Fehler/No Fill | Provider-/Consent-/Netzursache trennen; keine Frequenz erhöhen. |
| Reward-Commitdifferenz | Callback-/Idempotenzbug als Economyrisiko behandeln. |
| Kauf-/Restorerate und Pendingalter | Storeadapter und Produktkonfiguration prüfen. |
| D1/D7 und Impressionen pro aktivem Nutzer | nur aggregiert bewerten; Produktentscheidungen nicht automatisch ändern. |

Es gibt keine automatische Selbständerung der Monetarisierungsregeln. Dashboardbefunde führen zu einem neuen Work Package.

## 11. Tests

Schema- und Redactiontests versuchen verbotene Felder einzuschleusen. Consenttests verlangen null externe Events vor Freigabe. Die vier reproduzierbaren Szenarien aus `MOBILE_SERVICES.md` prüfen auf je einem physischen Android- und iOS-Gerät Fresh Install, Upgrade mit früher aktivem Analytics-Override, Widerruf und Re-enable. Der Paket-/Linkerscan belegt Crashlytics-Abwesenheit. Emulator oder Simulator allein ist kein bestandener Gerätesmoke. Secret-Scanning testet Logs und Artefakte. Offline-, Queuevoll-, Anbieterfehler- und Datenlöschtests sichern Fallbacks.

## Referenzen

[1]: https://firebase.google.com/docs/crashlytics/unity/customize-crash-reports "Firebase Crashlytics Unity opt-in reporting"
[2]: ../DECISIONS/ADR-020-privacy-lifecycle-und-sdk-grenzen.md "ADR-020 – Privacy-Lifecycle und SDK-Grenzen"
[3]: ./MOBILE_SERVICES.md "Mobile Services v0.3"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/13_Oekonomie_und_Monetarisierungs_Balancing.md "Stammstrecken-Puzzle – Ökonomie- und Monetarisierungs-Balancing"
