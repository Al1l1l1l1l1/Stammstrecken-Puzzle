# Hauptübergabe – Stammstrecken-Puzzle / WP-046

Stand: 21.10.2026. Releaselead R. Der vollständige Auftrag läuft; M1, M2 und M3 sind noch nicht angenommen.

## Auftrag und Quellen

Maßgeblich sind [WP-046](../../../WORK_PACKAGES/WP-046_Vollstaendiges-Spiel-und-Release-Candidate.md), [MANUS-2026-10-20-02 mit P01–P07](../../PRODUCTION_RECOVERY_AND_RELEASE_MANDATE_2026-10-20.md) und der direkte Gesamtauftrag des Owners. Development: `codex/wp-036-vollstaendige-textbibliothek`, [Draft-PR31](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/31). Gemeinsamer prospektiver Anker `39aaac5bdccfc612d71d012e556db80f96f2dcf9`, einziger Parent `bd05da191cd2bf8da89830f7db6da4941cc62343`. Der aktuelle Implementierungsstand wird im Git-Commit dieser Datei gesichert. Kein Mainmerge oder Storeakt. [Issue38](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/issues/38) bleibt offen; seine Texte wurden gelesen, dort fehlende Originalmedien nicht als betrachtet ausgegeben.

M1/M2 beenden den Auftrag nicht. R entscheidet technische Routinefragen, führt dieselbe Arbeit bis M3 fort und sammelt externe Voraussetzungen nur in der [kurzen Ownerliste](../../../Release/OWNER_INPUTS.md). Fehlende Menschen, Geräte oder Konten blockieren ausschließlich die davon tatsächlich abhängigen Schritte. Keine erfundenen Humanzeiten oder Owner-, Geräte-, Rechts- und Storeannahmen.

## Interne Teilbriefe und Dateieigentum

| Rolle | Ergebnis und Schreibbereich |
|---|---|
| A | Vollständige Spielpräsentation, Cartoon-Welten, Züge, Kosmetik, Audio und Accessibility; Presentation.UI/World, UI, Art, Audio und eigene Presentationtests. |
| B | 240 Rätsel, Proofs und Qualität; Application, Infrastructure.Content/Persistence, Editor.Content, Contentkataloge/Generator und eigene EditMode-Tests. |
| C | MobileServices.Google/Store, spezifische Platformadapter, Packages/native SDKs, Releaseunterlagen und Mobiletests; eigene Application-Ports und MobileServiceContracts. |
| R | Bootstrap/Composition, Editor.Build, Szenen, zentrale Projektdateien/Workflows/Steuerung/ADRregistry und Integration; eigene Kandidatensuche mit vorhandenem Solver. |

Drei ausdrücklich beauftragte Agenten im gemeinsamen Checkout. Keine Routine-WP-/Branchkaskade. Fremdes Dateieigentum vorher intern abstimmen. Dauerhafte Hash-/Save-/Providerverträge nach getrennter Fachreview: ADR037–041 angenommen. **R besitzt das einzige Unityfenster.** Keine zweite Editorinstanz und keine ungeplante Quellkopie während eines Laufs.

## Integrierter Spielumfang

Eigener `InternalPreview`-/`qa-mobile-candidate`-Verbraucher nach ADR039: Public240, lineare Kampagne mit fünf Welten und 20 Routen, 720 Playertexte, 240 Karten-/240 Zugreferenzen und 14 Cosmetics. Eigene Cartoonkompositionen, IBM-Plex/OFL-Schrift, 15 eigene PCM-Clips, echter Mixer mit fünf Gruppen/acht Stimmen und gespeicherte Audio-/Haptik-/Accessibilityoptionen.

Acht direkte Werkzeuge mit Setzen, Ersetzen, erneutem Tippen zum Leeren und Undo. Raster/Targets bleiben bei Eingabe und Autosave ortsfest. Getrennte feste Flächen für Randzahlen und Anschlüsse, auch zweistellig. Timer erst ab Mutation. Bestätigter Abschluss vor tatsächlicher Zugfahrt auf dem Spielerpfad, danach Ergebnis und Fortsetzung. Resultsheet passt sich dem Inhalt an; keine leere Videowerbungserklärung nach abgelehntem Bonus.

