using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Twenty-nine individually recognizable original 3D cosmetic illustrations, sharing a coherent construction kit.</summary>
    public sealed class RailwayCosmeticElement:VisualElement
    {
        private readonly string itemId;
        private static readonly string[] Items={"train-stammrot-default","obj-wartebank-ohne-anschluss","obj-wegweiser-vielleicht","obj-absperrung-laufzeit-offen","obj-anzeiger-in-pruefung","livery-betriebsrot","livery-baustellengelb","obj-bahnsteig-provisorisch","obj-tunnelportal-wartung","obj-sev-bucht-geplant","train-ersatzgarnitur","train-anschlusswunder","obj-signal-freigegeben","obj-tafel-trotzdem-weiter","train-u-bahn-muenchen","train-doppelstock-pendler","train-dampflok-sonderfahrt","train-rangierlok-notbetrieb","livery-pendlerblau","livery-ersatzgruen","livery-umleitungsorange","livery-sonderviolett","livery-bummlertuerkis","livery-nachtanthrazit","livery-kulanzrosa","livery-frischweiss","wagon-grossraum-sitzplatz-optional","wagon-bordbistro-kaffeemaschine-defekt","wagon-fahrradabteil-ueberfuellt"};
        /// <summary>Creates the renderer for one exact supported canonical item; unknown IDs fail closed.</summary>
        public RailwayCosmeticElement(string canonicalItemId)
        {
            if(!SupportsItem(canonicalItemId))throw new ArgumentException("RAILWAY_COSMETIC_RENDERER_MISSING",nameof(canonicalItemId));
            itemId=canonicalItemId;pickingMode=PickingMode.Ignore;AddToClassList("cosmetic-illustration");generateVisualContent+=Draw;
        }
        /// <summary>Whether an item has a real renderer rather than a placeholder.</summary>
        public static bool SupportsItem(string id)=>Array.IndexOf(Items,id)>=0;
        /// <summary>Closed decorative module vocabulary used by all twenty authored compositions.</summary>
        public static bool SupportsModule(string kind)=>kind=="bench"||kind=="wayfinding"||kind=="barrier"||kind=="indicator"||kind=="platform"||kind=="tunnel"||kind=="sev-stop"||kind=="signal"||kind=="noticeboard"||kind=="bridge"||kind=="depot";
        internal static string? ModuleForItem(string id)=>id switch
        {
            "obj-wartebank-ohne-anschluss"=>"bench","obj-wegweiser-vielleicht"=>"wayfinding","obj-absperrung-laufzeit-offen"=>"barrier",
            "obj-anzeiger-in-pruefung"=>"indicator","obj-bahnsteig-provisorisch"=>"platform","obj-tunnelportal-wartung"=>"tunnel",
            "obj-sev-bucht-geplant"=>"sev-stop","obj-signal-freigegeben"=>"signal","obj-tafel-trotzdem-weiter"=>"noticeboard",_=>null
        };
        private void Draw(MeshGenerationContext context)
        {
            var r=contentRect;if(r.width<=0||r.height<=0)return;
            StpShowcase.Paint(context,new Rect(0,0,r.width,r.height),itemId);
        }
    }
}
