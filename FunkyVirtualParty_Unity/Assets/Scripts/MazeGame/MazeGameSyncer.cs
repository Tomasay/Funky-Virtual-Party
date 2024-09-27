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
    public UnityEvent<int> OnCoinsToGoChangeEvent;

    public GameObject maze;

    public GameObject clientMarble;
    private Vector3 latestMarblePos;

    public string State { get => model.state; set => model.state = value; }
    public bool VRPlayerReady { get => model.vrPlayerReady; set => model.vrPlayerReady = value; }
    public Vector3 MarbleMazePos { get => model.marbleMazePos; set => model.marbleMazePos = value; }
    public int CoinsToGo { get => model.coinsToGo; set => model.coinsToGo = value; }

    private bool isWeb;

    private void Awake()
    {
        //Singleton
        instance = this;

        if (OnStateChangeEvent == null)
            OnStateChangeEvent = new MyStringEvent();

        if (OnCoinsToGoChangeEvent == null)
            OnCoinsToGoChangeEvent = new UnityEvent<int>();

#if UNITY_WEBGL
        isWeb = true;

        if (clientMarble)
        {
            latestMarblePos = clientMarble.transform.localPosition;
        }
#endif
    }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN //Only host has to worry about triggering allPlayersReady event
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

#if UNITY_WEBGL
    private void Update()
    {
        if (clientMarble)
        {
            clientMarble.transform.localPosition = Vector3.Lerp(clientMarble.transform.localPosition, latestMarblePos, Time.deltaTime * 10);
        }
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
            previousModel.coinsToGoDidChange -= OnCoinsToGoDidChange;
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
            currentModel.coinsToGoDidChange += OnCoinsToGoDidChange;
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

    void OnCoinsToGoDidChange(MazeGameSyncModel previousModel, int coins)
    {
        OnCoinsToGoChangeEvent.Invoke(coins);

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        MazeGameVRPlayerController vrPlayer = RealtimeSingleton.instance.VRAvatar.GetComponent<MazeGameVRPlayerController>();
        vrPlayer.vrCoinCounterText.text = "Coins: <color=yellow>" + coins + "</color> to go";
        vrPlayer.vrCoinCounterText.GetComponent<Animator>().SetTrigger("PulsateOnce");
#endif
    }
    #endregion
}