using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientEvents : MonoBehaviour
{
    public static ClientEvents current;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    public event Action<GameObject> onClientConnect;
    public void OnClientConnect(GameObject player)
    {
        if (onClientConnect != null)
        {
            onClientConnect(player);
        }
    }

    public event Action onClientDisonnect;
    public void OnClientDisconnect(string id, string ip)
    {
        if (onClientDisonnect != null)
        {
            onClientDisonnect();
        }
    }
}