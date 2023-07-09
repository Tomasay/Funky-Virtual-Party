using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;

[System.Serializable]
public class MyStringEvent : UnityEvent<string>
{
}

public class ChaseGameSyncer : RealtimeComponent<ChaseGameSyncModel>
{
    public static ChaseGameSyncer instance;

    public MyStringEvent OnStateChangeEvent;

    public string State { get => model.state; set => model.state = value; }
    public bool VRPlayerReady { get => model.vrPlayerReady; set => model.vrPlayerReady = value; }

    [SerializeField] private TutorialMenu tutorial;
    [SerializeField] private TutorialMenuClient tutorialWeb;

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
        tutorial.allPlayersReady.AddListener(delegate { State = "countdown"; });
    }

    private void OnDestroy()
    {
        tutorial.allPlayersReady.RemoveListener(delegate { State = "countdown"; });
    }
#endif

    protected override void OnRealtimeModelReplaced(ChaseGameSyncModel previousModel, ChaseGameSyncModel currentModel)
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

            //Update to match new data
            if (model.vrPlayerReady && !isWeb)
            {
                tutorial.ReadyUpVR();
            }
            else if(model.vrPlayerReady && isWeb)
            {
                tutorialWeb.ReadyUpVR();
            }

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.vrPlayerReadyDidChange += OnVRPlayerReadyUp;
        }
    }

#region Variable Callbacks
    void OnStateChange(ChaseGameSyncModel previousModel, string val)
    {
        Debug.Log("State changed to: " + val + "   previous model: " + previousModel.state);

        OnStateChangeEvent.Invoke(val);
    }

    void OnVRPlayerReadyUp(ChaseGameSyncModel previousModel, bool val)
    {
        if (val && !isWeb)
        {
            tutorial.ReadyUpVR();
        }
        else if (val && isWeb)
        {
            tutorialWeb.ReadyUpVR();
        }
    }
#endregion
}
