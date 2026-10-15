# WP-034 – Integration in den WP-026-Zweig

Stand: 15.10.2026. Dieses Dokument hält die tatsächlich ausgeführte Integration der geprüften WP-034-Lieferung (PR #29) in `codex/wp-026-season1-logical-content` (PR #21) fest. Es ersetzt weder die WP-034-Übergabe noch einen der unabhängigen Originalberichte. Es ist keine WP-026-Gesamtabnahme.

## Ergebnis

WP-034 ist tatsächlich in den WP-026-Zweig integriert.

| Feld | Wert |
|---|---|
| Integrationscommit | `085357d59a62dae73189ea76051c73f0ffb78ae3`, Merge-Commit, am 15.10.2026 um 12:58:08 UTC von GitHub erzeugt, Signatur als verifiziert gemeldet |
| Tree | `58ee055410afd3c5e6a3acba9c47ff4f42050cf8`, identisch zum Tree des Liefer-HEAD |
| Parents | `a2e26a658a2bcdd47002698c150cb5736d9760da` (bisheriger Zielstand), `7eed2483e5bdc9289e0c94f577a68148a7b8f46a` (WP-034-Liefer-HEAD) |
| Zielbranch | `codex/wp-026-season1-logical-content`, Head nach der Integration `085357d` |
| PR #29 | geschlossen als gemerged, `merge_commit_sha` = `085357d` |
| PR #21 | offen, weiterhin Draft, Basis `main`, Head `085357d`, `mergeable_state` = `clean` |
| `main` | unverändert `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |

Integrationsweg wie bei WP-028 (PR #25 nach WP-026): GitHub-Merge von PR #29 mit `merge_method=merge` und erwartetem Head `sha=7eed2483e5bdc9289e0c94f577a68148a7b8f46a`. Wäre der PR-Head zwischenzeitlich verändert worden, hätte der Merge abgelehnt. PR #29 war Draft und wurde dafür über die zulässige Route auf „bereit“ gesetzt. Der Zielbranch wurde unmittelbar vor dem Merge noch einmal abgefragt und stand unverändert auf `a2e26a6`. Die vollständige Commit- und Ankerhistorie bleibt erhalten, weil `a2e26a6`, `f59d28b`, `840b929`, `f34d6f4` und `7eed248` Vorfahren des Integrationscommits sind.

## Vor der Integration geprüft

| Prüfung | Ergebnis |
|---|---|
| Remote-Refs | WP-026-Zweig `a2e26a6`, WP-034-Zweig `7eed248`, `main` `d90e798`. PR #29 Draft, mergebar. Keine weitere Arbeit am WP-026-Zweig. Offene PRs sonst nur #21 und der Dokumentations-PR #28 (Head `a2e26a6`, Basis ist der WP-032-Zweig). |
| Verlauf | WP-034 liegt linear auf dem Zielstand: Merge-Base ist `a2e26a6`, vier Commits `f59d28b`, `840b929`, `f34d6f4`, `7eed248`. |
| Änderungsumfang | 30 Pfade zwischen `a2e26a6` und `7eed248`, alle in der Allowlist des WP-034-Manifests. `git diff --check` bestanden. Zwischen Korrekturfreeze `f34d6f4` und `7eed248` ist nichts unter Assets, Packages, ProjectSettings, `.github`, Architektur, ADRs, Content und Konzepten geändert. |
| Trust Anchor | `f59d28b` hat genau einen Parent `a2e26a6`, fügt ausschließlich WP-034 und sein Manifest als neu hinzu. Manifest-`baseCommit` ist `a2e26a6`. Manifest-SHA-256 `417c621ddfeea9a60963e8f5591720be8cd56af93515b18a66dc50b9c8deaa11` am Anker, am Liefer-HEAD und am Integrationscommit byteidentisch. Alle anderen Scope-Manifeste unverändert, neu ist nur das WP-034-Manifest. |
| Unabhängige Nachweise | Originalbericht auf `f34d6f4` (PASS, keine offenen technischen Befunde), gezielte Lieferprüfung auf `7eed248` (PASS) und unabhängiger Liefer-CI-Schlussnachtrag (PASS), beide im PR-#29-Text unverändert erhalten. |
| Byteprofil der Originale | Die genannten SHA-256 von `WP-034_QC.md` (`f17c2cc3…`) und `WP-034_QC_EVIDENCE.json` (`59042956…`) gelten für die ursprünglichen CRLF-Bytes. Der Git-Blob ist LF-normalisiert (`787ad773…` bzw. `c481fd6a…`). Selbst nachgerechnet: LF→CRLF ergibt exakt die genannten Rohhashes. Kein Inhaltsunterschied. |
| CI am Quellfreeze `f34d6f4` | Architecture 38725866779 und Unity 38725866769: SUCCESS (per API abgefragt). |
| CI am Liefer-HEAD `7eed248` | Architecture 38731907527 und Unity 38731907618: SUCCESS, alle zehn Unity-Jobs erfolgreich. |

## Am Integrationsstand geprüft

Alle Läufe unten laufen auf `085357d`. Ihre Ergebnisse sind neue Nachweise und keine Übertragung der früheren PASS-Berichte.

| Prüfung | Ergebnis |
|---|---|
| Tree und Diff | Compare Liefer-HEAD…Integrationscommit: 0 Dateien. Der PR-Merge-Ref von PR #21 (`7fda697…`, Parents `d90e798` und `085357d`) hat denselben Tree `58ee0554…`. |
| Architecture Validation | Push-Lauf 38738775296 (Job 116434762457) und PR-Lauf 38738782949 (Job 116434788633): SUCCESS. Im Push-Log gelesen: 17 Architekturgruppen und 18 Scope-Gruppen PASS, `workPackage=WP-034 base=a2e26a6 head=085357d worktreeDirty=false`. Das PR-Lauf-Log wurde nicht einzeln gelesen. |
| Unity CI Push | Lauf 38738775650: alle zehn Jobs SUCCESS. EditMode (Job 116434919592): 456/456 bestanden, 0 fehlgeschlagen, 0 übersprungen, aus dem heruntergeladenen XML selbst ausgewertet; Artefakt 11975775808, ZIP-SHA-256 `b58cfe94085619b654bf95bce51dc869541bb50ebb261904b7b8e0af4aafac5a` stimmt mit dem GitHub-Digest überein. PlayMode (Job 116438053961): 27/27 bestanden, Artefakt 11975447748, ZIP-SHA-256 `19d471a5b885340743cb5b6e4862b34e10ef83b8a9873bced5ecfa1eb8bf45ca`, ebenfalls digestgleich. Android-Development-IL2CPP (Job 116442436023) und iOS-Export (Job 116447642401): SUCCESS laut Jobstatus. |
| Unity CI Pull Request | Lauf 38738782830: alle zehn Jobs SUCCESS. EditMode 456/456 (Artefakt 11975427070, Digest `03dbeeff…` stimmt), PlayMode 27/27 (Artefakt 11975068322, Digest `961fd113…` stimmt), Android und iOS SUCCESS laut Jobstatus. |
| Harte Budgetgrenze | Der ursprüngliche HardBudget-Test `…NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` bestand in 45,958366 s (Push) und 46,918725 s (PR) bei unveränderter 180-s-Grenze. Der Season-1-Korpus-Leistungstest bestand in 12,722593 s bzw. 13,778639 s. |
| Check-Runs am Commit | 22 Check-Runs auf `085357d`, alle SUCCESS. Der frühere rote Check an `a2e26a6` ist der unveränderte historische Original-Timeoutlauf 38668540223 und bleibt FAIL. |

Grenzen dieser Prüfung: Die Android- und iOS-Jobs sind über den Jobstatus bestätigt. Deren vollständige Logs und das APK wurden an diesem Stand nicht neu ausgewertet. Assets, Packages, ProjectSettings und Workflows sind byteidentisch zum Liefer-HEAD `7eed248`, dessen Plattformjobs vollständig unabhängig geprüft wurden. Das Integrationsergebnis wurde über die GitHub-API (Commit-, Tree-, Compare- und Ref-Abfragen) und die CI-Läufe verifiziert. Ein erster lokaler Verifikationsbefehl wurde von der Berechtigungsprüfung der Sitzung abgelehnt und nicht umgangen.

## Unverändert erhalten

Historische Originalevidenz, Originalberichte und alle alten Manifestbasen (WP-026 = `c9516bd`, WP-028 = `d97c39d`, WP-034 = `a2e26a6`) bleiben unverändert, ebenso der WP-034-Anker `f59d28b`. Solver-v2-, Proof- und Hashverträge, Level- und Proofbytes sowie alle Testgrenzen sind nicht angefasst. Der alte Original-Timeout 38668540223 bleibt FAIL. Die konkrete historische Host-, Scheduling- und GC-Ursache der früheren Zeitüberschreitung bleibt unbewiesen. Davon zu unterscheiden ist die unabhängig bestätigte, äquivalente Aufwandskorrektur im `RootDeductionTracer`.

## Grenzen und offene Punkte

Diese Integration ist keine WP-026-Gesamtabnahme und erlaubt keinen Merge nach `main`, keinen Runtimeimport und keine Produkt- oder Releasefreigabe. WP-026 bleibt offen. Weiterhin offen sind die in `CURRENT_STATE.md` geführten Gates: O-01 menschlicher Spieltest, O-05 1.440 fehlende Referenzziele (Cross-Reference-FAIL), O-06 Zeitklasse (UNCALIBRATED, null Sternsekunden) und O-07 kreative, Import-, Produkt-, Release-, Geräte- und Store-Gates.

## Letzter und nächster Schritt

Letzter abgeschlossener Schritt: geprüfte Unterintegration von WP-034 in WP-026 auf `085357d` mit neuer vollständiger Architecture- und Unity-CI.

Nächster vorgesehener Schritt: Entscheidung des Auftraggebers, wie die offenen WP-026-Gates O-01, O-05, O-06 und O-07 bearbeitet werden. Jedes davon braucht externe Eingaben oder einen eigenen Auftrag. Dieses Dokument leitet keinen neuen Auftrag ab.

Dieser Dokumentationscommit ändert nur Status- und Nachweisdateien. Seine eigenen CI-Läufe werden erst nach ihrem tatsächlichen Ende am jeweiligen Commit gebunden und hier nicht vorweggenommen.
