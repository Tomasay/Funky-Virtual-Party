using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class FireballSyncModel
{
    [RealtimeProperty(1, true, true)]
    private float _currentScale; //Between 0 and 1, indicates level between min and max size

    [RealtimeProperty(2, true, true)]
    private bool _boosted; //Whether or not fireball has been boosted, if so turns blue and launches 3 more fireballs on explosion

    [RealtimeProperty(3, true, true)]
    private bool _active; //Whether or not fireball is visible and active

    [RealtimeProperty(4, true, true)]
    private bool _explosionTrigger;
}


