using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Cinemachine;
using DG.Tweening;

public class ChaseGameManagerWeb : GameManagerWeb
{
    [SerializeField] CinemachineFreeLook playerCamera;
    [SerializeField] Camera cam;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    [SerializeField] private ParticleSystem countdownParticles, playerCapturedParticles;
    private float timeRemaining;

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
        playerCamera.Follow = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        playerCamera.LookAt = ClientManagerWeb.instance.LocalPlayer.Anim.transform;

        (ClientManagerWeb.instance.LocalPlayer as ChaseGameClientPlayer).cam = cam;
    }

    protected override void OnStateChange(string s)
    {
        base.OnStateChange(s);

        switch (State)
        {
            case GameState.Tutorial:
                SetPlayerMovement(false);
                break;
            case GameState.Countdown:
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case GameState.GameLoop:
                SetPlayerMovement(true);
                break;
            case GameState.VRPlayerLoses:
                playerCapturedParticles.Play();
                StartCoroutine(GameOver(2, "PLAYER\nCAPTURED!"));
                break;
            case GameState.VRPlayerWins:
                break;
            case GameState.TimeEnded:
                StartCoroutine(GameOver(2, "TIMES UP!"));
                break;
            default:
                break;
        }
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                break;
            case GameState.Countdown:
                break;
            case GameState.GameLoop:
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                break;
            case GameState.VRPlayerLoses:
                break;
            case GameState.TimeEnded:
                break;
            default:
                break;
        }
    }

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

        //State = GameState.GameLoop;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        //State = GameState.GameOver;
        countdownText.enabled = true;
        countdownText.text = txt;
        yield return new WaitForSeconds(3);

        ClientManagerWeb.instance.LoadMainMenu();
    }
}