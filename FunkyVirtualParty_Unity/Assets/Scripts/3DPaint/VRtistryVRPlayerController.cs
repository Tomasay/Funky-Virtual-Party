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

    [SerializeField]
    public PointerVisualizer UIPointerPreview;

    [HideInInspector]
    public Vector3 spawnPos;

    private void Awake()
    {
        spawnPos = ahp.gameObject.transform.position;
    }

    private void OnDestroy()
    {
        UIPointer.GetComponent<HandCanvasPointer>().StartSelect.RemoveAllListeners();
        UIPointer.GetComponent<HandCanvasPointer>().StopSelect.RemoveAllListeners();
    }
}