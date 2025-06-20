namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    internal static class RealtimeEventUtil
    {
        public static string CreateEventId(int eventCount)
        {
            return $"{eventCount}_{System.DateTime.UtcNow.Ticks}";
        }
    }
}