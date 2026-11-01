# Architekturentscheidungen – verbindlicher ADR-Index

## Aktuelle Ausführungs-/Entscheidungsführung – 20.10.2026

[MANUS-2026-10-20-02](../PROJECT_CONTROL/PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md) überträgt R normale technische Ausführungsentscheidungen im vollständigen Releaseauftrag. Echte neue dauerhafte Hash-/Save-/Security-/Privacy-/Provider-/Modul-/Buildgrenzen erhalten weiterhin einen kurzen vorab nummerierten ADR und gezielte getrennte Review; dessen technische Annahme/Integration darf R innerhalb des Mandats selbst abschließen. Kein ADR pro View/DTO/Routinefix und kein neuer Geschäftsführerprompt pro internem Fortschritt. Historische ADR-Fachkörper/Registry/Statusdaten bleiben erhalten; kein neuer fachlicher ADR wird allein für diese Arbeitsweisenfortschreibung registriert.

## Zuständige Statusnachführung – 19.10.2026

Der unveränderte enge Fachkörper von ADR-033 ist **heute tatsächlich durch Manus angenommen**, nicht rückwirkend am 18.10.2026. [Maßgebliche heutige Entscheidung MANUS-2026-10-19-01](../PROJECT_CONTROL/ProductionBlocks/WP-038/MANUS_DECISION_AND_EXECUTION_MANDATE_2026-10-19.md), §3. Seine frühere behauptete zuständige Vorcode-Annahme bleibt historisch unbelegt. ADR-Fachkörper, historische Daten, alle übrigen ADRs, Indexzeilen und Registrybestand bleiben unverändert; keine neue Architekturentscheidung oder vollständige historische AK-Erfüllung wird aus der Statusnachführung abgeleitet.

**Aktueller Architekturstand:** **Architecture v1.0**, freigegeben am 2026-09-13 im Rahmen von [`WP-006`](../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md) nach unabhängig bestandenem Abschlussreview von Architecture v0.5.
**Architektureinstieg:** [`ARCHITECTURE/ARCHITECTURE.md`](../ARCHITECTURE/ARCHITECTURE.md)

Der Ordner `DECISIONS/` enthält die dauerhaft nachvollziehbaren Architecture Decision Records (ADRs) des Projekts. Ein ADR dokumentiert Entscheidung, Begründung, Alternativen, Folgen und Ablösung. Produktentscheidungen bleiben den bestätigten Produktquellen vorbehalten.

## Aktueller Index

