using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuWeb : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject playerNamesList;
    [SerializeField] GameObject playerIconPrefab;
    [SerializeField] GameObject VRPlayer;
    [SerializeField] GameObject stage;

    [SerializeField] Canvas joinRoomCanvas, controllerCanvas;
    [SerializeField] GameObject partyCodeInvalidDebug;
    [SerializeField] Button nameWebGLButton, codeWebGLButton; //Buttons used to access keyboard on WebGL
    [SerializeField] Button submitButton;

    [SerializeField] TMP_InputField partyCodeInputField;

    [SerializeField] Button enableCustomizationsButton;

    [SerializeField] Image titleBG;

    [SerializeField] Material dotsMat;

    [SerializeField] Color[] backgroundColors, dotsColors;
    Color currentBackgroundColor, currentDotsColor;

    public float colorChangeSpeed = 0.1f, timeOnColorSwatch = 5;

    private float currentColorSwatchTime;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        nameWebGLButton.gameObject.SetActive(false);
        codeWebGLButton.gameObject.SetActive(false);
#endif

        //ClientManagerWeb.instance.SpawnPlayers(playerPrefab);
        VRPlayer.SetActive(false);
        stage.SetActive(false);

        //Functionality for spawning new players who enter
        //ClientManagerWeb.instance.onClientConnect += SpawnPlayer;
        //ClientManagerWeb.instance.onClientConnect += SpawnPlayerIcon;
        //ClientManagerWeb.instance.onClientConnect += SwitchToController;
        //ClientManagerWeb.instance.onClientDisonnect += RemovePlayerIcon;

        //ClientManagerWeb.instance.onClientFailedConnect += EnableFailedConnectDebug;

        //If this is isn't first time visiting main menu
        /*
        if (ClientManagerWeb.instance.Players.Count > 0)
        {
            SwitchToController(null);
        }

        /*
        GetNewBackgroundColors();

        Camera.main.backgroundColor = currentBackgroundColor;
        titleBG.color = currentBackgroundColor;
        dotsMat.color = currentDotsColor;

        GetNewBackgroundColors();
        */
    }

    private void Update()
    {
        /*
        if((Time.time - currentColorSwatchTime) > timeOnColorSwatch &&
           Mathf.Abs(Camera.main.backgroundColor.r - currentBackgroundColor.r) < 0.1f &&
           Mathf.Abs(Camera.main.backgroundColor.g - currentBackgroundColor.g) < 0.1f &&
           Mathf.Abs(Camera.main.backgroundColor.b - currentBackgroundColor.b) < 0.1f)
        {
            GetNewBackgroundColors();
        }

        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, currentBackgroundColor, colorChangeSpeed * Time.deltaTime);
        dotsMat.color = Color.Lerp(dotsMat.color, currentDotsColor, colorChangeSpeed * Time.deltaTime);
        titleBG.color = Color.Lerp(titleBG.color, currentBackgroundColor, colorChangeSpeed * Time.deltaTime);
        */
    }

    private void EnableFailedConnectDebug(string s)
    {
        partyCodeInvalidDebug.SetActive(true);
        partyCodeInvalidDebug.GetComponentInChildren<TMP_Text>().text = s;
    }

    public void DisableFailedConnectDebug()
    {
        partyCodeInvalidDebug.SetActive(false);
    }

    public void CheckValidPartyCode(string val)
    {
        submitButton.interactable = (val.Length == 4);
    }

    public void PartyCodeInputToUpper(string val)
    {
        partyCodeInputField.text = val.ToUpper();
    }

    private void GetNewBackgroundColors()
    {
        int colorIndex = Random.Range(0, backgroundColors.Length);

        currentBackgroundColor = backgroundColors[colorIndex];
        currentDotsColor = dotsColors[colorIndex];

        currentColorSwatchTime = Time.time;
    }

    private void OnDisable()
    {
        //ClientManagerWeb.instance.onClientConnect -= SpawnPlayer;
        //ClientManagerWeb.instance.onClientConnect -= SpawnPlayerIcon;
        //ClientManagerWeb.instance.onClientDisonnect -= RemovePlayerIcon;

        //ClientManagerWeb.instance.onClientFailedConnect -= EnableFailedConnectDebug;
    }

    private void SpawnPlayer(GameObject player)
    {
        if (player.GetComponent<ClientPlayer>().IsLocal)
        {
            player.GetComponent<ClientPlayer>().InitialCustomize();
        }
        //player.transform.position = playerPositions[ClientManagerWeb.instance.Players.Count - 1].position;
        player.GetComponent<ClientPlayer>().syncer.NameColor = Color.white;
    }

    private void SpawnPlayerIcon(GameObject player)
    {
        ClientPlayer cp = player.GetComponent<ClientPlayer>();
        GameObject newIcon = Instantiate(playerIconPrefab, playerNamesList.transform);
        newIcon.name = cp.PlayerSocketID;
        newIcon.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullUpdateTransforms; //Weird workaround with Unity's animator
        newIcon.GetComponent<Image>().color = cp.syncer.Color;
        newIcon.GetComponentInChildren<TMP_Text>().text = cp.syncer.Name;
    }

    private void RemovePlayerIcon(string id)
    {
        foreach (Transform t in playerNamesList.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals(id))
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void SwitchToController(GameObject player)
    {
        VRPlayer.SetActive(true);
        stage.SetActive(true);
        joinRoomCanvas.enabled = false;
        controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);

        //ClientManagerWeb.instance.onClientConnect -= SwitchToController;
    }
}