using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(RealtimeView))]
public class RequestOwnershipOnConnectToRoom : MonoBehaviour
{
    RealtimeView realtimeView;

    void Start()
    {
        RealtimeSingleton.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        Debug.Log("Requesting ownership");

        realtimeView = GetComponent<RealtimeView>();

        realtimeView.RequestOwnership();

        if (TryGetComponent<RealtimeTransform>(out RealtimeTransform rt))
        {
            rt.RequestOwnership();
        }

        RealtimeSingleton.instance.Realtime.didConnectToRoom -= Realtime_didConnectToRoom;
    }
}