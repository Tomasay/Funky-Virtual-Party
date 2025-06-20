using Glitch9.Collections;
using Glitch9.ScriptableObjects;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// ScriptableObject database for storing file data.
    /// This database is used to keep track of the files available in the AI library.
    /// </summary> 
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class FileLibrary : ScriptableDatabase<FileLibrary.Repo, ApiFile, FileLibrary>
    {
        /// <summary>Database for storing file data.</summary>
        public class Repo : Database<ApiFile> { }
    }
}