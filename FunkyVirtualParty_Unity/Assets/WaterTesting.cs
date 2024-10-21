using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTesting : MonoBehaviour
{
    [SerializeField]
    GameObject uiParent, waterBottom, water2;

    int index = 0;

    public Camera cam;

    public void OnClick()
    {
        index++;

        switch (index)
        {
            case 1:
                //uiParent.SetActive(false);
                break;
            case 2:
                water2.SetActive(false);
                break;
            case 3:
                water2.SetActive(true);
                waterBottom.SetActive(false);
                break;
            case 4:
                water2.SetActive(false);
                break;
            default:
                break;
        }
    }
}