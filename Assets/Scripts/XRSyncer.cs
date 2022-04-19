using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class XRSyncer : MonoBehaviour
{
    //References
    [SerializeField] GameObject head;
    [SerializeField] Hand leftHand, rightHand;

    public class XRData
    {
        public Vector3 HeadPosition, LeftHandPosition, RightHandPosition;
        public Quaternion HeadRotation, LeftHandRotation, RightHandRotation;
        public float LeftThumbBend, LeftIndexBend, LeftMiddleBend, LeftRingBend, LeftPinkyBend;
        public float RightThumbBend, RightIndexBend, RightMiddleBend, RightRingBend, RightPinkyBend;
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
        //Position
        currentData.HeadPosition = head.transform.position;
        currentData.LeftHandPosition = leftHand.transform.position;
        currentData.RightHandPosition = rightHand.transform.position;

        //Rotation
        currentData.HeadRotation = head.transform.rotation;
        currentData.LeftHandRotation = leftHand.transform.rotation;
        currentData.RightHandRotation = rightHand.transform.rotation;

        //Fingies
        currentData.LeftThumbBend = leftHand.fingers[0].GetCurrentBend();
        currentData.LeftIndexBend = leftHand.fingers[1].GetCurrentBend();
        currentData.LeftMiddleBend = leftHand.fingers[2].GetCurrentBend();
        currentData.LeftRingBend = leftHand.fingers[3].GetCurrentBend();
        currentData.LeftPinkyBend = leftHand.fingers[4].GetCurrentBend();

        currentData.RightThumbBend = rightHand.fingers[0].GetCurrentBend();
        currentData.RightIndexBend = rightHand.fingers[1].GetCurrentBend();
        currentData.RightMiddleBend = rightHand.fingers[2].GetCurrentBend();
        currentData.RightRingBend = rightHand.fingers[3].GetCurrentBend();
        currentData.RightPinkyBend = rightHand.fingers[4].GetCurrentBend();

        string json = JsonUtility.ToJson(currentData);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("XRDataToServer", json);
        }
#endif
    }


    public void ReceiveData(string json)
    {
        ApplyNewData(JsonUtility.FromJson<XRData>(json));
    }

    private void ApplyNewData(XRData data)
    {
        //Position
        head.transform.position = data.HeadPosition;
        leftHand.transform.position = data.LeftHandPosition;
        rightHand.transform.position = data.RightHandPosition;

        //Rotation
        head.transform.rotation = data.HeadRotation;
        leftHand.transform.rotation = data.LeftHandRotation;
        rightHand.transform.rotation = data.RightHandRotation;

        //Fingies
        leftHand.fingers[0].SetFingerBend(data.LeftThumbBend);
        leftHand.fingers[1].SetFingerBend(data.LeftIndexBend);
        leftHand.fingers[2].SetFingerBend(data.LeftMiddleBend);
        leftHand.fingers[3].SetFingerBend(data.LeftRingBend);
        leftHand.fingers[4].SetFingerBend(data.LeftPinkyBend);

        rightHand.fingers[0].SetFingerBend(data.RightThumbBend);
        rightHand.fingers[1].SetFingerBend(data.RightIndexBend);
        rightHand.fingers[2].SetFingerBend(data.RightMiddleBend);
        rightHand.fingers[3].SetFingerBend(data.RightRingBend);
        rightHand.fingers[4].SetFingerBend(data.RightPinkyBend);
    }
}