using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class PaintHitLineModel
{
    [RealtimeProperty(1, true, true)]
    bool _preview;

    [RealtimeProperty(2, true, true)]
    int _priority;

    [RealtimeProperty(3, true, true)]
    float _pressure;

    [RealtimeProperty(4, true, true)]
    int _seed;

    [RealtimeProperty(5, true, true)]
    Vector3 _position;

    [RealtimeProperty(6, true, true)]
    Vector3 _endPosition;

    [RealtimeProperty(7, true, true)]
    Quaternion _rotation;

    [RealtimeProperty(8, true, true)]
    bool _clip;

    [RealtimeProperty(9, true, true)]
    Color _color;
}