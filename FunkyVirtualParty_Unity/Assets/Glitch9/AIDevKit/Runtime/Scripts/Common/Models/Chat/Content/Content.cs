using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit
{
    public class Content
    {
        public static implicit operator Content(string text) => new(text);
        public static implicit operator Content(ContentPartWrapper[] parts) => new(parts);
        public static implicit operator string(Content content) => new(content.ToString());

        private readonly List<ContentPartWrapper> _parts = new();
        internal string text;
        private string _textCache;

        [JsonConstructor] public Content() { }
        public Content(string text) => this.text = text;
        public Content(string[] texts) => _parts = texts.Select(text => new ContentPartWrapper(text)).ToList();
        public Content(ContentPartWrapper[] parts) => _parts = parts.ToList();
        public static Content FromParts(IEnumerable<ContentPart> parts) => new(parts.Select(part => new ContentPartWrapper(part)).ToArray());

        public bool HasValue => text != null || _parts != null;
        public bool IsString => text != null && _parts.IsNullOrEmpty();
        public bool IsNull => !HasValue;

        /// <summary>
        /// Returns the length of the array if the value is an array,
        /// returns the length of the string if the value is a string,
        /// otherwise returns 0.
        /// </summary>
        public int Length => text?.Length ?? _parts.Count;

        public override string ToString()
        {
            if (_textCache != null) return _textCache;

            bool isSet = false;

            if (text != null)
            {
                _textCache = text;
                isSet = true;
            }
            else if (!_parts.IsNullOrEmpty())
            {
                foreach (var part in _parts)
                {
                    if (part?.ToPart() is TextContentPart textPart)
                    {
                        _textCache += textPart.Text?.Value;
                        isSet = true;
                    }
                }
            }

            if (!isSet) _textCache = string.Empty;
            return _textCache;
        }

        internal List<ContentPartWrapper> ToList() => _parts;
        internal ContentPart[] ToPartArray() => _parts.Select(part => part.ToPart()).ToArray();

        public void ReplaceText(string newText)
        {
            if (string.IsNullOrEmpty(newText)) return;

            if (IsString)
            {
                text = newText;
                _textCache = newText; // 캐시 업데이트
            }
            else
            {
                // find the first TextContentPart and replace its text
                if (_parts.FirstOrDefault(p => p.ToPart() is TextContentPart) is ContentPartWrapper textPart)
                {
                    if (textPart.ToPart() is TextContentPart existingTextPart)
                    {
                        existingTextPart.Text.Value = newText;
                        _textCache = newText; // 캐시 업데이트
                    }
                }
                else
                {
                    // If no TextContentPart exists, create a new one
                    var newTextPart = new TextContentPart(newText);
                    _parts.Add(new ContentPartWrapper(newTextPart));
                    _textCache = newText; // 캐시 업데이트
                }
            }
        }

        public void AddPart<T>(T part) where T : ContentPart
        {
            if (text != null) TextToTextPart();
            _parts.Add(new ContentPartWrapper(part)); // 래핑 필요
        }

        private void TextToTextPart()
        {
            if (text == null) return;

            AIDevKitDebug.Mark($"Converting text to TextContentPart: {text}");

            var textPart = new TextContentPart(text);
            _parts.Add(new ContentPartWrapper(textPart)); // 반드시 wrapper로 
            text = null;
        }

        public void AddPartRange<T>(IEnumerable<T> parts) where T : ContentPart
        {
            AIDevKitDebug.Mark($"Adding part range of type {typeof(T)}");

            if (text != null) TextToTextPart();
            foreach (var part in parts)
            {
                if (part == null) continue;
                _parts.Add(new ContentPartWrapper(part)); // 반드시 wrapper로
            }
        }
    }

    internal class ContentConverter : JsonConverter<Content>
    {
        private readonly Api _api;
        internal ContentConverter(Api api) => _api = api;

        public override void WriteJson(JsonWriter writer, Content value, JsonSerializer serializer)
        {
            if (!value.HasValue)
            {
                //  writer.WriteNull();
                writer.WriteValue("-empty-");
                return;
            }

            string text = value.text;

            if (!string.IsNullOrEmpty(text))
            {
                writer.WriteValue(text);
                return;
            }

            var parts = value.ToPartArray();

            if (parts.IsNullOrEmpty())
            {
                parts = new ContentPart[1] { new TextContentPart("-empty-") };
                //return;
            }

            writer.WriteStartArray();

            foreach (var part in parts)
            {
                if (part == null) continue;
                bool ignore = _api == Api.None && part.IsBase64;
                if (!ignore) serializer.Serialize(writer, part);
            }

            writer.WriteEndArray();
        }

        public override Content ReadJson(JsonReader reader, Type objectType, Content existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            //GNDebug.Mark($"Writing JSON for api: {_api} / Token type: {token.Type}");

            if (token.Type == JTokenType.String)
            {
                return new Content(token.ToObject<string>());
            }
            else if (token.Type == JTokenType.Array)
            {
                List<ContentPartWrapper> parts = new();
                foreach (var item in token.Children())
                {
                    //GNDebug.Mark($"Parsing part token: {item.Type}");
                    if (item.Type == JTokenType.String)
                    {
                        //GNDebug.Mark($"Parsing string part: {item.ToObject<string>()}");
                        parts.Add(new ContentPartWrapper(item.ToObject<string>()));
                    }
                    else if (item.Type == JTokenType.Object)
                    {
                        var part = item.ToObject<ContentPartWrapper>(serializer);
                        if (part != null) parts.Add(part);
                    }
                }
                return new Content(parts.ToArray());
            }

            throw new JsonSerializationException("Unexpected token type when parsing ChatContent.");
        }
    }
}