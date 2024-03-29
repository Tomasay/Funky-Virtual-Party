using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    public UnityEvent<Collision> OnCollisionEntered, OnCollisionExited, OnCollisionStaying;

    private void Awake()
    {
        if (OnCollisionEntered == null)
            OnCollisionEntered = new UnityEvent<Collision>();

        if (OnCollisionExited == null)
            OnCollisionExited = new UnityEvent<Collision>();

        if (OnCollisionStaying == null)
            OnCollisionStaying = new UnityEvent<Collision>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEntered.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnCollisionExited.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionStaying.Invoke(collision);
    }
}