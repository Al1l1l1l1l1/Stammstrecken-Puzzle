'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const os=require('node:os');
const crypto=require('node:crypto');
const producer=require('../project_human_assignments.cjs');
const repo=path.resolve(process.env.STP_HUMAN_REPO_ROOT||process.cwd());
const output=path.resolve(process.env.STP_HUMAN_PROJECTION_OUTPUT||'.utmp/human-assignments');
const overlay=process.env.STP_HUMAN_UI_OVERLAY?path.resolve(process.env.STP_HUMAN_UI_OVERLAY):null;
const sample=path.resolve(process.env.STP_HUMAN_ENDLESS_SAMPLE||'Content/Quality/endless/p05-sample-2026-10-21/sample-audit.internal.json');
const sha=value=>crypto.createHash('sha256').update(value).digest('hex');
const read=file=>JSON.parse(fs.readFileSync(file,'utf8'));
const participant=path.join(output,'participant');
const lock=read(path.join(participant,'raw-sha256-lock.json'));
const c=producer.context(repo);
test.after(()=>c.dispose());
function exact(value,keys){assert.deepEqual(Object.keys(value).sort(),keys.slice().sort());}

test('54 actual campaign cases and 48 actual selected E1 cases form disjoint six-input packs without participant leaks',()=>{
  assert.equal(lock.manifests.length,17);assert.equal(lock.manifests.filter(m=>m.path.startsWith('campaign-')).length,9);assert.equal(lock.manifests.filter(m=>m.path.startsWith('endless-')).length,8);
  const ids=new Set();let campaigns=0,endless=0;
  for(const item of lock.manifests){
    const filename=path.join(participant,item.path);const raw=fs.readFileSync(filename);assert.equal(sha(raw),item.sha256);const canonical=c.strictFile(filename);assert.deepEqual(canonical.bytes,raw);
    const manifest=canonical.value;exact(manifest,['schemaVersion','kind','approvalStatus','applicationId','studyId','revision','buildId','uiSourceSha256','publicCatalogHashSha256','assignments']);
    assert.equal(manifest.schemaVersion,1);assert.equal(manifest.kind,'HUMAN_CALIBRATION_ASSIGNMENTS');assert.equal(manifest.approvalStatus,'DRAFT');assert.equal(manifest.applicationId,'com.STP.StammstreckenPuzzle.human');assert.equal(manifest.uiSourceSha256,lock.uiSourceSha256);assert.equal(manifest.assignments.length,6);
    const profileIds=new Set();const dimensions=new Set();
    for(const entry of manifest.assignments){
      exact(entry,['assignmentId','purpose','publicPuzzleInput','publicSourceSha256','sourceSha256','contentRevision','profileId','profileHashSha256']);assert.match(entry.assignmentId,/^[a-z][a-z0-9-]{0,63}$/);
      const p=producer.validatePublic(c,entry.publicPuzzleInput);assert.equal(c.jcs(p).sha256,entry.publicSourceSha256);assert(!ids.has(p.puzzleId));ids.add(p.puzzleId);dimensions.add(p.grid.width+'x'+p.grid.height);
      if(entry.purpose==='CAMPAIGN_TIME'){campaigns++;assert.equal(entry.profileId,null);assert.equal(entry.profileHashSha256,null);assert(entry.contentRevision>0);}
      else{endless++;profileIds.add(entry.profileId);assert.equal(entry.contentRevision,null);assert.match(p.puzzleId,/^E1-[0-9a-f]{64}$/);}
    }
    if(item.path.startsWith('campaign'))assert(dimensions.size>=4,'Each campaign pack must mix actual visible dimensions.');else assert(profileIds.size>=3,'Each E1 pack must mix at least three actual profiles.');
    assert(!/"(?:solution|solutionHash|proof|proofHash|metrics|seed|generationOrdinal|parameterHashSha256|technicalSampleBand|deductionSteps|maxDeductionDepth|requiredGuessDepth|searchNodes|sessionToken|attemptId|firstContactTimesSeconds)"\s*:/.test(raw.toString()),'Participant input contains internal or invented measurement fields.');
  }
  assert.equal(campaigns,54);assert.equal(endless,48);assert.equal(ids.size,102);
  const defaultManifest=read(path.join(participant,lock.defaultManifest));assert(defaultManifest.assignments.some(a=>a.publicPuzzleInput.puzzleId==='S1-05-04-12'));assert.equal(defaultManifest.assignments.length,6);
});

