# Prospektiver I-Nachtrag: Auswahl, Fortsetzen und echte ACK-Halteprobe

19.10.2026. Nur ignoriertes Planaddendum nach konkreter R-Ausführungsentscheidung; keine Fachquelle, neue Source-/Testhilfsdatei, WP-ID oder Implementierung. P, prospektiver I-Anker und angenommene I-ADR bleiben vor Code erforderlich. Dieser Nachtrag präzisiert ausschließlich die Auswahl-/Resume-Semantik und die bereits vorgesehenen echten Smokeproben. Die folgenden bisherigen Referenzen bleiben byteunverändert:

| Referenz im selben Verzeichnis | SHA256 |
|---|---|
| I_UI_PROSPECTIVE_PLAN.md | 479b793aa284025de4a43afadfc09f3e79878a0fe3d449dab09946fe31eaab17 |
| I_PROSPECTIVE_PLAN.md | 97915aa7be73d4f88ab2f52e92070f9718a96612017e5ee789019e39e0bd6852 |
| I_PROSPECTIVE_PLAN.json | 883e5affbaf14adcb20277849e6967907e87fbad429de60d3b6dca9ce4e097f4 |
| I_UI_STARTUP_API_ADDENDUM.md | 6910c9713810399c2df7537574489fdc0e39e1bf88b198d9ed06edac7273a567 |
| I_UI_STARTUP_API_ADDENDUM_V2.md | 1821a6bab4de80ecd17e183602462cac4b6a418627bf52b429db61689a1fc59e |

## Flüchtige Auswahl und ausdrücklich gestartete Appsession

Ein Dropdownwechsel ändert ausschließlich den lokalen vorgemerkten Index des SavedPresenters. Er ruft weder SelectPuzzleAsync noch Start/Restore/Restart/Save/Flush auf und erzeugt keinen Ready-Draft. Die tatsächliche laufende Appsession, PuzzleView und State.SelectedIndex bleiben unverändert. Der äußere Rahmen unterscheidet die vorgemerkte Auswahl sichtbar von einem bereits laufenden Puzzle; Refresh darf den vorgemerkten Index nicht fortlaufend aus State.SelectedIndex zurücksetzen.

Zwei ausdrücklich lokalisierte Aktionen übernehmen den zum Klickzeitpunkt erfassten vorgemerkten Index:

| Sichtbare Aktion | UI-Request | Tatsächliche Bridgeaktion |
|---|---|---|
| Neuen Versuch beginnen | OpenPuzzle/index | SelectPuzzleAsync(index, false, ct) |
| Gespeicherten Entwurf fortsetzen | ResumeDraft/index | SelectPuzzleAsync(index, true, ct) |

Die Aktionen benutzen die bestehenden beobachteten Task-/Busy-/Versionbindungen. Die Bridge prüft ihre tatsächlichen aktuellen Gates erneut. Nur die ausdrücklich gewählte Aktion beginnt den bereits geplanten serialisierten Sessionwechsel mit erforderlichem Flush; fehlgeschlagener oder unvollständiger Wechsel bestätigt keinen neuen State.SelectedIndex. Ein ausdrücklich neuer Versuch darf den Draft derselben Identität gemäß S-Vertrag ersetzen. Dropdown, Refresh, Initialload und bloßes Anzeigen der Fortsetzenoption dürfen diesen Ersatz nicht vorwegnehmen. Es entsteht kein zusätzlicher persistierter Auswahlwert.

Der App-Owner ergänzt in derselben bereits vorgesehenen neuen SavedPublic240ReviewSession-Datei die rein lesende Projektion `bool HasDraftForPuzzle(int index)`. Sie bezieht sich ausschließlich auf den aktuell bestätigten, vollständig validierten und passend gebundenen Snapshot und vergleicht den Draft mit der aktuellen Katalogidentität über das vollständige ordinale Tripel `PuzzleId`, `HashProfile`, `HashSha256`. Gleiche puzzleId allein, unbestätigte Dirtyprojektion oder ein alter Snapshot aus unklarer Recovery begründen keine Fortsetzenfreigabe. Ungültiger Index oder fehlende bestätigte Grundlage liefert keine Draftverfügbarkeit. Die Projektion führt weder IO noch Mutation aus und ersetzt keine erneute Restorevalidierung innerhalb SelectPuzzleAsync. HasDraftForSelection bleibt gegebenenfalls die Projektion der tatsächlichen Appauswahl; der Presenter fragt die vorgemerkte Auswahl ausdrücklich mit HasDraftForPuzzle ab.

