using System;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Reuses the existing strict parser and integer-only RFC8785 serializer through an inner byte port.</summary>
    public sealed class CanonicalJsonAdapter : ICanonicalJson
    {
        /// <summary>Canonicalizes bounded untrusted bytes; no domain projection or hash is added by this adapter.</summary>
        CanonicalJsonResult ICanonicalJson.Canonicalize(byte[] utf8, JsonResourceLimits limits)
        {
            if (limits == null) return CanonicalJsonResult.Failure("JSON_LIMITS_REQUIRED");
            var bytes = CanonicalizeBytes(utf8, limits.MaxBytes, limits.MaxDepth, limits.MaxStringLength,
                limits.MaxArrayLength, limits.MaxObjectMembers, out var error);
            return bytes == null ? CanonicalJsonResult.Failure(error!) : CanonicalJsonResult.Success(bytes);
        }

        /// <summary>Same execution path exposed with primitive types for the existing Content contract-test assembly.</summary>
        public byte[]? CanonicalizeBytes(byte[] utf8, int maxBytes, int maxDepth, int maxStringLength,
            int maxArrayLength, int maxObjectMembers, out string? error)
        {
            error = null;
            if (maxBytes < 1 || maxDepth < 1 || maxStringLength < 1 || maxArrayLength < 1 || maxObjectMembers < 1)
            { error = "JSON_LIMITS_INVALID"; return null; }
            // Parser never mutates the input; serializer constructs a fresh bounded output.
            var parsed = StrictJsonParser.Parse(utf8, new JsonParserLimits(maxBytes, maxDepth, maxStringLength, maxArrayLength, maxObjectMembers));
            if (parsed.Value == null) { error = parsed.Error!.Code; return null; }
            var canonical = JcsSerializer.Serialize(parsed.Value);
            if (canonical.Utf8 == null) { error = canonical.Error!.Code; return null; }
            return (byte[])canonical.Utf8.Clone();
        }
    }
}
