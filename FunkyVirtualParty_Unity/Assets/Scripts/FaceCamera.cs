using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] public Camera cameraToLookAt;
    [SerializeField] public bool transformIsRect = false;

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

        if (cameraToLookAt)
        {
            t.LookAt(2 * transform.position - cameraToLookAt.transform.position);
        }
    }

    [Button]
    public void UpdateAngle()
    {
        cameraToLookAt = Camera.main;
        var t = transformIsRect ? GetComponent<RectTransform>() : transform;

        t.LookAt(2 * transform.position - cameraToLookAt.transform.position);
    }
}