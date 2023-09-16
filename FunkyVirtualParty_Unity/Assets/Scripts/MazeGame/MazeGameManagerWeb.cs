using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameManagerWeb : MonoBehaviour
{
    [SerializeField] GameObject maze;
    [SerializeField] Transform[] spawnPoints;

    void Start()
    {
        /*
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab, true, spawnPoints);
        State = GameState.Tutorial;

        ClientManagerWeb.instance.Manager.Socket.On<string>("gameStateToClient", OnStateChange);

        for (int i = 0; i < ClientManagerWeb.instance.Players.Count; i++)
        {
            (ClientManagerWeb.instance.Players[i] as MazeGameClientPlayer).maze = maze;

            ClientManagerWeb.instance.Players[i].transform.parent = spawnPoints[i];
            ClientManagerWeb.instance.Players[i].transform.localPosition = Vector3.zero;
        }
        */
    }
}