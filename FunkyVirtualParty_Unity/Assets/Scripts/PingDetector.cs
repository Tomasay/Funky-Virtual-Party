using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class PingDetector : MonoBehaviour
{
    [SerializeField]
    GameObject highLatencyIcon;

    private int highPingThreshold = 100;

    private float lastPing;

    public static PingDetector Instance;

    void Start()
    {
        //Only need singleton on webgl. On Android, ping detector UI is in the XR Player base prefab and is recreated in each scene
#if UNITY_WEBGL
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
#endif

        InvokeRepeating("CheckPing", 3, 3);
    }

    public void CheckPing()
    {
        if (RealtimeSingletonWeb.instance.Realtime.connected)
        {
            lastPing = RealtimeSingletonWeb.instance.Realtime.ping;
            highLatencyIcon.SetActive(lastPing >= highPingThreshold);
        }
    }
}