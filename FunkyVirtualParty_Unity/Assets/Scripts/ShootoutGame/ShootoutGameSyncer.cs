using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;

public class ShootoutGameSyncer : RealtimeComponent<ShootoutGameSyncModel>
{
    public static ShootoutGameSyncer instance;

    public MyStringEvent OnStateChangeEvent;

    public string State { get => model.state; set => model.state = value; }
    public bool VRPlayerReady { get => model.vrPlayerReady; set => model.vrPlayerReady = value; }

    private bool isWeb;

    private void Awake()
    {
        //Singleton
        instance = this;

        if (OnStateChangeEvent == null)
            OnStateChangeEvent = new MyStringEvent();

#if UNITY_WEBGL
        isWeb = true;
#endif
    }

#if UNITY_ANDROID //Only host has to worry about triggering allPlayersReady event
    private void Start()
    {
        //Default states when entering scene
        State = "tutorial";
        VRPlayerReady = false;

        TutorialMenu.instance.allPlayersReady.AddListener(delegate { State = "countdown"; });
    }

    private void OnDestroy()
    {
        TutorialMenu.instance.allPlayersReady.RemoveListener(delegate { State = "countdown"; });
    }
#endif

    protected override void OnRealtimeModelReplaced(ShootoutGameSyncModel previousModel, ShootoutGameSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.stateDidChange -= OnStateChange;
            previousModel.vrPlayerReadyDidChange -= OnVRPlayerReadyUp;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.state = "tutorial";
            }

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.vrPlayerReadyDidChange += OnVRPlayerReadyUp;
        }
    }

    #region Variable Callbacks
    void OnStateChange(ShootoutGameSyncModel previousModel, string val)
    {
        OnStateChangeEvent.Invoke(val);
    }

    void OnVRPlayerReadyUp(ShootoutGameSyncModel previousModel, bool val)
    {
        if (val && !isWeb)
        {
            TutorialMenu.instance.ReadyUpVR();
        }
        else if (val && isWeb)
        {
            TutorialMenuClient.instance.ReadyUpVR();
        }
    }
    #endregion
}