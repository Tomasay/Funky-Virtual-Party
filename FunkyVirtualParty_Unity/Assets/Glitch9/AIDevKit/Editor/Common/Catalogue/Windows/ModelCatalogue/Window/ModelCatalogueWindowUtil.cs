using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ModelCatalogueWindowUtil
    {
        internal static Dictionary<ModelFeature, (string, Texture)> CreateCapabilityMap()
        {
            Dictionary<ModelFeature, (string, Texture)> dict = new();

            foreach (ModelFeature capability in System.Enum.GetValues(typeof(ModelFeature)))
            {
                if (capability == ModelFeature.None) continue;

                string name = capability.GetInspectorName().Replace(" ", "\n");
                Texture icon = AIDevKitGUIUtility.GetModelFeatureIcon(capability);

                dict.Add(capability, (name, icon));
            }

            return dict;
        }
    }
}