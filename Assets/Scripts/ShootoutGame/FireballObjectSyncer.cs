using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballObjectSyncer : ObjectSyncer
{
    public class FireballObjectData : ObjectData
    {
        public bool hasExploded, isDropped;
        public float timeDropped;
        public bool isKinematic, useGravity;
        public bool isActive;
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
        InvokeRepeating("SendData", 0, sendRate);
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
        currentFireballData.hasExploded = f.hasExploded;
        currentFireballData.isDropped = f.isDropped;
        currentFireballData.timeDropped = f.timeDropped;
        currentFireballData.isKinematic = f.rb.isKinematic;
        currentFireballData.useGravity = f.rb.useGravity;
        currentFireballData.isActive = f.fireball.activeSelf;

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
        Fireball f = GetComponent<Fireball>();
        f.hasExploded = data.hasExploded;
        f.isDropped = data.isDropped;
        f.timeDropped = data.timeDropped;
        f.rb.isKinematic = data.isKinematic;
        f.rb.useGravity = data.useGravity;
        f.fireball.SetActive(data.isActive);
    }
}