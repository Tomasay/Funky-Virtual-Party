using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class SceneChangerSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _currentScene;
}


