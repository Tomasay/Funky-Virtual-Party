using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    /// <summary>
    /// A session refers to a single WebSocket connection between a client and the server.
    /// <para>
    /// Once a client creates a session, it then sends JSON-formatted events containing text and audio chunks. 
    /// The server will respond in kind with audio containing voice output, a text transcript of that voice output, and function calls (if functions are provided by the client).
    /// </para><para>
    /// A realtime Session represents the overall client-server interaction, and contains default configuration.
    /// </para><para>
    /// It has a set of default values which can be updated at any time (via session.update) or on a per-response level (via response.create).
    /// </para>
    /// </summary>
    public class RealtimeSession
    {
        /// <summary>
        /// The unique ID of the session.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// The object type, must be "realtime.session".
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// The default model used for this session.
        /// </summary>
        [JsonProperty("model")] public Model Model { get; set; }

        /// <summary>
        /// The set of modalities the model can respond with.
        /// </summary>
        [JsonProperty("modalities")] public Modality[] Modalities { get; set; }

        /// <summary>
        /// The default system instructions.
        /// </summary>
        [JsonProperty("instructions")] public string Instructions { get; set; }

        /// <summary>
        /// The voice the model uses to respond.
        /// </summary>
        [JsonProperty("voice")] public Voice Voice { get; set; }

        /// <summary>
        /// The format of input audio.
        /// </summary>
        [JsonProperty("input_audio_format")] public RealtimeAudioFormat? InputAudioFormat { get; set; }

        /// <summary>
        /// The format of output audio.
        /// </summary>
        [JsonProperty("output_audio_format")] public RealtimeAudioFormat? OutputAudioFormat { get; set; }

        /// <summary>
        /// Configuration for input audio transcription.
        /// </summary>
        [JsonProperty("input_audio_transcription")] public SpeechToTextOptions InputAudioTranscription { get; set; }

        /// <summary>
        /// Configuration for turn detection.
        /// </summary>
        [JsonProperty("turn_detection")] public TurnDetection TurnDetection { get; set; }

        /// <summary>
        /// Tools (functions) available to the model.
        /// </summary>
        [JsonProperty("tools")] public FunctionDeclaration[] Tools { get; set; }

        /// <summary>
        /// How the model chooses tools.
        /// </summary>
        [JsonProperty("tool_choice")] public ToolChoice ToolChoice { get; set; }

        /// <summary>
        /// Sampling temperature.
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        /// <summary>
        /// Maximum number of output tokens.
        /// </summary>
        [JsonProperty("max_output_tokens")] public int? MaxOutputTokens { get; set; }

        public override string ToString()
        {
            return $"Session {Id}";
        }
    }

    /// <summary>
    /// Configuration for input audio transcription. Can be set to null to turn off.
    /// </summary>
    public class SpeechToTextOptions
    {
        /// <summary>
        /// Don't set this from the client side. This is a server-side field.
        /// </summary>
        [JsonProperty("enabled")] public bool? Enabled { get; set; }
        [JsonProperty("model")] public Model Model { get; set; } = AIDevKitConfig.kDefault_OpenAI_STT;
        [JsonProperty("language")] public SystemLanguage? Language { get; set; }
    }
}
