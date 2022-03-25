using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGameClientPlayer : ClientPlayer
{
    [SerializeField] private int tackleForce, tackleCooldown = 2, tacklePlayerRange = 10;

    private GameManager gm = null;
    private GameManagerWeb gmw = null;
    private bool isWebGL = false;
    private bool tackling;
    private float timeTackled = 0;

    bool isInWater;

    protected override void Start()
    {
        base.Start();

#if UNITY_WEBGL
        isWebGL = true;
#endif

        //TEMPORARY cause I am lazy
        if(isWebGL)
        {
            gmw = GameObject.Find("GameManager").GetComponent<GameManagerWeb>();
        }
        else
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }


    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (isWebGL)
        {
            /*
            if (gmw && gmw.State == GameManagerWeb.GameState.GameLoop && collision.gameObject.transform.root.tag.Equals("Player"))
            {
                gmw.State = GameManagerWeb.GameState.PlayerCaptured;
            }
            */
        }
        else
        {
            if (gm && gm.State == GameState.GameLoop && collision.gameObject.transform.root.tag.Equals("Player"))
            {
                gm.State = GameState.PlayerCaptured;
                gm.DisplayVRCapture(playerName);
            }
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

    public override void Action()
    {
        if (!canMove) return;

        if (timeTackled == 0 || (Time.time - timeTackled) > tackleCooldown)
        {
            timeTackled = Time.time;
            anim.SetTrigger("Tackle");
            canMove = false;
            StartCoroutine("TackleEnd", 2);

            //If VR player is within range, tackle towards them
            //Debug.Log("DISTANCE FROM VR PLAYER: " + Vector3.Distance(transform.position, isWebGL ? gmw.VRPlayerPos : gm.VRPlayerPos));
            if (Vector3.Distance(transform.position, isWebGL ? gmw.VRPlayerPos : gm.VRPlayerPos) < tacklePlayerRange)
            {
                //Get direction towards VR player
                Vector3 dir = ((isWebGL ? gmw.VRPlayerPos : gm.VRPlayerPos) - transform.position).normalized;
                //Debug.Log("DIRECTION TOWARDS VR PLAYER: " + dir);

                //Rotate to look at player, only on Y axis
                Quaternion lookDir = Quaternion.LookRotation(dir);
                lookRotation = Quaternion.Euler(new Vector3(0, lookDir.eulerAngles.y, 0));

                //Tackle!
                GetComponent<Rigidbody>().AddForce(dir * tackleForce);
            }
            else //If not, lunge in direction player is facing
            {
                GetComponent<Rigidbody>().AddForce(anim.transform.forward * tackleForce);
            }
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