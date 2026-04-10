using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Kamgam.ExcludeFromBuild
{
    [AddComponentMenu("Exclude From Build")]
    public class ExcludeFromBuildComponent : MonoBehaviour
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        public string GUID;

        public const string PREFAB_ROOT_ERROR =
            "You can not exclude the ROOT game object of a prefab from within the prefab. Please add the prefab file to your exclusion list and remove the GameObject exclusion on this object or change it to a component only exclusion. This will be ignored during exclusion.";

        [System.NonSerialized]
        public const string SessionActiveGroupIdKey = "Kamgam.EFB.ActiveGroupId";

        [Tooltip("Exclude this whole GameObject?\nDisable to exclude only certain components.")]
        public bool GameObject = true;

        [Tooltip("Exclude these components from the build?")]
        public List<Component> Components;

        [Tooltip("Active for all groups?\nDisable to customize for which groups this component will be taken into account.")]
        public bool AllGroups = true;

        [Tooltip("The exclusions will only be executed if the currently active group is in the list.")]
        public List<int> GroupIds = new List<int>();

        [Tooltip("Simulate the removal in the PlayMode?")]
        public bool TestInPlayMode = false;

        public bool HasComponentsToExclude => Components != null && Components.Count > 0;

        public void Execute(int activeGroupId = -1)
        {
            Execute(activeGroupId, null);
        }

        public bool CheckConditions(int activeGroupId)
        {
            // Don't execute if activeGroup is unknown.
            if (activeGroupId < 0)
            {
                Debug.LogWarning("ExcludeFromBuildComponent: Active group of '" + this.name + "' is unknown. Will not exclude.");
                return false;
            }

            // Don't execute if the group does not match.
            if (!AllGroups && activeGroupId >= 0 && !GroupIds.Contains(activeGroupId))
                return false;

            return true;
        }
        
        public void Execute(int activeGroupId, GameObject editScopeRoot)
        {
            if (!CheckConditions(activeGroupId))
                return;

            if (GameObject)
            {
                if (this.gameObject != null)
                {
                    GameObject prefabStageRoot = null;
                    var stage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (stage != null)
                        prefabStageRoot = stage.prefabContentsRoot;
                    if ((editScopeRoot != null && editScopeRoot == gameObject) 
                        || PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject)
                        || IsRootOfPrefabAsset(gameObject)
                        || (prefabStageRoot != null && gameObject == prefabStageRoot))
                    {
                        Debug.LogError("Ignoring Exclusion on '" + gameObject.name + "'. " + PREFAB_ROOT_ERROR);
                    }
                    else
                    {
                        DestroyImmediate(this.gameObject, allowDestroyingAssets: true);
                    }
                }
            }
            else if (Components != null && Components.Count > 0)
            {
                foreach (var comp in Components)
                {
                    if (comp != null)
                        DestroyImmediate(comp);
                }
                DestroyImmediate(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        public string GetPath()
        {
            var path = new System.Text.StringBuilder();

            var parent = transform.parent;
            path.Append(gameObject.name);
            while (parent != null)
            {
                path.Insert(0, "/");
                path.Insert(0, parent.name);
                parent = parent.parent;
            }

            return path.ToString();
        }

        public void Awake()
        {
            createNewGUIDIfNeeded();

            if (EditorApplication.isPlaying && TestInPlayMode)
            {
                int activeGroupId = SessionState.GetInt(SessionActiveGroupIdKey, -1);
                Execute(activeGroupId);
            }
        }

        public void Reset()
        {
            createNewGUIDIfNeeded();
            AllGroups = true;
        }

        protected void createNewGUIDIfNeeded()
        {
            if (GUID == null)
            {
                GUID = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (GUID == null)
            {
                GUID = System.Guid.NewGuid().ToString();
            }
        }
        
        public bool IsRootOfPrefabAsset(GameObject go)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            
            if (prefabStage == null)
                return false;

            return go == prefabStage.prefabContentsRoot;
        }
#endif
    }
}
