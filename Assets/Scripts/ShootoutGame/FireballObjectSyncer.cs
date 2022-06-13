using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballObjectSyncer : ObjectSyncer
{
    [SerializeField] GameObject fireballMesh;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Shootout_DestructibleTerrian terrain;

    public class FireballObjectData : ObjectData
    {
        public float timeDropped;
        public bool isActive;

        public bool explodeEvent;
    }

    FireballObjectData currentFireballData;

    protected override void Awake()
    {
        currentFireballData = new FireballObjectData();
        currentFireballData.Awake();

#if UNITY_WEBGL
        Debug.Log("CLIENT MANAGER IS HERE: " + (ClientManagerWeb.instance != null));
        ClientManagerWeb.instance.Manager.Socket.On<string>("ObjectDataToClient", ReceiveData);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/UpdatesPerSecond);
#endif
    }

#if !UNITY_WEBGL
    protected override void SendData()
    {
        //Position
        currentFireballData.Position = transform.position;

        //Rotation
        currentFireballData.Rotation = transform.rotation;


        //FireballObjectData fireballData = currentData as FireballObjectData;
        Fireball f = GetComponent<Fireball>();
        currentFireballData.timeDropped = f.timeDropped;
        currentFireballData.isActive = f.fireball.activeSelf;

        currentFireballData.explodeEvent = f.explodeEvent;
        if(f.explodeEvent)
        {
            f.explodeEvent = false;
        }

        string json = JsonUtility.ToJson(currentFireballData);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", json);
        }
    }
#endif

    public override void ReceiveData(string json)
    {
        ApplyNewFireballData(JsonUtility.FromJson<FireballObjectData>(json));
    }

    protected void ApplyNewFireballData(FireballObjectData data)
    {
        if (data.objectID != currentFireballData.objectID)
        {
            return;
        }

        //Position
        transform.position = data.Position;

        //Rotation
        transform.rotation = data.Rotation;

        //Fireball
        fireballMesh.SetActive(data.isActive);
        
        if(data.explodeEvent)
        {
            explosion.Play();
        }
    }
}