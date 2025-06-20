using Newtonsoft.Json;
using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public class EPrefs<T>
    {
        public T Value
        {
            get
            {
                InitializeIfNeeded();
                _cache ??= Load();
                return _cache;
            }

            set
            {
                _cache = value;
                Save();
            }
        }

        private readonly string _prefsKey;
        private T _cache;
        private readonly T _defaultValue;
        private bool _initialized;

        private void InitializeIfNeeded()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                if (!EditorPrefs.HasKey(_prefsKey))
                {
                    _cache = _defaultValue;
                    Save();
                    return;
                }

                T savedValue = Load();

                if (savedValue == null)
                {
                    _cache = _defaultValue;
                    Save();
                }
                else
                {
                    _cache = savedValue;
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                _cache = _defaultValue;
                Save();
            }
        }

        public EPrefs(string prefsKey, T defaultValue)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;
            _defaultValue = defaultValue;
        }

        private T Load()
        {
            if (!EditorPrefs.HasKey(_prefsKey)) return default;

            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(EditorPrefs.GetInt(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(uint))
            {
                return (T)Convert.ChangeType(EditorPrefsUtil.GetUInt(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(long))
            {
                return (T)Convert.ChangeType(EditorPrefsUtil.GetLong(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(EditorPrefs.GetFloat(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(EditorPrefs.GetString(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(EditorPrefs.GetBool(_prefsKey), typeof(T));
            }

            if (typeof(T) == typeof(UnixTime))
            {
                UnixTime unixTime = new(EditorPrefs.GetInt(_prefsKey));
                return (T)(object)unixTime;
            }

            if (typeof(T) == typeof(Vector2))
            {
                float x = EditorPrefs.GetFloat(_prefsKey + ".x");
                float y = EditorPrefs.GetFloat(_prefsKey + ".y");
                return (T)(object)new Vector2(x, y);
            }

            if (typeof(T) == typeof(Vector3))
            {
                float x = EditorPrefs.GetFloat(_prefsKey + ".x");
                float y = EditorPrefs.GetFloat(_prefsKey + ".y");
                float z = EditorPrefs.GetFloat(_prefsKey + ".z");
                return (T)(object)new Vector3(x, y, z);
            }

            if (typeof(T) == typeof(Quaternion))
            {
                float x = EditorPrefs.GetFloat(_prefsKey + ".x");
                float y = EditorPrefs.GetFloat(_prefsKey + ".y");
                float z = EditorPrefs.GetFloat(_prefsKey + ".z");
                float w = EditorPrefs.GetFloat(_prefsKey + ".w");
                return (T)(object)new Quaternion(x, y, z, w);
            }

            if (typeof(T).IsEnum)
            {
                int storedValue = EditorPrefs.GetInt(_prefsKey);
                return (T)Enum.ToObject(typeof(T), storedValue); // possible crash?
            }

            try
            {
                string json = EditorPrefs.GetString(_prefsKey, "{}");
                return JsonConvert.DeserializeObject<T>(json, JsonConfig.DefaultSerializerSettings);
            }
            catch (Exception e)
            {
                EditorPrefsUtil.HandleFailedDeserialization(_prefsKey, typeof(T).Name, e);
                return default;
            }
        }


        public void Save()
        {
            if (Value == null)
            {
                EditorPrefs.DeleteKey(_prefsKey);
                return;
            }

            if (Value is int intValue)
            {
                EditorPrefs.SetInt(_prefsKey, intValue);
            }
            else if (Value is uint uintValue)
            {
                EditorPrefsUtil.SetUInt(_prefsKey, uintValue);
            }
            else if (Value is long longValue)
            {
                EditorPrefsUtil.SetLong(_prefsKey, longValue);
            }
            else if (Value is float floatValue)
            {
                EditorPrefs.SetFloat(_prefsKey, floatValue);
            }
            else if (Value is string stringValue)
            {
                EditorPrefs.SetString(_prefsKey, stringValue);
            }
            else if (Value is bool boolValue)
            {
                EditorPrefs.SetBool(_prefsKey, boolValue);
            }
            else if (Value is UnixTime unixTimeValue)
            {
                EditorPrefs.SetInt(_prefsKey, (int)unixTimeValue);
            }
            else if (Value is Vector2 vector2)
            {
                EditorPrefs.SetFloat(_prefsKey + ".x", vector2.x);
                EditorPrefs.SetFloat(_prefsKey + ".y", vector2.y);
            }
            else if (Value is Vector3 vector3)
            {
                EditorPrefs.SetFloat(_prefsKey + ".x", vector3.x);
                EditorPrefs.SetFloat(_prefsKey + ".y", vector3.y);
                EditorPrefs.SetFloat(_prefsKey + ".z", vector3.z);
            }
            else if (Value is Quaternion quaternion)
            {
                EditorPrefs.SetFloat(_prefsKey + ".x", quaternion.x);
                EditorPrefs.SetFloat(_prefsKey + ".y", quaternion.y);
                EditorPrefs.SetFloat(_prefsKey + ".z", quaternion.z);
                EditorPrefs.SetFloat(_prefsKey + ".w", quaternion.w);
            }
            else if (Value.GetType().IsEnum)
            {
                // Safely convert enum to its underlying int value
                int enumValue = Convert.ToInt32(Value);
                EditorPrefs.SetInt(_prefsKey, enumValue);
            }
            else
            {
                string json = JsonConvert.SerializeObject(Value, JsonConfig.DefaultSerializerSettings);
                //Debug.Log($"Saving {Value} to {_prefsKey}");
                EditorPrefs.SetString(_prefsKey, json);
            }
        }
    }

    public static class EditorPrefsExtensions
    {
        public static bool HasValue<T>(this EPrefs<T> prefs)
        {
            return prefs != null && prefs.Value != null;
        }
    }
}