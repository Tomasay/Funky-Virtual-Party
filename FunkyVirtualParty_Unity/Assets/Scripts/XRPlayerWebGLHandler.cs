using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRPlayerWebGLHandler : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    void Start()
    {
#if UNITY_WEBGL
        cam.enabled = false;
#endif
    }
}