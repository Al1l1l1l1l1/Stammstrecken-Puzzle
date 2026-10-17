# WP-036 – Unabhängige technische Abschluss-QC der Copy-Nacharbeit

**Ergebnis:** **PASS – vollständige technische lokale DRAFT-/Scope-Lieferung am eingefrorenen Stand.**
**Kein Gesamtabschluss / CI:** Die neue reguläre CI dieses geänderten Freeze wurde von dieser Rolle weder gestartet noch als bestanden behauptet und bleibt **OFFEN**. Dieser PASS betrifft ausschließlich die hier tatsächlich frisch geprüfte technische DRAFT-Quelle, deren eng begrenzte Copy-Änderung und den verankerten Production-Scope. Er ist keine Owner-, kreative, Produkt-, Import-, Rechts- oder Releaseannahme.

## Prüfidentität und Unabhängigkeit

| Feld | Wert |
|---|---|
| Prüfrolle | Gesonderte unabhängige **technische** Abschluss-QC; nicht Autor, keine Übernahme der Eltern-Prüflogik |
| Eigener unveränderlicher Worktree | `/home/ubuntu/Stammstrecken-Puzzle/.utmp/wp036-independent-qc` |
| Prüf-HEAD / Freeze | `c7683ea445c65becd78cfd1e34ccb8ba587fd024` |
| Ausgangsanker | `6103d53f9a01b199db7eaf9b3e5fd0c76f4a63a1` |
| Main-Basis des WP-Manifests | `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` |
| Quelle | `Content/Localization/source/de-DE.json` |
| Source-SHA-256, Basisblob | `b45db9d6d8216de7a185b26da1edf77f5ad71b1c921ebcfbc49f9726834557be` |
| Source-SHA-256, Freezeblob und Worktree | `2cd576dc52a7e3379dfe8a925a7e8c4201c1e24e988913b247d39f1f81b605fb` |
| Kreativscope | `CREATIVE_SCOPE.json`, SHA-256 in Basisblob, Freezeblob und Worktree jeweils `255440ac5b1bf5b38d92f38273cead8ffebb4b8d00d88b88a738ea4d895f7271` |
| Laufzeit | `/home/ubuntu/.venv-stp-wp036-canonical/bin/python`: CPython `3.11.13`; Node `v22.13.0` |

Vor Arbeitsbeginn wurden die verpflichtende AGENTS-Reihenfolge (15, 00, 09, CURRENT_STATE, WP-036), Definition of Done, Handover-Regeln, CREATIVE_DECISION, CORRECTION_BRIEF, CREATIVE_SCOPE, Texttool-README, ADR-026/030, CONTENT_PIPELINE, CONTENT_CATALOGS und die erforderlichen Konzepte gelesen. Es wurden weder das kanonische Repository außerhalb dieser zwei Berichte noch Quellfreeze, Manifest, Validatoren, Workflows oder Produktionsartefakte verändert, committed oder gepusht. Eigene Audit- und Probeprogramme lagen ausschließlich unter dem ignorierten `.utmp/` des QC-Worktrees.

## Unabhängiger Blob- und Scopeaudit

Ein eigenes, read-only Auditprogramm verglich die Bytes direkt aus `git show <commit>:<path>` sowie den QC-Worktree. Es verwendet **kein Eltern-Auditskript**. Ergebnis: `INDEPENDENT_AUDIT PASS`, Exitcode `0`.

| Gegenstand | Eigenständig festgestelltes Ergebnis |
|---|---|
| Freeze-Diff `6103d53..c7683ea` | Exakt **16** geänderte/neu angelegte erlaubte Dateien, keine weitere |
| Quelle | Basis-, Freeze- und Worktreehashes wie oben; Worktreebytefolge identisch zum Freezeblob |
| Scopebindung | `CREATIVE_SCOPE.json` gegenüber dem Ausgangsanker byteidentisch; seine 110 `original_values` binden genau die Basiswerte |
| Zulässige Revisionen | 110 Scope-Records, exakt **120** erlaubte Felder: **109 `text`**, **11 `shortText`** |
| Tatsächliche Revisionen | Exakt **120** Feldänderungen in **110** vorhandenen Einträgen: **109 `text`**, **11 `shortText`**; die Menge ist exakt gleich der Scope-Menge |
| Schutz der anderen Eigenschaften | Root-Reihenfolge unverändert; 1.408 Einträge; Key-Reihenfolge und die vollständige Eigenschaftsreihenfolge/-form jedes Eintrags unverändert. Damit sind keine neuen Properties zugelassen. Alle nicht erlaubten Eigenschaften einschließlich Beschreibungen, Metadaten, `charLimit`, Platzhaltervertrag, Status, Zielgruppe, Sichtbarkeit, `availability`, `blockedBy` und Limitstatus sind wertgleich. |
| Unveränderte ganze Einträge | **1.298** |
| Geschützte Klassen | **240** Leveltitel, **240** INTERNAL-Einstiege, **308** UI-Einträge und **70** `BLOCKED_PRODUCT`-Entwürfe vorhanden, jeweils ohne Änderung |
| Gelieferter Copy-Audit | `COPY_CHANGE_AUDIT.json` wurde zusätzlich gegen beide Gitblobs nachgerechnet: 120 Records, Schlüssel/Feld, old/new, Codepointzahlen, bestehende Limits, Basis-/Freeze-/Scopehash und Zähler vollständig konsistent |
| Fremder technischer Umfang | Gegenüber `6103d53` keine Änderungen an Repositoryroot, Schema, `tools/`, Workflows, Scope-Manifest, Konzepten, Architektur, Leveln, Proofs, Assets, Katalogen, Decisions oder anderen Produktionsbereichen. Außer der Quelle liegen die 15 übrigen Freezepfade ausschließlich in WP-036-Review-/Status-/WP-Dokumentation. |

