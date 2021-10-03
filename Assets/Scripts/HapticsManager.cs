using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager instance;

    List<UnityEngine.XR.InputDevice> rightDevices = new List<UnityEngine.XR.InputDevice>();
    List<UnityEngine.XR.InputDevice> leftDevices = new List<UnityEngine.XR.InputDevice>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, rightDevices);
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, leftDevices);
    }

    public void TriggerHaptic(bool isLeft)
    {
        List <UnityEngine.XR.InputDevice> d = isLeft ? leftDevices : rightDevices;

        foreach (var device in d)
        {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.5f;
                    float duration = 0.5f;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}
