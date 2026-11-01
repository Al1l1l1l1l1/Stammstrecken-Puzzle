using System;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Persistence;

namespace STP.Bootstrap
{
    public sealed partial class MobileCandidateExperience
    {
        private LocalDiagnosticRing? localDiagnostics;
        private MobileCandidateServices? diagnosticServices;
        /// <summary>Observer-only binding by Root after the actual profile exists. Does not start a provider or await logging IO.
        /// Initial EnsureReady without a profile may precede this; an identical live binding is idempotent.</summary>
        public void BindLocalDiagnostics(LocalDiagnosticRing ring)
        {
            if(disposed||ring==null||ring.IsClosed||(localDiagnostics!=null&&!ReferenceEquals(localDiagnostics,ring)))
                throw new InvalidOperationException("LOCAL_DIAGNOSTIC_BIND_WINDOW_CLOSED");
            localDiagnostics=ring;WireLocalDiagnosticServices();
        }
        private void WireLocalDiagnosticServices()
        {
            if(localDiagnostics==null||disposed||ReferenceEquals(diagnosticServices,services))return;
            DetachLocalDiagnosticServices();diagnosticServices=services;
            if(diagnosticServices!=null)diagnosticServices.ServiceNotice+=OnLocalDiagnosticNotice;
        }
        private void DetachLocalDiagnosticServices()
        {
            if(diagnosticServices!=null)diagnosticServices.ServiceNotice-=OnLocalDiagnosticNotice;
            diagnosticServices=null;
        }
        private void OnLocalDiagnosticNotice(string code)
        {
            if(disposed)return;var ring=localDiagnostics;if(ring==null)return;
            try{_=ObserveLocalDiagnosticAsync(ring.RecordServiceNoticeAsync(code));}catch{ /* Diagnosis cannot fail a callback or recursively log itself. */ }
        }
        private void RecordLocalDiagnosticException(Exception exception)
        {
            var ring=localDiagnostics;if(ring==null)return;
            try{_=ObserveLocalDiagnosticAsync(ring.RecordExceptionAsync(exception));}catch{ }
        }
        private void RecordLocalDiagnosticResult(LocalDiagnosticOperation operation,MobileServiceStatus status,string code)
        {
            var ring=localDiagnostics;if(ring==null)return;
            // Ring classifies fixed status/code synchronously. No Value, operation/reservation/receipt or caller identifier is inspected.
            try{_=ObserveLocalDiagnosticAsync(ring.RecordServiceResultAsync(operation,status,code));}catch{ }
        }
        private static async Task ObserveLocalDiagnosticAsync(Task actual)
        {try{await actual.ConfigureAwait(false);}catch{ /* Observe unexpected logger failure; never create another diagnostic action. */ }}
        private async Task<MobileResult<T>> WithLocalDiagnosticAsync<T>(LocalDiagnosticOperation operation,Func<Task<MobileResult<T>>> actual)where T:class
        {
            try
            {
                // Invoke inside try so synchronous provider-boundary exceptions are observed too.
                var result=await actual().ConfigureAwait(false);RecordLocalDiagnosticResult(operation,result.Status,result.Code);return result;
            }
            catch(Exception exception){RecordLocalDiagnosticException(exception);throw;}
        }
    }
}
