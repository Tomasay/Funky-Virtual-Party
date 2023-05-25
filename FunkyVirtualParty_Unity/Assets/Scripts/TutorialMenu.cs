using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    private int canvasRotationIncrement = 45;
    private float timeRotated, rotationTime = 1;

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

#if UNITY_EDITOR
        StartCoroutine("ReadyUpDelayed");
#endif
    }

#if UNITY_EDITOR
    IEnumerator ReadyUpDelayed()
    {
        yield return new WaitForSeconds(2);

        foreach (ClientPlayer cp in ClientManager.instance.Players)
        {
            if (cp.isDebugPlayer)
            {
                ClientManager.instance.Manager.Socket.Emit("ReadyUpDebug", cp.PlayerByteID);
            }
        }
    }
#endif

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.rKey.wasPressedThisFrame && VrPlayerReady.interactable)
        {
            ReadyUpVR();
        }
#endif

        UpdateCameraRotationOffset();
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
                txt.color = ClientManager.instance.Players[i].syncer.Color;
                txt.text = ClientManager.instance.Players[i].syncer.Name;

                vrPlayerIcons.Add(ClientManager.instance.Players[i].PlayerSocketID, newPlayerIcon);
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
        vrPlayerIcons[p.PlayerSocketID].GetComponentInChildren<TMP_Text>().text = "READY";
        vrPlayerIcons[p.PlayerSocketID].GetComponent<Animator>().SetTrigger("Ready");
        vrPlayerIcons.Remove(p.PlayerSocketID);

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

        VrPlayerReady.interactable = false;
        VrPlayerReady.GetComponent<Animator>().SetTrigger("Stop");

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

    /// <summary>
    /// Rotates the tutorial screen incrementally to match the player's head rotation.
    /// Make sure that the origin of this tutorial screen object is the same pos/rot as the player
    /// </summary>
    void UpdateCameraRotationOffset()
    {
        if (Time.time > timeRotated + rotationTime)
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