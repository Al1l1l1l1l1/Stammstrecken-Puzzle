# ADR-026 – Validator-Evidenz und Scope-Vertrauensanker

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

ADR-022 trennt Architecture-only-, Documentation- und Production-Scope. Der v0.3-Validator verwendete jedoch Python-`fnmatch`, wodurch `*` auch Verzeichnistrenner matchen konnte. Globale Äquivalente wie `*`, `**/*` und `*/**` blieben möglich. Externe oder absolute Manifestpfade sowie ein im selben Diff erweiterbares Manifest konnten als Scopequelle dienen.

Zugleich fasste der Bericht strukturelle, tatsächlich ausgeführte semantische Modelle und nur manuell überprüfbare Dokumentreihenfolgen zu breit unter lokalen PASS-Aussagen zusammen.

## Entscheidung

ADR-026 ersetzt ADR-022 vollständig und restatiert dessen drei Ausführungsarten:

1. **Architecture-only:** prüft Repositoryverträge, führt aber keine Work-Package-Scope-Abnahme aus.
2. **Documentation scope:** prüft den realen Diff gegen einen vorab versionierten engen Documentation-Vertrauensanker und verbietet Produktartefakte.
3. **Production scope:** prüft den realen Diff gegen einen vorab versionierten, ausdrücklich für Produktionspfade freigegebenen Vertrauensanker.

Ein Scope-Manifest liegt ausschließlich unter `tools/architecture-validation/scopes/<WP-ID>.<scope>.scope.json`. Der CLI-Pfad muss exakt dieser kanonische repositoryrelative POSIX-Pfad sein. Absolute Pfade, `..`, Symlink-Escape, externe Dateien, nicht versionierte Dateien und abweichende WP-/Scope-Zuordnung sind ungültig.

Das Manifest wird **vor** den fachlichen Änderungen zusammen mit dem zugehörigen Work Package in einem eigenen Ankercommit eingeführt. Sein `baseCommit` ist exakt der Elterncommit dieses Ankers. Der Validator ermittelt den ersten Git-Commit, der das Manifest anlegt, und liest den autoritativen Manifestblob aus diesem Commit. Der aktuelle Blob muss bytegleich sein. Das Work Package aus demselben Ankercommit muss den kanonischen Manifestpfad ausdrücklich referenzieren. Damit kann der geprüfte Abschlussdiff seine eigene Allowlist oder Basis nicht erweitern.

Pfadmuster verwenden einen geschlossenen segmentbasierten POSIX-Globvertrag:

- Das erste Segment ist ein literal benannter Repositoryroot ohne Wildcard.
- `*` matcht nur innerhalb genau eines Segments und niemals `/`.
- `**` ist nur als vollständiges Segment zulässig und matcht null oder mehr vollständige Segmente.
- `*`, `**`, `**/*`, `*/**` und jedes global oder global-äquivalent matchende Muster sind verboten.
- Beide Endpunkte von Rename und Copy müssen erlaubt sein.

Der Validatorbericht trennt sieben Kategorien:

| Kategorie | Aussage |
|---|---|
| `LOCAL_DOCUMENT_STRUCTURE` | Ausgeführte Inventar-, Link-, ADR-, Status- und Strukturprüfung. |
| `LOCAL_ARCHITECTURE_SEMANTICS` | Ausgeführte strukturierte Reducer-, Schema-, Cross-Reference-, Hash-, Golden- und Mutationsprüfung. |
| `MANUAL_ARCHITECTURE_REVIEW` | Verbindlicher Dokumentreview, dessen Semantik nicht maschinenstrukturiert gebunden ist. Kein PASS-Label. |
| `LOCAL_SCOPE` | Ausgeführte Git-Diffprüfung gegen den unveränderten Ankerblob. |
| `CONTRACT_ONLY` | Vertrag oder Fixture vorhanden; kein Produktionscodebeleg. |
| `REQUIRED_LATER/NOT_EXECUTED` | Verpflichtender, hier nicht ausführbarer Produktions-, Geräte-, SDK- oder Storetest. |
| `BLOCKED` | Offene Produktentscheidung oder externe Freigabe. |

Eine automatische PASS-Gruppe darf nur Semantik nennen, die der aufgerufene Code tatsächlich ausführt. Die normative IAP-Dokumentreihenfolge bleibt `MANUAL_ARCHITECTURE_REVIEW`; die strukturierte IAP-Fixture darf separat als begrenzte semantische Fixtureprüfung PASS melden.

## Begründung

Ein Manifest ist nur dann ein Vertrauensanker, wenn es vor dem zu prüfenden Diff unveränderlich feststeht. Segmentglob-Semantik verhindert unbeabsichtigte Rekursion. Getrennte Evidenzkategorien machen sichtbar, ob eine Behauptung strukturell, semantisch, manuell oder erst später geprüft wird.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Manifest aus aktuellem Working Tree vertrauen | Kann im selben Diff seine eigene Allowlist erweitern. |
| Externes signiertes Manifest | Zusätzliche Schlüssel-/Dienstinfrastruktur ist für dieses Repository unnötig. |
| Python-`fnmatch` dokumentieren | Seine Slash-Semantik entspricht nicht dem verlangten POSIX-Segmentvertrag. |
| Alles unter semantischem PASS ausgeben | Überdehnt die tatsächliche Abdeckung. |

## Konsequenzen

Jedes neue Work Package benötigt zuerst einen Ankercommit mit WP und Scope-Manifest. Historische WP-003-Scopeaussagen bleiben historische Belege; sie werden nicht rückwirkend als nach ADR-026 verankert ausgegeben. WP-004 verwendet erstmals den neuen Vertrauensanker vollständig.

Der Architekturvalidator kann weiterhin in einer repositoryexternen, gepinnten Pythonumgebung offline laufen. Ein später autorisierter GitHub-Actions-Workflow verwendet denselben Befehl, ist aber nicht Bestandteil dieses ADRs.

## Betroffene Artefakte

Scope-Schema und -Manifeste, Architekturvalidator und README, Teststrategie, ADR-Index und alle künftigen Work Packages.

## Validierung

Negativtests prüfen globale und global-äquivalente Muster, Slash-Semantik, Rename-/Copy-Endpunkte, absolute/externe/unversionierte/falsch benannte/Symlink-Manifeste, falsche WP-Bindung, nachträgliche Manifestmutation und Scope-Escape. Berichtstests stellen sicher, dass manuelle sowie spätere Production-/Devicegates nie als lokale PASS-Zeilen erscheinen.

## Ersetzt / ersetzt durch

Ersetzt [ADR-022](./ADR-022-validator-scope-und-belegkategorien.md) vollständig. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../tools/architecture-validation/README.md "Architecture Validation Tool"
[2]: ../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.4"
[3]: ../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md "WP-004"
