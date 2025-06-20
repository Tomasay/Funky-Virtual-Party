using System.Collections.Generic;
using System.Text;
using Glitch9.IO.Json.Schema;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class UnityObjectProfile
    {
        [JsonSchemaProperty("type", Description = "The type of the currently selected Unity object", Required = true)]
        public string Type { get; set; }

        [JsonSchemaProperty("name", Description = "The name of the currently selected Unity object", Required = true)]
        public string Name { get; set; }

        [JsonSchemaProperty("components", Description = "The components this object has, if any", Required = false)]
        public string[] Components { get; set; }


        internal bool IsNull { get; private set; } = false;
        internal int InstanceID { get; private set; }

        public UnityObjectProfile(UnityEngine.Object selected)
        {
            if (selected is GameObject go && go.scene.name != null)
            {
                InstanceID = go.GetInstanceID();
                Type = selected.GetType().Name;
                Name = selected.name;

                if (selected is GameObject gameObject)
                {
                    List<string> components = new();
                    foreach (Component component in gameObject.GetComponents<Component>())
                    {
                        components.Add(component.GetType().Name);
                    }
                    Components = components.ToArray();
                }
            }
            else
            {
                // If the GameObject is not in a scene, we treat it as null
                IsNull = true;
            }
        }

        public static UnityObjectProfile Null => new(null)
        {
            Type = "null",
            Name = "null",
            Components = null
        };

        public override string ToString()
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.AppendLine($"{Type}(name=\"{Name}\")");
                if (!Components.IsNullOrEmpty()) sb.AppendLine($"Components: [{string.Join(", ", Components)}]");
                return sb.ToString();
            }
        }
    }
}