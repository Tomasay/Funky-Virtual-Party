using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class KaijuGameManagerWeb : MonoBehaviour
{  
    private const int PHASE_COUNT = 2, COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 50;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    [SerializeField] CinemachineFreeLook cinemachineCam;
    [SerializeField] Camera cam;

    [SerializeField] GameObject playerGrabAnchorLeft, playerGrabAnchorRight;

    // Start is called before the first frame update
    protected void Start()
    {
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);

        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerIndicatorVisibility(true);

        (RealtimeSingletonWeb.instance.LocalPlayer as KaijuGameClientPlayer).cam = cam;

        cinemachineCam.Follow = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        cinemachineCam.LookAt = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
    }

    protected void OnStateChange(string s)
    {
        switch (ShootoutGameSyncer.instance.State)
        {
            case "tutorial":

                break;
            case "countdown":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case "game loop":
            case "prepare":
            case "fight":
            case "kill":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = true;
                break;
            case "vr player won":
                countdownText.enabled = true;
                countdownText.text = "VR PLAYER WINS";
                break;
            case "time ended":
                countdownText.enabled = true;
                countdownText.text = "TIME'S UP!\nYOU WIN";
                break;
            default:
                break;
        }
    }

    void Update()
    {
        switch (KaijuGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                break;
            case "game loop":
            case "prepare":
            case "fight":
            case "kill":
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                break;
            case "vr player won":
                break;
            case "time ended":
                break;
            default:
                break;
        }
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
