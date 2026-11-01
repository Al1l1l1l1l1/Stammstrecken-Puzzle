using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Fourteen individually recognizable original cosmetic illustrations, sharing a coherent construction kit.</summary>
    public sealed class RailwayCosmeticElement:VisualElement
    {
        private readonly string itemId;
        private static readonly string[] Items={"train-stammrot-default","obj-wartebank-ohne-anschluss","obj-wegweiser-vielleicht","obj-absperrung-laufzeit-offen","obj-anzeiger-in-pruefung","livery-betriebsrot","livery-baustellengelb","obj-bahnsteig-provisorisch","obj-tunnelportal-wartung","obj-sev-bucht-geplant","train-ersatzgarnitur","train-anschlusswunder","obj-signal-freigegeben","obj-tafel-trotzdem-weiter"};
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
        internal static void ResolveTrainStyle(System.Collections.Generic.IReadOnlyList<string> ids,out string color,out int variant)
        {
            color="C83D45";variant=0;string? livery=null;
            foreach(var id in ids)
            {
                if(id=="train-ersatzgarnitur")variant=1;else if(id=="train-anschlusswunder"){variant=2;color="A6353F";}
                if(id.StartsWith("livery-",StringComparison.Ordinal))livery=id;
            }
            if(livery=="livery-betriebsrot")color="B7353E";else if(livery=="livery-baustellengelb")color="D3AF50";
        }
        internal static string? ModuleForItem(string id)=>id switch
        {
            "obj-wartebank-ohne-anschluss"=>"bench","obj-wegweiser-vielleicht"=>"wayfinding","obj-absperrung-laufzeit-offen"=>"barrier",
            "obj-anzeiger-in-pruefung"=>"indicator","obj-bahnsteig-provisorisch"=>"platform","obj-tunnelportal-wartung"=>"tunnel",
            "obj-sev-bucht-geplant"=>"sev-stop","obj-signal-freigegeben"=>"signal","obj-tafel-trotzdem-weiter"=>"noticeboard",_=>null
        };
        private void Draw(MeshGenerationContext context)
        {
            var p=context.painter2D;var r=contentRect;if(r.width<=0||r.height<=0)return;
            RailwayDrawing.Box(p,0,0,r.width,r.height,RailwayDrawing.Color("E8E9E2"),0);
            var scale=Mathf.Min(r.width/200,r.height/130);var ox=(r.width-200*scale)*.5f;var oy=(r.height-130*scale)*.5f;
            if(itemId.StartsWith("train-",StringComparison.Ordinal)||itemId.StartsWith("livery-",StringComparison.Ordinal))
            {
                var yellow=itemId=="livery-baustellengelb";var express=itemId=="train-anschlusswunder";var replacement=itemId=="train-ersatzgarnitur";
                var train=new Rect(ox+8*scale,oy+31*scale,184*scale,64*scale);
                RailwayDrawing.TrainSide(p,train,yellow?"D3AF50":express?"A6353F":"C83D45",replacement?1:express?2:0);
                RailwayDrawing.Line(p,new Vector2(ox+2*scale,oy+101*scale),new Vector2(ox+198*scale,oy+101*scale),RailwayDrawing.Ink,3*scale);
                if(itemId=="livery-betriebsrot")RailwayDrawing.Box(p,ox+29*scale,oy+117*scale,142*scale,7*scale,RailwayDrawing.Color("C83D45"),0);
                else if(yellow)RailwayDrawing.Box(p,ox+29*scale,oy+117*scale,142*scale,7*scale,RailwayDrawing.Color("D3AF50"),0);
                return;
            }
            string kind=itemId switch
            {
                "obj-wartebank-ohne-anschluss"=>"bench","obj-wegweiser-vielleicht"=>"wayfinding","obj-absperrung-laufzeit-offen"=>"barrier",
                "obj-anzeiger-in-pruefung"=>"indicator","obj-bahnsteig-provisorisch"=>"platform","obj-tunnelportal-wartung"=>"tunnel",
                "obj-sev-bucht-geplant"=>"sev-stop","obj-signal-freigegeben"=>"signal",_=>"noticeboard"
            };
            DrawModule(p,kind,new Vector2(ox+100*scale,oy+112*scale),scale*1.45f);
        }
        internal static void DrawModule(Painter2D p,string kind,Vector2 basePoint,float scale)
        {
            void Box(float x,float y,float w,float h,string color,float stroke=2)=>RailwayDrawing.Box(p,basePoint.x+x*scale,basePoint.y+y*scale,w*scale,h*scale,RailwayDrawing.Color(color),stroke*scale);
            void Line(float x,float y,float xx,float yy,string color,float width=2)=>RailwayDrawing.Line(p,basePoint+new Vector2(x,y)*scale,basePoint+new Vector2(xx,yy)*scale,RailwayDrawing.Color(color),width*scale);
            switch(kind)
            {
                case "bench":
                    Box(-43,-40,86,14,"B89D73");Box(-43,-20,86,10,"C5AE85");Box(-34,-10,6,14,"59666B");Box(28,-10,6,14,"59666B");
                    Line(-39,-33,39,-33,"8C765A",1);break;
                case "wayfinding":
                    Box(-3,-69,6,73,"59666B");Box(-40,-63,77,17,"EAE7D9");Box(-29,-39,77,17,"EAE7D9");
                    Line(-29,-55,20,-55,"59666B");Line(21,-59,28,-55,"59666B");Line(28,-55,21,-51,"59666B");
                    Line(34,-30,-13,-30,"59666B");Line(-13,-30,-6,-34,"59666B");Line(-13,-30,-6,-26,"59666B");break;
                case "barrier":
                    Box(-41,-45,82,31,"E8E0CB");Box(-34,-14,5,18,"59666B");Box(29,-14,5,18,"59666B");
                    for(var i=0;i<4;i++)RailwayDrawing.Polygon(p,RailwayDrawing.Color("C83D45"),new[]{basePoint+new Vector2(-38+i*22,-43)*scale,basePoint+new Vector2(-28+i*22,-43)*scale,basePoint+new Vector2(-38+i*22,-16)*scale,basePoint+new Vector2(-48+i*22,-16)*scale},0);break;
                case "indicator":
                    Box(-4,-45,8,49,"59666B");Box(-50,-75,100,38,"303A3E");Box(-47,-72,94,5,"C83D45",0);
                    for(var i=0;i<3;i++){Box(-42,-59+i*7,42,2,"D7D7C8",0);Box(15,-59+i*7,26,2,"A0ABA9",0);}break;
                case "platform":
                    Box(-54,-22,108,20,"9CA7A2");Box(-57,-28,114,9,"DAD6C8");Box(-44,-10,9,15,"66736F");Box(36,-10,9,15,"66736F");
                    Line(-49,-26,49,-26,"EAE7D9",2);Box(-3,-69,5,42,"59666B");Box(-30,-72,62,17,"C6C9BE");break;
                case "tunnel":
                    Box(-52,-66,104,70,"ADB4AA");RailwayDrawing.RoundBox(p,new Rect(basePoint.x-35*scale,basePoint.y-53*scale,70*scale,65*scale),24*scale,RailwayDrawing.Color("303A3E"),3*scale);
                    Box(-57,-72,114,10,"D0D0C0");Box(-50,-17,18,5,"C83D45");Box(33,-17,18,5,"C83D45");
                    Line(-18,-1,-27,11,"929B94");Line(18,-1,27,11,"929B94");break;
                case "sev-stop":
                    Box(-46,-10,100,9,"B6B9AB");Box(-33,-69,5,59,"59666B");Box(-46,-73,31,25,"D5C99A");Box(-43,-69,25,4,"C83D45",0);
                    Box(2,-53,47,42,"E0D9BC");Box(8,-47,34,14,"596D72");Box(37,-27,5,14,"A6AB96",1);
                    RailwayDrawing.Disc(p,basePoint+new Vector2(10,-10)*scale,5*scale,RailwayDrawing.Ink);RailwayDrawing.Disc(p,basePoint+new Vector2(40,-10)*scale,5*scale,RailwayDrawing.Ink);break;
                case "signal":
                    Box(-4,-55,8,59,"59666B");Box(-13,-86,26,42,"303A3E");
                    RailwayDrawing.Disc(p,basePoint+new Vector2(0,-75)*scale,5*scale,RailwayDrawing.Color("727B82"));
                    RailwayDrawing.Disc(p,basePoint+new Vector2(0,-56)*scale,5*scale,RailwayDrawing.Color("DDD7BF"));Box(-12,0,24,6,"A8AEA2");break;
                case "noticeboard":
                    Box(-40,-76,80,61,"D9D4C2");Box(-35,-71,70,11,"C83D45");Box(-31,-54,62,29,"303A3E");Box(-31,-15,5,19,"59666B");Box(26,-15,5,19,"59666B");
                    for(var i=0;i<3;i++)Box(-25,-48+i*8,48-i*7,2,"E8E4D2",0);break;
                case "bridge":
                    Box(-59,-47,118,13,"9EA9A1");Box(-46,-34,12,38,"8D9A92");Box(34,-34,12,38,"8D9A92");
                    for(var i=0;i<7;i++)Box(-54+i*17,-62,3,16,"59666B",1);Line(-57,-63,57,-63,"59666B",3);break;
                case "depot":
                    Box(-49,-62,98,66,"A9B0A5");RailwayDrawing.Polygon(p,RailwayDrawing.Color("D7D4C5"),new[]{basePoint+new Vector2(-57,-63)*scale,basePoint+new Vector2(0,-88)*scale,basePoint+new Vector2(57,-63)*scale},2*scale);
                    for(var i=0;i<3;i++)Box(-38+i*28,-39,20,43,"59666B");break;
            }
        }
    }
}
