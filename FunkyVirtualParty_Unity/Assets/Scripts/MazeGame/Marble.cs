using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Marble : MonoBehaviour
{
    [SerializeField] GameObject handheldMaze;
    [SerializeField] Collider mazeBounds;

    Rigidbody rb;

    private float threshold = 0.005f, startingMazeHeight;
    private Vector3 previousValidPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        previousValidPoint = GetPosRelativeToMaze();
        startingMazeHeight = GetPosRelativeToMaze().y;
    }

    void Update()
    {
        //If marble is at incorrect height in maze, or out of the bounds
        if(Mathf.Abs(GetPosRelativeToMaze().y - startingMazeHeight) > threshold || !CheckIfInBounds())
        {
            rb.position = handheldMaze.transform.TransformPoint(previousValidPoint);
            //Debug.Log("OUT OF THRESHOLD, setting new pos to: " + handheldMaze.transform.TransformPoint(previousValidPoint));
            //Debug.Log("New height: " + GetPosRelativeToMaze().y);
        }
        else
        {
            previousValidPoint = GetPosRelativeToMaze();
        }

        if(MazeGameSyncer.instance.State.Equals("game loop"))
        {
            CheckCollisionWithPlayers();
        }

        if (MazeGameSyncer.instance.State.Equals("game loop") || MazeGameSyncer.instance.State.Equals("countdown"))
        {
            //Sync marble
            MazeGameSyncer.instance.MarbleMazePos = handheldMaze.transform.InverseTransformPoint(rb.position);
        }
    }

    void CheckCollisionWithPlayers()
    {
        //Collision should be dictated by host
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            MazeGameClientPlayer mcp = cp as MazeGameClientPlayer;
            if (mcp.isAlive & Vector3.Distance(rb.position, mcp.transform.position) < 0.015f)
            {
                mcp.syncer.OnDeathTrigger = true;
            }
        }
#endif

    }

    private Vector3 GetPosRelativeToMaze()
    {
        Vector3 relativePoint = handheldMaze.transform.InverseTransformPoint(rb.position);
        return relativePoint;
    }

    private bool CheckIfInBounds()
    {
        return (mazeBounds.bounds.Contains(rb.position));
    }
}