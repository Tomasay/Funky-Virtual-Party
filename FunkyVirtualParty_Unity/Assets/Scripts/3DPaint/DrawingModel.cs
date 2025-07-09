using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class DrawingModel
{
    [RealtimeProperty(1, true)]
    private RealtimeArray<PenStrokeModel> _penStrokes;

    [RealtimeProperty(2, true, true)]
    private byte[] _paintTexture;

    [RealtimeProperty(3, true, true)]
    private RealtimeArray<JointModel> _poseData;

    [RealtimeProperty(4, true, true)]
    private string _title;
}