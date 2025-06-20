
namespace Glitch9.AIDevKit.OpenAI
{
    internal class OpenAIConfig
    {
        internal const string Version = "v1";
        internal const string BetaVersion = "v2";
        internal const string RealtimeVersion = "v1";

        internal const string BaseUrl = "https://api.openai.com";

        internal const string OrganizationHeaderName = "OpenAI-Organization";
        internal const string ProjectHeaderName = "OpenAI-Project";

        internal const string AutoType = "auto";
        internal const string BetaHeaderName = "OpenAI-Beta";
        internal const string BetaHeader_Assistants = "assistants=" + BetaVersion;
        internal const string BetaHeader_Realtime = "realtime=" + RealtimeVersion;

        internal const int MaxQuery = 100;

        internal static string[] AllVoices => new[]
        {
            OpenAIVoice.Alloy,
            OpenAIVoice.Echo,
            OpenAIVoice.Fable,
            OpenAIVoice.Onyx,
            OpenAIVoice.Nova,
            OpenAIVoice.Shimmer,
            OpenAIVoice.Ash,
            OpenAIVoice.Coral,
            OpenAIVoice.Sage,
            //OpenAIVoice.Ballad
        };

        // /// <summary>
        // /// OpenAI official default values
        // /// </summary>
        // internal static class Defaults
        // {
        //     // Image
        //     internal const int DALLE_2_MAX_N = 10;
        //     internal const int DALLE_3_MAX_N = 1;
        //     internal const int IMAGE_COUNT = 1;
        //     internal const string DALLE_MODEL = OpenAIModel.DallE2;
        //     internal const ImageSize IMAGE_SIZE = ImageSize._1024x1024;
        //     internal const ImageQuality IMAGE_QUALITY = ImageQuality.Standard;
        //     internal const ImageStyle IMAGE_STYLE = ImageStyle.Vivid;

        //     // Query 
        //     internal const QueryOrder QUERY_ORDER = QueryOrder.Descending;
        // }
    }
}
