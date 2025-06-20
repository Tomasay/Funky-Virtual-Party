using UnityEngine;

namespace Glitch9.ScriptableObjects
{
    public abstract class ScriptableResource<TSelf> : ScriptableObject
        where TSelf : ScriptableResource<TSelf>
    {
        private static TSelf _instance;
        public static TSelf Instance => CreateInstance();

        private static TSelf CreateInstance()
        {
            if (_instance == null)
            {
                AssetPathAttribute att = AttributeCache<AssetPathAttribute>.Get<TSelf>();
                string path = att == null ? "Resources" : att.Path;
                _instance = ScriptableObjectUtil.LoadSingleton<TSelf>(dirPath: path, create: true);
            }
            return _instance;
        }
    }
}