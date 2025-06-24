using System.Collections.Generic;

namespace Glitch9.AIDevKit.GENTasks
{
    internal static class RequestType
    {
        internal const string Unknown = "unknown"; // used for unrecognized request types

        // Text 
        // includes legacy completions
        internal const string ChatCompletion = "chat_completion"; // chat completion
        internal const string ChatCompletionStream = "chat_completion_stream"; // chat completion (streaming)
        internal const string JsonSchema = "chat_completion_structured_output"; // custom object (JSONSchema)
        internal const string CodeGeneration = "chat_completion_code_generation"; // code generation

        // Image
        internal const string Image = "image_generation"; // image generation
        internal const string ImageInpaint = "image_generation_inpaint"; // image edit
        //internal const string ImageVariation = 12;

        // Audio
        internal const string Speech = "text_to_speech"; // text to speech
        internal const string SpeechStream = "text_to_speech_stream"; // text to speech (streaming)
        internal const string Transcript = "speech_to_text"; // speech to text
        internal const string TranscriptStream = "speech_to_text_stream"; // speech to text (streaming)
        internal const string TranscriptTranslation = "speech_to_text_translation"; // speech to text (translation)
        internal const string SoundEffect = "sound_effect_generation"; // sound effect generation
        internal const string VoiceChange = "voice_change"; // voice change
        internal const string AudioIsolation = "audio_isolation"; // audio isolation

        // Video
        internal const string Video = "video_generation"; // video generation

        // Moderation
        internal const string Moderation = "moderation"; // content moderation

        // Advanced
        internal const string Assistant = "assistants_api"; // advanced assistant
        internal const string Realtime = "realtime_api"; // real-time API (e.g., for streaming)

        // List
        internal const string ListModels = "list_models"; // List all models
        internal const string ListVoices = "list_voices"; // List all voices
        internal const string ListCustomModels = "list_models_custom"; // List custom models
        internal const string ListCustomVoices = "list_voices_custom"; // List custom voices
        internal const string RetrieveModel = "retrieve_model"; // Retrieve a specific model
        internal const string DeleteModel = "delete_model"; // Delete a specific model

        private readonly static Dictionary<string, string> _names = new()
        {
            { RequestType.Unknown, "Unknown" },
            { RequestType.ChatCompletion, "Chat Completion" },
            { RequestType.ChatCompletionStream, "Chat Completion (Streaming)" },
            { RequestType.CodeGeneration, "Chat Completion (Code Generation)" },
            { RequestType.Assistant, "Assistants API" },
            { RequestType.Image, "Image Generation" },
            { RequestType.ImageInpaint, "Image Edit" },
            //{ EndpointType.ImageVariation, "Image Variation" },
            { RequestType.Speech, "Text to Speech" },
            { RequestType.SpeechStream, "Text to Speech (Streaming)" },
            { RequestType.Transcript, "Speech to Text" },
            { RequestType.TranscriptStream, "Speech to Text (Streaming)" },
            { RequestType.TranscriptTranslation, "Speech to Text (Translation)" },
            { RequestType.JsonSchema, "Chat Completion (JSONSchema)" },
            { RequestType.SoundEffect, "Sound Effect" },
            { RequestType.VoiceChange, "Voice Change" },
            { RequestType.AudioIsolation, "Audio Isolation" },
            { RequestType.Video, "Video Generation" },
            { RequestType.Moderation, "Moderation" },
            { RequestType.ListModels, "List Models" },
            { RequestType.ListVoices, "List Voices" },
            { RequestType.ListCustomModels, "List Custom Models" },
            { RequestType.ListCustomVoices, "List Custom Voices" }
        };

        internal static string GetDisplayName(string requestType)
        {
            if (_names.TryGetValue(requestType, out var name)) return name;
            return "-";
        }

        internal static bool HasTextInput(string requestType)
        {
            return requestType == RequestType.ChatCompletion
            || requestType == RequestType.Image
            || requestType == RequestType.ImageInpaint
            || requestType == RequestType.Speech
            || requestType == RequestType.JsonSchema
            || requestType == RequestType.SoundEffect
            || requestType == RequestType.Video
            || requestType == RequestType.Moderation
            || requestType == RequestType.ListModels
            || requestType == RequestType.ListVoices
            || requestType == RequestType.ListCustomModels
            || requestType == RequestType.ListCustomVoices;
        }

        internal static bool HasTextOutput(string requestType)
        {
            return requestType == RequestType.ChatCompletion
            || requestType == RequestType.Transcript
            || requestType == RequestType.TranscriptTranslation
            || requestType == RequestType.JsonSchema;
        }

        internal static bool HasAudioInput(string requestType)
        {
            return requestType == RequestType.Transcript
            || requestType == RequestType.TranscriptTranslation
            || requestType == RequestType.VoiceChange
            || requestType == RequestType.AudioIsolation;
        }

        internal static bool HasAudioOutput(string requestType)
        {
            return requestType == RequestType.Speech
            || requestType == RequestType.SoundEffect
            || requestType == RequestType.VoiceChange
            || requestType == RequestType.AudioIsolation;
        }

        internal static bool HasImageInput(string requestType)
        {
            return requestType == RequestType.ImageInpaint;
            //|| requestType == EndpointType.ImageVariation;
        }

        internal static bool HasImageOutput(string requestType)
        {
            return requestType == RequestType.Image
            || requestType == RequestType.ImageInpaint;
            //|| requestType == EndpointType.ImageVariation;
        }

        internal static string ConvertLegacyValue(int legacyRequestType)
        {
            return legacyRequestType switch
            {
                0 => ChatCompletion,
                1 => JsonSchema,
                2 => CodeGeneration,
                10 => Image,
                12 => ImageInpaint,
                20 => Speech,
                21 => Transcript,
                22 => TranscriptTranslation,
                23 => SoundEffect,
                24 => VoiceChange,
                25 => AudioIsolation,
                30 => Video,
                40 => Moderation,
                50 => Assistant,
                60 => Realtime,
                100 => ListModels,
                101 => ListVoices,
                102 => ListCustomModels,
                103 => ListCustomVoices,
                104 => RetrieveModel,
                105 => DeleteModel,
                _ => Unknown
            };
        }
    }
}