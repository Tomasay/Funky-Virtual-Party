using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class ChaseGameClientPlayer : ClientPlayer
{
    [SerializeField] private int tackleForce = 6, tackleCooldown = 2, tacklePlayerRange = 10;

    [SerializeField] private LookAtConstraint indicatorConstraint;
    [SerializeField] private Image indicatorImage;
    private GameObject lookAtTarget;

    private bool tackling;
    private float timeTackled = 0;

    public Camera cam;

    bool isInWater;

    protected override void CheckInput()
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
                Move(input);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        tackleForce -= (clients.Count / 4); // >= 4 clients tackleForce = 5, 8 clients tackleForce = 4 
    }

    protected override void Update()
    {
#if UNITY_WEBGL
        if (cam)
        {
            playerNameText.transform.LookAt(2 * transform.position - cam.transform.position);
        }

        CheckInput();

        //LookAt constraint
        if (lookAtTarget)
        {
            Vector3 headPos = RealtimeSingletonWeb.instance.VRAvatar.head.position;
            headPos.y = indicatorConstraint.transform.position.y;
            lookAtTarget.transform.position = headPos;
        }
#else
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }

#endif
    }

    public void SetupIndicatorConstraint()
    {
        indicatorConstraint.gameObject.SetActive(true);
        lookAtTarget = new GameObject("LookAt");

        ConstraintSource src = new();
        src.sourceTransform = lookAtTarget.transform;
        src.weight = 1;
        indicatorConstraint.AddSource(src);
        indicatorConstraint.constraintActive = true;

        indicatorImage.color = syncer.Color;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        Debug.Log("On collision");

        base.OnCollisionEnter(collision);

#if UNITY_WEBGL
        if (ChaseGameSyncer.instance.State.Equals("game loop") && collision.gameObject.transform.root.tag.Equals("Player"))
        {
            Debug.Log("VR PLAYER LOST");
            ChaseGameSyncer.instance.State = "vr player lost";
            //gm.DisplayVRCapture(syncer.Name);
        }
#endif
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

    public override void Action(InputAction.CallbackContext obj)
    {
        if (!canMove) return;

        if (timeTackled == 0 || (Time.time - timeTackled) > tackleCooldown)
        {
            timeTackled = Time.time;
            animSyncer.Trigger = "Tackle";
            canMove = false;
            StartCoroutine("TackleEnd", 2);

            //If VR player is within range, tackle towards them
            if (Vector3.Distance(transform.position, RealtimeSingletonWeb.instance.VRAvatar.head.position) < tacklePlayerRange)
            {
                //Get direction towards VR player
                Vector3 dir = (RealtimeSingletonWeb.instance.VRAvatar.head.position - transform.position).normalized;
                //Debug.Log("DIRECTION TOWARDS VR PLAYER: " + dir);

                //Rotate to look at player, only on Y axis
                Quaternion lookDir = Quaternion.LookRotation(dir);
                anim.transform.rotation = Quaternion.Euler(new Vector3(0, lookDir.eulerAngles.y, 0));

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

        animSyncer.Trigger = "";

        canMove = true;
    }

    IEnumerator ExitWater()
    {
        //TODO: Trigger water droplets particle effect
        yield return new WaitForSeconds(2);
        speed = startingSpeed;
    }
 }