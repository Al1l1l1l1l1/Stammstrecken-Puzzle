# ADR-045 – Explizite Operatorpackwahl und globaler Human-Laufübergang

## Status

**Angenommen** – Releaselead R am 21.10.2026 innerhalb WP046 nach getrennter C-Fachreview des exakten Entwurfs SHA256 `9aaada8186184283914862861c31937f74e8d770cf272fc84d3faed3f70a2f39`, vor abhängiger Implementierung.

## Datum

2026-10-21

## Kontext

Die aktuelle Human-Szene referenziert nur das Defaultmanifest. Die übrigen 16 Projektassets sind damit keine belegten Playerabhängigkeiten. Der bestehende Store bindet genau ein Assignmentcatalog; ein anderer globaler Manifesthash wird zu Recht als `HUMAN_INDEX_SOURCE_UNSUPPORTED` zurückgewiesen. Initiale Compositionwahl ist nur bei wirklich global fehlendem Lauf möglich. Die sichere Verteilung aller 102 Aufgaben über spätere Teilnehmerläufe benötigt deshalb die folgende begrenzte Ergänzung zu ADR042.

## Entscheidung

1. **Geschlossene Packregistry.** R referenziert alle 17 vorhandenen Teilnehmermanifest-Textassets sowie Rawlock und UI-Lock ausdrücklich in der Human-Szene. Vor jeder Nutzung werden vollständiges geschlossenes Fileset, eindeutige Pack-/Manifestidentitäten, tatsächliche Raw-SHAs, aktueller Publickatalog und tatsächliche UI-Bindung geprüft. Jedes Pack bleibt ein unverändertes Sechsermanifest. Es gibt keinen neuen Gesamträtselkatalog für freie Spielauswahl und keine Lösung, Proof-, Parameter-, Seed- oder Proxyquelle im Player. Fehlende, zusätzliche, doppelte oder falsch gebundene Quellen sperren den Einstieg; kein Defaultfallback.

2. **Read-only Discovery vor Auswahl.** Eine B-verantwortete globale Beobachtung liest unter derselben physischen Writersperre alle relevanten aktiven Index-/Backup-/Temp-/Recoveryrollen und die zugehörigen Record-/Archivbelege. Ihr Resolver akzeptiert ausschließlich die 17 tatsächlich rawgelockten Manifesthashes und liefert die vollständigen jeweiligen Originalcatalogbytes. Das Ergebnis ist `Missing`, eine eindeutig gebundene bestätigte Laufautorität oder ein gesperrter Recovery-/Unsupportedzustand. `Missing` ist nur bei tatsächlich leerem globalem Humanbereich zulässig, niemals aus dem Fehlen eines einzelnen Packverzeichnisses. Discovery, Operatoröffnung, Auswahl und Refresh schreiben keine Beobachtungsdateien. Eine anschließende reguläre Wiederaufnahme darf den exakt bestehenden Lauf gemäß ADR042 konservativ als unterbrochen/unsicher fortschreiben; sie erzeugt keine neue Lauf-/Manifestautorität und ersetzt keinen Sitzungstoken oder Export. Ein einzelner syntaktisch lesbarer Hauptindex ist keine ausreichende Autorität.

3. **Initiale Wahl und Fortsetzung.** Nur bei globalem `Missing` kann der Operator eines der 17 Packs wählen. Diese Wahl bleibt bis zum ausdrücklichen Start flüchtig. Ein vorhandener offener Lauf bestimmt zwingend sein exaktes Manifest und wird über dieselbe vorhandene `AssignedHumanSession` fortgesetzt; Packwechsel ist gesperrt. Ein abgeschlossener Lauf bleibt einschließlich bestehendem Export unverändert zugänglich. Unbekannte oder nicht zum tatsächlichen Build/UI passende Quellen werden nicht auf ein gleichnamiges neueres Pack umgebunden. Originale bleiben erhalten; eine passende Quellenfassung oder ausdrückliche Recovery ist erforderlich. Diese Entscheidung ist keine stillschweigende Cross-Version-Migration.

