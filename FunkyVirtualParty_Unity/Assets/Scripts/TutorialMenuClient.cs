using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenuClient : MonoBehaviour
{
    public static TutorialMenuClient instance;

    [SerializeField] Canvas tutorialCanvas, controllerCanvas;

    [SerializeField] GameObject playerIconPrefab, clientPlayerIconsParent;
    [SerializeField] Button readyUpButton;

    private Dictionary<int, GameObject> clientPlayerIcons;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tutorialCanvas.enabled = true;

        clientPlayerIcons = new Dictionary<int, GameObject>();
        SpawnVRPlayerIcon();
        SpawnPlayerIcons();

        ClientPlayer.OnReadyUp.AddListener(ReadyUp);


        //cm.onVRReadyUp += ReadyUpVR;
        //cm.onClientDisonnect += RemovePlayerIcon;
    }

    private void OnDestroy()
    {
        ClientPlayer.OnReadyUp.RemoveListener(ReadyUp);

        //cm.onVRReadyUp -= ReadyUpVR;
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            if (ClientPlayer.clients[i].syncer.IsDebugPlayer && clientPlayerIcons.ContainsKey(ClientPlayer.clients[i].DebugPlayerIndex))
            {
                continue;
            }

            GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = ClientPlayer.clients[i].syncer.Color;
            txt.text = ClientPlayer.clients[i].syncer.Name;

            if (ClientPlayer.clients[i].syncer.IsDebugPlayer && !clientPlayerIcons.ContainsKey(ClientPlayer.clients[i].DebugPlayerIndex))
            {
                clientPlayerIcons.Add(ClientPlayer.clients[i].DebugPlayerIndex, newPlayerIcon);
                continue;
            }

            clientPlayerIcons.Add(ClientPlayer.clients[i].realtimeView.ownerIDSelf, newPlayerIcon);
        } 
    }

    private void RemovePlayerIcon(int id)
    {
        Destroy(clientPlayerIcons[id]);
        clientPlayerIcons.Remove(id);
    }

    public void ReadyUpButtonPressed()
    {
        RealtimeSingletonWeb.instance.LocalPlayer.syncer.IsReady = true;
    }
    private void ReadyUp(ClientPlayer p)
    {
        if (p.syncer.IsDebugPlayer && clientPlayerIcons.ContainsKey(p.DebugPlayerIndex))
        {
            clientPlayerIcons[p.DebugPlayerIndex].GetComponentInChildren<TMP_Text>().text = "READY";
            clientPlayerIcons[p.DebugPlayerIndex].GetComponent<Animator>().SetTrigger("Ready");
            clientPlayerIcons.Remove(p.DebugPlayerIndex);
        }
        else if (clientPlayerIcons.ContainsKey(p.realtimeView.ownerIDSelf))
        {
            clientPlayerIcons[p.realtimeView.ownerIDSelf].GetComponentInChildren<TMP_Text>().text = "READY";
            clientPlayerIcons[p.realtimeView.ownerIDSelf].GetComponent<Animator>().SetTrigger("Ready");
            clientPlayerIcons.Remove(p.realtimeView.ownerIDSelf);
        }

        if(readyUpButton && p.IsLocal)
        {
            readyUpButton.interactable = false;
            readyUpButton.GetComponent<Animator>().SetTrigger("Stop");
        }

        //Check if every player is ready
        if (clientPlayerIcons.Count > 0)
        {
            return;
        }

        //manager.State = GameState.Countdown;
        this.gameObject.SetActive(false);
        tutorialCanvas.enabled = false;
        controllerCanvas.enabled = true;
    }
    private void SpawnVRPlayerIcon()
    {
        GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
        TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
        txt.color = new Color(255, 255, 255);
        txt.text = "";
        newPlayerIcon.GetComponentsInChildren<Image>(true)[2].gameObject.SetActive(true);

        clientPlayerIcons.Add(-1, newPlayerIcon);
    }

    public void ReadyUpVR()
    {
        if (!clientPlayerIcons.ContainsKey(-1))
            return;

        clientPlayerIcons[-1].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons[-1].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove(-1);

        //Check if every player is ready
        if (clientPlayerIcons.Count > 0)
        {
            return;
        }

        //manager.State = GameState.Countdown;
        this.gameObject.SetActive(false);
        tutorialCanvas.enabled = false;
        controllerCanvas.enabled = true;
    }
}