using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MannequinHeightSlider : MonoBehaviour
{
    [SerializeField]
    GameObject sculptStand, mannequin;

    [SerializeField]
    float minStandHeight, maxStandHeight;

    float currentVal;

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, byte>("MethodCallToClientByte", MethodCalledFromServer);
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByte");
#endif
    }

    public void OnValueChange(float val)
    {
        currentVal = val;
        Vector3 pos = sculptStand.transform.localPosition;
        pos.z = math.remap(0, 1, minStandHeight, maxStandHeight, val);
        sculptStand.transform.localPosition = pos;
    }

    public void OnPointerUp()
    {
        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "MannequinHeightChange", (byte)(currentVal*100));
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, byte data)
    {
        Debug.Log("Method called");
        if(methodName.Equals("MannequinHeightChange"))
        {
            Debug.Log(data + " -> " + ((float)data) / 100);
            OnValueChange(((float)data)/100);
        }
    }
#endif
}