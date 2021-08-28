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

    private const int COUNTDOWN_AMOUNT = 5;
    [SerializeField] private TMP_Text countdownText;
    private bool countingDown = false;

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
                break;
            case GameState.PlayerCaptured:
                StartCoroutine("GameOver", 2);
                break;
            case GameState.TimeEnded:
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

    IEnumerator GameOver(int countdown)
    {
        state = GameState.GameOver;

        countdownText.enabled = true;
        countdownText.text = "PLAYER\nCAPTURED";
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
}
