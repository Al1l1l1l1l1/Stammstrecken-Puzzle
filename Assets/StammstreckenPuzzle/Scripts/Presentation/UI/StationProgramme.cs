using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace STP.Presentation.UI
{
    // JsonUtility reads fields and arrays. The keyed depot dialogue uses the same JSON string decoder.
    internal sealed class StationProgramme
    {
        [Serializable] internal sealed class Mail { public string id="",trigger="",from="",subject="",ps=""; public string[] body=Array.Empty<string>(); }
        [Serializable] internal sealed class Gag { public string id="",scene="",kind="",line="",sound=""; public string[] objects=Array.Empty<string>(); }
        [Serializable] internal sealed class Rank { public int minStars=0;public string title=""; }
        [Serializable] internal sealed class Jubilee { public string condition="",text=""; }
        [Serializable] internal sealed class Tutorial { public int step=0;public string text=""; }
        [Serializable] internal sealed class Sign { public string title="",line="",endless="",note=""; }
        [Serializable] internal sealed class Proof { public string statement="";public string[] remarks=Array.Empty<string>(); }
        [Serializable] internal sealed class Season { public string greyedNext="",endless="",revision=""; }
        [Serializable] internal sealed class Lines
        {
            public Rank[] ranks=Array.Empty<Rank>();public Jubilee[] jubilees=Array.Empty<Jubilee>();
            public string[] loading=Array.Empty<string>(),arrivalAnnouncements=Array.Empty<string>(),boardMessagesRare=Array.Empty<string>();
            public Tutorial[] tutorial=Array.Empty<Tutorial>();public Sign deadlineSign=new Sign();
            public Proof proofOfPunctuality=new Proof();public Season seasonEnd=new Season();
        }
        [Serializable] private sealed class Letters { public Mail[] mails=Array.Empty<Mail>(); }
        [Serializable] private sealed class Directions { public Gag[] gags=Array.Empty<Gag>(); }
        internal readonly Mail[] Mails;internal readonly Gag[] Gags;internal readonly Lines Voice;
        internal readonly Dictionary<string,string> Depot=new Dictionary<string,string>(StringComparer.Ordinal);
        internal StationProgramme(StationBulletins assets)
        {
            Mails=JsonUtility.FromJson<Letters>(assets.mails.text).mails;
            Gags=JsonUtility.FromJson<Directions>(assets.gags.text).gags;
            Voice=JsonUtility.FromJson<Lines>(assets.voices.text);
            var dialogue=JObject.Parse(assets.voices.text)["depotLines"] as JObject;
            if(dialogue!=null)foreach(var line in dialogue.Properties())Depot[line.Name]=line.Value.ToString();
        }
    }
}
