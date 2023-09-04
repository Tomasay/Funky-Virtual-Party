using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggerEntered, OnTriggerExited;

    private void Awake()
    {
        if (OnTriggerEntered == null)
            OnTriggerEntered = new UnityEvent<Collider>();

        if (OnTriggerExited == null)
            OnTriggerExited = new UnityEvent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExited.Invoke(other);
    }
}