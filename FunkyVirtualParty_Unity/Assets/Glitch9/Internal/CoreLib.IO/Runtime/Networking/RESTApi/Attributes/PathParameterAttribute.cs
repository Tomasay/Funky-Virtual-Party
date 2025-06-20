using System;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PathParameterAttribute : PropertyAttribute
    {
        public string Name { get; protected set; }

        public PathParameterAttribute(string name)
        {
            Name = name;
        }
    }
}