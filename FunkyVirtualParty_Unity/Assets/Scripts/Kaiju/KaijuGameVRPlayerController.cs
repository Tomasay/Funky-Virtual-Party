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
        grabbable.gameObject.GetComponent<KaijuGameClientPlayer>().realtimeTransform.RequestOwnership();
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {
        

    }
}
