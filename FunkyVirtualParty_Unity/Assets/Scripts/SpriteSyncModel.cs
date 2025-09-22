using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class SpriteSyncModel
{
    [RealtimeProperty(1, true, true)]
    private bool _enabled;
}