using UnityEngine;

namespace Glitch9
{
    public static class RangeCheck
    {
        public static float ClampWithWarning(float value, float min, float max, string fieldName)
        {
            if (value < min || value > max)
            {
                LogService.Warning($"{fieldName} must be between {min} and {max}. Input value: {value}");
                return Mathf.Clamp(value, min, max);
            }
            return value;
        }

        public static int ClampWithWarning(int value, int min, int max, string fieldName)
        {
            if (value < min || value > max)
            {
                LogService.Warning($"{fieldName} must be between {min} and {max}. Input value: {value}");
                return Mathf.Clamp(value, min, max);
            }
            return value;
        }
    }
}