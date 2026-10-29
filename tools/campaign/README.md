# Interne Rätsel- und Humanprüfquellen

Alle Befehle im Repositoryroot ausführen. Node.js und das .NET-10-SDK müssen verfügbar sein. Ausgabeverzeichnisse müssen neu sein; vorhandene Originale werden nicht überschrieben. Diese Tools starten keine Unityinstanz und installieren keine Playerassets.

## Endlosstichprobe

```sh
dotnet run --project tools/campaign/EndlessSample/EndlessSample.csproj -- .utmp/endless-sample-new
```

Der Sampler nutzt die tatsächlichen Generator-, Solver-, Descriptor-, JCS- und Proofimplementierungen. Die feste Seedfolge erzeugt 24 Kandidaten je Profil; zwölf Größen-/Solverquantile je Profil ergeben 48 technische Stichproben aus 96 Kandidaten. Jeder öffentliche Input wird frisch auf UNIQUE/NoGuess gelöst und mit `EndlessProofV1Codec.ValidateAgainst` gebunden. Buildartefakte liegen ausschließlich unter `.utmp/wp046-endless-sample/{obj,bin}`.

Je Kandidat entstehen sechs interne Originale: Publicinput, vollständige Parameter, Descriptor, Lösung, Proof und Quellenaudit. Das äußere `sample-audit.internal.json` bindet die tatsächlichen Quellbytes und Auswahl. Laufzeitbudgets können die Suche begrenzen; gemessene `observedWallMs` sind technische Laufwerte und keine reproduzierbaren Teilnehmerzeiten. Bei unvollständigem Pool endet das Tool mit Fehler. Größen-/Solverquantile sind keine menschlich bestätigten Schwierigkeitsklassen.

Der erhaltene P05-Eingang liegt unter `Content/Quality/endless/p05-sample-2026-10-21/`. Er ist DRAFT, unissued und kein gespeicherter Endlosauftrag. Seine Lösungs-, Proof-, Parameter- und Proxydateien gehören nicht in Teilnehmerassets. Einen neuen Samplerlauf als neue Quelle aufbewahren; bestehende Quellen niemals umschreiben.

## Teilnehmerpacks projizieren

```sh
node tools/campaign/project_human_assignments.cjs --repo-root . --out .utmp/human-assignments --build-id wp046-human-android-01 --endless-sample Content/Quality/endless/p05-sample-2026-10-21/sample-audit.internal.json
```

`--build-id` ausdrücklich an die tatsächlich zu prüfende Fassung binden. Der Befehl oben reproduziert die derzeit gebundene WP046-Fassung nur bei exakt passenden Quellen. Ohne `--endless-sample` entstehen nur die neun Kampagnenpacks. Optionale Parameter: `--study-id`, `--revision`, `--campaign-sample`, `--public-catalog`, `--public-bindings` und `--ui-overlay`. Ein Overlay ist eine ausdrücklich gewählte Quellenüberlagerung innerhalb des Repositorys; finale Buildbindungen verwenden die tatsächlichen Livebytes.

Die aktuellen 54 Kampagnenfälle und 48 ausgewählten E1-Inputs ergeben 17 gemischte Packs mit jeweils sechs Aufgaben. `participant/` enthält genau diese 17 Manifeste, `ui-source-lock.json` und `raw-sha256-lock.json`. Teilnehmer erhalten ausschließlich öffentliche Inputs und deren exakte Quellenidentitäten. Die Defaultzuweisung ist das Kampagnenpack mit `S1-05-04-12`; ein Packwechsel erzeugt keinen automatischen Teilnehmer- oder Tokenwechsel.

`internal/source-bindings.json` enthält ausschließlich authoringseitige Verweise: Repository-relative Pfade mit Forwardslash und tatsächliche Raw-/JCS-SHAs. Für die aktuelle Fassung ist die portable Kopie `Content/Quality/human/assignment-source-bindings.json` maßgeblich. Interne Bindungen werden nicht in den Player kopiert. Projektion und Assetinstallation sind getrennte Schritte; vor einer beauftragten Installation alle 19 Rawbytes vergleichen und vorhandene Unity-Metas erhalten.

Die fünf Nodeprüfgruppen einschließlich Mutationsnegativen nach der Projektion ausführen. Für abweichende Ausgabepfade `STP_HUMAN_PROJECTION_OUTPUT`, für einen anderen erhaltenen Samplereingang `STP_HUMAN_ENDLESS_SAMPLE` setzen; `STP_HUMAN_REPO_ROOT` und `STP_HUMAN_UI_OVERLAY` sind bei Bedarf ausdrücklich anzugeben.

```sh
node --test tools/campaign/tests/project_human_assignments.test.cjs
```

## Aussagegrenzen

Die Projektion erzeugt DRAFT-Prüfeingänge, keine Teilnehmerexposition, Messung, Freischaltung oder Produktpromotion. `AUTOMATED_TEST` bleibt als Run-Origin von tatsächlichen `PARTICIPANT_UI`-Läufen getrennt; ein technischer Test wird nicht zu Humandaten umetikettiert. Auch `PARTICIPANT_UI` allein bestätigt weder unabhängigen Erstkontakt noch fachliche Annahme. Voranzeige, Selbstauskunft, Hinweise einschließlich früherer Versuche und unklare Anzeigen müssen im echten Beobachtungspfad erhalten bleiben. Exportprüfung, Difficulty-/Sternzeitenfreigabe, Geräteprüfung und Storeveröffentlichung sind eigene, tatsächlich nachzuweisende Schritte gemäß ADR042/043.
