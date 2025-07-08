using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using Shapes;
using PaintIn3D;

public class DrawingsSyncer : RealtimeComponent<DrawingsModel>
{
    public static DrawingsSyncer instance;

    public List<List<PolylinePath>> drawingLines;

    public RealtimeArray<DrawingModel> Drawings { get => model.drawings; }

    [SerializeField]
    P3dPaintableTexture paintTexture;

    private void Awake()
    {
        //Singleton
        instance = this;

        drawingLines = new List<List<PolylinePath>>();
    }

    protected override void OnRealtimeModelReplaced(DrawingsModel previousModel, DrawingsModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.drawings.modelAdded -= Drawings_modelAdded;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {

            }

            // Register for events
            currentModel.drawings.modelAdded += Drawings_modelAdded;
        }
    }


    #region Variable Callbacks
    //New drawing created
    private void Drawings_modelAdded(RealtimeArray<DrawingModel> array, DrawingModel model, bool remote)
    {
        drawingLines.Add(new List<PolylinePath>());

        model.penStrokes.modelAdded += PenStrokes_modelAdded;
        model.paintTextureDidChange += Model_paintTextureDidChange;
    }

    //Drawing's paint texture was changed
    private void Model_paintTextureDidChange(DrawingModel model, byte[] value)
    {
#if UNITY_WEBGL //Override final texture on mobile to ensure it is 100% correct
        paintTexture.LoadFromData(value);
#endif
    }

    //New line within a drawing created
    private void PenStrokes_modelAdded(RealtimeArray<PenStrokeModel> array, PenStrokeModel model, bool remote)
    {
        drawingLines[drawingLines.Count-1].Add(new PolylinePath());
        model.linePoints.modelAdded += LinePoints_modelAdded;
    }
    
    //New point within a line create
    private void LinePoints_modelAdded(RealtimeArray<LinePointModel> array, LinePointModel model, bool remote)
    {
        int currentLineIndex = drawingLines[drawingLines.Count-1].Count-1;
        int currentPenStrokeIndex = Drawings[Drawings.Count - 1].penStrokes.Count - 1;
        drawingLines[drawingLines.Count-1][currentLineIndex].AddPoint(model.position, Drawings[Drawings.Count-1].penStrokes[currentPenStrokeIndex].lineColor);
    }
#endregion
}