using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class DrawingModel
{
    [RealtimeProperty(1, true)]
    private RealtimeArray<PenStrokeModel> _penStrokes;
}