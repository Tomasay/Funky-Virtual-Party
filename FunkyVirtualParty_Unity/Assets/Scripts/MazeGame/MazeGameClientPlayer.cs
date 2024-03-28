using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class MazeGameClientPlayer : ClientPlayer
{
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void TriggerHaptic(int hapticTime);
#endif

    public Camera cam;

    private bool collidingWithWall;

    //The last non zero input by player
    private Vector3 lastInput;

    public bool isAlive = true;

    private GameObject playerProxy;

    protected override void Awake()
    {
        base.Awake();

        startingSpeed = speed = 0.05f; //Smol map = smol speed

        syncer.OnDeath.AddListener(HitByMarble);
        //transform.parent = MazeGameSyncer.instance.maze.transform;

        IsLocal = true;
    }

    bool localStartCalled;
    protected override void LocalStart()
    {
        base.LocalStart();

        playerProxy = GameObject.Find("PlayerProxy");
        playerProxy.transform.position = transform.position;
        playerProxy.transform.rotation = transform.rotation;

        localStartCalled = true;
    }

    public override void Move(Vector2 input, bool changeDirection = true, bool animate = true, Vector2 overrideRotation = default)
    {
        if (canMove)
        {
            movement = new Vector3(input.x, 0, input.y) * speed;
            playerProxy.transform.Translate(movement * Time.deltaTime, Space.Self);
            transform.SetPositionAndRotation(playerProxy.transform.position, playerProxy.transform.rotation);

            //Magnitude of movement for animations
            if (animate)
            {
                float val = Mathf.Abs(input.magnitude);
                if ((val > 0.05) || (val < -0.05))
                {
                    syncer.AnimSpeed = val;
                }
                else
                {
                    syncer.AnimSpeed = 0;
                }
            }
            else
            {
                syncer.AnimSpeed = 0;
            }

            //Update rotation
            if (changeDirection)
            {
                Vector3 lookDirection;
                if (overrideRotation != Vector2.zero)
                {
                    lookDirection = new Vector3(overrideRotation.x, 0, overrideRotation.y);
                }
                else
                {
                    lookDirection = new Vector3(input.x, 0, input.y);
                }

                if (lookDirection != Vector3.zero)
                {
                    anim.transform.localRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                }
            }

            if (movement != Vector3.zero)
            {
                lastInput = movement;
            }
        }
        else
        {
            movement = Vector3.zero;
            syncer.AnimSpeed = 0;
        }
    }

    protected override void CheckInput()
    {
        if (IsLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to call Move if input is 0 and we're already not moving
            {
                Move(input);
            }
            else
            {
                if (playerProxy)
                {
                    transform.SetPositionAndRotation(playerProxy.transform.position, playerProxy.transform.rotation);
                }
            }

            CustomCollision();
        }
    }

    void HitByMarble()
    {
        isAlive = false;

        if (isLocal)
        {
            canMove = false;
            animSyncer.Trigger = ("Fall");
            TriggerBlinkingAnimation(3);

#if UNITY_WEBGL && !UNITY_EDITOR
        TriggerHaptic(200);
#endif
        }
    }

    void CustomCollision()
    {
#if UNITY_WEBGL
        if (Physics.Raycast(transform.position, smr.transform.forward, out RaycastHit hit, 0.01f) && hit.collider.transform.tag.Equals("Wall"))
        {
            Debug.DrawRay(transform.position, smr.transform.forward * 0.01f, Color.green);
            Debug.Log("Hitting Wall");

            transform.localPosition -= (movement * Time.deltaTime);
        }
        else
        {
            Debug.DrawRay(transform.position, smr.transform.forward * 0.01f, Color.red);
            Debug.Log("NOT Hitting Wall");
        }
#endif

    }

    public void TriggerBlinkingAnimation(int time)
    {
        StartCoroutine("TriggerBlinkingAnimationCoroutine", time);
    }

    IEnumerator TriggerBlinkingAnimationCoroutine(int time)
    {
        for (int i = 0; i < time*2; i++)
        {
            smr.enabled = false;
            yield return new WaitForSeconds(0.25f);
            smr.enabled = true;
            yield return new WaitForSeconds(0.25f);
        }

        //canMove = true;
        //animSyncer.Trigger = ("Idle1");
    }
}