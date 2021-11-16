using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDelegate : MonoBehaviour
{
    [SerializeField] UnityEvent collisionEnter, collisionExit, triggerEnter, triggerExit;

    private void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionExit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke();
    }
}