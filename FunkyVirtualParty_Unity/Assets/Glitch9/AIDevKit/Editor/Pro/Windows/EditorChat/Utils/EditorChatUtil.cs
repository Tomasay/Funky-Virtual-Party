using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static partial class EditorChatUtil
    {
        internal static async UniTask<bool> CheckRequirementsAsync(ILogger logger)
        {
            try
            {
                bool hasAnyApiKey = OpenAISettings.Instance.HasApiKey()
                              || GenerativeAISettings.Instance.HasApiKey()
                              || OpenRouterSettings.Instance.HasApiKey();

                if (hasAnyApiKey) return true;

                bool hasOwnService = await OllamaSettings.CheckConnectionAsync();
                if (hasOwnService) return true;
            }
            catch (System.Exception ex)
            {
                logger?.Error($"Failed to check requirements: {ex.Message}");
                return false;
            }

            logger?.Warning("No API keys found. Please set up an API key in the settings.");
            return false;
        }

        internal static bool CheckNull<T>(string name, T controller, ILogger logger) where T : class
        {
            //if (EditorChatSettings.DebugMode) AIDevKitDebug.Green($"Rebuilding {name}...");
            if (controller == null)
            {
                logger?.Error($"{name} is not initialized.");
                return false;
            }
            return true;
        }

        internal static void FinalizeResponse(ChatMessage message, ChatScrollViewController controller, ILogger logger)
        {
            if (message is not ResponseMessage responseMessage)
            {
                logger?.Error("Message is not a ResponseMessage. Cannot finalize response.");
                return;
            }

            if (responseMessage.Usage == null)
            {
                logger?.Warning("ResponseMessage has no usage data. This may indicate an issue with the request.");
            }
            else if (EditorChatSettings.DebugMode)
            {
                AIDevKitDebug.Green($"Response usage: {responseMessage.Usage}");
            }

            controller.AddOrReplaceResponseMessage(responseMessage);
        }

        internal static Texture ResolveFileIcon(IFile file)
        {
            var path = file.FullPath.ToRelativePath();

            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj != null)
            {
                var content = EditorGUIUtility.ObjectContent(obj, obj.GetType());
                return content.image;
            }

            return AIDevKitIcons.File; // fallback
        }
    }
}