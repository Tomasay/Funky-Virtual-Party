
using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    public struct RESTHeader
    {
        internal const string kDefaultAuthHeaderName = "Authorization";
        internal const string kDefaultAuthHeaderFormat = "Bearer {0}";

        public string Name { get; set; }
        public string Value { get; set; }
        public readonly bool IsValid => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Value);

        public RESTHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public static class RESTHeaderExtensions
    {
        public static void SetRequestHeader(this UnityWebRequest request, RESTHeader header)
        {
            request.SetRequestHeader(header.Name, header.Value);
        }
    }
}