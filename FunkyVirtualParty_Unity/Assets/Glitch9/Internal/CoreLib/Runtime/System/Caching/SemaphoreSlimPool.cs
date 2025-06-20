using System.Threading;
using UnityEngine.Pool;

namespace Glitch9
{
    /// <summary>
    /// Provides a pool of <see cref="SemaphoreSlim"/> objects to manage resource usage efficiently.
    /// </summary>
    public static class SemaphoreSlimPool
    {
        /// <summary>
        /// Internal pool of <see cref="SemaphoreSlim"/> objects.
        /// </summary>
        internal static readonly ObjectPool<SemaphoreSlim> Pool = new(() => new SemaphoreSlim(1, 1), null, semaphore => semaphore.Wait(0));

        /// <summary>
        /// Gets a <see cref="SemaphoreSlim"/> from the pool.
        /// </summary>
        /// <returns>A <see cref="SemaphoreSlim"/> instance.</returns>
        public static SemaphoreSlim Get() => Pool.Get();

        /// <summary>
        /// Gets a pooled <see cref="SemaphoreSlim"/> and outputs it.
        /// </summary>
        /// <param name="semaphore">The <see cref="SemaphoreSlim"/> instance.</param>
        /// <returns>A pooled <see cref="SemaphoreSlim"/> instance.</returns>
        public static PooledObject<SemaphoreSlim> GetAsync(out SemaphoreSlim semaphore) => Pool.Get(out semaphore);

        /// <summary>
        /// Releases a <see cref="SemaphoreSlim"/> back to the pool.
        /// </summary>
        /// <param name="toRelease">The <see cref="SemaphoreSlim"/> to release.</param>
        public static void Release(SemaphoreSlim toRelease) => Pool.Release(toRelease);
    }
}