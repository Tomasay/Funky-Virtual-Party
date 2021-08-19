using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGameClientPlayer : ClientPlayer
{
    private GameManager gm;

    bool isInWater;

    protected override void Start()
    {
        base.Start();

        //TEMPORARY cause I am lazy
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if(collision.gameObject.tag.Equals("Player"))
        {
            gm.State = GameManager.GameState.PlayerCaptured;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Water"))
        {
            isInWater = true;
            speed = 0.5f * startingSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Water"))
        {
            isInWater = false;
            StartCoroutine("ExitWater");
        }
    }

    IEnumerator ExitWater()
    {
        //TODO: Trigger water droplets particle effect
        yield return new WaitForSeconds(2);
        speed = startingSpeed;
    }
 }