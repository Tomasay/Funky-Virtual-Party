using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTesting : MonoBehaviour
{
    [SerializeField]
    GameObject uiParent;

    int index = 0;

    public Camera cam;

    public void OnClick()
    {
        index++;

        switch (index)
        {
            case 1:
                uiParent.SetActive(false);
                break;
            default:
                break;
        }
    }
}