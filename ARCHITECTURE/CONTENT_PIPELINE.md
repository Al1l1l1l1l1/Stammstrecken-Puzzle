# Content Pipeline v0.1

## 1. Ziel

Content ist versionierter Quellcode. Level, Lokalisierungsschlüssel, Assets und Abschlussreferenzen müssen in Pull Requests diffbar, maschinell prüfbar und deterministisch in Unity-Laufzeitdaten überführbar sein. Unity-Szenen oder generierte ScriptableObjects sind nie die einzige Wissensquelle.

Diese Spezifikation beschreibt den Produktionsweg, erzeugt aber keine der 240 konkreten Season-1-Rätsel und keine finalen UI-/Audio-Assets.

## 2. Quellen und abgeleitete Artefakte

| Inhalt | Kanonische Quelle | Abgeleitet |
|---|---|---|
| Level | `Content/Levels/<season>/<section>/<route>/<id>.json` | Runtimekatalog, Addressables, Preview. |
| Levelschema | `Content/Schemas/level-vN.schema.json` | Validatorbindings und Dokumentation. |
| Kampagnenhierarchie | `Content/Catalogs/campaign-vN.json` | Karten-Read-Model und Unlockindex. |
| Abschlussmomente | `Content/Catalogs/completion-vN.json` | typisierte Addressable-Referenzen. |
| Kosmetik | `Content/Catalogs/cosmetics-vN.json` | Betriebswerk-Katalog. |
| Lokalisierung | `Content/Localization/source/<locale>.json` plus Schema | Unity String-/Asset-Tables. |
| Grafiken/Modelle/Audio | `Assets/StammstreckenPuzzle/...` plus `.meta` und Importpreset | Spriteatlanten, komprimierte Texturen, Audio-/AssetBundles. |
| Generierter Unitycontent | keine manuelle Quelle | `Assets/StammstreckenPuzzle/Generated/`. |

`Generated` wird vor jedem deterministischen Reimport geleert und vollständig neu erzeugt. Handänderungen dort sind verboten und werden durch CI-Hashvergleich erkannt.

## 3. Levelauthoring-Workflow

```mermaid
flowchart LR
    Brief[Content-Bible / Levelbrief] --> Draft[Level JSON Entwurf]
    Draft --> Schema[JSON-Schema]
    Schema --> Semantic[Semantischer Validator]
    Semantic --> Solver[Solver: exakt eine Lösung]
    Solver --> Quality[Qualitätsmetriken und Deduction Proof]
    Quality --> Review[Leveldesign-Review und Spieltest]
    Review --> Import[Deterministischer Unity-Import]
    Import --> Catalog[Katalog-/Addressable-Build]
    Catalog --> CI[CI-Gates]
```

### 3.1 Erstellen

Ein Levelautor beginnt mit einer stabilen Kampagnen-ID und dem Produktionsbrief aus Content-Bible 14. Ein Editorfenster darf Raster, A/B und Lösung visuell bearbeiten, schreibt aber ausschließlich das kanonische JSON. Dieselben Operationen müssen headless über eine Batch-CLI verfügbar sein.

### 3.2 Lokale Prüfungen

Vor Commit laufen in dieser Reihenfolge:

1. JSON Schema;
2. semantische Levelregeln;
3. aus Lösung neu abgeleitete Randzahlen;
4. Puzzle-/Lösungs-/Proofhash;
5. Solverzählung bis zwei;
6. Deduction Trace und Metriken;
7. Kampagnenhierarchie und ID-Eindeutigkeit;
8. Lokalisierungs- und Abschlussreferenzen;
9. Ähnlichkeitsbericht gegen vorhandene Level;
10. deterministischer Import-Dry-Run.

Tools dürfen offensichtliche technische Felder wie Hashes aktualisieren. Sie dürfen keine Qualitätsnotiz, Einstiegserkenntnis, Zeitwerte oder Produkttexte erfinden.

### 3.3 Reviewbeleg

Jeder Level-Pull-Request zeigt pro Level ID, Raster, Randzahlen, A/B, Lösungshash, Lösungsklasse, Solverversion, Such-/Deduktionsmetriken, Einstiegsschluss, Qualitätsnotiz, Miniaturvorschau und Teststatus. Binäre Editoransicht allein genügt nicht.

## 4. Kampagnen- und Unlockkatalog

Der Kampagnenkatalog bildet Season → fünf Netzabschnitte → je vier Routen → je zwölf Meldungen ab. Er speichert Reihenfolge und Kartenreferenzen, aber keine duplizierten Levelregeln. CI verlangt für Season 1 exakt 240 eindeutige IDs und keine Lücke, sobald der betreffende Content-Work-Package-Abschluss dies fordert.

