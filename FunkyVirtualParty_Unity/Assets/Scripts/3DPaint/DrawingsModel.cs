using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class DrawingsModel
{
    [RealtimeProperty(1, true)]
    private RealtimeDictionary<DrawingModel> _drawings;

    [RealtimeProperty(2, true)]
    private DrawingModel _practiceDrawing;
}