Die 16 Freezepfade sind:

```text
Content/Localization/source/de-DE.json
Content/Reviews/text-library/WP-036/COPY_CHANGE_AUDIT.json
Content/Reviews/text-library/WP-036/COPY_CHANGE_REVIEW.md
Content/Reviews/text-library/WP-036/COPY_LOCAL_ARCHITECTURE.txt
Content/Reviews/text-library/WP-036/COPY_LOCAL_RENDER_CHECK.txt
Content/Reviews/text-library/WP-036/COPY_LOCAL_SCOPE_CHECK.txt
Content/Reviews/text-library/WP-036/COPY_LOCAL_TEXT_CHECK.txt
Content/Reviews/text-library/WP-036/HANDOVER.md
Content/Reviews/text-library/WP-036/READING_EDITION.md
Content/Reviews/text-library/WP-036/README.md
Content/Reviews/text-library/WP-036/pseudolocale.json
Content/Reviews/text-library/WP-036/repetition-report.json
Content/Reviews/text-library/WP-036/technical-validation.json
PROJECT_CONTROL/CURRENT_STATE.md
PROJECT_CONTROL/WORK_QUEUE.md
WORK_PACKAGES/WP-036_Vollstaendige-Textbibliothek.md
```

## Frisch ausgeführte vorhandene Prüfungen

Alle folgenden Läufe wurden neu im eigenen, zu Beginn und nach Reportregeneration source-sauberen QC-Worktree ausgeführt; alte Logs wurden nicht als PASS übertragen.

| Befehl | Exit | Tatsächliches Ergebnis |
|---|---:|---|
| `/home/ubuntu/.venv-stp-wp036-canonical/bin/python tools/text-library/validate.py --self-test --write-reports` | 0 | `TEXT_LIBRARY PASS`: 1.408 Einträge, 960/960 Season-1-Referenzen, 240 INTERNAL, 480 ungelöste Fremdreferenzen; alle **12/12** vorhandenen Selbsttestmutationen erkannt |
| Eigenständiger Bytevergleich der vier mit `--write-reports` regenerierten Reports mit `git show HEAD:<path>` | 0 | `technical-validation.json`, `repetition-report.json`, `pseudolocale.json`, `existing-key-inventory.json` jeweils byteidentisch zum Freezeblob |
| `/home/ubuntu/.venv-stp-wp036-canonical/bin/python tools/text-library/render.py --check` | 0 | `READING_EDITION PASS: exact canonical text projection` |
| `/home/ubuntu/.venv-stp-wp036-canonical/bin/python tools/architecture-validation/validate.py --self-test` | 0 | **17** lokale Prüfgruppen PASS; Mutation-Selbsttest PASS |
| `/home/ubuntu/.venv-stp-wp036-canonical/bin/python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-036.production.scope.json --self-test` | 0 | **18** lokale Prüfgruppen PASS, einschließlich `LOCAL_SCOPE`; `SCOPE_CONTEXT`: WP-036, Mainbasis `d90e798…`, Freeze `c7683ea…`, `worktreeDirty=false` |
| `git diff --quiet -- Content/Localization/source/de-DE.json` nach den Reportwrites | 0 | Produktionsquelle source-clean; keine Reportregeneration veränderte sie |
| `git diff --check d90e798dd9b26d3f152f3b0c2d1d52cae641eb16 --` | 0 | Kein Whitespacefehler gegen Main |
| `git diff --check 6103d53f9a01b199db7eaf9b3e5fd0c76f4a63a1 --` | 0 | Kein Whitespacefehler gegen den Ausgangsanker |

Der Production-Scopelauf hielt ausdrücklich fest: lokale Scope-/Architektursemantik bestanden; `MANUAL_ARCHITECTURE_REVIEW` ist kein automatischer PASS, und Unity-/Geräte-/Storeprüfungen bleiben `REQUIRED_LATER/NOT_EXECUTED`; die drei Produktblocker bleiben `BLOCKED` und fail-closed.

## Eigene zusätzliche Negativproben

Diese Proben sind nicht die zwölf vorhandenen Selbsttestproben. Sie liefen ausschließlich auf `.utmp`-Kopien bzw. in-memory gegen die bestehenden, unveränderten Werkzeuge; die Produktionsquelle blieb danach byteidentisch. Alle **6/6** ergaben die erwartete Diagnose.

