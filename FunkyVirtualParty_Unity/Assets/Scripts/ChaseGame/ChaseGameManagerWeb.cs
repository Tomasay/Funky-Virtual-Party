using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class ChaseGameManagerWeb : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook playerCamera;
    [SerializeField] Camera cam;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    [SerializeField] private ParticleSystem countdownParticles;
    private float timeRemaining;

    public void DebugLog(string log)
    {
        Debug.Log((log));
    }

    private void Awake()
    {
        NormcoreRoomConnector.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);

        ChaseGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);
    }

    protected void Start()
    {
        NormcoreRoomConnector.instance.LocalPlayer.CanMove = false;

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
    }

    void OnLocalPlayerSpawned()
    {
        playerCamera.Follow = NormcoreRoomConnector.instance.LocalPlayer.Anim.transform;
        playerCamera.LookAt = NormcoreRoomConnector.instance.LocalPlayer.Anim.transform;

        (NormcoreRoomConnector.instance.LocalPlayer as ChaseGameClientPlayer).cam = cam;
    }

    protected void OnStateChange(string s)
    {
        switch (ChaseGameSyncer.instance.State)
        {
            case "tutorial":
                
                break;
            case "countdown":
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case "game loop":
                NormcoreRoomConnector.instance.LocalPlayer.CanMove = true;
                break;
            case "vr player loses":
                NormcoreRoomConnector.instance.VRAvatar.GetComponent<ChaseGameVRPlayerController>().capturedParticles.Play();
                StartCoroutine(GameOver(2, "PLAYER\nCAPTURED!"));
                break;
            case "vr player wins":
                break;
            case "time ended":
                StartCoroutine(GameOver(2, "TIMES UP!"));
                break;
            default:
                break;
        }
    }

    void Update()
    {
        switch (ChaseGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                break;
            case "game loop":
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                break;
            case "vr player loses":
                break;
            case "time ended":
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

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}