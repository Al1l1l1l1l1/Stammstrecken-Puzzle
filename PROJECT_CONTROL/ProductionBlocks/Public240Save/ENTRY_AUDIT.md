# Unabhängige Eingangsprüfung und B-CI-Nachlauf

Getrennte lesende Rolle `/root/entry_audit`, 19.10.2026. Eingangsprüfung gemäß MANUS-2026-10-19-01; dieser Bericht und seine beiden JSON-Dateien werden im gültigen WP-040-Scope, Anker `0da2b684a079c0caf9306dc21a67e2dc20e8489a`, persistiert. Keine Implementierung, Gitmutation oder Unityausführung durch diese Rolle. Root koordiniert technische Entscheidungen und Folgearbeit.

## Ausgangsbasis und historische Bindungen

B ist `3d1ac59c3f70d50e05434a54d3ddec5d282bb0c3`, der eindeutige Erstadd von `MANUS_DECISION_AND_EXECUTION_MANDATE_2026-10-19.md`; einziger Parent `fb99477da53008f4ec46492c4b243fdffa89c8ea`, Tree `51a14d09bfa19c80608133b333105d56ae850374`. Neun additive Markdownpfade. Der [Veröffentlichungsnachtrag in PR31](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/31#issuecomment-6261078217) bindet denselben SHA und Parent. Der PR-Body trägt noch den älteren Herkunftsstand; der spätere Kommentar dokumentiert das heutige Mandat.

Alle geforderten Vorfahren wurden mit `git merge-base --is-ancestor` tatsächlich bestätigt, jeweils Exit 0: WP037-Untermerge `b9b59647cf73e2dcfa05a413259ca27c18a5afdf`, MI `07ba4d3130414ee82e843823c2f04aeac230f25b`, C2 `524b45024438b93c592351d7683a7c62aec2ab8d` und `fb99477…`.

b9 hat die geordneten Parents `ff1836e293a2a5cff69773525c347b55deb67dfb` / `8dd68e22b58f126cb499fee408e98c173202a98e` und Tree `25b7907fee4e0741574c3f32867fbef03bfb1e96`; der Diff zur Lieferung ist leer. MI hat Parents `3772c0370b39ec95979fb9efdd47f4344c41a539` / C2 und Tree `f1adfa4e006169c32bd930cc6c76f1d62b98e780`; der Diff zu C2 ist leer. Dies ist eine Bindungsprüfung vorhandener Lieferungen, keine erneute Fachabnahme.

Die frische GitHub-API bestätigte PR31 offen/Draft mit Head B gegen Main `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. WP026 blieb `dc1652befd7545b4c2a2dd0580f35a4e8e05d72d`, WP035 `9e3f209cd92d491ab3a1ab51cce63677d3b5d53e`, WP037 `8dd68e2…`, WP038 C2. Im anfänglichen Inventar von 37 Remote-Branches waren keine T/A/S-Folgearbeiten erkennbar; auf B fehlten deren WPs/Manifeste, die Arbeitskopie war sauber. Das beschreibt den Eingang **vor** Roots späterem T-Anker und ist keine Aussage über den inzwischen fortgeschriebenen Arbeitsbaum.

## Ursprünglicher B-CI-Befund

Alle vier Ereignisse gehören API-seitig zu B, jeweils Attempt 1.

| Workflow / Ereignis | Run | Tatsächlicher Eingangsbefund |
|---|---:|---|
| Architecture / Push | 39209664878 | completed / failure |
| Architecture / PR | 39209670452 | completed / failure |
| Unity / Push | 39209664901 | completed / cancelled |
| Unity / PR | 39209670469 | in_progress |

Architecture-Originaljobs `117920018874` und `117920036608` melden jeweils exakt einen Fehler: `adr:status:ADR-033-interne-transiente-spielschleife.md`, `RESULT FAIL (1 Fehler)`. Folgeprüfungen wurden übersprungen. Der Unity-Push hat sechs Vor-/Evidenzjobs SUCCESS, EditMode `117920098010` CANCELLED und PlayMode/Android/iOS SKIPPED. Dieser Push bleibt CANCELLED.

Beim ursprünglichen Unity-PR-Befund waren neun Jobs SUCCESS; iOS `117932776599` lief noch im Xcode-Schritt, Seatfreigabe pending. Der damalige API-Stand trägt `updated_at=2026-10-19T07:05:40Z`. Ein exakter lokaler Beobachtungszeitpunkt wurde damals nicht aufgezeichnet und wird nicht nachträglich erfunden. Es wurde damals ausdrücklich kein terminaler Gesamt-PASS oder vierter Seatbeleg behauptet.

## Enger ADR033-Statusformatbefund

Das unveränderte `adr_check`-Pattern erwartet nach `## Status` den abgeschlossenen fett gesetzten Statusbegriff. Auf B steht die Erläuterung innerhalb derselben Fettschrift: `**Angenommen – tatsächliche zuständige Manusannahme …**`. Tatsächliche Regexprüfung: B false, Parent true. `validate.py` ist Parent→B bytegleich.

Der konkret erforderliche Fix schließt `**` unmittelbar nach `Angenommen`; Erläuterung, Provenienz, Datum und Fachkörper bleiben erhalten. Dies benötigt keine Validatorlockerung oder neue ADR033-Entscheidung. Root hat den Befund separat nachvollzogen; die eigentliche Korrektur ist Roots T-Arbeit, keine Implementierung dieser Prüfrolle.

## Tatsächlicher terminaler Unity-PR-Nachlauf

Am erneut gelesenen API-Stand, dokumentiert um **19.10.2026, 09:19:26 Europe/Berlin (07:19:26Z)**, ist [Unity-PR39209670469](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39209670469) **completed / success**, `updated_at=2026-10-19T07:18:27Z`, Attempt 1. Alle zehn Jobs sind tatsächlich SUCCESS. Die ursprüngliche in_progress-Beobachtung bleibt oben erhalten.

| Job | Job-ID | Terminal |
|---|---:|---|
| project-preflight | 117920036889 | SUCCESS |
| unity-environment-android | 117920036976 | SUCCESS |
| unity-environment-linux | 117920037062 | SUCCESS |
| unity-config | 117920037065 | SUCCESS |
| unity-environment-ios | 117920037086 | SUCCESS |
| unity-evidence-guard | 117920128157 | SUCCESS |
| unity-compile-editmode | 117920128807 | SUCCESS |
| unity-playmode-bootstrap-smoke | 117922425645 | SUCCESS |
| unity-android-development-il2cpp | 117927485374 | SUCCESS |
| unity-ios-export | 117932776599 | SUCCESS |

Der tatsächliche synthetische PR-Checkout ist `009b1227bb60fd53f9cf56833d9fa465140abe22`, Parents Main/B, Tree exakt `51a14d09bfa19c80608133b333105d56ae850374` = B. Die vier lizenzierten Originallogs nennen diesen Checkout jeweils ausdrücklich.

Die beiden Original-Testartefakte wurden über den GitHub-Connector bezogen, anschließend ausschließlich im Speicher heruntergeladen und selbst geprüft: ZIP-SHA256 entspricht jeweils dem API-Digest; XML-Wurzeln und alle tatsächlichen Testfälle wurden gezählt.

| Original | Tatsächliches Ergebnis | ZIP-SHA256 |
|---|---|---|
| editmode-results, Artefakt 12167248048 | 474/474 Passed, 0 Failed/Skipped/Inconclusive; 234.5849068 s | `39745b24e702fa2ddcb54eaeb7e833bfb4a0f7cc2a1d9f3f7fadb42f443367ed` |
| playmode-results, Artefakt 12167838466 | 49/49 Passed, 0 Failed/Skipped/Inconclusive; 9.9473139 s | `87c1176786c097ea7f21da8c7a1e9b43c943ef14be210e4e098e73cb778d9593` |

Beide Originallogs bestätigen Coveragebelege für 18 STP-Assemblies, ohne behauptete Mindestcoverage. Android und iOS protokollieren die tatsächliche BuildReport-Verarbeitung beider QA-Szenen und den jeweiligen `STP Build PASS`. Das iOS-Original meldet zusätzlich `** BUILD SUCCEEDED **` um 07:18:16.8370800Z. Keine erneute APK-/Geräte-/Grafikprüfung aus diesem Nachlauf behauptet.

Alle vier tatsächlichen Lizenzdeaktivierungen sind im Originallog als `Deactivation processed successfully.` belegt, jeweils mit Seat `2476057761288-UnityPersonal`:

| Job | Receipt UTC |
|---|---|
| EditMode | 2026-10-19T06:31:48.5099053Z |
| PlayMode | 2026-10-19T06:48:36.2663407Z |
| Android | 2026-10-19T07:05:25.2290252Z |
| iOS | 2026-10-19T07:18:19.8777720Z |

## Evidenz, Grenze und nächster Schritt

[ENTRY_EVIDENCE.json](ENTRY_EVIDENCE.json) bindet Git-/Remote-/PR-/Architectureeingänge und den konkreten Regexbefund. [B_CI_EVIDENCE.json](B_CI_EVIDENCE.json) enthält terminale API-Run-/Jobdaten, Checkout-, Build-, Coverage- und Deaktivierungszeilen samt Originalzeilennummern, SHA256 der dekodierten Originallogs und die selbst gezählten Original-XML-/ZIP-Bindungen. Kurzlebige Download-URLs werden nicht persistiert. XML-Text-Hashes sind ausdrücklich als dekodierte UTF-8-Hashes bezeichnet.

Der erfolgreiche Unity-PR löscht weder die beiden Architecture-FAILs noch den separaten Unity-Push-CANCELLED. Er ist ausschließlich B zugeordnet und kein PASS von T oder einer späteren A/S-/I-Implementierung. Keine pauschale Source-/Solver-/Textgesamtprüfung, kein neuer Grafik- oder Humanreviewnachweis, keine Produkt-/Main-/Releaseannahme. Keine Workflow-Reruns/Dispatches oder manuellen Unityläufe ausgeführt. Berichtdateien nicht committed oder gepusht; Root führt T und den Gesamtauftrag weiter.
