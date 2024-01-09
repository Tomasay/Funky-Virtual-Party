using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MazeGameManagerWeb : MonoBehaviour
{
    [SerializeField] GameObject maze;

    [SerializeField] GameObject playerRef;

    /*
    void Start()
    {
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab, true, spawnPoints);
        State = GameState.Tutorial;

        ClientManagerWeb.instance.Manager.Socket.On<string>("gameStateToClient", OnStateChange);

        for (int i = 0; i < ClientManagerWeb.instance.Players.Count; i++)
        {
            (ClientManagerWeb.instance.Players[i] as MazeGameClientPlayer).maze = maze;

            ClientManagerWeb.instance.Players[i].transform.parent = spawnPoints[i];
            ClientManagerWeb.instance.Players[i].transform.localPosition = Vector3.zero;
        }
    }
    */

    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    [SerializeField] private TMP_Text countdownText, gameTimeText;
    [SerializeField] private ParticleSystem countdownParticles;
    private float timeRemaining;

    [SerializeField] Camera cam;

    protected void Start()
    {
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);
        MazeGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);

        RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;

        timeRemaining = GAME_TIME_AMOUNT;
        gameTimeText.text = FormatTime(timeRemaining);
    }

    private void OnDestroy()
    {
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);
        MazeGameSyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChange);
    }

    void OnLocalPlayerSpawned()
    {
        MazeGameClientPlayer localPlayer = (RealtimeSingletonWeb.instance.LocalPlayer as MazeGameClientPlayer);

        localPlayer.SetPlayerIndicatorVisibility(true);
        localPlayer.cam = cam;
        localPlayer.maze = maze;
        localPlayer.playerRef = playerRef;
    }

    void SetVRPlayerVisibility(bool isVisible)
    {
        if(RealtimeSingletonWeb.instance.isVRAvatarSpawned)
        {
            RealtimeSingletonWeb.instance.VRAvatar.gameObject.GetComponent<MazeGameVRPlayerController>().headMesh.gameObject.layer = LayerMask.NameToLayer(isVisible ? "ClientOnly" : "VROnly");
            RealtimeSingletonWeb.instance.VRAvatar.gameObject.GetComponent<MazeGameVRPlayerController>().Ahp.handLeft.gameObject.layer = LayerMask.NameToLayer(isVisible ? "ClientOnly" : "VROnly");
            RealtimeSingletonWeb.instance.VRAvatar.gameObject.GetComponent<MazeGameVRPlayerController>().Ahp.handRight.gameObject.layer = LayerMask.NameToLayer(isVisible ? "ClientOnly" : "VROnly");
        }
    }

    protected void OnStateChange(string s)
    {
        switch (MazeGameSyncer.instance.State)
        {
            case "tutorial":

                break;
            case "countdown":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = false;
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);

                SetVRPlayerVisibility(false);
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
        switch (MazeGameSyncer.instance.State)
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
        yield return new WaitForSeconds(2);

        yield return new WaitForSeconds(2);

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
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}