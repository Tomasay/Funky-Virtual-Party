namespace Glitch9.IO.Networking
{
    /// <summary>
    /// Interface for objects that have an ID which is stored in the cloud.
    /// This interface indicates that only the ID of the object is stored in the cloud,
    /// while the rest of the object's data is managed locally or elsewhere.
    /// </summary>
    public interface IOnlyId
    {
        /// <summary>
        /// Gets the cloud-stored ID of the object.
        /// </summary>
        /// <returns>A string representing the ID of the object stored in the cloud.</returns>
        string GetId();
    }
}