4. **Bewusster nächster Teilnehmerlauf.** Ausschließlich nach bestätigt vollständigem Alt-Lauf darf der Operator das nächste Pack wählen und einen separaten Dialog bestätigen. Abbrechen und bloßes Auswählen ändern keine dauerhaften Daten. Der Dialog beschreibt die konkrete Packgruppe und sechs Aufgaben; die Bestätigung fordert einen getrennten Teilnehmerlauf an, attestiert aber keine tatsächliche neue Person. Die bisherige Einpack-API bleibt kompatibel und darf denselben sicheren Übergang für ihr bisheriges Ziel verwenden. Kein automatischer Packzyklus, kein Start beim Booten/Refresh/Export und kein neues Token allein wegen eines Manifestwechsels.

5. **Ein globaler manifestgebundener CAS.** Der bestätigte Start vergleicht unter dem einzigen bestehenden globalen physischen Writerlock exakt die unten geschlossene `expectedAuthority` mit der frisch beobachteten globalen Autorität. Dies gilt ausdrücklich auch für den Initialstart mit `Missing`. Der Zielhash muss zum zuvor geprüften, für diesen Build/UI zulässigen Pack gehören. Bei `Confirmed` muss der gebundene Alt-Lauf weiterhin vollständig bestätigt sein; `Recovery` und `Unsupported` sind niemals startfähig. Seine Originalrecords, Archivindexe und bereits erzeugten Exportbytes werden nicht überschrieben, umetikettiert oder gelöscht. Ein globaler Index darf nicht erfolgreich auf einen fehlenden oder unbestätigten Zielrecord zeigen. Erst nach vollständigem Commit/Reread-ACK wird die neue leere Zielsession gebunden; vorher existiert kein neuer Puzzleview. Zielrecord und Index gehören zu derselben bestätigten Startoperation, nicht zu einem vorgelagerten stillen Packwechsel.

6. **Manifestübergreifende Recovery ohne Umgehung.** Alte gültige Backup-/Archivrollen werden über ihren eigenen exakten Manifestcatalog geprüft, nicht mit dem Zielcatalog fehlinterpretiert. Fremde historische Indexrollen sind nur zusammen mit ihrem exakt archivierten bestätigten Completed-Record zulässig. Vor jeder Rollenbereinigung muss ein bestätigter Recoveryanchor die tatsächliche aktuelle Manifest-/Tokenautorität erhalten. Es gibt keine Transitiondatei oder neue persistierte Übergangsform. Ein nach Crash unbestätigter verwaister Zielrecord sperrt Start und Exposition; seine Bytes werden ausschließlich bei ausdrücklicher Recovery mit vollständigem Originalarchiv behandelt, niemals automatisch zur Autorität promoviert oder still gelöscht. Widersprüchliche Rollen, unbekannte Quellen und unklare Writergebnisse bleiben ebenfalls gesperrt. Konkurrenz, Replay und doppelte Bestätigung erzeugen höchstens den einen bestätigten Ziel-Lauf oder einen sichtbaren Konflikt; kein zweiter Token aus einem verspäteten Callback. Alte Sessioncallbacks dürfen nach dem Wechsel weder einen Record weiterführen noch die globale Autorität zurücksetzen. Alle folgenden Writes rechecken Manifest und Token zusätzlich zur Generation. Es gibt keinen anderen Root, separaten Packindex, Indexreset oder Verzeichniswechsel als Ausweg.

