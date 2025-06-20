using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class Texts
    {
        internal const string kReloadEntries = "Reload Entries";
        internal const string kRemoveInvalidEntries = "Remove Invalid Entries";
    }

    [UnityEditor.CustomEditor(typeof(FileLibrary))]
    public class FileDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Texts.kRemoveInvalidEntries))
            {
                FileLibrary.RemoveInvalidEntries();
            }
        }
    }

    [UnityEditor.CustomEditor(typeof(PromptHistory))]
    public class PromptRecordDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Texts.kRemoveInvalidEntries))
            {
                PromptHistory.RemoveInvalidEntries();
            }
        }
    }

    [UnityEditor.CustomEditor(typeof(ModelLibrary))]
    public class ModelDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Texts.kReloadEntries, ExStyles.bigButton))
            {
                ModelLibrary.FindAssets();
            }

            if (GUILayout.Button(Texts.kRemoveInvalidEntries, ExStyles.bigButton))
            {
                ModelLibrary.RemoveInvalidEntries();
            }
        }
    }

    [UnityEditor.CustomEditor(typeof(VoiceLibrary))]
    public class VoiceDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Texts.kReloadEntries, ExStyles.bigButton))
            {
                VoiceLibrary.FindAssets();
            }

            if (GUILayout.Button(Texts.kRemoveInvalidEntries, ExStyles.bigButton))
            {
                VoiceLibrary.RemoveInvalidEntries();
            }
        }
    }
}