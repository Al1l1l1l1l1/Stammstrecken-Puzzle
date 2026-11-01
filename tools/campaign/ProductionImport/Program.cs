using System;
using System.Collections.Generic;
using System.IO;

namespace STP.Tools.ProductionImport;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var options=new Dictionary<string,string>(StringComparer.Ordinal);for(int i=0;i<args.Length;i+=2)J.Need(i+1<args.Length&&Array.IndexOf(new[]{"--source-root","--request","--out","--trusted-approval-lock","--trusted-approval-sha256","--self-test"},args[i])>=0&&options.TryAdd(args[i],args[i+1]),"IMPORT_CLI_ARGUMENTS");
            string Need(string key)=>options.TryGetValue(key,out var s)?s:throw new ArgumentException("Missing "+key);
            if(options.TryGetValue("--self-test",out var scratch))return ImportTests.Run(Need("--source-root"),scratch);
            var importer=new ProductionImporter(Need("--source-root"));var result=importer.Validate(Need("--request"),options.GetValueOrDefault("--trusted-approval-lock"),options.GetValueOrDefault("--trusted-approval-sha256"));result.Write(Need("--out"));
            Console.WriteLine("PASS "+result.Use+": 240 typed levels/fresh solver proofs, exact catalog/quality/calibration/cross-reference/release bindings; no store, device or Human acceptance claim.");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(Array.IndexOf(args,"--self-test")>=0?e.ToString():"Production import stopped: "+e.Message);return 1;}
    }
}
