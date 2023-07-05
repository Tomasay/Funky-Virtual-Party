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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (realtime.connected)
        {
            realtimeAvatarManager.localAvatarPrefab = Resources.Load("XRPlayer" + scene.name) as GameObject;
            realtimeAvatarManager.CreateAvatarIfNeeded();
        }
    }
}