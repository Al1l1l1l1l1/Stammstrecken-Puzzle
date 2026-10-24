/* Original authoring utility for WP046. Reuses original reference IDs; never exports authoring solutions. */
const fs = require("fs");
const cp = require("child_process");
const crypto = require("crypto");
const sourceCommit = "dc1652befd7545b4c2a2dd0580f35a4e8e05d72d";
const sourcePath = "Content/Assets/railway-assets-v1.json";
const runtimePath = "Assets/StammstreckenPuzzle/UI/RailwayAssets.json";
const paths = cp.execFileSync("git",["ls-tree","-r","--name-only",sourceCommit,"Content/Levels"],{encoding:"utf8"}).trim().split(/\r?\n/).filter(p=>p.endsWith(".level-v2.json")).sort();
if(paths.length!==240)throw Error("Expected exactly 240 original source levels");
const moduleSets = [
 [["tunnel",347,155,.5],["platform",89,137,.35],["signal",219,140,.62]],
 [["signal",327,161,.9],["indicator",102,135,.45],["bench",349,193,.53]],
 [["wayfinding",331,157,.8],["bench",83,139,.4],["platform",227,139,.38]],
 [["tunnel",337,152,.52],["indicator",218,138,.48],["platform",87,143,.42]],
 [["barrier",324,178,.8],["platform",111,145,.36],["wayfinding",217,141,.45]],
 [["barrier",343,178,.62],["signal",232,139,.6],["bench",101,141,.4]],
 [["indicator",329,154,.8],["noticeboard",214,142,.42],["barrier",87,143,.36]],
 [["platform",328,166,.8],["wayfinding",217,139,.5],["barrier",87,143,.36]],
 [["platform",337,176,.75],["wayfinding",217,141,.5],["bench",97,140,.4]],
 [["depot",330,155,.6],["indicator",217,142,.4],["bench",92,141,.4]],
 [["depot",337,157,.6],["signal",218,142,.5],["platform",88,141,.38]],
 [["wayfinding",336,159,.8],["bench",214,140,.4],["indicator",93,140,.4]],
 [["bridge",324,152,.67],["wayfinding",215,140,.45],["platform",90,142,.4]],
 [["bridge",325,152,.62],["bench",217,141,.4],["indicator",97,141,.4]],
 [["wayfinding",337,157,.75],["noticeboard",215,142,.46],["bridge",95,139,.4]],
 [["bridge",329,155,.7],["wayfinding",216,141,.5],["signal",91,142,.45]],
 [["sev-stop",328,174,.85],["wayfinding",217,143,.5],["barrier",91,143,.4]],
 [["sev-stop",330,173,.9],["bench",218,141,.48],["indicator",91,143,.4]],
 [["sev-stop",332,174,.8],["wayfinding",218,143,.5],["platform",93,141,.42]],
 [["noticeboard",333,168,.77],["sev-stop",217,140,.45],["signal",91,143,.5]]
];
const worlds=Array.from({length:5},(_,i)=>({id:"s1-world-"+String(i+1).padStart(2,"0"),section:i+1,rendererWorld:i}));
const routes=moduleSets.map((set,i)=>({id:"s1-route-"+String(Math.floor(i/4)+1).padStart(2,"0")+"-"+String(i%4+1).padStart(2,"0"),worldId:worlds[Math.floor(i/4)].id,section:Math.floor(i/4)+1,route:i%4+1,modules:set.map(([kind,x,y,scale])=>({kind,x,y,scale}))}));
const moments=[];
for(const path of paths){
 const bytes=cp.execFileSync("git",["show",sourceCommit+":"+path]);
 const level=JSON.parse(bytes.toString("utf8"));
 const c=level.content,compositionId=routes[(c.networkSection-1)*4+c.route-1].id;
 for(const [kind,key] of [["MAP","mapSegmentId"],["TRAIN","trainMomentId"]]){
  const referenceId=level.completion[key];
  if(!referenceId||!/^draft-s1-(?:[0-9]{2}-){2}[0-9]{2}-(?:map|train)$/.test(referenceId))throw Error("Invalid original reference "+path);
  moments.push({referenceId,puzzleId:level.puzzleId,kind,assetAddress:"stp/moments/"+referenceId,compositionId,stage:c.position});
 }
}
moments.sort((a,b)=>a.referenceId.localeCompare(b.referenceId,"en"));
const catalog=JSON.parse(fs.readFileSync("Content/Catalogs/s1/cosmetics-v2.json","utf8"));
if(catalog.items.length!==14)throw Error("Expected 14 cosmetics");
const cosmetics=catalog.items.map(item=>({id:item.id,assetAddress:item.assetAddress,titleKey:item.titleKey,rendererKey:item.id})).sort((a,b)=>a.id.localeCompare(b.id,"en"));
const reusableMoments=[{referenceId:"draft-endless-map-default",puzzleId:"",kind:"MAP",assetAddress:"stp/moments/draft-endless-map-default",compositionId:"s1-route-05-04",stage:0},{referenceId:"draft-endless-train-default",puzzleId:"",kind:"TRAIN",assetAddress:"stp/moments/draft-endless-train-default",compositionId:"s1-route-05-04",stage:0}];
const manifest={schemaVersion:1,rendererVersion:"railway-cartoon-1",approvalStatus:"DRAFT",referenceSourceCommit:sourceCommit,styleSourceCommit:"9e3f209cd92d491ab3a1ab51cce63677d3b5d53e",origin:"Original code-native cartoon constructions in RailwayIllustrationElement and RailwayCosmeticElement; IBM Plex font license is recorded separately.",runtimeContract:"Presentation references only. No puzzle solution, reward, ownership, product approval or release authority.",worlds,routes,cosmetics,moments,reusableMoments};
const text=JSON.stringify(manifest,null,2)+"\n";
if(process.argv.includes("--check")){
 if(fs.readFileSync(sourcePath,"utf8")!==text||fs.readFileSync(runtimePath,"utf8")!==text)throw Error("Asset projection differs");
}else{
 fs.mkdirSync("Content/Assets",{recursive:true});fs.writeFileSync(sourcePath,text);fs.writeFileSync(runtimePath,text);
}
console.log(JSON.stringify({worlds:worlds.length,routes:routes.length,cosmetics:cosmetics.length,mapReferences:moments.filter(m=>m.kind==="MAP").length,trainReferences:moments.filter(m=>m.kind==="TRAIN").length,sha256:crypto.createHash("sha256").update(text).digest("hex"),check:process.argv.includes("--check")}));
