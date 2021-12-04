using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;

public class ChaseGameVRPlayerController : VRPlayerController
{
    [SerializeField] private float handMovementSpeed = 20, maxSprint = 2f, handMovementThreshold = 0;
    private float currentHandMovementSpeed;
    [SerializeField] private Image sprintMeter;

    private Vector3 movement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float sprintAmount, minimumSprintPercent = 0.5f;
    private bool handMovement = true, sprintCooldown;
    private float movementCooldown;
    private float walkSpeed;

    private void Start()
    {
        sprintAmount = maxSprint;
        walkSpeed = ahp.moveSpeed;
        currentHandMovementSpeed = handMovementSpeed;
    }

    void Update()
    {
        if(handMovement)
        {
            handDistance = Vector3.Distance(leftHandPos, ahp.handLeft.transform.localPosition) + Vector3.Distance(rightHandPos, ahp.handRight.transform.localPosition);

            leftHandPos = ahp.handLeft.transform.localPosition;
            rightHandPos = ahp.handRight.transform.localPosition;
        }

        if (handMovement && !sprintCooldown && /*ahp.handLeft.IsSqueezing() && ahp.handRight.IsSqueezing()*/ handDistance > handMovementThreshold)
        {
            movementCooldown = 0;

            if (handDistance < 1)
            {
                movement = Vector3.Lerp(movement, -(forwardDirection.transform.forward * handDistance * currentHandMovementSpeed), 5 * Time.deltaTime);
                ahp.AddMove(movement);

                sprintAmount = Mathf.Max(sprintAmount - Time.deltaTime, 0);
            }

            if (sprintAmount / maxSprint < 0.05f)
            {
                sprintCooldown = true;
                sprintMeter.color = Color.red;
            }
        }
        else
        {
            //Player must stop pumping arms for a second to start recharing sprint meter
            movementCooldown += Time.deltaTime;
            if (movementCooldown > 1)
            {
                sprintAmount = Mathf.Min(sprintAmount + Time.deltaTime, maxSprint);
            }

            //After depleting spring meter, it must hit mimimumSprintPercent in order to be able to sprint again
            if (sprintCooldown && sprintAmount / maxSprint > minimumSprintPercent)
            {
                sprintCooldown = false;
            }
            //After depleting sprint cooldown, lerp the color from red back to white to indicate when it's ready
            else if (sprintCooldown)
            {
                sprintMeter.color = Color.Lerp(Color.red, Color.white, (sprintAmount / maxSprint) / minimumSprintPercent);
            }
        }

        //Update sprint sprite
        sprintMeter.fillAmount = sprintAmount / maxSprint;
    }

    public void EnteredWater()
    {
        ahp.moveSpeed = walkSpeed * 0.5f;
        currentHandMovementSpeed = handMovementSpeed * 0.5f;
    }

    public void ExitedWater()
    {
        ahp.moveSpeed = walkSpeed;
        currentHandMovementSpeed = handMovementSpeed;
    }
}