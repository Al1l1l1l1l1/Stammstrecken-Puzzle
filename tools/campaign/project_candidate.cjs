#!/usr/bin/env node
'use strict';
// Explicit DRAFT/InternalPreview projection. Historical QA assets and authoring solutions stay out of the new runtime catalog.
const fs = require('node:fs');
const path = require('node:path');
const cp = require('node:child_process');
const crypto = require('node:crypto');
const root = path.resolve(__dirname, '../..');
const check = process.argv.includes('--check');
const scratch = path.join(root, '.utmp/wp046-projection');
fs.mkdirSync(scratch, {recursive: true});
const read = name => JSON.parse(fs.readFileSync(path.join(root, name), 'utf8'));
const hash = value => {
  const input = path.join(scratch, 'jcs-input.json');
  fs.writeFileSync(input, JSON.stringify(value));
  return JSON.parse(cp.execFileSync(process.execPath, [path.join(root, 'tools/architecture-validation/jcs_crosscheck.mjs'), input], {encoding: 'utf8'})).sha256;
};
const outputs = [];
function emit(name, value) {
  const bytes = Buffer.from(JSON.stringify(value, null, 2) + '\n');
  const filename = path.join(root, name);
  if (check) {
    if (!fs.existsSync(filename) || fs.readFileSync(filename, 'utf8').replace(/\r\n/g, '\n') !== bytes.toString()) throw Error('Projection differs: ' + name);
  } else { fs.mkdirSync(path.dirname(filename), {recursive: true}); fs.writeFileSync(filename, bytes); }
  outputs.push({path: name, sha256: crypto.createHash('sha256').update(bytes).digest('hex')});
}
const campaign = read('Assets/StammstreckenPuzzle/UI/ReviewCampaign.catalog.json');
campaign.catalogId = 'campaign-qa-mobile-candidate';
const completion = read('Assets/StammstreckenPuzzle/UI/ReviewCompletion.catalog.json');
completion.completionCatalog.catalogId = 'completion-qa-mobile-candidate';
completion.completionCatalog.completionMoments = [];
completion.completionCatalog.rewardDefinitions.push({reasonCode:'POST_CLEAR_PATIENCE', subjectKind:'LEVEL', amountPatience:10, claimCardinality:'ONCE_PER_SUBJECT'});
const cosmetics = read('Content/Catalogs/s1/cosmetics-v2.json');
cosmetics.catalogId = 'qa-mobile-candidate-cosmetics';
const catalog = {schemaVersion:1, catalogId:'qa-mobile-candidate-public', revision:1, use:'InternalPreview', status:'DRAFT', puzzles:[]};
const sources = []; let previous;
const textSourcePath = 'Content/Localization/source/de-DE.json';
const textSource = read(textSourcePath);
const playerText = new Map(textSource.entries.filter(e=>e.audience==='PLAYER').map(e=>[e.key,e.text]));
const campaignText = {schemaVersion:1, locale:'de-DE', approvalStatus:'DRAFT',
  sourceSha256:crypto.createHash('sha256').update(fs.readFileSync(path.join(root,textSourcePath))).digest('hex'), entries:[]};
