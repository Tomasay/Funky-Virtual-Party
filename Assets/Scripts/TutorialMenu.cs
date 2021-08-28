using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] GameObject playerIconPrefab, playerIconsParent;
    [SerializeField] GameManager manager;

    private Dictionary<string, GameObject> playerIcons;

    private ClientManager cm;

    void Start()
    {
        playerIcons = new Dictionary<string, GameObject>();
        cm = ClientManager.instance;
        SpawnPlayerIcons();

        cm.onReadyUp += ReadyUp;
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < cm.Players.Count; i++)
        {
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, playerIconsParent.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = cm.Players[i].PlayerColor;
            txt.text = cm.Players[i].PlayerName;

            playerIcons.Add(cm.Players[i].PlayerID, newPlayerIcon);
        }
    }

    private void ReadyUp(ClientPlayer p)
    {
        playerIcons[p.PlayerID].GetComponentInChildren<TMP_Text>().text = "READY";
        playerIcons[p.PlayerID].GetComponent<Animator>().SetTrigger("Ready");
        playerIcons.Remove(p.PlayerID);

        //Check if every player is ready
        if(playerIcons.Count > 0)
        {
            return;
        }

        manager.State = GameManager.GameState.Countdown;
        this.gameObject.SetActive(false);
    }
}