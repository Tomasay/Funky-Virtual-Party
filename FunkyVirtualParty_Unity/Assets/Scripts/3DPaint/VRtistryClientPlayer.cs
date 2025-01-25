using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRtistryClientPlayer : ClientPlayer
{
    [SerializeField] public Vector3[] spawnRotations;

    protected override void LocalStart()
    {
        base.LocalStart();

        SetSpawnRotation();
        anim.SetTrigger("Sit1");
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
}