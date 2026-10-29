# Prospektiver I-Nachtrag – sichtbarer Startup vor der Bridge

Reine API-Präzisierung, 19.10.2026. Kein I-Code, kein Anker, keine ID und keine Source-/Unity-/Gitaktion. Ergänzt I_UI_PROSPECTIVE_PLAN und den gemeinsamen I_PROSPECTIVE_PLAN nur im ignorierten Scratch. Die zusätzliche Saved-Localization-Verwendung benötigt die von R angekündigte, getrennt geprüfte prospektive I-ADR-Annahme vor Code; dieser Entwurf nimmt sie nicht vorweg.

## Konkrete neue Presenteroberfläche

Die neue Datei SavedPublic240ReviewPresenter.cs enthält zusätzlich die kleinen UI-Intent-/Startup-Typen. Kein weiterer Sourcepfad und keine neue Assemblykante. Der Konstruktor verlangt **keine Bridge**:

```csharp
public SavedPublic240ReviewPresenter(
    VisualElement savedRoot,
    VisualTreeAsset puzzleTree,
    TextAsset savedLocale,
    Func<long> monotonicMilliseconds,
    Func<SavedReviewUiRequest, Task> runRequest);

public VisualElement Root { get; }
public bool HasBoundModel { get; }
public SavedStartupPhase StartupPhase { get; }
public void ShowStartupLoading();
public void ShowStartupFailure(SavedStartupFailure failure);
public void BindModel(SavedPublic240ReviewSession model);
public void Refresh();
public void SetTrainTexture(Texture? texture);
public void Dispose();
```

SavedStartupPhase ist geschlossen: Loading, Failed, Bound, Disposed. SavedStartupFailure ist geschlossen: InvalidAssets, ProfileMismatch, LocalStorageUnavailable, CompositionFailed. Diese UI-Lifetimezustände beschreiben nur, ob überhaupt eine Composition angebunden wurde. Sie enthalten keine Generation, keinen SaveLoadStatus, keine Kataloginstanz, keine Fortschrittsprojektion und keine spielbare Ersatzsession. Nach BindModel ist ausschließlich der vorhandene immutable SavedReviewState des Modells die gespeicherte Anzeigeautorität.

Der Konstruktor prüft auf dem Unityhauptthread die eigene Rahmentree-/Localegrenze, erstellt nur die lokale Rahmentabelle, registriert die sichtbaren Status-/Requestcontrols und zeigt sofort lokalisiertes Loading. Er erzeugt keinen A-Corepresenter, kein Grid, kein Puzzlemodel und keine Datei. Die Styles und der normale Screen-/Panelaufbau sind bereits von R am geklonten Rahmentree bereitgestellt. Der Aufrufthread wird erfasst; sämtliche Presenter-/Unityänderungen erfolgen nur dort.

BindModel verlangt eine echte nichtnull Bridge und ist genau einmal erfolgreich. Eine zweite fremde Composition oder eine Bindung nach Dispose wird benannt abgewiesen. Erst dann werden Startup-Loading/-Failure durch den wirklichen SavedReviewState ersetzt. Ein eventuell noch null PuzzleView ist normal: eigener Picker/Status bleibt vorhanden, Gamehost und Core werden erst nach tatsächlichem Start/Restore gebaut. Startupmethoden nach BindModel werden abgewiesen, damit ein verspäteter alter Compositionfehler keinen neueren gespeicherten Zustand verdeckt. Dispose ist idempotent.

## Requesttasks und Fehlerdarstellung

SavedReviewUiRequest ist ein geschlossener tief unveränderlicher UI-Intent, keine Savewahrheit. In derselben neuen Presenterdatei: Kind, optional ExpectedStateVersion, optional PuzzleIndex und optional SaveCandidateKind; validierte Fabriken verhindern widersprüchliche Kombinationen. Kinds: RetryStartup, CreateProfile, OpenPuzzle, ResumeDraft, Restart, RetrySave, RetryCompletion, Reload, ConfirmRecovery, ConfirmReset. Startupretry hat keine erfundene StateVersion; gebundene Eingriffe erfassen die echte aktuell angezeigte Version. Root prüft vor Ausführung erneut Version und Bridgegates. Die vollständige RecoveryObservation bleibt in Application/Repository.

