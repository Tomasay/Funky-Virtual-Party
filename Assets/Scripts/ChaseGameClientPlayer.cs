using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGameClientPlayer : ClientPlayer
{
    [SerializeField] private int tackleForce, tackleCooldown = 2;

    private GameManager gm;
    private bool tackling;
    private float timeTackled = 0;

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
            Debug.Log("COLLIDING WITH WATER");
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

    public override void Action()
    {
        if (!canMove) return;

        if (timeTackled == 0 || (Time.time - timeTackled) > tackleCooldown)
        {
            timeTackled = Time.time;
            anim.SetTrigger("Tackle");
            canMove = false;
            StartCoroutine("TackleEnd", 2);
            GetComponent<Rigidbody>().AddForce(anim.transform.forward * tackleForce);
        }
    }

    IEnumerator TackleEnd(int delay)
    {
        yield return new WaitForSeconds(delay);

        canMove = true;
    }

    IEnumerator ExitWater()
    {
        //TODO: Trigger water droplets particle effect
        yield return new WaitForSeconds(2);
        speed = startingSpeed;
    }
 }