Gemeinsamer serialisierter Savev3-Weg für Fortschritt, Punkte, Bestwerte, Übung/Revision, Cosmeticskauf/Auswahl/Ablegen, Hint3+höchstens1Video je Identität und Tagesbonus. Hinweise werden erst nach gültig sichtbarem Displayreceipt abgebucht. Restart füllt sie nicht auf. Tagesbetrag vorab dauerhaft festgelegt, Claims und Grants dedupliziert. Echte UMP-/AdMob-/Unity-IAPadapter mit Consentfreigabe, Grant vor Nativefinalisierung, Restore, Widerruf und späten Callbacks. Keine erfundenen Storeangaben. Interstitial ausschließlich nach bestätigter Fahrt und ausdrücklicher Ergebnisfortsetzung.

**Vollständige Dauerbaustelle:** vier technische DRAFT-Profile; Reserve → begrenzte Worker-Generierung → erneute aktuelle Recent-Noveltyprüfung → bestätigtes ActiveDraft → Hint/Restart/Save/Resume → Abschluss15Punkte → optionales Video+10 oder Auslassen → nächste Reservierung. Separate ENDLESS_INSTANCE-Abschlussquelle und exakte Retainedbindung. Maximal20 offene Datensätze, monotone Ordinale, jüngste50 normalisierte öffentliche Signaturen und maximal576 Economyreceipts. Abschluss/Abbruch entfernt zugehörige Hintrecords. Vorhandene kanonische Solution-/Proofprojektionen, öffentliche UNIQUE-/Guess0-Prüfung jedes ausgegebenen Rätsels. Kein E1-Authoringreader-PASS behauptet.

Native Unity6000.3-Accessibilityhierarchie: sichtbare Buttons/Zellen/Zahlen/Ports, gleiche bewachte Aktionen, oberstes Ingame-Modal, Scrollgesten, sauberes Lösen eigener Hierarchien. OS-/Fokus- und tatsächliche native SDKfenster pausieren die Bedienung; Hintergrundabfragen tun das nicht. Exakte ephemere Handles verhindern, dass ein alter Schließen-Callback eine neue native Präsentation freigibt. Timeout oder unklarer Throw bestätigt keine Schließung. TalkBack-/VoiceOver-Gerätebelege fehlen.

**Kaufbedienung:** expliziter Preisabruf ohne Kauf, tatsächlicher lokalisierter Preis, danach eigener bewusster Kaufklick. Nur das adaptereigene, unverbrauchte aktuelle Storeangebot ist zulässig; kein versteckter Refetch. Alte/gefälschte/fremde Angebote und Duplikate werden abgewiesen. Optionaler typisierter Installer→Experience→Services-Eingang vor Awake; ohne tatsächliche Storekonfiguration und qualifizierten Review bleibt IAP gesperrt, das Offline-Spiel speichert weiter. Echte Profil-/Revieworiginale sind noch nicht geliefert; kein bool oder erfundener Hash erzeugt Readiness.

## Geschlossene Integrationsbefunde

Der tatsächliche Endlos-UIlauf fand nacheinander eine eigene Modalpause vor Spielfeldöffnung, eine zu frühe Testabfrage vor dem Hint-Displaycommit und einen echten Speicherlesefehler beim zweiten Rätsel. Letzterer entstand, weil Descriptor.recentPublicInputs sieben Felder einschließlich publicPuzzleHash schreibt, der Reader aber nur die allgemeine Sechsfeldform akzeptierte. Ein eigener strikter Reader prüft jetzt vollständiges Format, SemanticHashprofil und tatsächlichen SHA. Vorhandener Writer, Sechsfeldreader und Hash-/Saveformat bleiben unverändert.

