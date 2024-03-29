using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : MonoBehaviour
{
    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        MazeGameClientPlayer cp = RealtimeSingletonWeb.instance.LocalPlayer as MazeGameClientPlayer;
        if (col.bounds.Contains(cp.playerProxy.position))
        {
            cp.Move(-cp.LastInput, false, false);
        }
    }
}