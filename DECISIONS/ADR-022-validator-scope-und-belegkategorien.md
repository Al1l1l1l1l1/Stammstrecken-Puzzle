# ADR-022 – Validator-Scope und Belegkategorien

## Status

**Ersetzt**

## Datum

2026-09-12

## Kontext

Der v0.2-Validator koppelte alle Architekturprüfungen an eine globale Documentation-only-Suffixsperre gegen `origin/main`. Dadurch wäre ein später autorisierter Produktions-Scaffold grundsätzlich rot; zugleich bewies die breite Verzeichnisallowlist nicht den Scope eines einzelnen Work Packages. Ein lokaler PASS unterschied außerdem nicht zwischen Dokumentsemantik, noch fehlendem Produktionscode und späteren Gerätetests.

## Entscheidung

Ein Validatorcode besitzt drei explizite Ausführungsarten:

1. **Architecture-only:** `validate.py --self-test`. Prüft Inventar, ADRs, Links, Schemas, strukturierte Semantik, Blocker und Negativmutationen. Scope wird nicht bewertet; das Ergebnis ist kein Work-Package-Abnahmenachweis.
2. **Documentation scope:** zusätzlich `--scope documentation --scope-manifest <relative-file>`. Prüft gegen den im Manifest gebundenen unveränderlichen Pre-WP-Commit eine positive Pfadallowlist und verbietet Produktquellen sowie Produktionsartefakte.
3. **Production scope:** zusätzlich `--scope production --scope-manifest <relative-file>`. Erlaubt Produktionsartefakte nur innerhalb der vom späteren Work Package explizit benannten Muster; es gibt keine globale `.cs`-/`.asmdef`-Sperre.

Ein Scope-Manifest ist schema-validiert, repositoryrelativ und nennt `workPackageId`, vollen Basiscommit, Scopeart sowie enge zulässige Pfadmuster. Absolute Pfade, `..`, rootweite Wildcards, fehlende Argumente, falsche WP-ID, nicht existierende oder nicht ancestor-basierte Commits und nicht autorisierte Rename-Endpunkte scheitern fail-closed.

Alle Scope-Modi bewahren `main == origin/main`, `git diff --check`, Untracked-Erfassung, Product-Source-Schutz und Secretmusterprüfung. Die drei Abschnitte aus `OPEN_BLOCKERS.md` werden kanonisch mit der Basis verglichen; sie bleiben offen und fail-closed.

Der Report trennt:

- `LOCAL_DOCUMENT_STRUCTURE`
- `LOCAL_ARCHITECTURE_SEMANTICS`
- `LOCAL_SCOPE` nur bei einem Scope-Lauf
- `CONTRACT_ONLY`
- `REQUIRED_LATER/NOT_EXECUTED`
- `BLOCKED`

Nur lokale Kategorien dürfen in WP-003 `PASS` sein. Vertragsfixtures bleiben `CONTRACT_ONLY`; spätere Produktions-/Geräte-/Storegates erscheinen als `REQUIRED_LATER/NOT_EXECUTED`; Produktblocker als `BLOCKED`. Keine dieser Kategorien wird als PASS oder N/A kaschiert. Ein Scope-Lauf nennt Commit, Worktreezustand, Scope, WP und Basis.

## Begründung

Scope ist eine zweite Abnahmeschicht, keine Architekturregel. Positive WP-Allowlisten erlauben später echten Code, ohne die aktuelle Documentation-only-Sperre still zu entfernen. Getrennte Belegkategorien verhindern überzogene Freigabeaussagen.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Globale Produktionssuffixsperre | Blockiert legitimen Scaffold. |
| Breite Verzeichnisallowlist | Beweist keinen WP-spezifischen Scope. |
| Beliebige `--allow-path`-CLI-Option | Nicht reviewbar und leicht zu umgehen. |
| Architektur-PASS als Release-PASS | Vermischt lokale Vertragsprüfung mit Compile-, Store- und Gerätetest. |

## Konsequenzen

WP-003 besitzt ein eingechecktes Documentation-Scope-Manifest mit Basiscommit `6152eead04494386404241967da0d8a62e741718`. Künftige Produktions-Work-Packages müssen ihre tatsächlichen Pfade erst im eigenen Manifest festlegen. Der GitHub-Actions-Workflow führt den Architecture-only-Lauf auf jedem PR aus; eine scoped WP-Abnahme bleibt ein expliziter zusätzlicher Befehl.

## Betroffene Artefakte

Architekturvalidator, Scope-Schema und -Manifest, Validator-README, Teststrategie, Buildvertrag, GitHub-Actions-Workflow und Work Packages.

## Validierung

Selbsttests prüfen fehlende/ungültige Scopeargumente, nicht autorisierte Markdown- und Codepfade, geschützte Produktquellen, Renames, Blockermutation sowie ein synthetisches Produktionsmanifest, das einen explizit erlaubten Codepfad akzeptiert und denselben Suffix außerhalb der Allowlist ablehnt.

## Ersetzt / ersetzt durch

Ersetzte [ADR-017](./ADR-017-versionierter-architekturvalidator.md). Wird vollständig durch [ADR-026](./ADR-026-validator-evidenz-und-scope-vertrauensanker.md) ersetzt.

## Referenzen

[1]: ../tools/architecture-validation/README.md "Architecture Validation Tool"
[2]: ../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.3"
[3]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
