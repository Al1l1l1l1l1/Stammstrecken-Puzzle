#!/usr/bin/env python3
"""Reproduce WP-035's documentary HTML reference views; not a game build."""
from pathlib import Path
import json
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parent


def main():
    reports = []
    with sync_playwright() as p:
        browser = p.chromium.launch(executable_path='/usr/bin/chromium', args=['--no-sandbox'])
        page = browser.new_page(viewport={'width': 1440, 'height': 1000}, device_scale_factor=2)
        for source in sorted((ROOT / 'references').glob('*.html')):
            errors = []
            handler = lambda error: errors.append(str(error))
            page.on('pageerror', handler)
            page.goto(source.as_uri(), wait_until='networkidle')
            page.evaluate('document.fonts.ready')
            fonts = page.evaluate('Array.from(document.fonts).map(f => ({family:f.family,status:f.status}))')
            bad = [font for font in fonts if font['status'] == 'error']
            if bad or errors:
                raise RuntimeError(f'{source.name}: fonts={bad}; page={errors}')
            output = source.with_suffix('.png')
            page.screenshot(path=str(output), full_page=True)
            reports.append({'source': source.relative_to(ROOT).as_posix(),
                            'output': output.relative_to(ROOT).as_posix(),
                            'fonts': fonts, 'pageErrors': errors,
                            'renderedSize': page.evaluate('({width:document.documentElement.scrollWidth,height:document.documentElement.scrollHeight})')})
            page.remove_listener('pageerror', handler)
            print(f'Referenz erzeugt: {output.name}', flush=True)
        browser.close()
    (ROOT / 'evidence' / 'render-results.json').write_text(json.dumps(reports, indent=2, ensure_ascii=False) + '\n')


if __name__ == '__main__':
    main()
