using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class TutorialMenu : MonoBehaviour
{
    public static TutorialMenu instance;

    public UnityEvent allPlayersReady;

    [SerializeField] Canvas vrCanvas;

    [SerializeField] GameObject playerIconPrefab, VRPlayerIconsParent;
    [SerializeField] Button VrPlayerReady;

    private Dictionary<int, GameObject> vrPlayerIcons;

    private int canvasRotationIncrement = 45;
    private float timeRotated, rotationTime = 1;

    private void Awake()
    {
        instance = this;

        if (allPlayersReady == null)
            allPlayersReady = new UnityEvent();

        vrPlayerIcons = new Dictionary<int, GameObject>();
    }

    void Start()
    {
        ClientPlayer.OnReadyUp.AddListener(ReadyUp);

        if (VrPlayerReady != null)
        {
            SpawnVRPlayerIcon();
        }
        SpawnPlayerIcons();
        SpawnDebugPlayerIcons();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.rKey.wasPressedThisFrame && VrPlayerReady.interactable)
        {
            VrPlayerReady.onClick.Invoke();
        }
#endif

        UpdateCameraRotationOffset();
    }

    private void OnDestroy()
    {
        ClientPlayer.OnReadyUp.RemoveListener(ReadyUp);

        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            //ClientManager.instance.onClientDisonnect -= RemovePlayerIcon;
        }
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            if (!ClientPlayer.clients[i].syncer.IsDebugPlayer)
            {
                GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
                TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
                txt.color = ClientPlayer.clients[i].syncer.Color;
                txt.text = ClientPlayer.clients[i].syncer.Name;

                vrPlayerIcons.Add(ClientPlayer.clients[i].realtimeView.ownerIDSelf, newPlayerIcon);
            }
        }
    }

    private void SpawnDebugPlayerIcons()
    {
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            if (ClientPlayer.clients[i].syncer.IsDebugPlayer && !vrPlayerIcons.ContainsKey(ClientPlayer.clients[i].DebugPlayerIndex))
            {
                GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
                TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
                txt.color = ClientPlayer.clients[i].syncer.Color;
                txt.text = ClientPlayer.clients[i].syncer.Name;

                vrPlayerIcons.Add(ClientPlayer.clients[i].DebugPlayerIndex, newPlayerIcon);
            }
        }

        //Ready up
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            if (cp.syncer.IsDebugPlayer)
            {
                cp.syncer.IsReady = true;
            }
        }
    }

    private void ReadyUp(ClientPlayer p)
    {
        if (p.syncer.IsDebugPlayer && vrPlayerIcons.ContainsKey(p.DebugPlayerIndex))
        {
            vrPlayerIcons[p.DebugPlayerIndex].GetComponentInChildren<TMP_Text>().text = "READY";
            vrPlayerIcons[p.DebugPlayerIndex].GetComponent<Animator>().SetTrigger("Ready");
            vrPlayerIcons.Remove(p.DebugPlayerIndex);
        }
        else if(vrPlayerIcons.ContainsKey(p.realtimeView.ownerIDSelf))
        {
            vrPlayerIcons[p.realtimeView.ownerIDSelf].GetComponentInChildren<TMP_Text>().text = "READY";
            vrPlayerIcons[p.realtimeView.ownerIDSelf].GetComponent<Animator>().SetTrigger("Ready");
            vrPlayerIcons.Remove(p.realtimeView.ownerIDSelf);
        }

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        allPlayersReady.Invoke();
        this.gameObject.SetActive(false);
    }

    private void SpawnVRPlayerIcon()
    {
        GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
        TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
        txt.color = new Color(255, 255, 255);
        txt.text = "";
        newPlayerIcon.GetComponentsInChildren<Image>(true)[2].gameObject.SetActive(true);

        vrPlayerIcons.Add(-1, newPlayerIcon);
    }

    public void ReadyUpVR()
    {
        if (!vrPlayerIcons.ContainsKey(-1))
            return;

        vrPlayerIcons[-1].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[-1].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(-1);

        VrPlayerReady.interactable = false;
        VrPlayerReady.GetComponent<Animator>().SetTrigger("Stop");

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        allPlayersReady.Invoke();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Rotates the tutorial screen incrementally to match the player's head rotation.
    /// Make sure that the origin of this tutorial screen object is the same pos/rot as the player
    /// </summary>
    void UpdateCameraRotationOffset()
    {
        if (Time.time > timeRotated + rotationTime && Camera.main)
        {
            float rot = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.y);

            if (rot >= canvasRotationIncrement)
            {
                transform.DOBlendableRotateBy(new Vector3(0, canvasRotationIncrement, 0), rotationTime);
                timeRotated = Time.time;
            }
            else if (rot <= -canvasRotationIncrement)
            {
                transform.DOBlendableRotateBy(new Vector3(0, -canvasRotationIncrement, 0), rotationTime);
                timeRotated = Time.time;
            }
        }
    }
}