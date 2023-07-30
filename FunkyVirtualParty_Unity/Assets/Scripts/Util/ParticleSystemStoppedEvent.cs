using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemStoppedEvent : MonoBehaviour
{
    public UnityEvent ParticleSystemStopped;

    private void Awake()
    {
        if (ParticleSystemStopped == null)
            ParticleSystemStopped = new UnityEvent();
    }

    private void OnParticleSystemStopped()
    {
        ParticleSystemStopped.Invoke();
    }
}