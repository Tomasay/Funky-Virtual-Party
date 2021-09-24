using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ChaseGameVRPlayerController : VRPlayerController
{
    private Vector3 movement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    [SerializeField] private float handMovementSpeed = 20;

    private bool handMovement = true;

    void Update()
    {
        if (handMovement && ahp.handLeft.IsSqueezing() && ahp.handRight.IsSqueezing())
        {
            handDistance = Vector3.Distance(leftHandPos, ahp.handLeft.transform.localPosition) + Vector3.Distance(rightHandPos, ahp.handRight.transform.localPosition);

            leftHandPos = ahp.handLeft.transform.localPosition;
            rightHandPos = ahp.handRight.transform.localPosition;

            if (handDistance < 1 && handDistance > 0)
            {
                movement = Vector3.Lerp(movement, -(forwardDirection.transform.forward * handDistance * handMovementSpeed * Time.deltaTime), 5 * Time.deltaTime);
                ahp.AddMove(movement);
            }
        }
    }
}