using System;
using System.Threading;
using System.Threading.Tasks;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using UnityEngine;

namespace STP.Bootstrap
{
    public sealed partial class MobilePreviewInstaller
    {
        private LocalDiagnosticRing? localDiagnostics;
        private Task? diagnosticClose;

        // The repository owns the profile location. No profile means no diagnostic writer.
        private void TryStartLocalDiagnostics()
        {
            if(disposed||localDataInputFence||deletionOwnersClosed||localDiagnostics!=null||
                diagnosticClose!=null||fullGame==null||experience==null||model?.ConfirmedSnapshot==null)return;
            try
            {
                string build;
                if(UnityEngine.Application.isEditor)build="wp046-editor-mobile-preview";
                else
                {
                    string guid=UnityEngine.Application.buildGUID;
                    if(!Guid.TryParseExact(guid,"N",out _)||guid.Length!=32)return;
                    build=guid.Substring(0,12).ToLowerInvariant();
                }
                var platform=UnityEngine.Application.isEditor?LocalDiagnosticPlatform.Editor:
                    UnityEngine.Application.platform==RuntimePlatform.Android?LocalDiagnosticPlatform.Android:
                    UnityEngine.Application.platform==RuntimePlatform.IPhonePlayer?LocalDiagnosticPlatform.IOS:LocalDiagnosticPlatform.Other;
                var ring=new LocalDiagnosticRing(fullGame.Repository.ProfileDirectory,new CanonicalJsonAdapter(),
                    new LocalDiagnosticEnvironment(build,UnityEngine.Application.version,platform),production:true);
                experience.BindLocalDiagnostics(ring);
                localDiagnostics=ring;
                ObserveDiagnosticTask(ring.RecordAsync(LocalDiagnosticEvent.AppStarted));
            }
            catch(Exception){/* Diagnostics cannot grant or prevent gameplay. */}
        }

        private void RecordLocalDiagnosticException(Exception exception)
        {
            try{if(localDiagnostics!=null)ObserveDiagnosticTask(localDiagnostics.RecordExceptionAsync(exception));}
            catch(Exception){/* Do not substitute a logging failure for the original outcome. */}
        }

        private void RecordLocalDiagnosticEvent(LocalDiagnosticEvent code)
        {
            try{if(localDiagnostics!=null)ObserveDiagnosticTask(localDiagnostics.RecordAsync(code));}
            catch(Exception){/* The original save result remains authoritative. */}
        }

        private Task CloseLocalDiagnosticsAsync()
        {
            if(diagnosticClose!=null)return diagnosticClose;
            if(localDiagnostics==null)return Task.CompletedTask;
            try{ObserveDiagnosticTask(localDiagnostics.RecordAsync(LocalDiagnosticEvent.AppStopped));}
            catch(Exception){/* Closing still seals every previously started diagnostic operation. */}
            return diagnosticClose=localDiagnostics.CloseAndDrainAsync();
        }

        private static void ObserveDiagnosticTask(Task work)
        {
            _=work.ContinueWith(done=>{_ = done.Exception;},CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously|TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);
        }
    }
}
