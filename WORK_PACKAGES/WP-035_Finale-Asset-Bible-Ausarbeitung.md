# WP-035 – Finale Asset-Bible: vollständige Ausarbeitung

## ID

`WP-035`

## Ziel

Den bereits vorgesehenen Produktionsblock „Finale Asset-Bible“ vollständig als konsistente, konkrete und prüffähige Gestaltungs- und Produktionsvorlage ausarbeiten. Geliefert werden sieben Fachkapitel, echte Gestaltungsreferenzen, reproduzierbare Konstruktionsquellen, Schriftlizenznachweise, Produktionsinventar und Übergabe. Neue Details bleiben Vorschläge zur Produktfreigabe; kein Produktionsimport und keine automatische Freigabe.

## Voraussetzungen

Der ausdrücklich bestätigte Owner-Kontext dieses Chats steht über nachgeordneten Repo-Vorgaben. Insbesondere gilt die ursprüngliche, am 17.10.2026 erneut bekräftigte Absicht: sehr nahe deutsche Bahn-App-Anmutung, unmittelbare Pendler-Wiedererkennung, aber keine Kopie; nicht zu generischer oder vorsichtig entfernter Bahnsatire abschwächen. Die aktuellen Grenzen des Ausarbeitungsauftrags (Entwurfsstatus, kein Merge/Produktimport) bleiben bestehen. AGENTS-Lesereihenfolge abgeschlossen; Konzept 15, 00, 09, CURRENT_STATE, WORK_QUEUE, WORK_PACKAGE_RULES, AI_HANDOVER_RULES und DEFINITION_OF_DONE gelesen. Fachbasis: Konzept 03, 05, 07, 10, 11, 12, 13, 14 sowie TECH_STACK, MODULE_BOUNDARIES, CONTENT_PIPELINE, CONTENT_CATALOGS, TEST_STRATEGY und ADR-011/ADR-030. Direkter Ownerauftrag vom 17.10.2026 konkretisiert den bestehenden Planblock ohne neue Produktplanung. Frisch verifizierte Mainbasis: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Gesamtinventur einschließlich unintegrierter WPs 028–034 ergab keine vorhandene Asset-Bible-Bearbeitung und WP-035 als nächste freie ID. PR #21 wird ausschließlich lesend verwendet; keine Freigabe durch Beispielnutzung.

## Scope

A Marke/Zeichen/Icon; B Schriftlizenz, Typohierarchie und Farbwerte; C grafische Bausteine der bestehenden Oberfläche; D fiktive Zugfamilien/Lackierungen/Objektgruppen; E Material und Kartenbausteine aller fünf vorhandenen Netzabschnitte einschließlich ihrer 20 Routen; F Eingabe-, Abschluss-, Fahr-, Ergebnis- und Kosmetik-Motion/Sound; G nachvollziehbares Produktionsinventar und Übergabe. Konkrete Vorschläge werden zusammenhängend formuliert und strikt von bindenden Vorgaben sowie späteren Produktionsassets getrennt. Tatsächliche Referenzdateien bleiben dokumentarisch außerhalb von Assets und Content.

Verankerter Scope: [WP-035.documentation.scope.json](../tools/architecture-validation/scopes/WP-035.documentation.scope.json). Work Package und Manifest werden gemeinsam vor Fachänderungen im selben Add-Commit eingeführt; Manifest bleibt byteunveränderlich.

## Betroffene Dateien/Module

Neu: `PROJECT_CONTROL/AssetBible/WP-035/README.md`, `01_Marke_und_App-Icon.md`, `02_Typografie_und_Farben.md`, `03_UI_und_Puzzlebausteine.md`, `04_Zuege_Lackierungen_und_Betriebsobjekte.md`, `05_Welt_und_Kartenmaterial.md`, `06_Motion_und_Sound.md`, `07_Produktionsinventar_und_Uebergabe.md`.

Neu im selben dokumentarischen Verzeichnis: `references/` für SVG-/HTML-Konstruktionsquellen und PNG-Ansichten, `evidence/` für Originalquellenauszüge/Schriftlizenzen/Prüfprotokolle/Dateimanifest, `build_references.py` für reine statische Referenzwiedergabe (keine App), `validate_delivery.py` für die konkrete Lieferprüfung. Zusätzlich genau dieses Work Package, sein neues Manifest, `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` für additive branchlokale Statusnachführung. Keine Produktionsassembly ist betroffen.

