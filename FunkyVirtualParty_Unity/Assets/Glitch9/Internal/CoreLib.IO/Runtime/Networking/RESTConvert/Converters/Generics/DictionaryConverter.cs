using System;
using System.Collections;
using System.Collections.Generic;

namespace Glitch9.IO.Networking
{
    public class DictionaryConverter<TKey, TValue> : RESTConveter<Dictionary<TKey, TValue>>
    {
        public override Dictionary<TKey, TValue> ToLocalFormat(string propertyName, object propertyValue)
        {
            Dictionary<TKey, TValue> instance = new();
            if (propertyValue is not Dictionary<string, object> firestoreMap || firestoreMap.Count == 0) return instance;

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            foreach (KeyValuePair<string, object> element in firestoreMap)
            {
                string key = element.Key;
                object convertedKey = RESTConvertUtil.ConvertKey(keyType, key);

                object value = RESTConvert.ToLocalFormat(valueType, propertyName, element.Value);
                if (value == null) continue;

                instance.Add((TKey)convertedKey, (TValue)value);
            }

            return instance;
        }


        public override object ToCloudFormat(Dictionary<TKey, TValue> propertyValue)
        {
            Dictionary<string, object> firestoreMap = new();

            // propertyValue가 실제로 Dictionary인지 확인
            if (propertyValue is IDictionary propertyDict)
            {
                Type keyType = typeof(TKey);
                Type valueType = typeof(TValue);

                // 딕셔너리의 각 항목에 대해 반복
                foreach (DictionaryEntry element in propertyDict)
                {
                    string key = element.Key.ToString();
                    object value = RESTConvert.ToCloudFormat(valueType, element.Value);

                    firestoreMap.Add(key, value);
                }
            }

            return firestoreMap;
        }
    }
}
