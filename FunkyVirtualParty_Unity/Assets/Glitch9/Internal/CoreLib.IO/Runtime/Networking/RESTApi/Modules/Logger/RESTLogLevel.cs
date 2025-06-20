using System;

namespace Glitch9.IO.Networking.RESTApi
{
    [Flags]
    public enum RESTLogLevel
    {
        None = 0,
        RequestHeader = 1 << 0,
        RequestBody = 1 << 1,
        RequestEndpoint = 1 << 2,
        RequestDetails = 1 << 3,
        ResponseDetails = 1 << 4,
        ResponseBody = 1 << 5,
        ResponseStream = 1 << 6,
        All = RequestHeader | RequestBody | RequestEndpoint | RequestDetails | ResponseDetails | ResponseBody | ResponseStream,
    }

    public static class RESTLogLevelExtensions
    {
        public static bool RequestHeader(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.RequestHeader) != 0;
        public static bool RequestBody(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.RequestBody) != 0;
        public static bool RequestEndpoint(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.RequestEndpoint) != 0;
        public static bool RequestDetails(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.RequestDetails) != 0;
        public static bool ResponseDetails(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.ResponseDetails) != 0;
        public static bool ResponseBody(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.ResponseBody) != 0;
        public static bool ResponseStream(this RESTLogLevel logLevel) => (logLevel & RESTLogLevel.ResponseStream) != 0;
        public static string ToString(this RESTLogLevel logLevel) =>
           $"{(logLevel.RequestHeader() ? "RequestHeader" : "")} " +
           $"{(logLevel.RequestBody() ? "RequestBody" : "")} " +
           $"{(logLevel.RequestEndpoint() ? "RequestEndpoint" : "")} " +
           $"{(logLevel.RequestDetails() ? "RequestDetails" : "")} " +
           $"{(logLevel.ResponseDetails() ? "ResponseState" : "")} " +
           $"{(logLevel.ResponseBody() ? "ResponseBody" : "")} " +
           $"{(logLevel.ResponseStream() ? "ResponseStream" : "")}".Trim();
    }
}