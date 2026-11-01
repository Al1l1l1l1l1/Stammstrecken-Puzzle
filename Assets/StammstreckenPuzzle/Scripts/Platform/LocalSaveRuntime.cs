using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Platform
{
    /// <summary>Instance-local process time; UTC is diagnostic and never measures puzzle time.</summary>
    public sealed class LocalSaveClock : ILocalSessionClock
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        /// <summary>Milliseconds since this clock was constructed.</summary>
        public long MonotonicMilliseconds => stopwatch.ElapsedMilliseconds;
        /// <summary>Invariant RFC3339 UTC diagnostic time.</summary>
        public string UtcNow => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", CultureInfo.InvariantCulture);
    }

    /// <summary>Instance-owned application publication queue; construction and Drain belong to Unity main.</summary>
    public sealed class MainThreadSaveDispatcher : ILocalSaveDispatcher, IDisposable
    {
        private sealed class Pending
        {
            internal readonly Action Action;
            internal readonly TaskCompletionSource<bool> Completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            internal Pending(Action action) { Action = action; }
        }
        private readonly int owner = Thread.CurrentThread.ManagedThreadId;
        private readonly object gate = new object();
        private readonly Queue<Pending> queue = new Queue<Pending>();
        private bool disposed;

        /// <summary>Captures the logical owner; the composition root calls this on Unity main.</summary>
        public MainThreadSaveDispatcher() { }
        /// <summary>Runs inline on the owner or queues an observed publication from a worker.</summary>
        public Task InvokeAsync(Action publication)
        {
            if (publication == null) throw new ArgumentNullException(nameof(publication));
            lock (gate)
            {
                if (disposed) return Task.FromException(new ObjectDisposedException(nameof(MainThreadSaveDispatcher)));
                if (Thread.CurrentThread.ManagedThreadId != owner)
                {
                    var pending = new Pending(publication); queue.Enqueue(pending); return pending.Completion.Task;
                }
            }
            try { publication(); return Task.CompletedTask; }
            catch (Exception error) { return Task.FromException(error); }
        }
        /// <summary>Publishes queued work on the captured owner; a worker cannot drain Unity publications.</summary>
        public void Drain()
        {
            RequireOwner();
            // Bound this drain to its entry inventory; a continuous producer cannot monopolize a frame.
            int count; lock (gate) { if (disposed) return; count = queue.Count; }
            for (int i = 0; i < count; i++)
            {
                Pending pending; lock (gate) { if (disposed || queue.Count == 0) return; pending = queue.Dequeue(); }
                try { pending.Action(); pending.Completion.TrySetResult(true); }
                catch (Exception error) { pending.Completion.TrySetException(error); }
            }
        }
        /// <summary>Rejects future work and completes every waiting publication without running it.</summary>
        public void Dispose()
        {
            RequireOwner();
            lock (gate)
            {
                if (disposed) return; disposed = true;
                while (queue.Count > 0) queue.Dequeue().Completion.TrySetException(new ObjectDisposedException(nameof(MainThreadSaveDispatcher)));
            }
        }
        private void RequireOwner()
        {
            if (Thread.CurrentThread.ManagedThreadId != owner) throw new InvalidOperationException("SAVE_DISPATCH_OWNER_REQUIRED");
        }
    }
}
