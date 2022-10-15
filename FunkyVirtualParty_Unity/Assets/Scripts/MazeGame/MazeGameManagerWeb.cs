using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameManagerWeb : GameManagerWeb
{
    [SerializeField] GameObject maze;
    [SerializeField] Transform[] spawnPoints;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < ClientManagerWeb.instance.Players.Count; i++)
        {
            (ClientManagerWeb.instance.Players[i] as MazeGameClientPlayer).maze = maze;

            ClientManagerWeb.instance.Players[i].transform.parent = spawnPoints[i];
            ClientManagerWeb.instance.Players[i].transform.localPosition = Vector3.zero;
        }
    }
}