| ADR | Titel | Status | Geltung in Architecture v1.0 |
|---|---|---|---|
| [ADR-001](./ADR-001-unity-6-3-lts.md) | Unity 6.3 LTS als Game Engine | **Angenommen** | Gültig. |
| [ADR-002](./ADR-002-csharp-9-und-il2cpp.md) | C# 9 und IL2CPP | **Angenommen** | Gültig. |
| [ADR-003](./ADR-003-domain-trennung-und-modulgrenzen.md) | Reine Puzzle-Domain und gerichtete Modulgrenzen | **Ersetzt** | Historisch; durch ADR-013 ersetzt. |
| [ADR-004](./ADR-004-json-leveldaten-und-content-pipeline.md) | Versionierte JSON-Leveldaten | **Ersetzt** | Historisch; vollständig durch ADR-021 ersetzt, der alle fortgeltenden Grundsätze restatiert. |
| [ADR-005](./ADR-005-deterministisches-command-state-modell.md) | Deterministisches Command/State-Modell | **Angenommen** | Gültig. |
| [ADR-006](./ADR-006-lokale-persistenz-und-offline-first.md) | Lokaler atomarer Save als Offline-First-Wahrheit | **Ersetzt** | Historisch; durch ADR-014 und danach ADR-019 ersetzt. |
| [ADR-007](./ADR-007-solver-und-eindeutigkeitspruefung.md) | Deterministischer Constraint-Solver | **Angenommen** | Gültig; ADR-021 ergänzt das Proofartefakt. |
| [ADR-008](./ADR-008-mobile-service-ports-und-provider.md) | Mobile-Service-Ports mit Google- und Unity-Adaptern | **Ersetzt** | Historisch; durch ADR-015 und danach ADR-020 ersetzt. |
| [ADR-009](./ADR-009-automatisierte-teststrategie.md) | Automatisierte Tests als Architekturgrenze | **Ersetzt** | Historisch; durch ADR-017 und danach ADR-022 ersetzt. |
| [ADR-010](./ADR-010-build-release-und-observability.md) | Build, Release und Observability | **Angenommen** | Build-/Releasegrundsätze gültig; Production-Crashlytics durch ADR-020 ersetzt, ADR-023 ergänzt Kandidatenidentität und ADR-029 präzisiert den Rolloutreducer. |
| [ADR-011](./ADR-011-ui-assets-lokalisierung-und-audio.md) | UI, Assets, Lokalisierung und Audio | **Angenommen** | Produktiver Addressables-/Localizationvertrag gültig; ADR-033 ergänzt ausschließlich Inspector-/In-Memory-Locale für die benannte interne QA-Szene. |
| [ADR-012](./ADR-012-mobile-plattformbaselines.md) | Mobile Plattformbaselines | **Angenommen** | Gültig. |
| [ADR-013](./ADR-013-zyklusfreie-ports-und-modulgrenzen.md) | Zyklusfreie Ports und Modulgrenzen | **Angenommen** | Gültig; ADR-018 ergänzt die minimale Bootstrapkante. |
| [ADR-014](./ADR-014-save-kanonisierung-und-ledgerkompaktierung.md) | Save-Kanonisierung und Ledgerkompaktierung | **Ersetzt** | Historisch; durch ADR-019 ersetzt. |
| [ADR-015](./ADR-015-mobile-transaktionen-und-privacy-default-off.md) | Mobile Transaktionen und Privacy Default-Off | **Ersetzt** | Historisch; durch ADR-020 ersetzt. |
| [ADR-016](./ADR-016-katalogvertraege-und-endless-identitaet.md) | Katalogverträge und Endless-Identität | **Angenommen** | Historischer Entscheidungskörper unverändert; Katalog- und E1-ID-Grundvertrag gültig, Retention durch ADR-019 und Open-Lifecycle durch ADR-025 ersetzt, Cosmetics durch ADR-023 ergänzt. |
| [ADR-017](./ADR-017-versionierter-architekturvalidator.md) | Versionierter Architekturvalidator | **Ersetzt** | Historisch; durch ADR-022 ersetzt. |
| [ADR-018](./ADR-018-bootstrap-composition-root.md) | Kompilierbare Bootstrap-Composition-Root | **Angenommen** | Gültig; ergänzt ADR-013. |
| [ADR-019](./ADR-019-endless-watermark-und-save-v2.md) | Endless-Watermark und Save v2 | **Angenommen** | Watermark-/Save-v2-Grundsatz gültig; Open-Lifecycle und Claims durch ADR-025 präzisiert. |
| [ADR-020](./ADR-020-privacy-lifecycle-und-sdk-grenzen.md) | Privacy-Lifecycle und SDK-Grenzen | **Angenommen** | Provider-/Fail-closed-Grundsatz gültig; Bootstrapfence und Widerruf durch ADR-024 präzisiert. |
| [ADR-021](./ADR-021-puzzleidentitaet-und-proofartefakte.md) | Puzzleidentität und versionierte Proofartefakte | **Angenommen** | Gültig; ersetzt ADR-004 vollständig mit Restatement und ergänzt ADR-007. |
| [ADR-022](./ADR-022-validator-scope-und-belegkategorien.md) | Validator-Scope und Belegkategorien | **Ersetzt** | Historisch; vollständig durch ADR-026 ersetzt. |
| [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) | Releasekandidat-Identität und kosmetische Meilensteinclaims | **Angenommen** | Releaseidentität und Erwerbsgrundsätze gültig; Cosmetics-Reservation durch ADR-028 und Rollout-Evidenz durch ADR-029 präzisiert. |
| [ADR-024](./ADR-024-privacy-bootstrap-fence-und-widerruf.md) | Privacy-Bootstrap-Fence und crashsicherer Widerruf | **Angenommen** | Gültig; präzisiert ADR-020 für direkte Upgrades, Native Reconciliation und Widerruf. |
| [ADR-025](./ADR-025-endless-open-lifecycle-und-claims.md) | Endless-Open-Lifecycle und nachgelagerte Claims | **Angenommen** | Watermark, Rewardcommit und providerbestätigtes Close gültig; lokaler Skip und Providerstart durch ADR-027 präzisiert. |
| [ADR-026](./ADR-026-validator-evidenz-und-scope-vertrauensanker.md) | Validator-Evidenz und Scope-Vertrauensanker | **Angenommen** | Scope- und Evidenzgrundsätze gültig; gemeinsame historische WP-/Manifestbindung durch ADR-030 präzisiert. |
| [ADR-027](./ADR-027-endless-no-reward-terminalpfad.md) | Providerfreier Endless-No-Reward-Terminalpfad | **Angenommen** | Gültig; ersetzt den lokalen Nichtbeanspruchungs- und Providerstartanteil aus ADR-025. |
| [ADR-028](./ADR-028-cosmetics-reservation-binding.md) | Bindende Cosmetics-Claim-Reservation | **Angenommen** | Gültig; ersetzt die Meilensteinclaim-Reservation/-Commitbindung aus ADR-023. |
| [ADR-029](./ADR-029-rollout-reducer-semantik.md) | Vollständige Rollout-Reducer-Semantik | **Angenommen** | Gültig; präzisiert den operativen Rolloutvertrag aus ADR-010 und ADR-023. |
| [ADR-030](./ADR-030-wp-scope-trust-anchor.md) | Gemeinsamer historischer WP-/Scope-Trust-Anchor | **Angenommen** | Gültig; ersetzt die historische WP-/Manifestbindung aus ADR-026. |
| [ADR-031](./ADR-031-solver-v2-metriken-und-proof-regeneration.md) | Solver-v2-Metriken und Proofregeneration | **Angenommen** | Gültig; ergänzt ADR-007 und ADR-021 für Metriksemantik, solver-v2 und regenerierbare Proofcaches. |
| [ADR-032](./ADR-032-season1-ci-korpus-leistungsnachweis.md) | CI-Referenz-Leistungsnachweis für den Season-1-Korpus | **Angenommen** | Gültig; konkretisiert den bestehenden Budgetnachweis für reale Season-1-Inputs, ohne Solver- oder Contentsemantik zu ändern. |
| [ADR-033](./ADR-033-interne-transiente-spielschleife.md) | Interne transiente Spielschleife ohne Produktionscontentimport | **Angenommen** | Ergänzt ADR-011 ausschließlich um die benannte nichtproduktive QA-Szene; Productionimport-/Save-/Releaseverträge unverändert. |
| [ADR-034](./ADR-034-isolierter-public240-reviewpfad.md) | Isolierter Public240-Reviewpfad | **Angenommen** | Eigener nichtproduktiver QA-Entry mit geschlossener Projektion und isoliertem Profil; Produktionsgates unverändert. |
| [ADR-035](./ADR-035-lokaler-save-und-checkpoint-vertrag.md) | Lokaler Save-, Checkpoint- und Commitvertrag | **Angenommen** | Typisierte Ports, vorhandenes JCS per Injektion, vollständiger Checkpoint/CAS und Commit-vor-Fahrt; Fachverträge bleiben erhalten. |

