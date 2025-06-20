namespace Glitch9.AIDevKit.Editor
{
    internal static class Metadata_Ollama_Models
    {
        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            // Missing Properties:
            // ✓ Name
            // ✓ Capability
            // ✘ Version
            // ✘ CreatedAt
            // ✘ Description 
            // ✘ InputModality, OutputModality
            // ✘ InputTokenLimit, OutputTokenLimit 
            // ✓ Provider

            entry.Name = ModelNameResolver.ResolveFromId(entry.Id);
            entry.Feature = ModelFeature.TextGeneration;
            entry.Provider = ModelProviderResolver.Resolve(entry.Id);

            entry.Name = ModelNameResolver.RemoveColonPrefix(entry.Name, 0);
            entry.Name = ModelMetadataUtil.ResolveLegacyName(entry.Id, entry.Name);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }
    }
}