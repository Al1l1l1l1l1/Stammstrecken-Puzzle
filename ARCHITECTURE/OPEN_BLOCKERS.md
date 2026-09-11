# Offene Blocker nach Architecture v0.3

**Stand:** 2026-09-12

Architecture v0.3 ist als technische Grundlage vollständig. Die folgenden fehlenden Produktentscheidungen werden nicht durch Architekturannahmen ersetzt. Jeder betroffene Teilbereich bleibt **fail-closed**, bis die dokumentierte Unblock-Bedingung erfüllt ist. Lokaler Kampagnen-Rätselkern, Grundfortschritt, Save, UI-Scaffold und nicht betroffene Architekturmodule können unabhängig davon umgesetzt werden.

## BLOCKER-PROD-001 – Persistenter Hinweisanspruch

| Feld | Inhalt |
|---|---|
| Status | **Offen** |
| Blockierter Bereich | Finale `IHintPolicy`, Hint-Entitlement im Save, reguläres Hilfekontingent und freiwilliges Hinweisvideo. |
| Fehlende Entscheidung | „Auf jedem neuen Level einmal kostenlos“ ist bestätigt, aber „neu“ ist über Abbruch, mehrere Versuche, Übungsfahrt, Betriebsrevision und Dauerbaustelle nicht präzisiert. Quelle, Größe und Erneuerung des „regulär verfügbaren Hilfekontingents“ sind ebenfalls nicht festgelegt. |
| Sichere Zwischenregel | Solverhints werden ausschließlich aus dem öffentlichen Puzzle ohne Spielerannahmen bewiesen. Die Auswahl darf keine nicht objektiv fehlerhafte X-, Grau- oder konkrete Annahme als falsch enttarnen. Ist kein sicherer Hinweis auf einer unberührten Zelle verfügbar, wird kein widerspruchsauflösender Hint ausgegeben. |
| Fail-closed | Keine finale Hint-Economy und kein Rewarded-Hint-Placement veröffentlichen. Ein technischer Fake darf nur Testcredits verwenden. |
| Benötigte Freigabe | Product Owner entscheidet Lebensdauer/Modusbezug des kostenlosen Anspruchs, reguläre Creditquelle/-menge, Verbrauchszeitpunkt und Verhalten bei technisch nicht lieferbarem Hint. |
| Abschlussnachweis | Produktdatei fortgeschrieben; `IHintPolicy` per neuem/ersetzendem ADR präzisiert; Save-Migration und Policy-/Rewardtests grün. |

## BLOCKER-PROD-002 – Kalendertag der Betriebslage des Tages

| Feld | Inhalt |
|---|---|
| Status | **Offen** |
| Blockierter Bereich | Anspruchsberechnung und Veröffentlichung der täglichen kostenlosen sowie werbebasierten Teilnahme. |
| Fehlende Entscheidung | „Einmal pro Kalendertag“ ist bestätigt, aber maßgebliche Zeitzone und Verhalten bei Offlinebetrieb, Reise, Zeitzonenwechsel und manueller Uhränderung fehlen. |
| Sichere Zwischenregel | Save reserviert einen versionierten `DailyEntitlementRecord`, führt aber keine lokale-/UTC-Anti-Rollback-Policy als Wahrheit ein. |
| Fail-closed | Betriebslage des Tages bleibt bis zur Freigabe deaktiviert; normale Rätsel und Fortschritt bleiben verfügbar. |
| Benötigte Freigabe | Product Owner bestätigt Tagesgrenze, Kulanz-/Missbrauchshaltung und Nutzerkommunikation. |
| Abschlussnachweis | Produktdatei fortgeschrieben; eigenes/ersetzendes ADR; Zustandsautomat sowie Zeitzonen-/Offline-/Uhrtests grün. |

## BLOCKER-PROD-003 – Generator-Qualitätsprofil der Dauerbaustelle

| Feld | Inhalt |
|---|---|
| Status | **Offen** |
| Blockierter Bereich | Veröffentlichung generierter Dauerbaustellenlevel und finale automatische Schwierigkeitszuordnung. |
| Fehlende Entscheidung | Es existieren bestätigte Qualitätsdimensionen, aber keine kalibrierten Metrikintervalle, Novelty-Metrik/-Schwelle, Referenzfenster oder Freigaberegel je Schwierigkeitsklasse. |
| Sichere Zwischenregel | Generator und Solver dürfen als internes Tool implementiert und technisch getestet werden. Jeder Kandidat bleibt `DRAFT` und unveröffentlichbar. |
| Fail-closed | Kein generiertes Level darf in Production erscheinen, solange kein `GeneratorQualityProfile` mit Status `PRODUCT_APPROVED` im Release-Lock steht. |
| Benötigte Freigabe | Product Owner/Leveldesign kalibrieren Profile aus menschlich geprüften Rätseln; Solver-/Contentverantwortliche versionieren Metrik- und Noveltyvertrag. |
| Abschlussnachweis | Profil und Referenzkorpus versioniert; Product-Approval dokumentiert; Qualitäts-, Regression- und Human-Review-Gates grün. |

## Bekannte offene Produktpunkte ohne aktuellen Architekturblock

| Punkt | Warum kein Architecture-v0.3-Blocker |
|---|---|
| Finaler Werbefrei-Preis | Produkt-ID und Adapter sind vorbereitet; Preis blockiert erst Storekonfiguration/öffentlichen Release. |
| Finale Zwei-/Drei-Sterne-Zeiten | Levelschema erlaubt ausdrücklich `null`; korrekte Lösung und Grundfortschritt funktionieren mit einem Stern. |
| Finale Touchgesten | Alle Gesten mappen auf dieselben Commands; Domain und Ports bleiben stabil. |
| Finale Accessibility-/Motionwerte | Architektur enthält semantische UI- und Testgrenzen; konkrete UX-Abnahme folgt separat. |
| Finale Orientierung | Layout ist konfigurierbar; die Entscheidung verändert keine Domain. |
| Finale Marke, Assets und Zugkatalog | Asset- und Katalogverträge sind vorbereitet; öffentliche Freigabe bleibt ein späteres Produkt-/Rechtsgate. |
| Store-, AdMob-, Firebase- und Signingzugänge | Externe Voraussetzungen des Implementierungs-/Release-Work-Packages, nicht der Dokumentarchitektur. |

## Änderungsregel

Ein Blocker wird nicht allein in dieser Datei geschlossen. Zuerst wird die fehlende Produktentscheidung ausdrücklich bestätigt und in der zuständigen Produktquelle persistiert. Danach dokumentiert ein neues oder ersetzendes ADR die technische Folge. Erst ein Work Package mit Tests darf den Status auf **Geschlossen** setzen.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/06_Fortschritt_Belohnungen_und_Meisterschaft.md "Stammstrecken-Puzzle – Fortschritt, Belohnungen und Meisterschaft"
[3]: ../Stammstrecken_Puzzle_Konzept_00-15/08_Faire_Monetarisierung_und_Werbeangebote.md "Stammstrecken-Puzzle – Faire Monetarisierung und Werbeangebote"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Train Track Spiel – Schwierigkeit, Feldgrößen und Levelgenerierung"
