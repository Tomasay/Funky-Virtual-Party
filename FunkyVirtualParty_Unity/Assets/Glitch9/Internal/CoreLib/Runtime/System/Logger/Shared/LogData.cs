namespace Glitch9
{
    public class LogData
    {
        public string Log { get; private set; }
        public LogType Type { get; private set; }
        public LogData(LogType type, string log)
        {
            Log = log;
            Type = type;
        }
    }
}