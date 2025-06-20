
using Newtonsoft.Json;

namespace Glitch9
{
    public static class JsonConfig
    {
        private static JsonSerializerSettings _defaultSerializerSettings;
        public static JsonSerializerSettings DefaultSerializerSettings => _defaultSerializerSettings ??= new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}