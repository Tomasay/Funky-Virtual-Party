using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;
using TMPro;

public class MazeGameVRPlayerController : VRPlayerController
{
    public MeshRenderer headMesh;

    public Collider[] vrIgnoreColliders;

    public TMP_Text vrInfoText, vrGameTimeText;

    private bool isGrabbingLeft, isGrabbingRight;

    void Awake()
    {
        ahp.handRight.OnTriggerGrab += OnGrabbed;
        ahp.handRight.OnTriggerRelease += OnRelease;
        ahp.handLeft.OnTriggerGrab += OnGrabbed;
        ahp.handLeft.OnTriggerRelease += OnRelease;
    }

    private void OnGrabbed(Hand hand, Grabbable grabbable)
    {
        if (hand.left)
        {
            isGrabbingLeft = true;
        }
        else
        {
            isGrabbingRight = true;
        }
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {
        if (hand.left)
        {
            isGrabbingLeft = false;
        }
        else
        {
            isGrabbingRight = false;
        }
    }

    void SetupCollisionIgnore()
    {
        foreach (Fireball f in Fireball.pool)
        {
            Physics.IgnoreCollision(f.col, ahp.headModel.GetComponent<Collider>());
            Physics.IgnoreCollision(f.col, ahp.handLeft.GetComponent<Collider>());
            Physics.IgnoreCollision(f.col, ahp.handRight.GetComponent<Collider>());
            foreach (Finger fingy in ahp.handLeft.fingers)
            {
                Physics.IgnoreCollision(f.col, fingy.GetComponent<Collider>());
            }
            foreach (Finger fingy in ahp.handRight.fingers)
            {
                Physics.IgnoreCollision(f.col, fingy.GetComponent<Collider>());
            }
            foreach (ClientPlayer cp in ClientPlayer.clients)
            {
                ShootoutGameClientPlayer scp = cp.gameObject.GetComponent<ShootoutGameClientPlayer>();
                if (scp != null)
                {
                    Physics.IgnoreCollision(f.col, scp.Col);
                }

            }
        }
    }
}