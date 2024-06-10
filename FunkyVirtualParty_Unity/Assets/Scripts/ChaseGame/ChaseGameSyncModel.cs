using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ChaseGameSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _state;

    [RealtimeProperty(2, true, true)]
    private int _timer;

    [RealtimeProperty(3, true, true)]
    private bool _vrPlayerReady;
}


