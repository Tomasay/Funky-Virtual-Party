using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;
using System.Runtime.InteropServices;
using DG.Tweening;
using PaintIn3D;
using NaughtyAttributes;

public class VRtistryMainMenuManagerWeb : MonoBehaviour
{
    [SerializeField] Camera mainMenuCam, drawingPhaseCam;
    [SerializeField] Animator mainCamAnim;

    [SerializeField] GameObject[] clientIndicators;
    [SerializeField] Canvas joinedAndWaitingCanvas, faceDrawCanvas;
    [SerializeField] ClientPlayerCustomizer clientCustomizer;

    [SerializeField] Button submitFaceDrawingButton;
    [SerializeField] AnimationClip clientSitAnim;

    [SerializeField] P3dPaintableTexture proxyFacePaintTexture;

    [SerializeField] Button enableCustomizationsButton;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SetInteractiveWidgetOverlay();
#endif

    void Start()
    {
        RealtimeSingletonWeb.instance.ProperlyConnectedToRoom.AddListener(OnProperlyConnectedToRoom);
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);

        if (ClientPlayer.OnClientConnected == null)
            ClientPlayer.OnClientConnected = new MyCPEvent();

        if (ClientPlayer.OnClientDisconnected == null)
            ClientPlayer.OnClientDisconnected = new MyCPEvent();

        ClientPlayer.OnClientConnected.AddListener(UpdateClientIndicators);
        ClientPlayer.OnClientDisconnected.AddListener(UpdateClientIndicatorsDelayed); //Adding delay so that Client count is accurate

        clientCustomizer.OnCustomizationEnabled.AddListener(OnClientCustomizerEnabled);
        clientCustomizer.OnCustomizationDisabled.AddListener(OnClientCustomizerDisabled);
    }

    private void OnDestroy()
    {
        RealtimeSingletonWeb.instance.ProperlyConnectedToRoom.RemoveListener(OnProperlyConnectedToRoom);
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);

        ClientPlayer.OnClientConnected.RemoveListener(UpdateClientIndicators);
        ClientPlayer.OnClientDisconnected.RemoveListener(UpdateClientIndicatorsDelayed);

        clientCustomizer.OnCustomizationEnabled.RemoveListener(OnClientCustomizerEnabled);
        clientCustomizer.OnCustomizationDisabled.RemoveListener(OnClientCustomizerDisabled);
    }

    private void OnProperlyConnectedToRoom()
    {
        AnimatedLogoManager.instance.EraseOut();
        mainCamAnim.SetTrigger("Zoom In");

        Invoke("EnableFaceDrawCanvas", 1);

        Invoke("EnableVRAvatarVisibility", 1);

        RealtimeSingletonWeb.instance.ProperlyConnectedToRoom.RemoveListener(OnProperlyConnectedToRoom);

#if UNITY_WEBGL && !UNITY_EDITOR
        SetInteractiveWidgetOverlay();
#endif
    }

    void EnableFaceDrawCanvas()
    {
        faceDrawCanvas.gameObject.SetActive(true);
        RectTransform faceDrawCanvasRectTransform = faceDrawCanvas.transform as RectTransform;
        faceDrawCanvasRectTransform.localScale = Vector3.zero;
        faceDrawCanvasRectTransform.DOScale(1, 0.5f);

        VRtistryClientPlayer vcp = RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer;
        vcp.faceDrawCam.enabled = true;
        //vcp.Anim.enabled = false;

        // Formula: (1f / totalFrames) * desiredFrame
        float normalizedTime = (1f / (clientSitAnim.length * clientSitAnim.frameRate)) * 1;
        vcp.Anim.SetBool("UsingPhone" + vcp.usingPhone, false);
        vcp.Anim.Play("Sitting1", 0, normalizedTime);
        vcp.Anim.speed = 0;

        vcp.TogglePhone();
    }

    public void OnFaceDrawSubmitted()
    {
        mainCamAnim.SetTrigger("Zoom Out");

        RectTransform faceDrawCanvasRectTransform = faceDrawCanvas.transform as RectTransform;
        faceDrawCanvasRectTransform.DOScale(0, 0.5f);
        Invoke("DisableFaceDrawCanvas", 0.5f);

        VRtistryClientPlayer vcp = RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer;
        vcp.faceDrawCam.enabled = false;
        vcp.Anim.speed = 1;

        RealtimeSingletonWeb.instance.LocalPlayer.syncer.FaceDrawing = proxyFacePaintTexture.GetPngData();

        vcp.TogglePhone();

        Invoke("EnableCustomizationButton", 2);
    }

    void EnableCustomizationButton()
    {
        enableCustomizationsButton.gameObject.SetActive(true);

        RectTransform rt = enableCustomizationsButton.transform as RectTransform;
        rt.localScale = Vector3.zero;
        rt.DOScale(1, 0.25f);
    }

    void DisableFaceDrawCanvas()
    {
        faceDrawCanvas.gameObject.SetActive(false);
    }

    void OnClientCustomizerEnabled()
    {
        clientIndicators[0].transform.parent.gameObject.SetActive(false);
        joinedAndWaitingCanvas.enabled = false;
    }

    void OnClientCustomizerDisabled()
    {
        clientIndicators[0].transform.parent.gameObject.SetActive(true);
        joinedAndWaitingCanvas.enabled = true;
    }

    private void EnableVRAvatarVisibility()
    {
        int layerMaskToAdd = 1 << LayerMask.NameToLayer("ClientOnly");
        mainMenuCam.cullingMask |= layerMaskToAdd;

        layerMaskToAdd = 1 << LayerMask.NameToLayer("Hand");
        mainMenuCam.cullingMask |= layerMaskToAdd;
    }

    void OnLocalPlayerSpawned()
    {
        VRtistryClientPlayer vcp = (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer);
        if (vcp)
        {
            vcp.SetSitAnim();
        }
    }

    void UpdateClientIndicators(ClientPlayer cp)
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

    void UpdateClientIndicatorsDelayed(ClientPlayer cp)
    {
        StartCoroutine(UpdateClientIndicatorsDelayedCoroutine(cp));
    }

    IEnumerator UpdateClientIndicatorsDelayedCoroutine(ClientPlayer cp)
    {
        yield return new WaitForSeconds(0.5f);
        UpdateClientIndicators(cp);
    }

    public void HideMainMenuUI()
    {
        mainMenuCam.gameObject.SetActive(false);
        drawingPhaseCam.gameObject.SetActive(true);

        clientCustomizer.DisableCustomization();
        clientCustomizer.canvas.enabled = false;

        joinedAndWaitingCanvas.enabled = false;

        foreach (GameObject ci in clientIndicators)
        {
            ci.SetActive(false);
        }
    }
}