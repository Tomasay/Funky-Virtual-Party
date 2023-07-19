using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using DG.Tweening;

public class Drawbridge : MonoBehaviour
{
    [SerializeField]
    GameObject leftBridge, rightBridge;

    private bool isOpen;

    private float openTime = 10; //How long the bridge stays open before auto closing

    public void ButtonPressed()
    {
        if(!isOpen)
            OpenBridge();
    }

    void OpenBridge()
    {
        isOpen = true;

        RequestOwnership();

        leftBridge.transform.DOLocalRotate(new Vector3(0, 75, 180), 1);
        rightBridge.transform.DOLocalRotate(new Vector3(0, -75, 0), 1);

        Invoke("CloseBridge", openTime);
    }

    void CloseBridge()
    {
        isOpen = false;

        RequestOwnership();

        leftBridge.transform.DOLocalRotate(new Vector3(0, 0, 180), 1);
        rightBridge.transform.DOLocalRotate(new Vector3(0, 0, 0), 1);
    }

    void RequestOwnership()
    {
        leftBridge.GetComponent<RealtimeView>().RequestOwnership();
        leftBridge.GetComponent<RealtimeTransform>().RequestOwnership();
        rightBridge.GetComponent<RealtimeView>().RequestOwnership();
        rightBridge.GetComponent<RealtimeTransform>().RequestOwnership();
    }
}