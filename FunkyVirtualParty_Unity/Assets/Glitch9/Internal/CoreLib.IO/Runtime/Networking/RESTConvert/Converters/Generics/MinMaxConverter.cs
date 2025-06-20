using System;

namespace Glitch9.IO.Networking
{
    public class MinMaxConverter<T> : RESTConveter<MinMax<T>> where T : IComparable<T>
    {
        public override MinMax<T> ToLocalFormat(string propertyName, object propertyValue)
        {
            T convertedValue = (T)RESTConvertUtil.ConvertIComparable(typeof(T), propertyValue);
            return new MinMax<T> { Value = convertedValue };
        }

        public override object ToCloudFormat(MinMax<T> propertyValue)
        {
            return propertyValue.Value;
        }
    }
}