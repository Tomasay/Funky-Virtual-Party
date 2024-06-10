using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class AnimatorSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _trigger;
}


