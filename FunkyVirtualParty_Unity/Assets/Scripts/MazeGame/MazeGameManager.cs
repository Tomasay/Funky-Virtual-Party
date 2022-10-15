using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeGameManager : GameManager
{
    [SerializeField] GameObject maze;

    protected override void Start()
    {
        if (ClientManager.instance)
        {
            ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns, true);
            State = GameState.Tutorial;

            foreach (ClientPlayer cp in ClientManager.instance.Players)
            {
                (cp as MazeGameClientPlayer).maze = maze;
            }
        }
        else
        {
            Debug.LogError("Client Manager Instance Not Found!");
        }
    }
}
