using UnityEngine;

namespace Glitch9.AIDevKit.Demo
{
    public static class DemoStatusWidgetConfig
    {
        public static Color GetColor(DemoStatusType statusType)
        {
            return statusType switch
            {
                DemoStatusType.Unavailable => Color.gray,
                DemoStatusType.Positive => Color.green,
                DemoStatusType.Processing => Color.cyan,
                DemoStatusType.Warning => Color.yellow,
                DemoStatusType.Negative => Color.red,
                _ => Color.white,
            };
        }
    }
}