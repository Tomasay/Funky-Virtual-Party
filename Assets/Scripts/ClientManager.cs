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
    //const string URI = "https://socket-io-chat.now.sh/socket.io";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Attempting to connect to socket.io server: " + URI);

        manager = new SocketManager(new Uri(URI));

        manager.Socket.Once("connect", () => Debug.Log("connected!"));

        //manager.Socket.On<string>("time", OnTime);

        manager.Socket.On<string>("toUnity", OnInputReceived);
    }

    private void OnInputReceived(string msg)
    {
        Debug.Log("Input received: " + msg + "\n at " + DateTime.Now.ToString());
    }
}
