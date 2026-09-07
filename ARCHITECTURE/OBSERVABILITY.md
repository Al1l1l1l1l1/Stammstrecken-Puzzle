# Observability v0.1

## 1. Ziel

Observability macht Fehler und Produktflüsse nachvollziehbar, ohne Gameplay-Wahrheit oder unnötige Nutzerdaten in externe Systeme zu verschieben. Lokale strukturierte Logs funktionieren immer. Externe Analytics und Crashberichte werden ausschließlich nach freigegebener Capability aktiviert.

## 2. Ebenen

| Ebene | Zweck | Verfügbarkeit |
|---|---|---|
| Strukturierter lokaler Logger | Diagnosecodes, Zustandsübergänge und Fehlerursachen. | Immer; begrenzter Ringspeicher. |
| CI-/Buildlogs | Toolchain, Tests, Content, Build und Store-Preflight. | CI; secrets redigiert. |
| Firebase Analytics | aggregierte Produktmetriken aus Allowlist. | Nur freigegeben. |
| Firebase Crashlytics | Crash, ANR/non-fatal und Breadcrumbs. | Nur gemäß freigegebener Privacykonfiguration. |
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

## 5. Analytics-Ereignisschema

Alle Events tragen `eventSchemaVersion = 1`, App-/Buildversion, Plattform und eine lokal zufällige, nicht kontoübergreifende Installationsreferenz nur soweit zulässig. Namen und Parameter sind geschlossen versioniert.

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

## 7. Crashlytics

Crashreports erhalten ausschließlich:

- Build-ID und Commitabkürzung;
- App-/OS-/Geräteklasse;
- Save-Schema und letzte erfolgreiche Savegeneration;
- Contentkataloghash und Level-ID;
- Sessionphase und letzter Command-/Adaptercode;
- maximal 32 redigierte Breadcrumbs.

Android-IL2CPP-Symbole werden pro Release mit Firebase CLI hochgeladen; Apple-Symbole werden über den konfigurierten Buildschritt geprüft.[1] Symbolarchive werden nach Build-ID geschützt aufbewahrt. Ein Release ist blockiert, wenn ein interner Testcrash nicht symbolisiert erscheint.

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

Einstellungen bieten Privacy-Optionen, Analyticspräferenz soweit rechtlich/technisch vorgesehen und lokales Datenlöschen. Änderungen werden sofort an Adapter weitergegeben. Bereits gepufferte nicht erlaubte Events werden verworfen. SDK-eigene Reset-/Deletion-APIs werden genutzt, soweit vorhanden.

Da es kein Konto und keinen eigenen Backenddatensatz gibt, kann die App keinen geräteübergreifenden Profilabruf anbieten. Storetransaktionen unterliegen den Storeprozessen. Datenschutzerklärung, Anbieterauftragsverarbeitung, Retention und endgültige Consenttexte sind Release-Gates mit fachlicher/rechtlicher Freigabe.

## 10. Monitoring und Betrieb

Releaseverantwortliche prüfen nach interner, Staging- und öffentlicher Promotion mindestens:

| Signal | Reaktion |
|---|---|
| Crash-free sessions und neue Fatalcodes | Rollout stoppen, symbolisierten Stack prüfen. |
| Save-Recoveryrate | bei Anstieg Rollout stoppen; Datenmigration priorisieren. |
| Levelabschluss 1–2 | technische Regression von Contentschwierigkeit trennen. |
| Ads-Abbruch/Fehler/No Fill | Provider-/Consent-/Netzursache trennen; keine Frequenz erhöhen. |
| Reward-Commitdifferenz | Callback-/Idempotenzbug als Economyrisiko behandeln. |
| Kauf-/Restorerate und Pendingalter | Storeadapter und Produktkonfiguration prüfen. |
| D1/D7 und Impressionen pro aktivem Nutzer | nur aggregiert bewerten; Produktentscheidungen nicht automatisch ändern. |

Es gibt keine automatische Selbständerung der Monetarisierungsregeln. Dashboardbefunde führen zu einem neuen Work Package.

## 11. Tests

Schema- und Redactiontests versuchen verbotene Felder einzuschleusen. Consenttests verlangen null externe Events vor Freigabe. Crash-Smokes prüfen Symbolik und Keys. Secret-Scanning testet Logs und Artefakte. Offline-, Queuevoll-, Anbieterfehler- und Datenlöschtests sichern Fallbacks.

## Referenzen

[1]: https://firebase.google.com/docs/crashlytics/unity/get-started "Get started with Crashlytics for Unity"
[2]: ../DECISIONS/ADR-010-build-release-und-observability.md "ADR-010 – GitHub Actions, Store-Artefakte und Observability"
[3]: ./MOBILE_SERVICES.md "Mobile Services v0.1"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/13_Oekonomie_und_Monetarisierungs_Balancing.md "Stammstrecken-Puzzle – Ökonomie- und Monetarisierungs-Balancing"
