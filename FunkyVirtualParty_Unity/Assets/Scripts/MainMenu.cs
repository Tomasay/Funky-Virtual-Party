using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using epoching.easy_qr_code;
using UnityEngine.InputSystem.EnhancedTouch;
using Autohand;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions;

    [SerializeField] private TMP_Text partyCodeText, partyCodeQRText, inviteYourFriendsText, connectingText, connectionErrorText;

    [SerializeField] private const float TIMEOUT_TIME = 5.0f;

    [SerializeField] Generate_qr_code qr = null;
    [SerializeField] RawImage qrCode;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject playerNamesList;
    [SerializeField] GameObject playerIconPrefab;

    [SerializeField] Color[] backgroundColors;
    [SerializeField] Color[] discoBallMainColors, discoBallHighlightColors;
    int currentColorIndex;

    [SerializeField] GameObject discoBall;

    [SerializeField] GameObject lightDial;

    [SerializeField] PhysicsGadgetHingeAngleReader discoBallSwitch;
    private bool isDiscoBallOn;


    // Start is called before the first frame update
    void Start()
    {
        if(ClientManager.instance.Manager != null)
        {
            ClientManager.instance.Manager.Socket.Emit("setIsGameInPlay", ClientManager.instance.Passcode, false);
        }

        //Spawn all existing players
        ClientManager.instance.SpawnPlayers(playerPrefab, playerPositions);
        foreach (ClientPlayer cp in ClientManager.instance.Players)
        {
            SpawnPlayerIcon(cp.gameObject);
        }

        //Functionality for spawning new players who enter
        ClientManager.instance.onClientConnect += SpawnPlayer;
        ClientManager.instance.onClientConnect += SpawnPlayerIcon;
        ClientManager.instance.onClientDisonnect += RemovePlayerIcon;
        SetPartyCodeText(ClientManager.instance.Passcode);

        //Generate QR code
        if(qr != null)
        {
#if !UNITY_WEBGL
            qrCode.texture = qr.generate_qr_code(ClientManager.instance.QRURL + "/?partyCode=" + ClientManager.instance.Passcode);
#endif
        }

        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        ToggleBackgroundColor(0);

        ClientManager.instance.OnPlayerCustomized += OnPlayerCustomized;
    }

    private void Update()
    {
        if(ClientManager.instance.Manager.Socket.IsOpen)
        {
            partyCodeText.enabled = true;
            inviteYourFriendsText.enabled = true;
            connectingText.enabled = false;
            connectionErrorText.enabled = false;
        }
        else if(!ClientManager.instance.Manager.Socket.IsOpen && Time.time < TIMEOUT_TIME)
        {
            partyCodeText.enabled = false;
            inviteYourFriendsText.enabled = false;
            connectingText.enabled = true;
            connectionErrorText.enabled = false;
        }
        else if (!ClientManager.instance.Manager.Socket.IsOpen && Time.time > TIMEOUT_TIME)
        {
            partyCodeText.enabled = false;
            inviteYourFriendsText.enabled = false;
            connectingText.enabled = false;
            connectionErrorText.enabled = true;
        }

        CheckDiscoBallSwitch();
        UpdateLight();
    }

    private void OnDisable()
    {
        ClientManager.instance.onClientConnect -= SpawnPlayer;
        ClientManager.instance.onClientConnect -= SpawnPlayerIcon;
        ClientManager.instance.onClientDisonnect -= RemovePlayerIcon;
        ClientManager.instance.OnPlayerCustomized -= OnPlayerCustomized;
    }

    void OnPlayerCustomized(string id, string color, int headShape, float height, int hatIndex)
    {
        if (ColorUtility.TryParseHtmlString(color, out Color newCol))
        {
            UpdatePlayerIconColor(id, newCol);
        }
    }

    private void SpawnPlayer(GameObject player)
    {
        player.transform.position = playerPositions[ClientManager.instance.Players.Count-1].position;
        ClientManager.instance.SyncAllPlayerPos();
    }

    private void SpawnPlayerIcon(GameObject player)
    {
        StartCoroutine(SpawnPlayerIconDelayed(player, 0.5f));
    }

    IEnumerator SpawnPlayerIconDelayed(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);

        ClientPlayer cp = player.GetComponent<ClientPlayer>();
        GameObject newIcon = Instantiate(playerIconPrefab, playerNamesList.transform);
        newIcon.name = cp.PlayerSocketID;
        newIcon.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullUpdateTransforms; //Weird workaround with Unity's animator
        newIcon.GetComponent<Image>().color = cp.PlayerColor;
        newIcon.GetComponentInChildren<TMP_Text>().text = cp.PlayerName;
    }

    public void UpdatePlayerIconColor(string id, Color c)
    {
        foreach (Transform t in playerNamesList.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals(id))
            {
                t.GetComponent<Image>().color = c;
            }
        }
    }

    private void RemovePlayerIcon(string id)
    {
        foreach (Transform t in playerNamesList.GetComponentsInChildren<Transform>())
        {
            if(t.name.Equals(id))
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void SetPartyCodeText(string code)
    {
        if (partyCodeText)
        {
            partyCodeText.text = "Party Code: " + code;
            partyCodeQRText.text = code;
        }
    }

    public void ToggleBackgroundColor(int index = -1)
    {
        if (index == -1 || index > backgroundColors.Length || index < 0)
        {
            if (currentColorIndex == backgroundColors.Length)
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

    void UpdateLight()
    {
        float lightLevel = lightDial.transform.localRotation.eulerAngles.y / 180.0f;

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