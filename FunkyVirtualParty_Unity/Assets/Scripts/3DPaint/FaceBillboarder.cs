using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes;

public enum ForwardDirection
{
    Forward, //From VR perspective, facing VR player
    Left,    //From VR perspective, facing left
    Right,   //From VR perspective, facing right
    Custom   //Uses a custom transform's forward direction, clamped ±45°
}

public class FaceBillboarder : MonoBehaviour
{
    [SerializeField]
    public ForwardDirection forwardDirection = ForwardDirection.Forward;
    public Transform customForwardTransform;
    public bool inheritParentRotationX = false;
    public bool inheritParentRotationY = false;
    public bool inheritParentRotationZ = false;
    Camera vrCam;

void FixedUpdate()
    {
        if(!vrCam && Camera.main)
        {
            vrCam = Camera.main;
        }

        if(vrCam)
        {
            Vector3 lookPos = vrCam.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion lookRot = Quaternion.LookRotation(lookPos);
            Vector3 rot = lookRot.eulerAngles;
            
            switch (forwardDirection)
            {
                case ForwardDirection.Forward:
                    rot.y = Mathf.Clamp(rot.y, 135f, 225f);
                    break;
                case ForwardDirection.Left:
                    rot.y = Mathf.Clamp(rot.y, 225f, 315f);
                    break;
                case ForwardDirection.Right:
                    rot.y = Mathf.Clamp(rot.y, 45f, 135f);
                    break;
                case ForwardDirection.Custom:
                    if (customForwardTransform != null)
                    {
                        // SculptStand FBX has a Z-up import correction (X=-90°), so its local
                        // +Z (forward) points world-up and never changes with Z rotation.
                        // Its local +Y (up) is the horizontal facing direction that rotates with it.
                        Vector3 customFwd = customForwardTransform.up;
                        customFwd.y = 0f;
                        if (customFwd.sqrMagnitude > 0.001f)
                        {
                            float center = Mathf.Atan2(customFwd.x, customFwd.z) * Mathf.Rad2Deg;
                            if (center < 0f) center += 360f;
                            float delta = Mathf.DeltaAngle(rot.y, center);
                            rot.y = center - Mathf.Clamp(delta, -45f, 45f);
                        }
                    }
                    break;
            }
            Quaternion billboardRot = Quaternion.Euler(rot);

            if (transform.parent && (inheritParentRotationX || inheritParentRotationY || inheritParentRotationZ))
            {
                // Derive tilt angles from the parent's up/forward vectors rather than euler angles.
                // eulerAngles decomposition is ZXY-order and sign-flips when the parent has a Y
                // rotation, causing the billboard to tilt the wrong direction.
                float xAngle = 0f, yAngle = 0f, zAngle = 0f;

                if (inheritParentRotationX || inheritParentRotationZ)
                {
                    Vector3 parentUp = transform.parent.up;
                    if (inheritParentRotationX)
                        xAngle = Vector3.SignedAngle(Vector3.up,
                            new Vector3(0f, parentUp.y, parentUp.z).normalized, Vector3.right);
                    if (inheritParentRotationZ)
                        zAngle = Vector3.SignedAngle(Vector3.up,
                            new Vector3(parentUp.x, parentUp.y, 0f).normalized, Vector3.forward);
                }

                if (inheritParentRotationY)
                {
                    Vector3 parentFwd = transform.parent.forward;
                    parentFwd.y = 0f;
                    if (parentFwd.sqrMagnitude > 0.001f)
                        yAngle = Vector3.SignedAngle(Vector3.forward, parentFwd.normalized, Vector3.up);
                }

                transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle) * billboardRot;
            }
            else
            {
                transform.rotation = billboardRot;
            }
        }
    }
}
