using System;
using System.Collections.Generic;
using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class AIDevKitGUIUtility
    {
        private static Dictionary<ModelFeature, GUIContent> _capContents;

        private static readonly ModelFeature[] _priorityOrder = new[]
        {
            ModelFeature.TextGeneration,
            ModelFeature.Realtime,
            ModelFeature.ImageGeneration,
            ModelFeature.SpeechRecognition,
            ModelFeature.SpeechGeneration,
            ModelFeature.Moderation,
            ModelFeature.FunctionCalling,
            ModelFeature.Streaming,
            ModelFeature.Caching,
            ModelFeature.ImageInpainting,
            ModelFeature.SoundFXGeneration,
            ModelFeature.VideoGeneration,
            ModelFeature.TextEmbedding,
            ModelFeature.Search,
            ModelFeature.ComputerUse,
            ModelFeature.StructuredOutputs,
            ModelFeature.FineTuning,
        };

        internal static readonly CurrencyCode[] SelectedCurrencyCodes = new[]
        {
            CurrencyCode.USD,
            CurrencyCode.EUR,
            CurrencyCode.GBP,
            CurrencyCode.CAD,
            CurrencyCode.AUD,
            CurrencyCode.JPY,
            CurrencyCode.CNY,
            CurrencyCode.INR,
            CurrencyCode.RUB,
            CurrencyCode.BRL,
            CurrencyCode.KRW,
        };

        private static Dictionary<ModelFeature, GUIContent> CreateCapabilityContents()
        {
            Dictionary<ModelFeature, GUIContent> contents = new();
            var array = Enum.GetValues(typeof(ModelFeature));
            // Sort the array based on the priority order
            List<ModelFeature> sortedArray = new(array.Length);

            foreach (ModelFeature capability in array)
            {
                if (capability == ModelFeature.None) continue;
                sortedArray.Add(capability);
            }

            sortedArray.Sort((x, y) =>
            {
                int indexX = Array.IndexOf(_priorityOrder, x);
                int indexY = Array.IndexOf(_priorityOrder, y);
                return indexX.CompareTo(indexY);
            });

            foreach (ModelFeature cap in sortedArray)
            {
                if (cap == ModelFeature.None) continue;

                GUIContent content = new(GetModelFeatureIcon(cap), cap.GetInspectorName());
                contents.Add(cap, content);
            }

            return contents;
        }

        internal static List<GUIContent> GetFeatureContents(ModelFeature cap)
        {
            List<GUIContent> contents = new();

            if (cap == ModelFeature.None)
            {
                contents.Add(new GUIContent("None"));
                return contents;
            }

            _capContents ??= CreateCapabilityContents();

            long capValue = (long)cap;
            foreach (var kvp in _capContents)
            {
                long bit = (long)kvp.Key;
                if ((capValue & bit) == bit)
                {
                    contents.Add(kvp.Value);
                }
            }

            return contents;
        }

        internal static Texture GetModelFeatureIcon(ModelFeature cap)
        {
            return cap switch
            {
                ModelFeature.TextGeneration => AIDevKitIcons.Text,
                ModelFeature.StructuredOutputs => AIDevKitIcons.JsonSchema,
                ModelFeature.CodeExecution => AIDevKitIcons.Code,
                ModelFeature.FunctionCalling => AIDevKitIcons.Tools,
                ModelFeature.Caching => AIDevKitIcons.Caching,
                ModelFeature.ImageGeneration => AIDevKitIcons.Image,
                ModelFeature.ImageInpainting => AIDevKitIcons.Inpainting,
                ModelFeature.SpeechGeneration => AIDevKitIcons.TextToSpeech,
                ModelFeature.SpeechRecognition => AIDevKitIcons.SpeechToText,
                ModelFeature.SoundFXGeneration => AIDevKitIcons.SoundFX,
                ModelFeature.VideoGeneration => AIDevKitIcons.Video,
                ModelFeature.TextEmbedding => AIDevKitIcons.Embedding,
                ModelFeature.Moderation => AIDevKitIcons.Moderation,
                ModelFeature.Search => EditorIcons.Search,
                ModelFeature.Realtime => AIDevKitIcons.Realtime,
                ModelFeature.FineTuning => AIDevKitIcons.FineTuning,
                ModelFeature.Streaming => AIDevKitIcons.Streaming,
                ModelFeature.ComputerUse => AIDevKitIcons.Code,

                _ => EditorIcons.Question,
            };
        }

        internal static Texture2D GetApiIcon(Api api)
        {
            Texture2D texture = api switch
            {
                Api.OpenAI => AIDevKitIcons.OpenAI,
                Api.Google => AIDevKitIcons.Google,
                Api.ElevenLabs => AIDevKitIcons.ElevenLabs,
                Api.Mubert => AIDevKitIcons.Mubert,
                Api.Ollama => AIDevKitIcons.Ollama,
                Api.OpenRouter => AIDevKitIcons.OpenRouter,
                _ => AIDevKitIcons.OpenAI
            };

            return texture;
        }

        internal static string FormatValue<T>(T value)
        {
            return value switch
            {
                Enum e => e.GetInspectorName(),
                bool b => b ? "True" : "False",
                _ => value.ToString()
            };
        }

        internal static readonly Dictionary<HarmCategory, GUIContent> HarmCategoryMap = new()
        {
            { HarmCategory.None, new GUIContent("None", "Default value (it's null).") },
            { HarmCategory.Unspecified, new GUIContent("Unspecified", "Content that is not classified into any specific category.") },

            { HarmCategory.Derogatory, new GUIContent("Derogatory", "Negative or harmful comments targeting identity and/or protected attribute.") },
            { HarmCategory.Toxicity, new GUIContent("Toxicity", "Content that is rude, disrespectful, or profane.") },
            { HarmCategory.Violence, new GUIContent("Violence", "Scenarios depicting violence against an individual or group, or general descriptions of gore.") },
            { HarmCategory.Sexual, new GUIContent("Sexual", "Contains references to sexual acts or other lewd content.") },
            { HarmCategory.Medical, new GUIContent("Medical", "Promotes unchecked medical advice.") },
            { HarmCategory.Dangerous, new GUIContent("Dangerous", "Dangerous content that promotes, facilitates, or encourages harmful acts.") },

            { HarmCategory.Harassment, new GUIContent("Harassment", "Harassment content.") },
            { HarmCategory.SexuallyExplicit, new GUIContent("Sexually Explicit", "Sexually explicit content.") },
            { HarmCategory.DangerousContent, new GUIContent("Dangerous Content", "Dangerous content.") },
            { HarmCategory.SexualContent, new GUIContent("Sexual Content", "Arousing or promotional sexual content (excluding education/wellness).") },

            { HarmCategory.Hate, new GUIContent("Hate", "Content that promotes or incites hate based on protected attributes.") },
            { HarmCategory.SelfHarm, new GUIContent("Self-Harm", "Content that depicts, encourages, or describes acts of self-harm.") },
            { HarmCategory.SexualMinors, new GUIContent("Sexual (Minors)", "Sexual content involving individuals under 18 years old.") },
            { HarmCategory.HateThreatening, new GUIContent("Hate (Threatening)", "Hateful content that also includes violence or serious harm.") },
            { HarmCategory.ViolenceGraphic, new GUIContent("Violence (Graphic)", "Graphic detail of death, violence, or physical injury.") },

            { HarmCategory.SelfHarmIntent, new GUIContent("Self-Harm (Intent)", "Speaker expresses intent to engage in self-harm.") },
            { HarmCategory.SelfHarmInstructions, new GUIContent("Self-Harm (Instructions)", "Encourages or instructs how to commit acts of self-harm.") },
            { HarmCategory.HarassmentThreatening, new GUIContent("Harassment (Threatening)", "Harassment content involving violence or serious harm.") },
        };
    }
}