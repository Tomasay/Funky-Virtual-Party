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
        if (VrPlayerReady != null)
        {
            SpawnVRPlayerIcon();
        }
        SpawnPlayerIcons();

        ClientPlayer.OnReadyUp.AddListener(ReadyUp);

#if UNITY_EDITOR
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            if (cp.isDebugPlayer)
            {
                ReadyUpDebugPlayer(cp);
            }
        }
#endif
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
#if UNITY_EDITOR
            if (vrPlayerIcons.ContainsKey(ClientPlayer.clients[i].debugPlayerIndex))
            {
                continue;
            }
#endif

            GameObject newPlayerIcon = Instantiate(playerIconPrefab, VRPlayerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = ClientPlayer.clients[i].syncer.Color;
            txt.text = ClientPlayer.clients[i].syncer.Name;

#if UNITY_EDITOR
            if (ClientPlayer.clients[i].isDebugPlayer)
            {
                if (vrPlayerIcons.ContainsKey(ClientPlayer.clients[i].debugPlayerIndex))
                {
                    continue;
                }
                else
                {
                    vrPlayerIcons.Add(ClientPlayer.clients[i].debugPlayerIndex, newPlayerIcon);
                    continue;
                }
            }
#endif

            vrPlayerIcons.Add(ClientPlayer.clients[i].realtimeView.ownerIDSelf, newPlayerIcon);
        }
    }

    private void ReadyUp(ClientPlayer p)
    {
        vrPlayerIcons[p.realtimeView.ownerIDSelf].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[p.realtimeView.ownerIDSelf].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(p.realtimeView.ownerIDSelf);

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        allPlayersReady.Invoke();
        this.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void ReadyUpDebugPlayer(ClientPlayer p)
    {
        vrPlayerIcons[p.debugPlayerIndex].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[p.debugPlayerIndex].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(p.debugPlayerIndex);

        //Check if every player is ready
        if (vrPlayerIcons.Count > 0)
        {
            return;
        }

        allPlayersReady.Invoke();
        this.gameObject.SetActive(false);
    }
#endif

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