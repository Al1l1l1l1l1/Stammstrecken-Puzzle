/* Projects the actual generic endless PLAYER result text, retaining its source provenance and DRAFT status. */
const fs=require('fs'),crypto=require('crypto');
const source='Content/Localization/source/endless-de-DE.json',target=process.argv.find(a=>a.startsWith('--target='))?.slice(9)||'Assets/StammstreckenPuzzle/UI/ProductionGameLocale.json';
const bytes=fs.readFileSync(source),document=JSON.parse(bytes),entry=document.entries.find(e=>e.key==='endless.result'&&e.audience==='PLAYER'&&e.status==='DRAFT'&&Object.keys(e.placeholders).length===0);
if(document.locale!=='de-DE'||!entry)throw Error('Actual endless player source missing');
const locale=JSON.parse(fs.readFileSync(target,'utf8')),projected=locale.entries.find(e=>e.key==='product.endless.puzzle.result');if(!projected)throw Error('Target locale entry missing');
const binding={source,sha256:crypto.createHash('sha256').update(bytes).digest('hex'),sourceKey:entry.key,targetKey:projected.key,approvalStatus:'DRAFT'};
if(process.argv.includes('--check')){
 if(projected.value!==entry.text||projected.argumentTypes.length!==0||!(locale.sourceBindings||[]).some(b=>JSON.stringify(b)===JSON.stringify(binding)))throw Error('Endless player projection drift');
}else{projected.value=entry.text;projected.argumentTypes=[];locale.sourceBindings=(locale.sourceBindings||[]).filter(b=>b.targetKey!==projected.key).concat([binding]);fs.writeFileSync(target,JSON.stringify(locale,null,2)+'\n');}
console.log(JSON.stringify({check:process.argv.includes('--check'),sourceKey:entry.key,targetKey:projected.key,sourceSha256:binding.sha256}));
