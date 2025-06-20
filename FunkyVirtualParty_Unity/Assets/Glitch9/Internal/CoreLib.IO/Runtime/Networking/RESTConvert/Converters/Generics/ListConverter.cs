using System;
using System.Collections.Generic;

namespace Glitch9.IO.Networking
{
    public class ListConverter<T> : RESTConveter<List<T>>
    {
        public override List<T> ToLocalFormat(string propertyName, object propertyValue)
        {
            if (propertyValue is not List<object> cloudList || cloudList.Count == 0)
                return new List<T>();

            List<T> list = new();
            Type elementType = typeof(T);

            foreach (object item in cloudList)
            {
                T value = (T)RESTConvert.ToLocalFormat(elementType, propertyName, item);
                list.Add(value);
            }

            return list;
        }

        public override object ToCloudFormat(List<T> propertyValue)
        {
            List<object> cloudList = new();

            foreach (T element in propertyValue)
            {
                object value = RESTConvert.ToCloudFormat(element.GetType(), element);
                cloudList.Add(value);
            }

            return cloudList;
        }
    }
}