namespace Glitch9.AIDevKit.Google
{
    internal class GoogleAIConfig
    {
        internal const string BaseUrl = "https://generativelanguage.googleapis.com";
        internal const string Version = "v1";
        internal const string BetaVersion = "v1beta";
        internal const int MaxQuery = 10;
    }

    /// <summary>
    /// {URL}:{Method}
    /// <para>Example: https://generativelanguage.googleapis.com/v1beta/{name=corpora/*}:query</para>
    /// </summary>
    internal class Methods
    {
        internal const string Query = "query";
        internal const string BatchCreate = "batchCreate";
        internal const string BatchDelete = "batchDelete";
        internal const string BatchUpdate = "batchUpdate";
        internal const string BatchEmbedContents = "batchEmbedContents";
        internal const string CountTokens = "countTokens";
        internal const string EmbedContent = "embedContent";
        internal const string GenerateAnswer = "generateAnswer";
        internal const string GenerateContent = "generateContent";
        internal const string GenerateText = "generateText";
        internal const string StreamGenerateContent = "streamGenerateContent";
        internal const string TransferOwnership = "transferOwnership";
        internal const string Predict = "predict";  // Image Generation, Added 2025.03.30 
        internal const string PredictLongRunning = "predictLongRunning"; // Video Generation, Added 2025.05.05
    }

    // All true for now
    internal class IsBeta
    {
        internal const bool CachedContents = true;
        internal const bool Corpora = true;
        internal const bool CorporaDocuments = true;
        internal const bool CorporaDocumentsChunks = true;
        internal const bool CorporaPermissions = true;
        internal const bool Files = true;
        internal const bool MediaUpload = true;
        internal const bool MediaUploadMetadata = true;
        internal const bool Models = true;
        internal const bool TunedModels = true;
        internal const bool TunedModelsPermissions = true;
    }
}