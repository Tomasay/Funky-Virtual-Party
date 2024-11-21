using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using Autohand.Demo;

public class VRtistryVRPlayerController : VRPlayerController
{
    [SerializeField]
    public Hand leftHand, rightHand;

    [SerializeField]
    public Transform cameraHead, trackerOffsetsParent, leftController, rightController, leftHandGrabPoint, rightHandGrabPoint, leftHandRef, rightHandRef;

    [SerializeField]
    public GameObject UIPointer;
}