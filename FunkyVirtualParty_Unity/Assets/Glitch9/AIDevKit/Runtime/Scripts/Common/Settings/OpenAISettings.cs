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

        public static string DefaultLLM => Instance.defaultLLM.OrDefault(AIDevKitConfig.kDefault_OpenAI_LLM);
        public static string DefaultIMG => Instance.defaultIMG.OrDefault(AIDevKitConfig.kDefault_OpenAI_IMG);
        public static string DefaultTTS => Instance.defaultTTS.OrDefault(AIDevKitConfig.kDefault_OpenAI_TTS);
        public static string DefaultSTT => Instance.defaultSTT.OrDefault(AIDevKitConfig.kDefault_OpenAI_STT);
        public static string DefaultEMB => Instance.defaultEMB.OrDefault(AIDevKitConfig.kDefault_OpenAI_EMB);
        public static string DefaultMOD => Instance.defaultMOD.OrDefault(AIDevKitConfig.kDefault_OpenAI_MOD);
        public static string DefaultRTM => Instance.defaultRTM.OrDefault(AIDevKitConfig.kDefault_OpenAI_RTM);
        public static string DefaultVoice => Instance.defaultVoice.OrDefault(AIDevKitConfig.kDefault_OpenAI_Voice);

        public static string GetAdminApiKey() => Instance.adminKey?.GetKey();

        public static bool IsDefaultModel(string id, ModelFeature feature)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (feature.HasFlag(ModelFeature.SpeechGeneration) && id == DefaultTTS) return true;
            if (feature.HasFlag(ModelFeature.VoiceChanger) && id == DefaultVoice) return true;
            if (feature.HasFlag(ModelFeature.TextGeneration) && id == DefaultLLM) return true;
            if (feature.HasFlag(ModelFeature.ImageGeneration) && id == DefaultIMG) return true;
            if (feature.HasFlag(ModelFeature.TextEmbedding) && id == DefaultEMB) return true;
            if (feature.HasFlag(ModelFeature.Moderation) && id == DefaultMOD) return true;
            if (feature.HasFlag(ModelFeature.Realtime) && id == DefaultRTM) return true;
            if (feature.HasFlag(ModelFeature.SpeechRecognition) && id == DefaultSTT) return true;

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