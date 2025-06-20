using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google
{
    public enum PersonGeneration
    {
        /// <summary>
        /// Block generation of images of people.
        /// </summary>
        [ApiEnum("None", "DONT_ALLOW")] None,

        /// <summary>
        /// Generate images of adults, but not children. This is the default.
        /// </summary>        
        [ApiEnum("Allow Adult", "ALLOW_ADULT")] AllowAdult,
    }
}