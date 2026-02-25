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

    [SerializeField] GameObject animatedLogo;

    [SerializeField] GameObject[] clientIndicators;

    [SerializeField] Canvas lobbyInfoCanvas, tutorialCanvas, gameCanvas;

    [SerializeField] Button playButton;

    private void Start()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;

        ClientPlayer.OnClientConnected.AddListener(OnClientConnected);
        ClientPlayer.OnClientDisconnected.AddListener(UpdateClientIndicatorsDelayed); //Adding delay so that Client count is accurate
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;

        ClientPlayer.OnClientConnected.RemoveListener(OnClientConnected);
        ClientPlayer.OnClientDisconnected.RemoveListener(UpdateClientIndicatorsDelayed);
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        RealtimeSingleton.instance.Realtime.didConnectToRoom -= Realtime_didConnectToRoom;
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        startingCamera.gameObject.SetActive(false);
        animatedLogo.SetActive(true);
    }

    void OnClientConnected(ClientPlayer cp)
    {
        //Default to false because client will be drawing face
        playButton.interactable = false;
        cp.syncer.OnFaceDrawingChangedEvent.AddListener(OnClientsFaceDrawingFinished);

        UpdateClientIndicators(cp);
    }

    private void UpdateClientIndicators(ClientPlayer cp)
    {
        foreach (GameObject ci in clientIndicators)
        {
            ci.SetActive(true);
        }

        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            clientIndicators[i].SetActive(false);
        }
    }

    void OnClientsFaceDrawingFinished()
    {
        //If all clients are done drawing their faces, make play button interactable again
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            if(cp.syncer.FaceDrawing.Length == 0)
            {
                return;
            }
        }
        playButton.interactable = ClientPlayer.clients.Count >= ThreeDPaintGlobalVariables.MINIMUM_NUMBER_OF_PLAYERS;
    }

    void UpdateClientIndicatorsDelayed(ClientPlayer cp)
    {
        StartCoroutine(UpdateClientIndicatorsDelayedCoroutine(cp));
    }

    IEnumerator UpdateClientIndicatorsDelayedCoroutine(ClientPlayer cp)
    {
        yield return new WaitForSeconds(1);
        UpdateClientIndicators(cp);
    }

    public void ReturnToMainMenu()
    {
        lobbyInfoCanvas.enabled = true;
        tutorialCanvas.enabled = false;
        gameCanvas.enabled = false;

        clientIndicators[0].transform.parent.gameObject.SetActive(true);

        AnimatedLogoManager.instance.ResetAnimation();
    }

    [Button]
    public void PlayButtonClicked()
    {
        AnimatedLogoManager.instance.EraseOut();
        Invoke("PlayGame", 1);
    }

    void PlayGame()
    {
        lobbyInfoCanvas.enabled = false;
        tutorialCanvas.enabled = true;
        gameCanvas.enabled = true;

        if (!gameManager.gameSetup) gameManager.SetupGame();
        gameManager.StartGame();
        clientIndicators[0].transform.parent.gameObject.SetActive(false);
    }

    public void SettingsButtonClicked()
    {

    }
}
