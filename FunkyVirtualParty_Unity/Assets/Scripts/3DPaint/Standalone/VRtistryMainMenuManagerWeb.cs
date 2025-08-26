using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;

public class VRtistryMainMenuManagerWeb : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] Animator mainCamAnim;

    [SerializeField] RawImage animatedLogo;

    [SerializeField] GameObject[] clientIndicators;

    void Start()
    {
        RealtimeSingletonWeb.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;
        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);

        if (ClientPlayer.OnClientConnected == null)
            ClientPlayer.OnClientConnected = new MyCPEvent();

        if (ClientPlayer.OnClientDisconnected == null)
            ClientPlayer.OnClientDisconnected = new MyCPEvent();

        ClientPlayer.OnClientConnected.AddListener(UpdateClientIndicators);
        ClientPlayer.OnClientDisconnected.AddListener(UpdateClientIndicators);
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        animatedLogo.enabled = false; //TODO Setup animation so this gets erased nicely before zooming out
        mainCamAnim.SetTrigger("Zoom Out");

        Invoke("EnableVRAvatarVisibility", 1);

        RealtimeSingletonWeb.instance.Realtime.didConnectToRoom -= Realtime_didConnectToRoom;
    }

    private void EnableVRAvatarVisibility()
    {
        int layerMaskToAdd = 1 << LayerMask.NameToLayer("ClientOnly");
        mainCam.cullingMask |= layerMaskToAdd;

        layerMaskToAdd = 1 << LayerMask.NameToLayer("Hand");
        mainCam.cullingMask |= layerMaskToAdd;
    }

    void OnLocalPlayerSpawned()
    {
        VRtistryClientPlayer vcp = (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer);
        if (vcp && vcp.usingPhone == 0)
        {
            vcp.SetSitAnim();
            //vcp.TogglePhone();
        }
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
}