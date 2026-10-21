using System;
using STP.Application;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Stable product cell controls with isolated count gutters and a path-pose-only train overlay.</summary>
    public sealed class ProductionPuzzleGrid : VisualElement, IDisposable
    {
        private readonly Button[] cells;
        private readonly ProductionTrackGlyph[] glyphs;
        private readonly Label[] rows;
        private readonly Label[] columns;
        private readonly VisualElement board;
        private readonly TrainOverlay train;
        private readonly Label a;
        private readonly Label b;
        private readonly IReviewPuzzleSession model;
        private readonly Func<string, object[], string> localize;
        private Action<int,int>? activation;
        private bool disposed;

        /// <summary>Creates exactly one semantic focusable target for every public cell.</summary>
        public ProductionPuzzleGrid(IReviewPuzzleSession model, Func<string,object[],string> localize, Action<int,int> activated)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.localize = localize ?? throw new ArgumentNullException(nameof(localize));
            activation = activated ?? throw new ArgumentNullException(nameof(activated));
            if (model.Width < 2 || model.Width > 32 || model.Height < 2 || model.Height > 32) throw new ArgumentException("PRODUCT_GRID_DIMENSIONS");
            name = "puzzle-grid"; AddToClassList("puzzle-grid");
            GridWidth = model.Width; GridHeight = model.Height;
            board = new VisualElement { name = "puzzle-board" }; board.AddToClassList("puzzle-board"); Add(board);
            cells = new Button[GridWidth*GridHeight]; glyphs = new ProductionTrackGlyph[cells.Length];
            for(var y=0;y<GridHeight;y++) for(var x=0;x<GridWidth;x++)
            {
                var cx=x;var cy=y;var i=y*GridWidth+x;
                var cell=new Button(()=>activation?.Invoke(cx,cy)) { name=CellName(x,y),focusable=true,tabIndex=i };
                cell.AddToClassList("puzzle-cell");
                glyphs[i]=new ProductionTrackGlyph(false);cell.Add(glyphs[i]);board.Add(cell);cells[i]=cell;
            }
            rows=new Label[GridHeight];columns=new Label[GridWidth];
            for(var y=0;y<GridHeight;y++) { rows[y]=MakeLabel("puzzle-row-"+y,"puzzle-count");Add(rows[y]); }
            for(var x=0;x<GridWidth;x++) { columns[x]=MakeLabel("puzzle-column-"+x,"puzzle-count");Add(columns[x]); }
            a=MakeLabel("puzzle-endpoint-a","puzzle-endpoint");a.text=localize("endpoint.a",Array.Empty<object>());Add(a);
            b=MakeLabel("puzzle-endpoint-b","puzzle-endpoint");b.text=localize("endpoint.b",Array.Empty<object>());Add(b);
            train=new TrainOverlay();board.Add(train);
            RegisterCallback<GeometryChangedEvent>(OnGeometry);
            Refresh(false);
        }
        /// <summary>Number of displayed columns.</summary>
        public int GridWidth { get; }
        /// <summary>Number of displayed rows.</summary>
        public int GridHeight { get; }
        /// <summary>Board content, excluding targets and exterior ports.</summary>
        public VisualElement Board => board;
        /// <summary>Stable semantic cell identifier.</summary>
        public static string CellName(int x,int y)=>"puzzle-cell-"+x+"-"+y;
        /// <summary>Returns the same control throughout save, undo and tool changes.</summary>
        public Button CellButton(int x,int y)
        { if(x<0||y<0||x>=GridWidth||y>=GridHeight)throw new ArgumentOutOfRangeException(nameof(x));return cells[y*GridWidth+x]; }
        /// <summary>Updates contents only, never geometry or the element tree.</summary>
        public void Refresh(bool interactive)
        {
            if(disposed)return;
            var revealing=model.Phase==DevelopmentPreviewPhase.TrainRide||model.Phase==DevelopmentPreviewPhase.Results;
            for(var y=0;y<GridHeight;y++)for(var x=0;x<GridWidth;x++)
            {
                var i=y*GridWidth+x;var value=model.CellAt(x,y);
                glyphs[i].SetContent(revealing&&(value==DevelopmentTool.EmptyMarker||value==DevelopmentTool.OccupiedMarker)?DevelopmentTool.Clear:value);
                var key=value==DevelopmentTool.Clear?"accessibility.cell.empty":ProductionGamePresenter.ToolKey(value);
                cells[i].tooltip=localize("cell",new object[]{x+1,y+1,localize(key,Array.Empty<object>())});
                if(cells[i].enabledSelf!=interactive)cells[i].SetEnabled(interactive);
            }
            for(var y=0;y<GridHeight;y++){SetCount(rows[y],model.RowVisible(y),model.RowTarget(y));rows[y].tooltip=localize("accessibility.row",new object[]{y+1,model.RowVisible(y),model.RowTarget(y)});}
            for(var x=0;x<GridWidth;x++){SetCount(columns[x],model.ColumnVisible(x),model.ColumnTarget(x));columns[x].tooltip=localize("accessibility.column",new object[]{x+1,model.ColumnVisible(x),model.ColumnTarget(x)});}
            EnableInClassList("puzzle-grid--ride",revealing);
        }
        /// <summary>Displays an externally supplied world pose, without inferring a route.</summary>
        public void SetTrainPose(float x,float y,float clockwiseDegrees,bool visible)=>train.SetPose(x,y,clockwiseDegrees,visible);
        /// <summary>Projects selected train and livery without changing path geometry or timings.</summary>
        public void SetTrainAppearance(System.Collections.Generic.IReadOnlyList<string> ids)=>train.SetAppearance(ids);
        /// <summary>Releases callbacks; repeated disposal is safe.</summary>
        public void Dispose()
        {if(disposed)return;disposed=true;activation=null;UnregisterCallback<GeometryChangedEvent>(OnGeometry);SetEnabled(false);}
        private static Label MakeLabel(string id,string css)
        {var label=new Label{name=id,pickingMode=PickingMode.Ignore};label.AddToClassList(css);return label;}
        private void SetCount(Label label,int visible,int target)
        {
            label.text=target.ToString(System.Globalization.CultureInfo.InvariantCulture);
            label.tooltip=localize("count",new object[]{visible,target});
            label.EnableInClassList("puzzle-count--exact",visible==target);
            label.EnableInClassList("puzzle-count--exceeded",visible>target);
        }
        private void OnGeometry(GeometryChangedEvent evt)
        {
            // Count boxes and exterior ports own separate fixed lanes, independent of cell contents or save status.
            const float countLane=24,portLane=18,laneGap=6,portGap=2;
            const float left=countLane+laneGap+portLane+portGap,top=left,right=22,bottom=22;
            var availableW=contentRect.width-left-right;var availableH=contentRect.height-top-bottom;
            if(availableW<=0||availableH<=0)return;
            var size=Mathf.Min(availableW/GridWidth,availableH/GridHeight);
            var ox=left+(availableW-size*GridWidth)*.5f;var oy=top+(availableH-size*GridHeight)*.5f;
            RectFor(board,ox,oy,size*GridWidth,size*GridHeight);
            for(var y=0;y<GridHeight;y++)
            {
                RectFor(rows[y],ox-left,oy+y*size,countLane,size);
                for(var x=0;x<GridWidth;x++)RectFor(cells[y*GridWidth+x],x*size,y*size,size,size);
            }
            for(var x=0;x<GridWidth;x++)RectFor(columns[x],ox+x*size,oy-top,size,countLane);
            Port(a,model.EndpointA,ox,oy,size);Port(b,model.EndpointB,ox,oy,size);
            EndpointLabel(a,model.EndpointA);EndpointLabel(b,model.EndpointB);
            train.CellSize=size;RectFor(train,0,0,size*GridWidth,size*GridHeight);
        }
        private void EndpointLabel(Label label,DevelopmentEndpoint port)
        {label.tooltip=localize("accessibility.endpoint",new object[]{label.text,port.X+1,port.Y+1,localize("accessibility.edge."+port.Edge,Array.Empty<object>())});}
        private static void Port(Label label,DevelopmentEndpoint port,float ox,float oy,float size)
        {
            var x=ox+(port.X+.5f)*size-9;var y=oy+(port.Y+.5f)*size-9;
            switch(port.Edge)
            {case DevelopmentEdge.N:y=oy-20;break;case DevelopmentEdge.E:x=ox+(port.X+1)*size+2;break;
             case DevelopmentEdge.S:y=oy+(port.Y+1)*size+2;break;case DevelopmentEdge.W:x=ox-20;break;}
            RectFor(label,x,y,18,18);
        }
        private static void RectFor(VisualElement e,float x,float y,float w,float h)
        {e.style.left=x;e.style.top=y;e.style.width=w;e.style.height=h;}

        private sealed class TrainOverlay:VisualElement
        {
            private float x,y,angle;private bool trainVisible;private string bodyColor="C83D45";private int variant;
            public TrainOverlay(){name="puzzle-train";pickingMode=PickingMode.Ignore;style.position=Position.Absolute;generateVisualContent+=Draw;}
            public float CellSize{get;set;}
            public void SetPose(float px,float py,float degrees,bool show)
            {x=px;y=py;angle=degrees;trainVisible=show;MarkDirtyRepaint();}
            public void SetAppearance(System.Collections.Generic.IReadOnlyList<string> ids){RailwayCosmeticElement.ResolveTrainStyle(ids,out bodyColor,out variant);MarkDirtyRepaint();}
            private void Draw(MeshGenerationContext context)
            {
                if(!trainVisible||CellSize<=0)return;
                var p=context.painter2D;var c=new Vector2(x*CellSize,y*CellSize);
                var dir=new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad));var side=new Vector2(-dir.y,dir.x);
                Vector2 Point(float f,float s)=>c+dir*(f*CellSize)+side*(s*CellSize);
                RailwayDrawing.Polygon(p,RailwayDrawing.Color("20272C"),new[]{Point(-.38f,-.2f),Point(.28f,-.2f),Point(.4f,-.12f),Point(.4f,.12f),Point(.28f,.2f),Point(-.38f,.2f)},Mathf.Max(1,CellSize*.035f));
                RailwayDrawing.Polygon(p,RailwayDrawing.Color(bodyColor),new[]{Point(-.35f,-.16f),Point(.27f,-.16f),Point(.36f,-.1f),Point(.36f,.1f),Point(.27f,.16f),Point(-.35f,.16f)},0);
                RailwayDrawing.Polygon(p,RailwayDrawing.Color("E9E6D9"),new[]{Point(-.31f,-.10f),Point(.12f,-.10f),Point(.12f,.10f),Point(-.31f,.10f)},0);
                RailwayDrawing.Line(p,Point(.2f,-.13f),Point(.2f,.13f),RailwayDrawing.Color("394E58"),CellSize*.09f);
                RailwayDrawing.Line(p,Point(-.2f,-.05f),Point(.02f,-.05f),RailwayDrawing.Color("59666B"),CellSize*.06f);
                if(variant==1)RailwayDrawing.Line(p,Point(-.07f,-.16f),Point(-.07f,.16f),RailwayDrawing.Ink,CellSize*.045f);
                if(variant==2)RailwayDrawing.Line(p,Point(-.28f,.13f),Point(.15f,.13f),RailwayDrawing.Color("F1EEE4"),CellSize*.035f);
            }
        }
    }

    internal sealed class ProductionTrackGlyph:VisualElement
    {
        private DevelopmentTool tool;private readonly bool toolIcon;
        internal ProductionTrackGlyph(bool toolIcon)
        {this.toolIcon=toolIcon;pickingMode=PickingMode.Ignore;AddToClassList("track-glyph");generateVisualContent+=Draw;}
        internal void SetContent(DevelopmentTool content)
        {if(tool==content)return;tool=content;MarkDirtyRepaint();}
        private void Draw(MeshGenerationContext context)
        {
            var w=contentRect.width;var h=contentRect.height;if(w<=0||h<=0)return;
            var p=context.painter2D;var c=Mathf.Min(w,h);var inset=toolIcon?c*.18f:0;
            var size=c-2*inset;var origin=new Vector2((w-size)*.5f,(h-size)*.5f);
            Vector2 At(float x,float y)=>origin+new Vector2(x*size,y*size);
            if(tool==DevelopmentTool.Clear)
            {
                if(toolIcon)RailwayDrawing.Polygon(p,RailwayDrawing.Color("F4F2EE"),new[]{At(.19f,.62f),At(.53f,.25f),At(.82f,.52f),At(.57f,.79f),At(.36f,.79f)},2);
                return;
            }
            if(tool==DevelopmentTool.EmptyMarker)
            {RailwayDrawing.Line(p,At(.25f,.25f),At(.75f,.75f),RailwayDrawing.Color("B42E39"),Mathf.Max(2,size*.065f));RailwayDrawing.Line(p,At(.75f,.25f),At(.25f,.75f),RailwayDrawing.Color("B42E39"),Mathf.Max(2,size*.065f));return;}
            if(tool==DevelopmentTool.OccupiedMarker)
            {
                var gray=RailwayDrawing.Color("727B82");var line=Mathf.Max(1.5f,size*.045f);
                RailwayDrawing.Line(p,At(.28f,.42f),At(.72f,.42f),gray,line);RailwayDrawing.Line(p,At(.28f,.58f),At(.72f,.58f),gray,line);
                for(var i=0;i<3;i++)RailwayDrawing.Line(p,At(.35f+i*.15f,.32f),At(.35f+i*.15f,.68f),gray,line);return;
            }
            p.strokeColor=RailwayDrawing.Color("805535");p.lineWidth=size*.18f;p.lineCap=LineCap.Butt;p.lineJoin=LineJoin.Round;
            p.BeginPath();const float k=.276142375f;
            switch(tool)
            {
                case DevelopmentTool.TrackNS:p.MoveTo(At(.5f,0));p.LineTo(At(.5f,1));break;
                case DevelopmentTool.TrackEW:p.MoveTo(At(0,.5f));p.LineTo(At(1,.5f));break;
                case DevelopmentTool.TrackNE:p.MoveTo(At(.5f,0));p.BezierCurveTo(At(.5f,k),At(1-k,.5f),At(1,.5f));break;
                case DevelopmentTool.TrackES:p.MoveTo(At(1,.5f));p.BezierCurveTo(At(1-k,.5f),At(.5f,1-k),At(.5f,1));break;
                case DevelopmentTool.TrackSW:p.MoveTo(At(.5f,1));p.BezierCurveTo(At(.5f,1-k),At(k,.5f),At(0,.5f));break;
                case DevelopmentTool.TrackWN:p.MoveTo(At(0,.5f));p.BezierCurveTo(At(k,.5f),At(.5f,k),At(.5f,0));break;
            }
            p.Stroke();
        }
    }
}
