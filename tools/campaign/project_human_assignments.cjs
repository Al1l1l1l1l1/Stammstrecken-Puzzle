#!/usr/bin/env node
'use strict';
// ADR042: explicit public-only assignment projection. This tool never starts a participant, changes a profile or promotes content.
const fs=require('node:fs');
const path=require('node:path');
const os=require('node:os');
const cp=require('node:child_process');
const crypto=require('node:crypto');
const HASH=/^[0-9a-f]{64}$/;
const PROFILES=['dienstbeginn','regelbetrieb','stoerungslage','grosslage'];
const PUBLIC_PROFILE='STP-PUZZLE-SEMANTIC-JCS-1';
const UI_FIXED=['ProductionGame.uxml','ProductionGame.uss','ProductionGameLocale.json','ProductionCampaignText.json','RailwayAssets.json','ThirdPartyNotices.txt'].map(p=>'Assets/StammstreckenPuzzle/UI/'+p);
const SEARCH_FIELDS=['minSize','maxSize','minDensityPercent','maxDensityPercent','minDeductionSteps','minDeductionDepth','maxAttempts','pathStateBudget','totalBudgetMs','solverBudgetMs','maxTrivialLines','recentSimilarityPercent'];
const AUDIT_FIELDS=['profileId','profileHashSha256','seed','generationOrdinal','parameterHashSha256','puzzleId','publicPuzzleHashSha256','publicSourceSha256','solutionHashSha256','proofHashSha256','proofSourceSha256','width','height','deductionSteps','maxDeductionDepth','requiredGuessDepth','publicFile'];
const rawHash=bytes=>crypto.createHash('sha256').update(bytes).digest('hex');
function requireFact(ok,message){if(!ok)throw Error(message);}
function fields(value,names){requireFact(value&&typeof value==='object'&&!Array.isArray(value)&&Object.keys(value).length===names.length&&names.every(n=>Object.hasOwn(value,n)),'Unexpected or missing fields: '+names.join(','));}
function integer(value,min,max){requireFact(Number.isSafeInteger(value)&&value>=min&&value<=max,'Integer outside closed bounds');}
function hash(value){requireFact(typeof value==='string'&&HASH.test(value),'Invalid lowercase SHA256');return value;}
function inside(base,relative){requireFact(typeof relative==='string'&&!path.isAbsolute(relative)&&!relative.includes('\\')&&!relative.split('/').some(p=>p==='..'||p==='.'||p===''),'Non-repository source path');const result=path.resolve(base,relative);requireFact(result.startsWith(path.resolve(base)+path.sep),'Source path escapes root');return result;}
function repositoryPath(repo,filename){const relative=path.relative(path.resolve(repo),path.resolve(filename)).split(path.sep).join('/');inside(repo,relative);return relative;}
function readBytes(filename){const stat=fs.statSync(filename);requireFact(stat.isFile()&&stat.size<=8388608,'Source absent or too large: '+filename);return fs.readFileSync(filename);}
function context(repo){
  const reference=path.join(repo,'tools/architecture-validation/jcs_crosscheck.mjs');requireFact(fs.existsSync(reference),'Existing JCS reference is required');
  const scratch=fs.mkdtempSync(path.join(os.tmpdir(),'stp-human-jcs-'));const created=[];const cache=new Map();let index=0;
  function strictFile(filename){
    const bytes=readBytes(filename);const key=rawHash(bytes);if(cache.has(key))return {...cache.get(key),sourceRawSha256:key};
    const result=JSON.parse(cp.execFileSync(process.execPath,[reference,filename],{encoding:'utf8',maxBuffer:32*1024*1024,stdio:['ignore','pipe','pipe']}));
    const canonical=Buffer.from(result.canonicalBase64,'base64');requireFact(rawHash(canonical)===result.sha256,'JCS reference hash mismatch');
    const value={value:JSON.parse(canonical.toString('utf8')),bytes:canonical,sha256:result.sha256,sourceRawSha256:key};cache.set(key,value);return value;
  }
  function jcs(value){const file=path.join(scratch,(index++)+'.json');created.push(file);fs.writeFileSync(file,JSON.stringify(value),{flag:'wx'});return strictFile(file);}
  return {repo,strictFile,jcs,dispose(){for(const file of created)fs.unlinkSync(file);fs.rmdirSync(scratch);}};
}
function validatePublic(c,p){
  fields(p,['puzzleId','rulesetVersion','grid','endpoints','rowCounts','columnCounts','publicPuzzleHash']);requireFact(typeof p.puzzleId==='string'&&p.puzzleId.length<=128&&p.rulesetVersion==='train-track-v1','Unsupported public identity/ruleset');
  fields(p.grid,['width','height']);integer(p.grid.width,2,32);integer(p.grid.height,2,32);fields(p.endpoints,['a','b']);
  for(const endpoint of [p.endpoints.a,p.endpoints.b]){fields(endpoint,['side','index']);requireFact(['N','E','S','W'].includes(endpoint.side),'Unknown exterior side');integer(endpoint.index,0,(endpoint.side==='N'||endpoint.side==='S'?p.grid.width:p.grid.height)-1);}
  requireFact(p.endpoints.a.side!==p.endpoints.b.side||p.endpoints.a.index!==p.endpoints.b.index,'Duplicate exterior endpoints');
  for(const [counts,length,max] of [[p.rowCounts,p.grid.height,p.grid.width],[p.columnCounts,p.grid.width,p.grid.height]]){requireFact(Array.isArray(counts)&&counts.length===length,'Counts dimensions mismatch');for(const n of counts)integer(n,0,max);}
  requireFact(p.rowCounts.reduce((a,b)=>a+b,0)===p.columnCounts.reduce((a,b)=>a+b,0),'Count totals mismatch');fields(p.publicPuzzleHash,['profile','sha256']);requireFact(p.publicPuzzleHash.profile===PUBLIC_PROFILE,'Wrong semantic hash profile');hash(p.publicPuzzleHash.sha256);
  const {publicPuzzleHash,...projection}=p;requireFact(c.jcs(projection).sha256===publicPuzzleHash.sha256,'Actual public semantic hash mismatch');return p;
}
function uiSources(c,overlay){
  const paths=new Set(UI_FIXED);
  function walk(base,relative,accept){const absolute=path.join(base,relative);if(!fs.existsSync(absolute))return;for(const entry of fs.readdirSync(absolute,{withFileTypes:true})){requireFact(!entry.isSymbolicLink(),'UI source symlink rejected');const next=relative+'/'+entry.name;if(entry.isDirectory())walk(base,next,accept);else if(entry.isFile()&&accept(next))paths.add(next);}}
  for(const base of [c.repo,...(overlay?[overlay]:[])]){
    for(const dir of ['Assets/StammstreckenPuzzle/Scripts/Presentation/UI','Assets/StammstreckenPuzzle/Scripts/Presentation/World'])walk(base,dir,p=>p.endsWith('.cs'));
    walk(base,'Assets/StammstreckenPuzzle/Art',p=>/\.(png|svg|ttf|otf)$/i.test(p));
  }
  const files=[...paths].sort().map(relative=>{const staged=overlay&&path.join(overlay,relative);const source=staged&&fs.existsSync(staged)?staged:inside(c.repo,relative);requireFact(fs.existsSync(source),'Missing actual UI source: '+relative);return {path:relative,sha256:rawHash(fs.readFileSync(source))};});
  requireFact(files.length>UI_FIXED.length,'Incomplete UI source set');return c.jcs({schemaVersion:1,kind:'HUMAN_UI_SOURCE_LOCK',files});
}
function campaign(c,qualityPath,publicPath,bindingsPath){
  const source=c.strictFile(publicPath);const catalog=source.value;fields(catalog,['schemaVersion','catalogId','revision','use','status','puzzles','catalogHashSha256']);
  requireFact(catalog.schemaVersion===1&&catalog.catalogId==='qa-mobile-candidate-public'&&catalog.revision===1&&catalog.use==='InternalPreview'&&catalog.status==='DRAFT','Current DRAFT public source required');
  const {catalogHashSha256,...projection}=catalog;requireFact(c.jcs(projection).sha256===hash(catalogHashSha256),'Current public catalog hash mismatch');requireFact(catalog.puzzles.length===240,'Current public corpus must contain 240 inputs');
  const bindings=c.strictFile(bindingsPath).value;fields(bindings,['schemaVersion','catalogNamespace','use','approvalStatus','releaseEligible','publicCatalogHashSha256','campaignHashSha256','completionHashSha256','cosmeticsHashSha256']);
  requireFact(bindings.schemaVersion===1&&bindings.catalogNamespace==='qa-mobile-candidate'&&bindings.use==='InternalPreview'&&bindings.approvalStatus==='DRAFT'&&bindings.releaseEligible===false&&bindings.publicCatalogHashSha256===catalogHashSha256,'Current public binding mismatch');
  const byId=new Map();for(const p of catalog.puzzles){validatePublic(c,p);requireFact(/^S1-[0-9]{2}-[0-9]{2}-[0-9]{2}$/.test(p.puzzleId)&&!byId.has(p.puzzleId),'Unknown/duplicate campaign identity');byId.set(p.puzzleId,p);}
  const quality=c.strictFile(qualityPath);const q=quality.value;fields(q,['schemaVersion','policy','approvalStatus','levelCount','targetedLateCount','initialHumanSampleCount','humanAcceptance','starTimesReleased','entries']);requireFact(q.schemaVersion===1&&q.policy==='MANUS-2026-10-20-02-P02'&&q.approvalStatus==='DRAFT'&&q.levelCount===240&&q.initialHumanSampleCount===54&&q.humanAcceptance==='NOT_EXECUTED'&&q.starTimesReleased===false,'Actual unmeasured initial 54-case source required');
  requireFact(Array.isArray(q.entries)&&q.entries.length===240&&new Set(q.entries.map(e=>e.puzzleId)).size===240,'Quality coverage mismatch');const selected=q.entries.filter(e=>e.humanSampleRequired===true);requireFact(selected.length===54,'Initial sample is not exactly 54');const result=[];
  for(const e of selected){
    requireFact(e.humanStatus==='NOT_EXECUTED'&&Array.isArray(e.firstContactTimesSeconds)&&e.firstContactTimesSeconds.length===0,'Human evidence may not be rewritten');
    const p=byId.get(e.puzzleId);requireFact(p,'Selected input not in current corpus');const parts=e.puzzleId.split('-');const expected=`Content/Levels/s1/${parts[1]}/${parts[2]}/${e.puzzleId}.level-v2.json`;requireFact(e.authoringPath===expected,'Unknown authoring source path');
    const level=c.strictFile(inside(c.repo,expected));requireFact(level.sourceRawSha256===hash(e.sourceSha256)&&level.value.puzzleId===p.puzzleId,'Stale authoring source bytes: '+p.puzzleId);integer(level.value.contentRevision,1,2147483647);
    for(const field of ['puzzleId','rulesetVersion','grid','endpoints','rowCounts','columnCounts'])requireFact(c.jcs(level.value[field]).sha256===c.jcs(p[field]).sha256,'Authoring/public projection mismatch: '+p.puzzleId);
    result.push({assignment:{assignmentId:'campaign-'+p.puzzleId.toLowerCase(),purpose:'CAMPAIGN_TIME',publicPuzzleInput:p,publicSourceSha256:c.jcs(p).sha256,sourceSha256:level.sourceRawSha256,contentRevision:level.value.contentRevision,profileId:null,profileHashSha256:null},sourcePath:expected});
  }
  result.sort((a,b)=>a.assignment.publicPuzzleInput.grid.width*a.assignment.publicPuzzleInput.grid.height-b.assignment.publicPuzzleInput.grid.width*b.assignment.publicPuzzleInput.grid.height||a.assignment.publicPuzzleInput.puzzleId.localeCompare(b.assignment.publicPuzzleInput.puzzleId,'en'));
  const packs=Array.from({length:9},()=>[]);for(let i=0;i<result.length;i++)packs[i%9].push(result[i]);
  return {packs,catalogHashSha256,sourceSha256:source.sourceRawSha256,qualitySha256:quality.sourceRawSha256};
}
function endlessCandidate(c,sampleDir,record,catalogHash){
  fields(record,[...AUDIT_FIELDS,'technicalSampleBand']);requireFact(PROFILES.includes(record.profileId)&&typeof record.puzzleId==='string'&&record.puzzleId.length===67&&/^E1-[0-9a-f]{64}$/.test(record.puzzleId),'Unknown E1/profile identity');
  requireFact(record.publicFile===record.profileId+'/'+record.puzzleId+'/public.json','Unknown E1 public source path');const publicFile=inside(sampleDir,record.publicFile);const folder=path.dirname(publicFile);
  const publicSource=c.strictFile(publicFile);const p=validatePublic(c,publicSource.value);requireFact(p.puzzleId===record.puzzleId&&p.publicPuzzleHash.sha256===record.publicPuzzleHashSha256&&publicSource.sha256===hash(record.publicSourceSha256)&&publicSource.sourceRawSha256===publicSource.sha256,'E1 public source/hash mismatch');
  const audit=c.strictFile(path.join(folder,'source-audit.internal.json'));const descriptor=c.strictFile(path.join(folder,'descriptor.internal.json'));const parameters=c.strictFile(path.join(folder,'parameters.json'));
  fields(audit.value,AUDIT_FIELDS);fields(parameters.value,['contractVersion','profileId','profileHashSha256','noveltyVersion','campaignCatalog','recentPublicInputs',...SEARCH_FIELDS]);fields(parameters.value.campaignCatalog,['catalogId','revision','hashSha256']);
  requireFact(parameters.value.contractVersion===1&&parameters.value.noveltyVersion==='public-d4-l1-v1'&&parameters.value.campaignCatalog.catalogId==='qa-mobile-candidate-public'&&parameters.value.campaignCatalog.revision===1&&Array.isArray(parameters.value.recentPublicInputs)&&parameters.value.recentPublicInputs.length<=50,'Unsupported complete search parameters');
  const currentProfiles=c.strictFile(path.join(c.repo,'Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-profiles.json')).value;const current=currentProfiles.profiles.find(p=>p.profileId===record.profileId);requireFact(current&&currentProfiles.productApproved===false&&current.approvalStatus==='DRAFT'&&current.humanStatus==='NOT_EXECUTED'&&current.profileHashSha256===record.profileHashSha256&&current.publicCatalogHashSha256===catalogHash,'E1 current profile source mismatch');
  const {profileHashSha256,...profileProjection}=current;requireFact(c.jcs(profileProjection).sha256===profileHashSha256,'Current profile self-hash mismatch');for(const field of SEARCH_FIELDS)requireFact(parameters.value[field]===current.parameters[field],'Captured search setting does not bind current profile');
  fields(descriptor.value,['endlessContractVersion','rulesetVersion','generatorVersion','seed','generationOrdinal','parameterHashSha256']);requireFact(descriptor.value.endlessContractVersion===1&&descriptor.value.rulesetVersion===p.rulesetVersion&&descriptor.value.generatorVersion==='path-search-v1'&&'E1-'+descriptor.sha256===p.puzzleId,'E1 descriptor identity mismatch');
  requireFact(parameters.sha256===descriptor.value.parameterHashSha256&&parameters.sha256===record.parameterHashSha256&&parameters.value.profileId===record.profileId&&parameters.value.profileHashSha256===record.profileHashSha256&&parameters.value.campaignCatalog.hashSha256===catalogHash,'E1 complete parameter/source mismatch');
  for(const key of Object.keys(audit.value))requireFact(Object.hasOwn(record,key)&&c.jcs(audit.value[key]).sha256===c.jcs(record[key]).sha256,'E1 selected/source-audit mismatch: '+key);
  requireFact(audit.value.puzzleId===p.puzzleId&&audit.value.publicSourceSha256===publicSource.sha256,'E1 source audit does not bind public input');
  const solution=c.strictFile(path.join(folder,'solution.internal.json'));const proof=c.strictFile(path.join(folder,'proof.internal.json'));
  fields(solution.value,['puzzleId','publicPuzzleHash','path']);fields(proof.value.metrics,['searchNodes','deductionSteps','maxDeductionDepth','requiredGuessDepth']);for(const value of Object.values(proof.value.metrics))integer(value,0,Number.MAX_SAFE_INTEGER);
  requireFact(solution.sha256===hash(record.solutionHashSha256)&&solution.value.puzzleId===p.puzzleId&&solution.value.publicPuzzleHash.sha256===p.publicPuzzleHash.sha256,'E1 solution source mismatch');
  fields(proof.value,['proofFormatVersion','puzzleId','publicPuzzleHash','solutionHash','solverVersion','solutionCount','metrics','proofHash']);const {proofHash,...proofProjection}=proof.value;
  requireFact(proof.sourceRawSha256===hash(record.proofSourceSha256)&&proofHash.profile==='STP-PROOF-JCS-1'&&proofHash.sha256===record.proofHashSha256&&c.jcs(proofProjection).sha256===proofHash.sha256&&proof.value.puzzleId===p.puzzleId&&proof.value.publicPuzzleHash.sha256===p.publicPuzzleHash.sha256&&proof.value.solutionHash.profile==='STP-SOLUTION-JCS-1'&&proof.value.solutionHash.sha256===solution.sha256&&proof.value.proofFormatVersion===1&&proof.value.solverVersion==='solver-v2'&&proof.value.solutionCount===1&&proof.value.metrics.requiredGuessDepth===0,'E1 original proof binding mismatch');
  return {assignment:{assignmentId:'endless-'+p.puzzleId.slice(3,59),purpose:'ENDLESS_PROFILE',publicPuzzleInput:p,publicSourceSha256:publicSource.sha256,sourceSha256:audit.sourceRawSha256,contentRevision:null,profileId:record.profileId,profileHashSha256:hash(record.profileHashSha256)},sourcePath:repositoryPath(c.repo,path.join(folder,'source-audit.internal.json')),descriptorSha256:descriptor.sourceRawSha256,parametersSha256:parameters.sourceRawSha256};
}
function endless(c,filename,catalogHash){
  const source=c.strictFile(filename);const data=source.value;fields(data,['schemaVersion','status','humanStatus','humanTimingOrApprovalProduced','sourceFiles','pool','attempts','selected','selectionMethod']);requireFact(data.schemaVersion===1&&data.status==='DRAFT_TECHNICALLY_CURATED_UNISSUED_INPUTS'&&data.humanStatus==='NOT_EXECUTED'&&data.humanTimingOrApprovalProduced===false&&Array.isArray(data.selected)&&data.selected.length===48,'Actual unissued 48-case P05 source required');
  for(const file of data.sourceFiles)requireFact(rawHash(fs.readFileSync(inside(c.repo,file.path)))===hash(file.sha256),'P05 source lock changed: '+file.path);
  const groups=PROFILES.map(id=>data.selected.filter(r=>r.profileId===id));requireFact(groups.every(g=>g.length===12),'Exactly twelve selected candidates per profile required');
  const records=[];for(let i=0;i<12;i++)for(const group of groups)records.push(endlessCandidate(c,path.dirname(filename),group[i],catalogHash));requireFact(new Set(records.map(r=>r.assignment.publicPuzzleInput.puzzleId)).size===48,'Duplicate selected E1 input');
  return {packs:Array.from({length:8},(_,i)=>records.slice(i*6,i*6+6)),sourceSha256:source.sourceRawSha256};
}
function project(options){
  const repo=path.resolve(options.repoRoot||process.cwd());requireFact(fs.existsSync(path.join(repo,'AGENTS.md')),'Repository root required');requireFact(typeof options.buildId==='string'&&options.buildId.trim()===options.buildId&&options.buildId.length>0&&options.buildId.length<=128,'Explicit actual --build-id required');
  const study=options.studyId||'wp046-human';requireFact(/^[a-z][a-z0-9-]{0,44}$/.test(study),'Invalid study prefix');const revision=options.revision===undefined?1:Number(options.revision);integer(revision,1,2147483647);
  requireFact(options.out,'Explicit new --out directory required');const output=path.resolve(options.out);requireFact(!fs.existsSync(output),'Output already exists; originals are never replaced');const overlay=options.uiOverlay?path.resolve(options.uiOverlay):null;const sourceUiOverlay=overlay?repositoryPath(repo,overlay):null;const c=context(repo);
  try{
    const campaignSource=campaign(c,path.resolve(options.campaignSample||path.join(repo,'Content/Quality/s1/difficulty-and-human-sample.json')),path.resolve(options.publicCatalog||path.join(repo,'Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json')),path.resolve(options.publicBindings||path.join(repo,'Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json')));
    const ui=uiSources(c,overlay);const additional=options.endlessSample?endless(c,path.resolve(options.endlessSample),campaignSource.catalogHashSha256):null;
    const manifests=[];const audit=[];
    for(const [kind,packs] of [['campaign',campaignSource.packs],...(additional?[['endless',additional.packs]]:[])])for(let i=0;i<packs.length;i++){
      const entries=packs[i];requireFact(entries.length===6&&new Set(entries.map(e=>e.assignment.assignmentId)).size===6,'Each pack must have exactly six unique assigned inputs');const name=kind+'-pack-'+String(i+1).padStart(2,'0')+'.json';
      const document={schemaVersion:1,kind:'HUMAN_CALIBRATION_ASSIGNMENTS',approvalStatus:'DRAFT',applicationId:'com.STP.StammstreckenPuzzle.human',studyId:study+'-'+kind+'-'+String(i+1).padStart(2,'0'),revision,buildId:options.buildId,uiSourceSha256:ui.sha256,publicCatalogHashSha256:campaignSource.catalogHashSha256,assignments:entries.map(e=>e.assignment)};
      const result=c.jcs(document);manifests.push({path:name,sha256:result.sha256,studyId:document.studyId,assignmentCount:6,bytes:result.bytes});audit.push({manifest:name,sources:entries.map(e=>({assignmentId:e.assignment.assignmentId,puzzleId:e.assignment.publicPuzzleInput.puzzleId,sourcePath:e.sourcePath,sourceSha256:e.assignment.sourceSha256,...(e.descriptorSha256?{descriptorSha256:e.descriptorSha256,parametersSha256:e.parametersSha256}:{})}))});
    }
    const defaultManifest=audit.find(a=>a.sources.some(s=>s.puzzleId==='S1-05-04-12'))?.manifest;requireFact(defaultManifest,'Owner-named default assigned input is missing');
    const rawLock=c.jcs({schemaVersion:1,kind:'HUMAN_ASSIGNMENT_RAW_LOCK',uiSourceSha256:ui.sha256,defaultManifest,manifests:manifests.map(({bytes,...m})=>m)});
    requireFact(uiSources(c,overlay).sha256===ui.sha256,'Actual UI sources changed during projection; no output written');
    fs.mkdirSync(output);fs.mkdirSync(path.join(output,'participant'));fs.mkdirSync(path.join(output,'internal'));
    for(const m of manifests)fs.writeFileSync(path.join(output,'participant',m.path),m.bytes,{flag:'wx'});
    fs.writeFileSync(path.join(output,'participant','ui-source-lock.json'),ui.bytes,{flag:'wx'});fs.writeFileSync(path.join(output,'participant','raw-sha256-lock.json'),rawLock.bytes,{flag:'wx'});
    fs.writeFileSync(path.join(output,'internal','source-bindings.json'),c.jcs({schemaVersion:1,status:'DRAFT_UNISSUED_ASSIGNED_INPUTS',humanStatus:'NOT_EXECUTED',buildId:options.buildId,sourceUiOverlay,uiSourceSha256:ui.sha256,campaignPublicRawSha256:campaignSource.sourceSha256,campaignQualityRawSha256:campaignSource.qualitySha256,endlessSampleRawSha256:additional?.sourceSha256||null,packs:audit}).bytes,{flag:'wx'});
    return {output,manifestCount:manifests.length,campaignAssignments:54,endlessAssignments:additional?48:0,defaultManifest,uiSourceSha256:ui.sha256,rawLockSha256:rawLock.sha256};
  }finally{c.dispose();}
}
function cli(argv){const names={'--repo-root':'repoRoot','--out':'out','--build-id':'buildId','--study-id':'studyId','--revision':'revision','--campaign-sample':'campaignSample','--public-catalog':'publicCatalog','--public-bindings':'publicBindings','--ui-overlay':'uiOverlay','--endless-sample':'endlessSample'};const options={};for(let i=0;i<argv.length;i+=2){const key=names[argv[i]];requireFact(key&&argv[i+1]&&!Object.hasOwn(options,key),'Unknown, duplicate or missing CLI argument');options[key]=argv[i+1];}return project(options);}
module.exports={context,validatePublic,uiSources,campaign,endlessCandidate,endless,repositoryPath,project,cli};
if(require.main===module){try{process.stdout.write(JSON.stringify(cli(process.argv.slice(2)),null,2)+'\n');}catch(error){console.error('HUMAN ASSIGNMENT PROJECTION FAILED: '+error.message);process.exitCode=1;}}
