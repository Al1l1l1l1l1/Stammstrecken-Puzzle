const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const sharp=require('sharp');
const root=__dirname;const ink='#20272C',red='#C83D45',cream='#F4F2EE',ivory='#E9E6D9',rail='#815638';
const body='M266 222 H622 Q704 222 704 304 V676 Q704 764 616 764 H272 Q184 764 184 676 V304 Q184 222 266 222 Z';
const route='M156 822 H570 C716 822 836 702 836 558 V178';
const art=`<defs><clipPath id="train-face"><path d="${body}"/></clipPath></defs>
<path d="${route}" fill="none" stroke="${ink}" stroke-width="84" stroke-linecap="round" stroke-linejoin="round"/>
<path d="${route}" fill="none" stroke="${rail}" stroke-width="52" stroke-linecap="round" stroke-linejoin="round"/>
<rect x="278" y="170" width="332" height="80" rx="20" fill="#495256" stroke="${ink}" stroke-width="32"/>
<rect x="246" y="714" width="68" height="92" rx="14" fill="${ink}"/>
<rect x="574" y="714" width="68" height="92" rx="14" fill="${ink}"/>
<path d="${body}" fill="${red}"/>
<g clip-path="url(#train-face)"><rect x="176" y="538" width="536" height="108" fill="${ivory}"/></g>
<path d="${body}" fill="none" stroke="${ink}" stroke-width="32" stroke-linejoin="round"/>
<rect x="234" y="286" width="420" height="214" rx="26" fill="#394E58" stroke="${ink}" stroke-width="24"/>
<path d="M444 296 V490" stroke="${ivory}" stroke-width="24"/>
<path d="M274 324 H338 L274 388 Z" fill="#9EAFAE" opacity=".4"/>
<path d="M480 324 H544 L480 388 Z" fill="#9EAFAE" opacity=".4"/>
<circle cx="278" cy="592" r="29" fill="${cream}" stroke="${ink}" stroke-width="16"/>
<circle cx="610" cy="592" r="29" fill="${cream}" stroke="${ink}" stroke-width="16"/>
<rect x="360" y="692" width="168" height="32" rx="10" fill="${ink}"/>`;
const wrap=(inside)=>`<svg xmlns="http://www.w3.org/2000/svg" width="1024" height="1024" viewBox="0 0 1024 1024">${inside}</svg>\n`;
const master=wrap(`<rect width="1024" height="1024" fill="${cream}"/>${art}`);
const adaptiveScale=.56;
const foreground=wrap(`<g transform="translate(512 512) scale(${adaptiveScale}) translate(-512 -512)">${art}</g>`);
const background=wrap(`<rect width="1024" height="1024" fill="${cream}"/>`);
const monochrome=wrap(`<defs><mask id="symbol"><rect width="1024" height="1024" fill="black"/><g transform="translate(512 512) scale(${adaptiveScale}) translate(-512 -512)">
<path d="${route}" fill="none" stroke="white" stroke-width="84" stroke-linecap="round"/>
<rect x="278" y="170" width="332" height="80" rx="20" fill="white"/>
<rect x="246" y="714" width="68" height="92" rx="14" fill="white"/><rect x="574" y="714" width="68" height="92" rx="14" fill="white"/>
<path d="${body}" fill="white" stroke="white" stroke-width="32"/>
<rect x="234" y="286" width="420" height="214" rx="26" fill="black"/><path d="M444 296 V490" stroke="white" stroke-width="24"/>
<circle cx="278" cy="592" r="29" fill="black"/><circle cx="610" cy="592" r="29" fill="black"/><rect x="360" y="692" width="168" height="32" rx="10" fill="black"/>
</g></mask></defs><rect width="1024" height="1024" fill="white" mask="url(#symbol)"/>`);
const sources={'stp-appicon-master.svg':master,'stp-appicon-adaptive-foreground.svg':foreground,'stp-appicon-adaptive-background.svg':background,'stp-appicon-monochrome.svg':monochrome};
function hash(bytes){return crypto.createHash('sha256').update(bytes).digest('hex');}
async function render(svg,size,opaque){let pipeline=sharp(Buffer.from(svg),{density:384}).resize(size,size,{kernel:'lanczos3'}).toColourspace('srgb');if(opaque)pipeline=pipeline.flatten({background:cream}).removeAlpha();return pipeline.png({compressionLevel:9,adaptiveFiltering:false}).toBuffer();}
async function write(name,bytes){fs.writeFileSync(path.join(root,name),bytes);return {file:name,bytes:bytes.length,sha256:hash(bytes)};}
(async()=>{
 fs.mkdirSync(root,{recursive:true});const generated=[];
 for(const [name,text] of Object.entries(sources))generated.push(await write(name,Buffer.from(text)));
 const sizes=[1024,512,256,192,180,167,152,144,128,120,114,96,87,80,76,72,64,60,58,57,48,40,36,32,29,24,20,16];
 for(const size of sizes){const png=await render(master,size,true);const metadata=await sharp(png).metadata();if(metadata.width!==size||metadata.height!==size||metadata.hasAlpha)throw new Error('ICON_OPAQUE_SIZE_INVALID:'+size);generated.push(await write(`stp-appicon-${size}.png`,png));}
 for(const [kind,svg,opaque] of [['foreground',foreground,false],['background',background,true],['monochrome',monochrome,false]])for(const size of [1024,432])generated.push(await write(`stp-appicon-adaptive-${kind}-${size}.png`,await render(svg,size,opaque)));
 const raw=await sharp(await render(foreground,1024,false)).ensureAlpha().raw().toBuffer();let maxRadius=0,alphaPixels=0;
 for(let y=0;y<1024;y++)for(let x=0;x<1024;x++)if(raw[(y*1024+x)*4+3]>1){alphaPixels++;maxRadius=Math.max(maxRadius,Math.hypot(x+.5-512,y+.5-512));}
 const safeRadius=1024*66/108/2;if(maxRadius>safeRadius)throw new Error('ADAPTIVE_SAFE_ZONE_EXCEEDED:'+maxRadius);
 const mask=(shape)=>wrap(`<defs><clipPath id="mask">${shape==='circle'?'<circle cx="512" cy="512" r="512"/>':'<rect width="1024" height="1024" rx="224"/>'}</clipPath></defs><g clip-path="url(#mask)"><rect width="1024" height="1024" fill="${cream}"/><g transform="translate(512 512) scale(1.4) translate(-512 -512)">${foreground.replace(/^<svg[^>]+>|<\/svg>\s*$/g,'')}</g></g>`);
 const contactBackground=Buffer.from('<svg xmlns="http://www.w3.org/2000/svg" width="900" height="430"><rect width="900" height="430" fill="#DDDCD6"/><g fill="#20272C" font-family="sans-serif" font-size="16"><text x="22" y="26">FULL SQUARE · 256</text><text x="316" y="26">128 / 64 / 48 / 32 / 24 / 16</text><text x="316" y="214">ADAPTIVE · MASK PREVIEWS</text></g></svg>');
 const composites=[{input:await render(master,256,true),left:22,top:46}];let x=316;
 for(const size of [128,64,48,32,24,16]){composites.push({input:await render(master,size,true),left:x,top:46});x+=size+16;}
 composites.push({input:await render(mask('circle'),164,false),left:316,top:236});composites.push({input:await render(mask('square'),164,false),left:510,top:236});composites.push({input:await render(monochrome,164,false),left:704,top:236});
 generated.push(await write('preview-sizes-and-masks.png',await sharp(contactBackground).composite(composites).png().toBuffer()));
 const shapeSource='Assets/StammstreckenPuzzle/Scripts/Presentation/UI/RailwayIllustrationElement.cs';
 const provenance={schemaVersion:1,status:'DRAFT_PRODUCT_ICON_REVIEW',created:'2026-10-21',authorship:'Project-original procedural SVG authored by Codex/Bahn A for Stammstrecken-Puzzle. No external imagery, logos, fonts or textures in icon masters.',styleSourceCommit:'9e3f209cd92d491ab3a1ab51cce63677d3b5d53e',runtimeDrawingReference:{file:shapeSource,sha256:hash(fs.readFileSync(path.resolve(shapeSource))),reusedDesignLanguage:'Original RailwayDrawing train body, roof bar, glazing, cream band, dark bogies and thick ink contours; new front projection and one puzzle rail curve.'},palette:{ink,trainRed:red,background:cream,band:ivory,track:rail},rendering:{engine:'sharp/libvips/librsvg',sharp:sharp.versions.sharp,vips:sharp.versions.vips,inputDpi:384,outputColourSpace:'sRGB',resizeKernel:'lanczos3',masterSizes:sizes,opaqueSquarePngs:true,bakedCornerMask:false},adaptive:{canvasDp:108,guaranteedSafeZoneDiameterDp:66,foregroundScale:adaptiveScale,safeRadiusPixelsAt1024:safeRadius,maxNontransparentRadiusAt1024:maxRadius,nontransparentPixels:alphaPixels,foregroundAndBackgroundSeparate:true,monochromeSeparate:true},guidance:[{url:'https://developer.android.com/develop/ui/compose/system/icon_design_adaptive',checked:'2026-10-21',use:'108dp layers and central66dp safe zone; mask previews only.'},{url:'https://developer.apple.com/design/human-interface-guidelines/app-icons',checked:'2026-10-21',use:'1024px square master; system-applied masking; no baked corners. Native export assignment remains Root-owned.'}],acceptance:{renderedAndInspectedByAgent:false,ownerVisualAcceptance:false,deviceLauncherCheck:false,storeAcceptance:false},files:generated,generatorSha256:hash(fs.readFileSync(__filename))};
 fs.writeFileSync(path.join(root,'provenance.json'),JSON.stringify(provenance,null,2)+'\n');
 console.log(JSON.stringify({generated:generated.length,master1024:'stp-appicon-1024.png',preview:'preview-sizes-and-masks.png',maxRadius,safeRadius}));
})().catch(error=>{console.error(error);process.exitCode=1;});
