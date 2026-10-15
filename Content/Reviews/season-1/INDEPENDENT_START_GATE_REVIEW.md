# Unabhängige Startgate-Gegenprüfung

**Historische Prüfer-Evidenz, Schlussfolgerung überholt.** Dieser Bericht wurde
vor der Auftraggeberkorrektur erstellt. Seine Übernahme einer Koordinatorprämisse
belegt keine Manus-/Nutzervorgabe. Der Originalbericht bleibt unverändert;
die aktuelle Neubewertung und eigene widerlegende Probe stehen in
[ORDER_CORRECTION.md](ORDER_CORRECTION.md). Keine erneute unabhängige Abnahme.

Herkunft: tatsächlich getrennte, rein lesende Instanz `/root/content_startgate_review`, 13.10.2026; geprüfte Produktionsbasis `c9516bdbd5858fd9003d5207ec37d180ccc6ae23`. Kein Level-/Proof-/Freigabenachweis. Nachstehend vollständiger Ergebnistext des Prüfers; seine Quellenlektüre ist Prüfer-Evidenz, die Hauptinstanz hat die API-Gegenprobe separat ausgeführt.

Prüfergebnis: **Die fehlenden Referenzen blockieren den vorhandenen kanonischen Level-v2-/Produktionsproof-Weg. Fehlende spätere Freigaben allein blockieren dagegen nicht jedes Authoring.** Ein bereits dokumentierter Ersatzweg für 240 reine Logikquellen ohne diese Pflichtfelder wurde nicht gefunden.

Quellen und Code:

- `ARCHITECTURE/schemas/level-v2.schema.json:25–49`: `titleKey`, `statusKey`, `entryDeductionKey`, `mapSegmentId`, `trainMomentId`, `resultTextKey` sind zwingend; weder Auslassung noch `null` ist gültig.
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/LevelV2Reader.cs:92–107,153–156`: Der Produktionsreader erzwingt diese Felder tatsächlich.
- `Assets/StammstreckenPuzzle/Scripts/Editor/Content/ProofGenerator.cs:25–30`: Beide öffentlichen Generierungswege setzen `LevelV2Loader.Load` beziehungsweise einen gültigen `LevelV2LoadResult` voraus; bei ungültigem Level endet die Verarbeitung **vor** dem Solver. Ergebnis- und Dokumentkonstruktoren sind intern, kein regulärer öffentlicher Logik-only-Einstieg.
- `DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md:17,50`: kanonische JSON-Quelle und Level-v2 für neue Produktion verbindlich.
- `ARCHITECTURE/CONTENT_PIPELINE.md:42,75`: Authoring schreibt ausschließlich kanonisches JSON und verwendet dieselbe `LevelValidationPipeline`.
- `ARCHITECTURE/CONTENT_CATALOGS.md:7,25`: vorhandene Beispiele sind ausschließlich `FIXTURE_ONLY`, keine Season-1-Produktionsreferenzen. Im Arbeitsbaum fehlt das kanonische Verzeichnis `Content/` vollständig. Die gefundenen Schlüssel-/Completionzuordnungen liegen in Beispielen beziehungsweise Testdaten.
- `ARCHITECTURE/CONTENT_CATALOGS.md:61`: generische Production-Fallback-IDs dürfen fehlende Referenzen nicht verdecken.

Wichtige Abgrenzung:

- `ARCHITECTURE/CONTENT_CATALOGS.md:26` erlaubt ausdrücklich `DRAFT` für Authoring und Review.
- `ARCHITECTURE/CONTENT_PIPELINE.md:90` trennt erfolgreiches technisches Proofgate von späteren Cross-Reference-, Produkt- und Release-Lock-Gates. Deshalb **nicht** behaupten, fertige Assets, `PRODUCT_APPROVED` oder ein Release-Lock seien generell Voraussetzung für erste logische Arbeit.
- `starThresholdsSeconds: null` ist ausdrücklich zulässig; finale Zeitwerte sind kein Hindernis.
- `PuzzleDefinition.Create` und `PuzzleSolver.Solve` können reine Logik ohne Präsentationsreferenzen prüfen. Das kann sachliche Zwischenevidenz erzeugen, ist aber kein bestandener kanonischer Level-v2-/Produktionsproofpfad. `ProofV1Codec.Create` direkt mit übernommenen Metriken aufzurufen wäre ebenfalls kein Ersatz für die vorgeschriebene Regeneration durch `ProofGenerator`.

Zulässig bleiben quellengebundene Auftragsverankerung, Produktionsbrief-/Referenzlückeninventar und deren dokumentierte Übergabe. Ob zusätzlich konkrete, bewusst schemaungültige Rätselentwürfe persistiert werden dürfen, ist aus den vorhandenen Regeln **nicht eindeutig** belegt: Die Pipeline nennt zwar Entwürfe, verlangt jedoch kanonisches JSON und Prüfungen vor Commit; die Content-Bible §9 verlangt vor Produktion einen vollständigen Datensatz. Dies eigenmächtig als neues Logik-only-Format oder neue Ausnahme festzulegen wäre kein vorhandener regelkonformer Weg.

Für einen vollständigen geprüften Rätselbestand fehlt damit konkret entweder die belegte Zuordnung der Pflichtreferenzen oder eine separat bestätigte Lösung dieser Vertragslücke. Keine Schemaänderung, erfundenen Referenzen oder Abschwächung der Prüfungen ableiten.

Prüfung war rein lesend; keine Dateiänderungen, keine Wiederholung der WP-025-Abnahme und keine Freigabe erteilt.
