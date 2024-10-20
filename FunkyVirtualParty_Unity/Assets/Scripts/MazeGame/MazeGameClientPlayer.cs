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

    private bool collidingWithWall;

    public GameObject maze;

    //The last non zero input by player
    private Vector3 lastInput;

    protected override void Awake()
    {
        base.Awake();

        startingSpeed = speed = 0.05f; //Smol map = smol speed

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClient");
#endif
    }

    public override void Move(Vector2 input, bool changeDirection = true, bool animate = true, Vector2 overrideRotation = default)
    {
        //Reorient x and y for maze local space
        Vector3 newInput = maze.transform.rotation * new Vector3(input.x, 0, input.y);

        if (canMove)
        {
            movement = new Vector3(input.x * speed, 0, input.y * speed) * .01f;

            transform.DOBlendableMoveBy(movement * 0.01f, inputPollRate);

            //Magnitude of movement for animations
            if (animate)
            {
                float val = Mathf.Abs(input.magnitude);
                if ((val > 0.05) || (val < -0.05))
                {
                    anim.SetFloat("Speed", val);
                }
                else
                {
                    anim.SetFloat("Speed", 0);
                }
            }
            else
            {
                anim.SetFloat("Speed", 0);
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
                    lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                    anim.transform.DORotateQuaternion(lookRotation, inputPollRate);
                }
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

    protected override void CheckInput()
    {
        //Constantly update player rotation to be in line with maze board
        if(lastInput == Vector3.zero && maze)
        {
            //Players look north by default, before any input
            lastInput = maze.transform.rotation * new Vector3(0, 0, 1);
        }
        lookRotation = Quaternion.LookRotation(lastInput, maze.transform.up);

        base.CheckInput();

        //Keep player centered vertically in maze
        Vector3 localPos = transform.localPosition;
        localPos.y = 0;
        transform.localPosition = localPos;

        CustomCollision();
    }


#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("MarbleCollision"))
        {
            //Player falls over
            ClientPlayer player = ClientManagerWeb.instance.GetPlayerBySocketID(data);
            player.Anim.SetTrigger("Fall");
            TriggerBlinkingAnimation(3);

            if(player.IsLocal)
            {
                TriggerHaptic(200);
            }

            //TODO: Reduce score for appropriate player

        }
    }
#endif

    void CustomCollision()
    {
#if UNITY_ANDROID
        if (Physics.Raycast(transform.position, smr.transform.forward, out RaycastHit hit, 0.01f))
        {
            if(hit.collider.transform.tag.Equals("Wall"))
            {
                transform.Translate(-movement * Time.deltaTime);
            }
        }

#elif UNITY_WEBGL
        if (isLocal && Physics.Raycast(transform.position, smr.transform.forward, out RaycastHit hit, 0.01f))
        {
            if (hit.collider.transform.tag.Equals("Wall"))
            {
                transform.Translate(-movement * Time.deltaTime);
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
    }
}