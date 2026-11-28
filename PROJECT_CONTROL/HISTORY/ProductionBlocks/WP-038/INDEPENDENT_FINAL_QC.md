# WP038 – unabhängige technische Abschlussprüfung

**FAIL – neuer offener High SCREEN-01 am normalen Screenpfad, AK04/AK06 nicht vollständig erfüllt. Kein AK10-/Gesamt-PASS.** Prüfrolle `/root/wp037_independent_qc` ist von Implementierung, zuständiger technischer Annahme und Integration getrennt. Keine Sourceänderung, Shared-Git-/Remote-/CI-Mutation oder eigene Unityausführung. Dieser Bericht wird erst nach wirklicher terminaler CI samt selbst gelesenen Originalen abgeschlossen.

## Exakter Gegenstand

- Codefreeze `e77c5fa168b915979c3c9e39d45fbcfcaefa3aea`, Tree `e5f9235716d15b1ab0a74b46fac1e1cdd1aeddd0`. Einziger Parent4667413, davor gemeinsamer prospektiver Anker ee14a1b, dessen einziger Parent/Manifestbasis H3772c03. PR33 frisch offen/Draft exakt auf diesem Freeze gegen unveränderten WP036-Developmenthead H; PR31 weiterhin Draft gegen main d90e798. Kein Mainmerge.
- Manifest unverändert seit gemeinsamem Erst-Add, SHA256 `cf9087d50941eb64dea3dc1ea894dd29a3a5a2a7d611668e3228cc6b875b979a`. WP-Vertrag/AK/Tests bis Bearbeitungsstatus gegenüber Anker unverändert. 86 kumulative erlaubte Pfade; eigene vollständige Scopeprüfung am sauberen eingefrorenen Gitstand tatsächlich PASS. Parallel erzeugte reine CODEFREEZE_-Evidenzdateien der Koordination wurden vom Freeze getrennt erfasst, kein Fachdelta darin angenommen.
- Alle alten Manifestblobs, Content einschließlich WP036-Quelle SHA2564458cd92…, Packages, ProjectSettings, Puzzlequellen und bisherigen Test-/Scopeketten unverändert. Alle alten asmdefs außer der genau vorgesehenen UI-Paketreferenz bytegleich, keine neue Assembly; an UI ausschließlich Unity.Localization addiert. ADR033/ADR011-/Index-/Registry-/Architekturerläuterung seit getrenntem Annahmecommit466 unverändert. Keine Scope-/Gate-/Produktpromotion.

## Eigene tatsächlich ausgeführte kanonische Gates

Gepinnte CPython3.11.13-/Node22.20.0-Toolchain, eigener sauberer LF-Prüfcheckout exakt auf dem Freeze, beide mit --self-test: Architecture17 PASS und WP038-Production-Scope18 PASS. Vollständige Originaloutputs samt argv/Head/Digest stehen im [eigenen maschinenlesbaren Nachweis](INDEPENDENT_FINAL_QC.json), aktueller SHA256 `7291c30121391a8fa4ccfb3e01b413c18f7c7448acae4f46adb0e5fb846cfcff`. ArchitectureoutputSHA256 `820a3f7829f82c7390c108717f0a8bb395d2152f3dbf8f1ab70d7fc871b65ba9`, Scopeoutput `7d8d0c39519016505d0a3b00a5780612d2b2b0685b0d5916393129d666ab7d65`. Keine erneute allgemeine Solver-/Textprüfung und kein zusätzlicher Editorlauf.

## Standgebundene Wiederverwendung

AGENTS-Startfolge und WP038-Fachquellen wurden zuvor vollständig gelesen. Getrennte Boundary-/Vorcode-/statische Lektüre und die [native GUID-Korrekturprüfung](NATIVE_CORRECTION_QC.md) werden für identische Bytes wiederverwendet. Alle41 nativen Source-/Metabindungen sind jetzt selbst aus den tatsächlichen Freeze-Gitblobs gegengeprüft; damit gelten eigene App-/UI-/World-/Lifecycle-/Localization-/Builderlektüre und echte D3D11-/PNGbelege für denselben unveränderten Gegenstand. Historischer NATIVE-01-FAIL bleibt erhalten, konkrete Korrektur geschlossen. Native8/8 und echte betrachtete Bilder werden nicht als neue reguläre CI ausgegeben.

