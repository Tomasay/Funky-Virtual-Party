using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;

public class MazeGameSyncer : RealtimeComponent<MazeGameSyncModel>
{
    public static MazeGameSyncer instance;

    public MyStringEvent OnStateChangeEvent;

    public GameObject maze;

    public GameObject clientMarble;
    private Vector3 latestMarblePos;

    public string State { get => model.state; set => model.state = value; }
    public bool VRPlayerReady { get => model.vrPlayerReady; set => model.vrPlayerReady = value; }
    public Vector3 MarbleMazePos { get => model.marbleMazePos; set => model.marbleMazePos = value; }

    private bool isWeb;

    private void Awake()
    {
        //Singleton
        instance = this;

        if (OnStateChangeEvent == null)
            OnStateChangeEvent = new MyStringEvent();

        latestMarblePos = clientMarble.transform.localPosition;

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

    private void Update()
    {
        if (clientMarble)
        {
            clientMarble.transform.localPosition = Vector3.Lerp(clientMarble.transform.localPosition, latestMarblePos, Time.deltaTime * 10);
        }
    }

    private void OnDestroy()
    {
        TutorialMenu.instance.allPlayersReady.RemoveListener(delegate { State = "countdown"; });
    }
#endif

    protected override void OnRealtimeModelReplaced(MazeGameSyncModel previousModel, MazeGameSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.stateDidChange -= OnStateChange;
            previousModel.vrPlayerReadyDidChange -= OnVRPlayerReadyUp;
            previousModel.marbleMazePosDidChange -= OnMarbleMazePosDidChange;
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
            currentModel.marbleMazePosDidChange += OnMarbleMazePosDidChange;
        }
    }

    #region Variable Callbacks
    void OnStateChange(MazeGameSyncModel previousModel, string val)
    {
        OnStateChangeEvent.Invoke(val);
    }

    void OnVRPlayerReadyUp(MazeGameSyncModel previousModel, bool val)
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

    void OnMarbleMazePosDidChange(MazeGameSyncModel previousModel, Vector3 newPos)
    {
        if(clientMarble)
        {
            latestMarblePos = newPos;
        }
    }
    #endregion
}