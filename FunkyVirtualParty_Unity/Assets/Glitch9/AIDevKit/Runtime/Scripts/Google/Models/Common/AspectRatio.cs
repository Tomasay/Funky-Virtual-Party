using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google
{
    public enum AspectRatio
    {
        /// <summary>
        /// 1:1
        /// </summary>
        [ApiEnum("1:1")] Square,

        /// <summary>
        /// 3:4
        /// </summary>
        [ApiEnum("3:4")] Portrait,

        /// <summary>
        /// 4:3
        /// </summary>
        [ApiEnum("4:3")] Landscape,

        /// <summary>
        /// 9:16
        /// </summary>
        [ApiEnum("9:16")] Vertical,

        /// <summary>
        /// 16:9
        /// </summary>
        [ApiEnum("16:9")] Horizontal,
    }
}