using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using DG.Tweening;

public class KaijuGameSyncerWeb : RealtimeComponent<KaijuGameSyncModel>
{
    public static KaijuGameSyncerWeb instance;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;

    private float timeRemaining;

    [SerializeField] private TMP_Text countdownText, gameTimeText;

    [SerializeField] private ParticleSystem countdownParticles;

    public string State { get => model.state; set => model.state = value; }

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
    }

    protected override void OnRealtimeModelReplaced(KaijuGameSyncModel previousModel, KaijuGameSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.stateDidChange -= OnStateChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.state = "tutorial";
            }

            //Update to match new data
            if (model.vrPlayerReady) TutorialMenuClient.instance.ReadyUpVR();

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.vrPlayerReadyDidChange += OnVRPlayerReadyUp;
        }
    }

    private void Update()
    {
        if (State.Equals("game loop"))
        {
            timeRemaining -= Time.deltaTime;
            gameTimeText.text = FormatTime(timeRemaining);
        }
    }

    #region Variable Callbacks
    void OnStateChange(KaijuGameSyncModel previousModel, string val)
    {
        switch (val)
        {
            case "countdown":
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case "game loop":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = true;
                break;
            default:
                break;
        }
    }

    void OnVRPlayerReadyUp(KaijuGameSyncModel previousModel, bool val)
    {
        Debug.Log("VR PLAYER READIED UP");
        if (val)
        {
            TutorialMenuClient.instance.ReadyUpVR();
        }
    }
    #endregion

    IEnumerator StartCountdownTimer(int countdown)
    {
        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
            countdownText.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            countdownText.transform.DOScale(1, 0.25f);
            yield return new WaitForSeconds(1);
        }

        countdownParticles.Play();
        countdownText.text = "GO!";
        countdownText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        countdownText.transform.DOScale(1, 0.25f);

        yield return new WaitForSeconds(1);
        countdownText.enabled = false;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
