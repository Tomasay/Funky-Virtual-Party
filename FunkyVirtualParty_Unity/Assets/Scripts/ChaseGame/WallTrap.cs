using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrap : MonoBehaviour
{
    [SerializeField]
    AnimatorSyncer animSyncer;

    private bool isOpen = true;
    public void OnButtonPressed()
    {
        isOpen = !isOpen;

        animSyncer.Trigger = isOpen ? "Open" : "Close";
    }
}