#!/usr/bin/env python3
"""Archive and inspect one actual Candidate iOS source export; no Xcode, native link, IPA or device attestation."""
from pathlib import Path
import argparse
import hashlib
import json
import plistlib
import re
import shutil
import sys
import zipfile


def non_empty(value: str) -> str:
    if not value.strip():
        raise argparse.ArgumentTypeError("A non-empty explicit value is required")
    return value


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--build-label", type=non_empty, required=True, help="Explicit identity of this actual source export")
    parser.add_argument("--source-binding", type=non_empty, required=True, help="Caller-provided build-input description; not independently attested")
    parser.add_argument("--export", type=Path, required=True, help="Existing emitted Candidate Xcode source project")
    parser.add_argument("--output", type=Path, required=True, help="New archive directory; never overwritten and never inside the export")
    parser.add_argument("--build-log", type=Path, required=True, help="Original Unity export log")
    parser.add_argument("--editor-report", type=Path, help="Optional original report for this exact export, never guessed from a later build")
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[2]
    export = args.export.resolve()
    out = args.output.resolve()
    if not export.is_dir():
        parser.error("The export directory does not exist")
    if out == export or export in out.parents:
        parser.error("The archive must be outside the source export")
    if not args.build_log.is_file() or (args.editor_report is not None and not args.editor_report.is_file()):
        parser.error("An explicitly supplied original log/report is missing")
    out.mkdir(parents=True, exist_ok=False)
    def sha(path):
        return hashlib.sha256(path.read_bytes()).hexdigest()
    tokens=re.findall(r'//[^\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|[{}()=;,]|[^\s{}()=;,]+',(export/'Unity-iPhone.xcodeproj/project.pbxproj').read_text(encoding='utf-8'))
    tokens=[t for t in tokens if not t.startswith(('//','/*'))]
    i=0
    def atom(t):return json.loads(t) if t.startswith('"') else t
    def parse():
     nonlocal i
     token=tokens[i];i+=1
     if token=='{':
      value={}
      while tokens[i]!='}':
       key=atom(tokens[i]);i+=1;assert tokens[i]=='=';i+=1;value[key]=parse();assert tokens[i]==';';i+=1
      i+=1;return value
     if token=='(':
      value=[]
      while tokens[i]!=')':
       value.append(parse())
       if tokens[i]==',':i+=1
      i+=1;return value
     return atom(token)
    project=parse();objects=project['objects'];problems=[]
    targets={}
    for identifier,target in objects.items():
     if target.get('isa')!='PBXNativeTarget':continue
     sources=[];frameworks=[]
     for phase_id in target.get('buildPhases',[]):
      phase=objects[phase_id]
      for build_id in phase.get('files',[]):
       file=objects[objects[build_id]['fileRef']]
       if phase['isa']=='PBXSourcesBuildPhase':sources.append(file.get('path',file.get('name')))
       if phase['isa']=='PBXFrameworksBuildPhase':frameworks.append(file.get('path',file.get('name')))
     configs={}
     for config_id in objects[target['buildConfigurationList']]['buildConfigurations']:
      config=objects[config_id];values=config['buildSettings']
      keys=('SWIFT_VERSION','ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES','IPHONEOS_DEPLOYMENT_TARGET','PRODUCT_BUNDLE_IDENTIFIER','LD_RUNPATH_SEARCH_PATHS','CLANG_ENABLE_MODULES')
      configs[config['name']]={k:values.get(k) for k in keys}
     targets[target['name']]={'sources':sources,'frameworks':frameworks,'configs':configs}
    framework=targets['UnityFramework'];main=targets['Unity-iPhone']
    for filename in ('StpStoreKitVerifier.swift','StpHaptics.mm','StpPosixFileBridge.mm'):
     if sum(Path(p).name==filename for p in framework['sources'])!=1:problems.append('FRAMEWORK_SOURCE_OWNER:'+filename)
     if any(Path(p).name==filename for p in main['sources']):problems.append('DUPLICATE_APP_SOURCE_OWNER:'+filename)
    for name,target in (('UnityFramework',framework),('Unity-iPhone',main)):
     for config,values in target['configs'].items():
      if values['SWIFT_VERSION']!='5.0':problems.append('SWIFT_VERSION:'+name+':'+config)
      if values['IPHONEOS_DEPLOYMENT_TARGET']!='15.0':problems.append('DEPLOYMENT_TARGET:'+name+':'+config)
      if values['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES']!=('NO' if name=='UnityFramework' else 'YES'):problems.append('SWIFT_EMBEDDING:'+name+':'+config)
      if name=='Unity-iPhone' and values['PRODUCT_BUNDLE_IDENTIFIER']!='com.STP.StammstreckenPuzzle.qa':problems.append('CANDIDATE_ID:'+config)
    if not any(Path(p).name=='StoreKit.framework' for p in framework['frameworks']):problems.append('STOREKIT_LINK_MISSING')
    plist=plistlib.loads((export/'Info.plist').read_bytes())
    if plist.get('GADDelayAppMeasurementInit') is not True:problems.append('MEASUREMENT_DELAY_REQUIRED')
    if 'NSUserTrackingUsageDescription' in plist:problems.append('UNEXPECTED_ATT_CONFIGURATION')
    if plist.get('GADApplicationIdentifier')!='ca-app-pub-3940256099942544~1458002511':problems.append('QA_GOOGLE_TEST_APP_ID_REQUIRED')
    source_bindings=[]
    for filename in ('StpStoreKitVerifier.swift','StpHaptics.mm','StpPosixFileBridge.mm'):
     source=root/'Assets/Plugins/iOS'/filename;copied=export/'Libraries/Plugins/iOS'/filename
     source_bindings.append({'source':source.relative_to(root).as_posix(),'export':copied.relative_to(export).as_posix(),'sourceSha256':sha(source),'exportSha256':sha(copied)})
     if sha(source)!=sha(copied):problems.append('SOURCE_EXPORT_HASH_MISMATCH:'+filename)
    hook_report=dict(line.split('=',1) for line in (export/'stp-native-privacy-source.txt').read_text(encoding='utf-8').splitlines() if '=' in line)
    for key,relative in (('InfoPlistSha256','Info.plist'),('SwiftSourceSha256','Libraries/Plugins/iOS/StpStoreKitVerifier.swift'),('PBXProjectSha256','Unity-iPhone.xcodeproj/project.pbxproj')):
     if hook_report.get(key)!=sha(export/relative):problems.append('HOOK_REPORT_HASH_MISMATCH:'+key)
    pod=(export/'Podfile').read_text(encoding='utf-8')
    for required in ("platform :ios, '15.0'","pod 'Google-Mobile-Ads-SDK', '~> 13.9'","pod 'GoogleUserMessagingPlatform', '3.1.0'"):
     if required not in pod:problems.append('POD_CONFIGURATION_MISMATCH:'+required)
    privacy=[]
    for p in export.rglob('*.xcprivacy'):
     privacy.append({'path':p.relative_to(export).as_posix(),'sha256':sha(p),'declaration':plistlib.loads(p.read_bytes())})
    files=[{'path':p.relative_to(export).as_posix(),'sizeBytes':p.stat().st_size,'sha256':sha(p)} for p in sorted(export.rglob('*')) if p.is_file()]
    archive=out/'ios-exported-project-source.zip'
    with zipfile.ZipFile(archive,'x',zipfile.ZIP_DEFLATED,compresslevel=1) as z:
     for entry in files:z.write(export/entry['path'],entry['path'])
    shutil.copy2(args.build_log.resolve(),out/'unity-ios-export.log')
    if args.editor_report is not None:
     shutil.copy2(args.editor_report.resolve(),out/'LastBuild.buildreport')
    for relative in ('Info.plist','Podfile','stp-native-privacy-source.txt','Unity-iPhone.xcodeproj/project.pbxproj','Libraries/Plugins/iOS/StpStoreKitVerifier.swift','Libraries/Plugins/iOS/StpHaptics.mm','Libraries/Plugins/iOS/StpPosixFileBridge.mm','UnityFramework/PrivacyInfo.xcprivacy','UnityFramework/ApplePrivacyManifestsMerge.txt'):
     dest=out/relative;dest.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(export/relative,dest)
    evidence={'evidenceKind':'ACTUAL_IOS_EXPORTED_PROJECT_SOURCE_INSPECTION_ONLY','buildLabel':args.build_label,'sourceBinding':args.source_binding,'sourceBindingIsCallerDeclaration':True,
     'targets':{name:{'nativeHooks':[p for p in t['sources'] if Path(p).name in ('StpStoreKitVerifier.swift','StpHaptics.mm','StpPosixFileBridge.mm')],'storeKitLinks':[p for p in t['frameworks'] if Path(p).name=='StoreKit.framework'],'configs':t['configs']} for name,t in targets.items() if name in ('UnityFramework','Unity-iPhone')},
     'plist':{k:plist.get(k) for k in ('GADDelayAppMeasurementInit','GADApplicationIdentifier','NSUserTrackingUsageDescription','CFBundleIdentifier','CFBundleVersion')},'podfile':pod,'podfileLockPresent':(export/'Podfile.lock').exists(),'podsDirectoryPresent':(export/'Pods').exists(),
     'sourceBindings':source_bindings,'hookReport':hook_report,'privacyManifestDeclarations':privacy,'problems':problems,'files':files,
     'archive':{'path':archive.relative_to(out).as_posix(),'sha256':sha(archive),'sizeBytes':archive.stat().st_size},
     'log':{'path':'unity-ios-export.log','sha256':sha(out/'unity-ios-export.log')},'buildReportSha256':sha(out/'LastBuild.buildreport') if args.editor_report is not None else None,
     'xcodeCompileValidated':False,'swiftNativeLinkValidated':False,'podResolutionValidated':False,'ipaCreated':False,'productionSigned':False,'physicalDeviceValidated':False,'sandboxValidated':False,'physicalPrivacyValidated':False,'legalApproved':False,'promotionAllowed':False}
    (out/'audit.json').write_text(json.dumps(evidence,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'archiveDirectory':str(out),'buildLabel':args.build_label,'problems':problems,'fileCount':len(files),'archiveBytes':archive.stat().st_size,'archiveSha256':sha(archive)}))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
