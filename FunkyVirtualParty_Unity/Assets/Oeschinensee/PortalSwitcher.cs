using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSwitcher : MonoBehaviour
{
    [SerializeField]
    MeshRenderer mr;

    public void SwitchPortal()
    {
        Material[] mats = mr.materials;
        mats[0] = mr.materials[1];
        mats[1] = mr.materials[0];
        mr.materials = mats;
    }

    private void OnTriggerEnter(Collider other)
    {
        SwitchPortal();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchPortal();
    }
}
