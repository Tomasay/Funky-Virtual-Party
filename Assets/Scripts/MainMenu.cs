using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Transform[] playerPositions;

    [SerializeField]
    private TMP_Text partyCodeText;

    // Start is called before the first frame update
    void Start()
    {
        ClientManager.instance.onClientConnect += SpawnPlayer;
        SetPartyCodeText(ClientManager.instance.Passcode);
    }

    private void SpawnPlayer(GameObject player)
    {
        foreach (Transform t in playerPositions)
        {
            if(t.childCount == 0)
            {
                player.transform.position = t.position;
                break;
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