# A – unabhängige additive Korrekturprüfung

**PASS_NARROW_CODE_DELTA_PENDING_NATIVE_AND_CLEAN_REBUILD.** Beide konkret beanstandeten Quellursachen sind eng und nachvollziehbar korrigiert. Originale Eingangsberichte und Native04 bleiben unverändert. Kein tatsächlicher neuer Native-/Creation-/CI-/Lieferfreeze-PASS vor deren Ergebnissen.

Der vollständige Vergleich gegen die eingebetteten unveränderten Native04-Sourcebytes zeigt exakt drei Änderungen:

| Quelle | Neue SHA256 | Exakter Delta |
|---|---|---|
| Public240ReviewInstaller.cs | `5008fbc9baafd4e65666e786747bb659dd70211407574a21f5a904480096bb6f` | Im Finished-Zweig genau `ride.Dispose(); ride = null;` ergänzt. |
| Public240ReviewBuild.cs | `4cf45282a77464775f1fd26bda3d95d5030feef5b3f9fb40868fffa58b9625d0` | Genau Panelname auf geliefertes `Public240ReviewPanelSettings` vereinheitlicht. |
| Public240ReviewSmokeTests.cs | `11a77f1a4472f05c2fd38f5a071bf97f8cb4e894a090885ed8197c8633ba8c01` | Genau ein zusätzlicher echter Unitytest, keine sonstige Teständerung. |

A-QC-01: Der fertige Ride wird vor Rückgabe des Update und damit vor der nächsten Eingabewelle vollständig freigegeben. Der neue Test `ResultsThenRestartAndSolveWithoutYield_UsesFreshRideAndReachesSecondResult` enthält ausdrücklich keinen yield zwischen erster Resultsphase, Restart und allen echten UI-Lösungscommands. Er prüft neuen Attempt, neues Rideobjekt, Dispose des alten Rides, keinen STALE_ATTEMPT und tatsächliche zweite Resultsphase. Der zusätzliche Test ist fachlich geeignet; R führt Native05 tatsächlich aus.

A-QC-02: Der konkrete Namenswiderspruch ist beseitigt. Die vier erwarteten vollständigen Scene-/Panel-/Metabytefolgen in A_REBUILD_EXPECTED.json wurden unabhängig vollständig dekodiert: alle Bytezahlen, Raw-/LF-SHA256 und Übereinstimmung mit den gelieferten Artefakten sind korrekt. Die Anleitung beschränkt Creation/Reimport auf exakt diese vier eigenen Artefakte in isolierter gesicherter Kopie, bewahrt die neutralen Quellen und fordert tatsächlichen vollständigen Bytevergleich. Der tatsächliche Missing-Panel-Creationzweig steht noch aus und kann weitere serialisierte Unterschiede offenlegen; ein lediglich gleicher Name wird nicht als Bytebeweis ausgegeben.

Kein weiterer konkreter High im engen Delta. Vollständige maschinenlesbare Bindung in A_CORRECTION_CODE_QC.json. Keine Quelldatei-, Git-, Unity- oder CIaktion durch den Reviewer. Weitere Abnahme nur am tatsächlichen neuen Ergebnis und endgültigen veröffentlichten Freeze.
