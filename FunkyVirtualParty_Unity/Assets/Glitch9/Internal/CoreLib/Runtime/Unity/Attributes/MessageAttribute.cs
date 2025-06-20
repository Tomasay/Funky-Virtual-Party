using System;
using System.Collections.Generic;
using System.Reflection;

namespace Glitch9
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class MessageAttribute : Attribute
    {
        public string Message { get; }

        public MessageAttribute(string message)
        {
            Message = message;
        }
    }

    public static class MessageExtensions
    {
        private static readonly Dictionary<string, string> _messageCache = new();

        public static string GetMessage(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            string key = $"{type.Name}.{name}";

            if (_messageCache.TryGetValue(key, out string message))
            {
                return message;
            }

            FieldInfo field = type.GetField(name);
            MessageAttribute attribute = (MessageAttribute)Attribute.GetCustomAttribute(field, typeof(MessageAttribute));

            if (attribute != null)
            {
                message = attribute.Message;
            }
            else
            {
                message = name;
            }

            _messageCache[key] = message;

            return message;
        }
    }
}
