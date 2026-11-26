using System;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Presentation.World
{
    public sealed partial class StpWorldCamera
    {
        private static Color Tone(int rgb)=>StpKit.H(rgb);
        private float groupOrigin;
        private bool rareVisit;
        private void BuildStation(string name,int world,IReadOnlyList<string> props,int visits,bool rare)
        {
            rareVisit=rare;
            Place("Gelände",b=>b.Box(new Vector3(0,0,-.41f),new Vector3(36,36,.35f),
                Tone(name=="Office"||name=="Profile"?0xA87656:name=="Depot"?0xB8B2A3:world==1?0xC4A67F:0x9EB77E)));
            if(name=="Finale"){BuildFinale();return;}
            if(name=="Office"||name=="Profile"){BuildOffice(name=="Profile");return;}
            if(name=="Revision"){Place("Aktenschrank",b=>{
                for(int i=0;i<5;i++){var c=new Vector3(0,-.8f,.25f+i*.46f);b.Box(c,new Vector3(1.66f,.52f,.215f),Tone(0x789693));
                    b.Box(c+new Vector3(0,.54f,0),new Vector3(.34f,.05f,.048f),Tone(0xC3AC75));}
                });return;}
            if(name=="Map"){BuildDesk();return;}
            if(name=="Depot")
            {
                Place("Werkhalle",b=>{
                    b.Box(new Vector3(0,-2,1),new Vector3(4,.13f,1.05f),Tone(0x799392));
                    for(int i=-3;i<=3;i++){b.Box(new Vector3(i,-1.84f,1.25f),new Vector3(.39f,.035f,.37f),Tone(0xBFE8EF));}
                    for(int i=-1;i<=1;i+=2)b.Box(new Vector3(i*2.65f,-1.42f,1.23f),new Vector3(.10f,.12f,1.23f),Tone(0xCB8E43));
                    b.Box(new Vector3(0,-1.42f,2.38f),new Vector3(2.87f,.13f,.12f),Tone(0xDBAD55));
                    b.Cylinder(new Vector3(0,0,-.01f),2.22f,.07f,2,Tone(0x6F7C79),-1,false,64);
                    b.Cylinder(new Vector3(0,0,.045f),2.1f,.035f,2,Tone(0xB5AC90),-1,false,64);
                    for(int j=-2;j<2;j++)StpKit.TrackRails(b,1,j,0);
                },0,-.48f);
                if(props.Count==1)Place("Werkstück "+props[0],b=>StpKit.Module(b,props[0],0,0,2),0,1.15f,.1f);
                AddCommuter(1.85f,.65f,2);
                Place("Gerds Werkzeug",b=>{b.Box(new Vector3(0,0,.43f),new Vector3(.038f,.04f,.17f),Tone(0xBAC6C5),.3f);
                    b.Cylinder(new Vector3(-.32f,0,.35f),.075f,.085f,2,Tone(0xF0E6CD));},2,.65f);
                return;
            }
            Place("Bahnsteig",b=>{
                for(int i=-6;i<6;i++){StpKit.TrackBallast(b,1,i,0);StpKit.TrackRails(b,1,i,0);}
                b.Box(new Vector3(0,-.98f,.085f),new Vector3(6.4f,.92f,.12f),Tone(0xCECAB9));
                b.Box(new Vector3(0,-.19f,.215f),new Vector3(6.4f,.046f,.012f),Tone(0xFFF0C9));
                for(int i=-5;i<=5;i+=2){b.Box(new Vector3(i,-1.65f,1.05f),new Vector3(.07f,.09f,.86f),Tone(0x37647A));}
                b.Box(new Vector3(0,-1.68f,1.95f),new Vector3(5.8f,.53f,.09f),Tone(0x46778A));
                for(int i=-3;i<=3;i++)StpKit.Tree(b,i*1.85f,-4.5f,1.3f+(i&1)*.3f,i&1);
            },0,-.5f);
            BuildDistrict(name=="Endless"?1:world,visits);
            if(name=="Endless")return;
            var gantry=Place("Abfahrtsanzeiger",b=>b.Ellipsoid(new Vector3(0,0,0),new Vector3(1.32f,.09f,.28f),.32f,Tone(0x0F2A57)),.35f,-1.86f,1.66f);
            stationIndicator=Lettering("",gantry.transform.localPosition+new Vector3(0,0,-.102f),.034f,Color.white);
            Place("Aufzug",b=>{
                b.Box(new Vector3(0,0,.63f),new Vector3(.45f,.32f,.64f),Tone(0x536776));
                for(int i=-1;i<=1;i+=2)b.Box(new Vector3(i*.19f,.33f,.56f),new Vector3(.17f,.03f,.53f),Tone(0x9BAAAF));
                b.Box(new Vector3(0,.37f,.82f),new Vector3(.38f,.025f,.10f),Color.Lerp(Tone(0xF5DDAA),Tone(0xA09070),Mathf.Min(.6f,visits*.01f)));
            },-2.9f,-1.05f,.20f);
            Place("Fahrdraht",b=>{
                for(int i=-1;i<=1;i+=2)b.Cylinder(new Vector3(i*4.2f,0,.9f),.045f,.9f,2,Tone(0x515A60));
                b.Box(new Vector3(0,0,1.76f),new Vector3(4.2f,.014f,.014f),Tone(0x434E58));
            },0,1.12f);
            AddBird(-1.9f,1.12f,1.78f);AddBird(-1.58f,1.12f,1.78f);
            groupOrigin=-1.25f;
            for(int i=0;i<5;i++)AddCommuter(groupOrigin+i*.42f,-1.12f+(i%2)*.22f,i%3);
            AddClock(2.65f,-1.15f);
            string[] slots={"bench","wayfinding","barrier","indicator","platform","tunnel","sev-stop","signal","noticeboard"};
            foreach(var prop in props)
            {
                int slot=Array.IndexOf(slots,prop);if(slot<0)continue;
                Place("Ausstattung "+prop,b=>StpKit.Module(b,prop,0,0,1.2f),-3.6f+slot%5*1.65f,2.0f+slot/5*.95f);
            }
        }
        private void BuildDistrict(int world,int visits)
        {
            Place("Streckenabschnitt "+world,b=>{
                switch(world)
                {
                    case 0:
                        StpKit.House(b,-2,-3.25f,1.2f,.85f,1.72f,Tone(0xEACCA5),Tone(0xA55445));
                        StpKit.House(b,.1f,-3.4f,1.4f,.8f,1.3f,Tone(0xF0DAAA),Tone(0x535E75));
                        StpKit.Tunnel(b,2.9f,-3.3f,2.1f);break;
                    case 1:
                        b.Ellipsoid(new Vector3(0,-3,.01f),new Vector3(2.3f,.75f,.06f),.7f,Tone(0x836A50));
                        StpKit.Shed(b,3,-3,1.6f);
                        for(int i=-2;i<3;i++)StpKit.Barrier(b,i*.9f+(visits%3)*.13f,-2.45f,1);
                        b.Frame=StpFrame.Identity;
                        b.Ellipsoid(new Vector3(-.8f,-3.15f,.29f),new Vector3(.55f,.33f,.23f),.35f,Tone(0xEDAE40));
                        b.Box(new Vector3(-.54f,-3.16f,.68f),new Vector3(.21f,.24f,.21f),Tone(0x547281));
                        b.Box(new Vector3(-1.24f,-3.15f,.83f),new Vector3(.09f,.08f,.55f),Tone(0xE9AB37));break;
                    case 2:
                        StpKit.Bench(b,-1,-2.65f,2);StpKit.Sign(b,.25f,-2.6f,2,0);StpKit.Tree(b,2,-3.4f,2,1);break;
                    case 3:
                        StpKit.Bridge(b,-1,-3.35f,2.2f);for(int i=0;i<3;i++)StpKit.Sign(b,1.7f+i*.45f,-2.8f,1.5f,i);break;
                    default:
                        for(int i=-1;i<=1;i++){b.Frame=new StpFrame(new Vector3(i*1.72f,-3.1f,0),0,1);
                            b.Ellipsoid(new Vector3(0,0,.4f),new Vector3(.78f,.29f,.32f),.3f,Tone(0xEAB85C));
                            b.Box(new Vector3(0,.28f,.50f),new Vector3(.65f,.02f,.14f),Tone(0x406074));
                            for(int j=-1;j<=1;j+=2)b.Cylinder(new Vector3(j*.48f,.24f,.15f),.14f,.07f,1,Tone(0x40434B));}
                        StpKit.StopSign(b,3,-2.5f,1.9f);break;
                }
            });
        }
        private void BuildOffice(bool badge)
        {
            Place("Baubüro",b=>{
                b.Box(new Vector3(0,-2,1.8f),new Vector3(4,.12f,1.8f),Tone(0xE9D9B8));
                b.Box(new Vector3(0,-1.84f,1.8f),new Vector3(2.3f,.1f,1.15f),Tone(0x517077));
                b.Box(new Vector3(0,-1.72f,1.8f),new Vector3(2.15f,.04f,1f),Tone(0xADDBDF));
                // The model building site is visible through the mullions of the office window.
                for(int i=0;i<5;i++){
                    float x=-1.72f+i*.84f, height=.32f+(i%3)*.16f;
                    b.Box(new Vector3(x,-1.63f,1.05f+height),new Vector3(.35f,.022f,height),Tone(i%2==0?0x9BBAC0:0x82A8B1));
                    for(int j=0;j<3;j++)b.Box(new Vector3(x-.2f+j*.19f,-1.59f,1.28f),new Vector3(.04f,.012f,.066f),Tone(0xD5E3D8));
                }
                b.Box(new Vector3(.72f,-1.58f,1.92f),new Vector3(.05f,.035f,.68f),Tone(0xE7AA4E));
                b.Box(new Vector3(.24f,-1.57f,2.55f),new Vector3(.85f,.045f,.045f),Tone(0xD69537));
                b.Box(new Vector3(-.30f,-1.55f,2.31f),new Vector3(.009f,.014f,.21f),Tone(0x57686B));
                foreach(float x in new[]{-1.05f,1.05f})b.Box(new Vector3(x,-1.50f,1.8f),new Vector3(.032f,.04f,1.02f),Tone(0xF2E3C9));
                StpKit.Shed(b,-1,-1.46f,1.2f);StpKit.Barrier(b,.85f,-1.44f,1.5f);
                b.Frame=StpFrame.Identity;
                b.Ellipsoid(new Vector3(0,.65f,.18f),new Vector3(3.5f,1.4f,.2f),.2f,Tone(0xA8724D));
                b.Ellipsoid(new Vector3(-.25f,.55f,.44f),new Vector3(1.2f,.78f,.07f),.26f,Tone(badge?0xDAD8BD:0x24364C),-.08f);
                b.Ellipsoid(new Vector3(-.25f,.55f,.51f),new Vector3(1.06f,.66f,.016f),.24f,Tone(badge?0xABD5CE:0x638FA4),-.08f);
                b.Box(new Vector3(-.3f,.55f,.54f),new Vector3(.84f,.48f,.015f),Tone(badge?0xEFDFB9:0xA5CBD0),-.08f);
                for(int i=0;i<4;i++)b.Box(new Vector3(-.3f,.25f+i*.18f,.561f),new Vector3(.69f,.010f,.005f),Tone(0xEEF0D3),-.08f);
                for(int i=0;i<3;i++)b.Box(new Vector3(-.79f+i*.45f,.54f,.564f),new Vector3(.011f,.42f,.004f),Tone(0xEEF0D3),-.08f);
                b.Box(new Vector3(-.39f,.53f,.573f),new Vector3(.52f,.024f,.005f),Tone(0xA35444),-.27f);
                for(int i=0;i<3;i++)b.Cylinder(new Vector3(-.83f+i*.44f,.64f-i*.12f,.58f),.05f,.007f,2,Tone(0xF0D488));
                b.Box(new Vector3(.82f,1.03f,.442f),new Vector3(.032f,.34f,.03f),Tone(0xC14A41),.39f);
                b.Cylinder(new Vector3(1.42f,.28f,.59f),.23f,.23f,2,Tone(0xEEE4C7));
                b.Cylinder(new Vector3(1.42f,.28f,.823f),.196f,.006f,2,Tone(0x5B443D));
            });
        }
        private void BuildDesk()
        {
            Place("Gleisbildpult",b=>{
                b.Ellipsoid(new Vector3(0,0,0),new Vector3(4.8f,2.3f,.34f),.22f,Tone(0x385566));
                for(int k=0;k<5;k++)for(int j=0;j<4;j++){
                    float x=-3.4f+j*2.1f, y=-1.2f+k*.59f;
                    b.Box(new Vector3(x+.8f,y,.35f),new Vector3(.96f,.037f,.024f),Tone(k<=district?0xEDC468:0x72848C));
                    b.Cylinder(new Vector3(x,y,.39f),.105f,.03f,2,Tone(k==district?0xEA5963:k<district?0xFFE493:0x637383));
                }
            });
        }
        private void AddBird(float x,float y,float z=.25f)
        {
            var bird=Place("Taube",b=>{
                b.Ellipsoid(new Vector3(0,0,.08f),new Vector3(.12f,.07f,.08f),.8f,Tone(0x8795A0));
                b.Ellipsoid(new Vector3(.09f,0,.16f),new Vector3(.067f,.06f,.066f),.9f,Tone(0x718493));
                b.Box(new Vector3(.16f,0,.15f),new Vector3(.035f,.027f,.017f),Tone(0xC99A74));
            },x,y,z);
            pigeons.Add(bird.transform);
        }
        private void AddCommuter(float x,float y,int type)
        {
            var person=Place("Pendler "+type,b=>{
                var coat=Tone(type==2?0x2F7176:type==1?0xD7974A:0xAF5864);
                for(int j=-1;j<=1;j+=2){
                    b.Ellipsoid(new Vector3(j*.073f,0,.095f),new Vector3(.066f,.073f,.11f),.55f,Tone(0x384856));
                    b.Ellipsoid(new Vector3(j*.155f,0,.32f),new Vector3(.055f,.07f,.13f),.72f,coat);}
                b.Ellipsoid(new Vector3(0,0,.32f),new Vector3(.145f,.11f,.18f),.58f,coat);
                b.Ellipsoid(new Vector3(0,0,.57f),new Vector3(.129f,.116f,.14f),.75f,Tone(type==1?0xA7775B:0xE4B493));
                b.Ellipsoid(new Vector3(0,-.013f,.66f),new Vector3(.13f,.115f,.066f),.58f,Tone(0x584B47));
                for(int j=-1;j<=1;j+=2)b.Ellipsoid(new Vector3(j*.045f,.109f,.576f),new Vector3(.011f,.012f,.015f),1,Tone(0x3A393B),0,false,6,4);
                b.Box(new Vector3(.14f,.07f,.36f),new Vector3(.04f,.018f,.064f),Tone(0x39454F));
            },x,y,puzzle==null&&sceneName!="Depot"?.22f:0);
            passengers.Add(person.transform);
        }
        private void AddClock(float x,float y)
        {
            Place("Bahnsteiguhr",b=>{
                b.Cylinder(new Vector3(0,0,.68f),.05f,.68f,2,Tone(0x647978));
                b.Cylinder(new Vector3(0,0,1.48f),.32f,.08f,1,Tone(0x374D59));
                b.Cylinder(new Vector3(0,.085f,1.48f),.284f,.014f,1,Tone(0xFAEDD6));
                for(int i=0;i<12;i++){float a=i*Mathf.PI/6;b.Box(new Vector3(.244f*Mathf.Sin(a),.103f,1.48f+.244f*Mathf.Cos(a)),new Vector3(.012f,.01f,.016f),Tone(0x344D58));}
            },x,y);
            for(int i=0;i<2;i++){
                float len=i==0?.13f:.21f;clockHands.Add(Place("Zeiger "+i,b=>b.Box(new Vector3(0,0,len*.5f),new Vector3(.014f,.01f,len*.5f),Tone(0x293F4F)),x,y+.12f,1.48f).transform);}
        }
        private void AnimateStation(float t)
        {
            if(clockHands.Count==2){
                var time=DateTime.Now.AddMinutes(-3);clockHands[0].localRotation=Quaternion.Euler(0,0,-30*(time.Hour+time.Minute/60f));
                clockHands[1].localRotation=Quaternion.Euler(0,0,-6*time.Minute);}
            if(sceneName=="Platform"){
                float walk=Mathf.SmoothStep(0,1,Mathf.PingPong(t*.11f,1));
                for(int i=0;i<passengers.Count;i++){var p=passengers[i].localPosition;p.x=groupOrigin+i*.42f+walk*1.7f;p.y=.22f+.018f*Mathf.Abs(Mathf.Sin(t*5+i));passengers[i].localPosition=p;}
                if(rareVisit&&specialTrain!=null){float slide=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-1)/5))*12;foreach(var car in cars){var p=car.localPosition;p.x+=slide-lastSlide;car.localPosition=p;}lastSlide=slide;}
            }else if(riding){
                float passed=Time.unscaledTime-rideStart;
                for(int i=0;i<pigeons.Count;i++){var p=pigeons[i].localPosition;p.y=.25f+Mathf.Clamp(passed-i*.15f,0,1)*1.7f;pigeons[i].localPosition=p;}
                if(passed>2.8f)foreach(var person in passengers)person.localRotation=Quaternion.Euler(0,0,4*Mathf.Sin(passed*14));
            }
        }
        private float lastSlide;
    }
}
