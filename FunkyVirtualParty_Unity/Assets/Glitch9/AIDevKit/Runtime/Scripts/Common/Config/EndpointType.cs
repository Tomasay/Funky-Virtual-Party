using System.Collections.Generic;

namespace Glitch9.AIDevKit.GENTasks
{
    internal static class EndpointType
    {
        internal const int Unknown = -1;

        // Text 
        internal const int ChatCompletion = 0;
        internal const int JsonSchema = 1; // structured outputs
        internal const int CodeGeneration = 2;

        // Image
        internal const int ImageCreation = 10;
        internal const int ImageEdit = 11;
        //internal const int ImageVariation = 12;

        // Audio
        internal const int Speech = 20;
        internal const int Transcript = 21;
        internal const int Translation = 22;
        internal const int SoundEffect = 23;
        internal const int VoiceChange = 24;
        internal const int AudioIsolation = 25;

        // Video
        internal const int Video = 30;

        // Moderation
        internal const int Moderation = 40;

        // Advanced
        internal const int Assistant = 50; // Assistants API
        internal const int Realtime = 51; // Realtime API

        // List
        internal const int ListModels = 100;
        internal const int ListVoices = 101;
        internal const int ListCustomModels = 102;
        internal const int ListCustomVoices = 103;
        internal const int RetrieveModel = 104; // Retrieve a specific model
        internal const int DeleteModel = 105; // Delete a custom model

        private readonly static Dictionary<int, string> _names = new()
        {
            { EndpointType.Unknown, "Unknown" },
            { EndpointType.ChatCompletion, "Chat Completion" },
            { EndpointType.ImageCreation, "Image Generation" },
            { EndpointType.ImageEdit, "Image Edit" },
            //{ EndpointType.ImageVariation, "Image Variation" },
            { EndpointType.Speech, "Text to Speech" },
            { EndpointType.Transcript, "Speech to Text" },
            { EndpointType.Translation, "Speech to Text (Translation)" },
            { EndpointType.JsonSchema, "Custom Object (JSONSchema)" },
            { EndpointType.SoundEffect, "Sound Effect" },
            { EndpointType.VoiceChange, "Voice Change" },
            { EndpointType.AudioIsolation, "Audio Isolation" },
            { EndpointType.Video, "Video Generation" },
            { EndpointType.Moderation, "Moderation" },
            { EndpointType.ListModels, "List Models" },
            { EndpointType.ListVoices, "List Voices" },
            { EndpointType.ListCustomModels, "List Custom Models" },
            { EndpointType.ListCustomVoices, "List Custom Voices" }
        };

        internal static string GetName(int taskType)
        {
            if (_names.TryGetValue(taskType, out var name)) return name;
            return "Unknown";
        }

        internal static bool HasTextInput(int taskType)
        {
            return taskType == EndpointType.ChatCompletion
            || taskType == EndpointType.ImageCreation
            || taskType == EndpointType.ImageEdit
            || taskType == EndpointType.Speech
            || taskType == EndpointType.JsonSchema
            || taskType == EndpointType.SoundEffect
            || taskType == EndpointType.Video
            || taskType == EndpointType.Moderation
            || taskType == EndpointType.ListModels
            || taskType == EndpointType.ListVoices
            || taskType == EndpointType.ListCustomModels
            || taskType == EndpointType.ListCustomVoices;
        }

        internal static bool HasTextOutput(int taskType)
        {
            return taskType == EndpointType.ChatCompletion
            || taskType == EndpointType.Transcript
            || taskType == EndpointType.Translation
            || taskType == EndpointType.JsonSchema;
        }

        internal static bool HasAudioInput(int taskType)
        {
            return taskType == EndpointType.Transcript
            || taskType == EndpointType.Translation
            || taskType == EndpointType.VoiceChange
            || taskType == EndpointType.AudioIsolation;
        }

        internal static bool HasAudioOutput(int taskType)
        {
            return taskType == EndpointType.Speech
            || taskType == EndpointType.SoundEffect
            || taskType == EndpointType.VoiceChange
            || taskType == EndpointType.AudioIsolation;
        }

        internal static bool HasImageInput(int taskType)
        {
            return taskType == EndpointType.ImageEdit;
            //|| taskType == EndpointType.ImageVariation;
        }

        internal static bool HasImageOutput(int taskType)
        {
            return taskType == EndpointType.ImageCreation
            || taskType == EndpointType.ImageEdit;
            //|| taskType == EndpointType.ImageVariation;
        }
    }
}