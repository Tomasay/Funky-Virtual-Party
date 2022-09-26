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
        public float LeftThumbBend, LeftIndexBend, LeftMiddleBend, LeftRingBend, LeftPinkyBend;
        public float RightThumbBend, RightIndexBend, RightMiddleBend, RightRingBend, RightPinkyBend;
    }

    XRData currentData;

    void Start()
    {
        currentData = new XRData();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<byte[]>("XRDataToClient", ReceiveData);
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
        leftFingers[0].SetFingerBend(Mathf.Lerp(leftFingers[0].GetCurrentBend(), currentData.LeftThumbBend, t));
        leftFingers[1].SetFingerBend(Mathf.Lerp(leftFingers[1].GetCurrentBend(), currentData.LeftIndexBend, t));
        leftFingers[2].SetFingerBend(Mathf.Lerp(leftFingers[2].GetCurrentBend(), currentData.LeftMiddleBend, t));
        leftFingers[3].SetFingerBend(Mathf.Lerp(leftFingers[3].GetCurrentBend(), currentData.LeftRingBend, t));
        leftFingers[4].SetFingerBend(Mathf.Lerp(leftFingers[4].GetCurrentBend(), currentData.LeftPinkyBend, t));

        rightFingers[0].SetFingerBend(Mathf.Lerp(rightFingers[0].GetCurrentBend(), currentData.RightThumbBend, t));
        rightFingers[1].SetFingerBend(Mathf.Lerp(rightFingers[1].GetCurrentBend(), currentData.RightIndexBend, t));
        rightFingers[2].SetFingerBend(Mathf.Lerp(rightFingers[2].GetCurrentBend(), currentData.RightMiddleBend, t));
        rightFingers[3].SetFingerBend(Mathf.Lerp(rightFingers[3].GetCurrentBend(), currentData.RightRingBend, t));
        rightFingers[4].SetFingerBend(Mathf.Lerp(rightFingers[4].GetCurrentBend(), currentData.RightPinkyBend, t));
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
        currentData.LeftThumbBend = leftFingers[0].GetCurrentBend();
        currentData.LeftIndexBend = leftFingers[1].GetCurrentBend();
        currentData.LeftMiddleBend = leftFingers[2].GetCurrentBend();
        currentData.LeftRingBend = leftFingers[3].GetCurrentBend();
        currentData.LeftPinkyBend = leftFingers[4].GetCurrentBend();

        currentData.RightThumbBend = rightFingers[0].GetCurrentBend();
        currentData.RightIndexBend = rightFingers[1].GetCurrentBend();
        currentData.RightMiddleBend = rightFingers[2].GetCurrentBend();
        currentData.RightRingBend = rightFingers[3].GetCurrentBend();
        currentData.RightPinkyBend = rightFingers[4].GetCurrentBend();

        byte[] bytes = ByteArrayConverter.ObjectToByteArray<XRData>(currentData);

        //string json = JsonUtility.ToJson(currentData);
        //Debug.Log("Size of json: " + json.Length * sizeof(char) + "   size of byte array: " + bytes.Length);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("XRDataToServer", bytes);
        }
    }
#endif


    public void ReceiveData(string json)
    {
        ApplyNewData(JsonUtility.FromJson<XRData>(json));
    }

    public void ReceiveData(byte[] arrBytes)
    {
        ApplyNewData(ByteArrayConverter.ByteArrayToObject<XRData>(arrBytes));
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