Jeder Buttoncallback ist synchron und ruft den injizierten runRequest einmal auf. Root gibt ein **beobachtetes Task** seiner eigenen Lifetime-/Requestverwaltung zurück und führt nur dort Composition beziehungsweise die freigegebenen Bridgetasks aus. Presenter hält ebenfalls den laufenden Request, sperrt doppelte Requests und beobachtet dessen Fault/Cancel; keine async-void-Aktion, kein Wait/Result im Frame. Refresh verarbeitet abgeschlossene Tasks auf dem Hauptthread. Bei Dispose wird ein noch später faultendes Task weiterhin ohne Unityzugriff beobachtet; Root cancelt seine Lifetime und beendet wartende Dispatcherpublikationen kontrolliert.

**Taskcompletion ist niemals Saveerfolg.** Nach einer gebundenen Aktion rendert der Presenter nur den tatsächlichen Bridge-State. Ein normal abgeschlossenes Task mit abgewiesenem Load/Commit macht keine Controls oder Results erfolgreich. Eine unerwartete Requestexception erzeugt eine lokalisierte UI-Fehlermeldung, verändert aber weder Bridgezustand noch Progress-/ACKwahrheit. Root behält die technische Diagnose separat; keine rohe Exception/Exception.Message wird als sichtbarer Erfolg oder ungeprüfter Text eingesetzt.

Bei einem unerwarteten gebundenen Requestfault bleibt die eigene UI-Request-/Gamefreigabe konservativ gesperrt, bis Root den tatsächlichen Zustand über die vorhandene Reload-/Recoverygrenze neu festgestellt hat. Das ist eine zusätzliche UI-Lifetime-Sperre, keine erfundene SaveLoad-/Commitantwort und kein Ersatz für die S-Gates.

ShowStartupFailure nimmt nur den geschlossenen Fehlergrund an. Root übersetzt bekannte Workerfehler auf diesen Grund und protokolliert technische Details separat; unbekannte Fehler werden als CompositionFailed sichtbar. Neue erforderliche eigene Keys:

- qa.public240.saved.startup.loading
- qa.public240.saved.startup.failed.assets
- qa.public240.saved.startup.failed.profile
- qa.public240.saved.startup.failed.storage
- qa.public240.saved.startup.failed.composition
- qa.public240.saved.startup.retry
- qa.public240.saved.request.failed
- qa.public240.saved.request.cancelled

Zusätzlich dieselbe neue Savedlocale mit allen vorhandenen Corekeys und den schon geplanten Saved-/Recovery-/Bestätigungskeys. Kein qa.saved-Prefix, keine globale Localizationmutation und keine A-Quelländerung. Die sichtbare Fehleransicht besitzt einen Statuslabelbereich und einen passenden erneuten Startupversuch; sie wird nicht durch ein Log allein ersetzt. Fehlende Savedframe-/Localeassets selbst bleiben ein benannter Preflight-/Konstruktionsfehler; der Nachweis „sichtbarer Workerfehler“ setzt den gültigen gespeicherten Rahmen voraus und behauptet keinen Renderer ohne seine eigenen erforderlichen Assets.

## Restore und Reset

Entwurf fortsetzen ist ein eigener expliziter ResumeDraft-Buttonintent statt versteckter Autoauswahl. Für Kandidatenwiederherstellung: lokale Auswahl zeigt Kind/Status/Generation/gekürzte Sourcekennung, bewirkt noch keinen Request und keine Dateiwirkung. Erst „Diese Sicherung wiederherstellen“ erzeugt ConfirmRecovery mit gewähltem Valid-Kind und gebundener aktueller StateVersion. Application führt weiterhin read-only Recover und exakte Promotion mit vollständigen Source-/CASprüfungen aus.

