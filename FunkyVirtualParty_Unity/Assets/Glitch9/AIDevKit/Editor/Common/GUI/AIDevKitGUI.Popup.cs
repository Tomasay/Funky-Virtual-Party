using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal partial class AIDevKitGUI
    {
        private static readonly ModelPopupGUI _modelPopupGUI = new();
        private static readonly VoicePopupGUI _voicePopupGUI = new();

        // -- Model Return  
        internal static Model LLMPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.LLM(api), label, style, apiWidth);

        internal static Model IMGPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.IMG(api), label, style, apiWidth);

        internal static Model TTSPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.TTS(api), label, style, apiWidth);

        internal static Model STTPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.STT(api), label, style, apiWidth);

        internal static Model EMBPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.EMB(api), label, style, apiWidth);

        internal static Model MODPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.MOD(api), label, style, apiWidth);

        internal static Model RTMPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.RTM(api), label, style, apiWidth);

        internal static Model VIDPopup(Model selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.VID(api), label, style, apiWidth);

        internal static Voice VoicePopup(Voice selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawVoicePopup(selected, VoiceFilter.API(api), label, style, apiWidth);


        // -- SerializedProperty  
        internal static void LLMPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.LLM(api), label, style, apiWidth);

        internal static void IMGPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.IMG(api), label, style, apiWidth);

        internal static void TTSPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.TTS(api), label, style, apiWidth);

        internal static void STTPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.STT(api), label, style, apiWidth);

        internal static void EMBPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.EMB(api), label, style, apiWidth);

        internal static void MODPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.MOD(api), label, style, apiWidth);

        internal static void RTMPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.RTM(api), label, style, apiWidth);

        internal static void VCMPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.VCM(api), label, style, apiWidth);

        internal static void VIDPopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(property, ModelFilter.VID(api), label, style, apiWidth);

        internal static void VoicePopup(SerializedProperty property, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawVoicePopup(property, VoiceFilter.API(api), label, style, apiWidth);


        // -- string ID 
        internal static string LLMPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.LLM(api), label, style, apiWidth);

        internal static string IMGPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.IMG(api), label, style, apiWidth);

        internal static string TTSPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.TTS(api), label, style, apiWidth);

        internal static string STTPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.STT(api), label, style, apiWidth);

        internal static string EMBPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.EMB(api), label, style, apiWidth);

        internal static string MODPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.MOD(api), label, style, apiWidth);

        internal static string RTMPopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawModelPopup(selected, ModelFilter.RTM(api), label, style, apiWidth);

        internal static string VoicePopup(string selected, Api api = Api.All, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        => Render.DrawVoicePopup(selected, VoiceFilter.API(api), label, style, apiWidth);
    }
}