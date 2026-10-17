#!/usr/bin/env python3
"""WP-035 documentary delivery checks; not runtime QA or independent approval."""
from pathlib import Path
import csv
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET
import zipfile
from urllib.parse import unquote
from PIL import Image
from fontTools.ttLib import TTFont
from bs4 import BeautifulSoup

ROOT = Path(__file__).resolve().parent
REPO = ROOT.parents[2]
BASE = 'd90e798dd9b26d3f152f3b0c2d1d52cae641eb16'
CHAPTERS = ['README.md', '01_Marke_und_App-Icon.md', '02_Typografie_und_Farben.md',
            '03_UI_und_Puzzlebausteine.md', '04_Zuege_Lackierungen_und_Betriebsobjekte.md',
            '05_Welt_und_Kartenmaterial.md', '06_Motion_und_Sound.md',
            '07_Produktionsinventar_und_Uebergabe.md']
FONT_HASHES = {
    'IBMPlexSans-Regular.ttf': '975dcda37d80f038dcd143c22e33ca2d97a0cc5a929aace1c749153b0fe1afa5',
    'IBMPlexSans-SemiBold.ttf': 'a20caf8286023a6a7a85e40b1d2a4ae9fc3e3b1f9eda8f4c542dd4986af67bb1',
    'IBMPlexSansCondensed-SemiBold.ttf': 'acc2b1e3b9597ee13b08b767f845bd038fe120cbee806859af7c01b60fe6e9e6',
}