| Probe | Ziel / temporäre Mutation | Exit | Erwartete und beobachtete Diagnose |
|---|---|---:|---|
| Überlänge | Geänderter Key `level.s1.01.04.02.status.text`, 122 statt Limit 110 Codepoints | 1 | `over-limit:level.s1.01.04.02.status:text:122>110` |
| Unerlaubter Parameter | Derselbe geänderte Text mit undeclared `{probe}` | 1 | `placeholder-set:level.s1.01.04.02.status:text` |
| NFC | Derselbe geänderte Text mit dekomponiertem `ü` | 1 | `not-nfc:level.s1.01.04.02.status:text` |
| Sichtbarkeit / Status | Derselbe sichtbare Status temporär als `INTERNAL` | 1 | `binding:S1-01-04-02:audience:statusKey` |
| Scopeunzulässig | Simulierter Produktionspfad `ARCHITECTURE/CONTENT_PIPELINE.md` gegen das echte WP-036-Manifest | 0 (Diagnosefunktion) | `scope:out-of-scope:ARCHITECTURE/CONTENT_PIPELINE.md` |
| Stale-Projektion | Temporäre abweichende `READING_EDITION.md` | 1 | `READING_EDITION FAIL: missing or stale derived edition` |

Diese Experimente belegen ausschließlich die erwartete Diagnostik vorhandener Werkzeuge. Sie sind **kein** zusätzlicher Produkt-, Runtime-, Creative-, CI- oder Releasebeweis.

## AK01–AK10: technische Prüfgrenze

| AK | Technischer QC-Status | Tatsächliche Grenze |
|---|---|---|
| AK01 | PASS | 240 Leveltitel und 960/960 Referenzen sowie 240 INTERNAL/720 Level-PLAYER durch frischen Textlauf und unabhängigen Audit bestätigt. |
| AK02 | PASS im technischen Provenance-/Bindingumfang | Frischer Validator bindet 240 Level-/Author-/Review-/Inputanker. Kein erneuter Solverlauf und kein Beweis jeder sprachlichen Paraphrase. |
| AK03 | PASS bezüglich Erhaltung | Keine Kampagnen-/Routenstruktur, Titel oder nicht zugelassene Kampagnenfelder geändert; kreative Wirkung ist eine getrennte Rolle. |
| AK04 | PASS bezüglich Erhaltung | Alle 308 UI- und 70 `BLOCKED_PRODUCT`-Einträge sowie Metadaten unverändert; keine Produktregel freigegeben. |
| AK05 | PASS bezüglich Erhaltung | Variantenbestand erhalten; allein `variant.ride-ended.04.text` ist die zulässige Revision. Keine Auswahlmechanik geprüft oder geschaffen. |
| AK06 | PASS | Eintragsform, Metadaten, Limits, Platzhalter und Status blieben im Blobvergleich erhalten; Textvalidator PASS. |
| AK07 | PASS | Frischer vollständiger Validator, 12 vorhandene und 6 neue unabhängige Negativproben, Pseudolocale sowie exakte Lesefassung. Kein Runtimeimport. |
| AK08 | Nicht Gegenstand dieser technischen Rolle | Kreative Gesamtkritik und Restannahme werden nicht vorweggenommen; die kreative unabhängige Rolle ist getrennt. |
| AK09 | PASS lokal für Anker/Scope; **CI OFFEN** | ADR-030-gebundener Production-Scope und lokale Selftests bestanden. Neue reguläre Architecture-/Unity-CI wurde nicht gestartet, nicht durch Altlogs ersetzt und bleibt erforderlich. |
| AK10 | PASS für technische QC-Übergabe | Dieser Bericht und sein JSON binden Scope, Hashes, Befehle, Grenzen und offene CI an den Freeze; keine Main-Integration. |

## Verbleibende Grenzen und nächster Schritt

- **Neue reguläre CI am Freeze `c7683ea445c65becd78cfd1e34ccb8ba587fd024` ist offen.** Sie wurde hier nicht angestoßen; insbesondere kein CI-, Unity-, Solver-Neubuild oder Fremdquellenimport.
- Eine kreative Nachlektüre/Restannahme der korrigierten Fenster ist unabhängig und offen; dieser technische Bericht nimmt sie nicht vorweg.
- Alle Einträge bleiben **DRAFT**. Es gibt keine Ownerannahme, Produktfreigabe, Runtime-/Doppelimport, Cross-Reference-/Katalogfreigabe, Rechtsprüfung oder Releasefreigabe.
- `BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben fail-closed. Weitere WP-026-, Asset-, Zeit-, Layout/Accessibility-, Geräte-, SDK-, Datenschutz-, Signing- und Storegates bleiben außerhalb dieser QC offen.
- Kein Main-Merge, kein Commit und kein Push wurden durch diese Rolle vorgenommen.

**Befundbilanz:** Keine technischen Scope-, Hash-, Source-, Reportprojektion-, Validator-, Architekturselftest- oder Diffbefunde innerhalb der hier geprüften lokalen DRAFT-/Scope-Lieferung. Bei einer späteren echten CI-Abweichung ist sie getrennt, präzise gegen diesen Freeze zu dokumentieren; keine historische CI-Evidenz darf als Ersatz übertragen werden.
