using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] Canvas clientCanvas, vrCanvas;

    [SerializeField] GameObject playerIconPrefab, clientPlayerIconsParent, VRPlayerIconsParent;
    [SerializeField] GameManager manager;
    [SerializeField] Button VrPlayerReady;

    private Dictionary<string, GameObject> clientPlayerIcons, vrPlayerIcons;

    private ClientManager cm;

    void Start()
    {
        clientCanvas.enabled = true;

        clientPlayerIcons = new Dictionary<string, GameObject>();
        vrPlayerIcons = new Dictionary<string, GameObject>();
        cm = ClientManager.instance;
        if (VrPlayerReady != null)
        {
            SpawnVRPlayerIcon();
        }
        SpawnPlayerIcons();

        cm.onReadyUp += ReadyUp;
        

        if(VrPlayerReady != null )
        {
            VrPlayerReady.onClick.AddListener(ReadyUpVR);
        }
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

        for (int i = 0; i < cm.Players.Count; i++)
        {
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = cm.Players[i].PlayerColor;
            txt.text = cm.Players[i].PlayerName;

            vrPlayerIcons.Add(cm.Players[i].PlayerID, newPlayerIcon);
        }
    }

    private void ReadyUp(ClientPlayer p)
    {
        clientPlayerIcons[p.PlayerID].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons[p.PlayerID].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove(p.PlayerID);

        vrPlayerIcons[p.PlayerID].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[p.PlayerID].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(p.PlayerID);

        //Check if every player is ready
        if (clientPlayerIcons.Count > 0)
        {
            return;
        }

        cm.onReadyUp -= ReadyUp;
        manager.State = GameManager.GameState.Countdown;
        this.gameObject.SetActive(false);
    }
    private void SpawnVRPlayerIcon()
    {
        GameObject newPlayerIcon = Instantiate(playerIconPrefab, clientPlayerIconsParent.transform);
        TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
        txt.color = new Color(255, 255, 255);
        txt.text = "";
        newPlayerIcon.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);

        clientPlayerIcons.Add("VR", newPlayerIcon);

        newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
        txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
        txt.color = new Color(255, 255, 255);
        txt.text = "";
        newPlayerIcon.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);

        vrPlayerIcons.Add("VR", newPlayerIcon);
    }

    public void ReadyUpVR()
    {
        clientPlayerIcons["VR"].GetComponentInChildren<TMP_Text>().text = "READY";
        clientPlayerIcons["VR"].GetComponent<Animator>().SetTrigger("Ready");
        clientPlayerIcons.Remove("VR");

        vrPlayerIcons["VR"].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons["VR"].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove("VR");

        //Check if every player is ready
        if (clientPlayerIcons.Count > 0)
        {
            return;
        }

        cm.onReadyUp -= ReadyUp;
        manager.State = GameManager.GameState.Countdown;
        this.gameObject.SetActive(false);
    }
}