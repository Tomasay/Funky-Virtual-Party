using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] GameObject handheldMaze;
    Rigidbody RB, mazeRB;


    void Start()
    {
        RB = GetComponent<Rigidbody>();
        mazeRB = handheldMaze.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Lock local position to not move vertically, relative to the board
        if (RB && mazeRB)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(mazeRB.velocity);
            localVelocity.y = 0;

            RB.velocity = transform.TransformDirection(localVelocity);
        }
    }
}