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

    [SerializeField] Canvas joinRoomCanvas, controllerCanvas;
    [SerializeField] GameObject partyCodeInvalidDebug;
    [SerializeField] Button submitButton;

    [SerializeField] Button enableCustomizationsButton;

    [SerializeField] Material dotsMat;

    [SerializeField] Color[] backgroundColors, dotsColors;
    Color currentBackgroundColor, currentDotsColor;

    public float colorChangeSpeed = 0.1f, timeOnColorSwatch = 5;

    private float currentColorSwatchTime;

    // Start is called before the first frame update
    void Start()
    {
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab);
        VRPlayer.SetActive(false);

        //Functionality for spawning new players who enter
        //ClientManagerWeb.instance.onClientConnect += SpawnPlayer;
        //ClientManagerWeb.instance.onClientConnect += SpawnPlayerIcon;
        ClientManagerWeb.instance.onClientConnect += SwitchToController;
        //ClientManagerWeb.instance.onClientDisonnect += RemovePlayerIcon;

        ClientManagerWeb.instance.onClientFailedConnect += EnableFailedConnectDebug;

        //If this is isn't first time visiting main menu
        if (ClientManagerWeb.instance.Players.Count > 0)
        {
            SwitchToController(null);
            VRPlayer.SetActive(true);
        }

        GetNewBackgroundColors();

        Camera.main.backgroundColor = currentBackgroundColor;
        dotsMat.color = currentDotsColor;

        GetNewBackgroundColors();
    }

    private void Update()
    {
        if((Time.time - currentColorSwatchTime) > timeOnColorSwatch &&
           Mathf.Abs(Camera.main.backgroundColor.r - currentBackgroundColor.r) < 0.1f &&
           Mathf.Abs(Camera.main.backgroundColor.g - currentBackgroundColor.g) < 0.1f &&
           Mathf.Abs(Camera.main.backgroundColor.b - currentBackgroundColor.b) < 0.1f)
        {
            GetNewBackgroundColors();
        }

        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, currentBackgroundColor, colorChangeSpeed * Time.deltaTime);
        dotsMat.color = Color.Lerp(dotsMat.color, currentDotsColor, colorChangeSpeed * Time.deltaTime);
    }

    private void EnableFailedConnectDebug()
    {
        partyCodeInvalidDebug.SetActive(true);
    }

    public void DisableFailedConnectDebug()
    {
        partyCodeInvalidDebug.SetActive(false);
    }

    public void CheckValidPartyCode(string val)
    {
        submitButton.interactable = (val.Length == 4);
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
        ClientManagerWeb.instance.onClientConnect -= SwitchToController;
        //ClientManagerWeb.instance.onClientConnect -= SpawnPlayerIcon;
        //ClientManagerWeb.instance.onClientDisonnect -= RemovePlayerIcon;

        ClientManagerWeb.instance.onClientFailedConnect -= EnableFailedConnectDebug;
    }

    private void SpawnPlayer(GameObject player)
    {
        if (player.GetComponent<ClientPlayer>().IsLocal)
        {
            player.GetComponent<ClientPlayer>().InitialCustomize();
        }
        player.transform.position = playerPositions[ClientManagerWeb.instance.Players.Count - 1].position;
    }

    private void SpawnPlayerIcon(GameObject player)
    {
        ClientPlayer cp = player.GetComponent<ClientPlayer>();
        GameObject newIcon = Instantiate(playerIconPrefab, playerNamesList.transform);
        newIcon.name = cp.PlayerID;
        newIcon.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullUpdateTransforms; //Weird workaround with Unity's animator
        newIcon.GetComponent<Image>().color = cp.PlayerColor;
        newIcon.GetComponentInChildren<TMP_Text>().text = cp.PlayerName;
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
        joinRoomCanvas.enabled = false;
        controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);
    }
}