## Ausdrücklich nicht erlaubte Änderungen

Keine Spiel-/Domain-/Solverimplementierung, Level-/Proofänderung, Integration fremder Branches, Änderung bestehender Konzeptquellen/ADRs/Projektregeln/historischer WPs oder Manifeste. Keine Paket-, Prüfanforderungs-, Release- oder Freigabeänderung. Keine neue Pipeline, Rendererwahl, frei erfundene Performancegrenze, Sonderfassung, Demo-App oder QA-Architektur. Keine Festlegung von Gesten, Orientierung oder offenen Accessibility-Produktentscheidungen. Kein endgültiger Verkaufskatalog/Preis/Freischaltung, keine finale Textbibliothek/Zeitkalibrierung/menschlichen Spieltests. Keine Käufe, Veröffentlichung, Produktfreigabe oder Merge. `.github/workflows/` bleibt unverändert: der bestehende CI-Scope-Selektor verweist auf WP-027; eine daraus entstehende Scopeprüfung wird ehrlich als nicht bestandener CI-Nachweis dokumentiert, nicht umgangen oder als Produktblocker umgedeutet.

## Akzeptanzkriterien

1. A–G vollständig konkretisiert und untereinander konsistent; referenzierbare Konstruktionen, Werte, Material-/Varianten-/Zustandsvorgaben statt bloßer Stilwörter.
2. Bahnfahrer-Wiedererkennung, eigenständiges Zeichen/Fahrzeuge, dienstlicher Duktus und hochwertige Miniaturwelt erhalten; Schienenersatzverkehr ist sichtbarer Lore-Kern.
3. Sechs konkrete Formen, optionale X/Grau-Notizen und unabhängiges Auswahlgelb unverändert; 4×4 und 10×10 lesbar referenziert, keine versteckte Lösungsbewertung.
4. Bestehende Screens, fünf Abschnitte/20 Routen, Kartenfolge und Abschlussreihenfolge erhalten; Kosmetik bleibt ohne Spielvorteil.
5. Vorgeschlagene Schriften an Originalquellen lizenzgeprüft; Quellen/Versionen, Rechte/Einschränkungen und tatsächliche gelieferte Schrift-/Lizenzdateien eindeutig inventarisiert.
6. Tatsächliche Referenzansichten existieren, öffnen und passen zur Spezifikation; keine Behauptung eines implementierten Spiels. Beschriebene Motion-/Audiodateien zählen nicht als geliefert.
7. Inventar trennt gelieferte Referenzen, bindende Regeln, Vorschläge, künftige Produktionsassets und externe Freigaben. Format-/Adress-/Import-/Lizenzvorgaben entsprechen vorhandener Architektur.
8. Links, Git-Diff und unveränderte fremde Quellen nachgewiesen; vorhandener Validator mit eigenem verankerten Scope ausgeführt. Reguläre CI-Ergebnisse am finalen PR-Head ehrlich dokumentiert.
9. Status, Grenzen, Ergebnis, letzter und nächster Schritt persistent; eigener Draft-PR, kein Merge. Eigene Prüfung wird nicht als unabhängige Abschlussfreigabe bezeichnet.

## Tests

- Vollständigkeits-/Quellenmatrix A–G und Konsistenzprüfung gegen genannte Fachquellen; kein automatischer Test ersetzt die manuelle Gestaltungskontrolle.
- Eigene Lieferprüfung: Dateiexistenz/Öffnbarkeit von PNG/SVG/HTML, lokale Markdownlinks, Tabellen-/Inventarverweise, SHA-256-Manifest und Font-Lizenzprovenienz; finale PNG-Ansichten tatsächlich sichten.
- Konstruktionen reproduzieren; sechs Formen/Notiz-/Auswahl-/Countzustände visuell kontrollieren; Karten-/Motionreihenfolge abgleichen.
- `git diff --check`, dokumentierte unveränderte Fremdquellen und `python tools/architecture-validation/validate.py --scope documentation --scope-manifest tools/architecture-validation/scopes/WP-035.documentation.scope.json --self-test` in gepinnter Validatorumgebung.
- Draft-PR veröffentlichen und tatsächliche bestehende CI am eingefrorenen Head dokumentieren. Keine CI-Ausnahme, keine neue Prüfserie oder zusätzliche Abnahmerolle.
- Unity-/IL2CPP-/Geräte-/Audio-Hör-/menschliche Spieltests sind kein Nachweis dieses reinen Gestaltungspakets; vorhandene reguläre Unity-CI wird als Regressionsergebnis eingeordnet, nicht als Nachweis implementierter Gestaltung. Produktionsimport, Laufzeitkontrast und finale externe Rechtsprüfung bleiben später und werden nicht vorgetäuscht.

