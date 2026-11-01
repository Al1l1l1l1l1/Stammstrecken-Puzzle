using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Infrastructure.Content;

namespace STP.Tools.ProductionImport;

internal static class J
{
    internal static void Need(bool ok,string code){if(!ok)throw new ArgumentException(code);}
    internal static JsonValue Parse(byte[] bytes){var p=StrictJsonParser.Parse(bytes,new JsonParserLimits(16777216,64,1048576,65536,256));Need(p.Succeeded,"IMPORT_JSON:"+p.Error?.Code);return p.Value!;}
    internal static void Fields(JsonValue v,params string[] keys)=>Need(v.Kind==JsonKind.Object&&v.Members.Count==keys.Length&&v.Members.All(m=>keys.Contains(m.Name,StringComparer.Ordinal)),"IMPORT_CLOSED_FIELDS");
    internal static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new ArgumentException("IMPORT_MISSING:"+key);
    internal static string S(JsonValue v,string key)=>Text(Get(v,key));
    internal static string Text(JsonValue v){Need(v.Kind==JsonKind.String&&!string.IsNullOrWhiteSpace(v.StringValue),"IMPORT_STRING");return v.StringValue;}
    internal static long N(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.Integer,"IMPORT_INTEGER:"+key);return x.IntegerValue;}
    internal static IReadOnlyList<JsonValue> A(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.Array,"IMPORT_ARRAY:"+key);return x.Items;}
    internal static string H(JsonValue v,string key){string s=S(v,key);Need(Regex.IsMatch(s,"^[a-f0-9]{64}$"),"IMPORT_HASH");return s;}
    internal static JsonValue Str(string s)=>JsonValue.CreateString(s);
    internal static JsonValue Num(long n)=>JsonValue.CreateInteger(n);
    internal static JsonValue Arr(IEnumerable<JsonValue> v)=>JsonValue.CreateArray(v);
    internal static JsonValue Obj(params(string key,JsonValue value)[] v)=>JsonValue.CreateObject(v.Select(x=>new JsonMember(x.key,x.value)));
    internal static JsonValue Put(JsonValue v,string key,JsonValue value)=>JsonValue.CreateObject(v.Members.Where(m=>m.Name!=key).Concat(new[]{new JsonMember(key,value)}));
    internal static byte[] Bytes(JsonValue v)=>JcsSerializer.SerializeOrThrow(v);
    internal static string Hash(JsonValue v)=>LevelHashing.HashProjection(v);
    internal static string Raw(byte[] v)=>LevelHashing.Sha256Hex(v);
}

/// <summary>Read-once bounded original-byte snapshot. Neither a request nor an output can escape its explicit source root.</summary>
internal sealed class Sources
{
    internal string Root{get;}
    internal SortedDictionary<string,byte[]> Originals{get;}=new(StringComparer.Ordinal);
    internal List<JsonValue> HumanSourceBindings{get;}=new();
    internal Sources(string root){Root=Path.GetFullPath(root);CheckNoLinks(Root);J.Need(Directory.Exists(Root),"IMPORT_ROOT_MISSING");}
    internal string Inside(string relative)
    {
        J.Need(relative.Length<=512&&!Path.IsPathRooted(relative)&&!relative.Contains('\\')&&!relative.Contains(':')&&!relative.Split('/').Any(p=>p is "" or "." or ".."),"IMPORT_SOURCE_PATH");
        string full=Path.GetFullPath(Path.Combine(Root,relative.Replace('/',Path.DirectorySeparatorChar)));
        J.Need(full.StartsWith(Root.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),"IMPORT_SOURCE_ESCAPE");CheckNoLinks(full);return full;
    }
    internal byte[] Read(string relative)
    {
        if(Originals.TryGetValue(relative,out var existing))return(byte[])existing.Clone();
        string full=Inside(relative);var info=new FileInfo(full);J.Need(info.Exists&&info.Length>0&&info.Length<=16777216,"IMPORT_SOURCE_SIZE_OR_MISSING:"+relative);
        J.Need(Originals.Count<8192&&Originals.Values.Sum(b=>(long)b.Length)+info.Length<=536870912,"IMPORT_SOURCE_SNAPSHOT_BUDGET");byte[] b=File.ReadAllBytes(full);Originals.Add(relative,b);return(byte[])b.Clone();
    }
    internal byte[] Reference(JsonValue v){J.Fields(v,"path","rawSha256");byte[] b=Read(J.S(v,"path"));J.Need(J.Raw(b)==J.H(v,"rawSha256"),"IMPORT_RAW_SOURCE_MISMATCH:"+J.S(v,"path"));return b;}
    internal JsonValue ReadJson(string p)=>J.Parse(Read(p));
    internal JsonValue Lock()=>J.Arr(Originals.Select(x=>J.Obj(("path",J.Str(x.Key)),("rawSha256",J.Str(J.Raw(x.Value))))));
    internal void VerifyUnchanged(){foreach(var x in Originals)J.Need(File.ReadAllBytes(Inside(x.Key)).SequenceEqual(x.Value),"IMPORT_SOURCE_CHANGED_DURING_RUN:"+x.Key);}
    internal static void CheckNoLinks(string full){for(string? p=Path.GetFullPath(full);p!=null;p=Path.GetDirectoryName(p))if((File.Exists(p)||Directory.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new ArgumentException("IMPORT_REPARSE_FORBIDDEN");}
}
