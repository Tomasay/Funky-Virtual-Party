using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;

public class KaijuGameSyncer : RealtimeComponent<KaijuGameSyncModel>
{
    public static KaijuGameSyncer instance;

    public MyStringEvent OnStateChangeEvent;

    public string State { get => model.state; set => model.state = value; }
    public bool VRPlayerReady { get => model.vrPlayerReady; set => model.vrPlayerReady = value; }

    public int PlayerGrabbedEvent { get => model.grabbedPlayerEvent; set => model.grabbedPlayerEvent = value; }
    public int PlayerDroppedEvent { get => model.droppedPlayerEvent; set => model.droppedPlayerEvent = value; }

    private bool isWeb;

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
        PlayerGrabbedEvent = -1;
        PlayerDroppedEvent = -1;

        TutorialMenu.instance.allPlayersReady.AddListener(delegate { State = "countdown"; });
    }

    private void OnDestroy()
    {
        TutorialMenu.instance.allPlayersReady.RemoveListener(delegate { State = "countdown"; });
    }
#endif

    protected override void OnRealtimeModelReplaced(KaijuGameSyncModel previousModel, KaijuGameSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.stateDidChange -= OnStateChange;
            previousModel.vrPlayerReadyDidChange -= OnVRPlayerReadyUp;
            previousModel.grabbedPlayerEventDidChange -= OnPlayerGrabbedEventDidFire;
            previousModel.droppedPlayerEventDidChange -= OnPlayerDroppedEventDidFire;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.state = "tutorial";
                currentModel.grabbedPlayerEvent = -1;
                currentModel.droppedPlayerEvent = -1;
            }

            //Update to match new data
            if (model.vrPlayerReady && !isWeb)
            {
                TutorialMenu.instance.ReadyUpVR();
            }
            else if (model.vrPlayerReady && isWeb)
            {
                TutorialMenuClient.instance.ReadyUpVR();
            }

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.vrPlayerReadyDidChange += OnVRPlayerReadyUp;
            currentModel.grabbedPlayerEventDidChange += OnPlayerGrabbedEventDidFire;
            currentModel.droppedPlayerEventDidChange += OnPlayerDroppedEventDidFire;
        }
    }

    #region Variable Callbacks
    void OnStateChange(KaijuGameSyncModel previousModel, string val)
    {
        OnStateChangeEvent.Invoke(val);
    }

    void OnVRPlayerReadyUp(KaijuGameSyncModel previousModel, bool val)
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

    void OnPlayerGrabbedEventDidFire(KaijuGameSyncModel previousModel, int id)
    {
        Debug.Log("Grabbed syncer level " + id);
        ClientPlayer cp = ClientPlayer.GetClientByInitialOwnerID(id);
        if (cp)
        {
            Debug.Log("CP: " + cp.realtimeView.ownerIDSelf);
            (cp as KaijuGameClientPlayer).Grabbed();
        }

        Invoke("ResetPlayerGrabbedEvent", 1);
    }

    void ResetPlayerGrabbedEvent()
    {
        PlayerGrabbedEvent = -1;
    }

    void OnPlayerDroppedEventDidFire(KaijuGameSyncModel previousModel, int id)
    {
        Debug.Log("Dropped syncer level " + id);
        ClientPlayer cp = ClientPlayer.GetClientByInitialOwnerID(id);
        if (cp)
        {
            (cp as KaijuGameClientPlayer).Dropped();
        }

        Invoke("ResetPlayerDroppedEvent", 1);
    }

    void ResetPlayerDroppedEvent()
    {
        PlayerDroppedEvent = -1;
    }
    #endregion
}
