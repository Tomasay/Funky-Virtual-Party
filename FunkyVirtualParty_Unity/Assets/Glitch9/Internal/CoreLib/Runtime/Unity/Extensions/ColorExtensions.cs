using UnityEngine;

namespace Glitch9
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color);  // ex : #FFFFFF
        }

        /// <summary>
        /// Convert a hex string into a color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color ToColor(this string hex)
        {
            return hex.TryParseColor(out Color color) ? color : Color.white;
        }

        public static bool TryParseColor(this string hex, out Color color)
        {
            try
            {
                hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
                hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
                byte a = 255;//assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0, 2), global::System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), global::System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), global::System.Globalization.NumberStyles.HexNumber);
                //Only use alpha if the string has enough characters
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), global::System.Globalization.NumberStyles.HexNumber);
                }
                color = new Color32(r, g, b, a);
                return true;
            }
            catch
            {
                color = Color.white;
                return false;
            }
        }
    }
}