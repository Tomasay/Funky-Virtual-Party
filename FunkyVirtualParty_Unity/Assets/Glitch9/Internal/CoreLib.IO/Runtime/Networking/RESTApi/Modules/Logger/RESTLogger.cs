namespace Glitch9.IO.Networking.RESTApi
{
    public class RESTLogger : DefaultLogger
    {
        internal static bool enabled = true;

        private static class Tags
        {
            internal const string kRequestEndpoint = "Request-{0}";
            internal const string kRequestHeaders = "Request-Header";
            internal const string kRequestDetails = "Request";
            internal const string kRequestBody = "Request-Body";
            internal const string kResponseDetails = "Response";
            internal const string kResponseBody = "Response-Body";
            internal const string kResponseError = "Response-Error";
            internal const string kResponseStream = "Response-Stream";
        }

        public void ReqEndpoint(string method, string url)
        {
            if (!enabled) return;
            if (!_logLevel.RequestEndpoint()) return;
            url = EndpointFormatter.HideKeyFromEndpoint(url);
            Info(string.Format(Tags.kRequestEndpoint, method), url);
        }

        public void ReqBody(string body)
        {
            if (!enabled) return;
            if (!_logLevel.RequestBody()) return;
            Info(Tags.kRequestBody, body);
        }

        public void ReqVerbose(string details)
        {
            if (!enabled) return;
            if (!_logLevel.RequestDetails()) return;
            Info(Tags.kRequestDetails, details);
        }

        public void ReqHeaders(string header)
        {
            if (!enabled) return;
            if (!_logLevel.RequestHeader()) return;
            Info(Tags.kRequestHeaders, header);
        }

        public void ResContentType(string info)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseDetails()) return;
            Info(Tags.kResponseDetails, $"Content-Type: {info}");
        }

        public void ResVerbose(string info)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseDetails()) return;
            Info(Tags.kResponseDetails, info);
        }

        public void ResBody(string body)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseBody()) return;
            Info(Tags.kResponseBody, body);
        }

        public void Stream(string data)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseStream()) return;
            Info(Tags.kResponseStream, data);
        }

        public void ResVerbose(string eventId, string message)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseDetails()) return;
            Info(eventId, message);
        }

        public void Stream(string eventId, string message)
        {
            if (!enabled) return;
            if (!_logLevel.ResponseStream()) return;
            Info(eventId, message);
        }

        private readonly RESTLogLevel _logLevel;
        public RESTLogLevel LogLevel => _logLevel;

        public RESTLogger(string tag, RESTLogLevel logLevel) : base(tag) => _logLevel = logLevel;

        public override void Info(string message)
        {
            if (!enabled) return;
            base.Info(Tag, message);
        }

        public override void Warning(string message)
        {
            if (!enabled) return;
            base.Warning(Tag, message);
        }

        public override void Error(string message)
        {
            if (!enabled) return;
            base.Error(Tag, message);
        }

        public override void Info(object sender, string message)
        {
            if (!enabled) return;
            base.Info(sender, message);
        }

        public override void Warning(object sender, string message)
        {
            if (!enabled) return;
            base.Warning(sender, message);
        }

        public override void Error(object sender, string message)
        {
            if (!enabled) return;
            base.Error(sender, message);
        }
    }
}