using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameClientPlayer : ClientPlayer
{
    public GameObject maze;

    //The last non zero input by player
    private Vector3 lastInput;

    protected override void Awake()
    {
        base.Awake();

        startingSpeed = speed = 0.05f; //Smol map = smol speed
    }

    public override void Move(float x, float y, bool changeDirection = true)
    {
        //Reorient x and y for maze local space
        Vector3 newInput = maze.transform.rotation * new Vector3(x, 0, y);

        if (canMove)
        {
            movement = new Vector3(newInput.x, 0, newInput.z) * speed;

            //Magnitude of movement for animations
            float val = Mathf.Abs(newInput.magnitude);
            if ((val > 0.05) || (val < -0.05))
            {
                anim.SetFloat("Speed", val);
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }

            if (newInput != Vector3.zero)
            {
                lastInput = newInput;
            }
        }
        else
        {
            movement = Vector3.zero;
            anim.SetFloat("Speed", 0);
        }
    }

    protected override void Update()
    {
        //Constantly update player rotation to be in line with maze board
        if(lastInput == Vector3.zero && maze)
        {
            //Players look north by default, before any input
            lastInput = maze.transform.rotation * new Vector3(0, 0, 1);
        }
        lookRotation = Quaternion.LookRotation(lastInput, maze.transform.up);

        base.Update();

        //Keep player centered vertically in maze
        Vector3 localPos = transform.localPosition;
        localPos.y = 0;
        transform.localPosition = localPos;
    }
}