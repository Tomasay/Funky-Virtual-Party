using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectSyncer;

public class TerrainObjectSyncer : MonoBehaviour
{
    //How often data is sent to be synced
    public float UpdatesPerSecond = 5;

    public class TerrainObjectData : ObjectData
    {
        public Vector3 explodePos;
        public float fireballScale;

        public bool explodeEvent;
    }

    TerrainObjectData currentTerrainData;

    void Awake()
    {
        currentTerrainData = new TerrainObjectData();
        currentTerrainData.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string>("ObjectDataToClient", ReceiveData);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1 / UpdatesPerSecond);
#endif
    }

#if !UNITY_WEBGL
    protected void SendData()
    {
        Shootout_DestructibleTerrian t = GetComponent<Shootout_DestructibleTerrian>();
        currentTerrainData.explodePos = t.ExplodePos;
        currentTerrainData.fireballScale = t.FireballScale;
        currentTerrainData.explodeEvent = t.ExplodeEvent;
        if (t.ExplodeEvent)
        {
            t.ExplodeEvent = false;
        }

        string json = JsonUtility.ToJson(currentTerrainData);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", json);
        }
    }
#endif

    public  void ReceiveData(string json)
    {
        ApplyNewTerrainData(JsonUtility.FromJson<TerrainObjectData>(json));
    }

    protected void ApplyNewTerrainData(TerrainObjectData data)
    {
        if (data.objectID != currentTerrainData.objectID)
        {
            return;
        }

        currentTerrainData.explodePos = data.explodePos;
        currentTerrainData.fireballScale = data.fireballScale;

        if (data.explodeEvent)
        {
            Debug.Log("Explode event!");
            GetComponent<Shootout_DestructibleTerrian>().Explosion(data.fireballScale, data.explodePos);
        }
    }
}
