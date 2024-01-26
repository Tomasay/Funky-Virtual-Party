using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] GameObject handheldMaze;
    [SerializeField] Collider mazeBounds;

    private float threshold = 0.005f, startingMazeHeight;
    private Vector3 previousValidPoint;

    void Start()
    {
        previousValidPoint = GetPosRelativeToMaze();
        startingMazeHeight = GetPosRelativeToMaze().y;
    }

    void Update()
    {
        //If marble is at incorrect height in maze, or out of the bounds
        if(Mathf.Abs(GetPosRelativeToMaze().y - startingMazeHeight) > threshold || !CheckIfInBounds())
        {
            transform.position = handheldMaze.transform.TransformPoint(previousValidPoint);
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
    }

    void CheckCollisionWithPlayers()
    {
        //Collision should be dictated by host
#if UNITY_ANDROID
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            MazeGameClientPlayer mcp = cp as MazeGameClientPlayer;
            Debug.Log("Distance: " + Vector3.Distance(transform.position, mcp.transform.position));
            if (mcp.isAlive & Vector3.Distance(transform.position, mcp.transform.position) < 0.0075f)
            {
                mcp.syncer.OnDeathTrigger = true;
            }
        }
#endif

    }

    private Vector3 GetPosRelativeToMaze()
    {
        Vector3 relativePoint = handheldMaze.transform.InverseTransformPoint(transform.position);
        return relativePoint;
    }

    private bool CheckIfInBounds()
    {
        return (mazeBounds.bounds.Contains(transform.position));
    }
}