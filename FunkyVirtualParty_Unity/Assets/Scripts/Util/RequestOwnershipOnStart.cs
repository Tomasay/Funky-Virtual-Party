using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(RealtimeView))]
public class RequestOwnershipOnStart : MonoBehaviour
{
    RealtimeView realtimeView;

    void Start()
    {
        realtimeView = GetComponent<RealtimeView>();

        realtimeView.RequestOwnership();

        if (TryGetComponent<RealtimeTransform>(out RealtimeTransform rt))
        {
            rt.RequestOwnership();
        }
    }
}