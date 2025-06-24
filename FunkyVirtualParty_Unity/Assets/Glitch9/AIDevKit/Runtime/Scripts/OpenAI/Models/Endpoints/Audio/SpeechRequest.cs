using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public enum AudioStreamFormat
    {
        [ApiEnum("Server-Sent Events (SSE)", "sse")] SSE,
        [ApiEnum("Audio Data", "audio")] Audio
    }

    /// <summary>
    /// Generates audio from the input text.
    /// </summary>
    public class SpeechRequest : AIRequest
    {
        /// <summary>
        /// Required. The input text to generate audio for.
        /// The maximum length is 4096 characters.
        /// </summary>
        [JsonProperty("input")] public string Prompt { get; set; }

        /// <summary>
        /// Required. 
        /// The voice to use when generating the audio.
        /// Supported voices are alloy, echo, fable, onyx, nova, and shimmer.
        /// Previews of the voices are available in the Text to speech guide.
        /// </summary>
        [JsonProperty("voice")] public Voice Voice { get; set; }

        /// <summary>
        /// The speed of the generated audio.
        /// Select a value from 0.25 to 4.0. 
        /// </summary>
        /// <remarks>
        /// 1.0 is the default.
        /// </remarks>
        [JsonProperty("speed")] public float? Speed { get; set; }

        /// <summary>
        /// The format of the response. 
        /// </summary>
        [JsonProperty("response_format")] public AudioEncoding? ResponseFormat { get; set; }

        /// <summary>
        /// Optional. 
        /// Defaults to audio.
        /// Set this value to stream the audio response instead of returning it as a single response.
        /// The format to stream the audio in. 
        /// Supported formats are sse and audio. 
        /// sse is not supported for tts-1 or tts-1-hd.
        /// </summary>
        [JsonProperty("stream_format")] public AudioStreamFormat? StreamFormat { get; set; }

        public override void ValidateRequestBody()
        {
            base.ValidateRequestBody();

            if (Model == null)
            {
                throw new System.Exception("Model property is required for SpeechRequest.");
            }
            else
            {
                if (StreamFormat.HasValue && StreamFormat.Value == AudioStreamFormat.SSE)
                {
                    string modelId = Model.Id;

                    if (modelId == "tts-1" && modelId == "tts-1-hd")
                    {
                        throw new System.Exception("SSE streaming is not supported for tts-1 or tts-1-hd models. " +
                        "Please use audio streaming format instead or choose a different TTS model.");
                    }
                }
            }
        }

        public class Builder : ModelRequestBuilder<Builder, SpeechRequest>
        {
            public Builder SetPrompt(string promptText)
            {
                _req.Prompt = promptText;
                return this;
            }

            public Builder SetVoice(Voice voice)
            {
                _req.Voice = voice;
                return this;
            }

            public Builder SetVoice(string voice)
            {
                _req.Voice = voice;
                return this;
            }

            public Builder SetResponseFormat(AudioEncoding responseFormat)
            {
                _req.ResponseFormat = responseFormat;
                return this;
            }

            public Builder SetResponseFormat(MIMEType mimeType)
            {
                _req.ResponseFormat = mimeType.ToAudioEncoding(AudioEncoding.MP3);
                return this;
            }

            public Builder SetSpeed(float speed)
            {
                if (speed < 0.25f || speed > 4.0f)
                {
                    throw new System.Exception("Speed must be between 0.25 and 4.0");
                }

                _req.Speed = speed;
                return this;
            }

            public Builder SetStreamFormat(AudioStreamFormat streamFormat)
            {
                _req.StreamFormat = streamFormat;
                return this;
            }
        }
    }
}