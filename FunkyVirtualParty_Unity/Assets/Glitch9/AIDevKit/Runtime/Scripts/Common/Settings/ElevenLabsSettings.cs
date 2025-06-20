using UnityEngine;
using Glitch9.AIDevKit.Client;

namespace Glitch9.AIDevKit.ElevenLabs
{
    [AssetPath(AIDevKitConfig.CreatePath)]
    public partial class ElevenLabsSettings : AIClientSettings<ElevenLabsSettings>
    {
        // default models
        [SerializeField] private string defaultTTS = AIDevKitConfig.kDefault_ElevenLabs_TTS;
        [SerializeField] private string defaultVCM = AIDevKitConfig.kDefault_ElevenLabs_VCM;
        [SerializeField] private string defaultSTT = AIDevKitConfig.kDefault_ElevenLabs_STT;

        // default voice
        [SerializeField] private string defaultVoice = AIDevKitConfig.kDefault_ElevenLabs_Voice;

        public static string DefaultTTS => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultTTS, AIDevKitConfig.kDefault_ElevenLabs_TTS);
        public static string DefaultVCM => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultVCM, AIDevKitConfig.kDefault_ElevenLabs_VCM);
        public static string DefaultSTT => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultSTT, AIDevKitConfig.kDefault_ElevenLabs_STT);
        public static string DefaultVoice => ModelUtil.ReturnDefaultIfEmpty(Instance.defaultVoice, AIDevKitConfig.kDefault_ElevenLabs_Voice);

        public static bool IsDefaultModel(string id, ModelFeature cap)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (cap.HasFlag(ModelFeature.SpeechGeneration) && id == DefaultTTS) return true;
            if (cap.HasFlag(ModelFeature.VoiceChanger) && id == DefaultVCM) return true;

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