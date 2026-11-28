# WP-038 – unabhängige technische Integrationsprüfung

**PASS – tatsächlicher technischer Integrationsteil einschließlich neuer regulärer Push- und PR31-CI.** Keine offenen technischen Befunde im WP038-Scope. Die separate schmale Abschluss-Dokumentationsprüfung und persistente WP-Abschlussfortschreibung folgen; keine Mainfreigabe.

Prüfrolle `/root/wp037_independent_qc`, tatsächlich getrennt von Implementierung, CI-Sammlung und zuständiger Unterintegrationsentscheidung. Dokumentiert am 2026-10-19T01:26:22.574507+00:00. Ausschließlich eigene QC-Dokumente geschrieben; kein Source-/Git-/CI-/Editor-Eingriff.

## Tatsächlicher Prüfgegenstand und Tree

PR #33 wurde nach gesondertem `WP038-SUBINTEGRATION-01` am 19.10.2026 um 00:06:16 UTC in PR #31 unterintegriert. MI `07ba4d3130414ee82e843823c2f04aeac230f25b` hat geordnete Parents `[3772c0370b39ec95979fb9efdd47f4344c41a539, 524b45024438b93c592351d7683a7c62aec2ab8d]`. Tree `f1adfa4e006169c32bd930cc6c76f1d62b98e780` entspricht exakt der unabhängig abgenommenen korrigierten Lieferung C2. C2→MI ist vollständig leer; H→MI ist bytegleich H→C2. Eigene frische Git-/API-Abfragen bestätigen dies sowie PR31 offen/Draft und Main `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Die technische Lieferabnahme bleibt separat in [INDEPENDENT_CORRECTED_QC.md](INDEPENDENT_CORRECTED_QC.md), JSON-SHA256 `b487c3be2a1d2e57ddca70841229ad35a849fff87b369a391d69971f56fe88e7`.

## Separate Scopeketten und Schutz

- Eigener sauberer kanonischer LF-Scratch am exakten MI: Architecture17 und WP038-Scope18 einschließlich vorhandener Selbst-/Negativtests exit 0. Eigener Architectureoutput SHA256 `820a3f7829f82c7390c108717f0a8bb395d2152f3dbf8f1ab70d7fc871b65ba9`; Scopeoutput `eaa84c6dca6f82f946ebf5a5c8e5973f38b9e78749e971f30ee609e7ea18c6b2`.
- Eigenes WP038-Manifest SHA256 `cf9087d50941eb64dea3dc1ea894dd29a3a5a2a7d611668e3228cc6b875b979a`, unveränderte Basis H; 113 erlaubte Delta-Pfade. Alle 17 alten Manifestbytes erhalten.
- Historische Scopeketten getrennt: WP036 Main→`ff1836e293a2a5cff69773525c347b55deb67dfb`; WP037 ff→`b9b59647cf73e2dcfa05a413259ca27c18a5afdf`. Die eigenen kanonischen Scope18-/Selbsttestoriginale sind auf genau dieselben unveränderten Gitobjekte, Basen und Manifeste gebunden und erneut aus dem persistenten Originalpaket verifiziert. Ihre Outputs SHA256 `516837477667a6a975420e78a7a11f2d4ec10fe0285cebe8e3f3ae6b47f759dc` beziehungsweise `92b6f5ac6322197924aa1162a62e390d59adfbee5e24fae0c7f69372fa9ca039` stehen vollständig im eigenen JSON. Kein sachlich falscher kumulativer Altmanifestlauf über WP038.
- Content einschließlich angenommener Textbibliothek, Packages, ProjectSettings und Puzzle-Solver-/Testtrees seit H bytegleich. Alle 41 nativen Source-/Asset-/Metabindungen und sechs tatsächlich angesehenen PNGs stimmen mit C2/MI überein. Echter Nichtbatch-D3D11-Grafiklauf 9/9 und korrigierte C2-AK01–10-Abnahme werden deshalb unverändert wiederverwendet. Historische NATIVE-01-/SCREEN-01-FAILs und Korrekturbelege bleiben bytegleich erhalten. Keine neue allgemeine Solver-/Text-/Unitygrafikrunde.

## Tatsächlich neue reguläre Integrations-CI

Eigene vollständige frische Head-SHA-Discovery enthält genau diese vier Originalereignisse, jeweils attempt1 mit API-Head MI und Tree f1ad. Sämtliche 22 tatsächlichen Jobs terminal SUCCESS; beide Unityereignisse jeweils genau alle 10 Pflichtjobs. Es gab keinen Retry, Cancel oder zusätzlichen Dispatch durch diese Prüfrolle.

| Run | Ereignis | Jobs | Tatsächlicher Zustand |
|---|---|---|---|
| 39179256058 | push | 1 | completed / success |
| 39179259658 | pull_request | 1 | completed / success |
| 39179256064 | push | 10 | completed / success |
| 39179259987 | pull_request | 10 | completed / success |

Die beiden Architecture-Originallogs enthalten tatsächlich Architecture17 und Scope18 am MI. Eigene frische vollständige Connectorlogs für beide Architecturejobs und alle 8 lizenzierten Jobs sind bytegleich mit den gesammelten Originalen. Sämtliche 22 Original-Loghashes und vollständig deklariert bereinigten persistenten Profile wurden eigenständig reproduziert; 581 Einzelbindungen/Originalprüfungen bestanden. Das Autorenpaket erteilt selbst keine unabhängige Freigabe.

