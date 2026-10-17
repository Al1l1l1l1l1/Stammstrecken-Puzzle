"""Create the human review edition from the sole canonical DRAFT source."""
from pathlib import Path
import argparse
import json
import sys

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / 'Content/Localization/source/de-DE.json'
OUTPUT = ROOT / 'Content/Reviews/text-library/WP-036/READING_EDITION.md'


def render(source):
    lines = ['# Lesefassung der vollständigen Textbibliothek', '',
             'Automatisch aus der kanonischen de-DE.json abgeleitet. Alle Texte sind DRAFT. '
             'Die als INTERN ausgewiesenen Einstiege sind keine Spielerhilfe und dürfen nicht automatisch angezeigt werden.', '',
             '## Die 240 Meldungen', '']
    entries = source['entries']
    for i in range(0, 960, 4):
        title, status, entry, result = entries[i:i+4]
        pid = title['key'].removesuffix('.title').replace('level.s1.', 'S1-').replace('.', '-')
        lines += [f"### {pid} · {title['text']}", '',
                  f"Status: {status['text']}", '', f"Ergebnis: {result['text']}", '',
                  f"**INTERN · Einstieg:** {entry['text']}", '']
        for label, item in [('Status kurz', status), ('Ergebnis kurz', result)]:
            if 'shortText' in item:
                lines += [f"{label}: {item['shortText']}", '']
    lines += ['## Kampagne, Oberflächen und Varianten', '']
    previous = None
    for item in entries[960:]:
        group = '.'.join(item['key'].split('.')[:2])
        if group != previous:
            lines += [f'### {group}', '']
            previous = group
        lines += [f"**{item['key']}** · {item['text']}", '',
                  f"Einsatz: {item['description']}", '']
        if item['availability'] == 'BLOCKED_PRODUCT':
            lines += ['**GESPERRTER ENTWURF:** ' + ', '.join(item['blockedBy']), '']
        if 'shortText' in item:
            lines += [f"Kurz: {item['shortText']}", '']
    return '\n'.join(lines).rstrip() + '\n'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check', action='store_true')
    args = parser.parse_args()
    expected = render(json.loads(SOURCE.read_text(encoding='utf-8'))).encode('utf-8')
    if args.check:
        if not OUTPUT.exists() or OUTPUT.read_bytes().replace(b'\r\n', b'\n') != expected:
            print('READING_EDITION FAIL: missing or stale derived edition')
            return 1
        print('READING_EDITION PASS: exact canonical text projection')
    else:
        OUTPUT.write_bytes(expected)
        print('READING_EDITION written from canonical source')
    return 0


if __name__ == '__main__':
    sys.exit(main())
