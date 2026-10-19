'use strict';
// WP041: offline allowlist projection; existing strict JCS is the sole canonicalizer.
const fs=require('fs'), path=require('path'), cp=require('child_process'), crypto=require('crypto');
const root=path.resolve(__dirname,'../..');
const source='dc1652befd7545b4c2a2dd0580f35a4e8e05d72d';
const asset='Assets/StammstreckenPuzzle/UI/Public240.public.json';
const lock='PROJECT_CONTROL/ProductionBlocks/Public240Save/A_SOURCE_LOCK.json';
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const scratch=path.join(root,'.utmp/public240-project'); fs.mkdirSync(scratch,{recursive:true});
function git(args){return cp.execFileSync('git',args,{cwd:root,maxBuffer:16*1024*1024});}
function canonical(bytes){
 const file=path.join(scratch,'input.json'); fs.writeFileSync(file,bytes);
 const answer=JSON.parse(cp.execFileSync(process.execPath,[path.join(root,'tools/architecture-validation/jcs_crosscheck.mjs'),file],{cwd:root,encoding:'utf8',maxBuffer:16*1024*1024}));
 return {bytes:Buffer.from(answer.canonicalBase64,'base64'),hash:answer.sha256};
}
function generate(){
 const lines=git(['ls-tree','-r',source,'--','Content/Levels/s1']).toString('utf8').trim().split('\n').filter(x=>x.endsWith('.level-v2.json')).sort((a,b)=>a.split('\t')[1].localeCompare(b.split('\t')[1],'en'));
 if(lines.length!==240)throw Error('PUBLIC240_SOURCE_COUNT');
 const puzzles=[],sources=[];
 for(let i=0;i<240;i++){
  const match=/^100644 blob ([0-9a-f]{40})\t(Content\/Levels\/s1\/(\d{2})\/(\d{2})\/(S1-\d{2}-\d{2}-\d{2})\.level-v2\.json)$/.exec(lines[i]);
  if(!match)throw Error('PUBLIC240_SOURCE_PATH');
  const section=String(1+Math.floor(i/48)).padStart(2,'0'),route=String(1+Math.floor(i%48/12)).padStart(2,'0'),position=String(1+i%12).padStart(2,'0');
  const id=`S1-${section}-${route}-${position}`;
  if(match[3]!==section||match[4]!==route||match[5]!==id)throw Error('PUBLIC240_SOURCE_ORDER');
  const raw=git(['cat-file','blob',match[1]]);
  // The existing reference rejects duplicate keys, floats, unsafe integers and Unicode errors.
  const doc=JSON.parse(canonical(raw).bytes.toString('utf8'));
  if(doc.puzzleId!==id||doc.documentSchemaVersion!==2||doc.rulesetVersion!=='train-track-v1')throw Error('PUBLIC240_SOURCE_IDENTITY');
  const projection={puzzleId:doc.puzzleId,rulesetVersion:doc.rulesetVersion,
   grid:{width:doc.grid.width,height:doc.grid.height},
   endpoints:{a:{side:doc.endpoints.a.side,index:doc.endpoints.a.index},b:{side:doc.endpoints.b.side,index:doc.endpoints.b.index}},
   rowCounts:doc.rowCounts.slice(),columnCounts:doc.columnCounts.slice()};
  const pub=canonical(Buffer.from(JSON.stringify(projection)));
  puzzles.push({...projection,publicPuzzleHash:{profile:'STP-PUZZLE-SEMANTIC-JCS-1',sha256:pub.hash}});
  sources.push({puzzleId:id,path:match[2],blobId:match[1],sourceBytes:raw.length,sourceSha256:sha(raw),publicPuzzleSha256:pub.hash,projectionBytes:pub.bytes.length,projectionSha256:sha(pub.bytes)});
 }
 const payload={schemaVersion:1,catalogId:'qa-public240-v1-public',revision:1,use:'IsolatedReview',status:'DRAFT',puzzles};
 const publicPayload=canonical(Buffer.from(JSON.stringify(payload)));
 const output=Buffer.from(JSON.stringify({...payload,catalogHashSha256:publicPayload.hash},null,2)+'\n');
 const audit=Buffer.from(JSON.stringify({schemaVersion:1,sourceCommit:source,sourceCount:240,hashProfile:'STP-PUZZLE-SEMANTIC-JCS-1',jcsReference:'tools/architecture-validation/jcs_crosscheck.mjs',jcsReferenceByteProfile:'Git UTF8 LF; normalize CRLF only',jcsReferenceSha256:sha(Buffer.from(fs.readFileSync(path.join(root,'tools/architecture-validation/jcs_crosscheck.mjs'),'utf8').replaceAll('\r\n','\n'))),assetPath:asset,assetByteProfile:'generated UTF8 LF / Git LF blob; fresh Windows CRLF normalized only for verification',assetBytes:output.length,assetSha256:sha(output),catalogPayloadBytes:publicPayload.bytes.length,catalogPayloadSha256:publicPayload.hash,sources},null,2)+'\n');
 return {output,audit};
}
const first=generate();
if(process.argv.includes('--double')){const second=generate();if(!first.output.equals(second.output)||!first.audit.equals(second.audit))throw Error('PUBLIC240_NONDETERMINISTIC');}
if(process.argv.includes('--check')){
 if(!Buffer.from(fs.readFileSync(path.join(root,asset),'utf8').replaceAll('\r\n','\n')).equals(first.output)||!Buffer.from(fs.readFileSync(path.join(root,lock),'utf8').replaceAll('\r\n','\n')).equals(first.audit))throw Error('PUBLIC240_CHECK_MISMATCH');
}else{fs.writeFileSync(path.join(root,asset),first.output);fs.writeFileSync(path.join(root,lock),first.audit);}
console.log(JSON.stringify({count:240,source,assetBytes:first.output.length,assetSha256:sha(first.output),lockSha256:sha(first.audit),double:process.argv.includes('--double'),check:process.argv.includes('--check')}));
