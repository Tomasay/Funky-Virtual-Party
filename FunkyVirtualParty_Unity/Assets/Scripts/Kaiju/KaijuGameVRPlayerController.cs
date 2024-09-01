using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Autohand;
using TMPro;

public class KaijuGameVRPlayerController : VRPlayerController
{
    [SerializeField] private float handMovementSpeed = 20, maxSprintSeconds = 2f, handMovementThreshold = 0;
    private float currentHandMovementSpeed;
    public TMP_Text vrInfoText, vrGameTimeText;

    private Vector3 movement, newMovement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float movementCooldown;
    private float walkSpeed;

    private bool inWater;

    
    private void Start()
    {
        walkSpeed = ahp.maxMoveSpeed;
        currentHandMovementSpeed = handMovementSpeed;

        SetupCollisionIgnore();
    }

    void Update()
    {
        Vector3 forward = ahp.headCamera.transform.forward;
        forward.y = 0;

        // How does this work??
        //newMovement = Vector3.zero;

        //Update movement from hands
        movement = Vector3.Lerp(movement, newMovement, 5 * Time.deltaTime);
        ahp.transform.parent.Translate(movement);
    }

    public void OnGrabbed(Hand hand, Grabbable grabbable)
    {
        KaijuGameClientPlayer kcp = grabbable.gameObject.GetComponent<KaijuGameClientPlayer>();
        if (kcp)
        {
            kcp.realtimeTransform.RequestOwnership();
            kcp.realtimeView.RequestOwnership();
        }
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {

    }

    void SetupCollisionIgnore()
    {
        foreach (ClientPlayer kcp in ClientPlayer.clients)
        {
            Physics.IgnoreCollision(kcp.Col, ahp.headModel.GetComponent<Collider>());
            Physics.IgnoreCollision(kcp.Col, ahp.handLeft.GetComponent<Collider>());
            Physics.IgnoreCollision(kcp.Col, ahp.handRight.GetComponent<Collider>());
            foreach (Finger fingy in ahp.handLeft.fingers)
            {
                Physics.IgnoreCollision(kcp.Col, fingy.GetComponent<Collider>());
            }
            foreach (Finger fingy in ahp.handRight.fingers)
            {
                Physics.IgnoreCollision(kcp.Col, fingy.GetComponent<Collider>());
            }
        }
    }
}
