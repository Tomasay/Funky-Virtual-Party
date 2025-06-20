using System;

namespace Glitch9.IO.Networking
{
    public class TimeSpanConverter : RESTConveter<TimeSpan>
    {
        public override TimeSpan ToLocalFormat(string propertyName, object propertyValue)
        {
            if (propertyValue is double doubleValue)
            {
                return TimeSpan.FromMilliseconds(doubleValue);
            }

            LogService.Error($"Failed to parse TimeSpan: {propertyValue}");
            return TimeSpan.Zero;
        }

        public override object ToCloudFormat(TimeSpan propertyValue)
        {
            return propertyValue.TotalMilliseconds;
        }
    }
}