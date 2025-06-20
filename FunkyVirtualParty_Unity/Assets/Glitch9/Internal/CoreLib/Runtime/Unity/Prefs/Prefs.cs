using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9
{
    public class Prefs<T>
    {
        public static implicit operator T(Prefs<T> prefs) => prefs.Value;
        private readonly string _prefsKey;
        private T _cache;
        private readonly T _defaultValue;

        public T Value
        {
            get
            {
                _cache ??= Load();
                return _cache;
            }
            set
            {
                _cache = value;
                Save();
            }
        }

        public Prefs(string prefsKey, T defaultValue)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;
            _defaultValue = defaultValue;
            ProcessLoading();
        }

        private void SaveDefaultValue()
        {
            _cache = _defaultValue;
            Save();
        }

        private void ProcessLoading()
        {
            if (!PlayerPrefs.HasKey(_prefsKey))
            {
                SaveDefaultValue();
            }
            else
            {
                try
                {
                    _cache = Load() ?? _defaultValue;
                }
                catch (Exception e)
                {
                    LogService.Exception(e);
                    PlayerPrefs.DeleteKey(_prefsKey);
                    SaveDefaultValue();
                }
            }
        }


        private T Load()
        {
            try
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)Convert.ChangeType(PlayerPrefs.GetInt(_prefsKey), typeof(T));
                }

                if (typeof(T) == typeof(float))
                {
                    return (T)Convert.ChangeType(PlayerPrefs.GetFloat(_prefsKey), typeof(T));
                }

                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(PlayerPrefs.GetString(_prefsKey), typeof(T));
                }

                if (typeof(T) == typeof(bool))
                {
                    return (T)Convert.ChangeType(PlayerPrefs.GetInt(_prefsKey) == 1, typeof(T));
                }

                if (typeof(T) == typeof(UnixTime))
                {
                    UnixTime unixTime = new(PlayerPrefs.GetInt(_prefsKey));
                    return (T)(object)unixTime;
                }

                if (typeof(T) == typeof(Vector2))
                {
                    float x = PlayerPrefs.GetFloat(_prefsKey + ".x");
                    float y = PlayerPrefs.GetFloat(_prefsKey + ".y");
                    return (T)(object)new Vector2(x, y);
                }

                if (typeof(T) == typeof(Vector3))
                {
                    float x = PlayerPrefs.GetFloat(_prefsKey + ".x");
                    float y = PlayerPrefs.GetFloat(_prefsKey + ".y");
                    float z = PlayerPrefs.GetFloat(_prefsKey + ".z");
                    return (T)(object)new Vector3(x, y, z);
                }

                if (typeof(T) == typeof(Quaternion))
                {
                    float x = PlayerPrefs.GetFloat(_prefsKey + ".x");
                    float y = PlayerPrefs.GetFloat(_prefsKey + ".y");
                    float z = PlayerPrefs.GetFloat(_prefsKey + ".z");
                    float w = PlayerPrefs.GetFloat(_prefsKey + ".w");
                    return (T)(object)new Quaternion(x, y, z, w);
                }

                if (typeof(T).IsEnum)
                {
                    return (T)Enum.ToObject(typeof(T), PlayerPrefs.GetInt(_prefsKey));
                }

                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    string json = PlayerPrefs.GetString(_prefsKey, null);
                    if (string.IsNullOrEmpty(json)) return Activator.CreateInstance<T>();

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(json, JsonConfig.DefaultSerializerSettings);
                    }
                    catch (Exception e)
                    {
                        PrefsUtil.HandleFailedDeserialization(_prefsKey, typeof(T).Name, e);
                        return default;
                    }
                }

                string jsonData = PlayerPrefs.GetString(_prefsKey, "{}");
                return JsonConvert.DeserializeObject<T>(jsonData, JsonConfig.DefaultSerializerSettings);
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                return default;
            }
        }


        private void Save()
        {
            try
            {
                if (Value is int intValue)
                {
                    PlayerPrefs.SetInt(_prefsKey, intValue);
                }
                else if (Value is float floatValue)
                {
                    PlayerPrefs.SetFloat(_prefsKey, floatValue);
                }
                else if (Value is string stringValue)
                {
                    PlayerPrefs.SetString(_prefsKey, stringValue);
                }
                else if (Value is bool boolValue)
                {
                    PlayerPrefs.SetInt(_prefsKey, boolValue ? 1 : 0);
                }
                else if (Value is UnixTime unixTimeValue)
                {
                    PlayerPrefs.SetInt(_prefsKey, (int)unixTimeValue);
                }
                else if (Value is Vector2 vector2)
                {
                    PlayerPrefs.SetFloat(_prefsKey + ".x", vector2.x);
                    PlayerPrefs.SetFloat(_prefsKey + ".y", vector2.y);
                }
                else if (Value is Vector3 vector3)
                {
                    PlayerPrefs.SetFloat(_prefsKey + ".x", vector3.x);
                    PlayerPrefs.SetFloat(_prefsKey + ".y", vector3.y);
                    PlayerPrefs.SetFloat(_prefsKey + ".z", vector3.z);
                }
                else if (Value is Quaternion quaternion)
                {
                    PlayerPrefs.SetFloat(_prefsKey + ".x", quaternion.x);
                    PlayerPrefs.SetFloat(_prefsKey + ".y", quaternion.y);
                    PlayerPrefs.SetFloat(_prefsKey + ".z", quaternion.z);
                    PlayerPrefs.SetFloat(_prefsKey + ".w", quaternion.w);
                }
                else if (Value.GetType().IsEnum)
                {
                    PlayerPrefs.SetInt(_prefsKey, Convert.ToInt32(Value));
                }
                else
                {
                    string json = JsonConvert.SerializeObject(Value, JsonConfig.DefaultSerializerSettings);
                    PlayerPrefs.SetString(_prefsKey, json);
                }

                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                LogService.Exception(e);
            }
        }

        public void Clear()
        {
            Value = default;
            PlayerPrefs.DeleteKey(_prefsKey);
        }
    }
}
