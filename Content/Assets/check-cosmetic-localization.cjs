'use strict';
// Read-only cross-reference check. The existing UI Resolve namespace is explicit; no alias or fallback is installed.
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict'),crypto=require('node:crypto');
const raw=b=>crypto.createHash('sha256').update(b).digest('hex');
function fields(value,names){assert(value&&typeof value==='object'&&!Array.isArray(value));assert.deepEqual(Object.keys(value).sort(),[...names].sort());}
function verify(repo,sourcePath){
 const producer=require(path.join(repo,'tools/campaign/project_human_assignments.cjs'));const c=producer.context(repo);
 try {
  const source=c.strictFile(sourcePath),catalog=c.strictFile(path.join(repo,'Content/Catalogs/s1/cosmetics-v2.json'));
  const locale=c.strictFile(path.join(repo,'Assets/StammstreckenPuzzle/UI/ProductionGameLocale.json'));
  const bytes=fs.readFileSync(path.join(repo,'Content/Assets/railway-assets-v1.json')),assets=JSON.parse(bytes);
  assert(bytes.equals(fs.readFileSync(path.join(repo,'Assets/StammstreckenPuzzle/UI/RailwayAssets.json'))),'Actual source/runtime assets must remain identical');
  fields(source.value,['schemaVersion','locale','status','entries']);assert.equal(source.value.schemaVersion,1);assert.equal(source.value.locale,'de-DE');assert.equal(source.value.status,'DRAFT');
  assert.equal(source.value.entries.length,14);assert.equal(catalog.value.items.length,14);assert.equal(assets.cosmetics.length,14);
  const projected=new Map(locale.value.entries.map(e=>[e.key,e]));assert.equal(projected.size,locale.value.entries.length);
  const names=new Map();for(const row of source.value.entries){
   fields(row,['key','text','description','charLimit','placeholders','status','audience','availability','blockedBy','charLimitStatus']);
   assert(typeof row.key==='string'&&/^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)+$/.test(row.key)&&row.key.length<=160);assert(typeof row.description==='string'&&row.description.trim().length>0);assert(typeof row.text==='string');
   assert.equal(row.status,'DRAFT');assert.equal(row.audience,'PLAYER');assert.equal(row.availability,'DECIDED');assert.equal(row.charLimitStatus,'EDITORIAL_PROPOSAL');assert.deepEqual(row.blockedBy,[]);assert.deepEqual(row.placeholders,{});assert(Number.isInteger(row.charLimit)&&row.charLimit>0&&row.text.length>0&&row.text.length<=row.charLimit);assert(!names.has(row.key));names.set(row.key,row.text);
  }
  for(const item of catalog.value.items){
   assert.equal(item.titleKey,'cosmetic.'+item.id+'.title');assert(names.has(item.titleKey));
   const art=assets.cosmetics.filter(a=>a.id===item.id);assert.equal(art.length,1);assert.equal(art[0].titleKey,item.titleKey);assert.equal(art[0].assetAddress,item.assetAddress);
   const runtime=projected.get('product.'+item.titleKey);assert(runtime,'Actual renderer key missing');assert.equal(runtime.value,names.get(item.titleKey));assert.deepEqual(runtime.argumentTypes,[]);
   assert(!projected.has(item.titleKey),'No alias may be added to the actual runtime locale');
  }
  const resolve=fs.readFileSync(path.join(repo,'Assets/StammstreckenPuzzle/Scripts/Presentation/UI/ProductionGameLocalization.cs'));
  assert(resolve.toString('utf8').includes('var key="product."+suffix;'),'Existing namespace contract changed; independent review required');
  return {kind:'ACTUAL_DRAFT_COSMETIC_SOURCE_CROSS_REFERENCE_ONLY',titles:14,aliasesCreated:0,sourceRawSha256:source.sourceRawSha256,catalogRawSha256:catalog.sourceRawSha256,localeRawSha256:locale.sourceRawSha256,assetsRawSha256:raw(bytes),namespaceAdapterRawSha256:raw(resolve),approvalStatus:'DRAFT',productionApproval:false,saveOrCandidateRebinding:false};
 } finally {c.dispose();}
}
if(require.main===module){
 const args=process.argv.slice(2);assert(args.length===0||args.length===4&&args[0]==='--repo'&&args[2]==='--source','Expected --repo ABSOLUTE --source ABSOLUTE, or repository defaults');
 const repo=args.length?path.resolve(args[1]):path.resolve(__dirname,'../..');const source=args.length?path.resolve(args[3]):path.join(repo,'Content/Localization/source/cosmetics-de-DE.json');console.log(JSON.stringify(verify(repo,source)));
}
module.exports={verify};
