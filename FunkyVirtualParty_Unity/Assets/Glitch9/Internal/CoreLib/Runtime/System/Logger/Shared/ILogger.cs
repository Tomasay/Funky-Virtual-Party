namespace Glitch9
{
    public interface ILogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);

        void Info(object sender, string message);
        void Warning(object sender, string message);
        void Error(object sender, string message);
    }
}