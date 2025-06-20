using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public enum RealtimeItemStatus
    {
        [ApiEnum("Completed", "completed")]
        Completed,

        [ApiEnum("InProgress", "in_progress")]
        InProgress,

        [ApiEnum("Incomplete", "incomplete")]
        Incomplete,

        [ApiEnum("Cancelled", "cancelled")]
        Cancelled,

        [ApiEnum("Failed", "failed")]
        Failed,
    }
}
