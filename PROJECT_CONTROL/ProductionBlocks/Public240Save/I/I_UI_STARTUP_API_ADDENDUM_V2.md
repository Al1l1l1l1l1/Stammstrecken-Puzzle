# Prospektiver I-Startup – enges V2-Delta

19.10.2026. Nur ignoriertes Planaddendum, kein I-Code und keine Source-/Unity-/Gitaktion. Das ursprüngliche [Startup-Addendum](I_UI_STARTUP_API_ADDENDUM.md), SHA256 `6910c9713810399c2df7537574489fdc0e39e1bf88b198d9ed06edac7273a567`, bleibt byteunverändert erhalten. Die folgenden beiden konkreten R-Ausführungsentscheidungen ersetzen ausschließlich die entsprechenden Aussagen in dessen Schritten2/3 beziehungsweise Requestfault-Sperre. P, prospektiver I-Anker und angenommene I-ADR bleiben vor Code erforderlich.

## 1. Bridge ausschließlich auf dem Hauptthread konstruieren

Clock und Dispatcher werden bereits vor dem Worker auf dem Unityhauptthread erzeugt. Auch der noch ungebundene SavedPresenter mit sichtbarem lokalisiertem Loading wird dort aufgebaut.

Der Worker konstruiert ausdrücklich **keine SavedPublic240ReviewSession-Bridge**. Seine Rückgabe ist die reine Katalog-/S-/Repository-/Autosave-Composition: validierte Public-/Reviewkataloge und Bindungen, Policy/Codec, tatsächlicher FileSystem-/Repositoryadapter, PersistedPuzzleSession und der zugehörige AutosaveCoordinator. Er erhält nur kopierte Quelldaten, feste Root-/Diagnosewerte und die bereits erstellten Clock-/Dispatcherports; keine Unitygetter oder UIobjektarbeit.

Nach erfolgreicher Worker-Rückgabe prüft Root die aktuelle Startup-Epoch/Lifetime auf dem Hauptthread, konstruiert dort die echte SavedPublic240ReviewSession aus dieser Composition und bindet sie danach genau einmal per Presenter.BindModel. Erst anschließend startet beziehungsweise übernimmt Root die beobachtete InitializeAsync-Aktion der Bridge. Vor der Bindung erzeugt ein Fehler die lokalisierte Startup-Fehleransicht; nach der Bindung bleiben Save-/Loadzustände an die echte Bridge gebunden. Kein Worker darf einen View/Bridgeconstructor vorwegnehmen.

Dispatcherdrain und Presenter.Refresh laufen weiterhin auch ohne Composition, Bridge oder PuzzleView. Alte Epoch-/Disposeantworten können weder eine neue Bridge binden noch deren sichtbaren Zustand überschreiben.

## 2. Requestfault sperrt Spiel, erhält explizite Wiederherstellung

Nach einem unerwarteten **gebundenen** Requestfault bleibt eine lokalisierte Fehleransicht im äußeren Savedrahmen sichtbar. Gamehost, Mutationen, Pickerwechsel, Restart, Fahrt und Ergebnis bleiben gesperrt; keine Save-/Reward-/Commitantwort wird aus der Exception erfunden.

Der äußere Button „Erneut laden“ bleibt ausdrücklich erreichbar, sobald das fehlgeschlagene Requesttask beendet ist und kein neuer Request läuft. Er liegt außerhalb des deaktivierten Gamehosts und erzeugt genau einen beobachteten Reloadrequest. Diese Read-/Recoveryaktion wird nicht durch die UI-Faultsperre oder einen vor dem Fault gecachten CanRetryLoad-Wert unzugänglich gemacht. Root und Bridge serialisieren den tatsächlichen Aufruf; disposed oder noch laufende Operationen werden weiterhin berücksichtigt.

Erst eine vollständige frische Beobachtung kann die passende explizite Kandidaten-/Recoveryansicht anbieten. Auswahl/Bestätigung bleibt an die aktuellen Bridgegates und die vollständigen Repositorybindungen gebunden. Unsupported/Failed öffnen weder Spiel noch Reset. Eine bestätigte Spiel-/Resultfreigabe folgt ausschließlich der regulären Application-/ACKgrenze, niemals dem bloßen Abschluss des Reloadtasks.

Es gibt **keine automatische Reload-/Retryschleife**. Schlägt auch das explizite Laden fehl, bleibt die benannte Fehleransicht samt erneut bewusst auslösbarem Ladeweg sichtbar. Jede weitere Dateiwirkung benötigt weiterhin die bereits vorgesehenen expliziten Create-/Recover-/Promote-/Resetaktionen und deren bestätigte aktuelle Grundlage.

## Enger zusätzlicher Nachweis

Der Startup-Test unterscheidet Worker-Composition von Mainthread-Bridgeconstructor/BindModel. Ein gebundener Requestfaulttest prüft gleichzeitig: gesperrte Spiel-/Resultcontrols, sichtbaren erreichbaren äußeren Reloadbutton, genau einen Request pro bewusster Aktion, keine Hintergrundschleife, erneuten Fehler ohne Sackgasse und Recovery nur aus tatsächlich neu beobachtetem Zustand. Die vorhandene prospektive Testdateiliste genügt; keine zusätzliche Source wird durch dieses V2-Delta vorgeschlagen.
