namespace Glitch9.AIDevKit.Editor
{
    /// <summary>
    /// Utility class for resolving model metadata.
    /// </summary>
    internal static class ModelMetadataUtil
    {

        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            Api api = entry.Api;

            if (api == Api.OpenAI) return Metadata_OpenAI_Models.Resolve(entry);
            if (api == Api.Google) return Metadata_Google_Models.Resolve(entry);
            if (api == Api.ElevenLabs) return Metadata_ElevenLabs_Models.Resolve(entry);
            if (api == Api.Ollama) return Metadata_Ollama_Models.Resolve(entry);
            if (api == Api.OpenRouter) return Metadata_OpenRouter_Models.Resolve(entry);

            return entry;
        }

        internal static string ResolveLegacyName(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) return id;

            if (name.Contains("(Legacy)"))
            {
                name = name.Replace("(Legacy)", "").Trim();
            }

            return name;
        }

        internal static double ResolveCost(Api api, string costAsString)
        {
            if (api == Api.OpenRouter) return Metadata_OpenRouter_Models.ResolveCost(costAsString);
            return 0;
        }

        internal static (string family, string familyVersion) ResolveFamily(Api provider, string id, string name)
        {
            return ModelFamilyResolver.Resolve(provider, id, name);
        }

        internal static string RemoveSlashPrefix(string id)
        {
            if (id.Contains('/'))
            {
                string[] parts = id.Split('/');
                id = parts[1];
            }

            return id;
        }

        internal static bool IsOModel(string id)
        {
            id = id.ToLowerInvariant().Replace(" ", "-");
            if (id.StartsWith("o") && id.Length > 1 && char.IsDigit(id[1])) return true;
            return false;
        }

        internal static bool HasDiffProvider(Api api, string provider)
        {
            if (string.IsNullOrEmpty(provider)) return false;
            // make both lowercase for comparison
            string lowerApi = api.ToString().ToLowerInvariant();
            string lowerProvider = provider.ToLowerInvariant();
            return lowerApi != lowerProvider;
        }
    }
}