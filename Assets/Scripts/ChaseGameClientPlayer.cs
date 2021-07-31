using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGameClientPlayer : ClientPlayer
{
    private GameManager gm;

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
}
