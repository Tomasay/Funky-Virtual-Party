using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Transform[] playerPositions;

    // Start is called before the first frame update
    void Start()
    {
        ClientEvents.current.onClientConnect += SpawnPlayer;
    }

    private void SpawnPlayer(GameObject player)
    {
        foreach (Transform t in playerPositions)
        {
            if(t.childCount == 0)
            {
                Instantiate(player, t);
                break;
            }
        }
    }
}