Push checkt tatsächlich MI aus. PR31 checkt synthetisch `9443dc0ec6d1a375a745369befcb3eb6d9b94c69` aus. Dieses Objekt wurde selbst frisch per Git-API geprüft: identischer Tree f1ad, Parents `[Main d90e798…, MI07ba4d3…]`. Architecture-PR wechselt für den Scope danach explizit auf MI. Jede tatsächliche Checkoutfolge wurde gelesen; keine ungeprüfte Aliasannahme.

| Run | Neue Original-XML | Tatsächliche Fälle | Original-SHA256 |
|---|---|---|---|
| 39179256064 | playmode.xml | 49 Passed | `8133981fefc0eda22d375353ceeeb92f6929aa60d918fad72b607287b76a4f37` |
| 39179256064 | editmode.xml | 474 Passed | `b9bd2afdc38d29d0480e957e3665d464e0fe3ee987b2a78a05e8b75d99c1cb10` |
| 39179259987 | editmode.xml | 474 Passed | `d3ce9417d095482b9664e4a46612a681267f390895755ad49ca522f1c92594b5` |
| 39179259987 | playmode.xml | 49 Passed | `c5a20c6192437c0a0730e7d4cf5265a45cba9d070cb742fa9819faa2c5ee7cd5` |

Je Ereignis 474 EditMode +49 PlayMode Passed, null Failed/Skipped/Inconclusive. Alle neuen Application 19-, Presentation 13- und Bootstrap 9-Fälle einschließlich NormalScreen-Semantik sowie alte verpflichtende Composition-Smoke tatsächlich separat je Ereignis ausgeführt. Originale Coverage-Total-/Messsitzungen enthalten je 18 STP-Assemblies einschließlich aller 14 regulär erforderlichen Module; keine erfundene Mindestquote. Nullrenderer-Grafikfälle bezeichnen Bildausführung ausdrücklich NOT_EXECUTED und ersetzen den echten nativen Grafiknachweis nicht.

| Run / Original-ZIP | Artefakt | Bytes | SHA256 = frischer GitHub-Digest |
|---|---|---|---|
| 39179256064 / 12156920934 | android-development-il2cpp | 45584967 | `01bee9ebc886b1ab39bf72229c56e087b3e71ce24b2c71cbafabed13f03485f1` |
| 39179256064 / 12155733748 | playmode-results | 432512 | `78d8f934371798f2bc79a9e051d07f50738fd05ed8762241a102696ade87068c` |
| 39179256064 / 12155214594 | editmode-results | 498135 | `a0e63820a28401a464cf10d24cb358afd785af81a50403ae7359b27207762bef` |
| 39179259987 / 12156948095 | android-development-il2cpp | 45584989 | `7f0ca145cebda6fb0af55c2c35b761f63efdd0c5c5b3ef290812b23764254682` |
| 39179259987 / 12156041597 | editmode-results | 498191 | `3ed8fffe6ad8c1d1d21e204046497cf1c9269501ab055cb326c40f58628e0e59` |
| 39179259987 / 12156017712 | playmode-results | 432480 | `a93e0372901404dc31e6f0fc614c1964f29726a761d15e630d952d4b94f3f5e5` |

Für beide Androidjobs wurden echte neue APKs einschließlich Manifest/ARM64-IL2CPP entpackt und gehasht. Für beide Android- und beide iOSjobs enthalten die vollständigen Originallogs tatsächlich beide QA-BuildReport-Szenen, Unity Build PASS und echte Deactivation/200REVOKE_SEAT. Beide iOSjobs enthalten zusätzlich tatsächliches unsigned Xcode `BUILD SUCCEEDED` mit `CODE_SIGNING_ALLOWED=NO`. Der zum ProcessScene-Zeitpunkt nullwertige BuildGuid wird als Originalbefund erhalten und nicht als eindeutige Buildidentität benutzt; Run/Job/Checkout/Plattform, tatsächlich verarbeitete Szenen und tatsächliches Buildergebnis bilden die Bindung. BuildReport-Gesamtgröße ist keine physische APK-Dateigröße.

Alle 41 persistenten Payloads, Original-XMLs, ZIP-/Memberdigests und vollständigen deklariert bereinigten Logs wurden aus den neuen Originalen verifiziert. Paket [INTEGRATION_CI_EVIDENCE.json](INTEGRATION_CI_EVIDENCE.json), 7627239 Bytes, SHA256 `474abb52d0392acce9d3d8194a716d8bec4c70f8fd4544a5b70440179fc153a5`. Eigene überprüfbare Ergebnisse und Originalbindungen stehen in [INTEGRATION_QC.json](INTEGRATION_QC.json).

## Grenze der Abnahme

Der technische Integrationsnachweis ist bestanden. Für WP-Abschluss/AK11 bleibt die anschließende konkrete Dokumentationsfortschreibung und ihre schmale unabhängige Deltaprüfung; dafür ist bei unveränderten Source-/Vertrags-/Artefaktbytes keine neue technische Vollprüfung erforderlich. Mainintegration, Produktkatalogpromotion, persistenter Save-/Rewardpfad, Geräte-/Signierungs-/Store-/Releasefreigabe und menschlicher Kampagnentest sind nicht erteilt. Der nächste Produktionsblock richtet sich nach den gesondert verankerten fachlichen Grenzen.
