using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class SceneReviewer
    {
        internal class SceneObjectInfo
        {
            public string name;
            public string tag;
            public string layer;
            public List<string> components;
            public List<string> issues;
        }

        internal class SceneReviewSummary
        {
            public List<string> issues = new();
            public List<SceneObjectInfo> objects = new();
        }

        internal static string GenerateSceneReviewJson()
        {
            var allObjects = UnityCompat.FindObjectsByType<GameObject>();
            var summary = new SceneReviewSummary();

            foreach (var obj in allObjects)
            {
                var issues = new List<string>();
                var components = obj.GetComponents<Component>()
                                    .Where(c => c != null)
                                    .Select(c => c.GetType().Name)
                                    .ToList();

                // Pattern-based issue detection
                if (components.Contains("Rigidbody") && obj.GetComponent<Collider>() == null)
                    issues.Add("Rigidbody without Collider");

                if (components.Contains("Animator") && obj.GetComponent<Animator>().runtimeAnimatorController == null)
                    issues.Add("Animator has no controller");

                if (components.Contains("Canvas"))
                {
                    var canvas = obj.GetComponent<Canvas>();
                    if (canvas.renderMode == RenderMode.WorldSpace)
                    {
                        int depth = GetHierarchyDepth(obj.transform);
                        if (depth > 5)
                            issues.Add("WorldSpace Canvas nested too deep");
                    }
                }

                if (!obj.activeInHierarchy &&
                    components.Any(c => c is "Button" or "InputField" or "TMP_Text" or "TextMeshProUGUI"))
                    issues.Add("Disabled interactive UI element");

                if (obj.name.Contains("(Clone)"))
                    issues.Add("Dynamically instantiated object");

                summary.objects.Add(new SceneObjectInfo
                {
                    name = obj.name,
                    tag = obj.tag,
                    layer = LayerMask.LayerToName(obj.layer),
                    components = components,
                    issues = issues
                });

                summary.issues.AddRange(issues);
            }

            // make empty issues list null
            foreach (var obj in summary.objects)
            {
                if (obj.issues.Count == 0)
                    obj.issues = null;
            }

            return JsonConvert.SerializeObject(summary, JsonConfig.DefaultSerializerSettings);
        }

        private static int GetHierarchyDepth(Transform transform)
        {
            int depth = 0;
            while (transform.parent != null)
            {
                depth++;
                transform = transform.parent;
            }
            return depth;
        }
    }
}