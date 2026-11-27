using System;
using System.Collections.Generic;
using STP.Application;
using UnityEngine;
using UnityEngine.Rendering;

namespace STP.Presentation.World
{
    /// <summary>Owns a single camera, sun and the meshes for the currently visible railway scene.</summary>
    public sealed partial class StpWorldCamera : MonoBehaviour
    {
        private Camera lens = null!;
        private Material enamel = null!;
        private Transform stage = null!;
        private readonly Dictionary<string,Mesh> meshes = new Dictionary<string,Mesh>();
        private readonly List<Transform> cars = new List<Transform>();
        private readonly List<Transform> passengers = new List<Transform>();
        private readonly List<Transform> pigeons = new List<Transform>();
        private readonly List<PieceMotion> motions = new List<PieceMotion>();
        private readonly List<Transform> clockHands = new List<Transform>();
        private IReviewPuzzleSession? puzzle;
        private GameObject?[] pieces = Array.Empty<GameObject?>();
        private DevelopmentTool[] contents = Array.Empty<DevelopmentTool>();
        private string sceneName = "";
        private int district = -1;
        private int appearance;
        private bool riding;
        private float arrived, rideStart;
        private bool reduced;
        private Transform? specialTrain;
        private struct PieceMotion
        {
            internal GameObject Piece;
            internal float Start;
            internal bool Removing;
        }

