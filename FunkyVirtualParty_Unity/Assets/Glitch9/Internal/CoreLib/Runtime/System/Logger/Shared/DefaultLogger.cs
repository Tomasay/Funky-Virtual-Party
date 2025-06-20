namespace Glitch9
{
    public class DefaultLogger : ILogger
    {
        protected string Tag;
        public DefaultLogger(string tag) => Tag = tag;
        private LogListener _listener;

        public virtual void Info(string message)
        {
            Info(Tag, message);
        }

        public virtual void Warning(string message)
        {
            Warning(Tag, message);
        }

        public virtual void Error(string message)
        {
            Error(Tag, message);
        }

        public virtual void Info(object sender, string message)
        {
            LogService.Info(message, sender);
            _listener?.OnInfo?.Invoke(message);
        }

        public virtual void Warning(object sender, string message)
        {
            LogService.Warning(message, sender);
            _listener?.OnWarning?.Invoke(message);
        }

        public virtual void Error(object sender, string message)
        {
            LogService.Error(message, sender);
            _listener?.OnError?.Invoke(message);
        }

        public void AddListener(LogListener listener)
        {
            _listener = listener;
        }
    }
}