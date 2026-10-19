# Tatsächliche Quelle der nativen Logprofile

Die unveränderten I_NATIVE_04/05/06/07/08/09-Originalpacks verwenden ausschließlich das bestehende T-Redaktionsmuster und dessen Marker für Konto-/Lizenzzeilen. Das zusätzlich historisch mitkopierte generische Feld `redactionPolicy.originalHashMeaning` spricht noch von GitHub-Joblogs. Für diese **nativen** Packs sind tatsächliche Quelle und Hashbedeutung stattdessen die vollständigen lokalen Unity-UTF8-Logdateien unter dem jeweiligen isolierten Laufverzeichnis; `rawBytes`/`rawSha256` bezeichnen diese lokalen Originalbytes. Ihre vollständigen redaktierten Profile und alle übrigen unredigierten Originaldateien sind über die Transportfelder nachrechenbar.

Die unabhängige Native08-Prüfung hat alle acht lokalen Logoriginale und Redaktionsprofile tatsächlich bytegenau nachgerechnet. Es wird weder ein GitHub-Abruf dieser nativen Logs noch ein CI-Seatfreigabe-Receipt aus dem lokalen Editorende abgeleitet. Diese additive Quellenpräzisierung ändert keinen Originalpack, keinen Hash, kein Prüfergebnis und kein Redaktionsmuster.

## Ergänzung für die tatsächlichen Native10–13

Auch I_HEADLESS_001_NATIVE_10_13_ORIGINALS.json hat die ursprüngliche T-Redaktionspolicy unverändert übernommen. Deren generischer originalHashMeaning-Text nennt GitHub, während die spezifischen rawLogProvenance- und vier profile.rawLocalNative-Felder ausdrücklich tatsächliche lokale Unitydateien und deren SHA256 beschreiben. Diese spezifische lokale Herkunft ist maßgeblich; keine CI-Seatfreigabe wird daraus abgeleitet. Die eingefrorenen Originalbytes bleiben unverändert. Echte CI-Freigaben sind ausschließlich durch die jeweiligen vollständigen CI-Originallogs gebunden.
