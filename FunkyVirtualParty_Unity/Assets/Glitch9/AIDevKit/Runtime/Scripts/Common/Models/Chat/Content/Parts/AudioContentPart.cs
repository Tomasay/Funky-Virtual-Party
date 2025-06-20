using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public class AudioBase64ContentPart : AudioContentPart { }

    public class AudioContentPart : ContentPart
    {
        [JsonProperty("input_audio")] public AudioRef InputAudio { get; set; }

        public static AudioBase64ContentPart FromBase64(string base64Audio, MIMEType mimeType = MIMEType.WAV)
        {
            return new AudioBase64ContentPart()
            {
                Type = ContentPartType.Audio,
                InputAudio = new AudioRef
                {
                    Data = base64Audio,
                    Format = mimeType.GetExtension(),
                }
            };
        }
    }


    public class AudioRef
    {
        /// <summary>
        /// Optional. The base64 encoded audio data, used when passing the audio to the model as a string.
        /// </summary>
        [JsonProperty("data")] public string Data { get; set; }

        /// <summary>
        /// Optional. The format of the encoded audio data. Currently supports "wav" and "mp3".
        /// </summary>
        [JsonProperty("format")] public string Format { get; set; } = "wav";

        [JsonIgnore]
        public MIMEType MimeType
        {
            get
            {
                return Format switch
                {
                    "wav" => MIMEType.WAV,
                    "mp3" => MIMEType.MPEG,
                    _ => MIMEType.Unknown
                };
            }
        }
    }
}