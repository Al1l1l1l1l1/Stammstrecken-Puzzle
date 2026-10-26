using System;
using System.Collections.Generic;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Reuses the existing strict parser and integer-only RFC8785 serializer through an inner byte port.</summary>
    public sealed class CanonicalJsonAdapter : ICanonicalJson
    {
        // Only pure strict-JSON byte results are retained. No typed state, validation,
        // hash, file observation or persistence acknowledgement is retained here.
        private const int MemoCountLimit=16,MemoByteLimit=8*1024*1024,MemoInputMin=4096,MemoItemByteLimit=2*1024*1024;
        private readonly object memoGate=new object();
        private readonly Queue<CanonicalMemo> memo=new Queue<CanonicalMemo>();
        private int memoBytes;
        private sealed class CanonicalMemo
        {
            internal CanonicalMemo(byte[] input,byte[] output,int maxBytes,int maxDepth,int maxStringLength,int maxArrayLength,int maxObjectMembers)
            {Input=input;Output=output;MaxBytes=maxBytes;MaxDepth=maxDepth;MaxStringLength=maxStringLength;MaxArrayLength=maxArrayLength;MaxObjectMembers=maxObjectMembers;}
            internal readonly byte[] Input,Output;
            internal readonly int MaxBytes,MaxDepth,MaxStringLength,MaxArrayLength,MaxObjectMembers;
            internal bool Matches(byte[] input,int maxBytes,int maxDepth,int maxStringLength,int maxArrayLength,int maxObjectMembers)
            {
                if(MaxBytes!=maxBytes||MaxDepth!=maxDepth||MaxStringLength!=maxStringLength||MaxArrayLength!=maxArrayLength||MaxObjectMembers!=maxObjectMembers||Input.Length!=input.Length)return false;
                return Input.AsSpan().SequenceEqual(input);
            }
        }
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
            // Tiny and over-budget inputs use the original parser path. Cache eligibility
            // never changes parser limits or adds a document rejection.
            bool eligible=utf8!=null&&utf8.Length>=MemoInputMin&&utf8.Length<=MemoItemByteLimit/2&&utf8.Length<=maxBytes;
            // This one private snapshot is both the key and the exact miss parser input.
            byte[]? input=eligible?(byte[])utf8!.Clone():utf8;
            if(eligible)
                lock(memoGate)foreach(var hit in memo)
                    if(hit.Matches(input!,maxBytes,maxDepth,maxStringLength,maxArrayLength,maxObjectMembers))return (byte[])hit.Output.Clone();
            var parsed = StrictJsonParser.Parse(input, new JsonParserLimits(maxBytes, maxDepth, maxStringLength, maxArrayLength, maxObjectMembers));
            if (parsed.Value == null) { error = parsed.Error!.Code; return null; }
            var canonical = JcsSerializer.Serialize(parsed.Value);
            if (canonical.Utf8 == null) { error = canonical.Error!.Code; return null; }
            if(eligible&&canonical.Utf8.Length<=MemoItemByteLimit-input!.Length)
            {
                // Parsing stays outside the gate. A racing successful miss can only
                // reuse a second fully equal input+limits result, never a hash verdict.
                lock(memoGate)
                {
                    foreach(var hit in memo)
                        if(hit.Matches(input!,maxBytes,maxDepth,maxStringLength,maxArrayLength,maxObjectMembers))return (byte[])hit.Output.Clone();
                    int retained=input!.Length+canonical.Utf8.Length;
                    while(memo.Count>=MemoCountLimit||memoBytes+retained>MemoByteLimit)
                    {var oldest=memo.Dequeue();memoBytes-=oldest.Input.Length+oldest.Output.Length;}
                    memo.Enqueue(new CanonicalMemo(input!,canonical.Utf8,maxBytes,maxDepth,maxStringLength,maxArrayLength,maxObjectMembers));memoBytes+=retained;
                }
            }
            return (byte[])canonical.Utf8.Clone();
        }
    }
}
