using System.Security.Cryptography;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Uniform byte rejection sampling; no modulo bias and no persisted seed or user identifier.</summary>
    public sealed class LocalDailyRandom : ILocalDailyRandom
    {
        /// <summary>Returns each value 0–99 with equal probability.</summary>
        public int Next100()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var value = new byte[1]; do { random.GetBytes(value); } while (value[0] >= 200); return value[0] % 100;
            }
        }
    }
}
