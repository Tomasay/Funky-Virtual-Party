using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Realtime))]
public class RealtimeSingleton : MonoBehaviour
{
    public static RealtimeSingleton instance;

    Realtime realtime;
    public RealtimeAvatarManager realtimeAvatarManager;

    public string[] vinylDiscNames;

    public string[] debugPlayers;

    public List<GameObject> discs;

    public Realtime Realtime { get => realtime; }
    public RealtimeAvatarManager RealtimeAvatarManager { get => realtimeAvatarManager;}
    public RealtimeAvatar VRAvatar { get => realtimeAvatarManager.avatars[0]; }

    private void Awake()
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

        realtime = GetComponent<Realtime>();
        realtimeAvatarManager = GetComponent<RealtimeAvatarManager>();

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        realtime.didConnectToRoom += Realtime_didConnectToRoom;

        //Initializing client connected/disconnected callbacks so they can be referenced before a client is disconnected
        if(ClientPlayer.OnClientConnected == null)
            ClientPlayer.OnClientConnected = new MyCPEvent();

        if (ClientPlayer.OnClientDisconnected == null)
            ClientPlayer.OnClientDisconnected = new MyCPEvent();

        if (ClientPlayer.OnColorChanged == null)
            ClientPlayer.OnColorChanged = new MyCPEvent();

        if (discs == null)
            discs = new List<GameObject>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (realtime.connected)
        {
            realtimeAvatarManager.localAvatarPrefab = Resources.Load("XRPlayer" + scene.name) as GameObject;
            realtimeAvatarManager.CreateAvatarIfNeeded();

            if (scene.name.Equals("MainMenu"))
            {
                SpawnDiscs();
            }

            if (scene.name.Equals("MainMenu"))
            {
                Invoke("SpawnDebugPlayers", 1);
            }
            else
            {
                SpawnDebugPlayers();
            }
        }
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        SpawnDiscs();

        SpawnDebugPlayers();
    }

    /// <summary>
    /// Instantiates the discs for each game. Needs to occur when joining a room, and whenever the main menu is re-entered
    /// </summary>
    void SpawnDiscs()
    {
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.useInstance = realtime;
        options.ownedByClient = true;

        foreach (string s in vinylDiscNames)
        {
            discs.Add(Realtime.Instantiate("Vinyls/Vinyl_" + s, options));
        }
    }

    void SpawnDebugPlayers()
    {
        //Delete and save reference to any old debug client players
        List<ClientPlayer> oldDebugClientPlayers = new List<ClientPlayer>();
        if (ClientPlayer.clients != null)
        {
            foreach (ClientPlayer cp in ClientPlayer.clients)
            {
                if (cp.syncer.IsDebugPlayer)
                {
                    oldDebugClientPlayers.Add(cp);
                }
            }
        }

        //Spawn new debug client players
        ClientPlayer.debugPlayerCount = 10;
        for (int i = 0; i < debugPlayers.Length; i++)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            GameObject newPlayer = Realtime.Instantiate(currentScene + "ClientPlayer", Realtime.InstantiateOptions.defaults);
            ClientPlayer newPlayerCP = newPlayer.GetComponent<ClientPlayer>();

            newPlayerCP.syncer.IsDebugPlayer = true;
            newPlayerCP.IsLocal = true;

            if (oldDebugClientPlayers.Count > i) //If player has already been spawned before
            {
                newPlayerCP.syncer.Name = oldDebugClientPlayers[i].syncer.Name;
                newPlayerCP.SetCustomizations(oldDebugClientPlayers[i].syncer.Color, oldDebugClientPlayers[i].syncer.HeadType, oldDebugClientPlayers[i].syncer.Height, oldDebugClientPlayers[i].syncer.HatIndex);

                Realtime.Destroy(oldDebugClientPlayers[i].gameObject);
            }
            else
            {
                newPlayerCP.syncer.Name = debugPlayers[i];
                newPlayerCP.InitialCustomize();
            }
        }
    }
}