# WP-037 – Unterintegration und zusammenhängende Abschlussübergabe

**Abgeschlossen: unabhängig technisch angenommen, zuständig unterintegriert und vollständig am tatsächlichen Integrationsstand geprüft. AK01–AK09 erfüllt.** Die reine nachgelagerte Schlussdokumentation erhält ihre eigene separat ausgewiesene automatische CI; kein früheres Ergebnis wird als neuer Lauf etikettiert.

## Technisches Ergebnis und getrennte Rollen

Der vollständige, unveränderte Zwei-Hunk-Port aus WP034 (+29/−3) hat die [tatsächlich unabhängige technische Abschlussprüfung AK07](WP-037_INDEPENDENT_TECHNICAL_QC.md) am Lieferhead `8dd68e22b58f126cb499fee408e98c173202a98e` ohne offenen Befund bestanden. Die Koordination hat anschließend gesondert [WP-037-SUBINTEGRATION-01](WP-037_SUBINTEGRATION_DECISION.md) entschieden und den geprüften Head mit GitHub-Expected-HEAD-Absicherung über PR32 in PR31 integriert. Die Implementierung hat sich nicht selbst unabhängig freigegeben.

| Bindung | Tatsächlicher Wert |
|---|---|
| Codefreeze | `3638dd8ba39b1f4f6867fdad03ab70afb0de79af` |
| Unabhängig geprüfte Lieferung | `8dd68e22b58f126cb499fee408e98c173202a98e` |
| Expected-HEAD bei Untermerge | exakt diese Lieferung |
| Zielhead unmittelbar vor Untermerge | `ff1836e293a2a5cff69773525c347b55deb67dfb` |
| Tatsächlicher Untermerge PR32 → PR31 | `b9b59647cf73e2dcfa05a413259ca27c18a5afdf` |
| Geordnete Mergeparenten | `ff1836e293a2a5cff69773525c347b55deb67dfb`, `8dd68e22b58f126cb499fee408e98c173202a98e` |
| Liefer- und Integrationsbaum | identisch `25b7907fee4e0741574c3f32867fbef03bfb1e96` |
| Gemeinsamer WP037-Anker / einziger Parent | `4bfd4ee98153f64a948d2358c6ce30a49a441673` / `ff1836e293a2a5cff69773525c347b55deb67dfb` |
| WP037-Manifest-SHA256 | unverändert `022590d076f1e6a972b445b8696c56f83acccfebbaf7e1f30475a388e18e44a4` |
| Main / PR31-Basis | unverändert `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |

PR32 ist merged. PR31 bleibt offen/Draft gegen Main. Keine zusätzliche Fachänderung oder Konfliktauflösung beim Merge; `git diff 8dd68e2 b9b5964` ist leer. Der vollständige kumulative Main→b9-Diff ist die nachvollziehbare Summe des historischen WP036-Textvertrags und des unabhängig geprüften WP037-Ports.

## Scopeketten und neue integrierte Prüfung

Der [unabhängige Integrationsnachweis](WP-037_INTEGRATION_QC.md) prüft die Ketten getrennt, jeweils im sauberen Stand: Main d90→ff1836e gegen das unveränderte WP036-Manifest; ff1836e→b9 gegen das unveränderte WP037-Manifest. Kein WP036-Validatorlauf legitimiert den nun solverhaltigen kumulativen Diff. Keine Manifestmutation und keine Übernahme von WP026/Korpustest/Diagnosehunks. Die Textquelle bleibt SHA256 `4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c`, die kreative Annahme WP-036-CREATIVE-02 unverändert.

Neu automatisch gestartete Läufe auf exakt b9:

| Workflow / Ereignis | Run | Stand bei Erstellung |
|---|---:|---|
| Architecture / Push | 39162496562 | terminal SUCCESS, Attempt 1 |
| Architecture / Pull Request | 39162499593 | terminal SUCCESS, Attempt 1 |
| Unity / Push | 39162496566 | terminal CANCELLED; sieben Jobs SUCCESS, PlayMode CANCELLED, Android/iOS SKIPPED; kein vollständiger PASS |
| Unity / Pull Request | 39162499589 | terminal SUCCESS, Attempt 1, sämtliche zehn Jobs erfolgreich |

Die tatsächlichen terminalen Originalbelege stehen im [unabhängigen Integrationsbericht](WP-037_INTEGRATION_QC.md) und seiner maschinenlesbaren Evidenz. Neuer PR-EditMode: 455/455 Passed, HardBudget 47.345466 s, LargeRootTrace 0.361034 s; PlayMode 27/27 samt QA-Komposition, Android Development ARM64 IL2CPP und iOS/Xcode bestanden. PR-Jobs checken den tatsächlichen synthetischen Merge 1e95eba… aus; dessen vollständiger Tree ist ebenfalls 25b7907. API-head_sha b9 und wirklicher Checkout werden unterschieden. Der separate Push liefert außerdem echte 455/455-EditMode-Belege (HardBudget 48.342306 s, Trace 0.382111 s), bleibt insgesamt aber CANCELLED. Keine Abbruchursache behauptet; durch diese Koordination keine Retries, Stornierungen, Quarantäne, gelockerten Grenzen oder konkurrierenden Pushes ausgelöst.

## Wirkung und verbleibende Grenzen

WP036 enthält jetzt die isolierte Aufwandskorrektur und die unveränderte 1639er-Tracebindung. Die Textproduktion und kreative Annahme sind fertig; es gibt keinen weiteren Copyauftrag. Der frühere echte HardBudget-Timeout bleibt historisch dokumentiert und wird durch die neue Lieferung weder gelöscht noch einer unbewiesenen Hostursache zugeordnet. Das Ergebnis ist ein technischer Fortschritt des WP036-Integrationswegs, keine Spiel-/Produktfreigabe.

Keine Mainintegration, Ownerannahme, Runtime-/Katalog-/Textimport-, Rechts-, Zeit-, Asset-, Geräte-, SDK-, Privacy-, Signing-, Store- oder Releasefreigabe. Alle Texte bleiben DRAFT, 240 Einstiege INTERNAL, 70 Entwürfe BLOCKED_PRODUCT. WP026 und die offenen 480 Karten-/Zugziele bleiben gesondert. Hint-/Daily-/Endlessblocker bleiben fail-closed.

## Abschlussführung

AK01–AK07 technisch bestanden; AK08 Schutz-/Scope-/PR-Zielbindung erhalten und zuständiger Untermerge tatsächlich ausgeführt. AK09 mit terminaler integrierter CI und eigener Status-/Schlussnachführung erfüllt. Projektweite DoD für genau diesen Port erfüllt: Implementierung, ursprüngliche und neue erforderliche Tests/Builds, unabhängige Rolle, Integration/Scopeketten und dokumentierte Grenzen; keine Architekturänderung. Unveränderte Solver-/Textprüfgegenstände werden nicht wegen dieses Dokumentationsnachtrags nochmals fachlich vollständig geprüft. Eigener finaler Closeout-HEAD/Tree und sein gesonderter regulärer CI-Nachlauf werden nach Veröffentlichung im PR gebunden; keine rekursive zusätzliche Fachfreigabe über unveränderte Quellen.

Letzter abgeschlossener Schritt: tatsächliche vollständige integrierte Prüfung. Nächster Schritt innerhalb desselben Koordinationsauftrags: reine Schlussdokumentation schmal unabhängig gegenprüfen/persistieren und ihren eigenen CI-Nachlauf ehrlich ausweisen. Danach neue prospektive Ausführung des nächsten großen lokalen Spielablaufs gemäß neuem Produktionsplan; keine Gesellschafterfrage zur Agenten-/Git-/QC-Administration. Echte spätere Owner-/Produkt-/Main-/Releaseentscheidungen werden am konkreten prüfbaren Ergebnis vorgelegt.
