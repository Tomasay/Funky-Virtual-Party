using Newtonsoft.Json;

namespace Glitch9.IO.Networking
{
    public class TextModel
    {
        [JsonProperty("text")] public string Text { get; set; }
        public TextModel() { }
        public TextModel(string text) => Text = text;
    }
}