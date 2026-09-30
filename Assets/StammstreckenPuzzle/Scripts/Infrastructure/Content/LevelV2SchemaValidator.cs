using System.Collections.Generic;
using System.Text.RegularExpressions;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Strukturelle Prüfung gegen den level-v2-Vertrag. Das kanonische
    /// Schemadokument bleibt ARCHITECTURE/schemas/level-v2.schema.json
    /// (Draft 2020-12); diese Klasse bildet dessen feststehende Regeln
    /// (Typen, Pflichtfelder, <c>additionalProperties: false</c>, Konstanten,
    /// Enums, Muster, Wertebereiche, Listenlängen, <c>uniqueItems</c> und die
    /// <c>oneOf</c>-Form der Sternschwellen) als zweckgebauten Prüfer für
    /// genau diesen Vertrag ab. Eine generische Schemamaschine ist nicht
    /// Bestandteil; unbekannte Eigenschaften und unbekannte
    /// <c>documentSchemaVersion</c>/<c>rulesetVersion</c> schlagen fail-closed
    /// mit stabilen Codes der Familie <c>LVL-SCHEMA-*</c> fehl.
    /// </summary>
    public static class LevelV2SchemaValidator
    {
        private static readonly Regex PuzzleIdPattern = new Regex(@"^S[1-9][0-9]*-[0-9]{2}-[0-9]{2}-[0-9]{2}$", RegexOptions.CultureInvariant);
        private static readonly Regex LocalizationKeyPattern = new Regex(@"^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)+$", RegexOptions.CultureInvariant);
        private static readonly Regex ArtifactIdPattern = new Regex(@"^[A-Za-z0-9._/-]+$", RegexOptions.CultureInvariant);
        private static readonly Regex Sha256Pattern = new Regex(@"^[0-9a-f]{64}$", RegexOptions.CultureInvariant);

        private static readonly HashSet<string> FocusValues = new HashSet<string>(System.StringComparer.Ordinal)
        {
            "OCCUPANCY", "EXCLUSION", "ENDPOINT_GEOMETRY", "CHAIN", "DENSITY",
            "COMBINATION", "ROW_COLUMN_COUNTS", "CONNECTIVITY", "NO_BRANCH", "NO_LOOP",
        };

        private static readonly string[] RootKeys =
        {
            "documentSchemaVersion", "rulesetVersion", "puzzleId", "contentRevision", "grid", "endpoints",
            "rowCounts", "columnCounts", "content", "production", "completion", "solution", "proofRef",
        };

        /// <summary>
        /// Prüft ein geparstes Dokument strukturell gegen den level-v2-Vertrag
        /// und erzeugt bei Erfolg das DTO. Alle gefundenen Verletzungen werden
        /// als Diagnosen gesammelt; bei mindestens einer Verletzung ist das
        /// Ergebnis <c>null</c> (fail-closed).
        /// </summary>
        public static LevelV2Document? Validate(JsonValue root, List<LevelDiagnostic> diagnostics)
        {
            var cursor = new Cursor(diagnostics, "LVL-SCHEMA-TYPE", "LVL-SCHEMA-ADDITIONAL-PROPERTY", "LVL-SCHEMA-MISSING-PROPERTY");
            var rootObject = cursor.Object(root, "$", "LVL-SCHEMA-ROOT");
            if (rootObject is null)
            {
                return null;
            }
            cursor.CheckKeys(rootObject, "$", RootKeys);
            cursor.RequireKeys(rootObject, "$", RootKeys);

            var documentSchemaVersion = cursor.IntegerMember(rootObject, "$", "documentSchemaVersion");
            if (documentSchemaVersion.HasValue && documentSchemaVersion.Value != 2L)
            {
                cursor.Fail("LVL-SCHEMA-DOCUMENT-SCHEMA-VERSION", "$.documentSchemaVersion", $"Unbekannte documentSchemaVersion {documentSchemaVersion.Value}; erwartet wird 2.");
            }

            var rulesetVersion = cursor.StringMember(rootObject, "$", "rulesetVersion");
            if (rulesetVersion is not null && rulesetVersion != PuzzleDefinition.RulesetVersionV1)
            {
                cursor.Fail("LVL-SCHEMA-RULESET-VERSION", "$.rulesetVersion", $"Unbekannte rulesetVersion '{rulesetVersion}'; erwartet wird '{PuzzleDefinition.RulesetVersionV1}'.");
            }

            string? puzzleId = null;
            var puzzleIdText = cursor.StringMember(rootObject, "$", "puzzleId");
            if (puzzleIdText is not null)
            {
                if (!PuzzleIdPattern.IsMatch(puzzleIdText))
                {
                    cursor.Fail("LVL-SCHEMA-PUZZLE-ID-FORMAT", "$.puzzleId", $"puzzleId '{puzzleIdText}' verletzt das vertragliche Muster.");
                }
                else
                {
                    puzzleId = puzzleIdText;
                }
            }

            var contentRevision = cursor.IntegerMember(rootObject, "$", "contentRevision", 1, null, "LVL-SCHEMA-CONTENT-REVISION");

            LevelV2Grid? grid = null;
            var gridObject = cursor.ObjectMember(rootObject, "$", "grid");
            if (gridObject is not null)
            {
                cursor.CheckKeys(gridObject, "$.grid", "width", "height");
                cursor.RequireKeys(gridObject, "$.grid", "width", "height");
                var width = cursor.IntegerMember(gridObject, "$.grid", "width", 2, 10, "LVL-SCHEMA-GRID-WIDTH");
                var height = cursor.IntegerMember(gridObject, "$.grid", "height", 2, 10, "LVL-SCHEMA-GRID-HEIGHT");
                if (width.HasValue && height.HasValue)
                {
                    grid = new LevelV2Grid((int)width.Value, (int)height.Value);
                }
            }

            LevelV2Endpoint? endpointA = null;
            LevelV2Endpoint? endpointB = null;
            var endpointsObject = cursor.ObjectMember(rootObject, "$", "endpoints");
            if (endpointsObject is not null)
            {
                cursor.CheckKeys(endpointsObject, "$.endpoints", "a", "b");
                cursor.RequireKeys(endpointsObject, "$.endpoints", "a", "b");
                endpointA = ReadEndpoint(cursor, endpointsObject, "a");
                endpointB = ReadEndpoint(cursor, endpointsObject, "b");
            }

            var rowCounts = cursor.IntegerArrayMember(rootObject, "$", "rowCounts", 2, 10, 0, 10, "LVL-SCHEMA-ROW-COUNTS");
            var columnCounts = cursor.IntegerArrayMember(rootObject, "$", "columnCounts", 2, 10, 0, 10, "LVL-SCHEMA-COLUMN-COUNTS");

            LevelV2Content? content = null;
            var contentObject = cursor.ObjectMember(rootObject, "$", "content");
            if (contentObject is not null)
            {
                cursor.CheckKeys(contentObject, "$.content", "season", "networkSection", "route", "position", "titleKey", "statusKey");
                cursor.RequireKeys(contentObject, "$.content", "season", "networkSection", "route", "position", "titleKey", "statusKey");
                var season = cursor.IntegerMember(contentObject, "$.content", "season", 1, null, "LVL-SCHEMA-CONTENT-FIELD");
                var networkSection = cursor.IntegerMember(contentObject, "$.content", "networkSection", 1, null, "LVL-SCHEMA-CONTENT-FIELD");
                var route = cursor.IntegerMember(contentObject, "$.content", "route", 1, null, "LVL-SCHEMA-CONTENT-FIELD");
                var position = cursor.IntegerMember(contentObject, "$.content", "position", 1, null, "LVL-SCHEMA-CONTENT-FIELD");
                var titleKey = cursor.LocalizationKeyMember(contentObject, "$.content", "titleKey");
                var statusKey = cursor.LocalizationKeyMember(contentObject, "$.content", "statusKey");
                if (season.HasValue && networkSection.HasValue && route.HasValue && position.HasValue && titleKey is not null && statusKey is not null)
                {
                    content = new LevelV2Content(season.Value, networkSection.Value, route.Value, position.Value, titleKey, statusKey);
                }
            }

            LevelV2Production? production = null;
            var productionObject = cursor.ObjectMember(rootObject, "$", "production");
            if (productionObject is not null)
            {
                cursor.CheckKeys(productionObject, "$.production", "entryDeductionKey", "focus", "chainDepth", "qualityNote", "timeClass", "starThresholdsSeconds");
                cursor.RequireKeys(productionObject, "$.production", "entryDeductionKey", "focus", "chainDepth", "qualityNote", "timeClass", "starThresholdsSeconds");
                var entryDeductionKey = cursor.LocalizationKeyMember(productionObject, "$.production", "entryDeductionKey");
                var focus = ReadFocus(cursor, productionObject);
                var chainDepth = cursor.IntegerMember(productionObject, "$.production", "chainDepth", 1, null, "LVL-SCHEMA-PRODUCTION-FIELD");
                var qualityNote = cursor.StringMember(productionObject, "$.production", "qualityNote", 1, "LVL-SCHEMA-PRODUCTION-FIELD");
                var timeClass = cursor.StringMember(productionObject, "$.production", "timeClass", 1, "LVL-SCHEMA-PRODUCTION-FIELD");
                var thresholds = ReadThresholds(cursor, productionObject);
                if (entryDeductionKey is not null && focus is not null && chainDepth.HasValue && qualityNote is not null && timeClass is not null)
                {
                    production = new LevelV2Production(entryDeductionKey, focus, chainDepth.Value, qualityNote, timeClass, thresholds);
                }
            }

            LevelV2Completion? completion = null;
            var completionObject = cursor.ObjectMember(rootObject, "$", "completion");
            if (completionObject is not null)
            {
                cursor.CheckKeys(completionObject, "$.completion", "mapSegmentId", "trainMomentId", "resultTextKey");
                cursor.RequireKeys(completionObject, "$.completion", "mapSegmentId", "trainMomentId", "resultTextKey");
                var mapSegmentId = cursor.StringMember(completionObject, "$.completion", "mapSegmentId", 1, "LVL-SCHEMA-COMPLETION-FIELD");
                var trainMomentId = cursor.StringMember(completionObject, "$.completion", "trainMomentId", 1, "LVL-SCHEMA-COMPLETION-FIELD");
                var resultTextKey = cursor.LocalizationKeyMember(completionObject, "$.completion", "resultTextKey");
                if (mapSegmentId is not null && trainMomentId is not null && resultTextKey is not null)
                {
                    completion = new LevelV2Completion(mapSegmentId, trainMomentId, resultTextKey);
                }
            }

            LevelV2Solution? solution = null;
            var solutionObject = cursor.ObjectMember(rootObject, "$", "solution");
            if (solutionObject is not null)
            {
                cursor.CheckKeys(solutionObject, "$.solution", "path");
                cursor.RequireKeys(solutionObject, "$.solution", "path");
                var path = ReadPath(cursor, solutionObject);
                if (path is not null)
                {
                    solution = new LevelV2Solution(path);
                }
            }

            LevelV2ProofRef? proofRef = null;
            var proofRefObject = cursor.ObjectMember(rootObject, "$", "proofRef");
            if (proofRefObject is not null)
            {
                cursor.CheckKeys(proofRefObject, "$.proofRef", "artifactId", "proofFormatVersion", "proofHash");
                cursor.RequireKeys(proofRefObject, "$.proofRef", "artifactId", "proofFormatVersion", "proofHash");
                string? artifactId = null;
                var artifactIdText = cursor.StringMember(proofRefObject, "$.proofRef", "artifactId");
                if (artifactIdText is not null)
                {
                    if (!ArtifactIdPattern.IsMatch(artifactIdText))
                    {
                        cursor.Fail("LVL-SCHEMA-ARTIFACT-ID", "$.proofRef.artifactId", $"artifactId '{artifactIdText}' verletzt das vertragliche Muster.");
                    }
                    else
                    {
                        artifactId = artifactIdText;
                    }
                }
                var proofFormatVersion = cursor.IntegerMember(proofRefObject, "$.proofRef", "proofFormatVersion");
                if (proofFormatVersion.HasValue && proofFormatVersion.Value != 1L)
                {
                    cursor.Fail("LVL-SCHEMA-PROOF-FORMAT-VERSION", "$.proofRef.proofFormatVersion", $"Unbekannte proofFormatVersion {proofFormatVersion.Value}; erwartet wird 1.");
                }
                var proofHash = ReadProfiledHash(cursor, proofRefObject, "proofHash", "$.proofRef.proofHash", "LVL-SCHEMA-HASH");
                if (artifactId is not null && proofFormatVersion.HasValue && proofHash.HasValue)
                {
                    proofRef = new LevelV2ProofRef(artifactId, proofFormatVersion.Value, proofHash.Value);
                }
            }

            if (cursor.HasErrors)
            {
                return null;
            }
            return new LevelV2Document(
                documentSchemaVersion!.Value,
                rulesetVersion!,
                puzzleId!,
                contentRevision!.Value,
                grid!,
                endpointA!,
                endpointB!,
                rowCounts!,
                columnCounts!,
                content!,
                production!,
                completion!,
                solution!,
                proofRef!,
                rootObject);
        }

        private static LevelV2Endpoint? ReadEndpoint(Cursor cursor, JsonValue.Object endpoints, string key)
        {
            var path = $"$.endpoints.{key}";
            var endpointObject = cursor.ObjectMember(endpoints, "$.endpoints", key);
            if (endpointObject is null)
            {
                return null;
            }
            cursor.CheckKeys(endpointObject, path, "side", "index");
            cursor.RequireKeys(endpointObject, path, "side", "index");
            Direction? side = null;
            var sideText = cursor.StringMember(endpointObject, path, "side");
            if (sideText is not null)
            {
                side = sideText switch
                {
                    "N" => Direction.N,
                    "E" => Direction.E,
                    "S" => Direction.S,
                    "W" => Direction.W,
                    _ => null,
                };
                if (side is null)
                {
                    cursor.Fail("LVL-SCHEMA-ENDPOINT-SIDE", $"{path}.side", $"Unbekannte Endpoint-Seite '{sideText}'.");
                }
            }
            var index = cursor.IntegerMember(endpointObject, path, "index", 0, 9, "LVL-SCHEMA-ENDPOINT-INDEX");
            if (side.HasValue && index.HasValue)
            {
                return new LevelV2Endpoint(side.Value, (int)index.Value);
            }
            return null;
        }

        private static IReadOnlyList<string>? ReadFocus(Cursor cursor, JsonValue.Object production)
        {
            const string path = "$.production.focus";
            if (!production.TryGet("focus", out var value))
            {
                return null;
            }
            if (value is not JsonValue.Array array)
            {
                cursor.Fail(cursor.TypeCode, path, "focus muss ein Array sein.");
                return null;
            }
            if (array.Items.Count < 1)
            {
                cursor.Fail("LVL-SCHEMA-FOCUS", path, "focus benötigt mindestens einen Wert.");
            }
            var seen = new HashSet<string>(System.StringComparer.Ordinal);
            var values = new List<string>();
            var valid = true;
            for (var i = 0; i < array.Items.Count; i++)
            {
                if (array.Items[i] is not JsonValue.String text)
                {
                    cursor.Fail(cursor.TypeCode, $"{path}[{i}]", "focus-Werte müssen Zeichenketten sein.");
                    valid = false;
                    continue;
                }
                if (!FocusValues.Contains(text.Value))
                {
                    cursor.Fail("LVL-SCHEMA-FOCUS", $"{path}[{i}]", $"Unbekannter focus-Wert '{text.Value}'.");
                    valid = false;
                    continue;
                }
                if (!seen.Add(text.Value))
                {
                    cursor.Fail("LVL-SCHEMA-FOCUS", $"{path}[{i}]", $"Doppelter focus-Wert '{text.Value}' (uniqueItems).");
                    valid = false;
                    continue;
                }
                values.Add(text.Value);
            }
            return valid && values.Count >= 1 ? values : null;
        }

        private static LevelV2StarThresholds? ReadThresholds(Cursor cursor, JsonValue.Object production)
        {
            const string path = "$.production.starThresholdsSeconds";
            if (!production.TryGet("starThresholdsSeconds", out var value))
            {
                return null;
            }
            if (value is JsonValue.Null)
            {
                return null;
            }
            if (value is not JsonValue.Object thresholdsObject)
            {
                cursor.Fail("LVL-SCHEMA-STAR-THRESHOLDS", path, "starThresholdsSeconds muss null oder ein Objekt sein (oneOf).");
                return null;
            }
            cursor.CheckKeys(thresholdsObject, path, "twoStars", "threeStars", "calibrationVersion");
            cursor.RequireKeys(thresholdsObject, path, "twoStars", "threeStars", "calibrationVersion");
            var twoStars = cursor.IntegerMember(thresholdsObject, path, "twoStars", 1, null, "LVL-SCHEMA-THRESHOLDS-FIELD");
            var threeStars = cursor.IntegerMember(thresholdsObject, path, "threeStars", 1, null, "LVL-SCHEMA-THRESHOLDS-FIELD");
            var calibrationVersion = cursor.StringMember(thresholdsObject, path, "calibrationVersion", 1, "LVL-SCHEMA-THRESHOLDS-FIELD");
            if (twoStars.HasValue && threeStars.HasValue && calibrationVersion is not null)
            {
                return new LevelV2StarThresholds(twoStars.Value, threeStars.Value, calibrationVersion);
            }
            return null;
        }

        private static IReadOnlyList<LevelV2PathCell>? ReadPath(Cursor cursor, JsonValue.Object solution)
        {
            const string path = "$.solution.path";
            if (!solution.TryGet("path", out var value))
            {
                return null;
            }
            if (value is not JsonValue.Array array)
            {
                cursor.Fail(cursor.TypeCode, path, "path muss ein Array sein.");
                return null;
            }
            if (array.Items.Count < 1)
            {
                cursor.Fail("LVL-SCHEMA-SOLUTION-FIELD", path, "path benötigt mindestens eine Zelle.");
            }
            var cells = new List<LevelV2PathCell>();
            var valid = true;
            for (var i = 0; i < array.Items.Count; i++)
            {
                var cellPath = $"{path}[{i}]";
                if (array.Items[i] is not JsonValue.Object cellObject)
                {
                    cursor.Fail(cursor.TypeCode, cellPath, "Pfadzellen müssen Objekte sein.");
                    valid = false;
                    continue;
                }
                cursor.CheckKeys(cellObject, cellPath, "x", "y", "track");
                cursor.RequireKeys(cellObject, cellPath, "x", "y", "track");
                var x = cursor.IntegerMember(cellObject, cellPath, "x", 0, 9, "LVL-SCHEMA-PATH-CELL");
                var y = cursor.IntegerMember(cellObject, cellPath, "y", 0, 9, "LVL-SCHEMA-PATH-CELL");
                TrackShape? track = null;
                var trackText = cursor.StringMember(cellObject, cellPath, "track");
                if (trackText is not null)
                {
                    track = trackText switch
                    {
                        "TRACK_NS" => TrackShape.NS,
                        "TRACK_EW" => TrackShape.EW,
                        "TRACK_NE" => TrackShape.NE,
                        "TRACK_ES" => TrackShape.ES,
                        "TRACK_SW" => TrackShape.SW,
                        "TRACK_WN" => TrackShape.WN,
                        _ => null,
                    };
                    if (track is null)
                    {
                        cursor.Fail("LVL-SCHEMA-PATH-CELL", $"{cellPath}.track", $"Unbekannte Gleisform '{trackText}'.");
                    }
                }
                if (x.HasValue && y.HasValue && track.HasValue)
                {
                    cells.Add(new LevelV2PathCell((int)x.Value, (int)y.Value, track.Value));
                }
                else
                {
                    valid = false;
                }
            }
            return valid && cells.Count >= 1 ? cells : null;
        }

        internal static ProfiledHash? ReadProfiledHash(Cursor cursor, JsonValue.Object owner, string key, string path, string code)
        {
            if (!owner.TryGet(key, out var value))
            {
                return null;
            }
            var hashObject = cursor.Object(value, path, cursor.TypeCode);
            if (hashObject is null)
            {
                return null;
            }
            cursor.CheckKeys(hashObject, path, "profile", "sha256");
            cursor.RequireKeys(hashObject, path, "profile", "sha256");
            var profile = cursor.StringMember(hashObject, path, "profile", 1, code);
            string? sha256 = null;
            var sha256Text = cursor.StringMember(hashObject, path, "sha256");
            if (sha256Text is not null)
            {
                if (!Sha256Pattern.IsMatch(sha256Text))
                {
                    cursor.Fail(code, $"{path}.sha256", "sha256 muss ein kleingeschriebener 64-stelliger Hexwert sein.");
                }
                else
                {
                    sha256 = sha256Text;
                }
            }
            if (profile is not null && sha256 is not null)
            {
                return new ProfiledHash(profile, sha256);
            }
            return null;
        }

        /// <summary>Lese- und Prüfwerkzeuge der Strukturprüfung mit Diagnosesammlung.</summary>
        internal sealed class Cursor
        {
            private readonly List<LevelDiagnostic> _diagnostics;
            private int _errors;

            internal Cursor(List<LevelDiagnostic> diagnostics, string typeCode, string additionalPropertyCode, string missingPropertyCode)
            {
                _diagnostics = diagnostics;
                TypeCode = typeCode;
                AdditionalPropertyCode = additionalPropertyCode;
                MissingPropertyCode = missingPropertyCode;
            }

            internal string TypeCode { get; }

            internal string AdditionalPropertyCode { get; }

            internal string MissingPropertyCode { get; }

            internal bool HasErrors => _errors > 0;

            internal void Fail(string code, string subject, string message)
            {
                _errors++;
                _diagnostics.Add(LevelDiagnostic.Error(code, subject, message));
            }

            internal JsonValue.Object? Object(JsonValue value, string path, string code)
            {
                if (value is JsonValue.Object obj)
                {
                    return obj;
                }
                Fail(code, path, "Objekt erwartet.");
                return null;
            }

            internal void CheckKeys(JsonValue.Object obj, string path, params string[] allowedKeys)
            {
                var allowed = new HashSet<string>(System.StringComparer.Ordinal);
                foreach (var key in allowedKeys)
                {
                    allowed.Add(key);
                }
                foreach (var pair in obj.Properties)
                {
                    if (!allowed.Contains(pair.Key))
                    {
                        Fail(AdditionalPropertyCode, $"{path}.{pair.Key}", $"Unbekannte Eigenschaft '{pair.Key}' (additionalProperties: false).");
                    }
                }
            }

            internal void RequireKeys(JsonValue.Object obj, string path, params string[] requiredKeys)
            {
                foreach (var key in requiredKeys)
                {
                    if (!obj.Contains(key))
                    {
                        Fail(MissingPropertyCode, path, $"Pflichteigenschaft '{key}' fehlt.");
                    }
                }
            }

            internal JsonValue.Object? ObjectMember(JsonValue.Object owner, string ownerPath, string key)
            {
                if (!owner.TryGet(key, out var value))
                {
                    return null;
                }
                return Object(value, $"{ownerPath}.{key}", TypeCode);
            }

            internal long? IntegerMember(JsonValue.Object owner, string ownerPath, string key, long minimum = long.MinValue, long? maximum = null, string? rangeCode = null)
            {
                if (!owner.TryGet(key, out var value))
                {
                    return null;
                }
                var path = $"{ownerPath}.{key}";
                if (value is not JsonValue.Integer integer)
                {
                    Fail(TypeCode, path, "Ganzzahl erwartet.");
                    return null;
                }
                if (integer.Value < minimum || (maximum.HasValue && integer.Value > maximum.Value))
                {
                    Fail(rangeCode ?? TypeCode, path, maximum.HasValue
                        ? $"Wert {integer.Value} außerhalb des Bereichs {minimum}..{maximum.Value}."
                        : $"Wert {integer.Value} kleiner als das Minimum {minimum}.");
                    return null;
                }
                return integer.Value;
            }

            internal string? StringMember(JsonValue.Object owner, string ownerPath, string key, int minLength = 0, string? lengthCode = null)
            {
                if (!owner.TryGet(key, out var value))
                {
                    return null;
                }
                var path = $"{ownerPath}.{key}";
                if (value is not JsonValue.String text)
                {
                    Fail(TypeCode, path, "Zeichenkette erwartet.");
                    return null;
                }
                if (text.Value.Length < minLength)
                {
                    Fail(lengthCode ?? TypeCode, path, $"Zeichenkette benötigt mindestens Länge {minLength}.");
                    return null;
                }
                return text.Value;
            }

            internal string? LocalizationKeyMember(JsonValue.Object owner, string ownerPath, string key)
            {
                var text = StringMember(owner, ownerPath, key);
                if (text is null)
                {
                    return null;
                }
                if (!LocalizationKeyPattern.IsMatch(text))
                {
                    Fail("LVL-SCHEMA-LOCALIZATION-KEY", $"{ownerPath}.{key}", $"Lokalisierungsschlüssel '{text}' verletzt das vertragliche Muster.");
                    return null;
                }
                return text;
            }

            internal IReadOnlyList<int>? IntegerArrayMember(JsonValue.Object owner, string ownerPath, string key, int minItems, int maxItems, int minValue, int maxValue, string code)
            {
                if (!owner.TryGet(key, out var value))
                {
                    return null;
                }
                var path = $"{ownerPath}.{key}";
                if (value is not JsonValue.Array array)
                {
                    Fail(TypeCode, path, "Array erwartet.");
                    return null;
                }
                if (array.Items.Count < minItems || array.Items.Count > maxItems)
                {
                    Fail(code, path, $"Listenlänge {array.Items.Count} außerhalb von {minItems}..{maxItems}.");
                }
                var values = new List<int>();
                var valid = true;
                for (var i = 0; i < array.Items.Count; i++)
                {
                    if (array.Items[i] is not JsonValue.Integer integer)
                    {
                        Fail(TypeCode, $"{path}[{i}]", "Ganzzahl erwartet.");
                        valid = false;
                        continue;
                    }
                    if (integer.Value < minValue || integer.Value > maxValue)
                    {
                        Fail(code, $"{path}[{i}]", $"Wert {integer.Value} außerhalb des Bereichs {minValue}..{maxValue}.");
                        valid = false;
                        continue;
                    }
                    values.Add((int)integer.Value);
                }
                return valid && values.Count >= minItems && values.Count <= maxItems ? values : null;
            }
        }
    }
}
