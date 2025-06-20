using System.Text;
using UnityEngine.Pool;

namespace Glitch9
{
    /// <summary>
    /// Provides a pool of <see cref="StringBuilder"/> objects to manage resource usage efficiently.
    /// </summary>
    public static class StringBuilderPool
    {
        /// <summary>
        /// Internal pool of <see cref="StringBuilder"/> objects.
        /// </summary>
        internal static readonly ObjectPool<StringBuilder> Pool = new(() => new StringBuilder(), null, sb => sb.Clear());

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> from the pool.
        /// </summary>
        /// <returns>A <see cref="StringBuilder"/> instance.</returns>
        public static StringBuilder Get() => Pool.Get();

        /// <summary>
        /// Gets a pooled <see cref="StringBuilder"/> and outputs it.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> instance.</param>
        /// <returns>A pooled <see cref="StringBuilder"/> instance.</returns>
        public static PooledObject<StringBuilder> Get(out StringBuilder value) => Pool.Get(out value);

        /// <summary>
        /// Releases a <see cref="StringBuilder"/> back to the pool.
        /// </summary>
        /// <param name="toRelease">The <see cref="StringBuilder"/> to release.</param>
        public static void Release(StringBuilder toRelease) => Pool.Release(toRelease);
    }
}