7. **Vor-Exposition-UI und bestehender Spielpfad.** Der Operator sieht ausschließlich neutrale Gruppen-/Packnummern und Aufgabenzahl, keine konkreten Rätsel, Titel, Zahlen, Endpunkte oder Vorschau. Die 17 Einträge stehen in einer mobilen Scrollliste; aktive Bindung und gesperrte Gründe sind sichtbar. Auswahl/Bestätigung bleibt außerhalb der Spielschicht, durch External-/Focusfences geschützt und gegen doppelte Pendingaktionen gesperrt. Danach gelten unverändert ADR042-Expositionsreservation, VisibleReceipt, Selbstauskunft, Hinweishistorie, Domainzeit und derselbe Grid-/Werkzeug-/Fahrtpfad. Ein neuer echter Lauf ist eine persistierte Beobachtung mit `PARTICIPANT_UI` oder `AUTOMATED_TEST`, kein Human-PASS und keine Personenattestation.

## Geschlossene Datenträger- und CAS-Formen

Die bestehende persistierte Indexform bleibt unverändert und geschlossen, einschließlich der bestehenden Completed- und Recoveryindexrollen:

```text
{humanIndexVersion:1, manifestBytesSha256, sessionToken, generation, recordBytesSha256}
```

Es gibt keine zusätzlichen Felder oder neue Version. `manifestBytesSha256` und `recordBytesSha256` sind die tatsächlichen kleingeschriebenen 64-stelligen SHA256 der gebundenen Originalbytes, `sessionToken` bleibt ein nichtleerer GUID-N-Token und `generation` die positive bestätigte Beobachtungsgeneration. Bestehende Record-/Recoveryanchorformen, deren JCS-/Hashbytes und Beobachtungsexporte bleiben unverändert. Insbesondere wird **kein** `active-transition.json`, Phasenflag, Transitionrecord oder neues Hashprofil eingeführt; Bs früherer Transitiondateivorschlag ist verworfen.

`InspectAsync` liefert für einen startfähigen Zustand genau eine unveränderliche, nicht persistierte Authority-Union:

```text
Missing   {globalObservationToken}
Confirmed {manifestBytesSha256, sessionToken, generation, recordBytesSha256, globalObservationToken}
```

Dies sind die vollständigen jeweiligen Nutzfelder, keine offene Erweiterungsmenge. Der opaque `globalObservationToken` ist der frische Beobachtungsfingerabdruck des gesamten relevanten globalen Quellen-/Rollenzustands einschließlich verwaister Kandidaten; er ist keine Person-, Fortschritts- oder Startberechtigung. Unter demselben globalen physischen Lock muss der Start alle Felder des gewählten Unionarms frisch und exakt vergleichen, bevor er schreibt. `Missing` verlangt zusätzlich weiterhin den tatsächlich leeren globalen Bereich. `Confirmed` verlangt weiterhin die exakte, vollständig bestätigte Alt-Laufbindung. Recovery-/Unsupportedzustände liefern keine startfähige Authority; weder ein passender Token allein noch eine passende Generation allein genügt.

## Begründung

Die globale Manifestautorität muss auch beim bewussten Teilnehmerwechsel eindeutig bleiben. Die geschlossene flüchtige Authority-Union bindet eine frische Gesamtbeobachtung, ohne Beobachtungs-/Index-/Exportbytes umzudeuten.

## Betrachtete Alternativen

Getrennte Packroots oder ein Indexreset umgehen die globale Autorität. Ein automatischer Packzyklus erzeugt unbeabsichtigte neue Läufe. Eine neue Transitiondatei ist für den geschlossenen, bei unbestätigten Orphans sperrenden Übergang nicht erforderlich.

## Konsequenzen

Alle17 Quellen werden explizit montiert; neue Discovery-/CAS- und Operatorbedienung entstehen im Staging. Echte Exposition und menschliche Annahme folgen weiterhin ausschließlich dem bestehenden Beobachtungspfad.

## Betroffene Artefakte

