using Glitch9.IO.Networking.RESTApi;
using System;

namespace Glitch9.IO.Networking
{
    public class EnumConverter<TEnum> : RESTConveter<TEnum> where TEnum : Enum
    {
        public override TEnum ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = RESTConvertUtil.SafeConvertToString(propertyValue);
            if (stringValue == null) return default;

            if (ApiEnumUtil.TryParse(typeof(TEnum), stringValue, out object result, true))
            {
                return (TEnum)result;
            }
            else
            {
                LogService.Error($"Failed to parse enum: {stringValue}");
                return default;
            }
        }

        public override object ToCloudFormat(TEnum propertyValue)
        {
            return propertyValue.ToApiValue();
        }
    }
}