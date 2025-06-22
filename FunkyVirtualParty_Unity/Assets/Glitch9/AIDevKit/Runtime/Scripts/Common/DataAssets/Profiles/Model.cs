using UnityEngine;
using Glitch9.ScriptableObjects;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using UnityEngine.Serialization;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// ScriptableObject representation of a generative AI model with metadata, configuration, and pricing information.
    /// Supports token limits, ownership, creation time, and dynamic pricing for various content types (text, image, audio).
    /// </summary>
    [JsonConverter(typeof(ModelConverter))]
    public class Model : AIDevKitAsset, IData
    {
        [SerializeField] private string family;
        [SerializeField] private string familyVersion;
        [SerializeField] private string modelVersion;
        [SerializeField] private bool legacy;
        [SerializeField] private Modality inputModality;
        [SerializeField] private Modality outputModality;
        [SerializeField, FormerlySerializedAs("capability")] private ModelFeature feature;
        [SerializeField] private int maxInputTokens;
        [SerializeField] private int maxOutputTokens;
        [SerializeReference] private ModelPrice[] prices;
        [SerializeField] private ModelEndpoint endpoint;

        #region Getters

        /// <summary>
        /// Features of the model, such as text generation, image generation, etc.
        /// This information is used to determine the capabilities of the model.
        /// </summary>
        public ModelFeature Feature => feature;

        /// <summary>
        /// The family classification of the model (e.g., GPT, Gemini, Imagen).
        /// This information is usually used to determine the structure of the request body.
        /// </summary>
        public string Family => family;

        /// <summary>
        /// Indicates whether the model is a legacy version.
        /// Legacy models often have a different endpoint or API structure.
        /// </summary>
        public bool IsLegacy => legacy;

        /// <summary>
        /// A bitflag indicating supported input modalities (e.g., Text, Image, Audio).
        /// This information is used to determines which input types the model supports.
        /// </summary>
        public Modality InputModality => inputModality;

        /// <summary>
        /// A bitflag indicating supported output modalities (e.g., Text, Image, Audio).
        /// This information is used to determines which output types the model supports.
        /// </summary>
        public Modality OutputModality => outputModality;

        /// <summary>
        /// The maximum number of images that this model can generate in a single request.
        /// This information is only relevant for image generation models.
        /// </summary>
        public int MaxN => AIDevKitConfig.ResolveMaxN(this);

        /// <summary>
        /// The specific version identifier of the model.
        /// </summary>
        public string ModelVersion => modelVersion;

        /// <summary>
        /// The version identifier for the model's family or group (e.g., GPT-4, Gemini-1.5).
        /// </summary>
        public string FamilyVersion => familyVersion;

        /// <summary>
        /// The maximum number of input tokens allowed for this model.
        /// </summary>
        public int MaxInputTokens => maxInputTokens;

        /// <summary>
        /// The maximum number of output tokens that the model can generate.
        /// </summary>
        public int MaxOutputTokens => maxOutputTokens;

        /// <summary>
        /// Indicates whether the model is a fine-tuned version (i.e., custom).
        /// </summary>
        public bool IsFineTuned => IsCustom;

        /// <summary>
        /// An array of pricing definitions for this model based on content type and usage (e.g., per 1K tokens).
        /// </summary>
        public ModelPrice[] Prices => prices;

        /// <summary>
        /// The API endpoint available for this model.
        /// </summary>
        public ModelEndpoint Endpoint => endpoint;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Estimates the price of using the model based on a usage report.
        /// Automatically handles free usage and empty input.
        /// </summary>
        /// <param name="usage">The usage data containing token counts by type.</param>
        /// <returns>A <see cref="Currency"/> object representing the estimated price.</returns>
        public Currency EstimatePrice(Usage usage)
        {
            if (usage == null || usage.IsEmpty) return 0;
            if (usage.IsFree) return -1; // Free usage

            double priceResult = 0;

            foreach (var kvp in usage.usages)
            {
                if (kvp.Value == 0)
                {
                    //GNDebug.Pink($"Usage {kvp.Key} is 0. Skipping.");
                    continue;
                }
                double cost = kvp.Value * GetCost(kvp.Key);
                //GNDebug.Pink($"Usage {kvp.Key} is {kvp.Value}. Cost: {cost}.");
                priceResult += cost;
            }

            //GNDebug.Pink($"Estimated price for {usage} is {priceResult}."); 
            return new Currency(priceResult);
        }

        public bool HasFeature(ModelFeature feature)
        {
            return (this.feature & feature) == feature;
        }

        internal void UpdateData(
            string id = null,
            string name = null,
            Api? api = null,
            string family = null,
            string familyVersion = null,
            string modelVersion = null,
            Modality? inputModality = null,
            Modality? outputModality = null,
            ModelFeature? capability = null,
            int? inputTokenLimit = null,
            int? outputTokenLimit = null,
            bool? legacy = null,
            bool? fineTuned = null,
            ModelPrice[] prices = null,
            ModelEndpoint? endpoint = null)
        {
            if (!string.IsNullOrEmpty(id)) this.id = id;
            if (!string.IsNullOrEmpty(name)) displayName = name;
            if (api != null) this.api = api.Value;
            if (!string.IsNullOrEmpty(family)) this.family = family;
            if (!string.IsNullOrEmpty(familyVersion)) this.familyVersion = familyVersion;
            if (!string.IsNullOrEmpty(modelVersion)) this.modelVersion = modelVersion;
            if (inputModality != null) this.inputModality = inputModality.Value;
            if (outputModality != null) this.outputModality = outputModality.Value;
            if (capability != null) this.feature = capability.Value;
            if (inputTokenLimit != null) maxInputTokens = inputTokenLimit.Value;
            if (outputTokenLimit != null) maxOutputTokens = outputTokenLimit.Value;
            if (legacy != null) this.legacy = legacy.Value;
            if (fineTuned != null) custom = fineTuned.Value;
            if (prices != null) this.prices = prices;
            if (endpoint != null) this.endpoint = endpoint.Value;

            //this.SaveAsset(); // TODO: save in the editor code, not here
        }

        // internal void SetData(IModelData modelData, bool? isLegacy = null, ModelPrice[] prices = null)
        // {
        //     if (modelData == null) throw new ArgumentNullException(nameof(modelData), "Model data cannot be null.");

        //     if (!string.IsNullOrEmpty(modelData.Id)) id = modelData.Id;
        //     if (!string.IsNullOrEmpty(modelData.Name)) displayName = modelData.Name;
        //     if (modelData.Api != Api.None || modelData.Api != Api.All) api = modelData.Api;
        //     if (!string.IsNullOrEmpty(modelData.Family)) family = modelData.Family;
        //     if (!string.IsNullOrEmpty(modelData.FamilyVersion)) familyVersion = modelData.FamilyVersion;
        //     if (!string.IsNullOrEmpty(modelData.ModelVersion)) modelVersion = modelData.ModelVersion;
        //     if (modelData.InputModality != null) inputModality = modelData.InputModality.Value;
        //     if (modelData.OutputModality != null) outputModality = modelData.OutputModality.Value;
        //     if (modelData.Feature != null) feature = modelData.Feature.Value;
        //     if (modelData.InputTokenLimit != null) maxInputTokens = modelData.InputTokenLimit.Value;
        //     if (modelData.OutputTokenLimit != null) maxOutputTokens = modelData.OutputTokenLimit.Value;
        //     if (modelData.IsFineTuned != null) custom = modelData.IsFineTuned.Value;
        //     if (isLegacy != null) legacy = isLegacy.Value;
        //     if (prices != null) this.prices = prices;

        //     this.SaveAsset();
        // }

        internal double GetCost(UsageType type)
        {
            foreach (var price in prices)
            {
                if (price.type == type) return price.cost;
            }
            AIDevKitDebug.Warning($"Model '{name}' doesn't have price information for {type} not found. Returning 0.");
            return 0;
        }

        #endregion Utility Methods

        #region Caching

        public static implicit operator Model(string apiName) => Cache.Get(apiName);
        internal static class Cache
        {
            private static readonly Dictionary<string, Model> _cache = new();
            private static readonly HashSet<string> _tried = new();

            internal static Model Get(string id)
            {
                if (string.IsNullOrEmpty(id)) return null;

                //if (id == "ElevenLabs") id = "eleven_flash_v2_5"; // "ElevenLabs" is invalid. Idk where it came from....

                if (_cache.TryGetValue(id, out Model model)) return model;

                if (ModelLibrary.TryGetValue(id, out model))
                {
                    if (model != null)
                    {
                        _cache.AddOrUpdate(id, model);
                        return model;
                    }
                }

                if (_tried.Contains(id)) return null; // Avoid infinite loop
                _tried.Add(id);

                Debug.LogWarning($"The {typeof(Model).Name} {id} is not found in the '{typeof(ModelLibrary).Name}.asset'.");

                return null;
            }
        }

        #endregion Caching
    }

    internal class ModelConverter : JsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
            => writer.WriteValue(value.Id);
        public override Model ReadJson(JsonReader reader, Type objectType, Model existingValue, bool hasExistingValue, JsonSerializer serializer)
            => reader.Value as string;
    }
}