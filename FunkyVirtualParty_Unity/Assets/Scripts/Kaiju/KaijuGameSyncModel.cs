using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class KaijuGameSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _state;

    [RealtimeProperty(2, true, true)]
    private int _timer;

    [RealtimeProperty(3, true, true)]
    private bool _vrPlayerReady;

    [RealtimeProperty(4, true, true)]
    private int _grabbedPlayerEvent; //ID of player last grabbed

    [RealtimeProperty(5, true, true)]
    private int _droppedPlayerEvent; //ID of player last dropped
}


