using System;

namespace Glitch9
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AssetPathAttribute : Attribute
    {
        public string Path { get; private set; }
        public AssetPathAttribute(string path) => Path = path;
    }
}