namespace Glitch9.AIDevKit.Editor
{
    internal static class AIModuleUtil
    {
        internal static string GetModuleGameObjectName<T>() where T : UnityEngine.MonoBehaviour
        {
            // Get the type name of the module and format it to be more readable
            string typeName = typeof(T).Name;
            return $"{typeName} Module";
        }
    }
}