test('UI source lock uses exact actual raw source bytes, complete ordinal fileset and no circular generated/test/meta inputs',()=>{
  const file=path.join(participant,'ui-source-lock.json');const raw=fs.readFileSync(file);assert.equal(sha(raw),lock.uiSourceSha256);const source=c.strictFile(file).value;exact(source,['schemaVersion','kind','files']);assert.equal(source.kind,'HUMAN_UI_SOURCE_LOCK');assert.equal(source.schemaVersion,1);assert.equal(source.files.length,76);assert.equal(source.files.filter(f=>f.path==='Assets/StammstreckenPuzzle/UI/ThirdPartyNotices.txt').length,1);
  assert.deepEqual(source.files.map(f=>f.path),source.files.map(f=>f.path).sort());assert(source.files.some(f=>f.path.endsWith('ProductionGamePresenter.AssignedReview.cs')));assert(source.files.some(f=>f.path.includes('/Art/AppIcon/')&&f.path.endsWith('.png')));
  for(const item of source.files){assert(!item.path.includes('/Tests/')&&!item.path.endsWith('.meta')&&!item.path.includes('/HumanAssignments/'));const staged=overlay&&path.join(overlay,item.path);const actual=staged&&fs.existsSync(staged)?staged:path.join(repo,item.path);assert.equal(sha(fs.readFileSync(actual)),item.sha256,item.path);}
  assert.deepEqual(producer.uiSources(c,overlay).bytes,raw);
});

test('sourceSha256 binds actual campaign authoring bytes or exact original E1 source-audit bytes; audits remain outside participants',()=>{
  const audit=read(path.join(output,'internal/source-bindings.json'));assert.equal(audit.status,'DRAFT_UNISSUED_ASSIGNED_INPUTS');assert.equal(audit.humanStatus,'NOT_EXECUTED');
  for(const pack of audit.packs){const manifest=read(path.join(participant,pack.manifest));for(const source of pack.sources){const entry=manifest.assignments.find(a=>a.assignmentId===source.assignmentId);assert(entry);assert(!path.isAbsolute(source.sourcePath)&&!source.sourcePath.includes('\\')&&!/^[a-z]:/i.test(source.sourcePath),'Internal sources must use portable repository-relative paths.');assert(!source.sourcePath.split('/').some(p=>p==='..'||p==='.'||p===''));const actual=path.join(repo,source.sourcePath);assert.equal(sha(fs.readFileSync(actual)),entry.sourceSha256);}}
  assert.deepEqual(fs.readdirSync(participant).sort(),[...lock.manifests.map(m=>m.path),'raw-sha256-lock.json','ui-source-lock.json'].sort());
});

test('strict JCS reference rejects duplicate raw keys; public schema and semantic tampering fail closed',()=>{
  const temporary=path.join(os.tmpdir(),'stp-human-duplicate-'+process.pid+'.json');fs.writeFileSync(temporary,'{"puzzleId":"one","puzzleId":"two"}');try{assert.throws(()=>c.strictFile(temporary));}finally{fs.unlinkSync(temporary);}
  const p=read(path.join(participant,lock.defaultManifest)).assignments[0].publicPuzzleInput;assert.throws(()=>producer.validatePublic(c,{...p,unexpected:true}),/fields/);assert.throws(()=>producer.validatePublic(c,{...p,publicPuzzleHash:{...p.publicPuzzleHash,sha256:'0'.repeat(64)}}),/semantic hash mismatch/);
});

