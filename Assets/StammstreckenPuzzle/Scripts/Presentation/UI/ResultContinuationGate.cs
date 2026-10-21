using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Presentation.UI
{
    /// <summary>Observes one explicit result continuation policy task without giving that task game or save authority.</summary>
    public sealed class ResultContinuationGate:IDisposable
    {
        private Func<Task>? beforeContinue;
        private Task? pending;
        private Guid expectedAttempt;
        private bool disposed;
        /// <summary>True only while one accepted explicit result request remains unobserved.</summary>
        public bool IsPending=>pending!=null;
        /// <summary>Binds Root's optional service policy; binding never invokes it.</summary>
        public void Bind(Func<Task> before)
        {if(disposed)throw new ObjectDisposedException(nameof(ResultContinuationGate));if(pending!=null)throw new InvalidOperationException("RESULT_CONTINUATION_BUSY");beforeContinue=before??throw new ArgumentNullException(nameof(before));}
        /// <summary>Starts only for an actual eligible Results attempt; repeated taps cannot start another operation.</summary>
        public bool TryBegin(bool actualResults,Guid attempt)
        {
            if(disposed||pending!=null||!actualResults||attempt==Guid.Empty)return false;expectedAttempt=attempt;
            try{pending=beforeContinue?.Invoke()??Task.CompletedTask;}catch{pending=Task.CompletedTask;}
            return true;
        }
        /// <summary>Observes any outcome neutrally; continuation is permitted only for the same still-current result.</summary>
        public bool Poll(bool actualResults,Guid attempt)
        {
            if(disposed||pending==null||!pending.IsCompleted)return false;var completed=pending;pending=null;
            try{completed.GetAwaiter().GetResult();}catch{ /* Optional service failure cannot become a navigation lock. */ }
            return actualResults&&attempt==expectedAttempt;
        }
        /// <summary>Retains a completed policy while the same actual result is temporarily saving or suspended.</summary>
        public bool PollWhenReady(bool currentResult,bool canNavigate,Guid attempt)
        {
            if(currentResult&&attempt==expectedAttempt&&!canNavigate)return false;
            return Poll(currentResult&&canNavigate,attempt);
        }
        /// <summary>Observes late failures while discarding any future navigation permission.</summary>
        public void Dispose()
        {
            if(disposed)return;disposed=true;beforeContinue=null;
            if(pending!=null)_=pending.ContinueWith(task=>{_ = task.Exception;},CancellationToken.None,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);
            pending=null;
        }
    }
}
