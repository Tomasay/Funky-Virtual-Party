using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using BestHTTP;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Parsers;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;
using System.IO;

public class ClientManager : MonoBehaviour
{
    private static ClientManager Instance = null;
    public static ClientManager instance;
    /*{
    get
        {
            if(Instance == null)
            {
                Instance = new ClientManager();
            }
            return Instance;
        }
    }*/

    public UnityEvent onPlayerSpawned;

    [SerializeField]
    private GameObject playerPrefab;
    private static List<ClientPlayer> players = new List<ClientPlayer>();
    private byte playerCounter; //Incremented and used to assign single digit IDs to players

    public List<ClientPlayer> Players { get => players; }

    SocketManager manager;
    private string unityClientID = "";
    private const string url = "https://vrpartygame.herokuapp.com/";
    private const string QRurl = "https://www.partycrashers.app";
    private const string socketUrl = url + "socket.io/";

    private const int PASSCODE_LENGTH = 4;
    private static string passcode;

    [SerializeField] private VolumeProfile postProcessingProfile;

    public SocketManager Manager { get => manager; }
    public string URL { get => url; }
    public string QRURL { get => QRurl; }
    public string Passcode { get => passcode; }

#if UNITY_EDITOR
    [SerializeField]
    protected string[] debugPlayersToAdd;
#endif

    void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += FadeInScene;

        if (passcode == null)
        {
            Debug.Log("Attempting to connect to socket.io server: " + socketUrl);

            manager = new SocketManager(new Uri(socketUrl));
            manager.Parser = new MsgPackParser();
            passcode = GenerateCode();
            Debug.Log("PASSCODE: " + passcode);
            manager.Socket.Emit("unityJoinRoom", passcode);
            //manager.Socket.Emit("isUnity");

            manager.Socket.Once("connect", () => Debug.Log("connected!"));

            manager.Socket.On<byte[]>("IC", OnInputReceived);
            manager.Socket.On<string, string, string>("connectToHost", OnClientConnect);
            manager.Socket.On<string, string>("disconnectToUnity", OnClientDisconnect);
            manager.Socket.On<byte>("readyUp", OnReadyUp);
            manager.Socket.On<byte>("action", OnAction);
            manager.Socket.On<string, string, int, float, int>("syncCustomizationsFromServer", SyncCustomizations);
            manager.Socket.On<byte>("requestPlayerPosToHost", SyncPlayerPos);

            DontDestroyOnLoad(gameObject);
        }

