using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using BestHTTP;
using BestHTTP.SocketIO3;

public class ClientManager : MonoBehaviour
{
    SocketManager manager;
    const string URI = "https://vrpartygame.herokuapp.com/socket.io/";

    [SerializeField]
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Attempting to connect to socket.io server: " + URI);

        manager = new SocketManager(new Uri(URI));

        manager.Socket.Once("connect", () => Debug.Log("connected!"));

        //manager.Socket.On<string>("time", OnTime);

        manager.Socket.On<int>("toUnity", OnInputReceived);
    }

    private void OnInputReceived(int input)
    {
        Debug.Log("Input received: " + input + "\n at " + GetTime());

        player.transform.Translate(input, 0, 0);
    }

    private string GetTime()
    {
        DateTime now = DateTime.Now;
        return now.Hour + ":" + now.Minute + ":" + now.Second + "." + now.Millisecond;
    }
}