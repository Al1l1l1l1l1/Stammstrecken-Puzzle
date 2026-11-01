# ADR-046 – Bestätigte lokale Profillöschung und Writerstilllegung

## Status

**Angenommen** – Releaselead R am 21.10.2026 innerhalb WP-046 nach getrennter A-Fachreview des korrigierten exakten Entwurfs SHA256 `6043ee150ebb16b8b06cb4a21daf3606bfa2ff8a5560f9dfae64dda64b3b2f81`, vor abhängiger Implementierung. Die drei konkreten Befunde zu kalter Erstbindung, idempotenter Löschintention und Prozesscrashumfang wurden vor Annahme geschlossen.

## Datum

2026-10-21

## Kontext

P07 und Observability Abschnitt 9 verlangen lokale Datenlöschung einschließlich des redigierten Diagnoserings. Der vorhandene bestätigte Corrupt-Reset erhält Originale und ist hierfür ausdrücklich ungeeignet. Ein bloßes Entfernen von save.json ließe Backup/Pending/Archive und verspätete Providerwrites bestehen. Das Produkt hat einen appinternen serialisierten Writer, kann aber mehrere Repositoryobjekte besitzen. Der bestehende Save-v3-Fachkörper und alle Katalog-/Hash-/Grant-/Humanverträge bleiben unverändert.

## Entscheidung

1. **Ausdrückliche Handlung.** Die Produkteinstellungen bieten eine gesonderte, zunächst abbrechbare Bestätigung für das aktuell gebundene Spielprofil. Sie nennt Verlust von lokalem Fortschritt, Punkten, Sammlung und Einstellungen. Storekäufe werden dadurch weder storniert noch erneut gekauft; ihre vorhandene Wiederherstellung bleibt der Storeweg. Keine Bestätigung durch bloßes Öffnen/Refresh/Booten. Humanbeobachtungen, andere App-/Profil-/Katalogbereiche und externe Anbieter-/OSdaten sind kein Löschziel und werden in der Benutzerbeschreibung nicht als gelöscht ausgegeben.

2. **Quieszenz vor Wirkung.** Root sperrt neue Spiel-/Settings-/Dienstaktionen, pausiert die laufende Darstellung und schließt eigene Session-, Autosave- und Providercallbacks. Bereits gestartete eigene Arbeit wird beobachtet und beendet, bevor physische Löschung beginnt. Ab da ist die alte Repositoryinstanz dauerhaft stillgelegt. Kein neues Profil und kein neuer Binder, solange die Löschung nicht tatsächlich bestätigt ist. Ein unbekanntes Ergebnis zeigt einen gesperrten, wiederholbaren Zustand; es wird weder als Erfolg noch als unveränderter Spielstand ausgegeben.

3. **Genau ein physisches Ziel.** Infrastructure verwendet ausschließlich das bereits kanonisch gebundene `SaveFileSystem.ProfileDirectory` unter dessen ursprünglichem privatem Root und dieselbe exklusive Prozess-/Dateisystemsperre. Keine vom UI übergebene Datei-/Verzeichnisangabe. Vor jeder Wirkung werden Root-/Descendant-/Reparsegrenzen erneut geprüft. Gelöscht werden sämtliche regulären Profilkandidaten, Migrations-/Recoveryoriginale, zugehörige temporäre Dateien und der genau diesem Profil gehörende Diagnosering. Andere Verzeichnisse werden nicht enumeriert oder gelöscht. Symlinks/Reparsepunkte und unklare Zugriffsergebnisse sperren; kein Folgen von Links und kein rekursiver Delete ohne vorherige vollständige Zielprüfung.

4. **Kleine separate Stilllegungsdatei.** Im genauen Profilverzeichnis liegt nach einer Löschbestätigung die geschlossene technische Form `profile-lifecycle-v1.json`: `{profileLifecycleVersion:1,writerEpoch,deletePending}`. `writerEpoch` ist ein ausschließlich lokal erzeugter GUID-N-Token; `deletePending` ist Boolean. Diese Datei ist keine Spiel-/Personen-/Eligibility-/Economywahrheit, wird nie exportiert/geloggt/übertragen und enthält keine alten Nutzdaten. Sie bleibt nach erfolgreicher Löschung als leere technische Writergrenze erhalten. Fehlende Datei entspricht dem historischen Ausgangsepoch, ohne normalen Load zur Dateierzeugung zu machen. Erste bestätigte Löschung erzeugt einen neuen Epoch und `deletePending:true`; weitere Löschungen erzeugen jeweils einen frischen Epoch.

