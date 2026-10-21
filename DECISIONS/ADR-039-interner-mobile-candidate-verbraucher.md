# ADR-039 – Getrennter interner Mobile-Candidate-Verbraucher

## Status

**Angenommen** – 21.10.2026 nach getrennter fachlicher C-Review und prospektiver technischer R-Annahme im WP-046-Mandat. Keine blockierenden Vertragsbefunde. Umsetzung und negative Profil-/Provider-/Katalogtests bleiben gesondert erforderlich.

## Datum

2026-10-21

## Kontext

WP-046 muss die echten neuen Hint-/Daily-/Cosmetics-/Savefunktionen vor der abschließenden Human-/Produktfreigabe in einer installierbaren App zusammenführen. Die bestehende Public240-Prüffassung ist bewusst ein eigener enger IsolatedReview-/Save-v2-Vertrag. Die tatsächlichen Kampagnenquellen sind weiterhin DRAFT, menschliche Zeiten und Qualitätsannahmen fehlen. Weder Productionpromotion noch ContractTest als Laufzeitprofil oder Umdeutung des alten QA-Saves sind dafür zulässig.

## Entscheidung

Ein eigener `CatalogUse.InternalPreview` konsumiert ausschließlich DRAFT-Kataloge aus den tatsächlichen Produktquellen. Der exakt reservierte Profilnamespace ist `qa-mobile-candidate`; diese Namespace-/Use-Bindung ist bidirektional. Die Katalog-ID-Linien `qa-mobile-candidate`, `campaign-qa-mobile-candidate` und `completion-qa-mobile-candidate` (jeweils exakt oder mit weiterem Bindestrichsegment) sind ausschließlich diesem Use vorbehalten. Die bestehenden campaign-v2-/completion-v1-Schemata bleiben dadurch unverändert. Der alte `qa-public240-v1`-Namespace und seine IsolatedReview-Verträge bleiben getrennt. Es gibt keine automatische Profilübernahme, Fortschrittspromotion oder Identitätsmigration aus alten QAprofilen.

Separate Public-/Campaign-/Completion-/Cosmeticsprojektionen haben eigene interne Katalog-IDs und Inhaltslocks. Die vorhandenen öffentlichen Puzzle-IDs/-Hashprojektionen und Quell-DRAFT-Metadaten ändern sich dadurch nicht. Kein FIXTURE_ONLY-Input wird importiert. Die echte fachliche Hierarchie, Fortschritts-/Reward-/Hint-/Daily-/Ownershippolicy läuft im neuen internen Profil; fehlende echte Sternzeitklassen bleiben ausdrücklich unkalibriert. Eine Kampagne ohne die noch erforderliche Kalibrierung ist keine vollständige Launchfassung.

Der explizite Save-v3-Fabrikpfad darf zusätzlich InternalPreview konsumieren, ausschließlich mit use-/hash-/profilkonsistenten eigenen Katalogbindings. Generische passive Testreader bleiben aus der Runtime ausgeschlossen. Die neue Composition bindet diesen Namespace explizit an die QA-Appidentität und einen eigenen Speicherbereich. Vorhandene Production- und IsolatedReview-Gates werden nicht über ein pauschales `allowDraft` abgeschaltet.

Providerkonfiguration bleibt im neuen Verbraucher ausschließlich QA/Testanzeigen beziehungsweise tatsächliche Storesandbox. Ein Preview-Save kann weder Production-IAPreadiness noch Productionentitlements, Signing- oder Releasepromotion belegen. Reale Geld-/Consent-/Callback-/CASgrenzen gelten auch intern. Ohne passende Readiness/Capabilities starten Dienste nicht; das Spielen bleibt möglich.

## Begründung

So können die beschlossenen Funktionen wirklich zusammengebaut und geprüft werden, während Human-/Rechts-/Provider-/Storegates ehrlich offen bleiben. Die zusätzliche Verbraucheridentität trennt neue interne Vollfunktionsprüfung von historischer enger QA und späterer Produktion, anstatt deren Aussage zu verwischen.

## Betrachtete Alternativen

Production auf DRAFT freischalten: verwischt Produktgates. ContractTest als App verwenden: verwechselt Fixtures mit Laufzeitquellen. Alten Public240-Save still auf v3 erweitern: verändert seinen engen Vertrag und riskiert bestehende QAstände. Ein zweiter Metagame-Save: verletzt atomare Hint-/Daily-/Economytransaktionen.

## Konsequenzen

Neue fail-closed Consumer-/Namespaceprüfungen, eigene Projektionen und explizite neue Composition sind erforderlich. Historische QA bleibt unverändert ausführbar. Fehlende menschliche/physische/externe Nachweise bleiben M2-/M3-Gates; interne Funktionstests und Bilder ersetzen sie nicht. Die Produktentscheidung P01–P07 und der vollständige Launchumfang ändern sich nicht.

## Betroffene Artefakte

`Application/LocalGameContracts.cs`, gezielte Policy-Useprüfungen, `Infrastructure/Content` mit neuen internen Adapterprojektionen, `LocalSaveV3Codec.cs`/`SaveMigration.cs` mit exakt gebundenen Runtimefabriken, gezielte Tests, Root-Bootstrap-/Build-/Szenen-/Projektionsquellen, C-Providerprofil-/Bridgetests. Alles im bereits verankerten WP-046-Scope; kein weiteres Work Package.

## Validierung

Positive interne Vollfunktions-/Save-v3-Integration mit echten DRAFTquellen. Negative Fälle: falscher Namespace/Use, Public240-Übernahme, FIXTURE_ONLY-/PRODUCT_APPROVED-Verwechslung, falsche Inhaltslocks, generischer Testreader, Productionprovider/Entitlementpromotion, doppelte Vergabe und Save-/Catalogmismatch. Alte Public240-/Production-Guards regressieren. Getrennte Fachreview vor abhängiger Umsetzung und echte neue Unityintegration danach.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ergänzt ADR-034/035/036 um einen getrennten internen Verbraucher und ADR-037/038 um dessen explizite Profilbindung. Kein neuer Savehash, kein Providerwechsel, keine Productionfreigabe. Derzeit nicht ersetzt.
