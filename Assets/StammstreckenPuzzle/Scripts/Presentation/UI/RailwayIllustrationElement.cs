using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Original procedural 3D railway diorama (bubbly, depth-sorted mesh) with the player's actual train, wagons and decorations.</summary>
    public sealed class RailwayIllustrationElement : VisualElement
    {
        private int world;
        private RailwayRouteComposition? composition;
        private int clearedStage;
        private StpVehicle vehicle=StpKit.Resolve(System.Array.Empty<string>());
        private string[] selectedProps=System.Array.Empty<string>();
        /// <summary>Creates a decorative, non-interactive railway composition.</summary>
        public RailwayIllustrationElement()
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList("railway-illustration");
            generateVisualContent += Draw;
        }
        /// <summary>Selects one of the five campaign world compositions.</summary>
        public void SetWorld(int section)
        { var next = Mathf.Clamp(section, 0, 4); if (world != next) { world = next; MarkDirtyRepaint(); } }
        /// <summary>Uses an explicitly resolved route and stage from the versioned asset manifest.</summary>
        public void SetComposition(RailwayRouteComposition route,int stage)
        {
            if(route==null)throw new System.ArgumentNullException(nameof(route));
            var nextStage=Mathf.Clamp(stage,0,12);
            if(ReferenceEquals(composition,route)&&clearedStage==nextStage&&world==route.section-1)return;
            composition=route;world=route.section-1;clearedStage=nextStage;MarkDirtyRepaint();
        }

        /// <summary>Uses actual selected train/livery and decorative objects without altering any puzzle fact.</summary>
        public void SetActiveCosmetics(System.Collections.Generic.IReadOnlyList<string> ids)
        {
            vehicle=StpKit.Resolve(ids);var props=new System.Collections.Generic.List<string>();
            foreach(var id in ids){var module=RailwayCosmeticElement.ModuleForItem(id);if(module!=null)props.Add(module);}selectedProps=props.ToArray();MarkDirtyRepaint();
        }
        private void Draw(MeshGenerationContext context)
        {
            var w = contentRect.width; var h = contentRect.height;
            if (w <= 0 || h <= 0) return;
            StpDiorama.Paint(context, new Rect(0, 0, w, h), world, vehicle, selectedProps, composition, clearedStage);
        }
    }
    internal static class RailwayDrawing
    {
        internal static readonly Color Ink = Color("20272C");
        internal static Color Color(string hex)
        { UnityEngine.ColorUtility.TryParseHtmlString("#" + hex, out var result); return result; }
        internal static void Line(Painter2D p, Vector2 from, Vector2 to, Color color, float width)
        { p.strokeColor = color; p.lineWidth = width; p.lineCap = LineCap.Butt; p.BeginPath(); p.MoveTo(from); p.LineTo(to); p.Stroke(); }
        internal static void Polygon(Painter2D p, Color fill, Vector2[] vertices, float stroke = 2)
        {
            p.fillColor = fill; p.strokeColor = Ink; p.lineWidth = stroke; p.lineJoin = LineJoin.Round;
            p.BeginPath(); p.MoveTo(vertices[0]); for (var i = 1; i < vertices.Length; i++) p.LineTo(vertices[i]); p.ClosePath(); p.Fill(); if (stroke > 0) p.Stroke();
        }
        internal static void Box(Painter2D p, float x, float y, float w, float h, Color fill, float stroke = 2)
        { Polygon(p, fill, new[] { new Vector2(x,y), new Vector2(x+w,y), new Vector2(x+w,y+h), new Vector2(x,y+h) }, stroke); }
        internal static void RoundBox(Painter2D p, Rect r, float radius, Color fill, float stroke = 2)
        {
            var k = radius * .55228475f;
            p.fillColor = fill; p.strokeColor = Ink; p.lineWidth = stroke; p.lineJoin = LineJoin.Round;
            p.BeginPath(); p.MoveTo(new Vector2(r.x + radius,r.y)); p.LineTo(new Vector2(r.xMax-radius,r.y));
            p.BezierCurveTo(new Vector2(r.xMax-radius+k,r.y),new Vector2(r.xMax,r.y+radius-k),new Vector2(r.xMax,r.y+radius));
            p.LineTo(new Vector2(r.xMax,r.yMax-radius)); p.BezierCurveTo(new Vector2(r.xMax,r.yMax-radius+k),new Vector2(r.xMax-radius+k,r.yMax),new Vector2(r.xMax-radius,r.yMax));
            p.LineTo(new Vector2(r.x+radius,r.yMax)); p.BezierCurveTo(new Vector2(r.x+radius-k,r.yMax),new Vector2(r.x,r.yMax-radius+k),new Vector2(r.x,r.yMax-radius));
            p.LineTo(new Vector2(r.x,r.y+radius)); p.BezierCurveTo(new Vector2(r.x,r.y+radius-k),new Vector2(r.x+radius-k,r.y),new Vector2(r.x+radius,r.y));
            p.ClosePath(); p.Fill(); if (stroke > 0) p.Stroke();
        }
        internal static void Disc(Painter2D p, Vector2 center, float radius, Color fill)
        { RoundBox(p, new Rect(center.x-radius,center.y-radius,radius*2,radius*2),radius,fill,1.5f); }
        internal static void TrainSide(Painter2D p, Rect r,string bodyColor="C83D45",int variant=0)
        {
            var sx = r.width / 260; var sy = r.height / 84;
            void Box(float x,float y,float w,float h,string color,float stroke=2) => RailwayDrawing.Box(p,r.x+x*sx,r.y+y*sy,w*sx,h*sy,Color(color),stroke*sx);
            RoundBox(p,new Rect(r.x,r.y+9*sy,r.width,r.height-15*sy),12*sx,Color(bodyColor),3*sx);
            Box(12,3,229,10,"495256"); Box(5,57,247,14,"E9E6D9"); Box(15,72,224,7,"495256");
            Box(13,20,21,36,"E9E6D9"); Box(91,20,21,36,"E9E6D9"); Box(171,20,21,36,"E9E6D9");
            Box(18,24,11,20,"56676D",1); Box(96,24,11,20,"56676D",1); Box(176,24,11,20,"56676D",1);
            for(var i=0;i<2;i++) { Box(43+i*23,22,18,24,"394E58",1.5f); Box(122+i*23,22,18,24,"394E58",1.5f); }
            Box(207,20,36,27,"394E58"); Box(217,52,18,4,"E8D4A2",0);
            Box(20,62,22,2,"BDACA2",0); Box(118,62,31,2,"BDACA2",0);
            if(variant==1){Box(84,14,10,57,"A3A79C",1);Box(163,14,10,57,"D3B967",1);Box(8,57,72,4,"AD9181",0);}
            if(variant==2){Box(12,48,229,4,"E9E6D9",0);Box(119,6,20,5,"BDBFB0",1);Box(118,21,50,5,"E4DAC1",0);}
            foreach(var x in new[]{36,65,192,221}) Disc(p,new Vector2(r.x+x*sx,r.y+78*sy),5*sx,Color("303A3E"));
        }
    }
}
