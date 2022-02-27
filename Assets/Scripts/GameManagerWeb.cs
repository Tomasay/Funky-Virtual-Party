using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Cinemachine;

public class GameManagerWeb : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject VRPlayer;

    [SerializeField] CinemachineFreeLook playerCamera;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;
    
    public Vector3 VRPlayerPos { get => VRPlayer.transform.position; }

    public enum GameState
    {
        Tutorial,
        Countdown,
        GameLoop,
        PlayerCaptured,
        TimeEnded,
        GameOver
    }

    private GameState state;
    public GameState State { get => state; set => state = value; }

    void Start()
    {
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab);
        State = GameState.Tutorial;
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
        playerCamera.Follow = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        playerCamera.LookAt = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
    }

    // Update is called once per frame
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
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    State = GameState.TimeEnded;
                }
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

        State = GameState.GameLoop;
        SetPlayerMovement(true);

        countingDown = false;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        state = GameState.GameOver;
        countdownText.enabled = true;
        countdownText.text = txt;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("MainMenuClient");
    }

    private void SetPlayerMovement(bool canPlayerMove)
    {
        foreach (ClientPlayer p in ClientManagerWeb.instance.Players)
        {
            p.CanMove = canPlayerMove;
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}