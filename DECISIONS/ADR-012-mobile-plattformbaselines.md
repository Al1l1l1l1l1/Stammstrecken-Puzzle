# ADR-012 – Mobile Plattformbaselines für Android und iOS

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Die App benötigt konkrete Deployment Targets für Toolchain, SDK-Auswahl, Geräteprüfung und Support. Store-Uploadziele wie Android API 36 oder das iOS-26-SDK bestimmen nicht automatisch das älteste installierbare Betriebssystem. Eine sehr alte Baseline vergrößert Test- und SDK-Komplexität; eine zu neue Baseline schließt Spieler ohne technischen Produktvorteil aus.

Google Mobile Ads 11.5.0 setzt mindestens Android API 23 und iOS 13 voraus.[1] Das Spiel selbst benötigt keine neuere exklusive Betriebssystemfunktion. Die Architektur soll deshalb eine konservative, aber nicht unbegrenzt alte Reichweite unterstützen.

## Entscheidung

Die initialen Deployment Targets sind:

| Plattform | Mindestversion | Releaseziel |
|---|---:|---|
| Android | **Android 8.0 / API 26** | `targetSdkVersion` und `compileSdkVersion` mindestens API 36 sowie mindestens das bei Einreichung aktuelle Google-Play-Mandat. |
| iOS | **iOS 15.0** | Build mit Xcode 26+ und iOS-26-SDK+ sowie mindestens dem bei Einreichung aktuellen App-Store-Mandat. |

Release-Builds sind ARM64/IL2CPP. Kernfunktionalität darf keine API oberhalb der Mindestversion ohne Runtime-Guard voraussetzen. SDKs, Privacy Manifests und Storevorgaben werden je Release erneut geprüft.

Der initiale physische Mindest-Gerätevertrag umfasst mindestens ein ARM64-Android-Gerät mit API 26 und 3 GiB RAM sowie ein iPhone 8 beziehungsweise ein gleich oder schwächer eingestuftes iOS-15-Gerät. Zusätzlich werden je ein aktuelles Android- und iOS-Gerät geprüft. Emulatoren ergänzen, ersetzen aber nicht diese Geräte.

Vor dem ersten Produktions-Scaffold wird die Reichweite mit aktuellen Store-/Zielgruppen- und vorhandenen Geräteinformationen dokumentiert. Ergibt diese Prüfung eine relevante Zielgruppenausgrenzung oder ist eine notwendige SDK-Linie inkompatibel, wird dieses ADR vor Implementierung durch ein neues ADR ersetzt; die Werte werden nicht still geändert.

## Begründung

API 26 bietet einen stabilen app-privaten Dateisystem- und ARM64-Vertrag, liegt über dem SDK-Minimum und begrenzt die Android-Testmatrix. iOS 15 ermöglicht eine breite ältere Gerätebasis bei moderner Xcode-/SDK-Toolchain. Die Baselines sind bewusst Deployment- und Testentscheidungen, keine Aussage über Store-Target-APIs.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Android API 23 / iOS 13 | Maximale SDK-kompatible Reichweite, aber größere Altgeräte-/OS-Testfläche und höheres Performance-/Lifecycle-Risiko. |
| Android API 28 / iOS 16 | Einfachere Supportmatrix, schließt ohne derzeit belegten Produktbedarf mehr Geräte aus. |
| Nur „jeweils aktuell“ | Nicht reproduzierbar und für Runner, QA und Agenten nicht umsetzbar. |
| Unterschiedliche Featurestände je OS | Würde Produkt- und Testdrift zwischen Android und iOS erzeugen. |

## Konsequenzen

UI, Save, Ads, IAP, Consent und Audio müssen auf den Mindestgeräten getestet werden. Abhängigkeiten mit höheren Mindestversionen dürfen nicht eingeführt werden. Das Team muss reale oder remote erreichbare Referenzgeräte bereitstellen. Eine spätere Anhebung benötigt Nutzungsdaten, Supportaufwand und ein ersetzendes ADR.

## Betroffene Artefakte

`ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md` sowie spätere Player Settings und Gerätelisten.

## Validierung

Der erste Scaffold erzeugt installierbare IL2CPP-Builds für beide Mindestversionen. Kernflow, Save-Recovery, großes 10×10-Raster, Low-Memory, Hintergrundwechsel, Consent, Testanzeige, IAP-Sandbox und Audiointerruption laufen auf der Referenzmatrix. Store-Preflight prüft getrennt Deployment Target und aktuelles Uploadziel.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: https://developers.google.com/admob/unity/quick-start "Set up Google Mobile Ads Unity Plugin"
[2]: https://developer.android.com/google/play/requirements/target-sdk "Meet Google Play's target API level requirement"
[3]: https://developer.apple.com/news/upcoming-requirements/ "Apple Upcoming Requirements"
