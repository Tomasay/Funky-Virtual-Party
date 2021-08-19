using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] GameObject playerIconPrefab;
    [SerializeField] GameObject playerIcons;

    private ClientManager cm;

    void Start()
    {
        cm = ClientManager.instance;
        SpawnPlayerIcons();
    }

    private void SpawnPlayerIcons()
    {
        for (int i = 0; i < cm.Players.Count; i++)
        {
            GameObject newPlayerIcon = Instantiate(playerIconPrefab, playerIcons.transform);
            TMP_Text txt = newPlayerIcon.GetComponentInChildren<TMP_Text>();
            txt.color = cm.Players[i].PlayerColor;
            txt.text = cm.Players[i].PlayerName;
        }
    }
}
