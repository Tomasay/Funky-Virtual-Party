using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Normal.Realtime;

public class ChaseGameManager : MonoBehaviour
{
    [SerializeField]
    RealtimeView[] mapRealtimeViews;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    private TMP_Text vrInfoText, vrGameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    protected void Start()
    {
        timeRemaining = GAME_TIME_AMOUNT;
        
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;

        ChaseGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrInfoText = avatar.GetComponent<ChaseGameVRPlayerController>().vrInfoText;

        vrGameTimeText = avatar.GetComponent<ChaseGameVRPlayerController>().vrGameTimeText;
        vrGameTimeText.text = FormatTime(timeRemaining);

        SetVRPlayerMovement(false);
        SetVRPlayerHandMovement(false);
    }

    void Update()
    {
        switch (ChaseGameSyncer.instance.State)
        {
            case "tutorial":
                
                break;
            case "countdown":
                if (!countingDown)
                {
                    StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                }
                break;
            case "game loop":
                timeRemaining -= Time.deltaTime;
                vrGameTimeText.text = FormatTime(timeRemaining);
                if (timeRemaining <= 10) //Display final 10 seconds for VR player
                {
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                }
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    ChaseGameSyncer.instance.State = "time ended";
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                }
                break;
            case "vr player lost":
                StartCoroutine(GameOver(2, "YOU'VE BEEN CAPTURED"));
                break;
            case "time ended":
                StartCoroutine(GameOver(2, "TIMES UP!\nYOU WIN"));
                break;
            default:
                break;
        }
    }

    void OnStateChanged(string newState)
    {
        switch (newState)
        {
            case "tutorial":
                break;
            case "countdown":
                //Get ownership of map buttons. Waiting till countdown to ensure we are properly connected
                foreach (RealtimeView rt in mapRealtimeViews)
                {
                    rt.RequestOwnership();
                }
                break;
            case "game loop":
                break;
            case "vr player lost":
                break;
            case "time ended":
                break;
            default:
                break;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        countingDown = true;
        SetVRPlayerMovement(true);
        SetVRPlayerHandMovement(true);
        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        vrInfoText.text = "RUN AWAY!";
        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        ChaseGameSyncer.instance.State = "game loop";

        countingDown = false;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        vrInfoText.text = txt;
        yield return new WaitForSeconds(3);

        SceneChangerSyncer.instance.CurrentScene = "MainMenu";
    }
    public void DisplayVRCapture(string playerName)
    {
        HapticsManager.instance.TriggerHaptic(false, 2);
        HapticsManager.instance.TriggerHaptic(true, 2);
        vrInfoText.text = playerName + " captured you!";
    }

    private void SetVRPlayerHandMovement(bool enabled)
    {
        RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<ChaseGameVRPlayerController>().HandMovement = enabled;
    }

    protected void SetVRPlayerMovement(bool canPlayerMove)
    {
        RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<AutoHandPlayer>().useMovement = canPlayerMove;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}