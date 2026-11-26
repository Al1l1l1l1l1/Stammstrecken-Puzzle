# Stammstrecken-Puzzle – Arbeitswarteschlange

**Grundlage:** Architecture v1.0 · Produktquellen in [`PRODUKT/`](../PRODUKT/SATIRE_UND_LORE_BIBEL.md).

| Reihenfolge | Arbeitspaket | Branch | Status |
|---:|---|---|---|
| 1a | [`WP-047` – 3D-Bahnwelt und alle Bildschirme](../WORK_PACKAGES/WP-047_3D-Bahnwelt-und-alle-Bildschirme.md) | `rework/wp-047-welt` | Offen · parallel zu 1b |
| 1b | [`WP-048` – Satire-Inhalte, Stimmen, Klang, Einstieg](../WORK_PACKAGES/WP-048_Satire-Inhalte-Stimmen-Klang-und-Einstieg.md) | `rework/wp-048-satire` | Offen · parallel zu 1a, wird zuerst gemergt |
| 2 | [`WP-049` – Abschluss, Sternzeiten, Gerät, Aufräumen, Merge nach main](../WORK_PACKAGES/WP-049_Abschluss-Sternzeiten-Geraet-und-Aufraeumen.md) | `rework/wp-049-abschluss` | Wartet auf 1a und 1b |

Jedes Paket: ein Agent, ein Goal-Mode-Lauf, eigener Branch von `rework/manus-rundum-rework`, selbst zurückmergen. Kein Koordinator, keine Review-Schleifen. Abgelöst: [`WP-046`](../WORK_PACKAGES/WP-046_Vollstaendiges-Spiel-und-Release-Candidate.md).

## Historische Kontrollanker

`WP-001` bis `WP-005` und die Folgebasis sind abgeschlossen; dies ist historische Validatornavigation, keine aktuelle Arbeit.

| Historischer Auftrag | Status |
|---|---|
| CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)** |

[Vollständige unveränderte historische Warteschlange](./HISTORY/PRE_COURSE_CORRECTION_2026-10-20/README.md).