Unlockregeln werden als Application-Policy implementiert und aus stabilen IDs berechnet. Der Katalog darf keine Sternepflicht für Kampagnenfortschritt einführen. Betriebsrevision und Dauerbaustelle werden nach allen 240 korrekten Erstabschlüssen freigegeben.

## 5. Validatorarchitektur

Alle Authoringoberflächen, CI und Build verwenden dieselbe `LevelValidationPipeline`. Ein GUI-Tool darf kein eigenes Validierungsverhalten besitzen.

| Stufe | Eingang | Ausgang |
|---|---|---|
| Parse | Bytes | DTO oder Parsecodes. |
| Schema | DTO/JSON | Strukturdiagnosen. |
| Domain map | DTO | gültiges Domainobjekt oder Mappingcodes. |
| Semantic | Domain + Metadaten | sortierte Fehler/Warnungen. |
| Solver | öffentliches Puzzle | 0/1/2+, Lösung und Proof. |
| Cross-reference | Kataloge/Lokalisation/Assets | Referenzdiagnosen. |
| Quality | Proof + Katalog | Bericht, keine automatische Produktfreigabe. |
| Import | nur fehlerfreier Datensatz | deterministisches Runtimeartefakt. |

Ein `--strict`-Modus behandelt Warnungen als Fehler und ist in CI/Release verbindlich. Ausnahmen sind versionierte Allowlist-Einträge mit Diagnosecode, Level-ID, Begründung, Eigentümer und Ablaufdatum.

## 6. Deterministischer Import

Der Import hängt nur von kanonischen Quellen, Toolversion, Konfiguration und gepinnter Unityversion ab. Zeit, Dateisystemreihenfolge, Maschinenpfad und Locale dürfen Ausgaben nicht beeinflussen.

- Inputs werden nach normalisiertem Repositorypfad sortiert.
- GUIDs entstehen über Unity-`.meta`-Dateien und werden eingecheckt; generierte GUIDs werden aus stabilen IDs deterministisch verwaltet.
- Runtimekatalogeinträge sind nach stabiler ID sortiert.
- Zeitstempel und absolute Pfade werden nicht in generierte Daten geschrieben.
- Zwei saubere Imports desselben Commits müssen denselben Kataloghash liefern.
- Importeränderungen erhöhen `contentImporterVersion` und regenerieren alle Goldens bewusst.

## 7. Assetverwaltung

### 7.1 Ordner und Adressen

| Gruppe | Addressables-Label | Beispiele |
|---|---|---|
| Kern | `core` | Grid, Trackformen, Standard-UI, Startzug. |
| Season 1 | `season-s1` | Kartenabschnitte, lokale Levelkataloge, Abschlussmomente. |
| Kosmetik | `cosmetics` | Züge, Lackierungen, Betriebsobjekte. |
| Audio | `audio` plus Cue-/Bereichslabel | UI, Gleis, Zug, Ambience, Musik. |
| Lokalisierung | `localization` plus Locale | String-/Asset-Tables. |

Logische Adressen folgen `stp/<domain>/<stable-id>`, ausschließlich Kleinbuchstaben, Ziffern und Bindestriche. Code referenziert typisierte IDs, keine Assetpfade.

### 7.2 Importpresets

Textur-, Modell- und Audioimport verwendet versionierte Presets pro Zielklasse. Android-/iOS-Kompression, Maximalgröße, Mipmap, Read/Write, Meshimport und Audioqualität werden nicht pro Asset zufällig eingestellt. Abweichungen benötigen einen dokumentierten Label-/Preset-Override.

Binäre Quelldateien oberhalb einer festgelegten Schwelle werden mit Git LFS verwaltet. CI prüft fehlende LFS-Objekte, doppelte Inhalte, verwaiste Addressables, zyklische Bundles, Buildgrößen und Lizenzmetadaten.

### 7.3 Lokale Auslieferung

Alle Launchlevel und ihre notwendigen Kernassets liegen im installierten App-Bundle. Remote Catalogs, Pflichtdownloads und Online-Assetabhängigkeiten sind deaktiviert. Ein späterer Remote-Content-Kanal braucht ein neues ADR mit Cache-, Rollback-, Datenschutz- und Versionsstrategie.

## 8. Lokalisierungsvertrag

Deutsch (`de-DE`) ist die Startlocale. Jede sichtbare Zeichenfolge besitzt einen stabilen Schlüssel mit Namensraum, beispielsweise `level.s1.01.01.01.title` oder `ui.results.next`. Die Quellstruktur je Eintrag enthält Text, Beschreibung, Zeichenlimit, erlaubte Platzhalter und Status.