Reset besitzt einen gesonderten lokalen Bestätigungsschritt mit Abbrechen. Erst der Bestätigungsbutton erzeugt ConfirmReset. Der Schritt hält die angezeigte StateVersion; jede neue Version beziehungsweise neue Observation schließt ihn, statt eine alte Entscheidung auf neue Kandidaten zu übertragen. Unsupported/Failed oder fehlendes CanReset bieten keinen Resetrequest an. Dialogzustand ist flüchtige UI-Auswahl, keine zweite Persistenzautorität. Die verbleibenden Application-/Repositorygates bleiben auch bei künstlich ausgelöstem Callback verpflichtend.

## Rootablauf vor und nach Workercomposition

1. Hauptthread: Savedrahmen klonen, Panel/Screenkamera und eigene Styles bereitstellen; eigene Locale und A-Coretree referenzieren; TextAsset.bytes in private Workerarrays kopieren. Clock und Dispatcher ebenfalls vor Worker erzeugen. Presenter ohne Modell konstruieren, Loading sofort sichtbar.
2. Root startet genau ein beobachtetes Compositiontask mit Lifetime-/Startup-Epoch. Worker konstruiert die echten Katalog-/Policy-/Codec-/Repository-/Bridgegrenzen aus den kopierten Daten; kein Unitygetter im Worker.
3. Erfolg: Root veröffentlicht BindModel mit der echten Bridge über den schon laufenden Hauptthreaddispatcher und startet/übernimmt deren InitializeAsync. Die Bridge darf bis zum echten Load selbst Loading anzeigen. Loadfehler danach sind Bridgezustände, keine rückwirkenden Startupfehler.
4. Fehler vor Bridge: Root veröffentlicht ShowStartupFailure über denselben Dispatcher. Der Rahmen bleibt sichtbar; Gamehost bleibt leer, kein Saveerfolg und keine Fakeattempt.
5. Installer.Update pumpt Dispatcher und Presenter-Startupstatus **auch ohne Composition/Bridge/PuzzleView**. Ein früher Nullmodelreturn wäre ein konkreter Fehler. Retry ersetzt nur einen abgeschlossenen/abgebrochenen Startupversuch; alte Epoch-/Disposeantworten dürfen kein neues UI binden.

## Gezielte Tests für diesen Zusatz

- Ohne Bridge/Katalog: gültiger Savedrahmen zeigt lokalisiertes Loading, keine Spielzelle, keinen Save-/Progresswert und keine Create-/Reset-/Rideberechtigung.
- Kontrolliert wartende Workercomposition: Root-/Dispatcherpump funktioniert während Model null; Status bleibt sichtbar und keine Unitymutation erfolgt auf Worker.
- Compositionfehler vor Bridge: jeder geschlossene Fehlergrund erscheint über seinen eigenen Key; Retrytask beobachtet, Doppelklick gesperrt, kein roher Exceptiontext und keine Erfolgsanzeige.
- Echte BindModel-Publikation wechselt auf Bridge-State; null PuzzleView bleibt zulässig. Zweite Bindung, Workeraufruf, Dispose und verspätete alte Taskantwort öffnen keine Controls.
- Gebundener Requesttask kann regulär abgeschlossen sein, während Load/Commit fehlschlägt; gespeicherte Anzeige bleibt dem Bridge-State treu.
- Auswahl/Anzeigen/Abbrechen von Restore-/Resetbestätigung sind ohne Dateiwirkung; nur explizite gültige aktuelle Bestätigung löst den jeweiligen beobachteten Request aus.
- Neues Saved-Locale-/Constructor-/Core-Disposeverhalten wird mit den vorhandenen geplanten I-Tests geprüft; keine neuen Testdateien für dieses Addendum erforderlich.

## Präzise Buildguardzuständigkeit

Unveränderte A-Datei Public240ReviewBuild.cs besitzt bereits Public240ReviewSceneBuildCheck für ihren eigenen Public240Review-Entry. Root erweitert ausschließlich den bestehenden DevelopmentPuzzleSceneBuildCheck um SavedPublic240Review zusätzlich zu dessen unveränderten alten zwei Szenen. Zusammen entstehen vier tatsächlich verarbeitete normale QA-Szenenbelege aus den zuständigen beiden vorhandenen Prozessoren. Der neue SavedPublic240ReviewBuild enthält **keinen** zusätzlichen Prozessguard. Keine A-Änderung oder doppelte Guard-/Receiptbehauptung.