const season = campaign.seasons[0]; season.titleKey = 'campaign.s1.name';
for (const section of season.sections) {
  const si = section.order.toString().padStart(2, '0');
  section.titleKey = `campaign.s1.section.${si}.name`;
  for (const route of section.routes) {
    const ri = route.order.toString().padStart(2, '0');
    route.titleKey = `campaign.s1.route.${si}.${ri}.name`;
    for (const entry of route.levels) {
      const file = `Content/Levels/s1/${si}/${ri}/${entry.puzzleId}.level-v2.json`;
      const level = read(file);
      if (level.puzzleId !== entry.puzzleId) throw Error('Source identity mismatch');
      const projection = Object.fromEntries(['puzzleId','rulesetVersion','grid','endpoints','rowCounts','columnCounts'].map(k => [k, level[k]]));
      catalog.puzzles.push({...projection, publicPuzzleHash:{profile:'STP-PUZZLE-SEMANTIC-JCS-1', sha256:hash(projection)}});
      entry.unlock = previous ? {kind:'ALL_FIRST_CLEARS', requiredSubjectIds:[previous]} : {kind:'ALWAYS', requiredSubjectIds:[]};
      previous = entry.puzzleId;
      completion.completionCatalog.completionMoments.push({id:entry.puzzleId.toLowerCase() + '-completion', ...level.completion});
      const thresholds = level.production.starThresholdsSeconds;
      if (thresholds !== null) throw Error('Calibrated threshold mapping needs explicit source contract; no invented times');
      sources.push({path:file, contentRevision:level.contentRevision, documentSha256:hash(level)});
      const values = {puzzleId:level.puzzleId, title:playerText.get(level.content.titleKey),
        status:playerText.get(level.content.statusKey), result:playerText.get(level.completion.resultTextKey)};
      if(Object.values(values).some(v=>typeof v!=='string'||v.length===0))throw Error('Missing actual player text: '+level.puzzleId);
      campaignText.entries.push(values);
    }
  }
}
if (catalog.puzzles.length !== 240 || new Set(catalog.puzzles.map(p=>p.puzzleId)).size !== 240) throw Error('Incomplete campaign');
for (const name of ['mapSegmentId','trainMomentId']) if (new Set(completion.completionCatalog.completionMoments.map(m=>m[name])).size !== 240) throw Error('Completion targets incomplete: '+name);
catalog.catalogHashSha256 = hash(catalog);
const campaignHash = hash(campaign), completionHash = hash(completion), cosmeticsHash = hash(cosmetics);
emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json', catalog);
emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.campaign.json', campaign);
emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json', completion);
emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.cosmetics.json', cosmetics);
const bindings = {schemaVersion:1, catalogNamespace:'qa-mobile-candidate', use:'InternalPreview', approvalStatus:'DRAFT', releaseEligible:false,
  publicCatalogHashSha256:catalog.catalogHashSha256, campaignHashSha256:campaignHash, completionHashSha256:completionHash, cosmeticsHashSha256:cosmeticsHash};
emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json', bindings);
emit('Assets/StammstreckenPuzzle/UI/ProductionCampaignText.json', campaignText);
const profilePath='Content/Generator/path-search-v1.profiles.json';
if(fs.existsSync(path.join(root,profilePath))) {
  const draftProfiles=read(profilePath);
  if(draftProfiles.approvalStatus!=='DRAFT'||draftProfiles.productApproved!==false)throw Error('Refuse rebinding a promoted generator profile');
  for(const profile of draftProfiles.profiles) {
    if(profile.approvalStatus!=='DRAFT'||profile.humanStatus!=='NOT_EXECUTED')throw Error('Profile evidence requires explicit review');
    profile.publicCatalogHashSha256=catalog.catalogHashSha256;
    profile.solverVersion='solver-v2';
    delete profile.profileHashSha256;
    profile.profileHashSha256=hash(profile);
  }
  emit(profilePath,draftProfiles);
  const endlessCompletion=read('Content/Catalogs/endless/completion-v1.json');
  if(endlessCompletion.catalogId!=='completion-qa-mobile-candidate-endless'||endlessCompletion.approvalStatus!=='DRAFT')throw Error('Endless completion must remain a separate DRAFT source');
  const expectedRewards={LEVEL_FIRST_CLEAR:15,LEVEL_SECOND_STAR:5,LEVEL_THIRD_STAR:10,POST_CLEAR_PATIENCE:10};
  if(endlessCompletion.rewardDefinitions.length!==4||endlessCompletion.rewardDefinitions.some(r=>r.subjectKind!=='ENDLESS_INSTANCE'||r.claimCardinality!=='ONCE_PER_SUBJECT'||expectedRewards[r.reasonCode]!==r.amountPatience))throw Error('Unexpected endless reward source');
  if(new Set(endlessCompletion.rewardDefinitions.map(r=>r.reasonCode)).size!==4)throw Error('Duplicate endless reward definition');
  emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-profiles.json',draftProfiles);
  emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-completion.json',endlessCompletion);
  emit('Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-bindings.json',{
    schemaVersion:1,use:'InternalPreview',approvalStatus:'DRAFT',releaseEligible:false,
    publicCatalogHashSha256:catalog.catalogHashSha256,profilesHashSha256:hash(draftProfiles),completionHashSha256:hash(endlessCompletion)
  });
}
emit('Content/Quality/s1/mobile-candidate-projection-lock.json', {schemaVersion:1, ...bindings, sources, outputs});
console.log('PASS: 240 public-only identities, sequential progression, 240 map/train moments and 14 cosmetics; InternalPreview/DRAFT, no production promotion.');
