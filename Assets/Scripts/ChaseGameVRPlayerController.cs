using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ChaseGameVRPlayerController : VRPlayerController
{
    private Vector3 leftHandPos, rightHandPos; //Used to store previous frame hand positions
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
                if (ahp == null)
                    transform.position -= forwardDirection.transform.forward * handDistance * handMovementSpeed * Time.deltaTime;

                else
                    ahp.AddMove(-(forwardDirection.transform.forward * handDistance * handMovementSpeed * Time.deltaTime));
            }
        }
    }
}