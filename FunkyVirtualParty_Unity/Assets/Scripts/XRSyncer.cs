using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class XRSyncer : MonoBehaviour
{
    //How often data is sent to be synced
    public float UpdatesPerSecond = 10;

    //How fast the client version XR player lerps to the correct positions
    float TransformLerpSpeed = 50;

    //References
    [SerializeField] GameObject head;
    [SerializeField] GameObject leftHand, rightHand;
    [SerializeField] Finger[] leftFingers, rightFingers;

    public GameObject Head { get => head;}

    [Serializable]
    public class XRData
    {
        public SerializedVector3 HeadPosition, LeftHandPosition, RightHandPosition;
        public SerializedQuaternion HeadRotation, LeftHandRotation, RightHandRotation;
        public byte LeftThumbBend, LeftIndexBend, LeftMiddleBend, LeftRingBend, LeftPinkyBend;
        public byte RightThumbBend, RightIndexBend, RightMiddleBend, RightRingBend, RightPinkyBend;
    }

    XRData currentData;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(currentData.HeadPosition);
                writer.Write(currentData.LeftHandPosition);
                writer.Write(currentData.RightHandPosition);

                writer.Write(currentData.HeadRotation);
                writer.Write(currentData.LeftHandRotation);
                writer.Write(currentData.RightHandRotation);

                writer.Write(currentData.LeftThumbBend);
                writer.Write(currentData.LeftIndexBend);
                writer.Write(currentData.LeftMiddleBend);
                writer.Write(currentData.LeftRingBend);
                writer.Write(currentData.LeftPinkyBend);

                writer.Write(currentData.RightThumbBend);
                writer.Write(currentData.RightIndexBend);
                writer.Write(currentData.RightMiddleBend);
                writer.Write(currentData.RightRingBend);
                writer.Write(currentData.RightPinkyBend);

            }
            return m.ToArray();
        }
    }

    public static XRData Deserialize(byte[] data)
    {
        XRData result = new XRData();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.HeadPosition = reader.ReadVector3();
                result.LeftHandPosition = reader.ReadVector3();
                result.RightHandPosition = reader.ReadVector3();

                result.HeadRotation = reader.ReadQuaternion();
                result.LeftHandRotation = reader.ReadQuaternion();
                result.RightHandRotation = reader.ReadQuaternion();

                result.LeftThumbBend = reader.ReadByte();
                result.LeftIndexBend = reader.ReadByte();
                result.LeftMiddleBend = reader.ReadByte();
                result.LeftRingBend = reader.ReadByte();
                result.LeftPinkyBend = reader.ReadByte();

                result.RightThumbBend = reader.ReadByte();
                result.RightIndexBend = reader.ReadByte();
                result.RightMiddleBend = reader.ReadByte();
                result.RightRingBend = reader.ReadByte();
                result.RightPinkyBend = reader.ReadByte();
            }
        }
        return result;
    }

    void Start()
    {
        currentData = new XRData();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<byte[]>("XC", ReceiveData);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/UpdatesPerSecond);
