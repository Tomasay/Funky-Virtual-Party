using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MannequinHeightSlider : MonoBehaviour
{
    [SerializeField]
    GameObject sculptStand;

    [SerializeField]
    float minStandHeight, maxStandHeight;

    public void OnValueChange(float val)
    {
        Vector3 pos = sculptStand.transform.localPosition;
        pos.z = math.remap(0, 1, minStandHeight, maxStandHeight, val);
        sculptStand.transform.localPosition = pos;
    }
}