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

    void Start()
    {
        instance = this;

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
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = ClientPlayer.clients[i].syncer.Color;
            txt.text = ClientPlayer.clients[i].syncer.Name;

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
        clientPlayerIcons[p.realtimeView.ownerIDSelf].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons[p.realtimeView.ownerIDSelf].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove(p.realtimeView.ownerIDSelf);

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