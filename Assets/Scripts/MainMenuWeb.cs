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

    [SerializeField] Canvas joinRoomCanvas, controllerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        //Spawn all existing players
        /*
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab, playerPositions);
        foreach (ClientPlayer cp in ClientManagerWeb.instance.Players)
        {
            SpawnPlayerIcon(cp.gameObject);
        }
        */

        //Functionality for spawning new players who enter
        ClientManagerWeb.instance.onClientConnect += SpawnPlayer;
        ClientManagerWeb.instance.onClientConnect += SpawnPlayerIcon;
        ClientManagerWeb.instance.onClientConnect += SwitchToController;
        ClientManagerWeb.instance.onClientDisonnect += RemovePlayerIcon;
    }

    private void OnDisable()
    {
        ClientManagerWeb.instance.onClientConnect -= SpawnPlayer;
        ClientManagerWeb.instance.onClientConnect -= SpawnPlayerIcon;
        ClientManagerWeb.instance.onClientDisonnect -= RemovePlayerIcon;
    }

    private void SpawnPlayer(GameObject player)
    {
        player.GetComponent<ClientPlayer>().InitialCustomize();
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
        joinRoomCanvas.enabled = false;
        controllerCanvas.enabled = true;
    }
}