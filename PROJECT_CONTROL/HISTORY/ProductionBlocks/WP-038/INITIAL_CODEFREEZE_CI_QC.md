# WP038 – unabhängiger historischer C1-CI-Originalnachweis

**CI-Original-/Bindungsprüfung PASS; technische C1-Annahme weiterhin FAIL SCREEN-01. Keine Integration.** Getrennte Rolle `/root/wp037_independent_qc`. Gegenstand `e77c5fa168b915979c3c9e39d45fbcfcaefa3aea`, Tree `e5f9235716d15b1ab0a74b46fac1e1cdd1aeddd0`. [Eigener maschinenlesbarer Bericht](INITIAL_CODEFREEZE_CI_QC.json), SHA256 `d531f85fa621752df462007394cd72701c48aaf2bd89a6840caf1b97e1948122`. Erste [C1-Abschluss-QC](INDEPENDENT_FINAL_QC.md) und [SCREEN-01-Befund](SCREEN_OUTPUT_QC.md) bleiben bytegleich erhalten.

## Tatsächlicher regulärer Lauf

Architecture39171716266 und Unity39171716314 sind selbst frisch completed/SUCCESS gelesen, erster regulärer Pushversuch auf C1. Architecture genau1Job, Unity genau alle10 bestehenden Jobs SUCCESS; keine PR-Läufe am gegen WP036 gerichteten Child-PR erfunden. Alle tatsächlichen Quellcheckouts exakt C1, dessen Tree zuvor selbst aus Git und Remote gebunden wurde. Architecture17/Production-Scope18 einschließlich Self-/Negativtests tatsächlich PASS, eigenes unveränderliches WP038-Manifest ausgewählt.

Original-XMLs selbst gelesen: **474/474 EditMode** (neue19 Applicationfälle), **48/48 PlayMode** (neue13 Presentation-/8 Bootstrapfälle und sämtliche bisherigen27 Compositionfälle einschließlich verbindlichem QaScene-Smoke). Je0 Failed/Skipped/Inconclusive. Originale OpenCover-Gesamt-/Messdateien enthalten je18 tatsächlich gemessene STP-Module, alle workflowverlangten Assemblies vorhanden; keine erfundene Mindest-Coverage. Die drei gefrorenen Testsourcepayloads zusätzlich selbst gegen tatsächliche C1-Gitblobs geprüft, unabhängig von inzwischen geänderten Korrekturquellen.

## Originale und Plattformen

Alle elf vollständigen decodierten Originallogs, drei GitHub-digestgleichen Original-ZIPs,22 persistente Payloads und292 ursprüngliche Einzelbindungen selbst geprüft. Architecture und alle vier lizenzierten Joblogs zusätzlich selbst frisch per Connector geladen und exakt mit den gesammelten Rohbytes verglichen. Sämtliche gzip-/Original-/Redaktionsprofile vollständig reproduziert; Originaltest-ZIPs und XMLs bytegleich im dauerhaften [C1-Evidenzpaket](CODEFREEZE_CI_EVIDENCE.json),3759553Bytes, SHA256 `d92ec6f7dec10680ee8613702f5c364287d2bab572b66e3207fc5d0ef09342e5`.

- EditMode-ZIP12151809989: SHA256 `7c5f7e3f070c5d66a3ddf1c7d15abc11d306bcd4985b8b65c7adfc2cd55f5ded`; XML `6e909441c74c238f95aa0c98915ab53e0fd4dafb0e759be36b93d706e66cf8c2`.
- PlayMode-ZIP12153410658: SHA256 `d8efa0491b28fea15dc77098512b7071d2e2727918063e6be27478a66d3f6958`; XML `0d74e653c9491e83ecfed4981e8a423fdc1028b85be037d0c685d618e4b150ab`.
- Android-ZIP12153681049:45592029Bytes, SHA256 `3a9f771a3346e5ab5480a67d26bb1a302927b5c84bdda5e19bfc767e94656f68`; tatsächliches APK46845701Bytes, SHA256 `7427ddbc1b2b3d81a078597ca0446f502afaebe3ccd5ce31f7f5005ecb7f6daa`, Manifest und native ARM64-IL2CPP-Bibliothek vorhanden.

Beide QA-Szenen wurden im wirklichen Android- und iOS-BuildReport verarbeitet, jeweils Installer1, tatsächlicher Plattformbuild erfolgreich; iOS/Xcode zusätzlich unsigned `BUILD SUCCEEDED`23:01:04Z bei CODE_SIGNING_ALLOWED=NO. Der während OnProcessScene protokollierte BuildReport-GUID ist tatsächlich all-zero und wird ausdrücklich nicht als eindeutige Build-ID gewertet. Run/Job/echter Checkout, Plattform-/Szenenzeilen und Buildabschlüsse tragen die Bindung. Android report.summary.totalSize855105381 wird ausdrücklich von der tatsächlichen APK-Dateigröße unterschieden.

## Lizenzende und fachliche Annahmegrenze

Alle vier lizenzierten Jobs haben tatsächliche Deactivation-processed- und REVOKE_SEAT-Zeilen. Letzte Freigabe iOS23:01:07.7489860Z beziehungsweise23:01:07.7518770Z, danach gesamter C1-Lauf terminal. Das C1-Seatfreikriterium ist erfüllt; die Koordination prüft andere aktive Repo-Läufe und lokale Editorinstanzen separat unmittelbar vor neuer nativer Ausführung.

Die drei headless Grafikfälle melden ausdrücklich GRAPHICS NOT_EXECUTED/null renderer. Sie beweisen tatsächliche UI-Semantik, keinen normalen grafischen Screenoutput. Vorliegende D3D11-Panel-RT-Belege bleiben für ihre eigene Grenze gültig. **SCREEN-01 bleibt offen; AK04/AK06 am C1 FAIL, kein AK10-/Integrations-PASS.** Korrigierter tatsächlicher Screenlauf und neuer Freeze mit eigener vollständiger regulärer CI bleiben erforderlich. Keine zusätzliche allgemeine Solver-/Textprüfung oder eigene Unityausführung; keine Source-, Shared-Git-, Remote- oder CI-Mutation durch diese Prüfrolle.
