# Vollständige Textbibliothek – WP-036

Kanonische deutsche Produktionsquelle: [de-DE.json](../../../Localization/source/de-DE.json). Alle Einträge sind **DRAFT**, nicht produktfreigegeben und nicht in eine Runtime eingebunden. Die Textproduktion umfasst die gesamte beschlossene Season und die entschieden formulierbaren UI-Zustände. Konkrete Restentscheidungen und fremde Contentgates stehen in [HANDOVER.md](HANDOVER.md).

Die Quelle enthält vollständige Nachrichten, keine Textbausteinverkettung. Die ersten 960 Einträge folgen exakt den tatsächlichen WP-026-Keys in der Reihenfolge fünf Netzabschnitte × vier lokale Routen × zwölf Meldungen; je Meldung Titel, Status, interner Einstieg und Ergebnis. Kampagnentexte, Oberflächen und Varianten folgen danach. Eine lokale Route 01–04 wird zusammen mit ihrem Abschnitt identifiziert; die globale Routenposition 01–20 dient nur der redaktionellen Übersicht.

## Aktuelle Copy-Nacharbeit – 18.10.2026

Alle 120 verankerten Copy-Felder in 110 bestehenden Einträgen sind jetzt tatsächlich korrigiert; Source-SHA256 `2cd576dc52a7e3379dfe8a925a7e8c4201c1e24e988913b247d39f1f81b605fb`. [Audit](COPY_CHANGE_AUDIT.json) und [vollständiger Vorher-/Nachher-Vergleich](COPY_CHANGE_REVIEW.md). Ganze Lesefassung und abhängige Prüfartefakte frisch erzeugt. Eigene kanonische Prüfungen PASS; **gesonderte unabhängige Restprüfung, kreative Restannahme und neue reguläre CI am Freeze noch ausstehend**. Historische Aussagen „Nacharbeit noch nicht begonnen“ unten beschreiben ausschließlich die vorangehende Entscheidung. Alle DRAFT-/INTERNAL-/BLOCKED_PRODUCT-/Produkt-/Import-/Rechts-/Releasegrenzen bleiben bestehen.

## Aktuelle kreative Entscheidung – 17.10.2026

Die [Geschäftsführerentscheidung WP-036-CREATIVE-01](CREATIVE_DECISION.md) nimmt 1.298 Einträge kreativ unverändert an und beauftragt gezielte Nacharbeit an 110 bestehenden Keys mit 109 text-/11 shortText-Feldern. [Nacharbeitsbrief](CORRECTION_BRIEF.md), [abschließender Eigenschaftsscope](CREATIVE_SCOPE.json) und [vollständiger Lektüreumfang](CREATIVE_READING_RECORD.json) sind ohne Chat übernehmbar. Die Textquelle bleibt bytegleich zu f217ccbd; Nacharbeit noch nicht begonnen. Alle DRAFT-/INTERNAL-/BLOCKED_PRODUCT-Grenzen bleiben erhalten; keine behauptete Owner-, Produkt-, Import- oder Releasefreigabe. Die frühere allgemeine Aufforderung zur erstmaligen kreativen Bewertung ist damit für WP-036 erledigt; als nächster eigener fachlicher Schritt folgt nur die dokumentierte Nacharbeit und ihre Restannahme.

## Quellfelder und Grenzen

| Feld | Bedeutung |
|---|---|
| key | Stabiler Lokalisierungsschlüssel. Vorhandene Levelkeys werden unverändert aufgelöst. Neue UI-/Kampagnenkeys bleiben nach ihrer Bedeutung stabil. |
| text | Vollständiger deutscher Originaltext. |
| description | Einsatzort und semantische Vorbedingungen. Gehört nicht zur Spieleranzeige. |
| charLimit | Redaktioneller Längenvorschlag in Unicode-Codepoints der Quellnachricht; kein Pixel-/Zeilen-/Accessibilityvertrag. |
| shortText | Wo sinnvoll, redaktionelle Kurzfassung mit denselben erlaubten Parametern. Keine Laufzeitauswahl implementiert. |
| placeholders | Exakt erlaubte Namen und Typen. TEXT ist ein schon freigegebener lokalisierter Anzeigename, kein technischer Diagnosecode oder beliebiger Fremdinhalt. |
| status | Überall DRAFT. Das eigene Authoringschema erlaubt absichtlich keine versehentliche Produktfreigabe. Eine spätere Freigabe muss durch zuständige Rollen und Produktionsverträge erfolgen. |
| audience | PLAYER bezeichnet einen vorgesehenen Spielertext; INTERNAL bezeichnet ausschließlich interne Redaktionsinformation. Die 240 .entry-Keys sind zwingend INTERNAL. |
| availability | DECIDED bedeutet: Funktion beziehungsweise Textaussage konzeptionell entschieden. Es bedeutet weder implementiert noch runtime-verfügbar. BLOCKED_PRODUCT verlangt zusätzlich die benannten Entscheidungen. |
| blockedBy | Konkrete offene Produkt-/Darstellungs-/Katalogvoraussetzungen. Keine hier bestätigten Regeln. |
| charLimitStatus | Überall EDITORIAL_PROPOSAL. Spätere Layout- und Accessibilityannahme erforderlich. |

