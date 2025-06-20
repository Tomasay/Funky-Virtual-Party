using UnityEngine;

namespace Glitch9.IO.Networking
{
    public class ColorConverter : RESTConveter<Color>
    {
        private static readonly Color kDefaultColor = ExColor.lavender;

        public override Color ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = RESTConvertUtil.SafeConvertToString(propertyValue);
            if (stringValue == null) return kDefaultColor;
            if (stringValue.TryParseColor(out Color color))
            {
                return color;
            }
            else
            {
                LogService.Warning($"string '{stringValue}'를 Color로 변환할 수 없습니다. 기본값으로 lavender를 반환합니다.");
                return kDefaultColor;
            }
        }

        public override object ToCloudFormat(Color propertyValue)
        {
            return propertyValue.ToHex();
        }
    }
}