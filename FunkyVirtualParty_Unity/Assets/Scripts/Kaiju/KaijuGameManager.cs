using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum KaijuGameLoopState
{
    Not,
    Prepare,
    Fight,
    Kill
}


public class KaijuGameManager : GameManager
{
    public KaijuGameLoopState KaijuGameState = KaijuGameLoopState.Not;
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 50;
    [SerializeField] private TMP_Text vrInfoText, vrGameTimeText;
    private bool countingDown = false;
    private float timeRemaining;


#if UNITY_EDITOR
    [SerializeField] private Transform[] debugWaypoints;
    private Vector3[] currentWaypoints;
    private float[] currentWaypointDistances;
#endif

    protected override void Start()
    {
        base.Start();

        timeRemaining = GAME_TIME_AMOUNT;
        vrGameTimeText.text = FormatTime(timeRemaining);

        /*if (ClientManager.instance)
        {

            foreach (ClientPlayer p in ClientManager.instance.Players)
            {
                KaijuGameClientPlayer sp = (KaijuGameClientPlayer)p;
            }
        }*/

        SetPlayerMovement(false);
        SetVRPlayerMovementDelayed(false, 1);
        SetVRPlayerCanThrowFireballs(false);

#if UNITY_EDITOR
        currentWaypoints = new Vector3[ClientManager.instance.Players.Count];
        currentWaypointDistances = new float[ClientManager.instance.Players.Count];
#endif
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Tutorial:
                break;
            case GameState.Countdown:
                if (!countingDown)
                {
                    StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                }
                break;
            case GameState.GameLoop:
                switch (KaijuGameState)
                {
                    case KaijuGameLoopState.Not:
                        KaijuGameState = KaijuGameLoopState.Prepare;
                        break;
                    case KaijuGameLoopState.Prepare:
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            KaijuGameState = KaijuGameLoopState.Fight;
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
                    case KaijuGameLoopState.Fight:
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            State = GameState.TimeEnded;
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
                    case KaijuGameLoopState.Kill:
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            State = GameState.VRPlayerWins;
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
                }
                
                break;
            case GameState.VRPlayerWins:
                StartCoroutine(GameOver(2, "CITY SAVED!!\nYOU WIN!"));
                break;
            case GameState.TimeEnded:
                StartCoroutine(GameOver(2, "KAIJU DESTROYED\nTHE CITY!!\n YOU LOSE!!"));
                break;
            default:
                break;
        }

#if UNITY_EDITOR
        UpdateDebugPlayers();
#endif
    }

    void CheckPartsCollected()
    {
        if (KaijuGameState != KaijuGameLoopState.Fight)
            return;

        int partsNotCollected = 0;
        /*foreach (SwordPart p in Game.Something.WhereverTheHellTheyEndUp)
        {
            SwordPart sp = (SwordPart)p;
            if (!sp.isCollected)
            {
                partsNotCollected++;
            }
        }*/
        partsNotCollected = 1;
        if (partsNotCollected == 0)
        {
            KaijuGameState = KaijuGameLoopState.Kill;
            timeRemaining = 6;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        countingDown = true;
        SetVRPlayerCanThrowFireballs(true);

        yield return new WaitForSeconds(1);

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
        ClientManager.instance.GetPlayerByID(id).GetComponent<KaijuGameClientPlayer>().Action();
    }

    private void SetVRPlayerCanThrowFireballs(bool canThrow)
    {
        //VRPlayer.GetComponent<KaijuGameVRPlayerController>().canThrowFireballs = canThrow;
    }

#if UNITY_EDITOR
    private void UpdateDebugPlayers()
    {
        for (int i = 0; i < ClientManager.instance.Players.Count; i++)
        {
            if (ClientManager.instance.Players[i].isDebugPlayer)
            {
                UpdateDebugPlayerWaypoint(ClientManager.instance.Players[i], i);
            }
        }
    }

    private void UpdateDebugPlayerWaypoint(ClientPlayer cp, int index)
    {
        //If player does not have a waypoint, or has made it to their waypoint
        /*if (currentWaypoints[index] == Vector3.zero || Vector3.Distance(cp.transform.position, currentWaypoints[index]) < 0.1f)
        {
            //Get new waypoint
            currentWaypoints[index] = debugWaypoints[Random.Range(0, debugWaypoints.Length)].position;
            currentWaypointDistances[index] = Vector3.Distance(currentWaypoints[index], cp.transform.position);
        }
        //Else move towards next waypoint
        else
        {
            Vector3 dir = (currentWaypoints[index] - cp.transform.position).normalized;

            //Ease in & out speed
            float dist = Vector3.Distance(currentWaypoints[index], cp.transform.position);
            dir *= Mathf.Clamp(1 / (dist / currentWaypointDistances[index]), 0.1f, 1f);

            ClientManager.instance.Manager.Socket.Emit("inputDebug", dir.x, dir.z, cp.PlayerID);
        }*/
    }
#endif
}
