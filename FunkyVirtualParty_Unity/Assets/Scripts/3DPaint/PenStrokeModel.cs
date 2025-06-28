using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class PenStrokeModel
{
    [RealtimeProperty(1, true, true)]
    private RealtimeArray<LinePointModel> _linePoints;

    [RealtimeProperty(2, false)]
    private Color _lineColor;
}