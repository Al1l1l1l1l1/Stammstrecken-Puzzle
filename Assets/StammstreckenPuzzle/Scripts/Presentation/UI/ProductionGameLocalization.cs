using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace STP.Presentation.UI
{
    /// <summary>Explicit product-local Unity StringTable with validated typed formatting and no global locale mutation.</summary>
    public sealed class ProductionGameLocalization : IDisposable
    {
        private readonly StringTable table;
        private readonly SharedTableData shared;
        private readonly Locale locale;
        private readonly Dictionary<string,string[]> arguments = new Dictionary<string,string[]>(StringComparer.Ordinal);
        private readonly CultureInfo culture=CultureInfo.GetCultureInfo("de-DE");
        private bool disposed;
        /// <summary>Constructs a dedicated product table from the injected immutable text source.</summary>
        public ProductionGameLocalization(TextAsset source)
        {
            if(source==null)throw new ArgumentNullException(nameof(source));
            var document=JsonUtility.FromJson<Document>(source.text);
            if(document==null||document.locale!="de-DE"||document.entries==null||document.entries.Length==0)throw new ArgumentException("PRODUCT_LOCALE_INVALID");
            foreach(var entry in document.entries)
            {
                if(entry==null||string.IsNullOrWhiteSpace(entry.key)||!entry.key.StartsWith("product.",StringComparison.Ordinal)||string.IsNullOrWhiteSpace(entry.value)||entry.argumentTypes==null||arguments.ContainsKey(entry.key))throw new ArgumentException("PRODUCT_LOCALE_ENTRY_INVALID");
                var used=new bool[entry.argumentTypes.Length];
                foreach(var kind in entry.argumentTypes)if(kind!="string"&&kind!="integer")throw new ArgumentException("PRODUCT_LOCALE_TYPE_INVALID");
                const string pattern=@"\{([0-9]+)(?::(00|N0))?\}";
                foreach(Match match in Regex.Matches(entry.value,pattern,RegexOptions.CultureInvariant))
                {
                    if(!int.TryParse(match.Groups[1].Value,out var i)||i<0||i>=used.Length||(match.Groups[2].Success&&entry.argumentTypes[i]=="string"))throw new ArgumentException("PRODUCT_LOCALE_FORMAT_INVALID");used[i]=true;
                }
                var rest=Regex.Replace(entry.value,pattern,string.Empty,RegexOptions.CultureInvariant);
                if(rest.IndexOf('{')>=0||rest.IndexOf('}')>=0)throw new ArgumentException("PRODUCT_LOCALE_FORMAT_INVALID");
                foreach(var present in used)if(!present)throw new ArgumentException("PRODUCT_LOCALE_ARGUMENT_UNUSED");
                arguments.Add(entry.key,entry.argumentTypes);
            }
            locale=Locale.CreateLocale("de-DE");shared=ScriptableObject.CreateInstance<SharedTableData>();shared.TableCollectionName="stp.product";
            table=ScriptableObject.CreateInstance<StringTable>();table.SharedData=shared;table.LocaleIdentifier=locale.Identifier;
            for(var i=0;i<document.entries.Length;i++){var e=document.entries[i];shared.AddKey(e.key,i+1);table.AddEntry(i+1,e.value);}
        }
        /// <summary>Resolves an own key, rejecting missing values and mismatched argument contracts.</summary>
        public string Resolve(string suffix,params object[] values)
        {
            if(disposed)throw new ObjectDisposedException(nameof(ProductionGameLocalization));
            var key="product."+suffix;
            if(!arguments.TryGetValue(key,out var contract))throw new InvalidOperationException("PRODUCT_LOCALE_KEY_MISSING:"+key);
            if(values==null||values.Length!=contract.Length)throw new ArgumentException("PRODUCT_LOCALE_ARGUMENT_COUNT:"+key);
            for(var i=0;i<values.Length;i++)
            {
                var integer=values[i] is int||values[i] is long||values[i] is uint||values[i] is byte||values[i] is short;
                if(contract[i]=="string"?!(values[i] is string):!integer)throw new ArgumentException("PRODUCT_LOCALE_ARGUMENT_TYPE:"+key);
            }
            return table.GetEntry(key)!.GetLocalizedString(culture,values,null);
        }
        /// <summary>Releases only this table's objects, safely on repeated calls.</summary>
        public void Dispose(){if(disposed)return;disposed=true;Destroy(table);Destroy(shared);Destroy(locale);}
        private static void Destroy(UnityEngine.Object value){if(UnityEngine.Application.isPlaying)UnityEngine.Object.Destroy(value);else UnityEngine.Object.DestroyImmediate(value);}
        [Serializable]private sealed class Document{public string locale=string.Empty;public Entry[] entries=Array.Empty<Entry>();}
        [Serializable]private sealed class Entry{public string key=string.Empty;public string value=string.Empty;public string[] argumentTypes=Array.Empty<string>();}
    }
}