| [ADR-036](./ADR-036-gespeicherter-public240-qa-entry.md) | Gespeicherter Public240-QA-Entry | **Angenommen** | Ergänzt ADR-034 ausschließlich um die benannte gespeicherte QA-Szene, deren eigene Inspector-/Localegrenze und normalen internen Buildentry; ADR-035 unverändert. |
| [ADR-037](./ADR-037-hint-daily-save-v3.md) | Identitätsgebundene Hinweise und lokale Tagesclaims in Save v3 | **Angenommen** | Ergänzt P03/P04 und ADR-035 um versionierte Hint-/Dailyrecords und konservative Migration; bestehende Hash-/CAS-/Profilgrenzen bleiben. |
| [ADR-038](./ADR-038-mobile-provider-runtime-contracts.md) | Mobile Provider Runtime Contracts | **Angenommen** | Ergänzt ADR-020/024 um typisierte Ads-/UMP-/IAPruntimeports, Recoveryevents und durable-grant-gebundene Finalisierung. |
| [ADR-039](./ADR-039-interner-mobile-candidate-verbraucher.md) | Getrennter interner Mobile-Candidate-Verbraucher | **Angenommen** | Eigener InternalPreview-/qa-mobile-candidate-Verbraucher für echte DRAFTquellen/v3-Funktionen; keine Fixture-/Production-/Legacyprofilpromotion. |
| [ADR-040](./ADR-040-interner-endless-generator-und-novelty-save.md) | Interner Endlosgenerator und begrenzte Noveltydaten | **Angenommen** | Neue explizite Suchparameter und begrenztes öffentliches50er-Fenster ausschließlich InternalPreview; Hash-/Watermark-/Terminal-/Humanvertragsgrenzen bleiben. |
| [ADR-041](./ADR-041-begrenzte-endless-ledgerbelege.md) | Begrenzte Endlos-Ledgerbelege | **Angenommen** | Höchstens576 tatsächlich referenzierte Economybelege im neuen Endlosrecord; kein Lifecycle-/Reward-/Terminalarchiv und keine neue Eligibilityautorität. |
| [ADR-042](./ADR-042-zugewiesene-humanpruefung.md) | Zugewiesene interne Humanprüfung | **Angenommen** | Getrennter Android-Prüfeinstieg mit aktueller Bedienung, quellgebundenen Assignments und lokalen Beobachtungen; keine Produktprofil-/Human-/Providerpromotion. |
| [ADR-043](./ADR-043-getrennter-endless-proofreader.md) | Getrennter strikter Endlos-Proofreader | **Angenommen** | Eigener geschlossener E1-Reader-/Schemaentry; gemeinsame unveränderte Proofprojektion und echte Descriptor-/Public-/Lösungsbindung, keine Ausgabe-/Humanpromotion. |
| [ADR-044](./ADR-044-kalibrierte-sternzeiten-und-endlosergebnisse.md) | Kalibrierte Sternzeiten und begrenzte Endlosergebnisse | **Angenommen** | Gebundene echte Kalibrier-/Runtimecompletionquellen, explizite v2-Parameter und atomare begrenzte Zeitsternergebnisse; keine DRAFT-/Humanpromotion. |
| [ADR-045](./ADR-045-operator-packauswahl.md) | Explizite Operatorpackwahl und globaler Human-Laufübergang | **Angenommen** | Geschlossene flüchtige Authority-Union/Global-CAS für17Packs, unveränderte Index-/Exportbytes und Originalerhalt; keine automatische Teilnehmer-/Humanpromotion. |
| [ADR-046](./ADR-046-bestaetigte-lokale-profilloeschung.md) | Bestätigte lokale Profillöschung und Writerstilllegung | **Angenommen** | Exakte Profil-/Ringlöschung nach Quieszenz, geschlossener Lifecycle-/Writer-Epoch und idempotente Löschintention; bestehende Save-/Human-/Providerwahrheit unverändert. |
| [ADR-047](./ADR-047-produktionspaketierung-und-release-manifest-v2.md) | Produktionspaketierung und nachweisgebundenes Release-Manifest v2 | **Angenommen** | Ergänzt den ausführbaren Releasepfad; ersetzt ADR-023 nur für neue V2-Manifestnutzung und eng begrenzte Evidenzfortschreibung. Alle übrigen Kandidaten-/Artefakt-/Kosmetikgrenzen bleiben. |

