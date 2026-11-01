# WP-036 text-library authoring checker

`validate.py` checks the new German authoring source against its own closed
Draft 2020-12 schema and the pinned, read-only WP-026 Git objects. It does not
import content, modify foreign sources, run a solver or approve a release.
Use the Python environment installed from
`tools/architecture-validation/requirements.lock.txt`; no new package is needed.

```sh
python tools/text-library/validate.py --self-test --write-reports
python tools/text-library/render.py
python tools/text-library/render.py --check
```

The default inputs are `Content/Localization/source/de-DE.json` and
`Content/Reviews/text-library/WP-036/source-bindings.json`. `--source` and
`--bindings` can select temporary negative-test inputs. `--report-dir` selects a
directory inside `Content/Reviews/text-library/WP-036/`; the checker rejects
report writes outside that scope. Without either report option it only reads.

`render.py` creates the human-readable `READING_EDITION.md` from that canonical
source. It labels all 240 deductions INTERNAL and marks blocked UI drafts.
`--check` verifies the committed edition without changing it; CI runs it after
the full source validator. This is a review projection, not a game importer.

The full source history must be present. A fresh CI checkout may need the exact
read-only fetch below before this check; `FETCH_HEAD` is not merged or checked out.

```sh
git fetch --no-tags origin dc1652befd7545b4c2a2dd0580f35a4e8e05d72d
```

The script pins that SHA and integrated base
`d90e798dd9b26d3f152f3b0c2d1d52cae641eb16` in code. It reads blobs through
`git cat-file --batch`, hashes their actual Git bytes and never copies WP-026
levels or reviews into this delivery tree. It verifies exactly 240 level IDs
in 5 × 4 × 12 order and all four actual keys per level. It also checks the
original 1,440-target inventory: 960 source texts are covered while the other
480 Completion/World references remain unresolved by this work.

## Authoring source and limits

The envelope is `{schemaVersion: 1, locale: "de-DE", status: "DRAFT", entries: []}`.
Every entry carries `key`, `text`, `description`, `charLimit`, `placeholders`,
`status`, `audience`, `availability`, `blockedBy` and `charLimitStatus`.
`shortText` is optional. Both root and entries remain `DRAFT`.
`charLimitStatus` is always `EDITORIAL_PROPOSAL`; these limits do not establish
UI, layout or accessibility decisions.

Placeholder notation is `{name}`. Names and declarations must match exactly,
including in a short version. Allowed types are `INTEGER`, `TEXT`,
`FORMATTED_DURATION`, `FORMATTED_POINTS` and `FORMATTED_PRICE`. The latter three
receive already formatted display strings; the checker does not introduce a
formatter, currency policy, timing threshold or runtime dependency.

Lengths count Unicode code points of source strings, including literal
placeholder tokens. They do not measure glyph width, grapheme clusters,
substituted values or device layout. Text and descriptions must be NFC; invalid
control characters, replacement characters and unpaired surrogates fail.

`PLAYER` and `INTERNAL` are explicit audiences. The 240 `.entry` texts must be
`INTERNAL`; they are editorial deduction information, not automatic player
hints. `BLOCKED_PRODUCT` requires named blockers; `DECIDED` requires none.
These are authoring metadata, not a new runtime implementation. All `DRAFT`
content still needs its actual subsequent approval/import gates.

## Binding evidence

`source-bindings.json` has `sourceRef`, `authorReviewPath`,
`authorReviewSha256` and an ordered `levels` array. The checker reads the exact
pinned `Content/Reviews/season-1/author-review.json` Git blob, verifies its
path/hash binding, requires exactly 240 unique IDs in campaign order, and checks
every `entryDeduction` for exact equality with its individual route-review
entry quote. The report records the author-review blob hash and the number of
actual quote agreements. Each level binding record
contains the level ID/path/SHA-256, its four actual key fields, the route review
path/SHA-256, the exact `entrySourceQuote` extracted from that level's
`- Einstieg:` line, `entryTextSha256`, `entryFact` and `publicFacts`.

`entryFact.kind` is `SOURCE_REVIEW_QUOTE`; optional `entryFact.quote` must equal
`entrySourceQuote`. The hash binds the actual editorial rewrite, which may
differ from the source quote. `publicFacts` exactly reproduces the bound level's
`grid`, `endpoints`, `rowCounts` and `columnCounts`.

The separate `mathematicalFact` verifies one elementary public-input anchor:

```json
{"kind":"LINE_COUNT","axis":"ROW","index":0,"count":4,"conclusion":"FULL"}
```

`axis` is `ROW` or `COLUMN`; `conclusion` is `FULL` or `EMPTY`. Alternatively:

```json
{"kind":"ENDPOINT_LINE","endpoint":"a","axis":"ROW","index":2,"count":1,"conclusion":"REST_EMPTY"}
```

An endpoint line also accepts `OCCUPIED_ENDPOINT`. The compact aliases
`FULL_ROW`, `ZERO_ROW`, `FULL_COLUMN`, `ZERO_COLUMN` (with `index`) and `ENDPOINT`
(with endpoint `a` or `b`) are equivalent supported authoring forms.

This proves provenance and that explicit elementary fact. It **does not** prove
the whole paraphrased sentence, derive a fresh puzzle solution or replace
individual editorial verification against the actual review and input.

## Reports and negative probes

Reports are deterministic JSON with no timestamps or machine paths:

- `technical-validation.json`: input hashes, coverage, elementary anchors,
  source-binding diagnostics, boundaries and executed mutation results.
- `existing-key-inventory.json`: tracked base JSON `*Key` references and quoted
  C#/UXML key candidates. Fixture/test-only missing keys are listed separately;
  they do not demand invented production translations. Missing production
  references fail.
- `repetition-report.json`: repeated player strings, sentences and five-word
  phrases for critical editorial review. A recurring label or deliberate motif
  is not automatically a failure.
- `pseudolocale.json`: player text with vowel accents, preserved placeholder
  tokens and 35% non-placeholder padding. This is layout-test material only;
  it is not a performed Unity, screenshot or device layout test.

`--self-test` first requires valid complete inputs, then verifies detection of
missing and duplicate keys, undeclared and invalidly typed placeholders,
overlong text, a wrong level hash, a player-visible internal entry, a false
mathematical anchor, a wrong source quote and non-NFC text. It also checks an
incorrect author-review hash and a changed individual author quote. The latter
probe updates the mutated author's hash binding so it must fail the exact
quote-agreement check, rather than merely a stale checksum. Both author probes
require their specific expected diagnostic to count as detected. These probes run in
memory; production files are never mutated.

A successful text check cannot pass the independent architecture/scope, Unity,
runtime import, full cross-reference, creative/product, legal, device or release
gates. Run the existing architecture validator unchanged with this WP's own
anchored production manifest; report genuine CI outcomes separately.