        //InvokeRepeating("SyncAllPlayerPosWithLerp", 1, 0.5f);
    }

    private void Update()
    {
        foreach (ClientPlayer cp in players)
        {
            if (cp == null)
                Debug.Log("WE GOT A NULL");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");

        manager.Socket.Emit("unityCloseRoom", passcode);
        manager?.Close();
        manager?.Socket?.Disconnect();
    }

    public void OnMinigameStart(string game)
    {
        Socket s = manager.Socket.Emit("minigameStarted", passcode, game);
        StartCoroutine("LoadSceneWithFade", game);
    }

    public void LoadMainMenu()
    {
        StartCoroutine("LoadSceneWithFade", "MainMenu");
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        //Fade out
        if (postProcessingProfile.TryGet<ColorAdjustments>(out ColorAdjustments ca))
        {
            for (int i = 0; i < 60; i++)
            {
                float t = (float)i / (float)60;
                ca.postExposure.value = Mathf.Lerp(0, -10, t);
                yield return new WaitForSeconds(1 / 60);
            }
        }

        SceneManager.LoadScene(sceneName);
    }

    private void FadeInScene(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        //Fade in
        if (postProcessingProfile.TryGet<ColorAdjustments>(out ColorAdjustments ca))
        {
            for (int i = 0; i < 60; i++)
            {
                float t = (float)i / (float)60;
                ca.postExposure.value = Mathf.Lerp(-10, 0, t);
                yield return new WaitForSeconds(1 / 60);
            }
        }
    }

    public event Action<GameObject> onClientConnect;
    private void OnClientConnect(string id, string ip, string name)
    {
        Debug.Log("Client connected with ID: " + id + " at IP address: " + ip);

        manager.Socket.Emit("playerInfoFromHost", id, SerializePlayersInfo());

        ClientPlayer newPlayer = Instantiate(playerPrefab).GetComponent<ClientPlayer>();

        players.Add(newPlayer);
        newPlayer.PlayerSocketID = id;
        newPlayer.PlayerByteID = playerCounter;
        manager.Socket.Emit("assignPlayerByteIDServer", id, playerCounter);
        playerCounter++;
        newPlayer.PlayerIP = ip;
        newPlayer.PlayerName = name;


        if (onClientConnect != null)
        {
            onClientConnect(newPlayer.gameObject);
        }
    }

#if UNITY_EDITOR
    private void SpawnDebugPlayer(string id, string name)
    {
        ClientPlayer newPlayer = Instantiate(playerPrefab).GetComponent<ClientPlayer>();
        
        StartCoroutine("AddDebugDelayed", id);

        players.Add(newPlayer);
        newPlayer.PlayerSocketID = id;
        newPlayer.PlayerByteID = playerCounter;
        playerCounter++;

        newPlayer.PlayerName = name;

        if (onClientConnect != null)
        {
            onClientConnect(newPlayer.gameObject);
        }
    }

    //Not sure why this is necessary but doesn't work unless delayed... cool
    IEnumerator AddDebugDelayed(string id)
    {
        yield return new WaitForSeconds(1);
        GetPlayerBySocketID(id).PlayerIP = "test";
        GetPlayerBySocketID(id).isDebugPlayer = true;
        GetPlayerBySocketID(id).gameObject.AddComponent<ClientPlayerDebug>();
    }
#endif

    public event Action<string> onClientDisonnect;
    private void OnClientDisconnect(string id, string ip)
    {
        Debug.Log("Client disconnected with ID: " + id + " at IP address: " + ip);

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

    private void OnInputReceived(byte[] data)
    {
        ClientInputData newData = ClientPlayer.DeserializeInputData(data);
        GetPlayerByByteID(newData.id).Move(newData.input);
    }

    private void SyncCustomizations(string id, string color, int headShape, float height, int hatIndex)
    {
        GetPlayerBySocketID(id).SetCustomizations(color, headShape, height, hatIndex);
    }

    public void SyncPlayerPos(byte id)
    {
        ClientPlayer cp = GetPlayerByByteID(id);
        manager.Socket.Emit("syncPlayerPosFromHost", cp.PlayerByteID, cp.transform.position.x, cp.transform.position.y, cp.transform.position.z, false);
    }

    public void SyncAllPlayerPos()
    {
        foreach (ClientPlayer cp in players)
        {
            manager.Socket.Emit("syncPlayerPosFromHost", cp.PlayerByteID, cp.transform.position.x, cp.transform.position.y, cp.transform.position.z, false);
        }
    }

    public void SyncAllPlayerPosWithLerp()
    {
        foreach (ClientPlayer cp in players)
        {
            manager.Socket.Emit("syncPlayerPosFromHost", cp.PlayerByteID, cp.transform.position.x, cp.transform.position.y, cp.transform.position.z, true);
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

    private void OnAction(byte id)
    {
        if (GetPlayerByByteID(id))
        {
            GetPlayerByByteID(id).Action();
        }
    }

    public void SpawnPlayers(GameObject prefab, Transform[] locations, bool parentToLocation = false)
    {
        for (int i = 0; i < players.Count; i++)
        {
            string socketID = players[i].PlayerSocketID;
            string playerIP = players[i].PlayerIP;
            byte byteID = players[i].PlayerByteID;
            string playerName = players[i].PlayerName;
            Color playerColor = players[i].PlayerColor;
            int playerHeadType = players[i].PlayerHeadType;
            float playerHeight = players[i].PlayerHeight;
            int hatIndex = players[i].PlayerHatIndex;

#if UNITY_EDITOR
            bool isDebug = players[i].isDebugPlayer;
#endif

            Destroy(players[i].gameObject);
            players[i] = Instantiate(prefab).GetComponent<ClientPlayer>();
            players[i].PlayerSocketID = socketID;
            players[i].PlayerIP = playerIP;
            players[i].PlayerByteID = byteID;
            players[i].PlayerName = playerName;
            players[i].PlayerColor = playerColor;
            players[i].PlayerHeadType = playerHeadType;
            players[i].PlayerHeight = playerHeight;
            players[i].PlayerHatIndex = hatIndex;

            if (locations[i])
            {
                if (parentToLocation)
                {
                    players[i].transform.SetParent(locations[i]);
                    players[i].transform.localRotation = Quaternion.identity;
                    //players[i].transform.localScale = Vector3.one;
                    players[i].transform.localPosition = Vector3.zero;
                }
                else
                {
                    players[i].transform.position = locations[i].position;
                }
            }

#if UNITY_EDITOR
            if(isDebug)
            {
                StartCoroutine("AddDebugDelayed", socketID);
            }
#endif
        }

        onPlayerSpawned.Invoke();
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

    public byte[] SerializePlayersInfo()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write((byte)players.Count);
                foreach (ClientPlayer cp in players)
                {
                    writer.Write(cp.PlayerSocketID);
                    writer.Write(cp.PlayerByteID);
                    writer.Write(cp.PlayerIP);
                    writer.Write(cp.PlayerName);
                    writer.Write("#" + ColorUtility.ToHtmlStringRGB(cp.PlayerColor));
                    writer.Write((sbyte)cp.PlayerHeadType);
                    writer.Write(cp.PlayerHeight);
                    writer.Write((sbyte)cp.PlayerHatIndex);
                }
            }
            return m.ToArray();
        }
    }
}