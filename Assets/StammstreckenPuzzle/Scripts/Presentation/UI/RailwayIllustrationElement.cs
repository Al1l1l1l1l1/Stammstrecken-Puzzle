using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Original code-native cartoon railway artwork, sharing the accepted red/white/charcoal visual family.</summary>
    public sealed class RailwayIllustrationElement : VisualElement
    {
        private int world;
        private RailwayRouteComposition? composition;
        private int clearedStage;
        private string trainColor="C83D45";
        private int trainVariant;
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
            RailwayCosmeticElement.ResolveTrainStyle(ids,out trainColor,out trainVariant);var props=new System.Collections.Generic.List<string>();
            foreach(var id in ids){var module=RailwayCosmeticElement.ModuleForItem(id);if(module!=null)props.Add(module);}selectedProps=props.ToArray();MarkDirtyRepaint();
        }
        private void Draw(MeshGenerationContext context)
        {
            var w = contentRect.width; var h = contentRect.height;
            if (w <= 0 || h <= 0) return;
            var p = context.painter2D;
            var scale = new Vector2(w / 400f, h / 240f);
            void Box(float x, float y, float bw, float bh, string fill, float outline = 2.5f) =>
                RailwayDrawing.Box(p, x * scale.x, y * scale.y, bw * scale.x, bh * scale.y, RailwayDrawing.Color(fill), outline * scale.x);
            void Shape(string fill, params Vector2[] points)
            {
                for (var i = 0; i < points.Length; i++) points[i] = Vector2.Scale(points[i], scale);
                RailwayDrawing.Polygon(p, RailwayDrawing.Color(fill), points, 2.5f * scale.x);
            }
            Box(0, 0, 400, 240, world == 4 ? "E9E3D8" : "E7ECE9", 0);
            Shape("D9DFDB", new Vector2(0, 92), new Vector2(67, 72), new Vector2(132, 89), new Vector2(210, 65), new Vector2(285, 78), new Vector2(400, 56), new Vector2(400, 240), new Vector2(0, 240));
            // Cropped chunky station, window rhythm and two drawn shade planes.
            Box(10, 33, 138, 111, "B2B6AF");
            Box(18, 45, 120, 12, "D6D6CB", 0);
            for (var i = 0; i < 4; i++) { Box(24 + i * 28, 65, 17, 29, "59666B", 1.8f); Box(24 + i * 28, 101, 17, 27, "737D7D", 1.8f); }
            Shape("8C9794", new Vector2(148, 33), new Vector2(164, 45), new Vector2(164, 144), new Vector2(148, 144));
            Box(7, 26, 144, 10, "495256");
            if (world == 0)
            {
                Box(261, 58, 135, 99, "939E9C");
                RailwayDrawing.RoundBox(p, new Rect(280 * scale.x, 78 * scale.y, 91 * scale.x, 90 * scale.y), 29 * scale.x, RailwayDrawing.Color("303B40"), 3 * scale.x);
                Box(272, 47, 115, 15, "D2D2C8");
                Box(286, 83, 77, 6, "5D6969", 0);
            }
            else if (world == 1)
            {
                Shape("93745B", new Vector2(223, 126), new Vector2(368, 119), new Vector2(393, 165), new Vector2(211, 177));
                Box(260, 28, 7, 94, "D2A84B"); Box(250, 25, 111, 7, "D2A84B");
                Box(350, 31, 4, 47, "5F6969", 1); Box(338, 77, 29, 8, "D2A84B");
            }
            else if (world == 2)
            {
                Box(256, 59, 127, 83, "A9B0AC");
                Shape("D3D3C8", new Vector2(247, 58), new Vector2(321, 24), new Vector2(392, 58));
                for (var i = 0; i < 4; i++) Box(267 + i * 27, 89, 18, 53, "5B666B");
            }
            else if (world == 3)
            {
                Shape("BBC3BD", new Vector2(210, 110), new Vector2(391, 73), new Vector2(398, 101), new Vector2(216, 139));
                Box(240, 130, 13, 35, "8C9894"); Box(356, 105, 13, 47, "8C9894");
                Box(310, 34, 5, 62, "59666B"); Box(278, 42, 71, 17, "F1EEE4");
            }
            else
            {
                RailwayDrawing.RoundBox(p, new Rect(249 * scale.x, 84 * scale.y, 132 * scale.x, 59 * scale.y), 10 * scale.x, RailwayDrawing.Color("CEB475"), 3 * scale.x);
                Box(260, 93, 82, 22, "485B64"); Box(351, 93, 21, 40, "D7D4BB");
                RailwayDrawing.Disc(p, new Vector2(273 * scale.x, 144 * scale.y), 9 * scale.x, RailwayDrawing.Color("303A3E"));
                RailwayDrawing.Disc(p, new Vector2(355 * scale.x, 144 * scale.y), 9 * scale.x, RailwayDrawing.Color("303A3E"));
                Box(227, 50, 4, 102, "59666B"); Box(213, 43, 31, 24, "F1EEE4");
            }
            Shape("C8C8BC", new Vector2(0, 146), new Vector2(400, 137), new Vector2(400, 193), new Vector2(0, 209));
            Shape("A8ADA5", new Vector2(0, 178), new Vector2(400, 169), new Vector2(400, 181), new Vector2(0, 192));
            // Small separate signal is an environmental prop, never a puzzle rule.
            Box(184, 69, 6, 85, "59666B"); Box(175, 57, 24, 39, "303A3E");
            RailwayDrawing.Disc(p, new Vector2(187 * scale.x, 69 * scale.y), 4.5f * scale.x, RailwayDrawing.Color("C83D45"));
            RailwayDrawing.Disc(p, new Vector2(187 * scale.x, 84 * scale.y), 4.5f * scale.x, RailwayDrawing.Color("697473"));
            // Sleepers and double rail anchor the vehicle into the scene.
            for (var i = 0; i < 13; i++) Box(8 + i * 34, 207, 9, 23, "A28B70", 1);
            RailwayDrawing.Line(p, new Vector2(0, 213 * scale.y), new Vector2(w, 202 * scale.y), RailwayDrawing.Color("596469"), 4 * scale.x);
            RailwayDrawing.Line(p, new Vector2(0, 228 * scale.y), new Vector2(w, 217 * scale.y), RailwayDrawing.Color("596469"), 4 * scale.x);
            RailwayDrawing.TrainSide(p, new Rect(41 * scale.x, 129 * scale.y, 259 * scale.x, 84 * scale.y),trainColor,trainVariant);
            // Understated admin clutter and graphic dust, using original simple geometry.
            Box(335, 163, 42, 9, "C1AE8D"); Box(340, 173, 5, 23, "59666B"); Box(367, 173, 5, 23, "59666B");
            for (var i = 0; i < 3; i++) Box(20 + i * 9, 154 + i % 2 * 6, 3, 2, "8C9790", 0);
            for(var i=0;i<selectedProps.Length;i++)RailwayCosmeticElement.DrawModule(p,selectedProps[i],Vector2.Scale(new Vector2(32+i%5*76,143+i/5*43),scale),.55f*scale.x);
            if(composition!=null)
            {
                foreach(var module in composition.modules)RailwayCosmeticElement.DrawModule(p,module.kind,Vector2.Scale(new Vector2(module.x,module.y),scale),module.scale*scale.x);
                for(var i=0;i<12;i++)
                {
                    var color=RailwayDrawing.Color(i==clearedStage-1?"C83D45":"CCD0D1");
                    RailwayDrawing.Disc(p,new Vector2((21+i*32)*scale.x,233*scale.y),3.1f*scale.x,color);
                }
            }
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
