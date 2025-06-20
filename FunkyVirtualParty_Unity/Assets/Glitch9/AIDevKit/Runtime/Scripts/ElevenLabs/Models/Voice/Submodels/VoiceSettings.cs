using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class VoiceSettings
    {
        /// <summary>
        /// Optional. Determines how stable the voice is and the randomness between each generation.
        /// Lower values introduce broader emotional range for the voice.
        /// Higher values can result in a monotonous voice with limited emotion.
        /// </summary>
        [JsonProperty("stability")] public double? Stability { get; set; }

        /// <summary>
        /// Optional. Determines how closely the AI should adhere to the original voice when attempting to replicate it.
        /// </summary>
        [JsonProperty("similarity_boost")] public double? SimilarityBoost { get; set; }

        /// <summary>
        /// Optional. Determines the style exaggeration of the voice.
        /// This setting attempts to amplify the style of the original speaker.
        /// This setting consumes additional computational resources and might increase latency if set to anything other than 0.
        /// </summary>
        [JsonProperty("style")] public double? Style { get; set; }

        /// <summary>
        /// Optional. This setting boosts the similarity to the original speaker.
        /// Using this setting requires a slightly higher computational load, which in turn increases latency.
        /// </summary>
        [JsonProperty("use_speaker_boost")] public bool? UseSpeakerBoost { get; set; }

        /// <summary>
        /// Optional. Controls the speed of the generated speech.
        /// Values range from 0.7 to 1.2, with 1.0 being the default speed.
        /// Lower values create slower, more deliberate speech while higher values produce faster-paced speech.
        /// Extreme values can impact the quality of the generated speech.
        /// </summary>
        [JsonProperty("speed")] public double? Speed { get; set; }

        /// <summary>
        /// Optional. The use of style exaggeration in the voice output.
        /// </summary>
        [JsonProperty("use_style_exaggeration")] public bool? UseStyleExaggeration { get; set; }
    }
}