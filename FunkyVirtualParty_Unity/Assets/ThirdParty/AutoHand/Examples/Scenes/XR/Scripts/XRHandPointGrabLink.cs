using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
    public class XRHandPointGrabLink : MonoBehaviour{
        public HandDistanceGrabber pointGrab;
        public XRHandControllerLink link;

        [Header("Input")]
        public CommonButton pointInput;
        public CommonButton selectInput;

        public bool autoPoint = false;

        bool pointing;
        bool autoPointing;
        bool selecting;

        void Update(){
            if (autoPoint && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Grabbable"))))
            {
                if (hit.collider.gameObject.GetComponent<DistanceGrabbable>())
                {
                    autoPointing = true;
                    pointGrab.StartPointing();
                }
                else
                {
                    autoPointing = false;
                    pointGrab.StopPointing();
                }
            }
            else if(autoPoint && !pointing)
            {
                autoPointing = false;
                pointGrab.StopPointing();
            }

            if (link.ButtonPressed(pointInput) && !pointing) {
                pointing = true;
                pointGrab.StartPointing();
            }

            if (!link.ButtonPressed(pointInput) && pointing)
            {
                pointing = false;
                pointGrab.StopPointing();
            }

            
            if (link.ButtonPressed(selectInput) && !selecting) {
                selecting = true;
                pointGrab.SelectTarget();
            }
            
            if (!link.ButtonPressed(selectInput) && selecting){
                selecting = false;
                pointGrab.CancelSelect();
            }
        }
    }
}
