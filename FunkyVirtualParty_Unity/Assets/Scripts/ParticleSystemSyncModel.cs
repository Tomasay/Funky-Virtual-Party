using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ParticleSystemSyncModel
{
    [RealtimeProperty(1, true, true)]
    private bool _playParticles;

    [RealtimeProperty(2, true, true)]
    private bool _stopParticles;

    [RealtimeProperty(3, true, true)]
    private Color _startColor;

    [RealtimeProperty(4, true, true)]
    private Color _materialEmissionColor;

    [RealtimeProperty(5, true, true)]
    private Color _materialTintColor;
}


