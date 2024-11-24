using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class CustomRealtimePrefabLoadDelegate : MonoBehaviour, IRealtimePrefabLoadDelegate
{
    [SerializeField]
    private List<GameObject> gameObjectsToLoad = new List<GameObject>();

    public GameObject LoadRealtimePrefab(RealtimePrefabMetadata prefabMetadata)
    {
        GameObject realtimePrefab = null;

        Debug.Log("Trying to load: " + prefabMetadata.prefabName);
        foreach (GameObject g in gameObjectsToLoad)
        {
            if(g.name.Equals(prefabMetadata.prefabName))
            {
                realtimePrefab = g;
            }
#if UNITY_WEBGL
            //Load generic disc for client
            else if(prefabMetadata.prefabName.Contains("Vinyl") && g.name.Equals("VinylClient"))
            {
                realtimePrefab = g;
            }
#endif
        }

        return realtimePrefab;
    }
}