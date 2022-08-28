using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenuClient : MonoBehaviour
{
    [SerializeField] Canvas tutorialCanvas, controllerCanvas;

    [SerializeField] GameObject playerIconPrefab, clientPlayerIconsParent;
    [SerializeField] GameManagerWeb manager;

    private Dictionary<string, GameObject> clientPlayerIcons;

    private ClientManagerWeb cm;

    void Start()
    {
        tutorialCanvas.enabled = true;

        clientPlayerIcons = new Dictionary<string, GameObject>();
        cm = ClientManagerWeb.instance;
        SpawnVRPlayerIcon();
        SpawnPlayerIcons();

        cm.onVRReadyUp += ReadyUpVR;
        cm.onReadyUp += ReadyUp;
    }

    private void OnDestroy()
    {
        cm.onVRReadyUp -= ReadyUpVR;
        cm.onReadyUp -= ReadyUp;
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < cm.Players.Count; i++)
        {
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = cm.Players[i].PlayerColor;
            txt.text = cm.Players[i].PlayerName;

            clientPlayerIcons.Add(cm.Players[i].PlayerID, newPlayerIcon);
        }
    }
    public void ReadyUpButtonPressed()
    {
        ReadyUp(ClientManagerWeb.instance.LocalPlayer);
        ClientManagerWeb.instance.Manager.Socket.Emit("ReadyUp");
    }
    private void ReadyUp(ClientPlayer p)
    {
        clientPlayerIcons[p.PlayerID].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons[p.PlayerID].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove(p.PlayerID);

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