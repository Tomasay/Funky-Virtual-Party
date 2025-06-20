using Newtonsoft.Json;

namespace Glitch9.IO.Networking
{
    public class NameModel
    {
        [JsonProperty("name")] public string Name { get; set; }
        public NameModel() { }
        public NameModel(string name) => Name = name;
    }
}