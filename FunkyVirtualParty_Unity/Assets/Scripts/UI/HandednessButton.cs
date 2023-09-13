using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.OnScreen;
using System.Runtime.InteropServices;

public class HandednessButton : MonoBehaviour
{
    [SerializeField]
    RectTransform[] transformsToFlip;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetHandednessData();

    [DllImport("__Internal")]
    private static extern void StoreHandednessData(string h);
#endif

    void Start()
    {
        if (TryGetComponent<Button>(out Button b))
        {
            b.onClick.AddListener(delegate { HandednessSwitch(true); });
        }

        //Check local storage for handedness preference
#if UNITY_WEBGL && !UNITY_EDITOR
        string storedHandedness = GetHandednessData();
        Debug.Log("Checking handedness: " + storedHandedness);
        if (storedHandedness != null && storedHandedness.Equals("left"))
        {
            HandednessSwitch(false);
        }
#endif
    }

    void HandednessSwitch(bool storeHandedness)
    {
        foreach (RectTransform rt in transformsToFlip)
        {
            FlipXAxis(rt);

            if(rt.TryGetComponent<ClientJoystick>(out ClientJoystick joy))
            {
                joy.UpdateAnchor();
            }
        }
        FlipXScale(GetComponent<RectTransform>());

        //Store handedness local storage data
#if UNITY_WEBGL && !UNITY_EDITOR
        if(storeHandedness)
        {
            string storedHandedness = GetHandednessData();
            if (storedHandedness == null || storedHandedness.Equals("right"))
            {
                StoreHandednessData("left");
                Debug.Log("Setting handedness to left");
            }
            else if(storedHandedness.Equals("left"))
            {
                StoreHandednessData("right");
                Debug.Log("Setting handedness to right");
            }
        }
#endif
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
