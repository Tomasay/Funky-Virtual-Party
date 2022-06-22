using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballObjectSyncer : ObjectSyncer
{
    [SerializeField] GameObject fireballMesh;
    [SerializeField] ParticleSystem explosion;

    private bool lastActiveSent; //Value of isActive last sent to clients

    public class FireballObjectData : ObjectData
    {
        public Vector3 scale;
        public bool isActive;
    }

    FireballObjectData currentFireballData;

    protected override void Awake()
    {
        currentFireballData = new FireballObjectData();
        currentFireballData.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string>("ObjectDataToClient", ReceiveData);
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/UpdatesPerSecond);
#endif
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("FireballExplosionEvent"))
        {
            explosion.Play();
        }
    }

#if !UNITY_WEBGL
    protected override void SendData()
    {
        Fireball f = GetComponent<Fireball>();
        if (f.fireball.activeSelf || lastActiveSent == true) //Only send data if fireball is active, make sure it is marked inactive on client first
        {
            //Position
            currentFireballData.Position = transform.position;

            //Rotation
            currentFireballData.Rotation = transform.rotation;

            //Fireball variables
            currentFireballData.isActive = lastActiveSent = f.fireball.activeSelf;
            currentFireballData.scale = f.fireball.transform.localScale;

            //Send Data
            string json = JsonUtility.ToJson(currentFireballData);
            if (ClientManager.instance)
            {
                ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", json);
            }
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
        fireballMesh.transform.localScale = data.scale;
    }
}