Baseline `wp046-b-endless-codec/baseline.xml` reproduziert Reserve2→SAVE_FIELDS_INVALID. Fix: sieben Fälle/1801 Assertions einschließlich zweier Partien über tatsächliches AtomicSaveRepository, Reload und fünf neu gehashter Manipulationsfälle. C hat Source und Tests unabhängig angenommen. Play16 schließt den positiven gesamten UIweg bis zur zweiten gespeicherten Partie. Frühere fehlgeschlagene Logs/XML bleiben erhalten.

## Content und noch offene Kalibrierung

240 Originalquellen/Proofs aus `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d` importiert; ursprünglicher Source-Lock bleibt Herkunft. Sieben DRAFT-Geometrien verbessert; drei zunächst zu ähnliche Kandidaten erneut ersetzt. Finale neue Fälle S1-05-04-02:192Schritte/Tiefe16, -09:220/25, -12:240/42, jeweils Guess0. Gemeinsam gegen240 keine D4-Klone; minimale öffentliche Distanz6/5/5%, maximaler Pfad-Jaccard80,65/82,35/78,57%. Alle sieben10×10 erfüllen mindestens210Schritte/Tiefe23/Guess0. Vollständiger ursprünglicher C#-Parse-/Schema-/Hash-/Solver-/Proofpfad erneut **240/240 PASS**, Original `.utmp/wp046-b-content/full-proof-audit.log`.

56 gezielte späte Aufgaben, 54 initiale Humanfälle, zusätzlich zwölf Kandidaten pro Endlosprofil. Keine echten Erstkontaktzeiten, freigegebenen Sternschwellen oder menschlich angenommenen Profilklassen. Kampagne und Endlos liefern deshalb derzeit nur einen Stern: **kein vollständiger Launchzustand**. Proxies, synthetische Domainabschlüsse und Desktoptests sind keine menschlichen Spielbelege. Besonders -09 hat trotz höherer Proxies einen starken Doppelanker; menschlich prüfen. -12 bleibt benannter Ownerfall.

Aktuelle PublicHashbindung `886dab4515817e7f1bdf3142e4097c5bd87b4431e82262189d5fe4a5908c23f0`. Producer `tools/campaign/project_candidate.cjs` erzeugt nur DRAFT/InternalPreview; Rebinding nur ungemessener DRAFT-Profile. Qualitätsaudit verweigert Überschreiben echter Humanmessungen. Fünf neue INTERNAL-Einstiege separat in `Content/Localization/source/current-entry-deductions.json` an finale Levelbytes gebunden. Alle720 Playertexte und der historische Locale-Gitblob unverändert. Hashgebundene Textquellen erhalten explizite LF-Checkoutregeln; der Legacy-Workingtree ist dadurch jetzt identisch zum unveränderten Gitblob SHA256`4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c`. Frühere CRLF-Originalkopie lokal erhalten.

B bereitet den klar getrennten Zugang zu echten Humanstichproben vor, ohne240Pflichtspiele je Person und ohne Testfortschritt in Produktionsprofile einzuspeisen. Noch keine live Humanprüffassung oder erfundene Messung.

## Maßgebliche reale Prüfungen

Originale unter `.utmp/wp046-runtime/`, sofern anders angegeben. Desktop-/Fakeevidenz bleibt von Geräte-/Sandboxbelegen getrennt.

