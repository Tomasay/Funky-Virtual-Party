using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passthrough : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
#endif
    }
}
