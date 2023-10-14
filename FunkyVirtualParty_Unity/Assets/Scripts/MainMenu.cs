using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using epoching.easy_qr_code;
using UnityEngine.InputSystem.EnhancedTouch;
using Autohand;
using Unity.Mathematics;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions;

    [SerializeField] private TMP_Text partyCodeText, partyCodeQRText, inviteYourFriendsText, connectingText, connectionErrorText;

    [SerializeField] Generate_qr_code qr = null;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject playerNamesList;
    [SerializeField] GameObject playerIconPrefab;

    [SerializeField] Color[] backgroundColors;
    [SerializeField] Color[] discoBallMainColors, discoBallHighlightColors;
    int currentColorIndex;

    [SerializeField] GameObject discoBall;

    [SerializeField] GameObject lightSlider;

    [SerializeField] PhysicsGadgetHingeAngleReader discoBallSwitch;
    private bool isDiscoBallOn;

    [SerializeField] Camera startingCamera;


    // Start is called before the first frame update
    void Start()
    {

        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        ToggleBackgroundColor(0);

        Vector3 pos = lightSlider.transform.localPosition;
        pos.z = -0.05f;
        lightSlider.transform.localPosition = pos;

        ClientPlayer.OnClientConnected.AddListener(SpawnPlayerIcon);
        ClientPlayer.OnClientDisconnected.AddListener(RemovePlayerIcon);
        ClientPlayer.OnColorChanged.AddListener(UpdatePlayerIconColor);

        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;

        //If this is consecutive time in main menu, spawn all current players
        if(RealtimeSingleton.instance.Realtime.connected)
        {
            Invoke("SpawnAllCurrentPlayerIcons", 1);
        }
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        startingCamera.gameObject.SetActive(false);

        //Generate QR code
        if (qr != null)
        {
#if !UNITY_WEBGL
            RawImage qrCode = avatar.GetComponentInChildren<RawImage>();
            qrCode.texture = qr.generate_qr_code("https://partycrashers.app" + "/?partyCode=" + RealtimeSingleton.instance.Realtime.room.name);
            qrCode.GetComponentInChildren<TMP_Text>().text = "Code: " + RealtimeSingleton.instance.Realtime.room.name;
#endif
        }
    }

    private void OnDestroy()
    {
        ClientPlayer.OnClientConnected.RemoveListener(SpawnPlayerIcon);
        ClientPlayer.OnClientDisconnected.RemoveListener(RemovePlayerIcon);
        ClientPlayer.OnColorChanged.RemoveListener(UpdatePlayerIconColor);

        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void Update()
    {
        CheckDiscoBallSwitch();
        UpdateLight();
    }

    private void SpawnAllCurrentPlayerIcons()
    {
        foreach (ClientPlayer c in ClientPlayer.clients)
        {
            SpawnPlayerIcon(c);
        }
    }

    private void SpawnPlayerIcon(ClientPlayer cp)
    {
        //Check for duplicates
        foreach (Image i in playerNamesList.GetComponentsInChildren<Image>())
        {
            if(i.gameObject.name.Equals("" + cp.realtimeView.ownerIDSelf))
            {
                return;
            }
        }

        GameObject newIcon = Instantiate(playerIconPrefab, playerNamesList.transform);
        newIcon.name = "" + cp.realtimeView.ownerIDSelf;
        newIcon.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullUpdateTransforms; //Weird workaround with Unity's animator
        newIcon.GetComponent<Image>().color = cp.syncer.Color;
        newIcon.GetComponentInChildren<TMP_Text>().text = cp.syncer.Name;
    }

    public void UpdatePlayerIconColor(ClientPlayer cp)
    {
        foreach (Transform t in playerNamesList.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals("" + cp.realtimeView.ownerIDSelf))
            {
                t.GetComponent<Image>().color = cp.syncer.Color;
            }
        }
    }

    private void RemovePlayerIcon(ClientPlayer cp)
    {
        foreach (Transform t in playerNamesList.GetComponentsInChildren<Transform>())
        {
            if(t.name.Equals("" + cp.realtimeView.ownerIDSelf))
            {
                Destroy(t.gameObject);
            }
        }
    }

    public void ToggleBackgroundColor(int index = -1)
    {
        if (index == -1 || index >= backgroundColors.Length || index < 0)
        {
            if (currentColorIndex == backgroundColors.Length-1)
            {
                currentColorIndex = 0;
            }
            else
            {
                currentColorIndex++;
            }
        }
        else
        {
            currentColorIndex = index;
        }

        Camera.main.backgroundColor = backgroundColors[currentColorIndex];
        discoBall.GetComponent<Renderer>().sharedMaterial.SetColor("BaseColor", discoBallMainColors[currentColorIndex]);
        discoBall.GetComponent<Renderer>().sharedMaterial.SetColor("Highlight", discoBallHighlightColors[currentColorIndex]);
    }

    public void ToggleDiscoBall(bool down)
    {
        discoBall.transform.parent.GetComponent<Animator>().SetTrigger(down ? "Down" : "Up");
    }

    public void DistanceTurnDial()
    {
        lightSlider.transform.DOLocalMoveZ((lightSlider.transform.localPosition.z >= 0.04f) ? -0.05f : 0.05f, 0.5f);
    }

    void UpdateLight()
    {
        float lightLevel = lightSlider.transform.localPosition.z / 0.05f;
        lightLevel = math.remap(-1, 1, 0, 1, lightLevel);

        float h, s, v;
        Color.RGBToHSV(Camera.main.backgroundColor, out h, out s, out v);
        v = Mathf.Lerp(1.0f, 0.25f, lightLevel);
        Camera.main.backgroundColor = Color.HSVToRGB(h, s, v);

        Color.RGBToHSV(RenderSettings.ambientLight, out h, out s, out v);
        v = Mathf.Lerp(0.25f, 0.0f, lightLevel);
        RenderSettings.ambientLight = Color.HSVToRGB(h, s, v);
    }

    void CheckDiscoBallSwitch()
    {
        if(isDiscoBallOn && discoBallSwitch.GetValue() == 0)
        {
            ToggleDiscoBall(false);
            isDiscoBallOn = false;
        }
        if(!isDiscoBallOn && discoBallSwitch.GetValue() == 1)
        {
            ToggleDiscoBall(true);
            isDiscoBallOn = true;
        }
    }
}