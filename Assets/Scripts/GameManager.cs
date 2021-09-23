using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Transform[] playerSpawns;

    private const int COUNTDOWN_AMOUNT = 5, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

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
        ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns);
        State = GameState.Tutorial;
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
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
                if(timeRemaining <= 0)
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
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        state = GameState.GameOver;

        countdownText.enabled = true;
        countdownText.text = txt;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator DisableMirroring()
    {
        yield return new WaitForEndOfFrame();
        //UnityEngine.VR.VRSettings.showDeviceView = false;
        XRSettings.showDeviceView = false;
    }

    private void SetPlayerMovement(bool canPlayerMove)
    {
        foreach (ClientPlayer p in ClientManager.instance.Players)
        {
            p.CanMove = canPlayerMove;
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes*60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
