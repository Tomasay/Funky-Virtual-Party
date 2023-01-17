using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class ObjectSyncer : MonoBehaviour
{
    [Tooltip("ID of object to track in scene. Must be the same ID on mobile vs VR scenes")]
    public byte objectID;

    [Tooltip("How often data is sent to be synced. Set to 0 for non dynamic syncing (improves performance)")]
    public float UpdatesPerSecond = 10;

    [Serializable]
    public class ObjectData
    {
        public SerializedVector3 Position;
        public SerializedQuaternion Rotation;
        public byte objectID;

        public void Init(byte ID)
        {
            objectID = ID;
        }
    }

    protected ObjectData currentData;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(currentData.Position);
                writer.Write(currentData.Rotation);
                writer.Write(currentData.objectID);

            }
            return m.ToArray();
        }
    }

    public static ObjectData Deserialize(byte[] data)
    {
        ObjectData result = new ObjectData();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.Position = reader.ReadVector3();
                result.Rotation = reader.ReadQuaternion();
                result.objectID = reader.ReadByte();
            }
        }
        return result;
    }


    protected virtual void Awake()
    {
        currentData = new ObjectData();
        currentData.Init(objectID);

#if UNITY_WEBGL
        if(ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.On<byte[]>("ObjectDataToClient", ReceiveData);
#endif

#if !UNITY_WEBGL
        if (UpdatesPerSecond > 0)
        {
            InvokeRepeating("SendData", 0, 1 / UpdatesPerSecond);
        }
#endif

    }

    protected void OnDestroy()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("ObjectDataToClient");
#endif
    }

#if !UNITY_WEBGL
    protected virtual void SendData()
    {
        //Position
        currentData.Position = transform.position;

        //Rotation
        currentData.Rotation = transform.rotation;

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", Serialize());
        }
    }
#endif

    public virtual void ReceiveData(byte[] arrBytes)
    {
        ApplyNewData(Deserialize(arrBytes));
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