using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.SocketIO3;

public class ClientManagerWeb : MonoBehaviour
{
    public static ClientManager instance;

    [SerializeField]
    private GameObject playerPrefab;
    private static List<ClientPlayer> players = new List<ClientPlayer>();

    public List<ClientPlayer> Players { get => players; }

    SocketManager manager;
    private string unityClientID = "";
    private const string url = "https://vrpartygame.herokuapp.com/";
    private const string socketUrl = url + "socket.io/";

    private const int PASSCODE_LENGTH = 6;
    private static string passcode;
    public string URL { get => url; }
    public string Passcode { get => passcode; }

    string test = "test";

    // Start is called before the first frame update
    void Start()
    {
        TouchScreenKeyboard.Open(test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttemptJoinRoom(string code)
    {
        if (passcode == null)
        {
            Debug.Log("Attempting to connect to socket.io server: " + socketUrl);

            manager = new SocketManager(new Uri(socketUrl));
            manager.Socket.Emit("joinRoom", passcode);

            manager.Socket.Once("connect", () => Debug.Log("connected!"));

            manager.Socket.On<bool>("joinedRoom", JoinRoomCheck);

            DontDestroyOnLoad(gameObject);
        }
    }

    private void JoinRoomCheck(bool joined)
    {
        if(joined) //If we successfuly joined a room
        {

        }
        else //If room was invalid
        {

        }
    }
}
