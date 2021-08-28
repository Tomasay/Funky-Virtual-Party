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
    private List<ClientPlayer> players;

    public List<ClientPlayer> Players { get => players; }

    SocketManager manager;
    private string unityClientID = "";
    private const string url = "https://vrpartygame.herokuapp.com/";
    private const string socketUrl = url + "socket.io/";

    private const int PASSCODE_LENGTH = 6;
    private string passcode;
    public string URL { get => url; }
    public string Passcode { get => passcode; }

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
    }

    void Awake()
    {
        players = new List<ClientPlayer>();

        Debug.Log("Attempting to connect to socket.io server: " + socketUrl);

        manager = new SocketManager(new Uri(socketUrl));
        passcode = GenerateCode();
        Debug.Log("PASSCODE: " + passcode);
        manager.Socket.Emit("unityJoinRoom", passcode);
        //manager.Socket.Emit("isUnity");

        manager.Socket.Once("connect", () => Debug.Log("connected!"));

        //manager.Socket.On<int>("toUnity", OnInputReceived);
        manager.Socket.On<int, int, string>("toUnity", OnInputReceived);
        manager.Socket.On<string, string, string>("connectToUnity", OnClientConnect);
        manager.Socket.On<string, string>("disconnectToUnity", OnClientDisconnect);
        manager.Socket.On<string>("readyUp", OnReadyUp);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        foreach (ClientPlayer cp in players)
        {
            if (cp == null)
                Debug.Log("WE GOT A NULL");
        }
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

        ClientPlayer newPlayer = Instantiate(playerPrefab).GetComponent<ClientPlayer>();

        players.Add(newPlayer);
        newPlayer.PlayerID = id;
        //players[id].GetComponent<ClientPlayer>().SetPlayerName("Player " + players.Count);
        newPlayer.PlayerName = name;

        if (onClientConnect != null)
        {
            onClientConnect(newPlayer.gameObject);
        }
    }

    public event Action<string> onClientDisonnect;
    private void OnClientDisconnect(string id, string ip)
    {
        Debug.Log("Client disconnected with ID: " + id + " at IP address: " + ip);

        if (GetPlayerByID(id))
        {
            Destroy(GetPlayerByID(id).gameObject);
            players.Remove(GetPlayerByID(id));
        }

        if (onClientDisonnect != null)
        {
            onClientDisonnect(id);
        }
    }

    private void OnInputReceived(int x, int y, string id)
    {
        if (GetPlayerByID(id))
        {
            GetPlayerByID(id).Move(x, y);
        }
    }

    public event Action<ClientPlayer> onReadyUp;
    private void OnReadyUp(string id)
    {
        if (onReadyUp != null)
        {
            onReadyUp(GetPlayerByID(id));
        }
    }

    public void SpawnPlayers(GameObject prefab, Transform[] locations)
    {
        for (int i = 0; i < players.Count; i++)
        {
            string ID = players[i].PlayerID;
            string playerName = players[i].PlayerName;
            Color playerColor = players[i].PlayerColor;

            Destroy(players[i].gameObject);
            players[i] = Instantiate(prefab).GetComponent<ClientPlayer>();
            players[i].PlayerID = ID;
            players[i].PlayerName = playerName;
            players[i].PlayerColor = playerColor;

            if (locations[i])
            {
                players[i].transform.position = locations[i].position;
            }
        }
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

    private ClientPlayer GetPlayerByID(string id)
    {
        foreach (ClientPlayer p in players)
        {
            if (p.PlayerID.Equals(id))
                return p;
        }

        return null;
    }
}