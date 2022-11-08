using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandednessButton : MonoBehaviour
{
    [SerializeField]
    RectTransform[] transformsToFlip;

    void Start()
    {
        if (TryGetComponent<Button>(out Button b))
        {
            b.onClick.AddListener(HandednessSwitch);
        }
    }

    void HandednessSwitch()
    {
        foreach (RectTransform rt in transformsToFlip)
        {
            FlipXAxis(rt);
        }
        FlipXScale(GetComponent<RectTransform>());
    }

    void FlipXAxis(RectTransform r)
    {
        Vector3 pos = r.localPosition;
        pos.x = -pos.x;
        r.localPosition = pos;
    }

    void FlipXScale(RectTransform r)
    {
        Vector3 scale = r.localScale;
        scale.x = -scale.x;
        r.localScale = scale;
    }
}