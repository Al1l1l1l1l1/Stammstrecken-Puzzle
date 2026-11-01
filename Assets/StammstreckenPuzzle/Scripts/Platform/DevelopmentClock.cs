using System.Diagnostics;
using STP.Application.Ports;

namespace STP.Platform
{
    /// <summary>Instance-local monotonic clock for the transient QA session, with no wall-clock or save effects.</summary>
    public sealed class DevelopmentClock : IClock
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        /// <summary>Monotonic milliseconds since this composition was created.</summary>
        public long Milliseconds => stopwatch.ElapsedMilliseconds;
    }
}
