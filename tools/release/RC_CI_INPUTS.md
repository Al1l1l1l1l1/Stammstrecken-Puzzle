# Gepinnte RC-CI-Eingaben

Diese Bridge transportiert geschützte Eingaben. Sie erteilt keine Content-, Human-, Rechts-, Geräte-, Provider-, Signing-, Store- oder Veröffentlichungsfreigabe. Die vorhandenen `ProductionImportPreflight`-/`ProductionGameBuild`-/Preset-/IAP-/Publisherprüfungen bleiben erforderlich. Getaggter Code, Content und Profil werden weder entpackt noch überschrieben.

## Geschlossenes Bundle

Das gepinnte GitHub-Actions-Artefakt enthält ausschließlich `rc-inputs.json` und optional `iap-review-original.json`. Der ZIP-Raw-SHA256 stammt aus einer geschützten, plattformgebundenen CIvariablen. Der Descriptor hat genau diese fünf Felder:

```json
{
  "schemaVersion": 1,
  "platform": "ANDROID",
  "gitCommit": "<40 lowercase hex>",
  "applicationIdentifier": "<actual production app id>",
  "environment": {
    "STP_PRODUCTION_SOURCE_ROOT": ".",
    "STP_PRODUCTION_IMPORT_REQUEST": "Content/Approved/request.json",
    "STP_PRODUCTION_APPROVAL_LOCK": "Content/Approved/approval.json",
    "STP_PRODUCTION_APPROVAL_RAW_SHA256": "<actual raw sha256>",
    "STP_PRODUCTION_SERVICE_PROFILE": "Assets/StammstreckenPuzzle/Settings/ActualProduction.asset",
    "STP_PRODUCTION_SERVICE_PROFILE_RAW_SHA256": "<actual raw sha256>",
    "STP_PRODUCTION_IAP_REVIEW_ORIGINAL": "iap-review-original.json",
    "STP_PRODUCTION_IAP_REVIEW_RAW_SHA256": "<actual original raw sha256>",
    "STP_PRODUCTION_IAP_POLICY_REVISION": "<actual reviewed revision>",
    "STP_PRODUCTION_IAP_SDK_REVISION": "<actual reviewed SDK revision>",
    "STP_PUBLISHER_LEGAL_URL": "https://publisher.example/legal",
    "STP_PUBLISHER_SUPPORT_URL": "https://publisher.example/support"
  }
}
```

Die Beispiele sind Platzhalter, keine tatsächlich vorhandenen oder angenommenen Produktionsquellen. Der `environment`-Block enthält **genau zwölf bekannte Namen**. SourceRoot ist ausschließlich `.` und wird zum tatsächlichen Checkout aufgelöst. Request, ApprovalLock und ServiceProfile müssen kanonische ASCII/POSIX-Pfade regulärer, getrackter Dateien im identischen Commit sein; ihre Checkoutbytes müssen exakt den Gitblobs entsprechen. Approval-/Profilrawhash werden zusätzlich geprüft. Das Profil bleibt unter der bereits geltenden `Assets/StammstreckenPuzzle/Settings/*.asset`-Grenze. Der aktuelle Checkout und ein vorhandener annotierter RCtag müssen auf denselben erwarteten Commit zeigen; Tagauflösung verwendet `^{commit}`.

Das optionale IAPoriginal wird als unverändertes UTF-8-JSONobjekt transportiert und rawgehasht. Der Descriptor darf nur seinen festen Bundlenamen nennen. Inhaltliche PASS-/Rollenfelder werden nicht als Freigabe interpretiert. Ohne diese Datei müssen alle vier IAPwerte leer sein; das ist ausschließlich ein transportierbarer Fehl-/Vorbereitungsstand. Der bestehende Productionpreflight verlangt weiterhin seine echten IAP-/Policy-/SDKeingaben und wird fehlende Angaben ablehnen. Schlüssel, P12, Keychain oder beliebige weitere Dateien sind kein Bundleinput.

## CIaufruf

`GITHUB_TOKEN` wird ausschließlich über die Prozessumgebung übergeben. Der Onlinepfad erlaubt nur die feste API `https://api.github.com`, Version `2026-03-10`, und reine GETs. Keine Option verändert GitHubkonfiguration oder wählt einen fremden APIhost.

```sh
python tools/release/rc_ci_inputs.py \
  --repository "$GITHUB_REPOSITORY" \
  --run-id "$STP_RC_INPUT_RUN_ID" \
  --artifact-id "$STP_RC_INPUT_ARTIFACT_ID" \
  --zip-sha256 "$STP_RC_INPUT_ZIP_SHA256" \
  --ref "$STP_RC_EXPECTED_REF" \
  --commit "$STP_RC_EXPECTED_COMMIT" \
  --platform "$STP_RC_PLATFORM" \
  --application-id "$STP_RC_APPLICATION_ID" \
  --checkout "$GITHUB_WORKSPACE" \
  --runner-temp "$RUNNER_TEMP" \
  --github-env "$GITHUB_ENV" \
  --require-production-environment
```

