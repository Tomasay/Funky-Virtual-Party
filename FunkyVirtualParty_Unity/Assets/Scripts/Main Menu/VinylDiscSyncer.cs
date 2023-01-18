using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylDiscSyncer : GrabbableObjectSyncer
{
    [SerializeField] Transform vinylParent;

    Vector3 startingPos;
    Quaternion startingRot;

    float vinylSpinSpeed = 0.2f;
    bool isSpinning;

    protected override void Awake()
    {
        base.Awake();

        startingPos = transform.position;
        startingRot = transform.rotation;
    }

    private void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(0, vinylSpinSpeed, 0);
        }
    }

#if UNITY_WEBGL
    protected override void MethodCalledFromServer(string methodName, byte data)
    {
        base.MethodCalledFromServer(methodName, data);

        if (data == objectID)
        {
            if (methodName.Equals("RespawnDisc"))
            {
                RespawnDisc();
            }
            else if (methodName.Equals("DiscOnPlayer"))
            {
                SetDiscOnPlayer();
            }
        }
    }
#endif

    public void RespawnDisc()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPos;
        transform.rotation = startingRot;
    }

    public void SetDiscOnPlayer()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.position = vinylParent.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        isSpinning = true;
    }
}