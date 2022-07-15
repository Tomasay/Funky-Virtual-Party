using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Cinemachine;
using Digger.Modules.Core.Sources;

public class ShootoutGameManagerWeb : GameManagerWeb
{
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    [SerializeField] DiggerSystem digger;

    private void Awake()
    {
#if UNITY_WEBGL
            ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
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
                break;
            case GameState.VRPlayerWins:
                if (!endingGame)
                {
                    StartCoroutine(GameOver(2, "YOU LOSE!"));
                    endingGame = true;
                }
                break;
            case GameState.TimeEnded:
                if (!endingGame)
                {
                    StartCoroutine(GameOver(2, "TIMES UP!"));
                    endingGame = true;
                }
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
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        countdownText.enabled = true;
        countdownText.text = txt;
        yield return new WaitForSeconds(3);

        ClientManagerWeb.instance.LoadMainMenu();
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("GenerateVoxels"))
        {
            StartCoroutine("GenerateVoxelsCorouting", data);
        }
    }

    IEnumerator GenerateVoxelsCorouting(string data)
    {
        Debug.Log("Coroutine starting");
        yield return new WaitForSeconds(0);

        UpdateVoxelsData newData = JsonUtility.FromJson<UpdateVoxelsData>(data);
        VoxelChunk.GenerateVoxels(digger, newData.heightarray, newData.chunkAltitude, ref newData.voxelArray);
    }
}