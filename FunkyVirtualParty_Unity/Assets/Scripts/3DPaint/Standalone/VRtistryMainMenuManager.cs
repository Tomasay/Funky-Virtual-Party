using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using Autohand;
using Autohand.Demo;
using System.Linq;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using DG.Tweening;
#if !UNITY_WEBGL
using FMOD.Studio;
using FMODUnity;
#endif
using Normal.Realtime;
using NaughtyAttributes;

public class VRtistryMainMenuManager : MonoBehaviour
{
    [SerializeField] VRtistryGameManager gameManager;

    [SerializeField] Camera startingCamera;
    [SerializeField] VideoPlayer animatedLogoVideo;

    [SerializeField] GameObject[] clientIndicators;

    [SerializeField] Canvas lobbyInfoCanvas, tutorialCanvas, gameCanvas;

    [SerializeField] Button playButton;

    private void Start()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;

        ClientPlayer.OnClientConnected.AddListener(UpdateClientIndicators);
        ClientPlayer.OnClientDisconnected.AddListener(UpdateClientIndicators);
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;

        ClientPlayer.OnClientConnected.RemoveListener(UpdateClientIndicators);
        ClientPlayer.OnClientDisconnected.RemoveListener(UpdateClientIndicators);
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {

        RealtimeSingleton.instance.Realtime.didConnectToRoom -= Realtime_didConnectToRoom;
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        startingCamera.gameObject.SetActive(false);
        animatedLogoVideo.Play();
    }

    private void UpdateClientIndicators(ClientPlayer cp)
    {
        //TODO: Toggle play button based on # of clients compared to ThreeDPaintGlobalVariables.MINIMUM_NUMBER_OF_PLAYERS

        foreach (GameObject ci in clientIndicators)
        {
            ci.SetActive(true);
        }

        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            clientIndicators[i].SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        lobbyInfoCanvas.enabled = true;
        tutorialCanvas.enabled = false;
        gameCanvas.enabled = false;

        gameManager.SetupGame();
        gameManager.StartGame();
        clientIndicators[0].transform.parent.gameObject.SetActive(true);
    }

    [Button]
    public void PlayButtonClicked()
    {
        lobbyInfoCanvas.enabled = false;
        tutorialCanvas.enabled = true;
        gameCanvas.enabled = true;

        gameManager.SetupGame();
        gameManager.StartGame();
        clientIndicators[0].transform.parent.gameObject.SetActive(false);
    }

    public void SettingsButtonClicked()
    {

    }
}
