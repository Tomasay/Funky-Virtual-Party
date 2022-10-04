using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;

public class ChaseGameManager : GameManager
{
    private const int COUNTDOWN_AMOUNT = 10, GAME_TIME_AMOUNT = 60;
    [SerializeField] private TMP_Text vrInfoText, vrGameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        vrGameTimeText.text = FormatTime(timeRemaining);
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                SetPlayerMovement(false);
                SetVRPlayerMovement(false);
                SetVRPlayerHandMovement(false);
                break;
            case GameState.Countdown:
                if (!countingDown)
                {
                    StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                }
                break;
            case GameState.GameLoop:
                timeRemaining -= Time.deltaTime;
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
            case GameState.VRPlayerLoses:
                StartCoroutine(GameOver(2, "YOU'VE BEEN CAPTURED"));
                break;
            case GameState.TimeEnded:
                StartCoroutine(GameOver(2, "TIMES UP!\nYOU WIN"));
                break;
            default:
                break;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        countingDown = true;

        vrInfoText.text = "RUN AWAY!";
        SetVRPlayerMovement(true);
        SetVRPlayerHandMovement(true);
        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        State = GameState.GameLoop;
        SetPlayerMovement(true);

        countingDown = false;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        //State = GameState.GameOver;

        vrInfoText.text = txt;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("MainMenu");
    }
    public void DisplayVRCapture(string playerName)
    {
        HapticsManager.instance.TriggerHaptic(false, 2);
        HapticsManager.instance.TriggerHaptic(true, 2);
        vrInfoText.text = playerName + " captured you!";
    }

    public override void OnAction(string id)
    {
        ClientManager.instance.GetPlayerByID(id).GetComponent<ChaseGameClientPlayer>().Action();
    }

    private void SetVRPlayerHandMovement(bool enabled)
    {
        VRPlayer.GetComponent<ChaseGameVRPlayerController>().HandMovement = enabled;
    }
}