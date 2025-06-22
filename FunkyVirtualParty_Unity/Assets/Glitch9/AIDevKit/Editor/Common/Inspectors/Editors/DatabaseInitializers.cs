using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [InitializeOnLoad]
    internal static class ModelLibraryEditorInitializer
    {
        static ModelLibraryEditorInitializer()
        {
            EditorApplication.delayCall += () =>
            {
                var db = ModelLibrary.DB;
                if (db == null || db.Count > 0) return;

                if (!ScriptableObjectUtil.InitialLoad(db))
                {
                    Debug.LogError("There is no model in your library. Please add models to the library.");
                }
            };
        }
    }

    [InitializeOnLoad]
    internal static class VoiceLibraryEditorInitializer
    {
        static VoiceLibraryEditorInitializer()
        {
            EditorApplication.delayCall += () =>
            {
                var db = VoiceLibrary.DB;
                if (db == null || db.Count > 0) return;

                if (!ScriptableObjectUtil.InitialLoad(db))
                {
                    Debug.LogError("There is no voice in your library. Please add voices to the library.");
                }
            };
        }
    }
}