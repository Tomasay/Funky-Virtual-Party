using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ToggleUIPointerVolume : MonoBehaviour
{
#if !UNITY_WEBGL
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            if(other.gameObject.GetComponentInChildren<HandCanvasPointer>(true))
                other.gameObject.GetComponentInChildren<HandCanvasPointer>(true).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            if(other.gameObject.GetComponentInChildren<HandCanvasPointer>(true))
                other.gameObject.GetComponentInChildren<HandCanvasPointer>(true).gameObject.SetActive(true);
        }
    }
#endif
}