ADR-036 ist nach historischem nummeriertem Entwurf, getrennter technischer Review und dokumentierter mandatierter R-Annahme vor I-Fachcode wirksam; nur die geschlossene neue Registrierung wird ergänzt.

Damit existieren **47 ADRs**: 38 sind angenommen und aktuell wirksam; 9 bleiben als ersetzte historische Entscheidungen erhalten. ADR-037/038/039/040/041/042/043/044/045/046 wurden am 21.10.2026 vor abhängiger Implementierung getrennt fachlich geprüft und von R innerhalb WP-046 technisch angenommen; ihre konkrete Umsetzung bleibt separat nachzuweisen. ADR-047 wurde am22.10.2026 nach getrennter korrigierter Fachreview und vor abhängiger Implementierung von R innerhalb WP-046 technisch angenommen. Bei Widerspruch gilt das jüngere ausdrücklich ersetzende ADR. Teilersetzung oder Ergänzung ist nur zulässig, wenn Umfang und fortgeltender Teil im ADR und in diesem Index ausdrücklich benannt sind.

## Wann ein ADR erforderlich ist

Ein ADR ist erforderlich, bevor eine technische Entscheidung umgesetzt wird, wenn sie Architektur, Datenhaltung, Persistenz, Sicherheits- oder Datenschutzgrenzen, Plattformstrategie, externe Dienste, Integrationen, Build- oder Releasefähigkeit oder eine andere langfristige Systemgrenze beeinflusst.

