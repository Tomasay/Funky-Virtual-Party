using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectSyncer : MonoBehaviour
{
    //How often data is sent to be synced
    public float UpdatesPerSecond = 10;

    [Serializable]
    public class ObjectData
    {
        public SerializedVector3 Position;
        public SerializedQuaternion Rotation;
        public static int objectIndex;
        public int objectID;

        public void Awake()
        {
            objectIndex++;
            objectID = objectIndex;
        }
    }

    protected ObjectData currentData;


    protected virtual void Awake()
    {
        currentData = new ObjectData();
        currentData.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<byte[]>("ObjectDataToClient", ReceiveData);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/ UpdatesPerSecond);
#endif

    }

#if !UNITY_WEBGL
    protected virtual void SendData()
    {
        //Position
        currentData.Position = transform.position;

        //Rotation
        currentData.Rotation = transform.rotation;

        //string json = JsonUtility.ToJson(currentData);
        byte[] bytes = ByteArrayConverter.ObjectToByteArray<ObjectData>(currentData);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", bytes);
        }
    }
#endif


    public virtual void ReceiveData(string json)
    {
        ApplyNewData(JsonUtility.FromJson<ObjectData>(json));
    }

    public void ReceiveData(byte[] arrBytes)
    {
        ApplyNewData(ByteArrayConverter.ByteArrayToObject<ObjectData>(arrBytes));
    }

    protected virtual void ApplyNewData<T>(T data) where T : ObjectData
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