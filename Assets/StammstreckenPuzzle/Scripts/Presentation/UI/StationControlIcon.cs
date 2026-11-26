using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    internal sealed class StationControlIcon:VisualElement
    {
        private readonly string kind;
        internal StationControlIcon(string kind)
        {
            this.kind=kind;pickingMode=PickingMode.Ignore;AddToClassList("station-icon");
            generateVisualContent+=Draw;
        }
        private void Draw(MeshGenerationContext context)
        {
            float s=Mathf.Min(contentRect.width,contentRect.height)/32;if(s<=0)return;
            var p=context.painter2D;var ink=RailwayDrawing.Ink;
            var cream=RailwayDrawing.Color("FFEDCB");var navy=RailwayDrawing.Color("234B67");
            var red=RailwayDrawing.Color("D44946");var metal=RailwayDrawing.Color("9AB6B5");
            Vector2 At(float x,float y)=>new Vector2(x*s,y*s);
            void Rect(float x,float y,float w,float h,Color color)=>RailwayDrawing.RoundBox(p,new Rect(x*s,y*s,w*s,h*s),2*s,color,1.2f*s);
            void Line(float x,float y,float u,float v,Color color,float thickness)=>RailwayDrawing.Line(p,At(x,y),At(u,v),color,thickness*s);
            void Triangle(Vector2 a,Vector2 b,Vector2 c,Color color)=>RailwayDrawing.Polygon(p,color,new[]{a,b,c},s);
            switch(kind){
                case "mail-open":Rect(2,6,28,21,cream);Line(3,8,16,18,navy,2);Line(16,18,29,8,navy,2);RailwayDrawing.Disc(p,At(16,18),3*s,red);break;
                case "home-map":Rect(2,4,28,24,navy);Line(5,21,14,21,cream,2);Line(14,21,21,12,cream,2);Line(21,12,28,12,cream,2);
                    RailwayDrawing.Disc(p,At(14,21),3*s,red);break;
                case "home-daily":Rect(3,4,26,24,navy);Rect(6,9,8,12,cream);Rect(17,9,8,12,cream);Line(4,15,28,15,ink,2);break;
                case "home-endless":Rect(13,18,5,12,metal);Triangle(At(16,2),At(30,24),At(2,24),cream);Line(9,20,23,20,red,3);break;
                case "home-revision":Rect(4,3,24,26,metal);for(int y=7;y<=21;y+=7){Rect(7,y,18,6,cream);Line(13,y+2,19,y+2,ink,2);}break;
                case "nav-depot":Triangle(At(1,14),At(16,3),At(31,14),red);Rect(4,14,24,15,cream);Rect(11,17,10,12,navy);break;
                case "nav-profile":Rect(3,7,26,20,cream);Rect(12,5,8,4,metal);RailwayDrawing.Disc(p,At(10,14),3*s,navy);Rect(6,19,8,4,navy);Line(17,15,25,15,navy,2);break;
                case "nav-home":Rect(2,6,28,5,red);Rect(5,11,3,17,metal);Rect(24,11,3,17,metal);Rect(2,26,28,3,cream);break;
                case "puzzle-hint":Rect(3,13,12,9,metal);Triangle(At(12,13),At(26,5),At(26,26),cream);Line(9,22,11,28,ink,3);break;
                case "puzzle-undo":Rect(4,21,24,8,metal);Line(12,21,21,8,navy,4);RailwayDrawing.Disc(p,At(22,7),5*s,red);break;
                case "puzzle-reset":Rect(6,5,20,22,cream);Rect(4,3,6,24,metal);Line(12,14,18,14,navy,2);Line(18,14,22,10,navy,2);break;
            }
        }
    }
}
