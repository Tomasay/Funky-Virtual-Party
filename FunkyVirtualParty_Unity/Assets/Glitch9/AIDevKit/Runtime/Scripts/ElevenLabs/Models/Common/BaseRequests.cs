using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class AudioRequest : RequestBody
    {
        // Query Parameters --------------------------------------------------------------- 

        /// <summary>
        /// Optional. 
        /// Output format of the generated audio. 
        /// Formatted as codec_sample_rate_bitrate.
        /// So an mp3 with 22.05kHz sample rate at 32kbs is represented as mp3_22050_32. 
        /// MP3 with 192kbps bitrate requires you to be subscribed to Creator tier or above. 
        /// PCM with 44.1kHz sample rate requires you to be subscribed to Pro tier or above. 
        /// Note that the μ-law format (sometimes written mu-law, often approximated as u-law) is commonly used for Twilio audio inputs.
        /// </summary>
        [QueryParameter("output_format")]
        public ElevenLabsOutputFormat? OutputFormat
        {
            get => _outputFormat;
            set
            {
                _outputFormat = value;
                if (value != null) options.OutputAudioFormat = value.Value.ToAudioFormat();
            }
        }

        [JsonIgnore] ElevenLabsOutputFormat? _outputFormat;
    }


    public class VoiceRequest : AudioRequest
    {
        // Path Parameters ----------------------------------------------------------------

        /// <summary>
        /// Required.
        /// ID of the voice to be used. 
        /// Use the Get voices endpoint list all the available voices.
        /// </summary>
        [PathParameter("0")] public string VoiceId { get; set; }
        [JsonIgnore]
        public Voice Voice
        {
            get => _voice;
            set
            {
                _voice = value;
                if (value != null) VoiceId = value.Id;
            }
        }
        [JsonIgnore] private Voice _voice;

        // Query Parameters ---------------------------------------------------------------

        /// <summary>
        /// Optional.
        /// When enable_logging is set to false zero retention mode will be used for the request. 
        /// This will mean history features are unavailable for this request, including request stitching. 
        /// Zero retention mode may only be used by enterprise customers.
        /// </summary>
        [QueryParameter("enable_logging")] public bool? EnableLogging { get; set; }

        // Body Parameters ---------------------------------------------------------------

        /// <summary>
        /// Optional. 
        /// SpeechRequest:
        /// Identifier of the model that will be used, you can query them using GET /v1/models. 
        /// The model needs to have support for text to speech, 
        /// you can check this using the can_do_text_to_speech property.
        /// Defaults to 'eleven_monolingual_v1'.
        /// VoiceChangerRequest:
        /// Identifier of the model that will be used, you can query them using GET /v1/models.
        /// The model needs to have support for speech to speech, you can check this using the can_do_voice_conversion property.
        /// Defaults to eleven_english_sts_v2
        /// </summary>
        [JsonProperty("model_id")] public Model Model { get; set; }
    }

}