namespace Glitch9.AIDevKit.Editor
{
    internal static class Metadata_OpenRouter_Models
    {
        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            // Missing Properties:
            // ✓ Capability
            // ✘ Version
            // ✓ Provider

            //entry.Name = entry.Id;

            if (!string.IsNullOrWhiteSpace(entry.Name) && entry.Name.Contains("(free)"))
            {
                entry.SetPrices(ModelPrice.Free());
            }

            entry.Provider = ResolveProvider(entry.Id);

            bool featuresFound = false;

            if (entry.Provider == Api.OpenAI.ToString())
            {
                if (Metadata_OpenAI_Models.TryGetIOModalitiesAndFeatures(entry.Id, out (Modality, Modality, ModelFeature cap) result))
                {
                    entry.Feature = result.cap;
                    featuresFound = true;
                }
            }

            if (!featuresFound)
            {
                if (Metadata_Shared_Models.Features.TryGetValue(entry.Id, out ModelFeature features))
                {
                    entry.Feature = features;
                }
                else
                {
                    entry.Feature = ModelFeature.TextGeneration;
                }
            }

            (int performance, int speed) performanceAndSpeed;

            if (entry.Id.StartsWith("openai"))
            {
                performanceAndSpeed = Metadata_OpenAI_Models.Performances.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Performance, Api.OpenAI, notFound));
            }
            else if (entry.Id.StartsWith("google"))
            {
                performanceAndSpeed = Metadata_Google_Models.Performances.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Performance, Api.Google, notFound));
            }
            else
            {
                performanceAndSpeed = Metadata_Shared_Models.Performances.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Performance, Api.OpenRouter, notFound));
            }

            entry.Performance = performanceAndSpeed.performance;
            entry.Speed = performanceAndSpeed.speed;

            entry.Name = ModelNameResolver.RemoveColonPrefix(entry.Name, 1);
            entry.Name = ModelMetadataUtil.ResolveLegacyName(entry.Id, entry.Name);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }

        internal static double ResolveCost(string costAsString)
        {
            if (string.IsNullOrEmpty(costAsString)) return 0.0;
            return double.TryParse(costAsString, out double result) ? result : 0.0;
        }

        private static string ResolveProvider(string id)
        {
            if (id.Contains('/'))
            {
                string[] parts = id.Split('/');
                id = parts[0].CapFirstChars('-').Trim();

                if (id.EndsWith("ai") || id.EndsWith("-Ai"))
                {
                    // capitalize ai => AI
                    id = id[..^2] + "AI";
                }

                // if id starts with "Ai" and the third char is not a letter, capitalize AI
                if (id.StartsWith("Ai") && id.Length > 2 && !char.IsLetter(id[2]))
                {
                    id = "AI" + id[2..];
                }

                if (id.Contains("Deepseek"))
                {
                    id = id.Replace("Deepseek", "DeepSeek");
                }
            }

            return id;
        }
    }
}