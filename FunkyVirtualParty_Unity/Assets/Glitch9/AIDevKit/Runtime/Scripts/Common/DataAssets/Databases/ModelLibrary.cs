using System.Collections.Generic;
using UnityEngine;
using Glitch9.Collections;
using Glitch9.ScriptableObjects;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// ScriptableObject database for storing model data.
    /// This database is used to keep track of the models available in the AI library.
    /// </summary> 
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class ModelLibrary : ScriptableDatabase<ModelLibrary.Repo, Model, ModelLibrary>
    {
        /// <summary>Database for storing model data.</summary>
        public class Repo : Database<Model>
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
                            Debug.LogError("There is no model in your library. Please add models to the library.");
                        }
                    }

                    // const string kPrefsKey = "AIDevKit.ModelLibrary.SetupV2";

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

        internal static Dictionary<Api, List<Model>> GetFilteredRefs(ModelFilter filter)
        {
            Dictionary<Api, List<Model>> map = new();

            foreach (Model model in DB.Values)
            {
                if (model == null) continue;

                if (filter.IsEmpty || filter.Matches(model))
                {
                    if (!map.ContainsKey(model.Api)) map.Add(model.Api, new());
                    map[model.Api].Add(model);
                }
            }

            return map;
        }

        internal static List<Model> GetModelsByAPI(Api api)
        {
            List<Model> models = new();

            foreach (Model model in DB.Values)
            {
                if (model == null) continue;
                if (model.Api == api) models.Add(model);
            }

            return models;
        }

        public static List<Model> GetJsonSchemaSupportedModels()
        {
            List<Model> models = new();

            foreach (Model model in DB.Values)
            {
                if (model == null) continue;
                if (model.OutputModality.HasFlag(ModelFeature.StructuredOutputs)) models.Add(model);
            }

            return models;
        }
    }
}