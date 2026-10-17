# Font-Provenienz – IBM Plex Sans / Sans Condensed

**Arbeitsstand:** dokumentarische Referenz für WP-035; keine Produkt-, Rechts- oder Runtimefreigabe.
**Abrufdatum:** 17.10.2026
**Gepinnte Originalquelle:** [`IBM/plex` Commit `71f558992315a12ca1f06a2215d57962a1992162`](https://github.com/IBM/plex/tree/71f558992315a12ca1f06a2215d57962a1992162)
**Lizenzdatei:** Originalbytes unverändert im [ZIP](font-license-original.zip), Eintrag `LICENSE.txt`, abgelegt. [Die lesbare Textkopie](font-license-original.txt) hat normalisierte LF-Zeilenenden und entfernte Zeilenendleerzeichen; ihr Wortlaut ist unverändert. Nur der ZIP-Inhalt trägt den Originalhash.

IBM beschreibt Plex als eigene offene Schriftfamilie; das offizielle Projekt verweist auf das IBM-GitHub-Repository. Das Repository führt IBM Plex Sans und IBM Plex Sans Condensed und steht unter OFL-1.1. [1] [2]

## Nachweis der beschafften Originaldateien

Die drei Binärdateien wurden direkt aus `raw.githubusercontent.com/IBM/plex/<pinned-commit>/…` abgerufen. Es erfolgte keine Konvertierung, Subsetbildung, Umbenennung, Bearbeitung oder Einbindung in eine Laufzeitstruktur. `file` erkannte alle drei als TrueType; Familien-, Schnitt- und Versionsname wurden zusätzlich mit `fc-scan` aus den eingebetteten Namensdaten gelesen.

| Lokaler dokumentarischer Pfad | Originalpfad am gepinnten Commit | Eingebetteter Name / Version | Bytes | SHA-256 |
|---|---|---|---:|---|
| `references/Fonts/IBMPlexSans-Regular.ttf` | [`packages/plex-sans/fonts/complete/ttf/IBMPlexSans-Regular.ttf`](https://raw.githubusercontent.com/IBM/plex/71f558992315a12ca1f06a2215d57962a1992162/packages/plex-sans/fonts/complete/ttf/IBMPlexSans-Regular.ttf) | IBM Plex Sans / Regular / 3.005 | 200,500 | `975dcda37d80f038dcd143c22e33ca2d97a0cc5a929aace1c749153b0fe1afa5` |
| `references/Fonts/IBMPlexSans-SemiBold.ttf` | [`packages/plex-sans/fonts/complete/ttf/IBMPlexSans-SemiBold.ttf`](https://raw.githubusercontent.com/IBM/plex/71f558992315a12ca1f06a2215d57962a1992162/packages/plex-sans/fonts/complete/ttf/IBMPlexSans-SemiBold.ttf) | IBM Plex Sans / SemiBold / 3.005 | 202,632 | `a20caf8286023a6a7a85e40b1d2a4ae9fc3e3b1f9eda8f4c542dd4986af67bb1` |
| `references/Fonts/IBMPlexSansCondensed-SemiBold.ttf` | [`packages/plex-sans-condensed/fonts/complete/ttf/IBMPlexSansCondensed-SemiBold.ttf`](https://raw.githubusercontent.com/IBM/plex/71f558992315a12ca1f06a2215d57962a1992162/packages/plex-sans-condensed/fonts/complete/ttf/IBMPlexSansCondensed-SemiBold.ttf) | IBM Plex Sans Condensed / SemiBold / 3.000 | 202,624 | `acc2b1e3b9597ee13b08b767f845bd038fe120cbee806859af7c01b60fe6e9e6` |
| `evidence/font-license-original.zip`, innerer Eintrag `LICENSE.txt` | [`LICENSE.txt`](https://raw.githubusercontent.com/IBM/plex/71f558992315a12ca1f06a2215d57962a1992162/LICENSE.txt) | SIL Open Font License 1.1 | 4,456 | `7e6b2818edbd8f6a01ae80641cc8f16a51080d08fb4e532be3a0b6f74adb07da` |

Die HTTP-Pfade wurden am genannten Commit mit Status 200 geprüft. Der Commit ist bewusst in jeder Quellen-URL statt eines beweglichen `master`-Verweises angegeben. So lassen sich Bytequelle und Prüfsumme erneut vergleichen.

## Herkunft und beabsichtigte dokumentarische Nutzung

| Frage | Nachweis / Grenze |
|---|---|
| Quelle | IBM verweist auf die Plex-Familie und den eigenen GitHub-Downloadweg. Das offizielle Repository beschreibt Sans und Sans Condensed ausdrücklich als Bestandteile der Familie. [1] [2] |
| Nutzung in dieser Bible | Die TTFs stützen ausschließlich die acht statischen HTML-Referenzseiten dieses Dokumentationspakets. Sie sind keine Unity-Assets, keine Addressables, keine Runtimeimporte und keine verbindliche Produktionsentscheidung. |
| Weitergabe | Jede spätere Bündelung oder Weitergabe muss gemäß OFL die Urheberrechts- und Lizenzhinweise beilegen. Die unveränderten Originalbytes im ZIP und die lesbare, formatnormalisierte Textkopie bleiben deshalb neben der Provenienz erhalten. [3] |
| Embedding | OFL 1.1 erlaubt das Einbetten; dies entbindet nicht von ihren Bedingungen. Eine spätere technische Einbindung, Produktfreigabe und Rechtsprüfung sind nicht durch diesen Nachweis erteilt. [3] |
| Kosten | Der Abruf erfolgte direkt aus dem offiziellen öffentlichen IBM-Repository; es wurde nichts gekauft oder kostenpflichtig beschafft. |

## Lizenzbedingungen, auf die eine spätere Verwendung achten muss

Die folgende Zusammenfassung ersetzt weder den unveränderten Originaltext noch eine Rechtsprüfung.

1. OFL 1.1 erlaubt die kostenlose Nutzung, Untersuchung, Kopie, Zusammenführung, Einbettung, Änderung und Weitergabe der Font Software. Originale oder geänderte Schriftdateien dürfen jedoch nicht **für sich allein** verkauft werden. [3]
2. Beim Bündeln, Weitergeben oder Verkaufen zusammen mit Software verlangt Ziffer 2 die Urheberrechtsnotiz und diese Lizenz in jeder Kopie; zulässig sind separate Textdatei, lesbarer Header oder ein für Nutzende leicht einsehbares Metadatenfeld. [3]
3. Die vorliegende `LICENSE.txt` nennt nach ihrer Definition keine konkreten **Reserved Font Names**. Falls eine maßgebliche Originalversion solche Namen benennt, gilt Ziffer 3: Eine geänderte Fontversion darf sie ohne ausdrückliche schriftliche Erlaubnis nicht als primären, Nutzenden sichtbaren Schriftnamen führen. [3]
4. Geänderte und unveränderte Font Software muss vollständig unter OFL weitergegeben werden; sie darf nicht unter einer anderen Lizenz für die Schriftdatei stehen. Dokumente, die mit der Schrift gesetzt werden, fallen laut Lizenz nicht unter diese Weitergabepflicht. [3]
5. Namen von Rechteinhabenden oder Autor:innen dürfen nicht zur Bewerbung oder Unterstützung einer geänderten Fassung eingesetzt werden, außer für einen zutreffenden Beitragshinweis oder mit ausdrücklicher Erlaubnis. [3]

## Änderungsgrenze dieses Arbeitsstands

Diese Referenzen sind **Originalkopien**: Ihre Prüfsummen binden den Status. Jede spätere Subset-, Format-, Tabellen-, Hinting-, Metadaten- oder Namensänderung wäre eine geänderte Version im Sinne der OFL-Definition und benötigt eine neue Provenienzzeile, neue Hashes sowie eine separate Lizenz- und Produktprüfung. Keine solche Änderung ist hier vorgeschlagen oder durchgeführt.

Die Wortmarken- und UI-Empfehlung in Kapitel 02 verwendet die Familiennamen nur als typografische Spezifikation. Sie beansprucht weder IBM als Absender noch eine Unterstützung, Partnerschaft oder Produktfreigabe.

## Referenzen

[1]: https://www.ibm.com/plex/ "IBM Plex official typeface page"
[2]: https://github.com/IBM/plex "IBM Plex official source repository"
[3]: https://raw.githubusercontent.com/IBM/plex/71f558992315a12ca1f06a2215d57962a1992162/LICENSE.txt "SIL Open Font License 1.1 in IBM Plex repository"

## Git-Persistierung des Original-Lizenztexts

Die vorhandene Repositoryregel `* text=auto` würde CRLF-Zeilenenden normalisieren; ein eigenes Formatproblem des ersten Eigenprüfungsstands wurde deshalb ohne Änderung von `.gitattributes` oder Projektregeln korrigiert. Der ZIP-Eintrag `LICENSE.txt` enthält die 4.456 Originalbytes mit SHA-256 `7e6b2818edbd8f6a01ae80641cc8f16a51080d08fb4e532be3a0b6f74adb07da`, unverändert nach Entpacken. Die direkte `.txt`-Datei ist eine lesbare Formatkopie (LF, ohne Zeilenendleerzeichen); sie wird ausdrücklich nicht als bytegleicher Originaldownload ausgegeben. Keine Schriftdatei wurde verändert.