#endif

    }

    private void Update()
    {
#if UNITY_WEBGL
        float t = Time.deltaTime * TransformLerpSpeed;

        //Position
        head.transform.position = Vector3.Lerp(head.transform.position, currentData.HeadPosition, t);
        leftHand.transform.position = Vector3.Lerp(leftHand.transform.position, currentData.LeftHandPosition, t);
        rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, currentData.RightHandPosition, t);

        //Rotation
        head.transform.rotation = Quaternion.Lerp(head.transform.rotation, currentData.HeadRotation, t);
        leftHand.transform.rotation = Quaternion.Lerp(leftHand.transform.rotation, currentData.LeftHandRotation, t);
        rightHand.transform.rotation = Quaternion.Lerp(rightHand.transform.rotation, currentData.RightHandRotation, t);

        //Fingies
        leftFingers[0].SetFingerBend(Mathf.Lerp(leftFingers[0].GetCurrentBend(), ((float)currentData.LeftThumbBend) / 100, t));
        leftFingers[1].SetFingerBend(Mathf.Lerp(leftFingers[1].GetCurrentBend(), ((float)currentData.LeftIndexBend), t));
        leftFingers[2].SetFingerBend(Mathf.Lerp(leftFingers[2].GetCurrentBend(), ((float)currentData.LeftMiddleBend) / 100, t));
        leftFingers[3].SetFingerBend(Mathf.Lerp(leftFingers[3].GetCurrentBend(), ((float)currentData.LeftRingBend) / 100, t));
        leftFingers[4].SetFingerBend(Mathf.Lerp(leftFingers[4].GetCurrentBend(), ((float)currentData.LeftPinkyBend) / 100, t));

        rightFingers[0].SetFingerBend(Mathf.Lerp(rightFingers[0].GetCurrentBend(), ((float)currentData.RightThumbBend) / 100, t));
        rightFingers[1].SetFingerBend(Mathf.Lerp(rightFingers[1].GetCurrentBend(), ((float)currentData.RightIndexBend) / 100, t));
        rightFingers[2].SetFingerBend(Mathf.Lerp(rightFingers[2].GetCurrentBend(), ((float)currentData.RightMiddleBend) / 100, t));
        rightFingers[3].SetFingerBend(Mathf.Lerp(rightFingers[3].GetCurrentBend(), ((float)currentData.RightRingBend) / 100, t));
        rightFingers[4].SetFingerBend(Mathf.Lerp(rightFingers[4].GetCurrentBend(), ((float)currentData.RightPinkyBend) / 100, t));
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        //Unbind current XR player from callbacks, since this instance of XR player will not exist in the next scene, but the same socket will
        //Without this, null reference errors will be thrown in WebGL
        ClientManagerWeb.instance.Manager.Socket.Off("XRDataToClient");
#endif
    }

#if !UNITY_WEBGL
    protected virtual void SendData()
    {
        //Position
        currentData.HeadPosition = head.transform.position;
        currentData.LeftHandPosition = leftHand.transform.position;
        currentData.RightHandPosition = rightHand.transform.position;

        //Rotation
        currentData.HeadRotation = head.transform.rotation;
        currentData.LeftHandRotation = leftHand.transform.rotation;
        currentData.RightHandRotation = rightHand.transform.rotation;

        //Fingies
        currentData.LeftThumbBend = (byte)(leftFingers[0].GetCurrentBend() * 100);
        currentData.LeftIndexBend = (byte)(leftFingers[1].GetCurrentBend() * 100);
        currentData.LeftMiddleBend = (byte)(leftFingers[2].GetCurrentBend() * 100);
        currentData.LeftRingBend = (byte)(leftFingers[3].GetCurrentBend() * 100);
        currentData.LeftPinkyBend = (byte)(leftFingers[4].GetCurrentBend() * 100);

        currentData.RightThumbBend = (byte)(rightFingers[0].GetCurrentBend() * 100);
        currentData.RightIndexBend = (byte)(rightFingers[1].GetCurrentBend() * 100);
        currentData.RightMiddleBend = (byte)(rightFingers[2].GetCurrentBend() * 100);
        currentData.RightRingBend = (byte)(rightFingers[3].GetCurrentBend() * 100);
        currentData.RightPinkyBend = (byte)(rightFingers[4].GetCurrentBend() * 100);

        byte[] bytes = Serialize();

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("XS", bytes);
        }
    }
#endif


    public void ReceiveData(byte[] arrBytes)
    {
        ApplyNewData(Deserialize(arrBytes));
    }

    private void ApplyNewData(XRData data)
    {
        //Position
        currentData.HeadPosition = data.HeadPosition;
        currentData.LeftHandPosition = data.LeftHandPosition;
        currentData.RightHandPosition = data.RightHandPosition;

        //Rotation
        currentData.HeadRotation = data.HeadRotation;
        currentData.LeftHandRotation = data.LeftHandRotation;
        currentData.RightHandRotation = data.RightHandRotation;

        //Fingies
        currentData.LeftThumbBend = data.LeftThumbBend;
        currentData.LeftIndexBend = data.LeftIndexBend;
        currentData.LeftMiddleBend = data.LeftMiddleBend;
        currentData.LeftRingBend = data.LeftRingBend;
        currentData.LeftPinkyBend = data.LeftPinkyBend;

        currentData.RightThumbBend = data.RightThumbBend;
        currentData.RightIndexBend = data.RightIndexBend;
        currentData.RightMiddleBend = data.RightMiddleBend;
        currentData.RightRingBend = data.RightRingBend;
        currentData.RightPinkyBend = data.RightPinkyBend;
    }
}