        public static StpWorldCamera? Attach(Camera output, Shader shader)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) return null;
            var scene = output.gameObject.AddComponent<StpWorldCamera>();
            scene.lens = output;
            output.name = "StpWorldCamera";
            output.cullingMask = 1 << 29;
            output.orthographic = false;
            output.farClipPlane = 180;
            output.nearClipPlane = .1f;
            scene.enamel = new Material(shader) { name = "Railway enamel", enableInstancing = true };
            var sun = new GameObject("Afternoon light");
            sun.transform.SetParent(scene.transform.parent, false);
            var key = sun.AddComponent<Light>();
            key.type = LightType.Directional;
            key.shadows = LightShadows.Soft;
            key.color = new Color(1f,.94f,.82f);
            key.intensity = 1.05f;
            key.cullingMask = 1 << 29;
            sun.transform.rotation = Quaternion.Euler(52,-32,0);
            scene.sun = sun;
            return scene;
        }

        private GameObject? sun;
        internal bool ReversedOrder=>reversedOrder;
        internal void SetReducedMotion(bool value) => reduced = value;
        internal void SetStage(string name, int world, IReviewPuzzleSession? board, int revision,
            IReadOnlyList<string> equipment, IReadOnlyList<string> props, bool covered, int visits, bool rare)
        {
            if(name==sceneName && world==district && appearance==revision && ReferenceEquals(puzzle,board)) return;
            ClearStage();
            sceneName=name; district=world; appearance=revision; puzzle=board;
            stage=new GameObject("Railway · "+name).transform;
            stage.SetParent(transform.parent,false);
            arrived=Time.unscaledTime;
            if(board!=null){BuildBoard(board);BuildDeadline(board.Width*.70f,board.Height+1.12f);
                reversedOrder=unchecked((uint)board.AttemptId.GetHashCode())%15==0;}
            else BuildStation(name,world,props,visits,rare);
            BuildConsist(StpKit.Resolve(equipment),covered);
            if(name=="Platform"||name=="Office")BuildDeadline(2.35f,1.72f);
            if(name=="Endless")BuildDeadline(0,-.85f);
            if(name=="Depot"&&!reduced){turntable=new GameObject("Drehscheibe").transform;turntable.SetParent(stage,false);
                foreach(var car in cars)car.SetParent(turntable,false);turnStart=Time.unscaledTime;}
            if(name=="Finale")stationIndicator=Lettering("",new Vector3(0,2,1.3f),.11f,Tone(0xFFF4BC));
            if(board!=null) foreach(var car in cars)car.gameObject.SetActive(false);
        }

        private void ClearStage()
        {
            if(stage!=null){stage.gameObject.SetActive(false); Destroy(stage.gameObject);}
            foreach(var mesh in meshes.Values)Destroy(mesh);
            meshes.Clear(); cars.Clear(); passengers.Clear(); pigeons.Clear(); clockHands.Clear(); motions.Clear();
            yearSign=null;previousYears=null;highlight=null;dust=null;turntable=null;stationIndicator=null;confetti.Clear();reversedOrder=false;specialTrain=null;lastSlide=0; riding=false; pieces=Array.Empty<GameObject?>(); contents=Array.Empty<DevelopmentTool>();
        }
        private void OnDestroy(){ClearStage(); if(enamel!=null)Destroy(enamel); if(sun!=null)Destroy(sun);}

        private static Vector3 FromKit(Vector3 v)=>new Vector3(v.x,v.z,-v.y);
        private GameObject Place(string key,Action<StpBuilder> build, float x=0,float y=0,float height=0)
        {
            if(!meshes.TryGetValue(key,out var mesh))
            {
                var builder=new StpBuilder { Solid=true };
                build(builder);
                var pos=new List<Vector3>();var normals=new List<Vector3>();var colors=new List<Color32>();var indices=new List<int>();
                foreach(var part in builder.Take())
                {
                    if(part.Unlit && part.Color.a<.5f)continue;
                    int first=pos.Count;
                    var color=(Color32)part.Color.linear;
                    for(int i=0;i<part.P.Length;i++){pos.Add(FromKit(part.P[i]));normals.Add(FromKit(part.N[i]));colors.Add(color);}
                    foreach(int i in part.T)indices.Add(first+i);
                }
                mesh=new Mesh {name=key,indexFormat=pos.Count>65535?IndexFormat.UInt32:IndexFormat.UInt16};
                mesh.SetVertices(pos);mesh.SetNormals(normals);mesh.SetColors(colors);mesh.SetTriangles(indices,0);
                mesh.UploadMeshData(true);meshes.Add(key,mesh);
            }
            var obj=new GameObject(key){layer=29};
            obj.transform.SetParent(stage,false);obj.transform.localPosition=new Vector3(x,height,-y);
            obj.AddComponent<MeshFilter>().sharedMesh=mesh;obj.AddComponent<MeshRenderer>().sharedMaterial=enamel;
            return obj;
        }

        private void BuildBoard(IReviewPuzzleSession board)
        {
            int w=board.Width,h=board.Height;contents=new DevelopmentTool[w*h];pieces=new GameObject?[w*h];
            Place("Baufeld",b=>{
                b.Box(new Vector3(w*.5f,h*.5f,-.23f),new Vector3(w*.5f+.13f,h*.5f+.13f,.17f),StpKit.H(0xB69870));
                for(int y=0;y<h;y++) for(int x=0;x<w;x++)
                {
                    var tile=StpKit.H(((x+y)&1)==0?0xCCBFAA:0xC5B7A4);
                    b.Box(new Vector3(x+.5f,y+.5f,-.044f),new Vector3(.493f,.493f,.042f),tile);
                    if(w<9) for(int k=0;k<3;k++)
                    {
                        float u=(x*13+y*3+k*29)%89*.01f,v=(x*7+y*17+k*31)%83*.01f;
                        b.Ellipsoid(new Vector3(x+.05f+u,y+.08f+v,0),new Vector3(.028f,.02f,.012f),.8f,StpKit.Dark(tile,.88f),0,false,6,3);
                    }
                }
                StpKit.Stub(b,(int)board.EndpointA.Edge,board.EndpointA.X,board.EndpointA.Y);
                StpKit.Stub(b,(int)board.EndpointB.Edge,board.EndpointB.X,board.EndpointB.Y);
                StpKit.Shed(b,w*.24f,-1.6f,1.5f);StpKit.Barrier(b,w*.75f,-1.1f,1.5f);
                StpKit.Cone(b,-.75f,h+.6f);StpKit.Barrier(b,w-.5f,h+1,1.3f);
            });
            AddPlatformEnd(board.EndpointA);AddPlatformEnd(board.EndpointB);
            SyncBoard(false);
        }

        private void AddPlatformEnd(DevelopmentEndpoint port)
        {
            float x=port.X+.5f,y=port.Y+.5f;
            switch(port.Edge){case DevelopmentEdge.N:y-=.95f;break;case DevelopmentEdge.S:y+=.95f;break;
                case DevelopmentEdge.E:x+=.95f;break;default:x-=.95f;break;}
            var arch=Place("Tunnelmund",b=>StpKit.Tunnel(b,0,0,.82f),x,y);
            arch.transform.localRotation=Quaternion.Euler(0,port.Edge==DevelopmentEdge.E||port.Edge==DevelopmentEdge.W?90:0,0);
            AddCommuter(x+.49f,y+.28f,0); AddBird(x-.48f,y-.36f);
        }
        internal void SyncBoard(bool animate=true)
        {
            if(puzzle==null)return;
            for(int y=0;y<puzzle.Height;y++)for(int x=0;x<puzzle.Width;x++)
            {
                var value=puzzle.CellAt(x,y);int i=y*puzzle.Width+x;
                if(puzzle.Phase!=DevelopmentPreviewPhase.Editing && (value==DevelopmentTool.EmptyMarker||value==DevelopmentTool.OccupiedMarker))value=DevelopmentTool.Clear;
                if(contents[i]==value)continue;
                contents[i]=value;var old=pieces[i];
                if(old!=null)
                {
                    if(animate&&!reduced)motions.Add(new PieceMotion{Piece=old,Start=Time.unscaledTime,Removing=true});
                    else Destroy(old);
                }
                pieces[i]=null;if(value==DevelopmentTool.Clear)continue;
                var piece=Place(value.ToString(),b=>BuildPiece(b,value,puzzle.Width>=9),x,y);
                pieces[i]=piece;
                if(animate&&!reduced){motions.Add(new PieceMotion{Piece=piece,Start=Time.unscaledTime});Dust(x,y);}
            }
        }
        private static void BuildPiece(StpBuilder b, DevelopmentTool value,bool compact)
        {
            if(value==DevelopmentTool.EmptyMarker)
            {
                b.Box(new Vector3(.5f,.49f,.012f),new Vector3(.29f,.039f,.009f),StpKit.H(0xB73439),.74f);
                b.Box(new Vector3(.49f,.51f,.014f),new Vector3(.30f,.034f,.009f),StpKit.H(0xBC383E),-.82f);return;
            }
            if(value==DevelopmentTool.OccupiedMarker)
            {
                for(int j=0;j<3;j++)b.Box(new Vector3(.28f+j*.22f,.5f,.025f),new Vector3(.037f,.26f,.025f),StpKit.H(0x888C89));return;
            }
            int code=(int)value-(int)DevelopmentTool.TrackNS;
            StpKit.TrackBallast(b,code,0,0);StpKit.TrackRails(b,code,0,0,compact);
        }

        internal void Frame(Rect board,Vector2 panel)
        {
            // UI Toolkit can report an unmeasured rectangle when a screen is first attached.
            // Keep the last camera frame until the new board has a usable position and size.
            if(stage==null || !float.IsFinite(board.center.x) || !float.IsFinite(board.center.y) ||
                !float.IsFinite(board.width) || !float.IsFinite(board.height) ||
                !float.IsFinite(panel.x) || !float.IsFinite(panel.y) ||
                board.width<1 || board.height<1 || panel.x<1 || panel.y<1)return;
            bool editing=puzzle!=null;
            float tilt=editing?Mathf.Lerp(12,40,riding&&!reduced?Mathf.SmoothStep(0,1,(Time.unscaledTime-rideStart)/.6f):0):54;
            float visibleWidth=editing?puzzle!.Width:sceneName=="Depot"?Mathf.Lerp(3.4f,5.8f,(cars.Count-1)/3f):8.4f;
            float settle=reduced||editing?0:1-Mathf.SmoothStep(0,1,(Time.unscaledTime-arrived)/.48f);
            float distance=editing?38:12+settle*1.8f;
            float unitsPerPoint=visibleWidth/board.width;
            var center=editing?new Vector3(puzzle!.Width*.5f,0,-puzzle.Height*.5f):new Vector3(0,.45f,0);
            if(!editing)center+=new Vector3(-settle*.65f,settle*.20f,0);
            var orientation=Quaternion.Euler(90-tilt+settle*3,editing?0:-8-settle*4,0);
            float halfH=panel.y*.5f*unitsPerPoint;
            float aspect=panel.x/panel.y;
            float fieldOfView=2*Mathf.Atan(halfH/distance)*Mathf.Rad2Deg;
            if(!float.IsFinite(aspect)||aspect<=0||fieldOfView<1||fieldOfView>=179)return;
            var projection=Matrix4x4.Perspective(fieldOfView,aspect,lens.nearClipPlane,lens.farClipPlane);
            projection.m02=1-2*board.center.x/panel.x;projection.m12=2*board.center.y/panel.y-1;
            if(editing){
                // Rectify the ground plane at editing pitch so every real rail meets its fixed UITK cell.
                // The elevated geometry still has perspective. The rectification eases away for the ride.
                float amount=1-Mathf.InverseLerp(12,40,tilt);
                var shear=Matrix4x4.identity;float rad=tilt*Mathf.Deg2Rad;
                shear.m21=Mathf.Tan(rad)*amount;shear.m11=Mathf.Lerp(1,1/Mathf.Cos(rad),amount);
                projection*=shear;
            }
            transform.SetPositionAndRotation(center-orientation*Vector3.forward*distance,orientation);
            lens.aspect=aspect;
            lens.fieldOfView=fieldOfView;
            lens.projectionMatrix=projection;
            lens.backgroundColor=StpKit.H(editing?0xC7BA9A:district==1?0xE6C99D:0xAED8E3);
        }

        private void BuildConsist(StpVehicle stock,bool covered)
        {
            float total=StpKit.Length(stock.Type)+stock.Wagons.Length*(StpKit.WagonLength+.06f);
            float x=total*.5f-StpKit.Length(stock.Type)*.5f;
            var loco=Place("Triebfahrzeug",b=>StpKit.Loco(b,stock.Type,stock.Body),x,0,.019f);
            cars.Add(loco.transform);
            x-=StpKit.Length(stock.Type)*.5f+StpKit.WagonLength*.5f+.06f;
            foreach(int kind in stock.Wagons)
            {
                cars.Add(Place("Wagen "+kind,b=>StpKit.Wagon(b,kind,stock.Body),x,0,.019f).transform);x-=StpKit.WagonLength+.06f;
            }
            if(covered)Place("Abdeckplane",b=>b.Ellipsoid(new Vector3(0,0,.35f),new Vector3(total*.54f,.32f,.39f),.38f,StpKit.H(0x647F86)));
            if(sceneName=="Office"||sceneName=="Profile"||sceneName=="Map"||sceneName=="Revision")foreach(var car in cars)car.gameObject.SetActive(false);
            if(sceneName=="Platform")specialTrain=loco.transform;
        }

        internal void TrainPose(int index,Vector2 point,float angle)
        {
            if(puzzle==null||index>=cars.Count)return;
            if(!riding){riding=true;rideStart=Time.unscaledTime;}
            var car=cars[index];car.gameObject.SetActive(true);
            car.localPosition=new Vector3(point.x,.019f,-point.y);
            car.localRotation=Quaternion.Euler(0,angle,0);
        }
        internal void EndRide()
        {
            if(!riding)return;
            riding=false;foreach(var car in cars)car.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            float now=Time.unscaledTime;
            for(int i=motions.Count-1;i>=0;i--)
            {
                var motion=motions[i];float t=Mathf.Clamp01((now-motion.Start)/.15f);
                if(motion.Piece!=null)
                {
                    var tr=motion.Piece.transform;
                    if(motion.Removing)tr.localScale=Vector3.one*(1-t);
                    else{var p=tr.localPosition;p.y=.3f*(1-t)*(1-t);tr.localPosition=p;tr.localScale=Vector3.one*(1+.08f*Mathf.Sin(Mathf.PI*t));}
                    if(t>=1&&motion.Removing)Destroy(motion.Piece);
                }
                if(t>=1)motions.RemoveAt(i);
            }
            if(!reduced){AnimateStation(now-arrived);AnimateEffects(now);}
        }
    }
}
