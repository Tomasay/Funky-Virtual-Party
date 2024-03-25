using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;
using CW.Common;

public class MazeGameManagerWeb : MonoBehaviour
{
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
    [SerializeField] CinemachineVirtualCamera cinemachineCam, vrPlayerCam1, vrPlayerCam2;
    [SerializeField] Animator cameraAnim;
    [SerializeField] LocalLerpFollow playerFollow;

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
        playerFollow.target = RealtimeSingletonWeb.instance.LocalPlayer.transform;
        cinemachineCam.Follow = playerFollow.gameObject.transform;
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
                StartCoroutine("SetCamera");

                SetVRPlayerVisibility(false);
                break;
            case "game loop":
                RealtimeSingletonWeb.instance.LocalPlayer.CanMove = true;
                break;
            case "vr player won":
                countdownText.enabled = true;
                countdownText.text = "VR PLAYER WINS";
                SetPlayerDoNotDestroy();
                break;
            case "vr player lost":
                countdownText.enabled = true;
                countdownText.text = "TIME'S UP!\nYOU WIN";
                SetPlayerDoNotDestroy();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets local client player to DoNotDestroyOnLoad.
    /// This needs to be applied before loading back to the main menu,
    /// as childing to the maze removes this flag
    /// </summary>
    void SetPlayerDoNotDestroy()
    {
        RealtimeSingletonWeb.instance.LocalPlayer.gameObject.transform.parent = null;
        DontDestroyOnLoad(RealtimeSingletonWeb.instance.LocalPlayer.gameObject);
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
            case "vr player lost":
                break;
            default:
                break;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
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

    IEnumerator SetCamera()
    {
        cinemachineCam.Priority = 10;
        vrPlayerCam1.Priority = 10;
        vrPlayerCam2.Priority = 20;

        yield return new WaitForSeconds(2);

        cinemachineCam.Priority = 20;
        vrPlayerCam1.Priority = 10;
        vrPlayerCam2.Priority = 10;

        yield return new WaitForSeconds(1);

        cam.cullingMask &= ~(1 << LayerMask.NameToLayer("VROnly"));
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}