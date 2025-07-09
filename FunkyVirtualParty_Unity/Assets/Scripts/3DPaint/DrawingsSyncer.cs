using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using Shapes;
using PaintIn3D;
using TMPro;

public class DrawingsSyncer : RealtimeComponent<DrawingsModel>
{
    public static DrawingsSyncer instance;

    public List<List<PolylinePath>> drawingLines;

    public RealtimeArray<DrawingModel> Drawings { get => model.drawings; }

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    P3dPaintableTexture[] galleryPaintTextures;

    [SerializeField]
    GameObject[] galleryArmatures;

    [SerializeField]
    TMP_Text[] galleryTitles;

    private void Awake()
    {
        //Singleton
        instance = this;

        drawingLines = new List<List<PolylinePath>>();
    }

    private void Update()
    {
        Debug.Log("Drawings: " + model.drawings.Count);
    }

    /// <summary>
    /// Stores pose data for the current drawing being worked on
    /// </summary>
    /// <param name="armature">Parent GameObject of the mannequin. Should be named "Armature" in the scene</param>
    public void StorePoseData(GameObject armature)
    {
        foreach (Transform t in armature.GetComponentsInChildren<Transform>())
        {
            JointModel newJointInfo = new JointModel();
            newJointInfo.pos = t.position;
            newJointInfo.rot = t.rotation;

            Drawings[Drawings.Count - 1].poseData.Add(newJointInfo);
        }
    }

    /// <summary>
    /// Applies the pose data of a drawing of a specified index to a specified armature
    /// </summary>
    /// <param name="armature">The armature to apply the pose data to</param>
    /// <param name="drawingIndex">The index of the drawing to use the pose data from</param>
    public void ApplyPoseData(GameObject armature, int drawingIndex)
    {
        Transform[] transforms = armature.GetComponentsInChildren<Transform>();
        for (int i = 0; i < Drawings[drawingIndex].poseData.Count; i++)
        {
            transforms[i].position = Drawings[drawingIndex].poseData[i].pos;
            transforms[i].rotation = Drawings[drawingIndex].poseData[i].rot;
        }
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
        model.poseData.modelAdded += PoseData_modelAdded;
        model.titleDidChange += Model_titleDidChange;
    }

    private void Model_titleDidChange(DrawingModel model, string value)
    {
        if (Drawings.Count >= 1)
        {
            galleryTitles[Drawings.Count - 1].text = value;
        }
    }

    //New pose data added for current drawing
    private void PoseData_modelAdded(RealtimeArray<JointModel> array, JointModel model, bool remote)
    {
        ApplyPoseData(galleryArmatures[Drawings.Count - 1], Drawings.Count - 1);
    }

    //Drawing's paint texture was changed
    private void Model_paintTextureDidChange(DrawingModel model, byte[] value)
    {
#if UNITY_WEBGL //Override final texture on main model on mobile to ensure it is 100% correct
        paintTexture.LoadFromData(value);
#endif

        if (Drawings.Count >= 1)
        {
            galleryPaintTextures[Drawings.Count - 1].LoadFromData(value);
        }
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