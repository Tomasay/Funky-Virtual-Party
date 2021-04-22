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

    private bool isHoldingLeft, isHoldingRight;
    private int playerSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Attempting to connect to socket.io server: " + URI);

        manager = new SocketManager(new Uri(URI));

        manager.Socket.Once("connect", () => Debug.Log("connected!"));
        manager.Socket.On<int>("toUnity", OnInputReceived);
    }

    private void Update()
    {
        if(isHoldingLeft)
        {
            player.transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
        }
        if (isHoldingRight)
        {
            player.transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void OnInputReceived(int input)
    {
        switch (input)
        {
            case -1:
                isHoldingLeft = true;
                break;
            case 1:
                isHoldingRight = true;
                break;
            case 0:
                isHoldingLeft = false;
                isHoldingRight = false;
                break;
            default:
                break;
        }

        Debug.Log("Input received: " + input + "\n at " + GetTime());
    }

    private string GetTime()
    {
        DateTime now = DateTime.Now;
        return now.Hour + ":" + now.Minute + ":" + now.Second + "." + now.Millisecond;
    }
}