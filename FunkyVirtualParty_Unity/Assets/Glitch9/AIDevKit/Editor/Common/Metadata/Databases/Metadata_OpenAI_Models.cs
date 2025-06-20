using System;
using System.Text.RegularExpressions;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_OpenAI_Models
    {
        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            // Missing Properties: 
            // ✓ Capability
            // ✓ Name
            // ✘ Version 
            // ✓ Description 
            // ✓ InputModality, OutputModality 
            // ✘ InputTokenLimit, OutputTokenLimit 
            // ✓ IsFineTuned, BaseId

            entry.IsFineTuned = IsFineTuned(entry.Id);

            if (entry.IsFineTuned)
            {
                entry.BaseId = ResolveBaseId(entry.Id);
                entry.Name = ResolveFineTunedName(entry.Id);
                entry.ModelVersion = ResolveFineTunedVersion(entry.Id);
            }
            else
            {
                entry.Name = ModelNameResolver.ResolveFromId(entry.Id);
                entry.Description = Descriptions.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Description, Api.OpenAI, notFound));
                entry.SetPrices(Prices.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Price, Api.OpenAI, notFound)));
            }

            if (TryGetIOModalitiesAndFeatures(entry.Id, out (Modality input, Modality output, ModelFeature cap) result))
            {
                entry.InputModality = result.input;
                entry.OutputModality = result.output;
                entry.Feature = result.cap;
            }

            (int performance, int speed) = Performances.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Performance, Api.OpenAI, notFound));
            entry.Performance = performance;
            entry.Speed = speed;

            entry.Name = ModelMetadataUtil.ResolveLegacyName(entry.Id, entry.Name);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }

        internal static bool TryGetIOModalitiesAndFeatures(string id, out (Modality input, Modality output, ModelFeature features) result)
        {
            foreach (var kvp in ModelPatterns)
            {
                if (id.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    result = kvp.Value;
                    return true;
                }
            }
            result = default;
            return false;
        }

        #region Fine-Tuning Utilities

        /// <summary>
        /// Extracts the fine-tuned model name from an ID like "curie:ft-model-name-2023-04-30-07-37-01".
        /// Returns "model-name".
        /// </summary>
        internal static string ResolveFineTunedName(string id)
        {
            // Match ":ft-" and capture everything up to the date
            var match = Regex.Match(id, @":ft-([^:]+?)-\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2}");
            var name = match.Success ? match.Groups[1].Value : string.Empty;

            name = name.Replace('-', ' ');
            return name.ToTitleCase();
        }

        /// <summary>
        /// Extracts the timestamp portion from an ID like "curie:ft-model-name-2023-04-30-07-37-01".
        /// Returns "2023-04-30-07-37-01".
        /// </summary>
        internal static string ResolveFineTunedVersion(string id)
        {
            var match = Regex.Match(id, @"(\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2})$");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        internal static string ResolveBaseId(string id)
        {
            return id.Substring(0, id.IndexOf(":ft", System.StringComparison.Ordinal));
        }

        internal static bool IsFineTuned(string id) => id.Contains(":ft");

        #endregion Fine-Tuning Utilities
    }
}