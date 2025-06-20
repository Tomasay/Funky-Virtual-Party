
namespace Glitch9.AIDevKit.ElevenLabs
{
    public static class FluentAPIExtensions
    {
        private const string kOutputFormatKey = "elevenlabs.output_format";

        public static GENSpeechTask SetOutputFormat(this GENSpeechTask task, ElevenLabsOutputFormat outputFormat)
        {
            task.SetOption(kOutputFormatKey, outputFormat);
            return task;
        }

        internal static ElevenLabsOutputFormat? GetOutputFormat(this GENSpeechTask task)
        {
            if (task.TryGetOption(kOutputFormatKey, out ElevenLabsOutputFormat outputFormat)) return outputFormat;
            return null;
        }

        public static GENSoundEffectTask SetOutputFormat(this GENSoundEffectTask task, ElevenLabsOutputFormat outputFormat)
        {
            task.SetOption(kOutputFormatKey, outputFormat);
            return task;
        }

        internal static ElevenLabsOutputFormat? GetOutputFormat(this GENSoundEffectTask task)
        {
            if (task.TryGetOption(kOutputFormatKey, out ElevenLabsOutputFormat outputFormat)) return outputFormat;
            return null;
        }

        public static GENVoiceChangeTask SetOutputFormat(this GENVoiceChangeTask task, ElevenLabsOutputFormat outputFormat)
        {
            task.SetOption(kOutputFormatKey, outputFormat);
            return task;
        }

        internal static ElevenLabsOutputFormat? GetOutputFormat(this GENVoiceChangeTask task)
        {
            if (task.TryGetOption(kOutputFormatKey, out ElevenLabsOutputFormat outputFormat)) return outputFormat;
            return null;
        }

        public static GENAudioIsolationTask SetOutputFormat(this GENAudioIsolationTask task, ElevenLabsOutputFormat outputFormat)
        {
            task.SetOption(kOutputFormatKey, outputFormat);
            return task;
        }

        internal static ElevenLabsOutputFormat? GetOutputFormat(this GENAudioIsolationTask task)
        {
            if (task.TryGetOption(kOutputFormatKey, out ElevenLabsOutputFormat outputFormat)) return outputFormat;
            return null;
        }
    }
}