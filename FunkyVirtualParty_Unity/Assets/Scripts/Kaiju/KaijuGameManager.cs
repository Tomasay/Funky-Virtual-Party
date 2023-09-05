using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class KaijuGameManager : MonoBehaviour
{
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 50;
    [SerializeField] private TMP_Text vrInfoText, vrGameTimeText;
    private bool countingDown = false;
    private float timeRemaining;


#if UNITY_EDITOR
    [SerializeField] private Transform[] debugWaypoints;
    private Vector3[] currentWaypoints;
    private float[] currentWaypointDistances;


#endif

    protected void Start()
    {

        timeRemaining = GAME_TIME_AMOUNT;
        //vrGameTimeText.text = FormatTime(timeRemaining);

       

       foreach (ClientPlayer p in ClientPlayer.clients)
       {
                KaijuGameClientPlayer sp = p.GetComponent<KaijuGameClientPlayer>();
       }
        

        //SetPlayerMovement(false);
        //SetVRPlayerMovementDelayed(false, 1);

#if UNITY_EDITOR
        
#endif
    }

    void Update()
    {
        switch (KaijuGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                if (!countingDown)
                {
                    StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                }
                break;
            case "prepare":
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            KaijuGameSyncer.instance.State = "fight";
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
             case "fight":
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            KaijuGameSyncer.instance.State = "time ended";
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
              case "kill":
                        timeRemaining -= Time.deltaTime;
                        vrGameTimeText.text = FormatTime(timeRemaining);

                        if (timeRemaining <= 5) //Display final 5 seconds for VR player
                        {
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                        }
                        if (timeRemaining <= 0) //Phase end
                        {
                            KaijuGameSyncer.instance.State = "vr player won";
                            timeRemaining = GAME_TIME_AMOUNT;
                            vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                        }
                        break;
                
                
            case "vr player won":
                StartCoroutine(GameOver(2, "CITY SAVED!!\nYOU WIN!"));
                break;
            case "time ended":
                StartCoroutine(GameOver(2, "KAIJU DESTROYED\nTHE CITY!!\n YOU LOSE!!"));
                break;
            default:
                break;
        }

    }

    void CheckPartsCollected()
    {
        if (KaijuGameSyncer.instance.State != "fight")
            return;

        int partsNotCollected = 1;
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
            KaijuGameSyncer.instance.State = "kill";
            timeRemaining = 6;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        

        countingDown = true;
        

        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        vrInfoText.text = "GO!";

        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        KaijuGameSyncer.instance.State = "prepare";
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        vrInfoText.text = txt;
        yield return new WaitForSeconds(3);

        SceneChangerSyncer.instance.CurrentScene = "MainMenu";
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


}
