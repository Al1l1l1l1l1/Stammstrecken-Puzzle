/* Original deterministic material-cue synthesis. No recordings, samples, speech, jingles or external libraries. */
const fs=require("fs"),path=require("path"),crypto=require("crypto");
const check=process.argv.includes("--check"),destination=process.argv.slice(2).find(value=>value!=="--check");
const workspace=process.cwd(),out=path.resolve(workspace,destination||"Assets/StammstreckenPuzzle/Audio/Cues");
const relative=path.relative(workspace,out);if(relative.startsWith("..")||path.isAbsolute(relative))throw Error("Output must stay in workspace");
if(!check)fs.mkdirSync(out,{recursive:true});const rate=48000;
function write(file,data){if(check){if(!fs.existsSync(file)||!fs.readFileSync(file).equals(Buffer.isBuffer(data)?data:Buffer.from(data)))throw Error("Audio source drift: "+file);}else fs.writeFileSync(file,data);}
const definitions=[
 ["ui-tool-focus",.065,.085,"UI","paper"],["grid-empty-mark",.115,.1,"SFX","graphite"],
 ["grid-occupancy-note",.105,.1,"SFX","paper"],["grid-track-seat",.145,.14,"SFX","seat"],
 ["grid-clear",.095,.075,"UI","brush"],["ui-undo",.15,.09,"UI","brush"],
 ["completion-route-reveal",.31,.14,"SFX","unlatch"],["train-start",.45,.11,"SFX","motor-start"],
 ["train-roll",.8,.09,"Ambience","roll"],["train-arrive",.34,.13,"SFX","arrival"],
 ["results-panel-open",.16,.1,"UI","paper"],["completion-clearance",.22,.1,"SFX","brush"],
 ["ambience-scene-open",4,.035,"Ambience","air"],["music-workshop",12,.025,"Music","music"],["train-curve",.14,.07,"SFX","graphite"]
];
const manifest={version:1,approvalStatus:"DRAFT",origin:"Original deterministic synthesis for Stammstrecken-Puzzle WP046",source:"Content/Assets/build-product-audio.cjs",sourceSha256:crypto.createHash("sha256").update(fs.readFileSync(__filename)).digest("hex"),sampleRate:rate,channels:1,bitsPerSample:24,externalSources:[],rightsNote:"Original waveform synthesis authored in repository; no external samples, recordings, spoken announcements or copied jingles. No legal or hearing approval is asserted.",importPreset:"stp-audio-pcm-v1",cues:[]};
for(let index=0;index<definitions.length;index++){
 const[id,duration,peak,group,kind]=definitions[index],frames=Math.round(rate*duration),values=new Float64Array(frames);
 let seed=0x78126341+index*731,low=0,max=0,sum=0;
 function noise(){seed^=seed<<13;seed^=seed>>>17;seed^=seed<<5;return(seed>>>0)/2147483648-1;}
 for(let i=0;i<frames;i++){
  const t=i/rate,u=i/(frames-1),white=noise();low=.73*low+.27*white;
  const edges=Math.min(1,t/.004,(duration-t)/.012),envelope=Math.max(0,edges);
  let sample=0;
  if(kind==="paper")sample=(.8*low+.18*white)*Math.exp(-t/Math.max(.018,duration*.22));
  if(kind==="graphite")sample=(.8*low+.2*white)*Math.sin(Math.PI*u)**1.8*(.7+.3*Math.sin(t*127));
  if(kind==="brush")sample=low*Math.sin(Math.PI*u)**1.4*(.75+.25*Math.cos(t*151));
  if(kind==="seat")sample=.44*Math.sin(2*Math.PI*711*t)*Math.exp(-t/.028)+.14*Math.sin(2*Math.PI*1223*t)*Math.exp(-t/.019)+.42*low*Math.exp(-t/.04);
  if(kind==="unlatch")sample=.34*Math.sin(2*Math.PI*(410*t+90*t*t))*Math.exp(-t/.1)+.48*low*Math.exp(-t/.08);
  if(kind==="motor-start")sample=(.48*Math.sin(2*Math.PI*(74*t+62*t*t))+.16*Math.sin(2*Math.PI*(148*t+124*t*t))+.16*low)*Math.sin(Math.PI*u)**.7;
  if(kind==="roll")sample=.32*Math.sin(2*Math.PI*100*t)+.12*Math.sin(2*Math.PI*200*t)+.3*low*(.25+.75*Math.max(0,Math.cos(2*Math.PI*10*t))**9);
  if(kind==="arrival")sample=.24*Math.sin(2*Math.PI*(152*t-90*t*t))*Math.exp(-t/.14)+.45*low*Math.exp(-t/.08);
  if(kind==="air")sample=(.6*low+.13*Math.sin(2*Math.PI*81*t))*(.65+.2*Math.cos(2*Math.PI*t/duration));
  if(kind==="music")sample=(Math.sin(2*Math.PI*146.8323839587*t)+.44*Math.sin(2*Math.PI*220*t)+.24*Math.sin(2*Math.PI*261.6255653006*t)+.2*Math.sin(2*Math.PI*329.6275569129*t))*(.5-.5*Math.cos(2*Math.PI*u));
  values[i]=sample*envelope;max=Math.max(max,Math.abs(values[i]));
 }
 // Documented equal-power crossfade makes each synthetic ambient loop periodic, with matching endpoints.
 const looping=kind==="roll"||kind==="air"||kind==="music",crossfade=looping?Math.min(Math.round(rate*.04),Math.floor(frames/8)):0;
 if(crossfade){for(let i=0;i<crossfade;i++){const phase=i/(crossfade-1)*Math.PI*.5;const mixed=values[i]*Math.sin(phase)+values[frames-crossfade+i]*Math.cos(phase);values[i]=mixed;values[frames-crossfade+i]=mixed;}values[frames-1]=values[0];max=0;for(const value of values)max=Math.max(max,Math.abs(value));}
 const bytes=Buffer.alloc(44+frames*3);bytes.write("RIFF",0);bytes.writeUInt32LE(bytes.length-8,4);bytes.write("WAVEfmt ",8);bytes.writeUInt32LE(16,16);bytes.writeUInt16LE(1,20);bytes.writeUInt16LE(1,22);bytes.writeUInt32LE(rate,24);bytes.writeUInt32LE(rate*3,28);bytes.writeUInt16LE(3,32);bytes.writeUInt16LE(24,34);bytes.write("data",36);bytes.writeUInt32LE(frames*3,40);
 let measuredPeak=0;for(let i=0;i<frames;i++){const value=values[i]/max*peak;measuredPeak=Math.max(measuredPeak,Math.abs(value));sum+=value*value;let integer=Math.round(value*8388607);if(integer<0)integer+=16777216;bytes[44+i*3]=integer&255;bytes[45+i*3]=(integer>>>8)&255;bytes[46+i*3]=(integer>>>16)&255;}
 const file=id+".wav";write(path.join(out,file),bytes);
 manifest.cues.push({cueId:"stp/audio/"+id,file,group,durationSeconds:frames/rate,loop:looping,loopStartSample:looping?0:null,loopEndSampleExclusive:looping?frames:null,crossfadeSamples:crossfade,peak:measuredPeak,rms:Math.sqrt(sum/frames),sha256:crypto.createHash("sha256").update(bytes).digest("hex")});
}
write(path.join(out,"audio-provenance-v1.json"),JSON.stringify(manifest,null,2)+"\n");
console.log(JSON.stringify({check,cues:manifest.cues.length,sampleRate:rate,bits:24,maximumPeak:Math.max(...manifest.cues.map(c=>c.peak)),output:relative}));
