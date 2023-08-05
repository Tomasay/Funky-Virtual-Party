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
        }
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        SpawnDiscs();
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
            Realtime.Instantiate("Vinyls/Vinyl_" + s, options);
        }
    }
}