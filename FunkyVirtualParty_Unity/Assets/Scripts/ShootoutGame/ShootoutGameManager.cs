using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Digger.Modules.Core.Sources;

public class ShootoutGameManager : GameManager
{
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    [SerializeField] private TMP_Text vrInfoText, vrGameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        vrGameTimeText.text = FormatTime(timeRemaining);

        if (ClientManager.instance)
        {
            DiggerSocketManagerGetter.instance.manager = ClientManager.instance.Manager;

            foreach (ClientPlayer p in ClientManager.instance.Players)
            {
                ShootoutGameClientPlayer sp = (ShootoutGameClientPlayer)p;
                sp.OnDeath.AddListener(CheckPlayersLeft);
            }
        }
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                SetPlayerMovement(false);
                SetVRPlayerMovement(false);
                SetVRPlayerCanThrowFireballs(false);
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

                if (timeRemaining <= 5) //Display final 5 seconds for VR player
                {
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                }
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    State = GameState.TimeEnded;
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                }
                break;
            case GameState.VRPlayerWins:
                StartCoroutine(GameOver(2, "YOU WIN!"));
                break;
            case GameState.TimeEnded:
                StartCoroutine(GameOver(2, "TIMES UP!\nYOU LOSE"));
                break;
            default:
                break;
        }
    }

    void CheckPlayersLeft()
    {
        int playersAlive = 0;
        foreach (ClientPlayer p in ClientManager.instance.Players)
        {
            ShootoutGameClientPlayer sp = (ShootoutGameClientPlayer)p;
            if(sp.isAlive)
            {
                playersAlive++;
            }
        }

        if(playersAlive == 0)
        {
            State = GameState.VRPlayerWins;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        countingDown = true;
        SetVRPlayerCanThrowFireballs(true);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        vrInfoText.text = "GO!";

        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        State = GameState.GameLoop;
        SetPlayerMovement(true);

        countingDown = false;
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        vrInfoText.text = txt;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("MainMenu");
    }

    public override void OnAction(string id)
    {
        ClientManager.instance.GetPlayerByID(id).GetComponent<ShootoutGameClientPlayer>().Action();
    }

    private void SetVRPlayerCanThrowFireballs(bool canThrow)
    {
        VRPlayer.GetComponent<ShootoutGameVRPlayerController>().canThrowFireballs = canThrow;
    }
}