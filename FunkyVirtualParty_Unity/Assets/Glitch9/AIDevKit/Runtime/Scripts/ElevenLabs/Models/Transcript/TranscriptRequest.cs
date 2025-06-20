using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class TranscriptRequest : RequestBody
    {
        /// <summary>
        /// Required. 
        /// The ID of the model to use for transcription, 
        /// currently only ‘scribe_v1’ and ‘scribe_v1_experimental’ are available.
        /// </summary>
        [JsonProperty("model_id")] public Model Model { get; set; }

        /// <summary>
        /// Optional. 
        /// The file to transcribe. 
        /// All major audio and video formats are supported. 
        /// Exactly one of the file or cloud_storage_url parameters must be provided. 
        /// The file size must be less than 1GB.
        /// </summary>
        [JsonProperty("file")] public byte[] File { get; set; }

        /// <summary>
        /// Optional. 
        /// The valid AWS S3 or Google Cloud Storage URL of the file to transcribe. 
        /// Exactly one of the file or cloud_storage_url parameters must be provided. 
        /// The file must be a valid publicly accessible cloud storage URL. 
        /// The file size must be less than 2GB. URL can be pre-signed.
        /// </summary>
        [JsonProperty("cloud_storage_url")] public string CloudStorageUrl { get; set; }

        /// <summary>
        /// Optional. 
        /// An ISO-639-1 or ISO-639-3 language_code corresponding to the language of the audio file. 
        /// Can sometimes improve transcription performance if known beforehand. 
        /// Defaults to null, in this case the language is predicted automatically.
        /// </summary>
        [JsonProperty("language_code")] public string LanguageCode { get; set; }

        /// <summary>
        /// Optional. 
        /// Whether to tag audio events like (laughter), (footsteps), etc. in the transcription. 
        /// Defaults to true.
        /// </summary>
        [JsonProperty("tag_audio_events")] public bool? TagAudioEvents { get; set; }

        /// <summary>
        /// Optional. 
        /// The maximum amount of speakers talking in the uploaded file. 
        /// Can help with predicting who speaks when. 
        /// The maximum amount of speakers that can be predicted is 32. 
        /// Defaults to null, in this case the amount of speakers is set to the maximum value the model supports.
        /// </summary>
        [JsonProperty("num_speakers")] public int? NumSpeakers { get; set; }

        /// <summary>
        /// Optional. 
        /// The granularity of the timestamps in the transcription. 
        /// ‘word’ provides word-level timestamps and ‘character’ provides character-level timestamps per word. 
        /// Allowed values: none, word, character. Defaults to 'word'.
        /// </summary>
        [JsonProperty("timestamps_granularity")] public TimestampsGranularity? TimestampsGranularity { get; set; }

        /// <summary>
        /// Optional. 
        /// Whether to annotate which speaker is currently talking in the uploaded file. 
        /// Defaults to false.
        /// </summary>
        [JsonProperty("diarize")] public bool? Diarize { get; set; }

        /// <summary>
        /// Optional. 
        /// A list of additional formats to export the transcript to.
        /// </summary>
        [JsonProperty("additional_formats")] public List<ElevenLabsFormat> AdditionalFormats { get; set; }

        /// <summary>
        /// Optional. 
        /// The format of input audio. 
        /// Options are ‘pcm_s16le_16’ or ‘other’. For `pcm_s16le_16`, 
        /// the input audio must be 16-bit PCM at a 16kHz sample rate, single channel (mono), 
        /// and little-endian byte order. 
        /// atency will be lower than with passing an encoded waveform. 
        /// Defaults to 'other'.
        /// </summary>
        [JsonProperty("file_format")] public ElevenLabsInputFormat InputFormat { get; set; }

    }
}