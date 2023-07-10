using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;
using TMPro;

public class ChaseGameVRPlayerController : VRPlayerController
{
    [SerializeField] private float handMovementSpeed = 20, maxSprintSeconds = 2f, handMovementThreshold = 0;
    private float currentHandMovementSpeed;
    [SerializeField] private Image sprintMeter;
    [SerializeField] public ParticleSystem capturedParticles;

    public TMP_Text vrInfoText, vrGameTimeText;

    private Vector3 movement, newMovement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float sprintAmount, minimumSprintPercent = 0.5f;
    private bool handMovement = false, sprintCooldown;
    private float movementCooldown;
    private float walkSpeed;

    private bool inWater;

    public bool HandMovement { get => handMovement; set => handMovement = value; }

    private void Start()
    {
        sprintAmount = maxSprintSeconds;
        walkSpeed = ahp.maxMoveSpeed;
        currentHandMovementSpeed = handMovementSpeed;
    }

    void Update()
    {
        Vector3 forward = ahp.headCamera.transform.forward;
        forward.y = 0;

        if (handMovement)
        {
            handDistance = Vector3.Distance(leftHandPos, ahp.handLeft.transform.localPosition) + Vector3.Distance(rightHandPos, ahp.handRight.transform.localPosition);

            leftHandPos = ahp.handLeft.transform.localPosition;
            rightHandPos = ahp.handRight.transform.localPosition;
        }

        if (handMovement && !inWater && !sprintCooldown && /*ahp.handLeft.IsSqueezing() && ahp.handRight.IsSqueezing()*/ handDistance > handMovementThreshold)
        {
            movementCooldown = 0;

            if (handDistance < 1)
            {
                newMovement = (forward * handDistance * currentHandMovementSpeed);

                sprintAmount = Mathf.Max(sprintAmount - Time.deltaTime, 0);
            }

            if (sprintAmount / maxSprintSeconds < 0.05f)
            {
                sprintCooldown = true;
                sprintMeter.color = Color.red;
            }
        }
        else
        {
            newMovement = Vector3.zero;

            //Player must stop pumping arms for a second to start recharing sprint meter
            movementCooldown += Time.deltaTime;
            if (movementCooldown > 1)
            {
                sprintAmount = Mathf.Min(sprintAmount + Time.deltaTime, maxSprintSeconds);
            }

            //After depleting spring meter, it must hit mimimumSprintPercent in order to be able to sprint again
            if (sprintCooldown && sprintAmount / maxSprintSeconds > minimumSprintPercent)
            {
                sprintCooldown = false;
            }
            //After depleting sprint cooldown, lerp the color from red back to white to indicate when it's ready
            else if (sprintCooldown)
            {
                sprintMeter.color = Color.Lerp(Color.red, Color.white, (sprintAmount / maxSprintSeconds) / minimumSprintPercent);
            }
        }

        //Update sprint sprite
        sprintMeter.fillAmount = sprintAmount / maxSprintSeconds;

        //Update movement from hands
        movement = Vector3.Lerp(movement, newMovement, 5 * Time.deltaTime);
        ahp.transform.Translate(movement);
    }

    public void EnteredWater()
    {
        inWater = true;
        ahp.maxMoveSpeed = walkSpeed * 0.5f;
        currentHandMovementSpeed = handMovementSpeed * 0.5f;
    }

    public void ExitedWater()
    {
        inWater = false;
        ahp.maxMoveSpeed = walkSpeed;
        currentHandMovementSpeed = handMovementSpeed;
    }
}