Fortsetzen ist nur mit tatsächlicher Draftverfügbarkeit und den aktuellen Appgates erreichbar. Verliert die Grundlage zwischen Anzeige und Klick ihre Gültigkeit, darf daraus kein neuer Versuch als stiller Fallback entstehen. Der Fehler bleibt benannt; nach frischem Load muss der Nutzer erneut ausdrücklich wählen. Ein neuer bestätigter Load ersetzt die bisherige Session gemäß bestehendem Vertrag und verlangt wieder eine bewusste Start-/Fortsetzenaktion.

## Enge geplante Nachweise in den bereits vorgesehenen Testdateien

Presenter-/Bridgeprüfungen belegen: mehrfacher Dropdownwechsel verursacht null Apprequests und keine Änderung von Session, Draft, Generation oder State.SelectedIndex; ein vorhandener Draft bleibt vor Fortsetzen erhalten; beide Buttons senden den erfassten richtigen Index mit unterschiedlichem Restoreflag genau einmal; geänderte Hashidentität bei gleicher puzzleId bietet kein Fortsetzen; unbestätigter oder unklarer Snapshot liefert keine Verfügbarkeit. Expliziter neuer Versuch ersetzt den passenden Draft nur über den bestehenden S-Ablauf. Fehlender oder inzwischen ungültiger Draft fällt niemals auf Start(false) zurück. Busy-/Fehlerzustand eröffnet keine zweite Navigation und keine Fahrt-/Resultfreigabe.

Die tatsächlichen Saved-Szenentests setzen UseTestPrivateRoot vor dem tatsächlichen Saved-Awake beziehungsweise dem expliziten Szenenladen. Das normale TestRunner-EmptyScene-Bootstrap ist keine implizite Savedinitialisierung. Der Scope bleibt bis nach vollständigem Installer-/Task-/Objektteardown erhalten; echte SaveFS, Kataloge und Profil r bleiben unverändert.

Für die reale ACK-Halteprobe hält ein dedizierter Testownerthread die echte SaveFileSystem.AcquireExclusive-Sperre. Derselbe Thread führt Erwerb und Dispose aus; ein Ready-Signal bestätigt tatsächlichen Erwerb, ein Release-Signal fordert Freigabe an. Unitymain wartet ausschließlich per Frames beziehungsweise beobachteten Tasks, pumpt Dispatcher/Presenter weiter und kann dadurch ältere Publikationen drainen. Kein Task.Run-Erwerb mit Dispose auf Unitymain. Der Owner meldet Fehler beobachtet zurück; sein finally entsorgt die Sperre auf dem Ownerthread. Auch Assertionfehler/Abbruch setzen Release, warten begrenzt auf den tatsächlichen Threadabschluss und melden fehlende Freigabe ausdrücklich, statt den Tempbaum unter laufendem Besitzer zu entfernen. Hilfen/Signale bleiben innerhalb der bereits vorgesehenen SavedPublic240ReviewSmokeTests-Datei.

Die Halteprobe zeigt vor realer ACK keine Fahrt/Results und nach expliziter Freigabe nur den regulär bestätigten eigenen Übergang. Als separate echte Fehlerprobe kann nach bestätigtem Load der erwartete save.pending.json-Pfad im isolierten TempRoot mit einem realen Verzeichnis belegt werden. Der Writer muss sichtbar scheitern; Mainbytes vor/nach und ausbleibende Fahrt/Results werden geprüft. Nach Entfernen ausschließlich dieses eigenen Hindernisses folgt ein ausdrücklich ausgelöster Retry. Keine Fake-ACK, kein Runtimefaultservice und keine Wiederholung der bereits exakt belegten S-Lowdisk-/Abbruchfenster.

## Referenzpaket und Grenze

Die fünf oben gebundenen Originale zusammen mit diesem separaten Nachtrag bilden das bereitzuhaltende prospektive Planpaket. V2 bleibt für Worker-ohne-Bridge, Mainthreadkonstruktion/einmaliges BindModel, Pump ohne Modell und erreichbares explizites Reload nach gebundenem Requestfault maßgeblich. Die neuen Auswahlregeln verlagern nur die vorgemerkte UI-Auswahl; Save-/Recovery-/ACKautorität bleibt ausschließlich in der tatsächlichen App-/S-Grenze. Dieser Nachtrag beansprucht weder einen bereits angenommenen I-ADR noch einen I-Anker, durchgeführten Test oder Implementierungsbeginn.
