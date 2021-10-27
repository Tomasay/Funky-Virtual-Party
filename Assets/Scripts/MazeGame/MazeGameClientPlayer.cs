using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameClientPlayer : ClientPlayer
{
    protected override void Awake()
    {
        base.Awake();

        startingSpeed = speed = 0.2f; //Smol map = smol speed
    }

    protected override void Update()
    {
        transform.Translate(movement * Time.deltaTime);

        lookRotation = Quaternion.Euler(new Vector3(0, lookRotation.eulerAngles.y, 0)); //Override rotation, we want player to rotate with board and only care about Y

        anim.transform.localRotation = Quaternion.RotateTowards(lookRotation, transform.localRotation, Time.deltaTime);
    }
}