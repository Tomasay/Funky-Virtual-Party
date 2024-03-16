using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Common;

public class LocalLerpFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField]
    float damping;

    void Update()
    {
        if (target)
        {
            float t = CwHelper.DampenFactor(damping, Time.deltaTime);

            transform.localPosition = Vector3.Lerp(transform.localPosition, target.localPosition, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, target.localRotation, t);
        }
    }
}