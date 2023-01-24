using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGameClientPlayer : ClientPlayer
{
    [SerializeField] private int tackleForce, tackleCooldown = 2, tacklePlayerRange = 10;

    private ChaseGameManager gm = null;
    private GameManagerWeb gmw = null;
    private bool isWebGL = false;
    private bool tackling;
    private float timeTackled = 0;

    public Camera cam;

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
            gm = GameObject.Find("GameManager").GetComponent<ChaseGameManager>();
        }
    }

    protected override void Update()
    {
        if (IsLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //Input should be relative to camera, which is always facing forward from the player
            if (input.magnitude > 0.1f)
            {
                float magnitude = input.magnitude;
                input = input.normalized;
                float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y; // + camera eulerAngles y
                Vector3 newInput = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                input = new Vector2(newInput.x, newInput.z) * magnitude;
            }

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                ClientManagerWeb.instance.Manager.Socket.Emit("input", input.x, input.y, PlayerByteID);
            }

            Move(input.x, input.y);

            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);
        }
        else
        {
            transform.Translate(movement * Time.deltaTime);
        }

        // check if we are below the floor
        if (transform.position.y < -10 && transform.position.y != posFromHost.y)
        {
            transform.position = posFromHost;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);

#if UNITY_WEBGL
        playerNameText.transform.LookAt(2 * transform.position - cam.transform.position);
#else
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
#endif
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
                gm.State = GameState.VRPlayerLoses;
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
            if (Vector3.Distance(transform.position, isWebGL ? gmw.VRPlayerHeadPos : gm.VRPlayerHeadPos) < tacklePlayerRange)
            {
                //Get direction towards VR player
                Vector3 dir = ((isWebGL ? gmw.VRPlayerHeadPos : gm.VRPlayerHeadPos) - transform.position).normalized;
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