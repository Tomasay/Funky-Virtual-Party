using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Audio Service: https://platform.openai.com/docs/api-reference/audio
    /// </summary>
    public class AudioService : CRUDServiceBase<OpenAI>
    {
        public SpeechService Speech { get; }
        public TranscriptionService Transcriptions { get; }
        public TranslationService Translations { get; }

        public AudioService(OpenAI client) : base(client)
        {
            Speech = new SpeechService(client);
            Transcriptions = new TranscriptionService(client);
            Translations = new TranslationService(client);
        }
    }

    public class SpeechService : CRUDServiceBase<OpenAI>
    {
        public const string kEndpoint = "{ver}/audio/speech";
        public SpeechService(OpenAI client) : base(client) { }

        /// <summary>
        /// Generates audio from the input text.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async UniTask<GeneratedAudio> CreateAsync(SpeechRequest req)
        {
            if (req.Model == null) req.Model = OpenAISettings.DefaultTTS;
            if (req.Voice == null) req.Voice = OpenAISettings.DefaultVoice;

            if (req.ResponseFormat != null)
            {
                AudioEncoding encoding = req.ResponseFormat.Value;

                if (encoding == AudioEncoding.PCM)
                {
                    req.options.OutputAudioFormat = new AudioFormat()
                    {
                        Encoding = AudioEncoding.PCM,
                        SampleRate = SampleRate.Hz24000,
                        Bitrate = Bitrate.Kbps128,
                        BitDepth = BitDepth.Bit16
                    };
                }
                else if (encoding != AudioEncoding.MP3)
                {
                    client.Logger.Warning($"OpenAI or Unity does not support {encoding} audio format. Defaulting to MP3.");
                    req.ResponseFormat = null;
                }
            }

            RESTResponse res = await client.POSTCreateAsync(kEndpoint, this, req);
            if (res == null) return null;
            return new(res.AudioOutput, res.OutputPath);
        }
    }

    public class TranscriptionService : CRUDServiceBase<OpenAI>
    {
        public const string kEndpoint = "{ver}/audio/transcriptions";

        public TranscriptionService(OpenAI client) : base(client) { }

        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async UniTask<OpenAITranscript> CreateAsync(TranscriptionRequest req)
        {
            if (req.Model == null) req.Model = OpenAISettings.DefaultSTT;
            req.MIMEType = MIMEType.MultipartForm;
            return await client.POSTCreateAsync<TranscriptionRequest, OpenAITranscript>(kEndpoint, this, req);
        }
    }

    public class TranslationService : CRUDServiceBase<OpenAI>
    {
        public const string kEndpoint = "{ver}/audio/translations";

        public TranslationService(OpenAI client) : base(client) { }

        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async UniTask<string> CreateAsync(TranslationRequest req)
        {
            if (req.Model == null) req.Model = OpenAISettings.DefaultSTT;
            req.MIMEType = MIMEType.MultipartForm;
            TextModel textModel = await client.POSTCreateAsync<TranslationRequest, TextModel>(kEndpoint, this, req);
            return textModel?.Text;
        }
    }
}