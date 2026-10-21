# Stammstrecken-Puzzle – aktueller Projektstand

**Stand:** 21.10.2026 · **Aktiv:** [WP-046](../WORK_PACKAGES/WP-046_Vollstaendiges-Spiel-und-Release-Candidate.md) · Releaselead R.

**Architecture v1.0** bleibt Grundlage; `WP-001` bis `WP-007` sind abgeschlossen.

Direkter Ownergesamtauftrag führt [MANUS-2026-10-20-02](PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md) bis M3 aus. Gemeinsamer prospektiver Anker `39aaac5` mit einzigem Parent `bd05da191cd2bf8da89830f7db6da4941cc62343`; historisches Manifest unverändert. Development `codex/wp-036-vollstaendige-textbibliothek` / PR31, kein Mainmerge oder Storeakt.

A Spielerlebnis/UI/Art, B Metagame/Persistenz und C Mobilservices/Release arbeiten parallel. R besitzt Bootstrap, Build/Szenen/Projektsteuerung und neue Rätselkandidatensuche. [Eine Hauptübergabe](ProductionBlocks/ReleaseCandidate/HANDOVER.md) enthält Teilbriefe, Originalbelege und Fortsetzung.

**Tatsächlich geprüft:** Cartoonoberfläche/acht direkte Werkzeuge, gespeicherte6×5-Lösung → echte Pfadfahrt → Ergebnis/Nächste, ortsfeste10×10-Zellen. Frühere EditMode5 **121/121**, EditMode8 **116/116** bleiben an ihren Quellen gebunden. Aktuell EditMode9 **70/70**, Endlos-Play16 **1/1** vollständig bis Hint/Restart/Save/Resume/Fahrt/+15/Skip und neuer zweiter Partie, Store-Play17 **32/32**. Echter Zweitpartien-Codecfehler gezielt geschlossen, Format unverändert; 26 Presentationfälle einschließlich Anschlussabstand/Accessibility/Resultlayout bestanden. Reale Bilder R/A-geprüft, keine physische Geräte-/Ownerannahme.

**Content:** sieben DRAFT-Geometrien technisch verbessert; drei zu ähnliche Kandidaten erneut ersetzt, minimale D4-Distanz5–6%, maximaler Pfad-Jaccard79–82%. Alle sieben10×10 mindestens210 Schritte/Tiefe23/Guess0, neue Schlussaufgabe240/42/0. Tatsächliche C#-Parse-/Schema-/Hash-/Solver-/Proofpipeline erneut **240/240 PASS**. InternalPreview-Projektion neu gebunden (PublicHash886dab45…), 240 Map-/240 Trainmomente/14 Cosmetics/720 unveränderte Playertexte. 56er-Spätaudit und54 erforderliche initiale Humanfälle; Difficulty-/Sternzeiten bleiben unfreigegeben.

**Jetzt:** Vollständige Spielquelle `e636d9c` gesichert. Androidbuild3 tatsächlich gebaut, unabhängiger APKscan aber FAIL: AD_ID enthalten. Ursache: CLI ohne vorgewähltes Androidziel, daher Editorhooks als Standalone kompiliert. Universeller Active-/Compiled-Targetguard und früher Entrycheck korrigiert, gezielte EditMode10 2/2 PASS; nächster Build4 ausdrücklich mit Androidziel. APK3/Originale bleiben unverändert archiviert. Frühere APK2/iOSexport1 bleiben PrePostgame-Belege. ADR037–042 angenommen; getrennte Android-Humanprüffassung jetzt in Umsetzung, keine erfundenen Messwerte. M1/M2/M3 offen.

**Begleitende Ownermitwirkung:** [kurze Liste mit Bedarfszeitpunkten](../Release/OWNER_INPUTS.md). Konten, Signing, Geräte/Tester und Rechtsprüfung sind unbekannt; keine weitere allgemeine Abfrage und keine Blockade unabhängig möglicher Produktarbeit. Secrets nur über Rollen-/geschützte Übergabe.

**Lokales Umfeld:** Unity6000.3.23f1, Android/iOSmodule/API36 vorhanden. Vorherige Änderungen GraphicsSettings/QualitySettings sowie untracked PackageManagerSettings/URPProjectSettings getrennt erhalten. Unitygeneriertes Whitespace wird gezielt normalisiert. Arbeitsziel vier Wochen, bedingt vier–sechs; reale externe/Humangates bleiben offen.