AK01/AK02 sind im Quellen-/Scope-/Boundaryumfang bestätigt. Für AK03–AK07 liegen Source-/Nativebelege vor; vorgeschriebene vollständige reguläre App-/Presentation-/Bootstrapfälle müssen noch in den tatsächlichen Original-XMLs dieses Freeze gelesen werden. AK08 ist für GUID-/Reproduktion erfüllt, tatsächliche Android-/iOS-BuildReportbelege für neue und alte QA-Szene bleiben offen. AK09 und damit AK10 bleiben bis zu allen regulären terminalen Prüfungen offen; AK11 erfordert danach die gesonderte zuständige Unterintegration und deren integrierte Prüfung.

## Tatsächliche CI-Zuordnung – noch laufend

Regulärer Push: Architecture39171716266 und Unity39171716314, beide API-head_sha exakt Freeze. Child-PR33 richtet sich gegen den WP036-Developmentzweig; bestehender unveränderter pull_request-main-Filter erzeugt dafür keine regulären PR-Läufe. Es wird kein fehlender PR-PASS erfunden und kein zusätzlicher Workflow ausgelöst. Der vollständige reguläre Push mit zehn Unityjobs ist der hier erforderliche neue CI-Gegenstand. Parent-H-/b9-CI bleibt separat und wird nicht übertragen.

Noch ausstehend: selbst gelesene terminale Jobs, tatsächliche Checkout-/Treebindungen, originale NUnit-/Coverage-/Build-/ZIP-/Logbelege und neue Szene in Android/iOS. Implementierungssupport sammelt diese Originale, erteilt keine unabhängige Annahme. Ein konkreter FAIL wird intern korrigiert und betroffen nachgeprüft; bis dahin keine Unterintegration. Reine spätere Schlussdokumentation erhält die vertragliche schmale Delta-/Byteprüfung und ihre tatsächliche eigene CI-Zuordnung.

## Tatsächlich gelesene reguläre Testoriginale – weiterhin Teilstand

Original-EditMode-ZIP12151809989 (489853 Bytes, GitHub-SHA256 `7c5f7e3f070c5d66a3ddf1c7d15abc11d306bcd4985b8b65c7adfc2cd55f5ded`) selbst geprüft: XML411963 Bytes, SHA256 `6e909441c74c238f95aa0c98915ab53e0fd4dafb0e759be36b93d706e66cf8c2`, **474/474 Passed**, sämtliche19 neuen Applicationfälle. Original-PlayMode-ZIP12153410658 (423818 Bytes, GitHub-SHA256 `d8efa0491b28fea15dc77098512b7071d2e2727918063e6be27478a66d3f6958`) selbst geprüft: XML53803 Bytes, SHA256 `0d74e653c9491e83ecfed4981e8a423fdc1028b85be037d0c685d618e4b150ab`, **48/48 Passed**,13 neue Presentation- und8 neue Bootstrapfälle, bisherige27 Compositionfälle einschließlich verbindlicher QaScene-Compositionsprobe. BeideXMLs0 Failed/Skipped/Inconclusive; originale Gesamt-/Messcoverage je18 STP-Assemblies gelesen. Tatsächliche Seatfreigaben im Rohlog22:26:58Z und22:30:36Z.

Die drei headless Grafikfälle protokollieren ausdrücklich GRAPHICS NOT_EXECUTED/null renderer und gelten hier ausschließlich als tatsächliche UI-Semantikproben. Grafische Annahme beruht weiterhin ausschließlich auf dem getrennten selbst geprüften, exakt an C gebundenen realen D3D11-Achtfalllauf samt betrachteten PNGs. Android/iOS sowie finale vollständige Original-/Payloadprüfung bleiben offen; **weiterhin kein AK09-/AK10-Gesamt-PASS**. Zwischen-JSON nun SHA256 `8ea14142121b50abd44957aea2244e98436f99743460c95b38fe9b7a8f76d0fc`.

## Neuer unabhängiger gezielter Befund SCREEN-01

[Eigener Screenoutputbericht](SCREEN_OUTPUT_QC.md): gepinnter URP übernimmt bei World-Offscreenkamera die Screenoverlayverantwortung, die einzige Kamera kann wegen targetTexture den normalen UIToolkit-Screenpass aber nicht ausführen. Die reale bisherige Panel-RT-Probe deckt diesen Pfad nicht ab. **High offen, AK04/AK06 FAIL am C; keine Integration.** Tatsächlicher regulärer C-CI-Lauf bleibt getrennt historisch auszuwerten. Korrektur und gezielter normaler Screenlauf im bestehenden Scope erforderlich; kein allgemeiner Solver-/Textreview. Aktueller unabhängiger JSON-SHA256 `05fdc194812753d73fca028ebf1e05a15c5aab1d59e0a1b3e9c628b0fc81a975`.
