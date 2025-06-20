using System;
using Glitch9.Encryption;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    [Serializable]
    public class ApiKey
    {
        public static implicit operator string(ApiKey apiKey) => apiKey.GetKey();
        [SerializeField] private string key;
        [SerializeField] private bool encrypt;
        [SerializeField] private bool visible = true;

        internal ApiKey(string key, bool encrypt = false, bool visible = true)
        {
            this.key = key;
            this.encrypt = encrypt;
            this.visible = visible;
        }

        public bool HasValue => !string.IsNullOrEmpty(key);
        public string GetKey()
        {
            // string result = encrypt ? Encrypter.DecryptString(key) : key;
            // if (string.IsNullOrEmpty(result)) throw new ArgumentException($"This API key is invalid. Please set a valid API key in the user preferences. (Edit > Preferences > AIDevKit)");
            // return result;

            if (string.IsNullOrEmpty(key)) return string.Empty;

            string result = encrypt ? Encrypter.DecryptString(key) : key;

            if (string.IsNullOrEmpty(result))
                throw new ArgumentException("This API key is invalid. Please set a valid API key in the user preferences. (Edit > Preferences > AIDevKit)");

            return result;
        }
    }
}