5. **Alle alten Repositorywriter gefenced.** Ausschließlich ein ausdrücklicher Load darf ein noch ungebundenes Repository unter der vorhandenen Sperre an den tatsächlichen nicht ausstehenden Epoch binden. Normales Commit, Recovery/Leasepromotion und Corrupt-Reset dürfen niemals bei ihrer ersten Operation selbst den aktuellen Epoch übernehmen. Autorisierte erste Fresh-Initialisierung ohne vorherigen Load darf ausschließlich den historischen Ausgangsepoch bei fehlender Lifecycledatei binden; nach irgendeiner Löschung verlangt auch Create zuvor einen tatsächlichen Load. Damit kann ein vor Löschung gehaltenes kaltes Repository samt altem Snapshot nicht nachträglich den neuen Epoch übernehmen. Spätere Reads/Writes eines gebundenen Objekts verlangen unverändert denselben Epoch und `deletePending:false`; nach einem bestätigten neuen Epoch bleiben alte Instanzen auch nach neuer Profilinitialisierung gesperrt. Laden/Create/Commit/Recovery/Corrupt-Reset dürfen die Löschmarke nicht ignorieren oder löschen. Generation und bestehende Kandidaten bleiben zusätzliche unveränderte CASgrenzen; kein Epoch im Savepayload.

   Eine konkrete flüchtige Löschintention bindet das exakte Repositoryprofil/Katalog, den zuvor tatsächlich beobachteten Ausgangsepoch und genau einen bei dieser Bestätigung einmal erzeugten Zielepoch. Unter derselben Sperre wird der Ausgangsepoch vor Wirkung frisch verglichen. Wiederholung derselben Intention verwendet denselben Zielepoch: aktueller Zielepoch+Pending setzt genau diese Löschung fort; aktueller Zielepoch+false bestätigt dieselbe schon abgeschlossene Operation ohne jeden Delete oder neuen Epoch, auch wenn inzwischen ein neues Profil existiert. Ein fremder oder anderweitig fortgeschrittener Epoch ergibt Conflict. Ein verlorenes ACK darf keine neue Löschung auslösen. Erst eine neue ausdrückliche Bestätigung mit frischer Beobachtung erzeugt einen neuen Zielepoch. Diese Intention wird nicht als neuer Savepayload oder zusätzliche Datenträgerform persistiert; der Zielepoch ist ihr idempotenter Receipt.

6. **Crash und ACK.** Der bestätigte Pendingmarker wird geflusht und exakt wieder gelesen, bevor der erste Nutzdatensatz entfernt wird. Unter derselben Sperre folgen vollständige Löschung/Reread und erst dann die atomare geflushte Umstellung auf `deletePending:false` bei gleichem Epoch. Die Stilllegungsdatei selbst bleibt erhalten. Ein gültiger Pendingmarker erlaubt nach Neustart ausschließlich idempotentes Fertigstellen der schon bestätigten Löschung, niemals Recovery eines alten Kandidaten. Ungültige/partielle Lifecyclebytes oder unklare Stagefiles sperren normale Profilnutzung und verlangen ausdrückliche erneute Löschbestätigung; kein Erraten oder stilles Ersetzen. Atomare Austauschstages werden unter festen eigenen Dateinamen geführt und bei normalem Read nicht übergangen. Nach erfolgreichem Abschluss darf der vorhandene explizite Fresh-Profile-Einstieg wieder genutzt werden. Keine Absenkung der normalen Save-/Backup-/Flush-/Rereadgarantien.

   Der hier garantierte Crashumfang ist Abbruch des Appprozesses mit weiterlaufendem Betriebssystem und Neustart der App, einschließlich Abbruch an jedem einzelnen physischen Schritt. Flush(true)/Reread allein wird ausdrücklich nicht als Garantie für Stromverlust, Kernelabbruch oder dauerhaft gesicherte Unix-Verzeichnismetadaten ausgegeben. Ein zukünftiger entsprechender Durabilityclaim verlangt tatsächliche plattformspezifische Verzeichnisbarrieren und passende Gerätebelege; bis dahin bleibt dieser weitergehende Claim offen. ACK bestätigt den vollständig reread-geprüften lokalen Löschzustand im oben begrenzten Prozesscrashumfang.

