using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class JointModel
{
    [RealtimeProperty(1, true)]
    private Vector3 _pos;

    [RealtimeProperty(2, true)]
    private Quaternion _rot;
}