using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using DG.Tweening;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class XRSyncer : MonoBehaviour
{
    //How often data is sent to be synced
    private float UpdatesPerSecond = 10;
    private float updatePollRate;

    //References
    [SerializeField] GameObject head;
    [SerializeField] GameObject leftHand, rightHand;
    [SerializeField] Finger[] leftFingers, rightFingers;

    public GameObject Head { get => head;}

    [Serializable]
    public class XRData
    {
        public SerializedVector3 HeadPosition, LeftHandPosition, RightHandPosition;
        public SerializedVector3 HeadRotation, LeftHandRotation, RightHandRotation;
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

                result.HeadRotation = reader.ReadVector3();
                result.LeftHandRotation = reader.ReadVector3();
                result.RightHandRotation = reader.ReadVector3();

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
        updatePollRate = 1 / UpdatesPerSecond;

        currentData = new XRData();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<byte[]>("XC", ReceiveData);
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/UpdatesPerSecond);
#endif

    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        //Unbind current XR player from callbacks, since this instance of XR player will not exist in the next scene, but the same socket will
        //Without this, null reference errors will be thrown in WebGL
        ClientManagerWeb.instance.Manager.Socket.Off("XC");
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
        currentData.HeadRotation = head.transform.rotation.eulerAngles;
        currentData.LeftHandRotation = leftHand.transform.rotation.eulerAngles;
        currentData.RightHandRotation = rightHand.transform.rotation.eulerAngles;

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
        head.transform.DOMove(data.HeadPosition, updatePollRate);
        leftHand.transform.DOMove(data.LeftHandPosition, updatePollRate);
        rightHand.transform.DOMove(data.RightHandPosition, updatePollRate);

        //Rotation
        head.transform.DORotate(data.HeadRotation, updatePollRate);
        leftHand.transform.DORotate(data.LeftHandRotation, updatePollRate);
        rightHand.transform.DORotate(data.RightHandRotation, updatePollRate);

        //Fingies
        leftFingers[0].SetFingerBendTween(((float)data.LeftThumbBend / 100), updatePollRate);
        leftFingers[1].SetFingerBendTween(((float)data.LeftIndexBend / 100), updatePollRate);
        leftFingers[2].SetFingerBendTween(((float)data.LeftMiddleBend / 100), updatePollRate);
        leftFingers[3].SetFingerBendTween(((float)data.LeftRingBend / 100), updatePollRate);
        leftFingers[4].SetFingerBendTween(((float)data.LeftPinkyBend / 100), updatePollRate);

        rightFingers[0].SetFingerBendTween(((float)data.RightThumbBend / 100), updatePollRate);
        rightFingers[1].SetFingerBendTween(((float)data.RightIndexBend / 100), updatePollRate);
        rightFingers[2].SetFingerBendTween(((float)data.RightMiddleBend / 100), updatePollRate);
        rightFingers[3].SetFingerBendTween(((float)data.RightRingBend / 100), updatePollRate);
        rightFingers[4].SetFingerBendTween(((float)data.RightPinkyBend / 100), updatePollRate);
    }
}