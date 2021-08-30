using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Camera cameraToLookAt;
    [SerializeField] bool transformIsRect = false;

    void Start()
    {
        if (!cameraToLookAt)
        {
            cameraToLookAt = Camera.main;
        }
    }

    void LateUpdate()
    {
        var t = transformIsRect ? GetComponent<RectTransform>() : transform;

        t.LookAt(cameraToLookAt.transform);
        t.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }
}