using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Normal.Realtime;

public class MannequinHeightSlider : MonoBehaviour
{
    [SerializeField]
    GameObject sculptStand;

    [SerializeField]
    RealtimeTransform linesParent;

    Vector3 linesParentInitialPos;

    [SerializeField]
    float minStandHeight, maxStandHeight;

    private void Awake()
    {
        linesParentInitialPos = linesParent.transform.position;
    }

    public void OnValueChange(float val)
    {
        //Calc new height based on min/max
        float newHeight = math.remap(0, 1, minStandHeight, maxStandHeight, val);

        //Set stand pos
        Vector3 pos = sculptStand.transform.localPosition;
        pos.z = newHeight;
        sculptStand.transform.localPosition = pos;

        //Set lines pos height, making sure it is relative to parent scale of stand
        linesParent.RequestOwnership();
        linesParent.transform.position = linesParentInitialPos + new Vector3(0, newHeight * sculptStand.transform.parent.localScale.x, 0);
    }
}