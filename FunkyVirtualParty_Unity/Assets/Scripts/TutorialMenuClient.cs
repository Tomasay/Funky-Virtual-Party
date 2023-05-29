using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenuClient : MonoBehaviour
{
    [SerializeField] Canvas tutorialCanvas, controllerCanvas;

    [SerializeField] GameObject playerIconPrefab, clientPlayerIconsParent;
    [SerializeField] Button readyUpButton;

    private Dictionary<string, GameObject> clientPlayerIcons;

    void Start()
    {
        tutorialCanvas.enabled = true;

        clientPlayerIcons = new Dictionary<string, GameObject>();
        SpawnVRPlayerIcon();
        SpawnPlayerIcons();

        //cm.onVRReadyUp += ReadyUpVR;
        //cm.onReadyUp += ReadyUp;
        //cm.onClientDisonnect += RemovePlayerIcon;
    }

    private void OnDestroy()
    {
        //cm.onVRReadyUp -= ReadyUpVR;
        //cm.onReadyUp -= ReadyUp;
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = ClientPlayer.clients[i].syncer.Color;
            txt.text = ClientPlayer.clients[i].syncer.Name;

            clientPlayerIcons.Add(ClientPlayer.clients[i].realtimeView.viewUUID, newPlayerIcon);
        }
    }

    private void RemovePlayerIcon(string id)
    {
        Destroy(clientPlayerIcons[id]);
        clientPlayerIcons.Remove(id);
    }

    public void ReadyUpButtonPressed()
    {
        ReadyUp(ClientManagerWeb.instance.LocalPlayer);
        ClientManagerWeb.instance.Manager.Socket.Emit("ReadyUp", ClientManagerWeb.instance.LocalPlayer.PlayerByteID);
    }
    private void ReadyUp(ClientPlayer p)
    {
        clientPlayerIcons[p.realtimeView.viewUUID].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons[p.realtimeView.viewUUID].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove(p.realtimeView.viewUUID);

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

        clientPlayerIcons.Add("VR", newPlayerIcon);
    }

    public void ReadyUpVR()
    {
        clientPlayerIcons["VR"].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons["VR"].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove("VR");

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