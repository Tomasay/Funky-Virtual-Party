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
    Right    //From VR perspective, facing right
}

public class FaceBillboarder : MonoBehaviour
{
    [SerializeField]
    public ForwardDirection forwardDirection = ForwardDirection.Forward;
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
            
            float min = 0, max = 0;
            switch (forwardDirection)
            {
                case ForwardDirection.Forward:
                min = 135f; max = 225f;
                break;
                case ForwardDirection.Left:
                min = 225; max = 315;
                break;
                case ForwardDirection.Right:
                min = 45; max = 135;
                break;
            }

            rot.y = Mathf.Clamp(rot.y, min, max);
            transform.rotation = Quaternion.Euler(rot);
        }
    }
}
