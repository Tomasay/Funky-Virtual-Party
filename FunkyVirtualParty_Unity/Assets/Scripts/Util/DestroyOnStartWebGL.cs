using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStartWebGL : MonoBehaviour
{
    [SerializeField]
    Object[] objectsToDisable;

#if UNITY_WEBGL
    void Awake()
    {
        foreach (Object o in objectsToDisable)
        {
            Destroy(o);
        }
    }
#endif
}