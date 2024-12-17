using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;
using TMPro;
using CustomAvatars;
using NaughtyAttributes;

public class ChaseGameVRPlayerController : VRPlayerController
{
    [SerializeField] private float handMovementSpeed = 20, maxSprintSeconds = 2f, handMovementThreshold = 0;
    private float currentHandMovementSpeed;
    [SerializeField] private Image sprintMeter;
    [SerializeField] public ParticleSystemSyncer capturedParticles;

    private RealtimeAvatar realtimeAvatar;

    public TMP_Text vrInfoText, vrGameTimeText;

    private Vector3 movement, newMovement, leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float sprintAmount, minimumSprintPercent = 0.5f;
    private bool handMovement = false, sprintCooldown;
    private float movementCooldown;
    private float walkSpeed;
    
    private bool inWater;

    private bool handsActive;

    public bool HandMovement { get => handMovement; set => handMovement = value; }

    private void Start()
    {
        sprintAmount = (maxSprintSeconds / ClientPlayer.maxClients ) * ClientPlayer.clients.Count;
        maxSprintSeconds = sprintAmount + 0.25f; // minimum half second max 2.25

        walkSpeed = ahp.maxMoveSpeed;
        currentHandMovementSpeed = handMovementSpeed;

        realtimeAvatar = GetComponent<RealtimeAvatar>();
    }

    void Update()
    {
        Vector3 forward = ahp.headCamera.transform.forward;
        forward.y = 0;

        CheckHandsActive();

        if (handMovement && handsActive)
        {
            handDistance = Vector3.Distance(leftHandPos, ahp.handLeft.transform.localPosition) + Vector3.Distance(rightHandPos, ahp.handRight.transform.localPosition);

            leftHandPos = ahp.handLeft.transform.localPosition;
            rightHandPos = ahp.handRight.transform.localPosition;
        }

        if (handMovement && handsActive && !inWater && !sprintCooldown && /*ahp.handLeft.IsSqueezing() && ahp.handRight.IsSqueezing()*/ handDistance > handMovementThreshold)
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

    float timeHandsBecameActive = -1;
    void CheckHandsActive()
    {
        bool bothHandsActive = (realtimeAvatar.LeftHandActive && realtimeAvatar.RightHandActive); //Local parameter for if hands are both active

        if(handsActive && !bothHandsActive)
        {
            handsActive = false;
            timeHandsBecameActive = -1;
        }
        //If hands are active, mark time they became active
        else if (!handsActive && timeHandsBecameActive == -1 && (realtimeAvatar.LeftHandActive && realtimeAvatar.RightHandActive))
        {
            timeHandsBecameActive = Time.time;
            return;
        }

        //If time has marked for when hands became active, set bool to true with a 0.5 second delay
        if(!handsActive && timeHandsBecameActive != -1 && Time.time - timeHandsBecameActive > 0.5f)
        {
            handsActive = true;
            timeHandsBecameActive = -1;
        }
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