Ein ADR ist kein Ersatz für eine Produktentscheidung. Fehlt eine notwendige Produktentscheidung, wird sie als Blocker dokumentiert statt erfunden.

## Ablage und Dateinamen

Jeder ADR liegt unmittelbar in diesem Ordner und folgt `ADR-###-kurzer-titel.md`. Die Nummer ist fortlaufend, eindeutig und wird nie wiederverwendet. Ein angenommener ADR wird nicht still inhaltlich umgeschrieben. Erlaubt sind nachträgliche Status-, Link- und Ersetzungsverweise, die seine historische Entscheidung nicht verändern.

## Verbindliche ADR-Struktur

| Abschnitt | Inhalt |
|---|---|
| Titel und ID | Eindeutige ADR-Nummer und kurze Bezeichnung. |
| Status | Entwurf, angenommen, ersetzt oder verworfen. |
| Datum | Datum der Dokumentation. |
| Kontext | Ausgangslage, Anforderungen, Einschränkungen und relevante Quellen. |
| Entscheidung | Präzise und überprüfbare technische Entscheidung. |
| Begründung | Warum die Entscheidung den Anforderungen entspricht. |
| Betrachtete Alternativen | Relevante Alternativen und Gründe gegen ihre Wahl. |
| Konsequenzen | Folgen, Risiken, Migrationen und Prüfpflichten. |
| Betroffene Artefakte | Work Packages, Dateien und Module. |
| Validierung | Nachweis der Entscheidung. |
| Ersetzt / ersetzt durch | Beidseitige Referenz auf frühere oder spätere ADRs. |

## Lebenszyklus und Superseding

Ein ADR beginnt als **Entwurf** und ist nicht verbindlich. Erst **Angenommen** macht die Entscheidung wirksam. Eine angenommene Entscheidung wird nur durch einen neuen, fortlaufend nummerierten ADR vollständig oder ausdrücklich teilweise abgelöst. Der neue ADR benennt den Vorgänger; der alte ADR erhält ausschließlich Status-/Link-/Ersetzungsverweise. Bei vollständiger Ablösung wird sein Status **Ersetzt**. Bei klar abgegrenzter Teilersetzung kann er **Angenommen** bleiben, wenn der fortgeltende Umfang in beiden ADRs und im Index eindeutig ist.

Ein ersetzter ADR bleibt als historische Begründung erhalten. Der Index wird im selben Commit aktualisiert. Ein verworfener ADR erzeugt keine Vorgabe. Work Package, Architekturnavigation und Validator müssen denselben Status ausweisen.

## Governance-Prüfung

Der eingecheckte Validator prüft fortlaufende eindeutige Nummern, zulässige Statuswerte, Pflichtabschnitte, bidirektionale Ersetzungs-/Nachfolgerverweise, Indexvollständigkeit und den in `PROJECT_CONTROL/CURRENT_STATE.md` benannten Architekturstand. Ein fehlender oder widersprüchlicher Indexeintrag blockiert den Abschluss.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture v1.0"
[3]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005 – Architecture-v0.5-letzte-High-Korrekturen"
[4]: ../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md "WP-006 – Architecture-v1.0-Promotion"
