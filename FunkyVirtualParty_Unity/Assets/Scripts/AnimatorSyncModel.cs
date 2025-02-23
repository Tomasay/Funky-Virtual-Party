using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class AnimatorSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _trigger;

    [RealtimeProperty(2, true, true)]
    private string _toggleBool;

    [RealtimeProperty(3, true, true)]
    private float _animOffset;
}


