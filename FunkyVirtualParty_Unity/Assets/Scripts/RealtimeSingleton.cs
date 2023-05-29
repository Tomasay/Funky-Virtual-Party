using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(Realtime))]
public class RealtimeSingleton : MonoBehaviour
{
    public static RealtimeSingleton instance;

    Realtime realtime;
    RealtimeAvatarManager realtimeAvatarManager;

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

        DontDestroyOnLoad(gameObject);
    }
}