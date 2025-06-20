using System;
using System.Collections.Generic;

namespace Glitch9.IO.Networking
{
    public class ArrayConverter : RESTConveter<Array>
    {
        public override Array ToLocalFormat(string propertyName, object propertyValue)
        {
            if (propertyValue is not List<object> cloudList)
                return Array.Empty<object>();

            Type elementType = GetElementType(propertyValue);
            Array array = Array.CreateInstance(elementType, cloudList.Count);

            for (int i = 0; i < cloudList.Count; i++)
            {
                object value = RESTConvert.ToLocalFormat(elementType, propertyName, cloudList[i]);
                array.SetValue(value, i);
            }

            return array;
        }

        public override object ToCloudFormat(Array propertyValue)
        {
            List<object> cloudList = new();

            foreach (object element in propertyValue)
            {
                object value = RESTConvert.ToCloudFormat(element.GetType(), element);
                cloudList.Add(value);
            }

            return cloudList;
        }

        private Type GetElementType(object propertyValue)
        {
            // If propertyValue is a List<object>, we need to inspect the first non-null element
            if (propertyValue is List<object> list && list.Count > 0)
            {
                foreach (object item in list)
                {
                    if (item != null)
                    {
                        return item.GetType();
                    }
                }
            }
            return typeof(object);
        }
    }
}