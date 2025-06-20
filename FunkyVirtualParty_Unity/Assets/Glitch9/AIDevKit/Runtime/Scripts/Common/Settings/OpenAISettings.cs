using UnityEngine;
using Glitch9.AIDevKit.Client;

namespace Glitch9.AIDevKit.OpenAI
{
    [AssetPath(AIDevKitConfig.CreatePath)]
    public partial class OpenAISettings : AIClientSettings<OpenAISettings>
    {
        // general settings
        [SerializeField] private string organization;
        [SerializeField] private string projectId;
        [SerializeField] private ApiKey adminKey;

        // default models
        [SerializeField] private string defaultLLM = AIDevKitConfig.kDefault_OpenAI_LLM;
        [SerializeField] private string defaultIMG = AIDevKitConfig.kDefault_OpenAI_IMG;
        [SerializeField] private string defaultTTS = AIDevKitConfig.kDefault_OpenAI_TTS;
        [SerializeField] private string defaultSTT = AIDevKitConfig.kDefault_OpenAI_STT;
        [SerializeField] private string defaultEMB = AIDevKitConfig.kDefault_OpenAI_EMB;
        [SerializeField] private string defaultMOD = AIDevKitConfig.kDefault_OpenAI_MOD;
        [SerializeField] private string defaultASS = AIDevKitConfig.kDefault_OpenAI_ASS;
        [SerializeField] private string defaultRTM = AIDevKitConfig.kDefault_OpenAI_RTM;

        // default voice
        [SerializeField] private string defaultVoice = AIDevKitConfig.kDefault_OpenAI_Voice;

        /// <summary>
        /// Optional. Specifies the organization under which API calls are made, used for organizational billing and access management.
        /// </summary>
        public static string Organization => Instance.organization;

        /// <summary>
        /// Optional. Project IDs can be found on your general settings page by selecting the specific project.
        /// </summary>
        public static string ProjectId => Instance.projectId;

        /// <summary>
        /// Default model used for the Assistant API, setting a standard for automated interactions.
        /// </summary>
        public static string DefaultAssistantAPIModel => Instance.defaultASS;

        public static string DefaultLLM => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultLLM, AIDevKitConfig.kDefault_OpenAI_LLM);
        public static string DefaultIMG => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultIMG, AIDevKitConfig.kDefault_OpenAI_IMG);
        public static string DefaultTTS => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultTTS, AIDevKitConfig.kDefault_OpenAI_TTS);
        public static string DefaultSTT => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultSTT, AIDevKitConfig.kDefault_OpenAI_STT);
        public static string DefaultEMB => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultEMB, AIDevKitConfig.kDefault_OpenAI_EMB);
        public static string DefaultMOD => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultMOD, AIDevKitConfig.kDefault_OpenAI_MOD);
        public static string DefaultRTM => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultRTM, AIDevKitConfig.kDefault_OpenAI_RTM);
        public static string DefaultVoice => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultVoice, AIDevKitConfig.kDefault_OpenAI_Voice);
        public static string GetAdminApiKey() => Instance.adminKey?.GetKey();

        public static bool IsDefaultModel(string id, ModelFeature cap)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (cap.HasFlag(ModelFeature.SpeechGeneration) && id == DefaultTTS) return true;
            if (cap.HasFlag(ModelFeature.VoiceChanger) && id == DefaultVoice) return true;
            if (cap.HasFlag(ModelFeature.TextGeneration) && id == DefaultLLM) return true;
            if (cap.HasFlag(ModelFeature.ImageGeneration) && id == DefaultIMG) return true;
            if (cap.HasFlag(ModelFeature.TextEmbedding) && id == DefaultEMB) return true;
            if (cap.HasFlag(ModelFeature.Moderation) && id == DefaultMOD) return true;
            if (cap.HasFlag(ModelFeature.Realtime) && id == DefaultRTM) return true;
            if (cap.HasFlag(ModelFeature.SpeechRecognition) && id == DefaultSTT) return true;

            return false;
        }

        public static bool IsDefaultVoice(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (id == DefaultVoice) return true;

            return false;
        }
    }
}