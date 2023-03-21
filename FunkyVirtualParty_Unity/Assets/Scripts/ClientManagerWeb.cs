using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Parsers;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.IO;

public class ClientManagerWeb : MonoBehaviour
{
    public static ClientManagerWeb instance;

    [SerializeField]
    private GameObject playerPrefab;
    private static List<ClientPlayer> players = new List<ClientPlayer>();
    private ClientPlayer localPlayer;
    private bool joinedRoom;

    public List<ClientPlayer> Players { get => players; }

    SocketManager manager;
    private const string url = "https://vrpartygame.herokuapp.com/";
    private const string socketUrl = url + "socket.io/";
    public string URL { get => url; }

    public SocketManager Manager { get => manager; }
    public ClientPlayer LocalPlayer { get => localPlayer; }

    [SerializeField] TMP_Text debugText;

    [SerializeField] RectTransform fadeRect;
    private float fadeIncrementDistance;

    //id of vr headset
    string hostID;

    [DllImport("__Internal")]
    private static extern void ReloadPage();

    //Check to make sure we do not load main menu consecutively
    private bool isInMainMenu = true;

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            manager = new SocketManager(new Uri(socketUrl));
            manager.Parser = new MsgPackParser();

            manager.Socket.On<bool>("joinedRoom", JoinRoomCheck);
            manager.Socket.On<string, string, string>("connectToHost", OnClientConnect);
            manager.Socket.On<string, byte>("assignPlayerByteIDClient", AssignPlayerByteID);
            manager.Socket.On<string, string>("disconnectToUnity", OnClientDisconnect);
            manager.Socket.On<byte[]>("IC", OnInputReceived);
            manager.Socket.On<byte>("action", OnAction);
            manager.Socket.On<string, byte[]>("playerInfoToClient", playerInfoReceived);
            manager.Socket.On<string, string, int, float, int>("syncCustomizationsFromServer", SyncCustomizations);
            manager.Socket.On<byte, float, float, float, bool>("syncPlayerPosToClient", SyncPlayerPos);
            manager.Socket.On("roomClosed", ReloadPage);
            manager.Socket.On<string>("minigameStart", LoadGame);
            manager.Socket.On("readyUpVR", OnVRReadyUp);
            manager.Socket.On<byte>("readyUp", OnReadyUp);

            DontDestroyOnLoad(gameObject);

            instance = this;

