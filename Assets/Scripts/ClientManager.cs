using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using BestHTTP;
using BestHTTP.SocketIO3;

public class ClientManager : MonoBehaviour
{
    public static ClientManager instance;

    [SerializeField]
    private GameObject playerPrefab;
    private Dictionary<string, GameObject> players;

    public Dictionary<string, GameObject> Players { get => players; }

    SocketManager manager;
    private string unityClientID = "";
    private const string URI = "https://vrpartygame.herokuapp.com/socket.io/";

    private const int PASSCODE_LENGTH = 6;
    private string passcode;
    public string Passcode { get => passcode;}

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
    }

    void Awake()
    {
        players = new Dictionary<string, GameObject>();

        Debug.Log("Attempting to connect to socket.io server: " + URI);

        manager = new SocketManager(new Uri(URI));
        passcode = GenerateCode();
        Debug.Log("PASSCODE: " + passcode);
        manager.Socket.Emit("unityJoinRoom", passcode);
        //manager.Socket.Emit("isUnity");

        manager.Socket.Once("connect", () => Debug.Log("connected!"));

        //manager.Socket.On<int>("toUnity", OnInputReceived);
        manager.Socket.On<int, int, string>("toUnity", OnInputReceived);
        manager.Socket.On<string, string, string>("connectToUnity", OnClientConnect);
        manager.Socket.On<string, string>("disconnectToUnity", OnClientDisconnect);

        DontDestroyOnLoad(gameObject);
    }

    public event Action<GameObject> onClientConnect;
    private void OnClientConnect(string id, string ip, string name)
    {
        Debug.Log("Client connected with ID: " + id + " at IP address: " + ip);

        /* This was implemented because Unity was triggering as the first client connect. Seems to be unnecessary with the implementation of rooms
        if (unityClientID == "")
        {
            unityClientID = id;
            return;
        }
        */

        players.Add(id, Instantiate(playerPrefab));
        players[id].GetComponent<ClientPlayer>().PlayerID = id;
        //players[id].GetComponent<ClientPlayer>().SetPlayerName("Player " + players.Count);
        players[id].GetComponent<ClientPlayer>().SetPlayerName(name);

        if (onClientConnect != null)
        {
            onClientConnect(players[id]);
        }
    }


    public event Action<string> onClientDisonnect;
    private void OnClientDisconnect(string id, string ip)
    {
        Debug.Log("Client disconnected with ID: " + id + " at IP address: " + ip);

        if (players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }

        if (onClientDisonnect != null)
        {
            onClientDisonnect(id);
        }
    }

    private void OnInputReceived(int x, int y, string id)
    {
        players[id].GetComponent<ClientPlayer>().Move(x, y);
    }

    private string GetTime()
    {
        DateTime now = DateTime.Now;
        return now.Hour + ":" + now.Minute + ":" + now.Second + "." + now.Millisecond;
    }

    private string GenerateCode()
    {
        string newCode = "";
        for (int i = 0; i < PASSCODE_LENGTH; i++)
        {
            newCode += GetRandomLetter();
        }
        return newCode;
    }

    public static char GetRandomLetter()
    {
        return (char)UnityEngine.Random.Range(65, 91);
    }
}