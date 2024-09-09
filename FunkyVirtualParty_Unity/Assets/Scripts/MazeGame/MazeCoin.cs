using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using NaughtyAttributes;

public class MazeCoin : MonoBehaviour
{
    public static List<MazeCoin> pool;

    ParentConstraint parentConstraint;

    void Awake()
    {
        //Only host needs to worry about pooling
#if UNITY_ANDROID
        if (pool == null)
        {
            pool = new List<MazeCoin>();
        }
        pool.Add(this);
#endif
    }

    private void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);
    }
}