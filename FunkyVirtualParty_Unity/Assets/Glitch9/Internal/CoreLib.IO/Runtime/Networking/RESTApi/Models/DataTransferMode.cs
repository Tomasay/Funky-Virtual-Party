namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Data transfer mode for <see cref="RESTApiV5"/>.
    /// </summary>
    public enum DataTransferMode
    {
        /// <summary>
        /// Text(string) response received at once
        /// </summary>
        Text,

        /// <summary>
        /// binary(byte[]) response received at once
        /// </summary>
        Binary,
    }
}