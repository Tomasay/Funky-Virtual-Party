using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal class VoicePopupGUI : AssetPopupGUI<Voice, VoiceFilter>
    {
        protected override Dictionary<Api, List<Voice>> GetFilteredAssets(VoiceFilter filter) => VoiceLibrary.GetFilteredRefs(filter);
        protected override Voice GetDefaultAssetId(VoiceFilter filter) => GetDefaultVoiceId(filter.Api);
        private static string GetDefaultVoiceId(Api api)
        {
            return api switch
            {
                Api.OpenAI => AIDevKitConfig.kDefault_OpenAI_Voice,
                Api.ElevenLabs => AIDevKitConfig.kDefault_ElevenLabs_Voice,
                _ => AIDevKitConfig.kDefault_OpenAI_Voice,
            };
        }

        protected override void DrawLibraryButton(Voice voice, GUIStyle style, float width)
        {
            if (GUILayout.Button(EditorIcons.PlayButton, EditorStyles.miniButtonMid, GUILayout.Width(width)))
            {
                voice.PlayPreviewAsync().Forget();
            }

            if (GUILayout.Button(EditorIcons.Plus, style, GUILayout.Width(width)))
            {
                VoiceCatalogueWindow.ShowWindow();
            }
        }
    }
}