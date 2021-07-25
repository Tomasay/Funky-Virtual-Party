using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayerController : MonoBehaviour
{
    private GameObject leftHand, rightHand;

    [SerializeField]
    GameObject forwardDirection;

    private Vector3 leftHandPos, rightHandPos; //Used to store previous frame hand positions
    private float handDistance = 0;
    private float handMovementSpeed = 20;

    public GameObject LeftHand { get => leftHand; set => leftHand = value; }
    public GameObject RightHand { get => rightHand; set => rightHand = value; }

    private bool handMovement = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (handMovement && leftHand && rightHand)
        {
            handDistance = Vector3.Distance(leftHandPos, LeftHand.transform.localPosition) + Vector3.Distance(rightHandPos, RightHand.transform.localPosition);

            leftHandPos = LeftHand.transform.localPosition;
            rightHandPos = RightHand.transform.localPosition;

            if (handDistance < 1 && handDistance > 0)
                transform.position -= forwardDirection.transform.forward * handDistance * handMovementSpeed * Time.deltaTime;
        }
    }
}