using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class DrawingModel
{
    [RealtimeProperty(1, true)]
    private RealtimeArray<PenStrokeModel> _penStrokes;

    [RealtimeProperty(2, true, true)]
    private RealtimeArray<PaintHitLineModel> _paintHitLines; //Individual paint texture hit lines, used to progressively add to main mannequin

    [RealtimeProperty(3, true, true)]
    private byte[] _paintTexture; //Final paint texture, used to apply to gallery mannequins

    [RealtimeProperty(4, true, true)]
    private RealtimeArray<JointModel> _poseData;

    [RealtimeProperty(5, true, true)]
    private string _title;

    [RealtimeProperty(6, true)]
    private RealtimeArray<PenStrokeModel> _practicePenStrokes;
}