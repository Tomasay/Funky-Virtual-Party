using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBilllboard : MonoBehaviour
{
    [Header("Billboard Axis")]
    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;

    [SerializeField] Vector3 offset;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        /*
        Vector3 rot = transform.rotation.eulerAngles;
        Vector3 camRot = cam.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(x ? camRot.x : rot.x, y ? -camRot.y : rot.y, z ? -camRot.z : rot.z);
        */

        Vector3 lookPos = cam.transform.position - transform.position;
        if (!x) { lookPos.x = 0; }
        if (!y) { lookPos.y = 0; }
        if (!z) { lookPos.z = 0; }
        Quaternion lookRot = Quaternion.LookRotation(lookPos);
        transform.rotation = lookRot * Quaternion.Euler(offset);
    }
}
