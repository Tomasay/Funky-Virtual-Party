using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using BestHTTP.SocketIO3;

public class XRSyncer : MonoBehaviour
{
    //References
    [SerializeField] GameObject head;
    [SerializeField] Hand leftHand, rightHand;

    public class XRData
    {
        public Vector3 HeadPosition, LeftHandPosition, RightHandPosition;
        public Quaternion HeadRotation, LeftHandRotation, RightHandRotation;
    }

    XRData currentData;


    void Start()
    {
        currentData = new XRData();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string>("XRDataToClient", ReceiveData);
#endif

    }


    void Update()
    {
#if !UNITY_WEBGL
        currentData.HeadPosition = head.transform.position;
        currentData.LeftHandPosition = leftHand.transform.position;
        currentData.RightHandPosition = rightHand.transform.position;

        currentData.HeadRotation = head.transform.rotation;
        currentData.LeftHandRotation = leftHand.transform.rotation;
        currentData.RightHandRotation = rightHand.transform.rotation;

        string json = JsonUtility.ToJson(currentData);

        ClientManager.instance.Manager.Socket.Emit("XRDataToServer", json);
#endif
    }


    public void ReceiveData(string json)
    {
        ApplyNewData(JsonUtility.FromJson<XRData>(json));
    }

    private void ApplyNewData(XRData data)
    {
        head.transform.position = data.HeadPosition;
        leftHand.transform.position = data.LeftHandPosition;
        rightHand.transform.position = data.RightHandPosition;

        head.transform.rotation = data.HeadRotation;
        leftHand.transform.rotation = data.LeftHandRotation;
        rightHand.transform.rotation = data.RightHandRotation;
    }
}