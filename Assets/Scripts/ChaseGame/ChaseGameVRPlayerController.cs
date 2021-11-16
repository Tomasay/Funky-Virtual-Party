using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;

public class ChaseGameVRPlayerController : VRPlayerController
{
    [SerializeField] private float handMovementSpeed = 20, maxSprint = 2f;
    [SerializeField] private Image sprintMeter;

    private Vector3 movement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float sprintAmount, minimumSprintPercent = 0.5f;
    private bool handMovement = true, sprintCooldown;
    private float walkSpeed;

    private void Start()
    {
        sprintAmount = maxSprint;
        walkSpeed = ahp.moveSpeed;
    }

    void Update()
    {
        if (handMovement && !sprintCooldown && ahp.handLeft.IsSqueezing() && ahp.handRight.IsSqueezing())
        {
            handDistance = Vector3.Distance(leftHandPos, ahp.handLeft.transform.localPosition) + Vector3.Distance(rightHandPos, ahp.handRight.transform.localPosition);

            leftHandPos = ahp.handLeft.transform.localPosition;
            rightHandPos = ahp.handRight.transform.localPosition;

            if (handDistance < 1 && handDistance > 0)
            {
                movement = Vector3.Lerp(movement, -(forwardDirection.transform.forward * handDistance * handMovementSpeed), 5 * Time.deltaTime);
                ahp.AddMove(movement);

                sprintAmount = Mathf.Max(sprintAmount - Time.deltaTime, 0);
            }

            if(sprintAmount / maxSprint < 0.05f)
            {
                sprintCooldown = true;
                sprintMeter.color = Color.red;
            }
        }
        else
        {
            sprintAmount = Mathf.Min(sprintAmount + Time.deltaTime, maxSprint);

            if(sprintCooldown && sprintAmount / maxSprint > minimumSprintPercent)
            {
                sprintCooldown = false;
            }
            else if(sprintCooldown)
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
    }

    public void ExitedWater()
    {
        ahp.moveSpeed = walkSpeed;
    }
}