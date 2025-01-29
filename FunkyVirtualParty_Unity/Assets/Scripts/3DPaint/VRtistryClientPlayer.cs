using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRtistryClientPlayer : ClientPlayer
{
    [SerializeField] public Vector3[] spawnRotations;

    [SerializeField] public Button playerButton;

    protected override void LocalStart()
    {
        base.LocalStart();

        SetSpawnRotation();
        Invoke("SetSitAnim", 3);
    }

    void SetSitAnim()
    {
        animSyncer.Trigger = "Sit1";
    }

    protected void SetSpawnRotation()
    {
        if (syncer.IsDebugPlayer)
        {
            transform.rotation = Quaternion.Euler(spawnRotations[debugPlayerIndex - 9]);
            return;
        }

        transform.rotation = Quaternion.Euler(spawnRotations[realtimeView.ownerIDSelf - 1]);
    }

    public void SetButtonInteractable(bool interactable)
    {
        playerButton.interactable = interactable;
    }
}