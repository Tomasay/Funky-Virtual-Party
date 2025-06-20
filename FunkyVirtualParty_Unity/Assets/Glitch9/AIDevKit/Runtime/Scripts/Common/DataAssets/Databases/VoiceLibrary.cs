using System.Collections.Generic;
using UnityEngine;
using Glitch9.Collections;
using Glitch9.ScriptableObjects;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// ScriptableObject database for storing voice data used for TTS (Text-to-Speech) and other voice-related tasks.
    /// </summary> 
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class VoiceLibrary : ScriptableDatabase<VoiceLibrary.Repo, Voice, VoiceLibrary>
    {
        /// <summary>Database for storing voice data.</summary>
        public class Repo : Database<Voice>
        {
#if UNITY_EDITOR
            /// <summary>Initializes the database.</summary>
            public Repo()
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this == null) return;

                    if (Count == 0)
                    {
                        if (!InitialLoad())
                        {
                            Debug.LogError("There is no voice in your library. Please add voices to the library.");
                        }
                    }

                    // const string kPrefsKey = "AIDevKit.VoiceLibrary.Setup";

                    // bool initialSetup = UnityEditor.EditorPrefs.GetBool(kPrefsKey, false);
                    // if (!initialSetup)
                    // {
                    //     if (InitialLoad())
                    //     {
                    //         UnityEditor.EditorPrefs.SetBool(kPrefsKey, true);
                    //     }
                    // }
                };
            }
#endif
        }

        internal static Dictionary<Api, List<Voice>> GetFilteredRefs(VoiceFilter filter)
        {
            Dictionary<Api, List<Voice>> map = new();

            foreach (Voice voice in DB.Values)
            {
                if (voice == null) continue;

                if (filter.IsEmpty || filter.Matches(voice))
                {
                    if (!map.ContainsKey(voice.Api)) map.Add(voice.Api, new());
                    map[voice.Api].Add(voice);
                }
            }

            return map;
        }

        public static List<Voice> GetVoicesByAPI(Api api)
        {
            List<Voice> voices = new();

            foreach (Voice voice in DB.Values)
            {
                if (voice == null) continue;
                if (voice.Api == api) voices.Add(voice);
            }

            return voices;
        }
    }
}