test('E1 uppercase/path/hash mutations cannot become assigned input; CLI cannot silently replace outputs or invent a build identity',()=>{
  const data=read(sample);const record=data.selected[0];const base=path.dirname(sample);const catalogHash=read(path.join(repo,'Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json')).publicCatalogHashSha256;
  assert.throws(()=>producer.endlessCandidate(c,base,{...record,puzzleId:record.puzzleId.toUpperCase()},catalogHash),/identity/);
  assert.throws(()=>producer.endlessCandidate(c,base,{...record,publicFile:'../outside.json'},catalogHash),/source path/);
  assert.throws(()=>producer.endlessCandidate(c,base,{...record,publicSourceSha256:'0'.repeat(64)},catalogHash),/public source/);
  assert.throws(()=>producer.endlessCandidate(c,base,{...record,parameterHashSha256:'0'.repeat(64)},catalogHash),/parameter/);
  assert.throws(()=>producer.project({repoRoot:repo,out:output}),/build-id/);
  assert.throws(()=>producer.project({repoRoot:repo,out:output,buildId:'explicit-existing-projection'}),/already exists/);
  assert.throws(()=>producer.cli(['--unknown','x']),/CLI argument/);
  assert.throws(()=>producer.repositoryPath(repo,path.resolve(repo,'../outside.json')),/source path/);
  assert.throws(()=>producer.repositoryPath(repo,repo),/source path/);
  assert.throws(()=>producer.project({repoRoot:repo,out:path.join(output,'outside-overlay'),buildId:'explicit-source',uiOverlay:path.resolve(repo,'..')}),/source path/);
});

test('Offline notice raw bytes are mandatory in schema1 current76 source set; changed or absent notice cannot reuse the UI hash',()=>{
  const ui=read(path.join(participant,'ui-source-lock.json'));assert.equal(ui.schemaVersion,1);assert.equal(ui.files.length,76);
  const notice='Assets/StammstreckenPuzzle/UI/ThirdPartyNotices.txt';assert.equal(ui.files.filter(e=>e.path===notice).length,1);
  const scratch=path.join(path.dirname(output),path.basename(output)+'-notice-selector-controls');assert(!fs.existsSync(scratch));fs.mkdirSync(scratch);
  for(const entry of ui.files){const staged=overlay&&path.join(overlay,entry.path);const source=staged&&fs.existsSync(staged)?staged:path.join(repo,entry.path);const dest=path.join(scratch,entry.path);fs.mkdirSync(path.dirname(dest),{recursive:true});fs.copyFileSync(source,dest);}
  const reference='tools/architecture-validation/jcs_crosscheck.mjs';fs.mkdirSync(path.dirname(path.join(scratch,reference)),{recursive:true});fs.copyFileSync(path.join(repo,reference),path.join(scratch,reference));
  const local=producer.context(scratch);try{
    const baseline=producer.uiSources(local,null);assert.equal(baseline.sha256,lock.uiSourceSha256);const file=path.join(scratch,notice);const original=fs.readFileSync(file);
    fs.writeFileSync(file,Buffer.concat([original,Buffer.from('\n')]));const changed=producer.uiSources(local,null);assert.notEqual(changed.sha256,baseline.sha256);
    const a=JSON.parse(baseline.bytes).files,b=JSON.parse(changed.bytes).files;assert.deepEqual(a.filter(e=>e.path!==notice),b.filter(e=>e.path!==notice));assert.notEqual(a.find(e=>e.path===notice).sha256,b.find(e=>e.path===notice).sha256);
    fs.writeFileSync(file,original);assert.equal(producer.uiSources(local,null).sha256,baseline.sha256);fs.unlinkSync(file);assert.throws(()=>producer.uiSources(local,null),/Missing actual UI source/);
  }finally{local.dispose();}
});
