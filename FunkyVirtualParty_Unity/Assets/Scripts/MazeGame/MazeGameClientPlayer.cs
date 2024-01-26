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

    protected override void Awake()
    {
        base.Awake();

        startingSpeed = speed = 0.05f; //Smol map = smol speed

        syncer.OnDeath.AddListener(HitByMarble);
        transform.parent = MazeGameSyncer.instance.maze.transform;
    }

    bool localStartCalled;
    protected override void LocalStart()
    {
        base.LocalStart();
        localStartCalled = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Move(Vector2 input, bool changeDirection = true, bool animate = true, Vector2 overrideRotation = default)
    {
        if (canMove)
        {
            movement = new Vector3(input.x, 0, input.y) * speed;
            transform.Translate(movement * Time.deltaTime, Space.Self);

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
        if (IsLocal)
        {
            base.CheckInput();

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
        //Debug.DrawRay(t, smr.transform.forward * 0.01f, Color.green);
        if (isLocal && Physics.Raycast(transform.position, smr.transform.forward, out RaycastHit hit, 0.01f))
        {
            if (hit.collider.transform.tag.Equals("Wall"))
            {
                Debug.Log("Hitting Wall");
                //transform.Translate(-movement * Time.deltaTime);
                transform.localPosition -= (movement * Time.deltaTime);
            }
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