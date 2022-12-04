using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggerEntered, OnTriggerExited;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExited.Invoke(other);
    }
}