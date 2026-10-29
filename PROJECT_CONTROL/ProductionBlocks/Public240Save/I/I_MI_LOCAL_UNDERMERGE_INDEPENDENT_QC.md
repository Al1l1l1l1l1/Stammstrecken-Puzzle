# Tatsächlicher lokaler Untermerge — unabhängige Prüfung

**PASS_EXACT_LOCAL_UNDERMERGE_MI_SOURCE_TREE_AUTHORITY_PREPUSH_INTEGRATED_CI_OPEN**

MI `1af988cd73a7267796fb3b6fd77204166241a07a` hat geordnete Eltern `[B 3d1ac59c…, F2 242712d1…]` und Tree `8b4f4b2d9460979bd0a532c993933e7b5be4b93a`. Rawcommit/Tree neu gehasht; alle 1368 rekursiven Dateieinträge einschließlich Modi/Blobs exakt F2, Source-Diff leer.

Der separate mandatierte R-Entscheid WP043-SUBINTEGRATION-01 ist mit den vier echten Basisberichtshashes gebunden. Vollständiger Entscheid und Hash wurden vor der Mergemutation im tatsächlichen PR37 persistiert und unabhängig frisch gelesen. Expected-HEAD-Audit bis Abschluss:3.796s. Der sekundengenaue Git-Zeitstempel beweist keine Subsekundenreihenfolge; die Originalreceipt bindet den tatsächlichen vor-/nachgelagerten Ablauf.

Alle22 Scope-Manifeste sind unverändert gegenüber F2; WP040–043-Erstadds/Eltern/Basen/Manifestbytes sind historisch korrekt. P besitzt die geordneten LA-/LS-Eltern. Das ist keine kumulative Einmanifestfreigabe. Root auf MI sowie A/S/I auf ihren eingefrorenen Heads jeweils sauber. Frische unabhängige Server-GETs bestätigen sechs unveränderte Heads, insbesondere Development weiterhin B und Main unverändert.

Ein lokaler git-ls-remote-Transportfehler wurde bewahrt und durch erfolgreiche lesende GitHub-Ref-GETs ersetzt. Keine Mutation daraus abgeleitet.

Noch offen: normale erwartungsgebundene Veröffentlichung, tatsächliche integrierte Push-/PR31-CI und unabhängige I16-Annahme, danach separater Dokumentationsnachlauf mit eigener CI. Die erfolgreichen F2-Läufe werden nicht als integrierte Läufe ausgegeben. Keine offenen lokalen Mergebefunde.

JSON-SHA256: `8d04b17229c40358711fe042aeb1bd7eefecffb0932dafe3be13599186b71d43`.