| Regel | Konsequenz |
|---|---|
| Kein sichtbarer Literaltext in C#/UXML | CI-Scan und Review. |
| Typisierte Platzhalter | Name und Typ müssen in jeder Locale identisch sein. |
| Keine Stringverkettung | Grammatik wird als vollständige lokalisierbare Nachricht modelliert. |
| Fallback | gewünschte Locale → `de-DE`; fehlender deutscher Key ist Buildfehler. |
| Pseudolocale | Expansion, Akzente und lange Wörter für Layouttests. |
| Satireton | Übersetzungen benötigen redaktionelle Freigabe; Maschinenübersetzung ist kein Production-Endstand. |
| Rechtliche Texte | eigener Status und Freigabeeigentümer. |

Leveldaten speichern keine fertigen deutschen Texte. Contentbrief und Qualitätsnotiz dürfen interne deutsche Freitexte sein; Nutzertexte sind Schlüssel.

## 9. Audio-Pipeline

Audioquellen erhalten Cue-ID, Lizenz/Urheber, Rohformat, Loop-Punkte, Normalisierungsziel, Mixergruppe und Importpreset. Semantische Cue-IDs entkoppeln Code von Clips. Fehlende optionale Cues sind sichere No-ops; fehlende Pflichtcues sind Buildfehler.

Klang bestätigt Eingabehandlung, nicht Lösungswahrheit. Zugfahrt-Audio startet aus dem Präsentationsereignis nach persistiertem Abschluss. Apppause, Audiointerruption und Nutzerlautstärke werden über den Audioadapter behandelt.

## 10. Katalog- und Assetversionierung

Jeder Release enthält:

- `contentCatalogVersion`;
- SHA-256 des Kampagnenkatalogs;
- Release-Lock aller veröffentlichten Level-IDs auf ihren unveränderlichen `puzzleHashSha256`;
- Hashliste aller Levelinputs und Runtimeartefakte;
- Addressables Content State/Buildlayout;
- Localization-Key-Inventar;
- Asset-/SDK-Lizenzinventar;
- Größenbericht je Gruppe und Plattform.

Ein Save referenziert stabile IDs und den zuletzt gesehenen Kataloghash. Contentrevisionen dürfen verdienten Fortschritt nicht löschen. Entfernte IDs bleiben über ein Tombstone-/Aliasmanifest auflösbar, bis eine bestätigte Migration existiert.

Eine bereits veröffentlichte Level-ID darf niemals auf einen anderen öffentlichen Puzzlehash zeigen. Logische Korrekturen verwenden eine neue ID und benötigen eine ausdrücklich bestätigte Progress-/Grandfathering-Migration. Eine Abweichung zum Release-Lock ist ein harter Buildfehler.

## 11. CI-Gates

Content-Pull-Requests müssen Schema, Semantik, Solver, Hashes, Cross-References, Pseudolocale, deterministischen Doppelimport, Addressables Build, Lizenzscan und Größenbudgets bestehen. Release führt den vollständigen Katalog erneut aus; kein gecachter Proof ersetzt die aktuelle Solverprüfung.

Generierte Dauerbaustellenkandidaten benötigen zusätzlich ein kanonisches `GeneratorQualityProfile` nach `SOLVER_ARCHITECTURE.md`. CI prüft Status `PRODUCT_APPROVED`, Profilhash, Solver-/Ruleset-Kompatibilität, Referenzkorpus, Metrikintervalle, Noveltyalgorithmus/-schwelle, Laufzeitbudget und Human-Review-Nachweis. Fehlt ein Wert oder stimmt ein Hash nicht, bleibt der Kandidat `DRAFT` und der Productionimport bricht ab. Wegen `BLOCKER-PROD-003` darf bis zur fachlichen Kalibrierung kein Generatoroutput veröffentlicht werden.

## 12. Rollen und Übergabe

Ein Levelautor verantwortet Logik und Qualitätsnotiz. Ein Solver-/Tooling-Reviewer verantwortet Nachweis und Validatoränderungen. Ein Content-Reviewer verantwortet Lernbogen/Nichtredundanz. Ein Localization-/Legal-Eigentümer verantwortet Nutzertexte und freigabepflichtige Inhalte. Eine Person beziehungsweise ein Agent darf technische Hashes erzeugen, aber nicht allein alle fachlichen Freigaberollen simulieren, wenn das spätere Work Package unabhängiges Review verlangt.

## Referenzen

[1]: ../DECISIONS/ADR-004-json-leveldaten-und-content-pipeline.md "ADR-004 – Versionierte JSON-Leveldaten"
[2]: ../DECISIONS/ADR-011-ui-assets-lokalisierung-und-audio.md "ADR-011 – UI Toolkit, lokale Addressables, Unity Localization und Unity Audio"
[3]: ../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md "Stammstrecken-Puzzle – Season-1-Content-Bible"
[4]: ./LEVEL_DATA_FORMAT.md "Level Data Format v1"
