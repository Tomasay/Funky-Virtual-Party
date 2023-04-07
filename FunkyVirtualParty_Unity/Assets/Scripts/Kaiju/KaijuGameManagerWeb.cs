using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class KaijuGameManagerWeb : GameManagerWeb
{
    public KaijuGameLoopState KaijuGameState = KaijuGameLoopState.Not;  
    private const int PHASE_COUNT = 2, COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 50;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    [SerializeField] CinemachineFreeLook cinemachineCam;
    [SerializeField] Camera cam;


    private void Awake()
    {
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
    }

    protected  override void OnDisable()
    {
#if UNITY_WEBGL
        base.OnDisable();

        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClient");
#endif
    }

    void MethodCalledFromServer(string methodName, string data)
    {

    }

    IEnumerator GameOver(int countdown, string gameOverMessage)
    {
        countdownText.enabled = true;
        countdownText.text = gameOverMessage;
        yield return new WaitForSeconds(5);

        ClientManagerWeb.instance.LoadMainMenu();
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
        SetPlayerMovement(true);

        //cinemachineCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_YDamping = 1;
        //cinemachineCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_ZDamping = 1;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);

        ClientManagerWeb.instance.LocalPlayer.SetPlayerIndicatorVisibility(true);

        (ClientManagerWeb.instance.LocalPlayer as KaijuGameClientPlayer).cam = cam;

        cinemachineCam.Follow = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        cinemachineCam.LookAt = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
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
                SetPlayerMovement(true);
                switch (KaijuGameState)
                {
                    case KaijuGameLoopState.Not:
                        KaijuGameState = KaijuGameLoopState.Prepare; 
                        break;
                    case KaijuGameLoopState.Prepare:
                        countingDown = false;
                        if (timeRemaining > 0) { timeRemaining -= Time.deltaTime; } else { timeRemaining = GAME_TIME_AMOUNT; KaijuGameState = KaijuGameLoopState.Fight; }
                        gameTimeText.text = FormatTime(timeRemaining);
                        break;
                    case KaijuGameLoopState.Fight:
                        countingDown = false;
                        if (timeRemaining > 0) { timeRemaining -= Time.deltaTime; } else { State = GameState.TimeEnded; }
                        gameTimeText.text = FormatTime(timeRemaining);
                        break;
                    case KaijuGameLoopState.Kill:
                        countingDown = false;
                        if (timeRemaining > 0) { timeRemaining -= Time.deltaTime; }
                        gameTimeText.text = FormatTime(timeRemaining);
                        break;
                }
                break;
            case GameState.VRPlayerWins:
                if (!endingGame)
                {
                    StartCoroutine(GameOver(2, "KAIJU DEFEATED!\nYOU WIN!"));
                    endingGame = true;
                }
                break;
            case GameState.TimeEnded:
                if (!endingGame)
                {
                    StartCoroutine(GameOver(2, "YOU LOSE!"));
                    endingGame = true;
                }
                break;
            default:
                break;
        }
    }
}