| Lauf | Ergebnis / Umfang |
|---|---|
| `playmode-attempt2.xml` |24/24: gespeicherte6×5-Lösung/Fahrt/Result/Nächste;10×10-Werkzeuge/Undo/Autosave/Positionsinvarianz. |
| `editmode-attempt5.xml` |121/121: Saved/Meta/Providerpersistenz, Kaufwiderruf/durableGrant/Nativeack, tatsächlicher IAP-Emitter/EDM-Defaultoff. |
| `playmode-candidate-attempt6.xml` |1/1 ganzer Kandidatenweg Hint3→2/Restart→Daily→Settings→Sammlung. |
| `editmode-attempt8.xml` |116/116: Endloskern, gemeinsame Saves/Hints/Restart/Receipts/Watermark/Hashes, vier tatsächliche Suchprofile/Novelty/Slowclock, Metaregression. |
| `playmode-union-attempt12.xml` |58/59; nachfolgend gezielt geschlossener Endlosbefund. Alle damaligen28 Mobiladapterfälle und UI-/Accessibility-/Resultlayoutfälle bestanden. |
| `playmode-union-attempt15.xml` |26/27; alle aktuellen Presentationfälle einschließlich fester Anschlussabstände bestanden, allein neuer Atomic-Rereadbefund oben. |
| `scene-endless-attempt11.log` |Exit0, aktuelle Szene gespeichert; drei tatsächliche Endlos-GUID/FileID-Referenzen feldgebunden geprüft, kein Preflight-Repair. |
| `editmode-attempt9.xml` |**70/70**: tatsächlicher Runtimecodec/Datei-Repository, Saved-/Endlosfortsetzungen, vier Profile und Szenen-Referenznegative. |
| `playmode-endless-attempt16.xml` |**1/1** vollständiger Endlos-UIweg: Hint3→2, Restart ohne Refund, Xsave/Reload/Resume, Lösung/Fahrt/+15, Skip, Nächste→neue zweite Partie. |
| `playmode-store-attempt17.xml` |**32/32**:30 Providercontracts, expliziter Preis-/Kauf-UIweg und tatsächliches weiter speicherndes Offline-Spiel bei fehlendem qualifiziertem Storereview. |

Relevante reale RenderTexturebilder durch R/A angesehen: Home/6×5/10×10/Hint/Daily/Settings/Sammlung/Result/Endlosmenü und neue `candidate-endless-puzzle.png`/`candidate-endless-result.png`. Neuer Hinweistext-/Anschlussabstand gezielt korrigiert. Keine physische Touch- oder Ownerannahme. Unveränderte WP036-Textquelle/zwölf Negativmutationen und kanonische READING_EDITION erneut PASS.

## Native Artefakte

**Androidbuild2 PASS**, stabile Kopie `.utmp/wp046-native/android-build2-verified/stp-mobile-preview.apk`,57.580.800Bytes, SHA256`c37a59d1c85c651e55874736d8d132faa9786ee0c60c938a7f5c9c99fb4204a9`. Debug/v2/CNAndroidDebug, QApackage, ARM64/siebenELF183, min26/target36. Tatsächliches Manifest Delay=true, klassischesAD_ID/Firebaseprovider fehlen. MobileAdsInitProvider/androidxstartup, dreiAD_SERVICES-Berechtigungen und native Dependencies transparent inventarisiert; [Inspektion](../../../Release/ANDROID_BUILD2_INSPECTION.json).

**iOS-Quellprojektexport1 PASS**, `.utmp/wp046-native/ios-export1-verified/`,3257Dateien/ZIP359.604.500Bytes, SHA256`e62d8fddba9b4f38db71c22fb3d0ef85b83b80bfb416b023956f477e76d26e41`. Plist/PBX/Swiftzuordnung unabhängig geprüft. Pods/Podfile.lock fehlen; kein Xcode-/Swiftcompile/IPA-/Signierungsbeleg. [Inspektion](../../../Release/IOS_EXPORT1_INSPECTION.json).

