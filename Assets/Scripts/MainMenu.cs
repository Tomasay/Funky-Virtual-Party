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

    // Start is called before the first frame update
    void Start()
    {
        ClientManager.instance.onClientConnect += SpawnPlayer;
        SetPartyCodeText(ClientManager.instance.Passcode);

        if(qr != null)
        {
            qrCode.texture = qr.generate_qr_code(ClientManager.instance.URL);
        }
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