using System;

namespace STP.Infrastructure.Content
{
    /// <summary>Pipeline stage that produced a diagnostic (ARCHITECTURE/CONTENT_PIPELINE.md Abschnitt 5).</summary>
    public enum LevelDiagnosticStage
    {
        Parse,
        Schema,
        DomainMap,
        Semantic,
        Hash,
        Migration,
        Import
    }

    /// <summary>
    /// A stable, fail-closed content diagnostic. <see cref="Code"/> and <see cref="Path"/> are the stable
    /// contract; <see cref="Message"/> is informational and may be refined without a format change.
    /// </summary>
    public sealed class LevelDiagnostic : IEquatable<LevelDiagnostic>
    {
        public LevelDiagnosticStage Stage { get; }
        public string Code { get; }
        /// <summary>JSON-Pointer-like location (RFC 6901 style, without escaping), empty for the document root.</summary>
        public string Path { get; }
        public string Message { get; }

        public LevelDiagnostic(LevelDiagnosticStage stage, string code, string path, string message)
        {
            Stage = stage;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public bool Equals(LevelDiagnostic? other) => other != null && Stage == other.Stage && Code == other.Code && Path == other.Path && Message == other.Message;
        public override bool Equals(object? obj) => Equals(obj as LevelDiagnostic);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)Stage;
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(Code);
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(Path);
                return hash * 31 + StringComparer.Ordinal.GetHashCode(Message);
            }
        }
        public override string ToString() => Code + " @ " + (Path.Length == 0 ? "/" : Path) + ": " + Message;
    }

    /// <summary>All stable diagnostic codes of the Level-v2 foundation. Families follow LEVEL_DATA_FORMAT.md Abschnitt 8.</summary>
    public static class LevelDiagnosticCodes
    {
        // Parse stage: untrusted bytes to JSON value.
        public const string ParseEmpty = "LVL-PARSE-EMPTY";
        public const string ParseSize = "LVL-PARSE-SIZE";
        public const string ParseBom = "LVL-PARSE-BOM";
        public const string ParseUtf8 = "LVL-PARSE-UTF8";
        public const string ParseSyntax = "LVL-PARSE-SYNTAX";
        public const string ParseComment = "LVL-PARSE-COMMENT";
        public const string ParseDuplicateKey = "LVL-PARSE-DUPLICATE-KEY";
        public const string ParseFloat = "LVL-PARSE-FLOAT";
        public const string ParseNumberRange = "LVL-PARSE-NUMBER-RANGE";
        public const string ParseNegativeZero = "LVL-PARSE-NEGATIVE-ZERO";
        public const string ParseSurrogate = "LVL-PARSE-SURROGATE";
        public const string ParseEscape = "LVL-PARSE-ESCAPE";
        public const string ParseControlCharacter = "LVL-PARSE-CONTROL-CHARACTER";
        public const string ParseTrailingContent = "LVL-PARSE-TRAILING-CONTENT";
        public const string ParseDepth = "LVL-PARSE-DEPTH";
        public const string ParseStringLength = "LVL-PARSE-STRING-LENGTH";
        public const string ParseArrayLength = "LVL-PARSE-ARRAY-LENGTH";
        public const string ParseObjectMembers = "LVL-PARSE-OBJECT-MEMBERS";

        // Version selection and closed constants.
        public const string VersionUnknown = "LVL-VERSION-UNKNOWN";
        public const string VersionRulesetUnknown = "LVL-VERSION-RULESET-UNKNOWN";
        public const string VersionProofFormatUnknown = "LVL-VERSION-PROOF-FORMAT-UNKNOWN";

        // Schema stage: closed structure, types, enums, patterns and local bounds.
        public const string SchemaType = "LVL-SCHEMA-TYPE";
        public const string SchemaRequired = "LVL-SCHEMA-REQUIRED";
        public const string SchemaUnknownProperty = "LVL-SCHEMA-UNKNOWN-PROPERTY";
        public const string SchemaConst = "LVL-SCHEMA-CONST";
        public const string SchemaEnum = "LVL-SCHEMA-ENUM";
        public const string SchemaPattern = "LVL-SCHEMA-PATTERN";
        public const string SchemaRange = "LVL-SCHEMA-RANGE";
        public const string SchemaLength = "LVL-SCHEMA-LENGTH";
        public const string SchemaUnique = "LVL-SCHEMA-UNIQUE";

        // Domain mapping stage (the integrated Domain stays authoritative).
        public const string DomainInvalidDefinition = "LVL-DOMAIN-INVALID-DEFINITION";
        public const string DomainUnsupportedRuleset = "LVL-DOMAIN-UNSUPPORTED-RULESET";
        public const string DomainSolutionNotCompleted = "LVL-DOMAIN-SOLUTION-NOT-COMPLETED";
        public const string GridRowCountLength = "LVL-GRID-ROWCOUNT-LENGTH";
        public const string GridColumnCountLength = "LVL-GRID-COLUMNCOUNT-LENGTH";
        public const string GridRowCountValue = "LVL-GRID-ROWCOUNT-VALUE";
        public const string GridColumnCountValue = "LVL-GRID-COLUMNCOUNT-VALUE";
        public const string GridCountSum = "LVL-GRID-COUNT-SUM";
        public const string EndpointRange = "LVL-ENDPOINT-RANGE";
        public const string EndpointIdentical = "LVL-ENDPOINT-IDENTICAL";
        public const string EndpointCountZero = "LVL-ENDPOINT-COUNT-ZERO";

        // Semantic stage.
        public const string IdContentMismatch = "LVL-ID-CONTENT-MISMATCH";
        public const string IdSeasonOneSection = "LVL-ID-S1-SECTION-RANGE";
        public const string IdSeasonOneRoute = "LVL-ID-S1-ROUTE-RANGE";
        public const string IdSeasonOnePosition = "LVL-ID-S1-POSITION-RANGE";
        public const string PathOutOfBounds = "LVL-PATH-OUT-OF-BOUNDS";
        public const string PathDuplicate = "LVL-PATH-DUPLICATE";
        public const string PathNotAdjacent = "LVL-PATH-NOT-ADJACENT";
        public const string PathPortMismatch = "LVL-PATH-PORT-MISMATCH";
        public const string EndpointACell = "LVL-ENDPOINT-A-CELL";
        public const string EndpointAShape = "LVL-ENDPOINT-A-SHAPE";
        public const string EndpointBCell = "LVL-ENDPOINT-B-CELL";
        public const string EndpointBShape = "LVL-ENDPOINT-B-SHAPE";
        public const string RuleOpenConnection = "LVL-RULE-OPEN-CONNECTION";
        public const string RuleLoop = "LVL-RULE-LOOP";
        public const string CountRowMismatch = "LVL-COUNT-ROW-MISMATCH";
        public const string CountColumnMismatch = "LVL-COUNT-COLUMN-MISMATCH";
        public const string TimeOrder = "LVL-TIME-ORDER";

        // Hash contracts.
        public const string HashProfileUnknown = "LVL-HASH-PROFILE-UNKNOWN";
        public const string HashMismatch = "LVL-HASH-MISMATCH";
        public const string JcsIntegerRange = "LVL-JCS-INTEGER-RANGE";
        public const string JcsSurrogate = "LVL-JCS-SURROGATE";
        public const string JcsInvalidInput = "LVL-JCS-INVALID-INPUT";

        // Migration and import gate.
        public const string MigrationNeedsEditorialDecision = "LVL_MIGRATION_NEEDS_EDITORIAL_DECISION";
        public const string ImportProofGateMissing = "LVL-IMPORT-PROOF-GATE-MISSING";
    }
}