            SceneManager.sceneLoaded += FadeInScene;
        }
        else
        {
            Destroy(gameObject);
        }

        //Set fade rect proper size
        float aspect = (Screen.height / fadeRect.rect.height) * 2;
        fadeRect.sizeDelta = new Vector2(fadeRect.rect.width * aspect, fadeRect.rect.height * aspect);
        fadeIncrementDistance = Screen.width / 8;
    }

    public void AttemptJoinRoom(string code, string name)
    {
        Debug.Log("Attempting to connect to socket.io server: " + socketUrl);

        manager.Socket.Emit("joinRoom", code, name);
        //manager.Socket.Once("connect", () => Debug.Log("connected!"));
    }

    public void AssignPlayerByteID(string socketId, byte byteId)
    {
        GetPlayerBySocketID(socketId).PlayerByteID = byteId;
    }

    public void playerInfoReceived(string host, byte[] players)
    {
        hostID = host;

        if (players.Length == 0)
        {
            return;
        }

        DeserializePlayersInfo(players);
    }

    public void SyncPlayerPos(byte id, float x, float y, float z, bool lerp)
    {
        ClientPlayer cp = GetPlayerByByteID(id);

        //If player is too far away from real position, override lerp
        /*
        if (lerp && Vector3.Distance(new Vector3(x, y, z), cp.transform.position) > 1)
        {
            lerp = false;
        }
        */

        if (!lerp)
        {
            cp.transform.position = new Vector3(x, y, z);
        }
        else
        {
            //cp.transform.position = Vector3.Lerp(GetPlayerByID(id).transform.position, new Vector3(x, y, z), Time.deltaTime);
            cp.posFromHost = new Vector3(x, y, z);
        }
    }

    public void ActionButtonPressed()
    {
        localPlayer.Action();
        manager.Socket.Emit("Action", LocalPlayer.PlayerByteID);
    }

    private void OnAction(byte id)
    {
        if (GetPlayerByByteID(id))
        {
            GetPlayerByByteID(id).Action();
        }
    }

    public event Action onClientFailedConnect;
    private void JoinRoomCheck(bool joined)
    {
        if(joined) //If we successfuly joined a room
        {
            joinedRoom = true;
        }
        else //If room was invalid
        {
            onClientFailedConnect.Invoke();
        }
    }

    public event Action<GameObject> onClientConnect;
    private void OnClientConnect(string id, string ip, string name)
    {
        Debug.Log("Client connected with ID: " + id + " at IP address: " + ip);

        ClientPlayer newPlayer = Instantiate(playerPrefab).GetComponent<ClientPlayer>();

        players.Add(newPlayer);
        newPlayer.PlayerSocketID = id;
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
    private void OnClientConnect(string id, byte byteId, string ip, string name)
    {
        OnClientConnect(id, ip, name);
        GetPlayerBySocketID(id).PlayerByteID = byteId;
    }

    public event Action<string> onClientDisonnect;
    private void OnClientDisconnect(string id, string ip)
    {
        Debug.Log("Client disconnected with ID: " + id + " at IP address: " + ip);

        if(id.Equals(hostID))
        {
            ReloadPage();
        }

        if (GetPlayerBySocketID(id))
        {
            Destroy(GetPlayerBySocketID(id).gameObject);
            players.Remove(GetPlayerBySocketID(id));
        }

        if (onClientDisonnect != null)
        {
            onClientDisonnect(id);
        }
    }

    private void SyncCustomizations(string id, string color, int headShape, float height, int hatIndex)
    {
        GetPlayerBySocketID(id).SetCustomizations(color, headShape, height, hatIndex);
    }

    public ClientPlayer GetPlayerBySocketID(string id)
    {
        foreach (ClientPlayer p in players)
        {
            if (p.PlayerSocketID.Equals(id))
                return p;
        }

        return null;
    }

    public ClientPlayer GetPlayerByByteID(byte id)
    {
        foreach (ClientPlayer p in players)
        {
            if (p.PlayerByteID == id)
                return p;
        }

        return null;
    }

    private void OnInputReceived(byte[] data)
    {
        ClientInputData newData = ClientPlayer.DeserializeInputData(data);
        if (LocalPlayer.PlayerByteID != newData.id)
        {
            GetPlayerByByteID(newData.id).Move(newData.input);
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
    private void OnReadyUp(byte id)
    {
        if (onReadyUp != null)
        {
            onReadyUp(GetPlayerByByteID(id));
        }
    }

    private void LoadGame(string gameName)
    {
        StartCoroutine("LoadSceneWithFade", gameName + "Client");
    }

    public void LoadMainMenu()
    {
        if (!isInMainMenu)
        {
            StartCoroutine("LoadSceneWithFade", "MainMenuClient");
        }
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        isInMainMenu = sceneName.Equals("MainMenuClient");

        //Fade out
        float val = Screen.width + (Screen.width / 2);
        fadeRect.position = new Vector2(-val, fadeRect.position.y);

        for (int i = 0; i <= (val * 2) / fadeIncrementDistance; i++)
        {
            fadeRect.position = new Vector2(-val + (i * fadeIncrementDistance), fadeRect.position.y);
            yield return new WaitForSeconds(0.05f);
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        //Fade in
        float val = Screen.width + (Screen.width / 2);
        fadeRect.position = new Vector2(val, fadeRect.position.y);
        

        for (int i = 0; i <= (val * 2) / fadeIncrementDistance; i++)
        {
            fadeRect.position = new Vector2(val - (i * fadeIncrementDistance), fadeRect.position.y);
            yield return new WaitForSeconds(0.05f);
        }
        
    }

    private void FadeInScene(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine("FadeIn");
    }

    public void SpawnPlayers(GameObject prefab, bool parentToLocation = false, Transform[] locationParents = null)
    {
        for (int i = 0; i < players.Count; i++)
        {
            string socketID = players[i].PlayerSocketID;
            byte byteID = players[i].PlayerByteID;
            string playerName = players[i].PlayerName;
            Color playerColor = players[i].PlayerColor;
            int playerHeadType = players[i].PlayerHeadType;
            float playerHeight = players[i].PlayerHeight;
            int hatIndex = players[i].PlayerHatIndex;
            bool isLocal = players[i].IsLocal;

            Destroy(players[i].gameObject);
            players[i] = Instantiate(prefab).GetComponent<ClientPlayer>();
            players[i].PlayerSocketID = socketID;
            players[i].PlayerByteID = byteID;
            players[i].PlayerName = playerName;
            players[i].PlayerColor = playerColor;
            players[i].PlayerHeadType = playerHeadType;
            players[i].PlayerHeight = playerHeight;
            players[i].PlayerHatIndex = hatIndex;
            players[i].IsLocal = isLocal;
            if(isLocal)
            {
                localPlayer = players[i];
            }

            if (parentToLocation)
            {
                players[i].transform.SetParent(locationParents[i]);
                players[i].transform.localRotation = Quaternion.identity;
                players[i].transform.localPosition = Vector3.zero;
            }

            manager.Socket.Emit("requestPlayerPosFromClient", byteID);
        }
    }

    public void DeserializePlayersInfo(byte[] data)
    {
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                byte count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    string playerSocketID = reader.ReadString();
                    OnClientConnect(playerSocketID, reader.ReadByte(), reader.ReadString(), reader.ReadString());
                    GetPlayerBySocketID(playerSocketID).SetCustomizations(reader.ReadString(), reader.ReadSByte(), reader.ReadSingle(), reader.ReadSByte());
                }
            }
        }
    }
}