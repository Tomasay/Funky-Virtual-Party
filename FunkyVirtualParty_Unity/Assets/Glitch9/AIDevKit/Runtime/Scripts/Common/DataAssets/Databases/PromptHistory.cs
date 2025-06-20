using Glitch9.Collections;
using Glitch9.ScriptableObjects;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// ScriptableObject database for storing prompt history data.
    /// This database is used to keep track of the prompts sent to the AI and their responses.
    /// It is useful for debugging and analyzing the performance of the AI.
    /// </summary> 
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class PromptHistory : ScriptableDatabase<PromptHistory.Repo, PromptRecord, PromptHistory>
    {
        /// <summary>Database for storing prompt history data.</summary>
        public class Repo : Database<PromptRecord> { }
        internal static List<PromptRecord> GetBySender(string sender = null)
        => DB.FindAll(log => log.Sender == sender).ConvertAll(log => log);

        // public static void Archive(PromptRecord record)
        // {
        //     if (record == null) return;

        //     record.Archive();
        //     // Add the record to the database
        //     DB.Add(record);
        // }
    }
}