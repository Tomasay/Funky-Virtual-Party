using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] Canvas vrCanvas;

    [SerializeField] GameObject playerIconPrefab, VRPlayerIconsParent;
    [SerializeField] GameManager manager;
    [SerializeField] Button VrPlayerReady;

    private Dictionary<string, GameObject> vrPlayerIcons;

    void Start()
    {
        vrPlayerIcons = new Dictionary<string, GameObject>();
        if (VrPlayerReady != null)
        {
            SpawnVRPlayerIcon();
        }
        SpawnPlayerIcons();

        if (ClientManager.instance)
        {
            ClientManager.instance.onReadyUp += ReadyUp;
            ClientManager.instance.onClientDisonnect += RemovePlayerIcon;
        }
        
        if(VrPlayerReady != null )
        {
            VrPlayerReady.onClick.AddListener(ReadyUpVR);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ReadyUpVR();
        }
#endif
    }

    private void OnDestroy()
    {
        if (ClientManager.instance)
        {
            ClientManager.instance.onReadyUp -= ReadyUp;
            ClientManager.instance.onClientDisonnect -= RemovePlayerIcon;
        }
    }

    private void SpawnPlayerIcons()
    {
        if (ClientManager.instance)
        {
            for (int i = 0; i < ClientManager.instance.Players.Count; i++)
            {
                GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
                TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
                txt.color = ClientManager.instance.Players[i].PlayerColor;
                txt.text = ClientManager.instance.Players[i].PlayerName;

                vrPlayerIcons.Add(ClientManager.instance.Players[i].PlayerID, newPlayerIcon);
            }
        }
    }

    private void RemovePlayerIcon(string id)
    {
        Destroy(vrPlayerIcons[id]);
        vrPlayerIcons.Remove(id);
    }

    private void ReadyUp(ClientPlayer p)
    {
        vrPlayerIcons[p.PlayerID].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[p.PlayerID].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(p.PlayerID);

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        if (ClientManager.instance)
        {
            ClientManager.instance.onReadyUp -= ReadyUp;
        }
        manager.State = GameState.Countdown;
        this.gameObject.SetActive(false);
    }
    private void SpawnVRPlayerIcon()
    {
        GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
        TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
        txt.color = new Color(255, 255, 255);
        txt.text = "";
        newPlayerIcon.GetComponentsInChildren<Image>(true)[2].gameObject.SetActive(true);

        vrPlayerIcons.Add("VR", newPlayerIcon);
    }

    public void ReadyUpVR()
    {
        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("ReadyUpVR");
        }

        vrPlayerIcons["VR"].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons["VR"].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove("VR");

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        if (ClientManager.instance)
        {
            ClientManager.instance.onReadyUp -= ReadyUp;
        }
        manager.State = GameState.Countdown;
        this.gameObject.SetActive(false);
    }
}