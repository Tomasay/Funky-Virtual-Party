using System;

namespace Glitch9.IO.Networking.RESTApi
{
    public enum ParamType
    {
        Unset,
        Query,
        Header
    }

    public class CRUDParam
    {
        /// <summary>
        /// Required.
        /// Indicates how the API key should be sent.
        /// </summary>
        public ParamType Type { get; set; }

        /// <summary>
        /// Optional.
        /// If the API requires a specific header name instead of 'Authorization', it will be set here. 
        /// </summary>
        public string HeaderName { get; set; }

        /// <summary>
        /// Optional.
        /// If the API requires a specific header name instead of 'Bearer {0}', it will be set here. 
        /// </summary>
        public string HeaderFormat { get; set; }

        /// <summary>
        /// Required if <see cref="Type"/> is set to <see cref="ParamType.Query"/>,
        /// or disregarded otherwise.
        /// </summary>
        public string QueryKey { get; set; }

        /// <summary>
        /// Either Value or Getter must be set.
        /// <para>Value is used if the API key is static.</para>
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Required if <see cref="Type"/> is set.
        /// </summary>
        public Func<string> Getter { get; set; }

        public static CRUDParam Header(Func<string> getter, string headerName = null, string headerFormat = null)
        {
            return new CRUDParam
            {
                Type = ParamType.Header,
                Getter = getter,
                HeaderName = headerName,
                HeaderFormat = headerFormat
            };
        }

        public static CRUDParam Query(Func<string> getter, string queryKey = null)
        {
            return new CRUDParam
            {
                Type = ParamType.Query,
                Getter = getter,
                QueryKey = queryKey
            };
        }

        public static CRUDParam Header(string value, string headerName = null, string headerFormat = null)
        {
            return new CRUDParam
            {
                Type = ParamType.Header,
                Value = value,
                HeaderName = headerName,
                HeaderFormat = headerFormat
            };
        }

        public static CRUDParam Query(string value, string queryKey = null)
        {
            return new CRUDParam
            {
                Type = ParamType.Query,
                Value = value,
                QueryKey = queryKey
            };
        }

        public string GetValue()
        {
            if (Getter != null) return Getter();
            if (Value != null) return Value;
            throw new InvalidOperationException("Either Getter or Value must be set.");
        }
    }

    public class CRUDClientSettings : RESTClientSettings
    {
        /// <summary>
        /// Required.
        /// The name of the API.
        /// <para>Example: "OpenAI"</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// The default base url for all the requests.
        /// <para>Example: "https://api.openai.com"</para>
        /// </summary>
        public string BaseURL { get; set; }

        /// <summary>
        /// Required.
        /// Indicates how the API key should be sent.
        /// </summary>
        public CRUDParam ApiKey { get; set; }

        /// <summary>
        /// Required.
        /// This is the version of the API, in case it changes, it will be updated here.
        /// <para>Example: "v1"</para>
        /// </summary>
        public CRUDParam Version { get; set; }

        /// <summary>
        /// Optional.
        /// This is the beta version of the API, in case it changes, it will be updated here.
        /// <para>Example: "v1beta"</para>
        /// </summary>
        public CRUDParam BetaVersion { get; set; }

        /// <summary>
        /// Optional.
        /// Add additional headers to the request.
        /// </summary>
        public RESTHeader[] AdditionalHeaders { get; set; }
    }
}