Die ersten drei STPwerte stammen aus geschützten plattformgebundenen Variablen des Productionenvironments. `--run-id` ist die **Quell-Run-ID des vorbereiteten Inputartefakts**, nicht die ID des aktuellen RCpackagingruns. Die GitHubmetadaten müssen genau Repository, Quellrun, Tagref, Commit und Artefakt binden; fremde Headrepositories/Forks, abgelaufene Artefakte oder nicht erfolgreiche abgeschlossene Quellruns werden abgelehnt. Die Prüfung des Productionenvironments erfolgt unmittelbar vor dem Download und verlangt eine tatsächliche `required_reviewers`-Regel mit mindestens einem gültigen User-/Teamreviewer. Sie erfindet weder eine Zweipersonen-/Selfreviewpflicht noch eine bereits ausgeführte oder fachlich qualifizierte Freigabe. Der Workflow muss selbst das geschützte Environment verwenden.

Der authentifizierte API-Downloadrequest akzeptiert ausschließlich die erwartete 302-Antwort. Deren Location wird mit einem getrennten Opener ohne Authorization, Cookies, Proxies und automatische Redirects geladen. Nur öffentlich aufgelöste HTTPSziele sind zulässig; die geprüfte IP wird für den tatsächlichen TCPaufbau festgehalten, während Host, TLS-SNI und Zertifikatsprüfung den ursprünglichen Host behalten. Signed-URLs und Token werden nicht ausgegeben. Metadaten-/Downloadgrößen und Lesezeiten sind begrenzt; zusätzliche Redirects schlagen geschlossen fehl.

ZIP-Rawhashprüfung kommt vor ZIPparsing und Extraktion. Höchstens zwei exakt benannte reguläre Dateien, insgesamt maximal20MiB/einzeln16MiB; Größen-/Kompressionsratio-/CRCprüfungen und Ablehnung von Duplikaten, Casekollisionen, Pfadescapes, Backslashes, Symlink-/Reparse-/Directory-/Encrypted-Entries. Jeder Output liegt in einem frisch exklusiv erstellten `stp-rc-inputs-<uuid>`-Verzeichnis außerhalb des Checkouts. POSIXmodus0700; Windows erbt die bereits geschützte RunnerTemp-ACL. Beide Originale werden readonly geschrieben und erneut bytegeprüft. Vor Ausgabe werden Checkout/Tag und alle Quelldateien nochmals geprüft.

`--github-env` muss im Onlinepfad die tatsächliche `GITHUB_ENV`-Datei bezeichnen und kanonisch innerhalb RunnerTemp liegen. Erst nach allen Nachweisen werden ausschließlich die zwölf aufgelösten Namen als einzelne geprüfte ASCIIzeilen angehängt. Keine freie Envwahl, kein Eval, kein Multiline-/HereDocinput. Stdout enthält nur eine begrenzte geheimfreie Zusammenfassung; Fehler nur feste Codes ohne Rohantwort, URL oder Traceback.

## Offline und Tests

`--zip /absolute/path/to/original.zip` führt denselben Raw-/ZIP-/Descriptor-/Checkout-/Blob-/Ausgabepfad ohne API durch. Das Ergebnis weist ausdrücklich `githubArtifactProvenanceVerified=false` aus. Dieser Modus kann nicht mit `--require-production-environment` kombiniert werden und ist kein CIfallback. Der RCworkflow verwendet ausschließlich den Onlinepfad.

```sh
python -B tools/release/test_rc_ci_inputs.py
```

Die Tests verwenden kontrollierte Git-/HTTPantworten, reale isolierte temporäre Dateien und die tatsächlichen Python-SSL-/urllibfactorysignaturen. Sie starten weder Git noch Netzwerk, Unity, Signing oder Storeaktionen. Tempcleanup greift nur auf den erfolgreich selbst erstellten Root zu. Positiv- und Negativfälle prüfen insbesondere Credentialtrennung/302, IPbindung/TLS, Repo/Run/Ref/Commit, Tagpeeling, falsche Rawhashes/geänderte Quelldateien, manipulierte ZIPs, Environmentregel, zwölf Envwerte, Injection, fehlende Inputs und ausbleibende Ausgabe vor vollständigem Nachweis.

Erster lokaler Versuch: reiner Fixture-Setupfehler durch Python0700-DACL im Windows-AppContainer; keine Testkörper-/Produktbewertung daraus. Der Nachlauf verwendet vererbte Runner-ACLs und besitzt seine Roots ausdrücklich. Kein tatsächlicher GitHub-/CI-/Produktionsgate-PASS wird aus kontrollierten Tests abgeleitet.

Offizielle Schnittstellen: [Artifacts API](https://docs.github.com/en/rest/actions/artifacts?apiVersion=2026-03-10), [Workflow Runs API](https://docs.github.com/en/rest/actions/workflow-runs?apiVersion=2026-03-10), [Environments API](https://docs.github.com/en/rest/deployments/environments?apiVersion=2026-03-10). Der APIheader und die getrennte Redirectbehandlung folgen diesen Schnittstellen; ihre Verwendung ersetzt keine tatsächliche zuständige Freigabe.
