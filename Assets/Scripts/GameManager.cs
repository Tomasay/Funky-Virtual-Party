using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Transform[] playerSpawns;

    [SerializeField] GameObject VRPlayer;

    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text countdownText, vrInfoText, gameTimeText, vrGameTimeText;
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
        ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns);
        State = GameState.Tutorial;
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
        vrGameTimeText.text = FormatTime(timeRemaining);
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                SetPlayerMovement(false);
                SetVRPlayerMovement(false);
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
                vrGameTimeText.text = FormatTime(timeRemaining);
                if (timeRemaining <= 10) //Display final 10 seconds for VR player
                {
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                }
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    State = GameState.TimeEnded;
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
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

        vrInfoText.text = "RUN!";
        SetVRPlayerMovement(true);
        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "GO!";
        
        yield return new WaitForSeconds(1);
        countdownText.enabled = false;
        vrInfoText.text = "";

        State = GameState.GameLoop;
        SetPlayerMovement(true);

        countingDown = false;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        state = GameState.GameOver;

        vrInfoText.text = "YOU WIN!";
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

    private void SetVRPlayerMovement(bool canPlayerMove)
    {
        VRPlayer.GetComponentInChildren<AutoHandPlayer>().canMove = canPlayerMove;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes*60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void DisplayVRCapture(string playerName)
    {
        HapticsManager.instance.TriggerHaptic(false, 2);
        HapticsManager.instance.TriggerHaptic(true, 2);
        vrInfoText.text = playerName + " captured you!";
    }
}