Beide stammen aus dem eingefrorenen **uncommitteten PrePostgame-Stand**, kein aktueller Vollspiel-/GitHEAD-/RC-/Netzruheclaim. Original-Buildreport, ManagedStripped/IL2CPPinputs, Gradle-/Mergerbelege und Hashes archiviert. Aktuelle vollständige Spielquelle ist jetzt `e636d9c7ff9e4e6d2ddc173f575cce3ae089bb0d`. Androidbuild3 wurde tatsächlich gebaut, sein unabhängiger tatsächlicher APKscan ist aber **FAIL: klassisches AD_ID vorhanden**. Archiv `.utmp/wp046-native/android-build3-verified/`,80.414.010Bytes,SHA256`3fb317fe031c5c27b0b4100c4aff0ce9dd97b32d7bef3a1445bb8dc38f233943`. Kein Privacy-PASS. C bestätigte: CLI ohne `-buildTarget Android`; tatsächliche Editor-RSP hatte Standalone-/keineAndroiddefines, Launcher enthielt weder priorisierten Removemarker noch Gate-Apply. Originale und RSP/DLL-Snapshot bleiben archiviert. Neuer universeller Preprocessguard sowie frühe Buildentryprüfung verlangen übereinstimmende angefragte/aktive/kompilierte mobile Plattform; EditMode10 **2/2 PASS** mit je drei negativen Mismatchfällen. Androidbuild4 wird explizit für Android gestartet, danach aktueller iOSexport2 explizit für iOS. Reales SDKinventar86nativeAAR/JAR/125originalePOM-/Noticequellen unterRelease; keine Rechtsfreigabe daraus.

## Konkrete Fortsetzung und Grenzen

1. Targetguardkorrektur nach geschlossener EditMode10 sichern, Scope-/Architectureprüfung abschließen; Androidbuild4 mit eigenem Log/Archiv/Hash und ehrlicher Commit-/Inputbindung erzeugen. Vorbestehende funktionale UserSettings sind getrennt zu binden, kein falscher Clean-HEAD-Claim.
2. Neue tatsächliche APK/ARM64/Signatur/Manifest prüfen, aktuellen iOS-Quellprojektexport binden. PR31 bleibt Draft, Main geschützt. Eine vollständige Hauptübergabe aktualisieren, keine Reihe von Schlussberichten.
3. ADR042 ist nach getrenntem C-Review und vor abhängiger Umsetzung von R angenommen. B baut beobachtenden Assignmentadapter/isolierten Store, A denselben exklusiven Produktbinder, C native Androidfences; vorerst spiegelnd unter `.utmp/`, während aktuelle Nativequellen eingefroren bleiben. R besitzt Composition/Builds/Offlineprojektion. Kein Produktfortschritt, keine automatisch behauptete Humanannahme; Human-iOS bleibt gesperrt. Getrennten Humanzugang/Stichproben fertigstellen. Tatsächliche unbekannte Erstkontaktzeiten, Profilannahme, physische Android-/iOS-/Sandbox-/Storekanal-/Privacy-/Accessibilitytests und macOS/Xcode/Signing brauchen reale Menschen/Geräte/Konten. Rechtliche Freigaben und konkrete öffentliche Einreichung bleiben zuständige Ownerhandlungen. Nur betroffene Schritte warten.

Unity6000.3.23f1: `C:/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Unity.exe`; Android/iOSmodule/API36 vorhanden. Kein angeschlossenesadb-Gerät, kein Emulator/AVD belegt. Unity nur zentral und versteckt mit nötiger Sandboxfreigabe. .NET10/ vorhandene Proofpipeline, Pythonvenv`C:/STP/.venv-stp-architecture/`.

Vorherige Nutzeränderungen **nicht stagen**: GraphicsSettings(lineareLichtoption), QualitySettings(AA), untrackedPackageManagerSettings/URPProjectSettings. ShaderGraph nur vorherigesWhitespace, Original `.utmp/wp046-runtime/preexisting-ShaderGraphSettings.asset`. RootProjectSettings-Portraitfunktion behalten, Unity-Schreibwhitespace nach Editorende normalisieren. RootResources und ResolverXML sind SDK-Arbeitsdaten, nur bei geschlossenem Editor archivieren; kanonischer IAPBillingMode bleibt GUIDgleich unterSettings/Resources. Historische Manifeste erhalten.

Arbeitsziel vier Wochen, bedingter Korridor vier–sechs ab21.10.; keine Garantie. Replanung spätestensTag3 nach tatsächlicher Qualität/Durchsatz/Zugangslage. Der vollständige WP046-Auftrag und das M3-Ziel bleiben aktiv.
