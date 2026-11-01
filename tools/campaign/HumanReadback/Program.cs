using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STP.Tools.HumanReadback;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var options=new Dictionary<string,string>(StringComparer.Ordinal);for(int i=0;i<args.Length;i+=2){SourceBundle.Need(i+1<args.Length&&new[]{"--source-root","--packs","--public-catalog","--public-bindings","--source-bindings","--exports","--out","--self-test"}.Contains(args[i])&&options.TryAdd(args[i],args[i+1]),"READBACK_ARGUMENTS");}
            string Need(string key)=>options.TryGetValue(key,out var value)?value:throw new ArgumentException("Missing "+key);
            var sources=new SourceBundle(Need("--source-root"),Need("--packs"),Need("--public-catalog"),Need("--public-bindings"),Need("--source-bindings"));
            if(options.TryGetValue("--self-test",out var scratch))return SelfTests.Run(sources,scratch);
            string exports=Path.GetFullPath(Need("--exports"));SourceBundle.CheckPath(exports);SourceBundle.Need(Directory.Exists(exports),"READBACK_EXPORT_DIRECTORY_MISSING");var paths=Directory.GetFiles(exports,"*.json",SearchOption.TopDirectoryOnly).OrderBy(p=>p,StringComparer.Ordinal).ToArray();SourceBundle.Need(paths.Length<=512,"READBACK_EXPORT_FILE_LIMIT");var readback=new Readback(sources);foreach(var path in paths)readback.Add(path,SourceBundle.Read(path));readback.ClassifyDuplicates();readback.Write(Need("--out"));Console.WriteLine($"Original files: {readback.Inputs.Count}; observations: {readback.Rows.Count}; unattested unassisted first-contact candidates: {readback.Rows.Count(r=>r.Reasons.Count==0)}; rejected files: {readback.Inputs.Count(i=>i.Rejection!=null)}. No Human/Star/Profile approval.");return readback.Inputs.Any(i=>i.Rejection!=null)?2:0;
        }
        catch(Exception e){Console.Error.WriteLine("Human readback stopped: "+e.Message);return 1;}
    }
}
