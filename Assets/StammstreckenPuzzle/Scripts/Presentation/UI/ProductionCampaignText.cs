using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace STP.Presentation.UI
{
    /// <summary>Dedicated Unity StringTable for the exact 720 player-facing campaign strings.</summary>
    public sealed class ProductionCampaignText:IDisposable
    {
        private readonly StringTable table;
        private readonly SharedTableData shared;
        private readonly Locale locale;
        private bool disposed;
        /// <summary>Validates canonical ordering and explicit draft opt-in before constructing its own table.</summary>
        public ProductionCampaignText(TextAsset source,bool allowInternalDraft=false)
        {
            if(source==null)throw new ArgumentNullException(nameof(source));var data=JsonUtility.FromJson<Document>(source.text);
            if(data==null||data.schemaVersion!=1||data.locale!="de-DE"||data.entries==null||data.entries.Length!=240||
                !Regex.IsMatch(data.sourceSha256??string.Empty,"^[0-9a-f]{64}$",RegexOptions.CultureInvariant)||
                !(data.approvalStatus=="PRODUCT_APPROVED"||allowInternalDraft&&data.approvalStatus=="DRAFT"))throw new ArgumentException("CAMPAIGN_TEXT_SOURCE_INVALID");
            for(var i=0;i<240;i++)
            {
                var entry=data.entries[i];if(entry==null||entry.puzzleId!=PuzzleId(i)||!Plain(entry.title)||!Plain(entry.status)||!Plain(entry.result))throw new ArgumentException("CAMPAIGN_TEXT_ENTRY_INVALID");
            }
            locale=Locale.CreateLocale("de-DE");shared=ScriptableObject.CreateInstance<SharedTableData>();shared.TableCollectionName="stp.campaign.player";
            table=ScriptableObject.CreateInstance<StringTable>();table.SharedData=shared;table.LocaleIdentifier=locale.Identifier;
            for(var i=0;i<240;i++){Add(i,"title",data.entries[i].title);Add(i,"status",data.entries[i].status);Add(i,"result",data.entries[i].result);}
        }
        /// <summary>Resolves only title, contextual status and result; internal deduction entries are never accepted.</summary>
        public ProductionPuzzleDisplay Resolve(int index,string section,string route)
        {
            if(disposed)throw new ObjectDisposedException(nameof(ProductionCampaignText));if(index<0||index>=240)throw new ArgumentOutOfRangeException(nameof(index));
            return new ProductionPuzzleDisplay(section,route,Get(index,"title"),Get(index,"status"),Get(index,"result"));
        }
        /// <summary>Disposes only this table's owned Unity objects.</summary>
        public void Dispose(){if(disposed)return;disposed=true;Destroy(table);Destroy(shared);Destroy(locale);}
        private void Add(int index,string field,string value){var key=PuzzleId(index)+"."+field;shared.AddKey(key);table.AddEntry(key,value);}
        private string Get(int index,string field)=>table.GetEntry(PuzzleId(index)+"."+field)!.LocalizedValue;
        private static bool Plain(string value)=>!string.IsNullOrWhiteSpace(value)&&value.IndexOf('{')<0&&value.IndexOf('}')<0;
        private static string PuzzleId(int i)=>string.Format(CultureInfo.InvariantCulture,"S1-{0:00}-{1:00}-{2:00}",i/48+1,i%48/12+1,i%12+1);
        private static void Destroy(UnityEngine.Object value){if(UnityEngine.Application.isPlaying)UnityEngine.Object.Destroy(value);else UnityEngine.Object.DestroyImmediate(value);}
        [Serializable]private sealed class Document{public int schemaVersion=0;public string locale=string.Empty;public string approvalStatus=string.Empty;public string sourceSha256=string.Empty;public Entry[] entries=Array.Empty<Entry>();}
        [Serializable]private sealed class Entry{public string puzzleId=string.Empty;public string title=string.Empty;public string status=string.Empty;public string result=string.Empty;}
    }
}
