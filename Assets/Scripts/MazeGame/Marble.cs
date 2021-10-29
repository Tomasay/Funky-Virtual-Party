using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] GameObject handheldMaze;
    Rigidbody RB, mazeRB;

    private float threshold = 0.0015f, startingMazeHeight;
    private Vector3 previousValidPoint;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        mazeRB = handheldMaze.GetComponent<Rigidbody>();

        startingMazeHeight = GetPosRelativeToMaze().y;
    }

    void Update()
    {
        //If marble is out of threshold for maze center
        if(Mathf.Abs(GetPosRelativeToMaze().y - startingMazeHeight) > threshold)
        {
            transform.position = handheldMaze.transform.TransformPoint(previousValidPoint);
            //Debug.Log("OUT OF THRESHOLD, setting new pos to: " + handheldMaze.transform.TransformPoint(previousValidPoint));

        }
        else
        {
            previousValidPoint = GetPosRelativeToMaze();
        }
    }

    private Vector3 GetPosRelativeToMaze()
    {
        Vector3 relativePoint = handheldMaze.transform.InverseTransformPoint(transform.position);
        return relativePoint;
    }
}