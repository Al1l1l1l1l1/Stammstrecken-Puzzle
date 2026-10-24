# WP-037 – gesonderter Unterintegrationsentscheid

**Entscheidung: WP-037-SUBINTEGRATION-01, 18.10.2026.** Die Koordination trifft in der vom Nutzer ausdrücklich übertragenen Geschäftsführerrolle den zuständigen gesonderten Entscheid: PR #32 darf ausschließlich in den bestehenden WP036-Zweig von PR #31 integriert werden. Grundlage ist der neue ausdrückliche Auftrag „Produktionsblöcke zusammenführen und WP-037 abschließen“, einschließlich unabhängiger Prüfung, Eigenkorrektur, Unterintegration und Integrationsnachweis in einem intern koordinierten Arbeitsblock. Dies ersetzt keine Mainfreigabe.

## Erfüllte Voraussetzungen

Die tatsächlich von Implementierung und Koordination getrennte Prüfrolle `/root/wp037_independent_qc` hat den unveränderten Lieferhead `8dd68e22b58f126cb499fee408e98c173202a98e` mit Tree `25b7907fee4e0741574c3f32867fbef03bfb1e96` unabhängig ohne offenen Befund PASS geprüft. [Vollständiger unabhängiger AK07-Bericht](WP-037_INDEPENDENT_TECHNICAL_QC.md). Beide neuen vollständigen regulären CI-Stände wurden tatsächlich geprüft: Codefreeze Architecture 39155858104 / Unity 39155858157; Lieferung Architecture 39158530698 / Unity 39158530663, jeweils SUCCESS, Attempt 1. Die Koordination hat den Rollenbericht vollständig gelesen und Lieferchecks frisch bestätigt; sie etikettiert keinen Eigencheck als unabhängige Freigabe.

Genau die zwei autorisierten Hunks (+29/−3), unveränderter Budgetpfad, 1639er-Tracebindung, Quelle SHA256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c` und kreative Annahme bleiben erhalten. Historischer ff1836e-FAIL bleibt Historie. Keine Korrektur oder Scopeerweiterung erforderlich.

## Konkrete Ausführung und Grenzen

1. Unmittelbar vor Merge PR32-HEAD exakt `8dd68e22b58f126cb499fee408e98c173202a98e`, Ziel PR31-HEAD exakt `ff1836e293a2a5cff69773525c347b55deb67dfb`, Main exakt `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` erneut prüfen. Bei Abweichung diese Ausführung stoppen; keine versteckte Neubasierung.
2. PR32 nach Readiness-Umstellung als normalen Merge mit GitHub-Expected-HEAD auf exakt die geprüfte Lieferung integrieren. Keine Squash-/Rebase-/Konfliktänderung. Der Lieferbaum muss im Merge bytegleich erhalten sein.
3. Tatsächlichen Merge-SHA/Tree/Parenten, gesamten Diff und beide unveränderten Scopeketten prüfen: Main→ff1836e ausschließlich WP036; ff1836e→Integration ausschließlich WP037. Altes WP036-Manifest nicht kumulativ über Solvercode laufen lassen.
4. Alle tatsächlich neu ausgelösten regulären Architecture-/Unity-Läufe des integrierten PR31-Heads auslaufen lassen und Original-XMLs/Artefakte/Logs prüfen. Bis dahin kein Integrations-CI-PASS. Keine Retries, Quarantäne, Abbrüche oder konkurrierenden Pushes.
5. Rollenbericht, diesen vor Merge gefällten Entscheid und anschließende Integrationsnachweise zusammen als eigenen additiven WP037-Closeout im Zielzweig persistieren. Der geprüfte PR32-Head bleibt hierfür unverändert; reiner Closeout-Dokumentationsdiff erhält schmale unabhängige Delta-/Scopeprüfung und seine gesonderte tatsächliche CI-Bindung.

PR31 bleibt Draft gegen unverändertes Main. Keine Mainintegration. Keine Annahme neuer Produkttexte/Assets, kein Runtime-/Katalogimport, keine Owner-/Produkt-/Zeit-/Rechts-/Geräte-/SDK-/Signing-/Store-/Releasefreigabe. WP026 und sämtliche Fremdgates bleiben erhalten. Die Nutzeranweisung autorisiert anschließend die neue prospektiv abgegrenzte Produktionsbündelung; sie erweitert das unveränderliche WP037-Manifest nicht.

**Stand dieser Entscheidung:** nach unabhängigem PASS und vor jeder PR-Mutation getroffen. Tatsächliche Ausführung und nachgelagerte CI werden im eigenen Integrationsbericht separat nachgewiesen.
