using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Cinemachine;

public class ChaseGameManagerWeb : GameManagerWeb
{
    [SerializeField] CinemachineFreeLook playerCamera;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
        playerCamera.Follow = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        playerCamera.LookAt = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                SetPlayerMovement(false);
                break;
            case GameState.Countdown:
                if (!countingDown)
                {
                    StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                }
                break;
            case GameState.GameLoop:
                SetPlayerMovement(true);
                countingDown = false;
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                /*
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    State = GameState.TimeEnded;
                }
                */
                break;
            case GameState.PlayerCaptured:
                StartCoroutine(GameOver(2, "PLAYER\nCAPTURED!"));
                break;
            case GameState.TimeEnded:
                StartCoroutine(GameOver(2, "TIMES UP!"));
                break;
            default:
                break;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        countingDown = true;

        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "GO!";

        yield return new WaitForSeconds(1);
        countdownText.enabled = false;

        //State = GameState.GameLoop;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        //State = GameState.GameOver;
        countdownText.enabled = true;
        countdownText.text = txt;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("MainMenuClient");
    }
}
