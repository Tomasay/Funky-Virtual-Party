using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using epoching.easy_qr_code;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions;

    [SerializeField] private TMP_Text partyCodeText;

    [SerializeField] Generate_qr_code qr = null;
    [SerializeField] RawImage qrCode;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject playerNamesList;
    [SerializeField] GameObject playerIconPrefab;

    // Start is called before the first frame update
    void Start()
    {
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
            qrCode.texture = qr.generate_qr_code(ClientManager.instance.URL + "/?partyCode=" + ClientManager.instance.Passcode);
        }

        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    private void OnDisable()
    {
        ClientManager.instance.onClientConnect -= SpawnPlayer;
        ClientManager.instance.onClientConnect -= SpawnPlayerIcon;
        ClientManager.instance.onClientDisonnect -= RemovePlayerIcon;
    }

    private void SpawnPlayer(GameObject player)
    {
        player.GetComponent<ClientPlayer>().InitialCustomize();
        player.transform.position = playerPositions[ClientManager.instance.Players.Count-1].position;
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
        }
    }
}