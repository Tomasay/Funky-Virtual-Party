using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class EditorChatTools
    {
        private readonly EditorChatWindow _window;
        internal EditorChatTools(EditorChatWindow window) => _window = window;
        private bool _isExpanded = false;

        internal void Draw()
        {
            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.Space(8);
                _isExpanded = GUILayout.Toggle(_isExpanded, EditorIcons.Pick, ExStyles.miniButton, GUILayout.Width(20));

                if (_isExpanded)
                {
                    //if (GUILayout.Button("Review my scene", EditorChatConfig.ToolButton)) ReviewMyScene();
                }
            }
            finally
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        internal void ReviewMyScene()
        {
            string sceneJson = SceneReviewer.GenerateSceneReviewJson();
            if (string.IsNullOrEmpty(sceneJson)) throw new("Scene JSON is null or empty.");
            string systemMessage = $"User has requested for a feedback on their Unity scene.\n" +
                               $"Here is the details of the scene:\n{sceneJson}\n" +
                               $"Please review the scene and provide feedback.";
            _window.RequestWithSystemMessage(systemMessage);
            Debug.Log("Scene review request sent.");
            _isExpanded = false;
            _window.Repaint();
        }
    }
}