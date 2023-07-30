using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class ShootoutGameManagerWeb : MonoBehaviour
{
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    [SerializeField] private ParticleSystem countdownParticles;
    private float timeRemaining;

    [SerializeField] CinemachineVirtualCamera cinemachineCam;
    [SerializeField] Camera cam;

    private void Awake()
    {
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);

        ShootoutGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);
    }

    protected void Start()
    {
        RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
    }

    private void OnDestroy()
    {
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);

        ShootoutGameSyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChange);
    }

    void OnLocalPlayerSpawned()
    {
        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerIndicatorVisibility(true);
        cinemachineCam.Follow = RealtimeSingletonWeb.instance.LocalPlayer.transform;
        (RealtimeSingletonWeb.instance.LocalPlayer as ShootoutGameClientPlayer).cam = cam;
    }

    protected void OnStateChange(string s)
    {
        switch (ShootoutGameSyncer.instance.State)
        {
            case "tutorial":

                break;
            case "countdown":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
                break;
            case "game loop":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = true;
                break;
            case "vr player won":
                countdownText.enabled = true;
                countdownText.text = "VR PLAYER WINS";
                break;
            case "time ended":
                countdownText.enabled = true;
                countdownText.text = "TIME'S UP!\nYOU WIN";
                break;
            default:
                break;
        }
    }

    void Update()
    {
        switch (ShootoutGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                break;
            case "game loop":
                timeRemaining -= Time.deltaTime;
                gameTimeText.text = FormatTime(timeRemaining);
                break;
            case "vr player won":
                break;
            case "time ended":
                break;
            default:
                break;
        }
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

        countdownParticles.Play();
        countdownText.text = "GO!";
        countdownText.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        countdownText.transform.DOScale(1, 0.25f);

        yield return new WaitForSeconds(1);
        countdownText.enabled = false;

        cinemachineCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_YDamping = 1;
        cinemachineCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_ZDamping = 1;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}