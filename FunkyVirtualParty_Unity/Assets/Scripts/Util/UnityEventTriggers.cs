using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventTriggers : MonoBehaviour
{
    public UnityEvent OnAwake, OnStart, OnDestroyed;

    private void Awake()
    {
        if (OnAwake == null)
            OnAwake = new UnityEvent();

        if (OnStart == null)
            OnStart = new UnityEvent();

        if (OnDestroyed == null)
            OnDestroyed = new UnityEvent();

        OnAwake.Invoke();
    }

    void Start()
    {
        OnStart.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyed.Invoke();
    }
}