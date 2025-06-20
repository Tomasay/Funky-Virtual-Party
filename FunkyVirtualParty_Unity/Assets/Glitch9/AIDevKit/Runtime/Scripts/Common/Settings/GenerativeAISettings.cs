using UnityEngine;
using Glitch9.AIDevKit.Client;

namespace Glitch9.AIDevKit.Google
{
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class GenerativeAISettings : AIClientSettings<GenerativeAISettings>
    {
        [SerializeField] private string projectId;

        // default models
        [SerializeField] private string defaultLLM = AIDevKitConfig.kDefault_Google_LLM;
        [SerializeField] private string defaultEMB = AIDevKitConfig.kDefault_Google_EMB;
        [SerializeField] private string defaultIMG = AIDevKitConfig.kDefault_Google_IMG;
        [SerializeField] private string defaultVID = AIDevKitConfig.kDefault_Google_VID;
        [SerializeField] private string defaultTTS = AIDevKitConfig.kDefault_Google_TTS;
        [SerializeField] private string defaultVoice = AIDevKitConfig.kDefault_Google_Voice;

        public static string ProjectId => Instance.projectId;

        public static string DefaultLLM => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultLLM, AIDevKitConfig.kDefault_OpenAI_LLM);
        public static string DefaultEMB => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultEMB, AIDevKitConfig.kDefault_OpenAI_EMB);
        public static string DefaultIMG => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultIMG, AIDevKitConfig.kDefault_OpenAI_IMG);
        public static string DefaultVID => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultVID, AIDevKitConfig.kDefault_Google_VID);
        public static string DefaultTTS => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultTTS, AIDevKitConfig.kDefault_Google_TTS);
        public static string DefaultVoice => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultVoice, AIDevKitConfig.kDefault_Google_Voice);

        public static bool IsDefaultModel(string id, ModelFeature cap)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (cap.HasFlag(ModelFeature.TextGeneration) && id == DefaultLLM) return true;
            if (cap.HasFlag(ModelFeature.ImageGeneration) && id == DefaultIMG) return true;
            if (cap.HasFlag(ModelFeature.TextEmbedding) && id == DefaultEMB) return true;
            if (cap.HasFlag(ModelFeature.VideoGeneration) && id == DefaultVID) return true;
            if (cap.HasFlag(ModelFeature.SpeechGeneration) && id == DefaultTTS) return true;

            return false;
        }

    }
}