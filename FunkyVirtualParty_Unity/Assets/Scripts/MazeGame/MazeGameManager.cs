using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeGameManager : GameManager
{
    [SerializeField] GameObject maze;

    [SerializeField] Collider[] vrIgnoreColliders, mazeIgnoreColliders;


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

        foreach (Collider col in vrIgnoreColliders)
        {
            foreach (Collider col2 in mazeIgnoreColliders)
            {
                Physics.IgnoreCollision(col, col2);
            }
        }
    }
}