## Risikoklasse

Mittel: breite Produktionsvorlage und markennahe Gestaltungsrichtung, aber ausschließlich reversible, klar gekennzeichnete Dokumentationsentwürfe ohne Produkt- oder Architekturänderung.

## Definition of Done

Die konkret beauftragte Ausarbeitung und ihre Eigenprüfung sind vollständig und persistent. Die oben verlangten Belege, tatsächlichen CI-Ergebnisse, Einschränkungen und noch fehlenden Freigaben liegen vor. CURRENT_STATE und WORK_QUEUE enthalten additive branchlokale Hinweise. Produkt-/Rechtsfreigabe wird nicht behauptet; fehlende vorgeschriebene Nachweise halten WP-035 offen. Gestaltungsentwurf kann vollständig geliefert sein, ohne dadurch Production- oder unabhängige Abschlussfreigabe zu erhalten.

## Bearbeitungsstatus

**Fachliche Ausarbeitung geliefert; WP-035 bleibt offen.** Gemeinsamer Erst-Add von WP/Manifest: `ec7ed3aa35c40363b0ccb0842bdb14c3fee28039` mit direkter Eltern-/Manifestbasis `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`; danach fachliche Ausarbeitung im verankerten Dokumentationsscope. Manifest unverändert.

Geliefert: ein zusammenhängender Hauptreport, sieben vollständige Fachkapitel A–G, drei SVG-Konstruktionen, acht statische HTML-Quellen und zehn PNG-Ansichten (acht lokale Renderings plus zwei originale AI-Materialtafeln), drei unveränderte IBM-Plex-Original-TTFs mit gepinnter Herkunft/Lizenz/SHA-256, 40 nachverfolgbare Anforderungen, zwei reproduzierbare Liefer-/Renderingwerkzeuge und konkrete Nachweise. Hauptfundstelle: [vollständige Asset-Bible](../PROJECT_CONTROL/AssetBible/WP-035/README.md). Die AI-Tafeln sind ausdrücklich illustrativ, keine Maß-/Font-/Prop-Master; exakte Konstruktionsquellen liegen separat vor.

Tatsächliche Eigenprüfung: Lieferdateien/Links/HTML-Fontpfade/PNG/SVG/JSON lesbar; drei Originalfont-Hashes und deutsche Glyphen stimmen; 40 eindeutige Inventarzeilen mit vorhandenen Referenzen; 28 Countzustände passend zur dargestellten Belegung und 17 dokumentierte Kontrastpaare bestanden. Alle zehn finalen PNGs gesichtet; keine fremden Produktions-/Konzept-/Architektur-/Workflowdateien geändert. Der eigene unveränderte Architekturvalidator hat in der gepinnten Python-3.11.13-Umgebung mit eigenem Dokumentationsmanifest und --self-test RESULT PASS erreicht und die reguläre Draft-PR-CI am tatsächlichen Head nachgewiesen. Kein Ergebnis wird als unabhängige Abnahme ausgegeben.

Letzter Schritt: vollständige Gestaltung und eigene Lieferprüfung. Nächster Schritt: eigener Draft-PR mit tatsächlicher Architektur-/CI-Evidenz; ausdrückliche Annahme oder Änderung der im Hauptreport konkret benannten Vorschläge. Owner-/Produkt-/Rechtsfreigabe und spätere Runtime-/Medienproduktion ausstehend. Kein Merge beauftragt, keine fremden Branches integriert.