7. **Begrenzter Applicationpfad.** Ein eigener typisierter Löschstatus unterscheidet bestätigt, abgebrochen vor möglicher Wirkung, noch ausstehend/unklar und fehlgeschlagen. Nur Infrastructure besitzt Dateizugriff; UI erhält Status und explizite Callbacks. Root verbindet Begin/Drain/Löschen/Neustart, der Ring erhält Close/Drain vor Löschen. Der vorhandene Corrupt-Reset bleibt ein eigener datenbewahrender Recoveryweg. Keine Löschung beim Dispose allein und kein Zugriff auf einen anderen CatalogUse.

## Begründung

Ein separater minimaler Writer-Epoch verhindert, dass ein verspätetes altes Repository nach einem neuen Profil mit wieder kleiner Generation alte Nutzdaten zurückschreibt. Der Pendingmarker schützt vor Backupauferstehung nach Teil-Löschung. Beide Mechanismen benötigen keine neue Savepayloadversion und keine Änderung der Produktbelohnungen.

## Betrachtete Alternativen

Nur save.json löschen lässt Kopien bestehen. Corrupt-Reset archiviert gerade die zu löschenden Daten. Nur appinterne Cancellation schützt nicht vor einem später noch gehaltenen Repositoryobjekt. Alle persistentDataPath-Daten zu löschen verletzt Human-/Profiltrennung. Eine neue Save-v4-Migration wäre für diese geschlossene lokale Lifecyclegrenze unnötig.

## Konsequenzen

Eine einzige kleine technische Lifecycleform kommt hinzu; historische Spielstände bleiben unverändert lesbar, solange kein Löschvorgang bestätigt wurde. Löschung ist bewusst unumkehrbar und wird nur nach konkreter In-App-Bestätigung ausgeführt. Native Provider-/Store-/OSdatenlöschung und gesetzliche Ansprüche bleiben eigene zuständige Prozesse, keine behauptete Wirkung dieses lokalen Profilknopfs.

## Betroffene Artefakte

WP-046; Application-Löschstatus/Sessiongrenze, Infrastructure.Persistence SaveFileSystem/AtomicSaveRepository/Lifecyclecodec, Root MobilePreviewInstaller, Presentation.Settings und lokale Diagnoseringanbindung. Bestehende Assemblykanten bleiben unverändert. Keine Humanstoreänderung und kein Providerwechsel.

## Validierung

Vor abhängiger Implementierung getrennte technische Review und mandatierte R-Annahme. Danach: Bestätigung/Abbruch ohne Vorwirkung; gültige/korrupte/fehlende Profile; alle drei Kandidaten/Recovery-/Migration-/Ringdaten; andere Profile bytegleich; Links/Rootescape/unlesbare Dateien abweisen; Cancellation und Prozessabbruch an jedem Marker-/Flush-/Delete-/ACKfenster; gültiges Pending nach Neustart fertigstellen; ungültige Lifecycle-/Stagebytes sperren; zwei Repositoryobjekte und verspätete Grants nach neuer Initialisierung; keine doppelten Rewards oder Wiederherstellung alter Daten. Tests nur in kontrollierten temporären Wurzeln, niemals an echten Nutzerprofilen. Physische Plattform-/Geräteprüfung bleibt gesondert erforderlich.

## Ersetzt / ersetzt durch

Ersetzt keinen ADR. Ergänzt ADR035/037/038/044 ausschließlich um die ausdrücklich bestätigte lokale Löschung mit separater Lifecycle-/Writergrenze; Save-/Katalog-/Hash-/Grant-/Provider-/Humanfachkörper bleiben erhalten. Wird derzeit durch keinen ADR ersetzt.
