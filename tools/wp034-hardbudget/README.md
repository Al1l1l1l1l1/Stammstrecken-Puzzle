# WP-034 reproduzieren

Diese Hilfen verwenden ausschließlich bestehende Produktion/Tests. .NET10 SDK10.0.401 und NUnit/NUnitLite3.14.0 sind eine zusätzliche lokale Diagnoseumgebung, kein Unity-CI-Ersatz. Echte Referenz: unveränderter regulärer Unity6000.3.23f1-/Coverage-/IL2CPP-/Xcode-Workflow. Kanonische Governance-/Schemaumgebung: Linux/macOS CPython3.11.13, Node22.20.0 und exakt requirements.lock.txt aus tools/architecture-validation.

Keine NuGet-Abhängigkeit im Unityprojekt hinzufügen. Bestehende NUnit3.14-DLLs können verwendet werden. In einem neuen Checkout bei Bedarf ein temporäres net10.0-Projekt ausschließlich unter ignoriertem `.utmp` mit PackageReference NUnit3.14.0 und NUnitLite3.14.0 regulär wiederherstellen; dessen DLL-Verzeichnis als explizites Argument verwenden. Alle Skripte sind vom Repositoryroot mit Python3 auszuführen; `-B` verhindert untracked Importcaches.

```text
python -B tools/wp034-hardbudget/compare.py --version old --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version current --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version old --profile --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version current --profile --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version current --mutation late-result --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version current --mutation omit-restore --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/compare.py --version current --mutation reverse-deletion --nunit <nunit.framework.dll>
python -B tools/wp034-hardbudget/suite.py --nunit-dir <NUnit-und-NUnitLite-Verzeichnis> --result=.utmp/wp034/all-pure.xml --out=.utmp/wp034/all-pure.txt
python -B tools/wp034-hardbudget/production.py
```

`compare.py` exportiert die alte Produktion exakt aus a2e26a6, die neue aus der Arbeitskopie; jede Variante hat eigene Assemblies und Sourcehashes. Alle ursprünglichen HardBudget-Assertionen werden ausgeführt. Der neue Lauf muss alle13SNAPSHOT-/TRACE_CONTRACT-Zeilen gegen den zuerst erzeugten alten Output exakt vergleichen. Mutationen verändern ausschließlich Scratchquellen; ein Buildfehler gilt nicht als erkanntes Fehlverhalten. Profilwerte sind inklusiv und enthalten Messoverhead, deshalb nicht addieren. Allokationszähler sind im .NET-Harness verfügbar; reguläre Unitydiagnostik kennzeichnet sie explizit UNAVAILABLE.

`suite.py` schließt ausschließlich den Unity-only Season1CorpusPerformanceTests aus der zusätzlichen reinen Suite aus. Dieser Test läuft unverändert in echter CI. `production.py` exportiert aktuelle Produktionsquellen und kanonische Content-Gitbytes nach .utmp/wp034/production, führt dort den bestehenden Authoringverify aus und prüft alle240Level-/Proofbytes sowie alle20Rootdateien. Root-Prettyprint-Rohhashes und Git-LF-Hashprofil bleiben getrennt.

Danach unter der gepinnten Linuxumgebung im Repositoryroot:

```text
python -B .utmp/wp034/production/tools/season1-authoring/check_schema_hashes.py
python tools/architecture-validation/validate.py --self-test
python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-034.production.scope.json --self-test
```

Downloads in .utmp/wp034 sichern. `evidence.py` verifiziert die zwei Original-ZIP-Digests und erhält Original-ZIPs/XMLs verlustfrei. `record_ci.py --help` nimmt Run/Artefakt/Commit/ZIP/Digest und bewahrt Edit-/Play-ZIPs/XMLs sowie ausgewertete Ergebnisse; es rechnet alle350Korpussamples, p95 und Grenzen unabhängig nach. Android-ZIP/APK-Rohbytes bleiben im Downloadarchiv, im Bericht werden alle Memberhashes erfasst. Joblogs stammen aus dem GitHub-Connector; Texttransport ist ausdrücklich getrennt vom Original-ZIP-Profil. `persist.py` sammelt eigene lokale Proben mit Rohbytehashes und verlustfreien gzip/base64-Payloads; Ausgabe enthält keinen unabhängigen PASS.

AK-/DoD-/CI-/Rollenstand steht ausschließlich in PROJECT_CONTROL/Investigations/WP-034_HANDOVER.md und den dort gebundenen tatsächlichen Nachweisen. Kein Hilfslauf importiert Content oder erteilt Produkt-/Main-/Releasefreigaben.