def main():
    errors, checked, images, fonts = [], [], [], []
    for chapter in CHAPTERS:
        path = ROOT / chapter
        if not path.is_file() or path.stat().st_size < 1000:
            errors.append(f'Kapitel fehlt oder ist unvollständig: {chapter}')
    for path in sorted(ROOT.rglob('*')):
        if not path.is_file() or '__pycache__' in path.parts:
            continue
        relative = path.relative_to(ROOT).as_posix()
        checked.append(relative)
        if path.stat().st_size == 0:
            errors.append(f'Leere Datei: {relative}')
        try:
            if path.suffix.lower() == '.png':
                with Image.open(path) as image:
                    image.verify()
                with Image.open(path) as image:
                    images.append({'path': relative, 'width': image.width, 'height': image.height, 'mode': image.mode})
            elif path.suffix.lower() == '.svg':
                ET.parse(path)
            elif path.suffix.lower() == '.json':
                json.loads(path.read_text())
            elif path.suffix.lower() == '.zip':
                with zipfile.ZipFile(path) as archive:
                    if archive.testzip() is not None:
                        errors.append(f'Archiv beschädigt: {relative}')
                    if path.name == 'font-license-original.zip':
                        raw = archive.read('LICENSE.txt')
                        if len(raw) != 4456 or hashlib.sha256(raw).hexdigest() != '7e6b2818edbd8f6a01ae80641cc8f16a51080d08fb4e532be3a0b6f74adb07da':
                            errors.append('Original-Lizenzbytes im Archiv stimmen nicht.')
            elif path.suffix.lower() == '.ttf':
                font = TTFont(path)
                digest = hashlib.sha256(path.read_bytes()).hexdigest()
                if digest != FONT_HASHES.get(path.name):
                    errors.append(f'Originalfont-Hash stimmt nicht: {relative}')
                for character in 'ÄÖÜäöüß0123456789':
                    if ord(character) not in font.getBestCmap():
                        errors.append(f'Schriftzeichen {character} fehlt: {relative}')
                fonts.append({'path': relative, 'sha256': digest, 'germanGlyphsPresent': True})
                font.close()
            elif path.suffix.lower() == '.md':
                for destination in re.findall(r'\[[^\]]*\]\(([^)]+)\)', path.read_text()):
                    destination = destination.split(' "', 1)[0].strip()
                    if not destination or destination.startswith(('https:', 'http:', 'mailto:', '#')):
                        continue
                    target = unquote(destination.split('#', 1)[0])
                    if target and not (path.parent / target).resolve().exists():
                        errors.append(f'Gebrochener Link in {relative}: {destination}')
            elif path.suffix.lower() == '.html':
                text = path.read_text()
                soup = BeautifulSoup(text, 'html.parser')
                if soup.find('script') or soup.find('form') or soup.find('iframe'):
                    errors.append(f'Dynamische App-/Form-Struktur in {relative}')
                for target in re.findall(r'(?:src=[\'\"]|url\([\'\"]?)([^\'\"\)]+)', text):
                    if target.startswith(('http:', 'https:')):
                        errors.append(f'Externe Laufzeitanfrage in {relative}: {target}')
                    elif not target.startswith(('data:', '#')) and not (path.parent / target).exists():
                        errors.append(f'Fehlendes HTML-/Font-Ziel: {relative}: {target}')
        except Exception as exc:
            errors.append(f'Datei nicht lesbar {relative}: {exc}')
    for expected in ['01-brand', '02-type-color', '03-puzzle', '04-screens',
                     '05-vehicles-props', '05-vehicles-construction', '06-worlds',
                     '06-world-construction', '07-route-map', '08-motion-audio']:
        if not (ROOT / 'references' / (expected + '.png')).is_file():
            errors.append(f'Referenz-PNG fehlt: {expected}')
    rows = list(csv.DictReader((ROOT / 'evidence' / 'asset-requirements.csv').open(newline='')))
    ids = [row['asset_id'] for row in rows]
    if len(rows) != 40 or len(ids) != len(set(ids)):
        errors.append('Inventar enthält nicht exakt 40 eindeutige Anforderungen.')
    for row in rows:
        for target in row['reference_file'].split(';'):
            if not (ROOT / target.strip()).is_file():
                errors.append(f'Inventarziel fehlt: {row["asset_id"]}: {target}')
    soup = BeautifulSoup((ROOT / 'references' / '03-puzzle.html').read_text(), 'html.parser')
    counts = []
    for size in [4, 10]:
        cells = soup.select('.grid4 > .cell') if size == 4 else soup.select('.grid10 > .g10cell')
        occupied = []
        for i, cell in enumerate(cells):
            if not cell.select_one('.rail, .open-stub'):
                continue
            if size == 4:
                occupied.append(divmod(i, size))
            else:
                style = cell.get('style', '')
                col = int(re.search(r'left:(\d+)(?:px)?', style).group(1)) // 32
                row = int(re.search(r'top:(\d+)(?:px)?', style).group(1)) // 32
                occupied.append((row, col))
        for axis, selector in [('column', '.top-counts' if size == 4 else '.top10'),
                               ('row', '.side-counts' if size == 4 else '.side10')]:
            for i, span in enumerate(soup.select(selector + ' > span')):
                actual = sum(1 for row, col in occupied if (col if axis == 'column' else row) == i)
                target = int(span.get_text())
                expected = 'exact' if actual == target else 'over' if actual > target else 'open'
                if expected not in span.get('class', []):
                    errors.append(f'Count-Fixture falsch: {size}x{size} {axis}{i}: {actual}/{target} -> {expected}')
                counts.append({'size': size, 'axis': axis, 'index': i, 'occupancy': actual, 'target': target, 'state': expected})
    tokens = json.loads((ROOT / 'references' / 'design-tokens.json').read_text())
    colors = {name: data['$value'] for name, data in tokens['color'].items()}
    def luminance(color):
        channels = [int(color[i:i+2], 16) / 255 for i in [1, 3, 5]]
        linear = [c / 12.92 if c <= 0.04045 else ((c + 0.055) / 1.055) ** 2.4 for c in channels]
        return sum(weight * c for weight, c in zip([0.2126, 0.7152, 0.0722], linear))
    contrasts = []
    chapter = (ROOT / '02_Typografie_und_Farben.md').read_text()
    for a, b, documented in re.findall(r'\| `([^`]+)` auf `([^`]+)` \| ([\d.]+):1', chapter):
        la, lb = luminance(colors[a]), luminance(colors[b])
        calculated = (max(la, lb) + 0.05) / (min(la, lb) + 0.05)
        if abs(calculated - float(documented)) > 0.015:
            errors.append(f'Kontrastwert stimmt nicht: {a}/{b}: {calculated} statt {documented}')
        contrasts.append({'foreground': a, 'background': b, 'calculated': round(calculated, 4), 'documented': float(documented)})
    protected = ['Assets', 'Content', 'Packages', 'ProjectSettings', 'ARCHITECTURE',
                 'DECISIONS', 'AGENTS.md', 'Stammstrecken_Puzzle_Konzept_00-15', '.github']
    changes = subprocess.check_output(['git', '-C', str(REPO), 'diff', '--name-only', BASE, '--', *protected], text=True).splitlines()
    if changes:
        errors.append('Fremdquellen geändert: ' + ', '.join(changes))
    report = {'kind': 'WP-035 Eigenprüfung der dokumentarischen Lieferung',
              'notIndependentApproval': True, 'baseCommit': BASE,
              'filesChecked': checked, 'images': images, 'fonts': fonts,
              'inventoryRows': len(rows), 'countFixtures': counts, 'contrastChecks': contrasts,
              'protectedSourceChanges': changes, 'errors': errors, 'passed': not errors}
    (ROOT / 'evidence' / 'delivery-validation.json').write_text(json.dumps(report, indent=2, ensure_ascii=False) + '\n')
    inventory = []
    for path in sorted(ROOT.rglob('*')):
        if path.is_file() and path.name != 'file-manifest.json' and '__pycache__' not in path.parts:
            data = path.read_bytes()
            inventory.append({'path': path.relative_to(ROOT).as_posix(), 'bytes': len(data),
                              'sha256': hashlib.sha256(data).hexdigest()})
    (ROOT / 'evidence' / 'file-manifest.json').write_text(json.dumps(inventory, indent=2, ensure_ascii=False) + '\n')
    print(json.dumps({'passed': not errors, 'files': len(checked), 'images': len(images),
                      'fonts': len(fonts), 'inventoryRows': len(rows), 'countChecks': len(counts),
                      'contrastChecks': len(contrasts), 'errors': errors}, ensure_ascii=False, indent=2))
    raise SystemExit(1 if errors else 0)


if __name__ == '__main__':
    main()
