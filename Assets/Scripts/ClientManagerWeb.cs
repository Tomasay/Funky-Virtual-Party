using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.SocketIO3;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ClientManagerWeb : MonoBehaviour
{
    public static ClientManagerWeb instance;

    [SerializeField]
    private GameObject playerPrefab;
    private static List<ClientPlayer> players = new List<ClientPlayer>();
    private ClientPlayer localPlayer;

    public List<ClientPlayer> Players { get => players; }

    SocketManager manager;
    private const string url = "https://vrpartygame.herokuapp.com/";
    private const string socketUrl = url + "socket.io/";
    public string URL { get => url; }

    public SocketManager Manager { get => manager; }
    public ClientPlayer LocalPlayer { get => localPlayer; }

    [SerializeField] TMP_Text debugText;


    [DllImport("__Internal")]
    private static extern void ReloadPage();

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            manager = new SocketManager(new Uri(socketUrl));
            manager.Socket.On<bool, string>("joinedRoom", JoinRoomCheck);
            manager.Socket.On<string, string, string>("connectToHost", OnClientConnect);
            manager.Socket.On<string, string>("disconnectToUnity", OnClientDisconnect);
            manager.Socket.On<float, float, string>("toUnity", OnInputReceived);
            manager.Socket.On<string>("action", OnAction);
            manager.Socket.On<string[]>("playerInfoToClient", playerInfoReceived);
            manager.Socket.On<string, string, int, float>("syncCustomizationsFromServer", SyncCustomizations);
            manager.Socket.On<string, float, float, float>("syncPlayerPosToClient", SyncPlayerPos);
            manager.Socket.On("roomClosed", ReloadPage);
            manager.Socket.On<string>("minigameStart", LoadGame);
            manager.Socket.On("readyUpVR", OnVRReadyUp);
            manager.Socket.On<string>("readyUp", OnReadyUp);

            DontDestroyOnLoad(gameObject);

            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AttemptJoinRoom(string code, string name)
    {
        Debug.Log("Attempting to connect to socket.io server: " + socketUrl);

        manager.Socket.Emit("joinRoom", code, name);
        //manager.Socket.Once("connect", () => Debug.Log("connected!"));
    }

    public void playerInfoReceived(string[] players)
    {
        if (players.Length == 0)
        {
            return;
        }

        //Parse players string
        char[] delims = new[] { '\n' };
        foreach (string s in players)
        {
            string[] attributes = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            OnClientConnect(attributes[0], attributes[1], attributes[2]);

            if (int.TryParse(attributes[4], out int parsedHead) && float.TryParse(attributes[5], out float parsedHeight))
            {
                SyncCustomizations(attributes[0], attributes[3], parsedHead, parsedHeight);
            }
        }
    }

    public void SyncPlayerPos(string id, float x, float y, float z)
    {
        GetPlayerByID(id).transform.position = new Vector3(x, y, z);
    }

    public void ActionButtonPressed()
    {
        localPlayer.Action();
        manager.Socket.Emit("Action");
    }

    private void OnAction(string id)
    {
        if (GetPlayerByID(id))
        {
            GetPlayerByID(id).Action();
        }
    }

    private void JoinRoomCheck(bool joined, string socketID)
    {
        if(joined) //If we successfuly joined a room
        {
        }
        else //If room was invalid
        {
            //TODO: Inform player that party code was invalid
        }
    }

    public event Action<GameObject> onClientConnect;
    private void OnClientConnect(string id, string ip, string name)
    {
        Debug.Log("Client connected with ID: " + id + " at IP address: " + ip);

        ClientPlayer newPlayer = Instantiate(playerPrefab).GetComponent<ClientPlayer>();

        players.Add(newPlayer);
        newPlayer.PlayerID = id;
        newPlayer.PlayerName = name;

        if(players.Count == 1) //If this is the first player added, it is local
        {
            newPlayer.IsLocal = true;
            localPlayer = newPlayer;
        }

        newPlayer.InitialCustomize();


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

    private void SyncCustomizations(string id, string color, int headShape, float height)
    {
        GetPlayerByID(id).SetCustomizations(color, headShape, height);
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

    private void OnInputReceived(float x, float y, string id)
    {
        if (GetPlayerByID(id))
        {
            GetPlayerByID(id).Move(x, y);
        }
    }

    public event Action onVRReadyUp;
    private void OnVRReadyUp()
    {
        if (onVRReadyUp != null)
        {
            onVRReadyUp();
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

    private void LoadGame(string gameName)
    {
        switch (gameName)
        {
            case "ChaseGame":
                SceneManager.LoadScene("ChaseGameClient");
                break;
            default:
                break;
        }
    }

    public void SpawnPlayers(GameObject prefab)
    {
        for (int i = 0; i < players.Count; i++)
        {
            string ID = players[i].PlayerID;
            string playerName = players[i].PlayerName;
            Color playerColor = players[i].PlayerColor;
            int playerHeadType = players[i].PlayerHeadType;
            float playerHeight = players[i].PlayerHeight;
            bool isLocal = players[i].IsLocal;

            Destroy(players[i].gameObject);
            players[i] = Instantiate(prefab).GetComponent<ClientPlayer>();
            players[i].PlayerID = ID;
            players[i].PlayerName = playerName;
            players[i].PlayerColor = playerColor;
            players[i].PlayerHeadType = playerHeadType;
            players[i].PlayerHeight = playerHeight;
            players[i].IsLocal = isLocal;
            if(isLocal)
            {
                localPlayer = players[i];
            }
        }
    }
}