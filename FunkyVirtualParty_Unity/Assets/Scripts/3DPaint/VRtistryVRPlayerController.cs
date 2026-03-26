using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using Autohand.Demo;
using UnityEngine.Animations;
using DG.Tweening;

public class VRtistryVRPlayerController : VRPlayerController
{
    [SerializeField]
    public Hand leftHand, rightHand;

    [SerializeField]
    public Transform cameraHead, trackerOffsetsParent, leftController, rightController, leftHandGrabPoint, rightHandGrabPoint, leftHandRef, rightHandRef;

    [SerializeField]
    public GameObject UIWarningArrow;

    public HandCanvasPointer leftUIPointer, rightUIPointer;

    [SerializeField]
    public PointerVisualizer leftUIPointerPreview, rightUIPointerPreview;

    [HideInInspector]
    public Vector3 spawnPos;

    private GameObject canvas;

    private bool isArrowVisible;

    private void Awake()
    {
        spawnPos = ahp.gameObject.transform.position;
    }

    private void Update()
    {
        if(canvas)
        {
            //Toggle arrow based on if canvas is in view or not
            float rot = cameraHead.rotation.eulerAngles.y;
            if (rot > 180) rot = Mathf.Abs(360 - rot);
            if(rot > 45 && !isArrowVisible)
            {
                UIWarningArrow.transform.DOScale(1, 1);
                isArrowVisible = true;
            }
            else if(rot < 45 && isArrowVisible)
            {
                UIWarningArrow.transform.DOScale(0, 1);
                isArrowVisible = false;
            }
        }
    }

    private void OnDestroy()
    {
        leftUIPointer.GetComponent<HandCanvasPointer>().StartSelect.RemoveAllListeners();
        leftUIPointer.GetComponent<HandCanvasPointer>().StopSelect.RemoveAllListeners();

        rightUIPointer.GetComponent<HandCanvasPointer>().StartSelect.RemoveAllListeners();
        rightUIPointer.GetComponent<HandCanvasPointer>().StopSelect.RemoveAllListeners();
    }

    public void SetCanvas(GameObject c)
    {
        canvas = c;
        LookAtConstraint lac = UIWarningArrow.GetComponentInChildren<LookAtConstraint>();

        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = c.transform;
        src.weight = 1;
        lac.AddSource(src);

        lac.constraintActive = true;
    }
}