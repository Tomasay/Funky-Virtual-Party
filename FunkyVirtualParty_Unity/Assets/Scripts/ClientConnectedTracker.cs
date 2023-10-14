using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ClientConnectedTracker : MonoBehaviour
{
    [SerializeField]
    RealtimeView realtimeView;

    ClientPlayer cp;

    void Start()
    {
        DontDestroyOnLoad(this);

        cp = ClientPlayer.GetClientByOwnerID(realtimeView.ownerIDSelf);
    }


    private void OnDestroy()
    {
        ClientPlayer.OnClientDisconnected.Invoke(cp);
    }
}