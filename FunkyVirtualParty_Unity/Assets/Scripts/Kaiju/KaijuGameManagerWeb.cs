using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class KaijuGameManagerWeb : MonoBehaviour
{  
    private const int PHASE_COUNT = 2, COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 50;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    private bool countingDown = false;
    private float timeRemaining;

    [SerializeField] CinemachineFreeLook cinemachineCam;
    [SerializeField] Camera cam;


    // Start is called before the first frame update
    protected void Start()
    {
        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);

        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);
        KaijuGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);
    }

    private void OnDestroy()
    {
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);
        KaijuGameSyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChange);
    }

    void OnLocalPlayerSpawned()
    {
        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerIndicatorVisibility(true);

        (RealtimeSingletonWeb.instance.LocalPlayer as KaijuGameClientPlayer).cam = cam;

        cinemachineCam.Follow = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        cinemachineCam.LookAt = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
    }

    protected void OnStateChange(string s)
    {
        switch (KaijuGameSyncer.instance.State)
        {
            case "tutorial":

                break;
            case "countdown":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case "game loop":
            case "prepare":
            case "fight":
            case "kill":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = true;
                break;
            case "vr player won":
                countdownText.enabled = true;
                countdownText.text = "YOU WIN";
                break;
            case "time ended":
                countdownText.enabled = true;
                countdownText.text = "CITY DESTORYED!\nYOU LOSE";
                break;
            default:
                break;
        }
    }

    void Update()
    {

#if !UNITY_EDITOR
        switch (KaijuGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                break;
            case "game loop":
            case "prepare":
            case "fight":
                if (timeRemaining <= 0) //Phase end
                {
                        timeRemaining = GAME_TIME_AMOUNT;
                }
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                
                break;
            case "kill":
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                 if (timeRemaining < 0) //Phase end
                {
                        timeRemaining = 0;
                }
                break;
            case "vr player won":
                break;
            case "time ended":
                break;
            default:
                break;
        }
#endif
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        yield return new WaitForSeconds(1);

        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
            countdownText.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            countdownText.transform.DOScale(1, 0.25f);
            yield return new WaitForSeconds(1);
        }

        
        countdownText.text = "GO!";
        countdownText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        countdownText.transform.DOScale(1, 0.25f);

        yield return new WaitForSeconds(1);
        countdownText.enabled = false;
    }

}