- **B:** unveränderliche exakt oben geschlossene Authority-Union und Port für read-only Discovery sowie ausdrücklichen CAS-Start zum gewählten Zielmanifest; keine neue persistierte Belegform. Manifestresolver nur als Applicationport, ohne Application→Contentkante. Globale Sperre/CAS, eigene Codecs je tatsächlichem Manifest und Recovery bleiben B-Storeverantwortung. Bestehende Beobachtungsbytes und ADR042-Felder werden nicht still geändert.
- **R:** rawgeprüfte Registry, alle Assetreferenzen, Bootstrap-/Szenen-/Buildmontage und Wechsel zum bestätigten Sessioncatalog. Da `AssignedHumanSession` readonly Catalog/Repository hält, wird erst nach bestätigtem globalem Übergang eine neue Session für den Zielcatalog konstruiert und angezeigt. Bis dahin bleibt die alte Session geschlossen erhalten; UnknownOutcome hält beide Spielpfade zu, und ausdrückliches Retry entdeckt die tatsächliche Bindung. Das UI erhält ausschließlich Readmodels und explizite Callbacks; es liest oder schreibt keine Dateien. Eine alte Presenter-/Sessioninstanz wird erst nach bestätigter Routingentscheidung sicher abgelöst; verspätete Tasks bleiben gefenced.
- **A:** kleine neutrale Operatoransicht vor dem gemeinsamen Humanbinder und optionaler bewusster Folgepersonen-Hook, mit lokalisierter Auswahl/Bestätigung/Abbruch und bestehender Accessibility-/Safearea-/Externalgrenze. Keine neue Rätselbedienung. Erst nach R-Annahme spiegelndes Staging; noch keine Liveänderung.
- **C:** getrennte prospektive Vertrags-/Recoveryreview und spätere tatsächliche Packaged-Asset-/Nativeisolationprüfung. Der `.human`-Provider-/Netzwerkfence bleibt vollständig erhalten.

## Validierung

Getrennte C-Fachreview und prospektive R-Annahme erfolgt. Die folgenden tatsächlichen Implementierungsnachweise stehen noch aus:

- Alle 17 Rawlockmanifeste nachweislich Szenenabhängigkeit und im tatsächlichen Humanartefakt erreichbar; unveränderte öffentliche Teilnehmerbytes, keine internen Auditassets.
- Global `Missing` → Auswahl/Abbruch/Neustart erzeugt keinen Lauf; erst bewusster Start genau einen. Jedes der 17 Packs kann gezielt gewählt werden, jeweils mit exakt seinen sechs Aufgaben.
- Aktiver Lauf A → Neustart/Operatoröffnung findet exakt A, kein Wechsel nach B und keine neue Erstkontakt-/Hintgeschichte. Unklarer oder fehlender globaler Index mit vorhandenen Records bleibt gesperrt.
- Vollständig bestätigtes A → Wahl B/Abbruch unverändert; bestätigter Start B erhält alle A-Originale/Exporte bytegleich und bindet erst nach ACK den neuen leeren B-Lauf. Auch A→A bleibt korrekt.
- Zwei Writer, stale Generation/Manifest/Token, doppelte Bestätigung, alte Callbacks und Prozessabbruch an jedem Flush-/Archiv-/Index-/Rereadfenster schließen den Übergang oder führen in ausdrücklich gesperrte Recovery; kein automatisches Weiterschalten oder Verwerfen unklarer Originale.
- Bis zur bestätigten Expositionsreservation kein Grid, Hinttext oder Accessibilityrätselknoten. OS-/Fokusunterbrechung überschreibt weder Auswahlabsicht noch Pause-/Pendingfence. Reale Sicht-/Geräte-/Humanannahme bleibt getrennt von automatisierten Proben.

## Ersetzt / ersetzt durch

Ersetzt keinen ADR. Ergänzt ADR042 ausschließlich um die geschlossene Operatorpackwahl/Discovery und den globalen manifestgebundenen CAS bei unveränderten Index-/Beobachtungs-/Exportformen. ADR044 bleibt dem gesonderten Sternzeitenvertrag vorbehalten. Wird derzeit durch keinen ADR ersetzt.

Die technische Annahme bestätigt noch keine implementierte Multipackmontage, verteilten Aufgaben, Teilnehmeridentität, Humanmessung, Difficulty-/Sternzeitenfreigabe oder Veröffentlichung.
