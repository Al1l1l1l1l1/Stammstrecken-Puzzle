using System;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Presentation.World
{
    public sealed partial class StpWorldCamera
    {
        private TextMesh? yearSign,previousYears,stationIndicator;
        private string cachedYear="";
        private GameObject? highlight;
        private Transform? dust;
        private float dustStart;
        private Transform? turntable;
        private float turnStart;
        private bool reversedOrder;
        private readonly List<Transform> confetti=new List<Transform>();
        private TextMesh Lettering(string text,Vector3 location,float size,Color ink)
        {
            var holder=new GameObject("Beschriftung"){layer=29};
            holder.transform.SetParent(stage,false);holder.transform.localPosition=location;
            holder.transform.localRotation=Quaternion.Euler(sceneName=="Puzzle"?74:24,0,0);
            var lettering=holder.AddComponent<TextMesh>();
            lettering.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            holder.GetComponent<MeshRenderer>().sharedMaterial=lettering.font.material;
            lettering.anchor=TextAnchor.MiddleCenter;lettering.alignment=TextAlignment.Center;
            lettering.fontSize=52;lettering.characterSize=size;lettering.color=ink;lettering.text=text;return lettering;
        }
        private void BuildDeadline(float x,float y)
        {
            float pitch=sceneName=="Puzzle"?74:24;
            var post=Place("Bautafel",b=>{
                foreach(float side in new[]{-.55f,.55f})b.Box(new Vector3(side,0,.34f),new Vector3(.034f,.04f,.34f),Tone(0x746E62));
            },x,y);
            var center=post.transform.localPosition+Vector3.up*.91f;
            var face=Place("Baustellenschild",b=>{
                b.Box(Vector3.zero,new Vector3(.91f,.036f,.41f),Tone(0xF7ECCD));
                b.Box(new Vector3(-.84f,.042f,0),new Vector3(.051f,.012f,.4f),Tone(0xFF6A13));
                b.Box(new Vector3(.74f,.048f,.28f),new Vector3(.13f,.018f,.11f),Tone(0xF3D177),-.11f);
            });face.transform.localPosition=center;face.transform.localRotation=Quaternion.Euler(pitch,0,0);
            Vector3 TextAt(float yOffset,float xOffset=0)=>center+face.transform.localRotation*new Vector3(xOffset,yOffset,-.057f);
            previousYears=Lettering("",TextAt(.11f),.0185f,Tone(0x5E615C));
            yearSign=Lettering("",TextAt(-.13f),.024f,Tone(0x243A46));
            Lettering("?",TextAt(.27f,.73f),.045f,Tone(0x30383D));
            var strike=Place("Überholte Planung",b=>b.Box(Vector3.zero,new Vector3(.50f,.009f,.011f),Tone(0xB34B38)));
            strike.transform.localPosition=TextAt(.09f)-face.transform.forward*.012f;strike.transform.localRotation=face.transform.localRotation;
            cachedYear="";
        }
        internal void UpdateSign(string title,string year,int value,string line)
        {
            if(yearSign!=null&&previousYears!=null && cachedYear!=year){
                cachedYear=year;yearSign.text=title+"\n"+year;
                previousYears.text=$"{value-10}   {value-8}   {value-6}   {value-4}   {value-2}";
            }
            if(stationIndicator!=null&&stationIndicator.text!=line)stationIndicator.text=line;
        }
        private void Dust(float x,float y)
        {
            if(dust==null)dust=Place("Schotterstaub",b=>{
                for(int i=0;i<7;i++){float a=i*.90f;
                    b.Ellipsoid(new Vector3(.29f*Mathf.Cos(a),.29f*Mathf.Sin(a),.024f+(i%2)*.032f),
                        new Vector3(.09f,.08f,.075f),.92f,Tone(0xDCCFB5),0,false,6,4);}
            }).transform;
            dustStart=Time.unscaledTime;dust.localPosition=new Vector3(x+.5f,.045f,-y-.5f);dust.gameObject.SetActive(true);
        }
        private void Reveal(float elapsed)
        {
            if(puzzle==null||puzzle.BuiltPath.Count==0||reduced)return;
            int count=puzzle.BuiltPath.Count;
            int index=Mathf.FloorToInt(elapsed/Mathf.Min(.045f,.60f/count));
            if(index>=count){if(highlight!=null)highlight.SetActive(false);return;}
            var cell=puzzle.BuiltPath[index];
            if(highlight==null)highlight=Place("Freigabeglanz",b=>b.Ellipsoid(Vector3.zero,new Vector3(.2f,.2f,.04f),.68f,Tone(0xFFF9C4),0,false,10,4));
            highlight.transform.localPosition=new Vector3(cell.X+.5f,.145f,-cell.Y-.5f);
        }
        private void BuildFinale()
        {
            Place("Freigegebenes Netz",b=>{
                for(int j=-6;j<=6;j++){StpKit.TrackBallast(b,1,j,0);StpKit.TrackRails(b,1,j,0);}
                for(int i=0;i<5;i++){
                    float x=-3.8f+i*1.9f;StpKit.Platform(b,x,-.65f,1.48f,1.2f);StpKit.Signal(b,x+.65f,-1.12f,1.3f,true);
                    StpKit.House(b,x,-2.5f,1.22f,.7f,.80f+i*.14f,Tone(0xE0CAA1),Tone(0x786C68+i*0x030400));
                }
            },0,-.5f);
            for(int i=0;i<15;i++){AddCommuter(-4+i*.56f,-1.13f+(i%3)*.16f,i%3);}
            for(int i=0;i<34;i++){int shade=i%3;
                var piece=Place("Konfetti "+shade,b=>b.Box(Vector3.zero,new Vector3(.047f,.013f,.035f),Tone(shade==0?0xF4CC67:shade==1?0xD95550:0x63A8BC)));
                piece.transform.localPosition=new Vector3(-4.3f+(i*37%86)*.1f,2.3f+(i%8)*.13f,(i%7)*.28f);confetti.Add(piece.transform);}
        }
        private void AnimateEffects(float now)
        {
            if(riding)Reveal(now-rideStart);
            if(dust!=null&&dust.gameObject.activeSelf){
                float t=Mathf.Clamp01((now-dustStart)/.24f);dust.localScale=new Vector3(.3f+t,.95f*(1-t),.3f+t);
                if(t>=1)dust.gameObject.SetActive(false);}
            if(turntable!=null){
                float t=Mathf.Clamp01((now-turnStart)/.32f);
                turntable.localRotation=Quaternion.Euler(0,Mathf.Lerp(-20,0,Mathf.SmoothStep(0,1,t)),0);
            }
            if(sceneName=="Finale"){
                float t=now-arrived;
                for(int i=0;i<confetti.Count;i++){
                    var tr=confetti[i];var p=tr.localPosition;p.y=3.5f-Mathf.Repeat(t*.8f+i*.21f,3.4f);
                    tr.localPosition=p;tr.localRotation=Quaternion.Euler(t*53+i,0,t*80+i*32);}
                float advance=Mathf.Lerp(-3.6f,5.6f,Mathf.Clamp01(t/4.4f));
                foreach(var car in cars){var p=car.localPosition;p.x+=advance-lastSlide;car.localPosition=p;}lastSlide=advance;
                foreach(var person in passengers)person.localRotation=Quaternion.Euler(0,0,4*Mathf.Sin(t*14));
            }
        }
    }
}
