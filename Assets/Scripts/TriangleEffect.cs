using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleEffect : MonoBehaviour
{
    float rotateAmount = 40;
    bool flipTracker;

    void Start()
    {
        transform.Rotate(0, 0, -rotateAmount/2);

        InvokeRepeating("Flip", 0, 1);
    }

    void Flip()
    {
        transform.Rotate(0, 0, flipTracker ? -rotateAmount : rotateAmount);
        flipTracker = !flipTracker;
    }
}