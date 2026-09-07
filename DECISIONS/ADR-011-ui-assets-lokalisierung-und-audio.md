# ADR-011 – UI Toolkit, lokale Addressables, Unity Localization und Unity Audio

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Das Spiel kombiniert app-artige Informationsflächen und ein stark fokussiertes Raster mit einer räumlichen Zugfahrt. Assets und Texte müssen für 240 Level strukturiert, lokalisierbar und von wechselnden Agenten ohne Szenenwissen auffindbar sein. Ein Audio-Middleware-Stack wäre für den derzeitigen Umfang unnötig.

## Entscheidung

Die Runtime-Benutzeroberfläche wird mit **UI Toolkit** umgesetzt. Menüs, Betriebslage, Betriebswerk, Profil, Puzzle-HUD und Ergebnisfläche verwenden UXML/USS und Presenter. Das Raster ist ein getestetes Custom `VisualElement`; seine Darstellung konsumiert ausschließlich immutable View State. Die 2D-/2.5D-Zugfahrt verwendet Unity GameObjects, URP und Timeline/Animation nur in der Präsentationsschicht. uGUI wird nicht parallel als zweites UI-System eingeführt, außer ein Drittanbieter-SDK erzwingt eine isolierte Adapteroberfläche.

**Addressables 2.10.3** ist die Assetladegrenze für Unity 6.3. Alle Launchinhalte liegen lokal im App-Bundle; Remote Catalogs und nachgeladener Gameplay-Content sind für v0.1 deaktiviert. Gruppen heißen mindestens `core`, `season-s1`, `cosmetics`, `audio` und `localization`. Stabile logische Adressen ersetzen direkte Szenen- oder Resources-Pfade. Der `Resources`-Ordner ist außer für nachweislich SDK-erzwungene Konfiguration verboten.

**Unity Localization 1.5.13** verwaltet String- und Assettabellen. Deutsch (`de-DE`) ist Startlocale, aber jeder sichtbare Text verwendet einen stabilen Schlüssel; keine sichtbaren Literale in C# oder UXML. Variablen werden als typisierte Platzhalter übergeben. Leveldaten speichern Textschlüssel statt lokalisierter Texte.

Audio nutzt Unity `AudioSource`, importierte Clips und einen zentralen `AudioMixer` mit Gruppen `Master`, `Music`, `SFX`, `Ambience` und `UI`. `IAudioPort` nimmt semantische Cues entgegen; Domain und Application kennen keine Clipnamen. Einstellungen für Lautstärke, Musik, Effekte, Haptik und stummgeschalteten Hintergrundbetrieb sind lokal persistiert.

## Begründung

UI Toolkit passt zur dokumentierten Informationsarchitektur und unterstützt strukturierte Styles, Custom Controls und automatisierte UI-Prüfungen in Unity 6.3. Addressables und Localization sind offizielle, versionierbare Pakete. Lokale Bundles bewahren Offline-First. Unity Audio genügt für präzise UI-, Umgebungs- und Zuggeräusche ohne zusätzliche Lizenz oder Runtime.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| uGUI für alle Screens | Bewährt, aber stärker GameObject-/Szenen-zentriert und für die app-artige, stylesensitive Oberfläche weniger transparent. |
| UI Toolkit und uGUI gemischt | Erhöht Lern-, Test- und Wartungsfläche ohne bestätigten Bedarf. |
| Direkte Assetreferenzen und `Resources` | Erschweren Inventar, Speicherkontrolle und stabile logische Verträge. |
| Remote Addressables zum Launch | Fügt Netzwerk-, Cache- und Versionierungsfehler hinzu, obwohl alle 240 Level lokal verfügbar sein sollen. |
| FMOD oder Wwise | Leistungsfähig, aber für den bestätigten Audioumfang unverhältnismäßig. |

## Konsequenzen

Custom Grid, Focus-Navigation, Touchhitboxes, Safe Areas und Screenreader-Semantik benötigen frühe Spike- und Gerätetests. Alle Assetadressen und Lokalisierungsschlüssel werden in CI inventarisiert. Ein späterer Remote-Content-Kanal erfordert ein neues ADR.

## Betroffene Artefakte

`ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/CONTENT_PIPELINE.md`, `ARCHITECTURE/MOBILE_SERVICES.md` und `ARCHITECTURE/TEST_STRATEGY.md`.

## Validierung

PlayMode- und Gerätetests prüfen Puzzlegrid, Fokus, Touch, Safe Areas, dynamische Schrift, fehlende Keys, Addressable-Abhängigkeiten, Audioeinstellungen und Speicherfreigabe. CI verbietet sichtbare Stringliterale und nicht erlaubte `Resources`-Assets.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://unity.com/blog/unity-6-3-lts-is-now-available "Unity 6.3 LTS is now available"
[2]: https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.addressables.html "Addressables package for Unity 6.3"
[3]: https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.localization.html "Localization package for Unity 6.3"
[4]: ../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md "Stammstrecken-Puzzle – UI- und Bedienungsspezifikation"
