using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Editor
{
    [JsonObject]
    public class ModelCatalogueEntry : IModelData, ICatalogueEntry
    {
        // --- Core Properties ---
        [JsonProperty] public Api Api { get; set; }
        [JsonProperty] public string Id { get; set; }
        [JsonProperty] public string Name { get; internal set; }
        [JsonProperty] public string Family { get; internal set; }
        [JsonProperty] public string ModelVersion { get; internal set; }
        [JsonProperty] public string FamilyVersion { get; internal set; }
        [JsonProperty] public string Description { get; internal set; }
        [JsonProperty] public string OwnedBy { get; internal set; }
        [JsonProperty] public UnixTime? CreatedAt { get; internal set; }
        [JsonProperty] public bool IsLegacy { get; internal set; } = false;
        [JsonProperty] public bool IsDeprecated { get; set; } = false;


        // --- Token Limit Properties ---
        [JsonProperty] public int? InputTokenLimit { get; internal set; }
        [JsonProperty] public int? OutputTokenLimit { get; internal set; }


        // --- Fine-tuning Related Properties ---  
        [JsonProperty] public string BaseId { get; internal set; }
        [JsonProperty] public bool IsFineTuned { get; internal set; }

        // --- Resolved Properties (not in IModelData) ---
        [JsonProperty] public string Provider { get; internal set; }
        [JsonProperty("Capability")] public ModelFeature Feature { get; internal set; }
        [JsonProperty] public Modality InputModality { get; internal set; }
        [JsonProperty] public Modality OutputModality { get; internal set; }
        [JsonProperty] public Dictionary<UsageType, double> Pricing { get; internal set; } = new();

        [JsonProperty] public ModelEndpoint Endpoint { get; internal set; }    // Added on 2026-06-15
        [JsonProperty] public int Performance { get; internal set; }  // 1~5, 0 is not set
        [JsonProperty] public int Speed { get; internal set; }        // 1~5, 0 is not set

        [JsonConstructor] public ModelCatalogueEntry() { }

        internal static ModelCatalogueEntry Create(IModelData modelData)
        {
            if (modelData == null)
                throw new ArgumentNullException(nameof(modelData), "Model data cannot be null.");

            ModelCatalogueEntry entry = new(modelData);
            return ModelMetadataUtil.Resolve(entry);
        }

        internal static ModelCatalogueEntry ObsoleteEntry(string id, string fieldName)
        {
            return new ModelCatalogueEntry
            {
                Id = id,
                Name = fieldName,
                IsDeprecated = true,
            };
        }

        internal void SetPrices(ModelPrice[] prices)
        {
            if (prices == null) return;

            foreach (ModelPrice price in prices)
            {
                if (price == null) continue;
                Pricing.AddOrUpdate(price.type, price.cost);
            }
        }

        internal ModelPrice[] GetPrices()
        {
            List<ModelPrice> prices = new();
            foreach (KeyValuePair<UsageType, double> pair in Pricing)
            {
                if (pair.Value == 0) continue;
                prices.Add(new ModelPrice(pair.Key, pair.Value));
            }
            return prices.ToArray();
        }

        private ModelCatalogueEntry(IModelData modelData)
        {
            Api = modelData.Api;
            Id = modelData.Id;
            Name = modelData.Name;
            Provider = modelData.Provider;
            Family = modelData.Family;
            ModelVersion = modelData.ModelVersion;
            Description = modelData.Description;
            OwnedBy = modelData.OwnedBy;
            CreatedAt = modelData.CreatedAt;

            InputTokenLimit = modelData.InputTokenLimit;
            OutputTokenLimit = modelData.OutputTokenLimit;

            IsFineTuned = modelData.IsFineTuned == true;

            BaseId = modelData.BaseId;

            if (modelData.Feature != null) Feature = modelData.Feature.Value;
            if (modelData.InputModality != null) InputModality = modelData.InputModality.Value;
            if (modelData.OutputModality != null) OutputModality = modelData.OutputModality.Value;

            Pricing[UsageType.PerCharacter] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerCharacter);
            Pricing[UsageType.Image] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerImage);
            Pricing[UsageType.InputCacheRead] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerInputCacheRead);
            Pricing[UsageType.InputCacheWrite] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerInputCacheWrite);
            Pricing[UsageType.PerRequest] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerRequest);
            Pricing[UsageType.WebSearch] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerWebSearch);
            Pricing[UsageType.InternalReasoning] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerInternalReasoning);
            Pricing[UsageType.PerMinute] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerMinute);
            Pricing[UsageType.InputToken] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerInputToken);
            Pricing[UsageType.OutputToken] = ModelMetadataUtil.ResolveCost(Api, modelData.CostPerOutputToken);

            // remove 0 values from the dictionary
            for (int i = Pricing.Count - 1; i >= 0; i--)
            {
                KeyValuePair<UsageType, double> pair = Pricing.ElementAt(i);
                if (pair.Value == 0) Pricing.Remove(pair.Key);
            }

            (string family, string familyVersion) = ModelMetadataUtil.ResolveFamily(Api, Id, Name);

            if (!string.IsNullOrWhiteSpace(family)) Family = family;
            if (!string.IsNullOrWhiteSpace(familyVersion)) FamilyVersion = familyVersion;
        }

        public bool Equals(ModelCatalogueEntry other) => other != null && Id == other.Id;
    }
}