INTEGER-Parameter werden als nichtnegative deutsche Ganzzahlen dargestellt. FORMATTED_POINTS, FORMATTED_DURATION und FORMATTED_PRICE verlangen eine bereits korrekt für de-DE formatierte Darstellung durch die künftige Präsentationsgrenze. FORMATTED_POINTS enthält den nichtnegativen Zahlenwert ohne Vorzeichen oder Einheit; Pluszeichen und „Geduldspunkte“ stehen bei Bedarf bereits in der vollständigen Nachricht. FORMATTED_PRICE enthält die lokalisierte Preisangabe einschließlich der zum Storepreis gehörenden Währung. FORMATTED_DURATION enthält die vollständige Darstellung der Dauer. Die Bibliothek wählt keine Timerpräzision, Währungsregion, Zeitzone oder Storepreis. Kein Formatierungsadapter ist implementiert. Randzahlparameter für Spieler sind 1-basiert; interne Level-/Reviewkoordinaten bleiben 0-basiert. Parametrisierte UI-Nachrichten benötigen einen bestätigten Zustands-/Rewardrecord und sind keine eigene Preis- oder Rewardautorität.

Kein zukünftiger Import darf die Quellliste ungefiltert als sichtbare Texte behandeln. INTERNAL muss außerhalb des Spielerexports bleiben. DRAFT und offene blockedBy-Voraussetzungen sind keine Runtimefreigabe. Eine mögliche spätere Status-/Importabbildung ist Teil des vorhandenen Produktionsverfahrens; dieses WP erzeugt weder Unity String Tables noch einen Importer. Die Variante .entry ist keine automatische Hilfe. Die separaten ui.hint.template-Entwürfe sind nur nach aktiver Anfrage und geprüftem Hintpayload vorgesehen und bleiben gesperrt.

Die Eckfeldvorlage ui.hint.template.corner setzt das in Konzept 03 beschriebene belegte Eckfeld **ohne A/B-Außenanschluss** voraus: zwei zulässige innere Anschlüsse erzwingen dort die Kurve. Die Voll-/Restvorlagen verlangen entsprechend geprüfte sichere Prämissen; ungeprüfte X-/Grau-Notizen sind keine Wahrheitsquelle. Diese Einsatzgrenzen beschreiben die vorhandene Logik, keinen implementierten Hinweisgenerator oder neuen Hilfebezug.

## Prüfung und Lesefassungen

- [READING_EDITION.md](READING_EDITION.md): vollständige prüfbare Lesefassung sämtlicher Texte, reproduzierbar aus der einzigen kanonischen JSON-Quelle.
- [COVERAGE.md](COVERAGE.md): genaue Bestände, Flächen, Referenzabdeckung und Abnahmegrenzen.
- [source-bindings.json](source-bindings.json): alle 240 ID-/Key-/Level-/Reviewbytebindungen, Originaleinstieg, eigener Einstiegshash, öffentliche Fakten und individuell redaktionelle Notiz.
- [EDITORIAL_RULES.md](EDITORIAL_RULES.md): konkrete Tonregeln mit Positiv-/Negativbeispielen.
- [EDITORIAL_REVIEW.md](EDITORIAL_REVIEW.md): kritische Eigenprüfung, konkrete Stichprobe und Wiederholungsbewertung.
- [technical-validation.json](technical-validation.json): tatsächlicher vollständiger Prüflauf und erkannte Negativmutationen.
- [repetition-report.json](repetition-report.json): alle exakten Doppelungen, wiederkehrenden Sätze und häufigen Fünfwortfolgen als Prüfmaterial, kein automatisches Qualitätsurteil.
- [pseudolocale.json](pseudolocale.json): Akzente und 35% Expansion bei erhaltenen Platzhaltern, nur Layoutprüfmaterial.
- [existing-key-inventory.json](existing-key-inventory.json): sämtliche gefundenen Main-Schlüssel; isolierte Fixture-/Testkeys getrennt.
- [QC.md](QC.md): gesonderte unabhängige Prüfung am tatsächlichen Freeze, soweit tatsächlich durchgeführt.
- [CI_QC.md](CI_QC.md): gesonderter unabhängiger Nachtrag zu den tatsächlichen Push-/PR-CI-Läufen und Artefakten; nur sein ausgewiesener Schlussstand ist ein vollständiges CI-Ergebnis.

Reproduzierbarer Aufruf und Prüfumfang: [tools/text-library/README.md](../../../../tools/text-library/README.md). Der fremde WP-026-Commit wird ausschließlich als gepinnte Gitobjektquelle gelesen; seine Level, Proofs und Reports werden nicht in diesen Lieferbaum integriert. Der alte fremde FAIL-/Referenzbericht bleibt unverändert. Die eigene Abdeckung liefert tatsächliche Textquellen für 960 Ziele, ersetzt aber keine ganze Cross-Reference-, Import-, Produkt- oder Releaseprüfung.
