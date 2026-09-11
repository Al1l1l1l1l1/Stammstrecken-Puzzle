# ADR-023 – Releasekandidat-Identität und kosmetische Meilensteinclaims

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

Zwei verbliebene v0.2-Lücken betreffen releasekritische Atomizität: Staging war als releaseähnlich beschrieben, obwohl andere IDs und Providerwerte einen Neubau erzwingen; kosmetische Items kannten nur Kauf oder Default, obwohl die Produktquelle gezielte Freischaltung durch klare Fortschrittsmeilensteine bestätigt.

## Entscheidung

### Releasekandidat

`staging` ist eine nicht promotierbare Testdistribution mit `promotionAllowed=false`, Testidentitäten und Sandbox-/QA-Konfiguration. Ein Releasekandidat ist dagegen von Beginn an `buildProfile=production`: finale App-/Bundle-ID, Production-Signing, freigegebene Production-Providerkonfiguration, finaler Nutzer-Versionswert `X.Y.Z`, keine Development-/Profiler-/Testcrashwerte. `vX.Y.Z-rc.N` ist nur Kandidatenprovenienz.

Ein kanonisch gehashtes `release-manifest-v1` bindet Gitcommit, Workflow, Plattformbuildnummer, finale IDs, geheimfreie Signaturfingerprints, Toolchain-/Paket-/SDK-Lock, Content-/Release-Locks, Production-Config-Digests, geheimfreie Provideridentitäten, Artefakt-SHA-256, Storebuildreferenz, Gateberichte und offenen Blockerzustand. Nach Signierung ist das Manifest unveränderlich.

Promotion akzeptiert nur das archivierte Manifest und exakt dasselbe Binärartefakt beziehungsweise dieselbe immutable Storebuildreferenz. Promotion baut, signiert oder konfiguriert nicht neu. Der öffentliche Tag `vX.Y.Z` zeigt auf denselben Commit und dieselbe Artefakt-/Manifestidentität.

### Kosmetische Meilensteinclaims

`cosmetics-v2` gibt jedem Item genau einen Erwerbsmodus: `DEFAULT`, `PATIENCE_PURCHASE` oder `MILESTONE_GRANT`. Ein Milestone-Grant verwendet `milestoneEligibility.contractVersion = 1` und ausschließlich `ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS` mit einer nichtleeren eindeutigen Menge stabiler Campaign-Subject-IDs. Sterne, Zeiten, Hinweise, Währung, Analytics und Wanduhr sind keine Inputs. Konkrete Produktitems oder Meilensteinwerte werden nicht definiert.

`ClaimMilestoneCosmetic` prüft den gelockten Katalogsnapshot und alle First-Clear-Inputs. Die Claim-ID `cosmetic-milestone-claim:v1:<catalogId>:<itemId>` ist revisionsstabil. Inventory-Ownership ist der terminale Claimrecord und enthält Katalog- sowie Eligibility-Provenienz. Ein atomarer Copy-on-Write-Savecommit schreibt Ownership und Claim; es entsteht kein Ledgerdelta. Wiederholung ist `ALREADY_OWNED`, eine gleiche Claim-ID mit anderer Projektion ist `COS_MILESTONE_CLAIM_COLLISION`.

Eine veröffentlichte Item-ID darf Erwerbsmodus oder Eligibility nicht still ändern. V1→v2 erfindet aus `availableByDefault` keinen Besitz und erzeugt keine synthetischen Claims; bei nicht neutralem Mapping stoppt `CATALOG_MIGRATION_NEEDS_DECISION` beziehungsweise `SAVE_MIGRATION_NEEDS_DECISION`.

## Begründung

Beide Verträge verhindern einen scheinbaren Erfolg, der später neu gebaut oder doppelt gewährt werden müsste. Der Releasekandidat ist dasselbe Artefakt, das öffentlich wird; der Meilensteinclaim ist derselbe atomare Ownershiprecord, der Deduplikation nach Journalbereinigung trägt.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Staging-Build öffentlich promotieren | Andere IDs/Provider erfordern Neubau und brechen Artefaktidentität. |
| RC-Suffix als Storeversion | Kann für den finalen Versionswechsel Neubau erzwingen. |
| Separate unbegrenzte Milestone-Claimliste | Dupliziert terminale Inventory-Wahrheit. |
| Eligibility aus Sternen oder Meisterschaft ableiten | Nicht als konkrete Produktregel bestätigt. |

## Konsequenzen

Ein Architecture-v0.3-Fixture prüft Manifest und Promotionreceipt, nicht reale Storepromotion. Echte AAB-/IPA-, Signing-, Internal-/TestFlight- und Devicebelege bleiben spätere Produktionsgates.

`cosmetics-v1` bleibt Legacyvertrag; neue Produktion verwendet v2. Der bestehende Patience-Kauf bleibt unverändert atomar. BLOCKER-PROD-001 bis -003 bleiben offen; insbesondere wird der tägliche kosmetische Anspruch nicht aktiviert.

## Betroffene Artefakte

Release-/Builddokumentation, `release-manifest-v1`-Schema und Fixtures, `cosmetics-v2`-Schema und Fixtures, Contentkatalog/-pipeline, Persistenz, Zustandsmodell, Teststrategie und Architekturvalidator.

## Validierung

Strukturtests lehnen promotierbares Staging, Test-/Debugwerte im RC, Manifest-/Receipt-Differenzen, Secretfelder, unbekannte Erwerbsmodi, gemischte Kauf-/Milestonefelder, fehlende Campaignreferenzen, nichtatomare Claims, Eligibility-Kollisionen und synthetische Legacyclaims ab.

## Ersetzt / ersetzt durch

Ergänzt [ADR-010](./ADR-010-build-release-und-observability.md) und [ADR-016](./ADR-016-katalogvertraege-und-endless-identitaet.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/BUILD_AND_RELEASE.md "Build and Release v0.3"
[2]: ../ARCHITECTURE/CONTENT_CATALOGS.md "Content Catalogs v0.3"
[3]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.3"
[4]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
