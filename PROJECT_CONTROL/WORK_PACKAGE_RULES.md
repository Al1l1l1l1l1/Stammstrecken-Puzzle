# Regeln für Arbeitspakete

Ein Arbeitspaket ist ein **großer, vollständiger Auftrag** mit sichtbarem Ergebnis im Spiel. Ein Agent arbeitet es in **einem** Goal-Mode-Lauf ganz ab: verstehen, umsetzen, selbst durchsehen, korrigieren, mergen.

**Pflichtabschnitte** (der Validator prüft die Reihenfolge): ID, Ziel, Voraussetzungen, Scope, Betroffene Dateien/Module, Ausdrücklich nicht erlaubte Änderungen, Akzeptanzkriterien, Tests, Risikoklasse, Definition of Done. Dazu oben ein kurzer Bearbeitungsstatus.

**Inhalt:** Das Paket erklärt nicht nur *was*, sondern *warum* – besonders bei Satire und Optik, denn Umsetzungs-Agents nehmen sonst alles wörtlich. Es verlinkt die [Satire- und Lore-Bibel](../PRODUKT/SATIRE_UND_LORE_BIBEL.md) und nennt konkrete Dateien, Elementnamen und Befehle.

**Scope:** Das Rundum-Rework nutzt **ein** gemeinsames Scope-Manifest ([`WP-047`](../tools/architecture-validation/scopes/WP-047.production.scope.json)) für `WP-047` bis `WP-049`. Kein neues Manifest, kein neues Arbeitspaket für Teilschritte, Fixes oder Subagenten.

**Entscheidungen:** Technische Details im bestehenden Stack entscheidet der Agent selbst. Nur Änderungen am Produktversprechen (Regeln, Ökonomie, Spielstand-Format) brauchen Alex.

**Keine** QC-Schleifen, keine Prüfberichte, keine Evidenzordner, keine Teilbriefe, keine Abschlussdokumente außer dem Status im Paket und einer kurzen Chatmeldung an Alex.
