using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncer : MonoBehaviour
{
    public class ObjectData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public static int objectIndex;
        public int objectID;

        public void Awake()
        {
            objectIndex++;
            objectID = objectIndex;
        }
    }

    ObjectData currentData;


    protected virtual void Awake()
    {
        currentData = new ObjectData();
        currentData.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string>("ObjectDataToClient", ReceiveData);
#endif

    }


    protected virtual void Update()
    {
#if !UNITY_WEBGL
        //Position
        currentData.Position = transform.position;

        //Rotation
        currentData.Rotation = transform.rotation;

        string json = JsonUtility.ToJson(currentData);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", json);
        }
#endif
    }


    public void ReceiveData(string json)
    {
        ApplyNewData(JsonUtility.FromJson<ObjectData>(json));
    }

    protected void ApplyNewData(ObjectData data)
    {
        if(data.objectID != currentData.objectID)
        {
            return;
        }

        //Position
        transform.position = data.Position;

        //Rotation
        transform.rotation = data.Rotation;
    }
}